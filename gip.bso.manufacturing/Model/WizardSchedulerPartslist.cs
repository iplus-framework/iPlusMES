using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
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

        #region ctor's
        public WizardSchedulerPartslist()
        {

        }
        #endregion

        #region Properties

        #region Properties -> Not marked (private)

        public ProdOrderPartslistPos ProdOrderPartslistPos { get; set; }

        private Partslist _Partslist;
        public Partslist Partslist
        {
            get
            {
                return _Partslist;
            }
            set
            {
                _Partslist = value;
                SelectedUnitConvert = UnitConvertList.FirstOrDefault();
                OnPropertyChanged("Partslist");
            }
        }

        public GlobalApp.BatchPlanMode? PlanMode { get; set; }

        public BatchSuggestionCommandModeEnum? BatchSuggestionMode { get; set; }

        public int? DurationSecAVG { get; set; }
        public int? StartOffsetSecAVG { get; set; }


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
                }
            }
        }

        [ACPropertyList(110, "MDSchedulingGroup", "en{'List'}de{'List'}")]
        public List<MDSchedulingGroup> MDSchedulingGroupList { get; set; }

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
                _SelectedUnitConvert = value;
                OnPropertyChanged("SelectedUnitConvert");
            }
        }


        [ACPropertyList(513, "Conversion")]
        public List<MDUnit> UnitConvertList
        {
            get
            {
                if (Partslist == null)
                    return null;
                return Partslist.Material.MDUnitList;
            }
        }

        #endregion

        private double _TargetQuantity;
        [ACPropertyInfo(105, "TargetQuantity", "en{'Required quantity'}de{'Bedarfsmenge'}")]
        public double TargetQuantity
        {
            get
            {
                return _TargetQuantity;
            }
            set
            {
                if (_TargetQuantity != value)
                {
                    _TargetQuantity = value;
                    OnPropertyChanged("TargetQuantity");
                    try
                    {
                        if (Partslist != null && Partslist.Material != null)
                            _TargetQuantityUOM = Partslist.Material.ConvertToBaseQuantity(_TargetQuantity, SelectedUnitConvert);
                        else
                            _TargetQuantityUOM = _TargetQuantity;
                    }
                    catch (Exception ec)
                    {
                        _TargetQuantityUOM = 0;
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;
                        ACRoot.SRoot.Messages.LogException(this.ToString(), "TargetQuantity", msg);
                    }
                    OnPropertyChanged("TargetQuantityUOM");
                }
            }
        }


        private double _TargetQuantityUOM;
        [ACPropertyInfo(106, "TargetQuantityUOM", "en{'Quantity'}de{'Menge'}")]
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
                    _TargetQuantityUOM = value;
                    _NewTargetQuantityUOM = value;
                    OnPropertyChanged("TargetQuantityUOM");
                    OnPropertyChanged("NewTargetQuantityUOM");
                    quantityInChange = false;
                    try
                    {
                        if (Partslist != null && Partslist.Material != null)
                            _TargetQuantity = Partslist.Material.ConvertFromBaseQuantity(_TargetQuantityUOM, SelectedUnitConvert);
                        else
                            _TargetQuantity = _TargetQuantityUOM;
                    }
                    catch (Exception ec)
                    {
                        _TargetQuantityUOM = 0;
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;
                        ACRoot.SRoot.Messages.LogException(this.ToString(), "TargetQuantityUOM", msg);
                    }
                    OnPropertyChanged("TargetQuantity");
                }
            }
        }

        private bool quantityInChange;
        public double _NewTargetQuantityUOM;
        /// <summary>
        /// Doc  NewTargetQuantityUOM
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "NewTargetQuantityUOM", "en{'New quantity'}de{'Menge Neu'}")]
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
                    _NewTargetQuantityUOM = value;
                    OnPropertyChanged("NewTargetQuantityUOM");
                    quantityInChange = false;
                }
            }
        }

        public double? _NewSyncTargetQuantityUOM;
        /// <summary>
        /// Doc  NewSyncTargetQuantityUOM
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "NewSyncTargetQuantityUOM", "en{'New sync. quantity'}de{'Menge Neu Sync.'}")]
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
                    _NewSyncTargetQuantityUOM = value;
                    OnPropertyChanged("NewSyncTargetQuantityUOM");
                    quantityInChange = false;
                }
            }
        }

        public double? _ProductionUnits;
        [ACPropertyInfo(999, "ProductionUnits", "en{'Units of production'}de{'Produktionseinheiten'}")]
        public double? ProductionUnits

        {
            get
            {
                return _ProductionUnits;
            }
            set
            {
                if (_ProductionUnits != value)
                {
                    _ProductionUnits = value;
                    OnPropertyChanged("ProductionUnits");
                }
            }
        }

        #endregion

        #region Properties -> MDUnit

        [ACPropertyInfo(107, "MDUnit", "en{'MDUnit'}de{'MDUnit'}")]
        public MDUnit MDUnit { get; set; }

        [ACPropertyInfo(999, "BaseMDUnit", "en{'BaseMDUnit'}de{'BaseMDUnit'}")]
        public MDUnit BaseMDUnit { get; set; }

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
            set
            {
                if (_BatchSizeMin != value)
                {
                    _BatchSizeMin = value;
                    OnPropertyChanged("BatchSizeMin");
                }

            }
        }

        private double _BatchSizeMinUOM;
        [ACPropertyInfo(999, "BatchSizeMinUOM", "en{'Min. Batchsize (UOM)'}de{'Min. Batchgröße (BOM)'}")]
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
            set
            {
                if (_BatchSizeMax != value)
                {
                    _BatchSizeMax = value;
                    OnPropertyChanged("BatchSizeMax");
                }
            }
        }

        private double _BatchSizeMaxUOM;

        [ACPropertyInfo(999, "BatchSizeMaxUOM", "en{'Max. Batchsize UOM'}de{'Max. Batchgröße (BOM)'}")]
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
            set
            {
                if (_BatchSizeStandard != value)
                {
                    _BatchSizeStandard = value;
                    OnPropertyChanged("BatchSizeStandard");
                }
            }
        }

        private double _BatchSizeStandardUOM;
        [ACPropertyInfo(999, "BatchSizeStandardUOM", "en{'Standard Batchsize (UOM)'}de{'Standard Batchgröße (BOM)'}")]
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


        public bool IsEqualPartslist(WizardSchedulerPartslist second)
        {
            return
                (ProdOrderPartslistPos != null && second.ProdOrderPartslistPos != null && ProdOrderPartslistPos == second.ProdOrderPartslistPos)
                || (Partslist == second.Partslist);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return string.Format(@"ProgramNo: {0}, PartslistNo: {1}, IsSolved:{2}, TargetQuantityUOM: {3}", ProgramNo, PartslistNo, IsSolved, TargetQuantityUOM);
        }

        #endregion

    }
}
