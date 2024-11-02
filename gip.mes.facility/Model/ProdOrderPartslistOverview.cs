// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

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

        [ACPropertyInfo(101, Const_General, "en{'Material-No.'}de{'Material-Nr.'}")]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(102, Const_General, "en{'Material name'}de{'Materialname'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(103, Const_General, "en{'Department'}de{'Abteilung'}")]
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

        [ACPropertyInfo(301, Const_Output, "en{'Planned Output'}de{'Geplantes Ergebnis'}")]
        public double InwardPlannedQuantityUOM { get; set; }

        [ACPropertyInfo(302, Const_Output, "en{'Target Output'}de{'Soll Ergebnis'}")]
        public double InwardTargetQuantityUOM { get; set; }

        [ACPropertyInfo(303, Const_Output, "en{'Actual Output'}de{'Ist Ergebnis'}")]
        public double InwardActualQuantityUOM { get; set; }

        [ACPropertyInfo(304, Const_Output, "en{'Difference to Planned Q.'}de{'Differenz zur Planm.'}")]
        public double InwardDiffPlannedQuantity { get; set; }

        [ACPropertyInfo(305, Const_Output, "en{'Deviation to Planned Q. [%]'}de{'Abweichung zur Planm. [%]'}")]
        public double InwardDiffPlannedQuantityPer { get; set; }

        [ACPropertyInfo(306, Const_Output, "en{'Difference to Target-Q.'}de{'Differenz zur Sollm.'}")]
        public double InwardDiffQuantityUOM { get; set; }

        [ACPropertyInfo(307, Const_Output, "en{'Difference to Target-Quantity [%]'}de{'Differenz zur Sollm. [%]'}")]
        public double InwardDiffQuantityPer { get; set; }

        //[ACPropertyInfo(303, Const_Output, "en{'Diff. Out-In = O2 - I2'}de{'Diff. Erg.-Eins. = Erg2 - Ein2'}")]
        //public double InwardActualRestQuantityUOM { get; set; }

        [ACPropertyInfo(308, Const_Output, "en{'Good Quantity'}de{'Gutmenge'}")]
        public double InwardActualQuantityGoodUOM { get; set; }

        [ACPropertyInfo(309, Const_Output, "en{'Good Quantity [%]'}de{'Gutmenge [%]'}")]
        public double InwardActualQuantityGoodPer { get; set; }

        [ACPropertyInfo(310, Const_Output, "en{'Scrapped Quantity'}de{'Ausschussmenge'}")]
        public double InwardActualQuantityScrapUOM { get; set; }

        [ACPropertyInfo(311, Const_Output, "en{'Scrapped Quantity [%]'}de{'Ausschussmenge [%]'}")]
        public double InwardActualQuantityScrapPer { get; set; }

        #endregion

        #region Usage

        [ACPropertyInfo(400, Const_Usage, "en{'Target Usage (F1)'}de{'Soll Verw. (F1)'}")]
        public double UsageTargetQuantityUOM { get; set; }

        [ACPropertyInfo(401, Const_Usage, "en{'Actual Usage (F2)'}de{'Ist Verw. (F2)'}")]
        public double UsageActualQuantityUOM { get; set; }

        [ACPropertyInfo(402, Const_Usage, "en{'Diff. Usage (F2-F1)'}de{'Diff. Verw. (F2-F1)'}")]
        public double UsageDiffQuantityUOM { get; set; }

        [ACPropertyInfo(403, Const_Usage, "en{'Diff. Usage-Out (F2-B1)'}de{'Diff. Verw.-Erg. (F2-F1)'}")]
        public double UsageInwardDiffQuantityUOM { get; set; }

        #endregion

        #region Others
        public ProdOrderPartslist ProdOrderPartslist { get; set; }
        //public ProdOrderPartslist[] GroupedPartslists { get; set; }
        public ProdOrderPartslistOverview[] GroupedOverview { get; set; }

        [ACPropertyInfo(500, Const_Usage, "en{'InputQForActual'}de{'InputQForActual'}")]
        public InputQForActual InputQForActual { get; set; }

        public string[] MaterialNos { get; set; }

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
                // Calculate for ProdOrderPartslist values
                InwardActualQuantityGoodUOM = ProdOrderPartslist.ActualQuantityGoodUOM;
                InwardActualQuantityGoodPer = ProdOrderPartslist.ActualQuantityGoodPer;
                InwardActualQuantityScrapPer = ProdOrderPartslist.ActualQuantityScrapPer;
                InwardDiffPlannedQuantityPer = ProdOrderPartslist.DifferenceQuantityPer;
                InwardDiffPlannedQuantity = ProdOrderPartslist.DifferenceQuantity;
                InwardDiffQuantityPer = Math.Abs(InwardActualQuantityUOM) > Double.Epsilon && Math.Abs(InwardTargetQuantityUOM) > Double.Epsilon ? InwardActualQuantityUOM / InwardTargetQuantityUOM : 0;
            }
            else
                // Calculate for group by material values
                InwardDiffPlannedQuantity = InwardActualQuantityUOM - InwardPlannedQuantityUOM;

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
                InwardDiffQuantityPer += c.InwardDiffQuantityPer * c.InwardActualQuantityUOM / InwardActualQuantityUOM;
                InwardDiffPlannedQuantityPer += c.InwardDiffPlannedQuantityPer * c.InwardActualQuantityUOM / InwardActualQuantityUOM;
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
