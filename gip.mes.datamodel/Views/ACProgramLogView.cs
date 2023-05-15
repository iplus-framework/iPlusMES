using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using gip.core.datamodel;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'ACProgramLogView'}de{'ACProgramLogView'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "ProgramNo", "en{'Order No.'}de{'Auftragsnummer'}", "", "", true)]
    [ACPropertyEntity(2, "MaterialNo", "en{'Material-No.'}de{'Artikel-Nr.'}", "", "", true)]
    [ACPropertyEntity(3, "MaterialName", "en{'Material Desc.'}de{'Materialbez.'}", "", "", true)]
    [ACPropertyEntity(4, "MaterialName", "en{'Material Desc.'}de{'Materialbez.'}", "", "", true)]
    [ACPropertyEntity(5, "ACClassACIdentifier", "en{'Machine ID'}de{'Maschine ID'}", "", "", true)]
    [ACPropertyEntity(6, "OutwardTargetQuantityUOM", "en{'Input Target Quantity'}de{'Einsatz-Soll'}", "", "", true)]
    [ACPropertyEntity(7, "OutwardActualQuantityUOM", "en{'Input Actual Quantity'}de{'Einsatz-Ist'}", "", "", true)]
    [ACPropertyEntity(8, "InwardTargetQuantityUOM", "en{'Output Target Quantity'}de{'Ergebnis-Soll'}", "", "", true)]
    [ACPropertyEntity(9, "InwardActualQuantityUOM", "en{'Output Actual Quantity'}de{'Ergebnis-Ist'}", "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + ACProgramLogView.ClassName, "en{'ACProgramLogView'}de{'ACProgramLogView'}", typeof(ACProgramLogView), ACProgramLogView.ClassName, "ProgramNo", "ProgramNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<ACProgramLogView>) })]
    [NotMapped]
    public partial class ACProgramLogView
    {
        [NotMapped]
        public const string ClassName = "ACProgramLogView";

        [ACPropertyInfo(999, "Sn", "en{'Sn'}de{'Sn'}")]
        [NotMapped]
        public int Sn { get; set; }

        [NotMapped]
        private int Test { get; set; }

    }
}
