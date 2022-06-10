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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Platformscale'}de{'Plattformwaage'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMPlatformscale : PAProcessModuleVB
    {
        #region c'tors
        static PAMPlatformscale()
        {
            RegisterExecuteHandler(typeof(PAMPlatformscale), HandleExecuteACMethod_PAMPlatformscale);
        }

        public PAMPlatformscale(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _PAPointMatIn1 = new PAPoint(this, nameof(PAPointMatIn1));
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

        PAPoint _PAPointMatOut1;
        [ACPropertyConnectionPoint(9999, "PointMaterial")]
        public PAPoint PAPointMatOut1
        {
            get
            {
                return _PAPointMatOut1;
            }
        }
        #endregion

        #region Methods

        public List<PAEScaleBase> GetWeightDetectionScales()
        {
            IPAFuncScaleConfig scaleConfig = FindChildComponents<PAFManualWeighing>(c => c is PAFManualWeighing && !(c is PAFManualAddition)).FirstOrDefault();
            if (scaleConfig != null)
            {
                return scaleConfig.ScaleMappingHelper.AssignedScales.ToList();
            }
            else
            {
                scaleConfig = FindChildComponents<PAFDosing>(c => c is PAFDosing).FirstOrDefault();
                if (scaleConfig != null)
                {
                    return scaleConfig.ScaleMappingHelper.AssignedScales.ToList();
                }
            }

            return new List<PAEScaleBase>();
        }

        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMPlatformscale(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessModuleVB(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

    }
}
