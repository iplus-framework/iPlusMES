using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'DischargingItemManager'}de{'DischargingItemManager'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class DischargingItemManager : PARole
    {
        public const string C_DefaultServiceACIdentifier = "\\LocalServiceObjects\\DischargingItemManager";

        #region DI

        public IACContainerTNet<PANotifyState> ProcessAlarm { get; set; }

        public DischargingItemNoValidator DischargingItemNoValidator { get; private set; }

        public string ClassName { get; private set; }
        #endregion

        #region ctor's

        public DischargingItemManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
           : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }


        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseInit = base.ACInit(startChildMode);
            DischargingItemNoValidator = new DischargingItemNoValidator(this, nameof(DischargingItemManager));
            return baseInit;
        }


        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            bool baseDeInit = base.ACDeInit(deleteACClassTask);
            DischargingItemNoValidator = null;
            return baseDeInit;
        }
        #endregion

        #region Static instance

        public static ACComponent GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACComponent>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<DischargingItemManager> ACRefToServiceInstance(ACComponent requester)
        {
            DischargingItemManager serviceInstance = GetServiceInstance(requester) as DischargingItemManager;
            if (serviceInstance != null)
                return new ACRef<DischargingItemManager>(serviceInstance, requester);
            return null;
        }

        #endregion

        #region Manager


        #endregion

        #region Discharging Booking

        public KeyValuePair<Msg, DischargingItem> ProceeedBooking(FacilityManager facilityManager, ACProdOrderManager prodOrderManager, ManualPreparationSourceInfoTypeEnum sourceInfoType, string id, DischargingItem dischargingItem, string propertyACUrl = "")
        {
            BinSelectionModel binSelectionModel = new BinSelectionModel();
            binSelectionModel.ProdorderPartslistPosRelationID = dischargingItem.ProdorderPartslistPosRelationID ?? Guid.Empty;
            binSelectionModel.RestQuantity = dischargingItem.InwardBookingQuantityUOM;
            switch (sourceInfoType)
            {
                case ManualPreparationSourceInfoTypeEnum.FacilityChargeID:
                    binSelectionModel.FacilityChargeID = new Guid(id);
                    break;
                case ManualPreparationSourceInfoTypeEnum.FacilityID:
                    binSelectionModel.FacilityID = new Guid(id);
                    break;
                default:
                    break;
            }
            return DoOutwardBooking(facilityManager, prodOrderManager, binSelectionModel, propertyACUrl);
        }

        public virtual KeyValuePair<Msg, DischargingItem> DoOutwardBooking(FacilityManager facilityManager, ACProdOrderManager prodOrderManager, BinSelectionModel binSelectionModel, string propertyACUrl = "")
        {
            MsgWithDetails collectedMessages = new MsgWithDetails();
            Msg msg = null;
            DischargingItem outwardDischargingItem = null;
            bool changePosState = true;
            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                try
                {
                    MDProdOrderPartslistPosState posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                    Facility facility = null;
                    FacilityCharge facilityCharge = null;
                    if (binSelectionModel.FacilityChargeID != null)
                    {
                        facilityCharge = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == (binSelectionModel.FacilityChargeID ?? Guid.Empty));
                        facility = facilityCharge.Facility;
                    }
                    else
                    {
                        facility = dbApp.Facility.FirstOrDefault(c => c.FacilityID == (binSelectionModel.FacilityID ?? Guid.Empty));
                        facilityCharge = facility.FacilityCharge_Facility.FirstOrDefault();
                    }
                    ProdOrderPartslistPosRelation relation = dbApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == (binSelectionModel.ProdorderPartslistPosRelationID ?? Guid.Empty));
                    FacilityPreBooking facilityPreBooking = prodOrderManager.NewOutwardFacilityPreBooking(facilityManager, dbApp, relation);
                    ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                    bookingParam.OutwardQuantity = binSelectionModel.RestQuantity;
                    bookingParam.OutwardFacility = facility;
                    bookingParam.PropertyACUrl = propertyACUrl;
                    //bookingParam.MDReleaseState = MDReleaseState.DefaultMDReleaseState(dbApp, MDReleaseState.ReleaseStates.Free);
                    msg = dbApp.ACSaveChangesWithRetry();
                    if (msg != null)
                    {
                        collectedMessages.AddDetailMessage(msg);
                        Root.Messages.LogError(GetACUrl(), "DoOutwardBooking(50)", msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, ClassName, "DoOutwardBooking(40)", 991), false);
                        changePosState = false;
                    }
                    else
                    {
                        bookingParam.IgnoreIsEnabled = true;
                        ACMethodEventArgs resultBooking = facilityManager.BookFacilityWithRetry(ref bookingParam, dbApp) as ACMethodEventArgs;
                        if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                        {
                            collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                            OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, ClassName, "DoOutwardBooking(60)", 1016), false);
                            changePosState = false;
                        }
                        else
                        {
                            if (!bookingParam.ValidMessage.IsSucceded() || bookingParam.ValidMessage.HasWarnings())
                            {
                                Root.Messages.LogError(GetACUrl(), "DoOutwardBooking(70)", bookingParam.ValidMessage.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, ClassName, "DoOutwardBooking(70)", 1024), false);
                                changePosState = false;
                            }
                            changePosState = true;
                            if (bookingParam.ValidMessage.IsSucceded())
                            {
                                FacilityBooking facilityBooking = relation.FacilityBooking_ProdOrderPartslistPosRelation.FirstOrDefault();
                                outwardDischargingItem = new DischargingItem();
                                outwardDischargingItem.OutwardBookingNo = facilityBooking.FacilityBookingNo;
                                outwardDischargingItem.OutwardBookingQuantityUOM = facilityBooking.OutwardQuantity;
                                outwardDischargingItem.OutwardBookingTime = facilityBooking.InsertDate;

                                facilityPreBooking.DeleteACObject(dbApp, true);
                                relation.IncreaseActualQuantityUOM(bookingParam.OutwardQuantity.Value);
                                msg = dbApp.ACSaveChangesWithRetry();
                                if (msg != null)
                                {
                                    collectedMessages.AddDetailMessage(msg);
                                    Root.Messages.LogError(GetACUrl(), "DoOutwardBooking(80)", msg.InnerMessage);
                                    OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, ClassName, "DoOutwardBooking(80)", 1036), false);
                                }
                            }
                            else
                            {
                                collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                            }

                            if (changePosState)
                                relation.MDProdOrderPartslistPosState = posState;

                            msg = dbApp.ACSaveChangesWithRetry();
                            if (msg != null)
                            {
                                collectedMessages.AddDetailMessage(msg);
                                Root.Messages.LogError(GetACUrl(), "DoOutwardBooking(90)", msg.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, ClassName, "DoOutwardBooking(90)", 1048), false);
                            }
                            else
                            {
                                relation.RecalcActualQuantityFast();
                                if (dbApp.IsChanged)
                                    dbApp.ACSaveChangesWithRetry();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    collectedMessages.AddDetailMessage(new Msg(eMsgLevel.Exception, e.Message));
                    Root.Messages.LogException(GetACUrl(), "DoOutwardBooking(100)", e);
                }
            }
            return new KeyValuePair<Msg, DischargingItem>(collectedMessages.MsgDetailsCount > 0 ? collectedMessages : null, outwardDischargingItem);
        }
        #endregion

        #region Discharging List

        public List<DischargingItem> LoadDischargingItemList(Guid intermediateChildPosID, ManualPreparationSourceInfoTypeEnum sourceInfoType)
        {
            List<DischargingItem> items = new List<DischargingItem>();
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                ProdOrderPartslistPos intermedatePartPos = databaseApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == intermediateChildPosID);
                switch (sourceInfoType)
                {
                    case ManualPreparationSourceInfoTypeEnum.FacilityID:
                        items = LoadDischargingItemListForFacility(intermedatePartPos);
                        break;
                    case ManualPreparationSourceInfoTypeEnum.FacilityChargeID:
                        items = LoadDischargingItemListForFacilityCharge(intermedatePartPos);
                        break;
                }

                var reservations = intermedatePartPos
                   .FacilityBooking_ProdOrderPartslistPos
                   .Where(c => c.InwardQuantity == PWBinSelection.BinSelectionReservationQuantity)
                   .Select(c => new { c.FacilityBookingNo, c.InwardFacilityChargeID })
                   .ToList();

                // Expect only one source relation per batch
                // Can cumulate outward FB per many Relations
                // By creating new outward booking 
                ProdOrderPartslistPosRelation nextMixureRelation =
                    intermedatePartPos
                    .ProdOrderPartslistPos1_ParentProdOrderPartslistPos
                    .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                    .Where(c => c.TargetProdOrderPartslistPos.ProdOrderBatchID == intermedatePartPos.ProdOrderBatchID)
                    .FirstOrDefault();

                foreach (var item in items)
                {
                    if (sourceInfoType == ManualPreparationSourceInfoTypeEnum.FacilityID && item.InwardFacilityNo != null)
                    {
                        double inwardQuantity = intermedatePartPos.FacilityBooking_ProdOrderPartslistPos.Where(c => c.InwardFacility?.FacilityNo == item.InwardFacilityNo)
                                                                                                        .Sum(x => x.InwardQuantity);
                        item.InwardBookingQuantityUOM = inwardQuantity;
                    }
                    else
                    {
                        if (reservations.Any(c => c.InwardFacilityChargeID == item.InwardFacilityChargeID))
                            item.InwardBookingQuantityUOM += PWBinSelection.BinSelectionReservationQuantity;
                    }

                    #region Outward part setup
                    item.ProdorderPartslistPosRelationID = nextMixureRelation.ProdOrderPartslistPosRelationID;
                    Guid? outwardFacilityID = null;
                    Guid? outwardFacilityChargeID = null;
                    if (sourceInfoType == ManualPreparationSourceInfoTypeEnum.FacilityID)
                        outwardFacilityID = item.ItemID;
                    if (sourceInfoType == ManualPreparationSourceInfoTypeEnum.FacilityChargeID)
                        outwardFacilityChargeID = item.ItemID;
                    FacilityBooking outwardBooking =
                        nextMixureRelation
                        .FacilityBooking_ProdOrderPartslistPosRelation
                        .Where(c =>
                            (outwardFacilityID != null && c.OutwardFacilityID == outwardFacilityID) ||
                            (outwardFacilityChargeID != null && c.OutwardFacilityChargeID == outwardFacilityChargeID))
                        .FirstOrDefault();

                    if (outwardBooking != null)
                    {
                        item.OutwardBookingNo = outwardBooking.FacilityBookingNo;
                        item.OutwardBookingQuantityUOM = outwardBooking.OutwardQuantity;
                    }
                    #endregion
                }
            }
            return items;
        }
        private List<DischargingItem> LoadDischargingItemListForFacility(ProdOrderPartslistPos intermedatePartPos)
        {
            return
                  intermedatePartPos
                  .FacilityBooking_ProdOrderPartslistPos
                  .Where(c => c.Comment != null && c.Comment.Contains(PWBinSelection.BinSelectionReservationComment))
                  .Select(c => new DischargingItem()
                  {
                      ItemID = c.InwardFacilityID ?? Guid.Empty,

                      LotNo = c.FacilityBookingCharge_FacilityBooking.Select(x => x.InwardFacilityCharge.FacilityLot?.LotNo).DefaultIfEmpty().FirstOrDefault(),
                      InwardFacilityNo = c.InwardFacility.FacilityNo,
                      InwardFacilityName = c.InwardFacility.FacilityName,

                      InwardBookingNo = c.FacilityBookingNo,
                      InwardBookingQuantityUOM = c.InwardQuantity,
                      ProdorderPartslistPosID = (c.ProdOrderPartslistPosID ?? Guid.Empty),
                      InwardBookingTime = c.InsertDate,
                      InwardFacilityChargeID = c.InwardFacilityChargeID
                  }).ToList();
        }

        private List<DischargingItem> LoadDischargingItemListForFacilityCharge(ProdOrderPartslistPos intermedatePartPos)
        {
            return
                  intermedatePartPos
                  .FacilityBooking_ProdOrderPartslistPos
                  .Where(c => c.Comment != null && c.Comment.Contains(PWBinSelection.BinSelectionReservationComment))
                  .Select(c => new DischargingItem()
                  {
                      ItemID = c.FacilityBookingCharge_FacilityBooking.Select(x => x.InwardFacilityCharge.FacilityChargeID).DefaultIfEmpty().FirstOrDefault(),

                      LotNo = c.FacilityBookingCharge_FacilityBooking.Select(x => x.InwardFacilityCharge.FacilityLot.LotNo).DefaultIfEmpty().FirstOrDefault(),
                      InwardFacilityNo = c.FacilityBookingCharge_FacilityBooking.Select(x => x.InwardFacilityCharge.FacilityLot.LotNo).DefaultIfEmpty().FirstOrDefault(),
                      InwardFacilityName = c.FacilityBookingCharge_FacilityBooking.Select(x => x.InwardFacilityCharge.FacilityChargeID.ToString()).DefaultIfEmpty().FirstOrDefault(),

                      InwardBookingNo = c.FacilityBookingNo,
                      InwardBookingQuantityUOM = c.InwardQuantity,
                      ProdorderPartslistPosID = (c.ProdOrderPartslistPosID ?? Guid.Empty),
                      InwardBookingTime = c.InsertDate,
                      InwardFacilityChargeID = c.InwardFacilityChargeID
                  }).ToList();
        }

        #endregion


    }
}
