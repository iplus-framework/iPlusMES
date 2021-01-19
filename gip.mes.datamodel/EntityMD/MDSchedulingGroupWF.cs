using gip.core.datamodel;
using System;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'MDSchedulingGroupWF'}de{'MDSchedulingGroupWF'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, gip.core.datamodel.ACClassWF.ClassName, "en{'To Workflownode'}de{'Workflownode'}", Const.ContextDatabase + "\\" + gip.core.datamodel.ACClassWF.ClassName, "", true)]
    [ACPropertyEntity(2, "MDSchedulingGroup", "en{'MateriaWF-ACClassMethod'}de{'MateriaWF-ACClassMethod'}", Const.ContextDatabase + "\\" + MDSchedulingGroup.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDSchedulingGroupWF>) })]
    public partial class MDSchedulingGroupWF : IACObjectEntity
    {
        public const string ClassName = "MDSchedulingGroupWF";


        #region New / Delete
        public static MDSchedulingGroupWF NewACObject(DatabaseApp db, IACObject parentACObject)
        {
            MDSchedulingGroupWF entity = new MDSchedulingGroupWF();
            entity.MDSchedulingGroupWFID = Guid.NewGuid();
            if (parentACObject != null)
                if (parentACObject is MDSchedulingGroup)
                    entity.MDSchedulingGroup = (parentACObject as MDSchedulingGroup);
            entity.SetInsertAndUpdateInfo(Database.Initials, db);
            return entity;
        }

        #endregion

        #region Properties

        [ACPropertyInfo(9999, "ACClassWF", "en{'ACClassWF'}de{'ACClassWF'}")]
        public gip.core.datamodel.ACClassWF ACClassWF
        {
            get
            {
                if (VBiACClassWF == null) return null;
                return VBiACClassWF.FromIPlusContext<gip.core.datamodel.ACClassWF>(this.GetObjectContext<DatabaseApp>().ContextIPlus);
            }
            set
            {
                if (value == null)
                    VBiACClassWF = null;
                else
                    VBiACClassWF = value.FromAppContext<ACClassWF>(this.GetObjectContext<DatabaseApp>());
            }
        }

        #endregion

        #region Override

        public override string ToString()
        {
            return ""; // MDSchedulingGroup.MDKey + " <-> " + VBiACClassWF != null ? VBiACClassWF.ACIdentifier : "-";
        }

        #endregion
    }
}
