using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using gip.core.webservices;

namespace gip.mes.webservices
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Json Host MES'}de{'Json Host MES'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PAJsonServiceHostVB : PAJsonServiceHost
    {
        #region c´tors
        public PAJsonServiceHostVB(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion



        #region Implementation

        public override Type ServiceType
        {
            get
            {
                return typeof(VBWebService);
            }
        }

        public override Type ServiceInterfaceType
        {
            get
            {
                return typeof(IVBWebService);
            }
        }

        public override object GetWebServiceInstance()
        {
            return new VBWebService();
        }

        #endregion
    }
}
