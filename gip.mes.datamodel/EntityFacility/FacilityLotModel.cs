using gip.core.datamodel;
using System;

namespace gip.mes.datamodel
{

    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityLotModel'}de{'FacilityLotModel'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class FacilityLotModel
    {
        [ACPropertyInfo(999, "LotNo", ConstApp.LotNo)]
        public string LotNo { get; set; }

        [ACPropertyInfo(999, "ExternLotNo", ConstApp.ExternLotNo)]
        public string ExternLotNo { get; set; }

        [ACPropertyInfo(999, "ExternLotNo2", ConstApp.ExternLotNo2)]
        public string ExternLotNo2 { get; set; }

        [ACPropertyInfo(999, "MaterialNo", ConstApp.MaterialNo)]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(999, "MaterialName1", ConstApp.MaterialName1)]
        public string MaterialName1 { get; set; }

        [ACPropertyInfo(999, "InsertDate", Const.EntityTransInsertDate)]
        public DateTime InsertDate { get; set; }

        [ACPropertyInfo(9999, "ActualQuantity", ConstApp.ActualQuantity)]
        public double ActualQuantity { get; set; }

        [ACPropertyInfo(9999, "TargetQuantity", ConstApp.TargetQuantity)]
        public double TargetQuantity { get; set; }

        [ACPropertyInfo(999, "Comment", ConstApp.Comment)]
        public string Comment { get; set; }

        [ACPropertyInfo(9999, "MDUnitName", ConstApp.MDUnit)]
        public string MDUnitName { get; set; }

        public Guid FacilityLotID { get; set; }


        #region Mehtods
        public void RoundActualQuantity(string mdUnitForRounding = null)
        {
            if (mdUnitForRounding == null || mdUnitForRounding.Contains(MDUnitName))
            {
                ActualQuantity = (int)Math.Round(ActualQuantity);
            }
        }
        #endregion

    }
}
