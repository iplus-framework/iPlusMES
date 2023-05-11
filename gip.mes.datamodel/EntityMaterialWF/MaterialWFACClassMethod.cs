using gip.core.datamodel;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Partslist 
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Materialworkflow'}de{'Materialworkflow'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOMaterialWF")]
    [ACPropertyEntity(4, MaterialWF.ClassName, "en{'Materialflow'}de{'Materialfluss'}", Const.ContextDatabase + "\\" + MaterialWF.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, gip.core.datamodel.ACClassMethod.ClassName, "en{'Workflow'}de{'Workflow'}", Const.ContextDatabase + "\\" + gip.core.datamodel.ACClassMethod.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, Const.IsDefault, "en{'Default workflow'}de{'Standard workflow'}")]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MaterialWFACClassMethod.ClassName, "en{'MaterialWF-Workflow-Mapping'}de{'MaterialWF-Workflow-Mapping'}", typeof(MaterialWFACClassMethod), MaterialWFACClassMethod.ClassName, "", "MaterialWFACClassMethodID")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaterialWFACClassMethod>) })]
    public partial class MaterialWFACClassMethod : IACConfigStore
    {
        public const string ClassName = "MaterialWFACClassMethod";

        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(11020);

        #region New / Delete
        public static MaterialWFACClassMethod NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MaterialWFACClassMethod entity = new MaterialWFACClassMethod();
            entity.MaterialWFACClassMethodID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            entity.IsDefault = false;
            return entity;
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
                return null; // GetKey();
            }
        }

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "MaterialWF\\ACIdentifier,ACClassMethod\\ACIdentifier";
            }
        }

        public string GetKey()
        {
            return this.MaterialWF.GetACUrl() + ACUrlHelper.Delimiter_Relationship + ACClassMethod.GetACUrl();
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
            MaterialWFACClassMethodConfig acConfig = MaterialWFACClassMethodConfig.NewACObject(this.GetObjectContext<DatabaseApp>(), this);
            acConfig.KeyACUrl = ACConfigKeyACUrl;
            acConfig.LocalConfigACUrl = localConfigACUrl;
            acConfig.ValueTypeACClass = valueTypeACClass;
            MaterialWFACClassMethodConfig_MaterialWFACClassMethod.Add(acConfig);
            if (acObject != null && acObject is core.datamodel.ACClassWF)
                acConfig.VBiACClassWFID = ((core.datamodel.ACClassWF)acObject).ACClassWFID;
            ACConfigListCache.Add(acConfig);
            return acConfig;
        }

        /// <summary>Removes a configuration from ConfigurationEntries and the database-context.</summary>
        /// <param name="acObject">Entry as IACConfig</param>
        public void RemoveACConfig(IACConfig acObject)
        {
            MaterialWFACClassMethodConfig acConfig = acObject as MaterialWFACClassMethodConfig;
            if (acConfig == null)
                return;
            if (ACConfigListCache.Contains(acConfig))
                ACConfigListCache.Remove(acConfig);
            MaterialWFACClassMethodConfig_MaterialWFACClassMethod.Remove(acConfig);
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
                (acConfig as MaterialWFACClassMethodConfig).DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
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
                if (MaterialWFACClassMethodConfig_MaterialWFACClassMethod_IsLoaded)
                {
                    MaterialWFACClassMethodConfig_MaterialWFACClassMethod.AutoLoad(MaterialWFACClassMethodConfig_MaterialWFACClassMethodReference, this);
                }
                newSafeList = new SafeList<IACConfig>(MaterialWFACClassMethodConfig_MaterialWFACClassMethod.ToList().Select(x => (IACConfig)x));
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
                var query = database.MaterialWFACClassMethodConfig.Where(c => c.MaterialWFACClassMethodID == this.MaterialWFACClassMethodID);
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

        #region convention members

        public override string ToString()
        {
            if (MaterialWF == null || ACClassMethod == null)
                return null;
            return MaterialWF.MaterialWFNo + "<=>" + ACClassMethod.ACIdentifier;
        }

        protected override void OnPropertyChanged(string property)
        {
            base.OnPropertyChanged(property);
            if (property == Const.IsDefault && IsDefault && MaterialWF != null)
            {
                foreach (var mwMethod in this.MaterialWF.MaterialWFACClassMethod_MaterialWF)
                {
                    if (mwMethod == this)
                        continue;
                    mwMethod.IsDefault = false;
                }
            }
        }

        #endregion
    }
}
