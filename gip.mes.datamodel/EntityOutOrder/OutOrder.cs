using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'Sales Order'}de{'Kundenauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOOutOrder")]
    [ACPropertyEntity(1, "OutOrderNo", "en{'Sales Order No.'}de{'Auftragsnummer'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, "OutOrderDate", "en{'Sales Order Date'}de{'Auftragsdatum'}", "", "", true)]
    [ACPropertyEntity(3, "CustOrderNo", "en{'Extern Order No.'}de{'Externe Auftragsnummer'}", "", "", true)]
    [ACPropertyEntity(4, MDOutOrderType.ClassName, "en{'Sales Order Type'}de{'Auftragsart'}", Const.ContextDatabase + "\\" + MDOutOrderType.ClassName, "", true)]
    [ACPropertyEntity(5, MDOutOrderState.ClassName, "en{'Sales Order Status'}de{'Lieferstatus'}", Const.ContextDatabase + "\\" + MDOutOrderState.ClassName, "", true)]
    [ACPropertyEntity(6, "CustomerCompany", ConstApp.Company, Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(7, "BillingCompanyAddress", "en{'Billing Address'}de{'Rechnungsadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(8, "DeliveryCompanyAddress", "en{'Delivery Address'}de{'Lieferadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(9, "PriceNet", ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(10, "PriceGross", ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(11, MDTermOfPayment.ClassName, ConstApp.TermsOfPayment, Const.ContextDatabase + "\\" + MDTermOfPayment.ClassName, "", true)]
    [ACPropertyEntity(12, "TargetDeliveryDate", ConstApp.TargetDeliveryDate, "", "", true)]
    [ACPropertyEntity(13, "TargetDeliveryMaxDate", ConstApp.TargetDeliveryMaxDate, "", "", true)]
    [ACPropertyEntity(14, MDTimeRange.ClassName, "en{'Shift'}de{'Schicht'}", Const.ContextDatabase + "\\" + MDTimeRange.ClassName, "", true)]
    [ACPropertyEntity(15, MDDelivType.ClassName, "en{'Delivery Type'}de{'Lieferart'}", Const.ContextDatabase + "\\" + MDDelivType.ClassName, "", true)]
    [ACPropertyEntity(16, "BasedOnOutOffering", "en{'Sales Offer'}de{'Angebot'}", Const.ContextDatabase + "\\" + OutOffer.ClassName, "", true)]
    [ACPropertyEntity(17, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(18, "CPartnerCompany", "en{'Contract Partner'}de{'Vertragspartner'}", Const.ContextDatabase + "\\CPartnerCompanyList", "", true)]
    [ACPropertyEntity(19, ConstApp.IssuerCompanyAddress, ConstApp.IssuerCompanyAddress_ACCaption, Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(20, ConstApp.IssuerCompanyPerson, ConstApp.IssuerCompanyPerson_ACCaption, Const.ContextDatabase + "\\" + CompanyPerson.ClassName, "", true)]
    [ACPropertyEntity(21, MDCurrency.ClassName, "en{'Currency'}de{'Währung'}", Const.ContextDatabase + "\\" + MDCurrency.ClassName, "", true)]
    [ACPropertyEntity(22, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACPropertyEntity(9999, "XMLDesignStart", "en{'Design'}de{'Design'}")]
    [ACPropertyEntity(9999, "XMLDesignEnd", "en{'Design'}de{'Design'}")]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + OutOrder.ClassName, "en{'Sales Order'}de{'Kundenauftrag'}", typeof(OutOrder), OutOrder.ClassName, "OutOrderNo", "OutOrderNo", new object[]
        {
                new object[] {Const.QueryPrefix +  OutOrderPos.ClassName, "en{'Sales Order Pos.'}de{'Auftragsposition'}", typeof(OutOrderPos), OutOrderPos.ClassName + "_" + OutOrder.ClassName, "Sequence", "Sequence", new object[]
                    {
                        new object[] {Const.QueryPrefix +  OutOrderPosUtilization.ClassName, "en{'Utilization of Sales Item'}de{'Auslastung der Auftragspos.'}", typeof(OutOrderPosUtilization), OutOrderPosUtilization.ClassName + "_" + OutOrderPos.ClassName, "OutOrderPosUtilizationNo", "OutOrderPosUtilizationNo"}
                    }
                }
        })
    ]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<OutOrder>) })]
    public partial class OutOrder : IACConfigStore, IOutOrder
    {
        public const string ClassName = "OutOrder";
        public const string NoColumnName = "OutOrderNo";
        public const string FormatNewNo = "O{0}";


        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(11020);

        #region New/Delete
        public static OutOrder NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            OutOrder entity = new OutOrder();
            entity.OutOrderID = Guid.NewGuid();
            entity.DefaultValuesACObject();

            entity.MDOutOrderType = MDOutOrderType.DefaultMDOutOrderType(dbApp, GlobalApp.OrderTypes.Order);
            entity.MDOutOrderState = MDOutOrderState.DefaultMDOutOrderState(dbApp);

            entity.MDDelivType = MDDelivType.DefaultMDDelivType(dbApp);
            entity.OutOrderNo = secondaryKey;
            entity.TargetDeliveryDate = DateTime.Now;
            entity.MDCurrencyID = null;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
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
                return OutOrderNo;
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
            if (filterValues.Any() && className == OutOrderPos.ClassName)
            {
                Int32 sequence = 0;
                if (Int32.TryParse(filterValues[0], out sequence))
                    return this.OutOrderPos_OutOrder.Where(c => c.Sequence == sequence).FirstOrDefault();
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
            if (string.IsNullOrEmpty(OutOrderNo))
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
                return "OutOrderNo";
            }
        }

        public bool IsExpired
        {
            get
            {
                if (TargetDeliveryDate > DateTime.MinValue && TargetDeliveryMaxDate > DateTime.MinValue)
                {
                    if ((DateTime.Now < TargetDeliveryDate) || (DateTime.Now > TargetDeliveryMaxDate))
                        return true;
                    return false;
                }
                else if (TargetDeliveryDate > DateTime.MinValue)
                {
                    if (DateTime.Now < TargetDeliveryDate)
                        return true;
                    return false;
                }
                else if (TargetDeliveryMaxDate > DateTime.MinValue)
                {
                    if (DateTime.Now > TargetDeliveryMaxDate)
                        return true;
                    return false;
                }
                return false;
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
            OutOrderConfig acConfig = OutOrderConfig.NewACObject(this.GetObjectContext<DatabaseApp>(), this);
            acConfig.KeyACUrl = ACConfigKeyACUrl;
            acConfig.LocalConfigACUrl = localConfigACUrl;
            acConfig.ValueTypeACClass = valueTypeACClass;
            OutOrderConfig_OutOrder.Add(acConfig);
            ACConfigListCache.Add(acConfig);
            return acConfig;
        }

        /// <summary>Removes a configuration from ConfigurationEntries and the database-context.</summary>
        /// <param name="acObject">Entry as IACConfig</param>
        public void RemoveACConfig(IACConfig acObject)
        {
            OutOrderConfig acConfig = acObject as OutOrderConfig;
            if (acConfig == null)
                return;
            if (ACConfigListCache.Contains(acConfig))
                ACConfigListCache.Remove(acConfig);
            OutOrderConfig_OutOrder.Remove(acConfig);
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
                (acConfig as OutOrderConfig).DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
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
                if (OutOrderConfig_OutOrder_IsLoaded)
                {
                    OutOrderConfig_OutOrder.AutoLoad(OutOrderConfig_OutOrderReference, this);
                }
                newSafeList = new SafeList<IACConfig>(OutOrderConfig_OutOrder.ToList().Select(x => (IACConfig)x));
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
                var query = database.OutOrderConfig.Where(c => c.OutOrderID == this.OutOrderID);
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

        #region IOutOrder additional members
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
                if (OutOrderPos_OutOrder != null && OutOrderPos_OutOrder.Any())
                {
                    return OutOrderPos_OutOrder.Where(c => c.PriceNet < 0).Sum(o => o.PriceNet);
                }
                return 0;
            }
        }

        [ACPropertyInfo(100, "", "en{'Net'}de{'Netto'}")]
        public decimal PosPriceNetSum
        {
            get
            {
                if (OutOrderPos_OutOrder != null && OutOrderPos_OutOrder.Any())
                {
                    return OutOrderPos_OutOrder.Where(c => c.TotalPrice >= 0).Sum(o => o.TotalPrice);
                }
                return 0;
            }
        }

       
        #endregion

        public void OnPricePropertyChanged()
        {
            OnPropertyChanged("PosPriceNetTotal");
            OnPropertyChanged("PosPriceNetDiscount");
            OnPropertyChanged("PosPriceNetSum");
        }
    }
}
