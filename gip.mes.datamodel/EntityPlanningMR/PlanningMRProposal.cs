using System;
using System.ComponentModel.DataAnnotations.Schema;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Planning proposal'}de{'Planungsvorschlag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOPlanningMR")]

    [ACPropertyEntity(1, nameof(PlanningMR), ConstApp.PlanningMR, Const.ContextDatabase + "\\" + nameof(PlanningMR), "", true)]
    [ACPropertyEntity(2, nameof(InOrder), ConstApp.ProdOrder, Const.ContextDatabase + "\\" + nameof(InOrder), "", true)]
    [ACPropertyEntity(3, nameof(ProdOrder), ConstApp.ProdOrder, Const.ContextDatabase + "\\" + nameof(ProdOrder), "", true)]
    [ACPropertyEntity(4, nameof(ProdOrderPartslist), ConstApp.ProdOrderPartslist, Const.ContextDatabase + "\\" + ProdOrderPartslist.ClassName, "", true)]
    [ACPropertyEntity(5, nameof(IsPublished), "en{'Expected Posting'}de{'Erwartete Buchung'}", "", "", true)]

    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<PlanningMRProposal>) })]
    public partial class PlanningMRProposal
    {

        #region const
        public const string ClassName = "PlanningMRProposal";
        #endregion

        #region IACObjectEntity Members
        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "ProdOrder\\ProdOrderNo,ProdOrderPartslist\\Sequence,InOrder\\InOrderNo";
            }
        }
        #endregion

        #region New/Delete
        public static PlanningMRProposal NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            PlanningMRProposal entity = new PlanningMRProposal();
            entity.PlanningMRProposalID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject != null && parentACObject is PlanningMR)
                entity.PlanningMR = parentACObject as PlanningMR;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

    }
}
