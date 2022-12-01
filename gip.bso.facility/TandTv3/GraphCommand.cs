using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.facility.TandTv3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TandTv3 = gip.mes.facility.TandTv3;

namespace gip.bso.facility
{
    public class GraphCommand
    {
        public static string EmptyLotName = "#";

        #region ctor's

        public GraphCommand(DatabaseApp databaseApp, IFactoryTandTPointPresenterComponent componentFactory, ACComponent routingService)
        {
            DatabaseApp = databaseApp;
            ComponentFactory = componentFactory;
            RoutingService = routingService;
            FacilityIDAndACClassIDs = databaseApp.Facility.Where(c => c.VBiFacilityACClassID != null)
                .ToList()
                .GroupBy(c => c.VBiFacilityACClassID)
                .ToDictionary(key => key.Key ?? Guid.Empty, val => val.FirstOrDefault());
        }

        #endregion

        #region Properties

        public Dictionary<Guid, Facility> FacilityIDAndACClassIDs { get; set; }

        public DatabaseApp DatabaseApp { get; set; }
        public IFactoryTandTPointPresenterComponent ComponentFactory { get; private set; }
        public ACComponent RoutingService { get; private set; }
        #endregion

        #region methods

        public TandTGraphModel BuildGraphResult(TandTv3.TandTResult result, List<TandTv3.Model.DisplayGroupEnum> forShow, TandTGraphModel graphModel)
        {
            try
            {
                // #01 Build graph filter helper
                GraphFilterHelper graphFilterHelper = new GraphFilterHelper(forShow);
                bool getRoutingRelations = forShow.Contains(TandTv3.Model.DisplayGroupEnum.Storage) || forShow.Contains(TandTv3.Model.DisplayGroupEnum.Machines);
                GraphPointResult<IACObject> graphPointResult = BuildGrapPointResultFromTrackingResult(result);
                FactoryGrapMixPointRelations(result, graphPointResult);

                GraphPointResult<IACObject> filteredGraphPointResult = new GraphPointResult<IACObject>();
                FilterGraphPoints(graphPointResult, filteredGraphPointResult, graphFilterHelper);

                GraphRoutingRelations graphRoutingRelations = new GraphRoutingRelations(DatabaseApp, RoutingService, FacilityIDAndACClassIDs);
                List<GraphPointRelation<IACObject>> graphRoutingRelationList = graphRoutingRelations.BuildRoutingRelations(graphPointResult, result);
                foreach (var relation in graphRoutingRelationList)
                    if (!graphPointResult.Relations.Contains(relation))
                        graphPointResult.Relations.Add(relation);
                foreach (var msg in graphPointResult.Msgs)
                    result.ErrorMsg.AddDetailMessage(msg);

                GraphRelationCommand graphRelationCommand = new GraphRelationCommand();
                graphRelationCommand.Filter(filteredGraphPointResult, graphPointResult, graphFilterHelper);
                BSOTandTv3.ComponentNo = 0;

                foreach (var point in filteredGraphPointResult.Points)
                {
                    point.ProducedComponent = ComponentFactory.FactoryComponent(graphModel.JobID, point.ItemType, null, point.MixPointID, point.ItemID, point.Item);
                }
                foreach (var relation in filteredGraphPointResult.Relations)
                {
                    FactoryEdge(graphModel, relation.Source.ProducedComponent, relation.Target.ProducedComponent);
                }
                graphModel.Success = true;
            }
            catch (Exception ec)
            {
                graphModel.Error = ec;
            }
            return graphModel;
        }

        #endregion

        #region Private methods -> Graph

        private TandTEdge FactoryEdge(TandTGraphModel graphModel, TandTPointPresenter source, TandTPointPresenter target)
        {
            TandTEdge edge = graphModel.GraphEdges.FirstOrDefault(c => (c.SourceParent as TandTPointPresenter) == source && (c.TargetParent as TandTPointPresenter) == target);
            if (edge == null)
            {
                edge = new TandTEdge() { SourceParent = source, TargetParent = target };
                graphModel.GraphEdges.Add(edge);
            }
            return edge;
        }

        #endregion

        #region Graph points and relation build methods

