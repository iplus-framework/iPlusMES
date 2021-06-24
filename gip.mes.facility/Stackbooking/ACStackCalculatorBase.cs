using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Stackcalculator base'}de{'Stackcalculator base'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, false, false)]
    public abstract class ACStackCalculatorBase : ACComponent
    {
        public const string ClassName = "ACStackCalculatorBase";
        #region c´tors
        public ACStackCalculatorBase(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion

        #region Enum
        public enum SortOrderFIFOEnum : short
        {
            ByFacilityChargeSortNo = 0,
            ByFirstFillingDate = 1,
            ByExpirationDate = 2
        }
        #endregion


        #region CalculateInOut
        /// <summary>
        /// If isOutwardBooking (isInwardBooking is false) and passed booking-values are negative: Stock should be increased
        /// If isInwardBooking and passed booking-values are negative: Stock should be decreased
        /// If Relocation, then this method will be called first on Source-Facility with paramater "isOutwardBooking"
        ///                afterwards, method CalculateRelocation() will be called from the framework
        /// The passed List of facilityCharges is different if Facility is a Cell/Silo or a normal Storage area
        ///     if Cell/Silo: All facilityCharges in the cell are included in the list
        ///     if normal Storage area: the list is reduced depending on the parameters FacilityLot or Material are set
        /// The method returns a integer-casted value of FacilityBookingManager.BookingResult 
        /// The result of the calculation can be retrieved from the framework over the StackItemList of property StackItemListInOut
        /// </summary>
        public abstract Global.ACMethodResultState CalculateInOut(bool isInwardBooking, // false: Outward, true: Inward
                                 bool shiftBookingReverse, // false: normal, true: reverse booking
                                 bool negativeStockAllowed,
                                 Double quantityUOM, MDUnit mdUnitUOM,
                                 IEnumerable<FacilityCharge> facilityCharges,
                                 ACMethodBooking BP,
                                 out StackItemList stackItemListInOut,
                                 out MsgBooking msgBookingResult,
                                 bool isRetrogradePosting = false);
        #endregion


        #region Relocation
        /// <summary>
        /// Only if Relocation:
        /// facilityChargesSource is IStackItemList of Facility-Charges which was polpulated with call Calculate() before
        /// facilityChargesTarget is List of Facility-Charges on the Destination-Facility
        /// The List is different if Facility is a Cell/Silo or a normal Storage area
        ///     if Cell/Silo: All facilityCharges in the cell are included in the list
        ///     if normal Storage area: the list is reduced depending on the parameters FacilityLot or Material are set
        /// The method returns a integer-casted value of FacilityBookingManager.BookingResult 
        /// The result of the calculation can be retrieved from the framework over the StackItemList of property StackItemListRelocation
        /// </summary>
        public abstract Global.ACMethodResultState CalculateRelocation(bool shiftBookingReverse, // false: normal, true: reverse booking
                                bool negativeStockAllowed,
                                StackItemList facilityChargesSource,
                                IEnumerable<FacilityCharge> facilityChargesTarget,
                                ACMethodBooking BP,
                                out StackItemList stackItemListRelocation,
                                out MsgBooking msgBookingResult);
        #endregion


        #region ReOrganize
        /// <summary>
        /// If framework has inserted a new FacilityCharge on a Facility, afterwards the framework will call
        /// this method. This method Reorganizes the FacilityCharges.
        /// The method returns a integer-casted value of FacilityBookingManager.BookingResult
        /// The result of the calculation can be retrieved from the framework over the StackItemList of property StackItemListReOrganize
        /// </summary>
        public abstract Global.ACMethodResultState ReOrganize(IEnumerable<FacilityCharge> facilityCharges, out StackItemList stackItemListReOrganize, out MsgBooking msgBookingResult);
        #endregion
    }
}
