using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Work center child'}de{'Work center child'}", Global.ACKinds.TACAbstractClass, Global.ACStorableTypes.NotStorable, true, true)]
    public abstract class BSOWorkCenterChild : ACBSOvb
    {
        #region c'tors

        public BSOWorkCenterChild(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _BookParamRelocation = null;
            _BookParamRelocationClone = null;

            return base.ACDeInit(deleteACClassTask);
        }

        public const string Const_ACClassWFID = "PickingACClassWFID";

        #endregion

        #region Properties

        public WorkCenterItemFunction ItemFunction
        {
            get;
            set;
        }

        [ACPropertyInfo(510)]
        public BSOWorkCenterSelector ParentBSOWCS
        {
            get
            {
                return ParentACComponent as BSOWorkCenterSelector;
            }
        }

        public virtual ACComponent CurrentProcessModule
        {
            get;
            protected set;
        }

        #region Properties => Start workflow picking

        /// <summary>
        /// The _ facility manager
        /// </summary>
        protected ACRef<ACComponent> _ACFacilityManager = null;
        public FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }

        protected ACRef<ACPickingManager> _ACPickingManager = null;
        public ACPickingManager ACPickingManager
        {
            get
            {
                if (_ACPickingManager == null)
                    return null;
                return _ACPickingManager.ValueT;
            }
        }

        protected ACRef<ACComponent> _RoutingService = null;
        public ACComponent RoutingService
        {
            get
            {
                if (_RoutingService == null)
                    return null;
                return _RoutingService.ValueT;
            }
        }

        public bool IsRoutingServiceAvailable
        {
            get
            {
                return RoutingService != null && RoutingService.ConnectionState != ACObjectConnectionState.DisConnected;
            }
        }

        /// <summary>
        /// The _ book param relocation
        /// </summary>
        ACMethodBooking _BookParamRelocation;
        ACMethodBooking _BookParamRelocationClone;

        /// <summary>
        /// Gets the current book param relocation.
        /// </summary>
        /// <value>The current book param relocation.</value>
        [ACPropertyCurrent(704, "BookParamRelocation")]
        public ACMethodBooking CurrentBookParamRelocation
        {
            get
            {
                return _BookParamRelocation;
            }
            protected set
            {
                _BookParamRelocation = value;
                OnPropertyChanged("CurrentBookParamRelocation");
            }
        }

        #endregion

        #endregion

        #region Methods

        public virtual void Activate(ACComponent selectedProcessModule)
        {
            CurrentProcessModule = selectedProcessModule;
        }

        public virtual void DeActivate()
        {
            ClearBookingData();

            if (_RoutingService != null)
            {
                _RoutingService.Detach();
                _RoutingService = null;
            }

            if (_ACFacilityManager != null)
            {
                _ACFacilityManager.Detach();
                _ACFacilityManager = null;
            }

            if (_ACPickingManager != null)
            {
                _ACPickingManager.Detach();
                _ACPickingManager = null;
            }

            CurrentProcessModule = null;
        }

        #region Methods => Start workflow picking

        public bool RunWorkflow(DatabaseApp dbApp, core.datamodel.ACClassWF workflow, core.datamodel.ACClassMethod acClassMethod, ACComponent processModule, bool sourceFacilityValidation = true,
                                bool skipProcessModuleValidation = false, PARole.ValidationBehaviour validationBehaviour = PARole.ValidationBehaviour.Strict, bool onlyPreparePicking = false)
        {
            bool wfRunsBatches = false;
            ACComponent appManager = null;
            Route validRoute = null;

            if (processModule == null)
                return false;

            string orderReservationInfo = null;
            if (!skipProcessModuleValidation)
            {
                string orderInfo = processModule.ACUrlCommand(nameof(PAProcessModule.OrderInfo)) as string;

                if (!string.IsNullOrEmpty(orderInfo))
                {
                    //Question50075: The process module is occupied with order {0}. Are you sure that you want continue?
                    if (Messages.Question(this, "Question50075", Global.MsgResult.Yes, false, orderInfo) != Global.MsgResult.Yes)
                    {
                        return false;
                    }
                }

                orderReservationInfo = processModule.ACUrlCommand(nameof(PAProcessModule.ReservationInfo)) as string;
                if (!string.IsNullOrEmpty(orderReservationInfo))
                {
                    //Question50076: The process module is reserved for order {0}. Are you sure that you want continue?
                    if (Messages.Question(this, "Question50076",
                        Global.MsgResult.Yes, false, orderReservationInfo) != Global.MsgResult.Yes)
                    {
                        return false;
                    }
                }
            }

            if (!PrepareStartWorkflow(CurrentBookParamRelocation, acClassMethod, out wfRunsBatches, out appManager, out validRoute, workflow, sourceFacilityValidation))
            {
                ClearBookingData();
                return false;
            }

            if (ACPickingManager == null)
            {
                ClearBookingData();
                return false;
            }

            Picking picking = null;
            MsgWithDetails msgDetails = ACPickingManager.CreateNewPicking(CurrentBookParamRelocation, acClassMethod, dbApp, dbApp.ContextIPlus, true, out picking);
            if (msgDetails != null && msgDetails.MsgDetailsCount > 0)
            {
                Messages.Msg(msgDetails);
                ClearBookingData();
                dbApp.ACUndoChanges();
                return false;
            }
            if (picking == null)
            {
                dbApp.ACUndoChanges();
                ClearBookingData();
                return false;
            }
            dbApp.ACSaveChanges();

            bool result = PreStartWorkflow(dbApp, validRoute, workflow, picking);
            if (!result)
            {
                ClearBookingData();
                return false;
            }

            List<IACConfigStore> configStores = null;

            var configManager = ConfigManagerIPlus.GetServiceInstance(this);
            if (configManager != null)
                configStores = configManager.GetACConfigStores(new List<IACConfigStore>() { picking, workflow.ACClassMethod, workflow.RefPAACClassMethod });

            msgDetails = ACPickingManager.ValidateStart(dbApp, dbApp.ContextIPlus, picking, configStores, validationBehaviour, workflow);
            if (msgDetails != null && msgDetails.MsgDetailsCount > 0)
            {
                Messages.Msg(msgDetails);
                ClearBookingData();
                return false;
            }

            if (onlyPreparePicking)
            {
                PickingConfig pConfig = PickingConfig.NewACObject(dbApp, picking);
                pConfig.KeyACUrl = Const_ACClassWFID;
                pConfig.VBiValueTypeACClassID = dbApp.ContextIPlus.GetACType(typeof(string)).ACClassID;
                pConfig.XMLConfig = workflow.ACClassWFID.ToString();

                dbApp.ACSaveChanges();

                return true;
            }

            return StartWorkflow(acClassMethod, picking, appManager, workflow.ACClassWFID, string.IsNullOrEmpty(orderReservationInfo) ? processModule : null);
        }

        public void ClearBookingData()
        {
            if (ACFacilityManager == null)
                return;

            if (_BookParamRelocationClone == null)
                _BookParamRelocationClone = ACFacilityManager.ACUrlACTypeSignature("!" + gip.mes.datamodel.GlobalApp.FBT_Relocation_Facility_BulkMaterial, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            var clone = _BookParamRelocationClone.Clone() as ACMethodBooking;

            CurrentBookParamRelocation = clone;
        }

        protected ACRef<ACPickingManager> ACRefToPickingManager()
        {
            return ACPickingManager.ACRefToServiceInstance(this);
        }

        protected virtual bool PrepareStartWorkflow(ACMethodBooking forBooking, core.datamodel.ACClassMethod acClassMethod, out bool wfRunsBatches, out ACComponent appManager,
                                                    out Route validRoute, core.datamodel.ACClassWF workflow, bool sourceFacilityValidation = true)
        {
            string pwClassNameOfRoot = GetPWClassNameOfRoot(forBooking);
            wfRunsBatches = false;
            appManager = null;
            validRoute = null;

            Msg msg = null;

            if (   (sourceFacilityValidation && (forBooking.OutwardFacility == null || !forBooking.OutwardFacility.VBiFacilityACClassID.HasValue))
                || (!sourceFacilityValidation && forBooking.OutwardMaterial == null)
                || forBooking.InwardFacility == null 
                || !forBooking.InwardFacility.VBiFacilityACClassID.HasValue)
                return false;

            if (sourceFacilityValidation && forBooking.OutwardFacility != forBooking.InwardFacility)
            {
                msg = OnValidateRoutesForWF(forBooking, forBooking.OutwardFacility.FacilityACClass, forBooking.InwardFacility.FacilityACClass, out validRoute);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return false;
                }
            }

            if (workflow == null || workflow.ACClassMethod == null)
                return false;

            if (acClassMethod == null)
                return false;

            Type typePWWF = typeof(PWNodeProcessWorkflow);
            gip.core.datamodel.ACProject project = acClassMethod.ACClass.ACProject as gip.core.datamodel.ACProject;

            var AppManagersList = this.Root.FindChildComponents(project.RootClass, 1).Select(c => c as ACComponent).ToList();
            if (AppManagersList.Count > 1)
            {
                ShowDialog(this, "SelectAppManager"); //TODO
                if (appManager == null)
                    return false;
            }
            else
                appManager = AppManagersList.FirstOrDefault();

            ACComponent pAppManager = appManager as ACComponent;
            if (pAppManager == null)
                return false;
            if (pAppManager.IsProxy && pAppManager.ConnectionState == ACObjectConnectionState.DisConnected)
            {
                // 
                //Error50439: The connection to the server is unreachable, please try again when connection to server established.
                Messages.Error(this, "Error50439");
                return false;
            }
            return true;
        }

        public virtual string GetPWClassNameOfRoot(ACMethodBooking forBooking)
        {
            return nameof(PWMethodSingleDosing);
        }

        protected virtual bool StartWorkflow(gip.core.datamodel.ACClassMethod acClassMethod, Picking picking, ACComponent selectedAppManager, Guid allowedWFNode, ACComponent setReservationInfoOnModule = null)
        {
            using (Database db = new gip.core.datamodel.Database())
            {
                acClassMethod = acClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>(db);

                ACMethod acMethod = selectedAppManager.NewACMethod(acClassMethod.ACIdentifier);
                if (acMethod == null)
                    return false;
                string secondaryKey = Root.NoManager.GetNewNo(db, typeof(gip.core.datamodel.ACProgram), gip.core.datamodel.ACProgram.NoColumnName, gip.core.datamodel.ACProgram.FormatNewNo, this);
                gip.core.datamodel.ACProgram program = gip.core.datamodel.ACProgram.NewACObject(db, null, secondaryKey);
                program.ProgramACClassMethod = acClassMethod;
                program.WorkflowTypeACClass = acClassMethod.WorkflowTypeACClass;
                db.ACProgram.AddObject(program);
                if (db.ACSaveChanges() == null)
                {
                    if (setReservationInfoOnModule != null)
                        setReservationInfoOnModule.ACUrlCommand(nameof(PAProcessModule.ReservationInfo), secondaryKey);

                    ACValue paramProgram = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACProgram.ClassName);
                    if (paramProgram == null)
                        acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACProgram.ClassName, typeof(Guid), program.ACProgramID));
                    else
                        paramProgram.Value = program.ACProgramID;

                    ACValue acValue = acMethod.ParameterValueList.GetACValue(Picking.ClassName);
                    if (acValue == null)
                        acMethod.ParameterValueList.Add(new ACValue(Picking.ClassName, typeof(Guid), picking.PickingID));
                    else
                        acValue.Value = picking.PickingID;

                    ACValue paramACClassWF = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACClassWF.ClassName);
                    if (paramACClassWF == null)
                        acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACClassWF.ClassName, typeof(Guid), allowedWFNode));
                    else
                        paramACClassWF.Value = allowedWFNode;

                    selectedAppManager.ExecuteMethod(acClassMethod.ACIdentifier, acMethod);
                    return true;
                }
            }
            return false;
        }

        protected virtual Msg OnValidateRoutesForWF(ACMethodBooking forBooking, gip.core.datamodel.ACClass fromClass, gip.core.datamodel.ACClass toClass, out Route validRoute)
        {
            Msg msg = null;
            validRoute = null;
            string siloClass = this.ACFacilityManager.C_SiloClass;
            gip.core.datamodel.ACClass siloACClass = this.ACFacilityManager.GetACClassForIdentifier(siloClass, this.Database.ContextIPlus);
            if (siloACClass == null)
            {
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "OnValidateRoutesForWF(10)",
                    Message = String.Format("Type for {0} not found in Database or .NET-Type not loadable", siloClass)
                };
                return msg;
            }
            Type typeSilo = siloACClass.ObjectType;
            if (typeSilo == null)
            {
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "OnValidateRoutesForWF(20)",
                    Message = String.Format("Type for {0} not found in Database or .NET-Type not loadable", siloClass)
                };
                return msg;
            }

            ACRoutingParameters routingParameters = new ACRoutingParameters()
            {
                RoutingService = this.RoutingService,
                Database = this.Database.ContextIPlus,
                AttachRouteItemsToContext = false,
                Direction = RouteDirections.Forwards,
                SelectionRuleID = "",
                DBSelector = (c, p, r) => c.ACClassID == toClass.ACClassID,
                DBDeSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && (fromClass.ACClassID == c.ACClassID || typeSilo.IsAssignableFrom(c.ObjectType)),
                MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                IncludeReserved = true,
                IncludeAllocated = true,
                DBRecursionLimit = 10
            };

            RoutingResult result = ACRoutingService.SelectRoutes(fromClass, toClass, routingParameters);
            if (result.Routes == null || !result.Routes.Any())
            {
                //Error50122: No route found for this transport.
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "OnValidateRoutesForWF(30)",
                    Message = Root.Environment.TranslateMessage(this, "Error50122")
                };
                return msg;
            }

            Guid currentModule = CurrentProcessModule.ComponentClass.ACClassID;

            validRoute = result.Routes.FirstOrDefault(c => c.Items.Any(x => x.SourceGuid == currentModule || x.TargetGuid == currentModule));

            return msg;
        }

        protected virtual bool PreStartWorkflow(DatabaseApp dbApp, Route validRoute, gip.core.datamodel.ACClassWF rootWF, Picking picking)
        {
            List<Tuple<gip.core.datamodel.ACClassWF, string>> subWFs = new List<Tuple<gip.core.datamodel.ACClassWF, string>>();

            if (rootWF.PWACClass != null && rootWF.PWACClass.ACKindIndex == (short)Global.ACKinds.TPWNodeWorkflow)
            {
                Tuple<gip.core.datamodel.ACClassWF, string> subItem = new Tuple<gip.core.datamodel.ACClassWF, string>(rootWF, rootWF.ConfigACUrl);
                subWFs.Add(subItem);
            }

            GetSubWorkflows(new Tuple<gip.core.datamodel.ACClassWF, string>(rootWF, ""), subWFs, 0);

            List<SingleDosingConfigItem> configItems = new List<SingleDosingConfigItem>();

            foreach (var subWF in subWFs)
            {
                configItems.AddRange(subWF.Item1.RefPAACClassMethod.ACClassWF_ACClassMethod.Where(c => c.PWACClass != null && c.PWACClass.ACKindIndex == (short)Global.ACKinds.TPWGroup)
                                                                                     .Select(p => new SingleDosingConfigItem() { PreConfigACUrl = subWF.Item2, PWGroup = p }));
            }

            return OnPreStartWorkflow(dbApp, picking, configItems, validRoute, rootWF);

        }

        public virtual bool OnPreStartWorkflow(DatabaseApp dbApp, Picking picking, List<SingleDosingConfigItem> configItems, Route validRoute, gip.core.datamodel.ACClassWF rootWF)
        {
            return true;
        }

        private void GetSubWorkflows(Tuple<gip.core.datamodel.ACClassWF, string> acClassWF, List<Tuple<gip.core.datamodel.ACClassWF, string>> subworkflows, int depth, int maxDepth = 4)
        {
            string preConfigACUrl = "";
            var items = acClassWF.Item1.ACClassWF_ParentACClassWF.Where(c => c.PWACClass != null && c.PWACClass.ACKindIndex == (short)Global.ACKinds.TPWNodeWorkflow);
            if (items == null || !items.Any())
            {
                if (acClassWF.Item1.RefPAACClassMethod != null)
                {
                    preConfigACUrl = acClassWF.Item2 + "\\";
                    items = acClassWF.Item1.RefPAACClassMethod.ACClassWF_ACClassMethod.Where(c => c.PWACClass != null && c.PWACClass.ACKindIndex == (short)Global.ACKinds.TPWNodeWorkflow);
                }
            }

            if (items == null || !items.Any())
                return;

            if (depth >= maxDepth)
                return;
            depth++;

            var wfItems = items.Select(c => new Tuple<gip.core.datamodel.ACClassWF, string>(c, preConfigACUrl + c.ConfigACUrl));

            subworkflows.AddRange(wfItems);

            foreach (var subworkflow in wfItems)
            {
                GetSubWorkflows(subworkflow, subworkflows, depth, maxDepth);
            }
        }

        #endregion


        #endregion
    }
}