        #region Build net
        public GraphPointResult<IACObject> BuildGrapPointResultFromTrackingResult(TandTv3.TandTResult result)
        {
            GraphPointResult<IACObject> graphResult = new GraphPointResult<IACObject>();
            foreach (var mixPoint in result.MixPoints)
            {
                FactoryMixPointResult(result, graphResult, mixPoint);
            }
            return graphResult;
        }

        private void FactoryGrapMixPointRelations(TandTv3.TandTResult result, GraphPointResult<IACObject> graphPointResult)
        {
            MDTrackingStartItemTypeEnum[] mixPointTypes = new MDTrackingStartItemTypeEnum[]
                            {
                    MDTrackingStartItemTypeEnum.TandTv3Point,
                    MDTrackingStartItemTypeEnum.TandTv3PointPosGrouped,
                    MDTrackingStartItemTypeEnum.TandTv3PointDN };

            foreach (var relation in result.MixPointRelations)
            {
                GraphPoint<IACObject> sourcePoint = graphPointResult.Points.Where(c => mixPointTypes.Contains(c.ItemType) && c.MixPointID == relation.SourceMixPoint.MixPointID).FirstOrDefault();
                GraphPoint<IACObject> targetPoint = graphPointResult.Points.Where(c => mixPointTypes.Contains(c.ItemType) && c.MixPointID == relation.TargetMixPoint.MixPointID).FirstOrDefault();
                if (sourcePoint != null && targetPoint != null)
                {
                    GraphPointRelation<IACObject> graphRelation = new GraphPointRelation<IACObject>(sourcePoint, targetPoint, GraphPointRelationTypeEnum.MixPoint);
                    if (!graphPointResult.Relations.Contains(graphRelation))
                        graphPointResult.Relations.Add(graphRelation);
                }
            }
        }

        #endregion

        #region Filter net

        public GraphPointResult<IACObject> FilterGraphPoints(GraphPointResult<IACObject> pointResult, GraphPointResult<IACObject> filteredPointResult, GraphFilterHelper filter)
        {
            foreach (var point in pointResult.Points)
            {
                if (filter.ItemsForShow.Contains(point.ItemType))
                {
                    if (!filteredPointResult.Points.Contains(point))
                        filteredPointResult.Points.Add(point);
                }
            }
            return filteredPointResult;
        }

        #endregion

        #endregion

        #region Private methods

        #region Private methods -> factory mix point 
        public GraphPointResult<IACObject> FactoryMixPointResult(TandTv3.TandTResult result, GraphPointResult<IACObject> graphPointResult, TandTv3Point mixPoint)
        {
            GraphPoint<IACObject> graphMixPoint = FactoryPointFromMixPoint(mixPoint);
            if (mixPoint is TandTv3PointDN)
                graphMixPoint.ItemType = MDTrackingStartItemTypeEnum.TandTv3PointDN;
            graphPointResult.Points.Add(graphMixPoint);

            FactoryMixPointMachines(graphPointResult, mixPoint, graphMixPoint);
            FactoryMixPointLots(graphPointResult, mixPoint, graphMixPoint);
            if (mixPoint.ProdOrder != null)
                FactoryProdOrder(graphPointResult, mixPoint, graphMixPoint);

            List<GraphPoint<IACObject>> graphMaterialPoints = FactoryMixPointMaterials(result, graphPointResult, mixPoint, graphMixPoint);
            FactoryMixPointFacilities(graphPointResult, mixPoint, graphMixPoint, graphMaterialPoints);

            if (mixPoint is TandTv3PointDN)
            {
                TandTv3PointDN tandTv3PointDN = mixPoint as TandTv3PointDN;
                if (tandTv3PointDN.InOrderPosPreviews != null && tandTv3PointDN.InOrderPosPreviews.Any())
                    FactoryInOrderPosPreviews(graphPointResult, graphMixPoint, tandTv3PointDN.InOrderPosPreviews);
                if (tandTv3PointDN.OutOrderPosPreviews != null && tandTv3PointDN.OutOrderPosPreviews.Any())
                    FactoryOutOrderPosPreviews(graphPointResult, graphMixPoint, tandTv3PointDN.OutOrderPosPreviews);
                if (tandTv3PointDN.PickingPosPreviews != null && tandTv3PointDN.PickingPosPreviews.Any())
                    FactoryPickingPosPreviews(graphPointResult, graphMixPoint, tandTv3PointDN.PickingPosPreviews);
            }

            return graphPointResult;
        }

