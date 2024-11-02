// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace gip.mes.webservices
{
    public class PickingMaterial : EntityBase
    {
        public Material Material
        {
            get;
            set;
        }

        public IEnumerable<PickingPos> PickingItems
        {
            get;
            set;
        }

        public double TotalQuantity
        {
            get;
            set;
        }

        public double ActualQuantity
        {
            get;
            set;
        }

        public MDUnit MDUnit
        {
            get;
            set;
        }

        public double CompleteFactor
        {
            get => (ActualQuantity / TotalQuantity) * 100;
        }

        public void RecalculateActualQuantity()
        {
            if (PickingItems != null)
            {
                ActualQuantity = PickingItems.Sum(c => c.ActualQuantityUOM);
                OnPropertyChanged(nameof(ActualQuantity));
                OnPropertyChanged(nameof(CompleteFactor));
            }
        }

        
    }
}
