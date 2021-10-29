using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'User settings'}de{'Benutzereinstellungen'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + "Msg")]
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
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
        [ACPropertySelected(101, "Company", "en{'Tenant'}de{'Mandant'}")]
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
        [ACPropertyList(102, "Company")]
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
        [ACPropertySelected(9999, "CompanyAddress", "en{'Address for Invoice'}de{'Adresse zur Rechnungstellung'}")]
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
        [ACPropertyList(9999, "CompanyAddress")]
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
        [ACPropertySelected(9999, "CompanyPerson", "en{'Person for invoice'}de{'Person zur Rechnungstellung'}")]
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
        [ACPropertyList(9999, "CompanyPerson")]
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
        [ACMethodCommand(Partslist.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            if (!PreExecute("Save")) return;
            ACSaveChanges();
            OnSave();
            PostExecute("Save");
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand("Partslist", "en{'Undo'}de{'Rückgängig'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInfo("SetUserSettings", "en{'Set'}de{'Einstellen'}", 9999, false, false, true)]
        public void SetUserSettings()
        {
            if (!IsEnabledSetUserSettings()) return;
            if (CurrentUserSettings == null)
            {
                CurrentUserSettings = UserSettings.NewACObject(DatabaseApp, Root.Environment.User);
                DatabaseApp.UserSettings.AddObject(CurrentUserSettings);
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
        [ACMethodInfo("ResetUserSettings", "en{'Remove'}de{'Entfernen'}", 9999, false, false, true)]
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


        #endregion


    }
}
