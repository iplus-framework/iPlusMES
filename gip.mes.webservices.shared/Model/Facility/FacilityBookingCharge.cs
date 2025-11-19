using gip.core.datamodel;
using System;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cFBC")]
    public class FacilityBookingCharge : EntityBase, ICloneable
    {
        [DataMember(Name = "ID")]
        public Guid FacilityBookingChargeID
        {
            get; set;
        }

        

        [DataMember(Name = "FBCN")]
        public string FacilityBookingChargeNo
        {
            get; set;
        }

        public object Clone()
        {
            FacilityBookingCharge fc = new FacilityBookingCharge();
            fc.FacilityBookingChargeID = this.FacilityBookingChargeID;
            fc.FacilityBookingChargeNo = this.FacilityBookingChargeNo;

            return fc;
        }

        public void OnPropChanged(string propName)
        {
            OnPropertyChanged(propName);
        }
    }
}
