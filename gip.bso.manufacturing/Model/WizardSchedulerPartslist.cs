using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace gip.bso.manufacturing
{

    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'SchedulerPartslist'}de{'SchedulerPartslist.'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class WizardSchedulerPartslist: INotifyPropertyChanged
    {

        #region ctor's
        public WizardSchedulerPartslist()
        {

        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        public string ProgramNo { get; set; }
        public Guid? ProdOrderPartslistID { get; set; }
        public Guid PartslistID { get; set; }

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
                if(_SelectedMDSchedulingGroup != value)
                {
                    _SelectedMDSchedulingGroup = value;
                    OnPropertyNotifyChaged("SelectedMDSchedulingGroup");
                }
            }
        }

        [ACPropertyList(110, "MDSchedulingGroup", "en{'List'}de{'List'}")]
        public List<MDSchedulingGroup> MDSchedulingGroupList { get; set; }

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

        [ACPropertyInfo(105, "TargetQuantity", "en{'Required quantity'}de{'Bedarfsmenge'}")]
        public double TargetQuantity { get; set; }


        [ACPropertyInfo(106, "TargetQuantityUOM", "en{'Required quantity (UOM)'}de{'Bedarfsmenge (UOM)'}")]
        public double TargetQuantityUOM { get; set; }

        [ACPropertyInfo(107, "MDUnit", "en{'MDUnit'}de{'MDUnit'}")]
        public MDUnit MDUnit { get; set; }

        [ACPropertyInfo(999, "BaseMDUnit", "en{'BaseMDUnit'}de{'BaseMDUnit'}")]
        public MDUnit BaseMDUnit { get; set; }

        public void OnPropertyNotifyChaged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [ACPropertyInfo(999, "BatchSizeMin", "en{'Min. Batchsize'}de{'Min. Batchgröße'}")]
        public double BatchSizeMin { get; set; }

        [ACPropertyInfo(999, "BatchSizeMax", "en{'Max. Batchsize'}de{'Max. Batchgröße'}")]
        public double BatchSizeMax { get; set; }

        [ACPropertyInfo(999, "BatchSizeStandard", "en{'Standard Batchsize'}de{'Standard Batchgröße'}")]
        public double BatchSizeStandard { get; set; }


        [ACPropertyInfo(999, "PlanModeName", "en{'Batch planning mode'}de{'Batch Planmodus'}")]
        public string PlanModeName { get; set; }

        public GlobalApp.BatchPlanMode? PlanMode { get; set; }

        public BatchSuggestionCommandModeEnum? BatchSuggestionMode { get; set; }

        public int? DurationSecAVG { get; set; }
        public int? StartOffsetSecAVG { get; set; }

        public override string ToString()
        {
            return string.Format(@"ProgramNo: {0}, PartslistNo: {1}, IsSolved:{2}, TargetQuantityUOM: {3}", ProgramNo, PartslistNo, IsSolved, TargetQuantityUOM);
        }

    }
}
