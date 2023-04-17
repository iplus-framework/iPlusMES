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