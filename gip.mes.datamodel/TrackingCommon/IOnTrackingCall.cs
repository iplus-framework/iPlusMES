using gip.core.datamodel;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Decorate BSO object - indicate object have method for handling tracking ACMenuItem
    /// </summary>
    public interface IOnTrackingCall
    {
        void OnTrackingCall(GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter, TrackingEnginesEnum engine);
    }
}
