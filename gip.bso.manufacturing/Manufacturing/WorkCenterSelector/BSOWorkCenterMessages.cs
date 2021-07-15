using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    public class BSOWorkCenterMessages : BSOWorkCenterChild
    {
        public BSOWorkCenterMessages(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _MScaleWFNodes = null;

            return base.ACDeInit(deleteACClassTask);
        }

        #region Properties 

        private ACMonitorObject _WFNodesLock_40000 = new ACMonitorObject(40000);

        private IACContainerT<List<ACChildInstanceInfo>> _WFNodes;
        private IACContainerT<bool> _ScaleHasAlarms;
        private IACContainerTNet<string> _AlarmsAsText;

        private string _AlarmsAsTextCache = null;

        private List<ACChildInstanceInfo> _MScaleWFNodes;
        public List<ACChildInstanceInfo> MScaleWFNodes
        {
            get => _MScaleWFNodes;
            set
            {
                if (_WFNodesLock_40000 == null)
                    return;

                using (ACMonitor.Lock(_WFNodesLock_40000))
                {
                    if (_MScaleWFNodes != value)
                    {
                        _MScaleWFNodes = value;
                        HandleWFNodes(_MScaleWFNodes);
                    }
                }
            }
        }

        private List<MessageItem> _MessagesList = new List<MessageItem>();
        [ACPropertyList(610, "Messages")]
        public List<MessageItem> MessagesList
        {
            get
            {
                return _MessagesList;
            }
            set
            {
                _MessagesList = value;
                OnPropertyChanged("MessagesList");
            }
        }

        //private MessageItem _SelectedMessage;
        [ACPropertySelected(611, "Messages")]
        public MessageItem SelectedMessage
        {
            get;
            set;
        }

        private List<MessageItem> _AckMessageList;
        [ACPropertyList(612, "MessagesAck")]
        public List<MessageItem> AckMessageList
        {
            get => _AckMessageList;
            set
            {
                _AckMessageList = value;
                OnPropertyChanged("AckMessageList");
            }
        }

        private MessageItem _SelectedAckMessage;
        [ACPropertySelected(613, "MessagesAck")]
        public MessageItem SelectedAckMessage
        {
            get => _SelectedAckMessage;
            set
            {
                _SelectedAckMessage = value;
                OnPropertyChanged("SelectedAckMessage");
            }
        }

        #endregion

        #region Methods

        public override void Activate(ACComponent selectedProcessModule)
        {
            SubscribeToWFNodes();
        }

        public override void DeActivate()
        {
            UnSubscribeFromWFNodes();
        }

        private void SubscribeToWFNodes()
        {
            if (ParentBSOWCS.CurrentProcessModule == null)
            {
                //todo error;
                return;
            }

            var wfNodes = ParentBSOWCS.CurrentProcessModule.GetPropertyNet("WFNodes");
            if (wfNodes == null)
            {
                //Error50285: Initialization error: The process module doesn't have the property {0}.
                // Initialisierungsfehler: Das Prozessmodul besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50285", false, "WFNodes");
                return;
            }

            var hasAlarms = ParentBSOWCS.CurrentProcessModule.GetPropertyNet("HasAlarms");
            if (hasAlarms == null)
            {
                //Error50285: Initialization error: The process module doesn't have the property {0}.
                // Initialisierungsfehler: Das Prozessmodul besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50285", false, "HasAlarms");
                return;
            }

            var alarmsAsText = ParentBSOWCS.CurrentProcessModule.GetPropertyNet("AlarmsAsText");
            if (alarmsAsText == null)
            {
                //Error50285: Initialization error: The process module doesn't have the property {0}.
                // Initialisierungsfehler: Das Prozessmodul besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50285", false, "AlarmsAsText");
                return;
            }


            _WFNodes = wfNodes as IACContainerTNet<List<ACChildInstanceInfo>>;
            (_WFNodes as IACPropertyNetBase).PropertyChanged += WFNodes_PropertyChanged;
            if (_WFNodes.ValueT != null)
                Task.Run(() => MScaleWFNodes = _WFNodes.ValueT);

            _AlarmsAsText = alarmsAsText as IACContainerTNet<string>;

            _ScaleHasAlarms = hasAlarms as IACContainerTNet<bool>;
            (_ScaleHasAlarms as IACPropertyNetBase).PropertyChanged += ScaleHasAlarms_PropertyChanged;
            bool hasAlarmsTemp = _ScaleHasAlarms.ValueT;
            Task.Run(() => HandleAlarms(hasAlarmsTemp));
        }

        private void UnSubscribeFromWFNodes()
        {
            if (_WFNodes != null)
            {
                (_WFNodes as IACPropertyNetBase).PropertyChanged -= WFNodes_PropertyChanged;
                _WFNodes = null;
            }

            if (_ScaleHasAlarms != null)
            {
                (_ScaleHasAlarms as IACPropertyNetBase).PropertyChanged -= ScaleHasAlarms_PropertyChanged;
                _ScaleHasAlarms = null;
            }

            _MScaleWFNodes = null;
            _AlarmsAsTextCache = null;
            _AlarmsAsText = null;
        }

        private void WFNodes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
                Task.Run(() => MScaleWFNodes = _WFNodes.ValueT);
        }

        private void ScaleHasAlarms_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                bool hasAlarms = _ScaleHasAlarms.ValueT;
                Task.Run(() => HandleAlarms(hasAlarms));
            }
        }


        private void HandleWFNodes(List<ACChildInstanceInfo> connectionList)
        {
            if (BSOWorkCenterSelector.PWUserAckClasses == null || !BSOWorkCenterSelector.PWUserAckClasses.Any())
                return;

            if (connectionList == null)
            {
                //BtnAckBlink = false;

                OnHandleWFNodes(connectionList);

                var itemsToRemove = MessagesList.Where(c => c.UserAckPWNode != null).ToArray();
                foreach (var itemToRemove in itemsToRemove)
                {
                    RemoveFromMessageList(itemToRemove);
                    itemToRemove.DeInit();
                }

                return;
            }

            var pwInstanceInfos = connectionList.Where(c => BSOWorkCenterSelector.PWUserAckClasses.Contains(c.ACType.ValueT));

            var userAckItemsToRemove = MessagesList.Where(c => c.UserAckPWNode != null && !pwInstanceInfos.Any(x => x.ACUrlParent + "\\" + x.ACIdentifier == c.UserAckPWNode.ACUrl)).ToArray();
            foreach (var itemToRemove in userAckItemsToRemove)
            {
                RemoveFromMessageList(itemToRemove);
                itemToRemove.DeInit();
                //if (BtnAckBlink && !MessagesList.Any(c => c.HandleByAcknowledgeButton && !c.IsAlarmMessage))
                //    BtnAckBlink = false;
            }

            foreach (var instanceInfo in pwInstanceInfos)
            {
                string instanceInfoACUrl = instanceInfo.ACUrlParent + "\\" + instanceInfo.ACIdentifier;
                if (MessagesList.Any(c => c.UserAckPWNode != null && c.UserAckPWNode.ACUrl == instanceInfoACUrl))
                    continue;

                var pwNode = Root.ACUrlCommand(instanceInfoACUrl) as IACComponent;
                if (pwNode == null)
                    continue;

                var userAckItem = new MessageItem(pwNode, this);
                AddToMessageList(userAckItem);
                //if (!BtnAckBlink)
                //    BtnAckBlink = true;
            }

            OnHandleWFNodes(connectionList);
        }

        public virtual void OnHandleWFNodes(List<ACChildInstanceInfo> connectionList)
        {

        }


        private void HandleAlarms(bool hasAlarms)
        {
            string alarmsAsText = _AlarmsAsText.ValueT;
            if (alarmsAsText == _AlarmsAsTextCache)
                return;

            if (hasAlarms)
            {
                var alarms = ParentBSOWCS.CurrentProcessModule?.ExecuteMethod("GetAlarms", true, true, true) as List<Msg>;
                if (alarms == null)
                    return;

                var messagesToRemove = MessagesList.Where(c => c.GetType() == typeof(MessageItem) && c.UserAckPWNode == null && !alarms.Any(x => BuildAlarmMessage(x) == c.Message)).ToArray();
                foreach (var messageToRemove in messagesToRemove)
                    RemoveFromMessageList(messageToRemove);

                foreach (var alarm in alarms)
                {
                    MessageItem msgItem = new MessageItem(null, null);
                    msgItem.Message = BuildAlarmMessage(alarm);
                    AddToMessageList(msgItem);
                }
            }
            else if (MessagesList != null)
            {
                var messageList = MessagesList.Where(c => c.UserAckPWNode == null && c.HandleByAcknowledgeButton).ToArray();
                foreach (var messageItem in messageList)
                    RemoveFromMessageList(messageItem);
            }

            _AlarmsAsTextCache = _AlarmsAsText.ValueT;
        }

        private string BuildAlarmMessage(Msg msg)
        {
            return string.Format("{0}: {1} {2}", msg.Source, msg.ACCaption, msg.Message);
        }


        public virtual void AcknowledgeMessage()
        {

        }


        public bool AddToMessageList(MessageItem messageItem)
        {
            if (MessagesList == null)
                return false;

            if (MessagesList.Any(c => c.IsAlarmMessage && c.Message == messageItem.Message))
                return true;

            List<MessageItem> msgList = new List<MessageItem>(MessagesList);
            if (messageItem.IsAlarmMessage)
                msgList.Add(messageItem);
            else
                msgList.Insert(0, messageItem);

            MessagesList = msgList;
            return true;
        }

        public bool RemoveFromMessageList(MessageItem messageItem)
        {
            if (MessagesList == null)
                return false;

            List<MessageItem> msgList = new List<MessageItem>(MessagesList);
            msgList.Remove(messageItem);

            MessagesList = msgList;
            return true;
        }

        #endregion
    }
}
