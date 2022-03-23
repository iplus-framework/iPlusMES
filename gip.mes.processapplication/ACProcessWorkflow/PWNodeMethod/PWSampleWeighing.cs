using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Sample weighing'}de{'Sample weighing'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWSampleWeighing : PWNodeProcessMethod
    {
        #region c'tors

        public const string PWClassName = "PWSampleWeighing";

        static PWSampleWeighing()
        {
            ACMethod method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWSampleWeighing), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWSampleWeighing), ACStateConst.SMStarting, wrapper);
        }

        public PWSampleWeighing(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;


            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {

            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        public const string C_LabOrderTemplateName = "LOSampleWeighingTemplate";
        public const string C_LabOrderPosTagKey = "StoredSampleWeight";

        private gip.core.datamodel.ACProgramLog _NewAddedProgramLog = null;

        public ACProdOrderManager ProdOrderManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.ProdOrderManager : null;
            }
        }

        public ACLabOrderManager LabOrderManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.LabOrderManager : null;
            }
        }

        #endregion

        #region Methods

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            var pwGroup = ParentPWGroup;
            if (pwGroup == null) // Is null when Service-Application is shutting down
            {
                if (this.InitState == ACInitState.Initialized)
                    Messages.LogError(this.GetACUrl(), "SMStarting()", "ParentPWGroup is null");
                return;
            }

            core.datamodel.ACClassMethod refPAACClassMethod = null;
            if (this.ContentACClassWF != null)
            {

                using (ACMonitor.Lock(this.ContextLockForACClassWF))
                {
                    refPAACClassMethod = this.ContentACClassWF.RefPAACClassMethod;
                }
            }

            if (pwGroup != null
                && this.ContentACClassWF != null
                && refPAACClassMethod != null)
            {
                PAProcessModule module = null;
                if (ParentPWGroup.NeedsAProcessModule && (ACOperationMode == ACOperationModes.Live || ACOperationMode == ACOperationModes.Simulation))
                    module = ParentPWGroup.AccessedProcessModule;
                // Testmode
                else
                    module = ParentPWGroup.ProcessModuleForTestmode;

                Msg msg = null;

                if (module == null)
                {
                    //Error50372: The workflow group has not occupied a process module.
                    // Die Workflowgruppe hat kein Prozessmodul belegt.
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(10)", 1000, "Error50372");
                    ActivateProcessAlarmWithLog(msg, false);
                    SubscribeToProjectWorkCycle();
                    return;
                }

                PAFSampleWeighing sampleWeighing = CurrentExecutingFunction<PAFSampleWeighing>();
                if (sampleWeighing != null)
                {
                    if (CurrentACState == ACStateEnum.SMStarting)
                        CurrentACState = ACStateEnum.SMRunning;
                    return;
                }

                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                // If dosing is not for production, then do nothing
                if (pwMethodProduction == null)
                {
                    //TODO: completed
                    return;
                }

                if (ProdOrderManager == null)
                {
                    // Error50311: ProdOrderManager is null.
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(20)", 141, "Error50311");
                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, false);
                    return;
                }

                using (var dbIPlus = new Database())
                {
                    using (var dbApp = new DatabaseApp(dbIPlus))
                    {
                        ProdOrderPartslistPos endBatchPos = pwMethodProduction.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                        if (pwMethodProduction.CurrentProdOrderBatch == null)
                        {
                            // Error50312: No batch assigned to last intermediate material of this workflow-process.
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(30)", 156, "Error50312");

                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, false);
                            return;
                        }

                        var contentACClassWFVB = ContentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(dbApp);
                        ProdOrderBatch batch = pwMethodProduction.CurrentProdOrderBatch.FromAppContext<ProdOrderBatch>(dbApp);
                        ProdOrderBatchPlan batchPlan = batch.ProdOrderBatchPlan;
                        MaterialWFConnection matWFConnection = null;
                        if (batchPlan != null && batchPlan.MaterialWFACClassMethodID.HasValue)
                        {
                            matWFConnection = dbApp.MaterialWFConnection
                                                    .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan.MaterialWFACClassMethodID.Value
                                                            && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                                    .FirstOrDefault();
                        }
                        else
                        {
                            PartslistACClassMethod plMethod = endBatchPos.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                            if (plMethod != null)
                            {
                                matWFConnection = dbApp.MaterialWFConnection
                                                        .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                                && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                                        .FirstOrDefault();
                            }
                            else
                            {
                                matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
                                    .Where(c => c.MaterialWFACClassMethod.MaterialWFID == endBatchPos.ProdOrderPartslist.Partslist.MaterialWFID
                                                && c.MaterialWFACClassMethod.PartslistACClassMethod_MaterialWFACClassMethod.Where(d => d.PartslistID == endBatchPos.ProdOrderPartslist.PartslistID).Any())
                                    .FirstOrDefault();
                            }
                        }

                        if (matWFConnection == null)
                        {
                            // Error50313: No relation defined between Workflownode and intermediate material in Materialworkflow.
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(40)", 197, "Error50313");

                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, false);
                            return;
                        }

                        // Find intermediate position which is assigned to this Dosing-Workflownode
                        var currentProdOrderPartslist = endBatchPos.ProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
                        ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                            .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                                && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                                && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
                        if (intermediatePosition == null)
                        {
                            // Error50314: Intermediate line not found which is assigned to this PWSampleWeighing-Workflownode
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(50)", 214, "Error50314");

                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, false);
                            return;
                        }

                        ProdOrderPartslistPos intermediateChildPos = null;
                        // Lock, if a parallel Dosing also creates a child Position for this intermediate Position

                        using (ACMonitor.Lock(pwMethodProduction._62000_PWGroupLockObj))
                        {
                            // Find intermediate child position, which is assigned to this Batch
                            intermediateChildPos = intermediatePosition.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                                .Where(c => c.ProdOrderBatchID.HasValue
                                            && c.ProdOrderBatchID.Value == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID)
                                .FirstOrDefault();
                        }
                        if (intermediateChildPos == null)
                        {
                            //Error50315:intermediateChildPos is null.
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(60)", 236, "Error50315");
                            OnNewAlarmOccurred(ProcessAlarm, msg);
                            return;
                        }

                        ACMethod acMethod = refPAACClassMethod.TypeACSignature();
                        if (acMethod == null)
                        {
                            //Error50316: acMethod is null.
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(70)", 245, "Error50316");
                            OnNewAlarmOccurred(PropNameProcessAlarm, msg, false);
                            SubscribeToProjectWorkCycle();
                            return;
                        }

                        acMethod.ParameterValueList["PLPos"] = intermediateChildPos.ProdOrderPartslistPosID;

                        if (!acMethod.IsValid())
                        {
                            // Error50317: Sample weighing task is not startable Order {0}, Bill of material {1}, line {2}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(80)", 256, "Error50317", currentProdOrderPartslist.ProdOrder.ProgramNo,
                                                                                                                   currentProdOrderPartslist.Partslist.PartslistNo,
                                                                                                                   intermediateChildPos.BookingMaterial.MaterialName1);

                            if (IsAlarmActive(PropNameProcessAlarm, msg.Message) == null)
                                Root.Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(PropNameProcessAlarm, msg, false);
                            SubscribeToProjectWorkCycle();
                            return;
                        }

                        RecalcTimeInfo();
                        if (CreateNewProgramLog(acMethod, _NewAddedProgramLog == null) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                            return;

                        module.TaskInvocationPoint.ClearMyInvocations(this);
                        _CurrentMethodEventArgs = null;
                        if (!IsTaskStarted(module.TaskInvocationPoint.AddTask(acMethod, this)))
                        {
                            ACMethodEventArgs eM = _CurrentMethodEventArgs;
                            if (eM == null || eM.ResultState != Global.ACMethodResultState.FailedAndRepeat)
                            {
                                // Error50317: Sample weighing task is not startable Order {0}, Bill of material {1}, line {2}
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(90)", 279, "Error50317", currentProdOrderPartslist.ProdOrder.ProgramNo,
                                                                                                                       currentProdOrderPartslist.Partslist.PartslistNo,
                                                                                                                       intermediateChildPos.BookingMaterial.MaterialName1);

                                if (IsAlarmActive(PropNameProcessAlarm, msg.Message) == null)
                                    Root.Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                OnNewAlarmOccurred(PropNameProcessAlarm, msg, false);
                            }
                            SubscribeToProjectWorkCycle();
                            return;
                        }
                        UpdateCurrentACMethod();

                        if (CurrentACState == ACStateEnum.SMStarting)
                        {
                            CurrentACState = ACStateEnum.SMRunning;
                            RaiseRunningEvent();
                        }
                    }
                }
            }
        }

        public T ParentPWMethod<T>() where T : PWMethodVBBase
        {
            if (ParentRootWFNode == null)
                return null;
            return ParentRootWFNode as T;
        }

        protected override void OnNewProgramLogAddedToQueue(ACMethod acMethod, gip.core.datamodel.ACProgramLog currentProgramLog)
        {
            if (_NewAddedProgramLog == null)
            {
                _NewAddedProgramLog = currentProgramLog;
                //ACClassTaskQueue.TaskQueue.ObjectContext.ACChangesExecuted += ACClassTaskQueue_ChangesSaved;
            }
        }

        public override void Start()
        {
            base.Start();
        }

        public override void TaskCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            _InCallback = true;
            if (e != null)
            {
                IACTask taskEntry = wrapObject as IACTask;
                ACMethodEventArgs eM = e as ACMethodEventArgs;
                _CurrentMethodEventArgs = eM;
                if (taskEntry.State == PointProcessingState.Deleted && CurrentACState != ACStateEnum.SMIdle)
                {
                    try
                    {
                        PAProcessModule module = sender.ParentACComponent as PAProcessModule;
                        if (module != null)
                        {
                            PAProcessFunction function = module.GetExecutingFunction<PAProcessFunction>(eM.ACRequestID);
                            if (function != null)
                            {
                                if (function.CurrentACState == ACStateEnum.SMAborted)
                                {
                                    if (CurrentACState == ACStateEnum.SMRunning)
                                        CurrentACState = ACStateEnum.SMCompleted;
                                    _InCallback = false;
                                    return;
                                }

                                Guid plPosID = function.CurrentACMethod.ValueT.ParameterValueList.GetGuid("PLPos");
                                if (plPosID != Guid.Empty)
                                {
                                    ACValue actualWeightParam = function.CurrentACMethod.ValueT.ResultValueList.GetACValue("ActualWeight");
                                    double actualWeight = 0;
                                    if (actualWeightParam != null)
                                        actualWeight = actualWeightParam.ParamAsDouble;

                                    ACValue alibiNoParam = function.CurrentACMethod.ValueT.ResultValueList.GetACValue("AlibiNo");
                                    string alibiNo = "";
                                    if (alibiNoParam != null)
                                        alibiNo = alibiNoParam.ParamAsString;

                                    using (Database db = new core.datamodel.Database())
                                    using (DatabaseApp dbApp = new DatabaseApp(db))
                                    {
                                        Msg msg;
                                        ProdOrderPartslistPos plPos = dbApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == plPosID);
                                        if (plPos == null)
                                        {
                                            //Error50318: Can not find the ProdOrderPartslistPos in the database with ProdOrderPartslistPosID: {0}
                                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "TaskCallback(10)", 357, "Error50318", plPosID);
                                            AddAlarm(msg, true);
                                            _InCallback = false;
                                            return;
                                        }

                                        LabOrderPos labOrderPos;
                                        msg = CreateNewLabOrder(Root, this, LabOrderManager, dbApp, plPos, C_LabOrderTemplateName, actualWeight, alibiNo, out labOrderPos);
                                        if (msg == null && labOrderPos != null)
                                        {
                                            //Error50323: The LabOrder position Sample weight not exist.
                                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "TaskCallback(45)", 422, "Error50323");
                                        }
                                        if (msg != null)
                                        {
                                            AddAlarm(msg, true);
                                            _InCallback = false;
                                            return;
                                        }

                                        if (CurrentACState == ACStateEnum.SMRunning)
                                            CurrentACState = ACStateEnum.SMCompleted;
                                    }
                                }
                                else
                                {
                                    //Error50322: The ACValue PLPos is null from PAFSampleWeighing ACMethod parameters.
                                    Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "TaskCallback(50)", 443, "Error50322");
                                    AddAlarm(msg, true);
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        Messages.LogException(this.GetACUrl(), "TaskCallback(60)", exc);
                    }
                }
            }
            _InCallback = false;
        }

        public static Msg CreateNewLabOrder(IRoot root, IACComponent requester, ACLabOrderManager labOrderManager, DatabaseApp dbApp, ProdOrderPartslistPos plPos, string templateName, double actualWeight, string alibiNo, out LabOrderPos labOrderPos)
        {
            labOrderPos = null;
            string secondaryKey = root.NoManager.GetNewNo(dbApp, typeof(Weighing), Weighing.NoColumnName, Weighing.FormatNewNo, requester);
            Weighing weighing = Weighing.NewACObject(dbApp, null, secondaryKey);
            weighing.Weight = actualWeight;
            weighing.IdentNr = alibiNo == null ? "" : alibiNo;
            dbApp.Weighing.AddObject(weighing);

            Msg msg = dbApp.ACSaveChanges();
            if (msg != null)
            {
                dbApp.ACUndoChanges();
                return msg;
            }

            LabOrder sampleWeighingTemplate = dbApp.LabOrder.FirstOrDefault(c => c.LabOrderTypeIndex == (short)GlobalApp.LabOrderType.Template
                                                                              && c.TemplateName == templateName);

            if (sampleWeighingTemplate == null)
            {
                msg = CreateLabOrderTemplate(root, requester, dbApp, plPos.BookingMaterial, templateName, out sampleWeighingTemplate);
                if (msg != null)
                    return msg;
            }

            LabOrder labOrder = null;
            msg = labOrderManager.CreateNewLabOrder(dbApp, sampleWeighingTemplate, "Sample weighing", null, null, plPos, null, out labOrder);
            if (msg != null)
                return msg;

            if (labOrder.LabOrderPos_LabOrder.Any())
                labOrderPos = labOrder.LabOrderPos_LabOrder.Where(c => c.MDLabTag.MDKey == C_LabOrderPosTagKey).FirstOrDefault();
            if (labOrderPos == null)
                return null;

            weighing.LabOrderPos = labOrderPos;
            labOrderPos.ActualValue = weighing.Weight;

            msg = dbApp.ACSaveChanges();
            return msg;
        }

        public static Msg CreateLabOrderTemplate(IRoot root, IACComponent requester, DatabaseApp dbApp, Material material, string templateName, out LabOrder template)
        {
            string secondaryKey = root.NoManager.GetNewNo(dbApp, typeof(LabOrder), LabOrder.NoColumnName, LabOrder.FormatNewNo, requester);
            template = LabOrder.NewACObject(dbApp, null, secondaryKey);
            template.LabOrderTypeIndex = (short)GlobalApp.LabOrderType.Template;
            template.MDLabOrderState = dbApp.MDLabOrderState.FirstOrDefault(c => c.IsDefault);
            template.Material = material;
            template.SampleTakingDate = DateTime.Now;
            template.TemplateName = templateName;
            dbApp.LabOrder.AddObject(template);

            Msg msg = dbApp.ACSaveChanges();
            if (msg != null)
            {
                dbApp.ACUndoChanges();
                template = null;
                return msg;
            }

            MDLabTag labTag = dbApp.MDLabTag.FirstOrDefault(c => c.MDKey == C_LabOrderPosTagKey);
            if (labTag == null)
            {
                labTag = MDLabTag.NewACObject(dbApp, null);
                labTag.MDNameTrans = "en{'Sample weight'}de{'Stichproben gewicht'}";
                labTag.SortIndex = 10000;
                labTag.MDKey = C_LabOrderPosTagKey;
                dbApp.MDLabTag.AddObject(labTag);

                msg = dbApp.ACSaveChanges();
                if (msg != null)
                {
                    dbApp.ACUndoChanges();
                    template = null;
                    return msg;
                }
            }

            LabOrderPos templatePos = LabOrderPos.NewACObject(dbApp, template);
            templatePos.LabOrder = template;
            templatePos.MDLabTag = labTag;
            templatePos.LineNumber = "1";
            templatePos.LabOrder = template;
            dbApp.LabOrderPos.AddObject(templatePos);

            msg = dbApp.ACSaveChanges();
            if (msg != null)
            {
                dbApp.ACUndoChanges();
                template = null;
                return msg;
            }
            return null;
        }

        private void AddAlarm(Msg msg, bool autoAck)
        {
            OnNewAlarmOccurred(ProcessAlarm, msg, autoAck);
            ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;
            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                Messages.LogMessageMsg(msg);
        }

        #endregion
    }
}
