declare @batchNo nvarchar(20)

set @batchNo ='PB10083900';

select
	pos.ProdOrderPartslistPosID,
	bt.ProdOrderBatchNo
from ProdOrderPartslistPos pos
	left join ProdOrderBatch bt on bt.ProdOrderBatchID = pos.ProdOrderBatchID
where
	bt.ProdOrderBatchNo = @batchNo

-- FB00255583

select * from FacilityBooking where FacilityBookingNo = 'FB00255583';
select * from FacilityBookingCharge where FacilityBookingID = 'AEC7DCA0-2A18-4101-8FA0-0B4E6374504C'

select 
	fb.FacilityBookingID,
	fb.FacilityBookingNo,
	inLt.LotNo as inwardLotNo,
	outLot.LotNo as OutwardLotNo

from FacilityBookingCharge fbc 
	left join FacilityCharge inCh on inCh.FacilityChargeID = fbc.InwardFacilityChargeID
	inner join FacilityBooking fb on fb.FacilityBookingID = fbc.FacilityBookingID
	left join FacilityLot inLt on inLt.FacilityLotID = inCh.FacilityLotID

	left join FacilityCharge outCh on outCh.FacilityChargeID = fbc.OutwardFacilityChargeID
	left join FacilityLot outLot on outLot.FacilityLotID = outCh.FacilityLotID

where 
	fb.ProdOrderPartslistPosFacilityLotID = '272898D0-89BA-4FFA-A6FD-366FA71E2831'