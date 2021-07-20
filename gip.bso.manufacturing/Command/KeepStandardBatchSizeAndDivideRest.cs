using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    public class KeepStandardBatchSizeAndDivideRest
    {

        #region ctor's
        public KeepStandardBatchSizeAndDivideRest(int nr, double totalSize, int batchCount, double standardBatchSize, double minBatchSize, double maxBatchSize)
        {
            int calcBatchCount = batchCount;
            double calcBatchSize = 0;
            double rest = 0;
            rest = totalSize;

            if (totalSize > 0)
            {
                if ( (   Math.Abs(standardBatchSize) <= Double.Epsilon 
                      && Math.Abs(maxBatchSize) <= Double.Epsilon 
                      && Math.Abs(minBatchSize) <= Double.Epsilon)
                   || totalSize < minBatchSize)
                {
                    calcBatchCount = 1;
                    calcBatchSize = totalSize;
                    rest = 0;
                }
                else if (   standardBatchSize > Double.Epsilon 
                         && Math.Abs(maxBatchSize) <= Double.Epsilon 
                         && Math.Abs(minBatchSize) <= Double.Epsilon)
                {
                    if (totalSize <= standardBatchSize)
                    {
                        calcBatchCount = 1;
                        calcBatchSize = totalSize;
                        rest = 0;
                    }
                    else
                    {
                        calcBatchSize = standardBatchSize;
                        calcBatchCount = (int)(totalSize / calcBatchSize);
                        rest = totalSize - calcBatchSize * calcBatchCount;
                    }
                   
                }
                else if (   standardBatchSize > Double.Epsilon 
                         && minBatchSize > Double.Epsilon 
                         && maxBatchSize > Double.Epsilon)
                {
                    if (totalSize <= standardBatchSize)
                    {
                        calcBatchCount = 1;
                        calcBatchSize = totalSize;
                        rest = 0;
                    }
                    else
                    {
                        calcBatchSize = standardBatchSize;
                        calcBatchCount = (int)(totalSize / calcBatchSize);
                        rest = totalSize - calcBatchSize * calcBatchCount;
                        if (rest < minBatchSize || rest > maxBatchSize)
                        {
                            calcBatchSize += rest / calcBatchCount;
                            rest = totalSize - calcBatchSize * calcBatchCount;
                        }
                        if (calcBatchSize < minBatchSize || calcBatchSize > maxBatchSize)
                        {
                            if (calcBatchCount > 1)
                            {
                                calcBatchCount--;
                                calcBatchSize = standardBatchSize;
                                rest = totalSize - calcBatchSize * calcBatchCount;
                            }
                            else
                            {
                                calcBatchCount++;
                                calcBatchSize = totalSize / calcBatchCount;
                                rest = totalSize - calcBatchSize * calcBatchCount;
                            }
                        }
                    }
                }
            }


            if (calcBatchCount > 0 && calcBatchSize > Double.Epsilon)
                Suggestion = new BatchPlanSuggestionItem(nr, calcBatchSize, calcBatchCount, calcBatchSize * calcBatchCount);
            Rest = rest;
        }

        #endregion

        #region Result
        public BatchPlanSuggestionItem Suggestion { get; set; }
        public double Rest { get; set; }
        #endregion
    }
}
