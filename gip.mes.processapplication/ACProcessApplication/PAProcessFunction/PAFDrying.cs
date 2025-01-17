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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Drying'}de{'Trocknen'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWDrying.PWClassName, true)]
    public class PAFDrying : PAProcessFunction, IPAFSwitchable
    {
       
        #region Constructors

        static PAFDrying()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFDrying), ACStateConst.TMStart, CreateVirtualMethod("Drying", "en{'Drying'}de{'Trocknen'}", typeof(PWDrying)));
            RegisterExecuteHandler(typeof(PAFDrying), HandleExecuteACMethod_PAFDrying);
        }

        public PAFDrying(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #endregion 

        #region Public 

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAFDrying(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
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
            method.ParameterValueList.Add(new ACValue("SwitchOff", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("SwitchOff", "en{'Switch off'}de{'Ausschalten'}");
            method.ParameterValueList.Add(new ACValue("LeaveOn", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("LeaveOn", "en{'Drying switched on'}de{'Trocknung eingeschaltet lassen'}");
            method.ParameterValueList.Add(new ACValue("MinWeight", typeof(Double), 0, Global.ParamOption.Optional));
            paramTranslation.Add("MinWeight", "en{'Minimum weight'}de{'Mindestgewicht'}");
            method.ParameterValueList.Add(new ACValue("Temperature", typeof(Double), 0.0, Global.ParamOption.Required));
            paramTranslation.Add("Temperature", "en{'Temperature'}de{'Temperatur'}");
            method.ParameterValueList.Add(new ACValue("Duration", typeof(TimeSpan), 0, Global.ParamOption.Required));
            paramTranslation.Add("Duration", "en{'Duration'}de{'Dauer'}");
            method.ParameterValueList.Add(new ACValue("Pressure", typeof(Double), 0.0, Global.ParamOption.Required));
            paramTranslation.Add("Pressure", "en{'Pressure'}de{'Druck'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActDuration", typeof(TimeSpan), 0, Global.ParamOption.Required));
            resultTranslation.Add("ActDuration", "en{'Duration'}de{'Dauer'}");
            method.ResultValueList.Add(new ACValue("ActTemperature", typeof(Double), 0.0, Global.ParamOption.Required));
            resultTranslation.Add("ActTemperature", "en{'Temperature'}de{'Temperatur'}");
            method.ResultValueList.Add(new ACValue("WeightStart", typeof(Double), 0, Global.ParamOption.Optional));
            resultTranslation.Add("WeightStart", "en{'Weight on Start'}de{'Gewicht bei Start'}");
            method.ResultValueList.Add(new ACValue("WeightEnd", typeof(Double), 0, Global.ParamOption.Optional));
            resultTranslation.Add("WeightEnd", "en{'Weight on End'}de{'Gewicht am Ende'}");
            method.ResultValueList.Add(new ACValue("WeightDiff", typeof(Double), 0, Global.ParamOption.Optional));
            resultTranslation.Add("WeightDiff", "en{'Loss humidity'}de{'Feuchtigkeitsverlust'}");
            method.ResultValueList.Add(new ACValue("Rate", typeof(Double), 0, Global.ParamOption.Optional));
            resultTranslation.Add("Rate", "en{'Drying rate [kg/min]'}de{'Trockungsrate [kg/min]'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass);
        }

        #endregion

    }

}
