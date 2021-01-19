using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace gip.mes.facility
{
    public static class TandTv2Command
    {

        public static TandTv2Result DoTracking(DatabaseApp databaseApp, TandTv2Job jobFilter, string vBUserNo)
        {
            TandTv2Result result = null;
            TandTv2Job job = GetJob(databaseApp, jobFilter, vBUserNo);
            if (job.EntityState != System.Data.EntityState.Added)
            {
                if (jobFilter.RecalcAgain)
                {
                    databaseApp.udpTandTv2JobDelete(job.TandTv2JobID);
                    job = GetJob(databaseApp, jobFilter, vBUserNo);
                }
                else
                {
                    result = DoSelect(databaseApp, job);
                }
            }
            if (job.EntityState == System.Data.EntityState.Added)
            {
                result = new TandTv2Result();
                result.Job = job;
                result.LastStep = GetNextStep(databaseApp, job, 0);
                result.Job.TandTv2Step_TandTv2Job.Add(result.LastStep);
                List<IDoItem> itemsForSearchNext = new List<IDoItem>();
                switch (jobFilter.TrackingStyleEnum)
                {
                    case TandTv2TrackingStyleEnum.Backward:
                        itemsForSearchNext = GetDoItemBackward(databaseApp, result, jobFilter);
                        break;
                    case TandTv2TrackingStyleEnum.Forward:
                        itemsForSearchNext = GetDoItemForward(databaseApp, result, jobFilter);
                        break;
                }
                result.ItemsForSearchNext = itemsForSearchNext;
                result.DoProcess(databaseApp, jobFilter);
                result.Job.EndTime = DateTime.Now;
                core.datamodel.MsgWithDetails saveMsg = null;
                if (!jobFilter.IsDynamic)
                {
                    saveMsg = databaseApp.ACSaveChanges();
                }
                if (saveMsg != null && saveMsg.MessageLevel == core.datamodel.eMsgLevel.Error)
                {
                    result.Success = false;
                    result.ErrorMsg.AddDetailMessage(saveMsg);
                }
                else
                    result.Success = true;
            }
            BuildRelatedItems(databaseApp, result);
            return result;
        }

        public static TandTv2Result DoSelect(DatabaseApp databaseApp, TandTv2Job job)
        {
            TandTv2Result result = new TandTv2Result();
            result.Job = databaseApp.TandTv2Job.FirstOrDefault(c => c.TandTv2JobID == job.TandTv2JobID);
            result.Steps = databaseApp.TandTv2Step.Where(c => c.TandTv2JobID == job.TandTv2JobID).OrderBy(c => c.StepNo).ToList();
            result.StepItems =
                databaseApp
                .TandTv2StepItem
                .Include(c => c.TandTv2ItemType)
                .Include(c => c.TandTv2Step)
                .Where(c => c.TandTv2Step.TandTv2JobID == job.TandTv2JobID)
                .OrderBy(c => c.TandTv2Step.StepNo)
                .ToList();
            result.StepLots =
                databaseApp
                .TandTv2Step.
                Where(c => c.TandTv2JobID == job.TandTv2JobID)
                .OrderBy(c => c.StepNo)
                .SelectMany(c => c.TandTv2StepLot_TandTv2Step)
                .ToList();
            result.StepItemRelations = databaseApp.TandTv2StepItemRelation.Where(c => c.SourceTandTv2StepItem.TandTv2Step.TandTv2JobID == job.TandTv2JobID).ToList();
            BuildRelatedItems(databaseApp, result);
            result.Success = true;
            return result;
        }

        public static void JobDelete(DatabaseApp databaseApp, Guid? jobID)
        {
            databaseApp.udpTandTv2JobDelete(jobID);
        }

        public static TandTv2Job GetJob(DatabaseApp databaseApp, TandTv2Job jobFilter, string vBUserNo)
        {
            IEnumerable<TandTv2Job> queryJob = databaseApp.TandTv2Job.Where(c =>
                c.TandTv2TrackingStyleID == jobFilter.TandTv2TrackingStyleID &&
                c.TandTv2ItemTypeID == jobFilter.TandTv2ItemTypeID &&
                ((c.FilterDateFrom ?? (new DateTime())) == (jobFilter.FilterDateFrom ?? (new DateTime()))) &&
                ((c.FilterDateTo ?? (new DateTime())) == (jobFilter.FilterDateTo ?? (new DateTime()))) &&
                c.PrimaryKeyID == jobFilter.PrimaryKeyID);
            TandTv2Job job = null;
            if (jobFilter.MaterialIDs != null && jobFilter.MaterialIDs.Any())
            {
                foreach (TandTv2Job tmpJob in queryJob)
                {
                    List<Guid> tmpMaterialIds = tmpJob.TandTv2JobMaterial_TandTv2Job.Select(c => c.MaterialID).ToList();
                    if (tmpMaterialIds.SequenceEqual(jobFilter.MaterialIDs))
                    {
                        job = tmpJob;
                        break;
                    }
                }
            }
            else
            {
                job = queryJob.FirstOrDefault();
            }


            if (job == null)
            {
                job = new TandTv2Job()
                {
                    TandTv2JobID = Guid.NewGuid(),
                    TandTv2TrackingStyleID = jobFilter.TandTv2TrackingStyleID,
                    TandTv2ItemTypeID = jobFilter.TandTv2ItemTypeID,
                    JobNo = "#",
                    FilterDateFrom = jobFilter.FilterDateFrom,
                    FilterDateTo = jobFilter.FilterDateTo,
                    ItemSystemNo = jobFilter.ItemSystemNo,
                    PrimaryKeyID = jobFilter.PrimaryKeyID,
                    StartTime = DateTime.Now,
                    InsertName = vBUserNo
                };

                if (jobFilter.MaterialIDs != null && jobFilter.MaterialIDs.Any())
                    foreach (Guid materialID in jobFilter.MaterialIDs)
                    {
                        TandTv2JobMaterial dbJobMaterial = new TandTv2JobMaterial();
                        dbJobMaterial.TandTv2JobMaterialID = Guid.NewGuid();
                        Material tmpMaterial = databaseApp.Material.FirstOrDefault(c => c.MaterialID == materialID);
                        dbJobMaterial.Material = tmpMaterial;
                        dbJobMaterial.TandTv2Job = job;
                        job.TandTv2JobMaterial_TandTv2Job.Add(dbJobMaterial);
                    }
                databaseApp.TandTv2Job.AddObject(job);
            }
            return job;
        }

        public static TandTv2Step GetNextStep(DatabaseApp databaseApp, TandTv2Job job, int lastStepNo)
        {
            TandTv2Step step = new TandTv2Step();
            step.TandTv2StepID = Guid.NewGuid();
            step.StepNo = lastStepNo + 1;
            step.StepName = string.Format(@"Step No. {0}", step.StepNo);
            step.TandTv2Job = job;
            return step;
        }

        private static List<IDoItem> GetDoItemBackward(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> itemsForSearchNext = new List<IDoItem>();
            switch (jobFilter.ItemTypeEnum)
            {
                case TandTv2ItemTypeEnum.DeliveryNotePos:
                    DeliveryNotePos deliveryNotePos = databaseApp.DeliveryNotePos.FirstOrDefault(c => c.DeliveryNotePosID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoBackward_DeliveryNotePos(databaseApp, result, deliveryNotePos, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.FacilityBooking:
                    FacilityBooking facilityBooking = databaseApp.FacilityBooking.FirstOrDefault(c => c.FacilityBookingID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.AddRange(
                            facilityBooking
                            .FacilityBookingCharge_FacilityBooking
                            .Select(c => new DoBackward_FacilityBookingCharge(databaseApp, result, c, jobFilter))
                            .ToList());
                    break;
                case TandTv2ItemTypeEnum.FacilityBookingCharge:
                    FacilityBookingCharge facilityBookingCharge = databaseApp.FacilityBookingCharge.FirstOrDefault(c => c.FacilityBookingChargeID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoBackward_FacilityBookingCharge(databaseApp, result, facilityBookingCharge, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.FacilityCharge:
                    FacilityCharge facilityCharge = databaseApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoBackward_FacilityCharge(databaseApp, result, facilityCharge, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.FacilityLot:
                    FacilityLot facilityLot = databaseApp.FacilityLot.FirstOrDefault(c => c.FacilityLotID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoBackward_FacilityLot(databaseApp, result, facilityLot, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.InOrderPos:
                    InOrderPos inOrderPos = databaseApp.InOrderPos.FirstOrDefault(c => c.InOrderPosID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoBackward_InOrderPos(databaseApp, result, inOrderPos, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.OutOrderPos:
                    OutOrderPos outOrderPos = databaseApp.OutOrderPos.FirstOrDefault(c => c.OutOrderPosID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoBackward_OutOrderPos(databaseApp, result, outOrderPos, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.ProdOrderPartslistPos:
                    ProdOrderPartslistPos prodOrderPartslistPos = databaseApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoBackward_ProdOrderPartslistPos(databaseApp, result, prodOrderPartslistPos, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.ProdOrderPartslistPosRelation:
                    ProdOrderPartslistPosRelation prodOrderPartslistPosRelation = databaseApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoBackward_ProdOrderPartslistPosRelation(databaseApp, result, prodOrderPartslistPosRelation, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.FacilityPreBooking:
                    FacilityPreBooking facilityPreBooking = databaseApp.FacilityPreBooking.FirstOrDefault(c => c.FacilityPreBookingID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoBackward_FacilityPreBooking(databaseApp, result, facilityPreBooking, jobFilter));
                    break;
            }
            return itemsForSearchNext;
        }

        private static List<IDoItem> GetDoItemForward(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> itemsForSearchNext = new List<IDoItem>();
            switch (jobFilter.ItemTypeEnum)
            {
                case TandTv2ItemTypeEnum.DeliveryNotePos:
                    DeliveryNotePos deliveryNotePos = databaseApp.DeliveryNotePos.FirstOrDefault(c => c.DeliveryNotePosID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoForward_DeliveryNotePos(databaseApp, result, deliveryNotePos, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.FacilityBooking:
                    FacilityBooking facilityBooking = databaseApp.FacilityBooking.FirstOrDefault(c => c.FacilityBookingID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.AddRange(
                            facilityBooking
                            .FacilityBookingCharge_FacilityBooking
                            .Select(c => new DoForward_FacilityBookingCharge(databaseApp, result, c, jobFilter))
                            .ToList());
                    break;
                case TandTv2ItemTypeEnum.FacilityBookingCharge:
                    FacilityBookingCharge facilityBookingCharge = databaseApp.FacilityBookingCharge.FirstOrDefault(c => c.FacilityBookingChargeID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoForward_FacilityBookingCharge(databaseApp, result, facilityBookingCharge, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.FacilityCharge:
                    FacilityCharge facilityCharge = databaseApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoForward_FacilityCharge(databaseApp, result, facilityCharge, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.FacilityLot:
                    FacilityLot facilityLot = databaseApp.FacilityLot.FirstOrDefault(c => c.FacilityLotID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoForward_FacilityLot(databaseApp, result, facilityLot, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.InOrderPos:
                    InOrderPos inOrderPos = databaseApp.InOrderPos.FirstOrDefault(c => c.InOrderPosID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoForward_InOrderPos(databaseApp, result, inOrderPos, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.OutOrderPos:
                    OutOrderPos outOrderPos = databaseApp.OutOrderPos.FirstOrDefault(c => c.OutOrderPosID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoForward_OutOrderPos(databaseApp, result, outOrderPos, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.ProdOrderPartslistPos:
                    ProdOrderPartslistPos prodOrderPartslistPos = databaseApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoForward_ProdOrderPartslistPos(databaseApp, result, prodOrderPartslistPos, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.ProdOrderPartslistPosRelation:
                    ProdOrderPartslistPosRelation prodOrderPartslistPosRelation = databaseApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoForward_ProdOrderPartslistPosRelation(databaseApp, result, prodOrderPartslistPosRelation, jobFilter));
                    break;
                case TandTv2ItemTypeEnum.FacilityPreBooking:
                    FacilityPreBooking facilityPreBooking = databaseApp.FacilityPreBooking.FirstOrDefault(c => c.FacilityPreBookingID == jobFilter.PrimaryKeyID);
                    itemsForSearchNext.Add(new DoForward_FacilityPreBooking(databaseApp, result, facilityPreBooking, jobFilter));
                    break;
            }
            return itemsForSearchNext;
        }

        private static void BuildRelatedItems(DatabaseApp databaseApp, TandTv2Result result)
        {
            // Build LabOrder list
            result.ItemsWithLabOrder =
                result
                .StepItems
                .Where(c =>
                    (c.InOrderPosID != null && c.InOrderPos.LabOrder_InOrderPos.Any()) ||
                    (c.OutOrderPosID != null && c.OutOrderPos.LabOrder_OutOrderPos.Any()) ||
                    (c.ProdOrderPartslistPosID != null && c.ProdOrderPartslistPos.LabOrder_ProdOrderPartslistPos.Any())
                )
                .ToList();

            // BuildDeliveryNote
            result.DeliveryNotes =
                result
                .StepItems
                .Where(c => c.DeliveryNotePosID != null)
                .Select(c => c.DeliveryNotePos)
                .OrderBy(c => c.DeliveryNote.DeliveryNoteNo)
                .Select(c => new DeliveryNotePosPreview(c))
                .ToList();

            int nr = 0;
            if (result.DeliveryNotes != null)
                foreach (var item in result.DeliveryNotes)
                {
                    nr++;
                    item.Sn = nr;
                }

            var queryStepNoChargeId =
                result
                .StepItems
                .Where(c => c.FacilityChargeID != null)
                .Select(c => new { FacilityChargeID = c.FacilityChargeID ?? Guid.Empty, c.TandTv2Step.StepNo })
                .Distinct()
                .ToList();

            var chargeIds = queryStepNoChargeId.Select(c => c.FacilityChargeID).Distinct().ToList();
            var queryFc = databaseApp.FacilityCharge.Where(c => chargeIds.Contains(c.FacilityChargeID));

            result.FacilityChargeModels = FacilityCharge.GetFacilityChargeModelList(databaseApp, queryFc).ToList();
            if (result.FacilityChargeModels != null)
            {
                foreach (var item in result.FacilityChargeModels)
                {
                    var tmp = queryStepNoChargeId.FirstOrDefault(c => c.FacilityChargeID == item.FacilityChargeID);
                    if (tmp != null)
                        item.StepNo = tmp.StepNo;
                }
                result.FacilityChargeModels = result.FacilityChargeModels.OrderBy(c => c.StepNo).ToList();
            }


            // Prepare Operation and ItemType for offline
            string operationID = Enum.GetName(typeof(TandTv2OperationEnum), TandTv2OperationEnum.BW_FB_START);
            TandTv2Operation operation = databaseApp.TandTv2Operation.FirstOrDefault(c => c.TandTv2OperationID == operationID);
            List<TandTv2ItemType> itemTypes = databaseApp.TandTv2ItemType.ToList();
            if (result.StepItems != null)
                foreach (TandTv2StepItem stepItem in result.StepItems)
                {
                    stepItem.TandTv2Operation = operation;
                    stepItem.TandTv2ItemType = itemTypes.FirstOrDefault(c => c.TandTv2ItemTypeID == stepItem.TandTv2ItemTypeID);
                }
        }

        public static void BuildStepItemHelperSourceTargetList(TandTv2Result result)
        {
            // Build neighborhood
            foreach (TandTv2StepItemRelation item in result.StepItemRelations)
            {
                //if (item.SourceStepItem.HelperTargetItems == null)
                //    item.SourceStepItem.HelperTargetItems = new List<TandTv2StepItem>();

                //if (item.TargetStepItem.HelperSourceItems == null)
                //    item.TargetStepItem.HelperSourceItems = new List<TandTv2StepItem>();

                //if (!item.SourceStepItem.HelperTargetItems.Select(c => c.StepItemID).Contains(item.TargetStepItemID))
                //    item.SourceStepItem.HelperTargetItems.Add(item.TargetStepItem);

                //if (!item.TargetStepItem.HelperSourceItems.Select(c => c.StepItemID).Contains(item.SourceStepItemID))
                //    item.TargetStepItem.HelperSourceItems.Add(item.SourceStepItem);
            }

            // process filtered item

        }

        public static void BuildFilteredStepItemHelperSourceTargetList(TandTv2Result result)
        {

            foreach (var filteredStepItem in result.FilteredStepItems)
            {
                //if (filteredStepItem.HelperSourceItems != null)
                //{
                //    List<TandTv2StepItem> sourceItems = filteredStepItem.HelperSourceItems.ToList();
                //    foreach (var source in filteredStepItem.HelperSourceItems)
                //    {
                //        if (!result.FilteredStepItems.Select(c => c.StepItemID).Contains(source.StepItemID))
                //        {
                //            sourceItems.Remove(source);
                //            sourceItems.AddRange(GetSourceItems(result, source));
                //        }
                //    }
                //    filteredStepItem.HelperSourceItems = sourceItems;
                //}

                //if (filteredStepItem.HelperTargetItems != null)
                //{
                //    List<TandTv2StepItem> targetItems = filteredStepItem.HelperTargetItems.ToList();
                //    foreach (var target in filteredStepItem.HelperTargetItems)
                //    {
                //        if (!result.FilteredStepItems.Select(c => c.StepItemID).Contains(target.StepItemID))
                //        {
                //            targetItems.Remove(target);
                //            targetItems.AddRange(GetTargetItems(result, target));
                //        }
                //    }
                //    filteredStepItem.HelperTargetItems = targetItems;
                //}
            }
        }

        private static List<TandTv2StepItem> GetSourceItems(TandTv2Result result, TandTv2StepItem source)
        {
            List<TandTv2StepItem> items = new List<TandTv2StepItem>();
            //if (source.HelperSourceItems != null)
            //{
            //    items = source.HelperSourceItems.Where(sourceItem => result.FilteredStepItems.Select(c => c.StepItemID).Contains(sourceItem.StepItemID)).ToList();
            //    List<TandTv2StepItem> notFilteredItems = source.HelperSourceItems.Where(sourceItem => !result.FilteredStepItems.Select(c => c.StepItemID).Contains(sourceItem.StepItemID)).ToList();
            //    foreach (var notFilteredItem in notFilteredItems)
            //    {
            //        items.AddRange(GetSourceItems(result, notFilteredItem));
            //    }
            //}
            return items;
        }

        private static List<TandTv2StepItem> GetTargetItems(TandTv2Result result, TandTv2StepItem source)
        {
            List<TandTv2StepItem> items = new List<TandTv2StepItem>();
            //if (source.HelperTargetItems != null)
            //{
            //    items = source.HelperTargetItems.Where(targetItem => result.FilteredStepItems.Select(c => c.StepItemID).Contains(targetItem.StepItemID)).ToList();
            //    List<TandTv2StepItem> notFilteredItems = source.HelperTargetItems.Where(targetItem => !result.FilteredStepItems.Select(c => c.StepItemID).Contains(targetItem.StepItemID)).ToList();
            //    foreach (var notFilteredItem in notFilteredItems)
            //    {
            //        items.AddRange(GetTargetItems(result, notFilteredItem));
            //    }
            //}
            return items;
        }

        /// <summary>
        /// Build relations on HelperSourceItems and HelperTargetItems lists
        /// </summary>
        /// <param name="databaseApp"></param>
        /// <param name="result"></param>
        public static void BuildFilteredStepItemRelations(TandTv2Result result)
        {
            List<TandTv2StepItemRelation> stepItemRelations = new List<TandTv2StepItemRelation>();
            foreach (var filteredStepItem in result.FilteredStepItems)
            {
                //if (filteredStepItem.HelperSourceItems != null)
                //    foreach (var source in filteredStepItem.HelperSourceItems)
                //    {
                //        if (!stepItemRelations.Any(c => c.SourceStepItemID == source.StepItemID && c.TargetStepItemID == filteredStepItem.StepItemID))
                //        {
                //            TandTv2StepItemRelation tmpRelation = new TandTv2StepItemRelation();
                //            tmpRelation.StepItemRelationID = Guid.NewGuid();
                //            tmpRelation.TandT_RelationTypeEnum = TandT_RelationTypeEnum.TrackingFlow;
                //            tmpRelation.SourceStepItem = source;
                //            tmpRelation.TargetStepItem = filteredStepItem;
                //            stepItemRelations.Add(tmpRelation);

                //        }
                //    }
                //if (filteredStepItem.HelperTargetItems != null)
                //    foreach (var target in filteredStepItem.HelperTargetItems)
                //    {
                //        if (!stepItemRelations.Any(c => c.SourceStepItemID == filteredStepItem.StepItemID && c.TargetStepItemID == target.StepItemID))
                //        {
                //            TandTv2StepItemRelation tmpRelation = new TandTv2StepItemRelation();
                //            tmpRelation.StepItemRelationID = Guid.NewGuid();
                //            tmpRelation.TandT_RelationTypeEnum = TandT_RelationTypeEnum.TrackingFlow;
                //            tmpRelation.SourceStepItem = filteredStepItem;
                //            tmpRelation.TargetStepItem = target;
                //            stepItemRelations.Add(tmpRelation);
                //        }
                //    }
            }

            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                foreach (var item in stepItemRelations)
                {
                    databaseApp.ObjectStateManager.ChangeObjectState(item, EntityState.Unchanged);
                }
            }
            result.FilteredStepItemRelations = stepItemRelations;
        }

        public static void BuildEdges(TandTv2Result result)
        {
            result.Edges.Clear();
            foreach (TandTv2StepItemRelation item in result.FilteredStepItemRelations)
            {
                //PAEdge edge = new PAEdge(item.SourceStepItem.PAPointMatOut1, item.TargetStepItem.PAPointMatOut1, new core.datamodel.ACClassPropertyRelation());
                //result.Edges.Add(edge);
                //(item.SourceStepItem.PAPointMatOut1.ConnectionList as List<PAEdge>).Add(edge);
                //(item.TargetStepItem.PAPointMatOut1.ConnectionList as List<PAEdge>).Add(edge);
            }
        }

    }
}
