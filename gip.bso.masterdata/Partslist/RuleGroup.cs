using gip.core.datamodel;
using System.Collections.Generic;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'RuleGroup'}de{'RuleGroup'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class RuleGroup
    {

        [ACPropertyInfo(100, "", Const.ACGroup)]
        public ACClass RefPAACClass { get; set; }


        [ACPropertyInfo(101, "", Const.ACGroup)]
        public List<RuleSelection> RuleSelections { get; set; } = new List<RuleSelection>();

    }
}
