// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'POFacilityOEEModel'}de{'POFacilityOEEModel'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable)]
    public class FacilityOEEAvg : EntityBase, IACObject
    {
        #region c'tors
        public FacilityOEEAvg()
        {
        }

        public FacilityOEEAvg(IEnumerable<FacilityOEEAvg> details)
        {
            StartDate = DateTime.MaxValue;
            EndDate = DateTime.MinValue;
            Details = details;
            int i = 0;  
            if (details != null && details.Any())
            {
                double sumAvailability = 0.0;
                double sumPerformance = 0.0;
                double sumQuality = 0.0;
                double sumTotal = 0.0;
                foreach (var item in details)
                {
                    sumAvailability += item.AvailabilityOEE;
                    sumPerformance += item.PerformanceOEE;
                    sumQuality += item.QualityOEE;
                    sumTotal += item.TotalOEE;
                    if (item.StartDate < StartDate)
                        StartDate= item.StartDate;
                    if (item.EndDate > EndDate)
                        EndDate = item.EndDate;
                    i++;
                }
                AvailabilityOEE = sumAvailability / i;
                PerformanceOEE = sumPerformance / i;
                QualityOEE = sumQuality / i;
                TotalOEE = sumTotal / i;
            }
        }
        #endregion

        #region Properties
        [ACPropertyInfo(1, "", "en{'Machine'}de{'Maschine'}")]
        public Facility Facility { get; set; }

        public double AvailabilityOEE { get; set; }

        [ACPropertyInfo(110, "", "en{'Availability [%]'}de{'Verfügbarkeit [%]'}")]
        public double AvailabilityOEEPer { get { return AvailabilityOEE * 100; } }

        public double PerformanceOEE { get; set; }

        [ACPropertyInfo(111, "", "en{'Performance [%]'}de{'Leistung [%]'}")]
        public double PerformanceOEEPer { get { return PerformanceOEE * 100; } }

        public double QualityOEE { get; set; }

        [ACPropertyInfo(112, "", "en{'Quality [%]'}de{'Qualität [%]'}")]
        public double QualityOEEPer { get { return QualityOEE * 100; } }

        public double TotalOEE { get; set; }

        [ACPropertyInfo(113, "", "en{'OEE [%]'}de{'OEE [%]'}")]
        public double TotalOEEPer { get { return TotalOEE * 100; } }

        [ACPropertyInfo(114, "", "en{'From'}de{'Von'}")]
        public DateTime StartDate { get; set; }

        [ACPropertyInfo(115, "", "en{'To'}de{'Bis'}")]
        public DateTime EndDate { get; set; }

        public IEnumerable<FacilityOEEAvg> Details { get; private set; }

        [ACPropertyInfo(210, "", "en{'Availability Graph'}de{'Verfügbarkeit Graph'}")]
        public IEnumerable<FacilityOEEGraphItem> AvailabilityGraph
        {
            get
            {
                if (Details == null || !Details.Any())
                    return new FacilityOEEGraphItem[] { };
                return Details.Select(c => new FacilityOEEGraphItem() { ValueT1 = c.EndDate, ValueT2 = c.AvailabilityOEEPer });
            }
        }

        [ACPropertyInfo(211, "", "en{'Performance Graph'}de{'Leistung Graph'}")]
        public IEnumerable<FacilityOEEGraphItem> PerformanceGraph
        {
            get
            {
                if (Details == null || !Details.Any())
                    return new FacilityOEEGraphItem[] { };
                return Details.Select(c => new FacilityOEEGraphItem() { ValueT1 = c.EndDate, ValueT2 = c.PerformanceOEEPer });
            }
        }

        [ACPropertyInfo(212, "", "en{'Quality Graph'}de{'Qualität Graph'}")]
        public IEnumerable<FacilityOEEGraphItem> QualityGraph
        {
            get
            {
                if (Details == null || !Details.Any())
                    return new FacilityOEEGraphItem[] { };
                return Details.Select(c => new FacilityOEEGraphItem() { ValueT1 = c.EndDate, ValueT2 = c.QualityOEEPer });
            }
        }

        [ACPropertyInfo(213, "", "en{'OEE Graph'}de{'OEE Graph'}")]
        public IEnumerable<FacilityOEEGraphItem> TotalGraph
        {
            get
            {
                if (Details == null || !Details.Any())
                    return new FacilityOEEGraphItem[] { };
                return Details.Select(c => new FacilityOEEGraphItem() { ValueT1 = c.EndDate, ValueT2 = c.TotalOEEPer });
            }
        }
        #endregion


        #region IACObject
        public IACObject ParentACObject => null;

        public IACType ACType => this.ReflectACType();

        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        public string ACIdentifier => this.ReflectGetACIdentifier();

        public string ACCaption => ACIdentifier;

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ReflectGetACUrl(rootACObject);
        }

        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
        }
        #endregion
    }
}
