using gip.core.autocomponent;
using VD = gip.mes.datamodel;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioSystem, VD.ConstApp.PrefParam, Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, false, true)]
    public class BSOPreferredParameters : ACBSO
    {
        #region c´tors

        public BSOPreferredParameters(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            _VarioConfigManager = ConfigManagerIPlus.ACRefToServiceInstance(this);
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

        #region Properties

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

        #region Properties -> PWNodeParamValue

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
                    OnPropertyChanged(nameof(HistoryPWNodeParamValueList));
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

        #region Properties -> HistoryPWNodeParamValue

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

        #region Properties -> PAFunctionParamValue

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
                    OnPropertyChanged(nameof(HistoryPAFunctionParamValueList));
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

        #region Properties -> HistoryPAFunctionParamValue

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

        #endregion

        #region Methods

        [ACMethodInfo("Dialog", VD.ConstApp.PrefParam, (short)MISort.QueryPreviewDlg)]
        public void ShowParamDialog(VD.DatabaseApp databaseApp, Guid acClassWFID, Guid partslistID, Guid? prodOrderPartslistID)
        {
            _PWNodeParamValueList = null;
            _PAFunctionParamValueList = null;

            (List<ACConfigParam> pwNodeParams, List<ACConfigParam> paFunctionParams) = DoACConfigParams(databaseApp, acClassWFID, partslistID, prodOrderPartslistID);


            _PWNodeParamValueList = pwNodeParams;
            _PAFunctionParamValueList = paFunctionParams;

            OnPropertyChanged(nameof(PWNodeParamValueList));
            OnPropertyChanged(nameof(PAFunctionParamValueList));
        }

        #endregion

        #region Private methods

        private (List<ACConfigParam> pwNodeParams, List<ACConfigParam> paFunctionParams) DoACConfigParams(VD.DatabaseApp databaseApp, Guid acClassWFID, Guid partslistID, Guid? prodOrderPartslistID)
        {
            WFGroupStartData wFGroupStartData = new WFGroupStartData(databaseApp, VarioConfigManager, acClassWFID, partslistID, prodOrderPartslistID);

            List<IACConfig> allConfigs = wFGroupStartData.ConfigStores.SelectMany(c => c.ConfigurationEntries).ToList();
            List<IACConfig> configsWithExpression = allConfigs.Where(c => !string.IsNullOrEmpty(c.Expression)).ToList();

            string preConfigACUrl = ""; //wFGroupStartData.InvokerPWNode.ConfigACUrl;

            WFWrapper wFWrapper = GetWrapper(preConfigACUrl, wFGroupStartData.InvokerPWNode);
            ProcessWF(wFWrapper, configsWithExpression, allConfigs);

            List<ACConfigParam> pwNodeParams = new List<ACConfigParam>();
            List<ACConfigParam> paFunctionParams = new List<ACConfigParam>();

            FillParams(wFWrapper, pwNodeParams, paFunctionParams);

            return (pwNodeParams, paFunctionParams);
        }

        private WFWrapper GetWrapper(string preConfigACUrl, ACClassWF aCClassWF)
        {
            WFWrapper wFWrapper = new WFWrapper();
            wFWrapper.WF = aCClassWF;
            wFWrapper.PreConfigACUrl = preConfigACUrl;
            wFWrapper.LocalConfigACUrl = aCClassWF.ConfigACUrl;

            string childpreConfigACUrl = wFWrapper.LocalConfigACUrl;
            if (!string.IsNullOrEmpty(preConfigACUrl))
            {
                childpreConfigACUrl = preConfigACUrl + "\\" + childpreConfigACUrl;
            }

            List<ACClassWF> allSubWf = aCClassWF.RefPAACClassMethod.ACClassWF_ACClassMethod.ToList();
            foreach (ACClassWF subWf in allSubWf)
            {
                WFWrapper chWf = GetWrapper(childpreConfigACUrl, subWf);
                wFWrapper.ChildWF.Add(chWf);
            }
            return wFWrapper;
        }

        private void ProcessWF(WFWrapper wFWrapper, List<IACConfig> configsWithExpression, List<IACConfig> allConfigs)
        {
            wFWrapper.MatchedConfigs =
                configsWithExpression
                .Where(c =>
                    (
                        (string.IsNullOrEmpty(c.PreConfigACUrl) && string.IsNullOrEmpty(c.PreConfigACUrl))
                        || c.PreConfigACUrl.StartsWith(wFWrapper.PreConfigACUrl)
                    )
                    && c.LocalConfigACUrl.StartsWith(c.LocalConfigACUrl)
                )
                .ToList();

            if (wFWrapper.MatchedConfigs.Any())
            {
                (ACClassMethod pwNodeMehtod, ACClassMethod paFunctionMethod) = GetWFMethods(wFWrapper.WF);
                wFWrapper.PWNodeParams = GetACConfigParams(pwNodeMehtod.ACMethod, wFWrapper.PreConfigACUrl, wFWrapper.LocalConfigACUrl, wFWrapper.MatchedConfigs, allConfigs);
                wFWrapper.PAFunctionParams = GetACConfigParams(paFunctionMethod.ACMethod, wFWrapper.PreConfigACUrl, wFWrapper.LocalConfigACUrl, wFWrapper.MatchedConfigs, allConfigs);
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
        }


        private List<ACConfigParam> GetACConfigParams(ACMethod acMethod, string preConfigACUrl, string localConfigACUrl, List<IACConfig> matchedConfigs, List<IACConfig> allConfigs)
        {
            List<ACConfigParam> aCConfigParams = new List<ACConfigParam>();

            List<string> listOfProperies = acMethod.ParameterValueList.Select(x => localConfigACUrl + @"\" + x.ACIdentifier).ToList();

            foreach (ACValue aCValue in acMethod.ParameterValueList)
            {
                string localConfigUrl = localConfigACUrl + @"\" + aCValue.ACIdentifier;
                bool existConfig = matchedConfigs.Where(c => c.PreConfigACUrl == preConfigACUrl && c.LocalConfigACUrl == localConfigACUrl).Any();
                ACConfigParam aCConfigParam = new ACConfigParam();
                aCConfigParam.ACIdentifier = aCValue.ACIdentifier;
                aCConfigParam.ACCaption = acMethod.GetACCaptionForACIdentifier(aCValue.ACIdentifier);
                aCConfigParam.ValueTypeACClassID = aCValue.ValueTypeACClass != null ? aCValue.ValueTypeACClass.ACClassID : Guid.Empty;

                aCConfigParam.ConfigurationList =
                    allConfigs
                    .Where(c => c.PreConfigACUrl == preConfigACUrl && c.LocalConfigACUrl == localConfigACUrl)
                    .OrderByDescending(c => c.ConfigStore.OverridingOrder)
                    .ToList();

                aCConfigParams.Add(aCConfigParam);
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

        //public string CurrentPWNodeURL
        //{
        //    get
        //    {
        //        return CurrentACClassWF.ConfigACUrl + @"\" + CurrentPWNodeMethod.ACIdentifier;
        //    }
        //}
        //public string CurrentPAFunctionURL
        //{
        //    get
        //    {
        //        return CurrentACClassWF.ConfigACUrl + @"\" + CurrentPAFunctionMethod.ACIdentifier;
        //    }
        //}
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
