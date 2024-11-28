using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Demands from orders'}de{'Bedarfe nach Aufträgen'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOTemplateSchedule")]

    [ACPropertyEntity(1, nameof(PlanningMR), ConstApp.PlanningMR, Const.ContextDatabase + "\\" + PlanningMR.ClassName, "", true)]
    [ACPropertyEntity(2, nameof(PlanningMRProposal), "en{'Planning proposal'}de{'Planungsvorschlag'}", Const.ContextDatabase + "\\" + PlanningMRProposal.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, nameof(Material), ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(4, nameof(OutOrderPos), "en{'Sales order line'}de{'Verkaufsposition'}", Const.ContextDatabase + "\\" + nameof(OutOrderPos) + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, nameof(ProdOrderPartslistPos), "en{'Production order line'}de{'Prodauftragsposition'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, nameof(InOrderPos), "en{'Purchase order line'}de{'Bestellposition'}", Const.ContextDatabase + "\\" + nameof(InOrderPos) + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(7, nameof(ProdOrderPartslist), ConstApp.ProdOrderPartslist, Const.ContextDatabase + "\\" + ProdOrderPartslist.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(8, nameof(StoreQuantityUOM), "en{'Estimated new Stock'}de{'Geschätzer neuer Bestand'}", "", "", true)]
    [ACPropertyEntity(9, nameof(ExpectedPostingDate), "en{'Expected Posting'}de{'Erwartete Buchung'}", "", "", true)]
    [ACPropertyEntity(494, Const.EntityDeleteDate, Const.EntityTransDeleteDate)]
    [ACPropertyEntity(495, Const.EntityDeleteName, Const.EntityTransDeleteName)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<PlanningMRPos>) })]
    public partial class PlanningMRPos
    {

        #region const
        #endregion

        #region IACObjectEntity Members
        static public string KeyACIdentifier
        {
            get
            {
                return "Material\\MaterialNo";
            }
        }
        #endregion

        #region New/Delete
        public static PlanningMRPos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            PlanningMRPos entity = new PlanningMRPos();
            entity.PlanningMRPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject != null && parentACObject is PlanningMR)
                entity.PlanningMR = parentACObject as PlanningMR;
            else if (parentACObject != null && parentACObject is PlanningMRProposal)
            {
                entity.PlanningMRProposal = parentACObject as PlanningMRProposal;
                entity.PlanningMR = entity.PlanningMRProposal.PlanningMR;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

    }
}
