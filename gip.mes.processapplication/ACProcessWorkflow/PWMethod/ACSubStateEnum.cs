// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.facility;

namespace gip.mes.processapplication
{
    [Flags]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Substate of a process object'}de{'Unterzustand eines Prozessobjekts'}", Global.ACKinds.TACEnum)]
    public enum ACSubStateEnum : uint
    {
        SMIdle = 0x00000000,

        /// <summary>
        /// Cancel current Batch => Abort all active Nodes and then unload Workflow; 
        /// Batchabbruch => Breche alle aktiven Schritte ab und dann entlade Worflow
        /// </summary>
        SMBatchCancelled = 0x00000001,

        /// <summary>
        /// Abort dosings and switch to emptying mode; 
        /// Breche alle Dosierungen ab und fahre Anlage leer
        /// </summary>
        SMEmptyingMode = 0x00000002,

        /// <summary>
        /// Abort dosing and activate Discharging to extra target, then start dosing all components again
        /// Breche Dosierung ab entleere in Sonderziel und dosiere alle Komponenten von vorne
        /// </summary>
        SMDisThenNextComp = 0x00000004,

        /// <summary>
        /// Discharge scale because Maximum capacity of scale was reached. After that start again dosing with remaining components
        /// Führe Zwischenentleerung druch weil maximale Waagenkapazität erreicht ist. Starte danach wieder erneut Dosierungen für die restlichen Komponenten
        /// </summary>
        SMInterDischarging = 0x00000008,

        /// <summary>
        /// This is the last Batch. Don't restart a new Batch again. Order should be ended
        /// Das ist der letzte Batch. Es soll kein neuer Batch mehr gestartet werden. Der Auftrag soll beendet werden.
        /// </summary>
        SMLastBatchEndOrder = 0x00000010,

        SMLastBatchEndOrderEmptyingMode = 0x00000020,

        SMAllBatchesStarted = 0x00000040,

        SMAllowFollowingNode2Start = 0x00000080,

        SMRepeatGroup = 0x00000100,
        Res00000200 = 0x00000200,
        Res00000400 = 0x00000400,
        Res00000800 = 0x00000800,
        Res00001000 = 0x00001000,
        Res00002000 = 0x00002000,
        Res00004000 = 0x00004000,
        Res00008000 = 0x00008000,
        Res00010000 = 0x00010000,
        Res00020000 = 0x00020000,
        Res00040000 = 0x00040000,
        Res00080000 = 0x00080000,
        Res00100000 = 0x00100000,
        Res00200000 = 0x00200000,
        Res00400000 = 0x00400000,
        Res00800000 = 0x00800000,
        Res01000000 = 0x01000000,
        Res02000000 = 0x02000000,
        Res04000000 = 0x04000000,
        Res08000000 = 0x08000000,
        Res10000000 = 0x10000000,
        Res20000000 = 0x20000000,
        Res40000000 = 0x40000000,
        Res80000000 = 0x80000000,
    }

    //public static class ACSubStateConst
    //{
    //    public static ACSubStateEnum GetEnum(uint state)
    //    {
    //        return (ACSubStateEnum)state;
    //    }

    //    public static uint SetFlag(uint state, ACSubStateEnum flag)
    //    {
    //        return (uint)((ACSubStateEnum)state | flag);
    //    }
    //}

}
