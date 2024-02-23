using System;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System.Runtime.InteropServices.ComTypes;
using System.Collections.Generic;

namespace gip.mes.processapplication
{
    public partial class PWNodeProcessWorkflowVB
    {
        #region Properties
        #region Manager
        protected ACPickingManager PickingManager
        {
            get
            {
                PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
                return pwMethodTransport != null ? pwMethodTransport.PickingManager : null;
            }
        }
        #endregion

        public Picking CurrentPicking
        {
            get
            {
                Picking picking = null;
                if (IsRelocation)
                    picking = ParentPWMethod<PWMethodRelocation>().CurrentPicking;
                else if (IsLoading)
                    picking = ParentPWMethod<PWMethodLoading>().CurrentPicking;
                else if (IsIntake)
                    picking = ParentPWMethod<PWMethodIntake>().CurrentPicking;
                return picking;
            }
        }

        protected Guid? _NewChildPosForBatch_PickingPosID;
        protected Guid? NewChildPosForBatch_PickingPosID
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _NewChildPosForBatch_PickingPosID;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _NewChildPosForBatch_PickingPosID = value;
                }
            }
        }

        [ACPropertyBindingSource(9999, "", "en{'Has started any picking workflow'}de{'Has started any picking workflow'}", "", false, true)]
        public IACContainerTNet<bool> StartedAnyPickingWF
        {
            get;
            set;
                
        }

        #endregion

        #region Methods

        protected virtual StartNextBatchResult ReadAndStartNextBatch(Picking detachedPicking, out PickingPos nextPos, out Guid? startNextBatchAtProjectID1, out Guid? startNextBatchAtProjectID2, out bool isLastBatch)
        {
            nextPos = null;
            startNextBatchAtProjectID1 = null;
            startNextBatchAtProjectID2 = null;
            isLastBatch = false;
            if (detachedPicking == null)
                return StartNextBatchResult.CycleWait;

            if (ACProgramVB == null || ContentACClassWF == null || PickingManager == null)
            {
                //Error50155: ACProgramVB is null or ContentACClassWF or PickingManager is null.
                Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, nameof(ReadAndStartNextBatch), 1000, "Error50155");
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartNextBatchResult.CycleWait;
            }

            bool canBeStarted = CheckIfNextBatchCanBeStarted();
            if (!canBeStarted)
                return StartNextBatchResult.WaitForNextEvent;

            if (WillReadAndStartNextBatchCompleteNode_Picking(20))
            {
                StartNextPickingWF(detachedPicking);
                return StartNextBatchResult.Done;
            }

            if (IsThisDesignatedProcessNode.HasValue && !IsThisDesignatedProcessNode.Value)
                return StartNextBatchResult.Done;

            using (Database dbiPlus = new Database())
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                Picking picking = detachedPicking.FromAppContext<Picking>(dbApp);
                var mandantoryConfigStores = MandatoryConfigStores;
                if (!ValidateExpectedConfigStores())
                    return StartNextBatchResult.CycleWait;

                MsgWithDetails msg2 = PickingManager.ValidateStart(dbApp, dbiPlus, picking, mandantoryConfigStores, ValidationBehaviour);
                if (msg2 != null)
                {
                    if (!msg2.IsSucceded())
                    {
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Error, PWClassName, nameof(ReadAndStartNextBatch), 1010), true);
                        return StartNextBatchResult.CycleWait;
                    }
                }


                bool allCompleted = true;
                PickingPos pFirstReadyToLoad = null;
                PickingPos pFirstLoadingActive = null;
                foreach (PickingPos p in picking.PickingPos_Picking.OrderBy(c => c.Sequence))
                {
                    if (p.MDDelivPosLoadState == null || p.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.NewCreated)
                        allCompleted = false;
                    else
                    {
                        if (pFirstReadyToLoad == null && p.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)
                        {
                            pFirstReadyToLoad = p;
                            allCompleted = false;
                        }
                        else if (pFirstLoadingActive == null && p.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.LoadingActive)
                        {
                            pFirstLoadingActive = p;
                            allCompleted = false;
                        }
                    }
                    if (allCompleted == false && pFirstReadyToLoad != null && pFirstLoadingActive != null)
                        break;
                }

                if (allCompleted)
                {
                    if (EndProdOrderPartslistMode > EndPListMode.DoNothing)
                        picking.PickingState = PickingStateEnum.Finished;
                    else
                        picking.PickingState = PickingStateEnum.InProcess;
                    dbApp.ACSaveChanges();
                }
                else if (pFirstReadyToLoad != null)
                    nextPos = pFirstReadyToLoad;
                else if (pFirstLoadingActive != null)
                    nextPos = pFirstLoadingActive;

                if (nextPos == null)
                {
                    StartNextPickingWF(detachedPicking);
                    return StartNextBatchResult.Done;
                }
                if (nextPos.FromFacility != null && nextPos.FromFacility.VBiFacilityACClassID != null)
                {
                    Guid? classID = nextPos.FromFacility.VBiFacilityACClassID;
                    if (classID.HasValue)
                    {
                        var queryProject = dbApp.ACClass.Where(c => c.ACClassID == classID.Value).Select(c => c.ACProjectID);
                        if (queryProject.Any())
                            startNextBatchAtProjectID1 = queryProject.FirstOrDefault();
                    }
                }
                if (nextPos.ToFacility != null && nextPos.ToFacility.VBiFacilityACClassID != null)
                {
                    Guid? classID = nextPos.ToFacility.VBiFacilityACClassID;
                    if (classID.HasValue)
                    {
                        var queryProject = dbApp.ACClass.Where(c => c.ACClassID == classID.Value).Select(c => c.ACProjectID);
                        if (queryProject.Any())
                            startNextBatchAtProjectID2 = queryProject.FirstOrDefault();
                    }
                }

                if (WillReadAndStartNextBatchCompleteNode_Picking(30))
                    return StartNextBatchResult.Done;
            }
            return StartNextBatchResult.StartNextBatch;
        }

        protected virtual bool WillReadAndStartNextBatchCompleteNode_Picking(short invokeID)
        {
            //PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
            //Guid? designatedProcessNodeACClassWFID = pwMethodTransport?.DesignatedProcessNodeACClassWFID;
            //if (pwMethodTransport != null && designatedProcessNodeACClassWFID.HasValue && ContentACClassWF != null)
            //{
            //    if (ContentACClassWF.ACClassWFID != designatedProcessNodeACClassWFID.Value)
            //        return true;
            //    if (invokeID >= 20 && BatchSizeLoss && CurrentACState >= ACStateEnum.SMRunning && this.IterationCount.ValueT > 0)
            //        return true;
            //}
            //return false;

            PWMethodSingleDosing singleDosing = ParentPWMethod<PWMethodSingleDosing>();
            Guid? selectedSingleDosingACClassWFID = singleDosing?.SelectedSingleDosingACClassWFID;
            if (singleDosing != null && selectedSingleDosingACClassWFID.HasValue && ContentACClassWF != null)
            {
                if (ContentACClassWF.ACClassWFID != selectedSingleDosingACClassWFID.Value)
                    return true;
                if (invokeID >= 20 && BatchSizeLoss && CurrentACState >= ACStateEnum.SMRunning && this.IterationCount.ValueT > 0)
                    return true;
            }
            if (invokeID == 10 && IsThisDesignatedProcessNode.HasValue && !IsThisDesignatedProcessNode.Value && CompleteIfNotPlan)
                return true;
            
            return false;
        }

        protected bool? IsThisDesignatedProcessNode
        {
            get
            {
                PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
                if (pwMethodTransport == null || ContentACClassWF == null)
                    return false;
                Guid? designatedProcessNodeACClassWFID = pwMethodTransport.DesignatedProcessNodeACClassWFID;
                if (!designatedProcessNodeACClassWFID.HasValue)
                    return null;
                return ContentACClassWF.ACClassWFID == designatedProcessNodeACClassWFID.Value;
            }
        }

        protected virtual void StartNextPickingWF(Picking detachedPicking)
        {
            if (StartedAnyPickingWF.ValueT)
                return;

            try
            {
                using (Database dbiPlus = new Database())
                using (DatabaseApp dbApp = new DatabaseApp(dbiPlus))
                {
                    IEnumerable<Picking> possiblePickings = dbApp.Picking.Where(c => c.PickingStateIndex == (short)PickingStateEnum.New
                                                                                 && c.ACClassMethodID == detachedPicking.ACClassMethodID
                                                                                 && c.InsertDate > detachedPicking.InsertDate
                                                                                 && c.PickingConfig_Picking.Any());

                    if (possiblePickings.Any() && ContentACClassWF != null)
                    {
                        Picking nextPicking = possiblePickings.Where(c => c.PickingConfig_Picking.Any(x => x.XMLConfig == ContentACClassWF.ACClassWFID.ToString()))
                                                              .OrderBy(c => c.InsertDate)
                                                              .FirstOrDefault();

                        if (nextPicking != null)
                        {
                            core.datamodel.ACClassMethod acClassMethod = nextPicking.ACClassMethod.FromIPlusContext<core.datamodel.ACClassMethod>(dbiPlus);
                            MsgWithDetails saveMsg = this.PickingManager.StartPicking(dbApp, ApplicationManager, nextPicking, acClassMethod, ContentACClassWF, false);
                            if (saveMsg == null)
                            {
                                StartedAnyPickingWF.ValueT = true;
                            }

                            //ACMethod acMethod = ApplicationManager.NewACMethod(acClassMethod.ACIdentifier);
                            //if (acMethod == null)
                            //    return;
                            //string secondaryKey = Root.NoManager.GetNewNo(dbiPlus, typeof(gip.core.datamodel.ACProgram), gip.core.datamodel.ACProgram.NoColumnName, gip.core.datamodel.ACProgram.FormatNewNo, this);
                            //gip.core.datamodel.ACProgram program = gip.core.datamodel.ACProgram.NewACObject(dbiPlus, null, secondaryKey);
                            //program.ProgramACClassMethod = acClassMethod;
                            //program.WorkflowTypeACClass = acClassMethod.WorkflowTypeACClass;
                            //dbiPlus.ACProgram.AddObject(program);
                            //if (dbiPlus.ACSaveChanges() == null)
                            //{
                            //    ACValue paramProgram = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACProgram.ClassName);
                            //    if (paramProgram == null)
                            //        acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACProgram.ClassName, typeof(Guid), program.ACProgramID));
                            //    else
                            //        paramProgram.Value = program.ACProgramID;

                            //    ACValue acValue = acMethod.ParameterValueList.GetACValue(Picking.ClassName);
                            //    if (acValue == null)
                            //        acMethod.ParameterValueList.Add(new ACValue(Picking.ClassName, typeof(Guid), nextPicking.PickingID));
                            //    else
                            //        acValue.Value = nextPicking.PickingID;

                            //    ACValue paramACClassWF = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACClassWF.ClassName);
                            //    if (paramACClassWF == null)
                            //        acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACClassWF.ClassName, typeof(Guid), ContentACClassWF.ACClassWFID));
                            //    else
                            //        paramACClassWF.Value = ContentACClassWF.ACClassWFID;

                            //    nextPicking.PickingStateIndex = (short)PickingStateEnum.WFActive;
                            //    dbApp.ACSaveChanges();

                            //    ApplicationManager.ExecuteMethod(acClassMethod.ACIdentifier, acMethod);

                            //    StartedAnyPickingWF.ValueT = true;
                            //}
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), nameof(StartNextPickingWF), e);
            }
        }

        #endregion
    }
}
