using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.manager;
using System;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;

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
            _C_BSONameForShowFacilityOverview = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowFacilityOverview), "");
            _C_BSONameForShowOrder = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowOrder), "");
            _C_BSONameForShowComponent = new ACPropertyConfigValue<string>(this, nameof(BSONameForShowComponent), "");
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
                if (!String.IsNullOrEmpty(_C_BSONameForShowOrder.ValueT))
                    return _C_BSONameForShowOrder.ValueT;
                gip.core.datamodel.ACClass classOfBso = (Root.Database as Database).GetACType("BSOProdOrder");
                if (classOfBso != null)
                {
                    gip.core.datamodel.ACClass derivation = null;
                    using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                    {
                        derivation = gip.core.datamodel.Database.GlobalDatabase.ACClass
                                                .Where(c => c.BasedOnACClassID == classOfBso.ACClassID
                                                        && !String.IsNullOrEmpty(c.AssemblyQualifiedName)
                                                        && c.AssemblyQualifiedName != classOfBso.AssemblyQualifiedName).FirstOrDefault();
                    }
                    if (derivation != null)
                        _C_BSONameForShowOrder.ValueT = derivation.ACIdentifier + "(Dialog)";
                }

                if (String.IsNullOrEmpty(_C_BSONameForShowOrder.ValueT))
                    _C_BSONameForShowOrder.ValueT = "BSOProdOrder(Dialog)";
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
                if (!String.IsNullOrEmpty(_C_BSONameForShowComponent.ValueT))
                    return _C_BSONameForShowComponent.ValueT;
                gip.core.datamodel.ACClass classOfBso = (Root.Database as Database).GetACType("BSOProdOrderBatchComponents");
                if (classOfBso != null)
                {
                    gip.core.datamodel.ACClass derivation = null;
                    using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                    {
                        derivation = gip.core.datamodel.Database.GlobalDatabase.ACClass
                                                .Where(c => c.BasedOnACClassID == classOfBso.ACClassID
                                                        && !String.IsNullOrEmpty(c.AssemblyQualifiedName)
                                                        && c.AssemblyQualifiedName != classOfBso.AssemblyQualifiedName).FirstOrDefault();
                    }
                    if (derivation != null)
                        _C_BSONameForShowComponent.ValueT = derivation.ACIdentifier + "(Dialog)";
                }

                if (String.IsNullOrEmpty(_C_BSONameForShowComponent.ValueT))
                    _C_BSONameForShowComponent.ValueT = "BSOProdOrderBatchComponents(Dialog)";
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
                if (!String.IsNullOrEmpty(_C_BSONameForShowReservation.ValueT))
                    return _C_BSONameForShowReservation.ValueT;
                gip.core.datamodel.ACClass classOfBso = (Root.Database as Database).GetACType("BSOComponentReservation");
                if (classOfBso != null)
                {
                    gip.core.datamodel.ACClass derivation = null;
                    using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                    {
                        derivation = gip.core.datamodel.Database.GlobalDatabase.ACClass
                                                .Where(c => c.BasedOnACClassID == classOfBso.ACClassID
                                                        && !String.IsNullOrEmpty(c.AssemblyQualifiedName)
                                                        && c.AssemblyQualifiedName != classOfBso.AssemblyQualifiedName).FirstOrDefault();
                    }
                    if (derivation != null)
                        _C_BSONameForShowReservation.ValueT = derivation.ACIdentifier + "(Dialog)";
                }

                if (String.IsNullOrEmpty(_C_BSONameForShowReservation.ValueT))
                    _C_BSONameForShowReservation.ValueT = "BSOComponentReservation(Dialog)";
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
                if (!String.IsNullOrEmpty(_C_BSONameForShowProgramLog.ValueT))
                    return _C_BSONameForShowProgramLog.ValueT;
                gip.core.datamodel.ACClass classOfBso = typeof(PresenterProgramLogVB).GetACType() as gip.core.datamodel.ACClass;
                if (classOfBso != null)
                {
                    gip.core.datamodel.ACClass derivation = null;
                    using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                    {
                        derivation = gip.core.datamodel.Database.GlobalDatabase.ACClass
                                                .Where(c => c.BasedOnACClassID == classOfBso.ACClassID
                                                        && !String.IsNullOrEmpty(c.AssemblyQualifiedName)
                                                        && c.AssemblyQualifiedName != classOfBso.AssemblyQualifiedName).FirstOrDefault();
                    }
                    if (derivation != null)
                        _C_BSONameForShowProgramLog.ValueT = derivation.ACIdentifier;
                }

                if (String.IsNullOrEmpty(_C_BSONameForShowProgramLog.ValueT))
                    _C_BSONameForShowProgramLog.ValueT = "PresenterProgramLogVB";
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
                if (!String.IsNullOrEmpty(_C_BSONameForShowPicking.ValueT))
                    return _C_BSONameForShowPicking.ValueT;
                gip.core.datamodel.ACClass classOfBso = (Root.Database as Database).GetACType("BSOPicking");
                if (classOfBso != null)
                {
                    gip.core.datamodel.ACClass derivation = null;
                    using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                    {
                        derivation = gip.core.datamodel.Database.GlobalDatabase.ACClass
                                                .Where(c => c.BasedOnACClassID == classOfBso.ACClassID
                                                        && !String.IsNullOrEmpty(c.AssemblyQualifiedName)
                                                        && c.AssemblyQualifiedName != classOfBso.AssemblyQualifiedName).FirstOrDefault();
                    }
                    if (derivation != null)
                        _C_BSONameForShowPicking.ValueT = derivation.ACIdentifier + "(Dialog)";
                }

                if (String.IsNullOrEmpty(_C_BSONameForShowPicking.ValueT))
                    _C_BSONameForShowPicking.ValueT = "BSOPicking(Dialog)";
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
                if (!String.IsNullOrEmpty(_C_BSONameForShowInDeliveryNote.ValueT))
                    return _C_BSONameForShowInDeliveryNote.ValueT;
                gip.core.datamodel.ACClass classOfBso = (Root.Database as Database).GetACType("BSOInDeliveryNote");
                if (classOfBso != null)
                {
                    gip.core.datamodel.ACClass derivation = null;
                    using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                    {
                        derivation = gip.core.datamodel.Database.GlobalDatabase.ACClass
                                                .Where(c => c.BasedOnACClassID == classOfBso.ACClassID
                                                        && !String.IsNullOrEmpty(c.AssemblyQualifiedName)
                                                        && c.AssemblyQualifiedName != classOfBso.AssemblyQualifiedName).FirstOrDefault();
                    }
                    if (derivation != null)
                        _C_BSONameForShowInDeliveryNote.ValueT = derivation.ACIdentifier + "(Dialog)";
                }

                if (String.IsNullOrEmpty(_C_BSONameForShowInDeliveryNote.ValueT))
                    _C_BSONameForShowInDeliveryNote.ValueT = "BSOInDeliveryNote(Dialog)";
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
                if (!String.IsNullOrEmpty(_C_BSONameForShowOutDeliveryNote.ValueT))
                    return _C_BSONameForShowOutDeliveryNote.ValueT;
                gip.core.datamodel.ACClass classOfBso = (Root.Database as Database).GetACType("BSOOutDeliveryNote");
                if (classOfBso != null)
                {
                    gip.core.datamodel.ACClass derivation = null;
                    using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                    {
                        derivation = gip.core.datamodel.Database.GlobalDatabase.ACClass
                                                .Where(c => c.BasedOnACClassID == classOfBso.ACClassID
                                                        && !String.IsNullOrEmpty(c.AssemblyQualifiedName)
                                                        && c.AssemblyQualifiedName != classOfBso.AssemblyQualifiedName).FirstOrDefault();
                    }
                    if (derivation != null)
                        _C_BSONameForShowOutDeliveryNote.ValueT = derivation.ACIdentifier + "(Dialog)";
                }

                if (String.IsNullOrEmpty(_C_BSONameForShowOutDeliveryNote.ValueT))
                    _C_BSONameForShowOutDeliveryNote.ValueT = "BSOOutDeliveryNote(Dialog)";
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
                if (!String.IsNullOrEmpty(_C_BSONameForShowLabOrder.ValueT))
                    return _C_BSONameForShowLabOrder.ValueT;
                gip.core.datamodel.ACClass classOfBso = (Root.Database as Database).GetACType("BSOLabOrderMES");
                if (classOfBso != null)
                {
                    gip.core.datamodel.ACClass derivation = null;
                    using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                    {
                        derivation = gip.core.datamodel.Database.GlobalDatabase.ACClass
                                                .Where(c => c.BasedOnACClassID == classOfBso.ACClassID
                                                        && !String.IsNullOrEmpty(c.AssemblyQualifiedName)
                                                        && c.AssemblyQualifiedName != classOfBso.AssemblyQualifiedName).FirstOrDefault();
                    }
                    if (derivation != null)
                        _C_BSONameForShowLabOrder.ValueT = derivation.ACIdentifier + "(Dialog)";
                }

                if (String.IsNullOrEmpty(_C_BSONameForShowLabOrder.ValueT))
                    _C_BSONameForShowLabOrder.ValueT = "BSOLabOrderMES(Dialog)";
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
                if (!String.IsNullOrEmpty(_C_BSONameForShowFacilityBookCell.ValueT))
                    return _C_BSONameForShowFacilityBookCell.ValueT;
                gip.core.datamodel.ACClass classOfBso = (Root.Database as Database).GetACType("BSOFacilityBookCell");
                if (classOfBso != null)
                {
                    gip.core.datamodel.ACClass derivation = null;
                    using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                    {
                        derivation = gip.core.datamodel.Database.GlobalDatabase.ACClass
                                                .Where(c => c.BasedOnACClassID == classOfBso.ACClassID
                                                        && !String.IsNullOrEmpty(c.AssemblyQualifiedName)
                                                        && c.AssemblyQualifiedName != classOfBso.AssemblyQualifiedName).FirstOrDefault();
                    }
                    if (derivation != null)
                        _C_BSONameForShowFacilityBookCell.ValueT = derivation.ACIdentifier + "(Dialog)";
                }

                if (String.IsNullOrEmpty(_C_BSONameForShowFacilityBookCell.ValueT))
                    _C_BSONameForShowFacilityBookCell.ValueT = "BSOFacilityBookCell(Dialog)";
                return _C_BSONameForShowFacilityBookCell.ValueT;
            }
            set
            {
                _C_BSONameForShowFacilityBookCell.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _C_BSONameForShowFacilityOverview;
        [ACPropertyConfig("en{'Classname and ACIdentifier for Facility-Overview'}de{'Klassenname und ACIdentifier für Lagerplatzübersicht'}")]
        public string BSONameForShowFacilityOverview
        {
            get
            {
                if (!String.IsNullOrEmpty(_C_BSONameForShowFacilityOverview.ValueT))
                    return _C_BSONameForShowFacilityOverview.ValueT;
                gip.core.datamodel.ACClass classOfBso = (Root.Database as Database).GetACType("BSOFacilityOverview");
                if (classOfBso != null)
                {
                    gip.core.datamodel.ACClass derivation = null;
                    using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                    {
                        derivation = gip.core.datamodel.Database.GlobalDatabase.ACClass
                                                .Where(c => c.BasedOnACClassID == classOfBso.ACClassID
                                                        && !String.IsNullOrEmpty(c.AssemblyQualifiedName)
                                                        && c.AssemblyQualifiedName != classOfBso.AssemblyQualifiedName).FirstOrDefault();
                    }
                    if (derivation != null)
                        _C_BSONameForShowFacilityOverview.ValueT = derivation.ACIdentifier + "(Dialog)";
                }

                if (String.IsNullOrEmpty(_C_BSONameForShowFacilityOverview.ValueT))
                    _C_BSONameForShowFacilityOverview.ValueT = "BSOFacilityOverview(Dialog)";
                return _C_BSONameForShowFacilityOverview.ValueT;
            }
            set
            {
                _C_BSONameForShowFacilityOverview.ValueT = value;
            }
        }

        #endregion

        #region Precompiled Query
        public static readonly Func<DatabaseApp, Guid, IQueryable<ProdOrderBatch>> s_cQry_BatchInfo =
        CompiledQuery.Compile<DatabaseApp, Guid, IQueryable<ProdOrderBatch>>(
            (ctx, id) => ctx.ProdOrderBatch.Include("ProdOrderPartslist")
                                 .Include("ProdOrderPartslist.ProdOrder")
                                 .Include("ProdOrderPartslist.Partslist")
                                 .Include("ProdOrderPartslist.Partslist.Material")
                                 .Include("ProdOrderBatchPlan")
                                 .Include("ProdOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan")
                                 .Include("ProdOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan.Facility")
                                 .Where(c => c.ProdOrderBatchID == id)
        );

        public static readonly Func<DatabaseApp, Guid, IQueryable<ProdOrderPartslistPos>> s_cQry_POPosInfo =
        CompiledQuery.Compile<DatabaseApp, Guid, IQueryable<ProdOrderPartslistPos>>(
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

        public static readonly Func<DatabaseApp, Guid, IQueryable<Picking>> s_cQry_PickingInfo =
        CompiledQuery.Compile<DatabaseApp, Guid, IQueryable<Picking>>(
            (ctx, id) => ctx.Picking.Include("PickingPos_Picking")
                                 .Include("PickingPos_Picking.ToFacility")
                                 .Where(c => c.PickingID == id)
        );

        public static readonly Func<DatabaseApp, Guid, IQueryable<DeliveryNotePos>> s_cQry_DeliveryNotePosInfo =
        CompiledQuery.Compile<DatabaseApp, Guid, IQueryable<DeliveryNotePos>>(
            (ctx, id) => ctx.DeliveryNotePos.Include("DeliveryNote")
                                    .Include("InOrderPos")
                                    .Include("InOrderPos.Material")
                                    .Include("OutOrderPos")
                                    .Include("OutOrderPos.Material")
                                 .Where(c => c.DeliveryNotePosID == id)
        );

        public static readonly Func<DatabaseApp, Guid, IQueryable<FacilityBooking>> s_cQry_FacilityBookingInfo =
        CompiledQuery.Compile<DatabaseApp, Guid, IQueryable<FacilityBooking>>(
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
                    }
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
                    string bsoName = BSONameForShowOrder;
                    if (String.IsNullOrEmpty(bsoName))
                        bsoName = "BSOProdOrder(Dialog)";
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

        #endregion

        #endregion

    }
}
