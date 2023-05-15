using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.ESUnitConversion, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "ToMDUnit", "en{'To Unit'}de{'Nach Einheit'}", Const.ContextDatabase + "\\" + MDUnit.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, "Multiplier", "en{'Multiplier (From)'}de{'Multiplikator (Von)'}", "", "", true, DefaultValue = 1)]
    [ACPropertyEntity(3, "Divisor", "en{'Divisor (To)'}de{'Teiler (Nach)'}", "", "", true, DefaultValue = 1)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MDUnitConversion.ClassName, ConstApp.ESUnitConversion, typeof(MDUnitConversion), MDUnitConversion.ClassName, "ToMDUnit\\ISOCode", "ToMDUnit\\ISOCode")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDUnitConversion>) })]
    [NotMapped]
    public partial class MDUnitConversion : IACObjectEntity
    {
        [NotMapped]
        public const string ClassName = "MDUnitConversion";

        #region New/Delete
        public static MDUnitConversion NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDUnitConversion entity = new MDUnitConversion();
            entity.MDUnitConversionID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is MDUnit)
            {
                entity.MDUnit = parentACObject as MDUnit;
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
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return ToMDUnit.MDUnitName;
            }
        }

        /// <summary>
        /// Returns MDUnit
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to MDUnit</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return MDUnit;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "ToMDUnit\\MDUnitName";
            }
        }
        #endregion

    }
}
