using System;
#if NETFRAMEWORK
using System.ServiceModel;
#elif NETSTANDARD
using System.Threading.Tasks;
#endif

namespace gip.mes.webservices
{
    partial interface IVBWebService
    {
    }
}
