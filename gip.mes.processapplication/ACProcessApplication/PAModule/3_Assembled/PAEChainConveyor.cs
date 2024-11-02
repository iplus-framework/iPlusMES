// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Chain-Conveyor
    /// Kettenförderer, Redler
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Chain-conveyor'}de{'Kettenförderer/Redler'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAEChainConveyor : PAETransport
    {
        #region c'tors

        static PAEChainConveyor()
        {
            RegisterExecuteHandler(typeof(PAEChainConveyor), HandleExecuteACMethod_PAEChainConveyor);
        }

        public PAEChainConveyor(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
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

        #region Optional Members
        public IEnumerable<PAEMaterialSensor> MaterialSensors
        {
            get
            {
                List<PAEMaterialSensor> listResult = new List<PAEMaterialSensor>();
                if (!this.ACComponentChilds.Any())
                    return listResult;
                var query = this.ACComponentChilds.Where(c => typeof(PAEMaterialSensor).IsAssignableFrom(c.GetType())).Select(c => c as PAEMaterialSensor);
                if (!query.Any())
                    return listResult;
                foreach (PAEMaterialSensor member in query)
                {
                    listResult.Add(member);
                }
                return listResult;
            }
        }
        #endregion

        #region Handle execute helpers

        public static bool HandleExecuteACMethod_PAEChainConveyor(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAETransport(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
