using gip.core.datamodel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using dbMes = gip.mes.datamodel;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'MachineItem'}de{'MachineItem'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class MachineItem : INotifyPropertyChanged
    {
        #region DI
        public SourceSelectionRulesResult SourceSelectionRulesResult { get; private set; }
        public RuleGroup RuleGroup { get; private set; }
        #endregion

        #region ctor's

        public MachineItem(SourceSelectionRulesResult sourceSelectionRulesResult, RuleGroup ruleGroup, ACClass machine, dbMes.Material material, string preConfigACUrl)
        {
            SourceSelectionRulesResult = sourceSelectionRulesResult;
            RuleGroup = ruleGroup;
            Machine = machine;
            Material = material;
            PreConfigACUrl = preConfigACUrl;
        }

        #endregion

        #region Properties
        public string PreConfigACUrl { get; set; }

        public List<ACClassWF> PWNodes { get; private set; } = new List<ACClassWF>();

        #endregion

        #region ACProperties

        [ACPropertyInfo(100, "", Const.ProcessModule)]
        public ACClass Machine { get; set; }

        [ACPropertyInfo(101, "", dbMes.ConstApp.Material)]
        public dbMes.Material Material { get; set; }


        public bool _IsSelected;

        [ACPropertyInfo(102, "", gip.mes.datamodel.ConstApp.Select)]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    if (RuleGroup != null)
                    {
                        SourceSelectionRulesResult.UpdateMachineItems(value, this);
                    }
                }
            }
        }

        public bool AllSelectionStarted { get; set; } = false;
        public bool? _IsSelectedAll;

        [ACPropertyInfo(103, "", gip.mes.datamodel.ConstApp.Select)]
        public bool? IsSelectedAll
        {
            get
            {
                return _IsSelectedAll;
            }
            set
            {
                if (_IsSelectedAll != value)
                {
                    _IsSelectedAll = value;
                    OnPropertyChanged(nameof(IsSelectedAll));
                    if (RuleGroup == null)
                    {
                        SourceSelectionRulesResult.UpdateRuleGroupMachine(Machine.ACClassID, value);
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

        #region Metods
        public string GetConfigACUrl(ACClassWF aCClassWF)
        {
            return aCClassWF?.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Excluded_process_modules.ToString();
        }

        public void AddPWNode(ACClassWF pwNode)
        {
            if (!PWNodes.Select(c => c.ACClassWFID).Contains(pwNode.ACClassWFID))
            {
                PWNodes.Add(pwNode);
            }
        }

        public override string ToString()
        {
            return $"[{(IsSelected ? "X" : "")}] {Machine?.ACCaption}";
        }

        #endregion
    }
}
