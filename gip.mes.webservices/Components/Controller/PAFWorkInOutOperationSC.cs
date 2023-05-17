using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.webservices.Components
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scan-Controller for PAFWorkInOutOperation'}de{'Scan-Controller für PAFWorkInOutOperation'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PAFWorkInOutOperationSC : PAFWorkTaskScanBaseSC
    {
        public PAFWorkInOutOperationSC(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        private Type _WorkInOutOperationType = typeof(PAFWorkInOutOperation);


        public override void HandleBarcodeSequence(ACComponent component, BarcodeSequence sequence)
        {
            base.HandleBarcodeSequence(component, sequence);
        }

        protected override Type OnGetControlledType()
        {
            return _WorkInOutOperationType;
        }
    }
}
