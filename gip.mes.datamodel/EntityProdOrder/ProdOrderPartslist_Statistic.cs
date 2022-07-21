using gip.core.datamodel;

namespace gip.mes.datamodel
{
    public partial class ProdOrderPartslist
    {

        #region DifferenceQuantityPer

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

        #endregion

        #region InputQForActualOutput

        [ACPropertyInfo(999, "InputQForActualOutput", ConstIInputQForActual.InputQForActualOutput)]
        public double InputQForActualOutput
        {
            get
            {
                if (InputQForActualOutputPer == null || (InputQForActualOutputPer ?? 0) == 0)
                    return 0;
                return ActualQuantity / (InputQForActualOutputPer ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForActualOutput", ConstIInputQForActual.InputQForActualOutputDiff)]
        public double InputQForActualOutputDiff
        {
            get
            {
                return ActualQuantity - InputQForActualOutput;
            }
        }

        #endregion


        #region InputQForGoodActualOutput

        [ACPropertyInfo(999, "InputQForGoodActualOutput", ConstIInputQForActual.InputQForGoodActualOutput)]
        public double InputQForGoodActualOutput
        {
            get
            {
                if (InputQForGoodActualOutputPer == null || (InputQForGoodActualOutputPer ?? 0) == 0)
                    return 0;
                return ActualQuantity / (InputQForGoodActualOutputPer ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForGoodActualOutput", ConstIInputQForActual.InputQForGoodActualOutputDiff)]
        public double InputQForGoodActualOutputDiff
        {
            get
            {
                return ActualQuantity - InputQForGoodActualOutput;
            }
        }

        #endregion

        #region InputQForScrapActualOutput

        [ACPropertyInfo(999, "InputQForScrapActualOutput", ConstIInputQForActual.InputQForScrapActualOutput)]
        public double InputQForScrapActualOutput
        {
            get
            {
                if (InputQForScrapActualOutputPer == null || (InputQForScrapActualOutputPer ?? 0) == 0)
                    return 0;
                return ActualQuantity / (InputQForScrapActualOutputPer ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForScrapActualOutput", ConstIInputQForActual.InputQForScrapActualOutputDiff)]
        public double InputQForScrapActualOutputDiff
        {
            get
            {
                return ActualQuantity - InputQForScrapActualOutput;
            }
        }

        #endregion

        #region InputQForFinalActualOutput

        [ACPropertyInfo(999, "InputQForFinalActualOutput", ConstIInputQForActual.InputQForFinalActualOutput)]
        public double InputQForFinalActualOutput
        {
            get
            {
                if (InputQForFinalActualOutputPer == null || (InputQForFinalActualOutputPer ?? 0) == 0)
                    return 0;
                return ActualQuantity / (InputQForFinalActualOutputPer ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForFinalActualOutput", ConstIInputQForActual.InputQForFinalActualOutputDiff)]
        public double InputQForFinalActualOutputDiff
        {
            get
            {
                return ActualQuantity - InputQForFinalActualOutput;
            }
        }

        #endregion

        #region InputQForFinalGoodActualOutput

        [ACPropertyInfo(999, "InputQForFinalGoodActualOutput", ConstIInputQForActual.InputQForFinalGoodActualOutput)]
        public double InputQForFinalGoodActualOutput
        {
            get
            {
                if (InputQForFinalGoodActualOutputPer == null || (InputQForFinalGoodActualOutputPer ?? 0) == 0)
                    return 0;
                return ActualQuantity / (InputQForFinalGoodActualOutputPer ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForFinalGoodActualOutput", ConstIInputQForActual.InputQForFinalGoodActualOutputDiff)]
        public double InputQForFinalGoodActualOutputDiff
        {
            get
            {
                return ActualQuantity - InputQForFinalGoodActualOutput;
            }
        }

        #endregion

        #region InputQForFinalScrapActualOutput

        [ACPropertyInfo(999, "InputQForFinalScrapActualOutput", ConstIInputQForActual.InputQForFinalScrapActualOutput)]
        public double InputQForFinalScrapActualOutput
        {
            get
            {
                if (InputQForFinalScrapActualOutputPer == null || (InputQForFinalScrapActualOutputPer ?? 0) == 0)
                    return 0;
                return ActualQuantity / (InputQForFinalScrapActualOutputPer ?? 1);
            }
        }

        [ACPropertyInfo(999, "InputQForFinalScrapActualOutput", ConstIInputQForActual.InputQForFinalScrapActualOutputDiff)]
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
