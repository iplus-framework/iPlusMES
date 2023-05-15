using gip.core.datamodel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'OrderLogRelView'}de{'OrderLogRelView'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "ACUrl", "en{'ACUrl'}de{'ACUrl'}", "", "", true)]
    [ACPropertyEntity(2, "MachineName", "en{'Machine Name'}de{'Maschinename'}", "", "", true)]
    [ACPropertyEntity(3, "BasedOnMachine", "en{'Based on Machine'}de{'Basierend auf Maschine'}", "", "", true)]
    [ACPropertyEntity(4, "RelSequence", "en{'Input sequence'}de{'Einsatzreihe'}", "", "", true)]
    [ACPropertyEntity(5, "MaterialNo", "en{'Material No.'}de{'Material-Nr.'}", "", "", true)]
    [ACPropertyEntity(6, "MaterialName1", "en{'Material Desc. 1'}de{'Materialbez. 1'}", "", "", true)]
    [ACPropertyEntity(7, "ActualQuantityUOM", "en{'Input Actual Quantity (UOM)'}de{'Einsat Istmenge (BME)'}", "", "", true)]
    [ACPropertyEntity(8, "TargetQuantityUOM", "en{'Input Target Quantity (UOM)'}de{'Einsatz Sollmenge (BME)'}", "", "", true)]
    [ACPropertyEntity(9, "PosSequence", "en{'Pl. Nr.'}de{'Stückl. Nr.'}", "", "", true)]
    [ACPropertyEntity(10, "PosBatchNo", "en{'BatchNo'}de{'BatchNo'}", "", "", true)]
    [ACPropertyEntity(11, "MaterialPosTypeIndex", "en{'Positiontype'}de{'Posistionstyp'}", "", "", true)]
    [ACPropertyEntity(12, "PosMaterialNo", "en{'Material No.'}de{'Material-Nr.'}", "", "", true)]
    [ACPropertyEntity(13, "PosMaterialName", "en{'Material Desc. 1'}de{'Materialbez. 1'}", "", "", true)]
    [ACPropertyEntity(14, "PosTargetQuantityUOM", "en{'Target Inward Qty.'}de{'Ergebnismenge Soll'}", "", "", true)]
    [ACPropertyEntity(15, "PosActualQuantityUOM", "en{'Output Actual Quantity'}de{'Ergebnis-Ist'}", "", "", true)]
    [ACPropertyEntity(16, "InsertDate", "en{'Insert date'}de{'Anlegedatum'}", "", "", true)]
    [ACPropertyEntity(17, "PartslistSequence", "en{'Pl. Nr.'}de{'Stückl. Nr.'}", "", "", true)]
    [ACPropertyEntity(18, "ProdOrderProgramNo", "en{'Ordernumber'}de{'Auftragsnummer'}", "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + OrderLogRelView.ClassName, "en{'OrderLogRelView'}de{'OrderLogRelView'}", typeof(OrderLogRelView), OrderLogRelView.ClassName, "PosBatchNo", "PosBatchNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<OrderLogRelView>) })]
    [NotMapped]
    public partial class OrderLogRelView
    {
        [NotMapped]
        public const string ClassName = "OrderLogRelView";
    }
}
