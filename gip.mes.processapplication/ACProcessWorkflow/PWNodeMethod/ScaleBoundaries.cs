using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{
    public class ScaleBoundaries
    {
        public ScaleBoundaries(IPAMContScale scale)
        {
            RemainingWeightCapacity = scale.RemainingWeightCapacity;
            MinDosingWeight = scale.MinDosingWeight;
            MaxWeightCapacity = scale.MaxWeightCapacity.ValueT;
            RemainingVolumeCapacity = scale.RemainingVolumeCapacity;
            MaxVolumeCapacity = scale.MaxVolumeCapacity.ValueT;
        }

        public double? RemainingWeightCapacity
        {
            get;
            set;
        }

        public double? MinDosingWeight
        {
            get;
            set;
        }

        public double MaxWeightCapacity
        {
            get;
            set;
        }

        public double? RemainingVolumeCapacity
        {
            get;
            set;
        }

        public double MaxVolumeCapacity
        {
            get;
            set;
        }
    }
}
