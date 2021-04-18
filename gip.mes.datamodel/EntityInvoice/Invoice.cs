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
    [ACPropertyEntity(3, "InvoiceNo", "en{'Invoice No.'}de{'Rechnungsnummer'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(4, "InvoiceDate", "en{'Invoice Date'}de{'Rechnungsdatum'}", "", "", true)]
    [ACPropertyEntity(5, "CustomerCompany", ConstApp.Company, Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(6, "CustRequestNo", "en{'Extern Invoice No.'}de{'Externe Rechnungsnummer'}", "", "", true)]
    [ACPropertyEntity(7, "DeliveryCompanyAddress", "en{'Delivery Address'}de{'Lieferadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(8, "BillingCompanyAddress", "en{'Billing Address'}de{'Rechnungsadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(9, OutOrder.ClassName, "en{'Sales Order'}de{'Kundenauftrag'}", Const.ContextDatabase + "\\" + OutOrder.ClassName, "", true)]
    [ACPropertyEntity(10, MDCountrySalesTax.ClassName, ConstApp.ESCountrySalesTax, Const.ContextDatabase + "\\" + MDCountrySalesTax.ClassName, "", true)]
    [ACPropertyEntity(11, ConstApp.IssuerCompanyAddress, ConstApp.IssuerCompanyAddress_ACCaption, Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(12, ConstApp.IssuerCompanyPerson, ConstApp.IssuerCompanyPerson_ACCaption, Const.ContextDatabase + "\\" + CompanyPerson.ClassName, "", true)]
    [ACPropertyEntity(13, "Comment", ConstApp.Comment, "", "", true)]
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
    public partial class Invoice: IOutOrder
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
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);

            entity.XMLDesignStart = Const_XMLDesign;
            entity.XMLDesignEnd = Const_XMLDesign;

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

        [ACPropertyInfo(100, "", "en{'Neto total'}de{'Neto total'}")]
        public double PosPriceNetTotal
        {
            get
            {
                return PosPriceNetSum + PosPriceNetDiscount;
            }
        }

        [ACPropertyInfo(100, "", "en{'Discount'}de{'Rabatt'}")]
        public double PosPriceNetDiscount
        {
            get
            {
                if (InvoicePos_Invoice != null && InvoicePos_Invoice.Any())
                {
                    return (double)(InvoicePos_Invoice.Where(c => c.PriceNet < 0).Sum(o => o.PriceNet));
                }
                return 0;
            }
        }

        [ACPropertyInfo(100, "", "en{'Neto'}de{'Neto'}")]
        public double PosPriceNetSum
        {
            get
            {
                if (InvoicePos_Invoice != null && InvoicePos_Invoice.Any())
                {
                    return (double)(InvoicePos_Invoice.Where(c => c.PriceNet >= 0).Sum(o => o.PriceNet));
                }
                return 0;
            }
        }

        [ACPropertyInfo(101, "", "en{'Neto'}de{'Neto'}")]
        public double PosTotalSalesTax
        {
            get
            {
                if (InvoicePos_Invoice != null && InvoicePos_Invoice.Any())
                {
                    return (double)(InvoicePos_Invoice.Sum(o => o.TotalSalesTax));
                }
                return 0;
            }
        }

        [ACPropertyInfo(999)]
        public double PosTotalPriceWithTax
        {
            get
            {
                if (InvoicePos_Invoice != null && InvoicePos_Invoice.Any())
                {
                    return (double)(InvoicePos_Invoice.Sum(o => o.TotalPriceWithTax));
                }
                return 0;
            }
        }


        public void OnPricePropertyChanged()
        {
            OnPropertyChanged("PosPriceNetTotal");
            OnPropertyChanged("PosPriceNetDiscount");
            OnPropertyChanged("PosPriceNetSum");
        }


        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public void RenumberSequence(int sequence)
        {
            var elements = from c in InvoicePos_Invoice where c.Sequence > sequence && c.EntityState != System.Data.EntityState.Deleted orderby c.Sequence select c;
            int nr = 0;
            foreach (var element in elements)
            {
                element.Sequence = sequence;
                sequence++;
            }
        }
        #endregion

    }
}
