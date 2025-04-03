using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Facility charge item'}de{'Facility charge item'}", Global.ACKinds.TACSimpleClass)]
    public class FacilityChargeItem : EntityBase, IACObject
    {
        public FacilityChargeItem(FacilityChargeItem facilityChargeItem, double stockQuantity)
        {
            this.FacilityCharge = facilityChargeItem.FacilityCharge;
            this.FacilityChargeID = facilityChargeItem.FacilityChargeID;
            this.FacilityChargeNo = facilityChargeItem.FacilityChargeNo;
            this.FacilityNo = facilityChargeItem.FacilityNo;
            this.FacilityName = facilityChargeItem.FacilityName;
            this.ExternLotNo = facilityChargeItem.ExternLotNo;
            this.StockQuantityUOM = stockQuantity;
            this.FacilityChargeNo = facilityChargeItem.FacilityChargeNo;
            this.ExpirationDate = facilityChargeItem.ExpirationDate;
            this.FillingDate = facilityChargeItem.FillingDate;
            this.MDUnit = facilityChargeItem.MDUnit;
            this.FacilityType = facilityChargeItem.FacilityType;
            this.Comment = facilityChargeItem.Comment;
        }

        public FacilityChargeItem(FacilityCharge facilityCharge, double? targetQuantity)
        {
            FacilityCharge = facilityCharge;
            FacilityChargeID = facilityCharge.FacilityChargeID;
            FacilityChargeNo = facilityCharge.FacilityLot?.LotNo;
            ExternLotNo = facilityCharge.FacilityLot?.ExternLotNo;
            if (string.IsNullOrEmpty(FacilityChargeNo))
                FacilityChargeNo = facilityCharge.FacilityChargeSortNo.ToString();
            StockQuantityUOM = facilityCharge.StockQuantityUOM;
            ExpirationDate = facilityCharge.ExpirationDate;
            FillingDate = facilityCharge.FillingDate;
            MDUnit = facilityCharge.MDUnit;
            FacilityNo = facilityCharge.Facility.FacilityNo;
            FacilityName = facilityCharge.Facility.FacilityName;
            FacilityType = facilityCharge.Facility.MDFacilityType.FacilityType;
            Comment = facilityCharge.Comment;

            if (targetQuantity.HasValue && targetQuantity.Value > StockQuantityUOM)
            {
                Warning = true;
            }
        }

        [ACPropertyInfo(9999)]
        public FacilityCharge FacilityCharge
        {
            get;
            set;
        }

        public Guid FacilityChargeID
        {
            get;
            set;
        }

        [ACPropertyInfo(9999, "", ConstApp.ExternLotNo)]
        public string ExternLotNo
        {
            get;
            set;
        }

        [ACPropertyInfo(9999, "", ConstApp.LotNo)]
        public string FacilityChargeNo
        {
            get;
            set;
        }

        [ACPropertyInfo(9999, "", ConstApp.FacilityNo)]
        public string FacilityNo
        {
            get;
            set;
        }

        [ACPropertyInfo(9999, "", ConstApp.Name)]
        public string FacilityName
        {
            get;
            set;
        }

        private double _StockQuantityUOM;
        [ACPropertyInfo(9999, "", ConstApp.StockQuantity)]
        public double StockQuantityUOM
        {
            get => _StockQuantityUOM;
            set
            {
                _StockQuantityUOM = value;
                OnPropertyChanged();
            }
        }

        [ACPropertyInfo(9999, "", ConstApp.ExpirationDate)]
        public DateTime? ExpirationDate
        {
            get;
            set;
        }

        [ACPropertyInfo(9999, "", ConstApp.FillingDate)]
        public DateTime? FillingDate
        {
            get;
            set;
        }

        [ACPropertyInfo(9999)]
        public MDUnit MDUnit
        {
            get;
            set;
        }

        [ACPropertyInfo(9999, "", ConstApp.Comment)]
        public string Comment
        {
            get;
            set;
        }

        private bool _Warning;
        [ACPropertyInfo(9999)]
        public bool Warning
        {
            get => _Warning;
            set
            {
                _Warning = value;
                OnPropertyChanged("Warning");
            }
        }

        [ACPropertyInfo(9999)]
        public FacilityTypesEnum FacilityType
        {
            get;
            set;
        }

        public void OnTargetQunatityChanged(double? targetQuantity)
        {
            if (targetQuantity.HasValue && targetQuantity.Value > StockQuantityUOM)
            {
                Warning = true;
            }
        }

        public IACObject ParentACObject => null;

        public IACType ACType => this.ReflectACType();

        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        public string ACIdentifier => this.ReflectGetACIdentifier();

        public string ACCaption => FacilityChargeNo;

        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
        }

        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ReflectGetACUrl(rootACObject);
        }

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }
    }
}
