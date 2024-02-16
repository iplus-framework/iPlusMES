using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class TandTv3Process<T> : ITandTv3Process where T : IACObjectEntity
    {

        #region ctor's
        public TandTv3Process(DatabaseApp databaseApp, TandTv3Command tandTv3Command, TandTv3FilterTracking filter, T startItem, string vbUserNo, bool useGroupResult)
        {
            DatabaseApp = databaseApp;
            StartItem = startItem;
            TandTv3Command = tandTv3Command;
            UseGroupResult = useGroupResult;
            TandTResult = TandTv3Command.PrepareTracking(DatabaseApp, filter, vbUserNo);
            if (!filter.CheckCancelWork())
                DoTracking();
        }
        #endregion

        #region Properties
        public DatabaseApp DatabaseApp { get; set; }

        public TandTResult TandTResult { get; set; }

        public TandTv3Command TandTv3Command { get; set; }

        public T StartItem { get; set; }

        public bool UseGroupResult { get; set; }

        #endregion

        #region  ITandTv3Process

        #region ITandTv3Process -> Factory  IItemTracking<IACObjectEntity>
        public virtual IItemTracking<IACObjectEntity> FactoryBacwardItem(IACObjectEntity item)
        {
            IItemTracking<IACObjectEntity> tmpItem = null;

            if (item is gip.mes.datamodel.ACClass)
            {
                gip.mes.datamodel.ACClass aCClass = item as gip.mes.datamodel.ACClass;
                if (!TandTResult.Ids.Keys.Contains(aCClass.ACClassID))
                {
                    tmpItem = new DoBackward_ACClass(DatabaseApp, TandTResult, aCClass);
                }
            }
            if (item is DeliveryNote)
            {
                DeliveryNote deliveryNote = item as DeliveryNote;
                if (!TandTResult.Ids.Keys.Contains(deliveryNote.DeliveryNoteID))
                {
                    tmpItem = new DoBackward_DeliveryNote(DatabaseApp, TandTResult, deliveryNote);
                }
            }
            if (item is DeliveryNotePos)
            {
                DeliveryNotePos deliveryNotePos = item as DeliveryNotePos;
                if (!TandTResult.Ids.Keys.Contains(deliveryNotePos.DeliveryNotePosID))
                {
                    tmpItem = new DoBackward_DeliveryNotePos(DatabaseApp, TandTResult, deliveryNotePos);
                }
            }
            if (item is Facility)
            {
                Facility facility = item as Facility;
                if (!TandTResult.Ids.Keys.Contains(facility.FacilityID))
                {
                    tmpItem = new DoBackward_Facility(DatabaseApp, TandTResult, facility);
                }
            }
            if (item is FacilityBooking)
            {
                FacilityBooking facilityBooking = item as FacilityBooking;
                if (!TandTResult.Ids.Keys.Contains(facilityBooking.FacilityBookingID))
                {
                    tmpItem = new DoBackward_FacilityBooking(DatabaseApp, TandTResult, facilityBooking);
                }
            }
            if (item is FacilityBookingCharge)
            {
                FacilityBookingCharge facilityBookingCharge = item as FacilityBookingCharge;
                if (!TandTResult.Ids.Keys.Contains(facilityBookingCharge.FacilityBookingChargeID))
                {
                    tmpItem = new DoBackward_FacilityBookingCharge(DatabaseApp, TandTResult, facilityBookingCharge);
                }
            }
            if (item is FacilityCharge)
            {
                FacilityCharge facilityCharge = item as FacilityCharge;
                if (!TandTResult.Ids.Keys.Contains(facilityCharge.FacilityChargeID))
                {
                    tmpItem = new DoBackward_FacilityCharge(DatabaseApp, TandTResult, facilityCharge);
                }
            }
            if (item is FacilityLot)
            {
                FacilityLot facilityLot = item as FacilityLot;
                if (!TandTResult.Ids.Keys.Contains(facilityLot.FacilityLotID))
                {
                    tmpItem = new DoBackward_FacilityLot(DatabaseApp, TandTResult, facilityLot);
                }
            }
            if (item is FacilityPreBooking)
            {
                FacilityPreBooking facilityPreBooking = item as FacilityPreBooking;
                if (!TandTResult.Ids.Keys.Contains(facilityPreBooking.FacilityPreBookingID))
                {
                    tmpItem = new DoBackward_FacilityPreBooking(DatabaseApp, TandTResult, facilityPreBooking);
                }
            }
            if (item is InOrder)
            {
                InOrder inOrder = item as InOrder;
                if (!TandTResult.Ids.Keys.Contains(inOrder.InOrderID))
                {
                    tmpItem = new DoBackward_InOrder(DatabaseApp, TandTResult, inOrder);
                }
            }
            if (item is InOrderPos)
            {
                InOrderPos inOrderPos = item as InOrderPos;
                if (!TandTResult.Ids.Keys.Contains(inOrderPos.InOrderPosID))
                {
                    tmpItem = new DoBackward_InOrderPos(DatabaseApp, TandTResult, inOrderPos);
                }
            }
            if (item is OutOrder)
            {
                OutOrder outOrder = item as OutOrder;
                if (!TandTResult.Ids.Keys.Contains(outOrder.OutOrderID))
                {
                    tmpItem = new DoBackward_OutOrder(DatabaseApp, TandTResult, outOrder);
                }
            }
            if (item is OutOrderPos)
            {
                OutOrderPos outOrderPos = item as OutOrderPos;
                if (!TandTResult.Ids.Keys.Contains(outOrderPos.OutOrderPosID))
                {
                    tmpItem = new DoBackward_OutOrderPos(DatabaseApp, TandTResult, outOrderPos);
                }
            }
            if (item is ProdOrder)
            {
                ProdOrder prodOrder = item as ProdOrder;
                if (!TandTResult.Ids.Keys.Contains(prodOrder.ProdOrderID))
                {
                    tmpItem = new DoBackward_ProdOrder(DatabaseApp, TandTResult, prodOrder);
                }
            }
            if (item is ProdOrderPartslist)
            {
                ProdOrderPartslist prodOrderPartslist = item as ProdOrderPartslist;
                if (!TandTResult.Ids.Keys.Contains(prodOrderPartslist.ProdOrderPartslistID))
                {
                    tmpItem = new DoBackward_ProdOrderPartslist(DatabaseApp, TandTResult, prodOrderPartslist);
                }
            }
            if (item is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos prodOrderPartslistPos = item as ProdOrderPartslistPos;
                if (!TandTResult.Ids.Keys.Contains(prodOrderPartslistPos.ProdOrderPartslistPosID))
                {
                    tmpItem = new DoBackward_ProdOrderPartslistPos(DatabaseApp, TandTResult, prodOrderPartslistPos);
                }
            }
            if (item is ProdOrderPartslistPosRelation)
            {
                ProdOrderPartslistPosRelation prodOrderPartslistPosRelation = item as ProdOrderPartslistPosRelation;
                if (!TandTResult.Ids.Keys.Contains(prodOrderPartslistPosRelation.ProdOrderPartslistPosRelationID))
                {
                    tmpItem = new DoBackward_ProdOrderPartslistPosRelation(DatabaseApp, TandTResult, prodOrderPartslistPosRelation);
                }
            }

            return tmpItem;
        }

        public virtual IItemTracking<IACObjectEntity> FactoryForwardItem(IACObjectEntity item)
        {
            IItemTracking<IACObjectEntity> tmpItem = null;

            if (item is gip.mes.datamodel.ACClass)
            {
                gip.mes.datamodel.ACClass aCClass = item as gip.mes.datamodel.ACClass;
                if (!TandTResult.Ids.Keys.Contains(aCClass.ACClassID))
                {
                    tmpItem = new DoForward_ACClass(DatabaseApp, TandTResult, aCClass);
                }
            }
            if (item is DeliveryNote)
            {
                DeliveryNote deliveryNote = item as DeliveryNote;
                if (!TandTResult.Ids.Keys.Contains(deliveryNote.DeliveryNoteID))
                {
                    tmpItem = new DoForward_DeliveryNote(DatabaseApp, TandTResult, deliveryNote);
                }
            }
            if (item is DeliveryNotePos)
            {
                DeliveryNotePos deliveryNotePos = item as DeliveryNotePos;
                if (!TandTResult.Ids.Keys.Contains(deliveryNotePos.DeliveryNotePosID))
                {
                    tmpItem = new DoForward_DeliveryNotePos(DatabaseApp, TandTResult, deliveryNotePos) as IItemTracking<IACObjectEntity>;
                }
            }
            if (item is Facility)
            {
                Facility facility = item as Facility;
                if (!TandTResult.Ids.Keys.Contains(facility.FacilityID))
                {
                    tmpItem = new DoForward_Facility(DatabaseApp, TandTResult, facility);
                }
            }
            if (item is FacilityBooking)
            {
                FacilityBooking facilityBooking = item as FacilityBooking;
                if (!TandTResult.Ids.Keys.Contains(facilityBooking.FacilityBookingID))
                {
                    tmpItem = new DoForward_FacilityBooking(DatabaseApp, TandTResult, facilityBooking);
                }
            }
            if (item is FacilityBookingCharge)
            {
                FacilityBookingCharge facilityBookingCharge = item as FacilityBookingCharge;
                if (!TandTResult.Ids.Keys.Contains(facilityBookingCharge.FacilityBookingChargeID))
                {
                    tmpItem = new DoForward_FacilityBookingCharge(DatabaseApp, TandTResult, facilityBookingCharge);
                }
            }
            if (item is FacilityCharge)
            {
                FacilityCharge facilityCharge = item as FacilityCharge;
                if (!TandTResult.Ids.Keys.Contains(facilityCharge.FacilityChargeID))
                {
                    tmpItem = new DoForward_FacilityCharge(DatabaseApp, TandTResult, facilityCharge);
                }
            }
            if (item is FacilityLot)
            {
                FacilityLot facilityLot = item as FacilityLot;
                if (!TandTResult.Ids.Keys.Contains(facilityLot.FacilityLotID))
                {
                    tmpItem = new DoForward_FacilityLot(DatabaseApp, TandTResult, facilityLot);
                }
            }
            if (item is FacilityPreBooking)
            {
                FacilityPreBooking facilityPreBooking = item as FacilityPreBooking;
                if (!TandTResult.Ids.Keys.Contains(facilityPreBooking.FacilityPreBookingID))
                {
                    tmpItem = new DoForward_FacilityPreBooking(DatabaseApp, TandTResult, facilityPreBooking);
                }
            }
            if (item is InOrder)
            {
                InOrder inOrder = item as InOrder;
                if (!TandTResult.Ids.Keys.Contains(inOrder.InOrderID))
                {
                    tmpItem = new DoForward_InOrder(DatabaseApp, TandTResult, inOrder);
                }
            }
            if (item is InOrderPos)
            {
                InOrderPos inOrderPos = item as InOrderPos;
                if (!TandTResult.Ids.Keys.Contains(inOrderPos.InOrderPosID))
                {
                    tmpItem = new DoForward_InOrderPos(DatabaseApp, TandTResult, inOrderPos);
                }
            }
            if (item is OutOrder)
            {
                OutOrder outOrder = item as OutOrder;
                if (!TandTResult.Ids.Keys.Contains(outOrder.OutOrderID))
                {
                    tmpItem = new DoForward_OutOrder(DatabaseApp, TandTResult, outOrder);
                }
            }
            if (item is OutOrderPos)
            {
                OutOrderPos outOrderPos = item as OutOrderPos;
                if (!TandTResult.Ids.Keys.Contains(outOrderPos.OutOrderPosID))
                {
                    tmpItem = new DoForward_OutOrderPos(DatabaseApp, TandTResult, outOrderPos);
                }
            }
            if (item is ProdOrder)
            {
                ProdOrder prodOrder = item as ProdOrder;
                if (!TandTResult.Ids.Keys.Contains(prodOrder.ProdOrderID))
                {
                    tmpItem = new DoForward_ProdOrder(DatabaseApp, TandTResult, prodOrder);
                }
            }
            if (item is ProdOrderPartslist)
            {
                ProdOrderPartslist prodOrderPartslist = item as ProdOrderPartslist;
                if (!TandTResult.Ids.Keys.Contains(prodOrderPartslist.ProdOrderPartslistID))
                {
                    tmpItem = new DoForward_ProdOrderPartslist(DatabaseApp, TandTResult, prodOrderPartslist);
                }
            }
            if (item is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos prodOrderPartslistPos = item as ProdOrderPartslistPos;
                if (!TandTResult.Ids.Keys.Contains(prodOrderPartslistPos.ProdOrderPartslistPosID))
                {
                    tmpItem = new DoForward_ProdOrderPartslistPos(DatabaseApp, TandTResult, prodOrderPartslistPos);
                }
            }
            if (item is ProdOrderPartslistPosRelation)
            {
                ProdOrderPartslistPosRelation prodOrderPartslistPosRelation = item as ProdOrderPartslistPosRelation;
                if (!TandTResult.Ids.Keys.Contains(prodOrderPartslistPosRelation.ProdOrderPartslistPosRelationID))
                {
                    tmpItem = new DoForward_ProdOrderPartslistPosRelation(DatabaseApp, TandTResult, prodOrderPartslistPosRelation);
                }
            }

            return tmpItem;
        }

        #endregion

        #region ITandTv3Process -> Operate [same|next] step items
        public virtual List<IItemTracking<IACObjectEntity>> OperateSameStepItems(List<IACObjectEntity> sameStepItems, MDTrackingDirectionEnum trackingDirection, IItemTracking<IACObjectEntity> callerItem)
        {
            List<IItemTracking<IACObjectEntity>> sameStepResult = new List<IItemTracking<IACObjectEntity>>();
            foreach (var sameStepItem in sameStepItems)
            {
                if (trackingDirection == MDTrackingDirectionEnum.Backward)
                {
                    IItemTracking<IACObjectEntity> backwardTrackingItem = FactoryBacwardItem(sameStepItem);
                    if (backwardTrackingItem != null)
                        sameStepResult.Add(backwardTrackingItem);
                }
                else if (trackingDirection == MDTrackingDirectionEnum.Forward)
                {
                    IItemTracking<IACObjectEntity> forwardTrackingItem = FactoryForwardItem(sameStepItem);
                    if (forwardTrackingItem != null)
                        sameStepResult.Add(forwardTrackingItem);
                }
            }


            if (sameStepResult != null)
                foreach (var childItem in sameStepResult)
                    childItem.SameStepParent = callerItem;

            return sameStepResult;
        }

        public virtual List<IItemTracking<IACObjectEntity>> OperateNextStepItems(List<IACObjectEntity> nextStepItems, MDTrackingDirectionEnum trackingDirection, IItemTracking<IACObjectEntity> callerItem)
        {
            List<IItemTracking<IACObjectEntity>> nextStepResult = new List<IItemTracking<IACObjectEntity>>(); ;
            foreach (var nextStepItem in nextStepItems)
            {
                if (trackingDirection == MDTrackingDirectionEnum.Backward)
                {
                    IItemTracking<IACObjectEntity> backwardTrackingItem = FactoryBacwardItem(nextStepItem);
                    if (backwardTrackingItem != null)
                        nextStepResult.Add(backwardTrackingItem);
                }
                else if (trackingDirection == MDTrackingDirectionEnum.Forward)
                {
                    IItemTracking<IACObjectEntity> forwardTrackingItem = FactoryForwardItem(nextStepItem);
                    if (forwardTrackingItem != null)
                        nextStepResult.Add(forwardTrackingItem);
                }
            }

            if (nextStepResult != null)
                foreach (var childItem in nextStepResult)
                    childItem.NextStepParent = callerItem;

            return nextStepResult;
        }

        #endregion

        #region Process

        #region Process -> Main

        public void Process()
        {
            if (TandTResult.Filter.CheckCancelWork())
                return;
            if (TandTResult.Filter.BackgroundWorker != null)
            {
                TandTResult.Filter.BackgroundWorker.ProgressInfo.OnlyTotalProgress = false;
                TandTResult.Filter.BackgroundWorker.ProgressInfo.ProgressInfoIsIndeterminate = true;
                TandTResult.Filter.BackgroundWorker.ProgressInfo.TotalProgress.ProgressText = string.Format("Tracking: [{0}] | Start fetching data...", TandTResult.Filter.ItemSystemNo);
            }
            IItemTracking<IACObjectEntity> itemTracking = null;

            if(StartItem is FacilityBooking)
            {
                FacilityBooking facilityBooking = StartItem as FacilityBooking;
                if(facilityBooking != null)
                {
                    if(facilityBooking.InwardTargetQuantity > 0)
                    {
                        TandTResult.StartingQuantity = facilityBooking.InwardTargetQuantity;
                    }
                    else if(facilityBooking.OutwardQuantity > 0)
                    {
                        TandTResult.StartingQuantity = facilityBooking.OutwardTargetQuantity;
                    }
                }
            }

            if (TandTResult.Filter.MDTrackingDirectionEnum == MDTrackingDirectionEnum.Backward)
                itemTracking = FactoryBacwardItem(StartItem);
            if (TandTResult.Filter.MDTrackingDirectionEnum == MDTrackingDirectionEnum.Forward)
                itemTracking = FactoryForwardItem(StartItem);

#if DEBUG
            if (TandTResult.Filter != null && TandTv3Command.TrackingConfiguration != null)
            {
                string fileName = TandTv3Command.GetLogFileName(TandTResult.Filter.ItemSystemNo, TandTv3Command.TrackingConfiguration.UseMDFile);
                if (TandTv3Command.TandTWriteDiagnosticLog && Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            TandTResult.LogWritter = sw;
                            #region T&Tv3 Log
                            if (TandTResult.LogWritter != null)
                            {
                                TandTResult.LogWritter.WriteLine("Tracking log");
                                TandTResult.LogWritter.WriteLine(new String('-', 20));
                                TandTResult.LogWritter.WriteLine(string.Format(@"Direction: {0}", TandTResult.Filter.MDTrackingDirectionEnum.ToString()));
                                TandTResult.LogWritter.WriteLine(string.Format(@"Start Type: {0}", TandTResult.Filter.MDTrackingStartItemTypeEnum.ToString()));
                                TandTResult.LogWritter.WriteLine(string.Format(@"ItemNo: {0}", TandTResult.Filter.ItemSystemNo));
                                TandTResult.LogWritter.WriteLine(string.Format(@"PrimaryKeyID: {0}", TandTResult.Filter.PrimaryKeyID.ToString()));
                            }
                            #endregion
                            Process(TandTResult.Filter.MDTrackingDirectionEnum, new List<IItemTracking<IACObjectEntity>>() { itemTracking });
                        }
                    }
                }
                else
                    Process(TandTResult.Filter.MDTrackingDirectionEnum, new List<IItemTracking<IACObjectEntity>>() { itemTracking });
            }
