using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cFC")]
    public class FacilityCharge : EntityBase, ICloneable
    {
        [DataMember(Name = "ID")]
        public Guid FacilityChargeID
        {
            get; set;
        }

        [DataMember(Name = "xM")]
        public Material Material
        {
            get;set;
        }

        [DataMember(Name = "xFL")]
        public FacilityLot FacilityLot
        {
            get; set;
        }

        [DataMember(Name = "xF")]
        public Facility Facility
        {
            get; set;
        }

        [DataMember(Name = "SN")]
        public int SplitNo
        {
            get; set;
        }

        [DataMember(Name = "SQ")]
        public double StockQuantity
        {
            get; set;
        }

        [DataMember(Name = "xMDU")]
        public MDUnit MDUnit
        {
            get; set;
        }

        [DataMember(Name = "xMDRS")]
        public MDReleaseState MDReleaseState
        {
            get; set;
        }

        [DataMember(Name = "FD")]
        public DateTime? FillingDate
        {
            get; set;
        }

        [DataMember(Name = "SL")]
        public short StorageLife
        {
            get; set;
        }

        [DataMember(Name = "PD")]
        public DateTime? ProductionDate
        {
            get; set;
        }

        [DataMember(Name = "ED")]
        public DateTime? ExpirationDate
        {
            get; set;
        }

        [DataMember(Name = "CM")]
        public string Comment
        {
            get; set;
        }

        [DataMember(Name = "NA")]
        public bool NotAvailable
        {
            get; set;
        }

        public object Clone()
        {
            FacilityCharge fc = new FacilityCharge();
            fc.Material = this.Material;
            fc.FacilityLot = this.FacilityLot;
            fc.Facility = this.Facility;

            return fc;
        }

        public void OnPropChanged(string propName)
        {
            OnPropertyChanged(propName);
        }
    }
}
