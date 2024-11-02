// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public class KeepStandardBatchSizeAndDivideRest2
    {

        #region ctor's
        public KeepStandardBatchSizeAndDivideRest2(double roundingQuantity, WizardSchedulerPartslist wizardSchedulerPartslist, int nr, double totalSize, int batchCount, double standardBatchSize, double minBatchSize, double maxBatchSize)
        {
            Suggestions = new List<BatchPlanSuggestionItem>();
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

            if (minBatchSize > standardBatchSize)
                minBatchSize = standardBatchSize - 0.00001;

            if ((totalSize <= standardBatchSize && totalSize >= minBatchSize)
                || (totalSize > standardBatchSize && totalSize < maxBatchSize))
            {
                calcBatchCount = 1;
                calcBatchSize = totalSize;

                BatchPlanSuggestionItem suggestion = new BatchPlanSuggestionItem(wizardSchedulerPartslist, 1, calcBatchSize, calcBatchCount, calcBatchCount * calcBatchSize, null, true);
                Suggestions.Add(suggestion);
            }
            else
            {
                bool halfTest = false;
                bool useRest = false;

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

                    useRest = Rest >= minBatchSize && Rest <= maxBatchSize;
                    if (useRest)
                    {
                        break;
                    }

                    halfTest = (Rest / 2) >= minBatchSize && (Rest / 2) <= maxBatchSize && (Rest - standardBatchSize) < minBatchSize;
                    if (halfTest)
                    {
                        break;
                    }
                }

                BatchPlanSuggestionItem suggestion = new BatchPlanSuggestionItem(wizardSchedulerPartslist, 1, calcBatchSize, calcBatchCount, calcBatchCount * calcBatchSize, null, true);
                Suggestions.Add(suggestion);

                if (useRest)
                {
                    BatchPlanSuggestionItem restSuggestion = new BatchPlanSuggestionItem(wizardSchedulerPartslist, 2, Rest, 1, Rest, null, true);
                    Suggestions.Add(restSuggestion);
                }
                else if (halfTest)
                {
                    
                    BatchPlanSuggestionItem halfSuggestion = new BatchPlanSuggestionItem(wizardSchedulerPartslist, 2, (Rest / 2), 2, Rest, null, true);
                    Suggestions.Add(halfSuggestion);
                }

            }
        }

        #endregion

        #region Result
        public List<BatchPlanSuggestionItem> Suggestions { get; set; }
        public double Rest { get; set; }
        #endregion
    }
}
