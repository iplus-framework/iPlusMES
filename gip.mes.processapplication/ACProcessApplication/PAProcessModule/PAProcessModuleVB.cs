using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.core.processapplication;

namespace gip.mes.processapplication
{
    /// <summary>
    /// A process module that can handle application data (production order, picking order, ...). 
    /// End users interact with process modules via the user interface in order to access the application data and manage the data using business objects.
    /// </summary>
    /// <seealso cref="gip.core.autocomponent.PAProcessModule" />
    /// <seealso cref="gip.core.processapplication.IPAOEEProvider" />
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Processmodule MES'}de{'Prozessmodul MES'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public abstract class PAProcessModuleVB : PAProcessModule, IPAOEEProvider
    {
        #region c'tors
        static PAProcessModuleVB()
        {
            RegisterExecuteHandler(typeof(PAProcessModuleVB), HandleExecuteACMethod_PAProcessModuleVB);
        }

        public PAProcessModuleVB(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _ACUrlExtraDisDest = new ACPropertyConfigValue<string>(this, nameof(ACUrlExtraDisDest), "");
            _ReworkEnabled = new ACPropertyConfigValue<bool>(this, nameof(ReworkEnabled), false);
        }

        public override bool ACPostInit()
        {
            _ = ReworkEnabled;
            _ = ACUrlExtraDisDest;
            return base.ACPostInit();
        }
        #endregion

        #region Config
        private ACPropertyConfigValue<string> _ACUrlExtraDisDest;
        [ACPropertyConfig("en{'Extra destitnation ACUrl'}de{'Sonderentleerziel ACUrl'}")]
        public virtual string ACUrlExtraDisDest
        {
            get
            {
                return _ACUrlExtraDisDest.ValueT;
            }
        }

        private ACPropertyConfigValue<bool> _ReworkEnabled;
        [ACPropertyConfig("en{'Rework enabled'}de{'Rework aktiviert'}")]
        public bool ReworkEnabled
        {
            get
            {
                return _ReworkEnabled.ValueT;
            }
        }

        #endregion

        #region Properties

        [ACPropertyBindingSource(9999, "", "", "", false, true)]
        public IACContainerTNet<ACRef<ProdOrderPartslistPos>> CurrentBatchPos { get; set; }

        [ACPropertyBindingSource(9999, "", "", "", false, true)]
        public IACContainerTNet<ACRef<DeliveryNotePos>> CurrentDeliveryNotePos { get; set; }

        //
        [ACPropertyBindingSource(303, "Info", "en{'Reservation-Info'}de{'Reservierungsinformation'}", "", false, false)]
        public IACContainerTNet<string> OrderReservationInfo { get; set; }

        public virtual bool IsOccupied
        {
            get
            {
                if (CurrentBatchPos.ValueT != null && CurrentBatchPos.ValueT.ValueT != null)
                    return true;
                else if (CurrentDeliveryNotePos.ValueT != null && CurrentDeliveryNotePos.ValueT.ValueT != null)
                    return true;
                else if (this.Semaphore.ConnectionListCount > 0)
                    return true;
                return false;
            }
        }

        [ACPropertyBindingTarget(400, "Read from PLC", "en{'Availability-State for OEE'}de{'Verfügbarkeitsstatus für OEE'}", "", false, false)]
        public IACContainerTNet<AvailabilityState> AvailabilityState { get; set; }


        #endregion

        #region Methods

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "GetBSONameForShowOrder":
                    result = GetBSONameForShowOrder(acParameter[0] as string);
                    return true;
                case "GetBSONameForShowReservation":
                    result = GetBSONameForShowReservation(acParameter[0] as string);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }


