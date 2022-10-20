update tmpBp
	set tmpBp.StartOffsetSecAVG = tmpCalc.StartOffsetSecAVG,
		tmpBp.DurationSecAVG = tmpCalc.DurationSecAVG,
		tmpBp.UpdateName = @username,
		tmpBp.UpdateDate = getdate()
from ProdOrderBatchPlan tmpBp
inner join 
	(

select
	tmp.MDKey as MDSchedulingGroupMDKey,
	tmp.MDSchedulingGroupID,
	tmp.PartslistID,
	tmp.PartslistNo,
	tmp.MaterialNo,
	tmp.ProdOrderBatchPlanID,
	tmp.BatchTargetCount,
	tmp.BatchSize,
	avg(datediff(SECOND, prev.StartDate, tmp.StartDate)) as StartOffsetSecAVG,
	avg(datediff(SECOND, tmp.StartDate, tmp.EndDate)) as DurationSecAVG

from (
	select
		row_number() OVER (ORDER BY 
								bp.ProdOrderBatchPlanID,
								prLog.StartDate) as Sn,
		sg.MDSchedulingGroupID,
		pl.PartslistID,
		pll.PartslistNo,
		mt.MaterialNo,
		sg.MDKey,
		bp.BatchTargetCount,
		bp.BatchSize,
		bp.ProdOrderBatchPlanID,
		prLog.StartDate,
		prLog.EndDate
	from 
		ProdOrderBatchPlan bp
		left join ProdOrderBatch bt on bt.ProdOrderBatchPlanID = bp.ProdOrderBatchPlanID
		left join ProdOrderPartslistPos pos on pos.ProdOrderBatchID = bt.ProdOrderBatchID
        inner join ProdOrderPartslist pl on pl.ProdOrderPartslistID = pos.ProdOrderPartslistID
		inner join Partslist pll on pll.PartslistID = pl.PartslistID
		inner join Material mt on mt.MaterialID = pll.MaterialID
		inner join OrderLog ol on ol.ProdOrderPartslistPosID = pos.ProdOrderPartslistPosID
		inner join ACProgramLog prLog on prLog.ACProgramLogID = ol.VBiACProgramLogID

		-- process part
		left join ACClassWF wf on wf.ACClassWFID = bp.VBiACClassWFID
		left join MDSchedulingGroupWF sgWF on sgWF.VBiACClassWFID = wf.ACClassWFID
		left join MDSchedulingGroup sg on sg.MDSchedulingGroupID = sgWF.MDSchedulingGroupID



	where 
		(@prodOrderBatchPlanID is null or bp.ProdOrderBatchPlanID = @prodOrderBatchPlanID)
		and (@prodOrderPartslistID is null or pl.ProdOrderPartslistID = @prodOrderPartslistID)
		and sg.MDSchedulingGroupID is not null
		and bp.BatchTargetCount > 0
		and bp.BatchSize > 0
        and (@linie is null or sg.MDKey = @linie)
        and (@partslistNo is null or pll.PartslistNo = @partslistNo)

	)tmp

	left join
	(
		select
			row_number() OVER (ORDER BY 
									bp.ProdOrderBatchPlanID,
									prLog.StartDate) as Sn,
			sg.MDSchedulingGroupID,
			pl.PartslistID,
			sg.MDKey,
			bp.BatchTargetCount,
			bp.BatchSize,
			bp.ProdOrderBatchPlanID,
			prLog.StartDate,
			prLog.EndDate
		from 
			ProdOrderBatchPlan bp
			left join ProdOrderBatch bt on bt.ProdOrderBatchPlanID = bp.ProdOrderBatchPlanID
			left join ProdOrderPartslistPos pos on pos.ProdOrderBatchID = bt.ProdOrderBatchID
            inner join ProdOrderPartslist pl on pl.ProdOrderPartslistID = pos.ProdOrderPartslistID
			inner join Partslist pll on pll.PartslistID = pl.PartslistID
			inner join OrderLog ol on ol.ProdOrderPartslistPosID = pos.ProdOrderPartslistPosID
			inner join ACProgramLog prLog on prLog.ACProgramLogID = ol.VBiACProgramLogID

			-- process part
			left join ACClassWF wf on wf.ACClassWFID = bp.VBiACClassWFID
			left join MDSchedulingGroupWF sgWF on sgWF.VBiACClassWFID = wf.ACClassWFID
			left join MDSchedulingGroup sg on sg.MDSchedulingGroupID = sgWF.MDSchedulingGroupID



		where 
			(@prodOrderBatchPlanID is null or bp.ProdOrderBatchPlanID = @prodOrderBatchPlanID)
			and (@prodOrderPartslistID is null or pl.ProdOrderPartslistID = @prodOrderPartslistID)
			and sg.MDSchedulingGroupID is not null
			and bp.BatchTargetCount > 0
			and bp.BatchSize > 0
            and (@linie is null or sg.MDKey = @linie)
            and (@partslistNo is null or pll.PartslistNo = @partslistNo)
            
			
	) prev on 
		prev.ProdOrderBatchPlanID = tmp.ProdOrderBatchPlanID 
		and prev.MDSchedulingGroupID = tmp.MDSchedulingGroupID 
		and prev.PartslistID = tmp.PartslistID
		and prev.Sn = tmp.Sn -1
	
	
group by
	tmp.MDSchedulingGroupID,
	tmp.PartslistID,
	tmp.PartslistNo,
	tmp.MaterialNo,
	tmp.MDKey,
	tmp.ProdOrderBatchPlanID,
	tmp.BatchTargetCount,
	tmp.BatchSize) 
		tmpCalc on tmpCalc.ProdOrderBatchPlanID = tmpBp.ProdOrderBatchPlanID