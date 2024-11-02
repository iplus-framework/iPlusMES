// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿namespace gip.mes.datamodel
{
    public class DoumentFilter
    {
        public string ACTypeACUrl { get; set; }
        public string ACObjectACUrl { get; set; }

        public string Name { get; set; }
        public string[] Extensions { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }  
    }
}
