using gip.core.datamodel;
using VD = gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.autocomponent;

namespace gip.bso.masterdata
{
    public class WFGroupStartData
    {

        #region ctor's
        public WFGroupStartData(VD.DatabaseApp databaseApp, ConfigManagerIPlus iPlusMESConfigManager, Guid acClassWFID, Guid partslistID, Guid? prodOrderPartslistID)
        {
            InvokerPWNode = databaseApp.ContextIPlus.ACClassWF.Where(c => c.ACClassWFID == acClassWFID).FirstOrDefault();
            Method = databaseApp.ContextIPlus.ACClassMethod.FirstOrDefault(c => c.ACClassMethodID == InvokerPWNode.ACClassMethodID);
            Partslist = databaseApp.Partslist.FirstOrDefault(c => c.PartslistID == partslistID);
            if (prodOrderPartslistID != null)
            {
                ProdOrderPartslist = databaseApp.ProdOrderPartslist.FirstOrDefault(c => c.ProdOrderPartslistID == prodOrderPartslistID);
            }

            // Load main WF
            ConfigStores = GetConfigStores(iPlusMESConfigManager, new ACClassMethod[] { Method, InvokerPWNode.RefPAACClassMethod }, Partslist, ProdOrderPartslist);
        }
        #endregion

        #region Properties
        public ACClassWF InvokerPWNode {  get; private set; }   
        public ACClassMethod Method {  get; private set; }   
        public VD.Partslist Partslist {  get; private set; }   
        public VD.ProdOrderPartslist ProdOrderPartslist {  get; private set; }
        public List<IACConfigStore> ConfigStores {  get; private set; }   
        #endregion

        #region Methods
        private List<IACConfigStore> GetConfigStores(ConfigManagerIPlus iPlusMESConfigManager, ACClassMethod[] aCClassMethods, VD.Partslist partslist, VD.ProdOrderPartslist prodOrderPartslist)
        {
            List<IACConfigStore> configStores = new List<IACConfigStore>();

            if (prodOrderPartslist != null)
            {
                configStores.Add(prodOrderPartslist);
            }
            else
            {
                configStores.Add(partslist);
            }

            configStores.AddRange(aCClassMethods);

            configStores = iPlusMESConfigManager.GetACConfigStores(configStores);

            foreach (IACConfigStore configStore in configStores)
            {
                configStore.ClearCacheOfConfigurationEntries();
            }

            return configStores;
        }
        #endregion
    }
}
