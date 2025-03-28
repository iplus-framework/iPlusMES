using s2industries.ZUGFeRD;

namespace gip.mes.facility
{
    public class EInvoiceCompany
    {
        public string CompanyNo { get; set; }
        public string CompanyName { get; set; }
        public string Postcode { get;set;}
        public string City { get;set;}
        public string Street { get;set;}
        public string VATNumber { get;set;}
        public string PersonName { get;set;}
        public string NoteExternal { get;set;}
        public CountryCodes CountryCode { get;set;}
    }
}
