using gip.core.datamodel;
using mesDB = gip.mes.datamodel;
using System.Collections.Generic;
using gip.mes.facility;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'RuleSelection'}de{'RuleSelection'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class RuleSelection
    {

        [ACPropertyInfo(100, "", Const.Workflow)]
        public MapPosToWFConn WF { get; set; }


        [ACPropertyInfo(101, "", Const.ProcessModule)]
        public List<ACClass> AvailableValues { get; set; } = new List<ACClass>();

        [ACPropertyInfo(102, "", Const.ProcessModule)]
        public List<ACClass> SelectedValues { get; internal set; } = new List<ACClass>();
    }
}
