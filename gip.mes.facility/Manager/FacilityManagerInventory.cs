using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public partial class FacilityManager
    {

        #region Const

        public const string Const_Inventory_NotAllowedClosing = @"NotAllowedClosing";
        #endregion

        public string GetNewInventoryNo()
        {
            return Root.NoManager.GetNewNo(Database, typeof(FacilityInventory), FacilityInventory.NoColumnName, FacilityInventory.FormatNewNo, this);
        }

        public MsgWithDetails InventoryGenerate(string facilityInventoryNo, string facilityInventoryName, string facilityNo, Action<int, int> progressCallback)
        {
            MsgWithDetails msgWithDetails = null;

            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                FacilityInventory facilityInventory = FacilityInventory.NewACObject(databaseApp, null, facilityInventoryNo);
                facilityInventory.FacilityInventoryName = facilityInventoryName;

                List<FacilityCharge> facilityCharges =
                    databaseApp
                    .FacilityCharge
                    .Where(c => 
                            !c.NotAvailable
                            && (facilityNo == null || c.Facility.FacilityNo == facilityNo)
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
                msgWithDetails = databaseApp.ACSaveChanges();
            }

            return msgWithDetails;
        }


        public MsgWithDetails InventoryClosing(string facilityInventoryNo, Action<int, int> progressCallback)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            try
            {
                int nr = 0;
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    FacilityInventory facilityInventory = databaseApp.FacilityInventory.FirstOrDefault(c => c.FacilityInventoryNo == facilityInventoryNo);
                    int count = facilityInventory.FacilityInventoryPos_FacilityInventory.Count();
                    if (progressCallback != null)
                        progressCallback(nr, count);

                    List<FacilityInventoryPos> positions = facilityInventory.FacilityInventoryPos_FacilityInventory.ToList();


                    bool isNotAllowedClosing =
                        facilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex != (short)MDFacilityInventoryState.FacilityInventoryStates.InProgress
                        ||
                         positions.Any(c => c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex != (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished);
                    if (isNotAllowedClosing)
                    {
                        Msg msErrorNotAllowedClosing = new Msg() { MessageLevel = eMsgLevel.Error, ACIdentifier = Const_Inventory_NotAllowedClosing };
                        msgWithDetails.AddDetailMessage(msErrorNotAllowedClosing);
                    }
                    else
                    {
                        List<FacilityInventoryPos> positionsWithNewQuantity = positions
                            .Where(c =>
                            c.NotAvailable
                            ||
                            (
                                c.NewStockQuantity != null
                                && (Math.Abs(c.StockQuantity - (c.NewStockQuantity ?? 0)) > Double.Epsilon)
                            )
                        ).ToList();
                        foreach (FacilityInventoryPos facilityInventoryPos in positionsWithNewQuantity)
                        {
                            nr++;
                            ACMethodBooking aCMethodBooking = null;
                            if (facilityInventoryPos.NotAvailable)
                            {
                                aCMethodBooking = ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_FacilityCharge, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                                aCMethodBooking.BookingType = GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge;
                                aCMethodBooking.MDBalancingMode = DatabaseApp.s_cQry_GetMDBalancingMode(databaseApp, MDBalancingMode.BalancingModes.InwardOff_OutwardOff).FirstOrDefault();
                                aCMethodBooking.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(databaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
                                facilityInventoryPos.FacilityCharge.NotAvailable = true;
                                ACSaveChanges();
                            }
                            else
                            {
                                aCMethodBooking = ACUrlACTypeSignature("!" + GlobalApp.FBT_StockCorrection, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                                aCMethodBooking.BookingType = GlobalApp.FacilityBookingType.StockCorrection;
                                aCMethodBooking.InwardFacilityCharge = facilityInventoryPos.FacilityCharge;
                                aCMethodBooking.MDBalancingMode = DatabaseApp.s_cQry_GetMDBalancingMode(databaseApp, MDBalancingMode.BalancingModes.InwardOff_OutwardOff).FirstOrDefault();
                                aCMethodBooking.InwardTargetQuantity = facilityInventoryPos.NewStockQuantity - facilityInventoryPos.StockQuantity;
                                aCMethodBooking.InwardQuantity = facilityInventoryPos.NewStockQuantity - facilityInventoryPos.StockQuantity;
                                if (   facilityInventoryPos.FacilityCharge.MDUnit != null 
                                    && facilityInventoryPos.FacilityCharge.Material.BaseMDUnit != facilityInventoryPos.FacilityCharge.MDUnit)
                                {
                                    aCMethodBooking.InwardTargetQuantity = facilityInventoryPos.FacilityCharge.Material.ConvertQuantity(aCMethodBooking.InwardTargetQuantity.Value, facilityInventoryPos.FacilityCharge.Material.BaseMDUnit, facilityInventoryPos.FacilityCharge.MDUnit);
                                    aCMethodBooking.InwardQuantity = aCMethodBooking.InwardTargetQuantity;
                                }
                            }

                            aCMethodBooking.InwardFacilityCharge = facilityInventoryPos.FacilityCharge;
                            aCMethodBooking.FacilityInventoryPos = facilityInventoryPos;
                            ACMethodEventArgs result = BookFacility(aCMethodBooking, databaseApp) as ACMethodEventArgs;
                            if (!aCMethodBooking.ValidMessage.IsSucceded() || aCMethodBooking.ValidMessage.HasWarnings())
                            {

                                string msg = "[InventoryClosing] Error Closing FacilityInventoryNo: {0}, Position (FacilityChargeID:{1}) {2} {3}, LotNo:{4}";
                                string lotNo = "-";
                                if (facilityInventoryPos.FacilityCharge.FacilityLot != null)
                                {
                                    lotNo = facilityInventoryPos.FacilityCharge.FacilityLot.LotNo;
                                }
                                msg = string.Format(msg, facilityInventoryNo, facilityInventoryPos.FacilityInventoryPosID, facilityInventoryPos.FacilityCharge.Material.MaterialNo,
                                    facilityInventoryPos.FacilityCharge.Material.MaterialName1, lotNo);
                                Root.Messages.LogError("FacilityManager", "InventoryClosing", msg);
                                msgWithDetails.AddDetailMessage(new Msg() { Message = msg, MessageLevel = eMsgLevel.Error });
                                msgWithDetails.AddDetailMessage(aCMethodBooking.ValidMessage);
                            }

                            // Doing inventory booking
                            if (progressCallback != null)
                                progressCallback(nr, count);
                        }

                        if (msgWithDetails.IsSucceded())
                        {
                            MDFacilityInventoryState finishedState = databaseApp.MDFacilityInventoryState.Where(c => c.MDFacilityInventoryStateIndex == (short)MDFacilityInventoryState.FacilityInventoryStates.Finished).FirstOrDefault();
                            facilityInventory.MDFacilityInventoryState = finishedState;
                            MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
                            if (!saveMsg.IsSucceded())
                                msgWithDetails.AddDetailMessage(saveMsg);
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
            }
            return msgWithDetails;
        }
    }
}
