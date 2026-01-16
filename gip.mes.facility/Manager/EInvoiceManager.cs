using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using s2industries.ZUGFeRD;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioSales, "", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public partial class EInvoiceManager : PARole
    {
        #region c´tors

        public EInvoiceManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _UseOldExportVersion = new ACPropertyConfigValue<bool>(this, nameof(UseOldExportVersion), false);

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseACInit = base.ACInit(startChildMode);
            _ = UseOldExportVersion;

            _EInvoiceServiceClient = GetServiceInstance<IEInvoiceServiceClient>(this, "SveRacunServiceClient", CreationBehaviour.OnlyLocal);

            return baseACInit;
        }

        #endregion

        #region Configurations

        private ACPropertyConfigValue<bool> _UseOldExportVersion;
        [ACPropertyConfig("en{'Use old export version'}de{'Alte Exportversion verwenden'}")]
        public bool UseOldExportVersion
        {
            get
            {
                return _UseOldExportVersion.ValueT;
            }
            set
            {
                _UseOldExportVersion.ValueT = value;
            }
        }

        #endregion

        #region Properties

        private IEInvoiceServiceClient _EInvoiceServiceClient = null;
        public IEInvoiceServiceClient EInvoiceServiceClient
        {
            get
            {
                return _EInvoiceServiceClient;
            }
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

        #region Methods -> public

        #region Methods -> EInvoice

        public Msg SaveEInvoice(
            DatabaseApp databaseApp,
            Invoice invoice,
            string filename,
            Profile profile = Profile.Comfort,
            ZUGFeRDFormats zUGFeRDFormats = ZUGFeRDFormats.CII,
            bool isCroatianRegion = true,
            bool sendToService = false)
        {
            if(UseOldExportVersion)
            {
                return SaveEInvoiceOld(databaseApp, invoice, filename, profile, zUGFeRDFormats);
            }

            (Msg msg, InvoiceDescriptor desc, string ownVATNumber) = GetEInvoice(databaseApp, invoice, profile, zUGFeRDFormats, isCroatianRegion);

            if (msg == null || msg.IsSucceded())
            {
                try
                {
                    FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                    desc.Save(stream, ZUGFeRDVersion.Version23, profile, zUGFeRDFormats);
                    stream.Flush();
                    stream.Close();

                    // For Croatian invoices, XML post-processing may be needed to:
                    // 1. Update CustomizationID to "urn:cen.eu:en16931:2017#compliant#urn:mfin.gov.hr:cius-2025:1.0"
                    // 2. Update ProfileID to "P1"
                    // 3. Change EndpointID schemeID from "0088" to "9934"
                    // 4. Add PaymentID element
                    // 5. Add tax names like "HR:PDV25"
                    if (isCroatianRegion)
                    {
                        PostProcessCroatianInvoice(filename, invoice.Comment);
                    }

                    if (sendToService)
                    {
                        if (EInvoiceServiceClient == null || string.IsNullOrEmpty(EInvoiceServiceClient.EInvoiceAPIServiceKey) || string.IsNullOrEmpty(EInvoiceServiceClient.EInvoiceAPIServiceURL))
                        {
                            // Error50718
                            // EInvoiceManager
                            // e-invoice service not configured!
                            // E-Rechnungs-Service nicht konfiguriert!
                            msg = new Msg(this, eMsgLevel.Exception, nameof(EInvoiceManager), nameof(SaveEInvoice), 150, "Error50712", invoice.InvoiceNo);
                        }
                        else
                        {
                            var httpClient = new HttpClient();
                            using (var readStream = File.OpenRead(filename))
                            {
                                Task<IEInvoiceServiceClientResponse> task = EInvoiceServiceClient.SendDocumentAsync(readStream, ownVATNumber);
                                task.Wait();

                                IEInvoiceServiceClientResponse response = task.Result;
                                // Info50107
                                // EInvoiceManager
                                // e-invoice successfully send! Returned ID: {0}.
                                // Die elektronische Rechnung wurde erfolgreich versendet! Rückgabe-ID: {0}.
                                msg = new Msg(this, eMsgLevel.Info, nameof(EInvoiceManager), nameof(SaveEInvoice), 150, "Info50107", response.DocumentId);
                            }
                        }
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

            }
            return msg;
        }

        public Msg SendEInovoiceToService(DatabaseApp databaseApp,
            Invoice invoice)
        {
            return SaveEInvoice(databaseApp, invoice, Path.GetTempFileName(), Profile.XRechnung, ZUGFeRDFormats.UBL, true, true);
        }
        
        public (Msg msg, InvoiceDescriptor desc, string ownVATNumber) GetEInvoice(
            DatabaseApp databaseApp,
            Invoice invoice,
            Profile profile = Profile.Comfort,
            ZUGFeRDFormats zUGFeRDFormats = ZUGFeRDFormats.CII,
            bool isCroatianRegion = true)
        {
            Msg msg = null;
            InvoiceDescriptor desc = null;
            string ownVATNumber = "";
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
                    ownVATNumber = ownCompany.VATNumber;
                    CurrencyCodes currency = GetCurrency(invoice.MDCurrency);

                    desc = GetInvoiceDescriptor(invoice, currency);

                    SetBuyer(isCroatianRegion, invoice, desc, customer, ownCompany);
                    SetSeller(isCroatianRegion, invoice, desc, ownCompany);

                    EInvoiceTotals invoiceTotals = GetInvoiceTotals(invoice);
                    desc.SetTotals(
                        lineTotalAmount: invoiceTotals.LineTotalAmount.HasValue ? invoiceTotals.LineTotalAmount.Value : 0,
                        chargeTotalAmount: invoiceTotals.ChargeTotalAmount,
                        allowanceTotalAmount: invoiceTotals.AllowanceTotalAmount,
                        taxBasisAmount: invoiceTotals.TaxBasisAmount,
                        taxTotalAmount: invoiceTotals.TaxTotalAmount,
                        grandTotalAmount: invoiceTotals.GrandTotalAmount,
                        totalPrepaidAmount: invoiceTotals.TotalPrepaidAmount,
                        duePayableAmount: invoiceTotals.DuePayableAmount,
                        roundingAmount: invoiceTotals.RoundingAmount
                    );


                    desc.AddTradePaymentTerms(invoice.MDTermOfPayment != null ? invoice.MDTermOfPayment.MDTermOfPaymentName : "According due date", invoice.DueDate);


                    // Payment means with region-specific settings
                    if (isCroatianRegion)
                    {
                        // Croatian: Credit Transfer (30) with payment reference
                        string paymentReference = !string.IsNullOrEmpty(invoice.CustRequestNo) ? invoice.CustRequestNo : invoice.InvoiceNo;
                        desc.SetPaymentMeans(
                            PaymentMeansTypeCodes.CreditTransfer,
                            invoice.Comment
                        );
                        desc.PaymentReference = paymentReference;
                        // PaymentID needs to be added through another method or post-processed in XML
                    }
                    else
                    {
                        // German: SEPA Credit Transfer
                        desc.SetPaymentMeans(
                            PaymentMeansTypeCodes.SEPACreditTransfer,
                            myCompany.NoteInternal,
                            myCompany.BillingAccountNo);
                    }

                    desc.AddCreditorFinancialAccount(myCompany.BillingAccountNo, myCompany.NoteInternal);

                    SetInvoiceLinies(desc, invoice);

                    if (invoice.InvoicePos_Invoice.Any())
                    {
                        AddApplicableTradeTax(desc, invoice);
                    }

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
            return (msg, desc, ownVATNumber);
        }

        private InvoiceDescriptor GetInvoiceDescriptor(Invoice invoice, CurrencyCodes currency)
        {
            InvoiceDescriptor desc = InvoiceDescriptor.CreateInvoice(invoice.InvoiceNo, invoice.InvoiceDate, currency);
            desc.Name = nameof(Invoice);
            desc.ReferenceOrderNo = invoice.CustomerCompany.CompanyNo;
            //desc.BusinessProcess = "";
            if (!string.IsNullOrEmpty(invoice.Comment))
            {
                desc.AddNote(invoice.Comment);
            }
            desc.ActualDeliveryDate = invoice.DueDate;
            DeliveryNote deliveryNote = GetInvoiceDeliveryNote(invoice);
            if (deliveryNote != null)
            {
                desc.SetDeliveryNoteReferenceDocument(deliveryNote.DeliveryNoteNo, deliveryNote.DeliveryDate);
            }

            return desc;
        }

        private static void SetBuyer(bool isCroatianRegion, Invoice invoice, InvoiceDescriptor desc, EInvoiceCompany customer, EInvoiceCompany ownCompany)
        {
            if (isCroatianRegion)
            {
                // Croatian: Use OIB with schemeID 9934 (using CompanyNumber identifier)
                string vatNumberCustomized = $"9934:{customer.VATNumber.Replace("HR", "")}";
                desc.SetBuyer(
                    null,
                    customer.Postcode,
                    customer.City,
                    customer.Street,
                    customer.CountryCode,
                    vatNumberCustomized,
                    null, // new GlobalID(GlobalIDSchemeIdentifiers.CompanyNumber, ownCompany.VATNumber)
                    "",
                    new LegalOrganization()
                    {
                        TradingBusinessName = ownCompany.CompanyName,
                        ID = new GlobalID() { ID = customer.VATNumber.Replace("HR", "") }
                    },
                    customer.CompanyName
                );
                desc.SetBuyerElectronicAddress(customer.VATNumber.Replace("HR", ""), ElectronicAddressSchemeIdentifiers.CroatiaVatNumber); // 9934
            }
            else
            {
                // German: Use GLN
                desc.SetBuyer(customer.CompanyName, customer.Postcode, customer.City, customer.Street,
                    customer.CountryCode, customer.CompanyNo,
                    new GlobalID(GlobalIDSchemeIdentifiers.GLN, customer.NoteExternal));
                desc.SetBuyerElectronicAddress(customer.VATNumber, ElectronicAddressSchemeIdentifiers.GermanyVatNumber); // 9930
            }
            desc.AddBuyerTaxRegistration(customer.VATNumber, TaxRegistrationSchemeID.VA);
            desc.SetBuyerOrderReferenceDocument(invoice.CustRequestNo, invoice.InvoiceDate);
        }

        private static void SetSeller(bool isCroatianRegion, Invoice invoice, InvoiceDescriptor desc, EInvoiceCompany ownCompany)
        {
            if (isCroatianRegion)
            {
                // Croatian: Use OIB with schemeID 9934 (using CompanyNumber identifier)
                string vatNumberCustomized = $"9934:{ownCompany.VATNumber.Replace("HR", "")}";
                desc.SetSeller(
                    null,
                    ownCompany.Postcode,
                    ownCompany.City,
                    ownCompany.Street,
                    ownCompany.CountryCode,
                    vatNumberCustomized,
                    null, // new GlobalID(GlobalIDSchemeIdentifiers.CompanyNumber, ownCompany.VATNumber)
                    new LegalOrganization()
                    {
                        TradingBusinessName = ownCompany.CompanyName,
                        ID = new GlobalID() { ID = ownCompany.VATNumber.Replace("HR", "") }
                    },
                    ownCompany.CompanyName
                );
                desc.SetSellerElectronicAddress(ownCompany.VATNumber, ElectronicAddressSchemeIdentifiers.CroatiaVatNumber); // 9934
            }
            else
            {
                // German: Use GLN
                desc.SetSeller(ownCompany.CompanyName, ownCompany.Postcode, ownCompany.City, ownCompany.Street,
                    ownCompany.CountryCode, ownCompany.CompanyNo,
                    new GlobalID(GlobalIDSchemeIdentifiers.GLN, ownCompany.NoteExternal));
                desc.SetSellerElectronicAddress(ownCompany.VATNumber, ElectronicAddressSchemeIdentifiers.GermanyVatNumber); // 9930
            }
            desc.SetSellerContact(string.Format("{0} {1}", invoice.IssuerCompanyPerson?.Name1, invoice.IssuerCompanyPerson?.Name1),
                ownCompany.CompanyName, invoice.IssuerCompanyPerson?.EMail, invoice.IssuerCompanyPerson?.Phone, invoice.IssuerCompanyPerson?.PostOfficeBox);

            desc.AddSellerTaxRegistration(ownCompany.VATNumber, TaxRegistrationSchemeID.VA);
        }

        static void PostProcessCroatianInvoice(string filename, string invoiceComment)
        {
            try
            {
                var xmlDoc = XDocument.Load(filename);

                XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                XNamespace ubl = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";

                // 1. Update CustomizationID to Croatian CIUS-2025
                var customizationID = xmlDoc.Descendants(cbc + "CustomizationID").FirstOrDefault();
                if (customizationID != null)
                {
                    customizationID.Value = "urn:cen.eu:en16931:2017#compliant#urn:mfin.gov.hr:cius-2025:1.0#conformant#urn:mfin.gov.hr:ext-2025:1.0";
                }

                // 2. Update ProfileID to "P1"
                var profileID = xmlDoc.Descendants(cbc + "ProfileID").FirstOrDefault();
                if (profileID != null)
                {
                    profileID.Value = "P1";
                }

                // 3. Add IssueTime after IssueDate
                var issueDate = xmlDoc.Descendants(cbc + "IssueDate").FirstOrDefault();
                if (issueDate != null)
                {
                    var existingIssueTime = xmlDoc.Descendants(cbc + "IssueTime").FirstOrDefault();
                    if (existingIssueTime == null)
                    {
                        issueDate.AddAfterSelf(new XElement(cbc + "IssueTime", "12:00:00"));
                    }
                }

                // 4. Add InstructionNote and PaymentID under PaymentMeans
                //var paymentMeans = xmlDoc.Descendants(cac + "PaymentMeans").FirstOrDefault();
                //if (paymentMeans != null)
                //{
                //    var paymentMeansCode = paymentMeans.Element(cbc + "PaymentMeansCode");

                //    // Add InstructionNote if missing
                //    var existingInstructionNote = paymentMeans.Element(cbc + "InstructionNote");
                //    if (existingInstructionNote == null && paymentMeansCode != null)
                //    {
                //        paymentMeansCode.AddAfterSelf(new XElement(cbc + "InstructionNote", invoiceComment));
                //    }

                //    // Add PaymentID if missing
                //    var existingPaymentID = paymentMeans.Element(cbc + "PaymentID");
                //    if (existingPaymentID == null)
                //    {
                //        var instructionNote = paymentMeans.Element(cbc + "InstructionNote");
                //        var paymentIDElement = new XElement(cbc + "PaymentID", "HR00123456");

                //        if (instructionNote != null)
                //        {
                //            instructionNote.AddAfterSelf(paymentIDElement);
                //        }
                //        else if (paymentMeansCode != null)
                //        {
                //            paymentMeansCode.AddAfterSelf(paymentIDElement);
                //        }
                //    }
                //}

                // 5. Update tax category names to include "HR:PDV" prefix
                var taxCategories = xmlDoc.Descendants(cac + "TaxCategory");
                foreach (var taxCategory in taxCategories)
                {
                    var percent = taxCategory.Element(cbc + "Percent");
                    if (percent != null)
                    {
                        decimal percentValue = 0;
                        if (decimal.TryParse(percent.Value, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out percentValue))
                        {
                            var nameElement = taxCategory.Element(cbc + "Name");
                            string taxName = $"HR:PDV{(int)percentValue}";

                            if (nameElement == null)
                            {
                                var idElement = taxCategory.Element(cbc + "ID");
                                if (idElement != null)
                                {
                                    idElement.AddAfterSelf(new XElement(cbc + "Name", taxName));
                                }
                            }
                            else
                            {
                                nameElement.Value = taxName;
                            }
                        }
                    }
                }

                // 6. Update ClassifiedTaxCategory names in invoice lines
                var classifiedTaxCategories = xmlDoc.Descendants(cac + "ClassifiedTaxCategory");
                foreach (var taxCategory in classifiedTaxCategories)
                {
                    var percent = taxCategory.Element(cbc + "Percent");
                    if (percent != null)
                    {
                        decimal percentValue = 0;
                        if (decimal.TryParse(percent.Value, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out percentValue))
                        {
                            var nameElement = taxCategory.Element(cbc + "Name");
                            string taxName = $"HR:PDV{(int)percentValue}";

                            if (nameElement == null)
                            {
                                var idElement = taxCategory.Element(cbc + "ID");
                                if (idElement != null)
                                {
                                    idElement.AddAfterSelf(new XElement(cbc + "Name", taxName));
                                }
                            }
                            else
                            {
                                nameElement.Value = taxName;
                            }
                        }
                    }
                }

                // 7. Add UBL extensions if not present
                var root = xmlDoc.Root;
                if (root != null)
                {
                    XNamespace ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";
                    XNamespace sig = "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2";
                    XNamespace sac = "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2";

                    var existingExtensions = root.Element(ext + "UBLExtensions");
                    if (existingExtensions == null)
                    {
                        var extensions = new XElement(ext + "UBLExtensions",
                            new XElement(ext + "UBLExtension",
                                new XElement(ext + "ExtensionContent",
                                    new XElement(sig + "UBLDocumentSignatures",
                                        new XElement(sac + "SignatureInformation")
                                    )
                                )
                            )
                        );

                        root.AddFirst(extensions);
                    }
                }

                // 8. Add commodity classification to invoice line
                //var invoiceLines = xmlDoc.Descendants(cac + "InvoiceLine");
                //foreach (var invoiceLine in invoiceLines)
                //{
                //    var item = invoiceLine.Element(cac + "Item");
                //    if (item != null)
                //    {
                //        var existingClassification = item.Element(cac + "CommodityClassification");
                //        if (existingClassification == null)
                //        {
                //            var name = item.Element(cbc + "Name");
                //            if (name != null)
                //            {
                //                var classification = new XElement(cac + "CommodityClassification",
                //                    new XElement(cbc + "ItemClassificationCode",
                //                        new XAttribute("listID", "CG"),
                //                        "62.90.90"
                //                    )
                //                );
                //                name.AddAfterSelf(classification);
                //            }
                //        }
                //    }
                //}

                // 9. Add PartyIdentification for supplier
                //var supplierParty = xmlDoc.Descendants(cac + "AccountingSupplierParty")
                //    .FirstOrDefault()?.Element(cac + "Party");
                //if (supplierParty != null)
                //{
                //    var endpointID = supplierParty.Element(cbc + "EndpointID");
                //    var existingPartyId = supplierParty.Element(cac + "PartyIdentification");
                //    if (existingPartyId == null && endpointID != null)
                //    {
                //        var partyId = new XElement(cac + "PartyIdentification",
                //            new XElement(cbc + "ID", "9934:09180308057")
                //        );
                //        endpointID.AddAfterSelf(partyId);
                //    }
                //}

                // 10. Add SellerContact section after Contact
                //var accountingSupplierParty = xmlDoc.Descendants(cac + "AccountingSupplierParty").FirstOrDefault();
                //if (accountingSupplierParty != null)
                //{
                //    var existingSellerContact = accountingSupplierParty.Element(cac + "SellerContact");
                //    if (existingSellerContact == null)
                //    {
                //        var sellerContact = new XElement(cac + "SellerContact",
                //            new XElement(cbc + "ID", "09180308057"),
                //            new XElement(cbc + "Name", "gipSoft d.o.o.")
                //        );
                //        accountingSupplierParty.Add(sellerContact);
                //    }
                //}

                // 11. Add CompanyID to PartyLegalEntity for both parties
                //var legalEntities = xmlDoc.Descendants(cac + "PartyLegalEntity");
                //foreach (var legalEntity in legalEntities)
                //{
                //    var existingCompanyID = legalEntity.Element(cbc + "CompanyID");
                //    if (existingCompanyID == null)
                //    {
                //        var registrationName = legalEntity.Element(cbc + "RegistrationName");
                //        if (registrationName != null)
                //        {
                //            // Check if this is supplier or customer based on the ancestor
                //            var isSupplier = legalEntity.Ancestors(cac + "AccountingSupplierParty").Any();
                //            var companyID = isSupplier ? "09180308057" : "58203211592";
                //            registrationName.AddAfterSelf(new XElement(cbc + "CompanyID", companyID));
                //        }
                //    }
                //}

                // 12. Remove PaymentTerms element (not in target)
                //var paymentTerms = xmlDoc.Descendants(cac + "PaymentTerms").FirstOrDefault();
                //paymentTerms?.Remove();

                // 12b. Remove CountrySubentity (not in target)
                //var countrySubentities = xmlDoc.Descendants(cbc + "CountrySubentity");
                //foreach (var subentity in countrySubentities.ToList())
                //{
                //    subentity.Remove();
                //}

                // 13. Remove PartyName elements (not in target for supplier)
                //var supplierPartyName = xmlDoc.Descendants(cac + "AccountingSupplierParty")
                //    .FirstOrDefault()?.Element(cac + "Party")?.Element(cac + "PartyName");
                //supplierPartyName?.Remove();

                //var customerPartyName = xmlDoc.Descendants(cac + "AccountingCustomerParty")
                //    .FirstOrDefault()?.Element(cac + "Party")?.Element(cac + "PartyName");
                //customerPartyName?.Remove();

                // 14. Remove Name from Contact (keep only ElectronicMail)
                //var contacts = xmlDoc.Descendants(cac + "Contact");
                //foreach (var contact in contacts)
                //{
                //    var name = contact.Element(cbc + "Name");
                //    name?.Remove();
                //}

                // 15. Fix CompanyLegalForm to contain full legal description
                //var supplierLegalEntity = xmlDoc.Descendants(cac + "AccountingSupplierParty")
                //    .FirstOrDefault()?.Descendants(cac + "PartyLegalEntity").FirstOrDefault();
                //if (supplierLegalEntity != null)
                //{
                //    var legalForm = supplierLegalEntity.Element(cbc + "CompanyLegalForm");
                //    if (legalForm != null)
                //    {
                //        legalForm.Value = "gipSoft d.o.o. Društvo sa ograničenom odgovornošću gipSoft d.o.o. registrirano je na adresi Repovec 25B, 49210, Repovec, Hrvatska i posluje od 22.08.2014.. Kontakt telefon je 098378286 i kontakt email damir.lisak@iplus-framework.com. Trenutni direktor tvrtke je Damir Lisak.";
                //    }
                //}

                //var customerLegalEntity = xmlDoc.Descendants(cac + "AccountingCustomerParty")
                //    .FirstOrDefault()?.Descendants(cac + "PartyLegalEntity").FirstOrDefault();
                //if (customerLegalEntity != null)
                //{
                //    var legalForm = customerLegalEntity.Element(cbc + "CompanyLegalForm");
                //    if (legalForm != null)
                //    {
                //        legalForm.Value = "PAN-PEK d.o.o. Društvo sa ograničenom odgovornošću PAN-PEK, d.o.o. registrirano je na adresi Planinska ulica 2C, 10000, Zagreb, Hrvatska i posluje od 02.07.1992.. Kontakt telefon je 012390500 i kontakt email info@panpek.hr. Trenutni direktor tvrtke je Sandra Vojković.";
                //    }
                //    else
                //    {
                //        // Add CompanyLegalForm if missing
                //        var companyID = customerLegalEntity.Element(cbc + "CompanyID");
                //        if (companyID != null)
                //        {
                //            companyID.AddAfterSelf(new XElement(cbc + "CompanyLegalForm",
                //                "PAN-PEK d.o.o. Društvo sa ograničenom odgovornošću PAN-PEK, d.o.o. registrirano je na adresi Planinska ulica 2C, 10000, Zagreb, Hrvatska i posluje od 02.07.1992.. Kontakt telefon je 012390500 i kontakt email info@panpek.hr. Trenutni direktor tvrtke je Sandra Vojković."));
                //        }
                //    }
                //}

                // 16. Remove unwanted monetary total elements
                //var legalMonetaryTotal = xmlDoc.Descendants(cac + "LegalMonetaryTotal").FirstOrDefault();
                //if (legalMonetaryTotal != null)
                //{
                //    legalMonetaryTotal.Element(cbc + "AllowanceTotalAmount")?.Remove();
                //    legalMonetaryTotal.Element(cbc + "ChargeTotalAmount")?.Remove();
                //    legalMonetaryTotal.Element(cbc + "PrepaidAmount")?.Remove();
                //}

                // 17. Fix quantity format in invoice line and change unit code to H87
                //var invoicedQuantities = xmlDoc.Descendants(cbc + "InvoicedQuantity");
                //foreach (var qty in invoicedQuantities)
                //{
                //    // Change unit code to H87
                //    var unitCodeAttr = qty.Attribute("unitCode");
                //    if (unitCodeAttr != null)
                //    {
                //        unitCodeAttr.Value = "H87";
                //    }

                //    // Fix format to 1.000
                //    if (qty.Value.Contains("."))
                //    {
                //        decimal value;
                //        if (decimal.TryParse(qty.Value, System.Globalization.NumberStyles.Any,
                //            System.Globalization.CultureInfo.InvariantCulture, out value))
                //        {
                //            qty.Value = value.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
                //        }
                //    }
                //}

                // 18. Fix BaseQuantity format and unit code
                //var baseQuantities = xmlDoc.Descendants(cbc + "BaseQuantity");
                //foreach (var qty in baseQuantities)
                //{
                //    // Change unit code to H87
                //    var unitCodeAttr = qty.Attribute("unitCode");
                //    if (unitCodeAttr != null)
                //    {
                //        unitCodeAttr.Value = "H87";
                //    }

                //    // Fix format to 1.000
                //    if (qty.Value.Contains("."))
                //    {
                //        decimal value;
                //        if (decimal.TryParse(qty.Value, System.Globalization.NumberStyles.Any,
                //            System.Globalization.CultureInfo.InvariantCulture, out value))
                //        {
                //            qty.Value = value.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
                //        }
                //    }
                //}

                xmlDoc.Save(filename);
                Console.WriteLine($"Croatian invoice post-processed successfully: {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error post-processing Croatian invoice: {ex.Message}");
                throw;
            }
        }


        #endregion

        #region Methods -> SaveEInvoice old
        public Msg SaveEInvoiceOld(DatabaseApp databaseApp, Invoice invoice, string filename, Profile profile = Profile.Comfort, ZUGFeRDFormats zUGFeRDFormats = ZUGFeRDFormats.CII)
        {
            Msg msg = null;
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
                    //desc.BusinessProcess = "";
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
                    desc.SetSellerContact(string.Format("{0} {1}", invoice.IssuerCompanyPerson?.Name1, invoice.IssuerCompanyPerson?.Name1), ownCompany.CompanyName, invoice.IssuerCompanyPerson?.EMail, invoice.IssuerCompanyPerson?.Phone, invoice.IssuerCompanyPerson?.PostOfficeBox);

                    desc.AddSellerTaxRegistration(ownCompany.VATNumber, TaxRegistrationSchemeID.VA);
                    //desc.AddSellerTaxRegistration("DE123456789", TaxRegistrationSchemeID.VA);

                    DeliveryNote deliveryNote = GetInvoiceDeliveryNote(invoice);
                    if (deliveryNote != null)
                    {
                        desc.SetDeliveryNoteReferenceDocument(deliveryNote.DeliveryNoteNo, deliveryNote.DeliveryDate);
                    }

                    desc.ActualDeliveryDate = invoice.DueDate;
                    EInvoiceTotals invoiceTotals = GetInvoiceTotals(invoice);
                    desc.SetTotals(
                        lineTotalAmount: invoiceTotals.LineTotalAmount.HasValue ? invoiceTotals.LineTotalAmount.Value : 0,
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
                    desc.AddTradePaymentTerms(invoice.MDTermOfPayment != null ? invoice.MDTermOfPayment.MDTermOfPaymentName : "According due date", invoice.DueDate);
                    desc.SetPaymentMeans(PaymentMeansTypeCodes.SEPACreditTransfer, myCompany.NoteInternal, myCompany.BillingAccountNo);
                    desc.AddCreditorFinancialAccount(myCompany.BillingAccountNo, myCompany.NoteInternal);
                    //desc.AddTradePaymentTerms("3% Skonto innerhalb 10 Tagen bis 15.03.2018", new DateTime(2018, 3, 15), PaymentTermsType.Skonto, 30, 3m);

                    SetInvoiceLinies(desc, invoice);


                    FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                    desc.Save(stream, ZUGFeRDVersion.Version23, profile, zUGFeRDFormats);
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

        #endregion

        #endregion

        #region Methods -> private

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
                if (invoice.IsReverseCharge)
                    desc.AddApplicableTradeTax(priceNet, item.Key, taxAmount, TaxTypes.VAT, TaxCategoryCodes.AE, exemptionReasonCode: TaxExemptionReasonCodes.VATEX_EU_AE);
                else
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
                TradeLineItem tlItem = desc.AddTradeLineItem(
                    lineID: invoicePos.Sequence.ToString("0000"),
                    name: invoicePos.Material.MaterialNo,
                    netUnitPrice: invoicePos.PriceNet,
                    description: invoicePos.Material.MaterialName1,
                    unitCode: quantityCode,
                    unitQuantity: (decimal)invoicePos.TargetQuantity,
                    grossUnitPrice: invoicePos.PriceGross,
                    billedQuantity: (decimal)invoicePos.TargetQuantity,
                    lineTotalAmount: invoicePos.TotalPrice,
                    taxType: TaxTypes.VAT,
                    categoryCode: invoice.IsReverseCharge ? TaxCategoryCodes.AE : TaxCategoryCodes.S,
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
                if (tlItem != null && invoicePos.Material.MDMaterialType != null)
                    tlItem.AddDesignatedProductClassification(DesignatedProductClassificationClassCodes.ZZZ, null, invoicePos.Material.MDMaterialType.MDKey, invoicePos.Material.MDMaterialType.MDMaterialTypeName);

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

        #endregion
    }
}
