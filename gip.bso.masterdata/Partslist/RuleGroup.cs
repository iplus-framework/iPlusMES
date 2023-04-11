using gip.core.datamodel;
using System.Collections.Generic;
using System.ComponentModel;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'RuleGroup'}de{'RuleGroup'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class RuleGroup : INotifyPropertyChanged
    {

        #region Properties

        [ACPropertyInfo(100, "", "en{'Application Class'}de{'Anwendungsklasse'}")]
        public ACClass RefPAACClass { get; set; }


        [ACPropertyInfo(101, "", "en{'RuleSelections'}de{'RuleSelections'}")]
        public List<RuleSelection> RuleSelections { get; set; } = new List<RuleSelection>();

        private RuleSelection _CurrentRuleSelection;

        [ACPropertyInfo(102, "", "en{'CurrentRuleSelection'}de{'CurrentRuleSelection'}")]
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
                    if (_CurrentRuleSelection != null)
                    {
                        _CurrentRuleSelection.OnPropertyChanged(nameof(RuleSelection.Items));
                    }
                }
            }
        }

        #endregion

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

        #region Methods

        /// <summary>
        /// is same module is selected on other view - sync booth values
        /// </summary>
        /// <param name="ruleSelection"></param>
        /// <param name="aCClass"></param>
        /// <param name="isSelected"></param>
        public void PropagateSelection(RuleSelection ruleSelection, ACClass aCClass, bool isSelected)
        {
            foreach (RuleSelection currRuleSelection in RuleSelections)
            {
                if (currRuleSelection.RuleSelectionID != ruleSelection.RuleSelectionID)
                {
                    foreach (RuleItem ruleItem in currRuleSelection.Items)
                    {
                        if (ruleItem.Machine.ACClassID == aCClass.ACClassID)
                        {
                            ruleItem.IsSelected = isSelected;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return RefPAACClass?.ACCaption;
        }

        #endregion
    }
}
