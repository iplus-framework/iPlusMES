using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;

namespace gip.mes.maintenance
{
    /// <summary>
    /// Allgemeine Stammdatenmaske für MDMaintOrderPropertyState
    /// Bei den einfachen MD-Tabellen wird bewußt auf die Managerklassen verzichtet.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maint. Order Prop. Status'}de{'Wartungsplan Eigenschaft-Status'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDMaintOrderPropertyState.ClassName)]
    public class MDBSOMaintOrderPropertyState : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOMaintOrderPropertyState"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOMaintOrderPropertyState(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            var b =  base.ACDeInit(deleteACClassTask);
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
        ACAccessNav<MDMaintOrderPropertyState> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "MDMaintOrderPropertyState")]
        public ACAccessNav<MDMaintOrderPropertyState> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDMaintOrderPropertyState>("MDMaintOrderPropertyState", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the state of the selected maint order property.
        /// </summary>
        /// <value>The state of the selected maint order property.</value>
        [ACPropertySelected(9999, "MDMaintOrderPropertyState")]
        public MDMaintOrderPropertyState SelectedMaintOrderPropertyState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedMaintOrderPropertyState");
            }
        }

        /// <summary>
        /// Gets or sets the state of the current maint order property.
        /// </summary>
        /// <value>The state of the current maint order property.</value>
        [ACPropertyCurrent(9999, "MDMaintOrderPropertyState")]
        public MDMaintOrderPropertyState CurrentMaintOrderPropertyState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentMaintOrderPropertyState");
            }
        }

        /// <summary>
        /// Gets the maint order property state list.
        /// </summary>
        /// <value>The maint order property state list.</value>
        [ACPropertyList(9999, "MDMaintOrderPropertyState")]
        public IEnumerable<MDMaintOrderPropertyState> MaintOrderPropertyStateList
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
        [ACMethodCommand("MDMaintOrderPropertyState", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand("MDMaintOrderPropertyState", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction("MDMaintOrderPropertyState", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMaintOrderPropertyState", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDMaintOrderPropertyState>(requery, () => SelectedMaintOrderPropertyState, () => CurrentMaintOrderPropertyState, c => CurrentMaintOrderPropertyState = c,
                        DatabaseApp.MDMaintOrderPropertyState
                        .Where(c => c.MDMaintOrderPropertyStateID == SelectedMaintOrderPropertyState.MDMaintOrderPropertyStateID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedMaintOrderPropertyState != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("MDMaintOrderPropertyState", Const.New, (short)MISort.New, true, "SelectedMaintOrderPropertyState", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentMaintOrderPropertyState = MDMaintOrderPropertyState.NewACObject(DatabaseApp, null);
            DatabaseApp.MDMaintOrderPropertyState.AddObject(CurrentMaintOrderPropertyState);
            ACState = Const.SMNew;
            PostExecute("New");

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
        [ACMethodInteraction("MDMaintOrderPropertyState", Const.Delete, (short)MISort.Delete, true, "CurrentMaintOrderPropertyState", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentMaintOrderPropertyState.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentMaintOrderPropertyState);
            SelectedMaintOrderPropertyState = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentMaintOrderPropertyState != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("MDMaintOrderPropertyState", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("MaintOrderPropertyStateList");
        }

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
                case "IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case "Search":
                    Search();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
