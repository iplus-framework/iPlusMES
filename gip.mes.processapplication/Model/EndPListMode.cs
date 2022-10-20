using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.processapplication
{
    //[DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Mode for ending BOM-State'}de{'Stücklistenstatus Beenden Modus'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.processapplication.ACValueListEndPListMode")]
    public enum EndPListMode : short
    {
        /// <summary>
        /// Doesn't ends the BOM-State automatically
        /// </summary>
        DoNothing = 0,

        /// <summary>
        /// Ends if all batchplans are completed even if the order size is not reached
        /// </summary>
        AllBatchPlansCompleted = 1,

        /// <summary>
        /// Ends if the order size is reached (Production tolerance)
        /// </summary>
        OrderSizeReached = 2,
    }

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Mode for ending BOM-State'}de{'Stücklistenstatus Beenden Modus'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListEndPListMode : ACValueItemList
    {
        public ACValueListEndPListMode() : base("EndPListMode")
        {
            AddEntry(EndPListMode.DoNothing, "en{'No Action'}de{'Keine Aktion'}");
            AddEntry(EndPListMode.AllBatchPlansCompleted, "en{'Ends if all batchplans are completed'}de{'Beenden wenn alle Batchpläne abgearbeitet'}");
            AddEntry(EndPListMode.OrderSizeReached, "en{'Ends if the order size is reached'}de{'Beenden wenn Auftragsgröße erreicht'}");
        }
    }
}
