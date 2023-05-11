using System;
using gip.core.datamodel;


namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'ProdOrderPartslistPosFacilityLot'}de{'ProdOrderPartslistPosFacilityLot'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, ProdOrderPartslistPos.ClassName, "en{'ProdOrderPartslistPos'}de{'ProdOrderPartslistPos'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, FacilityLot.ClassName, "en{'Facility Lot'}de{'Chargen-Los'}", Const.ContextDatabase + "\\" + FacilityLot.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, "IsActive", "en{'Is active'}de{'Ist aktiv'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + ProdOrderPartslistPosFacilityLot.ClassName, "en{'ProdOrderPartslistPosFacilityLot'}de{'ProdOrderPartslistPosFacilityLot'}", typeof(ProdOrderPartslistPosFacilityLot), ProdOrderPartslistPosFacilityLot.ClassName, "IsActiv", "IsActiv")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<ProdOrderPartslistPosFacilityLot>) })]
    public partial class ProdOrderPartslistPosFacilityLot
    {
        public const string ClassName = "ProdOrderPartslistPosFacilityLot";

        #region New/Delete
        public static ProdOrderPartslistPosFacilityLot NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            ProdOrderPartslistPosFacilityLot entity = new ProdOrderPartslistPosFacilityLot();
            entity.ProdOrderPartslistPosFacilityLotID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is ProdOrderPartslistPos)
            {
                entity.ProdOrderPartslistPos = (parentACObject as ProdOrderPartslistPos);
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }
        #endregion
    }
}
