using gip.core.autocomponent;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    public interface IPrintManager: IACComponent
    {
        bool Print(PAOrderInfo pAOrderInfo, int copyCount);
    }
}
