// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'MachineMaterialView'}de{'MachineMaterialView'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Nr", "en{'Nr.'}de{'Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "ProgramNo", "en{'Pr. No.'}de{'Pr.Auf. Nr.'}", "", "", true)]
    [ACPropertyEntity(3, "MaterialNo", "en{'Material No.'}de{'Material-Nr.'}", "", "", true)]
    [ACPropertyEntity(4, "MaterialName1", "en{'Material Desc. 1'}de{'Materialbez. 1'}", "", "", true)]
    [ACPropertyEntity(5, "MachineName", "en{'Machine Name'}de{'Maschinename'}", "", "", true)]
    [ACPropertyEntity(6, "BasedOnMachineName", "en{'Baseclass'}de{'Basisklasse'}", "", "", true)]
    [ACPropertyEntity(7, "InwardTargetQuantityUOM", "en{'Target Inward Qty.'}de{'Ergebnismenge Soll'}", "", "", true)]
    [ACPropertyEntity(8, "InwardActualQuantityUOM", "en{'Output Actual Quantity'}de{'Ergebnis-Ist'}", "", "", true)]
    [ACPropertyEntity(9, "OutwardTargetQuantityUOM", "en{'Outward Target Qty(UOM)'}de{'Abgangsmenge Soll(BME)'}", "", "", true)]
    [ACPropertyEntity(10, "OutwardActualQuantityUOM", "en{'Input Actual Quantity'}de{'Einsatz-Ist'}", "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + MachineMaterialView.ClassName, "en{'MachineMaterialView'}de{'MachineMaterialView'}", typeof(MachineMaterialView), MachineMaterialView.ClassName, "Nr", "Nr")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MachineMaterialView>) })]
    [NotMapped]
    public partial class MachineMaterialView
    {
        [NotMapped]
        public const string ClassName = "MachineMaterialView";
    }
}
