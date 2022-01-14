using System;
using System.Collections.Generic;
using System.Text;

namespace gip.mes.webservices
{
    public static class VBWebServiceConst
    {
        #region General

        public const string UriBarcodeEntity_BarcodeID = "BarcodeEntity/{barcodeID}";
        public const string UriBarcodeEntity_BarcodeID_F = "BarcodeEntity/{0}";

        public const string UriBarcodeSequence = "BarcodeSequence";

        public const string UriPrint="Print";
        public const string UriGetAssignedPrinter = "GetPrinter";
        public const string UriGetScannedPrinter = "GetScannedPrinter/{printerID}";
        public const string UriGetScannedPrinter_F = "GetScannedPrinter/{0}";
        public const string UriAssignPrinter = "AssignPrinter";

        #endregion


        #region Masterdata

        public const string UriMaterial = "Materials";
        public const string UriMaterialID = "Materials/{materialID}";
        public const string UriMaterialID_F = "Materials/{0}";
        public const string UriMaterial_Search = "Materials/term/{term}";
        public const string UriMaterial_Search_F = "Materials/term/{0}";
        public const string UriMaterial_BarcodeID = "Materials/barcode/{barcodeID}";
        public const string UriMaterial_BarcodeID_F = "Materials/barcode/{0}";

        public const string UriFacility = "Facilities";
        public const string UriFacilityID = "Facilities/{facilityID}";
        public const string UriFacilityID_F = "Facilities/{0}";
        public const string UriFacility_Search = "Facilities/term/{term}/parent/{parentFacilityID}/type/{type}";
        public const string UriFacility_Search_F = "Facilities/term/{0}/parent/{1}/type/{2}";

        public const string UriFacility_BarcodeID = "Facilities/barcode/{barcodeID}";
        public const string UriFacility_BarcodeID_F = "Facilities/barcode/{0}";

        #endregion


        #region Logistics

        public const string UriPickingTypes = "PickingTypes";
        public const string UriPicking = "Pickings";
        public const string UriSearchPicking = "SPickings/{pType}/{fromFacility}/{toFacility}/{fromDate}/{toDate}";
        public const string UriSearchPickingF = "SPickings/{0}/{1}/{2}/{3}/{4}";
        public const string UriPickingID = "Pickings/{pickingID}";
        public const string UriPickingID_F = "Pickings/{0}";
        public const string UriPickingPosID = "PickingPos/{pickingPosID}";
        public const string UriPickingPosID_F = "PickingPos/{0}";
        public const string UriPickingPos_Postings = "PickingPos/Postings/{pickingPosID}";
        public const string UriPickingPos_Postings_F = "PickingPos/Postings/{0}";
        public const string UriPickingPos_Material = "PickingPosByMaterial";
        public const string UriPicking_Finish = "PickingFinish";
        public const string UriPicking_FinishWithoutCheck = "PickingFinishWithoutCheck";

        #endregion


        #region Manufacturing

        public const string UriProdOrderPartslists = "ProdOrderPartslists";
        public const string UriProdOrderPartslistID = "ProdOrderPartslists/{prodOrderPartslistID}";
        public const string UriProdOrderPartslistID_F = "ProdOrderPartslists/{0}";

        public const string UriProdOrderPLIntermediates = "ProdOrderPLIntermediates/Parent/{prodOrderPartslistID}";
        public const string UriProdOrderPLIntermediates_F = "ProdOrderPLIntermediates/Parent/{0}";
        public const string UriProdOrderPLIntermediateID = "ProdOrderPLIntermediates/{intermediateID}";
        public const string UriProdOrderPLIntermediatesID_F = "ProdOrderPLIntermediates/{0}";

        public const string UriProdOrderIntermBatches = "ProdOrderIntermBatches/Parent/{parentIntermediateID}";
        public const string UriProdOrderIntermBatches_F = "ProdOrderIntermBatches/Parent/{0}";
        public const string UriProdOrderIntermBatchID = "ProdOrderIntermBatches/{intermediateID}";
        public const string UriProdOrderIntermBatchID_F = "ProdOrderIntermBatches/{0}";


