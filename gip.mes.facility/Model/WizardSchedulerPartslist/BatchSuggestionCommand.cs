using System.ComponentModel;

namespace gip.mes.facility
{
    public class BatchSuggestionCommand
    {
        #region ctor's
        public BatchSuggestionCommand(WizardSchedulerPartslist wizardSchedulerPartslist, BatchSuggestionCommandModeEnum mode, double toleranceQuantity)
        {
            wizardSchedulerPartslist.BatchPlanSuggestion = new BatchPlanSuggestion(wizardSchedulerPartslist);
            wizardSchedulerPartslist.BatchPlanSuggestion.RestQuantityToleranceUOM = toleranceQuantity;

            wizardSchedulerPartslist.BatchPlanSuggestion.ItemsList = new BindingList<BatchPlanSuggestionItem>();

            double rest = wizardSchedulerPartslist.GetTargetQuantityUOM();
            if (rest > 0.0001)
            {
                int nr = 1;
                int i = 0;
                switch (mode)
                {
                    case BatchSuggestionCommandModeEnum.KeepStandardBatchSizeAndDivideRest:
                        KeepStandardBatchSizeAndDivideRest suggestion1 = null;
                        do
                        {
                            suggestion1 = new KeepStandardBatchSizeAndDivideRest(wizardSchedulerPartslist, nr, rest, 0, wizardSchedulerPartslist.BatchSizeStandardUOM, wizardSchedulerPartslist.BatchSizeMinUOM, wizardSchedulerPartslist.BatchSizeMaxUOM);
                            if (suggestion1 != null)
                            {
                                if (suggestion1.Suggestion != null)
                                {
                                    wizardSchedulerPartslist.BatchPlanSuggestion.AddItem(suggestion1.Suggestion);
                                    nr++;
                                }
                                rest = suggestion1.Rest;
                            }
                            else
                                rest = 0.0;
                            i++;
                        }
                        while (rest > double.Epsilon && i < 10);
                        break;
                    case BatchSuggestionCommandModeEnum.KeepEqualBatchSizes:
                        KeepEqualBatchSizes suggestion2 = null;
                        do
                        {
                            suggestion2 = new KeepEqualBatchSizes(wizardSchedulerPartslist, nr, rest, wizardSchedulerPartslist.BatchSizeStandardUOM, wizardSchedulerPartslist.BatchSizeMinUOM, wizardSchedulerPartslist.BatchSizeMaxUOM);
                            if (suggestion2 != null)
                            {
                                if (suggestion2.Suggestion != null)
                                {
                                    wizardSchedulerPartslist.BatchPlanSuggestion.AddItem(suggestion2.Suggestion);
                                    nr++;
                                }
                                rest = suggestion2.Rest;
                            }
                            else
                                rest = 0.0;
                            i++;
                        }
                        while (rest > double.Epsilon && i < 10);
                        break;
                    default:
                        break;
                }
                wizardSchedulerPartslist.BatchPlanSuggestion.RestNotUsedQuantityUOM = rest;
            }
        }

        #endregion

    }
}
