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
using System.Linq;
using System.Windows.Ink;

namespace gip.bso.masterdata
{
    public delegate void ReservationChange();

    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.FacilityReservation, Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOFacilityReservation : ACBSOvb
    {

        public event ReservationChange OnReservationChanged;

        #region const
        public double Const_ZeroQuantityCheckFactor = 0.01;
        #endregion

        #region ctor's
        public BSOFacilityReservation(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _UseBackGroundWorker = new ACPropertyConfigValue<bool>(this, nameof(UseBackGroundWorker), false);
            _IncludedFacilities = new ACPropertyConfigValue<string>(this, nameof(IncludedFacilities), "");
            _ExcludedFacilities = new ACPropertyConfigValue<string>(this, nameof(ExcludedFacilities), "");
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseInit = base.ACInit(startChildMode);
            _ = UseBackGroundWorker;

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _ = IncludedFacilities;
            _ = ExcludedFacilities;

            LoadFilterFacilityLists();

            return baseInit;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            SaveFilterFacilityLists();
            bool baseDeInit = base.ACDeInit(deleteACClassTask);
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;
            return baseDeInit;
        }

        #endregion

        #region Child BSO

        ACChildItem<BSOFacilityExplorer> _BSOFacilityExplorer_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo("BSOFacilityExplorer_Child", typeof(BSOFacilityExplorer))]
        public ACChildItem<BSOFacilityExplorer> BSOFacilityExplorer_Child
        {
            get
            {
                if (_BSOFacilityExplorer_Child == null)
                    _BSOFacilityExplorer_Child = new ACChildItem<BSOFacilityExplorer>(this, "BSOFacilityExplorer_Child");
                return _BSOFacilityExplorer_Child;
            }
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


        protected ACPropertyConfigValue<string> _IncludedFacilities;
        [ACPropertyConfig(ConstApp.FRIncludedFacilities)]
        public string IncludedFacilities
        {
            get
            {
                return _IncludedFacilities.ValueT;
            }
            set
            {
                _IncludedFacilities.ValueT = value;
            }
        }

        protected ACPropertyConfigValue<string> _ExcludedFacilities;
        [ACPropertyConfig(ConstApp.FRExcludedFacilities)]
        public string ExcludedFacilities
        {
            get
            {
                return _ExcludedFacilities.ValueT;
            }
            set
            {
                _ExcludedFacilities.ValueT = value;
            }
        }
        #endregion

        #region Properties
        private bool showLotDialog;

        private IACObjectEntity _FacilityReservationOwner;
        public IACObjectEntity FacilityReservationOwner
        {
            get
            {
                return _FacilityReservationOwner;
            }
            set
            {
                if (_FacilityReservationOwner != null)
                {
                    INotifyPropertyChanged notifyPropertyChanged = _FacilityReservationOwner as INotifyPropertyChanged;
                    notifyPropertyChanged.PropertyChanged -= _FacilityReservationOwner_OnPropertyChanged;
                }
                if (_FacilityReservationOwner != value)
                {
                    _FacilityReservationList = null;
                    OnPropertyChanged(nameof(FacilityReservationList));

                    Material = null;
                    FacilityReservationCollection = null;
                    _FacilityReservationOwner = value;
                    if (_FacilityReservationOwner != null)
                    {
                        SetupMaterial(_FacilityReservationOwner);
                        SetupTargetQuantity(_FacilityReservationOwner);
                        SetupFacilityReservationCollection(_FacilityReservationOwner);

                        if (_FacilityReservationOwner != null)
                        {
                            INotifyPropertyChanged notifyPropertyChanged = _FacilityReservationOwner as INotifyPropertyChanged;
                            notifyPropertyChanged.PropertyChanged += _FacilityReservationOwner_OnPropertyChanged;
                        }
                    }
                    OnPropertyChanged();
                    LoadFacilityReservationOwner();
                }
            }
        }

        private void _FacilityReservationOwner_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IACObjectEntity aCObjectEntity = sender as IACObjectEntity;
            switch (e.PropertyName)
            {
                case nameof(ProdOrderPartslistPos.Material):
                    SetupMaterial(aCObjectEntity);
                    DeleteReservationsOnMaterialChange();
                    break;
                case nameof(ProdOrderPartslistPos.TargetQuantityUOM):
                    SetupTargetQuantity(aCObjectEntity);
                    break;
            }
        }

