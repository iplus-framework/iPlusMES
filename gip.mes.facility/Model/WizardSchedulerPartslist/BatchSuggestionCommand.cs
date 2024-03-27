using System.ComponentModel;

namespace gip.mes.facility
{
    public class BatchSuggestionCommand
    {
        #region ctor's
        public BatchSuggestionCommand(double roundingQuantity, WizardSchedulerPartslist wizardSchedulerPartslist, BatchSuggestionCommandModeEnum mode, double toleranceQuantity)
        {
            wizardSchedulerPartslist.BatchPlanSuggestion = new BatchPlanSuggestion(wizardSchedulerPartslist);
            wizardSchedulerPartslist.BatchPlanSuggestion.RestQuantityToleranceUOM = toleranceQuantity;

            wizardSchedulerPartslist.BatchPlanSuggestion.ItemsList = new BindingList<BatchPlanSuggestionItem>();

            double rest = wizardSchedulerPartslist.GetTargetQuantityUOM();
            if (rest > 0.0001)
            {
                int nr = 1;
                int i = 0;
                KeepEqualBatchSizes suggestion2 = null;
                switch (mode)
                {
                    case BatchSuggestionCommandModeEnum.KeepStandardBatchSizeAndDivideRest:
                        // TODO: Test new KeepStandardBatchSizeAndDivideRest2: Comment section
                        KeepStandardBatchSizeAndDivideRest suggestion1 = null;
                        do
                        {
                            suggestion1 = new KeepStandardBatchSizeAndDivideRest(roundingQuantity, wizardSchedulerPartslist, nr, rest, 0, wizardSchedulerPartslist.BatchSizeStandardUOM, wizardSchedulerPartslist.BatchSizeMinUOM, wizardSchedulerPartslist.BatchSizeMaxUOM);
                            if (suggestion1 != null)
                            {
                                if (suggestion1.Suggestion != null)
                                {
                                    wizardSchedulerPartslist.BatchPlanSuggestion.AddItem(suggestion1.Suggestion);
                                    nr++;
                                }
                                rest = suggestion1.Rest;
                            }
                            i++;
                        }
                        while (rest >= wizardSchedulerPartslist.BatchSizeMinUOM && i < 10);
                        if (rest > 0.1 && rest > wizardSchedulerPartslist.BatchSizeMinUOM)
                        {
                            suggestion2 = new KeepEqualBatchSizes(wizardSchedulerPartslist, nr, rest, wizardSchedulerPartslist.BatchSizeStandardUOM, wizardSchedulerPartslist.BatchSizeMinUOM, wizardSchedulerPartslist.BatchSizeMaxUOM);
                            if (suggestion2.Suggestion != null)
                            {
                                wizardSchedulerPartslist.BatchPlanSuggestion.AddItem(suggestion2.Suggestion);
                            }
                        }

                        // TODO: Test new KeepStandardBatchSizeAndDivideRest2: Uncomment section
                        //KeepStandardBatchSizeAndDivideRest2 suggestionKPTmp = new KeepStandardBatchSizeAndDivideRest2(roundingQuantity, wizardSchedulerPartslist, 1, rest, 0, wizardSchedulerPartslist.BatchSizeStandardUOM, wizardSchedulerPartslist.BatchSizeMinUOM, wizardSchedulerPartslist.BatchSizeMaxUOM);
                        //foreach(BatchPlanSuggestionItem suggestionTmp in suggestionKPTmp.Suggestions)
                        //{
                        //    wizardSchedulerPartslist.BatchPlanSuggestion.AddItem(suggestionTmp);
                        //}
                        break;
                    case BatchSuggestionCommandModeEnum.KeepEqualBatchSizes:
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
