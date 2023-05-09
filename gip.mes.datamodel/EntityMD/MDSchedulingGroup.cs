using gip.core.datamodel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.MDSchedulingGroup, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, "MDSchedulingGroupIndex", "MDSchedulingGroupIndex", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + MDSchedulingGroup.ClassName, "en{'Scheduling Group'}de{'Scheduling Group'}", typeof(MDSchedulingGroup), MDSchedulingGroup.ClassName, Const.MDKey, Const.MDKey)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDSchedulingGroup>) })]
    public partial class MDSchedulingGroup : IACObjectEntity
    {

        public const string ClassName = "MDSchedulingGroup";

        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static MDSchedulingGroup NewACObject(DatabaseApp db, IACObject parentACObject)
        {
            MDSchedulingGroup entity = new MDSchedulingGroup();
            entity.MDSchedulingGroupID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(db.UserName, db);
            return entity;
        }

        #endregion

        #region IACObjectEntity

        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDSchedulingGroupName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDSchedulingGroupName");
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return MDSchedulingGroupName;
            }
        }

        #endregion

        #region Override

        public override string ToString()
        {
            return MDKey;
        }

        #endregion
    }
}
