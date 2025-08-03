using System;
using System.ComponentModel.DataAnnotations.Schema;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Demands from orders'}de{'Bedarfe nach Aufträgen'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOTemplateSchedule")]

    [ACPropertyEntity(1, nameof(PlanningMRCons), ConstApp.PlanningMR, Const.ContextDatabase + "\\" + nameof(PlanningMRCons), "", true)]
    [ACPropertyEntity(2, nameof(OutOrderPos), "en{'Sales order line'}de{'Verkaufsposition'}", Const.ContextDatabase + "\\" + nameof(OutOrderPos), "", true)]
    [ACPropertyEntity(3, nameof(ProdOrderPartslistPos), "en{'Production order line'}de{'Prodauftragsposition'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName, "", true)]
    [ACPropertyEntity(4, nameof(StoreQuantityUOM), "en{'Estimated new Stock'}de{'Geschätzer neuer Bestand'}", "", "", true)]
    [ACPropertyEntity(5, nameof(PlanningMRProposal), "en{'Planning proposal'}de{'Planungsvorschlag'}", Const.ContextDatabase + "\\" + PlanningMRProposal.ClassName, "", true)]
    [ACPropertyEntity(6, nameof(InOrderPos), "en{'Purchase order line'}de{'Bestellposition'}", Const.ContextDatabase + "\\" + nameof(InOrderPos), "", true)]
    [ACPropertyEntity(7, nameof(ProdOrderPartslist), ConstApp.ProdOrderPartslist, Const.ContextDatabase + "\\" + ProdOrderPartslist.ClassName, "", true)]
    [ACPropertyEntity(8, nameof(ExpectedBookingDate), "en{'Expected Posting'}de{'Erwartete Buchung'}", "", "", true)]

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
        [NotMapped]
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
            if (parentACObject != null && parentACObject is PlanningMRCons)
                entity.PlanningMRCons = parentACObject as PlanningMRCons;
            else if (parentACObject != null && parentACObject is PlanningMRProposal)
            {
                entity.PlanningMRProposal = parentACObject as PlanningMRProposal;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

    }
}
