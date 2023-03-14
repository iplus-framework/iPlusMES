using gip.core.datamodel;
using mesDB = gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'RuleGroup'}de{'RuleGroup'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class RuleGroup
    {

        public RuleGroup()
        {
            RuleSelections = new List<RuleSelection>();
        }

        [ACPropertyInfo(100, "", Const.ACGroup)]
        public ACClass RefPAACClass { get; set; }


        [ACPropertyInfo(101, "", Const.ACGroup)]
        public List<RuleSelection> RuleSelections { get; set; }

    }
}
