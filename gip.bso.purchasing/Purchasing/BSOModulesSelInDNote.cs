using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace gip.bso.purchasing
{
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Machine- and containerselection'}de{'Maschinen- und Siloauswahl'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, false, true)]
    public class BSOModulesSelInDNote : VBBSOModulesSelector
    {
        #region c´tors

        public BSOModulesSelInDNote(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (ParentACComponent is BSOInDeliveryNote)
            {
                BSOInDeliveryNote bso = ParentACComponent as BSOInDeliveryNote;
                CurrentDeliveryNotePos = bso.SelectedDeliveryNotePos;
            }
            if (!base.ACInit(startChildMode))
                return false;


            _InDeliveryNoteManager = ACInDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_InDeliveryNoteManager == null)
                throw new Exception("InDeliveryNoteManager not configured");

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._CurrentDeliveryNotePos = null;

            ACInDeliveryNoteManager.DetachACRefFromServiceInstance(this, _InDeliveryNoteManager);
            _InDeliveryNoteManager = null;

            return base.ACDeInit(deleteACClassTask);
        }

        protected override void ParentACComponent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ParentACComponent is BSOInDeliveryNote && e.PropertyName == "SelectedDeliveryNotePos")
            {
                BSOInDeliveryNote bso = ParentACComponent as BSOInDeliveryNote;
                CurrentDeliveryNotePos = bso.SelectedDeliveryNotePos;
            }
            if (ParentACComponent is BSOInDeliveryNote && e.PropertyName == "SetupCurrentDeliveryNotePos")
            {
                BSOInDeliveryNote bso = ParentACComponent as BSOInDeliveryNote;
                CurrentDeliveryNotePos = bso.SelectedDeliveryNotePos;
                RefreshTargets();
            }
        }

        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            //if (vbControl == null)
            //    return base.OnGetControlModes(vbControl);
            //switch (vbControl.VBContent)
            //{
            //    case "CurrentACMethodValue\\Value":
            //    case "CurrentACMethodValue\\Expression":
            //    case "CurrentACMethodValue\\Comment":

            //        if (CurrentACMethodValue == null || CurrentACMethodValue.Source != ProcessEntity.ToString())
            //            return Global.ControlModes.Disabled;
            //        break;
            //    case "CurrentPAACMethodValue\\Value":
            //    case "CurrentPAACMethodValue\\Expression":
            //    case "CurrentPAACMethodValue\\Comment":
            //        if (CurrentPAACMethodValue == null || CurrentPAACMethodValue.Source != ProcessEntity.ToString())
            //            return Global.ControlModes.Disabled;
            //        break;
            //}
            return base.OnGetControlModes(vbControl);
        }

        #endregion

        #region Manager

        protected ACRef<ACInDeliveryNoteManager> _InDeliveryNoteManager = null;
        public ACInDeliveryNoteManager InDeliveryNoteManager
        {
            get
            {
                if (_InDeliveryNoteManager == null)
                    return null;
                return _InDeliveryNoteManager.ValueT;
            }
        }

        #endregion

        #region BSO->ACProperty
        public override bool IsModuleSelDisabled
        {
            get
            {
                return false;
            }
        }

        protected DeliveryNotePos _CurrentDeliveryNotePos;
        [ACPropertyInfo(600)]
        public DeliveryNotePos CurrentDeliveryNotePos
        {
            get
            {
                return _CurrentDeliveryNotePos;
            }
            set
            {
                if (_CurrentDeliveryNotePos != value)
                {
                    SetupCurrentDeliveryNotePos(value);
                }
            }
        }

        public void SetupCurrentDeliveryNotePos(DeliveryNotePos val)
        {
            _CurrentDeliveryNotePos = val;
            this._ShowSelectedCells = false;
            OnPropertyChanged("ShowSelectedCells");
            RefreshModules();
            OnPropertyChanged("CurrentDeliveryNotePos");
        }

        protected override void _Modules_ListChanged(object sender, ListChangedEventArgs e)
        {
            base._Modules_ListChanged(sender, e);
        }
        protected override void _Targets_ListChanged(object sender, ListChangedEventArgs e)
        {
            base._Targets_ListChanged(sender, e);
        }


        private List<core.datamodel.ACClassMethod> _WorkflowList;
        [ACPropertyList(601, "Workflow")]
        public IEnumerable<core.datamodel.ACClassMethod> WorkflowList
        {
            get
            {
                return _WorkflowList;
            }
        }

        core.datamodel.ACClassMethod _SelectedWorkflow;
        [ACPropertySelected(602, "Workflow")]
        public core.datamodel.ACClassMethod SelectedWorkflow
        {
            get
            {
                return _SelectedWorkflow;
            }
            set
            {
                if (_SelectedWorkflow != value)
                {
                    _SelectedWorkflow = value;
                    OnPropertyChanged("SelectedWorkflow");
                }
            }
        }

        #endregion

        #region BSO->ACMethod

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "StartIntake":
                    StartIntake();
                    return true;
                case "IsEnabledStartIntake":
                    result = IsEnabledStartIntake();
                    return true;
                case "DialogWorkflowOK":
                    DialogWorkflowOK();
                    return true;
                case "DialogWorkflowCancel":
                    DialogWorkflowCancel();
                    return true;
                case "DialogOK":
                    DialogOK();
                    return true;
                case "DialogCancel":
                    DialogCancel();
                    return true;
                case "OpenRoute":
                    OpenRoute();
                    return true;
                case Const.IsEnabledPrefix + "OpenRoute":
                    result = IsEnabledOpenRoute();
                    return true;
                case "SetRoute":
                    SetRoute();
                    return true;
                case Const.IsEnabledPrefix + "SetRoute":
                    result = IsEnabledSetRoute();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        protected override void RefreshModules()
        {
            if (_CurrentDeliveryNotePos != null && _CurrentDeliveryNotePos.InOrderPos != null)
            {
                if (    _CurrentDeliveryNotePos.InOrderPos.EntityState == EntityState.Modified 
                     || _CurrentDeliveryNotePos.InOrderPos.EntityState == EntityState.Unchanged)
                    _CurrentDeliveryNotePos.InOrderPos.FacilityReservation_InOrderPos.AutoLoad(_CurrentDeliveryNotePos.InOrderPos.FacilityReservation_InOrderPosReference, _CurrentDeliveryNotePos);
                var selectedModules = _CurrentDeliveryNotePos.InOrderPos.FacilityReservation_InOrderPos.ToArray();
                var reservationCollection = new BindingList<POPartslistPosReservation>();
                var availableModules = GetAvailableModulesAsACClass(_CurrentDeliveryNotePos, this.Database.ContextIPlus);
                if (availableModules != null)
                {
                    foreach (var acClass in availableModules)
                    {
                        FacilityReservation selectedModule = selectedModules.Where(c => c.VBiACClassID == acClass.ACClassID).FirstOrDefault();
                        reservationCollection.Add(new POPartslistPosReservation(acClass, _CurrentDeliveryNotePos, null, selectedModule, null));
                    }
                }
                ModulesList = reservationCollection;
                SelectedModule = ModulesList.FirstOrDefault();
            }
            else
            {
                ModulesList = new BindingList<POPartslistPosReservation>();
                SelectedModule = null;
            }
        }

        protected override void RefreshTargets()
        {
            if (_CurrentDeliveryNotePos != null && SelectedModule != null && SelectedModule.SelectedReservation != null)
            {
                DatabaseApp.FacilityReservation.AutoMergeOption();
                var selectedModules = DatabaseApp.FacilityReservation
                     .Include(c => c.Facility)
                     .Include(c => c.Facility.Material)
                     .Where(c => c.ParentFacilityReservationID == SelectedModule.SelectedReservation.FacilityReservationID).AutoMergeOption();
                //SelectedModule.SelectedReservation.FacilityReservation_ParentFacilityReservation.AutoLoad();
                //var selectedModules = SelectedModule.SelectedReservation.FacilityReservation_ParentFacilityReservation.ToArray();
                var reservationCollection = new BindingList<POPartslistPosReservation>();
                var availableModules = GetAvailableTargetsAsACClass(_CurrentDeliveryNotePos, SelectedModule.Module, this.Database.ContextIPlus);
                if (availableModules != null)
                {
                    List<Guid> notSelected = new List<Guid>();
                    foreach (var acClass in availableModules)
                    {
                        //FacilityReservation selectedModule = selectedModules.Where(c => c.VBiACClassID == acClass.ACClassID).FirstOrDefault();
                        //if (selectedModule == null)
                        notSelected.Add(acClass.ACClassID);
                    }
                    DatabaseApp.Facility.AutoMergeOption();
                    var queryUnselFacilities = this.DatabaseApp.Facility
                        .Include(c => c.Material)
                        .Where(DynamicQueryable.BuildOrExpression<Facility, Guid>(c => c.VBiFacilityACClassID.Value, notSelected))
                        .ToList();
                    foreach (var facility in queryUnselFacilities)
                        facility.FacilityStock_Facility.AutoLoad(facility.FacilityStock_FacilityReference, facility);
                    foreach (var acClass in availableModules)
                    {
                        //if (ShowWCells && acClass.ACCaption.IndexOf("(W)") < 0)
                        //    continue;
                        Facility unselFacility = null;
                        FacilityReservation selectedModule = selectedModules.Where(c => c.VBiACClassID == acClass.ACClassID).FirstOrDefault();
                        if (selectedModule == null)
                        {
                            if (ShowSelectedCells)
                                continue;
                            unselFacility = queryUnselFacilities.Where(c => c.VBiFacilityACClassID == acClass.ACClassID).FirstOrDefault();
                            if (ShowEnabledCells && unselFacility != null && !unselFacility.InwardEnabled)
                                continue;
                            if (ShowSameMaterialCells && unselFacility != null && _CurrentDeliveryNotePos.Material != null && unselFacility.Material != null && unselFacility.MaterialID != _CurrentDeliveryNotePos.Material.MaterialID)
                                continue;
                        }
                        reservationCollection.Add(new POPartslistPosReservation(acClass, _CurrentDeliveryNotePos, SelectedModule.SelectedReservation, selectedModule, unselFacility));
                    }
                }
                TargetsList = reservationCollection;
                SelectedTarget = TargetsList.FirstOrDefault();
            }
            else
            {
                TargetsList = new BindingList<POPartslistPosReservation>();
                SelectedTarget = null;
            }
        }

        // Static, if more instances active
        //private static bool _IsStarting = false;
        [ACMethodCommand("", "en{'Start intake'}de{'Start Annahme'}", (short)MISort.Start)]
        public virtual void StartIntake()
        {
            //_IsStarting = true;
            try
            {
                ACSaveChanges();
                if (!IsEnabledStartIntake())
                    return;

                //if (!StartBatchValidation())
                //    return;

                core.datamodel.ACClass intakeDefClass = SelectedModule.Module.ACClass1_BasedOnACClass;
                while (intakeDefClass != null)
                {
                    if (intakeDefClass.ACProject == null)
                    {
                        intakeDefClass = null;
                        break;
                    }
                    if (intakeDefClass.ACProject.ACProjectType == Global.ACProjectTypes.AppDefinition)
                        break;
                    intakeDefClass = intakeDefClass.ACClass1_BasedOnACClass;
                }

                if (intakeDefClass == null)
                    return;

                if (!intakeDefClass.ACClassWF_RefPAACClass.Any())
                {
                    // TODO: Errormessage
                    return;
                }

                var queryIntakeRootWFs = intakeDefClass.ACClassWF_RefPAACClass.Where(c => c.ParentACClassWFID.HasValue
                                                                && !c.ACClassWF1_ParentACClassWF.ParentACClassWFID.HasValue
                                                                && c.ACClassWF1_ParentACClassWF.PWACClass.ACIdentifier == "PWMethodIntake")
                                                     .Select(c => c.ACClassWF1_ParentACClassWF);
                core.datamodel.ACClassWF wfNode = null;
                gip.core.datamodel.ACClassMethod acClassMethod = null;
                if (!queryIntakeRootWFs.Any())
                {
                    // TODO: Errormessage
                }
                // TODO: User must select Workflow for Intaking:
                else if (queryIntakeRootWFs.Count() > 1)
                {
                    SelectedWorkflow = null;
                    _WorkflowList = queryIntakeRootWFs.Select(c => c.ACClassMethod).ToList();
                    OnPropertyChanged("WorkflowList");
                    ShowDialog(this, "SelectProcessWorkflow");
                    //wfNode = queryIntakeRootWFs.FirstOrDefault();
                    if (SelectedWorkflow == null)
                        return;
                    acClassMethod = SelectedWorkflow;
                }
                else
                {
                    wfNode = queryIntakeRootWFs.FirstOrDefault();
                    if (wfNode == null || wfNode.ACClassMethod == null)
                        return;
                    acClassMethod = wfNode.ACClassMethod;
                }

                if (SelectedWorkflow == null && (wfNode == null || wfNode.ACClassMethod == null))
                    return;

                if (acClassMethod == null)
                    return;
                gip.core.datamodel.ACProject project = acClassMethod.ACClass.ACProject as gip.core.datamodel.ACProject;

                AppManagersList = this.Root.FindChildComponents(project.RootClass, 1);
                if (AppManagersList.Count > 1)
                {
                    DialogResult = null;
                    ShowDialog(this, "SelectAppManager");
                    if (DialogResult == null || DialogResult.SelectedCommand != eMsgButton.OK)
                        return;
                }
                else
                    SelectedAppManager = AppManagersList.FirstOrDefault();

                ACComponent pAppManager = SelectedAppManager as ACComponent;
                if (pAppManager == null)
                    return;
                if (pAppManager.IsProxy && pAppManager.ConnectionState == ACObjectConnectionState.DisConnected)
                {
                    // TODO: Message
                    return;
                }


                if (CurrentDeliveryNotePos.InOrderPos.MDDelivPosLoadState == null || CurrentDeliveryNotePos.InOrderPos.MDDelivPosLoadState.DelivPosLoadState != MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)
                    CurrentDeliveryNotePos.InOrderPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(this.DatabaseApp, MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad).FirstOrDefault();
                BSOInDeliveryNote bsoDN = ParentACComponent as BSOInDeliveryNote;
                if (bsoDN != null)
                {
                    bsoDN.OnPropertyChanged("CurrentDeliveryNote");
                    bsoDN.OnPropertyChanged("CurrentDeliveryNotePos");
                }

                bool succ = ACSaveChanges();
                if (!succ)
                    return;

                var acProgramIDs = this.DatabaseApp.OrderLog.Where(c => c.DeliveryNotePosID.HasValue
                                                    && c.DeliveryNotePos.DeliveryNoteID == CurrentDeliveryNotePos.DeliveryNoteID)
                                         .Select(c => c.VBiACProgramLog.ACProgramID)
                                         .Distinct()
                                         .ToArray();

                if (acProgramIDs != null && acProgramIDs.Any())
                {
                    ChildInstanceInfoSearchParam searchParam = new ChildInstanceInfoSearchParam() { OnlyWorkflows = true, ACProgramIDs = acProgramIDs };
                    var childInstanceInfos = pAppManager.GetChildInstanceInfo(1, searchParam);
                    if (childInstanceInfos != null && childInstanceInfos.Any())
                    {
                        var childInstanceInfo = childInstanceInfos.FirstOrDefault();
                        //string acUrlComand = String.Format("{0}\\{1}!{2}", childInstanceInfo.ACUrlParent, childInstanceInfo.ACIdentifier, PWMethodIntake.ReloadBPAndResumeACIdentifier);
                        //pAppManager.ACUrlCommand(acUrlComand);
                        return;
                    }
                }

                ACMethod acMethod = pAppManager.NewACMethod(acClassMethod.ACIdentifier);
                if (acMethod == null)
                    return;

                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(gip.core.datamodel.ACProgram), gip.core.datamodel.ACProgram.NoColumnName, gip.core.datamodel.ACProgram.FormatNewNo, this);
                gip.core.datamodel.ACProgram program = gip.core.datamodel.ACProgram.NewACObject(this.Database.ContextIPlus, null, secondaryKey);
                program.ProgramACClassMethod = acClassMethod;
                program.WorkflowTypeACClass = acClassMethod.WorkflowTypeACClass;
                this.Database.ContextIPlus.ACProgram.Add(program);
                //CurrentProdOrderPartslist.VBiACProgramID = program.ACProgramID;
                if (ACSaveChanges())
                {
                    ACValue paramProgram = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACProgram.ClassName);
                    if (paramProgram == null)
                        acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACProgram.ClassName, typeof(Guid), program.ACProgramID));
                    else
                        paramProgram.Value = program.ACProgramID;

                    if (CurrentDeliveryNotePos != null)
                    {
                        ACValue acValuePPos = acMethod.ParameterValueList.GetACValue(DeliveryNotePos.ClassName);
                        if (acValuePPos == null)
                            acMethod.ParameterValueList.Add(new ACValue(DeliveryNotePos.ClassName, typeof(Guid), CurrentDeliveryNotePos.DeliveryNotePosID));
                        else
                            acValuePPos.Value = CurrentDeliveryNotePos.DeliveryNotePosID;
                    }

                    pAppManager.ExecuteMethod(acClassMethod.ACIdentifier, acMethod);

                    //IACPointAsyncRMI rmiInvocationPoint = pAppManager.GetPoint(Const.TaskInvocationPoint) as IACPointAsyncRMI;
                    //if (rmiInvocationPoint != null)
                    //rmiInvocationPoint.AddTask(acMethod, this);

                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOModulesSelInDNote", "StartIntake", msg);
            }
            finally
            {
                //_IsStarting = false;
            }
        }

        public bool IsEnabledStartIntake()
        {
            if (SelectedModule == null || SelectedModule.Module == null || CurrentDeliveryNotePos == null || CurrentDeliveryNotePos.InOrderPos == null)
                return false;
            if (CurrentDeliveryNotePos.DeliveryNote.MDDelivNoteState.DelivNoteState >= MDDelivNoteState.DelivNoteStates.Completed
                || (CurrentDeliveryNotePos.InOrderPos.MDDelivPosLoadState != null && CurrentDeliveryNotePos.InOrderPos.MDDelivPosLoadState.DelivPosLoadState >= MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad))
                return false;
            return true;
        }

        #region Dialog select App-Manager
        public VBDialogResult DialogResult { get; set; }

        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay)]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            var selected = SelectedAppManager;
            CloseTopDialog();
            SelectedAppManager = selected;
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel)]
        public void DialogCancel()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            var selected = SelectedAppManager;
            CloseTopDialog();
            SelectedAppManager = selected;
        }
        #endregion

        #region Dialog select Workflow
        [ACMethodCommand("DialogWorkflow", Const.Ok, (short)MISort.Okay)]
        public void DialogWorkflowOK()
        {
            var selectedWF = _SelectedWorkflow;
            CloseTopDialog();
            _SelectedWorkflow = selectedWF;
        }

        [ACMethodCommand("DialogWorkflow", Const.Cancel, (short)MISort.Cancel)]
        public void DialogWorkflowCancel()
        {
            SelectedWorkflow = null;
            CloseTopDialog();
        }
        #endregion

        #region Routing

        [ACMethodInfo("", "en{'Set route'}de{'Route definieren'}", 600, true)]
        public void SetRoute()
        {
            IACVBBSORouteSelector routeSelector = ParentACComponent.ACUrlCommand("VBBSORouteSelector_Child") as IACVBBSORouteSelector;
            if (routeSelector == null)
            {
                Messages.Error(this, "Route selector is not installed", true);
                return;
            }
            if (!IsRoutingServiceAvailable)
            {
                Messages.Error(this, "Routing-Service is currently not available", true);
                return;
            }

            string targetCompACUrl = SelectedTarget.Module.ACUrlComponent;

            ACRoutingParameters routingParameters = new ACRoutingParameters()
            {
                RoutingService = this.RoutingService,
                Database = this.Database.ContextIPlus,
                Direction = RouteDirections.Backwards,
                SelectionRuleID = PAProcessModule.SelRuleID_ProcessModule,
                MaxRouteAlternativesInLoop = 1,
                IncludeReserved = true,
                IncludeAllocated = true,
            };

            var sources = ACRoutingService.MemFindSuccessors(targetCompACUrl, routingParameters);
            if (sources == null)
            {
                Messages.Info(this, string.Format("Successors are not found for the component with ACUrl", targetCompACUrl), true);
                return;
            }

            if (sources != null && sources.Message != null)
            {
                Messages.Msg(sources.Message);
                return;
            }

            //IEnumerable<string> sourceCompsACUrl = sources.Routes.Select(c => c.FirstOrDefault().Source.ACUrlComponent);

            routeSelector.ShowAvailableRoutes(sources.Routes.Select(c => c.FirstOrDefault().Source), new core.datamodel.ACClass[] { SelectedTarget.Module }, null, null, true, SelectedModule?.Module);

            if (routeSelector.RouteResult != null)
            {
                if (routeSelector.RouteResult.Count() > 1)
                    SelectedTarget.CurrentRoute = Route.MergeRoutes(routeSelector.RouteResult);
                else
                    SelectedTarget.CurrentRoute = routeSelector.RouteResult.FirstOrDefault();
            }

        }

        public bool IsEnabledSetRoute()
        {
            return SelectedTarget != null && SelectedTarget.SelectedReservation != null;
        }

        [ACMethodInfo("", "en{'View route'}de{'Route anzeigen'}", 601, true)]
        public void OpenRoute()
        {
            IACVBBSORouteSelector routeSelector = ParentACComponent.ACUrlCommand("VBBSORouteSelector_Child") as IACVBBSORouteSelector;
            if (routeSelector == null)
            {
                Messages.Error(this, "Route selector is not installed");
                return;
            }

            //set flag to true if route is read only
            routeSelector.EditRoutesWithAttach(SelectedTarget.CurrentRoute, false, true, true);

            if (routeSelector.RouteResult != null && SelectedTarget.CurrentRoute == null)
            {
                if (routeSelector.RouteResult != null)
                {
                    if (routeSelector.RouteResult.Count() > 1)
                        SelectedTarget.CurrentRoute = Route.MergeRoutes(routeSelector.RouteResult);
                    else
                        SelectedTarget.CurrentRoute = routeSelector.RouteResult.FirstOrDefault();
                }
            }
        }

        public bool IsEnabledOpenRoute()
        {
            return SelectedTarget != null && SelectedTarget.CurrentRoute != null;
        }

        #endregion

        #endregion

        #region virtual methods
        public virtual IList<gip.core.datamodel.ACClass> GetAvailableModulesAsACClass(DeliveryNotePos deliveryNotePos, Database db)
        {
            BSOInDeliveryNote bso = ParentACComponent as BSOInDeliveryNote;
            if (bso == null)
                return new gip.core.datamodel.ACClass[0];
            FacilityManager facilityManager = bso.ACFacilityManager as FacilityManager;
            if (facilityManager == null)
                return new gip.core.datamodel.ACClass[0];
            return facilityManager.GetAvailableIntakeModulesAsACClass(this.Database.ContextIPlus);
        }

        public virtual IList<gip.core.datamodel.ACClass> GetAvailableTargetsAsACClass(DeliveryNotePos deliveryNotePos, gip.core.datamodel.ACClass sourceModule, Database db)
        {
            BSOInDeliveryNote bso = ParentACComponent as BSOInDeliveryNote;
            if (bso == null)
                return new gip.core.datamodel.ACClass[0];
            FacilityManager facilityManager = bso.ACFacilityManager as FacilityManager;
            if (facilityManager == null)
                return new gip.core.datamodel.ACClass[0];
            return facilityManager.GetAvailableTargetsAsACClass(deliveryNotePos, SelectedModule.Module, this.Database.ContextIPlus);
        }

        #endregion
    }
}
