// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;

namespace gip.mes.cmdlet.DBSync
{
    public class DbSyncerInfo
    {
        public int DbSyncerInfoID { get; set; }
        public string DbSyncerInfoContextID { get; set; }
        public DateTime ScriptDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateAuthor { get; set; }
    }
}
