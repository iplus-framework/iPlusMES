using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    public class StackItem
    {
        public StackItem(FacilityCharge facilityCharge, Double quantity, Double quantityUOM, bool cloneOnDest)
        {
            _FacilityCharge = facilityCharge;
            _Quantity = quantity;
            _QuantityUOM = quantityUOM;
            _CloneOnDest = cloneOnDest;
        }

        FacilityCharge _FacilityCharge;
        Double _Quantity = 0;
        Double _QuantityUOM = 0;
        bool _CloneOnDest = false;

        public FacilityCharge FacilityCharge
        {
            get
            {
                return _FacilityCharge;
            }
        }

        /// <summary>
        /// If false, then property FacilityCharge can be directly manipulated
        /// If true, then property FacilityCharge is from Source-Facility and must be cloned on the Destination-Facility
        /// This means the FacilityBookingManager must generate a new FacilityCharge (Cell-Layer) on the Destination-Facility
        /// </summary>
        public bool CloneOnDest
        {
            get
            {
                return _CloneOnDest;
            }
        }

        /// <summary>
        /// Quantity is measured in QuantityUnits of FacilityCharge
        /// and can be positive or negative
        /// If Outward and value negative: Stock will be increased
        /// If Inward and value negative: Stock will be decreased
        /// </summary>
        public Double Quantity
        {
            get
            {
                return _Quantity;
            }
        }

        public Double QuantityUOM
        {
            get
            {
                return _Quantity;
            }
        }
    }
}
