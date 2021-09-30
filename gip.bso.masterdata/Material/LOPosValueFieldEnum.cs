using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.bso.masterdata
{
    //[DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Laborder line filter-field'}de{'Laborauftragsposition Filterfeld'}", Global.ACKinds.TACEnum, QRYConfig = "bso.masterdata.ACValueListLOPosValueFieldEnum")]
    public enum LOPosValueFieldEnum : short
    {
        MinMin = 0,
        Min = 1,
        Max = 2,
        MaxMax = 3
    }

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Laborder line filter-field'}de{'Laborauftragsposition Filterfeld'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListLOPosValueFieldEnum : ACValueItemList
    {
        public ACValueListLOPosValueFieldEnum() : base("LOPosValueFieldEnum")
        {
            AddEntry(LOPosValueFieldEnum.MinMin, "en{'Lowest value for alarm'}de{'Unterer Alarmgrenzwert'}");
            AddEntry(LOPosValueFieldEnum.Min, "en{'Lowest value'}de{'Unterer Grenzwert'}");
            AddEntry(LOPosValueFieldEnum.Max, "en{'Maximum value'}de{'Oberer Grenzwert'}");
            AddEntry(LOPosValueFieldEnum.MaxMax, "en{'Maximum value for alarm'}de{'Oberer Alarmgrenzwert'}");
        }
    }
}
