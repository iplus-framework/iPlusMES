// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.bso.iplus;
using gip.core.datamodel;

namespace gip.mes.cmdlet.DesignSync
{
    public delegate void ImportMessage(string msg);

    public class ACClassSyncBase
    {
        public event ImportMessage OnImportMessage;

        public string RootFolder { get; set; }
        public ACProjectManager ProjectManager { get; set; }
        public Database Database { get; set; }

        public void SendImportMessage(string msg)
        {
            if (OnImportMessage != null)
                OnImportMessage(msg);
        }
    }
}
