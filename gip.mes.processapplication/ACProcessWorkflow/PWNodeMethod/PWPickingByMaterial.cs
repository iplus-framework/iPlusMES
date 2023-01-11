using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Picking by material'}de{'Kommissionierung nach Material'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWPickingByMaterial : PWNodeProcessMethod
    {
        #region c'tors

        static PWPickingByMaterial()
        {
            var wrapper = CreateACMethodWrapper(typeof(PWPickingByMaterial));
            ACMethod.RegisterVirtualMethod(typeof(PWPickingByMaterial), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWPickingByMaterial), HandleExecuteACMethod_PWPickingByMaterial);
        }

        public PWPickingByMaterial(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
               base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #endregion

        private static bool HandleExecuteACMethod_PWPickingByMaterial(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PWBaseNodeProcess(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            base.SMStarting();
        }

        protected static ACMethodWrapper CreateACMethodWrapper(Type thisType)
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue("PickingType", typeof(string), "", Global.ParamOption.Optional));
            paramTranslation.Add("PickingType", "en{'Picking type'}de{'Picking type'}");

            method.ParameterValueList.Add(new ACValue("SourceFacilityNo", typeof(string), "", Global.ParamOption.Optional));
            paramTranslation.Add("SourceFacilityNo", "en{'Source facility No'}de{'Source facility No'}");

            method.ParameterValueList.Add(new ACValue("FromDT", typeof(DateTime), DateTime.MinValue, Global.ParamOption.Optional));
            paramTranslation.Add("FromDT", "en{'From date'}de{'From date'}");

            method.ParameterValueList.Add(new ACValue("ToDT", typeof(DateTime), DateTime.MinValue, Global.ParamOption.Optional));
            paramTranslation.Add("ToDT", "en{'To date'}de{'To date'}");

            return new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", thisType, paramTranslation, null);
        }
    }
}
