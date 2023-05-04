using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.layoutengine;
using gip.core.layoutengine.Helperclasses;
using gip.core.manager;
using gip.core.wpfservices;
using gip.core.wpfservices.Manager;
using gip.ext.design;
using gip.ext.designer.Services;
using gip.mes.datamodel;
using gip.mes.manager;
using static gip.core.manager.VBDesigner;

namespace gip.mes.wpfservices
{
    public class VBDesignerMaterialWFProxy : VBDesignerWorkflowProxy, IVBComponentVBDesignerMaterialWFProxy
    {
        public VBDesignerMaterialWFProxy(IACComponent component) : base(component)
        {
        }

        #region Visual items/edges

        public IEnumerable<VBVisual> VisualItems
        {
            get
            {
                VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
                if (vbDesigner == null)
                    return null;

                if (vbDesigner.VBDesignEditor != null)
                    return ((VBDesignEditor)vbDesigner.VBDesignEditor).DesignContext.Services.Component.DesignItems.Where(c => c.View is VBVisual && c.Parent != null).Select(x => x.View as VBVisual);
                return null;
            }
        }

        public IEnumerable<VBEdge> VisualEdges
        {
            get
            {
                VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
                if (vbDesigner == null)
                    return null;

                return ((VBDesignEditor)vbDesigner.VBDesignEditor).DesignContext.Services.Component.DesignItems.Where(c => c.View is VBEdge).Select(x => x.View as VBEdge);
            }
        }

        #endregion

        public override DesignItem CreateVBVisualDesignItem(VisualInfo visualInfo, IACWorkflowNode acVisualWF, DesignContext designContext, out DesignItem designItemParent)
        {
            DesignItem item = base.CreateVBVisualDesignItem(visualInfo, acVisualWF, designContext, out designItemParent);

            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return null;

            IACWorkflowContext vbWorkflow = vbDesigner.CurrentDesign as IACWorkflowContext;
            if (vbWorkflow == null)
                return item;
            vbDesigner.WFLayoutCalculatorProxy.LayoutMaterialWF(vbWorkflow, designItemParent, item, (short)visualInfo.LayoutAction);
            return item;
        }

        #region Modify/Create

        public bool IsEnabledDoModifyAction(IACWorkflowDesignContext visualMain, IACInteractiveObject dropObject, IACInteractiveObject targetVBDataObject, double x, double y)
        {
            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return false;

            if (!vbDesigner.IsDesignMode)
                return false;
            MaterialWF materialWF = visualMain as MaterialWF;

            if (targetVBDataObject is VBVisualGroup && !materialWF.MaterialWFRelation_MaterialWF.Any())
                return true;
            else if (targetVBDataObject is VBVisual)
            {
                Material source = GetMaterial(dropObject);
                Material target = GetMaterial(targetVBDataObject);
                if (source != target && !materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == source && c.TargetMaterial == target))
                    if (vbDesigner.CanInsertMaterialWFRelation(materialWF, source, target))
                        return true;
            }
            else if (targetVBDataObject is VBEdge)
            {
                Material sourceMaterial = ((PWOfflineNodeMaterial)((VBEdge)targetVBDataObject).SourceElement.DataContext).ContentMaterial;
                Material targetMaterial = ((PWOfflineNodeMaterial)((VBEdge)targetVBDataObject).TargetElement.DataContext).ContentMaterial;
                Material newMaterial = GetMaterial(dropObject);
                if (sourceMaterial != newMaterial && targetMaterial != newMaterial)
                    return true;
            }
            return false;
        }

