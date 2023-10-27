using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using static gip.mes.datamodel.MDReservationMode;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.FacilityReservation, Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOFacilityReservation : ACBSOvb
    {

        #region const
        public double Const_ZeroQuantityCheckFactor = 0.01;
        #endregion

        #region ctor's
        public BSOFacilityReservation(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _UseBackGroundWorker = new ACPropertyConfigValue<bool>(this, nameof(UseBackGroundWorker), false);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseInit = base.ACInit(startChildMode);
            _ = UseBackGroundWorker;
            return baseInit;
        }

        #endregion

        #region Configuration

        private ACPropertyConfigValue<bool> _UseBackGroundWorker;
        [ACPropertyConfig("en{'Use Background Worker'}de{'Use Background Worker'}")]
        public bool UseBackGroundWorker
        {
            get
            {
                return _UseBackGroundWorker.ValueT;
            }
            set
            {
                _UseBackGroundWorker.ValueT = value;
            }
        }

        #endregion

        #region Properties

        private IACObjectEntity _FacilityReservationOwner;
        public IACObjectEntity FacilityReservationOwner
        {
            get
            {
                return _FacilityReservationOwner;
            }
            set
            {
                if (_FacilityReservationOwner != value)
                {
                    Material = null;
                    FacilityReservationCollection = null;
                    _FacilityReservationOwner = value;
                    if (_FacilityReservationOwner != null)
                    {
                        if (_FacilityReservationOwner is ProdOrderPartslistPos)
                        {
                            ProdOrderPartslistPos pos = _FacilityReservationOwner as ProdOrderPartslistPos;
                            Material = pos.Material;
                            FacilityReservationCollection = pos.FacilityReservation_ProdOrderPartslistPos;
                            TargetQuantityUOM = pos.TargetQuantityUOM;
                            NeededQuantityUOM = pos.TargetQuantityUOM - pos.ActualQuantityUOM;
                        }
                    }
                    OnPropertyChanged();
                    LoadFacilityReservationOwner();
                }
            }
        }

        public Material Material { get; set; }

        public EntityCollection<FacilityReservation> FacilityReservationCollection { get; set; }

        public double TargetQuantityUOM { get; set; }
        public double NeededQuantityUOM { get; set; }
        public double MissingQuantityUOM { get; set; }

        #endregion

        #region FacilityReservation

        #region FacilityReservation -> Properties

        private FacilityReservationModel _SelectedFacilityReservation;
        /// <summary>
        /// Selected property for FacilityChargeSumMaterialHelper
        /// </summary>
        /// <value>The selected FacilityReservation</value>
        [ACPropertySelected(9999, nameof(FacilityReservation), "en{'TODO: FacilityReservation'}de{'TODO: FacilityReservation'}")]
        public FacilityReservationModel SelectedFacilityReservation
        {
            get
            {
                return _SelectedFacilityReservation;
            }
            set
            {
                if (_SelectedFacilityReservation != value)
                {
                    _SelectedFacilityReservation = value;
                    OnPropertyChanged(nameof(SelectedFacilityReservation));
                }
            }
        }

        private List<FacilityReservationModel> _FacilityReservationList;
        /// <summary>
        /// List property for FacilityChargeSumMaterialHelper
        /// </summary>
        /// <value>The FacilityReservation list</value>
        [ACPropertyList(9999, nameof(FacilityReservation))]
        public List<FacilityReservationModel> FacilityReservationList
        {
            get
            {
                return _FacilityReservationList;
            }
        }

        private FacilityReservationModel GetFacilityReservationModel(FacilityReservation facilityReservation)
        {
            FacilityReservationModel facilityReservationModel = new FacilityReservationModel();
            facilityReservationModel.Material = facilityReservation.Material;
            facilityReservationModel.FacilityLot = facilityReservation.FacilityLot;
            facilityReservationModel.FacilityReservation = facilityReservation;
            facilityReservationModel.ReservedQuantity = facilityReservation.ReservedQuantityUOM ?? 0;
            facilityReservationModel.OriginalReservedQuantity = facilityReservation.ReservedQuantityUOM ?? 0;
            return facilityReservationModel;
        }

        private FacilityReservationModel GetFacilityReservationModel(Material material, FacilityLot facilityLot)
        {
            FacilityReservationModel facilityReservationModel = new FacilityReservationModel();
            facilityReservationModel.Material = material;
            facilityReservationModel.FacilityLot = facilityLot;
            return facilityReservationModel;
        }

        #endregion

        #region FacilityReservation -> Methods

        #region FacilityReservation -> Methods -> ACMehtods

        FacilityLot facilityLotForNewReservation = null;
        /// <summary>
        /// Source Property: MethodName
        /// </summary>
        [ACMethodInfo(nameof(AddFaciltiyReservation), Const.Add, 400)]
        public void AddFaciltiyReservation()
        {
            if (!IsEnabledAddFaciltiyReservation())
                return;
            MissingQuantityUOM = GetMissingQuantity(NeededQuantityUOM, _FacilityReservationList);
            if (IsNegligibleQuantity(TargetQuantityUOM, NeededQuantityUOM, Const_ZeroQuantityCheckFactor))
            {
                // Error50604 Production component realise complete quantity!
                // Produktionskomponente in kompletter Stückzahl realisieren!
                Messages.Error(this, "Error50604");
            }
            else if (IsNegligibleQuantity(TargetQuantityUOM, MissingQuantityUOM, Const_ZeroQuantityCheckFactor))
            {
                // Error50601 Sufficient quantity has already been reserved
                // Es ist bereits eine ausreichende Menge reserviert
                Messages.Error(this, "Error50601");
            }
            else
            {
                BackgroundWorker.RunWorkerAsync(nameof(AddFaciltiyReservation));
                ShowDialog(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledAddFaciltiyReservation()
        {
            return FacilityReservationOwner != null;
        }

        /// <summary>
        /// Source Property: MethodName
        /// </summary>
        [ACMethodInfo(nameof(RemoveFacilityReservation), Const.Remove, 401)]
        public void RemoveFacilityReservation()
        {
            if (!IsEnabledRemoveFacilityReservation())
                return;

            SelectedFacilityReservation.FacilityReservation.DeleteACObject(DatabaseApp, false);
            SelectedFacilityReservation.PropertyChanged -= FacilityReservationModel_PropertyChanged;
            FacilityReservationList.Remove(SelectedFacilityReservation);
            OnPropertyChanged(nameof(FacilityReservationList));
            SelectedFacilityReservation = FacilityReservationList.FirstOrDefault();
        }

        public bool IsEnabledRemoveFacilityReservation()
        {
            return SelectedFacilityReservation != null;
        }

        #endregion

        #region FacilityReservation -> Methods -> Helper (private) methods

        private void LoadFacilityReservationOwner()
        {
            _FacilityReservationList = null;
            if (FacilityReservationOwner != null && FacilityReservationCollection != null && FacilityReservationCollection.Any())
            {
                if (UseBackGroundWorker)
                {
                    BackgroundWorker.RunWorkerAsync(nameof(LoadFacilityReservationList));
                    ShowDialog(this, DesignNameProgressBar);
                }
                else
                {
                    LoadFacilityReservationList(null);
                }
            }
            else
            {
                _FacilityReservationList = null;
                OnPropertyChanged(nameof(FacilityReservationList));
                SelectedFacilityReservation = null;
            }
        }

        private void LoadFacilityReservationModelQuantity(FacilityReservationModelBase facilityReservationModelBase)
        {
            SelectedFacilityReservation.CopyFrom(facilityReservationModelBase);
            OnPropertyChanged(nameof(SelectedFacilityReservation));
            if (SelectedFacilityReservation.FreeQuantityNegative)
            {
                Root.Messages.Warning(this, "Warning50069", false, SelectedFacilityReservation.FacilityLot.LotNo);
            }
        }

        private FacilityReservationModelBase CalcFacilityReservationModelQuantity(DatabaseApp databaseApp, FacilityReservationModel model, bool calculateReservedQuantity)
        {
            FacilityReservationModelBase reservationModelBase = new FacilityReservationModelBase();
            reservationModelBase.TotalReservedQuantity =
                databaseApp
                .FacilityReservation
                .Where(FacilityReservation.ProdOrderComponentReservations(model.Material.MaterialNo, model.FacilityLot.LotNo))
                .Select(c => c.ReservedQuantityUOM ?? 0)
                .DefaultIfEmpty()
                .Sum();

            reservationModelBase.UsedQuantity =
                databaseApp
                .FacilityReservation
                .Where(FacilityReservation.ProdOrderComponentReservations(model.Material.MaterialNo, model.FacilityLot.LotNo))
                .Select(c => c.ProdOrderPartslistPos)
                .SelectMany(c => c.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                .SelectMany(c => c.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                .Select(c => c.OutwardQuantity)
                .Sum();

            reservationModelBase.FreeQuantity =
                databaseApp
                .FacilityCharge
                .Where(c =>
                        c.Material != null
                        && c.Material.MaterialNo == model.Material.MaterialNo
                        && c.FacilityLot != null
                        && c.FacilityLot.LotNo == model.FacilityLot.LotNo
                    )
                .AsEnumerable()
                .Select(c => c.AvailableQuantity)
                .DefaultIfEmpty()
                .Sum();

            reservationModelBase.FreeQuantity = reservationModelBase.FreeQuantity - reservationModelBase.TotalReservedQuantity + reservationModelBase.UsedQuantity + (calculateReservedQuantity ? model.ReservedQuantity : 0);
            return reservationModelBase;
        }

        private double GetMissingQuantity(double neededQuantity, List<FacilityReservationModel> reservationModels)
        {
            double missingQuantity = neededQuantity;
            if (missingQuantity > 0)
            {
                if (reservationModels != null)
                {
                    missingQuantity = missingQuantity - reservationModels.Sum(c => c.ReservedQuantity);
                }
            }
            return missingQuantity;
        }

        private void LoadFacilityReservationList(List<FacilityReservationModel> reservationModels)
        {
            if (reservationModels == null)
            {
                reservationModels = GetFacilityReservationList();
            }
            _FacilityReservationList = reservationModels;
            OnPropertyChanged(nameof(FacilityReservationList));
            SelectedFacilityReservation = _FacilityReservationList.FirstOrDefault();
        }

        private List<FacilityReservationModel> GetFacilityReservationList()
        {
            List<FacilityReservationModel> reservations = new List<FacilityReservationModel>();
            if (FacilityReservationOwner != null && FacilityReservationCollection != null && FacilityReservationCollection.Any())
            {
                List<FacilityReservation> facilityReservations = FacilityReservationCollection.ToList();

                foreach (FacilityReservation facilityReservation in facilityReservations)
                {
                    FacilityReservationModel facilityReservationModel = GetFacilityReservationModel(facilityReservation);
                    FacilityReservationModelBase modelBase = CalcFacilityReservationModelQuantity(DatabaseApp, facilityReservationModel, false);
                    facilityReservationModel.CopyFrom(modelBase);
                    facilityReservationModel.PropertyChanged += FacilityReservationModel_PropertyChanged;
                    reservations.Add(facilityReservationModel);
                }
            }
            reservations = reservations.OrderBy(c => c.FacilityLot.LotNo).ToList();
            return reservations;
        }

        private void AlertNegativeReservationQuantity(string lotNo)
        {
            Root.Messages.Warning(this, "Warning50069", false, lotNo);
        }

        private void FacilityReservationModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(FacilityReservationModel.ReservedQuantity):
                    OnPropertyChanged(nameof(SelectedFacilityReservation));
                    break;
            }
        }

        private FacilityReservationModel GetNewFacilityReservation(FacilityReservationModel facilityReservationModel)
        {
            string secondaryKey = ACRoot.SRoot.NoManager.GetNewNo(DatabaseApp, typeof(FacilityReservation), FacilityReservation.NoColumnName, FacilityReservation.FormatNewNo, null);
            FacilityReservation facilityReservation = null;
            if (FacilityReservationOwner is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos pos = FacilityReservationOwner as ProdOrderPartslistPos;
                facilityReservation = FacilityReservation.NewACObject(DatabaseApp, pos, secondaryKey);
                pos.FacilityReservation_ProdOrderPartslistPos.Add(facilityReservation);
            }

            facilityReservation.Material = facilityReservationModel.Material;
            facilityReservation.FacilityLot = facilityReservationModel.FacilityLot;
            facilityReservation.ReservedQuantityUOM = facilityReservationModel.ReservedQuantity;
            facilityReservationModel.FacilityReservation = facilityReservation;

            return facilityReservationModel;
        }

        #endregion


        #endregion

        #endregion

        #region FacilityLot

        #region FacilityLot -> Properties

        private FacilityReservationModel _SelectedFacilityLot;
        /// <summary>
        /// Selected property for FacilityReservationModel
        /// </summary>
        /// <value>The selected FacilityLot</value>
        [ACPropertySelected(9999, "FacilityLot", "en{'TODO: FacilityLot'}de{'TODO: FacilityLot'}")]
        public FacilityReservationModel SelectedFacilityLot
        {
            get
            {
                return _SelectedFacilityLot;
            }
            set
            {
                if (_SelectedFacilityLot != value)
                {
                    _SelectedFacilityLot = value;
                    OnPropertyChanged(nameof(SelectedFacilityLot));
                }
            }
        }


        private List<FacilityReservationModel> _FacilityLotList;
        /// <summary>
        /// List property for FacilityReservationModel
        /// </summary>
        /// <value>The FacilityLot list</value>
        [ACPropertyList(9999, "FacilityLot")]
        public List<FacilityReservationModel> FacilityLotList
        {
            get
            {
                return _FacilityLotList;
            }
        }

        #endregion

        #region FaciltiyLot -> Methods

        #region FaciltiyLot -> Methods -> ACMethods
        /// <summary>
        /// Source Property: ShowLotDlgOk
        /// </summary>
        [ACMethodInfo("ShowLotDlgOk", Const.Ok, 500)]
        public void ShowLotDlgOk()
        {
            if (!IsEnabledShowLotDlgOk())
                return;

            if (FacilityLotList.Any(c => c.IsSelected && c.FreeQuantityNegative))
            {
                // Error50602 A quantity greater than available is reserved for the following lots: {0}
                // Eine größere Menge als verfügbar ist für die folgenden Lose reserviert: {0}
                string[] lotNos = FacilityLotList.Where(c => c.FreeQuantityNegative).Select(c => c.FacilityLot.LotNo).ToArray();
                string lotNosStr = string.Join(",", lotNos);
                Messages.Error(this, "Error50602", false, lotNosStr);
            }
            else
            {
                double reservedQuantity =
                    FacilityLotList
                    .Where(c => c.IsSelected && c.ReservedQuantity > 0)
                    .Sum(c => c.ReservedQuantity);
                if (MissingQuantityUOM < reservedQuantity)
                {
                    // Error50603 A larger quantity than required has been reserved! Reserved quantity {0}; Quantity required: {1}
                    // Es wurde eine größere Menge als benötigt reserviert! Reservierte Menge {0}; Erforderliche Menge: {1}
                    Messages.Error(this, "Error50603", false, reservedQuantity, MissingQuantityUOM);
                }
                else
                {
                    List<FacilityReservationModel> inputModels =
                        FacilityLotList
                        .Where(c =>
                            c.IsSelected
                            && !IsNegligibleQuantity(TargetQuantityUOM, c.ReservedQuantity, Const_ZeroQuantityCheckFactor)
                            )
                        .ToList();

                    List<FacilityReservationModel> outputModels = new List<FacilityReservationModel>();

                    foreach (FacilityReservationModel model in inputModels)
                    {
                        FacilityReservationModel facilityReservationModel = GetNewFacilityReservation(model);
                        outputModels.Add(facilityReservationModel);
                    }

                    if (outputModels.Any())
                    {
                        if (_FacilityReservationList == null)
                        {
                            _FacilityReservationList = new List<FacilityReservationModel>();
                        }
                        _FacilityReservationList.AddRange(outputModels);
                        OnPropertyChanged(nameof(FacilityReservationList));

                    }
                    SelectedFacilityLot = null;
                    _FacilityLotList = null;
                    CloseTopDialog();
                }
            }
        }

        public bool IsEnabledShowLotDlgOk()
        {
            return
                FacilityLotList != null
                && FacilityLotList.Any();
        }

        /// <summary>
        /// Source Property: ShowLotDlgCancel
        /// </summary>
        [ACMethodInfo("ShowLotDlgCancel", Const.Cancel, 501)]
        public void ShowLotDlgCancel()
        {
            if (!IsEnabledShowLotDlgCancel())
                return;
            SelectedFacilityLot = null;
            _FacilityLotList = null;
            CloseTopDialog();
        }

        public bool IsEnabledShowLotDlgCancel()
        {
            return true;
        }

        /// <summary>
        /// Source Property: DistributeQuantity
        /// </summary>
        [ACMethodInfo("DistributeQuantity", "en{'Distribute Quantity'}de{'Menge verteilen'}", 502)]
        public void DistributeQuantity()
        {
            if (!IsEnabledDistributeQuantity())
                return;
            foreach (FacilityReservationModel facilityReservation in FacilityLotList)
            {
                facilityReservation.IsSelected = false;
            }
            _FacilityLotList = DoDistributeQuantity(_FacilityLotList, MissingQuantityUOM, true);
            OnPropertyChanged(nameof(FacilityLotList));
        }

        public bool IsEnabledDistributeQuantity()
        {
            return FacilityLotList != null && FacilityLotList.Any();
        }

        #endregion

        #region FaciltiyLot -> Methods -> Helper (private) methods

        private DateTime? GetFacilityChargeDate(FacilityCharge facilityCharge)
        {
            DateTime? chargeDate = facilityCharge.FillingDate;
            if (chargeDate == null)
            {
                chargeDate = facilityCharge.ProductionDate;
            }
            if (chargeDate == null)
            {
                chargeDate = facilityCharge.InsertDate;
            }
            return chargeDate;
        }

        private List<FacilityReservationModel> LoadFacilityLotList(DatabaseApp databaseApp, Material material, string[] reservedLotNos)
        {
            List<FacilityReservationModel> facilityReservations = new List<FacilityReservationModel>();

            List<FacilityCharge> facilityCharges =
                databaseApp
                .FacilityCharge
                .Where(c => c.MaterialID == material.MaterialID && !c.NotAvailable && c.FacilityLot != null)
                .ToList();

            foreach (FacilityCharge facilityCharge in facilityCharges)
            {
                bool notReserved = FacilityReservationList == null || !FacilityReservationList.Any(c => c.FacilityLot.LotNo == facilityCharge.FacilityLot.LotNo);
                if (notReserved)
                {
                    FacilityReservationModel facilityReservation = facilityReservations.Where(c => c.FacilityLot.LotNo == facilityCharge.FacilityLot.LotNo).FirstOrDefault();
                    if (facilityReservation == null)
                    {
                        facilityReservation = GetFacilityReservationModel(material, facilityCharge.FacilityLot);
                        facilityReservations.Add(facilityReservation);
                    }

                    // Charge date
                    DateTime? chargeDate = GetFacilityChargeDate(facilityCharge);
                    if (chargeDate != null)
                    {
                        if (facilityReservation.OldestFacilityChargeDate == null || facilityReservation.OldestFacilityChargeDate.Value > chargeDate.Value)
                        {
                            facilityReservation.OldestFacilityChargeDate = chargeDate;
                        }
                    }

                    // FacilityNo list
                    if(facilityReservation.FacilityNoList == null)
                    {
                        facilityReservation.FacilityNoList = new List<string>();
                    }
                    if (!facilityReservation.FacilityNoList.Contains(facilityCharge.Facility.FacilityNo))
                    {
                        facilityReservation.FacilityNoList.Add(facilityCharge.Facility.FacilityNo);
                    }
                }
            }

            foreach (FacilityReservationModel facilityReservation in facilityReservations)
            {
                // Calculate values
                FacilityReservationModelBase modelBase = CalcFacilityReservationModelQuantity(databaseApp, facilityReservation, false);
                facilityReservation.CopyFrom(modelBase);

                // save original values
                facilityReservation.OriginalValues = new Dictionary<string, double>
                {
                    { nameof(FacilityReservationModel.TotalReservedQuantity), facilityReservation.TotalReservedQuantity },
                    { nameof(FacilityReservationModel.UsedQuantity), facilityReservation.UsedQuantity },
                    { nameof(FacilityReservationModel.FreeQuantity), facilityReservation.FreeQuantity }
                };
            }

            facilityReservations = DoDistributeQuantity(facilityReservations, MissingQuantityUOM);

            return facilityReservations;
        }

        public List<FacilityReservationModel> DoDistributeQuantity(List<FacilityReservationModel> facilityReservations, double quantity, bool clearOldQuantity = false)
        {
            if (clearOldQuantity)
            {
                foreach(FacilityReservationModel facilityReservation in facilityReservations)
                {
                    facilityReservation._ReservedQuantity = 0;
                    if(facilityReservation.OriginalValues != null)
                    {
                        foreach (KeyValuePair<string, double> originalValue in facilityReservation.OriginalValues)
                        {
                            switch(originalValue.Key)
                            {
                                case nameof(FacilityReservationModel.TotalReservedQuantity):
                                    facilityReservation.TotalReservedQuantity = originalValue.Value;
                                    break;
                                case nameof(FacilityReservationModel.UsedQuantity):
                                    facilityReservation.UsedQuantity = originalValue.Value;
                                    break;
                                case nameof(FacilityReservationModel.FreeQuantity):
                                    facilityReservation.FreeQuantity = originalValue.Value;
                                    break;
                            }
                        }
                    }
                }
            }
            facilityReservations =
                facilityReservations
                .OrderBy(c => c.OldestFacilityChargeDate)
                .ThenBy(c => c.FacilityLot.LotNo)
                .ToList();
            double restQuantity = quantity;
            foreach (FacilityReservationModel facilityReservation in facilityReservations)
            {
                if (IsNegligibleQuantity(quantity, restQuantity, Const_ZeroQuantityCheckFactor))
                {
                    break;
                }
                if (facilityReservation.FreeQuantity > 0 && !IsNegligibleQuantity(quantity, facilityReservation.FreeQuantity, Const_ZeroQuantityCheckFactor))
                {
                    if (restQuantity >= facilityReservation.FreeQuantity)
                    {
                        facilityReservation._ReservedQuantity = facilityReservation.FreeQuantity;
                        facilityReservation.TotalReservedQuantity += facilityReservation.ReservedQuantity;
                        facilityReservation.FreeQuantity = 0;
                        restQuantity -= facilityReservation.ReservedQuantity;
                    }
                    else
                    {
                        facilityReservation._ReservedQuantity = restQuantity;
                        facilityReservation.TotalReservedQuantity += facilityReservation.ReservedQuantity;
                        facilityReservation.FreeQuantity -= restQuantity;
                        restQuantity = 0;
                    }
                    facilityReservation.IsSelected = true;
                }
            }
            return facilityReservations;
        }

        private void DoFinishLoadFacilityLotList(List<FacilityReservationModel> facilityReservationModels)
        {
            foreach (FacilityReservationModel facilityReservationModel in facilityReservationModels)
            {
                facilityReservationModel.PropertyChanged += FacilityReservationModel_PropertyChanged;
            }

            _FacilityLotList = facilityReservationModels;
            OnPropertyChanged(nameof(FacilityLotList));
            SelectedFacilityLot = _FacilityLotList.FirstOrDefault();
            ShowDialog(this, "LotDlg");
        }

        #endregion

        #endregion

        #endregion

        #region BackgroundWorker

        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();

            worker.ProgressInfo.OnlyTotalProgress = true;
            worker.ProgressInfo.AddSubTask(command, 0, 9);
            string message = Translator.GetTranslation("en{'Running {0}...'}de{'{0} läuft...'}");
            worker.ProgressInfo.ReportProgress(command, 0, string.Format(message, command));

            string updateName = Root.Environment.User.Initials;

            switch (command)
            {
                case nameof(LoadFacilityReservationList):
                    e.Result = GetFacilityReservationList();
                    break;
                case nameof(AddFaciltiyReservation):
                    string[] reservedLotNos = new string[] { };
                    if (FacilityReservationList != null)
                    {
                        reservedLotNos = FacilityReservationList.Select(c => c.FacilityLot.LotNo).ToArray();
                    }
                    e.Result = LoadFacilityLotList(DatabaseApp, Material, reservedLotNos);
                    break;
            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = worker.EventArgs.Argument.ToString();
            MsgWithDetails resultMsg = null;
            ClearMessages();
            if (e.Cancelled)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Info, Message = string.Format(@"Operation {0} canceled by user!", command) });
            }
            if (e.Error != null)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format(@"Error by doing {0}! Message:{1}", command, e.Error.Message) });
            }
            else
            {
                switch (command)
                {
                    case nameof(LoadFacilityReservationList):
                        List<FacilityReservationModel> reservationModels = e.Result as List<FacilityReservationModel>;
                        LoadFacilityReservationList(reservationModels);
                        break;
                    case nameof(AddFaciltiyReservation):
                        List<FacilityReservationModel> reservations = e.Result as List<FacilityReservationModel>;
                        DoFinishLoadFacilityLotList(reservations);
                        break;
                }
            }
        }

        #endregion

        #region Message

        public void SendMessage(object result)
        {
            Msg msg = result as Msg;
            if (msg != null)
            {
                SendMessage(msg);
            }
        }

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(528, "Message", "en{'Message'}de{'Meldung'}")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged(nameof(CurrentMsg));
            }
        }

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets the MSG list.
        /// </summary>
        /// <value>The MSG list.</value>
        [ACPropertyList(529, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged(nameof(MsgList));
        }

        public void ClearMessages()
        {
            MsgList.Clear();
            OnPropertyChanged(nameof(MsgList));
        }

        #endregion

        #region Helper methods
        public bool IsNegligibleQuantity(double referentQuantity, double testedQuantity, double factor)
        {
            return (testedQuantity / referentQuantity) < factor;
        }
        #endregion
    }
}
