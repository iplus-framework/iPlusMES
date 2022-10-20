using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.layoutengine;
using gip.core.layoutengine.Helperclasses;
using gip.ext.designer.Services;
using gip.ext.design;
using gip.core.manager;
using gip.mes.datamodel;


namespace gip.mes.manager
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Materialworkflow Designer'}de{'Materialworkflow Designer'}", Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, false, false)]
    public partial class VBDesignerMaterialWF : VBDesignerWorkflow
    {
        #region c'tors

        /// <summary>
        /// Konstruktor beim Partslist verwenden
        /// </summary>
        public VBDesignerMaterialWF(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            ShowXMLEditor = true;
            IsToolSelection = false;

            if (!base.ACInit(startChildMode))
                return false;

            InitDesignManager(Const.VBPresenter_SelectedRootWFNode);
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            bool result = base.ACDeInit(deleteACClassTask);
            //_SelectedAvailableElement = null;
            //_CurrentAvailableElement = null;
            DeInitDesignManager(Const.VBPresenter_SelectedRootWFNode);
            return result;
        }

        #endregion

        #region DB

        private DatabaseApp _ParentDatabase = null;
        /// <summary>Returns the shared Database-Context for BSO's by calling GetAppContextForBSO()</summary>
        /// <value>Returns the shared Database-Context.</value>
        public DatabaseApp DatabaseApp
        {
            get
            {
                if (_ParentDatabase != null)
                    return _ParentDatabase;
                _ParentDatabase = this.GetAppContextForBSO();
                return _ParentDatabase;
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

        #region Visual items/edges

        public IEnumerable<VBVisual> VisualItems
        {
            get
            {

                if (VBDesignEditor != null)
                    return ((VBDesignEditor)VBDesignEditor).DesignContext.Services.Component.DesignItems.Where(c => c.View is VBVisual && c.Parent != null).Select(x => x.View as VBVisual);
                return null;
            }
        }

        public IEnumerable<VBEdge> VisualEdges
        {
            get
            {
                return ((VBDesignEditor)VBDesignEditor).DesignContext.Services.Component.DesignItems.Where(c => c.View is VBEdge).Select(x => x.View as VBEdge);
            }
        }

        #endregion

        #region Modify/Create

        public override bool IsEnabledDoModifyAction(IACWorkflowDesignContext visualMain, IACInteractiveObject dropObject, IACInteractiveObject targetVBDataObject, double x, double y)
        {
            if (!IsDesignMode)
                return false;
            MaterialWF materialWF = visualMain as MaterialWF;

            if (targetVBDataObject is VBVisualGroup && !materialWF.MaterialWFRelation_MaterialWF.Any())
                return true;
            else if (targetVBDataObject is VBVisual)
            {
                Material source = GetMaterial(dropObject);
                Material target = GetMaterial(targetVBDataObject);
                if (source != target && !materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == source && c.TargetMaterial == target))
                    if (CanInsertMaterialWFRelation(materialWF, source, target))
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

        public override IACWorkflowNode DoModifyAction(IACWorkflowDesignContext visualMain, IACInteractiveObject dropObject, IACInteractiveObject targetVBDataObject, double x, double y)
        {
            if (!IsDesignMode)
                return null;
            MaterialWF materialWF = visualMain as MaterialWF;
            if (materialWF == null)
                return null;

            IEnumerable<Material> MaterialList = materialWF.GetMaterials();
            MaterialWFRelation relation = null, relation1 = null;
            VisualChangeList.Clear();

            if (targetVBDataObject is VBVisualGroup && !materialWF.MaterialWFRelation_MaterialWF.Any())
            {
                relation = CreateRelation(materialWF, null, GetMaterial(dropObject));
            }
            else if ((targetVBDataObject is VBVisualGroup || targetVBDataObject is VBVisual) && (materialWF.MaterialWFRelation_MaterialWF.Count() == 1) && materialWF.GetMaterials().Count() == 1)
            {
                relation = materialWF.MaterialWFRelation_MaterialWF.FirstOrDefault();
                relation.SourceMaterial = GetMaterial(dropObject);
            }
            else if (targetVBDataObject is VBVisual)
            {
                relation = CreateRelation(materialWF, GetMaterial(dropObject), GetMaterial(targetVBDataObject));
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
                    var msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, @"Error50058", newMaterial.MaterialNo) };
                    Messages.Msg(msg, Global.MsgResult.OK);
                    return null;
                }

                DeleteEdge(materialWF, materialWF.MaterialWFRelation_MaterialWF.FirstOrDefault(c => c.SourceMaterial == sourceMaterial && c.TargetMaterial == targetMaterial), ((VBEdge)targetVBDataObject),false);

                if (!materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == sourceMaterial && c.TargetMaterial == newMaterial))
                    relation = CreateRelation(materialWF, sourceMaterial, newMaterial);

                if (!materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == newMaterial && c.TargetMaterial == targetMaterial))
                    relation1 = CreateRelation(materialWF, newMaterial, targetMaterial);
            }

            if (relation != null && relation.SourceMaterialID == relation.TargetMaterialID && materialWF.MaterialWFRelation_MaterialWF.Count > 1)
            {
                var msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, @"Error50058", relation.SourceMaterial.MaterialNo) };
                Messages.Msg(msg, Global.MsgResult.OK);
                return null;
            }

            if(relation != null)
                UpdateDesigner(relation, this, new Rect(new Point(x,y), new Size(150,50)));

            if (relation1 != null)
                UpdateDesigner(relation1, this);

            UpdateMaterialWFBSO();

            ((IToolService)ToolService).CurrentTool = ((IToolService)ToolService).PointerTool;
            Refresh();
            return null;
        }

        public override bool DoInsertRoot(IACWorkflowDesignContext vbWorkflow, core.datamodel.ACClass methodACClass)
        {
            VisualChangeList.Clear();
            MaterialWF materialWF = vbWorkflow as MaterialWF;

            string xmlDesign = "<vb:VBCanvas Enabled=\"true\" Width=\"600\" Height=\"600\" HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\">\n";
            xmlDesign += string.Format("<vb:VBVisualGroup VBContent=\"{0}\" Height=\"200\" Width=\"200\" Name=\"{1}\" Canvas.Top=\"0\" Canvas.Left=\"0\">\n", materialWF.ACIdentifier, materialWF.MaterialWFNo);
            xmlDesign += "<vb:VBCanvasInGroup>\n</vb:VBCanvasInGroup>\n</vb:VBVisualGroup>\n";
            xmlDesign += "</vb:VBCanvas>\n";
            materialWF.XMLDesign = xmlDesign;
            return true;
        }

        public override DesignItem CreateVBVisualDesignItem(VisualInfo visualInfo, IACWorkflowNode acVisualWF, DesignContext designContext, out DesignItem designItemParent)
        {
            DesignItem item = base.CreateVBVisualDesignItem(visualInfo, acVisualWF, designContext, out designItemParent);
            IACWorkflowContext vbWorkflow = CurrentDesign as IACWorkflowContext;
            if (vbWorkflow == null)
                return item;
            WFLayoutCalculator.LayoutMaterialWF(vbWorkflow, designItemParent, item, visualInfo.LayoutAction);
            return item;
        }

        #endregion

        #region MaterialWF

        private Material GetMaterial(IACInteractiveObject material)
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

        private MaterialWFRelation CreateRelation(MaterialWF materialWF, Material sourceMaterial, Material targetMaterial)
        {
            if (materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == targetMaterial && c.TargetMaterial == sourceMaterial))
                return null;
            MaterialWFRelation relation = MaterialWFRelation.NewACObject(DatabaseApp, null);
            relation.SourceMaterial = sourceMaterial;
            relation.TargetMaterial = targetMaterial;
            materialWF.MaterialWFRelation_MaterialWF.Add(relation);
            if (materialWF.MaterialWFRelation_MaterialWF.Count == 1 && relation.SourceMaterial == null)
                relation.SourceMaterial = relation.TargetMaterial;
            return relation;
        }

        private bool CanInsertMaterialWFRelation(MaterialWF materialWF, Material sourceMaterial, Material targetMaterial)
        {
            bool canInsert = true;
            if (sourceMaterial == targetMaterial)
                return false;

            if (sourceMaterial != targetMaterial && materialWF.MaterialWFRelation_MaterialWF.Count == 1)
                return true;

            foreach (MaterialWFRelation relation in targetMaterial.MaterialWFRelation_SourceMaterial.Where(c => c.MaterialWF == materialWF))
            {
                if (relation.TargetMaterialID == targetMaterial.MaterialID)
                    continue;
                if(!CanInsertMaterialWFRelation(materialWF, sourceMaterial, relation.TargetMaterial))
                    canInsert = false;
            }
            return canInsert;
        }

        public void ChangeMaterialWFName(string oldMaterialWFName, string newMaterialWFName)
        {
            XMLDesign = XMLDesign.Replace("VBContent=\"MaterialWF(" + oldMaterialWFName + ")\"", "VBContent=\"MaterialWF(" + newMaterialWFName + ")\"");
            XMLDesign = XMLDesign.Replace("Name=\"" + oldMaterialWFName + "\"", "Name=\"" + newMaterialWFName + "\"");
            XMLDesign = XMLDesign.Replace("VBConnectorSource=\"" + oldMaterialWFName + "\\", "VBConnectorSource=\"" + newMaterialWFName + "\\");
            XMLDesign = XMLDesign.Replace("VBConnectorTarget=\"" + oldMaterialWFName + "\\", "VBConnectorTarget=\"" + newMaterialWFName + "\\");
        }

        #endregion  

        #region Sync

        [ACMethodInfo("WF", "en{'Text-Synchronisation'}de{'Text-Synchronisierung'}", 500, true)]
        public void SyncWF()
        {
            MaterialWF materialWF = CurrentDesign as MaterialWF;

            if (materialWF == null || !materialWF.MaterialWFRelation_MaterialWF.Any())
                return;

            VisualChangeList.Clear();
            foreach (VBVisual item in VisualItems)
            {
                Material src = ((PWOfflineNodeMaterial)item.ContextACObject).ContentMaterial;
                if (src != null)
                    AddToVisualChangeList(src, LayoutActionType.Delete, src.ACIdentifier);
            }

            foreach (VBEdge edge in VisualEdges)
                 AddToVisualChangeList(new MaterialWFRelation(), LayoutActionType.DeleteEdge, edge.VBConnectorSource, edge.VBConnectorTarget);

            UpdateVisual();

            MaterialWFRelation firstRelation = materialWF.MaterialWFRelation_MaterialWF.FirstOrDefault(c => !c.TargetMaterial.MaterialWFRelation_SourceMaterial.Any(x => x.MaterialWF == materialWF));
            Guid materialID = firstRelation.SourceMaterialID;
            firstRelation.SourceMaterial = null;
            UpdateDesigner(firstRelation, this);
            firstRelation.SourceMaterial = DatabaseApp.Material.FirstOrDefault(c => c.MaterialID == materialID);
            UpdateDesigner(firstRelation, this);
            foreach (MaterialWFRelation relation in materialWF.MaterialWFRelation_MaterialWF.Where(c => c != firstRelation).OrderBy(c => c.Sequence))
                UpdateDesigner(relation, this);
            
            ((VBDesignEditor)VBDesignEditor).DesignSurface.UpdateLayout();
        }

        public bool IsEnabledSyncWF()
        {
            return !OnIsEnabledSave();
        }

        #endregion

        #region Edge creation

        public void PreCreateEdge(MaterialWFRelation materialWFRelation)
        {
            VBVisual source = VisualItems.FirstOrDefault(c => c.VBContent == materialWFRelation.SourceMaterial.ACIdentifier);
            VBVisual target = VisualItems.FirstOrDefault(c => c.VBContent == materialWFRelation.TargetMaterial.ACIdentifier);
            VBConnector sourceConn = VBLogicalTreeHelper.FindChildObjectInLogicalTree(source, "PWPointOut") as VBConnector;
            VBConnector targetConn = VBLogicalTreeHelper.FindChildObjectInLogicalTree(target, "PWPointIn") as VBConnector;
            ((MaterialWF)CurrentDesign).FromNode = materialWFRelation.SourceMaterial;
            ((MaterialWF)CurrentDesign).ToNode = materialWFRelation.TargetMaterial;
            CreateEdge(sourceConn, targetConn);
        }

        /// <summary>
        /// Creates a Edge between two points
        /// </summary>
        /// <param name="sourceVBConnector">The source VB connector.</param>
        /// <param name="targetVBConnector">The target VB connector.</param>
        public override void CreateEdge(IVBConnector sourceVBConnector, IVBConnector targetVBConnector)
        {
            VisualChangeList.Clear();
            
            IACWorkflowNode sourceVBVisual = sourceVBConnector.ParentACObject as IACWorkflowNode;
            if (sourceVBVisual == null)
            {
                if (sourceVBConnector.ParentACObject != null && sourceVBConnector.ParentACObject is PWOfflineNodeMaterial)
                    sourceVBVisual = ((PWOfflineNodeMaterial)sourceVBConnector.ParentACObject).ContentMaterial as IACWorkflowNode;
                else if(sourceVBConnector is VBConnector)
                {
                    VBConnector sourceVBConn = sourceVBConnector as VBConnector;
                        if(sourceVBConn.DataContext != null && sourceVBConn.DataContext is PWOfflineNodeMaterial)
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


            WFCreateWFEdge(sourceVBVisual, sourceACClassProperty, targetVBVisual, targetACClassProperty);
            UpdateVisual();
            VisualChangeList.Clear();
            OnPropertyChanged("DesignXAML");
        }

        /// <summary>
        /// Checks if a Edge can be created between two points
        /// </summary>
        /// <param name="sourceVBConnector">The source VB connector.</param>
        /// <param name="targetVBConnector">The target VB connector.</param>
        /// <returns><c>true</c> if is enabled; otherwise, <c>false</c>.</returns>
        public override bool IsEnabledCreateEdge(IVBConnector sourceVBConnector, IVBConnector targetVBConnector)
        {
            if (!IsDesignMode)
                return false;
            Material source = ((PWOfflineNodeMaterial)sourceVBConnector.ParentACObject).ContentMaterial;
            Material target = (((PWOfflineNodeMaterial)targetVBConnector.ParentACObject).ContentMaterial);
            MaterialWF materialWF = CurrentDesign as MaterialWF;
            if (source != null && target != null && materialWF != null)
            {
                if (!materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == source && c.TargetMaterial == target) && source != target)
                    return true;
            }
            return false;
        }


        /// <summary>Asks this design manager if he can create edges</summary>
        /// <returns><c>true</c> if this instance can create edges; otherwise, <c>false</c>.</returns>
        public override bool CanManagerCreateEdges()
        {
            return true;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Removes a WPF-Element from the design
        /// </summary>
        /// <param name="item">Item for delete.</param>
        /// <param name="isFromDesigner">If true, then call is invoked from this manager, else from gui</param>
        /// <returns><c>true</c> if is removed; otherwise, <c>false</c>.</returns>
        public override bool DeleteItem(object item, bool isFromDesigner = true)
        {
            VisualChangeList.Clear();
            MaterialWF materialWF = CurrentDesign as MaterialWF;
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
                    AddToVisualChangeList(source, LayoutActionType.Delete, source.ACIdentifier);
                    UpdateVisual();
                }
                DeleteEdge(materialWF, relation, vbEdge, true, true);
                OnPropertyChanged("DesignXAML");
                return true;
            }
            return false;
        }

        public bool DeleteFromBSO(MaterialWF materialWF, MaterialWFRelation materialWFRelation)
        {
            VBEdge vbEdge = VisualEdges.FirstOrDefault(c => c.ACIdentifier == materialWFRelation.ACIdentifier);
            return DeleteEdge(materialWF, materialWFRelation, vbEdge, false, false);
        }

        public override bool IsEnabledDeleteVBVisual()
        {
            return true;
        }

        public override bool DeleteVBVisual()
        {
            if (!IsEnabledDeleteVBVisual())
                return false;
            MaterialWF materialWF = CurrentDesign as MaterialWF;

            if (CurrentContentACObject is PWOfflineNodeMaterial)
            {
                Material material = ((PWOfflineNodeMaterial)CurrentContentACObject).ContentMaterial;
                DeleteItem(VisualItems.FirstOrDefault(c => c.VBContent == material.ACIdentifier), true);
            }
            else if (SelectedVBControl is VBEdge)
            {
                MaterialWFRelation relation = materialWF.MaterialWFRelation_MaterialWF.FirstOrDefault(c => c.ACIdentifier == SelectedVBControl.ACIdentifier);
                if (relation == null)
                    return false;
                Material source = relation.SourceMaterial;
                if (source.MaterialWFRelation_SourceMaterial.Count(c => c.MaterialWF == materialWF) + source.MaterialWFRelation_TargetMaterial.Count(c => c.MaterialWF == materialWF) == 1)
                    AddToVisualChangeList(source, LayoutActionType.Delete, source.ACIdentifier);
                DeleteEdge(materialWF, relation, SelectedVBControl as VBEdge, true);
            }
            Refresh();
            return false;
        }

        protected bool DeleteWF(MaterialWF materialWF, VBVisual vbVisual, Material source, bool isFromDesigner)
        {
            if (!source.MaterialWFRelation_TargetMaterial.Any(c => c.MaterialWF == materialWF))
            {
                if (isFromDesigner)
                    AddToVisualChangeList(source, LayoutActionType.Delete, source.ACIdentifier);
                foreach (MaterialWFRelation relation in source.MaterialWFRelation_SourceMaterial.Where(c => c.MaterialWF == materialWF).ToList())
                {
                    VBEdge edge = VisualEdges.FirstOrDefault(c => c.ACIdentifier == relation.ACIdentifier);
                    DeleteEdge(materialWF, relation, edge, false);
                }

                OnPropertyChanged("DesignXAML");
                return true;
            }
            else if (source.MaterialWFRelation_TargetMaterial.Any(c => c.MaterialWF == materialWF) && source.MaterialWFRelation_SourceMaterial.Count(c => c.MaterialWF == materialWF) == 1)
            {
                MaterialWFRelation sRelation = source.MaterialWFRelation_SourceMaterial.FirstOrDefault(c => c.MaterialWF == materialWF);
                Material newTarget = sRelation.TargetMaterial;
                if (isFromDesigner)
                {
                    AddToVisualChangeList(source, LayoutActionType.Delete, source.ACIdentifier);
                    UpdateVisual();
                }
                VBEdge sEdge = VisualEdges.FirstOrDefault(c => c.ACIdentifier == sRelation.ACIdentifier);
                DeleteEdge(materialWF, sRelation, sEdge, false);

                foreach (MaterialWFRelation relation in source.MaterialWFRelation_TargetMaterial.Where(c => c.MaterialWF == materialWF).ToList())
                {
                    Material newSource = relation.SourceMaterial;
                    if (!materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == newSource && c.TargetMaterial == newTarget))
                    {
                        MaterialWFRelation newRelation = CreateRelation(materialWF, newSource, newTarget);
                        UpdateDesigner(newRelation, this);
                    }
                    VBEdge edge = VisualEdges.FirstOrDefault(c => c.ACIdentifier == relation.ACIdentifier);
                    source.MaterialWFRelation_TargetMaterial.Remove(relation);
                    DeleteEdge(materialWF, relation, edge, false);
                }
                OnPropertyChanged("DesignXAML");
                return true;
            }
            else if (source.MaterialWFRelation_SourceMaterial.Any(c => c.MaterialWF == materialWF) && source.MaterialWFRelation_TargetMaterial.Count(c => c.MaterialWF == materialWF) == 1)
            {
                MaterialWFRelation tRelation = source.MaterialWFRelation_TargetMaterial.FirstOrDefault(c => c.MaterialWF == materialWF);
                Material newSource = tRelation.SourceMaterial;
                if (isFromDesigner)
                {
                    AddToVisualChangeList(source, LayoutActionType.Delete, source.ACIdentifier);
                    UpdateVisual();
                }
                VBEdge tEdge = VisualEdges.FirstOrDefault(c => c.ACIdentifier == tRelation.ACIdentifier);
                DeleteEdge(materialWF, tRelation, tEdge, false);

                foreach (MaterialWFRelation relation in source.MaterialWFRelation_SourceMaterial.Where(c => c.MaterialWF == materialWF).ToList())
                {
                    Material newTarget = relation.TargetMaterial;
                    if (!materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == newSource && c.TargetMaterial == newTarget))
                    {
                        MaterialWFRelation newRelation = CreateRelation(materialWF, newSource, newTarget);
                        UpdateDesigner(newRelation, this);
                    }
                    VBEdge edge = VisualEdges.FirstOrDefault(c => c.ACIdentifier == relation.ACIdentifier);
                    source.MaterialWFRelation_SourceMaterial.Remove(relation);
                    DeleteEdge(materialWF, relation, edge, false);
                }
                OnPropertyChanged("DesignXAML");
                return true;
            }
            //material ... can not be deleted
            var msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, @"Error50090", source.MaterialNo) };
            Messages.Msg(msg, Global.MsgResult.OK);
            return false;
        }

        protected bool DeleteEdge(MaterialWF materialWF, MaterialWFRelation relation, VBEdge vbEdge, bool deleteFromVBEdge, bool isFromDesigner=false)
        {
            if (relation == null || vbEdge == null)
                return false;
            Material source = relation.SourceMaterial;
            if (source != null && source.MaterialWFRelation_SourceMaterial.Count(c => c.MaterialWF == materialWF) < 2 
                && source.MaterialWFRelation_TargetMaterial.Any(x => x.MaterialWF == materialWF) && deleteFromVBEdge)
            {
                var msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, @"Error50091") };
                Messages.Msg(msg, Global.MsgResult.OK);
                return false;
            }

            if (!isFromDesigner)
            {
                AddToVisualChangeList(relation, LayoutActionType.DeleteEdge, vbEdge.VBConnectorSource, vbEdge.VBConnectorTarget);
                UpdateVisual();
                VisualChangeList.Clear();
                OnPropertyChanged("DesignerXAML");
            }
            if (materialWF.MaterialWFRelation_MaterialWF.Count() == 1)
            {
                relation.SourceMaterial = relation.TargetMaterial;
            }
            else 
            {
                materialWF.MaterialWFRelation_MaterialWF.Remove(relation);
                relation.DeleteACObject(DatabaseApp, false);
            }
            UpdateMaterialWFBSO();
            return true;
        }

        #endregion

        #region XAML design

        public override string DesignXAML
        {
            get
            {
                if (CurrentDesign == null)
                    return "";

                return CurrentDesign.XMLDesign;
            }
            set
            {
                if (CurrentDesign == null)
                    return;

                if (CurrentDesign.XMLDesign != value)
                {
                    CurrentDesign.XMLDesign = value;
                    OnPropertyChanged("DesignXAML");
                }
            }
        }

        /// <summary>
        /// XAML-Code for Presentation
        /// </summary>
        /// <value>
        /// XAML-Code for Presentation
        /// </value>
        public override string XMLDesign
        {
            get
            {
                return DesignXAML;
            }
            set
            {
                DesignXAML = value;
            }
        }

        #endregion

        #region Designer tools

        public override IEnumerable<IACObject> GetAvailableTools()
        {
            ACObjectItemList objectLayoutEntrys = new ACObjectItemList();
            objectLayoutEntrys.Add(new DesignManagerToolItem(gip.core.datamodel.Database.Root.Environment.TranslateText(this, "Pointer"), PointerTool.Instance, "DesignPointer"));
            objectLayoutEntrys.Add(new DesignManagerToolItem(gip.core.datamodel.Database.Root.Environment.TranslateText(this, "Connector"), new ConnectTool(this), "DesignConnector"));
            objectLayoutEntrys.Add(new DesignManagerToolItem(gip.core.datamodel.Database.Root.Environment.TranslateText(this, "EditPoints"), new DrawingToolEditPoints(), "DesignEditPoints"));
            return objectLayoutEntrys;
        }

        [ACPropertyList(9999, "MaterialWFElement")]
        new public ObservableCollection<IACObject> AvailableElementList
        {
            get
            {
                _AvailableElementList.Clear();
                foreach (Material material in DatabaseApp.Material.Where(c => c.IsIntermediate).OrderBy(c => c.MaterialNo))
                    _AvailableElementList.Add(new ACObjectItem(new NodeInfo(material.PWACClass, material), String.Format("{0} {1}", material.MaterialNo, material.MaterialName1)));
                
                return _AvailableElementList;
            }
        }

        #endregion

        #region Update methods

        protected override void UpdateVisual()
        {
            base.UpdateVisual();
        }

        private void UpdateDesigner(MaterialWFRelation materialWFRelation, ACBSO bso, Rect pos = new Rect())
        {
            VisualChangeList.Clear();
            VBDesignEditor vbDesignEditor = VBDesignEditor as VBDesignEditor;
            if (vbDesignEditor != null)
            {
                DesignContext designContext = vbDesignEditor.DesignSurface.DesignContext;
                if (designContext == null)
                    return;

                if (!VisualItems.Any())
                {
                    AddToVisualChangeList(materialWFRelation.TargetMaterial, LayoutActionType.Insert, materialWFRelation.TargetMaterial.GetACUrl(materialWFRelation.MaterialWF), "", pos);
                    UpdateVisual();
                }
                else if (!VisualItems.Any(c => c.VBContent == materialWFRelation.SourceMaterial.ACIdentifier))
                {
                    VisualChangeList.Clear();
                    AddToVisualChangeList(materialWFRelation.SourceMaterial, LayoutActionType.Insert, materialWFRelation.SourceMaterial.GetACUrl(materialWFRelation.MaterialWF));
                    UpdateVisual();
                    PreCreateEdge(materialWFRelation);
                }
                else if (!VisualItems.Any(c => c.VBContent == materialWFRelation.TargetMaterial.ACIdentifier))
                {
                    VisualChangeList.Clear();
                    AddToVisualChangeList(materialWFRelation.TargetMaterial, LayoutActionType.Insert, materialWFRelation.TargetMaterial.GetACUrl(materialWFRelation.MaterialWF));
                    UpdateVisual();
                    PreCreateEdge(materialWFRelation);
                }
                else
                {
                    PreCreateEdge(materialWFRelation);
                }
                ((MaterialWF)CurrentDesign).FromNode = null;
                ((MaterialWF)CurrentDesign).ToNode = null;
            }
        }

        private void UpdateMaterialWFBSO()
        {
            var bso = this.ParentACComponent.ParentACComponent;
            bso.OnPropertyChanged("MixureList");
            bso.OnPropertyChanged("MaterialList");
        }

        public override void UpdateAvailableElements()
        {
        }

        private void Refresh()
        {
            if (VBDesignEditor is VBDesignEditor)
            {
                ((VBDesignEditor)VBDesignEditor).SaveToXAML();
                ((VBDesignEditor)VBDesignEditor).RefreshViewFromXAML();
            }
        }

        #endregion

        #region DesignMode on/off

        /// <summary>
        /// Switches the this designer to design mode and the Designer-Tool (WPF-Control) appears on the gui.
        /// </summary>
        /// <param name="dockingManagerName">Name of the parent docking manager.</param>
        public override void ShowDesignManager(string dockingManagerName = "")
        {
            if (GetWindow("ToolWindow") == null)
            {
                ShowWindow(this, "ToolWindow", false, Global.VBDesignContainer.DockableWindow, Global.VBDesignDockState.AutoHideButton, Global.VBDesignDockPosition.Left, Global.ControlModes.Hidden, dockingManagerName, Global.ControlModes.Hidden);
                base.ShowDesignManager(dockingManagerName);
            }
        }

        /// <summary>
        /// Switches the designer off and the Designer-Tool (WPF-Control) disappears on the gui.
        /// </summary>
        public override void HideDesignManager()
        {
            IACObject window = GetWindow("ToolWindow");
            if (window != null)
                CloseDockableWindow(window);
            base.HideDesignManager();

        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"IsEnabledDoModifyAction":
                    result = IsEnabledDoModifyAction(acParameter[0] as IACWorkflowDesignContext, (IACInteractiveObject)acParameter[1], (IACInteractiveObject)acParameter[2], (Double)acParameter[3], (Double)acParameter[4]);
                    return true;
                case"SyncWF":
                    SyncWF();
                    return true;
                case"IsEnabledSyncWF":
                    result = IsEnabledSyncWF();
                    return true;
                case"IsEnabledCreateEdge":
                    result = IsEnabledCreateEdge((IVBConnector)acParameter[0], (IVBConnector)acParameter[1]);
                    return true;
                case"IsEnabledDeleteVBVisual":
                    result = IsEnabledDeleteVBVisual();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}


