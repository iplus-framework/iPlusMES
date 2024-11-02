// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;

namespace gip.mes.facility
{
    public class KeepEqualBatchSizes
    {
        #region ctor's
        public KeepEqualBatchSizes(WizardSchedulerPartslist wizardSchedulerPartslist, int nr, double totalSize, double standardBatchSize, double minBatchSize, double maxBatchSize)
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


            for (int i=0; i < 10000; i++)
            {
                calcBatchCount++;
                double tmpBatchSize = totalSize / calcBatchCount;
                tmpBatchSize = wizardSchedulerPartslist.CorrectQuantityWithProductionUnits(tmpBatchSize);
                if (Math.Abs(minBatchSize) > Double.Epsilon && tmpBatchSize < minBatchSize)
                {
                    calcBatchCount--;
                    break;
                }
                if (tmpBatchSize > maxBatchSize)
                    continue;
                calcBatchSize = tmpBatchSize;
                break;
            }
            Rest = totalSize - (calcBatchCount * calcBatchSize);
            if (Rest > 0.00001)
            {
                if (Math.Abs(minBatchSize) > Double.Epsilon && Rest < minBatchSize)
                    Rest = minBatchSize;
                Rest = wizardSchedulerPartslist.CorrectQuantityWithProductionUnits(Rest);
            }

            //while (true)
            //{
            //    calcBatchCount++;
            //    double tmpBatchSize = (int)totalSize / calcBatchCount;
            //    if (tmpBatchSize < minBatchSize)
            //    {
            //        calcBatchCount--;
            //        break;
            //    }
            //    calcBatchSize = tmpBatchSize;
            //    if (calcBatchSize <= standardBatchSize)
            //        break;
            //}
            //rest = totalSize - calcBatchCount * calcBatchSize;

            if (calcBatchSize > Double.Epsilon && calcBatchCount > 0)
            {
                Suggestion = new BatchPlanSuggestionItem(wizardSchedulerPartslist, nr, calcBatchSize, calcBatchCount, calcBatchSize * calcBatchCount, null, true);
            }
        }
        #endregion

        #region Result
        public BatchPlanSuggestionItem Suggestion { get; set; }
        public double Rest { get; set; }
        #endregion
    }
}
