using System;
#if NETFRAMEWORK
using CoreWCF;
#elif NETSTANDARD
using System.Threading.Tasks;
#endif

namespace gip.mes.webservices
{
    partial interface IVBWebService
    {
    }
}
