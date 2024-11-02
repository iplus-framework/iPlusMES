// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using gip.core.webservices;
using CoreWCF;
using CoreWCF.Description;
using CoreWCF.Web;

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

        public override void OnAddKnownTypesToOperationContract(ServiceEndpoint endpoint, OperationDescription opDescr)
        {
            if (   opDescr.Name == nameof(VBWebService.InvokeBarcodeSequence)
                || opDescr.Name == nameof(VBWebService.FinishPickingOrdersByMaterial))
            {
                var knownTypes = ACKnownTypes.GetKnownType();
                foreach (var knownType in knownTypes)
                {
                    opDescr.KnownTypes.Add(knownType);
                }
                foreach (IOperationBehavior behavior in opDescr.OperationBehaviors)
                {
                    if (behavior is DataContractSerializerOperationBehavior)
                    {
                        DataContractSerializerOperationBehavior dataContractBeh = behavior as DataContractSerializerOperationBehavior;
                        //dataContractBeh.MaxItemsInObjectGraph = WCFServiceManager.MaxItemsInObjectGraph;
                        dataContractBeh.DataContractResolver = ACConvert.MyDataContractResolver;
                    }
                }
            }

        }

        #endregion
    }
}
