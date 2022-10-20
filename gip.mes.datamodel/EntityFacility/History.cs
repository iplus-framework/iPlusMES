using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{
    // History (LagerHistorie)
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Balance Sheet History'}de{'Bilanzhistorie'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "BalanceDate", "en{'Closing Date'}de{'Abschlussdatum'}", "", "", true)]
    [ACPropertyEntity(2, "TimePeriodIndex", "en{'Time Period'}de{'Zeitspanne'}", typeof(GlobalApp.TimePeriods), Const.ContextDatabase + "\\TimePeriodsList", "", true)]
    [ACPropertyEntity(3, "PeriodNo", "en{'Period No.'}de{'Periodennr.'}", "", "", true)]
    [ACPropertyEntity(4, "Comment", ConstApp.Comment, "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + History.ClassName, "en{'Balance Sheet History'}de{'Bilanzhistorie'}", typeof(History), History.ClassName, "TimePeriodIndex,PeriodNo", "BalanceDate", new object[]
        {
                new object[] {Const.QueryPrefix + MaterialHistory.ClassName, "en{'Material history'}de{'Material historie'}", typeof(MaterialHistory), MaterialHistory.ClassName + "_" + History.ClassName, Material.ClassName + "\\MaterialNo", Material.ClassName + "\\MaterialNo"},
                new object[] {Const.QueryPrefix + CompanyMaterialHistory.ClassName, "en{'Companymaterial history'}de{'Unternehmensmaterial historie'}", typeof(CompanyMaterialHistory), CompanyMaterialHistory.ClassName + "_" + History.ClassName, CompanyMaterial.ClassName + "\\CompanyMaterialNo", CompanyMaterial.ClassName + "\\CompanyMaterialNo"},
                new object[] {Const.QueryPrefix + FacilityHistory.ClassName, "en{'Facility history'}de{'Lager historie'}", typeof(FacilityHistory), FacilityHistory.ClassName + "_" + History.ClassName, Facility.ClassName + "\\FacilityNo", Facility.ClassName + "\\FacilityNo"}
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<History>) })]
    public partial class History : IACConfigStore
    {
        public const string ClassName = "History";
        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(11020);

        #region New/Delete
        public static History NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            History entity = new History();
            entity.HistoryID = Guid.NewGuid();
            entity.BalanceDate = DateTime.Now;
            entity.TimePeriod = GlobalApp.TimePeriods.Day;
            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            return string.Format(@"{0}|[{1}] {2}", ClassName, TimePeriod, BalanceDate.ToString("dd.MM.yyyy HH:mm"));
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                ACValueItem valueItem = GlobalApp.TimePeriodsList.Where(c => (short)c.Value == TimePeriodIndex).FirstOrDefault();
                if (valueItem != null)
                    return valueItem.ACCaption + " (" + BalanceDate.ToString() + ")";
                else
                    return BalanceDate.ToString();
            }
        }

        [ACPropertyInfo(5, "", "en{'Prev. Closing Date'}de{'Letztes Abschlussdatum'}")]
        public DateTime BalanceDatePrev
        {
            get
            {
                History prevHistory = GetPreviousHistory(this.GetObjectContext<DatabaseApp>());
                if (prevHistory != null)
                    return prevHistory.BalanceDate;
                return DateTime.MinValue;
            }
        }


        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "BalanceDate,TimePeriodIndex";
            }
        }

        #endregion

        #region This Member
        /// <summary>
        /// Gets or sets the Dimension
        /// </summary>
        /// <value>Dimension</value>
        public GlobalApp.TimePeriods TimePeriod
        {
            get
            {
                return (GlobalApp.TimePeriods)TimePeriodIndex;
            }
            set
            {
                TimePeriodIndex = (Int16)value;
            }
        }
        #endregion

        #region Methods

        public History GetPreviousHistory(DatabaseApp dbApp)
        {
            if (dbApp == null)
                return null;
            return dbApp.History.Where(c => c.TimePeriodIndex == this.TimePeriodIndex && c.BalanceDate < this.BalanceDate).OrderByDescending(c => c.BalanceDate).FirstOrDefault();
        }

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
            get { return null; }
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
            HistoryConfig acConfig = HistoryConfig.NewACObject(this.GetObjectContext<DatabaseApp>(), this);
            acConfig.KeyACUrl = ACConfigKeyACUrl;
            acConfig.LocalConfigACUrl = localConfigACUrl;
            acConfig.ValueTypeACClass = valueTypeACClass;
            HistoryConfig_History.Add(acConfig);
            ACConfigListCache.Add(acConfig);
            return acConfig;
        }

        /// <summary>Removes a configuration from ConfigurationEntries and the database-context.</summary>
        /// <param name="acObject">Entry as IACConfig</param>
        public void RemoveACConfig(IACConfig acObject)
        {
            HistoryConfig acConfig = acObject as HistoryConfig;
            if (acConfig == null)
                return;
            if (ACConfigListCache.Contains(acConfig))
                ACConfigListCache.Remove(acConfig);
            HistoryConfig_History.Remove(acConfig);
            if (acConfig.EntityState != System.Data.EntityState.Detached)
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
                (acConfig as HistoryConfig).DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
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
                if (HistoryConfig_History.IsLoaded)
                {
                    HistoryConfig_History.AutoRefresh();
                    HistoryConfig_History.AutoLoad();
                }
                newSafeList = new SafeList<IACConfig>(HistoryConfig_History.ToList().Select(x => (IACConfig)x));
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
                var query = database.HistoryConfig.Where(c => c.HistoryID == this.HistoryID);
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



