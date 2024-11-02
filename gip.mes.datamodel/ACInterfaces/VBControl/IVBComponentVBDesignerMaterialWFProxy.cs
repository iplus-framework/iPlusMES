// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
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
}
