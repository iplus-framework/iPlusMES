using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESInvoiceState, Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDInvoiceState.ClassName)]
    public class MDBSOInvoiceState : ACBSOvbNav
    {

        public const string ClassName = "MDBSOInvoiceState";

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDInvoiceState"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOInvoiceState(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDInvoiceState> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDInvoiceState.ClassName)]
        public ACAccessNav<MDInvoiceState> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDInvoiceState>(MDInvoiceState.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the state of the selected out order.
        /// </summary>
        /// <value>The state of the selected out order.</value>
        [ACPropertySelected(9999, MDInvoiceState.ClassName)]
        public MDInvoiceState SelectedMDInvoiceState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedMDInvoiceState");
            }
        }

        /// <summary>
        /// Gets or sets the state of the current out order.
        /// </summary>
        /// <value>The state of the current out order.</value>
        [ACPropertyCurrent(9999, MDInvoiceState.ClassName)]
        public MDInvoiceState CurrentMDInvoiceState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentMDInvoiceState");
            }
        }

        /// <summary>
        /// Gets the out order state list.
        /// </summary>
        /// <value>The out order state list.</value>
        [ACPropertyList(9999, MDInvoiceState.ClassName)]
        public IEnumerable<MDInvoiceState> MDInvoiceStateList
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
        [ACMethodCommand(MDInvoiceState.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(MDInvoiceState.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(MDInvoiceState.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMDInvoiceState", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDInvoiceState>(requery, () => SelectedMDInvoiceState, () => CurrentMDInvoiceState, c => CurrentMDInvoiceState = c,
                        DatabaseApp.MDInvoiceState
                        .Where(c => c.MDInvoiceStateID == SelectedMDInvoiceState.MDInvoiceStateID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedMDInvoiceState != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDInvoiceState.ClassName, Const.New, (short)MISort.New, true, "SelectedMDInvoiceState", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentMDInvoiceState = MDInvoiceState.NewACObject(DatabaseApp, null);
            DatabaseApp.MDInvoiceState.Add(CurrentMDInvoiceState);
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
        [ACMethodInteraction(MDInvoiceState.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentMDInvoiceState", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentMDInvoiceState.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentMDInvoiceState);
            SelectedMDInvoiceState = AccessPrimary.NavList.FirstOrDefault();
            Load();

            PostExecute("Delete");
            OnPropertyChanged(nameof(MDInvoiceStateList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentMDInvoiceState != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDInvoiceState.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(MDInvoiceStateList));
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