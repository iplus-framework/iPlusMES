-- OrderLogPosView
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'V' AND name = 'OrderLogPosView')
	BEGIN
		DROP  view  dbo.[OrderLogPosView]
	END
GO
CREATE VIEW [dbo].[OrderLogPosView]
	AS 
select
	-- order log part
	prLog.ACProgramLogID,
	pr.ProgramNo,
	prLog.ACUrl,
	-- pos
	ol.ProdOrderPartslistPosID,
	targetPos.Sequence as PosSequence,
	bt.ProdOrderBatchNo as PosBatchNo,
	targetPos.MaterialPosTypeIndex,
	targetPosMt.MaterialNo as PosMaterialNo,
	targetPosMt.MaterialName1 as PosMaterialName,
	targetPos.TargetQuantityUOM as PosTargetQuantityUOM,
	targetPos.ActualQuantityUOM as PosActualQuantityUOM,
	-- prod order
	pl.Sequence as PartslistSequence,
	po.ProgramNo as ProdOrderProgramNo
from OrderLog ol 
	inner join ACProgramLog prLog on prLog.ACProgramLogID = ol.VBiACProgramLogID
	inner join ACProgram pr on pr.ACProgramID = prLog.ACProgramID
	-- pos
	inner join ProdOrderPartslistPos targetPos on targetPos.ProdOrderPartslistPosID = ol.ProdOrderPartslistPosID
	inner join Material targetPosMt on targetPosMt.MaterialID = targetPos.MaterialID
	left  join ProdOrderBatch bt on bt.ProdOrderBatchID = targetPos.ProdOrderBatchID
	inner join ProdOrderPartslist pl on pl.ProdOrderPartslistID = targetPos.ProdOrderPartslistID
	inner join ProdOrder po on po.ProdOrderID = pl.ProdOrderID
where 
	ol.ProdOrderPartslistPosID is not null
GO
-- MachineMaterialPosView
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'V' AND name = 'MachineMaterialPosView')
	BEGIN
		DROP  view  dbo.[MachineMaterialPosView]
	END
GO
CREATE VIEW [dbo].[MachineMaterialPosView]
	AS 
select 
	t.ProgramNo,
	t.Sequence,
	t.MaterialNo,
	t.MaterialName1,
	t.ACClassID,
	t.MachineName,
	t.BasedOnACClassID,
	t.BasedOnMachineName,
	sum(t.InwardTargetQuantityUOM) as InwardTargetQuantityUOM,
	sum(t.InwardActualQuantityUOM) as InwardActualQuantityUOM
from
(
	select 
	po.ProgramNo
	,poPl.Sequence
	,bt.ProdOrderBatchNo
	,mt.MaterialNo
	,mt.MaterialName1
	,pos.TargetQuantityUOM as InwardTargetQuantityUOM
	,pos.ActualQuantityUOM as InwardActualQuantityUOM
	,cl.ACClassID
	,cl.ACIdentifier as MachineName
	,baCl.ACClassID as BasedOnACClassID
	,baCl.ACIdentifier as BasedOnMachineName
	from ProdOrder po 
	inner join ProdOrderPartslist poPl on poPl.ProdOrderID = po.ProdOrderID
	inner join Partslist pl on pl.PartslistID = poPl.PartslistID
	inner join Material mt on mt.MaterialID = pl.MaterialID
	inner join ProdOrderPartslistPos pos on pos.ProdOrderPartslistID = poPl.ProdOrderPartslistID
	inner join ProdOrderBatch bt on bt.ProdOrderBatchID = pos.ProdOrderBatchID
	
	inner join OrderLog ol on ol.ProdOrderPartslistPosID = pos.ProdOrderPartslistPosID
	inner join ACProgramLog prLog on prLog.ACProgramLogID = ol.VBiACProgramLogID
	inner join ACProgramLog chPrLog on chPrLog.ParentACProgramLogID = prLog.ACProgramLogID
	inner join ACClass cl on cl.ACClassID = chPrLog.RefACClassID
	inner join ACClass baCl on baCl.ACClassID = cl.BasedOnACClassID
	where 
		--po.ProgramNo = @programNo 
		--and poPl.Sequence = @sequence
		baCl.ACIdentifier not  in (N'Dosierwaage', N'Packmaschine', N'MWaage', N'KontrollWaage') 
	) t
	group by
	t.ProgramNo,
	t.Sequence,
	t.MaterialNo,
	t.MaterialName1,
	t.ACClassID,
	t.MachineName,
	t.BasedOnACClassID,
	t.BasedOnMachineName
	--order by t.Sequence, t.MachineName
