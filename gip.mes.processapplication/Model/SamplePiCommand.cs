using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.processapplication
{
    //[DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Sample weighing command'}de{'Stichprobenwiegung Kommando'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.processapplication.ACValueListSamplePiCommand")]
    public enum SamplePiCommand : short
    {
        /// <summary>
        /// Send a order to all Pi-Boxes 
        /// </summary>
        SendOrder = 0,

        /// <summary>
        /// Read statistics from Pi-Boxes
        /// </summary>
        StopOrderAndRead = 1,
    }

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Sample weighing command'}de{'Stichprobenwiegung Kommando'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListSamplePiCommand : ACValueItemList
    {
        public ACValueListSamplePiCommand() : base("SamplePiCommand")
        {
            AddEntry(SamplePiCommand.SendOrder, "en{'Start order on sample light box'}de{'Start Auftrag auf Stichproben Ampelbox'}");
            AddEntry(SamplePiCommand.StopOrderAndRead, "en{'Stop order and read statistics'}de{'Stoppe Auftrag und lese Statistiken'}");
        }
    }
}