        public GlobalApp.ReservationState DefaultReservationState { get; set; }


        public Material Material { get; set; }

        public IEnumerable<FacilityReservation> FacilityReservationCollection { get; set; }

        public double TargetQuantityUOM { get; set; }
        public double NeededQuantityUOM { get; set; }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _ForReservationQuantityUOM;
        [ACPropertyInfo(999, nameof(ForReservationQuantityUOM), "en{'Quantity for reservation'}de{'Menge zur Reservierung'}")]
        public double ForReservationQuantityUOM
        {
            get
            {
                return _ForReservationQuantityUOM;
            }
            set
            {
                if (_ForReservationQuantityUOM != value)
                {
                    _ForReservationQuantityUOM = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _EditorReserverdQuantityUOM;
        [ACPropertyInfo(999, nameof(EditorReserverdQuantity), ConstApp.TotalAssignedQuantity)]
        public double EditorReserverdQuantity
        {
            get
            {
                return _EditorReserverdQuantityUOM;
            }
            set
            {
                if (_EditorReserverdQuantityUOM != value)
                {
                    _EditorReserverdQuantityUOM = value;
                    OnPropertyChanged(nameof(EditorReserverdQuantity));
                    OnPropertyChanged(nameof(DiffReservationQuantityUOM));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, nameof(DiffReservationQuantityUOM), "en{'Diff'}de{'Unterschied'}")]
        public double DiffReservationQuantityUOM
        {
            get
            {
                return ForReservationQuantityUOM - EditorReserverdQuantity;
            }
        }

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

        #endregion

        #region FacilityReservation -> Methods

        #region FacilityReservation -> Methods -> ACMehtods

        /// <summary>
        /// Source Property: MethodName
        /// </summary>
        [ACMethodInfo(nameof(AddFacilityReservation), Const.Add, 400, true)]
        public void AddFacilityReservation()
        {
            if (!IsEnabledAddFacilityReservation())
                return;
            double missingQuantity = ACFacilityManager.GetMissingQuantity(NeededQuantityUOM, _FacilityReservationList);
            if (IsNegligibleQuantity(TargetQuantityUOM, NeededQuantityUOM, Const_ZeroQuantityCheckFactor))
            {
                // Error50604 Production component realise complete quantity!
                // Produktionskomponente in kompletter Stückzahl realisieren!
                Messages.Error(this, "Error50604");
            }
            else if (IsNegligibleQuantity(TargetQuantityUOM, missingQuantity, Const_ZeroQuantityCheckFactor))
            {
                // Error50601 Sufficient quantity has already been reserved
                // Es ist bereits eine ausreichende Menge reserviert
                Messages.Error(this, "Error50601");
            }
            else
            {
                ForReservationQuantityUOM = NeededQuantityUOM;
                showLotDialog = true;
                BackgroundWorker.RunWorkerAsync(nameof(AddFacilityReservation));
                ShowDialog(this, DesignNameProgressBar);
            }
        }

        public virtual bool IsEnabledAddFacilityReservation()
        {
            return
                FacilityReservationOwner != null
                && Material != null;
        }

        /// <summary>
        /// Source Property: MethodName
        /// </summary>
        [ACMethodInfo(nameof(RemoveFacilityReservation), Const.Remove, 401, true)]
        public void RemoveFacilityReservation()
        {
            if (!IsEnabledRemoveFacilityReservation())
                return;

            SelectedFacilityReservation.FacilityReservation.DeleteACObject(DatabaseApp, false);
            SelectedFacilityReservation.PropertyChanged -= FacilityReservationModel_PropertyChanged;
            FacilityReservationList.Remove(SelectedFacilityReservation);
            OnPropertyChanged(nameof(FacilityReservationList));
            SelectedFacilityReservation = FacilityReservationList.FirstOrDefault();
            if (SelectedFacilityReservation == null)
            {
                if (OnReservationChanged != null)
                {
                    OnReservationChanged();
                }
            }
        }

        public virtual bool IsEnabledRemoveFacilityReservation()
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
                    List<FacilityReservationModel> facilityReservationModels = GetFacilityReservationList(DatabaseApp, Material != null ? Material.MaterialID : Guid.Empty);
                    LoadFacilityReservationList(facilityReservationModels);
                }
            }
            else
            {
                _FacilityReservationList = null;
                OnPropertyChanged(nameof(FacilityReservationList));
                SelectedFacilityReservation = null;
            }
        }

        private void LoadFacilityReservationList(List<FacilityReservationModel> reservationModels)
        {
            _FacilityReservationList = reservationModels;
            OnPropertyChanged(nameof(FacilityReservationList));
            SelectedFacilityReservation = _FacilityReservationList.FirstOrDefault();
        }

        private List<FacilityReservationModel> GetFacilityReservationList(DatabaseApp databaseApp, Guid materialID)
        {
            List<FacilityCharge> facilityCharges = GetFacilityCharges(databaseApp, materialID, FilterFacilityIncludedList, FilterFacilityExcludedList);

            List<FacilityReservationModel> reservations = new List<FacilityReservationModel>();
            if (FacilityReservationOwner != null && FacilityReservationCollection != null && FacilityReservationCollection.Any())
            {
                List<FacilityReservation> facilityReservations = FacilityReservationCollection.ToList();

                foreach (FacilityReservation facilityReservation in facilityReservations)
                {
                    FacilityReservationModel facilityReservationModel = ACFacilityManager.GetFacilityReservationModel(facilityReservation);
                    FacilityReservationModelBase modelBase = ACFacilityManager.CalcFacilityReservationModelQuantity(DatabaseApp, FacilityReservationOwner, facilityReservationModel, false);
                    facilityReservationModel.CopyFrom(modelBase);
                    facilityReservationModel.PropertyChanged += FacilityReservationModel_PropertyChanged;

                    facilityReservationModel.FacilityNoList = ACFacilityManager.GetFacilityReservationFacilityNos(facilityReservationModel, facilityCharges);

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
            FacilityReservationModel item = sender as FacilityReservationModel;
            switch (e.PropertyName)
            {
                case nameof(FacilityReservationModel.AssignedQuantity):
                    EditorReserverdQuantity = FacilityLotList == null ? 0 : FacilityLotList.Where(c => c.IsSelected).Select(c => c.AssignedQuantity).DefaultIfEmpty().Sum();
                    break;
                case nameof(FacilityReservationModel.IsSelected):
                    if (item.IsSelected)
                    {
                        if (DiffReservationQuantityUOM > 0)
                        {
                            if (item.FreeQuantity >= DiffReservationQuantityUOM)
                            {
                                item.AssignedQuantity = DiffReservationQuantityUOM;
                            }
                            else
                            {
                                item.AssignedQuantity = item.FreeQuantity;
                            }
                        }
                        else
                        {
                            item.AssignedQuantity = 0;
                        }
                    }
                    else
                    {
                        item.AssignedQuantity = 0;
                    }
                    EditorReserverdQuantity = FacilityLotList == null ? 0 : FacilityLotList.Where(c => c.IsSelected).Select(c => c.AssignedQuantity).DefaultIfEmpty().Sum();
                    break;
            }
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
                    .Where(c => c.IsSelected && c.AssignedQuantity > 0)
                    .Sum(c => c.AssignedQuantity);
                if (ForReservationQuantityUOM < reservedQuantity)
                {
                    // Error50603 A larger quantity than required has been reserved! Reserved quantity {0}; Quantity required: {1}
                    // Es wurde eine größere Menge als benötigt reserviert! Reservierte Menge {0}; Erforderliche Menge: {1}
                    Messages.Error(this, "Error50603", false, reservedQuantity, ForReservationQuantityUOM);
                }
                else
                {

                    List<FacilityReservationModel> notSelectedModels = FacilityLotList.Where(c => !c.IsSelected && c.FacilityReservation != null).ToList();

                    List<FacilityReservationModel> inputModels =
                        FacilityLotList
                        .Where(c =>
                            c.IsSelected
                            && c.FacilityReservation == null
                            // 1. why check small quantities - should be enalbed to reserve small quantities
                            // 2. comparation with summary quantity (TargetQuantityUOM) don't have sense
                            // TODO: maybe one another check to avoid some lot reservations for any reason
                            // && !IsNegligibleQuantity(TargetQuantityUOM, c.AssignedQuantity, Const_ZeroQuantityCheckFactor)
                            )
                        .ToList();

                    List<FacilityReservationModel> outputModels = new List<FacilityReservationModel>();

                    foreach (FacilityReservationModel model in inputModels)
                    {
                        FacilityReservationModel facilityReservationModel = ACFacilityManager.GetNewFacilityReservation(DatabaseApp, FacilityReservationOwner, model);
                        outputModels.Add(facilityReservationModel);
                    }

                    if (_FacilityReservationList == null)
                    {
                        _FacilityReservationList = new List<FacilityReservationModel>();
                    }

                    if (notSelectedModels.Any())
                    {
                        foreach (FacilityReservationModel forRemove in notSelectedModels)
                        {
                            _FacilityReservationList.Remove(forRemove);
                            forRemove.FacilityReservation.DeleteACObject(DatabaseApp, false);
                        }
                    }

                    _FacilityReservationList.AddRange(outputModels);
                    _FacilityReservationList = _FacilityReservationList.OrderBy(c => c.FacilityLot.LotNo).ToList();

                    OnPropertyChanged(nameof(FacilityReservationList));

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
                && FacilityLotList.Any()
                && FacilityLotList.Any(c => c.IsSelected)
                && EditorReserverdQuantity > 0;
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
            _FacilityLotList = DoDistributeQuantity(_FacilityLotList, ForReservationQuantityUOM, true);
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

        private List<FacilityReservationModel> LoadFacilityLotList(DatabaseApp databaseApp, Material material, List<FacilityReservationModel> externalReservationList)
        {
            List<FacilityReservationModel> facilityReservations = new List<FacilityReservationModel>();

            List<FacilityCharge> facilityCharges = GetFacilityCharges(databaseApp, material.MaterialID, FilterFacilityIncludedList, FilterFacilityExcludedList);

            foreach (FacilityCharge facilityCharge in facilityCharges)
            {
                FacilityReservationModel facilityReservation = facilityReservations.Where(c => c.FacilityLot.LotNo == facilityCharge.FacilityLot.LotNo).FirstOrDefault();

                if (facilityReservation == null && externalReservationList != null)
                {
                    FacilityReservationModel tempFacilityReservation = externalReservationList.Where(c => c.FacilityLot.LotNo == facilityCharge.FacilityLot.LotNo).FirstOrDefault();
                    if (tempFacilityReservation != null && tempFacilityReservation.FreeQuantity > 0)
                    {
                        facilityReservation = tempFacilityReservation;
                        facilityReservations.Add(facilityReservation);
                    }
                }

                if (facilityReservation == null)
                {
                    facilityReservation = ACFacilityManager.GetFacilityReservationModel(material, facilityCharge.FacilityLot, DefaultReservationState);
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
            }



            foreach (FacilityReservationModel facilityReservation in facilityReservations)
            {
                if (facilityReservation.FacilityReservation == null)
                {
                    // Calculate values  
                    FacilityReservationModelBase modelBase = ACFacilityManager.CalcFacilityReservationModelQuantity(databaseApp, FacilityReservationOwner, facilityReservation, false);
                    facilityReservation.CopyFrom(modelBase);


                    // save original values
                    facilityReservation.OriginalValues = new Dictionary<string, double>
                    {
                        { nameof(FacilityReservationModel.TotalReservedQuantity), facilityReservation.TotalReservedQuantity },
                        { nameof(FacilityReservationModel.UsedQuantity), facilityReservation.UsedQuantity },
                        { nameof(FacilityReservationModel.FreeQuantity), facilityReservation.FreeQuantity }
                    };
                }
                else
                {
                    facilityReservation.SetSelected(true);
                }

                facilityReservation.FacilityNoList = ACFacilityManager.GetFacilityReservationFacilityNos(facilityReservation, facilityCharges);
            }

            double missingQuantity = ACFacilityManager.GetMissingQuantity(NeededQuantityUOM, _FacilityReservationList);
            List<FacilityReservationModel> frForDistribution = facilityReservations.Where(c => c.FacilityReservation == null).ToList();
            DoDistributeQuantity(frForDistribution, missingQuantity, false);
            EditorReserverdQuantity = facilityReservations.Where(c => c.IsSelected).Select(c => c.AssignedQuantity).DefaultIfEmpty().Sum();

            return facilityReservations;
        }

        public List<FacilityReservationModel> DoDistributeQuantity(List<FacilityReservationModel> facilityReservations, double quantity, bool clearOldQuantity)
        {
            if (clearOldQuantity)
            {
                foreach (FacilityReservationModel facilityReservation in facilityReservations)
                {
                    facilityReservation._AssignedQuantity = 0;
                    if (facilityReservation.OriginalValues != null)
                    {
                        foreach (KeyValuePair<string, double> originalValue in facilityReservation.OriginalValues)
                        {
                            switch (originalValue.Key)
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
                        facilityReservation._AssignedQuantity = facilityReservation.FreeQuantity;
                        facilityReservation.TotalReservedQuantity += facilityReservation.AssignedQuantity;
                        if (!facilityReservation.IsOnlyStockMovement)
                        {
                            facilityReservation.FreeQuantity = 0;
                        }
                        restQuantity -= facilityReservation.AssignedQuantity;
                    }
                    else
                    {
                        facilityReservation._AssignedQuantity = restQuantity;
                        facilityReservation.TotalReservedQuantity += facilityReservation.AssignedQuantity;
                        if (!facilityReservation.IsOnlyStockMovement)
                        {
                            facilityReservation.FreeQuantity -= restQuantity;
                        }
                        restQuantity = 0;
                    }
                    facilityReservation.IsSelected = true;
                }
            }
            return facilityReservations;
        }

        private void DoFinishLoadFacilityLotList(List<FacilityReservationModel> facilityReservationModels, bool showDialog)
        {
            foreach (FacilityReservationModel facilityReservationModel in facilityReservationModels)
            {
                facilityReservationModel.PropertyChanged -= FacilityReservationModel_PropertyChanged;
                facilityReservationModel.PropertyChanged += FacilityReservationModel_PropertyChanged;
            }

            _FacilityLotList = facilityReservationModels;
            OnPropertyChanged(nameof(FacilityLotList));
            SelectedFacilityLot = _FacilityLotList.FirstOrDefault();

            if (showDialog)
            {
                ShowDialog(this, "LotDlg");
            }
        }

        #endregion

        #endregion

        #endregion

        #region FilterFacility

        #region FilterFacility -> Included



        public const string FilterFacilityIncluded = "FilterFacilityIncluded";

        private Facility _SelectedFilterFacilityIncluded;
        /// <summary>
        /// Selected property for Facility
        /// </summary>
        /// <value>The selected FilterFacilityIncluded</value>
        [ACPropertySelected(9999, nameof(FilterFacilityIncluded), ConstApp.FRIncludedFacilities)]
        public Facility SelectedFilterFacilityIncluded
        {
            get
            {
                return _SelectedFilterFacilityIncluded;
            }
            set
            {
                if (_SelectedFilterFacilityIncluded != value)
                {
                    _SelectedFilterFacilityIncluded = value;
                    OnPropertyChanged(nameof(SelectedFilterFacilityIncluded));
                }
            }
        }

        private List<Facility> _FilterFacilityIncludedList;
        /// <summary>
        /// List property for Facility
        /// </summary>
        /// <value>The FilterFacilityIncluded list</value>
        [ACPropertyList(9999, nameof(FilterFacilityIncluded))]
        public List<Facility> FilterFacilityIncludedList
        {
            get
            {
                return _FilterFacilityIncludedList;
            }
        }

        #endregion

        #region FilterFacility -> Excluded

        public const string FilterFacilityExcluded = "FilterFacilityExcluded";
        private Facility _SelectedFilterFacilityExcluded;
        /// <summary>
        /// Selected property for Facility
        /// </summary>
        /// <value>The selected FilterFacilityExcluded</value>
        [ACPropertySelected(9999, nameof(FilterFacilityExcluded), ConstApp.FRExcludedFacilities)]
        public Facility SelectedFilterFacilityExcluded
        {
            get
            {
                return _SelectedFilterFacilityExcluded;
            }
            set
            {
                if (_SelectedFilterFacilityExcluded != value)
                {
                    _SelectedFilterFacilityExcluded = value;
                    OnPropertyChanged(nameof(SelectedFilterFacilityExcluded));
                }
            }
        }


        private List<Facility> _FilterFacilityExcludedList;
        /// <summary>
        /// List property for Facility
        /// </summary>
        /// <value>The FilterFacilityExcluded list</value>
        [ACPropertyList(9999, nameof(FilterFacilityExcluded))]
        public List<Facility> FilterFacilityExcludedList
        {
            get
            {
                return _FilterFacilityExcludedList;
            }
        }

        #endregion

        #region FilterFacility -> ACMethod


        /// <summary>
        /// Source Property: AddFilterFacilityIncluded
        /// </summary>
        [ACMethodInfo(nameof(AddFilterFacilityIncluded), Const.Add, 999)]
        public void AddFilterFacilityIncluded()
        {
            if (!IsEnabledAddFilterFacilityIncluded())
                return;

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog();
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;

                if (_FilterFacilityIncludedList == null)
                {
                    _FilterFacilityIncludedList = new List<Facility>();
                }

                if (!_FilterFacilityIncludedList.Contains(facility))
                {
                    _FilterFacilityIncludedList.Add(facility);
                    OnPropertyChanged(nameof(FilterFacilityIncludedList));
                }
            }
        }

        public bool IsEnabledAddFilterFacilityIncluded()
        {
            return true;
        }


        /// <summary>
        /// Source Property: RemoveFilterFacilityIncluded
        /// </summary>
        [ACMethodInfo(nameof(RemoveFilterFacilityIncluded), Const.Remove, 999)]
        public void RemoveFilterFacilityIncluded()
        {
            if (!IsEnabledRemoveFilterFacilityIncluded())
                return;

            FilterFacilityIncludedList.Remove(SelectedFilterFacilityIncluded);
            SelectedFilterFacilityIncluded = FilterFacilityIncludedList.FirstOrDefault();
            OnPropertyChanged(nameof(FilterFacilityIncludedList));
        }

        public bool IsEnabledRemoveFilterFacilityIncluded()
        {
            return SelectedFilterFacilityIncluded != null;
        }


        /// <summary>
        /// Source Property: AddFilterFacilityIncluded
        /// </summary>
        [ACMethodInfo(nameof(AddFilterFacilityExcluded), Const.Add, 999)]
        public void AddFilterFacilityExcluded()
        {
            if (!IsEnabledAddFilterFacilityExcluded())
                return;

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog();
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;

                if (_FilterFacilityExcludedList == null)
                {
                    _FilterFacilityExcludedList = new List<Facility>();
                }

                if (!_FilterFacilityExcludedList.Contains(facility))
                {
                    _FilterFacilityExcludedList.Add(facility);
                    OnPropertyChanged(nameof(FilterFacilityExcludedList));
                    OnPropertyChanged(nameof(SelectedFilterFacilityExcluded));
                }
            }
        }

        public bool IsEnabledAddFilterFacilityExcluded()
        {
            return true;
        }


        /// <summary>
        /// Source Property: RemoveFilterFacilityIncluded
        /// </summary>
        [ACMethodInfo(nameof(RemoveFilterFacilityExcluded), Const.Remove, 999)]
        public void RemoveFilterFacilityExcluded()
        {
            if (!IsEnabledRemoveFilterFacilityExcluded())
                return;

            FilterFacilityExcludedList.Remove(SelectedFilterFacilityExcluded);
            SelectedFilterFacilityExcluded = FilterFacilityExcludedList.FirstOrDefault();
            OnPropertyChanged(nameof(FilterFacilityExcludedList));
        }

        public bool IsEnabledRemoveFilterFacilityExcluded()
        {
            return SelectedFilterFacilityExcluded != null;
        }

        /// <summary>
        /// Source Property: RemoveFilterFacilityIncluded
        /// </summary>
        [ACMethodInfo(nameof(SearchLot), ConstApp.Search, 999)]
        public void SearchLot()
        {
            if (!IsEnabledSearchLot())
                return;

            showLotDialog = false;
            BackgroundWorker.RunWorkerAsync(nameof(AddFacilityReservation));
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledSearchLot()
        {
            return true;
        }


        #endregion

        #region FilterFacility -> Load&Save methods
        private void LoadFilterFacilityLists()
        {
            if (!string.IsNullOrEmpty(IncludedFacilities))
            {
                _FilterFacilityIncludedList = DatabaseApp.Facility.Where(c => IncludedFacilities.Contains(c.FacilityNo)).ToList();
            }

            if (!string.IsNullOrEmpty(ExcludedFacilities))
            {
                _FilterFacilityExcludedList = DatabaseApp.Facility.Where(c => ExcludedFacilities.Contains(c.FacilityNo)).ToList();
            }

            OnPropertyChanged(nameof(FilterFacilityIncludedList));
            OnPropertyChanged(nameof(FilterFacilityExcludedList));
        }

        private void SaveFilterFacilityLists()
        {
            IncludedFacilities = "";
            if (_FilterFacilityIncludedList != null && _FilterFacilityIncludedList.Any())
            {
                IncludedFacilities = string.Join(";", _FilterFacilityIncludedList);
            }

            ExcludedFacilities = "";
            if (_FilterFacilityExcludedList != null && _FilterFacilityExcludedList.Any())
            {
                ExcludedFacilities = string.Join(";", _FilterFacilityExcludedList);
            }

            Root.Database.SaveChanges();
        }

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
                    e.Result = GetFacilityReservationList(DatabaseApp, Material != null ? Material.MaterialID : Guid.Empty);
                    break;
                case nameof(AddFacilityReservation):
                    e.Result = LoadFacilityLotList(DatabaseApp, Material, FacilityReservationList);
                    break;
            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = worker.EventArgs.Argument.ToString();
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
                    case nameof(AddFacilityReservation):
                        List<FacilityReservationModel> reservations = e.Result as List<FacilityReservationModel>;
                        DoFinishLoadFacilityLotList(reservations, showLotDialog);
                        if (OnReservationChanged != null)
                        {
                            OnReservationChanged();
                        }
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

        /// <summary>
        /// Used in case
        /// - change related IACObject or 
        /// - change Material of related IACObject
        /// </summary>
        /// <param name="aCObjectEntity"></param>
        private void SetupMaterial(IACObjectEntity aCObjectEntity)
        {
            if (aCObjectEntity is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos pos = aCObjectEntity as ProdOrderPartslistPos;
                Material = pos.Material;
            }
            else if (aCObjectEntity is PickingPos)
            {
                PickingPos pickingPos = aCObjectEntity as PickingPos;
                Material = pickingPos.Material;
            }
            else if (aCObjectEntity is OutOrderPos)
            {
                OutOrderPos outOrderPos = aCObjectEntity as OutOrderPos;
                Material = outOrderPos.Material;
            }
        }

        private void DeleteReservationsOnMaterialChange()
        {
            if (FacilityReservationList != null && FacilityReservationList.Any())
            {
                foreach (FacilityReservationModel item in FacilityReservationList)
                {
                    item.FacilityReservation.DeleteACObject(DatabaseApp, false);
                }

                _FacilityReservationList = null;
                OnPropertyChanged(nameof(FacilityReservationList));
            }
        }

        /// <summary>
        /// Used in case
        /// - change related IACObject or 
        /// - change TargetQuantityUOM of related IACObject
        /// </summary>
        /// <param name="aCObjectEntity"></param>
        private void SetupTargetQuantity(IACObjectEntity aCObjectEntity)
        {
            if (aCObjectEntity is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos pos = aCObjectEntity as ProdOrderPartslistPos;
                TargetQuantityUOM = pos.TargetQuantityUOM;
                NeededQuantityUOM = pos.TargetQuantityUOM - pos.ActualQuantityUOM;
            }
            else if (aCObjectEntity is PickingPos)
            {
                PickingPos pickingPos = aCObjectEntity as PickingPos;
                TargetQuantityUOM = pickingPos.TargetQuantityUOM;
                NeededQuantityUOM = pickingPos.TargetQuantityUOM - pickingPos.ActualQuantityUOM;
            }
            else if (aCObjectEntity is OutOrderPos)
            {
                OutOrderPos outOrderPos = aCObjectEntity as OutOrderPos;
                TargetQuantityUOM = outOrderPos.TargetQuantityUOM;
                NeededQuantityUOM = outOrderPos.TargetQuantityUOM - outOrderPos.ActualQuantityUOM;
            }
        }

        private void SetupFacilityReservationCollection(IACObjectEntity aCObjectEntity)
        {
            if (aCObjectEntity is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos pos = aCObjectEntity as ProdOrderPartslistPos;
                FacilityReservationCollection = pos.FacilityReservation_ProdOrderPartslistPos.Where(c => c.VBiACClassID == null && c.MaterialID != null);
            }
            else if (aCObjectEntity is PickingPos)
            {
                PickingPos pickingPos = aCObjectEntity as PickingPos;
                FacilityReservationCollection = pickingPos.FacilityReservation_PickingPos.Where(c => c.VBiACClassID == null && c.MaterialID != null);
            }
            else if (aCObjectEntity is OutOrderPos)
            {
                OutOrderPos outOrderPos = aCObjectEntity as OutOrderPos;
                FacilityReservationCollection = outOrderPos.FacilityReservation_OutOrderPos.Where(c => c.VBiACClassID == null && c.MaterialID != null);
            }
        }

        public bool IsNegligibleQuantity(double referentQuantity, double testedQuantity, double factor)
        {
            return (testedQuantity / referentQuantity) < factor;
        }

        private List<FacilityCharge> GetFacilityCharges(DatabaseApp databaseApp, Guid materialID, List<Facility> inlcudedFacilities, List<Facility> excludedFacilities)
        {
            string incl = null;
            if (inlcudedFacilities != null && inlcudedFacilities.Any())
            {
                incl = string.Join(",", inlcudedFacilities.Select(c => c.FacilityNo).Distinct().ToArray());
            }

            string excl = null;
            if (excludedFacilities != null && excludedFacilities.Any())
            {
                excl = string.Join(",", excludedFacilities.Select(c => c.FacilityNo).Distinct().ToArray());
            }

            return
                 databaseApp
               .FacilityCharge
               .Where(c =>
                            (
                                incl == null
                                ||
                                incl.Contains(c.Facility.FacilityNo)
                                || (
                                        c.Facility.Facility1_ParentFacility != null
                                        && incl.Contains(c.Facility.Facility1_ParentFacility.FacilityNo)
                                   ) // L1
                                || (
                                        c.Facility.Facility1_ParentFacility != null
                                        && c.Facility.Facility1_ParentFacility.Facility1_ParentFacility != null
                                        && incl.Contains(c.Facility.Facility1_ParentFacility.Facility1_ParentFacility.FacilityNo)
                                ) // L2
                                || (
                                        c.Facility.Facility1_ParentFacility != null
                                        && c.Facility.Facility1_ParentFacility.Facility1_ParentFacility != null
                                        && c.Facility.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility != null
                                        && incl.Contains(c.Facility.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility.FacilityNo)
                                ) // L3
                            )
                            && 
                            (
                                excl == null 
                                ||
                                !(
                                    excl.Contains(c.Facility.FacilityNo)
                                    || (
                                            c.Facility.Facility1_ParentFacility != null
                                            && excl.Contains(c.Facility.Facility1_ParentFacility.FacilityNo)
                                       ) // L1
                                        || (
                                                c.Facility.Facility1_ParentFacility != null
                                                && c.Facility.Facility1_ParentFacility.Facility1_ParentFacility != null
                                                && excl.Contains(c.Facility.Facility1_ParentFacility.Facility1_ParentFacility.FacilityNo)
                                        ) // L2
                                        || (
                                                c.Facility.Facility1_ParentFacility != null
                                                && c.Facility.Facility1_ParentFacility.Facility1_ParentFacility != null
                                                && c.Facility.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility != null
                                                && excl.Contains(c.Facility.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility.FacilityNo)
                                        ) // L3
                                  )
                            )
                            && c.MaterialID == materialID
                            && !c.NotAvailable
                            && c.FacilityLot != null)
               .ToList();
        }


        #endregion
    }
}