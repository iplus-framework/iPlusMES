IF EXISTS (SELECT * FROM sysobjects WHERE type = 'FN' AND name = 'udfConvertToUnit')
	BEGIN
		DROP  function  dbo.[udfConvertToUnit]
	END
GO
CREATE FUNCTION [dbo].[udfConvertToUnit]
(
	@quantity float,
	@fromMDUnitID uniqueidentifier,
	@toMDUnitID uniqueidentifier,
	@baseMDUnit uniqueidentifier
)
RETURNS float
AS
BEGIN
	declare @convertedQuantity float;
	set @convertedQuantity = 0;
	if @quantity is not null and @quantity > 0 and @fromMDUnitID is not null and @toMDUnitID is not null
	BEGIN
		IF @fromMDUnitID = @toMDUnitID
		BEGIN
			SET @convertedQuantity = @quantity;
		END
		IF @fromMDUnitID <> @toMDUnitID
		BEGIN
			declare @calculationTable table
			(
				ID int NOT NULL identity(1,1),
				Multiplier float,
				Divisor float,
				CalcQuantity float
			);
			insert into @calculationTable(Multiplier,Divisor)
			select top 1
				Mc.Multiplier as Multiplier,
				Mc.Divisor as Divisor
			from MDUnitConversion Mc
			where Mc.MDUnitID = @fromMDUnitID and Mc.ToMDUnitID = @toMDUnitID;
			insert into @calculationTable(Multiplier,Divisor)
			select top 1
				Mc.Divisor as Multiplier,
				Mc.Multiplier as Divisor
			from MDUnitConversion Mc
			where Mc.MDUnitID = @toMDUnitID and Mc.ToMDUnitID = @fromMDUnitID;
			if (select COUNT(ID) from @calculationTable) = 0
			begin
				insert into @calculationTable(Multiplier, Divisor)
				select top 1
					Mc.Multiplier as Multiplier,
					Mc.Divisor as Divisor
				from MDUnitConversion Mc
				where Mc.MDUnitID = @fromMDUnitID and Mc.ToMDUnitID = @baseMDUnit;
				insert into @calculationTable(Multiplier, Divisor)
				select top 1
					Mc.Divisor as Multiplier,
					Mc.Multiplier as Divisor
				from MDUnitConversion Mc
				where Mc.MDUnitID = @baseMDUnit and Mc.ToMDUnitID = @fromMDUnitID;
				insert into @calculationTable(Multiplier, Divisor)
				select top 1
					Mc.Multiplier as Multiplier,
					Mc.Divisor as Divisor
				from MDUnitConversion Mc
				where Mc.MDUnitID = @baseMDUnit and Mc.ToMDUnitID = @toMDUnitID;
				insert into @calculationTable(Multiplier, Divisor)
				select top 1
					Mc.Divisor as Multiplier,
					Mc.Multiplier as Divisor
				from MDUnitConversion Mc
				where Mc.MDUnitID = @toMDUnitID and Mc.ToMDUnitID = @baseMDUnit;
			end
			-- update first step
			update @calculationTable
			set CalcQuantity = @quantity * Divisor / Multiplier
			where ID = 1;
			-- update multiple steps
			UPDATE t1
			SET t1.CalcQuantity = t2.CalcQuantity * t1.Divisor / t1.Multiplier
			FROM @calculationTable AS t1
			INNER JOIN @calculationTable AS t2
			ON t1.ID = t2.ID + 1
			WHERE t1.ID > 1
			set @convertedQuantity = (select top 1 CalcQuantity from @calculationTable order by ID desc);
		END
	END
	RETURN isnull(@convertedQuantity, 0);
END
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'udpRecalcActualQuantity')
	BEGIN
		DROP  procedure  dbo.[udpRecalcActualQuantity]
	END
GO
CREATE PROCEDURE [dbo].[udpRecalcActualQuantity]
				
	@programNo nvarchar(50),
	@prodOrderPartslistID uniqueidentifier
	
