// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.processapplication;
using gip.mes.datamodel;

namespace gip.mes.webservices
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scan-Controller for PAFDischarging'}de{'Scan-Controller für PAFDischarging'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PAFDischargingSC : PAScannedCompContrBase
    {
        #region c'tors
        new public const string ClassName = "PAFDischargingSC";

        public PAFDischargingSC(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion


        #region Properties
        protected override Type OnGetControlledType()
        {
            return typeof(PAFDischarging);
        }
        #endregion 


        #region Methods
        public override void HandleBarcodeSequence(ACComponent component, BarcodeSequence sequence)
        {
            sequence.Message = new Msg(eMsgLevel.Info, "OK");
            sequence.State = BarcodeSequence.ActionState.Completed;
        }
        #endregion
    }

}
