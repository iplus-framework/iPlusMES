// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;

namespace gip.mes.facility
{
    public class KeepStandardBatchSizeAndDivideRest
    {

        #region ctor's
        public KeepStandardBatchSizeAndDivideRest(double roundingQuantity, WizardSchedulerPartslist wizardSchedulerPartslist, int nr, double totalSize, int batchCount, double standardBatchSize, double minBatchSize, double maxBatchSize)
        {
            int calcBatchCount = 0;
            double calcBatchSize = 0;
            Rest = 0;

            if (totalSize <= Double.Epsilon)
                return;

            if ((Math.Abs(standardBatchSize) <= Double.Epsilon
                    && Math.Abs(maxBatchSize) <= Double.Epsilon)
                || (Math.Abs(minBatchSize) > Double.Epsilon
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

            if ((totalSize <= standardBatchSize && totalSize >= minBatchSize)
                || (totalSize > standardBatchSize && totalSize < maxBatchSize))
            {
                calcBatchCount = 1;
                calcBatchSize = totalSize;
            }
            else
            {
                calcBatchSize = standardBatchSize;
                calcBatchCount = 1;
                Rest = totalSize - (calcBatchCount * calcBatchSize);
                double infValue = FacilityConst.C_ZeroCompare;
                if (roundingQuantity > 0)
                {
                    infValue = roundingQuantity;
                }
                while (Math.Round(Rest - minBatchSize, 6) > infValue)
                {
                    double checkValue = Math.Round((totalSize - ((calcBatchCount + 1) * calcBatchSize)), 6);
                    // check for overflow
                    if (checkValue < 0 && Math.Abs(checkValue) > infValue)
                    {
                        break;
                    }
                    calcBatchCount++;
                    Rest = totalSize - (calcBatchCount * calcBatchSize);
                }
                //int tempBatchCount = (int)(totalSize / standardBatchSize);
                //Rest = totalSize - (calcBatchCount * calcBatchSize);
                //tempBatchCount++;
                //for (int i = 1; i <= tempBatchCount; i++)
                //{
                //    Rest = totalSize - (i * calcBatchSize);
                //    bool fitIntoStandardSize = Math.Round(Rest, 6)
                //    if (Rest < maxBatchSize)
                //    {
                //        calcBatchCount = i;
                //        break;
                //    }
                //}
            }

            calcBatchSize = wizardSchedulerPartslist.CorrectQuantityWithProductionUnits(calcBatchSize);
            if (calcBatchCount > 0 && calcBatchSize > Double.Epsilon)
            {
                Suggestion = new BatchPlanSuggestionItem(wizardSchedulerPartslist, nr, calcBatchSize, calcBatchCount, calcBatchSize * calcBatchCount, null, true);
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
