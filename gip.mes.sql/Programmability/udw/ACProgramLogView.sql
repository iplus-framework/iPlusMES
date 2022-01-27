CREATE VIEW [dbo].[ACProgramLogView]
	AS 
select
	prLog.ACProgramLogID,
	cl.ACClassID,
	cl.BasedOnACClassID,
	cl.ACIdentifier as ACClassACIdentifier,
	cl.ACCaptionTranslation as ACClassACCaptionTranslation,
	prLog.ACUrl,
	pro.ProgramNo as ACProgramProgramNo, 
	po.ProgramNo,
	bt.ProdOrderBatchNo,
	pl.Sequence as PartslistSequence,
	mt.MaterialNo,
	mt.MaterialName1 as MaterialName,
	isnull(pos.Sequence, 0) as Sequence,
	parentPrLog.ACUrl as PosACUrl,
	posOrderLog.ProdOrderPartslistPosID,
	pos.TargetQuantityUOM as InwardTargetQuantityUOM,
	pos.ActualQuantityUOM as InwardActualQuantityUOM,
	childPrLog.ACUrl as RelACUrl,
	relOrderLog.ProdOrderPartslistPosRelationID,
	rel.TargetQuantityUOM as OutwardTargetQuantityUOM,
	rel.ActualQuantityUOM as OutwardActualQuantityUOM
from ACProgramLog prLog 
	inner join ACClass cl on cl.ACClassID = prLog.RefACClassID
	inner join ACProgram pro on pro.ACProgramID = prLog.ACProgramID
	left join ACProgramLog parentPrLog on parentPrLog.ACProgramLogID = prLog.ParentACProgramLogID
	left join OrderLog posOrderLog on posOrderLog.VBiACProgramLogID = parentPrLog.ACProgramLogID
	left join ACProgramLog childPrLog on childPrLog.ParentACProgramLogID = prLog.ACProgramLogID
	left join OrderLog relOrderLog on relOrderLog.VBiACProgramLogID = childPrLog.ACProgramLogID
	left join ProdOrderPartslistPos pos on pos.ProdOrderPartslistPosID = posOrderLog.ProdOrderPartslistPosID
	left join ProdOrderBatch bt on bt.ProdOrderBatchID = pos.ProdOrderBatchID
	left join ProdOrderPartslist pl on pl.ProdOrderPartslistID = pos.ProdOrderPartslistID
	left join Partslist pal on pal.PartslistID = pl.PartslistID
	left join Material mt on mt.MaterialID = pal.MaterialID
	left join ProdOrder po on po.ProdOrderID = pl.ProdOrderID
	left join ProdOrderPartslistPosRelation rel on rel.ProdOrderPartslistPosRelationID = relOrderLog.ProdOrderPartslistPosRelationID