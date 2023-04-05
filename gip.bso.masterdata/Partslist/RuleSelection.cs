using gip.core.datamodel;
using System.Collections.Generic;
using gip.mes.facility;
using System.ComponentModel;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'RuleSelection'}de{'RuleSelection'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class RuleSelection : INotifyPropertyChanged
    {

        [ACPropertyInfo(100, "", Const.Workflow)]
        public MapPosToWFConn WF { get; set; }

        private List<ACClass> _AvailableValues;
        [ACPropertyInfo(101, "", Const.ProcessModule)]
        public List<ACClass> AvailableValues
        {
            get
            {
                return _AvailableValues;
            }

            set
            {
                _AvailableValues = value;
                OnPropertyChanged(nameof(AvailableValues));
            }
        }

        private List<ACClass> _SelectedValues;
        [ACPropertyInfo(102, "", Const.ProcessModule)]
        public List<ACClass> SelectedValues
        {
            get
            {
                return _SelectedValues;
            }

            set
            {
                _SelectedValues = value;
                OnPropertyChanged(nameof(SelectedValues));
            }
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
