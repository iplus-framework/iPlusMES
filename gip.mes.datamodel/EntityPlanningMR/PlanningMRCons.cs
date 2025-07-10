using System;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Demands from store stats'}de{'Bedarfe nach Lagerstatistik'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOTemplateSchedule")]

    [ACPropertyEntity(1, nameof(PlanningMR), ConstApp.PlanningMR, Const.ContextDatabase + "\\" + PlanningMR.ClassName, "", true)]
    [ACPropertyEntity(2, nameof(Material), ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(3, nameof(ConsumptionDate), "en{'Outward at'}de{'Abgang am'}", "", "", true)]
    [ACPropertyEntity(4, nameof(EstimatedQuantityUOM), "en{'Estimated Demand'}de{'Geschätzer Bedarf'}", "", "", true)]
    [ACPropertyEntity(5, nameof(ReqCorrectionQuantityUOM), "en{'Correction'}de{'Korrektur'}", "", "", true)]
    [ACPropertyEntity(6, nameof(RequiredQuantityUOM), "en{'Required Demand'}de{'Erforderlicher Bedarf'}", "", "", true)]
    [ACPropertyEntity(7, nameof(DefaultPartslist), "en{'Bill of Material'}de{'Stückliste'}", Const.ContextDatabase + "\\" + nameof(Partslist), "", true)]

    [ACPropertyEntity(494, Const.EntityDeleteDate, Const.EntityTransDeleteDate)]
    [ACPropertyEntity(495, Const.EntityDeleteName, Const.EntityTransDeleteName)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<PlanningMRCons>) })]
    public partial class PlanningMRCons
    {
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
        public static PlanningMRCons NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            PlanningMRCons entity = new PlanningMRCons();
            entity.PlanningMRConsID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject != null && parentACObject is PlanningMR)
                entity.PlanningMR = parentACObject as PlanningMR;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region override

        public override string ToString()
        {
            return $"{Material?.MaterialNo} - {ConsumptionDate.ToString("dd.MM.yyyy")} ({EstimatedQuantityUOM})";
        }

        #endregion

    }
}
