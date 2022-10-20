using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel.EntityMaterial
{
    /// <summary>
    /// Material helper
    /// </summary>
    public class MaterialHelper
    {
        /// <summary>
        /// Helper method for convert on general manner to unique quantity value, for example for quantity comparasion
        /// </summary>
        /// <param name="material"></param>
        /// <param name="quantity"></param>
        /// <param name="mdUnit"></param>
        /// <returns></returns>
        public static double ConvertToBaseQuantity(Material material, Double quantity,MDUnit mdUnit)
        {
            if (mdUnit == null)
            {
                mdUnit = material.BaseMDUnit;
            }
            return material.ConvertToBaseQuantity(quantity, mdUnit);
        }

        public static double ConvertToBaseWeight(Material material, Double quantity,MDUnit mdUnit)
        {
            if (mdUnit == null)
            {
                mdUnit = material.BaseMDUnit;
            }
            return material.ConvertToBaseWeight(quantity, mdUnit);
        }
    }
}
