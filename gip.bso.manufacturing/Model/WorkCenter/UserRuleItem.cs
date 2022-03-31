using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [DataContract]
    public class UserRuleItem : EntityBase
    {
        [DataMember]
        public Guid VBUserID
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public string VBUserName
        {
            get;
            set;
        }

        [DataMember]
        public Guid RuleParamID
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public string RuleParamCaption
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("{0} => {1}", VBUserName, RuleParamCaption);
        }
    }
}
