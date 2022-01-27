declare @programNo varchar(20);
declare @partslistSequence int;
declare @intermediatePosSequence int;
declare @batchSequence int;

--set @programNo = '3937';
--set @partslistSequence = 2;
--set @intermediatePosSequence = 2;

select
distinct
po.ProgramNo,
pl.Sequence as PartslistSequence,
intermediatePos.Sequence as IntermediateSequence,
intermediateMaterial.MaterialNo as IntermediateMaterial,
batchPos.Sequence as BatchSequence,
bt.ProdOrderBatchNo,
rel.Sequence as RelationSequence,
fbc.FacilityBookingChargeNo,
fl.LotNo
from ProdOrder po
	inner join ProdOrderPartslist pl on pl.ProdOrderID = po.ProdOrderID
	inner join ProdOrderPartslistPos intermediatePos on intermediatePos.ProdOrderPartslistID = pl.ProdOrderPartslistID
	inner join Material intermediateMaterial on intermediateMaterial.MaterialID = intermediatePos.MaterialID
	inner join ProdOrderPartslistPos batchPos on batchPos.ParentProdOrderPartslistPosID = intermediatePos.ProdOrderPartslistPosID
	inner join ProdOrderBatch bt on bt.ProdOrderBatchID = batchPos.ProdOrderBatchID
	inner join ProdOrderPartslistPosRelation rel on rel.TargetProdOrderPartslistPosID = batchPos.ProdOrderPartslistPosID
	inner join FacilityBookingCharge fbc on fbc.ProdOrderPartslistPosRelationID = rel.ProdOrderPartslistPosRelationID
	inner join FacilityCharge fc on fc.FacilityChargeID = fbc.OutwardFacilityChargeID
	inner join FacilityLot fl on fl.FacilityLotID = fc.FacilityLotID
where
	(@programNo is null or po.ProgramNo = @programNo)
	and (@partslistSequence is null or pl.Sequence = @partslistSequence)
	and (@intermediatePosSequence is null or intermediatePos.Sequence =@intermediatePosSequence)
	and (@batchSequence is null or batchPos.Sequence = @batchSequence)
order by
	po.ProgramNo,
	pl.Sequence,
	intermediatePos.Sequence,
	batchPos.Sequence,
	bt.ProdOrderBatchNo,
	rel.Sequence,
	fbc.FacilityBookingChargeNo,
	fl.LotNo
