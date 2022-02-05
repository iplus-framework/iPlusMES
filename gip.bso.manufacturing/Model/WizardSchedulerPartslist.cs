using System.Runtime.CompilerServices;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace gip.bso.manufacturing
{

    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'SchedulerPartslist'}de{'SchedulerPartslist.'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class WizardSchedulerPartslist : INotifyPropertyChanged
    {

        #region const
        public const string Const_SelectedMDSchedulingGroup = @"SelectedMDSchedulingGroup";
        public const string Const_TargetQuantityUOM = @"TargetQuantityUOM";
        public const string Const_NewTargetQuantityUOM = @"NewTargetQuantityUOM";
        public const string Const_NewSyncTargetQuantityUOM = @"NewSyncTargetQuantityUOM";
        #endregion

        #region event

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region DI

        public DatabaseApp DatabaseApp { get; private set; }
        public ACProdOrderManager ProdOrderManager { get; private set; }

        #endregion


        #region ctor's
        public WizardSchedulerPartslist(DatabaseApp databaseApp, ACProdOrderManager prodOrderManager)
        {
            DatabaseApp = databaseApp;
            ProdOrderManager = prodOrderManager;
        }
        #endregion

        #region Properties

        #region Properties -> Not marked (private)

        public ProdOrderPartslistPos ProdOrderPartslistPos { get; set; }

        private Partslist _Partslist;
        [ACPropertyInfo(519, "BatchPlanSuggestion" , "en{'BOM'}de{'Stückliste.'}")]
        public Partslist Partslist
        {
            get
            {
                return _Partslist;
            }
            set
            {
                _Partslist = value;
                OnPropertyChanged("Partslist");
            }
        }

        public GlobalApp.BatchPlanMode? PlanMode { get; set; }

        public BatchSuggestionCommandModeEnum? BatchSuggestionMode { get; set; }

        public int? DurationSecAVG { get; set; }
        public int? StartOffsetSecAVG { get; set; }

        private BatchPlanSuggestion _BatchPlanSuggestion;
        [ACPropertyInfo(518, "BatchPlanSuggestion")]
        public BatchPlanSuggestion BatchPlanSuggestion
        {
            get
            {
                return _BatchPlanSuggestion;
            }
            set
            {
                _BatchPlanSuggestion = value;
                OnPropertyChanged("BatchPlanSuggestion");
            }
        }
        #endregion

        #region Properties -> Other (marked)

        [ACPropertyInfo(99, "ProgramNo", "en{'Order Number'}de{'Auftragsnummer'}")]
        public string ProgramNo { get; set; }

        [ACPropertyInfo(100, "Sn", "en{'No'}de{'Nr'}")]
        public int Sn { get; set; }

        [ACPropertyInfo(101, "PartslistNo", "en{'No'}de{'Nr'}")]
        public string PartslistNo { get; set; }

        [ACPropertyInfo(102, "PartslistName", "en{'Bill of material name'}de{'Stückliste Name'}")]
        public string PartslistName { get; set; }

        private bool _IsSolved { get; set; }
        [ACPropertyInfo(103, "IsSolved", "en{'IsSolved'}de{'IsSolved'}")]
        public bool IsSolved
        {
            get
            {
                return _IsSolved;
            }
            set
            {
                _IsSolved = value;
            }
        }

        #endregion

        #region Properties -> MDSchedulingGroup

        private MDSchedulingGroup _SelectedMDSchedulingGroup;
        [ACPropertySelected(109, "MDSchedulingGroup", "en{'Line'}de{'Linie'}")]
        public MDSchedulingGroup SelectedMDSchedulingGroup
        {
            get
            {
                return _SelectedMDSchedulingGroup;
            }
            set
            {
                if (_SelectedMDSchedulingGroup != value)
                {
                    _SelectedMDSchedulingGroup = value;
                    OnPropertyChanged("SelectedMDSchedulingGroup");
                    _WFNodeMES = null;
                    _WFNode = null;
                    OnPropertyChanged("WFNodeMES");
                    OnPropertyChanged("WFNode");
                }
            }
        }

        [ACPropertyList(110, "MDSchedulingGroup", "en{'List'}de{'List'}")]
        public List<MDSchedulingGroup> MDSchedulingGroupList { get; set; }

        gip.mes.datamodel.ACClassWF _WFNodeMES;
        public gip.mes.datamodel.ACClassWF WFNodeMES
        {
            get
            {
                if (_WFNodeMES != null)
                    return _WFNodeMES;
                if (   this.Partslist == null
                    || SelectedMDSchedulingGroup == null)
                    return null;
                PartslistACClassMethod method = this.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                if (method == null)
                    return null;
                _WFNodeMES =
                    DatabaseApp
                    .MDSchedulingGroup
                    .Where(c => c.MDSchedulingGroupID == SelectedMDSchedulingGroup.MDSchedulingGroupID)
                    .SelectMany(c => c.MDSchedulingGroupWF_MDSchedulingGroup)
                    .Where(c => c.VBiACClassWF.ACClassMethodID == method.MaterialWFACClassMethod.ACClassMethodID)
                    .Select(c => c.VBiACClassWF)
                    .FirstOrDefault();
                return _WFNodeMES;
            }
        }

        gip.core.datamodel.ACClassWF _WFNode;
        public gip.core.datamodel.ACClassWF WFNode
        {
            get
            {
                if (_WFNode != null)
                    return _WFNode;
                if (WFNodeMES == null)
                    return null;
                _WFNode = WFNodeMES.FromIPlusContext<gip.core.datamodel.ACClassWF>(DatabaseApp.ContextIPlus);
                return _WFNode;
            }
        }

        #endregion

        #region Properties -> Quantities

        #region Properties -> Quantities -> MDUnit

        MDUnit _SelectedUnitConvert;
        [ACPropertySelected(512, "Conversion", "en{'Unit'}de{'Einheit'}")]
        public MDUnit SelectedUnitConvert
        {
            get
            {
                return _SelectedUnitConvert;
            }
            set
            {
                MDUnit prevValue = _SelectedUnitConvert;
                _SelectedUnitConvert = value;
                if (_SelectedUnitConvert != prevValue)
                {
                    OnPropertyChanged("SelectedUnitConvert");
                    RecalcLimitsFromUOM();
                    RecalcDependantUOMFields(true);
                }
            }
        }


        [ACPropertyList(513, "Conversion")]
        public IEnumerable<MDUnit> UnitConvertList
        {
            get
            {
                if (Partslist == null || Partslist.Material == null)
                    return null;
                return Partslist.Material.MaterialUnit_Material.OrderBy(c => c.Multiplier).Select(c => c.ToMDUnit).ToArray();
            }
        }

        public void SelectFirstConversionUnit()
        {
            var unitList = UnitConvertList;
            if (unitList != null)
            {
                SelectedUnitConvert = unitList.FirstOrDefault();
            }
        }

        #endregion

        private double _TargetQuantity;
        [ACPropertyInfo(105, "TargetQuantity", "en{'Quantity'}de{'Menge'}")]
        public double TargetQuantity
        {
            get
            {
                return _TargetQuantity;
            }
            set
            {
                if (_TargetQuantity != value)
                    TargetQuantityUOM = ConvertQuantity(value, toBaseQuantity: true);
            }
        }


        private double _TargetQuantityUOM;
        [ACPropertyInfo(106, "TargetQuantityUOM", "en{'Quantity (UOM)'}de{'Menge (BME)'}")]
        public double TargetQuantityUOM
        {
            get
            {
                return _TargetQuantityUOM;
            }
            set
            {
                if (_TargetQuantityUOM != value)
                {
                    quantityInChange = true;
                    _TargetQuantityUOM = CorrectQuantityWithProductionUnits(value);
                    //_TargetQuantityUOM = value;
                    _NewTargetQuantityUOM = _TargetQuantityUOM;
                    OnPropertyChanged("TargetQuantityUOM");
                    OnPropertyChanged("NewTargetQuantityUOM");
                    quantityInChange = false;
                    RecalcDependantUOMFields(true);
                }
            }
        }


        private bool quantityInChange;
        public double _NewTargetQuantityUOM;
        /// <summary>
        /// Doc  NewTargetQuantityUOM
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "NewTargetQuantityUOM", "en{'New (UOM) quantity'}de{'Neu (BME) Menge'}")]
        public double NewTargetQuantityUOM
        {
            get
            {
                return _NewTargetQuantityUOM;
            }
            set
            {
                if (_NewTargetQuantityUOM != value && !quantityInChange)
                {
                    quantityInChange = true;
                    ChangeNewTargetQuantityUOM(value, true);
                    quantityInChange = false;
                }
            }
        }

        public void ChangeNewTargetQuantityUOM(double value, bool raisePropChanged)
        {
            //_NewTargetQuantityUOM = value;
            _NewTargetQuantityUOM = CorrectQuantityWithProductionUnits(value);
            if (raisePropChanged)
                OnPropertyChanged("NewTargetQuantityUOM");
            _NewTargetQuantity = ConvertQuantity(_NewTargetQuantityUOM, false);
            if (raisePropChanged)
                OnPropertyChanged("NewTargetQuantity");
        }

        public double? _NewSyncTargetQuantityUOM;
        /// <summary>
        /// Doc  NewSyncTargetQuantityUOM
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "NewSyncTargetQuantityUOM", "en{'New (UOM) sync. quntity'}de{'Neu (BME) sync. Menge'}")]
        public double? NewSyncTargetQuantityUOM
        {
            get
            {
                return _NewSyncTargetQuantityUOM;
            }
            set
            {
                if (_NewSyncTargetQuantityUOM != value && !quantityInChange)
                {
                    quantityInChange = true;
                    ChangeNewSyncTargetQuantityUOM(value, true);
                    quantityInChange = false;
                }
            }
        }

        public void ChangeNewSyncTargetQuantityUOM(double? value, bool raisePropChanged)
        {
            if (value.HasValue)
                _NewSyncTargetQuantityUOM = CorrectQuantityWithProductionUnits(value.Value);
            else
                _NewSyncTargetQuantityUOM = null;
            
            if (raisePropChanged)
                OnPropertyChanged("NewSyncTargetQuantityUOM");
            _NewSyncTargetQuantity = ConvertQuantity(_NewSyncTargetQuantityUOM.HasValue ? _NewSyncTargetQuantityUOM.Value : 0.0, false);
            if (raisePropChanged)
                OnPropertyChanged("NewSyncTargetQuantity");
        }


        public double _NewTargetQuantity;
        [ACPropertyInfo(999, "NewTargetQuantityUOM", "en{'New quantity'}de{'Neu Menge'}")]
        public double NewTargetQuantity
        {
            get
            {
                return _NewTargetQuantity;
            }
            set
            {
                if (_NewTargetQuantity != value)
                    NewTargetQuantityUOM = ConvertQuantity(value, true);
            }
        }

        public double? _NewSyncTargetQuantity;
        /// <summary>
        /// Doc  NewSyncTargetQuantityUOM
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "NewSyncTargetQuantityUOM", "en{'New sync. quantity'}de{'Neu sync. Menge'}")]
        public double? NewSyncTargetQuantity
        {
            get
            {
                if (SelectedUnitConvert == null && _NewSyncTargetQuantity.HasValue)
                    _NewSyncTargetQuantity = null;
                return _NewSyncTargetQuantity;
            }
            set
            {
                if (_NewSyncTargetQuantity != value)
                    NewSyncTargetQuantityUOM = ConvertQuantity(value.HasValue ? value.Value : 0.0, true);
            }
        }


        public double? _ProductionUnitsUOM;
        [ACPropertyInfo(999, "ProductionUnits", "en{'Units of production (UOM)'}de{'Produktionseinheiten (BME)'}")]
        public double? ProductionUnitsUOM

        {
            get
            {
                return _ProductionUnitsUOM;
            }
            set
            {
                if (_ProductionUnitsUOM != value)
                {
                    _ProductionUnitsUOM = value;
                    OnPropertyChanged("ProductionUnitsUOM");
                    OnPropertyChanged("ProductionUnits");
                }
            }
        }

        [ACPropertyInfo(999, "ProductionUnits", "en{'Units of production'}de{'Produktionseinheiten'}")]
        public double? ProductionUnits

        {
            get
            {
                if (!ProductionUnitsUOM.HasValue)
                    return null;
                return ConvertQuantity(ProductionUnitsUOM.Value, false);
            }
        }

        public double CorrectQuantityWithProductionUnits(double valueUOM)
        {
            if (   !ProductionUnitsUOM.HasValue 
                || ProductionUnitsUOM.Value < 0.000001
                || valueUOM < 0.000001)
                return valueUOM;
            double rest = valueUOM % ProductionUnitsUOM.Value;
            if (rest >= 0.000001)
                return valueUOM - rest;
            return valueUOM;
        }

        #endregion


        #region Properties -> Batch

        #region Properties -> Batch -> BatchSizes

        private double _BatchSizeMin;
        [ACPropertyInfo(999, "BatchSizeMin", "en{'Min. Batchsize'}de{'Min. Batchgröße'}")]
        public double BatchSizeMin
        {
            get
            {
                return _BatchSizeMin;
            }
        }

        private double _BatchSizeMinUOM;
        [ACPropertyInfo(999, "BatchSizeMinUOM", "en{'Min. Batchsize (UOM)'}de{'Min. Batchgröße (BME)'}")]
        public double BatchSizeMinUOM
        {
            get
            {
                return _BatchSizeMinUOM;
            }
            set
            {
                if (_BatchSizeMinUOM != value)
                {
                    _BatchSizeMinUOM = value;
                    OnPropertyChanged("BatchSizeMinUOM");
                    RecalcLimitsFromUOM();
                }
            }
        }

        private double _BatchSizeMax;

        [ACPropertyInfo(999, "BatchSizeMax", "en{'Max. Batchsize'}de{'Max. Batchgröße'}")]
        public double BatchSizeMax
        {
            get
            {
                return _BatchSizeMax;
            }
        }

        private double _BatchSizeMaxUOM;

        [ACPropertyInfo(999, "BatchSizeMaxUOM", "en{'Max. Batchsize UOM'}de{'Max. Batchgröße (BME)'}")]
        public double BatchSizeMaxUOM
        {
            get
            {
                return _BatchSizeMaxUOM;
            }
            set
            {
                if (_BatchSizeMaxUOM != value)
                {
                    _BatchSizeMaxUOM = value;
                    OnPropertyChanged("BatchSizeMaxUOM");
                    RecalcLimitsFromUOM();
                }
            }
        }

        private double _BatchSizeStandard;
        [ACPropertyInfo(999, "BatchSizeStandard", "en{'Standard Batchsize'}de{'Standard Batchgröße'}")]
        public double BatchSizeStandard
        {
            get
            {
                return _BatchSizeStandard;
            }
        }

        private double _BatchSizeStandardUOM;
        [ACPropertyInfo(999, "BatchSizeStandardUOM", "en{'Standard Batchsize (UOM)'}de{'Standard Batchgröße (BME)'}")]
        public double BatchSizeStandardUOM
        {
            get
            {
                return _BatchSizeStandardUOM;
            }
            set
            {
                if (_BatchSizeStandardUOM != value)
                {
                    _BatchSizeStandardUOM = value;
                    OnPropertyChanged("BatchSizeStandardUOM");
                    RecalcLimitsFromUOM();
                }
            }
        }

        #endregion


        [ACPropertyInfo(999, "PlanModeName", "en{'Batch planning mode'}de{'Batch Planmodus'}")]
        public string PlanModeName { get; set; }

        [ACPropertyInfo(999, "MDProdOrderState", "en{'MDProdOrderState'}de{'MDProdOrderState'}")]
        public MDProdOrderState MDProdOrderState { get; set; }

        [ACPropertyInfo(999, "OffsetToEndTime", "en{'Duration offset for completion date based scheduling'}de{'Daueroffset zur Fertigstellungszeit-basierten Planung'}")]
        public TimeSpan? OffsetToEndTime { get; set; }

        #endregion


        #endregion

        #region Methods

        public double GetTargetQuantityUOM()
        {
            return NewTargetQuantityUOM > 0 ? NewTargetQuantityUOM : TargetQuantityUOM;
        }


        public bool IsEqualPartslist(WizardSchedulerPartslist second)
        {
            return
                (ProdOrderPartslistPos != null && second.ProdOrderPartslistPos != null && ProdOrderPartslistPos == second.ProdOrderPartslistPos)
                || (Partslist == second.Partslist);
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return string.Format(@"ProgramNo: {0}, PartslistNo: {1}, IsSolved:{2}, TargetQuantityUOM: {3}", ProgramNo, PartslistNo, IsSolved, TargetQuantityUOM);
        }

        public double ConvertQuantity(double sourceQuantity, bool toBaseQuantity)
        {
            double toQuantity = 0;
            try
            {
                if (Partslist != null && Partslist.Material != null && SelectedUnitConvert != null)
                {
                    if (toBaseQuantity)
                        toQuantity = Partslist.Material.ConvertToBaseQuantity(sourceQuantity, SelectedUnitConvert);
                    else
                        toQuantity = Partslist.Material.ConvertFromBaseQuantity(sourceQuantity, SelectedUnitConvert);
                }
                else
                {
                    if (toBaseQuantity)
                        toQuantity = sourceQuantity;
                    else
                        toQuantity = 0.0;
                }
            }
            catch (Exception ec)
            {
                toQuantity = 0;
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;
                ACRoot.SRoot.Messages.LogException(this.ToString(), "ConvertQuantity", msg);
            }
            return toQuantity;
        }

        protected void RecalcDependantUOMFields(bool uomFieldChanged)
        {
            if (uomFieldChanged)
            {
                _TargetQuantity = ConvertQuantity(_TargetQuantityUOM, toBaseQuantity: false);
                OnPropertyChanged("TargetQuantity");
                _NewTargetQuantity = _TargetQuantity;
                OnPropertyChanged("NewTargetQuantity");
            }
            else
            {
                _TargetQuantityUOM = ConvertQuantity(_TargetQuantity, toBaseQuantity: true);
                OnPropertyChanged("TargetQuantityUOM");
                _NewTargetQuantityUOM = _TargetQuantityUOM;
                OnPropertyChanged("NewTargetQuantityUOM");
            }
        }

        protected void RecalcLimitsFromUOM()
        {
            _BatchSizeMin = ConvertQuantity(_BatchSizeMinUOM, toBaseQuantity: false);
            OnPropertyChanged("BatchSizeMin");
            _BatchSizeMax = ConvertQuantity(_BatchSizeMaxUOM, toBaseQuantity: false);
            OnPropertyChanged("BatchSizeMax");
            _BatchSizeStandard = ConvertQuantity(_BatchSizeStandardUOM, toBaseQuantity: false);
            OnPropertyChanged("BatchSizeStandard");
        }


        public void LoadExistingBatchSuggestion()
        {
            BatchPlanSuggestion = new BatchPlanSuggestion(this);
            double targetQuantity = TargetQuantityUOM;
            if (NewTargetQuantityUOM > 0)
                targetQuantity = NewTargetQuantityUOM;
            TargetQuantityUOM = targetQuantity;
            ProdOrderPartslistPos.TargetQuantityUOM = targetQuantity;
            BatchPlanSuggestion.RestQuantityToleranceUOM = (ProdOrderManager.TolRemainingCallQ / 100) * ProdOrderPartslistPos.TargetQuantityUOM;
            int nr = 0;
            foreach (ProdOrderBatchPlan batchPlan in ProdOrderPartslistPos.ProdOrderBatchPlan_ProdOrderPartslistPos)
            {
                if (   this.WFNodeMES == null 
                    || batchPlan.VBiACClassWFID != this.WFNodeMES.ACClassWFID)
                    continue;
                nr++;
                BatchPlanSuggestionItem item = new BatchPlanSuggestionItem(this, nr, batchPlan.BatchSize, batchPlan.BatchTargetCount, batchPlan.TotalSize);
                item.ProdOrderBatchPlan = batchPlan;
                item.ExpectedBatchEndTime = batchPlan.ScheduledEndDate;
                item.IsEditable =
                    (
                        ProdOrderPartslistPos == null ||
                        (ProdOrderPartslistPos.ProdOrderPartslist.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.ProdFinished &&
                        ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.ProdFinished)
                    )
                    &&
                    !(batchPlan.PlanState >= GlobalApp.BatchPlanState.Completed);
                item.IsInProduction =
                    batchPlan.PlanState >= GlobalApp.BatchPlanState.ReadyToStart
                    && batchPlan.PlanState <= GlobalApp.BatchPlanState.Paused;
                BatchPlanSuggestion.AddItem(item);
            }
        }

        public void LoadNewBatchSuggestion(BatchSuggestionCommandModeEnum? suggestionMode)
        {
            if (PlanMode != null && PlanMode == GlobalApp.BatchPlanMode.UseTotalSize)
            {
                double targetQuantity = TargetQuantityUOM;
                if (NewTargetQuantityUOM > 0)
                    targetQuantity = NewTargetQuantityUOM;
                BatchPlanSuggestion = new BatchPlanSuggestion(this)
                {
                    RestQuantityToleranceUOM = (ProdOrderManager.TolRemainingCallQ / 100) * targetQuantity,
                    ItemsList = new BindingList<BatchPlanSuggestionItem>()
                };
                BatchPlanSuggestion.AddItem(new BatchPlanSuggestionItem(
                    this,
                    1,
                    targetQuantity,
                    1,
                    targetQuantity
                    )
                { IsEditable = true });
            }
            else
            {
                BatchSuggestionCommand cmd = new BatchSuggestionCommand(this, suggestionMode ?? BatchSuggestionCommandModeEnum.KeepEqualBatchSizes, ProdOrderManager.TolRemainingCallQ);
            }
            if (
              BatchPlanSuggestion.ItemsList != null
              && BatchPlanSuggestion.ItemsList.Any()
              && OffsetToEndTime != null)
                LoadSuggestionItemExpectedBatchEndTime();
        }

        public void LoadSuggestionItemExpectedBatchEndTime()
        {
            double totalMinutes = 0;
            if (OffsetToEndTime.Value.TotalMinutes > 0)
                totalMinutes = OffsetToEndTime.Value.TotalMinutes;
            else
            {
                TimeSpan? calculatedDuration = GetExpectedBatchEndTime();
                if (calculatedDuration != null)
                    totalMinutes = calculatedDuration.Value.TotalMinutes;
            }

            DateTime tempTime = DateTime.Now;
            if (totalMinutes > 0)
            {
                foreach (var item in BatchPlanSuggestion.ItemsList)
                {
                    if (item.ExpectedBatchEndTime != null)
                        tempTime = item.ExpectedBatchEndTime.Value;
                    else
                    {
                        tempTime = tempTime.AddMinutes(OffsetToEndTime.Value.TotalMinutes);
                        item.ExpectedBatchEndTime = tempTime;
                    }
                }
            }
        }

        private TimeSpan? GetExpectedBatchEndTime()
        {
            gip.mes.datamodel.ACClassWF vbACClassWF = WFNodeMES;
            var materialWFConnection = ProdOrderManager.GetMaterialWFConnection(vbACClassWF, Partslist.MaterialWFID);
            return ProdOrderManager.GetCalculatedBatchPlanDuration(DatabaseApp, materialWFConnection.MaterialWFACClassMethodID, vbACClassWF.ACClassWFID);
        }

        #endregion

    }
}
