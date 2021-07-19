using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Work center child'}de{'Work center child'}", Global.ACKinds.TACAbstractClass, Global.ACStorableTypes.NotStorable, true, true)]
    public abstract class BSOWorkCenterChild : ACBSOvb
    {
        public BSOWorkCenterChild(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public WorkCenterItemFunction ItemFunction
        {
            get;
            set;
        }

        [ACPropertyInfo(510)]
        public BSOWorkCenterSelector ParentBSOWCS
        {
            get
            {
                return ParentACComponent as BSOWorkCenterSelector;
            }
        }

        public virtual void OnHandleOrderInfoChanged(string orderInfo, string pwGroupACUrl, ACComponent pwGroup)
        {

        }

        public abstract void Activate(ACComponent selectedProcessModule);

        public abstract void DeActivate();
    }
}
