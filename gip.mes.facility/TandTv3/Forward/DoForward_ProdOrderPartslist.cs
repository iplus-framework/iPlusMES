using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_ProdOrderPartslist : DoBackward_ProdOrderPartslist
    {

        #region ctor's

        public DoForward_ProdOrderPartslist(DatabaseApp databaseApp, TandTResult result, ProdOrderPartslist item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion

	}
}