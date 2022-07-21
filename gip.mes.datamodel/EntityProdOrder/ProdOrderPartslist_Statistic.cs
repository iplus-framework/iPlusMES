using gip.core.datamodel;

namespace gip.mes.datamodel
{
    public partial class ProdOrderPartslist
    {


        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "DifferenceQuantityPer", "en{'TODO:DifferenceQuantityPer'}de{'TODO:DifferenceQuantityPer'}")]
        public double DifferenceQuantityPer
        {
            get
            {
                if (TargetQuantity == 0)
                    return 0;
                return ActualQuantity / TargetQuantity;
            }
        }

        #region InputQForActualOutput

        [ACPropertyInfo(999, "InputQForActualOutput", "en{'Input Quantity for actual output'}de{'Input Quantity for actual output'}")]
        public double InputQForActualOutput
        {
            get
            {
                if (InputQForActualOutputPer == null || (InputQForActualOutputPer ?? 0) == 0)
                    return 0;
                return ActualQuantity / (InputQForActualOutputPer ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForActualOutput", "en{'Diff. Quantity for actual output'}de{'Diff. Quantity for actual output'}")]
        public double InputQForActualOutputDiff
        {
            get
            {
                return ActualQuantity - InputQForActualOutput;
            }
        }

        #endregion


        #region InputQForGoodActualOutput

        [ACPropertyInfo(999, "InputQForGoodActualOutput", "en{'Diff. Quantity for good output'}de{'Diff. Quantity for good output'}")]
        public double InputQForGoodActualOutput
        {
            get
            {
                if (InputQForGoodActualOutputPer == null || (InputQForGoodActualOutputPer ?? 0) == 0)
                    return 0;
                return ActualQuantity / (InputQForGoodActualOutputPer ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForGoodActualOutput", "en{'Diff. Quantity for good output'}de{'Diff. Quantity for good output'}")]
        public double InputQForGoodActualOutputDiff
        {
            get
            {
                return ActualQuantity - InputQForGoodActualOutput;
            }
        }

        #endregion

        #region InputQForScrapActualOutput

        [ACPropertyInfo(999, "InputQForScrapActualOutput", "en{'Input Quantity for scraped output'}de{'Input Quantity for scraped output'}")]
        public double InputQForScrapActualOutput
        {
            get
            {
                if (InputQForScrapActualOutputPer == null || (InputQForScrapActualOutputPer ?? 0) == 0)
                    return 0;
                return ActualQuantity / (InputQForScrapActualOutputPer ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForScrapActualOutput", "en{'Diff. Quantity for scraped output'}de{'Diff. Quantity for scraped output'}")]
        public double InputQForScrapActualOutputDiff
        {
            get
            {
                return ActualQuantity - InputQForScrapActualOutput;
            }
        }

        #endregion

        #region InputQForFinalActualOutput

        [ACPropertyInfo(999, "InputQForFinalActualOutput", "en{'nput Quantity for final actual output'}de{'nput Quantity for final actual output'}")]
        public double InputQForFinalActualOutput
        {
            get
            {
                if (InputQForFinalActualOutputPer == null || (InputQForFinalActualOutputPer ?? 0) == 0)
                    return 0;
                return ActualQuantity / (InputQForFinalActualOutputPer ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForFinalActualOutput", "en{'Diff. Quantity for final actual output'}de{'Diff. Quantity for final actual output'}")]
        public double InputQForFinalActualOutputDiff
        {
            get
            {
                return ActualQuantity - InputQForFinalActualOutput;
            }
        }

        #endregion

        #region InputQForFinalGoodActualOutput

        [ACPropertyInfo(999, "InputQForFinalGoodActualOutput", "en{'Input Quantity for final good output'}de{'Input Quantity for final good output'}")]
        public double InputQForFinalGoodActualOutput
        {
            get
            {
                if (InputQForFinalGoodActualOutputPer == null || (InputQForFinalGoodActualOutputPer ?? 0) == 0)
                    return 0;
                return ActualQuantity / (InputQForFinalGoodActualOutputPer ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForFinalGoodActualOutput", "en{'Diff. Quantity for final good output'}de{'Diff. Quantity for final good output'}")]
        public double InputQForFinalGoodActualOutputDiff
        {
            get
            {
                return ActualQuantity - InputQForFinalGoodActualOutput;
            }
        }

        #endregion

        #region InputQForFinalScrapActualOutput

        [ACPropertyInfo(999, "InputQForFinalScrapActualOutput", "en{'Input Quantity for final scraped output'}de{'Input Quantity for final scraped output'}")]
        public double InputQForFinalScrapActualOutput
        {
            get
            {
                if (InputQForFinalScrapActualOutputPer == null || (InputQForFinalScrapActualOutputPer ?? 0) == 0)
                    return 0;
                return ActualQuantity / (InputQForFinalScrapActualOutputPer ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForFinalScrapActualOutput", "en{'Diff Quantity for final scraped outpu'}de{'Diff Quantity for final scraped outpu'}")]
        public double InputQForFinalScrapActualOutputDiff
        {
            get
            {
                return ActualQuantity - InputQForFinalScrapActualOutput;
            }
        }

        #endregion

    }
}
