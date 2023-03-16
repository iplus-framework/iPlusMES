using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{
    [DataContract]
    [Serializable]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'OperationLogItemList'}de{'OperationLogItemList'}", Global.ACKinds.TACClass)]
    public class OperationLogItemList : ObservableCollection<OperationLogItem>, ICyclicRefreshableCollection
    {
        #region const

        public static readonly string[] PropertiesForRefreshDefault = new string[] 
        { 
            nameof(OperationLogItem.RestTime), 
            nameof(OperationLogItem.ElapsedTime), 
            nameof(OperationLogItem.OperationitemTimeStatus) ,
            
            nameof(OperationLogItem.TimeEntered) ,
            nameof(OperationLogItem.FinishTime) 
        };

        #endregion

        #region ctor's

        public OperationLogItemList():base()
        {

        }

        public OperationLogItemList(List<OperationLogItem> list):base(list)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string[] _PropertiesForRefresh;
        public string[] PropertiesForRefresh
        {
            get
            {
                if (_PropertiesForRefresh == null)
                    _PropertiesForRefresh = PropertiesForRefreshDefault;
                return _PropertiesForRefresh;
            }
            set
            {
                if (_PropertiesForRefresh != value)
                {
                    _PropertiesForRefresh = value;
                }
            }
        }

        #endregion

        #region Methods

        public void Refresh()
        {
            foreach (OperationLogItem item in Items)
            {
                foreach (string property in PropertiesForRefresh)
                {
                    item.OnPropertyChanged(property);
                }
            }
        }

        #endregion
    }
}
