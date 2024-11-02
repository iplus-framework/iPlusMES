// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="FacilityLotExpirationDateHelper.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
#if NETFRAMEWORK
using gip.mes.datamodel;
using gip.core.datamodel;
#endif

namespace gip.mes.facility
{
    /// <summary>
    /// Reine Hilfsliste auf der das Linq-Statement abgefeuert wird
    /// </summary>
#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityLotExpirationDateHelper'}de{'FacilityLotExpirationDateHelper'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + "FacilityLotExpirationDateHelper", "en{'FacilityLotExpirationDateHelper'}de{'FacilityLotExpirationDateHelper'}", typeof(FacilityLotExpirationDateHelper), "FacilityLotExpirationDateHelper", "ProductionDate", "ProductionDate")]
#endif
    public class FacilityLotExpirationDateHelper
    {
        /// <summary>
        /// Enum TimeSpanUnit
        /// </summary>
        public enum TimeSpanUnit : short
        {
            /// <summary>
            /// The seconds
            /// </summary>
            seconds = 0,
            /// <summary>
            /// The minutes
            /// </summary>
            minutes = 1,
            /// <summary>
            /// The hours
            /// </summary>
            hours = 2,
            /// <summary>
            /// The days
            /// </summary>
            days = 3,
            /// <summary>
            /// The weeks
            /// </summary>
            weeks = 4,
            /// <summary>
            /// The months
            /// </summary>
            months = 5,
            /// <summary>
            /// The years
            /// </summary>
            years = 6,
        }

        /// <summary>
        /// Enum TimeValidation
        /// </summary>
        public enum TimeValidation : short
        {
            /// <summary>
            /// The not initialized
            /// </summary>
            NotInitialized = -1,
            /// <summary>
            /// The difference is not valid
            /// </summary>
            DifferenceIsNotValid = 0,
            /// <summary>
            /// The difference is valid
            /// </summary>
            DifferenceIsValid = 1,
        }

        /// <summary>
        /// Enum ChangeResult
        /// </summary>
        public enum ChangeResult : short
        {
            /// <summary>
            /// The value not set error
            /// </summary>
            ValueNotSetError = -1,
            /// <summary>
            /// The value set dep val not changed
            /// </summary>
            ValueSetDepValNotChanged = 0,
            /// <summary>
            /// The value set dep val changed
            /// </summary>
            ValueSetDepValChanged = 1,
        }

        /// <summary>
        /// Gets or sets the facility lot expiration date helper ID.
        /// </summary>
        /// <value>The facility lot expiration date helper ID.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(9999)]
#endif
        public Guid FacilityLotExpirationDateHelperID { get; set; }

        /// <summary>
        /// The _ production date
        /// </summary>
        private DateTime _ProductionDate = new DateTime();
        /// <summary>
        /// The _ expiration date
        /// </summary>
        private DateTime _ExpirationDate = new DateTime();
        /// <summary>
        /// The _ storage life
        /// </summary>
        private TimeSpan _StorageLife = new TimeSpan();

        /// <summary>
        /// The const unchanged
        /// </summary>
        const short constUnchanged = 0;
        /// <summary>
        /// The const prod date
        /// </summary>
        const short constProdDate = 1;
        /// <summary>
        /// The const exp date
        /// </summary>
        const short constExpDate = 2;
        /// <summary>
        /// The const storage life
        /// </summary>
        const short constStorageLife = 3;
        /// <summary>
        /// The _ last value manipulated
        /// </summary>
        private short[] _LastValueManipulated = {constUnchanged,constUnchanged};

