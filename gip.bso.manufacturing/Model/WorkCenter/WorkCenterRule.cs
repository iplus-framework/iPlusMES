using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'WorkCenterRule'}de{'WorkCenterRule'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    public class WorkCenterRule : EntityBase
    {
        private string _VBUserName;
        [DataMember]
        [ACPropertyInfo(100, nameof(VBUserName), "en{'Name'}de{'Name'}")]
        public string VBUserName
        {
            get
            {
                return _VBUserName;
            }
            set
            {
                if(_VBUserName != value)
                {
                    _VBUserName =value;
                    OnPropertyChanged(nameof(VBUserName));
                }
            }
        }


        private string _ProcessModuleACUrl;
        [DataMember]
        [ACPropertyInfo(101, nameof(ProcessModuleACUrl), "en{'Param'}de{'Param'}")]
        public string ProcessModuleACUrl
        {
            get
            {
                return _ProcessModuleACUrl;
            }
            set
            {
                if (_ProcessModuleACUrl != value)
                {
                    _ProcessModuleACUrl = value;
                    OnPropertyChanged(nameof(ProcessModuleACUrl));
                }
            }
        }
    }
}
