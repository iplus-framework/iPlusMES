using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility.TandTv3.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace gip.mes.facility.TandTv3
{

    public class TandTv3Command
    {

        #region settings

        public static string EmptyLotName = "0000";

        public string LogFileNameTemplate = @"TandTV3-log-{0}-{1}.log";


        /// <summary>
        /// Use for save log file into other folder then Temp Windows Folder
        /// </summary>
        public string RootLogFolder { get; set; }


        private TrackingConfiguration _TrackingConfiguration;
        public TrackingConfiguration TrackingConfiguration
        {
            get

            {
                if (_TrackingConfiguration == null)
                    _TrackingConfiguration = (TrackingConfiguration)CommandLineHelper.ConfigCurrentDir.GetSection("trackingConfiguration");
                return _TrackingConfiguration;
            }
        }

        public bool TandTWriteDiagnosticLog
        {
            get
            {
                return TrackingConfiguration == null ? false : TrackingConfiguration.TandTWriteDiagnosticLog;
            }
        }

        #endregion

        #region ctor's

        public TandTv3Command()
        {
            if (TrackingConfiguration != null && !string.IsNullOrEmpty(TrackingConfiguration.RootLogFolder))
                RootLogFolder = TrackingConfiguration.RootLogFolder;
        }

        #endregion

        #region Properties

        #endregion

        #region Common TandTv3Command Interface

        public IACObjectEntity FactoryObject(DatabaseApp databaseApp, TandTv3FilterTracking filter)
        {
            EntityKey entityKey = new EntityKey(databaseApp.DefaultContainerName + "." + filter.TandTv3MDTrackingStartItemTypeID.ToString(), filter.TandTv3MDTrackingStartItemTypeID.ToString() + "ID", filter.PrimaryKeyID);
            return databaseApp.GetObjectByKey(entityKey) as IACObjectEntity;
        }

        public virtual TandTResult PrepareTracking(DatabaseApp databaseApp, TandTv3FilterTracking filter, string vbUserNo)
        {
            TandTv3FilterTracking dbFilter = ProcessFilter(databaseApp, filter, vbUserNo);
            return FactoryTandTResult(dbFilter);
        }

        public virtual TandTResult FactoryTandTResult(TandTv3FilterTracking filter)
        {
            return new TandTResult() { Filter = filter };
        }

        public TandTv3FilterTracking ProcessFilter(DatabaseApp databaseApp, TandTv3FilterTracking filter, string vBUserNo)
        {
            TandTv3FilterTracking dbFilter = null;
            if (filter.RecalcAgain)
            {
                dbFilter = databaseApp.TandTv3FilterTracking.FirstOrDefault(c => c.ItemSystemNo == filter.ItemSystemNo);
                if (dbFilter != null)
                    databaseApp.udpTandTv3FilterTrackingDelete(dbFilter.TandTv3FilterTrackingID);
            }
            dbFilter = GetFilter(databaseApp, filter, vBUserNo);
            return dbFilter;
        }

        public void DoTrackingFinish(DatabaseApp databaseApp, TandTResult result, TandTv3FilterTracking filter)
        {
            foreach (var mixPoint in result.MixPoints)
                mixPoint.Finish();

            Calculations(databaseApp, result, filter.MDTrackingDirectionEnum);
            bool machineInfoExist = FetchRelatedProgramLogs(databaseApp, result);
            if (machineInfoExist)
            {
                FetchRelatedMachines(databaseApp, result);
                foreach (var mixPoint in result.MixPoints)
                {
                    foreach (var machine in mixPoint.InwardMachines)
                        if (!result.Ids.Keys.Contains(machine.ACClassID))
                            result.Ids.Add(machine.ACClassID, MDTrackingStartItemTypeEnum.ACClass.ToString());
                    foreach (var machine in mixPoint.OutwardMachines)
                        if (!result.Ids.Keys.Contains(machine.ACClassID))
                            result.Ids.Add(machine.ACClassID, MDTrackingStartItemTypeEnum.ACClass.ToString());
                }
            }
            FetchRelatedFacilityACClasses(databaseApp, result);

        }



        /// <summary>
        /// Doing select for existing cached job
        /// </summary>
        /// <param name="databaseApp"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        public TandTResult DoSelect(DatabaseApp databaseApp, TandTv3FilterTracking filter)
        {
            TandTResult result = FactoryTandTResult(filter);
            result.Filter = filter;
            result.StartTime = DateTime.Now;
            List<TandTv3Step> steps = filter.TandTv3Step_TandTv3FilterTracking.ToList();
            int stepNo = 0;
            int stepCount = steps.Count();
            if (filter.BackgroundWorker != null)
            {
                filter.BackgroundWorker.ProgressInfo.ProgressInfoIsIndeterminate = false;
                filter.BackgroundWorker.ProgressInfo.OnlyTotalProgress = true;
                filter.BackgroundWorker.ProgressInfo.AddSubTask("DoSelect", 0, stepCount);
                filter.BackgroundWorker.ProgressInfo.TotalProgress.ProgressText = string.Format(@"Tracking: [{0}] | Start DoSelect() ...", filter.ItemSystemNo);
            }
            foreach (var dbStep in steps)
            {
                if (filter.CheckCancelWork())
                    return null;
                stepNo++;
                if (filter.BackgroundWorker != null)
                {
                    filter.BackgroundWorker.ProgressInfo.ReportProgress("DoSelect", stepNo);
                    filter.BackgroundWorker.ProgressInfo.TotalProgress.ProgressText = string.Format(@"Tracking: [{0}] | DoSelect() {1} / {2} steps...", filter.ItemSystemNo, stepNo, stepCount);
                }
                result.CurrentStep = new TandTStep();
                TandTStep step = new TandTStep() { StepNo = dbStep.StepNo };
                result.Steps.Add(step);
                foreach (var dbMixPoint in dbStep.TandTv3MixPoint_TandTv3Step)
                {
                    TandTv3Point mixPoint = DoSelectMixPoint(databaseApp, result, step, dbMixPoint);

                    #region charge prepare
                    foreach (var facilityBookingPreveiw in mixPoint.OutwardBookings)
                        if (facilityBookingPreveiw.FacilityChargeID != null)
                            result.RegisterFacilityChargeID(dbMixPoint.TandTv3Step.StepNo, facilityBookingPreveiw.FacilityChargeID ?? Guid.Empty);

                    foreach (var facilityBookingPreveiw in mixPoint.InwardBookings)
                        if (facilityBookingPreveiw.FacilityChargeID != null)
                            result.RegisterFacilityChargeID(dbMixPoint.TandTv3Step.StepNo, facilityBookingPreveiw.FacilityChargeID ?? Guid.Empty);
                    #endregion

                    #region fbc prepare
                    foreach (var facilityBookingPreveiw in mixPoint.OutwardBookings)
                        if (!result.Ids.Keys.Contains(facilityBookingPreveiw.FacilityBookingChargeID ?? Guid.Empty))
                            result.Ids.Add(facilityBookingPreveiw.FacilityBookingChargeID ?? Guid.Empty, MDTrackingStartItemTypeEnum.FacilityBookingCharge.ToString());

                    foreach (var facilityBookingPreveiw in mixPoint.InwardBookings)
                        if (!result.Ids.Keys.Contains(facilityBookingPreveiw.FacilityBookingChargeID ?? Guid.Empty))
                            result.Ids.Add(facilityBookingPreveiw.FacilityBookingChargeID ?? Guid.Empty, MDTrackingStartItemTypeEnum.FacilityBookingCharge.ToString());
                    #endregion

                    result.DeliveryNotePositions.AddRange(mixPoint.DeliveryNotePositions);
                    result.Lots.AddRange(mixPoint.OutwardLotsList.Select(c => c.LotNo));
                    if (mixPoint.IsProductionPoint && !result.ProgramNos.Contains(mixPoint.ProgramNo))
                        result.ProgramNos.Add(mixPoint.ProgramNo);
                    result.MixPoints.Add(mixPoint);
                    result.CurrentStep.MixingPoints.Add(mixPoint);

                }
            }
            if (filter.CheckCancelWork())
                return null;
            foreach (var dbStep in steps)
                foreach (var dbMixPoint in dbStep.TandTv3MixPoint_TandTv3Step)
                    foreach (var relation in dbMixPoint.TandTv3MixPointRelation_TargetTandTv3MixPoint)
                        if (!result.MixPointRelations.Any(x => x.SourceMixPoint.MixPointID == relation.SourceTandTv3MixPointID && x.TargetMixPoint.MixPointID == relation.TargetTandTv3MixPointID))
                        {
                            MixPointRelation mixPointRelation = new MixPointRelation();
                            mixPointRelation.SourceMixPoint = result.MixPoints.FirstOrDefault(c => c.MixPointID == relation.SourceTandTv3MixPointID);
                            mixPointRelation.TargetMixPoint = result.MixPoints.FirstOrDefault(c => c.MixPointID == relation.TargetTandTv3MixPointID);
                            if (mixPointRelation.SourceMixPoint != null && mixPointRelation.TargetMixPoint != null)
                                result.MixPointRelations.Add(mixPointRelation);
                        }
            result.Success = true;
            return result;
        }


        #endregion

        #region Program Log helper mehtods


        #endregion

        #region Filter Factory
        public TandTv3FilterTracking FactoryFilter(MDTrackingDirectionEnum trackingDirection, MDTrackingStartItemTypeEnum trackingStartItemType, Guid itemSystemID, string itemSystemNo,
           bool recalcAgain = false, bool isDynamic = false, bool isReport = false)
        {
            TandTv3FilterTracking filter = new TandTv3FilterTracking();
            filter.ItemSystemNo = itemSystemNo;
            filter.PrimaryKeyID = itemSystemID;
            filter.MDTrackingStartItemTypeEnum = trackingStartItemType;
            filter.MDTrackingDirectionEnum = trackingDirection;
            filter.RecalcAgain = recalcAgain;
            filter.IsDynamic = isDynamic;
            filter.IsReport = isReport;
            return filter;
        }
        #endregion

        #region others

        public void Calculations(DatabaseApp databaseApp, TandTResult result, MDTrackingDirectionEnum trackingDirection)
        {
            if (trackingDirection == MDTrackingDirectionEnum.Backward)
            {
                result.MixPoints =
                result
                .MixPoints
                .OrderByDescending(c => c.PartslistSequence)
                .ToList();
            }
            else
            {
                result.MixPoints =
                result
                .MixPoints
                .OrderBy(c => c.PartslistSequence)
                .ToList();
            }
            result.DeliveryNotes =
                result.DeliveryNotePositions
                .OrderBy(c => c.DeliveryNote.DeliveryNoteNo)
                .Select(c => FactoryDeliveryPreviewModel(c))
                .ToList();

            int nr = 0;
            if (result.DeliveryNotes != null)
                foreach (var item in result.DeliveryNotes)
                {
                    nr++;
                    item.Sn = nr;
                }

            #region group calc
            Guid[] fcIds = result.FacilityChargeIDs.Select(c => c.FacilityChargeID).ToArray();
            var queryFcs = databaseApp.FacilityCharge.Where(c => fcIds.Contains(c.FacilityChargeID));
            result.FacilityCharges = FacilityCharge.GetFacilityChargeModelList(databaseApp, queryFcs).ToList();
            result.FacilityCharges =
                result
                .FacilityCharges
                .OrderBy(c => c.LotNo)
                .ToList();

            #endregion
        }

        public virtual DeliveryNotePosPreview FactoryDeliveryPreviewModel(DeliveryNotePos dns)
        {
            DeliveryNotePosPreview dnsPreview = new DeliveryNotePosPreview();

            dnsPreview.DeliveryNotePosID = dns.DeliveryNotePosID;
            dnsPreview.DeliveryNoteNo = dns.DeliveryNote.DeliveryNoteNo;

            if (dns.DeliveryNote.DeliveryCompanyAddress != null)
            {
                dnsPreview.DeliveryAddress =
                    dns.DeliveryNote.DeliveryCompanyAddress.Company.CompanyName +
                    Environment.NewLine +
                    dns.DeliveryNote.DeliveryCompanyAddress.Street +
                    Environment.NewLine +
                    dns.DeliveryNote.DeliveryCompanyAddress.Postcode + " " +
                    dns.DeliveryNote.DeliveryCompanyAddress.City;
                if (dns.DeliveryNote.DeliveryCompanyAddress.MDCountry != null)
                    dnsPreview.DeliveryAddress += Environment.NewLine + dns.DeliveryNote.DeliveryCompanyAddress.MDCountry.MDCountryName;
            }

            if (dns.DeliveryNote.ShipperCompanyAddress != null)
            {
                dnsPreview.ShipperAddress =
                    dns.DeliveryNote.ShipperCompanyAddress.Company.CompanyName +
                    Environment.NewLine +
                    dns.DeliveryNote.ShipperCompanyAddress.Street +
                    Environment.NewLine +
                    dns.DeliveryNote.ShipperCompanyAddress.Postcode + " " +
                    dns.DeliveryNote.ShipperCompanyAddress.City;
                if (dns.DeliveryNote.ShipperCompanyAddress.MDCountry != null)
                    dnsPreview.ShipperAddress += Environment.NewLine + dns.DeliveryNote.ShipperCompanyAddress.MDCountry.MDCountryName;
            }

            dnsPreview.DeliveryDate = dns.DeliveryNote.DeliveryDate;

            if (dns.OutOrderPos != null)
            {
                dnsPreview.TargetQuantity = dns.OutOrderPos.TargetQuantity;
                dnsPreview.ActualQuantity = dns.OutOrderPos.ActualQuantity;
                if (dns.OutOrderPos.MDUnit == null)
                {
                    dnsPreview.MDUnitName = dns.OutOrderPos.Material.BaseMDUnit.MDUnitName;
                }
                else
                {
                    dnsPreview.MDUnitName = dns.OutOrderPos.MDUnit.MDUnitName;
                }
            }
            if (dns.InOrderPos != null)
            {
                dnsPreview.TargetQuantity = dns.InOrderPos.TargetQuantity;
                dnsPreview.ActualQuantity = dns.InOrderPos.ActualQuantity;
                if (dns.InOrderPos.MDUnit == null)
                {
                    dnsPreview.MDUnitName = dns.InOrderPos.Material.BaseMDUnit.MDUnitName;
                }
                else
                {
                    dnsPreview.MDUnitName = dns.InOrderPos.MDUnit.MDUnitName;
                }

                dnsPreview.LotList = dns
                   .InOrderPos
                   .FacilityBookingCharge_InOrderPos
                   .Where(c => c.InwardFacilityChargeID != null && c.InwardFacilityCharge.FacilityLotID != null)
                   .Select(c => c.InwardFacilityCharge.FacilityLot.LotNo)
                   .Distinct()
                   .OrderBy(c => c)
                   .ToList();


                foreach (var item in dns.InOrderPos.FacilityBookingCharge_InOrderPos)
                {
                    if (item.InwardFacilityID != null && (dnsPreview.FacilityNo == null || !dnsPreview.FacilityNo.Contains(item.InwardFacility.FacilityNo)))
                    {
                        dnsPreview.FacilityNo += item.InwardFacility.FacilityNo + ", ";
                    }
                }

                if (!string.IsNullOrEmpty(dnsPreview.FacilityNo))
                    dnsPreview.FacilityNo = dnsPreview.FacilityNo.TrimEnd(", ".ToCharArray());

            }

            dnsPreview.MaterialNo = dns.Material.MaterialNo;
            dnsPreview.MaterialName = dns.Material.MaterialName1;

            dnsPreview.DosedQuantity = 0;

            return dnsPreview;
        }

        public bool FetchRelatedProgramLogs(DatabaseApp databaseApp, TandTResult result)
        {
            result.OrderLogRelView = databaseApp.OrderLogRelView.Where(c => result.ProgramNos.Contains(c.ProdOrderProgramNo)).ToList();
            result.OrderLogPosMachines = databaseApp.OrderLogPosMachines.Where(c => result.ProgramNos.Contains(c.ProdOrderProgramNo)).ToList();
            List<Guid> acClassIDs = new List<Guid>();
            if (result.OrderLogRelView != null && result.OrderLogRelView.Any())
            {
                acClassIDs.AddRange(result.OrderLogRelView.Select(c => c.ACClassID));
                acClassIDs.AddRange(result.OrderLogRelView.Select(c => c.BasedOnACClassID ?? Guid.Empty));
            }
            if (result.OrderLogPosMachines != null && result.OrderLogPosMachines.Any())
            {
                acClassIDs.AddRange(result.OrderLogPosMachines.Select(c => c.ACClassID));
                acClassIDs.AddRange(result.OrderLogPosMachines.Select(c => c.BasedOnACClassID ?? Guid.Empty));
            }
            acClassIDs = acClassIDs.Distinct().ToList();

            using (ACMonitor.Lock(databaseApp.ContextIPlus.QueryLock_1X000))
            {
                result.ACClasses = databaseApp.ContextIPlus.ACClass.Where(c => acClassIDs.Contains(c.ACClassID)).ToList();
            }

            return
                (result.OrderLogRelView != null && result.OrderLogRelView.Any()) ||
                (result.OrderLogPosMachines != null && result.OrderLogPosMachines.Any());
        }

        public void FetchRelatedMachines(DatabaseApp databaseApp, TandTResult result)
        {
            foreach (var mixPoint in result.MixPoints)
            {
                if (mixPoint.Relations.Any() && mixPoint.ProductionPositions.Any())
                {
                    Guid[] posIDs = mixPoint.ProductionPositions.Select(c => c.ProdOrderPartslistPosID).Distinct().ToArray();
                    Guid[] relIDs = mixPoint.Relations.Select(c => c.ProdOrderPartslistPosRelationID).Distinct().ToArray();

                    Guid[] inwardMachines =
                        result
                        .OrderLogPosMachines
                        .Where(c => posIDs.Contains(c.ProdOrderPartslistPosID ?? Guid.Empty) && c.ActualQuantityUOM > 0)
                        .Select(c => c.ACClassID)
                        .Distinct()
                        .ToArray();

                    Guid[] outwardMachines =
                        result
                        .OrderLogRelView
                        .Where(c => relIDs.Contains(c.ProdOrderPartslistPosRelationID ?? Guid.Empty) && c.ActualQuantityUOM > 0)
                        .Select(c => c.ACClassID)
                        .Distinct()
                        .ToArray();

                    inwardMachines = inwardMachines.Where(c => !outwardMachines.Contains(c)).ToArray();

                    mixPoint.InwardMachines = result.ACClasses.Where(c => inwardMachines.Contains(c.ACClassID)).ToList();
                    mixPoint.OutwardMachines = result.ACClasses.Where(c => outwardMachines.Contains(c.ACClassID)).ToList();
                }
            }
        }


        private void FetchRelatedFacilityACClasses(DatabaseApp databaseApp, TandTResult result)
        {
            List<Guid> acClassIDs = new List<Guid>();
            foreach (var mixPoint in result.MixPoints)
            {
                foreach (var outwardFacility in mixPoint.OutwardFacilities)
                    acClassIDs.Add(outwardFacility.Value.VBiFacilityACClassID ?? Guid.Empty);

                foreach (var inwardFacility in mixPoint.InwardFacilities)
                    acClassIDs.Add(inwardFacility.Value.VBiFacilityACClassID ?? Guid.Empty);
            }
            List<gip.core.datamodel.ACClass> facilityACClasses = new List<core.datamodel.ACClass>();
            using (ACMonitor.Lock(databaseApp.ContextIPlus.QueryLock_1X000))
            {
                facilityACClasses = databaseApp.ContextIPlus.ACClass.Where(c => acClassIDs.Contains(c.ACClassID)).ToList();
            }
            foreach (var mixPoint in result.MixPoints)
            {
                foreach (var outwardFacility in mixPoint.OutwardFacilities)
                    if (outwardFacility.Value.VBiFacilityACClassID != null)
                        outwardFacility.Value.FacilityACClass = facilityACClasses.FirstOrDefault(c => c.ACClassID == (outwardFacility.Value.VBiFacilityACClassID ?? Guid.Empty));

                foreach (var inwardFacility in mixPoint.InwardFacilities)
                    if (inwardFacility.Value.VBiFacilityACClassID != null)
                        inwardFacility.Value.FacilityACClass = facilityACClasses.FirstOrDefault(c => c.ACClassID == (inwardFacility.Value.VBiFacilityACClassID ?? Guid.Empty));

            }
        }

        /// <summary>
        /// Grouping production points
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public TandTResult BuildGroupResult(DatabaseApp databaseApp, TandTResult result)
        {
            TandTResult groupResult = FactoryTandTResult(result.Filter);

            groupResult.CurrentStep = new TandTStep() { };

            groupResult.Success = result.Success;
            groupResult.ErrorMsg = result.ErrorMsg;
            groupResult.StartTime = result.StartTime;
            groupResult.EndTime = result.EndTime;

            groupResult.Ids = result.Ids;
            groupResult.BatchIDs = result.BatchIDs;
            groupResult.Lots = result.Lots;
            groupResult.Steps = result.Steps;


            groupResult.DeliveryNotePositions = result.DeliveryNotePositions;
            groupResult.FacilityChargeIDs = result.FacilityChargeIDs;
            groupResult.DeliveryNotes = result.DeliveryNotes;
            groupResult.FacilityCharges = result.FacilityCharges;
            groupResult.ProgramNos = result.ProgramNos;

            //groupResult.MixPointRelations = result.MixPointRelations;
            groupResult.OrderLogRelView = result.OrderLogRelView;
            groupResult.OrderLogPosMachines = result.OrderLogPosMachines;
            groupResult.ACClasses = result.ACClasses;

            // copy not productive items
            groupResult.MixPoints.AddRange(result.MixPoints.Where(c => c.IsInputPoint));

            // build productive group
            var query = result
                    .MixPoints
                    .Where(c => c.IsProductionPoint)
                    .GroupBy(c => new
                    {
                        c.PartslistSequence,
                        c.InwardMaterialNo,
                        c.InwardMaterialName,
                        c.ProgramNo
                    });

            var test = query.Select(c => c.Key).Count();

            // Merge connections with facilities, machines and lots
            foreach (var item in query)
            {
                KeyValuePair<MixPointGroup, List<TandTv3Point>> dItem = new KeyValuePair<MixPointGroup, List<TandTv3Point>>(
                    new MixPointGroup()
                    {
                        PartslistSequence = item.Key.PartslistSequence,
                        InwardMaterialNo = item.Key.InwardMaterialNo,
                        InwardMaterialName = item.Key.InwardMaterialName,
                        ProgramNo = item.Key.ProgramNo
                    },
                    item.ToList());
                TandTv3PointPosGrouped groupMixPoint = BuildGroupedMixPoint(databaseApp, dItem);
                groupResult.MixPoints.Add(groupMixPoint);
                groupResult.CurrentStep.MixingPoints.Add(groupMixPoint);
            }

            FactoryRelations(result, groupResult);
            groupResult.MixPoints = groupResult.MixPoints.OrderBy(c => c.StepNo).ThenBy(c => c.PartslistSequence).ToList();
            return groupResult;
        }

        public TandTv3PointPosGrouped BuildGroupedMixPoint(DatabaseApp databaseApp, KeyValuePair<MixPointGroup, List<TandTv3Point>> item)
        {
            TandTv3PointPosGrouped groupMixPoint = new TandTv3PointPosGrouped();
            groupMixPoint.ProgramNo = item.Key.ProgramNo;
            groupMixPoint.ProdOrder = databaseApp.ProdOrder.FirstOrDefault(c => c.ProgramNo == item.Key.ProgramNo);
            groupMixPoint.PartslistSequence = item.Key.PartslistSequence;
            groupMixPoint.InwardMaterialNo = item.Key.InwardMaterialNo;
            groupMixPoint.InwardMaterialName = item.Key.InwardMaterialName;
            groupMixPoint.InwardMaterial = item.Value.FirstOrDefault().InwardMaterial;
            groupMixPoint.IsProductionPoint = true;
            groupMixPoint.Step = item.Value.Select(c => c.Step).FirstOrDefault();

            foreach (var subItem in item.Value.AsEnumerable())
            {

                // DeliveryNotePositions
                foreach (var dns in subItem.DeliveryNotePositions)
                    if (!groupMixPoint.DeliveryNotePositions.Select(c => c.DeliveryNotePosID).Contains(dns.DeliveryNotePosID))
                        groupMixPoint.DeliveryNotePositions.Add(dns);

                // ExistLabOrder
                groupMixPoint.ExistLabOrder = subItem.ExistLabOrder || groupMixPoint.ExistLabOrder;

                // FacilityChargeIDs
                //foreach (var fcId in subItem.FacilityChargeIDs)
                //    if (!groupMixPoint.FacilityChargeIDs.Contains(fcId))
                //        groupMixPoint.FacilityChargeIDs.Add(fcId);

                // InOrderPositions
                foreach (var inOrderPos in subItem.InOrderPositions)
                    if (!groupMixPoint.InOrderPositions.Select(c => c.InOrderPosID).Contains(inOrderPos.InOrderPosID))
                        groupMixPoint.InOrderPositions.Add(inOrderPos);

                // OutOrderPositions
                foreach (var outOrderPos in subItem.OutOrderPositions)
                    if (!groupMixPoint.OutOrderPositions.Select(c => c.OutOrderPosID).Contains(outOrderPos.OutOrderPosID))
                        groupMixPoint.OutOrderPositions.Add(outOrderPos);

                // OutOrderPositions
                foreach (var pickingPos in subItem.PickingPositions)
                    if (!groupMixPoint.PickingPositions.Select(c => c.PickingPosID).Contains(pickingPos.PickingPosID))
                        groupMixPoint.PickingPositions.Add(pickingPos);

                // InwardBookings
                foreach (var inwardBookingPreview in subItem.InwardBookings)
                    if (!groupMixPoint.InwardBookings.Select(c => c.FacilityBookingChargeNo).Contains(inwardBookingPreview.FacilityBookingChargeNo))
                        groupMixPoint.InwardBookings.Add(inwardBookingPreview);

                // InwardPreBookings
                foreach (var fbcPre in subItem.InwardPreBookings)
                    if (!groupMixPoint.InwardPreBookings.Select(c => c.FacilityPreBookingNo).Contains(fbcPre.FacilityPreBookingNo))
                        groupMixPoint.InwardPreBookings.Add(fbcPre);

                // InwardFacilities
                foreach (var inwardFacility in subItem.InwardFacilities)
                    groupMixPoint.AddInwardFacility(inwardFacility.Value, inwardFacility.Value.StockQuantityUOM);

                // ItemsWithLabOrder
                foreach (var labOrderItem in subItem.ItemsWithLabOrder)
                    if (!groupMixPoint.ItemsWithLabOrder.Select(c => c.ID).Contains(labOrderItem.ID))
                        groupMixPoint.ItemsWithLabOrder.Add(labOrderItem);

                // OutwardBookings
                foreach (var outwardBookingPreview in subItem.OutwardBookings)
                    if (!groupMixPoint.OutwardBookings.Select(c => c.FacilityBookingChargeNo).Contains(outwardBookingPreview.FacilityBookingChargeNo))
                        groupMixPoint.OutwardBookings.Add(outwardBookingPreview);

                // OutwardPreBookings
                foreach (var fbcPre in subItem.OutwardPreBookings)
                    if (!groupMixPoint.OutwardPreBookings.Select(c => c.FacilityPreBookingNo).Contains(fbcPre.FacilityPreBookingNo))
                        groupMixPoint.OutwardPreBookings.Add(fbcPre);

                // OutwardFacilitites
                foreach (var outwardFacility in subItem.OutwardFacilities)
                    if (!groupMixPoint.OutwardFacilities.Select(c => c.Key).Contains(outwardFacility.Key))
                        groupMixPoint.AddOutwardFacility(outwardFacility.Value, outwardFacility.Value.StockQuantityUOM);

                // PartslistSequence
                groupMixPoint.PartslistSequence = subItem.PartslistSequence;

                //Relations
                foreach (var relation in subItem.Relations)
                    if (!groupMixPoint.Relations.Select(c => c.ProdOrderPartslistPosRelationID).Contains(relation.ProdOrderPartslistPosRelationID))
                        groupMixPoint.Relations.Add(relation);

                //BatchNoList
                foreach (var batchNo in subItem.BatchNoList)
                    if (!groupMixPoint.BatchNoList.Contains(batchNo))
                        groupMixPoint.BatchNoList.Add(batchNo);

                //Machines
                foreach (var machine in subItem.InwardMachines)
                    if (!groupMixPoint.InwardMachines.Select(c => c.ACClassID).Contains(machine.ACClassID))
                        groupMixPoint.InwardMachines.Add(machine);

                foreach (var machine in subItem.OutwardMachines)
                    if (!groupMixPoint.OutwardMachines.Select(c => c.ACClassID).Contains(machine.ACClassID))
                        groupMixPoint.OutwardMachines.Add(machine);


                // ChildMixPointIds
                if (!groupMixPoint.ChildMixPointIds.Contains(subItem.MixPointID))
                    groupMixPoint.ChildMixPointIds.Add(subItem.MixPointID);

                // Positions
                foreach (var pos in subItem.ProductionPositions)
                    if (!groupMixPoint.ProductionPositions.Select(c => c.ProdOrderPartslistPosID).Contains(pos.ProdOrderPartslistPosID))
                        groupMixPoint.ProductionPositions.Add(pos);


                // Materials

                // OutwardMaterials
                foreach (var outwardMaterial in subItem.OutwardMaterials)
                    if (!groupMixPoint.OutwardMaterials.Select(c => c.MaterialNo).Contains(outwardMaterial.MaterialNo))
                        groupMixPoint.OutwardMaterials.Add(outwardMaterial);

                if (!groupMixPoint.InwardMaterials.Select(c => c.MaterialNo).Contains(subItem.InwardMaterialNo))
                    groupMixPoint.InwardMaterials.Add(subItem.InwardMaterial);

                // batches
                if (!string.IsNullOrEmpty(subItem.InwardBatchNo))
                    if (!groupMixPoint.InwardBatchList.Contains(subItem.InwardBatchNo))
                        groupMixPoint.InwardBatchList.Add(subItem.InwardBatchNo);
            }


            // OutwardLots (SUM)
            groupMixPoint.OutwardLotsList =
                item
                .Value
                .AsEnumerable()
                .SelectMany(c => c.OutwardLotsList)
                .GroupBy(c => new { c.LotNo, c.ExternLotNo, c.ExternLotNo2, c.InsertDate, c.MaterialNo, c.MaterialName1, c.Comment })
                .Select(c => new FacilityLotModel()
                {
                    LotNo = c.Key.LotNo,
                    ExternLotNo = c.Key.ExternLotNo,
                    ExternLotNo2 = c.Key.ExternLotNo2,
                    Comment = c.Key.Comment,
                    MaterialNo = c.Key.MaterialNo,
                    MaterialName1 = c.Key.MaterialName1,
                    InsertDate = c.Key.InsertDate,
                    ActualQuantity = c.Sum(x => x.ActualQuantity),
                    FacilityLotID = Guid.NewGuid()
                }
                )
                .OrderBy(c => c.InsertDate)
                .ToList();

            // InwardLots (SUM)
            groupMixPoint.InwardLotsList =
                item
                .Value
                .AsEnumerable()
                .Select(c => c.InwardLot)
                .GroupBy(c => new { c.LotNo, c.ExternLotNo, c.ExternLotNo2, c.InsertDate, c.MaterialNo, c.MaterialName1, c.Comment })
                .Select(c => new FacilityLotModel()
                {
                    LotNo = c.Key.LotNo,
                    ExternLotNo = c.Key.ExternLotNo,
                    ExternLotNo2 = c.Key.ExternLotNo2,
                    Comment = c.Key.Comment,
                    MaterialNo = c.Key.MaterialNo,
                    MaterialName1 = c.Key.MaterialName1,
                    InsertDate = c.Key.InsertDate,
                    ActualQuantity = c.Sum(x => x.ActualQuantity),
                    FacilityLotID = Guid.NewGuid()
                }
                )
                .OrderBy(c => c.InsertDate)
                .ToList();
            return groupMixPoint;
        }

        private void FactoryRelations(TandTResult result, TandTResult groupResult)
        {
            foreach (var relation in result.MixPointRelations)
            {

                TandTv3Point gSourceMixPoint = relation.SourceMixPoint;
                if (relation.SourceMixPoint.IsProductionPoint)
                    gSourceMixPoint =
                        groupResult
                        .MixPoints
                        .Where(c => c is TandTv3PointPosGrouped)
                        .Where(c => (c as TandTv3PointPosGrouped).ChildMixPointIds.Contains(relation.SourceMixPoint.MixPointID))
                        .FirstOrDefault();

                TandTv3Point gTargetMixPoint = relation.TargetMixPoint;
                if (relation.TargetMixPoint.IsProductionPoint)
                    gTargetMixPoint =
                        groupResult
                        .MixPoints
                        .Where(c => c is TandTv3PointPosGrouped)
                        .Where(c => (c as TandTv3PointPosGrouped).ChildMixPointIds.Contains(relation.TargetMixPoint.MixPointID))
                        .FirstOrDefault();


                if (gSourceMixPoint != null && gTargetMixPoint != null && gSourceMixPoint.MixPointID != gTargetMixPoint.MixPointID)
                {
                    MixPointRelation groupRelation = new MixPointRelation()
                    {
                        SourceMixPoint = gSourceMixPoint,
                        TargetMixPoint = gTargetMixPoint
                    };

                    if (!groupResult.MixPointRelations.Any(c =>
                        c.SourceMixPoint.MixPointID == groupRelation.SourceMixPoint.MixPointID &&
                        c.TargetMixPoint.MixPointID == groupRelation.TargetMixPoint.MixPointID))
                        groupResult.MixPointRelations.Add(groupRelation);
                }
            }
        }

        public string GetLogFileName(string itemNo)
        {
            string folder = Path.GetTempPath();
            if (!string.IsNullOrEmpty(RootLogFolder))
                folder = RootLogFolder;
            return Path.Combine(folder, string.Format(LogFileNameTemplate, itemNo, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));
        }
        #endregion

        #region Private methods

        #region Private methods -> Storage

        private TandTv3FilterTracking GetFilter(DatabaseApp databaseApp, TandTv3FilterTracking filter, string vBUserNo)
        {
            IEnumerable<TandTv3FilterTracking> queryJob = databaseApp.TandTv3FilterTracking.Where(c =>
                c.TandTv3MDTrackingDirectionID == filter.TandTv3MDTrackingDirectionID &&
                c.TandTv3MDTrackingStartItemTypeID == filter.TandTv3MDTrackingStartItemTypeID &&
                ((c.FilterDateFrom ?? (new DateTime())) == (filter.FilterDateFrom ?? (new DateTime()))) &&
                ((c.FilterDateTo ?? (new DateTime())) == (filter.FilterDateTo ?? (new DateTime()))) &&
                c.PrimaryKeyID == filter.PrimaryKeyID);
            TandTv3FilterTracking dbFilter = null;
            if (filter.MaterialIDs != null && filter.MaterialIDs.Any())
            {
                foreach (TandTv3FilterTracking tmpJob in queryJob)
                {
                    List<Guid> tmpMaterialIds = tmpJob.TandTv3FilterTrackingMaterial_TandTv3FilterTracking.Select(c => c.MaterialID).ToList();
                    if (tmpMaterialIds.SequenceEqual(filter.MaterialIDs))
                    {
                        dbFilter = tmpJob;
                        break;
                    }
                }
            }
            else
            {
                dbFilter = queryJob.FirstOrDefault();
            }


            if (dbFilter == null)
            {
                dbFilter = new TandTv3FilterTracking()
                {
                    TandTv3FilterTrackingID = Guid.NewGuid(),
                    TandTv3MDTrackingDirectionID = filter.TandTv3MDTrackingDirectionID,
                    TandTv3MDTrackingStartItemTypeID = filter.TandTv3MDTrackingStartItemTypeID,
                    FilterTrackingNo = "#",
                    FilterDateFrom = filter.FilterDateFrom,
                    FilterDateTo = filter.FilterDateTo,
                    ItemSystemNo = filter.ItemSystemNo,
                    PrimaryKeyID = filter.PrimaryKeyID,
                    StartTime = DateTime.Now,
                    InsertName = vBUserNo,
                    IsNew = true,
                    IsDynamic = filter.IsDynamic,
                    IsReport = filter.IsReport,
                    MaterialIDs = new List<Guid>(),
                    RecalcAgain = filter.RecalcAgain,
                    OrderDepth = filter.OrderDepth
                };

                if (filter.MaterialIDs != null && filter.MaterialIDs.Any())
                    foreach (Guid materialID in filter.MaterialIDs)
                    {
                        TandTv3FilterTrackingMaterial dbFilterTrackingMaterial = new TandTv3FilterTrackingMaterial();
                        dbFilterTrackingMaterial.TandTv3FilterTrackingMaterialID = Guid.NewGuid();
                        Material tmpMaterial = databaseApp.Material.FirstOrDefault(c => c.MaterialID == materialID);
                        dbFilterTrackingMaterial.Material = tmpMaterial;
                        dbFilterTrackingMaterial.TandTv3FilterTracking = dbFilter;
                        dbFilter.TandTv3FilterTrackingMaterial_TandTv3FilterTracking.Add(dbFilterTrackingMaterial);
                        dbFilter.MaterialIDs.Add(materialID);
                    }
            }
            else
                dbFilter.IsNew = false;
            dbFilter.BreakTrackingCondition = filter.BreakTrackingCondition;
            dbFilter.AggregateOrderData = filter.AggregateOrderData;
            dbFilter.BackgroundWorker = filter.BackgroundWorker;
            dbFilter.DoWorkEventArgs = filter.DoWorkEventArgs;
            return dbFilter;
        }

        public MsgWithDetails DoSave(DatabaseApp databaseApp, TandTResult result)
        {
            TandTv3MDBookingDirection inwardDirection = databaseApp.TandTv3MDBookingDirection.ToList().FirstOrDefault(c => c.TandTv3MDBookingDirectionID == MDBookingDirectionEnum.Inward.ToString());
            TandTv3MDBookingDirection outwardDirection = databaseApp.TandTv3MDBookingDirection.ToList().FirstOrDefault(c => c.TandTv3MDBookingDirectionID == MDBookingDirectionEnum.Outward.ToString());

            databaseApp.TandTv3FilterTracking.AddObject(result.Filter);

            Dictionary<int, TandTv3Step> stepMapping = new Dictionary<int, TandTv3Step>();
            foreach (var step in result.Steps)
            {
                TandTv3Step dbStep = new TandTv3Step();
                dbStep.TandTv3StepID = Guid.NewGuid();
                dbStep.StepNo = step.StepNo;
                dbStep.StepName = @"#";
                result.Filter.TandTv3Step_TandTv3FilterTracking.Add(dbStep);
                stepMapping.Add(step.StepNo, dbStep);
            }


            foreach (var mixPoint in result.MixPoints)
            {
                TandTv3MixPoint dbMixPoint = new TandTv3MixPoint();
                dbMixPoint.TandTv3MixPointID = mixPoint.MixPointID;
                // *** Fields ***
                // StepID
                stepMapping[mixPoint.Step.StepNo].TandTv3MixPoint_TandTv3Step.Add(dbMixPoint);

                // ProdOrderPartslistPosID
                // ProdOrderBatchID
                if (mixPoint.IsProductionPoint)
                {
                    dbMixPoint.IsProductionPoint = true;
                    foreach (var item in mixPoint.ProductionPositions)
                    {
                        TandTv3MixPointProdOrderPartslistPos dbMixPointProdPosition = new TandTv3MixPointProdOrderPartslistPos()
                        {
                            TandTv3MixPointProdOrderPartslistPosID = Guid.NewGuid(),
                            ProdOrderPartslistPos = item
                        };
                        dbMixPoint.TandTv3MixPointProdOrderPartslistPos_TandTv3MixPoint.Add(dbMixPointProdPosition);
                    }
                }

                // InOrderPosID
                if (mixPoint.IsInputPoint)
                {
                    dbMixPoint.IsInputPoint = true;
                    foreach (var item in mixPoint.InOrderPositions)
                    {
                        TandTv3MixPointInOrderPos dbMixPointInOrderPos = new TandTv3MixPointInOrderPos()
                        {
                            TandTv3MixPointInOrderPosID = Guid.NewGuid(),
                            InOrderPos = item
                        };
                        dbMixPoint.TandTv3MixPointInOrderPos_TandTv3MixPoint.Add(dbMixPointInOrderPos);
                    }

                    foreach (PickingPos pickingPos in mixPoint.PickingPositions)
                    {
                        TandTv3MixPointPickingPos dbMixPointInOrderPos = new TandTv3MixPointPickingPos()
                        {
                            TandTv3MixPointPickingPosID = Guid.NewGuid(),
                            PickingPos = pickingPos
                        };
                        dbMixPoint.TandTv3MixPointPickingPos_TandTv3MixPoint.Add(dbMixPointInOrderPos);
                    }

                }

                //InwardLotID
                if (!string.IsNullOrEmpty(mixPoint.InwardLot.LotNo))
                {
                    dbMixPoint.InwardLot = databaseApp.FacilityLot.FirstOrDefault(c => c.LotNo == mixPoint.InwardLot.LotNo);
                }


                //InwardMaterialID
                dbMixPoint.InwardMaterial = databaseApp.Material.FirstOrDefault(c => c.MaterialNo == mixPoint.InwardMaterialNo);


                // *** Tables ***

                // TandTv3_MixPointProdOrderPartslistPosRelation
                foreach (var relation in mixPoint.Relations)
                {
                    TandTv3MixPointProdOrderPartslistPosRelation dbMixPointRelation = new TandTv3MixPointProdOrderPartslistPosRelation()
                    {
                        TandTv3MixPointProdOrderPartslistPosRelationID = Guid.NewGuid(),
                        ProdOrderPartslistPosRelation = relation
                    };
                    dbMixPoint.TandTv3MixPointProdOrderPartslistPosRelation_TandTv3MixPoint.Add(dbMixPointRelation);
                }

                //// TandTv3_MixPointFacilityBookingCharge
                foreach (var inwardFbc in mixPoint.InwardBookings)
                {
                    TandTv3MixPointFacilityBookingCharge dbMixPointFbc = new TandTv3MixPointFacilityBookingCharge()
                    {
                        TandTv3MixPointFacilityBookingChargeID = Guid.NewGuid(),
                        FacilityBookingCharge = inwardFbc.FacilityBookingCharge
                    };
                    dbMixPoint.TandTv3MixPointFacilityBookingCharge_TandTv3MixPoint.Add(dbMixPointFbc);
                }

                foreach (var outwardFbc in mixPoint.OutwardBookings)
                {
                    TandTv3MixPointFacilityBookingCharge dbMixPointFbc = new TandTv3MixPointFacilityBookingCharge()
                    {
                        TandTv3MixPointFacilityBookingChargeID = Guid.NewGuid(),
                        FacilityBookingCharge = outwardFbc.FacilityBookingCharge
                    };
                    dbMixPoint.TandTv3MixPointFacilityBookingCharge_TandTv3MixPoint.Add(dbMixPointFbc);
                }

                // TandTv3_MixPointFacilityPreBooking
                foreach (var tmp in mixPoint.InwardPreBookings)
                {
                    TandTv3MixPointFacilityPreBooking dbMixPointPreFb = new TandTv3MixPointFacilityPreBooking()
                    {
                        TandTv3MixPointFacilityPreBookingID = Guid.NewGuid(),
                        FacilityPreBooking = tmp.FacilityPreBooking
                    };
                    dbMixPoint.TandTv3MixPointFacilityPreBooking_TandTv3MixPoint.Add(dbMixPointPreFb);
                }

                foreach (var tmp in mixPoint.OutwardPreBookings)
                {
                    TandTv3MixPointFacilityPreBooking dbMixPointPreFb = new TandTv3MixPointFacilityPreBooking()
                    {
                        TandTv3MixPointFacilityPreBookingID = Guid.NewGuid(),
                        FacilityPreBooking = tmp.FacilityPreBooking
                    };
                    dbMixPoint.TandTv3MixPointFacilityPreBooking_TandTv3MixPoint.Add(dbMixPointPreFb);
                }

                //// TandTv3_MixPointLot
                string[] outwardLotNos = mixPoint.OutwardLotsList.Select(c => c.LotNo).ToArray();
                List<FacilityLot> outwardLots = databaseApp.FacilityLot.Where(c => outwardLotNos.Contains(c.LotNo)).ToList();
                IEnumerable<TandTv3MixPointFacilityLot> dbMixPointLots =
                    outwardLots.Select(c => new TandTv3MixPointFacilityLot()
                    {
                        TandTv3MixPointFacilityLotID = Guid.NewGuid(),
                        FacilityLot = c,
                        TandTv3MDBookingDirection = outwardDirection
                    });
                foreach (var dbMixPointLot in dbMixPointLots)
                    dbMixPoint.TandTv3MixPointFacilityLot_TandTv3MixPoint.Add(dbMixPointLot);


                //// TandTv3_MixPointDeliveryNotePos
                foreach (var dns in mixPoint.DeliveryNotePositions)
                {
                    if (!dbMixPoint.TandTv3MixPointDeliveryNotePos_TandTv3MixPoint.Any(c => c.DeliveryNotePosID == dns.DeliveryNotePosID))
                    {
                        TandTv3MixPointDeliveryNotePos dbMixPointDeliveryNotePos = new TandTv3MixPointDeliveryNotePos()
                        {
                            TandTv3MixPointDeliveryNotePosID = Guid.NewGuid(),
                            DeliveryNotePos = dns
                        };
                        dbMixPoint.TandTv3MixPointDeliveryNotePos_TandTv3MixPoint.Add(dbMixPointDeliveryNotePos);
                    }
                }
            }

            // TandTv3_MixPointRelation
            foreach (var mixPoint in result.MixPoints)
            {
                IEnumerable<Guid> targets = result.MixPointRelations.Where(c => c.SourceMixPoint.MixPointID == mixPoint.MixPointID).Select(c => c.TargetMixPoint.MixPointID);
                TandTv3MixPoint dbSourceMixPoint = stepMapping.Select(c => c.Value).SelectMany(c => c.TandTv3MixPoint_TandTv3Step).FirstOrDefault(c => c.TandTv3MixPointID == mixPoint.MixPointID);
                foreach (Guid targetID in targets)
                {
                    if (targetID != mixPoint.MixPointID)
                    {
                        TandTv3MixPoint dbTargetMixPoint = stepMapping.Select(c => c.Value).SelectMany(c => c.TandTv3MixPoint_TandTv3Step).FirstOrDefault(c => c.TandTv3MixPointID == targetID);
                        TandTv3MixPointRelation dbMixPointRelation = new TandTv3MixPointRelation()
                        {
                            TandTv3MixPointRelationID = Guid.NewGuid(),
                            SourceTandTv3MixPoint = dbSourceMixPoint,
                            TargetTandTv3MixPoint = dbTargetMixPoint
                        };
                        databaseApp.TandTv3MixPointRelation.AddObject(dbMixPointRelation);
                    }
                }
            }

            return databaseApp.ACSaveChanges();
        }

        #endregion

        #region Private methods -> prepare model

        private TandTv3Point DoSelectMixPoint(DatabaseApp databaseApp, TandTResult result, TandTStep step, TandTv3MixPoint dbMixPoint)
        {
            // MixPointID
            TandTv3Point mixPoint = null;
            if (dbMixPoint.IsInputPoint)
                mixPoint = new TandTv3PointDN() { MixPointID = dbMixPoint.TandTv3MixPointID, Step = step, IsInputPoint = true };
            else
                mixPoint = new TandTv3Point() { MixPointID = dbMixPoint.TandTv3MixPointID, Step = step };
            // *** Fields ***

            // InwardLotID
            if (dbMixPoint.InwardLotID != null)
            {
                mixPoint.AddInwardLot(dbMixPoint.InwardLot);
                if (!result.Ids.ContainsKey(dbMixPoint.InwardLot.FacilityLotID))
                    result.Ids.Add(dbMixPoint.InwardLot.FacilityLotID, MDTrackingStartItemTypeEnum.FacilityLot.ToString());
            }

            // InwardMaterialID
            mixPoint.InwardMaterialNo = dbMixPoint.InwardMaterial.MaterialNo;
            mixPoint.InwardMaterialName = dbMixPoint.InwardMaterial.MaterialName1;
            mixPoint.InwardMaterial = dbMixPoint.InwardMaterial;
            if (!result.Ids.ContainsKey(dbMixPoint.InwardMaterial.MaterialID))
                result.Ids.Add(dbMixPoint.InwardMaterial.MaterialID, MDTrackingStartItemTypeEnum.Material.ToString());


            // InwardBookings
            // OutwardBookings
            // => TandTv3_MixPointFacilityBookingCharge
            foreach (var fbc in
                    dbMixPoint.TandTv3MixPointFacilityBookingCharge_TandTv3MixPoint)
            {
                FacilityBookingCharge inwardFacilityBookingCharge = null;
                FacilityBookingCharge outwardFacilityBookingCharge = null;
                if (fbc.FacilityBookingCharge.InwardFacilityID != null)
                    inwardFacilityBookingCharge = fbc.FacilityBookingCharge;
                if (fbc.FacilityBookingCharge.OutwardFacilityID != null)
                    outwardFacilityBookingCharge = fbc.FacilityBookingCharge;

                if (inwardFacilityBookingCharge != null)
                    if (mixPoint.AddInwardBooking(inwardFacilityBookingCharge))
                        mixPoint.AddInwardLotQuantity(inwardFacilityBookingCharge);

                if (outwardFacilityBookingCharge != null)
                    if (mixPoint.AddOutwardBooking(outwardFacilityBookingCharge))
                        mixPoint.AddOutwardLotQuantity(outwardFacilityBookingCharge);

                if (!result.Ids.ContainsKey(fbc.FacilityBookingChargeID))
                    result.Ids.Add(fbc.FacilityBookingChargeID, MDTrackingStartItemTypeEnum.FacilityBookingCharge.ToString());

                if (fbc.FacilityBookingCharge.PickingPos != null)
                    if (!result.Ids.ContainsKey(fbc.FacilityBookingChargeID))
                        result.Ids.Add(fbc.FacilityBookingCharge.PickingPosID.Value, MDTrackingStartItemTypeEnum.PickingPos.ToString());
            }

            // InwardPreBookings
            // OutwardPreBookings
            // => TandTv3_MixPointFacilityPreBooking
            foreach (var fbPre in
                    dbMixPoint.TandTv3MixPointFacilityPreBooking_TandTv3MixPoint)
            {
                FacilityPreBooking inwardFacilityPreBooking = null;
                FacilityPreBooking outwardFacilityPreBooking = null;
                if (fbPre.FacilityPreBooking.InwardFacility != null)
                    inwardFacilityPreBooking = fbPre.FacilityPreBooking;
                if (fbPre.FacilityPreBooking.OutwardFacility != null)
                    outwardFacilityPreBooking = fbPre.FacilityPreBooking;

                if (inwardFacilityPreBooking != null)
                {
                    if (!mixPoint.InwardPreBookings.Select(c => c.FacilityPreBookingNo).Contains(inwardFacilityPreBooking.FacilityPreBookingNo))
                        mixPoint.InwardPreBookings.Add(new FacilityPreBookingPreveiw()
                        {
                            FacilityPreBookingNo = inwardFacilityPreBooking.FacilityPreBookingNo,
                            InsertDate = inwardFacilityPreBooking.InsertDate,
                            FacilityNo = inwardFacilityPreBooking.InwardFacility != null ? inwardFacilityPreBooking.InwardFacility.FacilityNo : "",
                            LotNo = inwardFacilityPreBooking.InwardFacilityCharge != null && inwardFacilityPreBooking.InwardFacilityCharge.FacilityLotID != null ?
                            inwardFacilityPreBooking.InwardFacilityCharge.FacilityLot.LotNo : ""
                        });
                }

                if (outwardFacilityPreBooking != null)
                {
                    if (!mixPoint.OutwardPreBookings.Select(c => c.FacilityPreBookingNo).Contains(outwardFacilityPreBooking.FacilityPreBookingNo))
                        mixPoint.OutwardPreBookings.Add(new FacilityPreBookingPreveiw()
                        {
                            FacilityPreBookingNo = outwardFacilityPreBooking.FacilityPreBookingNo,
                            InsertDate = outwardFacilityPreBooking.InsertDate,
                            FacilityNo = outwardFacilityPreBooking.OutwardFacility != null ? outwardFacilityPreBooking.OutwardFacility.FacilityNo : "",
                            LotNo = outwardFacilityPreBooking.OutwardFacilityCharge != null && outwardFacilityPreBooking.OutwardFacilityCharge.FacilityLotID != null ?
                            outwardFacilityPreBooking.OutwardFacilityCharge.FacilityLot.LotNo : ""
                        });
                }

                if (!result.Ids.ContainsKey(fbPre.TandTv3MixPointFacilityPreBookingID))
                    result.Ids.Add(fbPre.TandTv3MixPointFacilityPreBookingID, MDTrackingStartItemTypeEnum.FacilityPreBooking.ToString());


            }

            // DeliveryNotePositions
            // => TandTv3_MixPointDeliveryNotePos
            mixPoint.DeliveryNotePositions.AddRange(dbMixPoint.TandTv3MixPointDeliveryNotePos_TandTv3MixPoint.Select(c => c.DeliveryNotePos));
            foreach (var item in mixPoint.DeliveryNotePositions)
                if (!result.Ids.ContainsKey(item.DeliveryNotePosID))
                    result.Ids.Add(item.DeliveryNotePosID, MDTrackingStartItemTypeEnum.DeliveryNotePos.ToString());
            if (mixPoint.DeliveryNotePositions != null && mixPoint.DeliveryNotePositions.Any() && mixPoint is TandTv3PointDN)
            {
                TandTv3PointDN tandTv3PointDN = mixPoint as TandTv3PointDN;
                tandTv3PointDN.DeliveryNotePosPreviews = mixPoint.DeliveryNotePositions.Select(c => FactoryDeliveryPreviewModel(c)).ToList();
                tandTv3PointDN.DeliveryNo = tandTv3PointDN.DeliveryNotePosPreviews.Select(c => c.DeliveryNoteNo).FirstOrDefault();
            }

            // Relations
            // => TandTv3_MixPointProdOrderPartslistPosRelation
            mixPoint.Relations = dbMixPoint.TandTv3MixPointProdOrderPartslistPosRelation_TandTv3MixPoint.Select(c => c.ProdOrderPartslistPosRelation).ToList();
            foreach (var relation in mixPoint.Relations)
                if (!result.Ids.ContainsKey(relation.ProdOrderPartslistPosRelationID))
                    result.Ids.Add(relation.ProdOrderPartslistPosRelationID, MDTrackingStartItemTypeEnum.ProdOrderPartslistPosRelation.ToString());

            // ItemsWithLabOrder

            // ProductionPositions
            // ProdOrderPartslistPosID
            mixPoint.IsProductionPoint = dbMixPoint.IsProductionPoint;
            if (dbMixPoint.IsProductionPoint)
            {
                // mixPoint.Pos = dbMixPoint.ProdOrderPartslistPos;
                mixPoint.ProductionPositions = dbMixPoint.TandTv3MixPointProdOrderPartslistPos_TandTv3MixPoint.Select(c => c.ProdOrderPartslistPos).ToList();
                foreach (var positions in mixPoint.ProductionPositions)
                    if (!result.Ids.ContainsKey(positions.ProdOrderPartslistPosID))
                        result.Ids.Add(positions.ProdOrderPartslistPosID, MDTrackingStartItemTypeEnum.ProdOrderPartslistPos.ToString());
                var firstPos = dbMixPoint.TandTv3MixPointProdOrderPartslistPos_TandTv3MixPoint.FirstOrDefault();
                if (firstPos != null)
                {
                    mixPoint.PartslistSequence = firstPos.ProdOrderPartslistPos.ProdOrderPartslist.Sequence;
                    mixPoint.ProgramNo = firstPos.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                    mixPoint.ProdOrder = firstPos.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder;
                    mixPoint.InwardBatchNo = firstPos.ProdOrderPartslistPos.ProdOrderBatchID != null ? firstPos.ProdOrderPartslistPos.ProdOrderBatch.ProdOrderBatchNo : "";
                }

                foreach (var pos in mixPoint.ProductionPositions)
                {
                    if (pos.LabOrder_ProdOrderPartslistPos.Any())
                        mixPoint.ItemsWithLabOrder.Add(MixPointLabOrder.Factory(databaseApp, pos));
                    if (pos.ProdOrderBatchID != null)
                    {
                        mixPoint.BatchNoList.Add(pos.ProdOrderBatch.ProdOrderBatchNo);
                    }
                }

                mixPoint.InwardMaterialNo = dbMixPoint.InwardMaterial.MaterialNo;
                mixPoint.InwardMaterialName = dbMixPoint.InwardMaterial.MaterialName1;
            }

            // InOrderPositions
            // InOrderPosID
            mixPoint.IsInputPoint = dbMixPoint.IsInputPoint;
            if (dbMixPoint.IsInputPoint)
            {

                mixPoint.InOrderPositions = dbMixPoint.TandTv3MixPointInOrderPos_TandTv3MixPoint.Select(c => c.InOrderPos).ToList();
                foreach (var inOrderPos in mixPoint.InOrderPositions)
                {
                    if (inOrderPos.LabOrder_InOrderPos.Any())
                        mixPoint.ItemsWithLabOrder.Add(MixPointLabOrder.Factory(databaseApp, inOrderPos));

                    if (!result.Ids.ContainsKey(inOrderPos.InOrderPosID))
                        result.Ids.Add(inOrderPos.InOrderPosID, MDTrackingStartItemTypeEnum.InOrderPos.ToString());
                }

                mixPoint.PickingPositions = dbMixPoint.TandTv3MixPointPickingPos_TandTv3MixPoint.Select(c => c.PickingPos).ToList();
                foreach (PickingPos pickingPos in mixPoint.PickingPositions)
                    if (!result.Ids.ContainsKey(pickingPos.PickingPosID))
                        result.Ids.Add(pickingPos.PickingPosID, MDTrackingStartItemTypeEnum.PickingPos.ToString());
            }

            // TandTv3_MixPointLot
            foreach (var mixPointLot in dbMixPoint.TandTv3MixPointFacilityLot_TandTv3MixPoint)
            {
                if (mixPointLot.TandTv3MDBookingDirectionID == MDBookingDirectionEnum.Outward.ToString())
                {
                    mixPoint.AddOutwardLot(mixPointLot.FacilityLot);
                }

                if (!result.Ids.ContainsKey(mixPointLot.FacilityLotID))
                    result.Ids.Add(mixPointLot.FacilityLotID, MDTrackingStartItemTypeEnum.FacilityLot.ToString());
            }

            mixPoint.ExistLabOrder = mixPoint.ItemsWithLabOrder.Any();

            // register facilities into ids
            foreach (var facility in mixPoint.OutwardFacilities)
                if (!result.Ids.ContainsKey(facility.Value.FacilityID))
                    result.Ids.Add(facility.Value.FacilityID, MDTrackingStartItemTypeEnum.Facility.ToString());

            foreach (var facility in mixPoint.InwardFacilities)
                if (!result.Ids.ContainsKey(facility.Value.FacilityID))
                    result.Ids.Add(facility.Value.FacilityID, MDTrackingStartItemTypeEnum.Facility.ToString());




            return mixPoint;
        }

        public void ConnectMixPoints(DatabaseApp databaseApp, TandTResult result)
        {
            Dictionary<Guid, ProdOrderPartslistPos> finalMixures = new Dictionary<Guid, ProdOrderPartslistPos>();
            foreach (var mixPoint in result.MixPoints)
            {
                if (mixPoint.ProductionPositions != null)
                {
                    foreach (var secondMixPoint in result.MixPoints)
                    {
                        if (secondMixPoint.MixPointID != mixPoint.MixPointID &&
                            secondMixPoint.ProductionPositions != null &&
                            !secondMixPoint.ProductionPositions.Select(c => c.ProdOrderPartslistPosID).Intersect(mixPoint.ProductionPositions.Select(c => c.ProdOrderPartslistPosID)).Any())
                        {
                            foreach (var rel in secondMixPoint.Relations)
                            {
                                if (mixPoint.ProductionPositions.Select(c => c.ProdOrderPartslistPosID).Contains(rel.SourceProdOrderPartslistPosID))
                                {
                                    AddMixPointRelation(result, mixPoint, secondMixPoint);
                                }
                                if (rel.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                                {
                                    foreach (var childItem in rel.SourceProdOrderPartslistPos.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
                                    {
                                        if (mixPoint.ProductionPositions.Select(c => c.ProdOrderPartslistPosID).Contains(childItem.ProdOrderPartslistPosID))
                                        {
                                            AddMixPointRelation(result, mixPoint, secondMixPoint);
                                        }
                                    }
                                }
                                if (rel.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardRoot
                                    && rel.SourceProdOrderPartslistPos.SourceProdOrderPartslistID != null)
                                {
                                    ProdOrderPartslistPos finalMixure = null; //TandTv3Query.s_cQry_GetFinalMixure(databaseApp, rel.SourceProdOrderPartslistPos.SourceProdOrderPartslistID ?? Guid.Empty);
                                    if (finalMixures.Keys.Contains(rel.SourceProdOrderPartslistPos.ProdOrderPartslistID))
                                        finalMixure = finalMixures[rel.SourceProdOrderPartslistPos.ProdOrderPartslistID];
                                    else
                                    {
                                        finalMixure = rel.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist.Where(p =>
                                            //p.ProdOrderPartslistID == sourceProdOrderPartslistID &&
                                            p.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern &&
                                            !p.Material.MaterialWFRelation_SourceMaterial.Where(c => c.SourceMaterialID != c.TargetMaterialID).Any() &&
                                            !p.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any()
                                        ).FirstOrDefault();
                                        finalMixures.Add(rel.SourceProdOrderPartslistPos.ProdOrderPartslistID, finalMixure);
                                    }

                                    if (finalMixure != null)
                                    {
                                        if (mixPoint.ProductionPositions.Select(c => c.ProdOrderPartslistPosID).Contains(finalMixure.ProdOrderPartslistPosID))
                                        {
                                            AddMixPointRelation(result, mixPoint, secondMixPoint);
                                        }
                                        if (finalMixure.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                                        {
                                            foreach (var childItem in finalMixure.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
                                            {
                                                if (mixPoint.ProductionPositions.Select(c => c.ProdOrderPartslistPosID).Contains(childItem.ProdOrderPartslistPosID))
                                                {
                                                    AddMixPointRelation(result, mixPoint, secondMixPoint);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (mixPoint.InOrderPositions != null && mixPoint.InOrderPositions.Any())
                {
                    foreach (var secondMixPoint in result.MixPoints)
                    {
                        if (secondMixPoint.MixPointID != mixPoint.MixPointID && secondMixPoint.OutwardLotsList.Select(c => c.LotNo).Contains(mixPoint.InwardLot.LotNo))
                        {
                            AddMixPointRelation(result, mixPoint, secondMixPoint);
                        }
                    }
                }
            }
        }

        #endregion

        private void AddMixPointRelation(TandTResult result, TandTv3Point sourceMixPoint, TandTv3Point targetMixPoint)
        {
            if (!result.MixPointRelations.Any(c => c.SourceMixPoint.MixPointID == sourceMixPoint.MixPointID && c.TargetMixPoint.MixPointID == targetMixPoint.MixPointID))
            {
                result.MixPointRelations.Add(
                        new MixPointRelation()
                        {
                            SourceMixPoint = sourceMixPoint,
                            TargetMixPoint = targetMixPoint
                        }
                    );
            }
        }
        #endregion

    }

}

