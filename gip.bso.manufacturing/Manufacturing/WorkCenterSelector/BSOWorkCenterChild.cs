// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
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
    /// <summary>
    /// Abstract base class for work center child components in manufacturing environments.
    /// This class provides core functionality for managing workflow operations, picking processes, 
    /// and facility management within work center contexts.
    /// 
    /// Key responsibilities include:
    /// - Managing process module activation and deactivation
    /// - Executing manufacturing workflows with validation and routing
    /// - Handling facility booking operations and relocations
    /// - Managing picking operations through ACPickingManager integration
    /// - Validating routes between facilities for material transport
    /// - Coordinating with facility managers, routing services, and picking managers
    /// 
    /// The class maintains references to essential manufacturing services:
    /// - ACFacilityManager for facility operations
    /// - ACPickingManager for picking and material handling
    /// - RoutingService for path validation and route calculation
    /// 
    /// Workflow execution includes comprehensive validation of:
    /// - Process module availability and reservations
    /// - Source and destination facility validation
    /// - Route feasibility between facilities
    /// - Configuration stores and validation behaviors
    /// 
    /// Derived classes must implement specific work center functionality while leveraging
    /// the common workflow and facility management infrastructure provided by this base class.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Work center child'}de{'Work center child'}", Global.ACKinds.TACAbstractClass, Global.ACStorableTypes.NotStorable, true, true)]
    public abstract class BSOWorkCenterChild : ACBSOvb
    {
        #region c'tors

        /// <summary>
        /// Initializes a new instance of the BSOWorkCenterChild class.
        /// This constructor sets up the base infrastructure for work center child components,
        /// including initialization of core manufacturing services references and workflow management capabilities.
        /// The constructor calls the base ACBSOvb constructor to establish the component hierarchy and
        /// prepare the component for managing manufacturing operations, facility bookings, and picking processes.
        /// </summary>
        /// <param name="acType">The iPlus-Type information (ACClass) used for constructing this component instance, containing metadata about the class structure and capabilities.</param>
        /// <param name="content">The content object associated with this instance. For persistent instances in application trees, this is typically an ACClassTask object that ensures state persistence across service restarts. For dynamic instances, this is usually null.</param>
        /// <param name="parentACObject">The parent ACComponent under which this instance is created as a child object, establishing the component hierarchy within the work center structure.</param>
        /// <param name="parameter">Construction parameters passed via ACValueList containing ACValue entries with parameter names, values, and data types. Use ACClass.TypeACSignature() to get the correct parameter structure for the component type.</param>
        /// <param name="acIdentifier">Unique identifier for this instance within the parent component's child collection. If empty, the runtime assigns an ID automatically using the ACType identifier.</param>
        public BSOWorkCenterChild(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        /// <summary>
        /// Deinitializes the BSOWorkCenterChild instance, releasing booking parameter resources and detaching references.
        /// Calls the base ACDeInit method to complete deinitialization.
        /// </summary>
        /// <param name="deleteACClassTask">Indicates whether to delete the associated ACClassTask.</param>
        /// <returns>True if deinitialization succeeds; otherwise, false.</returns>
        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            _BookParamRelocation = null;
            _BookParamRelocationClone = null;

            return await base.ACDeInit(deleteACClassTask);
        }

        public const string Const_ACClassWFID = "PickingACClassWFID";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the function associated with the work center item.
        /// This property represents the operational function (e.g., weighing, sampling) installed under a process module
        /// and is used to manage the state and related business service objects (BSOs) for the work center child.
        /// </summary>
        public WorkCenterItemFunction ItemFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the parent work center selector (BSOWorkCenterSelector) for this work center child.
        /// This property provides access to the parent selector component, which manages work center selection
        /// and related operations in the manufacturing environment.
        /// </summary>
        [ACPropertyInfo(510,
                        Description = @"Gets the parent work center selector (BSOWorkCenterSelector) for this work center child.
                                        This property provides access to the parent selector component, which manages work center selection
                                        and related operations in the manufacturing environment.")]
        public BSOWorkCenterSelector ParentBSOWCS
        {
            get
            {
                return ParentACComponent as BSOWorkCenterSelector;
            }
        }

        /// <summary>
        /// Gets or sets the current process module associated with this work center child.
        /// Represents the active ACComponent process module for workflow operations and facility management.
        /// </summary>
        public virtual ACComponent CurrentProcessModule
        {
            get;
            protected set;
        }

        #region Properties => Start workflow picking

        protected ACRef<ACComponent> _ACFacilityManager = null;
        /// <summary>
        /// Gets the FacilityManager instance associated with this work center child.
        /// Returns the FacilityManager if the internal reference is set; otherwise, returns null.
        /// Used for facility operations such as booking, relocation, and management within the work center context.
        /// </summary>
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
        /// <summary>
        /// Gets the ACPickingManager instance associated with this work center child.
        /// Returns the ACPickingManager if the internal reference is set; otherwise, returns null.
        /// Used for managing picking and material handling operations within the work center context.
        /// </summary>
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
        /// <summary>
        /// Gets the routing service component associated with this work center child.
        /// Returns the ACComponent instance referenced by the internal _RoutingService property,
        /// or null if no routing service is available.
        /// Used for validating and calculating routes between facilities in manufacturing workflows.
        /// </summary>
        public ACComponent RoutingService
        {
            get
            {
                if (_RoutingService == null)
                    return null;
                return _RoutingService.ValueT;
            }
        }

        /// <summary>
        /// Indicates whether the routing service is available and connected.
        /// Returns true if the RoutingService property is not null and its connection state is not disconnected.
        /// </summary>
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
        /// Gets or sets the current booking parameter for relocation operations.
        /// This property holds the ACMethodBooking instance used for facility relocation bookings
        /// within the work center child. It is updated when booking data is cleared or set,
        /// and notifies property changes to support UI binding and workflow logic.
        /// </summary>
        [ACPropertyCurrent(704, "BookParamRelocation",
                           Description = @"Gets or sets the current booking parameter for relocation operations.
                                           This property holds the ACMethodBooking instance used for facility relocation bookings
                                           within the work center child. It is updated when booking data is cleared or set,
                                           and notifies property changes to support UI binding and workflow logic.")]
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

        /// <summary>
        /// Activates the specified process module for this work center child.
        /// Sets the CurrentProcessModule property to the provided ACComponent instance,
        /// enabling workflow and facility operations for the selected process module.
        /// Derived classes may override this method to implement additional activation logic.
        /// </summary>
        /// <param name="selectedProcessModule">The process module (ACComponent) to activate for this work center child.</param>
        public virtual void Activate(ACComponent selectedProcessModule)
        {
            CurrentProcessModule = selectedProcessModule;
        }

        /// <summary>
        /// Deactivates the work center child, clearing booking data and detaching references to routing service, facility manager, and picking manager.
        /// Resets the current process module to null.
        /// </summary>
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

        /// <summary>
        /// Executes a manufacturing workflow for the specified process module with comprehensive validation and picking management.
        /// This method orchestrates the complete workflow execution process including:
        /// - Process module validation and reservation checking with user confirmation dialogs
        /// - Workflow preparation with route validation between source and destination facilities
        /// - Picking order creation and validation through ACPickingManager integration
        /// - Configuration management and validation behavior enforcement
        /// - Optional picking preparation mode or full workflow execution
        /// The method performs extensive validation including:
        /// - Process module availability and existing order/reservation conflicts
        /// - Source and destination facility validation with route feasibility checking
        /// - Workflow configuration validation using ConfigManagerIPlus services
        /// - Picking parameter validation according to specified validation behavior
        /// Error handling includes automatic cleanup of booking data and database rollback on failures.
        /// The method supports both preparation-only mode (for configuration setup) and full execution mode.
        /// </summary>
        /// <param name="dbApp">The database application context for data operations and transaction management.</param>
        /// <param name="workflow">The ACClassWF workflow definition containing the workflow structure and configuration.</param>
        /// <param name="acClassMethod">The ACClassMethod defining the workflow method to be executed.</param>
        /// <param name="processModule">The target process module (ACComponent) where the workflow will be executed.</param>
        /// <param name="sourceFacilityValidation">If true, validates the source facility and routes; if false, skips source facility validation for material-only workflows.</param>
        /// <param name="skipProcessModuleValidation">If true, bypasses process module occupation and reservation checks (use with caution).</param>
        /// <param name="validationBehaviour">Defines the validation strictness level (Strict, Lax, etc.) for picking and workflow validation.</param>
        /// <param name="onlyPreparePicking">If true, only creates and configures the picking order without starting the actual workflow execution.</param>
        /// <returns>True if the workflow execution or picking preparation completed successfully; false if validation failed or errors occurred.</returns>
        public async Task<bool> RunWorkflow(DatabaseApp dbApp, core.datamodel.ACClassWF workflow, core.datamodel.ACClassMethod acClassMethod, ACComponent processModule, bool sourceFacilityValidation = true,
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
                    if (await Messages.QuestionAsync(this, "Question50075", Global.MsgResult.Yes, false, orderInfo) != Global.MsgResult.Yes)
                    {
                        return false;
                    }
                }

                orderReservationInfo = processModule.ACUrlCommand(nameof(PAProcessModule.ReservationInfo)) as string;
                if (!string.IsNullOrEmpty(orderReservationInfo))
                {
                    //Question50076: The process module is reserved for order {0}. Are you sure that you want continue?
                    if (await Messages.QuestionAsync(this, "Question50076",
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
                Messages.MsgAsync(msgDetails);
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
                Messages.MsgAsync(msgDetails);
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

        /// <summary>
        /// Clears and resets the booking data for relocation operations.
        /// If the FacilityManager is available, clones the relocation booking parameter template
        /// to ensure a fresh booking state and assigns it to CurrentBookParamRelocation.
        /// This prevents deadlocks by always using the global database context for cloning.
        /// </summary>
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
                    Messages.MsgAsync(msg);
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
                Messages.ErrorAsync(this, "Error50439");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns the class name of the root process workflow method for the given booking.
        /// Derived classes can override this to provide specific workflow method class names based on booking context.
        /// </summary>
        /// <param name="forBooking">The booking parameter used to determine the workflow method class name.</param>
        /// <returns>The class name of the root process workflow method.</returns>
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
                db.ACProgram.Add(program);
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

        /// <summary>
        /// Hook for pre-start workflow logic.
        /// Called before starting a workflow to allow custom configuration, validation, or setup.
        /// Override in derived classes to implement specific pre-start actions such as configuration of dosing, route checks, or picking preparation.
        /// By default, returns true to indicate success.
        /// </summary>
        /// <param name="dbApp">Database application context.</param>
        /// <param name="picking">Current picking order.</param>
        /// <param name="configItems">List of configuration items for single dosing.</param>
        /// <param name="validRoute">Validated route for material transport.</param>
        /// <param name="rootWF">Root workflow node.</param>
        /// <returns>True if pre-start workflow actions succeed; otherwise, false.</returns>
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
