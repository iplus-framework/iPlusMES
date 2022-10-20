using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.autocomponent;
using System.Data.Objects;
using gip.core.autocomponent;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Module reservations'}de{'Modul-Reservierung'}", Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, true, false)]
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._SelectedACComp = null;
            this._SelectedFacilityReservation = null;
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Queries
        public static readonly Func<DatabaseApp, Guid, IQueryable<FacilityReservation>> s_cQry_OpenReservations =
            CompiledQuery.Compile<DatabaseApp, Guid, IQueryable<FacilityReservation>>(
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
        [ACPropertySelected(500, FacilityReservation.ClassName)]
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

        [ACPropertyList(501, FacilityReservation.ClassName)]
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
        [ACMethodInfo("", "", 590)]
        public virtual string GetBSONameForShowOrder(string defaultBSOName)
        {
            return defaultBSOName;
        }


        [ACMethodInteraction(FacilityReservation.ClassName, "en{'View order'}de{'Auftrag anzeigen'}", (short)MISort.BringToFront, true, "SelectedFacilityReservation", Global.ACKinds.MSMethodPrePost, false, Global.ContextMenuCategory.ProdPlanLog)]
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
                    childBSO.Stop();
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
                childBSO.Stop();
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
                childBSO.Stop();
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

        [ACMethodInfo("Dialog", "en{'Dialog Production order'}de{'Dialog Produktionsauftrag'}", (short)MISort.QueryPrintDlg)]
        public void ShowReservationDialog(IACComponent component)
        {
            _SelectedACComp = component;
            OnPropertyChanged("FacilityReservationList");
            ShowDialog(this, "DisplayDialog");
            this.ParentACComponent.StopComponent(this);
        }

        [ACMethodCommand("Dialog", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
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
                case"GetBSONameForShowOrder":
                    result = GetBSONameForShowOrder((String)acParameter[0]);
                    return true;
                case"ShowOrder":
                    ShowOrder();
                    return true;
                case"IsEnabledShowOrder":
                    result = IsEnabledShowOrder();
                    return true;
                case"ShowReservationDialog":
                    ShowReservationDialog((IACComponent)acParameter[0]);
                    return true;
                case"DialogOK":
                    DialogOK();
                    return true;
                case"DialogCancel":
                    DialogCancel();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
