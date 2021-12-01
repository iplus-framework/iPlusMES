using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Picking Order'}de{'Kommissionierauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOPicking")]
    [ACPropertyEntity(1, "PickingNo", "en{'Picking-No.'}de{'Kommissions-Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "PickingTypeIndex", "en{'Picking Type'}de{'Kommissioniertyp'}", typeof(GlobalApp.PickingType), Const.ContextDatabase + "\\PickingTypeList", "", true)]
    [ACPropertyEntity(3, "PickingStateIndex", "en{'Picking Status'}de{'Status'}", typeof(GlobalApp.PickingState), Const.ContextDatabase + "\\PickingStateList", "", true)]
    [ACPropertyEntity(4, "DeliveryDateFrom", "en{'Date from'}de{'Datum von'}", "", "", true)]
    [ACPropertyEntity(5, "DeliveryDateTo", "en{'Date to'}de{'Datum bis'}", "", "", true)]
    [ACPropertyEntity(6, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(7, VisitorVoucher.ClassName, "en{'Visitor Voucher'}de{'Besucherbeleg'}", Const.ContextDatabase + "\\" + VisitorVoucher.ClassName, "", true)]
    [ACPropertyEntity(8, gip.core.datamodel.ACClassMethod.ClassName, "en{'Workflow'}de{'Workflow'}", Const.ContextDatabase + "\\" + gip.core.datamodel.ACClassMethod.ClassName, "", true)]
    [ACPropertyEntity(9, MDPickingType.ClassName, "en{'Picking type'}de{'Kommissionierung Typ'}", Const.ContextDatabase + "\\" + MDPickingType.ClassName, "", true)]
    [ACPropertyEntity(9999, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + Picking.ClassName, "en{'Picking Order'}de{'Kommissionierauftrag'}", typeof(Picking), Picking.ClassName, "PickingNo", "PickingNo", new object[]
        {
                new object[] {Const.QueryPrefix + PickingPos.ClassName, "en{'Picking Item'}de{'Kommissionierposition'}", typeof(PickingPos), PickingPos.ClassName + "_" + Picking.ClassName, "Sequence", "Sequence"}
        })
    ]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<Picking>) })]
    public partial class Picking : IACConfigStore
    {
        public const string ClassName = "Picking";
        public const string NoColumnName = "PickingNo";
        public const string FormatNewNo = "PK{0}";

        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(11020);

        #region New/Delete
        public static Picking NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            Picking entity = new Picking();
            entity.PickingID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDPickingType = MDPickingType.s_cQry_Default(dbApp).FirstOrDefault();
            entity.PickingState = GlobalApp.PickingState.New;
            entity.PickingNo = secondaryKey;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
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
                return PickingNo;
            }
        }

        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && className == PickingPos.ClassName)
            {
                Int32 sequence = 0;
                if (Int32.TryParse(filterValues[0], out sequence))
                    return this.PickingPos_Picking.Where(c => c.Sequence == sequence).FirstOrDefault();
            }
            return null;
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "PickingNo";
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

        #region others

        //public GlobalApp.PickingType PickingType
        //{
        //    get
        //    {
        //        return (GlobalApp.PickingType)MDPickingType.MDPickingTypeIndex;
        //    }
        //}

        public GlobalApp.PickingState PickingState
        {
            get
            {
                return (GlobalApp.PickingState)PickingStateIndex;
            }
            set
            {
                PickingStateIndex = (Int16)value;
            }
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
                return null; // GetKey();
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
            PickingConfig pickingConfig = PickingConfig.NewACObject(this.GetObjectContext<DatabaseApp>(), this);
            pickingConfig.KeyACUrl = ACConfigKeyACUrl;
            pickingConfig.ValueTypeACClass = valueTypeACClass;
            if (acObject != null && acObject is core.datamodel.ACClassWF)
                pickingConfig.VBiACClassWFID = ((core.datamodel.ACClassWF)acObject).ACClassWFID;
            PickingConfig_Picking.Add(pickingConfig);
            ACConfigListCache.Add(pickingConfig);
            return pickingConfig;
        }

        /// <summary>Removes a configuration from ConfigurationEntries and the database-context.</summary>
        /// <param name="acObject">Entry as IACConfig</param>
        public void RemoveACConfig(IACConfig acObject)
        {
            PickingConfig acConfig = acObject as PickingConfig;
            if (acConfig == null)
                return;
            if (ACConfigListCache.Contains(acConfig))
                ACConfigListCache.Remove(acConfig);
            PickingConfig_Picking.Remove(acConfig);
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
                (acConfig as PickingConfig).DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
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
                if (PickingConfig_Picking.IsLoaded)
                {
                    PickingConfig_Picking.AutoRefresh();
                    PickingConfig_Picking.AutoLoad();
                }
                newSafeList = new SafeList<IACConfig>(PickingConfig_Picking.ToList().Select(x => (IACConfig)x));
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
                var query = database.PickingConfig.Where(c => c.PickingID == this.PickingID);
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
