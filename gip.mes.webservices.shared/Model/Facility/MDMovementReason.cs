using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cMDMR")]
    public class MDMovementReason
    {
        [DataMember(Name = "ID")]
        public Guid MDMovementReasonID
        {
            get; set;
        }

        [DataMember(Name = "MDNT")]
        public string MDNameTrans
        {
            get; set;
        }

        public string MDMovementReasonName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
        }

        [DataMember(Name = "MDMRi")]
        public short MDMovementReasonIndex
        {
            get; set;
        }
    }
}
