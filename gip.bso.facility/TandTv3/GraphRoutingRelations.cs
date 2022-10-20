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
    public class GraphRoutingRelations
    {
        #region properties

        public DatabaseApp DatabaseApp { get; set; }

        public ACComponent RoutingService { get; private set; }

        public Dictionary<Guid, Facility> FacilityIDAndACClassIDs { get; set; }

        #endregion

        #region ctor's
        public GraphRoutingRelations(DatabaseApp databaseApp, ACComponent routingService, Dictionary<Guid, Facility> facilityIDAndACClassIDs)
        {
            DatabaseApp = databaseApp;
            RoutingService = routingService;
            FacilityIDAndACClassIDs = facilityIDAndACClassIDs;
        }
        #endregion

        #region Routing

        public List<GraphPointRelation<IACObject>> BuildRoutingRelations(GraphPointResult<IACObject> graphResult, TandTv3.TandTResult result)
        {
            List<GraphPointRelation<IACObject>> routingRelations = new List<GraphPointRelation<IACObject>>();
            try
            {
                if(RoutingService != null)
                {
                    using (ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
                    {
                        routingRelations = FindRoutingRelations(DatabaseApp.ContextIPlus, graphResult, result);
                        routingRelations = routingRelations.Distinct().ToList();
                    }
                }
            }
            catch (Exception ec)
            {
                graphResult.Msgs.Add(new Msg() { Message = "Error get routing relatios!", MessageLevel = eMsgLevel.Error });
                Exception tmpEc = ec;
                while (tmpEc != null)
                {
                    graphResult.Msgs.Add(new Msg() { MessageLevel = eMsgLevel.Error, Message = ec.Message });
                    tmpEc = tmpEc.InnerException;
                }
            }
            return routingRelations;
        }
        #endregion
        #region  Private methods -> Build relations from routing

        private List<GraphPointRelation<IACObject>> FindRoutingRelations(Database database, GraphPointResult<IACObject> graphResult, TandTv3.TandTResult result)
        {
            List<GraphPointRelation<IACObject>> machineRoutingRelations = new List<GraphPointRelation<IACObject>>();

            List<SearchRouteCombination> allUrlCombinations = RoutingSearchPrepareUrls(result);
            List<Route> routes = null;
            bool oneByOneRoute = true;

            if (oneByOneRoute)
            {

                // @aagincic: this work for me - search sourceUrl -> targetUrl one by one
                routes = new List<Route>();
                foreach (var tmpTestCombination in allUrlCombinations)
                {
                    RoutingResult test = ACRoutingService.MemSelectRoutes(database, tmpTestCombination.FromACUrl, tmpTestCombination.ToACUrl,
                        RouteDirections.Forwards, "", 1, true, true, null, RoutingService);
                    if (test.Routes != null)
                    {
                        routes.AddRange(test.Routes);
                    }
                }
                var test2 = routes.Count;
            }
            else
            {
                // @aagincic: this not working List<string> sourceUrls -> List<string> targetUrls
                RoutingResult memSelectRoutes =
                    ACRoutingService
                    .MemSelectRoutes(
                        database,
                        allUrlCombinations.Select(c => c.FromACUrl).ToList(),
                        allUrlCombinations.Select(c => c.ToACUrl).ToList(),
                        RouteDirections.Forwards, "", allUrlCombinations.Count(), true, true, null, RoutingService);
                var test1 = memSelectRoutes.Routes != null ? memSelectRoutes.Routes.Count() : 0;
                routes = memSelectRoutes.Routes.ToList();
            }

            if (routes != null)
            {
                machineRoutingRelations = ConvertRoutesToGrahPointRelations(graphResult, routes, allUrlCombinations);
                //// Filter machineRoutingRelations for only relations they are in current fetched model
                //machineRoutingRelations =
                //    machineRoutingRelations
                //    .Where(c =>
                //        result.Ids.Keys.Contains(c.Source.ItemID)
                //        || result.Ids.Keys.Contains(c.Target.ItemID)
                //    ).ToList();
            }

            return machineRoutingRelations;
        }

        public List<SearchRouteCombination> RoutingSearchPrepareUrls(TandTv3.TandTResult result)
        {
            List<SearchRouteCombination> routesForSearch = new List<SearchRouteCombination>();
            foreach (var mixPoint in result.MixPoints)
            {
                // Intern MixPoint Machine Relations mixPoint.OutwardMachines => mixPoint.InwardMachines
                foreach (var outwardMachine in mixPoint.OutwardMachines)
                {
                    foreach (var inwardMachine in mixPoint.InwardMachines)
                    {
                        SearchRouteCombination routeCombination = new SearchRouteCombination()
                        {
                            FromACUrl = outwardMachine.ACUrlComponent,
                            FromType = MDTrackingStartItemTypeEnum.ACClass,

                            ToACUrl = inwardMachine.ACUrlComponent,
                            ToType = MDTrackingStartItemTypeEnum.ACClass
                        };
                        routesForSearch.Add(routeCombination);
                    }
                }

                // Connection Machines <=> Facilities: (1) mixPoint.OutwardFacility => mixPoint.OutwardMachines
                foreach (var outwardFacility in mixPoint.OutwardFacilities)
                {
                    foreach (var outwardMachine in mixPoint.OutwardMachines)
                    {
                        SearchRouteCombination routeCombination = new SearchRouteCombination()
                        {
                            FromACUrl = outwardFacility.Value.FacilityACClass.ACUrlComponent,
                            FromType = MDTrackingStartItemTypeEnum.FacilityPreview,

                            ToACUrl = outwardMachine.ACUrlComponent,
                            ToType = MDTrackingStartItemTypeEnum.ACClass
                        };
                        routesForSearch.Add(routeCombination);
                    }
                }

                // Connection Machines <=> Facilities: (2) mixPoint.InwardMachine => mixPoint.InwardFacility
                foreach (var inwardMachine in mixPoint.InwardMachines)
                {
                    foreach (var inwardFacility in mixPoint.InwardFacilities)
                    {
                        SearchRouteCombination routeCombination = new SearchRouteCombination()
                        {
                            FromACUrl = inwardMachine.ACUrlComponent,
                            FromType = MDTrackingStartItemTypeEnum.ACClass,

                            ToACUrl = inwardFacility.Value.FacilityACClass.ACUrlComponent,
                            ToType = MDTrackingStartItemTypeEnum.FacilityPreview
                        };
                        routesForSearch.Add(routeCombination);
                    }
                }

                // Relation between mix points source.InwardMachines => target.OutwardMachines
                List<TandTv3Point> sourceMixPoints = result.MixPointRelations.Where(c => c.TargetMixPoint.MixPointID == mixPoint.MixPointID).Select(c => c.SourceMixPoint).ToList();
                foreach (var sourceMixPoint in sourceMixPoints)
                {
                    foreach (var inwardMachine in sourceMixPoint.InwardMachines)
                    {
                        foreach (var outwardMachine in mixPoint.OutwardMachines)
                        {
                            SearchRouteCombination routeCombination = new SearchRouteCombination()
                            {
                                FromACUrl = inwardMachine.ACUrlComponent,
                                FromType = MDTrackingStartItemTypeEnum.ACClass,

                                ToACUrl = outwardMachine.ACUrlComponent,
                                ToType = MDTrackingStartItemTypeEnum.ACClass
                            };
                            routesForSearch.Add(routeCombination);
                        }
                    }

                    // Connection Machines <=> Facilities: (3) sourceMixPoint.InwardFacility => mixPoint.OutwardMachine
                    foreach (var inwardFacility in sourceMixPoint.InwardFacilities)
                    {
                        foreach (var outwardMachine in mixPoint.OutwardMachines)
                        {
                            SearchRouteCombination routeCombination = new SearchRouteCombination()
                            {
                                FromACUrl = inwardFacility.Value.FacilityACClass.ACUrlComponent,
                                FromType = MDTrackingStartItemTypeEnum.FacilityPreview,

                                ToACUrl = outwardMachine.ACUrlComponent,
                                ToType = MDTrackingStartItemTypeEnum.ACClass
                            };
                            routesForSearch.Add(routeCombination);
                        }
                    }

                    // Connection Machines <=> Facilities: (4) sourceMixPoint.InwardFacility => mixPoint.OutwardFacility
                    // @aagincic: direct link facility -> facility?
                    foreach (var inwardFacility in sourceMixPoint.InwardFacilities)
                    {
                        foreach (var outwardFacility in mixPoint.OutwardFacilities)
                        {
                            if(inwardFacility.Value.FacilityACClass != null && outwardFacility.Value.FacilityACClass != null)
                            {
                                SearchRouteCombination routeCombination = new SearchRouteCombination()
                                {
                                    FromACUrl = inwardFacility.Value.FacilityACClass.ACUrlComponent,
                                    FromType = MDTrackingStartItemTypeEnum.FacilityPreview,

                                    ToACUrl = outwardFacility.Value.FacilityACClass.ACUrlComponent,
                                    ToType = MDTrackingStartItemTypeEnum.FacilityPreview
                                };
                                routesForSearch.Add(routeCombination);
                            }
                        }
                    }
                }
            }
            routesForSearch = routesForSearch.Distinct().ToList();
            return routesForSearch;
        }

        private List<GraphPointRelation<IACObject>> ConvertRoutesToGrahPointRelations(GraphPointResult<IACObject> graphResult, List<Route> routes,
            List<SearchRouteCombination> allUrlCombinations)
        {
            List<GraphPointRelation<IACObject>> routingRelations = new List<GraphPointRelation<IACObject>>();
            foreach (var route in routes)
            {
                foreach (var routeItem in route.Items)
                {
                    SearchRouteCombination sourceRouteCombination = allUrlCombinations.FirstOrDefault(c => c.FromACUrl == routeItem.Source.ACUrlComponent);
                    SearchRouteCombination targetRouteCombination = allUrlCombinations.FirstOrDefault(c => c.ToACUrl == routeItem.Target.ACUrlComponent);
                    if (sourceRouteCombination != null && targetRouteCombination != null)
                    {
                        GraphPoint<IACObject> sourceGraphPoint = GetGraphPointFromRouteItem(graphResult, sourceRouteCombination.FromType, routeItem.Source.ACClassID);
                        GraphPoint<IACObject> targetGraphPoint = GetGraphPointFromRouteItem(graphResult, targetRouteCombination.ToType, routeItem.Target.ACClassID);

                        if (sourceGraphPoint != null && targetGraphPoint != null)
                        {
                            GraphPointRelation<IACObject> relation = new GraphPointRelation<IACObject>(sourceGraphPoint, targetGraphPoint, GraphPointRelationTypeEnum.Routing);
                            if (!routingRelations.Contains(relation))
                                routingRelations.Add(relation);
                        }
                        else
                        {
                            // build passing relation
                            if (sourceGraphPoint == null)
                            {
                                IACObject item = routeItem.Source;
                                Guid itemID = routeItem.Source.ACClassID;
                                MDTrackingStartItemTypeEnum itemType = MDTrackingStartItemTypeEnum.ACClass;
                                string itemNo = routeItem.Source.ACIdentifier;
                                if (FacilityIDAndACClassIDs.Keys.Contains(itemID))
                                {
                                    Facility facility = FacilityIDAndACClassIDs[itemID];
                                    item = new FacilityPreview()
                                    {
                                        FacilityID = facility.FacilityID,
                                        FacilityName = facility.FacilityName,
                                        FacilityNo = facility.FacilityNo
                                    };
                                    itemID = facility.FacilityID;
                                    itemType = MDTrackingStartItemTypeEnum.FacilityPreview;
                                    itemNo = facility.FacilityNo;
                                }
                                sourceGraphPoint = new GraphPoint<IACObject>() { Item = item, ItemID = itemID, ItemType = itemType, ItemNo = itemNo, IsVirtual = true };
                            }
                            if (targetGraphPoint == null)
                            {
                                IACObject item = routeItem.Target;
                                Guid itemID = routeItem.Target.ACClassID;
                                MDTrackingStartItemTypeEnum itemType = MDTrackingStartItemTypeEnum.ACClass;
                                string itemNo = routeItem.Source.ACIdentifier;
                                if (FacilityIDAndACClassIDs.Keys.Contains(itemID))
                                {
                                    Facility facility = FacilityIDAndACClassIDs[itemID];
                                    item = new FacilityPreview()
                                    {
                                        FacilityID = facility.FacilityID,
                                        FacilityName = facility.FacilityName,
                                        FacilityNo = facility.FacilityNo
                                    };
                                    itemID = facility.FacilityID;
                                    itemType = MDTrackingStartItemTypeEnum.FacilityPreview;
                                    itemNo = facility.FacilityNo;
                                }
                                targetGraphPoint = new GraphPoint<IACObject>() { Item = item, ItemID = itemID, ItemType = itemType, ItemNo = itemNo, IsVirtual = true };
                            }
                            GraphPointRelation<IACObject> relation = new GraphPointRelation<IACObject>(sourceGraphPoint, targetGraphPoint, GraphPointRelationTypeEnum.Routing);
                            if (!routingRelations.Contains(relation))
                                routingRelations.Add(relation);
                        }
                    }
                }
            }
            return routingRelations;
        }

        private GraphPoint<IACObject> GetGraphPointFromRouteItem(GraphPointResult<IACObject> graphResult, MDTrackingStartItemTypeEnum itemType, Guid acClassID)
        {
            GraphPoint<IACObject> graphPoint = null;
            if (itemType == MDTrackingStartItemTypeEnum.ACClass)
                graphPoint = graphResult.Points.FirstOrDefault(c => c.ItemID == acClassID);
            else if (itemType == MDTrackingStartItemTypeEnum.FacilityPreview)
                graphPoint =
                    graphResult
                    .Points
                    .Where(c => c.Item is FacilityPreview && (c.Item as FacilityPreview) != null && ((c.Item as FacilityPreview).VBiFacilityACClassID ?? Guid.Empty) == acClassID)
                    .FirstOrDefault();
            return graphPoint;
        }

        #endregion

    }
}
