using gip.core.autocomponent;
using gip.core.datamodel;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Manual Addition'}de{'Manuelle Zugabe'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, true, PWInfoACClass = PWManualAddition.PWClassName, BSOConfig = "BSOManualAddition", SortIndex = 200)]
    public class PAFManualAddition : PAFManualWeighing
    {
        //#region Constructors

        public new const string ClassName = "PAFManualAddition";

        public new const string VMethodName_ManualWeighing = "ManualAddition";

        static PAFManualAddition()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFManualAddition), ACStateConst.TMStart, CreateVirtualMethod(VMethodName_ManualWeighing, "en{'Manual addition'}de{'Manuelle Zugabe'}", typeof(PWManualAddition)));
            RegisterExecuteHandler(typeof(PAFManualAddition), HandleExecuteACMethod_PAFManualAddition);
        }

        public PAFManualAddition(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #region Methods
        public static bool HandleExecuteACMethod_PAFManualAddition(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAFManualWeighing(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        public override bool TareCheck => false;

        public override bool CheckInToleranceOnlyManuallyAddedQuantity => true;

        public override bool SimulateWeightIfSimulationOn => false;
        #endregion

    }
}
