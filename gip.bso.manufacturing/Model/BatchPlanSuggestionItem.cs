using gip.core.datamodel;
using System;
using System.ComponentModel;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'BatchPlanSuggestionItem'}de{'BatchPlanSuggestionItem'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class BatchPlanSuggestionItem : EntityBase
    {
        public BatchPlanSuggestionItem(int nr, double batchSize, int batchTargetCount, double totalBatchSize)
        {
            Nr = nr;
            _BatchSize = batchSize;
            _BatchTargetCount = batchTargetCount;
            _TotalBatchSize = totalBatchSize;
            if (totalBatchSize > double.Epsilon && _BatchTargetCount > 0)
            {
                _PrevChanged = Field.BatchTargetCount;
                _LastChanged = Field.TotalBatchSize;
            }
            else if (_BatchSize > double.Epsilon && _BatchTargetCount > 0)
            {
                _PrevChanged = Field.BatchSize;
                _LastChanged = Field.BatchTargetCount;
            }
            else if (totalBatchSize > double.Epsilon)
            {
                _LastChanged = Field.TotalBatchSize;
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
            BatchTargetCount,
            BatchSize,
            TotalBatchSize
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
                    if (_LastChanged != Field.BatchTargetCount)
                        _PrevChanged = _LastChanged;
                    if (_PrevChanged == Field.TotalBatchSize)
                    {
                        if (_TotalBatchSize > double.Epsilon && _BatchTargetCount > 0)
                            _BatchSize = _TotalBatchSize / _BatchTargetCount;
                        else
                            _BatchSize = 0;
                        OnPropertyChanged("BatchSize");
                    }
                    else if (_PrevChanged == Field.BatchSize)
                    {
                        _TotalBatchSize = _BatchSize * _BatchTargetCount;
                        OnPropertyChanged("TotalBatchSize");
                    }
                    _LastChanged = Field.BatchTargetCount;
                }
            }
        }

        double _BatchSize;
        [ACPropertyInfo(300, "BatchSize", "en{'Batch Size'}de{'Batchgröße'}")]
        public double BatchSize 
        {
            get
            {
                return _BatchSize;
            }
            set
            {
                if (SetProperty(ref _BatchSize, value))
                {
                    if (_LastChanged != Field.BatchSize)
                        _PrevChanged = _LastChanged;
                    if (_PrevChanged == Field.TotalBatchSize)
                    {
                        if (_TotalBatchSize > double.Epsilon && _BatchSize > double.Epsilon)
                        {
                            _BatchTargetCount = System.Convert.ToInt32(_TotalBatchSize / _BatchSize);
                        }
                        else
                            _BatchTargetCount = 0;
                        OnPropertyChanged("BatchTargetCount");
                    }
                    else if (_PrevChanged == Field.BatchTargetCount)
                    {
                        _TotalBatchSize = _BatchSize * _BatchTargetCount;
                        OnPropertyChanged("TotalBatchSize");
                    }
                    _LastChanged = Field.BatchSize;
                }
            }
        }

        double _TotalBatchSize;
        [ACPropertyInfo(400, "TotalBatchSize", "en{'Total Size'}de{'Gesamtgröße'}")]
        public double TotalBatchSize
        {
            get
            {
                return _TotalBatchSize;
            }
            set
            {
                if (SetProperty(ref _TotalBatchSize, value))
                {
                    if (_LastChanged != Field.TotalBatchSize)
                        _PrevChanged = _LastChanged;
                    if (_PrevChanged == Field.BatchSize)
                    {
                        if (_TotalBatchSize > double.Epsilon && _BatchSize > double.Epsilon)
                        {
                            _BatchTargetCount = System.Convert.ToInt32(_TotalBatchSize / _BatchSize);
                        }
                        else
                            _BatchTargetCount = 0;
                        OnPropertyChanged("BatchTargetCount");
                    }
                    else if (_PrevChanged == Field.BatchTargetCount)
                    {
                        if (_TotalBatchSize > double.Epsilon && _BatchTargetCount > 0)
                            _BatchSize = _TotalBatchSize / _BatchTargetCount;
                        else
                            _BatchSize = 0;
                        OnPropertyChanged("BatchSize");
                    }
                    _LastChanged = Field.TotalBatchSize;
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

        public Guid? ProdOrderBatchPlanID { get; set; }

        public override string ToString()
        {
            return string.Format(@"[BatchPlanSuggestion] #{0} {1} x {2} = {3}", Nr, BatchTargetCount, BatchSize, TotalBatchSize);
        }
    }
}
