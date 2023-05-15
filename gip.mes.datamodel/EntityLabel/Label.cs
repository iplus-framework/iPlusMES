using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Label'}de{'Label'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Name", "en{'Name'}de{'Name'}","", "", true)]
    [ACPropertyEntity(2, "Desc", "en{'Description'}de{'Beschreibung'}","", "", true)]
    [NotMapped]
    public partial class Label
    {

        #region New/Delete
        public static Label NewACObject(DatabaseApp dbApp, IACObject parentACObject = null)
        {
            Label label = new Label();
            label.LabelID = Guid.NewGuid();
            if(parentACObject != null)
            {
                if(parentACObject is Material)
                {
                    label.Name = ((Material)parentACObject).MaterialName1;
                }
            }
            label.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return label;
        }

        #endregion
    }
}
