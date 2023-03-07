using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.processapplication
{
    //[DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Start next production stage'}de{'Nächste Fertigungsstufe starten'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.processapplication.ACValueListStartNextStageMode")]
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

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Start next production stage'}de{'Nächste Fertigungsstufe starten'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListStartNextStageMode : ACValueItemList
    {
        public ACValueListStartNextStageMode() : base("StartNextStageMode")
        {
            AddEntry(StartNextStageMode.DoNothing, "en{'No Action'}de{'Keine Aktion'}");
            AddEntry(StartNextStageMode.StartImmediately, "en{'Starts next stages when starting first batch'}de{'Start die nächsten Stufen wenn der erste Batch gestartet wird'}");
            AddEntry(StartNextStageMode.StartOnStartSecondBatch, "en{'Starts next stages when starting second batch'}de{'Startet die nächsten Stufen wenn der zweite Batch gestartet wird'}");
            AddEntry(StartNextStageMode.StartOnFirstBatchCompleted, "en{' Starts next stages when first batch is completed'}de{'Startet die nächsten Stufen wenn der erste Batch beendet wird'}");
            AddEntry(StartNextStageMode.StartOnLastBatchCompleted, "en{'Starts next stages when last batch is completed'}de{'Startet die nächsten Stufen wenn der letzte Batch beendet wurde'}");
            AddEntry(StartNextStageMode.CompleteOnLastBatch, "en{'Does not start next stages and completes the open batchplans'}de{'Startet keine nächste Stufe und beendet die offenen Batchpläne'}");
        }
    }
}
