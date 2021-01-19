IF EXISTS (SELECT * FROM sysobjects WHERE type = 'V' AND name = 'ProdOrderInwardsView')
	BEGIN
		DROP  View dbo.[ProdOrderInwardsView]
	END
GO

CREATE VIEW [dbo].[ProdOrderInwardsView]
	AS
select
distinct
po.ProgramNo,
pl.Sequence as PartslistSequence,
intermediatePos.Sequence as IntermediateSequence,
intermediateMaterial.MaterialNo as IntermediateMaterial,
batchPos.Sequence as BatchSequence,
bt.ProdOrderBatchNo,
fbc.FacilityBookingChargeNo,
fl.LotNo
from ProdOrder po
	inner join ProdOrderPartslist pl on pl.ProdOrderID = po.ProdOrderID
	inner join ProdOrderPartslistPos intermediatePos on intermediatePos.ProdOrderPartslistID = pl.ProdOrderPartslistID
	inner join Material intermediateMaterial on intermediateMaterial.MaterialID = intermediatePos.MaterialID
	inner join ProdOrderPartslistPos batchPos on batchPos.ParentProdOrderPartslistPosID = intermediatePos.ProdOrderPartslistPosID
	inner join ProdOrderBatch bt on bt.ProdOrderBatchID = batchPos.ProdOrderBatchID
	inner join FacilityBookingCharge fbc on fbc.ProdOrderPartslistPosID = batchPos.ProdOrderPartslistPosID
	inner join FacilityCharge fc on fc.FacilityChargeID = fbc.InwardFacilityChargeID
	inner join FacilityLot fl on fl.FacilityLotID = fc.FacilityLotID
--where
--	(@programNo is null or po.ProgramNo = @programNo)
--	and (@partslistSequence is null or pl.Sequence = @partslistSequence)
--	and (@intermediatePosSequence is null or intermediatePos.Sequence =@intermediatePosSequence)
--	and (@batchSequence is null or batchPos.Sequence = @batchSequence)
--order by
--	po.ProgramNo,
--	pl.Sequence,
--	intermediatePos.Sequence,
--	batchPos.Sequence,
--	bt.ProdOrderBatchNo,
--	fbc.FacilityBookingChargeNo,
--	fl.LotNo
