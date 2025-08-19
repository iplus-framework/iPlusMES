// ***********************************************************************
// Assembly         : gip.bso.iplus
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 01-15-2013
// ***********************************************************************
// <copyright file="BSOPartsList.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.manager;
using gip.core.reporthandler.Configuration;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using static gip.core.datamodel.Global;

namespace gip.bso.masterdata
{
    /// <summary>
    ///Businessobject or App for managing the bill of material (BOM) in the system.
    /// This class extends the <see cref="BSOPartslistExplorer"/> and provides methods for creating, deleting, and managing bill of materials.
    /// To search records enter the search string in the SearchWord property.
    /// The database result is copied to the PartlistList property.
    /// Then call NavigateFirst() method to set CurrentParstlist with the first record in the list.
    /// CurrentPartslist is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentPartslist and when all field values ​​have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentPartslist property.
    /// Enter the name of the bill of material in the CurrentPartslist.Name property.
    /// The material which will be produced with this bill of material should be assigned to the CurrentPartslist.Material property.
    /// The default production quantity must be assigned to the CurrentPartslist.TargetQuantityUOM property.
    /// Optionally, the validity period can be set with the CurrentPartslist.ValidFrom and CurrentPartslist.ValidTo properties.
    /// Definition how materials are mixed together in the production process can be defined with MaterialWorkflow. 
    /// The MaterialWorkflow must be assigned to the CurrentPartslist.MaterialWF property. Then must be called the SetMaterialWF() method to assign the MaterialWorkflow to the CurrentPartslist.
    /// How to control production process is defined in the ProcessWorkflow. To assign the ProcessWorkflow to the CurrentPartslist, call the AddProcessWorkflow() method.
    /// Then select the ProcessWorkflow in the NewProcessWorkflowList property and call the NewProcessWorkflowOk method to assign the ProcessWorkflow to the CurrentPartslist.
    /// Now the Save() method must be called to save the changes in the database. If comes warning message you can ignore it with the Yes button.
    /// To define materials which are used in the production process we use the PartslistPosList property.
    /// Example how to add a material: First create a new PartslistPos with the NewPartslistPos() method which adds it to the PartlistPosList list, then assign the material to the SelectedPartslistPos.Material property.
    /// Then enter the needed quantity of the material in the PartslistPos.TargetQuantityUOM property and call Save() method to save the changes in the database. If comes warning message you can ignore it with the Yes button.
    /// On this way you can add as many materials as needed to the PartslistPosList.
    /// The IntermediateList property contains the list of intermediate products from the material workflow. Intermediate products are connected with the workflow nodes from the ProcessWorkflow.
    /// The materials from the PartslistPosList are assigned to the intermediate products. With this assignment we tell the system on which step of the production process the material will be used.
    /// One material from the PartslistPosList can be used in multiple intermediate products.
    /// To assign the material to the intermediate product, select the intermediate product in the SelectedIntermediate property and then call NewIntermediateParts() method.
    /// NewIntermediateParts() method creates a new assignemnt and adds it to the SelectedIntermediateParts property, also automatically fills the list IntermediatePartsList where are all assigned materials to the intermediate product.
    /// Now we need selected PartslistPos from the PartslistPosList and assign it to the SelectedIntermediateParts.SourcePartslistPos property.
    /// With the sequence number (property SelectedIntermediateParts.Sequence) we can define on which position the material will be used in the production process.
    /// The property SelectedIntermediateParts.TargetQuantityUOM defines the quantity of the material which will be used in the step of the production process.
    /// Now the Save() method must be called to save the changes in the database. If comes warning message you can ignore it with the Yes button.
    /// The description of the bill of material can be entered in the CurrentPartslist.XMLConfig property.
    /// With the CurrentPartslist.Enabled property we can enable or disable the bill of material.
    /// </summary>


    [ACClassConstructorInfo(
        new object[]
        {
            new object[] {"AutoFilter", Global.ParamOption.Optional, typeof(String)},
        }
    )]
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Bill of Materials'}de{'Stückliste'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Partslist.ClassName)]
    [ACQueryInfo(Const.PackName_VarioMaterial, Const.QueryPrefix + "PartsParamCopy", "en{'Copy Bill of Materials param'}de{'Kopiere Stücklistenparameter'}", typeof(Partslist), Partslist.ClassName, "PartslistName,IsEnabled", "PartslistNo")]
    public class BSOPartslist : BSOPartslistExplorer, IACBSOConfigStoreSelection
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
        public BSOPartslist(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        /// <summary>
        /// Initialize the bill of material business object.
        /// Add references to the PartslistManager, FacilityManager, and ProdOrderManager services.
        /// Run the Search method to load the initial data.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode)) return false;
            TempReportData = new ReportData();
            _PartslistManager = ACPartslistManager.ACRefToServiceInstance(this);
            if (_PartslistManager == null)
                throw new Exception("PartslistManager not configured");

            _FacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_FacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");

            Search();
            return true;
        }

