using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Picking by material'}de{'Kommissionierung nach Material'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, true, PWInfoACClass = nameof(PWPickingByMaterial), BSOConfig = "BSOPickingByMaterial", SortIndex = 600)]
    public class PAFPickingByMaterial : PAProcessFunction
    {
        static PAFPickingByMaterial()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFPickingByMaterial), ACStateConst.TMStart, CreateVirtualMethod("PickingByMat", "en{'Picking by material'}de{'Kommissionierung nach Material'}", typeof(PWPickingByMaterial)));
            RegisterExecuteHandler(typeof(PAFPickingByMaterial), HandleExecuteACMethod_PAFPickingByMaterial);
        }

        public PAFPickingByMaterial(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") 
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        //protected override bool PWWorkTaskScanDeSelector(IACComponent c)
        //{
        //    return c is PWPickingByMaterial;
        //}

        //protected override bool PWWorkTaskScanSelector(IACComponent c)
        //{
        //    return c is PWPickingByMaterial;
        //}

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        public override void SMStarting()
        {
            base.SMStarting();
        }

        [ACMethodInfo("","",9999)]
        public string GetCurrentTaskACUrl()
        {
            if (CurrentTask != null && CurrentTask.ValueT != null)
                return CurrentTask.ValueT.ACUrl;
            return null;
        }

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        public static bool HandleExecuteACMethod_PAFPickingByMaterial(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
    }
}
