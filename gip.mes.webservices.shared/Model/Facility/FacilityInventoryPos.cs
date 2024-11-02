// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cFIPOS")]
    public class FacilityInventoryPos : EntityBase
    {
        [DataMember(Name = "ID")]
        public Guid FacilityInventoryPosID
        {
            get; set;
        }

        [DataMember(Name = "FC")]
        public Guid FacilityChargeID
        {
            get; set;
        }

        [DataMember(Name = "SQ")]
        public int Sequence
        {
            get; set;
        }

        [DataMember(Name = "CM")]
        public string Comment
        {
            get; set;
        }

        [DataMember(Name = "LT")]
        public string LotNo
        {
            get; set;
        }

        [DataMember(Name = "pFCNo")]
        public string ParentFacilityNo
        {
            get; set;
        }

        [DataMember(Name = "FCNo")]
        public string FacilityNo
        {
            get; set;
        }

        [DataMember(Name = "FCN")]
        public string FacilityName
        {
            get; set;
        }

        [DataMember(Name = "MNo")]
        public string MaterialNo
        {
            get; set;
        }

        [DataMember(Name = "MN")]
        public string MaterialName
        {
            get; set;
        }

        [DataMember(Name = "cFI")]
        public string FacilityInventoryNo
        {
            get; set;
        }

        [DataMember(Name = "MDFIPosS")]
        public short MDFacilityInventoryPosStateIndex
        {
            get; set;
        }

        public MDFacilityInventoryPosState MDFacilityInventoryPosState
        {
            get; set;
        }


        [DataMember(Name = "StQ")]
        public double StockQuantity
        {
            get; set;
        }


        private double? _NewStockQuantity;
        [DataMember(Name = "NStQ")]
        public double? NewStockQuantity
        {
            get
            {
                return _NewStockQuantity;
            }
            set
            {
                if (_NewStockQuantity != value)
                {
                    SetProperty<double?>(ref _NewStockQuantity, value);
                }
            }
        }

        private bool _NotAvailable;
        [DataMember(Name = "NA")]
        public bool NotAvailable
        {
            get
            {
                return _NotAvailable;
            }
            set
            {
                if (_NotAvailable != value)
                {
                    SetProperty<bool>(ref _NotAvailable, value);
                }
            }
        }

        [DataMember(Name = "UN")]
        public string UpdateName
        {
            get; set;
        }

        [DataMember(Name = "UD")]
        public DateTime UpdateDate
        {
            get; set;
        }

    }
}
