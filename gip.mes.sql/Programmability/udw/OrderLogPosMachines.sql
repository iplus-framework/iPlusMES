IF EXISTS (SELECT * FROM sysobjects WHERE type = 'V' AND name = 'OrderLogPosMachines')
	BEGIN
		DROP  view  dbo.[OrderLogPosMachines]
	END
GO
CREATE VIEW [dbo].[OrderLogPosMachines]
	AS 
select
	-- order log part
	childPrLog.ACProgramLogID ChildACProgramLogID,
	childPrLog.ACUrl,
	pr.ProgramNo,
	cl.ACClassID,
	cl.ACIdentifier as MachineName,
	cl.BasedOnACClassID,
	baCl.ACIdentifier as BasedOnMachine,
	-- pos
	ol.ProdOrderPartslistPosID,
	targetPos.Sequence as PosSequence,
	bt.ProdOrderBatchNo as PosBatchNo,
	targetPos.ActualQuantityUOM,
	targetPos.InsertDate,
	-- prod order
	pl.Sequence as PartslistSequence,
	po.ProgramNo as ProdOrderProgramNo
from OrderLog ol 
	inner join ACProgramLog prLog on prLog.ACProgramLogID = ol.VBiACProgramLogID
	inner join ACProgram pr on pr.ACProgramID = prLog.ACProgramID
	-- pos
	inner join ProdOrderPartslistPos targetPos on targetPos.ProdOrderPartslistPosID = ol.ProdOrderPartslistPosID
	left  join ProdOrderBatch bt on bt.ProdOrderBatchID = targetPos.ProdOrderBatchID
	inner join ProdOrderPartslist pl on pl.ProdOrderPartslistID = targetPos.ProdOrderPartslistID
	inner join ProdOrder po on po.ProdOrderID = pl.ProdOrderID
	left join ACProgramLog childPrLog on childPrLog.ParentACProgramLogID = prLog.ACProgramLogID
	-- class
	 inner join ACClass cl on cl.ACClassID = childPrLog.RefACClassID
     inner join ACClass baCl on baCl.ACClassID = cl.BasedOnACClassID
where 
	ol.ProdOrderPartslistPosID is not null