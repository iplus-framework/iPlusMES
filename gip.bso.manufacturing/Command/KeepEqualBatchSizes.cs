using gip.mes.datamodel;
using System;

namespace gip.bso.manufacturing
{
    public class KeepEqualBatchSizes
    {
        #region ctor's
        public KeepEqualBatchSizes(WizardSchedulerPartslist wizardSchedulerPartslist, int nr, double totalSize, double standardBatchSize, double minBatchSize, double maxBatchSize)
        {
            int calcBatchCount = 0;
            double calcBatchSize = 0;
            double rest = 0;

            double calcMaxBatchSize = 0;


            if (totalSize > Double.Epsilon)
            {
                if ((Math.Abs(standardBatchSize) <= Double.Epsilon
                      && Math.Abs(maxBatchSize) <= Double.Epsilon
                      && Math.Abs(minBatchSize) <= Double.Epsilon)
                   || totalSize < minBatchSize)
                {
                    // do nothing
                }
                else if (standardBatchSize > Double.Epsilon
                         && Math.Abs(maxBatchSize) <= Double.Epsilon
                         && Math.Abs(minBatchSize) <= Double.Epsilon)
                {
                    calcMaxBatchSize = standardBatchSize;
                }
                else if (maxBatchSize > Double.Epsilon)
                {
                    calcMaxBatchSize = maxBatchSize;
                }

                while (true)
                {
                    calcBatchCount++;
                    double tmpBatchSize = (int)totalSize / calcBatchCount;
                    if (tmpBatchSize < minBatchSize)
                    {
                        calcBatchCount--;
                        break;
                    }
                    calcBatchSize = tmpBatchSize;
                    if(calcBatchSize <= standardBatchSize)
                        break;
                }
                rest = totalSize - calcBatchCount * calcBatchSize;
            }

            if (calcBatchSize > 0 && calcBatchCount > 0)
                Suggestion = new BatchPlanSuggestionItem(wizardSchedulerPartslist, nr, calcBatchSize, calcBatchCount, calcBatchSize * calcBatchCount) { IsEditable = true };
            Rest = rest;
        }
        #endregion

        #region Result
        public BatchPlanSuggestionItem Suggestion { get; set; }
        public double Rest { get; set; }
        #endregion
    }
}
