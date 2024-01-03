using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    /// <summary>
    /// UserRuleItem
    /// </summary>
    [DataContract]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'UserRuleItem'}de{'UserRuleItem'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    public class UserRuleItem : EntityBase
    {
        [DataMember]
        public Guid VBUserID
        {
            get;
            set;
        }

        private string _VBUserName;
        //[IgnoreDataMember]
        [ACPropertyInfo(100, "VBUserName", "en{'Name'}de{'Name'}")]
        public string VBUserName
        {
            get
            {
                return _VBUserName;
            }
            set
            {
                if (_VBUserName != value)

                {
                    _VBUserName = value;
                    OnPropertyChanged(nameof(VBUserName));
                }
            }
        }

        [DataMember]
        public Guid RuleParamID
        {
            get;
            set;
        }

        private string _RuleParamCaption;
        //[IgnoreDataMember]
        [ACPropertyInfo(101, "RuleParamCaption", "en{'Param'}de{'Param'}")]
        public string RuleParamCaption
        {
            get
            {
                return _RuleParamCaption;
            }
            set
            {
                if (_RuleParamCaption != value)
                {
                    _RuleParamCaption = value;
                    OnPropertyChanged(nameof(RuleParamCaption));
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} => {1}", VBUserName, RuleParamCaption);
        }
    }
}
