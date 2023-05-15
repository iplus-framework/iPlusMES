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