using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Single dosing item'}de{'Single dosing item'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class SingleDosingItem : IACObject, INotifyPropertyChanged
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

        [ACPropertyInfo(103)]
        [IgnoreDataMember]
        public ACClassDesign MaterialIconDesign
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public IACObject ParentACObject => null;

        [IgnoreDataMember]
        public IACType ACType => this.ReflectACType();

        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        [IgnoreDataMember]
        public string ACIdentifier => this.ReflectGetACIdentifier();

        [IgnoreDataMember]
        public string ACCaption => this.ACIdentifier;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ReflectGetACUrl(rootACObject);
        }

        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
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

        public SingleDosingItems(IEnumerable<SingleDosingItem> collection) : base (collection)
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
