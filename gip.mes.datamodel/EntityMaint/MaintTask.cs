using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance Task'}de{'Wartungsaufgabe'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, "MaintACClassVBGroup", "en{'MaintACClassVBGroup'}de{'MaintACClassVBGroup'}", Const.ContextDatabase + "\\" + MaintACClassVBGroup.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, "MDMaintTaskState", "en{'Status of Work Task'}de{'Aufgabenstatus'}", Const.ContextDatabase + "\\" + MDMaintTaskState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, "StartTaskDate", "en{'Task commenced on'}de{'Aufgabe begonnen am'}", "", "", true)]
    [ACPropertyEntity(4, "EndTaskDate", "en{'Task completed on'}de{'Aufgabe beendet am'}", "", "", true)]
    [ACPropertyEntity(5, "Comment", "en{'Comment'}de{'Kommentar'}", "", "", true)]
    [ACPropertyEntity(6, "IsRepair", "en{'Is repaired'}de{'Repariert'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MaintTask.ClassName, "en{'Maintenance Task'}de{'Wartungsaufgabe'}", typeof(MaintTask), MaintTask.ClassName, Const.EntityInsertDate, Const.EntityInsertDate)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaintTask>) })]
    [NotMapped]
    public partial class MaintTask
    {
        [NotMapped]
        public const string ClassName = "MaintTask";

        public static MaintTask NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MaintTask entity = new MaintTask();
            entity.MaintTaskID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is MaintOrder)
                entity.MaintOrder = parentACObject as MaintOrder;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }
    }

    public enum MaintTaskState : short
    {
        UnfinishedTask = 1,
        TaskInProcess = 2,
        TaskCompleted = 3
    }
}
