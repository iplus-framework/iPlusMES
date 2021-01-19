
using gip.core.datamodel;
/*
TandT_Step
	StepID
	JobID
	StepNo
	StepName
*/
namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Step'}de{'Step'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, TandTv2Job.ItemName, "en{'Job'}de{'Job'}", Const.ContextDatabase + "\\" + TandTv2Job.ClassName, "", true)]
    [ACPropertyEntity(2, "StepNo", "en{'StepNo'}de{'StepNo'}", "", "", true)]
    [ACPropertyEntity(3, "StepName", "en{'Name'}de{'Name'}", "", "", true)]
    public partial class TandTv2Step
    {
        public const string ClassName = "TandTv2Step";
        public const string ItemName = "TandTv2Step";

        #region overriden methods

        public override string ToString()
        {
            return string.Format(@"#{0}", StepNo);
        }
        #endregion
    }
}