AS
begin
	begin tran
	declare @jobID uniqueidentifier;
	set @jobID = NEWID();
	insert into dbo.JobTableRecalcActualQuantity (JobID, ItemID, ItemType)
	select 
	distinct 
		@jobID as JobID,
		Rel.ProdOrderPartslistPosRelationID as ItemID,
		case when Rel.ParentProdOrderPartslistPosRelationID is null then 'rel' else 'rel-batch' end as ItemType
	from ProdOrder Pr
		left join ProdOrderPartslist Pl on Pl.ProdOrderID = Pr.ProdOrderID
		left join ProdOrderPartslistPos Pos on Pos.ProdOrderPartslistID = Pl.ProdOrderPartslistID
		left join ProdOrderPartslistPosRelation Rel on Rel.SourceProdOrderPartslistPosID = Pos.ProdOrderPartslistPosID
	where 
		Pr.ProgramNo = @programNo
		and (@prodOrderPartslistID is null or Pl.ProdOrderPartslistID = @prodOrderPartslistID)
		and Rel.ProdOrderPartslistPosRelationID is not null 
	insert into dbo.JobTableRecalcActualQuantity (JobID, ItemID, ItemType)
	select
	distinct 
		@jobID as JobID,
		Pos.ProdOrderPartslistPosID as ItemID,
		case when Pos.ParentProdOrderPartslistPosID is null then 'pos' else 'pos-batch' end as ItemType
	from ProdOrder Pr
		left join ProdOrderPartslist Pl on Pl.ProdOrderID = Pr.ProdOrderID
		left join ProdOrderPartslistPos Pos on Pos.ProdOrderPartslistID = Pl.ProdOrderPartslistID
	where 
		Pr.ProgramNo = @programNo
		and (@prodOrderPartslistID is null or Pl.ProdOrderPartslistID = @prodOrderPartslistID)
	-- select * from #recalcActualQuantityJobTable;
	-- update ActualQuantityUOM relation childs
	update Rel
	set 
		Rel.ActualQuantityUOM = isnull((select sum(Fbc.OutwardQuantityUOM) from FacilityBookingCharge Fbc inner join FacilityBooking Fb on Fb.FacilityBookingID = Fbc.FacilityBookingID where Fb.ProdOrderPartslistPosRelationID = Rel.ProdOrderPartslistPosRelationID),0)
	from ProdOrderPartslistPosRelation Rel 
	join  dbo.JobTableRecalcActualQuantity Tmp on Tmp.ItemID = Rel.ProdOrderPartslistPosRelationID
	where Tmp.JobID  = @jobID and Tmp.ItemType = 'rel-batch'
	-- update ActualQuantityUOM relation parents
	update Rel
	set 
		Rel.ActualQuantityUOM = 
		isnull((select sum(RelP.ActualQuantityUOM) from ProdOrderPartslistPosRelation RelP where RelP.ParentProdOrderPartslistPosRelationID = Rel.ProdOrderPartslistPosRelationID), 0) + 
		isnull((select sum(Fbc.OutwardQuantityUOM) from FacilityBookingCharge Fbc inner join FacilityBooking Fb on Fb.FacilityBookingID = Fbc.FacilityBookingID where Fb.ProdOrderPartslistPosRelationID = Rel.ProdOrderPartslistPosRelationID),0)
	from ProdOrderPartslistPosRelation Rel 
	join  dbo.JobTableRecalcActualQuantity Tmp on Tmp.ItemID = Rel.ProdOrderPartslistPosRelationID
	where Tmp.JobID  = @jobID and Tmp.ItemType = 'rel'
	-- update ActualQuantityUOM pos children
	update Pos
	set Pos.ActualQuantityUOM = 
		isnull((select sum(isnull(Fbc.OutwardQuantityUOM, 0)) + sum(isnull(Fbc.InwardQuantityUOM, 0))   from FacilityBookingCharge Fbc inner join FacilityBooking Fb on Fb.FacilityBookingID = Fbc.FacilityBookingID where Fb.ProdOrderPartslistPosID = Pos.ProdOrderPartslistPosID),0)	
	FROM ProdOrderPartslistPos Pos
	join dbo.JobTableRecalcActualQuantity Tmp on Tmp.ItemID = Pos.ProdOrderPartslistPosID
	where Tmp.JobID = @jobID and Tmp.ItemType = 'pos-batch'
	-- update ActualQuantityUOM pos parents 
	update Pos
	set Pos.ActualQuantityUOM = 
		isnull((select sum(PosP.ActualQuantityUOM) from ProdOrderPartslistPos PosP where PosP.ParentProdOrderPartslistPosID = Pos.ProdOrderPartslistPosID), 0) + 
		isnull((select sum(isnull(Fbc.OutwardQuantityUOM, 0)) + sum(isnull(Fbc.InwardQuantityUOM, 0))   from FacilityBookingCharge Fbc inner join FacilityBooking Fb on Fb.FacilityBookingID = Fbc.FacilityBookingID where Fb.ProdOrderPartslistPosID = Pos.ProdOrderPartslistPosID),0)
	FROM ProdOrderPartslistPos Pos
	join dbo.JobTableRecalcActualQuantity Tmp on Tmp.ItemID = Pos.ProdOrderPartslistPosID
	where Tmp.JobID = @jobID and Tmp.ItemType = 'pos'
	
	-- update ActualQuantityUOM pos  - add Soruce relation quanitites 
	update Pos
	set Pos.ActualQuantityUOM = Pos.ActualQuantityUOM +
		isnull((select sum(Rel.ActualQuantityUOM) from ProdOrderPartslistPosRelation Rel where Rel.SourceProdOrderPartslistPosID = Pos.ProdOrderPartslistPosID and Rel.ParentProdOrderPartslistPosRelationID is null),0)
	FROM ProdOrderPartslistPos Pos
	join dbo.JobTableRecalcActualQuantity Tmp on Tmp.ItemID = Pos.ProdOrderPartslistPosID
	where Tmp.JobID = @jobID and Tmp.ItemType = 'pos' and Pos.MaterialPosTypeIndex = 2110
	
	--update CalledUpQuantityUOM pos  - sum of children ActualQuantityUOM if exist else of TargetQuantityUOM
	 --1220 1120 1210 -> order sequence
	update Pos
	set Pos.CalledUpQuantityUOM = isnull((select sum(case when ChildPos.ActualQuantityUOM > 0.000001 then ChildPos.ActualQuantityUOM else ChildPos.TargetQuantityUOM end) from ProdOrderPartslistPos ChildPos where ChildPos.ParentProdOrderPartslistPosID = Pos.ProdOrderPartslistPosID), 0)
	FROM ProdOrderPartslistPos Pos
	inner join 
	(select 
	row_number() OVER (ORDER BY
	  CASE WHEN Pos.MaterialPosTypeIndex = 1220 THEN 1 
		   WHEN Pos.MaterialPosTypeIndex = 1120 THEN 2
		   WHEN Pos.MaterialPosTypeIndex = 1210 THEN 3
	END asc) as RB,
	Pos.ProdOrderPartslistPosID 
	FROM ProdOrderPartslistPos Pos
	join dbo.JobTableRecalcActualQuantity Tmp on Tmp.ItemID = Pos.ProdOrderPartslistPosID
	where Tmp.JobID = @jobID and Tmp.ItemType = 'pos' and Pos.MaterialPosTypeIndex in (1120, 1210, 1220)
	) tt on tt.ProdOrderPartslistPosID = Pos.ProdOrderPartslistPosID
	-- update ActualQuantity Relation
	update Rel
	set 
		Rel.ActualQuantity = dbo.udfConvertToUnit(Rel.ActualQuantityUOM, Mt.BaseMDUnitID, isnull(Pos.MDUnitID, mt.BaseMDUnitID),mt.BaseMDUnitID)
	from ProdOrderPartslistPosRelation Rel 
	join  dbo.JobTableRecalcActualQuantity Tmp on Tmp.ItemID = Rel.ProdOrderPartslistPosRelationID
	join ProdOrderPartslistPos Pos on Pos.ProdOrderPartslistPosID = rel.SourceProdOrderPartslistPosID
	join Material Mt on Pos.MaterialID = Mt.MaterialID
	where Tmp.JobID  = @jobID and (Tmp.ItemType = 'rel-batch' or Tmp.ItemType = 'rel')
	-- update ActualQuantity Pos
	update Pos
	set Pos.ActualQuantity = dbo.udfConvertToUnit(Pos.ActualQuantityUOM, Mt.BaseMDUnitID, isnull(Pos.MDUnitID, mt.BaseMDUnitID),mt.BaseMDUnitID)
	FROM ProdOrderPartslistPos Pos
	join dbo.JobTableRecalcActualQuantity Tmp on Tmp.ItemID = Pos.ProdOrderPartslistPosID
	join Material Mt on Pos.MaterialID = Mt.MaterialID
	where Tmp.JobID = @jobID and (Tmp.ItemType = 'pos-batch' or Tmp.ItemType = 'pos')
	-- update CalledUpQuantity Pos
	update Pos
	set Pos.CalledUpQuantity = dbo.udfConvertToUnit(Pos.CalledUpQuantityUOM, Mt.BaseMDUnitID, isnull(Pos.MDUnitID, mt.BaseMDUnitID),mt.BaseMDUnitID)
	FROM ProdOrderPartslistPos Pos
	join dbo.JobTableRecalcActualQuantity Tmp on Tmp.ItemID = Pos.ProdOrderPartslistPosID
	join Material Mt on Pos.MaterialID = Mt.MaterialID
	where Tmp.JobID = @jobID and Tmp.ItemType = 'pos' and Pos.MaterialPosTypeIndex in (1120, 1210, 1220);
	-- update ProdOrderParstslist ActualQuantity
	update Pl
	set Pl.ActualQuantity = dbo.udfConvertToUnit(
		(	select 
				top 1 Pos.ActualQuantityUOM 
			from ProdOrderPartslistPos Pos 
			where 
				Pos.ProdOrderPartslistID = Pl.ProdOrderPartslistID 
				and Pos.MaterialPosTypeIndex = 1120 
				and (select count(*) from MaterialWFRelation Mwr where Mwr.SourceMaterialID = pos.MaterialID and Mwr.SourceMaterialID != Mwr.TargetMaterialID) = 0
		)
	, Mt.BaseMDUnitID, isnull(P.MDUnitID, mt.BaseMDUnitID),mt.BaseMDUnitID)
	from ProdOrderPartslist Pl
	inner join ProdOrder Po on Po.ProdOrderID  = Pl.ProdOrderID
	inner join Partslist P on P.PartslistID = Pl.PartslistID
	inner join Material Mt on Mt.MaterialID  = P.MaterialID
	where 
	Po.ProgramNo = @programNo
	and (@prodOrderPartslistID is null or Pl.ProdOrderPartslistID = @prodOrderPartslistID)
	delete from dbo.JobTableRecalcActualQuantity where JobID = @jobID;
	commit tran;
end