using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.manager;
using System;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using gip.core.reporthandler;
using System.Collections.Generic;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ACPrintManagerMES'}de{'ACPrintManagerMES'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class ACPrintManagerMES : ACPrintManager
    {
        #region c´tors
        public ACPrintManagerMES(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public const string ClassNameMES = "ACPrintManagerMES";

        #endregion

        #region Methods
        protected override PrinterInfo OnGetPrinterInfo(PAOrderInfo pAOrderInfo, string vbUserName)
        {
            Guid? facilityID = null;
            Guid? aCClassID = null;

            PrinterInfo printerInfo = null;
            gip.core.datamodel.ACClass aCClass = null;
            List<PrinterInfo> configuredPrinters = null;
            using (Database database = new core.datamodel.Database())
            {
                using (DatabaseApp dbApp = new DatabaseApp(database))
                {
                    if (pAOrderInfo.Entities.Any(c => c.EntityName == Facility.ClassName))
                        facilityID = pAOrderInfo.Entities.Where(c => c.EntityName == Facility.ClassName).Select(c => c.EntityID).FirstOrDefault();
                    else if (pAOrderInfo.Entities.Any(c => c.EntityName == FacilityCharge.ClassName))
                    {
                        Guid facilityChargeID = pAOrderInfo.Entities.Where(c => c.EntityName == FacilityCharge.ClassName).Select(c => c.EntityID).FirstOrDefault();
                        FacilityCharge facilityCharge = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == facilityChargeID);
                        if (facilityCharge != null)
                            facilityID = facilityCharge.FacilityID;
                    }
                    // Workplace
                    if (pAOrderInfo.Entities.Any(c => c.EntityName == gip.core.datamodel.ACClass.ClassName))
                        aCClassID = pAOrderInfo.Entities.Where(c => c.EntityName == gip.core.datamodel.ACClass.ClassName).Select(c => c.EntityID).FirstOrDefault();

                    configuredPrinters = ACPrintManager.GetConfiguredPrinters(database, ComponentClass.ACClassID, false);

                    if (!string.IsNullOrEmpty(vbUserName))
                    {
                        core.datamodel.VBUser vbUser = database.VBUser.FirstOrDefault(c => c.VBUserName == vbUserName);
                        if (vbUser != null)
                        {
                            PrinterInfo printerForUser = configuredPrinters.FirstOrDefault(c => c.VBUserID == vbUser.VBUserID);
                            if (printerForUser != null)
                                return printerForUser;
                        }
                    }

                    if (aCClassID != null)
                        aCClass = database.ACClass.FirstOrDefault(c => c.ACClassID == aCClassID);
                }
            }
            if (configuredPrinters == null || !configuredPrinters.Any())
                return null;

            if (facilityID != null)
                printerInfo = GetPrinterInfoFromFacility(facilityID, configuredPrinters);
            if (printerInfo == null || aCClass != null)
            {
                PrinterInfo printerInfo2 = GetPrinterInfoFromMachine(aCClass, configuredPrinters, aCClass != null);
                if (printerInfo2 != null)
                    printerInfo = printerInfo2;
            }
            return printerInfo;
        }

        private PrinterInfo GetPrinterInfoFromFacility(Guid? facilityID, List<PrinterInfo> configuredPrinters)
        {
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                Facility facility = databaseApp.Facility.FirstOrDefault(c => c.FacilityID == facilityID);
                if (facility == null)
                    return null;
                return GetPrinterInfoFromFacility(facility, configuredPrinters);
            }
        }

        private PrinterInfo GetPrinterInfoFromFacility(Facility facility, List<PrinterInfo> configuredPrinters)
        {
            PrinterInfo printerInfo = configuredPrinters.FirstOrDefault(c => c.FacilityID == facility.FacilityID);
            if (printerInfo == null && facility.Facility1_ParentFacility != null)
                printerInfo = GetPrinterInfoFromFacility(facility.Facility1_ParentFacility, configuredPrinters);
            return printerInfo;
        }


        protected override core.datamodel.ACClass OnResolveBSOForOrderInfo(PAOrderInfo pAOrderInfo)
        {
            if (pAOrderInfo != null)
            {
                PAOrderInfoEntry batchEntry = pAOrderInfo.Entities.Where(c => c.EntityName == ProdOrderBatch.ClassName).FirstOrDefault();
                if (batchEntry != null)
                {
                    using (DatabaseApp dbApp = new DatabaseApp())
                    {
                        Guid? facilityChargeID =
                        dbApp.FacilityBookingCharge
                            .Where(c => c.ProdOrderPartslistPos != null
                                        && c.ProdOrderPartslistPos.ProdOrderBatchID.HasValue
                                        && c.ProdOrderPartslistPos.ProdOrderBatchID == batchEntry.EntityID
                                        && c.InwardFacilityChargeID.HasValue)
                            .Select(c => c.InwardFacilityChargeID)
                            .FirstOrDefault();
                        if (facilityChargeID.HasValue)
                        {
                            pAOrderInfo.Entities.Remove(batchEntry);
                            pAOrderInfo.Entities.Insert(0, new PAOrderInfoEntry(FacilityCharge.ClassName, facilityChargeID.Value));
                        }
                    }
                }
            }
            return base.OnResolveBSOForOrderInfo(pAOrderInfo);
        }

        #endregion

    }
}
