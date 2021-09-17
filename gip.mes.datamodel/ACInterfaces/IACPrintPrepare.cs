using gip.core.autocomponent;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    public interface IACPrintPrepare: IACComponent
    {
        /// <summary>
        /// Prepare BSO to be ready for printing
        /// </summary>
        /// <param name="orderInfo"></param>
        string PrintPrepareAndGetReportName(PAOrderInfo orderInfo);
    }

    
}
