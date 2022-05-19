using gip.core.datamodel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'BatchPlanSuggestion'}de{'BatchPlanSuggestion'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class BatchPlanSuggestion : INotifyPropertyChanged
    {
        #region DI
        public WizardSchedulerPartslist WizardSchedulerPartslist { get; set; }
        #endregion

        #region ctor's

        public BatchPlanSuggestion(WizardSchedulerPartslist wizardSchedulerPartslist)
        {
            WizardSchedulerPartslist = wizardSchedulerPartslist;
            WizardSchedulerPartslist.PropertyChanged += WizardSchedulerPartslist_PropertyChanged;
        }

        private void WizardSchedulerPartslist_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TargetQuantityUOM")
            {
                OnPropertyChanged("BatchSuggestionSumUOM");
                OnPropertyChanged("DifferenceUOM");

                OnPropertyChanged("BatchSuggestionSum");
                OnPropertyChanged("Difference");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _RestQuantityTolerance;
        [ACPropertySelected(200, "RestQuantityTolerance", "en{'Diff tolerance'}de{'Differenztoleranz'}")]
        public double RestQuantityTolerance
        {
            get
            {
                return _RestQuantityTolerance;
            }
            set
            {
                if (_RestQuantityTolerance != value)
                {
                    _RestQuantityTolerance = value;
                    OnPropertyChanged("RestQuantityTolerance");

                    _RestQuantityToleranceUOM = WizardSchedulerPartslist.ConvertQuantity(_RestQuantityTolerance, true);
                    OnPropertyChanged("RestQuantityToleranceUOM");
                }
            }
        }

        private double _RestQuantityToleranceUOM;
        /// <summary>
        /// Doc  MaximalRestQuantity
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(201, "RestQuantityToleranceUOM", "en{'Diff tolerance (UOM)'}de{'Differenztoleranz (BME)'}")]
        public double RestQuantityToleranceUOM
        {
            get
            {
                return _RestQuantityToleranceUOM;
            }
            set
            {
                if (_RestQuantityToleranceUOM != value)
                {
                    _RestQuantityToleranceUOM = value;
                    OnPropertyChanged("RestQuantityToleranceUOM");

                    _RestQuantityTolerance = WizardSchedulerPartslist.ConvertQuantity(_RestQuantityToleranceUOM, false);
                    OnPropertyChanged("RestQuantityTolerance");
                }
            }
        }

        /// <summary>
        /// Doc  BatchSuggestionSum
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(300, "BatchSuggestionSum", "en{'Sum'}de{'Sum'}")]
        public double? BatchSuggestionSum
        {
            get
            {
                if (ItemsList == null || !ItemsList.Any())
                    return null;
                return ItemsList.Select(c => c.TotalBatchSize).DefaultIfEmpty().Sum(c => c);
            }
        }

        /// <summary>
        /// Doc  BatchSuggestionSum
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(301, "BatchSuggestionSumUOM", "en{'Sum (UOM)'}de{'Sum (BME)'}")]
        public double? BatchSuggestionSumUOM
        {
            get
            {
                if (ItemsList == null || !ItemsList.Any())
                    return null;
                return ItemsList.Select(c => c.TotalBatchSizeUOM).DefaultIfEmpty().Sum(c => c);
            }
        }

        [ACPropertyInfo(400, "Difference", "en{'Diff'}de{'Diff'}")]
        public double Difference
        {
            get
            {
                return WizardSchedulerPartslist.TargetQuantity - BatchSuggestionSum ?? 0;
            }
        }

        /// <summary>
        /// Doc  Difference
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(401, "DifferenceUOM", "en{'Diff (UOM)'}de{'Diff (BME)'}")]
        public double DifferenceUOM
        {
            get
            {
                return WizardSchedulerPartslist.GetTargetQuantityUOM() - BatchSuggestionSumUOM ?? 0;
            }
        }

        [ACPropertyInfo(106, "RestNotUsedQuantityUOM", "en{'Not used quantity (UOM)'}de{'Unbenutzte Restmenge (BME)'}")]
        public double RestNotUsedQuantityUOM { get; set; }

        #region Items
        private BatchPlanSuggestionItem _SelectedItems;
        /// <summary>
        /// Selected property for BatchPlanSuggestionItem
        /// </summary>
        /// <value>The selected Items</value>
        [ACPropertySelected(107, "Items", "en{'TODO: Items'}de{'TODO: Items'}")]
        public BatchPlanSuggestionItem SelectedItems
        {
            get
            {
                return _SelectedItems;
            }
            set
            {
                if (_SelectedItems != value)
                {
                    _SelectedItems = value;
                    OnPropertyChanged("SelectedItems");
                }
            }
        }

        private BindingList<BatchPlanSuggestionItem> _ItemsList;


        /// <summary>
        /// List property for BatchPlanSuggestionItem
        /// </summary>
        /// <value>The Items list</value>
        [ACPropertyList(108, "Items")]
        public BindingList<BatchPlanSuggestionItem> ItemsList
        {
            get
            {
                if (_ItemsList == null)
                    _ItemsList = new BindingList<BatchPlanSuggestionItem>();
                return _ItemsList;
            }
            set
            {
                _ItemsList = value;
                OnPropertyChanged("ItemsList");
            }
        }

        public void AddItem(BatchPlanSuggestionItem item)
        {
            ItemsList.Add(item);
            item.PropertyChanged -= Item_PropertyChanged;
            item.PropertyChanged += Item_PropertyChanged;
            OnPropertyChanged("BatchSuggestionSum");
            OnPropertyChanged("Difference");
            OnPropertyChanged("BatchSuggestionSumUOM");
            OnPropertyChanged("DifferenceUOM");
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string[] properties = new string[] { "BatchTargetCount", "TotalBatchSize", "BatchSize" };
            if (properties.Contains(e.PropertyName))
            {
                OnPropertyChanged("BatchSuggestionSum");
                OnPropertyChanged("Difference");
            }
            properties = new string[] { "BatchTargetCount", "TotalBatchSizeUOM", "BatchSizeUOM" };
            if (properties.Contains(e.PropertyName))
            {
                OnPropertyChanged("BatchSuggestionSumUOM");
                OnPropertyChanged("DifferenceUOM");
            }
        }

        public bool IsSuggestionValid()
        {
            if (ItemsList == null || ItemsList.Any(c => c.BatchTargetCount == 0))
                return false;
            double sumSize = ItemsList.Sum(c => c.BatchTargetCount * c.BatchSizeUOM);
            bool sumSizeInTolerance = (sumSize - WizardSchedulerPartslist.GetTargetQuantityUOM()) < RestQuantityToleranceUOM;
            bool batchInRange = false;
            {
                double minBatchSize = ItemsList.Select(c => c.BatchSizeUOM).DefaultIfEmpty().Min();
                double maxBatchSize = ItemsList.Select(c => c.BatchSizeUOM).DefaultIfEmpty().Max();
                batchInRange =
                        WizardSchedulerPartslist.BatchSizeMinUOM <= minBatchSize
                    && (WizardSchedulerPartslist.BatchSizeMaxUOM == 0 || WizardSchedulerPartslist.BatchSizeMaxUOM >= maxBatchSize);
            }
            return sumSizeInTolerance && batchInRange;
        }

        #endregion

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return string.Format(@"{0} / {1}", WizardSchedulerPartslist.GetTargetQuantityUOM(), RestNotUsedQuantityUOM);
        }

        internal void UpdateBatchPlan()
        {
            if (WizardSchedulerPartslist.BatchSuggestionMode != null)
                if (WizardSchedulerPartslist.BatchSuggestionMode == BatchSuggestionCommandModeEnum.KeepEqualBatchSizes)
                {
                    IncreaseBatchCount();
                }
                else if (WizardSchedulerPartslist.BatchSuggestionMode == BatchSuggestionCommandModeEnum.KeepStandardBatchSizeAndDivideRest)
                {
                    if (ItemsList.Count() == 1)
                    {
                        IncreaseBatchCount();
                    }
                    else if (ItemsList.Count() == 2)
                    {
                        BatchPlanSuggestionItem item = ItemsList.FirstOrDefault();
                        if (item != null)
                        {
                            double diffQuantity = WizardSchedulerPartslist.NewTargetQuantityUOM - item.TotalBatchSizeUOM;
                            BatchPlanSuggestionItem secondItem = ItemsList.Skip(1).FirstOrDefault();
                            if (secondItem != null)
                            {
                                secondItem.BatchTargetCount = (int)(diffQuantity / item.BatchSizeUOM);
                            }
                        }
                    }
                }
        }

        private void IncreaseBatchCount()
        {
            BatchPlanSuggestionItem item = ItemsList.FirstOrDefault();
            if (item != null)
            {
                item.BatchSizeUOM = item.BatchSizeUOM;
                item.BatchTargetCount = (int)(WizardSchedulerPartslist.NewTargetQuantityUOM / item.BatchSizeUOM);
            }
        }
    }
}
