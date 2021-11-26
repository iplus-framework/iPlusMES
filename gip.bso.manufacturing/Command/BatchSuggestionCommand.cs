using gip.mes.processapplication;
using System.Collections.Generic;
using System.ComponentModel;

namespace gip.bso.manufacturing
{
    public class BatchSuggestionCommand
    {
        #region ctor's
        public BatchSuggestionCommand(WizardSchedulerPartslist wizardSchedulerPartslist, BatchSuggestionCommandModeEnum mode, double toleranceQuantity)
        {
            wizardSchedulerPartslist.BatchPlanSuggestion = new BatchPlanSuggestion(wizardSchedulerPartslist);
            wizardSchedulerPartslist.BatchPlanSuggestion.RestQuantityTolerance = toleranceQuantity;
            wizardSchedulerPartslist.BatchPlanSuggestion.TotalSize = wizardSchedulerPartslist.NewTargetQuantityUOM;
            wizardSchedulerPartslist.BatchPlanSuggestion.ItemsList = new BindingList<BatchPlanSuggestionItem>();

            

            if (wizardSchedulerPartslist.BatchPlanSuggestion.TotalSize > 0)
            {

                double rest = wizardSchedulerPartslist.BatchPlanSuggestion.TotalSize;
                int nr = 1;
                switch (mode)
                {
                    case BatchSuggestionCommandModeEnum.KeepStandardBatchSizeAndDivideRest:
                        KeepStandardBatchSizeAndDivideRest suggestion1 = null;
                        do
                        {
                            suggestion1 = new KeepStandardBatchSizeAndDivideRest(wizardSchedulerPartslist, nr, rest, 0, wizardSchedulerPartslist.BatchSizeStandard, wizardSchedulerPartslist.BatchSizeMin, wizardSchedulerPartslist.BatchSizeMax);
                            if (suggestion1.Suggestion != null)
                                wizardSchedulerPartslist.BatchPlanSuggestion.AddItem(suggestion1.Suggestion);
                            rest = suggestion1.Rest;
                            nr++;
                        }
                        while (suggestion1 != null && suggestion1.Suggestion != null && rest > double.Epsilon);
                        break;
                    case BatchSuggestionCommandModeEnum.KeepEqualBatchSizes:
                        KeepEqualBatchSizes suggestion2 = null;
                        do
                        {
                            suggestion2 = new KeepEqualBatchSizes(wizardSchedulerPartslist, nr, rest, wizardSchedulerPartslist.BatchSizeStandard, wizardSchedulerPartslist.BatchSizeMin, wizardSchedulerPartslist.BatchSizeMax);
                            if (suggestion2.Suggestion != null)
                                wizardSchedulerPartslist.BatchPlanSuggestion.AddItem(suggestion2.Suggestion);
                            rest = suggestion2.Rest;
                            nr++;
                        }
                        while (suggestion2 != null && suggestion2.Suggestion != null && rest > 0);
                        break;
                    default:
                        break;
                }
                wizardSchedulerPartslist.BatchPlanSuggestion.RestNotUsedQuantity = rest;
            }
        }

        #endregion

    }
}