        public const string UriProdOrderInputMaterials = "ProdOrderInputMaterials/{targetPOPLPosID}";
        public const string UriProdOrderInputMaterials_F = "ProdOrderInputMaterials/{0}";
        public const string UriProdOrderPosRelFacilityBooking = "ProdOrderPosRelFB/{POPLPosRelID}";
        public const string UriProdOrderPosRelFacilityBooking_F = "ProdOrderPosRelFB/{0}";
        public const string UriProdOrderPosFacilityBooking = "ProdOrderPosFB/{POPLPosID}";
        public const string UriProdOrderPosFacilityBooking_F = "ProdOrderPosFB/{0}";
        public const string UriProdOrderIntermOrIntermBatchByMachine = "ProdOrderIntermOrIntermBatchByMachine/{machineID}";
        public const string UriProdOrderIntermOrIntermBatchByMachine_F = "ProdOrderIntermOrIntermBatchByMachine/{0}";
        public const string UriProdOrderPLPos = "ProdOrderPLPos/{POPLPosID}";
        public const string UriProdOrderPLPos_F = "ProdOrderPLPos/{0}";
        public const string UriProdOrderPLPosRel = "ProdOrderPLPosRel/{POPLPosRelID}";
        public const string UriProdOrderPLPosRel_F = "ProdOrderPLPosRel/{0}";
        public const string UriProdOrderBatchTargetFacilities = "ProdOrderBTF/{intermBatchID}";
        public const string UriProdOrderBatchTargetFacilities_F = "ProdOrderBTF/{0}";

        #endregion


        #region Facility

        public const string UriFacilityCharge = "FacilityCharges";

        public const string UriFacilityChargeID = "FacilityCharges/{facilityChargeID}";
        public const string UriFacilityChargeID_F = "FacilityCharges/{0}";

        public const string UriFacilityCharge_BarcodeID = "FacilityCharges/barcode/{barcodeID}";
        public const string UriFacilityCharge_BarcodeID_F = "FacilityCharges/barcode/{0}";

        public const string UriFacilityCharge_Bookings = "FacilityCharges/Bookings/{facilityChargeID}/from/{dateFrom}/to/{dateTo}";
        public const string UriFacilityCharge_Bookings_F = "FacilityCharges/Bookings/{0}/from/{1}/to/{2}";

        public const string UriFacilityChargeFacilityMaterialLot = "FacilityChargeByFacilityMaterialLot/facility/{facilityID}/material/{materialID}/lot/{facilityLotID}/split/{splitNo}";
        public const string UriFacilityChargeFacilityMaterialLot_F = "FacilityChargeByFacilityMaterialLot/facility/{0}/material/{1}/lot/{2}/split/{3}";


        public const string UriFacilityLot = "FacilityLots";

        public const string UriFacilityLotID = "FacilityLots/{facilityLotID}";
        public const string UriFacilityLotID_F = "FacilityLots/{0}";

        public const string UriFacilityLot_Search = "FacilityLots/term/{term}";
        public const string UriFacilityLot_Search_F = "FacilityLots/term/{0}";

        public const string UriFacilityLot_BarcodeID = "FacilityLots/barcode/{barcodeID}";
        public const string UriFacilityLot_BarcodeID_F = "FacilityLots/barcode/{0}";

        public const string UriFacilityLot_SumID = "FacilityLots/Sum/{facilityLotID}";
        public const string UriFacilityLot_SumID_F = "FacilityLots/Sum/{0}";

        public const string UriFacilityLot_Bookings = "FacilityLots/Bookings/{facilityLotID}/from/{dateFrom}/to/{dateTo}";
        public const string UriFacilityLot_Bookings_F = "FacilityLots/Bookings/{0}/from/{1}/to/{2}";


        public const string UriMaterial_SumID = "Materials/Sum/{materialID}";
        public const string UriMaterial_SumID_F = "Materials/Sum/{0}";

        public const string UriMaterial_Bookings = "Materials/Bookings/{materialID}/from/{dateFrom}/to/{dateTo}";
        public const string UriMaterial_Bookings_F = "Materials/Bookings/{0}/from/{1}/to/{2}";


        public const string UriFacility_SumID = "Facilities/Sum/{facilityID}";
        public const string UriFacility_SumID_F = "Facilities/Sum/{0}";

        public const string UriFacility_Bookings = "Facilities/Bookings/{facilityID}/from/{dateFrom}/to/{dateTo}";
        public const string UriFacility_Bookings_F = "Facilities/Bookings/{0}/from/{1}/to/{2}";


