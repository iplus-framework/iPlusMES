using gip.core.datamodel;
using mesDB = gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'RuleSelection'}de{'RuleSelection'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class RuleSelection
    {

        [ACPropertyInfo(100, "", Const.Workflow)]
        public ACClassWF WF { get; set; }


        [ACPropertyInfo(101, "", Const.ProcessModule)]
        public List<ACClass> ProcessModules { get; set; }

    }
}
