using gip.mes.datamodel;
using System;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_ACClass : DoBackward_ACClass
    {

        #region ctor's

        public DoForward_ACClass(DatabaseApp databaseApp, TandTv2Result result, ACClass item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion
       
    }
}
