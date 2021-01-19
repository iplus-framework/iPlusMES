using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.autocomponent
{
    public abstract class ACBSOvb : ACBSO
    {
        #region c´tors
        public ACBSOvb(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            bool result = base.ACDeInit(deleteACClassTask);
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
