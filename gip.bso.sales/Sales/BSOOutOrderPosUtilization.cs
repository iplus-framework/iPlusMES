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
    /// Businessobject or App for recording efforts/utilizations (OutOrderPosUtilization) on order positions.
    /// To search records enter the search string in the SearchWord property.
    /// The database result is copied to the OutOrderPosUtilizationList property.
    /// Then call the NavigateFirst() method to set CurrentOutOrderPosUtilization with the first record in the list.
    /// CurrentOutOrderPosUtilization is used to display and edit the currently selected utilization record.
    /// The New() method creates a new utilization record with a new number (from the number manager), adds it to the database context and assigns it to the CurrentOutOrderPosUtilization property.
    /// The Delete() method deletes the current utilization record provided there are no foreign key relationships from other tables.
    /// The Load() method updates the CurrentOutOrderPosUtilization object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOOutOrderPosUtilization&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Outorderposutilization'}de{'Aufwandserfassung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOrderPosUtilization.ClassName,
        Description = @"Businessobject or App for recording efforts/utilizations (OutOrderPosUtilization) on order positions.
        To search records enter the search string in the SearchWord property.
        The database result is copied to the OutOrderPosUtilizationList property.
        Then call the NavigateFirst() method to set CurrentOutOrderPosUtilization with the first record in the list.
        CurrentOutOrderPosUtilization is used to display and edit the currently selected utilization record.
        The New() method creates a new utilization record with a new number (from the number manager), adds it to the database context and assigns it to the CurrentOutOrderPosUtilization property.
        The Delete() method deletes the current utilization record provided there are no foreign key relationships from other tables.
        The Load() method updates the CurrentOutOrderPosUtilization object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOOutOrderPosUtilization&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOOutOrderPosUtilization : ACBSOvbNav 
    {
        #region c´tors

        public BSOOutOrderPosUtilization(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
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
        public override IAccessNav AccessNav { get {return AccessPrimary;} }

        ACAccessNav<OutOrderPosUtilization> _AccessPrimary;
        [ACPropertyAccessPrimary(690, "OutOrderPosUtilization", Description = "Primary navigation access to the OutOrderPosUtilization records.")]
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

        [ACPropertyCurrent(600, "OutOrderPosUtilization", Description = "Currently loaded utilization record; edit this object.")]
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

        [ACPropertyList(601, "OutOrderPosUtilization", Description = "Result list of utilization records.")]
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

        [ACMethodInteraction("OutOrderPosUtilization", Const.New, (short)MISort.New, true, "SelectedOutOrderPosUtilization", Global.ACKinds.MSMethodPrePost, Description = "Creates a new utilization record with a new number and assigns it to the CurrentOutOrderPosUtilization property.")]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(OutOrderPosUtilization), OutOrderPosUtilization.NoColumnName, OutOrderPosUtilization.FormatNewNo, this);
            CurrentOutOrderPosUtilization = OutOrderPosUtilization.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.OutOrderPosUtilization.Add(CurrentOutOrderPosUtilization);
            ACState = Const.SMNew;
            PostExecute("New");
           
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction("OutOrderPosUtilization", Const.Delete, (short)MISort.Delete, true, "CurrentOutOrderPosUtilization", Global.ACKinds.MSMethodPrePost, Description = "Deletes the current utilization record if no foreign key relationships exist from other tables.")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentOutOrderPosUtilization.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentOutOrderPosUtilization);
            SelectedOutOrderPosUtilization = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        public bool IsEnabledDelete()
        {
            return CurrentOutOrderPosUtilization != null;
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
                case"Load":
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case"IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case"New":
                    New();
                    return true;
                case"IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
                case"Delete":
                    Delete();
                    return true;
                case"IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case"Search":
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
                case nameof(IsEnabledNew):
                    return new string[] { nameof(InitState) };
                #endregion

                #region Load
                case nameof(Load):
                case nameof(IsEnabledLoad):
                    return new string[] { nameof(ACState), nameof(SelectedOutOrderPosUtilization) };
                #endregion

                #region Delete
                case nameof(Delete):
                case nameof(IsEnabledDelete):
                    return new string[] { nameof(CurrentOutOrderPosUtilization) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}
