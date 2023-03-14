using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using dbMes = gip.mes.datamodel;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'BSOSourceSelectionRules'}de{'BSOSourceSelectionRules'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOSourceSelectionRules : ACBSOvb
    {

        #region ctor's

        public BSOSourceSelectionRules(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
           base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }


        #endregion


        #region Method

        [ACMethodInfo("Dialog", "en{'Select Sources'}de{'Quellen auswählen'}", (short)MISort.QueryPreviewDlg)]
        public void ShowDialogSelectSources(Guid acClassWFID, Guid partslistID)
        {

            using (Database database = new Database())
            using (dbMes.DatabaseApp databaseApp = new dbMes.DatabaseApp())
            {
                Console.WriteLine(acClassWFID.ToString());
                List<RuleGroup> list = LoadRuleGroupList(database, databaseApp, acClassWFID, partslistID);
            }


            //ShowDialog(this, "DlgSelectSources");
            //this.ParentACComponent.StopComponent(this);
        }

        #endregion

        #region RuleGroup

        private RuleGroup _SelectedRuleGroup;
        /// <summary>
        /// Selected property for EntityType
        /// </summary>
        /// <value>The selected RuleGroup</value>
        [ACPropertySelected(9999, "RuleGroup", "en{'TODO: RuleGroup'}de{'TODO: RuleGroup'}")]
        public RuleGroup SelectedRuleGroup
        {
            get
            {
                return _SelectedRuleGroup;
            }
            set
            {
                if (_SelectedRuleGroup != value)
                {
                    _SelectedRuleGroup = value;
                    OnPropertyChanged(nameof(SelectedRuleGroup));
                }
            }
        }


        private List<RuleGroup> _RuleGroupList;
        /// <summary>
        /// List property for EntityType
        /// </summary>
        /// <value>The RuleGroup list</value>
        [ACPropertyList(9999, "RuleGroup")]
        public List<RuleGroup> RuleGroupList
        {
            get
            {
                return _RuleGroupList;
            }
        }

        private List<RuleGroup> LoadRuleGroupList(Database database, dbMes.DatabaseApp databaseApp, Guid acClassWFID, Guid partslistID)
        {
            List<ACClassWF> dischargingItems = GetAllDischargingItems(database, databaseApp, acClassWFID, partslistID);
            List<RuleGroup> grouped = GroupDischargingItems(database, dischargingItems);
            return grouped;
        }

        #endregion

        #region Others

        public List<ACClassWF> GetAllDischargingItems(Database database, dbMes.DatabaseApp databaseApp, Guid acClassWFID, Guid partslistID)
        {
            Type typeReceiveMat = typeof(IPWNodeReceiveMaterialRouteable);
            ACClassWF wf = database.ACClassWF.Where(c => c.ACClassWFID == acClassWFID).FirstOrDefault();
            ACClassMethod refMth = wf.RefPAACClassMethod;

            List<ACClassWF> dischargingItems =
                refMth
                .ACClassWF_ACClassMethod
                .AsEnumerable()
                .Where(c => typeReceiveMat.IsAssignableFrom(c.PWACClass.ObjectType))
                .ToList();

            dbMes.Partslist pll = databaseApp.Partslist.FirstOrDefault(c => c.PartslistID == partslistID);
            Guid[] connectedACClassWFIDs =
                pll
                .MaterialWF
                .MaterialWFACClassMethod_MaterialWF
                .SelectMany(c => c.MaterialWFConnection_MaterialWFACClassMethod)
                .Select(c => c.ACClassWFID)
                .Distinct()
                .ToArray();

            List<ACClassWF> filteredDischargingItems = dischargingItems.Where(c => connectedACClassWFIDs.Contains(c.ACClassWFID)).ToList();

            return filteredDischargingItems;
        }

        public List<RuleGroup> GroupDischargingItems(Database database, List<ACClassWF> dischargingItems)
        {
            List<RuleGroup> ruleGroups = new List<RuleGroup>();
            foreach (ACClassWF aCClassWF in dischargingItems)
            {
                RuleSelection ruleSelection = new RuleSelection();
                ruleSelection.WF = aCClassWF;
                RuleGroup ruleGroup = GetRuleGroup(database, ruleGroups, aCClassWF);
                ruleGroup.RuleSelections.Add(ruleSelection);
                ruleSelection.ProcessModules = GetProcessModules(database, aCClassWF);
                PreselectModules(database, ruleSelection);
            }
            return ruleGroups;
        }

        private void PreselectModules(Database database, RuleSelection ruleSelection)
        {
            
        }

        private List<ACClass> GetProcessModules(Database database, ACClassWF aCClassWF)
        {
            return new List<ACClass>();
        }

        private RuleGroup GetRuleGroup(Database database, List<RuleGroup> ruleGroups, ACClassWF aCClassWF)
        {
            RuleGroup ruleGroup = ruleGroups.FirstOrDefault(c => c.RefPAACClass.ACClassID == aCClassWF.RefPAACClassID);
            if (ruleGroup == null)
            {
                ruleGroup = new RuleGroup()
                {
                    RefPAACClass = aCClassWF.RefPAACClass
                };
                ruleGroups.Add(ruleGroup);
            }
            return ruleGroup;
        }

        #endregion
    }
}
