using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TandTv3 = gip.mes.facility.TandTv3;

namespace gip.bso.facility
{
    public class GraphRelationCommand
    {

        #region ctor's
        public GraphRelationCommand()
        {

        }
        #endregion

        #region Public methods
        public virtual void Filter(GraphPointResult<IACObject> filteredPointResult, GraphPointResult<IACObject> graphPointResult, GraphFilterHelper filter)
        {
            List<GraphPointRelation<IACObject>> filteredRelations = BuildSplitedMixPointRouteRelationList(filteredPointResult, graphPointResult, filter);
            List<GraphPointRelation<IACObject>> calculatedRelations = BuildNewRelations(filter, filteredPointResult, filteredRelations);
            filteredPointResult.Relations = calculatedRelations;
        }

        #region BuildNewRelationsv2
        private List<GraphPointRelation<IACObject>> BuildNewRelations(GraphFilterHelper filter, GraphPointResult<IACObject> filteredPointResult,
            List<GraphPointRelation<IACObject>> filteredRelations)
        {

            List<GraphPointRelation<IACObject>> calculatedRelations = new List<GraphPointRelation<IACObject>>();

            calculatedRelations.AddRange(
                    filteredRelations.Where(c =>
                        filter.ItemsForShow.Contains(c.Source.ItemType) &&
                        filter.ItemsForShow.Contains(c.Target.ItemType)
                    )

            );

            if (!filter.IsMixPointStart || filter.IsMixPointFacilityWithoutMaterial)
            {
                filteredRelations.RemoveAll(c => calculatedRelations.Contains(c));
                IEnumerable<GraphPoint<IACObject>> forProcess =
                    filteredPointResult
                    .Points
                    .Where(c =>
                        filteredRelations.Select(x => x.Source).Contains(c)
                        );

                foreach (var item in forProcess)
                {
                    WhileBuildNewRelationsv2(filter, calculatedRelations, item, item, filteredRelations);
                }

            }

            return calculatedRelations;
        }

        private void WhileBuildNewRelationsv2(GraphFilterHelper filter, List<GraphPointRelation<IACObject>> calculatedRelations,
            GraphPoint<IACObject> startPoint, GraphPoint<IACObject> searchPoint, List<GraphPointRelation<IACObject>> filteredRelations)
        {
            IEnumerable<GraphPointRelation<IACObject>> tmpRelations = filteredRelations.Where(c => c.Source == searchPoint && !c.IsPassed);
            foreach (var tmpRelation in tmpRelations)
            {
                tmpRelation.IsUsedNrTime++;
                if(tmpRelation.IsUsedNrTime > 9)
                {
                    tmpRelation.IsPassed = true;
                }
                if (
                        !startPoint.IsVirtual
                        && !tmpRelation.Target.IsVirtual
                        && filter.ItemsForShow.Contains(startPoint.ItemType)
                        && filter.ItemsForShow.Contains(tmpRelation.Target.ItemType)
                        && !(
                                (startPoint.ItemType == MDTrackingStartItemTypeEnum.TandTv3PointPosGrouped || startPoint.ItemType == MDTrackingStartItemTypeEnum.TandTv3Point)
                                    && (tmpRelation.Target.ItemType == MDTrackingStartItemTypeEnum.TandTv3PointPosGrouped || tmpRelation.Target.ItemType == MDTrackingStartItemTypeEnum.TandTv3Point))
                    )
                {
                    GraphPointRelation<IACObject> relation = new GraphPointRelation<IACObject>(startPoint, tmpRelation.Target, GraphPointRelationTypeEnum.MixPoint);
                    if (!calculatedRelations.Contains(relation))
                        calculatedRelations.Add(relation);
                    //WhileBuildNewRelationsv2(filter, calculatedRelations, tmpRelation.Target, tmpRelation.Target, filteredRelations);
                }
                else
                    WhileBuildNewRelationsv2(filter, calculatedRelations, startPoint, tmpRelation.Target, filteredRelations);
            }
        }
        #endregion

        #endregion

        #region private methods

        private List<GraphPointRelation<IACObject>> BuildSplitedMixPointRouteRelationList(GraphPointResult<IACObject> filteredPointResult, GraphPointResult<IACObject> graphPointResult, GraphFilterHelper filter)
        {
            List<GraphPointRelation<IACObject>> filteredRelations = new List<GraphPointRelation<IACObject>>();
            var mixPointRelations = graphPointResult.Relations.Where(c => c.RelationType != GraphPointRelationTypeEnum.Routing);
            var routingRelations = graphPointResult.Relations.Where(c => c.RelationType == GraphPointRelationTypeEnum.Routing);

            if (filter.IsOnlyMixItems)
                filteredRelations.AddRange(mixPointRelations);
            else if (filter.IsOnlyRoutingItems && routingRelations != null && routingRelations.Any())
                filteredRelations.AddRange(routingRelations);
            else
            {
                filteredRelations.AddRange(routingRelations);
                foreach (var relation in mixPointRelations)
                    if (!filteredRelations.Any(c => c.Source.ItemID == relation.Source.ItemID && c.Target.ItemID == relation.Target.ItemID))
                        filteredRelations.Add(relation);
            }

            filteredRelations = filteredRelations.Distinct().ToList();
            return filteredRelations;
        }

        private List<GraphPointRelation<IACObject>> FilterRoutingRelations(GraphPointResult<IACObject> filteredPointResult, GraphPointResult<IACObject> pointResult)
        {
            var routingRelations = pointResult.Relations.Where(c => c.RelationType == GraphPointRelationTypeEnum.Routing);
            Guid[] routingRelationItemIDs =
                filteredPointResult
                .Points
                .Where(c =>
                    c.Item is gip.core.datamodel.ACClass
                    || c.Item is mes.facility.FacilityPreview
                )
                .Select(c =>
                   c.ItemID
                 )
                .ToArray();
            return
                routingRelations
                .Where(c =>
                    routingRelationItemIDs.Contains(c.Source.ItemID)
                    || routingRelationItemIDs.Contains(c.Target.ItemID)
                )
                .Distinct()
                .ToList();
        }



        #endregion
    }
}
