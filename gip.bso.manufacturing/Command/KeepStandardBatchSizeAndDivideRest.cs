using gip.mes.datamodel;
using System;

namespace gip.bso.manufacturing
{
    public class KeepStandardBatchSizeAndDivideRest
    {

        #region ctor's
        public KeepStandardBatchSizeAndDivideRest(WizardSchedulerPartslist wizardSchedulerPartslist, int nr, double totalSize, int batchCount, double standardBatchSize, double minBatchSize, double maxBatchSize)
        {
            int calcBatchCount = 0;
            double calcBatchSize = 0;
            Rest = 0;

            if (totalSize <= Double.Epsilon)
                return;

            if (   (   Math.Abs(standardBatchSize) <= Double.Epsilon
                    && Math.Abs(maxBatchSize) <= Double.Epsilon)
                || (   Math.Abs(minBatchSize) > Double.Epsilon 
                    && totalSize < minBatchSize))
            {
                return;
            }

            if (Math.Abs(maxBatchSize) <= Double.Epsilon)
                maxBatchSize = standardBatchSize;
            else if (Math.Abs(standardBatchSize) <= Double.Epsilon)
                standardBatchSize = maxBatchSize;
            //standardBatchSize = wizardSchedulerPartslist.CorrectQuantityWithProductionUnits(standardBatchSize);

            if (minBatchSize > standardBatchSize)
                minBatchSize = standardBatchSize - 0.00001;

            if (   (totalSize <= standardBatchSize && totalSize >= minBatchSize)
                || (totalSize > standardBatchSize && totalSize < maxBatchSize))
            {
                calcBatchCount = 1;
                calcBatchSize = totalSize;
            }
            else
            {
                calcBatchSize = standardBatchSize;
                calcBatchCount = (int)(totalSize / standardBatchSize);
                Rest = totalSize - (calcBatchCount * calcBatchSize);
                if (Math.Abs(minBatchSize) > Double.Epsilon && Math.Abs(Rest) > Double.Epsilon &&  Rest < minBatchSize)
                {
                    calcBatchCount--;
                    Rest = totalSize - (calcBatchCount * calcBatchSize);
                }
            }

            calcBatchSize = wizardSchedulerPartslist.CorrectQuantityWithProductionUnits(calcBatchSize);
            if (calcBatchCount > 0 && calcBatchSize > Double.Epsilon)
            {
                Suggestion = new BatchPlanSuggestionItem(wizardSchedulerPartslist, nr, calcBatchSize, calcBatchCount, calcBatchSize * calcBatchCount) { IsEditable = true };
                Rest = totalSize - (calcBatchCount * calcBatchSize);
            }
        }

        #endregion

        #region Result
        public BatchPlanSuggestionItem Suggestion { get; set; }
        public double Rest { get; set; }
        #endregion
    }
}
