using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Folgende Buchungsarten werden in dieser partiellen Klassen durchgeführt:
    /// StockCorrection:                     Bestandskorrektur allgemein
    /// StockCorrectionZeroStockMaterial:    Nullbestand (Materialbezogen)
    /// StockCorrectionZeroStockFacility:    Nullbestand (Lagerplatzbezogen)
    /// StockInventory:                      Inventurbuchung
    /// </summary>
    public partial class FacilityManager
    {
        #region Korrekturbuchungen
        public Global.ACMethodResultState DoStockCorrection(FacilityBooking facilityBooking)
        {
            //AddBookingMessage("DoStockCorrection");
            try
            {
                // TODO Damir: Umrechnungsfunktionen checken
                //StockCorrection(facilityBooking.InwardMaterial, facilityBooking.InwardFacility, facilityBooking.InwardFacilityCharge, facilityBooking.InwardQuantity, facilityBooking.InwardWeight);
                StockCorrection(facilityBooking.InwardMaterial, facilityBooking.InwardFacility, facilityBooking.InwardFacilityCharge, facilityBooking.InwardQuantity, 0);
                Database.ACSaveChanges();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("FacilityManager", "DoStockCorrection", msg);

                return Global.ACMethodResultState.Failed;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        // TODO: DoStockCorrectionZeroStockFacilityCharge
        public Global.ACMethodResultState DoStockCorrectionZeroStockFacilityCharge(FacilityBooking facilityBooking)
        {
            //AddBookingMessage("DoStockCorrectionZeroStockFacilityCharge");
            try
            {
                StockCorrectionZeroStockFacilityCharge(facilityBooking.InwardFacilityCharge);
                Database.ACSaveChanges();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("FacilityManager", "DoStockCorrectionZeroStockFacilityCharge", msg);

                return Global.ACMethodResultState.Failed;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        // TODO: DoStockCorrectionZeroStockMaterial
        public Global.ACMethodResultState DoStockCorrectionZeroStockMaterial(FacilityBooking facilityBooking)
        {
            //AddBookingMessage("DoStockCorrectionZeroStockMaterial");
            try
            {
                StockCorrectionZeroStockMaterial(facilityBooking.InwardMaterialStock);
                Database.ACSaveChanges();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("FacilityManager", "DoStockCorrectionZeroStockMaterial", msg);

                return Global.ACMethodResultState.Failed;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        // TODO: DoStockCorrectionZeroStockFacility
        public Global.ACMethodResultState DoStockCorrectionZeroStockFacility(FacilityBooking facilityBooking)
        {
            //AddBookingMessage("DoStockCorrectionZeroStockFacility");
            try
            {
                StockCorrectionZeroStockFacility(facilityBooking.InwardFacilityStock);
                Database.ACSaveChanges();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("FacilityManager", "DoStockCorrectionZeroStockFacility", msg);

                return Global.ACMethodResultState.Failed;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        // TODO: DoStockCorrectionZeroStockLot
        public Global.ACMethodResultState DoStockCorrectionZeroStockLot(FacilityBooking facilityBooking)
        {
            //AddBookingMessage("DoStockCorrectionZeroStockLot");
            try
            {
                StockCorrectionZeroStockLot(facilityBooking.InwardFacilityCharge.FacilityLot);
                Database.ACSaveChanges();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("FacilityManager", "DoStockCorrectionZeroStockLot", msg);

                return Global.ACMethodResultState.Failed;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        public Global.ACMethodResultState DoStockInventory(FacilityBooking facilityBooking)
        {
            //AddBookingMessage("DoStockInventory not Implemented");
            return Global.ACMethodResultState.Failed;
        }
        #endregion

        #region private Korrekturbuchungen

        //TODO: StockCorrectionZeroStockFacilityCharge
        Global.ACMethodResultState StockCorrectionZeroStockFacilityCharge(FacilityCharge facilityCharge)
        {
            //AddBookingMessage("StockCorrectionZeroStockFacilityCharge");
            return Global.ACMethodResultState.Succeeded;
        }

        //TODO: StockCorrectionZeroStockMaterial
        Global.ACMethodResultState StockCorrectionZeroStockMaterial(MaterialStock materialStock)
        {
            //AddBookingMessage("StockCorrectionZeroStockMaterial");
            materialStock.StockQuantity = 0;
            materialStock.StockWeight = 0;
            return Global.ACMethodResultState.Succeeded;
        }

        //TODO: StockCorrectionZeroStockFacility
        Global.ACMethodResultState StockCorrectionZeroStockFacility(FacilityStock facilityStock)
        {
            //AddBookingMessage("StockCorrectionZeroStockFacility");
            facilityStock.StockQuantity = 0;
            facilityStock.StockWeight = 0;
            return Global.ACMethodResultState.Succeeded;
        }

        //TODO: StockCorrectionZeroStockFacility
        Global.ACMethodResultState StockCorrectionZeroStockFacility(FacilityLotStock facilityLotStock)
        {
            //AddBookingMessage("StockCorrectionZeroStockFacilityLot");
            facilityLotStock.StockQuantity = 0;
            facilityLotStock.StockWeight = 0;
            return Global.ACMethodResultState.Succeeded;
        }

        //TODO: StockCorrectionZeroStockLot
        Global.ACMethodResultState StockCorrectionZeroStockLot(FacilityLot facilityLot)
        {
            //AddBookingMessage("StockCorrectionZeroStockLot");
            return Global.ACMethodResultState.Succeeded;
        }


        // TODO Damir: Umrechnungsfunktionen checken
        Global.ACMethodResultState StockCorrection(Material material, Facility facility, FacilityCharge facilityCharge, Double quantity, Double weight)
        {
            //AddBookingMessage("StockCorrection");
            facilityCharge.StockQuantity = quantity;
            return Global.ACMethodResultState.Succeeded;
        }
        #endregion
    }
}
