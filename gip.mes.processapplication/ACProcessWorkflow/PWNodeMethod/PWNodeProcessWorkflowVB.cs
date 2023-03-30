using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Xml;
using System.Data.Objects;
using gip.mes.facility;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Process-Knoten zur implementierung eines (asynchronen) Workflow-ACClassMethod-Aufruf auf die Model-Welt
    /// 
    /// Methoden zur Steuerung von außen: 
    /// -Start()    Starten des Processes
    ///
    /// Mögliche ACState:
    /// SMIdle      (Definiert in ACComponent)
    /// SMStarting (Definiert in PWNode)
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'WF-Batch-Manager'}de{'WF-Batch-Manager'}", Global.ACKinds.TPWNodeWorkflow, Global.ACStorableTypes.Optional, false, PWProcessFunction.PWClassName, true)]
    public partial class PWNodeProcessWorkflowVB : PWNodeProcessWorkflow
    {
        public new const string PWClassName = "PWNodeProcessWorkflowVB";

        #region c´tors
        static PWNodeProcessWorkflowVB()
        {
            ACMethod method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("MethodPriorisation", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("MethodPriorisation", "en{'Start if all batches of other order are started'}de{'Starte, wenn alle Batche von anderem Auftrag gestartet'}");
            method.ParameterValueList.Add(new ACValue("IgnoreFIFO", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("IgnoreFIFO", "en{'Ignore priorisation'}de{'Ignoriere Priorisierung'}");
            method.ParameterValueList.Add(new ACValue("MethodIDCompare", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("MethodIDCompare", "en{'Compare only identical identifier'}de{'Vergleiche nur gleiche Identifier'}");
            method.ParameterValueList.Add(new ACValue("BatchSizeLoss", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("BatchSizeLoss", "en{'Batchsize inputdependant (Lossreduction)'}de{'Batchgröße einsatzabhängig (reduziert um Verlust)'}");
            method.ParameterValueList.Add(new ACValue("AlarmOnCompleted", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("AlarmOnCompleted", "en{'Alarm on completed batch plan'}de{'Alarmmeldung wenn Batchplan abgearbeitet'}");


            #region Batch Duration Planning - Forecast Scheduler - Params

            // BatchSizeStandard
            method.ParameterValueList.Add(new ACValue(ProdOrderBatchPlan.C_BatchSizeStandard, typeof(double), false, Global.ParamOption.Optional));
            paramTranslation.Add(ProdOrderBatchPlan.C_BatchSizeStandard, "en{'Standard Batchsize'}de{'Standard Batchgröße'}");
            
            // BatchSizeMin
            method.ParameterValueList.Add(new ACValue(ProdOrderBatchPlan.C_BatchSizeMin, typeof(double), false, Global.ParamOption.Optional));
            paramTranslation.Add(ProdOrderBatchPlan.C_BatchSizeMin, "en{'Min. Batchsize'}de{'Min. Batchgröße'}");
            
            // BatchSizeMax
            method.ParameterValueList.Add(new ACValue(ProdOrderBatchPlan.C_BatchSizeMax, typeof(double), false, Global.ParamOption.Optional));
            paramTranslation.Add(ProdOrderBatchPlan.C_BatchSizeMax, "en{'Max. Batchsize'}de{'Max. Batchgröße'}");
            
            // PlanMode
            method.ParameterValueList.Add(new ACValue(ProdOrderBatchPlan.C_PlanMode, typeof(BatchPlanMode), false, Global.ParamOption.Optional));
            paramTranslation.Add(ProdOrderBatchPlan.C_PlanMode, "en{'Batch planning mode'}de{'Batch Planmodus'}");
            
            // BatchSuggestionCommandModeEnum
            method.ParameterValueList.Add(new ACValue(ProdOrderBatchPlan.C_BatchSuggestionMode, typeof(BatchSuggestionCommandModeEnum), false, Global.ParamOption.Optional));
            paramTranslation.Add(ProdOrderBatchPlan.C_BatchSuggestionMode, "en{'Batch suggestion mode'}de{'Batch-Vorschlagsmodus'}");

            // DurationSecAVG
            method.ParameterValueList.Add(new ACValue(ProdOrderBatchPlan.C_DurationSecAVG, typeof(int), false, Global.ParamOption.Optional));
            paramTranslation.Add(ProdOrderBatchPlan.C_DurationSecAVG, "en{'Batch Duration AVG (s)'}de{'Batch Duration AVG (s)'}");

            // StartOffsetSecAVG
            method.ParameterValueList.Add(new ACValue(ProdOrderBatchPlan.C_StartOffsetSecAVG, typeof(int), false, Global.ParamOption.Optional));
            paramTranslation.Add(ProdOrderBatchPlan.C_StartOffsetSecAVG, "en{'Starting Batch Offset AVG (s)'}de{'Starting Batch Offset AVG (s)'}");

            // LineOrderInPlan
            method.ParameterValueList.Add(new ACValue("LineOrderInPlan", typeof(int), false, Global.ParamOption.Optional));
            paramTranslation.Add("LineOrderInPlan", "en{'Order of line selection in production plan'}de{'Reihenfolge der Linienauswahl im Produktionsplan'}");

            // OffsetToEndTime
            method.ParameterValueList.Add(new ACValue(ProdOrderBatchPlan.C_OffsetToEndTime, typeof(TimeSpan?), null, Global.ParamOption.Optional));
            paramTranslation.Add(ProdOrderBatchPlan.C_OffsetToEndTime, "en{'Duration offset for completion date based scheduling'}de{'Daueroffset zur Fertigstellungszeit-basierten Planung'}");

            #endregion

            method.ParameterValueList.Add(new ACValue("SkipWaitingNodes", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("SkipWaitingNodes", "en{'Skip waiting nodes'}de{'Ignoriere wartende Knoten'}");

            method.ParameterValueList.Add(new ACValue("CompleteIfNotPlan", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("CompleteIfNotPlan", "en{'Complete if no batches planned'}de{'Beende falls kein Batche geplant'}");

            method.ParameterValueList.Add(new ACValue("StartNextStage", typeof(StartNextStageMode), (short)StartNextStageMode.DoNothing, Global.ParamOption.Required));
            paramTranslation.Add("StartNextStage", "en{'Start next production stage'}de{'Nächste Fertigungsstufe starten'}");

            method.ParameterValueList.Add(new ACValue("EndPList", typeof(EndPListMode), (short)EndPListMode.DoNothing, Global.ParamOption.Required));
            paramTranslation.Add("EndPList", "en{'Mode for ending BOM-State'}de{'Stücklistenstatus Beenden Modus'}");

            method.ParameterValueList.Add(new ACValue("ValidationBehaviour", typeof(short), (short)PARole.ValidationBehaviour.Strict, Global.ParamOption.Optional));
            paramTranslation.Add("ValidationBehaviour", "en{'Validationbehaviour'}de{'Validierungsverhalten'}");

            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWNodeProcessWorkflowVB), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWNodeProcessWorkflowVB), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWNodeProcessWorkflowVB), HandleExecuteACMethod_PWNodeProcessWorkflowVB);
        }

        public PWNodeProcessWorkflowVB(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool result = base.ACInit(startChildMode);
            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            UnSubscribeToProjectWorkCycle();
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _CurrentProdOrderPartslist = null;
                _MaterialWFConnection = null;
                _IgnoreFIFO = null;
                _IsLastBatch2Start = (Int16)PADosingLastBatchEnum.None;
                _LastPriorityTime = null;
                _BatchPlanningTimes = null;
                _NewChildPosForBatch_ProdOrderPartslistPosID = null;
                _NewChildPosForBatch_PickingPosID = null;
                _PlanningWait = null;
                _PreventHandleStartNextBatchAgain = false;
                _StartNextBatchAtProjectID1 = null;
                _StartNextBatchAtProjectID2 = null;
                _ACProgramVB = null;
                _MDSchedulingGroupLoaded = false;
                _MDSchedulingGroup = null;
            }

            bool result = base.ACDeInit(deleteACClassTask);
            return result;
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _CurrentProdOrderPartslist = null;
                _MaterialWFConnection = null;
                _IgnoreFIFO = null;
                _IsLastBatch2Start = (Int16)PADosingLastBatchEnum.None;
                _LastPriorityTime = null;
                _BatchPlanningTimes = null;
                _NewChildPosForBatch_ProdOrderPartslistPosID = null;
                _NewChildPosForBatch_PickingPosID = null;
                _PlanningWait = null;
                _PreventHandleStartNextBatchAgain = false;
                _StartNextBatchAtProjectID1 = null;
                _StartNextBatchAtProjectID2 = null;
                _ACProgramVB = null;
                _MDSchedulingGroupLoaded = false;
                _MDSchedulingGroup = null;
            }

            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }
        #endregion

        #region Enums
        [Flags]
        public enum StartNextBatchResult
        {
            WaitForNextEvent = 0x01,
            CycleWait = 0x02,
            StartNextBatch = 0x04,
            Done = 0x08
        }

        public enum NextBatchState
        {
            /// <summary>
            /// If there are no batchplans that are active (>= GlobalApp.BatchPlanState.AutoStart && <= GlobalApp.BatchPlanState.InProcess)
            /// </summary>
            NoPlanEntryFound = 0,

            /// <summary>
            /// All Batchplans that were activated with GlobalApp.BatchPlanState.AutoStart are completed 
            /// </summary>
            CompletedNoNewEntry = 1,

            /// <summary>
            /// When PartialQuantity in a Batchplan was reached but the total batch count is not reached
            /// </summary>
            UncompletedButPartialQuantityReached = 2,


            /// <summary>
            /// One Batch more can be started
            /// </summary>
            EntryFound = 3
        }
        #endregion

        #region Private Class
        protected class BatchPlanningTime
        {
            public Guid ProdOrderBatchPlanID
            {
                get;
                set;
            }

            public DateTime PlannedTime
            {
                get;
                set;
            }

            public GlobalApp.BatchPlanState BatchPlanState
            {
                get;
                set;
            }

            public override string ToString()
            {
                return String.Format("{0};{1};{2}", ProdOrderBatchPlanID, PlannedTime, BatchPlanState);
            }
        }
        #endregion

        #region Properties
        #region Manager
        public override void ClearMyConfiguration()
        {
            base.ClearMyConfiguration();
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _IgnoreFIFO = null;
            }
        }

        /// <summary>
        /// Filter-Option: This node can start if predecessor has started all of its batches. Otherwise, wait until predecessor has completed.
        /// </summary>
        public bool CanStartIfPredecessorIsStopping
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    // Dont change ACIdentifier, because customers already use this wrong identifier.
                    var acValue = method.ParameterValueList.GetACValue("MethodPriorisation");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// If MethodPriorisation is set to true:
        /// Then compare only Nodes which have the same ACIdentifier. (e.g. Mixery(0) - Mixery(1) - Mixery(3) are different)
        /// This compare ist needed, if there are Planning-Workflows with multiple Planning-Nodes, 
        /// that references the same Intermediate-Material AND points to the same subworkflow.
        /// In this case, the called subworkflow has different configurations and routing rules that ensures a right priorization.
        /// For example: There are three roasters that can be parallely started, but all three are from the same production line (Application)
        /// Inside the invoked subworkflows are different routing rules for each roaster.
        /// </summary>
        public bool CompareOnlySameACIdentifers
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    // Dont change ACIdentifier, because customers already use this wrong identifier.
                    var acValue = method.ParameterValueList.GetACValue("MethodIDCompare");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// If Parameter is set, and predecessor can't start its batchplan, than ignore predecessor for priorization
        /// </summary>
        public bool SkipWaitingNodes
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("SkipWaitingNodes");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Wokflownode should be automatically completed if no batch was planned for this node
        /// </summary>
        public bool CompleteIfNotPlan
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("CompleteIfNotPlan");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }


        /// <summary>
        /// Gibt an ob bei der Erzeugung von neuen Batchen die geplante Batchgröße um den Verlustfaktor/Schwund reduziert werden soll
        /// </summary>
        public bool BatchSizeLoss
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("BatchSizeLoss");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        bool? _IgnoreFIFO = null;
        public bool IgnoreFIFO
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_IgnoreFIFO.HasValue)
                        return _IgnoreFIFO.Value;
                }
                bool ignoreFIFO = false;
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("IgnoreFIFO");
                    if (acValue != null)
                    {
                        ignoreFIFO = acValue.ParamAsBoolean;
                    }
                }

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _IgnoreFIFO = ignoreFIFO;
                    return _IgnoreFIFO.Value;
                }
            }
            set
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _IgnoreFIFO = value;
                }
            }
        }

        protected Int16 _IsLastBatch2Start = (Int16) PADosingLastBatchEnum.None;
        protected Int16 IsLastBatch2Start
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _IsLastBatch2Start;
                }
            }
            set
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _IsLastBatch2Start = value;
                }
            }
        }

        public StartNextStageMode StartNextStage
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("StartNextStage");
                    if (acValue != null)
                    {
                        return (StartNextStageMode) acValue.ParamAsInt16;
                    }
                }
                return StartNextStageMode.DoNothing;
            }
        }

        public EndPListMode EndProdOrderPartslistMode
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("EndPList");
                    if (acValue != null)
                    {
                        return (EndPListMode)acValue.ParamAsInt16;
                    }
                }
                return EndPListMode.DoNothing;
            }
        }

        public PARole.ValidationBehaviour ValidationBehaviour
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ValidationBehaviour");
                    if (acValue != null)
                    {
                        short validBehv = acValue.ParamAsInt16;
                        if (validBehv >= (short)PARole.ValidationBehaviour.Laxly && validBehv <= (short)PARole.ValidationBehaviour.Strict)
                            return (PARole.ValidationBehaviour)validBehv;
                    }
                }
                return PARole.ValidationBehaviour.Strict;
            }
        }
        

        public bool AlarmOnCompleted
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("AlarmOnCompleted");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }


        #endregion  

        #region Common Properties
        private gip.mes.datamodel.ACProgram _ACProgramVB = null;
        public gip.mes.datamodel.ACProgram ACProgramVB
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_ACProgramVB != null)
                        return _ACProgramVB;
                }
                LoadVBEntities();

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _ACProgramVB;
                }
            }
        }

        private bool _MDSchedulingGroupLoaded = false;
        private gip.mes.datamodel.MDSchedulingGroup _MDSchedulingGroup = null;
        public gip.mes.datamodel.MDSchedulingGroup MDSchedulingGroup
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_MDSchedulingGroupLoaded)
                        return _MDSchedulingGroup;
                }
                LoadVBEntities();
                _MDSchedulingGroupLoaded = true;

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _MDSchedulingGroup;
                }
            }
        }


        public DateTime? _LastPriorityTime = null;
        public DateTime PriorityTime
        {
            get
            {
                var batchPlanningTimes = BatchPlanningTimes;
                if (batchPlanningTimes == null || !batchPlanningTimes.Any())
                {
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        if (_LastPriorityTime.HasValue)
                            return _LastPriorityTime.Value;
                    }
                    if (this.RootPW != null
                              && this.RootPW.TimeInfo.ValueT != null
                              && this.RootPW.TimeInfo.ValueT.ActualTimes != null)
                        return this.RootPW.TimeInfo.ValueT.ActualTimes.StartTime;
                    else if (this.TimeInfo.ValueT != null && this.TimeInfo.ValueT.ActualTimes.StartTime > DateTime.MinValue)
                        return this.TimeInfo.ValueT.ActualTimes.StartTime;
                    else
                        return ACProgramVB.InsertDate;
                }
                else
                {
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        _LastPriorityTime = batchPlanningTimes.FirstOrDefault().PlannedTime;
                        return _LastPriorityTime.Value;
                    }
                }
            }
        }


        [ACPropertyBindingSource(9999, "", "en{'Substate of a process object'}de{'Unterzustand eines Prozessobjekts'}", "", false, true)]
        public IACContainerTNet<uint> ACSubState { get; set; }

        public ACSubStateEnum CurrentACSubState
        {
            get
            {
                if (ACSubState == null)
                    return 0;
                return (ACSubStateEnum) ACSubState.ValueT;
            }
            set
            {
                ACSubState.ValueT = (uint) value;
            }
        }

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

        public bool IsIntake
        {
            get
            {
                return ParentPWMethod<PWMethodIntake>() != null;
            }
        }

        public bool IsLoading
        {
            get
            {
                return ParentPWMethod<PWMethodLoading>() != null;
            }
        }

        public bool IsRelocation
        {
            get
            {
                return ParentPWMethod<PWMethodRelocation>() != null;
            }
        }

        public bool IsTransport
        {
            get
            {
                return ParentPWMethod<PWMethodTransportBase>() != null;
            }
        }

        Guid? _StartNextBatchAtProjectID1 = null;
        protected Guid? StartNextBatchAtProjectID1
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _StartNextBatchAtProjectID1;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _StartNextBatchAtProjectID1 = value;
                }
            }
        }

        Guid? _StartNextBatchAtProjectID2 = null;
        protected Guid? StartNextBatchAtProjectID2
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _StartNextBatchAtProjectID2;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _StartNextBatchAtProjectID2 = value;
                }
            }
        }

        #endregion

        #region overrides
        //public override ACConfigStageList ACConfigStageList
        //{
        //    get
        //    {
        //        return base.ACConfigStageList;
        //    }
        //}

        public override List<ACComponent> InvokableTaskExecutors
        {
            get
            {
                List<ACComponent> invokableTaskExecutors = base.InvokableTaskExecutors;
                Guid? startNextBatchAtProjectID1 = StartNextBatchAtProjectID1;
                Guid? startNextBatchAtProjectID2 = StartNextBatchAtProjectID2;
                if (   (startNextBatchAtProjectID1.HasValue || startNextBatchAtProjectID2.HasValue) 
                    && IsTransport
                    && invokableTaskExecutors.Any())
                {
                    if (startNextBatchAtProjectID1.HasValue)
                    {
                        ACComponent projectOfSource = invokableTaskExecutors.Where(c => c.ComponentClass.ACProjectID == startNextBatchAtProjectID1.Value).FirstOrDefault();
                        if (projectOfSource != null)
                            return new List<ACComponent>() { projectOfSource };
                    }
                    if (startNextBatchAtProjectID2.HasValue)
                    {
                        ACComponent projectOfTarget = invokableTaskExecutors.Where(c => c.ComponentClass.ACProjectID == startNextBatchAtProjectID2.Value).FirstOrDefault();
                        if (projectOfTarget != null)
                            return new List<ACComponent>() { projectOfTarget };
                    }
                }
                return invokableTaskExecutors;
            }
        }
        #endregion

        #endregion

        #region Methods

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWNodeProcessWorkflowVB(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeProcessWorkflow(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        #region Helper methods
        public override void Reset()
        {
            ClearMyConfiguration();
            // Iteration-Counter is increased to signal that node has been active
            // this is needed for the property AreOtherParallelNodesCompletable to count the completed nodes
            if (   this.IterationCount.ValueT <= 0
                && CurrentACState >= ACStateEnum.SMStarting)
            {
                this.IterationCount.ValueT++;
            }
            base.Reset();
        }

        protected virtual void LoadVBEntities()
        {
            var rootPW = RootPW;
            if (rootPW == null)
                return;

            //ParentRootWFNode


            using (DatabaseApp dbApp = new DatabaseApp())
            {
                var acProgramVB = rootPW.CurrentACProgram.FromAppContext<gip.mes.datamodel.ACProgram>(dbApp);

                if (ContentACClassWF != null && !_MDSchedulingGroupLoaded)
                {
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        _MDSchedulingGroupLoaded = true;
                        var queryMD = dbApp.MDSchedulingGroupWF.Where(c => c.VBiACClassWFID == ContentACClassWF.ACClassWFID).Select(c => c.MDSchedulingGroup);
                        (queryMD as ObjectQuery).MergeOption = MergeOption.NoTracking;
                        _MDSchedulingGroup = queryMD.FirstOrDefault();
                    }
                }


                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _ACProgramVB = acProgramVB;
                }
                if (acProgramVB != null)
                {
                    if (IsProduction)
                    {
                        gip.mes.datamodel.MaterialWFConnection materialWFConnection = null;
                        gip.mes.datamodel.ProdOrderPartslist currentProdOrderPartslist = null;
                        currentProdOrderPartslist = acProgramVB.ACProgramLog_ACProgram
                                                        .Where(f => f.OrderLog_VBiACProgramLog != null && f.OrderLog_VBiACProgramLog.ProdOrderPartslistPosID.HasValue)
                                                        .Select(f => f.OrderLog_VBiACProgramLog.ProdOrderPartslistPos.ProdOrderPartslist)
                                                        .FirstOrDefault();
                        if (currentProdOrderPartslist == null)
                        {
                            PWMethodProduction pwMethodProd = rootPW as PWMethodProduction;
                            if (pwMethodProd != null && pwMethodProd.CurrentProdOrderPartslistPos != null)
                                currentProdOrderPartslist = dbApp.ProdOrderPartslist.Where(c => c.ProdOrderPartslistID == pwMethodProd.CurrentProdOrderPartslistPos.ProdOrderPartslistID).FirstOrDefault();
                        }

                        using (ACMonitor.Lock(_20015_LockValue))
                        {
                            _CurrentProdOrderPartslist = currentProdOrderPartslist;
                        }

                        if (ContentACClassWF != null)
                        {
                            materialWFConnection = dbApp.MaterialWFConnection.Where(c => c.ACClassWFID == ContentACClassWF.ACClassWFID).FirstOrDefault();

                            using (ACMonitor.Lock(_20015_LockValue))
                            {
                                _MaterialWFConnection = materialWFConnection;
                            }

                            var contentACClassWFVB = ContentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(dbApp);
                            if (contentACClassWFVB != null)
                            {
                                var uncompletedBatchPlans = LoadUncompletedBatchPlans(_CurrentProdOrderPartslist, contentACClassWFVB);
                                ReCreateBatchPlanningTimes(uncompletedBatchPlans);
                            }
                        }

                        using (ACMonitor.Lock(_20015_LockValue))
                        {
                            if (_CurrentProdOrderPartslist != null)
                                dbApp.Detach(_CurrentProdOrderPartslist);

                            if (_MaterialWFConnection != null)
                                dbApp.Detach(_MaterialWFConnection);
                            if (_MaterialWFConnection == null)
                            {
                                _MaterialWFConnection = new datamodel.MaterialWFConnection();
                                _MaterialWFConnection.MaterialID = Guid.NewGuid();
                                _MaterialWFConnection.ACClassWFID = Guid.NewGuid();
                            }
                        }
                    }
                    //else if (IsTransport)
                    //{
                    //    if (CurrentPicking != null)
                    //    {
                    //        using (ACMonitor.Lock(_20015_LockValue))
                    //        {
                    //            _BatchPlanningTimes = new List<BatchPlanningTime>();
                    //            _BatchPlanningTimes.Add(new BatchPlanningTime());
                    //        }
                    //    }
                    //}
                    dbApp.Detach(acProgramVB);
                }
            }
        }

        protected virtual bool CheckIfNextBatchCanBeStarted()
        {
            if (this.TaskSubscriptionPoint.LocalStorage == null)
                return false;
            ACPointAsyncRMISubscrWrap<ACComponent>[] activeInvocations = null;
            // Kann nicht beednet werden falls noch Sub-Workflows aktiv sind

            using (ACMonitor.Lock(this.TaskSubscriptionPoint.LockLocalStorage_20033))
            {
                activeInvocations = this.TaskSubscriptionPoint.LocalStorage.Where(c => c.State <= PointProcessingState.Accepted).ToArray();
            }
            if (activeInvocations == null || !activeInvocations.Any())
                return true;

            // Überprüfe in Subworkflows ob alle Chargenwiederholungsschritte bereits aktiviert worden sind
            foreach (var group in activeInvocations.GroupBy(c => c.ValueT))
            {
                ChildInstanceInfoSearchParam searchParam = new ChildInstanceInfoSearchParam() { OnlyWorkflows = true, ACRequestIDs = activeInvocations.Select(c => c.RequestID).ToArray() };
                var childInstanceInfos = group.Key.GetChildInstanceInfo(1, searchParam);
                if (childInstanceInfos != null && childInstanceInfos.Any())
                {
                    var acClassTypeOfStartNextBatch = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(PWNodeStartNextBatch));
                    if (acClassTypeOfStartNextBatch != null)
                    {
                        foreach(var childInstanceInfo in childInstanceInfos)
                        {
                            PWBaseChildsCompResult completed = null;
                            if (group.Key.IsProxy)
                            {
                                ACComponent acComponentProxy = group.Key.StartComponent(childInstanceInfo) as ACComponent;
                                if (acComponentProxy != null)
                                {
                                    completed = (PWBaseChildsCompResult)acComponentProxy.ExecuteMethod("AreChildsOfTypeCompleted", new ACRef<gip.core.datamodel.ACClass>(acClassTypeOfStartNextBatch, gip.core.datamodel.Database.GlobalDatabase));
                                }
                            }
                            else
                            {
                                completed = (PWBaseChildsCompResult)group.Key.ACUrlCommand(childInstanceInfo.ACIdentifier + "!AreChildsOfTypeCompleted", new ACRef<gip.core.datamodel.ACClass>(acClassTypeOfStartNextBatch, gip.core.datamodel.Database.GlobalDatabase));
                            }
                            if (completed == null || !completed.AllCompleted)
                                return false;
                        }
                    }
                }
            }


            return true;
        }

        protected bool WillReadAndStartNextBatchCompleteNode()
        {
            if (IsTransport)
                return WillReadAndStartNextBatchCompleteNode_Picking(10);
            else if (IsProduction)
                return WillReadAndStartNextBatchCompleteNode_Prod();
            return false;
        }

        protected virtual bool CheckIfBatchIsStartedInSubWf(Guid prodOrderBatchID)
        {
            if (this.TaskSubscriptionPoint.LocalStorage == null)
                return false;
            ACPointAsyncRMISubscrWrap<ACComponent>[] activeInvocations = null;
            // Kann nicht beednet werden falls noch Sub-Workflows aktiv sind

            using (ACMonitor.Lock(this.TaskSubscriptionPoint.LockLocalStorage_20033))
            {
                activeInvocations = this.TaskSubscriptionPoint.LocalStorage.Where(c => c.State <= PointProcessingState.Accepted).ToArray();
            }
            if (activeInvocations == null || !activeInvocations.Any())
                return false;

            // Überprüfe in Subworkflows ob alle Chargenwiederholungsschritte bereits aktiviert worden sind
            foreach (var group in activeInvocations.GroupBy(c => c.ValueT))
            {
                ChildInstanceInfoSearchParam searchParam = new ChildInstanceInfoSearchParam() { OnlyWorkflows = true, ACRequestIDs = activeInvocations.Select(c => c.RequestID).ToArray() };
                var childInstanceInfos = group.Key.GetChildInstanceInfo(1, searchParam);
                if (childInstanceInfos != null && childInstanceInfos.Any())
                {
                    foreach (var childInstanceInfo in childInstanceInfos)
                    {
                        object result = null;
                        if (group.Key.IsProxy)
                        {
                            ACComponent acComponentProxy = group.Key.StartComponent(childInstanceInfo) as ACComponent;
                            if (acComponentProxy != null)
                            {
                                // Try-Catch for methods whoch are not Production nd don't have IsProdOrderBatchStarted-Method
                                try
                                {
                                    result = (PWBaseChildsCompResult)acComponentProxy.ExecuteMethod("IsProdOrderBatchStarted", prodOrderBatchID);
                                }
                                catch (Exception ec)
                                {
                                    Messages.LogException("PWNodeProcessWorkflowVB", "CheckIfBatchIsStartedInSubWf", ec);
                                }
                            }
                        }
                        else
                        {
                            // Try-Catch for methods whoch are not Production nd don't have IsProdOrderBatchStarted-Method
                            try
                            {
                                result = (PWBaseChildsCompResult)group.Key.ACUrlCommand(childInstanceInfo.ACIdentifier + "!IsProdOrderBatchStarted", prodOrderBatchID);
                            }
                            catch (Exception ec)
                            {
                                Messages.LogException("PWNodeProcessWorkflowVB", "CheckIfBatchIsStartedInSubWf(10)", ec);
                            }
                        }
                        if (result != null && (bool)result)
                            return true;
                    }
                }
            }

            return false;
        }

        protected override void ACState_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            base.ACState_PropertyChanged(sender, e, phase);

            if (phase == ACPropertyChangedPhase.BeforeBroadcast || ACState.InRestorePhase)
                return;
            ACPropertyValueEvent<ACStateEnum> valueEventT = e.ValueEvent as ACPropertyValueEvent<ACStateEnum>;
            if (valueEventT.Sender == EventRaiser.Target)
                return;
            InformSchedulerOnStateChange();
        }

        protected void InformSchedulerOnStateChange()
        {
            PABatchPlanScheduler scheduler = GetScheduler();
            if (scheduler != null)
                scheduler.OnACStateChangedOfPWNode(this);
        }

        protected PABatchPlanScheduler GetScheduler()
        {
            var appManager = this.ApplicationManager;
            if (appManager == null)
                return null;
            return appManager.FindChildComponents<PABatchPlanScheduler>(c => c is PABatchPlanScheduler, null, 1).FirstOrDefault();
        }
        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case ACStateConst.SMStopping:
                    SMStopping();
                    return true;
                case ACStateConst.TMStopp:
                    Stopp();
                    return true;
                case "AllowFollowingNode2Start":
                    AllowFollowingNode2Start();
                    return true;
                case "ProhibitFollowingNode2Start":
                    ProhibitFollowingNode2Start();
                    return true;
                case "SetIgnoreFIFO":
                    SetIgnoreFIFO();
                    return true;
                case "ResetIgnoreFIFO":
                    ResetIgnoreFIFO();
                    return true;
                case Const.IsEnabledPrefix + ACStateConst.TMStopp:
                    result = IsEnabledStopp();
                    return true;
                case Const.IsEnabledPrefix + "AllowFollowingNode2Start":
                    result = IsEnabledAllowFollowingNode2Start();
                    return true;
                case Const.IsEnabledPrefix + "ProhibitFollowingNode2Start":
                    result = IsEnabledProhibitFollowingNode2Start();
                    return true;
                case Const.IsEnabledPrefix + "SetIgnoreFIFO":
                    result = IsEnabledSetIgnoreFIFO();
                    return true;
                case Const.IsEnabledPrefix + "ResetIgnoreFIFO":
                    result = IsEnabledResetIgnoreFIFO();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
