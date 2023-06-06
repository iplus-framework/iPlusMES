using gip.core.datamodel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{

    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESCountrySalesTaxMaterial, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(9999, MDCountrySalesTax.ClassName, ConstApp.ESCountrySalesTax, Const.ContextDatabase + "\\" + MDCountrySalesTax.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9999, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, "SalesTax", ConstApp.ESCountrySalesTax, "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + MDCountrySalesTaxMaterial.ClassName, ConstApp.ESCountrySalesTaxMaterial, typeof(MDCountrySalesTaxMaterial), MDCountrySalesTaxMaterial.ClassName, Material.ClassName + "\\MaterialNo", Material.ClassName + "\\MaterialNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDCountrySalesTaxMaterial>) })]
    [NotMapped]
    public partial class MDCountrySalesTaxMaterial
    {
        [NotMapped]
        public const string ClassName = "MDCountrySalesTaxMaterial";

        #region New/Delete

        public static MDCountrySalesTaxMaterial NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDCountrySalesTaxMaterial entity = new MDCountrySalesTaxMaterial();
            entity.MDCountrySalesTaxMaterialID = Guid.NewGuid();
            entity.Context = dbApp;
            entity.DefaultValuesACObject();
            if (parentACObject != null && parentACObject is MDCountrySalesTax)
                (parentACObject as MDCountrySalesTax).MDCountrySalesTaxMaterial_MDCountrySalesTax.Add(entity);
            return entity;
        }

        #endregion
    }
}
