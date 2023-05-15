using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'ProductionorderBillOfMaterials'}de{'Produktionsauftragsstückliste'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sort'}de{'Sortierung'}", "", "", true)]
    [ACPropertyEntity(2, "Partslist", "en{'Bill of Materials Master'}de{'Stamm-Stückliste'}", Const.ContextDatabase + "\\" + Partslist.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, ProdOrder.ClassName, "en{'Production Order'}de{'Produktionsauftrag'}", Const.ContextDatabase + "\\" + ProdOrder.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, MDProdOrderState.ClassName, "en{'Production Status'}de{'Produktionsstatus'}", Const.ContextDatabase + "\\" + MDProdOrderState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, "DepartmentUserDate", "en{'Ended by User on'}de{'Beendet von Bediener am'}", "", "", true)]
    [ACPropertyEntity(6, "DepartmentUserName", ConstApp.DepartmentUserName, "", "", true)]
    [ACPropertyEntity(7, "StartDate", "en{'Production Start'}de{'Produktionsstart'}", "", "", true)]
    [ACPropertyEntity(8, "EndDate", "en{'Production End'}de{'Produktionsende'}", "", "", true)]
    [ACPropertyEntity(9, Const.IsEnabled, "en{'Enabled'}de{'Freigegeben'}", "", "", true)]
    [ACPropertyEntity(11, "LossComment", "en{'Loss Comment'}de{'Schwundbemerkung'}", "", "", true)]
    [ACPropertyEntity(12, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(13, "ActualQuantity", ConstApp.ActualQuantity, "", "", true)]
    [ACPropertyEntity(14, "ExternProdOrderNo", "en{'Ext prod. ord. No'}de{'Ext FANr'}", "", "", true)]
    [ACPropertyEntity(15, "LastFormulaChange", "en{'Last formula change'}de{'Letzte Formeländerung'}", "", "", true)]

    [ACPropertyEntity(16, nameof(ActualQuantityScrapUOM), "en{'Scrapped Quantity'}de{'Ausschussmenge'}", "", "", true)]

    [ACPropertyEntity(17, nameof(InputQForActualOutputPer), ConstIInputQForActual.InputQForActualOutputPer, "", "", true)]
    [ACPropertyEntity(18, nameof(InputQForGoodActualOutputPer), ConstIInputQForActual.InputQForGoodActualOutputPer, "", "", true)]
    [ACPropertyEntity(19, nameof(InputQForScrapActualOutputPer), ConstIInputQForActual.InputQForScrapActualOutputPer, "", "", true)]

    [ACPropertyEntity(20, nameof(InputQForFinalActualOutputPer), ConstIInputQForActual.InputQForFinalActualOutputPer, "", "", true)]
    [ACPropertyEntity(21, nameof(InputQForFinalGoodActualOutputPer), ConstIInputQForActual.InputQForFinalGoodActualOutputPer, "", "", true)]
    [ACPropertyEntity(22, nameof(InputQForFinalScrapActualOutputPer), ConstIInputQForActual.InputQForFinalScrapActualOutputPer, "", "", true)]

    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]

    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + ProdOrderPartslist.ClassName, ConstApp.ProdOrderPartslist, typeof(ProdOrderPartslist), ProdOrderPartslist.ClassName, "PartslistNo", "PartslistNo", new object[]
        {
            new object[] {Const.QueryPrefix + ProdOrderPartslistPos.ClassName, "en{'Bill of Materials Position'}de{'Stücklistenposition'}", typeof(ProdOrderPartslistPos), ProdOrderPartslistPos.ClassName + "_" + ProdOrderPartslist.ClassName, "Sequence", "Sequence"}
            // TODO: Add ProdOrderPartslistConfig
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<ProdOrderPartslist>) })]
    [NotMapped]
    public partial class ProdOrderPartslist : IACObjectEntity, IACConfigStore, IPartslist
    {
        [NotMapped]
        public const string ClassName = "ProdOrderPartslist";
        //public const string NoColumnName = "LotNo";
        [NotMapped]
        public const string FormatNewNo = "PPL{0}";
        [NotMapped]
        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(11020);

        #region New/Delete
        public static ProdOrderPartslist NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            ProdOrderPartslist entity = new ProdOrderPartslist();
            entity.ProdOrderPartslistID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is ProdOrder)
            {
                entity.ProdOrder = parentACObject as ProdOrder;
                entity.ProdOrder.ProdOrderPartslist_ProdOrder.Add(entity);
            }
            //if (!String.IsNullOrEmpty(formatNewNo))
            //    entity.PartslistNo = String.Format(formatNewNo,NoConfigurationManager.GetNewNo("PartslistNo"));
            entity.DepartmentUserName = "";
            entity.LossComment = "";
            entity.ProdUserEndName = "";
            entity.MDProdOrderState = MDProdOrderState.DefaultMDProdOrderState(dbApp);
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                if (Partslist != null)
                    return Partslist.PartslistNo;
                return "";
            }
        }

        /// <summary>
        /// Returns ProdOrder
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to ProdOrder</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return ProdOrder;
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
                string[] filterParams = filterValues[0].Split(',');
                switch (className)
                {
                    case ProdOrderPartslistPos.ClassName:
                        int sequence = int.Parse(filterParams[0]);
                        short materialPosTypeIndex = short.Parse(filterParams[2]);
                        return
                                ProdOrderPartslistPos_ProdOrderPartslist
                                .Where(c =>
                                    c.Sequence == sequence
                                    && c.Material.MaterialNo == filterParams[1]
                                    && c.MaterialPosTypeIndex == materialPosTypeIndex
                                    ).FirstOrDefault();
                }
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
                return "Partslist\\PartslistNo,Partslist\\PartslistVersion,Sequence";
            }
        }


        #endregion

        #region IACObjectDesign Member
        public IACComponentDesignManager GetDesignManager(IACComponent acComponent, string vbContentDesign)
        {
            string instanceACName = "VBBSODesignManagerWFProdOrderPartslist(" + vbContentDesign + ")";

            IACComponentDesignManager designManager = null;
            var query = acComponent.ACComponentChilds.Where(c => c is IACComponentDesignManager && c.ACIdentifier == instanceACName);
            if (query.Any())
            {
                designManager = query.First() as IACComponentDesignManager;
            }
            else
            {
                designManager = acComponent.StartComponent(instanceACName, null, null) as IACComponentDesignManager;
                //designManager.InitDesignManager(vbContentDesign);
                //_DesignManagerWFACClassWF.OnDroppedElement += new VBDesignerWorkflow.DroppedElementEventHandler(_ACClassWFManager_OnDroppedElement);
            }
            return designManager;
        }
        #endregion

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(TargetQuantity):
                    base.OnPropertyChanged("DifferenceQuantity");
                    base.OnPropertyChanged("TargetQuantityLossFactor");
                    break;
                case nameof(ActualQuantity):
                    base.OnEntityPropertyChanged("DifferenceQuantity");
                    break;
            }
            base.OnPropertyChanged(propertyName);
        }

        #region AdditionalProperties

        [ACPropertyInfo(24, "", "en{'Difference Quantity'}de{'Differenzmenge'}")]
        [NotMapped]
        public double DifferenceQuantity
        {
            get
            {
                return ActualQuantity - TargetQuantity;
            }
        }

        [ACPropertyInfo(25, "", "en{'Good Quantity'}de{'Gutmenge'}")]
        [NotMapped]
        public double ActualQuantityGoodUOM
        {
            get
            {
                return ActualQuantity - ActualQuantityScrapUOM;
            }
        }

        [ACPropertyInfo(999, "", "en{'Sum Components'}de{'Summe Komponenten'}")]
        [NotMapped]
        public double OutwardRootTargetQuantitySum
        {
            get
            {
                if (this.ProdOrderPartslistPos_ProdOrderPartslist == null || !this.ProdOrderPartslistPos_ProdOrderPartslist.Any()) return 0;
                return this
                    .ProdOrderPartslistPos_ProdOrderPartslist
                    .Where(x => x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.OutwardRoot && x.AlternativeProdOrderPartslistPosID == null)
                    .Sum(x => x.TargetQuantity);
            }
        }

        [ACPropertyInfo(999, "", "en{'Loss Factor'}de{'Verlustfaktor'}")]
        [NotMapped]
        public double TargetQuantityLossFactor
        {
            get
            {
                if (OutwardRootTargetQuantitySum == 0) return 0;
                return TargetQuantity / OutwardRootTargetQuantitySum;
            }
        }


        internal void OnPostionTargetQuantityChanged()
        {
            OnPropertyChanged("TargetQuantityLossFactor");
        }

        public bool IsFinalProdOrderPartslist
        {
            get
            {
                return !this.ProdOrderPartslistPos_SourceProdOrderPartslist.Any();
            }
        }

        [NotMapped]
        public ProdOrderPartslistPos FinalIntermediate
        {
            get
            {
                return ProdOrderPartslistPos_ProdOrderPartslist
                .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                .AsEnumerable()
                .Where(x => x.IsFinalMixure)
                .FirstOrDefault();
            }
        }
        #endregion

        #region Methods

        public void RecalcActualQuantity(bool recalcChilds = false)
        {
            this.ProdOrderPartslistPos_ProdOrderPartslist.AutoLoad(this.ProdOrderPartslistPos_ProdOrderPartslistReference, this);

            List<ProdOrderPartslistPos> intermediates = ProdOrderPartslistPos_ProdOrderPartslist
                .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                .ToList();

            List<ProdOrderPartslistPos> outwards = ProdOrderPartslistPos_ProdOrderPartslist
                .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot)
                .ToList();

            if (recalcChilds)
            {
                foreach (ProdOrderPartslistPos inwardPos in intermediates)
                {
                    inwardPos.RecalcActualQuantity();
                }
                foreach (ProdOrderPartslistPos outwardPos in outwards)
                {
                    outwardPos.RecalcActualQuantity();
                }
            }

            ProdOrderPartslistPos finallProduct = intermediates.Where(x => x.IsFinalMixure)
                .FirstOrDefault();
            if (finallProduct != null)
            {
                ActualQuantity = finallProduct.ActualQuantity;
            }
        }

        /// <summary>
        /// Recalc via Stored Procedure (cca 2 sec for ProgNo=3163)
        /// </summary>
        /// <param name="dbApp"></param>
        public void RecalcActualQuantitySP(DatabaseApp dbApp, bool callRefresh = true)
        {
            dbApp.Database.ExecuteSql(FormattableStringFactory.Create("udpRecalcActualQuantity @p0, @p1", ProdOrder.ProgramNo, ProdOrderPartslistID));
            if (callRefresh)
                RefreshAfterRecalcActualQuantity(dbApp);
        }

        public void RefreshAfterRecalcActualQuantity(DatabaseApp dbApp)
        {
            this.Refresh(dbApp);
            foreach (var pos in ProdOrderPartslistPos_ProdOrderPartslist)
            {
                pos.Refresh(dbApp);
                foreach (var rel in pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                {
                    rel.Refresh(dbApp);
                }
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
                return null;// ".\\ProdOrderPartslist(" + this.ACIdentifier + ")";
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
            ProdOrderPartslistConfig acConfig = ProdOrderPartslistConfig.NewACObject(this.GetObjectContext<DatabaseApp>(), this);
            acConfig.KeyACUrl = ACConfigKeyACUrl;
            acConfig.LocalConfigACUrl = localConfigACUrl;
            if (acObject != null && acObject is core.datamodel.ACClassWF)
                acConfig.VBiACClassWFID = ((core.datamodel.ACClassWF)acObject).ACClassWFID;
            acConfig.ValueTypeACClass = valueTypeACClass;
            ProdOrderPartslistConfig_ProdOrderPartslist.Add(acConfig);
            ACConfigListCache.Add(acConfig);
            return acConfig;
        }

        /// <summary>Removes a configuration from ConfigurationEntries and the database-context.</summary>
        /// <param name="acObject">Entry as IACConfig</param>
        public void RemoveACConfig(IACConfig acObject)
        {
            ProdOrderPartslistConfig acConfig = acObject as ProdOrderPartslistConfig;
            if (acConfig == null)
                return;
            if (ACConfigListCache.Contains(acConfig))
                ACConfigListCache.Remove(acConfig);
            ProdOrderPartslistConfig_ProdOrderPartslist.Remove(acConfig);
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
                (acConfig as ProdOrderPartslistConfig).DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
            }
            ClearCacheOfConfigurationEntries();
        }

        [NotMapped]
        public decimal OverridingOrder { get; set; }

        /// <summary>
        /// A thread-safe and cached list of Configuration-Values of type IACConfig.
        /// </summary>
        [ACPropertyInfo(999)]
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
                if (ProdOrderPartslistConfig_ProdOrderPartslist_IsLoaded)
                {
                    ProdOrderPartslistConfig_ProdOrderPartslist.AutoLoad(ProdOrderPartslistConfig_ProdOrderPartslistReference, this);
                }
                newSafeList = new SafeList<IACConfig>(ProdOrderPartslistConfig_ProdOrderPartslist.ToList().Select(x => (IACConfig)x));
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
                var query = database.ProdOrderPartslistConfig.Where(c => c.ProdOrderPartslistID == this.ProdOrderPartslistID);
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

        /// <summary>
        /// XAML-Code for Presentation
        /// </summary>
        /// <value>
        /// XAML-Code for Presentation
        /// </value>
        [NotMapped]
        public string XMLDesign
        {
            get;
            set;
        }

        public override string ToString()
        {
            if (ProdOrder == null || Partslist == null)
                return this.ProdOrderPartslistID.ToString();
            return ProdOrder.ToString() + "/#" + Sequence + "/" + Partslist.ToString();
        }
        #endregion
    }
}
