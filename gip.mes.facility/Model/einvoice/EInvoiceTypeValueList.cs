using gip.core.datamodel;
using s2industries.ZUGFeRD;

namespace gip.mes.facility
{
    /// <summary>
    /// BT-3: Invoice Type (Invoice Type Code)
    /// cbc:InvoiceTypeCode
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Batch suggestion mode'}de{'Batch-Vorschlagsmodus'}", Global.ACKinds.TACEnumACValueList)]
    public class EInvoiceTypeValueList : ACValueItemList
    {
        public EInvoiceTypeValueList() : base("InvoiceType")
        {
            AddEntry(InvoiceType.CreditNoteRelatedToGoodsOrServices, "en{'Credit note related to goods or services'}de{'Gutschrift für Waren und Dienstleistungen'}hr{'Odobrenje za robu i usluge'}");
            AddEntry(InvoiceType.MeteredServicesInvoice, "en{'Metered services invoice'}de{'Rechnung für gemessene Dienstleistungen'}hr{'Račun za mjerene usluge'}");
            AddEntry(InvoiceType.CreditNoteRelatedToFinancialAdjustments, "en{'Credit note related to financial adjustments'}de{'Gutschrift für finanzielle Anpassungen'}hr{'Odobrenje za financijske ispravke'}");
            AddEntry(InvoiceType.InvoicingDataSheet, "en{'Invoicing data sheet'}de{'Rechnungsdatenblatt'}hr{'Interni račun s podacima za fakturiranje'}");
            AddEntry(InvoiceType.SelfBilledCreditNote, "en{'Self billed credit note'}de{'Selbst ausgestellte Gutschrift'}hr{'Samoizdavajuće odobrenje'}");
            AddEntry(InvoiceType.ProformaInvoice, "en{'Proforma invoice'}de{'Proforma-Rechnung'}hr{'Predračun'}");
            AddEntry(InvoiceType.PartialInvoice, "en{'Partial invoice'}de{'Teilrechnung'}hr{'Parcijalni račun'}");
            AddEntry(InvoiceType.CommercialInvoiceWithPackingList, "en{'Commercial invoice which includes a packing list'}de{'Handelsrechnung mit Packliste'}hr{'Komercijalna faktura koja uključuje popis za pakiranje'}");
            AddEntry(InvoiceType.Invoice, "en{'Commercial invoice'}de{'Handelsrechnung'}hr{'Komercijalni račun'}");
            AddEntry(InvoiceType.CreditNote, "en{'Credit note'}de{'Gutschrift'}hr{'Odobrenje'}");
            AddEntry(InvoiceType.DebitNote, "en{'Debit note'}de{'Belastungsanzeige'}hr{'Terećenje'}");
            AddEntry(InvoiceType.Correction, "en{'Corrected invoice'}de{'Korrekturrechnung'}hr{'Korektivni račun'}");
            AddEntry(InvoiceType.ConsolidatedInvoice, "en{'Consolidated invoice'}de{'Sammelrechnung'}hr{'Konsolidirani račun'}");
            AddEntry(InvoiceType.PrepaymentInvoice, "en{'Prepayment invoice'}de{'Vorauszahlungsrechnung'}hr{'Račun za predujam'}");
            AddEntry(InvoiceType.HireInvoice, "en{'Hire invoice'}de{'Mietrechnung'}hr{'Račun za najam'}");
            AddEntry(InvoiceType.TaxInvoice, "en{'Tax invoice'}de{'Steuerrechnung'}hr{'Porezni račun'}");
            AddEntry(InvoiceType.SelfBilledInvoice, "en{'Self-billed invoice'}de{'Selbst ausgestellte Rechnung'}hr{'Samoizdavajući račun'}");
            AddEntry(InvoiceType.DelcredereInvoice, "en{'Delcredere invoice'}de{'Delkredere-Rechnung'}hr{'Delkredere račun'}");
            AddEntry(InvoiceType.FactoredInvoice, "en{'Factored invoice'}de{'Factoring-Rechnung'}hr{'Faktoring račun'}");
            AddEntry(InvoiceType.LeaseInvoice, "en{'Lease invoice'}de{'Leasingrechnung'}hr{'Račun za leasing'}");
            AddEntry(InvoiceType.ConsignmentInvoice, "en{'Consignment invoice'}de{'Konsignationsrechnung'}hr{'Konsignacijski račun'}");
            AddEntry(InvoiceType.FactoredCreditNote, "en{'Factored credit note'}de{'Factoring-Gutschrift'}hr{'Faktoring odobrenje'}");
            AddEntry(InvoiceType.ReversalOfDebit, "en{'Reversal of debit'}de{'Stornierung der Belastung'}hr{'Povrat terećenja'}");
            AddEntry(InvoiceType.ReversalOfCredit, "en{'Reversal of credit'}de{'Stornierung der Gutschrift'}hr{'Storno kredita (banka)'}");
            AddEntry(InvoiceType.SelfBilledDebitNote, "en{'Self billed debit note'}de{'Selbst ausgestellte Belastungsanzeige'}hr{'Samoizdavajuće terećenje'}");
            AddEntry(InvoiceType.InsurersInvoice, "en{'Insurer\\'s invoice'}de{'Versichererrechnung'}hr{'Račun osiguravatelja'}");
            AddEntry(InvoiceType.ForwardersInvoice, "en{'Forwarder\\'s invoice'}de{'Spediteurrechnung'}hr{'Špediterski račun'}");
            AddEntry(InvoiceType.FreightInvoice, "en{'Freight invoice'}de{'Frachtrechnung'}hr{'Račun za prijevoz tereta'}");
            AddEntry(InvoiceType.ConsularInvoice, "en{'Consular invoice'}de{'Konsularrechnung'}hr{'Konzularna faktura'}");
            AddEntry(InvoiceType.PartialConstructionInvoice, "en{'Partial construction invoice'}de{'Teilbaurechnung'}hr{'Djelomična građevinska faktura'}");
            AddEntry(InvoiceType.PartialFinalConstructionInvoice, "en{'Partial final construction invoice'}de{'Teil-Schlussbaurechnung'}hr{'Djelomična završna građevinska faktura'}");
            AddEntry(InvoiceType.FinalConstructionInvoice, "en{'Final construction invoice'}de{'Schlussbaurechnung'}hr{'Završna građevinska faktura'}");
            AddEntry(InvoiceType.CustomsInvoice, "en{'Customs invoice'}de{'Zollrechnung'}hr{'Carinski račun'}");
        }
    }

}
