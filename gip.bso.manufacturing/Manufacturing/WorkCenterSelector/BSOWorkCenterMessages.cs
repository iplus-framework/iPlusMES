using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    public class BSOWorkCenterMessages : BSOWorkCenterChild
    {
        #region c'tors

        public BSOWorkCenterMessages(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            //_MainSyncContext = SynchronizationContext.Current;
            return base.ACInit(startChildMode);
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _MScaleWFNodes = null;
            //_MainSyncContext = null;
            UnSubscribeFromWFNodes();
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties 

        //private SynchronizationContext _MainSyncContext;
        //private ACMonitorObject _70050_MainSyncContextLock = new ACMonitorObject(70050);

        private ACMonitorObject _70010_AlarmsLock = new ACMonitorObject(70010);
        private ACMonitorObject _70100_MessageListLock = new ACMonitorObject(70100);

        private Type _MessageItemType = typeof(MessageItem);
        internal static readonly Type _WCSMessagesType = typeof(BSOWorkCenterMessages);

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
                if (_MScaleWFNodes != value)
                {
                    bool changed = true;

                    if (_MScaleWFNodes != null && value != null)
                    {
                        if (_MScaleWFNodes.Count != value.Count)
                        {
                            changed = true;
                        }
                        else
                        {
                            var newItems = value.Where(c => !_MScaleWFNodes.Any(x => x.ACUrlParent == c.ACUrlParent && x.ACIdentifier == x.ACIdentifier));
                            var removedItems = _MScaleWFNodes.Where(c => !value.Any(x => x.ACUrlParent == c.ACUrlParent && x.ACIdentifier == x.ACIdentifier));
                            if (!newItems.Any() && !removedItems.Any())
                            {
                                changed = false;
                            }
                        }
                    }

                    if (changed)
                    {
                        _MScaleWFNodes = value;
                        HandleWFNodes(_MScaleWFNodes);
                    }
                }
            }
        }

        protected readonly SafeList<MessageItem> _MessagesListSafe = new SafeList<MessageItem>();

        public SafeList<MessageItem> MessagesListSafe
        {
            get
            {
                using (ACMonitor.Lock(_70100_MessageListLock))
                {
                    return _MessagesListSafe;
                }
            }
        }

        private ObservableCollection<MessageItem> _MessagesList = new ObservableCollection<MessageItem>();
        [ACPropertyList(610, "Messages")]
        public ObservableCollection<MessageItem> MessagesList
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
            SubscribeToWFNodes(selectedProcessModule);
        }

        public override void DeActivate()
        {
            UnSubscribeFromWFNodes();
        }

        protected void SubscribeToWFNodes(ACComponent selectedProcessModule)
        {
            ACComponent currentProcessModule = selectedProcessModule;

            if (currentProcessModule == null)
            {
                //todo error;
                return;
            }

            var wfNodes = currentProcessModule.GetPropertyNet(nameof(PAProcessModule.WFNodes));
            if (wfNodes == null)
            {
                //Error50285: Initialization error: The process module doesn't have the property {0}.
                // Initialisierungsfehler: Das Prozessmodul besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50285", false, nameof(PAProcessModule.WFNodes));
                return;
            }

            var hasAlarms = currentProcessModule.GetPropertyNet(nameof(PAProcessModule.HasAlarms));
            if (hasAlarms == null)
            {
                //Error50285: Initialization error: The process module doesn't have the property {0}.
                // Initialisierungsfehler: Das Prozessmodul besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50285", false, nameof(PAProcessModule.HasAlarms));
                return;
            }

            var alarmsAsText = currentProcessModule.GetPropertyNet(nameof(PAProcessModule.AlarmsAsText));
            if (alarmsAsText == null)
            {
                //Error50285: Initialization error: The process module doesn't have the property {0}.
                // Initialisierungsfehler: Das Prozessmodul besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50285", false, nameof(PAProcessModule.AlarmsAsText));
                return;
            }


            _WFNodes = wfNodes as IACContainerTNet<List<ACChildInstanceInfo>>;

            if (_WFNodes.ValueT != null)
            {
                List<ACChildInstanceInfo> tempList = _WFNodes.ValueT;
                ParentBSOWCS?.ApplicationQueue.Add(() => MScaleWFNodes = tempList);
            }

            (_WFNodes as IACPropertyNetBase).PropertyChanged += WFNodes_PropertyChanged;

            using (ACMonitor.Lock(_70010_AlarmsLock))
            {
                _AlarmsAsText = alarmsAsText as IACContainerTNet<string>;
            }

            _ScaleHasAlarms = hasAlarms as IACContainerTNet<bool>;

            bool hasAlarmsTemp = _ScaleHasAlarms.ValueT;
            ParentBSOWCS?.ApplicationQueue.Add(() => HandleAlarms(hasAlarmsTemp));

            (_ScaleHasAlarms as IACPropertyNetBase).PropertyChanged += ScaleHasAlarms_PropertyChanged;
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

            MessagesListSafe.Clear();
            RefreshMessageList();

            _MScaleWFNodes = null;

            using (ACMonitor.Lock(_70010_AlarmsLock))
            {
                _AlarmsAsTextCache = null;
                _AlarmsAsText = null;
            }
        }

        private void WFNodes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                IACContainerTNet<List<ACChildInstanceInfo>> senderProp = sender as IACContainerTNet<List<ACChildInstanceInfo>>;
                if (senderProp != null)
                {
                    List<ACChildInstanceInfo> temp = senderProp.ValueT;
                    ParentBSOWCS?.ApplicationQueue.Add(() => MScaleWFNodes = temp);
                }
            }
        }

        private void ScaleHasAlarms_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                IACContainerTNet<bool> senderProp = sender as IACContainerTNet<bool>;
                if (senderProp != null)
                {
                    bool hasAlarms = senderProp.ValueT;
                    ParentBSOWCS?.ApplicationQueue.Add(() => HandleAlarms(hasAlarms));
                }
            }
        }


        private void HandleWFNodes(List<ACChildInstanceInfo> connectionList)
        {
            if (ParentBSOWCS == null || ParentBSOWCS.PWUserAckClasses == null || !ParentBSOWCS.PWUserAckClasses.Any())
                return;

            if (connectionList == null)
            {
                OnHandleWFNodes(connectionList);

                var itemsToRemove = MessagesListSafe.Where(c => c.UserAckPWNode != null).ToArray();
                foreach (var itemToRemove in itemsToRemove)
                {
                    RemoveFromMessageList(itemToRemove);
                    itemToRemove.DeInit();
                }
                if (itemsToRemove.Any())
                    RefreshMessageList();

                return;
            }

            var pwInstanceInfos = connectionList.Where(c => ParentBSOWCS.PWUserAckClasses.Contains(c.ACType.ValueT));

            List<MessageItem> userAckItemsToRemove = MessagesListSafe.Where(c => c.UserAckPWNode != null 
                                                                              && !pwInstanceInfos.Any(x => x.ACUrlParent + "\\" + x.ACIdentifier == c.UserAckPWNode.ACUrl))
                                                                     .ToList();

            // override when we need manually control remove messages for user acknowledge nodes.
            userAckItemsToRemove = OnHandleWFNodesRemoveMessageItems(userAckItemsToRemove);
            
            foreach (var itemToRemove in userAckItemsToRemove)
            {
                RemoveFromMessageList(itemToRemove);
                itemToRemove.DeInit();
            }
            if (userAckItemsToRemove.Any())
                RefreshMessageList();


            foreach (var instanceInfo in pwInstanceInfos)
            {
                string instanceInfoACUrl = instanceInfo.ACUrlParent + "\\" + instanceInfo.ACIdentifier;
                if (MessagesListSafe.Any(c => c.UserAckPWNode != null && c.UserAckPWNode.ACUrl == instanceInfoACUrl))
                    continue;

                var pwNode = Root.ACUrlCommand(instanceInfoACUrl) as IACComponent;
                if (pwNode == null)
                    continue;

                var userAckItem = new MessageItem(pwNode, this);
                AddToMessageList(userAckItem);
            }
            if (pwInstanceInfos.Any())
                RefreshMessageList();

            OnHandleWFNodes(connectionList);
        }

        protected virtual List<MessageItem> OnHandleWFNodesRemoveMessageItems(List<MessageItem> messageItems)
        {
            return messageItems;
        }

        public virtual void OnHandleWFNodes(List<ACChildInstanceInfo> connectionList)
        {

        }

        private void HandleAlarms(bool hasAlarms)
        {
            ACComponent currentProcessModule = null;
            using (ACMonitor.Lock(_70010_AlarmsLock))
            {
                string alarmsAsText = _AlarmsAsText?.ValueT;
                if (alarmsAsText == _AlarmsAsTextCache)
                    return;

                currentProcessModule = _AlarmsAsText?.ParentACComponent as ACComponent;
            }

            if (currentProcessModule == null)
                return;

            if (hasAlarms)
            {
                var alarms = currentProcessModule?.ExecuteMethod("GetAlarms", true, true, true) as List<Msg>;
                if (alarms == null)
                    return;

                var messagesToRemove = MessagesListSafe.Where(c => c.GetType() == _MessageItemType && c.UserAckPWNode == null && !alarms.Any(x => BuildAlarmMessage(x) == c.Message)).ToArray();
                foreach (var messageToRemove in messagesToRemove)
                    RemoveFromMessageList(messageToRemove);

                if (messagesToRemove.Any())
                    RefreshMessageList();

                foreach (var alarm in alarms)
                {
                    MessageItem msgItem = new MessageItem(null, null);
                    msgItem.Message = BuildAlarmMessage(alarm);
                    AddToMessageList(msgItem);
                }
                if (alarms.Any())
                    RefreshMessageList();
            }
            else
            {
                var messageList = MessagesListSafe?.Where(c => c.UserAckPWNode == null && c.HandleByAcknowledgeButton)?.ToArray();
                if (messageList != null)
                {
                    foreach (var messageItem in messageList)
                        RemoveFromMessageList(messageItem);

                    if (messageList.Any())
                        RefreshMessageList();
                }
            }

            using (ACMonitor.Lock(_70010_AlarmsLock))
            {
                if (_AlarmsAsText != null)
                    _AlarmsAsTextCache = _AlarmsAsText.ValueT;
                else
                    _AlarmsAsTextCache = null;
            }
        }

        private string BuildAlarmMessage(Msg msg)
        {
            return string.Format("{0}: {1} {2}", msg.Source, msg.ACCaption, msg.Message);
        }


        public virtual void AcknowledgeMessage()
        {

        }


        public virtual bool AddToMessageList(MessageItem messageItem)
        {
            using (ACMonitor.Lock(_70100_MessageListLock))
            {
                if (_MessagesListSafe == null)
                    return false;

                if (_MessagesListSafe.Any(c => c.IsAlarmMessage && c.Message == messageItem.Message))
                    return true;

                if (!_MessagesListSafe.Any() || messageItem.IsAlarmMessage)
                    _MessagesListSafe.Add(messageItem);
                else
                    _MessagesListSafe.Insert(0, messageItem);
            }

            return true;
        }

        public virtual bool RemoveFromMessageList(MessageItem messageItem)
        {
            using (ACMonitor.Lock(_70100_MessageListLock))
            {
                if (_MessagesListSafe == null)
                    return false;

                _MessagesListSafe.Remove(messageItem);
            }
            return true;
        }

        public void RefreshMessageList()
        {
            //DelegateToMainThread((object state) =>
            //    MessagesList = MessagesListSafe.ToList());
            MessagesList = new ObservableCollection<MessageItem>(MessagesListSafe);
        }

        //public void DelegateToMainThread(SendOrPostCallback d)
        //{
        //    SynchronizationContext context = null;
        //    using (ACMonitor.Lock(_70050_MainSyncContextLock))
        //    {
        //        context = _MainSyncContext;
        //    }
            
        //    if (context != null)
        //        context.Send(d, new object());
        //}

        #endregion
    }
}
