// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cMDPT")]
    public class MDPickingType
    {
        [DataMember(Name = "ID")]
        public Guid MDPickingTypeID
        {
            get; set;
        }

        [DataMember]
        public string MDKey
        {
            get;
            set;
        }

        [DataMember(Name = "MDPTi")]
        public short MDPickingTypeIndex
        {
            get; set;
        }

        [IgnoreDataMember]
        public GlobalApp.PickingType PickingType
        {
            get
            {
                return (GlobalApp.PickingType)MDPickingTypeIndex;
            }
            set
            {
                MDPickingTypeIndex = (Int16)value;
            }
        }

        [DataMember(Name = "MDPTT")]
        public string MDPickingTypeTrans
        {
            get; set;
        }

        [IgnoreDataMember]
        public string MDPickingTypeName
        {
            get
            {
                return Translator.GetTranslation(MDPickingTypeTrans);
            }
        }
    }
}
