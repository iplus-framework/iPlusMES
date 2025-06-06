// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.wpfservices;
using gip.mes.wpfservices;

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
            ACStartUpRoot startupRoot = new ACStartUpRoot(new WPFServicesMES());
            startupRoot.LoginUser(userName, password, registerACObjects, PropPersistenceOff, ref errorMsg);
            return errorMsg;
        }
    }
}
