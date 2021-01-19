using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'IntermediateMaterialOverview'}de{'IntermediateMaterialOverview'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class IntermediateMaterialOverview
    {

        [ACPropertyInfo(1, "Sn", "en{'Sn'}de{'Sn'}")]
        public int Sn { get; set; }

        [ACPropertyInfo(2, "MaterialNo", "en{'Mix Material-No.'}de{'Mix Artikel-Nr.'}")]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(3, "MaterialName", "en{'Mix Material Desc.'}de{'Mix Materialbez.'}")]
        public string MaterialName1 { get; set; }

        [ACPropertyInfo(4, "MaterialNo", "en{'Material-No.'}de{'Artikel-Nr.'}")]
        public string VMaterialNo { get; set; }

        [ACPropertyInfo(5, "MaterialName", "en{'Material Desc.'}de{'Materialbez.'}")]
        public string VMaterialName1 { get; set; }

        [ACPropertyInfo(6, "StockQuantity", ConstApp.StockQuantity)]
        public double? StockQuantity { get; set; }

        [ACPropertyInfo(7, "DayInward", "en{'Day Inward Qty'}de{'Tageszugang'}")]
        public double? DayInward { get; set; }

        [ACPropertyInfo(8, "DayOutward", "en{'Day Outward Qty'}de{'Tagesabgang'}")]
        public double? DayOutward { get; set; }

    }
}
