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
                    // en{'The value {0} is not valid GUID!'}de{'Der Wert {0} ist keine gültige GUID!'}
                    msg = new Msg(ACComponent, eMsgLevel.Error, ClassName, nameof(ValidateInputNo), 0, "Error50379", itemNo); 
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
                if (facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.PreparationBin)
                {
                    msg = CheckFacilityInwardBooking(intermediatePartPos, facility, behavior);
                    if (msg == null)
                    {
                        if (behavior == DischargingItemNoValidatorBehaviorEnum.BINDischarging_NoOutwardBookings)
                        {
                            msg = CheckFacilityOutwardBooking(intermediatePartPos, relation, facility);
                        }
                        if (msg == null)
                            // en{'Facility {0} finded!'}de{'Lagerplatz {0} gefunden!'}
                            msg = new Msg(ACComponent, eMsgLevel.Info, ClassName, nameof(ValidateFacility), 0, "Info50059", facility.FacilityNo);
                    }
                }
                else
                    // en{'Facility {0} is not type {1}!'}de{'Lagerplatz {0} ist nicht Typ {1}!'}
                    msg = new Msg(ACComponent, eMsgLevel.Error, ClassName, nameof(ValidateFacility), 0, "Error50380", facility.FacilityNo, FacilityTypesEnum.PreparationBin); 
            }
            else
                // missing facilit
                //en{'Facility with ID: {0} not found!'}de{'Lagerort mit ID: {0} nicht gefunden!'}
                msg = new Msg(ACComponent, eMsgLevel.Error, ClassName, nameof(ValidateInputNo), 0, "Error50381", itemNoID);
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
                        // en{'Reservation Inward Booking {0} for Facility {1} exist!'}de{'Geplante Ergebnis Buchung {0} für Lagerplatz {1} vorhanden!'}
                        msg = new Msg(ACComponent, eMsgLevel.Error, ClassName, nameof(CheckFacilityInwardBooking), 0, "Error50383", inwardReservationBooking.FacilityBookingNo, facility.FacilityNo); 
                    }
                    else
                    {
                        string batchNo = null;

                        FacilityBooking fbInward = facility.FacilityBooking_InwardFacility.OrderByDescending(c => c.InsertDate).FirstOrDefault();
                        if (fbInward != null)
                        {
                            FacilityBooking fbOutward = facility.FacilityBooking_OutwardFacility.OrderByDescending(c => c.InsertDate).FirstOrDefault();
                            if (fbOutward == null || fbOutward.InsertDate < fbInward.InsertDate)
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
                            // en{'Facility {0} already reserved with Booking {1} in Batch {2} but not discarged!'}de{'Lagerplatz {0} bereits mit Buchung {1} in Batch {2} reserviert, aber nicht entladen!'}
                            msg = new Msg(ACComponent, eMsgLevel.Error, ClassName, nameof(CheckFacilityInwardBooking), 0, "Error50387", facility.FacilityNo, reservationFacilityBookingNo, batchNo);
                        }
                    }
                    break;
                case DischargingItemNoValidatorBehaviorEnum.BINDischarging_NoOutwardBookings:
                    if (inwardReservationBooking == null)
                    {
                        // en{'Reservation Inward Booking for Facility {0} not exist!'}de{'Geplante Ergebnis Buchung für Lagerpatz {0} nicht existieren!'}
                        msg = new Msg(ACComponent, eMsgLevel.Error, ClassName, nameof(CheckFacilityInwardBooking), 0, "Error50385", facility.FacilityNo);
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
                // en{'Outward Booking {0} allready exist for Facility: {1}!'}de{'Einsatzbuchung {0} für Lagerplatz {1} existiert!'}
                return new Msg(ACComponent, eMsgLevel.Error, ClassName, nameof(CheckFacilityOutwardBooking), 0, "Error50389", outwardBooking.FacilityBookingNo, facility.FacilityNo);
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
                    // en{'Charge {0} finded!'}de{'Charge {0} gefunden!'}
                    msg = new Msg(ACComponent, eMsgLevel.Info, ClassName, nameof(ValidateFacilityCharge), 0, "Info50060", itemNoID);
            }
            else
                // en{'New barcode {0}!'}de{'Neu Barkode {0}!'}
                msg = new Msg(ACComponent, eMsgLevel.Info, ClassName, nameof(ValidateFacilityCharge), 0, "Info50061", itemNoID);
            return msg;
        }

        private Msg CheckFacilityChargeInwardBooking(ProdOrderPartslistPos intermediatePartPos, FacilityCharge facilityCharge, DischargingItemNoValidatorBehaviorEnum behavior)
        {
            Msg msg = null;

            FacilityBooking inwardReservationBooking = intermediatePartPos
                .FacilityBooking_ProdOrderPartslistPos
                .FirstOrDefault(c =>
                c.FacilityBookingCharge_FacilityBooking.Any(x => x.InwardFacilityChargeID == facilityCharge.FacilityChargeID)
                && c.Comment != null
                && c.Comment.Contains(PWBinSelection.BinSelectionReservationComment));

            switch (behavior)
            {
                case DischargingItemNoValidatorBehaviorEnum.BINSelection_NoInwardBookings:
                    if (inwardReservationBooking != null)
                    {
                        // en{'Reservation Inward Booking {0} for Charge {1} exist!'}de{'Geplante Ergebnis Buchung {0} für Charge {1} vorhanden!'}
                        msg = new Msg(ACComponent, eMsgLevel.Error, ClassName, nameof(CheckFacilityChargeInwardBooking), 0, "Error50384", inwardReservationBooking.FacilityBookingNo, facilityCharge.FacilityChargeID);
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

                            // en{'Charge {0} already reserved with Booking {1} in Batch {2} but not discarged!'}de{'Charge {0} bereits mit Buchung {1} in Batch {2} reserviert, aber nicht entladen!'}
                            msg = new Msg(ACComponent, eMsgLevel.Error, ClassName, nameof(CheckFacilityChargeInwardBooking), 0, "Error50388", facilityCharge.FacilityChargeID, reservationFacilityBookingNo, batchNo);
                        }
                    }
                    break;
                case DischargingItemNoValidatorBehaviorEnum.BINDischarging_NoOutwardBookings:
                    if (inwardReservationBooking == null)
                    {
                        // en{'Reservation Inward Booking for Charge {0} not exist!'}de{'Geplante Ergebnis Buchung für Charge {0} nicht existieren!'}
                        msg = new Msg(ACComponent, eMsgLevel.Error, ClassName, nameof(CheckFacilityChargeInwardBooking), 0, "Error50386", facilityCharge.FacilityChargeID);
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
                // en{'Outward Booking {0} allready exist for Charge: {1}!'}de{'Einsatzbuchung {0} für Charge {1} existiert!'}
                return new Msg(ACComponent, eMsgLevel.Error, ClassName, nameof(CheckFacilityChargeOutwardBooking), 0, "Error50390", outwardBooking.FacilityBookingNo, facilityCharge.FacilityChargeID);
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

        public bool ACUrlTypeInfo(string acUrl, ref ACUrlTypeInfo acUrlTypeInfo)
        {
            return this.ReflectACUrlTypeInfo(acUrl, ref acUrlTypeInfo);
        }

        #endregion

        #endregion

        #region Message Factory

        public Msg FactoryErrorMessage(DischargingItemValidationErrors error, params object[] parameters)
        {
            string errorID = "Error" + ((int)error).ToString();
            return new Msg(ACComponent, eMsgLevel.Error, ClassName, error.ToString(), 0, errorID, parameters);
        }


        #endregion
    }


    public enum DischargingItemValidationErrors
    {
        BinDischarging_FacilityCharge_OutwardBookingExist = 50390
    }


}