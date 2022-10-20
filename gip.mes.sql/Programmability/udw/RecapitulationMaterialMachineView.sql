CREATE VIEW [dbo].[RecapitulationMaterialMachineView]
	AS 
select
	isnull(ROW_NUMBER() OVER(ORDER BY un.ProgramNo, un.MachineName), -1) AS Sn,
	un.ProgramNo,
	un.BasedOnACClassID,
	un.MachineName,
	un.MaterialNo,
	un.MaterialName,
	sum(un.InputTargetQuantity) InputTargetQuantity,
	sum(un.InputActualQuantity) InputActualQuantity,
	sum(un.OutputTargetQuantity) OutputTargetQuantity,
	sum(un.OutputActualQuantity) OutputActualQuantity
from(
	select
		viewProgramLog.ProgramNo,
		viewProgramLog.BasedOnACClassID,
		max(viewProgramLog.AcIdentifier) as MachineName,
		viewProgramLog.MaterialNo as MaterialNo,
		viewProgramLog.MaterialName as MaterialName,
		0 as InputTargetQuantity,
		0 as InputActualQuantity,
		sum(viewProgramLog.PosTargetQuantityUOM) as OutputTargetQuantity,
		sum(viewProgramLog.PosActualQuantityUOM) as OutputActualQuantity
	from (
				select 
					viewProgramLog.ProgramNo,
					viewProgramLog.BasedOnACClassID,
					max(viewProgramLog.ACClassACIdentifier) as AcIdentifier,
					viewProgramLog.ProdOrderBatchNo,
					viewProgramLog.ProdOrderPartslistPosID,
					viewProgramLog.MaterialNo as MaterialNo,
					viewProgramLog.MaterialName as MaterialName,
					max(viewProgramLog.InwardTargetQuantityUOM) as PosTargetQuantityUOM,
					max(viewProgramLog.InwardActualQuantityUOM) as PosActualQuantityUOM
				from ACProgramLogView viewProgramLog
				where 
					--viewProgramLog.ProgramNo = '3816'
					viewProgramLog.ProdOrderPartslistPosRelationID is null
				group by
					viewProgramLog.ProgramNo,
					viewProgramLog.BasedOnACClassID,
					viewProgramLog.ProdOrderBatchNo,
					viewProgramLog.ProdOrderPartslistPosID,
					viewProgramLog.MaterialNo,
					viewProgramLog.MaterialName
					
	) viewProgramLog
	group by
		viewProgramLog.ProgramNo,
		viewProgramLog.BasedOnACClassID,
		viewProgramLog.AcIdentifier,
		viewProgramLog.MaterialNo,
		viewProgramLog.MaterialName
		union
	select
		viewProgramLog.ProgramNo,
		viewProgramLog.BasedOnACClassID,
		max(viewProgramLog.AcIdentifier) as MachineName,
		viewProgramLog.MaterialNo as MaterialNo,
		viewProgramLog.MaterialName as MaterialName,
		sum(viewProgramLog.RelTargetQuantityUOM) as InputTargetQuantity,
		sum(viewProgramLog.RelActualQuantityUOM) as InputActualQuantity,
		0 as RelTargetQuantityUOM,
		0 as RelActualQuantityUOM
	from (
				select 
					viewProgramLog.ProgramNo,
					viewProgramLog.BasedOnACClassID,
					max(viewProgramLog.ACClassACIdentifier) as AcIdentifier,
					viewProgramLog.ProdOrderBatchNo,
					viewProgramLog.ProdOrderPartslistPosID,
					viewProgramLog.MaterialNo as MaterialNo,
					viewProgramLog.MaterialName as MaterialName,
					max(viewProgramLog.OutwardTargetQuantityUOM) as RelTargetQuantityUOM,
					max(viewProgramLog.OutwardActualQuantityUOM) as RelActualQuantityUOM
				from ACProgramLogView viewProgramLog
				where 
					--viewProgramLog.ProgramNo = '3816'
					viewProgramLog.ProdOrderPartslistPosRelationID is not null
				group by
					viewProgramLog.ProgramNo,
					viewProgramLog.ProdOrderBatchNo,
					viewProgramLog.BasedOnACClassID,
					viewProgramLog.ProdOrderPartslistPosID,
					viewProgramLog.MaterialNo,
					viewProgramLog.MaterialName
					
	) viewProgramLog
	group by
		viewProgramLog.AcIdentifier,
		viewProgramLog.BasedOnACClassID,
		viewProgramLog.ProgramNo,
		viewProgramLog.MaterialNo,
		viewProgramLog.MaterialName
) un
group by
		un.ProgramNo,
		un.BasedOnACClassID,
		un.MachineName,
		un.MaterialNo,
		un.MaterialName