        /// <summary>
        /// De-initializes the bill of material business object.
        /// Detaches the references from the PartslistManager, FacilityManager, and ProdOrderManager services.
        /// Set private fields to null.
        /// </summary>
        /// <param name="deleteACClassTask"></param>
        /// <returns></returns>
        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessConfigurationTransfer != null)
                _AccessConfigurationTransfer.NavSearchExecuting -= _AccessConfigurationTransfer_NavSearchExecuting;

            if (_FacilityManager != null)
                FacilityManager.DetachACRefFromServiceInstance(this, _FacilityManager);
            _FacilityManager = null;

            if (_PartslistManager != null)
                ACPartslistManager.DetachACRefFromServiceInstance(this, _PartslistManager);
            _PartslistManager = null;

            if (_ProdOrderManager != null)
                ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;

            this._SelectedPartslistPos = null;
            this._AccessInputMaterial = null;
            this._AlternativeSelectedPartslistPos = null;
            this._presenter = null;
            this._ProcessWorkflow = null;
            this._SelectedIntermediate = null;
            this._SelectedIntermediateParts = null;
            this._selectedMaterialWF = null;
            this._SelectedPartslistPos = null;

            if (_AccessInputMaterial != null)
            {
                _AccessInputMaterial.ACDeInit(false);
                _AccessInputMaterial = null;
            }
            return b;
        }

        #endregion

        #region ChildBSO

        ACChildItem<BSOSourceSelectionRules> _BSOSourceSelectionRules_Child;
        /// <summary>
        /// Gets the child component BSOSourceSelectionRules representing the component for managing source selection rules.
        /// TODO
        /// </summary>
        [ACPropertyInfo(600)]
        [ACChildInfo("BSOSourceSelectionRules_Child", typeof(BSOSourceSelectionRules))]
        public ACChildItem<BSOSourceSelectionRules> BSOSourceSelectionRules_Child
        {
            get
            {
                if (_BSOSourceSelectionRules_Child == null)
                    _BSOSourceSelectionRules_Child = new ACChildItem<BSOSourceSelectionRules>(this, "BSOSourceSelectionRules_Child");
                return _BSOSourceSelectionRules_Child;
            }
        }

        ACChildItem<BSOPreferredParameters> _BSOPreferredParameters;
        /// <summary>
        /// Gets the child component representing the component for preferred parameters.
        /// TODO
        /// </summary>
        [ACPropertyInfo(603)]
        [ACChildInfo(nameof(BSOPreferredParameters_Child), typeof(BSOPreferredParameters))]
        public ACChildItem<BSOPreferredParameters> BSOPreferredParameters_Child
        {
            get
            {
                if (_BSOPreferredParameters == null)
                    _BSOPreferredParameters = new ACChildItem<BSOPreferredParameters>(this, nameof(BSOPreferredParameters_Child));
                return _BSOPreferredParameters;
            }
        }

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

        protected ACRef<ACComponent> _FacilityManager = null;

        private ReportData _TempReportData;
        /// <summary>
        /// Gets or sets the temporary report data used for processing or display purposes.
        /// </summary>
        [ACPropertyInfo(9999)]
        public ReportData TempReportData
        {
            get
            {
                return _TempReportData;
            }
            set
            {
                _TempReportData = value;
                OnPropertyChanged();
            }
        }

        protected ACRef<ACProdOrderManager> _ProdOrderManager = null;
        /// <summary>
        /// Gets the production order manager instance.
        /// </summary>
        public ACProdOrderManager ProdOrderManager
        {
            get
            {
                if (_ProdOrderManager == null)
                    return null;
                return _ProdOrderManager.ValueT;
            }
        }

        #endregion

        #region Partslist

        #region Partslist -> Methods

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Partslist.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description =
                         "Saves this instance.")]
        public void Save()
        {
            if (!PreExecute()) return;
            OnSave();
            PostExecute();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand("Partslist", "en{'Undo'}de{'Rückgängig'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description =
                         "Undoes the save.")]
        public void UndoSave()
        {
            if (!PreExecute()) return;
            OnUndoSave();

            ClearChangeTracking();

            Search(SelectedPartslist);
            PostExecute();
            this.VisitedMethods = null;
        }



        /// <summary>
        /// Executes pre-save logic for the current object, including validation, recalculations,  and processing of
        /// related data. This method is called before the object is saved.
        /// </summary>
        /// <remarks>This method performs several operations to ensure the object is in a valid state before saving: Clears any existing messages associated with the
        /// object.Updates the state of the ExcludeFromSumCalc +property.If a current parts list is present, checks for changes,
        /// recalculates remaining quantities, and validates the parts list. Validation messages, if any, are sent and returned as part of the result.Processes any changes in parts
        /// lists and visited methods.If any validation errors occur, they are returned as a message object, and the save operation should be aborted.</remarks>
        /// <returns>A Msg object containing validation errors or other messages. Returns null if the pre-save checks pass without issues.</returns>
        protected override Msg OnPreSave()
        {
            Msg result = base.OnPreSave();
            if (result != null)
            {
                return result;
            }

            ClearMessages();

            IsUpdatedExcludeFromSumCalc = UpdateExcludeFromSumCalc();

            if (CurrentPartslist != null)
            {
                //Damir to sasa: Caclulation doesn't work if:
                //    - TargetUOM-Quantity is set and MDUnit of Partslist is not set.
                //    - If TargetQuantity is Zero, then this function sets the TargetUOM-Quantity to zero.
                //MsgWithDetails calculationMessage = PartslistManager.CalculateUOMAndWeight(CurrentPartslist);

                bool anyChangeInCurrent = AnyChangeInCurrent();

                if (anyChangeInCurrent)
                {
                    PartslistManager.RecalcRemainingQuantity(CurrentPartslist);

                    MsgWithDetails validateCurrentPartslist = PartslistManager.Validate(CurrentPartslist);
                    if (validateCurrentPartslist != null && validateCurrentPartslist.MsgDetails.Any())
                    {
                        SendMessage(validateCurrentPartslist);
                        validateCurrentPartslist.Message = Root.Environment.TranslateText(this, "RecipeValidationMessages");
                        result = validateCurrentPartslist;
                    }
                }
            }

            ProcessChangedPartslistsAndVisitedMethods();

            return result;
        }

        private bool AnyChangeInCurrent()
        {
            return
                CurrentPartslist.EntityState != EntityState.Unchanged
                || CurrentPartslist.PartslistPos_Partslist.Select(c => c.EntityState).Where(c => c != EntityState.Unchanged).Any()
                || CurrentPartslist.PartslistPos_Partslist.SelectMany(c => c.PartslistPosRelation_TargetPartslistPos).Select(c => c.EntityState).Where(c => c != EntityState.Unchanged).Any();
        }


        public virtual void ProcessChangedPartslists()
        {

        }

        /// <summary>
        /// Performs post-save operations after the object has been persisted to the database.
        /// </summary>
        /// <remarks>This method executes a series of actions to ensure the object's state is updated and
        /// consistent after saving. It processes changes to bill of material, updates planning and maintenance orders,
        /// clears change tracking, and reloads configuration if necessary. Additionally, it raises property change
        /// notifications for specific properties if required.</remarks>
        protected override void OnPostSave()
        {
            ProcessChangedPartslists();
            UpdatePlanningMROrders();
            ClearChangeTracking();

            ConfigManagerIPlus.ReloadConfigOnServerIfChanged(this, VisitedMethods, this.Database);
            this.VisitedMethods = null;

            if (IsUpdatedExcludeFromSumCalc)
            {
                OnPropertyChanged(nameof(IntermediateList));
                if (SelectedIntermediate != null && SelectedIntermediate.Material.ExcludeFromSumCalc)
                {
                    OnPropertyChanged(nameof(IntermediatePartsList));
                    OnPropertyChanged(nameof(SelectedIntermediateParts));
                }
            }
            IsUpdatedExcludeFromSumCalc = false;

            base.OnPostSave();
        }

        private void ProcessChangedPartslistsAndVisitedMethods()
        {
            if (!ConfigManagerIPlus.MustConfigBeReloadedOnServer(this, VisitedMethods, this.Database))
            {
                this.VisitedMethods = null;
            }

            List<Partslist> changedPartslists = ProcessLastFormulaChange();
            foreach (Partslist changedPartslist in changedPartslists)
            {
                if (!ChangedPartslists.Contains(changedPartslist))
                    ChangedPartslists.Add(changedPartslist);
            }
        }


        protected override void OnPostUndoSave()
        {
            this.VisitedMethods = null;
            base.OnPostUndoSave();
        }


        /// <summary>
        /// Determines whether the "Save" operation is currently enabled.
        /// </summary>
        /// <remarks>This method delegates the determination of the "Save" operation's enabled state to
        /// the  <c>OnIsEnabledSave</c> method. Override <c>OnIsEnabledSave</c> in a derived class to customize  the
        /// behavior.</remarks>
        /// <returns><see langword="true"/> if the "Save" operation is enabled; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Determines whether the "Undo Save" operation is currently enabled.
        /// </summary>
        /// <returns><see langword="true"/> if the "Undo Save" operation is enabled; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        /// <summary>
        /// Creates a new instance of a bill of material (Partslist), assigns it a unique identifier, and adds it to the database and
        /// navigation list. This method initializes a new partslist object, assigns it a unique identifier using
        /// the database's numbering system,  and adds it to the application's parts list collection and navigation
        /// list. It also updates the current and selected  parts list references. The method ensures that pre- and
        /// post-execution logic is applied and temporarily disables  loading during the operation.</summary>
        [ACMethodInteraction("Partslist", Const.New, (short)MISort.New, true, "SelectedPartslist", Global.ACKinds.MSMethodPrePost, Description =
                             "Creates a new instance of a bill of material (Partslist), assigns it a unique identifier, and adds it to the database and " +
                             "navigation list. This method initializes a new partslist object, assigns it a unique identifier using " +
                             "the database's numbering system,  and adds it to the application's parts list collection and navigation list. " +
                             "It also updates the current and selected  parts list references. The method ensures that pre- and " +
                             "post-execution logic is applied and temporarily disables  loading during the operation.")]
        public void New()
        {
            if (!PreExecute()) return;
            IsLoadDisabled = true;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Partslist), Partslist.NoColumnName, Partslist.FormatNewNo, this);
            Partslist partsList = Partslist.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.Partslist.AddObject(partsList);
            AccessPrimary.NavList.Insert(0, partsList);
            SelectedPartslist = partsList;
            CurrentPartslist = partsList;
            OnPropertyChanged(nameof(PartslistList));
            IsLoadDisabled = false;
            PostExecute();
        }

        /// <summary>
        /// Creates a new version of the selected bill of material (Partslist) and updates the current state accordingly.
        /// This method generates a new version of the currently selected parts list, assigns it
        /// a unique identifier, and adds it to the navigation list. The new version is based on the existing parts
        /// list and is initialized  with the appropriate data. After the operation, the current parts list is updated
        /// to the newly created version, and the relevant UI bindings are refreshed.</summary>
        [ACMethodInteraction("Partslist", "en{'New BOM version'}de{'Neue Rezeptversion'}", (short)MISort.New, true, "SelectedPartslist", Global.ACKinds.MSMethodPrePost, Description =
                              "Creates a new version of the selected bill of material (Partslist) and updates the current state accordingly." +
                              "This method generates a new version of the currently selected parts list, assigns it " +
                              "a unique identifier, and adds it to the navigation list. The new version is based on the existing parts " +
                              "list and is initialized  with the appropriate data. After the operation, the current parts list is updated " +
                              "to the newly created version, and the relevant UI bindings are refreshed.")]
        public void NewVersion()
        {
            if (!PreExecute()) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Partslist), Partslist.NoColumnName, Partslist.FormatNewNo, this);
            Partslist partslistNewVersion = Partslist.NewACObject(DatabaseApp, null, secondaryKey);
            partslistNewVersion = Partslist.PartsListNewVersionGet(DatabaseApp, SelectedPartslist, partslistNewVersion);
            AccessPrimary.NavList.Add(partslistNewVersion);
            Load(true);
            CurrentPartslist = partslistNewVersion;
            OnPropertyChanged(nameof(PartslistList));
            PostExecute();
        }


        #region Methods  -> delete (soft) implementation
        /// <summary>
        /// Performs a delete operation on the current bill of material (Partslist), supporting both soft delete and hard delete
        /// scenarios. This method first checks preconditions using PreExecute and IsEnabledDelete. If the current parts list has a non-null delete date, a soft delete confirmation
        /// dialog is shown. Otherwise, a hard delete operation is performed by invoking OnDelete(bool with true parameter. 
        /// After the operation, PostExecute is called to finalize the process.</summary>
        [ACMethodInteraction(Partslist.ClassName, Const.Delete, (short)MISort.Delete, true, "SelectedPartslist", Global.ACKinds.MSMethodPrePost, Description =
                             "Performs a delete operation on the current bill of material (Partslist), supporting both soft delete and hard delete " +
                             "scenarios. This method first checks preconditions using PreExecute and IsEnabledDelete. If the current parts list has a non-null delete date, a soft delete confirmation " +
                             "dialog is shown. Otherwise, a hard delete operation is performed by invoking OnDelete(bool with true parameter. " +
                             "After the operation, PostExecute is called to finalize the process.")]
        public void Delete()
        {
            if (!PreExecute())
                return;
            if (!IsEnabledDelete())
                return;
            if (CurrentPartslist.DeleteDate != null)
                ShowDialog(this, ACBSONav.CDialogSoftDelete);
            else
                OnDelete(true);
            PostExecute();
        }

        /// <summary>
        /// Handles the deletion of the current parts list, either as a soft delete or a permanent delete.
        /// This method performs the necessary checks and operations to delete the current parts
        /// list.  If the parts list is used in a production order, an error message is displayed, and the deletion is
        /// aborted.  Otherwise, the user is prompted to confirm the deletion. If confirmed, the parts list is deleted, 
        /// and the application state is updated accordingly.  After a successful deletion, the method updates the
        /// navigation list, selects the next available parts list,  and reloads the data. The
        /// PartslistList property is also updated to reflect the changes.</summary>
        /// <param name="softDelete">A boolean value indicating whether the deletion should be a soft delete.  If true, the
        /// parts list is marked as deleted but not permanently removed;  otherwise, the parts list is permanently
        /// deleted.</param>
        public override void OnDelete(bool softDelete)
        {
            Msg msg = SelectedPartslist.DeleteACObject(DatabaseApp, true, softDelete);
            if (msg != null)
            {
                bool partslistUsedInProductionOrder = CurrentPartslist.ProdOrderPartslist_Partslist.Any();
                if (partslistUsedInProductionOrder)
                {
                    msg = new Msg() { Message = Root.Environment.TranslateMessage(this, "Error50045"), MessageLevel = eMsgLevel.Error };
                    Messages.Msg(msg);
                }
                else
                {
                    Global.MsgResult result = Messages.Msg(msg, Global.MsgResult.No, eMsgButton.YesNo);
                    if (result == Global.MsgResult.Yes)
                    {
                        msg = ACPartslistManager.PartslistDelete(DatabaseApp, CurrentPartslist);
                        if (msg != null)
                        {
                            Messages.Msg(msg);
                        }
                    }
                }
            }
            if (msg == null)
            {
                IsLoadDisabled = true;
                if (AccessPrimary == null)
                    return;
                AccessPrimary.NavList.Remove(CurrentPartslist);
                SelectedPartslist = AccessPrimary.NavList.FirstOrDefault();
                CurrentPartslist = SelectedPartslist;
                IsLoadDisabled = false;
                Load();
                OnPropertyChanged(nameof(PartslistList));
            }
        }

        /// <summary>
        /// Determines whether the delete operation is enabled based on the current state.
        /// </summary>
        /// <remarks>The delete operation is enabled if <c>CurrentPartslist</c> is not <see
        /// langword="null"/>.</remarks>
        /// <returns><see langword="true"/> if the delete operation is enabled; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentPartslist != null;
        }

        /// <summary>
        /// Restores the state of the object to its previous configuration.
        ///This method triggers the restoration process, which reverts the object to a prior
        /// state.  It is typically used to undo changes or return to a default configuration.</summary>
        [ACMethodCommand("Restore", "en{'Restore'}de{'Wiederherstellen'}", (short)MISort.Restore, true, Description =
                         "Restores the state of the object to its previous configuration. " +
                         "This method triggers the restoration process, which reverts the object to a prior " +
                         "state.  It is typically used to undo changes or return to a default configuration.")]
        public void Restore()
        {
            OnRestore();
        }

        /// <summary>
        /// Determines whether the restore operation is enabled.
        /// </summary>
        /// <returns><see langword="true"/> if the restore operation is enabled; otherwise, <see langword="false"/>.</returns>
        public override bool IsEnabledRestore()
        {
            return base.IsEnabledRestore();
        }

        /// <summary>
        /// Restores the state of the application and updates relevant data.
        /// This method restores the application state by saving any pending changes,  refreshing
        /// the search results, and notifying that the PartslistList  property has changed. It is
        /// typically called after a restore operation to ensure  the application is in a consistent state.</summary>
        public override void OnRestore()
        {
            base.OnRestore();
            DatabaseApp.ACSaveChanges();
            Search();
            OnPropertyChanged(nameof(PartslistList));
        }

        #endregion


        /// <summary>
        /// Handles changes to the selected parts list and updates related properties and workflows.
        /// </summary>
        /// <remarks>This method performs several updates when the selected parts list changes, including:
        /// - Searching for positions and intermediate data. - Loading process and material workflows. - Updating
        /// dependent properties such as production units, material units, and configuration transfers. If the new parts
        /// list contains valid material and production unit data, the current production MD unit is updated. Otherwise,
        /// it is set to <see langword="null"/>.</remarks>
        /// <param name="partsList">The newly selected <see cref="Partslist"/> instance.</param>
        /// <param name="prevPartslist">The previously selected <see cref="Partslist"/> instance.</param>
        /// <param name="name">The name of the property that triggered the change. Defaults to the caller member name.</param>
        public override void OnPartslistSelectionChanged(Partslist partsList, Partslist prevPartslist, [CallerMemberName] string name = "")
        {
            if (name == nameof(CurrentPartslist) && prevPartslist != partsList)
            {
                SearchPos();
                SearchIntermediate();
                LoadProcessWorkflows();
                LoadMaterialWorkflows();

                OnPropertyChanged(nameof(ProdUnitMDUnitList));
                OnPropertyChanged(nameof(MaterialUnitList));
                OnPropertyChanged(nameof(MDUnitList));
                OnPropertyChanged(nameof(CurrentMDUnit));
                OnPropertyChanged(nameof(ConfigurationTransferList));
                if (CurrentPartslist != null
                    && CurrentPartslist.Material != null
                    && CurrentPartslist.ProductionUnits.HasValue
                    && CurrentPartslist.Material.MaterialUnit_Material.Any())
                    CurrentProdMDUnit = CurrentPartslist.Material.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).FirstOrDefault().ToMDUnit;
                else
                    CurrentProdMDUnit = null;
            }
            SelectedMaterialWF = null;
        }

        #region Partslist -> Methods -> IsEnabled

        /// <summary>
        /// Determines whether the new bill of material (Partslist) operation is enabled.
        /// </summary>
        /// <remarks>The application is considered enabled if there are no unsaved changes in the
        /// database.</remarks>
        /// <returns><see langword="true"/> if the application is enabled; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledNew()
        {
            return !DatabaseApp.IsChanged;
        }

        /// <summary>
        /// Determines whether the new version feature is enabled based on the current state of the selected parts list.
        /// </summary>
        /// <remarks>This method checks the state of the <c>SelectedPartslist</c> to determine if it is
        /// non-null and contains any parts.</remarks>
        /// <returns><see langword="true"/> if a parts list is selected and contains at least one part; otherwise, <see
        /// langword="false"/>.</returns>
        public bool IsEnabledNewVersion()
        {
            return SelectedPartslist != null && SelectedPartslist.PartslistPos_Partslist.Any();
        }

        #endregion

        #region Partslist-> Methods -> ProcessLastFormulaChange && Update PlanningMR Orders

        private List<Partslist> ProcessLastFormulaChange()
        {
            List<Partslist> changedPartslist = new List<Partslist>();
            foreach (Partslist partslist in VisitedPartslists)
            {
                if (partslist.EntityState != EntityState.Deleted)
                {
                    bool isChanged = PartslistManager.IsFormulaChanged(DatabaseApp, partslist);
                    if (isChanged)
                    {
                        partslist.LastFormulaChange = DateTime.Now;
                        changedPartslist.Add(partslist);
                    }
                }
            }
            return changedPartslist;
        }

        private void UpdatePlanningMROrders()
        {
            List<Partslist> partslistsforUpdatePlanningMR = GetPlForUpdatePlanningMROrder();
            if (partslistsforUpdatePlanningMR != null && partslistsforUpdatePlanningMR.Any())
            {
                UpdatePlanningMROrders(partslistsforUpdatePlanningMR);
            }
        }

        private void ClearChangeTracking()
        {
            ChangedPartslists.Clear();
            VisitedPartslists.Clear();
            if (CurrentPartslist != null)
                VisitedPartslists.Add(CurrentPartslist);
        }


        private List<Partslist> GetPlForUpdatePlanningMROrder()
        {
            List<Partslist> partslists = new List<Partslist>();
            if (ChangedPartslists.Any())
            {
                List<Partslist> changedPLConnectedWithTemplate =
                    ChangedPartslists
                    .Where(c => c.ProdOrderPartslist_Partslist.Any(x => x.PlanningMRProposal_ProdOrderPartslist.Any(y => y.ProdOrderPartslist.LastFormulaChange < c.LastFormulaChange)))
                    .ToList();
                if (changedPLConnectedWithTemplate != null && changedPLConnectedWithTemplate.Any())
                {
                    string changedPlartslistNo = string.Join(",", changedPLConnectedWithTemplate.Select(c => c.PartslistNo).Distinct().OrderBy(c => c));
                    MsgResult msgResult = Root.Messages.Question(this, "Question50065", MsgResult.Yes, false, changedPlartslistNo);
                    if (msgResult == MsgResult.Yes)
                    {
                        partslists = changedPLConnectedWithTemplate;
                    }
                }
            }
            return partslists;
        }

        private void UpdatePlanningMROrders(List<Partslist> partslists)
        {
            foreach (Partslist partslist in partslists)
            {
                List<ProdOrderPartslist> relatedTemplateProdPl =
                    partslist
                    .ProdOrderPartslist_Partslist
                    .Where(c => c.PlanningMRProposal_ProdOrderPartslist.Any())
                    .ToList();

                foreach (ProdOrderPartslist prodOrderPartslist in relatedTemplateProdPl)
                    ProdOrderManager.RefreshScheduledTemplateOrders(DatabaseApp, partslist, prodOrderPartslist);
            }
        }

        private void WriteLastFormulaChangeByDeleteElement(Partslist partslist)
        {
            if (!ChangedPartslists.Contains(partslist))
                ChangedPartslists.Add(partslist);
            partslist.LastFormulaChange = DateTime.Now;
        }

        #endregion

        #endregion

        #region Partslist -> Search (Partslist)



        // TODO: @aagincic: _loadInProgress enable Delete method to woring, but Load behavior shuld be analised and behaviors good applyed
        private bool IsLoadDisabled = false;

        /// <summary>
        /// Loads the selected parts list and its related data from the database.
        /// This method retrieves the selected parts list and its associated entities, such as
        /// materials, material units, and parts list positions, from the database. If parameter requery is
        /// true, the method also refreshes and reloads related data for the current parts list.  The
        /// method ensures that loading is performed only if it is not disabled and executes pre- and post-processing
        /// logic as defined by the application. It raises property change notifications for dependent properties to
        /// update the UI or other bindings.</summary>
        /// <param name="requery">A boolean value indicating whether to reload related data for the current parts list. If true, related data is reloaded; otherwise, it is not.</param>
        [ACMethodInteraction(Partslist.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedPartslist", Global.ACKinds.MSMethodPrePost, Description =
                             "Loads the selected parts list and its related data from the database. " +
                             "This method retrieves the selected parts list and its associated entities, such as " +
                             "materials, material units, and parts list positions, from the database. If parameter requery is " +
                             "true, the method also refreshes and reloads related data for the current parts list. The " +
                             "method ensures that loading is performed only if it is not disabled and executes pre- and post-processing " +
                             "logic as defined by the application. It raises property change notifications for dependent properties to " +
                             "update the UI or other bindings.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute())
                return;
            if (IsLoadDisabled)
                return;
            IsLoadDisabled = true;

            LoadEntity<Partslist>(requery, () => SelectedPartslist, () => CurrentPartslist, c => CurrentPartslist = c,
                        DatabaseApp.Partslist
                        .Include(c => c.Material)
                        .Include(c => c.Material.BaseMDUnit)
                        .Include(c => c.MaterialWF)
                        .Include("Material.MaterialUnit_Material")
                        //.Include(c => c.Material.MDUnitList)
                        .Include(c => c.PartslistPos_Partslist)
                        .Include(c => c.MDUnit)
                        .Include(x => x.PartslistPos_Partslist)
                        .Where(c => c.PartslistID == SelectedPartslist.PartslistID));
            if (requery && CurrentPartslist != null)
            {
                CurrentPartslist.PartslistPos_Partslist.AutoLoad();
                CurrentPartslist.PartslistPos_Partslist.AutoRefresh();
                foreach (var item in CurrentPartslist.PartslistPos_Partslist)
                    item.PartslistPosRelation_TargetPartslistPos.AutoLoad(this.DatabaseApp);
            }

            PostExecute();
            OnPropertyChanged(nameof(PartslistPosList));
            OnPropertyChanged(nameof(MaterialWFList));
            IsLoadDisabled = false;
        }

        /// <summary>
        /// Use for refresh all list tey are needed to be refreshed
        /// </summary>
        private void Refresh()
        {
            // OnPropertyChanged("InputMaterials");
            OnPropertyChanged(Material.ClassName);
            // RefreshPos();
            // RefreshIntermediate();
        }

        #endregion

        #endregion

        #region Partslistpos

        #region Partslistpos -> Select, (Current,) List

        private PartslistPos _SelectedPartslistPos;
        /// <summary>
        /// Gets or sets the currently selected parts list position.
        /// Changing this property triggers updates to dependent properties 
        /// and invokes the OnPropertyChanged method for relevant property names.</summary>
        [ACPropertySelected(9999, "PartslistPos", Description = 
                            "Gets or sets the currently selected parts list position. " +
                            "Changing this property triggers updates to dependent properties " +
                            "and invokes the OnPropertyChanged method for relevant property names.")]
        public PartslistPos SelectedPartslistPos
        {
            get
            {
                return _SelectedPartslistPos;
            }
            set
            {
                if (_SelectedPartslistPos != value)
                {
                    if (_SelectedPartslistPos != null)
                        _SelectedPartslistPos.PropertyChanged -= _SelectedPartslistPos_PropertyChanged;
                    _SelectedPartslistPos = value;
                    if (_SelectedPartslistPos != null)
                        _SelectedPartslistPos.PropertyChanged += _SelectedPartslistPos_PropertyChanged;
                    SearchAlternative();
                    if (value != null)
                        SelectedInputMaterial = value.Material;
                    else
                        SelectedInputMaterial = null;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedPartslistQueryForPartslistpos));
                    OnPropertyChanged(nameof(PartslistQueryForPartslistposList));
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/> event for the
        /// selected parts list position.
        /// </summary>
        /// <remarks>This method listens for changes to the <see cref="PartslistPos.MaterialID"/> property
        /// and raises a property change notification for the <see cref="SelectedPartslistPos"/> property when the
        /// material ID changes.</remarks>
        /// <param name="sender">The source of the event, typically the object whose property has changed.</param>
        /// <param name="e">The event data containing the name of the property that changed.</param>
        public virtual void _SelectedPartslistPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PartslistPos.MaterialID))
            {
                OnPropertyChanged(nameof(SelectedPartslistPos));
            }
        }

        /// <summary>
        /// Gets a collection of parts list positions associated with the selected parts list.
        /// The returned collection includes only positions of type OutwardRoot with no
        /// alternative parts list position ID, ordered by their sequence. Each position in the collection has its used
        /// count calculated before being returned.</summary>
        [ACPropertyList(9999, "PartslistPos", Description =
                        "Gets a collection of parts list positions associated with the selected parts list. " +
                        "The returned collection includes only positions of type OutwardRoot with no " +
                        "alternative parts list position ID, ordered by their sequence. Each position in the collection has its used " +
                        "count calculated before being returned.")]
        public IEnumerable<PartslistPos> PartslistPosList
        {
            get
            {
                if (SelectedPartslist == null)
                    return null;
                var list =
                    SelectedPartslist
                    .PartslistPos_Partslist
                    .Where(x => x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.OutwardRoot && x.AlternativePartslistPosID == null)
                    .OrderBy(x => x.Sequence)
                    .ToList();
                foreach (var pos in list)
                {
                    pos.CalcPositionUsedCount();
                }
                return list;
            }
        }

        #endregion

        #region Partslistpos -> Methods

        /// <summary>
        /// Creates a new component (PartslistPos) in the bill of material (Partslist).
        /// This method initializes a new Partslist position, assigns it a sequence number based
        /// on the  current count of positions, and adds it to the associated Partslist. The newly created position  is
        /// set as the selected position, and the state is updated to indicate a new entry.</summary>
        [ACMethodInteraction("PartslistPos", "en{'New Component'}de{'Neue Komponente'}", (short)MISort.New, true, "SelectedPartslistPos", Global.ACKinds.MSMethodPrePost, Description =
                             "Creates a new component (PartslistPos) in the bill of material (Partslist). " +
                             "This method initializes a new Partslist position, assigns it a sequence number based " +
                             "on the current count of positions, and adds it to the associated Partslist. The newly created position " +
                             "is set as the selected position, and the state is updated to indicate a new entry.")]
        public void NewPartslistPos()
        {
            if (!PreExecute()) return;
            var partsListPos = PartslistPos.NewACObject(DatabaseApp, SelectedPartslist);
            partsListPos.Sequence = PartslistPosList.Count();
            CurrentPartslist.PartslistPos_Partslist.Add(partsListPos);
            SelectedPartslistPos = partsListPos;
            ACState = Const.SMNew;
            OnPropertyChanged(nameof(PartslistPosList));
            PostExecute();
        }

        /// <summary>
        /// Deletes the currently selected component (PartslistPos), including any associated relationships or references,
        /// based on user confirmation. This method performs the following actions: Prompts the
        /// user to confirm the deletion of relationships if the selected parts list position is part of any mixtures.
        /// Prompts the user to confirm the deletion of references if the selected parts list position is
        /// referenced in production orders. Deletes associated relationships and references if confirmed
        /// by the user. Removes the selected parts list position from the parts list and updates the
        /// sequence and related properties. If the deletion process encounters any issues, a detailed
        /// message is displayed to the user.</summary>
        [ACMethodInteraction("PartslistPos", "en{'Delete Component'}de{'Komponente löschen'}", (short)MISort.Delete, true, "CurrentPartslistPos", Global.ACKinds.MSMethodPrePost, Description =
                             "Deletes the currently selected component (PartslistPos), including any associated relationships or references, " +
                             "based on user confirmation. This method performs the following actions: Prompts the " +
                             "user to confirm the deletion of relationships if the selected parts list position is part of any mixtures. " +
                             "Prompts the user to confirm the deletion of references if the selected parts list position is " +
                             "referenced in production orders. Deletes associated relationships and references if confirmed " +
                             "by the user. Removes the selected parts list position from the parts list and updates the " +
                             "sequence and related properties. If the deletion process encounters any issues, a detailed " +
                             "message is displayed to the user.")]
        public void DeletePartslistPos()
        {
            if (!PreExecute()) return;
            MsgWithDetails msg = new MsgWithDetails();


            Global.MsgResult questionDeleteRelations = Global.MsgResult.Yes;
            bool takePartInMixures = SelectedPartslistPos.PartslistPosRelation_SourcePartslistPos.Any();
            if (takePartInMixures)
            {
                Msg childDeleteQuestion = new Msg() { MessageLevel = eMsgLevel.Question, Message = Root.Environment.TranslateMessage(this, "Error50048") };
                questionDeleteRelations = Messages.Msg(childDeleteQuestion, Global.MsgResult.No, eMsgButton.YesNo);
            }

            Global.MsgResult questionDeleteReferencedProdPos = Global.MsgResult.Yes;
            bool isReferencedInProd = SelectedPartslistPos.ProdOrderPartslistPos_BasedOnPartslistPos.Any();
            if (isReferencedInProd)
            {
                Msg msgRemoveProdOrderRef = new Msg() { MessageLevel = eMsgLevel.Question, Message = Root.Environment.TranslateMessage(this, "Error50545") };
                questionDeleteReferencedProdPos = Messages.Msg(msgRemoveProdOrderRef, Global.MsgResult.No, eMsgButton.YesNo);
            }

            if (questionDeleteRelations == Global.MsgResult.Yes && questionDeleteReferencedProdPos == MsgResult.Yes)
            {
                if (takePartInMixures)
                {
                    var relations = SelectedPartslistPos.PartslistPosRelation_SourcePartslistPos.ToList();
                    foreach (var item in relations)
                    {
                        Msg msgDelRelations = item.DeleteACObject(DatabaseApp, false);
                        if (msgDelRelations != null)
                        {
                            msg.AddDetailMessage(msgDelRelations);
                            if (SelectedIntermediate.PartslistPosRelation_SourcePartslistPos.Contains(item))
                                SelectedIntermediate.PartslistPosRelation_SourcePartslistPos.Remove(item);
                        }
                    }
                }

                if (isReferencedInProd)
                {
                    ProdOrderPartslistPos[] referendedPositions = SelectedPartslistPos.ProdOrderPartslistPos_BasedOnPartslistPos.ToArray();
                    foreach (ProdOrderPartslistPos refPos in referendedPositions)
                    {
                        SelectedIntermediate.ProdOrderPartslistPos_BasedOnPartslistPos.Remove(refPos);
                        refPos.BasedOnPartslistPosID = null;
                        refPos.BasedOnPartslistPos = null;
                    }
                }

                if (msg.MsgDetailsCount == 0)
                    msg = SelectedPartslistPos.DeleteACObject(Database, true);

                if (msg != null && msg.MsgDetailsCount > 0)
                {
                    Messages.Msg(msg);
                    return;
                }
                else
                {
                    WriteLastFormulaChangeByDeleteElement(SelectedPartslist);
                    SelectedPartslist.PartslistPos_Partslist.Remove(SelectedPartslistPos);
                    SequenceManager<PartslistPos>.Order(PartslistPosList);
                    SelectedPartslistPos = PartslistPosList.FirstOrDefault();
                    OnPropertyChanged(nameof(PartslistPosList));
                    OnPropertyChanged(nameof(IntermediatePartsList));
                }
            }
            PostExecute();
        }

        #region Partslistpos -> Methods -> IsEnabled

        /// <summary>
        /// Determines whether a new component (PartslistPos) can be created based on the current selection.
        /// </summary>
        /// <remarks>This method checks the state of the <c>SelectedPartslist</c> to determine if it is
        /// valid for creating a new parts list position. Ensure that <c>SelectedPartslist</c> is properly set before
        /// calling this method.</remarks>
        /// <returns><see langword="true"/> if a parts list is selected and its <c>PartslistID</c> is not an empty GUID;
        /// otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledNewPartslistPos()
        {
            return SelectedPartslist != null && SelectedPartslist.PartslistID != new Guid();
        }

        /// <summary>
        /// Determines whether the delete operation for the selected component (PartslistPos) is enabled.
        /// </summary>
        /// <remarks>This method checks the current state of the selected parts list positions to
        /// determine if the delete operation can be performed. Ensure that <c>SelectedPartslistPos</c> and
        /// <c>AlternativeSelectedPartslistPos</c> are properly set before calling this method.</remarks>
        /// <returns><see langword="true"/> if a parts list position is selected and no alternative parts list position is
        /// selected; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledDeletePartslistPos()
        {
            return SelectedPartslistPos != null && AlternativeSelectedPartslistPos == null;
        }

        #endregion

        #endregion

        #region Partslistpos -> Search (PartslistPos)

        /// <summary>
        /// Searches and update the selected position in the parts list based on the provided parameter or defaults to the first
        /// position. If SelectedPartslist is null, the selected position is cleared by
        /// setting SelectedPartslistPos to null. Otherwise, the method sets
        /// SelectedPartslistPos to the provided <paramref name="selected"/> position or defaults to the first
        /// position in PartslistPosList.</summary>
        /// <param name="selected">The specific <see cref="PartslistPos"/> to select. If null, the first position in the list is selected.</param>
        public void SearchPos(PartslistPos selected = null)
        {
            if (SelectedPartslist == null)
            {
                SelectedPartslistPos = null;
            }
            else
            {
                if (selected != null)
                {
                    SelectedPartslistPos = selected;
                }
                else
                {
                    SelectedPartslistPos = PartslistPosList.FirstOrDefault();
                }
            }
            OnPropertyChanged(nameof(PartslistPosList));
        }

        /// <summary>
        /// Refreshes the current position by recalculating or updating it based on the latest state.
        /// </summary>
        /// <remarks>This method invokes an internal operation to ensure the position is up-to-date.  Use
        /// this method when the position needs to be refreshed due to changes in the underlying state.</remarks>
        public void RefreshPos()
        {
            RefreshAlternative();
        }

        #endregion

        #endregion

        #region PartslistQueryForPartslistpos

        #region PartslistQueryForPartslistpos -> Select, (Current,) List

        /// <summary>
        /// Gets or sets the selected parent parts list associated with the current parts list position.
        /// Setting this property updates the parent parts list of the selected parts list
        /// position. If the provided value is null or has an empty Partslist.PartslistID",  the parent parts list will be cleared.</summary>
        [ACPropertySelected(9999, "PartslistQueryForPartslistpos", "en{'Manufactured from BOM'}de{'Hergestellt aus Stückliste'}", Description =
                            "Gets or sets the selected parent parts list associated with the current parts list position. " +
                            "Setting this property updates the parent parts list of the selected parts list " +
                            "position. If the provided value is null or has an empty Partslist.PartslistID, the parent parts list will be cleared.")]
        public Partslist SelectedPartslistQueryForPartslistpos
        {
            get
            {
                if (SelectedPartslistPos == null) return null;
                return SelectedPartslistPos.ParentPartslist;
            }
            set
            {
                if (SelectedPartslistPos != null && SelectedPartslistPos.ParentPartslist != value)
                {
                    if (value != null && value.PartslistID != Guid.Empty)
                    {
                        SelectedPartslistPos.ParentPartslist = value;
                    }
                    else
                    {
                        SelectedPartslistPos.ParentPartslist = null;
                    }
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a list of Partslist associated with the selected material in the selected component (PartslistPos).
        /// </summary>
        [ACPropertyList(9999, "PartslistQueryForPartslistpos", Description =
                        "Gets a list of Partslist associated with the selected material in the selected component (PartslistPos)")]
        public List<Partslist> PartslistQueryForPartslistposList
        {
            get
            {
                if (SelectedPartslistPos == null || SelectedPartslistPos.Material == null)
                    return null;
                List<Partslist> list = SelectedPartslistPos.Material.Partslist_Material.OrderBy(x => x.PartslistNo).ThenBy(x => x.PartslistName).ToList();
                // Clear selection item
                Partslist clearSelectionItem = new Partslist();
                clearSelectionItem.PartslistNo = "-";
                clearSelectionItem.PartslistName = Root.Environment.TranslateMessage(this, "Question50013");
                list.Insert(0, clearSelectionItem);
                return list;
            }
        }

        #endregion

        #endregion

        #region AlternativePartslistPos

        #region AlternativePartslistPos -> Select, (Current,) List

        private PartslistPos _AlternativeSelectedPartslistPos;
        /// <summary>
        /// Gets or sets the alternative selected component (PartslistPos).
        /// </summary>
        [ACPropertySelected(9999, "AlternativePartslistpos", Description =
                            "Gets or sets the alternative selected component (PartslistPos).")]
        public PartslistPos AlternativeSelectedPartslistPos
        {
            get
            {
                return _AlternativeSelectedPartslistPos;
            }
            set
            {
                if (_AlternativeSelectedPartslistPos != value)
                {
                    _AlternativeSelectedPartslistPos = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a collection of alternative components (PartslistPos) associated with the currently selected component (PartslistPos).
        /// The returned collection is filtered to include only those positions where the
        /// AlternativePartslistPosID matches the PartslistPosID of the currently selected parts list
        /// position. The collection is ordered by the Sequence property.</summary>
        [ACPropertyList(9999, "AlternativePartslistpos", Description =
                        "Gets a collection of alternative components (PartslistPos) associated with the currently selected component (PartslistPos). " +
                        "The returned collection is filtered to include only those positions where the " +
                        "AlternativePartslistPosID matches the PartslistPosID of the currently selected parts list " +
                        "position. The collection is ordered by the Sequence property.")]
        public IEnumerable<PartslistPos> AlternativePartslistPosList
        {
            get
            {
                if (SelectedPartslist == null || SelectedPartslistPos == null)
                    return null;
                return SelectedPartslist.PartslistPos_Partslist.Where(x => x.AlternativePartslistPosID == SelectedPartslistPos.PartslistPosID).OrderBy(x => x.Sequence);
            }
        }

        #endregion

        #region AlternativePartslistPos -> Methods

        /// <summary>
        /// Creates a new alternative component for the currently selected component (PartslistPos).
        /// This method generates a new alternative partslist position, assigns it a sequence
        /// number based on the  current count of alternative positions, and adds it to the associated parts list. The
        /// newly created  alternative position is then set as the selected alternative position. The method also
        /// updates the  state and notifies listeners of changes to the alternative parts list.</summary>
        [ACMethodInteraction("AlternativeNewPartlistPos", "en{'New alternative component'}de{'Neue Alternativkomponente'}", (short)MISort.New, true, "SelectedPartslistPos", Global.ACKinds.MSMethodPrePost, Description =
                             "Creates a new alternative component for the currently selected component (PartslistPos)." +
                             "This method generates a new alternative partslist position, assigns it a sequence " +
                             "number based on the  current count of alternative positions, and adds it to the associated parts list. The " +
                             "newly created  alternative position is then set as the selected alternative position. The method also " +
                             "updates the  state and notifies listeners of changes to the alternative parts list.")]
        public void AlternativeNewPartlistPos()
        {
            if (!PreExecute()) return;
            PartslistPos alternativePartslistpos = PartslistPos.NewAlternativePartslistPos(DatabaseApp, SelectedPartslist, SelectedPartslistPos);
            alternativePartslistpos.Sequence = AlternativePartslistPosList.Count();
            SelectedPartslist.PartslistPos_Partslist.Add(alternativePartslistpos);
            AlternativeSelectedPartslistPos = alternativePartslistpos;
            OnPropertyChanged(nameof(AlternativePartslistPosList));
            ACState = Const.SMNew;
            PostExecute();
        }

        /// <summary>
        /// Deletes the currently selected alternative component from the partslist.
        /// This method performs a pre-execution check before attempting to delete the selected
        /// alternative component. If the deletion requires confirmation, a message prompt is displayed to the user.
        /// Upon successful deletion, the component is removed from the partslist, and the remaining components are
        /// reordered. The method also updates the selected alternative component to the first available item in the
        /// list, if any.</summary>
        [ACMethodInteraction("AlternativeDeletePartslistPos", "en{'Delete alternative component'}de{'Alternativkomponente löschen'}", (short)MISort.Delete, true, "SelectedPartslist", Global.ACKinds.MSMethodPrePost, Description =
                             "Deletes the currently selected alternative component from the partslist. " +
                             "This method performs a pre-execution check before attempting to delete the selected " +
                             "alternative component. If the deletion requires confirmation, a message prompt is displayed to the user. " +
                             "Upon successful deletion, the component is removed from the partslist, and the remaining components are " +
                             "reordered. The method also updates the selected alternative component to the first available item in the " +
                             "list, if any.")]
        public void AlternativeDeletePartslistPos()
        {
            if (!PreExecute()) return;
            Msg msg = AlternativeSelectedPartslistPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Global.MsgResult result = Messages.Msg(msg, Global.MsgResult.No, eMsgButton.YesNo);
                if (result == Global.MsgResult.Yes)
                {
                    msg = AlternativeSelectedPartslistPos.DeleteACObject(DatabaseApp, false);
                    if (msg != null)
                    {
                        Messages.Msg(msg);
                    }
                }
            }
            if (msg == null)
            {
                SelectedPartslist.PartslistPos_Partslist.Remove(AlternativeSelectedPartslistPos);
                SequenceManager<PartslistPos>.Order(AlternativePartslistPosList);
                AlternativeSelectedPartslistPos = AlternativePartslistPosList.FirstOrDefault();
                OnPropertyChanged(nameof(AlternativePartslistPosList));
            }
            PostExecute();
        }


        #region AlternativePartslistPos -> Methods -> IsEnabled
        
        /// <summary>
        /// Determines whether the creation of a new alternative component (PartslistPos) is enabled.
        /// </summary>
        /// <remarks>This method checks whether a part list position is selected, as a prerequisite for
        /// enabling the creation of a new alternative part list position.</remarks>
        /// <returns><see langword="true"/> if a part list position is currently selected; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledAlternativeNewPartlistPos()
        {
            return SelectedPartslistPos != null;
        }

        /// <summary>
        /// Determines whether the  delete operation for the selected alternative component (PartslistPos) is enabled.
        /// </summary>
        /// <returns><see langword="true"/> if an alternative parts list position is selected; otherwise, <see
        /// langword="false"/>.</returns>
        public bool IsEnabledAlternativeDeletePartslistPos()
        {
            return AlternativeSelectedPartslistPos != null;
        }

        #endregion

        #endregion

        #region AlternativePartslistPos -> Search (SearchSectionTypeName)

        /// <summary>
        /// Search and update the selection for the current alternative component.
        /// </summary>
        /// <remarks>This method updates the <see cref="AlternativeSelectedPartslistPos"/> property based
        /// on the provided <paramref name="selected"/> parameter or the first available alternative. If <see
        /// cref="SelectedPartslistPos"/> is <see langword="null"/>, no alternative will be selected.</remarks>
        /// <param name="selected">The alternative parts list position to select. If <see langword="null"/>, the first available alternative
        /// from <see cref="AlternativePartslistPosList"/> will be selected, or no selection will be made if <see
        /// cref="SelectedPartslistPos"/> is <see langword="null"/>.</param>
        public void SearchAlternative(PartslistPos selected = null)
        {
            if (SelectedPartslistPos == null)
            {
                AlternativeSelectedPartslistPos = null;
            }
            else
            {
                if (selected != null)
                {
                    AlternativeSelectedPartslistPos = selected;
                }
                else
                {
                    AlternativeSelectedPartslistPos = AlternativePartslistPosList.FirstOrDefault();
                }
            }
            OnPropertyChanged(nameof(AlternativePartslistPosList));
        }

        public void RefreshAlternative()
        {

        }

        #endregion

        #endregion

        #region Intermediate

        #region Intermediate -> Select, (Current,) List

        private PartslistPos _SelectedIntermediate;
        /// <summary>
        /// Gets or sets the currently selected intermediate PartslistPos item.
        /// </summary>
        [ACPropertySelected(9999, "Intermediate", Description = "Gets or sets the currently selected intermediate PartslistPos item.")]
        public PartslistPos SelectedIntermediate
        {
            get
            {
                return _SelectedIntermediate;
            }
            set
            {
                if (_SelectedIntermediate != value)
                {
                    if (_SelectedIntermediate != null)
                        _SelectedIntermediate.PropertyChanged -= _SelectedIntermediate_PropertyChanged;
                    _SelectedIntermediate = value;
                    if (_SelectedIntermediate != null)
                        _SelectedIntermediate.PropertyChanged += _SelectedIntermediate_PropertyChanged;
                    SearchIntermediateParts();
                    OnPropertyChanged();
                }
            }
        }


        private void _SelectedIntermediate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var test = e.PropertyName;
        }

        /// <summary>
        /// Gets the collection of intermediate positions associated with the selected parts list.
        /// The returned collection includes only positions classified as "intermediate" based on
        /// their material position type. The collection is ordered by sequence and, if available, by material number.
        /// </summary>
        [ACPropertyList(9999, "Intermediate", Description =
                        "Gets the collection of intermediate positions associated with the selected parts list. " +
                        "The returned collection includes only positions classified as \"intermediate\" based on " +
                        "their material position type. The collection is ordered by sequence and, if available, by material number.")]
        public IEnumerable<PartslistPos> IntermediateList
        {
            get
            {
                if (SelectedPartslist == null) return null;
                return SelectedPartslist.PartslistPos_Partslist.Where(x => x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.InwardIntern)
                                                               .OrderBy(x => x.Sequence).ThenBy(x => x.Material != null ? x.Material.MaterialNo : "");
            }
        }

        #endregion

        #region Intermediate -> Methods
        // No Intermediate operatins there
        #endregion

        #region Intermediate -> Search (SearchSectionTypeName)

        /// <summary>
        /// Selects an intermediate item from the list of available intermediates, optionally based on a specified
        /// selection. If no parts list is currently selected, the intermediate selection is cleared.
        /// Otherwise, the specified item is selected if provided; if not, the first available intermediate is
        /// selected.</summary>
        /// <param name="selected">The intermediate item to select. If null, the first item in the list is selected. If no
        /// items are available, the selection is cleared.</param>
        [ACMethodCommand("Intermediate", "en{'Search Bill'}de{'Suchen'}", (short)MISort.Search, Description =
                         "Selects an intermediate item from the list of available intermediates, optionally based on a specified " +
                         "selection. If no parts list is currently selected, the intermediate selection is cleared. " +
                         "Otherwise, the specified item is selected if provided; if not, the first available intermediate is " +
                         "selected.")]
        public void SearchIntermediate(PartslistPos selected = null)
        {
            if (SelectedPartslist == null)
            {
                SelectedIntermediate = null;
            }
            else
            {
                if (selected != null)
                {
                    SelectedIntermediate = selected;
                }
                else
                {
                    SelectedIntermediate = IntermediateList.FirstOrDefault();
                }
            }
            OnPropertyChanged(nameof(IntermediateList));
        }

        /// <summary>
        /// Recalculates the totals and remaining quantities for the currently selected intermediate product.
        /// This method updates the calculated sums and remaining quantities for the current
        /// partslist and notifies listeners of changes to the IntermediateList property. It should be
        /// called after modifications to the parts list to ensure that all totals and related data are up to
        /// date.</summary>
        [ACMethodInteraction("IntermediateParts", "en{'Recalculate Totals'}de{'Summenberechnung'}", (short)MISort.Modify, true, "SelectedIntermediate", Global.ACKinds.MSMethodPrePost, Description =
                             "Recalculates the totals and remaining quantities for the currently selected intermediate product. " +
                             "This method updates the calculated sums and remaining quantities for the current " +
                             "partslist and notifies listeners of changes to the IntermediateList property. It should be " +
                             "called after modifications to the parts list to ensure that all totals and related data are up to " +
                             "date.")]
        public void RecalcIntermediateSum()
        {
            PartslistManager.RecalcIntermediateSum(CurrentPartslist);
            PartslistManager.RecalcRemainingQuantity(CurrentPartslist);
            OnPropertyChanged(nameof(IntermediateList));

            if (IntermediateList != null)
            {
                ClearForRecalculateFlag(IntermediateList);
            }
        }

        private void ClearForRecalculateFlag(IEnumerable<PartslistPos> intermediateList)
        {
            foreach (PartslistPos pos in intermediateList)
            {
                pos.IsIntermediateForRecalculate = false;
            }
        }

        /// <summary>
        /// Determines whether recalculation of the intermediate sum is enabled for the current partslist.
        /// </summary>
        /// <returns><see langword="true"/> if a current parts list is available; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledRecalcIntermediateSum()
        {
            return CurrentPartslist != null;
        }

        #endregion

        #endregion

        #region IntermedateParts

        #region IntermedateParts -> Select, (Current,) List

        private PartslistPosRelation _SelectedIntermediateParts;
        /// <summary>
        /// Gets or sets the selected component associated with the selected intermediate product.
        /// </summary>
        [ACPropertySelected(9999, "IntermediateParts", isRightmanagement: true, Description = "Gets or sets the selected component associated with the selected intermediate product.")]
        public PartslistPosRelation SelectedIntermediateParts
        {
            get
            {
                return _SelectedIntermediateParts;
            }
            set
            {
                if (_SelectedIntermediateParts != value)
                {
                    if (_SelectedIntermediateParts != null)
                        _SelectedIntermediateParts.PropertyChanged -= _SelectedIntermediateParts_PropertyChanged;
                    _SelectedIntermediateParts = value;
                    if (_SelectedIntermediateParts != null)
                        _SelectedIntermediateParts.PropertyChanged += _SelectedIntermediateParts_PropertyChanged;
                    OnPropertyChanged();
                }
            }
        }

        void _SelectedIntermediateParts_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PartslistPosRelation.SourcePartslistPosID))
            {
                if (SelectedIntermediateParts != null && SelectedIntermediateParts.TargetPartslistPos != null)
                {
                    SelectedIntermediateParts.TargetPartslistPos.IsIntermediateForRecalculate = true;
                }
                //_SelectedIntermediateParts.TargetQuantity = 0;
                if (_SelectedIntermediateParts != null)
                {
                    if (_SelectedIntermediateParts.SourcePartslistPos == null)
                        _SelectedIntermediateParts.SourcePartslistPos = PartslistPosList.FirstOrDefault(c => c.PartslistPosID == _SelectedIntermediateParts.SourcePartslistPosID);
                    _SelectedIntermediateParts.TargetQuantityUOM = _SelectedIntermediateParts.SourcePartslistPos?.TargetQuantityUOM ?? 0;
                    //if (PartslistPosList != null && PartslistPosList.Any())
                    //{
                    //    //_SelectedIntermediateParts.TargetQuantity = PartslistPosList
                    //    //    .Where(p => p.MaterialID == _SelectedIntermediateParts.SourcePartslistPos.MaterialID)
                    //    //    .Sum(p => p.TargetQuantity);
                    //    _SelectedIntermediateParts.TargetQuantityUOM = PartslistPosList
                    //        .Where(p =>
                    //            _SelectedIntermediateParts.SourcePartslistPos != null
                    //            && p.MaterialID == _SelectedIntermediateParts.SourcePartslistPos.MaterialID)
                    //        .Sum(p => p.TargetQuantityUOM);
                    //}

                    if (IntermediatePartsList != null && IntermediatePartsList.Any())
                    {
                        //double usedQuantity = IntermediateList.SelectMany(x=>x.PartslistPosRelation_TargetPartslistPos)
                        //    .Where(p => 
                        //        p.SourcePartslistPos.MaterialID == _SelectedIntermediateParts.SourcePartslistPos.MaterialID &&
                        //        p.PartslistPosRelationID != _SelectedIntermediateParts.PartslistPosRelationID).Sum(p => p.TargetQuantity);
                        //_SelectedIntermediateParts.TargetQuantity = _SelectedIntermediateParts.TargetQuantity - usedQuantity;
                        double usedQuantityUOM = IntermediateList.SelectMany(x => x.PartslistPosRelation_TargetPartslistPos)
                            .Where(p =>
                                _SelectedIntermediateParts.SourcePartslistPos != null
                                && p.SourcePartslistPosID != Guid.Empty
                                && p.SourcePartslistPos.MaterialID == _SelectedIntermediateParts.SourcePartslistPos.MaterialID
                                && p.SourcePartslistPosID == _SelectedIntermediateParts.SourcePartslistPosID
                                && p.PartslistPosRelationID != _SelectedIntermediateParts.PartslistPosRelationID
                               ).Sum(p => p.TargetQuantityUOM);
                        _SelectedIntermediateParts.TargetQuantityUOM = _SelectedIntermediateParts.TargetQuantityUOM - usedQuantityUOM;
                    }

                    _SelectedIntermediateParts.OnEntityPropertyChanged(nameof(PartslistPosRelation.TargetQuantity));
                    _SelectedIntermediateParts.OnEntityPropertyChanged(nameof(PartslistPosRelation.TargetQuantityUOM));
                    OnPropertyChanged(nameof(SelectedIntermediateParts));
                    OnPropertyChanged(nameof(IntermediatePartsList));

                    _SelectedIntermediateParts.SourcePartslistPos.CalcPositionUsedCount();
                }
            }
            else if (e.PropertyName == nameof(PartslistPosRelation.TargetQuantityUOM) || e.PropertyName == nameof(PartslistPosRelation.TargetQuantity))
            {
                if (SelectedIntermediateParts != null && SelectedIntermediateParts.TargetPartslistPos != null)
                {
                    SelectedIntermediateParts.TargetPartslistPos.IsIntermediateForRecalculate = true;
                }
            }
        }


        /// <summary>
        /// Gets the collection of components associated with the selected intermediate product, ordered
        /// by sequence.
        /// </summary>
        [ACPropertyList(9999, "IntermediateParts", Description = "Gets the collection of components associated with the selected intermediate product, ordered by sequence.")]
        public IEnumerable<PartslistPosRelation> IntermediatePartsList
        {
            get
            {
                if (SelectedIntermediate == null) return null;
                return SelectedIntermediate.PartslistPosRelation_TargetPartslistPos.OrderBy(x => x.Sequence);
            }
        }

        #endregion

        #region IntermedateParts -> Methods

        /// <summary>
        /// Creates a new assignemnt and adds it to the SelectedIntermediateParts property, also automatically fills 
        /// the list IntermediatePartsList where are all assigned materials to the intermediate product.
        /// This method initializes a new intermediate part relationship and updates the relevant
        /// collections and properties to reflect the addition. It also triggers property change notifications for
        /// dependent lists. The method should be called when a new component needs to be associated with the
        /// currently selected intermediate product.</summary>
        [ACMethodInteraction("IntermediateParts", "en{'New Input'}de{'Neuer Einsatz'}", (short)MISort.New, true, "SelectedIntermediateParts", Global.ACKinds.MSMethodPrePost, Description =
                             "Creates a new assignemnt and adds it to the SelectedIntermediateParts property, also automatically fills " +
                             "the list IntermediatePartsList where are all assigned materials to the intermediate product. " +
                             "This method initializes a new intermediate part relationship and updates the relevant " +
                             "collections and properties to reflect the addition. It also triggers property change notifications for " +
                             "dependent lists. The method should be called when a new component needs to be associated with the " +
                             "currently selected intermediate product.")]
        public void NewIntermediateParts()
        {
            if (!PreExecute()) return;
            PartslistPosRelation partslistPosRelation = new PartslistPosRelation();
            partslistPosRelation.PartslistPosRelationID = Guid.NewGuid();
            partslistPosRelation.TargetPartslistPos = SelectedIntermediate;
            partslistPosRelation.Sequence = 1;
            if (IntermediatePartsList != null && IntermediatePartsList.Any())
            {
                partslistPosRelation.Sequence = partslistPosRelation.Sequence + IntermediatePartsList.Select(c => c.Sequence).DefaultIfEmpty().Max();
            }
            SelectedIntermediate.PartslistPosRelation_TargetPartslistPos.Add(partslistPosRelation);
            SelectedIntermediateParts = partslistPosRelation;
            OnPropertyChanged(nameof(IntermediatePartsList));
            OnPropertyChanged(nameof(PartslistPosList));
            PostExecute();
            SelectedIntermediateParts.TargetPartslistPos.IsIntermediateForRecalculate = true;
        }

        /// <summary>
        /// Deletes the currently selected assignment from the intermediate product.
        /// This method removes the selected assignemnt and updates related data,
        /// including recalculating affected positions and notifying property changes. If the deletion requires user
        /// confirmation, a prompt is displayed. The method also ensures that any necessary recalculations are triggered
        /// for the target position.</summary>
        [ACMethodInteraction("IntermediateParts", "en{'Delete Input'}de{'Lösche Einsatz'}", (short)MISort.New, true, "SelectedIntermediateParts", Global.ACKinds.MSMethodPrePost, Description =
                             "Deletes the currently selected assignment from the intermediate product. " +
                             "This method removes the selected assignemnt and updates related data, " +
                             "including recalculating affected positions and notifying property changes. If the deletion requires user " +
                             "confirmation, a prompt is displayed. The method also ensures that any necessary recalculations are triggered " +
                             "for the target position.")]
        public void DeleteIntermediateParts()
        {
            if (!PreExecute()) return;
            PartslistPos sourcePos = SelectedIntermediateParts.SourcePartslistPos;
            PartslistPos targetPos = SelectedIntermediateParts.TargetPartslistPos;
            Msg msg = SelectedIntermediateParts.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Global.MsgResult result = Messages.Msg(msg, Global.MsgResult.No, eMsgButton.YesNo);
                if (result == Global.MsgResult.Yes)
                {
                    msg = SelectedIntermediateParts.DeleteACObject(DatabaseApp, false);
                    if (msg != null)
                    {
                        Messages.Msg(msg);
                    }
                }
            }
            if (msg == null)
            {
                WriteLastFormulaChangeByDeleteElement(SelectedPartslist);
                SelectedIntermediate.PartslistPosRelation_TargetPartslistPos.Remove(SelectedIntermediateParts);
                // Note: relation order used as configuration
                // SequenceManager<PartslistPosRelation>.Order(IntermediatePartsList);
                SelectedIntermediateParts = IntermediatePartsList.FirstOrDefault();
                sourcePos.CalcPositionUsedCount();
                OnPropertyChanged(nameof(IntermediatePartsList));
                OnPropertyChanged(nameof(PartslistPosList));

                targetPos.IsIntermediateForRecalculate = true;
            }
            PostExecute();
        }

        /// <summary>
        /// Recalculates the remaining quantities for all components in the current partslist.
        /// This method triggers the PartslistManager to recalculate remaining quantities for the entire parts list
        /// and refreshes the UI by notifying property change for the PartslistPosList property.
        /// Call this method after making changes to component quantities to ensure the remaining quantity calculations are up to date.
        /// </summary>
        [ACMethodInteraction("IntermediateParts", "en{'Sum remaining quantitiy'}de{'Restmengen berechnen'}", (short)MISort.Modify, true, "SelectPartslistPos", Global.ACKinds.MSMethodPrePost, Description =
                             "Recalculates the remaining quantities for all components in the current partslist. " +
                             "This method triggers the PartslistManager to recalculate remaining quantities for the entire parts list " +
                             "and refreshes the UI by notifying property change for the PartslistPosList property. " +
                             "Call this method after making changes to component quantities to ensure the remaining quantity calculations are up to date.")]
        public void RecalcRemainingQuantity()
        {
            PartslistManager.RecalcRemainingQuantity(CurrentPartslist);
            OnPropertyChanged(nameof(PartslistPosList));
        }

        /// <summary>
        /// Determines whether creating a new assignment is enabled.
        /// This method checks if a new component can be assigned to the currently selected 
        /// intermediate product. The operation is enabled only when an intermediate product is selected.</summary>
        /// <returns><see langword="true"/> if an intermediate product is currently selected; otherwise, 
        /// <see langword="false"/>.</returns>
        public bool IsEnabledNewIntermediateParts()
        {
            return SelectedIntermediate != null;
        }

        /// <summary>
        /// Determines whether the delete operation for the selected assignment from the intermediate product is enabled.
        /// </summary>
        /// <remarks>
        /// The delete operation is enabled when:
        /// - An intermediate assignment (SelectedIntermediateParts) is currently selected
        /// - The source parts list position exists and is of type OutwardRoot (indicating it's a main component rather than an alternative)
        /// 
        /// This method ensures that only valid assignments between components and intermediate products can be deleted,
        /// preventing deletion of alternative components or assignments without proper source positions.
        /// </remarks>
        /// <returns><see langword="true"/> if the delete operation is enabled; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledDeleteIntermediateParts()
        {

            bool isEnabledDelete = SelectedIntermediateParts != null;

            if (!isEnabledDelete)

                return isEnabledDelete;

            if (SelectedIntermediateParts.SourcePartslistPos != null)

                isEnabledDelete = isEnabledDelete && SelectedIntermediateParts.SourcePartslistPos.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.OutwardRoot;

            return isEnabledDelete;

        }

        /// <summary>
        /// Determines whether the recalculation of remaining quantities is enabled for the current partslist.
        /// This method checks if a partslist is currently loaded and available for processing.
        /// </summary>
        /// <returns><c>true</c> if a current parts list is available and recalculation can be performed; otherwise, <c>false</c>.</returns>
        public bool IsEnabledRecalculateRestQuantity()
        {
            return CurrentPartslist != null;
        }

        #endregion

        #endregion

        #region IntermedateParts -> Search (SearchSectionTypeName)

        /// <summary>
        /// 
        /// Searches and selects an assignment from the list of components associated with the selected intermediate product.
        /// If no intermediate product is selected, clears the selection by setting SelectedIntermediateParts to null.
        /// Otherwise, sets the SelectedIntermediateParts to the provided selected parameter or defaults to the first item in IntermediatePartsList.
        /// Also triggers a property change notification for the IntermediatePartsList to update the UI binding.
        /// </summary>
        /// <param name="selected">The specific intermediate parts assignment to select. If null, the first assignment in the list is selected.</param>
        public void SearchIntermediateParts(PartslistPosRelation selected = null)
        {
            if (SelectedIntermediate == null)
            {
                SelectedIntermediateParts = null;
            }
            else
            {
                if (selected != null)
                {
                    SelectedIntermediateParts = selected;
                }
                else
                {
                    SelectedIntermediateParts = IntermediatePartsList.FirstOrDefault();
                }
            }
            OnPropertyChanged(nameof(IntermediatePartsList));
        }

        #endregion

        #region MaterialWF Selection

        #region MaterialWF Selection -> Select, (Current,) List

        private MaterialWF _selectedMaterialWF;
        /// <summary>
        /// Gets or sets the currently selected material workflow of the current partslist.
        /// When a material workflow is selected, it triggers the loading of associated process workflows
        /// and updates the workflow-related properties. This selection is used with the SetMaterialWF() 
        /// method to assign the workflow to the CurrentPartslist.MaterialWF property.
        /// Setting this property to null clears the current selection.
        /// </summary>
        [ACPropertySelected(9999, MaterialWF.ClassName, "en{'Select Workflow'}de{'Workflow auswählen'}", "", true, Description =
                            "Gets or sets the currently selected material workflow of the current partslist. " +
                            "When a material workflow is selected, it triggers the loading of associated process workflows " +
                            "and updates the workflow-related properties. This selection is used with the SetMaterialWF() " +
                            "method to assign the workflow to the CurrentPartslist.MaterialWF property. " +
                            "Setting this property to null clears the current selection.")]
        public MaterialWF SelectedMaterialWF
        {
            get
            {
                return _selectedMaterialWF;
            }
            set
            {
                if (_selectedMaterialWF != value)
                {
                    _selectedMaterialWF = value;
                    OnPropertyChanged();
                    LoadProcessWorkflows();
                }
            }
        }

        /// <summary>
        /// Gets a collection of all available material workflows ordered alphabetically by name.
        /// This collection is used for selecting material workflows that can be assigned to the current partslist.
        /// The material workflows define how materials flow through the production process.
        /// </summary>
        [ACPropertyList(9999, MaterialWF.ClassName)]
        public IEnumerable<MaterialWF> MaterialWFList
        {
            get
            {
                return DatabaseApp.MaterialWF.OrderBy(x => x.Name);
            }
        }
        #endregion

        #region MaterialWF Selection -> Methods

        ///<summary>
        /// Assigns the selected material workflow (MaterialWF) to the current partslist.
        /// This method first unassigns any existing material workflow from the current partslist,
        /// then assigns the newly selected material workflow. After successful assignment, it updates
        /// the UI by refreshing related properties and load intermediate products.
        /// If any errors occur during the unassignment or assignment process, error messages are displayed
        /// and the operation is terminated early.
        /// </summary>
        [ACMethodCommand(MaterialWF.ClassName, "en{'Assign Materialworkflow'}de{'Materialworkflow zuweisen'}", (short)MISort.Save, true, Global.ACKinds.MSMethodPrePost, Description =
                         "Assigns the selected material workflow (MaterialWF) to the current partslist. " +
                         "This method first unassigns any existing material workflow from the current partslist, " +
                         "then assigns the newly selected material workflow. After successful assignment, it updates " +
                         "the UI by refreshing related properties and load intermediate products. " +
                         "If any errors occur during the unassignment or assignment process, error messages are displayed " +
                         "and the operation is terminated early.")]
        public void SetMaterialWF()
        {
            if (!PreExecute()) return;
            if (CurrentPartslist != null && SelectedMaterialWF != null && CurrentPartslist.MaterialWFID != SelectedMaterialWF.MaterialWFID)
            {
                Msg msg = PartslistManager.UnAssignMaterialWF(DatabaseApp, CurrentPartslist);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }

                msg = PartslistManager.AssignMaterialWF(DatabaseApp, SelectedMaterialWF, CurrentPartslist);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }

                //Save();

                OnPropertyChanged(nameof(CurrentPartslist));

                SearchIntermediate();
                this.LoadProcessWorkflows();
                LoadMaterialWorkflows();
                if (msg != null)
                {
                    Messages.Msg(msg);
                }

                SelectedMaterialWF = null;
            }
            PostExecute();
        }

        /// <summary>
        /// Unassigns the material workflow from the current partslist and removes associated process workflows.
        /// This method detaches the currently assigned material workflow from the partslist, effectively 
        /// removing the production flow definition. It also removes any process workflows that were dependent 
        /// on the material workflow. After successful removal, the method saves the changes and updates 
        /// the UI by refreshing related properties and reloading workflow data.
        /// If any errors occur during the workflow removal process, error messages are displayed.
        /// </summary>
        [ACMethodCommand(MaterialWF.ClassName, "en{'Unassign Materialworkflow'}de{'Materialworkflow entfernen'}", (short)MISort.Save, true, Global.ACKinds.MSMethodPrePost, Description =
                         "Unassigns the material workflow from the current partslist and removes associated process workflows. " +
                         "This method detaches the currently assigned material workflow from the partslist, effectively " +
                         "removing the production flow definition. It also removes any process workflows that were dependent " +
                         "on the material workflow. After successful removal, the method saves the changes and updates " +
                         "the UI by refreshing related properties and reloading workflow data. " +
                         "If any errors occur during the workflow removal process, error messages are displayed.")]
        public void UnSetMaterialWF()
        {
            if (!PreExecute()) return;

            Msg msg = PartslistManager.UnAssignMaterialWF(DatabaseApp, CurrentPartslist);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            else
            {
                RemoveProcessWorkflow();
            }

            Save();

            OnPropertyChanged(nameof(CurrentPartslist));
            SearchIntermediate();
            LoadMaterialWorkflows();
            PostExecute();
        }

        /// <summary>
        /// Updates the current bill of materials (partslist) by synchronizing it with its assigned material workflow (only addition operation is supported).
        /// This method retrieves and applies any changes from the material workflow to the current partslist,
        /// ensuring that the partslist structure remains consistent with its associated workflow definition.
        /// If any errors occur during the update process, error messages are displayed to the user.
        /// After successful update, the changes are saved to the database.
        /// </summary>
        [ACMethodCommand(MaterialWF.ClassName, "en{'Update from Materialworkflow'}de{'Materialworkflow aktualisieren'}", 710, true, Description =
                         "Updates the current bill of materials (partslist) by synchronizing it with its assigned material workflow (only addition operation is supported). " +
                         "This method retrieves and applies any changes from the material workflow to the current partslist, " +
                         "ensuring that the partslist structure remains consistent with its associated workflow definition. " +
                         "If any errors occur during the update process, error messages are displayed to the user. " +
                         "After successful update, the changes are saved to the database.")]
        public void UpdateFromMaterialWF()
        {
            if (CurrentPartslist != null && CurrentPartslist.MaterialWF != null)
            {
                Msg msg = PartslistManager.UpdatePartslistFromMaterialWF(DatabaseApp, CurrentPartslist);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }

                ACSaveChanges();
            }
        }

        /// <summary>
        /// Updates all bill of materials (BOMs) that use the same material workflow as the current partslist (only addition operation is supported).
        /// This method prompts the user for confirmation before proceeding with the update operation.
        /// If confirmed, it iterates through all partslists that share the same MaterialWFID and applies
        /// updates from their respective material workflows. If any errors occur during the update process,
        /// error messages are displayed and the operation is terminated. After successful updates, all
        /// changes are saved to the database.
        /// </summary>
        [ACMethodCommand(MaterialWF.ClassName, "en{'Update all BOMs from Materialworkflow'}de{'Update all BOMs from Materialworkflow'}", 711, true, Description =
                         "Updates all bill of materials (BOMs) that use the same material workflow as the current partslist (only addition operation is supported). " +
                         "This method prompts the user for confirmation before proceeding with the update operation. " +
                         "If confirmed, it iterates through all partslists that share the same MaterialWFID and applies " +
                         "updates from their respective material workflows. If any errors occur during the update process, " +
                         "error messages are displayed and the operation is terminated. After successful updates, all " +
                         "changes are saved to the database.")]
        public void UpdateAllFromMaterialWF()
        {
            //Question50050
            if (Messages.Question(this, "Question50050", MsgResult.Yes, false, CurrentPartslist?.MaterialWF?.Name) == MsgResult.Yes)
            {
                var updateablePartslist = AccessPrimary.NavList.Where(c => c.MaterialWFID == CurrentPartslist.MaterialWFID).ToArray();

                if (updateablePartslist != null && updateablePartslist.Any())
                {
                    foreach (Partslist pl in updateablePartslist)
                    {
                        Msg msg = PartslistManager.UpdatePartslistFromMaterialWF(DatabaseApp, pl);
                        if (msg != null)
                        {
                            Messages.Msg(msg);
                            return;
                        }
                    }

                    ACSaveChanges();
                }
            }
        }


        #region MaterialWF Selection -> Methods -> IsEnabled

        /// <summary>
        /// Determines whether setting a material workflow is enabled for the current partslist.
        /// The operation is enabled when all of the following conditions are met:
        /// - A current partslist exists
        /// - The current partslist does not already have a material workflow assigned
        /// - A material workflow is selected for assignment
        /// - The current partslist has a material assigned
        /// - The partslist number is not empty
        /// - The partslist name is not empty
        /// </summary>
        /// <returns><see langword="true"/> if setting a material workflow is enabled; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledSetMaterialWF()
        {
            return
                CurrentPartslist != null
                && CurrentPartslist.MaterialWF == null
                && SelectedMaterialWF != null
                && CurrentPartslist.Material != null
                && !string.IsNullOrEmpty(CurrentPartslist.PartslistNo)
                && !string.IsNullOrEmpty(CurrentPartslist.PartslistName);
        }

        
        ///<summary>
        /// Determines whether the unassignment of the material workflow from the current partslist is enabled.
        /// The operation is enabled when both a current partslist exists and it has a material workflow assigned.
        /// This method is typically called before executing the UnSetMaterialWF() method to validate
        /// that there is a material workflow that can be removed from the partslist.
        /// </summary>
        /// <returns><see langword="true"/> if a current partslist exists and has a material workflow assigned; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledUnSetMaterialWF()
        {
            return CurrentPartslist != null && CurrentPartslist.MaterialWF != null;
        }


        /// <summary>
        /// Determines whether updating from the material workflow is enabled for the current partslist.
        /// This method checks if a current partslist exists and has an assigned material workflow.
        /// </summary>
        /// <returns><see langword="true"/> if a current partslist exists and has a material workflow assigned; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledUpdateFromMaterialWF()
        {
            return CurrentPartslist != null && CurrentPartslist.MaterialWF != null;
        }


        /// <summary>
        /// Determines whether updating all bill of materials from the material workflow is enabled for the current partslist.
        /// This method checks if a current partslist exists and has an assigned material workflow.
        /// </summary>
        /// <returns><see langword="true"/> if a current partslist exists and has a material workflow assigned; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledUpdateAllFromMaterialWF()
        {
            return CurrentPartslist != null && CurrentPartslist.MaterialWF != null;
        }

        #endregion

        private void DeleteMixedProducts()
        {
            List<PartslistPos> intermediateItems = DatabaseApp.PartslistPos.Where(x =>
                x.PartslistID == CurrentPartslist.PartslistID &&
                x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern).ToList();
            List<Guid> Ids = intermediateItems.Select(x => x.PartslistPosID).ToList();
            List<PartslistPosRelation> relations = DatabaseApp.PartslistPosRelation
                .Where(x => Ids.Contains(x.TargetPartslistPosID) || Ids.Contains(x.SourcePartslistPosID)).ToList();
            relations.ForEach(x => x.DeleteACObject(DatabaseApp, false));
            intermediateItems.ForEach(x => x.DeleteACObject(DatabaseApp, false));
        }

        #endregion

        #endregion

        #region MaterialWF

        private MaterialWFACClassMethod _ProcessWorkflow;

        /// <summary>
        /// Gets the collection of process workflows (MaterialWFACClassMethod) associated with the current partslist.
        /// Returns the MaterialWFACClassMethod instances from the partslist's assigned MaterialWF that are
        /// connected to this partslist through PartslistACClassMethod_Partslist relationships.
        /// If no partslist is selected, no MaterialWF is assigned, or no workflow methods are configured,
        /// returns an empty array.
        /// </summary>
        [ACPropertyList(9999, "ProcessWorkflow", Description =
                        "Gets the collection of process workflows (MaterialWFACClassMethod) associated with the current partslist. " +
                        "Returns the MaterialWFACClassMethod instances from the partslist's assigned MaterialWF that are " +
                        "connected to this partslist through PartslistACClassMethod_Partslist relationships. " +
                        "If no partslist is selected, no MaterialWF is assigned, or no workflow methods are configured, " +
                        "returns an empty array.")]
        public ICollection<MaterialWFACClassMethod> ProcessWorkflowList
        {
            get
            {
                if (this.CurrentPartslist == null || this.CurrentPartslist.MaterialWF == null || !this.CurrentPartslist.PartslistACClassMethod_Partslist.Any())
                    return new MaterialWFACClassMethod[0];
                else
                {
                    return this.CurrentPartslist.PartslistACClassMethod_Partslist.ToArray().Select(c => c.MaterialWFACClassMethod).ToArray();
                    //return this.CurrentPartslist.MaterialWF.MaterialWFACClassMethod_MaterialWF;
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected process workflow (MaterialWFACClassMethod) for the current partslist.
        /// This property represents the active workflow method that defines how the production process should be executed.
        /// When set, it automatically loads the workflow into the ProcessWorkflowPresenter and triggers a search
        /// for configuration transfer options. Setting this property also updates the ConfigurationTransferList
        /// to show other partslists that use the same process workflow for parameter copying.
        /// </summary>
        [ACPropertyCurrent(9999, "ProcessWorkflow", Description =
                           "Gets or sets the currently selected process workflow (MaterialWFACClassMethod) for the current partslist. " +
                           "This property represents the active workflow method that defines how the production process should be executed. " +
                           "When set, it automatically loads the workflow into the ProcessWorkflowPresenter and triggers a search " +
                           "for configuration transfer options. Setting this property also updates the ConfigurationTransferList " +
                           "to show other partslists that use the same process workflow for parameter copying.")]
        public MaterialWFACClassMethod CurrentProcessWorkflow
        {
            get
            {
                return _ProcessWorkflow;
            }
            set
            {
                _ProcessWorkflow = value;

                if (ProcessWorkflowPresenter != null)
                {
                    if (_ProcessWorkflow == null)
                        this.ProcessWorkflowPresenter.Load(null);
                    else
                        this.ProcessWorkflowPresenter.Load(_ProcessWorkflow.ACClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>());
                }

                OnPropertyChanged();
                if (_ProcessWorkflow != null)
                {
                    AccessConfigurationTransfer.NavSearch();
                }
                OnPropertyChanged(nameof(ConfigurationTransferList));
            }
        }

        private VBPresenterMaterialWF _MaterialWFPresenter;
        bool _MatPresenterRightsChecked = false;
        /// <summary>
        ///  Gets the material workflow presenter component for visualizing and interacting with material workflows.
        /// This presenter is responsible for displaying the material workflow associated with the current partslist
        /// and provides functionality for workflow visualization and navigation. The presenter is lazily initialized
        /// and includes rights management to ensure only authorized users can access workflow viewing capabilities.
        /// </summary>
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

        /// <summary>
        /// Loads and displays the material workflow associated with the selected partslist in the MaterialWFPresenter.
        /// This method updates the MaterialWFPresenter component to show the workflow visualization for the currently
        /// selected partslist. If no MaterialWFPresenter is available or no partslist is selected, no action is taken.
        /// Call this method when the selected partslist changes or when the material workflow needs to be refreshed.
        /// </summary>
        public void LoadMaterialWorkflows()
        {
            if (MaterialWFPresenter != null && SelectedPartslist != null)
            {
                MaterialWFPresenter.Load(SelectedPartslist.MaterialWF);
            }
        }

        /// <summary>
        /// Sets the selected material in the intermediate list based on the provided material value.
        /// This method searches through the IntermediateList to find an intermediate product
        /// that matches the provided material's MaterialID and updates the SelectedIntermediate property.
        /// If no matching intermediate is found or if the current SelectedIntermediate already
        /// matches the provided material, no change is made.
        /// </summary>
        /// <param name="value">The material to select in the intermediate list. If null, no action is taken.</param>
        /// <param name="selectPWNode">Optional parameter for process workflow node selection (currently unused in implementation).</param>
        [ACMethodInfo("Material", "en{'SetSelectedMaterial'}de{'SetSelectedMaterial'}", 999, Description =
                      "Sets the selected material in the intermediate list based on the provided material value. " +
                      "This method searches through the IntermediateList to find an intermediate product " +
                      "that matches the provided material's MaterialID and updates the SelectedIntermediate property. " +
                      "If no matching intermediate is found or if the current SelectedIntermediate already " +
                      "matches the provided material, no change is made.")]
        public void SetSelectedMaterial(Material value, bool selectPWNode = false)
        {
            if (SelectedIntermediate != null && IntermediateList != null && SelectedIntermediate.Material != value)
            {
                SelectedIntermediate = IntermediateList.FirstOrDefault(c => c.MaterialID == value.MaterialID);
            }
        }

        #endregion

        #region IACBSOConfigStoreSelection

        /// <summary>
        /// Gets the list of mandatory configuration stores required for the current partslist operations.
        /// This property returns a collection of IACConfigStore instances that must be considered
        /// when performing configuration-related operations on the current partslist. If a current
        /// partslist exists, it is included as a mandatory configuration store.
        /// </summary>
        /// <value>
        /// A list of IACConfigStore instances containing the current partslist if available,
        /// or an empty list if no current partslist is selected.
        /// </value>
        public List<IACConfigStore> MandatoryConfigStores
        {
            get
            {
                List<IACConfigStore> listOfSelectedStores = new List<IACConfigStore>();
                if (CurrentPartslist != null)
                {
                    listOfSelectedStores.Add(CurrentPartslist);
                }
                return listOfSelectedStores;
            }
        }

        /// <summary>
        /// Gets the current configuration store for this bill of materials business object.
        /// Returns the currently selected partslist (CurrentPartslist) which implements IACConfigStore
        /// and serves as the configuration storage for workflow parameters, process settings, and other
        /// configuration data associated with the bill of materials. Returns null if no partslist is currently selected.
        /// </summary>
        public IACConfigStore CurrentConfigStore
        {
            get
            {
                if (CurrentPartslist == null) return null;
                return CurrentPartslist;
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
        /// <summary>
        ///  Gets or sets the collection of workflow methods that have been visited or accessed during the configuration process.
        /// This property is used to track which workflow methods (ACClassMethod instances) have been navigated to or modified
        /// during the current session, enabling proper configuration management and change tracking across the application.
        /// The collection is automatically managed by the system and helps ensure that configuration changes are properly
        /// synchronized and persisted when working with bill of materials and their associated workflows.
        /// </summary>
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
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Adds the specified workflow method to the collection of visited methods for configuration tracking.
        /// This method is used to track which workflow methods have been accessed during the current session,
        /// enabling proper configuration management and change tracking across the application.
        /// Only adds the method if it's not already in the visited methods collection.
        /// </summary>
        /// <param name="acClassMethod">The workflow method (ACClassMethod) to add to the visited methods collection.</param>
        public void AddVisitedMethods(core.datamodel.ACClassMethod acClassMethod)
        {
            if (!VisitedMethods.Contains(acClassMethod))
                VisitedMethods.Add(acClassMethod);
        }

        #endregion

        #region Config Transfer

        /// <summary>
        /// Initiates the configuration transfer process for copying workflow parameters from a source partslist to the current partslist.
        /// This method opens the VBBSOConfigTransfer dialog and sets up the source and target configuration stores and ACClassMethods
        /// for transferring workflow parameters between bill of materials. The transfer allows copying process workflow configurations
        /// from one partslist to another, facilitating configuration reuse and standardization.
        /// </summary>
        [ACMethodInfo("ConfigurationTransfer", "en{'Next'}de{'Weiter'}", 9999, Description =
                      "Initiates the configuration transfer process for copying workflow parameters from a source partslist to the current partslist. " +
                      "This method opens the VBBSOConfigTransfer dialog and sets up the source and target configuration stores and ACClassMethods " +
                      "for transferring workflow parameters between bill of materials. The transfer allows copying process workflow configurations  " +
                      "from one partslist to another, facilitating configuration reuse and standardization.")]
        public void ConfigurationTransferSetSource()
        {
            if (!IsEnabledConfigurationTransferSetSource()) return;
            VBBSOConfigTransfer cnfTransfer = ConfigurationTransferGetBSO();
            if (cnfTransfer != null)
            {
                cnfTransfer.ConfigTransferCommand.TargetACClassMethods = CurrentPartslist.PartslistACClassMethod_Partslist.ToList().Select(x => x.MaterialWFACClassMethod.ACClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>()).ToList();
                cnfTransfer.ConfigTransferCommand.SourceACClassMethods = SelectedConfigurationTransfer.PartslistACClassMethod_Partslist.ToList().Select(x => x.MaterialWFACClassMethod.ACClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>()).ToList();

                cnfTransfer.ConfigTransferCommand.TargetConfigStore = CurrentPartslist;
                cnfTransfer.ConfigTransferCommand.SourceConfigStore = SelectedConfigurationTransfer;
                cnfTransfer.ConfigTransferCommand.Search();
                cnfTransfer.RefreshStorePreview();
                cnfTransfer.RefreshLists();
            }
        }

        /// <summary>
        /// Determines whether the configuration transfer operation is enabled for copying workflow parameters from a source partslist to the current partslist.
        /// This method checks if both the current partslist and the selected source partslist have associated process workflow methods (PartslistACClassMethod_Partslist) configured.
        /// The transfer operation requires that both partslists have workflow configurations to enable parameter copying between them.
        /// </summary>
        /// <returns><see langword="true"/> if both the current partslist and selected configuration transfer source have workflow methods configured; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledConfigurationTransferSetSource()
        {
            return
                CurrentPartslist != null &&
                CurrentPartslist.PartslistACClassMethod_Partslist != null &&
                CurrentPartslist.PartslistACClassMethod_Partslist.Any() &&

                SelectedConfigurationTransfer != null &&
                SelectedConfigurationTransfer.PartslistACClassMethod_Partslist != null &&
                SelectedConfigurationTransfer.PartslistACClassMethod_Partslist.Any();
        }

        private VBBSOConfigTransfer ConfigurationTransferGetBSO()
        {
            VBBSOConfigTransfer cnfTransfer = null;
            if (!IsEnabledConfigurationTransferSetSource()) return null;
            ACComponent childBSO = ACUrlCommand("?" + ConstApp.VBBSOConfigTransfer_ChildName) as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent(ConstApp.VBBSOConfigTransfer_ChildName, null, null) as ACComponent;

            if (childBSO != null)
            {
                cnfTransfer = childBSO as VBBSOConfigTransfer;
                if (!cnfTransfer.WindowOpened)
                {
                    ShowWindow(childBSO, "Mainlayout", true, Global.VBDesignContainer.DockableWindow, Global.VBDesignDockState.FloatingWindow, Global.VBDesignDockPosition.Left);
                }
                cnfTransfer.WindowOpened = true;
            }
            cnfTransfer.OnTransferComplete -= cnfTransfer_OnTransferComplete;
            cnfTransfer.OnTransferComplete += cnfTransfer_OnTransferComplete;
            return cnfTransfer;
        }

        void cnfTransfer_OnTransferComplete(IACConfigStore targetConfigStore)
        {
            Partslist targetPl = targetConfigStore as Partslist;
            if (CurrentPartslist.PartslistID == targetPl.PartslistID)
                CurrentProcessWorkflow = CurrentProcessWorkflow;
        }


        #region Config Transfer -> Soruce partslist

        private Partslist _SelectedConfigurationTransfer;
        /// <summary>
        /// 
        /// <summary>
        /// Gets or sets the selected source partslist for configuration transfer operations.
        /// This property represents the source bill of materials from which workflow parameters
        /// will be copied to the current partslist. It is used in conjunction with the
        /// ConfigurationTransferSetSource() method to enable parameter copying between partslists
        /// that share the same process workflow methods.
        /// </summary>
        /// </summary>
        [ACPropertySelected(9999, "ConfigurationTransfer", "en{'Copy WF-Parameter from:'}de{'Kopiere WF-Parameter von:'}", Description = 
                            "Gets or sets the selected source partslist for configuration transfer operations. " +
                            "This property represents the source bill of materials from which workflow parameters " +
                            "will be copied to the current partslist. It is used in conjunction with the " +
                            "ConfigurationTransferSetSource() method to enable parameter copying between partslists " +
                            "that share the same process workflow methods.")]
        public Partslist SelectedConfigurationTransfer
        {
            get
            {
                return _SelectedConfigurationTransfer;
            }
            set
            {
                if (_SelectedConfigurationTransfer != value)
                {
                    _SelectedConfigurationTransfer = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a collection of partslists that can be used as sources for configuration transfer operations.
        /// This property returns partslists that share the same process workflow methods as the current partslist,
        /// enabling parameter copying between bill of materials with compatible workflow configurations.
        /// The list is filtered based on the current process workflow and accessed through the AccessConfigurationTransfer navigation property.
        /// Returns null if no current partslist is selected or if the partslist has no associated workflow methods.
        /// </summary>
        [ACPropertyList(999, "ConfigurationTransfer", Description =
                        "Gets a collection of partslists that can be used as sources for configuration transfer operations. " +
                        "This property returns partslists that share the same process workflow methods as the current partslist, " +
                        "enabling parameter copying between bill of materials with compatible workflow configurations. " +
                        "The list is filtered based on the current process workflow and accessed through the AccessConfigurationTransfer navigation property. " +
                        "Returns null if no current partslist is selected or if the partslist has no associated workflow methods.")]
        public IEnumerable<Partslist> ConfigurationTransferList
        {
            get
            {
                if (PartslistList == null || CurrentPartslist == null || CurrentPartslist.PartslistACClassMethod_Partslist == null)
                    return null;
                return AccessConfigurationTransfer.NavList;
            }
        }


        ACAccess<Partslist> _AccessConfigurationTransfer;
        /// <summary>
        /// Gets the data access component for managing configuration transfer operations between bill of materials.
        /// This property provides a filtered query interface for selecting source partslists that can be used
        /// to copy workflow parameters and configurations to the current partslist. The access component automatically
        /// filters partslists based on enabled status and compatible process workflow methods, ensuring only
        /// valid source partslists are available for configuration transfer operations.
        /// </summary>
        [ACPropertyAccess(9999, "ConfigurationTransfer", Description =
                          "Gets the data access component for managing configuration transfer operations between bill of materials. " +
                          "This property provides a filtered query interface for selecting source partslists that can be used " +
                          "to copy workflow parameters and configurations to the current partslist. The access component automatically " +
                          "filters partslists based on enabled status and compatible process workflow methods, ensuring only " +
                          "valid source partslists are available for configuration transfer operations.")]
        public ACAccess<Partslist> AccessConfigurationTransfer
        {
            get
            {
                if (_AccessConfigurationTransfer == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "PartsParamCopy", ACType.ACIdentifier);
                    bool rebuildACQueryDef = false;
                    if (!navACQueryDefinition.ACFilterColumns.Any())
                    {
                        rebuildACQueryDef = true;
                    }
                    else
                    {
                        foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            if (filterItem.PropertyName == nameof(Partslist.IsEnabled))
                            {
                                if (!(!String.IsNullOrEmpty(filterItem.SearchWord) && filterItem.SearchWord.ToLower() == "true" && filterItem.LogicalOperator == Global.LogicalOperators.equal && filterItem.Operator == Global.Operators.and))
                                {
                                    rebuildACQueryDef = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (rebuildACQueryDef)
                    {
                        navACQueryDefinition.ClearFilter(true);
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, nameof(Partslist.IsEnabled), Global.LogicalOperators.equal, Global.Operators.and, "True", true));
                        navACQueryDefinition.SaveConfig(true);
                    }

                    _AccessConfigurationTransfer = navACQueryDefinition.NewAccessNav<Partslist>(Partslist.ClassName, this);
                    _AccessConfigurationTransfer.NavSearchExecuting += _AccessConfigurationTransfer_NavSearchExecuting;
                }
                return _AccessConfigurationTransfer;
            }
        }

        IQueryable<Partslist> _AccessConfigurationTransfer_NavSearchExecuting(IQueryable<Partslist> result)
        {
            if (CurrentPartslist != null && CurrentProcessWorkflow != null && result != null)
            {
                //List<Guid> includedMehtodIDs = CurrentPartslist.PartslistACClassMethod_Partslist.Select(x => x.MaterialWFACClassMethod.ACClassMethod.ACClassMethodID).ToList();
                //if (includedMehtodIDs.Any())
                //    result = result.Where(x => x.PartslistACClassMethod_Partslist.Select(a => a.MaterialWFACClassMethod.ACClassMethod.ACClassMethodID).Intersect(includedMehtodIDs).Any());
                result = result.Where(c => c.PartslistACClassMethod_Partslist.Where(d => d.MaterialWFACClassMethodID == CurrentProcessWorkflow.MaterialWFACClassMethodID).Any());
            }
            return result;
        }
        #endregion

        /// <summary>
        /// Initializes standard machine configuration parameters for the current bill of materials (partslist).
        /// This method applies default configuration values to the partslist's workflow parameters, 
        /// ensuring that all necessary machine and process parameters are properly set up.
        /// The initialization is performed through the PartslistManager service using standard parameter templates.
        /// This operation is typically called when setting up a new partslist or when resetting 
        /// configuration parameters to their default values.
        /// </summary>
        [ACMethodInfo("ConfigurationTransfer", "en{'Init machine parameters'}de{'Init. Maschinenparameter'}", 9999, Description =
                      "Initializes standard machine configuration parameters for the current bill of materials (partslist). " +
                      "This method applies default configuration values to the partslist's workflow parameters, " +
                      "ensuring that all necessary machine and process parameters are properly set up. " +
                      "The initialization is performed through the PartslistManager service using standard parameter templates. " +
                      "This operation is typically called when setting up a new partslist or when resetting " +
                      "configuration parameters to their default values.")]
        public void InitStandardPartslistConfigParams()
        {
            if (!IsEnabledInitStandardPartslistConfigParams()) return;
            PartslistManager.InitStandardPartslistConfigParams(CurrentPartslist, false);
        }

        /// <summary>
        /// Determines whether the initialization of standard machine configuration parameters is enabled for the current bill of materials.
        /// This method checks if a current partslist exists and has associated process workflow methods (PartslistACClassMethod_Partslist) configured.
        /// The initialization operation requires that the partslist has workflow configurations to enable parameter setup.
        /// </summary>
        /// <returns><see langword="true"/> if a current partslist exists and has workflow methods configured; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledInitStandardPartslistConfigParams()
        {
            return
                CurrentPartslist != null &&
                CurrentPartslist.PartslistACClassMethod_Partslist != null &&
                CurrentPartslist.PartslistACClassMethod_Partslist.Any();
        }

        /// <summary>
        /// Initializes standard machine configuration parameters for all bill of materials (partslists) in the current list.
        /// This method prompts the user for confirmation before proceeding with the initialization operation.
        /// If confirmed, it iterates through all partslists in the PartslistList and applies standard configuration 
        /// values to each partslist's workflow parameters using the PartslistManager service.
        /// After successful initialization of all partslists, the changes are saved to the database.
        /// This operation is typically used when setting up multiple partslists with default parameter templates
        /// or when resetting configuration parameters to their standard values across the entire collection.
        /// </summary>
        [ACMethodInfo("ConfigurationTransfer", "en{'Init all machine parameters'}de{'Init. alle Maschinenparameter'}", 9999, Description =
                      "Initializes standard machine configuration parameters for all bill of materials (partslists) in the current list. " +
                      "This method prompts the user for confirmation before proceeding with the initialization operation. " +
                      "If confirmed, it iterates through all partslists in the PartslistList and applies standard configuration  " +
                      "values to each partslist's workflow parameters using the PartslistManager service. " +
                      "After successful initialization of all partslists, the changes are saved to the database. " +
                      "This operation is typically used when setting up multiple partslists with default parameter templates " +
                      "or when resetting configuration parameters to their standard values across the entire collection.")]
        public void InitAllStandardPartslistConfigParams()
        {
            if (!IsEnabledInitAllStandardPartslistConfigParams()) return;
            ShowDialog(this, "DlgApplyAllStandardPartslistConfigParams");
        }

        /// <summary>
        /// Executes the confirmation action for initializing standard machine configuration parameters for all bill of materials (partslists) in the current list.
        /// This method closes the confirmation dialog and then iterates through all partslists in the PartslistList,
        /// applying standard configuration values to each partslist's workflow parameters using the PartslistManager service.
        /// The initialization is performed with the forceNewPartslistOnly parameter set to false, meaning it will update
        /// all partslists regardless of whether they are new or existing. This operation is typically used when setting up
        /// multiple partslists with default parameter templates or when resetting configuration parameters to their
        /// standard values across the entire collection.
        /// </summary>
        [ACMethodInfo("ConfigurationTransfer", Const.Ok, 9999, Description =
                      "Executes the confirmation action for initializing standard machine configuration parameters for all bill of materials (partslists) in the current list. " +
                      "This method closes the confirmation dialog and then iterates through all partslists in the PartslistList, " +
                      "applying standard configuration values to each partslist's workflow parameters using the PartslistManager service. " +
                      "The initialization is performed with the forceNewPartslistOnly parameter set to false, meaning it will update " +
                      "all partslists regardless of whether they are new or existing. This operation is typically used when setting up " +
                      "multiple partslists with default parameter templates or when resetting configuration parameters to their " +
                      "standard values across the entire collection.")]
        public void InitAllStandardPartslistConfigParamsOK()
        {
            if (!IsEnabledInitAllStandardPartslistConfigParams()) return;
            CloseTopDialog();
            foreach (var partsList in PartslistList)
                PartslistManager.InitStandardPartslistConfigParams(partsList, false);
        }

        /// <summary>
        /// Cancels the initialization of standard machine configuration parameters for all bill of materials.
        /// This method is called when the user clicks the Cancel button in the confirmation dialog
        /// for initializing all standard partslist configuration parameters. It simply closes the dialog
        /// without performing any initialization operations.
        /// </summary>
        [ACMethodInfo("ConfigurationTransfer", Const.Cancel, 9999, Description =
                      "Cancels the initialization of standard machine configuration parameters for all bill of materials. " +
                      "This method is called when the user clicks the Cancel button in the confirmation dialog " +
                      "for initializing all standard partslist configuration parameters. It simply closes the dialog " +
                      "without performing any initialization operations.")]
        public void InitAllStandardPartslistConfigParamsCancel()
        {
            CloseTopDialog();
        }

        /// <summary>
        /// Determines whether the initialization of standard machine configuration parameters is enabled for all bill of materials in the current list.
        /// This method checks if the PartslistList contains any partslists that can have their configuration parameters initialized.
        /// The operation requires that at least one partslist exists in the collection to enable bulk parameter initialization.
        /// </summary>
        /// <returns><see langword="true"/> if the PartslistList is not null and contains at least one partslist; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledInitAllStandardPartslistConfigParams()
        {
            return PartslistList != null && PartslistList.Count() > 0;
        }

        #endregion

        #region MDUnit

        #region MDUnit -> Selected, (Current,) List

        /// <summary>
        /// Gets a collection of units of measurement (MDUnit) that are available for the current bill of materials (partslists) material.
        /// This property returns units associated with the material assigned to the current partslist, ordered alphabetically by unit name.
        /// If the current partslist's assigned unit is not included in the material's unit list, it is automatically added to ensure consistency.
        /// Returns null if no current partslist or material is selected.
        /// </summary>
        [ACPropertyList(9999, MDUnit.ClassName, Description =
                        "Gets a collection of units of measurement (MDUnit) that are available for the current bill of materials (partslists) material. " +
                        "This property returns units associated with the material assigned to the current partslist, ordered alphabetically by unit name. " +
                        "If the current partslist's assigned unit is not included in the material's unit list, it is automatically added to ensure consistency." +
                        "Returns null if no current partslist or material is selected.")]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                if (CurrentPartslist == null || CurrentPartslist.Material == null)
                    return null;
                List<MDUnit> mdUnitList = CurrentPartslist.Material.MDUnitList.OrderBy(x => x.MDUnitName).ToList();
                if (CurrentPartslist.MDUnit != null && !mdUnitList.Any(c => c.MDUnitID == CurrentPartslist.MDUnitID))
                {
                    mdUnitList.Add(CurrentPartslist.MDUnit);
                }
                return mdUnitList;
            }
        }

        /// <summary>
        /// Gets or sets the current unit of measurement (MDUnit) for the current partslist.
        /// This property manages the unit of measurement associated with the partslist and automatically
        /// converts quantities when the unit changes. When setting a new unit, if it differs from the
        /// material's base unit, the target quantity is converted using the material's conversion factors.
        /// If the new unit matches the base unit or no material is assigned, the target quantity equals
        /// the target quantity in UOM without conversion.
        /// </summary>
        [ACPropertyCurrent(9999, MDUnit.ClassName, "en{'Unit'}de{'Einheit'}", "", true, Description =
                           "Gets or sets the current unit of measurement (MDUnit) for the current partslist. " +
                           "This property manages the unit of measurement associated with the partslist and automatically " +
                           "converts quantities when the unit changes. When setting a new unit, if it differs from the " +
                           "material's base unit, the target quantity is converted using the material's conversion factors. " +
                           "If the new unit matches the base unit or no material is assigned, the target quantity equals " +
                           "the target quantity in UOM without conversion.")]
        public MDUnit CurrentMDUnit
        {
            get
            {
                if (CurrentPartslist == null)
                    return null;
                return CurrentPartslist.MDUnit;
            }
            set
            {
                if (CurrentPartslist != null && value != CurrentPartslist.MDUnit)
                {
                    CurrentPartslist.MDUnit = value;
                    if (CurrentPartslist.MDUnit != null && CurrentPartslist.Material != null && CurrentPartslist.MDUnit != CurrentPartslist.Material.BaseMDUnit)
                    {
                        CurrentPartslist.TargetQuantity = CurrentPartslist.Material.ConvertQuantity(CurrentPartslist.TargetQuantityUOM, CurrentPartslist.Material.BaseMDUnit, CurrentPartslist.MDUnit);
                    }
                    else
                    {
                        CurrentPartslist.TargetQuantity = CurrentPartslist.TargetQuantityUOM;
                    }
                    OnPropertyChanged();
                }
            }
        }

        MaterialUnit _SelectedMaterialUnit;
        /// <summary>
        /// Gets or sets the currently selected material unit for the current partslist.
        /// This property represents the material unit selection for the currently selected partslist,
        /// providing access to unit conversion information and related material properties.
        /// The selection is automatically updated when the current partslist changes or when
        /// material unit data is refreshed.
        /// </summary>
        [ACPropertySelected(9999, "MaterialUnit", "en{'Unit'}de{'Einheit'}", "", true, Description =
                            "Gets or sets the currently selected material unit for the current partslist. " +
                            "This property represents the material unit selection for the currently selected partslist, " +
                            "providing access to unit conversion information and related material properties. " +
                            "The selection is automatically updated when the current partslist changes or when " +
                            "material unit data is refreshed.")]
        public MaterialUnit SelectedMaterialUnit
        {
            get
            {
                return _SelectedMaterialUnit;
            }
            set
            {
                if (_SelectedMaterialUnit != value)
                {
                    _SelectedMaterialUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a collection of material units associated with the current partslist's material, ordered by unit sort index.
        /// This property returns the available material units for the material assigned to the current partslist.
        /// If no current partslist exists, no material is assigned, or the material has no associated units,
        /// the property returns null and clears the selected material unit. Otherwise, it returns the material
        /// units ordered by their MDUnit sort index and automatically selects the first unit in the collection.
        /// </summary>
        [ACPropertyList(9999, "MaterialUnit", Description =
                        "Gets a collection of material units associated with the current partslist's material, ordered by unit sort index. " +
                        "This property returns the available material units for the material assigned to the current partslist. " +
                        "If no current partslist exists, no material is assigned, or the material has no associated units, " +
                        "the property returns null and clears the selected material unit. Otherwise, it returns the material " +
                        "units ordered by their MDUnit sort index and automatically selects the first unit in the collection.")]
        public IEnumerable<MaterialUnit> MaterialUnitList
        {
            get
            {
                if (CurrentPartslist == null || CurrentPartslist.Material == null || !CurrentPartslist.Material.MaterialUnit_Material.Any())
                {
                    SelectedMaterialUnit = null;
                    return null;
                }

                List<MaterialUnit> mdUnits = CurrentPartslist.Material.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).ToList();
                SelectedMaterialUnit = mdUnits.FirstOrDefault();

                return mdUnits;
            }
        }


        #endregion

        #region ProductionUnit

        #region MDUnit for Production Units

        /// <summary>
        /// Gets a collection of production units of measurement (MDUnit) available for the current partslist's material.
        /// This property returns units associated with the material assigned to the current partslist, excluding the base unit,
        /// ordered by their properties. These units are used for production quantity calculations and conversions.
        /// Returns null if no current partslist or material is selected.
        /// </summary>
        [ACPropertyList(500, "ProdUnitsMD", Description =
                        "Gets a collection of production units of measurement (MDUnit) available for the current partslist's material. " +
                        "This property returns units associated with the material assigned to the current partslist, excluding the base unit, " +
                        "ordered by their properties. These units are used for production quantity calculations and conversions. " +
                        "Returns null if no current partslist or material is selected.")]
        public IEnumerable<MDUnit> ProdUnitMDUnitList
        {
            get
            {
                if (CurrentPartslist == null || CurrentPartslist.Material == null)
                    return null;
                return CurrentPartslist.Material.MDUnitList.Where(c => c.MDUnitID != CurrentPartslist.Material.BaseMDUnitID);
            }
        }

        MDUnit _CurrentProdMDUnit;

        /// <summary>
        /// Gets or sets the current production unit of measurement (MDUnit) for the current partslist.
        /// This property represents the unit of measurement used for production quantities and is used 
        /// in conjunction with the ProductionUnits property to display and convert production values.
        /// When set, it triggers updates to the ProductionUnits property to reflect the unit conversion.
        /// </summary>
        [ACPropertyCurrent(501, "ProdUnitsMD", "en{'Units of Prod'}de{'Einheit Prod'}", "", true, Description =
                           "Gets or sets the current production unit of measurement (MDUnit) for the current partslist. " +
                           "This property represents the unit of measurement used for production quantities and is used " +
                           "in conjunction with the ProductionUnits property to display and convert production values. " +
                           "When set, it triggers updates to the ProductionUnits property to reflect the unit conversion.")]
        public MDUnit CurrentProdMDUnit
        {
            get
            {
                return _CurrentProdMDUnit;
            }
            set
            {
                _CurrentProdMDUnit = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProductionUnits));
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the production units for the current partslist in the selected unit of measurement.
        /// This property provides a converted view of the ProductionUnits value from the base unit to the currently 
        /// selected production unit (CurrentProdMDUnit). When getting, it converts the base quantity to the selected 
        /// unit using the material's conversion factors. When setting, it converts the entered value back to the base 
        /// unit and stores it in the partslist's ProductionUnits property. Returns null if no partslist is selected, 
        /// no production units are defined, or no production unit is currently selected.
        /// </summary>
        [ACPropertyInfo(502, "ProductionUnits", "en{'Units of production'}de{'Produktionseinheiten'}", "", true, Description =
                        "Gets or sets the production units for the current partslist in the selected unit of measurement. " +
                        "This property provides a converted view of the ProductionUnits value from the base unit to the currently " +
                        "selected production unit (CurrentProdMDUnit). When getting, it converts the base quantity to the selected " +
                        "unit using the material's conversion factors. When setting, it converts the entered value back to the base " +
                        "unit and stores it in the partslist's ProductionUnits property. Returns null if no partslist is selected, " +
                        "no production units are defined, or no production unit is currently selected.")]
        public double? ProductionUnits
        {
            get
            {
                if (CurrentPartslist == null
                    || !CurrentPartslist.ProductionUnits.HasValue
                    || CurrentProdMDUnit == null)
                    return null;
                if (ProdUnitMDUnitList == null || !ProdUnitMDUnitList.Contains(CurrentProdMDUnit))
                    return null;
                return CurrentPartslist.Material.ConvertFromBaseQuantity(CurrentPartslist.ProductionUnits.Value, CurrentProdMDUnit);
            }
            set
            {
                if (CurrentPartslist == null
                    || CurrentProdMDUnit == null)
                    return;
                if (ProdUnitMDUnitList == null || !ProdUnitMDUnitList.Contains(CurrentProdMDUnit))
                    return;
                if (value.HasValue)
                    CurrentPartslist.ProductionUnits = CurrentPartslist.Material.ConvertToBaseQuantity(value.Value, CurrentProdMDUnit);
                else
                    CurrentPartslist.ProductionUnits = null;
            }
        }

        #endregion

        #endregion

        #region InputMaterials

        #region InputMaterials -> Select, (Current,) List
        ACAccess<Material> _AccessInputMaterial;

        /// <summary>
        /// Gets the data access component for managing input materials in the bill of materials.
        /// This property provides access to material selection and filtering capabilities for components
        /// that can be added to the current partslist. The access component automatically initializes
        /// with a query definition and clears existing search filters to provide a comprehensive
        /// material selection interface.
        /// </summary>
        [ACPropertyAccess(9999, "InputMaterial", Description =
                          "Gets the data access component for managing input materials in the bill of materials. " +
                          "This property provides access to material selection and filtering capabilities for components " +
                          "that can be added to the current partslist. The access component automatically initializes " +
                          "with a query definition and clears existing search filters to provide a comprehensive " +
                          "material selection interface.")]
        public ACAccess<Material> AccessInputMaterial
        {
            get
            {
                if (_AccessInputMaterial == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Material.ClassName, ACType.ACIdentifier);
                    if (navACQueryDefinition.ACFilterColumns.Count > 0)
                    {
                        foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            filterItem.SearchWord = "";
                        }
                    }
                    _AccessInputMaterial = navACQueryDefinition.NewAccessNav<Material>(Material.ClassName, this);
                    _AccessInputMaterial.NavSearch();
                }
                return _AccessInputMaterial;
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
            if (acAccess == _AccessInputMaterial)
            {
                _AccessInputMaterial.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(InputMaterialList));
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }

        /// <summary>
        /// Gets a collection of all available input materials that can be added to the bill of materials.
        /// This property provides access to materials through the AccessInputMaterial data access component,
        /// which handles filtering and querying of the material database. The collection is populated when
        /// the AccessInputMaterial.NavSearch() method is executed and contains materials that can be selected
        /// as components for the current partslist. Returns null if the AccessInputMaterial is not initialized.
        /// </summary>
        [ACPropertyList(9999, "InputMaterial", Description =
                        "Gets a collection of all available input materials that can be added to the bill of materials. " +
                        "This property provides access to materials through the AccessInputMaterial data access component, " +
                        "which handles filtering and querying of the material database. The collection is populated when " +
                        "the AccessInputMaterial.NavSearch() method is executed and contains materials that can be selected " +
                        "as components for the current partslist. Returns null if the AccessInputMaterial is not initialized.")]
        public IEnumerable<Material> InputMaterialList
        {
            get
            {
                if (AccessInputMaterial == null)
                    return null;
                return AccessInputMaterial.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected input material for the selected component (PartslistPos).
        /// This property provides access to the material assigned to the currently selected parts list position.
        /// When set, it updates the material of the selected parts list position and triggers property change notifications.
        /// Returns null if no parts list position is currently selected.
        /// </summary>
        [ACPropertySelected(9999, "InputMaterial", "en{'Material'}de{'Material'}", "", true, Description =
                            "Gets or sets the currently selected input material for the selected component (PartslistPos). " +
                            "This property provides access to the material assigned to the currently selected parts list position. " +
                            "When set, it updates the material of the selected parts list position and triggers property change notifications. " +
                            "Returns null if no parts list position is currently selected.")]
        public Material SelectedInputMaterial
        {
            get
            {
                if (SelectedPartslistPos == null) return null;
                return SelectedPartslistPos.Material;
            }
            set
            {
                if (SelectedPartslistPos != null)
                {
                    SelectedPartslistPos.Material = value;
                    OnPropertyChanged(nameof(SelectedInputMaterial));
                }
            }
        }

        #endregion

        #endregion

        #region - Workflow

        #region Workflow -> private
        private VBPresenterMethod _presenter = null;

        #endregion

        #region Workflow -> Propertes
        bool _PresenterRightsChecked = false;
        /// <summary>
        /// Gets the process workflow presenter component for visualizing and interacting with workflow methods.
        /// This presenter is responsible for displaying workflow methods associated with the current partslist
        /// and provides functionality for workflow visualization, method selection, and parameter configuration.
        /// The presenter is lazily initialized and includes rights management to ensure only authorized users
        /// can access workflow viewing capabilities. If the user lacks sufficient rights, an error message
        /// is displayed and the presenter remains null.
        /// </summary>
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

        /// <summary>
        /// Gets a collection of process workflow methods (MaterialWFACClassMethod) that can be added to the current partslist.
        /// Returns workflow methods from the partslist's assigned MaterialWF that are not yet configured
        /// for this partslist. These are the available workflow methods that can be added through the
        /// AddProcessWorkflow() method. Returns an empty array if no partslist is selected, no MaterialWF
        /// is assigned, or all available workflow methods are already configured.
        /// </summary>
        public IEnumerable<MaterialWFACClassMethod> NewProcessWorkflowList
        {
            get
            {
                if (this.CurrentPartslist == null || this.CurrentPartslist.MaterialWF == null)
                    return new MaterialWFACClassMethod[0];
                else
                    return this.CurrentPartslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.ToArray().Except(ProcessWorkflowList);
            }
        }

        MaterialWFACClassMethod _NewProcessWorkflow;
        /// <summary>
        /// Gets or sets the selected process workflow method that can be added to the current partslist.
        /// This property is used in the workflow selection dialog to hold the user's choice before
        /// adding it to the partslist through the NewProcessWorkflowOk() method. When set, it triggers
        /// a property change notification to update the UI binding.
        /// </summary>
        public MaterialWFACClassMethod NewProcessWorkflow
        {
            get
            {
                return _NewProcessWorkflow;
            }
            set
            {
                _NewProcessWorkflow = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Workflow -> Methods

        private void LoadProcessWorkflows()
        {
            OnPropertyChanged(nameof(ProcessWorkflowList));
            if (ProcessWorkflowList != null && ProcessWorkflowList.Any())
                this.CurrentProcessWorkflow = this.ProcessWorkflowList.FirstOrDefault();
            else
                this.CurrentProcessWorkflow = null;
        }

        /// <summary>
        /// Handles property change events for the current partslist and updates related UI properties and data accordingly.
        /// This method is called whenever a property of the CurrentPartslist changes and performs specific actions
        /// based on the property that was modified. When the MaterialID changes, it refreshes the unit lists and
        /// sets the appropriate current unit. When ProductionUnits changes, it updates the corresponding UI property.
        /// </summary>
        /// <param name="sender">The object that raised the property changed event, typically the CurrentPartslist instance.</param>
        /// <param name="e">Event arguments containing the name of the property that changed.</param>
        public override void CurrentPartslist_PropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Partslist.MaterialID):
                    {
                        OnPropertyChanged(nameof(MDUnitList));
                        OnPropertyChanged(nameof(MaterialUnitList));
                        if (CurrentPartslist != null && CurrentPartslist.Material != null && CurrentPartslist.Material.BaseMDUnit != null)
                            CurrentMDUnit = CurrentPartslist.Material.BaseMDUnit;
                        else
                            CurrentMDUnit = null;
                        OnPropertyChanged(nameof(CurrentPartslist));
                    }
                    break;
                case nameof(Partslist.ProductionUnits):
                    OnPropertyChanged(nameof(ProductionUnits));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Opens a dialog to add a new process workflow to the current partslist.
        /// This method displays the "SelectProcessWorkflow" dialog, allowing the user to select
        /// from available workflow methods that can be added to the current partslist.
        /// The available workflows are shown in the NewProcessWorkflowList property.
        /// </summary>
        [ACMethodInteraction("ProcessWorkflow", "en{'Add'}de{'Hinzufügen'}", (short)MISort.New, true, "CurrentProcessWorkflow", Description =
                             "Opens a dialog to add a new process workflow to the current partslist. " +
                             "This method displays the SelectProcessWorkflow dialog, allowing the user to select " +
                             "from available workflow methods that can be added to the current partslist. " +
                             "The available workflows are shown in the NewProcessWorkflowList property.")]
        public void AddProcessWorkflow()
        {
            ShowDialog(this, "SelectProcessWorkflow");
        }

        /// <summary>
        /// Determines whether adding a new process workflow to the current partslist is enabled.
        /// This method checks if there are any available workflow methods that can be added to the current partslist.
        /// The operation is enabled when the NewProcessWorkflowList contains at least one workflow method
        /// that is not yet configured for the current partslist.
        /// </summary>
        /// <returns><see langword="true"/> if there are available process workflows that can be added; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledAddProcessWorkflow()
        {
            return this.NewProcessWorkflowList.Any();
        }

        /// <summary>
        /// Removes the selected process workflow from the current partslist and updates the workflow configuration.
        /// This method finds and deletes the PartslistACClassMethod entry that links the current partslist 
        /// to the selected process workflow, effectively removing the workflow from the partslist's configuration.
        /// After removal, it selects the first available workflow from the remaining process workflows and 
        /// updates the ProcessWorkflowList property to reflect the changes in the UI.
        /// </summary>
        [ACMethodInteraction("ProcessWorkflow", "en{'Remove'}de{'Entfernen'}", (short)MISort.Delete, true, "CurrentProcessWorkflow", Description =
                             "Removes the selected process workflow from the current partslist and updates the workflow configuration. " +
                             "This method finds and deletes the PartslistACClassMethod entry that links the current partslist " +
                             "to the selected process workflow, effectively removing the workflow from the partslist's configuration. " +
                             "After removal, it selects the first available workflow from the remaining process workflows and " +
                             "updates the ProcessWorkflowList property to reflect the changes in the UI.")]
        public void RemoveProcessWorkflow()
        {
            if (this.CurrentProcessWorkflow != null)
            {
                PartslistACClassMethod entry = this.CurrentPartslist.PartslistACClassMethod_Partslist.Where(c => c.MaterialWFACClassMethodID == CurrentProcessWorkflow.MaterialWFACClassMethodID).FirstOrDefault();
                if (entry != null)
                {
                    entry.DeleteACObject(this.DatabaseApp, false);
                    this.CurrentPartslist.PartslistACClassMethod_Partslist.Remove(entry);
                }
                this.CurrentProcessWorkflow = this.ProcessWorkflowList.FirstOrDefault();
            }

            OnPropertyChanged(nameof(ProcessWorkflowList));
        }

        /// <summary>
        /// Determines whether the removal of the selected process workflow from the current partslist is enabled.
        /// This method checks if a process workflow is currently selected and available for removal.
        /// The operation is enabled when a CurrentProcessWorkflow exists and is assigned to the partslist.
        /// </summary>
        /// <returns><see langword="true"/> if a process workflow is currently selected and can be removed; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledRemoveProcessWorkflow()
        {
            return CurrentProcessWorkflow != null;
        }

        /// <summary>
        /// Confirms the addition of the selected process workflow to the current partslist and closes the selection dialog.
        /// This method creates a new PartslistACClassMethod entry that links the current partslist to the selected
        /// process workflow (NewProcessWorkflow), adds it to the partslist's workflow collection, and updates the
        /// current process workflow selection. After successful addition, it closes the dialog and clears the
        /// temporary workflow selection. This method is typically called when the user confirms their workflow
        /// selection in the "SelectProcessWorkflow" dialog.
        /// </summary>
        [ACMethodCommand("NewProcessWorkflow", Const.Ok, (short)MISort.Okay, Description =
                         "Confirms the addition of the selected process workflow to the current partslist and closes the selection dialog. " +
                         "This method creates a new PartslistACClassMethod entry that links the current partslist to the selected " +
                         "process workflow (NewProcessWorkflow), adds it to the partslist's workflow collection, and updates the " +
                         "current process workflow selection. After successful addition, it closes the dialog and clears the " +
                         "temporary workflow selection. This method is typically called when the user confirms their workflow " +
                         "selection in the SelectProcessWorkflow dialog.")]
        public void NewProcessWorkflowOk()
        {
            if (!IsEnabledNewProcessWorkflowOk())
                return;
            PartslistACClassMethod item;

            item = PartslistACClassMethod.NewACObject(this.DatabaseApp, CurrentPartslist);
            item.MaterialWFACClassMethod = this.NewProcessWorkflow;
            CurrentPartslist.PartslistACClassMethod_Partslist.Add(item);
            OnPropertyChanged(nameof(ProcessWorkflowList));
            this.CurrentProcessWorkflow = this.NewProcessWorkflow;

            CloseTopDialog();
            this.NewProcessWorkflow = null;
        }

        /// <summary>
        /// Determines whether the confirmation of adding a new process workflow to the current partslist is enabled.
        /// This method checks if a process workflow is selected for addition and verifies that it is not already
        /// present in the current partslist's workflow collection. The operation is enabled when both conditions
        /// are met: a NewProcessWorkflow exists and it is not already included in the ProcessWorkflowList.
        /// </summary>
        /// <returns><see langword="true"/> if a new process workflow is selected and not already in the workflow list; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledNewProcessWorkflowOk()
        {
            return this.NewProcessWorkflow != null && !ProcessWorkflowList.Contains(NewProcessWorkflow);
        }

        /// <summary>
        /// Cancels the process workflow selection dialog and clears the temporary workflow selection.
        /// This method is called when the user clicks the Cancel button in the "SelectProcessWorkflow" dialog.
        /// It closes the dialog without adding any workflow to the partslist and resets the NewProcessWorkflow
        /// property to null to clear the temporary selection.
        /// </summary>
        [ACMethodCommand("NewProcessWorkflow", Const.Cancel, (short)MISort.Cancel, Description =
                         "Cancels the process workflow selection dialog and clears the temporary workflow selection. " +
                         "This method is called when the user clicks the Cancel button in the \"SelectProcessWorkflow\" dialog. " +
                         "It closes the dialog without adding any workflow to the partslist and resets the NewProcessWorkflow " +
                         "property to null to clear the temporary selection.")]
        public void NewProcessWorkflowCancel()
        {
            CloseTopDialog();
            this.NewProcessWorkflow = null;
        }

        #endregion

        #endregion

        #region IACConfigTransferParentBSO

        /// <summary>
        /// Gets the current configuration store for this bill of materials business object.
        /// Returns the currently selected partslist (SelectedPartslist) which implements IACConfigStore
        /// and serves as the configuration storage for workflow parameters, process settings, and other
        /// configuration data associated with the bill of materials. Returns null if no partslist is currently selected.
        /// </summary>
        public IACConfigStore SelectedItemStore
        {
            get
            {
                return SelectedPartslist;
            }
        }

        #endregion

        #region ReportConfigHelper

        /// <summary>
        /// Gets a list of report configuration wrappers for the currently selected partslist (bill of materials).
        /// This property returns configuration items grouped by their associated workflow class (ACClassWF),
        /// providing a structured view of all configuration entries for the current partslist.
        /// Returns null if no partslist is currently selected.
        /// </summary>
        [ACPropertyInfo(999, Description =
                        "Gets a list of report configuration wrappers for the currently selected partslist (bill of materials). " +
                        "This property returns configuration items grouped by their associated workflow class (ACClassWF), " +
                        "providing a structured view of all configuration entries for the current partslist. " +
                        "Returns null if no partslist is currently selected.")]
        public List<ReportConfigurationWrapper> ReportConfigs
        {
            get
            {
                return CreateReportConfigWrappers(SelectedPartslist.ConfigurationEntries);
            }
        }

        /// <summary>
        /// Gets a list of report configuration wrappers for the material workflow associated with the current partslist.
        /// This property returns configuration items for the first MaterialWFACClassMethod that matches the CurrentProcessWorkflow's ACClassMethod
        /// from the selected partslist's MaterialWF. The configurations are grouped by their associated workflow class (ACClassWF).
        /// Returns null if no partslist is selected, no MaterialWF is assigned, or no matching workflow methods are configured.
        /// </summary>
        [ACPropertyInfo(999, Description =
                        "Gets a list of report configuration wrappers for the material workflow associated with the current partslist. " +
                        "This property returns configuration items for the first MaterialWFACClassMethod that matches the CurrentProcessWorkflow's ACClassMethod " +
                        "from the selected partslist's MaterialWF. The configurations are grouped by their associated workflow class (ACClassWF). " +
                        "Returns null if no partslist is selected, no MaterialWF is assigned, or no matching workflow methods are configured.")]
        public List<ReportConfigurationWrapper> ReportConfigsMaterialWF
        {
            get
            {
                if (SelectedPartslist != null && SelectedPartslist.MaterialWF != null && SelectedPartslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.Any())
                    return CreateReportConfigWrappers(SelectedPartslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.FirstOrDefault(c => c.ACClassMethod == CurrentProcessWorkflow.ACClassMethod).ConfigurationEntries);
                return null;
            }
        }

        /// <summary>
        /// Gets a list of report configuration wrappers for the workflow configuration entries associated with the current process workflow.
        /// This property returns configuration items for the ACClassMethod that corresponds to the CurrentProcessWorkflow,
        /// providing access to workflow-specific configuration parameters and settings. The configurations are grouped by 
        /// their associated workflow class (ACClassWF) and represent the workflow method configuration entries.
        /// Returns null if no partslist is selected, no MaterialWF is assigned, no workflow methods are configured,
        /// or no matching workflow method is found for the current process workflow.
        /// </summary>
        [ACPropertyInfo(9999, Description =
                        "Gets a list of report configuration wrappers for the workflow configuration entries associated with the current process workflow. " +
                        "This property returns configuration items for the ACClassMethod that corresponds to the CurrentProcessWorkflow, " +
                        "providing access to workflow-specific configuration parameters and settings. The configurations are grouped by " +
                        "their associated workflow class (ACClassWF) and represent the workflow method configuration entries. " +
                        "Returns null if no partslist is selected, no MaterialWF is assigned, no workflow methods are configured, " +
                        "or no matching workflow method is found for the current process workflow.")]
        public List<ReportConfigurationWrapper> ReportConfigsWF
        {
            get
            {
                if (SelectedPartslist != null && SelectedPartslist.MaterialWF != null && SelectedPartslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.Any())
                    return CreateReportConfigWrappers(SelectedPartslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.FirstOrDefault(c => c.ACClassMethod == CurrentProcessWorkflow.ACClassMethod).ACClassMethod.FromIPlusContext<core.datamodel.ACClassMethod>().ConfigurationEntries);
                return null;
            }
        }

        /// <summary>
        /// Creates a list of report configuration wrappers from configuration items, grouping them by their associated workflow class.
        /// This method processes a collection of configuration items and organizes them into ReportConfigurationWrapper objects
        /// based on their associated ACClassWF (workflow class). Each wrapper contains all configuration items that belong
        /// to the same workflow class, providing a structured view for report generation and configuration management.
        /// The method handles different types of configuration items including ProdOrderPartslistConfig, PartslistConfig,
        /// MaterialWFACClassMethodConfig, and ACClassMethodConfig, extracting the appropriate workflow class from each.
        /// </summary>
        /// <param name="configItemsSource">The collection of configuration items to be processed and grouped by workflow class.</param>
        /// <returns>A list of ReportConfigurationWrapper objects, each containing configuration items grouped by their associated workflow class.</returns>
        public List<ReportConfigurationWrapper> CreateReportConfigWrappers(IEnumerable<IACConfig> configItemsSource)
        {
            List<ReportConfigurationWrapper> wrapperList = new List<ReportConfigurationWrapper>();
            foreach (var config in configItemsSource.OrderBy(c => c.PreConfigACUrl).ThenBy(x => x.LocalConfigACUrl))
            {
                core.datamodel.ACClassWF acClassWF = null;
                if (config is ProdOrderPartslistConfig)
                    acClassWF = ((ProdOrderPartslistConfig)config).VBiACClassWF.FromIPlusContext<core.datamodel.ACClassWF>();
                else if (config is PartslistConfig)
                    acClassWF = ((PartslistConfig)config).VBiACClassWF.FromIPlusContext<core.datamodel.ACClassWF>();
                else if (config is MaterialWFACClassMethodConfig)
                    acClassWF = ((MaterialWFACClassMethodConfig)config).VBiACClassWF.FromIPlusContext<core.datamodel.ACClassWF>();
                else if (config is core.datamodel.ACClassMethodConfig)
                    acClassWF = ((core.datamodel.ACClassMethodConfig)config).ACClassWF;

                ReportConfigurationWrapper wrapper = wrapperList.FirstOrDefault(c => c.ConfigACClassWF == acClassWF);
                if (wrapper != null && wrapper.ConfigACClassWF != null)
                    wrapper.ConfigItems.Add(config);
                else
                {
                    wrapper = new ReportConfigurationWrapper();
                    wrapper.ConfigACClassWF = acClassWF;
                    if (wrapper.ConfigACClassWF != null)
                    {
                        wrapper.ConfigItems.Add(config);
                        wrapperList.Add(wrapper);
                    }
                }
            }
            return wrapperList;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates the routes and production paths for the current bill of materials (partslist) to ensure producibility.
        /// This method saves any pending changes to the database before performing validation checks on the partslist's
        /// routes, workflows, and production configurations. It verifies that all necessary components, intermediate
        /// products, and process workflows are properly configured and that the partslist can be successfully
        /// produced without errors. The validation includes checking material availability, workflow connectivity,
        /// facility assignments, and other production requirements. Results are displayed to the user through
        /// message dialogs indicating success, warnings, or critical errors that would prevent production.
        /// </summary>
        [ACMethodCommand("", "en{'Check Routes'}de{'Routenprüfung'}", (short)MISort.Cancel, Description =
                         "Validates the routes and production paths for the current bill of materials (partslist) to ensure producibility. " +
                         "This method saves any pending changes to the database before performing validation checks on the partslist's " +
                         "routes, workflows, and production configurations. It verifies that all necessary components, intermediate " +
                         "products, and process workflows are properly configured and that the partslist can be successfully " +
                         "produced without errors. The validation includes checking material availability, workflow connectivity, " +
                         "facility assignments, and other production requirements. Results are displayed to the user through " +
                         "message dialogs indicating success, warnings, or critical errors that would prevent production.")]
        public void ValidateRoutes()
        {
            ACSaveChanges();
            if (!IsEnabledValidateRoutes())
                return;
            ValidateRoutesInternal();
        }

        protected MsgWithDetails ValidateRoutesInternal()
        {
            MsgWithDetails msg = null;
            using (var dbIPlus = new Database())
            {
                msg = PartslistManager.ValidateRoutes(this.DatabaseApp, dbIPlus, CurrentPartslist,
                                                            MandatoryConfigStores,
                                                            PARole.ValidationBehaviour.Laxly);
                if (msg != null)
                {
                    if (!msg.IsSucceded())
                    {
                        if (String.IsNullOrEmpty(msg.Message))
                        {
                            // Die Stückliste wäre nicht produzierbar weil:
                            msg.Message = Root.Environment.TranslateMessage(this, "Info50020");
                        }
                        Messages.Msg(msg, Global.MsgResult.OK, eMsgButton.OK);
                        return msg;
                    }
                    else if (msg.HasWarnings())
                    {
                        if (String.IsNullOrEmpty(msg.Message))
                        {
                            // Es gäbe folgende Probleme wenn Sie einen Auftrag anlegen und starten würden:
                            msg.Message = Root.Environment.TranslateMessage(this, "Info50021");
                        }
                        Messages.Msg(msg, Global.MsgResult.OK, eMsgButton.OK);
                        return msg;
                    }
                }
                // Die Routenprüfung war erflogreich. Die Stückliste ist produzierbar.
                Messages.Info(this, "Info50022");
                return msg;
            }
        }

        /// <summary>
        /// Determines whether the route validation operation is enabled for the current bill of materials.
        /// This method checks if the necessary components are available to perform route validation,
        /// including verifying that a current partslist exists and the PartslistManager service is available.
        /// Route validation ensures that the partslist can be successfully produced by checking material availability,
        /// workflow connectivity, facility assignments, and other production requirements.
        /// </summary>
        /// <returns><see langword="true"/> if route validation can be performed; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledValidateRoutes()
        {
            if (CurrentPartslist == null || this.PartslistManager == null)
                return false;
            return true;
        }

        /// <summary>
        /// Updates the target quantity UOM to zero for intermediate positions that are marked to be excluded from sum calculations.
        /// This method iterates through all partslists in the PartslistList and finds intermediate positions (InwardIntern)
        /// where the associated material has ExcludeFromSumCalc set to true. For these positions, it sets the TargetQuantityUOM
        /// to zero if it currently has a value greater than epsilon. The method returns true if any updates were made,
        /// false otherwise. This ensures that materials excluded from sum calculations don't contribute to quantity totals.
        /// </summary>
        /// <returns>True if any position quantities were updated to zero, false if no changes were made.</returns>
        public bool UpdateExcludeFromSumCalc()
        {
            bool isUpdated = false;
            foreach (Partslist pl in PartslistList)
            {
                PartslistPos[] positionsExcludedFromSum =
                    pl
                    .PartslistPos_Partslist
                    .Where(c =>
                                c.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.InwardIntern
                                && c.Material.ExcludeFromSumCalc)
                    .ToArray();

                foreach (PartslistPos pos in positionsExcludedFromSum)
                {
                    if (pos.TargetQuantityUOM > double.Epsilon)
                    {
                        pos.TargetQuantityUOM = 0;
                        isUpdated = true;
                    }
                    // Relation Quantity should not be set to zero:
                    //PartslistPosRelation[] relations = pos.PartslistPosRelation_TargetPartslistPos.ToArray();
                    //foreach (PartslistPosRelation rel in relations)
                    //{
                    //    if (rel.TargetQuantityUOM > double.Epsilon)
                    //    {
                    //        rel.TargetQuantityUOM = 0;
                    //        isUpdated = true;
                    //    }
                    //}
                }
            }
            return isUpdated;
        }

        private bool IsUpdatedExcludeFromSumCalc;
        #endregion

        #region ACObject

        ///<summary>
        ///Creates a deep copy of the current BSOPartslist instance.
        /// This method clones the base object and copies essential partslist-related properties
        /// including the selected partslist, current partslist, selected material unit, and current production unit.
        /// </summary>
        /// <returns>A new BSOPartslist instance with copied property values.</returns>
        public override object Clone()
        {
            BSOPartslist clone = base.Clone() as BSOPartslist;
            clone.SelectedPartslist = this.SelectedPartslist;
            clone.CurrentPartslist = this.CurrentPartslist;
            clone.SelectedMaterialUnit = this.SelectedMaterialUnit;
            clone.CurrentProdMDUnit = this.CurrentProdMDUnit;
            return clone;
        }

        /// <summary>
        /// ACAction is called when one IACInteractiveObject (Source) wants to inform another IACInteractiveObject (Target) about an relevant interaction-event.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public override void ACAction(ACActionArgs actionArgs)
        {
            if (actionArgs.ElementAction == Global.ElementActionType.VBDesignLoaded)
                OnVBDesignLoaded(actionArgs.DropObject.VBContent);

            else
                base.ACAction(actionArgs);
        }

        private void OnVBDesignLoaded(string vbContent)
        {
            if (vbContent == nameof(VBPresenter.SelectedRootWFNode)
                && SelectedIntermediate != null
                && MaterialWFPresenter != null)
            {
                MaterialWFPresenter.SelectMaterial(SelectedIntermediate.Material);
            }
        }

        /// <summary>
        /// Determines the control modes for UI elements based on the current state of the bill of materials (partslist) and its properties.
        /// This method evaluates the visual state and validation status of WPF controls bound to partslist properties
        /// and returns appropriate control modes to indicate whether the fields are properly filled or require attention.
        /// It checks mandatory fields like PartslistNo, PartslistName, PartslistVersion, and Material for completeness
        /// and returns EnabledWrong mode for empty required fields to provide visual feedback to the user.
        /// </summary>
        /// <param name="vbControl">The WPF control that implements IVBContent interface, containing binding information for the UI element.</param>
        /// <returns>A Global.ControlModes value indicating the visual state of the control (Enabled, EnabledWrong, etc.).</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            switch (vbControl.VBContent)
            {
                case (nameof(CurrentPartslist) + "\\" + nameof(Partslist.PartslistNo)):
                    if (CurrentPartslist != null && string.IsNullOrEmpty(CurrentPartslist.PartslistNo))
                    {
                        result = ControlModes.EnabledWrong;
                    }
                    break;
                case (nameof(CurrentPartslist) + "\\" + nameof(Partslist.PartslistName)):
                    if (CurrentPartslist != null && string.IsNullOrEmpty(CurrentPartslist.PartslistName))
                    {
                        result = ControlModes.EnabledWrong;
                    }
                    break;
                case (nameof(CurrentPartslist) + "\\" + nameof(Partslist.PartslistVersion)):
                    if (CurrentPartslist != null && string.IsNullOrEmpty(CurrentPartslist.PartslistVersion))
                    {
                        result = ControlModes.EnabledWrong;
                    }
                    break;
                case (nameof(CurrentPartslist) + "\\" + nameof(Partslist.Material)):
                    if (CurrentPartslist != null && CurrentPartslist.Material == null)
                    {
                        result = ControlModes.EnabledWrong;
                    }
                    break;
            }
            return result;
        }

        #endregion

        #region ShowParamDialog

        /// <summary>
        /// Opens the preferred parameters dialog for configuring workflow parameters of the currently selected workflow node.
        /// This method displays a dialog that allows users to view and modify preferred parameters for the selected
        /// process workflow node in the context of the current partslist. The dialog is opened through the
        /// BSOPreferredParameters child component and provides access to workflow-specific configuration settings.
        /// </summary>
        [ACMethodCommand(nameof(ShowParamDialog), ConstApp.PrefParam, 656, true, Description =
                         "Opens the preferred parameters dialog for configuring workflow parameters of the currently selected workflow node. " +
                         "This method displays a dialog that allows users to view and modify preferred parameters for the selected " +
                         "process workflow node in the context of the current partslist. The dialog is opened through the " +
                         "BSOPreferredParameters child component and provides access to workflow-specific configuration settings.")]
        public void ShowParamDialog()
        {
            if (!IsEnabledShowParamDialog())
                return;

            BSOPreferredParameters_Child.Value.ShowParamDialog(
                ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF.ACClassWFID,
                SelectedPartslist.PartslistID,
                null,
                null);
        }

        /// <summary>
        /// Determines whether the preferred parameters dialog can be opened for the currently selected workflow node.
        /// This method checks if the required components are available to display the workflow parameter configuration dialog,
        /// including verifying that a ProcessWorkflowPresenter exists, a workflow node is selected, and the node has
        /// an associated workflow class (ContentACClassWF) that can be configured.
        /// </summary>
        /// <returns><see langword="true"/> if the preferred parameters dialog can be opened; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledShowParamDialog()
        {
            return
                ProcessWorkflowPresenter != null
                && ProcessWorkflowPresenter.SelectedWFNode != null
                && ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF != null;
        }

        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(AddProcessWorkflow):
                    AddProcessWorkflow();
                    return true;
                case nameof(AlternativeDeletePartslistPos):
                    AlternativeDeletePartslistPos();
                    return true;
                case nameof(AlternativeNewPartlistPos):
                    AlternativeNewPartlistPos();
                    return true;
                case nameof(ConfigurationTransferSetSource):
                    ConfigurationTransferSetSource();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(DeleteIntermediateParts):
                    DeleteIntermediateParts();
                    return true;
                case nameof(DeletePartslistPos):
                    DeletePartslistPos();
                    return true;
                case nameof(InitAllStandardPartslistConfigParams):
                    InitAllStandardPartslistConfigParams();
                    return true;
                case nameof(InitAllStandardPartslistConfigParamsCancel):
                    InitAllStandardPartslistConfigParamsCancel();
                    return true;
                case nameof(InitAllStandardPartslistConfigParamsOK):
                    InitAllStandardPartslistConfigParamsOK();
                    return true;
                case nameof(InitStandardPartslistConfigParams):
                    InitStandardPartslistConfigParams();
                    return true;
                case nameof(IsEnabledAddProcessWorkflow):
                    result = IsEnabledAddProcessWorkflow();
                    return true;
                case nameof(IsEnabledAlternativeDeletePartslistPos):
                    result = IsEnabledAlternativeDeletePartslistPos();
                    return true;
                case nameof(IsEnabledAlternativeNewPartlistPos):
                    result = IsEnabledAlternativeNewPartlistPos();
                    return true;
                case nameof(IsEnabledConfigurationTransferSetSource):
                    result = IsEnabledConfigurationTransferSetSource();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(IsEnabledDeleteIntermediateParts):
                    result = IsEnabledDeleteIntermediateParts();
                    return true;
                case nameof(IsEnabledDeletePartslistPos):
                    result = IsEnabledDeletePartslistPos();
                    return true;
                case nameof(IsEnabledInitAllStandardPartslistConfigParams):
                    result = IsEnabledInitAllStandardPartslistConfigParams();
                    return true;
                case nameof(IsEnabledInitStandardPartslistConfigParams):
                    result = IsEnabledInitStandardPartslistConfigParams();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(IsEnabledNewIntermediateParts):
                    result = IsEnabledNewIntermediateParts();
                    return true;
                case nameof(IsEnabledNewPartslistPos):
                    result = IsEnabledNewPartslistPos();
                    return true;
                case nameof(IsEnabledNewProcessWorkflowOk):
                    result = IsEnabledNewProcessWorkflowOk();
                    return true;
                case nameof(IsEnabledNewVersion):
                    result = IsEnabledNewVersion();
                    return true;
                case nameof(IsEnabledRecalcIntermediateSum):
                    result = IsEnabledRecalcIntermediateSum();
                    return true;
                case nameof(IsEnabledRecalculateRestQuantity):
                    result = IsEnabledRecalculateRestQuantity();
                    return true;
                case nameof(IsEnabledRemoveProcessWorkflow):
                    result = IsEnabledRemoveProcessWorkflow();
                    return true;
                case nameof(IsEnabledRestore):
                    result = IsEnabledRestore();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(IsEnabledSetMaterialWF):
                    result = IsEnabledSetMaterialWF();
                    return true;
                case nameof(IsEnabledShowParamDialog):
                    result = IsEnabledShowParamDialog();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(IsEnabledUnSetMaterialWF):
                    result = IsEnabledUnSetMaterialWF();
                    return true;
                case nameof(IsEnabledUpdateAllFromMaterialWF):
                    result = IsEnabledUpdateAllFromMaterialWF();
                    return true;
                case nameof(IsEnabledUpdateFromMaterialWF):
                    result = IsEnabledUpdateFromMaterialWF();
                    return true;
                case nameof(IsEnabledValidateRoutes):
                    result = IsEnabledValidateRoutes();
                    return true;
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(NewIntermediateParts):
                    NewIntermediateParts();
                    return true;
                case nameof(NewPartslistPos):
                    NewPartslistPos();
                    return true;
                case nameof(NewProcessWorkflowCancel):
                    NewProcessWorkflowCancel();
                    return true;
                case nameof(NewProcessWorkflowOk):
                    NewProcessWorkflowOk();
                    return true;
                case nameof(NewVersion):
                    NewVersion();
                    return true;
                case nameof(RecalcIntermediateSum):
                    RecalcIntermediateSum();
                    return true;
                case nameof(RecalcRemainingQuantity):
                    RecalcRemainingQuantity();
                    return true;
                case nameof(RemoveProcessWorkflow):
                    RemoveProcessWorkflow();
                    return true;
                case nameof(Restore):
                    Restore();
                    return true;
                case nameof(Save):
                    Save();
                    return true;
                case nameof(SearchIntermediate):
                    SearchIntermediate(acParameter.Count() == 1 ? (gip.mes.datamodel.PartslistPos)acParameter[0] : null);
                    return true;
                case nameof(SetMaterialWF):
                    SetMaterialWF();
                    return true;
                case nameof(SetSelectedMaterial):
                    SetSelectedMaterial((gip.mes.datamodel.Material)acParameter[0], acParameter.Count() == 2 ? (System.Boolean)acParameter[1] : false);
                    return true;
                case nameof(ShowParamDialog):
                    ShowParamDialog();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(UnSetMaterialWF):
                    UnSetMaterialWF();
                    return true;
                case nameof(UpdateAllFromMaterialWF):
                    UpdateAllFromMaterialWF();
                    return true;
                case nameof(UpdateFromMaterialWF):
                    UpdateFromMaterialWF();
                    return true;
                case nameof(ValidateRoutes):
                    ValidateRoutes();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region Messages

        /// <summary>
        /// Clears all validation and status messages from the message list and resets the current message.
        /// This method removes all messages from the MsgList collection, notifies UI components of the change,
        /// and sets the CurrentMsg property to null to clear any selected message.
        /// Call this method before performing operations that need a clean message state or when
        /// starting new validation processes.
        /// </summary>
        public void ClearMessages()
        {
            MsgList.Clear();
            OnPropertyChanged(nameof(MsgList));
            CurrentMsg = null;
        }

        /// <summary>
        /// Sends a message to the message list for display to the user.
        /// If the message is a MsgWithDetails containing multiple detail messages, each detail message
        /// is added individually to the MsgList. Otherwise, the message is added directly to the list.
        /// After adding messages, triggers a property change notification to update the UI binding.
        /// </summary>
        /// <param name="msg">The message to be sent and displayed. Can be a simple Msg or MsgWithDetails containing multiple detail messages.</param>
        public void SendMessage(Msg msg)
        {
            if (msg is MsgWithDetails)
            {
                MsgWithDetails msgWithDetails = msg as MsgWithDetails;
                if (msgWithDetails.MsgDetails != null && msgWithDetails.MsgDetails.Any())
                {
                    foreach (Msg tmpMsg in msgWithDetails.MsgDetails)
                    {
                        MsgList.Add(tmpMsg);
                    }
                }
            }
            else
            {
                MsgList.Add(msg);
            }
            OnPropertyChanged(nameof(MsgList));
        }

        #region Messages -> Properties

        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the currently selected message from the message list for display to the user.
        /// This property represents the message that is currently highlighted or selected in the UI,
        /// allowing users to view detailed information about validation errors, warnings, or other
        /// system notifications. When set, it triggers a property change notification to update
        /// the UI binding and ensure the selected message is properly displayed.
        /// </summary>
        [ACPropertyCurrent(9999, "Message", "en{'Message'}de{'Meldung'}", Description =
                          "Gets or sets the currently selected message from the message list for display to the user. " +
                          "This property represents the message that is currently highlighted or selected in the UI, " +
                          "allowing users to view detailed information about validation errors, warnings, or other " +
                          "system notifications. When set, it triggers a property change notification to update " +
                          "the UI binding and ensure the selected message is properly displayed.")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged(nameof(CurrentMsg));
            }
        }

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets a collection of all validation and status messages for display to the user.
        /// This property provides access to messages including validation errors, warnings, and informational
        /// messages that are generated during bill of materials operations such as saving, validation, and
        /// route checking. The collection is automatically updated when new messages are added through
        /// the SendMessage() method and can be cleared using the ClearMessages() method.
        /// </summary>
        [ACPropertyList(9999, "Message", "en{'Messagelist'}de{'Meldungsliste'}", Description =
                        "Gets a collection of all validation and status messages for display to the user. " +
                        "This property provides access to messages including validation errors, warnings, and informational " +
                        "messages that are generated during bill of materials operations such as saving, validation, and " +
                        "route checking. The collection is automatically updated when new messages are added through " +
                        "the SendMessage() method and can be cleared using the ClearMessages() method.")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        #endregion

        #endregion

    }
}