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
    /// Allgemeine Stammdatenmaske für MDMaintMode
    /// Bei den einfachen MD-Tabellen wird bewußt auf die Managerklassen verzichtet.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance Mode'}de{'Wartungsmodus'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDMaintMode.ClassName)]
    public class MDBSOMaintMode : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOMaintMode"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOMaintMode(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDMaintMode> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        /// <summary xml:lang="de">
        /// Ruft AccessPrimary ab. 
        /// </summary>
        [ACPropertyAccessPrimary(9999, "MDMaintMode")]
        public ACAccessNav<MDMaintMode> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDMaintMode>("MDMaintMode", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected maint mode.
        /// </summary>
        /// <value>The selected maint mode.</value>
        /// <summary>
        /// Liest oder setzt den ausgewählten Wartungsmodus.
        /// </summary>
        [ACPropertySelected(9999, "MDMaintMode")]
        public MDMaintMode SelectedMaintMode
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedMaintMode");
            }
        }

        /// <summary>
        /// Gets or sets the current maint mode.
        /// </summary>
        /// <value>The current maint mode.</value>
        /// <summary xml:lang="de">
        /// Liest oder setzt den aktuellen Wartungsmodus. 
        /// </summary>
        [ACPropertyCurrent(9999, "MDMaintMode")]
        public MDMaintMode CurrentMaintMode
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentMaintMode");
            }
        }

        /// <summary>
        /// Gets the maint mode list.
        /// </summary>
        /// <value>The maint mode list.</value>
        /// <summary xml:lang="de">
        /// Ruft die Liste der Pflegemodi ab. 
        /// </summary>
        [ACPropertyList(9999, "MDMaintMode")]
        public IEnumerable<MDMaintMode> MaintModeList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves the changes on the current maintenance mode. 
        /// </summary>
        /// <summary xml:lang="de">
        /// Speichert die Änderungen im aktuellen Wartungsmodus. 
        /// </summary>
        [ACMethodCommand("MDMaintMode", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        /// <summary xml:lang="de">
        /// Setzt die Änderungen in den vorherigen Zustand zurück. 
        /// </summary>
        [ACMethodCommand("MDMaintMode", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        /// Loads the selected/current maintenance mode from the database. 
        /// </summary>
        /// <summary xml:lang="de">
        /// Lädt den selektierten/aktuellen Wartungsmodus von der Datenbank. 
        /// </summary>
        [ACMethodInteraction("MDMaintMode", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMaintMode", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDMaintMode>(requery, () => SelectedMaintMode, () => CurrentMaintMode, c => CurrentMaintMode = c,
                        DatabaseApp.MDMaintMode
                        .Where(c => c.MDMaintModeID == SelectedMaintMode.MDMaintModeID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedMaintMode != null;
        }

        /// <summary>
        /// Creates a new maintenance mode. 
        /// </summary>
        /// <summary xml:lang="de">
        /// Legt einen neuen Wartungsmodus an. 
        /// </summary>
        [ACMethodInteraction("MDMaintMode", "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedMaintMode", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentMaintMode = MDMaintMode.NewACObject(DatabaseApp, null);
            DatabaseApp.MDMaintMode.Add(CurrentMaintMode);
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
        /// Deletes the selected/current maintenance mode. 
        /// </summary>
        /// <summary xml:lang="de">
        /// Löscht den ausgewählten/aktuellen Wartungsmodus. 
        /// </summary>
        [ACMethodInteraction("MDMaintMode", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentMaintMode", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentMaintMode.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentMaintMode);
            SelectedMaintMode = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentMaintMode != null;
        }

        /// <summary>
        /// Executes a search operation on the MaintMode list. 
        /// </summary>
        /// <summary xml:lang="de">
        /// Führt einen Suchvorgang in der MaintMode-Liste aus. 
        /// </summary>
        [ACMethodCommand("MDMaintMode", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("MaintModeList");
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