        /// <summary>
        /// Initializes a new instance of the <see cref="FacilityLotExpirationDateHelper"/> class.
        /// </summary>
        /// <param name="bSetNow">if set to <c>true</c> [b set now].</param>
        public FacilityLotExpirationDateHelper(bool bSetNow)
        {
            if (bSetNow)
            {
                _ProductionDate = DateTime.Now;
                _ExpirationDate = DateTime.Now;
                _StorageLife = new TimeSpan(0);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacilityLotExpirationDateHelper"/> class.
        /// </summary>
        /// <param name="productionDate">The production date.</param>
        /// <param name="expirationDate">The expiration date.</param>
        /// <param name="expTimeSpan">The exp time span.</param>
        /// <param name="storageLifeInUnits">The storage life in units.</param>
        public FacilityLotExpirationDateHelper(Nullable<DateTime> productionDate, Nullable<DateTime> expirationDate, TimeSpanUnit expTimeSpan, int storageLifeInUnits)
        {
            ResetProperties(productionDate, expirationDate, expTimeSpan, storageLifeInUnits);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacilityLotExpirationDateHelper"/> class.
        /// </summary>
        /// <param name="productionDate">The production date.</param>
        /// <param name="expirationDate">The expiration date.</param>
        /// <param name="expTimeSpan">The exp time span.</param>
        /// <param name="storageLifeInUnits">The storage life in units.</param>
        public FacilityLotExpirationDateHelper(string productionDate, string expirationDate, TimeSpanUnit expTimeSpan, int storageLifeInUnits)
        {
            ResetProperties(productionDate, expirationDate, expTimeSpan, storageLifeInUnits);
        }

        /// <summary>
        /// Resets the properties.
        /// </summary>
        public void ResetProperties()
        {
            if (_ProductionDate != DateTime.MinValue)
                _ProductionDate = new DateTime(0);
            if (_ExpirationDate != DateTime.MinValue)
                _ExpirationDate = new DateTime(0);
            if (_StorageLife != TimeSpan.MinValue)
                _StorageLife = new TimeSpan(0);
            _LastValueManipulated[0] = constUnchanged;
            _LastValueManipulated[1] = constUnchanged;
        }

        /// <summary>
        /// Resets the properties.
        /// </summary>
        /// <param name="productionDate">The production date.</param>
        /// <param name="expirationDate">The expiration date.</param>
        /// <param name="expTimeSpan">The exp time span.</param>
        /// <param name="storageLifeInUnits">The storage life in units.</param>
        public void ResetProperties(string productionDate, string expirationDate, TimeSpanUnit expTimeSpan, int storageLifeInUnits)
        {
            if (productionDate.Length > 0)
                _ProductionDate = DateTime.Parse(productionDate);
            if (expirationDate.Length > 0)
                _ExpirationDate = DateTime.Parse(expirationDate);
            SetStorageLifeInUnits(expTimeSpan, storageLifeInUnits);
            _LastValueManipulated[0] = constUnchanged;
            _LastValueManipulated[1] = constUnchanged;
        }

        /// <summary>
        /// Resets the properties.
        /// </summary>
        /// <param name="productionDate">The production date.</param>
        /// <param name="expirationDate">The expiration date.</param>
        /// <param name="expTimeSpan">The exp time span.</param>
        /// <param name="storageLifeInUnits">The storage life in units.</param>
        public void ResetProperties(Nullable<DateTime> productionDate, Nullable<DateTime> expirationDate, TimeSpanUnit expTimeSpan, int storageLifeInUnits)
        {
            if (productionDate.HasValue)
                _ProductionDate = productionDate.Value;
            else
                _ProductionDate = new DateTime(0);
            if (expirationDate.HasValue)
                _ExpirationDate = expirationDate.Value;
            else
                _ExpirationDate = new DateTime(0);
            SetStorageLifeInUnits(expTimeSpan, storageLifeInUnits);
            _LastValueManipulated[0] = constUnchanged;
            _LastValueManipulated[1] = constUnchanged;
        }

        // Checks if Time-Difference between ProductionDate and ExpirationDate is the StorageLife
        /// <summary>
        /// Ares the property values valid.
        /// </summary>
        /// <returns>TimeValidation.</returns>
        public TimeValidation ArePropertyValuesValid()
        {
            if ((_ProductionDate == DateTime.MinValue) || (_ExpirationDate == DateTime.MinValue))
                return TimeValidation.NotInitialized;
            TimeSpan timeDiff = _ExpirationDate.Subtract(_ProductionDate);
            if (_StorageLife.CompareTo(timeDiff) == 0)
                return TimeValidation.DifferenceIsValid;
            return TimeValidation.DifferenceIsNotValid;
        }

        /// <summary>
        /// Gets or sets the production date.
        /// </summary>
        /// <value>The production date.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(1)]
#endif
        public DateTime ProductionDate 
        {
            get { return _ProductionDate; }
            set { _ProductionDate = value; }
        }

        // Method which should be called, when user or program has changed the ProductionDate
        // Method returns 1 if other values have been changed otherwise 0, Method returns ValueNotSetError if nothing changed/error
        /// <summary>
        /// Called when [production date changed].
        /// </summary>
        /// <param name="newTime">The new time.</param>
        /// <returns>ChangeResult.</returns>
        public virtual ChangeResult OnProductionDateChanged(Nullable<DateTime> newTime)
        {
            if (!newTime.HasValue)
                return ChangeResult.ValueNotSetError;
            if (newTime.Value == DateTime.MinValue)
                return ChangeResult.ValueNotSetError;
            _ProductionDate = newTime.Value;
            ChangeResult bOtherValuesChanged = ChangeResult.ValueSetDepValNotChanged;

            // Recalc Storage Life
            if (   (  (_LastValueManipulated[0] == constExpDate)
                    ||((_LastValueManipulated[0] == constProdDate) && (_LastValueManipulated[1] == constExpDate)))
                && (newTime <= _ExpirationDate)
                && (_ExpirationDate.Ticks > 0))
            {
                bOtherValuesChanged = 0;
                TimeSpan newTimeSpan = _ExpirationDate - _ProductionDate;
                if (newTimeSpan != _StorageLife)
                {
                    _StorageLife = newTimeSpan;
                    bOtherValuesChanged = ChangeResult.ValueSetDepValChanged;
                }
            }
            // Recalc Expiration Date
            else
            {
                DateTime newExpDate = _ProductionDate + _StorageLife;
                if (newExpDate != _ExpirationDate)
                {
                    _ExpirationDate = newExpDate;
                    bOtherValuesChanged = ChangeResult.ValueSetDepValChanged;
                }
            }

            if (_LastValueManipulated[0] != constProdDate)
            {
                _LastValueManipulated[1] = _LastValueManipulated[0];
                _LastValueManipulated[0] = constProdDate;
            }
            return bOtherValuesChanged;
        }

        /// <summary>
        /// Called when [production date changed].
        /// </summary>
        /// <param name="newTime">The new time.</param>
        /// <returns>ChangeResult.</returns>
        public virtual ChangeResult OnProductionDateChanged(string newTime)
        {
            DateTime dtNewTime;
            if (DateTime.TryParse(newTime, out dtNewTime))
                return OnProductionDateChanged(dtNewTime);
            return ChangeResult.ValueNotSetError;
        }



        /// <summary>
        /// Gets or sets the expiration date.
        /// </summary>
        /// <value>The expiration date.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(2)]
#endif
        public DateTime ExpirationDate
        {
            get { return _ExpirationDate; }
            set { _ExpirationDate = value; }
        }

        /// <summary>
        /// Method which should be called, when user or program has changed the ProductionDate. Method returns ValueSetDepValChanged if other values have been changed otherwise ValueSetDepValNotChanged. Method returns ValueNotSetError if nothing changed/error.
        /// </summary>
        /// <param name="newTime">The new time.</param>
        /// <returns>ChangeResult.</returns>
        public virtual ChangeResult OnExpirationDateChanged(Nullable<DateTime> newTime)
        {
            if (!newTime.HasValue)
                return ChangeResult.ValueNotSetError;
            if (newTime.Value == DateTime.MinValue)
                return ChangeResult.ValueNotSetError;
            ChangeResult bOtherValuesChanged = ChangeResult.ValueSetDepValNotChanged;
            _ExpirationDate = newTime.Value;

            // Recalc Storage Life
            if (   (   (_LastValueManipulated[0] == constProdDate)
                    || ((_LastValueManipulated[0] == constExpDate) && (_LastValueManipulated[1] == constProdDate)))
                && (newTime >= _ProductionDate)
                && (_ProductionDate.Ticks > 0))
            {
                TimeSpan newTimeSpan = _ExpirationDate - _ProductionDate;
                if (newTimeSpan != _StorageLife)
                {
                    _StorageLife = newTimeSpan;
                    bOtherValuesChanged = ChangeResult.ValueSetDepValChanged;
                }
            }
            // Recalc Production Date
            else
            {
                DateTime newProdDate = _ExpirationDate - _StorageLife;
                if (newProdDate != _ProductionDate)
                {
                    _ProductionDate = newProdDate;
                    bOtherValuesChanged = ChangeResult.ValueSetDepValChanged;
                }
            }

            if (_LastValueManipulated[0] != constExpDate)
            {
                _LastValueManipulated[1] = _LastValueManipulated[0];
                _LastValueManipulated[0] = constExpDate;
            }
            return bOtherValuesChanged;
        }

        /// <summary>
        /// Called when [expiration date changed].
        /// </summary>
        /// <param name="newTime">The new time.</param>
        /// <returns>ChangeResult.</returns>
        public virtual ChangeResult OnExpirationDateChanged(string newTime)
        {
            DateTime dtNewTime;
            if (DateTime.TryParse(newTime, out dtNewTime))
                return OnExpirationDateChanged(dtNewTime);
            return ChangeResult.ValueNotSetError;
        }

        /// <summary>
        /// Gets or sets the storage life.
        /// </summary>
        /// <value>The storage life.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(3)]
#endif
        public TimeSpan StorageLife
        {
            get { return _StorageLife; }
            set { _StorageLife = value; }
        }

        /// <summary>
        /// Called when [storage life changed].
        /// </summary>
        /// <param name="ExpTimeSpan">The exp time span.</param>
        /// <param name="StorageLifeInUnits">The storage life in units.</param>
        /// <returns>ChangeResult.</returns>
        public virtual ChangeResult OnStorageLifeChanged(TimeSpanUnit ExpTimeSpan, int StorageLifeInUnits)
        {
            if ((_ProductionDate.Ticks <= 0) && (_ExpirationDate.Ticks <= 0))
                return ChangeResult.ValueNotSetError;

            ChangeResult bOtherValuesChanged = ChangeResult.ValueSetDepValNotChanged;
            _StorageLife = GetStorageLifeInTimeSpan(ExpTimeSpan, StorageLifeInUnits);

            // Recalc Expiration Date
            if (   ((_LastValueManipulated[0] == constProdDate)
                    || ((_LastValueManipulated[0] == constStorageLife) && (_LastValueManipulated[1] == constProdDate)))
                && (_ProductionDate.Ticks > 0))
            {
                DateTime newExpDate = _ProductionDate + _StorageLife;
                if (newExpDate != _ExpirationDate)
                {
                    _ExpirationDate = newExpDate;
                    bOtherValuesChanged = ChangeResult.ValueSetDepValChanged;
                }
            }
            // Recalc Production Date
            else if (_ExpirationDate.Ticks > 0)
            {
                DateTime newProdDate = _ExpirationDate - _StorageLife;
                if (newProdDate != _ProductionDate)
                {
                    _ProductionDate = newProdDate;
                    bOtherValuesChanged = ChangeResult.ValueSetDepValChanged;
                }
            }
            else if (_ProductionDate.Ticks > 0)
            {
                DateTime newExpDate = _ProductionDate + _StorageLife;
                if (newExpDate != _ExpirationDate)
                {
                    _ExpirationDate = newExpDate;
                    bOtherValuesChanged = ChangeResult.ValueSetDepValChanged;
                }
            }

            if (_LastValueManipulated[0] != constStorageLife)
            {
                _LastValueManipulated[1] = _LastValueManipulated[0];
                _LastValueManipulated[0] = constStorageLife;
            }
            return bOtherValuesChanged;
        }

        /// <summary>
        /// Gets the storage life in time span.
        /// </summary>
        /// <param name="ExpTimeSpan">The exp time span.</param>
        /// <param name="StorageLifeInUnits">The storage life in units.</param>
        /// <returns>TimeSpan.</returns>
        private TimeSpan GetStorageLifeInTimeSpan(TimeSpanUnit ExpTimeSpan, int StorageLifeInUnits)
        {
            if (StorageLifeInUnits > 0)
            {
                switch (ExpTimeSpan)
                {
                    case TimeSpanUnit.seconds:
                        return new TimeSpan(0, 0, StorageLifeInUnits);
                    case TimeSpanUnit.minutes:
                        return new TimeSpan(0, StorageLifeInUnits, 0);
                    case TimeSpanUnit.hours:
                        return new TimeSpan(StorageLifeInUnits, 0, 0);
                    case TimeSpanUnit.days:
                        return new TimeSpan(StorageLifeInUnits, 0, 0, 0);
                    case TimeSpanUnit.weeks:
                        return new TimeSpan((StorageLifeInUnits * 7), 0, 0, 0);
                    case TimeSpanUnit.months:
                        return new TimeSpan((StorageLifeInUnits * 30), 0, 0, 0);
                    case TimeSpanUnit.years:
                        return new TimeSpan((StorageLifeInUnits * 365), 0, 0, 0);
                    default:
                        return new TimeSpan(StorageLifeInUnits, 0, 0, 0);
                }
            }
            return new TimeSpan(0);
        }

        /// <summary>
        /// Sets the storage life in units.
        /// </summary>
        /// <param name="ExpTimeSpan">The exp time span.</param>
        /// <param name="StorageLifeInUnits">The storage life in units.</param>
        public void SetStorageLifeInUnits(TimeSpanUnit ExpTimeSpan, int StorageLifeInUnits)
        {
            _StorageLife = GetStorageLifeInTimeSpan(ExpTimeSpan, StorageLifeInUnits);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is expired.
        /// </summary>
        /// <value><c>true</c> if this instance is expired; otherwise, <c>false</c>.</value>
        public bool IsExpired
        {
            get
            {
                if (_ExpirationDate.Ticks > 0)
                {
                    if ((_ExpirationDate - DateTime.Now).Days < 0)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the expiration time span.
        /// </summary>
        /// <value>The expiration time span.</value>
        public TimeSpan ExpirationTimeSpan
        {
            get
            {
                if (_ExpirationDate != DateTime.MinValue)
                    return (_ExpirationDate - DateTime.Now);
                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Gets the expiration time span days.
        /// </summary>
        /// <value>The expiration time span days.</value>
        public int ExpirationTimeSpanDays
        {
            get
            {
                return GetExpirationTimeSpanInUnits(TimeSpanUnit.days);
            }
        }

        /// <summary>
        /// Gets the expiration time span in units.
        /// </summary>
        /// <param name="ExpTimeSpan">The exp time span.</param>
        /// <returns>System.Int32.</returns>
        public int GetExpirationTimeSpanInUnits(TimeSpanUnit ExpTimeSpan)
        {
            switch (ExpTimeSpan)
            {
                case TimeSpanUnit.seconds:
                    return ExpirationTimeSpan.Seconds;
                case TimeSpanUnit.minutes:
                    return ExpirationTimeSpan.Minutes;
                case TimeSpanUnit.hours:
                    return ExpirationTimeSpan.Hours;
                case TimeSpanUnit.days:
                    return ExpirationTimeSpan.Days;
                case TimeSpanUnit.weeks:
                    return (int)(ExpirationTimeSpan.Days / 7);
                case TimeSpanUnit.months:
                    return (int)(ExpirationTimeSpan.Days / 30);
                case TimeSpanUnit.years:
                    return (int)(ExpirationTimeSpan.Days / 365);
                default:
                    return ExpirationTimeSpan.Days;
            }
            
        }

        /*public System.Windows.Media.Brush IsExpired
        {
            get
            {
                if (_ExpirationDate.Ticks > 0)
                {
                    if ((_ExpirationDate - DateTime.Now).Days < 0)
                        return (System.Windows.Media.Brush) new SolidColorBrush(System.Windows.Media.Color.FromRgb(255,0,0));
                }
                return (System.Windows.Media.Brush) new SolidColorBrush(System.Windows.Media.Color.FromRgb(0,255,0));
            }
        }*/

        /// <summary>
        /// Gets the key AC identifier.
        /// </summary>
        /// <value>The key AC identifier.</value>
        static public string KeyACIdentifier
        {
            get
            {
                return "FacilityLotExpirationDateHelperID";
            }
        }
    }
}