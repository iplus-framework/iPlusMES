﻿using gip.core.datamodel;
using gip.core.processapplication;
using gip.mes.datamodel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace gip.mes.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    //[ACClassInfo(Const.PackName_VarioSystem, "en{'BinSelectionItem'}de{'BinSelectionItem'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class DosingRestInfo //: INotifyPropertyChanged
    {
        public DosingRestInfo()
        {
        }

        public DosingRestInfo(PAMSilo silo, PAFDosing dosing, double? minZeroTol)
        {
            DosedQuantity = 0;
            PAEScaleBase scale = dosing.CurrentScaleForWeighing;
            if (scale != null)
            {
                DosedQuantity = scale.ActualWeight.ValueT;
                if (!dosing.IsSimulationOn)
                {
                    PAEScaleTotalizing totalizingScale = scale as PAEScaleTotalizing;
                    if (totalizingScale != null)
                        DosedQuantity = totalizingScale.TotalActualWeight.ValueT;
                }
            }

            silo.RefreshFacility(false, null);

            ZeroTol = 10;
            if (silo.Facility.ValueT != null && silo.Facility.ValueT.ValueT != null)
            {
                ZeroTol = silo.Facility.ValueT.ValueT.Tolerance;
                Stock = silo.FillLevel.ValueT;
                FacilityNo = silo.Facility.ValueT.ValueT.FacilityNo;
                FacilityName = silo.Facility.ValueT.ValueT.FacilityName;
            }
            if (minZeroTol.HasValue && ZeroTol <= 0.1)
                ZeroTol = minZeroTol.Value;
        }

        //[ACPropertyInfo(1, "FacilityNo", "en{'Storage No'}de{'Lagerplatz No.'}")]
        [DataMember(Name = "FNo")]
        public string FacilityNo
        {
            get; set;
        }

        [DataMember(Name = "FNa")]
        public string FacilityName
        {
            get; set;
        }

        [DataMember(Name = "S")]
        public double Stock
        {
            get; set;
        }

        [DataMember(Name = "D")]
        public double DosedQuantity
        {
            get; set;
        }

        public double RemainingStock
        {
            get
            {
                return Stock - DosedQuantity;
            }
        }

        [DataMember(Name = "T")]
        public double ZeroTol
        {
            get; set;
        }

        public bool IsZeroTolSet
        {
            get
            {
                return Math.Abs(ZeroTol) > Double.Epsilon;
            }
        }

        public bool InZeroTolerance
        {
            get
            {
                return RemainingStock < ZeroTol;
            }
        }
    }
}