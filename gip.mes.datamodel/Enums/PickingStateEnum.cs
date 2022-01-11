using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Picking state'}de{'Buchungsverhalten'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListPickingStateEnum : ACValueItemList
    {
        public ACValueListPickingStateEnum() : base("PickingState")
        {
            AddEntry((short)GlobalApp.PickingState.New, "en{'New'}de{'Neu'}");
            AddEntry((short)GlobalApp.PickingState.InProcess, "en{'In process'}de{'In Bearbeitung'}");
            AddEntry((short)GlobalApp.PickingState.Finished, "en{'Finished'}de{'Fertiggestellt'}");
            AddEntry((short)GlobalApp.PickingState.Cancelled, "en{'Cancelled'}de{'Storniert'}");
        }
    }
}
