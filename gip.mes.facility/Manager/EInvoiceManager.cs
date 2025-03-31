using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using s2industries.ZUGFeRD;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioSales, "", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]

    public partial class EInvoiceManager : PARole
    {
        #region c´tors

        public EInvoiceManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }


        #endregion

        #region static Methods

        public const string C_DefaultServiceACIdentifier = "EInvoiceManager";

        public static EInvoiceManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<EInvoiceManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<EInvoiceManager> ACRefToServiceInstance(ACComponent requester)
        {
            EInvoiceManager serviceInstance = GetServiceInstance(requester) as EInvoiceManager;
            if (serviceInstance != null)
                return new ACRef<EInvoiceManager>(serviceInstance, requester);
            return null;
        }

        #endregion

        #region Methods


        public Msg SaveEInvoice(DatabaseApp databaseApp, Invoice invoice, string filename)
        {
            Msg msg = null;
            Profile profile = Profile.Basic;
            try
            {

                Company myCompany = databaseApp.Company.Where(c => c.IsOwnCompany).FirstOrDefault();

                if (myCompany == null)
                {
                    // Error50709
                    // EInvoiceManager
                    // Error by exporting invoice {0}! Owner company not configured!
                    // Fehler beim Exportieren der Rechnung {0}! Eigentümerfirma nicht konfiguriert!
                    msg = new Msg(this, eMsgLevel.Exception, nameof(EInvoiceManager), nameof(SaveEInvoice), 150, "Error50709", invoice.InvoiceNo);
                }
                else if (invoice.CustomerCompany == null)
                {
                    // Error50710
                    // EInvoiceManager
                    // Error by exporting invoice {0}! Invoice don't have customer company!
                    // Fehler beim Exportieren der Rechnung {0}! Die Rechnung hat kein Kundenunternehmen!
                    msg = new Msg(this, eMsgLevel.Exception, nameof(EInvoiceManager), nameof(SaveEInvoice), 150, "Error50710", invoice.InvoiceNo);
                }
                else
                {
                    EInvoiceCompany customer = GetEInvoiceCompany(invoice.CustomerCompany);
                    EInvoiceCompany ownCompany = GetEInvoiceCompany(myCompany);

                    CurrencyCodes currency = GetCurrency(invoice.MDCurrency);

                    InvoiceDescriptor desc = InvoiceDescriptor.CreateInvoice(invoice.InvoiceNo, invoice.InvoiceDate, currency);
                    desc.Name = nameof(Invoice);
                    desc.ReferenceOrderNo = invoice.CustomerCompany.CompanyNo;
                    if (!string.IsNullOrEmpty(invoice.Comment))
                    {
                        desc.AddNote(invoice.Comment);
                    }

                    //desc.AddNote("Es bestehen Rabatt- und Bonusvereinbarungen.", SubjectCodes.AAK);
                    //GLN == Company.NoteExternal)
                    desc.SetBuyer(customer.CompanyName, customer.Postcode, customer.City, customer.Street, customer.CountryCode, customer.CompanyNo, new GlobalID(GlobalIDSchemeIdentifiers.GLN, customer.NoteExternal));
                    desc.AddBuyerTaxRegistration(customer.VATNumber, TaxRegistrationSchemeID.VA);
                    desc.SetBuyerOrderReferenceDocument(invoice.CustRequestNo, invoice.InvoiceDate);

                    desc.SetSeller(ownCompany.CompanyName, ownCompany.Postcode, ownCompany.City, ownCompany.Street, ownCompany.CountryCode, ownCompany.CompanyNo, new GlobalID(GlobalIDSchemeIdentifiers.GLN, ownCompany.NoteExternal));

                    //desc.AddSellerTaxRegistration("201/113/40209", TaxRegistrationSchemeID.FC);
                    //desc.AddSellerTaxRegistration("DE123456789", TaxRegistrationSchemeID.VA);

                    DeliveryNote deliveryNote = GetInvoiceDeliveryNote(invoice);
                    if (deliveryNote != null)
                    {
                        desc.SetDeliveryNoteReferenceDocument(deliveryNote.DeliveryNoteNo, deliveryNote.DeliveryDate);
                    }

                    desc.ActualDeliveryDate = invoice.DueDate;
                    EInvoiceTotals invoiceTotals = GetInvoiceTotals(invoice);
                    desc.SetTotals(
                        lineTotalAmount: invoiceTotals.LineTotalAmount,
                        chargeTotalAmount: invoiceTotals.ChargeTotalAmount,
                        allowanceTotalAmount: invoiceTotals.AllowanceTotalAmount,
                        taxBasisAmount: invoiceTotals.TaxBasisAmount,
                        taxTotalAmount: invoiceTotals.TaxTotalAmount,
                        grandTotalAmount: invoiceTotals.GrandTotalAmount,
                        totalPrepaidAmount: invoiceTotals.TotalPrepaidAmount,
                        duePayableAmount: invoiceTotals.DuePayableAmount,
                        roundingAmount: invoiceTotals.RoundingAmount
                    );

                    if (invoice.InvoicePos_Invoice.Any())
                    {
                        AddApplicableTradeTax(desc, invoice);
                    }

                    //desc.AddApplicableTradeTax(64.46m, 19m, 23, TaxTypes.VAT, TaxCategoryCodes.S);
                    //desc.AddLogisticsServiceCharge(5.80m, "Versandkosten", TaxTypes.VAT, TaxCategoryCodes.S, 7m);
                    desc.AddTradePaymentTerms(invoice.Comment, invoice.DueDate);
                    //desc.AddTradePaymentTerms("3% Skonto innerhalb 10 Tagen bis 15.03.2018", new DateTime(2018, 3, 15), PaymentTermsType.Skonto, 30, 3m);

                    SetInvoiceLinies(desc, invoice);


                    FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                    desc.Save(stream, ZUGFeRDVersion.Version23, profile);
                    stream.Flush();
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                // Error50711
                // EInvoiceManager
                // Error by exporting invoice {0}! Message: {1}
                // Fehler beim Exportieren der Rechnung {0}! Meldung: {1}
                msg = new Msg(this, eMsgLevel.Exception, nameof(EInvoiceManager), nameof(SaveEInvoice), 150, "Error50711", invoice.InvoiceNo, ex.Message);
            }
            return msg;
        }

        private void AddApplicableTradeTax(InvoiceDescriptor desc, Invoice invoice)
        {
            var group =
                invoice
                .InvoicePos_Invoice
                .GroupBy(c => c.SalesTax);

            foreach (var item in group)
            {
                decimal priceNet = (decimal)Math.Round(item.Select(c => ((double)c.PriceNet * c.TargetQuantityUOM)).Sum(), 2);
                decimal priceGross = (decimal)Math.Round(item.Select(c => ((double)c.PriceGross * c.TargetQuantityUOM)).Sum(), 2);
                decimal taxAmount = priceGross - priceNet;
                desc.AddApplicableTradeTax(priceNet, item.Key, taxAmount, TaxTypes.VAT, TaxCategoryCodes.S);
            }
        }

        private EInvoiceTotals GetInvoiceTotals(Invoice invoice)
        {
            //[BR-CO-15]-
            //     Invoice total amount with VAT (BT-112 GrandTotalAmount) =
            //             Invoice total amount without VAT (BT-109 TaxBasisAmount)
            //             + Invoice total VAT amount (BT-110 TaxTotalAmount).
            EInvoiceTotals totals = new EInvoiceTotals();
            totals.LineTotalAmount = invoice.PosPriceNetTotal;
            totals.ChargeTotalAmount = 0;
            totals.AllowanceTotalAmount = 0;
            totals.TaxBasisAmount = invoice.PriceNet;
            totals.TaxTotalAmount = invoice.PriceGross - invoice.PriceNet;
            totals.GrandTotalAmount = invoice.PriceGross;
            totals.TotalPrepaidAmount = 0;
            totals.DuePayableAmount = invoice.PriceGross;
            totals.RoundingAmount = 0;
            return totals;
        }

        private void SetInvoiceLinies(InvoiceDescriptor desc, Invoice invoice)
        {
            InvoicePos[] invoicePositions = invoice.InvoicePos_Invoice.OrderBy(c => c.Sequence).ToArray();
            foreach (InvoicePos invoicePos in invoicePositions)
            {
                QuantityCodes quantityCode = GetQuantityCode(invoicePos.MDUnit != null ? invoicePos.MDUnit : invoicePos.Material.BaseMDUnit);
                desc.AddTradeLineItem(
                    lineID: invoicePos.Sequence.ToString("0000"),
                    name: invoicePos.Material.MaterialNo,
                    netUnitPrice: invoicePos.PriceNet,
                    description: invoicePos.Material.MaterialName1,
                    unitCode: quantityCode,
                    unitQuantity: (decimal)invoicePos.TargetQuantity,
                    grossUnitPrice: invoicePos.PriceGross,
                    billedQuantity: (decimal)invoicePos.TargetQuantity,
                    lineTotalAmount: invoicePos.TotalPrice,
                    taxType: invoicePos.SalesTax == 0 ? TaxTypes.FET : TaxTypes.VAT,
                    categoryCode: TaxCategoryCodes.S,
                    taxPercent: invoicePos.SalesTax,
                    comment: invoicePos.Comment,
                    id: null,
                    sellerAssignedID: "",
                    buyerAssignedID: "",
                    deliveryNoteID: "",
                    deliveryNoteDate: null,
                    buyerOrderLineID: "",
                    buyerOrderID: "",
                    buyerOrderDate: null,
                    billingPeriodStart: null,
                    billingPeriodEnd: null);

                //desc.AddApplicableTradeTax(invoicePos.PriceNet, invoicePos.SalesTaxAmount, invoicePos.SalesTax, TaxTypes.VAT, TaxCategoryCodes.S);
            }
        }

        private QuantityCodes GetQuantityCode(MDUnit mDUnit)
        {
            QuantityCodes code = QuantityCodes.KGM;

            if (mDUnit.ISOCode == "STK")
            {
                code = QuantityCodes.XST;
            }
            else
            {
                Enum.TryParse(mDUnit.ISOCode, out code);
            }

            return code;
        }

        private DeliveryNote GetInvoiceDeliveryNote(Invoice invoice)
        {
            DeliveryNote deliveryNote = null;

            if (invoice.OutOrder != null)
            {
                deliveryNote =
                    invoice
                    .OutOrder
                    .OutOrderPos_OutOrder
                    .SelectMany(c => c.DeliveryNotePos_OutOrderPos)
                    .Select(c => c.DeliveryNote)
                    .FirstOrDefault();
            }

            return deliveryNote;
        }

        public EInvoiceCompany GetEInvoiceCompany(Company company)
        {
            EInvoiceCompany model = new EInvoiceCompany();
            CompanyAddress companyAddress = company.HouseCompanyAddress;
            CompanyPerson person = company.CompanyPerson_Company.FirstOrDefault();

            model.CompanyNo = company.CompanyNo;
            model.CompanyName = company.CompanyName;
            model.VATNumber = company.VATNumber;
            model.NoteExternal = company.NoteExternal;

            if (companyAddress != null)
            {
                model.Postcode = companyAddress.Postcode;
                model.City = companyAddress.City;
                model.Street = companyAddress.Street;
                model.CountryCode = GetCountryCode(companyAddress.MDCountry.MDCountryName);
            }

            if (person != null)
            {
                model.PersonName = person.Name1;
            }

            return model;
        }

        private CountryCodes GetCountryCode(string mDKey)
        {
            CountryCodes countryCode = CountryCodes.DE;
            Enum.TryParse(mDKey, out countryCode);
            return countryCode;
        }

        private CurrencyCodes GetCurrency(MDCurrency mDCurrency)
        {
            CurrencyCodes currency = CurrencyCodes.EUR;

            if (mDCurrency.MDKey == "US Dollar")
            {
                currency = CurrencyCodes.USD;
            }

            return currency;
        }

        #endregion
    }
}
