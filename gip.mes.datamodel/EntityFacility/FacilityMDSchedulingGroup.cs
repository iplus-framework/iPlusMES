using gip.core.datamodel;
using System;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.FacilityMDSchedulingGroup, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, Facility.ClassName, ConstApp.Facility, Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, MDSchedulingGroup.ClassName, ConstApp.Facility, Const.ContextDatabase + "\\" + MDSchedulingGroup.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, MDPickingType.ClassName, ConstApp.PickingType, Const.ContextDatabase + "\\" + MDPickingType.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    public partial class FacilityMDSchedulingGroup
    {
        #region New/Delete

        public static FacilityMDSchedulingGroup NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            FacilityMDSchedulingGroup entity = new FacilityMDSchedulingGroup();
            entity.FacilityMDSchedulingGroupID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Facility)
            {
                entity.Facility = parentACObject as Facility;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion
    }
}
