// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;

namespace gip.bso.logistics
{
    /// <summary>
    /// Empty stub of a business object for picking orders processed directly at a workplace (workplace picking).
    /// The class currently contains no properties or methods; the functionality is not implemented yet.
    /// The ACClassInfo caption ("Picking Order over workplace") reserves the menu entry for this future workplace-based picking app.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Picking Order over workplace'}de{'Kommissionierauftrag über Arbeitsplatz'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true,
        Description = @"Empty stub of a business object for picking orders processed directly at a workplace (workplace picking).
        The class currently contains no properties or methods; the functionality is not implemented yet.
        The ACClassInfo caption (Picking Order over workplace) reserves the menu entry for this future workplace-based picking app.")]
    public class BSOPickingWorkplace
    {
    }
}
