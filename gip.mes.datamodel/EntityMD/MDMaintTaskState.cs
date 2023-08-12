using System;
using System.ComponentModel.DataAnnotations.Schema;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, ConstApp.ESMaintTaskState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    //[ACPropertyEntity(3, "MDMaintTaskStateIndex", ConstApp.ESMaintTaskState, typeof(MaintTaskState), "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MDMaintTaskState.ClassName, ConstApp.ESMaintTaskState, typeof(MDMaintTaskState), MDMaintTaskState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [NotMapped]
    public partial class MDMaintTaskState
    {
        [NotMapped]
        public const string ClassName = "MDMaintTaskState";

        public static MDMaintTaskState NewACObject(DatabaseApp dbApp, IACObject parent)
        {
            var entity = new MDMaintTaskState();
            entity.MDMaintTaskStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDMaintTaskStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDMaintTaskStateName");
            }
        }
    }
}
