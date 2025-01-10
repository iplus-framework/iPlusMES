using System.Runtime.CompilerServices;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace gip.mes.facility
{

    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'WizardSchedulerPartslist'}de{'WizardSchedulerPartslist'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class WizardSchedulerPartslist : INotifyPropertyChanged
    {

        #region event

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region DI

        public DatabaseApp DatabaseApp { get; private set; }
        public ACProdOrderManager ProdOrderManager { get; private set; }
        public ConfigManagerIPlus VarioConfigManager { get; private set; }

        public double RoundingQuantity { get; set; }
        #endregion

        #region ctor's
        private WizardSchedulerPartslist(DatabaseApp databaseApp, ACProdOrderManager prodOrderManager,
            ConfigManagerIPlus configManager, double roundingQuantity)
        {
            DatabaseApp = databaseApp;
            ProdOrderManager = prodOrderManager;
            VarioConfigManager = configManager;
            RoundingQuantity = roundingQuantity;
        }

        public WizardSchedulerPartslist(DatabaseApp databaseApp,
                                        ACProdOrderManager prodOrderManager,
                                        ConfigManagerIPlus configManager,
                                        double roundingQuantity,
                                        Partslist partslist,
                                        double targetQuantityUOM,
                                        int sn,
                                        MDBatchPlanGroup selectedBatchPlanGroup,
                                        List<MDSchedulingGroup> schedulingGroups,
                                        MDSchedulingGroup selectedSchedulingGroup = null)
            : this(databaseApp, prodOrderManager, configManager, roundingQuantity)
        {
            Partslist = partslist;
            PartslistNo = partslist.PartslistNo;
            PartslistName = partslist.PartslistName;
            SelectFirstConversionUnit();
            Sn = sn;
            if (targetQuantityUOM > Double.Epsilon)
            {
                TargetQuantityUOM = targetQuantityUOM;
                if (partslist.MDUnitID.HasValue && partslist.Material.BaseMDUnitID != partslist.MDUnitID)
                    TargetQuantity = partslist.Material.ConvertQuantity(TargetQuantityUOM, partslist.Material.BaseMDUnit, partslist.MDUnit);
            }
            MDSchedulingGroupList = schedulingGroups;
            DefineSelectedSchedulingGroup(selectedSchedulingGroup);
            ProductionUnitsUOM = partslist.ProductionUnits;
            SelectedBatchPlanGroup = selectedBatchPlanGroup;
            PartslistACClassMethod method = this.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
            if (method != null)
            {
                HasRequiredParams = method.MaterialWFACClassMethod.ACClassMethod.HasRequiredParams;
            }
        }

        public WizardSchedulerPartslist(DatabaseApp databaseApp,
                                        ACProdOrderManager prodOrderManager,
                                        ConfigManagerIPlus configManager,
                                        double roundingQuantity,
                                        Partslist partslist,
                                        double targetQuantityUOM,
                                        int sn,
                                        List<MDSchedulingGroup> schedulingGroups,
                                        ProdOrderPartslist prodOrderPartslist)
            : this(databaseApp, prodOrderManager, configManager, roundingQuantity, partslist,
                  targetQuantityUOM, sn, null, schedulingGroups)
        {
            ProgramNo = prodOrderPartslist.ProdOrder.ProgramNo;
            MDProdOrderState = prodOrderPartslist.MDProdOrderState;
            gip.mes.datamodel.ACClassWF tempACClassWFItem = WFNodeMES;

            var materialWFConnection = ProdOrderManager.GetMaterialWFConnection(WFNodeMES, prodOrderPartslist.Partslist.MaterialWFID);
            ProdOrderPartslistPos finalMix = ProdOrderManager.GetIntermediate(prodOrderPartslist, materialWFConnection);

            // Read selected MDSchedulingGroup
            if (finalMix != null)
            {
                if (finalMix.ProdOrderBatchPlan_ProdOrderPartslistPos.Any())
                {
                    ProdOrderBatchPlan bp = finalMix.ProdOrderBatchPlan_ProdOrderPartslistPos.FirstOrDefault();
                    gip.mes.datamodel.ACClassWF vbACClassWf = bp.VBiACClassWF;
                    gip.mes.datamodel.MDSchedulingGroup mDSchedulingGroup = vbACClassWf.MDSchedulingGroupWF_VBiACClassWF.Select(c => c.MDSchedulingGroup).FirstOrDefault();
                    if (mDSchedulingGroup != null)
                        SelectedMDSchedulingGroup = mDSchedulingGroup;

                    MDBatchPlanGroup gr = finalMix.ProdOrderBatchPlan_ProdOrderPartslistPos.Select(c => c.MDBatchPlanGroup).Where(c => c != null).FirstOrDefault();
                    SelectedBatchPlanGroup = gr;

                    LoadAlreadyPlannedBatchPlans(prodOrderPartslist, bp);
                }

                ProdOrderPartslistPos = finalMix;
                if (finalMix.MDUnit == null)
                    SelectedUnitConvert = null;
                else
                    SelectedUnitConvert = finalMix.MDUnit;

            }

            if (SelectedMDSchedulingGroup == null)
            {
                SelectedMDSchedulingGroup = MDSchedulingGroupList.FirstOrDefault();
            }

            // for now only check if exist any defined param
            IsRequiredParamsSolved = prodOrderPartslist.ProdOrderPartslistConfig_ProdOrderPartslist.Any();
        }

        #endregion

        #region Properties

        #region Properties -> Not marked (private)

        public ProdOrderPartslist ProdOrderPartslist { get; set; }
        public ProdOrderPartslistPos ProdOrderPartslistPos { get; set; }

        private Partslist _Partslist;
        [ACPropertyInfo(519, nameof(BatchPlanSuggestion), "en{'BOM'}de{'Stückliste.'}")]
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

        public BatchPlanMode? PlanMode { get; set; }

        public BatchSuggestionCommandModeEnum? BatchSuggestionMode { get; set; }

        public int? DurationSecAVG { get; set; }
        public int? StartOffsetSecAVG { get; set; }

        private BatchPlanSuggestion _BatchPlanSuggestion;
        [ACPropertyInfo(518, nameof(BatchPlanSuggestion))]
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

        [ACPropertyInfo(99, nameof(ProgramNo), "en{'Order Number'}de{'Auftragsnummer'}")]
        public string ProgramNo { get; set; }

        [ACPropertyInfo(100, nameof(Sn), "en{'No'}de{'Nr'}")]
        public int Sn { get; set; }

        [ACPropertyInfo(101, nameof(PartslistNo), "en{'No'}de{'Nr'}")]
        public string PartslistNo { get; set; }

        [ACPropertyInfo(102, nameof(PartslistName), "en{'Bill of material name'}de{'Stückliste Name'}")]
        public string PartslistName { get; set; }

        private bool _IsSolved { get; set; }
        [ACPropertyInfo(103, nameof(IsSolved), "en{'IsSolved'}de{'IsSolved'}")]
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

        private string _AlreadyPlannedBatchPlans { get; set; }
        [ACPropertyInfo(102, nameof(AlreadyPlannedBatchPlans), "en{'Batch planned'}de{'Charge geplant'}")]
        public string AlreadyPlannedBatchPlans
        {
            get
            {
                return _AlreadyPlannedBatchPlans;
            }
            set
            {
                _AlreadyPlannedBatchPlans = value;
            }
        }

        public WizardPlanStatusEnum WizardPlanStatus { get; set; }

        private bool _HasRequiredParams { get; set; }
        [ACPropertyInfo(104, nameof(HasRequiredParams), "en{'Has requiered params'}de{'Hat erforderliche Parameter'}")]
        public bool HasRequiredParams
        {
            get
            {
                return _HasRequiredParams;
            }
            set
            {
                _HasRequiredParams = value;
            }
        }

        private bool _IsRequiredParamsSolved { get; set; }
        [ACPropertyInfo(105, nameof(IsRequiredParamsSolved), "en{'Is required params solved'}de{'Sind die erforderlichen Parameter gelöst?'}")]
        public bool IsRequiredParamsSolved
        {
            get
            {
                return _IsRequiredParamsSolved;
            }
            set
            {
                _IsRequiredParamsSolved = value;
                OnPropertyChanged(nameof(IsRequiredParamsSolved));
            }
        }

        [ACPropertyInfo(107, nameof(ParamState), "en{'Param state'}de{'Parameterstatus'}")]
        public PreferredParamStateEnum ParamState

        {
            get
            {
                PreferredParamStateEnum paramState = PreferredParamStateEnum.ParamsNotRequired;
                if (HasRequiredParams)
                {
                    paramState = IsRequiredParamsSolved ? PreferredParamStateEnum.ParamsRequiredDefined : PreferredParamStateEnum.ParamsRequiredNotDefined;
                }
                return paramState;
            }
        }

        [ACPropertyInfo(108, nameof(ParamStateName), "en{'Param state name'}de{'Parameterstatusname'}")]
        public string ParamStateName
        {
            get
            {
                ACValueItem item = DatabaseApp.PreferredParamStateList.Where(c => (PreferredParamStateEnum)c.Value == ParamState).FirstOrDefault();
                return item.ACCaption;
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
                    _WFNodeMES = null;
                    _WFNode = null;
                    _SelectedMDSchedulingGroup = value;
                    OnPropertyChanged("SelectedMDSchedulingGroup");
                    OnPropertyChanged("WFNodeMES");
                    OnPropertyChanged("WFNode");
                }
            }
        }

        [ACPropertyList(110, "MDSchedulingGroup", "en{'List'}de{'List'}")]
        public List<MDSchedulingGroup> MDSchedulingGroupList { get; set; }

        gip.mes.datamodel.ACClassWF _WFNodeMES;
        [ACPropertyInfo(522, nameof(WFNodeMES), "en{'WFNodeMES'}de{'WFNodeMES'}")]
        public gip.mes.datamodel.ACClassWF WFNodeMES
        {
            get
            {
                if (_WFNodeMES != null)
                    return _WFNodeMES;
                if (this.Partslist == null
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
            set
            {
                _WFNodeMES = value;
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

        #region Properties -> MDBatchPlanGroup


        private MDBatchPlanGroup _SelectedBatchPlanGroup;
        /// <summary>
        /// Selected property for PAScheduleForPWNode
        /// </summary>
        /// <value>The selected FilterConnectedLine</value>
        [ACPropertySelected(210, "BatchPlanGroup", "en{'Batchplan group'}de{'Batchplan Gruppe'}")]
        public MDBatchPlanGroup SelectedBatchPlanGroup
        {
            get
            {
                return _SelectedBatchPlanGroup;
            }
            set
            {
                if (_SelectedBatchPlanGroup != value)
                {
                    _SelectedBatchPlanGroup = value;
                    OnPropertyChanged("SelectedBatchPlanGroup");
                }
            }
        }


        private List<MDBatchPlanGroup> _BatchPlanGroupList;
        /// <summary>
        /// List property for PAScheduleForPWNode
        /// </summary>
        /// <value>The FilterConnectedLine list</value>
        [ACPropertyList(220, "BatchPlanGroup", "en{'Batchplan group'}de{'Batchplan Gruppe'}")]
        public List<MDBatchPlanGroup> BatchPlanGroupList
        {
            get
            {
                if (_BatchPlanGroupList == null)
                    _BatchPlanGroupList = LoadBatchPlanGroupList();
                return _BatchPlanGroupList;
            }
        }

        private List<MDBatchPlanGroup> LoadBatchPlanGroupList()
        {
            return DatabaseApp.MDBatchPlanGroup.OrderBy(c => c.SortIndex).ToList();
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
                return Partslist.Material.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).Select(c => c.ToMDUnit).ToArray();
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
        [ACPropertyInfo(105, nameof(TargetQuantity), "en{'Quantity'}de{'Menge'}")]
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
        [ACPropertyInfo(106, nameof(TargetQuantityUOM), "en{'Quantity (UOM)'}de{'Menge (BME)'}")]
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
        [ACPropertyInfo(999, nameof(NewTargetQuantityUOM), "en{'New (UOM) quantity'}de{'Neu (BME) Menge'}")]
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
        [ACPropertyInfo(999, nameof(NewSyncTargetQuantityUOM), "en{'New (UOM) sync. quntity'}de{'Neu (BME) sync. Menge'}")]
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
        [ACPropertyInfo(999, nameof(NewTargetQuantityUOM), "en{'New quantity'}de{'Neu Menge'}")]
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
        [ACPropertyInfo(999, nameof(NewSyncTargetQuantityUOM), "en{'New sync. quantity'}de{'Neu sync. Menge'}")]
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
        [ACPropertyInfo(999, nameof(ProductionUnits), "en{'Units of production (UOM)'}de{'Produktionseinheiten (BME)'}")]
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

        [ACPropertyInfo(999, nameof(ProductionUnits), "en{'Units of production'}de{'Produktionseinheiten'}")]
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
            if (!ProductionUnitsUOM.HasValue
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
        [ACPropertyInfo(999, ProdOrderBatchPlan.C_BatchSizeMin, "en{'Min. Batchsize'}de{'Min. Batchgröße'}")]
        public double BatchSizeMin
        {
            get
            {
                return _BatchSizeMin;
            }
        }

        private double _BatchSizeMinUOM;
        [ACPropertyInfo(999, nameof(BatchSizeMinUOM), "en{'Min. Batchsize (UOM)'}de{'Min. Batchgröße (BME)'}")]
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

        [ACPropertyInfo(999, ProdOrderBatchPlan.C_BatchSizeMax, "en{'Max. Batchsize'}de{'Max. Batchgröße'}")]
        public double BatchSizeMax
        {
            get
            {
                return _BatchSizeMax;
            }
        }

        private double _BatchSizeMaxUOM;

        [ACPropertyInfo(999, nameof(BatchSizeMaxUOM), "en{'Max. Batchsize UOM'}de{'Max. Batchgröße (BME)'}")]
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
        [ACPropertyInfo(999, nameof(BatchSizeStandard), "en{'Standard Batchsize'}de{'Standard Batchgröße'}")]
        public double BatchSizeStandard
        {
            get
            {
                return _BatchSizeStandard;
            }
        }

        private double _BatchSizeStandardUOM;
        [ACPropertyInfo(999, nameof(BatchSizeStandardUOM), "en{'Standard Batchsize (UOM)'}de{'Standard Batchgröße (BME)'}")]
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


        [ACPropertyInfo(999, nameof(PlanModeName), "en{'Batch planning mode'}de{'Batch Planmodus'}")]
        public string PlanModeName { get; set; }

        [ACPropertyInfo(999, nameof(MDProdOrderState), "en{'MDProdOrderState'}de{'MDProdOrderState'}")]
        public MDProdOrderState MDProdOrderState { get; set; }

        [ACPropertyInfo(999, nameof(OffsetToEndTime), "en{'Duration offset for completion date based scheduling'}de{'Daueroffset zur Fertigstellungszeit-basierten Planung'}")]
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
            OnPropertyChanged(nameof(BatchSizeMin));
            _BatchSizeMax = ConvertQuantity(_BatchSizeMaxUOM, toBaseQuantity: false);
            OnPropertyChanged(nameof(BatchSizeMax));
            _BatchSizeStandard = ConvertQuantity(_BatchSizeStandardUOM, toBaseQuantity: false);
            OnPropertyChanged(nameof(BatchSizeStandard));
        }


        public void LoadBatchSuggestion()
        {
            if (ProdOrderPartslistPos != null && ProdOrderPartslistPos.ProdOrderBatchPlan_ProdOrderPartslistPos.Any())
                LoadExistingBatchSuggestion();
            else
                LoadNewBatchSuggestion();

            if (BatchPlanSuggestion != null && BatchPlanSuggestion.ItemsList == null)
                BatchPlanSuggestion.ItemsList = new BindingList<BatchPlanSuggestionItem>();
        }

        public void LoadExistingBatchSuggestion()
        {
            BatchPlanSuggestion = new BatchPlanSuggestion(this);
            double targetQuantity = GetTargetQuantityUOM();
            // Fix TargetQuantityUOM <> TargetQuantity in case when changed from other context
            if (TargetQuantityUOM != targetQuantity)
            {
                TargetQuantityUOM = targetQuantity;
                ProdOrderPartslistPos.TargetQuantityUOM = targetQuantity;
            }
            BatchPlanSuggestion.RestQuantityToleranceUOM = (ProdOrderManager.TolRemainingCallQ / 100) * ProdOrderPartslistPos.TargetQuantityUOM;
            int nr = 0;
            foreach (ProdOrderBatchPlan batchPlan in ProdOrderPartslistPos.ProdOrderBatchPlan_ProdOrderPartslistPos)
            {
                if (this.WFNodeMES == null
                    || batchPlan.VBiACClassWFID != this.WFNodeMES.ACClassWFID)
                    continue;
                nr++;
                BatchPlanSuggestionItem item = new BatchPlanSuggestionItem(this, nr, batchPlan.BatchSize, batchPlan.BatchTargetCount, batchPlan.TotalSize, batchPlan, false);
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

        public void LoadNewBatchSuggestion(WizardSchedulerPartslist[] alreadyGenerated = null)
        {
            double targetQuantity = GetTargetQuantityUOM();
            if (BatchSuggestionMode != null && BatchSuggestionMode == BatchSuggestionCommandModeEnum.TransferBatchCount)
            {
                LoadTransferBatchCountSuggestion(targetQuantity, alreadyGenerated);
            }
            else
            {
                if (PlanMode != null && PlanMode == BatchPlanMode.UseTotalSize)
                {

                    BatchPlanSuggestion = new BatchPlanSuggestion(this)
                    {
                        RestQuantityToleranceUOM = (ProdOrderManager.TolRemainingCallQ / 100) * targetQuantity,
                        ItemsList = new BindingList<BatchPlanSuggestionItem>()
                    };
                    BatchPlanSuggestion.AddItem(new BatchPlanSuggestionItem(this, 1, targetQuantity, 1, targetQuantity, null, true));
                }
                else if (BatchSuggestionMode != null)
                {
                    BatchSuggestionCommand cmd = new BatchSuggestionCommand(RoundingQuantity, this, BatchSuggestionMode ?? BatchSuggestionCommandModeEnum.KeepEqualBatchSizes, ProdOrderManager.TolRemainingCallQ);
                }
                else
                {
                    BatchPlanSuggestion = new BatchPlanSuggestion(this);
                }
            }

            if (
                 BatchPlanSuggestion != null
              && BatchPlanSuggestion.ItemsList != null
              && BatchPlanSuggestion.ItemsList.Any()
              && OffsetToEndTime != null)
                LoadSuggestionItemExpectedBatchEndTime();
        }

        private void LoadTransferBatchCountSuggestion(double targetQuantity, WizardSchedulerPartslist[] alreadyGenerated = null)
        {
            ProdOrderPartslistPos targetPos = ProdOrderPartslistPos.ProdOrderPartslist.ProdOrderPartslistPos_SourceProdOrderPartslist.FirstOrDefault();
            if (targetPos != null)
            {
                // 1 get from existing plan
                double batchSize = 0;
                int batchCount = 0;
                ProdOrderPartslistPos targetFinalIntermediate  = targetPos.ProdOrderPartslist.FinalIntermediate;
                ProdOrderBatchPlan targetBatchPlan = targetFinalIntermediate.ProdOrderBatchPlan_ProdOrderPartslistPos.FirstOrDefault();
                if (targetBatchPlan != null)
                {
                    batchCount = targetBatchPlan.BatchTargetCount;
                    if (batchCount > 0)
                    {
                        batchSize = targetQuantity / batchCount;
                    }
                }
                else
                {
                    WizardSchedulerPartslist otherWpl = alreadyGenerated.Where(c=>c.ProdOrderPartslistPos != null && c.ProdOrderPartslistPos.ProdOrderPartslistPosID == targetFinalIntermediate.ProdOrderPartslistPosID).FirstOrDefault();
                    if(otherWpl != null && otherWpl.BatchPlanSuggestion != null && otherWpl.BatchPlanSuggestion.ItemsList != null)
                    {
                        BatchPlanSuggestionItem targetSuggestionItem = otherWpl.BatchPlanSuggestion.ItemsList.FirstOrDefault();
                        if(targetSuggestionItem != null)
                        {
                            batchSize = targetSuggestionItem.BatchSizeUOM;
                            batchCount = targetSuggestionItem.BatchTargetCount;
                        }
                    }
                }

                if (batchSize > 0 && batchCount > 0)
                {
                    BatchPlanSuggestion = new BatchPlanSuggestion(this)
                    {
                        RestQuantityToleranceUOM = (ProdOrderManager.TolRemainingCallQ / 100) * targetQuantity,
                        ItemsList = new BindingList<BatchPlanSuggestionItem>()
                    };
                    BatchPlanSuggestion.AddItem(new BatchPlanSuggestionItem(this, 1, batchSize, batchCount, targetQuantity, null, true));
                }
                else
                {
                    // Error50702	Die Zielproduktliste enthält keinen Batchplan!
                    ProdOrderManager.Messages.Error(ProdOrderManager, "Error50702");
                }
            }
            else
            {
                // Error50703	Produkt ist kein Bestandteil einer anderen Produktliste!
                ProdOrderManager.Messages.Error(ProdOrderManager, "Error50703");
            }

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

        public const string C_WFParam_OffsetToEndTime = "OffsetToEndTime";
        public const string C_WFParam_BatchSuggestionMode = "BatchSuggestionMode";
        public const string C_WFParam_PlanMode = "PlanMode";
        public const string C_WFParam_BatchSizeStandard = "BatchSizeStandard";
        public const string C_WFParam_BatchSizeMin = "BatchSizeMin";
        public const string C_WFParam_BatchSizeMax = "BatchSizeMax";

        public void LoadConfiguration()
        {
            Partslist partslist = Partslist;
            core.datamodel.ACClassWF aCClassWF = WFNode;
            if (aCClassWF == null)
                return;
            gip.mes.datamodel.ACClassWF vbACClassWF = WFNodeMES;

            PartslistConfigExtract partslistConfigExtract = new PartslistConfigExtract(VarioConfigManager, ProdOrderManager, partslist, WFNode, WFNodeMES);

            IACConfig batchSizeMin = partslistConfigExtract.GetConfig(ProdOrderBatchPlan.C_BatchSizeMin);
            IACConfig batchSizeMax = partslistConfigExtract.GetConfig(ProdOrderBatchPlan.C_BatchSizeMax);
            IACConfig batchSizeStandard = partslistConfigExtract.GetConfig(ProdOrderBatchPlan.C_BatchSizeStandard);
            IACConfig batchPlanMode = partslistConfigExtract.GetConfig(ProdOrderBatchPlan.C_PlanMode);
            IACConfig batchSuggestionMode = partslistConfigExtract.GetConfig(ProdOrderBatchPlan.C_BatchSuggestionMode);
            IACConfig durationSecAVG = partslistConfigExtract.GetConfig(ProdOrderBatchPlan.C_DurationSecAVG);
            IACConfig startOffsetSecAVG = partslistConfigExtract.GetConfig(ProdOrderBatchPlan.C_StartOffsetSecAVG);
            IACConfig offsetToEndTime = partslistConfigExtract.GetConfig(ProdOrderBatchPlan.C_OffsetToEndTime);

            if (batchSizeMin != null && batchSizeMin.Value != null)
                BatchSizeMinUOM = (double)batchSizeMin.Value;

            if (batchSizeMax != null && batchSizeMax.Value != null)
                BatchSizeMaxUOM = (double)batchSizeMax.Value;

            if (batchSizeStandard != null && batchSizeStandard.Value != null)
                BatchSizeStandardUOM = (double)batchSizeStandard.Value;

            if (batchPlanMode != null && batchPlanMode.Value != null)
            {
                LoadPlanMode((BatchPlanMode)batchPlanMode.Value);
            }
            if (PlanMode == null)
            {
                if (BatchSizeMaxUOM == 0 && BatchSizeMaxUOM == 0 && BatchSizeStandardUOM == 0)
                {
                    LoadPlanMode(BatchPlanMode.UseTotalSize);
                }
                else
                {
                    LoadPlanMode(BatchPlanMode.UseBatchCount);
                }
            }

            LoadBatchPlanSuggestionMode(batchSuggestionMode);

            if (durationSecAVG != null)
            {
                DurationSecAVG = (int)durationSecAVG.Value;
            }

            if (startOffsetSecAVG != null)
            {
                StartOffsetSecAVG = (int)startOffsetSecAVG.Value;
            }

            if (offsetToEndTime != null)
            {
                OffsetToEndTime = (TimeSpan)offsetToEndTime.Value;
            }

            SelectFirstConversionUnit();

        }

        private void LoadBatchPlanSuggestionMode(IACConfig batchSuggestionMode)
        {
            ProdOrderPartslistPos targetPos = null;
            if (ProdOrderPartslistPos != null)
            {
                targetPos = ProdOrderPartslistPos.ProdOrderPartslist.ProdOrderPartslistPos_SourceProdOrderPartslist.FirstOrDefault();
            }
            bool useBatchCountFromTargetPl = targetPos != null && targetPos.BasedOnPartslistPos.KeepBatchCount;

            if (useBatchCountFromTargetPl)
            {
                BatchSuggestionMode = BatchSuggestionCommandModeEnum.TransferBatchCount;
            }
            else
            {
                if (batchSuggestionMode != null && batchSuggestionMode.Value != null)
                {
                    BatchSuggestionMode = (BatchSuggestionCommandModeEnum)batchSuggestionMode.Value;
                }
                if (BatchSuggestionMode == null)
                {
                    BatchSuggestionMode = BatchSuggestionCommandModeEnum.KeepEqualBatchSizes;
                }
            }
        }

        public void LoadPlanMode(BatchPlanMode mode)
        {
            PlanMode = mode;
            PlanModeName = DatabaseApp.BatchPlanModeList.FirstOrDefault(c => ((short)c.Value) == (short)mode).ACCaption;
        }

        #endregion

        #region Methods -> Private
        private void DefineSelectedSchedulingGroup(MDSchedulingGroup selectedSchedulingGroup)
        {
            MDSchedulingGroup firstGroupInList = MDSchedulingGroupList.FirstOrDefault();
            if (
                firstGroupInList != null
                && selectedSchedulingGroup != null
                && MDSchedulingGroupList != null
                && MDSchedulingGroupList.Select(c => c.MDSchedulingGroupID).Contains(selectedSchedulingGroup.MDSchedulingGroupID))
            {
                SelectedMDSchedulingGroup = SortSchedulingGroup(firstGroupInList, selectedSchedulingGroup);
            }
            else
            {
                SelectedMDSchedulingGroup = firstGroupInList;
            }
        }

        private MDSchedulingGroup SortSchedulingGroup(MDSchedulingGroup first, MDSchedulingGroup second)
        {
            MDSchedulingGroup selectedGroup = null;
            int firstOrder = 0;
            int secondOrder = 0;

            IEnumerable<Tuple<int, Guid>> items =
                Partslist
                .PartslistConfig_Partslist
                .Where(c => !string.IsNullOrEmpty(c.LocalConfigACUrl) && c.LocalConfigACUrl.Contains("LineOrderInPlan") && c.VBiACClassWFID != null && c.Value != null)
                .ToArray()
                .Select(c => new Tuple<int, Guid>((int)c.Value, c.VBiACClassWFID.Value))
                .OrderBy(c => c.Item1)
                .ToArray();

            if (items.Any())
            {
                firstOrder = items.Where(c => first.MDSchedulingGroupWF_MDSchedulingGroup.Any(x => x.VBiACClassWFID == c.Item2)).Select(c => c.Item1).FirstOrDefault();
                secondOrder = items.Where(c => second.MDSchedulingGroupWF_MDSchedulingGroup.Any(x => x.VBiACClassWFID == c.Item2)).Select(c => c.Item1).FirstOrDefault();
            }


            // #1 case
            // firstOrder = 0
            // secondOrder = 2

            // #2 case
            // firstOrder = 2
            // secondOrder = 0

            // #3 case
            // firstOrder = 0
            // secondOrder = 0

            // #4 case
            // firstOrder = 1
            // secondOrder = 2

            // #4 case
            // firstOrder = 2
            // secondOrder = 1

            selectedGroup = first;
            if (firstOrder == 0 && secondOrder != 0)
            {
                selectedGroup = second;
            }
            else if (firstOrder == secondOrder || firstOrder > secondOrder)
            {
                selectedGroup = second;
            }

            return selectedGroup;
        }

        private void LoadAlreadyPlannedBatchPlans(ProdOrderPartslist prodOrderPartslist, ProdOrderBatchPlan bp)
        {
            AlreadyPlannedBatchPlans = $"{bp.BatchTargetCount} x {bp.BatchSize.ToString("#0.00")}";
            WizardPlanStatus = WizardPlanStatusEnum.Partial;
            if (prodOrderPartslist.TargetQuantity > 0)
            {
                double diff = Math.Abs(prodOrderPartslist.TargetQuantity - (bp.BatchSize * bp.BatchTargetCount));
                if ((diff / prodOrderPartslist.TargetQuantity) < 0.05)
                {
                    WizardPlanStatus = WizardPlanStatusEnum.Full;
                }
            }
        }
        #endregion

    }
}
