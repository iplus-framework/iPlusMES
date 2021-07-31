using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.processapplication
{
    //[DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Mode for ending BOM-State'}de{'Stücklistenstatus Beenden Modus'}", Global.ACKinds.TACEnum)]
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
}
