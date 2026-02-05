using gip.core.datamodel;
using s2industries.ZUGFeRD;

namespace gip.mes.facility
{
    /// <summary>
    /// BT-23: (Business Process Type)
    /// cbc:ProfileID
    /// urn:fdc:peppol.eu:2017:poacc:billing:01:1.0
    /// List of business process types for Croatian e-invoicing, used in the "Bussines Process Type" field of the e-invoice.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Bussines Process Type'}de{'Batch-Vorschlagsmodus'}", Global.ACKinds.TACEnumACValueList)]
    public class EInvoiceBussinesProcessTypeValueList : ACValueItemList
    {
        public EInvoiceBussinesProcessTypeValueList() : base("BussinesProcessType")
        {
            AddEntry(BusinessProcessType.OrderBasedOnContract, "en{'Invoicing of deliveries of goods and services through orders based on contracts'}de{'Rechnungsstellung für Warenlieferungen und Dienstleistungen auf Grundlage von Bestellungen und Verträgen'}hr{'Fakturiranje isporuka dobara i usluga preko narudžbi na temelju ugovora'}");
            AddEntry(BusinessProcessType.PeriodicContractBilling, "en{'Periodic invoicing of deliveries based on contracts'}de{'Periodische Rechnungsstellung für Lieferungen auf Grundlage von Verträgen'}hr{'Periodično fakturiranje isporuka na temelju ugovora'}");
            AddEntry(BusinessProcessType.UnplannedOrderBilling, "en{'Invoicing of deliveries through unplanned orders'}de{'Rechnungsstellung für Lieferungen über ungeplante Bestellungen'}hr{'Fakturiranje isporuka preko nepredviđene narudžbe'}");
            AddEntry(BusinessProcessType.AdvancePayment, "en{'Advance payment'}de{'Vorauszahlung (Anzahlung)'}hr{'Plaćanje predujma (avansno plaćanje)'}");
            AddEntry(BusinessProcessType.OnSitePayment, "en{'Payment on site'}de{'Zahlung vor Ort'}hr{'Plaćanje na licu mjesta'}");
            AddEntry(BusinessProcessType.PreDeliveryPaymentBasedOnOrder, "en{'Payment before delivery based on order'}de{'Zahlung vor Lieferung auf Grundlage einer Bestellung'}hr{'Plaćanje prije isporuke na temelju narudžbe'}");
            AddEntry(BusinessProcessType.InvoiceWithDispatchAdviceReference, "en{'Invoices with reference to dispatch advice'}de{'Rechnungen mit Bezug auf Lieferschein'}hr{'Računi s referencom na otpremnicu'}");
            AddEntry(BusinessProcessType.InvoiceWithDispatchAdviceAndReceiptReference, "en{'Invoices with reference to dispatch advice and receipt'}de{'Rechnungen mit Bezug auf Lieferschein und Empfangsbestätigung'}hr{'Računi s referencom na otpremnicu i primku'}");
            AddEntry(BusinessProcessType.CreditNoteOrNegativeInvoicing, "en{'Credit note or negative invoicing'}de{'Gutschrift oder Negativ-Rechnungsstellung'}hr{'Odobrenje ili negativno fakturiranje'}");
            AddEntry(BusinessProcessType.CorrectiveInvoicing, "en{'Corrective invoicing'}de{'Korrekturrechnungsstellung'}hr{'Korektivno fakturiranje'}");
            AddEntry(BusinessProcessType.PartialAndFinalInvoicing, "en{'Partial and final invoicing'}de{'Teilweise und endgültige Rechnungsstellung'}hr{'Parcijalno i završno fakturiranje'}");
            AddEntry(BusinessProcessType.SelfBilling, "en{'Self-billing'}de{'Selbstfakturierung'}hr{'Samoizdavanje računa'}");
            AddEntry(BusinessProcessType.CustomerDefinedBusinessProcess, "en{'Customer-defined business process'}de{'Kundendefinierter Geschäftsprozess'}hr{'Poslovni proces koji definira kupac'}");
        }
    }

}
