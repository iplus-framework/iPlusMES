using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VD = gip.mes.datamodel;
using System.Xml;
using System.Data;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Worfklow scheduler base'}de{'Workflow Zeitplaner Basis'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + VD.ProdOrderBatchPlan.ClassName)]
    public abstract class BSOWorkflowSchedulerBase : ACBSOvb
    {
        #region const
        public const int Const_MaxFilterDaySpan = 10;
        public const int Const_MaxResultSize = 500;
        #endregion

        #region Configuration

        #region Configuration -> ConfigPreselectedLine

        public IACConfig GetSelectedLineConfig()
        {
            var configs = ACType.GetConfigByKeyACUrl(Root.Environment.User.GetACUrl());
            IACConfig config = configs.FirstOrDefault();
            return config;
        }

        public string GetSelectedLine()
        {

            IACConfig config = GetSelectedLineConfig();
            return config != null ?
                (config.Value != null ? config.Value.ToString() : "") : "";
        }

        public void SetSelectedLineConfig(PAScheduleForPWNode line)
        {
            IACConfig config = GetSelectedLineConfig();
            if (config == null && line != null)
            {
                config = ACType.ValueTypeACClass.NewACConfig(null, ACType.ValueTypeACClass.Database.GetACType(typeof(string)));
                config.KeyACUrl = Root.Environment.User.GetACUrl();
            }
            if (line != null)
                config.Value = line.MDSchedulingGroup.MDKey;
            else if (config != null)
                (config as VBEntityObject).DeleteACObject(Database, false);
            Msg msg = DatabaseApp.ContextIPlus.ACSaveChanges();
        }

        #endregion

        #region Scheduler
        private ACPropertyConfigValue<string> _PABatchPlanSchedulerURL;
        [ACPropertyConfig("PABatchPlanSchedulerURL")]
        public string PABatchPlanSchedulerURL
        {
            get
            {
                if (string.IsNullOrEmpty(_PABatchPlanSchedulerURL.ValueT))
                    _PABatchPlanSchedulerURL.ValueT = LoadPABatchPlanSchedulerURL();
                return _PABatchPlanSchedulerURL.ValueT;
            }
            set
            {
                _PABatchPlanSchedulerURL.ValueT = value;
            }
        }

        protected abstract string LoadPABatchPlanSchedulerURL();

        private short? _GroupIndexFrom;
        public short GroupIndexFrom
        {
            get
            {
                if (_GroupIndexFrom.HasValue)
                    return _GroupIndexFrom.Value;
                if (PAWorkflowScheduler != null)
                {
                    try
                    {
                        _GroupIndexFrom = (short)PAWorkflowScheduler[nameof(PAWorkflowSchedulerBase.GroupIndexFrom)];
                    }
                    catch (Exception) 
                    { 
                    }
                }
                if (!_GroupIndexFrom.HasValue)
                    _GroupIndexFrom = 0;
                return _GroupIndexFrom.Value;
            }
        }

        private short? _GroupIndexTo;
        public short GroupIndexTo
        {
            get
            {
                if (_GroupIndexTo.HasValue)
                    return _GroupIndexTo.Value;
                if (PAWorkflowScheduler != null)
                {
                    try
                    {
                        _GroupIndexTo = (short)PAWorkflowScheduler[nameof(PAWorkflowSchedulerBase.GroupIndexTo)];
                    }
                    catch (Exception)
                    {
                    }
                }
                if (!_GroupIndexTo.HasValue)
                    _GroupIndexTo = short.MaxValue;
                return _GroupIndexTo.Value;
            }
        }

        #endregion
        #endregion

        #region c´tors

        public BSOWorkflowSchedulerBase(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _BSOWorkflowSchedulerBaseRules = new ACPropertyConfigValue<string>(this, nameof(BSOWorkflowSchedulerBaseRules), "");
            _PABatchPlanSchedulerURL = new ACPropertyConfigValue<string>(this, nameof(PABatchPlanSchedulerURL), "");
        }

        #region c´tors -> ACInit

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _ = BSOWorkflowSchedulerBaseRules;
            _ = PABatchPlanSchedulerURL;


            var refBatchPlanSchedulerComponent = ACUrlCommand(PABatchPlanSchedulerURL) as ACComponent;
            if (refBatchPlanSchedulerComponent != null)
                _PAWorkflowScheduler = new ACRef<ACComponent>(refBatchPlanSchedulerComponent, this);

            if (PAWorkflowScheduler != null)
            {
                PAWorkflowScheduler.SubscribeAllNetPoints();
            }

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            InitBatchPlanSchedulerComponent();

            string selectedLine = GetSelectedLine();
            LoadScheduleListForPWNodes(selectedLine);
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            if (_PAWorkflowScheduler != null)
            {
                if (_SchedulesForPWNodesProp != null)
                    _SchedulesForPWNodesProp.PropertyChanged -= SchedulesForPWNodesProp_Changed;
                _PAWorkflowScheduler.Detach();
                _PAWorkflowScheduler = null;
            }

            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            _TempRules = null;
            return await base.ACDeInit(deleteACClassTask);
        }

        private VD.DatabaseApp _DatabaseApp;
        public override VD.DatabaseApp DatabaseApp
        {
            get
            {
                if (_DatabaseApp != null)
                    return _DatabaseApp;
                if (ParentACComponent is Businessobjects
                    || !(ParentACComponent is ACBSOvb || ParentACComponent is ACBSOvbNav))
                {
                    _DatabaseApp = ACObjectContextManager.GetOrCreateContext<VD.DatabaseApp>(this.GetACUrl());
                    return _DatabaseApp;
                }
                else
                {
                    ACBSOvbNav parentNav = ParentACComponent as ACBSOvbNav;
                    if (parentNav != null)
                        return parentNav.DatabaseApp;
                    ACBSOvb parent = ParentACComponent as ACBSOvb;
                    if (parent != null)
                        return parent.DatabaseApp;
                    _DatabaseApp = ACObjectContextManager.GetOrCreateContext<VD.DatabaseApp>(this.GetACUrl());
                    return _DatabaseApp;
                }
            }
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(ConfigureBSO):
                    _= ConfigureBSO();
                    return true;
                case nameof(IsEnabledConfigureBSO):
                    result = IsEnabledConfigureBSO();
                    return true;
                case nameof(AddRule):
                    AddRule();
                    return true;
                case nameof(IsEnabledAddRule):
                    result = IsEnabledAddRule();
                    return true;
                case nameof(RemoveRule):
                    RemoveRule();
                    return true;
                case nameof(IsEnabledRemoveRule):
                    result = IsEnabledRemoveRule();
                    return true;
                case nameof(ApplyRulesAndClose):
                    ApplyRulesAndClose();
                    return true;
                case nameof(IsEnabledApplyRulesAndClose):
                    result = IsEnabledApplyRulesAndClose();
                    return true;
                case nameof(InitialBuildLines):
                    InitialBuildLines();
                    return true;
                case nameof(IsEnabledInitialBuildLines):
                    result = IsEnabledInitialBuildLines();
                    return true;
                case nameof(ChangeMode):
                    ChangeMode();
                    return true;
                case nameof(IsEnabledChangeMode):
                    result = IsEnabledChangeMode();
                    return true;
                case nameof(ResetFilterStartTime):
                    ResetFilterStartTime();
                    return true;
                case nameof(IsEnabledResetFilterStartTime):
                    result = IsEnabledResetFilterStartTime();
                    return true;
                case nameof(ResetFilterEndTime):
                    ResetFilterEndTime();
                    return true;
                case nameof(IsEnabledResetFilterEndTime):
                    result = IsEnabledResetFilterEndTime();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(IsEnabledSearch):
                    result = IsEnabledSearch();
                    return true;
                case nameof(ItemDrag):
                    ItemDrag((System.Collections.Generic.Dictionary<System.Int32, System.String>)acParameter[0]);
                    return true;
                case nameof(IsEnabledItemDrag):
                    result = IsEnabledItemDrag();
                    return true;
                case nameof(Save):
                    _= Save();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #region c´tors -> ACInit -> Scheduler Component

        private void InitBatchPlanSchedulerComponent()
        {
            var refBatchPlanSchedulerComponent = ACUrlCommand(PABatchPlanSchedulerURL) as ACComponent;
            if (refBatchPlanSchedulerComponent != null)
                _PAWorkflowScheduler = new ACRef<ACComponent>(refBatchPlanSchedulerComponent, this);

            if (_PAWorkflowScheduler != null)
            {
                _SchedulesForPWNodesProp = _PAWorkflowScheduler.ValueT.GetPropertyNet(PABatchPlanScheduler.PN_SchedulesForPWNodes) as IACContainerTNet<PAScheduleForPWNodeList>;
                if (_SchedulesForPWNodesProp != null)
                    _SchedulesForPWNodesProp.PropertyChanged += SchedulesForPWNodesProp_Changed;
            }
        }

        private void SchedulesForPWNodesProp_Changed(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string selectedLine = GetSelectedLine();
            LoadScheduleListForPWNodes(selectedLine, RefreshScheduleListOnReceivedChange);
        }

        #endregion

        #endregion

        #endregion

        #region Managers
        protected ACRef<ACComponent> _PAWorkflowScheduler = null;
        public ACComponent PAWorkflowScheduler
        {
            get
            {
                if (_PAWorkflowScheduler == null) return null;
                return _PAWorkflowScheduler.ValueT;
            }
        }

        protected ACRef<ACComponent> _ACFacilityManager = null;
        public FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }
        #endregion

        #region Properties

        #region Properties Filter

        /// <summary>
        /// Flag is network batch list refresh is executed
        /// </summary>
        public bool RefreshScheduleListOnReceivedChange = true;

        private DateTime? _FilterStartTime;
        [ACPropertyInfo(525, "FilterStartTime", "en{'From'}de{'Von'}")]
        public DateTime? FilterStartTime
        {
            get
            {
                return _FilterStartTime;
            }
            set
            {
                if (_FilterStartTime != value)
                {
                    _FilterStartTime = value;
                    OnPropertyChanged(nameof(FilterStartTime));
                }
            }
        }

        private DateTime? _FilterEndTime;
        [ACPropertyInfo(526, "FilterEndTime", "en{'To'}de{'Bis'}")]
        public DateTime? FilterEndTime
        {
            get
            {
                return _FilterEndTime;
            }
            set
            {
                if (_FilterEndTime != value)
                {
                    _FilterEndTime = value;
                    OnPropertyChanged(nameof(FilterEndTime));
                }
            }
        }


        #endregion


        #region Properties -> Explorer -> ScheduleForPWNode

        private IACContainerTNet<PAScheduleForPWNodeList> _SchedulesForPWNodesProp;

        protected PAScheduleForPWNode _SelectedScheduleForPWNode;
        /// <summary>
        /// Selected property for BatchStartModeConfiguration
        /// </summary>
        /// <value>The selected BatchStartModeConfiguration</value>
        [ACPropertySelected(501, "ScheduleForPWNode", "en{'Schedule for WF-Batch-Manager'}de{'Zeitplan für WF-Batch-Manager'}")]
        public virtual PAScheduleForPWNode SelectedScheduleForPWNode
        {
            get
            {
                return _SelectedScheduleForPWNode;
            }
            set
            {
                if (_SelectedScheduleForPWNode != value)
                {
                    _SelectedScheduleForPWNode = value;
                    OnPropertyChanged(nameof(SelectedScheduleForPWNode));
                    if (_SelectedScheduleForPWNode != null)
                        SelectedFilterBatchPlanStartMode = FilterBatchPlanStartModeList.Where(c => (BatchPlanStartModeEnum)c.Value == _SelectedScheduleForPWNode.StartMode).FirstOrDefault();
                    else
                        SelectedFilterBatchPlanStartMode = FilterBatchPlanStartModeList.Where(c => (BatchPlanStartModeEnum)c.Value == BatchPlanStartModeEnum.Off).FirstOrDefault();
                    RefreshScheduleForSelectedNode();

                    OnPAScheduleForPWNodeChanged();

                    string selectedLinie = GetSelectedLine();
                    string mdKey = "";
                    if (value != null)
                        mdKey = value.MDSchedulingGroup.MDKey;
                    if (selectedLinie != mdKey)
                        SetSelectedLineConfig(value);
                }
            }
        }

        protected virtual void OnPAScheduleForPWNodeChanged()
        {
        }

        bool _SchedulingGroupValidated = false;
        /// <summary>
        /// List property for BatchStartModeConfiguration
        /// </summary>
        /// <value>The BatchStartModeConfiguration list</value>
        private PAScheduleForPWNodeList _ScheduleForPWNodeList;
        [ACPropertyList(502, "ScheduleForPWNode")]
        public IEnumerable<PAScheduleForPWNode> ScheduleForPWNodeList
        {
            get
            {
                if (_ScheduleForPWNodeList != null && _ScheduleForPWNodeList.Any())
                {
                    if (!_SchedulingGroupValidated && _ScheduleForPWNodeList.Where(c => c.MDSchedulingGroup == null).Any())
                        Messages.ErrorAsync(this, "A Scheduling-Group was removed. Invoke Reset on Scheduler");
                    _SchedulingGroupValidated = true;

                    _RulesForCurrentUser = GetRulesForCurrentUser();

                    if (_RulesForCurrentUser != null && _RulesForCurrentUser.Any() && !Root.Environment.User.IsSuperuser)
                    {
                        return _ScheduleForPWNodeList.Where(c => c.MDSchedulingGroup != null && _RulesForCurrentUser.Any(r => r.RuleParamID == c.MDSchedulingGroupID))
                                                     .OrderBy(c => c.MDSchedulingGroup.SortIndex)
                                                     .ThenBy(c => c.MDSchedulingGroup.MDSchedulingGroupName);
                    }

                    return _ScheduleForPWNodeList
                        .Where(c => c.MDSchedulingGroup != null)
                        .OrderBy(c => c.MDSchedulingGroup.SortIndex)
                        .ThenBy(c => c.MDSchedulingGroup.MDSchedulingGroupName);
                }
                return _ScheduleForPWNodeList;
            }
        }

        #endregion

        #region Properties -> Explorer -> FilterBatchPlanStartMode

        private ACValueItem _SelectedFilterBatchPlanStartMode;
        [ACPropertySelected(510, "FilterBatchPlanStartMode", "en{'Scheduler mode'}de{'Zeitplanermodus'}", "", true)]
        public ACValueItem SelectedFilterBatchPlanStartMode
        {
            get
            {
                return _SelectedFilterBatchPlanStartMode;
            }
            set
            {
                _SelectedFilterBatchPlanStartMode = value;
                OnPropertyChanged(nameof(SelectedFilterBatchPlanStartMode));
            }
        }

        private ACValueItemList _FilterBatchPlanStartModeList;
        [ACPropertyList(511, "FilterBatchPlanStartMode")]
        public ACValueItemList FilterBatchPlanStartModeList
        {
            get
            {
                if (_FilterBatchPlanStartModeList == null)
                    _FilterBatchPlanStartModeList = DatabaseApp.BatchPlanStartModeEnumList as ACValueItemList;
                return _FilterBatchPlanStartModeList;
            }
        }

        public bool HasUserRoleOfPlanner
        {
            get
            {
                ClassRightManager rightManager = CurrentRightsOfInvoker;
                if (rightManager == null)
                    return true;
                IACPropertyBase acProperty = this.GetProperty(nameof(SelectedFilterBatchPlanStartMode));
                if (acProperty == null)
                    return true;
                Global.ControlModes rightMode = rightManager.GetControlMode(acProperty.ACType);
                return rightMode >= Global.ControlModes.Enabled;

            }
        }
        #endregion


        #region Properties -> Messages

        public void SendMessage(object result)
        {
            Msg msg = result as Msg;
            if (msg != null)
            {
                SendMessage(msg);
            }
        }

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(528, "Message", "en{'Message'}de{'Meldung'}")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged(nameof(CurrentMsg));
            }
        }

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets the MSG list.
        /// </summary>
        /// <value>The MSG list.</value>
        [ACPropertyList(529, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged(nameof(MsgList));
        }

        public void ClearMessages()
        {
            MsgList.Clear();
            OnPropertyChanged(nameof(MsgList));
        }
        #endregion

        #region Properties => Configuration Rules

        private ACPropertyConfigValue<string> _BSOWorkflowSchedulerBaseRules;
        [ACPropertyConfig("en{'Work center rules'}de{'Work center rules'}")]
        public string BSOWorkflowSchedulerBaseRules
        {
            get
            {
                string rules = _BSOWorkflowSchedulerBaseRules.ValueT;
                if (String.IsNullOrEmpty(rules))
                    rules = OnGetWorkflowSchedulerBaseRules();
                return rules;
            }
            set
            {
                _BSOWorkflowSchedulerBaseRules.ValueT = value;
                OnPropertyChanged();
            }
        }

        protected virtual string OnGetWorkflowSchedulerBaseRules()
        {
            return "";
        }

        // select * from ACClassConfig where LocalConfigACUrl like '%BSOBatchPlanSchedulerRules%';
        // <ArrayOfACPropertyExt xmlns:i="http://www.w3.org/2001/XMLSchema-instance" z:Id="1" z:Size="1" xmlns:z="http://schemas.microsoft.com/2003/10/Serialization/" xmlns="http://schemas.datacontract.org/2004/07/gip.core.datamodel"><ACPropertyExt z:Id="2"><ACIdentifier z:Id="3">Value</ACIdentifier><TypeString z:Id="4">System.String</TypeString><XMLValue z:Id="5">&lt;ArrayOfUserRuleItem xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.datacontract.org/2004/07/gip.bso.manufacturing"&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc86f64d-97f4-438d-8405-b037071246e3&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5920f3e6-f813-46f4-a88a-5b8c57eeb6c5&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;db259fd7-bb2d-4518-abf5-e558ca0ef1cb&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;a85dbbb0-6022-4871-bb64-77f04d607b63&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;a3e7463b-300a-4252-b10a-27355873e71f&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48b4db60-c121-41f3-9ee0-34381f20c611&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c79c50e-85ec-4ada-9056-8cc459500e70&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c601c3f-1421-495f-868f-822b659e661b&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;394fc22c-a3cb-403d-92ff-74631df34f0d&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7135cac0-0ed4-4483-a114-a2bb93cb66f9&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;e897ea1d-e092-4d45-a2c2-cee4ccbf817e&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48043dec-7477-4f03-ba1b-03fcad5dcc12&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;782bfec8-3c78-4663-a114-f729201c1ee4&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5e85869d-aad7-4bc6-8c8b-fb0a7c762ee2&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;85bb9d49-c4d6-4792-ad08-5f292e2a9472&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;82295ba4-b90d-4fe3-bc1e-405e4e2f2d79&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7f088d1e-d8de-4c7f-9e19-b007dd0b641d&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;736332be-87fb-4949-a8f5-a9e7ed084d26&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc090596-f9c2-412a-97f7-422ec5b98b62&lt;/RuleParamID&gt;&lt;VBUserID&gt;e76fbb76-cf4c-4196-bbcc-adc5a6d4e52e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;31083099-5183-432e-b0f3-e8ab676f3d6c&lt;/RuleParamID&gt;&lt;VBUserID&gt;76579a38-9ce4-4d25-985d-8a459c9a8919&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;eea9aed0-71bc-4525-bbe8-07d90c18bcc8&lt;/RuleParamID&gt;&lt;VBUserID&gt;76579a38-9ce4-4d25-985d-8a459c9a8919&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;f5d209bc-82ef-4095-9c77-a13fdd411f21&lt;/RuleParamID&gt;&lt;VBUserID&gt;76579a38-9ce4-4d25-985d-8a459c9a8919&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7d42044b-71a3-4924-9db2-a6f81f6f9b48&lt;/RuleParamID&gt;&lt;VBUserID&gt;76579a38-9ce4-4d25-985d-8a459c9a8919&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc86f64d-97f4-438d-8405-b037071246e3&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5920f3e6-f813-46f4-a88a-5b8c57eeb6c5&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;db259fd7-bb2d-4518-abf5-e558ca0ef1cb&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;a85dbbb0-6022-4871-bb64-77f04d607b63&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;a3e7463b-300a-4252-b10a-27355873e71f&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48b4db60-c121-41f3-9ee0-34381f20c611&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c79c50e-85ec-4ada-9056-8cc459500e70&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c601c3f-1421-495f-868f-822b659e661b&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;394fc22c-a3cb-403d-92ff-74631df34f0d&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7135cac0-0ed4-4483-a114-a2bb93cb66f9&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;e897ea1d-e092-4d45-a2c2-cee4ccbf817e&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48043dec-7477-4f03-ba1b-03fcad5dcc12&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;782bfec8-3c78-4663-a114-f729201c1ee4&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5e85869d-aad7-4bc6-8c8b-fb0a7c762ee2&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;85bb9d49-c4d6-4792-ad08-5f292e2a9472&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;82295ba4-b90d-4fe3-bc1e-405e4e2f2d79&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7f088d1e-d8de-4c7f-9e19-b007dd0b641d&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;736332be-87fb-4949-a8f5-a9e7ed084d26&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc090596-f9c2-412a-97f7-422ec5b98b62&lt;/RuleParamID&gt;&lt;VBUserID&gt;1921501c-60bc-40c7-95eb-79a2a2f1b1de&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc86f64d-97f4-438d-8405-b037071246e3&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5920f3e6-f813-46f4-a88a-5b8c57eeb6c5&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;db259fd7-bb2d-4518-abf5-e558ca0ef1cb&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;a85dbbb0-6022-4871-bb64-77f04d607b63&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;a3e7463b-300a-4252-b10a-27355873e71f&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48b4db60-c121-41f3-9ee0-34381f20c611&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c79c50e-85ec-4ada-9056-8cc459500e70&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c601c3f-1421-495f-868f-822b659e661b&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;394fc22c-a3cb-403d-92ff-74631df34f0d&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7135cac0-0ed4-4483-a114-a2bb93cb66f9&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;e897ea1d-e092-4d45-a2c2-cee4ccbf817e&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48043dec-7477-4f03-ba1b-03fcad5dcc12&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;782bfec8-3c78-4663-a114-f729201c1ee4&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5e85869d-aad7-4bc6-8c8b-fb0a7c762ee2&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;85bb9d49-c4d6-4792-ad08-5f292e2a9472&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;82295ba4-b90d-4fe3-bc1e-405e4e2f2d79&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7f088d1e-d8de-4c7f-9e19-b007dd0b641d&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;736332be-87fb-4949-a8f5-a9e7ed084d26&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc090596-f9c2-412a-97f7-422ec5b98b62&lt;/RuleParamID&gt;&lt;VBUserID&gt;1b0780de-3c46-482e-8868-70ffbbf17b01&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;31083099-5183-432e-b0f3-e8ab676f3d6c&lt;/RuleParamID&gt;&lt;VBUserID&gt;3bcb5f70-3242-40f4-8772-4c54c1ebfe35&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;eea9aed0-71bc-4525-bbe8-07d90c18bcc8&lt;/RuleParamID&gt;&lt;VBUserID&gt;78862bbc-cb75-4ef2-bbb2-9289784a9f51&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;e49f7af9-5c82-4ab7-8a18-05d393f2a27a&lt;/RuleParamID&gt;&lt;VBUserID&gt;028ce0c7-c568-47cd-9ada-a18ca68ecbaa&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc86f64d-97f4-438d-8405-b037071246e3&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48b4db60-c121-41f3-9ee0-34381f20c611&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c79c50e-85ec-4ada-9056-8cc459500e70&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c601c3f-1421-495f-868f-822b659e661b&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;394fc22c-a3cb-403d-92ff-74631df34f0d&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7135cac0-0ed4-4483-a114-a2bb93cb66f9&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;e897ea1d-e092-4d45-a2c2-cee4ccbf817e&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48043dec-7477-4f03-ba1b-03fcad5dcc12&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;782bfec8-3c78-4663-a114-f729201c1ee4&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5e85869d-aad7-4bc6-8c8b-fb0a7c762ee2&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;85bb9d49-c4d6-4792-ad08-5f292e2a9472&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;82295ba4-b90d-4fe3-bc1e-405e4e2f2d79&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7f088d1e-d8de-4c7f-9e19-b007dd0b641d&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;736332be-87fb-4949-a8f5-a9e7ed084d26&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc090596-f9c2-412a-97f7-422ec5b98b62&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;ed52b36c-3b8d-4a81-9f31-490b7288abd9&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;216b3e94-52e7-4e48-8ea0-7a7e6fa4b243&lt;/RuleParamID&gt;&lt;VBUserID&gt;b50efb11-3ae0-4191-af6a-1d696595a87a&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5920f3e6-f813-46f4-a88a-5b8c57eeb6c5&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48b4db60-c121-41f3-9ee0-34381f20c611&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c79c50e-85ec-4ada-9056-8cc459500e70&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c601c3f-1421-495f-868f-822b659e661b&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;394fc22c-a3cb-403d-92ff-74631df34f0d&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7135cac0-0ed4-4483-a114-a2bb93cb66f9&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;e897ea1d-e092-4d45-a2c2-cee4ccbf817e&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48043dec-7477-4f03-ba1b-03fcad5dcc12&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;782bfec8-3c78-4663-a114-f729201c1ee4&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5e85869d-aad7-4bc6-8c8b-fb0a7c762ee2&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;85bb9d49-c4d6-4792-ad08-5f292e2a9472&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;82295ba4-b90d-4fe3-bc1e-405e4e2f2d79&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;ed52b36c-3b8d-4a81-9f31-490b7288abd9&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;216b3e94-52e7-4e48-8ea0-7a7e6fa4b243&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7f088d1e-d8de-4c7f-9e19-b007dd0b641d&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;736332be-87fb-4949-a8f5-a9e7ed084d26&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc090596-f9c2-412a-97f7-422ec5b98b62&lt;/RuleParamID&gt;&lt;VBUserID&gt;7b510317-2d7f-41d6-be2c-8861db0fed50&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;db259fd7-bb2d-4518-abf5-e558ca0ef1cb&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48b4db60-c121-41f3-9ee0-34381f20c611&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c79c50e-85ec-4ada-9056-8cc459500e70&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c601c3f-1421-495f-868f-822b659e661b&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;394fc22c-a3cb-403d-92ff-74631df34f0d&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7135cac0-0ed4-4483-a114-a2bb93cb66f9&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;e897ea1d-e092-4d45-a2c2-cee4ccbf817e&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48043dec-7477-4f03-ba1b-03fcad5dcc12&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;782bfec8-3c78-4663-a114-f729201c1ee4&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5e85869d-aad7-4bc6-8c8b-fb0a7c762ee2&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;85bb9d49-c4d6-4792-ad08-5f292e2a9472&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;82295ba4-b90d-4fe3-bc1e-405e4e2f2d79&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;ed52b36c-3b8d-4a81-9f31-490b7288abd9&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;216b3e94-52e7-4e48-8ea0-7a7e6fa4b243&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7f088d1e-d8de-4c7f-9e19-b007dd0b641d&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;736332be-87fb-4949-a8f5-a9e7ed084d26&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc090596-f9c2-412a-97f7-422ec5b98b62&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc86f64d-97f4-438d-8405-b037071246e3&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5920f3e6-f813-46f4-a88a-5b8c57eeb6c5&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;a85dbbb0-6022-4871-bb64-77f04d607b63&lt;/RuleParamID&gt;&lt;VBUserID&gt;8864f1c4-d986-4164-94d0-c2fe0daa096b&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48b4db60-c121-41f3-9ee0-34381f20c611&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c79c50e-85ec-4ada-9056-8cc459500e70&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c601c3f-1421-495f-868f-822b659e661b&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;394fc22c-a3cb-403d-92ff-74631df34f0d&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7135cac0-0ed4-4483-a114-a2bb93cb66f9&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;e897ea1d-e092-4d45-a2c2-cee4ccbf817e&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48043dec-7477-4f03-ba1b-03fcad5dcc12&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;782bfec8-3c78-4663-a114-f729201c1ee4&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5e85869d-aad7-4bc6-8c8b-fb0a7c762ee2&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;85bb9d49-c4d6-4792-ad08-5f292e2a9472&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;82295ba4-b90d-4fe3-bc1e-405e4e2f2d79&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;ed52b36c-3b8d-4a81-9f31-490b7288abd9&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;216b3e94-52e7-4e48-8ea0-7a7e6fa4b243&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7f088d1e-d8de-4c7f-9e19-b007dd0b641d&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;736332be-87fb-4949-a8f5-a9e7ed084d26&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc090596-f9c2-412a-97f7-422ec5b98b62&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;a85dbbb0-6022-4871-bb64-77f04d607b63&lt;/RuleParamID&gt;&lt;VBUserID&gt;970d195d-b510-4354-989a-c2f94d9e97f2&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;eea9aed0-71bc-4525-bbe8-07d90c18bcc8&lt;/RuleParamID&gt;&lt;VBUserID&gt;d4779711-77d6-4a40-b252-f87e89895e77&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;31083099-5183-432e-b0f3-e8ab676f3d6c&lt;/RuleParamID&gt;&lt;VBUserID&gt;d4779711-77d6-4a40-b252-f87e89895e77&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7d42044b-71a3-4924-9db2-a6f81f6f9b48&lt;/RuleParamID&gt;&lt;VBUserID&gt;d4779711-77d6-4a40-b252-f87e89895e77&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;f5d209bc-82ef-4095-9c77-a13fdd411f21&lt;/RuleParamID&gt;&lt;VBUserID&gt;d4779711-77d6-4a40-b252-f87e89895e77&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc86f64d-97f4-438d-8405-b037071246e3&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5920f3e6-f813-46f4-a88a-5b8c57eeb6c5&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;db259fd7-bb2d-4518-abf5-e558ca0ef1cb&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;a85dbbb0-6022-4871-bb64-77f04d607b63&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;a3e7463b-300a-4252-b10a-27355873e71f&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48b4db60-c121-41f3-9ee0-34381f20c611&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c79c50e-85ec-4ada-9056-8cc459500e70&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7c601c3f-1421-495f-868f-822b659e661b&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;394fc22c-a3cb-403d-92ff-74631df34f0d&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7135cac0-0ed4-4483-a114-a2bb93cb66f9&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;e897ea1d-e092-4d45-a2c2-cee4ccbf817e&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;48043dec-7477-4f03-ba1b-03fcad5dcc12&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;782bfec8-3c78-4663-a114-f729201c1ee4&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;5e85869d-aad7-4bc6-8c8b-fb0a7c762ee2&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;85bb9d49-c4d6-4792-ad08-5f292e2a9472&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;82295ba4-b90d-4fe3-bc1e-405e4e2f2d79&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7f088d1e-d8de-4c7f-9e19-b007dd0b641d&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;736332be-87fb-4949-a8f5-a9e7ed084d26&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;bc090596-f9c2-412a-97f7-422ec5b98b62&lt;/RuleParamID&gt;&lt;VBUserID&gt;5511757d-19cd-41bd-a000-aa64b121d35e&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;eea9aed0-71bc-4525-bbe8-07d90c18bcc8&lt;/RuleParamID&gt;&lt;VBUserID&gt;90fd182c-c2ea-4b2a-afd2-9d6d83c5f835&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;31083099-5183-432e-b0f3-e8ab676f3d6c&lt;/RuleParamID&gt;&lt;VBUserID&gt;90fd182c-c2ea-4b2a-afd2-9d6d83c5f835&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;f5d209bc-82ef-4095-9c77-a13fdd411f21&lt;/RuleParamID&gt;&lt;VBUserID&gt;90fd182c-c2ea-4b2a-afd2-9d6d83c5f835&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7d42044b-71a3-4924-9db2-a6f81f6f9b48&lt;/RuleParamID&gt;&lt;VBUserID&gt;90fd182c-c2ea-4b2a-afd2-9d6d83c5f835&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;31083099-5183-432e-b0f3-e8ab676f3d6c&lt;/RuleParamID&gt;&lt;VBUserID&gt;6b2bc54d-6ac4-49fe-b973-894caf200bcc&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;eea9aed0-71bc-4525-bbe8-07d90c18bcc8&lt;/RuleParamID&gt;&lt;VBUserID&gt;6b2bc54d-6ac4-49fe-b973-894caf200bcc&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;f5d209bc-82ef-4095-9c77-a13fdd411f21&lt;/RuleParamID&gt;&lt;VBUserID&gt;6b2bc54d-6ac4-49fe-b973-894caf200bcc&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7d42044b-71a3-4924-9db2-a6f81f6f9b48&lt;/RuleParamID&gt;&lt;VBUserID&gt;6b2bc54d-6ac4-49fe-b973-894caf200bcc&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;eea9aed0-71bc-4525-bbe8-07d90c18bcc8&lt;/RuleParamID&gt;&lt;VBUserID&gt;fdd04cc8-c9bb-42ec-9c5d-ac63cfc1df06&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;31083099-5183-432e-b0f3-e8ab676f3d6c&lt;/RuleParamID&gt;&lt;VBUserID&gt;fdd04cc8-c9bb-42ec-9c5d-ac63cfc1df06&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;7d42044b-71a3-4924-9db2-a6f81f6f9b48&lt;/RuleParamID&gt;&lt;VBUserID&gt;fdd04cc8-c9bb-42ec-9c5d-ac63cfc1df06&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;UserRuleItem&gt;&lt;RuleParamID&gt;f5d209bc-82ef-4095-9c77-a13fdd411f21&lt;/RuleParamID&gt;&lt;VBUserID&gt;fdd04cc8-c9bb-42ec-9c5d-ac63cfc1df06&lt;/VBUserID&gt;&lt;/UserRuleItem&gt;&lt;/ArrayOfUserRuleItem&gt;</XMLValue></ACPropertyExt></ArrayOfACPropertyExt>
        private List<UserRuleItem> _TempRules;

        private List<UserRuleItem> _RulesForCurrentUser;

        private core.datamodel.VBUser _SelectedVBUser;
        [ACPropertySelected(604, "VBUser", "en{'User'}de{'Benutzer'}")]
        public core.datamodel.VBUser SelectedVBUser
        {
            get => _SelectedVBUser;
            set
            {
                _SelectedVBUser = value;
                OnPropertyChanged(nameof(SelectedVBUser));
            }
        }

        private core.datamodel.VBUser[] _VBUserList;
        [ACPropertyList(605, "VBUser")]
        public IEnumerable<core.datamodel.VBUser> VBUserList
        {
            get
            {
                if (_VBUserList == null)
                {
                    using (ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
                    {
                        _VBUserList = DatabaseApp.ContextIPlus.VBUser.OrderBy(c => c.VBUserName).ToArray();
                    }
                }
                return _VBUserList;
            }
        }

        private core.datamodel.VBGroup _SelectedVBGroup;
        [ACPropertySelected(604, "VBGroup", "en{'Group'}de{'Gruppe'}")]
        public core.datamodel.VBGroup SelectedVBGroup
        {
            get => _SelectedVBGroup;
            set
            {
                _SelectedVBGroup = value;
                OnPropertyChanged(nameof(SelectedVBUser));
            }
        }

        private core.datamodel.VBGroup[] _VBGroupList;
        [ACPropertyList(605, "VBGroup")]
        public IEnumerable<core.datamodel.VBGroup> VBGroupList
        {
            get
            {
                if (_VBGroupList == null)
                {
                    using (ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
                    {
                        _VBGroupList = DatabaseApp.ContextIPlus.VBGroup.OrderBy(c => c.VBGroupName).ToArray();
                    }
                }
                return _VBGroupList;
            }
        }

        private UserRuleItem _SelectedAssignedUserRule;
        [ACPropertySelected(606, "UserRelatedRules", "en{'Assigned user rule'}de{'Assigned user rule'}")]
        public UserRuleItem SelectedAssignedUserRule
        {
            get => _SelectedAssignedUserRule;
            set
            {


                _SelectedAssignedUserRule = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<UserRuleItem> _AssignedUserRules;
        [ACPropertyList(607, "UserRelatedRules")]
        public IEnumerable<UserRuleItem> AssignedUserRules
        {
            get => _AssignedUserRules;
            set
            {
                _AssignedUserRules = value;
                OnPropertyChanged();
            }
        }

        private MDSchedulingGroup _SelectedAvailableSchedulingGroup;
        [ACPropertySelected(608, "AvailableRules", "en{'Available scheduling groups'}de{'Available scheduling groups'}")]
        public MDSchedulingGroup SelectedAvailableSchedulingGroup
        {
            get => _SelectedAvailableSchedulingGroup;
            set
            {
                _SelectedAvailableSchedulingGroup = value;
                OnPropertyChanged();
            }
        }

        private List<MDSchedulingGroup> _AvailableSchedulingGroupsList;
        [ACPropertyList(609, "AvailableRules")]
        public List<MDSchedulingGroup> AvailableSchedulingGroupsList
        {
            get => _AvailableSchedulingGroupsList;
            set
            {
                _AvailableSchedulingGroupsList = value;
                OnPropertyChanged();
            }
        }

        #endregion



        #endregion

        #region Methods

        #region Methods -> ACMethod

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("BatchPlanList", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public virtual async Task Save()
        {
            if (!PreExecute("Save")) return;
            using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
            {
                await OnSave();
            }
            PostExecute("Save");
        }

        public virtual bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        protected override void OnPostSave()
        {
            HandleRefreshServerStateOnPostSave();
            base.OnPostSave();
        }

        protected virtual void HandleRefreshServerStateOnPostSave()
        {
            if (SelectedScheduleForPWNode != null && ParentACObject != null)
            {
                RefreshServerState(SelectedScheduleForPWNode);
            }
        }

        public void RefreshServerState(Guid mdSchedulingGroupID)
        {
            PAScheduleForPWNode pAScheduleForPWNode = ScheduleForPWNodeList.FirstOrDefault(c => c.MDSchedulingGroupID == mdSchedulingGroupID);
            if (pAScheduleForPWNode != null)
                RefreshServerState(pAScheduleForPWNode);
        }

        public void RefreshServerState(PAScheduleForPWNode pAScheduleForPWNode)
        {
            pAScheduleForPWNode.RefreshCounter++;
            if(PAWorkflowScheduler != null)
            {
                var result = PAWorkflowScheduler.ExecuteMethod(PABatchPlanScheduler.MN_UpdateScheduleFromClient, new object[] { pAScheduleForPWNode });
                if (result != null)
                {
                    SendMessage(result);
                }
            }
        }


        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand("BatchPlanList", "en{'Undo'}de{'Rückgängig'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public virtual void UndoSave()
        {
            if (!PreExecute("UndoSave")) return;
            using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
            {
                OnUndoSave();
            }
            PostExecute("UndoSave");
        }

        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction(Partslist.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedProdOrderBatchPlan", Global.ACKinds.MSMethodPrePost)]
        public virtual void Load(bool requery = false)
        {
            if (requery)
            {
                OnPropertyChanged(nameof(ScheduleForPWNodeList));
                RefreshScheduleForSelectedNode();
            }
        }

        public bool IsEnabledLoad()
        {
            return SelectedScheduleForPWNode != null;
        }

        public virtual bool LocalSaveChanges()
        {
            Msg msg = null;
            bool isPowerUser = HasUserRoleOfPlanner;
            using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
            {
                msg = DatabaseApp.ACSaveChanges();
            }
            if (msg != null)
            {
                if (isPowerUser)
                {
                    Messages.MsgAsync(msg);
                }
                else
                {
                    SendMessage(msg);
                }
                Messages.LogMessageMsg(msg);
            }
            return msg == null || msg.IsSucceded();
        }

        #endregion

        #region Methods -> ChangeMode

        [ACMethodCommand("ChangeMode", "en{'Change Mode'}de{'Mode ändern'}", 501, true)]
        public void ChangeMode()
        {
            if (!IsEnabledChangeMode())
                return;
            PAScheduleForPWNode updateNode = new PAScheduleForPWNode();
            updateNode.CopyFrom(SelectedScheduleForPWNode, true);
            updateNode.StartMode = (VD.BatchPlanStartModeEnum)SelectedFilterBatchPlanStartMode.Value;
            updateNode.UpdateName = Root.CurrentInvokingUser.Initials;
            var result = PAWorkflowScheduler.ExecuteMethod(PABatchPlanScheduler.MN_UpdateScheduleFromClient, new object[] { updateNode });
            if (result != null)
            {
                SendMessage(result);
            }
        }

        public bool IsEnabledChangeMode()
        {
            return
                SelectedScheduleForPWNode != null &&
                PAWorkflowScheduler != null &&
                SelectedFilterBatchPlanStartMode != null &&
                _SchedulesForPWNodesProp != null;
        }

        #endregion

        #region Methods -> (Tab)BatchPlanScheduler

        #region Methods ResetFilter

        /// <summary>
        /// Method ResetFilterStartTime
        /// </summary>
        [ACMethodInfo("ResetFilterStartTime", "en{'Set / Reset'}de{'Setzen / Zurücksetzen'}", 700)]
        public void ResetFilterStartTime()
        {
            if (!IsEnabledResetFilterStartTime())
                return;
            FilterStartTime = FilterStartTime == null ? (DateTime?)DateTime.Now : null;
        }

        public bool IsEnabledResetFilterStartTime()
        {
            return true;
        }

        /// <summary>
        /// Method ResetFilterEndTime
        /// </summary>
        [ACMethodInfo("ResetFilterEndTime", "en{'Set / Reset'}de{'Setzen / Zurücksetzen'}", 701)]
        public void ResetFilterEndTime()
        {
            if (!IsEnabledResetFilterEndTime())
                return;
            if (FilterEndTime != null)
                FilterEndTime = null;
            else if (FilterStartTime != null)
                FilterEndTime = FilterStartTime.Value.AddDays(1);
            else
                FilterEndTime = null;
        }

        public bool IsEnabledResetFilterEndTime()
        {
            return true;
        }


        #endregion

        #region Methods -> Search
        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("Search", "en{'Search'}de{'Suche'}", (short)MISort.Search)]
        public void Search()
        {
            if (!IsEnabledSearch())
                return;
            OnPropertyChanged(nameof(ScheduleForPWNodeList));
            RefreshScheduleForSelectedNode();
        }

        public virtual bool IsEnabledSearch()
        {
            return SelectedScheduleForPWNode != null
                   && FilterStartTime != null
                    && FilterEndTime != null
                    && (FilterEndTime.Value - FilterStartTime.Value).TotalDays > 0
                    && (FilterEndTime.Value - FilterStartTime.Value).TotalDays <= Const_MaxFilterDaySpan;
        }

        #endregion

        #region Methods Interaction

        [ACMethodInfo("ItemDrag", "en{'Drag'}de{'Objekt ziehen'}", 506, true)]
        public virtual void ItemDrag(Dictionary<int, string> newOrder)
        {
            if (!IsEnabledItemDrag()) 
                return;
        }

        public virtual bool IsEnabledItemDrag()
        {
            return SelectedScheduleForPWNode != null;
        }

        #endregion

        #endregion

        #region Methods -> Overrides
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            var result = base.OnGetControlModes(vbControl);

            if (vbControl != null && !string.IsNullOrEmpty(vbControl.VBContent))
            {
                switch (vbControl.VBContent)
                {
                    case nameof(FilterStartTime):
                        result = Global.ControlModes.Enabled;
                        bool filterStartTimeIsRequired =
                           FilterStartTime == null
                                    || (
                                            FilterStartTime != null
                                            && FilterEndTime != null
                                            && (FilterEndTime.Value - FilterStartTime.Value).TotalDays > Const_MaxFilterDaySpan
                                        );
                        if (filterStartTimeIsRequired)
                            result = Global.ControlModes.EnabledWrong;
                        break;
                    case nameof(FilterEndTime):
                        result = Global.ControlModes.Enabled;
                        bool filterEndTimeIsRequired =
                            FilterEndTime == null
                                    || (
                                            FilterStartTime != null
                                            && FilterEndTime != null
                                            && (FilterEndTime.Value - FilterStartTime.Value).TotalDays > Const_MaxFilterDaySpan
                                        );
                        if (filterEndTimeIsRequired)
                            result = Global.ControlModes.EnabledWrong;
                        break;
                }
            }
            return result;
        }
        #endregion


        #region Methods -> Private (Helper) Methods -> Load

        protected abstract Guid? EntityIDOfSelectedSchedule { get; }

        protected virtual void RefreshScheduleForSelectedNode(Guid? selectedEntityID = null)
        {
        }

        private void LoadScheduleListForPWNodes(string schedulingGroupMDKey, bool reloadScheduleList = true)
        {
            PAScheduleForPWNodeList newScheduleForWFNodeList = null;
            if (_SchedulesForPWNodesProp != null && _SchedulesForPWNodesProp.ValueT != null)
                newScheduleForWFNodeList = _SchedulesForPWNodesProp.ValueT;
            else
                newScheduleForWFNodeList = PAWorkflowSchedulerBase.CreateScheduleListForPWNodes(this, DatabaseApp, null, GroupIndexFrom, GroupIndexTo);
            //int removedCount = newScheduleForWFNodeList.RemoveAll(x => x.MDSchedulingGroupID == Guid.Empty);
            UpdateScheduleForPWNodeList(newScheduleForWFNodeList, reloadScheduleList);
            if (!string.IsNullOrEmpty(schedulingGroupMDKey))
                SelectedScheduleForPWNode = ScheduleForPWNodeList.FirstOrDefault(c => c.MDSchedulingGroup.MDKey == schedulingGroupMDKey);
        }

        private void UpdateScheduleForPWNodeList(PAScheduleForPWNodeList newScheduleForWFNodeList, bool reloadScheduleList = true)
        {
            PAScheduleForPWNodeList.DiffResult diffResult = PAScheduleForPWNodeList.DiffResult.Equal;
            if (_ScheduleForPWNodeList == null)
            {
                _ScheduleForPWNodeList = newScheduleForWFNodeList.Clone() as PAScheduleForPWNodeList;
                diffResult = PAScheduleForPWNodeList.DiffResult.NewPWNodesDetected;
            }
            else
            {
                diffResult = _ScheduleForPWNodeList.CompareAndUpdateFrom(newScheduleForWFNodeList, this.SelectedScheduleForPWNode);
            }

            if (diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.NewPWNodesDetected))
            {
                PAScheduleForPWNode[] newNodes = _ScheduleForPWNodeList.Where(c => c.MDSchedulingGroup == null).ToArray();
                if (newNodes != null && newNodes.Any())
                {
                    List<MDSchedulingGroup> mDSchedulingGroups = DatabaseApp.MDSchedulingGroup.ToList();
                    foreach (var newNode in newNodes)
                    {
                        newNode.MDSchedulingGroup = mDSchedulingGroups.Where(c => c.MDSchedulingGroupID == newNode.MDSchedulingGroupID).FirstOrDefault();
                    }
                }
            }

            if (diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.NewPWNodesDetected)
                || diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.PWNodesRemoved)
                || diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.StartModeChanged))
            {
                if (diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.StartModeChanged)
                    && !diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.NewPWNodesDetected)
                    && !diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.PWNodesRemoved))
                {
                    OnPropertyChanged(nameof(SelectedScheduleForPWNode));
                }
                else
                {
                    var selected = SelectedScheduleForPWNode;
                    OnPropertyChanged(nameof(ScheduleForPWNodeList));
                    SelectedScheduleForPWNode = selected;
                    if (SelectedScheduleForPWNode != null)
                        SelectedFilterBatchPlanStartMode = FilterBatchPlanStartModeList.Where(c => (BatchPlanStartModeEnum)c.Value == _SelectedScheduleForPWNode.StartMode).FirstOrDefault();
                }
            }

            if ((diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.RefreshCounterChanged))
                //|| diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.ValueChangesInList))
                && reloadScheduleList)
            {
                OnUpdateScheduleForPWNodeList();
            }
        }

        protected virtual void OnUpdateScheduleForPWNodeList()
        {
            RefreshScheduleForSelectedNode(EntityIDOfSelectedSchedule);
        }

        #endregion


        #region Methods => Configuration

        [ACMethodInteraction("", "en{'Configure scheduler rules'}de{'Konfigurieren Regeln für den Zeitplaner'}", 601, true)]
        public async Task ConfigureBSO()
        {
            if (_TempRules == null)
                _TempRules = GetStoredRules();
            AssignedUserRules = _TempRules.OrderBy(c => c.RuleParamCaption);

            if (AvailableSchedulingGroupsList == null)
                AvailableSchedulingGroupsList =
                    DatabaseApp
                    .MDSchedulingGroup.ToArray().OrderBy(c => c.ACCaption).ToList();

            await ShowDialogAsync(this, "ConfigurationDialog");
        }

        public bool IsEnabledConfigureBSO()
        {
            return true;//Root.Environment.User.IsSuperuser;
        }

        [ACMethodInfo("", "en{'Grant permission'}de{'Berechtigung erteilen'}", 602)]
        public void AddRule()
        {
            UserRuleItem existingRule = null;

            if (SelectedVBUser != null)
            {
                existingRule = AssignedUserRules.FirstOrDefault(c => c.VBUserID == SelectedVBUser.VBUserID
                                                                  && c.RuleParamID == SelectedAvailableSchedulingGroup.MDSchedulingGroupID);
            }
            else if (SelectedVBGroup != null)
            {
                existingRule = AssignedUserRules.FirstOrDefault(c => c.VBUserID == SelectedVBGroup.VBGroupID
                                                                  && c.RuleParamID == SelectedAvailableSchedulingGroup.MDSchedulingGroupID);
            }

            if (existingRule != null)
                return;

            UserRuleItem rule = new UserRuleItem()
            {
                VBUserID = SelectedVBUser != null ? SelectedVBUser.VBUserID : SelectedVBGroup.VBGroupID,
                VBUserName = SelectedVBUser != null ? SelectedVBUser.VBUserName : SelectedVBGroup.VBGroupName,
                RuleParamID = SelectedAvailableSchedulingGroup.MDSchedulingGroupID,
                RuleParamCaption = SelectedAvailableSchedulingGroup.ACCaption
            };

            _TempRules.Add(rule);
            AssignedUserRules = _TempRules.OrderBy(c => c.RuleParamCaption).ToList();
        }

        public bool IsEnabledAddRule()
        {
            return (SelectedVBUser != null || SelectedVBGroup != null) && SelectedAvailableSchedulingGroup != null;
        }

        [ACMethodInfo("", "en{'Remove permission'}de{'Berechtigung entfernen'}", 603)]
        public void RemoveRule()
        {
            UserRuleItem rule = _TempRules.FirstOrDefault(c => c == SelectedAssignedUserRule);
            if (rule != null)
            {
                _TempRules.Remove(rule);
                AssignedUserRules = _TempRules.OrderBy(c => c.RuleParamCaption).ToList();
                SelectedAssignedUserRule = null;
            }
        }

        public bool IsEnabledRemoveRule()
        {
            return SelectedAssignedUserRule != null;
        }

        [ACMethodInfo("", "en{'Apply rules and close'}de{'Regeln anwenden und schließen'}", 604)]
        public void ApplyRulesAndClose()
        {
            string xml = "";
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<UserRuleItem>));
                serializer.WriteObject(xmlWriter, _TempRules);
                xml = sw.ToString();
            }
            BSOWorkflowSchedulerBaseRules = xml;
            LocalSaveChanges();

            _TempRules = GetStoredRules();

            CloseTopDialog();
        }

        public bool IsEnabledApplyRulesAndClose()
        {
            return true;
        }

        private List<UserRuleItem> GetRulesForCurrentUser()
        {
            if (_TempRules == null)
                _TempRules = GetStoredRules();

            using (Database db = new core.datamodel.Database())
            {
                core.datamodel.VBUser vbUser = db.VBUser.FirstOrDefault(c => c.VBUserName == Root.Environment.User.VBUserName);
                if (vbUser == null)
                    return _TempRules;

                var userGroups = vbUser.VBUserGroup_VBUser.ToArray();

                return _TempRules.Where(c => c.VBUserID == vbUser.VBUserID || userGroups.Any(x => x.VBUserGroupID == c.VBUserID)).ToList();
            }
        }

        private List<UserRuleItem> GetStoredRules()
        {
            if (string.IsNullOrEmpty(BSOWorkflowSchedulerBaseRules))
                return new List<UserRuleItem>();

            try
            {
                using (StringReader ms = new StringReader(BSOWorkflowSchedulerBaseRules))
                using (XmlTextReader xmlReader = new XmlTextReader(ms))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(List<UserRuleItem>));
                    List<UserRuleItem> result = serializer.ReadObject(xmlReader) as List<UserRuleItem>;
                    if (result == null)
                        return new List<UserRuleItem>();

                    using (Database db = new core.datamodel.Database())
                    {
                        foreach (UserRuleItem item in result)
                        {
                            core.datamodel.VBGroup vbGroup = null;
                            core.datamodel.VBUser vbUser = db.VBUser.FirstOrDefault(c => c.VBUserID == item.VBUserID);
                            if (vbUser == null)
                            {
                                vbGroup = db.VBGroup.FirstOrDefault(c => c.VBGroupID == item.VBUserID);
                            }

                            if (vbUser != null)
                            {
                                item.VBUserName = vbUser.VBUserName;
                            }
                            else if (vbGroup != null)
                            {
                                item.VBUserName = vbGroup.VBGroupName;
                            }

                            if (string.IsNullOrEmpty(item.VBUserName))
                                continue;

                            MDSchedulingGroup group = DatabaseApp.MDSchedulingGroup.FirstOrDefault(c => c.MDSchedulingGroupID == item.RuleParamID);
                            if (group == null)
                                continue;

                            item.RuleParamCaption = group.ACCaption;
                        }
                    }

                    if (result != null)
                        result = result.OrderBy(c => c.VBUserName).ToList();
                    return result;
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), nameof(GetStoredRules), e);
                return new List<UserRuleItem>();
            }
        }

        #endregion


        #region Transformation - WFsLines

        /// <summary>
        /// InitialBuildLines
        /// </summary>
        /// <exception cref="Exception"></exception>
        [ACMethodInfo("InitialBuildLines", "en{'Initial build lines'}de{'Vorbereite Linien'}", 507)]
        public void InitialBuildLines()
        {
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                var query = databaseApp
               .ACClassWF
               .Where(c =>
                   c.RefPAACClassMethodID.HasValue
                   && c.RefPAACClassID.HasValue
                   && c.RefPAACClassMethod.ACKindIndex == (short)Global.ACKinds.MSWorkflow
                   && c.RefPAACClassMethod.PWACClass != null
                   && (c.RefPAACClassMethod.PWACClass.ACIdentifier == PWNodeProcessWorkflowVB.PWClassName
                       || c.RefPAACClassMethod.PWACClass.ACClass1_BasedOnACClass.ACIdentifier == PWNodeProcessWorkflowVB.PWClassName)
                   && !string.IsNullOrEmpty(c.Comment))
               .ToArray();

                int nr = 0;
                foreach (var item in query)
                {
                    nr++;
                    MDSchedulingGroup group = MDSchedulingGroup.NewACObject(databaseApp, null);
                    string wfName = item.ACCaption;
                    if (string.IsNullOrEmpty(wfName))
                        wfName = item.Comment;
                    group.MDNameTrans = string.Format(@"en{{'{0}'}}de{{'{0}'}}", wfName);
                    group.MDKey = item.ACIdentifier + nr.ToString("00");
                    if (group.MDKey.Length > 40)
                        throw new Exception();
                    MDSchedulingGroupWF groupWf = MDSchedulingGroupWF.NewACObject(databaseApp, group);
                    groupWf.VBiACClassWF = item;
                    databaseApp.MDSchedulingGroup.Add(group);
                }
                LocalSaveChanges();
                OnPropertyChanged(nameof(ScheduleForPWNodeList));
            }
        }

        private bool? _IsEnabledInitialBuildLines;
        public bool IsEnabledInitialBuildLines()
        {
            if (_IsEnabledInitialBuildLines == null)
            {
                _IsEnabledInitialBuildLines = !DatabaseApp.MDSchedulingGroup.Any();
            }
            return _IsEnabledInitialBuildLines ?? false;
        }

        #endregion

        #endregion
    }
}