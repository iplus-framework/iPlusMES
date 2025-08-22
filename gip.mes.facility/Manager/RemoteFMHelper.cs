using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using gip.core.autocomponent;
using System.ComponentModel;
using static gip.mes.datamodel.GlobalApp;

namespace gip.mes.facility
{

    /// <summary>
    /// TODO: @aagincic: Will be deleted after some comparasion with RemoteFacilityManager
    /// </summary>
    public class RemoteFMHelper
    {
        #region const
        public static string PickingFromRemoteNotPresentInCurrent = @"
declare @mdPickingType varchar(10);
set @mdPickingType = '{mdPickingType}';

select 
    ROW_NUMBER() OVER (ORDER BY pp.PickingNo) RowNr,
    pp.PickingNo,
    pp.DeliveryDateFrom,
    pp.InsertDate,
    pp.InsertName,
    (select MIN(pos.InsertDate) 
     from [{RemoteDBName}].[dbo].[PickingPos] pos 
     where pos.PickingID = pp.PickingID) as PosStartUpdate,
    (select MAX(pos.UpdateDate) 
     from [{RemoteDBName}].[dbo].[PickingPos] pos 
     where pos.PickingID = pp.PickingID) as PosEndUpdate,
    (select MIN(fb.InsertDate) 
     from [{RemoteDBName}].[dbo].[FacilityBooking] fb 
     inner join [{RemoteDBName}].[dbo].[PickingPos] pos 
     on fb.PickingPosID = pos.PickingPosID 
     where pos.PickingID = pp.PickingID) as FBStartUpdate,
    (select MAX(fb.InsertDate) 
     from [{RemoteDBName}].[dbo].[FacilityBooking] fb 
     inner join [{RemoteDBName}].[dbo].[PickingPos] pos 
     on fb.PickingPosID = pos.PickingPosID 
     where pos.PickingID = pp.PickingID) as FBEndUpdate,
    cast(0 as bit) as IsSuccessfullyClosed 
from 
    [{RemoteDBName}].[dbo].[Picking] pp 
inner join 
    [{RemoteDBName}].[dbo].[MDPickingType] pt 
    on pt.MDPickingTypeID = pp.MDPickingTypeID 
where 
    pt.MDKey = @mdPickingType 
    and pp.PickingID not in (select PickingID from [{LocalDBName}].[dbo].[Picking]);
";
        #endregion

        #region Methods

        public void SynchronizeFacility(IACComponent component, IMessages messages, FacilityManager facilityManager, ACPickingManager pickingManager, string remoteConnString, RemoteStorePostingData remoteStorePosting, bool syncRemoteStore = true, bool LoggingOn = true)
        {
            try
            {
                if (remoteStorePosting == null || remoteStorePosting.FBIds == null || !remoteStorePosting.FBIds.Any())
                    return;

                if (LoggingOn)
                {
                    try
                    {
                        messages.LogDebug(component.GetACUrl(), "SynchronizeFacility(10)", ACConvert.ObjectToXML(remoteStorePosting, true, true));
                    }
                    catch (Exception ex)
                    {
                        messages.LogException(component.GetACUrl(), "SynchronizeFacility(20)", ex);
                    }
                }

                MsgWithDetails msgWithDetails = null;
                using (DatabaseApp dbRemote = new DatabaseApp(remoteConnString))
                using (DatabaseApp dbLocal = new DatabaseApp())
                {
                    RSPDEntry addOrUpdateFacilityMD = remoteStorePosting.FBIds.FirstOrDefault(c => c.EntityType == Facility.ClassName);


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

                                if (remoteFBC.PickingPos != null && remoteFBC.InwardFacilityCharge != null && dbLocal.Facility.Any(c => c.FacilityID == remoteFBC.InwardFacilityID))
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
                                        pickingManager.UnassignPickingPos(localPickingPos, dbLocal);

                                    localPicking.DeleteACObject(dbLocal, false);
                                }
                            }
                        }
                    }

                    foreach (FacilityCharge changedRemoteFC in changedRemoteFCs)
                    {
                        SynchronizeFacilityCharge(dbLocal, messages, changedRemoteFC);
                    }

                    foreach (Picking remotePicking in changedRemotePickings)
                    {
                        SynchronizePicking(dbLocal, pickingManager, remotePicking);
                    }

                    msgWithDetails = dbLocal.ACSaveChanges();
                    if (msgWithDetails != null)
                        messages.LogMessageMsg(msgWithDetails);

