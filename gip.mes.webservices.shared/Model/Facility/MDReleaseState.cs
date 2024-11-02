// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cMDRS")]
    public class MDReleaseState
    {
        [DataMember(Name = "ID")]
        public Guid MDReleaseStateID
        {
            get; set;
        }

        [DataMember(Name = "MDNT")]
        public string MDNameTrans
        { 
            get;set;
        }

        public string MDReleaseStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
        }

        [DataMember(Name = "MDRSi")]
        public short MDReleaseStateIndex
        {
            get; set;
        }

        [IgnoreDataMember]
        public ReleaseStates ReleaseState
        {
            get
            {
                return (ReleaseStates)MDReleaseStateIndex;
            }
            set
            {
                MDReleaseStateIndex = (short)value;
            }
        }

        public enum ReleaseStates : short
        {
            Free = 1, // freigegebener Bestand
            AbsFree = 2, // Absolut gesperrten Bestand freigeben, anschließender Status ist Free
            Locked = 3, // gesperrter Bestand
            AbsLocked = 4 // absolut gesperrter Bestand. Kann nur mit AbsFree zurückgesetzt werden
        }

    }
}
