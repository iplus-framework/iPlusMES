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

        public MsgWithDetails InventoryGenerate(string facilityInventoryNo, string facilityInventoryName, Action<int, int> progressCallback)
        {
            MsgWithDetails msgWithDetails = null;

            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                FacilityInventory facilityInventory = FacilityInventory.NewACObject(databaseApp, null, facilityInventoryNo);
                facilityInventory.FacilityInventoryName = facilityInventoryName;

                List<FacilityCharge> facilityCharges =
                    databaseApp
                    .FacilityCharge
                    .Where(c => !c.NotAvailable)
                    .OrderBy(c => c.FacilityLot.LotNo)
                   .ToList();
                int count = facilityCharges.Count();
                if (progressCallback != null)
                    progressCallback(0, count);
                List<FacilityInventoryPos> poses = new List<FacilityInventoryPos>();
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

                    List<FacilityInventoryPos> poses = facilityInventory.FacilityInventoryPos_FacilityInventory.ToList();


                    bool isNotAllowedClosing =
                        facilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)MDFacilityInventoryState.FacilityInventoryStates.InProgress
                        ||
                         poses.Any(c => c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex != (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished);
                    if (isNotAllowedClosing)
                    {
                        Msg msErrorNotAllowedClosing = new Msg() { MessageLevel = eMsgLevel.Error, ACIdentifier = Const_Inventory_NotAllowedClosing };
                        msgWithDetails.AddDetailMessage(msErrorNotAllowedClosing);
                    }
                    else
                    {
                        foreach (FacilityInventoryPos pos in poses)
                        {
                            nr++;

                            ACMethodBooking aCMethodBooking = ACUrlACTypeSignature("!" + GlobalApp.FBT_StockCorrection, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                            aCMethodBooking.BookingType = GlobalApp.FacilityBookingType.StockCorrection;
                            aCMethodBooking.MDBalancingMode = DatabaseApp.s_cQry_GetMDBalancingMode(databaseApp, MDBalancingMode.BalancingModes.InwardOff_OutwardOff).FirstOrDefault();
                            aCMethodBooking.InwardTargetQuantity = pos.NewStockQuantity - pos.StockQuantity;

                            ACMethodEventArgs result = BookFacility(aCMethodBooking, databaseApp) as ACMethodEventArgs;
                            if (!aCMethodBooking.ValidMessage.IsSucceded() || aCMethodBooking.ValidMessage.HasWarnings())
                                msgWithDetails.AddDetailMessage(aCMethodBooking.ValidMessage);

                            // Doing inventory booking
                            if (progressCallback != null)
                                progressCallback(nr, count);
                        }

                        MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
                        if (!saveMsg.IsSucceded())
                            msgWithDetails.AddDetailMessage(saveMsg);
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
