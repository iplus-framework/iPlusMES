using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.processapplication
{
    //[DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Start next production stage'}de{'Nächste Fertigungsstufe starten'}", Global.ACKinds.TACEnum)]
    public enum StartNextStageMode : short
    {
        /// <summary>
        /// Doesn't start next stages
        /// </summary>
        DoNothing = 0,

        /// <summary>
        /// Starts next stages when starting first batch
        /// </summary>
        StartImmediately = 1,

        /// <summary>
        /// Starts next stages when starting second batch
        /// </summary>
        StartOnStartSecondBatch = 2,

        /// <summary>
        /// Starts next stages when first batch is completed
        /// </summary>
        StartOnFirstBatchCompleted = 3,

        /// <summary>
        /// Starts next stages when last batch is completed and node will complete
        /// </summary>
        StartOnLastBatchCompleted = 4,

        /// <summary>
        /// Doesn't start next stages and completes the open batchplans
        /// </summary>
        CompleteOnLastBatch = 5
    }
}
