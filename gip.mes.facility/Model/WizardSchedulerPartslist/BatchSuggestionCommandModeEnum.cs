using gip.core.datamodel;

namespace gip.mes.facility
{
    //[DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Batch suggestion mode'}de{'Batch-Vorschlagsmodus'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.facility.ACValueListBatchSuggestionCommandModeEnum")]
    public enum BatchSuggestionCommandModeEnum : short
    {
        KeepEqualBatchSizes = 0,
        KeepStandardBatchSizeAndDivideRest = 1
    }

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Batch suggestion mode'}de{'Batch-Vorschlagsmodus'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListBatchSuggestionCommandModeEnum : ACValueItemList
    {
        public ACValueListBatchSuggestionCommandModeEnum() : base("BatchSuggestionCommandModeEnum")
        {
            AddEntry(BatchSuggestionCommandModeEnum.KeepEqualBatchSizes, "en{'Divide into equal batch sizes'}de{'Auf gleiche Batchgrößen aufteilen'}");
            AddEntry(BatchSuggestionCommandModeEnum.KeepStandardBatchSizeAndDivideRest, "en{'Keep standard batchsize and divide rest'}de{'Standard Batchgröße verwenden und Rest aufteilen'}");
        }
    }
}
