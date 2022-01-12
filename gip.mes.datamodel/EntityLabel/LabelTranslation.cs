using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'LabelTranslation'}de{'LabelTranslation'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Name", "en{'Name'}de{'Name'}","", "", true)]
    [ACPropertyEntity(2, "Desc", "en{'Description'}de{'Beschreibung'}","", "", true)]
    [ACPropertyEntity(3, "VBLanguage", "en{'Language'}de{'Sprache'}", Const.ContextDatabase + "\\VBLanguage", "", true)]
    [ACPropertyEntity(4, "Label", "en{'Label'}de{'Label'}", Const.ContextDatabase + "\\Label", "", true)]
    public partial class LabelTranslation
    {

        #region New/Delete
        public static LabelTranslation NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            LabelTranslation labelTrans = new LabelTranslation();
            labelTrans.LabelTranslationID = Guid.NewGuid();
            if(parentACObject != null && parentACObject.GetType() == typeof(Label))
            {
                Label label = parentACObject as Label;
                labelTrans.Label = label;
            }
            labelTrans.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return labelTrans;
        }

        #endregion
    }
}
