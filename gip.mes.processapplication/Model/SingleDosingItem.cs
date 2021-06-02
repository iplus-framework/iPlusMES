using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Single dosing item'}de{'Single dosing item'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class SingleDosingItem
    {
        public SingleDosingItem()
        {

        }

        [ACPropertyInfo(100)]
        [DataMember(Name = "A")]
        public string MaterialNo
        {
            get;
            set;
        }

        [ACPropertyInfo(101)]
        [DataMember(Name = "B")]
        public string MaterialName
        {
            get;
            set;
        }

        [ACPropertyInfo(102)]
        [DataMember(Name = "C")]
        public string FacilityNo
        {
            get;
            set;
        }
    }

    [CollectionDataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Single dosing items'}de{'Single dosing items'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class SingleDosingItems : List<SingleDosingItem>
    {
        public SingleDosingItems()
        {

        }

        [DataMember]
        public Msg Error
        {
            get;
            set;
        }
    }
}
