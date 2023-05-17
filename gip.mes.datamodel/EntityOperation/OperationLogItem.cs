using gip.core.datamodel;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{
    [DataContract]
    [Serializable]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'OperationLogItem'}de{'OperationLogItem'}", Global.ACKinds.TACClass)]
    public class OperationLogItem : INotifyPropertyChanged
    {

        /*
         - MaterialNo (string)
        - MaterialName (string)
        - ProgramNo (string) ??
        - TimeEntered (DateTime)
        - Duration (TimeSpan)
        - HintDuration (TimeSpan) kada se pokaže obavijest prije kraja pečenja

        - ElapsedTime = Now - TimeEntered
- FinishTime (DateTime) = TimeEntered + Duration
- RestTime (TimeSpan)= FinishTime - Now
         */

        public Guid ACProgramLogID;

        #region DataMembers

        [DataMember]
        [ACPropertyInfo(9999)]
        public Guid FacilityChargeID
        {
            get;
            set;
        }

        [DataMember]
        [ACPropertyInfo(1, "Sn", "en{'Sn'}de{'Sn'}")]
        public int Sn { get; set; }

        [DataMember]
        [ACPropertyInfo(2, nameof(MaterialNo), ConstApp.MaterialNo)]
        public string MaterialNo { get; set; }

        [DataMember]
        [ACPropertyInfo(3, nameof(MaterialName), ConstApp.MaterialName1)]
        public string MaterialName { get; set; }

        [DataMember]
        [ACPropertyInfo(4, nameof(LotNo), ConstApp.LotNo)]
        public string LotNo { get; set; }

        [DataMember]
        [ACPropertyInfo(5, nameof(SplitNo), ConstApp.SplitNo)]
        public int SplitNo { get; set; }

        [DataMember]
        [ACPropertyInfo(6, nameof(ProgramNo), ConstApp.ProdOrderProgramNo)]
        public string ProgramNo { get; set; }

        [DataMember]
        [ACPropertyInfo(7, nameof(TimeEntered), ConstApp.TimeEntered)]
        public DateTime TimeEntered { get; set; }

        [DataMember]
        [ACPropertyInfo(8, nameof(MinDuration), ConstApp.MinDuration)]
        public TimeSpan MinDuration { get; set; }

        [DataMember]
        [ACPropertyInfo(9, nameof(MaxDuration), ConstApp.MaxDuration)]
        public TimeSpan? MaxDuration { get; set; }

        [DataMember]
        [ACPropertyInfo(10, nameof(Duration), ConstApp.Duration)]
        public TimeSpan Duration { get; set; }

        [DataMember]
        [ACPropertyInfo(11, nameof(HintDuration), ConstApp.HintDuration)]
        public TimeSpan HintDuration { get; set; }

        #endregion

        #region Calculated

        [IgnoreDataMember]
        [ACPropertyInfo(8, nameof(ElapsedTime), ConstApp.ElapsedTime)]
        public TimeSpan ElapsedTime
        {
            get
            {
                return DateTime.Now - TimeEntered;
            }
        }

        [IgnoreDataMember]
        [ACPropertyInfo(10, nameof(FinishTime), ConstApp.FinishTime)]
        public DateTime FinishTime
        {
            get
            {
                return TimeEntered + Duration;
            }
        }

        [IgnoreDataMember]
        [ACPropertyInfo(10, nameof(RestTime), ConstApp.RestTime)]
        public TimeSpan RestTime
        {
            get
            {
                return FinishTime - DateTime.Now;
            }
        }

        public ACProgramLog ACProgramLog { get; set; }

        [IgnoreDataMember]
        [ACPropertyInfo(11, nameof(OperationitemTimeStatus), ConstApp.OperationitemTimeStatus)]
        public OperationitemTimeStatusEnum OperationitemTimeStatus
        {
            get
            {
                OperationitemTimeStatusEnum item = OperationitemTimeStatusEnum.Elapsing;

                /*
                Elapsing = 0, // blue
                NotifyTimeReaching = 1, // yellow
                TimeReached = 2, // green
                TimeExceeded = 3 // red | if maxduration have value
                */

                if ((FinishTime - HintDuration) <= DateTime.Now && DateTime.Now <= FinishTime)
                {
                    item = OperationitemTimeStatusEnum.NotifyTimeReaching;
                }

                if (DateTime.Now > FinishTime)
                {
                    item = OperationitemTimeStatusEnum.TimeReached;
                }

                if (MaxDuration != null && DateTime.Now >= (TimeEntered + MaxDuration))
                {
                    item = OperationitemTimeStatusEnum.TimeExceeded;
                }

                return item;
            }
        }

        [IgnoreDataMember]
        [ACPropertyInfo(12, nameof(OperationitemTimeStatusShort), ConstApp.OperationitemTimeStatus)]
        public short OperationitemTimeStatusShort
        {
            get
            {
                return (short)OperationitemTimeStatus;
            }
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
