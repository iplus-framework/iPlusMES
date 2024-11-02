// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.facility.TandTv3;

namespace gip.mes.facility
{
    public interface ITandTv3Manager : IACComponent, ITandTFetchCharge
    {
        void StartTandTBSO(ACBSO bso, TandTv3FilterTracking filter);

        TandTv3.TandTResult DoTracking(DatabaseApp databaseApp, TandTv3FilterTracking filter, string vBUserNo, bool useGroupResult);
        TandTv3.TandTResult DoSelect(DatabaseApp databaseApp, TandTv3FilterTracking filter, string vbUserNo, bool useGroupResult);
        MsgWithDetails DoDeleteTracking(DatabaseApp databaseApp, TandTv3FilterTracking filter);

        TandTv3Command TandTv3Command { get; set; }

    }
}
