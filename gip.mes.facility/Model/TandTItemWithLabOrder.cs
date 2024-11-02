// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTItemWithLabOrder'}de{'TandTItemWithLabOrder'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class TandTItemWithLabOrder
    {
        [ACPropertyInfo(1, "Name", "en{'Name'}de{'Name'}")]
        public string Name { get; set; }

        [ACPropertyInfo(2, "TypeName", "en{'Typ'}de{'Typ'}")]
        public string TypeName { get; set; }

        [ACPropertyInfo(3, Const.EntityInsertDate, Const.EntityTransInsertDate)]
        public DateTime InsertDate { get; set; }

        public IACObjectEntity Related { get; set; }
    }
}
