using gip.core.datamodel;
using System;
using System.ComponentModel;
using TandTv3 = gip.mes.facility.TandTv3;

namespace gip.mes.facility
{
    [Serializable]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTv2DisplayItemType'}de{'TandTv2DisplayItemType'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    public class TrackingDisplayItemType : INotifyPropertyChanged
    {
        [ACPropertyInfo(9999, "Title", "en{'Title'}de{'Title'}")]
        public string Title { get; set; }

        [ACPropertyInfo(9999, "SubTitle", "en{'SubTitle'}de{'SubTitle'}")]
        public string SubTitle { get; set; }

        private bool _IsIncluded;
        [ACPropertyInfo(9999, "IsIncluded", "en{'IsIncluded'}de{'IsIncluded'}")]
        public bool IsIncluded
        {
            get
            {
                return _IsIncluded;
            }
            set
            {
                if (_IsIncluded != value)
                {
                    _IsIncluded = value;
                    OnPropertyChanged("IsIncluded");
                }
            }
        }

        public TandTv3.Model.DisplayGroupEnum ItemType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        [ACMethodInfo("TandTv2DisplayItemType", "en{'PropertyChanged'}de{'PropertyChanged'}", 9999)]
        public virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
