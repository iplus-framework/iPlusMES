using System;

namespace gip.bso.manufacturing
{
    public class KeepStandardBatchSizeAndDivideRest
    {

        #region ctor's
        public KeepStandardBatchSizeAndDivideRest(int nr, double totalSize, int batchCount, double standardBatchSize, double minBatchSize, double maxBatchSize)
        {
            int calcBatchCount = 0;
            double calcBatchSize = 0;
            double rest = 0;
            rest = totalSize;


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
                    calcBatchSize = standardBatchSize;
                }
                else if (maxBatchSize > Double.Epsilon)
                {
                    calcBatchSize = maxBatchSize;
                }


                if(calcBatchSize > Double.Epsilon)
                {
                    if(totalSize <= calcBatchSize && totalSize >= minBatchSize)
                    {
                        calcBatchCount = 1;
                        calcBatchSize = totalSize;
                    }
                    else
                    {
                        calcBatchCount = (int)(totalSize / calcBatchSize);
                    }
                    rest = totalSize - calcBatchCount * calcBatchSize;
                }
            }

            if (calcBatchCount > Double.Epsilon && calcBatchSize > Double.Epsilon)
                Suggestion = new BatchPlanSuggestionItem(nr, calcBatchSize, calcBatchCount, calcBatchSize * calcBatchCount) { IsEditable = true };
          
            Rest = rest;
        }

        #endregion

        #region Result
        public BatchPlanSuggestionItem Suggestion { get; set; }
        public double Rest { get; set; }
        #endregion
    }
}
