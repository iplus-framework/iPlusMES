// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'InputOverview'}de{'InputOverview.'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]

    public class InputOverview
    {

        #region Final product

        [ACPropertyInfo(301, "General", "en{'Final product No.'}de{'Endprodukt Nr.'}")]
        public string FinalProductMaterialNo { get; set; }

        [ACPropertyInfo(302, "General", "en{'Final productname'}de{'Endproduktname'}")]
        public string FinalProductMaterialName { get; set; }

        #endregion

        #region General

        [ACPropertyInfo(101, "General", "en{'Material-No.'}de{'Material-Nr.'}")]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(102, "General", "en{'Material name'}de{'Materialname'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(103, "General", ConstApp.MDUnit)]
        public string MDUnitName { get; set; }

        #endregion


        #region Quantities

        [ACPropertyInfo(400, "Quantities", "en{'Target Q.'}de{'Sollmenge'}")]
        public double TargetQuantityUOM { get; set; }

        [ACPropertyInfo(401, "Quantities", "en{'Actual Q.'}de{'Istmenge'}")]
        public double ActualQuantityUOM { get; set; }

        [ACPropertyInfo(402, "Quantities", "en{'Diff. to target Q.'}de{'Diff. zur Sollm.'}")]
        public double DiffQuantityUOM { get; set; }

        [ACPropertyInfo(403, "Quantities", "en{'Diff. to target Q. %'}de{'Diff. zur Sollm. %'}")]
        public double DiffQuantityPer { get; set; }

        [ACPropertyInfo(404, "Quantities", "en{'Planned Q.'}de{'Geplante M.'}")]
        public double PlannedQuantityUOM { get; set; }

        [ACPropertyInfo(405, "Quantities", "en{'Diff. to planned Q.'}de{'Diff. zur Planm. %'}")]
        public double DiffTargetPlanedQuantityUOM { get; set; }

        [ACPropertyInfo(406, "Quantities", "en{'Zero posting'}de{'Null Buchung'}")]
        public double ZeroPostingQuantityUOM { get; set; }


        [ACPropertyInfo(500, "", "en{'InputQForActual'}de{'InputQForActual'}")]
        public InputQForActual InputQForActual { get; set; }

        #endregion

        public IEnumerable<ProdOrderPartslistPos> GroupedPos { get; set; }

        #region Methods


        public void CalculateDiff()
        {
            DiffQuantityUOM = ActualQuantityUOM - TargetQuantityUOM;
            DiffTargetPlanedQuantityUOM = TargetQuantityUOM - PlannedQuantityUOM;
            DiffQuantityPer = Math.Abs(TargetQuantityUOM) > double.Epsilon ? ActualQuantityUOM / TargetQuantityUOM : 0;
        }

        public void CalculateStatistics()
        {
            if (GroupedPos == null)
                return;

            InputQForActual = new InputQForActual();

            foreach (ProdOrderPartslistPos c in GroupedPos)
            {
                InputQForActual.InputQForActualOutput += c.InputQForActualOutput ?? 0;
                InputQForActual.InputQForGoodActualOutput += c.InputQForGoodActualOutput ?? 0;
                InputQForActual.InputQForScrapActualOutput += c.InputQForScrapActualOutput ?? 0;
                InputQForActual.InputQForFinalActualOutput += c.InputQForFinalActualOutput ?? 0;
                InputQForActual.InputQForFinalGoodActualOutput += c.InputQForFinalGoodActualOutput ?? 0;
                InputQForActual.InputQForFinalScrapActualOutput += c.InputQForFinalScrapActualOutput ?? 0;

                InputQForActual.InputQForActualOutputDiff += c.InputQForActualOutputDiff ?? 0;
                InputQForActual.InputQForGoodActualOutputDiff += c.InputQForGoodActualOutputDiff ?? 0;
                InputQForActual.InputQForScrapActualOutputDiff += c.InputQForScrapActualOutputDiff ?? 0;
                InputQForActual.InputQForFinalActualOutputDiff += c.InputQForFinalActualOutputDiff ?? 0;
                InputQForActual.InputQForFinalGoodActualOutputDiff += c.InputQForFinalGoodActualOutputDiff ?? 0;
                InputQForActual.InputQForFinalScrapActualOutputDiff += c.InputQForFinalScrapActualOutputDiff ?? 0;

                PlannedQuantityUOM += c.TargetQuantityUOM;
                ActualQuantityUOM += c.ActualQuantityUOM;
            }

            foreach (ProdOrderPartslistPos c in GroupedPos)
            {
                InputQForActual.InputQForActualOutputPer += c.InputQForActualOutputPer != null ? c.InputQForActualOutputPer.Value * c.ActualQuantityUOM / ActualQuantityUOM : 0;
                InputQForActual.InputQForGoodActualOutputPer += c.InputQForGoodActualOutputPer != null ? c.InputQForGoodActualOutputPer.Value * c.ActualQuantityUOM  / ActualQuantityUOM : 0;
                InputQForActual.InputQForScrapActualOutputPer += c.InputQForScrapActualOutputPer != null ? c.InputQForScrapActualOutputPer.Value * c.ActualQuantityUOM / ActualQuantityUOM : 0;
                InputQForActual.InputQForFinalActualOutputPer += c.InputQForFinalActualOutputPer != null ? c.InputQForFinalActualOutputPer.Value * c.ActualQuantityUOM / ActualQuantityUOM : 0;
                InputQForActual.InputQForFinalGoodActualOutputPer += c.InputQForFinalGoodActualOutputPer != null ? c.InputQForFinalGoodActualOutputPer.Value * c.ActualQuantityUOM  / ActualQuantityUOM : 0;
                InputQForActual.InputQForFinalScrapActualOutputPer += c.InputQForFinalScrapActualOutputPer != null ? c.InputQForFinalScrapActualOutputPer.Value * c.ActualQuantityUOM / ActualQuantityUOM : 0;
            }
        }
        #endregion
    }
}
