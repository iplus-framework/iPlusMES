// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using static gip.core.datamodel.Global;

namespace gip.bso.manufacturing
{
    public partial class BSOProdOrder
    {

        #region OutwardPartslistPos

        #region OutwardPartslistPos -> Select, (Current,) List


        [ACPropertyInfo(777, "SelectedComponent", "en{'Selected component'}de{'Ausgewählte Komponente'}")]
        public ProdOrderPartslistPos SelectedComponent
        {
            get
            {
                if (SelectedOutwardPartslistPos == null)
                    return null;
                return SelectedOutwardPartslistPos.SourceProdOrderPartslistPos;
            }
            set
            {
                if (
                    SelectedOutwardPartslistPos != null
                    && SelectedOutwardPartslistPos.TargetProdOrderPartslistPos != null
                    && (
                         (value == null && SelectedOutwardPartslistPos.SourceProdOrderPartslistPos == null)
                        || (value != null && SelectedOutwardPartslistPos.TargetProdOrderPartslistPos.ProdOrderPartslistID == value.ProdOrderPartslistID)
                       )
                    )
                {
                    if (SelectedOutwardPartslistPos.SourceProdOrderPartslistPos == null || SelectedOutwardPartslistPos.SourceProdOrderPartslistPosID != value.ProdOrderPartslistPosID)
                        SelectedOutwardPartslistPos.SourceProdOrderPartslistPos = value;
                }
                OnPropertyChanged(nameof(SelectedComponent));
            }
        }

        ProdOrderPartslistPosRelation _SelectedOutwardPartslistPos;
        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(648, "OutwardPartslistPos")]
        public ProdOrderPartslistPosRelation SelectedOutwardPartslistPos
        {
            get
            {
                return _SelectedOutwardPartslistPos;
            }
            set
            {
                if (_SelectedOutwardPartslistPos != value)
                {
                    if (_SelectedOutwardPartslistPos != null)
                        _SelectedOutwardPartslistPos.PropertyChanged -= _SelectedOutwardPartslistPos_PropertyChanged;
                    _SelectedOutwardPartslistPos = value;
                    if (_SelectedOutwardPartslistPos != null)
                    {
                        if (_SelectedOutwardPartslistPos.EntityState != EntityState.Added)
                            SearchOutwardFacilityPreBooking();
                        _SelectedOutwardPartslistPos.PropertyChanged += _SelectedOutwardPartslistPos_PropertyChanged;
                    }
                    SelectedComponent = value?.SourceProdOrderPartslistPos;
                    OnPropertyChanged("SelectedOutwardPartslistPos");
                    OnPropertyChanged("OutwardFacilityBookingList");
                }
            }
        }

        private void _SelectedOutwardPartslistPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SourceProdOrderPartslistPosID")
            {
                OnPropertyChanged("SelectedOutwardPartslistPos");
            }
        }

        private List<ProdOrderPartslistPosRelation> _OutwardPartslistPosList;
        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(649, "OutwardPartslistPos")]
        public List<ProdOrderPartslistPosRelation> OutwardPartslistPosList
        {
            get
            {
                if (_OutwardPartslistPosList == null)
                    _OutwardPartslistPosList = new List<ProdOrderPartslistPosRelation>();
                return _OutwardPartslistPosList;
            }
        }

        #endregion

        #region OutwardPartslistPos -> Methods
        [ACMethodCommand("OutwardPartslistPos", "en{'New Input'}de{'Neuer Einsatz'}", (short)MISort.New, true, Global.ACKinds.MSMethodPrePost)]
        public void NewOutwardPartslistPos()
        {
            SearchOutwardPartslistPos();

            ProdOrderPartslistPosRelation newRelation = ProdOrderPartslistPosRelation.NewACObject(DatabaseApp, null);
            DatabaseApp.ProdOrderPartslistPosRelation.Add(newRelation);
            if (SelectedProdOrderIntermediateBatch != null)
                newRelation.TargetProdOrderPartslistPos = SelectedProdOrderIntermediateBatch;
            else
                newRelation.TargetProdOrderPartslistPos = SelectedIntermediate;
            newRelation.Sequence = 1;
            if (OutwardPartslistPosList.Any())
                newRelation.Sequence = OutwardPartslistPosList.Max(x => x.Sequence) + 1;
            PreselecteRelationID = newRelation?.ProdOrderPartslistPosRelationID;
            ACState = Const.SMNew;

            newRelation.PropertyChanged += NewRelation_PropertyChanged;
            OutwardPartslistPosList.Add(newRelation);
            SelectedOutwardPartslistPos = newRelation;
            OnPropertyChanged(nameof(OutwardPartslistPosList));
        }
        public bool IsEnabledNewOutwardPartslistPos()
        {
            return SelectedIntermediate != null && ProdOrderPartslistPosList != null && ProdOrderPartslistPosList.Any();
        }


        private bool disableNewRelation_PropertyChanged;
        private void NewRelation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            bool recalc = false;
            if (disableNewRelation_PropertyChanged)
                return;

            ProdOrderPartslistPosRelation item = sender as ProdOrderPartslistPosRelation;

            switch (e.PropertyName)
            {
                case nameof(ProdOrderPartslistPosRelation.SourceProdOrderPartslistPosID):
                    disableNewRelation_PropertyChanged = true;
                    item.TargetQuantityUOM = item.SourceProdOrderPartslistPos.RemainingCallQuantityUOM;
                    recalc = true;
                    break;
                case nameof(ProdOrderPartslistPosRelation.TargetQuantityUOM):
                    recalc = true;
                    break;
            }
            if (recalc)
            {
                if (item.SourceProdOrderPartslistPos != null)
                    ProdOrderManager.RecalcRemainingOutwardQuantity(item.SourceProdOrderPartslistPos);
            }
            disableNewRelation_PropertyChanged = false;
        }

        [ACMethodCommand("OutwardPartslistPos", "en{'Delete Input'}de{'Lösche Einsatz'}", (short)MISort.Delete, true, Global.ACKinds.MSMethodPrePost)]
        public void DeleteOutwardPartslistPos()
        {
            if (!PreExecute("DeleteOutwardPartslistPos")) return;
            Msg msg = SelectedOutwardPartslistPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
            }
            else
            {
                SearchOutwardPartslistPos();
                ACState = Const.CmdDeleteData;
                RecalculateComponentRemainingQuantity();
            }
            OnPropertyChanged("OutwardPartslistPosList");
            PostExecute("DeleteOutwardPartslistPos");
        }

        public bool IsEnabledDeleteOutwardPartslistPos()
        {
            return SelectedOutwardPartslistPos != null
                &&
                (
                    SelectedOutwardPartslistPos.SourceProdOrderPartslistPos == null
                    ||
                    SelectedOutwardPartslistPos.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                );
        }

        [ACMethodInteraction("OutwardPartslistPos", "en{'Sum bookings to Actual Quantity'}de{'Summiere Buchungen auf Istmenge'}", 610, true, "SelectedOutwardPartslistPos")]
        public void RecalcOutwardPartslistPos()
        {
            SelectedOutwardPartslistPos.RecalcActualQuantity();
            //SelectedOutwardPartslistPos.RecalcActualQuantityFast();
        }

        public bool IsEnabledRecalcOutwardPartslistPos()
        {
            return SelectedOutwardPartslistPos != null;
        }


        public void RecalculateComponentRemainingQuantity()
        {
            if (ProdOrderPartslistPosList != null)
            {
                foreach (ProdOrderPartslistPos pos in ProdOrderPartslistPosList)
                {
                    ProdOrderManager?.RecalcRemainingOutwardQuantity(pos);
                }
            }
            OnPropertyChanged(nameof(ProdOrderPartslistPosList));
        }
        #endregion

        #region OutwardPartslistPos -> Search
        protected void SearchOutwardPartslistPos()
        {
            _OutwardPartslistPosList = null;
            List<Guid> iDs = new List<Guid>();
            if (SelectedIntermediate != null)
            {
                Guid targetID = SelectedIntermediate.ProdOrderPartslistPosID;
                if (SelectedProdOrderIntermediateBatch != null)
                    targetID = SelectedProdOrderIntermediateBatch.ProdOrderPartslistPosID;
                var baseItems =
                    DatabaseApp
                    .ProdOrderPartslistPosRelation
                    .Include(x => x.SourceProdOrderPartslistPos)
                    .Include(x => x.SourceProdOrderPartslistPos.MDUnit)
                    .Include(x => x.SourceProdOrderPartslistPos.Material)
                    .Include(x => x.SourceProdOrderPartslistPos.Material.BaseMDUnit)
                    .Include(x => x.FacilityPreBooking_ProdOrderPartslistPosRelation)
                    .Include(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                    .Where(x => x.TargetProdOrderPartslistPosID == targetID)
                    .OrderBy(x => x.Sequence)
                    .AutoMergeOption(DatabaseApp)
                    .ToList()
                    .Where(x => x.EntityState != EntityState.Deleted)
                    .ToList();
                var localItems = DatabaseApp.GetAddedEntities<ProdOrderPartslistPosRelation>()
                     .Where(x => x.TargetProdOrderPartslistPosID == targetID)
                    .OrderBy(x => x.Sequence)
                    .ToList();
                _OutwardPartslistPosList = baseItems.Union(localItems).ToList();
            }
            if (_OutwardPartslistPosList == null)
                SelectedOutwardPartslistPos = null;
            else
            {
                if (PreselecteRelationID != null)
                    SelectedOutwardPartslistPos = _OutwardPartslistPosList.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == PreselecteRelationID);
                else
                    SelectedOutwardPartslistPos = _OutwardPartslistPosList.FirstOrDefault();
            }
            OnPropertyChanged("OutwardPartslistPosList");
        }
        #endregion

        #endregion

        #region OutwardFacilityPreBooking

        #region OutwardFacilityPreBooking -> Select, (Current,) List

        FacilityPreBooking _SelectedOutwardFacilityPreBooking;
        [ACPropertySelected(650, "OutwardFacilityPreBooking")]
        public FacilityPreBooking SelectedOutwardFacilityPreBooking
        {
            get
            {
                return _SelectedOutwardFacilityPreBooking;
            }
            set
            {
                if (_SelectedOutwardFacilityPreBooking != null && _SelectedOutwardFacilityPreBooking.ACMethodBooking != null)
                    _SelectedOutwardFacilityPreBooking.ACMethodBooking.PropertyChanged -= SelectedOutwardACMethodBooking_OnPropertyChanged;
                if (_SelectedOutwardFacilityPreBooking != value)
                {
                    _SelectedOutwardFacilityPreBooking = value;

                    RefreshFilterOutFacilityAccess();

                    if (_SelectedOutwardFacilityPreBooking != null)
                    {
                        if(_SelectedOutwardFacilityPreBooking.OutwardFacility != null && !AccessOutBookingFacility.NavList.Contains(_SelectedOutwardFacilityPreBooking.OutwardFacility))
                        {
                            AccessOutBookingFacility.NavList.Add(_SelectedOutwardFacilityPreBooking.OutwardFacility);
                        }

                        if (_SelectedOutwardFacilityPreBooking.ACMethodBooking != null)
                        {
                            _SelectedOutwardFacilityPreBooking.ACMethodBooking.PropertyChanged += SelectedOutwardACMethodBooking_OnPropertyChanged;
                        }
                    }
                    
                    OnPropertyChanged(nameof(SelectedOutwardFacilityPreBooking));
                    OnPropertyChanged(nameof(BookingOutwardFacilityList));
                    OnPropertyChanged(nameof(OutwardFacilityChargeList));
                    OnPropertyChanged(nameof(SelectedOutwardACMethodBooking));
                    OnPropertyChanged(nameof(IsEnabledOutwardFacilityChargeSelect));
                }
            }
        }

        public bool IsNotNullSelectedOutwardFacilityPreBooking
        {
            get
            {
                return SelectedOutwardFacilityPreBooking != null;
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(651, "OutwardFacilityPreBooking")]
        public List<FacilityPreBooking> OutwardFacilityPreBookingList
        {
            get
            {
                if (SelectedOutwardPartslistPos == null)
                    return null;
                if (_SelectedOutwardPartslistPos != null
                         && _SelectedOutwardPartslistPos.EntityState != EntityState.Added
                         && _SelectedOutwardPartslistPos.EntityState != EntityState.Detached)
                    SelectedOutwardPartslistPos.FacilityPreBooking_ProdOrderPartslistPosRelation.AutoLoad(SelectedOutwardPartslistPos.FacilityPreBooking_ProdOrderPartslistPosRelationReference, SelectedOutwardPartslistPos);
                return SelectedOutwardPartslistPos.FacilityPreBooking_ProdOrderPartslistPosRelation.ToList();
            }
        }


        private Material _SelectedOutwardACMethodBookingAlternativeMaterial;
        [ACPropertySelected(652, "OutwardACMethodBookingAlternativeMaterial", "en{'Alternative material'}de{'Aternativmaterial'}")]
        public Material SelectedOutwardACMethodBookingAlternativeMaterial
        {
            get
            {
                return _SelectedOutwardACMethodBookingAlternativeMaterial;
            }
            set
            {
                if (_SelectedOutwardACMethodBookingAlternativeMaterial != value)
                {
                    _SelectedOutwardACMethodBookingAlternativeMaterial = value;
                    if (value == null)
                    {
                        ProdOrderPartslistPos posItemForBooking = SelectedIntermediate;
                        if (SelectedProdOrderIntermediateBatch != null)
                            posItemForBooking = SelectedProdOrderIntermediateBatch;
                        (SelectedOutwardACMethodBooking as ACMethodBooking).OutwardMaterial = posItemForBooking.BookingMaterial;
                    }
                    else
                    {
                        (SelectedOutwardACMethodBooking as ACMethodBooking).OutwardMaterial = value;
                    }
                    RefreshFilterOutFacilityAccess();
                    SelectedOutwardACMethodBooking.OutwardFacility = null;
                    OnPropertyChanged("BookingOutwardFacilityList");
                    OnPropertyChanged("SelectedOutwardACMethodBookingAlternativeMaterial");
                }
            }
        }

        [ACPropertyList(653, "OutwardACMethodBookingAlternativeMaterial")]
        public List<Material> OutwardACMethodBookingAlternativeMaterialList
        {
            get
            {
                if (AlternativeProdOrderPartslistPosList == null) return null;
                return AlternativeProdOrderPartslistPosList.Select(c => c.Material).ToList();
            }
        }

        #endregion

        #region OutwardFacilityPreBooking -> Methods

        #region OutwardFacilityPreBooking -> Methods -> Book, Cancel

        // GUI #4 BookSelectedOutwardACMethodBooking
        [ACMethodInteraction("OutwardFacilityPreBooking", "en{'Post Item'}de{'Position Buchen'}", (short)MISort.New, true, "SelectedOutwardFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public virtual void BookSelectedOutwardACMethodBooking(bool refreshList = true)
        {
            if (!IsEnabledBookSelectedOutwardACMethodBooking()) return;

            if (SelectedOutwardACMethodBooking.PartslistPosRelation != SelectedOutwardPartslistPos)
                SelectedOutwardACMethodBooking.PartslistPosRelation = SelectedOutwardPartslistPos;
            if (CurrentProdOrder.CPartnerCompany != null && SelectedOutwardACMethodBooking.CPartnerCompany != CurrentProdOrder.CPartnerCompany)
                SelectedOutwardACMethodBooking.CPartnerCompany = CurrentProdOrder.CPartnerCompany;

            if (SelectedOutwardACMethodBooking.FacilityBooking != null)
            {
                if (SelectedOutwardACMethodBooking.FacilityBooking.ProdOrderPartslistPosRelation != SelectedOutwardACMethodBooking.PartslistPosRelation)
                    SelectedOutwardACMethodBooking.FacilityBooking.ProdOrderPartslistPosRelation = SelectedOutwardACMethodBooking.PartslistPosRelation;

                if (SelectedOutwardACMethodBooking.FacilityBooking.OutwardMaterial != SelectedOutwardACMethodBooking.OutwardMaterial)
                    SelectedOutwardACMethodBooking.FacilityBooking.OutwardMaterial = SelectedOutwardACMethodBooking.OutwardMaterial;

                if (SelectedOutwardACMethodBooking.FacilityBooking.OutwardFacility != SelectedOutwardACMethodBooking.OutwardFacility)
                    SelectedOutwardACMethodBooking.FacilityBooking.OutwardFacility = SelectedOutwardACMethodBooking.OutwardFacility;
            }

            bool isCancellation = SelectedOutwardACMethodBooking.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutwardCancel || SelectedOutwardACMethodBooking.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel;

            Save();
            if (DatabaseApp.IsChanged)
                return;

            if (SelectedOutwardACMethodBooking.OutwardFacilityCharge != null)
                SelectedOutwardACMethodBooking.OutwardFacilityLot = SelectedOutwardACMethodBooking.OutwardFacilityCharge.FacilityLot;


            SelectedOutwardACMethodBooking.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(SelectedOutwardACMethodBooking, this.DatabaseApp) as ACMethodEventArgs;
            if (!SelectedOutwardACMethodBooking.ValidMessage.IsSucceded() || SelectedOutwardACMethodBooking.ValidMessage.HasWarnings())
                Messages.Msg(SelectedOutwardACMethodBooking.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(SelectedOutwardACMethodBooking.ValidMessage.Message))
                    SelectedOutwardACMethodBooking.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(SelectedOutwardACMethodBooking.ValidMessage);
                if (refreshList)
                    OnPropertyChanged(nameof(OutwardFacilityBookingList));
            }
            else
            {
                double quantity = 0;
                if (SelectedOutwardACMethodBooking.OutwardQuantity.HasValue)
                    quantity = SelectedOutwardACMethodBooking.OutwardQuantity.Value;
                MDUnit unit = SelectedOutwardACMethodBooking.MDUnit;

                DeleteOutwardFacilityPreBooking();
                OnPropertyChanged(nameof(OutwardFacilityBookingList));

                // First recalc actual quantity for relation parts
                if (unit != null)
                    SelectedOutwardPartslistPos.IncreaseActualQuantity(quantity, unit, true);
                else
                    SelectedOutwardPartslistPos.IncreaseActualQuantityUOM(quantity, true);

                //SelectedOutwardPartslistPos.TopParentPartslistPosRelation.RecalcActualQuantity();
                //SelectedOutwardPartslistPos.SourceProdOrderPartslistPos.TopParentPartslistPos.RecalcActualQuantity();
                RecalcInBookingValues();
                if (isCancellation)
                {
                    MDProdOrderPartslistPosState state = DatabaseApp.s_cQry_GetMDProdOrderPosState(DatabaseApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled).FirstOrDefault();
                    if (state != null)
                    {

                    }
                    SelectedOutwardPartslistPos.SourceProdOrderPartslistPos.MDProdOrderPartslistPosState = state;
                }
                Save();

                SelectedOutwardPartslistPos.RecalcActualQuantityFast();
                if (DatabaseApp.IsChanged)
                {
                    Save();
                }
            }
        }

        public virtual bool IsEnabledBookSelectedOutwardACMethodBooking()
        {
            bool bRetVal = false;
            if (_SelectedOutwardACMethodBookingDummy != null)
                return false;
            if (SelectedOutwardACMethodBooking != null)
                bRetVal = SelectedOutwardACMethodBooking.IsEnabled();
            else
                return false;
            //if (this.OutwardPartslistPosList == null || this.OutwardPartslistPosList.Where(c => c.ActualQuantityUOM < 0.00001).Any())
            //    return false;
            //UpdateBSOMsg();
            return true;
        }

        // GUI #5 BookAllOutwardACMBookings
        [ACMethodCommand("OutwardFacilityPreBooking", "en{'Post All'}de{'Buche alle'}", (short)MISort.Cancel)]
        public void BookAllOutwardACMBookings()
        {
            if (!IsEnabledBookAllOutwardACMBookings())
                return;
            foreach (FacilityPreBooking facilityPreBooking in OutwardFacilityPreBookingList.ToList())
            {
                SelectedOutwardFacilityPreBooking = facilityPreBooking;
                if (SelectedOutwardFacilityPreBooking == facilityPreBooking)
                    BookSelectedOutwardACMethodBooking(false);
            }
            OnPropertyChanged("OutwardFacilityBookingList");
        }

        public bool IsEnabledBookAllOutwardACMBookings()
        {
            return !(SelectedOutwardPartslistPos == null || OutwardFacilityPreBookingList == null || !OutwardFacilityPreBookingList.Any());
        }

        /// <summary>
        /// back to storage bin (material quantity)
        /// Delete facility booking record - 
        /// </summary>
        [ACMethodInteraction("OutwardFacilityPreBooking", "en{'Cancel Posting'}de{'Buchung abbrechen'}", (short)MISort.Cancel, true, "SelectedOutwardFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void CancelOutwardFacilityPreBooking()
        {
            if (!IsEnabledCancelOutwardFacilityPreBooking()) return;
            var result = ProdOrderManager.CancelOutFacilityPreBooking(ACFacilityManager, DatabaseApp, SelectedProdOrderPartslist);
            if (result != null && result.Any())
            {
                foreach (var fbc in result)
                    SelectedOutwardPartslistPos.FacilityPreBooking_ProdOrderPartslistPosRelation.Add(fbc);
                SelectedOutwardFacilityPreBooking = OutwardFacilityPreBookingList.FirstOrDefault();
                OnPropertyChanged("OutwardFacilityPreBookingList");
                OnPropertyChanged("OutwardFacilityBookingList");
            }
        }

        public bool IsEnabledCancelOutwardFacilityPreBooking()
        {
            return !(SelectedOutwardPartslistPos == null || OutwardFacilityPreBookingList.Any());
        }

        #endregion

        #region OutwardFacilityPreBooking -> Methods -> Manupulate (New, Delete..)

        [ACMethodInteraction("OutwardFacilityPreBooking", "en{'New Posting'}de{'Neue Buchung'}", (short)MISort.New, true, "SelectedOutwardFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void NewOutwardFacilityPreBooking()
        {
            if (!IsEnabledNewOutwardFacilityPreBooking()) return;
            if (!PreExecute("NewOutwardFacilityPreBooking")) return;
            FacilityPreBooking newFacilityPrebookingItem = ProdOrderManager.NewOutwardFacilityPreBooking(ACFacilityManager, DatabaseApp, SelectedOutwardPartslistPos);
            SelectedOutwardPartslistPos.FacilityPreBooking_ProdOrderPartslistPosRelation.Add(newFacilityPrebookingItem);
            SelectedOutwardFacilityPreBooking = newFacilityPrebookingItem;
            OnPropertyChanged("OutwardFacilityPreBookingList");
            PostExecute("NewOutwardFacilityPreBooking");
        }

        public bool IsEnabledNewOutwardFacilityPreBooking()
        {
            return SelectedOutwardPartslistPos != null;
        }

        // GUI #2 DeleteOutwardFacilityPreBooking
        [ACMethodInteraction("OutwardFacilityPreBooking", "en{'Delete Posting'}de{'Lösche Buchung'}", (short)MISort.Delete, true, "SelectedPartslistPos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteOutwardFacilityPreBooking()
        {
            if (!IsEnabledDeleteOutwardFacilityPreBooking()) return;
            SelectedOutwardPartslistPos.FacilityPreBooking_ProdOrderPartslistPosRelation.Remove(SelectedOutwardFacilityPreBooking);
            Msg msg = SelectedOutwardFacilityPreBooking.DeleteACObject(DatabaseApp, true);
            Msg saveMsg = DatabaseApp.ACSaveChanges();
            if (msg != null || (saveMsg != null && !saveMsg.IsSucceded()))
            {
                if (msg != null)
                    Messages.Msg(msg);
                if (saveMsg != null)
                    Messages.Msg(saveMsg);
                return;
            }
            else
            {
                SelectedOutwardFacilityPreBooking = OutwardFacilityPreBookingList.FirstOrDefault();
                OnPropertyChanged("OutwardFacilityPreBookingList");
            }
        }
        public bool IsEnabledDeleteOutwardFacilityPreBooking()
        {
            return SelectedOutwardFacilityPreBooking != null;
        }

        #endregion

        #region OutwardFacilityPreBooking -> Methods -> Dlg Facility, FaciltiyCharge
        public FacilitySelectLoctation FacilitySelectLoctation { get; set; }

        [ACMethodInfo("ShowDlgInwardFacility", "en{'Choose facility'}de{'Lager auswählen'}", 999)]
        public void ShowDlgOutwardFacility()
        {
            if (!IsEnabledShowDlgOutwardFacility())
                return;
            FacilitySelectLoctation = FacilitySelectLoctation.PrebookingOutward;
            ShowDlgFacility(SelectedOutwardACMethodBooking.OutwardFacility);
        }

        public bool IsEnabledShowDlgOutwardFacility()
        {
            return SelectedOutwardACMethodBooking != null;
        }

        /// <summary>
        /// @aagincic: only added - not used for now
        /// </summary>
        [ACMethodInfo("ShowDlgOutwardAvailableQuants", "en{'Choose quant'}de{'Quant auswählen'}", 999)]
        public void ShowDlgOutwardAvailableQuants()
        {
            if (!IsEnabledShowDlgOutwardAvailableQuants())
                return;
            _QuantDialogMaterial = SelectedOutwardACMethodBooking.OutwardMaterial;
            _PreBookingAvailableQuantsList = null;
            ShowDialog(this, "DlgAvailableQuants");
        }

        public bool IsEnabledShowDlgOutwardAvailableQuants()
        {
            return SelectedOutwardACMethodBooking != null && SelectedOutwardACMethodBooking.OutwardMaterial != null;
        }
        #endregion

        #endregion

        #region OutwardFacilityPreBooking -> Search

        protected void SearchOutwardFacilityPreBooking()
        {
            if (OutwardFacilityPreBookingList == null)
                SelectedOutwardFacilityPreBooking = null;
            else
                SelectedOutwardFacilityPreBooking = OutwardFacilityPreBookingList.FirstOrDefault();
            OnPropertyChanged("OutwardFacilityPreBookingList");
        }

        #endregion

        #region OutwardFacilityPreBooking -> ACMethod
        ACMethodBooking _SelectedOutwardACMethodBookingDummy = null; // Dummy-Parameter notwendig, damit Bindung an Oberfläche erfolgen kann, da abgeleitete Klasse
        [ACPropertyInfo(654, "", "en{'Posting parameter'}de{'Buchungsparameter'}")]
        public ACMethodBooking SelectedOutwardACMethodBooking
        {
            get
            {
                if (SelectedOutwardFacilityPreBooking == null && this.ProdOrderManager != null)
                {
                    if (_SelectedOutwardACMethodBookingDummy != null) return _SelectedOutwardACMethodBookingDummy;
                    ACMethodBooking acMethodClone = this.ProdOrderManager.BookParamOutwardMovementClone(this.ACFacilityManager, this.DatabaseApp);
                    if (acMethodClone != null)
                        _SelectedOutwardACMethodBookingDummy = acMethodClone.Clone() as ACMethodBooking;
                    return _SelectedOutwardACMethodBookingDummy;
                }
                _SelectedOutwardACMethodBookingDummy = null;
                if (_SelectedOutwardFacilityPreBooking == null)
                    return null;
                //if (_SelectedOutwardFacilityPreBooking.ACMethodBooking != null)
                //{
                //    _SelectedOutwardFacilityPreBooking.ACMethodBooking.PropertyChanged -= ACMethodBookingInward_PropertyChanged;
                //    _SelectedOutwardFacilityPreBooking.ACMethodBooking.PropertyChanged += ACMethodBookingInward_PropertyChanged;
                //}
                return SelectedOutwardFacilityPreBooking.ACMethodBooking as ACMethodBooking;
            }
            set
            {
                if (SelectedOutwardFacilityPreBooking != null)
                {
                    if (SelectedOutwardFacilityPreBooking.ACMethodBooking != null)
                        SelectedOutwardFacilityPreBooking.ACMethodBooking.PropertyChanged -= SelectedOutwardACMethodBooking_OnPropertyChanged;
                    SelectedOutwardFacilityPreBooking.ACMethodBooking = value;
                    if (SelectedOutwardFacilityPreBooking.ACMethodBooking != null)
                        SelectedOutwardFacilityPreBooking.ACMethodBooking.PropertyChanged += SelectedOutwardACMethodBooking_OnPropertyChanged;
                }
                else
                {
                    _SelectedOutwardACMethodBookingDummy = null;
                }
                OnPropertyChanged("SelectedOutwardACMethodBooking");
            }
        }

        bool _UpdatingControlModeOutward = false;
        void SelectedOutwardACMethodBooking_OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_UpdatingControlModeOutward)
                return;
            try
            {
                if (e.PropertyName == "OutwardFacility")
                {
                    _UpdatingControlModeOutward = true;
                    OnPropertyChanged("OutwardFacilityChargeList");
                    SelectedOutwardACMethodBooking.OnEntityPropertyChanged("OutwardFacility");
                    SelectedOutwardACMethodBooking.OnEntityPropertyChanged("OutwardFacilityCharge");
                    SelectedOutwardACMethodBooking.OnEntityPropertyChanged("OutwardFacilityLot");
                }

                if (e.PropertyName == "OutwardQuantity")
                {
                    _UpdatingControlModeOutward = true;
                    SelectedOutwardACMethodBooking.OnEntityPropertyChanged("OutwardQuantity");
                }
            }
            finally
            {
                _UpdatingControlModeOutward = false;
            }
        }

        #endregion

        #endregion

        #region BookingOutwardFacility

        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<Facility> _AccessOutBookingFacility;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(694, "BookingOutwardFacility")]
        public ACAccessNav<Facility> AccessOutBookingFacility
        {
            get
            {
                if (_AccessOutBookingFacility == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "BookingFacility", ACType.ACIdentifier);
                    _AccessOutBookingFacility = navACQueryDefinition.NewAccessNav<Facility>("BookingOutwardFacility", this);
                    _AccessOutBookingFacility.AutoSaveOnNavigation = false;
                    RefreshFilterOutFacilityAccess();
                }
                return _AccessOutBookingFacility;
            }
        }


        [ACPropertyList(655, "BookingOutwardFacility")]
        public IList<Facility> BookingOutwardFacilityList
        {
            get
            {
                if (AccessOutBookingFacility == null)
                    return null;
                return AccessOutBookingFacility.NavList;
            }
        }

        private void RefreshFilterOutFacilityAccess()
        {
            if (AccessOutBookingFacility == null || SelectedOutwardFacilityPreBooking == null || OutwardFacilityPreBookingList == null || !OutwardFacilityPreBookingList.Any())
                return;
            bool rebuildACQueryDef = false;
            short fcTypeContainer = (short)FacilityTypesEnum.StorageBinContainer;
            short fcTypeBin = (short)FacilityTypesEnum.StorageBin;
            short fcTypePrepBin = (short)FacilityTypesEnum.PreparationBin;

            if (AccessOutBookingFacility.NavACQueryDefinition.ACFilterColumns.Count <= 0)
            {
                rebuildACQueryDef = true;
            }
            else
            {
                int countFoundCorrect = 0;
                foreach (ACFilterItem filterItem in AccessOutBookingFacility.NavACQueryDefinition.ACFilterColumns)
                {
                    if (filterItem.FilterType != Global.FilterTypes.filter)
                        continue;
                    if (filterItem.PropertyName == "MDFacilityType\\MDFacilityTypeIndex")
                    {
                        if ((BookingOutwardFilterMaterial && filterItem.SearchWord == fcTypeContainer.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal)
                            || (!BookingOutwardFilterMaterial && filterItem.SearchWord == fcTypeBin.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal)
                            || (!BookingOutwardFilterMaterial && filterItem.SearchWord == fcTypePrepBin.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal))
                            countFoundCorrect++;
                    }
                    else if (BookingOutwardFilterMaterial)
                    {
                        if (filterItem.PropertyName == "Material\\MaterialNo"
                        && SelectedOutwardPartslistPos != null
                        && SelectedOutwardPartslistPos.SourceProdOrderPartslistPos != null
                        && SelectedOutwardPartslistPos.SourceProdOrderPartslistPos.Material != null)
                        {
                            if (filterItem.SearchWord == SelectedOutwardPartslistPos.SourceProdOrderPartslistPos.Material.MaterialNo)
                                countFoundCorrect++;
                        }

                        if (SelectedOutwardACMethodBookingAlternativeMaterial != null &&
                            filterItem.SearchWord != SelectedOutwardACMethodBookingAlternativeMaterial.MaterialNo)
                            countFoundCorrect--;
                    }
                }
                if (BookingOutwardFilterMaterial && countFoundCorrect < 2)
                    rebuildACQueryDef = true;
                else if (!BookingOutwardFilterMaterial && countFoundCorrect < 2)
                    rebuildACQueryDef = true;
            }
            if (rebuildACQueryDef)
            {
                AccessOutBookingFacility.NavACQueryDefinition.ClearFilter(true);
                if (BookingOutwardFilterMaterial)
                {
                    AccessOutBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, fcTypeContainer.ToString(), true));
                    if (SelectedOutwardPartslistPos != null
                        && SelectedOutwardPartslistPos.SourceProdOrderPartslistPos != null
                        && SelectedOutwardPartslistPos.SourceProdOrderPartslistPos.Material != null

                        )
                    {
                        AccessOutBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, false));
                        string materialNo = SelectedOutwardPartslistPos.SourceProdOrderPartslistPos.Material.MaterialNo;
                        if (SelectedOutwardACMethodBookingAlternativeMaterial != null)
                            materialNo = SelectedOutwardACMethodBookingAlternativeMaterial.MaterialNo;
                        AccessOutBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialNo", Global.LogicalOperators.equal, Global.Operators.or, materialNo, false));
                        if (SelectedOutwardPartslistPos.SourceProdOrderPartslistPos.Material.Material1_ProductionMaterial != null)
                            AccessOutBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialNo", Global.LogicalOperators.equal, Global.Operators.or, SelectedOutwardPartslistPos.SourceProdOrderPartslistPos.Material.Material1_ProductionMaterial.MaterialNo, false));
                        AccessOutBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, false));
                    }
                }
                else
                {
                    AccessOutBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, false));
                    AccessOutBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.or, fcTypeBin.ToString(), true));
                    AccessOutBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.or, fcTypePrepBin.ToString(), true));
                    AccessOutBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, false));
                }
                AccessOutBookingFacility.NavACQueryDefinition.SaveConfig(false);
            }
            AccessOutBookingFacility.NavSearch(this.DatabaseApp);
        }

        #endregion

        #region OutwardFacilityCharge

        #region OutwardFacilityCharge -> Select, (Current,) List


        [ACPropertyList(656, "OutwardFacilityCharge")]
        public IEnumerable<FacilityCharge> OutwardFacilityChargeList
        {
            get
            {
                if (SelectedOutwardACMethodBooking == null || SelectedOutwardACMethodBooking.OutwardFacility == null) return null;
                return SelectedOutwardACMethodBooking.OutwardFacility.FacilityCharge_Facility
                    .Where(x => x.MaterialID == SelectedOutwardPartslistPos.SourceProdOrderPartslistPos.MaterialID && !x.NotAvailable).OrderByDescending(x => x.InsertDate);
            }
        }

        /// <summary>
        /// Selection of facility charge is enabled when:
        /// - material is lot managed
        /// - facility is "Freifäche" - user can select facility charge is not managed behind
        /// </summary>
        [ACPropertyInfo(657, "OutwardFacilityCharge")]
        public bool IsEnabledOutwardFacilityChargeSelect
        {
            get
            {
                return
                        SelectedOutwardACMethodBooking != null
                        && SelectedOutwardACMethodBooking.OutwardFacility != null
                        && SelectedOutwardACMethodBooking.OutwardMaterial != null
                        //&& !SelectedOutwardACMethodBooking.OutwardFacility.MDFacilityType.AutomaticControlFacilityCharge
                        && SelectedOutwardACMethodBooking.IsLotManaged;
            }
        }
        #endregion


        #endregion

        #region OutwardFacilityBooking

        #region OutwardFacilityBooking -> Select, (Current,) List

        FacilityBooking _SelectedOutwardFacilityBooking;
        [ACPropertySelected(658, "OutwardFacilityBooking")]
        public FacilityBooking SelectedOutwardFacilityBooking
        {
            get
            {
                return _SelectedOutwardFacilityBooking;
            }
            set
            {
                if (_SelectedOutwardFacilityBooking != value)
                {
                    _SelectedOutwardFacilityBooking = value;
                    OnPropertyChanged("SelectedOutwardFacilityBooking");
                    OnPropertyChanged("OutwardFacilityBookingChargeList");
                }
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(659, "OutwardFacilityBooking")]
        public IEnumerable<FacilityBooking> OutwardFacilityBookingList
        {
            get
            {
                if (SelectedOutwardPartslistPos == null)
                    return null;
                if (_SelectedOutwardPartslistPos != null
                        && _SelectedOutwardPartslistPos.EntityState != EntityState.Added
                        && _SelectedOutwardPartslistPos.EntityState != EntityState.Detached)
                    SelectedOutwardPartslistPos.FacilityBooking_ProdOrderPartslistPosRelation.AutoLoad(SelectedOutwardPartslistPos.FacilityBooking_ProdOrderPartslistPosRelationReference, SelectedOutwardPartslistPos);
                return SelectedOutwardPartslistPos.FacilityBooking_ProdOrderPartslistPosRelation.OrderBy(c => c.FacilityBookingNo).ToList();
            }
        }

        #endregion

        #region OutwardFacilityBooking -> Company list

        [ACPropertyList(660, "")]
        public IEnumerable<Company> OutwardContractualPartnerList
        {
            get
            {
                if (SelectedOutwardPartslistPos == null)
                    return null;
                if (SelectedOutwardPartslistPos.SourceProdOrderPartslistPos.Material == null)
                    return null;
                return SelectedOutwardPartslistPos.SourceProdOrderPartslistPos.Material.CompanyMaterial_Material.Where(c => c.Company.IsTenant).Select(c => c.Company);
            }
        }

        #endregion

        #endregion

        #region OutwardFacilityBookingCharge

        #region OutwardFacilityBookingCharge -> Select, (Current,) List

        FacilityBookingCharge _SelectedOutwardFacilityBookingCharge;
        [ACPropertySelected(661, "OutwardFacilityBookingCharge")]
        public FacilityBookingCharge SelectedOutwardFacilityBookingCharge
        {
            get
            {
                return _SelectedOutwardFacilityBookingCharge;
            }
            set
            {
                if (_SelectedOutwardFacilityBookingCharge != value)
                {
                    _SelectedOutwardFacilityBookingCharge = value;
                    OnPropertyChanged("SelectedOutwardFacilityBookingCharge");
                }
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(662, "OutwardFacilityBookingCharge")]
        public IEnumerable<FacilityBookingCharge> OutwardFacilityBookingChargeList
        {
            get
            {
                if (SelectedOutwardFacilityBooking == null)
                    return null;
                SelectedOutwardFacilityBooking.FacilityBookingCharge_FacilityBooking.AutoRefresh(SelectedOutwardFacilityBooking.FacilityBookingCharge_FacilityBookingReference, SelectedOutwardFacilityBooking);
                return SelectedOutwardFacilityBooking.FacilityBookingCharge_FacilityBooking.OrderBy(c => c.FacilityBookingChargeNo);
            }
        }

        #endregion

        [ACMethodInteraction("", "en{'Correct posting'}de{'Korrekte Buchung'}", 660, true, nameof(SelectedOutwardFacilityBookingCharge))]
        public void CorrectBooking()
        {
            if (SelectedOutwardFacilityBookingCharge == null)
                return;

            FacilityCharge outwardFacilityCharge = SelectedOutwardFacilityBookingCharge.OutwardFacilityCharge;

            if (outwardFacilityCharge == null)
                return;

            NewOutwardFacilityPreBooking();

            if (SelectedOutwardACMethodBooking == null)
            {
                //todo error
                return;
            }

            SelectedOutwardACMethodBooking.OutwardFacility = outwardFacilityCharge.Facility;
            SelectedOutwardACMethodBooking.OutwardFacilityCharge = outwardFacilityCharge;
            SelectedOutwardACMethodBooking.OutwardQuantity = outwardFacilityCharge.StockQuantity < 0.00001 ? outwardFacilityCharge.StockQuantity : 0;

            ShowDialog(this, "CorrectBookingDialog");

            if (ACFacilityManager != null)
            {
                Msg msg = ACFacilityManager.IsQuantStockConsumed(outwardFacilityCharge, DatabaseApp);
                if (msg != null)
                {
                    if (Messages.Question(this, msg.Message, MsgResult.No, true) == MsgResult.Yes)
                    {
                        ACMethodBooking fbtZeroBookingClone = ACFacilityManager.GetBookParamNotAvailableClone();

                        fbtZeroBookingClone.InwardFacilityCharge = outwardFacilityCharge;
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
                            Messages.Msg(msgSave);
                    }
                }
            }

            if (SelectedOutwardFacilityPreBooking != null)
            {
                DeleteOutwardFacilityPreBooking();
            }
        }

        public bool IsEnabledCorrectBooking()
        {
            return SelectedOutwardFacilityBookingCharge != null
                && SelectedOutwardFacilityBookingCharge.OutwardFacilityCharge != null
                && SelectedOutwardFacilityBookingCharge.OutwardFacilityCharge.Facility != null
                && SelectedOutwardFacilityBookingCharge.OutwardFacilityCharge.Facility.MDFacilityType != null
                && SelectedOutwardFacilityBookingCharge.OutwardFacilityCharge.Facility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBinContainer;
        }

        #endregion

        #region Others

        [ACPropertyList(663, "FacilityLots")]
        public IEnumerable<FacilityLot> BookableOutwardFacilityLots
        {
            get
            {
                if ((SelectedOutwardPartslistPos == null) || (SelectedOutwardACMethodBooking == null))
                    return null;

                List<FacilityLot> BookableOutwardFacilityLots = null;
                if (OutwardFacilityPreBookingList != null && OutwardFacilityPreBookingList.Any())
                    BookableOutwardFacilityLots = OutwardFacilityPreBookingList.Where(c => c.InwardFacilityLot != null).Select(c => c.InwardFacilityLot).Distinct().ToList();

                if (OutwardFacilityBookingList.Any())
                {
                    var query2 = OutwardFacilityBookingList.Where(c => c.InwardFacilityLot != null).Select(c => c.InwardFacilityLot).Distinct();
                    if (BookableOutwardFacilityLots == null)
                        BookableOutwardFacilityLots = query2.ToList();
                    else
                    {
                        var query3 = BookableOutwardFacilityLots.Union(query2);
                        if ((query3 != null) && (query3.Any()))
                            BookableOutwardFacilityLots = query3.ToList();
                    }
                }
                var queryFromDB = DatabaseApp.FacilityLot; // TODO: fix .Where(c => c.Material != null && c.MaterialID == CurrentOutwardPartslistPos.MaterialID);
                if (BookableOutwardFacilityLots == null)
                    BookableOutwardFacilityLots = queryFromDB.ToList();
                else
                {
                    var query3 = BookableOutwardFacilityLots.Union(queryFromDB);
                    if ((query3 != null) && (query3.Any()))
                        BookableOutwardFacilityLots = query3.ToList();
                }
                return BookableOutwardFacilityLots;
            }
        }

        private bool _BookingOutwardFilterMaterial = true;
        [ACPropertyInfo(664, "", "en{'Only show bins with material'}de{'Zeige Lagerpätze mit Material'}")]
        public bool BookingOutwardFilterMaterial
        {
            get
            {
                return _BookingOutwardFilterMaterial;
            }
            set
            {
                if (value != _BookingOutwardFilterMaterial)
                {
                    _BookingOutwardFilterMaterial = value;
                    OnPropertyChanged("BookingOutwardFilterMaterial");
                    RefreshFilterOutFacilityAccess();
                    OnPropertyChanged("BookingOutwardFacilityList");
                    OnPropertyChanged("OutwardFacilityChargeList");
                    OnPropertyChanged("SelectedOutwardACMethodBooking");
                }
            }
        }

        #endregion

    }
}
