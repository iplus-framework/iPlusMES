using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Mixer'}de{'Mischer'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMMixer : PAProcessModuleVB, IPAMContScale
    {
        #region c'tors
        static PAMMixer()
        {
            RegisterExecuteHandler(typeof(PAMMixer), HandleExecuteACMethod_PAMMixer);
        }

        public PAMMixer(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _PAPointMatIn1 = new PAPoint(this, nameof(PAPointMatIn1));
            _PAPointMatIn2 = new PAPoint(this, nameof(PAPointMatIn2));
            _PAPointMatIn3 = new PAPoint(this, nameof(PAPointMatIn3));
            _PAPointMatIn4 = new PAPoint(this, nameof(PAPointMatIn4));
            _PAPointMatOut1 = new PAPoint(this, nameof(PAPointMatOut1));
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

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

        PAPoint _PAPointMatIn2;
        [ACPropertyConnectionPoint(9999, "PointMaterial")]
        public PAPoint PAPointMatIn2
        {
            get
            {
                return _PAPointMatIn2;
            }
        }

        PAPoint _PAPointMatIn3;
        [ACPropertyConnectionPoint(9999, "PointMaterial")]
        public PAPoint PAPointMatIn3
        {
            get
            {
                return _PAPointMatIn3;
            }
        }

        PAPoint _PAPointMatIn4;
        [ACPropertyConnectionPoint(9999, "PointMaterial")]
        public PAPoint PAPointMatIn4
        {
            get
            {
                return _PAPointMatIn4;
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
        #endregion

        #region Properties
        [ACPropertyBindingTarget(501, "Read from PLC", "en{'UnlockDoor'}de{'Tür Freigabe'}", "", false, false)]
        public IACContainerTNet<Boolean> UnlockDoor { get; set; }

        public virtual PAEScaleGravimetric Scale
        {
            get
            {
                return PAMContScaleExtension.GetScale(this);
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

        [ACPropertyBindingTarget(402, "Read from PLC", "en{'Filling volume [dm³]'}de{'Füllvolumen [dm³]'}", "", false, true)]
        public IACContainerTNet<Double> FillVolume { get; set; }


        public double? RemainingVolumeCapacity
        {
            get
            {
                return PAMContScaleExtension.RemainingVolumeCapacity(this);
            }
        }

        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMMixer(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessModuleVB(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region Public
        public override void Reset()
        {
            ResetFillVolume();
            base.Reset();
        }

        public void ResetFillVolume()
        {
            IACPropertyNetTarget fillVolumeProp = FillVolume as IACPropertyNetTarget;
            if (fillVolumeProp == null || fillVolumeProp.Source == null)
                FillVolume.ValueT = 0;
        }
        #endregion

        #region Damir Test

        private void DoSomethingWithIt()
        {
            // Search example for predecessors
            var querySourceComponents = PAPointMatIn1.ConnectionList
                                .Where(c => c.Source.ParentACComponent is PAMSilo)
                                .Select(c => c.Source.ParentACComponent as PAMSilo);

            // Search example for sucessors
            var endingConnPoints = PAPointMatOut1.ConnectionList
                                .Where(c => !c.Target.ParentACComponent.ACPointList.Where(d => d.ACIdentifier == "PAPointMatOut1").Any())
                                .Select(c => c.Target);

            // Search for a route using ACRoutingService
            using (Database db = new Database())
            {
                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    RoutingService = this.RoutingService,
                    Database = db,
                    AttachRouteItemsToContext = RoutingService != null && RoutingService.IsProxy,
                    SelectionRuleID = SelRuleID_ProcessModule,
                    Direction = RouteDirections.Forwards,
                    DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                    DBDeSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                    MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                    IncludeReserved = true,
                    IncludeAllocated = true
                };

                RoutingResult rResult = ACRoutingService.FindSuccessors(this.GetACUrl(), routingParameters);
                if (rResult.Routes != null && rResult.Routes.Any())
                {
                    Route firstFoundRouteToADest = rResult.Routes.FirstOrDefault();
                }
            }
        }

        #endregion

    }
}
