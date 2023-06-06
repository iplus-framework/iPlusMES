using gip.core.datamodel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESPriceListMaterial, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(9999, PriceList.ClassName, ConstApp.ESPriceList, Const.ContextDatabase + "\\" + PriceList.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9999, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, "Price", "en{'Price'}de{'Preis'}", "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + PriceListMaterial.ClassName, ConstApp.ESPriceListMaterial, typeof(PriceListMaterial), PriceListMaterial.ClassName, Material.ClassName + "\\MaterialNo", Material.ClassName + "\\MaterialNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<PriceListMaterial>) })]
    [NotMapped]
    public partial class PriceListMaterial
    {
        [NotMapped]
        public const string ClassName = "PriceListMaterial";

        #region New/Delete

        public static PriceListMaterial NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            PriceListMaterial entity = new PriceListMaterial();
            entity.PriceListMaterialID = Guid.NewGuid();
            entity.Context = dbApp;
            entity.DefaultValuesACObject();
            if (parentACObject != null && parentACObject is PriceList)
                entity.PriceList = parentACObject as PriceList;
            return entity;
        }

        #endregion
    }
}
