using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'Offer'}de{'Angebot'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOOutOffer")]
    [ACPropertyEntity(1, "OutOfferNo", "en{'Offering No.'}de{'Angebot Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "OutOfferDate", "en{'Offering Date'}de{'Angebotsdatum'}", "", "", true)]
    [ACPropertyEntity(3, "OutOfferVersion", "en{'Version'}de{'Version'}", "", "", true)]
    [ACPropertyEntity(4, "CustRequestNo", "en{'Extern Offering No.'}de{'Externe Anfragenummer'}", "", "", true)]
    [ACPropertyEntity(5, MDOutOrderType.ClassName, "en{'Order Type'}de{'Auftragsart'}", Const.ContextDatabase + "\\" + MDOutOrderType.ClassName, "", true)]
    [ACPropertyEntity(6, "MDOutOfferState", "en{'Offering Status'}de{'Angebotsstatus'}", Const.ContextDatabase + "\\MDOutOfferState", "", true)]
    [ACPropertyEntity(7, "CustomerCompany", ConstApp.Company, Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(8, "BillingCompanyAddress", ConstApp.BillingCompanyAddress, Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(9, "DeliveryCompanyAddress", ConstApp.DeliveryCompanyAddress, Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(10, "PriceNet", ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(11, "PriceGross", ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(12, MDTermOfPayment.ClassName, ConstApp.TermsOfPayment, Const.ContextDatabase + "\\" + MDTermOfPayment.ClassName, "", true)]
    [ACPropertyEntity(13, "TargetDeliveryDate", ConstApp.TargetDeliveryDate, "", "", true)]
    [ACPropertyEntity(14, "TargetDeliveryMaxDate", ConstApp.TargetDeliveryMaxDate, "", "", true)]
    [ACPropertyEntity(15, MDTimeRange.ClassName, ConstApp.ESTimeRange, Const.ContextDatabase + "\\" + MDTimeRange.ClassName, "", true)]
    [ACPropertyEntity(16, MDDelivType.ClassName, ConstApp.ESDelivType, Const.ContextDatabase + "\\" + MDDelivType.ClassName, "", true)]
    [ACPropertyEntity(17, ConstApp.IssuerCompanyAddress, ConstApp.IssuerCompanyAddress_ACCaption, Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(18, ConstApp.IssuerCompanyPerson, ConstApp.IssuerCompanyPerson_ACCaption, Const.ContextDatabase + "\\" + CompanyPerson.ClassName, "", true)]
    [ACPropertyEntity(19, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(20, MDCurrency.ClassName, "en{'Currency'}de{'Währung'}", Const.ContextDatabase + "\\" + MDCurrency.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACPropertyEntity(9999, "XMLDesignStart", "en{'Design'}de{'Design'}")]
    [ACPropertyEntity(9999, "XMLDesignEnd", "en{'Design'}de{'Design'}")]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + OutOffer.ClassName, "en{'Offering'}de{'Angebot'}", typeof(OutOffer), OutOffer.ClassName, "OutOfferNo", "OutOfferNo", new object[]
        {
                new object[] {Const.QueryPrefix + OutOfferPos.ClassName, "en{'Offer Position'}de{'Angebotsposition'}", typeof(OutOfferPos), OutOfferPos.ClassName + "_" + OutOffer.ClassName, "Sequence", "Sequence"}
        })
    ]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<OutOffer>) })]
    public partial class OutOffer : IACConfigStore, IOutOrder, ICloneable
    {
        public const string ClassName = "OutOffer";
        public const string NoColumnName = "OutOfferNo";
        public const string FormatNewNo = "OO{0}";

        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(11020);

        #region New/Delete
        public static OutOffer NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            OutOffer entity = new OutOffer();
            entity.OutOfferID = Guid.NewGuid();
            entity.DefaultValuesACObject();

            entity.MDOutOrderType = MDOutOrderType.DefaultMDOutOrderType(dbApp, GlobalApp.OrderTypes.Order);
            entity.MDOutOfferState = MDOutOfferState.DefaultMDOutOfferState(dbApp);

            entity.MDDelivType = MDDelivType.DefaultMDDelivType(dbApp);
            entity.OutOfferNo = secondaryKey;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            entity.OutOfferDate = DateTime.Now;

            entity.XMLDesignStart = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FlowDocument PageWidth=\"816\" PageHeight=\"1056\" PagePadding=\"96,96,96,96\" AllowDrop=\"True\" NumberSubstitution.CultureSource=\"User\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"></FlowDocument>";
            entity.XMLDesignEnd = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FlowDocument PageWidth=\"816\" PageHeight=\"1056\" PagePadding=\"96,96,96,96\" AllowDrop=\"True\" NumberSubstitution.CultureSource=\"User\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"></FlowDocument>";
            
            entity.IssuerCompanyAddress = null;
            entity.IssuerCompanyPerson = null;
            entity.MDCurrencyID = null;
            
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
                return OutOfferNo + "(" + OutOfferVersion.ToString() + ") ";
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
            if (filterValues.Any() && className == OutOfferPos.ClassName)
            {
                Int32 sequence = 0;
                if (Int32.TryParse(filterValues[0], out sequence))
                    return this.OutOfferPos_OutOffer.Where(c => c.Sequence == sequence).FirstOrDefault();
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
            if (string.IsNullOrEmpty(OutOfferNo))
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
                return "OutOfferNo";
            }
        }
        #endregion

        #region IEntityProperty Members

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

        #region IACConfigStore

        private string configStoreName;
        public string ConfigStoreName
        {
            get
            {
                if (configStoreName == null)
                {
                    ACClassInfo acClassInfo = (ACClassInfo)GetType().GetCustomAttributes(typeof(ACClassInfo), false)[0];
                    configStoreName = Translator.GetTranslation(acClassInfo.ACCaptionTranslation);
                }
                return configStoreName;
            }
        }

        /// <summary>
        /// ACConfigKeyACUrl returns the relative Url to the "main table" in group a group of semantically related tables.
        /// This property is used when NewACConfig() is called. NewACConfig() creates a new IACConfig-Instance and set the IACConfig.KeyACUrl-Property with this ACConfigKeyACUrl.
        /// </summary>
        public string ACConfigKeyACUrl
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Creates and adds a new IACConfig-Entry to ConfigItemsSource.
        /// The implementing class creates a new entity object an add it to its "own Configuration-Table".
        /// It sets automatically the IACConfig.KeyACUrl-Property with this ACConfigKeyACUrl.
        /// </summary>
        /// <param name="acObject">Optional: Reference to another Entity-Object that should be related for this new configuration entry.</param>
        /// <param name="valueTypeACClass">The iPlus-Type of the "Value"-Property.</param>
        /// <returns>IACConfig as a new entry</returns>
        public IACConfig NewACConfig(IACObjectEntity acObject = null, gip.core.datamodel.ACClass valueTypeACClass = null, string localConfigACUrl = null)
        {
            OutOfferConfig acConfig = OutOfferConfig.NewACObject(this.GetObjectContext<DatabaseApp>(), this);
            acConfig.KeyACUrl = ACConfigKeyACUrl;
            acConfig.LocalConfigACUrl = localConfigACUrl;
            acConfig.ValueTypeACClass = valueTypeACClass;
            OutOfferConfig_OutOffer.Add(acConfig);
            ACConfigListCache.Add(acConfig);
            return acConfig;
        }

        /// <summary>Removes a configuration from ConfigurationEntries and the database-context.</summary>
        /// <param name="acObject">Entry as IACConfig</param>
        public void RemoveACConfig(IACConfig acObject)
        {
            OutOfferConfig acConfig = acObject as OutOfferConfig;
            if (acConfig == null)
                return;
            if (ACConfigListCache.Contains(acConfig))
                ACConfigListCache.Remove(acConfig);
            OutOfferConfig_OutOffer.Remove(acConfig);
            if (acConfig.EntityState != EntityState.Detached)
                acConfig.DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
        }

        /// <summary>
        /// Deletes all IACConfig-Entries in the Database-Context as well as in ConfigurationEntries.
        /// </summary>
        public void DeleteAllConfig()
        {
            if (!ConfigurationEntries.Any())
                return;
            ClearCacheOfConfigurationEntries();
            List<IACConfig> list = ConfigurationEntries.ToList();
            foreach (var acConfig in list)
            {
                (acConfig as OutOfferConfig).DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
            }
            ClearCacheOfConfigurationEntries();
        }

        public decimal OverridingOrder { get; set; }

        /// <summary>
        /// A thread-safe and cached list of Configuration-Values of type IACConfig.
        /// </summary>
        public IEnumerable<IACConfig> ConfigurationEntries
        {
            get
            {
                return ACConfigListCache;
            }
        }

        private SafeList<IACConfig> _ACConfigListCache;
        private SafeList<IACConfig> ACConfigListCache
        {
            get
            {
                using (ACMonitor.Lock(_11020_LockValue))
                {
                    if (_ACConfigListCache != null)
                        return _ACConfigListCache;
                }
                SafeList<IACConfig> newSafeList = new SafeList<IACConfig>();
                if (OutOfferConfig_OutOffer_IsLoaded)
                {
                    OutOfferConfig_OutOffer.AutoLoad(OutOfferConfig_OutOfferReference, this);
                }
                newSafeList = new SafeList<IACConfig>(OutOfferConfig_OutOffer.ToList().Select(x => (IACConfig)x));
                using (ACMonitor.Lock(_11020_LockValue))
                {
                    _ACConfigListCache = newSafeList;
                    return _ACConfigListCache;
                }
            }
        }

        /// <summary>Clears the cache of configuration entries. (ConfigurationEntries)
        /// Re-accessing the ConfigurationEntries property rereads all configuration entries from the database.</summary>
        public void ClearCacheOfConfigurationEntries()
        {
            using (ACMonitor.Lock(_11020_LockValue))
            {
                _ACConfigListCache = null;
            }
        }

        /// <summary>
        /// Checks if cached configuration entries are loaded from database successfully
        /// </summary>
        public bool ValidateConfigurationEntriesWithDB(ConfigEntriesValidationMode mode = ConfigEntriesValidationMode.AnyCheck)
        {
            if (mode == ConfigEntriesValidationMode.AnyCheck)
            {
                if (ConfigurationEntries.Any())
                    return true;
            }
            using (DatabaseApp database = new DatabaseApp())
            {
                var query = database.OutOfferConfig.Where(c => c.OutOfferID == this.OutOfferID);
                if (mode == ConfigEntriesValidationMode.AnyCheck)
                {
                    if (query.Any())
                        return false;
                }
                else if (mode == ConfigEntriesValidationMode.CompareCount
                         || mode == ConfigEntriesValidationMode.CompareContent)
                    return query.Count() == ConfigurationEntries.Count();
            }
            return true;
        }

        #endregion

        #region Additional properties

        [ACPropertyInfo(100, "", "en{'Net total'}de{'Netto Gesamt'}")]
        public decimal PosPriceNetTotal
        {
            get
            {
                return PosPriceNetSum + PosPriceNetDiscount;
            }
        }

        [ACPropertyInfo(100, "", "en{'Discount'}de{'Rabatt'}")]
        public decimal PosPriceNetDiscount
        {
            get
            {
                if (OutOfferPos_OutOffer != null && OutOfferPos_OutOffer.Any())
                {
                    return OutOfferPos_OutOffer.Where(c => c.PriceNet < 0).Sum(o => o.PriceNet);
                }
                return 0;
            }
        }

        [ACPropertyInfo(100, "", "en{'Net'}de{'Netto'}")]
        public decimal PosPriceNetSum
        {
            get
            {
                if (OutOfferPos_OutOffer != null && OutOfferPos_OutOffer.Any())
                {
                    return OutOfferPos_OutOffer.Where(c => c.TotalPrice >= 0).Sum(o => o.TotalPrice);
                }
                return 0;
            }
        }

        #endregion

        #region Methods

        public void OnPricePropertyChanged()
        {
            OnPropertyChanged("PosPriceNetTotal");
            OnPropertyChanged("PosPriceNetDiscount");
            OnPropertyChanged("PosPriceNetSum");
        }

        #endregion

        public object Clone()
        {
            OutOffer clonedObject = new OutOffer();
            clonedObject.OutOfferID = this.OutOfferID;
            clonedObject.OutOfferNo = this.OutOfferNo;
            clonedObject.OutOfferVersion = this.OutOfferVersion;
            clonedObject.OutOfferDate = this.OutOfferDate;
            clonedObject.MDCurrencyID = this.MDCurrencyID;
            clonedObject.MDOutOrderTypeID = this.MDOutOrderTypeID;
            clonedObject.MDOutOfferStateID = this.MDOutOfferStateID;
            clonedObject.CustomerCompanyID = this.CustomerCompanyID;
            clonedObject.CustRequestNo = this.CustRequestNo;
            clonedObject.DeliveryCompanyAddressID = this.DeliveryCompanyAddressID;
            clonedObject.BillingCompanyAddressID = this.BillingCompanyAddressID;
            clonedObject.IssuerCompanyAddressID = this.IssuerCompanyAddressID;
            clonedObject.IssuerCompanyPersonID = this.IssuerCompanyPersonID;
            clonedObject.TargetDeliveryDate = this.TargetDeliveryDate;
            clonedObject.TargetDeliveryMaxDate = this.TargetDeliveryMaxDate;
            clonedObject.MDTimeRangeID = this.MDTimeRangeID;
            clonedObject.MDDelivTypeID = this.MDDelivTypeID;
            clonedObject.PriceNet = this.PriceNet;
            clonedObject.PriceGross = this.PriceGross;
            clonedObject.SalesTax = this.SalesTax;
            clonedObject.MDTermOfPaymentID = this.MDTermOfPaymentID;
            clonedObject.Comment = this.Comment;
            clonedObject.XMLDesignStart = this.XMLDesignStart;
            clonedObject.XMLDesignEnd = this.XMLDesignEnd;
            clonedObject.XMLConfig = this.XMLConfig;

            return clonedObject;
        }
    }
}
