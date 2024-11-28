using System;
using System.ComponentModel.DataAnnotations.Schema;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, ConstApp.PlanningMR, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOTemplateSchedule")]
    
    [ACPropertyEntity(1, nameof(PlanningMRNo), "en{'Schedule number'}de{'Plannummer'}", "", "", true)]
    [ACPropertyEntity(2, nameof(PlanningName), "en{'Name'}de{'Name'}", "", "", true)]
    [ACPropertyEntity(3, nameof(RangeFrom), "en{'Valid From'}de{'Gültig von'}", "", "", true)]
    [ACPropertyEntity(4, nameof(RangeTo), "en{'Valid To'}de{'Gültig bis'}", "", "", true)]
    [ACPropertyEntity(5, nameof(Template), "en{'Is template'}de{'Ist Vorlage'}", "", "", true)]
    [ACPropertyEntity(6, nameof(Comment), ConstApp.Comment, "", "", true)]
    
    [ACPropertyEntity(494, Const.EntityDeleteDate, Const.EntityTransDeleteDate)]
    [ACPropertyEntity(495, Const.EntityDeleteName, Const.EntityTransDeleteName)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + PlanningMR.ClassName, "en{'Planning'}de{'Planung'}", typeof(PlanningMR), PlanningMR.ClassName, "PlanningMRNo,PlanningName", "PlanningMRNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<PlanningMR>) })]
    [NotMapped]
    public partial class PlanningMR
    {
        #region const
        [NotMapped]
        public const string ClassName = "PlanningMR";
        [NotMapped]
        public const string NoColumnName = "PlanningMRNo";
        [NotMapped]
        public const string FormatNewNo = "MR{0}";
        #endregion

        #region IACObjectEntity Members
        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "PlanningMRNo";
            }
        }
        #endregion

        #region New/Delete
        public static PlanningMR NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            PlanningMR entity = new PlanningMR();
            entity.PlanningMRID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.PlanningMRNo = secondaryKey;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

    }
}
