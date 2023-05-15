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