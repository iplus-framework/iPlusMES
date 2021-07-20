using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.processapplication
{
    //[DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Batch suggestion mode'}de{'Batch-Vorschlagsmodus'}", Global.ACKinds.TACEnum)]
    public enum BatchSuggestionCommandModeEnum : short
    {
        KeepEqualBatchSizes = 0,
        KeepStandardBatchSizeAndDivideRest = 1
    }
}
