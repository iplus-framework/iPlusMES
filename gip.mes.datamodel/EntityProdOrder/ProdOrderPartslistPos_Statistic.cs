using gip.core.datamodel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    public partial class ProdOrderPartslistPos
    {
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
