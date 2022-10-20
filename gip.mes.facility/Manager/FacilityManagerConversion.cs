using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Diese partielle Klasse beenhaltet Hilfsfunktionen für Mengenumrechnungen und Konvertierungen
    /// </summary>
    public partial class FacilityManager
    {
        public Global.ACMethodResultState ConvertQuantityUOMToFacilityChargeUnit(ACMethodBooking BP, Nullable<Double> quantityUOM, FacilityCharge facilityCharge, ref Double toQuantity)
        {
            if (facilityCharge == null)
                return Global.ACMethodResultState.Failed;

            if (quantityUOM.HasValue)
            {
                try
                {
                    toQuantity = facilityCharge.Material.ConvertQuantity(quantityUOM.Value, facilityCharge.Material.BaseMDUnit, facilityCharge.MDUnit);
                }
                catch (Exception e)
                {
                    BP.AddBookingMessage(ACMethodBooking.eResultCodes.QuantityConversionError, e.Message);
                    return Global.ACMethodResultState.Failed;
                }
            }
            else
                toQuantity = 0;
            return Global.ACMethodResultState.Succeeded;
        }

        public Global.ACMethodResultState ConvertQuantityInFacilityChargeUnit(ACMethodBooking BP, Nullable<Double> fromQuantity, MDUnit fromMDUnit, FacilityCharge facilityCharge, ref Double toQuantity)
        {
            if (fromMDUnit == null)
                return Global.ACMethodResultState.Failed;
            if (facilityCharge == null)
                return Global.ACMethodResultState.Failed;

            if (fromQuantity.HasValue)
            {
                try
                {
                    toQuantity = facilityCharge.Material.ConvertQuantity(fromQuantity.Value, fromMDUnit, facilityCharge.MDUnit);
                }
                catch (Exception e)
                {
                    BP.AddBookingMessage(ACMethodBooking.eResultCodes.QuantityConversionError, e.Message);
                    return Global.ACMethodResultState.Failed;
                }
            }
            else
                toQuantity = 0;
            return Global.ACMethodResultState.Succeeded;
        }


        public Global.ACMethodResultState ConvertQuantityToMaterialBaseUnit(ACMethodBooking BP, Nullable<Double> fromQuantity, MDUnit fromMDUnit, Material material, ref Double toQuantity)
        {
            if (fromMDUnit == null)
                return Global.ACMethodResultState.Failed;
            if (material == null)
                return Global.ACMethodResultState.Failed;

            if (fromQuantity.HasValue)
            {
                try
                {
                    toQuantity = material.ConvertToBaseQuantity(fromQuantity.Value, fromMDUnit);
                }
                catch (Exception e)
                {
                    BP.AddBookingMessage(ACMethodBooking.eResultCodes.QuantityConversionError, e.Message);
                    return Global.ACMethodResultState.Failed;
                }
            }
            else
                toQuantity = 0;
            return Global.ACMethodResultState.Succeeded;
        }
        
        /// <summary>
        /// Erzeugt eine neue eindeutige Chargennummer
        /// </summary>
        /// <returns></returns>
        public string GenerateChargeNo()
        {
            // TODO: GenerateChargeNo
            return "TODO";
        }

        public virtual Guid ResolveFacilityChargeIdFromBarcode(DatabaseApp dbApp, string barcodeID)
        {
            if (String.IsNullOrEmpty(barcodeID))
                return Guid.Empty;
            Guid facilityChargeId;
            if (Guid.TryParse(barcodeID, out facilityChargeId))
                return facilityChargeId;
            return Guid.Empty;
        }

        public virtual Guid ResolveFacilityLotIdFromBarcode(DatabaseApp dbApp, string barcodeID)
        {
            if (String.IsNullOrEmpty(barcodeID))
                return Guid.Empty;
            Guid facilityChargeId;
            if (Guid.TryParse(barcodeID, out facilityChargeId))
                return facilityChargeId;
            return Guid.Empty;
        }

        public virtual Guid ResolveFacilityIdFromBarcode(DatabaseApp dbApp, string barcodeID)
        {
            if (String.IsNullOrEmpty(barcodeID))
                return Guid.Empty;
            Guid facilityChargeId;
            if (Guid.TryParse(barcodeID, out facilityChargeId))
                return facilityChargeId;
            return Guid.Empty;
        }

        public virtual Guid ResolveMaterialIdFromBarcode(DatabaseApp dbApp, string barcodeID)
        {
            if (String.IsNullOrEmpty(barcodeID))
                return Guid.Empty;
            Guid materialId;
            if (Guid.TryParse(barcodeID, out materialId))
                return materialId;
            return Guid.Empty;
        }
    }
}
