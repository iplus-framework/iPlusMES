using gip.core.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            ACStartUpRoot startupVariobatch = new ACStartUpRoot();
            startupVariobatch.LoginUser(userName, password, registerACObjects, PropPersistenceOff, ref errorMsg);
            return errorMsg;
        }
    }
}
