using gip.core.datamodel;
using gip.mes.datamodel;
using System;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'CompanyMaterialOverviewModel'}de{'CompanyMaterialOverviewModel'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]

    public class CompanyMaterialOverviewModel
    {
        [ACPropertyInfo(1, "", Const.Sn)]
        public int Sn { get; set; }

        public Guid MaterialID { get; set; }

        [ACPropertyInfo(2, "", ConstApp.MaterialNo)]
        public string MaterialNo { get; set; }


        [ACPropertyInfo(3, "", ConstApp.MaterialName1)]
        public string MaterialName { get; set; }


        [ACPropertyInfo(4, "", ConstApp.CompanyMaterialNo)]
        public string CompanyMaterialNo { get; set; }


        [ACPropertyInfo(5, "", ConstApp.CompanyMaterialName)]
        public string CompanyMaterialName { get; set; }


        [ACPropertyInfo(6, "", ConstApp.StockQuantity)]
        public MaterialStock CurrentMaterialStock { get; set; }

    }
}
