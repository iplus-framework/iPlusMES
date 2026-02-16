// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static gip.core.datamodel.Global;

namespace gip.bso.manufacturing
{
    public partial class BSOProdOrder
    {

        #region InwardFacilityPreBooking

        #region InwardFacilityPreBooking -> Select, (Current,) List

        FacilityPreBooking _SelectedInwardFacilityPreBooking;
        [ACPropertySelected(637, "InwardFacilityPreBooking")]
        public FacilityPreBooking SelectedInwardFacilityPreBooking
        {
            get
            {
                return _SelectedInwardFacilityPreBooking;
            }
            set
            {
                if (_SelectedInwardFacilityPreBooking != value)
                {
                    _SelectedInwardFacilityPreBooking = value;

                    RefreshFilterInFacilityAccess();

                    if (
                            _SelectedInwardFacilityPreBooking != null
                            && _SelectedInwardFacilityPreBooking.InwardFacility != null
                            && !BookingInwardFacilityList.Contains(_SelectedInwardFacilityPreBooking.InwardFacility)
                       )
                    {
                        AccessInBookingFacility.NavList.Add(_SelectedInwardFacilityPreBooking.InwardFacility);
                    }

                    OnPropertyChanged(nameof(SelectedInwardFacilityPreBooking));
                    OnPropertyChanged(nameof(SelectedInwardFacilityBooking));
                    OnPropertyChanged(nameof(BookingInwardFacilityList));
                    OnPropertyChanged(nameof(SelectedInwardACMethodBooking));
                }
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(638, "InwardFacilityPreBooking")]
        public IEnumerable<FacilityPreBooking> InwardFacilityPreBookingList
        {
            get
            {
                if (SelectedIntermediate == null)
                    return null;
                if (SelectedProdOrderIntermediateBatch != null)
                    return SelectedProdOrderIntermediateBatch.FacilityPreBooking_ProdOrderPartslistPos.ToList();
                return SelectedIntermediate.FacilityPreBooking_ProdOrderPartslistPos.ToList();
            }
        }

        #endregion

        #region InwardFacilityPreBooking -> Methods

        #region InwardFacilityPreBooking -> Methods -> Booking, Recalc

        // GUI #4 BookSelectedInwardACMethodBooking
        [ACMethodInteraction("InwardFacilityPreBooking", "en{'Post Item'}de{'Position Buchen'}", (short)MISort.New, true, "SelectedInwardFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public virtual async Task BookSelectedInwardACMethodBooking(bool refreshList = true)
        {
            if (!IsEnabledBookSelectedInwardACMethodBooking())
                return;
            if (SelectedProdOrderIntermediateBatch != null)
            {
                SelectedInwardACMethodBooking.PartslistPos = SelectedProdOrderIntermediateBatch;
                SelectedInwardACMethodBooking.InwardFacilityLot = SelectedProdOrderIntermediateBatch.FacilityLot;
                if (SelectedInwardACMethodBooking.ProdOrderPartslistPosFacilityLot != null)
                {
                    SelectedInwardACMethodBooking.InwardFacilityLot = SelectedInwardACMethodBooking.ProdOrderPartslistPosFacilityLot.FacilityLot;
                }
            }
            else
            {
                SelectedInwardACMethodBooking.PartslistPos = SelectedIntermediate;
                SelectedInwardACMethodBooking.InwardFacilityLot = SelectedIntermediate.FacilityLot;
            }

            if (SelectedInwardACMethodBooking.FacilityBooking != null)
            {
                if (SelectedInwardACMethodBooking.FacilityBooking.ProdOrderPartslistPos != SelectedInwardACMethodBooking.PartslistPos)
                {
                    SelectedInwardACMethodBooking.FacilityBooking.ProdOrderPartslistPos = SelectedInwardACMethodBooking.PartslistPos;
                }

                if (SelectedInwardACMethodBooking.FacilityBooking.InwardMaterial != SelectedInwardACMethodBooking.InwardMaterial)
                    SelectedInwardACMethodBooking.FacilityBooking.InwardMaterial = SelectedInwardACMethodBooking.InwardMaterial;

                if (SelectedInwardACMethodBooking.FacilityBooking.MDUnit != SelectedInwardACMethodBooking.MDUnit)
                    SelectedInwardACMethodBooking.FacilityBooking.MDUnit = SelectedInwardACMethodBooking.MDUnit;

                if (SelectedInwardACMethodBooking.FacilityBooking.InwardFacility != SelectedInwardACMethodBooking.InwardFacility)
                    SelectedInwardACMethodBooking.FacilityBooking.InwardFacility = SelectedInwardACMethodBooking.InwardFacility;

                if (SelectedInwardACMethodBooking.FacilityBooking.MDMovementReason != SelectedInwardACMethodBooking.MDMovementReason)
                    SelectedInwardACMethodBooking.FacilityBooking.MDMovementReason = SelectedInwardACMethodBooking.MDMovementReason;
            }

            //if (CurrentProdOrder.CPartnerCompany != null && SelectedInwardACMethodBooking.CPartnerCompany != CurrentProdOrder.CPartnerCompany)
            SelectedInwardACMethodBooking.CPartnerCompany = CurrentProdOrder.CPartnerCompany;

            bool isCancellation = SelectedInwardACMethodBooking.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosInwardCancel || SelectedInwardACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel;

            await Save();
            if (DatabaseApp.IsChanged)
                return;
            if (!PreExecute(nameof(BookSelectedInwardACMethodBooking)))
                return;


            SelectedInwardACMethodBooking.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(SelectedInwardACMethodBooking, this.DatabaseApp) as ACMethodEventArgs;
            if (!SelectedInwardACMethodBooking.ValidMessage.IsSucceded() || SelectedInwardACMethodBooking.ValidMessage.HasWarnings())
                await Messages.MsgAsync(SelectedInwardACMethodBooking.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(SelectedInwardACMethodBooking.ValidMessage.Message))
                    SelectedInwardACMethodBooking.ValidMessage.Message = result.ResultState.ToString();
                await Messages.MsgAsync(SelectedInwardACMethodBooking.ValidMessage);
                OnPropertyChanged(nameof(InwardFacilityBookingList));
            }
            else
            {
                double quantity = 0;
                if (SelectedInwardACMethodBooking.InwardQuantity.HasValue)
                    quantity = SelectedInwardACMethodBooking.InwardQuantity.Value;
                MDUnit unit = SelectedInwardACMethodBooking.MDUnit;
                ProdOrderPartslistPos bookingPos = SelectedInwardACMethodBooking.PartslistPos;

                DeleteInwardFacilityPreBooking();
                OnPropertyChanged(nameof(InwardFacilityBookingList));
                if (unit != null)
                    bookingPos.IncreaseActualQuantity(quantity, unit, true);
                else
                    bookingPos.IncreaseActualQuantityUOM(quantity, true);
                //SelectedIntermediate.TopParentPartslistPos.RecalcActualQuantity();
                if (SelectedIntermediate.IsFinalMixure)
                {
                    SelectedIntermediate.ProdOrderPartslist.RecalcActualQuantitySP(DatabaseApp);
                    OnPropertyChanged(nameof(SelectedProdOrder));
                }
                if (isCancellation)
                {
                    MDProdOrderPartslistPosState state = DatabaseApp.s_cQry_GetMDProdOrderPosState(DatabaseApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled).FirstOrDefault();
                    if (state != null)
                        SelectedIntermediate.MDProdOrderPartslistPosState = state;
                }

                await Save();

                bookingPos.RecalcActualQuantityFast();
                if (DatabaseApp.IsChanged)
                {
                    if (SelectedIntermediate.IsFinalMixure)
                    {
                        SelectedIntermediate.ProdOrderPartslist.RecalcActualQuantitySP(DatabaseApp);
                        OnPropertyChanged(nameof(SelectedProdOrder));
                    }
                    await Save();
                }
            }

            PostExecute(nameof(BookSelectedInwardACMethodBooking));
        }

        public virtual bool IsEnabledBookSelectedInwardACMethodBooking()
        {
            return
                SelectedInwardACMethodBooking != null
                && SelectedInwardFacilityPreBooking != null
                && SelectedInwardACMethodBooking.InwardFacility != null
                && (
                        SelectedProdOrderIntermediateBatch != null
                        || (SelectedIntermediate != null && AllowPostingOnIntermediate)
                );
        }

        // GUI #5 BookAllInwardACMBookings
        [ACMethodCommand("InwardFacilityPreBooking", "en{'Post All'}de{'Buche alle'}", (short)MISort.Cancel)]
        public async Task BookAllInwardACMBookings()
        {
            if (!IsEnabledBookAllInwardACMBookings())
                return;
            foreach (FacilityPreBooking facilityPreBooking in InwardFacilityPreBookingList.ToList())
            {
                SelectedInwardFacilityPreBooking = facilityPreBooking;
                if (SelectedInwardFacilityPreBooking == facilityPreBooking)
                    await BookSelectedInwardACMethodBooking(false);
            }
        }

        public bool IsEnabledBookAllInwardACMBookings()
        {
            if (SelectedIntermediate == null || InwardFacilityPreBookingList == null || !InwardFacilityPreBookingList.Any())
                return false;
            //if (this.OutwardPartslistPosList == null || this.OutwardPartslistPosList.Where(c => c.ActualQuantityUOM < 0.00001).Any())
            //    return false;
            return true;
        }

        protected void RecalcInBookingValues()
        {
            // TODO: (aagincic): RecalcInBookingValues - analise function
            if (SelectedInwardFacilityPreBooking == null || SelectedInwardACMethodBooking == null)
                return;
            if (this.InwardFacilityBookingList.Any())
                return;
            try
            {
                double nSumVol = OutwardPartslistPosList.Sum(c => c.TargetQuantity);
                // COMMENT: (aagincic) - this is temperature calculation (ELG) not applicable there
                //double nSumTemp = 0;
                //foreach (ProdOrderPartslistPosRelation outPos in OutwardPartslistPosList)
                //{
                //    outPos.SourceProdOrderPartslistPos.FacilityBooking_ProdOrderPartslistPos.AutoRefresh();
                //    FacilityBooking fb = outPos.SourceProdOrderPartslistPos.FacilityBooking_ProdOrderPartslistPos.FirstOrDefault();
                //    if (fb != null)
                //    {
                //        double temp = (double)fb["Temperature"];
                //        double tempGew = (outPos.Quantity / nSumVol) * temp;
                //        nSumTemp += tempGew;
                //    }
                //}
                if (SelectedInwardACMethodBooking != null)
                {
                    //SelectedInwardACMethodBooking.Temperature = nSumTemp;
                    SelectedInwardACMethodBooking.InwardQuantity = nSumVol;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOProdOrder", "RecalcInBookingValues", msg);
                // TODO: (aagincic) RecalcInBookingValues - process exception
            }
        }

        #endregion

        #region InwardFacilityPreBooking -> Methods -> Manipupulate (New, Edit, Delete...)

        // GUI #1 NewInwardFacilityPreBooking
        [ACMethodInteraction("InwardFacilityPreBooking", "en{'New Posting'}de{'Neue Buchung'}", (short)MISort.New, true, "SelectedInwardFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void NewInwardFacilityPreBooking()
        {
            if (!IsEnabledNewInwardFacilityPreBooking())
                return;
            if (!PreExecute("NewInwardFacilityPreBooking"))
                return;
            ProdOrderPartslistPos posItemForBooking = SelectedIntermediate;
            if (SelectedProdOrderIntermediateBatch != null)
                posItemForBooking = SelectedProdOrderIntermediateBatch;
            SelectedInwardFacilityPreBooking = ProdOrderManager.NewInwardFacilityPreBooking(ACFacilityManager, DatabaseApp, posItemForBooking);
            OnPropertyChanged("InwardFacilityPreBookingList");
            PostExecute("NewInwardFacilityPreBooking");
        }

        public bool IsEnabledNewInwardFacilityPreBooking()
        {
            return SelectedProdOrderIntermediateBatch != null || (AllowPostingOnIntermediate && SelectedIntermediate != null);
        }

        // GUI #2 DeleteInwardFacilityPreBooking
        [ACMethodInteraction("InwardFacilityPreBooking", "en{'Delete Posting'}de{'Lösche Buchung'}", (short)MISort.Delete, true, "SelectedPartslistPos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteInwardFacilityPreBooking()
        {
            if (!IsEnabledDeleteInwardFacilityPreBooking()) return;
            Msg msg = SelectedInwardFacilityPreBooking.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            else
            {
                SelectedInwardFacilityPreBooking = null;
                OnPropertyChanged("InwardFacilityPreBookingList");
            }
            PostExecute("DeleteInwardFacilityPreBooking");

        }

        public bool IsEnabledDeleteInwardFacilityPreBooking()
        {
            return SelectedInwardFacilityPreBooking != null;
        }

        // GUI #3 CancelInwardFacilityPreBooking
        [ACMethodInteraction("InwardFacilityPreBooking", "en{'Cancel Posting'}de{'Buchung abbrechen'}", (short)MISort.Cancel, true, "SelectedInwardFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void CancelInwardFacilityPreBooking()
        {
            if (!IsEnabledCancelInwardFacilityPreBooking())
                return;
            if (!PreExecute("CancelInwardFacilityPreBooking"))
                return;
            var result = ProdOrderManager.CancelInFacilityPreBooking(ACFacilityManager, DatabaseApp, SelectedIntermediate);
            if (result != null && result.Any())
            {
                SelectedInwardFacilityPreBooking = result.FirstOrDefault();
                OnPropertyChanged("InwardFacilityPreBookingList");
                OnPropertyChanged("InwardFacilityBookingList");
            }
            PostExecute("CancelInwardFacilityPreBooking");
        }

        public bool IsEnabledCancelInwardFacilityPreBooking()
        {
            if (SelectedIntermediate == null
                || InwardFacilityPreBookingList.Any())
                return false;
            return true;
        }

        #endregion

        #region InwardFacilityPreBooking -> Methods -> Dlg Facility, FaciltiyCharge

        /// <summary>
        /// Source Property: ShowDlgInwardFacility
        /// </summary>
        [ACMethodInfo("ShowDlgInwardFacility", "en{'Choose facility'}de{'Lager auswählen'}", 999)]
        public async Task ShowDlgInwardFacility()
        {
            if (!IsEnabledShowDlgInwardFacility())
                return;
            FacilitySelectLoctation = FacilitySelectLoctation.PrebookingInward;
            await ShowDlgFacility(SelectedInwardACMethodBooking.InwardFacility);
        }

        public bool IsEnabledShowDlgInwardFacility()
        {
            return SelectedInwardACMethodBooking != null;
        }

        [ACMethodInteraction("", "en{'Correct posting'}de{'Korrigiere Buchung'}", 670, true, nameof(SelectedInwardFacilityBookingCharge))]
        public async void CorrectInwardBooking()
        {
            if (SelectedInwardFacilityBookingCharge == null)
                return;

            FacilityCharge inwardFacilityCharge = SelectedInwardFacilityBookingCharge.InwardFacilityCharge;

            if (inwardFacilityCharge == null)
                return;

            NewInwardFacilityPreBooking();

            if (SelectedInwardACMethodBooking == null)
            {
                //todo error
                return;
            }

            SelectedInwardACMethodBooking.InwardFacility = inwardFacilityCharge.Facility;
            SelectedInwardACMethodBooking.InwardFacilityCharge = inwardFacilityCharge;
            SelectedInwardACMethodBooking.InwardQuantity = inwardFacilityCharge.StockQuantity < 0.00001 ? inwardFacilityCharge.StockQuantity : 0;

            await ShowDialogAsync(this, "CorrectInwardBookingDialog");

            if (ACFacilityManager != null)
            {
                Msg msg = ACFacilityManager.IsQuantStockConsumed(inwardFacilityCharge, DatabaseApp);
                if (msg != null)
                {
                    if (await Messages.QuestionAsync(this, msg.Message, MsgResult.No, true) == MsgResult.Yes)
                    {
                        ACMethodBooking fbtZeroBookingClone = ACFacilityManager.GetBookParamNotAvailableClone();

                        fbtZeroBookingClone.InwardFacilityCharge = inwardFacilityCharge;
                        fbtZeroBookingClone.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);

                        ACMethodEventArgs resultZeroBook = ACFacilityManager.BookFacility(fbtZeroBookingClone, DatabaseApp);
                        if (!fbtZeroBookingClone.ValidMessage.IsSucceded() || fbtZeroBookingClone.ValidMessage.HasWarnings())
                        {
                            //return fbtZeroBooking.ValidMessage;
                        }
                        else if (resultZeroBook.ResultState == Global.ACMethodResultState.Failed || resultZeroBook.ResultState == Global.ACMethodResultState.Notpossible)
                        {
                            if (String.IsNullOrEmpty(resultZeroBook.ValidMessage.Message))
                                resultZeroBook.ValidMessage.Message = resultZeroBook.ResultState.ToString();

                            //return result.ValidMessage;
                        }

                        Msg msgSave = DatabaseApp.ACSaveChanges();
                        if (msgSave != null)
                            await Messages.MsgAsync(msgSave);
                    }
                }
            }

            if (SelectedInwardFacilityPreBooking != null)
            {
                DeleteInwardFacilityPreBooking();
            }
        }

        public bool IsEnabledCorrectInwardBooking()
        {
            return SelectedInwardFacilityBookingCharge != null
                    && SelectedInwardFacilityBookingCharge.InwardFacilityCharge != null
                    && SelectedInwardFacilityBookingCharge.InwardFacilityCharge.Facility != null
                    && SelectedInwardFacilityBookingCharge.InwardFacilityCharge.Facility.MDFacilityType != null
                    && SelectedInwardFacilityBookingCharge.InwardFacilityCharge.Facility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBinContainer;
        }

        #endregion

        #endregion

        #region InwardFacilityPreBooking -> ACMethodBooking
        // TODO: (aagincic) For refactory InwardFacilityPreBooking -> ACMethodBooking
        ACMethodBooking _SelectedInwardACMethodBookingDummy = null; // Dummy-Parameter notwendig, damit Bindung an Oberfläche erfolgen kann, da abgeleitete Klasse
        [ACPropertyInfo(639, "", "en{'Posting parameter'}de{'Buchungsparameter'}")]
        public ACMethodBooking SelectedInwardACMethodBooking
        {
            get
            {
                // TODO: @aagincic InwardACMethodBooking(1) - to complicated logic for checking is SelectedInwardACMethodBooking created and selected prebooking not null
                if (SelectedInwardFacilityPreBooking == null && this.ProdOrderManager != null)
                {
                    if (_SelectedInwardACMethodBookingDummy != null) return _SelectedInwardACMethodBookingDummy;
                    ACMethodBooking acMethodClone = this.ProdOrderManager.BookParamInwardMovementClone(this.ACFacilityManager, this.DatabaseApp);
                    if (acMethodClone != null)
                        _SelectedInwardACMethodBookingDummy = acMethodClone.Clone() as ACMethodBooking;
                    return _SelectedInwardACMethodBookingDummy;
                }
                _SelectedInwardACMethodBookingDummy = null;
                if (_SelectedInwardFacilityPreBooking == null)
                    return null;
                if (_SelectedInwardFacilityPreBooking.ACMethodBooking != null)
                {
                    _SelectedInwardFacilityPreBooking.ACMethodBooking.PropertyChanged -= ACMethodBookingInward_PropertyChanged;
                    _SelectedInwardFacilityPreBooking.ACMethodBooking.PropertyChanged += ACMethodBookingInward_PropertyChanged;
                }
                return SelectedInwardFacilityPreBooking.ACMethodBooking as ACMethodBooking;
            }
            set
            {
                if (SelectedInwardFacilityPreBooking != null)
                    SelectedInwardFacilityPreBooking.ACMethodBooking = value;
                else
                    _SelectedInwardACMethodBookingDummy = null;
                OnPropertyChanged("SelectedInwardACMethodBooking");
            }
        }

        bool _UpdatingControlModeInward = false;
        void ACMethodBookingInward_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_UpdatingControlModeInward)
                return;
            try
            {
                if (e.PropertyName == "InwardFacility" && SelectedInwardACMethodBooking != null)
                {
                    _UpdatingControlModeInward = true;
                    SelectedInwardACMethodBooking.OnEntityPropertyChanged("InwardFacility");
                }

                if (e.PropertyName == "InwardQuantity" && SelectedInwardACMethodBooking != null)
                {
                    _UpdatingControlModeInward = true;
                    SelectedInwardACMethodBooking.OnEntityPropertyChanged("InwardQuantity");
                }
            }
            finally
            {
                _UpdatingControlModeInward = false;
            }
        }


        #endregion

        #region InwardFacilityPreBooking -> Additional members
        // TODO: (aagincic) InwardFacilityPreBooking -> Additional members : for refactor

        private bool _BookingInwardFilterMaterial = true;
        [ACPropertyInfo(639, "", "en{'Only show bins with material'}de{'Zeige Lagerpätze mit Material'}")]
        public bool BookingInwardFilterMaterial
        {
            get
            {
                return _BookingInwardFilterMaterial;
            }
            set
            {
                if (_BookingInwardFilterMaterial != value)
                {
                    _BookingInwardFilterMaterial = value;
                    OnPropertyChanged("BookingInwardFilterMaterial");
                    RefreshFilterInFacilityAccess();
                    OnPropertyChanged("BookingInwardFacilityList");
                }
            }
        }

        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<Facility> _AccessInBookingFacility;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(640, "BookingInwardFacility")]
        public ACAccessNav<Facility> AccessInBookingFacility
        {
            get
            {
                if (_AccessInBookingFacility == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "BookingFacility", ACType.ACIdentifier);
                    _AccessInBookingFacility = navACQueryDefinition.NewAccessNav<Facility>("BookingInwardFacility", this);
                    _AccessInBookingFacility.AutoSaveOnNavigation = false;
                    RefreshFilterInFacilityAccess();
                }
                return _AccessInBookingFacility;
            }
        }

        [ACPropertyList(641, "BookingInwardFacility")]
        public IList<Facility> BookingInwardFacilityList
        {
            get
            {
                if (AccessInBookingFacility == null)
                    return null;
                return AccessInBookingFacility.NavList;
            }
        }

        private void RefreshFilterInFacilityAccess()
        {
            if (AccessInBookingFacility == null || SelectedInwardFacilityPreBooking == null || InwardFacilityPreBookingList == null || !InwardFacilityPreBookingList.Any())
                return;
            bool rebuildACQueryDef = false;
            short fcTypeContainer = (short)FacilityTypesEnum.StorageBinContainer;
            short fcTypeBin = (short)FacilityTypesEnum.StorageBin;
            if (AccessInBookingFacility.NavACQueryDefinition.ACFilterColumns.Count <= 0)
            {
                rebuildACQueryDef = true;
            }
            else
            {
                int countFoundCorrect = 0;
                foreach (ACFilterItem filterItem in AccessInBookingFacility.NavACQueryDefinition.ACFilterColumns)
                {
                    if (filterItem.FilterType != Global.FilterTypes.filter)
                        continue;
                    if (filterItem.PropertyName == "MDFacilityType\\MDFacilityTypeIndex")
                    {
                        if ((BookingInwardFilterMaterial && filterItem.SearchWord == fcTypeContainer.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal)
                            || (!BookingInwardFilterMaterial && filterItem.SearchWord == fcTypeBin.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal))
                            countFoundCorrect++;
                    }
                    else if (BookingInwardFilterMaterial && filterItem.PropertyName == "Material\\MaterialNo" && SelectedIntermediate != null && SelectedIntermediate.BookingMaterial != null)
                    {
                        if (filterItem.SearchWord == SelectedIntermediate.BookingMaterial.MaterialNo)
                            countFoundCorrect++;
                    }
                }
                if (BookingInwardFilterMaterial && countFoundCorrect < 2)
                    rebuildACQueryDef = true;
                else if (!BookingInwardFilterMaterial && countFoundCorrect < 1)
                    rebuildACQueryDef = true;
            }
            if (rebuildACQueryDef)
            {
                AccessInBookingFacility.NavACQueryDefinition.ClearFilter(true);
                if (BookingInwardFilterMaterial)
                {
                    AccessInBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, fcTypeContainer.ToString(), true));
                    if (SelectedIntermediate != null && SelectedIntermediate.BookingMaterial != null)
                    {
                        AccessInBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, false));
                        AccessInBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialNo", Global.LogicalOperators.equal, Global.Operators.or, SelectedIntermediate.BookingMaterial.MaterialNo, false));
                        if (SelectedIntermediate.BookingMaterial.Material1_ProductionMaterial != null)
                            AccessInBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialNo", Global.LogicalOperators.equal, Global.Operators.or, SelectedIntermediate.BookingMaterial.Material1_ProductionMaterial.MaterialNo, false));
                        AccessInBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, false));
                    }
                }
                else
                {
                    AccessInBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, fcTypeBin.ToString(), true));
                }
                AccessInBookingFacility.NavACQueryDefinition.SaveConfig(false);
            }
            AccessInBookingFacility.NavSearch(this.DatabaseApp);
        }

        [ACPropertyList(642, "FacilityLots")]
        public IEnumerable<FacilityLot> BookableInwardFacilityLots
        {
            get
            {
                if ((SelectedIntermediate == null) || (SelectedInwardACMethodBooking == null))
                    return null;

                List<FacilityLot> BookableInwardFacilityLots = null;
                if (InwardFacilityPreBookingList != null && InwardFacilityPreBookingList.Any())
                    BookableInwardFacilityLots = InwardFacilityPreBookingList.Where(c => c.InwardFacilityLot != null).Select(c => c.InwardFacilityLot).Distinct().ToList();

                if (InwardFacilityBookingList.Any())
                {
                    var query2 = InwardFacilityBookingList.Where(c => c.InwardFacilityLot != null).Select(c => c.InwardFacilityLot).Distinct();
                    if (BookableInwardFacilityLots == null)
                        BookableInwardFacilityLots = query2.ToList();
                    else
                    {
                        var query3 = BookableInwardFacilityLots.Union(query2);
                        if ((query3 != null) && (query3.Any()))
                            BookableInwardFacilityLots = query3.ToList();
                    }
                }
                var queryFromDB = DatabaseApp.FacilityLot.Where(c => c.Material != null && c.MaterialID == SelectedIntermediate.BookingMaterial.MaterialID);
                if (BookableInwardFacilityLots == null)
                    BookableInwardFacilityLots = queryFromDB.ToList();
                else
                {
                    var query3 = BookableInwardFacilityLots.Union(queryFromDB);
                    if ((query3 != null) && (query3.Any()))
                        BookableInwardFacilityLots = query3.ToList();
                }
                return BookableInwardFacilityLots;
            }
        }
        #endregion

        #endregion

        #region InwardFacilityBooking

        #region InwardFacilityBooking -> Select, (Current,) List

        FacilityBooking _SelectedInwardFacilityBooking;
        [ACPropertySelected(643, "InwardFacilityBooking")]
        public FacilityBooking SelectedInwardFacilityBooking
        {
            get
            {
                return _SelectedInwardFacilityBooking;
            }
            set
            {
                if (_SelectedInwardFacilityBooking != value)
                {
                    _SelectedInwardFacilityBooking = value;
                    OnPropertyChanged("SelectedInwardFacilityBooking");
                    OnPropertyChanged("InwardFacilityBookingChargeList");
                }
            }
        }

        [ACPropertyList(644, "InwardFacilityBooking")]
        public IEnumerable<FacilityBooking> InwardFacilityBookingList
        {
            get
            {
                if (SelectedIntermediate == null)
                    return null;
                if (SelectedProdOrderIntermediateBatch != null)
                {
                    if (_SelectedProdOrderIntermediateBatch != null
                         && _SelectedProdOrderIntermediateBatch.EntityState != EntityState.Added
                         && _SelectedProdOrderIntermediateBatch.EntityState != EntityState.Detached)
                        SelectedProdOrderIntermediateBatch.FacilityBooking_ProdOrderPartslistPos.AutoLoad(SelectedProdOrderIntermediateBatch.FacilityBooking_ProdOrderPartslistPosReference, SelectedProdOrderIntermediateBatch);
                    return SelectedProdOrderIntermediateBatch.FacilityBooking_ProdOrderPartslistPos.OrderBy(c => c.FacilityBookingNo).ToList();
                }
                else
                {
                    if (_SelectedIntermediate != null
                         && _SelectedIntermediate.EntityState != EntityState.Added
                         && _SelectedIntermediate.EntityState != EntityState.Detached)
                        SelectedIntermediate.FacilityBooking_ProdOrderPartslistPos.AutoLoad(SelectedIntermediate.FacilityBooking_ProdOrderPartslistPosReference, SelectedIntermediate);
                    return SelectedIntermediate.FacilityBooking_ProdOrderPartslistPos.OrderBy(c => c.FacilityBookingNo).ToList();
                }
            }
        }

        #endregion


        #region InwardFacilityBooking -> Partner list

        [ACPropertyList(645, "")]
        public IEnumerable<Company> InwardContractualPartnerList
        {
            get
            {
                if (SelectedIntermediate == null)
                    return null;
                if (SelectedIntermediate.BookingMaterial == null)
                    return null;
                return SelectedIntermediate.BookingMaterial.CompanyMaterial_Material.Where(c => c.Company.IsTenant).Select(c => c.Company);
            }
        }

        #endregion

        #endregion

        #region InwardFacilityBookingCharge

        #region InwardFacilityBookingCharge -> Select, (Current,) List

        FacilityBookingCharge _SelectedInwardFacilityBookingCharge;
        [ACPropertySelected(646, "InwardFacilityBookingCharge")]
        public FacilityBookingCharge SelectedInwardFacilityBookingCharge
        {
            get
            {
                return _SelectedInwardFacilityBookingCharge;
            }
            set
            {
                if (_SelectedInwardFacilityBookingCharge != value)
                {
                    _SelectedInwardFacilityBookingCharge = value;
                }
                OnPropertyChanged("SelectedInwardFacilityBookingCharge");
            }
        }

        [ACPropertyList(647, "InwardFacilityBookingCharge")]
        public IEnumerable<FacilityBookingCharge> InwardFacilityBookingChargeList
        {
            get
            {
                if (SelectedInwardFacilityBooking == null)
                    return null;
                SelectedInwardFacilityBooking.FacilityBookingCharge_FacilityBooking.AutoRefresh(SelectedInwardFacilityBooking.FacilityBookingCharge_FacilityBookingReference, SelectedInwardFacilityBooking);
                return SelectedInwardFacilityBooking.FacilityBookingCharge_FacilityBooking.OrderBy(c => c.FacilityBookingChargeNo);
            }
        }

        #endregion

        #endregion

    }
}