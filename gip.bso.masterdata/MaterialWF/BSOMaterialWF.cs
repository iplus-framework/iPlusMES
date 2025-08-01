// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.manager;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace gip.bso.masterdata
{
    /// <summary>
    /// </summary>
    [ACClassConstructorInfo(
        new object[]
        {
            new object[] {"AutoFilter", Global.ParamOption.Optional, typeof(String)},
        }
    )]
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Material Workflow'}de{'Material-Workflow'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MaterialWF.ClassName)]
    public class BSOMaterialWF : ACBSOvbNav, IACBSOConfigStoreSelection
    {

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOPartslist"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOMaterialWF(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

            _PartslistManager = ACPartslistManager.ACRefToServiceInstance(this);
            if (_PartslistManager == null)
                throw new Exception("PartslistManager not configured");


            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_PresenterSubscribed && this.ProcessWorkflowPresenter != null)
            {
                this.ProcessWorkflowPresenter.PropertyChanged -= ProcessWorkflowPresenter_PropertyChanged;
                _PresenterSubscribed = false;
            }
            if (_VBDesignerMaterialWF != null)
            {
                _VBDesignerMaterialWF.PropertyChanged -= _VBDesignerMaterialWF_PropertyChanged;
                _VBDesignerMaterialWF = null;
            }

            if (_PartslistManager != null)
                ACPartslistManager.DetachACRefFromServiceInstance(this, _PartslistManager);
            _PartslistManager = null;

            this._ProcessWorkflow = null;
            this._selectedMaterial = null;
            this._selectedMixure = null;
            this._presenter = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region private members

        bool _IsCurrentMaterialWFNoChanged = false;
        bool _IsSavedAfterAddedNewMaterialWF = true;

        #endregion

        #region Filters

        #region Filter -> InputMaterials -> Select, (Current,) List
        ACAccess<Material> _AccessInputMaterials;
        [ACPropertyAccess(9999, "InputMaterials")]
        public ACAccess<Material> AccessInputMaterials
        {
            get
            {
                if (_AccessInputMaterials == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Material.ClassName, ACType.ACIdentifier);
                    navACQueryDefinition.ClearFilter();
                    ACFilterItem aciFilterItem = new ACFilterItem(Global.FilterTypes.filter, "IsIntermediate", Global.LogicalOperators.equal, Global.Operators.and, "True", false);
                    navACQueryDefinition.ACFilterColumns.Add(aciFilterItem);
                    //if (navACQueryDefinition.ACFilterColumns.Any())
                    //{
                    //    foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                    //    {
                    //        if (filterItem.PropertyName != "IsIntermediate")
                    //            filterItem.SearchWord = null;
                    //    }
                    //}
                    _AccessInputMaterials = navACQueryDefinition.NewAccessNav<Material>(Material.ClassName, this);
                    _AccessInputMaterials.NavSearch();
                }
                return _AccessInputMaterials;
            }
        }

        /// <summary>
        /// Its invoked from a WPF-Itemscontrol that wants to refresh its CollectionView because the user has changed the LINQ-Expressiontree in the ACQueryDefinition-Property of IAccess. 
        /// The BSO should execute the query on the database first, to get the new results for refreshing the CollectionView of the control.
        /// If the bso don't want to handle this request or manipulate the ACQueryDefinition it returns false. The WPF-control invokes then the IAccess.NavSearch()-Method itself.  
        /// </summary>
        /// <param name="acAccess">Reference to IAccess that contains the changed query (Property NavACQueryDefinition)</param>
        /// <returns>True if the bso has handled this request and queried the database context. Otherwise it returns false.</returns>
        public override bool ExecuteNavSearch(IAccess acAccess)
        {
            if (acAccess == _AccessInputMaterials)
            {
                _AccessInputMaterials.NavSearch(this.DatabaseApp);
                OnPropertyChanged("InputMaterialsList");
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }

        /// <summary>
        /// All available materials for input in partslist
        /// </summary>
        [ACPropertyList(651, "InputMaterials")]
        public IEnumerable<Material> InputMaterialsList
        {
            get
            {
                if (AccessInputMaterials == null)
                    return null;
                return AccessInputMaterials.NavList;
            }
        }

        private Material _SelectedInputMaterials;
        [ACPropertySelected(652, "InputMaterials", "en{'All materials'}de{'Alle Materialien'}")]
        public Material SelectedInputMaterials
        {
            get
            {
                return _SelectedInputMaterials;
            }
            set
            {
                if (_SelectedInputMaterials != value)
                {
                    _SelectedInputMaterials = value;
                    OnPropertyChanged("SelectedInputMaterials");
                }
            }
        }
        #endregion

        #endregion

        #region Properties (Manager)

        protected ACRef<ACPartslistManager> _PartslistManager = null;
        protected ACPartslistManager PartslistManager
        {
            get
            {
                if (_PartslistManager == null)
                    return null;
                return _PartslistManager.ValueT;
            }
        }

        #endregion

        #region MaterialWF

        #region MaterialWF -> AccessNav
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MaterialWF> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MaterialWF.ClassName)]
        public ACAccessNav<MaterialWF> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MaterialWF>(MaterialWF.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        #endregion

        #region MaterialWF -> Select, (Current,) List
        /// <summary>
        /// Gets the partslist list.
        /// </summary>
        /// <value>The partslist list.</value>
        [ACPropertyList(601, MaterialWF.ClassName)]
        public IEnumerable<MaterialWF> MaterialWFList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the current partslist.
        /// </summary>
        /// <value>The current partslist.</value>
        [ACPropertyCurrent(602, MaterialWF.ClassName)]
        public MaterialWF CurrentMaterialWF
        {
            get
            {
                return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary.Current != value)
                {
                    AccessPrimary.Current = value;

                    if (value != null
                        && value.XMLDesign == null
                        && VBDesignerMaterialWF != null)
                        VBDesignerMaterialWF.DoInsertRoot(value, null);
                    if (MaterialWFPresenter != null)
                        MaterialWFPresenter.Load(value);
                    CurrentChanged();
                    OnPropertyChanged("CurrentMaterialWF");
                    OnPropertyChanged("MaterialPWNodeConnectionList");
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected partslist.
        /// </summary>
        /// <value>The selected partslist.</value>
        [ACPropertySelected(603, MaterialWF.ClassName)]
        public MaterialWF SelectedMaterialWF
        {
            get
            {
                return AccessPrimary.Selected;
            }
            set
            {
                AccessPrimary.Selected = value;
                CurrentMaterialWF = value;
                OnPropertyChanged("SelectedMaterialWF");

            }
        }

        private void CurrentChanged()
        {
            _IsCurrentMaterialWFNoChanged = false;
            if (CurrentMaterialWF != null)
                CurrentMaterialWFNo = CurrentMaterialWF.MaterialWFNo;
            SearchMaterials();
            LoadProcessWorkflows();
        }

        string _currentMaterialWFNo;
        [ACPropertyInfo(604, "", "en{'Material Workflow No.'}de{'Material-Workflow Nr.'}")]
        public string CurrentMaterialWFNo
        {
            get
            {
                return _currentMaterialWFNo;
            }
            set
            {
                _currentMaterialWFNo = value;
                if (!string.IsNullOrEmpty(_currentMaterialWFNo)
                    && VBDesignerMaterialWF != null
                    && VBDesignerMaterialWF.IsDesignMode
                    && _IsCurrentMaterialWFNoChanged
                    && MaterialWFPresenter != null)
                {
                    string oldName = CurrentMaterialWF.MaterialWFNo;
                    CurrentMaterialWF.MaterialWFNo = _currentMaterialWFNo;
                    VBDesignerMaterialWF.ChangeMaterialWFName(oldName, CurrentMaterialWF.MaterialWFNo);
                    MaterialWFPresenter.Load(CurrentMaterialWF);
                    BroadcastToVBControls(Const.CmdDesignModeOff, null, new object[1] { this });
                }
                OnPropertyChanged("CurrentMaterialWFNo");
                _IsCurrentMaterialWFNoChanged = true;
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _NewMaterialWFNo;
        [ACPropertyInfo(999, nameof(NewMaterialWFNo), "en{'New Material Workflow No.'}de{'Neu Material-Workflow Nr.'}")]
        public string NewMaterialWFNo
        {
            get
            {
                return _NewMaterialWFNo;
            }
            set
            {
                if (_NewMaterialWFNo != value)
                {
                    _NewMaterialWFNo = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        #region MaterialWF -> Methods

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MaterialWF.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            AccessPrimary.NavSearch(DatabaseApp);
            SelectedMaterialWF = MaterialWFList != null ? MaterialWFList.FirstOrDefault() : null;
            OnPropertyChanged("MaterialWFList");
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(MaterialWF.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
            _IsSavedAfterAddedNewMaterialWF = true;
        }

        private bool IsLoadDisabled = false;
        [ACMethodInteraction(MaterialWF.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMaterialWF", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (IsLoadDisabled) return;
            IsLoadDisabled = true;
            LoadEntity<MaterialWF>(requery, () => SelectedMaterialWF, () => CurrentMaterialWF, c => CurrentMaterialWF = c,
                        DatabaseApp.MaterialWF
                        .Where(c => c.MaterialWFID == SelectedMaterialWF.MaterialWFID));
            if (MaterialWFPresenter != null)
                MaterialWFPresenter.Load(SelectedMaterialWF);
            IsLoadDisabled = false;
        }

        protected override Msg OnPreSave()
        {
            if (!ConfigManagerIPlus.MustConfigBeReloadedOnServer(this, VisitedMethods, this.Database))
                this.VisitedMethods = null;
            return base.OnPreSave();
        }

        protected override void OnPostSave()
        {
            ConfigManagerIPlus.ReloadConfigOnServerIfChanged(this, VisitedMethods, this.Database);
            this.VisitedMethods = null;
            base.OnPostSave();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand(MaterialWF.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        protected override void OnPostUndoSave()
        {
            this.VisitedMethods = null;
            base.OnPostUndoSave();
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MaterialWF.ClassName, Const.New, (short)MISort.New, true, "SelectedMaterialWF", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!IsEnabledNew())
                return;
            _IsSavedAfterAddedNewMaterialWF = false;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(MaterialWF), MaterialWF.NoColumnName, MaterialWF.FormatNewNo, this);
            MaterialWF newMaterialWF = MaterialWF.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.MaterialWF.Add(newMaterialWF);
            AccessPrimary.NavList.Add(newMaterialWF);
            VBDesignerMaterialWF.DoInsertRoot(newMaterialWF, null);
            SelectedMaterialWF = newMaterialWF;
            OnPropertyChanged("MaterialWFList");
            OnPropertyChanged("MaterialPWNodeConnectionList");

            if (ACSaveOrUndoChanges())
                _IsSavedAfterAddedNewMaterialWF = true;
        }

        public bool IsEnabledNew()
        {
            return VBDesignerMaterialWF != null;
        }


        [ACMethodInteraction(MaterialWF.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.New, true, "SelectedMaterialWF", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            MaterialWFACClassMethod item;
            MsgWithDetails msg = null;
            IsLoadDisabled = true;
            if (CurrentMaterialWF.Partslist_MaterialWF.Any())
            {
                msg = new MsgWithDetails();
                string partslist = string.Join(",", CurrentMaterialWF.Partslist_MaterialWF.Select(x => x.PartslistName).ToArray());
                partslist = partslist.TrimEnd(',');
                msg.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, @"Error50046", partslist) });
            }

            if (msg == null && CurrentMaterialWF.MaterialWFRelation_MaterialWF.Any())
            {
                Global.MsgResult result = Messages.Msg(new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, @"Error50047", CurrentMaterialWF.Name) }, Global.MsgResult.No, eMsgButton.YesNo);
                if (result == Global.MsgResult.Yes)
                {
                    msg = new MsgWithDetails();
                    while (CurrentMaterialWF.MaterialWFACClassMethod_MaterialWF.Any())
                    {
                        item = this.ProcessWorkflowList.First();
                        this.RemoveProcessWorkflowConnections(item);
                        Msg wfMsg = item.DeleteACObject(DatabaseApp, false);
                        if (wfMsg != null)
                            msg.AddDetailMessage(wfMsg);
                    }

                    List<MaterialWFRelation> relations = CurrentMaterialWF.MaterialWFRelation_MaterialWF.ToList();
                    foreach (var relation in relations)
                    {
                        Msg relMsg = relation.DeleteACObject(DatabaseApp, false);
                        if (relMsg != null)
                            msg.AddDetailMessage(relMsg);
                    }
                }
            }

            if (msg != null && msg.MsgDetailsCount > 0)
            {
                Messages.Msg(msg);
            }
            else
            {
                msg = CurrentMaterialWF.DeleteACObject(DatabaseApp, false);
            }
            if (msg != null)
            {
                Messages.Msg(msg);
            }
            else
            {
                AccessPrimary.NavList.Remove(CurrentMaterialWF);
                SelectedMaterialWF = AccessPrimary.NavList.FirstOrDefault();
            }
            IsLoadDisabled = false;
            OnPropertyChanged("MaterialWFList");
        }

        #region  MaterialWF -> Methods -> IsEnabled

        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }
        public bool IsEnabledDelete()
        {
            return SelectedMaterialWF != null;
        }

        #endregion


        /// <summary>
        /// Clone current material workflow
        /// </summary>
        [ACMethodInteraction(nameof(CloneMaterialWF), "en{'Clone'}de{'Duplizieren'}", 500, true, nameof(SelectedMaterialWF))]
        public void CloneMaterialWF()
        {
            if (!IsEnabledCloneMaterialWF())
                return;
            NewMaterialWFNo = null;
            for (int i = 1; i < 100; i++)
            {
                string newNo = $"{CurrentMaterialWF.MaterialWFNo}_{i:000}";
                bool existWithSameNo = DatabaseApp.MaterialWF.Any(m => m.MaterialWFNo == newNo);
                if (!existWithSameNo)
                {
                    NewMaterialWFNo = newNo;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(NewMaterialWFNo))
            {
                ShowDialog(this, "DlgCloneMaterialWF");
            }
        }

        /// <summary>
        /// Is clone from current material workflow enabled.
        /// </summary>
        /// <returns>True if is enabled, otherwise false.</returns>
        public bool IsEnabledCloneMaterialWF()
        {
            return SelectedMaterialWF != null;
        }

        /// <summary>
        /// 
        /// </summary>
        [ACMethodInfo(nameof(CloneMaterialWFOK), Const.Ok, 999)]
        public void CloneMaterialWFOK()
        {
            if (!IsEnabledCloneMaterialWFOK())
                return;

            bool isValidName = IsValidXamlName(NewMaterialWFNo);
            if (!isValidName)
            {
                // Error50714
                // BSOMaterialWF
                // Material Workflow No. {0} is not valid!
                // Material-Workflow Nr. {0} nicht gültig!
                Msg msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, "Error50714", NewMaterialWFNo) };
                Messages.Msg(msg, Global.MsgResult.OK);
                return;
            }

            bool existWithSameNo = DatabaseApp.MaterialWF.Any(m => m.MaterialWFNo == NewMaterialWFNo);
            if (existWithSameNo)
            {
                // Error50715
                // BSOMaterialWF
                // Material Workflow No. {0} already exist!
                // Material-Workflow Nr. {0} existiert bereits!
                var msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, "Error50715", NewMaterialWFNo) };
                Messages.Msg(msg, Global.MsgResult.OK);
            }
            else
            {
                CloseTopDialog();

                BackgroundWorker.RunWorkerAsync(nameof(DoCloneMaterialWF));
                ShowDialog(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledCloneMaterialWFOK()
        {
            return CurrentMaterialWF != null && !string.IsNullOrEmpty(NewMaterialWFNo);
        }

        private static readonly Regex _validIdentifierRegex = new Regex(@"^[_\p{L}][_\p{L}\p{N}]*$", RegexOptions.Compiled);

        public static bool IsValidXamlName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (name.Contains("(") || name.Contains(")"))
                return false;

            return _validIdentifierRegex.IsMatch(name);
        }

        #endregion

        #endregion

        #region Material

        #region Material -> Select, (Current,) List

        [ACPropertyList(605, "Materials")]
        public IEnumerable<Material> MaterialList
        {
            get
            {
                if (CurrentMaterialWF == null) return null;
                return CurrentMaterialWF.GetMaterials();
            }
        }

        private Material _selectedMaterial;

        [ACPropertySelected(606, "Materials")]
        public Material SelectedMaterial
        {
            get
            {
                return _selectedMaterial;
            }
            set
            {
                SetSelectedMaterial(value, true);
            }
        }

        [ACMethodInfo("Material", "en{'SetSelectedMaterial'}de{'SetSelectedMaterial'}", 605)]
        public void SetSelectedMaterial(Material value, bool selectPWNode = false)
        {
            if (_selectedMaterial != value)
            {
                _selectedMaterial = value;
                SearchMixures();
                OnPropertyChanged("SelectedMaterial");
                if (selectPWNode)
                    SelectProcessWorkflowNode();
            }
        }

        #endregion

        #region Material -> Methods

        [ACMethodInfo("Materials", "en{'New intermediate product'}de{'Neues Zwischenprodukt'}", 601, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public void AddMaterialDlg()
        {
            ShowDialog(this, "AddMaterialDlg");
        }

        [ACMethodInfo("Materials", Const.Cancel, 602, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public void AddMaterialDlgCancel()
        {
            CloseTopDialog();
        }

        [ACMethodInteraction("Materials", Const.Ok, 603, true, "SelectedMaterial", Global.ACKinds.MSMethodPrePost)]
        public void AddMaterialOK()
        {
            if (!IsEnabledAddMaterialOK()) return;
            MaterialWFRelation relation = null;
            if (!MaterialList.Any())
            {
                // add new root material
                relation = MaterialWFRelation.NewACObject(DatabaseApp, null);
                relation.TargetMaterial = SelectedInputMaterials;
                CurrentMaterialWF.MaterialWFRelation_MaterialWF.Add(relation);

            }
            else if (MaterialList.Count() == 1 && CurrentMaterialWF.MaterialWFRelation_MaterialWF.Count() == 1)
            {
                relation = CurrentMaterialWF.MaterialWFRelation_MaterialWF.FirstOrDefault();
                relation.SourceMaterial = SelectedInputMaterials;
            }
            else
            {
                if (SelectedInputMaterials == SelectedMaterial)
                {
                    var msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, @"Error50058", relation.SourceMaterial.MaterialNo) };
                    Messages.Msg(msg, Global.MsgResult.OK);
                    return;
                }
                // for selected material add new input
                relation = MaterialWFRelation.NewACObject(DatabaseApp, null);
                relation.SourceMaterial = SelectedInputMaterials;
                relation.TargetMaterial = SelectedMaterial;
                CurrentMaterialWF.MaterialWFRelation_MaterialWF.Add(relation);
            }

            CloseTopDialog();
            OnPropertyChanged("MaterialList");
            OnPropertyChanged("MixureList");
            //VBDesignerMaterialWF.UpdateDesigner(relation, this);
        }


        #region Material -> Methods -> IsEnabled

        public bool IsEnabledAddMaterialDlg()
        {
            return SelectedMaterialWF != null && !CurrentMaterialWF.Partslist_MaterialWF.Any();
        }

        public bool IsEnabledAddMaterialOK()
        {
            return SelectedInputMaterials != null && CurrentMaterialWF != null;
        }

        #endregion

        #endregion

        #region Material -> Search (SearchSectionTypeName)
        public void SearchMaterials()
        {
            if (MaterialList != null)
            {
                foreach (var item in MaterialList)
                    item.MixingLevel = MaterialWF.CalculateMixingLevel(item, CurrentMaterialWF.GetMaterials(), CurrentMaterialWF.MaterialWFRelation_MaterialWF);

            }

            SelectedMaterial = MaterialList != null ? MaterialList.FirstOrDefault() : null;
            OnPropertyChanged("MaterialList");
        }

        #endregion

        #endregion

        #region Mixures

        #region Mixures -> Select, (Current,) List

        [ACPropertyList(607, "Mixure")]
        public List<MaterialWFRelation> MixureList
        {
            get
            {
                if (SelectedMaterial == null) return null;
                return SelectedMaterial.MaterialWFRelation_TargetMaterial.Where(x => x.MaterialWFID == CurrentMaterialWF.MaterialWFID).OrderBy(x => x.Sequence).ToList();
            }
        }

        private MaterialWFRelation _selectedMixure;

        [ACPropertySelected(608, "Mixure")]
        public MaterialWFRelation SelectedMixure
        {
            get
            {
                return _selectedMixure;
            }
            set
            {
                if (_selectedMixure != value)
                {
                    _selectedMixure = value;
                    OnPropertyChanged("SelectedMixure");
                }
            }
        }

        #endregion

        #region Mixures -> Methods

        [ACMethodInteraction("Mixure", "en{'New part'}de{'Neuer Anteil'}", (short)MISort.New, true, "SelectedMixure", Global.ACKinds.MSMethodPrePost)]
        public void NewMaterialWFRelation()
        {
            if (!PreExecute("NewMaterialWFRelation")) return;
            MaterialWFRelation newMixRelation = MaterialWFRelation.NewACObject(DatabaseApp, null);
            newMixRelation.MaterialWFID = CurrentMaterialWF.MaterialWFID;
            SelectedMaterial.MaterialWFRelation_TargetMaterial.Add(newMixRelation);
            OnPropertyChanged("MixureList");
            PostExecute("NewMaterialWFRelation");
        }

        [ACMethodInteraction("Mixure", "en{'Delete part'}de{'Anteil löschen'}", (short)MISort.Delete, true, "SelectedMixure", Global.ACKinds.MSMethodPrePost)]
        public void DeleteMaterialWFRelation()
        {
            if (!PreExecute("DeleteMaterialWFRelation"))
                return;
            Msg msg = SelectedMixure.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Global.MsgResult result = Messages.Msg(msg, Global.MsgResult.No, eMsgButton.YesNo);
                if (result == Global.MsgResult.Yes)
                {
                    msg = SelectedMixure.DeleteACObject(DatabaseApp, false);
                    if (msg != null)
                    {
                        Messages.Msg(msg);
                    }
                }
            }
            if (msg == null)
            {
                SelectedMaterial.MaterialWFRelation_TargetMaterial.Remove(SelectedMixure);
                OnPropertyChanged("MaterialList");
                OnPropertyChanged("MixureList");
                SelectedMixure = MixureList != null ? MixureList.FirstOrDefault() : null;
            }
            PostExecute("DeleteMaterialWFRelation");
        }

        #region Mixures -> Methods -> IsEnabled

        public bool IsEnabledNewMaterialWFRelation()
        {
            return SelectedMaterial != null && !CurrentMaterialWF.Partslist_MaterialWF.Any();
        }

        public bool IsEnabledDeleteMaterialWFRelation()
        {
            return SelectedMixure != null && !CurrentMaterialWF.Partslist_MaterialWF.Any();
        }

        #endregion

        #endregion

        #region Mixures -> Selectd
        public void SearchMixures()
        {
            SelectedMixure = MixureList != null ? MixureList.FirstOrDefault() : null;
            OnPropertyChanged("MixureList");
        }

        #endregion

        #endregion

        #region Process workflow

        private VBPresenterMethod _presenter = null;
        private MaterialWFACClassMethod _ProcessWorkflow;
        bool _PresenterRightsChecked = false;

        public VBPresenterMethod ProcessWorkflowPresenter
        {
            get
            {
                if (_presenter == null)
                {
                    _presenter = this.ACUrlCommand("VBPresenterMethod(CurrentDesign)") as VBPresenterMethod;
                    if (_presenter == null && !_PresenterRightsChecked)
                        Messages.Error(this, "This user has no rights for viewing workflows. Assign rights for VBPresenterMethod in the group management!", true);
                    _PresenterRightsChecked = true;
                }
                return _presenter;
            }
        }

        [ACPropertyList(609, "ProcessWorkflow")]
        public ICollection<MaterialWFACClassMethod> ProcessWorkflowList
        {
            get
            {
                if (this.SelectedMaterialWF == null)
                    return new MaterialWFACClassMethod[0];
                else
                    return this.SelectedMaterialWF.MaterialWFACClassMethod_MaterialWF.ToList();
            }
        }

        private bool _PresenterSubscribed = false;
        [ACPropertyCurrent(610, "ProcessWorkflow")]
        public MaterialWFACClassMethod CurrentProcessWorkflow
        {
            get
            {
                return _ProcessWorkflow;
            }
            set
            {
                _ProcessWorkflow = value;

                if (this.ProcessWorkflowPresenter != null)
                {
                    if (!_PresenterSubscribed)
                    {
                        this.ProcessWorkflowPresenter.PropertyChanged += ProcessWorkflowPresenter_PropertyChanged;
                        _PresenterSubscribed = true;
                    }
                    if (_ProcessWorkflow == null)
                        this.ProcessWorkflowPresenter.Load(null);
                    else
                        this.ProcessWorkflowPresenter.Load(_ProcessWorkflow.ACClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>());
                }

                OnPropertyChanged("CurrentProcessWorkflow");
                OnPropertyChanged("MaterialPWNodeConnectionList");
            }
        }

        void ProcessWorkflowPresenter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedWFNode")
            {
                if (ProcessWorkflowPresenter.SelectedWFNode == null)
                    return;
                var connection = CurrentProcessWorkflow.MaterialWFConnection_MaterialWFACClassMethod.Where(c => c.ACClassWFID == (ProcessWorkflowPresenter.SelectedWFNode.Content as gip.core.datamodel.ACClassWF).ACClassWFID).FirstOrDefault();
                if (connection != null)
                {
                    var material = this.MaterialList.Where(c => c.MaterialID == connection.MaterialID).FirstOrDefault();
                    SetSelectedMaterial(material, false);
                }
            }
        }

        [ACMethodInteraction("ProcessWorkflow", "en{'Add workflow'}de{'Steuerrezept hinzufügen'}", (short)MISort.New, true, "CurrentProcessWorkflow")]
        public void AddProcessWorkflow()
        {
            ShowDialog(this, "SelectProcessWorkflow");
        }

        public bool IsEnabledAddProcessWorkflow()
        {
            return this.NewProcessWorkflowList.Any();
        }

        [ACMethodInteraction("ProcessWorkflow", "en{'Remove workflow'}de{'Steuerrezept entfernen'}", (short)MISort.Delete, true, "CurrentProcessWorkflow")]
        public void RemoveProcessWorkflow()
        {
            if (this.CurrentProcessWorkflow != null)
            {
                this.RemoveProcessWorkflowConnections(this.CurrentProcessWorkflow);
                _ProcessWorkflow.DeleteACObject(this.DatabaseApp, false);
                this.SelectedMaterialWF.MaterialWFACClassMethod_MaterialWF.Remove(_ProcessWorkflow);
                this.CurrentProcessWorkflow = this.ProcessWorkflowList.FirstOrDefault();
            }

            OnPropertyChanged("ProcessWorkflowList");
            OnPropertyChanged("IsEnabledAddProcessWorkflow");
            OnPropertyChanged("IsEnabledRemoveProcessWorkflow");
            OnPropertyChanged("MaterialPWNodeConnectionList");
        }

        public bool IsEnabledRemoveProcessWorkflow()
        {
            return _ProcessWorkflow != null;
        }

        private void LoadProcessWorkflows()
        {
            OnPropertyChanged("ProcessWorkflowList");
            this.CurrentProcessWorkflow = this.ProcessWorkflowList.FirstOrDefault();
        }

        [ACPropertyInfo(9999)]
        public List<ConnectionIACObject> MaterialPWNodeConnectionList
        {
            get
            {
                if (!_IsConnectionShow)
                    return null;
                return CreateMaterialPWNodeConnectonList();
            }
        }

        public List<ConnectionIACObject> CreateMaterialPWNodeConnectonList()
        {
            if (this.CurrentProcessWorkflow == null)
                return null;
            List<ConnectionIACObject> tempList = new List<ConnectionIACObject>();
            if (this.CurrentProcessWorkflow != null)
            {
                foreach (Material material in MaterialList)
                {
                    foreach (var connection in material.MaterialWFConnection_Material.Where(m => m.MaterialWFACClassMethodID == this.CurrentProcessWorkflow.MaterialWFACClassMethodID))
                    {
                        if (connection.ACClassWF != null && ProcessWorkflowPresenter != null)
                        {
                            var node = this.FindPWNode(this.ProcessWorkflowPresenter.SelectedRootWFNode, connection.ACClassWF.ACClassWFID);
                            if (node != null)
                                tempList.Add(new ConnectionIACObject(material, node.ContentACClassWF));
                        }
                    }
                }
            }
            return tempList;
        }

        bool _IsConnectionShow = true;

        /// <summary>
        /// ACAction is called when one IACInteractiveObject (Source) wants to inform another IACInteractiveObject (Target) about an relevant interaction-event.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public override void ACAction(ACActionArgs actionArgs)
        {
            if ((actionArgs.ElementAction == Global.ElementActionType.TabItemActivated
                    && actionArgs.DropObject.VBContent == "*TabMaterials")
                || actionArgs.ElementAction == Global.ElementActionType.DesignModeOn)
            {
                _IsConnectionShow = false;
                OnPropertyChanged("MaterialPWNodeConnectionList");
                OnPropertyChanged("CurrentMaterialWFNo");
                actionArgs.Handled = true;
            }
            else if (
                        (
                            (actionArgs.DropObject.VBContent == "*Workflows"
                             && (actionArgs.ElementAction == Global.ElementActionType.TabItemActivated
                                 || actionArgs.ElementAction == Global.ElementActionType.TabItemLoaded)
                            )
                          || actionArgs.ElementAction == Global.ElementActionType.DesignModeOff
                          || actionArgs.ElementAction == Global.ElementActionType.Refresh
                         )
                      && VBDesignerMaterialWF != null
                      && ProcessWorkflowPresenter != null
                      && !VBDesignerMaterialWF.IsDesignMode
                      && !((VBDesignerWorkflowMethod)ProcessWorkflowPresenter.ACUrlCommand("VBDesignerWorkflowMethod(CurrentDesign)")).IsDesignMode
                    )
            {
                _IsConnectionShow = true;
                OnPropertyChanged("MaterialPWNodeConnectionList");
                OnPropertyChanged("CurrentMaterialWFNo");
                actionArgs.Handled = true;
            }
            else if (actionArgs.ElementAction == Global.ElementActionType.VBDesignLoaded)
            {
                OnVBDesignLoaded(actionArgs.DropObject.VBContent);
            }
            else if (actionArgs.ElementAction == Global.ElementActionType.ContextMenu)
            {
                if (!_IsSavedAfterAddedNewMaterialWF)
                    return;
            }

            base.ACAction(actionArgs);
        }

        private void OnVBDesignLoaded(string vbContent)
        {
            if (vbContent == "SelectedRootWFNode"
                && MaterialWFPresenter != null)
            {
                MaterialWFPresenter.SelectMaterial(SelectedMaterial);
            }
        }

        private void SelectProcessWorkflowNode()
        {
            if (!IsEnabledRemoveMaterialConnection())
                this.SelectProcessWorkflowNode(Guid.Empty);
            else
            {
                foreach (var connection in SelectedMaterial.MaterialWFConnection_Material.Where(m => m.MaterialWFACClassMethodID == this.CurrentProcessWorkflow.MaterialWFACClassMethodID))
                {
                    if (connection.ACClassWF != null)
                    {
                        var acClassWFID = connection.ACClassWF.ACClassWFID;
                        if (this.SelectProcessWorkflowNode(acClassWFID))
                            break;
                    }
                }
            }
        }

        private bool SelectProcessWorkflowNode(Guid acClassWFID)
        {
            if (this.ProcessWorkflowPresenter == null || this.ProcessWorkflowPresenter.SelectedRootWFNode == null)
                return false;

            if (acClassWFID == Guid.Empty)
            {
                this.ProcessWorkflowPresenter.SelectedWFNode = null;
                return false;
            }
            else
            {
                var node = this.FindPWNode(this.ProcessWorkflowPresenter.SelectedRootWFNode, acClassWFID);
                this.ProcessWorkflowPresenter.SelectedWFNode = node;
                return node != null;
            }
        }

        private IACComponentPWNode FindPWNode(IACComponentPWNode root, Guid key)
        {
            if (root.Content != null && ((gip.core.datamodel.ACClassWF)root.Content).ACClassWFID == key)
                return root;
            else
            {
                IACComponentPWNode result;

                foreach (IACComponentPWNode item in root.ACComponentChilds)
                {
                    result = this.FindPWNode(item, key);
                    if (result != null) return result;
                }

                return null;
            }
        }

        #region - Dialog

        public IEnumerable<gip.core.datamodel.ACClassMethod> NewProcessWorkflowList
        {
            get
            {
                IList<gip.core.datamodel.ACClassMethod> items = this.ProcessWorkflowList.Select(w => w.ACClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>()).ToArray();
                return this.Database.ContextIPlus.ACClassMethod.Where(w => w.ACKindIndex == (short)Global.ACKinds.MSWorkflow && !w.IsSubMethod).ToArray().Where(w => !items.Contains(w));
            }
        }

        public gip.core.datamodel.ACClassMethod NewProcessWorkflow
        {
            get;
            set;
        }

        [ACMethodCommand("NewProcessWorkflow", Const.Ok, (short)MISort.Okay)]
        public void NewProcessWorkflowOk()
        {
            MaterialWFACClassMethod item;

            item = MaterialWFACClassMethod.NewACObject(this.DatabaseApp, null);
            item.MaterialWF = this.CurrentMaterialWF;
            item.ACClassMethodID = this.NewProcessWorkflow.ACClassMethodID;
            //item = MaterialWFACClassMethod.CreateMaterialWFACClassMethod(Guid.NewGuid(), this.CurrentMaterialWF.MaterialWFID, this.NewProcessWorkflow.ACClassMethodID);

            ((gip.mes.datamodel.DatabaseApp)this.DatabaseApp).MaterialWFACClassMethod.Add(item);

            OnPropertyChanged("ProcessWorkflowList");
            this.CurrentProcessWorkflow = item;

            CloseTopDialog();
            this.NewProcessWorkflow = null;
        }

        public bool IsEnabledNewProcessWorkflowOk()
        {
            return this.NewProcessWorkflow != null;
        }

        [ACMethodCommand("NewProcessWorkflow", Const.Cancel, (short)MISort.Cancel)]
        public void NewProcessWorkflowCancel()
        {
            CloseTopDialog();
            this.NewProcessWorkflow = null;
        }

        private void RemoveProcessWorkflowConnections(MaterialWFACClassMethod item)
        {
            List<PartslistACClassMethod> plMethods = item.PartslistACClassMethod_MaterialWFACClassMethod.ToList();
            foreach (PartslistACClassMethod plMethod in plMethods)
                plMethod.DeleteACObject(DatabaseApp, false);

            List<ProdOrderBatchPlan> batchPlans = item.ProdOrderBatchPlan_MaterialWFACClassMethod.ToList();
            foreach (ProdOrderBatchPlan batchPlan in batchPlans)
                batchPlan.MaterialWFACClassMethod = null;

            var methodsToDelete = item.MaterialWFConnection_MaterialWFACClassMethod.ToList();
            foreach(var method in methodsToDelete)
            {
                method.DeleteACObject(DatabaseApp, false);
            }
        }

        #endregion

        /// <summary>
        /// Called from a WPF-Control inside it's ACAction-Method when a relevant interaction-event as occured (e.g. Drag-And-Drop).
        /// </summary>
        /// <param name="targetVBDataObject">The target object that was involved in the interaction event.</param>
        /// <param name="actionArgs">Information about the type of interaction and the source.</param>
        public override void ACActionToTarget(IACInteractiveObject targetVBDataObject, ACActionArgs actionArgs)
        {
            if (actionArgs.ElementAction == Global.ElementActionType.Drop)
            {
                gip.core.datamodel.ACClassWF relationWf = actionArgs.DropObject.ACContentList.OfType<gip.core.datamodel.ACClassWF>().FirstOrDefault();
                Material relationMaterial = targetVBDataObject.ACContentList.OfType<Material>().FirstOrDefault();

                if (relationWf != null
                    && relationMaterial != null
                    && !relationMaterial.MaterialWFConnection_Material.Where(w => w.MaterialWFACClassMethodID == this.CurrentProcessWorkflow.MaterialWFACClassMethodID && w.ACClassWFID == relationWf.ACClassWFID).Any())
                {
                    MaterialWFConnection entry = MaterialWFConnection.NewACObject(DatabaseApp, this.CurrentProcessWorkflow);
                    entry.Material = relationMaterial;
                    entry.ACClassWFID = relationWf.ACClassWFID;
                    //MaterialWFConnection entry = MaterialWFConnection.CreateMaterialWFConnection(Guid.NewGuid(),
                    //                                                                             relationMaterial.MaterialID, relationWf.ACClassWFID,
                    //                                                                             this.CurrentProcessWorkflow.MaterialWFACClassMethodID,
                    //                                                                             Root.Environment.User.Initials, DateTime.Now,
                    //                                                                             Root.Environment.User.Initials, DateTime.Now);

                    DatabaseApp.MaterialWFConnection.Add(entry);
                }
            }
            OnPropertyChanged("MaterialPWNodeConnectionList");
            base.ACActionToTarget(targetVBDataObject, actionArgs);
        }

        /// <summary>
        /// Called from a WPF-Control when a relevant interaction-event as occured (e.g. Drag-And-Drop) and the related component should check if this interaction-event should be handled.
        /// </summary>
        /// <param name="targetVBDataObject">The target object that was involved in the interaction event.</param>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        /// <returns><c>true</c> if ACAction can be invoked otherwise, <c>false</c>.</returns>
        public override bool IsEnabledACActionToTarget(IACInteractiveObject targetVBDataObject, ACActionArgs actionArgs)
        {
            if (this.CurrentProcessWorkflow != null)
            {
                gip.core.datamodel.ACClassWF relationWf = actionArgs.DropObject.ACContentList.OfType<gip.core.datamodel.ACClassWF>().FirstOrDefault();
                Material relationMaterial = targetVBDataObject.ACContentList.OfType<Material>().FirstOrDefault();

                // Connection is allowed only if 'relationMaterial' doesn't have any connections with current process workflow
                return relationWf != null
                    && relationMaterial != null
                    && !relationMaterial.MaterialWFConnection_Material.Where(w => w.MaterialWFACClassMethodID == this.CurrentProcessWorkflow.MaterialWFACClassMethodID && w.ACClassWFID == relationWf.ACClassWFID).Any();
            }
            else return false;
        }

        [ACMethodInteraction("Materials", "en{'Remove WF connection'}de{'Entferne Verbindung zu Steuerschritten'}", (short)MISort.Delete, false)]
        public void RemoveMaterialConnection()
        {
            if (!this.IsEnabledRemoveMaterialConnection())
                return;
            // Möchten Sie nur die Beziehung zum dem aktuell angezeigten Steuerschritt löschen? (Sonst werden alle Beziehungen zu dem ausgewählten Zwischenprodukt gelöscht)
            var result = Messages.YesNoCancel(this, "Question50029", Global.MsgResult.Yes, false);
            if (result == Global.MsgResult.Cancel)
                return;


            if (result == Global.MsgResult.Yes
                && this.ProcessWorkflowPresenter != null
                && this.ProcessWorkflowPresenter.SelectedWFNode != null
                && this.ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF != null)
            {
                var conn = this.SelectedMaterial.MaterialWFConnection_Material.Where(m => m.MaterialWFACClassMethodID == this.CurrentProcessWorkflow.MaterialWFACClassMethodID
                                                                              && m.ACClassWFID == this.ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF.ACClassWFID)
                                                                              .FirstOrDefault();
                if (conn != null)
                    conn.DeleteACObject(DatabaseApp, false);
            }
            else if (result == Global.MsgResult.No)
            {
                foreach (var conn in this.SelectedMaterial.MaterialWFConnection_Material.Where(m => m.MaterialWFACClassMethodID == this.CurrentProcessWorkflow.MaterialWFACClassMethodID).ToArray())
                {
                    conn.DeleteACObject(DatabaseApp, false);
                }
            }
            SelectProcessWorkflowNode();
            OnPropertyChanged("MaterialPWNodeConnectionList");
        }

        public bool IsEnabledRemoveMaterialConnection()
        {
            return this.SelectedMaterial != null
                && this.CurrentProcessWorkflow != null
                && this.SelectedMaterial.MaterialWFConnection_Material.Where(m => m.MaterialWFACClassMethodID == this.CurrentProcessWorkflow.MaterialWFACClassMethodID).Any();
        }


        #endregion

        #region IACBSOConfigStoreSelection
        public List<IACConfigStore> MandatoryConfigStores
        {
            get
            {
                List<IACConfigStore> listOfSelectedStores = new List<IACConfigStore>();
                if (CurrentProcessWorkflow != null)
                {
                    listOfSelectedStores.Add(CurrentProcessWorkflow);
                }
                return listOfSelectedStores;
            }
        }

        public IACConfigStore CurrentConfigStore
        {
            get
            {
                if (CurrentProcessWorkflow == null) return null;
                return CurrentProcessWorkflow;
            }
        }

        public bool IsReadonly
        {
            get
            {
                return false;
            }
        }

        private List<core.datamodel.ACClassMethod> _VisitedMethods;
        public List<core.datamodel.ACClassMethod> VisitedMethods
        {
            get
            {
                if (_VisitedMethods == null)
                    _VisitedMethods = new List<core.datamodel.ACClassMethod>();
                return _VisitedMethods;
            }
            set
            {
                _VisitedMethods = value;
                OnPropertyChanged("VisitedMethods");
            }
        }
        public void AddVisitedMethods(core.datamodel.ACClassMethod acClassMethod)
        {
            if (!VisitedMethods.Contains(acClassMethod))
                VisitedMethods.Add(acClassMethod);
        }

        #endregion

        #region Layout

        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            switch (vbControl.VBContent)
            {
                case nameof(CurrentMaterialWFNo):
                    {
                        if (VBDesignerMaterialWF == null || !VBDesignerMaterialWF.IsDesignMode)
                            return Global.ControlModes.Disabled;
                        break;
                    }
            }

            return result;
        }

        private VBPresenterMaterialWF _MaterialWFPresenter;
        bool _MatPresenterRightsChecked = false;
        public VBPresenterMaterialWF MaterialWFPresenter
        {
            get
            {
                if (_MaterialWFPresenter == null)
                {
                    _MaterialWFPresenter = this.ACUrlCommand("VBPresenterMaterialWF(CurrentDesign)") as VBPresenterMaterialWF;
                    if (_MaterialWFPresenter == null && !_MatPresenterRightsChecked)
                        Messages.Error(this, "This user has no rights for viewing workflows. Assign rights for VBPresenterMaterialWF in the group management!", true);
                    _MatPresenterRightsChecked = true;
                }
                return _MaterialWFPresenter;
            }
        }

        public string CurrentMaterialWFLayout
        {
            get
            {
                if (MaterialWFPresenter == null || ProcessWorkflowPresenter == null)
                {
                    return null;
                }
                string layoutXAML = LayoutHelper.DockingManagerBegin("tabControl1", "Padding=\"0\"");
                layoutXAML += LayoutHelper.DockingManagerAdd("*TabMaterialWF", "TabMaterialWF_0");
                layoutXAML += LayoutHelper.DockingManagerEnd();
                return layoutXAML;
            }
        }

        VBDesignerMaterialWF _VBDesignerMaterialWF;
        public VBDesignerMaterialWF VBDesignerMaterialWF
        {
            get
            {
                if (MaterialWFPresenter != null)
                {
                    var vbDesignerMaterialWF = MaterialWFPresenter.ACUrlCommand("VBDesignerMaterialWF(CurrentDesign)") as VBDesignerMaterialWF;
                    if (vbDesignerMaterialWF != _VBDesignerMaterialWF)
                    {
                        if (_VBDesignerMaterialWF != null)
                            _VBDesignerMaterialWF.PropertyChanged -= _VBDesignerMaterialWF_PropertyChanged;
                        _VBDesignerMaterialWF = vbDesignerMaterialWF;
                        if (_VBDesignerMaterialWF != null)
                            _VBDesignerMaterialWF.PropertyChanged += _VBDesignerMaterialWF_PropertyChanged;
                    }
                }
                return _VBDesignerMaterialWF;
            }
        }

        private void _VBDesignerMaterialWF_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e != null && e.PropertyName == "IsDesignMode")
            {
                OnPropertyChanged(nameof(CurrentMaterialWFNo));
            }
        }

        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(AddMaterialDlg):
                    AddMaterialDlg();
                    return true;
                case nameof(AddMaterialDlgCancel):
                    AddMaterialDlgCancel();
                    return true;
                case nameof(AddMaterialOK):
                    AddMaterialOK();
                    return true;
                case nameof(AddProcessWorkflow):
                    AddProcessWorkflow();
                    return true;
                case nameof(CloneMaterialWF):
                    CloneMaterialWF();
                    return true;
                case nameof(CloneMaterialWFOK):
                    CloneMaterialWFOK();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(DeleteMaterialWFRelation):
                    DeleteMaterialWFRelation();
                    return true;
                case nameof(IsEnabledACActionToTarget):
                    result = IsEnabledACActionToTarget((gip.core.datamodel.IACInteractiveObject)acParameter[0], (gip.core.datamodel.ACActionArgs)acParameter[1]);
                    return true;
                case nameof(IsEnabledAddMaterialDlg):
                    result = IsEnabledAddMaterialDlg();
                    return true;
                case nameof(IsEnabledAddMaterialOK):
                    result = IsEnabledAddMaterialOK();
                    return true;
                case nameof(IsEnabledAddProcessWorkflow):
                    result = IsEnabledAddProcessWorkflow();
                    return true;
                case nameof(IsEnabledCloneMaterialWF):
                    result = IsEnabledCloneMaterialWF();
                    return true;
                case nameof(IsEnabledCloneMaterialWFOK):
                    result = IsEnabledCloneMaterialWFOK();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(IsEnabledDeleteMaterialWFRelation):
                    result = IsEnabledDeleteMaterialWFRelation();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(IsEnabledNewMaterialWFRelation):
                    result = IsEnabledNewMaterialWFRelation();
                    return true;
                case nameof(IsEnabledNewProcessWorkflowOk):
                    result = IsEnabledNewProcessWorkflowOk();
                    return true;
                case nameof(IsEnabledRemoveMaterialConnection):
                    result = IsEnabledRemoveMaterialConnection();
                    return true;
                case nameof(IsEnabledRemoveProcessWorkflow):
                    result = IsEnabledRemoveProcessWorkflow();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(NewMaterialWFRelation):
                    NewMaterialWFRelation();
                    return true;
                case nameof(NewProcessWorkflowCancel):
                    NewProcessWorkflowCancel();
                    return true;
                case nameof(NewProcessWorkflowOk):
                    NewProcessWorkflowOk();
                    return true;
                case nameof(RemoveMaterialConnection):
                    RemoveMaterialConnection();
                    return true;
                case nameof(RemoveProcessWorkflow):
                    RemoveProcessWorkflow();
                    return true;
                case nameof(Save):
                    Save();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(SetSelectedMaterial):
                    SetSelectedMaterial((gip.mes.datamodel.Material)acParameter[0], acParameter.Count() == 2 ? (System.Boolean)acParameter[1] : false);
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region BackgroundWorker

        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();

            worker.ProgressInfo.OnlyTotalProgress = true;
            worker.ProgressInfo.AddSubTask(command, 0, 9);
            string message = Translator.GetTranslation("en{'Running {0}...'}de{'{0} läuft...'}");
            worker.ProgressInfo.ReportProgress(command, 0, string.Format(message, command));

            string updateName = Root.Environment.User.Initials;

            switch (command)
            {
                case nameof(DoCloneMaterialWF):
                    e.Result = DoCloneMaterialWF(DatabaseApp, CurrentMaterialWF, NewMaterialWFNo);
                    break;

            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = worker.EventArgs.Argument.ToString();
            ClearMessages();
            if (e.Cancelled)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Info, Message = string.Format(@"Operation {0} canceled by user!", command) });
            }
            if (e.Error != null)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format(@"Error by doing {0}! Message:{1}", command, e.Error.Message) });
            }
            else
            {
                switch (command)
                {
                    case nameof(DoCloneMaterialWF):
                        MsgWithDetails msgWithDetails = e.Result as MsgWithDetails;
                        if (msgWithDetails.IsSucceded())
                        {
                            MaterialWF newMaterialWF = DatabaseApp.MaterialWF.FirstOrDefault(m => m.MaterialWFNo == NewMaterialWFNo);
                            if (!MaterialWFList.Any(c => c.MaterialWFNo == NewMaterialWFNo))
                            {
                                AccessPrimary.NavList.Insert(0, newMaterialWF);
                                OnPropertyChanged(nameof(MaterialWFList));
                            }
                            SelectedMaterialWF = newMaterialWF;
                            NewMaterialWFNo = null;
                        }
                        else
                        {
                            Messages.Msg(msgWithDetails);
                        }
                        break;
                }
            }
        }

        #endregion

        #region BackgroundWorker -> DoMethods

        public static MsgWithDetails DoCloneMaterialWF(DatabaseApp databaseApp, MaterialWF materialWF, string newMaterialWFNo)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            try
            {
                MaterialWF newMaterialWF = MaterialWF.NewACObject(databaseApp, null, newMaterialWFNo);
                newMaterialWF.Name = materialWF.Name;
                newMaterialWF.XMLConfig = materialWF.XMLConfig;
                newMaterialWF.XMLDesign = VBDesignerMaterialWF.ChangeMaterialWFName(materialWF.XMLDesign, materialWF.MaterialWFNo, newMaterialWFNo);

                MaterialWFRelation[] relations = materialWF.MaterialWFRelation_MaterialWF.ToArray();
                foreach(MaterialWFRelation relation in relations)
                {
                    MaterialWFRelation newRelation = MaterialWFRelation.NewACObject(databaseApp, null);
                    newRelation.MaterialWF = newMaterialWF;
                    newRelation.Sequence = relation.Sequence;
                    newRelation.TargetMaterial = relation.TargetMaterial;
                    newRelation.SourceMaterial = relation.SourceMaterial;
                    databaseApp.MaterialWFRelation.Add(newRelation);
                }

                MaterialWFACClassMethod[] mwMethods = materialWF.MaterialWFACClassMethod_MaterialWF.ToArray();
                foreach (MaterialWFACClassMethod mwMethod in mwMethods)
                {
                    MaterialWFACClassMethod newMwMethod = MaterialWFACClassMethod.NewACObject(databaseApp, null);
                    newMwMethod.MaterialWF = newMaterialWF;
                    newMwMethod.ACClassMethodID = mwMethod.ACClassMethodID;
                    newMwMethod.IsDefault = mwMethod.IsDefault;
                    databaseApp.MaterialWFACClassMethod.Add(newMwMethod);

                    CloneMethodConfig(databaseApp, mwMethod, newMwMethod);
                    CloneMethodConnections(databaseApp, mwMethod, newMwMethod);
                }

                MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
                if(saveMsg != null)
                {
                    msgWithDetails.AddDetailMessage(saveMsg);
                }
            }
            catch (Exception ex)
            {
                databaseApp.ACUndoChanges();
                Msg exMsg = new Msg()
                {
                    MessageLevel = eMsgLevel.Error,
                    Message = string.Format("Error by cloning MaterialWF {0} to {1}! Message: {2}", materialWF.MaterialWFNo, newMaterialWFNo, ex.Message)
                };
                msgWithDetails.AddDetailMessage(exMsg);
            }

            return msgWithDetails;
        }

        private static void CloneMethodConfig(DatabaseApp databaseApp, MaterialWFACClassMethod mwMethod, MaterialWFACClassMethod newMwMethod)
        {
            MaterialWFACClassMethodConfig[] configs = mwMethod.MaterialWFACClassMethodConfig_MaterialWFACClassMethod.ToArray();
            Dictionary<Guid, Guid> configMap = new Dictionary<Guid, Guid>();
            List<MaterialWFACClassMethodConfig> newConfigs = new List<MaterialWFACClassMethodConfig>();
            foreach (MaterialWFACClassMethodConfig config in configs)
            {
                MaterialWFACClassMethodConfig newConfig = MaterialWFACClassMethodConfig.NewACObject(databaseApp, null);
                configMap.Add(config.MaterialWFACClassMethodConfigID, newConfig.MaterialWFACClassMethodConfigID);
                newConfigs.Add(newConfig);
                newConfig.MaterialWFACClassMethod = newMwMethod;
                newConfig.VBiACClassID = config.VBiACClassID;
                newConfig.VBiACClassPropertyRelationID = config.VBiACClassPropertyRelationID;
                newConfig.VBiACClassPropertyRelationID = config.VBiACClassPropertyRelationID;
                newConfig.VBiValueTypeACClassID = config.VBiValueTypeACClassID;
                newConfig.PreConfigACUrl = config.PreConfigACUrl;
                newConfig.LocalConfigACUrl = config.LocalConfigACUrl;
                newConfig.Expression = config.Expression;
                newConfig.Comment = config.Comment;
                newConfig.XMLConfig = config.XMLConfig;
                newConfig.VBiACClassWFID = config.VBiACClassWFID;
                databaseApp.MaterialWFACClassMethodConfig.Add(newConfig);
            }

            foreach (MaterialWFACClassMethodConfig config in configs)
            {
                if (config.ParentMaterialWFACClassMethodConfigID != null)
                {
                    KeyValuePair<Guid, Guid> pair = configMap.FirstOrDefault(p => p.Key == config.ParentMaterialWFACClassMethodConfigID.Value);
                    if (pair.Key != Guid.Empty && pair.Value != Guid.Empty)
                    {
                        MaterialWFACClassMethodConfig newConfig = newConfigs.FirstOrDefault(c => c.MaterialWFACClassMethodConfigID == pair.Value);
                        if (newConfig != null)
                        {
                            newConfig.ParentMaterialWFACClassMethodConfigID = pair.Value;
                        }
                    }
                }
            }
        }

        private static void CloneMethodConnections(DatabaseApp databaseApp, MaterialWFACClassMethod mwMethod, MaterialWFACClassMethod newMwMethod)
        {
            MaterialWFConnection[] matConnections = mwMethod.MaterialWFConnection_MaterialWFACClassMethod.ToArray();
            foreach (MaterialWFConnection matConnection in matConnections)
            {
                MaterialWFConnection newMatConnection = MaterialWFConnection.NewACObject(databaseApp, newMwMethod);
                newMatConnection.Material = matConnection.Material;
                newMatConnection.ACClassWFID = matConnection.ACClassWFID;
                newMatConnection.MaterialWFACClassMethod = newMwMethod;
                databaseApp.MaterialWFConnection.Add(newMatConnection);
            }
        }

        #endregion

        #region Message

        public void SendMessage(object result)
        {
            Msg msg = result as Msg;
            if (msg != null)
            {
                Messages.Msg(msg);
                SendMessage(msg);
            }
        }

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(528, "Message", "en{'Message'}de{'Meldung'}")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets the MSG list.
        /// </summary>
        /// <value>The MSG list.</value>
        [ACPropertyList(529, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged();
        }

        public void ClearMessages()
        {
            MsgList.Clear();
            OnPropertyChanged(nameof(MsgList));
        }

        #endregion

    }
}