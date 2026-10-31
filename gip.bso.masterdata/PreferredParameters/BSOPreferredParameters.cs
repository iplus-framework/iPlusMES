using gip.core.autocomponent;
using VD = gip.mes.datamodel;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using gip.mes.autocomponent;
using gip.mes.facility;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Businessobject (dialog) for viewing and overriding the preferred parameter values of a workflow.
    /// Unlike most business objects this BSO is not opened with a search list; it is a dialog that is invoked
    /// programmatically with the context of a workflow.
    /// Call the ShowParamDialog(acClassWFID, partslistID, prodOrderPartslistID, pickingID) method to open the
    /// parameter dialog. The dialog collects all parameters of the workflow nodes (PWNode) and process functions
    /// (PAFunction) for the given context (workflow, partslist, production order partslist or picking) and shows
    /// them in the PWNodeParamValueList and PAFunctionParamValueList properties.
    /// If the config property UseShortDialog is enabled, only the editable parameters are copied to the
    /// QuickParamList property and the compact quick parameter dialog is shown instead.
    /// To override a parameter select it in the SelectedPWNodeParamValue (or SelectedPAFunctionParamValue) property.
    /// If the override should only apply to a specific machine, select the machine in the SelectedPWNodeMachine
    /// (or SelectedPAFunctionMachine) property before overriding. The machine list contains only machines that
    /// are attached to the workflow of the selected parameter.
    /// Then call the CreatePWNodeValue() (or CreatePAFunctionValue()) method ("Override parameter"). It creates a
    /// new configuration entry in the current config store and inserts it at the first position of the
    /// ConfigurationList of the selected parameter. Enter the new value in the DefaultConfiguration.Value property.
    /// The HistoryPWNodeParamValueList (or HistoryPAFunctionParamValueList) property contains the previously
    /// defined configurations of the selected parameter (config store, expression, value, comment).
    /// To remove an override select the parameter and call the DeletePWNodeParamValue() (or DeletePAFunctionValue())
    /// method ("Remove overridden parameter"). It deletes the configuration from the current config store and saves
    /// the changes immediately.
    /// Confirm the dialog with the DialogOK() method. DialogOK() saves all changes and sets the DialogResult to OK.
    /// The DialogCancel() method closes the dialog without saving.
    /// To open the dialog programmatically and evaluate the result use the ShowParamDialogResult(...) method which
    /// returns true when the user has confirmed the dialog with OK.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOPreferredParameters&amp;type=code on github
    /// to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSystem, VD.ConstApp.PrefParam, Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, false, true,
        Description = @"Businessobject (dialog) for viewing and overriding the preferred parameter values of a workflow.
        Unlike most business objects this BSO is not opened with a search list; it is a dialog that is invoked programmatically with the context of a workflow.
        Call the ShowParamDialog(acClassWFID, partslistID, prodOrderPartslistID, pickingID) method to open the parameter dialog. The dialog collects all parameters of the workflow nodes (PWNode) and process functions (PAFunction) for the given context (workflow, partslist, production order partslist or picking) and shows them in the PWNodeParamValueList and PAFunctionParamValueList properties.
        If the config property UseShortDialog is enabled, only the editable parameters are copied to the QuickParamList property and the compact quick parameter dialog is shown instead.
        To override a parameter select it in the SelectedPWNodeParamValue (or SelectedPAFunctionParamValue) property. If the override should only apply to a specific machine, select the machine in the SelectedPWNodeMachine (or SelectedPAFunctionMachine) property before overriding. The machine list contains only machines that are attached to the workflow of the selected parameter.
        Then call the CreatePWNodeValue() (or CreatePAFunctionValue()) method (""Override parameter""). It creates a new configuration entry in the current config store and inserts it at the first position of the ConfigurationList of the selected parameter. Enter the new value in the DefaultConfiguration.Value property.
        The HistoryPWNodeParamValueList (or HistoryPAFunctionParamValueList) property contains the previously defined configurations of the selected parameter (config store, expression, value, comment).
        To remove an override select the parameter and call the DeletePWNodeParamValue() (or DeletePAFunctionValue()) method (""Remove overridden parameter""). It deletes the configuration from the current config store and saves the changes immediately.
        Confirm the dialog with the DialogOK() method. DialogOK() saves all changes and sets the DialogResult to OK. The DialogCancel() method closes the dialog without saving.
        To open the dialog programmatically and evaluate the result use the ShowParamDialogResult(...) method which returns true when the user has confirmed the dialog with OK.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOPreferredParameters&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.")]
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

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {

            if (_VarioConfigManager != null)
                ConfigManagerIPlus.DetachACRefFromServiceInstance(this, _VarioConfigManager);
            _VarioConfigManager = null;
            return await base.ACDeInit(deleteACClassTask);
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
        [ACPropertyConfig("en{'Use short dialog'}de{'Kurzdialog verwenden'}", Description = "Config flag: if enabled, ShowParamDialog shows the compact quick parameter dialog (QuickParamList) instead of the full parameter dialog with the Configuration and Method tabs.")]
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
        [ACPropertySelected(9999, "PWNodeParamValue", "en{'Parameter'}de{'Parameter'}", Description = "Gets or sets the currently selected parameter of a workflow node (PWNode). Selecting a parameter refreshes the machine list, the configuration history and the visibility of the method editor.")]
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
        [ACPropertyList(9999, "PWNodeParamValue", Description = "Gets the list of workflow node (PWNode) parameters for the context passed to ShowParamDialog.")]
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
        [ACPropertySelected(9999, "HistoryPWNodeParamValue", "en{'Configuration history'}de{'Konfigurationshistorie'}", Description = "Gets or sets the selected entry of the configuration history of the selected workflow node parameter.")]
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
        [ACPropertyList(9999, "HistoryPWNodeParamValue", Description = "Gets the previously defined configurations of the selected workflow node parameter (config store, expression, value, comment).")]
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
        [ACPropertySelected(9999, "PWNodeMachine", "en{'Machine'}de{'Maschine'}", Description = "Gets or sets the machine for which a workflow node parameter override is created. The list contains only machines that are attached to the workflow of the selected parameter.")]
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
        [ACPropertyList(9999, "PWNodeMachine", Description = "Gets the machines that are attached to the workflow of the selected workflow node parameter.")]
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
        [ACPropertySelected(9999, "PAFunctionParamValue", "en{'Parameter'}de{'Parameter'}", Description = "Gets or sets the currently selected parameter of a process function (PAFunction). Selecting a parameter refreshes the machine list, the configuration history and the visibility of the method editor.")]
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
        [ACPropertyList(9999, "PAFunctionParamValue", Description = "Gets the list of process function (PAFunction) parameters for the context passed to ShowParamDialog.")]
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
        [ACPropertySelected(9999, "HistoryPAFunctionParamValue", "en{'Configuration history'}de{'Konfigurationshistorie'}", Description = "Gets or sets the selected entry of the configuration history of the selected process function parameter.")]
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
        [ACPropertyList(9999, "HistoryPAFunctionParamValue", Description = "Gets the previously defined configurations of the selected process function parameter (config store, expression, value, comment).")]
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
        [ACPropertySelected(9999, "PAFunctionMachine", "en{'Machine'}de{'Maschine'}", Description = "Gets or sets the machine for which a process function parameter override is created. The list contains only machines that are attached to the workflow of the selected parameter.")]
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
        [ACPropertyList(9999, "PAFunctionMachine", Description = "Gets the machines that are attached to the workflow of the selected process function parameter.")]
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
        [ACPropertySelected(9999, nameof(QuickParam), "en{'Parameter'}de{'Parameter'}", Description = "Gets or sets the currently selected parameter in the compact quick parameter dialog (shown when UseShortDialog is enabled).")]
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
        [ACPropertyList(9999, nameof(QuickParam), Description = "Gets the parameters of the compact quick parameter dialog: the editable workflow node and process function parameters cloned into the current config store.")]
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
                    .Where(c => !string.IsNullOrEmpty(c.Expression) && c.Expression[0] != ACUrlHelper.OpeningBrace)
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

        [ACMethodInfo("Dialog", VD.ConstApp.PrefParam, (short)MISort.QueryPreviewDlg, Description = "Opens the preferred parameters dialog for the given workflow context (workflow, partslist, production order partslist, picking). Loads the workflow node and process function parameters into PWNodeParamValueList/PAFunctionParamValueList, or into QuickParamList when UseShortDialog is enabled, and shows the dialog.")]
        public async Task ShowParamDialog(Guid acClassWFID, Guid? partslistID, Guid? prodOrderPartslistID, Guid? pickingID)
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

            await ShowDialogAsync(this, dialogName);
        }

        [ACMethodInfo("Dialog", VD.ConstApp.PrefParam, (short)MISort.QueryPreviewDlg, Description = "Opens the preferred parameters dialog like ShowParamDialog and returns true when the user has confirmed the dialog with OK, otherwise false. Use this overload to evaluate the dialog result programmatically.")]
        public async Task<bool> ShowParamDialogResult(Guid acClassWFID, Guid? partslistID, Guid? prodOrderPartslistID, Guid? pickingID)
        {
            DialogResult = new VBDialogResult() { SelectedCommand = eMsgButton.Cancel };

            await ShowParamDialog(acClassWFID, partslistID, prodOrderPartslistID, pickingID);

            bool success = DialogResult.SelectedCommand == eMsgButton.OK;
            DialogResult = null;

            return success;
        }

        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay, Description = "Saves all parameter changes (ACSaveOrUndoChanges), sets the DialogResult to OK and closes the dialog.")]
        public async void DialogOK()
        {
            bool successSave = await ACSaveOrUndoChanges();

            if(DialogResult != null)
            {
                DialogResult.SelectedCommand = eMsgButton.OK;
            }

            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel, Description = "Closes the dialog without saving the changes.")]
        public void DialogCancel()
        {
            CloseTopDialog();
        }

        #endregion

        #endregion

        #region Methods -> PWNode

        [ACMethodInteraction("PWNodeParamValue", "en{'Override parameter'}de{'Parameter überschreiben'}", (short)MISort.InsertData, true, nameof(SelectedPWNodeParamValue), Description = "Creates an override of the selected workflow node parameter in the current config store and inserts it at the first position of its ConfigurationList. If a machine is selected in SelectedPWNodeMachine and the parameter is not yet bound to it, a machine-specific parameter entry is created first. Enter the new value in DefaultConfiguration.Value.")]
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

        [ACMethodInteraction("PWNodeParamValue", "en{'Remove overridden parameter'}de{'Überschriebenen Parameter entfernen'}", (short)MISort.Delete, true, nameof(SelectedPWNodeParamValue), Description = "Removes the override of the selected workflow node parameter from the current config store and saves the changes immediately. Enabled only when the parameter has a default configuration in the current config store.")]
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

        [ACMethodInteraction("PAFunctionParamValue", "en{'Override parameter'}de{'Parameter überschreiben'}", (short)MISort.InsertData, true, nameof(SelectedPAFunctionParamValue), Description = "Creates an override of the selected process function parameter in the current config store and inserts it at the first position of its ConfigurationList. If a machine is selected in SelectedPAFunctionMachine and the parameter is not yet bound to it, a machine-specific parameter entry is created first. Enter the new value in DefaultConfiguration.Value.")]
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

        [ACMethodInteraction("PAFunctionParamValue", "en{'Remove overridden parameter'}de{'Überschriebenen Parameter entfernen'}", (short)MISort.Delete, true, nameof(SelectedPAFunctionParamValue), Description = "Removes the override of the selected process function parameter from the current config store and saves the changes immediately. Enabled only when the parameter has a default configuration in the current config store.")]
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


        #region override
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            //if (vbControl.VBContent == null && vbControl.ACIdentifier == "VBGrid[]:")
            //{
            //    if (IsEnabledDeletePWNodeParamValue())
            //        return result;
            //    else
            //        return Global.ControlModes.Hidden;
            //}
            switch (vbControl.VBContent)
            {
                //case "SelectedPWNodeParamValue\\DefaultConfiguration":
                //    {
                //        if (IsEnabledDeletePWNodeParamValue())
                //            return result;
                //        else
                //            return Global.ControlModes.Disabled;
                //    }
                //case "SelectedPAFunctionParamValue\\DefaultConfiguration":
                //    {
                //        if (IsEnabledDeletePAFunctionValue())
                //            return result;
                //        else
                //            return Global.ControlModes.Disabled;
                //    }
                case "SelectedPAFunctionParamValue\\DefaultConfiguration\\Value":
                case "SelectedPWNodeParamValue\\DefaultConfiguration\\Value":
                    {
                        ACConfigParam aCConfigParam = vbControl.VBContent == "SelectedPAFunctionParamValue\\DefaultConfiguration\\Value" ? SelectedPAFunctionParamValue : SelectedPWNodeParamValue;
                        IACConfig lastExpressionEntry = aCConfigParam.ConfigurationList.Where(c => !string.IsNullOrEmpty(c.Expression) && c.Expression.IndexOf(ACUrlHelper.OpeningBrace) >= 0).LastOrDefault();
                        if (lastExpressionEntry != null)
                        {
                            int codeStart = lastExpressionEntry.Expression.IndexOf(ACUrlHelper.OpeningBrace);
                            int codeEnd = lastExpressionEntry.Expression.LastIndexOf(ACUrlHelper.ClosingBrace);
                            if (codeEnd - 1 > codeStart)
                            {
                                Global.ControlModesInfo validationResult = VarioConfigManager.ValidateExpressionParams(aCConfigParam.DefaultConfiguration, lastExpressionEntry);
                                result = validationResult.Mode;
                            }
                        }
                        return result;
                    }
                default:
                    return result;

            }
        }
        #endregion

        #endregion

        #region Private methods

        private (List<ACConfigParam> pwNodeParams, List<ACConfigParam> paFunctionParams) DoACConfigParams(VD.DatabaseApp databaseApp, Guid acClassWFID, Guid? partslistID, Guid? prodOrderPartslistID, Guid? pickingID)
        {

            WFGroupStartData wFGroupStartData = new WFGroupStartData(databaseApp, VarioConfigManager, acClassWFID, partslistID, prodOrderPartslistID, pickingID);

            List<IACConfig> allConfigs = wFGroupStartData.ConfigStores.SelectMany(c => c.ConfigurationEntries).ToList();
            List<IACConfig> configsWithExpression = allConfigs.Where(c => !string.IsNullOrEmpty(c.Expression) && c.Expression[0] != ACUrlHelper.OpeningBrace).ToList();

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
