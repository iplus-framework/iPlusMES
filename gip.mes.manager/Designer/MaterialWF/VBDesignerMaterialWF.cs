// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using gip.core.datamodel;
using gip.core.autocomponent;
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

        protected IVBComponentVBDesignerMaterialWFProxy WPFProxyWF
        {
            get
            {
                return WPFProxy as IVBComponentVBDesignerMaterialWFProxy;
            }
        }

        #endregion

        #region Modify/Create

        public override bool IsEnabledDoModifyAction(IACWorkflowDesignContext visualMain, IACInteractiveObject dropObject, IACInteractiveObject targetVBDataObject, double x, double y)
        {
            return WPFProxyWF.IsEnabledDoModifyAction(visualMain, dropObject, targetVBDataObject, x, y);
        }

        public override IACWorkflowNode DoModifyAction(IACWorkflowDesignContext visualMain, IACInteractiveObject dropObject, IACInteractiveObject targetVBDataObject, double x, double y)
        {
            return WPFProxyWF.DoModifyAction(visualMain, dropObject, targetVBDataObject, x, y);
        }

        public override bool DoInsertRoot(IACWorkflowDesignContext vbWorkflow, core.datamodel.ACClass methodACClass)
        {
            WPFProxy.ClearVisualChangeList();
            MaterialWF materialWF = vbWorkflow as MaterialWF;

            string xmlDesign = "<vb:VBCanvas Enabled=\"true\" Width=\"600\" Height=\"600\" HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\">\n";
            xmlDesign += string.Format("<vb:VBVisualGroup VBContent=\"{0}\" Height=\"200\" Width=\"200\" Name=\"{1}\" Canvas.Top=\"0\" Canvas.Left=\"0\">\n", materialWF.ACIdentifier, materialWF.MaterialWFNo);
            xmlDesign += "<vb:VBCanvasInGroup>\n</vb:VBCanvasInGroup>\n</vb:VBVisualGroup>\n";
            xmlDesign += "</vb:VBCanvas>\n";
            materialWF.XMLDesign = xmlDesign;
            return true;
        }

        #endregion

        #region MaterialWF

        private Material GetMaterial(IACInteractiveObject material)
        {
            return WPFProxyWF.GetMaterial(material);
        }

        public MaterialWFRelation CreateRelation(MaterialWF materialWF, Material sourceMaterial, Material targetMaterial)
        {
            if (materialWF.MaterialWFRelation_MaterialWF.Any(c => c.SourceMaterial == targetMaterial && c.TargetMaterial == sourceMaterial))
                return null;
            MaterialWFRelation relation = MaterialWFRelation.NewACObject(DatabaseApp, null);
            relation.SourceMaterial = sourceMaterial;
            relation.TargetMaterial = targetMaterial;
            DatabaseApp.Add(relation);
            materialWF.MaterialWFRelation_MaterialWF.Add(relation);
            if (materialWF.MaterialWFRelation_MaterialWF.Count == 1 && relation.SourceMaterial == null)
                relation.SourceMaterial = relation.TargetMaterial;
            return relation;
        }

        public bool CanInsertMaterialWFRelation(MaterialWF materialWF, Material sourceMaterial, Material targetMaterial)
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
            XMLDesign = ChangeMaterialWFName(XMLDesign, oldMaterialWFName, newMaterialWFName);
        }

        public static string ChangeMaterialWFName(string xmlDesign, string oldMaterialWFName, string newMaterialWFName)
        {
            xmlDesign = xmlDesign.Replace("VBContent=\"MaterialWF(" + oldMaterialWFName + ")\"", "VBContent=\"MaterialWF(" + newMaterialWFName + ")\"");
            xmlDesign = xmlDesign.Replace("Name=\"" + oldMaterialWFName + "\"", "Name=\"" + newMaterialWFName + "\"");
            xmlDesign = xmlDesign.Replace("VBConnectorSource=\"" + oldMaterialWFName + "\\", "VBConnectorSource=\"" + newMaterialWFName + "\\");
            xmlDesign = xmlDesign.Replace("VBConnectorTarget=\"" + oldMaterialWFName + "\\", "VBConnectorTarget=\"" + newMaterialWFName + "\\");
            return xmlDesign;
        }

        #endregion  

        #region Sync

        [ACMethodInfo("WF", "en{'Text-Synchronisation'}de{'Text-Synchronisierung'}", 500, true)]
        public void SyncWF()
        {
            WPFProxyWF.SyncWF();
        }

        public bool IsEnabledSyncWF()
        {
            return !OnIsEnabledSave();
        }

        #endregion

        #region Edge creation

        public void PreCreateEdge(MaterialWFRelation materialWFRelation)
        {
            WPFProxyWF.PreCreateEdge(materialWFRelation);
        }

        /// <summary>
        /// Creates a Edge between two points
        /// </summary>
        /// <param name="sourceVBConnector">The source VB connector.</param>
        /// <param name="targetVBConnector">The target VB connector.</param>
        public override void CreateEdge(IVBConnector sourceVBConnector, IVBConnector targetVBConnector)
        {
            WPFProxyWF.CreateEdge(sourceVBConnector, targetVBConnector);
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
            return WPFProxyWF.DeleteItem(item, isFromDesigner);
        }

        public bool DeleteFromBSO(MaterialWF materialWF, MaterialWFRelation materialWFRelation)
        {
            return WPFProxyWF.DeleteFromBSO(materialWF, materialWFRelation);
        }

        public override bool IsEnabledDeleteVBVisual()
        {
            return true;
        }

        public override bool DeleteVBVisual()
        {
            return WPFProxyWF.DeleteVBVisual();
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
            return WPFProxyWF.GetAvailableTools();
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

        public void UpdateMaterialWFBSO()
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
            WPFProxyWF.Refresh();
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


