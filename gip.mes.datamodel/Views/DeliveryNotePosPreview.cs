using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'DeliveryNotePosPreview'}de{'DeliveryNotePosPreview'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    public class DeliveryNotePosPreview
    {

        #region ctor's

        public DeliveryNotePosPreview()
        {

        }

        public DeliveryNotePosPreview(DeliveryNotePos dns)
        {
            DeliveryNotePosID = dns.DeliveryNotePosID;
            DeliveryNoteNo = dns.DeliveryNote.DeliveryNoteNo;

            if (dns.DeliveryNote.DeliveryCompanyAddress != null)
            {
                DeliveryAddress =
                    dns.DeliveryNote.DeliveryCompanyAddress.Company.CompanyName +
                    Environment.NewLine +
                    dns.DeliveryNote.DeliveryCompanyAddress.Street +
                    Environment.NewLine +
                    dns.DeliveryNote.DeliveryCompanyAddress.Postcode + " " +
                    dns.DeliveryNote.DeliveryCompanyAddress.City;
                if (dns.DeliveryNote.DeliveryCompanyAddress.MDCountry != null)
                    DeliveryAddress += Environment.NewLine + dns.DeliveryNote.DeliveryCompanyAddress.MDCountry.MDCountryName;
            }

            if (dns.DeliveryNote.ShipperCompanyAddress != null)
            {
                ShipperAddress =
                    dns.DeliveryNote.ShipperCompanyAddress.Company.CompanyName +
                    Environment.NewLine +
                    dns.DeliveryNote.ShipperCompanyAddress.Street +
                    Environment.NewLine +
                    dns.DeliveryNote.ShipperCompanyAddress.Postcode + " " +
                    dns.DeliveryNote.ShipperCompanyAddress.City;
                if (dns.DeliveryNote.ShipperCompanyAddress.MDCountry != null)
                    ShipperAddress += Environment.NewLine + dns.DeliveryNote.ShipperCompanyAddress.MDCountry.MDCountryName;
            }

            DeliveryDate = dns.DeliveryNote.DeliveryDate;

            if (dns.OutOrderPos != null)
            {
                TargetQuantity = dns.OutOrderPos.TargetQuantity;
                ActualQuantity = dns.OutOrderPos.ActualQuantity;
                if (dns.OutOrderPos.MDUnit == null)
                {
                    MDUnitName = dns.OutOrderPos.Material.BaseMDUnit.MDUnitName;
                }
                else
                {
                    MDUnitName = dns.OutOrderPos.MDUnit.MDUnitName;
                }
            }
            if (dns.InOrderPos != null)
            {
                TargetQuantity = dns.InOrderPos.TargetQuantity;
                ActualQuantity = dns.InOrderPos.ActualQuantity;
                if (dns.InOrderPos.MDUnit == null)
                {
                    MDUnitName = dns.InOrderPos.Material.BaseMDUnit.MDUnitName;
                }
                else
                {
                    MDUnitName = dns.InOrderPos.MDUnit.MDUnitName;
                }

                LotList = dns
                   .InOrderPos
                   .FacilityBookingCharge_InOrderPos
                   .Where(c => c.InwardFacilityChargeID != null && c.InwardFacilityCharge.FacilityLotID != null)
                   .Select(c => c.InwardFacilityCharge.FacilityLot.LotNo)
                   .Distinct()
                   .OrderBy(c => c)
                   .ToList();


                foreach (var item in dns.InOrderPos.FacilityBookingCharge_InOrderPos)
                {
                    if (item.InwardFacilityID != null && (FacilityNo == null || !FacilityNo.Contains(item.InwardFacility.FacilityNo)))
                    {
                        FacilityNo += item.InwardFacility.FacilityNo + ", ";
                    }
                }

                if (!string.IsNullOrEmpty(FacilityNo))
                    FacilityNo = FacilityNo.TrimEnd(", ".ToCharArray());

            }

            MaterialNo = dns.Material.MaterialNo;
            MaterialName = dns.Material.MaterialName1;

            DosedQuantity = 0;
        }

        #endregion

        #region Properties

        [ACPropertyInfo(9999, "Sn", "en{'Sn'}de{'Sn'}")]
        public int Sn { get; set; }

        public Guid DeliveryNotePosID { get; set; }

        [ACPropertyInfo(9999, "DeliveryNoteNo", "en{'Deliverynote-No.'}de{'Lieferschein-Nr.'}")]
        public string DeliveryNoteNo { get; set; }

        [ACPropertyInfo(9999, "MaterialNo", ConstApp.MaterialNo)]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(9999, "MaterialName", "en{'Material Desc. 1'}de{'Materialbez. 1'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(9999, "DeliveryAddress", "en{'Delivery address'}de{'Lieferadresse'}")]
        public string DeliveryAddress { get; set; }

        [ACPropertyInfo(9999, "TrackingAndTracingDeliveryNote", "en{'Shipper address'}de{'Speditionsadresse'}")]
        public string ShipperAddress { get; set; }

        [ACPropertyInfo(9999, "DeliveryDate", "en{'Delivery Date'}de{'Lieferdatum'}")]
        public DateTime DeliveryDate { get; set; }

        [ACPropertyInfo(9999, "TargetQuantity", ConstApp.TargetQuantity)]
        public double TargetQuantity { get; set; }

        [ACPropertyInfo(9999, "ActualQuantity", ConstApp.ActualQuantity)]
        public double ActualQuantity { get; set; }

        [ACPropertyInfo(9999, "MDUnitName", "en{'Unit'}de{'Einheit'}")]
        public string MDUnitName { get; set; }

        public List<string> LotList { get; set; }

        private string _lots;
        [ACPropertyInfo(9999, "TrackingAndTracingDeliveryNote", "en{'Lots'}de{'Loses'}")]
        public string Lots
        {
            get
            {
                if (string.IsNullOrEmpty(_lots) && LotList != null && LotList.Any())
                {
                    _lots = string.Join(", ", LotList);
                    _lots = _lots.TrimEnd();
                }
                return _lots;
            }
        }

        [ACPropertyInfo(9999, "FacilityNo", "en{'Silo'}de{'Silo'}")]
        public string FacilityNo { get; set; }

        /// <summary>
        /// How many material is spended
        /// </summary>
        [ACPropertyInfo(9999, "DosedQuantity", "en{'Dosed quantity'}de{'Dosierte Menge'}")]
        public double DosedQuantity { get; set; }

        #endregion

    }
}