GO
-- MachineMaterialRelView
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'V' AND name = 'MachineMaterialRelView')
	BEGIN
		DROP  view  dbo.[MachineMaterialRelView]
	END
GO
CREATE VIEW [dbo].[MachineMaterialRelView]
	AS 
select 
	t.ProgramNo,
	t.Sequence,
	t.MaterialNo,
	t.MaterialName1,
	t.ACClassID,
	t.MachineName,
	t.BasedOnACClassID,
	t.BasedOnMachineName,
	sum(t.OutwardTargetQuantityUOM) as OutwardTargetQuantityUOM,
	sum(t.OutwardActualQuantityUOM) as OutwardActualQuantityUOM
from
(
	select 
	po.ProgramNo
	,poPl.Sequence
	,bt.ProdOrderBatchNo
	,mt.MaterialNo
	,mt.MaterialName1
	,sum(rel.TargetQuantity) as OutwardTargetQuantityUOM
	,sum(rel.ActualQuantityUOM) as OutwardActualQuantityUOM
	,oll.ACClassID
	,oll.MachineName
	,oll.BasedOnACClassID
	,oll.BasedOnMachineName
	from ProdOrder po 
	inner join ProdOrderPartslist poPl on poPl.ProdOrderID = po.ProdOrderID
	inner join Partslist pl on pl.PartslistID = poPl.PartslistID
	inner join Material mt on mt.MaterialID = pl.MaterialID
	inner join ProdOrderPartslistPos pos on pos.ProdOrderPartslistID = poPl.ProdOrderPartslistID
	inner join ProdOrderBatch bt on bt.ProdOrderBatchID = pos.ProdOrderBatchID
	
	left join ProdOrderPartslistPosRelation rel on rel.TargetProdOrderPartslistPosID =  pos.ProdOrderPartslistPosID
	inner join
	(select distinct
		ol.ProdOrderPartslistPosRelationID,
		cl.ACClassID,
		cl.ACIdentifier as MachineName,
		baCl.ACClassID as BasedOnACClassID,
		baCl.ACIdentifier as BasedOnMachineName
		from
		OrderLog ol 
		inner join ACProgramLog prLog on prLog.ACProgramLogID = ol.VBiACProgramLogID
		inner join ACProgramLog paPrLog on paPrLog.ACProgramLogID = prLog.ParentACProgramLogID
		inner join ACClass cl on cl.ACClassID = paPrLog.RefACClassID
		inner join ACClass baCl on baCl.ACClassID = cl.BasedOnACClassID
	) oll on oll.ProdOrderPartslistPosRelationID = rel.ProdOrderPartslistPosRelationID
	--where 
	--	po.ProgramNo = @programNo 
	group by
	po.ProgramNo
	,poPl.Sequence
	,bt.ProdOrderBatchNo
	,mt.MaterialNo
	,mt.MaterialName1
	,oll.ACClassID
	,oll.MachineName
	,oll.BasedOnACClassID
	,oll.BasedOnMachineName
		--and poPl.Sequence = @sequence
		--and baCl.ACIdentifier  in (N'Dosierwaage', N'Packmaschine', N'MWaage', N'KontrollWaage') 
	) t
	group by
	t.ProgramNo,
	t.Sequence,
	t.MaterialNo,
	t.MaterialName1,
	t.ACClassID,
	t.MachineName,
	t.BasedOnACClassID,
	t.BasedOnMachineName
	--order by t.Sequence, t.MachineName
GO
-- MachineMaterialView
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'V' AND name = 'MachineMaterialView')
	BEGIN
		DROP  view  dbo.[MachineMaterialView]
	END
GO
CREATE VIEW [dbo].[MachineMaterialView]
	AS 