        public IACWorkflowNode DoModifyAction(IACWorkflowDesignContext visualMain, IACInteractiveObject dropObject, IACInteractiveObject targetVBDataObject, double x, double y)
        {
            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return null;
            if (!vbDesigner.IsDesignMode)
                return null;
            MaterialWF materialWF = visualMain as MaterialWF;
            if (materialWF == null)
                return null;


            IEnumerable<Material> MaterialList = materialWF.GetMaterials();
            MaterialWFRelation relation = null, relation1 = null;
            ClearVisualChangeList();

            if (targetVBDataObject is VBVisualGroup && !materialWF.MaterialWFRelation_MaterialWF.Any())
            {
                relation = vbDesigner.CreateRelation(materialWF, null, GetMaterial(dropObject));
            }
            else if ((targetVBDataObject is VBVisualGroup || targetVBDataObject is VBVisual) && (materialWF.MaterialWFRelation_MaterialWF.Count() == 1) && materialWF.GetMaterials().Count() == 1)
            {
                relation = materialWF.MaterialWFRelation_MaterialWF.FirstOrDefault();
                relation.SourceMaterial = GetMaterial(dropObject);
            }
            else if (targetVBDataObject is VBVisual)
            {
                relation = vbDesigner.CreateRelation(materialWF, GetMaterial(dropObject), GetMaterial(targetVBDataObject));
            }
            else if (targetVBDataObject is VBEdge)
            {
                Material sourceMaterial = ((PWOfflineNodeMaterial)((VBEdge)targetVBDataObject).SourceElement.DataContext).ContentMaterial;
                Material targetMaterial = ((PWOfflineNodeMaterial)((VBEdge)targetVBDataObject).TargetElement.DataContext).ContentMaterial;
                Material newMaterial = GetMaterial(dropObject);
                if (sourceMaterial == null || targetMaterial == null || newMaterial == null)
                    return null;
                if (sourceMaterial.MaterialID == newMaterial.MaterialID || targetMaterial.MaterialID == newMaterial.MaterialID)
                {
                    var msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = vbDesigner.Root.Environment.TranslateMessage(vbDesigner, @"Error50058", newMaterial.MaterialNo) };
                    vbDesigner.Messages.Msg(msg, Global.MsgResult.OK);
                    return null;
                }

                DeleteEdge(materialWF, materialWF.MaterialWFRelation_MaterialWF.FirstOrDefault(c => c.SourceMaterial == sourceMaterial && c.TargetMaterial == targetMaterial), ((VBEdge)targetVBDataObject), false);

                if (!materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == sourceMaterial && c.TargetMaterial == newMaterial))
                    relation = vbDesigner.CreateRelation(materialWF, sourceMaterial, newMaterial);

