using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;
using gip.mes.datamodel;
using gip.mes.facility;
using System.Diagnostics;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Relocation station'}de{'Umlagerstation'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMRelocationStation : PAMLoadingstation
    {
        #region c'tors
        static PAMRelocationStation()
        {
            RegisterExecuteHandler(typeof(PAMRelocationStation), HandleExecuteACMethod_PAMRelocationStation);
        }

        public PAMRelocationStation(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _DestFacilityNo = new ACPropertyConfigValue<string>(this, nameof(DestFacilityNo), "");
            _MethodNameWF = new ACPropertyConfigValue<string>(this, nameof(MethodNameWF), "");
            _ConfigACUrlOfNode = new ACPropertyConfigValue<string>(this, nameof(ConfigACUrlOfNode), "");
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _ = DestFacilityNo;
            _ = MethodNameWF;
            _ = ConfigACUrlOfNode;
            return true;
        }
        #endregion

        #region Properties
        private ACPropertyConfigValue<string> _DestFacilityNo;
        [ACPropertyConfig("en{'No of Destination'}de{'Lagerplatz-Nr Umagerungsziel'}")]
        public string DestFacilityNo
        {
            get
            {
                return _DestFacilityNo.ValueT;
            }
            set
            {
                _DestFacilityNo.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _MethodNameWF;
        [ACPropertyConfig("en{'Methodnamme of workflow'}de{'Methodenname des Workflows'}")]
        public string MethodNameWF
        {
            get
            {
                return _MethodNameWF.ValueT;
            }
            set
            {
                _MethodNameWF.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _ConfigACUrlOfNode;
        [ACPropertyConfig("en{'ACUrl of Planningnode'}de{'ACUrl des Planungsknoten'}")]
        public string ConfigACUrlOfNode
        {
            get
            {
                return _ConfigACUrlOfNode.ValueT;
            }
            set
            {
                _ConfigACUrlOfNode.ValueT = value;
            }
        }
        #endregion

        #region Methods
        [ACMethodInteractionClient("", "en{'Relocation'}de{'Umlagerung'}", 450, false)]
        public static void StartRelocation(IACComponent acComponent)
        {
            if (acComponent == null)
                return;
            using (Database db = new Database())
            using (DatabaseApp dbApp = new DatabaseApp(db))
            {
                FacilityManager facilityManager = FacilityManager.GetServiceInstance(acComponent.Root as ACComponent) as FacilityManager;
                if (facilityManager == null)
                {
                    acComponent.Messages.Error(acComponent, "FacilityManager not configured as local service object.", true);
                    return;
                }
                ACPickingManager pickingManager =  ACPickingManager.GetServiceInstance(acComponent.Root as ACComponent) as ACPickingManager;
                if (pickingManager == null)
                {
                    acComponent.Messages.Error(acComponent, "ACPickingManager not configured as local service object.", true);
                    return;
                }
                var methodNameWF = new ACPropertyConfigValue<string>(acComponent as ACComponent, nameof(MethodNameWF), "");
                if (String.IsNullOrEmpty(methodNameWF.ValueT))
                {
                    acComponent.Messages.Error(acComponent, "Methodname of workflow not configured on instance.", true);
                    return;
                }

                var configACUrlOfNode = new ACPropertyConfigValue<string>(acComponent as ACComponent, nameof(ConfigACUrlOfNode), "");
                if (String.IsNullOrEmpty(configACUrlOfNode.ValueT))
                {
                    acComponent.Messages.Error(acComponent, "ACUrl of Planningnode not configured on instance.", true);
                    return;
                }

                var destFacilityNo = new ACPropertyConfigValue<string>(acComponent as ACComponent, nameof(DestFacilityNo), "");
                if (String.IsNullOrEmpty(destFacilityNo.ValueT))
                {
                    acComponent.Messages.Error(acComponent, "No of Destination not configured on instance.", true);
                    return;
                }
                
                Facility inwardFacility = dbApp.Facility.FirstOrDefault(c => c.FacilityNo == destFacilityNo.ValueT);
                if (inwardFacility == null)
                {
                    acComponent.Messages.Error(acComponent, String.Format("Facility with No {0} doesn't exist.", destFacilityNo.ValueT), true);
                    return;
                }

                ACMethodBooking currentBookParamRelocation = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Relocation_Facility_BulkMaterial, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking;
                if (currentBookParamRelocation == null)
                    return;
                currentBookParamRelocation = currentBookParamRelocation.Clone() as ACMethodBooking;

                Material material = inwardFacility.Material;
                if (material == null)
                {
                    acComponent.Messages.Error(acComponent, String.Format("No material assigned to Facility {0}",configACUrlOfNode.ValueT), true);
                    return;
                }

                core.datamodel.ACClassWF workflowNode = dbApp.ContextIPlus.ACClassWF.Where(c => c.ACClassMethod != null && c.ACClassMethod.ACIdentifier == methodNameWF.ValueT && c.ACIdentifier == configACUrlOfNode.ValueT).FirstOrDefault();
                if (workflowNode == null)
                {
                    acComponent.Messages.Error(acComponent, String.Format("Workflow-Node {0} not found in workflow {1}.", configACUrlOfNode.ValueT, methodNameWF.ValueT), true);
                    return;
                }
                core.datamodel.ACClassMethod acClassMethod = workflowNode.ACClassMethod;


                currentBookParamRelocation.InwardFacility = inwardFacility;
                currentBookParamRelocation.InwardMaterial = material;
                currentBookParamRelocation.OutwardMaterial = material;
                currentBookParamRelocation.InwardQuantity = 10000000;
                currentBookParamRelocation.OutwardQuantity = 10000000;

                RunWorkflow(dbApp, workflowNode, acClassMethod, acComponent as ACComponent, currentBookParamRelocation, facilityManager, pickingManager);
            }
        }

        public static bool IsEnabledStartRelocation(IACComponent acComponent)
        {
            string orderInfo = acComponent.ACUrlCommand(nameof(PAProcessModule.OrderInfo)) as string;
            return string.IsNullOrEmpty(orderInfo);
        }

        protected static bool RunWorkflow(DatabaseApp dbApp, core.datamodel.ACClassWF workflow, core.datamodel.ACClassMethod acClassMethod, ACComponent processModule,
                        ACMethodBooking currentBookParamRelocation, FacilityManager facilityManager, ACPickingManager pickingManager,
                        bool sourceFacilityValidation = true, bool skipProcessModuleValidation = false, PARole.ValidationBehaviour validationBehaviour = PARole.ValidationBehaviour.Strict)
        {
            bool wfRunsBatches = false;
            ACComponent appManager = null;
            Route validRoute = null;

            if (processModule == null)
                return false;

            if (!skipProcessModuleValidation)
            {
                string orderInfo = processModule.ACUrlCommand(nameof(PAProcessModule.OrderInfo)) as string;

                if (!string.IsNullOrEmpty(orderInfo))
                {
                    //Question50075: The process module is occupied with order {0}. Are you sure that you want continue?
                    if (processModule.Messages.Question(processModule, "Question50075", Global.MsgResult.Yes, false, orderInfo) != Global.MsgResult.Yes)
                    {
                        return false;
                    }
                }

                string orderReservationInfo = processModule.ACUrlCommand(nameof(PAProcessModuleVB.OrderReservationInfo)) as string;
                if (!string.IsNullOrEmpty(orderReservationInfo))
                {
                    //Question50076: The process module is reserved for order {0}. Are you sure that you want continue?
                    if (processModule.Messages.Question(processModule, "Question50076",
                        Global.MsgResult.Yes, false, orderReservationInfo) != Global.MsgResult.Yes)
                    {
                        return false;
                    }
                }
            }

            if (!PrepareStartWorkflow(currentBookParamRelocation, acClassMethod, processModule, out wfRunsBatches, out appManager, out validRoute, workflow, sourceFacilityValidation))
            {
                return false;
            }

            Picking picking = null;
            MsgWithDetails msgDetails = pickingManager.CreateNewPicking(currentBookParamRelocation, acClassMethod, dbApp, dbApp.ContextIPlus, true, out picking);
            if (msgDetails != null && msgDetails.MsgDetailsCount > 0)
            {
                processModule.Messages.Msg(msgDetails);
                dbApp.ACUndoChanges();
                return false;
            }
            if (picking == null)
            {
                dbApp.ACUndoChanges();
                return false;
            }
            dbApp.ACSaveChanges();

            //bool result = PreStartWorkflow(dbApp, validRoute, workflow, picking);
            //if (!result)
            //{
            //    return false;
            //}

            msgDetails = pickingManager.ValidateStart(dbApp, dbApp.ContextIPlus, picking, null, validationBehaviour);
            if (msgDetails != null && msgDetails.MsgDetailsCount > 0)
            {
                processModule.Messages.Msg(msgDetails);
                return false;
            }

            processModule.ACUrlCommand(nameof(PAProcessModuleVB.OrderReservationInfo), picking.PickingNo);

            return StartWorkflow(dbApp, acClassMethod, picking, appManager, workflow.ACClassWFID, processModule);
        }

        protected static bool PrepareStartWorkflow(ACMethodBooking forBooking, gip.core.datamodel.ACClassMethod acClassMethod, ACComponent processModule, out bool wfRunsBatches,
                                                     out ACComponent appManager, out Route validRoute, gip.core.datamodel.ACClassWF workflow, bool sourceFacilityValidation = true)
        {
            wfRunsBatches = false;
            appManager = null;
            validRoute = null;

            if (forBooking.OutwardMaterial == null
                || forBooking.InwardFacility == null || !forBooking.InwardFacility.VBiFacilityACClassID.HasValue)
                return false;

            if (workflow == null || workflow.ACClassMethod == null)
                return false;

            if (acClassMethod == null)
                return false;

            gip.core.datamodel.ACProject project = acClassMethod.ACClass.ACProject as gip.core.datamodel.ACProject;
            var AppManagersList = processModule.Root.FindChildComponents(project.RootClass, 1).Select(c => c as ACComponent).ToList();
            if (AppManagersList.Count > 1)
            {
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
                processModule.Messages.Error(processModule, "Die Verbindung zum Server ist unerreichbar, bitte versuchen Sie es erneut, wenn die Verbindung zum Server hergestellt ist.", true);
                return false;
            }
            return true;
        }

        private class SingleDosingConfigItem
        {
            public string PreConfigACUrl
            {
                get;
                set;
            }

            public gip.core.datamodel.ACClassWF PWGroup
            {
                get;
                set;
            }

            public IEnumerable<gip.core.datamodel.ACClass> PossibleMachines
            {
                get => PWGroup.RefPAACClass.DerivedClassesInProjects;
            }
        }

        //protected static bool PreStartWorkflow(DatabaseApp dbApp, Route validRoute, gip.core.datamodel.ACClassWF rootWF, Picking picking)
        //{
        //    List<Tuple<gip.core.datamodel.ACClassWF, string>> subWFs = new List<Tuple<gip.core.datamodel.ACClassWF, string>>();

        //    if (rootWF.PWACClass != null && rootWF.PWACClass.ACKindIndex == (short)Global.ACKinds.TPWNodeWorkflow)
        //    {
        //        Tuple<gip.core.datamodel.ACClassWF, string> subItem = new Tuple<gip.core.datamodel.ACClassWF, string>(rootWF, rootWF.ConfigACUrl);
        //        subWFs.Add(subItem);
        //    }

        //    GetSubWorkflows(new Tuple<gip.core.datamodel.ACClassWF, string>(rootWF, ""), subWFs, 0);

        //    List<SingleDosingConfigItem> configItems = new List<SingleDosingConfigItem>();

        //    foreach (var subWF in subWFs)
        //    {
        //        configItems.AddRange(subWF.Item1.RefPAACClassMethod.ACClassWF_ACClassMethod.Where(c => c.PWACClass != null && c.PWACClass.ACKindIndex == (short)Global.ACKinds.TPWGroup)
        //                                                                             .Select(p => new SingleDosingConfigItem() { PreConfigACUrl = subWF.Item2, PWGroup = p }));
        //    }

        //    return true;
        //}

        //protected static void GetSubWorkflows(Tuple<gip.core.datamodel.ACClassWF, string> acClassWF, List<Tuple<gip.core.datamodel.ACClassWF, string>> subworkflows, int depth, int maxDepth = 4)
        //{
        //    string preConfigACUrl = "";
        //    var items = acClassWF.Item1.ACClassWF_ParentACClassWF.Where(c => c.PWACClass != null && c.PWACClass.ACKindIndex == (short)Global.ACKinds.TPWNodeWorkflow);
        //    if (items == null || !items.Any())
        //    {
        //        if (acClassWF.Item1.RefPAACClassMethod != null)
        //        {
        //            preConfigACUrl = acClassWF.Item2 + "\\";
        //            items = acClassWF.Item1.RefPAACClassMethod.ACClassWF_ACClassMethod.Where(c => c.PWACClass != null && c.PWACClass.ACKindIndex == (short)Global.ACKinds.TPWNodeWorkflow);
        //        }
        //    }

        //    if (items == null || !items.Any())
        //        return;

        //    if (depth >= maxDepth)
        //        return;
        //    depth++;

        //    var wfItems = items.Select(c => new Tuple<gip.core.datamodel.ACClassWF, string>(c, preConfigACUrl + c.ConfigACUrl));

        //    subworkflows.AddRange(wfItems);

        //    foreach (var subworkflow in wfItems)
        //    {
        //        GetSubWorkflows(subworkflow, subworkflows, depth, maxDepth);
        //    }
        //}


        protected static bool StartWorkflow(DatabaseApp dbApp, gip.core.datamodel.ACClassMethod acClassMethod, Picking picking, ACComponent selectedAppManager, Guid allowedWFNode, ACComponent processModule)
        {
            Database db = dbApp.ContextIPlus as Database;
            //using (Database db = new gip.core.datamodel.Database())
            {
                acClassMethod = acClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>(db);

                ACMethod acMethod = selectedAppManager.NewACMethod(acClassMethod.ACIdentifier);
                if (acMethod == null)
                    return false;
                string secondaryKey = processModule.Root.NoManager.GetNewNo(db, typeof(gip.core.datamodel.ACProgram), gip.core.datamodel.ACProgram.NoColumnName, gip.core.datamodel.ACProgram.FormatNewNo, processModule);
                gip.core.datamodel.ACProgram program = gip.core.datamodel.ACProgram.NewACObject(db, null, secondaryKey);
                program.ProgramACClassMethod = acClassMethod;
                program.WorkflowTypeACClass = acClassMethod.WorkflowTypeACClass;
                db.ACProgram.AddObject(program);
                if (db.ACSaveChanges() == null)
                {
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

        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMRelocationStation(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(StartRelocation):
                    StartRelocation(acComponent);
                    return true;
                case nameof(IsEnabledStartRelocation):
                    result = IsEnabledStartRelocation(acComponent);
                    return true;
            }
            return HandleExecuteACMethod_PAMLoadingstation(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

    }
}
