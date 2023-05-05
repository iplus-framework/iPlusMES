// ***********************************************************************
// Assembly         : gip.bso.purchasing
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOInRequest.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.purchasing
{
    /// <summary>
    /// Version 3
    /// Neue Masken:
    /// 1. Anfragenverwaltung
    /// TODO: Betroffene Tabellen: InRequest, InRequestPos
    /// Request for quotation (RfQ)
    /// </summary>
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Quotation'}de{'Anfrage'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + InRequest.ClassName)]
    public class BSOInRequest : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOInRequest"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOInRequest(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            this._AccessInRequestPos = null;
            this._CurrentInRequestPos = null;
            this._SelectedInRequestPos = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessInRequestPos != null)
            {
                _AccessInRequestPos.ACDeInit(false);
                _AccessInRequestPos = null;
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
        #region 1. InRequest
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<InRequest> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, "InRequest")]
        public ACAccessNav<InRequest> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<InRequest>("InRequest", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the current in request.
        /// </summary>
        /// <value>The current in request.</value>
        [ACPropertyCurrent(600, "InRequest")]
        public InRequest CurrentInRequest
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                CurrentInRequestPos = null;
                OnPropertyChanged("CurrentInRequest");
                OnPropertyChanged("InRequestPosList");
                OnPropertyChanged("DeliveryCompanyAddressList");
                OnPropertyChanged("DistributorCompanyList");
                OnPropertyChanged("ContractualCompanyList");
            }
        }

        /// <summary>
        /// Gets the in request list.
        /// </summary>
        /// <value>The in request list.</value>
        [ACPropertyList(601, "InRequest")]
        public IEnumerable<InRequest> InRequestList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected in request.
        /// </summary>
        /// <value>The selected in request.</value>
        [ACPropertySelected(602, "InRequest")]
        public InRequest SelectedInRequest
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedInRequest");
            }
        }
        #endregion

        #region 1.1 InRequestPos
        /// <summary>
        /// The _ access in request pos
        /// </summary>
        ACAccess<InRequestPos> _AccessInRequestPos;
        /// <summary>
        /// Gets the access in request pos.
        /// </summary>
        /// <value>The access in request pos.</value>
        [ACPropertyAccess(691, "InRequestPos")]
        public ACAccess<InRequestPos> AccessInRequestPos
        {
            get
            {
                if (_AccessInRequestPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + InRequestPos.ClassName) as ACQueryDefinition;
                    _AccessInRequestPos = acQueryDefinition.NewAccess<InRequestPos>("InRequestPos", this);
                }
                return _AccessInRequestPos;
            }
        }

        /// <summary>
        /// The _ current in request pos
        /// </summary>
        InRequestPos _CurrentInRequestPos;
        /// <summary>
        /// Gets or sets the current in request pos.
        /// </summary>
        /// <value>The current in request pos.</value>
        [ACPropertyCurrent(603, "InRequestPos")]
        public InRequestPos CurrentInRequestPos
        {
            get
            {
                return _CurrentInRequestPos;
            }
            set
            {
                if (_CurrentInRequestPos != null)
                    _CurrentInRequestPos.PropertyChanged -= CurrentInRequestPos_PropertyChanged;
                _CurrentInRequestPos = value;
                if (_CurrentInRequestPos != null)
                    _CurrentInRequestPos.PropertyChanged += CurrentInRequestPos_PropertyChanged;
                OnPropertyChanged("MDUnitList");
                OnPropertyChanged("CurrentInRequestPos");
                OnPropertyChanged("CurrentMDUnit");
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the CurrentInRequestPos control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void CurrentInRequestPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MaterialID":
                    {
                        OnPropertyChanged("MDUnitList");
                        if (CurrentInRequestPos.Material != null && CurrentInRequestPos.Material.BaseMDUnit != null)
                            CurrentMDUnit = CurrentInRequestPos.Material.BaseMDUnit;
                        else
                            CurrentMDUnit = null;
                        OnPropertyChanged("CurrentInRequestPos");
                    }
                    break;
                case "TargetQuantityUOM":
                case "MDUnitID":
                    {
                        CurrentInRequestPos.TargetQuantity = CurrentInRequestPos.Material.ConvertToBaseQuantity(CurrentInRequestPos.TargetQuantityUOM, CurrentInRequestPos.MDUnit);
                        CurrentInRequestPos.TargetWeight = CurrentInRequestPos.Material.ConvertToBaseWeight(CurrentInRequestPos.TargetQuantityUOM, CurrentInRequestPos.MDUnit);
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets the in request pos list.
        /// </summary>
        /// <value>The in request pos list.</value>
        [ACPropertyList(604, "InRequestPos")]
        public IEnumerable<InRequestPos> InRequestPosList
        {
            get
            {
                if (CurrentInRequest == null)
                    return null;
                return CurrentInRequest.InRequestPos_InRequest.AsEnumerable();
            }
        }

        /// <summary>
        /// The _ selected in request pos
        /// </summary>
        InRequestPos _SelectedInRequestPos;
        /// <summary>
        /// Gets or sets the selected in request pos.
        /// </summary>
        /// <value>The selected in request pos.</value>
        [ACPropertySelected(605, "InRequestPos")]
        public InRequestPos SelectedInRequestPos
        {
            get
            {
                return _SelectedInRequestPos;
            }
            set
            {
                _SelectedInRequestPos = value;
                OnPropertyChanged("SelectedInRequestPos");
            }
        }
        #endregion

        /// <summary>
        /// Gets the MU quantity unit list.
        /// </summary>
        /// <value>The MU quantity unit list.</value>
        [ACPropertyList(606, MDUnit.ClassName)]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                if (CurrentInRequestPos == null || CurrentInRequestPos.Material == null)
                    return null;
                return CurrentInRequestPos.Material.MDUnitList;
            }
        }

        /// <summary>
        /// Gets or sets the current MU quantity unit.
        /// </summary>
        /// <value>The current MU quantity unit.</value>
        [ACPropertyCurrent(607, MDUnit.ClassName)]
        public MDUnit CurrentMDUnit
        {
            get
            {
                if (CurrentInRequestPos == null)
                    return null;
                return CurrentInRequestPos.MDUnit;
            }
            set
            {
                if (CurrentInRequestPos != null && value != null)
                {
                    CurrentInRequestPos.MDUnit = value;
                    OnPropertyChanged("CurrentMDUnit");
                }
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
                return DatabaseApp.CompanyAddress.Where(c => c.Company.IsOwnCompany && c.IsDeliveryCompanyAddress).OrderBy(c => c.Name1);
            }
        }

        /// <summary>
        /// Gets the distributor company list.
        /// </summary>
        /// <value>The distributor company list.</value>
        [ACPropertyList(609, "DistributorCompany")]
        public IEnumerable<Company> DistributorCompanyList
        {
            get
            {
                return DatabaseApp.Company.Where(c => c.IsDistributor).OrderBy(c => c.CompanyName);
            }
        }

        /// <summary>
        /// Gets the contractual company list.
        /// </summary>
        /// <value>The contractual company list.</value>
        [ACPropertyList(610, "ContractualCompany")]
        public IEnumerable<Company> ContractualCompanyList
        {
            get
            {
                return DatabaseApp.Company.Where(c => c.IsCustomer).OrderBy(c => c.CompanyName).AsEnumerable();
            }
        }

        /// <summary>
        /// Gets the current delivery company address.
        /// </summary>
        /// <value>The current delivery company address.</value>
        [ACPropertyCurrent(611, "DeliveryCompanyAddress")]
        public CompanyAddress CurrentDeliveryCompanyAddress
        {
            get
            {
                if (CurrentInRequest == null)
                    return null;
                return CurrentInRequest.DeliveryCompanyAddress;
            }
        }


        /// <summary>
        /// Gets the current contractual partner address.
        /// </summary>
        /// <value>The current contractual partner address.</value>
        [ACPropertyCurrent(612, "ContractualPartnerAddress")]
        public CompanyAddress CurrentContractualPartnerAddress
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the current distrbutor company address.
        /// </summary>
        /// <value>The current distrbutor company address.</value>
        [ACPropertyCurrent(613, "DistrbutorCompanyAddress")]
        public CompanyAddress CurrentDistrbutorCompanyAddress
        {
            get
            {
                if (CurrentInRequest == null)
                    return null;
                try
                {
                    if (!CurrentInRequest.DistributorCompanyReference.IsLoaded)
                        CurrentInRequest.DistributorCompanyReference.Load();
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOInRequest", "CurrentDistributorCompanyAddress", msg);

                    if (CurrentInRequest.DistributorCompany == null)
                        return null;
                }
                try
                {
                    if (!CurrentInRequest.DistributorCompany.CompanyAddress_Company_IsLoaded)
                        CurrentInRequest.DistributorCompany.CompanyAddress_Company.AutoLoad(CurrentInRequest.DistributorCompany.CompanyAddress_CompanyReference, CurrentInRequest);
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOInRequest", "CurrentDistributorCompanyAddress(10)", msg);

                    return null;
                }

                try
                {
                    return CurrentInRequest.DistributorCompany.CompanyAddress_Company.Where(c => c.IsHouseCompanyAddress).FirstOrDefault();
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOInRequest", "CurrentDistributorCompanyAddress(20)", msg);
                    return null;
                }
            }
        }
        #endregion

        #region BSO->ACMethod
        #region 1. InRequest
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("InRequest", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand("InRequest", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction("InRequest", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedInRequest", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<InRequest>(requery, () => SelectedInRequest, () => CurrentInRequest, c => CurrentInRequest = c,
                        DatabaseApp.InRequest
                        .Include(c => c.InRequestPos_InRequest)
                        .Where(c => c.InRequestID == SelectedInRequest.InRequestID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedInRequest != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("InRequest", "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedInRequest", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(InRequest), InRequest.NoColumnName, InRequest.FormatNewNo, this);
            CurrentInRequest = InRequest.NewACObject(DatabaseApp, null, secondaryKey);

            // Vorbelegung mit der eigenen Anschrift
            try
            {
                CurrentInRequest.DeliveryCompanyAddress = DatabaseApp.CompanyAddress.Where(c => c.Company.IsOwnCompany && c.IsDeliveryCompanyAddress).OrderBy(c => c.Name1).FirstOrDefault();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOInRequest", "New", msg);
            }

            DatabaseApp.InRequest.Add(CurrentInRequest);
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
        [ACMethodInteraction("InRequest", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentInRequest", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentInRequest.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentInRequest);
            SelectedInRequest = AccessPrimary.NavList.FirstOrDefault();
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
        [ACMethodCommand("InRequest", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("InRequestList");
        }
        #endregion

        #region 1.1 InRequestPos
        /// <summary>
        /// Loads the in request pos.
        /// </summary>
        [ACMethodInteraction("InRequestPos", "en{'Load Position'}de{'Position laden'}", (short)MISort.Load, false, "SelectedInRequestPos", Global.ACKinds.MSMethodPrePost)]
        public void LoadInRequestPos()
        {
            if (!IsEnabledLoadInRequestPos())
                return;
            if (!PreExecute("LoadInRequestPos"))
                return;
            // Laden des aktuell selektierten InRequestPos 
            CurrentInRequestPos = CurrentInRequest.InRequestPos_InRequest.Where(c => c.InRequestPosID == SelectedInRequestPos.InRequestPosID).FirstOrDefault();
            PostExecute("LoadInRequestPos");
        }

        /// <summary>
        /// Determines whether [is enabled load in request pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load in request pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadInRequestPos()
        {
            return SelectedInRequestPos != null && CurrentInRequest != null;
        }

        /// <summary>
        /// News the in request pos.
        /// </summary>
        [ACMethodInteraction("InRequestPos", "en{'New Position'}de{'Neue Position'}", (short)MISort.New, true, "SelectedInRequestPos", Global.ACKinds.MSMethodPrePost)]
        public void NewInRequestPos()
        {
            if (!PreExecute("NewInRequestPos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            CurrentInRequestPos = InRequestPos.NewACObject(DatabaseApp, CurrentInRequest);
            CurrentInRequestPos.InRequest = CurrentInRequest;
            CurrentInRequest.InRequestPos_InRequest.Add(CurrentInRequestPos);

            PostExecute("NewInRequestPos");
        }

        /// <summary>
        /// Determines whether [is enabled new in request pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new in request pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewInRequestPos()
        {
            return true;
        }

        /// <summary>
        /// Deletes the in request pos.
        /// </summary>
        [ACMethodInteraction("InRequestPos", "en{'Delete Position'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentInRequestPos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteInRequestPos()
        {
            if (!PreExecute("DeleteInRequestPos")) return;
            Msg msg = CurrentInRequestPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            PostExecute("DeleteInRequestPos");
        }

        /// <summary>
        /// Determines whether [is enabled delete in request pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete in request pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteInRequestPos()
        {
            return CurrentInRequest != null && CurrentInRequestPos != null;
        }
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
                case "LoadInRequestPos":
                    LoadInRequestPos();
                    return true;
                case "IsEnabledLoadInRequestPos":
                    result = IsEnabledLoadInRequestPos();
                    return true;
                case "NewInRequestPos":
                    NewInRequestPos();
                    return true;
                case "IsEnabledNewInRequestPos":
                    result = IsEnabledNewInRequestPos();
                    return true;
                case "DeleteInRequestPos":
                    DeleteInRequestPos();
                    return true;
                case "IsEnabledDeleteInRequestPos":
                    result = IsEnabledDeleteInRequestPos();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