        private void FactoryInOrderPosPreviews(GraphPointResult<IACObject> result, GraphPoint<IACObject> graphMixPoint, List<InOrderPosPreview> inOrderPosPreviews)
        {
            foreach (var inOrderPos in inOrderPosPreviews)
            {
                GraphPoint<IACObject> inOrderPosPoint = result.FactoryPoint(inOrderPos.ID, inOrderPos.InOrderNo, MDTrackingStartItemTypeEnum.InOrderPosPreview,
                    MDBookingDirectionEnum.None, null, inOrderPos);
                result.AddRelation(inOrderPosPoint, graphMixPoint, GraphPointRelationTypeEnum.MixPoint);
            }
        }

        private void FactoryOutOrderPosPreviews(GraphPointResult<IACObject> result, GraphPoint<IACObject> graphMixPoint, List<OutOrderPosPreview> outOrderPosPreviews)
        {
            foreach (var outOrderPos in outOrderPosPreviews)
            {
                GraphPoint<IACObject> outOrderPosPoint = result.FactoryPoint(outOrderPos.ID, outOrderPos.OutOrderNo, MDTrackingStartItemTypeEnum.OutOrderPosPreview,
                    MDBookingDirectionEnum.None, null, outOrderPos);
                result.AddRelation(graphMixPoint, outOrderPosPoint, GraphPointRelationTypeEnum.MixPoint);
            }
        }

        private void FactoryPickingPosPreviews(GraphPointResult<IACObject> result, GraphPoint<IACObject> graphMixPoint, List<PickingPosPreview> pickingPosPreviews)
        {
            foreach (var pickingPos in pickingPosPreviews)
            {
                GraphPoint<IACObject> pickingPosPoint = result.FactoryPoint(pickingPos.ID, pickingPos.PickingNo, MDTrackingStartItemTypeEnum.PickingPosPreview,
                    MDBookingDirectionEnum.None, null, pickingPos);
                result.AddRelation(pickingPosPoint, graphMixPoint, GraphPointRelationTypeEnum.MixPoint);
            }
        }

        private List<GraphPoint<IACObject>> FactoryMixPointMaterials(TandTv3.TandTResult result, GraphPointResult<IACObject> graphPointResult, TandTv3Point mixPoint, GraphPoint<IACObject> graphMixPoint)
        {
            List<GraphPoint<IACObject>> graphPoints = new List<GraphPoint<IACObject>>();

            #region Display outward materials they are not part of any other elements
            List<string> inwardMaterialNos = new List<string>();
            var prevousMixPoints = result.MixPointRelations.Where(c => c.TargetMixPoint.MixPointID == mixPoint.MixPointID).Select(c=>c.SourceMixPoint);
            foreach (var tmpMixPoint in prevousMixPoints)
            {
                if (tmpMixPoint is TandTv3PointPosGrouped)
                {
                    TandTv3PointPosGrouped tmpMixPointGroupped = tmpMixPoint as TandTv3PointPosGrouped;
                    inwardMaterialNos.AddRange(tmpMixPointGroupped.InwardMaterials.Select(c => c.MaterialNo));
                }
                else
                {
                    inwardMaterialNos.Add(tmpMixPoint.InwardMaterialNo);
                }
            }

            foreach (var outwardMaterial in mixPoint.OutwardMaterials)
            {
                if (!inwardMaterialNos.Contains(outwardMaterial.MaterialNo))
                {
                    GraphPoint<IACObject> outwardMaterialPoint = graphPointResult.FactoryPoint(outwardMaterial.MaterialID, outwardMaterial.MaterialNo, MDTrackingStartItemTypeEnum.Material,
                    MDBookingDirectionEnum.None, null, outwardMaterial);
                    graphPoints.Add(outwardMaterialPoint);
                    graphPointResult.AddRelation(outwardMaterialPoint, graphMixPoint, GraphPointRelationTypeEnum.MixPoint);
                }
            }

            #endregion

            if (mixPoint is TandTv3PointPosGrouped)
            {
                TandTv3PointPosGrouped groupPoint = mixPoint as TandTv3PointPosGrouped;
                foreach (var inwardMaterial in groupPoint.InwardMaterials)
                {
                    GraphPoint<IACObject> inwardMaterialPoint =
                    graphPointResult.FactoryPoint(inwardMaterial.MaterialID, inwardMaterial.MaterialNo,
                    MDTrackingStartItemTypeEnum.Material, MDBookingDirectionEnum.None, null, mixPoint.InwardMaterial);
                    graphPoints.Add(inwardMaterialPoint);
                    graphPointResult.AddRelation(graphMixPoint, inwardMaterialPoint, GraphPointRelationTypeEnum.MixPoint);
                }
            }
            else
            {
                GraphPoint<IACObject> inwardMaterialPoint =
                    graphPointResult.FactoryPoint(mixPoint.InwardMaterial.MaterialID, mixPoint.InwardMaterial.MaterialNo,
                    MDTrackingStartItemTypeEnum.Material, MDBookingDirectionEnum.None, null, mixPoint.InwardMaterial);
                graphPoints.Add(inwardMaterialPoint);
                graphPointResult.AddRelation(graphMixPoint, inwardMaterialPoint, GraphPointRelationTypeEnum.MixPoint);
            }
            return graphPoints;
        }

