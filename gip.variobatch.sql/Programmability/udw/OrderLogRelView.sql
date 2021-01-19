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