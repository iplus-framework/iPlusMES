using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Work task on hold'}de{'Arbeitsaufgabe in der Warteschleife'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWWorkTaskOnHold : PWWorkTaskScanBase
    {
        static PWWorkTaskOnHold()
        {
            var wrapper = CreateACMethodWrapper(typeof(PWWorkTaskOnHold));
            ACMethod.RegisterVirtualMethod(typeof(PWWorkTaskOnHold), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWWorkTaskOnHold), HandleExecuteACMethod_PWWorkTaskOnHold);
        }

        public PWWorkTaskOnHold(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        protected static ACMethodWrapper CreateACMethodWrapper(Type thisType)
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue(nameof(PWWorkTaskOnHold.AutoComplete), typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(PWWorkTaskOnHold.AutoComplete), "en{'Auto complete on scan'}de{'Beende automatisch bei Scan'}");

            return new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", thisType, paramTranslation, null);
        }


        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            //result = null;
            //switch (acMethodName)
            //{
            //}
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PWWorkTaskOnHold(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWWorkTaskScanBase(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }


        #region Properties

        public bool AutoComplete
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue(nameof(AutoComplete));
                    if (acValue != null)
                        return acValue.ParamAsBoolean;
                }
                return false;
            }
        }

        #endregion

        #region Methods

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            base.SMStarting();
        }

        public override void SMRunning()
        {
            base.SMRunning();
            if (AutoComplete)
            {
                ResetAndComplete();
            }
        }

        public override void Reset()
        {
            base.Reset();
            ClearMyConfiguration();
        }

        public override void SMIdle()
        {
            base.SMIdle();
            ClearMyConfiguration();
        }
        #endregion


    }
}