        private GraphPoint<IACObject> FactoryPointFromMixPoint(TandTv3Point mixPoint)
        {
            var graphPoint = new GraphPoint<IACObject>()
            {
                Item = mixPoint,
                ItemID = mixPoint.MixPointID,
                ItemNo = mixPoint.MixPointID.ToString(),
                ItemType = MDTrackingStartItemTypeEnum.TandTv3Point,
                MixPointID = mixPoint.MixPointID
            };
            if (mixPoint is TandTv3PointDN)
                graphPoint.ItemType = MDTrackingStartItemTypeEnum.TandTv3PointDN;
            if (mixPoint is TandTv3PointPosGrouped)
                graphPoint.ItemType = MDTrackingStartItemTypeEnum.TandTv3PointPosGrouped;
            return graphPoint;
        }

        private void FactoryMixPointFacilities(GraphPointResult<IACObject> result, TandTv3Point mixPoint, GraphPoint<IACObject> graphMixPoint, List<GraphPoint<IACObject>> graphMaterialPoints)
        {
            foreach (var facility in mixPoint.OutwardFacilities)
            {
                if (!mixPoint.InwardFacilities.Keys.Contains(facility.Key))
                {
                    GraphPoint<IACObject> targetGraphPoint = graphMaterialPoints.FirstOrDefault(c => c.ItemType == MDTrackingStartItemTypeEnum.Material && c.ItemNo == facility.Value.MaterialNo);
                    if (targetGraphPoint == null)
                        targetGraphPoint = graphMixPoint;
                    GraphPoint<IACObject> outwardFacilityPoint = result.FactoryPoint(facility.Value.FacilityID, facility.Key, MDTrackingStartItemTypeEnum.FacilityPreview, MDBookingDirectionEnum.None, null, facility.Value);
                    result.AddRelation(outwardFacilityPoint, targetGraphPoint, GraphPointRelationTypeEnum.MixPoint);
                }
            }

            foreach (var facility in mixPoint.InwardFacilities)
            {
                GraphPoint<IACObject> sourceGraphPoint = graphMaterialPoints.FirstOrDefault(c => c.ItemType == MDTrackingStartItemTypeEnum.Material && c.ItemNo == facility.Value.MaterialNo);
                if (sourceGraphPoint == null)
                    sourceGraphPoint = graphMixPoint;
                GraphPoint<IACObject> inwardFacilityPoint = result.FactoryPoint(facility.Value.FacilityID, facility.Key, MDTrackingStartItemTypeEnum.FacilityPreview, MDBookingDirectionEnum.None, null, facility.Value);
                if (!result.ContainsRelation(inwardFacilityPoint, sourceGraphPoint, GraphPointRelationTypeEnum.MixPoint))
                    result.AddRelation(sourceGraphPoint, inwardFacilityPoint, GraphPointRelationTypeEnum.MixPoint);
                else
                    result.AddRelation(graphMixPoint, inwardFacilityPoint, GraphPointRelationTypeEnum.MixPoint);
            }
        }

