using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.datamodel; using gip.core.datamodel;
//using gip.core.manager;
using gip.core.autocomponent;
using gip.mes.autocomponent;

namespace gip.bso.sales
{
    /// <summary>
    /// Businessobject or App for the utilization analysis (overview of OutOrderPosUtilization records).
    /// It shows a list of recorded efforts/utilizations for evaluation purposes.
    /// To search records enter the search string in the SearchWord property.
    /// The database result is copied to the OutOrderPosUtilizationList property.
    /// Then call the NavigateFirst() method to set CurrentOutOrderPosUtilization with the first record in the list.
    /// CurrentOutOrderPosUtilization is used to display and edit the currently selected utilization record.
    /// Property changes should always be made to CurrentOutOrderPosUtilization and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The Load() method re-reads the selected record from the database if another user has made changes in the background.
    /// The UndoSave() method discards unsaved changes of the current record.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOOutOrderPosUtilizationOverview&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Utilizationanalysis'}de{'Aufwandsauswertung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOrderPosUtilization.ClassName,
        Description = @"Businessobject or App for the utilization analysis (overview of OutOrderPosUtilization records).
        It shows a list of recorded efforts/utilizations for evaluation purposes.
        To search records enter the search string in the SearchWord property.
        The database result is copied to the OutOrderPosUtilizationList property.
        Then call the NavigateFirst() method to set CurrentOutOrderPosUtilization with the first record in the list.
        CurrentOutOrderPosUtilization is used to display and edit the currently selected utilization record.
        Property changes should always be made to CurrentOutOrderPosUtilization and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The Load() method re-reads the selected record from the database if another user has made changes in the background.
        The UndoSave() method discards unsaved changes of the current record.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOOutOrderPosUtilizationOverview&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOOutOrderPosUtilizationOverview : ACBSOvbNav 
    {
        #region c´tors

        public BSOOutOrderPosUtilizationOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
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
        ACAccessNav<OutOrderPosUtilization> _AccessPrimary;
        [ACPropertyAccessPrimary(690, "OutOrderPosUtilization", Description = "Primary navigation access to the OutOrderPosUtilization records of the utilization analysis.")]
        public ACAccessNav<OutOrderPosUtilization> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<OutOrderPosUtilization>("OutOrderPosUtilization", this);
                }
                return _AccessPrimary;
            }
        }

        [ACPropertyCurrent(600, "OutOrderPosUtilization", Description = "Currently loaded utilization record; edit this object and call Save().")]
        public OutOrderPosUtilization CurrentOutOrderPosUtilization
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentOutOrderPosUtilization");
            }
        }

        [ACPropertyList(601, "OutOrderPosUtilization", Description = "Result list of utilization records shown in the analysis grid.")]
        public IEnumerable<OutOrderPosUtilization> OutOrderPosUtilizationList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, "OutOrderPosUtilization", Description = "Utilization record currently selected in the list.")]
        public OutOrderPosUtilization SelectedOutOrderPosUtilization
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedOutOrderPosUtilization");
            }
        }
        #endregion

        #region BSO->ACMethod
        [ACMethodCommand("OutOrderPosUtilization", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves the changes of the current utilization record to the database.")]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand("OutOrderPosUtilization", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards the unsaved changes of the current utilization record.")]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction("OutOrderPosUtilization", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedOutOrderPosUtilization", Global.ACKinds.MSMethodPrePost, Description = "Re-reads the selected utilization record from the database, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<OutOrderPosUtilization>(requery, () => SelectedOutOrderPosUtilization, () => CurrentOutOrderPosUtilization, c => CurrentOutOrderPosUtilization = c,
                        DatabaseApp.OutOrderPosUtilization
                        .Where(c => c.OutOrderPosUtilizationID == SelectedOutOrderPosUtilization.OutOrderPosUtilizationID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedOutOrderPosUtilization != null;
        }

        [ACMethodCommand("OutOrderPosUtilization", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Searches utilization records according to the search word and fills the OutOrderPosUtilizationList.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("OutOrderPosUtilizationList");
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
                    return new string[] { nameof(ACState), nameof(SelectedOutOrderPosUtilization) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}
