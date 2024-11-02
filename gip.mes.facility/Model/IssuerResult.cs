// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public class IssuerResult
    {
        public string IssuerMessage { get; set; }

        public List<CompanyPerson> CompanyPeople{get;set;}
        
        public CompanyAddress IssuerCompanyAddress{get;set;}
        public CompanyPerson IssuerCompanyPerson{get;set;}
    }
}
