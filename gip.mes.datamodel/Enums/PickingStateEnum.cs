﻿using gip.core.datamodel;
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

    public enum PickingStateEnum : short
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
            AddEntry(PickingStateEnum.New, "en{'New'}de{'Neu'}");
            AddEntry(PickingStateEnum.InProcess, "en{'In process'}de{'In Bearbeitung'}");
            AddEntry(PickingStateEnum.Finished, "en{'Finished'}de{'Fertiggestellt'}");
            AddEntry(PickingStateEnum.Cancelled, "en{'Cancelled'}de{'Storniert'}");
        }
    }
}