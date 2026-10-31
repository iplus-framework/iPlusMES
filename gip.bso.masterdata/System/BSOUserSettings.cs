using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Businessobject or App for managing the user settings of the currently logged-in user.
    /// On startup the UserSettings record of the current user (Root.Environment.User) is loaded into the CurrentUserSettings property and the saved tenant company (TenantCompany), invoice address (InvoiceCompanyAddress) and invoice person (InvoiceCompanyPerson) are preselected in the lists.
    /// Select the tenant in the SelectedCompany property (the CompanyList property contains only companies flagged as tenant), the address for invoicing in the SelectedCompanyAddress property and the person for invoicing in the SelectedCompanyPerson property.
    /// Call the SetUserSettings() method to apply the selections to CurrentUserSettings (the record is created if the user has none yet), then call the Save() method to persist the changes in the database.
    /// Call the ResetUserSettings() method to delete the UserSettings record of the current user, then call the Save() method to execute the delete in the database.
    /// The UndoSave() method discards all unsaved changes.
    /// This BSO has no search or navigation; it always works on the settings of the current user.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOUserSettings&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSystem, "en{'User settings'}de{'Benutzereinstellungen'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + "Msg",
        Description = @"Businessobject or App for managing the user settings of the currently logged-in user.
        On startup the UserSettings record of the current user (Root.Environment.User) is loaded into the CurrentUserSettings property and the saved tenant company (TenantCompany), invoice address (InvoiceCompanyAddress) and invoice person (InvoiceCompanyPerson) are preselected in the lists.
        Select the tenant in the SelectedCompany property (the CompanyList property contains only companies flagged as tenant), the address for invoicing in the SelectedCompanyAddress property and the person for invoicing in the SelectedCompanyPerson property.
        Call the SetUserSettings() method to apply the selections to CurrentUserSettings (the record is created if the user has none yet), then call the Save() method to persist the changes in the database.
        Call the ResetUserSettings() method to delete the UserSettings record of the current user, then call the Save() method to execute the delete in the database.
        The UndoSave() method discards all unsaved changes.
        This BSO has no search or navigation; it always works on the settings of the current user.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOUserSettings&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    class BSOUserSettings : ACBSOvb
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOUserSettings(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            AccessCompany.NavSearch();
            CurrentUserSettings = DatabaseApp.UserSettings.FirstOrDefault(c => c.VBUserID == Root.Environment.User.VBUserID);
            if (CurrentUserSettings != null)
            {
                if (!CompanyList.Contains(CurrentUserSettings.TenantCompany))
                    AccessCompany.NavList.Add(CurrentUserSettings.TenantCompany);
                SelectedCompany = CurrentUserSettings.TenantCompany;
                SelectedCompanyAddress = CurrentUserSettings.InvoiceCompanyAddress;
                SelectedCompanyPerson = CurrentUserSettings.InvoiceCompanyPerson;

            }
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            return await base.ACDeInit(deleteACClassTask);
        }



        #endregion

        public UserSettings CurrentUserSettings { get; set; }

        #region Tenant

        #region Tenant -> Company

        ACAccessNav<Company> _AccessCompany;
        [ACPropertyAccess(100, "Company")]
        public ACAccessNav<Company> AccessCompany
        {
            get
            {
                if (_AccessCompany == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "Company", ACType.ACIdentifier);
                    _AccessCompany = navACQueryDefinition.NewAccessNav<Company>("Company", this);
                    ACFilterItem filter = navACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "IsTenant").FirstOrDefault();
                    if (filter == null)
                    {
                        filter = new ACFilterItem(Global.FilterTypes.filter, "IsTenant", Global.LogicalOperators.equal, Global.Operators.and, "true", true);
                        navACQueryDefinition.ACFilterColumns.Add(filter);
                    }
                    _AccessCompany.AutoSaveOnNavigation = false;
                }
                return _AccessCompany;
            }
        }

        /// <summary>
        /// Gets or sets the selected Company.
        /// </summary>
        /// <value>The selected Company.</value>
        [ACPropertySelected(101, "Company", "en{'Tenant'}de{'Mandant'}", Description = "Tenant company selected for the current user. The list contains only companies with IsTenant=true.")]
        public Company SelectedCompany
        {
            get
            {
                if (AccessCompany == null)
                    return null;
                return AccessCompany.Selected;
            }
            set
            {
                if (AccessCompany == null)
                    return;
                if (AccessCompany.Selected != value)
                {
                    AccessCompany.Selected = value;
                    _CompanyAddressList = null;
                    _CompanyPersonList = null;
                    OnPropertyChanged("CompanyAddressList");
                    OnPropertyChanged("CompanyPersonList");
                }
                OnPropertyChanged("SelectedCompany");
            }
        }

        /// <summary>
        /// Gets the Company list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(102, "Company", Description = "All tenant companies (IsTenant=true) available for selection.")]
        public IEnumerable<Company> CompanyList
        {
            get
            {
                if (AccessCompany == null)
                    return null;
                return AccessCompany.NavList;
            }
        }

        #endregion


        #region Tenant -> CompanyAddress
        private CompanyAddress _SelectedCompanyAddress;
        /// <summary>
        /// Selected property for CompanyAddress
        /// </summary>
        /// <value>The selected CompanyAddress</value>
        [ACPropertySelected(9999, "CompanyAddress", "en{'Address for Invoice'}de{'Adresse zur Rechnungstellung'}", Description = "Company address of the selected tenant that is used for invoicing.")]
        public CompanyAddress SelectedCompanyAddress
        {
            get
            {
                return _SelectedCompanyAddress;
            }
            set
            {
                if (_SelectedCompanyAddress != value)
                {
                    _SelectedCompanyAddress = value;
                    OnPropertyChanged("SelectedCompanyAddress");
                }
            }
        }


        private List<CompanyAddress> _CompanyAddressList;
        /// <summary>
        /// List property for CompanyAddress
        /// </summary>
        /// <value>The CompanyAddress list</value>
        [ACPropertyList(9999, "CompanyAddress", Description = "Addresses of the currently selected tenant company.")]
        public List<CompanyAddress> CompanyAddressList
        {
            get
            {
                if (_CompanyAddressList == null)
                    _CompanyAddressList = LoadCompanyAddressList();
                return _CompanyAddressList;
            }
        }

        private List<CompanyAddress> LoadCompanyAddressList()
        {
            if (SelectedCompany == null) return null;
            return SelectedCompany.CompanyAddress_Company.OrderBy(c => c.Name1).ToList();

        }
        #endregion


        #region Tenant -> CompanyPerson
        private CompanyPerson _SelectedCompanyPerson;
        /// <summary>
        /// Selected property for CompanyPerson
        /// </summary>
        /// <value>The selected CompanyPerson</value>
        [ACPropertySelected(9999, "CompanyPerson", "en{'Person for invoice'}de{'Person zur Rechnungstellung'}", Description = "Company person of the selected tenant that is used for invoicing.")]
        public CompanyPerson SelectedCompanyPerson
        {
            get
            {
                return _SelectedCompanyPerson;
            }
            set
            {
                if (_SelectedCompanyPerson != value)
                {
                    _SelectedCompanyPerson = value;
                    OnPropertyChanged("SelectedCompanyPerson");
                }
            }
        }


        private List<CompanyPerson> _CompanyPersonList;
        /// <summary>
        /// List property for CompanyPerson
        /// </summary>
        /// <value>The CompanyPerson list</value>
        [ACPropertyList(9999, "CompanyPerson", Description = "Persons of the currently selected tenant company.")]
        public List<CompanyPerson> CompanyPersonList
        {
            get
            {
                if (_CompanyPersonList == null)
                    _CompanyPersonList = LoadCompanyPersonList();
                return _CompanyPersonList;
            }
        }

        private List<CompanyPerson> LoadCompanyPersonList()
        {
            if (SelectedCompany == null) return null;
            return SelectedCompany.CompanyPerson_Company.OrderBy(c => c.Name1).ThenBy(c => c.Name2).ToList();

        }
        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Partslist.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves the user settings of the current user to the database.")]
        public void Save()
        {
            if (!PreExecute("Save")) return;
            ACSaveChanges();
            _ = OnSave();
            PostExecute("Save");
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand("Partslist", "en{'Undo'}de{'Rückgängig'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards all unsaved changes of the user settings.")]
        public void UndoSave()
        {
            if (!PreExecute("UndoSave")) return;
            OnUndoSave();
            PostExecute("UndoSave");
        }
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        /// <summary>
        /// Method SaveUserSettings
        /// </summary>
        [ACMethodInfo("SetUserSettings", "en{'Set'}de{'Einstellen'}", 9999, false, false, true, Description = "Applies the selected tenant, invoice address and invoice person to the UserSettings record of the current user (creates the record if the user has none yet). Call Save() to persist.")]
        public void SetUserSettings()
        {
            if (!IsEnabledSetUserSettings()) return;
            if (CurrentUserSettings == null)
            {
                CurrentUserSettings = UserSettings.NewACObject(DatabaseApp, Root.Environment.User);
                DatabaseApp.UserSettings.Add(CurrentUserSettings);
            }
            CurrentUserSettings.TenantCompany = SelectedCompany;
            CurrentUserSettings.InvoiceCompanyAddress = SelectedCompanyAddress;
            CurrentUserSettings.InvoiceCompanyPerson = SelectedCompanyPerson;
        }

        public bool IsEnabledSetUserSettings()
        {
            return
                SelectedCompany != null
                &&
                    (CurrentUserSettings == null
                    || (CurrentUserSettings.TenantCompany != SelectedCompany
                        || CurrentUserSettings.InvoiceCompanyAddress != SelectedCompanyAddress
                        || CurrentUserSettings.InvoiceCompanyPerson != SelectedCompanyPerson)
                    );
        }


        /// <summary>
        /// Method DeleteUserSettings
        /// </summary>
        [ACMethodInfo("ResetUserSettings", "en{'Remove'}de{'Entfernen'}", 9999, false, false, true, Description = "Deletes the UserSettings record of the current user and clears the selection. Call Save() to execute the delete in the database.")]
        public void ResetUserSettings()
        {
            if (!IsEnabledResetUserSettings()) return;
            CurrentUserSettings.DeleteACObject(DatabaseApp, false);
            CurrentUserSettings = null;
            SelectedCompany = null;
        }

        public bool IsEnabledResetUserSettings()
        {
            return CurrentUserSettings != null;
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
                case nameof(SetUserSettings):
                    SetUserSettings();
                    return true;
                case nameof(IsEnabledSetUserSettings):
                    result = IsEnabledSetUserSettings();
                    return true;
                case nameof(ResetUserSettings):
                    ResetUserSettings();
                    return true;
                case nameof(IsEnabledResetUserSettings):
                    result = IsEnabledResetUserSettings();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(SetUserSettings):
                case nameof(IsEnabledSetUserSettings):
                    return new string[] { nameof(CurrentUserSettings) };
                case nameof(ResetUserSettings):
                case nameof(IsEnabledResetUserSettings):
                    return new string[] { nameof(CurrentUserSettings) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion
    }
}