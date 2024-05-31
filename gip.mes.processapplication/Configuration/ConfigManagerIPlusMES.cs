using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Config Manager for MES'}de{'Config Manager for MES'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class ConfigManagerIPlusMES : ConfigManagerIPlus
    {
        #region c´tors
        public ConfigManagerIPlusMES(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion

        #region IACConfigProvider (and overrides)

        public override List<IACConfigStore> GetACConfigStores(List<IACConfigStore> callingConfigStoreList)
        {
            List<IACConfigStore> resultList = base.GetACConfigStores(callingConfigStoreList);
            IACConfigStore configStore = callingConfigStoreList.Where(x => x is Picking).FirstOrDefault();
            if (configStore != null)
            {
                resultList = GetACConfigStoresImplementation((configStore as Picking), callingConfigStoreList);
            }
            configStore = callingConfigStoreList.Where(x => x is ProdOrderPartslist).FirstOrDefault();
            if (configStore != null)
            {
                resultList = GetACConfigStoresImplementation((configStore as ProdOrderPartslist), callingConfigStoreList);
                return resultList;
            }
            configStore = callingConfigStoreList.Where(x => x is Partslist).FirstOrDefault();
            if (configStore != null)
            {
                resultList = GetACConfigStoresImplementation((configStore as Partslist), callingConfigStoreList);
                return resultList;
            }
            configStore = callingConfigStoreList.Where(x => x is MaterialWFACClassMethod).FirstOrDefault();
            if (configStore != null)
            {
                resultList = GetACConfigStoresImplementation((configStore as MaterialWFACClassMethod), callingConfigStoreList);
            }
            return resultList;
        }

        public override void DeleteConfigNode(IACEntityObjectContext db, Guid acClassWFID)
        {
            var dbApp = db as DatabaseApp;
            if (dbApp != null)
            {
                List<PickingConfig> picikingConfigs = dbApp.PickingConfig.Where(c => c.VBiACClassWFID == acClassWFID).ToList();
                picikingConfigs.ForEach(x => x.DeleteACObject(dbApp, false));

                List<MaterialWFACClassMethodConfig> materialWFACClassMethodConfigs = dbApp.MaterialWFACClassMethodConfig.Where(c => c.VBiACClassWFID == acClassWFID).ToList();
                materialWFACClassMethodConfigs.ForEach(x => x.DeleteACObject(dbApp, false));

                List<PartslistConfig> partslistConfigs = dbApp.PartslistConfig.Where(c => c.VBiACClassWFID == acClassWFID).ToList();
                partslistConfigs.ForEach(x => x.DeleteACObject(dbApp, false));

                List<ProdOrderPartslistConfig> prodOrderPartslistConfig = dbApp.ProdOrderPartslistConfig.Where(c => c.VBiACClassWFID == acClassWFID).ToList();
                prodOrderPartslistConfig.ForEach(x => x.DeleteACObject(dbApp, false));

            }
            base.DeleteConfigNode(db, acClassWFID);
        }

        #endregion

        #region Implementation of overriding confing by this class

        private List<IACConfigStore> GetACConfigStoresImplementation(Picking picking, List<IACConfigStore> callingConfigStoreList)
        {
            if (picking == null)
                return callingConfigStoreList;
            if (!callingConfigStoreList.Any(x => x is Picking))
            {
                callingConfigStoreList.Add(picking);
            }
            picking.OverridingOrder = 2 + GetCallingMethodCount(callingConfigStoreList);
            return callingConfigStoreList;
        }

        private List<IACConfigStore> GetACConfigStoresImplementation(ProdOrderPartslist prodOrderPartslist, List<IACConfigStore> callingConfigStoreList)
        {
            if (prodOrderPartslist == null)
                return callingConfigStoreList;
            if (!callingConfigStoreList.Any(x => x is ProdOrderPartslist))
            {
                callingConfigStoreList.Add(prodOrderPartslist);
            }
            prodOrderPartslist.OverridingOrder = 5 + GetCallingMethodCount(callingConfigStoreList);
            Partslist partslist = callingConfigStoreList.Any(x => (x is Partslist) && (x as Partslist).PartslistID == prodOrderPartslist.PartslistID) ?
                (Partslist)callingConfigStoreList.FirstOrDefault(x => (x is Partslist) && (x as Partslist).PartslistID == prodOrderPartslist.PartslistID) : prodOrderPartslist.Partslist;
            return GetACConfigStoresImplementation(partslist, callingConfigStoreList);
        }

        private List<IACConfigStore> GetACConfigStoresImplementation(Partslist partslist, List<IACConfigStore> callingConfigStoreList)
        {
            if (partslist == null)
                return callingConfigStoreList;
            if (!callingConfigStoreList.Any(x => x is Partslist))
            {
                callingConfigStoreList.Add(partslist);
            }
            MaterialWF materialWF = callingConfigStoreList.Any(x => (x is MaterialWF) && (x as MaterialWF).MaterialWFID == partslist.MaterialWFID) ?
                (MaterialWF)callingConfigStoreList.FirstOrDefault(x => (x is MaterialWF) && (x as MaterialWF).MaterialWFID == partslist.MaterialWFID) : partslist.MaterialWF;
            if (materialWF != null)
                callingConfigStoreList = GetACConfigStoresImplementation(materialWF, callingConfigStoreList);
            partslist.OverridingOrder = 4 + GetCallingMethodCount(callingConfigStoreList);
            return callingConfigStoreList;
        }

        private List<IACConfigStore> GetACConfigStoresImplementation(MaterialWF materialWF, List<IACConfigStore> callingConfigStoreList)
        {
            MaterialWFACClassMethod mwfACMethod = null;
            if (materialWF == null || callingConfigStoreList == null)
                return callingConfigStoreList;
            List<Guid> methodCaller = callingConfigStoreList.Where(x => x is gip.core.datamodel.ACClassMethod).Select(x => x as gip.core.datamodel.ACClassMethod).Select(x => x.ACClassMethodID).ToList();
            mwfACMethod = (MaterialWFACClassMethod)callingConfigStoreList.FirstOrDefault(x => (x is MaterialWFACClassMethod)
                && (x as MaterialWFACClassMethod).MaterialWFID == materialWF.MaterialWFID
                && methodCaller.Contains((x as MaterialWFACClassMethod).ACClassMethodID));
            if (mwfACMethod == null)
                mwfACMethod = materialWF.MaterialWFACClassMethod_MaterialWF.Where(x => x.MaterialWFID == materialWF.MaterialWFID &&
                    methodCaller.Contains(x.ACClassMethodID)).FirstOrDefault();
            if (callingConfigStoreList.Contains(materialWF))
                callingConfigStoreList.Remove(materialWF);
            if (mwfACMethod == null)
                return callingConfigStoreList;
            return GetACConfigStoresImplementation(mwfACMethod, callingConfigStoreList);
        }

        private List<IACConfigStore> GetACConfigStoresImplementation(MaterialWFACClassMethod materialWFACClassMethod, List<IACConfigStore> callingConfigStoreList)
        {
            if (materialWFACClassMethod == null)
                return callingConfigStoreList;
            if (!callingConfigStoreList.Any(x => x is MaterialWFACClassMethod))
            {
                callingConfigStoreList.Add(materialWFACClassMethod);
            }
            materialWFACClassMethod.OverridingOrder = 2 + GetCallingMethodCount(callingConfigStoreList);
            return callingConfigStoreList;
        }


        #endregion

        #region other methods

        public List<IACConfigStore> GetProductionPartslistConfigStoreOfflineList(Guid acClassTaskID, Guid acClassMethodID, out int expectedConfigStoresCount, out string message)
        {
            expectedConfigStoresCount = 3;
            message = "";
            MaterialWFACClassMethod configStageMatWF = null;
            Partslist configStagePartslist = null;
            ProdOrderPartslist configStageProdPartslist = null;
            List<IACConfigStore> mandatoryConfigStores = new List<IACConfigStore>();
            using (var dbApp = new DatabaseApp())
            {
                gip.mes.datamodel.ACClassTask task = dbApp.ACClassTask.FirstOrDefault(x => x.ACClassTaskID == acClassTaskID);
                if (task == null)
                {
                    message = String.Format("ACClassTask-Object doesn't exist in Database with ACClassTaskID {0}", acClassTaskID);
                    Messages.LogError(this.GetACUrl(), "GetProductionPartslistConfigStoreOfflineList()", message);
                    return null;
                }

                Guid prodOrderPartslistposID = task.ProdOrderPartslistPos_ACClassTask.FirstOrDefault().ProdOrderPartslistID;
                configStageProdPartslist = dbApp.ProdOrderPartslist
                    .Include("ProdOrderPartslistConfig_ProdOrderPartslist")
                    .Include(Partslist.ClassName)
                    .Include("Partslist.PartslistConfig_Partslist")
                    .Include("Partslist.MaterialWF")
                    .Where(c => c.ProdOrderPartslistID == prodOrderPartslistposID).FirstOrDefault();
                if (configStageProdPartslist != null)
                {
                    configStagePartslist = configStageProdPartslist.Partslist;
                    if (configStagePartslist != null && configStagePartslist.MaterialWFID.HasValue)
                    {
                        configStageMatWF = dbApp.MaterialWFACClassMethod.Include(c => c.MaterialWFACClassMethodConfig_MaterialWFACClassMethod)
                            .Where(c => c.MaterialWFID == configStagePartslist.MaterialWFID
                                && c.ACClassMethodID == acClassMethodID).FirstOrDefault();
                    }
                }

                IEnumerable<IACConfig> cfSource = null;
                if (configStageMatWF != null)
                    cfSource = configStageMatWF.ConfigurationEntries; // Read Cache, because afterwards config-entities will be detached
                if (configStagePartslist != null)
                    cfSource = configStagePartslist.ConfigurationEntries; // Read Cache, because afterwards config-entities will be detached
                if (configStageProdPartslist != null)
                    cfSource = configStageProdPartslist.ConfigurationEntries; // Read Cache, because afterwards config-entities will be detached
                dbApp.DetachAllEntitiesAndDispose(true, false);
            }

            if (configStageMatWF != null)
                mandatoryConfigStores.Add(configStageMatWF);
            else
                expectedConfigStoresCount--;
            if (configStagePartslist != null)
                mandatoryConfigStores.Add(configStagePartslist);
            if (configStageProdPartslist != null)
            {
                configStageProdPartslist.OverridingOrder = 1;
                mandatoryConfigStores.Add(configStageProdPartslist);
            }
            return mandatoryConfigStores;
        }

        public List<IACConfigStore> GetPickingConfigStoreOfflineList(Guid acClassTaskID, Guid pickingID, out int expectedConfigStoresCount, out string message)
        {
            expectedConfigStoresCount = 1;
            message = "";
            Picking configStagePicking = null;

            List<IACConfigStore> mandatoryConfigStores = new List<IACConfigStore>();

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                gip.mes.datamodel.ACClassTask task = dbApp.ACClassTask.FirstOrDefault(x => x.ACClassTaskID == acClassTaskID);
                if (task == null)
                {
                    message = String.Format("ACClassTask-Object doesn't exist in Database with ACClassTaskID {0}", acClassTaskID);
                    Messages.LogError(this.GetACUrl(), "GetPickingConfigStoreOfflineList()", message);
                    return null;
                }

                configStagePicking = dbApp.Picking.Include("PickingConfig_Picking").FirstOrDefault(c => c.PickingID == pickingID);

                IEnumerable<IACConfig> cfSource = null;
                if (configStagePicking != null)
                    cfSource = configStagePicking.ConfigurationEntries;

                dbApp.DetachAllEntitiesAndDispose(true, false);
            }

            if (configStagePicking != null)
            {
                configStagePicking.OverridingOrder = 1;
                mandatoryConfigStores.Add(configStagePicking);
            }

            return mandatoryConfigStores;
        }

        public override List<IACConfigStore> AttachConfigStoresToDatabase(IACEntityObjectContext db, List<ACConfigStoreInfo> rmiResult)
        {
            DbContext obContext = db as DbContext;
            List<IACConfigStore> mandatoryConfigStore = base.AttachConfigStoresToDatabase(db, rmiResult);
            List<ACConfigStoreInfo> subsetRMIresult = rmiResult.Where(x => !mandatoryConfigStore.Select(a => (a as VBEntityObject).EntityKey).Contains(x.ConfigStoreEntity)).ToList();
            if (rmiResult != null)
            {
                foreach (ACConfigStoreInfo configStoreInfo in subsetRMIresult)
                {
                    IACConfigStore dbConfigStoreItem = obContext.Find<IACConfigStore>(configStoreInfo.ConfigStoreEntity);
                    if (dbConfigStoreItem != null)
                    {
                        dbConfigStoreItem.OverridingOrder = configStoreInfo.Priority;
                        mandatoryConfigStore.Add(dbConfigStoreItem);
                    }
                }
            }
            return mandatoryConfigStore;
        }

        #endregion

        #region QueryAllCOnfigs

        public override List<IACConfig> QueryAllCOnfigs(IACEntityObjectContext db, IACConfigStore sameConfigStore, string preConfigACUrl, string localConfigACUrl, Guid? vbiACClassID)
        {
            var dbApp = db as DatabaseApp;
            List<IACConfig> result = null;
            if (sameConfigStore.GetType().Name.StartsWith("AC"))
                return base.QueryAllCOnfigs(db, sameConfigStore, preConfigACUrl, localConfigACUrl, vbiACClassID);
            switch (sameConfigStore.GetType().Name)
            {
                case "Picking":
                    result =
                        ACConfigQuery<PickingConfig>.QueryConfigSource(dbApp.PickingConfig, preConfigACUrl, localConfigACUrl, vbiACClassID)
                        .ToList()
                        .Select(c => (IACConfig)c)
                        .ToList();
                    break;
                case "MaterialWFACClassMethod":
                    result =
                        ACConfigQuery<MaterialWFACClassMethodConfig>.QueryConfigSource(dbApp.MaterialWFACClassMethodConfig, preConfigACUrl, localConfigACUrl, vbiACClassID)
                        .ToList()
                        .Select(c => (IACConfig)c)
                        .ToList();
                    break;
                case "Partslist":
                    result =
                        ACConfigQuery<PartslistConfig>.QueryConfigSource(dbApp.PartslistConfig, preConfigACUrl, localConfigACUrl, vbiACClassID)
                        .ToList()
                        .Select(c => (IACConfig)c)
                        .ToList();
                    break;
                case "ProdOrderPartslist":
                    result = ACConfigQuery<ProdOrderPartslistConfig>.QueryConfigSource(dbApp.ProdOrderPartslistConfig, preConfigACUrl, localConfigACUrl, vbiACClassID)
                        .ToList()
                        .Select(c => (IACConfig)c)
                        .ToList();
                    break;
            }
            return result;
        }
        #endregion


        #region Planning

        public override bool HasPlanning(IACEntityObjectContext db, IACConfigStore configStore, Guid acClassWFID)
        {
            bool havePlanning = false;
            var dbApp = db as DatabaseApp;
            if (dbApp != null && configStore != null)
            {
                if (configStore is ProdOrderPartslist)
                {
                    ProdOrderPartslist prodOrderPartslist = configStore as ProdOrderPartslist;
                    havePlanning = dbApp.ProdOrderBatchPlan.Where(c => c.VBiACClassWFID == acClassWFID && c.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID).Any();
                }
                else if (configStore is Picking)
                {
                    Picking picking = configStore as Picking;
                    havePlanning = dbApp.Picking.Where(c => c.VBiACClassWFID == acClassWFID && c.PickingID == picking.PickingID).Any();
                }
            }
            return havePlanning;
        }

        #endregion

    }
}
