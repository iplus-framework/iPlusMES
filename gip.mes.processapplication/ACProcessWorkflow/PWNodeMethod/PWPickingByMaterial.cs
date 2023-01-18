using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Picking by material'}de{'Kommissionierung nach Material'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWPickingByMaterial : PWNodeProcessMethod
    {
        #region c'tors

        static PWPickingByMaterial()
        {
            var wrapper = CreateACMethodWrapper(typeof(PWPickingByMaterial));
            ACMethod.RegisterVirtualMethod(typeof(PWPickingByMaterial), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWPickingByMaterial), HandleExecuteACMethod_PWPickingByMaterial);
        }

        public PWPickingByMaterial(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
               base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        protected ACPickingManager PickingManager
        {
            get
            {
                PWMethodRelocation pwMethodProduction = ParentRootWFNode as PWMethodRelocation;
                return pwMethodProduction != null ? pwMethodProduction.PickingManager : null;
            }
        }

        #endregion

        #region Methods

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            base.SMStarting();

            ACMethod myConfig = MyConfiguration;
            if (myConfig != null)
            {
                PWMethodRelocation relocation = ParentRootWFNode as PWMethodRelocation;
                if (relocation != null)
                {
                    Picking picking = relocation.CurrentPicking;

                    myConfig.ParameterValueList["FromDT"] = picking.DeliveryDateFrom;
                    myConfig.ParameterValueList["ToDT"] = picking.DeliveryDateTo;
                }
            }
        }

        public override void SMRunning()
        {
            base.SMRunning();

            ACMethod myConfig = MyConfiguration;
            if (myConfig != null)
            {
                PWMethodRelocation relocation = ParentRootWFNode as PWMethodRelocation;
                if (relocation != null)
                {
                    Picking picking = relocation.CurrentPicking;

                    myConfig.ParameterValueList["FromDT"] = picking.DeliveryDateFrom;
                    myConfig.ParameterValueList["ToDT"] = picking.DeliveryDateTo;
                }
            }
        }

        public override void SMCompleted()
        {
            PWMethodRelocation relocation = ParentRootWFNode as PWMethodRelocation;
            if (relocation != null)
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    PickingPos currentPickingPos = relocation.CurrentPickingPos.FromAppContext<PickingPos>(dbApp);
                    MDDelivPosLoadState finishedState = dbApp.MDDelivPosLoadState.FirstOrDefault(c => c.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck);
                    if (currentPickingPos != null && finishedState != null)
                    {
                        currentPickingPos.MDDelivPosLoadState = finishedState;

                        Msg msg = dbApp.ACSaveChanges();
                        if (msg != null)
                        {
                            OnNewAlarmOccurred(ProcessAlarm, msg);
                            Messages.LogMessageMsg(msg);
                        }
                    }
                }
            }

            base.SMCompleted();
        }

        [ACMethodInfo("", "", 9999)]
        public ACValueList GetRoutableFacilities(Guid pPosID)
        {
            using (Database db = new Database())
            using (DatabaseApp dbApp = new DatabaseApp(db))
            {
                PickingPos pos = dbApp.PickingPos.FirstOrDefault(c => c.PickingPosID == pPosID);
                if (pos != null)
                {
                    IEnumerable<Facility> facilities = GetAvailableFacilitiesForMaterial(dbApp, pos);
                    return new ACValueList(facilities.Select(c => new ACValue("ID", c.FacilityID)).ToArray());
                }
                return null;
            }

        }

        public IEnumerable<Facility> GetAvailableFacilitiesForMaterial(DatabaseApp dbApp, PickingPos pos)
        {
            if (ParentPWGroup == null || ParentPWGroup.AccessedProcessModule == null || PickingManager == null)
            {
                throw new NullReferenceException("AccessedProcessModule is null");
            }

            IList<Facility> facilities;

            core.datamodel.ACClass accessAClass = ParentPWGroup.AccessedProcessModule.ComponentClass;
            IEnumerable<Route> routes = PickingManager.GetRoutes(pos, dbApp, dbApp.ContextIPlus,
                                                                    accessAClass,
                                                                    ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                                                    null,
                                                                    out facilities,
                                                                    null,
                                                                    null,
                                                                    null,
                                                                    false);

            if (routes == null || facilities == null)
                return new List<Facility>();

            var routeList = routes.ToList();

            List<Facility> routableFacilities = new List<Facility>();

            PAFManualWeighing manWeigh = CurrentExecutingFunction<PAFManualWeighing>();
            if (manWeigh == null)
            {
                core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
                ACMethod acMethod = refPAACClassMethod?.TypeACSignature();
                if (acMethod != null)
                {
                    PAProcessModule module = ParentPWGroup.AccessedProcessModule;
                    manWeigh = CanStartProcessFunc(module, acMethod) as PAFManualWeighing;
                }
            }

            foreach (Route currRoute in routes)
            {
                RouteItem lastRouteItem = currRoute.Items.LastOrDefault();
                if (lastRouteItem != null && lastRouteItem.TargetProperty != null)
                {
                    // Gehe zur nächsten Komponente, weil es mehrere Dosierfunktionen gibt und der Eingangspunkt des Prozessmoduls nicht mit dem Eingangspunkt dieser Funktion übereinstimmt.
                    // => eine andere Funktion ist dafür zuständig
                    if (manWeigh != null && !manWeigh.PAPointMatIn1.ConnectionList.Where(c => ((c as PAEdge).Source as PAPoint).ACIdentifier == lastRouteItem.TargetProperty.ACIdentifier).Any())
                    {
                        routeList.Remove(currRoute);
                    }
                    else
                    {
                        RouteItem source = currRoute.GetRouteSource();
                        if (source != null)
                        {
                            Facility facilityToAdd = facilities.FirstOrDefault(c => c.VBiFacilityACClassID.HasValue && c.VBiFacilityACClassID == source.SourceGuid);
                            if (facilityToAdd != null)
                                routableFacilities.Add(facilityToAdd);
                        }
                    }
                }
            }

            //if (!IncludeContainerStores)
            //{
            //    routableFacilities = routableFacilities.Where(c => c.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBin).ToList();
            //}

            return routableFacilities;
        }



        protected static ACMethodWrapper CreateACMethodWrapper(Type thisType)
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue("PickingType", typeof(string), "", Global.ParamOption.Optional));
            paramTranslation.Add("PickingType", "en{'Picking type'}de{'Picking type'}");

            method.ParameterValueList.Add(new ACValue("SourceFacilityNo", typeof(string), "", Global.ParamOption.Optional));
            paramTranslation.Add("SourceFacilityNo", "en{'Source facility No'}de{'Source facility No'}");

            method.ParameterValueList.Add(new ACValue("AutoPrintOnPosting", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("AutoPrintOnPosting", "en{'Auto print on posting'}de{'Auto print on posting'}");

            method.ParameterValueList.Add(new ACValue("FromDT", typeof(DateTime), DateTime.MinValue, Global.ParamOption.Optional));
            paramTranslation.Add("FromDT", "en{'From date'}de{'From date'}");

            method.ParameterValueList.Add(new ACValue("ToDT", typeof(DateTime), DateTime.MinValue, Global.ParamOption.Optional));
            paramTranslation.Add("ToDT", "en{'To date'}de{'To date'}");

            return new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", thisType, paramTranslation, null);
        }


        private static bool HandleExecuteACMethod_PWPickingByMaterial(out object result, IACComponent acComponent, string acMethodName, core.datamodel.ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PWBaseNodeProcess(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
