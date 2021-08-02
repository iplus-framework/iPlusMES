using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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

        public string ProgramNo { get; set; }

        public Guid? ProdOrderPartslistID { get; set; }
        public Guid? ProdOrderPartslistPosID { get; set; }

        public Guid PartslistID { get; set; }

        public GlobalApp.BatchPlanMode? PlanMode { get; set; }

        public BatchSuggestionCommandModeEnum? BatchSuggestionMode { get; set; }

        public int? DurationSecAVG { get; set; }
        public int? StartOffsetSecAVG { get; set; }


        #endregion

        #region Properties -> Other (marked)

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

        [ACPropertyInfo(105, "TargetQuantity", "en{'Required quantity'}de{'Bedarfsmenge'}")]
        public double TargetQuantity { get; set; }


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

        #endregion

        #region Properties -> MDUnit

        [ACPropertyInfo(107, "MDUnit", "en{'MDUnit'}de{'MDUnit'}")]
        public MDUnit MDUnit { get; set; }

        [ACPropertyInfo(999, "BaseMDUnit", "en{'BaseMDUnit'}de{'BaseMDUnit'}")]
        public MDUnit BaseMDUnit { get; set; }

        #endregion

        #region Properties -> Batch

        [ACPropertyInfo(999, "BatchSizeMin", "en{'Min. Batchsize'}de{'Min. Batchgröße'}")]
        public double BatchSizeMin { get; set; }

        [ACPropertyInfo(999, "BatchSizeMax", "en{'Max. Batchsize'}de{'Max. Batchgröße'}")]
        public double BatchSizeMax { get; set; }

        [ACPropertyInfo(999, "BatchSizeStandard", "en{'Standard Batchsize'}de{'Standard Batchgröße'}")]
        public double BatchSizeStandard { get; set; }


        [ACPropertyInfo(999, "PlanModeName", "en{'Batch planning mode'}de{'Batch Planmodus'}")]
        public string PlanModeName { get; set; }

        #endregion


        #endregion

        #region Methods


        public bool IsEqualPartslist(WizardSchedulerPartslist second)
        {
            return
                (ProdOrderPartslistID != null && second.ProdOrderPartslistID != null && ProdOrderPartslistID == second.ProdOrderPartslistID)
                || (PartslistID == second.PartslistID);
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
