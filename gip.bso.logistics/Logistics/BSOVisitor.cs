// ***********************************************************************
// Assembly         : gip.bso.logistics
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOVisitor.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;

namespace gip.bso.logistics
{
    /// <summary>
    /// Version 3
    /// Neue Masken:
    /// 1. Angebotsverwaltung
    /// TODO: Betroffene Tabellen: Visitor, VisitorVoucher
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Visitor Registration'}de{'Besucher Registrierung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Visitor.ClassName)]
    public class BSOVisitor : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOVisitor"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOVisitor(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            this._AccessFacilityVehicle = null;
            this._AccessVisitorVoucher = null;
            this._CurrentAssignVisitorCard = null;
            this._CurrentVisitorVoucher = null;
            this._SelectedVisitorVoucher = null;
            this._AssignVisitorCardList = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessVisitorVoucher != null)
            {
                _AccessVisitorVoucher.ACDeInit(false);
                _AccessVisitorVoucher = null;
            }
            if (_AccessFacilityVehicle != null)
            {
                _AccessFacilityVehicle.ACDeInit(false);
                _AccessFacilityVehicle = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region BSO->ACProperty

        #region Visitor
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<Visitor> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, Visitor.ClassName)]
        public ACAccessNav<Visitor> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Visitor>(Visitor.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the current visitor.
        /// </summary>
        /// <value>The current visitor.</value>
        [ACPropertyCurrent(600, Visitor.ClassName)]
        public Visitor CurrentVisitor
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Current = value;
                OnPropertyChanged("CurrentVisitor");
                OnPropertyChanged("VisitorVoucherList");
                OnPropertyChanged("CompanyList");
                OnPropertyChanged("CustomerCompanyAddressList");
                OnPropertyChanged("DeliveryCompanyAddressList");
            }
        }

        /// <summary>
        /// Gets the visitor list.
        /// </summary>
        /// <value>The visitor list.</value>
        [ACPropertyList(601, Visitor.ClassName)]
        public IEnumerable<Visitor> VisitorList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected visitor.
        /// </summary>
        /// <value>The selected visitor.</value>
        [ACPropertySelected(602, Visitor.ClassName)]
        public Visitor SelectedVisitor
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.SelectedNavObject = value;
                OnPropertyChanged("SelectedVisitor");
            }
        }
        #endregion

        #region VisitorVoucher
        /// <summary>
        /// The _ access visitor voucher
        /// </summary>
        ACAccess<VisitorVoucher> _AccessVisitorVoucher;
        /// <summary>
        /// Gets the access visitor voucher.
        /// </summary>
        /// <value>The access visitor voucher.</value>
        [ACPropertyAccess(691, VisitorVoucher.ClassName)]
        public ACAccess<VisitorVoucher> AccessVisitorVoucher
        {
            get
            {
                if (_AccessVisitorVoucher == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + "VisitorVoucher") as ACQueryDefinition;
                    _AccessVisitorVoucher = acQueryDefinition.NewAccess<VisitorVoucher>(VisitorVoucher.ClassName, this);
                }
                return _AccessVisitorVoucher;
            }
        }


        /// <summary>
        /// The _ current visitor voucher
        /// </summary>
        VisitorVoucher _CurrentVisitorVoucher;
        /// <summary>
        /// Gets or sets the current visitor voucher.
        /// </summary>
        /// <value>The current visitor voucher.</value>
        [ACPropertyCurrent(603, VisitorVoucher.ClassName)]
        public VisitorVoucher CurrentVisitorVoucher
        {
            get
            {
                return _CurrentVisitorVoucher;
            }
            set
            {
                _CurrentVisitorVoucher = value;
                OnPropertyChanged("CurrentVisitorVoucher");
            }
        }

        /// <summary>
        /// Gets the visitor voucher list.
        /// </summary>
        /// <value>The visitor voucher list.</value>
        [ACPropertyList(604, VisitorVoucher.ClassName)]
        public IEnumerable<VisitorVoucher> VisitorVoucherList
        {
            get
            {
                if (CurrentVisitor == null)
                    return null;
                return CurrentVisitor.VisitorVoucher_Visitor.AsEnumerable();
            }
        }

        /// <summary>
        /// The _ selected visitor voucher
        /// </summary>
        VisitorVoucher _SelectedVisitorVoucher;
        /// <summary>
        /// Gets or sets the selected visitor voucher.
        /// </summary>
        /// <value>The selected visitor voucher.</value>
        [ACPropertySelected(605, VisitorVoucher.ClassName)]
        public VisitorVoucher SelectedVisitorVoucher
        {
            get
            {
                return _SelectedVisitorVoucher;
            }
            set
            {
                _SelectedVisitorVoucher = value;
                OnPropertyChanged("SelectedVisitorVoucher");
            }
        }
        #endregion

        #region Company
        /// <summary>
        /// Liste aller Unternehmen, die Lieferanten sind
        /// </summary>
        /// <value>The company list.</value>
        [ACPropertyList(606, Company.ClassName)]
        public IEnumerable<Company> CompanyList
        {
            get
            {
                return DatabaseApp.Company.Where( c => c.IsCustomer).OrderBy(c => c.CompanyName);
            }
        }

        /// <summary>
        /// Gets the customer company address list.
        /// </summary>
        /// <value>The customer company address list.</value>
        [ACPropertyList(607, "CustomerCompanyAddress")]
        public IEnumerable<CompanyAddress> CustomerCompanyAddressList
        {
            get
            {
                if (CurrentVisitor == null || CurrentVisitor.VisitedCompany == null)
                    return null;
                if (!CurrentVisitor.VisitedCompany.CompanyAddress_Company.IsLoaded)
                    CurrentVisitor.VisitedCompany.CompanyAddress_Company.Load();
                return CurrentVisitor.VisitedCompany.CompanyAddress_Company.Where(c => c.IsHouseCompanyAddress).OrderBy(c => c.Name1);
            }
        }

        /// <summary>
        /// Gets the delivery company address list.
        /// </summary>
        /// <value>The delivery company address list.</value>
        [ACPropertyList(608, "DeliveryCompanyAddress")]
        public IEnumerable<CompanyAddress> DeliveryCompanyAddressList
        {
            get
            {
                if (CurrentVisitor == null || CurrentVisitor.VisitedCompany == null)
                    return null;
                if (!CurrentVisitor.VisitedCompany.CompanyAddress_Company.IsLoaded)
                    CurrentVisitor.VisitedCompany.CompanyAddress_Company.Load();
                return CurrentVisitor.VisitedCompany.CompanyAddress_Company.Where(c => c.IsDeliveryCompanyAddress).OrderBy(c => c.Name1);
            }
        }
        #endregion

        #region CompanyPerson
        #endregion

        #region VisitorCard
        /// <summary>
        /// The _ current assign visitor card
        /// </summary>
        MDVisitorCard _CurrentAssignVisitorCard;
        /// <summary>
        /// Gets or sets the current assign visitor card.
        /// </summary>
        /// <value>The current assign visitor card.</value>
        [ACPropertyCurrent(609, "AssignVisitorCard", "en{'Visitor Card'}de{'Besucherkarte'}")]
        public MDVisitorCard CurrentAssignVisitorCard
        {
            get
            {
                return _CurrentAssignVisitorCard;
            }
            set
            {
                _CurrentAssignVisitorCard = value;
                OnPropertyChanged("_CurrentAssignVisitorCard");
            }
        }

        /// <summary>
        /// The _ assign visitor card list
        /// </summary>
        List<MDVisitorCard> _AssignVisitorCardList;
        /// <summary>
        /// Gets the assign visitor card list.
        /// </summary>
        /// <value>The assign visitor card list.</value>
        [ACPropertyList(610, "AssignVisitorCard", "en{'Visitor Card List'}de{'Besucherkartenliste'}")]
        public IEnumerable<MDVisitorCard> AssignVisitorCardList
        {
            get
            {
                _AssignVisitorCardList = new List<MDVisitorCard>();
                foreach (var mdVisitorCard in DatabaseApp.MDVisitorCard)
                {
                    if (!mdVisitorCard.Visitor_MDVisitorCard.Any())
                    {
                        _AssignVisitorCardList.Add(mdVisitorCard);
                    }
                }
                return _AssignVisitorCardList;
            }
        }
        #endregion

        #region Visitor-Vehicle
        ACAccess<Facility> _AccessFacilityVehicle;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(692, "Vehicle")]
        public ACAccess<Facility> AccessFacilityVehicle
        {
            get
            {
                if (_AccessFacilityVehicle == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "Facility", ACType.ACIdentifier);
                    bool rebuildACQueryDef = false;
                    short vehicle = (short)FacilityTypesEnum.Vehicle;
                    if (navACQueryDefinition.ACFilterColumns.Count <= 0)
                    {
                        rebuildACQueryDef = true;
                    }
                    else
                    {
                        int countFoundCorrect = 0;
                        foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            if (filterItem.PropertyName == "MDFacilityType\\MDFacilityTypeIndex")
                            {
                                if (filterItem.SearchWord == vehicle.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal)
                                    countFoundCorrect++;
                            }
                        }
                        if (countFoundCorrect < 1)
                            rebuildACQueryDef = true;
                    }
                    if (rebuildACQueryDef)
                    {
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, vehicle.ToString(), true));
                    }
                    _AccessFacilityVehicle = navACQueryDefinition.NewAccessNav<Facility>("Vehicle", this);
                }
                return _AccessFacilityVehicle;
            }
        }

        /// <summary>
        /// Gets the visitor voucher list.
        /// </summary>
        /// <value>The visitor voucher list.</value>
        [ACPropertyList(611, "Vehicle")]
        public IEnumerable<Facility> FacilityVehicleList
        {
            get
            {
                if (AccessFacilityVehicle == null)
                    return null;
                return AccessFacilityVehicle.NavList;
            }
        }



        #endregion

        #region BSO->ACMethod

        #region Visitor
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Visitor.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(Visitor.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(Visitor.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedVisitor", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Visitor>(requery, () => SelectedVisitor, () => CurrentVisitor, c => CurrentVisitor = c,
                        DatabaseApp.Visitor
                        .Where(c => c.VisitorID == SelectedVisitor.VisitorID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedVisitor != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(Visitor.ClassName, Const.New, (short)MISort.New, true, "SelectedVisitor", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Visitor), Visitor.NoColumnName, Visitor.FormatNewNo, this);
            CurrentVisitor = Visitor.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.Visitor.AddObject(CurrentVisitor);
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
        [ACMethodInteraction(Visitor.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentVisitor", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentVisitor.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            AccessPrimary.NavList.Remove(CurrentVisitor);
            SelectedVisitor = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return true;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Visitor.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            if (AccessFacilityVehicle != null)
                AccessFacilityVehicle.NavSearch(DatabaseApp);
            OnPropertyChanged("VisitorList");
            OnPropertyChanged("FacilityVehicleList");
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
            if (acAccess == _AccessVisitorVoucher)
            {
                _AccessVisitorVoucher.NavSearch(this.DatabaseApp);
                OnPropertyChanged("VisitorVoucherList");
                return true;
            }
            else if (acAccess == _AccessFacilityVehicle)
            {
                _AccessFacilityVehicle.NavSearch(this.DatabaseApp);
                OnPropertyChanged("FacilityVehicleList");
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }

        #endregion

        #region VisitorVoucher
        /// <summary>
        /// Loads the visitor voucher.
        /// </summary>
        [ACMethodInteraction(VisitorVoucher.ClassName, "en{'Load Position'}de{'Position laden'}", (short)MISort.Load, false, "SelectedVisitorVoucher", Global.ACKinds.MSMethodPrePost)]
        public void LoadVisitorVoucher()
        {
            if (!IsEnabledLoadVisitorVoucher())
                return;
            if (!PreExecute("LoadVisitorVoucher")) return;
            // Laden des aktuell selektierten VisitorVoucher 
            CurrentVisitorVoucher = CurrentVisitor.VisitorVoucher_Visitor.Where(c => c.VisitorVoucherID == SelectedVisitorVoucher.VisitorVoucherID).FirstOrDefault();
            PostExecute("LoadVisitorVoucher");
        }

        /// <summary>
        /// Determines whether [is enabled load visitor voucher].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load visitor voucher]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadVisitorVoucher()
        {
            return SelectedVisitorVoucher != null && CurrentVisitor != null;
        }
        #endregion

        #region VisitorCard
        /// <summary>
        /// Assigns the visitor card.
        /// </summary>
        [ACMethodCommand(Visitor.ClassName, "en{'Assign Card'}de{'Karte zuordnen'}", (short)600)]
        public void AssignVisitorCard()
        {
            if (!PreExecute("AssignVisitorCard")) return;

            ShowDialog(this, "AssignVisitorCard");

            PostExecute("AssignVisitorCard");
        }

        /// <summary>
        /// Determines whether [is enabled assign visitor card].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign visitor card]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignVisitorCard()
        {
            if (CurrentVisitor == null || CurrentVisitor.MDVisitorCard == null)
                return false;
            return true;
        }

        /// <summary>
        /// Assigns the visitor card OK.
        /// </summary>
        [ACMethodCommand("NewACClass", Const.Ok, (short)MISort.Okay)]
        public void AssignVisitorCardOK()
        {
            CloseTopDialog();
            CurrentVisitor.MDVisitorCard = CurrentAssignVisitorCard;
            CurrentAssignVisitorCard = null;
            OnPropertyChanged("CurrentProjectItemRoot");
        }

        /// <summary>
        /// Determines whether [is enabled assign visitor card OK].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign visitor card OK]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignVisitorCardOK()
        {
            if (CurrentAssignVisitorCard == null)
                return false;
            return true;
        }

        /// <summary>
        /// Assigns the visitor card cancel.
        /// </summary>
        [ACMethodCommand("NewACClass", Const.Cancel, (short)MISort.Cancel)]
        public void AssignVisitorCardCancel()
        {
            CloseTopDialog();
            CurrentAssignVisitorCard = null;
        }


        /// <summary>
        /// Unassigns the visitor card.
        /// </summary>
        [ACMethodCommand(Visitor.ClassName, "en{'Unassign Card'}de{'Karte freigeben'}", (short)601)]
        public void UnassignVisitorCard()
        {
            if (!PreExecute("UnassignVisitorCard")) return;

            CurrentVisitor.MDVisitorCard = null;
            ACSaveChanges();

            PostExecute("UnassignVisitorCard");
        }

        /// <summary>
        /// Determines whether [is enabled unassign visitor card].
        /// </summary>
        /// <returns><c>true</c> if [is enabled unassign visitor card]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUnassignVisitorCard()
        {
            if (CurrentVisitor == null || CurrentVisitor.MDVisitorCard == null)
                return false;
            if (DatabaseApp.IsChanged)
                return false;
            return true;
        }
        #endregion

        #region Dialog New DeliveryNote
        public VBDialogResult DialogResult { get; set; }

        [ACMethodInfo("Dialog", "en{'New Vistor'}de{'Neuer Besucher'}", (short)MISort.QueryPrintDlg)]
        public VBDialogResult ShowDialogNewVisitor()
        {
            New();
            ShowDialog(this, "VisitorDialog");
            this.ParentACComponent.StopComponent(this);
            return DialogResult;
        }

        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay)]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            DialogResult.ReturnValue = CurrentVisitor;
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel)]
        public void DialogCancel()
        {
            Delete();
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            CloseTopDialog();
        }
        #endregion

        #endregion



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
                case "LoadVisitorVoucher":
                    LoadVisitorVoucher();
                    return true;
                case "IsEnabledLoadVisitorVoucher":
                    result = IsEnabledLoadVisitorVoucher();
                    return true;
                case "AssignVisitorCard":
                    AssignVisitorCard();
                    return true;
                case "IsEnabledAssignVisitorCard":
                    result = IsEnabledAssignVisitorCard();
                    return true;
                case "AssignVisitorCardOK":
                    AssignVisitorCardOK();
                    return true;
                case "IsEnabledAssignVisitorCardOK":
                    result = IsEnabledAssignVisitorCardOK();
                    return true;
                case "AssignVisitorCardCancel":
                    AssignVisitorCardCancel();
                    return true;
                case "UnassignVisitorCard":
                    UnassignVisitorCard();
                    return true;
                case "IsEnabledUnassignVisitorCard":
                    result = IsEnabledUnassignVisitorCard();
                    return true;
                case "ShowDialogNewVisitor":
                    result = ShowDialogNewVisitor();
                    return true;
                case "DialogOK":
                    DialogOK();
                    return true;
                case "DialogCancel":
                    DialogCancel();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
