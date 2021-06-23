using System;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;

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
                Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, MN_ReadAndStartNextBatch, 1000, "Error50155");
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartNextBatchResult.CycleWait;
            }

            bool canBeStarted = CheckIfNextBatchCanBeStarted();
            if (!canBeStarted)
                return StartNextBatchResult.WaitForNextEvent;

            string message = "";
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
                MsgWithDetails msg2 = PickingManager.ValidateStart(dbApp, dbiPlus, picking, mandantoryConfigStores, PARole.ValidationBehaviour.Strict);
                if (msg2 != null)
                {
                    if (!msg2.IsSucceded())
                    {
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Error, PWClassName, MN_ReadAndStartNextBatch, 1010), true);
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
                if (!picking.PickingPos_Picking.Where(c => c.InOrderPosID.HasValue || c.OutOrderPosID.HasValue || c.PickingPosProdOrderPartslistPos_PickingPos.Any()).Any())
                {
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
                }
                else
                {
                    message = "TODO: Orderhandling with Picking is not implemented";
                    if (IsAlarmActive(ProcessAlarm, message) == null)
                        Messages.LogError(this.GetACUrl(), "StartDischargingPicking(2)", message);
                    OnNewAlarmOccurred(ProcessAlarm, message, true);
                    return StartNextBatchResult.Done;
                }

                PWMethodSingleDosing singleDosing = ParentPWMethod<PWMethodSingleDosing>();
                Guid? selectedSingleDosingACClassWFID = singleDosing?.SelectedSingleDosingACClassWFID;
                if (singleDosing != null && selectedSingleDosingACClassWFID.HasValue && ContentACClassWF != null)
                {
                    if (ContentACClassWF.ACClassWFID != selectedSingleDosingACClassWFID.Value)
                        return StartNextBatchResult.Done;
                }

            }
            return StartNextBatchResult.StartNextBatch;
        }

        #endregion
    }
}
