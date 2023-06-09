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
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using static gip.core.datamodel.Global;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Folgende alte Masken sind in diesem BSO enthalten:
    /// 1. Rezepte, Materialzusammensetzung
    /// 2. Rezepte Verarbeitungshinweise
    /// 3. Rezepte Deklaration
    /// 4. Rezepte Filme
    /// 5. Knetprogramme
    /// 6. Steuerkomponenten
    /// 7. Gruppenauswahl
    /// Neue Masken:
    /// 1. Rezepte
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
        /// ACs the init.
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);
            ACPartslistManager.DetachACRefFromServiceInstance(this, _PartslistManager);
            if (_AccessConfigurationTransfer != null)
                _AccessConfigurationTransfer.NavSearchExecuting -= _AccessConfigurationTransfer_NavSearchExecuting;

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
        [ACMethodCommand(Partslist.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            if (!PreExecute()) return;
            OnSave();
            PostExecute();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand("Partslist", "en{'Undo'}de{'Rückgängig'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Msg OnPreSave()
        {
            Msg result = null;

            IsUpdatedExcludeFromSumCalc = UpdateExcludeFromSumCalc();

            if (CurrentPartslist != null)
            {
                // Damir to sasa: Caclulation doesn't work if, TargetUOM-Quantity is set and MDUnit of Partslist is not set. If TargetQuantity is Zero, 
                // then this function sets the TargetUOM-Quantity to zero.
                if (PartslistManager != null)
                {
                    //MsgWithDetails calculationMessage = PartslistManager.CalculateUOMAndWeight(CurrentPartslist);
                    MsgWithDetails recalcMessage = PartslistManager.RecalcRemainingQuantity(CurrentPartslist);
                    //if (calculationMessage != null)
                    //    return calculationMessage;
                    if (recalcMessage != null)
                        return recalcMessage;
                }
                // @aagincic: Turn off validation of partslist for a moment
                // result = PartslistManager.Validation(CurrentPartslist);
            }
            if (!ConfigManagerIPlus.MustConfigBeReloadedOnServer(this, VisitedMethods, this.Database))
                this.VisitedMethods = null;

            var changedPartslists = ProcessLastFormulaChange();
            foreach (Partslist changedPartslist in changedPartslists)
            {
                if (!ChangedPartslists.Contains(changedPartslist))
                    ChangedPartslists.Add(changedPartslist);
            }

            ProcessChangedPartslists();

            UpdatePlanningMROrders();
            ClearChangeTracking();

            return result;
        }

        public virtual void ProcessChangedPartslists()
        {

        }

        protected override void OnPostSave()
        {
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

        protected override void OnPostUndoSave()
        {
            this.VisitedMethods = null;
            base.OnPostUndoSave();
        }


        /// <summary>
        /// Define is enabled save !!
        /// </summary>
        /// <returns></returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("Partslist", "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedPartslist", Global.ACKinds.MSMethodPrePost)]
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
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("Partslist", "en{'New BOM version'}de{'Neue Rezeptversion'}", (short)MISort.New, true, "SelectedPartslist", Global.ACKinds.MSMethodPrePost)]
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
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(Partslist.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "SelectedPartslist", Global.ACKinds.MSMethodPrePost)]
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
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentPartslist != null;
        }

        [ACMethodCommand("Restore", "en{'Restore'}de{'Wiederherstellen'}", (short)MISort.Restore, true)]
        public void Restore()
        {
            OnRestore();
        }

        public override bool IsEnabledRestore()
        {
            return base.IsEnabledRestore();
        }

        public override void OnRestore()
        {
            base.OnRestore();
            DatabaseApp.ACSaveChanges();
            Search();
            OnPropertyChanged(nameof(PartslistList));
        }

        #endregion


        /// <summary>
        /// Helper 
        /// </summary>
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
        /// Determines whether [is enabled new].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNew()
        {
            return !DatabaseApp.IsChanged;
        }

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
            UpdatePlanningMROrders(partslistsforUpdatePlanningMR);
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
        [ACMethodInteraction(Partslist.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedPartslist", Global.ACKinds.MSMethodPrePost)]
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
        /// Gets or sets the selected partslist.
        /// </summary>
        /// <value>The selected partslist.</value>
        [ACPropertySelected(9999, "PartslistPos")]
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

        public virtual void _SelectedPartslistPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PartslistPos.MaterialID))
            {
                OnPropertyChanged(nameof(SelectedPartslistPos));
            }
        }

        [ACPropertyList(9999, "PartslistPos")]
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

        [ACMethodInteraction("PartslistPos", "en{'New Component'}de{'Neue Komponente'}", (short)MISort.New, true, "SelectedPartslistPos", Global.ACKinds.MSMethodPrePost)]
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

        [ACMethodInteraction("PartslistPos", "en{'Delete Component'}de{'Komponente löschen'}", (short)MISort.Delete, true, "CurrentPartslistPos", Global.ACKinds.MSMethodPrePost)]
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
                    SelectedPartslistPos.ProdOrderPartslistPos_BasedOnPartslistPos.Clear();
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

        public bool IsEnabledNewPartslistPos()
        {
            return SelectedPartslist != null && SelectedPartslist.PartslistID != new Guid();
        }

        public bool IsEnabledDeletePartslistPos()
        {
            return SelectedPartslistPos != null && AlternativeSelectedPartslistPos == null;
        }

        #endregion

        #endregion

        #region Partslistpos -> Search (PartslistPos)

        /// <summary>
        /// /Searching partlist pos
        /// </summary>
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

        public void RefreshPos()
        {
            RefreshAlternative();
        }

        #endregion

        #endregion

        #region PartslistQueryForPartslistpos

        #region PartslistQueryForPartslistpos -> Select, (Current,) List

        /// <summary>
        /// Selected partslist wiht same material as Pos item 
        /// defined as query for production for this position
        /// ParentPartslist is PartslistPos field they define wittch Partslist is prefered for this position production1
        /// </summary>
        [ACPropertySelected(9999, "PartslistQueryForPartslistpos", "en{'Manufactured from BOM'}de{'Hergestellt aus Stückliste'}")]
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

        [ACPropertyList(9999, "PartslistQueryForPartslistpos")]
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

        #region PartslistQueryForPartslistpos -> Methods
        #endregion

        #region PartslistQueryForPartslistpos -> Search (SearchSectionTypeName)
        #endregion

        #endregion

        #region AlternativePartslistPos

        #region AlternativePartslistPos -> Select, (Current,) List

        private PartslistPos _AlternativeSelectedPartslistPos;
        /// <summary>
        /// Gets or sets the selected partslist.
        /// </summary>
        /// <value>The selected partslist.</value>
        [ACPropertySelected(9999, "AlternativePartslistpos")]
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

        [ACPropertyList(9999, "AlternativePartslistpos")]
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

        [ACMethodInteraction("AlternativeNewPartlistPos", "en{'New alternative component'}de{'Neue Alternativkomponente'}", (short)MISort.New, true, "SelectedPartslistPos", Global.ACKinds.MSMethodPrePost)]
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


        [ACMethodInteraction("AlternativeDeletePartslistPos", "en{'Delete alternative component'}de{'Alternativkomponente löschen'}", (short)MISort.Delete, true, "SelectedPartslist", Global.ACKinds.MSMethodPrePost)]
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
        public bool IsEnabledAlternativeNewPartlistPos()
        {
            return SelectedPartslistPos != null;
        }

        public bool IsEnabledAlternativeDeletePartslistPos()
        {
            return AlternativeSelectedPartslistPos != null;
        }

        #endregion

        #endregion

        #region AlternativePartslistPos -> Search (SearchSectionTypeName)

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
        [ACPropertySelected(9999, "Intermediate")]
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

        [ACPropertyList(9999, "Intermediate")]
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

        [ACMethodCommand("Intermediate", "en{'Search Bill'}de{'Suchen'}", (short)MISort.Search)]
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


        [ACMethodInteraction("IntermediateParts", "en{'Recalculate Totals'}de{'Summenberechnung'}", (short)MISort.Modify, true, "SelectedIntermediate", Global.ACKinds.MSMethodPrePost)]
        public void RecalcIntermediateSum()
        {
            PartslistManager.RecalcIntermediateSum(CurrentPartslist);
            PartslistManager.RecalcRemainingQuantity(CurrentPartslist);
            OnPropertyChanged(nameof(IntermediateList));
        }


        public bool IsEnabledRecalcIntermediateSum()
        {
            return CurrentPartslist != null;
        }
        #endregion

        #endregion

        #region IntermedateParts

        #region IntermedateParts -> Select, (Current,) List

        private PartslistPosRelation _SelectedIntermediateParts;
        [ACPropertySelected(9999, "IntermediateParts", isRightmanagement: true)]
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
        }

        [ACPropertyList(9999, "IntermediateParts")]
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

        [ACMethodInteraction("IntermediateParts", "en{'New Input'}de{'Neuer Einsatz'}", (short)MISort.New, true, "SelectedIntermediateParts", Global.ACKinds.MSMethodPrePost)]
        public void NewIntermediateParts()
        {
            if (!PreExecute()) return;
            PartslistPosRelation partslistPosRelation = new PartslistPosRelation();
            partslistPosRelation.PartslistPosRelationID = Guid.NewGuid();
            partslistPosRelation.TargetPartslistPos = SelectedIntermediate;
            partslistPosRelation.Sequence = IntermediatePartsList.Count();
            SelectedIntermediate.PartslistPosRelation_TargetPartslistPos.Add(partslistPosRelation);
            SelectedIntermediateParts = partslistPosRelation;
            OnPropertyChanged(nameof(IntermediatePartsList));
            OnPropertyChanged(nameof(PartslistPosList));
            PostExecute();
        }

        [ACMethodInteraction("IntermediateParts", "en{'Delete Input'}de{'Lösche Einsatz'}", (short)MISort.New, true, "SelectedIntermediateParts", Global.ACKinds.MSMethodPrePost)]
        public void DeleteIntermediateParts()
        {
            if (!PreExecute()) return;
            PartslistPos sourcePos = SelectedIntermediateParts.SourcePartslistPos;
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
                SequenceManager<PartslistPosRelation>.Order(IntermediatePartsList);
                SelectedIntermediateParts = IntermediatePartsList.FirstOrDefault();
                sourcePos.CalcPositionUsedCount();
                OnPropertyChanged(nameof(IntermediatePartsList));
                OnPropertyChanged(nameof(PartslistPosList));
            }
            PostExecute();
        }

        [ACMethodInteraction("IntermediateParts", "en{'Sum remaining quantitiy'}de{'Restmengen berechnen'}", (short)MISort.Modify, true, "SelectPartslistPos", Global.ACKinds.MSMethodPrePost)]
        public void RecalcRemainingQuantity()
        {
            PartslistManager.RecalcRemainingQuantity(CurrentPartslist);
            OnPropertyChanged(nameof(PartslistPosList));
        }

        #region IntermedateParts -> Methods -> IsEnabled

        public bool IsEnabledNewIntermediateParts()
        {
            return SelectedIntermediate != null;
        }

        public bool IsEnabledDeleteIntermediateParts()
        {
            bool isEnabledDelete = SelectedIntermediateParts != null;
            if (!isEnabledDelete)
                return isEnabledDelete;
            if (SelectedIntermediateParts.SourcePartslistPos != null)
                isEnabledDelete = isEnabledDelete && SelectedIntermediateParts.SourcePartslistPos.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.OutwardRoot;
            return isEnabledDelete;
        }

        public bool IsEnabledRecalculateRestQuantity()
        {
            return CurrentPartslist != null;
        }

        #endregion

        #endregion

        #region IntermedateParts -> Search (SearchSectionTypeName)

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

        #endregion

        #region MaterialWF Selection

        #region MaterialWF Selection -> Select, (Current,) List

        private MaterialWF _selectedMaterialWF;
        [ACPropertySelected(9999, MaterialWF.ClassName, "en{'Select Workflow'}de{'Workflow auswählen'}", "", true)]
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

        [ACMethodCommand(MaterialWF.ClassName, "en{'Assign Materialworkflow'}de{'Materialworkflow zuweisen'}", (short)MISort.Save, true, Global.ACKinds.MSMethodPrePost)]
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

                Save();

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

        [ACMethodCommand(MaterialWF.ClassName, "en{'Unassign Materialworkflow'}de{'Materialworkflow entfernen'}", (short)MISort.Save, true, Global.ACKinds.MSMethodPrePost)]
        public void UnSetMaterialWF()
        {
            if (!PreExecute()) return;

            Msg msg = PartslistManager.UnAssignMaterialWF(DatabaseApp, CurrentPartslist);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            Save();

            OnPropertyChanged(nameof(CurrentPartslist));
            SearchIntermediate();
            LoadMaterialWorkflows();
            PostExecute();
        }

        /// <summary>
        /// Updates partslist from material workflow, only add operation is supported.
        /// </summary>
        [ACMethodCommand(MaterialWF.ClassName, "en{'Update from Materialworkflow'}de{'Materialworkflow aktualisieren'}", 710, true)]
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
        /// Updates partslist from material workflow, only add operation is supported.
        /// </summary>
        [ACMethodCommand(MaterialWF.ClassName, "en{'Update all BOMs from Materialworkflow'}de{'Update all BOMs from Materialworkflow'}", 711, true)]
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

        public bool IsEnabledUnSetMaterialWF()
        {
            return CurrentPartslist != null && CurrentPartslist.MaterialWF != null;
        }

        public bool IsEnabledUpdateFromMaterialWF()
        {
            return CurrentPartslist != null && CurrentPartslist.MaterialWF != null;
        }

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

        [ACPropertyList(9999, "ProcessWorkflow")]
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

        [ACPropertyCurrent(9999, "ProcessWorkflow")]
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

        public void LoadMaterialWorkflows()
        {
            if (MaterialWFPresenter != null && SelectedPartslist != null)
            {
                MaterialWFPresenter.Load(SelectedPartslist.MaterialWF);
            }
        }

        [ACMethodInfo("Material", "en{'SetSelectedMaterial'}de{'SetSelectedMaterial'}", 999)]
        public void SetSelectedMaterial(Material value, bool selectPWNode = false)
        {
            if (SelectedIntermediate != null && IntermediateList != null && SelectedIntermediate.Material != value)
            {
                SelectedIntermediate = IntermediateList.FirstOrDefault(c => c.MaterialID == value.MaterialID);
            }
        }

        #endregion

        #region IACBSOConfigStoreSelection

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
        public void AddVisitedMethods(core.datamodel.ACClassMethod acClassMethod)
        {
            if (!VisitedMethods.Contains(acClassMethod))
                VisitedMethods.Add(acClassMethod);
        }

        #endregion

        #region Config Transfer

        [ACMethodInfo("ConfigurationTransfer", "en{'Next'}de{'Weiter'}", 999)]
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
        [ACPropertySelected(9999, "ConfigurationTransfer", "en{'Copy WF-Parameter from:'}de{'Kopiere WF-Parameter von:'}")]
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

        [ACPropertyList(999, "ConfigurationTransfer")]
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
        [ACPropertyAccess(9999, "ConfigurationTransfer")]
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

        [ACMethodInfo("ConfigurationTransfer", "en{'Init machine parameters'}de{'Init. Maschinenparameter'}", 999)]
        public void InitStandardPartslistConfigParams()
        {
            if (!IsEnabledInitStandardPartslistConfigParams()) return;
            PartslistManager.InitStandardPartslistConfigParams(CurrentPartslist, false);
        }

        public bool IsEnabledInitStandardPartslistConfigParams()
        {
            return
                CurrentPartslist != null &&
                CurrentPartslist.PartslistACClassMethod_Partslist != null &&
                CurrentPartslist.PartslistACClassMethod_Partslist.Any();
        }

        [ACMethodInfo("ConfigurationTransfer", "en{'Init all machine parameters'}de{'Init. alle Maschinenparameter'}", 999)]
        public void InitAllStandardPartslistConfigParams()
        {
            if (!IsEnabledInitAllStandardPartslistConfigParams()) return;
            ShowDialog(this, "DlgApplyAllStandardPartslistConfigParams");
        }

        [ACMethodInfo("ConfigurationTransfer", "en{'Ok'}de{'Ok'}", 999)]
        public void InitAllStandardPartslistConfigParamsOK()
        {
            if (!IsEnabledInitAllStandardPartslistConfigParams()) return;
            CloseTopDialog();
            foreach (var partsList in PartslistList)
                PartslistManager.InitStandardPartslistConfigParams(partsList, false);
        }

        [ACMethodInfo("ConfigurationTransfer", "en{'Cancel'}de{'Abbrechen'}", 999)]
        public void InitAllStandardPartslistConfigParamsCancel()
        {
            CloseTopDialog();
        }

        public bool IsEnabledInitAllStandardPartslistConfigParams()
        {
            return PartslistList != null && PartslistList.Count() > 0;
        }

        #endregion

        #region MDUnit

        #region MDUnit -> Selected, (Current,) List
        [ACPropertyList(9999, MDUnit.ClassName)]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                if (CurrentPartslist == null || CurrentPartslist.Material == null)
                    return null;
                return CurrentPartslist.Material.MDUnitList.OrderBy(x => x.MDUnitName);
            }
        }

        [ACPropertyCurrent(9999, MDUnit.ClassName, "en{'Unit'}de{'Einheit'}", "", true)]
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
                if (CurrentPartslist != null && value != null)
                {
                    CurrentPartslist.MDUnit = value;
                    CurrentPartslist.TargetQuantity = CurrentPartslist.Material.ConvertQuantity(CurrentPartslist.TargetQuantityUOM, CurrentPartslist.Material.BaseMDUnit, CurrentPartslist.MDUnit);
                    OnPropertyChanged();
                }
            }
        }

        MaterialUnit _SelectedMaterialUnit;
        [ACPropertySelected(9999, "MaterialUnit", "en{'Unit'}de{'Einheit'}", "", true)]
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

        [ACPropertyList(9999, "MaterialUnit")]
        public IEnumerable<MaterialUnit> MaterialUnitList
        {
            get
            {
                if (CurrentPartslist == null || CurrentPartslist.Material == null)
                {
                    SelectedMaterialUnit = null;
                    return null;
                }
                if (!CurrentPartslist.Material.MaterialUnit_Material.Any())
                    return null;
                return CurrentPartslist.Material.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).ToList();
            }
        }


        #endregion

        #region ProductionUnit

        #region MDUnit for Production Units
        [ACPropertyList(500, "ProdUnitsMD")]
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
        [ACPropertyCurrent(501, "ProdUnitsMD", "en{'Units of Prod'}de{'Einheit Prod'}", "", true)]
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


        [ACPropertyInfo(502, "ProductionUnits", "en{'Units of production'}de{'Produktionseinheiten'}", "", true)]
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
        [ACPropertyAccess(9999, "InputMaterial")]
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
        /// All available materials for input in partslist
        /// </summary>
        [ACPropertyList(9999, "InputMaterial")]
        public IEnumerable<Material> InputMaterialList
        {
            get
            {
                if (AccessInputMaterial == null)
                    return null;
                return AccessInputMaterial.NavList;
            }
        }

        [ACPropertySelected(9999, "InputMaterial", "en{'Material'}de{'Material'}", "", true)]
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

        [ACMethodInteraction("ProcessWorkflow", "en{'Add'}de{'Hinzufügen'}", (short)MISort.New, true, "CurrentProcessWorkflow")]
        public void AddProcessWorkflow()
        {
            ShowDialog(this, "SelectProcessWorkflow");
        }

        public bool IsEnabledAddProcessWorkflow()
        {
            return this.NewProcessWorkflowList.Any();
        }

        [ACMethodInteraction("ProcessWorkflow", "en{'Remove'}de{'Entfernen'}", (short)MISort.Delete, true, "CurrentProcessWorkflow")]
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

        public bool IsEnabledRemoveProcessWorkflow()
        {
            return CurrentProcessWorkflow != null;
        }


        [ACMethodCommand("NewProcessWorkflow", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
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

        public bool IsEnabledNewProcessWorkflowOk()
        {
            return this.NewProcessWorkflow != null && !ProcessWorkflowList.Contains(NewProcessWorkflow);
        }

        [ACMethodCommand("NewProcessWorkflow", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
        public void NewProcessWorkflowCancel()
        {
            CloseTopDialog();
            this.NewProcessWorkflow = null;
        }

        #endregion

        #endregion

        #region IACConfigTransferParentBSO

        public IACConfigStore SelectedItemStore
        {
            get
            {
                return SelectedPartslist;
            }
        }

        #endregion

        #region ReportConfigHelper

        [ACPropertyInfo(999)]
        public List<ReportConfigurationWrapper> ReportConfigs
        {
            get
            {
                return CreateReportConfigWrappers(SelectedPartslist.ConfigurationEntries);
            }
        }

        [ACPropertyInfo(999)]
        public List<ReportConfigurationWrapper> ReportConfigsMaterialWF
        {
            get
            {
                if (SelectedPartslist != null && SelectedPartslist.MaterialWF != null && SelectedPartslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.Any())
                    return CreateReportConfigWrappers(SelectedPartslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.FirstOrDefault(c => c.ACClassMethod == CurrentProcessWorkflow.ACClassMethod).ConfigurationEntries);
                return null;
            }
        }

        [ACPropertyInfo(999)]
        public List<ReportConfigurationWrapper> ReportConfigsWF
        {
            get
            {
                if (SelectedPartslist != null && SelectedPartslist.MaterialWF != null && SelectedPartslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.Any())
                    return CreateReportConfigWrappers(SelectedPartslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.FirstOrDefault(c => c.ACClassMethod == CurrentProcessWorkflow.ACClassMethod).ACClassMethod.FromIPlusContext<core.datamodel.ACClassMethod>().ConfigurationEntries);
                return null;
            }
        }

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
        [ACMethodCommand("", "en{'Check Routes'}de{'Routenprüfung'}", (short)MISort.Cancel)]
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

        public bool IsEnabledValidateRoutes()
        {
            if (CurrentPartslist == null || this.PartslistManager == null)
                return false;
            return true;
        }

        public bool UpdateExcludeFromSumCalc()
        {
            bool isUpdated = false;
            foreach (Partslist pl in PartslistList)
            {
                PartslistPos[] positionsExcludedFromSum = pl.PartslistPos_Partslist.Where(c => c.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.InwardIntern && c.Material.ExcludeFromSumCalc).ToArray();
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

        public override object Clone()
        {
            BSOPartslist clone = base.Clone() as BSOPartslist;
            clone.SelectedPartslist = this.SelectedPartslist;
            clone.CurrentPartslist = this.CurrentPartslist;
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

        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(RemoveProcessWorkflow):
                    RemoveProcessWorkflow();
                    return true;
                case nameof(IsEnabledRemoveProcessWorkflow):
                    result = IsEnabledRemoveProcessWorkflow();
                    return true;
                case nameof(NewProcessWorkflowOk):
                    NewProcessWorkflowOk();
                    return true;
                case nameof(IsEnabledNewProcessWorkflowOk):
                    result = IsEnabledNewProcessWorkflowOk();
                    return true;
                case nameof(NewProcessWorkflowCancel):
                    NewProcessWorkflowCancel();
                    return true;
                case nameof(ValidateRoutes):
                    ValidateRoutes();
                    return true;
                case nameof(IsEnabledValidateRoutes):
                    result = IsEnabledValidateRoutes();
                    return true;
                case nameof(Save):
                    Save();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(NewVersion):
                    NewVersion();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(Restore):
                    Restore();
                    return true;
                case nameof(IsEnabledRestore):
                    result = IsEnabledRestore();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(IsEnabledNewVersion):
                    result = IsEnabledNewVersion();
                    return true;
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(NewPartslistPos):
                    NewPartslistPos();
                    return true;
                case nameof(DeletePartslistPos):
                    DeletePartslistPos();
                    return true;
                case nameof(IsEnabledNewPartslistPos):
                    result = IsEnabledNewPartslistPos();
                    return true;
                case nameof(IsEnabledDeletePartslistPos):
                    result = IsEnabledDeletePartslistPos();
                    return true;
                case nameof(AlternativeNewPartlistPos):
                    AlternativeNewPartlistPos();
                    return true;
                case nameof(AlternativeDeletePartslistPos):
                    AlternativeDeletePartslistPos();
                    return true;
                case nameof(IsEnabledAlternativeNewPartlistPos):
                    result = IsEnabledAlternativeNewPartlistPos();
                    return true;
                case nameof(IsEnabledAlternativeDeletePartslistPos):
                    result = IsEnabledAlternativeDeletePartslistPos();
                    return true;
                case nameof(SearchIntermediate):
                    SearchIntermediate(acParameter.Count() == 1 ? (gip.mes.datamodel.PartslistPos)acParameter[0] : null);
                    return true;
                case nameof(RecalcIntermediateSum):
                    RecalcIntermediateSum();
                    return true;
                case nameof(IsEnabledRecalcIntermediateSum):
                    result = IsEnabledRecalcIntermediateSum();
                    return true;
                case nameof(NewIntermediateParts):
                    NewIntermediateParts();
                    return true;
                case nameof(DeleteIntermediateParts):
                    DeleteIntermediateParts();
                    return true;
                case nameof(RecalcRemainingQuantity):
                    RecalcRemainingQuantity();
                    return true;
                case nameof(IsEnabledNewIntermediateParts):
                    result = IsEnabledNewIntermediateParts();
                    return true;
                case nameof(IsEnabledDeleteIntermediateParts):
                    result = IsEnabledDeleteIntermediateParts();
                    return true;
                case nameof(IsEnabledRecalculateRestQuantity):
                    result = IsEnabledRecalculateRestQuantity();
                    return true;
                case nameof(SetMaterialWF):
                    SetMaterialWF();
                    return true;
                case nameof(UnSetMaterialWF):
                    UnSetMaterialWF();
                    return true;
                case nameof(UpdateFromMaterialWF):
                    UpdateFromMaterialWF();
                    return true;
                case nameof(UpdateAllFromMaterialWF):
                    UpdateAllFromMaterialWF();
                    return true;
                case nameof(IsEnabledSetMaterialWF):
                    result = IsEnabledSetMaterialWF();
                    return true;
                case nameof(IsEnabledUnSetMaterialWF):
                    result = IsEnabledUnSetMaterialWF();
                    return true;
                case nameof(IsEnabledUpdateFromMaterialWF):
                    result = IsEnabledUpdateFromMaterialWF();
                    return true;
                case nameof(IsEnabledUpdateAllFromMaterialWF):
                    result = IsEnabledUpdateAllFromMaterialWF();
                    return true;
                case nameof(SetSelectedMaterial):
                    SetSelectedMaterial((gip.mes.datamodel.Material)acParameter[0], acParameter.Count() == 2 ? (System.Boolean)acParameter[1] : false);
                    return true;
                case nameof(ConfigurationTransferSetSource):
                    ConfigurationTransferSetSource();
                    return true;
                case nameof(IsEnabledConfigurationTransferSetSource):
                    result = IsEnabledConfigurationTransferSetSource();
                    return true;
                case nameof(InitStandardPartslistConfigParams):
                    InitStandardPartslistConfigParams();
                    return true;
                case nameof(IsEnabledInitStandardPartslistConfigParams):
                    result = IsEnabledInitStandardPartslistConfigParams();
                    return true;
                case nameof(InitAllStandardPartslistConfigParams):
                    InitAllStandardPartslistConfigParams();
                    return true;
                case nameof(InitAllStandardPartslistConfigParamsOK):
                    InitAllStandardPartslistConfigParamsOK();
                    return true;
                case nameof(InitAllStandardPartslistConfigParamsCancel):
                    InitAllStandardPartslistConfigParamsCancel();
                    return true;
                case nameof(IsEnabledInitAllStandardPartslistConfigParams):
                    result = IsEnabledInitAllStandardPartslistConfigParams();
                    return true;
                case nameof(AddProcessWorkflow):
                    AddProcessWorkflow();
                    return true;
                case nameof(IsEnabledAddProcessWorkflow):
                    result = IsEnabledAddProcessWorkflow();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}