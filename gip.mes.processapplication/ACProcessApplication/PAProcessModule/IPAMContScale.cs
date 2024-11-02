// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;
using gip.mes.datamodel;

namespace gip.mes.processapplication
{
    public interface IPAMCont : IACComponent
    {
        double? RemainingVolumeCapacity
        {
            get;
        }

        IACContainerTNet<Double> MaxVolumeCapacity { get; set; }
        IACContainerTNet<String> OrderInfo { get; set; }
        IACContainerTNet<String> ReservationInfo { get; set; }
        IACContainerTNet<Double> FillVolume { get; set; }

        void ResetFillVolume();
    }


    public interface IPAMContScale : IPAMCont
    {

        #region Properties
        PAEScaleGravimetric Scale
        {
            get;
        }

        double? RemainingWeightCapacity
        {
            get;
        }

        double? MinDosingWeight
        {
            get;
        }

        IACContainerTNet<Double> MaxWeightCapacity { get; set; }

        #endregion

    }

    public static class PAMContScaleExtension
    {
        public static PAEScaleGravimetric GetScale(IPAMContScale contScale)
        {
            PAEScaleGravimetric scale = null;
            var discharging = contScale.FindChildComponents<PAFDischarging>(c => c is PAFDischarging).FirstOrDefault();
            if (discharging != null)
                scale = discharging.CurrentScaleForWeighing as PAEScaleGravimetric;
            if (scale != null)
                return scale;
            return contScale.FindChildComponents<PAEScaleGravimetric>(c => c is PAEScaleGravimetric).FirstOrDefault();
        }

        public static bool IsScaleEmpty(IPAMContScale contScale)
        {
            var scale = contScale.Scale;
            if (scale == null)
                return true;
            double emptyRange = 5.0;
            if (Math.Abs(scale.LowerLimit1.ValueT - 0) > Double.Epsilon)
                emptyRange = scale.LowerLimit1.ValueT;
            else if (contScale.MaxWeightCapacity.ValueT > 0.001)
                emptyRange = contScale.MaxWeightCapacity.ValueT / 50;
            return scale.ActualValue.ValueT < emptyRange;
        }

        public static double? RemainingWeightCapacity(IPAMContScale contScale)
        {
            if (contScale.MaxWeightCapacity.ValueT <= 0.00000001)
                return null;
            var scale = contScale.Scale;
            if (scale == null)
                return null;
            double actualWeight = scale.ActualValue.ValueT;
            // Scale is not calibrated well:
            if (actualWeight < 0.000001)
                actualWeight = 0;
            return contScale.MaxWeightCapacity.ValueT - actualWeight;
        }

        public static double? RemainingVolumeCapacity(IPAMContScale contScale)
        {
            if (contScale.MaxVolumeCapacity.ValueT <= 0.00000001)
                return null;
            return contScale.MaxVolumeCapacity.ValueT - contScale.FillVolume.ValueT;
        }

        public static double? MinDosingWeight(IPAMContScale contScale)
        {
            double? minWeight = null;
            foreach (PAEScaleGravimetric scale in contScale.FindChildComponents<PAEScaleGravimetric>(c => c is PAEScaleGravimetric, null, 1))
            {
                if (!minWeight.HasValue || minWeight.Value > scale.MinDosingWeight.ValueT)
                    minWeight = scale.MinDosingWeight.ValueT;
            }
            return minWeight;
        }
    }
}
