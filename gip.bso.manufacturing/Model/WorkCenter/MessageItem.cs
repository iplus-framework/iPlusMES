using System.Runtime.CompilerServices;
using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gip.mes.processapplication;
using System.ComponentModel;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'UserAckNode'}de{'UserAckNode'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, true)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + "UserAckNode", "", typeof(MessageItem), "UserAckNode", "", "")]
    public class MessageItem : IACObject, INotifyPropertyChanged
    {
        public MessageItem(IACComponent pwNode, IACBSO bso, eMsgLevel msgLevel = eMsgLevel.Default)
        {
            if (pwNode != null)
            {
                UserAckPWNode = new ACRef<IACComponent>(pwNode, bso);
                _AlarmsAsText = UserAckPWNode.ValueT.GetPropertyNet("AlarmsAsText");
                if (_AlarmsAsText != null)
                {
                    _AlarmsAsText.PropertyChanged += AlarmsAsText_PropertyChanged;
                    Message = _AlarmsAsText.Value as string;
                }
                _BSOManualWeighing = bso as BSOManualWeighing;
            }
            _MessgeLevel = msgLevel;
        }

        private IACPropertyNetBase _AlarmsAsText;

        private BSOManualWeighing _BSOManualWeighing;

        private string _Message;
        [ACPropertyInfo(100)]
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                if (_Message != value)
                {
                    _Message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        public virtual bool IsAlarmMessage => UserAckPWNode == null;

        private bool _HandleByAcknowledgeButton = true;
        public virtual bool HandleByAcknowledgeButton
        {
            get => _HandleByAcknowledgeButton;
            set
            {
                _HandleByAcknowledgeButton = value;
            }
        }

        public ACRef<IACComponent> UserAckPWNode
        {
            get;
            set;
        }

        private eMsgLevel _MessgeLevel;
        public eMsgLevel MessageLevel => _MessgeLevel;


        public IACObject ParentACObject => null;

        public IACType ACType => this.ReflectACType();

        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        public string ACIdentifier => this.ReflectGetACIdentifier();

        public string ACCaption => this.ACIdentifier;

        private void AlarmsAsText_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                string alarmMessage = _AlarmsAsText.Value as string;
                Task.Run(() => HandleAlarm(alarmMessage));
            }
        }

        private string _LastAlarmMessage;

        private void HandleAlarm(string alarmMessage)
        {
            if (string.IsNullOrEmpty(alarmMessage))
            {
                if (_LastAlarmMessage != null && Message.Contains(_LastAlarmMessage))
                {
                    if (!string.IsNullOrEmpty(_LastAlarmMessage))
                    {
                        Message = Message.Replace(_LastAlarmMessage, "");
                        _LastAlarmMessage = alarmMessage;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Message))
                {
                    Message = alarmMessage;
                    _LastAlarmMessage = alarmMessage;
                }
                else
                {
                    Message = Message + alarmMessage;
                    _LastAlarmMessage = alarmMessage;
                }
            }
        }

        [ACMethodInfo("", "en{'Acknowledge'}de{'Quittieren'}", 100)]
        public virtual void AcknowledgeMsg()
        {
            if (UserAckPWNode != null && UserAckPWNode.ValueT != null)
            {
                if (_BSOManualWeighing != null && _BSOManualWeighing.CurrentComponentPWNode == UserAckPWNode.ValueT)
                {
                    bool isCompExceedsMaxScaleWeight = _BSOManualWeighing.MaxScaleWeight.HasValue && _BSOManualWeighing.TargetWeight > _BSOManualWeighing.MaxScaleWeight;

                    if (isCompExceedsMaxScaleWeight && _BSOManualWeighing.ScaleBckgrState != ScaleBackgroundState.InTolerance)
                    {
                        //Question50072 : Are you sure that you want acknowledge current component?
                        if (_BSOManualWeighing.Messages.Question(_BSOManualWeighing, "Question50072") != Global.MsgResult.Yes)
                            return;
                    }

                    UserAckPWNode.ValueT.ExecuteMethod(nameof(PWManualWeighing.CompleteWeighing), _BSOManualWeighing.ScaleActualWeight, 
                                                       _BSOManualWeighing.ScaleBckgrState != ScaleBackgroundState.InTolerance);
                }
                else
                {
                    UserAckPWNode.ValueT.ExecuteMethod(PWNodeUserAck.MN_AckStartClient);
                }
            }
        }

        [ACMethodInfo("", "en{'Yes'}de{'Ja'}", 101)]
        public virtual void QuestionYes()
        {
            if (UserAckPWNode != null && UserAckPWNode.ValueT != null)
            {
                UserAckPWNode.ValueT.ACUrlCommand("!UserResponseYes");
            }
        }

        [ACMethodInfo("", "en{'No'}de{'Nein'}", 102)]
        public virtual void QuestionNo()
        {
            if (UserAckPWNode != null && UserAckPWNode.ValueT != null)
            {
                UserAckPWNode.ValueT.ACUrlCommand("!UserResponseNo");
            }
        }

        public virtual void DeInit()
        {
            if (UserAckPWNode == null)
                return;

            if (_AlarmsAsText != null)
                _AlarmsAsText.PropertyChanged -= AlarmsAsText_PropertyChanged;
            _AlarmsAsText = null;
            UserAckPWNode.Detach();
            UserAckPWNode = null;
            _BSOManualWeighing = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
        }

        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ReflectGetACUrl(rootACObject);
        }

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }
    }

    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'MessageItemPWInfo'}de{'MessageItemPWInfo'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, true)]
    public class MessageItemPWInfo : MessageItem
    {
        public MessageItemPWInfo(IACComponent pwNode, IACBSO bso, eMsgLevel msgLevel = eMsgLevel.Default) : base(pwNode, bso, msgLevel)
        {
        }

        public override bool IsAlarmMessage => false;
    }
}
