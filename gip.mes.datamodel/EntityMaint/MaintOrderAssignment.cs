using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'MaintOrderAssignment'}de{'MaintOrderAssignment'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, "Comment", "en{'Tasks'}de{'Aufgaben'}")]
    [ACPropertyEntity(2, MaintOrder.ClassName, "en{'Maintenance order'}de{'Maintenance order'}", Const.ContextDatabase + "\\" + MaintOrder.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, core.datamodel.VBGroup.ClassName, "en{'Group'}de{'Gruppe'}", Const.ContextDatabaseIPlus + "\\" + core.datamodel.VBGroup.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, core.datamodel.VBUser.ClassName, "en{'User'}de{'Benutzer'}", Const.ContextDatabaseIPlus + "\\" + core.datamodel.VBUser.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, Company.ClassName, "en{'Company'}de{'Unternehmen'}", Const.ContextDatabaseIPlus + "\\" + Company.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, nameof(IsDefault), "en{'Default'}de{'Standard'}")]
    [ACPropertyEntity(7, nameof(IsActive), "en{'Active'}de{'Aktiv'}")]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MaintOrderAssignment.ClassName, "en{'MaintOrderAssignment'}de{'MaintOrderAssignment'}", typeof(MaintOrderAssignment), MaintOrderAssignment.ClassName, "", "")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaintOrderAssignment>) })]
    public partial class MaintOrderAssignment
    {
        public const string ClassName = nameof(MaintOrderAssignment);

        #region New

        public static MaintOrderAssignment NewACObject(DatabaseApp dbApp, IACObject parent)
        {
            var entity = new MaintOrderAssignment();
            entity.MaintOrderAssignmentID = Guid.NewGuid();
            MaintOrder maintOrder = parent as MaintOrder;
            if (maintOrder != null)
            {
                entity.MaintOrder = maintOrder;
            }

            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        [NotMapped]
        [ACPropertyInfo(999)]
        public core.datamodel.VBGroup VBGroupTask
        {
            get
            {
                return VBGroup.FromIPlusContext<core.datamodel.VBGroup>();
            }
        }

        [NotMapped]
        [ACPropertyInfo(999)]
        public string AssignmentName
        {
            get
            {
                if (VBUser != null)
                    return VBUser.VBUserName;
                else if (VBGroup != null)
                    return VBGroup.VBGroupName;
                else if (Company != null)
                    return Company.CompanyName;

                return null;
            }
        }

        [NotMapped]
        [ACPropertyInfo(999)]
        public string AssignmentType
        {
            get
            {
                if (VBUser != null)
                    return ACRoot.SRoot.Environment.TranslateText(this, "txtUser");
                else if (VBGroup != null)
                    return ACRoot.SRoot.Environment.TranslateText(this, "txtGroup");
                else if (Company != null)
                    return ACRoot.SRoot.Environment.TranslateText(this, "txtCompany");

                return null;
            }
        }

        public void CopyAssignmentValues(MaintOrderAssignment newAssignment)
        {
            newAssignment.VBUserID = VBUserID;
            newAssignment.VBGroupID = VBGroupID;
            newAssignment.CompanyID = CompanyID;
            newAssignment.Comment = Comment;
            newAssignment.IsActive = IsActive;
            newAssignment.IsDefault = IsDefault;
            newAssignment.XMLConfig = XMLConfig;
        }

        #endregion
    }
}
