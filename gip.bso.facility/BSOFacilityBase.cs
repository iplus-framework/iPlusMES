﻿// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 11-08-2012
// ***********************************************************************
// <copyright file="BSOFacilityBase.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using gip.mes.datamodel; using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.facility;
using System.Data.Objects;
using gip.mes.autocomponent;
using System.Runtime.CompilerServices;

namespace gip.bso.facility
{


    /// <summary>
    /// BSOFacilityBase dient zur Einlagerung, Umlagerung und Ausbuchung von Chargen
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'BSOFacilityBase'}de{'BSOFacilityBase'}", Global.ACKinds.TACAbstractClass, Global.ACStorableTypes.NotStorable, true, true)]
    public abstract class BSOFacilityBase : ACBSOvbNav
    {
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

        protected ACRef<IACPickingManager> _ACPickingManager = null;
        public IACPickingManager ACPickingManager
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



        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityBase"/> class.
        /// </summary>
        /// <param name="typeACClass">The type AC class.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityBase(gip.core.datamodel.ACClass typeACClass, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(typeACClass, content, parentACObject, parameter)
        {
        }

        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _ACPickingManager = ACRefToPickingManager();

            _RoutingService = ACRoutingService.ACRefToServiceInstance(this);

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            if (_ACPickingManager != null)
                DetachACRefToPickingManager(_ACPickingManager);
            _ACPickingManager = null;

            ACRoutingService.DetachACRefFromServiceInstance(this, _RoutingService);
            _RoutingService = null;

            return base.ACDeInit(deleteACClassTask);
        }

        public override DatabaseApp DatabaseApp
        {
            get
            {
                return base.DatabaseApp;
            }
        }

        protected ACRef<IACPickingManager> ACRefToPickingManager()
        {
            // Falls als Unterobjekt Konfiguriert:
            IACPickingManager facilityMgr = this.ACUrlCommand("PickingManager") as IACPickingManager;

            // Falls als lokaler Dienst konfiguriert
            if (facilityMgr == null)
            {
                if (this.Root == null || this.Root.InitState == ACInitState.Destructing || this.Root.InitState == ACInitState.Destructed)
                    return null;

                facilityMgr = this.ACUrlCommand("\\LocalServiceObjects\\PickingManager") as IACPickingManager;

                // Falls als Service Konfiguriert
                if (facilityMgr == null)
                    facilityMgr = this.ACUrlCommand("\\Service\\PickingManager") as IACPickingManager;
            }

            if (facilityMgr != null)
                return new ACRef<IACPickingManager>(facilityMgr, this);
            return null;
        }

        protected void DetachACRefToPickingManager(ACRef<IACPickingManager> acRef)
        {
            if (acRef == null)
                return;
            IACPickingManager manager = acRef.ValueT;
            acRef.Detach();
            if (manager != null)
            {
                if (manager.ParentACComponent == (this.Root as ACRoot).LocalServiceObjects)
                {
                    if (!manager.ReferencePoint.HasStrongReferences)
                    {
                        manager.Stop();
                    }
                }
            }
        }

        #endregion

        #region Properties
        #region Workflow-Selection
        protected IList<gip.core.datamodel.ACClass> _SourceModuleList;
        [ACPropertyList(601, "SourceModule")]
        public IList<gip.core.datamodel.ACClass> SourceModuleList
        {
            get
            {
                return _SourceModuleList;
            }
            set
            {
                _SourceModuleList = value;
                OnPropertyChanged("SourceModuleList");
            }
        }

        protected gip.core.datamodel.ACClass _SelectedSourceModule;
        [ACPropertySelected(602, "SourceModule")]
        public gip.core.datamodel.ACClass SelectedSourceModule
        {
            get
            {
                return _SelectedSourceModule;
            }
            set
            {
                if (_SelectedSourceModule != value)
                {
                    _SelectedSourceModule = value;
                    OnPropertyChanged("SelectedSourceModule");
                }
            }
        }

        protected List<gip.core.datamodel.ACClassMethod> _WorkflowList;
        [ACPropertyList(603, "Workflow", "en{'Select Workflow'}de{'Workflow auswählen'}")]
        public IEnumerable<gip.core.datamodel.ACClassMethod> WorkflowList
        {
            get
            {
                return _WorkflowList;
            }
        }

        protected gip.core.datamodel.ACClassMethod _SelectedWorkflow;
        [ACPropertySelected(604, "Workflow", "en{'Select Workflow'}de{'Workflow auswählen'}")]
        public gip.core.datamodel.ACClassMethod SelectedWorkflow
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

        protected ACComponent _SelectedAppManager;
        [ACPropertySelected(605, "AppManagers")]
        public ACComponent SelectedAppManager
        {
            get
            {
                return _SelectedAppManager;
            }
            set
            {
                _SelectedAppManager = value;
                OnPropertyChanged("SelectedAppManager");
            }
        }

        protected List<ACComponent> _AppManagersList;
        [ACPropertyList(606, "AppManagers")]
        public List<ACComponent> AppManagersList
        {
            get
            {
                return _AppManagersList;
            }
            set
            {
                _AppManagersList = value;
                OnPropertyChanged("AppManagersList");
            }
        }

        [ACPropertyCurrent(607, "IsPhysicalTransportPossible")]
        public virtual bool IsPhysicalTransportPossible
        {
            get
            {
                return false;
            }
        }

        #endregion
        #endregion

        #region Methods
        #region Workflow-Starting
        protected virtual bool PrepareStartWorkflow(ACMethodBooking forBooking, out gip.core.datamodel.ACClassMethod acClassMethod, out bool wfRunsBatches)
        {
            string pwClassNameOfRoot = GetPWClassNameOfRoot(forBooking);
            acClassMethod = null;
            wfRunsBatches = false;
            SelectedSourceModule = null;
            Msg msg = null;

            IEnumerable<gip.core.datamodel.ACClassWF> workflowRootWFs = null;
            // Falls Umlagerung eines Quants, dann muss Benutzer gefargt werden an welcher Aufgabestelle er das Quant aufgeben will für einen Transport
            if (forBooking.OutwardFacilityCharge != null)
            {
                if (!forBooking.InwardFacility.VBiFacilityACClassID.HasValue)
                    return false;
                SourceModuleList = this.ACFacilityManager.GetAvailableIntakeModulesAsACClass(this.Database.ContextIPlus);
                if (SourceModuleList.Count <= 0)
                    return false;
                else if (SourceModuleList.Count > 1)
                {
                    ShowDialog(this, "SelectSourceModule");
                    if (SelectedSourceModule == null)
                        return false;
                }
                else
                    SelectedSourceModule = SourceModuleList.FirstOrDefault();

                if (forBooking.InwardFacility == null || !forBooking.InwardFacility.VBiFacilityACClassID.HasValue)
                    return false;
                msg = OnValidateRoutesForWF(forBooking, SelectedSourceModule, forBooking.InwardFacility.FacilityACClass);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return false;
                }

                gip.core.datamodel.ACClass sourceDefClass = SelectedSourceModule.ACClass1_BasedOnACClass;
                while (sourceDefClass != null)
                {
                    if (sourceDefClass.ACProject == null)
                    {
                        sourceDefClass = null;
                        break;
                    }
                    if (sourceDefClass.ACProject.ACProjectType == Global.ACProjectTypes.AppDefinition)
                        break;
                    sourceDefClass = sourceDefClass.ACClass1_BasedOnACClass;
                }

                if (sourceDefClass == null)
                    return false;

                if (!sourceDefClass.ACClassWF_RefPAACClass.Any())
                {
                    //Error50123: No workflow found for this intake place.
                    msg = new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "PrepareStartWorkflow(10)",
                        Message = Root.Environment.TranslateMessage(this, "Error50123")
                    };
                    Messages.Msg(msg);
                    return false;
                }
                workflowRootWFs = sourceDefClass.ACClassWF_RefPAACClass.Where(c => c.ParentACClassWFID.HasValue
                                                                && !c.ACClassWF1_ParentACClassWF.ParentACClassWFID.HasValue
                                                                && (c.ACClassWF1_ParentACClassWF.PWACClass.ACIdentifier == pwClassNameOfRoot
                                                                    || (c.ACClassWF1_ParentACClassWF.PWACClass.BasedOnACClassID.HasValue
                                                                            && (c.ACClassWF1_ParentACClassWF.PWACClass.ACClass1_BasedOnACClass.ACIdentifier == pwClassNameOfRoot
                                                                                || (c.ACClassWF1_ParentACClassWF.PWACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue && c.ACClassWF1_ParentACClassWF.PWACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pwClassNameOfRoot))))
                                                                && !c.ACClassMethod.IsSubMethod)
                                                     .Select(c => c.ACClassWF1_ParentACClassWF);
            }
            // Sonst Umlagerungsprozess von Silo zu Silo
            else
            {
                if (forBooking.OutwardFacility == null || !forBooking.OutwardFacility.VBiFacilityACClassID.HasValue
                    || forBooking.InwardFacility == null || !forBooking.InwardFacility.VBiFacilityACClassID.HasValue)
                    return false;
                msg = OnValidateRoutesForWF(forBooking, forBooking.OutwardFacility.FacilityACClass, forBooking.InwardFacility.FacilityACClass);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return false;
                }

                workflowRootWFs = this.Database.ContextIPlus.ACClassWF.Where(c => !c.ParentACClassWFID.HasValue
                                                                && (c.PWACClass.ACIdentifier == pwClassNameOfRoot
                                                                    || (c.PWACClass.BasedOnACClassID.HasValue 
                                                                        && (c.PWACClass.ACClass1_BasedOnACClass.ACIdentifier == pwClassNameOfRoot
                                                                            || (c.PWACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue && c.PWACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pwClassNameOfRoot))))
                                                                && !c.ACClassMethod.IsSubMethod);

            }

            SelectedWorkflow = null;
            gip.core.datamodel.ACClassWF wfNode = null;
            if (!workflowRootWFs.Any())
            {
                //Error50124: No workflow found for this transport.
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "PrepareStartWorkflow(20)",
                    Message = Root.Environment.TranslateMessage(this, "Error50124")
                };
                Messages.Msg(msg);
                return false;
            }
            // User must select Workflow for Intaking:
            else if (workflowRootWFs.Count() > 1)
            {
                _WorkflowList = workflowRootWFs.Select(c => c.ACClassMethod).ToList();
                OnPropertyChanged("WorkflowList");
                ShowDialog(this, "SelectProcessWorkflow");
                //wfNode = queryIntakeRootWFs.FirstOrDefault();
                if (SelectedWorkflow == null)
                    return false;
                acClassMethod = SelectedWorkflow;
            }
            else
            {
                wfNode = workflowRootWFs.FirstOrDefault();
                if (wfNode == null || wfNode.ACClassMethod == null)
                    return false;
                acClassMethod = wfNode.ACClassMethod;
                SelectedWorkflow = acClassMethod;
            }

            if (SelectedWorkflow == null && (wfNode == null || wfNode.ACClassMethod == null))
                return false;

            if (acClassMethod == null)
                return false;

            Type typePWWF = typeof(PWNodeProcessWorkflow);
            wfRunsBatches = acClassMethod.ACClassWF_ACClassMethod.ToArray().Where(c => c.RefPAACClassMethodID.HasValue && c.PWACClass != null && c.PWACClass.ObjectType != null && typePWWF.IsAssignableFrom(c.PWACClass.ObjectType)).Any();
            gip.core.datamodel.ACProject project = acClassMethod.ACClass.ACProject as gip.core.datamodel.ACProject;

            SelectedAppManager = null;
            AppManagersList = this.Root.FindChildComponents(project.RootClass, 1).Select(c => c as ACComponent).ToList();
            if (AppManagersList.Count > 1)
            {
                ShowDialog(this, "SelectAppManager");
                if (SelectedAppManager == null)
                    return false;
            }
            else
                SelectedAppManager = AppManagersList.FirstOrDefault();

            ACComponent pAppManager = SelectedAppManager as ACComponent;
            if (pAppManager == null)
                return false;
            if (pAppManager.IsProxy && pAppManager.ConnectionState == ACObjectConnectionState.DisConnected)
            {
                // TODO: Message
                return false;
            }
            return true;
        }

        public virtual string GetPWClassNameOfRoot(ACMethodBooking forBooking)
        {
            if (this.ACFacilityManager != null)
                return this.ACFacilityManager.C_PWMethodRelocClass;
            return "PWMethodRelocation";
        }

        protected virtual bool StartWorkflow(gip.core.datamodel.ACClassMethod acClassMethod, FacilityBooking facilityBooking)
        {
            ACMethod acMethod = SelectedAppManager.NewACMethod(acClassMethod.ACIdentifier);
            if (acMethod == null)
                return false;

            if (SelectedSourceModule != null)
                facilityBooking.PropertyACUrl = SelectedSourceModule.ACUrlComponent;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(gip.core.datamodel.ACProgram), gip.core.datamodel.ACProgram.NoColumnName, gip.core.datamodel.ACProgram.FormatNewNo, this);
            gip.core.datamodel.ACProgram program = gip.core.datamodel.ACProgram.NewACObject(this.Database.ContextIPlus, null, secondaryKey);
            program.ProgramACClassMethod = acClassMethod;
            program.WorkflowTypeACClass = acClassMethod.WorkflowTypeACClass;
            this.Database.ContextIPlus.ACProgram.AddObject(program);
            if (ACSaveChanges())
            {
                ACValue paramProgram = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACProgram.ClassName);
                if (paramProgram == null)
                    acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACProgram.ClassName, typeof(Guid), program.ACProgramID));
                else
                    paramProgram.Value = program.ACProgramID;

                ACValue acValue = acMethod.ParameterValueList.GetACValue(FacilityBooking.ClassName);
                if (acValue == null)
                    acMethod.ParameterValueList.Add(new ACValue(FacilityBooking.ClassName, typeof(Guid), facilityBooking.FacilityBookingID));
                else
                    acValue.Value = facilityBooking.FacilityBookingID;

                SelectedAppManager.ExecuteMethod(acClassMethod.ACIdentifier, acMethod);
                return true;
            }
            return false;
        }

        protected virtual bool StartWorkflow(gip.core.datamodel.ACClassMethod acClassMethod, Picking picking)
        {
            ACMethod acMethod = SelectedAppManager.NewACMethod(acClassMethod.ACIdentifier);
            if (acMethod == null)
                return false;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(gip.core.datamodel.ACProgram), gip.core.datamodel.ACProgram.NoColumnName, gip.core.datamodel.ACProgram.FormatNewNo, this);
            gip.core.datamodel.ACProgram program = gip.core.datamodel.ACProgram.NewACObject(this.Database.ContextIPlus, null, secondaryKey);
            program.ProgramACClassMethod = acClassMethod;
            program.WorkflowTypeACClass = acClassMethod.WorkflowTypeACClass;
            this.Database.ContextIPlus.ACProgram.AddObject(program);
            if (ACSaveChanges())
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

                SelectedAppManager.ExecuteMethod(acClassMethod.ACIdentifier, acMethod);
                return true;
            }
            return false;
        }

        protected virtual Msg OnValidateRoutesForWF(ACMethodBooking forBooking, gip.core.datamodel.ACClass fromClass, gip.core.datamodel.ACClass toClass)
        {
            Msg msg = null;
            string siloClass = this.ACFacilityManager.C_SiloClass;
            gip.core.datamodel.ACClass siloACClass = this.ACFacilityManager.GetACClassForIdentifier(siloClass,this.Database.ContextIPlus);
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

            RoutingResult result = ACRoutingService.SelectRoutes(RoutingService, this.Database.ContextIPlus, false,
                                    fromClass, toClass, RouteDirections.Forwards, "", new object[] { },
                                    (c, p, r) => c.ACClassID == toClass.ACClassID,
                                    (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && (fromClass.ACClassID == c.ACClassID || typeSilo.IsAssignableFrom(c.ObjectType)),
                                    0, true, true, false, false, 10);
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

            return msg;
        }

        protected virtual bool BookRelocation()
        {
            return true;
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "DialogSourceModuleOK":
                    DialogSourceModuleOK();
                    return true;
                case "DialogSourceModuleCancel":
                    DialogSourceModuleCancel();
                    return true;
                case "DialogWorkflowOK":
                    DialogWorkflowOK();
                    return true;
                case "DialogWorkflowCancel":
                    DialogWorkflowCancel();
                    return true;
                case "DialogAppManagerOK":
                    DialogAppManagerOK();
                    return true;
                case "DialogAppManagerCancel":
                    DialogAppManagerCancel();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


        #region Workflow-Dialog's
        [ACMethodCommand("DialogIntakeModule", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
        public void DialogSourceModuleOK()
        {
            var selectedWF = _SelectedSourceModule;
            CloseTopDialog();
            _SelectedSourceModule = selectedWF;
        }

        [ACMethodCommand("DialogIntakeModule", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
        public void DialogSourceModuleCancel()
        {
            SelectedSourceModule = null;
            CloseTopDialog();
        }

        [ACMethodCommand("DialogWorkflow", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
        public void DialogWorkflowOK()
        {
            var selectedWF = _SelectedWorkflow;
            CloseTopDialog();
            _SelectedWorkflow = selectedWF;
        }

        [ACMethodCommand("DialogWorkflow", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
        public void DialogWorkflowCancel()
        {
            SelectedWorkflow = null;
            CloseTopDialog();
        }

        [ACMethodCommand("DialogAppManager", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
        public void DialogAppManagerOK()
        {
            var selectedWF = _SelectedAppManager;
            CloseTopDialog();
            _SelectedAppManager = selectedWF;
        }

        [ACMethodCommand("DialogAppManager", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
        public void DialogAppManagerCancel()
        {
            SelectedAppManager = null;
            CloseTopDialog();
        }
        #endregion
        #endregion

    }

}