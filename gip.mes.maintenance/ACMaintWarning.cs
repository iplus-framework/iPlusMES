using gip.core.datamodel;
using gip.core.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace gip.mes.maintenance
{
    [DataContract]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance warning'}de{'Maintenance warning'}", Global.ACKinds.TACSimpleClass)]
    [ACSerializeableInfo(new Type[] { typeof(ACMaintWarning), typeof(List<ACMaintWarning>), typeof(List<ACMaintDetailsWarning>) })]
    public class ACMaintWarning
    {
        [DataMember]
        [ACPropertyInfo(999)]
        public string InstanceName
        {
            get;
            set;
        }

        [DataMember]
        [ACPropertyInfo(999)]
        public List<ACMaintDetailsWarning> DetailsList
        {
            get;
            set;
        }

        [ACPropertyInfo(999)]
        [IgnoreDataMember]
        public string Text
        {
            get
            {
                string text = "";
                int itemsCount = DetailsList.Count;
                foreach (ACMaintDetailsWarning warningDetail in DetailsList)
                {
                    text += warningDetail.Text;
                    if (DetailsList.IndexOf(warningDetail) + 1 < itemsCount)
                        text += System.Environment.NewLine;
                }
                return text;
            }
        }
    }

    [DataContract]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance warning details'}de{'Maintenance warning details'}", Global.ACKinds.TACSimpleClass)]
    [ACSerializeableInfo]
    public class ACMaintDetailsWarning
    {
        [DataMember]
        [ACPropertyInfo(999)]
        public string ACIdentifier
        {
            get;
            set;
        }

        [DataMember]
        [ACPropertyInfo(999)]
        public string ACCaptionTranslation
        {
            get;
            set;
        }

        [DataMember]
        [ACPropertyInfo(999)]
        public string ActualValue
        {
            get;
            set;
        }

        [DataMember]
        [ACPropertyInfo(999)]
        public string MaxValue
        {
            get;
            set;
        }

        [ACPropertyInfo(999)]
        [IgnoreDataMember]
        public string Text
        {
            get
            {
                if (!string.IsNullOrEmpty(MaxValue))
                    return Translator.GetTranslation(ACCaptionTranslation) + " (" + ActualValue + ")" + MaxValue;
                return Translator.GetTranslation(ACCaptionTranslation) + ActualValue;
            }
        }
    }
}
