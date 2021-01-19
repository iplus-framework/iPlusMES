using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    public class TandTv2Result
    {

        #region ctor's

        #endregion

        #region Success info result
        public bool Success { get; set; }

        public MsgWithDetails ErrorMsg { get; set; }
        #endregion

        #region Tracking Data Members

        public TandTv2Job Job { get; set; }

        private List<TandTv2Step> _Steps;
        public List<TandTv2Step> Steps
        {
            get
            {
                if (_Steps == null)
                    _Steps = new List<TandTv2Step>();
                return _Steps;
            }
            set
            {
                _Steps = value;
            }
        }

        private List<TandTv2TempPos> _TempPoses;
        public List<TandTv2TempPos> TempPoses
        {
            get
            {
                if (_TempPoses == null)
                    _TempPoses = new List<TandTv2TempPos>();
                return _TempPoses;
            }
            set

            {
                _TempPoses = value; ;
            }
        }
        public List<TandTv2StepItem> FilteredStepItems { get; set; }

        private List<TandTv2StepItem> _StepItems;
        public List<TandTv2StepItem> StepItems
        {
            get
            {
                if (_StepItems == null)
                    _StepItems = new List<TandTv2StepItem>();
                return _StepItems;
            }
            set
            {
                _StepItems = value;
            }
        }

        private List<TandTv2StepItemRelation> _StepItemRelations;
        public List<TandTv2StepItemRelation> StepItemRelations
        {
            get
            {
                if (_StepItemRelations == null)
                    _StepItemRelations = new List<TandTv2StepItemRelation>();
                return _StepItemRelations;
            }
            set
            {
                _StepItemRelations = value;
            }
        }

        private List<TandTv2StepItemRelation> _FilteredStepItemRelations;
        public List<TandTv2StepItemRelation> FilteredStepItemRelations
        {
            get
            {
                if (_FilteredStepItemRelations == null)
                    _FilteredStepItemRelations = new List<TandTv2StepItemRelation>();
                return _FilteredStepItemRelations;
            }
            set
            {
                _FilteredStepItemRelations = value;
            }
        }

        private List<TandTv2StepLot> _StepLots;
        public List<TandTv2StepLot> StepLots
        {
            get
            {
                if (_StepLots == null)
                    _StepLots = new List<TandTv2StepLot>();
                return _StepLots;
            }
            set
            {
                _StepLots = value;
            }
        }


        public TandTv2Step LastStep { get; set; }


        private List<IDoItem> _ItemsForSearchNext;
        public List<IDoItem> ItemsForSearchNext
        {
            get
            {
                if (_ItemsForSearchNext == null)
                    _ItemsForSearchNext = new List<IDoItem>();
                return _ItemsForSearchNext;
            }
            set
            {
                _ItemsForSearchNext = value;
            }
        }

        private List<Guid> _IDs;
        public List<Guid> IDs
        {
            get
            {
                if (_IDs == null) _IDs = new List<Guid>();
                return _IDs;
            }
        }

        private List<Guid> _BatchIDs;
        public List<Guid> BatchIDs
        {
            get
            {
                if (_BatchIDs == null) _BatchIDs = new List<Guid>();
                return _BatchIDs;
            }
        }

        public List<FacilityChargeModel> FacilityChargeModels { get; set; }

        #endregion

        #region Builded Graphical Elements

        private List<PAEdge> _Edges;
        public List<PAEdge> Edges
        {
            get
            {
                if (_Edges == null)
                    _Edges = new List<PAEdge>();
                return _Edges;
            }
        }

        public List<List<ACRoutingPath>> AvailableRoutes
        {
            get
            {
                {
                    ACRoutingPath tmp = new ACRoutingPath();
                    tmp.AddRange(Edges);
                    return new List<List<ACRoutingPath>>()
                    {
                        new List<ACRoutingPath>()
                        {
                            tmp
                        }
                    };
                }
            }
        }

        public List<IACComponent> ActiveRouteComponents
        {
            get
            {
                if (FilteredStepItems == null) return null;
                return FilteredStepItems.Cast<IACComponent>().ToList();
            }
        }

        public List<IACObject> ActiveRoutePaths
        {
            get
            {
                if (Edges == null) return null;
                return Edges.Cast<IACObject>().ToList();
            }
        }
        #endregion

        #region Important Material and Manufacturing Items: DeliveryNote, LabOrder

        public List<TandTv2StepItem> ItemsWithLabOrder { get; set; }

        public List<DeliveryNotePosPreview> DeliveryNotes { get; set; }

        public void DoProcess(DatabaseApp databaseApp, TandTv2Job jobFilter)
        {
            LastStep = TandTv2Command.GetNextStep(databaseApp, Job, LastStep.StepNo);
            Job.TandTv2Step_TandTv2Job.Add(LastStep);
            Steps.Add(LastStep);
            List<IDoItem> sameStepItems = new List<IDoItem>();
            List<IDoItem> nextStepItems = new List<IDoItem>();
            foreach (var item in ItemsForSearchNext)
            {
                if (!IDs.Contains(item.StepItem.PrimaryKeyID))
                    IDs.Add(item.StepItem.PrimaryKeyID);
                List<IDoItem> relatedSameStep = ProcessItemSameStep(databaseApp, jobFilter, item);
                if (relatedSameStep != null && relatedSameStep.Any())
                {
                    relatedSameStep.Remove(item);
                    foreach (var relatedSameStepItem in relatedSameStep)
                        if (!sameStepItems.Select(c => c.StepItem.PrimaryKeyID).Contains(relatedSameStepItem.StepItem.PrimaryKeyID))
                            sameStepItems.Add(relatedSameStepItem);
                }
            }
            foreach (var sameStepItem in sameStepItems)
            {
                List<IDoItem> sameStepItemRelatedItems = sameStepItem.SearchRelatedNextStep(databaseApp, this, jobFilter);
                if (sameStepItemRelatedItems != null)
                {
                    sameStepItemRelatedItems =
                        sameStepItemRelatedItems
                        .Where(c =>
                            !nextStepItems.Select(ni => ni.StepItem.PrimaryKeyID).Contains(c.StepItem.PrimaryKeyID)
                         ).ToList();

                    if (sameStepItemRelatedItems != null && sameStepItemRelatedItems.Any())
                    {
                        CheckPoint(sameStepItemRelatedItems);
                    }
                    foreach (var sameStepItemRelatedItem in sameStepItemRelatedItems)
                        if (!nextStepItems.Select(c => c.StepItem.PrimaryKeyID).Contains(sameStepItemRelatedItem.StepItem.PrimaryKeyID))
                            nextStepItems.Add(sameStepItemRelatedItem);

                }
            }
            foreach (var item in ItemsForSearchNext)
            {
                List<IDoItem> tmpNextStepItems = item.SearchRelatedNextStep(databaseApp, this, jobFilter);
                if (tmpNextStepItems != null && tmpNextStepItems.Any())
                {
                    CheckPoint(tmpNextStepItems);
                    item.BuildRelations(this, item.StepItem, tmpNextStepItems.Select(c => c.StepItem).ToList());
                    tmpNextStepItems =
                        tmpNextStepItems
                        .Where(c =>
                            !nextStepItems.Select(ni => ni.StepItem.PrimaryKeyID).Contains(c.StepItem.PrimaryKeyID)
                         ).ToList();
                    foreach (var tmpNextStepItem in tmpNextStepItems)
                        if (!nextStepItems.Select(c => c.StepItem.PrimaryKeyID).Contains(tmpNextStepItem.StepItem.PrimaryKeyID))
                            nextStepItems.Add(tmpNextStepItem);
                }
            }
            nextStepItems = nextStepItems.Where(c => !IDs.Contains(c.StepItem.PrimaryKeyID)).ToList();

            if (sameStepItems != null && sameStepItems.Any())
            {
                CheckPoint(sameStepItems);
            }
            if (nextStepItems != null && nextStepItems.Any())
            {
                CheckPoint(nextStepItems);
            }
            ItemsForSearchNext = nextStepItems;
            if (ItemsForSearchNext != null && ItemsForSearchNext.Any())
                DoProcess(databaseApp, jobFilter);
        }

        private void CheckPoint(List<IDoItem> items)
        {
            //DateTime tmpDate = new DateTime(2019, 1, 30, 0, 0, 0);
            //List<TandT_ItemTypeEnum> excludedItems = new List<TandT_ItemTypeEnum>() { TandT_ItemTypeEnum.FacilityBookingCharge };
            //List<IDoItem> filtererd = items
            //    .Where(c => excludedItems.Contains(c.StepItem.TandT_ItemTypeEnum))
            //    .Where(x => x.StepItem.InsertDate != null && x.StepItem.InsertDate < tmpDate)
            //.ToList();
            //if (filtererd.Any())
            //    System.Diagnostics.Debugger.Break();
        }

        private List<IDoItem> ProcessItemSameStep(DatabaseApp databaseApp, TandTv2Job jobFilter, IDoItem item)
        {
            List<IDoItem> finallProcessedItems = new List<IDoItem>();
            finallProcessedItems.Add(item);
            List<IDoItem> itemsProcessed = item.ProcessRelatedSameStep(databaseApp, this, jobFilter);
            if (itemsProcessed != null && itemsProcessed.Any())
            {
                itemsProcessed = itemsProcessed.Where(c => !IDs.Contains(c.StepItem.PrimaryKeyID)).ToList();
                foreach (var tmpItem in itemsProcessed)
                    finallProcessedItems.AddRange(ProcessItemSameStep(databaseApp, jobFilter, tmpItem));
            }
            return finallProcessedItems;
        }
        #endregion

        #region Helper fields

        #endregion
    }
}
