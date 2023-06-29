using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    public interface IVBComponentVBDesignerMaterialWFProxy : IVBComponentDesignManagerProxy
    {
        bool IsEnabledDoModifyAction(IACWorkflowDesignContext visualMain, IACInteractiveObject dropObject, IACInteractiveObject targetVBDataObject, double x, double y);
        IACWorkflowNode DoModifyAction(IACWorkflowDesignContext visualMain, IACInteractiveObject dropObject, IACInteractiveObject targetVBDataObject, double x, double y);

        Material GetMaterial(IACInteractiveObject material);
        void SyncWF();

        #region Edge
        void PreCreateEdge(MaterialWFRelation materialWFRelation);
        void CreateEdge(IVBConnector sourceVBConnector, IVBConnector targetVBConnector);
        #endregion

        #region Delete
        bool DeleteItem(object item, bool isFromDesigner = true);
        bool DeleteFromBSO(MaterialWF materialWF, MaterialWFRelation materialWFRelation);
        bool DeleteVBVisual();
        #endregion

        new IEnumerable<IACObject> GetAvailableTools();
        void Refresh();
    }

    public interface IVBMESDesignerService
    {
        IVBComponentDesignManagerProxy GetDesignMangerProxy(IACComponent component);
        void RemoveDesignMangerProxy(IACComponent component);
    }
}
