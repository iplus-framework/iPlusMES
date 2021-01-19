using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'DischargingItemNoValidator'}de{'DischargingItemNoValidator'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, true)]
    public class DischargingItemNoValidator : IACObject
    {

        #region DI
        public IACComponent ACComponent { get; private set; }

        public string ClassName { get; private set; }
        #endregion

        #region ctor's

        public DischargingItemNoValidator(IACComponent aCComponent, string className)
        {
            ACComponent = aCComponent;
            ClassName = className;
        }

        #endregion

        #region Methods

        #region PAFBINSelection  -> Validate Input

        public Msg ValidateInputNo(string itemNo, Guid intermediateChildPosID, Guid? prodOrderPartslistPosRelation, ManualPreparationSourceInfoTypeEnum sourceInfoType, DischargingItemNoValidatorBehaviorEnum behavior)
        {
            Msg msg = null;
            Guid itemNoID = Guid.Empty;
            Guid.TryParse(itemNo, out itemNoID);
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                ProdOrderPartslistPos intermediatePartPos = databaseApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == intermediateChildPosID);
                ProdOrderPartslistPosRelation relation = null;
                if (prodOrderPartslistPosRelation != null)
                {
                    relation = databaseApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == prodOrderPartslistPosRelation);
                }

                if (itemNoID != Guid.Empty)
                {
                    if (sourceInfoType == ManualPreparationSourceInfoTypeEnum.FacilityID)
                    {
                        msg = ValidateFacility(databaseApp, intermediatePartPos, relation, itemNoID, behavior);
                    }
                    else if (sourceInfoType == ManualPreparationSourceInfoTypeEnum.FacilityChargeID)
                    {

                        msg = ValidateFacilityCharge(databaseApp, intermediatePartPos, relation, itemNoID, behavior);
                    }
                }
                else
                    msg = FactoryErrorMessage(DischargingItemValidationErrors.NotValidGUID, itemNo);
            }
            return msg;
        }

        #endregion

        #region Validate Facility

        private Msg ValidateFacility(DatabaseApp databaseApp, ProdOrderPartslistPos intermediatePartPos, ProdOrderPartslistPosRelation relation, Guid itemNoID, DischargingItemNoValidatorBehaviorEnum behavior)
        {
            Msg msg = null;
            Facility facility = databaseApp.Facility.FirstOrDefault(c => c.FacilityID == itemNoID);
            if (facility != null)
            {
                if (facility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.PreparationBin)
                {
                    msg = CheckFacilityInwardBooking(intermediatePartPos, facility, behavior);
                    if (msg == null)
                    {
                        if (behavior == DischargingItemNoValidatorBehaviorEnum.BINDischarging_NoOutwardBookings)
                        {
                            msg = CheckFacilityOutwardBooking(intermediatePartPos, relation, facility);
                        }
                        if (msg == null)
                            msg = FactoryInfoMessage(DischargingItemValidationInfos.BinFacilityFounded, facility.FacilityNo);
                    }
                }
                else
                    msg = FactoryErrorMessage(DischargingItemValidationErrors.FacilityNotBinTypePreparation, facility.FacilityNo, MDFacilityType.FacilityTypes.PreparationBin);
            }
            else
                // missing facilit
                msg = FactoryErrorMessage(DischargingItemValidationErrors.Facility_NotFound, itemNoID);
            return msg;
        }

        private Msg CheckFacilityInwardBooking(ProdOrderPartslistPos intermediatePartPos, Facility facility, DischargingItemNoValidatorBehaviorEnum behavior)
        {
            Msg msg = null;

            FacilityBooking inwardReservationBooking = intermediatePartPos.FacilityBooking_ProdOrderPartslistPos.FirstOrDefault(c => c.InwardFacilityID == facility.FacilityID && c.InwardQuantity == PWBinSelection.BinSelectionReservationQuantity);

            switch (behavior)
            {
                case DischargingItemNoValidatorBehaviorEnum.BINSelection_NoInwardBookings:
                    if (inwardReservationBooking != null)
                    {
                        msg = FactoryErrorMessage(DischargingItemValidationErrors.BinSelection_Facility_ReservationBookingExist, inwardReservationBooking.FacilityBookingNo, facility.FacilityNo);
                    }
                    else
                    {
                        string batchNo = null;

                        FacilityBooking fbInward = facility.FacilityBooking_InwardFacility.OrderByDescending(c => c.InsertDate).FirstOrDefault();
                        if(fbInward != null)
                        {
                            FacilityBooking fbOutward = facility.FacilityBooking_OutwardFacility.OrderByDescending(c => c.InsertDate).FirstOrDefault();
                            if(fbOutward == null || fbOutward.InsertDate < fbInward.InsertDate)
                            {
                                batchNo = fbInward.ProdOrderPartslistPos.ProdOrderBatch.ProdOrderBatchNo;
                            }
                        }

                        // Search batch were exist facility reservation but is not discharged!
                            //batchNo =
                            //intermediatePartPos
                            //.ProdOrderPartslistPos1_ParentProdOrderPartslistPos
                            //.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                            //.Where(c =>
                            //        c.ProdOrderPartslistPosID != intermediatePartPos.ProdOrderPartslistPosID
                            //        && c.ProdOrderBatch != null

                            //        && c.FacilityBooking_ProdOrderPartslistPos.Any(x => x.InwardFacilityID == facility.FacilityID && x.InwardQuantity == PWBinSelection.BinSelectionReservationQuantity)
                            //        && !c.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                            //            .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                            //            .Where(x => x.OutwardFacilityID == facility.FacilityID)
                            //            .Any()
                            //    )
                            //.Select(c => c.ProdOrderBatch.ProdOrderBatchNo)
                            //.DefaultIfEmpty()
                            //.FirstOrDefault();

                        if (!string.IsNullOrEmpty(batchNo))
                        {
                            string reservationFacilityBookingNo = intermediatePartPos
                            .ProdOrderPartslistPos1_ParentProdOrderPartslistPos
                            .ProdOrderPartslistPos_ParentProdOrderPartslistPos
                            .Where(c => c.ProdOrderBatch.ProdOrderBatchNo == batchNo)
                            .SelectMany(c => c.FacilityBooking_ProdOrderPartslistPos)
                            .Where(c => c.InwardQuantity == PWBinSelection.BinSelectionReservationQuantity)
                            .Select(c => c.FacilityBookingNo)
                            .DefaultIfEmpty()
                            .FirstOrDefault();

                            msg = FactoryErrorMessage(DischargingItemValidationErrors.BinSelection_Facility_ReservedButNotDiscarged, facility.FacilityNo, reservationFacilityBookingNo, batchNo);
                        }
                    }
                    break;
                case DischargingItemNoValidatorBehaviorEnum.BINDischarging_NoOutwardBookings:
                    if (inwardReservationBooking == null)
                    {
                        msg = FactoryErrorMessage(DischargingItemValidationErrors.BinSelection_Facility_ReservationBookingMissing, facility.FacilityNo);
                    }
                    break;
                default:
                    break;
            }

            return msg;
        }

        private Msg CheckFacilityOutwardBooking(ProdOrderPartslistPos intermediatePartPos, ProdOrderPartslistPosRelation relation, Facility facility)
        {
            FacilityBooking outwardBooking = relation.FacilityBooking_ProdOrderPartslistPosRelation.FirstOrDefault(c => c.OutwardFacilityID == facility.FacilityID);
            if (outwardBooking != null)
                return FactoryErrorMessage(DischargingItemValidationErrors.BinDischarging_Facility_OutwardBookingExist, outwardBooking.FacilityBookingNo, facility.FacilityNo);
            return null;
        }

        #endregion

        #region Validate FacilityCharge

        private Msg ValidateFacilityCharge(DatabaseApp databaseApp, ProdOrderPartslistPos intermediatePartPos, ProdOrderPartslistPosRelation relation, Guid itemNoID, DischargingItemNoValidatorBehaviorEnum behavior)
        {
            Msg msg = null;
            FacilityCharge facilityCharge = databaseApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == itemNoID);
            if (facilityCharge != null)
            {
                msg = CheckFacilityChargeInwardBooking(intermediatePartPos, facilityCharge, behavior);
                if (msg == null)
                {
                    if (behavior == DischargingItemNoValidatorBehaviorEnum.BINDischarging_NoOutwardBookings)
                    {
                        msg = CheckFacilityChargeOutwardBooking(intermediatePartPos, relation, facilityCharge);
                    }
                }
                if (msg == null)
                    msg = FactoryInfoMessage(DischargingItemValidationInfos.BinFacilityChargeFounded, itemNoID);
            }
            else
                msg = FactoryInfoMessage(DischargingItemValidationInfos.BinFacilityChargeNew, itemNoID);
            return msg;
        }

        private Msg CheckFacilityChargeInwardBooking(ProdOrderPartslistPos intermediatePartPos, FacilityCharge facilityCharge, DischargingItemNoValidatorBehaviorEnum behavior)
        {
            Msg msg = null;

            FacilityBooking inwardReservationBooking = intermediatePartPos
                .FacilityBooking_ProdOrderPartslistPos
                .FirstOrDefault(c => 
                c.FacilityBookingCharge_FacilityBooking.Any(x=>x.InwardFacilityChargeID == facilityCharge.FacilityChargeID)
                && c.Comment != null
                && c.Comment.Contains(PWBinSelection.BinSelectionReservationComment));

            switch (behavior)
            {
                case DischargingItemNoValidatorBehaviorEnum.BINSelection_NoInwardBookings:
                    if (inwardReservationBooking != null)
                    {
                        msg = FactoryErrorMessage(DischargingItemValidationErrors.BinSelection_FacilityCharge_ReservationBookingExist, inwardReservationBooking.FacilityBookingNo, facilityCharge.FacilityChargeID);
                    }
                    else
                    {
                        // Search batch were exist facility reservation but is not discharged!
                        string batchNo =
                            intermediatePartPos
                            .ProdOrderPartslistPos1_ParentProdOrderPartslistPos
                            .ProdOrderPartslistPos_ParentProdOrderPartslistPos
                            .Where(c =>
                                    c.ProdOrderPartslistPosID == intermediatePartPos.ProdOrderPartslistPosID
                                    && c.ProdOrderBatch != null

                                    && c.FacilityBooking_ProdOrderPartslistPos.Any(x => x.InwardFacilityChargeID == facilityCharge.FacilityChargeID && x.InwardQuantity == PWBinSelection.BinSelectionReservationQuantity)
                                    && !c.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                                        .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                                        .Where(x => x.OutwardFacilityChargeID == facilityCharge.FacilityChargeID)
                                        .Any()
                                )
                            .Select(c => c.ProdOrderBatch.ProdOrderBatchNo)
                            .DefaultIfEmpty()
                            .FirstOrDefault();

                        if (!string.IsNullOrEmpty(batchNo))
                        {
                            string reservationFacilityBookingNo = intermediatePartPos
                            .ProdOrderPartslistPos1_ParentProdOrderPartslistPos
                            .ProdOrderPartslistPos_ParentProdOrderPartslistPos
                            .Where(c => c.ProdOrderBatch.ProdOrderBatchNo == batchNo)
                            .SelectMany(c => c.FacilityBooking_ProdOrderPartslistPos)
                            .Where(c => c.InwardQuantity == PWBinSelection.BinSelectionReservationQuantity)
                            .Select(c => c.FacilityBookingNo)
                            .DefaultIfEmpty()
                            .FirstOrDefault();

                            msg = FactoryErrorMessage(DischargingItemValidationErrors.BinSelection_FacilityCharge_ReservedButNotDiscarged, facilityCharge.FacilityChargeID, reservationFacilityBookingNo, batchNo);
                        }
                    }
                    break;
                case DischargingItemNoValidatorBehaviorEnum.BINDischarging_NoOutwardBookings:
                    if (inwardReservationBooking == null)
                    {
                        msg = FactoryErrorMessage(DischargingItemValidationErrors.BinSelection_FacilityCharge_ReservationBookingMissing, facilityCharge.FacilityChargeID);
                    }
                    break;
                default:
                    break;
            }

            return msg;
        }

        private Msg CheckFacilityChargeOutwardBooking(ProdOrderPartslistPos intermediatePartPos, ProdOrderPartslistPosRelation relation, FacilityCharge facilityCharge)
        {
            Msg msg = null;
            FacilityBooking outwardBooking = relation.FacilityBooking_ProdOrderPartslistPosRelation.FirstOrDefault(c => c.OutwardFacilityChargeID == facilityCharge.FacilityChargeID);
            if (outwardBooking != null)
                return FactoryErrorMessage(DischargingItemValidationErrors.BinDischarging_FacilityCharge_OutwardBookingExist, outwardBooking.FacilityBookingNo, facilityCharge.FacilityChargeID);
            return msg;
        }


        #endregion

        #endregion

        #region IACObject

        #region IACObject -> Properties

        public IACObject ParentACObject
        {
            get
            {
                return null;
            }
        }

        public IACType ACType
        {
            get
            {
                return this.ReflectACType();
            }
        }

        public IEnumerable<IACObject> ACContentList
        {
            get
            {
                return null;
            }
        }

        public string ACIdentifier
        {
            get
            {
                return "DischargingItemNoValidator";
            }
        }

        public string ACCaption
        {
            get
            {
                return ACIdentifier;
            }
        }

        #endregion

        #region IACObject -> Methods

        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return null;
        }

        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return false;
        }

        public string GetACUrl(IACObject rootACObject = null)
        {
            return null;
        }

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return false;
        }

        #endregion

        #endregion

        #region Message Factory

        public Msg FactoryErrorMessage(DischargingItemValidationErrors error, params object[] parameters)
        {
            string errorID = "Error" + ((int)error).ToString();
            return new Msg(ACComponent, eMsgLevel.Error, ClassName, error.ToString(), 0, errorID, parameters);
        }

        public Msg FactoryInfoMessage(DischargingItemValidationInfos info, params object[] parameters)
        {
            string infoID = "Info" + ((int)info).ToString();
            return new Msg(ACComponent, eMsgLevel.Info, ClassName, info.ToString(), 0, infoID, parameters);
        }
        #endregion
    }


    public enum DischargingItemValidationErrors
    {
        NotValidGUID = 50379,
        FacilityNotBinTypePreparation = 50380,
        Facility_NotFound = 50381,
        BinSelection_Facility_ReservationBookingExist = 50383,
        BinSelection_FacilityCharge_ReservationBookingExist = 50384,
        BinSelection_Facility_ReservationBookingMissing = 50385,
        BinSelection_FacilityCharge_ReservationBookingMissing = 50386,
        BinSelection_Facility_ReservedButNotDiscarged = 50387,
        BinSelection_FacilityCharge_ReservedButNotDiscarged = 50388,
        BinDischarging_Facility_OutwardBookingExist = 50389,
        BinDischarging_FacilityCharge_OutwardBookingExist = 50390
    }

    public enum DischargingItemValidationInfos
    {
        BinFacilityFounded = 50059,
        BinFacilityChargeFounded = 50060,
        BinFacilityChargeNew = 50061
    }
}