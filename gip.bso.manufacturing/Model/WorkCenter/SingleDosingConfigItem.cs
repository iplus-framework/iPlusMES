using gip.core.datamodel;
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
