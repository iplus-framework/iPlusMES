using gip.core.datamodel;
using System;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESTaxMDMaterialGroup, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(9999, Tax.ClassName, ConstApp.ESTax, Const.ContextDatabase + "\\" + Tax.ClassName, "", true)]
    [ACPropertyEntity(9999, MDMaterialGroup.ClassName, ConstApp.ESMaterialGroup, Const.ContextDatabase + "\\" + MDMaterialGroup.ClassName, "", true)]
    [ACPropertyEntity(1, "TaxValue", "en{'Value'}de{'Wert'}", "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + TaxMDMaterialGroup.ClassName, ConstApp.ESTaxMaterial, typeof(TaxMDMaterialGroup), TaxMDMaterialGroup.ClassName, MDMaterialGroup.ClassName + "\\MDKey", MDMaterialGroup.ClassName + "\\MDKey")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<TaxMDMaterialGroup>) })]
    public partial class TaxMDMaterialGroup
    {
         public const string ClassName = "TaxMDMaterialGroup";

        #region New/Delete

        public static TaxMDMaterialGroup NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            TaxMDMaterialGroup entity = new TaxMDMaterialGroup();
            entity.TaxMDMaterialGroupID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject != null && parentACObject is Tax)
                (parentACObject as Tax).TaxMDMaterialGroup_Tax.Add(entity);
            return entity;
        }

        #endregion
    }
}
