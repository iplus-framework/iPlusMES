using gip.core.datamodel;
using System;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESTaxMaterial, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(9999, Tax.ClassName, ConstApp.ESTax, Const.ContextDatabase + "\\" + Tax.ClassName, "", true)]
    [ACPropertyEntity(9999, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(3, "Price", "en{'Price'}de{'Preis'}", "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + TaxMaterial.ClassName, ConstApp.ESTaxMaterial, typeof(TaxMaterial), TaxMaterial.ClassName, Material.ClassName + "\\MaterialNo", Material.ClassName + "\\MaterialNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<TaxMaterial>) })]
    public partial class TaxMaterial
    {
        public const string ClassName = "TaxMaterial";

        #region New/Delete

        public static TaxMaterial NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            TaxMaterial entity = new TaxMaterial();
            entity.TaxMaterialID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject != null && parentACObject is Tax)
                entity.Tax = parentACObject as Tax;
            return entity;
        }

        #endregion
    }
}
