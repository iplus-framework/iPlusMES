

declare @facilityBookingNo varchar(20);

set @facilityBookingNo = 'FB00197162';

select
	Fb.FacilityBookingNo,
	fb.InsertDate,
	fb.ProdOrderPartslistPosID,
	pos.Sequence as PosSequence,
	pos.MaterialPosTypeIndex,
	pl.Sequence as PartslistSequence,
	po.ProgramNo
from FacilityBooking fb
	inner join ProdOrderPartslistPos pos on pos.ProdOrderPartslistPosID = fb.ProdOrderPartslistPosID
	inner join ProdOrderPartslist pl on pl.ProdOrderPartslistID = pos.ProdOrderPartslistID
	inner join ProdOrder po on po.ProdOrderID = pl.ProdOrderID
where fb.FacilityBookingNo = @facilityBookingNo

select
	Fb.FacilityBookingNo,
	fb.InsertDate,
	fb.ProdOrderPartslistPosRelationID,
	rel.Sequence as RelationSequence,
	pos.Sequence as PosSequence,
	pos.MaterialPosTypeIndex,
	pl.Sequence as PartslistSequence,
	po.ProgramNo
from FacilityBooking fb
	inner join ProdOrderPartslistPosRelation rel on rel.ProdOrderPartslistPosRelationID = fb.ProdOrderPartslistPosRelationID
	inner join ProdOrderPartslistPos pos on pos.ProdOrderPartslistPosID = rel.TargetProdOrderPartslistPosID
	inner join ProdOrderPartslist pl on pl.ProdOrderPartslistID = pos.ProdOrderPartslistID
	inner join ProdOrder po on po.ProdOrderID = pl.ProdOrderID
where fb.FacilityBookingNo = @facilityBookingNo