        public const string UriFacilityLocation_SumID = "FacilityLocation/Sum/{facilityID}";
        public const string UriFacilityLocation_SumID_F = "FacilityLocation/Sum/{0}";

        public const string UriFacilityLocation_Bookings = "FacilityLocation/Bookings/{facilityID}/from/{dateFrom}/to/{dateTo}";
        public const string UriFacilityLocation_Bookings_F = "FacilityLocation/Bookings/{0}/from/{1}/to/{2}";

        public const string UriBookFacility = "BookFacility";
        public const string UriBookFacilities = "BookFacilities";

        #endregion

        #region Inventory
        public const string UrlInventory_Root = "FacilityInventory";

        #region Inventory-> MD
        public const string UrlInventory_MDFacilityInventoryState = "FacilityInventory/Get/MDFacilityInventoryState";
        public const string UrlInventory_MDFacilityInventoryPosState = "FacilityInventory/Get/MDFacilityInventoryPosState";
        #endregion

        #region Inventory -> Get

        // WSResponse<List<FacilityInventory>> GetFacilityInventories (short? inventoryState, DateTime dateFrom, DateTime dateTo)
        public const string UrlInventory_Inventories = "FacilityInventory/FacilityInventories/InventoryState/{inventoryState}/from/{dateFrom}/to/{dateTo}";
        public const string UrlInventory_Inventories_F = "FacilityInventory/FacilityInventories/inventoryState/{0}/from/{1}/to/{2}";

        #endregion

        #region Inventory -> Pos
        #region Inventory -> Pos - Get
        // WSResponse<List<FacilityInventoryPos>> GetFacilityInventoryLines (string facilityInventoryNo, string inputCode, string storageLocationNo, string facilityNo, string lotNo, string materialNo, string inventoryPosState, string notAvailable, string zeroStock)
        public const string UrlInventory_InventoryLines = "FacilityInventoryLines/FacilityInventoryNo/{facilityInventoryNo}/InputCode/{inputCode}/StorageLocationNo/{storageLocationNo}/FacilityNo/{facilityNo}/LotNo/{lotNo}/MaterialNo/{materialNo}/InventoryPosState/{inventoryPosState}/NotAvailable/{notAvailable}/ZeroStock/{zeroStock}/NotProcessed/{notProcessed}";
        public const string UrlInventory_InventoryLines_F = "FacilityInventoryLines/FacilityInventoryNo/{0}/InputCode/{1}/StorageLocationNo/{2}/FacilityNo/{3}/LotNo/{4}/MaterialNo/{5}/InventoryPosState/{6}/NotAvailable/{7}/ZeroStock/{8}/NotProcessed/{9}";

        // WSResponse<SearchFacilityCharge> GetFacilityInventorySearchCharge (string facilityInventoryNo, string storageLocationNo, string facilityNo, string facilityChargeID)
        public const string UrlInventory_SearchCharge = "FacilityInventoryPos/FacilityInventoryNo/{facilityInventoryNo}/StorageLocationNo/{storageLocationNo}/FacilityNo/{facilityNo}/FacilityChargeID/{facilityChargeID}";
        public const string UrlInventory_SearchCharge_F = "FacilityInventoryPos/FacilityInventoryNo/{0}/StorageLocationNo/{1}/FacilityNo/{2}/FacilityChargeID/{3}";

        // WSResponse<FacilityInventoryPos> SetFacilityInventoryChargeAvailable (string facilityInventoryNo, string facilityChargeID)
        public const string UrlInventory_SetChargeAvailable = "FacilityInventoryChargeAvailable/FacilityInventoryNo/{facilityInventoryNo}/FacilityChargeID/{facilityChargeID}";
        public const string UrlInventory_SetChargeAvailable_F = "FacilityInventoryChargeAvailable/FacilityInventoryNo/{0}/FacilityChargeID/{1}";


        #endregion

        #region Inventory -> Pos -> Lifecycle
        // WSResponse UpdateFacilityInventoryPos (string facilityInventoryNo, string facilityChargeID, string newStockQuantity, string comment, string notAvailable)
        public const string UrlInventory_InventoryPos_Update = "FacilityInventoryPos/Update";

        #endregion

        #endregion

        #endregion

    }
}
