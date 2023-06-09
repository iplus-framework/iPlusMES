using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;


namespace gip.bso.masterdata
{
    /// <summary>
    /// Allgemeine Stammdatenmaske für MDBatchPlanGroup
    /// Bei den einfachen MD-Tabellen wird bewußt auf die Managerklassen verzichtet.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Batchplan group'}de{'Batchplan Gruppe'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + nameof(MDBatchPlanGroup))]
    public class MDBSOBatchPlanGroup : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOBatchPlanGroup"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOBatchPlanGroup(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MDBatchPlanGroup> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, nameof(MDBatchPlanGroup))]
        public ACAccessNav<MDBatchPlanGroup> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDBatchPlanGroup>(nameof(MDBatchPlanGroup), this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the state of the selected picking type.
        /// </summary>
        /// <value>The state of the selected picking type.</value>
        [ACPropertySelected(9999, nameof(MDBatchPlanGroup))]
        public MDBatchPlanGroup SelectedMDBatchPlanGroup
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged(nameof(SelectedMDBatchPlanGroup));
            }
        }

        /// <summary>
        /// Gets or sets the type of the current picking.
        /// </summary>
        /// <value>The type of the current picking.</value>
        [ACPropertyCurrent(9999, nameof(MDBatchPlanGroup))]
        public MDBatchPlanGroup CurrentMDBatchPlanGroup
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged(nameof(CurrentMDBatchPlanGroup));
            }
        }

        /// <summary>
        /// Gets the picking type list.
        /// </summary>
        /// <value>The picking typee list.</value>
        [ACPropertyList(9999, nameof(MDBatchPlanGroup))]
        public IEnumerable<MDBatchPlanGroup> MDBatchPlanGroupList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(nameof(MDBatchPlanGroup), "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(nameof(MDBatchPlanGroup), "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(nameof(MDBatchPlanGroup), "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMDBatchPlanGroup", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDBatchPlanGroup>(requery, () => SelectedMDBatchPlanGroup, () => CurrentMDBatchPlanGroup, c => CurrentMDBatchPlanGroup = c,
                        DatabaseApp.MDBatchPlanGroup
                        .Where(c => c.MDBatchPlanGroupID == SelectedMDBatchPlanGroup.MDBatchPlanGroupID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedMDBatchPlanGroup != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(nameof(MDBatchPlanGroup), "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedMDBatchPlanGroup", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            MDBatchPlanGroup newGroup = MDBatchPlanGroup.NewACObject(DatabaseApp, null);
            DatabaseApp.MDBatchPlanGroup.AddObject(newGroup);
            AccessPrimary.NavList.Add(newGroup);
            OnPropertyChanged(nameof(MDBatchPlanGroupList));
            CurrentMDBatchPlanGroup = newGroup;
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
        [ACMethodInteraction(nameof(MDBatchPlanGroup), "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentMDBatchPlanGroup", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!IsEnabledDelete())
                return;
            Msg msg = CurrentMDBatchPlanGroup.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            if (AccessPrimary == null) return; if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentMDBatchPlanGroup);
            SelectedMDBatchPlanGroup = AccessPrimary.NavList.FirstOrDefault();
            Load();
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentMDBatchPlanGroup != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(nameof(MDBatchPlanGroup), "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(MDBatchPlanGroupList));
        }

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
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}