using gip.mes.datamodel;
using System.Collections.Generic;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_Facility : DoBackward_Facility
    {

        #region ctor's

        public DoForward_Facility(DatabaseApp databaseApp, TandTv2Result result, Facility item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion


        #region DoItem overriden
        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp dbApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            if (Item.VBiFacilityACClassID == null) return null;
            return new List<IDoItem>() { new DoForward_ACClass(dbApp, result, Item.VBiFacilityACClass, jobFilter) };
        }

        #endregion

    }
}
