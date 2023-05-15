using gip.core.datamodel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'OrderLogPosView'}de{'OrderLogPosView'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "ProgramNo", "en{'Pr. No.'}de{'Pr.Auf. Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "ACUrl", "en{'ACUrl'}de{'ACUrl'}", "", "", true)]
    [ACPropertyEntity(3, "PosSequence", "en{'Sequence'}de{'Reihenfolge'}", "", "", true)]
    [ACPropertyEntity(4, "PosBatchNo", "en{'Batchnumber'}de{'Batchnummer'}", "", "", true)]
    [ACPropertyEntity(5, "MaterialPosTypeIndex", "en{'Positiontype'}de{'Posistionstyp'}", "", "", true)]
    [ACPropertyEntity(6, "PosMaterialNo", "en{'Material No.'}de{'Material-Nr.'}", "", "", true)]
    [ACPropertyEntity(7, "PosMaterialName", "en{'Material Desc. 1'}de{'Materialbez. 1'}", "", "", true)]
    [ACPropertyEntity(8, "PosTargetQuantityUOM", "en{'Target Inward Qty.'}de{'Ergebnismenge Soll'}", "", "", true)]
    [ACPropertyEntity(9, "PosActualQuantityUOM", "en{'Output Actual Quantity'}de{'Ergebnis-Ist'}", "", "", true)]
    [ACPropertyEntity(10, "PartslistSequence", "en{'Pl. Nr.'}de{'Stückl. Nr.'}", "", "", true)]
    [ACPropertyEntity(11, "ProdOrderProgramNo", "en{'Ordernumber'}de{'Auftragsnummer'}", "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + OrderLogPosView.ClassName, "en{'OrderLogPosView'}de{'OrderLogPosView'}", typeof(OrderLogPosView), OrderLogPosView.ClassName, "PosBatchNo", "PosBatchNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<OrderLogPosView>) })]
    [NotMapped]
    public partial class OrderLogPosView
    {
        [NotMapped]
        public const string ClassName = "OrderLogPosView";
    }
}
