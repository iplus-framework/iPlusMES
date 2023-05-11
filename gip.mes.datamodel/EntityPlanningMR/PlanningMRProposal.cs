using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'PlanningMRProposal'}de{'PlanningMRProposal'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOPlanningMR")]

    [ACPropertyEntity(1, PlanningMR.ClassName, ConstApp.PlanningMR, Const.ContextDatabase + "\\" + PlanningMR.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, ProdOrder.ClassName, ConstApp.ProdOrder, Const.ContextDatabase + "\\" + ProdOrder.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, ProdOrderPartslist.ClassName, ConstApp.ProdOrderPartslist, Const.ContextDatabase + "\\" + ProdOrderPartslist.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, InOrder.ClassName, ConstApp.ProdOrderPartslist, Const.ContextDatabase + "\\" + InOrder.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]

    [ACPropertyEntity(494, Const.EntityDeleteDate, Const.EntityTransDeleteDate)]
    [ACPropertyEntity(495, Const.EntityDeleteName, Const.EntityTransDeleteName)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<PlanningMRProposal>) })]
    public partial class PlanningMRProposal
    {

        #region const
        [NotMapped]
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
