using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using gip.core.manager;
using gip.mes.datamodel; using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;

namespace gip.bso.sales
{
    /// <summary>
    /// Businessobject or App for the order summary (order positions overview).
    /// It shows a list of order positions (OutOrderPos) with order number, customer, material,
    /// target delivery date and priority, and target/actual quantities and weights.
    /// To search records enter the search string in the SearchWord property.
    /// The database result is copied to the OutOrderPosList property.
    /// Then call the NavigateFirst() method to set CurrentOutOrderPos with the first record in the list.
    /// CurrentOutOrderPos is used to display and edit the currently selected order position.
    /// Property changes should always be made to CurrentOutOrderPos and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The SelectedOutOrderPos property holds the record selected in the list; the Load() method re-reads the selected record from the database if another user has made changes in the background.
    /// The UndoSave() method discards unsaved changes of the current record.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOOutOrderOverview&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Order Summary'}de{'Auftragsübersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOrderPos.ClassName,
        Description = @"Businessobject or App for the order summary (order positions overview).
        It shows a list of order positions (OutOrderPos) with order number, customer, material, target delivery date and priority, and target/actual quantities and weights.
        To search records enter the search string in the SearchWord property.
        The database result is copied to the OutOrderPosList property.
        Then call the NavigateFirst() method to set CurrentOutOrderPos with the first record in the list.
        CurrentOutOrderPos is used to display and edit the currently selected order position.
        Property changes should always be made to CurrentOutOrderPos and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The SelectedOutOrderPos property holds the record selected in the list; the Load() method re-reads the selected record from the database if another user has made changes in the background.
        The UndoSave() method discards unsaved changes of the current record.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOOutOrderOverview&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOOutOrderOverview : ACBSOvbNav 
    {
        #region c´tors

        public BSOOutOrderOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            var x1 = DatabaseApp.MDUnit.ToList();
            var x2 = DatabaseApp.Material.ToList();
            var x3 = DatabaseApp.OutOrder.ToList();
            var x4 = DatabaseApp.Company.ToList();

            Search();
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        ACAccessNav<OutOrderPos> _AccessPrimary;
        [ACPropertyAccess(690, OutOrderPos.ClassName, Description = "Primary navigation access to the OutOrderPos records of the order summary.")]
        public ACAccessNav<OutOrderPos> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<OutOrderPos>(OutOrderPos.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        [ACPropertyCurrent(600, OutOrderPos.ClassName, Description = "Currently loaded order position; edit this object and call Save().")]
        public OutOrderPos CurrentOutOrderPos
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentOutOrderPos");
                OnPropertyChanged("CurrentOutOrder");
            }
        }

        [ACPropertyList(601, OutOrderPos.ClassName, Description = "Result list of order positions shown in the overview grid.")]
        public IEnumerable<OutOrderPos> OutOrderPosList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, OutOrderPos.ClassName, Description = "Order position currently selected in the list.")]
        public OutOrderPos SelectedOutOrderPos
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedOutOrderPos");
            }
        }
        #endregion

        #region BSO->ACMethod
        [ACMethodCommand(OutOrderPos.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves the changes of the current order position to the database.")]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(OutOrderPos.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards the unsaved changes of the current order position.")]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction(OutOrderPos.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedOutOrderPos", Global.ACKinds.MSMethodPrePost, Description = "Re-reads the selected order position from the database, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<OutOrderPos>(requery, () => SelectedOutOrderPos, () => CurrentOutOrderPos, c => CurrentOutOrderPos = c,
                        DatabaseApp.OutOrderPos
                        .Where(c => c.OutOrderPosID == SelectedOutOrderPos.OutOrderPosID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedOutOrderPos != null;
        }

        [ACMethodCommand(OutOrderPos.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Searches order positions according to the search word and fills the OutOrderPosList.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("OutOrderPosList");
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
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                #region Always Enabled
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                #endregion

                #region Load
                case nameof(Load):
                case nameof(IsEnabledLoad):
                    return new string[] { nameof(ACState), nameof(SelectedOutOrderPos) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}
