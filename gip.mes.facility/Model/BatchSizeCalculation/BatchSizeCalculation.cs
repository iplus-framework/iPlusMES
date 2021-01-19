using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    /// <summary>
    /// Converter between two models of batch definition
    /// </summary>
    public static class BatchSizeCalculation
    {
        /// <summary>
        /// Calculate percentages of quantity parts in batches and return
        /// a representative model
        /// </summary>
        /// <param name="targetQuantity">top (sum) quantity</param>
        /// <param name="quantityModel">model with defined quantities</param>
        /// <returns></returns>
        public static List<BatchPercentageModel> GetPercentageModel(double targetQuantity, List<BatchQuantityModel> quantityModel)
        {
            return quantityModel.Select
                (x => new BatchPercentageModel()
                {
                    Sequence = x.Sequence,
                    Percentage = x.TargetQuantity / targetQuantity
                }).ToList();
        }

        /// <summary>
        /// From percentage model return 
        /// </summary>
        /// <param name="targetQuantity">sum quantity</param>
        /// <param name="percentageModel">percentage definition for every step</param>
        /// <param name="calculationModel"></param>
        /// <param name="roundingDecimalPlaces"></param>
        /// <returns></returns>
        public static List<BatchQuantityModel> GetQuantityModel(double targetQuantity, List<BatchPercentageModel> percentageModel, RestHandleModeEnum calculationModel, int roundingDecimalPlaces = 2)
        {
            List<BatchQuantityModel> list = new List<BatchQuantityModel>();
            list = percentageModel.Select(x => new BatchQuantityModel() { Sequence = x.Sequence, TargetQuantity = Math.Round(targetQuantity * x.Percentage, roundingDecimalPlaces) }).ToList();

            if (calculationModel != RestHandleModeEnum.DoNothing)
            {
                double restQuantity = targetQuantity - list.Sum(x => x.TargetQuantity);
                if (restQuantity > 0)
                    switch (calculationModel)
                    {
                        case RestHandleModeEnum.ToFirstBatch:
                            list.First().TargetQuantity = list.First().TargetQuantity + restQuantity;
                            break;
                        case RestHandleModeEnum.ToLastBatch:
                            list.Last().TargetQuantity = list.First().TargetQuantity + restQuantity;
                            break;
                        case RestHandleModeEnum.DevideToAllBatches:
                            double partToAdd = Math.Round(restQuantity / list.Count(), roundingDecimalPlaces);
                            list.ForEach(x => x.TargetQuantity = x.TargetQuantity + partToAdd);
                            break;
                    }
            }
            return list;
        }
    }
}
