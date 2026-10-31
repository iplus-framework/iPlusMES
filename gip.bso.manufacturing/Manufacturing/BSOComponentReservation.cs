// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.autocomponent;
using gip.core.autocomponent;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Business object or App (dialog) for displaying the open facility reservations of a module/component.
    /// It is not a standalone search/edit app: it is opened as a dialog by another component (e.g. a process module
    /// or work center selector) via ShowReservationDialog(component). The passed component is stored in the
    /// SelectedACComp property and its open reservations (InOrderPos not delivered, production order positions/batch
    /// plans not finished) are provided by the FacilityReservationList property; the selected reservation is in the
    /// SelectedFacilityReservation property.
    /// ShowOrder() opens the order behind the selected reservation: for reservations on an InOrderPos the incoming
    /// delivery note dialog (BSOInDeliveryNote(Dialog)) is opened, for reservations on a ProdOrderPartslistPos or
    /// ProdOrderBatchPlan the production order dialog (BSOProdOrder(Dialog)). The dialog BSO name can be overridden
    /// by the hosting component via GetBSONameForShowOrder().
    /// DialogOK()/DialogCancel() close the dialog with the corresponding result (DialogResult property).
    /// This BSO is read-only; there are no New/Save/Delete entity workflows.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOComponentReservation&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Module reservations'}de{'Modul-Reservierung'}", Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, true, false,
        Description = @"Business object or App (dialog) for displaying the open facility reservations of a module/component.
        It is not a standalone search/edit app: it is opened as a dialog by another component (e.g. a process module or work center selector) via ShowReservationDialog(component). The passed component is stored in the SelectedACComp property and its open reservations (InOrderPos not delivered, production order positions/batch plans not finished) are provided by the FacilityReservationList property; the selected reservation is in the SelectedFacilityReservation property.
        ShowOrder() opens the order behind the selected reservation: for reservations on an InOrderPos the incoming delivery note dialog (BSOInDeliveryNote(Dialog)) is opened, for reservations on a ProdOrderPartslistPos or ProdOrderBatchPlan the production order dialog (BSOProdOrder(Dialog)). The dialog BSO name can be overridden by the hosting component via GetBSONameForShowOrder().
        DialogOK()/DialogCancel() close the dialog with the corresponding result (DialogResult property).
        This BSO is read-only; there are no New/Save/Delete entity workflows.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOComponentReservation&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOComponentReservation : ACBSOvb
    {

        #region c´tors
        public BSOComponentReservation(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            return base.OnGetControlModes(vbControl);
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._SelectedACComp = null;
            this._SelectedFacilityReservation = null;
            return await base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Queries
        public static readonly Func<DatabaseApp, Guid, IEnumerable<FacilityReservation>> s_cQry_OpenReservations =
            EF.CompileQuery<DatabaseApp, Guid, IEnumerable<FacilityReservation>>(
                (ctx, acClassID) => ctx.FacilityReservation
                            .Include(InOrderPos.ClassName)
                            .Include("InOrderPos.DeliveryNotePos_InOrderPos")
                            .Include("InOrderPos.MDDelivPosLoadState")
                            .Include("InOrderPos.Material")
                            .Include(ProdOrderPartslistPos.ClassName)
                            .Include("ProdOrderPartslistPos.MDProdOrderPartslistPosState")
                            .Include("ProdOrderPartslistPos.ProdOrderPartslist")
                            .Include("ProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material")
                            .Include("ProdOrderPartslistPos.ProdOrderPartslist.MDProdOrderState")
                            .Include("ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder")
                            .Include("ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.MDProdOrderState")
                            .Include(ProdOrderBatchPlan.ClassName)
                            .Include("ProdOrderBatchPlan.ProdOrderPartslistPos")
                            .Include("ProdOrderBatchPlan.ProdOrderPartslistPos.MDProdOrderPartslistPosState")
                            .Include("ProdOrderBatchPlan.ProdOrderPartslist")
                            .Include("ProdOrderBatchPlan.ProdOrderPartslist.Partslist.Material")
                            .Include("ProdOrderBatchPlan.ProdOrderPartslist.MDProdOrderState")
                            .Include("ProdOrderBatchPlan.ProdOrderPartslist.ProdOrder")
                            .Include("ProdOrderBatchPlan.ProdOrderPartslist.ProdOrder.MDProdOrderState")
                            .Include(Facility.ClassName)
                            .Where(c => c.VBiACClassID.HasValue && c.VBiACClassID == acClassID && (
                                    (c.InOrderPosID.HasValue
                                        && c.InOrderPos.MDDelivPosState.MDDelivPosStateIndex != (short)MDDelivPosState.DelivPosStates.Delivered)
                             || (c.ProdOrderPartslistPosID.HasValue
                                    && c.ProdOrderPartslistPos.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex < (short)MDProdOrderState.ProdOrderStates.ProdFinished
                                    && c.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex < (short)MDProdOrderState.ProdOrderStates.ProdFinished
                                    && (c.ProdOrderPartslistPos.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex < (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                       || c.ProdOrderPartslistPos.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex > (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled))
                             || (c.ProdOrderBatchPlanID.HasValue
                                    && c.ProdOrderBatchPlan.ProdOrderPartslistPosID.HasValue
                                    && c.ProdOrderBatchPlan.ProdOrderPartslistPos.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex < (short)MDProdOrderState.ProdOrderStates.ProdFinished
                                    && c.ProdOrderBatchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex < (short)MDProdOrderState.ProdOrderStates.ProdFinished
                                    && c.ProdOrderBatchPlan.PlanStateIndex < (short)GlobalApp.BatchPlanState.Completed)
                                    ))
            );

        #endregion

        #region BSO->ACProperty

        private IACComponent _SelectedACComp;
        public IACComponent SelectedACComp
        {
            get
            {
                return _SelectedACComp;
            }
        }


        FacilityReservation _SelectedFacilityReservation;
        [ACPropertySelected(500, FacilityReservation.ClassName, Description = @"Currently selected facility reservation in the reservation list; target of ShowOrder().")]
        public FacilityReservation SelectedFacilityReservation
        {
            get
            {
                return _SelectedFacilityReservation;
            }
            set
            {
                _SelectedFacilityReservation = value;
                OnPropertyChanged("SelectedFacilityReservation");
            }
        }

        [ACPropertyList(501, FacilityReservation.ClassName, Description = @"Open facility reservations of the component passed to ShowReservationDialog(): reservations on incoming order positions not yet delivered, on production order positions not yet finished/cancelled and on batch plans not yet completed.")]
        public IEnumerable<FacilityReservation> FacilityReservationList
        {
            get
            {
                if (SelectedACComp == null)
                    return null;
                return s_cQry_OpenReservations(DatabaseApp, SelectedACComp.ACType.ACTypeID).ToArray();
            }
        }

        #endregion

        #region BSO->ACMethod
        [ACMethodInfo("", "", 590, Description = @"Returns the name of the dialog BSO to use for ShowOrder(). Can be overridden by the hosting component to redirect the navigation to a derived order dialog; the default returns the passed defaultBSOName unchanged.")]
        public virtual string GetBSONameForShowOrder(string defaultBSOName)
        {
            return defaultBSOName;
        }


        [ACMethodInteraction(FacilityReservation.ClassName, "en{'View order'}de{'Auftrag anzeigen'}", (short)MISort.BringToFront, true, "SelectedFacilityReservation", Global.ACKinds.MSMethodPrePost, false, Global.ContextMenuCategory.ProdPlanLog, Description = @"Opens the order behind the selected reservation: the incoming delivery note dialog for InOrderPos reservations, otherwise the production order dialog (ProdOrderPartslistPos or ProdOrderBatchPlan reservations).")]
        public void ShowOrder()
        {
            if (!PreExecute("Delete")) 
                return;

            if (SelectedFacilityReservation.InOrderPos != null)
            {
                DeliveryNotePos dnPos = SelectedFacilityReservation.InOrderPos.DeliveryNotePos_InOrderPos.FirstOrDefault();
                if (dnPos != null)
                {
                    string bsoName = SelectedACComp.ACUrlCommand("!GetBSONameForShowOrder", "BSOInDeliveryNote(Dialog)") as string;
                    if (String.IsNullOrEmpty(bsoName))
                    {
                        bsoName = ACUrlCommand("!GetBSONameForShowOrder", "BSOInDeliveryNote(Dialog)") as string;
                        if (String.IsNullOrEmpty(bsoName))
                            bsoName = "BSOInDeliveryNote(Dialog)";
                    }
                    ACComponent childBSO = this.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = this.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogNote", dnPos.DeliveryNote.DeliveryNoteNo, dnPos.DeliveryNotePosID);
                    _= childBSO.Stop();
                    return;
                }
            }
            else if (SelectedFacilityReservation.ProdOrderPartslistPos != null)
            {
                string bsoName = SelectedACComp.ACUrlCommand("!GetBSONameForShowOrder", "BSOProdOrder(Dialog)") as string;
                if (String.IsNullOrEmpty(bsoName))
                {
                    bsoName = ACUrlCommand("!GetBSONameForShowOrder", "BSOProdOrder(Dialog)") as string;
                    if (String.IsNullOrEmpty(bsoName))
                        bsoName = "BSOProdOrder(Dialog)";
                }
                ACComponent childBSO = this.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                if (childBSO == null)
                    childBSO = this.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                if (childBSO == null)
                    return;
                childBSO.ACUrlCommand("!ShowDialogOrder", SelectedFacilityReservation.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                    SelectedFacilityReservation.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrderPartslistID, 
                    SelectedFacilityReservation.ProdOrderPartslistPos.ProdOrderPartslistPosID);
                _= childBSO.Stop();
                return;
            }
            else if (SelectedFacilityReservation.ProdOrderBatchPlan != null && SelectedFacilityReservation.ProdOrderBatchPlan.ProdOrderPartslistPos != null)
            {
                string bsoName = SelectedACComp.ACUrlCommand("!GetBSONameForShowOrder", "BSOProdOrder(Dialog)") as string;
                if (String.IsNullOrEmpty(bsoName))
                {
                    bsoName = ACUrlCommand("!GetBSONameForShowOrder", "BSOProdOrder(Dialog)") as string;
                    if (String.IsNullOrEmpty(bsoName))
                        bsoName = "BSOProdOrder(Dialog)";
                }
                ACComponent childBSO = this.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                if (childBSO == null)
                    childBSO = this.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                if (childBSO == null)
                    return;
                childBSO.ACUrlCommand("!ShowDialogOrder", SelectedFacilityReservation.ProdOrderBatchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                    SelectedFacilityReservation.ProdOrderBatchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrderPartslistID,
                    SelectedFacilityReservation.ProdOrderBatchPlan.ProdOrderPartslistPos.ProdOrderPartslistPosID);
                _= childBSO.Stop();
                return;
            }

        }

        public bool IsEnabledShowOrder()
        {
            if (SelectedFacilityReservation == null || SelectedACComp == null)
                return false;
            return true;
        }
        #endregion

        #region Dialog
        public VBDialogResult DialogResult
        {
            get;
            set;
        }

        [ACMethodInfo("Dialog", "en{'Dialog Production order'}de{'Dialog Produktionsauftrag'}", (short)MISort.QueryPrintDlg, Description = @"Opens this BSO as a dialog for the given component: stores the component in SelectedACComp, loads its open reservations into FacilityReservationList and shows the DisplayDialog design. The BSO stops itself after the dialog is closed.")]
        public async Task ShowReservationDialog(IACComponent component)
        {
            _SelectedACComp = component;
            OnPropertyChanged("FacilityReservationList");
            await ShowDialogAsync(this, "DisplayDialog");
            await this.ParentACComponent.StopComponent(this);
        }

        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay, Description = @"Closes the reservation dialog with result OK (DialogResult.SelectedCommand = OK).")]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel, Description = @"Closes the reservation dialog with result Cancel (DialogResult.SelectedCommand = Cancel).")]
        public void DialogCancel()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            CloseTopDialog();
        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(GetBSONameForShowOrder):
                    result = GetBSONameForShowOrder((String)acParameter[0]);
                    return true;
                case nameof(ShowOrder):
                    ShowOrder();
                    return true;
                case nameof(IsEnabledShowOrder):
                    result = IsEnabledShowOrder();
                    return true;
                case nameof(ShowReservationDialog):
                    result = ShowReservationDialog((IACComponent)acParameter[0]);
                    return true;
                case nameof(DialogOK):
                    DialogOK();
                    return true;
                case nameof(DialogCancel):
                    DialogCancel();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(ShowOrder):
                case nameof(IsEnabledShowOrder):
                    return new string[] { nameof(SelectedFacilityReservation), nameof(SelectedACComp) };
                case nameof(GetBSONameForShowOrder):
                    return new string[] { nameof(SelectedFacilityReservation), nameof(SelectedACComp) };
                case nameof(ShowReservationDialog):
                    return new string[] { nameof(SelectedACComp) };
                case nameof(DialogOK):
                case nameof(DialogCancel):
                    return new string[] { nameof(InitState) };
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}
