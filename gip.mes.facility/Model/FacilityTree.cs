using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public static class FacilityTree
    {
        public static ACFSItem LoadFacilityTree(DatabaseApp databaseApp)
        {
            ACFSItem rootItem = GetRootFacilityACFSItem(databaseApp.ContextIPlus);
            FactoryFacilityACFSItem(databaseApp, rootItem);
            return rootItem;
        }



        public static ACFSItem GetNewRootFacilityACFSItem(Database database, IEnumerable<IACContainerWithItems> items)
        {
            ACFSItem rootFacilityACFSItem = GetRootFacilityACFSItem(database);
            foreach (var item in items)
                rootFacilityACFSItem.Add(item);
            return rootFacilityACFSItem;
        }



        public static void FactoryFacilityACFSItem(DatabaseApp databaseApp, ACFSItem parentACFSItem)
        {
            Guid? parentFaciltiyID = null;
            if (parentACFSItem.ACObject != null)
            {
                Facility parentFacility = parentACFSItem.ACObject as Facility;
                parentFaciltiyID = parentFacility.FacilityID;
            }

            List<Facility> levelFacilities = databaseApp.Facility.Where(c => (c.ParentFacilityID ?? Guid.Empty) == (parentFaciltiyID ?? Guid.Empty)).OrderBy(c => c.FacilityNo).ToList();
            foreach (Facility facility in levelFacilities)
            {
                ACFSItem cFSItem = new ACFSItem(null, parentACFSItem.Container, facility, FacilityACCaption(facility), ResourceTypeEnum.IACObject);
                cFSItem.OnACFSItemChange += CFSItem_OnACFSItemChange;
                FactoryFacilityACFSItem(databaseApp, cFSItem);
                parentACFSItem.Add(cFSItem);
            }
        }


        #region Helper methods

        public static ACFSItem GetRootFacilityACFSItem(Database database)
        {
            string rootACCaption = Translator.GetTranslation(ConstApp.Facility);
            ACFSItem rootItem = new ACFSItem(null, new ACFSItemContainer(database), null, rootACCaption, ResourceTypeEnum.IACObject);
            rootItem.IsVisible = true;
            return rootItem;
        }

        public static string FacilityACCaption(Facility facility)
        {
            return string.Format(@"[{0}] {1}", facility.FacilityNo, facility.FacilityName);
        }

        public static void SetupCurrentVisible(ACFSItem aCFSItem)
        {
            aCFSItem.IsVisible = true;
            if (aCFSItem.ParentACObject != null)
                SetupCurrentVisible(aCFSItem.ParentACObject as ACFSItem);
        }

        public static void CFSItem_OnACFSItemChange(ACFSItem aCFSItem, string propertyName, IACObject aCObject, string acObjectPropertyName)
        {
            if (!string.IsNullOrWhiteSpace(acObjectPropertyName) && (acObjectPropertyName == "FacilityNo" || acObjectPropertyName == "FacilityName"))
            {
                aCFSItem.ACCaption = FacilityACCaption(aCObject as Facility);
            }
        }

        #endregion
    }
}
