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

    /// <summary>
    /// Represents the message management component for a manufacturing work center.
    /// Handles initialization, subscription to workflow nodes, alarm processing, and message acknowledgment.
    /// Maintains lists of messages and acknowledged messages, synchronizes with process modules,
    /// and provides thread-safe operations for message handling.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Work center messages'}de{'Work center messages'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true,
                 Description = @"Represents the message management component for a manufacturing work center.
                                 Handles initialization, subscription to workflow nodes, alarm processing, and message acknowledgment.
                                 Maintains lists of messages and acknowledged messages, synchronizes with process modules,
                                 and provides thread-safe operations for message handling.")]
    public class BSOWorkCenterMessages : BSOWorkCenterChild
    {
        #region c'tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOWorkCenterMessages"/> class.
        /// This constructor sets up the message management component for a manufacturing work center,
        /// establishing the base infrastructure for handling messages, alarms, and workflow node subscriptions.
        /// </summary>
        /// <param name="acType">The iPlus-Type information (ACClass) used for constructing this component instance, containing metadata about the class structure and capabilities.</param>
        /// <param name="content">The content object associated with this instance. For persistent instances in application trees, this is typically an ACClassTask object that ensures state persistence across service restarts. For dynamic instances, this is usually null.</param>
        /// <param name="parentACObject">The parent ACComponent under which this instance is created as a child object, establishing the component hierarchy within the work center structure.</param>
        /// <param name="parameter">Construction parameters passed via ACValueList containing ACValue entries with parameter names, values, and data types. Use ACClass.TypeACSignature() to get the correct parameter structure for the component type.</param>
        /// <param name="acIdentifier">Unique identifier for this instance within the parent component's child collection. If empty, the runtime assigns an ID automatically using the ACType identifier.</param>
        public BSOWorkCenterMessages(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        /// <summary>
        /// Initializes the BSOWorkCenterMessages component.
        /// Sets up the message management infrastructure for the work center, including message, alarm, and workflow node handling.
        /// Calls the base initialization logic.
        /// </summary>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            //_MainSyncContext = SynchronizationContext.Current;
            return base.ACInit(startChildMode);
        }

        /// <summary>
        /// Deinitializes the BSOWorkCenterMessages component.
        /// Releases workflow node references, unsubscribes from workflow node events, and clears alarm/message state.
        /// Calls the base deinitialization logic to complete cleanup.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the list of workflow node instance information for the scale (MScale) in the work center.
        /// When set, detects changes in the list and triggers workflow node handling logic if the list has changed.
        /// </summary>
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
        /// <summary>
        /// Gets a thread-safe list of message items for the work center.
        /// Access to the list is synchronized using a monitor lock to ensure safe concurrent operations.
        /// </summary>
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
        /// <summary>
        /// Gets or sets the collection of message items for the work center.
        /// This property provides an observable collection that can be data-bound to UI elements for displaying messages.
        /// When set, it updates the internal collection and notifies listeners of the change.
        /// </summary>
        [ACPropertyList(610, "Messages",
                        Description = @"Gets or sets the collection of message items for the work center.
                                        This property provides an observable collection that can be data-bound to UI elements for displaying messages.
                                        When set, it updates the internal collection and notifies listeners of the change.")]
        public ObservableCollection<MessageItem> MessagesList
        {
            get
            {
                return _MessagesList;
            }
            set
            {
                _MessagesList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected message item in the work center.
        /// This property is typically used for data binding to UI elements, allowing the user to view or interact with the selected message.
        /// </summary>
        [ACPropertySelected(611, "Messages",
                            Description = @"Gets or sets the currently selected message item in the work center.
                                            This property is typically used for data binding to UI elements, allowing the user to view or interact with the selected message.")]
        public MessageItem SelectedMessage
        {
            get;
            set;
        }

        private List<MessageItem> _AckMessageList;
        /// <summary>
        /// Gets or sets the list of acknowledged message items for the work center.
        /// This property provides access to messages that have been acknowledged by the user.
        /// When set, it updates the internal list and notifies listeners of the change.
        /// </summary>
        [ACPropertyList(612, "MessagesAck",
                        Description = @"Gets or sets the list of acknowledged message items for the work center.
                                        This property provides access to messages that have been acknowledged by the user.
                                        When set, it updates the internal list and notifies listeners of the change.")]
        public List<MessageItem> AckMessageList
        {
            get => _AckMessageList;
            set
            {
                _AckMessageList = value;
                OnPropertyChanged();
            }
        }

        private MessageItem _SelectedAckMessage;
        /// <summary>
        /// Gets or sets the currently selected acknowledged message item in the work center.
        /// This property is typically used for data binding to UI elements, allowing the user to view or interact with the selected acknowledged message.
        /// Notifies listeners when the selected acknowledged message changes.
        /// </summary>
        [ACPropertySelected(613, "MessagesAck",
                            Description = @"Gets or sets the currently selected acknowledged message item in the work center.
                                            This property is typically used for data binding to UI elements, allowing the user to view or interact with the selected acknowledged message.
                                            Notifies listeners when the selected acknowledged message changes.")]
        public MessageItem SelectedAckMessage
        {
            get => _SelectedAckMessage;
            set
            {
                _SelectedAckMessage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Activates the message management for the specified process module in the work center.
        /// Subscribes to workflow node events and initializes message handling for the selected process module.
        /// </summary>
        /// <param name="selectedProcessModule">The process module (ACComponent) to activate and monitor for messages and alarms.</param>
        public override void Activate(ACComponent selectedProcessModule)
        {
            SubscribeToWFNodes(selectedProcessModule);
        }

        /// <summary>
        /// Deactivates the message management for the work center.
        /// Unsubscribes from workflow node events and releases related resources.
        /// </summary>
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

        /// <summary>
        /// Hook method for customizing the removal of message items associated with user-acknowledge workflow nodes.
        /// Allows derived classes to filter or modify the list of message items to be removed when workflow nodes change.
        /// By default, returns the input list unchanged.
        /// </summary>
        /// <param name="messageItems">The list of message items considered for removal.</param>
        /// <returns>The (optionally filtered) list of message items to remove.</returns>
        protected virtual List<MessageItem> OnHandleWFNodesRemoveMessageItems(List<MessageItem> messageItems)
        {
            return messageItems;
        }

        /// <summary>
        /// Virtual method invoked when workflow node connection list changes.
        /// Allows derived classes to implement custom logic for handling workflow node updates.
        /// The default implementation does nothing.
        /// </summary>
        /// <param name="connectionList">The updated list of workflow node instance information, or null if no nodes are present.</param>
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

        /// <summary>
        /// Handles the acknowledgment of the currently selected message in the work center.
        /// This method is intended to be overridden in derived classes to implement custom acknowledgment logic,
        /// such as marking a message as acknowledged, moving it to the acknowledged messages list,
        /// or updating the UI to reflect the acknowledgment state.
        /// </summary>
        public virtual void AcknowledgeMessage()
        {

        }

        /// <summary>
        /// Adds a message item to the thread-safe message list for the work center.
        /// Ensures that alarm messages are not duplicated and that new messages are inserted at the correct position.
        /// Returns true if the message was added or already exists, false if the message list is unavailable.
        /// </summary>
        /// <param name="messageItem">The message item to add.</param>
        /// <returns>True if the message was added or already exists, false if the message list is unavailable.</returns>
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

        /// <summary>
        /// Removes the specified message item from the thread-safe message list for the work center.
        /// Ensures thread-safe access using a monitor lock. Returns true if the message was removed or the list is available, false otherwise.
        /// </summary>
        /// <param name="messageItem">The message item to remove.</param>
        /// <returns>True if the message was removed or the list is available, false otherwise.</returns>
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

        /// <summary>
        /// Refreshes the observable collection of message items (`MessagesList`) to reflect the current state
        /// of the thread-safe message list (`MessagesListSafe`). This ensures that any UI elements bound to
        /// `MessagesList` are updated with the latest messages from the work center.
        /// </summary>
        public void RefreshMessageList()
        {
            MessagesList = new ObservableCollection<MessageItem>(MessagesListSafe);
        }

        #endregion
    }
}
