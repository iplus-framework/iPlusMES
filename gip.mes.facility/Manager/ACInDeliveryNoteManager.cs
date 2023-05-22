using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioPurchase, "", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public partial class ACInDeliveryNoteManager : PARole
    {
        #region c´tors
        public ACInDeliveryNoteManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _AutoInsertNewPreBooking = new ACPropertyConfigValue<bool>(this, "AutoInsertNewPreBooking", false);
        }
        #endregion

        #region PrecompiledQueries
        static readonly Func<DatabaseApp, IEnumerable<MDDelivPosState>> s_cQry_CompletelyAssigned =
        EF.CompileQuery<DatabaseApp, IEnumerable<MDDelivPosState>>(
            (ctx) => from c in ctx.MDDelivPosState where c.MDDelivPosStateIndex == (Int16)MDDelivPosState.DelivPosStates.CompletelyAssigned select c
        );

        static readonly Func<DatabaseApp, IEnumerable<MDDelivPosState>> s_cQry_SubsetAssigned =
        EF.CompileQuery<DatabaseApp, IEnumerable<MDDelivPosState>>(
            (ctx) => from c in ctx.MDDelivPosState where c.MDDelivPosStateIndex == (Int16)MDDelivPosState.DelivPosStates.SubsetAssigned select c
        );

        static readonly Func<DatabaseApp, IEnumerable<MDDelivPosState>> s_cQry_NotPlanned =
        EF.CompileQuery<DatabaseApp, IEnumerable<MDDelivPosState>>(
            (ctx) => from c in ctx.MDDelivPosState where c.MDDelivPosStateIndex == (Int16)MDDelivPosState.DelivPosStates.NotPlanned select c
        );

        static readonly Func<DatabaseApp, IEnumerable<MDInOrderPosState>> s_cQry_InOrderInProcess =
        EF.CompileQuery<DatabaseApp, IEnumerable<MDInOrderPosState>>(
            (ctx) => from c in ctx.MDInOrderPosState where c.MDInOrderPosStateIndex == (Int16)MDInOrderPosState.InOrderPosStates.InProcess select c
        );

        static readonly Func<DatabaseApp, IEnumerable<MDInOrderPosState>> s_cQry_InOrderCompleted =
        EF.CompileQuery<DatabaseApp, IEnumerable<MDInOrderPosState>>(
            (ctx) => from c in ctx.MDInOrderPosState where c.MDInOrderPosStateIndex == (Int16)MDInOrderPosState.InOrderPosStates.Completed select c
        );
        #endregion

        #region static Methods
        public const string C_DefaultServiceACIdentifier = "InDeliveryNoteManager";

        public static ACInDeliveryNoteManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACInDeliveryNoteManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ACInDeliveryNoteManager> ACRefToServiceInstance(ACComponent requester)
        {
            ACInDeliveryNoteManager serviceInstance = GetServiceInstance(requester) as ACInDeliveryNoteManager;
            if (serviceInstance != null)
                return new ACRef<ACInDeliveryNoteManager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region Properties
        ACMethodBooking _BookParamInwardMovementClone;
        public ACMethodBooking BookParamInwardMovementClone(ACComponent facilityManager, DatabaseApp dbApp)
        {
            if (_BookParamInwardMovementClone != null)
                return _BookParamInwardMovementClone;
            _BookParamInwardMovementClone = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_InOrderPosInwardMovement.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            return _BookParamInwardMovementClone;
        }

        ACMethodBooking _BookParamInCancelClone;
        public ACMethodBooking BookParamInCancelClone(ACComponent facilityManager, DatabaseApp dbApp)
        {
            if (_BookParamInCancelClone != null)
                return _BookParamInCancelClone;
            _BookParamInCancelClone = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_InOrderPosCancel.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            return _BookParamInCancelClone;
        }

        private ACPropertyConfigValue<bool> _AutoInsertNewPreBooking;
        [ACPropertyConfig("en{'Automatic pre posting'}de{'Automatische Buchungsanlage'}")]
        public bool AutoInsertNewPreBooking
        {
            get
            {
                return _AutoInsertNewPreBooking.ValueT;
            }
            set
            {
                _AutoInsertNewPreBooking.ValueT = value;
            }
        }

        #endregion

        #region Public Methods

        #region Contract Assignment
        [ACMethodInfo("", "en{'Assign'}de{'Zuordnen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public virtual Msg AssignContractInOrderPos(InOrderPos currentInOrderPos, InOrder currentReleasingOrder, Nullable<double> enteredPartialQuantity, DatabaseApp dbApp, ACComponent facilityManager, List<object> resultNewEntities)
        {
            if (currentInOrderPos == null || currentReleasingOrder == null)
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
                    ACIdentifier = "AssignContractInOrderPos(0)",
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
                    ACIdentifier = "AssignContractInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }


            // 1. Erzeuge Unterposition
            currentInOrderPos.AutoRefresh(dbApp);
            InOrderPos childInOrderPos = InOrderPos.NewACObject(dbApp, currentInOrderPos, currentReleasingOrder);
            //childInOrderPos.InOrder = currentReleasingOrder;
            childInOrderPos.MaterialPosType = GlobalApp.MaterialPosTypes.InwardPart;
            var queryInOrderPosState = s_cQry_InOrderInProcess.Invoke(dbApp);
            if (!queryInOrderPosState.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignContractInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }
            childInOrderPos.MDInOrderPosState = queryInOrderPosState.FirstOrDefault();
            resultNewEntities.Add(childInOrderPos);

            // 2. Rufe Teilmenge ab
            double partialQuantityUOM = 0;
            if (enteredPartialQuantity.HasValue)
            {
                partialQuantityUOM = currentInOrderPos.Material.ConvertToBaseQuantity(enteredPartialQuantity.Value, currentInOrderPos.MDUnit);
                if ((currentInOrderPos.RemainingCallQuantityUOM + 0.000001) < partialQuantityUOM
                    || currentInOrderPos.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.CompletelyAssigned
                    || currentInOrderPos.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.Delivered)
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "AssignContractInOrderPos(0)",
                        Message = Root.Environment.TranslateMessage(this, "Warning50000", enteredPartialQuantity.Value, currentInOrderPos.MDUnit.MDUnitName, currentInOrderPos.RemainingCallQuantity, currentInOrderPos.MDUnit.MDUnitName)
                    };
                }
                currentInOrderPos.CalledUpQuantityUOM += partialQuantityUOM;
                if (Math.Abs(currentInOrderPos.RemainingCallQuantityUOM - 0) <= Double.Epsilon)
                    currentInOrderPos.MDDelivPosState = queryDelivStateAssigned.First();
                else
                    currentInOrderPos.MDDelivPosState = queryDelivStateSubsetAssigned.First();
            }
            else
            {
                partialQuantityUOM = currentInOrderPos.RemainingCallQuantityUOM;
                currentInOrderPos.CalledUpQuantityUOM = currentInOrderPos.TargetQuantityUOM;
                currentInOrderPos.MDDelivPosState = queryDelivStateAssigned.First();
            }
            childInOrderPos.TargetQuantityUOM = partialQuantityUOM;

            return null;
        }


        [ACMethodInfo("", "en{'Remove'}de{'Entfernen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public virtual Msg UnassignContractInOrderPos(InOrderPos currentReleasePos, DatabaseApp dbApp)
        {
            if (currentReleasePos == null || currentReleasePos.InOrderPos1_ParentInOrderPos == null)
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
                    ACIdentifier = "UnassignContractInOrderPos(0)",
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
                    ACIdentifier = "UnassignContractInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }

            if (currentReleasePos.FacilityBooking_InOrderPos.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignContractInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50010")
                };
            }

            // 1. Hole Parent-Bestellposition aus Bestellung un korrigiere Abgerufene teilmenge
            InOrderPos parentInOrderPos = currentReleasePos.InOrderPos1_ParentInOrderPos;
            parentInOrderPos.AutoRefresh(dbApp);
            currentReleasePos.AutoRefresh(dbApp);
            parentInOrderPos.CalledUpQuantityUOM -= currentReleasePos.TargetQuantityUOM;

            // 2. Lösche Unter-Bestellposition
            // aagincic note: Autoload of parentInOrder pos was changed state of InOrderPos from Deleted to Unmodified
            parentInOrderPos.InOrderPos_ParentInOrderPos.AutoLoad(parentInOrderPos.InOrderPos_ParentInOrderPosReference, parentInOrderPos);
            currentReleasePos.DeleteACObject(dbApp, false);

            // 3. Korrigiere Status der Parent-Bestellposition
            var queryChildsSubPos = parentInOrderPos.InOrderPos_ParentInOrderPos.Where(c => c.EntityState != EntityState.Deleted);
            if (!queryChildsSubPos.Any())
            {
                parentInOrderPos.MDDelivPosState = queryDelivStateNotPlanned.First();
                if (Math.Abs(parentInOrderPos.CalledUpQuantityUOM - 0) > Double.Epsilon)
                    parentInOrderPos.CalledUpQuantityUOM = 0;
            }
            else
            {
                parentInOrderPos.MDDelivPosState = queryDelivStateSubsetAssigned.First();
            }

            return null;
        }
        #endregion

        #region Deliverynote Assignment
        [ACMethodInfo("", "en{'Assign'}de{'Zuordnen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public virtual Msg AssignInOrderPos(InOrderPos currentInOrderPos, DeliveryNote currentDeliveryNote, Nullable<double> enteredPartialQuantity, DatabaseApp dbApp, ACComponent facilityManager, List<object> resultNewEntities)
        {
            if (!PreExecute("AssignInOrderPos"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50002")
                };
            }

            if (currentInOrderPos == null || currentDeliveryNote == null)
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
                    ACIdentifier = "AssignInOrderPos(0)",
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
                    ACIdentifier = "AssignInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }


            // 1. Erzeuge Unterposition
            currentInOrderPos.AutoRefresh(dbApp);
            InOrderPos childInOrderPos = InOrderPos.NewACObject(dbApp, currentInOrderPos);
            childInOrderPos.MaterialPosType = GlobalApp.MaterialPosTypes.InwardPart;
            var queryInOrderPosState = s_cQry_InOrderInProcess.Invoke(dbApp);
            if (!queryInOrderPosState.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }
            childInOrderPos.MDInOrderPosState = queryInOrderPosState.FirstOrDefault();
            resultNewEntities.Add(childInOrderPos);

            // 2. Rufe Teilmenge ab
            double partialQuantityUOM = 0;
            if (enteredPartialQuantity.HasValue)
            {
                partialQuantityUOM = currentInOrderPos.Material.ConvertToBaseQuantity(enteredPartialQuantity.Value, currentInOrderPos.MDUnit);
                if ((currentInOrderPos.RemainingCallQuantityUOM + 0.000001) < partialQuantityUOM
                    || currentInOrderPos.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.CompletelyAssigned
                    || currentInOrderPos.MDDelivPosState.DelivPosState == MDDelivPosState.DelivPosStates.Delivered)
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Info,
                        ACIdentifier = "AssignInOrderPos(0)",
                        Message = Root.Environment.TranslateMessage(this, "Warning50000", enteredPartialQuantity.Value, currentInOrderPos.MDUnit.MDUnitName, currentInOrderPos.RemainingCallQuantity, currentInOrderPos.MDUnit.MDUnitName)
                    };
                }
                currentInOrderPos.CalledUpQuantityUOM += partialQuantityUOM;
                if (Math.Abs(currentInOrderPos.RemainingCallQuantityUOM - 0) <= Double.Epsilon)
                    currentInOrderPos.MDDelivPosState = queryDelivStateAssigned.First();
                else
                    currentInOrderPos.MDDelivPosState = queryDelivStateSubsetAssigned.First();
            }
            else
            {
                partialQuantityUOM = currentInOrderPos.RemainingCallQuantityUOM;
                currentInOrderPos.CalledUpQuantityUOM = currentInOrderPos.TargetQuantityUOM;
                currentInOrderPos.MDDelivPosState = queryDelivStateAssigned.First();
            }
            childInOrderPos.TargetQuantityUOM = partialQuantityUOM;

            // 3. Erzeuge Lieferscheinposition und weise Unterposition zu
            Msg subMsg = NewDeliveryNotePos(childInOrderPos, currentDeliveryNote, dbApp, resultNewEntities);

            DeliveryNotePos newDeliveryNotePos = resultNewEntities.Where(c => c is DeliveryNotePos).FirstOrDefault() as DeliveryNotePos;
            if (newDeliveryNotePos == null)
                return subMsg;

            // 4. Erzeuge Buchungsparameter im Vorfeld
            if (facilityManager != null && AutoInsertNewPreBooking)
                NewFacilityPreBooking(facilityManager, dbApp, newDeliveryNotePos);

            PostExecute("AssignInOrderPos");
            return subMsg;
        }

        [ACMethodInfo("", "en{'New deliverynote line'}de{'Neue Lieferscheinposition'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public virtual Msg NewDeliveryNotePos(InOrderPos childInOrderPos, DeliveryNote currentDeliveryNote, DatabaseApp dbApp, List<object> resultNewEntities)
        {
            DeliveryNotePos newDeliveryNotePos = null;
            if (!PreExecute("NewDeliveryNotePos"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50002")
                };
            }

            if (childInOrderPos == null || currentDeliveryNote == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50009"));
            }

            newDeliveryNotePos = DeliveryNotePos.NewACObject(dbApp, currentDeliveryNote);
            newDeliveryNotePos.InOrderPos = childInOrderPos;
            newDeliveryNotePos.Comment = childInOrderPos.Comment;
            if (String.IsNullOrEmpty(currentDeliveryNote.Comment))
                currentDeliveryNote.Comment = childInOrderPos.InOrder.Comment;
            currentDeliveryNote.DeliveryNotePos_DeliveryNote.Add(newDeliveryNotePos);
            resultNewEntities.Add(newDeliveryNotePos);
            PostExecute("NewDeliveryNotePos");
            return null;
        }


        [ACMethodInfo("", "en{'Remove'}de{'Entfernen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public virtual Msg UnassignInOrderPos(DeliveryNotePos currentDeliveryNotePos, DatabaseApp dbApp)
        {
            if (!PreExecute("UnassignInOrderPos"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50002")
                };
            }

            if (currentDeliveryNotePos == null || currentDeliveryNotePos.InOrderPos == null || currentDeliveryNotePos.InOrderPos.InOrderPos1_ParentInOrderPos == null)
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
                    ACIdentifier = "UnassignInOrderPos(0)",
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
                    ACIdentifier = "UnassignInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50007")
                };
            }

            if (currentDeliveryNotePos.InOrderPos.FacilityBooking_InOrderPos.Any())
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignInOrderPos(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50010")
                };
            }

            // 1. Hole Parent-Bestellposition aus Bestellung un korrigiere Abgerufene teilmenge
            InOrderPos parentInOrderPos = currentDeliveryNotePos.InOrderPos.InOrderPos1_ParentInOrderPos;
            parentInOrderPos.AutoRefresh(dbApp);
            currentDeliveryNotePos.InOrderPos.AutoRefresh(dbApp);
            parentInOrderPos.CalledUpQuantityUOM -= currentDeliveryNotePos.InOrderPos.TargetQuantityUOM;

            // 2. Lösche Unter-Bestellposition
            // aagincic note: Autoload of parentInOrder pos was changed state of InOrderPos from Deleted to Unmodified
            parentInOrderPos.InOrderPos_ParentInOrderPos.AutoLoad(parentInOrderPos.InOrderPos_ParentInOrderPosReference, parentInOrderPos);
            currentDeliveryNotePos.InOrderPos.DeleteACObject(dbApp, false);

            // 3. Korrigiere Status der Parent-Bestellposition
            var queryChildsSubPos = parentInOrderPos.InOrderPos_ParentInOrderPos.Where(c => c.EntityState != EntityState.Deleted);
            if (!queryChildsSubPos.Any())
            {
                parentInOrderPos.MDDelivPosState = queryDelivStateNotPlanned.First();
                if (Math.Abs(parentInOrderPos.CalledUpQuantityUOM - 0) > Double.Epsilon)
                    parentInOrderPos.CalledUpQuantityUOM = 0;
            }
            else
            {
                parentInOrderPos.MDDelivPosState = queryDelivStateSubsetAssigned.First();
            }

            // Lösche Lieferscheinposition
            currentDeliveryNotePos.DeleteACObject(dbApp, true);

            PostExecute("UnassignInOrderPos");
            return null;
        }
        #endregion

        #region PreBooking
        public FacilityPreBooking NewFacilityPreBooking(ACComponent facilityManager, DatabaseApp dbApp, DeliveryNotePos deliveryNotePos)
        {
            ACMethodBooking acMethodClone = BookParamInwardMovementClone(facilityManager, dbApp);
            deliveryNotePos.InOrderPos.AutoRefresh(dbApp);
            string secondaryKey = Root.NoManager.GetNewNo(dbApp, typeof(FacilityPreBooking), FacilityPreBooking.NoColumnName, FacilityPreBooking.FormatNewNo, this);
            FacilityPreBooking facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, deliveryNotePos.InOrderPos, secondaryKey);
            ACMethodBooking acMethodBooking = acMethodClone.Clone() as ACMethodBooking;
            //acMethodBooking.InwardQuantity = deliveryNotePos.InOrderPos.TargetQuantityUOM;
            double quantityUOM = deliveryNotePos.InOrderPos.TargetQuantityUOM - deliveryNotePos.InOrderPos.PreBookingInwardQuantityUOM() - deliveryNotePos.InOrderPos.ActualQuantityUOM;
            if (deliveryNotePos.InOrderPos.MDUnit != null)
            {
                acMethodBooking.InwardQuantity = deliveryNotePos.InOrderPos.Material.ConvertQuantity(quantityUOM, deliveryNotePos.InOrderPos.Material.BaseMDUnit, deliveryNotePos.InOrderPos.MDUnit);
                acMethodBooking.MDUnit = deliveryNotePos.InOrderPos.MDUnit;
            }
            else
            {
                acMethodBooking.InwardQuantity = quantityUOM;
            }
            acMethodBooking.InwardMaterial = deliveryNotePos.InOrderPos.Material;
            acMethodBooking.InOrderPos = deliveryNotePos.InOrderPos;
            if (deliveryNotePos.InOrderPos.InOrder.CPartnerCompany != null)
                acMethodBooking.CPartnerCompany = deliveryNotePos.InOrderPos.InOrder.CPartnerCompany;
            // TODO: Restliche Parameter von acMethodBooking ausfüllen
            facilityPreBooking.ACMethodBooking = acMethodBooking;
            return facilityPreBooking;
        }

        public List<FacilityPreBooking> CancelFacilityPreBooking(ACComponent facilityManager, DatabaseApp dbApp, DeliveryNotePos deliveryNotePos)
        {
            List<FacilityPreBooking> bookings = new List<FacilityPreBooking>();
            ACMethodBooking acMethodClone = null;
            FacilityPreBooking facilityPreBooking = null;
            if (deliveryNotePos.InOrderPos == null || deliveryNotePos.InOrderPos.MDInOrderPosState.InOrderPosState == MDInOrderPosState.InOrderPosStates.Cancelled)
                return null;
            if (deliveryNotePos.InOrderPos.EntityState != EntityState.Added)
            {
                deliveryNotePos.InOrderPos.FacilityBooking_InOrderPos.AutoLoad(deliveryNotePos.InOrderPos.FacilityBooking_InOrderPosReference, deliveryNotePos);
                deliveryNotePos.InOrderPos.FacilityPreBooking_InOrderPos.AutoLoad(deliveryNotePos.InOrderPos.FacilityPreBooking_InOrderPosReference, deliveryNotePos);
            }
            else
                return null;
            if (deliveryNotePos.InOrderPos.FacilityPreBooking_InOrderPos.Any())
                return null;
            foreach (FacilityBooking previousBooking in deliveryNotePos.InOrderPos.FacilityBooking_InOrderPos)
            {
                if (previousBooking.FacilityBookingType != GlobalApp.FacilityBookingType.OutOrderPosOutwardMovement)
                    continue;
                // Wenn einmal Storniert, dann kann nicht mehr storniert werden. Der Fall dürfte normalerweise nicht auftreten, 
                // da der Positionsstatus auch MDOutOrderPosState.OutOrderPosStates.Cancelled sein müsste
                else if (previousBooking.FacilityBookingType == GlobalApp.FacilityBookingType.InOrderPosCancel)
                    return null;
            }

            foreach (FacilityBooking previousBooking in deliveryNotePos.InOrderPos.FacilityBooking_InOrderPos)
            {
                if (previousBooking.FacilityBookingType != GlobalApp.FacilityBookingType.InOrderPosInwardMovement)
                    continue;
                acMethodClone = BookParamInCancelClone(facilityManager, dbApp);
                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityPreBooking), FacilityPreBooking.NoColumnName, FacilityPreBooking.FormatNewNo, this);
                facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, deliveryNotePos.InOrderPos, secondaryKey);
                ACMethodBooking acMethodBooking = acMethodClone.Clone() as ACMethodBooking;
                acMethodBooking.InwardQuantity = previousBooking.InwardQuantity * -1;
                if (previousBooking.MDUnit != null)
                    acMethodBooking.MDUnit = previousBooking.MDUnit;
                acMethodBooking.InwardFacility = previousBooking.InwardFacility;
                if (previousBooking.InwardFacilityLot != null)
                    acMethodBooking.InwardFacilityLot = previousBooking.InwardFacilityLot;
                if (previousBooking.InwardFacilityCharge != null)
                    acMethodBooking.InwardFacilityCharge = previousBooking.InwardFacilityCharge;
                if (previousBooking.InwardMaterial != null)
                    acMethodBooking.InwardMaterial = previousBooking.InwardMaterial;
                else
                    acMethodBooking.InwardMaterial = deliveryNotePos.InOrderPos.Material;
                acMethodBooking.InOrderPos = deliveryNotePos.InOrderPos;
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
            if (deliveryNote.EntityState != EntityState.Added)
                deliveryNote.DeliveryNotePos_DeliveryNote.AutoLoad(deliveryNote.DeliveryNotePos_DeliveryNoteReference, deliveryNote);
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

        #region Complete Delivery Note

        public MsgWithDetails CompleteInDeliveryNote(DatabaseApp dbApp, DeliveryNote deliveryNote)
        {
            InOrder inOrder = deliveryNote.DeliveryNotePos_DeliveryNote.Select(c => c.InOrderPos.InOrder).FirstOrDefault();
            CompleteInDeliveryNote(dbApp, deliveryNote, inOrder);
            return dbApp.ACSaveChanges();
        }

        public void CompleteInDeliveryNote(DatabaseApp dbApp, DeliveryNote deliveryNote, InOrder inOrder)
        {
            deliveryNote.MDDelivNoteState = dbApp.MDDelivNoteState.FirstOrDefault(c => c.MDDelivNoteStateIndex == (short)MDDelivNoteState.DelivNoteStates.Completed);
            InOrderPos[] lines = inOrder.InOrderPos_InOrder.ToArray();
            FacilityPreBooking[] preBookings = lines.SelectMany(c => c.FacilityPreBooking_InOrderPos).ToArray();
            foreach (FacilityPreBooking preBooking in preBookings)
                preBooking.DeleteACObject(dbApp, false);
            MDInOrderPosState completedState = s_cQry_InOrderCompleted(dbApp).FirstOrDefault();
            foreach (InOrderPos line in lines)
                line.MDInOrderPosState = completedState;
            inOrder.MDInOrderState = dbApp.MDInOrderState.FirstOrDefault(c => c.MDInOrderStateIndex == (short)MDInOrderState.InOrderStates.Completed);
        }

        #endregion

        #endregion

    }
}


