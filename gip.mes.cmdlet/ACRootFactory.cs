using gip.core.autocomponent;
using gip.core.wpfservices;

namespace gip.mes.cmdlet
{
    public static class ACRootFactory
    {
        /// <summary>
        /// Factory variobatch object for test Purposes
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Factory(string userName = "superuser", string password = "netspirit", bool registerACObjects = false)
        {
            #region setup 
            string errorMsg = null;
            bool PropPersistenceOff = false;
            #endregion

            //TODO Linux implementation
            ACStartUpRoot startupRoot = new ACStartUpRoot(new WPFServices());
            startupRoot.LoginUser(userName, password, registerACObjects, PropPersistenceOff, ref errorMsg);
            return errorMsg;
        }
    }
}