#else
            Process(TandTResult.Filter.MDTrackingDirectionEnum, new List<IItemTracking<IACObjectEntity>>() { itemTracking });
#endif

            TandTv3Command.ConnectMixPoints(DatabaseApp, TandTResult);

        }

        #endregion

        #region Process -> Helpers
        public void Process(MDTrackingDirectionEnum trackingDirection, List<IItemTracking<IACObjectEntity>> items)
        {
            if (TandTResult.Filter.CheckCancelWork())
                return;
            TandTResult.CurrentStep = FactoryNextStepItem();
            string subTaskName = string.Format(@"Step{0}", TandTResult.CurrentStep.StepNo);
            if (TandTResult.Filter.BackgroundWorker != null)
            {
                TandTResult.Filter.BackgroundWorker.ProgressInfo.AddSubTask(subTaskName, 0, 0);
                TandTResult.Filter.BackgroundWorker.ProgressInfo.TotalProgress.ProgressText = string.Format("Tracking: [{0}] | Process() Step {0} ... ", TandTResult.CurrentStep.StepNo);
                TandTResult.Filter.BackgroundWorker.ProgressInfo.ReportProgress(subTaskName, 0, string.Format(@"Tracking: [{0}] | Starting fetch Step {1} items...", TandTResult.Filter.ItemSystemNo, TandTResult.CurrentStep.StepNo));
            }

            TandTResult.Steps.Add(TandTResult.CurrentStep);

            #region T&Tv3 Log
            if (TandTResult.LogWritter != null)
            {
                TandTResult.LogWritter.WriteLine(TandTResult.CurrentStep);
                TandTResult.LogWritter.WriteLine(new String('=', 20));
                TandTResult.LogWritter.WriteLine("");
            }
            #endregion

            List<IItemTracking<IACObjectEntity>> trackSameStepItems = new List<IItemTracking<IACObjectEntity>>();
            foreach (var item in items)
            {
                if (TandTResult.Filter.BackgroundWorker != null)
                {
                    TandTResult.Filter.BackgroundWorker.ProgressInfo.ReportProgress(subTaskName, 0, string.Format(@"Tracking: [{0}] | Processing item {1} ...", TandTResult.Filter.ItemSystemNo, item.ToString()));
                }
                item.Step = TandTResult.CurrentStep;
                List<IItemTracking<IACObjectEntity>> tmpSameStepItems = ProcessSameStepItems(item, trackingDirection);
                trackSameStepItems.AddRange(tmpSameStepItems);
            }

            foreach (var trackItemSameStep in trackSameStepItems)
            {
                trackItemSameStep.Step = TandTResult.CurrentStep;
            }

            //trackSameStepItems =
            //    trackSameStepItems
            //    .OrderBy(c => c.Item, new IACObjectEntityComparer())
            //    .ToList();

            #region T&Tv3 Log
            if (TandTResult.LogWritter != null)
            {
                TandTResult.LogWritter.WriteLine(new String('=', 20));
                TandTResult.LogWritter.WriteLine("Step items:");
                TandTResult.LogWritter.WriteLine(new String('-', 20));
                foreach (var item in trackSameStepItems)
                {
                    if (TandTv3Command.TrackingConfiguration.UseMDFile)
                    {
                        TandTResult.LogWritter.WriteLine(item.ToMDString());
                    }
                    else
                    {
                        TandTResult.LogWritter.WriteLine(item);
                        TandTResult.LogWritter.WriteLine("");
                    }
                }
                TandTResult.LogWritter.WriteLine(new String('=', 20));
                TandTResult.LogWritter.WriteLine("");
            }

            #endregion

            string[] getPointitems = new string[] { "FacilityBookingCharge", "FacilityPreBooking" };

            foreach (var trackItemSameStep in trackSameStepItems.Where(c => getPointitems.Contains(c.Item.ACType.ACIdentifier)))
            {
                trackItemSameStep.AssignItemToMixPoint(trackSameStepItems.Select(t => t.Item).ToList());
            }

            foreach (var trackItemSameStep in trackSameStepItems.Where(c => !getPointitems.Contains(c.Item.ACType.ACIdentifier)))
            {
                trackItemSameStep.AssignItemToMixPoint(trackSameStepItems.Select(t => t.Item).ToList());
            }

            List<IItemTracking<IACObjectEntity>> trackNextStepItems = new List<IItemTracking<IACObjectEntity>>();

            foreach (var trackItemSameStep in trackSameStepItems)
            {
                List<IACObjectEntity> itemGetNextStepResult = trackItemSameStep.GetNextStepItems();
                if (itemGetNextStepResult != null)
                {
                    List<IItemTracking<IACObjectEntity>> itemNextStepItems = OperateNextStepItems(itemGetNextStepResult, trackingDirection, trackItemSameStep);
                    if (itemNextStepItems != null && itemNextStepItems.Any())
                    {
                        trackNextStepItems.AddRange(itemNextStepItems);
                    }
                }
            }

            bool breakTracking = TandTResult.Filter.BreakTrackingCondition != null && TandTResult.Filter.BreakTrackingCondition.Invoke(TandTResult.CurrentStep);

            if (!breakTracking && !TandTResult.Filter.CheckCancelWork())
            {
                if (trackNextStepItems.Any())
                {
                    //#region T&Tv3 Log
                    //if (TandTResult.LogWritter != null)
                    //{
                    //    TandTResult.LogWritter.WriteLine("Item Next Step Items:");
                    //    TandTResult.LogWritter.WriteLine(new String('>', 20));
                    //    foreach (var nextStepItem in trackNextStepItems)
                    //    {
                    //        TandTResult.LogWritter.WriteLine(nextStepItem);
                    //        TandTResult.LogWritter.WriteLine("");
                    //    }
                    //    TandTResult.LogWritter.WriteLine(new String('<', 20));
                    //    TandTResult.LogWritter.WriteLine("");
                    //}
                    //#endregion
                    Process(trackingDirection, trackNextStepItems);
                }
            }
            else
            {
                var test = breakTracking;
            }
        }

        public TandTStep FactoryNextStepItem()
        {
            int lastStepNr = 0;
            if (TandTResult.Steps.Any())
                lastStepNr = TandTResult.Steps.Max(c => c.StepNo);
            lastStepNr++;
            TandTStep step = new TandTStep();
            step.StepNo = lastStepNr;
            return step;
        }

        public List<IItemTracking<IACObjectEntity>> ProcessSameStepItems(IItemTracking<IACObjectEntity> item, MDTrackingDirectionEnum trackingDirection)
        {
            List<IItemTracking<IACObjectEntity>> sameStepItems = new List<IItemTracking<IACObjectEntity>>();
            sameStepItems.Add(item);

            List<IACObjectEntity> itemGetSameStepResult = item.GetSameStepItems();
            if (itemGetSameStepResult != null)
            {
                List<IItemTracking<IACObjectEntity>> itemSameStepItems = OperateSameStepItems(itemGetSameStepResult, trackingDirection, item);
                if (itemSameStepItems != null)
                {
                    foreach (var childItem in itemSameStepItems)
                    {
                        var tmpSameStepItems = ProcessSameStepItems(childItem, trackingDirection);
                        if (tmpSameStepItems != null)
                            sameStepItems.AddRange(tmpSameStepItems);
                    }
                }
            }
            return sameStepItems;
        }

        #endregion

        #endregion



        public virtual void DoTracking()
        {
            string previousCommand = "";
            if (TandTResult.Filter.IsNew || TandTResult.Filter.IsDynamic)
            {
                previousCommand = "Process()";
                Process();
                MsgWithDetails saveMsg = null;
                if (!TandTResult.Filter.IsDynamic)
                {
                    saveMsg = TandTv3Command.DoSave(DatabaseApp, TandTResult);
                }

                if (saveMsg == null)
                {
                    TandTResult.EndTime = DateTime.Now;
                    TandTResult.Filter.EndTime = TandTResult.EndTime;
                    DatabaseApp.ACSaveChanges();
                    TandTResult.Success = true;
                }
                else
                    TandTResult.ErrorMsg = saveMsg;
            }
            else
            {
                previousCommand = "DoSelect()";
                TandTResult = TandTv3Command.DoSelect(DatabaseApp, TandTResult.Filter);
            }

            if (TandTResult.Filter.BackgroundWorker != null)
                TandTResult.Filter.BackgroundWorker.ProgressInfo.TotalProgress.ProgressText = string.Format(@"Tracking: [{0}] | {1} --> DoTrackingFinish() ...", TandTResult.Filter.ItemSystemNo, previousCommand);
            TandTv3Command.DoTrackingFinish(DatabaseApp, TandTResult, TandTResult.Filter);
            if (UseGroupResult)
                TandTResult = TandTv3Command.BuildGroupResult(DatabaseApp, TandTResult);
        }

        #endregion

        #region Helper methods
        private bool DebugItem(IItemTracking<IACObjectEntity> item)
        {
            return false;
            //string[] debugItemNos = new string[] { "aea95994-7d4d-4986-9cd8-1b1e746a0644" };
            //MDTrackingStartItemTypeEnum[] debugItemTypes =
            //new MDTrackingStartItemTypeEnum[]
            //{
            //        MDTrackingStartItemTypeEnum.FacilityBookingCharge,
            //        MDTrackingStartItemTypeEnum.FacilityBooking,
            //        MDTrackingStartItemTypeEnum.ProdOrderPartslistPos,
            //        MDTrackingStartItemTypeEnum.ProdOrderPartslistPosRelation,
            //        MDTrackingStartItemTypeEnum.FacilityLot
            //};
            //if (debugItemTypes.Select(c => c.ToString()).Contains(item.Item.ACType.ACIdentifier))
            //{
            //    foreach (var debugItemNo in debugItemNos)
            //        if (item.GetItemNo().EndsWith(debugItemNo))
            //            return true;
            //}
            //return false;
        }

       #endregion
    }
}
