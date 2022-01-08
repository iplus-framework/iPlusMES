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
                        }
                        else if (localFC.NotAvailable)
                        {
                            // Restore
                        }
                        // TODO: Booking to Absolute stock
                    }

                    foreach (Picking remotePicking in changedRemotePickings)
                    {
                        // TODO:
                    }

                    //Material m1 = dbRemote.Material.FirstOrDefault();
                    //Material m2 = dbLocal.Material.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), "SynchronizeFacility", e);
            }
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
