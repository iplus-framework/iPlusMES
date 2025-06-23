using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, ConstApp.PlanningMR, Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]

    public class MRPPlanningManager : PARole
    {
        #region const
        public const string C_DefaultServiceACIdentifier = "MRPPlanningManager";
        #endregion

        #region c´tors

        public MRPPlanningManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
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

        #region static Methods
        public static MRPPlanningManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<MRPPlanningManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<MRPPlanningManager> ACRefToServiceInstance(ACComponent requester)
        {
            MRPPlanningManager serviceInstance = GetServiceInstance(requester) as MRPPlanningManager;
            if (serviceInstance != null)
                return new ACRef<MRPPlanningManager>(serviceInstance, requester);
            return null;
        }
        #endregion
    }
}
