using gip.core.datamodel;

namespace gip.mes.datamodel
{
    public partial class ProdOrderPartslistPos
    {

        #region  DifferenceQuantityPer

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

        #endregion

        #region InputQForActualOutput

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForActualOutput", ConstIInputQForActual.InputQForActualOutputPer)]
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
        [ACPropertyInfo(999, "InputQForActualOutput", ConstIInputQForActual.InputQForActualOutputDiff)]
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
        [ACPropertyInfo(999, "InputQForGoodActualOutput", ConstIInputQForActual.InputQForGoodActualOutputPer)]
        public double InputQForGoodActualOutputPer
        {
            get
            {
                if (InputQForGoodActualOutput == null || (InputQForGoodActualOutput ?? 0) == 0)
                    return 0;
                return ActualQuantityUOM / (InputQForGoodActualOutput ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForGoodActualOutput", ConstIInputQForActual.InputQForGoodActualOutputDiff)]
        public double InputQForGoodActualOutputDiff
        {
            get
            {
                return ActualQuantityUOM - (InputQForGoodActualOutput ?? 0);
            }
        }

        #endregion

        #region InputQForScrapActualOutput

        [ACPropertyInfo(999, "InputQForScrapActualOutput", ConstIInputQForActual.InputQForScrapActualOutputPer)]
        public double InputQForScrapActualOutputPer
        {
            get
            {
                if (InputQForScrapActualOutput == null || (InputQForScrapActualOutput ?? 0) == 0)
                    return 0;
                return ActualQuantityUOM / (InputQForScrapActualOutput ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForScrapActualOutput", ConstIInputQForActual.InputQForScrapActualOutputDiff)]
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
        [ACPropertyInfo(999, "InputQForFinalActualOutput", ConstIInputQForActual.InputQForFinalActualOutputPer)]
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
        [ACPropertyInfo(999, "InputQForFinalActualOutput", ConstIInputQForActual.InputQForFinalActualOutputDiff)]
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
        [ACPropertyInfo(999, "InputQForFinalGoodActualOutput", ConstIInputQForActual.InputQForFinalGoodActualOutputPer)]
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
        [ACPropertyInfo(999, "InputQForFinalGoodActualOutput", ConstIInputQForActual.InputQForFinalGoodActualOutputDiff)]
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
        [ACPropertyInfo(999, "InputQForFinalScrapActualOutput", ConstIInputQForActual.InputQForFinalScrapActualOutputPer)]
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
        [ACPropertyInfo(999, "InputQForFinalScrapActualOutput", ConstIInputQForActual.InputQForFinalScrapActualOutputDiff)]
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
