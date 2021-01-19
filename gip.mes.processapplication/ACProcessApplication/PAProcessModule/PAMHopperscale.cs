using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;
using gip.mes.datamodel;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Hopperscale'}de{'Behälterwaage'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMHopperscale : PAProcessModuleVB, IPAMContScale
    {
        static PAMHopperscale()
        {
            RegisterExecuteHandler(typeof(PAMHopperscale), HandleExecuteACMethod_PAMHopperscale);
        }

        public PAMHopperscale(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _PAPointMatIn1 = new PAPoint(this, Const.PAPointMatIn1);
            _PAPointMatOut1 = new PAPoint(this, Const.PAPointMatOut1);
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
        [ACPointStateInfo(GlobalProcApp.AvailabilityStatePropName, GlobalProcApp.AvailabilityState.Idle, GlobalProcApp.AvailabilityStateGroupName, "", Global.Operators.none)]
        public PAPoint PAPointMatOut1
        {
            get
            {
                return _PAPointMatOut1;
            }
        }
        #endregion

        #region Properties
        [ACPropertyBindingSource(401, "Config", "en{'Blocked'}de{'Gesperrt'}", "", true, true, DefaultValue=false)]
        public IACContainerTNet<Boolean> IsBlocked { get; set; }

        [ACPropertyBindingTarget(402, "Read from PLC", "en{'Filling volume [dm³]'}de{'Füllvolumen [dm³]'}", "", false, true)]
        public IACContainerTNet<Double> FillVolume { get; set; }


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


        #endregion

        #region Methods

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMHopperscale(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessModuleVB(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #endregion

        #region public
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

    }
}
