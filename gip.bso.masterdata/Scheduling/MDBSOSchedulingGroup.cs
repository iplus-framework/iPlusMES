using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;


namespace gip.bso.masterdata.Scheduling
{
    [ACClassInfo(Const.PackName_VarioScheduling, "en{'Scheduling Group'}de{'Planungsgruppe'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDSchedulingGroup.ClassName)]
    public class MDBSOSchedulingGroup : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOTimeRange"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOSchedulingGroup(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
            _ProcessWFClassName = new ACPropertyConfigValue<string>(this, "ProcessWFClassName", "PWNodeProcessWorkflowVB"); // PWNodeProcessWorkflowVB.PWClassName
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

            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }
        #endregion

        #region ChildBSO

        ACChildItem<BSOFacilityExplorer> _BSOFacilityExplorer_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo("BSOFacilityExplorer_Child", typeof(BSOFacilityExplorer))]
        public ACChildItem<BSOFacilityExplorer> BSOFacilityExplorer_Child
        {
            get
            {
                if (_BSOFacilityExplorer_Child == null)
                    _BSOFacilityExplorer_Child = new ACChildItem<BSOFacilityExplorer>(this, "BSOFacilityExplorer_Child");
                return _BSOFacilityExplorer_Child;
            }
        }

        #endregion


        #region BSO->ACProperty

        #region Config
        private ACPropertyConfigValue<string> _ProcessWFClassName;
        [ACPropertyConfig("ProcessWFClassName")]
        public string ProcessWFClassName
        {
            get
            {
                return _ProcessWFClassName.ValueT;
            }
            set
            {
                _ProcessWFClassName.ValueT = value;
            }
        }
        #endregion

        #region 1.1 MDSchedulingGroup
        public override IAccessNav AccessNav { get { return AccessPrimary; } }


