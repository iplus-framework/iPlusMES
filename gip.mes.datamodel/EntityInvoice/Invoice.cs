using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESInvoice, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOInvoice")]
    [ACPropertyEntity(1, MDInvoiceType.ClassName, ConstApp.ESInvoiceType, Const.ContextDatabase + "\\" + MDInvoiceType.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, MDInvoiceState.ClassName, ConstApp.ESInvoiceState, Const.ContextDatabase + "\\" + MDInvoiceState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, "InvoiceNo", "en{'Invoice No.'}de{'Rechnungsnummer'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(4, "InvoiceDate", "en{'Invoice Date'}de{'Rechnungsdatum'}", "", "", true)]
    [ACPropertyEntity(5, "CustomerCompany", ConstApp.Company, Const.ContextDatabase + "\\" + Company.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, "CustRequestNo", "en{'Extern Invoice No.'}de{'Externe Rechnungsnummer'}", "", "", true)]
    [ACPropertyEntity(7, "DeliveryCompanyAddress", "en{'Delivery Address'}de{'Lieferadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(8, "BillingCompanyAddress", "en{'Billing Address'}de{'Rechnungsadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9, OutOrder.ClassName, "en{'Sales Order'}de{'Kundenauftrag'}", Const.ContextDatabase + "\\" + OutOrder.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    //[ACPropertyEntity(10, MDCountrySalesTax.ClassName, ConstApp.ESCountrySalesTax, Const.ContextDatabase + "\\" + MDCountrySalesTax.ClassName, "", true)]
    [ACPropertyEntity(11, ConstApp.IssuerCompanyAddress, ConstApp.IssuerCompanyAddress_ACCaption, Const.ContextDatabase + "\\" + CompanyAddress.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(12, ConstApp.IssuerCompanyPerson, ConstApp.IssuerCompanyPerson_ACCaption, Const.ContextDatabase + "\\" + CompanyPerson.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(13, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(14, "PriceNet", ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(15, "PriceGross", ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(16, MDTermOfPayment.ClassName, ConstApp.TermsOfPayment, Const.ContextDatabase + "\\" + MDTermOfPayment.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(20, MDCurrency.ClassName, "en{'Currency'}de{'Währung'}", Const.ContextDatabase + "\\" + MDCurrency.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(21, MDCurrencyExchange.ClassName, "en{'Currency-Exchange'}de{'Währungsumrechnung'}", Const.ContextDatabase + "\\" + MDCurrencyExchange.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACPropertyEntity(9999, "XMLDesignStart", "en{'Design'}de{'Design'}")]
    [ACPropertyEntity(9999, "XMLDesignEnd", "en{'Design'}de{'Design'}")]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + Invoice.ClassName, ConstApp.ESInvoice, typeof(Invoice), Invoice.ClassName, "InvoiceNo", "InvoiceNo", new object[]
        {
                new object[] {Const.QueryPrefix +  InvoicePos.ClassName, ConstApp.ESInvoicePos, typeof(InvoicePos), InvoicePos.ClassName + "_" + InvoicePos.ClassName, "Sequence", "Sequence", new object[]
                    {

                    }
                }
        })
    ]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<Invoice>) })]
    [NotMapped]
    public partial class Invoice : IOutOrder
    {
        [NotMapped]
        public const string ClassName = "Invoice";
        [NotMapped]
        public const string NoColumnName = "InvoiceNo";
        [NotMapped]
        public const string FormatNewNo = "O{0}";

        [NotMapped]
        public const string Const_XMLDesign = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FlowDocument PageWidth=\"816\" PageHeight=\"1056\" PagePadding=\"96,96,96,96\" AllowDrop=\"True\" NumberSubstitution.CultureSource=\"User\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"></FlowDocument>";

        [NotMapped]
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
        [NotMapped]
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

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "InvoiceNo";
            }
        }


        #endregion

        #region IEntityProperty Members

        [NotMapped]
        bool bRefreshConfig = false;
        protected override void OnPropertyChanging<T>(T newValue, string propertyName, bool afterChange)
        {
            if (propertyName == nameof(XMLConfig))
            {
                string xmlConfig = newValue as string;
                if (afterChange)
                {
                    if (bRefreshConfig)
                        ACProperties.Refresh();
                }
                else
                {
                    bRefreshConfig = false;
                    if (this.EntityState != EntityState.Detached && (!(String.IsNullOrEmpty(xmlConfig) && String.IsNullOrEmpty(XMLConfig)) && xmlConfig != XMLConfig))
                        bRefreshConfig = true;
                }
            }
            base.OnPropertyChanging(newValue, propertyName, afterChange);
        }

        #endregion

        #region Additional
        [NotMapped]
        public bool IsReverseCharge
        {
            get
            {
                return !InvoicePos_Invoice.Where(c => c.SalesTax > 0).Any();
            }
        }

        [ACPropertyInfo(31, "", "en{'Net total'}de{'Netto Gesamt'}")]
        [NotMapped]
        public decimal PosPriceNetTotal
        {
            get
            {
                return PosPriceNetSum + PosPriceNetDiscount;
            }
        }

        [ACPropertyInfo(32, "", "en{'Discount'}de{'Rabatt'}")]
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
            base.OnPropertyChanged("PosPriceNetTotal");
            base.OnPropertyChanged("PosPriceNetDiscount");
            base.OnPropertyChanged("PosPriceNetSum");
            base.OnPropertyChanged("ForeginPosPriceNetTotal");
            base.OnPropertyChanged("ForeginPosPriceNetDiscount");
            base.OnPropertyChanged("ForeginPosPriceNetSum");
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(PriceGross))
            {
                OnPropertyChanged("ForeignPriceGross");
            }
            base.OnPropertyChanged(propertyName);
        }

        [ACPropertyInfo(38, "", "en{'Due date'}de{'Fälligkeitsdatum'}")]
        [NotMapped]
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

            var elements = InvoicePos_Invoice.AsEnumerable().Where(c => c.EntityState != EntityState.Deleted && c.Sequence >= sequence).OrderBy(c => c.Sequence);
            foreach (var element in elements)
            {
                element.Sequence = sequence;
                sequence++;
            }
        }

        [NotMapped]
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
