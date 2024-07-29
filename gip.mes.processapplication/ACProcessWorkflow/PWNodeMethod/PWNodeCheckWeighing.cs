using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Xml;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.core.processapplication;
using gip.mes.facility;

namespace gip.mes.processapplication
{

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Checkweighing'}de{'Kontrollverwiegung'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWNodeCheckWeighing : PWBaseNodeProcess, IPWNodeCheckWeight
    {
        public const string PWClassName = "PWNodeCheckWeighing";

        #region Constructors

        static PWNodeCheckWeighing()
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("TolerancePlus", typeof(Double), (Double)5.0, Global.ParamOption.Required));
            paramTranslation.Add("TolerancePlus", "en{'Tolerance + [+=kg/-=%]'}de{'Toleranz + [+=kg/-=%]'}");
            method.ParameterValueList.Add(new ACValue("ToleranceMinus", typeof(Double), (Double)5.0, Global.ParamOption.Required));
            paramTranslation.Add("ToleranceMinus", "en{'Tolerance - [+=kg/-=%]'}de{'Toleranz - [+=kg/-=%]'}");
            method.ParameterValueList.Add(new ACValue("TargetQValidation", typeof(bool), (bool)false, Global.ParamOption.Required));
            paramTranslation.Add("TargetQValidation", "en{'Validation with target quantity'}de{'Validierung gegenüber Sollwert'}");
            method.ParameterValueList.Add(new ACValue("SetToEmptyingMode", typeof(bool), (bool)false, Global.ParamOption.Required));
            paramTranslation.Add("SetToEmptyingMode", "en{'Switch to emptying mode if not in tolerance'}de{'In Leerfahrmodus setzen wenn ausserhalb Toleranz'}");
            method.ParameterValueList.Add(new ACValue("CompareWithSum", typeof(bool), (bool)false, Global.ParamOption.Required));
            paramTranslation.Add("CompareWithSum", "en{'Compare target with actual weight (no scale)'}de{'Vergleiche Soll mit Istwert (ohne Waage)'}");
            var wrapper = new ACMethodWrapper(method, "en{'Checkweighing'}de{'Kontrollverwiegung'}", typeof(PWNodeCheckWeighing), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWNodeCheckWeighing), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWNodeCheckWeighing), HandleExecuteACMethod_PWNodeCheckWeighing);
        }

        public PWNodeCheckWeighing(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _IsToleranceError = null;
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties
        public T ParentPWMethod<T>() where T : PWMethodVBBase
        {
            if (ParentRootWFNode == null)
                return null;
            return ParentRootWFNode as T;
        }

        public bool IsProduction
        {
            get
            {
                return ParentPWMethod<PWMethodProduction>() != null;
            }
        }

        protected double TolerancePlus
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("TolerancePlus");
                    if (acValue != null)
                    {
                        return acValue.ParamAsDouble;
                    }
                }
                return 5.0;
            }
        }

        protected double ToleranceMinus
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ToleranceMinus");
                    if (acValue != null)
                    {
                        return acValue.ParamAsDouble;
                    }
                }
                return 5.0;
            }
        }

        protected bool TargetQValidation
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("TargetQValidation");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        protected bool SetToEmptyingMode
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("SetToEmptyingMode");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        protected bool CompareWithSum
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("CompareWithSum");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        public override bool MustBeInsidePWGroup
        {
            get
            {
                return true;
            }
        }
        #endregion


        #region Methods

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "AcknowledgeTolerance":
                    AcknowledgeTolerance();
                    return true;
                case "CheckAgain":
                    CheckAgain();
                    return true;
                case Const.IsEnabledPrefix + "AcknowledgeTolerance":
                    result = IsEnabledAcknowledgeTolerance();
                    return true;
                case Const.IsEnabledPrefix + "CheckAgain":
                    result = IsEnabledCheckAgain();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PWNodeCheckWeighing(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWBaseNodeProcess(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        bool? _IsToleranceError = null;
        [ACPropertyBindingSource(9999, "", "", "", false, true)]
        public IACContainerTNet<short> InTol { get; set; }

        public override void SMIdle()
        {
            ClearMyConfiguration();
            _IsToleranceError = null;
            base.SMIdle();
            if (ParentPWGroup != null)
            {
                var processModule = ParentPWGroup.AccessedProcessModule;
                if (processModule != null)
                    processModule.RefreshPWNodeInfo();
            }
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            _IsToleranceError = null;
            //if (!PreExecute(PABaseState.SMStarting))
            //  return;
            RecalcTimeInfo();
            if (CreateNewProgramLog(NewACMethodWithConfiguration()) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                return;

            var rootPW = RootPW;
            bool doCheck = true;
            if (rootPW != null 
                && (((ACSubStateEnum)rootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                    || ((ACSubStateEnum)rootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)
                    || ((ACSubStateEnum)rootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)))
                doCheck = false;
            PWGroup pwGroup = ParentPWGroup;
            if (pwGroup != null 
                && (((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                    || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)
                    || ((ACSubStateEnum)rootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)))
                doCheck = false;

            if (doCheck)
            {
                if (ACOperationMode == ACOperationModes.Live)
                {
                    if (ParentPWGroup != null)
                    {
                        var processModule = ParentPWGroup.AccessedProcessModule;
                        if (processModule != null)
                            processModule.RefreshPWNodeInfo();
                    }
                    CurrentACState = ACStateEnum.SMRunning;
                }
                else
                    CurrentACState = ACStateEnum.SMCompleted;
            }
            else
                CurrentACState = ACStateEnum.SMCompleted;
        }

        [ACMethodState("en{'Running'}de{'Läuft'}", 30, true)]
        public override void SMRunning()
        {
            if (!_IsToleranceError.HasValue)
            {
                if (!StartCheckWeighing())
                {
                    SubscribeToProjectWorkCycle();
                    return;
                }
            }
            if (_IsToleranceError.HasValue && _IsToleranceError.Value)
                return;

            UnSubscribeToProjectWorkCycle();
            AcknowledgeAlarms();
            CurrentACState = ACStateEnum.SMCompleted;
        }

        [ACMethodState("en{'Completed'}de{'Beendet'}", 40, true)]
        public override void SMCompleted()
        {
            base.SMCompleted();
        }


        protected virtual bool StartCheckWeighing()
        {
            if (!IsProduction)
                return true;

            var processModule = ParentPWGroup.AccessedProcessModule;
            if (processModule == null)
                return true;
            PAEScaleGravimetric scale = processModule.FindChildComponents<PAEScaleGravimetric>(c => c is PAEScaleGravimetric).FirstOrDefault();
            if (scale == null && !CompareWithSum)
                return true;

            Msg msg = null;
            var pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            if (pwMethodProduction.CurrentProdOrderPartslistPos == null 
                || pwMethodProduction.CurrentProdOrderBatch == null 
                || !pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchPlanID.HasValue)
            {
                // Error50094:Batchplan not found
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(1)", 1000, "Error50094");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), "StartDischarging(1)", msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return false;
            }

            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                ProdOrderPartslistPos endBatchPos = pwMethodProduction.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                var batchPlan = dbApp.ProdOrderBatchPlan.Where(c => c.ProdOrderBatchPlanID == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchPlanID.Value).FirstOrDefault();
                if (batchPlan == null || !batchPlan.VBiACClassWFID.HasValue)
                {
                    // Error50095: Reference from Batchplan to Workflownode is null
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(2)", 1010, "Error50095",
                                    batchPlan == null ? "" : batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                    batchPlan == null ? "" : batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                    batchPlan == null ? "" : batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), "StartDischarging(2)", msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return false;
                }

                MaterialWFConnection matWFConnection = null;
                if (batchPlan.MaterialWFACClassMethodID.HasValue)
                {
                    matWFConnection = dbApp.MaterialWFConnection
                                            .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan.MaterialWFACClassMethodID.Value
                                                        && c.ACClassWFID == ContentACClassWF.ACClassWFID)
                                            .FirstOrDefault();
                }
                else
                {
                    PartslistACClassMethod plMethod = batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                    if (plMethod != null)
                    {
                        matWFConnection = dbApp.MaterialWFConnection
                                                .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                        && c.ACClassWFID == ContentACClassWF.ACClassWFID)
                                                .FirstOrDefault();
                    }
                    else
                    {
                        matWFConnection = dbApp.MaterialWFConnection
                                                .Where(c => c.MaterialWFACClassMethod.MaterialWFID == batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.MaterialWFID
                                                        && c.ACClassWFID == ContentACClassWF.ACClassWFID)
                                                .FirstOrDefault();
                    }
                }

                if (matWFConnection == null)
                {
                    // Error50096: No relation defined between Workflownode and intermediate material in Materialworkflow
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(3)", 1020, "Error50096");

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), "StartNextProdComponent(3)", msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return false;
                }

                // Find intermediate position which is assigned to this Workflownode
                var currentProdOrderPartslist = endBatchPos.ProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
                ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                    .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                        && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                        && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
                if (intermediatePosition == null)
                {
                    // Error50097: Intermediate line not found which is assigned to this Workflownode
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(4)", 1030, "Error50097");

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), "StartNextProdComponent(4)", msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return false;
                }

                ProdOrderPartslistPos intermediateChildPos = intermediatePosition.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                    .Where(c => c.ProdOrderBatchID.HasValue
                                && c.ProdOrderBatchID.Value == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID)
                    .FirstOrDefault();
                if (intermediateChildPos == null)
                {
                    // Error50097: Intermediate line not found which is assigned to this Workflownode
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(4)", 1040, "Error50097");

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), "StartNextProdComponent(4)", msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return false;
                }

                double actualBatchWeight = intermediateChildPos.ActualQuantityUOM;
                double targetBatchWeight = intermediateChildPos.TargetQuantityUOM;
                if (!TargetQValidation && actualBatchWeight <= 0.000001)
                {
                    actualBatchWeight = intermediateChildPos.TargetQuantityUOM;
                    ACProdOrderManager prodOrderManager = ACProdOrderManager.GetServiceInstance(this);
                    if (prodOrderManager != null)
                    {
                        double calculatedBatchWeight = 0;
                        if (prodOrderManager.CalcProducedBatchWeight(dbApp, intermediateChildPos, null, out calculatedBatchWeight) == null)
                        {
                            actualBatchWeight = calculatedBatchWeight;
                        }
                    }
                }

                _IsToleranceError = false;
                double diff = 0;
                double weightForToleranceCalc = 0;
                if (CompareWithSum)
                {
                    diff = actualBatchWeight - targetBatchWeight;
                    weightForToleranceCalc = targetBatchWeight;
                }
                else if (TargetQValidation)
                {
                    diff = scale.ActualValue.ValueT - targetBatchWeight;
                    weightForToleranceCalc = targetBatchWeight;
                }
                else
                {
                    diff = scale.ActualValue.ValueT - actualBatchWeight;
                    weightForToleranceCalc = actualBatchWeight;
                }

                if (weightForToleranceCalc >= 0.00001)
                {
                    double maxDiff = 0;
                    if (this.TolerancePlus <= -0.000001)
                        maxDiff = weightForToleranceCalc * Math.Abs(this.TolerancePlus) * 0.01;
                    else
                        maxDiff = this.TolerancePlus;

                    double minDiff = 0;
                    if (this.ToleranceMinus <= -0.000001)
                        minDiff = weightForToleranceCalc * this.ToleranceMinus * 0.01;
                    else
                        minDiff = this.ToleranceMinus * -1;

                    if (diff > maxDiff || diff < minDiff)
                    {
                        if (SetToEmptyingMode)
                        {
                            PWMethodVBBase rootPW = RootPW as PWMethodVBBase;
                            if (rootPW != null)
                            {
                                rootPW.SwitchToEmptyingMode();
                                InTol.ValueT = 1;
                            }
                            else
                            {
                                //Error50159:Root node not found.
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(5)", 1050, "Error50159");

                                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                    Messages.LogError(this.GetACUrl(), "StartNextProdComponent(5)", msg.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                return false;
                            }
                        }
                        else
                        {
                            if (!IsSimulationOn || IsManualSimulation)
                            {
                                if (CompareWithSum)
                                {
                                    // Error50404: Weighing-Check-Alarm: At this point is posted {0} kg material, but the target quantity is {1} kg.The difference of {2} kg is too high (Min-Tol.: {3}, Max-Tol.: {4}).
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(4)", 495, "Error50404",
                                                  actualBatchWeight, weightForToleranceCalc, diff, maxDiff, minDiff);
                                }

                                else
                                {
                                    // Error50324: Weighing-Check-Alarm: The scale measures a total weight of {0} kg. But {1} kg material must appear in it. The difference of {2} kg is too high (Min-Tol.: {3}, Max-Tol.: {4}).
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(4)", 502, "Error50324",
                                                    scale.ActualValue.ValueT, weightForToleranceCalc, diff, maxDiff, minDiff);
                                }

                                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                    Messages.LogError(this.GetACUrl(), "StartNextProdComponent(4)", msg.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                _IsToleranceError = true;
                                InTol.ValueT = 1;
                            }
                            else
                                InTol.ValueT = 2;
                        }
                    }
                    else
                        InTol.ValueT = 2;
                }
                else
                    InTol.ValueT = 2;
            }
            return true;
        }

        public double? CalcTargetWeight()
        {
            if (!IsProduction)
                return null;
             var pwMethodProduction = ParentPWMethod<PWMethodProduction>();
             if (pwMethodProduction.CurrentProdOrderPartslistPos == null
                 || pwMethodProduction.CurrentProdOrderBatch == null
                 || !pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchPlanID.HasValue)
             {
                 return null;
             }

            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                ProdOrderPartslistPos endBatchPos = pwMethodProduction.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                var batchPlan = dbApp.ProdOrderBatchPlan.Where(c => c.ProdOrderBatchPlanID == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchPlanID.Value).FirstOrDefault();
                if (batchPlan == null || !batchPlan.VBiACClassWFID.HasValue)
                    return null;

                MaterialWFConnection matWFConnection = null;
                if (batchPlan.MaterialWFACClassMethodID.HasValue)
                {
                    matWFConnection = dbApp.MaterialWFConnection
                                            .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan.MaterialWFACClassMethodID.Value
                                                        && c.ACClassWFID == ContentACClassWF.ACClassWFID)
                                            .FirstOrDefault();
                }
                else
                {
                    PartslistACClassMethod plMethod = batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                    if (plMethod != null)
                    {
                        matWFConnection = dbApp.MaterialWFConnection
                                                .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                        && c.ACClassWFID == ContentACClassWF.ACClassWFID)
                                                .FirstOrDefault();
                    }
                    else
                    {
                        matWFConnection = dbApp.MaterialWFConnection
                                                .Where(c => c.MaterialWFACClassMethod.MaterialWFID == batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.MaterialWFID
                                                        && c.ACClassWFID == ContentACClassWF.ACClassWFID)
                                                .FirstOrDefault();
                    }
                }

                if (matWFConnection == null)
                    return null;

                // Find intermediate position which is assigned to this Workflownode
                var currentProdOrderPartslist = endBatchPos.ProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
                ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                    .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                        && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                        && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
                if (intermediatePosition == null)
                    return null;

                ProdOrderPartslistPos intermediateChildPos = intermediatePosition.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                    .Where(c => c.ProdOrderBatchID.HasValue
                                && c.ProdOrderBatchID.Value == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID)
                    .FirstOrDefault();
                if (intermediateChildPos == null)
                    return null;

                double actualBatchWeight = intermediateChildPos.ActualQuantityUOM;
                if (actualBatchWeight <= 0.000001)
                {
                    actualBatchWeight = intermediateChildPos.TargetQuantityUOM;
                    ACProdOrderManager prodOrderManager = ACProdOrderManager.GetServiceInstance(this);
                    if (prodOrderManager != null)
                    {
                        double calculatedBatchWeight = 0;
                        if (prodOrderManager.CalcProducedBatchWeight(dbApp, intermediateChildPos, null, out calculatedBatchWeight) == null)
                        {
                            actualBatchWeight = calculatedBatchWeight;
                        }
                    }
                }
                return actualBatchWeight;
            }

       }

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList, ref DumpStats dumpStats)
        {
            base.DumpPropertyList(doc, xmlACPropertyList, ref dumpStats);

            XmlElement xmlChild = xmlACPropertyList["TolerancePlus"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("TolerancePlus");
                if (xmlChild != null)
                    xmlChild.InnerText = TolerancePlus.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["ToleranceMinus"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("ToleranceMinus");
                if (xmlChild != null)
                    xmlChild.InnerText = ToleranceMinus.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["TargetQValidation"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("TargetQValidation");
                if (xmlChild != null)
                    xmlChild.InnerText = TargetQValidation.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["SetToEmptyingMode"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("SetToEmptyingMode");
                if (xmlChild != null)
                    xmlChild.InnerText = SetToEmptyingMode.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["CompareWithSum"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("CompareWithSum");
                if (xmlChild != null)
                    xmlChild.InnerText = CompareWithSum.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }
        }

        #endregion

        #region User Interaction
        [ACMethodInteraction("Process", "en{'Acknowledge Tolerance Alarm'}de{'Toleranzüberschreitung akzeptieren'}", (short)500, true)]
        public void AcknowledgeTolerance()
        {
            if (IsEnabledAcknowledgeTolerance())
            {
                UnSubscribeToProjectWorkCycle();
                AcknowledgeAlarms();
                CurrentACState = ACStateEnum.SMCompleted;
            }
        }

        public bool IsEnabledAcknowledgeTolerance()
        {
            return CurrentACState == ACStateEnum.SMRunning 
                && _IsToleranceError.HasValue 
                && _IsToleranceError.Value;
        }

        [ACMethodInteraction("Process", "en{'Check weight again'}de{'Erneute Gewichtsüberprüfung'}", (short)501, true)]
        public void CheckAgain()
        {
            if (IsEnabledCheckAgain())
            {
                _IsToleranceError = null;
                SubscribeToProjectWorkCycle();
            }
        }

        public bool IsEnabledCheckAgain()
        {
            return CurrentACState == ACStateEnum.SMRunning 
                && _IsToleranceError.HasValue 
                && _IsToleranceError.Value;
        }
        #endregion

    }
}
