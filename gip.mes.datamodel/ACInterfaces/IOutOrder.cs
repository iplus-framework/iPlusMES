// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿
namespace gip.mes.datamodel
{
    public interface IOutOrder
    {
        Company CustomerCompany { get; set; }
        CompanyAddress DeliveryCompanyAddress { get; set; }
        CompanyAddress BillingCompanyAddress { get; set; }
        MDCurrency MDCurrency { get; set; }

        CompanyAddress IssuerCompanyAddress { get; set; }
        CompanyPerson IssuerCompanyPerson { get; set; }

        decimal PosPriceNetDiscount { get; }
        decimal PosPriceNetSum { get; }
        decimal PosPriceNetTotal { get;  }

        decimal PriceNet { get; set; }
        decimal PriceGross { get; set; }

        void OnEntityPropertyChanged(string property);
    }
}
