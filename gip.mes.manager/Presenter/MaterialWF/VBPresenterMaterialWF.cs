using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.manager;

namespace gip.mes.manager
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'VBPresenterMaterialWF'}de{'VBPresenterMaterialWF'}", Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, true, true)]
    public class VBPresenterMaterialWF : VBPresenter
    {
        #region c´tors
        public VBPresenterMaterialWF(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #region Load

        public void Load(IACWorkflowContext acWorkflow)
        {
            if (acWorkflow == null || acWorkflow.RootWFNode == null)
            {
                WFRootContext = null;
                SelectedRootWFNode = null;
                SelectedRootWFNode = null;
                SelectedWFNode = null;
                return;
            }

            ACClass pwNodeMaterialWF = Root.Database.ContextIPlus.GetACType("PWOfflineNodeMaterial");
            if (pwNodeMaterialWF != null)
            {
                ACValueList acValueList = pwNodeMaterialWF.ACParameter;
                acValueList["WFContext"] = acWorkflow;

                IACComponentPWNode presenterACWorkflowNode = ACActivator.CreateInstance(pwNodeMaterialWF, acWorkflow.RootWFNode, this, acValueList, Global.ACStartTypes.Automatic, null, acWorkflow.RootWFNode.ACIdentifier) as IACComponentPWNode;

                WFRootContext = acWorkflow;
                PresenterACWorkflowNode = presenterACWorkflowNode;
                SelectedRootWFNode = presenterACWorkflowNode;
                SelectedWFNode = presenterACWorkflowNode;
            }
        }
        
        #endregion

        public void SelectMaterial(datamodel.Material material)
        {
            if (PresenterACWorkflowNode == null)
                return;

            if (material == null)
            {
                SelectedWFNode = null;
                return;
            }
            
            SelectedWFNode = PresenterACWorkflowNode.ACComponentChilds.FirstOrDefault(c => c.Content is datamodel.Material &&
                                                                                         (c.Content as datamodel.Material).MaterialID == material.MaterialID) as IACComponentPWNode;
        }

        protected override void OnSelectionChanged()
        {
            if (SelectedWFNode is PWOfflineNodeMaterial && ((PWOfflineNodeMaterial)SelectedWFNode).ContentMaterial != null)
                ParentACComponent.ACUrlCommand("!SetSelectedMaterial", new object[] { ((PWOfflineNodeMaterial)SelectedWFNode).ContentMaterial, true });
            base.OnSelectionChanged();
        }

        /// <summary>
        /// ACAction is called when one IACInteractiveObject (Source) wants to inform another IACInteractiveObject (Target) about an relevant interaction-event.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public override void ACAction(ACActionArgs actionArgs)
        {
            if (actionArgs.ElementAction == Global.ElementActionType.DesignModeOff || 
                actionArgs.ElementAction == Global.ElementActionType.DesignModeOn ||
                actionArgs.ElementAction == Global.ElementActionType.VBDesignLoaded)
                ParentACComponent.ACAction(actionArgs);
            base.ACAction(actionArgs);
        }

        /// <summary>
        /// Called from a WPF-Control inside it's ACAction-Method when a relevant interaction-event as occured (e.g. Drag-And-Drop).
        /// </summary>
        /// <param name="targetVBDataObject">The target object that was involved in the interaction event.</param>
        /// <param name="actionArgs">Information about the type of interaction and the source.</param>
        public override void ACActionToTarget(IACInteractiveObject targetVBDataObject, ACActionArgs actionArgs)
        {
            //todo: if design mode on !!
            ParentACComponent.ACActionToTarget(targetVBDataObject, actionArgs);
            base.ACActionToTarget(targetVBDataObject, actionArgs);
        }

        /// <summary>
        /// Called from a WPF-Control when a relevant interaction-event as occured (e.g. Drag-And-Drop) and the related component should check if this interaction-event should be handled.
        /// </summary>
        /// <param name="targetVBDataObject">The target object that was involved in the interaction event.</param>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        /// <returns><c>true</c> if ACAction can be invoked otherwise, <c>false</c>.</returns>
        public override bool IsEnabledACActionToTarget(IACInteractiveObject targetVBDataObject, ACActionArgs actionArgs)
        {
            return ParentACComponent.IsEnabledACActionToTarget(targetVBDataObject, actionArgs);
        }

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"IsEnabledACActionToTarget":
                    result = IsEnabledACActionToTarget((IACInteractiveObject)acParameter[0], (ACActionArgs)acParameter[1]);
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