        public static bool HandleExecuteACMethod_PAProcessModuleVB(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "ShowOrderDialog":
                    ShowOrderDialog(acComponent);
                    return true;
                case "ShowReservationDialog":
                    ShowReservationDialog(acComponent);
                    return true;
                case Const.IsEnabledPrefix + "ShowOrderDialog":
                    result = IsEnabledShowOrderDialog(acComponent);
                    return true;
                case Const.IsEnabledPrefix + "ShowReservationDialog":
                    result = IsEnabledShowReservationDialog(acComponent);
                    return true;
                case "ShowLabOrderDialog":
                    ShowLabOrderDialog(acComponent);
                    return true;
                case "NewLabOrderDialog":
                    NewLabOrderDialog(acComponent);
                    return true;
                case Const.IsEnabledPrefix + "ShowLabOrderDialog":
                    result = IsEnabledShowLabOrderDialog(acComponent);
                    return true;
                case Const.IsEnabledPrefix + "NewLabOrderDialog":
                    result = IsEnabledNewLabOrderDialog(acComponent);
                    return true;
            }
            return HandleExecuteACMethod_PAProcessModule(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region Instance
        public override void Reset()
        {
            base.Reset();
            if (!Semaphore.LocalStorage.Any() && !String.IsNullOrEmpty(OrderInfo.ValueT))
            {
                OrderInfo.ValueT = "";
                Allocated.ValueT = false;
            }

            if (OrderReservationInfo.ValueT != null)
                OrderReservationInfo.ValueT = null;
        }

        [ACMethodInfo("","",9999)]
        public virtual string GetBSONameForShowOrder(string defaultBSOName)
        {
            return defaultBSOName;
        }

        [ACMethodInfo("", "", 9999)]
        public virtual string GetBSONameForShowReservation(string defaultBSOName)
        {
            return defaultBSOName;
        }

        [ACMethodInfo("", "", 9999)]
        public virtual SingleDosingItems GetDosableComponents(bool withFacilityCombination = true)
        {
            using (Database db = new gip.core.datamodel.Database())
            {
                core.datamodel.ACClass compClass = null;

                using (ACMonitor.Lock(core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                {
                    compClass = ComponentClass.FromIPlusContext<core.datamodel.ACClass>(db);
                }

                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    RoutingService = this.RoutingService,
                    Database = db,
                    SelectionRuleID = PAMSilo.SelRuleID_Silo,
                    Direction = RouteDirections.Backwards,
                    MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                    IncludeReserved = true,
                    IncludeAllocated = true
                };

                RoutingResult rResult = ACRoutingService.FindSuccessors(compClass, routingParameters);

                if (rResult == null)
                {
                    return new SingleDosingItems() { Error = new Msg(eMsgLevel.Error, "Routing result is null!") };
                }

                if ( rResult.Message != null && rResult.Message.MessageLevel == eMsgLevel.Error)
                {
                    return new SingleDosingItems() { Error = rResult.Message };
                }

                SingleDosingItems result = new SingleDosingItems();

                foreach (Route route in rResult.Routes)
                {
                    RouteItem rItem = route.GetRouteSource();
                    if (rItem == null)
                        continue;

                    PAMSilo silo = rItem.SourceACComponent as PAMSilo;
                    if (silo == null || string.IsNullOrEmpty(silo.Facility?.ValueT?.ValueT?.FacilityNo) || string.IsNullOrEmpty(silo.MaterialNo?.ValueT))
                        continue;

                    if (!withFacilityCombination && result.Any(c => c.MaterialNo == silo.MaterialNo.ValueT))
                        continue;

                    result.Add(new SingleDosingItem() {  FacilityNo = withFacilityCombination ? silo.Facility.ValueT.ValueT.FacilityNo : "", 
                                                         MaterialName = silo.MaterialName?.ValueT, 
                                                         MaterialNo = silo.MaterialNo.ValueT });
                }

                return result;
            }
        }

        public override void RefreshOrderInfo()
        {
            base.RefreshOrderInfo();
        }

        #endregion

        #region Helper-Methods
        #endregion

        #endregion

        #region Client-Methods

        #region Prodorder
        [ACMethodInteractionClient("", "en{'View order'}de{'Auftrag anschauen'}", 450, false, "", false, Global.ContextMenuCategory.ProdPlanLog)]
        public static void ShowOrderDialog(IACComponent acComponent)
        {
            if (!IsEnabledShowOrderDialog(acComponent))
                return;
            PAShowDlgManagerBase serviceInstance = PAShowDlgManagerBase.GetServiceInstance(acComponent.Root as ACComponent);
            if (serviceInstance == null)
                return;
            serviceInstance.ShowDialogOrder(acComponent);
        }

        public static bool IsEnabledShowOrderDialog(IACComponent acComponent)
        {
            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerBase.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return false;
            return serviceInstance.IsEnabledShowDialogOrder(acComponent);
        }
        #endregion

        #region Reservation
        [ACMethodInteractionClient("", "en{'View reservation'}de{'Reservierungen anschauen'}", 450,false, "", false, Global.ContextMenuCategory.ProdPlanLog)]
        public static void ShowReservationDialog(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            if (!IsEnabledShowReservationDialog(acComponent))
                return;

            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerVB.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return;
            serviceInstance.ShowReservationDialog(acComponent);

            return;

        }

        public static bool IsEnabledShowReservationDialog(IACComponent acComponent)
        {
            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerVB.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return false;
            return serviceInstance.IsEnabledShowReservationDialog(acComponent);
        }
        #endregion

        #region Laborder
        [ACMethodInteractionClient("", "en{'View labroratory order'}de{'Laboraufträge anschauen'}", 451, false, "", false, Global.ContextMenuCategory.ProdPlanLog)]
        public static void ShowLabOrderDialog(IACComponent acComponent)
        {
            if (!IsEnabledShowOrderDialog(acComponent))
                return;
            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerBase.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return;
            serviceInstance.ShowLabOrder(acComponent);
        }

        public static bool IsEnabledShowLabOrderDialog(IACComponent acComponent)
        {
            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerBase.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return false;
            return serviceInstance.IsEnabledShowLabOrder(acComponent);
        }


        [ACMethodInteractionClient("", "en{'New labroratory order'}de{'Neuer Laborauftrag'}", 452, false, "", false, Global.ContextMenuCategory.ProdPlanLog)]
        public static void NewLabOrderDialog(IACComponent acComponent)
        {
            if (!IsEnabledShowOrderDialog(acComponent))
                return;
            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerBase.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return;
            serviceInstance.GenerateNewLabOrder(acComponent);
        }

        public static bool IsEnabledNewLabOrderDialog(IACComponent acComponent)
        {
            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerBase.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return false;
            return serviceInstance.IsEnabledGenerateNewLabOrder(acComponent);
        }
        #endregion

        #endregion

    }
}
