using gip.core.datamodel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace gip.mes.facility
{
    public interface IEInvoiceServiceClient: IACComponent
    {

        #region Properties
        string EInvoiceAPIServiceURL { get; set; }
        string EInvoiceAPIServiceKey { get; set; }
        #endregion


        #region Methods

        Task<IEInvoiceServiceClientResponse> SendDocumentAsync(
           Stream ublXmlStream,
           string companyVatNumber,
           bool? fiscalization = null,
           string fileName = null,
           string softwareId = null,
           CancellationToken ct = default(CancellationToken));

        #endregion
    }
}
