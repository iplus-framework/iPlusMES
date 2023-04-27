using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using dbMes = gip.mes.datamodel;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'RuleGroup'}de{'RuleGroup'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class RuleGroup : INotifyPropertyChanged
    {

        #region Properties

        [ACPropertyInfo(100, "", "en{'Application Class'}de{'Anwendungsklasse'}")]
        public ACClass RefPAACClass { get; set; }


        #region RuleSelection
        private RuleSelection _SelectedRuleSelection;
        /// <summary>
        /// Selected property for RuleSelection
        /// </summary>
        /// <value>The selected RuleSelection</value>
        [ACPropertySelected(9999, "RuleSelection", "en{'TODO: RuleSelection'}de{'TODO: RuleSelection'}")]
        public RuleSelection SelectedRuleSelection
        {
            get
            {
                return _SelectedRuleSelection;
            }
            set
            {
                if (_SelectedRuleSelection != value)
                {
                    _SelectedRuleSelection = value;
                    OnPropertyChanged(nameof(SelectedRuleSelection));
                }
            }
        }


        private List<RuleSelection> _RuleSelectionList;
        /// <summary>
        /// List property for RuleSelection
        /// </summary>
        /// <value>The RuleSelection list</value>
        [ACPropertyList(9999, "RuleSelection")]
        public List<RuleSelection> RuleSelectionList
        {
            get
            {
                if (_RuleSelectionList == null)
                {
                    _RuleSelectionList = new List<RuleSelection>();
                }
                return _RuleSelectionList;
            }
            set
            {
                _RuleSelectionList = value;
                OnPropertyChanged(nameof(RuleSelectionList));
            }
        }

        #endregion


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

        public RuleSelection AddRuleSelection(ACClassWF pwNode, dbMes.Material material, ACClass source, ACClass target, string preConfigACUrl)
        {
            RuleSelection ruleSelection =
                RuleSelectionList
                .Where(c => 
                            c.Source == source
                            && c.Target == target
                            && (material == null || (c.Material !=null && c.Material.MaterialNo == material.MaterialNo))
                ).FirstOrDefault();

            if (ruleSelection == null)
            {
                ruleSelection = new RuleSelection();
                ruleSelection.RuleSelectionID = Guid.NewGuid();
                ruleSelection.RuleGroup = this;
                ruleSelection.Material = material;
                ruleSelection.Source = source;
                ruleSelection.Target = target;
                ruleSelection.PreConfigACUrl = preConfigACUrl;
                RuleSelectionList.Add(ruleSelection);
            }

            if (!ruleSelection.PWNodes.Select(c => c.ACClassWFID).Contains(pwNode.ACClassWFID))
            {
                ruleSelection.PWNodes.Add(pwNode);
            }

            return ruleSelection;
        }

        /// <summary>
        /// is same module is selected on other view - sync booth values
        /// </summary>
        /// <param name="ruleSelection"></param>
        /// <param name="aCClass"></param>
        /// <param name="isSelected"></param>
        public void PropagateSelection(RuleSelection ruleSelection, ACClass aCClass, bool isSelected)
        {
            //foreach (RuleSelection currRuleSelection in RuleSelections)
            //{
            //    if (currRuleSelection.RuleSelectionID != ruleSelection.RuleSelectionID)
            //    {
            //        if(currRuleSelection.Items != null)
            //        {
            //            foreach (RuleItem ruleItem in currRuleSelection.Items)
            //            {
            //                if (ruleItem.Machine.ACClassID == aCClass.ACClassID)
            //                {
            //                    ruleItem.IsSelected = isSelected;
            //                }
            //            }
            //        }
            //    }
            //}
        }

        public override string ToString()
        {
            return RefPAACClass?.ACCaption;
        }

        #endregion
    }
}
