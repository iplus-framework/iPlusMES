using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ACClassTaskModel'}de{'ACClassTaskModel'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable)]
    public class ACClassTaskModel
    {
        [ACPropertyInfo(1, "ShowCol", "en{'Order-No.'}de{'Auftrag-Nr.'}")]
        public string ProgramNo { get; set; }


        [ACPropertyInfo(2, "ShowCol", "en{'Batch-No.'}de{'Batch-Nr.'}")]
        public string BatchNo { get; set; }

        [ACPropertyInfo(3, "ShowCol", "en{'Material No.'}de{'Material-Nr.'}")]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(3, "ShowCol", "en{'Material name'}de{'Materialname'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(5, "ShowCol", "en{'Startet'}de{'Gestartet'}")]
        public DateTime InsertDate { get; set; }

        [ACPropertyInfo(6, "ShowCol", "en{'Program-No'}de{'Programm-Nr.'}")]
        public string ACProgramNo { get; set; }

        [ACPropertyInfo(7, "ShowCol", "en{'Id'}de{'Id'}")]
        public string ACIdentifier { get; set; }

        public Guid ACClassTaskID { get; set; }

        public List<string> ProcessModules { get; set; } = new List<string>();


        [ACPropertyInfo(8, "ProcesModulesStr", "en{'Process Modules'}de{'Prozessmodule'}")]
        public string ProcesModulesStr
        {
            get
            {
                return ProcessModules != null ? string.Join(", ", ProcessModules) : string.Empty;
            }
        }

        public List<MDSchedulingGroup> SchedulingGroups { get; set; } = new List<MDSchedulingGroup>();


        [ACPropertyInfo(10, "SchedulingGroupsStr", "en{'Scheduling Groups'}de{'Planungsgruppen'}")]
        public string SchedulingGroupsStr
        {
            get
            {
                return SchedulingGroups != null ? string.Join(", ", SchedulingGroups.Select(x => x.MDSchedulingGroupName)) : string.Empty;
            }
        }
    }
}
