using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{

    /// <summary>
    /// This is a point with option sorting 
    /// On by tracking generated tree will be 
    /// </summary>
    public class TandTPointSorted : TandTPoint
    {

        public TandTPointSorted(DatabaseApp dbApp, TandTResult result, TandTResult sortedResult, TandTPoint parentPoint, TandTPoint originalPoint, TandTSortingShemaEnum sortingShema, TandTFilter filter)
            : base(dbApp, originalPoint.Item, filter)
        {
            if (sortedResult.Add(this))
            {
                Parent = parentPoint;
                if (parentPoint != null)
                    parentPoint.AddChild(this);
                Item = originalPoint.Item;
                ProcessSortedRelated(dbApp, result, sortedResult, originalPoint, sortingShema);
            }
        }

        public virtual void ProcessSortedRelated(DatabaseApp dbApp, TandTResult result, TandTResult sortedResult, TandTPoint originalPoint, TandTSortingShemaEnum sortingShema)
        {

        }
    }
}
