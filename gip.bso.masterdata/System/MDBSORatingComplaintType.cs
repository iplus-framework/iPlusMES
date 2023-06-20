using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Complaint Type'}de{'Reklamationsart'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDRatingComplaintType.ClassName)]
    public class MDBSORatingComplaintType : ACBSOvbNav
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="MDRatingComplaintType"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSORatingComplaintType(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Primary Navigation
        public override IAccessNav AccessNav { get { return AccessPrimary; } }

        ACAccessNav<MDRatingComplaintType> _AccessPrimary;
        /// <summary>
        /// The primary Access Property for MDRatingComplaintType
        /// </summary>
        /// <value>Access Property for MDRatingComplaintType</value>
        [ACPropertyAccessPrimary(9999, "MDRatingComplaintType")]
        public ACAccessNav<MDRatingComplaintType> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDRatingComplaintType>("MDRatingComplaintType", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Selected property for MDRatingComplaintType
        /// </summary>
        /// <value>The selected PropertyName</value>
        [ACPropertySelected(9999, "MDRatingComplaintType", "en{'TODO: PropertyName'}de{'TODO: PropertyName'}")]
        public MDRatingComplaintType SelectedMDRatingComplaintType
        {
            get
            {
                return AccessPrimary.Selected;
            }
            set
            {
                AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedMDRatingComplaintType");
            }
        }

        /// <summary>
        /// Current property for MDRatingComplaintType
        /// </summary>
        /// <value>The current PropertyName</value>
        [ACPropertyCurrent(9999, "MDRatingComplaintType", "en{'TODO: PropertyName'}de{'TODO: PropertyName'}")]
        public MDRatingComplaintType CurrentMDRatingComplaintType
        {
            get
            {
                return AccessPrimary.Current;
            }
            set
            {
                if (CurrentMDRatingComplaintType == value)
                    return;

                AccessPrimary.Current = value;
                OnPropertyChanged("CurrentMDRatingComplaintType");
            }
        }

        /// <summary>
        /// List property for MDRatingComplaintType
        /// </summary>
        /// <value>The PropertyName list</value>
        [ACPropertyList(9999, "MDRatingComplaintType")]
        public IEnumerable<MDRatingComplaintType> CurrentMDRatingComplaintTypeList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("MDRatingComplaintType", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            if (OnSave())
                Search();
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
        [ACMethodCommand("MDRatingComplaintType", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction("MDRatingComplaintType", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMDRatingComplaintType", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDRatingComplaintType>(requery, () => SelectedMDRatingComplaintType, () => CurrentMDRatingComplaintType, c => CurrentMDRatingComplaintType = c,
                        DatabaseApp.MDRatingComplaintType
                        .Where(c => c.MDRatingComplaintTypeID == SelectedMDRatingComplaintType.MDRatingComplaintTypeID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedMDRatingComplaintType != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("MDRatingComplaintType", "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedMDRatingComplaintType", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentMDRatingComplaintType = MDRatingComplaintType.NewACObject(DatabaseApp, null);
            DatabaseApp.MDRatingComplaintType.Add(CurrentMDRatingComplaintType);
            AccessPrimary.NavList.Add(CurrentMDRatingComplaintType);
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
        [ACMethodInteraction("MDRatingComplaintType", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentMDRatingComplaintType", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentMDRatingComplaintType.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentMDRatingComplaintType);
            SelectedMDRatingComplaintType = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged("MDRatingComplaintTypeList");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentMDRatingComplaintType != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("MDRatingComplaintType", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("MDRatingComplaintTypeList");
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
