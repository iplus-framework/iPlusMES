// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;
using gip.mes.facility;
using gip.mes.datamodel;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Loadingstation'}de{'Befüllstation'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMLoadingstation : PAProcessModuleVB, IPAMContScale
    {
        static PAMLoadingstation()
        {
            RegisterExecuteHandler(typeof(PAMLoadingstation), HandleExecuteACMethod_PAMLoadingstation);
        }

        public PAMLoadingstation(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _PAPointMatIn1 = new PAPoint(this, nameof(PAPointMatIn1));
            _PAPointMatOut1 = new PAPoint(this, nameof(PAPointMatOut1));
            _MDSchedulingKey = new ACPropertyConfigValue<string>(this, nameof(MDSchedulingKey), "");
            _EventSubscr = new ACPointEventSubscr(this, nameof(EventSubscr), 0);

        }

        public override bool ACPostInit()
        {
            _ = MDSchedulingKey;
            PAEScannerDecoderVB scanner = FindChildComponents<PAEScannerDecoderVB>(c => c is PAEScannerDecoderVB, null, 1).FirstOrDefault();
            if (scanner != null)
            {
                _Scanner = new ACRef<PAEScannerDecoderVB>(scanner, this);
                EventSubscr.SubscribeEvent(scanner, nameof(PAEScannerDecoderVB.ScanSequenceCompleteEvent), EventCallback);
            }
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_Scanner != null)
            {
                _Scanner.Detach();
                _Scanner = null;
            }
            if (_Scheduler != null)
            {
                _Scheduler.Detach();
                _Scheduler = null;
            }
            _MDSchedulingID = null;
            return base.ACDeInit(deleteACClassTask);
        }

        #region Points
        PAPoint _PAPointMatIn1;
        [ACPropertyConnectionPoint(9999, "PointMaterial")]
        public PAPoint PAPointMatIn1
        {
            get
            {
                return _PAPointMatIn1;
            }
        }

        PAPoint _PAPointMatOut1;
        [ACPropertyConnectionPoint(9999, "PointMaterial")]
        [ACPointStateInfo(GlobalProcApp.AvailabilityStatePropName, core.processapplication.AvailabilityState.Idle, GlobalProcApp.AvailabilityStateGroupName, "", Global.Operators.none)]
        public PAPoint PAPointMatOut1
        {
            get
            {
                return _PAPointMatOut1;
            }
        }

        ACPointEventSubscr _EventSubscr;
        [ACPropertyEventPointSubscr(9999, false)]
        public ACPointEventSubscr EventSubscr
        {
            get
            {
                return _EventSubscr;
            }
        }

        protected ACRef<PAEScannerDecoderVB> _Scanner;
        public PAEScannerDecoderVB Scanner
        {
            get
            {
                return _Scanner?.ValueT;
            }
        }

        protected ACRef<PAWorkflowSchedulerBase> _Scheduler;
        public PAWorkflowSchedulerBase Scheduler
        {
            get
            {
                if (_Scheduler != null)
                    return _Scheduler.ValueT;
                if (MDSchedulingID == Guid.Empty)
                    return null;
                foreach (var appManager in this.Root.FindChildComponents<ApplicationManager>(c => c is ApplicationManager, null, 1))
                {
                    List<PAWorkflowSchedulerBase> schedulers = appManager.FindChildComponents<PAWorkflowSchedulerBase>(c => c is PAWorkflowSchedulerBase, null, 1);
                    if (schedulers != null && schedulers.Any())
                    {
                        foreach (PAWorkflowSchedulerBase scheduler in schedulers)
                        {
                            if (scheduler.IsSchedulerFor(MDSchedulingID))
                            {
                                _Scheduler = new ACRef<PAWorkflowSchedulerBase>(scheduler, this);
                                break;
                            }
                        }
                        if (_Scheduler == null)
                            _Scheduler = new ACRef<PAWorkflowSchedulerBase>(null, this);
                    }
                }
                return _Scheduler?.ValueT;
            }
        }

        public ApplicationManager PlanningAppManager
        {
            get
            {
                return Scheduler?.ApplicationManager;
            }
        }

        #endregion

        #region IPAMContScale
        public virtual PAEScaleGravimetric Scale
        {
            get
            {
                return PAMContScaleExtension.GetScale(this);
            }
        }

        public virtual bool IsScaleEmpty
        {
            get
            {
                return PAMContScaleExtension.IsScaleEmpty(this);
            }
        }

        public double? RemainingWeightCapacity
        {
            get
            {
                return PAMContScaleExtension.RemainingWeightCapacity(this);
            }
        }

        public double? MinDosingWeight
        {
            get
            {
                return PAMContScaleExtension.MinDosingWeight(this);
            }
        }

        public double? RemainingVolumeCapacity
        {
            get
            {
                return PAMContScaleExtension.RemainingVolumeCapacity(this);
            }
        }

        public IACContainerTNet<double> FillVolume { get; set; }
        #endregion


        #region Config
        private ACPropertyConfigValue<string> _MDSchedulingKey;
        [ACPropertyConfig("en{'Line assignment for self-service'}de{'Linienzuordnung für Selbstbedienung'}")]
        public virtual string MDSchedulingKey
        {
            get
            {
                return _MDSchedulingKey.ValueT;
            }
        }

        private Guid? _MDSchedulingID = null;
        public Guid MDSchedulingID
        {
            get
            {
                if (_MDSchedulingID.HasValue)
                    return _MDSchedulingID.Value;
                string schedulingKey = MDSchedulingKey;
                if (String.IsNullOrEmpty(MDSchedulingKey))
                    schedulingKey = this.ACIdentifier;
                try
                {
                    using (DatabaseApp dbApp = new DatabaseApp())
                    {
                        var query = dbApp.MDSchedulingGroup.Where(c => c.MDKey == schedulingKey).Select(c => c.MDSchedulingGroupID);
                        _MDSchedulingID = query.Any() ? query.FirstOrDefault() : Guid.Empty;
                        return _MDSchedulingID.Value;
                    }
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }
        #endregion


        #region Properties
        #endregion

        #region Methods
        [ACMethodInfo("Function", "en{'EventCallback'}de{'EventCallback'}", 9999)]
        public void EventCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            if (e != null)
            {
                if (sender.ACIdentifier == nameof(PAEScanner.ScanSequenceCompleteEvent))
                {
                    HandleDetectedOrderInfo(e);
                }
            }
        }

        protected virtual void HandleDetectedOrderInfo(ACEventArgs e)
        {
            ACValue acValue = e.GetACValue(nameof(PAOrderInfo));
            if (acValue == null)
                return;
            PAOrderInfo paOrderInfo = acValue.ValueT<PAOrderInfo>();
            if (paOrderInfo == null)
                return;
            PAOrderInfoEntry entityEntry = paOrderInfo.Entities.FirstOrDefault();
            if (entityEntry == null)
                return;
            ApplicationManager appManager = PlanningAppManager;
            if (appManager == null)
                return;
            if (this.IsOccupied) // TODO: Display Message
                return;

            List<PWNodeProcessWorkflowVB> queryNodes = appManager.FindChildComponents<PWNodeProcessWorkflowVB>(c => c is PWNodeProcessWorkflowVB
                                                && (c as PWNodeProcessWorkflowVB).MDSchedulingGroup != null
                                                && (c as PWNodeProcessWorkflowVB).MDSchedulingGroup.MDSchedulingGroupID == MDSchedulingID
                                                , null, 2);
            if (queryNodes == null || !queryNodes.Any())
                return;
            queryNodes.ForEach(c => c.RunNodeOnModuleTrigger(entityEntry, this));
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMLoadingstation(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessModuleVB(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        public void ResetFillVolume()
        {
        }
        #endregion

    }
}
