using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'MaintOrderPos'}de{'MaintOrderPos'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, "MaintOrder", "en{'MaintOrder'}de{'MaintOrder'}", Const.ContextDatabase + "\\" + MaintOrder.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, "ParentMaintOrderPos", "en{'MaintOrderPos parent'}de{'MaintOrderPos parent'}", Const.ContextDatabase + "\\" + MaintOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, "Material", "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, "Quantity", "en{'Quantity'}de{'Menge'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MaintOrderPos.ClassName, "en{'Maintenance order position'}de{'Position des Wartungsauftrags'}", typeof(MaintOrderPos), MaintOrderPos.ClassName, Const.EntityInsertDate, Const.EntityInsertDate)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaintOrderPos>) })]
    public partial class MaintOrderPos
    {
        public const string ClassName = nameof(MaintOrderPos);


    }
}
