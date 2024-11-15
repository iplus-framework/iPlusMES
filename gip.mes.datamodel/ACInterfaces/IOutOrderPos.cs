// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿namespace gip.mes.datamodel
{
    public interface IOutOrderPos
    {
        MDCountrySalesTax MDCountrySalesTax { get; set; }
        MDCountrySalesTaxMDMaterialGroup MDCountrySalesTaxMDMaterialGroup { get; set; }
        MDCountrySalesTaxMaterial MDCountrySalesTaxMaterial { get; set; }
        int Sequence { get; set; }
        Material Material { get; set; }
        MDUnit MDUnit { get; set; }
        double TargetQuantityUOM { get; set; }
        double TargetQuantity { get; set; }
        decimal PriceNet { get; set; }
        decimal PriceGross { get; set; }
        decimal SalesTax { get; set; }

        void OnEntityPropertyChanged(string property);

        decimal SalesTaxAmount { get; }

        decimal TotalSalesTax { get; }

        bool InRecalculation { get; set; }

    }
}
