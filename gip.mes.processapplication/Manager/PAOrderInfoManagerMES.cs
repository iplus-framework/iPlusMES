using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.reporthandler;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.processapplication.Manager
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'PAOrderInfoManagerMES'}de{'PAOrderInfoManagerMES'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]

    public class PAOrderInfoManagerMES : PAOrderInfoManagerBase
    {
        #region const
        public string[] Const_SupportedEntityTypes = new string[] { FacilityCharge.ClassName };
        #endregion

        #region ctor's

        public PAOrderInfoManagerMES(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
           : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool initSuccess = base.ACInit(startChildMode);

            _PrintManager = ACPrintManager.ACRefToServiceInstance(this);
            if (_PrintManager == null)
                throw new Exception("ACPrintManager not configured");
            return initSuccess;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            bool deinitSuccess = base.ACDeInit(deleteACClassTask);

            if (_PrintManager != null)
                ACPrintManager.DetachACRefFromServiceInstance(this, _PrintManager);
            _PrintManager = null;

            return deinitSuccess;
        }

        #endregion

        #region Manager

        #region Managers

        protected ACRef<ACComponent> _PrintManager = null;
        protected ACComponent PrintManager
        {
            get
            {
                if (_PrintManager == null)
                    return null;
                return _PrintManager.ValueT;
            }
        }

        #endregion

        #endregion

        #region Methods


        public override bool IsResponsibleFor(PAOrderInfo pAOrderInfo)
        {
            return
                pAOrderInfo != null
                && pAOrderInfo.Entities != null
                && pAOrderInfo.Entities.ToArray().Select(c => c.EntityName).Intersect(Const_SupportedEntityTypes).Any();
        }

        public override PAOrderInfoDestination GetOrderInfoDestination(PAOrderInfo pAOrderInfo)
        {
            PAOrderInfoDestination destination = null;
            if (pAOrderInfo.Entities.Any(c => c.EntityName == FacilityCharge.ClassName))
            {
                destination = new PAOrderInfoDestination()
                {
                    BSOACUrl = "\\Businessobjects#BSOFacilityBookCharge",
                    ReportACIdentifier = "LabelQR"
                };
            }
            return destination;
        }

        public override PrinterInfo GetPrinterInfo(PAOrderInfo pAOrderInfo)
        {
            Guid? facilityID = null;
            Guid? aCClassID = null;
            if (pAOrderInfo.Entities.Any(c => c.EntityName == Facility.ClassName))
                facilityID = pAOrderInfo.Entities.Where(c => c.EntityName == Facility.ClassName).Select(c => c.EntityID).FirstOrDefault();
            else if (pAOrderInfo.Entities.Any(c => c.EntityName == FacilityCharge.ClassName))
            {
                Guid facilityChargeID = pAOrderInfo.Entities.Where(c => c.EntityName == FacilityCharge.ClassName).Select(c => c.EntityID).FirstOrDefault();
                FacilityCharge facilityCharge = (Database as gip.mes.datamodel.DatabaseApp).FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == facilityChargeID);
                facilityID = facilityCharge.FacilityID;
            }
            else if (pAOrderInfo.Entities.Any(c => c.EntityName == gip.core.datamodel.ACClass.ClassName))
                aCClassID = pAOrderInfo.Entities.Where(c => c.EntityName == gip.core.datamodel.ACClass.ClassName).Select(c => c.EntityID).FirstOrDefault();


            PrinterInfo printerInfo = null;
            gip.core.datamodel.ACClass aCClass = null;
            List<PrinterInfo> configuredPrinters = null;
            using (Database database = new core.datamodel.Database())
            {
                configuredPrinters = ACPrintManager.GetConfiguredPrinters(database, PrintManager.ComponentClass.ACClassID, false);
                if (aCClassID != null)
                    aCClass = database.ACClass.FirstOrDefault();
            }


            if (facilityID != null)
                printerInfo = GetPrinterInfoFromFacility(facilityID, configuredPrinters);
            else
                printerInfo = GetPrinterInfoFromMachine(aCClass, configuredPrinters);
            return printerInfo;
        }

        private PrinterInfo GetPrinterInfoFromFacility(Guid? facilityID, List<PrinterInfo> configuredPrinters)
        {
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                Facility facility = databaseApp.Facility.FirstOrDefault(c => c.FacilityID == facilityID);
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

        private PrinterInfo GetPrinterInfoFromMachine(gip.core.datamodel.ACClass acClass, List<PrinterInfo> configuredPrinters)
        {
            return configuredPrinters.FirstOrDefault(c => c.MachineACUrl == acClass.ACURLCached);
        }

        #endregion


    }
}
