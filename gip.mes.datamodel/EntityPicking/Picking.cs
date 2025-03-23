using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.Picking, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOPicking")]
    [ACPropertyEntity(1, "PickingNo", ConstApp.PickingNo, "", "", true)]
    [ACPropertyEntity(2, "PickingTypeIndex", "en{'Picking Type'}de{'Kommissioniertyp'}", typeof(GlobalApp.PickingType), Const.ContextDatabase + "\\PickingTypeList", "", true)]
    [ACPropertyEntity(3, "PickingStateIndex", "en{'Picking Status'}de{'Status'}", typeof(PickingStateEnum), Const.ContextDatabase + "\\PickingStateList", "", true)]
    [ACPropertyEntity(4, "DeliveryDateFrom", "en{'Date from'}de{'Datum von'}", "", "", true)]
    [ACPropertyEntity(5, "DeliveryDateTo", "en{'Date to'}de{'Datum bis'}", "", "", true)]
    [ACPropertyEntity(6, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(7, "Comment2", ConstApp.Comment2, "", "", true)]
    [ACPropertyEntity(8, VisitorVoucher.ClassName, "en{'Visitor Voucher'}de{'Besucherbeleg'}", Const.ContextDatabase + "\\" + VisitorVoucher.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9, gip.core.datamodel.ACClassMethod.ClassName, "en{'Workflow'}de{'Workflow'}", Const.ContextDatabase + "\\" + gip.core.datamodel.ACClassMethod.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(10, MDPickingType.ClassName, "en{'Picking type'}de{'Kommissionierung Typ'}", Const.ContextDatabase + "\\" + MDPickingType.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(11, "DeliveryCompanyAddress", "en{'Delivery Address'}de{'Lieferadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(12, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
    [ACPropertyEntity(16, "ScheduledOrder", "en{'Scheduled Order'}de{'Reihenfolge Plan'}", "", "", true)]
    [ACPropertyEntity(17, "ScheduledStartDate", "en{'Planned Start Date'}de{'Geplante Startzeit'}", "", "", true)]
    [ACPropertyEntity(18, "ScheduledEndDate", "en{'Planned End Date'}de{'Geplante Endezeit'}", "", "", true)]
    [ACPropertyEntity(19, "CalculatedStartDate", "en{'Calculated Start Date'}de{'Berechnete Startzeit'}", "", "", true)]
    [ACPropertyEntity(20, "CalculatedEndDate", "en{'Calculated End Date'}de{'Berechnete Endezeit'}", "", "", true)]
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
    [NotMapped]
    public partial class Picking : IACConfigStore, IScheduledOrder
    {
        [NotMapped]
        public const string ClassName = "Picking";
        [NotMapped]
        public const string NoColumnName = "PickingNo";
        [NotMapped]
        public const string FormatNewNo = "PK{0}";
        [NotMapped]
        public const string FormatNoForSupply = "{0}-{1:D4}";
        public const string FormatNoForOrderSupply = "{0}-{1}-{2:D4}";

        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(11020);

        #region New/Delete
        public static Picking NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            Picking entity = new Picking();
            entity.PickingID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDPickingType = MDPickingType.s_cQry_Default(dbApp).FirstOrDefault();
            entity.PickingState = PickingStateEnum.New;
            entity.PickingNo = secondaryKey;
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

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "PickingNo";
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

        #region others

        [NotMapped]
        public GlobalApp.PickingType PickingType
        {
            get
            {
                return (GlobalApp.PickingType)MDPickingType.MDPickingTypeIndex;
            }
        }

        [ACPropertyInfo(999)]
        [NotMapped]
        public PickingStateEnum PickingState
        {
            get
            {
                return (PickingStateEnum)PickingStateIndex;
            }
            set
            {
                PickingStateIndex = (Int16)value;
            }
        }

        private gip.core.datamodel.ACClassWF _IplusVBiACClassWF;
        [ACPropertyInfo(9999)]
        public gip.core.datamodel.ACClassWF IplusVBiACClassWF
        {
            get
            {
                if (VBiACClassWFID == null || VBiACClassWFID == Guid.Empty)
                    return null;
                if (this._IplusVBiACClassWF == null)
                {
                    DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                    using (ACMonitor.Lock(dbApp.ContextIPlus.QueryLock_1X000))
                    {
                        _IplusVBiACClassWF = dbApp.ContextIPlus.ACClassWF.Where(c => c.ACClassWFID == VBiACClassWFID).FirstOrDefault();
                    }
                }
                return _IplusVBiACClassWF;
            }
        }

        private bool _IsSelected;
        [ACPropertyInfo(999, nameof(IsSelected), Const.Select)]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }


        private PickingPreparationStatusEnum? _PreparationStatus;
        [ACPropertyInfo(999, nameof(PreparationStatus), ConstApp.PickingPreparationStatus)]
        public PickingPreparationStatusEnum? PreparationStatus
        {
            get
            {
                return _PreparationStatus;
            }
            set
            {
                _PreparationStatus = value;
                OnPropertyChanged(nameof(PreparationStatus));
            }
        }

        private string _PreparationStatusName;
        [ACPropertyInfo(999, nameof(PreparationStatusName), ConstApp.PickingPreparationStatus)]
        public string PreparationStatusName
        {
            get
            {
                return _PreparationStatusName;
            }
            set
            {
                _PreparationStatusName = value;
                OnPropertyChanged(nameof(PreparationStatusName));
            }
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
                (acConfig as PickingConfig).DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
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
                if (PickingConfig_Picking_IsLoaded)
                {
                    PickingConfig_Picking.AutoLoad(PickingConfig_PickingReference, this);
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

        #region Param State

        [NotMapped]
        private PreferredParamStateEnum _ParamState;
        [ACPropertyInfo(999, nameof(ParamState), ConstApp.PrefParam)]
        [NotMapped]
        public PreferredParamStateEnum ParamState
        {
            get
            {
                return _ParamState;
            }
            set
            {
                if (_ParamState != value)
                {
                    _ParamState = value;
                    OnPropertyChanged(nameof(ParamState));
                }
            }
        }

        [NotMapped]
        [ACPropertyInfo(999, nameof(ParamStateName), "en{'Param state name'}de{'Parameterstatusname'}")]
        public string ParamStateName
        {
            get
            {
                ACValueItem item = this.GetObjectContext<DatabaseApp>().PreferredParamStateList.Where(c => (PreferredParamStateEnum)c.Value == ParamState).FirstOrDefault();
                return item.ACCaption;
            }
        }

        #endregion

        #region Cloning
        public object Clone(bool withReferences)
        {
            Picking clonedObject = new Picking();
            clonedObject.PickingID = this.PickingID;
            clonedObject.CopyFrom(this, withReferences, PickingNo);
            return clonedObject;
        }

        public void CopyFrom(Picking from, bool withReferences, string pickingNo = null)
        {
            if (withReferences)
            {
                MDPickingTypeID = from.MDPickingTypeID;
                ACClassMethodID = from.ACClassMethodID;
                VisitorVoucherID = from.VisitorVoucherID;
                TourplanID = from.TourplanID;
                DeliveryCompanyAddressID = from.DeliveryCompanyAddressID;
            }

            if (!string.IsNullOrEmpty(pickingNo))
                PickingNo = pickingNo;
            PickingStateIndex = from.PickingStateIndex;
            DeliveryDateFrom = from.DeliveryDateFrom;
            DeliveryDateTo = from.DeliveryDateTo;
            Comment = from.Comment;
            XMLConfig = from.XMLConfig;
            KeyOfExtSys = from.KeyOfExtSys;
            ParamState = from.ParamState;
        }
        #endregion
    }
}
