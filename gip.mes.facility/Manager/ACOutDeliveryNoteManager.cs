using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Data.Objects;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioSales, "", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public partial class ACOutDeliveryNoteManager : PARole
    {
        #region c´tors
        public ACOutDeliveryNoteManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
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

        static readonly Func<DatabaseApp, IQueryable<MDOutOrderPosState>> s_cQry_OutOrderInProcess =
        CompiledQuery.Compile<DatabaseApp, IQueryable<MDOutOrderPosState>>(
            (ctx) => from c in ctx.MDOutOrderPosState where c.MDOutOrderPosStateIndex == (Int16)MDOutOrderPosState.OutOrderPosStates.InProcess select c
        );

        static readonly Func<DatabaseApp, IQueryable<MDOutOrderPosState>> s_cQry_OutOrderCompleted =
        CompiledQuery.Compile<DatabaseApp, IQueryable<MDOutOrderPosState>>(
            (ctx) => from c in ctx.MDOutOrderPosState where c.MDOutOrderPosStateIndex == (Int16)MDOutOrderPosState.OutOrderPosStates.Completed select c
        );
        #endregion

        #region Properties
        ACMethodBooking _BookParamOutwardMovementClone;
        public ACMethodBooking BookParamOutwardMovementClone(ACComponent facilityManager, DatabaseApp dbApp)
        {
            if (_BookParamOutwardMovementClone != null)
                return _BookParamOutwardMovementClone;
            _BookParamOutwardMovementClone = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_OutOrderPosOutwardMovement.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            return _BookParamOutwardMovementClone;
        }

        ACMethodBooking _BookParamOutCancelClone;
        public ACMethodBooking BookParamOutCancelClone(ACComponent facilityManager, DatabaseApp dbApp)
        {
            if (_BookParamOutCancelClone != null)
                return _BookParamOutCancelClone;
            _BookParamOutCancelClone = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_OutOrderPosCancel.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            return _BookParamOutCancelClone;
        }
        #endregion

        #region static Methods
        public const string C_DefaultServiceACIdentifier = "OutDeliveryNoteManager";

        public static ACOutDeliveryNoteManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACOutDeliveryNoteManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ACOutDeliveryNoteManager> ACRefToServiceInstance(ACComponent requester)
        {
            ACOutDeliveryNoteManager serviceInstance = GetServiceInstance(requester) as ACOutDeliveryNoteManager;
            if (serviceInstance != null)
                return new ACRef<ACOutDeliveryNoteManager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region Public Methods

        #region Contract Assignment
        [ACMethodInfo("", "en{'Assign'}de{'Zuordnen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public virtual Msg AssignContractOutOrderPos(OutOrderPos currentOutOrderPos, OutOrder currentContract, Nullable<double> enteredPartialQuantity, DatabaseApp dbApp, ACComponent facilityManager, List<object> resultNewEntities)
        {
            if (currentOutOrderPos == null || currentContract == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50009"));
            }

            var queryDelivStateAssigned = s_cQry_CompletelyAssigned.Invoke(dbApp);
            if (!queryDelivStateAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignContractOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }

            var queryDelivStateSubsetAssigned = s_cQry_SubsetAssigned.Invoke(dbApp);
            if (!queryDelivStateSubsetAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignContractOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }


            // 1. Erzeuge Unterposition
            currentOutOrderPos.AutoRefresh(dbApp);
            OutOrderPos childOutOrderPos = OutOrderPos.NewACObject(dbApp, currentOutOrderPos);
            childOutOrderPos.OutOrder = currentContract;
            childOutOrderPos.MaterialPosType = GlobalApp.MaterialPosTypes.InwardPart;
            var queryOutOrderPosState = s_cQry_OutOrderInProcess.Invoke(dbApp);
            if (!queryOutOrderPosState.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignContractOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }
            childOutOrderPos.MDOutOrderPosState = queryOutOrderPosState.FirstOrDefault();
            resultNewEntities.Add(childOutOrderPos);

            // 2. Rufe Teilmenge ab
            double partialQuantityUOM = 0;
            if (enteredPartialQuantity.HasValue)
            {
                partialQuantityUOM = currentOutOrderPos.Material.ConvertToBaseQuantity(enteredPartialQuantity.Value, currentOutOrderPos.MDUnit);
                if ((currentOutOrderPos.RemainingCallQuantityUOM + 0.000001) < partialQuantityUOM
                    || currentOutOrderPos.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.CompletelyAssigned
                    || currentOutOrderPos.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.Delivered)
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "AssignContractOutOrderPos(0)",
                        Message = Root.Environment.TranslateMessage(this, "Warning50000", enteredPartialQuantity.Value, currentOutOrderPos.MDUnit.MDUnitName, currentOutOrderPos.RemainingCallQuantity, currentOutOrderPos.MDUnit.MDUnitName)
                    };
                }
                currentOutOrderPos.CalledUpQuantityUOM += partialQuantityUOM;
                if (Math.Abs(currentOutOrderPos.RemainingCallQuantityUOM - 0) <= Double.Epsilon)
                    currentOutOrderPos.MDDelivPosState = queryDelivStateAssigned.First();
                else
                    currentOutOrderPos.MDDelivPosState = queryDelivStateSubsetAssigned.First();
            }
            else
            {
                partialQuantityUOM = currentOutOrderPos.RemainingCallQuantityUOM;
                currentOutOrderPos.CalledUpQuantityUOM = currentOutOrderPos.TargetQuantityUOM;
                currentOutOrderPos.MDDelivPosState = queryDelivStateAssigned.First();
            }
            childOutOrderPos.TargetQuantityUOM = partialQuantityUOM;

            return null;
        }


        [ACMethodInfo("", "en{'Remove'}de{'Entfernen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public virtual Msg UnassignContractOutOrderPos(OutOrderPos currentReleasePos, DatabaseApp dbApp)
        {
            if (currentReleasePos == null || currentReleasePos.OutOrderPos1_ParentOutOrderPos == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50008"));
            }

            var queryDelivStateNotPlanned = s_cQry_NotPlanned.Invoke(dbApp);
            if (!queryDelivStateNotPlanned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignContractOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }

            var queryDelivStateSubsetAssigned = s_cQry_SubsetAssigned.Invoke(dbApp);
            if (!queryDelivStateSubsetAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignContractOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }

            if (currentReleasePos.FacilityBooking_OutOrderPos.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignContractOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50010")
                };
            }

            // 1. Hole Parent-Bestellposition aus Bestellung un korrigiere Abgerufene teilmenge
            OutOrderPos parentOutOrderPos = currentReleasePos.OutOrderPos1_ParentOutOrderPos;
            parentOutOrderPos.AutoRefresh(dbApp);
            currentReleasePos.AutoRefresh(dbApp);
            parentOutOrderPos.CalledUpQuantityUOM -= currentReleasePos.TargetQuantityUOM;

            // 2. Lösche Unter-Bestellposition
            // aagincic note: Autoload of parentOutOrder pos was changed state of OutOrderPos from Deleted to Unmodified
            parentOutOrderPos.OutOrderPos_ParentOutOrderPos.AutoLoad(dbApp);
            currentReleasePos.DeleteACObject(dbApp, false);

            // 3. Korrigiere Status der Parent-Bestellposition
            var queryChildsSubPos = parentOutOrderPos.OutOrderPos_ParentOutOrderPos.Where(c => c.EntityState != System.Data.EntityState.Deleted);
            if (!queryChildsSubPos.Any())
            {
                parentOutOrderPos.MDDelivPosState = queryDelivStateNotPlanned.First();
                if (Math.Abs(parentOutOrderPos.CalledUpQuantityUOM - 0) > Double.Epsilon)
                    parentOutOrderPos.CalledUpQuantityUOM = 0;
            }
            else
            {
                parentOutOrderPos.MDDelivPosState = queryDelivStateSubsetAssigned.First();
            }

            return null;
        }
        #endregion


        #region Delivery-Note

        #region DeliveryNote -> Public methods

        [ACMethodInfo("", "en{'Assign'}de{'Zuordnen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg AssignOutOrderPos(OutOrderPos currentOutOrderPos, DeliveryNote currentDeliveryNote, Nullable<double> enteredPartialQuantity, DatabaseApp dbApp, ACComponent facilityManager, List<object> resultNewEntities)
        {
            if (!PreExecute("AssignOutOrderPos"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50002")
                };
            }

            if (currentOutOrderPos == null || currentDeliveryNote == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50009"));
            }

            var queryDelivStateAssigned = s_cQry_CompletelyAssigned.Invoke(dbApp);
            if (!queryDelivStateAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }

            var queryDelivStateSubsetAssigned = s_cQry_SubsetAssigned.Invoke(dbApp);
            if (!queryDelivStateSubsetAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }


            // 1. Erzeuge Unterposition
            currentOutOrderPos.AutoRefresh(dbApp);
            OutOrderPos childOutOrderPos = OutOrderPos.NewACObject(dbApp, currentOutOrderPos);
            childOutOrderPos.MaterialPosType = GlobalApp.MaterialPosTypes.OutwardPart;
            var queryOutOrderPosState = s_cQry_OutOrderInProcess.Invoke(dbApp);
            if (!queryOutOrderPosState.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }
            resultNewEntities.Add(childOutOrderPos);

            // 2. Rufe Teilmenge ab
            double partialQuantityUOM = 0;
            if (enteredPartialQuantity.HasValue)
            {
                partialQuantityUOM = currentOutOrderPos.Material.ConvertToBaseQuantity(enteredPartialQuantity.Value, currentOutOrderPos.MDUnit);
                if ((currentOutOrderPos.RemainingCallQuantityUOM + 0.000001) < partialQuantityUOM
                    || currentOutOrderPos.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.CompletelyAssigned
                    || currentOutOrderPos.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.Delivered)
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "AssignOutOrderPos(0)",
                        Message = Root.Environment.TranslateMessage(this, "Warning50000", enteredPartialQuantity.Value, currentOutOrderPos.MDUnit.MDUnitName, currentOutOrderPos.RemainingCallQuantity, currentOutOrderPos.MDUnit.MDUnitName)
                    };
                }
                currentOutOrderPos.CalledUpQuantityUOM += partialQuantityUOM;
                if (Math.Abs(currentOutOrderPos.RemainingCallQuantityUOM - 0) <= Double.Epsilon)
                    currentOutOrderPos.MDDelivPosState = queryDelivStateAssigned.First();
                else
                    currentOutOrderPos.MDDelivPosState = queryDelivStateSubsetAssigned.First();
            }
            else
            {
                partialQuantityUOM = currentOutOrderPos.RemainingCallQuantityUOM;
                currentOutOrderPos.CalledUpQuantityUOM = currentOutOrderPos.TargetQuantityUOM;
                currentOutOrderPos.MDDelivPosState = queryDelivStateAssigned.First();
            }
            childOutOrderPos.TargetQuantityUOM = partialQuantityUOM;

            // 3. Erzeuge Lieferscheinposition und weise Unterposition zu
            Msg subMsg = NewDeliveryNotePos(childOutOrderPos, currentDeliveryNote, dbApp, resultNewEntities);

            DeliveryNotePos newDeliveryNotePos = resultNewEntities.Where(c => c is DeliveryNotePos).FirstOrDefault() as DeliveryNotePos;
            if (newDeliveryNotePos == null)
                return subMsg;

            // 4. Erzeuge Buchungsparameter im Vorfeld
            if (facilityManager != null)
                NewFacilityPreBooking(facilityManager, dbApp, newDeliveryNotePos);

            PostExecute("AssignOutOrderPos");
            return null;
        }

        [ACMethodInfo("", "en{'New deliverynote line'}de{'Neue Lieferscheinposition'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg NewDeliveryNotePos(OutOrderPos childOutOrderPos, DeliveryNote currentDeliveryNote, DatabaseApp dbApp, List<object> resultNewEntities)
        {
            DeliveryNotePos newDeliveryNotePos = null;
            if (!PreExecute("NewDeliveryNotePos"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50002")
                };
            }

            if (childOutOrderPos == null || currentDeliveryNote == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50009"));
            }

            newDeliveryNotePos = DeliveryNotePos.NewACObject(dbApp, currentDeliveryNote);
            newDeliveryNotePos.OutOrderPos = childOutOrderPos;
            newDeliveryNotePos.Comment = childOutOrderPos.Comment;
            if (String.IsNullOrEmpty(currentDeliveryNote.Comment))
                currentDeliveryNote.Comment = childOutOrderPos.OutOrder.Comment;
            currentDeliveryNote.DeliveryNotePos_DeliveryNote.Add(newDeliveryNotePos);
            resultNewEntities.Add(newDeliveryNotePos);
            PostExecute("NewDeliveryNotePos");
            return null;
        }

        [ACMethodInfo("", "en{'Remove'}de{'Entfernen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg UnassignOutOrderPos(DeliveryNotePos currentDeliveryNotePos, DatabaseApp dbApp)
        {
            if (!PreExecute("UnassignOutOrderPos"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50002")
                };
            }

            if (currentDeliveryNotePos == null || currentDeliveryNotePos.OutOrderPos == null || currentDeliveryNotePos.OutOrderPos.OutOrderPos1_ParentOutOrderPos == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50008"));
            }

            var queryDelivStateNotPlanned = s_cQry_NotPlanned.Invoke(dbApp);
            if (!queryDelivStateNotPlanned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }

            var queryDelivStateSubsetAssigned = s_cQry_SubsetAssigned.Invoke(dbApp);
            if (!queryDelivStateSubsetAssigned.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignOutOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }

            // 1. Hole Parent-Bestellposition aus Bestellung un korrigiere Abgerufene teilmenge
            OutOrderPos parentOutOrderPos = currentDeliveryNotePos.OutOrderPos.OutOrderPos1_ParentOutOrderPos;
            parentOutOrderPos.AutoRefresh(dbApp);
            currentDeliveryNotePos.OutOrderPos.AutoRefresh(dbApp);
            parentOutOrderPos.CalledUpQuantityUOM -= currentDeliveryNotePos.OutOrderPos.TargetQuantityUOM;

            // 2. Lösche Unter-Bestellposition
            currentDeliveryNotePos.OutOrderPos.DeleteACObject(dbApp, true);
            currentDeliveryNotePos.OutOrderPos = null;

            // 3. Korrigiere Status der Parent-Bestellposition
            parentOutOrderPos.OutOrderPos_ParentOutOrderPos.AutoLoad(dbApp);
            var queryChildsSubPos = parentOutOrderPos.OutOrderPos_ParentOutOrderPos.Where(c => c.EntityState != System.Data.EntityState.Deleted);
            if (!queryChildsSubPos.Any())
            {
                parentOutOrderPos.MDDelivPosState = queryDelivStateNotPlanned.First();
                if (Math.Abs(parentOutOrderPos.CalledUpQuantityUOM - 0) > Double.Epsilon)
                    parentOutOrderPos.CalledUpQuantityUOM = 0;
            }
            else
            {
                parentOutOrderPos.MDDelivPosState = queryDelivStateSubsetAssigned.First();
            }

            // Lösche Lieferscheinposition
            currentDeliveryNotePos.DeleteACObject(dbApp, true);

            PostExecute("UnassignOutOrderPos");
            return null;
        }


        #endregion

        #region Delivery-Note -> Helper methods
        public FacilityPreBooking NewFacilityPreBooking(ACComponent facilityManager, DatabaseApp dbApp, DeliveryNotePos deliveryNotePos)
        {
            ACMethodBooking acMethodClone = BookParamOutwardMovementClone(facilityManager, dbApp);
            deliveryNotePos.OutOrderPos.AutoRefresh(dbApp);
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityPreBooking), FacilityPreBooking.NoColumnName, FacilityPreBooking.FormatNewNo, this);
            FacilityPreBooking facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, deliveryNotePos.OutOrderPos, secondaryKey);
            ACMethodBooking acMethodBooking = acMethodClone.Clone() as ACMethodBooking;
            //acMethodBooking.OutwardQuantity = deliveryNotePos.OutOrderPos.TargetQuantityUOM;
            double quantityUOM = deliveryNotePos.OutOrderPos.TargetQuantityUOM - deliveryNotePos.OutOrderPos.PreBookingOutwardQuantityUOM() - deliveryNotePos.OutOrderPos.ActualQuantityUOM;
            if (deliveryNotePos.OutOrderPos.MDUnit != null)
            {
                acMethodBooking.OutwardQuantity = deliveryNotePos.OutOrderPos.Material.ConvertQuantity(quantityUOM, deliveryNotePos.OutOrderPos.Material.BaseMDUnit, deliveryNotePos.OutOrderPos.MDUnit);
                acMethodBooking.MDUnit = deliveryNotePos.OutOrderPos.MDUnit;
            }
            else
            {
                acMethodBooking.OutwardQuantity = quantityUOM;
            }
            acMethodBooking.OutwardMaterial = deliveryNotePos.OutOrderPos.Material;
            acMethodBooking.OutOrderPos = deliveryNotePos.OutOrderPos;
            if (deliveryNotePos.OutOrderPos.OutOrder.CPartnerCompany != null)
                acMethodBooking.CPartnerCompany = deliveryNotePos.OutOrderPos.OutOrder.CPartnerCompany;
            // TODO: Restliche Parameter von acMethodBooking ausfüllen
            facilityPreBooking.ACMethodBooking = acMethodBooking;
            return facilityPreBooking;
        }

        public List<FacilityPreBooking> CancelFacilityPreBooking(ACComponent facilityManager, DatabaseApp dbApp, DeliveryNotePos deliveryNotePos)
        {
            List<FacilityPreBooking> bookings = new List<FacilityPreBooking>();
            ACMethodBooking acMethodClone = null;
            FacilityPreBooking facilityPreBooking = null;
            if (deliveryNotePos.OutOrderPos == null || deliveryNotePos.OutOrderPos.MDOutOrderPosState.OutOrderPosState == MDOutOrderPosState.OutOrderPosStates.Cancelled)
                return null;
            if (deliveryNotePos.OutOrderPos.EntityState != System.Data.EntityState.Added)
            {
                deliveryNotePos.OutOrderPos.FacilityBooking_OutOrderPos.AutoLoad(dbApp);
                deliveryNotePos.OutOrderPos.FacilityPreBooking_OutOrderPos.AutoLoad(dbApp);
            }
            else
                return null;
            if (deliveryNotePos.OutOrderPos.FacilityPreBooking_OutOrderPos.Any())
                return null;
            foreach (FacilityBooking previousBooking in deliveryNotePos.OutOrderPos.FacilityBooking_OutOrderPos)
            {
                // Wenn einmal Storniert, dann kann nicht mehr storniert werden. Der Fall dürfte normalerweise nicht auftreten, 
                // da der Positionsstatus auch MDOutOrderPosState.OutOrderPosStates.Cancelled sein müsste
                if (previousBooking.FacilityBookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel)
                    return null;
            }

            foreach (FacilityBooking previousBooking in deliveryNotePos.OutOrderPos.FacilityBooking_OutOrderPos)
            {
                if (previousBooking.FacilityBookingType != GlobalApp.FacilityBookingType.OutOrderPosOutwardMovement)
                    continue;
                acMethodClone = BookParamOutCancelClone(facilityManager, dbApp);
                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityPreBooking), FacilityPreBooking.NoColumnName, FacilityPreBooking.FormatNewNo, this);
                facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, deliveryNotePos.OutOrderPos, secondaryKey);
                ACMethodBooking acMethodBooking = acMethodClone.Clone() as ACMethodBooking;
                acMethodBooking.OutwardQuantity = previousBooking.OutwardQuantity * -1;
                if (previousBooking.MDUnit != null)
                    acMethodBooking.MDUnit = previousBooking.MDUnit;
                acMethodBooking.OutwardFacility = previousBooking.OutwardFacility;
                if (previousBooking.OutwardFacilityLot != null)
                    acMethodBooking.OutwardFacilityLot = previousBooking.OutwardFacilityLot;
                if (previousBooking.OutwardFacilityCharge != null)
                    acMethodBooking.OutwardFacilityCharge = previousBooking.OutwardFacilityCharge;
                if (previousBooking.OutwardMaterial != null)
                    acMethodBooking.OutwardMaterial = previousBooking.OutwardMaterial;
                else
                    acMethodBooking.OutwardMaterial = deliveryNotePos.OutOrderPos.Material;
                acMethodBooking.OutOrderPos = deliveryNotePos.OutOrderPos;
                if (previousBooking.CPartnerCompany != null)
                    acMethodBooking.CPartnerCompany = previousBooking.CPartnerCompany;
                facilityPreBooking.ACMethodBooking = acMethodBooking;
                bookings.Add(facilityPreBooking);
            }
            return bookings;
        }

        public List<FacilityPreBooking> CancelFacilityPreBooking(ACComponent facilityManager, DatabaseApp dbApp, DeliveryNote deliveryNote)
        {
            if (deliveryNote == null)
                return null;
            if (deliveryNote.EntityState != System.Data.EntityState.Added)
                deliveryNote.DeliveryNotePos_DeliveryNote.AutoLoad();
            List<FacilityPreBooking> result = null;
            foreach (DeliveryNotePos deliveryNotePos in deliveryNote.DeliveryNotePos_DeliveryNote)
            {
                var subResult = CancelFacilityPreBooking(facilityManager, dbApp, deliveryNotePos);
                if (subResult != null)
                {
                    if (result == null)
                        result = subResult;
                    else
                        result.AddRange(subResult);
                }
            }
            return result;
        }
        #endregion

        #endregion

        #region Invoice

        [ACMethodInfo("", "en{'Assign'}de{'Zuordnen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg GenerateInvoiceFromOutOrder(Guid outOrderID, DatabaseApp dbApp)
        {
            if (!PreExecute("GenerateInvoice"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "GenerateInvoice(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50002")
                };
            }

            OutOrder outOrder = dbApp.OutOrder.FirstOrDefault(c => c.OutOrderID == outOrderID);

            if (outOrder == null || outOrder == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50009"));
            }

            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Invoice), Invoice.NoColumnName, Invoice.FormatNewNo, this);
            Invoice invoice = Invoice.NewACObject(dbApp, null, secondaryKey);
            invoice.OutOrder = outOrder;
            invoice.MDInvoiceState = MDInvoiceState.DefaultMDInvoiceState(dbApp);
            invoice.MDInvoiceType = MDInvoiceType.DefaultMDInvoiceType(dbApp);
            invoice.InvoiceDate = DateTime.Now;
            invoice.CustomerCompany = outOrder.CustomerCompany;
            invoice.BillingCompanyAddress = outOrder.BillingCompanyAddress;
            invoice.DeliveryCompanyAddress = outOrder.DeliveryCompanyAddress;

            foreach (OutOrderPos outOrderPos in outOrder.OutOrderPos_OutOrder)
            {
                InvoicePos invoicePos = InvoicePos.NewACObject(dbApp, invoice);
                invoicePos.Sequence = outOrderPos.Sequence;
                invoicePos.Material = outOrderPos.Material;

                invoicePos.MDUnit = outOrderPos.MDUnit;
                invoicePos.TargetQuantityUOM = outOrderPos.TargetQuantityUOM;

                // MDCountrySalesTaxMDMaterialGroupID
                // MDCountrySalesTaxMaterialID

                invoicePos.PriceNet = outOrderPos.PriceNet;
                invoicePos.PriceGross = outOrderPos.PriceGross;
                invoicePos.SalesTax = 0;
                invoicePos.OutOrderPos = outOrderPos;

                invoice.InvoicePos_Invoice.Add(invoicePos);
            }


            PostExecute("GenerateInvoice");
            return null;
        }

        #endregion

        #endregion

    }
}


