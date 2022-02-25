using System.Runtime.CompilerServices;
using gip.core.datamodel;
using vd = gip.mes.datamodel;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;

namespace gip.mes.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Schedule for WF-Batch-Manager'}de{'Zeitplan für WF-Batch-Manager'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class PAScheduleForPWNode : INotifyPropertyChanged, ICloneable, IACObject
    {
        private Guid _MDSchedulingGroupID;
        [DataMember]
        public Guid MDSchedulingGroupID
        {
            get
            {
                return _MDSchedulingGroupID;
            }
            set
            {
                if (_MDSchedulingGroupID != value)
                {
                    _MDSchedulingGroupID = value;
                    OnPropertyChanged("MDSchedulingGroupID");
                }
            }
        }

        public MDSchedulingGroup _MDSchedulingGroup = null;
        public MDSchedulingGroup MDSchedulingGroup
        {
            get
            {
                return _MDSchedulingGroup;
            }
            set
            {
                if (_MDSchedulingGroup != value)
                {
                    _MDSchedulingGroup = value;
                    OnPropertyChanged("MDSchedulingGroup");
                    OnPropertyChanged("ACCaption");
                }
            }
        }

        [ACPropertyInfo(1, "ACCaption", "en{'Line'}de{'Linie'}")]
        public string ACCaption
        {
            get
            {
                return MDSchedulingGroup != null ? MDSchedulingGroup.ACCaption : MDSchedulingGroupID.ToString();
            }
        }

        private vd.GlobalApp.BatchPlanStartModeEnum _StartMode;
        [DataMember]
        public vd.GlobalApp.BatchPlanStartModeEnum StartMode
        {
            get
            {
                return _StartMode;
            }
            set
            {
                if (_StartMode != value)
                {
                    _StartMode = value;
                    OnPropertyChanged("StartMode");
                    OnPropertyChanged("StartModeName");
                }
            }
        }


        private uint _RefreshCounter;
        [DataMember]
        [ACPropertyInfo(2, "RefreshCounter", "en{'Refresh counter'}de{'Aktualisierungszaehler'}")]
        public uint RefreshCounter
        {
            get
            {
                return _RefreshCounter;
            }
            set

            {
                if (_RefreshCounter != value)
                {
                    _RefreshCounter = value;
                    OnPropertyChanged("RefreshCounter");
                }
            }
        }

        [ACPropertyInfo(999, "StartModeName", "en{'Start mode'}de{'Startmodus'}")]
        public string StartModeName
        {
            get
            {
                ACValueItem item = vd.GlobalApp.BatchPlanStartModeEnumList.Where(c => (vd.GlobalApp.BatchPlanStartModeEnum)c.Value == StartMode).FirstOrDefault();
                if (item != null)
                    return item.ACCaption;
                return StartMode.ToString();
            }
        }

        private DateTime _UpdateTime;
        [DataMember]
        [ACPropertyInfo(999, "UpdateTime", Const.EntityTransUpdateDate)]
        public DateTime UpdateTime
        {
            get

            {
                return _UpdateTime;
            }
            set
            {
                if (_UpdateTime != null)
                {
                    _UpdateTime = value;
                    OnPropertyChanged("UpdateTime");
                }
            }
        }

        private string _UpdateName;
        [DataMember]
        [ACPropertyInfo(999, "UpdateName", Const.EntityTransUpdateName)]
        public string UpdateName
        {
            get
            {
                return _UpdateName;
            }
            set
            {
                if (_UpdateName != value)
                {
                    _UpdateName = value;
                    OnPropertyChanged("UpdateName");
                }
            }
        }

        DateTime? _LastProcessingTime;
        [IgnoreDataMember]
        internal DateTime? LastProcessingTime
        {
            get
            {
                return _LastProcessingTime;
            }
            set
            {
                _LastProcessingTime = value;
            }
        }

        public IACObject ParentACObject
        {
            get
            {
                return null;
            }
        }

        public IACType ACType
        {
            get
            {
                return null;
            }

        }

        public IEnumerable<IACObject> ACContentList
        {
            get
            {
                return null;
            }
        }

        public string ACIdentifier
        {
            get
            {
                if (MDSchedulingGroup == null) return null;
                return MDSchedulingGroup.ACIdentifier;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool AreBatchPlansChanged(PAScheduleForPWNode changedScheduleNode)
        {
            return RefreshCounter != changedScheduleNode.RefreshCounter;
        }

        public bool WasStartModeChanged(PAScheduleForPWNode changedScheduleNode)
        {
            return StartMode != changedScheduleNode.StartMode;
        }

        public void CopyFrom(PAScheduleForPWNode changedScheduleNode, bool onlyRefreshLiveValues)
        {
            this.MDSchedulingGroupID = changedScheduleNode.MDSchedulingGroupID;
            this.StartMode = changedScheduleNode.StartMode;
            this.RefreshCounter = changedScheduleNode.RefreshCounter;
            this.UpdateTime = changedScheduleNode.UpdateTime;
            this.UpdateName = changedScheduleNode.UpdateName;
            if (!onlyRefreshLiveValues)
                this.MDSchedulingGroup = changedScheduleNode.MDSchedulingGroup;
        }

        public const UInt32 MaxRefreshCounter = 4000000000;
        internal void IncrementRefreshCounter()
        {
            this.RefreshCounter++;
            CheckMaxRefreshCounter();
        }

        internal void UpdateAndMaintainRefreshCounter(PAScheduleForPWNode changedScheduleNode)
        {
            if (changedScheduleNode.RefreshCounter > this.RefreshCounter)
            {
                this.RefreshCounter = changedScheduleNode.RefreshCounter;
                CheckMaxRefreshCounter();
            }
        }

        private void CheckMaxRefreshCounter()
        {
            if (this.RefreshCounter > MaxRefreshCounter)
                this.RefreshCounter = 0;
        }

        public object Clone()
        {
            PAScheduleForPWNode clone = new PAScheduleForPWNode();
            clone.CopyFrom(this, false);
            return clone;
        }

        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return null;
            //throw new NotImplementedException();
        }

        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return false;
            //throw new NotImplementedException();
        }

        public string GetACUrl(IACObject rootACObject = null)
        {
            return null;
        }

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return false;
        }
    }
}
