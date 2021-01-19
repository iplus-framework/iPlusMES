using gip.core.autocomponent;
using gip.core.datamodel;
using System;
/*
TandT_StepItemRelation
StepItemRelationID
RelationTypeID
SourceStepItemID
TargetStepItemID
*/
namespace gip.mes.datamodel
{

    [ACPropertyEntity(1, TandTv2Step.ClassName, "en{'Step'}de{'Step'}", Const.ContextDatabase + "\\" + TandTv2Step.ClassName, "", true)]
    [ACPropertyEntity(2, TandTv2RelationType.ItemName, "en{'RelationType'}de{'RelationType'}", Const.ContextDatabase + "\\" + TandTv2RelationType.ClassName, "", true)]
    [ACPropertyEntity(3, "SourceStepItem", "en{'SourceStepItem'}de{'SourceStepItem'}", Const.ContextDatabase + "\\" + TandTv2StepItem.ClassName, "", true)]
    [ACPropertyEntity(4, "TargetStepItem", "en{'TargetStepItem'}de{'TargetStepItem'}", Const.ContextDatabase + "\\" + TandTv2StepItem.ClassName, "", true)]

    public partial class TandTv2StepItemRelation
    {
        public const string ClassName = "TandTv2StepItemRelation";
        public const string ItemName = "TandTv2StepItemRelation";


        #region Lookup enum properties
        public TandTv2RelationTypeEnum TandTv2RelationTypeEnum
        {
            get
            {
                return (TandTv2RelationTypeEnum)Enum.Parse(typeof(TandTv2RelationTypeEnum), TandTv2RelationTypeID);
            }
            set
            {
                TandTv2RelationTypeID = Enum.GetName(typeof(TandTv2RelationTypeEnum), value);
            }
        }
        #endregion

    }
}
