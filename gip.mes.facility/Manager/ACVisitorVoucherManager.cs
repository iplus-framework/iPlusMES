// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioLogistics, "", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public partial class ACVisitorVoucherManager : PARole
    {
        #region c´tors
        public ACVisitorVoucherManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _DeliveryNoteAutoCreatePerOrder = new ACPropertyConfigValue<bool>(this, nameof(DeliveryNoteAutoCreatePerOrder), true);
            _ClosePickingOnCheckOut = new ACPropertyConfigValue<bool>(this, nameof(ClosePickingOnCheckOut), true);
        }
        #endregion

        #region Properties

        #region Config
        private ACPropertyConfigValue<bool> _DeliveryNoteAutoCreatePerOrder;
        [ACPropertyConfig("en{'Deliverynotecreation per Supplier and order'}de{'Lieferscheinerzeugung pro Lieferant und Bestellung'}", DefaultValue = true)]
        public bool DeliveryNoteAutoCreatePerOrder
        {
            get
            {
                return _DeliveryNoteAutoCreatePerOrder.ValueT;
            }
            set
            {
                _DeliveryNoteAutoCreatePerOrder.ValueT = value;
            }
        }

        private ACPropertyConfigValue<bool> _ClosePickingOnCheckOut;
        [ACPropertyConfig("en{'Close Picking Order on Checkout'}de{'Kommissionieraufträge beenden bei Checkout'}", DefaultValue = true)]
        public bool ClosePickingOnCheckOut
        {
            get
            {
                return _ClosePickingOnCheckOut.ValueT;
            }
            set
            {
                _ClosePickingOnCheckOut.ValueT = value;
            }
        }
        #endregion

        #endregion

        #region static Methods
        public const string C_DefaultServiceACIdentifier = "VisitorVoucherManager";

        public static ACVisitorVoucherManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACVisitorVoucherManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ACVisitorVoucherManager> ACRefToServiceInstance(ACComponent requester)
        {
            ACVisitorVoucherManager serviceInstance = GetServiceInstance(requester) as ACVisitorVoucherManager;
            if (serviceInstance != null)
                return new ACRef<ACVisitorVoucherManager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region Public Methods

        #region Checkout
        public virtual MsgWithDetails CheckOut(VisitorVoucher currentVisitorVoucher, DatabaseApp dbApp, ACPickingManager pickingManager, ACInDeliveryNoteManager inDeliveryNoteManager, ACOutDeliveryNoteManager outDeliveryNoteManager, FacilityManager facilityManager)
        {
            MsgWithDetails msgWithDetails = null;
            MDVisitorVoucherState state = dbApp.MDVisitorVoucherState.Where(c => c.MDVisitorVoucherStateIndex == (short)MDVisitorVoucherState.VisitorVoucherStates.CheckedOut).FirstOrDefault();
            if (state != null)
            {
                currentVisitorVoucher.MDVisitorVoucherState = state;
                currentVisitorVoucher.CheckOutDate = DateTime.Now;
            }
            if (ClosePickingOnCheckOut && pickingManager != null)
            {
                foreach (Picking picking in currentVisitorVoucher.Picking_VisitorVoucher.ToArray())
                {
                    if (picking == null || picking.PickingState == PickingStateEnum.Finished || picking.PickingState == PickingStateEnum.Cancelled)
                        continue;
                    MsgWithDetails msgWithDetails2 = pickingManager.FinishOrder(dbApp, picking, inDeliveryNoteManager, outDeliveryNoteManager, facilityManager, true);
                    if (msgWithDetails == null)
                        msgWithDetails = msgWithDetails2;
                    else
                        msgWithDetails.Append(msgWithDetails2);
                }
            }
            return msgWithDetails;
        }
        #endregion

        #region Deliverynote

        [ACMethodInfo("", "en{'Assign Delivery Note'}de{'Lieferschein zuordnen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg AssignDeliveryNote(VisitorVoucher currentVisitorVoucher, DeliveryNote currentDeliveryNote, DatabaseApp dbApp)
        {
            if (!PreExecute("AssignDeliveryNote"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignDeliveryNote(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50008")
                };
            }

            if (currentVisitorVoucher == null || currentDeliveryNote == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50017"));
            }

            currentDeliveryNote.VisitorVoucher = currentVisitorVoucher;

            // Durchsuche InOrder-Positionen nach Unterpositionen, die einer Kommissionierliste zugeordnet sind
            // Falls Kommissionierliste ein LKW-Entladungsplan ist, dann weise Komissionierliste der Hofliste zu
            currentDeliveryNote.DeliveryNotePos_DeliveryNote.AutoLoad(currentDeliveryNote.DeliveryNotePos_DeliveryNoteReference, currentDeliveryNote);
            var queryInOrderPos2Level = currentDeliveryNote.DeliveryNotePos_DeliveryNote.Where(c => c.InOrderPos != null && c.InOrderPos.InOrderPos_ParentInOrderPos.Any()).Select(c => c.InOrderPos);
            foreach (InOrderPos inOrderPos2Level in queryInOrderPos2Level)
            {
                // Schaue in dritter Ebene nach
                inOrderPos2Level.InOrderPos_ParentInOrderPos.AutoLoad(inOrderPos2Level.InOrderPos_ParentInOrderPosReference, inOrderPos2Level);
                foreach (InOrderPos inOrderPos3Level in inOrderPos2Level.InOrderPos_ParentInOrderPos)
                {
                    inOrderPos3Level.PickingPos_InOrderPos.AutoLoad(inOrderPos3Level.PickingPos_InOrderPosReference, inOrderPos3Level);
                    IEnumerable<Picking> pickingList = inOrderPos3Level.PickingPos_InOrderPos.Where(c => c.Picking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.ReceiptVehicle
                                                                                                      && (c.Picking.VisitorVoucher == null || c.Picking.VisitorVoucher != currentVisitorVoucher))
                                                                                                      .Select(c => c.Picking).Distinct();
                    foreach (Picking picking in pickingList)
                    {
                        picking.VisitorVoucher = currentVisitorVoucher;
                    }
                }
            }

            // Durchsuche OutOrder-Positionen nach Unterpositionen, die einer Kommissionierliste zugeordnet sind
            // Falls Kommissionierliste ein LKW-Verladeplan ist, dann weise Komissionierliste der Hofliste zu
            var queryOutOrderPos2Level = currentDeliveryNote.DeliveryNotePos_DeliveryNote.Where(c => c.OutOrderPos != null && c.OutOrderPos.OutOrderPos_ParentOutOrderPos.Any()).Select(c => c.OutOrderPos);
            foreach (OutOrderPos outOrderPos2Level in queryOutOrderPos2Level)
            {
                // Schaue in dritter Ebene nach
                outOrderPos2Level.OutOrderPos_ParentOutOrderPos.AutoLoad(outOrderPos2Level.OutOrderPos_ParentOutOrderPosReference, outOrderPos2Level);
                foreach (OutOrderPos outOrderPos3Level in outOrderPos2Level.OutOrderPos_ParentOutOrderPos)
                {
                    outOrderPos3Level.PickingPos_OutOrderPos.AutoLoad(outOrderPos3Level.PickingPos_OutOrderPosReference, outOrderPos3Level);
                    IEnumerable<Picking> pickingList = outOrderPos3Level.PickingPos_OutOrderPos.Where(c => c.Picking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.IssueVehicle
                                                                                                      && (c.Picking.VisitorVoucher == null || c.Picking.VisitorVoucher != currentVisitorVoucher))
                                                                                                      .Select(c => c.Picking).Distinct();
                    foreach (Picking picking in pickingList)
                    {
                        picking.VisitorVoucher = currentVisitorVoucher;
                    }
                }
            }

            PostExecute("AssignDeliveryNote");
            return null;
        }

        [ACMethodInfo("", "en{'Remove Delivery Note'}de{'Lieferschein entfernen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg UnassignDeliveryNote(VisitorVoucher currentVisitorVoucher, DeliveryNote currentDeliveryNote, DatabaseApp dbApp)
        {
            if (!PreExecute("UnassignDeliveryNote"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignDeliveryNote(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50008")
                };
            }

            if (currentDeliveryNote == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50018"));
            }

            currentDeliveryNote.VisitorVoucher = null;

            // Durchsuche InOrder-Positionen nach Unterpositionen, die einer Kommissionierliste zugeordnet sind
            // Falls Kommissionierliste ein LKW-Entladungsplan ist, dann weise Komissionierliste der Hofliste zu
            currentDeliveryNote.DeliveryNotePos_DeliveryNote.AutoLoad(currentDeliveryNote.DeliveryNotePos_DeliveryNoteReference, currentDeliveryNote);
            var queryInOrderPos2Level = currentDeliveryNote.DeliveryNotePos_DeliveryNote.Where(c => c.InOrderPos != null && c.InOrderPos.InOrderPos_ParentInOrderPos.Any()).Select(c => c.InOrderPos);
            foreach (InOrderPos inOrderPos2Level in queryInOrderPos2Level)
            {
                // Schaue in dritter Ebene nach
                inOrderPos2Level.InOrderPos_ParentInOrderPos.AutoLoad(inOrderPos2Level.InOrderPos_ParentInOrderPosReference, inOrderPos2Level);
                foreach (InOrderPos inOrderPos3Level in inOrderPos2Level.InOrderPos_ParentInOrderPos)
                {
                    inOrderPos3Level.PickingPos_InOrderPos.AutoLoad(inOrderPos3Level.PickingPos_InOrderPosReference, inOrderPos3Level);
                    IEnumerable<Picking> pickingList = inOrderPos3Level.PickingPos_InOrderPos.Where(c => c.Picking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.ReceiptVehicle
                                                                                                      && (c.Picking.VisitorVoucher != null && c.Picking.VisitorVoucher == currentVisitorVoucher))
                                                                                                      .Select(c => c.Picking).Distinct();
                    foreach (Picking picking in pickingList)
                    {
                        picking.VisitorVoucher = null;
                    }
                }
            }

            // Durchsuche OutOrder-Positionen nach Unterpositionen, die einer Kommissionierliste zugeordnet sind
            // Falls Kommissionierliste ein LKW-Verladeplan ist, dann weise Komissionierliste der Hofliste zu
            var queryOutOrderPos2Level = currentDeliveryNote.DeliveryNotePos_DeliveryNote.Where(c => c.OutOrderPos != null && c.OutOrderPos.OutOrderPos_ParentOutOrderPos.Any()).Select(c => c.OutOrderPos);
            foreach (OutOrderPos outOrderPos2Level in queryOutOrderPos2Level)
            {
                // Schaue in dritter Ebene nach
                outOrderPos2Level.OutOrderPos_ParentOutOrderPos.AutoLoad(outOrderPos2Level.OutOrderPos_ParentOutOrderPosReference, outOrderPos2Level);
                foreach (OutOrderPos outOrderPos3Level in outOrderPos2Level.OutOrderPos_ParentOutOrderPos)
                {
                    outOrderPos3Level.PickingPos_OutOrderPos.AutoLoad(outOrderPos3Level.PickingPos_OutOrderPosReference, outOrderPos3Level);
                    IEnumerable<Picking> pickingList = outOrderPos3Level.PickingPos_OutOrderPos.Where(c => c.Picking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.IssueVehicle
                                                                                                      && (c.Picking.VisitorVoucher != null && c.Picking.VisitorVoucher == currentVisitorVoucher))
                                                                                                      .Select(c => c.Picking).Distinct();
                    foreach (Picking picking in pickingList)
                    {
                        picking.VisitorVoucher = null;
                    }
                }
            }

            PostExecute("UnassignDeliveryNote");
            return null;
        }

        #endregion

        #region Tourplan

        [ACMethodInfo("", "en{'Assign Tourplan'}de{'Tourenplan zuordnen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg AssignTourplan(VisitorVoucher currentVisitorVoucher, Tourplan currentTourplan, DatabaseApp dbApp)
        {
            if (!PreExecute("AssignTourplan"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignTourplan(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50008")
                };
            }

            if (currentVisitorVoucher == null || currentTourplan == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50019"));
            }

            currentTourplan.VisitorVoucher = currentVisitorVoucher;
            foreach (Picking picking in currentTourplan.Picking_Tourplan)
            {
                picking.VisitorVoucher = currentVisitorVoucher;
            }

            currentTourplan.TourplanPos_Tourplan.AutoLoad(currentTourplan.TourplanPos_TourplanReference, currentTourplan);
            foreach (TourplanPos tourPos in currentTourplan.TourplanPos_Tourplan)
            {
                tourPos.DeliveryNote_TourplanPos.AutoLoad(tourPos.DeliveryNote_TourplanPosReference, tourPos);
                foreach (DeliveryNote note in tourPos.DeliveryNote_TourplanPos)
                {
                    if (note.VisitorVoucherID == null)
                    {
                        AssignDeliveryNote(currentVisitorVoucher, note, dbApp);
                    }
                }
            }

            PostExecute("AssignTourplan");
            return null;
        }

        [ACMethodInfo("", "en{'Remove Tourplan'}de{'Tourenplan entfernen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public Msg UnassignTourplan(VisitorVoucher currentVisitorVoucher, Tourplan currentTourplan, DatabaseApp dbApp)
        {
            if (!PreExecute("UnassignTourplan"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignTourplan(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50008")
                };
            }

            if (currentTourplan == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50020"));
            }

            currentTourplan.VisitorVoucher = null;
            currentTourplan.Picking_Tourplan.AutoLoad(currentTourplan.Picking_TourplanReference, currentTourplan);
            foreach (Picking picking in currentTourplan.Picking_Tourplan)
            {
                picking.VisitorVoucher = null;
            }

            currentTourplan.TourplanPos_Tourplan.AutoLoad(currentTourplan.TourplanPos_TourplanReference, currentTourplan);
            foreach (TourplanPos tourPos in currentTourplan.TourplanPos_Tourplan)
            {
                tourPos.DeliveryNote_TourplanPos.AutoLoad(tourPos.DeliveryNote_TourplanPosReference, tourPos);
                foreach (DeliveryNote note in tourPos.DeliveryNote_TourplanPos)
                {
                    if (note.VisitorVoucher != null)
                    {
                        UnassignDeliveryNote(currentVisitorVoucher, note, dbApp);
                    }
                }
            }

            PostExecute("UnassignTourplan");
            return null;
        }

        #endregion

        #region Picking

        [ACMethodInfo("", "en{'Assign Picking'}de{'Kommissionierplan zuordnen'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public virtual Msg AssignPicking(VisitorVoucher currentVisitorVoucher, Picking currentPicking, DatabaseApp dbApp,
                                ACComponent acFacilityManager, ACInDeliveryNoteManager inDeliveryNoteManager, ACOutDeliveryNoteManager outDeliveryNoteManager)
        {
            if (!PreExecute("AssignPicking"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignPicking(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50008")
                };
            }

            if (currentVisitorVoucher == null || currentPicking == null || dbApp == null || acFacilityManager == null || inDeliveryNoteManager == null || outDeliveryNoteManager == null || currentVisitorVoucher.Visitor == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50021"));
            }

            currentPicking.VisitorVoucher = currentVisitorVoucher;

            // Umgekehrt, falls Picking einem Tourenplan angehört, dann weise Picking per Tourenplanethde zu, weil Lieferscheine bereits zugeordnet sein müssten
            if (currentPicking.Tourplan != null)
            {
                //CurrentUnAssignedTourplan = currentPicking.Tourplan;
                AssignTourplan(currentVisitorVoucher, currentPicking.Tourplan, dbApp);
            }
            // Wareneingang
            else if (currentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.ReceiptVehicle)
            {
                currentPicking.PickingPos_Picking.AutoLoad(currentPicking.PickingPos_PickingReference, currentPicking);
                if (currentPicking.PickingPos_Picking.Any())
                {
                    // Weise bereits angelegte Lieferscheine dem Besuch zu
                    IEnumerable<DeliveryNote> inDeliveryNoteList = currentPicking.PickingPos_Picking
                                                                   .Where(c => c.InOrderPos != null && c.InOrderPos.InOrderPos1_ParentInOrderPos != null && c.InOrderPos.InOrderPos1_ParentInOrderPos.DeliveryNotePos_InOrderPos.Any())
                                                                   .Select(c => c.InOrderPos.InOrderPos1_ParentInOrderPos.DeliveryNotePos_InOrderPos.FirstOrDefault().DeliveryNote)
                                                                   .Distinct();
                    foreach (DeliveryNote inDeliveryNote in inDeliveryNoteList)
                    {
                        if (inDeliveryNote.VisitorVoucher != null)
                        {
                            return new Msg
                            {
                                Source = GetACUrl(),
                                MessageLevel = eMsgLevel.Info,
                                ACIdentifier = "AssignPicking(1)",
                                Message = Root.Environment.TranslateMessage(this, "Info50009")
                            };
                        }
                        else
                            inDeliveryNote.VisitorVoucher = currentVisitorVoucher;
                    }

                    // Erzeuge neue Lieferscheine
                    var queryWithoutOutDelivNote = currentPicking.PickingPos_Picking
                                                   .Where(c => c.InOrderPos != null && c.InOrderPos.InOrderPos1_ParentInOrderPos != null && !c.InOrderPos.InOrderPos1_ParentInOrderPos.DeliveryNotePos_InOrderPos.Any())
                                                   .OrderBy(c => c.InOrderPos.InOrder.DistributorCompany)
                                                   .ThenBy(c => c.InOrderPos.InOrder.InOrderNo)
                                                   .Select(c => c.InOrderPos.InOrderPos1_ParentInOrderPos);
                    Company lastCompany = null;
                    InOrder lastInOrder = null;
                    DeliveryNote lastDeliveryNote = null;
                    foreach (InOrderPos inOrderPos in queryWithoutOutDelivNote)
                    {
                        // DeliveryNoteAutoCreatePerOrder: Soll neuer Lieferschein pro Lieferant oder pro Lieferant + Bestellung (Standard)
                        if (inOrderPos.InOrder.DistributorCompany != lastCompany &&
                            (!DeliveryNoteAutoCreatePerOrder
                            || DeliveryNoteAutoCreatePerOrder && lastInOrder != inOrderPos.InOrder))
                        {
                            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(DeliveryNote), DeliveryNote.NoColumnName, DeliveryNote.FormatNewNo, this);
                            lastDeliveryNote = DeliveryNote.NewACObject(dbApp, null, secondaryKey);
                            lastDeliveryNote.VisitorVoucher = currentVisitorVoucher;
                            lastDeliveryNote.DeliveryNoteType = GlobalApp.DeliveryNoteType.Receipt;

                            // DeliveryCompanyAddress setzen
                            if (inOrderPos.InOrder.DeliveryCompanyAddress != null)
                                lastDeliveryNote.DeliveryCompanyAddress = inOrderPos.InOrder.DeliveryCompanyAddress;
                            else if (inOrderPos.InOrder.DistributorCompany != null)
                            {
                                if (inOrderPos.InOrder.DistributorCompany.DeliveryCompanyAddress != null)
                                    lastDeliveryNote.DeliveryCompanyAddress = inOrderPos.InOrder.DistributorCompany.DeliveryCompanyAddress;
                                else
                                    lastDeliveryNote.DeliveryCompanyAddress = inOrderPos.InOrder.DistributorCompany.HouseCompanyAddress;
                            }

                            // ShipperCompanyAddress setzen
                            if (currentVisitorVoucher.Visitor.VisitorCompany != null)
                            {
                                if (currentVisitorVoucher.Visitor.VisitorCompany.DeliveryCompanyAddress != null)
                                    lastDeliveryNote.ShipperCompanyAddress = currentVisitorVoucher.Visitor.VisitorCompany.DeliveryCompanyAddress;
                                else
                                    lastDeliveryNote.ShipperCompanyAddress = currentVisitorVoucher.Visitor.VisitorCompany.HouseCompanyAddress;
                            }
                            else if (currentVisitorVoucher.Visitor.VisitorCompanyPerson != null && currentVisitorVoucher.Visitor.VisitorCompanyPerson.Company != null)
                            {
                                if (currentVisitorVoucher.Visitor.VisitorCompanyPerson.Company.DeliveryCompanyAddress != null)
                                    lastDeliveryNote.ShipperCompanyAddress = currentVisitorVoucher.Visitor.VisitorCompanyPerson.Company.DeliveryCompanyAddress;
                                else
                                    lastDeliveryNote.ShipperCompanyAddress = currentVisitorVoucher.Visitor.VisitorCompanyPerson.Company.HouseCompanyAddress;
                            }

                        }
                        lastCompany = inOrderPos.InOrder.DistributorCompany;
                        lastInOrder = inOrderPos.InOrder;
                        List<object> resultNewEntities = new List<object>();
                        inDeliveryNoteManager.NewDeliveryNotePos(inOrderPos, lastDeliveryNote, dbApp, resultNewEntities);
                    }
                }
            }
            // Warenausgang
            else if (currentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.IssueVehicle)
            {
                if (currentPicking.EntityState != EntityState.Added)
                    currentPicking.PickingPos_Picking.AutoLoad(currentPicking.PickingPos_PickingReference, currentPicking);
                if (currentPicking.PickingPos_Picking.Any())
                {
                    // Weise bereits angelegte Lieferscheine dem Besuch zu
                    IEnumerable<DeliveryNote> outDeliveryNoteList = currentPicking.PickingPos_Picking
                                                                    .Where(c => c.OutOrderPos != null && c.OutOrderPos.OutOrderPos1_ParentOutOrderPos != null && c.OutOrderPos.OutOrderPos1_ParentOutOrderPos.DeliveryNotePos_OutOrderPos.Any())
                                                                    .Select(c => c.OutOrderPos.OutOrderPos1_ParentOutOrderPos.DeliveryNotePos_OutOrderPos.FirstOrDefault().DeliveryNote)
                                                                    .Distinct();
                    foreach (DeliveryNote outDeliveryNote in outDeliveryNoteList)
                    {
                        if (outDeliveryNote.VisitorVoucher != null)
                        {
                            return new Msg
                            {
                                Source = GetACUrl(),
                                MessageLevel = eMsgLevel.Info,
                                ACIdentifier = "AssignPicking(2)",
                                Message = Root.Environment.TranslateMessage(this, "Info50009")
                            };
                        }
                        else
                            outDeliveryNote.VisitorVoucher = currentVisitorVoucher;
                    }

                    // Erzeuge neue Lieferscheine
                    var queryWithoutIntDelivNote = currentPicking.PickingPos_Picking
                                                   .Where(c => c.OutOrderPos != null && c.OutOrderPos.OutOrderPos1_ParentOutOrderPos != null && !c.OutOrderPos.OutOrderPos1_ParentOutOrderPos.DeliveryNotePos_OutOrderPos.Any())
                                                   .OrderBy(c => c.OutOrderPos.OutOrder.CustomerCompany)
                                                   .ThenBy(c => c.OutOrderPos.OutOrder.OutOrderNo)
                                                   .Select(c => c.OutOrderPos.OutOrderPos1_ParentOutOrderPos);
                    Company lastCompany = null;
                    OutOrder lastOutOrder = null;
                    DeliveryNote lastDeliveryNote = null;
                    foreach (OutOrderPos outOrderPos in queryWithoutIntDelivNote)
                    {
                        // DeliveryNoteAutoCreatePerOrder: Soll neuer Lieferschein pro Lieferant oder pro Lieferant + Bestellung (Standard)
                        if (outOrderPos.OutOrder.CustomerCompany != lastCompany &&
                            (!DeliveryNoteAutoCreatePerOrder
                            || DeliveryNoteAutoCreatePerOrder && lastOutOrder != outOrderPos.OutOrder))
                        {
                            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(DeliveryNote), DeliveryNote.NoColumnName, DeliveryNote.FormatNewNo, this);
                            lastDeliveryNote = DeliveryNote.NewACObject(dbApp, null, secondaryKey);
                            lastDeliveryNote.VisitorVoucher = currentVisitorVoucher;
                            lastDeliveryNote.DeliveryNoteType = GlobalApp.DeliveryNoteType.Issue;

                            // DeliveryCompanyAddress setzen
                            if (outOrderPos.OutOrder.DeliveryCompanyAddress != null)
                                lastDeliveryNote.DeliveryCompanyAddress = outOrderPos.OutOrder.DeliveryCompanyAddress;
                            else if (outOrderPos.OutOrder.CustomerCompany != null)
                            {
                                if (outOrderPos.OutOrder.CustomerCompany.DeliveryCompanyAddress != null)
                                    lastDeliveryNote.DeliveryCompanyAddress = outOrderPos.OutOrder.CustomerCompany.DeliveryCompanyAddress;
                                else
                                    lastDeliveryNote.DeliveryCompanyAddress = outOrderPos.OutOrder.CustomerCompany.HouseCompanyAddress;
                            }

                            // ShipperCompanyAddress setzen
                            if (currentVisitorVoucher.Visitor.VisitorCompany != null)
                            {
                                if (currentVisitorVoucher.Visitor.VisitorCompany.DeliveryCompanyAddress != null)
                                    lastDeliveryNote.ShipperCompanyAddress = currentVisitorVoucher.Visitor.VisitorCompany.DeliveryCompanyAddress;
                                else
                                    lastDeliveryNote.ShipperCompanyAddress = currentVisitorVoucher.Visitor.VisitorCompany.HouseCompanyAddress;
                            }
                            else if (currentVisitorVoucher.Visitor.VisitorCompanyPerson != null && currentVisitorVoucher.Visitor.VisitorCompanyPerson.Company != null)
                            {
                                if (currentVisitorVoucher.Visitor.VisitorCompanyPerson.Company.DeliveryCompanyAddress != null)
                                    lastDeliveryNote.ShipperCompanyAddress = currentVisitorVoucher.Visitor.VisitorCompanyPerson.Company.DeliveryCompanyAddress;
                                else
                                    lastDeliveryNote.ShipperCompanyAddress = currentVisitorVoucher.Visitor.VisitorCompanyPerson.Company.HouseCompanyAddress;
                            }

                            if (lastDeliveryNote.ShipperCompanyAddress == null)
                            {
                                lastDeliveryNote.ShipperCompanyAddress = lastDeliveryNote.DeliveryCompanyAddress;
                            }
                        }
                        lastCompany = outOrderPos.OutOrder.CustomerCompany;
                        lastOutOrder = outOrderPos.OutOrder;
                        List<object> resultNewEntities = new List<object>();
                        outDeliveryNoteManager.NewDeliveryNotePos(outOrderPos, lastDeliveryNote, dbApp, resultNewEntities);
                    }
                }
            }

            currentPicking.PickingState = PickingStateEnum.InProcess;
            currentPicking.VisitorVoucher = currentVisitorVoucher;

            PostExecute("AssignPicking");
            return null;
        }

        [ACMethodInfo("", "en{'Remove Picking'}de{'Entfernen Kommissionierplan'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public virtual Msg UnassignPicking(Picking currentPicking, VisitorVoucher currentVisitorVoucher, DatabaseApp dbApp)
        {
            if (!PreExecute("UnassignPicking"))
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "UnassignPicking(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50008")
                };
            }

            if (currentPicking == null || currentVisitorVoucher == null)
            {
                throw new ArgumentNullException(Root.Environment.TranslateMessage(this, "Error50022"));
            }

            if (currentPicking.Tourplan != null)
            {
                //CurrentTourplan = currentPicking.Tourplan;
                UnassignTourplan(currentVisitorVoucher, currentPicking.Tourplan, dbApp);
            }
            // Wareneingang
            else if (currentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.ReceiptVehicle)
            {
                currentPicking.PickingPos_Picking.AutoLoad(currentPicking.PickingPos_PickingReference, currentPicking);
                if (currentPicking.PickingPos_Picking.Any())
                {
                    IEnumerable<DeliveryNote> inDeliveryNoteList = currentPicking.PickingPos_Picking
                                                                   .Where(c => c.InOrderPos != null && c.InOrderPos.InOrderPos1_ParentInOrderPos != null && c.InOrderPos.InOrderPos1_ParentInOrderPos.DeliveryNotePos_InOrderPos.Any())
                                                                   .Select(c => c.InOrderPos.InOrderPos1_ParentInOrderPos.DeliveryNotePos_InOrderPos.FirstOrDefault().DeliveryNote)
                                                                   .Distinct();
                    foreach (DeliveryNote inDeliveryNote in inDeliveryNoteList)
                    {
                        if (inDeliveryNote.VisitorVoucher != currentVisitorVoucher)
                        {
                            return new Msg
                            {
                                Source = GetACUrl(),
                                MessageLevel = eMsgLevel.Info,
                                ACIdentifier = "UnssignPicking(0)",
                                Message = Root.Environment.TranslateMessage(this, "Error50023")
                            };
                        }
                        else
                            inDeliveryNote.VisitorVoucher = null;
                    }
                }
            }
            else if (currentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.IssueVehicle)
            {
                currentPicking.PickingPos_Picking.AutoLoad(currentPicking.PickingPos_PickingReference, currentPicking);
                if (currentPicking.PickingPos_Picking.Any())
                {
                    IEnumerable<DeliveryNote> outDeliveryNoteList = currentPicking.PickingPos_Picking
                                                                    .Where(c => c.OutOrderPos != null && c.OutOrderPos.OutOrderPos1_ParentOutOrderPos != null && c.OutOrderPos.OutOrderPos1_ParentOutOrderPos.DeliveryNotePos_OutOrderPos.Any())
                                                                    .Select(c => c.OutOrderPos.OutOrderPos1_ParentOutOrderPos.DeliveryNotePos_OutOrderPos.FirstOrDefault().DeliveryNote)
                                                                    .Distinct();
                    foreach (DeliveryNote outDeliveryNote in outDeliveryNoteList)
                    {
                        if (outDeliveryNote.VisitorVoucher != currentVisitorVoucher)
                        {
                            return new Msg
                            {
                                Source = GetACUrl(),
                                MessageLevel = eMsgLevel.Info,
                                ACIdentifier = "UnssignPicking(0)",
                                Message = Root.Environment.TranslateMessage(this, "Error50023")
                            };
                        }
                        else
                            outDeliveryNote.VisitorVoucher = null;
                    }
                }
            }

            currentPicking.PickingState = PickingStateEnum.New;
            currentPicking.VisitorVoucher = null;

            PostExecute("UnassignPicking");
            return null;
        }

        #endregion

        #endregion

    }
}


