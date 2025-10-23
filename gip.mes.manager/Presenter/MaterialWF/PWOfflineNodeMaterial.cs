using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.manager;
using gip.mes.datamodel;

namespace gip.mes.manager
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'PWOfflineNodeMaterial'}de{'PWOfflineNodeMaterial'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    [ACClassConstructorInfo(
        new object[] 
        { 
            new object[] {"WFContext", Global.ParamOption.Optional, typeof(IACWorkflowContext)},
        }
    )]
    public class PWOfflineNodeMaterial : PWOfflineNode
    {
        #region c´tors
        public PWOfflineNodeMaterial(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _ContentACWorkflow = ParameterValue("WFContext") as IACWorkflowContext;
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (Content is MaterialWF)
            {
                MaterialWF materialWF = Content as MaterialWF;

                foreach (Material childMaterial in materialWF.GetMaterials())
                {
                    if (ACComponentChilds.Any(c => c.Content is Material && ((Material)c.Content).MaterialID == childMaterial.MaterialID))
                        continue;
                    CreateChildPWNode(childMaterial, startChildMode);
                }
            }

            _DefaultBSOMaterialWF = new ACPropertyConfigValue<string>(this, "DefaultBSOMaterialWF", "BSOMaterialWF");
            return base.ACInit(startChildMode);
        }

        public override void CreateChildPWNode(IACWorkflowNode acWorkflowNode, Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            ACActivator.CreateInstance(ComponentClass, acWorkflowNode, this, null, startChildMode, null, acWorkflowNode.ACIdentifier);
        }
        #endregion


        private ACPropertyConfigValue<string> _DefaultBSOMaterialWF;
        [ACPropertyConfig("en{'Default BSO MaterialWF editor'}de{'Standard BSO MaterialWF Editor'}")]
        public string DefaultBSOMaterialWF
        {
            get
            {
                return _DefaultBSOMaterialWF.ValueT;
            }
            set
            {
                _DefaultBSOMaterialWF.ValueT = value;
            }
        }

        IACWorkflowContext _ContentACWorkflow;
        [ACPropertyInfo(2, "", "", "", false)]
        public IACWorkflowContext ContentACWorkflow
        {
            get
            {
                return _ContentACWorkflow;
            }
        }

        [ACPropertyInfo(999, "", "", "", false)]
        public Material ContentMaterial
        {
            get
            {
                if (Content is Material)
                    return Content as Material;
                else
                    return null;
            }
        }

        [ACMethodInteraction("", "en{'Remove WF connection'}de{'Entferne Verbindung zu Steuerschritten'}", 9999, false)]
        public void RemoveMaterialConnection()
        {
            if(ParentACComponent != null && ParentACComponent.ParentACComponent != null & ParentACComponent.ParentACComponent.ParentACComponent != null)
                this.ParentACComponent.ParentACComponent.ParentACComponent.ACUrlCommand("!RemoveMaterialConnection", null);
        }

        public bool IsEnabledRemoveMaterialConnection()
        {
            if (ParentACComponent is PWOfflineNodeMaterial && ParentACComponent.ParentACComponent != null && ParentACComponent.ParentACComponent is VBPresenterMaterialWF)
            {
                VBPresenterMaterialWF presenter = ParentACComponent.ParentACComponent as VBPresenterMaterialWF;
                IACComponent parentComponent = presenter.ParentACComponent;
                if (parentComponent != null && parentComponent.ACIdentifier != DefaultBSOMaterialWF)
                    return false;

                if (!((VBDesignerMaterialWF)presenter.ACUrlCommand("VBDesignerMaterialWF(CurrentDesign)")).IsDesignMode)
                {
                    return true;
                }
            }
            return false;
        }

        #region IACDesignProvider

        /// <summary>Returns a ACClassDesign for presenting itself on the gui</summary>
        /// <param name="acUsage">Filter for selecting designs that belongs to this ACUsages-Group</param>
        /// <param name="acKind">Filter for selecting designs that belongs to this ACKinds-Group</param>
        /// <param name="vbDesignName">Optional: The concrete acIdentifier of the design</param>
        /// <returns>ACClassDesign</returns>
        public override gip.core.datamodel.ACClassDesign GetDesign(Global.ACUsages acUsage, Global.ACKinds acKind, string vbDesignName = "")
        {
            return ((IACClassDesignProvider)Content).GetDesign(acUsage, acKind, vbDesignName);
        }

        #endregion

        //#region IACObject
        //public override object ACUrlCommand(string acUrl, params object[] acParameter)
        //{
        //    object result = base.ACUrlCommand(acUrl, acParameter);
        //    if (result == null && acUrl != "ContentACWorkflow")
        //    {
        //        Material childMaterial = null;
        //        if (((MaterialWF)WFContext).MaterialWFRelation_MaterialWF.Any(x => x.SourceMaterial != null && x.SourceMaterial.ACIdentifier == acUrl))
        //            childMaterial = ((MaterialWF)WFContext).MaterialWFRelation_MaterialWF.FirstOrDefault(c => c.SourceMaterial.ACIdentifier == acUrl).SourceMaterial;
        //        else if (((MaterialWF)WFContext).MaterialWFRelation_MaterialWF.Any(c => c.TargetMaterial.ACIdentifier == acUrl))
        //            childMaterial = ((MaterialWF)WFContext).MaterialWFRelation_MaterialWF.FirstOrDefault(c => c.TargetMaterial.ACIdentifier == acUrl).TargetMaterial;

        //        //if (childMaterial != null)
        //        //{
        //        //    result = ACActivator.CreateInstance(ACType as gip.core.datamodel.ACClass, childMaterial, this, null) as IACObjectWithInit;
        //        //}
        //    }
        //    return result;
        //}
        //#endregion

        #region IACComponentWorkflow
        /// <summary>
        /// XAML-Code for Presentation
        /// </summary>
        /// <value>
        /// XAML-Code for Presentation
        /// </value>
        public override string XAMLDesign
        {
            get
            {
                MaterialWF materialWF = Content as MaterialWF;
                return materialWF?.XAMLDesign;
            }
            set
            {
                MaterialWF materialWF = Content as MaterialWF;
                if (materialWF != null)
                    materialWF.XAMLDesign = value;
            }
        }

        /// <summary>
        /// Root-Workflownode of type PWOfflineNodeMaterial
        /// </summary>
        /// <value>Root-Workflownode of type PWOfflineNodeMaterial</value>
        public override IACComponentPWNode ParentRootWFNode
        {
            get
            {
                if (!(this.ParentACComponent is PWOfflineNodeMaterial))
                    return null;
                PWOfflineNodeMaterial parentPWNode = this.ParentACComponent as PWOfflineNodeMaterial;
                return parentPWNode;
            }
        }

        /// <summary>
        /// Returns the Workflow-Context (MaterialWF) for reading and saving the configuration-data of a workflow.
        /// </summary>
        /// <value>The Workflow-Context</value>
        public override IACWorkflowContext WFContext
        {
            get
            {
                if (Content is MaterialWF)
                    return Content as MaterialWF;
                else if (Content is Material)
                {
                    return ParentRootWFNode.WFContext;
                }
                return null;
            }
        }
        #endregion


        /// <summary>Reference to the definition of this Workflownode.</summary>
        /// <value>Reference to the definition of this Workflownode.</value>
        public override core.datamodel.ACClassWF ContentACClassWF
        {
            get { return null; }
        }       
    }
}
