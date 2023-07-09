using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TrackAndTracingResult'}de{'TrackAndTracingResult'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, false, true)]
    public class TrackingAndTracingResult
    {
        #region Properties

        #region Properites - > Point

        private TrackingAndTracingPoint rootPoint;
        [ACPropertyInfo(999)]
        public TrackingAndTracingPoint RootPoint
        {
            get
            {
                return rootPoint;
            }
            set
            {
                rootPoint = value;
                rootPoint.ParentResultObject = this;
            }
        }

        #endregion

        #region Properties -> Lists

        private List<Guid> processedItems;
        public List<Guid> ProcessedItems
        {
            get
            {
                if (processedItems == null)
                    processedItems = new List<Guid>();
                return processedItems;
            }
        }

        private List<FacilityCharge> _FacilityChargeList;
        public List<FacilityCharge> FaciltiyChargeList
        {
            get
            {
                return _FacilityChargeList;
            }
        }

        private List<FacilityChargeModel> _FacilityChargeModelList;
        public List<FacilityChargeModel> FacilityChargeModelList
        {
            get
            {
                if (_FacilityChargeModelList == null)
                    _FacilityChargeModelList = new List<FacilityChargeModel>();
                return _FacilityChargeModelList;
            }
            set
            {
                _FacilityChargeModelList = value;
            }
        }

        private List<FacilityBooking> _FacilityBookingList;
        public List<FacilityBooking> FacilityBookingList
        {
            get
            {
                if (_FacilityBookingList == null)
                    _FacilityBookingList = new List<FacilityBooking>();
                return _FacilityBookingList;
            }
            set
            {
                _FacilityBookingList = value;
            }
        }

        private List<DeliveryNotePosPreview> _DeliveryNoteList;
        public List<DeliveryNotePosPreview> DeliveryNoteList
        {
            get
            {
                if (_DeliveryNoteList == null)
                    _DeliveryNoteList = new List<DeliveryNotePosPreview>();
                return _DeliveryNoteList;
            }
            set
            {
                _DeliveryNoteList = value;
            }
        }

        private List<string> filter;
        public List<string> Filter
        {
            get
            {
                if (filter == null)
                    filter = new List<string>();
                return filter;
            }
            set
            {
                filter = value;
            }
        }

        /// <summary>
        /// List items with laborder
        /// </summary>
        private List<TrackingAndTracingPoint> itemsWithLabOrder;
        public List<TrackingAndTracingPoint> ItemsWithLabOrder
        {
            get
            {
                if (itemsWithLabOrder == null)
                    itemsWithLabOrder = new List<TrackingAndTracingPoint>();
                return itemsWithLabOrder;
            }
        }
        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///  Filtering tree building by points
        /// </summary>
        /// <param name="point"></param>
        /// <param name="rootPoint"></param>
        /// <returns></returns>
        public TrackingAndTracingPoint ReturnPointFiltered(TrackingAndTracingPoint point, TrackingAndTracingPoint rootPoint)
        {
            if (Filter.Count == 0 || Filter.Contains(point.Related.GetType().Name))
                return point;
            return rootPoint;
        }

        public void ProcessStatistics(DatabaseApp dbApp)
        {
            _FacilityChargeList = new List<FacilityCharge>();
            RootPoint.ProcessStatistics();
            if (_FacilityChargeList != null && _FacilityChargeList.Any())
            {
                var tmpIDs = _FacilityChargeList.Select(fc => fc.FacilityChargeID).ToArray();
                IQueryable<FacilityCharge> tmpFcQuery = dbApp.FacilityCharge.Where(c => tmpIDs.Contains(c.FacilityChargeID));
                _FacilityChargeModelList = FacilityCharge.GetFacilityChargeModelList(dbApp, tmpFcQuery).ToList();
            }
            if (FacilityChargeModelList != null)
                FacilityChargeModelList = FacilityChargeModelList.OrderBy(x => x.InsertDate).ToList();
            if (DeliveryNoteList != null)
                DeliveryNoteList = DeliveryNoteList.OrderBy(x => x.DeliveryDate).ToList();
            if (FacilityBookingList != null)
                FacilityBookingList = FacilityBookingList.OrderBy(x => x.InsertDate).ToList();
        }

        /// <summary>
        /// Head method for recive events for processing a added elements by type
        /// </summary>
        /// <param name="acObject"></param>
        internal void ProcessStatistics(IACObject acObject)
        {
            TrackingAndTracingPoint point = acObject as TrackingAndTracingPoint;
            if (point.Related is FacilityCharge)
            {
                FacilityCharge fc = point.Related as FacilityCharge;
                if (!_FacilityChargeList.Any(x => x.FacilityChargeID == fc.FacilityChargeID))
                    _FacilityChargeList.Add(fc);
            }
            if (point.Related is FacilityBooking)
            {
                FacilityBooking fb = point.Related as FacilityBooking;
                if (!FacilityBookingList.Any(x => x.FacilityBookingID == fb.FacilityBookingID))
                    FacilityBookingList.Add(fb);
            }
            if (point.Related is DeliveryNotePos)
            {
                DeliveryNotePos dns = point.Related as DeliveryNotePos;
                if (!DeliveryNoteList.Any(x => x.DeliveryNotePosID == dns.DeliveryNotePosID))
                {
                    DeliveryNotePosPreview dnsPreview = new DeliveryNotePosPreview();

                    dnsPreview.DeliveryNotePosID = dns.DeliveryNotePosID;
                    dnsPreview.DeliveryNoteNo = dns.DeliveryNote.DeliveryNoteNo;

                    if (dns.DeliveryNote.DeliveryCompanyAddress != null)
                    {
                        dnsPreview.DeliveryAddress =
                            dns.DeliveryNote.DeliveryCompanyAddress.Company.CompanyName +
                            Environment.NewLine +
                            dns.DeliveryNote.DeliveryCompanyAddress.Street +
                            Environment.NewLine +
                            dns.DeliveryNote.DeliveryCompanyAddress.Postcode + " " +
                            dns.DeliveryNote.DeliveryCompanyAddress.City;
                        if (dns.DeliveryNote.DeliveryCompanyAddress.MDCountry != null)
                            dnsPreview.DeliveryAddress += Environment.NewLine + dns.DeliveryNote.DeliveryCompanyAddress.MDCountry.MDCountryName;
                    }

                    if (dns.DeliveryNote.ShipperCompanyAddress != null)
                    {
                        dnsPreview.ShipperAddress =
                            dns.DeliveryNote.ShipperCompanyAddress.Company.CompanyName +
                            Environment.NewLine +
                            dns.DeliveryNote.ShipperCompanyAddress.Street +
                            Environment.NewLine +
                            dns.DeliveryNote.ShipperCompanyAddress.Postcode + " " +
                            dns.DeliveryNote.ShipperCompanyAddress.City;
                        if (dns.DeliveryNote.ShipperCompanyAddress.MDCountry != null)
                            dnsPreview.ShipperAddress += Environment.NewLine + dns.DeliveryNote.ShipperCompanyAddress.MDCountry.MDCountryName;
                    }


                    dnsPreview.DeliveryDate = dns.DeliveryNote.DeliveryDate;


                    if (dns.OutOrderPos != null)
                    {
                        dnsPreview.TargetQuantity = dns.OutOrderPos.TargetQuantity;
                        dnsPreview.ActualQuantity = dns.OutOrderPos.ActualQuantity;
                        if (dns.OutOrderPos.MDUnit == null)
                        {
                            dnsPreview.MDUnitName = dns.OutOrderPos.Material.BaseMDUnit.MDUnitName;
                        }
                        else
                        {
                            dnsPreview.MDUnitName = dns.OutOrderPos.MDUnit.MDUnitName;
                        }
                    }
                    if (dns.InOrderPos != null)
                    {
                        dnsPreview.TargetQuantity = dns.InOrderPos.TargetQuantity;
                        dnsPreview.ActualQuantity = dns.InOrderPos.ActualQuantity;
                        if (dns.InOrderPos.MDUnit == null)
                        {
                            dnsPreview.MDUnitName = dns.InOrderPos.Material.BaseMDUnit.MDUnitName;
                        }
                        else
                        {
                            dnsPreview.MDUnitName = dns.InOrderPos.MDUnit.MDUnitName;
                        }
                    }

                    dnsPreview.MaterialNo = dns.Material.MaterialNo;
                    dnsPreview.MaterialName = dns.Material.MaterialName1;

                    DeliveryNoteList.Add(dnsPreview);
                }
            }

            if (point.Related is InOrderPos)
            {
                InOrderPos iop = point.Related as InOrderPos;
                if (iop.LabOrder_InOrderPos.Any())
                    ItemsWithLabOrder.Add(point);
            }

            if (point.Related is OutOrderPos)
            {
                OutOrderPos oop = point.Related as OutOrderPos;
                if (oop.LabOrder_OutOrderPos.Any())
                    ItemsWithLabOrder.Add(point);
            }

            if (point.Related is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos pos = point.Related as ProdOrderPartslistPos;
                if (pos.LabOrder_ProdOrderPartslistPos.Any())
                    ItemsWithLabOrder.Add(point);
            }
        }

        public void ApplyFilter()
        {
            if (Filter.Count != 0)
            {
                RootPoint.ApplyFilter(null, Filter);
            }
        }

        public List<string> TestLotNo
        {
            get
            {
                return RootPoint.Items.Select(x => (TrackingAndTracingPoint)x).Where(x => x.Related is FacilityCharge).Select(x => x.Related as FacilityLot).Select(x => x.LotNo).ToList();
            }
        }

        #endregion

        #region ToString HTML presentation

        public static string ToHtmlStringTemplate = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN""
        ""http://www.w3.org/TR/html4/loose.dtd"">
            <html lang=""en"">
            <head>
	            <meta http-equiv=""content-type"" content=""text/html; charset=utf-8"">
	            <title>Tracking and tracking result</title>
            </head>
            <body>
            <h1>T&T export - filtered by {0}</h1>
            <ul>{1}</ul>
            </body>
            </html>
            ";

        public override string ToString()
        {
            return "Result point" + RootPoint != null ? RootPoint.ToString() : "";
        }

        public string ToHTMLString()
        {
            string filteredBy = "";
            if (Filter.Any())
                filteredBy = string.Join(", ", Filter);
            filteredBy = filteredBy.TrimEnd(", ".ToCharArray());
            return string.Format(ToHtmlStringTemplate, filteredBy, RootPoint.ToHTMLString());
        }

        #endregion
    }
}
