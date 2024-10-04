using gip.core.autocomponent;
using VD = gip.mes.datamodel;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using gip.mes.autocomponent;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioSystem, VD.ConstApp.PrefParam, Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, false, true)]
    public class BSOPreferredParameters : ACBSOvb
    {
        #region c´tors

        public BSOPreferredParameters(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _UseShortDialog = new ACPropertyConfigValue<bool>(this, nameof(UseShortDialog), false);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            _VarioConfigManager = ConfigManagerIPlus.ACRefToServiceInstance(this);
            _ = UseShortDialog;
            return base.ACInit(startChildMode);
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {

            if (_VarioConfigManager != null)
                ConfigManagerIPlus.DetachACRefFromServiceInstance(this, _VarioConfigManager);
            _VarioConfigManager = null;
            return base.ACDeInit(deleteACClassTask);
        }


        #endregion

        #region Managers

        protected ACRef<ConfigManagerIPlus> _VarioConfigManager = null;
        public ConfigManagerIPlus VarioConfigManager
        {
            get
            {
                if (_VarioConfigManager == null)
                    return null;
                return _VarioConfigManager.ValueT;
            }
        }

        #endregion

        #region Config
        private ACPropertyConfigValue<bool> _UseShortDialog;
        [ACPropertyConfig("en{'Use short dialog'}de{'Kurzdialog verwenden'}")]
        public bool UseShortDialog
        {
            get
            {
                return _UseShortDialog.ValueT;
            }
            set
            {
                _UseShortDialog.ValueT = value;
            }
        }
        #endregion

        #region Properties

        #region Properties -> General

        private Type _TypeOfPWNodeProcessWorkflow;
        protected Type TypeOfPWNodeProcessWorkflow
        {
            get
            {
                if (_TypeOfPWNodeProcessWorkflow == null)
                    _TypeOfPWNodeProcessWorkflow = typeof(PWNodeProcessWorkflow);
                return _TypeOfPWNodeProcessWorkflow;
            }
        }


        public IACConfigStore CurrentConfigStore { get; set; }

        public Dictionary<ACClass, List<Guid>> AllMachines { get; set; } = new Dictionary<ACClass, List<Guid>>();

        public VBDialogResult DialogResult;

        #endregion 

        #region Properties -> PWNode

        #region Properties -> PWNode -> PWNodeParamValue

        private ACConfigParam _SelectedPWNodeParamValue;
        /// <summary>
        /// Selected property for ACValue
        /// </summary>
        /// <value>The selected PWNodeParamValue</value>
        [ACPropertySelected(9999, "PWNodeParamValue", "en{'TODO: PWNodeParamValue'}de{'TODO: PWNodeParamValue'}")]
        public ACConfigParam SelectedPWNodeParamValue
        {
            get
            {
                return _SelectedPWNodeParamValue;
            }
            set
            {
                if (_SelectedPWNodeParamValue != value)
                {
                    _SelectedPWNodeParamValue = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PWNodeMachineList));
                    OnPropertyChanged(nameof(HistoryPWNodeParamValueList));
                    OnPropertyChanged(nameof(PWNodeMethodEditorVisible));

                    if (PWNodeMachineList != null)
                    {
                        SelectedPWNodeMachine = PWNodeMachineList.Where(c => c.ACClassID == (SelectedPWNodeParamValue?.VBiACClassID ?? Guid.Empty)).FirstOrDefault();
                    }
                    else
                    {
                        SelectedPWNodeMachine = null;
                    }
                }
            }
        }

        private List<ACConfigParam> _PWNodeParamValueList;
        /// <summary>
        /// List property for ACValue
        /// </summary>
        /// <value>The PWNodeParamValue list</value>
        [ACPropertyList(9999, "PWNodeParamValue")]
        public List<ACConfigParam> PWNodeParamValueList
        {
            get
            {
                return _PWNodeParamValueList;
            }
        }

        #endregion

        #region Properties -> PWNode -> HistoryPWNodeParamValue

        private IACConfig _SelectedHistoryPWNodeParamValue;
        /// <summary>
        /// Selected property for IACConfig
        /// </summary>
        /// <value>The selected HistoryPWNodeParamValue</value>
        [ACPropertySelected(9999, "HistoryPWNodeParamValue", "en{'TODO: HistoryPWNodeParamValue'}de{'TODO: HistoryPWNodeParamValue'}")]
        public IACConfig SelectedHistoryPWNodeParamValue
        {
            get
            {
                return _SelectedHistoryPWNodeParamValue;
            }
            set
            {
                if (_SelectedHistoryPWNodeParamValue != value)
                {
                    _SelectedHistoryPWNodeParamValue = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// List property for IACConfig
        /// </summary>
        /// <value>The HistoryPWNodeParamValue list</value>
        [ACPropertyList(9999, "HistoryPWNodeParamValue")]
        public BindingList<IACConfig> HistoryPWNodeParamValueList
        {
            get
            {
                return SelectedPWNodeParamValue == null ? null : new BindingList<IACConfig>(SelectedPWNodeParamValue.ConfigurationList);
            }
        }

        #endregion

        #region Properties -> PWNode-> PWNodeMachine

        private ACClass _SelectedPWNodeMachine;
        /// <summary>
        /// Selected property for ACClass
        /// </summary>
        /// <value>The selected PWNodeMachine</value>
        [ACPropertySelected(9999, "PWNodeMachine", "en{'Machine'}de{'Maschine'}")]
        public ACClass SelectedPWNodeMachine
        {
            get
            {
                return _SelectedPWNodeMachine;
            }
            set
            {
                if (_SelectedPWNodeMachine != value)
                {
                    _SelectedPWNodeMachine = value;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// List property for ACClass
        /// </summary>
        /// <value>The PWNodeMachine list</value>
        [ACPropertyList(9999, "PWNodeMachine")]
        public List<ACClass> PWNodeMachineList
        {
            get
            {
                if (SelectedPWNodeParamValue == null)
                {
                    return null;
                }
                return AllMachines.Where(c => c.Value.Contains(SelectedPWNodeParamValue.ACClassWF.ACClassWFID)).Select(c => c.Key).ToList();
            }
        }

        #endregion

        #region Properties -> PWNode ->  Other

        public bool PWNodeMethodEditorVisible
        {
            get
            {
                return IsEnabledDeletePWNodeParamValue();
            }
        }
        #endregion

        #endregion

        #region Properties-> PAFunction

        #region Properties -> PAFunction -> PAFunctionParamValue

        private ACConfigParam _SelectedPAFunctionParamValue;
        /// <summary>
        /// Selected property for ACValue
        /// </summary>
        /// <value>The selected PAFunctionParamValue</value>
        [ACPropertySelected(9999, "PAFunctionParamValue", "en{'TODO: PAFunctionParamValue'}de{'TODO: PAFunctionParamValue'}")]
        public ACConfigParam SelectedPAFunctionParamValue
        {
            get
            {
                return _SelectedPAFunctionParamValue;
            }
            set
            {
                if (_SelectedPAFunctionParamValue != value)
                {
                    _SelectedPAFunctionParamValue = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PAFunctionMachineList));
                    OnPropertyChanged(nameof(HistoryPAFunctionParamValueList));
                    OnPropertyChanged(nameof(PAFunctionMethodEditorVisible));

                    if (PAFunctionMachineList != null)
                    {
                        SelectedPAFunctionMachine = PAFunctionMachineList.Where(c => c.ACClassID == (_SelectedPAFunctionParamValue?.VBiACClassID ?? Guid.Empty)).FirstOrDefault();
                    }
                    else
                    {
                        SelectedPAFunctionMachine = null;
                    }
                }
            }
        }

        private List<ACConfigParam> _PAFunctionParamValueList;
        /// <summary>
        /// List property for ACValue
        /// </summary>
        /// <value>The PAFunctionParamValue list</value>
        [ACPropertyList(9999, "PAFunctionParamValue")]
        public List<ACConfigParam> PAFunctionParamValueList
        {
            get
            {
                return _PAFunctionParamValueList;
            }
        }

        #endregion

        #region Properties -> PAFunction -> HistoryPAFunctionParamValue

        private IACConfig _SelectedHistoryPAFunctionParamValue;
        /// <summary>
        /// Selected property for IACConfig
        /// </summary>
        /// <value>The selected HistoryPAFunctionParamValue</value>
        [ACPropertySelected(9999, "HistoryPAFunctionParamValue", "en{'TODO: HistoryPAFunctionParamValue'}de{'TODO: HistoryPAFunctionParamValue'}")]
        public IACConfig SelectedHistoryPAFunctionParamValue
        {
            get
            {
                return _SelectedHistoryPAFunctionParamValue;
            }
            set
            {
                if (_SelectedHistoryPAFunctionParamValue != value)
                {
                    _SelectedHistoryPAFunctionParamValue = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// List property for IACConfig
        /// </summary>
        /// <value>The HistoryPAFunctionParamValue list</value>
        [ACPropertyList(9999, "HistoryPAFunctionParamValue")]
        public BindingList<IACConfig> HistoryPAFunctionParamValueList
        {
            get
            {

                return SelectedPAFunctionParamValue == null ? null : new BindingList<IACConfig>(SelectedPAFunctionParamValue.ConfigurationList);
            }
        }

        #endregion

        #region Properties -> PAFunction -> PAFunctionMachine

        private ACClass _SelectedPAFunctionMachine;
        /// <summary>
        /// Selected property for ACClass
        /// </summary>
        /// <value>The selected PAFunctionMachine</value>
        [ACPropertySelected(9999, "PAFunctionMachine", "en{'Machine'}de{'Maschine'}")]
        public ACClass SelectedPAFunctionMachine
        {
            get
            {
                return _SelectedPAFunctionMachine;
            }
            set
            {
                if (_SelectedPAFunctionMachine != value)
                {
                    _SelectedPAFunctionMachine = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// List property for ACClass
        /// </summary>
        /// <value>The PAFunctionMachine list</value>
        [ACPropertyList(9999, "PAFunctionMachine")]
        public List<ACClass> PAFunctionMachineList
        {
            get
            {
                if (SelectedPAFunctionParamValue == null)
                {
                    return null;
                }
                return AllMachines.Where(c => c.Value.Contains(SelectedPAFunctionParamValue.ACClassWF.ACClassWFID)).Select(c => c.Key).ToList();
            }
        }
        #endregion

        #region #region Properties -> PAFunction -> Others

        public bool PAFunctionMethodEditorVisible
        {
            get
            {
                return IsEnabledDeletePAFunctionValue();
            }
        }

        #endregion

        #endregion

        #region Properties -> QuickParam
        public const string QuickParam = "QuickParam";

        #region QuickParam
        private IACConfig _SelectedQuickParam;
        /// <summary>
        /// Selected property for IACConfig
        /// </summary>
        /// <value>The selected QuickParam</value>
        [ACPropertySelected(9999, nameof(QuickParam), "en{'TODO: QuickParam'}de{'TODO: QuickParam'}")]
        public IACConfig SelectedQuickParam
        {
            get
            {
                return _SelectedQuickParam;
            }
            set
            {
                if (_SelectedQuickParam != value)
                {
                    _SelectedQuickParam = value;
                    OnPropertyChanged(nameof(SelectedQuickParam));
                }
            }
        }


        private List<IACConfig> _QuickParamList;
        /// <summary>
        /// List property for IACConfig
        /// </summary>
        /// <value>The QuickParam list</value>
        [ACPropertyList(9999, nameof(QuickParam))]
        public List<IACConfig> QuickParamList
        {
            get
            {
                if (_QuickParamList == null)
                    _QuickParamList = new List<IACConfig>();
                return _QuickParamList;
            }
        }

        private List<IACConfig> CloneParamInCurrentConfigStore(IACConfigStore currentConfigStore, List<ACConfigParam> configParams)
        {
            List<IACConfig> aCConfigs = new List<IACConfig>();
            foreach (ACConfigParam configParam in configParams)
            {
                IACConfig newConfigParam = null;
                if (configParam.DefaultConfiguration.ConfigStore == currentConfigStore)
                {
                    newConfigParam = configParam.DefaultConfiguration;
                }
                else
                {
                    newConfigParam = GetNewConfigParam(CurrentConfigStore, configParam, null);
                    newConfigParam.Value = configParam.DefaultConfiguration.Value;
                }

                string lastDefinedExpression =
                    configParam
                    .ConfigurationList
                    .Where(c => !string.IsNullOrEmpty(c.Expression))
                    .OrderByDescending(c => c.ConfigStore.OverridingOrder)
                    .Select(c => c.Expression)
                    .DefaultIfEmpty()
                    .FirstOrDefault();

                newConfigParam.Expression = lastDefinedExpression;

                aCConfigs.Add(newConfigParam);
            }
            return aCConfigs;
        }

        #endregion


        #endregion

        #endregion

        #region Methods

        #region Methods -> ACMehtods

        #region Methods -> ACMehtods -> ParamDialog

        [ACMethodInfo("Dialog", VD.ConstApp.PrefParam, (short)MISort.QueryPreviewDlg)]
        public void ShowParamDialog(Guid acClassWFID, Guid? partslistID, Guid? prodOrderPartslistID, Guid? pickingID)
        {
            Clear();

            string dialogName = "ParamDlg";

            SetupCurrentConfigStore(DatabaseApp, partslistID, prodOrderPartslistID, pickingID);

            (List<ACConfigParam> pwNodeParams, List<ACConfigParam> paFunctionParams) = DoACConfigParams(DatabaseApp, acClassWFID, partslistID, prodOrderPartslistID, pickingID);

            if (UseShortDialog)
            {
                List<IACConfig> pwNodeParamsQuick = CloneParamInCurrentConfigStore(CurrentConfigStore, pwNodeParams);
                List<IACConfig> paFunctionParamsQuick = CloneParamInCurrentConfigStore(CurrentConfigStore, paFunctionParams);

                _QuickParamList = new List<IACConfig>();
                _QuickParamList.AddRange(pwNodeParamsQuick);
                _QuickParamList.AddRange(paFunctionParamsQuick);

                SelectedQuickParam = _QuickParamList.FirstOrDefault();

                dialogName = "QuickParamDlg";
            }
            else
            {
                _PWNodeParamValueList = pwNodeParams;
                _PAFunctionParamValueList = paFunctionParams;

                OnPropertyChanged(nameof(PWNodeParamValueList));
                OnPropertyChanged(nameof(PAFunctionParamValueList));
            }

            ShowDialog(this, dialogName);
        }

        [ACMethodInfo("Dialog", VD.ConstApp.PrefParam, (short)MISort.QueryPreviewDlg)]
        public bool ShowParamDialogResult(Guid acClassWFID, Guid? partslistID, Guid? prodOrderPartslistID, Guid? pickingID)
        {
            DialogResult = new VBDialogResult() { SelectedCommand = eMsgButton.Cancel };

            ShowParamDialog(acClassWFID, partslistID, prodOrderPartslistID, pickingID);
            
            bool success = DialogResult.SelectedCommand == eMsgButton.OK;
            DialogResult = null;

            return success;
        }

        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay)]
        public void DialogOK()
        {
            bool successSave = ACSaveOrUndoChanges();

            if(DialogResult != null)
            {
                DialogResult.SelectedCommand = eMsgButton.OK;
            }

            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel)]
        public void DialogCancel()
        {
            CloseTopDialog();
        }

        #endregion

        #endregion

        #region Methods -> PWNode

        [ACMethodInteraction("PWNodeParamValue", "en{'Override parameter'}de{'Parameter überschreiben'}", (short)MISort.InsertData, true, nameof(SelectedPWNodeParamValue))]
        public void CreatePWNodeValue()
        {
            if (!IsEnabledCreatePWNodeValue())
            {
                return;
            }
            if (SelectedPWNodeMachine != null && (SelectedPWNodeParamValue.VBiACClassID == null
                || SelectedPWNodeParamValue.VBiACClassID != SelectedPWNodeMachine.ACClassID))
            {
                var tmpParamList = PWNodeParamValueList.ToList();
                int currentIndex = tmpParamList.IndexOf(SelectedPWNodeParamValue);
                ACConfigParam additionalParam = GetConfigParamWithMachine(SelectedPWNodeParamValue, SelectedPWNodeMachine);
                tmpParamList.Insert(++currentIndex, additionalParam);
                _PWNodeParamValueList = tmpParamList;
                OnPropertyChanged(nameof(PWNodeParamValueList));
                SelectedPWNodeParamValue = additionalParam;
            }

            SelectedPWNodeParamValue.DefaultConfiguration = GetNewConfigParam(CurrentConfigStore, SelectedPWNodeParamValue, SelectedPAFunctionMachine);

            SelectedPWNodeParamValue.ConfigurationList.Insert(0, SelectedPWNodeParamValue.DefaultConfiguration);
            OnPropertyChanged(nameof(HistoryPWNodeParamValueList));
            OnPropertyChanged(nameof(SelectedPWNodeParamValue));
            OnPropertyChanged(nameof(PWNodeMethodEditorVisible));
        }

        public bool IsEnabledCreatePWNodeValue()
        {
            return
                SelectedPWNodeParamValue != null
                && (
                       SelectedPWNodeParamValue.DefaultConfiguration == null
                       ||
                       (
                            SelectedPWNodeParamValue.DefaultConfiguration.ConfigStore != null
                            &&
                            (
                                SelectedPWNodeParamValue.DefaultConfiguration.ConfigStore.GetACUrl() != CurrentConfigStore.GetACUrl()
                                || ((SelectedPWNodeParamValue.DefaultConfiguration.VBiACClassID == null) && (SelectedPWNodeMachine != null))
                            )
                        )
                );
        }

        [ACMethodInteraction("PWNodeParamValue", "en{'Remove overridden parameter'}de{'Überschriebenen Parameter entfernen'}", (short)MISort.Delete, true, nameof(SelectedPWNodeParamValue))]
        public void DeletePWNodeParamValue()
        {
            if (!IsEnabledDeletePWNodeParamValue())
            {
                return;
            }

            DeleteConfigParam(SelectedPWNodeParamValue);

            ACSaveChanges();
            OnPropertyChanged(nameof(SelectedPWNodeParamValue));
            OnPropertyChanged(nameof(HistoryPWNodeParamValueList));
            OnPropertyChanged(nameof(PWNodeMethodEditorVisible));
        }

        public bool IsEnabledDeletePWNodeParamValue()
        {
            return SelectedPWNodeParamValue != null
            && SelectedPWNodeParamValue.DefaultConfiguration != null
            && SelectedPWNodeParamValue.DefaultConfiguration.ConfigStore == CurrentConfigStore;
        }

        #endregion

        #region Methods -> PAFunction

        [ACMethodInteraction("PAFunctionParamValue", "en{'Override parameter'}de{'Parameter überschreiben'}", (short)MISort.InsertData, true, nameof(SelectedPAFunctionParamValue))]
        public void CreatePAFunctionValue()
        {
            if (!IsEnabledCreatePAFunctionValue())
            {
                return;
            }

            if (SelectedPAFunctionMachine != null && (SelectedPAFunctionParamValue.VBiACClassID == null
               || SelectedPAFunctionParamValue.VBiACClassID != SelectedPAFunctionMachine.ACClassID))
            {
                var tmpParamList = PAFunctionParamValueList.ToList();
                int currentIndex = tmpParamList.IndexOf(SelectedPAFunctionParamValue);
                ACConfigParam additionalParam = GetConfigParamWithMachine(SelectedPAFunctionParamValue, SelectedPAFunctionMachine);
                tmpParamList.Insert(++currentIndex, additionalParam);
                _PAFunctionParamValueList = tmpParamList;
                OnPropertyChanged(nameof(PAFunctionParamValueList));
                SelectedPAFunctionParamValue = additionalParam;
            }
            Guid? vbiACClassID = SelectedPAFunctionMachine != null ? SelectedPAFunctionMachine.ACClassID : (Guid?)null;
            if (vbiACClassID == null && SelectedPAFunctionParamValue.VBiACClassID != null)
                vbiACClassID = SelectedPAFunctionParamValue.VBiACClassID;

            string localParamConfigACUrl = SelectedPAFunctionParamValue.LocalConfigACUrl + @"\" + SelectedPAFunctionParamValue.ACMehtodACIdentifier + @"\" + SelectedPAFunctionParamValue.ACIdentifier;


            SelectedPAFunctionParamValue.DefaultConfiguration =
                ConfigManagerIPlus
                .ACConfigFactory(
                    CurrentConfigStore,
                    SelectedPAFunctionParamValue,
                    SelectedPAFunctionParamValue.PreConfigACUrl,
                    localParamConfigACUrl,
                    vbiACClassID);
            SelectedPAFunctionParamValue.ConfigurationList.Insert(0, SelectedPAFunctionParamValue.DefaultConfiguration);
            OnPropertyChanged(nameof(SelectedPAFunctionParamValue));
            OnPropertyChanged(nameof(HistoryPAFunctionParamValueList));
            OnPropertyChanged(nameof(PAFunctionMethodEditorVisible));
        }

        public bool IsEnabledCreatePAFunctionValue()
        {
            return
                SelectedPAFunctionParamValue != null &&
                (
                    SelectedPAFunctionParamValue.DefaultConfiguration == null
                    ||
                        (
                            SelectedPAFunctionParamValue.DefaultConfiguration.ConfigStore != null
                            &&
                            (
                                SelectedPAFunctionParamValue.DefaultConfiguration.ConfigStore.GetACUrl() != CurrentConfigStore.GetACUrl()) ||
                                ((SelectedPAFunctionParamValue.DefaultConfiguration.VBiACClassID == null) && (SelectedPAFunctionMachine != null))
                            )
                );
        }

        [ACMethodInteraction("PAFunctionParamValue", "en{'Remove overridden parameter'}de{'Überschriebenen Parameter entfernen'}", (short)MISort.Delete, true, nameof(SelectedPAFunctionParamValue))]
        public void DeletePAFunctionValue()
        {
            if (!IsEnabledDeletePAFunctionValue())
            {
                return;
            }

            DeleteConfigParam(SelectedPAFunctionParamValue);

            ACSaveChanges();
            OnPropertyChanged(nameof(SelectedPAFunctionParamValue));
            OnPropertyChanged(nameof(HistoryPAFunctionParamValueList));
            OnPropertyChanged(nameof(PAFunctionMethodEditorVisible));
        }

        public bool IsEnabledDeletePAFunctionValue()
        {
            return SelectedPAFunctionParamValue != null
            && SelectedPAFunctionParamValue.DefaultConfiguration != null
            && SelectedPAFunctionParamValue.DefaultConfiguration.ConfigStore == CurrentConfigStore;
        }

        #endregion

        #endregion

        #region Private methods

        private (List<ACConfigParam> pwNodeParams, List<ACConfigParam> paFunctionParams) DoACConfigParams(VD.DatabaseApp databaseApp, Guid acClassWFID, Guid? partslistID, Guid? prodOrderPartslistID, Guid? pickingID)
        {
            WFGroupStartData wFGroupStartData = new WFGroupStartData(databaseApp, VarioConfigManager, acClassWFID, partslistID, prodOrderPartslistID, pickingID);

            List<IACConfig> allConfigs = wFGroupStartData.ConfigStores.SelectMany(c => c.ConfigurationEntries).ToList();
            List<IACConfig> configsWithExpression = allConfigs.Where(c => !string.IsNullOrEmpty(c.Expression)).ToList();

            string preConfigACUrl = ""; //wFGroupStartData.InvokerPWNode.ConfigACUrl;

            WFWrapper wFWrapper = GetWrapper(preConfigACUrl, wFGroupStartData.InvokerPWNode);
            ProcessWF(wFWrapper, configsWithExpression, allConfigs);

            List<ACConfigParam> pwNodeParams = new List<ACConfigParam>();
            List<ACConfigParam> paFunctionParams = new List<ACConfigParam>();

            FillParams(wFWrapper, pwNodeParams, paFunctionParams);

            if (CurrentConfigStore != null)
            {
                Dictionary<ACClass, List<Guid>> machines = new Dictionary<ACClass, List<Guid>>();
                FillMachines(wFWrapper, machines);
                AllMachines = machines;
            }

            return (pwNodeParams, paFunctionParams);
        }


        private WFWrapper GetWrapper(string preConfigACUrl, ACClassWF aCClassWF)
        {
            WFWrapper wFWrapper = new WFWrapper();
            wFWrapper.WF = aCClassWF;
            wFWrapper.PreConfigACUrl = preConfigACUrl;
            wFWrapper.LocalConfigACUrl = aCClassWF.ConfigACUrl;

            string childpreConfigACUrl = wFWrapper.LocalConfigACUrl + "\\";
            if (!string.IsNullOrEmpty(preConfigACUrl))
            {
                childpreConfigACUrl = preConfigACUrl + "\\" + childpreConfigACUrl;
            }

            if (aCClassWF.RefPAACClassMethod != null)
            {
                List<ACClassWF> allSubWf = aCClassWF.RefPAACClassMethod.ACClassWF_ACClassMethod.ToList();
                foreach (ACClassWF subWf in allSubWf)
                {
                    WFWrapper chWf = GetWrapper(childpreConfigACUrl, subWf);
                    wFWrapper.ChildWF.Add(chWf);
                }
            }

            return wFWrapper;
        }

        private void ProcessWF(WFWrapper wFWrapper, List<IACConfig> configsWithExpression, List<IACConfig> allConfigs)
        {
            wFWrapper.MatchedConfigs =
                configsWithExpression
                .Where(c =>
                    //(
                    //    (string.IsNullOrEmpty(c.PreConfigACUrl) && string.IsNullOrEmpty(wFWrapper.PreConfigACUrl))
                    //    || c.PreConfigACUrl == wFWrapper.PreConfigACUrl
                    //)
                    //&& 
                    c.LocalConfigACUrl.StartsWith(wFWrapper.LocalConfigACUrl)
                )
                .ToList();

            if (wFWrapper.MatchedConfigs.Any())
            {
                (ACClassMethod pwNodeMehtod, ACClassMethod paFunctionMethod) = GetWFMethods(wFWrapper.WF);
                wFWrapper.PWNodeParams = GetACConfigParams(wFWrapper.WF, pwNodeMehtod.ACMethod, wFWrapper.PreConfigACUrl, wFWrapper.LocalConfigACUrl, wFWrapper.MatchedConfigs, allConfigs);
                if (paFunctionMethod != null)
                {
                    wFWrapper.PAFunctionParams = GetACConfigParams(wFWrapper.WF, paFunctionMethod.ACMethod, wFWrapper.PreConfigACUrl, wFWrapper.LocalConfigACUrl, wFWrapper.MatchedConfigs, allConfigs);
                }
            }

            foreach (WFWrapper chWf in wFWrapper.ChildWF)
            {
                ProcessWF(chWf, configsWithExpression, allConfigs);
            }
        }

        private void FillParams(WFWrapper wFWrapper, List<ACConfigParam> pwNodeParams, List<ACConfigParam> paFunctionParams)
        {
            if (wFWrapper.PWNodeParams != null && wFWrapper.PWNodeParams.Any())
            {
                pwNodeParams.AddRange(wFWrapper.PWNodeParams);
            }

            if (wFWrapper.PAFunctionParams != null && wFWrapper.PAFunctionParams.Any())
            {
                paFunctionParams.AddRange(wFWrapper.PAFunctionParams);
            }

            foreach (WFWrapper chWf in wFWrapper.ChildWF)
            {
                FillParams(chWf, pwNodeParams, paFunctionParams);
            }
        }

        private void FillMachines(WFWrapper wFWrapper, Dictionary<ACClass, List<Guid>> machines)
        {
            List<ACClass> currentMachines = GetMachnies(wFWrapper.WF);
            if (currentMachines != null && currentMachines.Any())
            {
                foreach (ACClass cls in currentMachines)
                {
                    if (!machines.Keys.Contains(cls))
                    {
                        machines[cls] = new List<Guid>();
                    }

                    if (!machines[cls].Contains(wFWrapper.WF.ACClassWFID))
                    {
                        machines[cls].Add(wFWrapper.WF.ACClassWFID);
                    }
                }
            }

            if (wFWrapper.ChildWF != null)
            {
                foreach (WFWrapper child in wFWrapper.ChildWF)
                {
                    FillMachines(child, machines);
                }
            }
        }

        private List<ACConfigParam> GetACConfigParams(ACClassWF acClassWF, ACMethod acMethod, string preConfigACUrl, string localConfigACUrl, List<IACConfig> matchedConfigs, List<IACConfig> allConfigs)
        {
            List<ACConfigParam> aCConfigParams = new List<ACConfigParam>();

            List<string> listOfProperies = acMethod.ParameterValueList.Select(x => localConfigACUrl + @"\" + x.ACIdentifier).ToList();

            foreach (ACValue aCValue in acMethod.ParameterValueList)
            {
                string localParamConfigACUrl = localConfigACUrl + @"\" + acMethod.ACIdentifier + @"\" + aCValue.ACIdentifier;
                bool existConfig = matchedConfigs.Where(c => c.LocalConfigACUrl == localParamConfigACUrl).Any();

                if (existConfig)
                {
                    ACConfigParam aCConfigParam = new ACConfigParam();

                    aCConfigParam.PreConfigACUrl = preConfigACUrl;
                    aCConfigParam.LocalConfigACUrl = localConfigACUrl;

                    aCConfigParam.ACIdentifier = aCValue.ACIdentifier;
                    aCConfigParam.ACMehtodACIdentifier = acMethod.ACIdentifier;
                    aCConfigParam.ACCaption = acMethod.GetACCaptionForACIdentifier(aCValue.ACIdentifier);
                    aCConfigParam.ValueTypeACClassID = aCValue.ValueTypeACClass != null ? aCValue.ValueTypeACClass.ACClassID : Guid.Empty;
                    aCConfigParam.ACClassWF = acClassWF;
                    aCConfigParam.ConfigurationList =
                        allConfigs
                        .Where(c =>
                                //c.PreConfigACUrl == preConfigACUrl 
                                //&& 
                                c.VBiACClassID == null
                                && c.LocalConfigACUrl == localParamConfigACUrl
                         )
                        .OrderByDescending(c => c.ConfigStore.OverridingOrder)
                        .ToList();

                    aCConfigParam.DefaultConfiguration = aCConfigParam.ConfigurationList.OrderByDescending(x => x.ConfigStore.OverridingOrder).FirstOrDefault();

                    aCConfigParams.Add(aCConfigParam);

                    var machineConfigs = allConfigs.Where(c => c.LocalConfigACUrl == localParamConfigACUrl && c.VBiACClassID != null);

                    foreach (IACConfig machineConfig in machineConfigs)
                    {
                        ACConfigParam additionalParam = GetConfigParamWithMachine(aCConfigParam, machineConfig.VBACClass);
                        additionalParam.DefaultConfiguration = machineConfig;
                        aCConfigParams.Add(additionalParam);
                    }
                }
            }

            return aCConfigParams;
        }

        private (ACClassMethod pwNodeMehtod, ACClassMethod paFunctionMethod) GetWFMethods(ACClassWF aCClassWF)
        {
            ACClassMethod pwNodeMehtod = null;
            ACClassMethod paFunctionMethod = null;

            List<ACClassMethod> pwNodeMethods = aCClassWF.PWACClass.MethodsCached.Where(c => c.ACGroup == Const.ACState).OrderBy(c => c.SortIndex).ToList();
            var firstMethod = pwNodeMethods.Where(c => c.ACIdentifier == ACStateConst.SMStarting).FirstOrDefault();
            if (firstMethod == null)
                firstMethod = pwNodeMethods.FirstOrDefault();
            pwNodeMehtod = firstMethod;

            paFunctionMethod = aCClassWF.RefPAACClassMethod;

            return (pwNodeMehtod, paFunctionMethod);
        }

        private void DeleteConfigParam(ACConfigParam aCConfigParam)
        {
            aCConfigParam.ConfigurationList.Remove(aCConfigParam.DefaultConfiguration);
            CurrentConfigStore.RemoveACConfig(aCConfigParam.DefaultConfiguration);
            aCConfigParam.DefaultConfiguration = aCConfigParam.ConfigurationList.OrderByDescending(x => x.ConfigStore.OverridingOrder).FirstOrDefault();
        }

        private List<ACClass> GetMachnies(ACClassWF acClassWF)
        {
            List<ACClass> pwNodeMachineList = null;
            // Group <-> Allowed_instances
            RuleTypeDefinition allowedInstancesRuleTypeDef = RulesCommand.ListOfRuleInfoPatterns.FirstOrDefault(p => p.RuleType == ACClassWFRuleTypes.Allowed_instances);
            // Method <-> Excluded_process_modules
            RuleTypeDefinition excludedProcessModulesRuleTypeDef = RulesCommand.ListOfRuleInfoPatterns.FirstOrDefault(p => p.RuleType == ACClassWFRuleTypes.Excluded_process_modules);

            // In case method
            if (excludedProcessModulesRuleTypeDef.RuleApplyedWFACKindTypes.Contains(acClassWF.PWACClass.ACKind))
            {
                pwNodeMachineList = acClassWF.ACClassWF1_ParentACClassWF.RefPAACClass.DerivedClassesInProjects.ToList();
                //paFunctionMachineList = RulesCommand.GetProcessModules(acClassWF, Database.ContextIPlus).ToList();
            }
            // In case PWGroup
            else if (allowedInstancesRuleTypeDef.RuleApplyedWFACKindTypes.Contains(acClassWF.PWACClass.ACKind))
            {
                pwNodeMachineList = acClassWF.RefPAACClass.DerivedClassesInProjects.ToList();
            }

            return pwNodeMachineList;
        }

        private ACConfigParam GetConfigParamWithMachine(ACConfigParam aCConfigParam, ACClass machine)
        {
            ACConfigParam additionalParam = ACConfigHelper.FactoryMachineParam(aCConfigParam, machine);
            additionalParam.ACClassWF = aCConfigParam.ACClassWF;
            additionalParam.LocalConfigACUrl = aCConfigParam.LocalConfigACUrl;
            additionalParam.PreConfigACUrl = aCConfigParam.PreConfigACUrl;
            additionalParam.ACMehtodACIdentifier = aCConfigParam.ACMehtodACIdentifier;
            return additionalParam;
        }

        private void Clear()
        {
            _SelectedPWNodeParamValue = null;
            _PWNodeParamValueList = null;
            _SelectedHistoryPWNodeParamValue = null;

            _SelectedPAFunctionParamValue = null;
            _PAFunctionParamValueList = null;
            _SelectedHistoryPAFunctionParamValue = null;

            AllMachines = new Dictionary<ACClass, List<Guid>>();
        }

        private void SetupCurrentConfigStore(VD.DatabaseApp databaseApp, Guid? partslistID, Guid? prodOrderPartslistID, Guid? pickingID)

        {
            if (pickingID != null)
            {
                CurrentConfigStore = databaseApp.Picking.Where(c => c.PickingID == pickingID).FirstOrDefault();
            }
            else if (prodOrderPartslistID != null)
            {
                CurrentConfigStore = databaseApp.ProdOrderPartslist.Where(c => c.ProdOrderPartslistID == prodOrderPartslistID).FirstOrDefault();
            }
            else if (partslistID != null)
            {
                CurrentConfigStore = databaseApp.Partslist.Where(c => c.PartslistID == partslistID).FirstOrDefault();
            }
        }

        private static IACConfig GetNewConfigParam(IACConfigStore currentConfigStore, ACConfigParam aCConfigParam, ACClass machine)
        {
            Guid? vbiACClassID = machine != null ? machine.ACClassID : (Guid?)null;
            if (vbiACClassID == null && aCConfigParam.VBiACClassID != null)
                vbiACClassID = aCConfigParam.VBiACClassID;

            string localParamConfigACUrl = aCConfigParam.LocalConfigACUrl + @"\" + aCConfigParam.ACMehtodACIdentifier + @"\" + aCConfigParam.ACIdentifier;

            return
                ConfigManagerIPlus
                .ACConfigFactory(
                    currentConfigStore,
                    aCConfigParam,
                    aCConfigParam.PreConfigACUrl,
                    localParamConfigACUrl,
                    vbiACClassID);
        }
        #endregion

    }

    class WFWrapper
    {
        public string PreConfigACUrl { get; set; }
        public string LocalConfigACUrl { get; set; }
        public ACClassWF WF { get; set; }
        public List<WFWrapper> ChildWF { get; set; } = new List<WFWrapper>();
        public List<IACConfig> MatchedConfigs { get; set; } = new List<IACConfig>();
        public List<ACConfigParam> PWNodeParams { get; set; } = new List<ACConfigParam>();
        public List<ACConfigParam> PAFunctionParams { get; set; } = new List<ACConfigParam>();
    }
}
