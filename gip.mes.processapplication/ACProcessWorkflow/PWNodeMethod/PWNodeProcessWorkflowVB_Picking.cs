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
                bool validationSuccess = true;
                var mandantoryConfigStores = MandatoryConfigStores;
                if (!ValidateExpectedConfigStores())
                {
                    validationSuccess = false;
                    return StartNextBatchResult.CycleWait;
                }

                MsgWithDetails msg2 = PickingManager.ValidateStart(dbApp, dbiPlus, picking, mandantoryConfigStores, ValidationBehaviour);
                if (msg2 != null)
                {
                    if (!msg2.IsSucceded())
                    {
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Error, PWClassName, nameof(ReadAndStartNextBatch), 1010), true);
                        validationSuccess = false;
                        return StartNextBatchResult.CycleWait;
                    }
                }
                if (validationSuccess)
                {
                    //if (ProdOrderManager.SetBatchPlanValidated(dbApp, currentProdOrderPartslist) > 0)
                    //dbApp.ACSaveChanges();
                }


                PickingPos pickingPos = null;
                //var queryOrderRelatedPickingLines = picking.PickingPos_Picking.Where(c => c.InOrderPosID.HasValue || c.OutOrderPosID.HasValue || c.PickingPosProdOrderPartslistPos_PickingPos.Any());
                //if (queryOrderRelatedPickingLines.Any())
                //{
                pickingPos = picking.PickingPos_Picking
                    .Where(c => !c.MDDelivPosLoadStateID.HasValue || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)
                    .OrderBy(c => c.Sequence)
                    .FirstOrDefault();
                if (pickingPos == null)
                {
                    pickingPos = picking.PickingPos_Picking
                    .Where(c => c.MDDelivPosLoadStateID.HasValue && c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive)
                    .OrderBy(c => c.Sequence)
                    .FirstOrDefault();
                }
                if (pickingPos == null)
                {
                    StartNextPickingWF(detachedPicking);
                    return StartNextBatchResult.Done;
                }
                nextPos = pickingPos;
                if (pickingPos.FromFacility != null && pickingPos.FromFacility.VBiFacilityACClassID != null)
                {
                    var queryProject = dbApp.ACClass.Where(c => c.ACClassID == pickingPos.FromFacility.VBiFacilityACClassID).Select(c => c.ACProjectID);
                    if (queryProject.Any())
                        startNextBatchAtProjectID1 = queryProject.FirstOrDefault();
                }
                if (pickingPos.ToFacility != null && pickingPos.ToFacility.VBiFacilityACClassID != null)
                {
                    var queryProject = dbApp.ACClass.Where(c => c.ACClassID == pickingPos.ToFacility.VBiFacilityACClassID).Select(c => c.ACProjectID);
                    if (queryProject.Any())
                        startNextBatchAtProjectID2 = queryProject.FirstOrDefault();
                }
                //}
                //else
                //{
                //    message = "No order related lines found";
                //    if (IsAlarmActive(ProcessAlarm, message) == null)
                //        Messages.LogError(this.GetACUrl(), "StartDischargingPicking(2)", message);
                //    OnNewAlarmOccurred(ProcessAlarm, message, true);
                //    return StartNextBatchResult.Done;
                //}

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
                using (DatabaseApp dbApp = new DatabaseApp())
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

                            ACMethod acMethod = ApplicationManager.NewACMethod(acClassMethod.ACIdentifier);
                            if (acMethod == null)
                                return;
                            string secondaryKey = Root.NoManager.GetNewNo(dbiPlus, typeof(gip.core.datamodel.ACProgram), gip.core.datamodel.ACProgram.NoColumnName, gip.core.datamodel.ACProgram.FormatNewNo, this);
                            gip.core.datamodel.ACProgram program = gip.core.datamodel.ACProgram.NewACObject(dbiPlus, null, secondaryKey);
                            program.ProgramACClassMethod = acClassMethod;
                            program.WorkflowTypeACClass = acClassMethod.WorkflowTypeACClass;
                            dbiPlus.ACProgram.AddObject(program);
                            if (dbiPlus.ACSaveChanges() == null)
                            {
                                ACValue paramProgram = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACProgram.ClassName);
                                if (paramProgram == null)
                                    acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACProgram.ClassName, typeof(Guid), program.ACProgramID));
                                else
                                    paramProgram.Value = program.ACProgramID;

                                ACValue acValue = acMethod.ParameterValueList.GetACValue(Picking.ClassName);
                                if (acValue == null)
                                    acMethod.ParameterValueList.Add(new ACValue(Picking.ClassName, typeof(Guid), nextPicking.PickingID));
                                else
                                    acValue.Value = nextPicking.PickingID;

                                ACValue paramACClassWF = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACClassWF.ClassName);
                                if (paramACClassWF == null)
                                    acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACClassWF.ClassName, typeof(Guid), ContentACClassWF.ACClassWFID));
                                else
                                    paramACClassWF.Value = ContentACClassWF.ACClassWFID;

                                nextPicking.PickingStateIndex = (short)PickingStateEnum.WFActive;
                                dbApp.ACSaveChanges();

                                ApplicationManager.ExecuteMethod(acClassMethod.ACIdentifier, acMethod);

                                StartedAnyPickingWF.ValueT = true;
                            }
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
