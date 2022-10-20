using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Data.Objects;
using gip.mes.facility;
using gip.core.processapplication;

namespace gip.mes.processapplication
{

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Question for emptying mode'}de{'Abfrage bei Leerfahrmodus'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWEmptyingModeQuestion : PWNodeDecisionFunc
    {
        public const string PWClassName = "PWEmptyingModeQuestion";

        #region c´tors
        static PWEmptyingModeQuestion()
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("ForceElseEvent", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("ForceElseEvent", "en{'Force ELSE-Event'}de{'Immer SONST-Ereignis auslösen'}");
            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWEmptyingModeQuestion), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWEmptyingModeQuestion), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWEmptyingModeQuestion), HandleExecuteACMethod_PWEmptyingModeQuestion);
        }

        public PWEmptyingModeQuestion(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

        #region Properties
        protected bool ForceElseEvent
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ForceElseEvent");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        #endregion

        #region Methods

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWEmptyingModeQuestion(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeDecisionFunc(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        #region ACState
        public override void Reset()
        {
            ClearMyConfiguration();
            base.Reset();
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            RecalcTimeInfo();
            if (CreateNewProgramLog(NewACMethodWithConfiguration()) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                return;

            bool emptyingMode =    ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                                || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode);

            if (ForceElseEvent || emptyingMode)
                RaiseElseEventAndComplete();
            else
                RaiseOutEventAndComplete();
        }
        #endregion

        #region User Interaction
        #endregion

        #endregion


    }
}