                    foreach (FacilityBookingCharge fbcForMirroring in fbcForMirroringToPreBooking)
                    {
                        if (fbcForMirroring.InwardFacilityChargeID != null)
                            AssignMirroredPreBooking(dbLocal, facilityManager, fbcForMirroring);
                    }

                    msgWithDetails = dbLocal.ACSaveChanges();
                    if (msgWithDetails != null)
                        messages.LogMessageMsg(msgWithDetails);
                }
            }
            catch (Exception e)
            {
                messages.LogException(component.GetACUrl(), "SynchronizeFacility", e);
            }
        }

        public RemoteStorePostingData GetRemoteStorePostingData(string pickingNo, string remoteConnString)
        {
            RemoteStorePostingData remoteStorePostingData = null;

            Picking picking = null;

            using (DatabaseApp remoteDbApp = new DatabaseApp(remoteConnString))
            {
                picking = remoteDbApp.Picking.Where(c => c.PickingNo == pickingNo).FirstOrDefault();
                if (picking != null)
                {
                    remoteStorePostingData = new RemoteStorePostingData();
                    Guid[] faciltiyBookingIDs = picking.PickingPos_Picking.SelectMany(c => c.FacilityBooking_PickingPos).Select(c => c.FacilityBookingID).ToArray();
                    Guid facilityID = Guid.Empty;
                    foreach (PickingPos pickingPos in picking.PickingPos_Picking.ToArray())
                    {
                        if (pickingPos.FromFacility != null)
                        {
                            facilityID = pickingPos.FromFacility.FacilityID;
                            break;
                        }
                        if (pickingPos.ToFacility != null)
                        {
                            facilityID = pickingPos.ToFacility.FacilityID;
                            break;
                        }
                    }

                    remoteStorePostingData.FBIds.Add(
                        new RSPDEntry()
                        {
                            EntityType = nameof(Picking),
                            KeyId = picking.PickingID
                        });

                    foreach (Guid id in faciltiyBookingIDs)
                    {
                        remoteStorePostingData.FBIds.Add(
                        new RSPDEntry()
                        {
                            EntityType = nameof(FacilityBooking),
                            KeyId = id
                        });
                    }
                }
            }
            return remoteStorePostingData;
        }

        public List<RemotePickingInfo> GetRemotePickingInfos(DatabaseApp databaseApp, ACComponent aCComponent, string mdPickingType, string remoteConnString)
        {
            string localDatabaseName = GetDatabaseNameFromConnectionString(databaseApp.Connection.ConnectionString);
            string remoteDatabaseName = GetDatabaseNameFromConnectionString(remoteConnString);

            string sql = PickingFromRemoteNotPresentInCurrent;
            sql = sql.Replace("{mdPickingType}", mdPickingType);
            sql = sql.Replace("{LocalDBName}", localDatabaseName);
            sql = sql.Replace("{RemoteDBName}", remoteDatabaseName);

            List<RemotePickingInfo> remotePickingInfos = databaseApp.ExecuteStoreQuery<RemotePickingInfo>(sql).ToList<RemotePickingInfo>();
            return remotePickingInfos;
        }

        public Msg GetSyncMissingPickings(ACComponent aCComponent, string remoteConnString, string[] missingPickings)
        {
            Msg msg = null;
            FacilityManager facilityManager = FacilityManager.GetServiceInstance(ACRoot.SRoot) as FacilityManager;
            ACPickingManager aCPickingManager = ACRoot.SRoot.ACUrlCommand("\\LocalServiceObjects\\PickingManager") as ACPickingManager;
            if (string.IsNullOrEmpty(remoteConnString) || facilityManager == null || aCPickingManager == null)
            {
                string message = "";
                if (string.IsNullOrEmpty(remoteConnString))
                {
                    message += "remoteConnString  is not set!";
                }
                if (facilityManager == null)
                {
                    message += "FacilityManager is not set!";
                }
                if (facilityManager == null)
                {
                    message += "FacilityManager is not set!";
                }
                msg = new Msg(eMsgLevel.Error, message);
            }
            else
            {
                RemoteFMHelper fm = new RemoteFMHelper();
                List<RemotePickingInfo> result = new List<RemotePickingInfo>();
                foreach (string missingPicking in missingPickings)
                {
                    RemoteStorePostingData remoteStorePostingData = fm.GetRemoteStorePostingData(missingPicking, remoteConnString);
                    Console.WriteLine($"Sync {missingPicking}..");
                    fm.SynchronizeFacility(aCComponent, aCComponent.Messages, facilityManager, aCPickingManager, remoteConnString, remoteStorePostingData);
                }
            }
            return msg;
        }


        public List<RemotePickingInfo> CloseMissingPickings(DatabaseApp databaseApp, List<RemotePickingInfo> missingPickings)
        {
            foreach (RemotePickingInfo missingPicking in missingPickings)
            {
                missingPicking.IsSuccessfullyClosed = false;
                ClosePicking(databaseApp, missingPicking.PickingNo, true);
            }
            return missingPickings;
        }

        public (bool? success, string message) FixPickingPreBookings(DatabaseApp databaseApp, string pickingNo)
        {
            bool? success = null;
            string message = "";

            try
            {
                Picking picking = databaseApp.Picking.Where(c => c.PickingNo == pickingNo).FirstOrDefault();
                if (picking != null)
                {
                    PickingPos[] pickingPositions = picking.PickingPos_Picking.ToArray();
                    foreach (PickingPos position in pickingPositions)
                    {
                        if (position.FacilityPreBooking_PickingPos.Any())
                        {
                            double missingQuantity = position.TargetQuantityUOM - position.ActualQuantityUOM;
                            double preBookingQuantity = position.FacilityPreBooking_PickingPos.Select(c => (c.OutwardQuantity ?? 0)).Sum();
                            if (Math.Abs(missingQuantity - preBookingQuantity) < (position.TargetQuantityUOM * 0.1))
                            {
                                FacilityPreBooking[] preBookings = position.FacilityPreBooking_PickingPos.ToArray();
                                foreach (FacilityPreBooking preBooking in preBookings)
                                {
                                    if (preBooking.InwardFacility == null && position.PickingMaterialID != null)
                                    {
                                        success = false;
                                        FacilityBooking[] bookings =
                                            databaseApp
                                            .FacilityBooking
                                            .Where(c =>
                                                        c.PickingPosID != null
                                                        && c.PickingPos.PickingMaterialID != null
                                                        && c.PickingPos.PickingMaterialID == position.PickingMaterialID
                                                        && c.InwardFacilityID != null)
                                            .OrderByDescending(c => c.InsertDate)
                                            .Take(5)
                                            .ToArray();
                                        if (bookings.Any())
                                        {
                                            Dictionary<string, int> facilityOccurance =
                                                bookings
                                                .GroupBy(c => c.InwardFacility.FacilityNo)
                                                .ToDictionary(g => g.Key, g => g.Count());

                                            string facilityNo = facilityOccurance.OrderByDescending(c => c.Value).FirstOrDefault().Key;
                                            Facility inwardFacility = databaseApp.Facility.FirstOrDefault(c => c.FacilityNo == facilityNo);
                                            if (inwardFacility != null)
                                            {
                                                ACMethodBooking aCMethodBooking = preBooking.ACMethodBooking as ACMethodBooking;
                                                aCMethodBooking.InwardFacility = inwardFacility;
                                                preBooking.ACMethodBooking = aCMethodBooking;
                                                databaseApp.ACSaveChanges();

                                                message += System.Environment.NewLine;
                                                message += $"#{position.Sequence} {position.Material.MaterialNo} {position.Material.MaterialName1}| => {inwardFacility.FacilityNo}";
                                            }
                                        }
                                    }
                                }
                                success = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                success = false;
            }

            return (success, message);
        }

        public bool? ClosePicking(DatabaseApp databaseApp, string pickingNo, bool checkInwardStorageBin)
        {
            bool? success = null;
            FacilityManager facilityManager = FacilityManager.GetServiceInstance(ACRoot.SRoot) as FacilityManager;
            ACPickingManager aCPickingManager = ACRoot.SRoot.ACUrlCommand("\\LocalServiceObjects\\PickingManager") as ACPickingManager;


            Picking picking = databaseApp.Picking.Where(c => c.PickingNo == pickingNo).FirstOrDefault();
            if (picking != null)
            {
                PickingPos[] pickingPositions = picking.PickingPos_Picking.ToArray();
                bool saveChanges = false;
                foreach (PickingPos position in pickingPositions)
                {
                    if (position.FacilityPreBooking_PickingPos.Any())
                    {
                        double missingQuantity = position.TargetQuantityUOM - position.ActualQuantityUOM;
                        double preBookingQuantity = position.FacilityPreBooking_PickingPos.Select(c => (c.OutwardQuantity ?? 0)).Sum();
                        if (Math.Abs(missingQuantity - preBookingQuantity) < (position.TargetQuantityUOM * 0.1))
                        {
                            saveChanges = true || saveChanges;
                            FacilityPreBooking[] preBookings = position.FacilityPreBooking_PickingPos.ToArray();
                            foreach (FacilityPreBooking preBooking in preBookings)
                            {
                                if (checkInwardStorageBin && preBooking.InwardFacility == null)
                                {
                                    success = false;
                                }
                                else
                                {
                                    ACMethodBooking aCMethodBooking = preBooking.ACMethodBooking as ACMethodBooking;
                                    ACMethodEventArgs resultBooking = facilityManager.BookFacility(aCMethodBooking, databaseApp) as ACMethodEventArgs;
                                    if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                                    {
                                        success = false;
                                    }
                                    else
                                    {
                                        preBooking.DeleteACObject(databaseApp, false);

                                        double changedQuantity = 0;
                                        FacilityCharge outwardFC = null;

                                        if (aCMethodBooking != null)
                                        {
                                            if (aCMethodBooking.OutwardQuantity.HasValue)
                                                changedQuantity = aCMethodBooking.OutwardQuantity.Value;
                                            else if (aCMethodBooking.InwardQuantity.HasValue)
                                                changedQuantity = aCMethodBooking.InwardQuantity.Value;

                                            outwardFC = aCMethodBooking.OutwardFacilityCharge;
                                        }
                                        bool isCancellation = aCMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel || aCMethodBooking.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel;
                                        facilityManager.RecalcAfterPosting(databaseApp, position, changedQuantity, isCancellation, true);
                                        databaseApp.ACSaveChanges();

                                        bool isStockConsumed = outwardFC != null && outwardFC.StockQuantity <= 0.001;

                                        if (isStockConsumed)
                                        {
                                            ACMethodBooking fbtZeroBooking = aCPickingManager.BookParamZeroStockFacilityChargeClone(facilityManager, databaseApp);
                                            ACMethodBooking fbtZeroBookingClone = fbtZeroBooking.Clone() as ACMethodBooking;

                                            fbtZeroBookingClone.InwardFacilityCharge = outwardFC;
                                            fbtZeroBookingClone.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(databaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);

                                            fbtZeroBookingClone.AutoRefresh = true;
                                            ACMethodEventArgs resultZeroBook = facilityManager.BookFacility(fbtZeroBookingClone, databaseApp);
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                if (saveChanges)
                {
                    MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
                    success = (success ?? true) && (saveMsg == null || saveMsg.IsSucceded());
                }
            }
            return success;
        }

        public string GetRemoteFacilityManagerConnectionString(ACComponent remoteFacilityManager)
        {
            List<string> hierarchy = ACUrlHelper.ResolveParents(remoteFacilityManager.ACUrl);
            string remoteProxyACurl = hierarchy.FirstOrDefault();
            if (String.IsNullOrEmpty(remoteProxyACurl))
                return null;
            ACComponent remoteAppManager = remoteFacilityManager.Root.ACUrlCommand(remoteProxyACurl) as ACComponent;
            if (remoteAppManager == null)
                return null;
            return remoteAppManager[nameof(RemoteAppManager.RemoteConnString)] as string;
        }

        #endregion

        #region Private methods

        private void SynchronizeFacilityCharge(DatabaseApp dbLocal, IMessages messages, FacilityCharge changedRemoteFC)
        {
            FacilityCharge localFC = null;

            // sync material
            Material localMaterial = dbLocal.Material.FirstOrDefault(c => c.MaterialID == changedRemoteFC.MaterialID);
            if (localMaterial == null)
            {
                localMaterial = Material.NewACObject(dbLocal, null);
                localMaterial.MaterialID = changedRemoteFC.MaterialID;
                localMaterial.CopyFrom(changedRemoteFC.Material, false);
                dbLocal.Material.AddObject(localMaterial);
            }

            // Search charge with same ID
            localFC = dbLocal.FacilityCharge.Where(c => c.FacilityChargeID == changedRemoteFC.FacilityChargeID).FirstOrDefault();

            //bool successSaveCharge = true;
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
                localFC.NotAvailable = false;
                dbLocal.FacilityCharge.AddObject(localFC);
                MsgWithDetails msgSaveCharge = dbLocal.ACSaveChanges();
                if (msgSaveCharge != null)
                {
                    messages.LogMessageMsg(msgSaveCharge);
                    //successSaveCharge = false;
                }
            }
            else
            {
                localFC.CopyFrom(changedRemoteFC, true, false);
                localFC.NotAvailable = false;
                MsgWithDetails msgSaveCharge = dbLocal.ACSaveChanges();
                if (msgSaveCharge != null)
                {
                    messages.Msg(msgSaveCharge);
                    //successSaveCharge = false;
                }
            }

            //if (localFC.NotAvailable)
            //{
            //    // Restore -> Make charge available again
            //    ACMethodBooking bookReleaseStateFree = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ReleaseState_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            //    bookReleaseStateFree.MDReleaseState = MDReleaseState.DefaultMDReleaseState(dbLocal, MDReleaseState.ReleaseStates.Free);
            //    bookReleaseStateFree.PreventSendToRemoteStore = true;
            //    ACMethodEventArgs resultRelaseFree = ACFacilityManager.BookFacility(bookReleaseStateFree, dbLocal) as ACMethodEventArgs;
            //    if (!bookReleaseStateFree.ValidMessage.IsSucceded() || bookReleaseStateFree.ValidMessage.HasWarnings())
            //        messages.LogMessageMsg(bookReleaseStateFree.ValidMessage);
            //    else if (resultRelaseFree.ResultState == Global.ACMethodResultState.Failed || resultRelaseFree.ResultState == Global.ACMethodResultState.Notpossible)
            //    {
            //        if (String.IsNullOrEmpty(resultRelaseFree.ValidMessage.Message))
            //            resultRelaseFree.ValidMessage.Message = resultRelaseFree.ResultState.ToString();
            //        messages.LogMessageMsg(resultRelaseFree.ValidMessage);
            //        successSaveCharge = false;
            //    }
            //}

            //if (successSaveCharge)
            //{
            //    // Booking to absolute stock
            //    ACMethodBooking bookAbsolute = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_InwardMovement_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            //    bookAbsolute.QuantityIsAbsolute = true;
            //    bookAbsolute.InwardFacilityCharge = localFC;
            //    bookAbsolute.InwardTargetQuantity = changedRemoteFC.AvailableQuantity;
            //    bookAbsolute.InwardQuantity = changedRemoteFC.AvailableQuantity;
            //    bookAbsolute.PreventSendToRemoteStore = true;



            //    ACMethodEventArgs resultAbsolute = ACFacilityManager.BookFacility(bookAbsolute, dbLocal) as ACMethodEventArgs;
            //    if (!bookAbsolute.ValidMessage.IsSucceded() || bookAbsolute.ValidMessage.HasWarnings())
            //        messages.LogMessageMsg(bookAbsolute.ValidMessage);
            //    else if (resultAbsolute.ResultState == Global.ACMethodResultState.Failed || resultAbsolute.ResultState == Global.ACMethodResultState.Notpossible)
            //    {
            //        if (String.IsNullOrEmpty(resultAbsolute.ValidMessage.Message))
            //            resultAbsolute.ValidMessage.Message = resultAbsolute.ResultState.ToString();
            //        messages.LogMessageMsg(resultAbsolute.ValidMessage);
            //    }
            //}

        }

        private void SynchronizePicking(DatabaseApp dbLocal, ACPickingManager PickingManager, Picking remotePicking)
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

                // make proposition to facility
                if (localPos.ToFacility == null && remotePos.PickingMaterialID != null)
                {
                    PickingPos samplePickingPos = dbLocal.PickingPos.Where(c => c.PickingMaterialID != null && c.PickingMaterialID == remotePos.PickingMaterialID && c.ToFacilityID != null).FirstOrDefault();
                    if (samplePickingPos != null)
                    {
                        localPos.ToFacility = samplePickingPos.ToFacility;
                    }
                }
            }
        }

        private void AssignMirroredPreBooking(DatabaseApp dbLocal, FacilityManager facilityManager, FacilityBookingCharge fbcForMirroring)
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
                    preBooking = facilityManager.NewFacilityPreBooking(dbLocal, localPos);
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

        private string GetDatabaseNameFromConnectionString(string connectionString)
        {
            string tmpConnectionString = connectionString;
            string searchValue = "connection string=\"";
            int indexOf = tmpConnectionString.IndexOf(searchValue);
            indexOf = indexOf + searchValue.Length;
            tmpConnectionString = tmpConnectionString.Substring(indexOf);
            indexOf = tmpConnectionString.IndexOf("MultipleActiveResult");
            tmpConnectionString = tmpConnectionString.Substring(0, indexOf);
            IDbConnection connection = new SqlConnection(tmpConnectionString);
            var dbName = connection.Database;
            return dbName;
        }


        #endregion
    }
}
