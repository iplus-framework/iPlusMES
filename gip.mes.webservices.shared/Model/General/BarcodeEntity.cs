// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="FacilityChargeSumFacilityHelper.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using gip.mes.facility;
using gip.core.datamodel;
using gip.core.processapplication;

namespace gip.mes.webservices
{
    [DataContract(Name = "cBE")]
    public class BarcodeEntity
    {
        [DataMember]
        public core.webservices.ACClass ACClass
        {
            get; set;
        }

        [DataMember(Name = "xMM")]
        public bool? MachineMalfunction
        {
            get;
            set;
        }

        [DataMember(Name = "xMA")]
        public AvailabilityState? MachineAvailability
        {
            get;
            set;
        }
        
        [DataMember(Name = "xM")]
        public Material Material
        {
            get; set;
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

        [DataMember(Name = "xFC")]
        public FacilityCharge FacilityCharge
        {
            get; set;
        }

        [DataMember(Name = "xP")]
        public Picking Picking
        {
            get; set;
        }

        [DataMember(Name = "ixPP")]
        public PickingPos PickingPos
        {
            get; set;
        }

        [DataMember(Name = "OWFI")]
        public ProdOrderPartslistWFInfo[] OrderWFInfos
        {
            get; set;
        }

        [DataMember(Name = "SOWF")]
        public ProdOrderPartslistWFInfo SelectedOrderWF
        {
            get; set;
        }

        [DataMember(Name = "xMR")]
        public Global.MsgResult? MsgResult
        {
            get; set;
        }

        [DataMember]
        public ACMethod WFMethod
        {
            get; set;
        }

        [DataMember]
        public BarcodeEntityCommand Command
        {
            get;set;
        }

        public object ValidEntity
        {
            get
            {
                if (ACClass != null)
                    return ACClass;
                else if (Material != null)
                    return Material;
                else if (FacilityLot != null)
                    return FacilityLot;
                else if (Facility != null)
                    return Facility;
                else if (FacilityCharge != null)
                    return FacilityCharge;
                else if (Picking != null)
                    return Picking;
                else if (PickingPos != null)
                    return PickingPos;
                else if (SelectedOrderWF != null)
                    return SelectedOrderWF;
                else if (WFMethod != null)
                    return WFMethod;
                else if (MsgResult != null)
                    return MsgResult;
                else if (Command != null)
                    return Command;
                return null;
            }
        }

        public string Barcode
        {
            get
            {
                if (ACClass != null)
                    return ACClass.ACClassID.ToString();
                else if (Material != null)
                    return Material.MaterialID.ToString();
                else if (FacilityLot != null)
                    return FacilityLot.FacilityLotID.ToString();
                else if (Facility != null)
                    return Facility.FacilityID.ToString();
                else if (FacilityCharge != null)
                    return FacilityCharge.FacilityChargeID.ToString();
                else if (Picking != null)
                    return Picking.PickingID.ToString();
                else if (PickingPos != null)
                    return PickingPos.PickingPosID.ToString();
                return null;
            }
        }
    }
}