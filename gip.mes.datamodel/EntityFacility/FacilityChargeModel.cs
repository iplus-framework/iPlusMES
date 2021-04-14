using gip.core.datamodel;
using System;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityChargeModel'}de{'FacilityChargeModel'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class FacilityChargeModel : FacilityLotModel
    {

        [ACPropertyInfo(999, "FacilityNo", ConstApp.FacilityNo)]
        public string FacilityNo { get; set; }

        [ACPropertyInfo(5, "FacilityName", ConstApp.Facility)]
        public string FacilityName { get; set; }
       
        [ACPropertyInfo(8, "ProdOrderProgramNo", ConstApp.ProdOrderProgramNo)]
        public string ProdOrderProgramNo { get; set; }

        [ACPropertyInfo(9, "BatchNo", ConstApp.BatchNo)]
        public string BatchNo { get; set; }

        [ACPropertyInfo(10, "MachineName", "en{'Machine Name'}de{'Maschinename'}")]
        public string MachineName { get; set; }

        [ACPropertyInfo(999, "InOrderNo", "en{'Purchase order number'}de{'Bestellnummer'}")]
        public string InOrderNo { get; set; }

        [ACPropertyInfo(9999, "DosedQuantity", "en{'Dosed quantity'}de{'Dosierte Menge'}", "", false)]
        public double DosedQuantity { get; set; }

        public ProdOrderPartslistPos IntermediateItem { get; set; }

        public bool IsFinalOutput { get; set; }

        public Guid FacilityChargeID { get; set; }

        public int StepNo { get; set; }


    }
}
