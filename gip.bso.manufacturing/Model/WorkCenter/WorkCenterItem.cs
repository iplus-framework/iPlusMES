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
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'WorkCenterItem'}de{'WorkCenterItem'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, true)]
    public class WorkCenterItem : IACObject, INotifyPropertyChanged
    {
        #region c'tors

        public WorkCenterItem(ACComponent processModule, BSOWorkCenterSelector bso)
        {
            _ProcessModule = new ACRef<ACComponent>(processModule, bso);
            ItemFunctions = new List<WorkCenterItemFunction>();
            _BSOWorkCenterSelector = bso;
            _UserPWNodeAckInfo = new SafeList<string>();
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

        protected BSOWorkCenterSelector _BSOWorkCenterSelector;
        public BSOWorkCenterSelector BSOWorkCenterSelector => _BSOWorkCenterSelector;

        private string _DefaultLayout = "";
        public string DefaultLayout
        {
            get
            {
                if (string.IsNullOrEmpty(_DefaultLayout))
                {
                    if (BSOWorkCenterSelector != null)
                        _DefaultLayout = BSOWorkCenterSelector.GetDesign("DefaultLayout")?.XMLDesign;
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
                    if (BSOWorkCenterSelector != null)
                        _DefaultTabItemLayout = BSOWorkCenterSelector.GetDesign("DefaultTabItemLayout")?.XMLDesign;
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

        public readonly ACMonitorObject _70100_WFNodesListLock = new ACMonitorObject(70100);

        private ACMonitorObject _70200_ItemFunctionsLock = new ACMonitorObject(70200);

        #endregion

        #region Methods

        public void AddItemFunction(WorkCenterItemFunction function)
        {
            if (function == null)
                return;

            if (function.ACStateProperty != null)
            {
                using (ACMonitor.Lock(_70200_ItemFunctionsLock))
                {
                    ItemFunctions.Add(function);

                    function.ACStateProperty.PropertyChanged += ACStateProperty_PropertyChanged;
                    if (function.NeedWorkProperty != null)
                        function.NeedWorkProperty.PropertyChanged += ACStateProperty_PropertyChanged;

                    function.SetIsFunctionActive();
                    ActiveFunctionsCount = ItemFunctions.Count(c => c.IsFunctionActive);
                }

                if (function.IsReponsibleForUserAck)
                    RegisterItemForUserAck();
            }
            else
            {
                if (BSOWorkCenterSelector != null)
                {
                    string compositionText = "";
                    if (function.RelatedBSOs != null && function.RelatedBSOs.Any())
                        compositionText = function.RelatedBSOs.FirstOrDefault().ACUrlComposition;
                    BSOWorkCenterSelector.Messages.LogWarning(BSOWorkCenterSelector.GetACUrl(), "AddItemFunction()", String.Format("ACStateProperty is null of function {0}", compositionText));
                }
            }
        }

        private void ACStateProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                object senderProp = sender;
                BSOWorkCenterSelector.ApplicationQueue.Add(() => HandleACStatePropertyChanged(senderProp));
            }
        }

        private void HandleACStatePropertyChanged(object sender)
        {
            WorkCenterItemFunction changedFunc = null;
            string exception = null;
            using (ACMonitor.Lock(_70200_ItemFunctionsLock))
            {
                try
                {
                    changedFunc = ItemFunctions?.FirstOrDefault(c => c.ACStateProperty == sender || c.NeedWorkProperty == sender);

                    if (changedFunc == null)
                        return;

                    bool result = changedFunc.SetIsFunctionActive();

                    if (!result && changedFunc.IsReponsibleForUserAck && _UserPWNodeAckInfo.Any())
                    {
                        changedFunc.SetFunctionActive(true);
                    }

                    ActiveFunctionsCount = ItemFunctions.Count(c => c.IsFunctionActive);
                }
                catch (Exception e)
                {
                    exception = e.Message + e.StackTrace;
                }
            }

            if (exception != null)
                BSOWorkCenterSelector.Messages.LogMessage(eMsgLevel.Exception, "WorkCenterItem", "HandleACStatePropertyChanged", exception);
        }

        public void OnItemSelected(BSOWorkCenterSelector parentBSO)
        {
            string dynamicContent = "";

            IOrderedEnumerable<ACComposition> relatedBSOs = null;

            using (ACMonitor.Lock(_70200_ItemFunctionsLock))
            {
                relatedBSOs = ItemFunctions.SelectMany(c => c.RelatedBSOs).Distinct().Where(c => c.ValueT is core.datamodel.ACClass)
                                                                                     .OrderBy(c => (c.ValueT as core.datamodel.ACClass).SortIndex);
            }
            foreach (var bso in relatedBSOs)
            {
                ACBSO acBSO = parentBSO.ACComponentChilds.FirstOrDefault(c => c.ACIdentifier.StartsWith(bso.ACIdentifier)) as ACBSO;
                if (acBSO == null)
                    acBSO = parentBSO.StartComponent(bso.ValueT as core.datamodel.ACClass, null, null) as ACBSO;

                BSOWorkCenterChild selectorChild = acBSO as BSOWorkCenterChild;
                if (selectorChild == null)
                    continue;

                using (ACMonitor.Lock(_70200_ItemFunctionsLock))
                {
                    WorkCenterItemFunction item = ItemFunctions.FirstOrDefault(c => c.RelatedBSOs.Any(x => x == bso));
                    selectorChild.ItemFunction = item;
                }

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
            using (ACMonitor.Lock(_70200_ItemFunctionsLock))
            {
                if (ItemFunctions == null)
                    return;

                foreach (var func in ItemFunctions)
                {
                    if (func.ACStateProperty != null)
                        func.ACStateProperty.PropertyChanged -= ACStateProperty_PropertyChanged;

                    if (func.NeedWorkProperty != null)
                        func.NeedWorkProperty.PropertyChanged -= ACStateProperty_PropertyChanged;

                    if (func._ProcessFunction != null)
                        func._ProcessFunction.Detach();
                    func._ProcessFunction = null;
                    func.ACStateProperty = null;
                }

                ItemFunctions = null;
            }

            if (_ProcessModule != null)
                _ProcessModule.Detach();

            using (ACMonitor.Lock(_70100_WFNodesListLock))
            {
                if (_WFNodes != null)
                {
                    (_WFNodes as IACPropertyNetBase).PropertyChanged -= WFNodes_PropertyChanged;
                    _WFNodes = null;
                }
                _CurrentWFNodesList = null;
                _UserPWNodeAckInfo = null;
            }

            _ProcessModule = null;
            _BSOWorkCenterSelector = null;
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
                BSOWorkCenterSelector?.ApplicationQueue?.Add(() => CheckActivePWNodeUserAck());
            }
        }

        private void CheckActivePWNodeUserAck()
        {
            List<ACChildInstanceInfo> currentWFNodesList;

            using (ACMonitor.Lock(_70100_WFNodesListLock))
            {
                var temp = _WFNodes != null ? _WFNodes.ValueT : null;

                if (_CurrentWFNodesList == temp)
                    return;

                _CurrentWFNodesList = temp;
                currentWFNodesList = _CurrentWFNodesList;
            }

            if (BSOWorkCenterSelector.PWUserAckClasses == null || !BSOWorkCenterSelector.PWUserAckClasses.Any())
                return;

            if (currentWFNodesList == null)
            {
                _UserPWNodeAckInfo.Clear();

                using (ACMonitor.Lock(_70200_ItemFunctionsLock))
                {
                    var func = ItemFunctions?.FirstOrDefault(c => c.IsReponsibleForUserAck);
                    if (func != null && func.IsFunctionActive && func.ACStateProperty != null)
                    {
                        if (!func.CheckIsFunctionActive())
                        {
                            func.SetFunctionActive(false);
                            ActiveFunctionsCount = ItemFunctions.Count(c => c.IsFunctionActive);
                        }
                    }
                }
                return;
            }

            var pwInstanceInfos = currentWFNodesList.Where(c => BSOWorkCenterSelector.PWUserAckClasses.Contains(c.ACType.ValueT));

            var userAckItemsToRemove = _UserPWNodeAckInfo.Where(c => !pwInstanceInfos.Any(x => x.ACUrlParent + "\\" + x.ACIdentifier == c)).ToArray();
            foreach (var itemToRemove in userAckItemsToRemove)
                _UserPWNodeAckInfo.Remove(itemToRemove);


            foreach (var instanceInfo in pwInstanceInfos)
            {
                string instanceInfoACUrl = instanceInfo.ACUrlParent + "\\" + instanceInfo.ACIdentifier;
                if (_UserPWNodeAckInfo.Any(c => c == instanceInfoACUrl))
                    continue;

                var pwNode = ProcessModule.Root.ACUrlCommand(instanceInfoACUrl) as IACComponent;
                if (pwNode == null)
                    continue;

                _UserPWNodeAckInfo.Add(instanceInfoACUrl);
            }

            if (_UserPWNodeAckInfo.Any())
            {
                using (ACMonitor.Lock(_70200_ItemFunctionsLock))
                {
                    var func = ItemFunctions?.FirstOrDefault(c => c.IsReponsibleForUserAck);
                    if (func != null && !func.IsFunctionActive)
                    {
                        func.SetFunctionActive(true);
                        ActiveFunctionsCount = ItemFunctions.Count(c => c.IsFunctionActive);
                    }
                }
            }
            else
            {
                using (ACMonitor.Lock(_70200_ItemFunctionsLock))
                {
                    var func = ItemFunctions.FirstOrDefault(c => c.IsReponsibleForUserAck);
                    if (func != null && func.IsFunctionActive && func.ACStateProperty != null)
                    {
                        if (!func.CheckIsFunctionActive())
                        {
                            func.SetFunctionActive(false);
                            ActiveFunctionsCount = ItemFunctions.Count(c => c.IsFunctionActive);
                        }
                    }
                }
            }
        }

        private SafeList<string> _UserPWNodeAckInfo;

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
            return this.ReflectGetACUrl(rootACObject);
        }

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        #endregion
    }
}
