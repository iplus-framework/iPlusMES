// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿namespace gip.bso.masterdata
{
    public class FilterFacilityModel
    {
        public string Group { get;set; }
        public string[] IncludedFacilities { get;set; }
        public string[] ExcludedFacilities { get;set; }
    }
}
