using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.datamodel;
using System.Threading;
using System.Globalization;
using vd = gip.mes.datamodel;

namespace gip.mes.maintenance
{
    /// <summary>
    /// Convention:
    /// ACMaintService must be added under root component of project(application manager).
    /// Name of virtual component must be "ACMaintService".
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance Service'}de{'Maintenance Service'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, true)]
    public class ACMaintService : PAJobScheduler
    {
        #region c'tors

        static ACMaintService()
        {
            RegisterExecuteHandler(typeof(ACMaintService), HandleExecuteACMethod_ACMaintService);
        }

        public ACMaintService(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            CompWarningList = new List<ACMaintWarning>();
            using (vd.DatabaseApp dbApp = new vd.DatabaseApp())
            {
                if (dbApp.MaintOrder.Any(c => c.MDMaintOrderState.MDMaintOrderStateIndex == (short)vd.MDMaintOrderState.MaintOrderStates.MaintenanceNeeded))
                    IsMaintenanceWarning.ValueT = true;
            }
            return true;
        }

        public override bool ACPostInit()
        {
            ThreadPool.QueueUserWorkItem((object state) => RebuildMaintCacheInternal());
            if (this.ApplicationManager != null)
                this.ApplicationManager.ProjectWorkCycleR1min += ApplicationManager_ProjectWorkCycleR1min;
            (Root as ACRoot).OnSendPropertyValueEvent += OnMaintPropertyChanged;
            Task.Run(() => CheckWarningOnStartUp());
            return base.ACPostInit();
        }


        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this.ApplicationManager.ProjectWorkCycleR1min -= ApplicationManager_ProjectWorkCycleR1min;
            (Root as ACRoot).OnSendPropertyValueEvent -= OnMaintPropertyChanged;
            return base.ACDeInit(deleteACClassTask);
        }

        public const string ClassName = "ACMaintService";

        #endregion


        #region Private Classes

        /// <summary>
        /// Wrapper-Class for Maintenance-Rule (MaintACClass). It helps to resolve and cache the information about .NET-Types and iPlus-ACTypes
        /// </summary>
        private class MaintTypeInfo
        {
            public MaintTypeInfo(vd.MaintACClass maintACClass, ACClass acType)
            {
                _MaintACClass = maintACClass;
                _ACType = acType;
            }

            private vd.MaintACClass _MaintACClass;
            private ACClass _ACType;

            private Type _NETType = null;
            public Type NETType
            {
                get
                {
                    if (_NETType == null)
                        _NETType = _ACType.ObjectType;
                    return _NETType;
                }
            }

            public ACClass ACType
            {
                get
                {
                    return _ACType;
                }
            }

            public vd.MaintACClass MaintACClass
            {
                get
                {
                    return _MaintACClass;
                }
            }

        }

        /// <summary>
        /// Wrapper-Class for holding a reference to each ACComponent-Instance and a reference to a Maintenance-Rule
        /// </summary>
        private class MaintainableInstance
        {
            public MaintainableInstance(ACRef<ACComponent> acComponent, MaintTypeInfo maintInfo)
            {
                _MaintInfo = maintInfo;
                _Instance = acComponent;
            }

            public ACRef<ACComponent> _Instance;
            public ACComponent Instance
            {
                get
                {
                    return _Instance.ValueT;
                }
            }

            private MaintTypeInfo _MaintInfo;
            public MaintTypeInfo MaintInfo
            {
                get
                {
                    return _MaintInfo;
                }
                set
                {
                    _MaintInfo = value;
                }
            }
        }

        #endregion


        #region Properties


        #region Warning
        [ACPropertyBindingSource()]
        public IACContainerTNet<Boolean> IsMaintenanceWarning
        {
            get;
            set;
        }

        private readonly ACMonitorObject _60010_WarningLock = new ACMonitorObject(60010);

        private ManualResetEventSlim _manualResetEventSlim = new ManualResetEventSlim(false);

        private List<ACMaintWarning> _CompWarningList;
        private List<ACMaintWarning> CompWarningList
        {
            get
            {
                return _CompWarningList;
            }
            set
            {
                _CompWarningList = value;
            }
        }

        [ACPropertyBindingSource()]
        public IACContainerTNet<List<ACMaintWarning>> ComponentsWarningList
        {
            get;
            set;
        }

