using gip.core.datamodel;
using System;
using System.ComponentModel;

namespace gip.mes.datamodel
{
    public partial class ProdOrderPartslistPos
    {
        public bool IsExcludedFromStat
        {
            get
            {
                return Material == null || Material.SpecHeatCapacity > 4000;
            }
        }

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
        [ACPropertyInfo(999, "InputQForActualOutput", ConstIInputQForActual.InputQForActualOutputPer )]
        public double? InputQForActualOutputPer
        {
            get
            {
                if (InputQForActualOutput == null || Math.Abs(InputQForActualOutput.Value) <= double.Epsilon || IsExcludedFromStat)
                    return null;
                else if (IsBaseQuantityExcluded)
                    return 1;
                return ActualQuantityUOM / InputQForActualOutput;
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForActualOutput", ConstIInputQForActual.InputQForActualOutputDiff)]
        public double? InputQForActualOutputDiff
        {
            get
            {
                return InputQForActualOutput != null ? ActualQuantityUOM - InputQForActualOutput : null;
            }
        }

        #endregion

        #region InputQForGoodActualOutput

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForGoodActualOutput", ConstIInputQForActual.InputQForGoodActualOutputPer)]
        public double? InputQForGoodActualOutputPer
        {
            get
            {
                if (InputQForGoodActualOutput == null || Math.Abs(InputQForGoodActualOutput.Value) <= double.Epsilon|| IsExcludedFromStat)
                    return null;
                else if (IsBaseQuantityExcluded)
                    return 1;
                return ActualQuantityUOM / InputQForGoodActualOutput;
            }
        }

        [ACPropertyInfo(999, "InputQForGoodActualOutput", ConstIInputQForActual.InputQForGoodActualOutputDiff)]
        public double? InputQForGoodActualOutputDiff
        {
            get
            {
                return InputQForGoodActualOutput != null ? ActualQuantityUOM - InputQForGoodActualOutput : null;
            }
        }

        #endregion

        #region InputQForScrapActualOutput

        [ACPropertyInfo(999, "InputQForScrapActualOutput", ConstIInputQForActual.InputQForScrapActualOutputPer)]
        public double? InputQForScrapActualOutputPer
        {
            get
            {
                if (InputQForScrapActualOutput == null || Math.Abs(InputQForScrapActualOutput.Value) <= double.Epsilon|| IsExcludedFromStat)
                    return null;
                else if (IsBaseQuantityExcluded)
                    return null;
                return ActualQuantityUOM / InputQForScrapActualOutput;
            }
        }

        [ACPropertyInfo(999, "InputQForScrapActualOutput", ConstIInputQForActual.InputQForScrapActualOutputDiff)]
        public double? InputQForScrapActualOutputDiff
        {
            get
            {
                return InputQForScrapActualOutput != null ? ActualQuantityUOM - InputQForScrapActualOutput : null;
            }
        }

        #endregion

        #region InputQForFinalActualOutput

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForFinalActualOutput", ConstIInputQForActual.InputQForFinalActualOutputPer)]
        public double? InputQForFinalActualOutputPer
        {
            get
            {
                if (InputQForFinalActualOutput == null || Math.Abs(InputQForFinalActualOutput.Value) <= double.Epsilon|| IsExcludedFromStat)
                    return null;
                else if (IsBaseQuantityExcluded)
                    return 1;
                return ActualQuantityUOM / InputQForFinalActualOutput;
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForFinalActualOutput", ConstIInputQForActual.InputQForFinalActualOutputDiff)]
        public double? InputQForFinalActualOutputDiff
        {
            get
            {
                return InputQForFinalActualOutput != null ? ActualQuantityUOM - InputQForFinalActualOutput : null;
            }
        }

        #endregion

        #region InputQForFinalGoodActualOutput

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForFinalGoodActualOutput", ConstIInputQForActual.InputQForFinalGoodActualOutputPer)]
        public double? InputQForFinalGoodActualOutputPer
        {
            get
            {
                if (InputQForFinalGoodActualOutput == null || Math.Abs(InputQForFinalGoodActualOutput.Value) <= double.Epsilon|| IsExcludedFromStat)
                    return null;
                else if (IsBaseQuantityExcluded)
                    return 1;
                return ActualQuantityUOM / InputQForFinalGoodActualOutput;
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForFinalGoodActualOutput", ConstIInputQForActual.InputQForFinalGoodActualOutputDiff)]
        public double? InputQForFinalGoodActualOutputDiff
        {
            get
            {
                return InputQForFinalGoodActualOutput != null ? ActualQuantityUOM - InputQForFinalGoodActualOutput : null;
            }
        }

        #endregion

        #region InputQForFinalScrapActualOutput

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForFinalScrapActualOutput", ConstIInputQForActual.InputQForFinalScrapActualOutputPer)]
        public double? InputQForFinalScrapActualOutputPer
        {
            get
            {
                if (InputQForFinalScrapActualOutput == null || Math.Abs(InputQForFinalScrapActualOutput.Value) <= double.Epsilon|| IsExcludedFromStat)
                    return null;
                else if (IsBaseQuantityExcluded)
                    return null;
                return ActualQuantityUOM / InputQForFinalScrapActualOutput;
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, "InputQForFinalScrapActualOutput", ConstIInputQForActual.InputQForFinalScrapActualOutputDiff)]
        public double? InputQForFinalScrapActualOutputDiff
        {
            get
            {
                return InputQForFinalScrapActualOutput != null ? ActualQuantityUOM - InputQForFinalScrapActualOutput : null;
            }
        }

        #endregion

    }
}
