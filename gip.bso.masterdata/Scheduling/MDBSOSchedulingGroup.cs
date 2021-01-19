using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.processapplication;
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
        /// <param name="content">The content.</pawp
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOSchedulingGroup(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
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
            _AvailableACClassWFList = LoadAvailableACClassWFList();
            Search();
            LoadMDSchedulingGroupWFList();
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

        #region BSO->ACProperty

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
                if (AccessPrimary == null)
                    return;
                if (AccessPrimary.Selected != value)
                {
                    AccessPrimary.Selected = value;
                    OnPropertyChanged("SelectedMDSchedulingGroup");
                    LoadMDSchedulingGroupWFList();
                }
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
        [ACPropertySelected(9999, "MDSchedulingGroupWF", "en{'TODO: MDSchedulingGroupWF'}de{'TODO: MDSchedulingGroupWF'}")]
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
        [ACPropertyList(9999, "MDSchedulingGroupWF")]
        public List<MDSchedulingGroupWF> MDSchedulingGroupWFList
        {
            get
            {
                return _MDSchedulingGroupWFList;
            }
        }

        private void LoadMDSchedulingGroupWFList()
        {
            Guid? acclasWFID = null;
            if (SelectedMDSchedulingGroup == null)
            {
                _MDSchedulingGroupWFList = null;
                _SelectedMDSchedulingGroupWF = null;
            }
            else
            {
                _MDSchedulingGroupWFList = SelectedMDSchedulingGroup.MDSchedulingGroupWF_MDSchedulingGroup.ToList();
                _SelectedMDSchedulingGroupWF = _MDSchedulingGroupWFList.FirstOrDefault();
            }
                
            if (_SelectedMDSchedulingGroupWF != null)
                acclasWFID = _SelectedMDSchedulingGroupWF.VBiACClassWFID;

            _SelectedAvailableACClassWF = null;
            if (_AvailableACClassWFList != null)
                _SelectedAvailableACClassWF = _AvailableACClassWFList.FirstOrDefault(c => acclasWFID == null || c.ACClassWFID == acclasWFID);

            OnPropertyChanged("MDSchedulingGroupWFList");
            OnPropertyChanged("SelectedMDSchedulingGroupWF");
            OnPropertyChanged("SelectedAvailableACClassWF");
        }
        #endregion

        #region 1.2.2 AvaialbleACClassWF


        #region AvailableACClassWF
        private core.datamodel.ACClassWF _SelectedAvailableACClassWF;
        /// <summary>
        /// Selected property for ACClassWF
        /// </summary>
        /// <value>The selected AvailableACClassWF</value>
        [ACPropertySelected(9999, "AvailableACClassWF", "en{'Product linie'}de{'Product linie'}")]
        public core.datamodel.ACClassWF SelectedAvailableACClassWF
        {
            get
            {
                return _SelectedAvailableACClassWF;
            }
            set
            {
                if (_SelectedAvailableACClassWF != value)
                {
                    _SelectedAvailableACClassWF = value;
                    OnPropertyChanged("SelectedAvailableACClassWF");
                }
                if (SelectedMDSchedulingGroupWF != null)
                {
                    if (SelectedMDSchedulingGroupWF.VBiACClassWFID != ((_SelectedAvailableACClassWF != null) ? _SelectedAvailableACClassWF.ACClassWFID : Guid.Empty))
                    {
                        SelectedMDSchedulingGroupWF.ACClassWF = value;
                        OnPropertyChanged("MDSchedulingGroupWFList");
                    }
                }
            }
        }


        private List<core.datamodel.ACClassWF> _AvailableACClassWFList;
        /// <summary>
        /// List property for ACClassWF
        /// </summary>
        /// <value>The AvailableACClassWF list</value>
        [ACPropertyList(9999, "AvailableACClassWF")]
        public List<core.datamodel.ACClassWF> AvailableACClassWFList
        {
            get
            {
                if (_AvailableACClassWFList == null)
                    _AvailableACClassWFList = LoadAvailableACClassWFList();
                return _AvailableACClassWFList;
            }
        }

        private List<core.datamodel.ACClassWF> LoadAvailableACClassWFList()
        {
            var query = DatabaseApp.ContextIPlus
             .ACClassWF
             .Where(c =>
                 c.RefPAACClassMethodID.HasValue
                 && c.RefPAACClassID.HasValue
                 && c.RefPAACClassMethod.ACKindIndex == (short)Global.ACKinds.MSWorkflow
                 && c.RefPAACClassMethod.PWACClass != null
                 && (c.RefPAACClassMethod.PWACClass.ACIdentifier == PWNodeProcessWorkflowVB.PWClassName
                     || c.RefPAACClassMethod.PWACClass.ACClass1_BasedOnACClass.ACIdentifier == PWNodeProcessWorkflowVB.PWClassName)
                 && !string.IsNullOrEmpty(c.Comment))
             .ToArray();
            return query.ToList();
        }
        #endregion


        #endregion

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
        [ACMethodInteraction(MDSchedulingGroup.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedTimeRangeModel", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDSchedulingGroup>(requery, () => SelectedMDSchedulingGroup, null, c => SelectedMDSchedulingGroup = c,
                        DatabaseApp
                        .MDSchedulingGroup
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
        [ACMethodInteraction(MDSchedulingGroup.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedTimeRange", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            SelectedMDSchedulingGroup = MDSchedulingGroup.NewACObject(DatabaseApp, null);
            DatabaseApp.MDSchedulingGroup.AddObject(SelectedMDSchedulingGroup);
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
        [ACMethodInteraction(MDSchedulingGroup.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentTimeRange", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = SelectedMDSchedulingGroup.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(SelectedMDSchedulingGroup);
            SelectedMDSchedulingGroup = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether this instance is enabled.
        /// </summary>
        /// <returns><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</returns>
        public bool IsEnabled()
        {
            return SelectedMDSchedulingGroup != null;
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

        [ACMethodInfo("AddMDSchedulingGroupWF", "en{'Add'}de{'Neu'}", 9999, false)]
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
        }

        public bool IsEnabledAddMDSchedulingGroupWF()
        {
            return SelectedMDSchedulingGroup != null && AvailableACClassWFList != null && AvailableACClassWFList.Any();
        }

        [ACMethodInfo("DeleteMDSchedulingGroupWF", "en{'Delete'}de{'Löschen'}", 9999, false)]
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
        }


        public bool IsEnabledDeleteMDSchedulingGroupWF()
        {
            return SelectedMDSchedulingGroupWF != null;
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
