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

        #region DataMembers
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
        [ACPropertyInfo(4, nameof(ProgramNo), ConstApp.ProdOrderProgramNo)]
        public string ProgramNo { get; set; }

        [DataMember]
        [ACPropertyInfo(5, nameof(TimeEntered), ConstApp.TimeEntered)]
        public DateTime TimeEntered { get; set; }

        [DataMember]
        [ACPropertyInfo(6, nameof(Duration), ConstApp.Duration)]
        public TimeSpan Duration { get; set; }

        [DataMember]
        [ACPropertyInfo(7, nameof(HintDuration), ConstApp.HintDuration)]
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
        [ACPropertyInfo(9, nameof(FinishTime), ConstApp.FinishTime)]
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
                 - Elapsing      - Now < (FinishTime - HintDuration) (bez boje)
                - TimeReaching   - (FinishTime - HintDuration) <= Now <= FinishTime (žuto)
                - TimeExceeded   - Now > FinishTime (crveno)
                */
                if ((FinishTime - HintDuration) <= DateTime.Now && DateTime.Now <= FinishTime)
                {
                    item = OperationitemTimeStatusEnum.TimeReaching;
                }
                else if (DateTime.Now > FinishTime)
                {
                    item = OperationitemTimeStatusEnum.TimeExceeded;
                }
                return item;
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
