// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Work task on hold'}de{'Warteschleife'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, nameof(PWWorkTaskOnHold), true, BSOConfig = "BSOWorkTaskOnHold")]
    public class PAFWorkTaskOnHold : PAFWorkTaskScanBase
    {
        static PAFWorkTaskOnHold()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFWorkTaskOnHold), ACStateConst.TMStart, CreateVirtualMethod("Work", "en{'Work task on hold'}de{'Arbeitsaufgabe in der Warteschleife'}", typeof(PWWorkTaskOnHold)));
            RegisterExecuteHandler(typeof(PAFWorkTaskOnHold), HandleExecuteACMethod_PAFWorkTaskOnHold);
        }

        public PAFWorkTaskOnHold(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        private DateTime? _NextCheckACState;


        protected override bool PWWorkTaskScanDeSelector(IACComponent c)
        {
            return c is PWWorkTaskOnHold;
        }

        protected override bool PWWorkTaskScanSelector(IACComponent c)
        {
            return c is PWWorkTaskOnHold;
        }

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        public override void SMStarting()
        {
            base.SMStarting();
        }

        public override void SMIdle()
        {
            DateTime? nextCheck = null;

            using(ACMonitor.Lock(_20015_LockValue))
                nextCheck = _NextCheckACState;

            if (nextCheck.HasValue && nextCheck.Value > DateTime.Now)
                HandleACStateOfWorkTaskOnHold();
        }

        public override void SMRunning()
        {
            DateTime? nextCheck = null;

            using (ACMonitor.Lock(_20015_LockValue))
                nextCheck = _NextCheckACState;

            if (nextCheck.HasValue && nextCheck.Value > DateTime.Now)
                HandleACStateOfWorkTaskOnHold();
        }

        protected override void HandleACStateOfWorkTask()
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _NextCheckACState = DateTime.Now.AddSeconds(5);
            }

            SubscribeToProjectWorkCycle();
        }

        private void HandleACStateOfWorkTaskOnHold()
        {
            UnSubscribeToProjectWorkCycle();

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _NextCheckACState = null;
            }

            var workTaskResult = GetOrderInfos();
            if (workTaskResult != null && workTaskResult.OrderInfos != null && workTaskResult.OrderInfos.Any())
            {
                if (CurrentACState != ACStateEnum.SMRunning)
                    CurrentACState = ACStateEnum.SMRunning;
                return;
            }

            if (CurrentACState != ACStateEnum.SMIdle)
                CurrentACState = ACStateEnum.SMIdle;
        }

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        public static bool HandleExecuteACMethod_PAFWorkTaskOnHold(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAFWorkTaskScanBase(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
    }
}
