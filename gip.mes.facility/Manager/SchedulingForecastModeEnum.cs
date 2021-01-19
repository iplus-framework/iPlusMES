using gip.core.datamodel;

namespace gip.mes.facility
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'SchedulingForecastModeEnum'}de{'SchedulingForecastModeEnum'}", Global.ACKinds.TACEnum)]
    public enum SchedulingForecastModeEnum
    {
        LinearAverage,
        AverageWithoutDeviateValues
    }
}