        private void FactoryMixPointMachines(GraphPointResult<IACObject> result, TandTv3Point mixPoint, GraphPoint<IACObject> graphMixPoint)
        {
            foreach (var machine in mixPoint.InwardMachines)
            {
                GraphPoint<IACObject> inwardMachinePoint = result.FactoryPoint(machine.ACClassID, machine.ACIdentifier, MDTrackingStartItemTypeEnum.ACClass, MDBookingDirectionEnum.None, null, machine);
                result.AddRelation(graphMixPoint, inwardMachinePoint, GraphPointRelationTypeEnum.MixPoint);
            }

            foreach (var machine in mixPoint.OutwardMachines)
            {
                GraphPoint<IACObject> outwardMachinePoint = result.FactoryPoint(machine.ACClassID, machine.ACIdentifier, MDTrackingStartItemTypeEnum.ACClass, MDBookingDirectionEnum.None, null, machine);
                result.AddRelation(outwardMachinePoint, graphMixPoint, GraphPointRelationTypeEnum.MixPoint);
            }

        }

        private void FactoryMixPointLots(GraphPointResult<IACObject> result, TandTv3Point mixPoint, GraphPoint<IACObject> graphMixPoint)
        {
            foreach (var lot in mixPoint.OutwardLotsList)
            {
                GraphPoint<IACObject> outwardLotPoint = result.FactoryPoint(lot.FacilityLotID, lot.LotNo,
                    MDTrackingStartItemTypeEnum.FacilityLot, MDBookingDirectionEnum.None, null, new FacilityLot() { FacilityLotID = lot.FacilityLotID, LotNo = lot.LotNo });
                result.AddRelation(outwardLotPoint, graphMixPoint, GraphPointRelationTypeEnum.MixPoint);
            }
            if (mixPoint is TandTv3PointPosGrouped)
            {
                TandTv3PointPosGrouped groupMixPoint = mixPoint as TandTv3PointPosGrouped;
                foreach (var lot in groupMixPoint.InwardLotsList)
                {
                    GraphPoint<IACObject> inwardLotPoint = result.FactoryPoint(lot.FacilityLotID, lot.LotNo, MDTrackingStartItemTypeEnum.FacilityLot,
                        MDBookingDirectionEnum.None, null, new FacilityLot() { FacilityLotID = lot.FacilityLotID, LotNo = lot.LotNo });
                    result.AddRelation(graphMixPoint, inwardLotPoint, GraphPointRelationTypeEnum.MixPoint);
                }
            }
            else
            {
                GraphPoint<IACObject> inwardLotPoint = result.FactoryPoint(mixPoint.InwardLot.FacilityLotID, mixPoint.InwardLot.LotNo,
                    MDTrackingStartItemTypeEnum.FacilityLot, MDBookingDirectionEnum.None, null, new FacilityLot() { LotNo = mixPoint.InwardLot.LotNo, FacilityLotID = mixPoint.InwardLot.FacilityLotID });
                result.AddRelation(graphMixPoint, inwardLotPoint, GraphPointRelationTypeEnum.MixPoint);
            }
        }

        private void FactoryProdOrder(GraphPointResult<IACObject> result, TandTv3Point mixPoint, GraphPoint<IACObject> graphMixPoint)
        {
            GraphPoint<IACObject> inwardOrderPoint = result.FactoryPoint(mixPoint.ProdOrder.ProdOrderID, mixPoint.ProdOrder.ProgramNo, MDTrackingStartItemTypeEnum.ProdOrder, MDBookingDirectionEnum.None, null, mixPoint.ProdOrder);
            result.AddRelation(graphMixPoint, inwardOrderPoint, GraphPointRelationTypeEnum.MixPoint);
        }

        #endregion

        #endregion
    }
}
