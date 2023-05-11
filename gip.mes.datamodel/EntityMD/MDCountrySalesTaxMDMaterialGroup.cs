using gip.core.datamodel;
using System;

namespace gip.mes.datamodel
{
    
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESCountrySalesTaxMDMaterialGroup, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(9999, MDCountrySalesTax.ClassName, ConstApp.ESCountrySalesTax, Const.ContextDatabase + "\\" + MDCountrySalesTax.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9999, MDMaterialGroup.ClassName, ConstApp.ESMaterialGroup, Const.ContextDatabase + "\\" + MDMaterialGroup.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, "SalesTax", ConstApp.ESCountrySalesTax, "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + MDCountrySalesTaxMDMaterialGroup.ClassName, ConstApp.ESCountrySalesTaxMDMaterialGroup, typeof(MDCountrySalesTaxMDMaterialGroup), MDCountrySalesTaxMDMaterialGroup.ClassName, MDMaterialGroup.ClassName + "\\MDKey", MDMaterialGroup.ClassName + "\\MDKey")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDCountrySalesTaxMDMaterialGroup>) })]
    public partial class MDCountrySalesTaxMDMaterialGroup
    {
        public const string ClassName = "MDCountrySalesTaxMDMaterialGroup";

        #region New/Delete

        public static MDCountrySalesTaxMDMaterialGroup NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDCountrySalesTaxMDMaterialGroup entity = new MDCountrySalesTaxMDMaterialGroup();
            entity.MDCountrySalesTaxMDMaterialGroupID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject != null && parentACObject is MDCountrySalesTax)
                (parentACObject as MDCountrySalesTax).MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax.Add(entity);
            return entity;
        }

        #endregion
    }
}