        [ACPropertyBindingSource(210, "Error", "en{'Alarm'}de{'Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> MaintAlarm { get; set; }
        #endregion


        #region Maintenance Configuration Cache

        /// <summary>
        /// Shared Dictionary with all configured Maintenance-Rules over all ACProjects
        /// ACClassID is the key in the Dictionary
        /// </summary>
        static Dictionary<Guid, MaintTypeInfo> _AllMaintainableTypes = null;
        private static object _LockAllMaintainableTypes = new object();
        private static int _MaintainableTypesVersion = 0;
        public static int MaintainableTypesVersion
        {
            get
            {
                return _MaintainableTypesVersion;
            }
        }
        private int _ThisMaintainableTypesVersion = 0;

        /// <summary>
        /// List of Instances, which must be maintainted periodically
        /// </summary>
        Dictionary<int, MaintainableInstance> _MaintainableInstancesTime = null;
        private object _LockMaintainableInstancesTime = new object();

        /// <summary>
        /// Dictionary with Property-ACUrl of each Instance which has to be maintainted event-driven
        /// </summary>
        Dictionary<string, vd.MaintACClassProperty> _MaintConfigForPropertyInstance = null;
        private object _LockMaintConfigForPropertyInstance = new object();

        private DateTime? _RebuildCacheAt = null;
        private bool _CacheRebuilded = false;
        #endregion


        #endregion


        #region Methods

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "SetNewMaintOrderManual":
                    SetNewMaintOrderManual(acParameter[0] as string);
                    return true;
                case "RebuildMaintCache":
                    RebuildMaintCache();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_ACMaintService(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "ShowMaintenanceWarning":
                    ShowMaintenanceWarning(acComponent);
                    return true;
                    //case Const.IsEnabledPrefix + "ShowMaintenanceWarning":
                    //    result = IsEnabledShowMaintenanceWarning(acComponent);
                    //    return true;
            }
            //return HandleExecuteACMethod_PAProcessModule(out result, acComponent, acMethodName, acClassMethod, acParameter);
            return false;
        }
        #endregion



        #region Public

        [ACMethodInfo("", "en{'New maint order manually'}de{'New maint order manually'}", 999, true)]
        public void SetNewMaintOrderManual(string acComponentACUrl)
        {
            ACComponent acComponent = this.ApplicationManager.ACUrlCommand(acComponentACUrl) as ACComponent;
            if (acComponent == null)
                return;
            MaintTypeInfo maintTypeInfo = FindMaintTypeInfo(acComponent);
            if (maintTypeInfo != null)
            {
                using (vd.DatabaseApp dbApp = new vd.DatabaseApp())
                {
                    SetNewMaintOrder(maintTypeInfo.MaintACClass.MaintACClassID, acComponent, dbApp);
                }
            }
        }

        [ACMethodInfo("", "en{'Rebuild maintenance cache'}de{'Wartungscache neu aufbauen'}", 500, true)]
        public void RebuildMaintCache()
        {
            _RebuildCacheAt = DateTime.Now.AddMinutes(5);
        }

        #endregion


        #region Cache building

        private void ApplicationManager_ProjectWorkCycleR1min(object sender, EventArgs e)
        {
            if (_RebuildCacheAt == null || !_CacheRebuilded)
                return;
            if (DateTime.Now < _RebuildCacheAt.Value)
                return;
            _RebuildCacheAt = null;
            ThreadPool.QueueUserWorkItem((object state) => RebuildMaintCacheInternal());
        }

        private void RebuildMaintCacheInternal()
        {
            try
            {
                _CacheRebuilded = false;
                // 1. Rebuild the static shared Rule-Cache
                lock (_LockAllMaintainableTypes)
                {
                    // Another Thread is currently Rebulding the Maintenance-Rule-Cache
                    if (_ThisMaintainableTypesVersion < MaintainableTypesVersion)
                        _ThisMaintainableTypesVersion = MaintainableTypesVersion;
                    // This Service is the first, which should rebuild the Maintenance-Rule-Cache
                    else
                    {
                        _AllMaintainableTypes = new Dictionary<Guid, MaintTypeInfo>();
                        using (var dbApp = new vd.DatabaseApp())
                        {
                            var configuredClassesForMaint = dbApp.MaintACClass
                                                                .Include("MaintACClassProperty_MaintACClass")
                                                                .Include(c => c.MDMaintMode)
                                                                .Where(c => c.IsActive);
                            if (!configuredClassesForMaint.Any())
                                return;
                            foreach (var maintACClass in configuredClassesForMaint)
                            {
                                maintACClass.CopyMaintACClassPropertiesToLocalCache();
                                AddMaintRuleToCache(maintACClass);
                            }
                        }
                        _MaintainableTypesVersion++;
                        _ThisMaintainableTypesVersion = _MaintainableTypesVersion;
                    }
                    if (_AllMaintainableTypes == null || !_AllMaintainableTypes.Any())
                        return;
                }

                // 2. Rebuild the Instance-Cache for this Application-Manager
                lock (_LockMaintConfigForPropertyInstance)
                {
                    _MaintainableInstancesTime = new Dictionary<int, MaintainableInstance>();
                    _MaintConfigForPropertyInstance = new Dictionary<string, vd.MaintACClassProperty>();
                }

                // 2.1 Determine Instances which has to be periodically maintained in this Application and build the cache
                var appManager = ApplicationManager;
                IEnumerable<MaintTypeInfo> maintenanceRules = null;
                lock (_LockAllMaintainableTypes)
                {
                    maintenanceRules = _AllMaintainableTypes.Values.ToArray();
                }
                foreach (var maintRule in maintenanceRules)
                {
                    ApplyTimeBasedRuleToInstances(maintRule, appManager);
                }

                // 2.2 Determine instances which has to be maintained event driven when it's property is relevant von maintenance
                using (var dbApp = new vd.DatabaseApp())
                {
                    var queryEventBasedProps = dbApp.MaintACClassProperty
                                            .Include(c => c.MaintACClass)
                                            .Include(c => c.MaintACClass.MDMaintMode)
                                            .Include(c => c.VBiACClassProperty)
                                            .Where(x => x.IsActive
                                                        && x.MaintACClass.IsActive
                                                        && x.MaintACClass.MDMaintMode.MDMaintModeIndex >= (short)vd.MDMaintMode.MaintModes.EventOnly);

                    foreach (vd.MaintACClassProperty maintACClassProperty in queryEventBasedProps)
                    {
                        ApplyEventBasedRuleToInstances(maintACClassProperty, appManager);
                    }
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                this.Root().Messages.LogException("ACMaintService", "RebuildMaintCacheInternal", msg);
            }
            finally
            {
                _CacheRebuilded = true;
                if (_manualResetEventSlim != null)
                    _manualResetEventSlim.Set();
            }
        }

        private MaintTypeInfo AddMaintRuleToCache(vd.MaintACClass maintACClass)
        {
            MaintTypeInfo typeInfo = null;
            lock (_LockAllMaintainableTypes)
            {
                if (_AllMaintainableTypes == null)
                    return null;

                if (!_AllMaintainableTypes.TryGetValue(maintACClass.VBiACClassID, out typeInfo))
                {
                    ACClass iPlusACClass = maintACClass.GetACClass(this.Root.Database.ContextIPlus);
                    if (iPlusACClass == null)
                        return null;
                    typeInfo = new MaintTypeInfo(maintACClass, iPlusACClass);
                    _AllMaintainableTypes.Add(maintACClass.VBiACClassID, typeInfo);
                }
            }
            return typeInfo;
        }

        private void ApplyTimeBasedRuleToInstances(MaintTypeInfo maintRule, ApplicationManager appManager)
        {
            if (!(maintRule.MaintACClass.MDMaintMode.MaintMode == vd.MDMaintMode.MaintModes.TimeOnly || maintRule.MaintACClass.MDMaintMode.MaintMode == vd.MDMaintMode.MaintModes.TimeAndEvent))
                return;
            IEnumerable<ACComponent> affectedComponents = GetAffectedInstances(maintRule.MaintACClass.VBiACClassID, appManager);
            if (affectedComponents == null || !affectedComponents.Any())
                return;
            foreach (var affectedComponent in affectedComponents)
            {
                MaintainableInstance maintInstance = null;
                lock (_LockMaintainableInstancesTime)
                {
                    if (_MaintainableInstancesTime.TryGetValue(affectedComponent.GetHashCode(), out maintInstance))
                    {
                        // Check if Maintenance-Rule is more concrete because of overriden rule, then replace MaintTypeInfo
                        if (maintRule != maintInstance.MaintInfo
                            && maintRule.ACType.InheritanceLevel > maintInstance.MaintInfo.ACType.InheritanceLevel)
                            maintInstance.MaintInfo = maintRule;
                        else
                            continue;
                    }
                    else
                    {
                        ACRef<ACComponent> affectedRef = new ACRef<ACComponent>(affectedComponent, this);
                        _MaintainableInstancesTime.Add(affectedComponent.GetHashCode(), new MaintainableInstance(affectedRef, maintRule));
                    }
                }
            }
        }

        private void ApplyEventBasedRuleToInstances(vd.MaintACClassProperty maintACClassProperty, ApplicationManager appManager)
        {
            IEnumerable<ACComponent> affectedComponents = GetAffectedInstances(maintACClassProperty.MaintACClass.VBiACClassID, appManager);
            if (affectedComponents == null || !affectedComponents.Any())
                return;

            foreach (ACComponent affectedComponent in affectedComponents)
            {
                IACPropertyBase affectedProperty = affectedComponent.GetProperty(maintACClassProperty.VBiACClassProperty.ACIdentifier);
                if (affectedProperty != null)
                {
                    //int propInstanceHash = affectedProperty.GetHashCode();
                    string propertyUrl = affectedProperty.GetACUrl();
                    lock (_LockMaintConfigForPropertyInstance)
                    {
                        if (!_MaintConfigForPropertyInstance.ContainsKey(propertyUrl))
                        {
                            _MaintConfigForPropertyInstance.Add(propertyUrl, maintACClassProperty);
                        }
                    }
                }
            }
        }

        private IEnumerable<ACComponent> GetAffectedInstances(Guid acClassID, ApplicationManager appManager)
        {
            MaintTypeInfo maintTypeInfo = null;
            lock (_LockAllMaintainableTypes)
            {
                if (_AllMaintainableTypes == null)
                    return null;
                if (!_AllMaintainableTypes.TryGetValue(acClassID, out maintTypeInfo))
                    return null;
            }

            List<ACComponent> affectedInstances = new List<ACComponent>();
            IEnumerable<ACComponent> affectedComponents = appManager.ACCompTypeDict.GetComponentsOfType<ACComponent>(maintTypeInfo.NETType);
            if (affectedComponents == null || !affectedComponents.Any())
                return affectedInstances;
            foreach (var componentToCheck in affectedComponents)
            {
                if (componentToCheck.ComponentClass.IsDerivedClassFrom(maintTypeInfo.ACType))
                {
                    affectedInstances.Add(componentToCheck);
                }
            }
            return affectedInstances;
        }

        private MaintTypeInfo FindMaintTypeInfo(ACComponent acComponent)
        {
            lock (_LockAllMaintainableTypes)
            {
                if (_AllMaintainableTypes == null || !_AllMaintainableTypes.Any())
                    return null;
            }

            List<MaintTypeInfo> maintTypeInfoList = new List<MaintTypeInfo>();

            // Not performant:
            //maintTypeInfoList = _AllMaintainableTypes.Values.Where(c => ((ACClass)acComponent.ACType).IsDerivedClassFrom(c.ACType)).ToList();

            foreach (var acClass in acComponent.ComponentClass.ClassHierarchy)
            {
                MaintTypeInfo maintRule = null;
                lock (_LockAllMaintainableTypes)
                {
                    if (_AllMaintainableTypes.TryGetValue(acClass.ACClassID, out maintRule))
                        maintTypeInfoList.Add(maintRule);
                }
            }
            if (maintTypeInfoList.Any())
                return maintTypeInfoList.OrderByDescending(c => c.ACType.InheritanceLevel).FirstOrDefault();

            return null;
        }

        #endregion


        #region Periodical checks

        protected override void RunJob(DateTime now, DateTime lastRun, DateTime nextRun)
        {
            base.RunJob(now, lastRun, nextRun);
            if (!_CacheRebuilded)
                return;
            ThreadPool.QueueUserWorkItem((object state) => PeriodicalCheck());
        }

        private void PeriodicalCheck()
        {
            ACComponent applicationManager = this.ApplicationManager;
            if (applicationManager == null || _MaintainableInstancesTime == null)
                return;
            lock (_LockAllMaintainableTypes)
            {
                if (!_CacheRebuilded || !_MaintainableInstancesTime.Any())
                    return;
            }

            using (vd.DatabaseApp dbApp = new vd.DatabaseApp())
            {
                lock (_LockAllMaintainableTypes)
                {
                    var openMaintenanceOrders = dbApp.MaintOrder
                                                    .Where(c => c.MDMaintOrderState.MDMaintOrderStateIndex < (short)vd.MDMaintOrderState.MaintOrderStates.MaintenanceCompleted)
                                                    .ToDictionary(c => c.VBiPAACClassID);
                    foreach (MaintainableInstance maintInstance in _MaintainableInstancesTime.Values)
                    {
                        if (maintInstance.Instance == null)
                            continue;
                        vd.MaintOrder maintOrder = null;
                        if (openMaintenanceOrders.TryGetValue(maintInstance.Instance.ComponentClass.ACClassID, out maintOrder))
                            continue;

                        maintOrder = dbApp.MaintOrder.Where(c => c.VBiPAACClassID == maintInstance.Instance.ComponentClass.ACClassID)
                                                                   .OrderByDescending(x => x.MaintActEndDate)
                                                                   .FirstOrDefault();

                        if (maintOrder == null)
                        {
                            if (maintInstance.MaintInfo.MaintACClass.NextMaintTerm.HasValue
                                && DateTime.Now > maintInstance.MaintInfo.MaintACClass.NextMaintTerm)
                            {
                                SetNewMaintOrder(maintInstance.MaintInfo.MaintACClass.MaintACClassID, maintInstance.Instance, dbApp);
                            }
                            else if (maintInstance.MaintInfo.MaintACClass.IsWarningActive
                                && maintInstance.MaintInfo.MaintACClass.NextMaintTerm.HasValue
                                && DateTime.Now >= maintInstance.MaintInfo.MaintACClass.NextMaintTerm.Value.Subtract(TimeSpan.FromDays(maintInstance.MaintInfo.MaintACClass.WarningDiff.Value)))
                                SetMaintenanceWarning(maintInstance.Instance, maintInstance.MaintInfo.MaintACClass.NextMaintTerm.Value);
                        }
                        else if (maintOrder != null && maintOrder.MaintActEndDate.HasValue && maintInstance.MaintInfo.MaintACClass != null && maintInstance.MaintInfo.MaintACClass.MaintInterval.HasValue)
                        {
                            DateTime? nextTerm = maintOrder.MaintActEndDate + TimeSpan.FromDays(maintInstance.MaintInfo.MaintACClass.MaintInterval.Value);
                            if (nextTerm != null && DateTime.Now >= nextTerm.Value)
                            {
                                SetNewMaintOrder(maintInstance.MaintInfo.MaintACClass.MaintACClassID, maintInstance.Instance, dbApp);
                            }
                            else if (maintInstance.MaintInfo.MaintACClass.IsWarningActive && nextTerm != null && DateTime.Now >= nextTerm.Value.Subtract(TimeSpan.FromDays(maintInstance.MaintInfo.MaintACClass.WarningDiff.Value)))
                                SetMaintenanceWarning(maintInstance.Instance, nextTerm.Value);
                        }
                    }
                }
            }
        }

        #endregion


        #region Eventdriven checks

        private void CheckWarningOnStartUp()
        {
            _manualResetEventSlim.Wait(new TimeSpan(0, 5, 0));

            if (!_CacheRebuilded || _MaintConfigForPropertyInstance == null || !this.RunScheduler || !IsEnabledStopScheduling())
                return;

            lock (_LockMaintConfigForPropertyInstance)
            {
                if (_MaintConfigForPropertyInstance == null || !_MaintConfigForPropertyInstance.Any())
                    return;

                foreach (var item in _MaintConfigForPropertyInstance)
                {
                    string componentUrl = item.Key.Substring(0, item.Key.LastIndexOf('\\'));
                    string propertyUrl = item.Key.Substring(item.Key.LastIndexOf('\\') + 1);
                    ACComponent component = this.ApplicationManager.ACUrlCommand(componentUrl) as ACComponent;
                    if (component == null)
                        continue;
                    var value = component.ACUrlCommand(propertyUrl);
                    if (value == null)
                        continue;

                    Type changeValueType = value.GetType();
                    IComparable changedValue = value as IComparable;
                    IComparable maxValue = ACConvert.ChangeType(item.Value.MaxValue, changeValueType, true, gip.core.datamodel.Database.GlobalDatabase) as IComparable;
                    if (changedValue == null || maxValue == null)
                        return;
                    if (item.Value.IsWarningActive)
                    {
                        IComparable warningValue = ACConvert.ChangeType(item.Value.WarningValueDiff, changeValueType, true, gip.core.datamodel.Database.GlobalDatabase) as IComparable;
                        if (warningValue == null)
                            return;
                        if (changedValue.CompareTo(warningValue) >= 0)
                            SetMaintenanceWarning(component, item.Value, propertyUrl, changedValue);
                    }
                }
                _manualResetEventSlim.Dispose();
                _manualResetEventSlim = null;
            }
        }

        private void OnMaintPropertyChanged(object sender, ACPropertyNetSendEventArgs e)
        {
            if (!_CacheRebuilded || _MaintConfigForPropertyInstance == null || !this.RunScheduler || !IsEnabledStopScheduling())
                return;

            if (e.ForACComponent == null
                //|| eventArgs.NetValueEventArgs.Sender != EventRaiser.Source 
                || e.NetValueEventArgs.EventType != EventTypes.ValueChangedInSource
                || !(e.ForACComponent is PAClassAlarmingBase))
                return;

            lock (_LockMaintConfigForPropertyInstance)
            {
                if (_MaintConfigForPropertyInstance == null || !_MaintConfigForPropertyInstance.Any())
                    return;
            }

            if (this.ApplicationManager != ((PAClassAlarmingBase)e.ForACComponent).ApplicationManager)
                return;
            string propertyUrl = e.NetValueEventArgs.ACUrl + "\\" + e.NetValueEventArgs.ACIdentifier;
            vd.MaintACClassProperty configProp = null;
            lock (_LockMaintConfigForPropertyInstance)
            {
                if (!_MaintConfigForPropertyInstance.TryGetValue(propertyUrl, out configProp))
                    return;
            }

            ThreadPool.QueueUserWorkItem((object state) =>
            {
                CheckPropertyValue(e, configProp);
            });
        }

        private void CheckPropertyValue(ACPropertyNetSendEventArgs eventArgs, vd.MaintACClassProperty configProp)
        {
            try
            {
                if (eventArgs.NetValueEventArgs.ChangedValue == null)
                    return;
                Type changeValueType = eventArgs.NetValueEventArgs.ChangedValue.GetType();
                IComparable changedValue = eventArgs.NetValueEventArgs.ChangedValue as IComparable;
                IComparable maxValue = ACConvert.ChangeType(configProp.MaxValue, changeValueType, true, gip.core.datamodel.Database.GlobalDatabase) as IComparable;
                if (changedValue == null || maxValue == null)
                    return;
                if (changedValue.CompareTo(maxValue) >= 0)
                {
                    using (vd.DatabaseApp dbApp = new vd.DatabaseApp())
                    {
                        SetNewMaintOrder(configProp.MaintACClass.MaintACClassID, eventArgs.ForACComponent as ACComponent, dbApp);
                    }
                }

                else if (configProp.IsWarningActive)
                {
                    IComparable warningValue = ACConvert.ChangeType(configProp.WarningValueDiff, changeValueType, true, gip.core.datamodel.Database.GlobalDatabase) as IComparable;
                    if (warningValue == null)
                        return;
                    if (changedValue.CompareTo(warningValue) >= 0)
                        SetMaintenanceWarning(eventArgs.ForACComponent as ACComponent, configProp, eventArgs.NetValueEventArgs.ACIdentifier, changedValue);
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                this.Root().Messages.LogException("ACMaintService", "CheckPropertyValue", msg);
            }
        }

        protected void ResetPropertyValue(ACComponent acComponent, IACPropertyBase acProperty)
        {
            acProperty.ResetToDefaultValue();
            // TODO: Converter for PLC???
        }

        #endregion


        #region Maintenance Order

        protected void SetNewMaintOrder(Guid maintACClassID, ACComponent acComponent, vd.DatabaseApp dbApp)
        {
            GenerateMaintOrder(maintACClassID, acComponent, dbApp);
            ACMaintWarning warning = null;

            using (ACMonitor.Lock(_60010_WarningLock))
            {
                warning = CompWarningList.FirstOrDefault(c => c.ACComponentACUrl == acComponent.ACUrl);
            }
            if (warning != null)
            {
                using (ACMonitor.Lock(_60010_WarningLock))
                {
                    CompWarningList.Remove(warning);
                }
                ComponentsWarningList.ValueT = CompWarningList.ToList();
            }
            //if (!CompWarningList.Any())
            //    IsMaintenanceWarning.ValueT = false;
        }

        private void GenerateMaintOrder(Guid maintACClassID, ACComponent acComponent, vd.DatabaseApp dbApp)
        {

            if (dbApp.MaintOrder.Any(c => c.VBiPAACClassID == acComponent.ComponentClass.ACClassID
                           && c.MDMaintOrderState.MDMaintOrderStateIndex < (short)vd.MDMaintOrderState.MaintOrderStates.MaintenanceCompleted))
                return;

            var maintACClass = dbApp.MaintACClass.Include("MaintACClassProperty_MaintACClass")
                                                .Include("MaintACClassVBGroup_MaintACClass")
                                                .Where(c => c.MaintACClassID == maintACClassID)
                                                .FirstOrDefault();

            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(vd.MaintOrder), vd.MaintOrder.NoColumnName, vd.MaintOrder.FormatNewNo, this);
            vd.MaintOrder maintOrder = vd.MaintOrder.NewACObject(dbApp, null, secondaryKey);
            maintOrder.MaintACClass = maintACClass;
            maintOrder.MDMaintOrderState = dbApp.MDMaintOrderState.FirstOrDefault(c => c.MDMaintOrderStateIndex == (short)vd.MDMaintOrderState.MaintOrderStates.MaintenanceNeeded);
            maintOrder.VBiPAACClassID = acComponent.ComponentClass.ACClassID;
            maintOrder.MDMaintMode = maintACClass.MDMaintMode;
            maintOrder.MaintSetDate = DateTime.Now;

            foreach (vd.MaintACClassVBGroup maintVBGroup in maintACClass.MaintACClassVBGroup_MaintACClass.Where(c => c.IsActive))
            {
                vd.MaintTask maintTask = vd.MaintTask.NewACObject(dbApp, maintOrder);
                maintTask.MaintACClassVBGroup = maintVBGroup;
                maintTask.MDMaintTaskState = dbApp.MDMaintTaskState.FirstOrDefault(c => c.MDMaintTaskStateIndex == (short)vd.MaintTaskState.UnfinishedTask);
            }

            if ((maintACClass.MDMaintMode.MDMaintModeIndex == (short)vd.MDMaintMode.MaintModes.TimeOnly ||
                maintACClass.MDMaintMode.MDMaintModeIndex == (short)vd.MDMaintMode.MaintModes.TimeAndEvent)
                && maintACClass.MaintInterval.HasValue)
                maintOrder.MaintSetDuration = maintACClass.MaintInterval.Value;

            if (maintACClass.MDMaintMode.MDMaintModeIndex == (short)vd.MDMaintMode.MaintModes.EventOnly
                || maintACClass.MDMaintMode.MDMaintModeIndex == (short)vd.MDMaintMode.MaintModes.TimeAndEvent)
            {
                foreach (vd.MaintACClassProperty maintACClassProperty in maintACClass.MaintACClassProperty_MaintACClass.Where(c => c.IsActive))
                {
                    var property = acComponent.GetProperty(maintACClassProperty.VBiACClassProperty.ACIdentifier);
                    if (property != null)
                    {
                        vd.MaintOrderProperty maintOrderProperty = vd.MaintOrderProperty.NewACObject(dbApp, maintOrder);
                        maintOrderProperty.SetValue = maintACClassProperty.MaxValue;
                        if (property.Value != null)
                            maintOrderProperty.ActValue = property.Value.ToString();
                        ResetPropertyValue(acComponent, property);
                        maintOrderProperty.MaintACClassProperty = maintACClassProperty;
                    }
                }
            }

            Msg msg = dbApp.ACSaveChanges();
            if (msg != null)
            {
                if (IsAlarmActive(MaintAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), "GenerateMaintOrder(1)", msg.Message);
                OnNewAlarmOccurred(MaintAlarm, new Msg(msg.Message, this, eMsgLevel.Error, ClassName, "GenerateMaintOrder", 1000), true);
            }
            IsMaintenanceWarning.ValueT = true;
        }

        #endregion


        #region Warning

        /// <summary>
        /// Create Warning for maintenance relevant property
        /// </summary>
        private void SetMaintenanceWarning(ACComponent acComponent, gip.mes.datamodel.MaintACClassProperty maintACClassProperty, string propertyACIdentifier, IComparable changedValue)
        {
            ACMaintWarning warning = GetOrCreateWarningFor(acComponent);
            ACMaintDetailsWarning warningDetail = warning.DetailsList.FirstOrDefault(c => c.ACIdentifier == maintACClassProperty.VBiACClassProperty.ACIdentifier);
            if (warningDetail == null)
            {
                var property = (acComponent as ACComponent).GetProperty(propertyACIdentifier);
                warningDetail = new ACMaintDetailsWarning()
                {
                    ACIdentifier = propertyACIdentifier,
                    ACCaptionTranslation = property.ACCaption,
                    ActualValue = changedValue.ToString(),
                    MaxValue = maintACClassProperty.MaxValue
                };
                warning.DetailsList.Add(warningDetail);
            }
            else
                warningDetail.ActualValue = acComponent.GetValue(maintACClassProperty.VBiACClassProperty.ACIdentifier).ToString();
            ComponentsWarningList.ValueT = CompWarningList.ToList();
            IsMaintenanceWarning.ValueT = true;
        }

        /// <summary>
        /// Create Warning periodic Maintenance
        /// </summary>
        private void SetMaintenanceWarning(ACComponent acComponent, DateTime maintTerm)
        {
            ACMaintWarning warning = GetOrCreateWarningFor(acComponent);
            ACMaintDetailsWarning warningDetail = warning.DetailsList.FirstOrDefault(c => c.ACIdentifier == "MaintenanceInterval");
            if (warningDetail == null)
            {
                warningDetail = new ACMaintDetailsWarning()
                {
                    ACIdentifier = "MaintenanceInterval",
                    ACCaptionTranslation = "en{'Next maintenance at '}de{'Nächste Wartung bei'}",
                    ActualValue = maintTerm.ToShortDateString()
                };
                warning.DetailsList.Add(warningDetail);
            }
            ComponentsWarningList.ValueT = CompWarningList.ToList();
            IsMaintenanceWarning.ValueT = true;
        }

        /// <summary>
        /// Create and add Warning to List
        /// </summary>
        private ACMaintWarning GetOrCreateWarningFor(ACComponent acComponent)
        {
            ACMaintWarning warning = null;

            using (ACMonitor.Lock(_60010_WarningLock))
            {
                warning = CompWarningList.FirstOrDefault(c => c.ACComponentACUrl == acComponent.ACUrl);
            }
            if (warning == null)
            {
                warning = new ACMaintWarning();
                warning.ACComponentACUrl = acComponent.ACUrl;
                warning.DetailsList = new List<ACMaintDetailsWarning>();

                using (ACMonitor.Lock(_60010_WarningLock))
                {
                    CompWarningList.Add(warning);
                }
            }
            return warning;
        }

        #endregion


        #region User-Interaction

        [ACMethodInteractionClient("", "en{'Show maintenance'}de{'Show maintenance'}", 999, true)]
        public static void ShowMaintenanceWarning(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            if (_this == null)
                return;

            PAShowDlgManagerBase serviceInstance = PAShowDlgManagerBase.GetServiceInstance(acComponent.Root as ACComponent);
            if (serviceInstance == null)
                return;

            if (!(bool)serviceInstance.ACUrlCommand("DlgManagerMaint!ShowMaintenaceDialog", acComponent, _this.GetValue("ComponentsWarningList")))
            {
                if (_this.ACIdentifier == ClassName)
                    _this.ACUrlCommand("IsMaintenanceWarning", false);
            }
        }

        #endregion


        #endregion
    }

}
