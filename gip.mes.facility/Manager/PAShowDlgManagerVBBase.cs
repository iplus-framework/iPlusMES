// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.manager;
using System;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PAShowDlgManagerVB'}de{'PAShowDlgManagerVB'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class PAShowDlgManagerVBBase : PAShowDlgManagerBase
    {
        #region c´tors
        public PAShowDlgManagerVBBase(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _C_BSONameForShowReservation = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowReservation), "");
            _C_BSONameForShowProgramLog = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowProgamLog), "");
            _C_BSONameForShowPicking = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowPicking), "");
            _C_BSONameForShowInDeliveryNote = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowInDeliveryNote), "");
            _C_BSONameForShowOutDeliveryNote = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowOutDeliveryNote), "");
            _C_BSONameForShowLabOrder = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowLabOrder), "");
            _C_BSONameForShowFacilityBookCell = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowFacilityBookCell), "");
            _C_BSONameForShowFacilityBookCharge = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowFacilityBookCharge), "");
            _C_BSONameForShowFacilityOverview = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowFacilityOverview), "");
            _C_BSONameForShowOrder = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowOrder), "");
            _C_BSONameForShowComponent = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowComponent), "");
            _C_BSONameForShowVisitorVoucher = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowVisitorVoucher), "");
            _C_BSONameForFacilityLotOverview = new ACPropertyConfigValue<string>(this, nameof(BSONameForFacilityLotOverview), "");
            _C_BSONameForFacilityLot = new ACPropertyConfigValue<string>(this, nameof(BSONameForFacilityLot), "");
            _C_BSONameForFacilityChargeOverview = new ACPropertyConfigValue<string>(this, nameof(BSONameForFacilityChargeOverview), "");
            _C_BSONameForShowMaterial = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowMaterial), "");
            _C_BSONameForMaterialOverview = new ACPropertyConfigValue<string>(this, nameof(BSONameForMaterialOverview), "");
            _C_BSONameForShowInOrder = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowInOrder), "");
            _C_BSONameForShowOutOrder = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowOutOrder), "");
            _C_BSONameForShowInvoice = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowInvoice), "");
            _C_BSONameForShowPartslist = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowPartslist), "");
        }

        public const string ClassNameVBBase = "PAShowDlgManagerVBBase";

        #endregion

        #region Configuration

        private ACPropertyConfigValue<string> _C_BSONameForShowOrder;
        [ACPropertyConfig("en{'Classname and ACIdentifier for Prod.-Order'}de{'Klassenname und ACIdentifier für Produktionsauftrag'}")]
        public string BSONameForShowOrder
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowOrder.ValueT))
                {
                    _C_BSONameForShowOrder.ValueT = GetBSOName("BSOProdOrder", "Dialog");
                }

                return _C_BSONameForShowOrder.ValueT;
            }
            set
            {
                _C_BSONameForShowOrder.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowComponent;
        [ACPropertyConfig("en{'Classname and ACIdentifier for show componets'}de{'Klassenname und ACIdentifier für Komponenten anzeigen'}")]
        public string BSONameForShowComponent
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowComponent.ValueT))
                {
                    _C_BSONameForShowComponent.ValueT = GetBSOName("BSOProdOrderBatchComponents", "Dialog");
                }

                return _C_BSONameForShowComponent.ValueT;
            }
            set
            {
                _C_BSONameForShowComponent.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowReservation;
        [ACPropertyConfig("en{'Classname and ACIdentifier for ShowReservationDlg'}de{'Klassenname und ACIdentifier für ShowReservationDlg'}")]
        public string BSONameForShowReservation
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowReservation.ValueT))
                {
                    _C_BSONameForShowReservation.ValueT = GetBSOName("BSOComponentReservation", "Dialog");
                }

                return _C_BSONameForShowReservation.ValueT;
            }
            set
            {
                _C_BSONameForShowReservation.ValueT = value;
            }
        }

        public override string BSONameForShowProgamLog
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowProgramLog.ValueT))
                {
                    _C_BSONameForShowProgramLog.ValueT = GetBSOName(nameof(PresenterProgramLogVB), typeof(PresenterProgramLogVB).GetACType() as gip.core.datamodel.ACClass, null);
                }

                return _C_BSONameForShowProgramLog.ValueT;
            }
            set
            {
                _C_BSONameForShowProgramLog.ValueT = value;
            }
        }

        public override string BSONameForShowPropertyLog
        {
            get
            {
                if (!String.IsNullOrEmpty(_C_BSONameForShowPropertyLog.ValueT))
                    return _C_BSONameForShowPropertyLog.ValueT;
                _C_BSONameForShowPropertyLog.ValueT = "BSOPropertyLogPresenterVB";
                return _C_BSONameForShowPropertyLog.ValueT;
            }
            set
            {
                _C_BSONameForShowPropertyLog.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowPicking;
        [ACPropertyConfig("en{'Classname and ACIdentifier for Pickingorder'}de{'Klassenname und ACIdentifier für Kommissionierauftrag'}")]
        public string BSONameForShowPicking
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowPicking.ValueT))
                {
                    _C_BSONameForShowPicking.ValueT = GetBSOName("BSOPicking", "Dialog");
                }

                return _C_BSONameForShowPicking.ValueT;
            }
            set
            {
                _C_BSONameForShowPicking.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowInDeliveryNote;
        [ACPropertyConfig("en{'Classname and ACIdentifier for In-Deliverynote'}de{'Klassenname und ACIdentifier für Eingangs-Lieferschein'}")]
        public string BSONameForShowInDeliveryNote
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowInDeliveryNote.ValueT))
                {
                    _C_BSONameForShowInDeliveryNote.ValueT = GetBSOName("BSOInDeliveryNote", "Dialog");
                }

                return _C_BSONameForShowInDeliveryNote.ValueT;
            }
            set
            {
                _C_BSONameForShowInDeliveryNote.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowOutDeliveryNote;
        [ACPropertyConfig("en{'Classname and ACIdentifier for In-Deliverynote'}de{'Klassenname und ACIdentifier für Eingangs-Lieferschein'}")]
        public string BSONameForShowOutDeliveryNote
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowOutDeliveryNote.ValueT))
                {
                    _C_BSONameForShowOutDeliveryNote.ValueT = GetBSOName("BSOOutDeliveryNote", "Dialog");
                }

                return _C_BSONameForShowOutDeliveryNote.ValueT;
            }
            set
            {
                _C_BSONameForShowOutDeliveryNote.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowLabOrder;
        [ACPropertyConfig("en{'Classname and ACIdentifier for ShowLabOrderDlg'}de{'Klassenname und ACIdentifier für ShowLabOrderDlg'}")]
        public string BSONameForShowLabOrder
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowLabOrder.ValueT))
                {
                    _C_BSONameForShowLabOrder.ValueT = GetBSOName("BSOLabOrderMES", "Dialog");
                }

                return _C_BSONameForShowLabOrder.ValueT;
            }
            set
            {
                _C_BSONameForShowLabOrder.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowFacilityBookCell;
        [ACPropertyConfig("en{'Classname and ACIdentifier for Cellfacility'}de{'Klassenname und ACIdentifier für Silobestandsführung'}")]
        public string BSONameForShowFacilityBookCell
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowFacilityBookCell.ValueT))
                {
                    _C_BSONameForShowFacilityBookCell.ValueT = GetBSOName("BSOFacilityBookCell", "Dialog");
                }

                return _C_BSONameForShowFacilityBookCell.ValueT;
            }
            set
            {
                _C_BSONameForShowFacilityBookCell.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowFacilityBookCharge;
        [ACPropertyConfig("en{'Classname and ACIdentifier for Cellfacility'}de{'Klassenname und ACIdentifier für Silobestandsführung'}")]
        public string BSONameForShowFacilityBookCharge
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowFacilityBookCharge.ValueT))
                {
                    _C_BSONameForShowFacilityBookCharge.ValueT = GetBSOName("BSOFacilityBookCharge", "Dialog");
                }

                return _C_BSONameForShowFacilityBookCharge.ValueT;
            }
            set
            {
                _C_BSONameForShowFacilityBookCharge.ValueT = value;
            }
        }


        private ACPropertyConfigValue<string> _C_BSONameForFacilityLot;
        [ACPropertyConfig("en{'Classname and ACIdentifier for Lot Mangement'}de{'Klassenname und ACIdentifier für Losverwaltung'}")]
        public string BSONameForFacilityLot
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForFacilityLot.ValueT))
                {
                    _C_BSONameForFacilityLot.ValueT = GetBSOName("BSOFacilityLot", "Dialog");
                }

                return _C_BSONameForFacilityLot.ValueT;
            }
            set
            {
                _C_BSONameForFacilityLot.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowFacilityOverview;
        [ACPropertyConfig("en{'Classname and ACIdentifier for Facility-Overview'}de{'Klassenname und ACIdentifier für Lagerplatzübersicht'}")]
        public string BSONameForShowFacilityOverview
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowFacilityOverview.ValueT))
                {
                    _C_BSONameForShowFacilityOverview.ValueT = GetBSOName("BSOFacilityOverview", "Dialog");
                }

                return _C_BSONameForShowFacilityOverview.ValueT;
            }
            set
            {
                _C_BSONameForShowFacilityOverview.ValueT = value;
            }
        }



        private ACPropertyConfigValue<string> _C_BSONameForShowVisitorVoucher;
        [ACPropertyConfig("en{'Classname and ACIdentifier for Visitor voucher'}de{'Klassenname und ACIdentifier für Besucherbeleg'}")]
        public string BSONameForShowVisitorVoucher
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowVisitorVoucher.ValueT))
                {
                    _C_BSONameForShowVisitorVoucher.ValueT = GetBSOName("BSOVisitorVoucher", "Dialog");
                }

                return _C_BSONameForShowVisitorVoucher.ValueT;
            }
            set
            {
                _C_BSONameForShowVisitorVoucher.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForFacilityLotOverview;
        [ACPropertyConfig("en{'Classname and ACIdentifier for Lot Overview'}de{'Klassenname und ACIdentifier für Losübersicht'}")]
        public string BSONameForFacilityLotOverview
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForFacilityLotOverview.ValueT))
                {
                    _C_BSONameForFacilityLotOverview.ValueT = GetBSOName("BSOFacilityLotOverview", "Dialog");
                }

                return _C_BSONameForFacilityLotOverview.ValueT;
            }
            set
            {
                _C_BSONameForFacilityLotOverview.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForFacilityChargeOverview;
        [ACPropertyConfig("en{'Classname and ACIdentifier for Quant Overview'}de{'Klassenname Quantübersicht'}")]
        public string BSONameForFacilityChargeOverview
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForFacilityChargeOverview.ValueT))
                {
                    _C_BSONameForFacilityChargeOverview.ValueT = GetBSOName("BSOFacilityChargeOverview", "Dialog");
                }

                return _C_BSONameForFacilityChargeOverview.ValueT;
            }
            set
            {
                _C_BSONameForFacilityChargeOverview.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForMaterialOverview;
        [ACPropertyConfig("en{'Classname and ACIdentifier for Material Overview'}de{'Klassenname Materialübersicht'}")]
        public string BSONameForMaterialOverview
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForMaterialOverview.ValueT))
                {
                    _C_BSONameForMaterialOverview.ValueT = GetBSOName("BSOFacilityMaterialOverview", "Dialog");
                }

                return _C_BSONameForMaterialOverview.ValueT;
            }
            set
            {
                _C_BSONameForMaterialOverview.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowMaterial;
        [ACPropertyConfig("en{'Classname and ACIdentifier for Material management'}de{'Klassenname und ACIdentifier für Materialverwaltung'}")]
        public string BSONameForShowMaterial
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowMaterial.ValueT))
                {
                    _C_BSONameForShowMaterial.ValueT = GetBSOName("BSOMaterial", "Dialog");
                }

                return _C_BSONameForShowMaterial.ValueT;
            }
            set
            {
                _C_BSONameForShowMaterial.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowInOrder;
        [ACPropertyConfig("en{'Classname and ACIdentifier for purchase Order'}de{'Klassenname und ACIdentifier für Bestellung'}")]
        public string BSONameForShowInOrder
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowInOrder.ValueT))
                {
                    _C_BSONameForShowInOrder.ValueT = GetBSOName("BSOInOrder", "Dialog");
                }

                return _C_BSONameForShowInOrder.ValueT;
            }
            set
            {
                _C_BSONameForShowInOrder.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowOutOrder;
        [ACPropertyConfig("en{'Classname and ACIdentifier for sales Order'}de{'Klassenname und ACIdentifier für Verkaufsauftrag'}")]
        public string BSONameForShowOutOrder
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowOutOrder.ValueT))
                {
                    _C_BSONameForShowOutOrder.ValueT = GetBSOName("BSOOutOrder", "Dialog");
                }

                return _C_BSONameForShowOutOrder.ValueT;
            }
            set
            {
                _C_BSONameForShowOutOrder.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowInvoice;
        [ACPropertyConfig("en{'Classname and ACIdentifier for invoice'}de{'Klassenname und ACIdentifier für Rechnung'}")]
        public string BSONameForShowInvoice
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowInvoice.ValueT))
                {
                    _C_BSONameForShowInvoice.ValueT = GetBSOName("BSOInvoice", "Dialog");
                }

                return _C_BSONameForShowInvoice.ValueT;
            }
            set
            {
                _C_BSONameForShowInvoice.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowPartslist;
        [ACPropertyConfig("en{'Classname and ACIdentifier for bill of materials'}de{'Klassenname und ACIdentifier für Stückliste'}")]
        public string BSONameForShowPartslist
        {
            get
            {
                if (string.IsNullOrEmpty(_C_BSONameForShowPartslist.ValueT))
                {
                    _C_BSONameForShowPartslist.ValueT = GetBSOName("BSOPartslist", "Dialog");
                }

                return _C_BSONameForShowPartslist.ValueT;
            }
            set
            {
                _C_BSONameForShowPartslist.ValueT = value;
            }
        }
        #endregion

        #region Precompiled Query
        public static readonly Func<DatabaseApp, Guid, IEnumerable<ProdOrderBatch>> s_cQry_BatchInfo =
        EF.CompileQuery<DatabaseApp, Guid, IEnumerable<ProdOrderBatch>>(
            (ctx, id) => ctx.ProdOrderBatch.Include("ProdOrderPartslist")
                                 .Include("ProdOrderPartslist.ProdOrder")
                                 .Include("ProdOrderPartslist.Partslist")
                                 .Include("ProdOrderPartslist.Partslist.Material")
                                 .Include("ProdOrderBatchPlan")
                                 .Include("ProdOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan")
                                 .Include("ProdOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan.Facility")
                                 .Where(c => c.ProdOrderBatchID == id)
        );

        public static readonly Func<DatabaseApp, Guid, IEnumerable<ProdOrderPartslistPos>> s_cQry_POPosInfo =
        EF.CompileQuery<DatabaseApp, Guid, IEnumerable<ProdOrderPartslistPos>>(
            (ctx, id) => ctx.ProdOrderPartslistPos.Include("ProdOrderPartslist")
                                 .Include("ProdOrderPartslist.ProdOrder")
                                 .Include("ProdOrderPartslist.Partslist")
                                 .Include("ProdOrderPartslist.Partslist.Material")
                                 .Include("ProdOrderBatch")
                                 .Include("ProdOrderBatch.ProdOrderBatchPlan")
                                 .Include("ProdOrderBatch.ProdOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan")
                                 .Include("ProdOrderBatch.ProdOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan.Facility")
                                 .Where(c => c.ProdOrderPartslistPosID == id)
        );

        public static readonly Func<DatabaseApp, Guid, IEnumerable<Picking>> s_cQry_PickingInfo =
        EF.CompileQuery<DatabaseApp, Guid, IEnumerable<Picking>>(
            (ctx, id) => ctx.Picking.Include("PickingPos_Picking")
                                 .Include("PickingPos_Picking.ToFacility")
                                 .Where(c => c.PickingID == id)
        );

        public static readonly Func<DatabaseApp, Guid, IEnumerable<DeliveryNotePos>> s_cQry_DeliveryNotePosInfo =
        EF.CompileQuery<DatabaseApp, Guid, IEnumerable<DeliveryNotePos>>(
            (ctx, id) => ctx.DeliveryNotePos.Include("DeliveryNote")
                                    .Include("InOrderPos")
                                    .Include("InOrderPos.Material")
                                    .Include("OutOrderPos")
                                    .Include("OutOrderPos.Material")
                                 .Where(c => c.DeliveryNotePosID == id)
        );

        public static readonly Func<DatabaseApp, Guid, IEnumerable<FacilityBooking>> s_cQry_FacilityBookingInfo =
        EF.CompileQuery<DatabaseApp, Guid, IEnumerable<FacilityBooking>>(
            (ctx, id) => ctx.FacilityBooking.Include("OutwardMaterial")
                                            .Include("InwardMaterial")
                                    .Include("OutwardFacility")
                                    .Include("InwardFacility")
                                 .Where(c => c.FacilityBookingID == id)
        );

        #endregion

        #region Public Methods

        #region Dialog methods
        public override void ShowDialogOrder(IACComponent caller, PAOrderInfo orderInfo = null)
        {
            if (caller == null)
                return;

            if (orderInfo == null)
                orderInfo = QueryOrderInfo(caller);
            if (orderInfo != null)
            {
                // Falls Produktionsauftrag
                if (orderInfo.Entities.Where(c => c.EntityName == ProdOrderBatch.ClassName
                                                || c.EntityName == ProdOrderPartslistPos.ClassName
                                                || c.EntityName == ProdOrderPartslistPosRelation.ClassName
                                                || c.EntityName == ProdOrderPartslist.ClassName
                                                || c.EntityName == OrderLog.ClassName).Any())
                {
                    string bsoName = BSONameForShowOrder;
                    if (String.IsNullOrEmpty(bsoName))
                        bsoName = "BSOProdOrder(Dialog)";
                    ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                    childBSO.Stop();
                    return;
                }
                // Falls Kommissionierauftrag
                else if (orderInfo.Entities.Where(c => c.EntityName == Picking.ClassName
                                                    || c.EntityName == PickingPos.ClassName).Any())
                {
                    string bsoName = BSONameForShowPicking;
                    if (String.IsNullOrEmpty(bsoName))
                        bsoName = "BSOPicking(Dialog)";
                    ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                    childBSO.Stop();
                    return;
                }
                // Lieferschein WE/WA
                else if (orderInfo.Entities.Where(c => c.EntityName == DeliveryNote.ClassName
                                                    || c.EntityName == DeliveryNotePos.ClassName).Any())
                {
                    var notePosEntry = orderInfo.Entities.Where(c => c.EntityName == DeliveryNotePos.ClassName).FirstOrDefault();
                    var noteEntry = orderInfo.Entities.Where(c => c.EntityName == DeliveryNote.ClassName).FirstOrDefault();
                    using (DatabaseApp dbApp = new DatabaseApp())
                    {
                        DeliveryNotePos dnPos = null;
                        if (notePosEntry == null && noteEntry != null)
                            dnPos = dbApp.DeliveryNotePos.Where(c => c.DeliveryNoteID == noteEntry.EntityID).FirstOrDefault();
                        else
                            dnPos = dbApp.DeliveryNotePos.Where(c => c.DeliveryNotePosID == notePosEntry.EntityID).FirstOrDefault();
                        if (dnPos != null)
                        {
                            if (dnPos.InOrderPosID.HasValue)
                            {
                                string bsoName = BSONameForShowInDeliveryNote;
                                if (String.IsNullOrEmpty(bsoName))
                                    bsoName = "BSOInDeliveryNote(Dialog)";
                                ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                                if (childBSO == null)
                                    childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                                if (childBSO == null)
                                    return;
                                childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                                childBSO.Stop();
                                return;
                            }
                            else if (dnPos.OutOrderPosID.HasValue)
                            {
                                string bsoName = BSONameForShowOutDeliveryNote;
                                if (String.IsNullOrEmpty(bsoName))
                                    bsoName = "BSOOutDeliveryNote(Dialog)";
                                ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                                if (childBSO == null)
                                    childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                                if (childBSO == null)
                                    return;
                                childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                                childBSO.Stop();
                                return;
                            }
                        }
                        else
                        {
                            DeliveryNote dNote = dbApp.DeliveryNote.Where(c => c.DeliveryNoteID == noteEntry.EntityID).FirstOrDefault();
                            if (dNote != null)
                            {
                                string bsoName = BSONameForShowInDeliveryNote;
                                if (String.IsNullOrEmpty(bsoName))
                                    bsoName = "BSOInDeliveryNote(Dialog)";
                                ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                                if (childBSO == null)
                                    childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                                if (childBSO == null)
                                    return;
                                childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                                childBSO.Stop();
                                return;
                            }
                        }
                    }
                }
                // Falls Besucherbeleg
                else if (orderInfo.Entities.Where(c => c.EntityName == VisitorVoucher.ClassName).Any())
                {
                    string bsoName = BSONameForShowVisitorVoucher;
                    if (String.IsNullOrEmpty(bsoName))
                        bsoName = "BSOVisitorVoucher(Dialog)";
                    ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                    childBSO.Stop();
                    return;
                }
                else if (orderInfo.Entities.Where(c => c.EntityName == nameof(FacilityCharge)).Any())
                {
                    string bsoName = (new short[] { 0, 2 }).Contains(orderInfo.DialogSelectInfo) ? BSONameForShowFacilityBookCharge : BSONameForFacilityChargeOverview;
                    ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                    {
                        ACValueList acValueList = new ACValueList();
                        acValueList.Add(new ACValue(Const.SkipSearchOnStart, typeof(bool), true));
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { acValueList }) as ACComponent;
                    }
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                    childBSO.Stop();
                    return;
                }
                else if (orderInfo.Entities.Where(c => c.EntityName == nameof(FacilityLot)).Any())
                {
                    string bsoName = orderInfo.DialogSelectInfo == 0 ? BSONameForFacilityLot : BSONameForFacilityLotOverview;
                    ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                    childBSO.Stop();
                    return;
                }
                else if (orderInfo.Entities.Where(c => c.EntityName == nameof(Facility)).Any())
                {
                    string bsoName = orderInfo.DialogSelectInfo == 0 ? this.BSONameForShowFacilityBookCell : BSONameForShowFacilityOverview;
                    ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                    childBSO.Stop();
                    return;
                }
                else if (orderInfo.Entities.Where(c => c.EntityName == nameof(Material)).Any())
                {
                    string bsoName = orderInfo.DialogSelectInfo == 0 ? this.BSONameForShowMaterial : BSONameForMaterialOverview;
                    ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                    childBSO.Stop();
                    return;
                }
                else if (orderInfo.Entities.Where(c => c.EntityName == InOrder.ClassName
                                                    || c.EntityName == InOrderPos.ClassName).Any())
                {
                    string bsoName = this.BSONameForShowInOrder;
                    if (String.IsNullOrEmpty(bsoName))
                        bsoName = "BSOInOrder(Dialog)";
                    ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                    childBSO.Stop();
                    return;
                }
                else if (orderInfo.Entities.Where(c => c.EntityName == OutOrder.ClassName
                                                    || c.EntityName == OutOrderPos.ClassName).Any())
                {
                    string bsoName = this.BSONameForShowOutOrder;
                    if (String.IsNullOrEmpty(bsoName))
                        bsoName = "BSOOutOrder(Dialog)";
                    ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                    childBSO.Stop();
                    return;
                }
                else if (orderInfo.Entities.Where(c => c.EntityName == Invoice.ClassName).Any())
                {
                    string bsoName = this.BSONameForShowInvoice;
                    if (String.IsNullOrEmpty(bsoName))
                        bsoName = "BSOInvoice(Dialog)";
                    ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                    childBSO.Stop();
                    return;
                }
                else if (orderInfo.Entities.Where(c => c.EntityName == Partslist.ClassName).Any())
                {
                    string bsoName = this.BSONameForShowPartslist;
                    if (String.IsNullOrEmpty(bsoName))
                        bsoName = "BSOPartslist(Dialog)";
                    ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogOrderInfo", orderInfo);
                    childBSO.Stop();
                    return;
                }
            }
            // Wegen Kompatibilität zu Version 3, die noch keine PAOrderInfo-Struktur kannte
            else
            {
                ACComponent paModule = caller as ACComponent;
                if (paModule == null)
                    return;
                IACContainerTNet<ACRef<ProdOrderPartslistPos>> batchPos = paModule.GetProperty("CurrentBatchPos") as IACContainerTNet<ACRef<ProdOrderPartslistPos>>;
                if (batchPos != null && batchPos.ValueT != null && batchPos.ValueT.ValueT != null)
                {
                    string bsoName = BSONameForShowOrder;
                    if (String.IsNullOrEmpty(bsoName))
                        bsoName = "BSOProdOrder(Dialog)";
                    ACComponent childBSO = paModule.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogOrder",
                            batchPos.ValueT.ValueT.ProdOrderPartslist.ProdOrder.ProgramNo,
                            batchPos.ValueT.ValueT.ProdOrderPartslist.ProdOrderPartslistID,
                            Guid.Empty,
                            batchPos.ValueT.ValueT.ProdOrderPartslistPosID);
                    childBSO.Stop();
                    return;
                }

                IACContainerTNet<ACRef<DeliveryNotePos>> dnPos = paModule.GetProperty("CurrentDeliveryNotePos") as IACContainerTNet<ACRef<DeliveryNotePos>>;
                if (dnPos != null && dnPos.ValueT != null && dnPos.ValueT.ValueT != null)
                {
                    string bsoName = BSONameForShowInDeliveryNote;
                    if (String.IsNullOrEmpty(bsoName))
                        bsoName = "BSOInDeliveryNote(Dialog)";
                    ACComponent childBSO = paModule.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogNote", dnPos.ValueT.ValueT.DeliveryNote.DeliveryNoteNo, dnPos.ValueT.ValueT.DeliveryNotePosID);
                    childBSO.Stop();
                    return;
                }
            }
        }

        public override void ShowDialogComponents(IACComponent caller, PAOrderInfo orderInfo = null)
        {
            if (caller == null)
                return;

            if (orderInfo == null)
                orderInfo = QueryOrderInfo(caller);
            if (orderInfo != null)
            {
                // Falls Produktionsauftrag
                if (orderInfo.Entities.Where(c => c.EntityName == ProdOrderBatchPlan.ClassName).Any())
                {
                    string bsoName = BSONameForShowComponent;
                    ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
                    if (childBSO == null)
                        childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
                    if (childBSO == null)
                        return;
                    childBSO.ACUrlCommand("!ShowDialogComponent", orderInfo);
                    childBSO.Stop();
                    return;
                }
            }
        }

        public override bool IsEnabledShowDialogOrder(IACComponent caller)
        {
            IACContainerTNet<String> orderInfoProp = null;
            if (HasOrderInfo(caller, out orderInfoProp))
                return true;

            IACContainerTNet<ACRef<ProdOrderPartslistPos>> batchPos = caller.GetProperty("CurrentBatchPos") as IACContainerTNet<ACRef<ProdOrderPartslistPos>>;
            if (batchPos != null && batchPos.ValueT != null && batchPos.ValueT.ValueT != null)
                return true;

            IACContainerTNet<ACRef<DeliveryNotePos>> dnPos = caller.GetProperty("CurrentDeliveryNotePos") as IACContainerTNet<ACRef<DeliveryNotePos>>;
            if (dnPos != null && dnPos.ValueT != null && dnPos.ValueT.ValueT != null)
                return true;

            return false;
        }

        public virtual void ShowReservationDialog(IACComponent caller)
        {
            if (caller == null)
                return;
            ACComponent paModule = caller as ACComponent;
            if (!IsEnabledShowReservationDialog(caller))
                return;

            string bsoName = BSONameForShowReservation;
            if (String.IsNullOrEmpty(bsoName))
                bsoName = "BSOComponentReservation(Dialog)";

            ACComponent childBSO = paModule.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
            if (childBSO == null)
                childBSO = paModule.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            childBSO.ACUrlCommand("!ShowReservationDialog", paModule);
            childBSO.Stop();
        }

        public virtual bool IsEnabledShowReservationDialog(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            string reservation = _this.ACUrlCommand("ReservationInfo") as string;
            return !String.IsNullOrEmpty(reservation);
        }

        public virtual void ShowLabOrder(IACComponent caller, LabOrder labOrder = null)
        {
            if (caller == null)
                return;

            if (!IsEnabledShowLabOrder(caller))
                return;

            PAOrderInfo orderInfo = null;
            if (labOrder == null)
            {
                orderInfo = QueryOrderInfo(caller);
                if (orderInfo == null)
                    return;
            }

            string bsoName = BSONameForShowLabOrder;
            if (String.IsNullOrEmpty(bsoName))
                bsoName = "BSOLabOrder(Dialog)";

            ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
            if (childBSO == null)
                childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            childBSO.ACUrlCommand("!ShowLabOrderViewDialog", null, null, null, null, null, labOrder, true, orderInfo);
            childBSO.Stop();
        }

        public virtual void ShowInOrderDialog(string inOrderNo, Guid? inOrderPosID)
        {
            string bsoName = "BSOInOrder";
            ACComponent childBSO = Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
            if (childBSO == null)
                childBSO = Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            childBSO.ACUrlCommand("!ShowDialogOrder", inOrderNo, inOrderPosID);
        }

        public virtual bool IsEnabledShowLabOrder(IACComponent caller)
        {
            IACContainerTNet<String> orderInfoProp = null;
            if (!HasOrderInfo(caller, out orderInfoProp))
                return false;
            Type typeOfInstance = caller.ACType.ObjectType;
            if (typeOfInstance == null)
                return false;
            return true;
        }

        public virtual void GenerateNewLabOrder(IACComponent caller, bool createAlwaysNewOne = true)
        {
            if (caller == null)
                return;

            if (!IsEnabledGenerateNewLabOrder(caller))
                return;

            ACLabOrderManager labOrderMgr = ACLabOrderManager.GetServiceInstance(caller as ACComponent);
            if (labOrderMgr == null)
                return;

            PAOrderInfo orderInfo = QueryOrderInfo(caller);
            if (orderInfo == null)
                return;

            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    ProdOrderPartslistPos batchPos = null;
                    DeliveryNotePos dnPos = null;
                    PickingPos pickingPos = null;
                    FacilityBooking fBooking = null;
                    InOrderPos inOrderPos = null;
                    OutOrderPos outOrderPos = null;
                    LabOrder labOrder = null;

                    if (!labOrderMgr.ResolveEntities(dbApp, orderInfo, out batchPos, out dnPos, out pickingPos, out fBooking, out inOrderPos, out outOrderPos, out labOrder))
                        return;
                    if (createAlwaysNewOne)
                        labOrder = null;

                    LabOrder labOrderTemplate = null;
                    Material materialForTemplate = null;
                    if (batchPos != null)
                    {
                        if (batchPos.ProdOrderPartslist != null && batchPos.ProdOrderPartslist.Partslist != null && batchPos.ProdOrderPartslist.Partslist.Material != null)
                            materialForTemplate = batchPos.ProdOrderPartslist.Partslist.Material;
                        if (materialForTemplate == null)
                            materialForTemplate = batchPos.BookingMaterial;
                    }
                    else if (pickingPos != null)
                        materialForTemplate = pickingPos.Material;
                    else if (inOrderPos != null)
                        materialForTemplate = inOrderPos.Material;
                    else if (outOrderPos != null)
                        materialForTemplate = outOrderPos.Material;
                    else if (fBooking != null)
                    {
                        materialForTemplate = fBooking.InwardMaterial;
                        if (materialForTemplate == null)
                            materialForTemplate = fBooking.OutwardMaterial;
                    }

                    if (materialForTemplate != null)
                        labOrderTemplate = materialForTemplate.LabOrder_Material.Where(c => c.LabOrderTypeIndex == (short)GlobalApp.LabOrderType.Template).FirstOrDefault();

                    if (labOrder == null)
                    {
                        if (labOrderTemplate != null)
                            labOrderMgr.CreateNewLabOrder(dbApp, labOrderTemplate, caller.ACIdentifier, inOrderPos, outOrderPos, batchPos, null, pickingPos, out labOrder);
                        else
                            labOrderMgr.CreateNewLabOrder(dbApp, caller.ACIdentifier, inOrderPos, outOrderPos, batchPos, null, pickingPos, out labOrder);
                    }

                    if (labOrder != null)
                    {
                        MsgWithDetails msg = dbApp.ACSaveChanges();
                        if (msg != null)
                            caller.Messages.Msg(msg);
                        else
                            ShowLabOrder(caller, labOrder);
                    }
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("PAShowDlgManagerVB", "GenerateNewLabOrder", msg);
            }
        }

        public virtual bool IsEnabledGenerateNewLabOrder(IACComponent caller)
        {
            IACContainerTNet<String> orderInfoProp = null;
            if (!HasOrderInfo(caller, out orderInfoProp))
                return false;
            Type typeOfInstance = caller.ACType.ObjectType;
            if (typeOfInstance == null)
                return false;
            return true;
        }

        public virtual void ShowFacilityBookCellDialog(IACComponent caller)
        {
            if (caller == null)
                return;
            ACComponent paModule = caller as ACComponent;
            if (!IsEnabledShowFacilityBookCellDialog(caller))
                return;

            IACContainerTNet<ACRef<Facility>> facility = paModule.GetProperty(gip.mes.datamodel.Facility.ClassName) as IACContainerTNet<ACRef<Facility>>;
            if (facility == null || facility.ValueT == null || facility.ValueT.ValueT == null)
                return;

            ShowFacilityBookCellDialog(facility.ValueT.ValueT.FacilityNo);
        }

        public virtual void ShowFacilityBookCellDialog(string facilityNo)
        {
            string bsoName = BSONameForShowFacilityBookCell;
            if (String.IsNullOrEmpty(bsoName))
                bsoName = "BSOFacilityBookCell(Dialog)";

            ACComponent childBSO = Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
            if (childBSO == null)
                childBSO = Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            childBSO.ACUrlCommand("!ShowDialogFacility", facilityNo);
            childBSO.Stop();
        }

        public virtual bool IsEnabledShowFacilityBookCellDialog(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            IACContainerTNet<ACRef<Facility>> facility = _this.GetProperty(gip.mes.datamodel.Facility.ClassName) as IACContainerTNet<ACRef<Facility>>;
            if (facility != null && facility.ValueT != null && facility.ValueT.ValueT != null)
                return true;
            return false;
        }


        public virtual void ShowFacilityOverviewDialog(IACComponent caller)
        {
            if (caller == null)
                return;
            ACComponent paModule = caller as ACComponent;
            if (!IsEnabledShowFacilityOverviewDialog(caller))
                return;

            IACContainerTNet<ACRef<Facility>> facility = paModule.GetProperty(gip.mes.datamodel.Facility.ClassName) as IACContainerTNet<ACRef<Facility>>;
            if (facility == null || facility.ValueT == null || facility.ValueT.ValueT == null)
                return;

            ShowFacilityOverviewDialog(facility.ValueT.ValueT.FacilityNo);
        }

        public virtual void ShowFacilityOverviewDialog(string facilityNo, DateTime? searchFrom = null, DateTime? searchTo = null)
        {
            string bsoName = BSONameForShowFacilityOverview;
            if (String.IsNullOrEmpty(bsoName))
                bsoName = "BSOFacilityOverview(Dialog)";

            ACComponent childBSO = Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
            if (childBSO == null)
                childBSO = Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            childBSO.ACUrlCommand("!ShowDialogFacility", facilityNo, searchFrom, searchTo);
            childBSO.Stop();
        }

        public virtual bool IsEnabledShowFacilityOverviewDialog(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            IACContainerTNet<ACRef<Facility>> facility = _this.GetProperty(gip.mes.datamodel.Facility.ClassName) as IACContainerTNet<ACRef<Facility>>;
            if (facility != null && facility.ValueT != null && facility.ValueT.ValueT != null)
                return true;
            return false;
        }



        #endregion

        #region OrderInfo methods
        public static PAOrderInfo QueryOrderInfo(IACComponent caller)
        {
            if (caller == null)
                return null;
            PAOrderInfo orderInfo = null;
            if (caller is IACComponentPWNode)
            {
                orderInfo = caller.ACUrlCommand(ACUrlHelper.Delimiter_InvokeMethod + nameof(PWBase.GetPAOrderInfo)) as PAOrderInfo;
            }
            else
            {
                string[] accessedFromVBGroupACUrl = caller.ACUrlCommand(ACUrlHelper.Delimiter_InvokeMethod + nameof(PAProcessModule.SemaphoreAccessedFrom)) as string[];
                if (accessedFromVBGroupACUrl != null && accessedFromVBGroupACUrl.Any())
                {
                    string firstModule = accessedFromVBGroupACUrl[0];
                    if (!String.IsNullOrEmpty(firstModule))
                        orderInfo = caller.ACUrlCommand(accessedFromVBGroupACUrl[0] + ACUrlHelper.Delimiter_InvokeMethod + nameof(PWBase.GetPAOrderInfo)) as PAOrderInfo;
                }
                else
                {
                    orderInfo = caller.ACUrlCommand(ACUrlHelper.Delimiter_InvokeMethod + nameof(PAProcessModule.GetPAOrderInfo)) as PAOrderInfo;
                }
            }
            return orderInfo;
        }

        public static bool HasOrderInfo(IACComponent caller, out IACContainerTNet<String> orderInfoProp)
        {
            orderInfoProp = null;
            if (caller == null)
                return false;
            if (caller is IACComponentPWNode)
                return true;
            orderInfoProp = caller.GetProperty(nameof(PAProcessModule.OrderInfo)) as IACContainerTNet<String>;
            if (orderInfoProp == null)
                return false;
            if (String.IsNullOrEmpty(orderInfoProp.ValueT))
                return false;
            return true;
        }

        public override string BuildAndSetOrderInfo(PAProcessModule pm)
        {
            throw new NotImplementedException();
        }

        public override string BuildOrderInfo(PWBase pw)
        {
            throw new NotImplementedException();
        }


        public PAOrderInfo GetPAOrderInfoForFaciliyBooking(DatabaseApp databaseApp, FacilityBooking facilityBooking, FacilityBookingOverview facilityBookingOverview, PresenterMenuItems menuItemType)
        {
            PAOrderInfo pAOrderInfo = new PAOrderInfo();
            switch (menuItemType)
            {
                case PresenterMenuItems.ProdOrderPartslist:
                    pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                    {
                        EntityID = facilityBooking.FacilityBookingID,
                        EntityName = FacilityBooking.ClassName
                    });
                    if (facilityBooking.ProdOrderPartslistPosRelationID != null)
                    {
                        pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = facilityBooking.ProdOrderPartslistPosRelationID.Value,
                            EntityName = ProdOrderPartslistPosRelation.ClassName
                        });
                    }
                    if (facilityBooking.ProdOrderPartslistPosID != null)
                    {
                        pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = facilityBooking.ProdOrderPartslistPosID.Value,
                            EntityName = ProdOrderPartslistPos.ClassName
                        });
                    }
                    break;
                case PresenterMenuItems.InOrderPos:
                    InOrder inOrder = databaseApp.InOrder.FirstOrDefault(c => c.InOrderNo == facilityBookingOverview.InwardFacilityChargeInOrderNo);
                    pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                    {
                        EntityID = inOrder.InOrderID,
                        EntityName = InOrder.ClassName
                    });
                    break;
                case PresenterMenuItems.DeliveryNotePos:
                    DeliveryNotePos dns = null;
                    if(facilityBooking.InOrderPos != null)
                    {
                        dns = facilityBooking.InOrderPos.DeliveryNotePos_InOrderPos.FirstOrDefault();
                    }
                    if (dns == null)
                    {
                        dns = 
                            databaseApp
                            .DeliveryNote
                            .Where(c=>c.DeliveryNoteNo == facilityBookingOverview.DeliveryNoteNo)
                            .SelectMany(c=>c.DeliveryNotePos_DeliveryNote)
                            .OrderBy(c=>c.Sequence)
                            .FirstOrDefault();
                    }
                    pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                    {
                        EntityID = dns.DeliveryNotePosID,
                        EntityName = DeliveryNotePos.ClassName
                    });
                    break;
                case PresenterMenuItems.PickingPos:
                    PickingPos pickingPos = facilityBooking.PickingPos;
                    pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                    {
                        EntityID = pickingPos.PickingPosID,
                        EntityName = PickingPos.ClassName
                    });
                    break;
                default:
                    break;
            }

            return pAOrderInfo;
        }

        public PAOrderInfo GetPAOrderInfoForFaciliyBookingCharge(DatabaseApp databaseApp, FacilityBookingCharge facilityBookingCharge, PresenterMenuItems menuItemType)
        {
            PAOrderInfo pAOrderInfo = new PAOrderInfo();
            switch (menuItemType)
            {
                case PresenterMenuItems.ProdOrderPartslist:
                    pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                    {
                        EntityID = facilityBookingCharge.FacilityBookingID,
                        EntityName = FacilityBooking.ClassName
                    });
                    if (facilityBookingCharge.ProdOrderPartslistPosRelationID != null)
                    {
                        pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = facilityBookingCharge.ProdOrderPartslistPosRelationID.Value,
                            EntityName = ProdOrderPartslistPosRelation.ClassName
                        });
                    }
                    if (facilityBookingCharge.ProdOrderPartslistPosID != null)
                    {
                        pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = facilityBookingCharge.ProdOrderPartslistPosID.Value,
                            EntityName = ProdOrderPartslistPos.ClassName
                        });
                    }
                    break;
                case PresenterMenuItems.InOrderPos:
                    InOrderPos inOrderPos = facilityBookingCharge.InOrderPos;
                    string inOrderNo = facilityBookingCharge.InOrderPos.InOrder.InOrderNo;
                    InOrder inOrder = databaseApp.InOrder.FirstOrDefault(c => c.InOrderNo == inOrderNo);
                    pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                    {
                        EntityID = inOrder.InOrderID,
                        EntityName = InOrder.ClassName
                    });
                    break;
                case PresenterMenuItems.DeliveryNotePos:
                    DeliveryNotePos dns = facilityBookingCharge.InOrderPos.DeliveryNotePos_InOrderPos.FirstOrDefault();
                    pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                    {
                        EntityID = dns.DeliveryNotePosID,
                        EntityName = DeliveryNotePos.ClassName
                    });
                    break;
                case PresenterMenuItems.PickingPos:
                    PickingPos pickingPos = facilityBookingCharge.PickingPos;
                    pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                    {
                        EntityID = pickingPos.PickingPosID,
                        EntityName = PickingPos.ClassName
                    });
                    break;
                default:
                    break;
            }

            return pAOrderInfo;
        }

        #endregion

        #endregion

    }
}
