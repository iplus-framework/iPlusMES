using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using System.Xml;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Virtual material transport'}de{'Virtueller Materialabtransport'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWNodeDeliverMaterial : PWBaseNodeProcess, IPWNodeDeliverMaterial
    {
        public const string PWClassName = "PWNodeDeliverMaterial";

        #region Properties
        public Route CurrentDischargingRoute { get { return null; } set { } }

        public override bool MustBeInsidePWGroup
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Constructors

        static PWNodeDeliverMaterial()
        {
            ACMethod TMP;
            TMP = new ACMethod(ACStateConst.SMStarting);
            ACMethod.RegisterVirtualMethod(typeof(PWNodeDeliverMaterial), ACStateConst.SMStarting, TMP, "en{'SVirtual material transport'}de{'Virtueller Materialabtransport'}", null);
            RegisterExecuteHandler(typeof(PWNodeDeliverMaterial), HandleExecuteACMethod_PWNodeDeliverMaterial);
        }

        public PWNodeDeliverMaterial(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Public

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWNodeDeliverMaterial(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWBaseNodeProcess(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            RecalcTimeInfo();
            if (CreateNewProgramLog(NewACMethodWithConfiguration()) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                return;
            // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
            if (CurrentACState == ACStateEnum.SMStarting)
                CurrentACState = ACStateEnum.SMCompleted;
        }

#endregion

#region Protected


#endregion

    }
}
