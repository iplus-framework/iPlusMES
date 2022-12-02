using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.test
{
    public class RemoteFMHelper
    {

        public void SynchronizeFacility(IACComponent component, IMessages messages, ACPickingManager PickingManager, string remoteConnString, RemoteStorePostingData remoteStorePosting, bool syncRemoteStore = true, bool LoggingOn = true)
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

                                //if (remoteFBC.PickingPos != null && remoteFBC.InwardFacilityCharge != null && dbLocal.Facility.Any(c => c.FacilityID == remoteFBC.InwardFacilityID))
                                //{
                                //    if (!fbcForMirroringToPreBooking.Contains(remoteFBC))
                                //        fbcForMirroringToPreBooking.Add(remoteFBC);
                                //}
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
                        SynchronizeFacilityCharge(dbLocal, messages, changedRemoteFC);
                    }

                    foreach (Picking remotePicking in changedRemotePickings)
                    {
                        SynchronizePicking(dbLocal, PickingManager, remotePicking);
                    }

                    msgWithDetails = dbLocal.ACSaveChanges();
                    if (msgWithDetails != null)
                        messages.LogMessageMsg(msgWithDetails);

                    //if (MirroringBookingToPreBooking)
                    //{
                    //    foreach (FacilityBookingCharge fbcForMirroring in fbcForMirroringToPreBooking)
                    //    {
                    //        if (fbcForMirroring.InwardFacilityChargeID != null)
                    //            AssignMirroredPreBooking(dbLocal, fbcForMirroring);
                    //    }
                    //}

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

        private void SynchronizeFacilityCharge(DatabaseApp dbLocal, IMessages messages, FacilityCharge changedRemoteFC)
        {
            FacilityCharge localFC = null;

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
                localFC.NotAvailable= false;
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
            }
        }
    }
}
