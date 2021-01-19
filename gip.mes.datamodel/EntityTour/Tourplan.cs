using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Tour Plan'}de{'Tourenplanung'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOTourplan")]
    [ACPropertyEntity(1, "TourplanNo", "en{'Tour Plan No.'}de{'Tourenplan-Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "TourplanName", "en{'Tour Plan Name'}de{'Name'}", "", "", true)]
    [ACPropertyEntity(3, "VehicleFacility", "en{'Vehicle'}de{'Fahrzeug'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(4, "TrailerFacility", "en{'Trailer'}de{'Anhänger'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(5, Company.ClassName, "en{'Company'}de{'Firma'}", Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(6, MDTour.ClassName, "en{'Tour'}de{'Tour'}", Const.ContextDatabase + "\\" + MDTour.ClassName, "", true)]
    [ACPropertyEntity(7, "LoadingStation", "en{'Loading Station'}de{'Verladestation'}", "", "", true)]
    [ACPropertyEntity(8, "DeliveryDate", "en{'Delivery Date'}de{'Soll Lieferdatum'}", "", "", true)]
    [ACPropertyEntity(9, "ActDeliveryDate", "en{'Act.Delivery Date'}de{'Ist Lieferdatum'}", "", "", true)]
    [ACPropertyEntity(10, "VisitorCardFacilityCharge", "en{'Transponder Card'}de{'Transponderkarte'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName, "", true)]
    [ACPropertyEntity(11, "FirstWeighing", "en{'First Weighing'}de{'Erstwägung'}", "", "", true)]
    [ACPropertyEntity(12, "FirstWeighingIdentityNo", "en{'First Weighing No'}de{'Erstwägung Nr.'}", "", "", true)]
    [ACPropertyEntity(13, "SecondWeighing", "en{'Second Weighing'}de{'Zweitwägung'}", "", "", true)]
    [ACPropertyEntity(14, "SecondWeighingIdentityNo", "en{'Second Weighing No'}de{'Zweit Wiegungsnr'}", "", "", true)]
    [ACPropertyEntity(15, "LastWeighing", "en{'Last Weighing'}de{'Letzte Wägung'}", "", "", true)]
    [ACPropertyEntity(16, "LastWeighingIdentityNo", "en{'Last Weighing No.'}de{'Letzte Wägung Nr.'}", "", "", true)]
    [ACPropertyEntity(17, "MaxCapacityDiff", "en{'Max.Capacity Diff.'}de{'Max.Kapazitätsdifferenz'}", "", "", true)]
    [ACPropertyEntity(18, "MaxCapacitySum", "en{'Max.Capacity Sum.'}de{'Max.Kapazität'}", "", "", true)]
    [ACPropertyEntity(19, "MaxWeightDiff", "en{'Max.Weight Diff.'}de{'Max.Gewichtsdifferenz'}", "", "", true)]
    [ACPropertyEntity(20, "MaxWeightSum", "en{'Max.Weight Sum.'}de{'Max.Gewicht'}", "", "", true)]
    [ACPropertyEntity(21, MDTourplanState.ClassName, "en{'Picking Status'}de{'Kommissionierstatus'}", Const.ContextDatabase + "\\" + MDTourplanState.ClassName, "", true)]
    [ACPropertyEntity(22, "NetWeight", "en{'Net Weight'}de{'Nettogewicht'}", "", "", true)]
    [ACPropertyEntity(23, "NightLoading", "en{'Overnight Loading'}de{'Nachtverladung'}", "", "", true)]
    [ACPropertyEntity(24, "PeriodInt", "en{'Period'}de{'Periode'}", "", "", true)]
    [ACPropertyEntity(32, VisitorVoucher.ClassName, "en{'Visitor Voucher'}de{'Besucher Beleg'}", Const.ContextDatabase + "\\" + VisitorVoucher.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + Tourplan.ClassName, "en{'Tour Plan'}de{'Tourenplanung'}", typeof(Tourplan), Tourplan.ClassName, "TourplanNo", "TourplanNo", new object[]
        {
                new object[] {Const.QueryPrefix + TourplanPos.ClassName, "en{'Tour Plan Pos.'}de{'Tourplan Pos.'}", typeof(TourplanPos), TourplanPos.ClassName + "_" + Tourplan.ClassName, Const.SortIndex, Const.SortIndex}
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<Tourplan>) })]
    public partial class Tourplan : IACConfigStore
    {
        public const string ClassName = "Tourplan";
        public const string NoColumnName = "TourplanNo";
        public const string FormatNewNo = "TP{0}";

        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(11020);

        #region New/Delete
        public static Tourplan NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            Tourplan entity = new Tourplan();
            entity.TourplanID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.TourplanNo = secondaryKey;
            entity.TourplanName = entity.TourplanNo;
            entity.DeliveryDate = DateTime.Now;
            entity.MDTourplanState = MDTourplanState.DefaultMDTourplanState(dbApp);
            entity.MDTour = dbApp.MDTour.First();
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
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
                return TourplanNo + " " + TourplanName;
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
            if (filterValues.Any() && className == TourplanPos.ClassName)
            {
                Int16 sortIndex = 0;
                if (Int16.TryParse(filterValues[0], out sortIndex))
                    return this.TourplanPos_Tourplan.Where(c => c.SortIndex == sortIndex).FirstOrDefault();
            }
            return null;
        }

        #endregion

        #region IACObjectEntity Members
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            if (string.IsNullOrEmpty(TourplanNo) || string.IsNullOrEmpty(TourplanName))
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
                return "TourplanName";
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
            TourplanConfig TourplanConfig = TourplanConfig.NewACObject(this.GetObjectContext<DatabaseApp>(), this);
            TourplanConfig.KeyACUrl = ACConfigKeyACUrl;
            TourplanConfig.ValueTypeACClass = valueTypeACClass;
            //if (acObject != null && acObject is core.datamodel.ACClassWF)
            //    TourplanConfig.VBiACClassWFID = ((core.datamodel.ACClassWF)acObject).ACClassWFID;
            TourplanConfig_Tourplan.Add(TourplanConfig);
            ACConfigListCache.Add(TourplanConfig);
            return TourplanConfig;
        }

        /// <summary>Removes a configuration from ConfigurationEntries and the database-context.</summary>
        /// <param name="acObject">Entry as IACConfig</param>
        public void RemoveACConfig(IACConfig acObject)
        {
            TourplanConfig acConfig = acObject as TourplanConfig;
            if (acConfig == null)
                return;
            if (ACConfigListCache.Contains(acConfig))
                ACConfigListCache.Remove(acConfig);
            TourplanConfig_Tourplan.Remove(acConfig);
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
                (acConfig as TourplanConfig).DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
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
                if (TourplanConfig_Tourplan.IsLoaded)
                {
                    TourplanConfig_Tourplan.AutoRefresh();
                    TourplanConfig_Tourplan.AutoLoad();
                }
                newSafeList = new SafeList<IACConfig>(TourplanConfig_Tourplan.ToList().Select(x => (IACConfig)x));
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

        #endregion


    }
}
