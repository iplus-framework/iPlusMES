// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.reporthandler;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Businessobject or App for managing the facility master data (storage locations, storage bins, vehicles and their sub-structures) as a hierarchy tree.
    /// This class extends the <see cref="BSOFacilityExplorer"/> which provides the facility tree and the search.
    /// To search for records, enter the search string in the FilterFacility property; the tree is filtered live by FacilityNo, FacilityName and KeyOfExtSys and the first match is preselected.
    /// Selecting a node sets the CurrentFacility property (tree node) and the SelectedFacility property (the Facility entity of the selected node).
    /// SelectedFacility is used to display and edit the currently selected record.
    /// Property changes should always be made on SelectedFacility and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to another node or creating a new record.
    /// The AddFacility() method creates a new Facility as a child of the CurrentFacility node, assigns a generated number (NewFacility{n}) and the default facility type and inserts it into the tree. Fill in all required fields before saving.
    /// The DeleteFacility() method deletes the Facility of the CurrentFacility node; it is only enabled when the node has no child facilities. Always call the Save() method after calling DeleteFacility() to execute the delete operation in the database.
    /// The UndoSave() method discards unsaved changes and reloads the current Facility.
    /// The Load method updates the SelectedFacility object with fresh database data if another user has made changes in the background.
    /// The FacilityMaterialList property contains the materials assigned to the SelectedFacility. With AddFacilityMaterial() a new FacilityMaterial is created for the SelectedFacility and selected; with DeleteFacilityMaterial() the SelectedFacilityMaterial is removed. Call Save() afterwards.
    /// The OEE test data and throughput values of the SelectedFacilityMaterial can be managed with GenerateTestOEEData(), DeleteTestOEEData(), RecalcThroughputAverage() and RecalcThroughputAndOEE().
    /// A ProcessWorkflow can be assigned to the SelectedFacility: call SetProcessWorkflow() to open the workflow selection dialog, select a workflow in the SelectedProcessWorkflow property and confirm with SetWFOk(). RemoveProcessWorkflow() removes the assigned workflow after a confirmation question.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacility&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Storage Location'}de{'Lagerplatz'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Facility.ClassName,
        Description = @"Businessobject or App for managing the facility master data (storage locations, storage bins, vehicles and their sub-structures) as a hierarchy tree.
        This class extends the BSOFacilityExplorer which provides the facility tree and the search.
        To search for records, enter the search string in the FilterFacility property; the tree is filtered live by FacilityNo, FacilityName and KeyOfExtSys and the first match is preselected.
        Selecting a node sets the CurrentFacility property (tree node) and the SelectedFacility property (the Facility entity of the selected node).
        SelectedFacility is used to display and edit the currently selected record.
        Property changes should always be made on SelectedFacility and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to another node or creating a new record.
        The AddFacility() method creates a new Facility as a child of the CurrentFacility node, assigns a generated number (NewFacility{n}) and the default facility type and inserts it into the tree. Fill in all required fields before saving.
        The DeleteFacility() method deletes the Facility of the CurrentFacility node; it is only enabled when the node has no child facilities. Always call the Save() method after calling DeleteFacility() to execute the delete operation in the database.
        The UndoSave() method discards unsaved changes and reloads the current Facility.
        The Load method updates the SelectedFacility object with fresh database data if another user has made changes in the background.
        The FacilityMaterialList property contains the materials assigned to the SelectedFacility. With AddFacilityMaterial() a new FacilityMaterial is created for the SelectedFacility and selected; with DeleteFacilityMaterial() the SelectedFacilityMaterial is removed. Call Save() afterwards.
        The OEE test data and throughput values of the SelectedFacilityMaterial can be managed with GenerateTestOEEData(), DeleteTestOEEData(), RecalcThroughputAverage() and RecalcThroughputAndOEE().
        A ProcessWorkflow can be assigned to the SelectedFacility: call SetProcessWorkflow() to open the workflow selection dialog, select a workflow in the SelectedProcessWorkflow property and confirm with SetWFOk(). RemoveProcessWorkflow() removes the assigned workflow after a confirmation question.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacility&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOFacility : BSOFacilityExplorer
    {

        #region consts
        public const string ClassName = "BSOFacility";
        public const string ParameterName_ParentFacilityID = "ParentFacilityID";
        #endregion

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityLocation"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacility(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }


        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _FacilityOEEManager = ACFacilityOEEManager.ACRefToServiceInstance(this);
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            if (_FacilityOEEManager != null)
                ACFacilityOEEManager.DetachACRefFromServiceInstance(this, _FacilityOEEManager);
            _FacilityOEEManager = null;
            return await base.ACDeInit(deleteACClassTask);
        }

        public override object Clone()
        {
            object clonedObject = base.Clone();
            BSOFacility clonedBSOFacility = clonedObject as BSOFacility;
            if (CurrentFacility != null && CurrentFacility.ACObject != null)
            {
                Facility facility = CurrentFacility.ACObject as Facility;
                clonedBSOFacility.CurrentFacility = new ACFSItem(null, CurrentFacility.Container, facility, FacilityTree.FacilityACCaption(facility), ResourceTypeEnum.IACObject);
            }
            return clonedBSOFacility;
        }

        #endregion

        #region Properties

        #region Managers
        protected ACRef<ACFacilityOEEManager> _FacilityOEEManager = null;
        public ACFacilityOEEManager FacilityOEEManager
        {
            get
            {
                if (_FacilityOEEManager == null)
                    return null;
                return _FacilityOEEManager.ValueT;
            }
        }
        #endregion

        #region FacilityMaterial
        private FacilityMaterial _SelectedFacilityMaterial;
        /// <summary>
        /// Selected property for FacilityMaterial
        /// </summary>
        /// <value>The selected FacilityMaterial</value>
        [ACPropertySelected(9999, "FacilityMaterial", "en{'Facility Material'}de{'Lagerplatz Material'}", Description = "Gets or sets the FacilityMaterial currently selected in the FacilityMaterialList. Changes to the material assignment of the selected facility are made on this record, followed by Save().")]
        public FacilityMaterial SelectedFacilityMaterial
        {
            get
            {
                return _SelectedFacilityMaterial;
            }
            set
            {
                if (_SelectedFacilityMaterial != value)
                {
                    _SelectedFacilityMaterial = value;
                    OnPropertyChanged(nameof(SelectedFacilityMaterial));
                }
            }
        }


        private List<FacilityMaterial> _FacilityMaterialList;
        /// <summary>
        /// List property for FacilityMaterial
        /// </summary>
        /// <value>The FacilityMaterial list</value>
        [ACPropertyList(9999, "FacilityMaterial", Description = "Gets the list of FacilityMaterial records assigned to the SelectedFacility (the materials that may be stored or produced at this facility). The list is refreshed when the selected facility changes.")]
        public List<FacilityMaterial> FacilityMaterialList
        {
            get
            {
                if (_FacilityMaterialList == null)
                    _FacilityMaterialList = LoadFacilityMaterialList();
                return _FacilityMaterialList;
            }
            set
            {
                _FacilityMaterialList = value;
                OnPropertyChanged(nameof(FacilityMaterialList));
            }
        }

        private List<FacilityMaterial> LoadFacilityMaterialList()
        {
            if (SelectedFacility == null)
                return new List<FacilityMaterial>();
            return
                SelectedFacility
                .FacilityMaterial_Facility
                .OrderBy(c => c.Material.MaterialNo)
                .ToList();
        }
        #endregion

        #region  Assigned Workflow

        private List<gip.core.datamodel.ACClassMethod> _ProcessWorkflowList;
        [ACPropertyList(9999, "ProcessWorkflow", Description = "Gets the list of all process workflows (MSWorkflow methods) that can be assigned to a facility in the workflow assignment dialog.")]
        public List<gip.core.datamodel.ACClassMethod> ProcessWorkflowList
        {
            get
            {
                if (_ProcessWorkflowList == null)
                {
                    _ProcessWorkflowList =
                        DatabaseApp
                        .ContextIPlus
                        .ACClassMethod
                        .Where(c =>
                                    c.ACKindIndex == (short)Global.ACKinds.MSWorkflow
                                    &&
                                    !c.IsSubMethod
                               )
                        .OrderBy(c => c.ACIdentifier)
                        .ToList();
                }
                return _ProcessWorkflowList;
            }
        }

        private gip.core.datamodel.ACClassMethod _SelectedProcessWorkflow;

        [ACPropertySelected(9999, "ProcessWorkflow", "en{'ProcessWorkflow'}de{'ProcessWorkflow'}", Description = "Gets or sets the process workflow selected in the workflow assignment dialog. Confirming the dialog with SetWFOk() assigns it to the SelectedFacility.")]
        public gip.core.datamodel.ACClassMethod SelectedProcessWorkflow
        {
            get
            {
                return _SelectedProcessWorkflow;
            }
            set
            {
                _SelectedProcessWorkflow = value;
                OnPropertyChanged();
            }
        }
        #endregion

        [ACPropertyInfo(9999, Description = "Gets the CurrentFacility and all its child facilities as a flat list, e.g. as the record source for reports.")]
        public IEnumerable<Facility> ReportFacilityList
        {
            get
            {
                if (CurrentFacility == null)
                    return null;

                List<Facility> resultList = new List<Facility>();

                GetFacilityTreeAsList(resultList, CurrentFacility);

                return resultList;
            }
        }
            


        #endregion

        #region Methods

        #region BSO

        public override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(name);
            if (name == nameof(SelectedFacility))
            {
                FacilityMaterialList = LoadFacilityMaterialList();
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves all changes made on the SelectedFacility and its FacilityMaterial assignments to the database.")]
        public void Save()
        {
            OnSave();
            DatabaseApp.OnPropertyChanged(Facility.ClassName);
        }

        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction(nameof(Facility), "en{'Load'}de{'Laden'}", (short)MISort.Load, false, nameof(SelectedFacility), Global.ACKinds.MSMethodPrePost, Description = "Reloads the SelectedFacility from the database to pick up changes made by other users in the background.")]
        public void Load(bool requery = false)
        {
            LoadEntity<Facility>(requery, () => SelectedFacility, () => SelectedFacility, c => SelectedFacility = c,
                        DatabaseApp.Facility
                         .Include(c => c.Material)
                         .Include(c => c.MDFacilityType)

                        .Where(c => c.FacilityID == SelectedFacility.FacilityID));
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedFacility != null;
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand(Material.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards unsaved changes on the SelectedFacility and reloads it.")]
        public void UndoSave()
        {
            OnUndoSave();
            Load();
        }

        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }


        #region Mehtods ->  BSO -> ACMethod -> Tree operation

        [ACMethodInteraction(Facility.ClassName, Const.New, (short)MISort.New, true, "CurrentFacility", Global.ACKinds.MSMethodPrePost, Description = "Creates a new Facility as a child of the CurrentFacility tree node, generates a temporary number (NewFacility{n}) and assigns the default facility type. Fill in the required fields and call Save().")]
        public virtual void AddFacility()
        {
            if (!IsEnabledAddFacility()) return;
            ACFSItem currentFacilityACFSItem = CurrentFacility;

            Facility parentFacility = null;
            if (CurrentFacility.ACObject != null)
                parentFacility = CurrentFacility.ACObject as Facility;
            Facility facility = Facility.NewACObject(DatabaseApp, parentFacility);

            MDFacilityType mDFacilityType = DatabaseApp.MDFacilityType.FirstOrDefault(C => C.IsDefault);
            if (mDFacilityType == null)
                mDFacilityType = DatabaseApp.MDFacilityType.OrderBy(c => c.SortIndex).FirstOrDefault();
            int nr = 0;
            Action<ACFSItem> countNewItems = null;
            countNewItems = (ACFSItem item) =>
            {
                if (item.ACObject != null && (item.ACObject as Facility).FacilityNo.StartsWith("NewFacility"))
                    nr++;
                foreach (var childItem in item.Items)
                    countNewItems(childItem as ACFSItem);
            };
            countNewItems(CurrentFacilityRoot);

            facility.FacilityNo = string.Format(@"NewFacility{0}", nr + 1);
            facility.FacilityName = "-";
            facility.MDFacilityType = mDFacilityType;
            facility.MDFacilityTypeID = mDFacilityType.MDFacilityTypeID;
            DatabaseApp.Facility.Add(facility);
            if (parentFacility != null)
                parentFacility.Facility_ParentFacility.Add(facility);

            ACFSItem newFacilityACFSItem = new ACFSItem(null, CurrentFacility.Container, facility, FacilityTree.FacilityACCaption(facility), ResourceTypeEnum.IACObject);
            newFacilityACFSItem.OnACFSItemChange += FacilityTree.CFSItem_OnACFSItemChange;
            currentFacilityACFSItem.Add(newFacilityACFSItem);

            CurrentFacilityRootChangeInfo = new ChangeInfo(currentFacilityACFSItem, newFacilityACFSItem, Const.CmdAddChildData);

            OnPropertyChanged("CurrentFacility");
            OnPropertyChanged("SelectedFacility");
        }

        public virtual bool IsEnabledAddFacility()
        {
            return CurrentFacility != null;
        }

        [ACMethodInteraction(Facility.ClassName, "en{'Delete'}de{'Loschen'}", (short)MISort.Delete, true, "CurrentFacility", Global.ACKinds.MSMethodPrePost, Description = "Deletes the Facility of the CurrentFacility tree node. Only enabled when the node has no child facilities. Call Save() afterwards to execute the delete in the database.")]
        public virtual void DeleteFacility()
        {
            if (!IsEnabledDeleteFacility()) return;
            ACFSItem parent = CurrentFacility.ParentACObject as ACFSItem;
            ACFSItem current = CurrentFacility;

            parent.Remove(current);

            Facility dbFacility = current.ACObject as Facility;
            MsgWithDetails msg = dbFacility.DeleteACObject(DatabaseApp, false);

            CurrentFacility = parent;

            CurrentFacilityRootChangeInfo = new ChangeInfo(null, CurrentFacility, Const.CmdDeleteData);


            OnPropertyChanged("CurrentFacility");
            OnPropertyChanged("SelectedFacility");
        }

        public virtual bool IsEnabledDeleteFacility()
        {
            return CurrentFacility != null && CurrentFacility.ACObject != null && !CurrentFacility.Items.Any();
        }

        #endregion


        //[ACMethodInteraction("", "en{'Print labels for child facilites'}de{'Print labels for child facilites'}", 800, true)]
        //public void PrintLabelsForChildFacilities()
        //{
        //    var items = CurrentFacility.VisibleItemsT;

        //    foreach (var item in items)
        //    {

        //    }
        //}

        //public bool IsEnabledPrintLabelsForChildFacilities()
        //{
        //    return CurrentFacility != null;
        //}

        public void GetFacilityTreeAsList(List<Facility> facilityList,  ACFSItem currentFacility)
        {
            facilityList.Add(currentFacility.Value as Facility);

            foreach (ACFSItem childItem in currentFacility.Items)
            {
                GetFacilityTreeAsList(facilityList, childItem);
            }
        }

        #endregion

        #region FacilityMaterial and OEE
        [ACMethodCommand("", "en{'Add'}de{'Neu'}", 700, Description = "Creates a new FacilityMaterial for the SelectedFacility, adds it to the FacilityMaterialList and selects it. Call Save() afterwards.")]
        public void AddFacilityMaterial()
        {
            if (!IsEnabledAddFacilityMaterial())
                return;

            FacilityMaterial facilityMaterial = FacilityMaterial.NewACObject(DatabaseApp, null);
            facilityMaterial.Facility = SelectedFacility;
            SelectedFacility.FacilityMaterial_Facility.Add(facilityMaterial);
            FacilityMaterialList.Add(facilityMaterial);
            OnPropertyChanged(nameof(FacilityMaterialList));

            SelectedFacilityMaterial = facilityMaterial;
        }

        public bool IsEnabledAddFacilityMaterial()
        {
            return SelectedFacility != null;
        }


        /// <summary>
        /// Source Property: DeleteFacility
        /// </summary>
        [ACMethodCommand("", "en{'Delete'}de{'Löschen'}", 701, Description = "Removes the SelectedFacilityMaterial from the SelectedFacility and deletes it from the database.")]
        public void DeleteFacilityMaterial()
        {
            if (!IsEnabledDeleteFacilityMaterial())
                return;

            SelectedFacility.FacilityMaterial_Facility.Remove(SelectedFacilityMaterial);
            FacilityMaterialList.Remove(SelectedFacilityMaterial);

            SelectedFacilityMaterial.DeleteACObject(DatabaseApp, false);

            OnPropertyChanged(nameof(FacilityMaterialList));
        }

        public bool IsEnabledDeleteFacilityMaterial()
        {
            return SelectedFacilityMaterial != null;
        }


        [ACMethodCommand("", "en{'Generate OEE test data'}de{'OEE Testdaten generieren'}", 703, Description = "Generates OEE test data for the SelectedFacilityMaterial via the FacilityOEEManager.")]
        public void GenerateTestOEEData()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                FacilityMaterial facilityMaterial = SelectedFacilityMaterial.FromAppContext<FacilityMaterial>(dbApp);
                Msg msg = FacilityOEEManager.GenerateTestOEEData(dbApp, facilityMaterial);
                if (msg != null)
                    Messages.MsgAsync(msg);
            }
        }

        public bool IsEnabledGenerateTestOEEData()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }

        [ACMethodCommand("", "en{'Delete OEE test data'}de{'OEE Testdaten löschen'}", 704, Description = "Deletes the OEE test data of the SelectedFacilityMaterial.")]
        public void DeleteTestOEEData()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                FacilityMaterial facilityMaterial = SelectedFacilityMaterial.FromAppContext<FacilityMaterial>(dbApp);
                FacilityOEEManager.DeleteTestOEEData(dbApp, facilityMaterial);
            }
        }

        public bool IsEnabledDeleteTestOEEData()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }


        [ACMethodCommand("", "en{'Recalc average throughput'}de{'Aktualisiere Mittelwert Durchsatz'}", 705, Description = "Recalculates the average throughput of the SelectedFacilityMaterial.")]
        public void RecalcThroughputAverage()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;

            Msg msg = FacilityOEEManager.RecalcThroughputAverage(this.DatabaseApp, SelectedFacilityMaterial, false);
            if (msg != null)
                Messages.MsgAsync(msg);
        }

        public bool IsEnabledRecalcThroughputAverage()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }

        [ACMethodCommand("", "en{'Correct throuhputs and OEE'}de{'Korrigiere Durchsätze und OEE'}", 706, Description = "Recalculates the throughputs and OEE values of the SelectedFacilityMaterial.")]
        public void RecalcThroughputAndOEE()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                FacilityMaterial facilityMaterial = SelectedFacilityMaterial.FromAppContext<FacilityMaterial>(dbApp);
                Msg msg = FacilityOEEManager.RecalcThroughputAndOEE(dbApp, facilityMaterial, null, null);
                if (msg != null)
                    Messages.MsgAsync(msg);
            }
        }

        public bool IsEnabledRecalcThroughputAndOEE()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Save):
                    Save();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(AddFacility):
                    AddFacility();
                    return true;
                case nameof(IsEnabledAddFacility):
                    result = IsEnabledAddFacility();
                    return true;
                case nameof(DeleteFacility):
                    DeleteFacility();
                    return true;
                case nameof(IsEnabledDeleteFacility):
                    result = IsEnabledDeleteFacility();
                    return true;
                case nameof(AddFacilityMaterial):
                    AddFacilityMaterial();
                    return true;
                case nameof(IsEnabledAddFacilityMaterial):
                    result = IsEnabledAddFacilityMaterial();
                    return true;
                case nameof(DeleteFacilityMaterial):
                    DeleteFacilityMaterial();
                    return true;
                case nameof(IsEnabledDeleteFacilityMaterial):
                    result = IsEnabledDeleteFacilityMaterial();
                    return true;
                case nameof(GenerateTestOEEData):
                    GenerateTestOEEData();
                    return true;
                case nameof(IsEnabledGenerateTestOEEData):
                    result = IsEnabledGenerateTestOEEData();
                    return true;
                case nameof(DeleteTestOEEData):
                    DeleteTestOEEData();
                    return true;
                case nameof(IsEnabledDeleteTestOEEData):
                    result = IsEnabledDeleteTestOEEData();
                    return true;
                case nameof(RecalcThroughputAverage):
                    RecalcThroughputAverage();
                    return true;
                case nameof(IsEnabledRecalcThroughputAverage):
                    result = IsEnabledRecalcThroughputAverage();
                    return true;
                case nameof(RecalcThroughputAndOEE):
                    RecalcThroughputAndOEE();
                    return true;
                case nameof(IsEnabledRecalcThroughputAndOEE):
                    result = IsEnabledRecalcThroughputAndOEE();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(Load):
                case nameof(IsEnabledLoad):
                case nameof(AddFacilityMaterial):
                case nameof(IsEnabledAddFacilityMaterial):
                case nameof(SetProcessWorkflow):
                case nameof(IsEnabledSetProcessWorkflow):
                case nameof(RemoveProcessWorkflow):
                case nameof(IsEnabledRemoveProcessWorkflow):
                     return new string[] { nameof(SelectedFacility) };
                case nameof(DeleteFacilityMaterial):
                case nameof(IsEnabledDeleteFacilityMaterial):
                case nameof(GenerateTestOEEData):
                case nameof(IsEnabledGenerateTestOEEData):
                case nameof(DeleteTestOEEData):
                case nameof(IsEnabledDeleteTestOEEData):
                case nameof(RecalcThroughputAverage):
                case nameof(IsEnabledRecalcThroughputAverage):
                case nameof(RecalcThroughputAndOEE):
                case nameof(IsEnabledRecalcThroughputAndOEE):
                     return new string[] { nameof(SelectedFacilityMaterial) };
                case nameof(SetWFOk):
                case nameof(IsEnabledSetWFOk):
                     return new string[] { nameof(SelectedFacility), nameof(SelectedProcessWorkflow) };
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

        #region Assigned Workflow

        [ACMethodInfo(nameof(SetProcessWorkflow), "en{'Assign process workflow'}de{'Prozess-Workflow zuweisen'}", 50, true, Description = "Opens the dialog for assigning a process workflow to the SelectedFacility. Select a workflow in the dialog and confirm with SetWFOk().")]
        public async Task SetProcessWorkflow()
        {
            if (!IsEnabledSetProcessWorkflow())
            {
                return;
            }
            await ShowDialogAsync(this, "DlgSetProcessWorkfow");
        }

        public bool IsEnabledSetProcessWorkflow()
        {
            return SelectedFacility != null;
        }

        [ACMethodInfo(nameof(RemoveProcessWorkflow), "en{'Remove process workflow'}de{'Prozess-Workflow entfernen'}", 51, true, Description = "Removes the process workflow assigned to the SelectedFacility after a confirmation question (Question50111).")]
        public async void RemoveProcessWorkflow()
        {
            if (!IsEnabledRemoveProcessWorkflow())
            {
                return;
            }
            // Question50111
            // Remove assigned process workflow from facility?
            // Zugewiesenen Prozessablauf aus der Einrichtung entfernen?
            Global.MsgResult saveQuestion = await Messages.QuestionAsync(this, "Question50111", Global.MsgResult.No);
            if (saveQuestion == Global.MsgResult.Yes)
            {
                SelectedFacility.VBiACClassMethod = null;
                OnPropertyChanged(nameof(SelectedFacility));
            }
        }

        public bool IsEnabledRemoveProcessWorkflow()
        {
            return SelectedFacility != null && SelectedFacility.VBiACClassMethodID != null;
        }

        [ACMethodInfo(nameof(SetProcessWorkflow), "en{'Assign process workflow'}de{'Prozess-Workflow zuweisen'}", 50, true, Description = "Assigns the SelectedProcessWorkflow to the SelectedFacility and closes the workflow assignment dialog.")]
        public void SetWFOk()
        {
            if (!IsEnabledSetWFOk())
            {
                return;
            }
            SelectedFacility.VBiACClassMethod = SelectedProcessWorkflow.FromAppContext<gip.mes.datamodel.ACClassMethod>(DatabaseApp);
            CloseTopDialog();
            OnPropertyChanged(nameof(SelectedFacility));
        }

        public bool IsEnabledSetWFOk()
        {
            return SelectedFacility != null && SelectedProcessWorkflow != null;
        }


        #endregion


        #endregion
    }
}