        ACAccessNav<MDSchedulingGroup> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDSchedulingGroup.ClassName)]
        public ACAccessNav<MDSchedulingGroup> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDSchedulingGroup>(MDSchedulingGroup.ClassName, this);
                }
                return _AccessPrimary;
            }
        }


        /// <summary>
        /// Gets or sets the selected time range model.
        /// </summary>
        /// <value>The selected time range model.</value>
        [ACPropertySelected(9999, MDSchedulingGroup.ClassName)]
        public MDSchedulingGroup SelectedMDSchedulingGroup
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return;
                if (AccessPrimary.Selected != value)
                {
                    AccessPrimary.Selected = value;
                    OnPropertyChanged(nameof(SelectedMDSchedulingGroup));
                    ConnectedFacilityList = LoadConnectedFacilityList();
                    SelectedConnectedFacility = ConnectedFacilityList.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the current out order.
        /// </summary>
        /// <value>The type of the current out order.</value>
        [ACPropertyCurrent(9999, MDSchedulingGroup.ClassName)]
        public MDSchedulingGroup CurrentMDSchedulingGroup
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return;
                AccessPrimary.Current = value;
                LoadMDSchedulingGroupWFList();
                LoadAvailableACClassWFList();
                OnPropertyChanged("CurrentMDSchedulingGroup");
            }
        }

        /// <summary>
        /// Gets the time range list.
        /// </summary>
        /// <value>The time range list.</value>
        [ACPropertyList(9999, MDSchedulingGroup.ClassName)]
        public IEnumerable<MDSchedulingGroup> MDSchedulingGroupList
        {
            get
            {
                if (AccessNav == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region 1.2 MDSchedulingGroupWF

        #region 1.2.1 MDSchedulingGroupWF -> MDSchedulingGroupWF

        private MDSchedulingGroupWF _SelectedMDSchedulingGroupWF;
        /// <summary>
        /// Selected property for MDSchedulingGroupWF
        /// </summary>
        /// <value>The selected MDSchedulingGroupWF</value>
        [ACPropertySelected(9999, MDSchedulingGroupWF.ClassName, "en{'TODO: MDSchedulingGroupWF'}de{'TODO: MDSchedulingGroupWF'}")]
        public MDSchedulingGroupWF SelectedMDSchedulingGroupWF
        {
            get
            {
                return _SelectedMDSchedulingGroupWF;
            }
            set
            {
                if (_SelectedMDSchedulingGroupWF != value)
                {
                    _SelectedMDSchedulingGroupWF = value;
                    OnPropertyChanged("SelectedMDSchedulingGroupWF");
                }
            }
        }

        private List<MDSchedulingGroupWF> _MDSchedulingGroupWFList;
        /// <summary>
        /// List property for MDSchedulingGroupWF
        /// </summary>
        /// <value>The MDSchedulingGroupWF list</value>
        [ACPropertyList(9999, MDSchedulingGroupWF.ClassName)]
        public List<MDSchedulingGroupWF> MDSchedulingGroupWFList
        {
            get
            {
                return _MDSchedulingGroupWFList;
            }
        }

        private void LoadMDSchedulingGroupWFList()
        {
            if (SelectedMDSchedulingGroup == null)
            {
                _MDSchedulingGroupWFList = null;
                _SelectedMDSchedulingGroupWF = null;
            }
            else
            {
                _MDSchedulingGroupWFList = SelectedMDSchedulingGroup.Context.Entry(SelectedMDSchedulingGroup)
                                                    .Collection(c => c.MDSchedulingGroupWF_MDSchedulingGroup)
                                                    .Query()
                                                    .Include(c => c.VBiACClassWF)
                                                    .Include(c => c.VBiACClassWF.ACClassMethod)
                                                    .AsEnumerable().OrderBy(c => c.ACClassWF.ACCaption).ToList();
                //_MDSchedulingGroupWFList = SelectedMDSchedulingGroup.MDSchedulingGroupWF_MDSchedulingGroup.AsEnumerable().OrderBy(c => c.ACClassWF.ACCaption).ToList();
                _SelectedMDSchedulingGroupWF = _MDSchedulingGroupWFList.FirstOrDefault();
            }

            OnPropertyChanged("MDSchedulingGroupWFList");
            OnPropertyChanged("SelectedMDSchedulingGroupWF");
        }
        #endregion

        #region 1.2.2 AvaialbleACClassWF

        ACAccessNav<core.datamodel.ACClassWF> _AccessFilterAvailableACClassWF;
        [ACPropertyAccess(100, "AvailableACClassWF")]
        public ACAccessNav<core.datamodel.ACClassWF> AccessAvailableACClassWF
        {
            get
            {
                if (_AccessFilterAvailableACClassWF == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + core.datamodel.ACClassWF.ClassName, "ACCaption");
                    _AccessFilterAvailableACClassWF = navACQueryDefinition.NewAccessNav<core.datamodel.ACClassWF>("AvailableACClassWF", this);
                    _AccessFilterAvailableACClassWF.NavSearchExecuting += _AccessFilterAvailableACClassWF_NavSearchExecuting;
                    _AccessFilterAvailableACClassWF.AutoSaveOnNavigation = false;
                }
                return _AccessFilterAvailableACClassWF;
            }
        }

        private IQueryable<core.datamodel.ACClassWF> _AccessFilterAvailableACClassWF_NavSearchExecuting(IQueryable<core.datamodel.ACClassWF> result)
        {
            if (result != null)
            {
                List<Guid> assignedWorkflowNodeIds = new List<Guid>();
                if (_MDSchedulingGroupWFList != null)
                    assignedWorkflowNodeIds = _MDSchedulingGroupWFList.Select(c => c.VBiACClassWFID).ToList();
                IQueryable<core.datamodel.ACClassWF> query = result as IQueryable<core.datamodel.ACClassWF>;
                if (query != null)
                {
                    result = query.Include(c => c.ACClassMethod)
                        .Where(c =>
                     c.RefPAACClassMethodID.HasValue
                     && c.RefPAACClassID.HasValue
                     && c.RefPAACClassMethod.ACKindIndex == (short)Global.ACKinds.MSWorkflow
                     && c.RefPAACClassMethod.PWACClass != null
                     && (c.RefPAACClassMethod.PWACClass.ACIdentifier == ProcessWFClassName
                         || c.RefPAACClassMethod.PWACClass.ACClass1_BasedOnACClass.ACIdentifier == ProcessWFClassName)
                     && !string.IsNullOrEmpty(c.Comment)
                     && !assignedWorkflowNodeIds.Contains(c.ACClassWFID));
                }
            }
            return result;
        }

        /// <summary>
        /// Gets or sets the selected FilterPropertyName.
        /// </summary>
        /// <value>The selected FilterPropertyName.</value>
        [ACPropertySelected(101, "AvailableACClassWF")]
        public core.datamodel.ACClassWF SelectedAvailableACClassWF
        {
            get
            {
                if (AccessAvailableACClassWF == null)
                    return null;
                return AccessAvailableACClassWF.Selected;
            }
            set
            {
                if (AccessAvailableACClassWF == null)
                    return;
                AccessAvailableACClassWF.Selected = value;
                OnPropertyChanged("SelectedAvailableACClassWF");
            }
        }

        /// <summary>
        /// Gets the FilterPropertyName list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(102, "AvailableACClassWF")]
        public IEnumerable<core.datamodel.ACClassWF> AvailableACClassWFList
        {
            get
            {
                if (AccessAvailableACClassWF == null)
                    return null;
                return AccessAvailableACClassWF.NavList;
            }
        }

        public void LoadAvailableACClassWFList()
        {
            AccessAvailableACClassWF.NavSearch();
            OnPropertyChanged("AvailableACClassWFList");
        }

        #endregion

        #endregion

        #region 1.3 ConnectedFacility (FacilityMDSchedulingGroup)

        private FacilityMDSchedulingGroup _SelectedConnectedFacility;
        /// <summary>
        /// Selected property for FacilityMDSchedulingGroup
        /// </summary>
        /// <value>The selected ConnectedFacility</value>
        [ACPropertySelected(9999, "ConnectedFacility", "en{'TODO: ConnectedFacility'}de{'TODO: ConnectedFacility'}")]
        public FacilityMDSchedulingGroup SelectedConnectedFacility
        {
            get
            {
                return _SelectedConnectedFacility;
            }
            set
            {
                if (_SelectedConnectedFacility != value)
                {
                    _SelectedConnectedFacility = value;
                    OnPropertyChanged(nameof(SelectedConnectedFacility));
                }
            }
        }


        private List<FacilityMDSchedulingGroup> _ConnectedFacilityList;
        /// <summary>
        /// List property for FacilityMDSchedulingGroup
        /// </summary>
        /// <value>The ConnectedFacility list</value>
        [ACPropertyList(9999, "ConnectedFacility")]
        public List<FacilityMDSchedulingGroup> ConnectedFacilityList
        {
            get
            {
                if (_ConnectedFacilityList == null)
                    _ConnectedFacilityList = LoadConnectedFacilityList();
                return _ConnectedFacilityList;
            }
            set
            {
                _ConnectedFacilityList = value;
                OnPropertyChanged(nameof(ConnectedFacilityList));
            }
        }

        private List<FacilityMDSchedulingGroup> LoadConnectedFacilityList()
        {
            if (SelectedMDSchedulingGroup == null)
                return new List<FacilityMDSchedulingGroup>();
            return
                SelectedMDSchedulingGroup
                .FacilityMDSchedulingGroup_MDSchedulingGroup
                .OrderBy(c => c.Facility.FacilityNo)
                .ToList();
        }


        #endregion

        #endregion

        #region BSO->ACMethod

        #region 1. MDSchedulingGroup

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("MDSchedulingGroup", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
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
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand(MDSchedulingGroup.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction(MDSchedulingGroup.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMDSchedulingGroup", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDSchedulingGroup>(requery, () => SelectedMDSchedulingGroup, () => CurrentMDSchedulingGroup, c => CurrentMDSchedulingGroup = c,
                        DatabaseApp.MDSchedulingGroup
                        .Where(c => c.MDSchedulingGroupID == SelectedMDSchedulingGroup.MDSchedulingGroupID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedMDSchedulingGroup != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDSchedulingGroup.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedMDSchedulingGroup", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            MDSchedulingGroup mDSchedulingGroup = MDSchedulingGroup.NewACObject(DatabaseApp, null);
            DatabaseApp.MDSchedulingGroup.Add(mDSchedulingGroup);
            AccessPrimary.NavList.Add(mDSchedulingGroup);
            CurrentMDSchedulingGroup = mDSchedulingGroup;
            OnPropertyChanged("MDSchedulingGroupList");
            ACState = Const.SMNew;
            PostExecute("Neu");

        }

        /// <summary>
        /// Determines whether [is enabled new].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNew()
        {
            return true;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(MDSchedulingGroup.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentMDSchedulingGroup", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentMDSchedulingGroup.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentMDSchedulingGroup);
            CurrentMDSchedulingGroup = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether this instance is enabled.
        /// </summary>
        /// <returns><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</returns>
        public bool IsEnabled()
        {
            return CurrentMDSchedulingGroup != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDSchedulingGroup.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("MDSchedulingGroupList");
        }

        #endregion

        #region 2. MDSchedulingGroupWF

        [ACMethodInfo("AddMDSchedulingGroupWF", "en{'>'}de{'>'}", 9999, false)]
        public void AddMDSchedulingGroupWF()
        {
            if (!IsEnabledAddMDSchedulingGroupWF())
                return;
            MDSchedulingGroupWF newWf = MDSchedulingGroupWF.NewACObject(DatabaseApp, SelectedMDSchedulingGroup);
            if (_MDSchedulingGroupWFList == null)
                _MDSchedulingGroupWFList = new List<MDSchedulingGroupWF>();
            newWf.ACClassWF = SelectedAvailableACClassWF;
            MDSchedulingGroupWFList.Add(newWf);
            SelectedMDSchedulingGroupWF = newWf;

            OnPropertyChanged("MDSchedulingGroupWFList");
            LoadAvailableACClassWFList();
        }

        public bool IsEnabledAddMDSchedulingGroupWF()
        {
            return
                SelectedMDSchedulingGroup != null
                && AvailableACClassWFList != null
                && AvailableACClassWFList.Any();
        }

        [ACMethodInfo("DeleteMDSchedulingGroupWF", "en{'<'}de{'<'}", 9999, false)]
        public void DeleteMDSchedulingGroupWF()
        {
            if (!IsEnabledDeleteMDSchedulingGroupWF())
                return;
            MDSchedulingGroupWF item = SelectedMDSchedulingGroupWF;
            MsgWithDetails msg = item.DeleteACObject(DatabaseApp, false);
            MDSchedulingGroupWFList.Remove(item);
            if (msg == null || msg.IsSucceded())
                SelectedMDSchedulingGroupWF = MDSchedulingGroupWFList.FirstOrDefault();
            else
                MDSchedulingGroupWFList.Add(item);

            OnPropertyChanged("MDSchedulingGroupWFList");
            LoadAvailableACClassWFList();
        }


        public bool IsEnabledDeleteMDSchedulingGroupWF()
        {
            return SelectedMDSchedulingGroupWF != null;
        }
        #endregion

        #region 1.3 ConnectedFacility (FacilityMDSchedulingGroup)

        /// <summary>
        /// Source Property: AddFacility
        /// </summary>
        [ACMethodInfo("AddFacility", "en{'Add'}de{'Neu'}", 999)]
        public void AddFacility()
        {
            if (!IsEnabledAddFacility())
                return;

            FacilityMDSchedulingGroup facilityMDSchedulingGroup = FacilityMDSchedulingGroup.NewACObject(DatabaseApp, null);
            facilityMDSchedulingGroup.MDSchedulingGroup = SelectedMDSchedulingGroup;
            SelectedMDSchedulingGroup.FacilityMDSchedulingGroup_MDSchedulingGroup.Add(facilityMDSchedulingGroup);
            ConnectedFacilityList.Add(facilityMDSchedulingGroup);
            OnPropertyChanged(nameof(ConnectedFacilityList));

            SelectedConnectedFacility = facilityMDSchedulingGroup;
        }

        public bool IsEnabledAddFacility()
        {
            return SelectedMDSchedulingGroup != null;
        }


        /// <summary>
        /// Source Property: DeleteFacility
        /// </summary>
        [ACMethodInfo("DeleteFacility", "en{'Delete'}de{'Löschen'}", 999)]
        public void DeleteFacility()
        {
            if (!IsEnabledDeleteFacility())
                return;

            SelectedMDSchedulingGroup.FacilityMDSchedulingGroup_MDSchedulingGroup.Remove(SelectedConnectedFacility);
            ConnectedFacilityList.Remove(SelectedConnectedFacility);

            SelectedConnectedFacility.DeleteACObject(DatabaseApp, false);

            OnPropertyChanged(nameof(ConnectedFacilityList));
        }

        public bool IsEnabledDeleteFacility()
        {
            return SelectedConnectedFacility != null;
        }

        /// <summary>
        /// Source Property: ShowDlgInwardFacility
        /// </summary>
        [ACMethodInfo("ShowFacility", "en{'Choose facility'}de{'Lager auswählen'}", 999)]
        public void ShowFacility()
        {
            if (!IsEnabledShowFacility())
                return;

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(SelectedConnectedFacility?.Facility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                SelectedConnectedFacility.Facility = facility;
                OnPropertyChanged(nameof(SelectedConnectedFacility));
            }
        }

        public bool IsEnabledShowFacility()
        {
            return SelectedConnectedFacility != null;
        }

        #endregion

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "Save":
                    Save();
                    return true;
                case "IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
                case "UndoSave":
                    UndoSave();
                    return true;
                case "IsEnabledUndoSave":
                    result = IsEnabledUndoSave();
                    return true;
                case "Load":
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case "IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case "New":
                    New();
                    return true;
                case "IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
                case "Delete":
                    Delete();
                    return true;
                case "IsEnabled":
                    result = IsEnabled();
                    return true;
                case "Search":
                    Search();
                    return true;
                case "AddMDSchedulingGroupWF":
                    AddMDSchedulingGroupWF();
                    return true;
                case "IsEnabledAddMDSchedulingGroupWF":
                    result = IsEnabledAddMDSchedulingGroupWF();
                    return true;
                case "DeleteMDSchedulingGroupWF":
                    DeleteMDSchedulingGroupWF();
                    return true;
                case "IsEnabledDeleteMDSchedulingGroupWF":
                    result = IsEnabledDeleteMDSchedulingGroupWF();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
