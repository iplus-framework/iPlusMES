// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cPOPLPosRel")]
    public class ProdOrderPartslistPosRelation : EntityBase
    {
        [DataMember(Name = "ID")]
        public Guid ProdOrderPartslistPosRelationID
        {
            get;set;
        }


        [DataMember(Name = "SPos")]
        public ProdOrderPartslistPos SourcePos
        {
            get;set;
        }

        [DataMember(Name = "TPos")]
        public ProdOrderPartslistPos TargetPos
        {
            get;set;
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

        [DataMember(Name = "RF")]
        public bool? RetrogradeFIFO
        {
            get; set;
        }

        public double CompleteFactor
        {
            get => (ActualQuantity / TargetQuantity) * 100;
        }

        public bool IsRetrograde
        {
            get 
            {
                if (RetrogradeFIFO.HasValue)
                    return RetrogradeFIFO.Value;

                if (SourcePos != null && SourcePos.RetrogradeFIFO.HasValue)
                    return SourcePos.RetrogradeFIFO.Value;

                if (SourcePos != null && SourcePos.Material != null && SourcePos.Material.RetrogradeFIFO.HasValue)
                    return SourcePos.Material.RetrogradeFIFO.Value;

                return false;
            }
        }
    }
}
