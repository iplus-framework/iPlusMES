using System.Runtime.CompilerServices;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace gip.mes.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'BinSelectionItem'}de{'BinSelectionItem'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class BinSelectionItem : INotifyPropertyChanged
    {
        public Guid FacilityID { get; set; }
        public Guid FacilityChargeID { get; set; }

        [ACPropertyInfo(1, "FacilityNo", "en{'Storage No'}de{'Lagerplatz No.'}")]
        public string FacilityNo { get; set; }

        [ACPropertyInfo(2, "FacilityName", "en{'Storage Name'}de{'Lagerplatz name'}")]
        public string FacilityName { get; set; }

        [ACPropertyInfo(3, "IsReserved", "en{'Is reserved'}de{'Reserviert'}")]
        public bool IsReserved
        {
            get
            {
                if (LastInwardFBC == null)
                    return false;

                if (LastOutwardFBC == null)
                    return true;

                return LastOutwardFBC.InsertDate < LastInwardFBC.InsertDate;
            }
        }

        [ACPropertyInfo(4, "FaciltiyBookingNo", "en{'Reservation Booking No.'}de{'Rezervierung Buchung Nr.'}")]
        public string FaciltiyBookingNo { get; set; }

        public string BatchNo { get; set; }

        private FacilityBooking _LastInwardFBC;
        public FacilityBooking LastInwardFBC
        {
            get => _LastInwardFBC;
            set
            {
                _LastInwardFBC = value;
                OnPropertyChanged("IsReserved");
            }
        }

        private FacilityBooking _LastOutwardFBC;
        public FacilityBooking LastOutwardFBC
        {
            get => _LastOutwardFBC;
            set
            {
                _LastOutwardFBC = value;
                OnPropertyChanged("IsReserved");
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
