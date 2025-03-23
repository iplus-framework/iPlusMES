// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cPOPLPos")]
    public class ProdOrderPartslistPos : EntityBase
    {
        [DataMember(Name = "ID")]
        public Guid ProdOrderPartslistPosID
        {
            get; set;
        }

        [DataMember(Name = "MT")]
        public Material Material
        {
            get; set;
        }

        [DataMember(Name = "BMTID")]
        public Guid? BookingMaterialID
        {
            get;set;
        }

        [DataMember(Name = "BMT")]
        public Material BookingMaterial
        {
            get; set;
        }

        [DataMember(Name = "FLID")]
        public Guid? FacilityLotID
        {
            get;set;
        }

        [DataMember(Name = "POPL")]
        public ProdOrderPartslist ProdOrderPartslist
        {
            get;set;
        }

        [DataMember(Name = "POB")]
        public ProdOrderBatch ProdOrderBatch
        {
            get; set;
        }

        [DataMember(Name = "SQ")]
        public int Sequence
        {
            get; set;
        }

        [DataMember(Name = "TQ")]
        public double TargetQuantity
        {
            get; set;
        }

        [DataMember(Name = "TQU")]
        public double TargetQuantityUOM
        {
            get; set;
        }

        [DataMember(Name = "AQ")]
        public double ActualQuantity
        {
            get; set;
        }

        [DataMember(Name = "AQU")]
        public double ActualQuantityUOM
        {
            get; set;
        }

        [DataMember(Name = "MDU")]
        public MDUnit MDUnit
        {
            get; set;
        }

        [DataMember(Name = "IFM")]
        public bool IsFinalMixure
        {
            get; set;
        }

        [DataMember(Name = "HIM")]
        public bool HasInputMaterials
        {
            get; set;
        }

        [DataMember(Name = "RF")]
        public bool? RetrogradeFIFO
        {
            get; set;
        }

        [DataMember(Name = "BMI")]
        public string BookingMaterialInfo
        {
            get;
            set;
        }
    }
}
