using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.Material, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "")]
    public partial class OperationLog 
    {
        public static OperationLog NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            OperationLog entity = new OperationLog();
            entity.OperationLogID = Guid.NewGuid();

            ACClass refACClass = parentACObject as ACClass;
            if (refACClass != null)
                entity.RefACClassID = refACClass.ACClassID;

            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);

            return entity;
        }

    }
}
