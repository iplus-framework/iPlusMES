using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility.TandTv3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using TandTv3 = gip.mes.facility.TandTv3;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTv3PointPosGrouped'}de{'TandTv3PointPosGrouped'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, false)]
    public class TandTv3PointPosGrouped : TandTv3Point
    {

        #region ctor's

        public TandTv3PointPosGrouped() : base()
        {
            InwardLotsList = new List<FacilityLotModel>();
            ChildMixPointIds = new List<Guid>();
            InwardMaterials = new List<Material>();
            InwardBatchList = new List<string>();
        }

        #endregion

        #region Group members


        #endregion

        #region Lots

        #region Lots -> Properties

        public string SumInwardLotNo
        {
            get
            {
                if (InwardLotsList == null || !InwardLotsList.Any()) return null;
                return string.Join(",", InwardLotsList.Select(x => x.LotNo).OrderBy(c => c));
            }
        }


        [ACPropertyInfo(9999, "InwardBatchList", "en{'Batch Nos.'}de{'Batchnummers'}")]
        public List<string> InwardBatchList { get; set; }

        public List<Guid> ChildMixPointIds { get; set; }

        [ACPropertyInfo(9999, "InwardLotsNos", "en{'Output lots'}de{'Ergebnislosnummerns'}")]
        public override string InwardLotsNos
        {
            get
            {
                if (InwardLotsList == null || !InwardLotsList.Any()) return "";
                if (InwardLotsList.Count() <= 3)
                    return string.Join(",", InwardLotsList.Select(c => c.LotNo).OrderBy(c => c));
                else
                    return string.Format(@"{0}...{1}", InwardLotsList.FirstOrDefault().LotNo, InwardLotsList.LastOrDefault().LotNo);
            }
        }

        #endregion

        #region Lots -> (Selected)InwardLots

        private List<FacilityLotModel> _InwardLotsList;
        [ACPropertyList(9999, "InwardLots")]
        public List<FacilityLotModel> InwardLotsList
        {
            get
            {
                return _InwardLotsList;
            }
            set
            {
                _InwardLotsList = value;
                OnPropertyChanged("InwardLotsList");
            }
       }

        private FacilityLotModel _SelectedInwardLots;
        [ACPropertySelected(9999, "InwardLots")]
        public FacilityLotModel SelectedInwardLots
        {
            get
            {
                return _SelectedInwardLots;
            }
            set
            {
                if (_SelectedInwardLots != value)
                {
                    _SelectedInwardLots = value;
                    OnPropertyChanged("SelectedInwardLots");
                }
            }
        }

        #endregion

        #region Lots -> Methods


        #endregion

        #endregion

        #region Materials

        #region Materials -> InwardMaterials

        public List<Material> InwardMaterials { get; set; }

        #endregion

        #endregion

        #region Facility

        public void AddInwardFacility(FacilityPreview facilityPreview, double quantity)
        {
            AddFacility(InwardFacilities, facilityPreview, quantity);
        }

        public void AddOutwardFacility(FacilityPreview facilityPreview, double quantity)
        {
            AddFacility(OutwardFacilities, facilityPreview, quantity);
        }
        public void AddFacility(Dictionary<string, FacilityPreview> list, FacilityPreview facilityPreview, double quantity)
        {
            FacilityPreview tmpFacilityPrevew = null;
            if (list.Keys.Contains(facilityPreview.FacilityNo))
                tmpFacilityPrevew = list[facilityPreview.FacilityNo];
            else
            {
                tmpFacilityPrevew = new FacilityPreview()
                {
                    FacilityNo = facilityPreview.FacilityNo,
                    FacilityACClass = facilityPreview.FacilityACClass,
                    VBiFacilityACClassID = facilityPreview.VBiFacilityACClassID,
                    FacilityID = facilityPreview.FacilityID,
                    FacilityName = facilityPreview.FacilityName,
                    MaterialNo = facilityPreview.MaterialNo,
                    MaterialName1 = facilityPreview.MaterialName1

                };
                list.Add(facilityPreview.FacilityNo, tmpFacilityPrevew);
            }
            tmpFacilityPrevew.StockQuantityUOM += quantity;
        }
        #endregion

        #region IACObject

        public override string GetACCaption()
        {
            if (IsProductionPoint)
            {
                string lotNos = "";
                if (InwardLotsList.Count() > 3)
                    lotNos = string.Join(",", InwardLotsList.OrderBy(c => c.LotNo).Take(3)) + "...";
                else
                    lotNos = InwardLot.LotNo;
                return string.Format(@"{0}({1}) {2}", InwardMaterialName, lotNos, PositionsActualQuantityUOM.ToString("#0.00"));
            }
            else
                return base.GetACCaption();
        }

        #endregion

    }
}
