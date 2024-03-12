using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Objects;
using System.Linq;
using static gip.mes.facility.ACPartslistManager;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioLogistics, "", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class ACPickingManager : PARole
    {
        #region c´tors
        public ACPickingManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        protected override void Construct(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            base.Construct(acType, content, parentACObject, parameter, acIdentifier);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override bool ACPostInit()
        {
            bool init = base.ACPostInit();
            _RoutingService = ACRoutingService.ACRefToServiceInstance(this);
            return init;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACRoutingService.DetachACRefFromServiceInstance(this, _RoutingService);
            _RoutingService = null;

            bool result = base.ACDeInit(deleteACClassTask);
            return result;
        }

        public override bool IsPoolable
        {
            get
            {
                return true;
            }
        }


        #endregion

        #region PrecompiledQueries
        static readonly Func<DatabaseApp, IQueryable<MDDelivPosState>> s_cQry_CompletelyAssigned =
        CompiledQuery.Compile<DatabaseApp, IQueryable<MDDelivPosState>>(
            (ctx) => from c in ctx.MDDelivPosState where c.MDDelivPosStateIndex == (Int16)MDDelivPosState.DelivPosStates.CompletelyAssigned select c
        );

        static readonly Func<DatabaseApp, IQueryable<MDDelivPosState>> s_cQry_SubsetAssigned =
        CompiledQuery.Compile<DatabaseApp, IQueryable<MDDelivPosState>>(
            (ctx) => from c in ctx.MDDelivPosState where c.MDDelivPosStateIndex == (Int16)MDDelivPosState.DelivPosStates.SubsetAssigned select c
        );

        static readonly Func<DatabaseApp, IQueryable<MDDelivPosState>> s_cQry_NotPlanned =
        CompiledQuery.Compile<DatabaseApp, IQueryable<MDDelivPosState>>(
            (ctx) => from c in ctx.MDDelivPosState where c.MDDelivPosStateIndex == (Int16)MDDelivPosState.DelivPosStates.NotPlanned select c
        );

        static readonly Func<DatabaseApp, IQueryable<MDInOrderPosState>> s_cQry_InOrderInProcess =
        CompiledQuery.Compile<DatabaseApp, IQueryable<MDInOrderPosState>>(
            (ctx) => from c in ctx.MDInOrderPosState where c.MDInOrderPosStateIndex == (Int16)MDInOrderPosState.InOrderPosStates.InProcess select c
        );

        static readonly Func<DatabaseApp, IQueryable<MDInOrderPosState>> s_cQry_InOrderCompleted =
        CompiledQuery.Compile<DatabaseApp, IQueryable<MDInOrderPosState>>(
            (ctx) => from c in ctx.MDInOrderPosState where c.MDInOrderPosStateIndex == (Int16)MDInOrderPosState.InOrderPosStates.Completed select c
        );

        static readonly Func<DatabaseApp, IQueryable<MDOutOrderPosState>> s_cQry_OutOrderInProcess =
        CompiledQuery.Compile<DatabaseApp, IQueryable<MDOutOrderPosState>>(
            (ctx) => from c in ctx.MDOutOrderPosState where c.MDOutOrderPosStateIndex == (Int16)MDOutOrderPosState.OutOrderPosStates.InProcess select c
        );

        static readonly Func<DatabaseApp, IQueryable<MDOutOrderPosState>> s_cQry_OutOrderCompleted =
        CompiledQuery.Compile<DatabaseApp, IQueryable<MDOutOrderPosState>>(
            (ctx) => from c in ctx.MDOutOrderPosState where c.MDOutOrderPosStateIndex == (Int16)MDOutOrderPosState.OutOrderPosStates.Completed select c
        );

        #region Batch -> Select batch
        protected static readonly Func<DatabaseApp, Guid?, short, short, DateTime?, DateTime?, string, string, IQueryable<Picking>> s_cQry_PickingForPWNode =
        CompiledQuery.Compile<DatabaseApp, Guid?, short, short, DateTime?, DateTime?, string, string, IQueryable<Picking>>(
            (ctx, mdSchedulingGroupID, greaterEqualState, lessEqualState, filterStartTime, filterEndTime, pickingNo, materialNo) =>
                                    ctx.Picking
                                    .Include("PickingPos_Picking.PickingMaterial")
                                    .Include("PickingPos_Picking.OutOrderPos.Material")
                                    .Include("PickingPos_Picking.InOrderPos.Material")
                                    .Include("PickingPos_Picking.ToFacility")
                                    .Include("PickingPos_Picking.FromFacility")
                                    .Include("PickingPos_Picking.FacilityReservation_PickingPos.FacilityLot")
                                    .Include("PickingPos_Picking.FacilityReservation_PickingPos.Facility")
                                    .Include("PickingPos_Picking.FacilityReservation_PickingPos.Facility.Material")
                                    .Include("PickingPos_Picking.FacilityReservation_PickingPos.Facility.FacilityStock_Facility")
                                    .Where(c => (mdSchedulingGroupID == null || (c.VBiACClassWF != null && c.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Any(x => x.MDSchedulingGroupID == (mdSchedulingGroupID ?? Guid.Empty))))
                                            && (   c.PickingStateIndex >= greaterEqualState
                                                || c.PickingStateIndex <= lessEqualState)
                                            && (string.IsNullOrEmpty(pickingNo) || c.PickingNo.Contains(pickingNo))
                                            && (
                                                    string.IsNullOrEmpty(materialNo)
                                                    || (c.PickingPos_Picking.Any(x => x.PickingMaterial != null && x.PickingMaterial.MaterialNo.Contains(materialNo)))
                                                )
                                            && (filterStartTime == null
                                                 || (c.ScheduledStartDate != null && c.ScheduledStartDate >= filterStartTime)
                                                 || (c.ScheduledStartDate == null && c.UpdateDate >= filterStartTime)
                                                )
                                            && (filterEndTime == null
                                                 || (c.ScheduledEndDate != null && c.ScheduledEndDate < filterEndTime)
                                                 || (c.ScheduledEndDate == null && c.ScheduledStartDate != null && c.ScheduledStartDate <= filterEndTime)
                                                 || (c.ScheduledEndDate == null && c.ScheduledStartDate == null && c.UpdateDate <= filterEndTime)
                                                )
                                          )
                                    .OrderBy(c => c.ScheduledOrder ?? 0)
                                    .ThenBy(c => c.InsertDate)
        );


        public ObjectQuery<Picking> GetScheduledPickings(
            DatabaseApp databaseApp,
            Guid? mdSchedulingGroupID,
            PickingStateEnum greaterEqualState,
            PickingStateEnum lessEqualState,
            DateTime? filterStartTime,
            DateTime? filterEndTime,
            string pickingNo,
            string materialNo)
        {
            ObjectQuery<Picking> query = s_cQry_PickingForPWNode(databaseApp, mdSchedulingGroupID, (short)greaterEqualState,
                (short)lessEqualState, filterStartTime, filterEndTime, pickingNo, materialNo) as ObjectQuery<Picking>;
            query.MergeOption = MergeOption.OverwriteChanges;
            return query;
        }


        #endregion

        #endregion

        #region Properties
        protected ACRef<ACComponent> _RoutingService = null;
        public override ACComponent RoutingService
        {
            get
            {
                if (_RoutingService == null)
                    return null;
                return _RoutingService.ValueT;
            }
        }

        ACMethodBooking _BookParamOutCancelClone;
        public ACMethodBooking BookParamOutCancelClone(ACComponent facilityManager, DatabaseApp dbApp)
        {
            if (_BookParamOutCancelClone != null)
                return _BookParamOutCancelClone;
            _BookParamOutCancelClone = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_OutOrderPosCancel.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            _BookParamOutCancelClone.Database = dbApp;
            return _BookParamOutCancelClone;
        }

        private ACMethodBooking _BookParamZeroStockFacilityChargeClone;
        public ACMethodBooking BookParamZeroStockFacilityChargeClone(ACComponent facilityManager, DatabaseApp dbApp)
        {
            if (_BookParamZeroStockFacilityChargeClone != null)
                return _BookParamZeroStockFacilityChargeClone;

            _BookParamZeroStockFacilityChargeClone = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_FacilityCharge, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            _BookParamZeroStockFacilityChargeClone.Database = dbApp;

            return _BookParamZeroStockFacilityChargeClone;
        }

        #endregion

        #region static Methods
        public const string C_DefaultServiceACIdentifier = "PickingManager";

        public static ACPickingManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACPickingManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ACPickingManager> ACRefToServiceInstance(ACComponent requester)
        {
            ACPickingManager serviceInstance = GetServiceInstance(requester) as ACPickingManager;
            if (serviceInstance != null)
                return new ACRef<ACPickingManager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region Public Methods

        #region InOrder

        [ACMethodInfo("", "en{'Assign Order line'}de{'Zuordnen Auftragsposition'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg AssignInOrderPos(Picking currentPicking, InOrderPos currentInOrderPos1Level, Nullable<double> enteredPartialQuantity, DatabaseApp dbApp, List<object> resultNewEntities, bool autoInsertFirstPickingPos = true, bool callQuantityExceedable = false)
        {
            if (!PreExecute("AssignInOrderPos"))
            {
                //""
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50018")
                };
            }

            if (currentPicking == null || currentInOrderPos1Level == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50011"));
            }

            // Falls nicht von erster Ebene, dann kann Position nicht zugewiesen werden
            if (currentInOrderPos1Level.InOrderPos1_ParentInOrderPos != null)
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignInOrderPos(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50012")
                };
            }


            var queryDelivStateAssigned = s_cQry_CompletelyAssigned.Invoke(dbApp);
            if (!queryDelivStateAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignInOrderPos(2)",
                    Message = Root.Environment.TranslateMessage(this, "Error50013")
                };
            }

            var queryDelivStateSubsetAssigned = s_cQry_SubsetAssigned.Invoke(dbApp);
            if (!queryDelivStateSubsetAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignInOrderPos(3)",
                    Message = Root.Environment.TranslateMessage(this, "Error50014")
                };
            }

            if (enteredPartialQuantity.HasValue && (enteredPartialQuantity.Value - 0 <= Double.Epsilon))
                return null;

            // 1. Zweite Ebene einfügen (für Lieferscheinzuordnung vorbereiten)
            // Suche zuerst ob es bereits eine zweite Ebene für diese Auftragsposition im Kommissionierauftrag gibt
            if (currentPicking.EntityState != System.Data.EntityState.Added)
                currentPicking.PickingPos_Picking.AutoLoad(dbApp);

            currentInOrderPos1Level.AutoRefresh(dbApp);

            // 2. Rufe Teilmenge ab
            double partialQuantityUOM = 0;
            if (enteredPartialQuantity.HasValue)
            {
                partialQuantityUOM = currentInOrderPos1Level.Material.ConvertToBaseQuantity(enteredPartialQuantity.Value, currentInOrderPos1Level.MDUnit);
                if (!callQuantityExceedable && (currentInOrderPos1Level.RemainingCallQuantityUOM + 0.000001) < partialQuantityUOM)
                //|| currentInOrderPos1Level.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.CompletelyAssigned
                //|| currentInOrderPos1Level.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.Delivered)
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "AssignInOrderPos(4)",
                        Message = Root.Environment.TranslateMessage(this, "Warning50001", enteredPartialQuantity.Value, currentInOrderPos1Level.MDUnit.MDUnitName, currentInOrderPos1Level.RemainingCallQuantity, currentInOrderPos1Level.MDUnit.MDUnitName)
                    };
                }
                currentInOrderPos1Level.CalledUpQuantityUOM += partialQuantityUOM;
                if (currentInOrderPos1Level.RemainingCallQuantityUOM <= Double.Epsilon)
                    currentInOrderPos1Level.MDDelivPosState = queryDelivStateAssigned.First();
                else
                    currentInOrderPos1Level.MDDelivPosState = queryDelivStateSubsetAssigned.First();
            }
            else
            {
                partialQuantityUOM = currentInOrderPos1Level.RemainingCallQuantityUOM;
                currentInOrderPos1Level.CalledUpQuantityUOM = currentInOrderPos1Level.TargetQuantityUOM;
                currentInOrderPos1Level.MDDelivPosState = queryDelivStateAssigned.First();
            }

            InOrderPos inOrderPos2Level = currentPicking.PickingPos_Picking.ToArray().Where(c => c.InOrderPos != null && c.InOrderPos.TopParentInOrderPos == currentInOrderPos1Level).Select(c => c.InOrderPos.InOrderPos1_ParentInOrderPos).FirstOrDefault();
            if (inOrderPos2Level == null)
            {
                inOrderPos2Level = InOrderPos.NewACObject(dbApp, currentInOrderPos1Level);
                inOrderPos2Level.MaterialPosType = GlobalApp.MaterialPosTypes.InwardPart;
                var queryInOrderPosState = s_cQry_InOrderInProcess.Invoke(dbApp);
                if (!queryInOrderPosState.Any())
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "AssignInOrderPos(3)",
                        Message = Root.Environment.TranslateMessage(this, "Error50014")
                    };
                }
                inOrderPos2Level.MDInOrderPosState = queryInOrderPosState.FirstOrDefault();
                inOrderPos2Level.TargetQuantityUOM += partialQuantityUOM;
            }
            else
                inOrderPos2Level.TargetQuantityUOM = partialQuantityUOM;

            resultNewEntities.Add(inOrderPos2Level);

            Msg subMsg = null;
            // Dritte Ebene einfügen für Kommissionierposition
            if (autoInsertFirstPickingPos)
            {
                subMsg = AssignDNoteInOrderPos(currentPicking, inOrderPos2Level, enteredPartialQuantity, dbApp, resultNewEntities, callQuantityExceedable);
            }

            PostExecute("AssignInOrderPos");
            return subMsg;
        }

        [ACMethodInfo("", "en{'Assign Deliverynote line'}de{'Zuordnen Lieferscheinposition'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg AssignDNoteInOrderPos(Picking currentPicking, InOrderPos currentDNoteInOrderPos2Level, Nullable<double> enteredPartialQuantity, DatabaseApp dbApp, List<object> resultNewEntities, bool callQuantityExceedable = false)
        {
            if (!PreExecute("AssignDNoteInOrderPos"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignDNoteInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50004")
                };
            }

            if (currentPicking == null || currentDNoteInOrderPos2Level == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50011"));
            }
            if (currentDNoteInOrderPos2Level.InOrderPos1_ParentInOrderPos == null) // Es können auch Positionen zugeordnet werden, die noch nicht einem Lieferschein zugeordnet wurden: || !currentDNoteInOrderPos2Level.DeliveryNotePos_InOrderPos.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignDNoteInOrderPos(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50015")
                };
            }


            var queryDelivStateAssigned = s_cQry_CompletelyAssigned.Invoke(dbApp);
            if (!queryDelivStateAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignDNoteInOrderPos(2)",
                    Message = Root.Environment.TranslateMessage(this, "Error50013")
                };
            }

            var queryDelivStateSubsetAssigned = s_cQry_SubsetAssigned.Invoke(dbApp);
            if (!queryDelivStateSubsetAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignDNoteInOrderPos(3)",
                    Message = Root.Environment.TranslateMessage(this, "Error50014")
                };
            }

            if (enteredPartialQuantity.HasValue && (enteredPartialQuantity.Value - 0 <= Double.Epsilon))
                return null;

            // 1. Dritte Ebene einfügen für Kommissionierposition
            currentDNoteInOrderPos2Level.AutoRefresh(dbApp);

            // 2. Teilmenge abrufen
            double partialQuantityUOM = 0;
            if (enteredPartialQuantity.HasValue)
            {
                partialQuantityUOM = currentDNoteInOrderPos2Level.Material.ConvertToBaseQuantity(enteredPartialQuantity.Value, currentDNoteInOrderPos2Level.MDUnit);
                if (!callQuantityExceedable && (currentDNoteInOrderPos2Level.RemainingCallQuantityUOM + 0.000001) < partialQuantityUOM)
                //|| currentDNoteInOrderPos2Level.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.CompletelyAssigned
                //|| currentDNoteInOrderPos2Level.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.Delivered)
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "AssignInOrderPos(4)",
                        Message = Root.Environment.TranslateMessage(this, "Warning50001", enteredPartialQuantity.Value, currentDNoteInOrderPos2Level.MDUnit.MDUnitName, currentDNoteInOrderPos2Level.RemainingCallQuantity, currentDNoteInOrderPos2Level.MDUnit.MDUnitName)
                    };
                }
                currentDNoteInOrderPos2Level.CalledUpQuantityUOM += partialQuantityUOM;
                if (currentDNoteInOrderPos2Level.RemainingCallQuantityUOM <= Double.Epsilon)
                    currentDNoteInOrderPos2Level.MDDelivPosState = queryDelivStateAssigned.First();
                else
                    currentDNoteInOrderPos2Level.MDDelivPosState = queryDelivStateSubsetAssigned.First();
            }
            else
            {
                partialQuantityUOM = currentDNoteInOrderPos2Level.RemainingCallQuantityUOM;
                currentDNoteInOrderPos2Level.CalledUpQuantityUOM = currentDNoteInOrderPos2Level.TargetQuantityUOM;
                currentDNoteInOrderPos2Level.MDDelivPosState = queryDelivStateAssigned.First();
            }

            InOrderPos inOrderPos3Level = InOrderPos.NewACObject(dbApp, currentDNoteInOrderPos2Level);
            var queryInOrderPosState = s_cQry_InOrderInProcess.Invoke(dbApp);
            if (!queryInOrderPosState.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignInOrderPos(3)",
                    Message = Root.Environment.TranslateMessage(this, "Error50014")
                };
            }
            inOrderPos3Level.MDInOrderPosState = queryInOrderPosState.FirstOrDefault();
            inOrderPos3Level.MaterialPosType = GlobalApp.MaterialPosTypes.InwardPart;
            inOrderPos3Level.TargetQuantityUOM = partialQuantityUOM;

            resultNewEntities.Add(inOrderPos3Level);

            PickingPos pickingPos = PickingPos.NewACObject(dbApp, currentPicking);
            pickingPos.InOrderPos = inOrderPos3Level;
            resultNewEntities.Add(pickingPos);

            PostExecute("AssignDNoteInOrderPos");
            return null;
        }

        #endregion

        #region OutOrder

        [ACMethodInfo("", "en{'Assign order line'}de{'Zuordnen Auftragsposition'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg AssignOutOrderPos(Picking currentPicking, OutOrderPos currentOutOrderPos1Level, Nullable<double> enteredPartialQuantity, DatabaseApp dbApp, List<object> resultNewEntities, bool autoInsertFirstPickingPos = true, PickingPos alreadyExistingPos = null, bool callQuantityExceedable = false)
        {
            if (!PreExecute("AssignOutOrderPos"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50004")
                };
            }

            if (currentPicking == null || currentOutOrderPos1Level == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50011"));
            }

            // Falls nicht von erster Ebene, dann kann Position nicht zugewiesen werden
            if (currentOutOrderPos1Level.OutOrderPos1_ParentOutOrderPos != null)
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignOutOrderPos(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50012")
                };
            }

            var queryDelivStateAssigned = s_cQry_CompletelyAssigned.Invoke(dbApp);
            if (!queryDelivStateAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignOutOrderPos(2)",
                    Message = Root.Environment.TranslateMessage(this, "Error50013")
                };
            }

            var queryDelivStateSubsetAssigned = s_cQry_SubsetAssigned.Invoke(dbApp);
            if (!queryDelivStateSubsetAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignOutOrderPos(3)",
                    Message = Root.Environment.TranslateMessage(this, "Error50014")
                };
            }

            if (enteredPartialQuantity.HasValue && (enteredPartialQuantity.Value - 0 <= Double.Epsilon))
                return null;

            // 1. Zweite Ebene einfügen (für Lieferscheinzuordnung vorbereiten)
            // Suche zuerst ob es bereits eine zweite Ebene für diese Auftragsposition im Kommissionierauftrag gibt
            if (currentPicking.EntityState != System.Data.EntityState.Added)
                currentPicking.PickingPos_Picking.AutoLoad(dbApp);

            currentOutOrderPos1Level.AutoRefresh(dbApp);

            // 2. Rufe Teilmenge ab
            double partialQuantityUOM = 0;
            if (enteredPartialQuantity.HasValue)
            {
                partialQuantityUOM = currentOutOrderPos1Level.Material.ConvertToBaseQuantity(enteredPartialQuantity.Value, currentOutOrderPos1Level.MDUnit);
                if (!callQuantityExceedable && (currentOutOrderPos1Level.RemainingCallQuantityUOM + 0.000001) < partialQuantityUOM)
                //|| currentOutOrderPos1Level.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.CompletelyAssigned
                //|| currentOutOrderPos1Level.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.Delivered)
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "AssignOutOrderPos(4)",
                        Message = Root.Environment.TranslateMessage(this, "Warning50001", enteredPartialQuantity.Value, currentOutOrderPos1Level.MDUnit.MDUnitName, currentOutOrderPos1Level.RemainingCallQuantity, currentOutOrderPos1Level.MDUnit.MDUnitName)
                    };
                }
                currentOutOrderPos1Level.CalledUpQuantityUOM += partialQuantityUOM;
                if (currentOutOrderPos1Level.RemainingCallQuantityUOM <= Double.Epsilon)
                    currentOutOrderPos1Level.MDDelivPosState = queryDelivStateAssigned.First();
                else
                    currentOutOrderPos1Level.MDDelivPosState = queryDelivStateSubsetAssigned.First();
            }
            else
            {
                partialQuantityUOM = currentOutOrderPos1Level.RemainingCallQuantityUOM;
                currentOutOrderPos1Level.CalledUpQuantityUOM = currentOutOrderPos1Level.TargetQuantityUOM;
                currentOutOrderPos1Level.MDDelivPosState = queryDelivStateAssigned.First();
            }

            OutOrderPos outOrderPos2Level = currentPicking.PickingPos_Picking.ToArray().Where(c => c.OutOrderPos != null && c.OutOrderPos.TopParentOutOrderPos == currentOutOrderPos1Level).Select(c => c.OutOrderPos.OutOrderPos1_ParentOutOrderPos).FirstOrDefault();
            if (outOrderPos2Level == null)
            {
                outOrderPos2Level = OutOrderPos.NewACObject(dbApp, currentOutOrderPos1Level);
                outOrderPos2Level.MaterialPosType = GlobalApp.MaterialPosTypes.OutwardPart;
                var queryOutOrderPosState = s_cQry_OutOrderInProcess.Invoke(dbApp);
                if (!queryOutOrderPosState.Any())
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "AssignOutOrderPos(3)",
                        Message = Root.Environment.TranslateMessage(this, "Error50014")
                    };
                }
                outOrderPos2Level.MDOutOrderPosState = queryOutOrderPosState.FirstOrDefault();
                outOrderPos2Level.TargetQuantityUOM = partialQuantityUOM;
            }
            else
                outOrderPos2Level.TargetQuantityUOM += partialQuantityUOM;

            resultNewEntities.Add(outOrderPos2Level);

            Msg subMsg = null;
            // Dritte Ebene einfügen für Kommissionierposition
            if (autoInsertFirstPickingPos || alreadyExistingPos != null)
            {
                subMsg = AssignDNoteOutOrderPos(currentPicking, outOrderPos2Level, enteredPartialQuantity, dbApp, resultNewEntities, alreadyExistingPos, callQuantityExceedable);
            }

            PostExecute("AssignOutOrderPos");
            return subMsg;
        }

        [ACMethodInfo("", "en{'Assign Deliverynote line'}de{'Zuordnen Lieferscheinposition'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg AssignDNoteOutOrderPos(Picking currentPicking, OutOrderPos currentDNoteOutOrderPos2Level, Nullable<double> enteredPartialQuantity, DatabaseApp dbApp, List<object> resultNewEntities, PickingPos alreadyExistingPos = null, bool callQuantityExceedable = false)
        {
            if (!PreExecute("AssignDNoteOutOrderPos"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignDNoteOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50004")
                };
            }

            if (currentPicking == null || currentDNoteOutOrderPos2Level == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50011"));
            }
            if (currentDNoteOutOrderPos2Level.OutOrderPos1_ParentOutOrderPos == null) // Es können auch Positionen zugeordnet werden, die noch nicht einem Lieferschein zugeordnet wurden: || !currentDNoteOutOrderPos2Level.DeliveryNotePos_OutOrderPos.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignDNoteOutOrderPos(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50015")
                };
            }

            var queryDelivStateAssigned = s_cQry_CompletelyAssigned.Invoke(dbApp);
            if (!queryDelivStateAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignOutOrderPos(2)",
                    Message = Root.Environment.TranslateMessage(this, "Error50013")
                };
            }

            var queryDelivStateSubsetAssigned = s_cQry_SubsetAssigned.Invoke(dbApp);
            if (!queryDelivStateSubsetAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignOutOrderPos(3)",
                    Message = Root.Environment.TranslateMessage(this, "Error50014")
                };
            }

            if (enteredPartialQuantity.HasValue && (enteredPartialQuantity.Value - 0 <= Double.Epsilon))
                return null;

            // 1. Dritte Ebene einfügen für Kommissionierposition
            currentDNoteOutOrderPos2Level.AutoRefresh(dbApp);

            // 2. Teilmenge abrufen
            double partialQuantityUOM = 0;
            if (enteredPartialQuantity.HasValue)
            {
                partialQuantityUOM = currentDNoteOutOrderPos2Level.Material.ConvertToBaseQuantity(enteredPartialQuantity.Value, currentDNoteOutOrderPos2Level.MDUnit);
                if (!callQuantityExceedable && (currentDNoteOutOrderPos2Level.RemainingCallQuantityUOM + 0.000001) < partialQuantityUOM)
                //|| currentDNoteOutOrderPos2Level.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.CompletelyAssigned
                //|| currentDNoteOutOrderPos2Level.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.Delivered)
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "AssignOutOrderPos(4)",
                        Message = Root.Environment.TranslateMessage(this, "Warning50001", enteredPartialQuantity.Value, currentDNoteOutOrderPos2Level.MDUnit.MDUnitName, currentDNoteOutOrderPos2Level.RemainingCallQuantity, currentDNoteOutOrderPos2Level.MDUnit.MDUnitName)
                    };
                }
                currentDNoteOutOrderPos2Level.CalledUpQuantityUOM += partialQuantityUOM;
                if (currentDNoteOutOrderPos2Level.RemainingCallQuantityUOM <= Double.Epsilon)
                    currentDNoteOutOrderPos2Level.MDDelivPosState = queryDelivStateAssigned.First();
                else
                    currentDNoteOutOrderPos2Level.MDDelivPosState = queryDelivStateSubsetAssigned.First();
            }
            else
            {
                partialQuantityUOM = currentDNoteOutOrderPos2Level.RemainingCallQuantityUOM;
                currentDNoteOutOrderPos2Level.CalledUpQuantityUOM = currentDNoteOutOrderPos2Level.TargetQuantityUOM;
                currentDNoteOutOrderPos2Level.MDDelivPosState = queryDelivStateAssigned.First();
            }

            OutOrderPos outOrderPos3Level = OutOrderPos.NewACObject(dbApp, currentDNoteOutOrderPos2Level);
            var queryOutOrderPosState = s_cQry_OutOrderInProcess.Invoke(dbApp);
            if (!queryOutOrderPosState.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignOutOrderPos(3)",
                    Message = Root.Environment.TranslateMessage(this, "Error50014")
                };
            }
            outOrderPos3Level.MDOutOrderPosState = queryOutOrderPosState.FirstOrDefault();
            outOrderPos3Level.MaterialPosType = GlobalApp.MaterialPosTypes.OutwardPart;
            outOrderPos3Level.TargetQuantityUOM = partialQuantityUOM;

            resultNewEntities.Add(outOrderPos3Level);

            PickingPos pickingPos = alreadyExistingPos;
            if (alreadyExistingPos == null)
                pickingPos = PickingPos.NewACObject(dbApp, currentPicking);
            pickingPos.OutOrderPos = outOrderPos3Level;
            if (alreadyExistingPos == null)
                resultNewEntities.Add(pickingPos);

            PostExecute("AssignDNoteOutOrderPos");
            return null;
        }

        #endregion

        #region DeliveryNote

        public Msg CreateOrUpdatePickingFromInDeliveryNote(DeliveryNote deliveryNote, DatabaseApp dbApp, out Picking picking, bool dnNoInPickingComment2 = true)
        {
            picking = null;

            if (!deliveryNote.DeliveryNotePos_DeliveryNote.Any() || deliveryNote == null)
                return null;

            InOrderPos inOrderPos = deliveryNote.DeliveryNotePos_DeliveryNote.Select(c => c.InOrderPos).Where(c => c.InOrderPos_ParentInOrderPos.Any()).FirstOrDefault();
            picking = inOrderPos?.InOrderPos_ParentInOrderPos.FirstOrDefault()?.PickingPos_InOrderPos.FirstOrDefault()?.Picking;

            if (picking == null)
            {
                string secKey = Root.NoManager.GetNewNo(dbApp, typeof(Picking), Picking.NoColumnName, Picking.FormatNewNo);
                picking = Picking.NewACObject(dbApp, null, secKey);
                picking.MDPickingType = dbApp.MDPickingType.FirstOrDefault(c => c.MDKey == nameof(GlobalApp.PickingType.Receipt));
                picking.Comment2 = deliveryNote.DeliveryNoteNo;
            }

            List<PickingPos> existingPickingPosList = picking.PickingPos_Picking.ToList();

            MsgWithDetails msgDet = new MsgWithDetails();

            foreach(DeliveryNotePos dnPos in deliveryNote.DeliveryNotePos_DeliveryNote)
            {
                InOrderPos pickingInOrderPos = dnPos.InOrderPos.InOrderPos_ParentInOrderPos.FirstOrDefault();
                if (pickingInOrderPos != null)
                {
                    PickingPos pPos = existingPickingPosList.FirstOrDefault(c => c.InOrderPosID == pickingInOrderPos.InOrderPosID);
                    if (pPos != null)
                        existingPickingPosList.Remove(pPos);

                    if (pickingInOrderPos.TargetQuantityUOM != dnPos.InOrderPos.TargetQuantityUOM)
                        pickingInOrderPos.TargetQuantityUOM = dnPos.InOrderPos.TargetQuantityUOM;
                }
                else
                {
                    List<object> resultNewEntites = new List<object>();
                    Msg msgAssign = AssignDNoteInOrderPos(picking, dnPos.InOrderPos, null, dbApp, resultNewEntites);
                    if (msgAssign != null)
                        msgDet.AddDetailMessage(msgAssign);
                }
            }

            foreach (PickingPos pPos in existingPickingPosList)
            {
                Msg msgUnassign = UnassignPickingPos(pPos, dbApp);
                if (msgUnassign != null)
                    msgDet.AddDetailMessage(msgUnassign);
            }

            if (msgDet.MsgDetailsCount > 0)
                return msgDet;

            return null;
        }

        #endregion

        #region ProdOrderPartslist

        [ACMethodInfo("", "en{'Assign bill of materials line'}de{'Stücklistenposition zuordnen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg AssignProdOrderPartslistPos(Picking currentPicking, ProdOrderPartslistPos currentProdOrderPartslistPos1Level, Nullable<double> enteredPartialQuantity, DatabaseApp dbApp, List<object> resultNewEntities)
        {
            if (!PreExecute("AssignProdOrderPartslistPos"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignProdOrderPartslistPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50004")
                };
            }

            if (currentPicking == null || currentProdOrderPartslistPos1Level == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50011"));
            }

            // Falls nicht von erster Ebene, dann kann Position nicht zugewiesen werden
            if (currentProdOrderPartslistPos1Level.ProdOrderPartslistPos1_ParentProdOrderPartslistPos != null)
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignProdOrderPartslistOrderPos(1)",
                    Message = Root.Environment.TranslateMessage(this, "Info50012")
                };
            }

            if (enteredPartialQuantity.HasValue && (enteredPartialQuantity.Value - 0 <= Double.Epsilon))
                return null;

            // Zweite Ebene einfügen (für Lieferscheinzuordnung vorbereiten)
            //currentProdOrderPartslistPos1Level.AutoRefresh(dbApp);
            //ProdOrderPartslistPos prodOrderPartslistPos2Level = ProdOrderPartslistPos.NewACObject(dbApp, currentProdOrderPartslistPos1Level);
            //if (enteredPartialQuantity.HasValue)
            //    prodOrderPartslistPos2Level.TargetQuantity = enteredPartialQuantity.Value;
            //else
            //    prodOrderPartslistPos2Level.TargetQuantity = currentProdOrderPartslistPos1Level.TargetQuantity;
            //resultNewEntities.Add(prodOrderPartslistPos2Level);

            //// Dritte Ebene einfügen für Kommissionierposition
            //ProdOrderPartslistPos prodOrderPartslistPos3Level = ProdOrderPartslistPos.NewACObject(dbApp, prodOrderPartslistPos2Level);
            //if (enteredPartialQuantity.HasValue)
            //    prodOrderPartslistPos3Level.TargetQuantity = enteredPartialQuantity.Value;
            //else
            //    prodOrderPartslistPos3Level.TargetQuantity = currentProdOrderPartslistPos1Level.TargetQuantity;
            //resultNewEntities.Add(prodOrderPartslistPos3Level);

            PickingPos pickingPos = PickingPos.NewACObject(dbApp, currentPicking);
            pickingPos.PickingMaterial = currentProdOrderPartslistPos1Level.Material;
            PickingPosProdOrderPartslistPos pickingPosProdOrderPartslistPos = PickingPosProdOrderPartslistPos.NewACObject(dbApp, pickingPos, currentProdOrderPartslistPos1Level);
            pickingPos.PickingPosProdOrderPartslistPos_PickingPos.Add(pickingPosProdOrderPartslistPos);
            resultNewEntities.Add(pickingPos);


            PostExecute("AssignProdOrderPartslistPos");
            return null;
        }

        #endregion

        #region Picking-Pos
        [ACMethodInfo("", "en{'Remove pickings'}de{'Entfernen Kommissionierpositionen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg UnassignAllPickingPos(Picking currentPicking, DatabaseApp dbApp, bool deletePickingPos = true)
        {
            if (currentPicking == null)
                return null;
            currentPicking.PickingPos_Picking.AutoRefresh(dbApp);
            foreach (PickingPos pickingPos in currentPicking.PickingPos_Picking.ToArray())
            {
                Msg result = UnassignPickingPos(pickingPos, dbApp, deletePickingPos);
                if (result != null)
                    return result;
            }
            return null;
        }

        [ACMethodInfo("", "en{'Remove picking'}de{'Entfernen Kommissionierposition'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg UnassignPickingPos(PickingPos currentPickingPos, DatabaseApp dbApp, bool deletePickingPos = true)
        {
            if (!PreExecute("UnassignPickingPos"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignPickingPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50004")
                };
            }

            if (currentPickingPos == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50016"));
            }
            else if (deletePickingPos && currentPickingPos.Picking.VisitorVoucher != null)
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignPickingPos(1)",
                    Message = Root.Environment.TranslateMessage(this, "Info50005")
                };
            }
            else if (currentPickingPos.Picking.Tourplan != null)
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignPickingPos(2)",
                    Message = Root.Environment.TranslateMessage(this, "Info50006")
                };
            }

            var queryDelivStateSubsetAssigned = s_cQry_SubsetAssigned.Invoke(dbApp);
            if (!queryDelivStateSubsetAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignInOrderPos(3)",
                    Message = Root.Environment.TranslateMessage(this, "Error50014")
                };
            }


            if (currentPickingPos.InOrderPos != null && currentPickingPos.InOrderPos.InOrderPos1_ParentInOrderPos != null)
            {
                // Falls bereits Buchungen drauf liefen, dann kann Position nicht mehr abgewählt werden
                if (currentPickingPos.InOrderPos.FacilityBooking_InOrderPos.Any())
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "UnassignPickingPos(3)",
                        Message = Root.Environment.TranslateMessage(this, "Info50007")
                    };
                }

                // 1. Hole Parent-Bestellposition aus Bestellung und korrigiere Abgerufene teilmenge
                currentPickingPos.InOrderPos.AutoRefresh(dbApp);
                InOrderPos parentInOrderPos = currentPickingPos.InOrderPos.InOrderPos1_ParentInOrderPos;
                parentInOrderPos.AutoRefresh(dbApp);
                parentInOrderPos.InOrderPos_ParentInOrderPos.AutoRefresh(dbApp);
                int childs = parentInOrderPos.InOrderPos_ParentInOrderPos.Count;

                // Addiere Abrufmenge auf Elternpositionen hinzu
                parentInOrderPos.CalledUpQuantityUOM -= currentPickingPos.InOrderPos.TargetQuantityUOM;
                parentInOrderPos.MDDelivPosState = queryDelivStateSubsetAssigned.First();
                if (parentInOrderPos.InOrderPos1_ParentInOrderPos != null)
                {
                    parentInOrderPos.InOrderPos1_ParentInOrderPos.AutoRefresh(dbApp);
                    parentInOrderPos.InOrderPos1_ParentInOrderPos.CalledUpQuantityUOM -= currentPickingPos.InOrderPos.TargetQuantityUOM;
                    parentInOrderPos.InOrderPos1_ParentInOrderPos.MDDelivPosState = queryDelivStateSubsetAssigned.First();
                }

                // 2. Lösche 3 Ebene
                InOrderPos inOrderPos3 = currentPickingPos.InOrderPos;
                currentPickingPos.InOrderPos = null;
                inOrderPos3.DeleteACObject(dbApp, true);
                childs--;

                // 3. Lösche zweite Ebene, wenn nicht einem Lieferschein zugeordnet
                parentInOrderPos.DeliveryNotePos_InOrderPos.AutoRefresh(dbApp);
                if (!parentInOrderPos.DeliveryNotePos_InOrderPos.Any() && childs == 0)
                {
                    if (parentInOrderPos.InOrderPos1_ParentInOrderPos != null)
                    {
                        parentInOrderPos.InOrderPos1_ParentInOrderPos = null;
                        parentInOrderPos.DeleteACObject(dbApp, true);
                    }
                }

                // 4. Lösche Kommissionierposition
                if (deletePickingPos)
                    currentPickingPos.DeleteACObject(dbApp, true);
            }
            else if (currentPickingPos.OutOrderPos != null && currentPickingPos.OutOrderPos.OutOrderPos1_ParentOutOrderPos != null)
            {
                // Falls bereits Buchungen drauf liefen, dann kann Position nicht mehr abgewählt werden
                if (currentPickingPos.OutOrderPos.FacilityBooking_OutOrderPos.Any())
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "UnassignPickingPos(4)",
                        Message = Root.Environment.TranslateMessage(this, "Info50007")
                    };
                }

                // 1. Hole Parent-Bestellposition aus Bestellung und korrigiere Abgerufene teilmenge
                currentPickingPos.OutOrderPos.AutoRefresh(dbApp);
                OutOrderPos parentOutOrderPos = currentPickingPos.OutOrderPos.OutOrderPos1_ParentOutOrderPos;
                parentOutOrderPos.AutoRefresh(dbApp);
                parentOutOrderPos.OutOrderPos_ParentOutOrderPos.AutoRefresh(dbApp);
                int childs = parentOutOrderPos.OutOrderPos_ParentOutOrderPos.Count;

                // Addiere Abrufmenge auf Elternpositionen hinzu
                parentOutOrderPos.CalledUpQuantityUOM -= currentPickingPos.OutOrderPos.TargetQuantityUOM;
                parentOutOrderPos.MDDelivPosState = queryDelivStateSubsetAssigned.First();
                if (parentOutOrderPos.OutOrderPos1_ParentOutOrderPos != null)
                {
                    parentOutOrderPos.OutOrderPos1_ParentOutOrderPos.AutoRefresh(dbApp);
                    parentOutOrderPos.OutOrderPos1_ParentOutOrderPos.CalledUpQuantityUOM -= currentPickingPos.OutOrderPos.TargetQuantityUOM;
                    parentOutOrderPos.OutOrderPos1_ParentOutOrderPos.MDDelivPosState = queryDelivStateSubsetAssigned.First();
                }

                // 2. Lösche 3 Ebene
                OutOrderPos outOrderPos3 = currentPickingPos.OutOrderPos;
                currentPickingPos.OutOrderPos = null;
                outOrderPos3.DeleteACObject(dbApp, true);
                //parentOutOrderPos.OutOrderPos_ParentOutOrderPos.Remove(currentPickingPos.OutOrderPos);
                childs--;

                // 3. Lösche zweite Ebene, wenn nicht einem Lieferschein zugeordnet
                parentOutOrderPos.DeliveryNotePos_OutOrderPos.AutoRefresh(dbApp);
                if (!parentOutOrderPos.DeliveryNotePos_OutOrderPos.Any() && childs == 0)
                {
                    if (parentOutOrderPos.OutOrderPos1_ParentOutOrderPos != null)
                    {
                        parentOutOrderPos.OutOrderPos1_ParentOutOrderPos = null;
                        parentOutOrderPos.DeleteACObject(dbApp, true);
                    }
                }

                // 4. Lösche Kommissionierposition
                if (deletePickingPos)
                    currentPickingPos.DeleteACObject(dbApp, true);
            }
            else if (currentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any())
            {
                // Falls bereits Buchungen drauf liefen, dann kann Position nicht mehr abgewählt werden
                if (currentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.SelectMany(c => c.ProdorderPartslistPos.FacilityBooking_ProdOrderPartslistPos).Any())
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "UnassignPickingPos(5)",
                        Message = Root.Environment.TranslateMessage(this, "Info50007")
                    };
                }

                // 1. Hole Parent-Bestellposition aus Bestellung und korrigiere Abgerufene teilmenge
                currentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.AutoRefresh(dbApp);
                //List<ProdOrderPartslistPos> parentProdOrderPartslistPositions = currentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Select(c => c.ProdorderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos).ToList();
                //foreach (var parentProdOrderPartslistPos in parentProdOrderPartslistPositions)
                //    parentProdOrderPartslistPos.AutoRefresh(dbApp);
                //parentProdOrderPartslistPos.CalledUpQuantityUOM -= currentPickingPos.ProdOrderPartslistPos.TargetQuantityUOM;

                // 2. Lösche Unter-Position
                List<PickingPosProdOrderPartslistPos> pickingPosPositions = currentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.ToList();
                //List<ProdOrderPartslistPos> posPositions = pickingPosPositions.Select(c => c.ProdorderPartslistPos).ToList();

                //foreach (var pickingPosPosition in pickingPosPositions)
                //    pickingPosPosition.DeleteACObject(dbApp, false);
                foreach (PickingPosProdOrderPartslistPos pos in pickingPosPositions)
                {
                    pos.DeleteACObject(dbApp, true);
                    //currentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Remove(pos);
                }

                // 3. Lösche Kommissionierposition
                if (deletePickingPos)
                    currentPickingPos.DeleteACObject(dbApp, true);
            }
            else if (deletePickingPos)
            {
                // Falls bereits Buchungen drauf liefen, dann kann Position nicht mehr abgewählt werden
                if (currentPickingPos.FacilityBooking_PickingPos.Any())
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "UnassignPickingPos(3)",
                        Message = Root.Environment.TranslateMessage(this, "Info50007")
                    };
                }
                currentPickingPos.DeleteACObject(dbApp, true);
            }

            PostExecute("UnassignPickingPos");
            return null;
        }
        #endregion

        #region Booking

        public MsgWithDetails CreateNewPicking(ACMethodBooking relocationBooking, gip.core.datamodel.ACClassMethod aCClassMethod, DatabaseApp dbApp, Database dbIPlus, bool setReadyToLoad, out Picking picking)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            Msg msg;
            if (relocationBooking.OutwardFacilityCharge == null && relocationBooking.OutwardFacility == null)
            {
                //Error50121: Cannot create picking because passed quant or source is null
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CreateNewPicking(10)",
                    Message = Root.Environment.TranslateMessage(this, "Error50121")
                };
                msgWithDetails.AddDetailMessage(msg);
            }
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Picking), Picking.NoColumnName, Picking.FormatNewNo, this);
            picking = Picking.NewACObject(dbApp, null, secondaryKey);
            picking.ACClassMethodID = aCClassMethod.ACClassMethodID;
            dbApp.Picking.AddObject(picking);
            PickingPos pickingPos = PickingPos.NewACObject(dbApp, picking);
            picking.PickingPos_Picking.Add(pickingPos);
            if (relocationBooking.OutwardFacilityCharge != null)
            {
                pickingPos.PickingMaterial = relocationBooking.OutwardFacilityCharge.Material;
                pickingPos.FromFacility = relocationBooking.OutwardFacilityCharge.Facility;
                pickingPos.ToFacility = relocationBooking.InwardFacility != null ? relocationBooking.InwardFacility : relocationBooking.InwardFacilityCharge.Facility;
            }
            else if (relocationBooking.OutwardFacility != null)
            {
                pickingPos.PickingMaterial = relocationBooking.OutwardMaterial != null ? relocationBooking.OutwardMaterial : relocationBooking.OutwardFacility.Material;
                pickingPos.FromFacility = relocationBooking.OutwardFacility;
                pickingPos.ToFacility = relocationBooking.InwardFacility;
            }
            else if (relocationBooking.OutwardMaterial != null && relocationBooking.InwardFacility != null)
            {
                pickingPos.PickingMaterial = relocationBooking.OutwardMaterial;
                pickingPos.ToFacility = relocationBooking.InwardFacility;
            }
            pickingPos.PickingQuantityUOM = relocationBooking.OutwardQuantity;
            if (setReadyToLoad)
                pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad).FirstOrDefault();
            else
                pickingPos.MDDelivPosLoadState = MDDelivPosLoadState.DefaultMDDelivPosLoadState(dbApp);

            if (pickingPos.Material.IsLotReservationNeeded)
            {
                List<FacilityCharge> lotsForReservation = new List<FacilityCharge>();
                if (relocationBooking.OutwardFacilityCharge != null && relocationBooking.OutwardFacilityCharge.FacilityLot != null)
                    lotsForReservation.Add(relocationBooking.OutwardFacilityCharge);
                else if (relocationBooking.OutwardFacility != null)
                    lotsForReservation = dbApp.FacilityCharge.Include(c => c.FacilityLot).Where(c => c.FacilityID == relocationBooking.OutwardFacility.FacilityID && !c.NotAvailable).ToList();
                if (lotsForReservation.Any())
                {
                    foreach (var flGroup in lotsForReservation.GroupBy(c => c.FacilityLot))
                    {
                        double reservationQ = flGroup.Sum(c => c.StockQuantityUOM);
                        secondaryKey = Root.NoManager.GetNewNo(dbApp, typeof(FacilityReservation), FacilityReservation.NoColumnName, FacilityReservation.FormatNewNo, null);
                        FacilityReservation reservation = FacilityReservation.NewACObject(dbApp, pickingPos, secondaryKey);
                        reservation.ReservedQuantityUOM = reservationQ;
                        reservation.FacilityLot = flGroup.Key;
                        reservation.PickingPos = pickingPos;
                        dbApp.FacilityReservation.AddObject(reservation);
                    }
                }
            }

            msgWithDetails = dbApp.ACSaveChanges();
            return msgWithDetails;
        }

        public Msg GetActivatedFacilityChargeForBooking(DatabaseApp dbApp, PickingPos pPos, MaterialConfig[] materialConfigs, out FacilityCharge fc)
        {
            fc = null;

            MaterialConfig mc = materialConfigs.FirstOrDefault(c => c.MaterialID == pPos.Material.MaterialID);

            string materialName = pPos.Material.MaterialName1;

            if (mc == null)
            {
                //Error50548: The quant for {0} is not acitvated.
                return new Msg(this, eMsgLevel.Error, nameof(ACPickingManager), nameof(GetActivatedFacilityChargeForBooking) + "(10)", 1009, "Error50548", materialName);
            }

            Guid? fcID = mc.Value as Guid?;
            if (!fcID.HasValue)
            {
                //Error50549: The quant for {0} is not valid.
                return new Msg(this, eMsgLevel.Error, nameof(ACPickingManager), nameof(GetActivatedFacilityChargeForBooking) + "(20)", 1018, "Error50549", materialName);
            }

            fc = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == fcID.Value);
            if (fc == null || fc.NotAvailable)
            {
                //Error50550: The activated quant for {0} is not available or missing!
                return new Msg(this, eMsgLevel.Error, nameof(ACPickingManager), nameof(GetActivatedFacilityChargeForBooking) + "(30)", 1024, "Error50550", materialName);
            }

            return null;
        }

        public Msg ValidateQuantityForBooking(double reqQuantity, double availableQuantity, string materialName)
        {
            if (reqQuantity > availableQuantity)
            {
                //Error50551: The required quantity is lager than available quantity for {0}.
                return new Msg(this, eMsgLevel.Error, nameof(ACPickingManager), nameof(ValidateQuantityForBooking) + "(10)", 1038, "Error50551", materialName);
            }
            return null;
        }

        #endregion

        #region Validation
        public virtual MsgWithDetails ValidateStart(DatabaseApp dbApp, Database dbiPlus,
            Picking picking, List<IACConfigStore> configStores,
            PARole.ValidationBehaviour validationBehaviour)
        {
            Msg msg = null;
            MsgWithDetails detailMessages = new MsgWithDetails();
            if (!picking.PickingPos_Picking.Any())
                return detailMessages;

            if (picking.ACClassMethod == null && validationBehaviour == PARole.ValidationBehaviour.Strict)
            {
                //Error50112: No Workflow assigned.The picking order can't be started. 
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "ValidateStart(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50112")
                };
                detailMessages.AddDetailMessage(msg);

                return detailMessages;
            }


            // Some global Checks
            //foreach (PickingPos pos in picking.PickingPos_Picking.Where(c => !c.MDDelivPosLoadStateID.HasValue || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)
            //            .OrderBy(c => c.Sequence))
            //{
            //}

            CheckResourcesAndRouting(dbApp, dbiPlus, picking, configStores, validationBehaviour, detailMessages);

            return detailMessages;
        }

        public virtual MsgWithDetails ValidateRoutes(DatabaseApp dbApp, Database dbiPlus, Picking picking, List<IACConfigStore> configStores, PARole.ValidationBehaviour validationBehaviour)
        {
            MsgWithDetails detailMessages = new MsgWithDetails();

            // Some global Checks
            //foreach (PickingPos pos in picking.PickingPos_Picking.Where(c => !c.MDDelivPosLoadStateID.HasValue || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)
            //            .OrderBy(c => c.Sequence))
            //{
            //}

            CheckResourcesAndRouting(dbApp, dbiPlus, picking, configStores, validationBehaviour, detailMessages);

            return detailMessages;
        }

        #region Virtual and protected
        public virtual void CheckResourcesAndRouting(DatabaseApp dbApp, Database dbiPlus, Picking picking, List<IACConfigStore> configStores,
                                        PARole.ValidationBehaviour validationBehaviour, MsgWithDetails detailMessages)
        {
            //if (configStores == null)
            //return;
            Msg msg = null;

            Type siloType = null;
            gip.core.datamodel.ACClass acClassSilo = ACClassManager.s_cQry_ACClassIdentifier(dbiPlus, FacilityManager.SiloClass);
            if (acClassSilo != null)
                siloType = acClassSilo.ObjectType;
            if (acClassSilo == null || siloType == null)
            {
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CheckResourcesAndRouting(10)",
                    Message = String.Format("Type for {0} not found in Database or .NET-Type not loadable", FacilityManager.SiloClass)
                };
                detailMessages.AddDetailMessage(msg);
                return;
            }

            foreach (PickingPos pos in picking.PickingPos_Picking.Where(c => !c.MDDelivPosLoadStateID.HasValue || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)
                        .OrderBy(c => c.Sequence))
            {
                if (pos.Material != null && pos.Material.IsLotReservationNeeded)
                {
                    if (!pos.FacilityReservation_PickingPos.Where(c => !c.VBiACClassID.HasValue).Any())
                    {
                        // Error50634: The material {0} {1} at position {2} requires reservation and no batch has been reserved.
                        msg = new Msg
                        {
                            Source = GetACUrl(),
                            MessageLevel = eMsgLevel.Error,
                            ACIdentifier = "CheckResourcesAndRouting(20)",
                            Message = Root.Environment.TranslateMessage(this, "Error50634", pos.Material.MaterialNo, pos.Material.MaterialName1, pos.Sequence)
                        };
                        detailMessages.AddDetailMessage(msg);
                    }
                }
                if (pos.FromFacility != null)
                {
                    CheckResourcesAndRoutingKnownSource(dbApp, dbiPlus, picking, configStores, validationBehaviour, detailMessages, pos, siloType);
                }
                else
                {
                    CheckResourcesAndRoutingUnknownSource(dbApp, dbiPlus, picking, configStores, validationBehaviour, detailMessages, pos, siloType);
                }
            }
        }

        private void CheckResourcesAndRoutingKnownSource(DatabaseApp dbApp, Database dbiPlus, Picking picking, List<IACConfigStore> configStores,
                                                         PARole.ValidationBehaviour validationBehaviour, MsgWithDetails detailMessages, PickingPos pos, Type siloType, string selectionRuleID = "PAMSilo.Deselector")
        {
            Msg msg;

            if (pos.FromFacility == null || pos.ToFacility == null || pos.FromFacility.MDFacilityType == null || pos.ToFacility.MDFacilityType == null)
            {
                //Error50113: No source, no target or no facilitytype defined in Line {0}.
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CheckResourcesAndRouting(20)",
                    Message = Root.Environment.TranslateMessage(this, "Error50113", pos.Sequence)
                };
                detailMessages.AddDetailMessage(msg);
                return;
            }
            if (!pos.FromFacility.VBiFacilityACClassID.HasValue || !pos.ToFacility.VBiFacilityACClassID.HasValue)
            {
                //Error50114: Source or target is not referenced to a Processmodule-instance in Line {0}.
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CheckResourcesAndRouting(30)",
                    Message = Root.Environment.TranslateMessage(this, "Error50114", pos.Sequence)
                };
                detailMessages.AddDetailMessage(msg);
                return;
            }

            if (validationBehaviour == PARole.ValidationBehaviour.Strict
                && pos.FromFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer
                && pos.ToFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
            {
                if (!pos.FromFacility.MaterialID.HasValue)
                {
                    //Error50115: No Material assigned to Bin/Silo/Container {0}.
                    msg = new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "CheckResourcesAndRouting(40)",
                        Message = Root.Environment.TranslateMessage(this, "Error50115", pos.FromFacility.FacilityNo)
                    };
                    detailMessages.AddDetailMessage(msg);
                    return;
                }
                if (pos.ToFacility.MaterialID.HasValue && pos.FromFacility.MaterialID.HasValue && !Material.IsMaterialEqual(pos.ToFacility.Material, pos.FromFacility.Material))
                {
                    //Error50116: Material {0} in source {1} is not equal to Material {2} in target {3}.
                    msg = new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "CheckResourcesAndRouting(50)",
                        Message = Root.Environment.TranslateMessage(this, "Error50116",
                                pos.FromFacility.Material.MaterialNo, pos.FromFacility.FacilityNo,
                                pos.ToFacility.Material.MaterialNo, pos.ToFacility.FacilityNo)
                    };
                    detailMessages.AddDetailMessage(msg);
                    return;
                }
                if (pos.ToFacility.PartslistID.HasValue && pos.FromFacility.PartslistID.HasValue && pos.ToFacility.PartslistID.Value != pos.FromFacility.PartslistID.Value)
                {
                    //Error50117: Bill of material {0} in source {1} is not equal to Bill of material {2} in target {3}.
                    msg = new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "CheckResourcesAndRouting(60)",
                        Message = Root.Environment.TranslateMessage(this, "Error50117",
                                pos.FromFacility.Partslist.PartslistNo, pos.FromFacility.FacilityNo,
                                pos.ToFacility.Partslist.PartslistNo, pos.ToFacility.FacilityNo)
                    };
                    detailMessages.AddDetailMessage(msg);
                    return;
                }
                if (!pos.ToFacility.InwardEnabled)
                {
                    //Error50118: Inward to Bin/Silo/Container {0} is not enabled.
                    msg = new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "CheckResourcesAndRouting(70)",
                        Message = Root.Environment.TranslateMessage(this, "Error50118", pos.ToFacility.FacilityNo)
                    };
                    detailMessages.AddDetailMessage(msg);
                    return;
                }
                if (!pos.FromFacility.OutwardEnabled)
                {
                    //Error50119: Outward from Bin/Silo/Container {0} is not enabled.
                    msg = new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "CheckResourcesAndRouting(80)",
                        Message = Root.Environment.TranslateMessage(this, "Error50119", pos.FromFacility.FacilityNo)
                    };
                    detailMessages.AddDetailMessage(msg);
                    return;
                }
            }
            else if (validationBehaviour == PARole.ValidationBehaviour.Strict
                && pos.Material != null
                && pos.ToFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
            {
                if (pos.ToFacility.MaterialID.HasValue && !Material.IsMaterialEqual(pos.ToFacility.Material, pos.Material))
                {
                    //Error50116: Material {0} in source {1} is not equal to Material {2} in target {3}.
                    msg = new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "CheckResourcesAndRouting(50)",
                        Message = Root.Environment.TranslateMessage(this, "Error50116",
                                pos.Material.MaterialNo, pos.Material.MaterialName1,
                                pos.ToFacility.Material.MaterialNo, pos.ToFacility.FacilityNo)
                    };
                    detailMessages.AddDetailMessage(msg);
                    return;
                }
                if (!pos.ToFacility.InwardEnabled)
                {
                    //Error50118: Inward to Bin/Silo/Container {0} is not enabled.
                    msg = new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "CheckResourcesAndRouting(70)",
                        Message = Root.Environment.TranslateMessage(this, "Error50118", pos.ToFacility.FacilityNo)
                    };
                    detailMessages.AddDetailMessage(msg);
                    return;
                }
            }
            var fromClass = pos.FromFacility.GetFacilityACClass(dbiPlus);
            var toClass = pos.ToFacility.GetFacilityACClass(dbiPlus);
            if (fromClass == toClass)
                return;

            ACRoutingParameters routingParameters = new ACRoutingParameters()
            {
                RoutingService = this.RoutingService,
                Database = this.Database.ContextIPlus,
                AttachRouteItemsToContext = false,
                Direction = RouteDirections.Forwards,
                SelectionRuleID = selectionRuleID,
                DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && pos.ToFacility.VBiFacilityACClassID == c.ACClassID,
                DBDeSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && (pos.FromFacility.VBiFacilityACClassID == c.ACClassID || siloType.IsAssignableFrom(c.ObjectType)),
                MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                IncludeReserved = true,
                IncludeAllocated = true,
                DBRecursionLimit = 10
            };

            RoutingResult result = ACRoutingService.SelectRoutes(fromClass, toClass, routingParameters);
            if (result.Routes == null || !result.Routes.Any())
            {
                //Error50120: No route found for transport from {0} to {1}
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CheckResourcesAndRouting(90)",
                    Message = Root.Environment.TranslateMessage(this, "Error50120", pos.FromFacility.FacilityNo, pos.ToFacility.FacilityNo)
                };
                detailMessages.AddDetailMessage(msg);
                return;
            }
        }

        private void CheckResourcesAndRoutingUnknownSource(DatabaseApp dbApp, Database dbiPlus, Picking picking, List<IACConfigStore> configStores,
                                                         PARole.ValidationBehaviour validationBehaviour, MsgWithDetails detailMessages, PickingPos pos, Type siloType, string selectionRuleID = "PAMSilo.Deselector")
        {
            Msg msg;

            if (pos.ToFacility == null || pos.ToFacility.MDFacilityType == null)
            {
                //Error50113: No source, no target or no facilitytype defined in Line {0}.
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CheckResourcesAndRouting(20)",
                    Message = Root.Environment.TranslateMessage(this, "Error50113", pos.Sequence)
                };
                detailMessages.AddDetailMessage(msg);
                return;
            }

            if (!pos.ToFacility.VBiFacilityACClassID.HasValue)
            {
                //Error50114: Source or target is not referenced to a Processmodule-instance in Line {0}.
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CheckResourcesAndRouting(30)",
                    Message = Root.Environment.TranslateMessage(this, "Error50114", pos.Sequence)
                };
                detailMessages.AddDetailMessage(msg);
                return;
            }

            if (pos.ToFacility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBinContainer && !pos.PickingMaterialID.HasValue)
            {
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CheckResourcesAndRouting(30)",
                    Message = "No material assigned to picking position."
                };
                detailMessages.AddDetailMessage(msg);
                return;
            }

            if (validationBehaviour == PARole.ValidationBehaviour.Strict
                    && pos.ToFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
            {
                if (!pos.ToFacility.MaterialID.HasValue && pos.PickingMaterialID.HasValue)
                {
                    //Error50115: No Material assigned to Bin/Silo/Container {0}.
                    msg = new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "CheckResourcesAndRouting(40)",
                        Message = Root.Environment.TranslateMessage(this, "Error50115", pos.ToFacility.FacilityNo)
                    };
                    detailMessages.AddDetailMessage(msg);
                    return;
                }

                if (!pos.ToFacility.InwardEnabled)
                {
                    //Error50118: Inward to Bin/Silo/Container {0} is not enabled.
                    msg = new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "CheckResourcesAndRouting(70)",
                        Message = Root.Environment.TranslateMessage(this, "Error50118", pos.ToFacility.FacilityNo)
                    };
                    detailMessages.AddDetailMessage(msg);
                    return;
                }
            }


            QrySilosResult possibleSilos = null;
            core.datamodel.ACClass compClass = pos.ToFacility.FacilityACClass;

            IEnumerable<Route> routes = GetRoutes(pos, dbApp, dbiPlus, compClass, ACPartslistManager.SearchMode.SilosWithOutwardEnabled, null, out possibleSilos, null, null, null, true, 0, selectionRuleID);

            if (routes == null || !routes.Any())
            {
                //Error50120: No route found for transport from {0} to {1}
                msg = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CheckResourcesAndRouting(90)",
                    Message = Root.Environment.TranslateMessage(this, "Error50120", "", pos.ToFacility.FacilityNo)
                };
                detailMessages.AddDetailMessage(msg);
                return;
            }
        }


        //protected gip.mes.datamodel.ACClass[] ApplyRulesOnProjects(Database dbiPlus, gip.mes.datamodel.ACClass[] possibleProjects, MapPosToWFConn mapPosWF, List<IACConfigStore> configStores)
        //{
        //    if (possibleProjects == null || !possibleProjects.Any())
        //        return possibleProjects;
        //    RuleValueList ruleValueList = null;
        //    gip.core.datamodel.ACClassWF acClassWF = mapPosWF.MatWFConn.ACClassWF.FromIPlusContext<gip.core.datamodel.ACClassWF>(dbiPlus);
        //    ConfigManagerIPlus serviceInstance = ConfigManagerIPlus.GetServiceInstance(this);
        //    ruleValueList = serviceInstance.GetRuleValueList(configStores, "", acClassWF.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Allowed_instances.ToString());
        //    if (ruleValueList != null)
        //    {
        //        var selectedClasses = ruleValueList.GetSelectedClasses(ACClassWFRuleTypes.Allowed_instances, dbiPlus);
        //        if (selectedClasses != null && selectedClasses.Any())
        //        {
        //            var allowedComponents = selectedClasses.Select(c => c.ACClassID);
        //            var filteredList = possibleProjects.Where(c => allowedComponents.Contains(c.ACClassID)).ToArray();
        //            return filteredList;
        //        }
        //    }
        //    return possibleProjects;
        //}

        //protected void ApplyRulesOnPossibleInstances(Database dbiPlus, MapPosToWFConn invoker, MapPWGroup2Modules mapPWGroup2Module, List<IACConfigStore> configStores)
        //{
        //    if (mapPWGroup2Module == null || !mapPWGroup2Module.ProcessModuleList.Any())
        //        return;
        //    RuleValueList ruleValueList = null;
        //    ConfigManagerIPlus serviceInstance = ConfigManagerIPlus.GetServiceInstance(this);
        //    if (invoker.PWNode == null)
        //        invoker.PWNode = invoker.MatWFConn.ACClassWF.FromIPlusContext<gip.core.datamodel.ACClassWF>(dbiPlus);

        //    ruleValueList = serviceInstance.GetRuleValueList(configStores,
        //        String.IsNullOrEmpty(invoker.PWNode.ConfigACUrl) ? invoker.PWNode.ConfigACUrl : invoker.PWNode.ConfigACUrl + "\\",
        //        mapPWGroup2Module.PWGroup.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Allowed_instances.ToString());
        //    if (ruleValueList != null)
        //    {
        //        var selectedClasses = ruleValueList.GetSelectedClasses(ACClassWFRuleTypes.Allowed_instances, dbiPlus);
        //        if (selectedClasses != null && selectedClasses.Any())
        //        {
        //            var allowedComponents = selectedClasses.Select(c => c.ACClassID);
        //            mapPWGroup2Module.ProcessModuleList = mapPWGroup2Module.ProcessModuleList.Where(c => allowedComponents.Contains(c.ACClassID)).ToList();
        //        }
        //    }
        //}
        #endregion

        #endregion

        #region FinishOrder

        public MsgWithDetails FinishOrder(
            DatabaseApp dbApp, Picking picking,
            ACInDeliveryNoteManager inDeliveryNoteManager, ACOutDeliveryNoteManager outDeliveryNoteManager, FacilityManager facilityManager,
            out DeliveryNote deliveryNote,
            out InOrder inOrder,
            out OutOrder outOrder,
        bool skipCheck = false)
        {
            deliveryNote = null;
            inOrder = null;
            outOrder = null;
            if (dbApp == null)
            {
                return new MsgWithDetails(new Msg[] { new Msg(eMsgLevel.Error, "dbApp is null.") });
            }

            if (picking == null)
            {
                return new MsgWithDetails(new Msg[] { new Msg(eMsgLevel.Error, "picking is null.") });
            }

            MsgWithDetails result = null;

            MDDelivPosLoadState posState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
            MDInOrderPosState inPosState = dbApp.MDInOrderPosState.FirstOrDefault(c => c.MDInOrderPosStateIndex == (short)MDInOrderPosState.InOrderPosStates.Completed);
            MDOutOrderPosState outPosState = dbApp.MDOutOrderPosState.FirstOrDefault(c => c.MDOutOrderPosStateIndex == (short)MDOutOrderPosState.OutOrderPosStates.Completed);

            foreach (PickingPos pPos in picking.PickingPos_Picking)
            {
                bool missingQuantity = pPos.DiffQuantityUOM < 0;

                if (pPos.InOrderPos != null)
                {
                    bool isCompleted = pPos.InOrderPos.MDInOrderPosState != null
                                    && pPos.InOrderPos.MDInOrderPosState.InOrderPosState == MDInOrderPosState.InOrderPosStates.Completed;

                    if (!skipCheck && missingQuantity && !isCompleted)
                    {
                        if (result == null)
                        {
                            //Question50077: The following lines are unprocessed or actual quantity is insufficient. Do you want still finish order?
                            result = new MsgWithDetails(this, eMsgLevel.Question, "ACPickingManager", "FinishOrder", 2018, "Question50077");
                        }

                        //Warning50043: Line {0}, material {1} {2} insufficient quantity is {3} {4}.
                        Msg warrMsgMatIssQuant = new Msg(this, eMsgLevel.Warning, this.ACIdentifier, "FinishOrder", 2087, "Warning50043",
                            pPos.Sequence, pPos.Material.MaterialNo, pPos.Material.MaterialName1, Math.Abs(pPos.DiffQuantityUOM), pPos.MDUnit.Symbol);
                        result.AddDetailMessage(warrMsgMatIssQuant);
                        continue;
                    }

                    if (!isCompleted)
                    {
                        pPos.InOrderPos.MDInOrderPosState = inPosState;
                    }
                }
                else if (pPos.OutOrderPos != null)
                {
                    bool isCompleted = pPos.OutOrderPos.MDOutOrderPosState != null
                                    && pPos.OutOrderPos.MDOutOrderPosState.OutOrderPosState == MDOutOrderPosState.OutOrderPosStates.Completed;

                    if (!skipCheck && missingQuantity && !isCompleted)
                    {
                        if (result == null)
                        {
                            //Question50077: The following lines are unprocessed or actual quantity is insufficient. Do you want still finish order?
                            result = new MsgWithDetails(this, eMsgLevel.Question, "ACPickingManager", "FinishOrder(10)", 2018, "Question50077");
                        }

                        //Warning50043: Line {0}, material {1} {2} insufficient quantity is {3} {4}.
                        Msg warrMsgMatIssQuant = new Msg(this, eMsgLevel.Warning, this.ACIdentifier, "FinishOrder", 2113, "Warning50043",
                            pPos.Sequence, pPos.Material.MaterialNo, pPos.Material.MaterialName1, Math.Abs(pPos.DiffQuantityUOM), pPos.MDUnit.Symbol);
                        result.AddDetailMessage(warrMsgMatIssQuant);
                        continue;
                    }

                    if (!isCompleted)
                    {
                        pPos.OutOrderPos.MDOutOrderPosState = outPosState;
                    }
                }
                else
                {
                    bool isCompleted = pPos.MDDelivPosLoadState != null && pPos.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck;

                    if (!skipCheck && missingQuantity && !isCompleted)
                    {
                        if (result == null)
                        {
                            //Question50077: The following lines are unprocessed or actual quantity is insufficient. Do you want still finish order?
                            result = new MsgWithDetails(this, eMsgLevel.Question, "ACPickingManager", "FinishOrder(10)", 2018, "Question50077");
                        }

                        //Warning50043: Line {0}, material {1} {2} insufficient quantity is {3} {4}.
                        Msg warrMsgMatIssQuant = new Msg(this, eMsgLevel.Warning, this.ACIdentifier, "FinishOrder", 2113, "Warning50043",
                            pPos.Sequence, pPos.Material.MaterialNo, pPos.Material.MaterialName1, Math.Abs(pPos.DiffQuantityUOM), pPos.MDUnit.Symbol);
                        result.AddDetailMessage(warrMsgMatIssQuant);
                        continue;
                    }

                    if (!isCompleted)
                    {
                        pPos.MDDelivPosLoadState = posState;
                    }

                    if (picking.MDPickingType != null && picking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.InternalRelocation)
                    {
                        IEnumerable<FacilityCharge> facilityCharges = pPos.FacilityBooking_PickingPos.SelectMany(f => f.FacilityBookingCharge_FacilityBooking)
                                                                          .Where(x => x.InwardFacilityCharge != null 
                                                                                   && x.InwardFacility != null
                                                                                   && x.InwardFacility.PostingBehaviourIndex == (short)PostingBehaviourEnum.BlockOnRelocation)
                                                                          .Select(c => c.InwardFacilityCharge)
                                                                          .Where(fc => fc.MDReleaseState != null && 
                                                                                       fc.MDReleaseState.MDReleaseStateIndex != (short)MDReleaseState.ReleaseStates.Free);

                        foreach (FacilityCharge fc in facilityCharges)
                        {
                            fc.MDReleaseState = MDReleaseState.DefaultMDReleaseState(dbApp, MDReleaseState.ReleaseStates.Free);
                        }
                    }

                    if (pPos.FacilityPreBooking_PickingPos.Any())
                    {
                        FacilityPreBooking[] preBookings = pPos.FacilityPreBooking_PickingPos.ToArray();
                        foreach (FacilityPreBooking facilityPreBooking in preBookings)
                        {
                            pPos.FacilityPreBooking_PickingPos.Remove(facilityPreBooking);
                            facilityPreBooking.DeleteACObject(dbApp, false);
                        }
                    }
                }
            }

            Msg msg = dbApp.ACSaveChanges();
            if (msg != null)
                result.AddDetailMessage(msg);

            if (result != null && result.MsgDetailsCount > 0 && !skipCheck)
            {
                return result;
            }

            picking.PickingStateIndex = (short)PickingStateEnum.Finished;

            // Close related documents
            if (HasRelatedDocuments(picking, out deliveryNote, out inOrder, out outOrder))
            {
                if (inOrder != null)
                {
                    inDeliveryNoteManager.CompleteInDeliveryNote(dbApp, deliveryNote, inOrder);
                }
                else if (outOrder != null)
                {
                    outDeliveryNoteManager.CompleteOutDeliveryNote(dbApp, deliveryNote, outOrder);
                }
            }

            msg = dbApp.ACSaveChanges();
            if (msg != null)
            {
                result.AddDetailMessage(msg);
                return result;
            }
            else
                OnOrderFinished(dbApp, picking, facilityManager);

            return null;
        }

        public virtual void OnOrderFinished(DatabaseApp dbApp, Picking picking, FacilityManager facilityManager)
        {
        }

        public bool HasRelatedDocuments(Picking picking, out DeliveryNote deliveryNote, out InOrder inOrder, out OutOrder outOrder)
        {
            bool isRelated = false;
            deliveryNote = null;
            inOrder = null;
            outOrder = null;

            // Search InOrder
            IEnumerable<InOrder> queryInOrder = picking.PickingPos_Picking.Where(c => c.InOrderPos != null).Select(c => c.InOrderPos).Select(c => c.InOrder).Distinct();
            if (queryInOrder.Any() && queryInOrder.Count() == 1)
            {
                inOrder = queryInOrder.FirstOrDefault();
                if (inOrder.InOrderNo == picking.PickingNo)
                    isRelated = true;

                deliveryNote = inOrder.InOrderPos_InOrder.SelectMany(c => c.DeliveryNotePos_InOrderPos).Select(c => c.DeliveryNote).FirstOrDefault();
            }

            // Search OutOrder
            IEnumerable<OutOrder> queryOutOrder = picking.PickingPos_Picking.Where(c => c.OutOrderPos != null).Select(c => c.OutOrderPos).Select(c => c.OutOrder).Distinct();
            if (queryOutOrder.Any() && queryOutOrder.Count() == 1)
            {
                outOrder = queryOutOrder.FirstOrDefault();
                if (outOrder.OutOrderNo == picking.PickingNo)
                    isRelated = true;

                deliveryNote = outOrder.OutOrderPos_OutOrder.SelectMany(c => c.DeliveryNotePos_OutOrderPos).Select(c => c.DeliveryNote).FirstOrDefault();
            }

            return isRelated;
        }



        #endregion

        #region MirrorPicking

        public virtual string OnGetNewNoForMirroredPicking(Picking from, string formatNewNo = Picking.FormatNewNo)
        {
            return Root.NoManager.GetNewNo(Database, typeof(Picking), Picking.NoColumnName, formatNewNo, this);
        }

        public virtual Picking MirrorPicking(DatabaseApp databaseApp, Picking from, string formatNewNo = Picking.FormatNewNo)
        {
            string secondaryKey = OnGetNewNoForMirroredPicking(from, formatNewNo);
            Picking mirroredPicking = Picking.NewACObject(databaseApp, null, secondaryKey);
            mirroredPicking.CopyFrom(from, true);
            mirroredPicking.MirroredFromPickingID = from.PickingID;
            PickingPos[] positions = from.PickingPos_Picking.ToArray();
            foreach (PickingPos fromPos in positions)
            {
                PickingPos mirroredPos = PickingPos.NewACObject(databaseApp, mirroredPicking);
                mirroredPos.CopyFrom(fromPos, true, false);
                MirroredPickingProcesPos(mirroredPos, fromPos);
                mirroredPicking.PickingPos_Picking.Add(mirroredPos);
            }
            return mirroredPicking;
        }

        public virtual void MirroredPickingProcesPos(PickingPos mirroredPos, PickingPos fromPos)
        {
            mirroredPos.ToFacility = fromPos.FromFacility;
            mirroredPos.FromFacility = fromPos.ToFacility;
        }

        public virtual string OnGetNewNoForSupplyPicking(Picking from, List<Picking> mirroredPickings, int storedMirroredPickingsCount, string formatNewNo = Picking.FormatNoForSupply)
        {
            int newSequence = mirroredPickings.Count + storedMirroredPickingsCount + 1;
            return String.Format(formatNewNo, from.PickingNo, newSequence);
        }

        public virtual IEnumerable<Picking> CreateSupplyPickings(DatabaseApp databaseApp, Picking from, bool? separatePickingForEachPos = null, string formatNewNo = Picking.FormatNoForSupply)
        {
            int storedMirroredPickingsCount = databaseApp.Picking.Where(c => c.MirroredFromPickingID == from.PickingID).Count();

            if (!separatePickingForEachPos.HasValue)
            {
                if (from.ACClassMethod != null)
                    separatePickingForEachPos = from.ACClassMethod.ContinueByError;
                else
                    separatePickingForEachPos = false;
            }

            MDPickingType pickingType =  databaseApp.MDPickingType.FirstOrDefault(c => c.MDPickingTypeIndex == (short)GlobalApp.PickingType.AutomaticRelocation);
            MDDelivPosLoadState loadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(databaseApp, MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad).FirstOrDefault();

            List<Picking> mirroredPickings = new List<Picking>();
            Picking mirroredPicking = null;
            PickingPos[] positions = from.PickingPos_Picking.ToArray();
            foreach (PickingPos fromPos in positions)
            {
                if (mirroredPicking == null || separatePickingForEachPos.Value)
                {
                    string secondaryKey = OnGetNewNoForSupplyPicking(from, mirroredPickings, storedMirroredPickingsCount, formatNewNo);
                    mirroredPicking = Picking.NewACObject(databaseApp, null, secondaryKey);
                    // Assign related Wokflow for supply
                    mirroredPicking.CopyFrom(from, true);
                    if (pickingType != null)
                        mirroredPicking.MDPickingType = pickingType;
                    if (from.ACClassMethod != null && from.ACClassMethod.ACClassMethod1_ParentACClassMethod != null)
                        mirroredPicking.ACClassMethod = from.ACClassMethod.ACClassMethod1_ParentACClassMethod;
                    mirroredPicking.MirroredFromPickingID = from.PickingID;
                    mirroredPickings.Add(mirroredPicking);
                }

                PickingPos mirroredPos = PickingPos.NewACObject(databaseApp, mirroredPicking);
                mirroredPos.CopyFrom(fromPos, true, false);
                mirroredPos.ToFacility = fromPos.FromFacility;
                mirroredPos.FromFacility = null;
                mirroredPos.MDDelivPosLoadState = loadState;
                foreach (FacilityReservation reservation in fromPos.FacilityReservation_PickingPos.ToArray())
                {
                    string secondaryKey = Root.NoManager.GetNewNo(databaseApp, typeof(FacilityReservation), FacilityReservation.NoColumnName, FacilityReservation.FormatNewNo, null);
                    FacilityReservation mirroredReservation = FacilityReservation.NewACObject(databaseApp, mirroredPos, secondaryKey);
                    mirroredReservation.CopyFrom(reservation, true);
                    mirroredReservation.PickingPos = mirroredPos;

                    mirroredPos.FacilityReservation_PickingPos.Add(mirroredReservation);
                }
                mirroredPicking.PickingPos_Picking.Add(mirroredPos);
            }
            return mirroredPickings;
        }

        #endregion

        #endregion

        #region Routing

        public virtual IEnumerable<Route> GetRoutes(PickingPos pickingPos,
                                DatabaseApp dbApp, Database dbIPlus,
                                gip.core.datamodel.ACClass currentProcessModule,
                                ACPartslistManager.SearchMode searchMode,
                                DateTime? filterTimeOlderThan,
                                out QrySilosResult possibleSilos,
                                Guid? ignoreFacilityID,
                                IEnumerable<gip.core.datamodel.ACClass> exclusionList = null,
                                ACValueList projSpecificParams = null,
                                bool onlyContainer = true,
                                short reservationMode = 0,
                                string selectionRuleID = "PAMSilo.Deselector")
        {
            if (currentProcessModule == null)
            {
                throw new NullReferenceException("AccessedProcessModule is null");
            }

            possibleSilos = FindSilos(pickingPos, dbApp, dbIPlus, searchMode, filterTimeOlderThan, ignoreFacilityID, exclusionList, projSpecificParams, onlyContainer, reservationMode);
            if (possibleSilos == null || possibleSilos.FilteredResult == null || !possibleSilos.FilteredResult.Any())
                return null;

            RoutingResult result = null;
            if (searchMode == ACPartslistManager.SearchMode.OnlyEnabledOldestSilo)
            {
                Facility oldestSilo = possibleSilos.FilteredResult.FirstOrDefault().StorageBin;
                if (oldestSilo == null)
                    return null;
                if (!oldestSilo.VBiFacilityACClassID.HasValue)
                    return null;
                var oldestSiloClass = oldestSilo.GetFacilityACClass(dbIPlus);

                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    RoutingService = this.RoutingService,
                    Database = dbIPlus,
                    AttachRouteItemsToContext = true,
                    Direction = RouteDirections.Backwards,
                    SelectionRuleID = selectionRuleID,
                    DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && oldestSilo.VBiFacilityACClassID == c.ACClassID,
                    DBDeSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != currentProcessModule.ACClassID,
                    MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                    IncludeReserved = true,
                    IncludeAllocated = true,
                    DBRecursionLimit = 10
                };

                result = ACRoutingService.SelectRoutes(currentProcessModule, oldestSiloClass, routingParameters);
                if (result.Routes == null || !result.Routes.Any())
                {
                    // TODO: Fehler
                    return null;
                }
            }
            else
            {
                // 2. Suche Routen zu dieser Waage die von den vorgeschlagenen Silos aus führen
                var acClassIDsOfPossibleSilos = possibleSilos.FilteredResult.Where(c => c.StorageBin.VBiFacilityACClassID.HasValue).Select(c => c.StorageBin.VBiFacilityACClassID.Value);
                IEnumerable<string> possibleSilosACUrl = possibleSilos.FilteredResult.Where(c => c.StorageBin.FacilityACClass != null).Select(x => x.StorageBin.FacilityACClass.GetACUrlComponent());

                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    RoutingService = this.RoutingService,
                    Database = dbIPlus,
                    AttachRouteItemsToContext = true,
                    Direction = RouteDirections.Backwards,
                    SelectionRuleID = selectionRuleID,
                    DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && acClassIDsOfPossibleSilos.Contains(c.ACClassID),
                    DBDeSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != currentProcessModule.ACClassID,
                    MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                    IncludeReserved = true,
                    IncludeAllocated = true,
                    DBRecursionLimit = 10
                };

                result = ACRoutingService.SelectRoutes(currentProcessModule, possibleSilosACUrl, routingParameters);

                if (result.Routes == null || !result.Routes.Any())
                {
                    // TODO: Fehler
                    return null;
                }
            }
            return result.Routes;
        }

        public virtual QrySilosResult FindSilos(PickingPos pickingPos,
                                DatabaseApp dbApp, Database dbIPlus,
                                ACPartslistManager.SearchMode searchMode,
                                DateTime? filterTimeOlderThan,
                                Guid? ignoreFacilityID,
                                IEnumerable<gip.core.datamodel.ACClass> exclusionList = null,
                                ACValueList projSpecificParams = null,
                                bool onlyContainer = true,
                                short reservationMode = 0)
        {
            Material material = pickingPos.Material;

            // 1. Suche freie Silos, mit dem zu dosierenden Material + die Freigegeben sind + die keine gesperrte Chargen haben
            // soriert nach der ältesten eingelagerten Charge
            ACPartslistManager.QrySilosResult facilityQuery = null;
            if (material != null)
            {
                if (filterTimeOlderThan.HasValue)
                {
                    facilityQuery = SilosWithMaterial(dbApp, pickingPos, filterTimeOlderThan.Value, searchMode != ACPartslistManager.SearchMode.AllSilos, projSpecificParams, onlyContainer);
                }
                else
                {
                    facilityQuery = SilosWithMaterial(dbApp, pickingPos, searchMode != ACPartslistManager.SearchMode.AllSilos, projSpecificParams, onlyContainer);
                }
            }
            ApplyLotReservationFilter(facilityQuery, pickingPos, reservationMode);

            if (onlyContainer)
            {
                facilityQuery.ApplyBlockedQuantsFilter();
                //possibleSilos = facilityQuery.FoundSilos
                //                .Where(c => !c.QryHasBlockedQuants.Any())
                //                .ToList();
            }
            else
            {
                facilityQuery.ApplyFreeQuantsInBinFilter();
                //possibleSilos = facilityQuery.FoundSilos
                //                .Where(c => (c.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBin && c.QryHasFreeQuants.Any())
                //                           || (!c.QryHasBlockedQuants.Any()))
                //                .ToList();
            }
            facilityQuery.RemoveFacility(ignoreFacilityID, exclusionList);

            return facilityQuery;
        }

        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// </summary>
        virtual public ACPartslistManager.QrySilosResult SilosWithMaterial(DatabaseApp ctx,
                                                        PickingPos pickingPos,
                                                        bool checkOutwardEnabled = true,
                                                        ACValueList projSpecificParams = null,
                                                        bool onlyContainer = true)
        {
            try
            {
                Material material = pickingPos?.Material;
                if (material == null)
                    return new ACPartslistManager.QrySilosResult();

                return new ACPartslistManager.QrySilosResult(s_cQry_PickingSilosWithMaterial(ctx, material, checkOutwardEnabled, onlyContainer).ToArray());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithMaterial(110)", msg);

                return new ACPartslistManager.QrySilosResult();
            }
        }

        virtual public ACPartslistManager.QrySilosResult SilosWithMaterial(DatabaseApp ctx,
                                                        PickingPos pickingPos,
                                                        DateTime filterTimeOlderThan,
                                                        bool checkOutwardEnabled = true,
                                                        ACValueList projSpecificParams = null,
                                                        bool onlyContainer = true)
        {
            try
            {
                Material material = pickingPos?.Material;
                if (material == null)
                    return new ACPartslistManager.QrySilosResult();

                return new ACPartslistManager.QrySilosResult(s_cQry_PickingSilosWithMaterialTime(ctx, material, checkOutwardEnabled, filterTimeOlderThan, onlyContainer).ToArray());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithMaterial(110)", msg);

                return new ACPartslistManager.QrySilosResult();
            }
        }

        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// </summary>
        protected static readonly Func<DatabaseApp, Material, bool, bool, IQueryable<FacilityCharge>> s_cQry_PickingSilosWithMaterial =
        CompiledQuery.Compile<DatabaseApp, Material, bool, bool, IQueryable<FacilityCharge>>(
            (ctx, material, checkOutwardEnabled, onlyContainer) => ctx.FacilityCharge
                                                .Include("Facility.FacilityStock_Facility")
                                                .Include("MDReleaseState")
                                                .Include("FacilityLot.MDReleaseState")
                                                .Include("Facility.MDFacilityType")
                                                .Where(c => c.NotAvailable == false
                                                        && ((onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                            || (!onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.Facility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                        && ((!onlyContainer
                                                                && (   (material.ProductionMaterialID.HasValue && (c.MaterialID == material.ProductionMaterialID || (c.Material.ProductionMaterialID.HasValue && c.Material.ProductionMaterialID == material.ProductionMaterialID)))
                                                                    || (c.MaterialID == material.MaterialID)))
                                                            || (onlyContainer && c.Facility.MaterialID.HasValue
                                                                && ((material.ProductionMaterialID.HasValue && c.Facility.MaterialID == material.ProductionMaterialID)
                                                                    || (!material.ProductionMaterialID.HasValue && c.Facility.MaterialID == material.MaterialID)))
                                                            )
                                                      && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                          || !checkOutwardEnabled)
                                                      && c.FillingDate.HasValue)
                                               .OrderBy(c => c.FillingDate)
                                               //.Select(c => c.Facility)
        );

        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// </summary>
        protected static readonly Func<DatabaseApp, Material, bool, DateTime, bool, IQueryable<FacilityCharge>> s_cQry_PickingSilosWithMaterialTime =
        CompiledQuery.Compile<DatabaseApp, Material, bool, DateTime, bool, IQueryable<FacilityCharge>>(
            (ctx, material, checkOutwardEnabled, filterTimeOlderThan, onlyContainer) => ctx.FacilityCharge
                                                .Include("Facility.FacilityStock_Facility")
                                                .Include("MDReleaseState")
                                                .Include("FacilityLot.MDReleaseState")
                                                .Include("Facility.MDFacilityType")
                                                .Where(c => c.NotAvailable == false
                                                        && ((onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                            || (!onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.Facility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                        && ((!onlyContainer
                                                                && (   (material.ProductionMaterialID.HasValue && (c.MaterialID == material.ProductionMaterialID || (c.Material.ProductionMaterialID.HasValue && c.Material.ProductionMaterialID == material.ProductionMaterialID)))
                                                                    || (c.MaterialID == material.MaterialID)))
                                                            || (onlyContainer && c.Facility.MaterialID.HasValue
                                                                && ((material.ProductionMaterialID.HasValue && c.Facility.MaterialID == material.ProductionMaterialID)
                                                                    || (!material.ProductionMaterialID.HasValue && c.Facility.MaterialID == material.MaterialID)))
                                                            )
                                                      && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                          || !checkOutwardEnabled)
                                                      && ((c.Facility.MinStockQuantity.HasValue && c.Facility.MinStockQuantity.Value < -0.1)
                                                          || (c.FillingDate.HasValue && c.FillingDate <= filterTimeOlderThan)))
                                               .OrderBy(c => c.FillingDate)
                                               //.Select(c => c.Facility)
        );


        protected virtual void ApplyLotReservationFilter(QrySilosResult qrySilosResult, PickingPos pickingPos, short reservationMode)
        {
            if (qrySilosResult == null)
                return;
            qrySilosResult.ApplyLotReservationFilter(pickingPos, reservationMode);
        }
        #endregion

        #region Target-Selection
        public BindingList<POPartslistPosReservation> GetTargets(DatabaseApp databaseApp, ConfigManagerIPlus configManager, ACComponent routingService, gip.mes.datamodel.ACClassWF vbACClassWF,
            Picking picking, PickingPos pickingPos, string configACUrl,
            bool showCellsInRoute, bool showSelectedCells, bool showEnabledCells, bool showSameMaterialCells, bool preselectFirstReservation, string selectionRuleID = "Storage")
        {
            BindingList<POPartslistPosReservation> reservationCollection = new BindingList<POPartslistPosReservation>();
            MaterialWFACClassMethod materialWFACClassMethod = null;
            gip.core.datamodel.ACClassWF acClassWFDischarging = GetACClassWFDischarging(databaseApp, picking, vbACClassWF, pickingPos, out materialWFACClassMethod);

            if (acClassWFDischarging != null && materialWFACClassMethod != null)
            {
                core.datamodel.ACClassWF aCClassWF = vbACClassWF.FromIPlusContext<gip.core.datamodel.ACClassWF>(databaseApp.ContextIPlus);
                List<Route> routes = new List<Route>();

                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    RoutingService = routingService,
                    Database = databaseApp.ContextIPlus,
                    AttachRouteItemsToContext = true,
                    SelectionRuleID = selectionRuleID,
                    Direction = RouteDirections.Forwards,
                    DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                    MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                    IncludeReserved = true,
                    IncludeAllocated = true,
                    ResultMode = RouteResultMode.ShortRoute
                };

                foreach (gip.core.datamodel.ACClass instance in acClassWFDischarging.ParentACClass.DerivedClassesInProjects)
                {
                    RoutingResult rResult = ACRoutingService.FindSuccessors(instance, routingParameters);
                    if (rResult.Routes != null && rResult.Routes.Any())
                        routes.AddRange(rResult.Routes);
                }

                #region Filter routes if is selected    ShowCellsInRoute
                bool checkShowCellsInRoute = showCellsInRoute && acClassWFDischarging != null && acClassWFDischarging.ACClassWF1_ParentACClassWF != null;
                if (checkShowCellsInRoute)
                {
                    List<IACConfigStore> mandatoryConfigStores =
                    GetCurrentConfigStores(
                        aCClassWF,
                        vbACClassWF,
                        materialWFACClassMethod.MaterialWFID,
                        picking
                    );

                    int priorityLevel = 0;
                    IACConfig allowedInstancesOnRouteConfig =
                        configManager.GetConfiguration(
                            mandatoryConfigStores,
                            configACUrl + "\\",
                            acClassWFDischarging.ACClassWF1_ParentACClassWF.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Allowed_instances.ToString(),
                            null,
                            out priorityLevel);
                    if (allowedInstancesOnRouteConfig != null)
                    {
                        List<RuleValue> allowedInstancesRuleValueList = RulesCommand.ReadIACConfig(allowedInstancesOnRouteConfig);
                        List<string> classes = allowedInstancesRuleValueList.SelectMany(c => c.ACClassACUrl).Distinct().ToList();
                        if (classes.Any())
                        {
                            routes = routes.Where(c => c.Items.Select(x => x.Source.GetACUrl()).Intersect(classes).Any()).ToList();
                        }
                    }
                }
                #endregion

                var availableModules = routes.Select(c => c.LastOrDefault())
                    .Distinct(new TargetEqualityComparer())
                    .OrderBy(c => c.Target.ACIdentifier);
                if (availableModules.Any())
                {
                    FacilityReservation[] selectedModules = new FacilityReservation[] { };
                    if (
                            pickingPos.EntityState != System.Data.EntityState.Added
                            && !pickingPos.FacilityReservation_PickingPos.Any(c => c.VBiACClassID.HasValue && c.EntityState != System.Data.EntityState.Unchanged)
                        )
                    {
                        pickingPos.FacilityReservation_PickingPos.AutoRefresh();
                        selectedModules = pickingPos
                            .FacilityReservation_PickingPos
                            .CreateSourceQuery()
                            .Include(c => c.Facility)
                            .Include(c => c.Facility.Material)
                            .Where(c => c.VBiACClassID.HasValue)
                            .AutoMergeOption()
                            .ToArray();
                    }
                    else
                    {
                        selectedModules = pickingPos.FacilityReservation_PickingPos.Where(c => c.VBiACClassID.HasValue).ToArray();
                    }


                    if (availableModules != null)
                    {
                        List<Guid> notSelected = new List<Guid>();
                        foreach (var routeItem in availableModules)
                        {
                            //FacilityReservation selectedModule = selectedModules.Where(c => c.VBiACClassID == acClass.ACClassID).FirstOrDefault();
                            //if (selectedModule == null)
                            notSelected.Add(routeItem.Target.ACClassID);
                        }
                        var queryUnselFacilities =
                            databaseApp
                            .Facility
                            .Include(c => c.Material)
                            .Where(DynamicQueryable.BuildOrExpression<Facility, Guid>(c => c.VBiFacilityACClassID.Value, notSelected))
                            .AutoMergeOption()
                            .ToArray();

                        foreach (var routeItem in availableModules)
                        {
                            //if (OnFilterTarget(routeItem))
                            //    continue;
                            Facility unselFacility = null;
                            FacilityReservation selectedReservationForModule = selectedModules.Where(c => c.VBiACClassID == routeItem.Target.ACClassID).FirstOrDefault();
                            if (selectedReservationForModule == null)
                            {
                                if (showSelectedCells)
                                    continue;
                                unselFacility = queryUnselFacilities.Where(c => c.VBiFacilityACClassID == routeItem.Target.ACClassID).FirstOrDefault();
                                if (showEnabledCells && unselFacility != null && !unselFacility.InwardEnabled)
                                    continue;
                                bool ifMaterialMatch =
                                        unselFacility != null
                                        && unselFacility.Material != null
                                        && (
                                                pickingPos.Material != null
                                            && (   (unselFacility.MaterialID == pickingPos.Material.MaterialID)
                                                || (pickingPos.Material.ProductionMaterialID.HasValue && pickingPos.Material.ProductionMaterialID.Value == unselFacility.MaterialID))
                                         );
                                if (showSameMaterialCells && !ifMaterialMatch)
                                    continue;
                            }
                            reservationCollection.Add(new POPartslistPosReservation(routeItem.Target, pickingPos, null, selectedReservationForModule, unselFacility, acClassWFDischarging, aCClassWF));
                        }
                    }

                    // select first if only one is present
                    if (preselectFirstReservation
                        && ((pickingPos.EntityState == System.Data.EntityState.Added && reservationCollection.Count() == 1)
                            || ((pickingPos.EntityState == System.Data.EntityState.Unchanged || pickingPos.EntityState == System.Data.EntityState.Modified)
                                    && reservationCollection.Count() == 1
                                    && !reservationCollection.Any(c => c.IsChecked))))
                        reservationCollection[0].IsChecked = true;
                }
            }
            return reservationCollection;
        }

        public List<IACConfigStore> GetCurrentConfigStores(gip.core.datamodel.ACClassWF currentACClassWF, gip.mes.datamodel.ACClassWF vbCurrentACClassWF, Guid? materialWFID, Picking picking)
        {
            List<IACConfigStore> configStores = new List<IACConfigStore>();
            if (picking != null)
                configStores.Add(picking);
            MaterialWFConnection matWFConnection = GetMaterialWFConnection(vbCurrentACClassWF, materialWFID);
            configStores.Add(matWFConnection.MaterialWFACClassMethod);
            configStores.Add(currentACClassWF.ACClassMethod);
            if (currentACClassWF.RefPAACClassMethod != null)
                configStores.Add(currentACClassWF.RefPAACClassMethod);
            return configStores;
        }

        public MaterialWFConnection GetMaterialWFConnection(gip.mes.datamodel.ACClassWF aCClassWF, Guid? materialWFID)
        {
            return
                aCClassWF
                .MaterialWFConnection_ACClassWF
                .Where(c => c.MaterialWFACClassMethod.MaterialWFID == materialWFID)
                .FirstOrDefault();
        }

        public gip.core.datamodel.ACClassWF GetACClassWFDischarging(DatabaseApp databaseApp, Picking picking, gip.mes.datamodel.ACClassWF vbACClassWF, PickingPos pickingPos, out MaterialWFACClassMethod materialWFACClassMethod)
        {
            materialWFACClassMethod = null;

            Material pickingIntermediateMaterial = databaseApp.Material.Where(c => c.MDMaterialType != null && c.MDMaterialType.MDMaterialTypeIndex == (short)MDMaterialGroup.MaterialGroupTypes.Picking).FirstOrDefault();
            if (pickingIntermediateMaterial == null)
                return null;

            materialWFACClassMethod = vbACClassWF.ACClassMethod.MaterialWFACClassMethod_ACClassMethod.FirstOrDefault();
            if (materialWFACClassMethod == null)
                return null;
            Guid materialWFACClassMethodID = materialWFACClassMethod.MaterialWFACClassMethodID;

            gip.core.datamodel.ACClass acClassOfPWDischarging = null;
            gip.core.datamodel.ACClassWF acClassWFDischarging = null;

            var matWFConnections = databaseApp.MaterialWFConnection.Where(c => c.MaterialID == pickingIntermediateMaterial.MaterialID
                                        && c.MaterialWFACClassMethodID == materialWFACClassMethodID);

            if (!matWFConnections.Any())
            {
                // TODO: Show Message
                return null;
            }

            foreach (MaterialWFConnection matWFConnection in matWFConnections)
            {
                acClassWFDischarging = matWFConnection.ACClassWF.FromIPlusContext<gip.core.datamodel.ACClassWF>(databaseApp.ContextIPlus);
                if (acClassWFDischarging == null)
                {
                    continue;
                }

                acClassOfPWDischarging = acClassWFDischarging.PWACClass;
                if (acClassOfPWDischarging == null)
                {
                    acClassWFDischarging = null;
                    // TODO: Show Message
                    continue;
                }

                Type typeOfDis = acClassOfPWDischarging.ObjectType;
                if (!typeof(IPWNodeDeliverMaterial).IsAssignableFrom(typeOfDis)
                    || acClassWFDischarging.ACClassMethodID != vbACClassWF.RefPAACClassMethodID)
                {
                    acClassOfPWDischarging = null;
                    acClassWFDischarging = null;
                    // TODO: Show Message
                    continue;
                }
                break;
            }

            return acClassWFDischarging;
        }

        #endregion

        #region Start Workflow
        public MsgWithDetails StartPicking(ACComponent appManager, DatabaseApp databaseApp,
                                            gip.core.datamodel.ACClassMethod acClassMethod, gip.mes.datamodel.ACClassWF vbACClassWf, Picking picking,
                                            bool checkIfWorkflowIsRunning = true)
        {
            gip.core.datamodel.ACClassWF acClassWf = null;
            if (vbACClassWf != null)
                acClassWf = vbACClassWf.FromIPlusContext<gip.core.datamodel.ACClassWF>(databaseApp.ContextIPlus);
            return StartPicking(databaseApp, appManager, picking, acClassMethod, acClassWf, checkIfWorkflowIsRunning);
        }

        public MsgWithDetails StartPicking(DatabaseApp databaseApp, ACComponent appManager,
                                            Picking picking, gip.core.datamodel.ACClassMethod acClassMethod, gip.core.datamodel.ACClassWF acClassWf = null,
                                            bool checkIfWorkflowIsRunning = true)
        {
            if (checkIfWorkflowIsRunning)
            {
                var acProgramIDs = databaseApp.OrderLog.Where(c => c.PickingPosID.HasValue
                                                    && c.PickingPos.PickingID == picking.PickingID)
                                         .Select(c => c.VBiACProgramLog.ACProgramID)
                                         .Distinct()
                                         .ToArray();

                if (acProgramIDs != null && acProgramIDs.Any())
                {
                    ChildInstanceInfoSearchParam searchParam = new ChildInstanceInfoSearchParam() { OnlyWorkflows = true, ACProgramIDs = acProgramIDs };
                    var childInstanceInfos = appManager.GetChildInstanceInfo(1, searchParam);
                    if (childInstanceInfos != null && childInstanceInfos.Any())
                    {
                        //var childInstanceInfo = childInstanceInfos.FirstOrDefault();
                        //string acUrlComand = String.Format("{0}\\{1}!{2}", childInstanceInfo.ACUrlParent, childInstanceInfo.ACIdentifier, PWMethodTransportBase.ReloadBPAndResumeACIdentifier);
                        //pAppManager.ACUrlCommand(acUrlComand);
                        return null;
                    }
                }
            }


            ACMethod acMethod = appManager.NewACMethod(acClassMethod.ACIdentifier);
            if (acMethod == null)
                return null;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(gip.core.datamodel.ACProgram), gip.core.datamodel.ACProgram.NoColumnName, gip.core.datamodel.ACProgram.FormatNewNo, this);
            gip.core.datamodel.ACProgram program = gip.core.datamodel.ACProgram.NewACObject(databaseApp.ContextIPlus, null, secondaryKey);
            program.ProgramACClassMethod = acClassMethod;
            program.WorkflowTypeACClass = acClassMethod.WorkflowTypeACClass;
            databaseApp.ContextIPlus.ACProgram.AddObject(program);
            //CurrentProdOrderPartslist.VBiACProgramID = program.ACProgramID;
            picking.PickingState = PickingStateEnum.WFActive;
            MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
            if (saveMsg == null)
            {
                ACValue paramProgram = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACProgram.ClassName);
                if (paramProgram == null)
                    acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACProgram.ClassName, typeof(Guid), program.ACProgramID));
                else
                    paramProgram.Value = program.ACProgramID;

                ACValue acValuePPos = acMethod.ParameterValueList.GetACValue(Picking.ClassName);
                if (acValuePPos == null)
                    acMethod.ParameterValueList.Add(new ACValue(Picking.ClassName, typeof(Guid), picking.PickingID));
                else
                    acValuePPos.Value = picking.PickingID;

                if (acClassWf != null)
                {
                    ACValue paramACClassWF = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACClassWF.ClassName);
                    if (paramACClassWF == null)
                        acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACClassWF.ClassName, typeof(Guid), acClassWf.ACClassWFID));
                    else
                        paramACClassWF.Value = acClassWf.ACClassWFID;
                }

                appManager.ExecuteMethod(acClassMethod.ACIdentifier, acMethod);
            }
            return saveMsg;
        }
        #endregion
    }
}


