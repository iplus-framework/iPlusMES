using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Work center selector BSO base'}de{'Work center selector BSO base'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, "", true)]
    public abstract class PAFWorkCenterSelItemBase : PAProcessFunction
    {
        public PAFWorkCenterSelItemBase(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        [ACPropertyBindingSource]
        public IACContainerTNet<bool> NeedWork
        {
            get;
            set;
        }
    }
}
