// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cCA")]
    public class CompanyAddress
    {
        [DataMember(Name = "ID")]
        public Guid CompanyAddressID
        {
            get; set;
        }

        [DataMember(Name = "N1")]
        public string Name1
        {
            get; set;
        }

        [DataMember(Name = "St")]
        public string Street
        {
            get; set;
        }

        [DataMember(Name = "PC")]
        public string PostCode
        {
            get; set;
        }

        public override string ToString()
        {
            string result = "";

            if (!string.IsNullOrEmpty(Name1))
            {
                result = Name1;
            }

            if (!string.IsNullOrEmpty(Street))
            {
                result = result + " " + Street;
            }

            return result;
        }
    }
}
