﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance Task'}de{'Wartungsaufgabe'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, "MaintOrder", "en{'MaintOrder'}de{'MaintOrder'}", Const.ContextDatabase + "\\" + MaintOrder.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, "MDMaintTaskState", "en{'Status of Work Task'}de{'Aufgabenstatus'}", Const.ContextDatabase + "\\" + MDMaintTaskState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, "Task description", "en{'Task description'}de{'Task description'}", "", "", true)]
    [ACPropertyEntity(3, "PlannedStartDate", "en{'Planned start date'}de{'Planned start date'}", "", "", true)]
    [ACPropertyEntity(4, "PlannedDuration", "en{'PlannedDuration'}de{'PlannedDuration'}", "", "", true)]
    [ACPropertyEntity(3, "StartDate", "en{'Task commenced on'}de{'Aufgabe begonnen am'}", "", "", true)]
    [ACPropertyEntity(4, "EndDate", "en{'Task completed on'}de{'Aufgabe beendet am'}", "", "", true)]
    [ACPropertyEntity(5, "Comment", "en{'Comment'}de{'Kommentar'}", "", "", true)]
    [ACPropertyEntity(6, "IsRepair", "en{'Is repaired'}de{'Repariert'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MaintOrderTask.ClassName, "en{'Maintenance Task'}de{'Wartungsaufgabe'}", typeof(MaintOrderTask), MaintOrderTask.ClassName, Const.EntityInsertDate, Const.EntityInsertDate)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaintOrderTask>) })]
    public partial class MaintOrderTask

    {
        [NotMapped]
        public const string ClassName = nameof(MaintOrderTask);

        public static MaintOrderTask NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MaintOrderTask entity = new MaintOrderTask();
            entity.MaintOrderTaskID = Guid.NewGuid();
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