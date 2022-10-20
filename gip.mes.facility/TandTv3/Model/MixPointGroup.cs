using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility.TandTv3.Model
{
    public class MixPointGroup
    {
        public int PartslistSequence { get; set; }
        public string InwardMaterialNo { get; set; }
        public string InwardMaterialName { get; set; }
        public string ProgramNo { get; set; }

        #region overrides

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if(obj != null && obj is MixPointGroup)
            {
                MixPointGroup second = obj as MixPointGroup;
                isEqual =
                    PartslistSequence == second.PartslistSequence
                    && InwardMaterialNo == second.InwardMaterialNo
                    && InwardMaterialName == second.InwardMaterialName
                    && ProgramNo == second.ProgramNo;
            }
            return isEqual;
        }

        #endregion
    }
}
