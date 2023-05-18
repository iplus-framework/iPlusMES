using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'InOut operation work'}de{'InOut operation work'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWWorkInOutOperation : PWWorkTaskGeneric
    {
        public PWWorkInOutOperation(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
    }
}
