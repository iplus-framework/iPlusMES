using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace gip.mes.facility
{
    public partial class FacilityManager
    {

        #region Const

        public const string Const_Inventory_NotAllowedClosing = @"NotAllowedClosing";
        #endregion

        public string GetNewInventoryNo()
        {
            return Root.NoManager.GetNewNo(Database, typeof(FacilityInventory), nameof(FacilityInventory.FacilityInventoryNo), FacilityInventory.FormatNewNo, this);
        }

        public MsgWithDetails InventoryGenerate(string facilityInventoryNo, string facilityInventoryName,
            Guid? facilityID, bool generatePositions, bool omitGenerateSiloQuantPosition, Action<int, int> progressCallback)
        {
            MsgWithDetails msgWithDetails = null;

            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                FacilityInventory facilityInventory = FacilityInventory.NewACObject(databaseApp, null, facilityInventoryNo);
                facilityInventory.FacilityInventoryName = facilityInventoryName;
                facilityInventory.FacilityID = facilityID;
                if (generatePositions)
                {
                    InventoryGeneratePositions(databaseApp, facilityInventory, omitGenerateSiloQuantPosition, progressCallback);
                }
                msgWithDetails = databaseApp.ACSaveChanges();
            }

            return msgWithDetails;
        }


        public void InventoryGeneratePositions(DatabaseApp databaseApp, FacilityInventory facilityInventory, bool omitGenerateSiloQuantPosition, Action<int, int> progressCallback)
        {
            List<FacilityCharge> facilityCharges =
                    databaseApp
                    .FacilityCharge
                    .Where(c =>
                            !c.NotAvailable
                            && (!omitGenerateSiloQuantPosition || c.Facility.MDFacilityType.MDFacilityTypeIndex != (short)FacilityTypesEnum.StorageBinContainer)
                            && (facilityInventory.FacilityID == null
                                || c.FacilityID == facilityInventory.FacilityID
                                || (c.Facility.Facility1_ParentFacility != null
                                    && (c.Facility.Facility1_ParentFacility.FacilityID == facilityInventory.FacilityID
                                         || (c.Facility.Facility1_ParentFacility.Facility1_ParentFacility != null && c.Facility.Facility1_ParentFacility.Facility1_ParentFacility.FacilityID == facilityInventory.FacilityID))))
                    )
                    // TODO: @aagincic remove limit
                    // .Take(10)
                    .OrderBy(c => c.FacilityLot.LotNo)
                   .ToList();
            int count = facilityCharges.Count();
            if (progressCallback != null)
                progressCallback(0, count);
            List<FacilityInventoryPos> positions = new List<FacilityInventoryPos>();
            int nr = 0;
            foreach (FacilityCharge facilityCharge in facilityCharges)
            {
                nr++;
                FacilityInventoryPos inventoryPos = FacilityInventoryPos.NewACObject(databaseApp, facilityInventory);
                inventoryPos.FacilityCharge = facilityCharge;
                inventoryPos.StockQuantity = facilityCharge.StockQuantity;
                if (progressCallback != null)
                    progressCallback(nr, count);
            }
        }


        public MsgWithDetails InventoryClosing(string facilityInventoryNo, Action<int, int> progressCallback)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                try
                {
                    int nr = 0;

                    MDFacilityInventoryPosState postedInventoryPosState = databaseApp.MDFacilityInventoryPosState.FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.Posted);

                    FacilityInventory facilityInventory =
                        databaseApp
                        .FacilityInventory
                        .FirstOrDefault(c => c.FacilityInventoryNo == facilityInventoryNo);
                    int count = facilityInventory.FacilityInventoryPos_FacilityInventory.Count();
                    if (progressCallback != null)
                    {
                        progressCallback(nr, count);
                    }

                    List<FacilityInventoryPos> positions = facilityInventory.FacilityInventoryPos_FacilityInventory.ToList();

                    bool isNotAllowedClosing =
                        facilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex != (short)FacilityInventoryStateEnum.InProgress
                        ||
                         positions.Any(c => c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex < (short)FacilityInventoryPosStateEnum.Finished);
                    if (isNotAllowedClosing)
                    {
                        Msg msErrorNotAllowedClosing = new Msg() { MessageLevel = eMsgLevel.Error, ACIdentifier = Const_Inventory_NotAllowedClosing };
                        msgWithDetails.AddDetailMessage(msErrorNotAllowedClosing);
                    }
                    else
                    {
                        FixIsInfiniteStockPositions(positions);

                        List<FacilityInventoryPos> positionsWithNewQuantity =
                            positions
                            .Where(c =>
                                        c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.Finished
                                        &&
                                        (
                                            c.NotAvailable
                                            ||
                                            (
                                                c.NewStockQuantity != null
                                                && Math.Abs(c.StockQuantity - (c.NewStockQuantity ?? 0)) > FacilityConst.C_ZeroCompare
                                            )
                                        )
                                   )
                            .ToList();

                        count = positionsWithNewQuantity.Count;
                        progressCallback?.Invoke(nr, count);

                        foreach (FacilityInventoryPos facilityInventoryPos in positionsWithNewQuantity)
                        {
                            nr++;

                            if (!facilityInventoryPos.FacilityBooking_FacilityInventoryPos.Any())
                            {
                                bool? isQuantNotAvailable = null;
                                double? inwardTargetQuantity = null;
                                if (facilityInventoryPos.NotAvailable)
                                {
                                    isQuantNotAvailable = true;
                                }
                                else
                                {
                                    inwardTargetQuantity = (facilityInventoryPos.NewStockQuantity ?? 0) - facilityInventoryPos.StockQuantity;
                                }
                                MsgWithDetails bookingMsg = InventoryPosBooking(databaseApp,
                                                                             facilityInventoryPos,
                                                                             isQuantNotAvailable,
                                                                             inwardTargetQuantity,
                                                                             true);

                                if (bookingMsg != null && !bookingMsg.IsSucceded())
                                {
                                    msgWithDetails.AddDetailMessage(bookingMsg);
                                }

                                if (bookingMsg.IsSucceded() && facilityInventoryPos.MDFacilityInventoryPosStateID != postedInventoryPosState.MDFacilityInventoryPosStateID)
                                {
                                    facilityInventoryPos.MDFacilityInventoryPosState = postedInventoryPosState;
                                }
                            }
                            
                            progressCallback?.Invoke(nr, count);
                        }

                        List<FacilityInventoryPos> otherPositions = positions.Where(c => c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.Finished).ToList();
                        foreach (FacilityInventoryPos otherPos in otherPositions)
                        {
                            otherPos.MDFacilityInventoryPosState = postedInventoryPosState;
                        }

                        if (msgWithDetails.IsSucceded())
                        {
                            MDFacilityInventoryState finishedState = databaseApp.MDFacilityInventoryState.Where(c => c.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.Finished).FirstOrDefault();
                            facilityInventory.MDFacilityInventoryState = finishedState;
                            MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
                            if (saveMsg != null && !saveMsg.IsSucceded())
                            {
                                msgWithDetails.AddDetailMessage(saveMsg);
                                databaseApp.ACUndoChanges();
                            }
                        }
                    }
                }
                catch (Exception ec)
                {
                    string errMsg = "";
                    Exception tmpEc = ec;
                    while (tmpEc != null)
                    {
                        errMsg += tmpEc.Message;
                        tmpEc = tmpEc.InnerException;
                    }
                    msgWithDetails.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = errMsg });
                    databaseApp.ACUndoChanges();
                }
            }
            return msgWithDetails;
        }

        public MsgWithDetails CancelInventory(string facilityInventoryNo, Action<int, int> progressCallback)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                try
                {
                    int nr = 0;
                    MDFacilityInventoryPosState postedInventoryPosState = databaseApp.MDFacilityInventoryPosState.FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.Posted);
                    FacilityInventory facilityInventory = databaseApp.FacilityInventory.FirstOrDefault(c => c.FacilityInventoryNo == facilityInventoryNo);
                    FacilityInventoryPos[] facilityInventoryLines = facilityInventory.FacilityInventoryPos_FacilityInventory.ToArray();
                    int count = facilityInventoryLines.Length;

                    foreach (FacilityInventoryPos facilityInventoryPos in facilityInventoryLines)
                    {
                        nr++;

                        if (facilityInventoryPos.FacilityBooking_FacilityInventoryPos.Count() == 1)
                        {
                            FacilityBooking facilityBooking = facilityInventoryPos.FacilityBooking_FacilityInventoryPos.FirstOrDefault();

                            //MsgWithDetails bookingMsg =InventoryPosCanceling(databaseApp, postedInventoryPosState, facilityInventoryNo, facilityInventoryLine, nr, count, progressCallback);
                            bool? isQuantNotAvailable = null;
                            double? inwardTargetQuantity = null;
                            if (facilityInventoryPos.NotAvailable)
                            {
                                isQuantNotAvailable = false;
                            }
                            else
                            {
                                inwardTargetQuantity = -facilityBooking.InwardTargetQuantity;
                            }
                            MsgWithDetails bookingMsg = InventoryPosBooking(databaseApp,
                                                                         facilityInventoryPos,
                                                                         isQuantNotAvailable,
                                                                         inwardTargetQuantity,
                                                                         true);
                            if (bookingMsg != null && !bookingMsg.IsSucceded())
                            {
                                msgWithDetails.AddDetailMessage(bookingMsg);
                            }
                        }

                        progressCallback?.Invoke(nr, count);
                    }

                    MDFacilityInventoryState canceledInventoryState = databaseApp.MDFacilityInventoryState.FirstOrDefault(c => c.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.Canceled);
                    facilityInventory.MDFacilityInventoryState = canceledInventoryState;
                    databaseApp.ACSaveChanges();
                }
                catch (Exception ex)
                {
                    string errMsg = "";
                    Exception tmpEc = ex;
                    while (tmpEc != null)
                    {
                        errMsg += tmpEc.Message;
                        tmpEc = tmpEc.InnerException;
                    }
                    msgWithDetails.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = errMsg });
                    databaseApp.ACUndoChanges();
                }
            }

            return msgWithDetails;
        }


        /// <summary>
        /// Check if position is not available and Material.MDInventoryManagementType == InfiniteStock
        /// In case when Material.MDInventoryManagementType is changed later or on other side
        /// </summary>
        /// <param name="positions"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void FixIsInfiniteStockPositions(List<FacilityInventoryPos> positions)
        {
            foreach (FacilityInventoryPos pos in positions)
            {
                if (pos.IsInfiniteStock && pos.NotAvailable)
                {
                    pos.NotAvailable = false;
                    pos.NewStockQuantity = pos.StockQuantity;
                }
            }
        }

        /// <summary>
        /// Common mehtod for booking On InventoryPos
        /// </summary>
        /// <param name="databaseApp"></param>
        /// <param name="facilityCharge"></param>
        /// <param name="isQuantNotAvailable"> is true when on inventory is quant marked as not available more, in other case when we make inventory canceling, canceled quants is bringed back to life</param>
        /// <returns></returns>
        private MsgWithDetails InventoryPosBooking(DatabaseApp databaseApp, FacilityInventoryPos facilityInventoryPos, bool? isQuantNotAvailable, double? inwardTargetQuantity, bool isClosing)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            ACMethodBooking aCMethodBooking = null;
            if (isQuantNotAvailable != null)
            {
                // common params for charge availability booking
                aCMethodBooking = ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_FacilityCharge, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                aCMethodBooking.BookingType = GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge;
                aCMethodBooking.MDBalancingMode = DatabaseApp.s_cQry_GetMDBalancingMode(databaseApp, MDBalancingMode.BalancingModes.InwardOff_OutwardOff).FirstOrDefault();

                if (isQuantNotAvailable ?? false)
                {
                    aCMethodBooking.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(databaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
                    facilityInventoryPos.FacilityCharge.NotAvailable = true;
                }
                else
                {
                    aCMethodBooking.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(databaseApp, MDZeroStockState.ZeroStockStates.RestoreQuantityIfNotAvailable);

                    FacilityBookingCharge fbc =
                        facilityInventoryPos
                        .FacilityCharge
                        .FacilityBookingCharge_InwardFacilityCharge
                        .Where(c =>
                                    c.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge
                                    && Math.Abs(c.InwardQuantity) >= double.Epsilon
                        )
                        .OrderByDescending(c => c.InsertDate)
                        .FirstOrDefault();

                    if (fbc == null)
                    {
                        aCMethodBooking.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(databaseApp, MDZeroStockState.ZeroStockStates.ResetIfNotAvailable);
                    }
                }
            }
            else
            {
                // Summary inventory booking: correction, canceling
                aCMethodBooking = ACUrlACTypeSignature("!" + GlobalApp.FBT_StockCorrection, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                aCMethodBooking.BookingType = GlobalApp.FacilityBookingType.StockCorrection;
                aCMethodBooking.InwardFacilityCharge = facilityInventoryPos.FacilityCharge;
                aCMethodBooking.MDBalancingMode = DatabaseApp.s_cQry_GetMDBalancingMode(databaseApp, MDBalancingMode.BalancingModes.InwardOff_OutwardOff).FirstOrDefault();
                aCMethodBooking.InwardTargetQuantity = inwardTargetQuantity;
                aCMethodBooking.InwardQuantity = inwardTargetQuantity;
                if (facilityInventoryPos.FacilityCharge.MDUnit != null && facilityInventoryPos.FacilityCharge.Material.BaseMDUnit != facilityInventoryPos.FacilityCharge.MDUnit)
                {
                    double convertedQuantity = facilityInventoryPos.FacilityCharge.Material.ConvertQuantity(inwardTargetQuantity ?? 0, facilityInventoryPos.FacilityCharge.Material.BaseMDUnit, facilityInventoryPos.FacilityCharge.MDUnit);
                    aCMethodBooking.InwardTargetQuantity = convertedQuantity;
                    aCMethodBooking.InwardQuantity = convertedQuantity;
                }
            }

            aCMethodBooking.InwardFacilityCharge = facilityInventoryPos.FacilityCharge;
            aCMethodBooking.FacilityInventoryPos = facilityInventoryPos;
            ACMethodEventArgs result = BookFacilityWithRetry(ref aCMethodBooking, databaseApp, true) as ACMethodEventArgs;
            if (!aCMethodBooking.ValidMessage.IsSucceded() || aCMethodBooking.ValidMessage.HasWarnings())
            {
                string msg = $"[InventoryBooking] Error {(isClosing ? "closing" : "canceling")}  FacilityInventoryNo: {0}, Position (FacilityChargeID:{1}) {2} {3}, LotNo:{4}";
                string lotNo = "-";
                if (facilityInventoryPos.FacilityCharge.FacilityLot != null)
                {
                    lotNo = facilityInventoryPos.FacilityCharge.FacilityLot.LotNo;
                }
                msg = string.Format(msg, facilityInventoryPos.FacilityInventory.FacilityInventoryNo, facilityInventoryPos.FacilityInventoryPosID, facilityInventoryPos.FacilityCharge.Material.MaterialNo,
                    facilityInventoryPos.FacilityCharge.Material.MaterialName1, lotNo);
                Root.Messages.LogError("FacilityManager", "InventoryClosing", msg);
                msgWithDetails.AddDetailMessage(new Msg() { Message = msg, MessageLevel = eMsgLevel.Error });
                msgWithDetails.AddDetailMessage(aCMethodBooking.ValidMessage);
            }


            MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
            if (saveMsg != null && !saveMsg.IsSucceded())
            {
                msgWithDetails.AddDetailMessage(saveMsg);
                databaseApp.ACUndoChanges();
            }
            return msgWithDetails;
        }
    }
}
