using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'FacilityReservationView'}de{'FacilityReservationView'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class POPartslistPosReservation : ACObjectItemWCheckBox
    {
        public POPartslistPosReservation(gip.core.datamodel.ACClass module, ProdOrderPartslistPos parentPos, FacilityReservation parentReservation, FacilityReservation selectedReservation, Facility unselFacility, gip.core.datamodel.ACClassWF destWorkflowNode = null, gip.core.datamodel.ACClassWF planningWorkflowNode = null)
            : base(parentReservation, module.ACCaption)
        {
            _Module = module;
            _ParentPos = parentPos;
            _ParentReservation = parentReservation;
            _UnselFacility = unselFacility;
            _WorkflowNode = destWorkflowNode;
            _PlanningNode = planningWorkflowNode;
            SelectedReservation = selectedReservation;
        }

        public POPartslistPosReservation(gip.core.datamodel.ACClass module, ProdOrderBatchPlan parentBatchPlan, FacilityReservation parentReservation, FacilityReservation selectedReservation, Facility unselFacility, gip.core.datamodel.ACClassWF destWorkflowNode = null, gip.core.datamodel.ACClassWF planningWorkflowNode = null)
            : base(parentReservation, module.ACCaption)
        {
            _Module = module;
            _ParentBatchPlan = parentBatchPlan;
            _ParentReservation = parentReservation;
            _UnselFacility = unselFacility;
            _WorkflowNode = destWorkflowNode;
            _PlanningNode = planningWorkflowNode;
            SelectedReservation = selectedReservation;
        }

        public POPartslistPosReservation(gip.core.datamodel.ACClass module, DeliveryNotePos parentPos, FacilityReservation parentReservation, FacilityReservation selectedReservation, Facility unselFacility, gip.core.datamodel.ACClassWF destWorkflowNode = null, gip.core.datamodel.ACClassWF planningWorkflowNode = null)
            : base(parentReservation, module.ACCaption)
        {
            _Module = module;
            _ParentDeliveryNotePos = parentPos;
            _ParentReservation = parentReservation;
            _UnselFacility = unselFacility;
            _WorkflowNode = destWorkflowNode;
            _PlanningNode = planningWorkflowNode;
            SelectedReservation = selectedReservation;
        }

        public POPartslistPosReservation(gip.core.datamodel.ACClass module, PickingPos parentPos, FacilityReservation parentReservation, FacilityReservation selectedReservation, Facility unselFacility, gip.core.datamodel.ACClassWF destWorkflowNode = null, gip.core.datamodel.ACClassWF planningWorkflowNode = null)
            : base(parentReservation, module.ACCaption)
        {
            _Module = module;
            _ParentPickingPos = parentPos;
            _ParentReservation = parentReservation;
            _UnselFacility = unselFacility;
            _WorkflowNode = destWorkflowNode;
            _PlanningNode = planningWorkflowNode;
            SelectedReservation = selectedReservation;
        }

        gip.core.datamodel.ACClass _Module;
        [ACPropertyInfo(9999, "", "en{'Module'}de{'Module'}")]
        public gip.core.datamodel.ACClass Module
        {
            get
            {
                return _Module;
            }
        }

        gip.core.datamodel.ACClassWF _WorkflowNode;
        [ACPropertyInfo(9999, "", "en{'Workflownode'}de{'Workflowknoten'}")]
        public gip.core.datamodel.ACClassWF WorkflowNode
        {
            get
            {
                return _WorkflowNode;
            }
        }

        gip.core.datamodel.ACClassWF _PlanningNode;
        [ACPropertyInfo(9999, "", "en{'Workflownode'}de{'Workflowknoten'}")]
        public gip.core.datamodel.ACClassWF PlanningNode
        {
            get
            {
                return _PlanningNode;
            }
        }

        Facility _UnselFacility = null;
        [ACPropertyInfo(9999, "", "en{'Facility of module'}de{'Module Lagerplatz'}")]
        public Facility FacilityOfModule
        {
            get
            {
                if (_UnselFacility != null)
                    return _UnselFacility;
                else if (SelectedReservation != null)
                    return SelectedReservation.Facility;
                return null;
            }
        }


        ProdOrderBatchPlan _ParentBatchPlan;
        [ACPropertyInfo(9999)]
        public ProdOrderBatchPlan ParentBatchPlan
        {
            get
            {
                return _ParentBatchPlan;
            }
        }

        ProdOrderPartslistPos _ParentPos;
        [ACPropertyInfo(9999, "", "en{'ParentPos'}de{'Herkunftposition'}")]
        public ProdOrderPartslistPos ParentPos
        {
            get
            {
                return _ParentPos;
            }
        }

        DeliveryNotePos _ParentDeliveryNotePos;
        [ACPropertyInfo(9999)]
        public DeliveryNotePos ParentDeliveryNotePos
        {
            get
            {
                return _ParentDeliveryNotePos;
            }
        }

        PickingPos _ParentPickingPos;
        [ACPropertyInfo(9999)]
        public PickingPos ParentPickingPos
        {
            get
            {
                return _ParentPickingPos;
            }
        }

        FacilityReservation _ParentReservation;
        [ACPropertyInfo(9999)]
        FacilityReservation ParentReservation
        {
            get
            {
                return _ParentReservation;
            }
        }

        FacilityReservation _SelectedReservation;
        [ACPropertyInfo(9999)]
        public FacilityReservation SelectedReservation
        {
            get
            {
                return _SelectedReservation;
            }
            set
            {
                _SelectedReservation = value;
                OnPropertyChanged(nameof(SelectedReservation));
                OnPropertyChanged("IsChecked");

            }
        }

        [ACPropertyInfo(9999, "", "en{'Sequence'}de{'Reihenfolge'}")]
        public Nullable<int> Sequence
        {
            get
            {
                if (_SelectedReservation == null)
                    return null;
                return _SelectedReservation.Sequence;
            }
            set
            {
                if (_SelectedReservation != null)
                {
                    if (value.HasValue)
                        _SelectedReservation.Sequence = value.Value;
                    else
                        _SelectedReservation.Sequence = 0;
                    OnPropertyChanged("Sequence");
                }
            }
        }

        //private Route _CurrentRoute;
        [ACPropertyInfo(999, "", "en{'Route'}de{'Route'}")]
        public Route CurrentRoute
        {
            get
            {
                if (SelectedReservation == null)
                    return null;
                return SelectedReservation.PredefinedRoute;
            }
            set
            {
                if (SelectedReservation == null)
                    return;
                SelectedReservation.PredefinedRoute = value;
                OnPropertyChanged();
            }
        }

        public override bool IsChecked
        {
            get
            {
                return SelectedReservation != null;
            }
            set
            {
                if (ParentReservation != null)
                {
                    if (value && SelectedReservation == null)
                    {
                        FacilityReservation tempReservation = ParentReservation.FacilityReservation_ParentFacilityReservation.ToArray().Where(c => c.VBiACClassID == Module.ACClassID).FirstOrDefault();
                        if (tempReservation == null)
                        {
                            DatabaseApp dbApp = ParentReservation.GetObjectContext() as DatabaseApp;
                            string secondaryKey = ACRoot.SRoot.NoManager.GetNewNo(dbApp, typeof(FacilityReservation), FacilityReservation.NoColumnName, FacilityReservation.FormatNewNo, null);
                            tempReservation = FacilityReservation.NewACObject(dbApp, ParentReservation, secondaryKey);
                            tempReservation.FacilityACClass = Module;
                            tempReservation.Facility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == Module.ACClassID).FirstOrDefault();
                            if (ParentPos != null)
                                tempReservation.ProdOrderPartslistPos = ParentPos;
                            else if (ParentBatchPlan != null)
                                tempReservation.ProdOrderBatchPlan = ParentBatchPlan;
                            else if (ParentDeliveryNotePos != null)
                            {
                                if (ParentDeliveryNotePos.InOrderPos != null)
                                    tempReservation.InOrderPos = ParentDeliveryNotePos.InOrderPos;
                                else if (ParentDeliveryNotePos.OutOrderPos != null)
                                    tempReservation.OutOrderPos = ParentDeliveryNotePos.OutOrderPos;
                            }
                            else if (ParentPickingPos != null)
                                tempReservation.PickingPos = ParentPickingPos;
                            ParentReservation.FacilityReservation_ParentFacilityReservation.Add(tempReservation);
                        }
                        _SelectedReservation = tempReservation;
                        OnPropertyChanged(nameof(SelectedReservation));
                    }
                    else if (!value && SelectedReservation != null)
                    {
                        FacilityReservation tempReservation = ParentReservation.FacilityReservation_ParentFacilityReservation.ToArray().Where(c => c == SelectedReservation).FirstOrDefault();
                        if (tempReservation != null)
                        {
                            ParentReservation.FacilityReservation_ParentFacilityReservation.Remove(tempReservation);
                            tempReservation.DeleteACObject(ParentReservation.GetObjectContext(), true);
                        }
                        _SelectedReservation = null;
                        OnPropertyChanged(nameof(SelectedReservation));
                    }
                }
                else if (ParentPos != null)
                {
                    if (value && SelectedReservation == null)
                    {
                        FacilityReservation tempReservation = ParentPos.FacilityReservation_ProdOrderPartslistPos.ToArray().Where(c => c.VBiACClassID == Module.ACClassID).FirstOrDefault();
                        if (tempReservation == null)
                        {
                            DatabaseApp dbApp = ParentPos.GetObjectContext() as DatabaseApp;
                            string secondaryKey = ACRoot.SRoot.NoManager.GetNewNo(dbApp, typeof(FacilityReservation), FacilityReservation.NoColumnName, FacilityReservation.FormatNewNo, null);
                            tempReservation = FacilityReservation.NewACObject(dbApp, ParentPos, secondaryKey);
                            tempReservation.FacilityACClass = Module;
                            tempReservation.Facility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == Module.ACClassID).FirstOrDefault();
                            tempReservation.ProdOrderPartslistPos = ParentPos;
                            ParentPos.FacilityReservation_ProdOrderPartslistPos.Add(tempReservation);
                        }
                        _SelectedReservation = tempReservation;
                        OnPropertyChanged(nameof(SelectedReservation));
                    }
                    else if (!value && SelectedReservation != null)
                    {
                        FacilityReservation tempReservation = ParentPos.FacilityReservation_ProdOrderPartslistPos.ToArray().Where(c => c == SelectedReservation).FirstOrDefault();
                        if (tempReservation != null)
                        {
                            foreach (FacilityReservation childReservation in tempReservation.FacilityReservation_ParentFacilityReservation.ToArray())
                            {
                                tempReservation.FacilityReservation_ParentFacilityReservation.Remove(childReservation);
                                childReservation.DeleteACObject(ParentPos.GetObjectContext(), true);
                            }

                            ParentPos.FacilityReservation_ProdOrderPartslistPos.Remove(tempReservation);
                            tempReservation.DeleteACObject(ParentPos.GetObjectContext(), true);
                        }
                        _SelectedReservation = null;
                        OnPropertyChanged(nameof(SelectedReservation));
                    }
                }
                else if (ParentBatchPlan != null)
                {
                    if (value && SelectedReservation == null)
                    {
                        FacilityReservation tempReservation = ParentBatchPlan.FacilityReservation_ProdOrderBatchPlan.ToArray().Where(c => c.VBiACClassID == Module.ACClassID).FirstOrDefault();
                        if (tempReservation == null)
                        {
                            DatabaseApp dbApp = ParentBatchPlan.GetObjectContext() as DatabaseApp;
                            string secondaryKey = ACRoot.SRoot.NoManager.GetNewNo(dbApp, typeof(FacilityReservation), FacilityReservation.NoColumnName, FacilityReservation.FormatNewNo, null);
                            tempReservation = FacilityReservation.NewACObject(dbApp, ParentBatchPlan, secondaryKey);
                            tempReservation.FacilityACClass = Module;
                            tempReservation.Facility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == Module.ACClassID).FirstOrDefault();
                            tempReservation.ProdOrderBatchPlan = ParentBatchPlan;
                            ParentBatchPlan.FacilityReservation_ProdOrderBatchPlan.Add(tempReservation);
                        }
                        _SelectedReservation = tempReservation;
                        OnPropertyChanged(nameof(SelectedReservation));
                    }
                    else if (!value && SelectedReservation != null)
                    {
                        FacilityReservation tempReservation = ParentBatchPlan.FacilityReservation_ProdOrderBatchPlan.ToArray().Where(c => c == SelectedReservation).FirstOrDefault();
                        if (tempReservation != null)
                        {
                            foreach (FacilityReservation childReservation in tempReservation.FacilityReservation_ParentFacilityReservation.ToArray())
                            {
                                tempReservation.FacilityReservation_ParentFacilityReservation.Remove(childReservation);
                                childReservation.DeleteACObject(ParentBatchPlan.GetObjectContext(), true);
                            }

                            ParentBatchPlan.FacilityReservation_ProdOrderBatchPlan.Remove(tempReservation);
                            tempReservation.DeleteACObject(ParentBatchPlan.GetObjectContext(), true);
                        }
                        _SelectedReservation = null;
                        OnPropertyChanged(nameof(SelectedReservation));
                    }
                }
                else if (ParentDeliveryNotePos != null)
                {
                    if (ParentDeliveryNotePos.InOrderPos != null)
                    {
                        if (value && SelectedReservation == null)
                        {
                            FacilityReservation tempReservation = ParentDeliveryNotePos.InOrderPos.FacilityReservation_InOrderPos.ToArray().Where(c => c.VBiACClassID == Module.ACClassID).FirstOrDefault();
                            if (tempReservation == null)
                            {
                                DatabaseApp dbApp = ParentDeliveryNotePos.GetObjectContext() as DatabaseApp;
                                string secondaryKey = ACRoot.SRoot.NoManager.GetNewNo(dbApp, typeof(FacilityReservation), FacilityReservation.NoColumnName, FacilityReservation.FormatNewNo, null);
                                tempReservation = FacilityReservation.NewACObject(dbApp, ParentDeliveryNotePos.InOrderPos, secondaryKey);
                                tempReservation.FacilityACClass = Module;
                                tempReservation.Facility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == Module.ACClassID).FirstOrDefault();
                                tempReservation.InOrderPos = ParentDeliveryNotePos.InOrderPos;
                                ParentDeliveryNotePos.InOrderPos.FacilityReservation_InOrderPos.Add(tempReservation);
                            }
                            _SelectedReservation = tempReservation;
                            OnPropertyChanged(nameof(SelectedReservation));
                        }
                        else if (!value && SelectedReservation != null)
                        {
                            FacilityReservation tempReservation = ParentDeliveryNotePos.InOrderPos.FacilityReservation_InOrderPos.ToArray().Where(c => c == SelectedReservation).FirstOrDefault();
                            if (tempReservation != null)
                            {
                                foreach (FacilityReservation childReservation in tempReservation.FacilityReservation_ParentFacilityReservation.ToArray())
                                {
                                    tempReservation.FacilityReservation_ParentFacilityReservation.Remove(childReservation);
                                    childReservation.DeleteACObject(ParentDeliveryNotePos.GetObjectContext(), true);
                                }

                                ParentDeliveryNotePos.InOrderPos.FacilityReservation_InOrderPos.Remove(tempReservation);
                                tempReservation.DeleteACObject(ParentDeliveryNotePos.GetObjectContext(), true);
                            }
                            _SelectedReservation = null;
                            OnPropertyChanged(nameof(SelectedReservation));
                        }
                    }
                    else if (ParentDeliveryNotePos.OutOrderPos != null)
                    {
                        if (value && SelectedReservation == null)
                        {
                            FacilityReservation tempReservation = ParentDeliveryNotePos.OutOrderPos.FacilityReservation_OutOrderPos.ToArray().Where(c => c.VBiACClassID == Module.ACClassID).FirstOrDefault();
                            if (tempReservation == null)
                            {
                                DatabaseApp dbApp = ParentDeliveryNotePos.GetObjectContext() as DatabaseApp;
                                string secondaryKey = ACRoot.SRoot.NoManager.GetNewNo(dbApp, typeof(FacilityReservation), FacilityReservation.NoColumnName, FacilityReservation.FormatNewNo, null);
                                tempReservation = FacilityReservation.NewACObject(dbApp, ParentDeliveryNotePos.OutOrderPos, secondaryKey);
                                tempReservation.FacilityACClass = Module;
                                tempReservation.Facility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == Module.ACClassID).FirstOrDefault();
                                tempReservation.OutOrderPos = ParentDeliveryNotePos.OutOrderPos;
                                ParentDeliveryNotePos.OutOrderPos.FacilityReservation_OutOrderPos.Add(tempReservation);
                            }
                            _SelectedReservation = tempReservation;
                            OnPropertyChanged(nameof(SelectedReservation));
                        }
                        else if (!value && SelectedReservation != null)
                        {
                            FacilityReservation tempReservation = ParentDeliveryNotePos.OutOrderPos.FacilityReservation_OutOrderPos.ToArray().Where(c => c == SelectedReservation).FirstOrDefault();
                            if (tempReservation != null)
                            {
                                foreach (FacilityReservation childReservation in tempReservation.FacilityReservation_ParentFacilityReservation.ToArray())
                                {
                                    tempReservation.FacilityReservation_ParentFacilityReservation.Remove(childReservation);
                                    childReservation.DeleteACObject(ParentDeliveryNotePos.GetObjectContext(), true);
                                }

                                ParentDeliveryNotePos.OutOrderPos.FacilityReservation_OutOrderPos.Remove(tempReservation);
                                tempReservation.DeleteACObject(ParentDeliveryNotePos.GetObjectContext(), true);
                            }
                            _SelectedReservation = null;
                            OnPropertyChanged(nameof(SelectedReservation));
                        }
                    }
                }
                else if (ParentPickingPos != null)
                {
                    List<FacilityReservation> reservationList = null;
                    if (value && SelectedReservation == null)
                    {
                        reservationList = ParentPickingPos.FacilityReservation_PickingPos.Where(c => c.VBiACClassID.HasValue).ToArray().OrderBy(c => c.Sequence).ToList();
                        FacilityReservation tempReservation = reservationList.Where(c => c.VBiACClassID == Module.ACClassID).FirstOrDefault();
                        if (tempReservation == null)
                        {
                            DatabaseApp dbApp = ParentPickingPos.GetObjectContext() as DatabaseApp;
                            string secondaryKey = ACRoot.SRoot.NoManager.GetNewNo(dbApp, typeof(FacilityReservation), FacilityReservation.NoColumnName, FacilityReservation.FormatNewNo, null);
                            tempReservation = FacilityReservation.NewACObject(dbApp, ParentPickingPos, secondaryKey);
                            tempReservation.FacilityACClass = Module;
                            tempReservation.Facility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == Module.ACClassID).FirstOrDefault();
                            tempReservation.PickingPos = ParentPickingPos;
                            ParentPickingPos.FacilityReservation_PickingPos.Add(tempReservation);
                            ParentPickingPos.Picking.VBiACClassWFID = PlanningNode?.ACClassWFID;
                            reservationList.Add(tempReservation);
                        }
                        _SelectedReservation = tempReservation;
                        OnPropertyChanged(nameof(SelectedReservation));
                    }
                    else if (!value && SelectedReservation != null)
                    {
                        reservationList = ParentPickingPos.FacilityReservation_PickingPos.Where(c => c.VBiACClassID.HasValue).ToArray().OrderBy(c => c.Sequence).ToList();
                        FacilityReservation tempReservation = reservationList.Where(c => c == SelectedReservation).FirstOrDefault();
                        if (tempReservation != null)
                        {
                            foreach (FacilityReservation childReservation in tempReservation.FacilityReservation_ParentFacilityReservation.ToArray())
                            {
                                tempReservation.FacilityReservation_ParentFacilityReservation.Remove(childReservation);
                                childReservation.DeleteACObject(ParentPickingPos.GetObjectContext(), true);
                            }

                            ParentPickingPos.FacilityReservation_PickingPos.Remove(tempReservation);
                            tempReservation.DeleteACObject(ParentPickingPos.GetObjectContext(), true);
                            reservationList.Remove(tempReservation);
                        }
                        _SelectedReservation = null;
                        OnPropertyChanged(nameof(SelectedReservation));
                    }
                    if (reservationList != null)
                    {
                        bool changeDestination = true;
                        var toFacility = ParentPickingPos.ToFacility;
                        if (toFacility != null)
                            changeDestination = !reservationList.Where(c => c.Facility == toFacility).Any();
                        if (changeDestination)
                            ParentPickingPos.ToFacility = reservationList.FirstOrDefault()?.Facility;
                    }
                    else
                    {
                        ParentPickingPos.ToFacility = null;
                    }
                }
                OnPropertyChanged("IsChecked");
                OnPropertyChanged("CurrentRoute");
            }
        }
    }
}
