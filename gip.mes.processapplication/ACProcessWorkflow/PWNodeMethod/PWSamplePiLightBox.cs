using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using System.Xml;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System.Diagnostics.Eventing.Reader;
using gip.core.processapplication;

namespace gip.mes.processapplication
{

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Sample light box RaspPi node'}de{'Stichproben Ampelbox Knoten'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWSamplePiLightBox : PWBaseNodeProcess
    {
        public const string PWClassName = "PWSamplePiLightBox";

        #region Properties
        #endregion

        #region Constructors

        static PWSamplePiLightBox()
        {
            ACMethod TMP;
            TMP = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> translation = new Dictionary<string, string>();

            TMP.ParameterValueList.Add(new ACValue("PiCommand", typeof(SamplePiCommand), (short)SamplePiCommand.SendOrder, Global.ParamOption.Required));
            translation.Add("PiCommand", "en{'Sample command'}de{'Stichproben Kommando'}");
            TMP.ParameterValueList.Add(new ACValue("TolerancePlus", typeof(Double), (Double)0.0, Global.ParamOption.Required));
            translation.Add("TolerancePlus", "en{'Tolerance + [+=kg/-=%]'}de{'Toleranz + [+=kg/-=%]'}");
            TMP.ParameterValueList.Add(new ACValue("ToleranceMinus", typeof(Double), (Double)0.0, Global.ParamOption.Required));
            translation.Add("ToleranceMinus", "en{'Tolerance - [+=kg/-=%]'}de{'Toleranz - [+=kg/-=%]'}");
            TMP.ParameterValueList.Add(new ACValue("LabOrderTemplateName", typeof(string), PWSampleWeighing.C_LabOrderTemplateName, Global.ParamOption.Required));
            translation.Add("LabOrderTemplateName", "en{'Laborder template name'}de{'Name der Laborauftragsvorlage'}");

            var wrapper = new ACMethodWrapper(TMP, "en{'Sample command'}de{'Stichproben Kommando'}", typeof(PWSamplePiLightBox), translation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWSamplePiLightBox), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWSamplePiLightBox), HandleExecuteACMethod_PWSamplePiLightBox);
        }

        public PWSamplePiLightBox(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (ParentPWGroup != null)
                ParentPWGroup.ProcessModuleChanged -= ParentPWGroup_ProcessModuleChanged;

            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            ClearMyConfiguration();
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }

        public override bool ACPostInit()
        {
            if (Root != null
                && !Root.Initialized
                && ParentPWGroup != null)
            {
                ParentPWGroup.ProcessModuleChanged += ParentPWGroup_ProcessModuleChanged;
            }
            return base.ACPostInit();
        }

        #endregion

        #region Public

        #region Properties
        public SamplePiCommand PiCommand
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("PiCommand");
                    if (acValue != null)
                        return (SamplePiCommand)acValue.ParamAsInt16;
                }
                return SamplePiCommand.SendOrder;
            }
        }

        public double TolerancePlus
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("TolerancePlus");
                    if (acValue != null)
                        return acValue.ParamAsDouble;
                }
                return -1.0;
            }
        }

        public double ToleranceMinus
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    ACValue acValue = method.ParameterValueList.GetACValue("ToleranceMinus");
                    if (acValue != null)
                        return acValue.ParamAsDouble;
                }
                return -1.0;
            }
        }

        /// <summary>
        /// If null in configuration, then constant C_LabOrderTemplateName is used and therefore only one LabOrderTemplate is used.
        /// If string is empty, then the MaterialNo will be used to find a Template. If a Template doesn't exist in the database, then a new one is created for each material.
        /// Else Template with the configured name is used.
        /// </summary>
        public string LabOrderTemplateName
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    ACValue acValue = method.ParameterValueList.GetACValue("LabOrderTemplateName");
                    if (acValue != null)
                    {
                        if (acValue.ParamAsString == null)
                            return PWSampleWeighing.C_LabOrderTemplateName;
                        if (!String.IsNullOrEmpty(acValue.ParamAsString))
                            return acValue.ParamAsString.Trim();
                        return acValue.ParamAsString;
                    }
                }
                return PWSampleWeighing.C_LabOrderTemplateName;
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

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWSamplePiLightBox(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWBaseNodeProcess(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            //if (!PreExecute(PABaseState.SMStarting))
            //  return;
            RecalcTimeInfo();
            CreateNewProgramLog(NewACMethodWithConfiguration());

            if (CheckParentGroupAndHandleSkipMode())
            {
                if (ParentPWGroup != null)
                {
                    ParentPWGroup.ProcessModuleChanged += ParentPWGroup_ProcessModuleChanged;
                    if (ParentPWGroup.AccessedProcessModule != null)
                    {
                        HandleProcessModuleMapping(ParentPWGroup.AccessedProcessModule, false);
                    }
                }
            }

            if (CurrentACState == ACStateEnum.SMStarting)
                CurrentACState = ACStateEnum.SMCompleted;
        }

        public T ParentPWMethod<T>() where T : PWMethodVBBase
        {
            if (ParentRootWFNode == null)
                return null;
            return ParentRootWFNode as T;
        }

        public ACLabOrderManager LabOrderManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.LabOrderManager : null;
            }
        }

        public const string C_LabOrderExtFieldStats = "WStats";


        private void ParentPWGroup_ProcessModuleChanged(object sender, ProcessModuleChangedArgs e)
        {
            PWGroup pwGroup = sender as PWGroup;
            if (pwGroup != ParentPWGroup)
            {
                ParentPWGroup.ProcessModuleChanged -= ParentPWGroup_ProcessModuleChanged;
                return;
            }
            try
            {
                HandleProcessModuleMapping(e.Module, e.Removed);
            }
            catch (Exception ex)
            {
                Messages.LogException(this.GetACUrl(), "ParentPWGroup_ProcessModuleChanged", ex);
            }
        }

        private class Nodes2Stop
        {
            public Nodes2Stop(PWSamplePiLightBox node)
            {
                Node = node;
            }

            public PWSamplePiLightBox Node { get; private set; }

            private List<PAESamplePiLightBox> _Boxes = new List<PAESamplePiLightBox>();
            public List<PAESamplePiLightBox> Boxes
            {
                get
                {
                    return _Boxes;
                }
            }
        }

        private void HandleProcessModuleMapping(PAProcessModule paModule, bool pmReleased)
        {
            bool occupyPM = !pmReleased;
            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            if (pwMethodProduction == null)
                return;
            List<PAESamplePiLightBox> boxes = paModule.FindChildComponents<PAESamplePiLightBox>();
            if (boxes == null
                || !boxes.Any()
                || !boxes.Where(c => c.IsEnabledGetValues()).Any())
                return;

            List<Nodes2Stop> otherActiveNodesToStop = new List<Nodes2Stop>();
            List<PAESamplePiLightBox> boxesToStopMyself = new List<PAESamplePiLightBox>();
            List<PAESamplePiLightBox> boxesToStartMyself = new List<PAESamplePiLightBox>();
            foreach (PAESamplePiLightBox lightBox in boxes)
            {
                // If other workflow has this Box in usage
                if (!String.IsNullOrEmpty(lightBox.PWSampleNode)
                    && lightBox.PWSampleNode != this.GetACUrl())
                {
                    if (occupyPM)
                    {
                        PWSamplePiLightBox otherNode = ACUrlCommand(lightBox.PWSampleNode) as PWSamplePiLightBox;
                        if (otherNode != null)
                        {
                            Nodes2Stop nodeToStop = otherActiveNodesToStop.FirstOrDefault(c => c.Node == otherNode);
                            if (nodeToStop == null)
                            {
                                nodeToStop = new Nodes2Stop(otherNode);
                                otherActiveNodesToStop.Add(nodeToStop);
                            }
                            nodeToStop.Boxes.Add(lightBox);
                        }
                        boxesToStartMyself.Add(lightBox);
                    }
                    else
                    {
                        // Do nothing, because already unsubscribed through a another node that has occupied process module
                    }
                }
                // This box is occupied from me
                else
                {
                    if (pmReleased)
                        boxesToStopMyself.Add(lightBox);
                    else
                        boxesToStartMyself.Add(lightBox);
                }
            }

            foreach (Nodes2Stop nodeToStop in otherActiveNodesToStop)
            {
                nodeToStop.Node.StartAndStopLightBoxes(false, nodeToStop.Boxes);
            }
            if (boxesToStopMyself.Any())
                StartAndStopLightBoxes(false, boxesToStopMyself);
            if (boxesToStartMyself.Any())
                StartAndStopLightBoxes(true, boxesToStartMyself);
        }

        internal virtual void StartAndStopLightBoxes(bool startOrder, List<PAESamplePiLightBox> boxes)
        {
            bool stopOrder = !startOrder;
            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            if (pwMethodProduction == null)
                return;
            if (ParentPWGroup.TimeInfo.ValueT.ActualTimes == null
                || ParentPWGroup.TimeInfo.ValueT.ActualTimes.IsNull
                || !ParentPWGroup.TimeInfo.ValueT.ActualTimes.StartTimeValue.HasValue)
                return;

            DateTime startTimeFrom = ParentPWGroup.TimeInfo.ValueT.ActualTimes.StartTimeValue.Value.GetDateTimeDSTCorrected();

            double tolPlus = 0;
            double tolMinus = 0;
            double setPoint = 0;
            List<Tuple<double, double, double>> setPoints = new List<Tuple<double, double, double>>();

            ProdOrderPartslistPos intermediateChildPos;
            ProdOrderPartslistPos intermediatePosition;
            MaterialWFConnection matWFConnection;
            ProdOrderBatch batch;
            ProdOrderBatchPlan batchPlan;
            ProdOrderPartslistPos endBatchPos;
            MaterialWFConnection[] matWFConnections;

            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                bool posFound = PWDosing.GetRelatedProdOrderPosForWFNode(this, dbIPlus, dbApp, pwMethodProduction,
                    out intermediateChildPos, out intermediatePosition, out endBatchPos, out matWFConnection, out batch, out batchPlan, out matWFConnections);
                if (!posFound)
                    return;
                Material material = null;
                if (endBatchPos != null)
                    material = endBatchPos.BookingMaterial;
                if (material == null)
                    material = intermediatePosition.BookingMaterial;
                if (material == null)
                {
                    // Error50659 The posting material is not defined!
                    Msg msg = new Msg(this, eMsgLevel.Error, nameof(PWSamplePiLightBox), nameof(StartAndStopLightBoxes) + "(10)", 365, "Error50659");
                    OnNewAlarmOccurred(ProcessAlarm, msg);
                    return;
                }

                setPoint = material.ProductionWeight;
                if (setPoint <= Double.Epsilon)
                {
                    MaterialUnit materialUnit = material.MaterialUnit_Material.Where(c => c.ToMDUnit.SIDimension == GlobalApp.SIDimensions.Mass).FirstOrDefault();
                    if (materialUnit != null && materialUnit.ProductionWeight >= Double.Epsilon)
                        setPoint = materialUnit.ProductionWeight;
                }

                if (setPoint <= Double.Epsilon)
                {
                    // Error50658 The material has not defined a production weight! Please define a production weight in the Material master!
                    Msg msg = new Msg(this, eMsgLevel.Error, nameof(PWSamplePiLightBox), nameof(StartAndStopLightBoxes) + "(10)", 365, "Error50658");
                    OnNewAlarmOccurred(ProcessAlarm, msg);
                    return;
                }

                tolPlus = PAFDosing.RecalcAbsoluteTolerance(TolerancePlus, setPoint);
                tolMinus = PAFDosing.RecalcAbsoluteTolerance(ToleranceMinus, setPoint);

                // setPoints.Add(new Tuple<double, double, double>(setPoint, tolPlus, tolMinus));

                if (matWFConnections.Count() > 1)
                {
                    setPoints.Clear();

                    List<ProdOrderPartslistPos> relatedIntermediateChildren = GetReleatedIntermediates(matWFConnections, endBatchPos, intermediateChildPos, intermediatePosition);

                    foreach (ProdOrderPartslistPos relatedPos in relatedIntermediateChildren)
                    {
                        if (relatedPos == endBatchPos)
                        {
                            setPoints.Add(new Tuple<double, double, double>(setPoint, tolPlus, tolMinus));
                            continue;
                        }

                        if (relatedPos.MDUnit == null || relatedPos.MDUnit.SIDimension != GlobalApp.SIDimensions.Mass)
                            continue;

                        if (relatedPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Any(x => !x.SourceProdOrderPartslistPos.Material.IsIntermediate))
                        {
                            double setPointPos = relatedPos.TargetQuantityUOM / endBatchPos.TargetQuantityUOM;
                            double tolPlusPos = PAFDosing.RecalcAbsoluteTolerance(TolerancePlus, setPoint);
                            double tolMinusPos = PAFDosing.RecalcAbsoluteTolerance(ToleranceMinus, setPoint);
                            setPoints.Insert(0, (new Tuple<double, double, double>(setPointPos, tolPlusPos, tolMinusPos)));
                        }
                    }
                }

                if (!setPoints.Any())
                    setPoints.Add(new Tuple<double, double, double>(setPoint, tolPlus, tolMinus));
            }

            string labOrderTemplateName = LabOrderTemplateName;
            var labOrderManager = LabOrderManager;
            if (labOrderManager == null)
                return;

            this.ApplicationManager.ApplicationQueue.Add(() =>
            {
                if (stopOrder)
                {
                    using (var dbIPlus = new Database())
                    using (var dbApp = new DatabaseApp(dbIPlus))
                    {
                        Msg msg = null;
                        ProdOrderPartslistPos plPos = dbApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == intermediateChildPos.ProdOrderPartslistPosID);
                        if (plPos == null)
                        {
                            //Error50318: Can not find the ProdOrderPartslistPos in the database with ProdOrderPartslistPosID: {0}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "TaskCallback(10)", 357, "Error50318", intermediateChildPos.ProdOrderPartslistPosID);
                            AddAlarm(msg, true);
                            return;
                        }
                        foreach (PAESamplePiLightBox box in boxes)
                        {
                            box.GetValues();
                            if (box.StopOrder())
                            {
                                SamplePiStatsCollection statsCollection = box.GetArchivedValues(startTimeFrom, DateTime.Now, true);
                                if (statsCollection != null)
                                {
                                    foreach (SamplePiStats stats in statsCollection)
                                    {
                                        if (!stats.Values.Any())
                                            continue;

                                        stats.SetToleranceAndRecalc(stats.SetPoint, stats.TolPlus, stats.TolMinus);

                                        LabOrderPos labOrderPos = null;
                                        msg = PWSampleWeighing.CreateNewLabOrder(Root, this, labOrderManager, dbApp, plPos, labOrderTemplateName, Math.Round(stats.AverageValue, 3), null, PWSampleWeighing.StorageFormatEnum.PositionForEachWeighing, box.ComponentClass.ACClassID, out labOrderPos);

                                        if (msg == null && labOrderPos == null)
                                        {
                                            //Error50323: The LabOrder position Sample weight not exist.
                                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "TaskCallback(45)", 422, "Error50323");
                                        }
                                        if (msg != null)
                                        {
                                            AddAlarm(msg, true);
                                            return;
                                        }
                                        labOrderPos.ReferenceValue = stats.SetPoint;
                                        labOrderPos.ValueMax = stats.SetPoint + stats.TolPlus;
                                        labOrderPos.ValueMin = stats.SetPoint - stats.TolMinus;
                                        labOrderPos[C_LabOrderExtFieldStats] = stats;

                                        int seq = 1;
                                        if (labOrderPos.LabOrder.LabOrderPos_LabOrder.Count > 1)
                                        {
                                            seq = labOrderPos.LabOrder.LabOrderPos_LabOrder.Max(c => c.Sequence) + 1;
                                            labOrderPos.Sequence = seq;
                                        }

                                        gip.core.datamodel.ACClass machine = ParentPWGroup?.AccessedProcessModule?.ComponentClass;
                                        if (machine != null)
                                        {
                                            labOrderPos.LabOrder.RefACClassID = machine.ACClassID;
                                        }
                                    }
                                    msg = dbApp.ACSaveChanges();
                                }
                            }
                        }
                    }
                }
                else
                {
                    string acUrl = this.GetACUrl();

                    foreach (PAESamplePiLightBox box in boxes)
                    {
                        int sequence = 0;
                        int tryCounter = 0;

                        for (int i = 0; i < setPoints.Count; i++)
                        {
                            sequence++;

                            var setPointItem = setPoints[i];

                            if (!box.SetParamsAndStartOrder(setPointItem.Item1, setPointItem.Item2, setPointItem.Item3, acUrl, sequence))
                            {
                                bool result = box.StopOrder();
                                if (!result)
                                {
                                    Messages.LogError(this.GetACUrl(), nameof(StartAndStopLightBoxes) + "(10)", "Error with stop order!");
                                    result = box.StopOrder();
                                    if (!result)
                                    {
                                        Messages.LogError(this.GetACUrl(), nameof(StartAndStopLightBoxes) + "(11)", "Error with stop order!");
                                        break;
                                    }
                                }
                                i = 0; //Start again
                                tryCounter++;
                                if (tryCounter > 5)
                                {
                                    Msg msg = new Msg(this, eMsgLevel.Error, nameof(PWSamplePiLightBox), nameof(StartAndStopLightBoxes) + "(12)", 486, "Error50636");
                                    AddAlarm(msg, true);
                                    break;
                                }
                            }

                        }
                    }
                }
            });
        }

        public void ReadLogsAndStopFromLightBoxes()
        {
        }

        private void AddAlarm(Msg msg, bool autoAck)
        {
            OnNewAlarmOccurred(ProcessAlarm, msg, autoAck);
            ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;
            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                Messages.LogMessageMsg(msg);
        }


        [ACMethodInteraction("", "en{'Reactivate orders on light box'}de{'Aufträge auf Ampelbox reaktivieren'}", 650, true)]
        public void ReactivateOrderOnBox()
        {
            if (!IsEnabledReactivateOrderOnBox())
                return;
            HandleProcessModuleMapping(ParentPWGroup.AccessedProcessModule, false);
        }

        public bool IsEnabledReactivateOrderOnBox()
        {
            if (ParentPWGroup == null || ParentPWGroup.AccessedProcessModule == null)
                return false;
            PAProcessModule paModule = ParentPWGroup.AccessedProcessModule;
            List<PAESamplePiLightBox> boxes = paModule.FindChildComponents<PAESamplePiLightBox>();
            if (boxes == null
                || !boxes.Any()
                || !boxes.Where(c => c.IsEnabledGetValues()).Any())
                return false;
            return true;
        }


        [ACMethodState("en{'Running'}de{'Läuft'}", 30, true)]
        public override void SMRunning()
        {
        }

        [ACMethodState("en{'Completed'}de{'Beendet'}", 40, true)]
        public override void SMCompleted()
        {
            base.SMCompleted();
        }

        #endregion

        #region Protected

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(ReactivateOrderOnBox):
                    ReactivateOrderOnBox();
                    return true;
                case nameof(IsEnabledReactivateOrderOnBox):
                    result = IsEnabledReactivateOrderOnBox();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList, ref DumpStats dumpStats)
        {
            base.DumpPropertyList(doc, xmlACPropertyList, ref dumpStats);
        }

        protected List<ProdOrderPartslistPos> GetReleatedIntermediates(MaterialWFConnection[] connectionList, ProdOrderPartslistPos endBatchPos, ProdOrderPartslistPos intermediateChildPos, ProdOrderPartslistPos intermediatePosition)
        {
            List<ProdOrderPartslistPos> resultList = new List<ProdOrderPartslistPos>();

            GetRelatedMatWFConn(connectionList, endBatchPos, resultList);

            return resultList;
        }

        private void GetRelatedMatWFConn(MaterialWFConnection[] connectionList, ProdOrderPartslistPos currentPos, List<ProdOrderPartslistPos> resultList)
        {
            MaterialWFConnection matWFConnection = connectionList.FirstOrDefault(c => c.MaterialID == currentPos.MaterialID);

            if (matWFConnection != null)
            {
                resultList.Add(currentPos);
            }

            foreach (ProdOrderPartslistPos sourcePos in currentPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Select(x => x.SourceProdOrderPartslistPos))
            {
                GetRelatedMatWFConn(connectionList, sourcePos, resultList);
            }
        }


        #endregion
    }
}
