// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Enum BatchPlanMode
    /// </summary>
#if NETFRAMEWORK
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, ConstApp.ESMDBatchPlanMode, Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListBatchPlanMode")]
#else
        [DataContract]
#endif

    public enum BatchPlanMode : short
    {
        UseFromTo = 0,
        UseBatchCount = 1,
        UseTotalSize = 2,
    }


#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioAutomation, ConstApp.ESMDBatchPlanMode, Global.ACKinds.TACEnumACValueList)]
    public class ACValueListBatchPlanMode : ACValueItemList
    {
        public ACValueListBatchPlanMode() : base("BatchPlanMode")
        {
            AddEntry(BatchPlanMode.UseFromTo, "en{'Use from/to values'}de{'Nach Von/Bis-Batch-Nr.'}");
            AddEntry(BatchPlanMode.UseBatchCount, "en{'Use target batch count'}de{'Nach Soll-Batchzahl'}");
            AddEntry(BatchPlanMode.UseTotalSize, "en{'Use total size'}de{'Nach Gesamtgröße'}");

        }
    }
#endif
}
