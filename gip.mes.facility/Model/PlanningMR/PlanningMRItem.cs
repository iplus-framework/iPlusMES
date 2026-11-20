using gip.mes.datamodel;
using System;

namespace gip.mes.facility
{
    /// <summary>
    /// Basic item for calculation of planning material requirements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PlanningMRItem<T>
    {

        public DateTime ConsumptionDate { get;set; }

        public Material Material { get; set; }

        public T Item { get; set; }


        public override string ToString()
        {
            return $"[{ConsumptionDate}] | {Material.MaterialNo} | {Item}";
        }
    }
}
