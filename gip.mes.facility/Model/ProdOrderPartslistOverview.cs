using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Linq;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'ProdOrderPartslistOverview'}de{'ProdOrderPartslistOverview.'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class ProdOrderPartslistOverview
    {

        #region Const
        public const string Const_General = "General";
        public const string Const_Input = "Input";
        public const string Const_Output = "Output";
        public const string Const_Usage = "Output";
        #endregion

        #region General

        [ACPropertyInfo(100, Const_General, ConstApp.ProdOrderProgramNo)]
        public string ProgramNo { get; set; }

        [ACPropertyInfo(101, Const_General, "en{'MaterialNo'}de{'MaterialNo.'}")]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(102, Const_General, "en{'Material name'}de{'Materialname'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(103, Const_General, "en{'Department'}de{'Abteilung.'}")]
        public string DepartmentUserName { get; set; }

        [ACPropertyInfo(104, Const_General, ConstApp.MDUnit)]
        public string MDUnitName { get; set; }

        #endregion

        #region Input

        [ACPropertyInfo(200, Const_Input, "en{'Target Input (I1)'}de{'Soll Einsatz (Ein1)'}")]
        public double OutwardTargetQuantityUOM { get; set; }

        [ACPropertyInfo(201, Const_Input, "en{'Actual Input (I2)'}de{'Ist Einsatz (Ein2)'}")]
        public double OutwardActualQuantityUOM { get; set; }

        [ACPropertyInfo(202, Const_Input, "en{'Diff. Input = I2 - I1 (I3)'}de{'Diff. Einsatz = Ein2 - Ein1 (Ein3)'}")]
        public double OutwardDiffQuantityUOM { get; set; }

        #endregion

        #region Output

        [ACPropertyInfo(300, Const_Output, "en{'Planned Output'}de{'Geplantes Ergebnis'}")]
        public double InwardPlannedQuantityUOM { get; set; }

        [ACPropertyInfo(300, Const_Output, "en{'Target Output (O1)'}de{'Soll Ergebnis (Erg1)'}")]
        public double InwardTargetQuantityUOM { get; set; }

        [ACPropertyInfo(301, Const_Output, "en{'Actual Output (O2)'}de{'Ist Ergebnis (Erg2)'}")]
        public double InwardActualQuantityUOM { get; set; }

        [ACPropertyInfo(302, Const_Output, "en{'Diff. Output = O2 - O1 (O3)'}de{'Diff. Ergebnis = Erg2 - Erg1 (Erg3)'}")]
        public double InwardDiffQuantityUOM { get; set; }

        [ACPropertyInfo(302, Const_Output, "en{'Difference [%] to Target-Quantity'}de{'Differenz [%] zu Sollmenge'}")]
        public double InwardDifferenceQuantityPer { get; set; }

        //[ACPropertyInfo(303, Const_Output, "en{'Diff. Out-In = O2 - I2'}de{'Diff. Erg.-Eins. = Erg2 - Ein2'}")]
        //public double InwardActualRestQuantityUOM { get; set; }

        [ACPropertyInfo(304, Const_Output, "en{'Good Quantity'}de{'Gutmenge'}")]
        public double InwardActualQuantityGoodUOM { get; set; }

        [ACPropertyInfo(305, Const_Output, "en{'Good Quantity [%]'}de{'Gutmenge [%]'}")]
        public double InwardActualQuantityGoodPer { get; set; }

        [ACPropertyInfo(306, Const_Output, "en{'Scrapped Quantity'}de{'Ausschussmenge'}")]
        public double InwardActualQuantityScrapUOM { get; set; }

        [ACPropertyInfo(307, Const_Output, "en{'Scrapped Quantity [%]'}de{'Ausschussmenge [%]'}")]
        public double InwardActualQuantityScrapPer { get; set; }

        #endregion

        #region Usage

        [ACPropertyInfo(400, Const_Usage, "en{'Target Usage (U1)'}de{'Soll Verw. (V1)'}")]
        public double UsageTargetQuantityUOM { get; set; }

        [ACPropertyInfo(401, Const_Usage, "en{'Actual Usage (U2)'}de{'Ist Verw. (V2)'}")]
        public double UsageActualQuantityUOM { get; set; }

        [ACPropertyInfo(402, Const_Usage, "en{'Diff. Usage = U2 - U1'}de{'Diff. Verw.  = V2 - V1'}")]
        public double UsageDiffQuantityUOM { get; set; }

        [ACPropertyInfo(403, Const_Usage, "en{'Diff. Usg-Out = U2 - O2'}de{'Diff. Verw.-Erg.= U2 - O2'}")]
        public double UsageInwardDiffQuantityUOM { get; set; }

        #endregion

        #region Others
        public ProdOrderPartslist ProdOrderPartslist { get; set; }
        //public ProdOrderPartslist[] GroupedPartslists { get; set; }
        public ProdOrderPartslistOverview[] GroupedOverview { get; set; }

        [ACPropertyInfo(500, Const_Usage, "en{'InputQForActual'}de{'InputQForActual'}")]
        public InputQForActual InputQForActual { get; set; }

        #endregion

        #region Methods
        public void CalculateSums()
        {
            if (GroupedOverview == null || !GroupedOverview.Any())
                return;

            foreach (ProdOrderPartslistOverview c in GroupedOverview)
            {
                InwardPlannedQuantityUOM += c.InwardPlannedQuantityUOM;
                InwardActualQuantityUOM += c.InwardActualQuantityUOM;
                InwardTargetQuantityUOM += c.InwardTargetQuantityUOM;
                InwardActualQuantityGoodUOM += c.InwardActualQuantityGoodUOM;
                InwardActualQuantityScrapUOM += c.InwardActualQuantityScrapUOM;
                UsageTargetQuantityUOM += c.UsageTargetQuantityUOM;
                UsageActualQuantityUOM += c.UsageActualQuantityUOM;
            }
        }

        public void CalculateDiff()
        {
            if (ProdOrderPartslist != null)
            {
                InwardActualQuantityGoodUOM = ProdOrderPartslist.ActualQuantityGoodUOM;
                InwardActualQuantityGoodPer = ProdOrderPartslist.ActualQuantityGoodPer;
                InwardActualQuantityScrapPer = ProdOrderPartslist.ActualQuantityScrapPer;
                InwardDifferenceQuantityPer = ProdOrderPartslist.DifferenceQuantityPer;
            }

            InwardDiffQuantityUOM = InwardActualQuantityUOM - InwardTargetQuantityUOM;
            UsageDiffQuantityUOM = UsageActualQuantityUOM - UsageTargetQuantityUOM;
            UsageInwardDiffQuantityUOM = UsageActualQuantityUOM - InwardActualQuantityUOM;
        }

        public void CalculateStatistics()
        {
            InputQForActual = new InputQForActual()
            {
                //InputQForActualOutput = ProdOrderPartslist.InputQForActualOutput,
                //InputQForActualOutputDiff = ProdOrderPartslist.InputQForActualOutputDiff,
                InputQForActualOutputPer = ProdOrderPartslist.InputQForActualOutputPer ?? 0,

                //InputQForGoodActualOutput = ProdOrderPartslist.InputQForGoodActualOutput,
                //InputQForGoodActualOutputDiff = ProdOrderPartslist.InputQForGoodActualOutputDiff,
                InputQForGoodActualOutputPer = ProdOrderPartslist.InputQForGoodActualOutputPer ?? 0,

                //InputQForScrapActualOutput = ProdOrderPartslist.InputQForScrapActualOutput,
                //InputQForScrapActualOutputDiff = ProdOrderPartslist.InputQForScrapActualOutputDiff,
                InputQForScrapActualOutputPer = ProdOrderPartslist.InputQForScrapActualOutputPer ?? 0,


                //InputQForFinalActualOutput = ProdOrderPartslist.InputQForFinalActualOutput,
                //InputQForFinalActualOutputDiff = ProdOrderPartslist.InputQForFinalActualOutputDiff,
                InputQForFinalActualOutputPer = ProdOrderPartslist.InputQForFinalActualOutputPer ?? 0,

                //InputQForFinalGoodActualOutput = ProdOrderPartslist.InputQForFinalGoodActualOutput,
                //InputQForFinalGoodActualOutputDiff = ProdOrderPartslist.InputQForFinalGoodActualOutputDiff,
                InputQForFinalGoodActualOutputPer = ProdOrderPartslist.InputQForFinalGoodActualOutputPer ?? 0,

                //InputQForFinalScrapActualOutput = ProdOrderPartslist.InputQForFinalScrapActualOutput,
                //InputQForFinalScrapActualOutputDiff = ProdOrderPartslist.InputQForFinalScrapActualOutputDiff,
                InputQForFinalScrapActualOutputPer = ProdOrderPartslist.InputQForFinalScrapActualOutputPer ?? 0
            };
        }

        public void CalculateGroupedStatistics()
        {
            InputQForActual = new InputQForActual();

            if (GroupedOverview == null || !GroupedOverview.Any())
                return;

            if (InwardActualQuantityUOM <= 0)
                return;

            foreach (ProdOrderPartslistOverview c in GroupedOverview)
            {
                InwardDifferenceQuantityPer += c.InwardDifferenceQuantityPer * c.InwardActualQuantityUOM / InwardActualQuantityUOM;
                InwardActualQuantityGoodPer += c.InwardActualQuantityGoodPer * c.InwardActualQuantityUOM / InwardActualQuantityUOM;
                InwardActualQuantityScrapPer += c.InwardActualQuantityScrapPer * c.InwardActualQuantityUOM / InwardActualQuantityUOM;

                InputQForActual.InputQForActualOutputPer += c.ProdOrderPartslist.InputQForActualOutputPer != null ? c.ProdOrderPartslist.InputQForActualOutputPer.Value * c.InwardActualQuantityUOM / InwardActualQuantityUOM : 0;
                InputQForActual.InputQForGoodActualOutputPer += c.ProdOrderPartslist.InputQForGoodActualOutputPer != null ? c.ProdOrderPartslist.InputQForGoodActualOutputPer.Value * c.InwardActualQuantityUOM / InwardActualQuantityUOM : 0;
                InputQForActual.InputQForGoodActualOutputPer += c.ProdOrderPartslist.InputQForScrapActualOutputPer != null ? c.ProdOrderPartslist.InputQForScrapActualOutputPer.Value * c.InwardActualQuantityUOM / InwardActualQuantityUOM : 0;
                InputQForActual.InputQForFinalActualOutputPer += c.ProdOrderPartslist.InputQForFinalActualOutputPer != null ? c.ProdOrderPartslist.InputQForFinalActualOutputPer.Value * c.InwardActualQuantityUOM / InwardActualQuantityUOM : 0;
                InputQForActual.InputQForFinalGoodActualOutputPer += c.ProdOrderPartslist.InputQForFinalGoodActualOutputPer != null ? c.ProdOrderPartslist.InputQForFinalGoodActualOutputPer.Value * c.InwardActualQuantityUOM / InwardActualQuantityUOM : 0;
                InputQForActual.InputQForFinalScrapActualOutputPer += c.ProdOrderPartslist.InputQForFinalScrapActualOutputPer != null ? c.ProdOrderPartslist.InputQForFinalScrapActualOutputPer.Value * c.InwardActualQuantityUOM / InwardActualQuantityUOM : 0;
            }
        }
        #endregion
    }
}