                if (!materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == newMaterial && c.TargetMaterial == targetMaterial))
                    relation1 = vbDesigner.CreateRelation(materialWF, newMaterial, targetMaterial);
            }

            if (relation != null && relation.SourceMaterialID == relation.TargetMaterialID && materialWF.MaterialWFRelation_MaterialWF.Count > 1)
            {
                var msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = vbDesigner.Root.Environment.TranslateMessage(vbDesigner, @"Error50058", relation.SourceMaterial.MaterialNo) };
                vbDesigner.Messages.Msg(msg, Global.MsgResult.OK);
                return null;
            }

            if (relation != null)
                UpdateDesigner(relation, vbDesigner, new Rect(new Point(x, y), new Size(150, 50)));

            if (relation1 != null)
                UpdateDesigner(relation1, vbDesigner);

            vbDesigner.UpdateMaterialWFBSO();

            ((IToolService)vbDesigner.ToolService).CurrentTool = ((IToolService)vbDesigner.ToolService).PointerTool;
            Refresh();
            return null;
        }
        #endregion


        #region UpdateMethods

        private void UpdateDesigner(MaterialWFRelation materialWFRelation, ACBSO bso, Rect pos = new Rect())
        {
            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return;

            VisualChangeList.Clear();
            VBDesignEditor vbDesignEditor = vbDesigner.VBDesignEditor as VBDesignEditor;
            if (vbDesignEditor != null)
            {
                DesignContext designContext = vbDesignEditor.DesignSurface.DesignContext;
                if (designContext == null)
                    return;

                if (!VisualItems.Any())
                {
                    AddToVisualChangeListRect(materialWFRelation.TargetMaterial, LayoutActionType.Insert, materialWFRelation.TargetMaterial.GetACUrl(materialWFRelation.MaterialWF), "", pos);
                    UpdateVisual();
                }
                else if (!VisualItems.Any(c => c.VBContent == materialWFRelation.SourceMaterial.ACIdentifier))
                {
                    VisualChangeList.Clear();
                    AddToVisualChangeListRect(materialWFRelation.SourceMaterial, LayoutActionType.Insert, materialWFRelation.SourceMaterial.GetACUrl(materialWFRelation.MaterialWF));
                    UpdateVisual();
                    PreCreateEdge(materialWFRelation);
                }
                else if (!VisualItems.Any(c => c.VBContent == materialWFRelation.TargetMaterial.ACIdentifier))
                {
                    VisualChangeList.Clear();
                    AddToVisualChangeList(materialWFRelation.TargetMaterial, (short)LayoutActionType.Insert, materialWFRelation.TargetMaterial.GetACUrl(materialWFRelation.MaterialWF));
                    UpdateVisual();
                    PreCreateEdge(materialWFRelation);
                }
                else
                {
                    PreCreateEdge(materialWFRelation);
                }
                ((MaterialWF)vbDesigner.CurrentDesign).FromNode = null;
                ((MaterialWF)vbDesigner.CurrentDesign).ToNode = null;
            }
        }

        #endregion

        #region MaterialWF

        public Material GetMaterial(IACInteractiveObject material)
        {
            if (material is VBTreeViewItem)
            {
                if (((VBTreeViewItem)material).ContentACObject is ACObjectItem)
                    if (((ACObjectItem)((VBTreeViewItem)material).ContentACObject).Value is NodeInfo)
                        return ((NodeInfo)((ACObjectItem)((VBTreeViewItem)material).ContentACObject).Value).NodeItem as Material;
            }
            else if (material is VBVisual)
                if (material.ContextACObject is PWOfflineNodeMaterial)
                    return ((PWOfflineNodeMaterial)material.ContextACObject).ContentMaterial;

            return null;
        }

        #endregion

        #region Sync

        public void SyncWF()
        {
            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return;

            MaterialWF materialWF = vbDesigner.CurrentDesign as MaterialWF;

            if (materialWF == null || !materialWF.MaterialWFRelation_MaterialWF.Any())
                return;

            VisualChangeList.Clear();
            foreach (VBVisual item in VisualItems)
            {
                Material src = ((PWOfflineNodeMaterial)item.ContextACObject).ContentMaterial;
                if (src != null)
                    AddToVisualChangeList(src, (short)LayoutActionType.Delete, src.ACIdentifier);
            }

            foreach (VBEdge edge in VisualEdges)
                AddToVisualChangeList(new MaterialWFRelation(), (short)LayoutActionType.DeleteEdge, edge.VBConnectorSource, edge.VBConnectorTarget);

            UpdateVisual();

            MaterialWFRelation firstRelation = materialWF.MaterialWFRelation_MaterialWF.FirstOrDefault(c => !c.TargetMaterial.MaterialWFRelation_SourceMaterial.Any(x => x.MaterialWF == materialWF));
            Guid materialID = firstRelation.SourceMaterialID;
            firstRelation.SourceMaterial = null;
            UpdateDesigner(firstRelation, vbDesigner);
            firstRelation.SourceMaterial = vbDesigner.DatabaseApp.Material.FirstOrDefault(c => c.MaterialID == materialID);
            UpdateDesigner(firstRelation, vbDesigner);
            foreach (MaterialWFRelation relation in materialWF.MaterialWFRelation_MaterialWF.Where(c => c != firstRelation).OrderBy(c => c.Sequence))
                UpdateDesigner(relation, vbDesigner);

            ((VBDesignEditor)vbDesigner.VBDesignEditor).DesignSurface.UpdateLayout();
        }

        #endregion

        #region EdgeCreation

        public void PreCreateEdge(MaterialWFRelation materialWFRelation)
        {
            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return;

            VBVisual source = VisualItems.FirstOrDefault(c => c.VBContent == materialWFRelation.SourceMaterial.ACIdentifier);
            VBVisual target = VisualItems.FirstOrDefault(c => c.VBContent == materialWFRelation.TargetMaterial.ACIdentifier);
            VBConnector sourceConn = VBLogicalTreeHelper.FindChildObjectInLogicalTree(source, "PWPointOut") as VBConnector;
            VBConnector targetConn = VBLogicalTreeHelper.FindChildObjectInLogicalTree(target, "PWPointIn") as VBConnector;
            ((MaterialWF)vbDesigner.CurrentDesign).FromNode = materialWFRelation.SourceMaterial;
            ((MaterialWF)vbDesigner.CurrentDesign).ToNode = materialWFRelation.TargetMaterial;
            CreateEdge(sourceConn, targetConn);
        }

        public void CreateEdge(IVBConnector sourceVBConnector, IVBConnector targetVBConnector)
        {
            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return;

            VisualChangeList.Clear();

            IACWorkflowNode sourceVBVisual = sourceVBConnector.ParentACObject as IACWorkflowNode;
            if (sourceVBVisual == null)
            {
                if (sourceVBConnector.ParentACObject != null && sourceVBConnector.ParentACObject is PWOfflineNodeMaterial)
                    sourceVBVisual = ((PWOfflineNodeMaterial)sourceVBConnector.ParentACObject).ContentMaterial as IACWorkflowNode;
                else if (sourceVBConnector is VBConnector)
                {
                    VBConnector sourceVBConn = sourceVBConnector as VBConnector;
                    if (sourceVBConn.DataContext != null && sourceVBConn.DataContext is PWOfflineNodeMaterial)
                        sourceVBVisual = ((PWOfflineNodeMaterial)sourceVBConn.DataContext).ContentMaterial as IACWorkflowNode;
                }

            }
            gip.core.datamodel.ACClassProperty sourceACClassProperty = null;
            IACWorkflowNode targetVBVisual = targetVBConnector.ParentACObject as IACWorkflowNode;
            if (targetVBVisual == null)
            {
                if (targetVBConnector.ParentACObject != null && targetVBConnector.ParentACObject is PWOfflineNodeMaterial)
                    targetVBVisual = ((PWOfflineNodeMaterial)targetVBConnector.ParentACObject).ContentMaterial as IACWorkflowNode;
                else if (targetVBConnector is VBConnector)
                {
                    VBConnector targetVBConn = targetVBConnector as VBConnector;
                    if (targetVBConn.DataContext != null && targetVBConn.DataContext is PWOfflineNodeMaterial)
                        targetVBVisual = ((PWOfflineNodeMaterial)targetVBConn.DataContext).ContentMaterial as IACWorkflowNode;
                }
            }
            gip.core.datamodel.ACClassProperty targetACClassProperty = null;

            sourceACClassProperty = sourceVBVisual.GetConnector(sourceVBConnector.VBContent);

            targetACClassProperty = targetVBVisual.GetConnector(targetVBConnector.VBContent);


            vbDesigner.WFCreateWFEdge(sourceVBVisual, sourceACClassProperty, targetVBVisual, targetACClassProperty);
            UpdateVisual();
            VisualChangeList.Clear();
            vbDesigner.OnPropertyChanged("DesignXAML");
        }

        #endregion

        #region Delete

        public bool DeleteItem(object item, bool isFromDesigner = true)
        {
            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return false;

            VisualChangeList.Clear();
            MaterialWF materialWF = vbDesigner.CurrentDesign as MaterialWF;
            if (materialWF == null && item == null)
                return false;
            Material source = null;
            DesignItem designItem = item as DesignItem;
            if (item is VBVisual || (designItem != null && designItem.View is VBVisual))
            {
                VBVisual vbVisual = item as VBVisual;
                if (vbVisual == null)
                    vbVisual = designItem.View as VBVisual;
                source = ((PWOfflineNodeMaterial)vbVisual.ContextACObject).ContentMaterial;
                if (vbVisual != null && source != null)
                    DeleteWF(materialWF, vbVisual, source, isFromDesigner);
                return true;
            }
            else if (item is VBEdge || (designItem != null && designItem.View is VBEdge))
            {
                VBEdge vbEdge = item as VBEdge;
                if (vbEdge == null)
                    vbEdge = designItem.View as VBEdge;
                MaterialWFRelation relation = materialWF.MaterialWFRelation_MaterialWF.FirstOrDefault(c => c.ACIdentifier == vbEdge.ACIdentifier);
                if (relation == null)
                    return false;
                source = relation.SourceMaterial;
                if (source.MaterialWFRelation_SourceMaterial.Count(c => c.MaterialWF == materialWF) + source.MaterialWFRelation_TargetMaterial.Count(c => c.MaterialWF == materialWF) == 1)
                {
                    AddToVisualChangeList(source, (short)LayoutActionType.Delete, source.ACIdentifier);
                    UpdateVisual();
                }
                DeleteEdge(materialWF, relation, vbEdge, true, true);
                vbDesigner.OnPropertyChanged("DesignXAML");
                return true;
            }
            return false;
        }

        public bool DeleteFromBSO(MaterialWF materialWF, MaterialWFRelation materialWFRelation)
        {
            VBEdge vbEdge = VisualEdges.FirstOrDefault(c => c.ACIdentifier == materialWFRelation.ACIdentifier);
            return DeleteEdge(materialWF, materialWFRelation, vbEdge, false, false);
        }

        public bool DeleteVBVisual()
        {
            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return false;

            if (!vbDesigner.IsEnabledDeleteVBVisual())
                return false;
            MaterialWF materialWF = vbDesigner.CurrentDesign as MaterialWF;

            if (vbDesigner.CurrentContentACObject is PWOfflineNodeMaterial)
            {
                Material material = ((PWOfflineNodeMaterial)vbDesigner.CurrentContentACObject).ContentMaterial;
                DeleteItem(VisualItems.FirstOrDefault(c => c.VBContent == material.ACIdentifier), true);
            }
            else if (vbDesigner.SelectedVBControl is VBEdge)
            {
                MaterialWFRelation relation = materialWF.MaterialWFRelation_MaterialWF.FirstOrDefault(c => c.ACIdentifier == vbDesigner.SelectedVBControl.ACIdentifier);
                if (relation == null)
                    return false;
                Material source = relation.SourceMaterial;
                if (source.MaterialWFRelation_SourceMaterial.Count(c => c.MaterialWF == materialWF) + source.MaterialWFRelation_TargetMaterial.Count(c => c.MaterialWF == materialWF) == 1)
                    AddToVisualChangeList(source, (short)LayoutActionType.Delete, source.ACIdentifier);
                DeleteEdge(materialWF, relation, vbDesigner.SelectedVBControl as VBEdge, true);
            }
            Refresh();
            return false;
        }

        protected bool DeleteWF(MaterialWF materialWF, VBVisual vbVisual, Material source, bool isFromDesigner)
        {
            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return false;

            if (!source.MaterialWFRelation_TargetMaterial.Any(c => c.MaterialWF == materialWF))
            {
                if (isFromDesigner)
                    AddToVisualChangeList(source, (short)LayoutActionType.Delete, source.ACIdentifier);
                foreach (MaterialWFRelation relation in source.MaterialWFRelation_SourceMaterial.Where(c => c.MaterialWF == materialWF).ToList())
                {
                    VBEdge edge = VisualEdges.FirstOrDefault(c => c.ACIdentifier == relation.ACIdentifier);
                    DeleteEdge(materialWF, relation, edge, false);
                }

                vbDesigner.OnPropertyChanged("DesignXAML");
                return true;
            }
            else if (source.MaterialWFRelation_TargetMaterial.Any(c => c.MaterialWF == materialWF) && source.MaterialWFRelation_SourceMaterial.Count(c => c.MaterialWF == materialWF) == 1)
            {
                MaterialWFRelation sRelation = source.MaterialWFRelation_SourceMaterial.FirstOrDefault(c => c.MaterialWF == materialWF);
                Material newTarget = sRelation.TargetMaterial;
                if (isFromDesigner)
                {
                    AddToVisualChangeList(source, (short)LayoutActionType.Delete, source.ACIdentifier);
                    UpdateVisual();
                }
                VBEdge sEdge = VisualEdges.FirstOrDefault(c => c.ACIdentifier == sRelation.ACIdentifier);
                DeleteEdge(materialWF, sRelation, sEdge, false);

                foreach (MaterialWFRelation relation in source.MaterialWFRelation_TargetMaterial.Where(c => c.MaterialWF == materialWF).ToList())
                {
                    Material newSource = relation.SourceMaterial;
                    if (!materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == newSource && c.TargetMaterial == newTarget))
                    {
                        MaterialWFRelation newRelation = vbDesigner.CreateRelation(materialWF, newSource, newTarget);
                        UpdateDesigner(newRelation, vbDesigner);
                    }
                    VBEdge edge = VisualEdges.FirstOrDefault(c => c.ACIdentifier == relation.ACIdentifier);
                    source.MaterialWFRelation_TargetMaterial.Remove(relation);
                    DeleteEdge(materialWF, relation, edge, false);
                }
                vbDesigner.OnPropertyChanged("DesignXAML");
                return true;
            }
            else if (source.MaterialWFRelation_SourceMaterial.Any(c => c.MaterialWF == materialWF) && source.MaterialWFRelation_TargetMaterial.Count(c => c.MaterialWF == materialWF) == 1)
            {
                MaterialWFRelation tRelation = source.MaterialWFRelation_TargetMaterial.FirstOrDefault(c => c.MaterialWF == materialWF);
                Material newSource = tRelation.SourceMaterial;
                if (isFromDesigner)
                {
                    AddToVisualChangeList(source, (short)LayoutActionType.Delete, source.ACIdentifier);
                    UpdateVisual();
                }
                VBEdge tEdge = VisualEdges.FirstOrDefault(c => c.ACIdentifier == tRelation.ACIdentifier);
                DeleteEdge(materialWF, tRelation, tEdge, false);

                foreach (MaterialWFRelation relation in source.MaterialWFRelation_SourceMaterial.Where(c => c.MaterialWF == materialWF).ToList())
                {
                    Material newTarget = relation.TargetMaterial;
                    if (!materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == newSource && c.TargetMaterial == newTarget))
                    {
                        MaterialWFRelation newRelation = vbDesigner.CreateRelation(materialWF, newSource, newTarget);
                        UpdateDesigner(newRelation, vbDesigner);
                    }
                    VBEdge edge = VisualEdges.FirstOrDefault(c => c.ACIdentifier == relation.ACIdentifier);
                    source.MaterialWFRelation_SourceMaterial.Remove(relation);
                    DeleteEdge(materialWF, relation, edge, false);
                }
                vbDesigner.OnPropertyChanged("DesignXAML");
                return true;
            }
            //material ... can not be deleted
            var msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = vbDesigner.Root.Environment.TranslateMessage(vbDesigner, @"Error50090", source.MaterialNo) };
            vbDesigner.Messages.Msg(msg, Global.MsgResult.OK);
            return false;
        }

        protected bool DeleteEdge(MaterialWF materialWF, MaterialWFRelation relation, VBEdge vbEdge, bool deleteFromVBEdge, bool isFromDesigner = false)
        {
            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return false;

            if (relation == null || vbEdge == null)
                return false;
            Material source = relation.SourceMaterial;
            if (source != null && source.MaterialWFRelation_SourceMaterial.Count(c => c.MaterialWF == materialWF) < 2
                && source.MaterialWFRelation_TargetMaterial.Any(x => x.MaterialWF == materialWF) && deleteFromVBEdge)
            {
                var msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = vbDesigner.Root.Environment.TranslateMessage(vbDesigner, @"Error50091") };
                vbDesigner.Messages.Msg(msg, Global.MsgResult.OK);
                return false;
            }

            if (!isFromDesigner)
            {
                AddToVisualChangeList(relation, (short)LayoutActionType.DeleteEdge, vbEdge.VBConnectorSource, vbEdge.VBConnectorTarget);
                UpdateVisual();
                VisualChangeList.Clear();
                vbDesigner.OnPropertyChanged("DesignerXAML");
            }
            if (materialWF.MaterialWFRelation_MaterialWF.Count() == 1)
            {
                relation.SourceMaterial = relation.TargetMaterial;
            }
            else
            {
                materialWF.MaterialWFRelation_MaterialWF.Remove(relation);
                relation.DeleteACObject(vbDesigner.DatabaseApp, false);
            }
            vbDesigner.UpdateMaterialWFBSO();
            return true;
        }

        #endregion

        #region DesignerTools

        public override IEnumerable<IACObject> GetAvailableTools()
        {
            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return null;

            ACObjectItemList objectLayoutEntrys = new ACObjectItemList();
            objectLayoutEntrys.Add(new DesignManagerToolItem(gip.core.datamodel.Database.Root.Environment.TranslateText(vbDesigner, "Pointer"), PointerTool.Instance, "DesignPointer"));
            objectLayoutEntrys.Add(new DesignManagerToolItem(gip.core.datamodel.Database.Root.Environment.TranslateText(vbDesigner, "Connector"), new ConnectTool(vbDesigner), "DesignConnector"));
            objectLayoutEntrys.Add(new DesignManagerToolItem(gip.core.datamodel.Database.Root.Environment.TranslateText(vbDesigner, "EditPoints"), new DrawingToolEditPoints(), "DesignEditPoints"));
            return objectLayoutEntrys;
        }

        public void Refresh()
        {
            VBDesignerMaterialWF vbDesigner = Designer<VBDesignerMaterialWF>();
            if (vbDesigner == null)
                return;

            if (vbDesigner.VBDesignEditor is VBDesignEditor)
            {
                ((VBDesignEditor)vbDesigner.VBDesignEditor).SaveToXAML();
                ((VBDesignEditor)vbDesigner.VBDesignEditor).RefreshViewFromXAML();
            }
        }

        #endregion

    }
}
