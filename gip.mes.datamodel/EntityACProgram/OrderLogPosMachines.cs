using gip.core.datamodel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'OrderLogPosMachines'}de{'OrderLogPosMachines'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "ACUrl", "en{'ACUrl'}de{'ACUrl'}", "", "", true)]
    [ACPropertyEntity(2, "ProgramNo", "en{'Pr. No.'}de{'Pr.Auf. Nr.'}", "", "", true)]
    [ACPropertyEntity(3, "MachineName", "en{'Machine Name'}de{'Maschinename'}", "", "", true)]
    [ACPropertyEntity(4, "BasedOnMachine", "en{'Based on Machine'}de{'Basierend auf Maschine'}", "", "", true)]
    [ACPropertyEntity(5, "PosSequence", "en{'Sequence'}de{'Reihenfolge'}", "", "", true)]
    [ACPropertyEntity(6, "PosBatchNo", "en{'Batchnumber'}de{'Batchnummer'}", "", "", true)]
    [ACPropertyEntity(7, "ActualQuantityUOM", "en{'Output Actual Quantity'}de{'Ergebnis-Ist'}", "", "", true)]
    [ACPropertyEntity(8, "InsertDate", "en{'Insert date'}de{'Anlegedatum'}", "", "", true)]
    [ACPropertyEntity(9, "PartslistSequence", "en{'Pl. Nr.'}de{'Stückl. Nr.'}", "", "", true)]
    [ACPropertyEntity(10, "ProdOrderProgramNo", "en{'Ordernumber'}de{'Auftragsnummer'}", "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + OrderLogPosMachines.ClassName, "en{'OrderLogPosMachines'}de{'OrderLogPosMachines'}", typeof(OrderLogPosMachines), OrderLogPosMachines.ClassName, "PosBatchNo", "PosBatchNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<OrderLogPosMachines>) })]
    public partial class OrderLogPosMachines
    {
        [NotMapped]
        public const string ClassName = "OrderLogPosMachines";
    }
}
