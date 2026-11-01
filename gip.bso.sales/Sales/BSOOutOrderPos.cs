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
    /// Businessobject or App for order entry (Jobentry): creating and maintaining order positions (OutOrderPos) with their parent order (OutOrder).
    /// To search records enter the search string in the SearchWord property.
    /// The database result is copied to the OutOrderPosList property.
    /// Then call the NavigateFirst() method to set CurrentOutOrderPos with the first record in the list.
    /// CurrentOutOrderPos is used to display and edit the currently selected order position; CurrentOutOrder provides the parent order of the position.
    /// Property changes should always be made to CurrentOutOrderPos and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new order position and assigns the new entity object to the CurrentOutOrderPos property.
    /// Fill in all required fields before saving. The customer is chosen from the CompanyList property (companies flagged as customers).
    /// The billing and delivery addresses of the customer are provided by the BillingCompanyAddressList and DeliveryCompanyAddressList properties; the currently assigned addresses are available via CurrentBillingCompanyAddress and CurrentDeliveryCompanyAddress.
    /// The Delete() method deletes the current order position provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load() method updates the CurrentOutOrderPos object with fresh database data if another user has made changes in the background.
    /// The UndoSave() method discards unsaved changes of the current record.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOOutOrderPos&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Jobentry'}de{'Aufgabenerfassung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOrderPos.ClassName,
        Description = @"Businessobject or App for order entry (Jobentry): creating and maintaining order positions (OutOrderPos) with their parent order (OutOrder).
        To search records enter the search string in the SearchWord property.
        The database result is copied to the OutOrderPosList property.
        Then call the NavigateFirst() method to set CurrentOutOrderPos with the first record in the list.
        CurrentOutOrderPos is used to display and edit the currently selected order position; CurrentOutOrder provides the parent order of the position.
        Property changes should always be made to CurrentOutOrderPos and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new order position and assigns the new entity object to the CurrentOutOrderPos property.
        Fill in all required fields before saving. The customer is chosen from the CompanyList property (companies flagged as customers).
        The billing and delivery addresses of the customer are provided by the BillingCompanyAddressList and DeliveryCompanyAddressList properties; the currently assigned addresses are available via CurrentBillingCompanyAddress and CurrentDeliveryCompanyAddress.
        The Delete() method deletes the current order position provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load() method updates the CurrentOutOrderPos object with fresh database data if another user has made changes in the background.
        The UndoSave() method discards unsaved changes of the current record.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOOutOrderPos&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOOutOrderPos : ACBSOvbNav 
    {
        #region c´tors

        public BSOOutOrderPos(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
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
        ACAccessNav<OutOrderPos> _AccessPrimary;
        [ACPropertyAccessPrimary(690, OutOrderPos.ClassName, Description = "Primary navigation access to the OutOrderPos records of the order entry.")]
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
                OnPropertyChanged("OutOrderPosDeclarationList");
                OnPropertyChanged("CurrentBillingCompanyAddress");
                OnPropertyChanged("CurrentDeliveryCompanyAddress");
            }
        }

        [ACPropertyList(601, OutOrderPos.ClassName, Description = "Result list of order positions.")]
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

        [ACPropertyCurrent(603, OutOrder.ClassName, Description = "Parent order of the current order position.")]
        public OutOrder CurrentOutOrder
        {
            get
            {
                if (CurrentOutOrderPos == null)
                    return null;
                return CurrentOutOrderPos.OutOrder;
            }
        }


        /// <summary>
        /// Liste aller Unternehmen, die Lieferanten sind
        /// </summary>
        [ACPropertyList(604, Company.ClassName, Description = "List of all companies flagged as customers, available for selection as order customer.")]
        public IEnumerable<Company> CompanyList
        {
            get
            {
                return DatabaseApp.Company.Where(c => c.IsCustomer).OrderBy(c => c.CompanyName).AsEnumerable();
            }
        }

        [ACPropertyList(605, "BillingCompanyAddress", Description = "Billing addresses of the customer of the current order.")]
        public IEnumerable<CompanyAddress> BillingCompanyAddressList
        {
            get
            {
                if (CurrentOutOrder == null || CurrentOutOrder.CustomerCompany == null)
                    return null;
                if (!CurrentOutOrder.CustomerCompany.CompanyAddress_Company_IsLoaded)
                    CurrentOutOrder.CustomerCompany.CompanyAddress_Company.AutoLoad(CurrentOutOrder.CustomerCompany.CompanyAddress_CompanyReference, CurrentOutOrder);
                return CurrentOutOrder.CustomerCompany.CompanyAddress_Company.Where(c => c.IsBillingCompanyAddress).OrderBy(c => c.Name1).AsEnumerable();
            }
        }

        [ACPropertyList(606, "DeliveryCompanyAddress", Description = "Delivery addresses of the customer of the current order.")]
        public IEnumerable<CompanyAddress> DeliveryCompanyAddressList
        {
            get
            {
                if (CurrentOutOrder == null || CurrentOutOrder.CustomerCompany == null)
                    return null;
                if (!CurrentOutOrder.CustomerCompany.CompanyAddress_Company_IsLoaded)
                    CurrentOutOrder.CustomerCompany.CompanyAddress_Company.AutoLoad(CurrentOutOrder.CustomerCompany.CompanyAddress_CompanyReference, CurrentOutOrder);
                return CurrentOutOrder.CustomerCompany.CompanyAddress_Company.Where(c => c.IsDeliveryCompanyAddress).OrderBy(c => c.Name1).AsEnumerable();
            }
        }

        [ACPropertyCurrent(607, "BillingCompanyAddress", Description = "Billing address currently assigned to the current order.")]
        public CompanyAddress CurrentBillingCompanyAddress
        {
            get
            {
                if ( CurrentOutOrder == null )
                    return null;
                return CurrentOutOrder.BillingCompanyAddress;
            }
        }

        [ACPropertyCurrent(608, "DeliveryCompanyAddress", Description = "Delivery address currently assigned to the current order.")]
        public CompanyAddress CurrentDeliveryCompanyAddress
        {
            get
            {
                if (CurrentOutOrder == null)
                    return null;
                return CurrentOutOrder.DeliveryCompanyAddress;
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

        [ACMethodInteraction(OutOrderPos.ClassName, Const.New, (short)MISort.New, true, "SelectedOutOrderPos", Global.ACKinds.MSMethodPrePost, Description = "Creates a new order position and assigns it to the CurrentOutOrderPos property.")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentOutOrderPos = OutOrderPos.NewACObject(DatabaseApp, null);
            ACState = Const.SMNew;
            PostExecute("New");
           
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(OutOrderPos.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentOutOrderPos", Global.ACKinds.MSMethodPrePost, Description = "Deletes the current order position if no foreign key relationships exist from other tables.")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentOutOrderPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentOutOrderPos);
            SelectedOutOrderPos = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");

        }

        public bool IsEnabledDelete()
        {
            return CurrentOutOrderPos != null;
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
                    return new string[] { nameof(ACState), nameof(SelectedOutOrderPos) };
                #endregion

                #region Delete
                case nameof(Delete):
                case nameof(IsEnabledDelete):
                    return new string[] { nameof(CurrentOutOrderPos) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}
