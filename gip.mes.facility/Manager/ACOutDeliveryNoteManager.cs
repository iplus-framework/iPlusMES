using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Data.Objects;
using System.ComponentModel;

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

        /*
         
        Info50062	en{'Invoice {0} created!'}de{'Rechnung {0} gemacht!'}
        Info50065	en{'Sales Order {0} created!'}de{'Kundenauftrag {0} gemacht!'}

        */

        #region Invoice -> OutOrder


        [ACMethodInfo("", "en{'Assign'}de{'Zuordnen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg NewOutOrderFromOutOffer(DatabaseApp databaseApp, OutOffer outOffer)
        {
            Msg msg = null;
            try
            {
                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(OutOrder), OutOrder.NoColumnName, OutOrder.FormatNewNo, this);
                OutOrder outOrder = OutOrder.NewACObject(databaseApp, null, secondaryKey);
                outOrder.OutOrderDate = DateTime.Now;
                outOrder.BasedOnOutOffer = outOffer;
                outOrder.MDOutOrderType = outOffer.MDOutOrderType;
                outOrder.MDTermOfPayment = outOffer.MDTermOfPayment;
                outOrder.CustomerCompany = outOffer.CustomerCompany;
                outOrder.IssuerCompanyAddress = outOffer.IssuerCompanyAddress;
                outOrder.IssuerCompanyPerson = outOffer.IssuerCompanyPerson;
                outOrder.DeliveryCompanyAddress = outOffer.DeliveryCompanyAddress;
                outOrder.BillingCompanyAddress = outOffer.BillingCompanyAddress;
                outOrder.TargetDeliveryDate = outOffer.TargetDeliveryDate;
                outOrder.TargetDeliveryMaxDate = outOffer.TargetDeliveryMaxDate;
                outOrder.MDTimeRange = outOffer.MDTimeRange;
                outOrder.MDDelivType = outOffer.MDDelivType;
                outOrder.PriceNet = outOffer.PriceNet;
                outOrder.PriceGross = outOffer.PriceGross;
                outOrder.SalesTax = outOffer.SalesTax;
                outOrder.Comment = outOffer.Comment;
                outOrder.XMLDesignStart = outOffer.XMLDesignStart;
                outOrder.XMLDesignEnd = outOffer.XMLDesignEnd;

                Dictionary<Guid, Guid> connection = new Dictionary<Guid, Guid>();

                foreach (OutOfferPos outOfferPos in outOffer.OutOfferPos_OutOffer)
                {
                    OutOrderPos outOrderPos = OutOrderPos.NewACObject(databaseApp, outOrder);

                    connection.Add(outOfferPos.OutOfferPosID, outOrderPos.OutOrderPosID);

                    outOrderPos.MDTimeRange = outOfferPos.MDTimeRange;

                    outOrderPos.MaterialPosTypeIndex = outOfferPos.MaterialPosTypeIndex;
                    outOrderPos.MDCountrySalesTax = outOfferPos.MDCountrySalesTax;
                    outOrderPos.MDCountrySalesTaxMDMaterialGroup = outOfferPos.MDCountrySalesTaxMDMaterialGroup;
                    outOrderPos.MDCountrySalesTaxMaterial = outOfferPos.MDCountrySalesTaxMaterial;
                    outOrderPos.Sequence = outOfferPos.Sequence;
                    outOrderPos.Material = outOfferPos.Material;
                    outOrderPos.MDUnit = outOfferPos.MDUnit;
                    outOrderPos.TargetQuantityUOM = outOfferPos.TargetQuantityUOM;
                    outOrderPos.TargetQuantity = outOfferPos.TargetQuantity;
                    outOrderPos.PriceNet = outOfferPos.PriceNet;
                    outOrderPos.PriceGross = outOfferPos.PriceGross;
                    outOrderPos.SalesTax = outOfferPos.SalesTax;


                    outOrderPos.GroupSum = outOfferPos.GroupSum;
                    outOrderPos.TargetDeliveryDate = outOfferPos.TargetDeliveryDate;
                    outOrderPos.TargetDeliveryMaxDate = outOfferPos.TargetDeliveryMaxDate;
                    outOrderPos.TargetDeliveryPriority = outOfferPos.TargetDeliveryPriority;

                    outOrderPos.Comment = outOfferPos.Comment;
                    outOrderPos.Comment2 = outOfferPos.Comment2;
                    outOrderPos.XMLDesign = outOfferPos.XMLDesign;

                    outOrder.OutOrderPos_OutOrder.Add(outOrderPos);
                }

                // Update parents
                foreach (OutOfferPos outOfferPos in outOffer.OutOfferPos_OutOffer)
                {
                    if (outOfferPos.ParentOutOfferPosID != null)
                    {
                        Guid outOrderPosID = connection[outOffer.OutOfferID];
                        OutOrderPos relatedPos = outOrder.OutOrderPos_OutOrder.Where(c => c.OutOrderID == outOrderPosID).FirstOrDefault();
                        Guid parentOutOrderPosID = connection[outOfferPos.ParentOutOfferPosID ?? Guid.Empty];
                        OutOrderPos parentRelatedPos = outOrder.OutOrderPos_OutOrder.Where(c => c.OutOrderID == parentOutOrderPosID).FirstOrDefault();
                        relatedPos.OutOrderPos1_ParentOutOrderPos = parentRelatedPos;
                    }
                }

                databaseApp.OutOrder.AddObject(outOrder);
                databaseApp.SaveChanges();

                msg = new Msg() { MessageLevel = eMsgLevel.Info, Message = Root.Environment.TranslateMessage(this, "Info50065", outOrder.OutOrderNo) };
            }
            catch (Exception ec)
            {
                msg = new Msg() { MessageLevel = eMsgLevel.Exception, Message = ec.Message };
            }

            return msg;
        }

        #endregion

        #region Invoice -> Invoice

        [ACMethodInfo("", "en{'NewInvoiceFromOutDeliveryNote'}de{'NewInvoiceFromOutDeliveryNote'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg NewInvoiceFromOutDeliveryNote(DatabaseApp databaseApp, DeliveryNote deliveryNote)
        {
            Msg msg = null;
            if (!PreExecute("NewInvoiceFromOutDeliveryNote"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "NewInvoiceFromOutDeliveryNote(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50002")
                };
            }
            try
            {
                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Invoice), Invoice.NoColumnName, Invoice.FormatNewNo, this);
                Invoice invoice = Invoice.NewACObject(databaseApp, null, secondaryKey);

                // Selected first outorder but can be many
                OutOrder outOrder =
                    deliveryNote
                    .DeliveryNotePos_DeliveryNote
                    .OrderBy(c => c.Sequence)
                    .FirstOrDefault()
                    .OutOrderPos
                    .OutOrder;
                invoice.OutOrder = outOrder;

                invoice.MDInvoiceState = MDInvoiceState.DefaultMDInvoiceState(databaseApp);
                invoice.MDInvoiceType = MDInvoiceType.DefaultMDInvoiceType(databaseApp);
                invoice.InvoiceDate = DateTime.Now;
                invoice.CustomerCompany = outOrder.CustomerCompany;
                invoice.BillingCompanyAddress = outOrder.BillingCompanyAddress;
                invoice.DeliveryCompanyAddress = outOrder.DeliveryCompanyAddress;

                List<OutOrderPos> items =
                    deliveryNote
                    .DeliveryNotePos_DeliveryNote
                    .OrderBy(c => c.Sequence)
                    .Select(c => c.OutOrderPos)
                    .ToList();

                int nr = 0;
                foreach (OutOrderPos outOrderPos in items)
                {
                    nr++;
                    InvoicePos invoicePos = GetInvoicePos(databaseApp, invoice, nr, outOrderPos);
                    invoice.InvoicePos_Invoice.Add(invoicePos);
                }
                databaseApp.SaveChanges();
                msg = new Msg() { MessageLevel = eMsgLevel.Info, Message = Root.Environment.TranslateMessage(this, "Info50062", invoice.InvoiceNo) };
            }
            catch (Exception ec)
            {
                msg = new Msg() { MessageLevel = eMsgLevel.Exception, Message = ec.Message };
            }
            return msg;
        }

        [ACMethodInfo("", "en{'NewInvoiceFromOutOrder'}de{'NewInvoiceFromOutOrder'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg NewInvoiceFromOutOrder(DatabaseApp databaseApp, OutOrder outOrder)
        {
            Msg msg = null;
            if (!PreExecute("NewInvoiceFromOutOrder"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "NewInvoiceFromOutOrder(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50002")
                };
            }
            try
            {
                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Invoice), Invoice.NoColumnName, Invoice.FormatNewNo, this);
                Invoice invoice = Invoice.NewACObject(databaseApp, null, secondaryKey);
                invoice.OutOrder = outOrder;
                invoice.MDInvoiceState = MDInvoiceState.DefaultMDInvoiceState(databaseApp);
                invoice.MDInvoiceType = MDInvoiceType.DefaultMDInvoiceType(databaseApp);
                invoice.InvoiceDate = DateTime.Now;
                invoice.CustomerCompany = outOrder.CustomerCompany;
                invoice.BillingCompanyAddress = outOrder.BillingCompanyAddress;
                invoice.DeliveryCompanyAddress = outOrder.DeliveryCompanyAddress;
                invoice.IssuerCompanyAddress = outOrder.IssuerCompanyAddress;
                invoice.IssuerCompanyPerson = outOrder.IssuerCompanyPerson;
                invoice.MDTermOfPayment = outOrder.MDTermOfPayment;
                invoice.XMLDesignStart = outOrder.XMLDesignStart;
                invoice.XMLDesignEnd = outOrder.XMLDesignEnd;

                List<OutOrderPos> items =
                    outOrder
                    .OutOrderPos_OutOrder
                    .Where(c => c.ParentOutOrderPosID == null)
                    .OrderBy(c => c.Sequence)
                    .ToList();

                int nr = 0;
                foreach (OutOrderPos outOrderPos in items)
                {
                    nr++;
                    InvoicePos invoicePos = GetInvoicePos(databaseApp, invoice, nr, outOrderPos);
                    invoice.InvoicePos_Invoice.Add(invoicePos);
                }
                databaseApp.SaveChanges();
            }
            catch (Exception ec)
            {
                msg = new Msg() { MessageLevel = eMsgLevel.Exception, Message = ec.Message };
            }

            PostExecute("GenerateInvoice");
            return msg;
        }

        #endregion

        #region Invoice -> IOutOrder & IOutOrderPosBSO

        [ACMethodInfo("", "en{'HanldeIOrderPosPropertyChange'}de{'HanldeIOrderPosPropertyChange'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public void HandleIOrderPropertyChange(string propertyName, IOutOrder item)
        {
            if (item != null)
            {
                switch (propertyName)
                {
                    case "CustomerCompanyID":
                        if (item.CustomerCompany != null)
                        {
                            item.BillingCompanyAddress = item.CustomerCompany.BillingCompanyAddress;
                            item.DeliveryCompanyAddress = item.CustomerCompany.DeliveryCompanyAddress;
                        }
                        else
                        {
                            item.BillingCompanyAddress = null;
                            item.DeliveryCompanyAddress = null;
                        }
                        break;
                    
                }
            }

        }

        [ACMethodInfo("", "en{'HanldeIOrderPosPropertyChange'}de{'HanldeIOrderPosPropertyChange'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public void HandleIOrderPosPropertyChange(DatabaseApp databaseApp, IOutOrderPosBSO callerObject, string propertyName,
            IOutOrder outOrder, IOutOrderPos posItem, List<IOutOrderPos> posItems, CompanyAddress billingCompanyAddress)
        {
            if (posItem != null && !posItem.InRecalculation)
            {
                posItem.InRecalculation = true;
                switch (propertyName)
                {
                    case "MaterialID":
                        {
                            callerObject.OnPropertyChanged("MDUnitList");
                            //if (posItem.Material != null && posItem.Material.BaseMDUnit != null)
                            //    callerObject.CurrentMDUnit = posItem.Material.BaseMDUnit;
                            //else
                            //    callerObject.CurrentMDUnit = null;

                            posItem.MDUnit = posItem.Material?.BaseMDUnit;
                            posItem.OnEntityPropertyChanged("MDUnit");

                            callerObject.OnPropertyChanged("CurrentOutOfferPos");

                            if (posItem.Material != null)
                            {

                                // TODO: scenario if past document is updated: Valid time is document time
                                DateTime now = DateTime.Now;

                                callerObject.SelectedPriceListMaterial = null;
                                callerObject.PriceListMaterialItems = databaseApp.PriceListMaterial.Where(c => c.MaterialID == posItem.Material.MaterialID && c.PriceList.DateFrom < now
                                                                                             && (!c.PriceList.DateTo.HasValue || c.PriceList.DateTo > now)).ToList();

                                if (billingCompanyAddress != null)
                                {

                                    ActualSalesTax countrySalesTaxModel = GetActualSalesTax(billingCompanyAddress.MDCountry, posItem.Material, now);

                                    if (countrySalesTaxModel.MDCountrySalesTaxMaterial != null)
                                    {
                                        posItem.SalesTax = countrySalesTaxModel.MDCountrySalesTaxMaterial.SalesTax;
                                        posItem.MDCountrySalesTaxMaterial = countrySalesTaxModel.MDCountrySalesTaxMaterial;
                                    }
                                    else if (countrySalesTaxModel.MDCountrySalesTaxMDMaterialGroup != null)
                                    {
                                        posItem.SalesTax = countrySalesTaxModel.MDCountrySalesTaxMDMaterialGroup.SalesTax;
                                        posItem.MDCountrySalesTaxMDMaterialGroup = countrySalesTaxModel.MDCountrySalesTaxMDMaterialGroup;
                                    }
                                    else if (countrySalesTaxModel.MDCountrySalesTax != null)
                                    {
                                        posItem.SalesTax = countrySalesTaxModel.MDCountrySalesTax.SalesTax;
                                        posItem.MDCountrySalesTax = countrySalesTaxModel.MDCountrySalesTax;
                                    }

                                    if (posItem.SalesTax > 0)
                                        posItem.PriceGross = posItem.PriceNet + (decimal)posItem.SalesTaxAmount;
                                }
                            }
                        }
                        break;
                    case "TargetQuantityUOM":
                    case "MDUnitID":
                        {
                            if (posItem.Material != null && posItem.MDUnit != null)
                            {
                                posItem.TargetQuantity = posItem.Material.ConvertToBaseQuantity(posItem.TargetQuantityUOM, posItem.MDUnit);
                                //CurrentOutOfferPos.TargetWeight = CurrentOutOfferPos.Material.ConvertToBaseWeight(CurrentOutOfferPos.TargetQuantityUOM, CurrentOutOfferPos.MDUnit);
                            }

                            posItem.OnEntityPropertyChanged("TotalPrice");
                            posItem.OnEntityPropertyChanged("TotalSalesTax");
                            posItem.OnEntityPropertyChanged("TotalPriceWithTax");
                            callerObject.OnPricePropertyChanged();
                            CalculateTaxOverview(callerObject, outOrder, posItems);
                        }
                        break;
                    case "PriceNet":
                        posItem.OnEntityPropertyChanged("SalesTaxAmount");
                        posItem.PriceGross = posItem.PriceNet + (decimal)posItem.SalesTaxAmount;
                        callerObject.OnPricePropertyChanged();
                        CalculateTaxOverview(callerObject, outOrder, posItems);
                        posItem.OnEntityPropertyChanged("TotalPrice");
                        posItem.OnEntityPropertyChanged("TotalSalesTax");
                        posItem.OnEntityPropertyChanged("TotalPriceWithTax");
                        break;
                    case "PriceGross":
                        if (posItem.PriceGross < 0)
                            posItem.SalesTax = 0;

                        posItem.PriceNet = posItem.PriceGross - (decimal)posItem.SalesTaxAmount;
                        posItem.OnEntityPropertyChanged("SalesTaxAmount");
                        callerObject.OnPricePropertyChanged();

                        CalculateTaxOverview(callerObject, outOrder, posItems);

                        posItem.OnEntityPropertyChanged("TotalPrice");
                        posItem.OnEntityPropertyChanged("TotalSalesTax");
                        posItem.OnEntityPropertyChanged("TotalPriceWithTax");
                        break;
                    case "SalesTax":
                        posItem.OnEntityPropertyChanged("SalesTaxAmount");
                        posItem.PriceGross = posItem.PriceNet + (decimal)posItem.SalesTaxAmount;
                        // clean up tax connections
                        posItem.MDCountrySalesTaxMaterial = null;
                        posItem.MDCountrySalesTaxMDMaterialGroup = null;
                        posItem.MDCountrySalesTax = null;
                        if (billingCompanyAddress != null)
                        {
                            DateTime now = DateTime.Now;
                            ActualSalesTax countrySalesTaxModel = GetActualSalesTax(billingCompanyAddress.MDCountry, posItem.Material, now);
                            if (countrySalesTaxModel.MDCountrySalesTaxMaterial != null && countrySalesTaxModel.MDCountrySalesTaxMaterial.SalesTax == posItem.SalesTax)
                                posItem.MDCountrySalesTaxMaterial = countrySalesTaxModel.MDCountrySalesTaxMaterial;
                            else if (countrySalesTaxModel.MDCountrySalesTaxMDMaterialGroup != null && countrySalesTaxModel.MDCountrySalesTaxMDMaterialGroup.SalesTax == posItem.SalesTax)
                                posItem.MDCountrySalesTaxMDMaterialGroup = countrySalesTaxModel.MDCountrySalesTaxMDMaterialGroup;
                            else if (countrySalesTaxModel.MDCountrySalesTax != null && countrySalesTaxModel.MDCountrySalesTax.SalesTax == posItem.SalesTax)
                                posItem.MDCountrySalesTax = countrySalesTaxModel.MDCountrySalesTax;
                        }
                        callerObject.OnPricePropertyChanged();
                        CalculateTaxOverview(callerObject, outOrder, posItems);
                        posItem.OnEntityPropertyChanged("TotalPrice");
                        posItem.OnEntityPropertyChanged("TotalSalesTax");
                        posItem.OnEntityPropertyChanged("TotalPriceWithTax");
                        break;
                }
                posItem.InRecalculation = false;
            }

        }

        public void CalculateTaxOverview(IOutOrderPosBSO callerObject, IOutOrder outOrder, List<IOutOrderPos> posItems)
        {
            string documentLangCode = GetDocumentLangCode(outOrder);
            string langCode = !string.IsNullOrEmpty(documentLangCode) ? documentLangCode : Root.CurrentInvokingUser.VBLanguage.VBLanguageCode;
            List<MDCountrySalesTax> mdCountrySalesTax = GetTaxOverviewList(langCode, outOrder, posItems);
            callerObject.TaxOverviewList = mdCountrySalesTax;
            decimal totalTax = mdCountrySalesTax.Sum(c => c.SalesTax);
            outOrder.PriceNet = (decimal)outOrder.PosPriceNetTotal;
            outOrder.PriceGross = ((decimal)outOrder.PosPriceNetTotal + totalTax);
        }

        #endregion

        #region Invoice -> Private

        private ActualSalesTax GetActualSalesTax(MDCountry country, Material material, DateTime dateTime)
        {
            ActualSalesTax model = new ActualSalesTax();
            model.MDCountrySalesTax =
                   country
                   .MDCountrySalesTax_MDCountry
                   .FirstOrDefault(c =>
                           c.MDCountryID == country.MDCountryID
                           && c.DateFrom < dateTime
                           && (!c.DateTo.HasValue || c.DateTo > dateTime)
                   );

            model.MDCountrySalesTaxMDMaterialGroup =
                material
                .MDMaterialGroup
                .MDCountrySalesTaxMDMaterialGroup_MDMaterialGroup
                .FirstOrDefault(c =>
                        c.MDCountrySalesTax.MDCountryID == country.MDCountryID
                        && c.MDCountrySalesTax.DateFrom < dateTime
                        && (!c.MDCountrySalesTax.DateTo.HasValue || c.MDCountrySalesTax.DateTo > dateTime)
                );

            model.MDCountrySalesTaxMaterial =
                material
                .MDCountrySalesTaxMaterial_Material
                .FirstOrDefault(c =>
                        c.MDCountrySalesTax.MDCountryID == country.MDCountryID
                        && c.MDCountrySalesTax.DateFrom < dateTime
                        && (!c.MDCountrySalesTax.DateTo.HasValue || c.MDCountrySalesTax.DateTo > dateTime)
                );
            return model;
        }

        private InvoicePos GetInvoicePos(DatabaseApp databaseApp, Invoice invoice, int nr, OutOrderPos outOrderPos)
        {
            InvoicePos invoicePos = InvoicePos.NewACObject(databaseApp, invoice);
            invoicePos.Sequence = nr;

            invoicePos.Material = outOrderPos.Material;
            invoicePos.MDUnit = outOrderPos.MDUnit;
            invoicePos.TargetQuantityUOM = outOrderPos.TargetQuantityUOM;
            invoicePos.TargetQuantity = invoicePos.MDUnit != null ?
                invoicePos.MDUnit.ConvertToUnit(invoicePos.TargetQuantityUOM, invoicePos.Material.BaseMDUnit)
                : invoicePos.TargetQuantityUOM;

            invoicePos.PriceNet = outOrderPos.PriceNet;
            invoicePos.PriceGross = outOrderPos.PriceGross;
            invoicePos.SalesTax = outOrderPos.SalesTax;

            invoicePos.MDCountrySalesTax = outOrderPos.MDCountrySalesTax;
            invoicePos.MDCountrySalesTaxMDMaterialGroup = outOrderPos.MDCountrySalesTaxMDMaterialGroup;
            invoicePos.MDCountrySalesTaxMaterial = outOrderPos.MDCountrySalesTaxMaterial;

            OutOrderPos topPos = outOrderPos.TopParentOutOrderPos;
            OutOrderPos invoicePosRelation =
                OutOrderPos.NewACObject(databaseApp, topPos);

            invoicePosRelation.TargetQuantityUOM = outOrderPos.TargetQuantityUOM;
            invoicePosRelation.TargetQuantity = outOrderPos.TargetQuantity;
            invoicePosRelation.ActualQuantityUOM = outOrderPos.ActualQuantityUOM;
            invoicePosRelation.ActualQuantity = outOrderPos.ActualQuantity;
            invoicePosRelation.CalledUpQuantityUOM = outOrderPos.CalledUpQuantityUOM;
            invoicePosRelation.CalledUpQuantity = outOrderPos.CalledUpQuantity;
            invoicePosRelation.ExternQuantityUOM = outOrderPos.ExternQuantityUOM;
            invoicePosRelation.ExternQuantity = outOrderPos.ExternQuantity;

            invoicePos.OutOrderPos = invoicePosRelation;
            invoicePos.XMLDesign = outOrderPos.XMLDesign;
            return invoicePos;
        }

        private List<MDCountrySalesTax> GetTaxOverviewList(string langCode, IOutOrder outOrder, List<IOutOrderPos> poses)
        {
            List<MDCountrySalesTax> mDCountrySalesTaxes = new List<MDCountrySalesTax>();
            string vatFormat = Root.Environment.TranslateMessageLC(this, "Info50066", langCode);
            //string vatFormat = "VAT with {0} %";
            //if (billingcompanyAddress?.MDCountry?.MDCountryName == "DE")
            //    vatFormat = "MwSt. mit {0} %";

            if (outOrder.PosPriceNetDiscount < 0)
            {
                var percent = (decimal)((Math.Abs(outOrder.PosPriceNetDiscount) / outOrder.PosPriceNetSum) * 100);

                mDCountrySalesTaxes = poses
                                        .Where(c => c.PriceNet > 0)
                                        .Select(x => new Tuple<decimal, decimal>(x.SalesTax, (x.PriceNet - (x.PriceNet * (percent / 100))) * (decimal)x.TargetQuantity * (x.SalesTax / 100)))
                                        .GroupBy(t => t.Item1)
                                        .Select(o => new MDCountrySalesTax()
                                        {
                                            MDKey = string.Format(vatFormat, o.Key.ToString("N2")),
                                            SalesTax = o.Sum(s => s.Item2)
                                        })
                                        .ToList();
            }
            else
            {
                mDCountrySalesTaxes = poses
                                        .Where(c => c.SalesTax > 0 && c.SalesTaxAmount > 0)
                                        .GroupBy(g => g.SalesTax)
                                        .Select(o => new MDCountrySalesTax()
                                        {
                                            MDKey = string.Format(vatFormat, o.Key.ToString("N2")),
                                            SalesTax = (decimal)o.Sum(s => s.TotalSalesTax)
                                        })
                                        .ToList();
            }
            return mDCountrySalesTaxes;
        }

        private string GetDocumentLangCode(IOutOrder outOrder)
        {
            if (outOrder.BillingCompanyAddress == null)
                return null;
            string langCode = "en";
            string countryMDKey = outOrder.BillingCompanyAddress.MDCountry.MDKey;
            switch (countryMDKey)
            {
                case "HR":
                    langCode = "hr";
                    break;
                case "D":
                    langCode = "de";
                    break;
            }
            return langCode;
        }

        #endregion

        #endregion

        #endregion

    }
}


