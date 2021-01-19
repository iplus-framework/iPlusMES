using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cMDU")]
    public class MDUnit
    {
        [DataMember(Name = "ID")]
        public Guid MDUnitID
        {
            get; set;
        }

        [DataMember(Name = "MDNT")]
        public string MDUnitNameTrans
        { 
            get;set;
        }

        [DataMember(Name = "ST")]
        public string SymbolTrans
        {
            get;set;
        }

        public string MDUnitName
        {
            get
            {
                return Translator.GetTranslation(MDUnitNameTrans);
            }
        }

        public string Symbol
        {
            get => Translator.GetTranslation(SymbolTrans);
        }
    }
}
