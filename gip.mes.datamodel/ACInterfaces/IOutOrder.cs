
namespace gip.mes.datamodel
{
    public interface IOutOrder
    {
        Company CustomerCompany { get; set; }
        CompanyAddress DeliveryCompanyAddress { get; set; }
        CompanyAddress BillingCompanyAddress { get; set; }

        CompanyAddress IssuerCompanyAddress { get; set; }
        CompanyPerson IssuerCompanyPerson { get; set; }

        double PosPriceNetDiscount { get; }
        double PosPriceNetSum { get; }
        double PosPriceNetTotal { get;  }

        decimal PriceNet { get; set; }
        decimal PriceGross { get; set; }

    }
}
