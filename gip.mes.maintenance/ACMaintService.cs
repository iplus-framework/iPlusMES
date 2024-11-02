// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.datamodel;
using System.Threading;
using System.Globalization;
using gip.mes.datamodel;
using Microsoft.EntityFrameworkCore;

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

        public ACMaintService(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            CompWarningList = new List<ACMaintWarning>();
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                if (dbApp.MaintOrder.Any(c => c.MDMaintOrderState.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceNeeded))
                    IsMaintenanceWarning.ValueT = true;
            }
            return true;
        }

        public override bool ACPostInit()
        {
            ThreadPool.QueueUserWorkItem((object state) => RebuildMaintCacheInternal());
            if (this.ApplicationManager != null)
                this.ApplicationManager.ProjectWorkCycleR20sec += ApplicationManager_ProjectWorkCycle;
            (Root as ACRoot).OnSendPropertyValueEvent += OnMaintPropertyChanged;
            Task.Run(() => CheckWarningOnStartUp());
            return base.ACPostInit();
        }


        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (this.ApplicationManager != null)
                this.ApplicationManager.ProjectWorkCycleR20sec -= ApplicationManager_ProjectWorkCycle;
            (Root as ACRoot).OnSendPropertyValueEvent -= OnMaintPropertyChanged;
            return base.ACDeInit(deleteACClassTask);
        }

        public const string ClassName = nameof(ACMaintService);

        #endregion

        #region Private Classes

        /// <summary>
        /// Wrapper-Class for Maintenance-Rule (MaintACClass). It helps to resolve and cache the information about .NET-Types and iPlus-ACTypes
        /// </summary>
        public class MaintTypeInfo
        {
            public MaintTypeInfo(MaintOrder maintOrderTemplate, core.datamodel.ACClass acType)
            {
                _MaintOrderTemplate = maintOrderTemplate;
                _MaintACClass = maintOrderTemplate.MaintACClass;
                _ACType = acType;
            }

            private MaintOrder _MaintOrderTemplate;
            private MaintACClass _MaintACClass;
            private core.datamodel.ACClass _ACType;

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

            public core.datamodel.ACClass ACType
            {
                get
                {
                    return _ACType;
                }
            }

            public MaintOrder MaintOrderTemplate
            {
                get => _MaintOrderTemplate;
            }

            public MaintACClass MaintACClass
            {
                get => _MaintACClass;
            }

        }

        /// <summary>
        /// Wrapper-Class for holding a reference to each ACComponent-Instance and a reference to a Maintenance-Rule
        /// </summary>
        public class MaintainableInstance
        {
            public MaintainableInstance(ACRef<ACComponent> acComponent, MaintTypeInfo maintInfo)
            {
                _MaintInfo = maintInfo;
                _Instance = acComponent;
                InstanceName = acComponent.ACUrl;
            }

            public MaintainableInstance(MaintTypeInfo maintInfo)
            {
                _MaintInfo = maintInfo;
                InstanceName = maintInfo.MaintOrderTemplate.Facility?.FacilityNo;
            }

            public ACRef<ACComponent> _Instance;
            public ACComponent Instance
            {
                get
                {
                    return _Instance?.ValueT;
                }
            }

            public Facility FacilityInstance
            {
                get
                {
                    return MaintInfo?.MaintOrderTemplate?.Facility;
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

            public string InstanceName
            {
                get;
                set;
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

        [ACPropertyBindingSource(210, "Error", "en{'On new warning alarm'}de{'On new warning alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> OnNewWarningAlarm { get; set; }

        [ACPropertyBindingSource(210, "Error", "en{'Alarm'}de{'Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> MaintAlarm { get; set; }
        #endregion

        #region Maintenance Configuration Cache

        [ACPropertyBindingSource(210, "Error", "en{'On new maintenance order alarm'}de{'On new maintenance order alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> OnNewMaintOrderAlarm { get; set; }

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
        Dictionary<string, MaintACClassProperty> _MaintConfigForPropertyInstance = null;
        private object _LockMaintConfigForPropertyInstance = new object();

        private DateTime? _RebuildCacheAt = null;
        private bool _CacheRebuilded = false;
        #endregion

        #endregion

        #region Methods

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(SetNewMaintOrderManual):
                    SetNewMaintOrderManual(acParameter[0] as string);
                    return true;
                case nameof(RebuildMaintCache):
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
                case nameof(ShowMaintenanceWarning):
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

        //TODO: Add maint order manually (report problem)
        [ACMethodInfo("", "en{'New maint order manually'}de{'New maint order manually'}", 999, true)]
        public void SetNewMaintOrderManual(string acComponentACUrl)
        {
            ACComponent acComponent = this.ApplicationManager.ACUrlCommand(acComponentACUrl) as ACComponent;
            if (acComponent == null)
                return;
            MaintTypeInfo maintTypeInfo = FindMaintTypeInfo(acComponent);
            if (maintTypeInfo != null)
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    //SetNewMaintOrder(maintTypeInfo.MaintACClass.MaintACClassID, acComponent, dbApp);
                }
            }
        }

        [ACMethodInfo("", "en{'Rebuild maintenance cache'}de{'Wartungscache neu aufbauen'}", 500, true)]
        public void RebuildMaintCache()
        {
            _RebuildCacheAt = DateTime.Now.AddMinutes(1);
        }

        #endregion

        #region Cache building

        private void ApplicationManager_ProjectWorkCycle(object sender, EventArgs e)
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
                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            var maintOrderTemplates = dbApp.MaintOrder.Include(c => c.MaintACClass)
                                                                      .Where(c => c.BasedOnMaintOrderID == null);
                            if (!maintOrderTemplates.Any())
                                return;
                            foreach (MaintOrder maintOrderTemplate in maintOrderTemplates)
                            {
                                if (maintOrderTemplate.MaintACClassID.HasValue)
                                    maintOrderTemplate.MaintACClass.CopyMaintACClassPropertiesToLocalCache();

                                AddMaintRuleToCache(maintOrderTemplate);
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
                    _MaintConfigForPropertyInstance = new Dictionary<string, MaintACClassProperty>();
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
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var maintOrderTemplates = dbApp.MaintOrder.Include(c => c.MaintACClass)
                                                              .Where(c => c.BasedOnMaintOrderID == null
                                                                       && c.MaintACClassID.HasValue);


                    foreach (MaintOrder template in maintOrderTemplates)
                    {
                        var queryEventBasedProps = template.MaintACClass.MaintACClassProperty_MaintACClass.Where(c => c.IsActive);

                        foreach (MaintACClassProperty maintACClassProperty in queryEventBasedProps)
                        {
                            ApplyEventBasedRuleToInstances(maintACClassProperty, appManager, template);
                        }
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

        private MaintTypeInfo AddMaintRuleToCache(MaintOrder maintOrderTemplate)
        {
            MaintTypeInfo typeInfo = null;
            lock (_LockAllMaintainableTypes)
            {
                if (_AllMaintainableTypes == null)
                    return null;

                Guid id = Guid.Empty;
                if (maintOrderTemplate.MaintACClassID.HasValue)
                    id = maintOrderTemplate.MaintACClass.VBiACClassID;
                else if (maintOrderTemplate.FacilityID.HasValue)
                    id = maintOrderTemplate.FacilityID.Value;

                if (!_AllMaintainableTypes.TryGetValue(id, out typeInfo))
                {
                    core.datamodel.ACClass iPlusACClass = maintOrderTemplate.MaintACClass?.GetACClass(this.Root.Database.ContextIPlus);
                    if (iPlusACClass == null && maintOrderTemplate.Facility != null)
                    {
                        iPlusACClass = maintOrderTemplate.Facility.ACType as core.datamodel.ACClass;
                    }

                    if (iPlusACClass == null)
                        return null;

                    typeInfo = new MaintTypeInfo(maintOrderTemplate, iPlusACClass);
                    _AllMaintainableTypes.Add(id, typeInfo);
                }
            }
            return typeInfo;
        }

        private void ApplyTimeBasedRuleToInstances(MaintTypeInfo maintRule, ApplicationManager appManager)
        {
            if (maintRule.MaintOrderTemplate.MaintInterval == null)
                return;

            if (maintRule.MaintACClass != null)
            {
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
            else if (maintRule.MaintOrderTemplate.FacilityID.HasValue)
            {
                Facility facility = maintRule.MaintOrderTemplate.Facility;

                lock (_LockMaintainableInstancesTime)
                {
                    MaintainableInstance maintInstance = null;
                    int hash = facility.GetHashCode();
                    if (!_MaintainableInstancesTime.TryGetValue(hash, out maintInstance))
                    {
                        _MaintainableInstancesTime.Add(hash, new MaintainableInstance(maintRule));
                    }
                }
            }
        }

        private void ApplyEventBasedRuleToInstances(MaintACClassProperty maintACClassProperty, ApplicationManager appManager, MaintOrder template)
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
                            maintACClassProperty.MaintOrderTemplate = template;
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
                if (componentToCheck.ComponentClass.IsDerivedClassFrom(maintTypeInfo.ACType) || componentToCheck.ComponentClass == maintTypeInfo.ACType)
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
            //ThreadPool.QueueUserWorkItem((object state) => PeriodicalCheck());
            ApplicationManager.ApplicationQueue.Add(() => PeriodicalCheck());
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

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                lock (_LockAllMaintainableTypes)
                {
                    try
                    {
                        var openMaintenanceOrders = dbApp.MaintOrder
                                                        .Where(c => c.MDMaintOrderState.MDMaintOrderStateIndex < (short)MDMaintOrderState.MaintOrderStates.MaintenanceCompleted
                                                                 && c.BasedOnMaintOrderID != null)
                                                        .ToArray();

                        var openMaintenanceOrdersComp = openMaintenanceOrders.Where(c => c.VBiPAACClassID.HasValue).ToDictionary(c => c.VBiPAACClassID);
                        var openMaintenanceOrdersFacility = openMaintenanceOrders.Where(c => c.FacilityID.HasValue).ToDictionary(c => c.FacilityID);


                        foreach (MaintainableInstance maintInstance in _MaintainableInstancesTime.Values)
                        {
                            MaintOrder maintOrder = null;

                            if (maintInstance.Instance != null)
                            {
                                if (openMaintenanceOrdersComp.TryGetValue(maintInstance.Instance.ComponentClass.ACClassID, out maintOrder))
                                    continue;

                                maintOrder = dbApp.MaintOrder.Where(c => c.VBiPAACClassID == maintInstance.Instance.ComponentClass.ACClassID)
                                                             .OrderByDescending(x => x.UpdateDate)
                                                             .FirstOrDefault();
                            }
                            else if (maintInstance.FacilityInstance != null)
                            {
                                if (openMaintenanceOrdersFacility.TryGetValue(maintInstance.FacilityInstance.FacilityID, out maintOrder))
                                    continue;

                                maintOrder = dbApp.MaintOrder.Where(c => c.FacilityID == maintInstance.FacilityInstance.FacilityID)
                                                             .OrderByDescending(x => x.UpdateDate)
                                                             .FirstOrDefault();
                            }

                            if (maintOrder == null)
                            {
                                if (maintInstance.MaintInfo.MaintOrderTemplate.NextMaintTerm.HasValue
                                    && DateTime.Now > maintInstance.MaintInfo.MaintOrderTemplate.NextMaintTerm)
                                {
                                    SetNewMaintOrder(maintInstance, dbApp);
                                }
                                else if (maintInstance.MaintInfo.MaintOrderTemplate.WarningDiff.HasValue
                                    && maintInstance.MaintInfo.MaintOrderTemplate.NextMaintTerm.HasValue
                                    && DateTime.Now >= maintInstance.MaintInfo.MaintOrderTemplate.NextMaintTerm.Value.Subtract(TimeSpan.FromDays(maintInstance.MaintInfo.MaintOrderTemplate.WarningDiff.Value)))
                                {
                                    SetMaintenanceWarning(maintInstance.Instance, maintInstance.FacilityInstance, maintInstance.MaintInfo.MaintOrderTemplate.NextMaintTerm.Value);
                                }
                            }
                            else if (maintOrder != null && maintOrder.EndDate.HasValue && maintInstance.MaintInfo.MaintOrderTemplate != null
                                                                                       && maintInstance.MaintInfo.MaintOrderTemplate.MaintInterval.HasValue)
                            {
                                DateTime? nextTerm = maintOrder.EndDate + TimeSpan.FromDays(maintInstance.MaintInfo.MaintOrderTemplate.MaintInterval.Value);
                                if (nextTerm != null && DateTime.Now >= nextTerm.Value)
                                {
                                    SetNewMaintOrder(maintInstance, dbApp);
                                }
                                else if (maintInstance.MaintInfo.MaintOrderTemplate.WarningDiff.HasValue && nextTerm != null && DateTime.Now >= nextTerm.Value.Subtract(TimeSpan.FromDays(maintInstance.MaintInfo.MaintOrderTemplate.WarningDiff.Value)))
                                    SetMaintenanceWarning(maintInstance.Instance, maintInstance.FacilityInstance, nextTerm.Value);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Messages.LogException(this.GetACUrl(), nameof(PeriodicalCheck), e);
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
                            SetMaintenanceWarning(component, item.Value, propertyUrl, changedValue, true);
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
                //|| e.NetValueEventArgs.Sender != EventRaiser.Source 
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
            MaintACClassProperty configProp = null;
            lock (_LockMaintConfigForPropertyInstance)
            {
                if (!_MaintConfigForPropertyInstance.TryGetValue(propertyUrl, out configProp))
                    return;
            }

            ApplicationManager.ApplicationQueue.Add(() =>
            {
                CheckPropertyValue(e, configProp);
            });

            //ThreadPool.QueueUserWorkItem((object state) =>
            //{
            //    CheckPropertyValue(e, configProp);
            //});
        }

        private void CheckPropertyValue(ACPropertyNetSendEventArgs eventArgs, MaintACClassProperty configProp)
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
                    using (DatabaseApp dbApp = new DatabaseApp())
                    {
                        SetNewMaintOrder(configProp, eventArgs.ForACComponent as ACComponent, dbApp);
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

                this.Root().Messages.LogException(nameof(ACMaintService), nameof(CheckPropertyValue), msg);
            }
        }

        protected void ResetPropertyValue(ACComponent acComponent, IACPropertyBase acProperty)
        {
            acProperty.ResetToDefaultValue();
            // TODO: Converter for PLC???
        }

        #endregion

        #region Maintenance Order

        protected void SetNewMaintOrder(MaintainableInstance instance, DatabaseApp dbApp)
        {
            GenerateMaintOrder(instance.Instance, instance.FacilityInstance, instance.MaintInfo.MaintOrderTemplate, dbApp);

            ACMaintWarning warning = null;

            using (ACMonitor.Lock(_60010_WarningLock))
            {
                warning = CompWarningList.FirstOrDefault(c => c.InstanceName == instance.InstanceName);
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

        protected void SetNewMaintOrder(MaintACClassProperty maintProperty, ACComponent acComponent, DatabaseApp dbApp)
        {
            GenerateMaintOrder(acComponent, null, maintProperty.MaintOrderTemplate, dbApp);

            ACMaintWarning warning = null;

            using (ACMonitor.Lock(_60010_WarningLock))
            {
                warning = CompWarningList.FirstOrDefault(c => c.InstanceName == acComponent.ACUrl);
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

        private void GenerateMaintOrder(ACComponent instance, Facility facilityInstance, MaintOrder template, DatabaseApp dbApp)
        {
            MaintOrder tempTemplate = template.FromAppContext<MaintOrder>(dbApp);

            if (instance != null && dbApp.MaintOrder.Any(c => c.VBiPAACClassID == instance.ComponentClass.ACClassID
                           && c.BasedOnMaintOrderID == tempTemplate.MaintOrderID
                           && c.MDMaintOrderState.MDMaintOrderStateIndex < (short)MDMaintOrderState.MaintOrderStates.MaintenanceCompleted))
                return;

            if (facilityInstance != null && dbApp.MaintOrder.Any(c => c.FacilityID == facilityInstance.FacilityID
                           && c.BasedOnMaintOrderID == tempTemplate.MaintOrderID
                           && c.MDMaintOrderState.MDMaintOrderStateIndex < (short)MDMaintOrderState.MaintOrderStates.MaintenanceCompleted))
                return;


            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(MaintOrder), MaintOrder.NoColumnName, MaintOrder.FormatNewNo, this);
            MaintOrder maintOrder = MaintOrder.NewACObject(dbApp, null, secondaryKey);
            maintOrder.MaintOrder1_BasedOnMaintOrder = tempTemplate;
            maintOrder.MDMaintOrderState = dbApp.MDMaintOrderState.FirstOrDefault(c => c.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceNeeded);

            if (instance != null)
                maintOrder.VBiPAACClassID = instance.ComponentClass.ACClassID;

            if (tempTemplate.MaintACClassID.HasValue)
            {

                var maintACClass = dbApp.MaintACClass.Include("MaintACClassProperty_MaintACClass")
                                                     .Where(c => c.MaintACClassID == tempTemplate.MaintACClassID)
                                                     .FirstOrDefault();

                maintOrder.MaintACClass = maintACClass;
            }

            foreach (MaintOrderTask task in tempTemplate.MaintOrderTask_MaintOrder)
            {
                MaintOrderTask newTask = MaintOrderTask.NewACObject(dbApp, maintOrder);
                task.CopyTaskValues(newTask);
            }

            foreach (MaintOrderAssignment assignment in tempTemplate.MaintOrderAssignment_MaintOrder)
            {
                MaintOrderAssignment newAssignment = MaintOrderAssignment.NewACObject(dbApp, maintOrder);
                assignment.CopyAssignmentValues(newAssignment);
            }

            if (maintOrder.MaintACClass != null && instance != null)
            {
                foreach (MaintACClassProperty maintACClassProperty in maintOrder.MaintACClass.MaintACClassProperty_MaintACClass.Where(c => c.IsActive))
                {
                    var property = instance.GetProperty(maintACClassProperty.VBiACClassProperty.ACIdentifier);
                    if (property != null)
                    {
                        MaintOrderProperty maintOrderProperty = MaintOrderProperty.NewACObject(dbApp, maintOrder);
                        maintOrderProperty.SetValue = maintACClassProperty.MaxValue;
                        if (property.Value != null)
                            maintOrderProperty.ActValue = property.Value.ToString();
                        ResetPropertyValue(instance, property);
                        maintOrderProperty.MaintACClassProperty = maintACClassProperty;
                    }
                }

            }

            Msg msg = dbApp.ACSaveChanges();
            if (msg != null)
            {
                if (IsAlarmActive(MaintAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), "GenerateMaintOrder(1)", msg.Message);
                OnNewAlarmOccurred(MaintAlarm, new Msg(msg.Message, this, eMsgLevel.Error, ClassName, nameof(GenerateMaintOrder), 1000), true);
            }

            string instanceName = instance?.ACUrl;
            if (string.IsNullOrEmpty(instanceName))
                instanceName = facilityInstance.FacilityNo;

            OnNewAlarmOccurred(OnNewMaintOrderAlarm, new Msg(eMsgLevel.Info, String.Format("New maintenace order {0} is generated for {1}", maintOrder.MaintOrderNo, instanceName)));

            IsMaintenanceWarning.ValueT = true;
        }

        #endregion

        #region Warning

        /// <summary>
        /// Create Warning for maintenance relevant property
        /// </summary>
        private void SetMaintenanceWarning(ACComponent acComponent, gip.mes.datamodel.MaintACClassProperty maintACClassProperty, string propertyACIdentifier, IComparable changedValue, bool onInit = false)
        {
            ACMaintWarning warning = GetOrCreateWarningFor(acComponent.ACUrl);
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

            var tempList = ComponentsWarningList.ValueT;
            ComponentsWarningList.ValueT = CompWarningList.ToList();
            IsMaintenanceWarning.ValueT = true;


            if (!onInit && (tempList == null || !tempList.Contains(warning)))
                OnNewAlarmOccurred(OnNewWarningAlarm, new Msg(eMsgLevel.Info, String.Format("New maintenace warning ({0}) is appeard for {1} {2}", warning.Text, acComponent.ACCaption, acComponent.ACUrl)));
        }

        /// <summary>
        /// Create Warning periodic Maintenance
        /// </summary>
        private void SetMaintenanceWarning(ACComponent instance, Facility facilityInstance, DateTime maintTerm)
        {
            string instanceName = null;
            if (instance != null)
                instanceName = instance.ACUrl;
            else if (facilityInstance != null)
                instanceName = facilityInstance.FacilityNo;

            ACMaintWarning warning = GetOrCreateWarningFor(instanceName);
            ACMaintDetailsWarning warningDetail = warning.DetailsList.FirstOrDefault(c => c.ACIdentifier == "MaintenanceInterval");
            if (warningDetail == null)
            {
                warningDetail = new ACMaintDetailsWarning()
                {
                    ACIdentifier = "MaintenanceInterval",
                    ACCaptionTranslation = "en{'Next maintenance at '}de{'Nächste Wartung bei '}", //TODO: translation and constant
                    ActualValue = maintTerm.ToShortDateString()
                };
                warning.DetailsList.Add(warningDetail);
            }

            var tempList = ComponentsWarningList.ValueT;
            ComponentsWarningList.ValueT = CompWarningList.ToList();
            IsMaintenanceWarning.ValueT = true;

            if (tempList == null || !tempList.Contains(warning))
            {
                OnNewAlarmOccurred(OnNewWarningAlarm, new Msg(eMsgLevel.Info, String.Format("New maintenace warning ({0}) is appeard for {1}", warning.Text, instanceName)));
            }
        }

        /// <summary>
        /// Create and add Warning to List
        /// </summary>
        private ACMaintWarning GetOrCreateWarningFor(string instanceName)
        {
            ACMaintWarning warning = null;

            using (ACMonitor.Lock(_60010_WarningLock))
            {
                warning = CompWarningList.FirstOrDefault(c => c.InstanceName == instanceName);
            }
            if (warning == null)
            {
                warning = new ACMaintWarning();
                warning.InstanceName = instanceName;
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

            if (!(bool)serviceInstance.ACUrlCommand("DlgManagerMaint!ShowMaintenaceDialog", acComponent, _this.GetValue(nameof(ComponentsWarningList))))
            {
                if (_this.ACIdentifier == ClassName)
                    _this.ACUrlCommand("IsMaintenanceWarning", false);
            }
        }

        #endregion

        #endregion
    }
}
