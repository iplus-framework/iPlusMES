using gip.core.datamodel;

namespace gip.mes.datamodel
{
#if NETFRAMEWORK
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Movement reasons'}de{'Bewegungsgründe'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListMovementReasonsEnum")]
#else
        [DataContract]
#endif
    public enum MovementReasonsEnum : short
    {
        Adjustment = 1, //Korrektur
        ZeroStock = 2, //Nullbestand
        Enabling = 3, //Freigabe
        Blocking = 4, //Sperrung            
        Consumption = 5, //Verbrauch
        Production = 6, //Herstellung
        GoodsReceipt = 7, //Wareneingang
        GoodsIssue = 8, //Warenausgang           
        Relocation = 9, //Warenumlagerung
        Inventory = 10, //Inventur
        ConsumptionWithoutBalance = 11, //Verbrauch ohne Bilanz
        ProductionWithoutBalance = 12, //Herstellung ohne Bilanz
        CorrectionFromERP = 90,
        Reject = 100
    }

#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Movement reasons'}de{'Bewegungsgründe'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListMovementReasonsEnum : ACValueItemList
    {
        public ACValueListMovementReasonsEnum() : base("MovementReasons")
        {
            AddEntry((int)MovementReasonsEnum.Adjustment, "en{'Adjustment'}de{'Korrektur'}");
            AddEntry((int)MovementReasonsEnum.ZeroStock, "en{'Zero Stock'}de{'Nullbestand'}");
            AddEntry((int)MovementReasonsEnum.Enabling, "en{'Enabling'}de{'Freigabe'}");
            AddEntry((int)MovementReasonsEnum.Blocking, "en{'Blocking'}de{'Sperrung'}");
            AddEntry((int)MovementReasonsEnum.Consumption, "en{'Consumption'}de{'Verbrauch'}");
            AddEntry((int)MovementReasonsEnum.Production, "en{'Production'}de{'Herstellung'}");
            AddEntry((int)MovementReasonsEnum.GoodsReceipt, "en{'Goods Receipt'}de{'Wareneingang'}");
            AddEntry((int)MovementReasonsEnum.GoodsIssue, "en{'Goods Issue'}de{'Warenausgang'}");
            AddEntry((int)MovementReasonsEnum.Relocation, "en{'Relocation'}de{'Umlagerung'}");
            AddEntry((int)MovementReasonsEnum.Inventory, "en{'Inventory'}de{'Inventur'}");
            AddEntry((int)MovementReasonsEnum.ConsumptionWithoutBalance, "en{'Consumption Without Balance'}de{'Verbrauch ohne Bilanz'}");
            AddEntry((int)MovementReasonsEnum.ProductionWithoutBalance, "en{'Production Without Balance'}de{'Herstellung ohne Bilanz'}");
            AddEntry((int)MovementReasonsEnum.Reject, "en{'Reject'}de{'Ausschuss'}");
        }
    }
#endif
}