select
		ROW_NUMBER() over (order by t.ProgramNo, t.PartslistSequence, t.MachineName) as Nr,
		t.ProgramNo,
		t.MaterialNo,
		t.MaterialName1,
		t.ACClassID,
		t.MachineName,
		t.BasedOnACClassID,
		t.BasedOnMachineName,
		t.InwardTargetQuantityUOM,
		t.InwardActualQuantityUOM,
		t.OutwardTargetQuantityUOM,
		t.OutwardActualQuantityUOM
	from
	(
		select 
			mmvPos.ProgramNo,
			mmvPos.Sequence as PartslistSequence,
			mmvPos.MaterialNo,
			mmvPos.MaterialName1,
			mmvPos.ACClassID,
			mmvPos.MachineName,
			mmvPos.BasedOnACClassID,
			mmvPos.BasedOnMachineName,
			0 as OutwardTargetQuantityUOM,
			0 as OutwardActualQuantityUOM,
			mmvPos.InwardTargetQuantityUOM,
			mmvPos.InwardActualQuantityUOM
		from MachineMaterialPosView mmvPos
		union
		select 
			mmvRel.ProgramNo,
			mmvRel.Sequence as PartslistSequence,
			mmvRel.MaterialNo,
			mmvRel.MaterialName1,
			mmvRel.ACClassID,
			mmvRel.MachineName,
			mmvRel.BasedOnACClassID,
			mmvRel.BasedOnMachineName,
			mmvRel.OutwardTargetQuantityUOM,
			mmvRel.OutwardActualQuantityUOM,
			0 as InwardTargetQuantityUOM,
			0 as InwardActualQuantityUOM
		from MachineMaterialRelView mmvRel
	) t
GO
-- OrderLogRelView
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'V' AND name = 'OrderLogRelView')
	BEGIN
		DROP  view  dbo.[OrderLogRelView]
	END
GO
CREATE VIEW [dbo].[OrderLogRelView]
	AS 
select
	-- order log part
	prLog.ACProgramLogID,
	pr.ProgramNo,
	ol.ProdOrderPartslistPosRelationID,
	prLog.ACUrl,
	parentPrLog.RefACClassID,
	cl.ACClassID,
	cl.ACIdentifier as MachineName,
	cl.BasedOnACClassID,
	baCl.ACIdentifier as BasedOnMachine,
	-- relation
	rel.Sequence as RelSequence,
	sourcePosMt.MaterialNo,
	sourcePosMt.MaterialName1,
	rel.ActualQuantityUOM,
	rel.TargetQuantityUOM,
	-- pos
	targetPos.Sequence as PosSequence,
	bt.ProdOrderBatchNo as PosBatchNo,
	targetPos.MaterialPosTypeIndex,
	targetPosMt.MaterialNo as PosMaterialNo,
	targetPosMt.MaterialName1 as PosMaterialName,
	targetPos.TargetQuantityUOM as PosTargetQuantityUOM,
	targetPos.ActualQuantityUOM as PosActualQuantityUOM,
	targetPos.InsertDate,
	-- prod order
	pl.Sequence as PartslistSequence,
	po.ProgramNo as ProdOrderProgramNo
from OrderLog ol 
 inner join ACProgramLog prLog on prLog.ACProgramLogID = ol.VBiACProgramLogID
 inner join ACProgram pr on pr.ACProgramID = prLog.ACProgramID
 inner join ACProgramLog parentPrLog on parentPrLog.ACProgramLogID = prLog.ParentACProgramLogID
 inner join ACClass cl on cl.ACClassID = parentPrLog.RefACClassID
 inner join ACClass baCl on baCl.ACClassID = cl.BasedOnACClassID
 -- rel
 inner join ProdOrderPartslistPosRelation rel on rel.ProdOrderPartslistPosRelationID = ol.ProdOrderPartslistPosRelationID
 -- pos
 inner join ProdOrderPartslistPos targetPos on targetPos.ProdOrderPartslistPosID = rel.TargetProdOrderPartslistPosID
 inner join Material targetPosMt on targetPosMt.MaterialID = targetPos.MaterialID
 left  join ProdOrderBatch bt on bt.ProdOrderBatchID = targetPos.ProdOrderBatchID
 inner join ProdOrderPartslist pl on pl.ProdOrderPartslistID = targetPos.ProdOrderPartslistID
 inner join ProdOrder po on po.ProdOrderID = pl.ProdOrderID
 -- pos source
 inner join ProdOrderPartslistPos sourcePos on sourcePos.ProdOrderPartslistPosID = rel.SourceProdOrderPartslistPosID
 inner join Material sourcePosMt on sourcePosMt.MaterialID = sourcePos.MaterialID
where 
	ol.ProdOrderPartslistPosRelationID is not null