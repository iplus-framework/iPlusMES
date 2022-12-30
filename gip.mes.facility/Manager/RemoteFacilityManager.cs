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

            _MirroringBookingToPreBooking = new ACPropertyConfigValue<bool>(this, "MirroringBookingToPreBooking", false);

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _PickingManager = ACPickingManager.ACRefToServiceInstance(this);
            if (_PickingManager == null)
                throw new Exception("PickingManager not configured");

            _DelegateQueue = new ACDelegateQueue(this.GetACUrl());
            _DelegateQueue.StartWorkerThread();
            return result;
        }

        public override bool ACPostInit()
        {
            bool postInitResult = base.ACPostInit();
            _ = MirroringBookingToPreBooking;
            return postInitResult;
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

        #region Configuration

        private ACPropertyConfigValue<bool> _MirroringBookingToPreBooking;
        [ACPropertyConfig("en{'Mirrorint stock movement to planned posting'}de{'Spiegelnde Lagerbewegung zu Geplante Buchung'}")]
        public bool MirroringBookingToPreBooking
        {
            get { return _MirroringBookingToPreBooking.ValueT; }
            set { _MirroringBookingToPreBooking.ValueT = value; }
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
        /// Source Property: 
        /// </summary>
        private DateTime _LastSyncTime;
        [ACPropertyInfo(true, 200, "LastSyncTime", "en{'LastSyncTime'}de{'LastSyncTime'}")]
        public DateTime LastSyncTime
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

        [ACPropertyInfo(true, 401, "", "en{'LoggingOn'}de{'LoggingOn'}", DefaultValue = false)]
        public bool LoggingOn { get; set; }

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

        [ACMethodInfo("", "en{'SynchronizeFacility2'}de{'SynchronizeFacility2'}", 1002)]
        public void SynchronizeFacility(string acUrlOfCaller, string remoteConnString, RemoteStorePostingData remoteStorePosting, bool syncRemoteStore = true)
        {
            try
            {
                if (remoteStorePosting == null || remoteStorePosting.FBIds == null || !remoteStorePosting.FBIds.Any())
                    return;

                if (LoggingOn)
                {
                    try
                    {
                        Messages.LogDebug(GetACUrl(), "SynchronizeFacility(10)", ACConvert.ObjectToXML(remoteStorePosting, true, true));
                    }
                    catch (Exception ex)
                    {
                        Messages.LogException(GetACUrl(), "SynchronizeFacility(20)", ex);
                    }
                }

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

                    if (syncRemoteStore)
                    {

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
                    }

                    //remoteStorePosting.FacilityUrlOfRecipient
                    List<FacilityCharge> changedRemoteFCs = new List<FacilityCharge>();
                    List<Picking> changedRemotePickings = new List<Picking>();
                    List<FacilityBookingCharge> fbcForMirroringToPreBooking = new List<FacilityBookingCharge>();
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
                                if (remoteFBC.InwardFacilityID.HasValue && dbLocal.Facility.Any(c => c.FacilityID == remoteFBC.InwardFacilityID))
                                {
                                    if (!changedRemoteFCs.Contains(remoteFBC.InwardFacilityCharge))
                                        changedRemoteFCs.Add(remoteFBC.InwardFacilityCharge);
                                }

                                if (remoteFBC.OutwardFacilityID.HasValue && dbLocal.Facility.Any(c => c.FacilityID == remoteFBC.OutwardFacilityID))
                                {
                                    if (!changedRemoteFCs.Contains(remoteFBC.OutwardFacilityCharge))
                                        changedRemoteFCs.Add(remoteFBC.OutwardFacilityCharge);
                                }

                                if (remoteFBC.PickingPos != null && MirroringBookingToPreBooking && remoteFBC.InwardFacilityCharge != null && dbLocal.Facility.Any(c => c.FacilityID == remoteFBC.InwardFacilityID))
                                {
                                    if (!fbcForMirroringToPreBooking.Contains(remoteFBC))
                                        fbcForMirroringToPreBooking.Add(remoteFBC);
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
                                else
                                {
                                    Messages.LogDebug(GetACUrl(), nameof(SynchronizeFacility), $"Remote picking is null: {entry.KeyId}! Local picking for delete not present to!");
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

                    if (MirroringBookingToPreBooking)
                    {
                        foreach (FacilityBookingCharge fbcForMirroring in fbcForMirroringToPreBooking)
                        {
                            if (fbcForMirroring.InwardFacilityChargeID != null)
                                AssignMirroredPreBooking(dbLocal, fbcForMirroring);
                        }
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

        private void AssignMirroredPreBooking(DatabaseApp dbLocal, FacilityBookingCharge fbcForMirroring)
        {
            PickingPos localPos = dbLocal.PickingPos.FirstOrDefault(c => c.PickingPosID == fbcForMirroring.PickingPosID);
            if (localPos != null && !localPos.FacilityBooking_PickingPos.Any())
            {
                // FacilityCharge
                FacilityCharge outwardFacilityCharge = null;
                if (fbcForMirroring.InwardFacilityChargeID != null)
                    outwardFacilityCharge = dbLocal.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == fbcForMirroring.InwardFacilityChargeID);

                FacilityPreBooking preBooking = null;
                if (outwardFacilityCharge != null)
                    preBooking = localPos.FacilityPreBooking_PickingPos.FirstOrDefault(c => c.InwardFacilityCharge != null && c.InwardFacilityCharge.FacilityChargeID == outwardFacilityCharge.FacilityChargeID);

                if (preBooking == null)
                {
                    preBooking = ACFacilityManager.NewFacilityPreBooking(dbLocal, localPos);
                    localPos.FacilityPreBooking_PickingPos.Add(preBooking);
                }

                ACMethodBooking acMethod = preBooking.ACMethodBooking as ACMethodBooking;
                acMethod.OutwardQuantity = fbcForMirroring.InwardQuantity;

                // FacilityCharge
                acMethod.OutwardFacilityCharge = outwardFacilityCharge;

                // Facility
                Facility facility = null;
                if (fbcForMirroring.InwardFacilityID != null)
                {
                    facility = dbLocal.Facility.FirstOrDefault(c => c.FacilityID == fbcForMirroring.InwardFacilityID);
                    acMethod.OutwardFacility = facility;
                }
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
                FacilityLot localLot = null;
                if (changedRemoteFC.FacilityLotID != null && changedRemoteFC.FacilityLotID != Guid.Empty)
                {
                    localLot = dbLocal.FacilityLot.FirstOrDefault(c => c.FacilityLotID == changedRemoteFC.FacilityLotID);
                    if (localLot == null)
                    {
                        localLot = changedRemoteFC.FacilityLot.Clone(true) as FacilityLot;
                        int countExisting = dbLocal.FacilityLot.Where(c => c.LotNo == changedRemoteFC.FacilityLot.LotNo).Count();
                        if (countExisting > 0)
                        {
                            localLot.LotNo = localLot.LotNo + string.Format(@"-{0}", countExisting);
                        }
                    }
                    else
                    {
                        localLot.ExpirationDate = changedRemoteFC.FacilityLot.ExpirationDate;
                        localLot.ExternLotNo = changedRemoteFC.FacilityLot.ExternLotNo;
                        localLot.ExternLotNo2 = changedRemoteFC.FacilityLot.ExternLotNo2;
                    }
                }

                // Add new FacilityCharge with same FacilityChargeID
                localFC = FacilityCharge.NewACObject(dbLocal, localLot);
                localFC.FacilityChargeID = changedRemoteFC.FacilityChargeID;
                localFC.CopyFrom(changedRemoteFC, true, false);
                dbLocal.FacilityCharge.AddObject(localFC);
                MsgWithDetails msgSaveCharge = dbLocal.ACSaveChanges();
                if (msgSaveCharge != null)
                {
                    Messages.LogMessageMsg(msgSaveCharge);
                    successSaveCharge = false;
                }
            }
            else
            {
                localFC.CopyFrom(changedRemoteFC, true, false);
                MsgWithDetails msgSaveCharge = dbLocal.ACSaveChanges();
                if (msgSaveCharge != null)
                {
                    Messages.Msg(msgSaveCharge);
                    successSaveCharge = false;
                }
            }

            if (localFC.NotAvailable)
            {
                // Restore -> Make charge available again
                ACMethodBooking bookReleaseStateFree = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ReleaseState_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                bookReleaseStateFree.MDReleaseState = MDReleaseState.DefaultMDReleaseState(dbLocal, MDReleaseState.ReleaseStates.Free);
                bookReleaseStateFree.PreventSendToRemoteStore = true;
                ACMethodEventArgs resultRelaseFree = ACFacilityManager.BookFacility(bookReleaseStateFree, dbLocal) as ACMethodEventArgs;
                if (!bookReleaseStateFree.ValidMessage.IsSucceded() || bookReleaseStateFree.ValidMessage.HasWarnings())
                    Messages.LogMessageMsg(bookReleaseStateFree.ValidMessage);
                else if (resultRelaseFree.ResultState == Global.ACMethodResultState.Failed || resultRelaseFree.ResultState == Global.ACMethodResultState.Notpossible)
                {
                    if (String.IsNullOrEmpty(resultRelaseFree.ValidMessage.Message))
                        resultRelaseFree.ValidMessage.Message = resultRelaseFree.ResultState.ToString();
                    Messages.LogMessageMsg(resultRelaseFree.ValidMessage);
                    successSaveCharge = false;
                }
            }

            if (successSaveCharge)
            {
                // Booking to absolute stock
                ACMethodBooking bookAbsolute = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_InwardMovement_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                bookAbsolute.QuantityIsAbsolute = true;
                bookAbsolute.InwardFacilityCharge = localFC;
                bookAbsolute.InwardTargetQuantity = changedRemoteFC.AvailableQuantity;
                bookAbsolute.InwardQuantity = changedRemoteFC.AvailableQuantity;
                bookAbsolute.PreventSendToRemoteStore = true;



                ACMethodEventArgs resultAbsolute = ACFacilityManager.BookFacility(bookAbsolute, dbLocal) as ACMethodEventArgs;
                if (!bookAbsolute.ValidMessage.IsSucceded() || bookAbsolute.ValidMessage.HasWarnings())
                    Messages.LogMessageMsg(bookAbsolute.ValidMessage);
                else if (resultAbsolute.ResultState == Global.ACMethodResultState.Failed || resultAbsolute.ResultState == Global.ACMethodResultState.Notpossible)
                {
                    if (String.IsNullOrEmpty(resultAbsolute.ValidMessage.Message))
                        resultAbsolute.ValidMessage.Message = resultAbsolute.ResultState.ToString();
                    Messages.LogMessageMsg(resultAbsolute.ValidMessage);
                }
            }

        }

        private Facility SynchronizeFacility(DatabaseApp dbLocal, Facility remoteFacility, RSPDEntry addOrUpdateFacilityMD)
        {
            Facility localFacility =
                dbLocal
                .Facility
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

        private void SynchronizePicking(DatabaseApp dbLocal, Picking remotePicking)
        {
            Picking localPicking = dbLocal.Picking.FirstOrDefault(c => c.PickingID == remotePicking.PickingID);
            if (localPicking == null)
            {
                localPicking = remotePicking.Clone(true) as Picking;
                dbLocal.Picking.AddObject(localPicking);
            }
            else
            {
                localPicking.CopyFrom(remotePicking, true, remotePicking.PickingNo);
            }

            // updating pos
            Guid[] remotePosIDs = remotePicking.PickingPos_Picking.Select(c => c.PickingPosID).ToArray();
            Guid[] localPosIDs = localPicking.PickingPos_Picking.Select(c => c.PickingPosID).ToArray();


            Guid[] localPosIDsForDelete = localPosIDs.Where(c => !remotePosIDs.Contains(c)).ToArray();
            foreach (Guid posID in localPosIDsForDelete)
            {
                PickingPos posForDelete = localPicking.PickingPos_Picking.Where(c => c.PickingPosID == posID).FirstOrDefault();
                PickingManager.UnassignPickingPos(posForDelete, dbLocal);
            }

            foreach (Guid posID in remotePosIDs)
            {
                PickingPos remotePos = remotePicking.PickingPos_Picking.FirstOrDefault(c => c.PickingPosID == posID);
                PickingPos localPos = localPicking.PickingPos_Picking.FirstOrDefault(c => c.PickingPosID == posID);
                if (localPos == null)
                {
                    localPos = remotePos.Clone(true) as PickingPos;
                    localPos.PickingActualUOM = 0;
                    localPicking.PickingPos_Picking.Add(localPos);
                }
                else
                    localPos.CopyFrom(remotePos, true, false);

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
                            DateTime lastSyncTime = new DateTime(2022, 1, 1);
                            if (LastSyncTime > DateTime.MinValue)
                                lastSyncTime = LastSyncTime;

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

                    MsgWithDetails msgWithDetails = dbLocal.ACSaveChanges();
                    if (msgWithDetails != null)
                        Messages.LogMessageMsg(msgWithDetails);
                    else
                        LastSyncTime = DateTime.Now;
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
