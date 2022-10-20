using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public interface IACPickingManager :  IACComponent
    {
        MsgWithDetails ValidateStart(DatabaseApp dbApp, Database dbiPlus, Picking picking, List<IACConfigStore> configStores, PARole.ValidationBehaviour validationBehaviour);

        MsgWithDetails ValidateRoutes(DatabaseApp dbApp, Database dbiPlus, Picking picking, List<IACConfigStore> configStores, PARole.ValidationBehaviour validationBehaviour);

        MsgWithDetails CreateNewPicking(ACMethodBooking relocationBooking, gip.core.datamodel.ACClassMethod aCClassMethod, DatabaseApp dbApp, Database dbIPlus, bool setReadyToLoad, out Picking picking);
    }
}
