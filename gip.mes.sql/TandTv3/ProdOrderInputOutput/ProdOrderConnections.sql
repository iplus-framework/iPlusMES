

select
	distinct
	poInward.ProgramNo as InwardProgramNo,
	poInward.PartslistSequence as InwardPartslistSequence,
	--poInward.IntermediateSequence as InwardIntermediateSequence,
	--poInward.ProdOrderBatchNo as InwardProdOrderBatchNo,
	poInward.LotNo as InwardLotNo,

	poOutward.ProgramNo as OutwardProgramNo,
	poOutward.PartslistSequence as OutwardPartslistSequence
	--poOutward.IntermediateSequence as OutwardIntermediateSequence,
	--poOutward.ProdOrderBatchNo as OutwardProdOrderBatchNo
from 
ProdOrderInwardsView poInward
left join ProdOrderOutwardsView poOutward on poOutward.LotNo = poInward.LotNo
where poInward.ProgramNo <> poOutward.ProgramNo