using gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public interface IDoItem
    {

        #region properties

        TandTv2StepItem StepItem { get; set; }


        #endregion

        #region methods

        List<IDoItem> SearchRelatedNextStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter);

        List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter);
        void BuildRelations(TandTv2Result result, TandTv2StepItem stepItem, List<TandTv2StepItem> related);

        #endregion
    }
}
