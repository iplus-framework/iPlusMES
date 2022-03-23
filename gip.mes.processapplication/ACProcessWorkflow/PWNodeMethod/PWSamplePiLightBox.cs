using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using System.Xml;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;

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
                        return (SamplePiCommand) acValue.ParamAsInt16;
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

        public string LabOrderTemplateName
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    ACValue acValue = method.ParameterValueList.GetACValue("LabOrderTemplateName");
                    if (acValue != null)
                        return !String.IsNullOrEmpty(acValue.ParamAsString) ? acValue.ParamAsString : PWSampleWeighing.C_LabOrderTemplateName;
                }
                return PWSampleWeighing.C_LabOrderTemplateName;
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

            if (ParentPWGroup != null)
                ParentPWGroup.ProcessModuleChanged += ParentPWGroup_ProcessModuleChanged;

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

            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            if (pwMethodProduction == null)
                return;
            List<PAESamplePiLightBox> boxes = e.Module.FindChildComponents<PAESamplePiLightBox>();
            if (boxes == null || !boxes.Any())
                return;

            if (   ParentPWGroup.TimeInfo.ValueT.ActualTimes == null
                || ParentPWGroup.TimeInfo.ValueT.ActualTimes.IsNull
                || !TimeInfo.ValueT.ActualTimes.StartTimeValue.HasValue)
                return;
            DateTime startTimeFrom = TimeInfo.ValueT.ActualTimes.StartTimeValue.Value;

            double tolPlus = 0;
            double tolMinus = 0;
            double setPoint = 0;
            ProdOrderPartslistPos intermediateChildPos;
            ProdOrderPartslistPos intermediatePosition;
            MaterialWFConnection matWFConnection;
            ProdOrderBatch batch;
            ProdOrderBatchPlan batchPlan;
            ProdOrderPartslistPos endBatchPos;

            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                bool posFound = PWDosing.GetRelatedProdOrderPosForWFNode(this, dbIPlus, dbApp, pwMethodProduction,
                    out intermediateChildPos, out intermediatePosition, out endBatchPos, out matWFConnection, out batch, out batchPlan);
                if (!posFound)
                    return;
                Material material = intermediatePosition.BookingMaterial;
                if (material == null)
                {
                    // TODO Error:
                    return;
                }

                MaterialUnit materialUnit = material.MaterialUnit_Material.Where(c => c.ToMDUnit.SIDimension == GlobalApp.SIDimensions.Mass).FirstOrDefault();
                if (materialUnit == null)
                {
                    // TODO Error:
                    return;
                }
                if (materialUnit.ProductionWeight <= Double.Epsilon)
                {
                    // TODO Error:
                    return;
                }
                setPoint = materialUnit.ProductionWeight;
                tolPlus = PAFDosing.RecalcAbsoluteTolerance(TolerancePlus, setPoint);
                tolMinus = PAFDosing.RecalcAbsoluteTolerance(ToleranceMinus, setPoint);
            }

            string labOrderTemplateName = LabOrderTemplateName;
            var labOrderManager = LabOrderManager;
            if (labOrderManager == null)
                return;
            this.ApplicationManager.ApplicationQueue.Add(() =>
            {
                if (e.Removed)
                {
                    using (var dbIPlus = new Database())
                    using (var dbApp = new DatabaseApp(dbIPlus))
                    {
                        Msg msg;
                        ProdOrderPartslistPos plPos = dbApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == intermediatePosition.ProdOrderPartslistPosID);
                        if (plPos == null)
                        {
                            //Error50318: Can not find the ProdOrderPartslistPos in the database with ProdOrderPartslistPosID: {0}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "TaskCallback(10)", 357, "Error50318", intermediatePosition.ProdOrderPartslistPosID);
                            AddAlarm(msg, true);
                            return;
                        }
                        foreach (PAESamplePiLightBox box in boxes)
                        {
                            box.GetValues();
                            if (box.StopOrder())
                            {
                                SamplePiStats stats = box.GetArchivedValues(startTimeFrom, DateTime.Now, true);
                                if (stats != null)
                                {
                                    LabOrderPos labOrderPos;
                                    msg = PWSampleWeighing.CreateNewLabOrder(Root, this, labOrderManager, dbApp, plPos, labOrderTemplateName, stats.AverageValue, null, out labOrderPos);
                                    if (msg == null && labOrderPos != null)
                                    {
                                        //Error50323: The LabOrder position Sample weight not exist.
                                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "TaskCallback(45)", 422, "Error50323");
                                    }
                                    if (msg != null)
                                    {
                                        AddAlarm(msg, true);
                                        return;
                                    }
                                    labOrderPos[C_LabOrderExtFieldStats] = stats;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (PAESamplePiLightBox box in boxes)
                    {
                        if (!box.SetParamsAndStartOrder(setPoint, tolPlus, tolMinus))
                        {
                            // TODO: Error
                        }
                    }
                }
            });
        }

        private void AddAlarm(Msg msg, bool autoAck)
        {
            OnNewAlarmOccurred(ProcessAlarm, msg, autoAck);
            ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;
            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                Messages.LogMessageMsg(msg);
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

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);
        }

        #endregion
    }
}
