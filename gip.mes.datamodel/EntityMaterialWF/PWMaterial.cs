using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'PWMaterial'}de{'PWMaterial'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.Optional, false, "PWMaterial", false)]
    public class PWMaterial : PWBaseInOut
    {
        public PWMaterial(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override void PWPointInCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
        }
    }

    [ACClassInfo(Const.PackName_VarioSystem, "en{'PWMaterialGroup'}de{'PWMaterialGroup'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.Optional, false, "PWMaterialGroup", false)]
    public class PWMaterialGroup : PWGroup
    {
        public PWMaterialGroup(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }
    }
}
