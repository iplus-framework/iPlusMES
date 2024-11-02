// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTFastViewModel'}de{'TandTFastViewModel'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]

    public class TandTFastViewModel
    {
        #region Order info
        public Guid ProdOrderPartslistID { get; set; }

        [ACPropertyInfo(50, nameof(ProductionDate), ConstApp.ProductionDate)]
        public DateTime ProductionDate { get; set; }

        [ACPropertyInfo(100, nameof(OrderNo), "en{'Program'}de{'AuftragNo.'}")]
        public string OrderNo { get; set; }

        [ACPropertyInfo(101, nameof(ConsumptionActualQuantity), "en{'Consumption Act. Quantity'}de{'Verbrauchsgesetz. Menge'}")]
        public double ConsumptionActualQuantity { get; set; }

        [ACPropertyInfo(102, nameof(ConsMDUnit), ConstApp.MDUnit)]
        public MDUnit ConsMDUnit { get; set; }
        #endregion

        #region ProdOrderPartslist info

        [ACPropertyInfo(200, nameof(MaterialNo), "en{'MaterialNo'}de{'MaterialNo.'}")]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(201, "en{'Material name'}de{'Materialname'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(202, nameof(TargetActualQuantityUOM), "en{'Actual Output'}de{'Ist Ergebnis'}")]
        public double TargetActualQuantityUOM { get; set; }

        [ACPropertyInfo(203, nameof(MDUnit), ConstApp.MDUnit)]
        public MDUnit MDUnit { get; set; }
        #endregion

        #region Final product info

        [ACPropertyInfo(300, nameof(FinalMaterialNo), "en{'Final MaterialNo'}de{'Finale MaterialNo.'}")]
        public string FinalMaterialNo { get; set; }

        [ACPropertyInfo(301, nameof(FinalMaterialName), "en{'Final Material name'}de{'Finale Materialname'}")]
        public string FinalMaterialName { get; set; }

        [ACPropertyInfo(302, nameof(FinalTargetActualQuantityUOM), "en{'Final Actual Output'}de{'Finale Ist Ergebnis'}")]
        public double FinalTargetActualQuantityUOM { get; set; }

        [ACPropertyInfo(303, nameof(FinalMDUnit), ConstApp.MDUnit)]
        public MDUnit FinalMDUnit { get; set; }
        #endregion

    }
}
