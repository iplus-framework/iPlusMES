// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System.Collections.Generic;

namespace gip.mes.datamodel
{
    public interface IOutOrderPosBSO: IACComponent
    {
        MDUnit CurrentMDUnit { get; set; }

        void OnPricePropertyChanged();

        List<PriceListMaterial> PriceListMaterialItems { get; set; }
        PriceListMaterial SelectedPriceListMaterial { get; set; }

        List<MDCountrySalesTax> TaxOverviewList{ get; set; }

    }
}
