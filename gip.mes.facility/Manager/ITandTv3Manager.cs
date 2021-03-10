using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.core.datamodel;

namespace gip.mes.facility
{
    public interface ITandTv3Manager:IACComponent, ITandTFetchCharge
    {
        void StartTandTBSO(ACBSO bso, TandTv3FilterTracking filter);

        TandTv3.TandTResult DoTracking(DatabaseApp databaseApp, TandTv3FilterTracking filter, string vBUserNo, bool useGroupResult);
        TandTv3.TandTResult DoSelect(DatabaseApp databaseApp, TandTv3FilterTracking filter, string vbUserNo, bool useGroupResult);
        MsgWithDetails DoDeleteTracking(DatabaseApp databaseApp, TandTv3FilterTracking filter);

    }
}
