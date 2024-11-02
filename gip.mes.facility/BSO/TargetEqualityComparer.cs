// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public class TargetEqualityComparer : IEqualityComparer<RouteItem>
    {
        /// <summary>
        /// Equalses the specified val1.
        /// </summary>
        /// <param name="val1">The val1.</param>
        /// <param name="val2">The val2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Equals(RouteItem val1, RouteItem val2)
        {
            if (val1.Target == val2.Target)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(RouteItem val)
        {
            return val.Target.GetHashCode();
        }
    }
}
