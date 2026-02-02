// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.processapplication;
using gip.mes.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.processapplication;
using System.Collections.ObjectModel;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Sample weighing'}de{'Gewichtsprüfung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, SortIndex = 400)]
    public class BSOSampleWeighing : BSOManualWeighing
    {
        public BSOSampleWeighing(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            DeActivate();
            return await base.ACDeInit(deleteACClassTask);
        }

        public new const string ClassName = nameof(BSOSampleWeighing);

        #region Properties

        [ACPropertyInfo(601)]
        public ACComponent ProcessModule
        {
            get;
            set;
        }

        public ACComponent ProcessFunction
        {
            get;
            set;
        }

        public IACContainerTNet<ACStateEnum> PAFACState
        {
            get;
            set;
        }

        [ACPropertyInfo(602)]
        public double TolerancePlus
        {
            get => _TolerancePlus;
        }

        [ACPropertyInfo(603)]
        public double ToleranceMinus
        {
            get => _ToleranceMinus;
        }

        public bool AckInTol
        {
            get;
            set;
        }

        #endregion

        #region Methods


        public override void Activate(ACComponent selectedProcessModule)
        {
            CurrentProcessModule = selectedProcessModule;
            ParentBSOWCS?.ApplicationQueue.Add(() => ActivateModule(selectedProcessModule));
        }

        public override void DeActivate()
        {
            if (PAFACState != null)
                PAFACState.PropertyChanged -= _PAFACStateProp_PropertyChanged;

            ProcessFunction = null;
            ProcessModule = null;
            PAFACState = null;
            CurrentProcessModule = null;
        }

        private void ActivateModule(ACComponent selectedProcessModule)
        {
            ACComponent currentProcessModule = selectedProcessModule;
            if (currentProcessModule == null)
            {
                //Error50283: The manual weighing module can not be initialized. The property CurrentProcessModule is null.
                // Die Handverwiegungsstation konnte nicht initialisiert werden. Die Eigenschaft CurrentProcessModule ist null.
                Messages.ErrorAsync(this, "Error50283");
                return;
            }

            //PAProcessModuleACUrl = currentProcessModule.ACUrl;
            //PAProcessModuleACCaption = currentProcessModule.ACCaption;

            if (currentProcessModule.ConnectionState == ACObjectConnectionState.DisConnected)
            {
                //Info50040: The server is unreachable.Reopen the program once the connection to the server has been established.
                //     Der Server ist nicht erreichbar.Öffnen Sie das Programm erneut sobal die Verbindung zum Server wiederhergestellt wurde.
                Messages.InfoAsync(this, "Info50040");
                return;
            }

            ProcessFunction = ItemFunction?.ProcessFunction;

            var processModuleChildComps = currentProcessModule.ACComponentChildsOnServer;
            IEnumerable<IACComponent> scaleObjects = processModuleChildComps.Where(c => typeof(PAEScaleBase).IsAssignableFrom(c.ComponentClass.ObjectType)).ToArray();
            if (scaleObjects != null && scaleObjects.Any())
            {
                _ProcessModuleScales = scaleObjects.Select(c => new ACRef<IACComponent>(c, this)).ToArray();
                ActivateScale(scaleObjects.FirstOrDefault());

                var scaleObjectInfoList = new ObservableCollection<ACValueItem>(_ProcessModuleScales.Select(c => new ACValueItem(c.ValueT.ACCaption, c.ACUrl, null)));
                ScaleObjectsList = scaleObjectInfoList;
                CurrentScaleObject = ScaleObjectsList.FirstOrDefault();
            }

            var pafACState = ProcessFunction.GetPropertyNet(nameof(ACState));
            if (pafACState == null)
            {
                Messages.ErrorAsync(this, "50285", false, nameof(ACState));
                return;
            }

            PAFACState = pafACState as IACContainerTNet<ACStateEnum>;
            if (PAFACState == null)
            {
                //Error50326: The property ACState can not be found on the current process function.
                Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "Activate(20)", 114, "Error50326");
                Messages.LogMessageMsg(msg);
                Messages.MsgAsync(msg);
                return;
            }

            PAFACState.PropertyChanged += _PAFACStateProp_PropertyChanged;

            var currentACMethod = ProcessFunction.GetPropertyNet(nameof(PAProcessFunction.CurrentACMethod)) as IACContainerTNet<ACMethod>;
            if (currentACMethod == null)
            {
                //Error50287: Initialization error: The weighing function doesn't have the property {0}.
                // Initialisierungsfehler: Die Verwiegefunktion besitzt nicht die Eigenschaft {0}.
                Messages.InfoAsync(this, "Error50287", false, nameof(PAProcessFunction.CurrentACMethod));
                return;
            }

            HandlePAFCurrentACMethod(currentACMethod.ValueT);
        }

        [ACMethodInfo("", "en{'Register sample weight'}de{'Stichprobengewicht registrieren'}", 601, true)]
        public void RegisterSampleWeight()
        {
            Msg msg = ProcessFunction.ExecuteMethod(nameof(PAFSampleWeighing.RegisterSampleWeight)) as Msg;
            if (msg != null)
                Messages.MsgAsync(msg);
        }

        public bool IsEnabledRegisterSampleWeight()
        {
            if (ProcessFunction == null)
                return false;

            if (PAFACState == null)
                return false;

            if (PAFACState.ValueT != ACStateEnum.SMRunning)
                return false;

            if (AckInTol && ScaleBckgrState != ScaleBackgroundState.InTolerance)
                return false;

            return true;
        }

        private void _PAFACStateProp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                var tempSender = sender as IACContainerTNet<ACStateEnum>;
                if (tempSender != null)
                {
                    ACStateEnum tempState = tempSender.ValueT;
                    ParentBSOWCS?.ApplicationQueue.Add(() => HandlePAFACState(tempState));
                }
            }
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;

            switch(acMethodName)
            {
                case nameof(RegisterSampleWeight):
                    RegisterSampleWeight();
                    return true;
                case nameof(IsEnabledRegisterSampleWeight):
                    result = IsEnabledRegisterSampleWeight();
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        protected override ScaleBackgroundState DetermineBackgroundState(double? tolPlus, double? tolMinus, double target, double actual)
        {
            if (!tolPlus.HasValue)
                tolPlus = 0;

            if (!tolMinus.HasValue)
                tolMinus = 0;

            if (tolPlus.HasValue && tolMinus.HasValue && target > 0)
            {
                double act = Math.Round(ScaleGrossWeight, 5);

                if (act > target)
                {
                    if (act <= Math.Round(target + tolPlus.Value, 5))
                        return ScaleBackgroundState.InTolerance;
                    else
                        return ScaleBackgroundState.Weighing;
                }
                else
                {
                    if (act >= Math.Round(target - tolMinus.Value, 5))
                        return ScaleBackgroundState.InTolerance;
                }
            }
            return ScaleBackgroundState.OutTolerance;
        }

        private void HandlePAFACState(ACStateEnum acState)
        {
            if (acState == ACStateEnum.SMRunning)
            {
                var currentACMethod = ProcessFunction.GetPropertyNet(nameof(PAProcessFunction.CurrentACMethod)) as IACContainerTNet<ACMethod>;
                if (currentACMethod == null)
                {
                    //Error50287: Initialization error: The weighing function doesn't have the property {0}.
                    // Initialisierungsfehler: Die Verwiegefunktion besitzt nicht die Eigenschaft {0}.
                    Messages.InfoAsync(this, "Error50287", false, nameof(PAProcessFunction.CurrentACMethod));
                    return;
                }

                HandlePAFCurrentACMethod(currentACMethod.ValueT);
            }
            else if (acState == ACStateEnum.SMResetting)
            {
                TargetWeight = 0;
            }
        }

        protected override void HandlePAFCurrentACMethod(ACMethod currentACMethod)
        {
            ACStateEnum? acState = PAFACState?.ValueT;

            if (acState.HasValue && acState == ACStateEnum.SMRunning)
            {
                base.HandlePAFCurrentACMethod(currentACMethod);

                if (currentACMethod != null)
                {
                    ACValue paramAck = currentACMethod.ParameterValueList.GetACValue("AckInTol");
                    if (paramAck != null)
                        AckInTol = paramAck.ParamAsBoolean;
                    else
                        AckInTol = false;
                }
            }
            else
            {
                TargetWeight = 0;
                _TolerancePlus = 0;
                _ToleranceMinus = 0;
            }
        }

        #endregion
    }
}
