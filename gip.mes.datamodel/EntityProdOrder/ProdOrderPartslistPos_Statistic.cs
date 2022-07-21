using gip.core.datamodel;

namespace gip.mes.datamodel
{
    public partial class ProdOrderPartslistPos
    {

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "DifferenceQuantityPer", "en{'Difference Quantity (%)'}de{'Difference Quantity (%)'}")]
        public double DifferenceQuantityPer
        {
            get
            {
                if (TargetQuantityUOM == 0)
                    return 0;
                return ActualQuantityUOM / TargetQuantityUOM;
            }
        }

        #region InputQForActualOutput

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForActualOutput", "en{'Input Quantity for actual output (%)'}de{'Input Quantity for actual output (%)'}")]
        public double InputQForActualOutputPer
        {
            get
            {
                if (InputQForActualOutput == null || (InputQForActualOutput ?? 0) == 0)
                    return 0;
                return ActualQuantityUOM / (InputQForActualOutput ?? 1);
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForActualOutput", "en{'Diff. Quantity for actual output'}de{'Diff. Quantity for actual output'}")]
        public double InputQForActualOutputDiff
        {
            get
            {
                return ActualQuantityUOM - (InputQForActualOutput ?? 0);
            }
        }

        #endregion

        #region InputQForGoodActualOutput

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForGoodActualOutput", "en{'Input Quantity for good output (%)'}de{'Input Quantity for good output (%)'}")]
        public double InputQForGoodActualOutputPer
        {
            get
            {
                if (InputQForGoodActualOutput == null || (InputQForGoodActualOutput ?? 0) == 0)
                    return 0;
                return ActualQuantityUOM / (InputQForGoodActualOutput ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForGoodActualOutput", "en{'Diff. Quantity for good output'}de{'Diff. Quantity for good output'}")]
        public double InputQForGoodActualOutputDiff
        {
            get
            {
                return ActualQuantityUOM - (InputQForGoodActualOutput ?? 0);
            }
        }

        #endregion

        #region InputQForScrapActualOutput

        [ACPropertyInfo(999, "InputQForScrapActualOutput", "en{'Input Quantity for scraped output (%)'}de{'Input Quantity for scraped output (%)'}")]
        public double InputQForScrapActualOutputPer
        {
            get
            {
                if (InputQForScrapActualOutput == null || (InputQForScrapActualOutput ?? 0) == 0)
                    return 0;
                return ActualQuantityUOM / (InputQForScrapActualOutput ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForScrapActualOutput", "en{'Diff. Quantity for scraped output'}de{'Diff. Quantity for scraped output'}")]
        public double InputQForScrapActualOutputDiff
        {
            get
            {
                return ActualQuantityUOM - (InputQForScrapActualOutput ?? 0);
            }
        }

        #endregion

        #region InputQForFinalActualOutput

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForFinalActualOutput", "en{'Input Quantity for final actual output (%)'}de{'Input Quantity for final actual output (%)'}")]
        public double InputQForFinalActualOutputPer
        {
            get
            {
                if (InputQForFinalActualOutput == null || (InputQForFinalActualOutput ?? 0) == 0)
                    return 0;
                return ActualQuantityUOM / (InputQForFinalActualOutput ?? 1);
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForFinalActualOutput", "en{'Diff. Quantity for final actual output'}de{'Diff. Quantity for final actual output'}")]
        public double InputQForFinalActualOutputDiff
        {
            get
            {
                return ActualQuantityUOM - (InputQForFinalActualOutput ?? 0);
            }
        }

        #endregion

        #region InputQForFinalGoodActualOutput

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForFinalGoodActualOutput", "en{'Input Quantity for final good output (%)'}de{'Input Quantity for final good output (%)'}")]
        public double InputQForFinalGoodActualOutputPer
        {
            get
            {
                if (InputQForFinalGoodActualOutput == null || (InputQForFinalGoodActualOutput ?? 0) == 0)
                    return 0;
                return ActualQuantityUOM / (InputQForFinalGoodActualOutput ?? 1);
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForFinalGoodActualOutput", "en{'Diff. Quantity for final good output'}de{'Diff. Quantity for final good output'}")]
        public double InputQForFinalGoodActualOutputDiff
        {
            get
            {
                return ActualQuantityUOM - (InputQForFinalGoodActualOutput ?? 0);
            }
        }

        #endregion

        #region InputQForFinalScrapActualOutput

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForFinalScrapActualOutput", "en{'nput Quantity for final scraped output (%)'}de{'nput Quantity for final scraped output (%)'}")]
        public double InputQForFinalScrapActualOutputPer
        {
            get
            {
                if (InputQForFinalScrapActualOutput == null || (InputQForFinalScrapActualOutput ?? 0) == 0)
                    return 0;
                return ActualQuantityUOM / (InputQForFinalScrapActualOutput ?? 1);
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForFinalScrapActualOutput", "en{'Diff Quantity for final scraped output'}de{'Diff Quantity for final scraped output'}")]
        public double InputQForFinalScrapActualOutputDiff
        {
            get
            {
                return ActualQuantityUOM - (InputQForFinalScrapActualOutput ?? 0);
            }
        }

        #endregion

    }
}
