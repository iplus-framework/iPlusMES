using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'MaintACClassVBGroup'}de{'MaintACClassVBGroup'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, "Comment", "en{'Tasks'}de{'Aufgaben'}")]
    [ACPropertyEntity(2, core.datamodel.VBGroup.ClassName, "en{'Group'}de{'Gruppe'}", Const.ContextDatabaseIPlus + "\\" + core.datamodel.VBGroup.ClassName, "", true)]
    [ACPropertyEntity(5, MaintACClass.ClassName, "en{'Maintenance rule'}de{'Wartungsregel'}", Const.ContextDatabase + "\\" + MaintACClass.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, MaintACClassProperty.ClassName, "en{'Maintenance property'}de{'Wartungseigenschaft'}", Const.ContextDatabase + "\\" + MaintACClassProperty.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MaintACClassVBGroup.ClassName, "en{'MaintACClassVBGroup'}de{'MaintACClassVBGroup'}", typeof(MaintACClassVBGroup), MaintACClassVBGroup.ClassName, "", "")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaintACClassVBGroup>) })]
    [NotMapped]
    public partial class MaintACClassVBGroup
    {
        [NotMapped]
        public const string ClassName = "MaintACClassVBGroup";

        #region New

        public static MaintACClassVBGroup NewACObject(DatabaseApp dbApp, IACObject parent)
        {
            var entity = new MaintACClassVBGroup();
            entity.MaintACClassVBGroupID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        [ACPropertyInfo(999)]
        [NotMapped]
        public core.datamodel.VBGroup VBGroupTask
        {
            get
            {
                return VBGroup.FromIPlusContext<core.datamodel.VBGroup>();
            }
        }


        #endregion
    }
}
