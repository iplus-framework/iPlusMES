// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.FacilityReservation, Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOFacilityReservationOverview : ACBSOvb
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityReservationOverview"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityReservationOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {

            bool baseResult = base.ACDeInit(deleteACClassTask);

            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            return baseResult;
        }

        #endregion

        #region Managers
        protected ACRef<ACComponent> _ACFacilityManager = null;
        public FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }
        #endregion

        #region BSO->ACProperty

        #region BSO->ACProperty->PickingReservation

        private FacilityReservationModel _SelectedPickingReservation;
        /// <summary>
        /// Selected property for FacilityReservation
        /// </summary>
        /// <value>The selected PickingReservation</value>
        [ACPropertySelected(10, "PickingReservation", "en{'TODO: PickingReservation'}de{'TODO: PickingReservation'}")]
        public FacilityReservationModel SelectedPickingReservation
        {
            get
            {
                return _SelectedPickingReservation;
            }
            set
            {
                if (_SelectedPickingReservation != value)
                {
                    _SelectedPickingReservation = value;
                    OnPropertyChanged(nameof(SelectedPickingReservation));
                }
            }
        }


        private List<FacilityReservationModel> _PickingReservationList;
        /// <summary>
        /// List property for FacilityReservation
        /// </summary>
        /// <value>The PickingReservation list</value>
        [ACPropertyList(20, "PickingReservation")]
        public List<FacilityReservationModel> PickingReservationList
        {
            get
            {
                return _PickingReservationList;
            }
        }

        #endregion

        #region BSO->ACProperty->ProdOrderReservation

        private FacilityReservationModel _SelectedProdOrderReservation;
        /// <summary>
        /// Selected property for FacilityReservation
        /// </summary>
        /// <value>The selected ProdOrderReservation</value>
        [ACPropertySelected(30, "ProdOrderReservation", "en{'TODO: ProdOrderReservation'}de{'TODO: ProdOrderReservation'}")]
        public FacilityReservationModel SelectedProdOrderReservation
        {
            get
            {
                return _SelectedProdOrderReservation;
            }
            set
            {
                if (_SelectedProdOrderReservation != value)
                {
                    _SelectedProdOrderReservation = value;
                    OnPropertyChanged(nameof(SelectedProdOrderReservation));
                }
            }
        }


        private List<FacilityReservationModel> _ProdOrderReservationList;
        /// <summary>
        /// List property for FacilityReservation
        /// </summary>
        /// <value>The ProdOrderReservation list</value>
        [ACPropertyList(40, "ProdOrderReservation")]
        public List<FacilityReservationModel> ProdOrderReservationList
        {
            get
            {
                return _ProdOrderReservationList;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region BSO->ACMethod

        [ACMethodInteraction("Navigation", "en{'Show Picking'}de{'Zum Kommissionauftrag'}", 100, true, nameof(SelectedPickingReservation))]
        public void ReservationNavigateToPicking()
        {
            if (!IsEnabledReservationNavigateToPicking())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(Picking), SelectedPickingReservation.PickingID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledReservationNavigateToPicking()
        {
            return SelectedPickingReservation != null && !string.IsNullOrEmpty(SelectedPickingReservation.DocumentNo) && SelectedPickingReservation.PickingID != Guid.Empty;
        }

        [ACMethodInteraction("Navigation", "en{'Show ProdOrder'}de{'Zum Produktionauftrag'}", 110, true, nameof(SelectedProdOrderReservation))]
        public void ReservationNavigateToProdOrder()
        {
            if (!IsEnabledReservationNavigateToProdOrder())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(ProdOrderPartslist), SelectedProdOrderReservation.ProdOrderPartslistID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledReservationNavigateToProdOrder()
        {
            return SelectedProdOrderReservation != null && !string.IsNullOrEmpty(SelectedProdOrderReservation.DocumentNo) && SelectedProdOrderReservation.ProdOrderPartslistID != Guid.Empty;
        }

        #endregion

        #region Methods -> Loading

        public void LoadReservation(FacilityCharge facilityCharge)
        {
            _PickingReservationList = null;
            _ProdOrderReservationList = null;

            if (facilityCharge != null)
            {
                FacilityReservation[] reservations = facilityCharge.FacilityReservation_FacilityCharge.ToArray();
                BuildReservationList(reservations);
            }

        }

        public void LoadReservation(FacilityLot facilityLot)
        {
            _PickingReservationList = null;
            _ProdOrderReservationList = null;

            if (facilityLot != null)
            {
                FacilityReservation[] reservations = facilityLot.FacilityReservation_FacilityLot.ToArray();
                BuildReservationList(reservations);
            }
        }

        public void LoadReservation(Material material, DateTime startTime, DateTime endTime)
        {
            _PickingReservationList = null;
            _ProdOrderReservationList = null;

            if (material != null)
            {
                FacilityReservation[] reservations = material.FacilityReservation_Material.Where(c => c.InsertDate >= startTime && c.InsertDate < endTime).ToArray();
                BuildReservationList(reservations);
            }
        }

        public void ClearReservation()
        {
            _PickingReservationList = null;
            _ProdOrderReservationList = null;
            OnPropertyChanged(nameof(PickingReservationList));
            OnPropertyChanged(nameof(ProdOrderReservationList));
        }


        #endregion


        #region Methods -> Private

        private void BuildReservationList(FacilityReservation[] reservations)
        {
            _PickingReservationList = null;
            _ProdOrderReservationList = null;
            List<FacilityReservation> pickingReservations = reservations.Where(c => c.PickingPos != null).OrderBy(c => c.InsertDate).ToList();

            if (pickingReservations.Any())
            {
                _PickingReservationList = new List<FacilityReservationModel>();
                foreach (FacilityReservation reservation in pickingReservations)
                {
                    FacilityReservationModel facilityReservationModel = ACFacilityManager.GetFacilityReservationModel(reservation);
                    facilityReservationModel.DocumentNo = reservation.PickingPos.Picking.PickingNo;
                    facilityReservationModel.PickingID = reservation.PickingPos.PickingID;
                    _PickingReservationList.Add(facilityReservationModel);
                }
            }

            List<FacilityReservation> prodOrderReservations = reservations.Where(c => c.ProdOrderPartslistPos != null).OrderBy(c => c.InsertDate).ToList();
            if (prodOrderReservations.Any())
            {
                _ProdOrderReservationList = new List<FacilityReservationModel>();
                foreach (FacilityReservation reservation in prodOrderReservations)
                {
                    FacilityReservationModel facilityReservationModel = ACFacilityManager.GetFacilityReservationModel(reservation);
                    facilityReservationModel.DocumentNo = reservation.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                    facilityReservationModel.ProdOrderPartslistID = reservation.ProdOrderPartslistPos.ProdOrderPartslistID;
                    _ProdOrderReservationList.Add(facilityReservationModel);
                }
            }

            OnPropertyChanged(nameof(PickingReservationList));
            OnPropertyChanged(nameof(ProdOrderReservationList));
        }

        #endregion
        #endregion
    }

}
