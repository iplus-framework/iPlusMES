using gip.core.datamodel;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Enum CompanyMaterialTypes
    /// </summary>
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'MRP procedure'}de{'MRP Verfahren'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListBatchMRPProcedure")]
    public enum MRPProcedure : short
    {
        None = 0,
        /// <summary>
        /// Requirement based => According orders
        /// </summary>
        RequirementBased = 1,

        /// <summary>
        /// According stock history
        /// </summary>
        ConsumptionBased = 2,
    }

    [ACClassInfo(Const.PackName_VarioAutomation, ConstApp.MRPProcedure, Global.ACKinds.TACEnumACValueList)]
    public class ACValueListBatchMRPProcedure : ACValueItemList
    {
        
        public ACValueListBatchMRPProcedure() : base("MRPProcedureIndex")
        {
            AddEntry((short)MRPProcedure.None, "en{'None'}de{'Keine'}");
            AddEntry((short)MRPProcedure.RequirementBased, "en{'Use from/to values'}de{'Nach Von/Bis-Batch-Nr.'}");
            AddEntry((short)MRPProcedure.ConsumptionBased, "en{'Consumption based (According stock history)'}de{'Verbrauchsgesteuert (Nach Lagerhistorie)'}");
        }
    }
}
