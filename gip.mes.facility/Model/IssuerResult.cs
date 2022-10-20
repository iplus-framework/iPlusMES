using gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public class IssuerResult
    {
        public string IssuerMessage { get; set; }

        public List<CompanyPerson> CompanyPeople{get;set;}
        
        public CompanyAddress IssuerCompanyAddress{get;set;}
        public CompanyPerson IssuerCompanyPerson{get;set;}
    }
}
