using gip.core.autocomponent;

namespace gip.mes.datamodel
{
    public static class HelperPrintManager
    {
        public const string C_DefaultServiceACIdentifier = "ACPrintManager";

        public static IPrintManager GetServiceInstance(ACComponent requester = null)
        {
            if (requester == null)
                requester = ACRoot.SRoot;
            return PARole.GetServiceInstance<IPrintManager>(requester, C_DefaultServiceACIdentifier, PARole.CreationBehaviour.OnlyLocal);
        }
    }
}
