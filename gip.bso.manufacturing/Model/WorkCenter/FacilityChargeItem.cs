using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Facility charge item'}de{'Facility charge item'}", Global.ACKinds.TACSimpleClass)]
    public class FacilityChargeItem : EntityBase, IACObject
    {
        public FacilityChargeItem(FacilityCharge facilityCharge, double? targetQuantity)
        {
            FacilityChargeID = facilityCharge.FacilityChargeID;
            FacilityChargeNo = facilityCharge.FacilityLot?.LotNo;
            if (string.IsNullOrEmpty(FacilityChargeNo))
                FacilityChargeNo = facilityCharge.FacilityChargeSortNo.ToString();
            StockQuantityUOM = facilityCharge.StockQuantityUOM;
            ExpirationDate = facilityCharge.ExpirationDate;
            FillingDate = facilityCharge.FillingDate;
            MDUnit = facilityCharge.MDUnit;
            FacilityNo = facilityCharge.Facility.FacilityNo;
            FacilityName = facilityCharge.Facility.FacilityName;

            if (targetQuantity.HasValue && targetQuantity.Value > StockQuantityUOM)
            {
                Warning = true;
            }
        }

        public Guid FacilityChargeID
        {
            get;
            set;
        }

        [ACPropertyInfo(9999)]
        public string FacilityChargeNo
        {
            get;
            set;
        }

        [ACPropertyInfo(9999)]
        public string FacilityNo
        {
            get;
            set;
        }

        [ACPropertyInfo(9999)]
        public string FacilityName
        {
            get;
            set;
        }

        private double _StockQuantityUOM;
        [ACPropertyInfo(9999)]
        public double StockQuantityUOM
        {
            get => _StockQuantityUOM;
            set
            {
                _StockQuantityUOM = value;
                OnPropertyChanged();
            }
        }

        [ACPropertyInfo(9999)]
        public DateTime? ExpirationDate
        {
            get;
            set;
        }

        [ACPropertyInfo(9999)]
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
