using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Parkingspace'}de{'Stellplatz'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMParkingspace : PAProcessModuleVB
    {
        public const string SelRuleID_ParkingSpace = "PAMParkingSpace";
        public const string SelRuleID_ParkingSpace_Deselector = "PAMParkingSpace.Deselector";

        static PAMParkingspace()
        {
            RegisterExecuteHandler(typeof(PAMParkingspace), HandleExecuteACMethod_PAMParkingspace);
            ACRoutingService.RegisterSelectionQuery(SelRuleID_ParkingSpace, (c, p) => c.Component.ValueT is PAMParkingspace, (c, p) => c.Component.ValueT is PAProcessModule);
            ACRoutingService.RegisterSelectionQuery(SelRuleID_ParkingSpace_Deselector, null, (c, p) => c.Component.ValueT is PAMParkingspace);
        }

        public PAMParkingspace(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
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

        [ACMethodInfo("", "en{'Refresh Facility'}de{'Aktualisiere Lagerplatz'}", 400, true)]
        public virtual void RefreshFacility()
        {
            int current = RefreshParkingSpace.ValueT;
            current++;
            if (current > 1000)
                current = 0;

            RefreshParkingSpace.ValueT = current;
        }

        [ACPropertyBindingSource]
        public IACContainerTNet<int> RefreshParkingSpace
        {
            get;
            set;
        }


        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMParkingspace(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessModuleVB(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

    }
}
