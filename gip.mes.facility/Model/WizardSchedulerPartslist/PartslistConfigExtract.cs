using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Collections.Generic;
using gipCoreData = gip.core.datamodel;

namespace gip.mes.facility
{
    public class PartslistConfigExtract
    {

        #region DI
        public ConfigManagerIPlus VarioConfigManager { get; set; }
        public ACProdOrderManager ProdOrderManager { get; set; }
        public Partslist Partslist { get; set; }
        public gipCoreData.ACClassWF WFNode { get; set; }
        public ACClassWF WFNodeMES { get; set; }

        public List<gipCoreData.IACConfigStore> MandatoryConfigStores { get; set; }
        #endregion

        public PartslistConfigExtract(ConfigManagerIPlus varioConfigManager, ACProdOrderManager prodOrderManager, Partslist partslist, gipCoreData.ACClassWF wfNode, ACClassWF wfNodeMES)
        {
            VarioConfigManager = varioConfigManager;
            ProdOrderManager = prodOrderManager;
            Partslist = partslist;
            WFNode = wfNode;
            WFNodeMES = wfNodeMES;
            MandatoryConfigStores = GetCurrentConfigStores();
        }

        public List<gipCoreData.IACConfigStore> GetCurrentConfigStores()
        {
            List<gipCoreData.IACConfigStore> configStores = new List<gipCoreData.IACConfigStore>();
            if (Partslist != null)
            {
                configStores.Add(Partslist);
                MaterialWFConnection matWFConnection = ProdOrderManager.GetMaterialWFConnection(WFNodeMES, Partslist.MaterialWFID);
                configStores.Add(matWFConnection.MaterialWFACClassMethod);
                configStores.Add(WFNode.ACClassMethod);
                if (WFNode.RefPAACClassMethod != null)
                    configStores.Add(WFNode.RefPAACClassMethod);
            }

            foreach (var item in configStores)
                item.ClearCacheOfConfigurationEntries();

            return configStores;
        }

        public gipCoreData.IACConfig GetConfig(string propertyName)
        {
            int priorityLevel = 0;
            string preValueACUrl = null; //(LocalBSOBatchPlan.CurrentPWInfo as IACConfigURL).PreValueACUrl
            string localConfigACUrl = WFNode.ConfigACUrl + @"\" + ACStateConst.SMStarting.ToString() + @"\" + propertyName;
            gipCoreData.IACConfig aCConfig = VarioConfigManager.GetConfiguration(MandatoryConfigStores, preValueACUrl, localConfigACUrl, null, out priorityLevel);
            return aCConfig;
        }
    }
}
