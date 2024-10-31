using System;

namespace gip.mes.processapplication
{

    /// <summary>
    /// BinSelectionModel
    /// </summary>
    public class BinSelectionModel
    {

        #region Properties
        public Guid? ProdorderPartslistPosID { get; set; }
        public Guid? ProdorderPartslistPosRelationID { get; set; }

        public Guid? FacilityID { get; set; }
        public Guid? FacilityChargeID { get; set; }

        public Guid? FacilityLotID { get; set; }

        public double RestQuantity { get; set; }
        public string Comment { get; internal set; }

        #endregion

        #region OVerrides

        public override string ToString()
        {
            return string.Format(@"FaciltiyID:{0} FacilityChargeID:{1} Quantity:{2} | {3}", FacilityID, FacilityChargeID, RestQuantity, 
                ProdorderPartslistPosID != null ? "P" : (ProdorderPartslistPosRelationID != null ? "R" : ""));
        }
        #endregion
    }
}
