// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System.Collections.Generic;

namespace gip.bso.manufacturing
{
    public class SingleDosingConfigItem
    {
        public string PreConfigACUrl
        {
            get;
            set;
        }

        public ACClassWF PWGroup
        {
            get;
            set;
        }

        public IEnumerable<ACClass> PossibleMachines
        {
            get => PWGroup.RefPAACClass.DerivedClassesInProjects;
        }
    }
}
