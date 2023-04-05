using gip.core.datamodel;
using System.Collections.Generic;
using System.ComponentModel;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'RuleGroup'}de{'RuleGroup'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class RuleGroup : INotifyPropertyChanged
    {

        [ACPropertyInfo(100, "", Const.ACGroup)]
        public ACClass RefPAACClass { get; set; }


        [ACPropertyInfo(101, "", Const.ACGroup)]
        public List<RuleSelection> RuleSelections { get; set; } = new List<RuleSelection>();

        private RuleSelection _CurrentRuleSelection;

        [ACPropertyInfo(102, "", Const.ACGroup)]
        public RuleSelection CurrentRuleSelection

        {
            get
            {
                return _CurrentRuleSelection;
            }
            set
            {
                if (_CurrentRuleSelection != value)
                {

                    _CurrentRuleSelection = value;
                    OnPropertyChanged(nameof(CurrentRuleSelection));
                    if(_CurrentRuleSelection != null)
                    {
                        _CurrentRuleSelection.OnPropertyChanged(nameof(RuleSelection.AvailableValues));
                        _CurrentRuleSelection.OnPropertyChanged(nameof(RuleSelection.SelectedValues));
                    }
                }
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
