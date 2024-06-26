using VD = gip.mes.datamodel;
using gip.core.datamodel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace gip.bso.masterdata
{
    public class SourceSelectionRulesResult
    {
        #region Properties

        public List<RuleGroup> RuleGroups { get; set; } = new List<RuleGroup>();
        public List<MachineItem> MachineItems { get; set; } = new List<MachineItem>();
        public List<VD.Material> DosableMaterials { get; set; } = new List<VD.Material>();
        public List<VD.Material> NotDosableMaterials { get; set; } = new List<VD.Material>();

        public IACConfigStore CurrentConfigStore { get; set; }

        #endregion

        #region Methods

        public void AddDosableMaterial(VD.Material material)
        {
            if (!DosableMaterials.Select(c => c.MaterialNo).Contains(material.MaterialNo))
            {
                DosableMaterials.Add(material);
            }
        }


        public void AddNotDosableMaterial(VD.Material material)
        {
            if (!NotDosableMaterials.Select(c => c.MaterialNo).Contains(material.MaterialNo))
            {
                NotDosableMaterials.Add(material);
            }
        }

        public void FillMachineItems(List<ACClass> allProcessModules, List<ACClass> excludedProcessModules, string preConfigACUrl, ACClassWF pwNode)
        {
            foreach (ACClass machine in allProcessModules)
            {
                MachineItem machineItem = MachineItems.Where(c => c.RuleGroup == null && c.Machine.ACClassID == machine.ACClassID).FirstOrDefault();

                if (machineItem == null)
                {
                    machineItem = new MachineItem(this, null, machine, null, preConfigACUrl);
                    MachineItems.Add(machineItem);
                }

                if (machineItem.RuleGroup == null)
                {
                    machineItem.AddPWNode(pwNode);
                    if (machineItem.RuleGroup != null)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                    // IsSelected == true - if 
                    bool isIncluded = !excludedProcessModules.Select(c => c.ACClassID).Contains(machine.ACClassID);
                    if (!machineItem.AllSelectionStarted)
                    {
                        machineItem.AllSelectionStarted = true;
                        machineItem._IsSelectedAll = isIncluded;
                    }
                    else if (machineItem._IsSelectedAll != null)
                    {
                        if (machineItem._IsSelectedAll != isIncluded)
                        {
                            machineItem._IsSelectedAll = null;
                        }
                    }
                }
            }
        }

        public void UpdateRuleGroupMachine(Guid acclassID, bool? isSelected)
        {
            foreach (RuleGroup ruleGroup in RuleGroups)
            {
                foreach (RuleSelection ruleSelectionItem in ruleGroup.RuleSelectionList)
                {
                    if (ruleSelectionItem.MachineItem.Machine.ACClassID == acclassID && isSelected != null)
                    {
                        ruleSelectionItem.MachineItem._IsSelected = isSelected.Value;
                        ruleSelectionItem.MachineItem.OnPropertyChanged(nameof(MachineItem.IsSelected));
                    }
                }
            }
        }

        public void UpdateMachineItems(bool isSelected, MachineItem refMachineItem)
        {
            foreach (MachineItem machineItem in MachineItems)
            {
                if (
                        machineItem != refMachineItem
                        && machineItem._IsSelectedAll != null
                        && machineItem.Machine.ACClassID == refMachineItem.Machine.ACClassID
                    )
                {
                    if (machineItem._IsSelectedAll != isSelected)
                    {
                        Guid[] wf1 = machineItem.PWNodes.Select(c => c.ACClassWFID).ToArray();
                        Guid[] wf2 = refMachineItem.PWNodes.Select(c => c.ACClassWFID).ToArray();
                        bool areEqual = wf1.OrderBy(g => g).SequenceEqual(wf2.OrderBy(g => g));
                        if (areEqual)
                        {
                            machineItem._IsSelectedAll = isSelected;
                        }
                        else
                        {
                            machineItem._IsSelectedAll = null;
                        }

                    }
                    machineItem.OnPropertyChanged(nameof(MachineItem.IsSelectedAll));
                }
            }
        }

        #endregion
    }
}
