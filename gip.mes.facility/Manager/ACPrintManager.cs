using gip.bso.iplus;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Linq;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'ACPrintManager'}de{'ACPrintManager'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class ACPrintManager : PARole, IPrintManager
    {

        #region const
        public const string Const_KeyACUrl_ConfiguredPrintersConfig = ".\\ACClassProperty(ConfiguredPrintersConfig)";
        #endregion

        #region c'tors
        public ACPrintManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
           : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }
        public const string C_DefaultServiceACIdentifier = "ACPrintManager";


        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseACInit = base.ACInit(startChildMode);
            return baseACInit;
        }
        #endregion

        #region Attach / Deattach
        public static ACPrintManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACPrintManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ACPrintManager> ACRefToServiceInstance(ACComponent requester)
        {
            ACPrintManager serviceInstance = GetServiceInstance(requester) as ACPrintManager;
            if (serviceInstance != null)
                return new ACRef<ACPrintManager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region Properties

        #region Properties -> Project Manager

        ACProjectManager _ACProjectManager;
        public ACProjectManager ProjectManager
        {
            get
            {
                if (_ACProjectManager != null)
                    return _ACProjectManager;
                _ACProjectManager = new ACProjectManager(Database, Root);
                return _ACProjectManager;
            }
        }

        #endregion

        #region Properties -> ConfiguredPrinters

        [ACPropertyPointConfig(9999, "", typeof(PrinterInfo), "en{'Configured printers'}de{'Konfigurierte Drucker'}")]
        public List<gip.core.datamodel.ACClassConfig> ConfiguredPrintersConfig
        {
            get
            {
                List<gip.core.datamodel.ACClassConfig> result = null;
                ACClassTaskQueue.TaskQueue.ProcessAction(() =>
                {
                    try
                    {
                        ACTypeFromLiveContext.ACClassConfig_ACClass.Load(MergeOption.OverwriteChanges);
                        var query = ACTypeFromLiveContext.ACClassConfig_ACClass.Where(c => c.KeyACUrl == Const_KeyACUrl_ConfiguredPrintersConfig);
                        if (query.Any())
                            result = query.ToList();
                        else
                            result = new List<gip.core.datamodel.ACClassConfig>();
                    }
                    catch (Exception e)
                    {
                        Messages.LogException(this.GetACUrl(), "ConfiguredPrintersConfig", e.Message);
                    }
                });
                return result;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Methods -> Public

        public virtual Msg Print(PAOrderInfo pAOrderInfo, int copyCount)
        {
            Msg msg = null;
            try
            {
                string bsoName = "";
                string designName = "";
                if (pAOrderInfo.Entities.Any(c => c.EntityName == FacilityCharge.ClassName))
                {
                    bsoName = "\\Businessobjects#BSOFacilityBookCharge";
                    designName = "LabelQR";
                }
                ACBSO bso = this.Root.ACUrlCommand(bsoName) as ACBSO;
                PrinterInfo printerInfo = GetPrinterInfo(pAOrderInfo);

                if (bso == null)
                    msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = "Print(110) fail! No mandatory BSO found!" };

                if (printerInfo == null)
                    msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = "Print(113) fail! No mandatory printer found!" };

                if (msg == null)
                {
                    msg = bso.SetDataFromPAOrderInfo(pAOrderInfo);
                    if (msg == null)
                        msg = bso.PrintViaOrderInfo(designName, printerInfo.PrinterName, (short)copyCount);
                }

                if (msg != null)
                    Root.Messages.LogError(this.GetACUrl(), "Print(119)", msg.Message);
            }
            catch (Exception ex)
            {
                msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = "Print(120) fail! Error: " + ex.Message };
                Root.Messages.LogException(ACPrintManager.C_DefaultServiceACIdentifier, "Print(125)", ex);
            }
            return msg;
        }

        public virtual PrinterInfo GetPrinterInfo(PAOrderInfo pAOrderInfo)
        {
            PrinterInfo printerInfo = null;
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                if (pAOrderInfo.Entities.Any(c => c.EntityName == FacilityCharge.ClassName))
                {
                    PAOrderInfoEntry entry = pAOrderInfo.Entities.FirstOrDefault(c => c.EntityName == FacilityCharge.ClassName);
                    FacilityCharge facilityCharge = databaseApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == entry.EntityID);
                    printerInfo = GetPrinterInfo(facilityCharge.Facility);
                }
            }
            return printerInfo;
        }

        public List<PrinterInfo> GetPrintServers()
        {
            gip.core.datamodel.ACClass basePrintServerClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(ACPrintServerBase));
            IQueryable<gip.core.datamodel.ACClass> queryClasses = FacilityManager.s_cQry_GetAvailableModulesAsACClass(Database.ContextIPlus, basePrintServerClass.ACIdentifier);
            List<PrinterInfo> printServers = new List<PrinterInfo>();
            if (queryClasses != null && queryClasses.Any())
            {
                gip.core.datamodel.ACClass[] acClasses = queryClasses.ToArray();
                foreach (gip.core.datamodel.ACClass aCClass in acClasses)
                {
                    PrinterInfo printerInfo = new PrinterInfo();
                    printerInfo.Name = aCClass.ACIdentifier;
                    printerInfo.PrinterACUrl = ACItem.FactoryACUrlComponent(aCClass);
                    printServers.Add(printerInfo);
                }
            }
            return printServers;
        }

        #endregion

        #region Methods -> Private

        private PrinterInfo GetPrinterInfo(Facility facility)
        {
            PrinterInfo printerInfo = ConfiguredPrintersConfig.Select(c => c.Value as PrinterInfo).FirstOrDefault(c => c.FacilityNo == facility.FacilityNo);
            if (printerInfo == null && facility.Facility1_ParentFacility != null)
                printerInfo = GetPrinterInfo(facility.Facility1_ParentFacility);
            return printerInfo;
        }

        #endregion

        #endregion
    }
}
