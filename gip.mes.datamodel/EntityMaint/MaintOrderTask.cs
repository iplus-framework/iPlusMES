using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance Task'}de{'Wartungsaufgabe'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, "MaintOrder", "en{'MaintOrder'}de{'MaintOrder'}", Const.ContextDatabase + "\\" + MaintOrder.ClassName, "", true)]
    [ACPropertyEntity(2, "MDMaintTaskState", "en{'Status of Work Task'}de{'Aufgabenstatus'}", Const.ContextDatabase + "\\" + MDMaintTaskState.ClassName, "", true)]
    [ACPropertyEntity(3, "TaskDescription", "en{'Task description'}de{'Task description'}", "", "", true)]
    [ACPropertyEntity(4, "TaskName", "en{'Task name'}de{'Task name'}", "", "", true)]
    [ACPropertyEntity(5, "PlannedStartDate", "en{'Planned start date'}de{'Planned start date'}", "", "", true)]
    [ACPropertyEntity(6, "PlannedDuration", "en{'PlannedDuration'}de{'PlannedDuration'}", "", "", true)]
    [ACPropertyEntity(7, "StartDate", "en{'Task commenced on'}de{'Aufgabe begonnen am'}", "", "", true)]
    [ACPropertyEntity(8, "EndDate", "en{'Task completed on'}de{'Aufgabe beendet am'}", "", "", true)]
    [ACPropertyEntity(9, "Comment", "en{'Comment'}de{'Kommentar'}", "", "", true)]
    [ACPropertyEntity(10, "IsRepair", "en{'Is repaired'}de{'Repariert'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MaintOrderTask.ClassName, "en{'Maintenance Task'}de{'Wartungsaufgabe'}", typeof(MaintOrderTask), MaintOrderTask.ClassName, Const.EntityInsertDate, Const.EntityInsertDate)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaintOrderTask>) })]
    public partial class MaintOrderTask
    {
        public const string ClassName = nameof(MaintOrderTask);

        public static MaintOrderTask NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MaintOrderTask entity = new MaintOrderTask();
            entity.MaintOrderTaskID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDMaintTaskState = dbApp.MDMaintTaskState.FirstOrDefault(c => c.MDMaintTaskStateIndex == (short)MaintTaskState.UnfinishedTask);

            if (parentACObject is MaintOrder)
                entity.MaintOrder = parentACObject as MaintOrder;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        [ACPropertyInfo(9999, "", "en{'Planned duration'}de{'Geplante Dauer'}")]
        public TimeSpan PlannedDurationTS
        {
            get
            {
                if (PlannedDuration.HasValue)
                    return TimeSpan.FromMinutes(PlannedDuration.Value);
                return TimeSpan.Zero;
        }
            set
            {
                PlannedDuration = (int)value.TotalMinutes;
                OnPropertyChanged(nameof(PlannedDurationTS));
            }
        }

        [ACPropertyInfo(999, "", "en{'Duration'}de{'Dauer'}")]
        public TimeSpan MaintOrderTaskDuration
        {
            get
            {
                if (StartDate.HasValue && EndDate.HasValue && EndDate.Value > StartDate.Value)
                {
                    return EndDate.Value - StartDate.Value;
                }
                return TimeSpan.Zero;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Task name'}de{'Task name'}")]
        public string MaintTaskName
        {
            get
            {
                if (!string.IsNullOrEmpty(TaskName))
                    return TaskName;

                if (MaintOrder != null)
                {
                    var tasks = MaintOrder.MaintOrderTask_MaintOrder.OrderBy(c => c.InsertDate).ToList();
                    if (tasks != null)
                        return (tasks.IndexOf(this) + 1).ToString();
                }

                return "001";
            }
        }

        protected override void OnPropertyChanged(string property)
        {
            base.OnPropertyChanged(property);

            if (property == nameof(TaskName))
            {
                OnPropertyChanged(nameof(MaintTaskName));
            }
        }

        public void CopyTaskValues(MaintOrderTask newTask)
        {
            newTask.TaskDescription = TaskDescription;
            newTask.TaskName = TaskName;
            newTask.PlannedStartDate = PlannedStartDate;
            newTask.PlannedDuration = PlannedDuration;
        }

    }

    public enum MaintTaskState : short
    {
        UnfinishedTask = 1,
        TaskInProcess = 2,
        TaskCompleted = 3
    }
}
