// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
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
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.facility;
using gip.mes.autocomponent;
using System.Runtime.CompilerServices;
using gip.mes.processapplication;

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

            _OpenPickingBeforeStart = new ACPropertyConfigValue<bool>(this, nameof(OpenPickingBeforeStart), false);

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

        public override bool ACPostInit()
        {
            _ = OpenPickingBeforeStart;
            return base.ACPostInit();
        }

        public override DatabaseApp DatabaseApp
        {
            get
            {
                return base.DatabaseApp;
            }
        }

        protected ACRef<ACPickingManager> ACRefToPickingManager()
        {
            // Falls als Unterobjekt Konfiguriert:
            ACPickingManager facilityMgr = this.ACUrlCommand("PickingManager") as ACPickingManager;

            // Falls als lokaler Dienst konfiguriert
            if (facilityMgr == null)
            {
                if (this.Root == null || this.Root.InitState == ACInitState.Destructing || this.Root.InitState == ACInitState.Destructed)
                    return null;

                facilityMgr = this.ACUrlCommand("\\LocalServiceObjects\\PickingManager") as ACPickingManager;

                // Falls als Service Konfiguriert
                if (facilityMgr == null)
                    facilityMgr = this.ACUrlCommand("\\Service\\PickingManager") as ACPickingManager;
            }

            if (facilityMgr != null)
                return new ACRef<ACPickingManager>(facilityMgr, this);
            return null;
        }

        protected void DetachACRefToPickingManager(ACRef<ACPickingManager> acRef)
        {
            if (acRef == null)
                return;
            ACPickingManager manager = acRef.ValueT;
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
        [ACPropertyList(601, "SourceModule", "en{'Select point of transport'}de{'Transportpunkt auswählen'}")]
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
        [ACPropertySelected(602, "SourceModule", "en{'Select point of transport'}de{'Transportpunkt auswählen'}")]
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

        [ACPropertyInfo(607, "IsPhysicalTransportPossible", "en{'Is physical transport allowed'}de{'Physikalischer Transport erlaubt'}", "", true)]
        public virtual bool IsPhysicalTransportPossible
        {
            get
            {
                return false;
            }
        }

        public bool HasRightsForPhysicalTransport
        {
            get
            {
                ClassRightManager rightManager = CurrentRightsOfInvoker;
                if (rightManager == null)
                    return true;
                IACPropertyBase acProperty = this.GetProperty(nameof(IsPhysicalTransportPossible));
                if (acProperty == null)
                    return true;
                Global.ControlModes rightMode = rightManager.GetControlMode(acProperty.ACType);
                return !(rightMode == Global.ControlModes.Collapsed || rightMode == Global.ControlModes.Disabled);

            }
        }

        private ACPropertyConfigValue<bool> _OpenPickingBeforeStart;
        [ACPropertyConfig("en{'Open picking order before start workflow'}de{'Kommissionierauftrag öffnen vor Workflowstart'}")]
        public bool OpenPickingBeforeStart
        {
            get
            {
                return _OpenPickingBeforeStart.ValueT;
            }
            set
            {
                _OpenPickingBeforeStart.ValueT = value;
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
            // Falls Umlagerung eines Quants, dann muss Benutzer gefragt werden an welcher Aufgabestelle er das Quant aufgeben will für einen Transport
            if (forBooking.OutwardFacilityCharge != null
                && (forBooking.OutwardFacilityCharge.Facility.MDFacilityType == null || forBooking.OutwardFacilityCharge.Facility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBinContainer))
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
            else if ((forBooking.OutwardFacility != null
                       || (forBooking.OutwardFacilityCharge != null && forBooking.OutwardFacilityCharge.Facility.MDFacilityType != null && forBooking.OutwardFacilityCharge.Facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer))
                     && forBooking.InwardFacility != null)
            {
                Facility outwardFacility = forBooking.OutwardFacility != null ? forBooking.OutwardFacility : forBooking.OutwardFacilityCharge.Facility;
                if (!outwardFacility.VBiFacilityACClassID.HasValue
                    || !forBooking.InwardFacility.VBiFacilityACClassID.HasValue)
                    return false;
                msg = OnValidateRoutesForWF(forBooking, outwardFacility.FacilityACClass, forBooking.InwardFacility.FacilityACClass);
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
            // Sonst Einlagerungsprozess in Silo
            else if (forBooking.OutwardFacility == null && forBooking.InwardFacility != null)
            {
                if (!forBooking.InwardFacility.VBiFacilityACClassID.HasValue)
                    return false;
                Type typeIntake = typeof(PAMIntake);

                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    RoutingService = this.RoutingService,
                    Database = this.Database.ContextIPlus,
                    SelectionRuleID = PAMIntake.SelRuleID_PAMIntake,
                    Direction = RouteDirections.Backwards,
                    AttachRouteItemsToContext = true,
                    DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && typeIntake.IsAssignableFrom(c.ObjectType),
                    DBDeSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != forBooking.InwardFacility.VBiFacilityACClassID,
                    MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                    IncludeReserved = true,
                    IncludeAllocated = true,
                    AutoDetachFromDBContext = false,
                    DBIncludeInternalConnections = false,
                    DBRecursionLimit = 10,
                    DBIgnoreRecursionLoop = false,
                    ForceReattachToDatabaseContext = true
                };

                RoutingResult result = ACRoutingService.FindSuccessors(forBooking.InwardFacility.FacilityACClass, routingParameters);
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
                    Messages.Msg(msg);
                    return false;
                }

                // Remove Components that are not in automatic operation mode
                List<RouteItem> routeItemsWithComp = result.Routes.Select(c => c.FirstOrDefault()).ToList();
                if (routeItemsWithComp != null && routeItemsWithComp.Any())
                {
                    foreach (RouteItem routeItem in routeItemsWithComp.ToArray())
                    {
                        if (routeItem.SourceACComponent != null && routeItem.SourceACComponent.ConnectionState == ACObjectConnectionState.ValuesReceived)
                        {
                            IACContainerTNet<Global.OperatingMode> property = routeItem.SourceACComponent.GetPropertyNet(nameof(PAClassPhysicalBase.OperatingMode)) as IACContainerTNet<Global.OperatingMode>;
                            if (property != null && property.ValueT != Global.OperatingMode.Automatic)
                                routeItemsWithComp.Remove(routeItem);
                        }
                    }
                }

                SourceModuleList = routeItemsWithComp.Select(c => c.Source).OrderBy(c => c.ACIdentifier).ToList();
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
            // Sonst Auslagerungsprozess in Silo
            else if (forBooking.OutwardFacility != null && forBooking.InwardFacility == null)
            {
                return false;
            }
            else
            {
                return false;
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

            if (!SelectAppManager(project))
            {
                return false;
            }

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

        public virtual bool SelectAppManager(gip.core.datamodel.ACProject project)
        {
            SelectedAppManager = null;
            AppManagersList = this.Root.FindChildComponents(project.RootClass, 1).Select(c => c as ACComponent).ToList();
            if (AppManagersList.Count > 1)
            {
                ShowDialog(this, "SelectAppManager");
                if (SelectedAppManager == null)
                    return false;
            }
            else
            {
                SelectedAppManager = AppManagersList.FirstOrDefault();
            }
            return true;
        }

        public virtual string GetPWClassNameOfRoot(ACMethodBooking forBooking)
        {
            if ((forBooking.OutwardFacilityCharge != null || forBooking.OutwardFacility != null)
                    && forBooking.InwardFacility != null)
            {
                if (this.ACFacilityManager != null)
                    return this.ACFacilityManager.C_PWMethodRelocClass;
                else
                    return nameof(PWMethodRelocation);
            }
            else if (forBooking.OutwardFacility == null && forBooking.InwardFacility != null)
            {
                if (this.ACFacilityManager != null)
                    return this.ACFacilityManager.C_PWMethodIntakeClass;
                else
                    return nameof(PWMethodIntake);
            }
            else if (forBooking.OutwardFacility != null && forBooking.InwardFacility == null)
            {
                if (this.ACFacilityManager != null)
                    return this.ACFacilityManager.C_PWMethodLoadingClass;
                else
                    return nameof(PWMethodLoading);
            }

            return nameof(PWMethodRelocation);
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
            this.Database.ContextIPlus.ACProgram.Add(program);
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
            this.Database.ContextIPlus.ACProgram.Add(program);
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
        [ACMethodCommand("DialogIntakeModule", Const.Ok, (short)MISort.Okay)]
        public void DialogSourceModuleOK()
        {
            var selectedWF = _SelectedSourceModule;
            CloseTopDialog();
            _SelectedSourceModule = selectedWF;
        }

        [ACMethodCommand("DialogIntakeModule", Const.Cancel, (short)MISort.Cancel)]
        public void DialogSourceModuleCancel()
        {
            SelectedSourceModule = null;
            CloseTopDialog();
        }

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

        [ACMethodCommand("DialogAppManager", Const.Ok, (short)MISort.Okay)]
        public void DialogAppManagerOK()
        {
            var selectedWF = _SelectedAppManager;
            CloseTopDialog();
            _SelectedAppManager = selectedWF;
        }

        [ACMethodCommand("DialogAppManager", Const.Cancel, (short)MISort.Cancel)]
        public void DialogAppManagerCancel()
        {
            SelectedAppManager = null;
            CloseTopDialog();
        }
        #endregion
        #endregion

    }

}
