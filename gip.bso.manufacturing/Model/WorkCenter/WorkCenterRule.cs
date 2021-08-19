using System.Runtime.Serialization;

namespace gip.bso.manufacturing
{
    [DataContract]
    public class WorkCenterRule
    {
        [DataMember]
        public string VBUserName
        {
            get;
            set;
        }

        [DataMember]
        public string ProcessModuleACUrl
        {
            get;
            set;
        }
    }
}
