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
using System.Threading.Tasks;

namespace gip.bso.purchasing
{
    /// <summary>
    /// Businessobject or App for managing requests for quotation (RfQ / purchase inquiries) in the system.
    /// This class extends the <see cref="BSOInRequestExplorer"/> and provides methods for creating, deleting and managing purchase inquiries with their positions.
    /// To search records enter the search string in the SearchWord property.
    /// The database result is copied to the InRequestList property.
    /// Then call NavigateFirst() method to set CurrentInRequest with the first record in the list.
    /// CurrentInRequest is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentInRequest and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentInRequest property.
    /// The delivery address of the own company is pre-assigned to the CurrentInRequest.DeliveryCompanyAddress property.
    /// Enter the inquiry number in the CurrentInRequest.InRequestNo property (generated automatically by the number manager), the date in CurrentInRequest.InRequestDate and the version in CurrentInRequest.InRequestVersion.
    /// The supplier (distributor) must be assigned to the CurrentInRequest.DistributorCompany property, the contractual partner to the CurrentInRequest.ContractualCompany property and the requested delivery type to the CurrentInRequest.MDDelivType property.
    /// The desired delivery dates can be set with the CurrentInRequest.TargetDeliveryDate and CurrentInRequest.TargetDeliveryMaxDate properties, the payment terms with CurrentInRequest.MDTermOfPayment and the time range with CurrentInRequest.MDTimeRange.
    /// Prices can be entered with the CurrentInRequest.PriceNet and CurrentInRequest.PriceGross properties and a free comment with the CurrentInRequest.Comment property.
    /// The positions of the inquiry are managed with the InRequestPosList property.
    /// Example how to add a position: First create a new InRequestPos with the NewInRequestPos() method which adds it to the InRequestPosList list and assigns it to the CurrentInRequestPos property.
    /// Then assign the requested material to the CurrentInRequestPos.Material property. When the material is assigned, the unit list MDUnitList is filled with the units of the material and the base unit is pre-assigned to the CurrentMDUnit property.
    /// Then enter the requested quantity in the CurrentInRequestPos.TargetQuantityUOM property. The base quantity (TargetQuantity) and the weight (TargetWeight) are calculated automatically from the unit.
    /// Optionally the position prices (CurrentInRequestPos.PriceNet, CurrentInRequestPos.PriceGross), the delivery dates (CurrentInRequestPos.TargetDeliveryDate, CurrentInRequestPos.TargetDeliveryMaxDate), the priority (CurrentInRequestPos.TargetDeliveryPriority) and a comment (CurrentInRequestPos.Comment) can be set.
    /// Now the Save() method must be called to save the changes in the database.
    /// The DeleteInRequestPos() method deletes the position in the CurrentInRequestPos property.
    /// The Delete() method deletes the current inquiry (CurrentInRequest) from the database.
    /// The Load() method re-reads the current inquiry if another user has changed it in the background.
    /// Visit https://github.com/search?q=org%3Aiplus-framework%20BSOInRequest&type=code for more information or ask the iplus-framework Copilot.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Quotation'}de{'Anfrage'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + InRequest.ClassName,
                 Description = @"Businessobject or App for managing requests for quotation (RfQ / purchase inquiries) in the system.
                                This class extends the <see cref=""BSOInRequestExplorer""/> and provides methods for creating, deleting and managing purchase inquiries with their positions.
                                To search records enter the search string in the SearchWord property.
                                The database result is copied to the InRequestList property.
                                Then call NavigateFirst() method to set CurrentInRequest with the first record in the list.
                                CurrentInRequest is used to display and edit the currently selected record.
                                Property changes should always be made to CurrentInRequest and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
                                The New() method creates a new record and assigns the new entity object to the CurrentInRequest property.
                                The delivery address of the own company is pre-assigned to the CurrentInRequest.DeliveryCompanyAddress property.
                                Enter the inquiry number in the CurrentInRequest.InRequestNo property (generated automatically by the number manager), the date in CurrentInRequest.InRequestDate and the version in CurrentInRequest.InRequestVersion.
                                The supplier (distributor) must be assigned to the CurrentInRequest.DistributorCompany property, the contractual partner to the CurrentInRequest.ContractualCompany property and the requested delivery type to the CurrentInRequest.MDDelivType property.
                                The desired delivery dates can be set with the CurrentInRequest.TargetDeliveryDate and CurrentInRequest.TargetDeliveryMaxDate properties, the payment terms with CurrentInRequest.MDTermOfPayment and the time range with CurrentInRequest.MDTimeRange.
                                Prices can be entered with the CurrentInRequest.PriceNet and CurrentInRequest.PriceGross properties and a free comment with the CurrentInRequest.Comment property.
                                The positions of the inquiry are managed with the InRequestPosList property.
                                Example how to add a position: First create a new InRequestPos with the NewInRequestPos() method which adds it to the InRequestPosList list and assigns it to the CurrentInRequestPos property.
                                Then assign the requested material to the CurrentInRequestPos.Material property. When the material is assigned, the unit list MDUnitList is filled with the units of the material and the base unit is pre-assigned to the CurrentMDUnit property.
                                Then enter the requested quantity in the CurrentInRequestPos.TargetQuantityUOM property. The base quantity (TargetQuantity) and the weight (TargetWeight) are calculated automatically from the unit.
                                Optionally the position prices (CurrentInRequestPos.PriceNet, CurrentInRequestPos.PriceGross), the delivery dates (CurrentInRequestPos.TargetDeliveryDate, CurrentInRequestPos.TargetDeliveryMaxDate), the priority (CurrentInRequestPos.TargetDeliveryPriority) and a comment (CurrentInRequestPos.Comment) can be set.
                                Now the Save() method must be called to save the changes in the database.
                                The DeleteInRequestPos() method deletes the position in the CurrentInRequestPos property.
                                The Delete() method deletes the current inquiry (CurrentInRequest) from the database.
                                The Load() method re-reads the current inquiry if another user has changed it in the background.
                                Visit https://github.com/search?q=org%3Aiplus-framework%20BSOInRequest&type=code for more information or ask the iplus-framework Copilot.")]
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

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._AccessInRequestPos = null;
            this._CurrentInRequestPos = null;
            this._SelectedInRequestPos = null;
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessInRequestPos != null)
            {
                await _AccessInRequestPos.ACDeInit(false);
                _AccessInRequestPos = null;
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
        [ACPropertyAccessPrimary(690, "InRequest", Description = "Primary navigation access to the InRequest entities. Used by the search list and the navigation methods.")]
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
        [ACPropertyCurrent(600, "InRequest", Description = "Gets or sets the currently edited purchase inquiry (InRequest). All header changes must be made on this entity and saved with Save().")]
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
        [ACPropertyList(601, "InRequest", Description = "Gets the result list of purchase inquiries after Search() was executed.")]
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
        [ACPropertySelected(602, "InRequest", Description = "Gets or sets the inquiry selected in the search list. Load() loads this inquiry into CurrentInRequest.")]
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
        [ACPropertyAccess(691, "InRequestPos", Description = "Access to the InRequestPos child entities of the current inquiry.")]
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
        [ACPropertyCurrent(603, "InRequestPos", Description = "Gets or sets the currently edited inquiry position. Changes must be made on this entity and saved with Save().")]
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
        [ACPropertyList(604, "InRequestPos", Description = "Gets the list of positions of the current inquiry (CurrentInRequest).")]
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
        [ACPropertySelected(605, "InRequestPos", Description = "Gets or sets the inquiry position selected in the position list. LoadInRequestPos() loads this position into CurrentInRequestPos.")]
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
        [ACPropertyList(606, MDUnit.ClassName, Description = "Gets the list of quantity units available for the material of the current inquiry position.")]
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
        [ACPropertyCurrent(607, MDUnit.ClassName, Description = "Gets or sets the quantity unit of the current inquiry position. Changing the unit recalculates the base quantity and weight of the position.")]
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
        [ACPropertyList(608, "DeliveryCompanyAddress", Description = "Gets the delivery addresses of the own company, offered as delivery address for the inquiry.")]
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
        [ACPropertyList(609, "DistributorCompany", Description = "Gets the companies flagged as distributors (suppliers), offered as supplier for the inquiry.")]
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
        [ACPropertyList(610, "ContractualCompany", Description = "Gets the companies flagged as customers, offered as contractual partner for the inquiry.")]
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
        [ACPropertyCurrent(611, "DeliveryCompanyAddress", Description = "Gets the delivery address of the current inquiry.")]
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
        [ACPropertyCurrent(612, "ContractualPartnerAddress", Description = "Gets the address of the contractual partner of the current inquiry. Currently always returns null (not implemented).")]
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
        [ACPropertyCurrent(613, "DistrbutorCompanyAddress", Description = "Gets the house address of the distributor (supplier) of the current inquiry.")]
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
        [ACMethodCommand("InRequest", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves all changes of the current inquiry and its positions to the database.")]
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
        [ACMethodCommand("InRequest", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards all unsaved changes of the current inquiry and its positions.")]
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
        [ACMethodInteraction("InRequest", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedInRequest", Global.ACKinds.MSMethodPrePost, Description = "Re-reads the selected inquiry (including its positions) from the database into CurrentInRequest, e.g. if another user has changed it in the background.")]
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
        [ACMethodInteraction("InRequest", Const.New, (short)MISort.New, true, "SelectedInRequest", Global.ACKinds.MSMethodPrePost, Description = "Creates a new purchase inquiry with an automatically generated inquiry number, pre-assigns the delivery address of the own company and assigns it to CurrentInRequest.")]
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
        [ACMethodInteraction("InRequest", Const.Delete, (short)MISort.Delete, true, "CurrentInRequest", Global.ACKinds.MSMethodPrePost, Description = "Deletes the current inquiry (CurrentInRequest) from the database and selects the first remaining inquiry.")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentInRequest.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
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
        [ACMethodCommand("InRequest", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Executes the search with the current search criteria and refreshes the InRequestList property.")]
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
        [ACMethodInteraction("InRequestPos", "en{'Load Position'}de{'Position laden'}", (short)MISort.Load, false, "SelectedInRequestPos", Global.ACKinds.MSMethodPrePost, Description = "Loads the position selected in the position list into CurrentInRequestPos for editing.")]
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
        [ACMethodInteraction("InRequestPos", "en{'New Position'}de{'Neue Position'}", (short)MISort.New, true, "SelectedInRequestPos", Global.ACKinds.MSMethodPrePost, Description = "Creates a new position for the current inquiry, adds it to the InRequestPosList and assigns it to CurrentInRequestPos.")]
        public void NewInRequestPos()
        {
            if (!PreExecute("NewInRequestPos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            InRequestPos newInRequestPos = InRequestPos.NewACObject(DatabaseApp, CurrentInRequest);
            newInRequestPos.InRequest = CurrentInRequest;
            // Add to collection BEFORE assigning CurrentInRequestPos, otherwise the DataGrid
            // coerces SelectedItem back to null (item not yet in ItemsSource) and writes
            // null back into CurrentInRequestPos via OnSelectionChanged.
            CurrentInRequest.InRequestPos_InRequest.Add(newInRequestPos);
            OnPropertyChanged("InRequestPosList");
            CurrentInRequestPos = newInRequestPos;

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
        [ACMethodInteraction("InRequestPos", "en{'Delete Position'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentInRequestPos", Global.ACKinds.MSMethodPrePost, Description = "Deletes the current inquiry position (CurrentInRequestPos) from the database and removes it from the position list.")]
        public void DeleteInRequestPos()
        {
            if (!PreExecute("DeleteInRequestPos")) return;
            InRequestPos posToDelete = CurrentInRequestPos;
            Msg msg = posToDelete.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            // EF Core does not remove the entity from navigation collections automatically
            // (unlike EF4). Remove it manually, otherwise the DataGrid still shows the
            // deleted row after OnPropertyChanged("InRequestPosList").
            CurrentInRequest?.InRequestPos_InRequest.Remove(posToDelete);
            // Clear Current/Selected BEFORE raising the list notification, so the DataGrid
            // doesn't try to restore a selection on the deleted entity.
            CurrentInRequestPos = null;
            SelectedInRequestPos = null;

            PostExecute("DeleteInRequestPos");
            OnPropertyChanged("InRequestPosList");
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
                case nameof(LoadInRequestPos):
                    LoadInRequestPos();
                    return true;
                case nameof(IsEnabledLoadInRequestPos):
                    result = IsEnabledLoadInRequestPos();
                    return true;
                case nameof(NewInRequestPos):
                    NewInRequestPos();
                    return true;
                case nameof(IsEnabledNewInRequestPos):
                    result = IsEnabledNewInRequestPos();
                    return true;
                case nameof(DeleteInRequestPos):
                    DeleteInRequestPos();
                    return true;
                case nameof(IsEnabledDeleteInRequestPos):
                    result = IsEnabledDeleteInRequestPos();
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
                    return new string[] { nameof(ACState), nameof(SelectedInRequest) };
                #endregion

                #region Delete
                case nameof(Delete):
                case nameof(IsEnabledDelete):
                    return new string[] { nameof(CurrentInRequest) };
                #endregion

                #region InRequestPos
                case nameof(LoadInRequestPos):
                case nameof(IsEnabledLoadInRequestPos):
                    return new string[] { nameof(CurrentInRequest) };
                case nameof(NewInRequestPos):
                case nameof(IsEnabledNewInRequestPos):
                    return new string[] { nameof(CurrentInRequest) };
                case nameof(DeleteInRequestPos):
                case nameof(IsEnabledDeleteInRequestPos):
                    return new string[] { nameof(CurrentInRequest), nameof(CurrentInRequestPos) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}
