using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    public interface IOutOrder
    {
        Company CustomerCompany { get; set; }
        CompanyAddress DeliveryCompanyAddress { get; set; }
        CompanyAddress BillingCompanyAddress { get; set; }

        CompanyAddress IssuerCompanyAddress{get; set;}
        CompanyPerson IssuerCompanyPerson{get; set;}
    }
}
