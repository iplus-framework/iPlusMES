using System;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maint Order Properties'}de{'Wartungsauftrag Eigenschaften'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "MaintACClassProperty", "en{'ACClass property'}de{'ACClass property'}", "", "", true)]
    [ACPropertyEntity(2, "SetValue", "en{'Max Value'}de{'Maximum'}", "", "", true)]
    [ACPropertyEntity(3, "ActValue", "en{'Actual Value'}de{'Istwert'}", "", "", true)]
    [ACPropertyEntity(4, "MDMaintOrderPropertyState", "en{'Maintenance Order Property'}de{'Wartungsauftrag Eigenschaften'}", Const.ContextDatabase + "\\MDMaintOrderPropertyState", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MaintOrderProperty.ClassName, "en{'Maintenance Order Property'}de{'Wartungsauftrag Eigenschaften'}", typeof(MaintOrderProperty), MaintOrderProperty.ClassName, gip.core.datamodel.ACClassProperty.ClassName + "\\ACIdentifier", gip.core.datamodel.ACClassProperty.ClassName + "\\ACIdentifier")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaintOrderProperty>) })]
    public partial class MaintOrderProperty : IACObjectEntity
    {
        public const string ClassName = "MaintOrderProperty";

        #region New/Delete
        public static MaintOrderProperty NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MaintOrderProperty entity = new MaintOrderProperty();
            entity.MaintOrderPropertyID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is MaintOrder)
            {
                entity.MaintOrder = parentACObject as MaintOrder;
            }

            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            return ACCaption;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                return ACIdentifier;
            }
        }

        /// <summary>
        /// Returns MaintOrder
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to MaintOrder</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return MaintOrder;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "ACClassProperty\\ACIdentifier";
            }
        }

        #endregion

    }
}
