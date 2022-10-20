
namespace gip.mes.facility
{
    /// <summary>
    ///  Define calculation model by calculating 
    ///  with rest of quantity in batch generation
    /// </summary>
    public enum RestHandleModeEnum
    {
        ToFirstBatch,
        ToLastBatch,
        DevideToAllBatches,
        DoNothing
    }
}