#endregion


#region ACState
        protected bool _PreventHandleStartNextBatchAgain = false;
        protected bool PreventHandleStartNextBatchAgain
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _PreventHandleStartNextBatchAgain;
                }
            }
            set
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _PreventHandleStartNextBatchAgain = value;
                }
            }
        }

        private DateTime? _PlanningWait = null;
        public bool IsInPlanningWaitNotElapsed
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _PlanningWait.HasValue && _PlanningWait.Value > DateTime.Now;
                }
            }
        }

        public bool IsInPlanningWait
        { 
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _PlanningWait.HasValue;
                }
            }
        }

        public void ResetPlanningWait()
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _PlanningWait = null;
            }
        }

        public override void SMIdle()
        {
            CurrentACSubState = ACSubStateEnum.SMIdle;
            base.SMIdle();
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            bool wasInPlanningWait = IsInPlanningWaitNotElapsed;
            HandleStartNextBatch();
            if (this.Root != null 
                && (   !this.Root.Initialized
                    || RootPW.IsStartingProcessFunction
                    || RootPW.CurrentACState <= ACStateEnum.SMStarting)
                && IsSubscribedToWorkCycle)
                return;
            if (   !wasInPlanningWait 
                && (BatchPlanningTimes == null || !BatchPlanningTimes.Any()) 
                && this.CurrentACState == ACStateEnum.SMStarting)
            {
                ParallelNodeStats stats = GetParallelNodeStats();
                if (stats.AreOtherParallelNodesCompletable)
                {
                    int countParallelNodes;
                    if (CompleteParallelNodes(out countParallelNodes))
                    {
                        if (!this.IsProduction && IsInPlanningWait)
                            return;
                        CurrentACSubState = ACSubStateEnum.SMIdle;
                        // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                        if (CurrentACState == ACStateEnum.SMStarting)
                            CurrentACState = ACStateEnum.SMCompleted;
                    }
                }
            }
        }

        public override void SMRunning()
        {
            if (PreventHandleStartNextBatchAgain)
            {
                PreventHandleStartNextBatchAgain = false;
                return;
            }

            HandleStartNextBatch();

            // HandleStartNextBatch has completed this node (not in stopping or starting) because there are no other nodes that are active
            if (CurrentACState == ACStateEnum.SMCompleted || CurrentACState == ACStateEnum.SMIdle)
            {
                ParallelNodeStats stats = GetParallelNodeStats();
                if (stats.AreOtherParallelNodesCompletable)
                {
                    int countParallelNodes;
                    CompleteParallelNodes(out countParallelNodes);
                }
            }
        }


        [ACMethodState("en{'Stopping'}de{'Stoppend'}", 90, true)]
        public virtual void SMStopping()
        {
            if (PreventHandleStartNextBatchAgain)
            {
                PreventHandleStartNextBatchAgain = false;
                return;
            }

            if (IsInPlanningWaitNotElapsed && IsSubscribedToWorkCycle)
                return;

            if ((BatchPlanningTimes != null && BatchPlanningTimes.Any()) || IsTransport)
                HandleStartNextBatch();
            else if (!HasActiveSubworkflows)
            {
                ResetPlanningWait();
                UnSubscribeToProjectWorkCycle();

                bool resetToSMStarting = false;
                ParallelNodeStats stats = GetParallelNodeStats();
                if (stats.ActiveParallelNodesCount > 0)
                    resetToSMStarting = true;
                int countParallelNodes = 0;
                if (CompleteParallelNodes(out countParallelNodes))
                {
                    CurrentACSubState = ACSubStateEnum.SMIdle;
                    // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                    if (CurrentACState == ACStateEnum.SMStopping)
                        CurrentACState = resetToSMStarting && !CompleteIfNotPlan ? ACStateEnum.SMStarting : ACStateEnum.SMCompleted;
                }
                else if (resetToSMStarting)
                {
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        _PlanningWait = DateTime.Now.AddSeconds(30);
                    }
                    SubscribeToProjectWorkCycle();
                    CurrentACSubState = ACSubStateEnum.SMIdle;
                    CurrentACState = CompleteIfNotPlan ? ACStateEnum.SMCompleted : ACStateEnum.SMStarting;
                    OnSubworkflowsCompleted();
                }
            }
            else if (IsSubscribedToWorkCycle)
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_PlanningWait.HasValue)
                    {
                        ResetPlanningWait();
                        UnSubscribeToProjectWorkCycle();
                    }
                    else
                        _PlanningWait = DateTime.Now.AddSeconds(30);
                }
            }
            else
            {
                ResetPlanningWait();
            }
        }

        public struct ParallelNodeStats
        {
            public int CountParallelNodes;
            public int StartingParallelNodes;
            public int CompletedParallelNodes;
            public int NodesWithBatchPlanningTimes;
            public int UnstartedParallelNodes;
            public int IdleParallelNodesCount { get { return UnstartedParallelNodes + CompletedParallelNodes; } }
            public int WaitingParallelNodesCount { get { return StartingParallelNodes - NodesWithBatchPlanningTimes; } }
            public int ActiveParallelNodesCount { get { return CountParallelNodes - StartingParallelNodes - IdleParallelNodesCount; } }
            public bool AreOtherParallelNodesCompletable 
            { 
                get 
                {
                    return (CountParallelNodes == StartingParallelNodes + CompletedParallelNodes)
                            && (NodesWithBatchPlanningTimes <= 0);
                    //return     (CountParallelNodes == StartingParallelNodes + IdleParallelNodesCount) 
                    //        && (NodesWithBatchPlanningTimes <= 0); 
                }
            }
        }

        private ParallelNodeStats GetParallelNodeStats()
        {
            ParallelNodeStats stats = new ParallelNodeStats();
            stats.CountParallelNodes = 0;
            stats.StartingParallelNodes = 0;
            stats.CompletedParallelNodes = 0;
            stats.NodesWithBatchPlanningTimes = 0;
            stats.UnstartedParallelNodes = 0;
            //if (this.ContentACClassWF == null || !this.ContentACClassWF.RefPAACClassMethodID.HasValue)
            //    return false;
            // Are there any parallel nodes which starts the same Sub-Method for parallel Production
            var parallelNodes = AllParallelNodes; // ParallelNodes;
            if (parallelNodes == null || !parallelNodes.Any())
                return stats;
            stats.CountParallelNodes = parallelNodes.Count;
            var startingNodes = parallelNodes.Where(c => c.CurrentACState == ACStateEnum.SMStarting);
            if (startingNodes != null)
                stats.StartingParallelNodes = startingNodes.Count();
            var completedNodes = parallelNodes.Where(c => c.CurrentACState == ACStateEnum.SMIdle && c.IterationCount.ValueT > 0);
            if (completedNodes != null)
                stats.CompletedParallelNodes = completedNodes.Count();
            var idleNodes = parallelNodes.Where(c => c.CurrentACState == ACStateEnum.SMIdle && c.IterationCount.ValueT <= 0);
            if (idleNodes != null)
                stats.UnstartedParallelNodes = idleNodes.Count();
            var withBatchPlanningTimes = parallelNodes.Where(c =>   (    (c.IsProduction && c.BatchPlanningTimes != null && c.BatchPlanningTimes.Any())
                                                                      || (!c.IsProduction && c.IsInPlanningWait))
                                                                  && c.CurrentACState != ACStateEnum.SMIdle);
            if (withBatchPlanningTimes != null)
                stats.NodesWithBatchPlanningTimes = withBatchPlanningTimes.Count();
            return stats;
        }

        private bool CompleteParallelNodes(out int countParallelNodes)
        {
            countParallelNodes = 0;
            //if (this.ContentACClassWF == null || !this.ContentACClassWF.RefPAACClassMethodID.HasValue)
            //    return true;
            // Are there any parallel nodes which starts the same Sub-Method for parallel Production
            var parallelNodes = AllParallelNodes; //ParallelNodes;
            if (parallelNodes == null || !parallelNodes.Any())
                return true;
            countParallelNodes = parallelNodes.Count;

            var completedNodes = parallelNodes.Where(c =>      (     c.CurrentACState == ACStateEnum.SMStarting 
                                                                 && (   (c.IsProduction && (c.BatchPlanningTimes == null || !c.BatchPlanningTimes.Any()))
                                                                     || (!c.IsProduction && !c.IsInPlanningWait)))
                                                            || (c.CurrentACState == ACStateEnum.SMStopping && !c.HasActiveSubworkflows) 
                                                            || c.CurrentACState == ACStateEnum.SMIdle);

            // Are all completed?
            if (completedNodes != null && completedNodes.Count() == countParallelNodes)
            {
                foreach (var nodeToReset in completedNodes)
                {
                    if (nodeToReset.CurrentACState != ACStateEnum.SMIdle)
                    {
                        nodeToReset.UnSubscribeToProjectWorkCycle();
                        nodeToReset.CurrentACSubState = ACSubStateEnum.SMIdle;
                        nodeToReset.CurrentACState = ACStateEnum.SMCompleted;
                    }
                }
                return true;
            }
            return false;
        }

        public List<PWNodeProcessWorkflowVB> ParallelNodes
        {
            get
            {
                // Are there any parallel nodes which starts the same Sub-Method for parallel Production
                return ParentRootWFNode.FindChildComponents<PWNodeProcessWorkflowVB>
                            (c => c != this
                            && c is PWNodeProcessWorkflowVB
                            && (c as PWNodeProcessWorkflowVB).ContentACClassWF != null
                            && (c as PWNodeProcessWorkflowVB).ContentACClassWF.RefPAACClassMethodID.HasValue
                            && (c as PWNodeProcessWorkflowVB).ContentACClassWF.RefPAACClassMethodID.Value == this.ContentACClassWF.RefPAACClassMethodID.Value);

            }
        }

        public List<PWNodeProcessWorkflowVB> AllParallelNodes
        {
            get
            {
                return ParentRootWFNode.FindChildComponents<PWNodeProcessWorkflowVB>
                            (c => c != this
                            && c is PWNodeProcessWorkflowVB
                            && (c as PWNodeProcessWorkflowVB).ContentACClassWF != null);

            }
        }

        protected virtual void HandleStartNextBatch()
        {
            if (CurrentACState == ACStateEnum.SMPaused)
                return;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _NewChildPosForBatch_ProdOrderPartslistPosID = null;
                IsLastBatch2Start = (Int16)PADosingLastBatchEnum.None;
                _NewChildPosForBatch_PickingPosID = null;
                _StartNextBatchAtProjectID1 = null;
                _StartNextBatchAtProjectID2 = null;
            }

            if (   !Root.Initialized
                || (    RootPW != null 
                    && (   RootPW.IsStartingProcessFunction 
                        || RootPW.CurrentACState <= ACStateEnum.SMStarting)))
            {
                SubscribeToProjectWorkCycle();
                return;
            }

            if (IsInPlanningWaitNotElapsed)
                return;

            ResetPlanningWait();

            if (!IgnoreFIFO && !WillReadAndStartNextBatchCompleteNode())
            {
                gip.core.datamodel.ACClass refPAAClass = RefACClassOfContentWF;

                if (refPAAClass == null)
                {
                    //Error50169: ContentACClassWF.RefPAACClass is null.
                    Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "HandleStartNextBatch(0)", 1000, "Error50169");
                    
                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), "HandleStartNextBatch(0)", msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);

                    return;
                }

                //perfEvent = vbDump != null ? vbDump.PerfLoggerStart(loggerInstance, 100) : null;

                bool canStartIfPredecessorIsStopping = this.CanStartIfPredecessorIsStopping;
                bool compareOnlySameACIdentifers = CompareOnlySameACIdentifers;
                PWNodeProcessWorkflowVB[] otherPlanningNodes = null;
                /// Überprüfe ob es Planungsschritte in anderen Planungsworkflows gibt, die eine höhere Priorität haben
                // Finde alle Planungsschritte die nicht meinem Auftrag angehören 
                var query = this.ApplicationManager.ACCompTypeDict.GetComponentsOfType<PWNodeProcessWorkflowVB>(true);
                if (query != null && query.Any())
                {
                    query = query.Where(c => c.InitState == ACInitState.Initialized);
                    Func<PWNodeProcessWorkflowVB, bool> safeAccessToAppDefinition = (c) => {
                        return RefACClassOfContentWF?.ACProjectID == refPAAClass.ACProjectID;
                        //using (ACMonitor.Lock(ContextLockForACClassWF))
                        //{
                        //    return     c.ContentACClassWF.RefPAACClass != null
                        //            && c.ContentACClassWF.RefPAACClass.ACProjectID == refPAAClass.ACProjectID;
                        //}
                    };

                    // Search for other Planning-Nodes which can be considered for priorization
                    otherPlanningNodes = query.Where(c => c != this
                                // 1. Compare RootPW: If there are more Planning-Nodes inside this Planning-Worflow, than ignore them because the mustn't be compared, because they starts other subworkflows:
                                && c.RootPW != this.RootPW
                                // 2. Compares Applicationdefinition-Project: If subworkflow that should be started is from another application-definition than this one, than priorisation-check makes no sense:
                                && safeAccessToAppDefinition(c)
                                // 3. Wether SchedulingGroup ist assigned to node, then priorization is done via SchedulingGroup
                                //    else Compares if the planning node produces another Intermediate-product. If yes, than the subworkflow needs other resources to start. 
                                //    Therefore the planning node can also be ignored for prirorization
                                && (  (    this.MDSchedulingGroup != null 
                                        && c.MDSchedulingGroup != null 
                                        && this.MDSchedulingGroup.MDSchedulingGroupID == c.MDSchedulingGroup.MDSchedulingGroupID)
                                   || (    this.MDSchedulingGroup == null 
                                        && this.MaterialWFConnection != null
                                        && c.MaterialWFConnection != null 
                                        && this.MaterialWFConnection.MaterialID == c.MaterialWFConnection.MaterialID)
                                   || (    this.CurrentPicking != null 
                                        && this.CurrentPicking.ACClassMethodID.HasValue
                                        && c.CurrentPicking != null
                                        && c.CurrentPicking.ACClassMethodID.HasValue
                                        && c.CurrentPicking.ACClassMethodID == c.CurrentPicking.ACClassMethodID) )
                                ).ToArray();

                }

                // Priorization-Check only if there other other nodes. Else subworkflow can be started.
                if (otherPlanningNodes != null && otherPlanningNodes.Any())
                {
                    var rootPW = this.RootPW;

                    // Determine the Application-Project, where Subworkflow must be started depending on the Routing-Rules (Which Production-Line) 
                    var thisAppProject4Starting = this.FirstInvokableTaskExecutor;
                    // If this Reference is null, then ConfigStores are not loaded completely => wait!
                    if (thisAppProject4Starting == null)
                    {
                        SubscribeToProjectWorkCycle();
                        return;
                    }

                    // Make a priorized List that's sorted by PriorityTime
                    // Consider only the not started nodes (IterationCount.ValueT <= 0) and which are active (This means: Ignore Planing nodes that are completed (IterationCount.ValueT >= 1)
                    // and active planning nodes
                    var sortedPlanningNodes = otherPlanningNodes
                        .Where(c =>     // 1. Consider only which should run on the same application-project
                                        c.FirstInvokableTaskExecutor == thisAppProject4Starting
                                    // 2. Consider only those that was not started
                                    && (c.IterationCount.ValueT <= 0
                                        // 3. And those that are currently running and have started already one subworkflow
                                        || (c.CurrentACState != ACStateEnum.SMIdle && c.CurrentACState != ACStateEnum.SMBreakPoint && c.CurrentACState != ACStateEnum.SMBreakPointStart))
                        )
                        .OrderBy(c => c.PriorityTime)
                        .ToArray();
                    foreach (var otherPriorPlanningNode in sortedPlanningNodes)
                    {
                        // Dieser Knoten darf nur dann gestartet werden, wenn es keinen früher gestarteten Knoten gibt der nocht nicht alle seiner Batche gestartet hat
                        if (otherPriorPlanningNode != null
                            && otherPriorPlanningNode.PriorityTime < this.PriorityTime
                            && !otherPriorPlanningNode.CurrentACSubState.HasFlag(ACSubStateEnum.SMAllowFollowingNode2Start))
                        {
                            // Option "compareOnlySameACIdentifers": If other node has another ACIdentifier, than ignore it for priorization
                            if (compareOnlySameACIdentifers && otherPriorPlanningNode.ACIdentifier != this.ACIdentifier)
                                continue;

                            // Option "canStartIfPredecessorIsStopping": If other node is in Stopping-State, than this node can start.
                            if (   canStartIfPredecessorIsStopping 
                                && (   (otherPriorPlanningNode.CurrentACState >= ACStateEnum.SMCompleted && otherPriorPlanningNode.CurrentACState <= ACStateEnum.SMResetting) // Node has already started it's batches an waits till last subworkflow has completed
                                    || (otherPriorPlanningNode.CurrentACState == ACStateEnum.SMStarting && otherPriorPlanningNode.IterationCount.ValueT > 0)) // Node has already started it's batches an waits for a new batchplan
                               )
                               continue;

                            // Option "SkipWaitingNodes":  If other node waits, because no batchplan was defined for it, than this node can be priorized
                            if ( (   (otherPriorPlanningNode.CurrentACState == ACStateEnum.SMStarting && otherPriorPlanningNode.IsInPlanningWait)
                                   || otherPriorPlanningNode.CurrentACState == ACStateEnum.SMPaused)
                                && SkipWaitingNodes)
                                continue;

                            using (ACMonitor.Lock(_20015_LockValue))
                            {
                                _PlanningWait = DateTime.Now.AddSeconds(30);
                            }
                            SubscribeToProjectWorkCycle();
                            // TODO: Message for user
                            return;
                        }
                    }
                }
            }

            ProdOrderBatchPlan batchPlanToStart = null;
            ProdOrderBatch nextBatch = null;
            ProdOrderPartslistPos newChildPosForBatch = null;
            PickingPos nextPickingPos = null;

            StartNextBatchResult result = StartNextBatchResult.Done;
            if (IsProduction)
            {
                bool isLastBatch = false;
                Guid? startNextBatchAtProjectID1;
                result = ReadAndStartNextBatch(out batchPlanToStart, out nextBatch, out newChildPosForBatch, out startNextBatchAtProjectID1, out isLastBatch);
                StartNextBatchAtProjectID1 = startNextBatchAtProjectID1;
                if (newChildPosForBatch != null)
                {
                    NewChildPosForBatch_ProdOrderPartslistPosID = newChildPosForBatch.ProdOrderPartslistPosID;
                    IsLastBatch2Start = isLastBatch ? (Int16)PADosingLastBatchEnum.LastBatch : (Int16)PADosingLastBatchEnum.None;
                }
            }
            else if (IsTransport)
            {
                Picking picking = CurrentPicking;
                bool isLastBatch = false;
                if (picking != null)
                {
                    Guid? startNextBatchAtProjectID1;
                    Guid? startNextBatchAtProjectID2;

                    result = ReadAndStartNextBatch(picking, out nextPickingPos, out startNextBatchAtProjectID1, out startNextBatchAtProjectID2, out isLastBatch);
                    StartNextBatchAtProjectID1 = startNextBatchAtProjectID1;
                    StartNextBatchAtProjectID2 = startNextBatchAtProjectID2;
                    if (nextPickingPos != null)
                    {
                        NewChildPosForBatch_PickingPosID = nextPickingPos.PickingPosID;
                        IsLastBatch2Start = isLastBatch ? (Int16)PADosingLastBatchEnum.LastBatch : (Int16)PADosingLastBatchEnum.None;
                    }
                }
            }

            if (result == StartNextBatchResult.CycleWait)
            {
                CurrentACSubState = ACSubStateEnum.SMIdle;

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _PlanningWait = DateTime.Now.AddSeconds(30);
                }
                SubscribeToProjectWorkCycle();
                return;
            }
            UnSubscribeToProjectWorkCycle();

            if (result == StartNextBatchResult.StartNextBatch)
            {
                CurrentACSubState = ACSubStateEnum.SMIdle;
                if (AddNewTaskToApplication())
                {
                    this.IterationCount.ValueT++;
                    PreventHandleStartNextBatchAgain = true;
                    if (CurrentACState == ACStateEnum.SMStarting)
                        CurrentACState = ACStateEnum.SMRunning;
                    else
                        InformSchedulerOnStateChange();

                    if (   (StartNextStage == StartNextStageMode.StartImmediately && this.IterationCount.ValueT == 1)
                        || (StartNextStage == StartNextStageMode.StartOnStartSecondBatch && this.IterationCount.ValueT == 2)
                        || (StartNextStage == StartNextStageMode.StartOnFirstBatchCompleted && _LastCallbackResult != null && this.IterationCount.ValueT == 2)
                        )
                    {
                        StartPOListOfNextStage(nextBatch, newChildPosForBatch);
                    }
                    return;
                }
                else
                {
                    // Try to restart same batch again, if invocation of subworkflow failed:
                    if (IsProduction 
                        && batchPlanToStart != null 
                        && nextBatch != null
                        && batchPlanToStart.PlanState == GlobalApp.BatchPlanState.Completed)
                    {
                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            ProdOrderBatchPlan batchPlanToStart2 = batchPlanToStart.FromAppContext<ProdOrderBatchPlan>(dbApp);
                            if (batchPlanToStart2 != null)
                            {
                                batchPlanToStart2.PlanState = GlobalApp.BatchPlanState.AutoStart;
                                dbApp.ACSaveChanges();
                            }
                        }
                    }

                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        _PlanningWait = DateTime.Now.AddSeconds(10);
                    }
                    SubscribeToProjectWorkCycle();
                    InformSchedulerOnStateChange();
                    return;
                }
            }
            else if (result == StartNextBatchResult.Done)
            {
                // _LastCallbackResult
                if (   (   StartNextStage == StartNextStageMode.StartOnLastBatchCompleted
                        && !HasActiveSubworkflows 
                        && (BatchPlanningTimes == null || !BatchPlanningTimes.Any()))
                    || (    StartNextStage == StartNextStageMode.StartOnFirstBatchCompleted
                        && _LastCallbackResult != null)
                    )
                {
                    StartPOListOfNextStage(null, null);
                }

                if (HasActiveSubworkflows && (BatchPlanningTimes == null || !BatchPlanningTimes.Any()))
                {
                    if (IsEnabledStopp())
                    {
                        PreventHandleStartNextBatchAgain = true;
                        Stopp();
                    }
                    return;
                }
                else if (BatchPlanningTimes == null || !BatchPlanningTimes.Any())
                {
                    ParallelNodeStats stats = GetParallelNodeStats();
                    CurrentACSubState = ACSubStateEnum.SMIdle;
                    if (!stats.AreOtherParallelNodesCompletable && IsProduction)
                    {
                        using (ACMonitor.Lock(_20015_LockValue))
                        {
                            _PlanningWait = DateTime.Now.AddSeconds(30);
                        }
                        if (CurrentACState == ACStateEnum.SMRunning)
                        {
                            OnSubworkflowsCompleted();
                            CurrentACState = CompleteIfNotPlan ? ACStateEnum.SMCompleted : ACStateEnum.SMStarting;
                        }
                        else if (CurrentACState == ACStateEnum.SMStarting && CompleteIfNotPlan)
                            CurrentACState = ACStateEnum.SMCompleted;
                        SubscribeToProjectWorkCycle();
                        return;
                    }
                    else
                    {
                        if (CurrentACState != ACStateEnum.SMStarting)
                            OnSubworkflowsCompleted();
                        CurrentACState = ACStateEnum.SMCompleted;
                    }
                }
                else
                {
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        _PlanningWait = DateTime.Now.AddSeconds(30);
                    }
                    SubscribeToProjectWorkCycle();
                }
            }
            else if (result == StartNextBatchResult.WaitForNextEvent)
            {
            }
        }


        public override void UnSubscribeToProjectWorkCycle()
        {
            ResetPlanningWait();
            base.UnSubscribeToProjectWorkCycle();
        }


        public override void SMCompleted()
        {
            base.SMCompleted();
        }

        protected virtual void OnSubworkflowsCompleted()
        {
            if (IsProduction)
            {
                SchedulingForecastManager schedulingManager = SchedulingForecastManager.GetServiceInstance(this);
                if (schedulingManager != null && CurrentProdOrderBatchPlanID != null)
                    schedulingManager.UpdateBatchPlanDuration(null, CurrentProdOrderBatchPlanID.Value, Root.CurrentInvokingUser.Initials);
            }
        }

        /// <summary>
        /// Fills Parameterlist in ACMethod with values from Config-Store-Hierarchy
        /// Derivations of PWClasses can manipulate the Paramterlist as well according to their individual logic
        /// </summary>
        /// <param name="paramMethod">ACMethod to fill</param>
        /// <param name="isForPAF">If its a acMethod which will be passed to the Start-Method of a PAPocessFunction else it's the local configuration for this PWNode</param>
        public override bool GetConfigForACMethod(ACMethod paramMethod, bool isForPAF, params Object[] customParams)
        {
            if (!base.GetConfigForACMethod(paramMethod, isForPAF, customParams))
                return false;
            if (isForPAF)
            {
                if (paramMethod.ParameterValueList != null)
                {
                    ACValue acValue = null;
                    if (NewChildPosForBatch_ProdOrderPartslistPosID.HasValue)
                    {
                        acValue = paramMethod.ParameterValueList.GetACValue(ProdOrderPartslistPos.ClassName);
                        if (acValue == null)
                        {
                            acValue = new ACValue() { ACIdentifier = ProdOrderPartslistPos.ClassName, Value = NewChildPosForBatch_ProdOrderPartslistPosID.Value, ValueTypeACClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(Guid)), Option = Global.ParamOption.Required };
                            paramMethod.ParameterValueList.Add(acValue);
                        }
                        else
                        {
                            acValue.Value = NewChildPosForBatch_ProdOrderPartslistPosID.Value;
                        }
                    }
                    if (NewChildPosForBatch_PickingPosID.HasValue)
                    {
                        acValue = paramMethod.ParameterValueList.GetACValue(Picking.ClassName);
                        Picking picking = CurrentPicking;
                        if (picking != null)
                        {
                            if (acValue == null)
                            {
                                acValue = new ACValue() { ACIdentifier = Picking.ClassName, Value = picking.PickingID, ValueTypeACClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(Guid)), Option = Global.ParamOption.Required };
                                paramMethod.ParameterValueList.Add(acValue);
                            }
                            else
                            {
                                acValue.Value = picking.PickingID;
                            }
                        }

                        acValue = paramMethod.ParameterValueList.GetACValue(PickingPos.ClassName);
                        if (acValue == null)
                        {
                            acValue = new ACValue() { ACIdentifier = PickingPos.ClassName, Value = NewChildPosForBatch_PickingPosID.Value, ValueTypeACClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(Guid)), Option = Global.ParamOption.Required };
                            paramMethod.ParameterValueList.Add(acValue);
                        }
                        else
                        {
                            acValue.Value = NewChildPosForBatch_PickingPosID.Value;
                        }
                    }

                    ACValue acValueLastBatch = paramMethod.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                    if (acValueLastBatch == null)
                    {
                        acValueLastBatch = new ACValue() { ACIdentifier = PWMethodVBBase.IsLastBatchParamName, Value = this.IsLastBatch2Start, ValueTypeACClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(Int16)), Option = Global.ParamOption.Optional };
                        paramMethod.ParameterValueList.Add(acValueLastBatch);
                    }
                    else
                    {
                        acValueLastBatch.Value = this.IsLastBatch2Start;
                    }

                }
            }
            return true;
        }


        public override PAOrderInfo GetPAOrderInfo()
        {
            if (CurrentProdOrderPartslist != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Add(ProdOrderPartslist.ClassName, CurrentProdOrderPartslist.ProdOrderPartslistID);
                return info;
            }
            return base.GetPAOrderInfo();
        }


        protected void ResetPlanningWaitOnFollowingPlanningNode()
        {
            gip.core.datamodel.ACClass refPAAClass = RefACClassOfContentWF;
            if (refPAAClass == null)
                return;

            PWNodeProcessWorkflowVB[] otherPlanningNodes = null;
            var query = this.ApplicationManager.ACCompTypeDict.GetComponentsOfType<PWNodeProcessWorkflowVB>(true);
            if (query == null || !query.Any())
                return;
            query = query.Where(c => c.InitState == ACInitState.Initialized);
            Func<PWNodeProcessWorkflowVB, bool> safeAccessToAppDefinition = (c) =>
            {
                return RefACClassOfContentWF?.ACProjectID == refPAAClass.ACProjectID;
            };

            // Search for other Planning-Nodes which can be considered for priorization
            otherPlanningNodes = query.Where(c => c != this
                        // 1. Compare RootPW: If there are more Planning-Nodes inside this Planning-Worflow, than ignore them because the mustn't be compared, because they starts other subworkflows:
                        && c.RootPW != this.RootPW
                        // 2. Compares Applicationdefinition-Project: If subworkflow that should be started is from another application-definition than this one, than priorisation-check makes no sense:
                        && safeAccessToAppDefinition(c)
                        // 3. Wether SchedulingGroup ist assigned to node, then priorization is done via SchedulingGroup
                        //    else Compares if the planning node produces another Intermediate-product. If yes, than the subworkflow needs other resources to start. 
                        //    Therefore the planning node can also be ignored for prirorization
                        && ((this.MDSchedulingGroup != null
                                && c.MDSchedulingGroup != null
                                && this.MDSchedulingGroup.MDSchedulingGroupID == c.MDSchedulingGroup.MDSchedulingGroupID)
                            || (this.MDSchedulingGroup == null
                                && this.MaterialWFConnection != null
                                && c.MaterialWFConnection != null
                                && this.MaterialWFConnection.MaterialID == c.MaterialWFConnection.MaterialID)
                            || (this.CurrentPicking != null
                                && this.CurrentPicking.ACClassMethodID.HasValue
                                && c.CurrentPicking != null
                                && c.CurrentPicking.ACClassMethodID.HasValue
                                && c.CurrentPicking.ACClassMethodID == c.CurrentPicking.ACClassMethodID))
                        ).ToArray();

            if (otherPlanningNodes == null || !otherPlanningNodes.Any())
                return;
            var rootPW = this.RootPW;

            var thisAppProject4Starting = this.FirstInvokableTaskExecutor;
            if (thisAppProject4Starting == null)
                return;

            bool canStartIfPredecessorIsStopping = this.CanStartIfPredecessorIsStopping;
            bool compareOnlySameACIdentifers = CompareOnlySameACIdentifers;
            var sortedPlanningNodes = otherPlanningNodes
                .Where(c =>     // 1. Consider only which should run on the same application-project
                                c.FirstInvokableTaskExecutor == thisAppProject4Starting
                            // 2. Consider only those that was not started
                            && (c.IterationCount.ValueT <= 0
                                // 3. And those that are currently running and have started already one subworkflow
                                || (c.CurrentACState != ACStateEnum.SMIdle && c.CurrentACState != ACStateEnum.SMBreakPoint && c.CurrentACState != ACStateEnum.SMBreakPointStart)
                            && (!compareOnlySameACIdentifers || c.ACIdentifier == this.ACIdentifier)
                            )
                )
                .OrderBy(c => c.PriorityTime)
                .ToArray();
        }
        #endregion

        #region Interaction
            /// <summary>
            /// Method is named "Stopp" (StopProcess) because Stop()-MEthod already exists in ACComponent
            /// </summary>
            [ACMethodInteraction("Process", "en{'Stop'}de{'Stoppen'}", (short)MISort.Stop, true, "", Global.ACKinds.MSMethod, false, Global.ContextMenuCategory.ProcessCommands)]
        public virtual void Stopp()
        {
            if (!IsEnabledStopp())
                return;
            CurrentACState = ACStateEnum.SMStopping;
        }

        public virtual bool IsEnabledStopp()
        {
            return CurrentACState == ACStateEnum.SMRunning;
        }

        public override void Resume()
        {
            if (!IsEnabledResume())
                return;
            ClearMyConfiguration();
            // Ein stoppender Batch kann nur fortgesetzt werden, falls auch neue Batchplaneinträge eingetragen worden sind
            if (CurrentACState == ACStateEnum.SMStopping)
            {
                ReloadBatchPlans();
                if (BatchPlanningTimes.Any())
                {
                    // Set PreventHandleStartNextBatchAgain, to prevent that new Subworkflow is not started from ClientChannel
                    // It must be started from cyclic Thread to prevent, that two threads are creating a subworkflow at the same time
                    PreventHandleStartNextBatchAgain = true;
                    base.Resume();
                    SubscribeToProjectWorkCycle();
                }
            }
            else
            {
                // Set PreventHandleStartNextBatchAgain, to prevent that new Subworkflow is not started from ClientChannel
                // It must be started from cyclic Thread to prevent, that two threads are creating a subworkflow at the same time
                PreventHandleStartNextBatchAgain = true;
                base.Resume();
                SubscribeToProjectWorkCycle();
            }
        }

        public override bool IsEnabledResume()
        {
            return ((CurrentACState == ACStateEnum.SMPaused || CurrentACState == ACStateEnum.SMStopping) 
                    && (CurrentProdOrderPartslist != null || CurrentPicking != null));
        }

        [ACMethodInteraction("Process", "en{'Allow following order to start'}de{'Erteile nachfolgendem Auftrag die Starterlaubnis'}", (short)297, true)]
        public virtual void AllowFollowingNode2Start()
        {
            if (!IsEnabledAllowFollowingNode2Start())
                return;
            CurrentACSubState = ACSubStateEnum.SMAllowFollowingNode2Start;
        }

        public virtual bool IsEnabledAllowFollowingNode2Start()
        {
            return !CurrentACSubState.HasFlag(ACSubStateEnum.SMAllowFollowingNode2Start);
        }

        [ACMethodInteraction("Process", "en{'Prohibit following order to start'}de{'Entziehe nachfolgendem Auftrag die Starterlaubnis'}", (short)298, true)]
        public virtual void ProhibitFollowingNode2Start()
        {
            if (!IsEnabledProhibitFollowingNode2Start())
                return;
            CurrentACSubState = ACSubStateEnum.SMIdle;
        }

        public virtual bool IsEnabledProhibitFollowingNode2Start()
        {
            return CurrentACSubState.HasFlag(ACSubStateEnum.SMAllowFollowingNode2Start);
        }

        [ACMethodInteraction("Process", "en{'Ignore Priorization'}de{'Priorisierung ignorieren'}", (short)299, true)]
        public virtual void SetIgnoreFIFO()
        {
            IgnoreFIFO = true;
        }

        public virtual bool IsEnabledSetIgnoreFIFO()
        {
            return !IgnoreFIFO;
        }


        [ACMethodInteraction("Process", "en{'Activate priorizing'}de{'Priorisierung aktivieren'}", (short)299, true)]
        public virtual void ResetIgnoreFIFO()
        {
            IgnoreFIFO = false;
        }

        public virtual bool IsEnabledResetIgnoreFIFO()
        {
            return IgnoreFIFO;
        }
        
        #endregion

        #region Callback
        public override void TaskCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            _InCallback = true;
            try
            {
                if (e != null)
                {
                    IACTask taskEntry = wrapObject as IACTask;

                    ACMethodEventArgs eM = e as ACMethodEventArgs;
                    _CurrentMethodEventArgs = eM;
                    if (taskEntry.State == PointProcessingState.Deleted /*&& taskEntry.InProcess*/)
                    {
                        _LastCallbackResult = e;
                        if (CurrentACState == ACStateEnum.SMRunning || CurrentACState == ACStateEnum.SMStopping)
                            SubscribeToProjectWorkCycle();
                        //CurrentACState = PABaseState.SMCompleted;
                        // TODO: Log in ACProgram
                    }
                    else if (taskEntry.State == PointProcessingState.Accepted)
                    {
                        if (eM != null)
                        {
                            ACValue acValue = eM.GetACValue(typeof(PWNodeStartNextBatch).Name);
                            if (acValue != null)
                            {
                                if (CurrentACState == ACStateEnum.SMRunning || CurrentACState == ACStateEnum.SMStopping)
                                    SubscribeToProjectWorkCycle();
                            }
                        }
                    }
                    // Starting of a new Sub-Workflow failed
                    else if (taskEntry.State == PointProcessingState.Rejected)
                    {
                        //if (eM != null && eM.ResultState == Global.ACMethodResultState.Failed)
                        //{
                        //}
                    }

                    //ACPointAsyncRMIWrap<ACComponent> rmiWrap = wrapObject as ACPointAsyncRMIWrap<ACComponent>;
                    //rmiWrap.ACMethod;
                }
            }
            finally
            {
                _InCallback = false;
            }
        }
