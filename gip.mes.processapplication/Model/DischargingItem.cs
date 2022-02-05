using System.Runtime.CompilerServices;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace gip.mes.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'DischargingItem'}de{'DischargingItem'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class DischargingItem : IACObject, INotifyPropertyChanged
    {

        public ManualPreparationSourceInfoTypeEnum SourceInfoType { get; set; }

        [ACPropertyInfo(1, "ItemID", "en{'ItemID'}de{'ItemID'}")]
        public Guid ItemID { get; set; }

        [ACPropertyInfo(2, "LotNo", "en{'Lot'}de{'Los'}")]
        public string LotNo { get; set; }

        [ACPropertyInfo(3, "InwardFacilityNo", "en{'Inward Storage No'}de{'Ergebnis Lagerplatz No.'}")]
        public string InwardFacilityNo { get; set; }

        [ACPropertyInfo(4, "InwardFacilityName", "en{'Inward Storage Name'}de{'Ergebnis Lgp. Name'}")]
        public string InwardFacilityName { get; set; }

        [ACPropertyInfo(5, "InwardBookingNo", "en{'Inward Booking No'}de{'Ergebnis Buchung Nr.'}")]
        public string InwardBookingNo { get; set; }

        [ACPropertyInfo(6, "InwardBookingQuantityUOM", "en{'Inward Quantity'}de{'Ergebnismege'}")]
        public double InwardBookingQuantityUOM { get; set; }

        [ACPropertyInfo(7, "InwardBookingTime", "en{'Inward Booking Time'}de{'Ergebnisbuchung Zeit'}")]
        public DateTime InwardBookingTime { get; set; }

        [ACPropertyInfo(8, "InwardMaterialNo", "en{'MaterialNo'}de{'MaterialNr'}")]
        public string InwardMaterialNo { get; set; }

        [ACPropertyInfo(9, "InwardMaterialName", "en{'Name'}de{'Name'}")]
        public string InwardMaterialName { get; set; }

        [ACPropertyInfo(10, "OutwardBookingNo", "en{'Outward Booking No'}de{'Einzatz Buchung Nr.'}")]
        public string OutwardBookingNo { get; set; }

        private double? _OutwardBookingQuantityUOM;
        [ACPropertyInfo(11, "OutwardBookingQuantityUOM", "en{'Outward Quantity'}de{'Einsaztmenge'}")]
        public double? OutwardBookingQuantityUOM
        {
            get => _OutwardBookingQuantityUOM;
            set
            {
                _OutwardBookingQuantityUOM = value;
                OnPropertyChanged("OutwardBookingQuantityUOM");
                OnPropertyChanged("IsDischarged");
            }
        }

        [ACPropertyInfo(12, "OutwardBookingTime", "en{'Outward Booking Time'}de{'Einzatzbuchung Zeit'}")]
        public DateTime? OutwardBookingTime { get; set; }

        [ACPropertyInfo(13, "IsDischarged", "en{'Is Discharged'}de{'Ist entleeren'}")]
        public bool IsDischarged
        {
            get
            {
                return Math.Abs((OutwardBookingQuantityUOM ?? 0) - InwardBookingQuantityUOM) <= PWBinSelection.BinSelectionReservationQuantity;
            }
        }

        private bool _IsDischarging;
        public bool IsDischarging
        {
            get => _IsDischarging;
            set
            {
                _IsDischarging = value;
                OnPropertyChanged("IsDischarging");
            }
        }

        public Guid ProdorderPartslistPosID { get; set; }
        public Guid? ProdorderPartslistPosRelationID { get; set; }

        public Guid? InwardFacilityChargeID { get; set; }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IACObject members

        public IACObject ParentACObject => null;

        public IACType ACType => this.ReflectACType();

        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        public string ACIdentifier => "DischargingItem";

        public string ACCaption => ACIdentifier;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ReflectGetACUrl(rootACObject);
        }

        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
        }

        #endregion
    }
}
