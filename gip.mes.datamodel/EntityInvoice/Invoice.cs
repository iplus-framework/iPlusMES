using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESInvoice, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOInvoice")]
    [ACPropertyEntity(1, MDInvoiceType.ClassName, ConstApp.ESInvoiceType, Const.ContextDatabase + "\\" + MDInvoiceType.ClassName, "", true)]
    [ACPropertyEntity(2, MDInvoiceState.ClassName, ConstApp.ESInvoiceState, Const.ContextDatabase + "\\" + MDInvoiceState.ClassName, "", true)]
    [ACPropertyEntity(3, nameof(Invoice.InvoiceNo), "en{'Invoice No.'}de{'Rechnungsnummer'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(4, nameof(Invoice.InvoiceDate), "en{'Invoice Date'}de{'Rechnungsdatum'}", "", "", true)]
    [ACPropertyEntity(5, nameof(Invoice.CustomerCompany), ConstApp.Company, Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(6, nameof(Invoice.CustRequestNo), "en{'Extern Invoice No.'}de{'Externe Rechnungsnummer'}", "", "", true)]
    [ACPropertyEntity(7, nameof(Invoice.DeliveryCompanyAddress), "en{'Delivery Address'}de{'Lieferadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(8, nameof(Invoice.BillingCompanyAddress), "en{'Billing Address'}de{'Rechnungsadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(9, OutOrder.ClassName, "en{'Sales Order'}de{'Kundenauftrag'}", Const.ContextDatabase + "\\" + OutOrder.ClassName, "", true)]
    //[ACPropertyEntity(10, MDCountrySalesTax.ClassName, ConstApp.ESCountrySalesTax, Const.ContextDatabase + "\\" + MDCountrySalesTax.ClassName, "", true)]
    [ACPropertyEntity(11, ConstApp.IssuerCompanyAddress, ConstApp.IssuerCompanyAddress_ACCaption, Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(12, ConstApp.IssuerCompanyPerson, ConstApp.IssuerCompanyPerson_ACCaption, Const.ContextDatabase + "\\" + CompanyPerson.ClassName, "", true)]
    [ACPropertyEntity(13, nameof(Invoice.Comment), ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(14, nameof(Invoice.PriceNet), ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(15, nameof(Invoice.PriceGross), ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(16, MDTermOfPayment.ClassName, ConstApp.TermsOfPayment, Const.ContextDatabase + "\\" + MDTermOfPayment.ClassName, "", true)]
    [ACPropertyEntity(20, MDCurrency.ClassName, "en{'Currency'}de{'Währung'}", Const.ContextDatabase + "\\" + MDCurrency.ClassName, "", true)]
    [ACPropertyEntity(21, MDCurrencyExchange.ClassName, "en{'Currency-Exchange'}de{'Währungsumrechnung'}", Const.ContextDatabase + "\\" + MDCurrencyExchange.ClassName, "", true)]
    [ACPropertyEntity(22, nameof(Invoice.Invoice1_ReferenceInvoice), "en{'Reference Invoice'}de{'Referenzrechnung'}", Const.ContextDatabase + "\\" + Invoice.ClassName, "", true)]
    [ACPropertyEntity(23, nameof(Invoice.EInvoiceType), ConstApp.EInvoiceType, Const.ContextDatabase + "\\" + Invoice.ClassName, "", true)]
    [ACPropertyEntity(24, nameof(Invoice.EInvoiceBusinessProcessType), ConstApp.EInvoiceBusinessProcessType, Const.ContextDatabase + "\\" + Invoice.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACPropertyEntity(9999, nameof(Invoice.XMLDesignStart), "en{'Design'}de{'Design'}")]
    [ACPropertyEntity(9999, nameof(Invoice.XMLDesignEnd), "en{'Design'}de{'Design'}")]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + Invoice.ClassName, ConstApp.ESInvoice, typeof(Invoice), Invoice.ClassName, "InvoiceNo", "InvoiceNo", new object[]
        {
                new object[] {Const.QueryPrefix +  InvoicePos.ClassName, ConstApp.ESInvoicePos, typeof(InvoicePos), InvoicePos.ClassName + "_" + InvoicePos.ClassName, "Sequence", "Sequence", new object[]
                    {

                    }
                }
        })
    ]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<Invoice>) })]
    public partial class Invoice : IOutOrder
    {
        public const string ClassName = "Invoice";
        public const string NoColumnName = "InvoiceNo";
        public const string FormatNewNo = "O{0}";

        public const string Const_XMLDesign = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FlowDocument PageWidth=\"816\" PageHeight=\"1056\" PagePadding=\"96,96,96,96\" AllowDrop=\"True\" NumberSubstitution.CultureSource=\"User\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"></FlowDocument>";

        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(11020);

        #region New/Delete
        public static Invoice NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            Invoice entity = new Invoice();
            entity.InvoiceID = Guid.NewGuid();
            entity.DefaultValuesACObject();

            entity.MDInvoiceType = MDInvoiceType.DefaultMDInvoiceType(dbApp);
            entity.MDInvoiceState = MDInvoiceState.DefaultMDInvoiceState(dbApp);

            entity.IssuerCompanyAddress = null;
            entity.IssuerCompanyPerson = null;

            entity.InvoiceNo = secondaryKey;
            entity.InvoiceDate = DateTime.Now;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);

            entity.XMLDesignStart = Const_XMLDesign;
            entity.XMLDesignEnd = Const_XMLDesign;
            entity.MDCurrencyExchangeID = null;
            entity.MDCurrency = MDCurrency.DefaultMDCurrency(dbApp);

            return entity;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            return ACCaption;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                return InvoiceNo;
            }
        }

        /// <summary>
        /// Returns a related EF-Object which is in a Child-Relationship to this.
        /// </summary>
        /// <param name="className">Classname of the Table/EF-Object</param>
        /// <param name="filterValues">Search-Parameters</param>
        /// <returns>A Entity-Object as IACObject</returns>
        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && className == InvoicePos.ClassName)
            {
                Int32 sequence = 0;
                if (Int32.TryParse(filterValues[0], out sequence))
                    return this.InvoicePos_Invoice.Where(c => c.Sequence == sequence).FirstOrDefault();
            }
            return null;
        }

        #endregion

        #region IACObjectEntity Members
        /// <summary>
        /// Method for validating values and references in this EF-Object.
        /// Is called from Change-Tracking before changes will be saved for new unsaved entity-objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">Entity-Framework databasecontext</param>
        /// <returns>NULL if sucessful otherwise a Message-List</returns>
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            if (string.IsNullOrEmpty(InvoiceNo))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "Key",
                    Message = "Key",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "Key"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

        static public string KeyACIdentifier
        {
            get
            {
                return "InvoiceNo";
            }
        }


        #endregion

        #region IEntityProperty Members

        bool bRefreshConfig = false;
        partial void OnXMLConfigChanging(global::System.String value)
        {
            bRefreshConfig = false;
            if (this.EntityState != System.Data.EntityState.Detached && (!(String.IsNullOrEmpty(value) && String.IsNullOrEmpty(XMLConfig)) && value != XMLConfig))
                bRefreshConfig = true;
        }

        partial void OnXMLConfigChanged()
        {
            if (bRefreshConfig)
                ACProperties.Refresh();
        }

        #endregion

        #region Additional
        public bool IsReverseCharge
        {
            get
            {
                return !InvoicePos_Invoice.Where(c => c.SalesTax > 0).Any();
            }
        }

        [ACPropertyInfo(31, "", "en{'Net total'}de{'Netto Gesamt'}")]
        public decimal PosPriceNetTotal
        {
            get
            {
                return PosPriceNetSum + PosPriceNetDiscount;
            }
        }

        [ACPropertyInfo(32, "", "en{'Discount'}de{'Rabatt'}")]
        public decimal PosPriceNetDiscount
        {
            get
            {
                if (InvoicePos_Invoice != null && InvoicePos_Invoice.Any())
                {
                    return InvoicePos_Invoice.Where(c => c.PriceNet < 0 && c.Sequence >= 1000).Sum(o => o.PriceNet);
                }
                return 0;
            }
        }

        [ACPropertyInfo(33, "", "en{'Net'}de{'Netto'}")]
        public decimal PosPriceNetSum
        {
            get
            {
                if (InvoicePos_Invoice != null && InvoicePos_Invoice.Any())
                {
                    return InvoicePos_Invoice.Where(c => c.Sequence < 1000).Sum(o => o.TotalPrice);
                }
                return 0;
            }
        }

        [ACPropertyInfo(34, "", "en{'Net in f. currency'}de{'Netto in Fremdwährung'}")]
        public decimal? ForeginPosPriceNetSum
        {
            get
            {
                if (MDCurrencyExchange == null)
                    return null;
                return MDCurrencyExchange.ConvertToForeignCurrency(PosPriceNetSum);
            }
        }

        [ACPropertyInfo(35, "", "en{'Discount in f. currency'}de{'Rabatt in Fremdwährung'}")]
        public decimal? ForeginPosPriceNetDiscount
        {
            get
            {
                if (MDCurrencyExchange == null)
                    return null;
                return MDCurrencyExchange.ConvertToForeignCurrency(PosPriceNetDiscount);
            }
        }

        [ACPropertyInfo(36, "", "en{'Net total in f. currency'}de{'Netto Gesamt in Fremdwährung'}")]
        public decimal? ForeginPosPriceNetTotal
        {
            get
            {
                if (MDCurrencyExchange == null)
                    return null;
                return MDCurrencyExchange.ConvertToForeignCurrency(PosPriceNetTotal);
            }
        }

        [ACPropertyInfo(37, "", ConstApp.ForeignPriceGross)]
        public decimal? ForeignPriceGross
        {
            get
            {
                if (MDCurrencyExchange == null)
                    return null;
                return MDCurrencyExchange.ConvertToForeignCurrency(PriceGross);
            }
        }


        public void OnPricePropertyChanged()
        {
            OnPropertyChanged("PosPriceNetTotal");
            OnPropertyChanged("PosPriceNetDiscount");
            OnPropertyChanged("PosPriceNetSum");
            OnPropertyChanged("ForeginPosPriceNetTotal");
            OnPropertyChanged("ForeginPosPriceNetDiscount");
            OnPropertyChanged("ForeginPosPriceNetSum");
        }

        partial void OnPriceGrossChanged()
        {
            OnPropertyChanged("ForeignPriceGross");
        }

        [ACPropertyInfo(38, "", "en{'Due date'}de{'Fälligkeitsdatum'}")]
        public DateTime DueDate
        {
            get
            {
                if (MDTermOfPayment == null)
                    return InvoiceDate.AddDays(14);
                else
                    return InvoiceDate.AddDays(MDTermOfPayment.TermOfPaymentDays);
            }
        }


        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public void RenumberSequence(int sequence)
        {
            if (!InvoicePos_Invoice.Any())
                return;

            var elements = InvoicePos_Invoice.AsEnumerable().Where(c => c.EntityState != System.Data.EntityState.Deleted && c.Sequence >= sequence).OrderBy(c => c.Sequence);
            foreach (var element in elements)
            {
                element.Sequence = sequence;
                sequence++;
            }
        }

        public bool IsExchangeRateValid
        {
            get
            {
                if (this.OutOrder != null)
                {
                    if (!this.OutOrder.MDCurrencyID.HasValue)
                        return true;
                    if (this.MDCurrencyID == this.OutOrder.MDCurrencyID)
                        return true;
                    else if (this.MDCurrencyExchange == null)
                        return false;
                    if (this.MDCurrencyExchange != null)
                    {
                        DateTime dateTimeFrom = new DateTime(InvoiceDate.Year, InvoiceDate.Month, InvoiceDate.Day);
                        DateTime dateTimeTo = dateTimeFrom.AddDays(1);
                        return this.MDCurrencyExchange.InsertDate >= dateTimeFrom && this.MDCurrencyExchange.InsertDate <= dateTimeTo;
                    }
                }
                //else if (this.InOrder != null)
                //{
                //}
                return true;
            }
        }

        public void UpdateExchangeRate()
        {
            if (IsExchangeRateValid)
                return;
            MDCurrencyExchange = MDCurrency.GetExchangeRate(OutOrder.MDCurrency, InvoiceDate);
        }

        #endregion

       
    }
}