#endregion

#region Planning and Testing
        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);

            XmlElement xmlChild = xmlACPropertyList["MethodPriorisation"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("MethodPriorisation");
                if (xmlChild != null)
                    xmlChild.InnerText = CanStartIfPredecessorIsStopping.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["IgnoreFIFO"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("IgnoreFIFO");
                if (xmlChild != null)
                    xmlChild.InnerText = IgnoreFIFO.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["StartNextStage"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("StartNextStage");
                if (xmlChild != null)
                    xmlChild.InnerText = StartNextStage.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["EndProdOrderPartslistMode"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("EndProdOrderPartslistMode");
                if (xmlChild != null)
                    xmlChild.InnerText = EndProdOrderPartslistMode.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }
            
            xmlChild = xmlACPropertyList["CurrentProdOrderPartslist"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("CurrentProdOrderPartslist");
                if (xmlChild != null)
                    xmlChild.InnerText = _CurrentProdOrderPartslist != null ? _CurrentProdOrderPartslist.ProdOrderPartslistID.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["ACProgramVB"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("ACProgramVB");
                if (xmlChild != null)
                    xmlChild.InnerText = _ACProgramVB != null ? _ACProgramVB.ACProgramID.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["IsLastBatch2Start"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("IsLastBatch2Start");
                if (xmlChild != null)
                    xmlChild.InnerText = IsLastBatch2Start.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["PriorityTime"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("PriorityTime");
                if (xmlChild != null)
                    xmlChild.InnerText = PriorityTime.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["BatchPlanningTimes"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("BatchPlanningTimes");
                if (xmlChild != null)
                {
                    var batchPlanningTimes = BatchPlanningTimes;
                    if (batchPlanningTimes == null || !batchPlanningTimes.Any())
                        xmlChild.InnerText = "null";
                    else
                    {
                        int i = 0;
                        StringBuilder sb = new StringBuilder();
                        foreach (var plTime in batchPlanningTimes)
                        {
                            sb.AppendFormat("{0}:{1}|", i, plTime.ToString());
                        }
                        xmlChild.InnerText = sb.ToString();
                    }
                }
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["NewChildPosForBatch_ProdOrderPartslistPosID"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("NewChildPosForBatch_ProdOrderPartslistPosID");
                if (xmlChild != null)
                    xmlChild.InnerText = NewChildPosForBatch_ProdOrderPartslistPosID.HasValue ? NewChildPosForBatch_ProdOrderPartslistPosID.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["NewChildPosForBatch_PickingPosID"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("NewChildPosForBatch_PickingPosID");
                if (xmlChild != null)
                    xmlChild.InnerText = NewChildPosForBatch_PickingPosID.HasValue ? NewChildPosForBatch_PickingPosID.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["IsInPlanningWait"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("IsInPlanningWait");
                if (xmlChild != null)
                    xmlChild.InnerText = IsInPlanningWait.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["StartNextBatchAtProjectID1"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("StartNextBatchAtProjectID1");
                if (xmlChild != null)
                    xmlChild.InnerText = StartNextBatchAtProjectID1.HasValue ? StartNextBatchAtProjectID1.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["StartNextBatchAtProjectID2"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("StartNextBatchAtProjectID2");
                if (xmlChild != null)
                    xmlChild.InnerText = StartNextBatchAtProjectID2.HasValue ? StartNextBatchAtProjectID2.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }
        }
        #endregion

        #endregion
    }
}
