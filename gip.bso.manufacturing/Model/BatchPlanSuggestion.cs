using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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


        [ACPropertyInfo(100, "TotalBatchSize", "en{'Total Size'}de{'Gesamtgröße'}")]
        public double TotalSize { get; set; }

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
            return string.Format(@"{0} / {1}",  TotalSize, RestNotUsedQuantity);
        }
    }
}
