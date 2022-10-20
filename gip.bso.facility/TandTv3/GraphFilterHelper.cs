using gip.mes.datamodel;
using gip.mes.facility.TandTv3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TandTv3 = gip.mes.facility.TandTv3;

namespace gip.bso.facility
{
    public class GraphFilterHelper
    {
        public GraphFilterHelper(List<TandTv3.Model.DisplayGroupEnum> forShow)
        {
            ForShow = forShow;
            PopulateItemsForShow(forShow);

            PopulateIsOnlyProperties(forShow);
            PopulateIsOnlyMixOrRoute(forShow);

            IsMixPointStart = forShow.Contains(TandTv3.Model.DisplayGroupEnum.MixPoint) || forShow.Contains(TandTv3.Model.DisplayGroupEnum.MixPointGroup);
            IsMixPointFacilityWithoutMaterial =
                IsMixPointStart
                && forShow.Contains(TandTv3.Model.DisplayGroupEnum.Storage)
                && !forShow.Contains(TandTv3.Model.DisplayGroupEnum.Material);
            IsLotStart = forShow.Contains(TandTv3.Model.DisplayGroupEnum.Lots);
        }

      

        #region Properties
        public List<TandTv3.Model.DisplayGroupEnum> ForShow { get; set; }

        public List<MDTrackingStartItemTypeEnum> ItemsForShow { get; set; }

        public bool IsOnlyFacility { get; set; }
        public bool IsOnlyProdOrder { get; set; }
        public bool IsOnlyMachines { get; set; }
        public bool IsMixPointStart { get; set; }
        public bool IsMixPointFacilityWithoutMaterial { get; set; }
        public bool IsLotStart { get; set; }

        public bool IsOnlyMixItems { get; set; }

        public bool IsOnlyRoutingItems { get; set; }

        #endregion

        #region Helper methods
        private void PopulateIsOnlyProperties(List<DisplayGroupEnum> forShow)
        {
            List<TandTv3.Model.DisplayGroupEnum> onlyFacility = new List<TandTv3.Model.DisplayGroupEnum>() { TandTv3.Model.DisplayGroupEnum.Storage };
            List<TandTv3.Model.DisplayGroupEnum> onlyProdOrder = new List<TandTv3.Model.DisplayGroupEnum>() { TandTv3.Model.DisplayGroupEnum.Orders };
            List<TandTv3.Model.DisplayGroupEnum> onlyMachines = new List<TandTv3.Model.DisplayGroupEnum>() { TandTv3.Model.DisplayGroupEnum.Machines };

            IsOnlyFacility = forShow.SequenceEqual(onlyFacility);
            IsOnlyProdOrder = forShow.SequenceEqual(onlyProdOrder);
            IsOnlyMachines = forShow.SequenceEqual(onlyMachines);
        }

        private void PopulateItemsForShow(List<DisplayGroupEnum> forShow)
        {
            ItemsForShow = new List<MDTrackingStartItemTypeEnum>();
            foreach (var filterItemForShow in forShow)
            {
                List<MDTrackingStartItemTypeEnum> itemForShow = TandTv3.CastDisplayGroupToItemTypeEnum.Cast(filterItemForShow);
                if (itemForShow.Any())
                    ItemsForShow.AddRange(itemForShow);
            }
        }

        private void PopulateIsOnlyMixOrRoute(List<DisplayGroupEnum> forShow)
        {
            TandTv3.Model.DisplayGroupEnum[] mixGroupTypes = new TandTv3.Model.DisplayGroupEnum[] {
               TandTv3.Model.DisplayGroupEnum.Orders,
               TandTv3.Model. DisplayGroupEnum.MixPointGroup,
               TandTv3.Model. DisplayGroupEnum.Material,
               TandTv3.Model. DisplayGroupEnum.MixPoint
            };
            //MDTrackingStartItemTypeEnum[] mixStartItemTypes = mixGroupTypes.Select(c => TandTv3.CastDisplayGroupToItemTypeEnum.Cast(c)).SelectMany(c => c).ToArray();

            TandTv3.Model.DisplayGroupEnum[] routingGroupTypes = new TandTv3.Model.DisplayGroupEnum[] {
               TandTv3.Model.DisplayGroupEnum.Storage,
               TandTv3.Model. DisplayGroupEnum.Machines
            };
            //MDTrackingStartItemTypeEnum[] routingStartItemTypes = routingGroupTypes.Select(c => TandTv3.CastDisplayGroupToItemTypeEnum.Cast(c)).SelectMany(c => c).ToArray();

            IsOnlyMixItems = !forShow.Intersect(routingGroupTypes).Any();
            IsOnlyRoutingItems = !forShow.Intersect(mixGroupTypes).Any();
        }

        #endregion
    }
}
