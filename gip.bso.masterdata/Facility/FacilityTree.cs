using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.masterdata
{
    public static class FacilityTree
    {
        public static ACFSItem LoadFacilityTree(DatabaseApp databaseApp)
        {
            ACFSItem rootItem = GetRootFacilityACFSItem(databaseApp.ContextIPlus);
            FactoryFacilityACFSItem(databaseApp, rootItem);
            return rootItem;
        }

        public static string FacilityACCaption(Facility facility)
        {
            return string.Format(@"[{0}] {1}", facility.FacilityNo, facility.FacilityName);
        }

        public static ACFSItem GetNewRootFacilityACFSItem(Database database, IEnumerable<IACContainerWithItems> items)
        {
            ACFSItem rootFacilityACFSItem = GetRootFacilityACFSItem(database);
            foreach (var item in items)
                rootFacilityACFSItem.Add(item);
            return rootFacilityACFSItem;
        }

        public static ACFSItem GetRootFacilityACFSItem(Database database)
        {
            string rootACCaption = Translator.GetTranslation(ConstApp.Facility);
            ACFSItem rootItem = new ACFSItem(null, new ACFSItemContainer(database), null, rootACCaption, ResourceTypeEnum.IACObject);
            rootItem.IsVisible = true;
            return rootItem;
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
                FactoryFacilityACFSItem(databaseApp, cFSItem);
                parentACFSItem.Add(cFSItem);
            }
        }
    }
}
