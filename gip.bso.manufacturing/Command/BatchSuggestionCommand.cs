using gip.mes.processapplication;
using System.Collections.Generic;
using System.ComponentModel;

namespace gip.bso.manufacturing
{
    public class BatchSuggestionCommand
    {
        #region ctor's
        public BatchSuggestionCommand(WizardSchedulerPartslist wizardSchedulerPartslist, BatchSuggestionCommandModeEnum mode)
        {
            BatchPlanSuggestion = new BatchPlanSuggestion();
            BatchPlanSuggestion.TotalSize = wizardSchedulerPartslist.NewTargetQuantityUOM;
            BatchPlanSuggestion.ItemsList = new BindingList<BatchPlanSuggestionItem>();

            if (BatchPlanSuggestion.TotalSize > 0)
            {

                double rest = BatchPlanSuggestion.TotalSize;
                int nr = 1;
                switch (mode)
                {
                    case BatchSuggestionCommandModeEnum.KeepStandardBatchSizeAndDivideRest:
                        KeepStandardBatchSizeAndDivideRest suggestion1 = null;
                        do
                        {
                            suggestion1 = new KeepStandardBatchSizeAndDivideRest(nr, rest, 0, wizardSchedulerPartslist.BatchSizeStandard, wizardSchedulerPartslist.BatchSizeMin, wizardSchedulerPartslist.BatchSizeMax);
                            if (suggestion1.Suggestion != null)
                                BatchPlanSuggestion.ItemsList.Add(suggestion1.Suggestion);
                            rest = suggestion1.Rest;
                            nr++;
                        }
                        while (suggestion1 != null && suggestion1.Suggestion != null && rest > 0);
                        break;
                    case BatchSuggestionCommandModeEnum.KeepEqualBatchSizes:
                        KeepEqualBatchSizes suggestion2 = null;
                        do
                        {
                            suggestion2 = new KeepEqualBatchSizes(nr, rest, wizardSchedulerPartslist.BatchSizeStandard, wizardSchedulerPartslist.BatchSizeMin, wizardSchedulerPartslist.BatchSizeMax);
                            if (suggestion2.Suggestion != null)
                                BatchPlanSuggestion.ItemsList.Add(suggestion2.Suggestion);
                            rest = suggestion2.Rest;
                            nr++;
                        }
                        while (suggestion2 != null && suggestion2.Suggestion != null && rest > 0);
                        break;
                    default:
                        break;
                }
                BatchPlanSuggestion.RestNotUsedQuantity = rest;
            }
        }

        #endregion

        public BatchPlanSuggestion BatchPlanSuggestion { get; private set; }
    }
}
