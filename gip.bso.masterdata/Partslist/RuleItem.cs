using gip.core.datamodel;
using System.ComponentModel;
using System.Reflection;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'RuleItem'}de{'RuleItem'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class RuleItem : INotifyPropertyChanged
    {
        public RuleSelection RuleSelection { get; private set; }

        public RuleItem(RuleSelection ruleSelection)
        {
            RuleSelection = ruleSelection;
        }


        [ACPropertyInfo(100, "", Const.ProcessModule)]
        public ACClass Machine { get; set; }

        private bool _IsSelected;

        [ACPropertyInfo(101, "", gip.mes.datamodel.ConstApp.Select)]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                OnPropertyChanged(nameof(IsSelected));
                if (RuleSelection.PropagateRuleSelection)
                {
                    RuleSelection.RuleGroup.PropagateSelection(RuleSelection, Machine, _IsSelected);
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

        #region Metods

        public override string ToString()
        {
            return $"[{(IsSelected ? "X" : "")}] {Machine?.ACCaption}";
        }

        #endregion
    }
}
