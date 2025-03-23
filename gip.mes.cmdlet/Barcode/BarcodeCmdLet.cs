using gip.mes.cmdlet.Settings;
using VD = gip.mes.datamodel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;
using gip.core.datamodel;

namespace gip.mes.cmdlet.Barcode
{
    [Cmdlet(VerbsCommon.Get, CmdLetSettings.iPlusBarcodeCmdlet_Name)]
    public class BarcodeCmdLet : Cmdlet
    {


        #region Mandatory params

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public BarcodeTypeEnum BarcodeType { get; set; }

        #endregion

        #region OptionalParams
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string Facility { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string Material { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string Machine { get; set; }

        #endregion

        #region Overrides
        protected override void ProcessRecord()
        {
            List<BarcodeResult> results = null;
            if (BarcodeType == BarcodeTypeEnum.FacilityCharge)
            {
                results = GetFacilityCharges(Material, Facility);
            }
            else
            {
                results = GetMachines(Machine);
            }

            if (results != null)
            {
                WriteObject(results);
            }
        }

        #endregion

        #region Methods
        private List<BarcodeResult> GetFacilityCharges(string material, string facility)
        {
            List<BarcodeResult> result = new List<BarcodeResult>();
            using (VD.DatabaseApp databaseApp = new VD.DatabaseApp())
            {
                List<VD.FacilityCharge> facilityCharges =
                    databaseApp
                    .FacilityCharge
                    .Where(c =>
                    !c.NotAvailable
                    && (string.IsNullOrEmpty(material) || c.Material.MaterialNo.Contains(material) || c.Material.MaterialName1.Contains(material))
                    && (string.IsNullOrEmpty(facility) || c.Facility.FacilityNo.Contains(facility) || c.Facility.FacilityName.Contains(facility))

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

        private List<BarcodeResult> GetMachines(string machine)
        {
            List<BarcodeResult> result = new List<BarcodeResult>();
            using (Database database = new Database())
            {
                List<ACClass> cls =
                    database
                    .ACClass
                    .Where(c =>
                        c.ACIdentifier == "Work"
                        && (string.IsNullOrEmpty(Machine) || c.ACIdentifier.Contains(Machine))
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
        #endregion
    }
}
