// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Vacuum mode'}de{'Vakuummodus'}", Global.ACKinds.TACEnum)]
    public enum PAFVacuumMode : ushort
    {
        Evacuate = 0,
        ReleaseVacuum = 1,
        Inflate = 2,
        BreakPressure = 3
    }

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Vacuum'}de{'Vakuum'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWVacuum.PWClassName, true)]
    public class PAFVacuum : PAProcessFunction
    {
       
        #region Constructors

        static PAFVacuum()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFVacuum), ACStateConst.TMStart, CreateVirtualMethod("Vacuum", "en{'Vacuum'}de{'Vakuum'}", typeof(PWVacuum)));
            ACMethod.RegisterVirtualMethod(typeof(PAFVacuum), ACStateConst.TMStart, CreateVirtualTimeMethod("VacuumTime", "en{'Vacuum on time'}de{'Vakuum auf Zeit'}", typeof(PWVacuum)));
            RegisterExecuteHandler(typeof(PAFVacuum), HandleExecuteACMethod_PAFVacuum);
        }

        public PAFVacuum(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion 

        #region Public 

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAFVacuum(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        #endregion

        #region Private

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("Mode", typeof(PAFVacuumMode), PAFVacuumMode.Evacuate, Global.ParamOption.Required));
            paramTranslation.Add("Mode", "en{'Mode'}de{'Modus'}");
            method.ParameterValueList.Add(new ACValue("Pressure", typeof(Double), 0.0, Global.ParamOption.Required));
            paramTranslation.Add("Pressure", "en{'Pressure'}de{'Druck'}");
            method.ParameterValueList.Add(new ACValue("HoldPressure", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("HoldPressure", "en{'Hold pressure'}de{'Druck halten'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActPressure", typeof(Double), 0, Global.ParamOption.Optional));
            resultTranslation.Add("ActPressure", "en{'Act.Pressure'}de{'Ist-Druck'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        protected static ACMethodWrapper CreateVirtualTimeMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("Mode", typeof(PAFVacuumMode), PAFVacuumMode.Evacuate, Global.ParamOption.Required));
            paramTranslation.Add("Mode", "en{'Mode'}de{'Modus'}");
            method.ParameterValueList.Add(new ACValue("Pressure", typeof(Double), 0.0, Global.ParamOption.Required));
            paramTranslation.Add("Pressure", "en{'Pressure'}de{'Druck'}");
            method.ParameterValueList.Add(new ACValue("HoldPressure", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("HoldPressure", "en{'Hold pressure'}de{'Druck halten'}");
            method.ParameterValueList.Add(new ACValue("Duration", typeof(TimeSpan), 0, Global.ParamOption.Required));
            paramTranslation.Add("Duration", "en{'Duration'}de{'Dauer'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActPressure", typeof(Double), 0, Global.ParamOption.Optional));
            resultTranslation.Add("ActPressure", "en{'Act.Pressure'}de{'Ist-Druck'}");
            method.ResultValueList.Add(new ACValue("ActDuration", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            resultTranslation.Add("ActDuration", "en{'Duration'}de{'Dauer'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }


        #endregion

    }

}
