using System;

namespace gip.bso.manufacturing
{
    public class KeepEqualBatchSizes
    {
        #region ctor's
        public KeepEqualBatchSizes(int nr, double totalSize, double standardBatchSize, double minBatchSize, double maxBatchSize)
        {
            int calcBatchCount = 0;
            double calcBatchSize = 0;
            double rest = 0;

            if ((minBatchSize == 0 && maxBatchSize == 0 && standardBatchSize == 0) || totalSize < minBatchSize)
            {
                // Invalid input value
                rest = totalSize;
            }
            else if (totalSize < standardBatchSize)
            {
                calcBatchCount = 1;
                calcBatchSize = totalSize;
            }
            else
            {
                calcBatchSize = standardBatchSize;
                calcBatchCount = (int)Math.Round(totalSize / calcBatchSize);
                rest = totalSize - calcBatchCount * calcBatchSize;
                if (Math.Abs(rest) < 0.1)
                {

                }
                else
                {
                    if (minBatchSize > 0 && maxBatchSize > 0)
                    {
                        calcBatchSize += rest / calcBatchCount;
                        if (calcBatchSize > standardBatchSize)
                        {
                            calcBatchCount++;
                            calcBatchSize = totalSize / calcBatchCount;
                            if (calcBatchSize < minBatchSize || calcBatchSize > maxBatchSize)
                            {
                                calcBatchCount--;
                                calcBatchSize = totalSize / calcBatchCount;
                                rest = totalSize - calcBatchCount * calcBatchSize;
                                if (calcBatchSize < minBatchSize || calcBatchSize > maxBatchSize)
                                {
                                    calcBatchCount = 0;
                                    calcBatchSize = 0;
                                }
                            }
                        }
                        else if (calcBatchSize < minBatchSize)
                        {
                            calcBatchCount--;
                            calcBatchSize = totalSize / calcBatchCount;
                            rest = totalSize - calcBatchCount * calcBatchSize;
                            if (calcBatchSize < minBatchSize || calcBatchSize > maxBatchSize)
                            {
                                calcBatchCount = 0;
                                calcBatchSize = 0;
                            }
                        }
                        rest = totalSize - calcBatchSize * calcBatchCount;
                        if (Math.Abs(rest) > 0.1)
                        {
                            calcBatchCount = 0;
                            calcBatchSize = 0;
                            rest = totalSize;
                        }
                    }
                    else
                    {
                        if (rest > 0)
                            calcBatchCount++;
                        else
                            calcBatchCount--;
                        calcBatchSize = totalSize / calcBatchCount;
                        if(calcBatchSize > standardBatchSize)
                        {
                            calcBatchCount++;
                            calcBatchSize = totalSize / calcBatchCount;
                        }
                        rest = totalSize - calcBatchCount * calcBatchSize;
                        if (Math.Abs(rest) > 0.1)
                        {
                            calcBatchCount = 0;
                            calcBatchSize = 0;
                            rest = totalSize;
                        }
                    }

                }
            }

            if (calcBatchSize > 0 && calcBatchCount > 0)
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
