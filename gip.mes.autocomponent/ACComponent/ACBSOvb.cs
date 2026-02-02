// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.autocomponent
{
    /// <summary>
    /// The ACBSOvbNav class extends ACBSO class with the Database-Context for iplus-MES-Applications. It overrides the Database-Property to return the DatabaseApp-Context.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework+ACBSOvb&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSystem, @"en{'Baseclass for navigationless MES Businessobjects'}de{'Basisklasse für navigationslose MES Geschäftsobjekte'}", Global.ACKinds.TACAbstractClass, Global.ACStorableTypes.NotStorable, true, true,
        Description = @"The ACBSOvbNav class extends ACBSONav class with the Database-Context for iplus-MES-Applications. It overrides the Database-Property to return the DatabaseApp-Context.
                        Visit the https://github.com/search?q=org%3Aiplus-framework+ACBSOvbNav&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.")]
    public abstract class ACBSOvb : ACBSO
    {
        #region c´tors
        public ACBSOvb(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            bool result = await base.ACDeInit(deleteACClassTask);
            _DatabaseApp = null;
            return result;
        }
        #endregion

        #region Database
        private DatabaseApp _DatabaseApp = null;
        /// <summary>Returns the shared Database-Context for BSO's by calling GetAppContextForBSO()</summary>
        /// <value>Returns the shared Database-Context.</value>
        public virtual DatabaseApp DatabaseApp
        {
            get
            {
                if (_DatabaseApp == null && this.InitState != ACInitState.Destructed && this.InitState != ACInitState.Destructing && this.InitState != ACInitState.DisposedToPool && this.InitState != ACInitState.DisposingToPool)
                    _DatabaseApp = this.GetAppContextForBSO();
                return _DatabaseApp as DatabaseApp;
            }
        }

        /// <summary>
        /// Overriden: Returns the DatabaseApp-Property.
        /// </summary>
        /// <value>The context as IACEntityObjectContext.</value>
        public override IACEntityObjectContext Database
        {
            get
            {
                return DatabaseApp;
            }
        }
        #endregion
        
    }
}
