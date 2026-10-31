// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 11.01.2018
// ***********************************************************************
// <copyright file="BSOCompany.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.autocomponent;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Businessobject or App for managing the company master data (customers, suppliers, partners and own companies).
    /// To search for records, enter the search string in the SearchWord property.
    /// The database result is copied to the CompanyList property.
    /// Then call the NavigateFirst() method to set CurrentCompany with the first record in the list.
    /// CurrentCompany is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentCompany and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new company with a generated company number and an empty house address and assigns the new entity object to the CurrentCompany property. Enter at least the CompanyName before calling Save().
    /// Use the Delete() method to delete the company provided there are no foreign key relationships from other tables. The user is asked for confirmation. Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load method updates the CurrentCompany object with fresh database data if another user has made changes in the background.
    /// The UndoSave method discards all unsaved changes of the current database context.
    /// The company record has several sub-lists which are edited on the tabs of the app:
    /// CompanyAddress: addresses of the company. NewCompanyAddress() creates a new address, DeleteCompanyAddress() deletes the selected address, LoadCompanyAddress() loads the selected address. The house address (CurrentHouseAdress) is created automatically by New().
    /// CompanyAddressDepartment: departments of the selected address. NewCompanyAddressDepartment() adds a new department to the current address.
    /// CompanyPerson: contact persons of the company. NewCompanyPerson() creates a person with a generated person number, DeleteCompanyPerson() deletes the current person, LoadCompanyPerson() loads the selected person. The SearchPerson property filters the person list by person number or name.
    /// Factory: plants (subordinate companies) of the company. NewFactory() creates a plant below the current company, LoadFactory() loads the selected plant, DeleteFactory() deletes the current plant.
    /// CompanyMaterial: material mappings of the company (customer/supplier material numbers with min./opt. stock quantities). NewCompanyMaterial() / DeleteCompanyMaterial() / LoadCompanyMaterial(). With the FindCompanyWithCMaterialNo property a company can be found and loaded by a company material number.
    /// CompanyMaterialPickup: pickup relations of the company (with validity period). NewCompanyMaterialPickup() / DeleteCompanyMaterialPickup() / LoadCompanyMaterialPickup(). With the FindCompanyWithPickupNo property a company can be found and loaded by a pickup number.
    /// CompanyMaterialStock: read-only stock information of the company materials.
    /// The BSOCompanyMaterialOverview_Child property gives access to the embedded child BSO BSOCompanyMaterialOverview which shows a material/stock overview for the current company; it is refreshed automatically when CurrentCompany changes.
    /// New or deleted sub-records (addresses, departments, persons, factories, materials, pickups) are only written to the database when the Save() method of this BSO is called.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOCompany&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Company'}de{'Unternehmen'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Company.ClassName,
        Description = @"Businessobject or App for managing the company master data (customers, suppliers, partners and own companies).
        To search for records, enter the search string in the SearchWord property.
        The database result is copied to the CompanyList property.
        Then call the NavigateFirst() method to set CurrentCompany with the first record in the list.
        CurrentCompany is used to display and edit the currently selected record.
        Property changes should always be made to CurrentCompany and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new company with a generated company number and an empty house address and assigns the new entity object to the CurrentCompany property. Enter at least the CompanyName before calling Save().
        Use the Delete() method to delete the company provided there are no foreign key relationships from other tables. The user is asked for confirmation. Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load method updates the CurrentCompany object with fresh database data if another user has made changes in the background.
        The UndoSave method discards all unsaved changes of the current database context.
        The company record has several sub-lists which are edited on the tabs of the app:
        CompanyAddress: addresses of the company. NewCompanyAddress() creates a new address, DeleteCompanyAddress() deletes the selected address, LoadCompanyAddress() loads the selected address. The house address (CurrentHouseAdress) is created automatically by New().
        CompanyAddressDepartment: departments of the selected address. NewCompanyAddressDepartment() adds a new department to the current address.
        CompanyPerson: contact persons of the company. NewCompanyPerson() creates a person with a generated person number, DeleteCompanyPerson() deletes the current person, LoadCompanyPerson() loads the selected person. The SearchPerson property filters the person list by person number or name.
        Factory: plants (subordinate companies) of the company. NewFactory() creates a plant below the current company, LoadFactory() loads the selected plant, DeleteFactory() deletes the current plant.
        CompanyMaterial: material mappings of the company (customer/supplier material numbers with min./opt. stock quantities). NewCompanyMaterial() / DeleteCompanyMaterial() / LoadCompanyMaterial(). With the FindCompanyWithCMaterialNo property a company can be found and loaded by a company material number.
        CompanyMaterialPickup: pickup relations of the company (with validity period). NewCompanyMaterialPickup() / DeleteCompanyMaterialPickup() / LoadCompanyMaterialPickup(). With the FindCompanyWithPickupNo property a company can be found and loaded by a pickup number.
        CompanyMaterialStock: read-only stock information of the company materials.
        The BSOCompanyMaterialOverview_Child property gives access to the embedded child BSO BSOCompanyMaterialOverview which shows a material/stock overview for the current company; it is refreshed automatically when CurrentCompany changes.
        New or deleted sub-records (addresses, departments, persons, factories, materials, pickups) are only written to the database when the Save() method of this BSO is called.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOCompany&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    [ACQueryInfo(Const.PackName_VarioCompany, Const.QueryPrefix + "CompanyPersonFind", "en{'Search Person'}de{'Finde Person'}", typeof(CompanyPerson), CompanyPerson.ClassName, Company.ClassName + "\\CompanyNo,CompanyPersonNo,Name1", "CompanyPersonNo")]
    public class BSOCompany : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOCompany"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOCompany(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

            ACUrlCommand("AccessCompany!Test", null);
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._AccessCompanyAddress = null;
            this._AccessCompanyPerson = null;
            this._CurrentCompanyAddress = null;
            this._CurrentCompanyAddressDepartment = null;
            this._CurrentCompanyMaterial = null;
            this._CurrentCompanyMaterialPickup = null;
            this._CurrentCompanyPerson = null;
            this._CurrentFactory = null;
            this._CurrentHouseAdress = null;
            this._FindCompanyWithCMaterialNo = null;
            this._FindCompanyWithPickupNo = null;
            this._SearchPerson = null;
            this._SelectedACClass = null;
            this._SelectedAssignedACClass = null;
            this._SelectedCompanyAddress = null;
            this._SelectedCompanyMaterial = null;
            this._SelectedCompanyMaterialPickup = null;
            this._SelectedCompanyMaterialStock = null;
            this._SelectedCompanyPerson = null;
            this._SelectedFactory = null;
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessCompanyAddress != null)
            {
                await _AccessCompanyAddress.ACDeInit(false);
                _AccessCompanyAddress = null;
            }
            if (_AccessCompanyPerson != null)
            {
                await _AccessCompanyPerson.ACDeInit(false);
                _AccessCompanyPerson = null;
            }
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region BSO->ACProperty

        #region Child BSO

        ACChildItem<BSOCompanyMaterialOverview> _BSOCompanyMaterialOverview_Child;
        [ACPropertyInfo(600, Description = "Child BSO for the material overview tab. Shows a material/stock overview for the current company.")]
        [ACChildInfo(nameof(BSOCompanyMaterialOverview_Child), typeof(BSOCompanyMaterialOverview))]
        public ACChildItem<BSOCompanyMaterialOverview> BSOCompanyMaterialOverview_Child
        {
            get
            {
                if (_BSOCompanyMaterialOverview_Child == null)
                    _BSOCompanyMaterialOverview_Child = new ACChildItem<BSOCompanyMaterialOverview>(this, nameof(BSOCompanyMaterialOverview_Child));
                return _BSOCompanyMaterialOverview_Child;
            }
        }

        #endregion

        #region 1. Company
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        protected ACAccessNav<Company> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, Company.ClassName, Description = "Navigation access to the Company entities. SearchWord filters the CompanyList; NavigateFirst() sets CurrentCompany.")]
        public virtual ACAccessNav<Company> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Company>(Company.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the current company.
        /// </summary>
        /// <value>The current company.</value>
        [ACPropertyCurrent(600, Company.ClassName, Description = "The currently loaded company record. Edit its properties and call Save() to persist changes. Changing it refreshes the address, factory, material, pickup, stock and person lists and reloads the material overview child BSO.")]
        public Company CurrentCompany
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Current = value;
                OnPropertyChanged("CurrentCompany");
                OnPropertyChanged("FactoryList");
                OnPropertyChanged("CompanyAddressList");
                OnPropertyChanged("ACProjectList");
                OnPropertyChanged("CompanyMaterialList");
                OnPropertyChanged("CompanyMaterialPickupList");
                OnPropertyChanged("CompanyMaterialStockList");
                OnPropertyChanged("CompanyPersonList");
                if (value != null)
                {
                    var query = value.CompanyAddress_Company.Where(c => c.IsHouseCompanyAddress);
                    // Hausadress lesen
                    if (query.Any())
                    {
                        CurrentHouseAdress = value.CompanyAddress_Company.Where(c => c.IsHouseCompanyAddress).FirstOrDefault();
                        CurrentCompanyAddress = CurrentHouseAdress;
                    }
                    else
                    {
                        // Falls keine Hausadresse vorhanden, dann anlegen
                        CurrentHouseAdress = CompanyAddress.NewACObject(DatabaseApp, CurrentCompany);
                        CurrentHouseAdress.IsHouseCompanyAddress = true;
                    }

                    if (BSOCompanyMaterialOverview_Child != null && BSOCompanyMaterialOverview_Child.Value != null)
                    {
                        BSOCompanyMaterialOverview_Child.Value.CurrentCompany =  value;
                        BSOCompanyMaterialOverview_Child.Value.Search();
                    }
                }
                else
                {
                    CurrentHouseAdress = null;
                    CurrentCompanyAddress = null;
                    if (BSOCompanyMaterialOverview_Child != null && BSOCompanyMaterialOverview_Child.Value != null)
                    {
                        BSOCompanyMaterialOverview_Child.Value.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected company.
        /// </summary>
        /// <value>The selected company.</value>
        [ACPropertySelected(601, Company.ClassName, Description = "The company selected in the search result list. Load() copies it to CurrentCompany.")]
        public Company SelectedCompany
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedCompany");
            }
        }

        /// <summary>
        /// Gets the company list.
        /// </summary>
        /// <value>The company list.</value>
        [ACPropertyList(602, Company.ClassName, Description = "Result list of the company search (SearchWord).")]
        public IEnumerable<Company> CompanyList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region 1.1 CompanyAddress
        /// <summary>
        /// The _ current house adress
        /// </summary>
        CompanyAddress _CurrentHouseAdress;
        /// <summary>
        /// Gets or sets the current house adress.
        /// </summary>
        /// <value>The current house adress.</value>
        [ACPropertyCurrent(603, "HouseAdress", Description = "House address of the current company. Created automatically by New() if the company has none.")]
        public CompanyAddress CurrentHouseAdress
        {
            get
            {
                return _CurrentHouseAdress;
            }
            set
            {
                _CurrentHouseAdress = value;
                OnPropertyChanged("CurrentHouseAdress");
            }

        }

        /// <summary>
        /// The _ access company address
        /// </summary>
        ACAccess<CompanyAddress> _AccessCompanyAddress;
        /// <summary>
        /// Gets the access address.
        /// </summary>
        /// <value>The access address.</value>
        [ACPropertyAccess(691, CompanyAddress.ClassName, Description = "Access to the CompanyAddress entities of the current company.")]
        public ACAccess<CompanyAddress> AccessAddress
        {
            get
            {
                if (_AccessCompanyAddress == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + CompanyAddress.ClassName) as ACQueryDefinition;
                    _AccessCompanyAddress = acQueryDefinition.NewAccess<CompanyAddress>(CompanyAddress.ClassName, this);
                }
                return _AccessCompanyAddress;
            }
        }

        /// <summary>
        /// The _ current company address
        /// </summary>
        CompanyAddress _CurrentCompanyAddress;
        /// <summary>
        /// Gets or sets the current company address.
        /// </summary>
        /// <value>The current company address.</value>
        [ACPropertyCurrent(604, CompanyAddress.ClassName, Description = "The currently edited address of the company. Edit its properties and call Save() to persist.")]
        public CompanyAddress CurrentCompanyAddress
        {
            get
            {
                return _CurrentCompanyAddress;
            }
            set
            {
                _CurrentCompanyAddress = value;
                OnPropertyChanged("CurrentCompanyAddress");
            }
        }

        /// <summary>
        /// Gets the company address list.
        /// </summary>
        /// <value>The company address list.</value>
        [ACPropertyList(605, CompanyAddress.ClassName, Description = "All addresses of the current company.")]
        public IEnumerable<CompanyAddress> CompanyAddressList
        {
            get
            {
                if (CurrentCompany == null)
                    return null;
                return CurrentCompany.CompanyAddress_Company.AsEnumerable();

            }
        }

        /// <summary>
        /// The _ selected company address
        /// </summary>
        CompanyAddress _SelectedCompanyAddress;
        /// <summary>
        /// Gets or sets the selected company address.
        /// </summary>
        /// <value>The selected company address.</value>
        [ACPropertySelected(606, CompanyAddress.ClassName, Description = "The address selected in the address list. LoadCompanyAddress() copies it to CurrentCompanyAddress.")]
        public CompanyAddress SelectedCompanyAddress
        {
            get
            {
                return _SelectedCompanyAddress;
            }
            set
            {
                _SelectedCompanyAddress = value;
                OnPropertyChanged("SelectedCompanyAddress");
                OnPropertyChanged("CompanyAddressDepartmentList");
            }
        }
        #endregion

        #region 1.1.1 CompanyAddressDepartment
        /// <summary>
        /// The _ current company address department
        /// </summary>
        CompanyAddressDepartment _CurrentCompanyAddressDepartment;
        /// <summary>
        /// Gets or sets the current company address department.
        /// </summary>
        /// <value>The current company address department.</value>
        [ACPropertyCurrent(607, "CompanyAddressDepartment", Description = "The currently edited department of the selected company address.")]
        public CompanyAddressDepartment CurrentCompanyAddressDepartment
        {
            get
            {
                return _CurrentCompanyAddressDepartment;
            }
            set
            {
                _CurrentCompanyAddressDepartment = value;
                OnPropertyChanged("CurrentCompanyAddressDepartment");
            }
        }

        /// <summary>
        /// Gets the company address department list.
        /// </summary>
        /// <value>The company address department list.</value>
        [ACPropertyList(608, "CompanyAddressDepartment", Description = "All departments of the selected company address.")]
        public IEnumerable<CompanyAddressDepartment> CompanyAddressDepartmentList
        {
            get
            {
                if (CurrentCompanyAddress == null)
                    return null;
                return CurrentCompanyAddress.CompanyAddressDepartment_CompanyAddress;
            }
        }
        #endregion

        #region 1.2 CompanyPerson
        /// <summary>
        /// The _ access company person
        /// </summary>
        ACAccess<CompanyPerson> _AccessCompanyPerson;
        /// <summary>
        /// Gets the access person.
        /// </summary>
        /// <value>The access person.</value>
        [ACPropertyAccess(692, CompanyPerson.ClassName, Description = "Access to the CompanyPerson search query (person number and name filter).")]
        public ACAccess<CompanyPerson> AccessPerson
        {
            get
            {
                if (_AccessCompanyPerson == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "CompanyPersonFind", ACType.ACIdentifier);
                    bool rebuildACQueryDef = false;
                    if (!navACQueryDefinition.ACFilterColumns.Any())
                    {
                        rebuildACQueryDef = true;
                    }
                    else
                    {
                        int countFoundCorrect = 0;
                        foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            if (filterItem.PropertyName == "Company\\CompanyNo")
                                countFoundCorrect++;
                            else if (filterItem.PropertyName == "CompanyPersonNo")
                                countFoundCorrect++;
                            else if (filterItem.PropertyName == "Name1")
                                countFoundCorrect++;
                        }
                        if (countFoundCorrect < 6)
                            rebuildACQueryDef = true;
                    }
                    if (rebuildACQueryDef)
                    {
                        navACQueryDefinition.ClearFilter(true);
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "Company\\CompanyNo", Global.LogicalOperators.equal, Global.Operators.and, null, true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, false));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "CompanyPersonNo", Global.LogicalOperators.contains, Global.Operators.or, null, true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "Name1", Global.LogicalOperators.contains, Global.Operators.or, null, true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, false));
                        navACQueryDefinition.SaveConfig(true);
                    }

                    _AccessCompanyPerson = navACQueryDefinition.NewAccess<CompanyPerson>(CompanyPerson.ClassName, this);
                }
                return _AccessCompanyPerson;
            }
        }

        /// <summary>
        /// Its invoked from a WPF-Itemscontrol that wants to refresh its CollectionView because the user has changed the LINQ-Expressiontree in the ACQueryDefinition-Property of IAccess. 
        /// The BSO should execute the query on the database first, to get the new results for refreshing the CollectionView of the control.
        /// If the bso don't want to handle this request or manipulate the ACQueryDefinition it returns false. The WPF-control invokes then the IAccess.NavSearch()-Method itself.  
        /// </summary>
        /// <param name="acAccess">Reference to IAccess that contains the changed query (Property NavACQueryDefinition)</param>
        /// <returns>True if the bso has handled this request and queried the database context. Otherwise it returns false.</returns>
        public override bool ExecuteNavSearch(IAccess acAccess)
        {
            if (acAccess == _AccessCompanyPerson)
            {
                _AccessCompanyPerson.NavSearch(this.DatabaseApp);
                OnPropertyChanged("CompanyPersonList");
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }


        /// <summary>
        /// The _ current company person
        /// </summary>
        CompanyPerson _CurrentCompanyPerson;
        /// <summary>
        /// Gets or sets the current company person.
        /// </summary>
        /// <value>The current company person.</value>
        [ACPropertyCurrent(609, CompanyPerson.ClassName, Description = "The currently edited contact person of the company. Edit its properties and call Save() to persist.")]
        public CompanyPerson CurrentCompanyPerson
        {
            get
            {
                return _CurrentCompanyPerson;
            }
            set
            {
                _CurrentCompanyPerson = value;
                OnPropertyChanged("CurrentCompanyPerson");
            }
        }

        /// <summary>
        /// Gets the company person list.
        /// </summary>
        /// <value>The company person list.</value>
        [ACPropertyList(610, CompanyPerson.ClassName, Description = "Contact persons of the current company, filtered by the SearchPerson search word.")]
        public IEnumerable<CompanyPerson> CompanyPersonList
        {
            get
            {
                if (CurrentCompany == null || AccessPerson == null)
                    return null;
                ACFilterItem filterItem = AccessPerson.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "Company\\CompanyNo").FirstOrDefault();
                if (filterItem == null)
                    return null;
                if (filterItem.SearchWord != CurrentCompany.CompanyNo)
                {
                    AccessPerson.NavACQueryDefinition.SaveToACConfigOff = true;
                    filterItem.SearchWord = CurrentCompany.CompanyNo;
                    AccessPerson.NavSearch(DatabaseApp);
                    AccessPerson.NavACQueryDefinition.SaveToACConfigOff = false;
                }
                return AccessPerson.NavList;
            }
        }

        /// <summary>
        /// The _ selected company person
        /// </summary>
        CompanyPerson _SelectedCompanyPerson;
        /// <summary>
        /// Gets or sets the selected company person.
        /// </summary>
        /// <value>The selected company person.</value>
        [ACPropertySelected(611, CompanyPerson.ClassName, Description = "The person selected in the person list. LoadCompanyPerson() copies it to CurrentCompanyPerson.")]
        public CompanyPerson SelectedCompanyPerson
        {
            get
            {
                return _SelectedCompanyPerson;
            }
            set
            {
                _SelectedCompanyPerson = value;
                OnPropertyChanged("SelectedCompanyPerson");
            }
        }

        private string _SearchPerson;
        [ACPropertyInfo(612, "", "en{'Search Person'}de{'Suche Person'}", Description = "Search word for the person list. Filters the persons of the current company by person number or name.")]
        public string SearchPerson
        {
            get
            {
                return _SearchPerson;
            }
            set
            {
                _SearchPerson = value;
                OnPropertyChanged("SearchPerson");
                if (AccessPerson != null)
                {
                    AccessPerson.NavACQueryDefinition.SaveToACConfigOff = true;
                    bool refresh = false;
                    ACFilterItem filterItem = AccessPerson.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "CompanyPersonNo").FirstOrDefault();
                    if (filterItem != null)
                    {
                        if (filterItem.SearchWord != value)
                        {
                            filterItem.SearchWord = value;
                            refresh = true;
                        }
                    }
                    filterItem = AccessPerson.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "Name1").FirstOrDefault();
                    if (filterItem != null)
                    {
                        if (filterItem.SearchWord != value)
                        {
                            filterItem.SearchWord = value;
                            refresh = true;
                        }
                    }
                    if (refresh)
                        AccessPerson.NavSearch(DatabaseApp);
                    AccessPerson.NavACQueryDefinition.SaveToACConfigOff = false;
                }
                OnPropertyChanged("CompanyPersonList");
            }
        }
        #endregion

        #region 1.2.1 CompanyPersonRole
        /// <summary>
        /// The _ selected AC class
        /// </summary>
        gip.core.datamodel.ACClass _SelectedACClass = null;
        /// <summary>
        /// Gets or sets the selected AC class.
        /// </summary>
        /// <value>The selected AC class.</value>
        [ACPropertySelected(613, "RoleACClass", Description = "Role (BSO type) selected in the available-roles list, used to assign a new role to the current person.")]
        public gip.core.datamodel.ACClass SelectedACClass
        {
            get
            {
                return _SelectedACClass;
            }
            set
            {
                _SelectedACClass = value;
                OnPropertyChanged("SelectedACClass");
            }
        }

        /// <summary>
        /// Gets the AC class list.
        /// </summary>
        /// <value>The AC class list.</value>
        [ACPropertyList(614, "RoleACClass", Description = "Available roles (BSO types) that can be assigned to the current company person.")]
        public IEnumerable<gip.core.datamodel.ACClass> ACClassList
        {
            get
            {
                if (CurrentCompanyPerson == null)
                    return null;
                List<gip.core.datamodel.ACClass> bsos = new List<gip.core.datamodel.ACClass>();

                foreach (var acClass in DatabaseApp.ContextIPlus.RoleACClassList)
                {
                    // Nur ACClass die vom Type OrderTypes.WorkOrder sind
                    if (CompanyPersonRoleList.Where(c => c.RoleACClass.ACClassID == acClass.ACClassID).
                        Select(c => c).Any())
                        continue;

                    bsos.Add(acClass);
                }
                return bsos;

            }
        }


        /// <summary>
        /// The _ selected assigned AC class
        /// </summary>
        CompanyPersonRole _SelectedAssignedACClass;
        /// <summary>
        /// Gets or sets the selected assigned AC class.
        /// </summary>
        /// <value>The selected assigned AC class.</value>
        [ACPropertySelected(615, "AssignedACClass", Description = "The role assignment selected in the assigned-roles list of the current person.")]
        public CompanyPersonRole SelectedAssignedACClass
        {
            get
            {
                return _SelectedAssignedACClass;
            }
            set
            {
                _SelectedAssignedACClass = value;
                OnPropertyChanged("SelectedAssignedACClass");
            }
        }

        /// <summary>
        /// Gets the company person role list.
        /// </summary>
        /// <value>The company person role list.</value>
        [ACPropertyList(616, "AssignedACClass", Description = "Role assignments of the current company person, ordered by role caption.")]
        public IEnumerable<CompanyPersonRole> CompanyPersonRoleList
        {
            get
            {
                if (CurrentCompanyPerson == null)
                    return null;
                return CurrentCompanyPerson.CompanyPersonRole_CompanyPerson.OrderBy(c => c.RoleACClass.ACCaptionTranslation).Select(c => c);
            }
        }

        #endregion

        #region 1.3 Factory
        /// <summary>
        /// The _ current factory
        /// </summary>
        Company _CurrentFactory;
        /// <summary>
        /// Gets or sets the current factory.
        /// </summary>
        /// <value>The current factory.</value>
        [ACPropertyCurrent(617, "Factory", Description = "The currently edited plant (subordinate company) of the current company.")]
        public Company CurrentFactory
        {
            get
            {
                return _CurrentFactory;
            }
            set
            {
                _CurrentFactory = value;
                OnPropertyChanged("CurrentFactory");
            }
        }

        /// <summary>
        /// The _ selected factory
        /// </summary>
        Company _SelectedFactory;
        /// <summary>
        /// Gets or sets the selected factory.
        /// </summary>
        /// <value>The selected factory.</value>
        [ACPropertySelected(618, "Factory", Description = "The plant selected in the factory list. LoadFactory() copies it to CurrentFactory.")]
        public Company SelectedFactory
        {
            get
            {
                return _SelectedFactory;
            }
            set
            {
                _SelectedFactory = value;
                OnPropertyChanged("SelectedFactory");
            }
        }

        /// <summary>
        /// Gets the factory list.
        /// </summary>
        /// <value>The factory list.</value>
        [ACPropertyList(619, "Factory", Description = "Plants (subordinate companies) of the current company.")]
        public IEnumerable<Company> FactoryList
        {
            get
            {
                if (CurrentCompany == null)
                    return null;
                return CurrentCompany.Company_ParentCompany;
            }
        }
        #endregion

        #region 1.4 CompanyMaterial


        /// <summary>
        /// The _ current company person
        /// </summary>
        CompanyMaterial _CurrentCompanyMaterial;
        /// <summary>
        /// Gets or sets the current company person.
        /// </summary>
        /// <value>The current company person.</value>
        [ACPropertyCurrent(620, CompanyMaterial.ClassName, Description = "The currently edited material mapping of the company (customer/supplier material number).")]
        public CompanyMaterial CurrentCompanyMaterial
        {
            get
            {
                return _CurrentCompanyMaterial;
            }
            set
            {
                _CurrentCompanyMaterial = value;
                OnPropertyChanged(CompanyMaterial.ClassName);
            }
        }

        /// <summary>
        /// Gets the company person list.
        /// </summary>
        /// <value>The company person list.</value>
        [ACPropertyList(621, CompanyMaterial.ClassName, Description = "Material mappings of the current company.")]
        public IEnumerable<CompanyMaterial> CompanyMaterialList
        {
            get
            {
                if (CurrentCompany == null)
                    return null;
                return CurrentCompany.CompanyMaterial_Company.Where(c => c.CMTypeIndex == (short)GlobalApp.CompanyMaterialTypes.MaterialMapping).ToList();
            }
        }

        /// <summary>
        /// The _ selected company person
        /// </summary>
        CompanyMaterial _SelectedCompanyMaterial;
        /// <summary>
        /// Gets or sets the selected company person.
        /// </summary>
        /// <value>The selected company person.</value>
        [ACPropertySelected(622, CompanyMaterial.ClassName, Description = "The material mapping selected in the material mapping list. LoadCompanyMaterial() copies it to CurrentCompanyMaterial.")]
        public CompanyMaterial SelectedCompanyMaterial
        {
            get
            {
                return _SelectedCompanyMaterial;
            }
            set
            {
                _SelectedCompanyMaterial = value;
                OnPropertyChanged("SelectedCompanyMaterial");
            }
        }

        /// <summary>
        /// The _ current company person
        /// </summary>
        CompanyMaterial _CurrentCompanyMaterialPickup;
        /// <summary>
        /// Gets or sets the current company person.
        /// </summary>
        /// <value>The current company person.</value>
        [ACPropertyCurrent(623, "CompanyMaterialPickup", Description = "The currently edited pickup relation of the company.")]
        public CompanyMaterial CurrentCompanyMaterialPickup
        {
            get
            {
                return _CurrentCompanyMaterialPickup;
            }
            set
            {
                _CurrentCompanyMaterialPickup = value;
                OnPropertyChanged("CompanyMaterialPickup");
            }
        }

        /// <summary>
        /// Gets the company person list.
        /// </summary>
        /// <value>The company person list.</value>
        [ACPropertyList(624, "CompanyMaterialPickup", Description = "Pickup relations of the current company.")]
        public IEnumerable<CompanyMaterial> CompanyMaterialPickupList
        {
            get
            {
                if (CurrentCompany == null)
                    return null;
                return CurrentCompany.CompanyMaterial_Company.Where(c => c.CMTypeIndex == (short)GlobalApp.CompanyMaterialTypes.Pickup).ToList();
            }
        }

        /// <summary>
        /// The _ selected company person
        /// </summary>
        CompanyMaterial _SelectedCompanyMaterialPickup;
        /// <summary>
        /// Gets or sets the selected company person.
        /// </summary>
        /// <value>The selected company person.</value>
        [ACPropertySelected(625, "CompanyMaterialPickup", Description = "The pickup relation selected in the pickup list. LoadCompanyMaterialPickup() copies it to CurrentCompanyMaterialPickup.")]
        public CompanyMaterial SelectedCompanyMaterialPickup
        {
            get
            {
                return _SelectedCompanyMaterialPickup;
            }
            set
            {
                _SelectedCompanyMaterialPickup = value;
                OnPropertyChanged("SelectedCompanyMaterialPickup");
            }
        }
        #endregion

        #region 1.5 CompanyMaterialStock
        /// <summary>
        /// Gets the company person list.
        /// </summary>
        /// <value>The company person list.</value>
        [ACPropertyList(626, "CompanyMaterialStock", Description = "Read-only stock information of the company materials of the current company, ordered by company material number.")]
        public IEnumerable<CompanyMaterialStock> CompanyMaterialStockList
        {
            get
            {
                if (CurrentCompany == null)
                    return null;
                var query = DatabaseApp.CompanyMaterialStock.Where(c => c.CompanyMaterial.CompanyID == CurrentCompany.CompanyID).OrderBy(c => c.CompanyMaterial.CompanyMaterialNo).ToList();
                //(query as ObjectQuery).MergeOption = MergeOption.OverwriteChanges;
                return query;
            }
        }

        /// <summary>
        /// The _ selected company person
        /// </summary>
        CompanyMaterialStock _SelectedCompanyMaterialStock;
        /// <summary>
        /// Gets or sets the selected company person.
        /// </summary>
        /// <value>The selected company person.</value>
        [ACPropertySelected(627, "CompanyMaterialStock", Description = "The stock entry selected in the company material stock list.")]
        public CompanyMaterialStock SelectedCompanyMaterialStock
        {
            get
            {
                return _SelectedCompanyMaterialStock;
            }
            set
            {
                _SelectedCompanyMaterialStock = value;
                OnPropertyChanged("SelectedCompanyMaterialStock");
            }
        }
        #endregion

        #endregion

        #region BSO->ACMethod
        #region 1. Company
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Company.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves all changes of the current database context (company and its addresses, departments, persons, factories, material mappings and pickups). Enabled when the database has changes and the current company has a company name.")]
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

        protected override bool OnIsEnabledSave()
        {
            if (CurrentCompany == null)
                return false;
            return DatabaseApp.IsChanged && !string.IsNullOrEmpty(CurrentCompany.CompanyName);
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand(Company.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards all unsaved changes of the current database context and clears the person search filter.")]
        public void UndoSave()
        {
            OnUndoSave();
        }

        protected override void OnPostUndoSave()
        {
            if (AccessPerson != null)
            {
                ACFilterItem filterItem = AccessPerson.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "Company\\CompanyNo").FirstOrDefault();
                if (filterItem != null)
                    filterItem.SearchWord = "";
                OnPropertyChanged("CompanyPersonList");
            }
            base.OnPostUndoSave();
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
        [ACMethodInteraction(Company.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedCompany", Global.ACKinds.MSMethodPrePost, Description = "Reloads the selected company with all its addresses, factories, material mappings and persons from the database, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Company>(requery, () => SelectedCompany, () => CurrentCompany, c => CurrentCompany = c,
                        DatabaseApp.Company
                        .Include(c => c.CompanyAddress_Company)
                        .Include(c => c.Company_ParentCompany)
                        .Include(c => c.CompanyMaterial_Company)
                        .Include(c => c.CompanyPerson_Company)
                        .Where(c => c.CompanyID == SelectedCompany.CompanyID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedCompany != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(Company.ClassName, Const.New, (short)MISort.New, true, "SelectedCompany", Global.ACKinds.MSMethodPrePost, Description = "Creates a new company with a generated company number and an empty house address and assigns it to CurrentCompany. Enter at least the company name, then call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Company), Company.NoColumnName, Company.FormatNewNo, this);
            CurrentCompany = Company.NewACObject(DatabaseApp, null, secondaryKey);

            DatabaseApp.Company.Add(CurrentCompany);
            DatabaseApp.CompanyAddress.Add(CurrentCompany.HouseCompanyAddress);
            CurrentHouseAdress = CurrentCompany.HouseCompanyAddress;
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
        [ACMethodInteraction(Company.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentCompany", Global.ACKinds.MSMethodPrePost, Description = "Deletes the current company after user confirmation. Fails if foreign key relationships exist from other tables. Call Save() afterwards to execute the delete in the database.")]
        public async void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentCompany.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Global.MsgResult result = await Messages.MsgAsync(msg, Global.MsgResult.No, eMsgButton.YesNo);
                if (result == Global.MsgResult.Yes)
                {
                    msg = CurrentCompany.DeleteACObject(DatabaseApp, false);
                    if (msg != null)
                    {
                        await Messages.MsgAsync(msg);
                        return;
                    }
                }
            }

            PostExecute("Delete");
            OnPropertyChanged("CompanyList");
            SelectedCompany = AccessPrimary.NavList.FirstOrDefault();
            Load();
            Search();
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentCompany != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Company.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Executes the company search with the current SearchWord and refreshes the CompanyList.")]
        public void Search()
        {
            AccessPrimary.NavSearch(DatabaseApp, MergeOption.OverwriteChanges);
            OnPropertyChanged("CompanyList");
        }
        #endregion

        #region 1.1 CompanyAddress
        /// <summary>
        /// Loads the company address.
        /// </summary>
        [ACMethodInteraction(CompanyAddress.ClassName, "en{'Load Address'}de{'Adresse laden'}", (short)MISort.Load, false, "SelectedCompanyAddress", Global.ACKinds.MSMethodPrePost, Description = "Loads the address selected in the address list into CurrentCompanyAddress for editing.")]
        public void LoadCompanyAddress()
        {
            if (!IsEnabledLoadCompanyAddress())
                return;
            if (!PreExecute("LoadCompanyAddress")) return;
            CurrentCompanyAddress = CurrentCompany.CompanyAddress_Company.Where(c => c.CompanyAddressID == SelectedCompanyAddress.CompanyAddressID).FirstOrDefault();
            PostExecute("LoadCompanyAddress");
        }

        /// <summary>
        /// Determines whether [is enabled load company address].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load company address]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadCompanyAddress()
        {
            return SelectedCompanyAddress != null;
        }

        /// <summary>
        /// News the company address.
        /// </summary>
        [ACMethodInteraction(CompanyAddress.ClassName, "en{'New Address'}de{'Neue Adresse'}", (short)MISort.New, true, "SelectedCompanyAddress", Global.ACKinds.MSMethodPrePost, Description = "Creates a new address for the current company and assigns it to CurrentCompanyAddress. Call Save() to persist.")]
        public void NewCompanyAddress()
        {
            if (!PreExecute("NewCompanyAddress")) return;
            CurrentCompanyAddress = CompanyAddress.NewACObject(DatabaseApp, CurrentCompany);
            CurrentCompany.CompanyAddress_Company.Add(CurrentCompanyAddress);
            DatabaseApp.CompanyAddress.Add(CurrentCompanyAddress);
            PostExecute("NewCompanyAddress");
            OnPropertyChanged("CompanyAddressList");
        }

        /// <summary>
        /// Determines whether [is enabled new company address].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new company address]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewCompanyAddress()
        {
            return CurrentCompany != null;
        }

        /// <summary>
        /// Deletes the company address.
        /// </summary>
        [ACMethodInteraction(CompanyAddress.ClassName, "en{'Delete Address'}de{'Adresse löschen'}", (short)MISort.Delete, true, "CurrentCompanyAddress", Global.ACKinds.MSMethodPrePost, Description = "Deletes the current company address. Call Save() to persist.")]
        public void DeleteCompanyAddress()
        {
            if (!PreExecute("DeleteCompanyAddress")) return;
            CurrentCompanyAddress.DeleteACObject(DatabaseApp, true);
            PostExecute("DeleteCompanyAddress");
            OnPropertyChanged("CompanyAddressList");
        }

        /// <summary>
        /// Determines whether [is enabled delete company address].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete company address]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteCompanyAddress()
        {
            return CurrentCompany != null && CurrentCompanyAddress != null;
        }
        #endregion

        #region 1.1.1 CompanyAddressDepartment
        /// <summary>
        /// News the company address department.
        /// </summary>
        [ACMethodInteraction("CompanyAddressDepartment", "en{'New Department'}de{'Neue Abteilung'}", (short)MISort.New, true, "SelectedCompanyAddressDepartment", Global.ACKinds.MSMethodPrePost, Description = "Creates a new department below the current company address. Call Save() to persist.")]
        public void NewCompanyAddressDepartment()
        {
            if (!PreExecute("NewCompanyAddressDepartment")) return;
            CurrentCompanyAddressDepartment = CompanyAddressDepartment.NewACObject(DatabaseApp, CurrentCompanyAddress);
            CurrentCompanyAddressDepartment.CompanyAddress = CurrentCompanyAddress;
            OnPropertyChanged("CompanyAddressDepartmentList");
            PostExecute("NewCompanyAddressDepartment");
        }

        /// <summary>
        /// Determines whether [is enabled new company address department].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new company address department]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewCompanyAddressDepartment()
        {
            return CurrentCompany != null;
        }

        /// <summary>
        /// Deletes the company address department.
        /// </summary>
        [ACMethodInteraction("Factory", "en{'Delete Department'}de{'Abteilung löschen'}", (short)MISort.Delete, true, "CurrentCompanyAddressDepartment", Global.ACKinds.MSMethodPrePost, Description = "Deletes the department of the current company address. Call Save() to persist.")]
        public void DeleteCompanyAddressDepartment()
        {
            if (CurrentCompanyAddressDepartment != null)
            {
                if (!PreExecute("DeleteCompanyAddressDepartment")) return;
                CurrentCompanyAddressDepartment.DeleteACObject(DatabaseApp, true);
                //CurrentCompanyAddress.CompanyAddressDepartment_CompanyAddress.Remove(CurrentCompanyAddressDepartment);
                OnPropertyChanged("CompanyAddressDepartmentList");
                PostExecute("DeleteCompanyAddressDepartment");
            }
        }

        /// <summary>
        /// Determines whether [is enabled delete company address department].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete company address department]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteCompanyAddressDepartment()
        {
            return CurrentCompany != null && CurrentCompanyAddress != null && CurrentCompanyAddressDepartment != null;
        }
        #endregion

        #region 1.2 CompanyPerson
        /// <summary>
        /// Loads the company person.
        /// </summary>
        [ACMethodInteraction(CompanyPerson.ClassName, "en{'Load Person'}de{'Person laden'}", (short)MISort.Load, false, "SelectedCompanyPerson", Global.ACKinds.MSMethodPrePost, Description = "Loads the person selected in the person list into CurrentCompanyPerson for editing.")]
        public void LoadCompanyPerson()
        {
            if (!IsEnabledLoadCompanyPerson())
                return;
            if (!PreExecute("LoadCompanyPerson")) 
                return;
            if (SelectedCompanyPerson == null || CurrentCompany == null)
            {
                CurrentCompanyPerson = null;
                return;
            }
            CurrentCompanyPerson = CurrentCompany.CompanyPerson_Company.ToArray().Where(c => c.CompanyPersonID == SelectedCompanyPerson.CompanyPersonID).FirstOrDefault();
            PostExecute("LoadCompanyPerson");
        }

        /// <summary>
        /// Determines whether [is enabled load company person].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load company person]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadCompanyPerson()
        {
            return SelectedCompanyPerson != null;
        }

        /// <summary>
        /// News the company person.
        /// </summary>
        [ACMethodInteraction(CompanyPerson.ClassName, "en{'New Person'}de{'Neue Person'}", (short)MISort.New, true, "SelectedCompanyPerson", Global.ACKinds.MSMethodPrePost, Description = "Creates a new contact person with a generated person number for the current company. Call Save() to persist.")]
        public void NewCompanyPerson()
        {
            if (!PreExecute("NewCompanyPerson")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(CompanyPerson), CompanyPerson.NoColumnName, CompanyPerson.FormatNewNo, this);
            CurrentCompanyPerson = CompanyPerson.NewACObject(DatabaseApp, CurrentCompany, secondaryKey);
            CurrentCompany.CompanyPerson_Company.Add(CurrentCompanyPerson);
            //DatabaseApp.CompanyPerson.AddObject(CurrentCompanyPerson);
            AccessPerson.NavList.Add(CurrentCompanyPerson);
            PostExecute("NewCompanyPerson");
            OnPropertyChanged("CompanyPersonList");
        }

        /// <summary>
        /// Determines whether [is enabled new company person].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new company person]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewCompanyPerson()
        {
            return CurrentCompany != null;
        }

        /// <summary>
        /// Deletes the company person.
        /// </summary>
        [ACMethodInteraction(CompanyPerson.ClassName, "en{'Delete Person'}de{'Person löschen'}", (short)MISort.Delete, true, "CurrentCompanyPerson", Global.ACKinds.MSMethodPrePost, Description = "Deletes the current contact person and removes it from the person list. Call Save() to persist.")]
        public void DeleteCompanyPerson()
        {
            if (!PreExecute("DeleteCompanyPerson"))
                return;
            if (AccessPerson == null)
                return;
            CurrentCompanyPerson.DeleteACObject(DatabaseApp, true);
            AccessPerson.NavList.Remove(CurrentCompanyPerson);
            //CurrentCompany.CompanyPerson_Company.Remove(CurrentCompanyPerson);
            CurrentCompanyPerson.Company = null;
            OnPropertyChanged("CompanyPersonList");
            PostExecute("DeleteCompanyPerson");
        }

        /// <summary>
        /// Determines whether [is enabled delete company person].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete company person]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteCompanyPerson()
        {
            return CurrentCompany != null && CurrentCompanyPerson != null;
        }
        #endregion

        #region 1.3 Factory
        /// <summary>
        /// News the factory.
        /// </summary>
        [ACMethodInteraction("Factory", "en{'New Factory'}de{'Neues Werk'}", (short)MISort.New, true, "SelectedFactoy", Global.ACKinds.MSMethodPrePost, Description = "Creates a new plant (subordinate company) below the current company. Call Save() to persist.")]
        public void NewFactory()
        {
            if (!PreExecute("NewFactory")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Company), Company.NoColumnName, Company.FormatNewNo, this);
            CurrentFactory = Company.NewACObject(DatabaseApp, CurrentCompany, secondaryKey);
            CurrentFactory.Company1_ParentCompany = CurrentCompany;
            PostExecute("NewFactory");
        }

        /// <summary>
        /// Determines whether [is enabled new factory].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new factory]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewFactory()
        {
            return CurrentCompany != null;
        }

        /// <summary>
        /// Loads the factory.
        /// </summary>
        [ACMethodInteraction("Factory", "en{'Load Factory'}de{'Werk laden'}", (short)MISort.Load, false, "SelectedFactoy", Global.ACKinds.MSMethodPrePost, Description = "Loads the plant selected in the factory list into CurrentFactory for editing.")]
        public void LoadFactory()
        {
            if (!PreExecute("LoadFactory")) return;
            CurrentFactory = CurrentCompany.Company_ParentCompany.Where(c => c.CompanyID == SelectedFactory.CompanyID).FirstOrDefault();
            PostExecute("LoadFactory");
        }

        /// <summary>
        /// Determines whether [is enabled load factory].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load factory]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadFactory()
        {
            return SelectedFactory != null;
        }

        /// <summary>
        /// Deletes the factory.
        /// </summary>
        [ACMethodInteraction("Factory", "en{'Delete Factory'}de{'Werk löschen'}", (short)MISort.Delete, true, "CurrentFactory", Global.ACKinds.MSMethodPrePost, Description = "Deletes the current plant. Call Save() to persist.")]
        public void DeleteFactory()
        {
            if (CurrentFactory != null)
            {
                if (!PreExecute("DeleteFactory")) return;
                CurrentFactory.DeleteACObject(DatabaseApp, true);
                //CurrentCompany.Company_ParentCompany.Remove(CurrentFactory);
                PostExecute("DeleteFactory");
                OnPropertyChanged("FactoryList");
            }
        }

        /// <summary>
        /// Determines whether [is enabled delete factory].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete factory]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteFactory()
        {
            return CurrentCompany != null && CurrentFactory != null;
        }
        #endregion

        #region 1.4 CompanyMaterial
        /// <summary>
        /// Loads the company person.
        /// </summary>
        [ACMethodInteraction(CompanyMaterial.ClassName, "en{'Load Material'}de{'Material laden'}", (short)MISort.Load, false, "SelectedCompanyMaterial", Global.ACKinds.MSMethodPrePost, Description = "Loads the material mapping selected in the list into CurrentCompanyMaterial for editing.")]
        public void LoadCompanyMaterial()
        {
            if (!IsEnabledLoadCompanyMaterial())
                return;
            if (!PreExecute("LoadCompanyMaterial")) return;
            CurrentCompanyMaterial = CurrentCompany.CompanyMaterial_Company.Where(c => c.CompanyMaterialID == SelectedCompanyMaterial.CompanyMaterialID).FirstOrDefault();
            PostExecute("LoadCompanyMaterial");
        }

        /// <summary>
        /// Determines whether [is enabled load company person].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load company person]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadCompanyMaterial()
        {
            return SelectedCompanyMaterial != null;
        }

        /// <summary>
        /// News the company person.
        /// </summary>
        [ACMethodInteraction(CompanyMaterial.ClassName, "en{'New Material'}de{'Neues Material'}", (short)MISort.New, true, "SelectedCompanyMaterial", Global.ACKinds.MSMethodPrePost, Description = "Creates a new material mapping for the current company and assigns it to CurrentCompanyMaterial. Assign a material and call Save() to persist.")]
        public void NewCompanyMaterial()
        {
            if (!PreExecute("NewCompanyMaterial")) return;
            CurrentCompanyMaterial = CompanyMaterial.NewACObject(DatabaseApp, CurrentCompany);
            CurrentCompanyMaterial.CMMype = GlobalApp.CompanyMaterialTypes.MaterialMapping;
            CurrentCompany.CompanyMaterial_Company.Add(CurrentCompanyMaterial);
            DatabaseApp.CompanyMaterial.Add(CurrentCompanyMaterial);
            PostExecute("NewCompanyMaterial");
            OnPropertyChanged("CompanyMaterialList");
        }

        /// <summary>
        /// Determines whether [is enabled new company person].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new company person]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewCompanyMaterial()
        {
            return CurrentCompany != null;
        }

        /// <summary>
        /// Deletes the company person.
        /// </summary>
        [ACMethodInteraction(CompanyMaterial.ClassName, "en{'Delete Material'}de{'Material löschen'}", (short)MISort.Delete, true, "CurrentCompanyMaterial", Global.ACKinds.MSMethodPrePost, Description = "Deletes the current material mapping. Call Save() to persist.")]
        public void DeleteCompanyMaterial()
        {
            if (!PreExecute("DeleteCompanyMaterial")) return;
            CurrentCompanyMaterial.DeleteACObject(DatabaseApp, true);
            //CurrentCompany.CompanyMaterial_Company.Remove(CurrentCompanyMaterial);
            OnPropertyChanged("CompanyMaterialList");
            PostExecute("DeleteCompanyMaterial");
        }

        /// <summary>
        /// Determines whether [is enabled delete company person].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete company person]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteCompanyMaterial()
        {
            return CurrentCompany != null && CurrentCompanyMaterial != null;
        }

        private string _FindCompanyWithCMaterialNo;
        [ACPropertyInfo(628, "", "en{'Find company with material no.'}de{'Suche Firma mit Material-Nr.'}", Description = "Search field: enter a company material number to find and load the company that owns this material mapping.")]
        public string FindCompanyWithCMaterialNo
        {
            get
            {
                return _FindCompanyWithCMaterialNo;
            }
            set
            {
                if (!String.IsNullOrEmpty(value) && _FindCompanyWithCMaterialNo != value)
                {
                    CompanyMaterial foundCMaterial = DatabaseApp.CompanyMaterial.Where(c => c.CompanyMaterialNo == value && c.CMTypeIndex == (short)GlobalApp.CompanyMaterialTypes.MaterialMapping).AutoMergeOption(DatabaseApp).FirstOrDefault();
                    if (foundCMaterial != null)
                    {
                        SelectedCompany = foundCMaterial.Company;
                        Load();
                    }
                }
                _FindCompanyWithCMaterialNo = value;
                OnPropertyChanged("FindCompanyWithCMaterialNo");
            }
        }


        /// <summary>
        /// Loads the company person.
        /// </summary>
        [ACMethodInteraction("CompanyMaterialPickup", "en{'Load pickup'}de{'Abholung laden'}", (short)MISort.Load, false, "SelectedCompanyMaterialPickup", Global.ACKinds.MSMethodPrePost, Description = "Loads the pickup relation selected in the list into CurrentCompanyMaterialPickup for editing.")]
        public void LoadCompanyMaterialPickup()
        {
            if (!IsEnabledLoadCompanyMaterialPickup())
                return;
            if (!PreExecute("LoadCompanyMaterialPickup")) return;
            CurrentCompanyMaterialPickup = CurrentCompany.CompanyMaterial_Company.Where(c => c.CompanyMaterialID == SelectedCompanyMaterialPickup.CompanyMaterialID).FirstOrDefault();
            PostExecute("LoadCompanyMaterialPickup");
        }

        /// <summary>
        /// Determines whether [is enabled load company person].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load company person]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadCompanyMaterialPickup()
        {
            return SelectedCompanyMaterialPickup != null;
        }

        /// <summary>
        /// News the company person.
        /// </summary>
        [ACMethodInteraction("CompanyMaterialPickup", "en{'New pickup'}de{'Neue Abholung'}", (short)MISort.New, true, "SelectedCompanyMaterialPickup", Global.ACKinds.MSMethodPrePost, Description = "Creates a new pickup relation for the current company and assigns it to CurrentCompanyMaterialPickup. Assign a material and call Save() to persist.")]
        public void NewCompanyMaterialPickup()
        {
            if (!PreExecute("NewCompanyMaterialPickup")) return;
            CurrentCompanyMaterialPickup = CompanyMaterial.NewACObject(DatabaseApp, CurrentCompany);
            CurrentCompanyMaterialPickup.CMMype = GlobalApp.CompanyMaterialTypes.Pickup;
            CurrentCompany.CompanyMaterial_Company.Add(CurrentCompanyMaterialPickup);
            DatabaseApp.CompanyMaterial.Add(CurrentCompanyMaterialPickup);
            PostExecute("NewCompanyMaterialPickup");
            OnPropertyChanged("CompanyMaterialPickupList");
        }

        /// <summary>
        /// Determines whether [is enabled new company person].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new company person]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewCompanyMaterialPickup()
        {
            return CurrentCompany != null;
        }

        /// <summary>
        /// Deletes the company person.
        /// </summary>
        [ACMethodInteraction("CompanyMaterialPickup", "en{'Delete pickup'}de{'Abholung löschen'}", (short)MISort.Delete, true, "CurrentCompanyMaterialPickup", Global.ACKinds.MSMethodPrePost, Description = "Deletes the current pickup relation. Call Save() to persist.")]
        public void DeleteCompanyMaterialPickup()
        {
            if (!PreExecute("DeleteCompanyMaterialPickup")) return;
            CurrentCompanyMaterialPickup.DeleteACObject(DatabaseApp, true);
            //CurrentCompany.CompanyMaterial_Company.Remove(CurrentCompanyMaterial);
            OnPropertyChanged("CompanyMaterialPickupList");
            PostExecute("DeleteCompanyMaterialPickup");
        }

        /// <summary>
        /// Determines whether [is enabled delete company person].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete company person]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteCompanyMaterialPickup()
        {
            return CurrentCompany != null && CurrentCompanyMaterialPickup != null;
        }

        private string _FindCompanyWithPickupNo;
        [ACPropertyInfo(629, "", "en{'Find company with pickup no.'}de{'Suche Firma mit Abhol-Nr.'}", Description = "Search field: enter a pickup number to find and load the company that owns this pickup relation.")]
        public string FindCompanyWithPickupNo
        {
            get
            {
                return _FindCompanyWithPickupNo;
            }
            set
            {
                if (!String.IsNullOrEmpty(value) && _FindCompanyWithPickupNo != value)
                {
                    CompanyMaterial foundPickup = DatabaseApp.CompanyMaterial.Where(c => c.CompanyMaterialNo == value && c.CMTypeIndex == (short)GlobalApp.CompanyMaterialTypes.Pickup).AutoMergeOption(DatabaseApp).FirstOrDefault();
                    if (foundPickup != null)
                    {
                        SelectedCompany = foundPickup.Company;
                        Load(true);
                    }
                }
                _FindCompanyWithPickupNo = value;
                OnPropertyChanged("FindCompanyWithPickupNo");
            }
        }

        #endregion

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
                case nameof(LoadCompanyAddress):
                    LoadCompanyAddress();
                    return true;
                case nameof(IsEnabledLoadCompanyAddress):
                    result = IsEnabledLoadCompanyAddress();
                    return true;
                case nameof(NewCompanyAddress):
                    NewCompanyAddress();
                    return true;
                case nameof(IsEnabledNewCompanyAddress):
                    result = IsEnabledNewCompanyAddress();
                    return true;
                case nameof(DeleteCompanyAddress):
                    DeleteCompanyAddress();
                    return true;
                case nameof(IsEnabledDeleteCompanyAddress):
                    result = IsEnabledDeleteCompanyAddress();
                    return true;
                case nameof(NewCompanyAddressDepartment):
                    NewCompanyAddressDepartment();
                    return true;
                case nameof(IsEnabledNewCompanyAddressDepartment):
                    result = IsEnabledNewCompanyAddressDepartment();
                    return true;
                case nameof(DeleteCompanyAddressDepartment):
                    DeleteCompanyAddressDepartment();
                    return true;
                case nameof(IsEnabledDeleteCompanyAddressDepartment):
                    result = IsEnabledDeleteCompanyAddressDepartment();
                    return true;
                case nameof(LoadCompanyPerson):
                    LoadCompanyPerson();
                    return true;
                case nameof(IsEnabledLoadCompanyPerson):
                    result = IsEnabledLoadCompanyPerson();
                    return true;
                case nameof(NewCompanyPerson):
                    NewCompanyPerson();
                    return true;
                case nameof(IsEnabledNewCompanyPerson):
                    result = IsEnabledNewCompanyPerson();
                    return true;
                case nameof(DeleteCompanyPerson):
                    DeleteCompanyPerson();
                    return true;
                case nameof(IsEnabledDeleteCompanyPerson):
                    result = IsEnabledDeleteCompanyPerson();
                    return true;
                case nameof(NewFactory):
                    NewFactory();
                    return true;
                case nameof(IsEnabledNewFactory):
                    result = IsEnabledNewFactory();
                    return true;
                case nameof(LoadFactory):
                    LoadFactory();
                    return true;
                case nameof(IsEnabledLoadFactory):
                    result = IsEnabledLoadFactory();
                    return true;
                case nameof(DeleteFactory):
                    DeleteFactory();
                    return true;
                case nameof(IsEnabledDeleteFactory):
                    result = IsEnabledDeleteFactory();
                    return true;
                case nameof(LoadCompanyMaterial):
                    LoadCompanyMaterial();
                    return true;
                case nameof(IsEnabledLoadCompanyMaterial):
                    result = IsEnabledLoadCompanyMaterial();
                    return true;
                case nameof(NewCompanyMaterial):
                    NewCompanyMaterial();
                    return true;
                case nameof(IsEnabledNewCompanyMaterial):
                    result = IsEnabledNewCompanyMaterial();
                    return true;
                case nameof(DeleteCompanyMaterial):
                    DeleteCompanyMaterial();
                    return true;
                case nameof(IsEnabledDeleteCompanyMaterial):
                    result = IsEnabledDeleteCompanyMaterial();
                    return true;
                case nameof(LoadCompanyMaterialPickup):
                    LoadCompanyMaterialPickup();
                    return true;
                case nameof(IsEnabledLoadCompanyMaterialPickup):
                    result = IsEnabledLoadCompanyMaterialPickup();
                    return true;
                case nameof(NewCompanyMaterialPickup):
                    NewCompanyMaterialPickup();
                    return true;
                case nameof(IsEnabledNewCompanyMaterialPickup):
                    result = IsEnabledNewCompanyMaterialPickup();
                    return true;
                case nameof(DeleteCompanyMaterialPickup):
                    DeleteCompanyMaterialPickup();
                    return true;
                case nameof(IsEnabledDeleteCompanyMaterialPickup):
                    result = IsEnabledDeleteCompanyMaterialPickup();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(Load):
                case nameof(IsEnabledLoad):
                    return new string[] { nameof(SelectedCompany) };
                case nameof(New):
                case nameof(IsEnabledNew):
                    return new string[] { nameof(InitState) };
                case nameof(Delete):
                case nameof(IsEnabledDelete):
                    return new string[] { nameof(CurrentCompany) };
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                case nameof(LoadCompanyAddress):
                case nameof(IsEnabledLoadCompanyAddress):
                    return new string[] { nameof(SelectedCompanyAddress) };
                case nameof(NewCompanyAddress):
                case nameof(IsEnabledNewCompanyAddress):
                    return new string[] { nameof(CurrentCompany) };
                case nameof(DeleteCompanyAddress):
                case nameof(IsEnabledDeleteCompanyAddress):
                    return new string[] { nameof(CurrentCompanyAddress) };
                case nameof(NewCompanyAddressDepartment):
                case nameof(IsEnabledNewCompanyAddressDepartment):
                    return new string[] { nameof(CurrentCompany) };
                case nameof(DeleteCompanyAddressDepartment):
                case nameof(IsEnabledDeleteCompanyAddressDepartment):
                    return new string[] { nameof(CurrentCompanyAddressDepartment) };
                case nameof(LoadCompanyPerson):
                case nameof(IsEnabledLoadCompanyPerson):
                    return new string[] { nameof(SelectedCompanyPerson) };
                case nameof(NewCompanyPerson):
                case nameof(IsEnabledNewCompanyPerson):
                    return new string[] { nameof(CurrentCompany) };
                case nameof(DeleteCompanyPerson):
                case nameof(IsEnabledDeleteCompanyPerson):
                    return new string[] { nameof(CurrentCompanyPerson) };
                case nameof(NewFactory):
                case nameof(IsEnabledNewFactory):
                    return new string[] { nameof(CurrentCompany) };
                case nameof(LoadFactory):
                case nameof(IsEnabledLoadFactory):
                    return new string[] { nameof(SelectedFactory) };
                case nameof(DeleteFactory):
                case nameof(IsEnabledDeleteFactory):
                    return new string[] { nameof(CurrentFactory) };
                case nameof(LoadCompanyMaterial):
                case nameof(IsEnabledLoadCompanyMaterial):
                    return new string[] { nameof(SelectedCompanyMaterial) };
                case nameof(NewCompanyMaterial):
                case nameof(IsEnabledNewCompanyMaterial):
                    return new string[] { nameof(CurrentCompany) };
                case nameof(DeleteCompanyMaterial):
                case nameof(IsEnabledDeleteCompanyMaterial):
                    return new string[] { nameof(CurrentCompanyMaterial) };
                case nameof(LoadCompanyMaterialPickup):
                case nameof(IsEnabledLoadCompanyMaterialPickup):
                    return new string[] { nameof(SelectedCompanyMaterialPickup) };
                case nameof(NewCompanyMaterialPickup):
                case nameof(IsEnabledNewCompanyMaterialPickup):
                    return new string[] { nameof(CurrentCompany) };
                case nameof(DeleteCompanyMaterialPickup):
                case nameof(IsEnabledDeleteCompanyMaterialPickup):
                    return new string[] { nameof(CurrentCompanyMaterialPickup) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}