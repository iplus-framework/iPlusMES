using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility.TandTv3
{
    public class TandTStep
    {
        #region  ctor's

        public TandTStep()
        {
            MixingPoints = new List<TandTv3Point>();
        }

        #endregion

        #region Elements

        public int StepNo { get; set; }

        public List<TandTv3Point> MixingPoints { get; set; }

        #endregion

        #region method overrides
        public override string ToString()
        {
            return string.Format(@"Step nr. #{0}", StepNo);
        }
        #endregion
    }
}
