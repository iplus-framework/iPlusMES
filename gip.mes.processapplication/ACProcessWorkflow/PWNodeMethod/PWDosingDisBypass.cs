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
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Bypassing Dosing if SWT'}de{'Bypass für Dosieren bei SWT'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWProcessFunction.PWClassName, true)]
    public class PWDosingDisBypass : PWNodeOr
    {
        #region c´tors

        public PWDosingDisBypass(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }
        #endregion

        #region Methods
        public override void ReinterpretGate()
        {
            if (!IsEnabledReinterpretGate())
                return;

            if (   PWPointIn.IsActiveAND
                || (PWPointIn.IsActiveOR && ((PWDosing) PWPointIn.ConnectionList.Where(c => c.ValueT is PWDosing).FirstOrDefault()?.ValueT).IsAutomaticContinousWeighing))
            {
                PWPointIn.ResetActiveStates();
                RaiseOutEvent();
            }
        }
        #endregion

    }
}
