using gip.core.autocomponent;
using gip.core.datamodel;

namespace gip.mes.datamodel
{

    /// <summary>
    /// Starting point for every version of tracking
    /// </summary>
    public interface ITrackingCommand
    {
        void DoTracking(ACBSO bso, GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter);
        void DoTracking(ACBSO bso, GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter, TrackingEnginesEnum engine);
    }
}
