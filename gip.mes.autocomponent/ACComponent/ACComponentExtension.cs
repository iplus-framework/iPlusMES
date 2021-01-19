// ***********************************************************************
// Assembly         : gip.core.datamodel
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="ACFacility.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.autocomponent;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    public static class IACComponentExtension
    {
        public static DatabaseApp GetAppContextForBSO(this ACComponent bso)
        {
            if (bso.ParentACComponent != null && bso.ParentACComponent.Database != null && bso.ParentACComponent.Database is DatabaseApp)
                return bso.ParentACComponent.Database as DatabaseApp;
            DatabaseApp dbApp = ACObjectContextManager.GetContext("BSOAppContext") as DatabaseApp;
            if (dbApp == null)
            {
                Database parentIPlusContext = new Database();
                dbApp = ACObjectContextManager.GetOrCreateContext<DatabaseApp>("BSOAppContext", null, parentIPlusContext);
            }
            return dbApp;
        }
    }
}
