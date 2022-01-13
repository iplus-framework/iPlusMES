using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Globalization;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "", Global.ACKinds.TPARole, Global.ACStorableTypes.Required, false, false)]
    public class RemoteFacilityManager : PARole
    {
        #region ctor's
        public RemoteFacilityManager(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool result = base.ACInit(startChildMode);
            _DelegateQueue = new ACDelegateQueue(ACIdentifier);
            _DelegateQueue.StartWorkerThread();
            return result;
        }

        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_DelegateQueue != null)
            {
                _DelegateQueue.StopWorkerThread();
                _DelegateQueue = null;
            }
            bool result = base.ACDeInit(deleteACClassTask);
            return result;
        }


        #endregion

        #region Properties
        private ACDelegateQueue _DelegateQueue = null;
        public ACDelegateQueue DelegateQueue
        {
            get
            {
                return _DelegateQueue;
            }
        }
        #endregion

        #region static Methods
        #endregion

        #region private 
        #endregion

        #region Methods
        [ACMethodInfo("", "en{'Synchronize remote facility'}de{'Synchronize remote facility'}", 1000)]
        public void RefreshFacility(string acUrlOfCaller, RemoteStorePostingData remoteStorePosting)
        {
            if (String.IsNullOrEmpty(acUrlOfCaller) || remoteStorePosting == null)
                return;
            List<string> hierarchy = ACUrlHelper.ResolveParents(acUrlOfCaller);
            string remoteProxyACurl = hierarchy.FirstOrDefault();
            if (String.IsNullOrEmpty(remoteProxyACurl))
                return;
            ACComponent remoteAppManager = ACUrlCommand(remoteProxyACurl) as ACComponent;
            if (remoteAppManager == null)
                return;
            string remoteConnString = remoteAppManager["RemoteConnString"] as string;
            if (String.IsNullOrEmpty(remoteConnString))
                return;
            DelegateQueue.Add(() => { SynchronizeFacility(acUrlOfCaller, remoteConnString, remoteStorePosting); });
        }

        protected void SynchronizeFacility(string acUrlOfCaller, string remoteConnString, RemoteStorePostingData remoteStorePosting)
        {
            ACPickingManager pickingManager = null;
            FacilityManager facilityManager = null;
            try
            {
                using (DatabaseApp dbRemote = new DatabaseApp(remoteConnString))
                using (DatabaseApp dbLocal = new DatabaseApp())
                {
                    Database dbIplus = Root.Database as Database;
                    core.datamodel.ACClass aCClass = dbIplus.GetACTypeByACUrlComp(remoteStorePosting.FacilityUrlOfRecipient);
                    if (aCClass == null)
                        return;
                    Facility localFacility = dbLocal.Facility
                                                    .Include(c => c.Facility1_ParentFacility)
                                                    .Where(c => c.VBiFacilityACClassID.HasValue && c.VBiFacilityACClassID == aCClass.ACClassID)
                                                    .FirstOrDefault();
                    if (localFacility == null || localFacility.Facility1_ParentFacility == null)
                        return;
                    Facility remoteFacility = dbRemote.Facility.Include(c => c.Facility1_ParentFacility)
                                      .Where(c => c.FacilityNo == localFacility.FacilityNo
                                                    && c.Facility1_ParentFacility != null
                                                    && c.Facility1_ParentFacility.FacilityNo == localFacility.Facility1_ParentFacility.FacilityNo)
                                     .FirstOrDefault();
                    if (remoteFacility == null)
                        return;
                    //remoteStorePosting.FacilityUrlOfRecipient
                    List<FacilityCharge> changedRemoteFCs = new List<FacilityCharge>();
                    List<Picking> changedRemotePickings = new List<Picking>();
                    foreach (var entry in remoteStorePosting.FBIds)
                    {
                        if (entry.EntityType == FacilityBooking.ClassName)
                        {
                            var queryRemoteFBCs =
                                dbRemote.FacilityBookingCharge
                                .Include(c => c.PickingPos)
                                .Include(c => c.PickingPos.Picking)
                                .Include(c => c.InwardFacilityCharge)
                                .Include(c => c.InwardFacilityCharge.Facility)
                                .Include(c => c.InwardFacilityCharge.FacilityLot)
                                .Include(c => c.OutwardFacilityCharge)
                                .Include(c => c.OutwardFacilityCharge.Facility)
                                .Include(c => c.OutwardFacilityCharge.FacilityLot)
                                .Where(c => c.FacilityBookingID == entry.KeyId);
                            foreach (var remoteFBC in queryRemoteFBCs)
                            {
                                if (remoteFBC.InwardFacilityID.HasValue && remoteFBC.InwardFacilityID.Value == remoteFacility.FacilityID)
                                {
                                    if (!changedRemoteFCs.Contains(remoteFBC.InwardFacilityCharge))
                                        changedRemoteFCs.Add(remoteFBC.InwardFacilityCharge);
                                }
                                if (remoteFBC.OutwardFacilityID.HasValue && remoteFBC.OutwardFacilityID.Value == remoteFacility.FacilityID)
                                {
                                    if (!changedRemoteFCs.Contains(remoteFBC.OutwardFacilityCharge))
                                        changedRemoteFCs.Add(remoteFBC.OutwardFacilityCharge);
                                }
                                if (remoteFBC.PickingPos != null)
                                {
                                    if (!changedRemotePickings.Contains(remoteFBC.PickingPos.Picking))
                                        changedRemotePickings.Add(remoteFBC.PickingPos.Picking);
                                }
                            }
                        }
                        else if (entry.EntityType == Picking.ClassName)
                        {
                            var remotePicking = dbRemote.Picking.Include(c => c.MDPickingType)
                                            .Include("PickingPos_Picking")
                                            .Include("PickingPos_Picking.PickingMaterial")
                                            .Include("PickingPos_Picking.FromFacility")
                                            .Include("PickingPos_Picking.ToFacility")
                                            .Include("PickingPos_Picking.MDDelivPosLoadState")
                                            .Where(c => c.PickingID == entry.KeyId)
                                            .FirstOrDefault();

                            if (remotePicking != null)
                            {
                                // All lines deleted => delete Picking
                                if (!remotePicking.PickingPos_Picking.Any())
                                    remotePicking = null;
                                else if (!changedRemotePickings.Contains(remotePicking))
                                    changedRemotePickings.Add(remotePicking);
                            }
                            if (remotePicking == null)
                            {
                                // Remote Picking was deleted, delete local Picking
                                var localPicking = dbLocal.Picking.Include(c => c.MDPickingType)
                                            .Include("PickingPos_Picking")
                                            .Include("PickingPos_Picking.PickingMaterial")
                                            .Include("PickingPos_Picking.FromFacility")
                                            .Include("PickingPos_Picking.ToFacility")
                                            .Include("PickingPos_Picking.MDDelivPosLoadState")
                                            .Where(c => c.PickingID == entry.KeyId)
                                            .FirstOrDefault();
                                // TODO: Unassign pos an delete Picking....
                                if (localPicking != null)
                                {
                                    PickingPos[] localPickingLines = localPicking.PickingPos_Picking.ToArray();
                                    foreach (PickingPos localPickingPos in localPickingLines)
                                        pickingManager.UnassignPickingPos(localPickingPos, dbLocal);

                                    localPicking.DeleteACObject(dbLocal, false);
                                }
                            }
                        }
                    }

                    foreach (FacilityCharge changedRemoteFC in changedRemoteFCs)
                    {
                        FacilityCharge localFC = dbLocal.FacilityCharge.Where(c => c.Material.MaterialNo == changedRemoteFC.Material.MaterialNo
                                                                                && c.FacilityID == localFacility.FacilityID
                                                                                && (!changedRemoteFC.FacilityLotID.HasValue || c.FacilityLotID == changedRemoteFC.FacilityLotID)
                                                                                && c.SplitNo == changedRemoteFC.SplitNo)
                                                                        .FirstOrDefault();
                        if (localFC == null)
                        {
                            // Add New
                            FacilityLot localLot = dbLocal.FacilityLot.FirstOrDefault(c => c.LotNo == changedRemoteFC.FacilityLot.LotNo);
                            if (localLot == null)
                            {
                                localLot = FacilityLot.NewACObject(dbLocal, null, changedRemoteFC.FacilityLot.LotNo);
                                localLot.ExternLotNo = changedRemoteFC.FacilityLot.ExternLotNo;
                                localLot.ExternLotNo2 = changedRemoteFC.FacilityLot.ExternLotNo2;
                                localLot.ExpirationDate = changedRemoteFC.FacilityLot.ExpirationDate;
                                dbLocal.FacilityLot.AddObject(localLot);

                                localFC = FacilityCharge.NewACObject(dbLocal, localLot);
                                dbLocal.FacilityCharge.AddObject(localFC);
                            }
                        }
                        else if (localFC.NotAvailable)
                        {
                            // Restore
                            // Restore -> Make charge available again
                            ACMethodBooking bookReleaseStateFree = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ReleaseState_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                            bookReleaseStateFree.MDReleaseState = MDReleaseState.DefaultMDReleaseState(dbLocal, MDReleaseState.ReleaseStates.Free);
                            ACMethodEventArgs resultRelaseFree = facilityManager.BookFacility(bookReleaseStateFree, dbLocal) as ACMethodEventArgs;
                            if (!bookReleaseStateFree.ValidMessage.IsSucceded() || bookReleaseStateFree.ValidMessage.HasWarnings())
                                Messages.Msg(bookReleaseStateFree.ValidMessage);
                            else if (resultRelaseFree.ResultState == Global.ACMethodResultState.Failed || resultRelaseFree.ResultState == Global.ACMethodResultState.Notpossible)
                            {
                                if (String.IsNullOrEmpty(resultRelaseFree.ValidMessage.Message))
                                    resultRelaseFree.ValidMessage.Message = resultRelaseFree.ResultState.ToString();
                                Messages.Msg(resultRelaseFree.ValidMessage);
                            }
                        }
                        // TODO: Booking to Absolute stock
                        ACMethodBooking bookAbsolute = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_InwardMovement_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                        bookAbsolute.QuantityIsAbsolute = true;
                        bookAbsolute.InwardFacilityCharge = localFC;
                        bookAbsolute.InwardTargetQuantity = changedRemoteFC.AvailableQuantity;

                        ACMethodEventArgs resultAbsolute = facilityManager.BookFacility(bookAbsolute, dbLocal) as ACMethodEventArgs;
                        if (!bookAbsolute.ValidMessage.IsSucceded() || bookAbsolute.ValidMessage.HasWarnings())
                            Messages.Msg(bookAbsolute.ValidMessage);
                        else if (resultAbsolute.ResultState == Global.ACMethodResultState.Failed || resultAbsolute.ResultState == Global.ACMethodResultState.Notpossible)
                        {
                            if (String.IsNullOrEmpty(resultAbsolute.ValidMessage.Message))
                                resultAbsolute.ValidMessage.Message = resultAbsolute.ResultState.ToString();
                            Messages.Msg(resultAbsolute.ValidMessage);
                        }
                    }

                    foreach (Picking remotePicking in changedRemotePickings)
                    {
                        if (!IsRemotePickingRequiredHere(acUrlOfCaller, remoteConnString, remoteStorePosting, dbRemote, dbLocal, remotePicking))
                            continue;
                        OnHandleRemotePicking(acUrlOfCaller, remoteConnString, remoteStorePosting, dbRemote, dbLocal, remotePicking);
                        Picking localPicking = dbLocal.Picking.FirstOrDefault(c => c.PickingID == remotePicking.PickingID);
                        if (localPicking == null)
                        {
                            // TODO: Check PickingNo - if duplicate with same PickingNo and different KeyOfExtSys exist ?? 
                            localPicking = Picking.NewACObject(dbLocal, null, remotePicking.PickingNo);
                            localPicking.PickingID = remotePicking.PickingID;
                            localPicking.KeyOfExtSys = remotePicking.KeyOfExtSys;
                            dbLocal.Picking.AddObject(localPicking);
                        }

                        localPicking.MDPickingType = dbLocal.MDPickingType.FirstOrDefault(c => c.MDPickingTypeIndex == remotePicking.MDPickingType.MDPickingTypeIndex);
                        localPicking.DeliveryDateFrom = remotePicking.DeliveryDateFrom;
                        CompanyAddress companyAddress = dbLocal.CompanyAddress.FirstOrDefault(c => c.Company.KeyOfExtSys == remotePicking.DeliveryCompanyAddress.Company.KeyOfExtSys);
                        remotePicking.DeliveryCompanyAddress = companyAddress;
                        localPicking.PickingStateIndex = remotePicking.PickingStateIndex;

                        // updating pos
                        Guid[] remotePosIDs = remotePicking.PickingPos_Picking.Select(c => c.PickingPosID).ToArray();
                        Guid[] localPosIDs = localPicking.PickingPos_Picking.Select(c => c.PickingPosID).ToArray();


                        Guid[] localPosIDsForDelete = localPosIDs.Where(c => !remotePosIDs.Contains(c)).ToArray();
                        foreach (Guid posID in localPosIDsForDelete)
                        {
                            PickingPos posForDelete = localPicking.PickingPos_Picking.Where(c => c.PickingPosID == posID).FirstOrDefault();
                            localPicking.PickingPos_Picking.Remove(posForDelete);
                            posForDelete.DeleteACObject(dbLocal, false);
                        }

                        foreach (Guid posID in remotePosIDs)
                        {
                            PickingPos remotePos = remotePicking.PickingPos_Picking.FirstOrDefault(c => c.PickingPosID == posID);
                            PickingPos localPos = localPicking.PickingPos_Picking.FirstOrDefault(c => c.PickingPosID == posID);
                            if (localPos == null)
                            {
                                localPos = PickingPos.NewACObject(dbLocal, localPicking);
                                localPicking.PickingPos_Picking.Add(localPos);
                            }
                            localPos.PickingMaterial = dbLocal.Material.FirstOrDefault(c => c.MaterialNo == remotePos.PickingMaterial.MaterialNo);
                            localPos.PickingQuantityUOM = remotePos.PickingQuantityUOM;
                            localPos.Sequence = remotePos.Sequence;
                            if (remotePos.FromFacility != null)
                                localPos.FromFacility = dbLocal.Facility.FirstOrDefault(c => c.FacilityNo == remotePos.ToFacility.FacilityNo);
                            else
                                localPos.FromFacility = null;
                            if (remotePos.ToFacility != null)
                                localPos.ToFacility = dbLocal.Facility.FirstOrDefault(c => c.FacilityNo == remotePos.FromFacility.FacilityNo);
                            else
                                localPos.ToFacility = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), "SynchronizeFacility", e);
            }
        }

        protected virtual bool IsRemotePickingRequiredHere(string acUrlOfCaller, string remoteConnString, RemoteStorePostingData remoteStorePosting, DatabaseApp dbRemote, DatabaseApp dbLocal, Picking remotePicking)
        {
            return true;
        }

        protected virtual void OnHandleRemotePicking(string acUrlOfCaller, string remoteConnString, RemoteStorePostingData remoteStorePosting, DatabaseApp dbRemote, DatabaseApp dbLocal, Picking remotePicking)
        {
            // TODO:
        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "RefreshFacility":
                    RefreshFacility((string)acParameter[0], (RemoteStorePostingData)acParameter[1]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }

}
