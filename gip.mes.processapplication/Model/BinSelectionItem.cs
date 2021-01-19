using gip.core.datamodel;
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
                if (!LastInwardFBDateTime.HasValue)
                    return false;

                if (!LastOutwardFBDateTime.HasValue)
                    return true;

                return LastOutwardFBDateTime < LastInwardFBDateTime;
            }
        }

        [ACPropertyInfo(4, "FaciltiyBookingNo", "en{'Reservation Booking No.'}de{'Rezervierung Buchung Nr.'}")]
        public string FaciltiyBookingNo { get; set; }

        public string BatchNo { get; set; }

        private DateTime? _LastInwardFBDateTime;
        public DateTime? LastInwardFBDateTime
        {
            get => _LastInwardFBDateTime;
            set
            {
                _LastInwardFBDateTime = value;
                OnPropertyChanged("IsReserved");
            }
        }

        private DateTime? _LastOutwardFBDateTime;
        public DateTime? LastOutwardFBDateTime
        {
            get => _LastOutwardFBDateTime;
            set
            {
                _LastOutwardFBDateTime = value;
                OnPropertyChanged("IsReserved");
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
