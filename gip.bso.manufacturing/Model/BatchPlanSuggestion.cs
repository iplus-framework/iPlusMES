using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.ComponentModel;
using System.Linq;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'BatchPlanSuggestion'}de{'BatchPlanSuggestion'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class BatchPlanSuggestion : INotifyPropertyChanged
    {
        #region ctor's

        public BatchPlanSuggestion()
        {
        }

        #endregion

        #region Properties
        public ProdOrderPartslistPos Intermediate { get; set; }

        private double _TotalSize;
        [ACPropertyInfo(100, "TotalBatchSize", "en{'Total Size'}de{'Gesamtgröße'}")]
        public double TotalSize
        {
            get
            {
                return _TotalSize;
            }
            set
            {
                if (_TotalSize != value)
                {
                    _TotalSize = value;
                    OnPropertyChanged("TotalSize");

                    OnPropertyChanged("BatchSuggestionSum");
                    OnPropertyChanged("Difference");
                }
            }
        }

        private double _RestQuantityTolerance;
        /// <summary>
        /// Doc  MaximalRestQuantity
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(200, "MaximalRestQuantity", "en{'Diff tolerance'}de{'Differenztoleranz'}")]
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
                    OnPropertyChanged("MaximalRestQuantity");
                }
            }
        }

        /// <summary>
        /// Doc  BatchSuggestionSum
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(201, "BatchSuggestionSum", "en{'Sum'}de{'Sum'}")]
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
        /// Doc  Difference
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(202, "Difference", "en{'Diff'}de{'Diff'}")]
        public double Difference
        {
            get
            {
                return TotalSize - BatchSuggestionSum ?? 0;
            }
        }

        [ACPropertyInfo(106, "RestNotUsedQuantity", "en{'Not used quantity'}de{'Unbenutzte Restmenge'}")]
        public double RestNotUsedQuantity { get; set; }

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
            }
        }

        public void AddItem(BatchPlanSuggestionItem item)
        {
            ItemsList.Add(item);
            item.PropertyChanged -= Item_PropertyChanged;
            item.PropertyChanged += Item_PropertyChanged;
            OnPropertyChanged("BatchSuggestionSum");
            OnPropertyChanged("Difference");
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string[] properties = new string[] { "BatchTargetCount", "TotalBatchSize", "BatchSize" };
            if (properties.Contains(e.PropertyName))
            {
                OnPropertyChanged("BatchSuggestionSum");
                OnPropertyChanged("Difference");
            }
        }

        public bool IsSuggestionValid()
        {
            if (ItemsList == null || ItemsList.Any(c => c.BatchTargetCount == 0))
                return false;
            double sumSize = ItemsList.Sum(c => c.BatchTargetCount * c.BatchSize);
            return (sumSize - TotalSize) < RestQuantityTolerance;
        }

        #endregion

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return string.Format(@"{0} / {1}", TotalSize, RestNotUsedQuantity);
        }
    }
}
