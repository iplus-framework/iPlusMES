// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System.Collections.Generic;
using System.IO;

namespace gip.mes.datamodel
{
    public class DocumentSearchResult
    {
        public DoumentFilter Filter { get; set; }

        public int ItemsCount { get; set; }
        public List<FileInfo> FileInfos { get; set; }
    }
}
