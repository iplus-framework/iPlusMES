using dbMes = gip.mes.datamodel;
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
        public List<dbMes.Material> NotDosableMaterials { get; set; } = new List<dbMes.Material>();

        #endregion

        #region Methods

        public void AddNotDosableMaterial(dbMes.Material material)
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
                machineItem.AddPWNode(pwNode);
                machineItem.IsSelected = machineItem.IsSelected || !excludedProcessModules.Contains(machine);
            }
        }

        public void MachineItemSelectionChanged(MachineItem machineItem)
        {
            foreach (RuleGroup ruleGroup in RuleGroups)
            {
                foreach (RuleSelection ruleSelectionItem in ruleGroup.RuleSelectionList)
                {
                    if (ruleSelectionItem.MachineItem.Machine.ACClassID == machineItem.Machine.ACClassID)
                    {
                        ruleSelectionItem.MachineItem._IsSelected = machineItem.IsSelected;
                        ruleSelectionItem.MachineItem.OnPropertyChanged(nameof(MachineItem.IsSelected));
                    }
                }
            }
        }

        #endregion
    }
}
