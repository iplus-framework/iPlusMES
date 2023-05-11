
using gip.core.datamodel;
using System.ComponentModel.DataAnnotations.Schema;
/*
TandTv3_Step
StepID
JobID
StepNo
StepName
*/
namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Step'}de{'Step'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, TandTv3FilterTracking.ClassName, "en{'Filter'}de{'Filter'}", Const.ContextDatabase + "\\" + TandTv3FilterTracking.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, "StepNo", "en{'StepNo'}de{'StepNo'}", "", "", true)]
    [ACPropertyEntity(3, "StepName", "en{'Name'}de{'Name'}", "", "", true)]
    public partial class TandTv3Step
    {
        [NotMapped]
        public const string ClassName = "TandTv3Step";

        #region overriden methods

        public override string ToString()
        {
            return string.Format(@"#{0}", StepNo);
        }
        #endregion
    }
}
