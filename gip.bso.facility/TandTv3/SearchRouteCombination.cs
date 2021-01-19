using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.facility
{
    public class SearchRouteCombination
    {
        public string FromACUrl { get; set; }

        public MDTrackingStartItemTypeEnum FromType { get; set; }
        public string ToACUrl { get; set; }

        public MDTrackingStartItemTypeEnum ToType { get; set; }

        #region overrides
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(@"[{0}] {1} => [{2}] {3}", FromType, FromACUrl, ToType, ToACUrl);
        }

        public override bool Equals(object obj)
        {
            bool isEq = false;
            if(obj != null &&  obj is SearchRouteCombination)
            {
                SearchRouteCombination second = obj as SearchRouteCombination;
                isEq = second.FromACUrl == FromACUrl && second.ToACUrl == ToACUrl;
            }
            return isEq;
        }
        #endregion

    }
}
