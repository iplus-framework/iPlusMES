using gip.core.datamodel;

namespace gip.bso.masterdata
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'CompanyMaterialOverviewTypeEnum'}de{'CompanyMaterialOverviewTypeEnum'}", Global.ACKinds.TACEnum, QRYConfig = "gip.bso.masterdata.ACValueListCompanyMaterialOverviewTypeEnum")]
    public enum CompanyMaterialOverviewTypeEnum : short
    {
        /// <summary>
        /// Filter FacilityCharge.CompanyMaterialID
        /// </summary>
        ViewSupplierStock = 0,

        /// <summary>
        /// -> Material - Lotmanged: Filter FacilityCharge.CPartnerCompanyMaterial
	    /// -> Else: CompanyMaterialStock
        /// </summary>
        ViewPartnerStock = 1,

        /// <summary>
        /// Filter na FacilityCharge.MaterialID
        /// </summary>
        ViewPartnerStockOfUniqueMaterials = 2
    }

    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'ACValueListCompanyMaterialOverviewTypeEnum'}de{'ACValueListCompanyMaterialOverviewTypeEnum'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListCompanyMaterialOverviewTypeEnum : ACValueItemList
    {
        public ACValueListCompanyMaterialOverviewTypeEnum() : base("CompanyMaterialOverviewTypeEnum")
        {
            AddEntry((short)CompanyMaterialOverviewTypeEnum.ViewSupplierStock, "en{'View Supplier stock'}de{'Lieferantenbestand anzeigen'}");
            AddEntry((short)CompanyMaterialOverviewTypeEnum.ViewPartnerStock, "en{'View Partner stock'}de{'Partnerbestand anzeigen'}");
            AddEntry((short)CompanyMaterialOverviewTypeEnum.ViewPartnerStockOfUniqueMaterials, "en{'View Partner stock of unique Materials'}de{'Sehen Sie sich den Partnerbestand einzigartiger Materialien an'}");
        }
    }
}
