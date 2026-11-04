using VD = gip.mes.datamodel;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;
using gip.core.datamodel;
using gip.mes.cmdlet.Barcode.Model;
using gip.mes.datamodel;

namespace gip.mes.cmdlet.Barcode
{
    /// <summary>
    /// Gets item IDs used for barcode generation.
    /// (1) BarcodeType == FacilityCharge: gets the FacilityChargeID for an item in a facility.
    ///     Enter the data:
    ///     FacilityNo - identifier of the facility where the quant is located
    ///     MaterialNo - material number of the quant
    ///     LotNo - lot number of the quant
    /// 
    /// 
    /// (2) BarcodeType == Machine: gets the ACClassID for a machine.
    ///     MachineACIdentifier - ACClass.ACIdentifier of the machine
    /// 
    /// (3) BarcodeType == Facility: gets the FacilityID for a facility.
    /// 
    /// </summary>
    [Cmdlet(VerbsCommon.Get, CmdletName)]
    public class iPlusDataForBarcodeCmdlet : Cmdlet
    {
        public const string CmdletName = "iPlusDataForBarcode";

        #region Mandatory params

        /// <summary>
        /// Defines the item type:
        /// - FacilityCharge for a quant
        /// - Machine for a machine
        /// - Facility for a facility
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public BarcodeTypeEnum BarcodeType { get; set; }

        #endregion

        #region OptionalParams
        /// <summary>
        /// Facility number used to filter the returned barcode data.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string FacilityNo { get; set; }

        /// <summary>
        /// Material number used to filter the returned barcode data.
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string MaterialNo { get; set; }

        /// <summary>
        /// Machine name used to filter the returned barcode data.
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string MachineACIdentifier { get; set; }

        /// <summary>
        /// Lot number used to filter the returned barcode data.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string LotNo { get; set; }

        #endregion

        #region Overrides
        protected override void ProcessRecord()
        {
            List<BarcodeResult> results = null;
            switch (BarcodeType)
            {
                case BarcodeTypeEnum.FacilityCharge:
                    results = GetFacilityCharges(MaterialNo, FacilityNo, LotNo);
                    break;
                case BarcodeTypeEnum.Machine:
                    results = GetMachines(MachineACIdentifier);
                    break;
                case BarcodeTypeEnum.Facility:
                    results = GetFacilities(FacilityNo);
                    break;
                default:
                    break;
            }

            if (results != null)
            {
                WriteObject(results);
            }
        }

        #endregion

        #region Methods

        private List<BarcodeResult> GetFacilityCharges(string materialNo, string facilityNo, string lotNo)
        {
            List<BarcodeResult> result = new List<BarcodeResult>();
            using (VD.DatabaseApp databaseApp = new VD.DatabaseApp())
            {
                List<VD.FacilityCharge> facilityCharges =
                    databaseApp
                    .FacilityCharge
                    .Where(c =>
                        !c.NotAvailable
                        && (string.IsNullOrEmpty(materialNo) || c.Material.MaterialNo.Contains(materialNo) || c.Material.MaterialName1.Contains(materialNo))
                        && (string.IsNullOrEmpty(facilityNo) || c.Facility.FacilityNo.Contains(facilityNo) || c.Facility.FacilityName.Contains(facilityNo))
                        && (string.IsNullOrEmpty(lotNo) || c.FacilityLot.LotNo.Contains(lotNo))
                   )
                   .ToList();
                result =
                    facilityCharges
                    .Select(c => new BarcodeResult()
                    {
                        ID = c.FacilityChargeID,
                        ItemNo = $"{c.Facility.FacilityNo} | {c.Material.MaterialNo}",
                        ItemName = $"{c.Facility.FacilityName} | {c.Material.MaterialName1}"
                    })
                    .ToList();
            }

            return result;
        }

        private List<BarcodeResult> GetMachines(string machineACIdentifier)
        {
            List<BarcodeResult> result = new List<BarcodeResult>();
            using (Database database = new Database())
            {
                List<gip.core.datamodel.ACClass> cls =
                    database
                    .ACClass
                    .Where(c =>
                        (string.IsNullOrEmpty(machineACIdentifier) || c.ACIdentifier.Contains(machineACIdentifier))
                    )
                    .OrderBy(c => c.ACProject.ACProjectName)
                    .ThenBy(c => c.ACIdentifier)
                    .ToList();

                result =
                    cls
                    .Select(c => new BarcodeResult()
                    {
                        ID = c.ACClassID,
                        ItemNo = $"{c.ACProject.ACProjectName} | {c.ACIdentifier}",
                        ItemName = c.ACUrlComponent
                    })
                    .ToList();
            }

            return result;
        }
        
        private List<BarcodeResult> GetFacilities(string facility)
        {
            List<BarcodeResult> result = new List<BarcodeResult>();
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                List<Facility> cls =
                    databaseApp
                    .Facility
                    .Where(c =>
                        (string.IsNullOrEmpty(facility) || c.FacilityNo.Contains(facility) || c.FacilityName.Contains(facility))
                    )
                    .OrderBy(c => c.FacilityNo)
                    .ToList();

                result =
                    cls
                    .Select(c => new BarcodeResult()
                    {
                        ID = c.FacilityID,
                        ItemNo = $"{c.FacilityNo}",
                        ItemName = c.FacilityName
                    })
                    .ToList();
            }

            return result;
        }

        #endregion

    }
}
