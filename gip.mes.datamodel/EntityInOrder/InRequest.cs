using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Quotation'}de{'Anfrage'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOInRequest")]
    [ACPropertyEntity(1, "InRequestNo", "en{'Quotation No.'}de{'Anfrage Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "InRequestDate", "en{'Quotation Date'}de{'Anfrage Datum'}", "", "", true)]
    [ACPropertyEntity(3, "InRequestVersion", "en{'Version'}de{'Version'}", "", "", true)]
    [ACPropertyEntity(4, "DistributorOfferingNo", "en{'Extern Quotation No.'}de{'Externe Angebotsnr.'}", "", "", true)]
    [ACPropertyEntity(5, "MDInOrderType", ConstApp.ESInOrderType, Const.ContextDatabase + "\\MDInOrderType" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, "MDInRequestState", ConstApp.ESInRequestState, Const.ContextDatabase + "\\MDInRequestState" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(7, "DistributorCompany", "en{'DistributorCompany'}de{'Lieferant'}", Const.ContextDatabase + "\\" + Company.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(8, "BillingCompanyAddress", ConstApp.BillingCompanyAddress, Const.ContextDatabase + "\\" + CompanyAddress.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9, "DeliveryCompanyAddress", ConstApp.DeliveryCompanyAddress, Const.ContextDatabase + "\\" + CompanyAddress.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(10, "PriceNet", ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(11, "PriceGross", ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(12, MDTermOfPayment.ClassName, ConstApp.TermsOfPayment, Const.ContextDatabase + "\\" + MDTermOfPayment.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(13, "TargetDeliveryDate", ConstApp.TargetDeliveryDate, "", "", true)]
    [ACPropertyEntity(14, "TargetDeliveryMaxDate", ConstApp.TargetDeliveryMaxDate, "", "", true)]
    [ACPropertyEntity(15, MDTimeRange.ClassName, ConstApp.ESTimeRange, Const.ContextDatabase + "\\" + MDTimeRange.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(16, MDDelivType.ClassName, ConstApp.ESDelivType, Const.ContextDatabase + "\\" + MDDelivType.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(17, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioPurchase, Const.QueryPrefix + InRequest.ClassName, "en{'Quotation'}de{'Anfrage'}", typeof(InRequest), InRequest.ClassName, "InRequestNo", "InRequestNo", new object[]
        {
                new object[] {Const.QueryPrefix + InRequestPos.ClassName, "en{'Quotation Pos.'}de{'Anfrageposition'}", typeof(InRequestPos), InRequestPos.ClassName + "_" + InRequest.ClassName, "Sequence", "Sequence"}
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<InRequest>) })]
    public partial class InRequest : IACConfigStore
    {
        [NotMapped]
        public const string ClassName = "InRequest";
        [NotMapped]
        public const string NoColumnName = "InRequestNo";
        [NotMapped]
        public const string FormatNewNo = "IR{0}";

        [NotMapped]
        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(101020);

        #region New/Delete
        public static InRequest NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            InRequest entity = new InRequest();
            entity.InRequestID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDInOrderType = MDInOrderType.DefaultMDInOrderType(dbApp);
            entity.MDInRequestState = MDInRequestState.DefaultMDInRequestState(dbApp);

            entity.MDDelivType = MDDelivType.DefaultMDDelivType(dbApp);
            entity.InRequestNo = secondaryKey;
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
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return InRequestNo + "(" + InRequestVersion.ToString() + ")" + DistributorCompany.ACCaption;
            }
        }

        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && className == InRequestPos.ClassName)
            {
                Int32 sequence = 0;
                if (Int32.TryParse(filterValues[0], out sequence))
                    return this.InRequestPos_InRequest.Where(c => c.Sequence == sequence).FirstOrDefault();
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
            if (string.IsNullOrEmpty(InRequestNo))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "InRequestNo",
                    Message = "InRequestNo is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "InRequestNo"), 
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
                return "InRequestNo";
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

        #region IACConfigStore


        [NotMapped]
        private string configStoreName;
        [NotMapped]
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
        [NotMapped]
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
            InRequestConfig acConfig = InRequestConfig.NewACObject(this.GetObjectContext<DatabaseApp>(), this);
            acConfig.KeyACUrl = ACConfigKeyACUrl;
            acConfig.LocalConfigACUrl = localConfigACUrl;
            acConfig.ValueTypeACClass = valueTypeACClass;
            InRequestConfig_InRequest.Add(acConfig);
            ACConfigListCache.Add(acConfig);
            return acConfig;
        }

        /// <summary>Removes a configuration from ConfigurationEntries and the database-context.</summary>
        /// <param name="acObject">Entry as IACConfig</param>
        public void RemoveACConfig(IACConfig acObject)
        {
            InRequestConfig acConfig = acObject as InRequestConfig;
            if (acConfig == null)
                return;
            if (ACConfigListCache.Contains(acConfig))
                ACConfigListCache.Remove(acConfig);
            InRequestConfig_InRequest.Remove(acConfig);
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
                (acConfig as InRequestConfig).DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
            }
            ClearCacheOfConfigurationEntries();
        }

        [NotMapped]
        public decimal OverridingOrder { get; set; }

        /// <summary>
        /// A thread-safe and cached list of Configuration-Values of type IACConfig.
        /// </summary>
        [NotMapped]
        public IEnumerable<IACConfig> ConfigurationEntries
        {
            get
            {
                return ACConfigListCache;
            }
        }

        [NotMapped]
        private SafeList<IACConfig> _ACConfigListCache;
        [NotMapped]
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
                if (InRequestConfig_InRequest_IsLoaded)
                {
                    InRequestConfig_InRequest.AutoLoad(InRequestConfig_InRequestReference, this);
                }
                newSafeList = new SafeList<IACConfig>(InRequestConfig_InRequest.ToList().Select(x => (IACConfig)x));
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
                var query = database.InRequestConfig.Where(c => c.InRequestID == this.InRequestID);
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

    }
}
