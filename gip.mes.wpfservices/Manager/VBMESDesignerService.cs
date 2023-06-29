using gip.core.datamodel;
using gip.core.manager;
using gip.core.wpfservices;
using gip.mes.datamodel;
using gip.mes.manager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.wpfservices
{
    public class VBMESDesignerService : VBDesignerService, IVBMESDesignerService
    {
        public VBMESDesignerService()
        {
        }

        private ConcurrentDictionary<IACComponent, IVBComponentDesignManagerProxy> _DesignManagerProxies = new ConcurrentDictionary<IACComponent, IVBComponentDesignManagerProxy>();

        public override IVBComponentDesignManagerProxy GetDesignMangerProxy(IACComponent component)
        {
            IVBComponentDesignManagerProxy proxy = null;
            if (!_DesignManagerProxies.TryGetValue(component, out proxy))
            {
                if (component is VBDesignerMaterialWF)
                    proxy = new VBDesignerMaterialWFProxy(component);
                else if (component is VBDesignerWorkflowMethod)
                    proxy = new VBDesignerWorkflowMethodProxy(component);
                else if (component is VBDesignerWorkflow)
                    proxy = new VBDesignerWorkflowProxy(component);
                else if (component is VBDesignerXAML)
                    proxy = new VBDesignerXAMLProxy(component);
                if (proxy != null)
                    _DesignManagerProxies.TryAdd(component, proxy);
            }
            return proxy;
        }

        public override void RemoveDesignMangerProxy(IACComponent component)
        {
            IVBComponentDesignManagerProxy proxy = null;
            if (_DesignManagerProxies.ContainsKey(component))
                _DesignManagerProxies.Remove(component, out proxy);
        }

    }
}
