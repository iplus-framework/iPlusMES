using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.reporthandler;
using gip.mes.datamodel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'PrintManager'}de{'PrintManager'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class ACPrintManager : PARole, IPrintManager
    {
        #region c'tors
        public ACPrintManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
           : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _ConfiguredPrinters = new ACPropertyConfigValue<string>(this, "ConfiguredPrinters", "");
        }
        public const string C_DefaultServiceACIdentifier = "ACPrintManager";
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

        #region Config
        private ACPropertyConfigValue<string> _ConfiguredPrinters;
        [ACPropertyConfig("en{'Configured printers'}de{'Konfigurierte Drucker'}")]
        public string ConfiguredPrinters
        {
            get
            {
                return _ConfiguredPrinters.ValueT;
            }
            set
            {
                _ConfiguredPrinters.ValueT = value;
            }
        }
        #endregion

        #region xx
        private ObservableCollection<PrinterInfo> _ConfiguredPrinterList;
        /// <summary>
        /// List property for PrinterInfo
        /// </summary>
        /// <value>The ConfiguredPrinters list</value>
        [ACPropertyList(9999, "ConfiguredPrinter")]
        public ObservableCollection<PrinterInfo> ConfiguredPrinterList
        {
            get
            {
                if (_ConfiguredPrinterList == null)
                {
                    if (string.IsNullOrEmpty(ConfiguredPrinters))
                        _ConfiguredPrinterList = new ObservableCollection<PrinterInfo>();
                    else
                    {
                        PrinterInfo[] tmpList = JsonConvert.DeserializeObject<PrinterInfo[]>(ConfiguredPrinters);
                        _ConfiguredPrinterList = new ObservableCollection<PrinterInfo>(tmpList);
                    }
                    _ConfiguredPrinterList.CollectionChanged += _ConfiguredPrinterList_CollectionChanged;
                }
                return _ConfiguredPrinterList;
            }
        }

        private void _ConfiguredPrinterList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_ConfiguredPrinterList.Any())
                ConfiguredPrinters = JsonConvert.SerializeObject(_ConfiguredPrinterList.ToArray());
            else
                ConfiguredPrinters = null;
        }

        #endregion

        #region Methods

        public Msg Print(PAOrderInfo pAOrderInfo, int copyCount)
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

                if (printerInfo != null)
                    msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = "Print(113) fail! No mandatory printer found!" };

                if (msg == null)
                    msg = bso.PrintViaOrderInfo(designName, printerInfo.PrinterName, (short)copyCount, pAOrderInfo);

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

        public PrinterInfo GetPrinterInfo(PAOrderInfo pAOrderInfo)
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

        private PrinterInfo GetPrinterInfo(Facility facility)
        {
            PrinterInfo printerInfo = ConfiguredPrinterList.FirstOrDefault(c => c.FacilityNo == facility.FacilityNo);
            if (printerInfo == null && facility.Facility1_ParentFacility != null)
                printerInfo = GetPrinterInfo(facility.Facility1_ParentFacility);
            return printerInfo;
        }

        #endregion
    }
}
