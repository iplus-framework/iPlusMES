using gip.core.datamodel;

namespace gip.mes.datamodel
{

#if NETFRAMEWORK
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.SiloTitleMode, Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListSiloTitleModeEnum")]
#else
    [DataContract]
#endif

    public enum SiloTitleModeEnum : short
    {
        DoNothing = 0,
        ShowPartslistMaterial = 1,
        ShowProductMaterial = 2
    }


    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.SiloTitleMode, Global.ACKinds.TACEnumACValueList)]
    public class ACValueListSiloTitleModeEnum : ACValueItemList
    {
        public ACValueListSiloTitleModeEnum() : base("SiloTitleMode")
        {
            AddEntry(SiloTitleModeEnum.DoNothing, "en{'Do nothing'}de{'Nichts tun'}");
            AddEntry(SiloTitleModeEnum.ShowPartslistMaterial, "en{'Show recipe material'}de{'Rezeptmaterial anzeigen'}");
            AddEntry(SiloTitleModeEnum.ShowProductMaterial, "en{'Show product material'}de{'Produktmaterial anzeigen'}");
        }
    }
}
