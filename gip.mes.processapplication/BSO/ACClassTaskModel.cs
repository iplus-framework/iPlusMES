using gip.core.datamodel;
using System;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ACClassTaskModel'}de{'ACClassTaskModel'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable)]
    public class ACClassTaskModel
    {
        [ACPropertyInfo(7, "ShowCol", "en{'Order-No.'}de{'Auftrag-Nr.'}")]
        public string ProgramNo { get; set; }


        [ACPropertyInfo(8, "ShowCol", "en{'Batch-No.'}de{'Batch-Nr.'}")]
        public string BatchNo { get; set; }

        [ACPropertyInfo(9, "ShowCol", "en{'Material'}de{'Material'}")]
        public string Material { get; set; }

        [ACPropertyInfo(10, "ShowCol", "en{'Startet'}de{'Gestartet'}")]
        public DateTime InsertDate { get; set; }

        [ACPropertyInfo(12, "ShowCol", "en{'Program-No'}de{'Programm-Nr.'}")]
        public string ACProgramNo { get; set; }

        [ACPropertyInfo(11, "ShowCol", "en{'Id'}de{'Id'}")]
        public string ACIdentifier { get; set; }

        public Guid ACClassTaskID { get; set; }
    }
}
