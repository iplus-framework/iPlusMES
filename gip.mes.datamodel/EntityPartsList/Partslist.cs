using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Partslist 
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Bill of Material'}de{'Stückliste'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOPartslist")]
    [ACPropertyEntity(1, "PartslistName", "en{'Name'}de{'Name'}", "", "", true)]
    [ACPropertyEntity(2, "PartslistNo", ConstApp.Number, "", "", true)]
    [ACPropertyEntity(3, "PartslistVersion", "en{'Version'}de{'Version'}", "", "", true)]
    [ACPropertyEntity(4, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(6, "TargetQuantityUOM", "en{'Reference Quantity (UOM)'}de{'Bezugsgröße (BME)'}", "", "", true)]
    [ACPropertyEntity(7, MDUnit.ClassName, "en{'Unit of Measurement'}de{'Maßeinheit'}", Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(8, "TargetQuantity", "en{'Reference Quantity'}de{'Bezugsgröße'}", "", "", true)]
    [ACPropertyEntity(10, "ProductionWeight", "en{'Production Weight'}de{'Produktionsgewicht'}", "", "", true)]
    [ACPropertyEntity(11, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(12, "EnabledFrom", "en{'Valid From'}de{'Gültig von'}", "", "", true)]
    [ACPropertyEntity(13, "EnabledTo", "en{'Valid To'}de{'Gültig bis'}", "", "", true)]
    [ACPropertyEntity(14, Const.IsEnabled, "en{'Enabled'}de{'Freigegeben'}", "", "", true)]
    [ACPropertyEntity(16, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(17, "IsInEnabledPeriod", "en{'Validity Period'}de{'Gültigkeitszeitraum'}", "", "", true)]
    [ACPropertyEntity(18, "Partslist1_PreviousPartslist", "en{'Based on'}de{'Basiert auf'}", Const.ContextDatabase + "\\" + Partslist.ClassName, "", true)]
    [ACPropertyEntity(19, "ViewPreviousPartslistName", "en{'Based on'}de{'Basiert auf'}", "", "", true)]
    [ACPropertyEntity(20, "ViewMDUnitName", "en{'Unit of Measurement'}de{'Maßeinheit'}", "", "", true)]
    [ACPropertyEntity(21, MaterialWF.ClassName, "en{'Material Workflow'}de{'Material Workflow'}", Const.ContextDatabase + "\\" + MaterialWF.ClassName, "", true)]
    [ACPropertyEntity(9999, "IsProductionUnit", "en{'Production Unit'}de{'Produktionseinheit'}", "", "", true)]
    [ACPropertyEntity(9999, "IsStandard", "en{'Standard'}de{'Standard'}", "", "", true)]
    [ACPropertyEntity(494, Const.EntityDeleteDate, Const.EntityTransDeleteDate)]
    [ACPropertyEntity(495, Const.EntityDeleteName, Const.EntityTransDeleteName)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + Partslist.ClassName, "en{'Bill of Materials'}de{'Stückliste'}", typeof(Partslist), Partslist.ClassName, "PartslistNo,PartslistName", "PartslistName", new object[]
        {
            new object[] {Const.QueryPrefix + PartslistPos.ClassName, "en{'Bill of Materials Position'}de{'Stücklistenposition'}", typeof(PartslistPos), PartslistPos.ClassName + "_" + Partslist.ClassName, "Sequence", "Sequence"}
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<Partslist>) })]
    public partial class Partslist : IACConfigStore, IPartslist
    {
        public const string ClassName = "Partslist";
        public const string NoColumnName = "PartslistNo";
        public const string FormatNewNo = "PL{0}";

        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(11020);

        #region New/Delete
        public static Partslist NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            Partslist entity = new Partslist();
            entity.PartslistID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.PartslistNo = secondaryKey;
            entity.PartslistVersion = "1";
            entity.IsEnabled = true;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }

        /// <summary>
        /// Create new partslist version 
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="parentACObject"></param>
        /// <param name="formatNewNo"></param>
        /// <returns></returns>
        public static Partslist PartsListNewVersionGet(DatabaseApp dbApp, IACObject parentACObject, Partslist partsListNewVersion, string formatNewNo = "PL{0}")
        {
            Partslist previusPartList = parentACObject as Partslist;
            partsListNewVersion.Partslist1_PreviousPartslist = previusPartList;
            partsListNewVersion.PartslistVersion = PartlistVersionCalculateNext(dbApp, previusPartList);

            // rewrite from previus partlist
            partsListNewVersion.PartslistName = previusPartList.PartslistName + " (#" + partsListNewVersion.PartslistVersion + ")";
            partsListNewVersion.MaterialID = previusPartList.MaterialID;
            partsListNewVersion.MDUnitID = previusPartList.MDUnitID;
            partsListNewVersion.TargetQuantityUOM = previusPartList.TargetQuantityUOM;
            //partsListNewVersion.TargetQuantity = previusPartList.TargetQuantity;
            partsListNewVersion.GrossWeight = previusPartList.GrossWeight;
            partsListNewVersion.IsEnabled = previusPartList.IsEnabled;
            partsListNewVersion.EnabledFrom = previusPartList.EnabledFrom;
            partsListNewVersion.EnabledTo = previusPartList.EnabledTo;
            partsListNewVersion.ProductionWeight = previusPartList.ProductionWeight;
            partsListNewVersion.IsStandard = previusPartList.IsStandard;
            partsListNewVersion.IsProductionUnit = previusPartList.IsProductionUnit;
            partsListNewVersion.MaterialWF = previusPartList.MaterialWF;
            partsListNewVersion.MaterialWF = previusPartList.MaterialWF;

            foreach (PartslistACClassMethod oldMehtod in previusPartList.PartslistACClassMethod_Partslist)
            {
                PartslistACClassMethod newMethod = PartslistACClassMethod.NewACObject(dbApp, partsListNewVersion);
                newMethod.MaterialWFACClassMethod = oldMehtod.MaterialWFACClassMethod;
                partsListNewVersion.PartslistACClassMethod_Partslist.Add(newMethod);
            }

            partsListNewVersion.IsDefault = false;
            previusPartList.IsDefault = true;

            dbApp.Partslist.AddObject(partsListNewVersion);

            List<PartslistPos> previusPartPosList = dbApp.PartslistPos.Where(x => x.PartslistID == previusPartList.PartslistID).ToList();
            List<Guid> previusPartPosIDList = previusPartPosList.Select(x => x.PartslistPosID).ToList();
            List<PartslistPosRelation> previousPartsPosRelationsList =
                dbApp.PartslistPosRelation.Where(x => previusPartPosIDList.Contains(x.TargetPartslistPosID) || previusPartPosIDList.Contains(x.SourcePartslistPosID)).ToList();

            foreach (var item in previusPartPosList)
            {
                PartslistPos newPartlistPos = PartslistPos.NewACObject(dbApp, partsListNewVersion);
                newPartlistPos.Sequence = item.Sequence;
                newPartlistPos.MaterialPosTypeIndex = item.MaterialPosTypeIndex;
                newPartlistPos.Material = dbApp.Material.FirstOrDefault(x => x.MaterialID == item.MaterialID);
                newPartlistPos.XMLConfig = item.XMLConfig;
                newPartlistPos.MDUnit = item.MDUnit;
                newPartlistPos.TargetQuantityUOM = item.TargetQuantityUOM;
                //newPartlistPos.TargetQuantity = item.TargetQuantity;
                newPartlistPos.AlternativePartslistPosID = item.AlternativePartslistPosID;
                newPartlistPos.LineNumber = item.LineNumber;
                item.NewVersion = newPartlistPos;
                dbApp.PartslistPos.AddObject(newPartlistPos);
            }

            foreach (var item in previousPartsPosRelationsList)
            {
                PartslistPosRelation newRelationItem = new PartslistPosRelation();
                newRelationItem.PartslistPosRelationID = Guid.NewGuid();
                newRelationItem.TargetQuantityUOM = item.TargetQuantityUOM;
                //newRelationItem.TargetQuantity = item.TargetQuantity;
                newRelationItem.MaterialWFRelationID = item.MaterialWFRelationID;
                newRelationItem.Sequence = item.Sequence;

                PartslistPos source = previusPartPosList.FirstOrDefault(x => x.PartslistPosID == item.SourcePartslistPosID);
                PartslistPos target = previusPartPosList.FirstOrDefault(x => x.PartslistPosID == item.TargetPartslistPosID);

                newRelationItem.SourcePartslistPos = source.NewVersion;
                newRelationItem.TargetPartslistPos = target.NewVersion;

                dbApp.PartslistPosRelation.AddObject(newRelationItem);
            }

            return partsListNewVersion;
        }

        #endregion

        #region IACUrl Member

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                return PartslistNo + " " + PartslistName;
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
            if (filterValues.Any())
            {
                switch (className)
                {
                    case PartslistPos.ClassName:
                        string[] filterParams = filterValues[0].Split(',');
                        int sequence = int.Parse(filterParams[0]);
                        int materialPosTypeIndex = int.Parse(filterParams[2]);
                        string materialNo = filterParams[1];
                        return PartslistPos_Partslist.FirstOrDefault(x => x.Sequence == sequence && x.MaterialPosTypeIndex == materialPosTypeIndex && x.Material.MaterialNo == materialNo);
                }
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
            if (string.IsNullOrEmpty(PartslistNo) || string.IsNullOrEmpty(PartslistName))
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

        public PartslistStock GetPartslistStock_InsertIfNotExists(DatabaseApp dbApp)
        {
            PartslistStock facilityStock = PartslistStock_Partslist.FirstOrDefault();
            if (facilityStock != null)
                return facilityStock;
            facilityStock = PartslistStock.NewACObject(dbApp, this);
            return facilityStock;
        }

        public PartslistStock CurrentPartslistStock
        {
            get
            {
                return PartslistStock_Partslist.FirstOrDefault();
            }
        }

        static public string KeyACIdentifier
        {
            get
            {
                return "PartslistNo,PartslistVersion";
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
            PartslistConfig acConfig = PartslistConfig.NewACObject(this.GetObjectContext<DatabaseApp>(), this);
            acConfig.KeyACUrl = ACConfigKeyACUrl;
            acConfig.LocalConfigACUrl = localConfigACUrl;
            acConfig.ValueTypeACClass = valueTypeACClass;
            PartslistConfig_Partslist.Add(acConfig);
            if (acObject != null && acObject is core.datamodel.ACClassWF)
                acConfig.VBiACClassWFID = (acObject as core.datamodel.ACClassWF).ACClassWFID;
            ACConfigListCache.Add(acConfig);
            return acConfig;
        }

        /// <summary>Removes a configuration from ConfigurationEntries and the database-context.</summary>
        /// <param name="acObject">Entry as IACConfig</param>
        public void RemoveACConfig(IACConfig acObject)
        {
            PartslistConfig acConfig = acObject as PartslistConfig;
            if (acConfig == null)
                return;
            if (ACConfigListCache.Contains(acConfig))
                ACConfigListCache.Remove(acConfig);
            PartslistConfig_Partslist.Remove(acConfig);
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
                (acConfig as PartslistConfig).DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
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
                if (PartslistConfig_Partslist.IsLoaded)
                {
                    PartslistConfig_Partslist.AutoRefresh();
                    PartslistConfig_Partslist.AutoLoad();
                }
                newSafeList = new SafeList<IACConfig>(PartslistConfig_Partslist.ToList().Select(x => (IACConfig)x));
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
                var query = database.PartslistConfig.Where(c => c.PartslistID == this.PartslistID);
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

        #region Others

        public gip.core.datamodel.ACClassMethod GetDefaultProgramACClassMethod()
        {
            //var query = this.PartslistProgram_Partslist;
            //if (query.Any())
            //    return query.First().ProgramACClassMethod.FromIPlusContext<core.datamodel.ACClassMethod>();
            return null;
        }


        /// <summary>
        /// Calculate new version of partslist in same level (PreviousPartsList)
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="previusPartList"></param>
        /// <returns></returns>
        private static string PartlistVersionCalculateNext(DatabaseApp dbApp, Partslist previusPartList)
        {
            List<string> allversionsInSameRange = dbApp.Partslist.Where(pl => (pl.PreviousPartslistID ?? (new Guid())) == previusPartList.PartslistID).Select(pl => pl.PartslistVersion).ToList();
            int maxVersion = 1;
            foreach (string version in allversionsInSameRange)
            {
                int currentVersion = 0;
                int.TryParse(version, out currentVersion);
                if (maxVersion < currentVersion)
                    maxVersion = currentVersion;
            }
            return (maxVersion + 1).ToString();
        }
        #endregion

        #region Additional propertyes - display purpurose

        /// <summary>
        /// Mapping partslist name
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Based on'}de{'Basiert auf'}")]
        public string ViewPreviousPartslistName
        {
            get
            {
                if (Partslist1_PreviousPartslist == null)
                    return null;
                return Partslist1_PreviousPartslist.PartslistName;
            }
        }
        /// <summary>
        /// Mappint MDUnit - Name
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Unit of measurement'}de{'Maßeinheit'}")]
        public string ViewMDUnitName
        {
            get
            {
                if (MDUnit == null)
                    return null;
                return MDUnit.MDUnitName;
            }
        }

        #endregion

        #region partial methods
        bool _OnTargetQuantityChanging = false;
        partial void OnTargetQuantityChanged()
        {
            if (!_OnTargetQuantityUOMChanging && Material != null && MDUnit != null)
            {
                _OnTargetQuantityChanging = true;
                try
                {
                    TargetQuantityUOM = Material.ConvertToBaseQuantity(TargetQuantity, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnTargetQuantityChanged", msg);
                }
                finally
                {
                    _OnTargetQuantityChanging = false;
                }
            }
        }

        bool _OnTargetQuantityUOMChanging = false;
        partial void OnTargetQuantityUOMChanged()
        {
            if (!_OnTargetQuantityChanging && Material != null && MDUnit != null)
            {
                _OnTargetQuantityUOMChanging = true;
                try
                {
                    TargetQuantity = Material.ConvertQuantity(TargetQuantityUOM, Material.BaseMDUnit, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnTargetQuantityUOMChanged", msg);
                }
                finally
                {
                    _OnTargetQuantityUOMChanging = false;
                }
            }
        }
        #endregion

        #region Convention implementation
        public override string ToString()
        {
            string materialName = "-";
            if (MaterialID != Guid.Empty && Material != null)
                materialName = Material.ToString();
            return PartslistNo + "/" + materialName;
        }
        #endregion
    }
}
