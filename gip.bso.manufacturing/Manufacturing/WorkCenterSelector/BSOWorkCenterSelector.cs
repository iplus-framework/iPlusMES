using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.layoutengine;
using gip.core.manager;
using gip.mes.autocomponent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using gip.mes.datamodel;
using System.Threading;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Work center'}de{'Arbeitsplatz'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    [ACQueryInfo(Const.PackName_VarioManufacturing, Const.QueryPrefix + ClassName, "en{'Work center'}de{'Arbeitsplatz'}", typeof(WorkCenterItem), ClassName, "", "")]
    public class BSOWorkCenterSelector : ACBSOvbNav
    {
        #region c'tors

        public BSOWorkCenterSelector(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool result = base.ACInit(startChildMode);
            if (!result)
                return result;

            _BSOWorkCenterSelectorRules = new ACPropertyConfigValue<string>(this, "BSOWorkCenterSelectorRules", "");

            return result;
        }

        public override bool ACPostInit()
        {
            BuildWorkCenterItems();

            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (TaskPresenter != null)
            {
                TaskPresenter.Unload();
            }
            _TaskPresenter = null;

            if (ProcessModuleOrderInfo != null)
                ProcessModuleOrderInfo.PropertyChanged -= ProcessModuleOrderInfo_PropertyChanged;

            ProcessModuleOrderInfo = null;

            foreach (WorkCenterItem item in WorkCenterItems)
            {
                item.DeInit();
            }

            foreach (BSOWorkCenterChild childBSO in FindChildComponents<BSOWorkCenterChild>(1))
            {
                childBSO.DeActivate();
            }

            BSOManualWeighingType = null;

            return base.ACDeInit(deleteACClassTask);
        }

        public const string ClassName = "BSOWorkCenterSelector";

        #endregion

        #region Properties

        #region Properties => AccessNav

        public override IAccessNav AccessNav { get { return AccessPrimary; } }

        ACAccessNav<WorkCenterItem> _AccessPrimary;

        [ACPropertyAccessPrimary(9999, "WorkCenterItem")]
        public virtual ACAccessNav<WorkCenterItem> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(this, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<WorkCenterItem>("WorkCenterItem", this);

                    //_AccessPrimary.ToNavList(GetRelatedProcessModules(Db));

                    //if (!_IsInManualWeighingAndAdditionMode)
                    //{
                    //    SelectedPWManualWeighing = _AccessPrimary.NavList.FirstOrDefault();
                    //    CurrentPWManualWeighing = SelectedPWManualWeighing;
                    //}
                }
                return _AccessPrimary;
            }
        }

        [ACPropertySelected(601, "WorkCenterItem")]
        public WorkCenterItem SelectedWorkCenterItem
        {
            get
            {
                return AccessPrimary?.Selected;
            }
            set
            {
                if (AccessPrimary == null || AccessPrimary.Selected == value)
                    return;

                CurrentWorkCenterItem = value;
                AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedWorkCenterItem");
            }
        }

        [ACPropertyCurrent(602, "WorkCenterItem")]
        public WorkCenterItem CurrentWorkCenterItem
        {
            get
            {
                return AccessPrimary?.Current;
            }
            set
            {
                if (AccessPrimary == null || AccessPrimary.Current == value)
                    return;

                if (AccessPrimary.Current != null)
                    AccessPrimary.Current.OnItemDeselected();

                AccessPrimary.Current = value;

                if (value != null)
                {
                    AccessPrimary.Current.OnItemSelected(this);
                    CurrentLayout = null;
                    CurrentLayout = AccessPrimary.Current.ItemLayout;
                }
                else
                    TaskPresenter?.Unload();

                OnPropertyChanged("CurrentWorkCenterItem");
            }
        }

        [ACPropertyList(603, "WorkCenterItem")]
        public IEnumerable<WorkCenterItem> WorkCenterItems
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        private ACComponent _CurrentProcessModule;
        public ACComponent CurrentProcessModule
        {
            get => _CurrentProcessModule;
            set
            {
                if (_CurrentProcessModule != value)
                {
                    _CurrentProcessModule = value;
                    if (_CurrentProcessModule != null)
                    {
                        PAProcessModuleACCaption = _CurrentProcessModule.ACCaption;
                        RegisterOnOrderInfoPropChanged(value);
                    }
                    else
                        PAProcessModuleACCaption = null;
                }
            }
        }

        #endregion

        #region Properties => Configuration (Rules)

        private List<WorkCenterRule> _tempRules;

        private core.datamodel.VBUser _SelectedVBUser;
        [ACPropertySelected(604, "VBUser", "en{'User to assign'}de{'Zuweisender Benutzer'}")]
        public core.datamodel.VBUser SelectedVBUser
        {
            get => _SelectedVBUser;
            set
            {
                _SelectedVBUser = value;
                OnPropertyChanged("SelectedVBUser");
            }
        }

        [ACPropertyList(605, "VBUser")]
        public IEnumerable<core.datamodel.VBUser> VBUserList
        {
            get
            {
                return DatabaseApp.ContextIPlus.VBUser/*.Where(c => !c.IsSuperuser)*/.ToArray();
            }
        }

        private ACValueItem _SelectedAssignedProcessModule;
        [ACPropertySelected(606, "ProcessModuleRules", "en{'Assigned process module'}de{'Assigned process module'}")]
        public ACValueItem SelectedAssignedProcessModule
        {
            get => _SelectedAssignedProcessModule;
            set
            {


                _SelectedAssignedProcessModule = value;
                OnPropertyChanged("SelectedAssignedProcessModule");
            }
        }

        private IEnumerable<ACValueItem> _AssignedProcessModulesList;
        [ACPropertyList(607, "ProcessModuleRules")]
        public IEnumerable<ACValueItem> AssignedProcessModulesList
        {
            get => _AssignedProcessModulesList;
            set
            {
                _AssignedProcessModulesList = value;
                OnPropertyChanged("AssignedProcessModulesList");
            }
        }

        private ACValueItem _SelectedAvailableProcessModule;
        [ACPropertySelected(608, "AvailableRules", "en{'Available process module'}de{'Available process module'}")]
        public ACValueItem SelectedAvailableProcessModule
        {
            get => _SelectedAvailableProcessModule;
            set
            {
                _SelectedAvailableProcessModule = value;
                OnPropertyChanged("SelectedAvailableProcessModule");
            }
        }

        private List<ACValueItem> _AvailableProcessModulesList;
        [ACPropertyList(609, "AvailableRules")]
        public List<ACValueItem> AvailableProcessModulesList
        {
            get => _AvailableProcessModulesList;
            set
            {
                _AvailableProcessModulesList = value;
                OnPropertyChanged("AvailableProcessModulesList");
            }
        }

        private ACPropertyConfigValue<string> _BSOWorkCenterSelectorRules;
        [ACPropertyConfig("en{'Work center rules'}de{'Work center rules'}")]
        public string BSOWorkCenterSelectorRules
        {
            get => _BSOWorkCenterSelectorRules.ValueT;
            set
            {
                _BSOWorkCenterSelectorRules.ValueT = value;
                OnPropertyChanged("BSOWorkCenterSelectorRules");
            }
        }

        #endregion

        #region Properties => Workflow

        private VBPresenterTask _TaskPresenter;
        bool _PresenterRightsChecked = false;
        public VBPresenterTask TaskPresenter
        {
            get
            {
                if (_TaskPresenter == null)
                {
                    _TaskPresenter = this.ACUrlCommand("VBPresenterTask(CurrentDesign)") as VBPresenterTask;
                    if (_TaskPresenter == null && !_PresenterRightsChecked)
                        Messages.Error(this, "This user has no rights for viewing workflows. Assign rights for VBPresenterTask in the group management!", true);
                    _PresenterRightsChecked = true;
                }
                return _TaskPresenter;
            }
        }

        #endregion

        #region Properties => Partslist

        private InputComponentItem _SelectedInputComponent;
        [ACPropertySelected(620, "InputComponent")]
        public InputComponentItem SelectedInputComponent
        {
            get => _SelectedInputComponent;
            set
            {
                _SelectedInputComponent = value;
                OnPropertyChanged("SelectedInputComponent");
            }
        }

        private List<InputComponentItem> _InputComponentList;
        [ACPropertyList(621, "InputComponent")]
        public List<InputComponentItem> InputComponentList
        {
            get => _InputComponentList;
            set
            {
                _InputComponentList = value;
                OnPropertyChanged("InputComponentList");
            }
        }

        private string _PAProcessModuleACCaption;

        [ACPropertyInfo(603, "", "en{'Module'}de{'Modul'}")]
        public string PAProcessModuleACCaption
        {
            get => _PAProcessModuleACCaption;
            set
            {
                _PAProcessModuleACCaption = value;
                OnPropertyChanged("PAProcessModuleACCaption");
            }
        }

        #endregion

        #region Properties => OrderInfo

        public IACContainerTNet<string> ProcessModuleOrderInfo;

        private ProdOrderBatch _CurrentBatch;
        public ProdOrderBatch CurrentBatch
        {
            get => _CurrentBatch;
            set
            {
                _CurrentBatch = value;
                LoadPartslist();
            }
        }

        private ProdOrderPartslistPos _EndBatchPos;
        [ACPropertyInfo(604)]
        public ProdOrderPartslistPos EndBatchPos
        {
            get => _EndBatchPos;
            set
            {
                _EndBatchPos = value;
                if (_EndBatchPos == null)
                {
                    ProdOrderProgramNo = null;
                    BatchSeqNo = null;
                    EBPMaterialName = null;
                }
                else
                {
                    ProdOrderProgramNo = _EndBatchPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                    BatchSeqNo = _EndBatchPos.ProdOrderBatch?.BatchSeqNo;
                    EBPMaterialName = _EndBatchPos.ProdOrderPartslist?.Partslist?.Material?.MaterialNo + " " + _EndBatchPos.ProdOrderPartslist?.Partslist?.Material?.MaterialName1;
                }
                OnPropertyChanged("EndBatchPos");
            }
        }

        private string _ProdOrderProgramNo;
        [ACPropertyInfo(605, "", "en{'Order Number'}de{'Auftragsnummer'}")]
        public string ProdOrderProgramNo
        {
            get => _ProdOrderProgramNo;
            set
            {
                _ProdOrderProgramNo = value;
                OnPropertyChanged("ProdOrderProgramNo");
            }
        }

        private int? _BatchSeqNo;
        [ACPropertyInfo(999, "", "en{'Batch'}de{'Batch'}")]
        public int? BatchSeqNo
        {
            get => _BatchSeqNo;
            set
            {
                _BatchSeqNo = value;
                OnPropertyChanged("BatchSeqNo");
            }
        }

        private string _EBPMaterialName;
        [ACPropertyInfo(607, "", "en{'Material'}de{'Material'}")]
        public string EBPMaterialName
        {
            get => _EBPMaterialName;
            set
            {
                _EBPMaterialName = value;
                OnPropertyChanged("EBPMaterialName");
            }
        }

        #endregion

        IACComponent CurrentChildBSO
        {
            get;
            set;
        }

        private string _CurrentLayout;
        [ACPropertyInfo(610)]
        public string CurrentLayout
        {
            get => _CurrentLayout;
            set
            {
                _CurrentLayout = value;
                OnPropertyChanged("CurrentLayout");
            }
        }

        public Type BSOManualWeighingType = typeof(BSOManualWeighing);

        private static core.datamodel.ACClass[] _PWUserAckClasses;
        public static core.datamodel.ACClass[] PWUserAckClasses
        {
            get
            {
                if (_PWUserAckClasses == null)
                {
                    var acClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(PWNodeUserAck));
                    if (acClass != null)
                    {
                        var derivedClasses = acClass.DerivedClasses;
                        derivedClasses.Add(acClass);
                        _PWUserAckClasses = derivedClasses.ToArray();
                    }
                }
                return _PWUserAckClasses;
            }
        }

        #endregion

        #region Methods

        #region Methods => Configuration

        [ACMethodInteraction("", "en{'Configure work center'}de{'Arbeitsplatz konfigurieren'}", 601, true)]
        public void ConfigureBSO()
        {
            _tempRules = GetStoredRules();
            AssignedProcessModulesList = new List<ACValueItem>(_tempRules.Select(x => new ACValueItem(x.VBUserName + " => " + x.ProcessModuleACUrl, x, null)));

            var availablePAFs = s_cQry_GetRelevantPAProcessFunctions(DatabaseApp.ContextIPlus, "PAProcessFunction", Const.KeyACUrl_BusinessobjectList).ToArray();
            AvailableProcessModulesList = availablePAFs.Select(c => c.ACClass1_ParentACClass).Distinct().Select(x => new ACValueItem(x.ACUrlComponent, x.ACUrlComponent, null)).ToList();


            ShowDialog(this, "ConfigurationDialog");
        }

        public bool IsEnabledConfigureBSO()
        {
            return Root.Environment.User.IsSuperuser;
        }

        [ACMethodInfo("", "en{'Add rule'}de{'Additionsregel'}", 602)]
        public void AddRule()
        {
            //Tuple<string, string> ruleValue = SelectedAvailableProcessModule.Value as Tuple<string, string>;

            WorkCenterRule existingRule = AssignedProcessModulesList.Select(x => x.Value as WorkCenterRule).FirstOrDefault(c => c.VBUserName == SelectedVBUser.VBUserName
                                                                                                            && c.ProcessModuleACUrl == SelectedAvailableProcessModule.Value as string);
            if (existingRule != null)
            {
                //if (existingRule.ProcessModuleACUrl == SelectedAvailableProcessModule.Value as string)
                //{
                //    //Info50039:User {0} is already assigned to {1} function. You must first unassign user, then you can assign it again."
                //    Messages.Info(this, "Info50039", false, SelectedVBUser.VBUserName, existingRule.ProcessModuleACUrl);
                //    return;
                //}
                return;
            }

            WorkCenterRule rule = new WorkCenterRule()
            {
                ProcessModuleACUrl = SelectedAvailableProcessModule.Value as string,
                VBUserName = SelectedVBUser.VBUserName,
                //ProcessModulePAFName = ruleValue.Item2
            };
            _tempRules.Add(rule);

            var tempRules = AssignedProcessModulesList.ToList();
            tempRules.Add(new ACValueItem(string.Format("{0} => {1}", rule.VBUserName, rule.ProcessModuleACUrl), rule, null));
            AssignedProcessModulesList = tempRules.ToList();
        }

        public bool IsEnabledAddRule()
        {
            return SelectedVBUser != null && SelectedAvailableProcessModule != null;
        }

        [ACMethodInfo("", "en{'Remove rule'}de{'Regel entfernen'}", 603)]
        public void RemoveRule()
        {
            WorkCenterRule rule = _tempRules.FirstOrDefault(c => c == SelectedAssignedProcessModule.Value);
            if (rule != null)
            {
                _tempRules.Remove(rule);
                var tempRules = AssignedProcessModulesList.ToList();
                tempRules.Remove(SelectedAssignedProcessModule);
                AssignedProcessModulesList = tempRules.ToList();
                SelectedAssignedProcessModule = null;
            }
        }

        public bool IsEnabledRemoveRule()
        {
            return SelectedAssignedProcessModule != null;
        }

        [ACMethodInfo("", "en{'Apply rules and close'}de{'Regeln anwenden und schließen'}", 604)]
        public void ApplyRulesAndClose()
        {
            string xml = "";
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<WorkCenterRule>));
                serializer.WriteObject(xmlWriter, _tempRules);
                xml = sw.ToString();
            }
            BSOWorkCenterSelectorRules = xml;
            var msg = DatabaseApp.ACSaveChanges();

            if (msg != null)
                Messages.Msg(msg);

            CloseTopDialog();
        }

        public bool IsEnabledApplyRulesAndClose()
        {
            return true;
        }

        private List<WorkCenterRule> GetRulesByCurrentUser()
        {
            var result = GetStoredRules();
            return result.Where(c => c.VBUserName == Root.Environment.User.VBUserName).ToList();
        }

        private List<WorkCenterRule> GetStoredRules()
        {
            if (string.IsNullOrEmpty(BSOWorkCenterSelectorRules))
                return new List<WorkCenterRule>();

            using (StringReader ms = new StringReader(BSOWorkCenterSelectorRules))
            using (XmlTextReader xmlReader = new XmlTextReader(ms))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<WorkCenterRule>));
                List<WorkCenterRule> result = serializer.ReadObject(xmlReader) as List<WorkCenterRule>;
                if (result == null)
                    return new List<WorkCenterRule>();
                return result;
            }
        }

        #endregion

        private void RegisterOnOrderInfoPropChanged(ACComponent processModule)
        {
            if(ProcessModuleOrderInfo != null)
                ProcessModuleOrderInfo.PropertyChanged -= ProcessModuleOrderInfo_PropertyChanged;

            ProcessModuleOrderInfo = null;

            ProcessModuleOrderInfo = processModule.GetPropertyNet("OrderInfo") as IACContainerTNet<string>;
            if(ProcessModuleOrderInfo == null)
            {
                //error
                return;
            }

            ProcessModuleOrderInfo.PropertyChanged += ProcessModuleOrderInfo_PropertyChanged;
            string orderInfo = ProcessModuleOrderInfo.ValueT;
            HandleOrderInfoPropChanged(orderInfo);
        }

        private void ProcessModuleOrderInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == Const.ValueT)
            {
                string orderInfo = ProcessModuleOrderInfo.ValueT;
                Task.Run(() => HandleOrderInfoPropChanged(orderInfo));
            }
        }

        private void HandleOrderInfoPropChanged(string orderInfo)
        {
            if (string.IsNullOrEmpty(orderInfo))
            {
                CurrentBatch = null;
                EndBatchPos = null;
            }
            else
            {
                string[] accessArr = (string[])CurrentProcessModule?.ACUrlCommand("!SemaphoreAccessedFrom");
                if (accessArr == null || !accessArr.Any())
                {
                    CurrentBatch = null;
                    EndBatchPos = null;
                    return;
                }

                string pwGroupACUrl = accessArr[0];
                ACComponent pwGroup = Root.ACUrlCommand(pwGroupACUrl) as ACComponent;

                PAOrderInfo currentOrderInfo = pwGroup?.ExecuteMethod("GetPAOrderInfo") as PAOrderInfo;
                if (currentOrderInfo == null)
                {
                    Thread.Sleep(300);
                    currentOrderInfo = Root.ACUrlCommand(pwGroupACUrl + "!GetPAOrderInfo") as PAOrderInfo;
                }
                if (currentOrderInfo != null)
                {
                    PAOrderInfoEntry entry = currentOrderInfo.Entities.FirstOrDefault(c => c.EntityName == ProdOrderBatch.ClassName);
                    if (entry != null)
                    {
                        var pb = DatabaseApp.ProdOrderBatch.FirstOrDefault(c => c.ProdOrderBatchID == entry.EntityID);
                        if (pb != null)
                        {
                            CurrentBatch = pb;
                            EndBatchPos = pb.ProdOrderPartslistPos_ProdOrderBatch.FirstOrDefault(c => c.IsFinalMixureBatch);

                            if (EndBatchPos == null)
                                EndBatchPos = pb.ProdOrderPartslistPos_ProdOrderBatch.OrderByDescending(c => c.Sequence).FirstOrDefault();
                        }
                    }
                    else
                    {
                        entry = currentOrderInfo.Entities.FirstOrDefault(c => c.EntityName == Picking.ClassName);
                        if (entry != null)
                        {
                            var picking = DatabaseApp.Picking.FirstOrDefault(c => c.PickingID == entry.EntityID);
                            if (picking != null)
                            {
                                ProdOrderProgramNo = picking.ACCaption;
                            }
                        }
                    }
                }
            }
        }

        public virtual void BuildWorkCenterItems()
        {
            var configuredRules = GetRulesByCurrentUser();
            var relevantPAFs = s_cQry_GetRelevantPAProcessFunctions(DatabaseApp.ContextIPlus, "PAProcessFunction", Const.KeyACUrl_BusinessobjectList).ToArray().OrderBy(c => c.ACCaption);

            if (configuredRules != null && configuredRules.Any() && !Root.Environment.User.IsSuperuser)
                relevantPAFs = relevantPAFs.Where(c => configuredRules.Any(x => x.ProcessModuleACUrl == c.ACClass1_ParentACClass.ACUrlComponent)).ToArray().OrderBy(c => c.ACCaption);

            if (!relevantPAFs.Any())
                return;

            Type processModuleType = typeof(PAProcessModule);

            List<WorkCenterItem> workCenterItems = new List<WorkCenterItem>();

            foreach (core.datamodel.ACClass paf in relevantPAFs)
            {
                core.datamodel.ACClass processModule = paf.ACClass1_ParentACClass;
                if (processModule == null || !processModuleType.IsAssignableFrom(processModule.ObjectType) || processModule.ACStartTypeIndex != (short)Global.ACStartTypes.Automatic)
                    continue;

                var basePAF = paf.ClassHierarchy.FirstOrDefault(c => c.ACClass1_ParentACClass == null && c.ConfigurationEntries.Any(x => x.KeyACUrl == Const.KeyACUrl_BusinessobjectList));
                if (basePAF == null)
                    continue;

                var BSOs = basePAF.ConfigurationEntries.Where(c => c.KeyACUrl == Const.KeyACUrl_BusinessobjectList).Select(x => x.Value as ACComposition).ToArray();
                if (!BSOs.Any())
                    continue;


                WorkCenterItem workCenterItem = workCenterItems.FirstOrDefault(c => c.ProcessModule.ACUrl == processModule.ACUrlComponent);
                if (workCenterItem == null)
                {
                    ACComponent procModule = ACUrlCommand(processModule.ACUrlComponent) as ACComponent;
                    if (procModule == null)
                        continue;

                    workCenterItem = new WorkCenterItem(procModule, this);
                    workCenterItems.Add(workCenterItem);
                }
                if (workCenterItem.ProcessModule == null)
                    continue;

                BSOs = OnAddFunctionBSOs(paf, BSOs);

                WorkCenterItemFunction func = new WorkCenterItemFunction(workCenterItem.ProcessModule, paf.ACIdentifier);
                func.RelatedBSOs = BSOs.ToList();
                //func.PAFName = paf.ACUrlComponent;

                workCenterItem.AddItemFunction(func);

                if (BSOs.Any(c => BSOManualWeighingType.IsAssignableFrom((c.ValueT as core.datamodel.ACClass)?.ObjectType)))
                {
                    workCenterItem.RegisterItemForUserAck();
                }

                //if (BSOs.Any(c => c.ACIdentifier == "BSOManualWeighing"))
                //    workCenterItem.RegisterItemForUserAck();
            }

            AccessPrimary.ToNavList(workCenterItems.OrderBy(c => c.ACCaption));
            SelectedWorkCenterItem = WorkCenterItems.FirstOrDefault();
        }

        public virtual ACComposition[] OnAddFunctionBSOs(core.datamodel.ACClass pafACClass, ACComposition[] bsos)
        {
            return bsos;
        }

        public override void ACAction(ACActionArgs actionArgs)
        {
            base.ACAction(actionArgs);

            if (actionArgs == null || actionArgs.DropObject == null || string.IsNullOrEmpty(actionArgs.DropObject.VBContent))
                return;

            var childBSO = this.ACComponentChilds.FirstOrDefault(c => c.ACIdentifier == actionArgs.DropObject.VBContent) as BSOWorkCenterChild;
            if (childBSO == null && (actionArgs.DropObject.VBContent != "Workflow" && actionArgs.DropObject.VBContent != "BOM"))
                return;

            if (CurrentChildBSO == childBSO && CurrentProcessModule == CurrentWorkCenterItem.ProcessModule)
                return;

            if (actionArgs.ElementAction == Global.ElementActionType.TabItemActivated)
            {
                if (CurrentChildBSO != null)
                    (CurrentChildBSO as BSOWorkCenterChild)?.DeActivate();

                CurrentChildBSO = childBSO;
                CurrentProcessModule = CurrentWorkCenterItem.ProcessModule;

                if (childBSO != null)
                    childBSO.Activate(CurrentProcessModule);
                else if (actionArgs.DropObject.VBContent == "Workflow")
                {
                    ShowWorkflow();
                }
                else if (actionArgs.DropObject.VBContent == "BOM")
                {
                    LoadPartslist();
                }
            }
        }

        [ACMethodInfo("", "en{'Show/Refresh workflow'}de{'Workflow anzeigen/aktualisieren'}", 610)]
        public void ShowWorkflow()
        {
            string[] accessArr = (string[])CurrentWorkCenterItem?.ProcessModule?.ACUrlCommand("!SemaphoreAccessedFrom");
            if (accessArr == null || !accessArr.Any())
            {
                if (TaskPresenter != null)
                    TaskPresenter.Unload();
                return;
            }

            string wf = accessArr[0];
            if (!string.IsNullOrEmpty(wf))
            {
                var wfInstance = Root.ACUrlCommand(wf) as IACComponentPWNode;
                if (TaskPresenter != null && wfInstance != null && wfInstance.ParentRootWFNode != TaskPresenter.SelectedRootWFNode)
                {
                    TaskPresenter.Unload();
                    TaskPresenter.LoadWFInstance(wfInstance.ParentRootWFNode);
                }
            }
        }

        public bool IsEnabledShowWorkflow()
        {
            return CurrentWorkCenterItem != null && CurrentWorkCenterItem.ProcessModule != null;
        }

        [ACMethodInfo("","en{'Load Bill of material'}de{'Load Bill of material'}", 620, true)]
        public void LoadPartslist()
        {
            if (_CurrentBatch == null)
            {
                InputComponentList = null;
                return;
            }
            else
            {
                _CurrentBatch.ProdOrderPartslistPosRelation_ProdOrderBatch.AutoRefresh();
                var inputList = _CurrentBatch.ProdOrderPartslistPosRelation_ProdOrderBatch
                                                    .Where(c => c.SourceProdOrderPartslistPos != null
                                                            && c.MDProdOrderPartslistPosState != null
                                                            && c.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                                                            && c.TargetQuantityUOM > 0.00001)
                                                    .OrderBy(p => p.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex);

                List<InputComponentItem> inputComponentsList = new List<InputComponentItem>();
                foreach(var relation in inputList)
                {
                    InputComponentItem compItem = new InputComponentItem(relation);
                    OnInputComponentCreated(compItem, relation);
                    inputComponentsList.Add(compItem);
                }

                InputComponentList = inputComponentsList;
            }
        }

        protected virtual void OnInputComponentCreated(InputComponentItem item, ProdOrderPartslistPosRelation relation)
        {

        }

        /// <summary>
        /// With this method you can show additional tabs on the interface according a selected process module.
        /// </summary>
        /// <param name="item">Represents the work center item (process module)</param>
        /// <param name="dynamicContent">Represents the dynamic content on the user interface.</param>
        public virtual void OnWorkcenterItemSelected(WorkCenterItem item, ref string dynamicContent)
        {

        }

        #endregion

        #region Execute helper handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;

            switch (acMethodName)
            {
                case "ConfigureBSO":
                    ConfigureBSO();
                    return true;
                case "IsEnabledConfigureBSO":
                    result = IsEnabledConfigureBSO();
                    return true;
                case "AddRule":
                    AddRule();
                    return true;
                case "IsEnabledAddRule()":
                    result = IsEnabledAddRule();
                    return true;
                case "RemoveRule":
                    RemoveRule();
                    return true;
                case "IsEnabledRemoveRule":
                    result = IsEnabledRemoveRule();
                    return true;
                case "ApplyRulesAndClose":
                    ApplyRulesAndClose();
                    return true;
                case "IsEnabledApplyRulesAndClose":
                    result = IsEnabledApplyRulesAndClose();
                    return true;
                case "ShowWorkflow":
                    ShowWorkflow();
                    return true;
                case "IsEnabledShowWorkflow":
                    result = IsEnabledShowWorkflow();
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region Precompiled Queries

        /// <summary>
        /// Get ACClass by based on base ACClass ACIdentifier. Max depth is 5 levels.
        /// </summary>
        public static readonly Func<Database, string, string, IQueryable<gip.core.datamodel.ACClass>> s_cQry_GetRelevantPAProcessFunctions =
        CompiledQuery.Compile<Database, string, string, IQueryable<gip.core.datamodel.ACClass>>(
            (ctx, pafACIdentifier, configKeyACUrl) => ctx.ACClass.Where(c => (c.BasedOnACClassID.HasValue
                                                            && (c.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier // 1. Ableitungsstufe
                                                                || (c.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                        && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier // 2. Ableitungsstufe
                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                        && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier // 3. Ableitungsstufe
                                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier) // 4. Ableitungsstufe
                                                                                                    || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                        && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier) // 5. Ableitungsstufe
                                                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                            && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier)
                                                                                            )
                                                                                )
                                                                            )
                                                                    )
                                                                )
                                                            )
                                                            && (c.BasedOnACClassID.HasValue
                                                            && (c.ACClass1_BasedOnACClass.ACClassConfig_ACClass.Any(cf => cf.KeyACUrl == configKeyACUrl) // 1. Ableitungsstufe
                                                                || (c.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                        && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClassConfig_ACClass.Any(cf => cf.KeyACUrl == configKeyACUrl) // 2. Ableitungsstufe
                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                        && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClassConfig_ACClass.Any(cf => cf.KeyACUrl == configKeyACUrl) // 3. Ableitungsstufe
                                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClassConfig_ACClass.Any(cf => cf.KeyACUrl == configKeyACUrl) // 4. Ableitungsstufe
                                                                                                    || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                        && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClassConfig_ACClass.Any(cf => cf.KeyACUrl == configKeyACUrl) // 5. Ableitungsstufe
                                                                                            )
                                                                                )
                                                                            )
                                                                    )
                                                                )
                                                            )

            ) && c.ACProject != null && c.ACProject.ACProjectTypeIndex == (short)Global.ACProjectTypes.Application && c.ACStartTypeIndex == (short)Global.ACStartTypes.Automatic))
        );

        #endregion
    }

    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'WorkCenterItem'}de{'WorkCenterItem'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, true)]
    public class WorkCenterItem : IACObject, INotifyPropertyChanged
    {
        #region c'tors

        public WorkCenterItem(ACComponent processModule, BSOWorkCenterSelector bso)
        {
            _ProcessModule = new ACRef<ACComponent>(processModule, bso);
            ItemFunctions = new List<WorkCenterItemFunction>();
            ParentBSO = bso;
            userPWNodeAckInfo = new List<string>();
        }

        #endregion

        #region Properties

        ACRef<ACComponent> _ProcessModule;

        public event PropertyChangedEventHandler PropertyChanged;

        [ACPropertyInfo(100)]
        public ACComponent ProcessModule
        {
            get => _ProcessModule?.ValueT;
        }

        public List<WorkCenterItemFunction> ItemFunctions
        {
            get;
            private set;
        }

        private int _ActiveFunctionsCount;
        [ACPropertyInfo(101)]
        public int ActiveFunctionsCount
        {
            get => _ActiveFunctionsCount;
            set
            {
                _ActiveFunctionsCount = value;
                OnPropertyChanged("ActiveFunctionsCount");
            }
        }

        public BSOWorkCenterSelector ParentBSO;

        private string _DefaultLayout = "";
        public string DefaultLayout
        {
            get
            {
                if (string.IsNullOrEmpty(_DefaultLayout))
                {
                    if (ParentBSO != null)
                        _DefaultLayout = ParentBSO.GetDesign("DefaultLayout")?.XMLDesign;
                }
                return _DefaultLayout;
            }
        }

        private string _DefaultTabItemLayout = "";
        public string DefaultTabItemLayout
        {
            get
            {
                if (string.IsNullOrEmpty(_DefaultTabItemLayout))
                {
                    if (ParentBSO != null)
                        _DefaultTabItemLayout = ParentBSO.GetDesign("DefaultTabItemLayout")?.XMLDesign;
                }
                return _DefaultTabItemLayout;
            }
        }

        public string ItemLayout
        {
            get;
            private set;
        }

        public IACObject ParentACObject => null;

        public IACType ACType => this.ReflectACType();

        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        public string ACIdentifier => ProcessModule?.ACIdentifier;

        public string ACCaption => ProcessModule?.ACCaption;

        private IACContainerT<List<ACChildInstanceInfo>> _WFNodes;

        private List<ACChildInstanceInfo> _CurrentWFNodesList;

        public readonly ACMonitorObject _60200_WFNodesListLock = new ACMonitorObject(60200);
        #endregion

        #region Methods

        public void AddItemFunction(WorkCenterItemFunction function)
        {
            if (function == null)
                return;

            if (function.ACStateProperty != null)
            {
                ItemFunctions.Add(function);
                function.ACStateProperty.PropertyChanged += ACStateProperty_PropertyChanged;
                ActiveFunctionsCount = ItemFunctions.Count(c => (ACStateEnum)c.ACStateProperty.Value == ACStateEnum.SMRunning || (ACStateEnum)c.ACStateProperty.Value == ACStateEnum.SMStarting);

                if (function.ACStateProperty != null && (ACStateEnum)function.ACStateProperty.Value == ACStateEnum.SMRunning || (ACStateEnum)function.ACStateProperty.Value == ACStateEnum.SMStarting)
                    function.IsFunctionActive = true;
                else
                    function.IsFunctionActive = false;
            }
            else
            {
                if (ParentBSO != null)
                {
                    string compositionText = "";
                    if (function.RelatedBSOs != null && function.RelatedBSOs.Any())
                        compositionText = function.RelatedBSOs.FirstOrDefault().ACUrlComposition;
                    ParentBSO.Messages.LogWarning(ParentBSO.GetACUrl(), "AddItemFunction()", String.Format("ACStateProperty is null of function {0}", compositionText));
                }
            }
        }

        private void ACStateProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                foreach (var func in ItemFunctions)
                {
                    if (func.ACStateProperty != null && ((ACStateEnum)func.ACStateProperty.Value == ACStateEnum.SMRunning
                                                         || (ACStateEnum)func.ACStateProperty.Value == ACStateEnum.SMStarting))
                    {
                        func.IsFunctionActive = true;
                    }
                    else
                    {
                        if (func.RelatedBSOs.Any(x => ParentBSO.BSOManualWeighingType.IsAssignableFrom((x.ValueT as core.datamodel.ACClass)?.ObjectType))
                            && ( userPWNodeAckInfo != null && userPWNodeAckInfo.Any()))
                        {
                            continue;
                        }
                        func.IsFunctionActive = false;
                    }
                }
                ActiveFunctionsCount = ItemFunctions.Count(c => c.IsFunctionActive);
            }
        }

        public void OnItemSelected(BSOWorkCenterSelector parentBSO)
        {
            string dynamicContent = "";

            var relatedBSOs = ItemFunctions.SelectMany(c => c.RelatedBSOs).Distinct().Where(c => c.ValueT is core.datamodel.ACClass).OrderBy(c => (c.ValueT as core.datamodel.ACClass).SortIndex);

            foreach (var bso in relatedBSOs)
            {
                ACBSO acBSO = parentBSO.ACComponentChilds.FirstOrDefault(c => c.ACIdentifier.StartsWith(bso.ACIdentifier)) as ACBSO;
                if (acBSO == null)
                    acBSO = parentBSO.StartComponent(bso.ValueT as core.datamodel.ACClass, null, null) as ACBSO;

                BSOWorkCenterChild selectorChild = acBSO as BSOWorkCenterChild;
                if (selectorChild == null)
                    continue;

                WorkCenterItemFunction item = ItemFunctions.FirstOrDefault(c => c.RelatedBSOs.Any(x => x == bso));
                selectorChild.ItemFunction = item;

                if (DefaultTabItemLayout != null)
                    dynamicContent += DefaultTabItemLayout.Replace("[childBSO]", acBSO.ACIdentifier).Replace("[tabItemHeader]", acBSO.ACCaption);
            }

            parentBSO.LoadPartslist();

            parentBSO.OnWorkcenterItemSelected(this, ref dynamicContent);

            ItemLayout = DefaultLayout.Replace("[dynamicContent]", dynamicContent);
        }

        public void OnItemDeselected()
        {

        }

        public void DeInit()
        {
            foreach (var func in ItemFunctions)
            {
                if (func.ACStateProperty != null)
                    func.ACStateProperty.PropertyChanged -= ACStateProperty_PropertyChanged;
                if (func._ProcessFunction != null)
                    func._ProcessFunction.Detach();
                func._ProcessFunction = null;
                func.ACStateProperty = null;
            }

            if (_ProcessModule != null)
                _ProcessModule.Detach();

            if (_WFNodes != null)
            {
                (_WFNodes as IACPropertyNetBase).PropertyChanged -= WFNodes_PropertyChanged;
                _WFNodes = null;
            }
            _CurrentWFNodesList = null;

            _ProcessModule = null;
            ParentBSO = null;
        }

        public void RegisterItemForUserAck()
        {
            var wfNodes = ProcessModule.GetPropertyNet("WFNodes");
            if (wfNodes == null)
                return;

            _WFNodes = wfNodes as IACContainerTNet<List<ACChildInstanceInfo>>;
            (_WFNodes as IACPropertyNetBase).PropertyChanged += WFNodes_PropertyChanged;
            if (_WFNodes.ValueT != null)
                CheckActivePWNodeUserAck();
        }

        private void WFNodes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                Task.Run(() => CheckActivePWNodeUserAck());
            }
        }

        private void CheckActivePWNodeUserAck()
        {
            using (ACMonitor.Lock(_60200_WFNodesListLock))
            {
                var temp = _WFNodes != null ? _WFNodes.ValueT : null;

                if (_CurrentWFNodesList == temp)
                    return;

                _CurrentWFNodesList = temp;
            }

            if (BSOWorkCenterSelector.PWUserAckClasses == null || !BSOWorkCenterSelector.PWUserAckClasses.Any())
                return;

            if (_CurrentWFNodesList == null)
            {
                using (ACMonitor.Lock(_60200_WFNodesListLock))
                {
                    userPWNodeAckInfo.Clear();
                }
                var func = ItemFunctions.FirstOrDefault(c => c.RelatedBSOs.Any(x => x.ACIdentifier == BSOManualWeighing.ClassName));
                if (func != null && func.IsFunctionActive && func.ACStateProperty != null)
                {
                    var acStateProp = (func.ACStateProperty as IACContainerTNet<ACStateEnum>);
                    if (acStateProp != null && acStateProp.ValueT != ACStateEnum.SMRunning && acStateProp.ValueT != ACStateEnum.SMStarting)
                    {
                        func.IsFunctionActive = false;
                        ActiveFunctionsCount = ItemFunctions.Count(c => c.IsFunctionActive);
                    }
                }
                return;
            }

            var pwInstanceInfos = _CurrentWFNodesList.Where(c => BSOWorkCenterSelector.PWUserAckClasses.Contains(c.ACType.ValueT));

            using (ACMonitor.Lock(_60200_WFNodesListLock))
            {
                var userAckItemsToRemove = userPWNodeAckInfo.Where(c => !pwInstanceInfos.Any(x => x.ACUrlParent + "\\" + x.ACIdentifier == c)).ToArray();
                foreach (var itemToRemove in userAckItemsToRemove)
                    userPWNodeAckInfo.Remove(itemToRemove);
            }


            foreach (var instanceInfo in pwInstanceInfos)
            {
                string instanceInfoACUrl = instanceInfo.ACUrlParent + "\\" + instanceInfo.ACIdentifier;
                if (userPWNodeAckInfo.Any(c => c == instanceInfoACUrl))
                    continue;

                var pwNode = ProcessModule.Root.ACUrlCommand(instanceInfoACUrl) as IACComponent;
                if (pwNode == null)
                    continue;

                userPWNodeAckInfo.Add(instanceInfoACUrl);
            }

            if (userPWNodeAckInfo.Any())
            {
                var func = ItemFunctions.FirstOrDefault(c => c.RelatedBSOs.Any(x => ParentBSO.BSOManualWeighingType.IsAssignableFrom((x.ValueT as core.datamodel.ACClass)?.ObjectType)));
                if (func != null && !func.IsFunctionActive)
                {
                    func.IsFunctionActive = true;
                    ActiveFunctionsCount = ItemFunctions.Count(c => c.IsFunctionActive);
                }
            }
            else
            {
                var func = ItemFunctions.FirstOrDefault(c => c.RelatedBSOs.Any(x => ParentBSO.BSOManualWeighingType.IsAssignableFrom((x.ValueT as core.datamodel.ACClass)?.ObjectType)));
                if (func != null && func.IsFunctionActive && func.ACStateProperty != null)
                {
                    var acStateProp = (func.ACStateProperty as IACContainerTNet<ACStateEnum>);
                    if (acStateProp != null && acStateProp.ValueT != ACStateEnum.SMRunning && acStateProp.ValueT != ACStateEnum.SMStarting)
                    {
                        func.IsFunctionActive = false;
                        ActiveFunctionsCount = ItemFunctions.Count(c => c.IsFunctionActive);
                    }
                }
            }
        }

        private List<string> userPWNodeAckInfo;

        public void OnPropertyChanged(string propertyName)
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
            throw new NotImplementedException();
        }

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        #endregion
    }

    [DataContract]
    public class WorkCenterRule
    {
        [DataMember]
        public string VBUserName
        {
            get;
            set;
        }

        [DataMember]
        public string ProcessModuleACUrl
        {
            get;
            set;
        }

        //[DataMember]
        //public List<string> ProcessModulePAFName
        //{
        //    get;
        //    set;
        //}
    }

    public class WorkCenterItemFunction : INotifyPropertyChanged
    {
        public WorkCenterItemFunction(ACComponent parentProcessModule, string PAFACIdentifier)
        {
            ACComponent paf = parentProcessModule.ACUrlCommand(PAFACIdentifier) as ACComponent;
            if (paf != null)
            {
                _ProcessFunction = new ACRef<ACComponent>(paf, parentProcessModule);
                ACStateProperty = ProcessFunction.GetPropertyNet("ACState");
            }
        }

        private bool _IsFunctionActive;
        public bool IsFunctionActive
        {
            get
            {
                return _IsFunctionActive;
            }
            set
            {
                _IsFunctionActive = value;
                OnPropertyChanged("IsFunctionActive");
            }
        }

        public List<ACComposition> RelatedBSOs
        {
            get;
            set;
        }

        internal ACRef<ACComponent> _ProcessFunction;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ACComponent ProcessFunction
        {
            get => _ProcessFunction?.ValueT;
        }

        public IACPropertyNetBase ACStateProperty
        {
            get;
            set;
        }

    }
}
