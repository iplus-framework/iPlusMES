using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

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

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _PickingManager = ACPickingManager.ACRefToServiceInstance(this);
            if (_PickingManager == null)
                throw new Exception("PickingManager not configured");

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

            if (_PickingManager != null)
                ACPickingManager.DetachACRefFromServiceInstance(this, _PickingManager);
            _PickingManager = null;

            if (_ACFacilityManager != null)
                FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            return result;
        }


        #endregion

        #region Properties

        #region Managers

        protected ACRef<ACComponent> _ACFacilityManager = null;
        protected FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }

        protected ACRef<ACPickingManager> _PickingManager = null;
        protected ACPickingManager PickingManager
        {
            get
            {
                if (_PickingManager == null)
                    return null;
                return _PickingManager.ValueT;
            }
        }
        #endregion

        private ACDelegateQueue _DelegateQueue = null;
        public ACDelegateQueue DelegateQueue
        {
            get
            {
                return _DelegateQueue;
            }
        }

        /// <summary>
        /// <summary>
        /// Source Property: 
        /// </summary>
        private DateTime? _LastSyncTime;
        [ACPropertyInfo(999, "LastSyncTime", "en{'TODO:LastSyncTime'}de{'TODO:LastSyncTime'}")]
        public DateTime? LastSyncTime
        {
            get
            {
                return _LastSyncTime;
            }
            set
            {
                if (_LastSyncTime != value)
                {
                    _LastSyncTime = value;
                    OnPropertyChanged("LastSyncTime");
                }
            }
        }

        #endregion

        #region Methods

        #region Private
        private bool ResolveCallerContext(string acUrlOfCaller, RemoteStorePostingData remoteStorePosting, out string remoteConnString, out ACComponent remoteAppManager)
        {
            remoteAppManager = null;
            remoteConnString = null;
            if (String.IsNullOrEmpty(acUrlOfCaller) || remoteStorePosting == null)
                return false;
            List<string> hierarchy = ACUrlHelper.ResolveParents(acUrlOfCaller);
            string remoteProxyACurl = hierarchy.FirstOrDefault();
            if (String.IsNullOrEmpty(remoteProxyACurl))
                return false;
            remoteAppManager = ACUrlCommand(remoteProxyACurl) as ACComponent;
            if (remoteAppManager == null)
                return false;
            remoteConnString = remoteAppManager[nameof(RemoteAppManager.RemoteConnString)] as string;
            if (String.IsNullOrEmpty(remoteConnString))
                return false;
            return true;
        }
        #endregion

        #region Called from Remote-System
        [ACMethodInfo("", "en{'Synchronize remote facility'}de{'Synchronize remote facility'}", 1000)]
        public void RefreshFacility(string acUrlOfCaller, RemoteStorePostingData remoteStorePosting)
        {
            string remoteConnString;
            ACComponent remoteAppManager;
            if (!ResolveCallerContext(acUrlOfCaller, remoteStorePosting, out remoteConnString, out remoteAppManager))
                return;
            DelegateQueue.Add(() => { SynchronizeFacility(acUrlOfCaller, remoteConnString, remoteStorePosting); });
        }

        [ACMethodInfo("", "en{'Update from remote material'}de{'Update from remote material'}", 1001)]
        public void RefreshMaterial(string acUrlOfCaller, RemoteStorePostingData remoteStorePosting)
        {
            string remoteConnString;
            ACComponent remoteAppManager;
            if (!ResolveCallerContext(acUrlOfCaller, remoteStorePosting, out remoteConnString, out remoteAppManager))
                return;
            DelegateQueue.Add(() => { SynchronizeMaterial(acUrlOfCaller, remoteConnString, remoteStorePosting); });
        }
        #endregion

        #region Booking and Picking-Sync

        #region Booking and Picking-Sync -> SynchronizeFacility

        protected void SynchronizeFacility(string acUrlOfCaller, string remoteConnString, RemoteStorePostingData remoteStorePosting)
        {
            try
            {
                if (remoteStorePosting == null || remoteStorePosting.FBIds == null || !remoteStorePosting.FBIds.Any())
                    return;
                MsgWithDetails msgWithDetails = null;
                using (DatabaseApp dbRemote = new DatabaseApp(remoteConnString))
                using (DatabaseApp dbLocal = new DatabaseApp())
                {
                    Facility remoteFacility = null;
                    Facility localFacility = null;
                    RSPDEntry addOrUpdateFacilityMD = remoteStorePosting.FBIds.FirstOrDefault(c => c.EntityType == Facility.ClassName);
                    if (addOrUpdateFacilityMD != null)
                    {
                        remoteFacility = dbRemote.Facility
                                        .Include(c => c.Facility1_ParentFacility)
                                        .Where(c => c.FacilityID == addOrUpdateFacilityMD.KeyId)
                                        .FirstOrDefault();
                        if (remoteFacility != null)
                        {
                            localFacility = SynchronizeFacility(dbLocal, remoteFacility, addOrUpdateFacilityMD);
                        }
                    }

                    if (!SynchronizeFacilityACClass(remoteStorePosting, dbLocal, remoteFacility, ref localFacility, addOrUpdateFacilityMD))
                        return;

                    if (remoteFacility == null)
                    {
                        remoteFacility = dbRemote.Facility
                                        .Include(c => c.Facility1_ParentFacility)
                                        .Where(c => c.FacilityID == localFacility.FacilityID)
                                        .FirstOrDefault();

                        // If local Facility doesn't have the same Guid as Remote
                        if (remoteFacility == null)
                        {
                            remoteFacility = dbRemote.Facility
                                            .Include(c => c.Facility1_ParentFacility)
                                            .Where(c => c.FacilityNo == localFacility.FacilityNo
                                                        && c.Facility1_ParentFacility != null
                                                        && c.Facility1_ParentFacility.FacilityNo == localFacility.Facility1_ParentFacility.FacilityNo)
                                            .FirstOrDefault();
                        }
                    }
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
                                        PickingManager.UnassignPickingPos(localPickingPos, dbLocal);

                                    localPicking.DeleteACObject(dbLocal, false);
                                }
                            }
                        }
                    }

                    foreach (FacilityCharge changedRemoteFC in changedRemoteFCs)
                    {
                        SynchronizeFacilityCharge(dbLocal, changedRemoteFC);
                    }

                    foreach (Picking remotePicking in changedRemotePickings)
                    {
                        if (!IsRemotePickingRequiredHere(acUrlOfCaller, remoteConnString, remoteStorePosting, dbRemote, dbLocal, remotePicking))
                            continue;
                        OnHandleRemotePicking(acUrlOfCaller, remoteConnString, remoteStorePosting, dbRemote, dbLocal, remotePicking);
                        SynchronizePicking(dbLocal, remotePicking);
                    }

                    msgWithDetails = dbLocal.ACSaveChanges();
                    if (msgWithDetails != null)
                        Messages.LogMessageMsg(msgWithDetails);
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), "SynchronizeFacility", e);
            }
        }

        private bool SynchronizeFacilityACClass(RemoteStorePostingData remoteStorePosting, DatabaseApp dbLocal, Facility remoteFacility, ref Facility localFacility, RSPDEntry addOrUpdateFacilityMD)
        {
            MsgWithDetails msgWithDetails = null;
            Database dbIplus = Root.Database as Database;
            core.datamodel.ACClass aCClass = dbIplus.GetACTypeByACUrlComp(remoteStorePosting.FacilityUrlOfRecipient);
            if (aCClass == null)
            {
                if (addOrUpdateFacilityMD != null
                    && localFacility != null
                    && remoteFacility != null
                    && remoteFacility.VBiFacilityACClassID.HasValue)
                {
                    using (ACMonitor.Lock(dbIplus.QueryLock_1X000))
                    {
                        aCClass = core.datamodel.ACClass.NewACObject(dbIplus, ParentACComponent.ComponentClass);
                    }
                    remoteFacility.VBiFacilityACClass.CopyTo(aCClass, false);
                    aCClass.ACClassID = remoteFacility.VBiFacilityACClassID.Value;
                    aCClass.ACURLComponentCached = remoteStorePosting.FacilityUrlOfRecipient;
                    aCClass.ACURLCached = null;
                    aCClass.ACClass1_BasedOnACClass = dbIplus.GetACType("PAMRemoteStore");
                    aCClass.ACProjectID = ParentACComponent.ComponentClass.ACProjectID;
                    aCClass.ACPackageID = aCClass.ACClass1_BasedOnACClass.ACPackageID;
                    localFacility.VBiFacilityACClassID = aCClass.ACClassID;
                    using (ACMonitor.Lock(dbIplus.QueryLock_1X000))
                    {
                        dbIplus.ACClass.AddObject(aCClass);
                        msgWithDetails = dbIplus.ACSaveChanges();
                        if (msgWithDetails != null)
                            dbIplus.ACUndoChanges();
                    }
                    if (msgWithDetails != null)
                        Messages.LogMessageMsg(msgWithDetails);

                    msgWithDetails = dbLocal.ACSaveChanges();
                    if (msgWithDetails != null)
                    {
                        Messages.LogMessageMsg(msgWithDetails);
                        dbLocal.ACUndoChanges();
                    }
                }
                return false;
            }
            else if (addOrUpdateFacilityMD != null && localFacility.EntityState == System.Data.EntityState.Added)
            {
                localFacility.VBiFacilityACClassID = aCClass.ACClassID;
                msgWithDetails = dbLocal.ACSaveChanges();
                if (msgWithDetails != null)
                {
                    Messages.LogMessageMsg(msgWithDetails);
                    dbLocal.ACUndoChanges();
                }
            }

            if (localFacility == null)
            {
                localFacility = dbLocal.Facility
                                            .Include(c => c.Facility1_ParentFacility)
                                            .Where(c => c.VBiFacilityACClassID.HasValue && c.VBiFacilityACClassID == aCClass.ACClassID)
                                            .FirstOrDefault();
            }
            if (localFacility == null || localFacility.Facility1_ParentFacility == null)
                return false;
            return true;
        }

        private void SynchronizeFacilityCharge(DatabaseApp dbLocal, FacilityCharge changedRemoteFC)
        {
            FacilityCharge localFC = null;

            // Search charge with same ID
            localFC = dbLocal.FacilityCharge.Where(c => c.FacilityChargeID == changedRemoteFC.FacilityChargeID).FirstOrDefault();

            bool successSaveCharge = true;
            if (localFC == null)
            {
                // Add New
                FacilityLot localLot = null;
                if (changedRemoteFC.FacilityLotID != null && changedRemoteFC.FacilityLotID != Guid.Empty)
                {
                    localLot = dbLocal.FacilityLot.FirstOrDefault(c => c.FacilityLotID == changedRemoteFC.FacilityLotID);
                    if (localLot == null)
                    {
                        localLot = FacilityLot.NewACObject(dbLocal, null, changedRemoteFC.FacilityLot.LotNo);
                        localLot.FacilityLotID = changedRemoteFC.FacilityLotID ?? Guid.Empty;
                        dbLocal.FacilityLot.AddObject(localLot);
                    }
                    localLot.ExternLotNo = changedRemoteFC.FacilityLot.ExternLotNo;
                    localLot.ExternLotNo2 = changedRemoteFC.FacilityLot.ExternLotNo2;
                    localLot.ExpirationDate = changedRemoteFC.FacilityLot.ExpirationDate;
                }

                localFC = FacilityCharge.NewACObject(dbLocal, localLot);
                localFC.FacilityChargeID = changedRemoteFC.FacilityChargeID;
                localFC.CopyFrom(changedRemoteFC, true);
                dbLocal.FacilityCharge.AddObject(localFC);
                MsgWithDetails msgSaveCharge = dbLocal.ACSaveChanges();
                if (msgSaveCharge != null)
                {
                    Messages.Msg(msgSaveCharge);
                    successSaveCharge = false;
                }
            }
            else if (localFC.NotAvailable)
            {
                // Restore
                // Restore -> Make charge available again
                ACMethodBooking bookReleaseStateFree = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ReleaseState_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                bookReleaseStateFree.MDReleaseState = MDReleaseState.DefaultMDReleaseState(dbLocal, MDReleaseState.ReleaseStates.Free);
                bookReleaseStateFree.PreventSendToRemoteStore = true;
                ACMethodEventArgs resultRelaseFree = ACFacilityManager.BookFacility(bookReleaseStateFree, dbLocal) as ACMethodEventArgs;
                if (!bookReleaseStateFree.ValidMessage.IsSucceded() || bookReleaseStateFree.ValidMessage.HasWarnings())
                    Messages.Msg(bookReleaseStateFree.ValidMessage);
                else if (resultRelaseFree.ResultState == Global.ACMethodResultState.Failed || resultRelaseFree.ResultState == Global.ACMethodResultState.Notpossible)
                {
                    if (String.IsNullOrEmpty(resultRelaseFree.ValidMessage.Message))
                        resultRelaseFree.ValidMessage.Message = resultRelaseFree.ResultState.ToString();
                    Messages.Msg(resultRelaseFree.ValidMessage);
                    successSaveCharge = false;
                }
            }

            if(successSaveCharge)
            {
                // TODO: Booking to Absolute stock??
                ACMethodBooking bookAbsolute = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_InwardMovement_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                bookAbsolute.QuantityIsAbsolute = true;
                bookAbsolute.InwardFacilityCharge = localFC;
                bookAbsolute.InwardTargetQuantity = changedRemoteFC.AvailableQuantity;
                bookAbsolute.InwardQuantity = changedRemoteFC.AvailableQuantity;
                bookAbsolute.PreventSendToRemoteStore = true;
                //Facility inwardFacility = dbLocal.Facility.FirstOrDefault(c=>c.FacilityID == changedRemoteFC.FacilityID);
                //bookAbsolute.InwardFacility = inwardFacility;

                ACMethodEventArgs resultAbsolute = ACFacilityManager.BookFacility(bookAbsolute, dbLocal) as ACMethodEventArgs;
                if (!bookAbsolute.ValidMessage.IsSucceded() || bookAbsolute.ValidMessage.HasWarnings())
                    Messages.Msg(bookAbsolute.ValidMessage);
                else if (resultAbsolute.ResultState == Global.ACMethodResultState.Failed || resultAbsolute.ResultState == Global.ACMethodResultState.Notpossible)
                {
                    if (String.IsNullOrEmpty(resultAbsolute.ValidMessage.Message))
                        resultAbsolute.ValidMessage.Message = resultAbsolute.ResultState.ToString();
                    Messages.Msg(resultAbsolute.ValidMessage);
                }
            }
            
        }

        private Facility SynchronizeFacility(DatabaseApp dbLocal, Facility remoteFacility, RSPDEntry addOrUpdateFacilityMD)
        {
            Facility localFacility = dbLocal.Facility
                                                                .Include(c => c.Facility1_ParentFacility)
                                                                .Where(c => c.FacilityID == addOrUpdateFacilityMD.KeyId)
                                                                .FirstOrDefault();
            if (localFacility == null)
            {
                Facility parentFacility = null;
                if (remoteFacility.ParentFacilityID.HasValue)
                    parentFacility = dbLocal.Facility.Where(c => c.FacilityID == remoteFacility.ParentFacilityID.Value).FirstOrDefault();
                localFacility = Facility.NewACObject(dbLocal, parentFacility);
                localFacility.CopyFrom(remoteFacility, false);
                localFacility.FacilityID = addOrUpdateFacilityMD.KeyId;
                localFacility.MDFacilityTypeID = remoteFacility.MDFacilityTypeID;
                localFacility.VBiStackCalculatorACClassID = remoteFacility.VBiStackCalculatorACClassID;
                dbLocal.Facility.AddObject(localFacility);
            }
            else
            {
                localFacility.CopyFrom(remoteFacility, false);
            }

            return localFacility;
        }

        private static void SynchronizePicking(DatabaseApp dbLocal, Picking remotePicking)
        {
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
            if (remotePicking.DeliveryCompanyAddress != null)
            {
                CompanyAddress companyAddress = dbLocal.CompanyAddress.FirstOrDefault(c => c.Company.KeyOfExtSys == remotePicking.DeliveryCompanyAddress.Company.KeyOfExtSys);
                remotePicking.DeliveryCompanyAddress = companyAddress;
            }
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
                if (remotePos.ToFacility != null)
                    localPos.FromFacility = dbLocal.Facility.FirstOrDefault(c => c.FacilityNo == remotePos.ToFacility.FacilityNo);
                else
                    localPos.FromFacility = null;
                if (remotePos.FromFacility != null)
                    localPos.ToFacility = dbLocal.Facility.FirstOrDefault(c => c.FacilityNo == remotePos.FromFacility.FacilityNo);
                else
                    localPos.ToFacility = null;
            }
        }

        #endregion

        protected virtual bool IsRemotePickingRequiredHere(string acUrlOfCaller, string remoteConnString, RemoteStorePostingData remoteStorePosting, DatabaseApp dbRemote, DatabaseApp dbLocal, Picking remotePicking)
        {
            return true;
        }

        protected virtual void OnHandleRemotePicking(string acUrlOfCaller, string remoteConnString, RemoteStorePostingData remoteStorePosting, DatabaseApp dbRemote, DatabaseApp dbLocal, Picking remotePicking)
        {
        }

        #endregion

        #region Material-Sync
        protected void SynchronizeMaterial(string acUrlOfCaller, string remoteConnString, RemoteStorePostingData remoteStorePosting)
        {
            try
            {
                using (DatabaseApp dbRemote = new DatabaseApp(remoteConnString))
                using (DatabaseApp dbLocal = new DatabaseApp())
                {

                    foreach (var entry in remoteStorePosting.FBIds)
                    {
                        Material[] remoteMaterials = new Material[] { };

                        // Synchronize all Materials
                        if (entry.KeyId == Guid.Empty)
                        {
                            DateTime lastSyncTime = DateTime.Now;
                            if (LastSyncTime != null)
                                lastSyncTime = LastSyncTime.Value;

                            remoteMaterials = dbRemote.Material.Where(c => c.UpdateDate > lastSyncTime).ToArray();
                        }
                        // Synchronize particular Materials
                        else
                        {
                            remoteMaterials = dbRemote.Material.Where(c => c.MaterialID == entry.KeyId).ToArray();
                        }

                        foreach (Material remoteMaterial in remoteMaterials)
                        {
                            Material localMaterial = dbLocal.Material.FirstOrDefault(c => c.MaterialID == remoteMaterial.MaterialID);
                            if (localMaterial == null)
                            {
                                localMaterial = Material.NewACObject(dbLocal, null);
                                dbLocal.Material.AddObject(localMaterial);
                                localMaterial.MaterialID = remoteMaterial.MaterialID;
                            }

                            localMaterial.CopyFrom(remoteMaterial, true);
                        }
                    }

                    LastSyncTime = DateTime.Now;
                    MsgWithDetails msgWithDetails = dbLocal.ACSaveChanges();
                    if (msgWithDetails != null)
                        Messages.LogMessageMsg(msgWithDetails);
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), "SynchronizeMaterial", e);
            }
        }
        #endregion


        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(RefreshFacility):
                    RefreshFacility((string)acParameter[0], (RemoteStorePostingData)acParameter[1]);
                    return true;
                case nameof(RefreshMaterial):
                    RefreshMaterial((string)acParameter[0], (RemoteStorePostingData)acParameter[1]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }

}
