using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.ComponentModel;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'BatchPlanSuggestionItem'}de{'BatchPlanSuggestionItem'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class BatchPlanSuggestionItem : EntityBase
    {
        #region DI
        public WizardSchedulerPartslist WizardSchedulerPartslist { get; private set; }
        #endregion

        public BatchPlanSuggestionItem(WizardSchedulerPartslist wizardSchedulerPartslist, int nr, double batchSize, int batchTargetCount, double totalBatchSize)
        {
            WizardSchedulerPartslist = wizardSchedulerPartslist;
            Nr = nr;
            _BatchSizeUOM = batchSize;
            _BatchSize = wizardSchedulerPartslist.ConvertQuantity(batchSize, false);
            _BatchTargetCount = batchTargetCount;
            _TotalBatchSizeUOM = totalBatchSize;
            _TotalBatchSize = wizardSchedulerPartslist.ConvertQuantity(totalBatchSize, false);
            if (totalBatchSize > double.Epsilon && _BatchTargetCount > 0)
            {
                _PrevChanged = Field.BatchTargetCountUOM;
                _LastChanged = Field.TotalBatchSizeUOM;
            }
            else if (_BatchSizeUOM > double.Epsilon && _BatchTargetCount > 0)
            {
                _PrevChanged = Field.BatchSizeUOM;
                _LastChanged = Field.BatchTargetCountUOM;
            }
            else if (totalBatchSize > double.Epsilon)
            {
                _LastChanged = Field.TotalBatchSizeUOM;
                _PrevChanged = Field.None;
            }
        }

        [ACPropertyInfo(100, "Nr", "en{'Batch-No.'}de{'Batch-Nr.'}")]
        public int Nr { get; set; }

        private Field _LastChanged = Field.None;
        private Field _PrevChanged = Field.None;

        private enum Field
        {
            None,
            BatchTargetCountUOM,
            BatchSizeUOM,
            TotalBatchSizeUOM
        }

        int _BatchTargetCount;
        [ACPropertyInfo(200, "BatchTargetCount", "en{'Target Batch Count'}de{'Soll Batchanzahl'}")]
        public int BatchTargetCount
        {
            get
            {
                return _BatchTargetCount;
            }
            set
            {
                if (SetProperty(ref _BatchTargetCount, value))
                {
                    if (_LastChanged != Field.BatchTargetCountUOM)
                        _PrevChanged = _LastChanged;
                    if (_PrevChanged == Field.TotalBatchSizeUOM)
                    {
                        if (_TotalBatchSizeUOM > double.Epsilon && _BatchTargetCount > 0)
                            _BatchSizeUOM = _TotalBatchSizeUOM / _BatchTargetCount;
                        else
                            _BatchSizeUOM = 0;
                        OnPropertyChanged("BatchSizeUOM");
                        _BatchSize = WizardSchedulerPartslist.ConvertQuantity(_BatchSizeUOM, false);
                        OnPropertyChanged("BatchSize");
                    }
                    else if (_PrevChanged == Field.BatchSizeUOM)
                    {
                        _TotalBatchSizeUOM = _BatchSizeUOM * _BatchTargetCount;
                        OnPropertyChanged("TotalBatchSizeUOM");
                        _TotalBatchSize = _BatchSize * _BatchTargetCount;
                        OnPropertyChanged("TotalBatchSize");
                    }
                    _LastChanged = Field.BatchTargetCountUOM;
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _BatchSize;
        [ACPropertySelected(301, "BatchSize", "en{'Batch Size'}de{'Batchgröße'}")]
        public double BatchSize
        {
            get
            {
                return _BatchSize;
            }
            set
            {
                if (_BatchSize != value)
                    BatchSizeUOM = WizardSchedulerPartslist.ConvertQuantity(value, true);
            }
        }



        double _BatchSizeUOM;
        [ACPropertyInfo(300, "BatchSizeUOM", "en{'Batch Size (UOM)'}de{'Batchgröße (BME)'}")]
        public double BatchSizeUOM
        {
            get
            {
                return _BatchSizeUOM;
            }
            set
            {
                if (SetProperty(ref _BatchSizeUOM, value))
                {
                    _BatchSize = WizardSchedulerPartslist.ConvertQuantity(_BatchSizeUOM, false);
                    OnPropertyChanged("BatchSize");

                    if (_LastChanged != Field.BatchSizeUOM)
                        _PrevChanged = _LastChanged;
                    if (_PrevChanged == Field.TotalBatchSizeUOM)
                    {
                        if (_TotalBatchSizeUOM > double.Epsilon && _BatchSizeUOM > double.Epsilon)
                        {
                            _BatchTargetCount = System.Convert.ToInt32(_TotalBatchSizeUOM / _BatchSizeUOM);
                        }
                        else
                            _BatchTargetCount = 0;
                        OnPropertyChanged("BatchTargetCount");
                    }
                    else if (_PrevChanged == Field.BatchTargetCountUOM)
                    {
                        _TotalBatchSizeUOM = _BatchSizeUOM * _BatchTargetCount;
                        OnPropertyChanged("TotalBatchSizeUOM");
                        _TotalBatchSize = _BatchSize * _BatchTargetCount;
                        OnPropertyChanged("TotalBatchSize");
                    }
                    _LastChanged = Field.BatchSizeUOM;
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _TotalBatchSize;
        [ACPropertySelected(999, "TotalBatchSize", "en{'Total Size'}de{'Gesamtgröße'}")]
        public double TotalBatchSize
        {
            get
            {
                return _TotalBatchSize;
            }
            set
            {
                if (_TotalBatchSize != value)
                    TotalBatchSizeUOM = WizardSchedulerPartslist.ConvertQuantity(value, true);
            }
        }



        double _TotalBatchSizeUOM;
        [ACPropertyInfo(400, "TotalBatchSizeUOM", "en{'Total Size (UOM)'}de{'Gesamtgröße (BME)'}")]
        public double TotalBatchSizeUOM
        {
            get
            {
                return _TotalBatchSizeUOM;
            }
            set
            {
                if (SetProperty(ref _TotalBatchSizeUOM, value))
                {
                    _TotalBatchSize = WizardSchedulerPartslist.ConvertQuantity(_TotalBatchSizeUOM, false);
                    OnPropertyChanged("TotalBatchSize");
                    if (_LastChanged != Field.TotalBatchSizeUOM)
                        _PrevChanged = _LastChanged;
                    if (_PrevChanged == Field.BatchSizeUOM)
                    {
                        if (_TotalBatchSizeUOM > double.Epsilon && _BatchSizeUOM > double.Epsilon)
                        {
                            _BatchTargetCount = System.Convert.ToInt32(_TotalBatchSizeUOM / _BatchSizeUOM);
                        }
                        else
                            _BatchTargetCount = 0;
                        OnPropertyChanged("BatchTargetCount");
                    }
                    else if (_PrevChanged == Field.BatchTargetCountUOM)
                    {
                        if (_TotalBatchSizeUOM > double.Epsilon && _BatchTargetCount > 0)
                        {
                            _BatchSizeUOM = _TotalBatchSizeUOM / _BatchTargetCount;
                            _BatchSize = _TotalBatchSize / _BatchTargetCount;
                        }
                        else
                        {
                            _BatchSizeUOM = 0;
                            _BatchSize = 0;
                        }
                        OnPropertyChanged("BatchSizeUOM");
                        OnPropertyChanged("BatchSize");
                    }
                    _LastChanged = Field.TotalBatchSizeUOM;

                }
            }
        }


        private bool _IsEditable;
        /// <summary>
        /// Doc  IsEditable
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(401, "IsEditable", "en{'IsEditable'}de{'IsEditable'}")]
        public bool IsEditable
        {
            get
            {
                return _IsEditable;
            }
            set
            {
                if (_IsEditable != value)
                {
                    _IsEditable = value;
                    OnPropertyChanged("IsEditable");
                }
            }
        }

        private bool _IsInProduction;
        /// <summary>
        /// Doc  IsEditable
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(402, "IsInProduction", "en{'In Production'}de{'In Arbeit'}")]
        public bool IsInProduction
        {
            get
            {
                return _IsInProduction;
            }
            set
            {
                if (_IsInProduction != value)
                {
                    _IsInProduction = value;
                    OnPropertyChanged("IsInProduction");
                }
            }
        }

        /// <summary>
        /// Doc  IsEditable
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(403, "IsEditableBatchSize", "en{'IsEditableBatchSize'}de{'IsEditableBatchSize'}")]
        public bool IsEditableBatchSize
        {
            get
            {
                return !_IsInProduction && IsEditable;
            }
        }

        private ProdOrderBatchPlan _ProdOrderBatchPlan;
        [ACPropertyInfo(403, "ProdOrderBatchPlan", "en{'ProdOrderBatchPlan'}de{'ProdOrderBatchPlan'}")]
        public ProdOrderBatchPlan ProdOrderBatchPlan
        {
            get
            {
                return _ProdOrderBatchPlan;
            }
            set
            {
                if (_ProdOrderBatchPlan != value)
                {
                    _ProdOrderBatchPlan = value;
                    OnPropertyChanged("ProdOrderBatchPlan");
                }
            }
        }

        [ACPropertyInfo(999, "ExpectedBatchEndTime", "en{'Expected batch end time'}de{'Erwartete Batch-Endzeit'}")]
        public DateTime? ExpectedBatchEndTime { get; set; }


        public override string ToString()
        {
            return string.Format(@"[BatchPlanSuggestion] #{0} {1} x {2} = {3}", Nr, BatchTargetCount, BatchSizeUOM, TotalBatchSize);
        }
    }
}
