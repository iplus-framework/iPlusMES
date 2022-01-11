using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
#if NETFRAMEWORK
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Picking state'}de{'Kommissionsstatus'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListPickingStateEnum")]
#else
    [DataContract]
#endif

    public enum PickingState : short
    {
        New = 0,
        InProcess = 1,
        Finished = 2,
        Cancelled = 3,
    }


    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Picking state'}de{'Zustand der Kommissionierung'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListPickingStateEnum : ACValueItemList
    {
        public ACValueListPickingStateEnum() : base("PickingState")
        {
            AddEntry((short)PickingState.New, "en{'New'}de{'Neu'}");
            AddEntry((short)PickingState.InProcess, "en{'In process'}de{'In Bearbeitung'}");
            AddEntry((short)PickingState.Finished, "en{'Finished'}de{'Fertiggestellt'}");
            AddEntry((short)PickingState.Cancelled, "en{'Cancelled'}de{'Storniert'}");
        }
    }
}
