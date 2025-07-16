// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'ProdOrderManager'}de{'ProdOrderManager'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public partial class ACProdOrderManager : PARole
    {
        public const string ReloadBPAndResumeACIdentifier = "ReloadBPAndResume";

        #region c´tors
        public ACProdOrderManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _TolRemainingCallQ = new ACPropertyConfigValue<double>(this, nameof(TolRemainingCallQ), 20);
            _IsActiveMatReqCheck = new ACPropertyConfigValue<bool>(this, nameof(IsActiveMatReqCheck), false);
            _CalculateOEEs = new ACPropertyConfigValue<bool>(this, nameof(CalculateOEEs), false);
            _OneLotPerOrder = new ACPropertyConfigValue<ushort>(this, nameof(OneLotPerOrder), 0);
            _IgnoreLineOrderInPlanZero = new ACPropertyConfigValue<bool>(this, nameof(IgnoreLineOrderInPlanZero), false);
            _ValidateBatchOverplan = new ACPropertyConfigValue<bool>(this, nameof(ValidateBatchOverplan), true);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _FacilityOEEManager = ACFacilityOEEManager.ACRefToServiceInstance(this);
            _ = OneLotPerOrder;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_FacilityOEEManager != null)
            {
                ACFacilityOEEManager.DetachACRefFromServiceInstance(this, _FacilityOEEManager);
                _FacilityOEEManager = null;
            }

            return base.ACDeInit(deleteACClassTask);
        }
        #endregion


        #region Properties
        public const string ClassName = "ACProdOrderManager";
        public const string C_DefaultServiceACIdentifier = "ProdOrderManager";

        protected ACRef<ACFacilityOEEManager> _FacilityOEEManager = null;
        public ACFacilityOEEManager FacilityOEEManager
        {
            get
            {
                if (_FacilityOEEManager == null)
                    return null;
                return _FacilityOEEManager.ValueT;
            }
        }
        #endregion


        #region PrecompiledQueries
        //static readonly Func<DatabaseApp, IEnumerable<MDDelivPosState>> s_cQry_CompletelyAssigned =
        //EF.CompileQuery<DatabaseApp, IEnumerable<MDDelivPosState>>(
        //    (ctx) => from c in ctx.MDDelivPosState where c.MDDelivPosStateIndex == (Int16)MDDelivPosState.DelivPosStates.CompletelyAssigned select c
        //);

        //static readonly Func<DatabaseApp, IEnumerable<MDDelivPosState>> s_cQry_SubsetAssigned =
        //EF.CompileQuery<DatabaseApp, IEnumerable<MDDelivPosState>>(
        //    (ctx) => from c in ctx.MDDelivPosState where c.MDDelivPosStateIndex == (Int16)MDDelivPosState.DelivPosStates.SubsetAssigned select c
        //);

        //static readonly Func<DatabaseApp, IEnumerable<MDDelivPosState>> s_cQry_NotPlanned =
        //EF.CompileQuery<DatabaseApp, IEnumerable<MDDelivPosState>>(
        //    (ctx) => from c in ctx.MDDelivPosState where c.MDDelivPosStateIndex == (Int16)MDDelivPosState.DelivPosStates.NotPlanned select c
        //);

        public static Func<DatabaseApp, Guid, Guid, IEnumerable<ProdOrderPartslistPos>> s_cQry_ProdOrderAlternativePositions =
            EF.CompileQuery<DatabaseApp, Guid, Guid, IEnumerable<ProdOrderPartslistPos>>(
            (db, selectedProdOrderPartslistID, selectedProdOrderPartslistPosID) =>
                    db.ProdOrderPartslistPos.Where(c =>
                    c.ProdOrderPartslistID == selectedProdOrderPartslistID &&
                    c.AlternativeProdOrderPartslistPosID == selectedProdOrderPartslistPosID &&
                    c.MaterialPosTypeIndex == (int)(gip.mes.datamodel.GlobalApp.MaterialPosTypes.OutwardRoot) &&
                    c.ParentProdOrderPartslistPosID == null &&
                    c.AlternativeProdOrderPartslistPosID == null)
                    .Select(c => c)
        );


        #endregion

        #region static Methods
        public static ACProdOrderManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACProdOrderManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ACProdOrderManager> ACRefToServiceInstance(ACComponent requester)
        {
            ACProdOrderManager serviceInstance = GetServiceInstance(requester) as ACProdOrderManager;
            if (serviceInstance != null)
                return new ACRef<ACProdOrderManager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region Properties

        ACMethodBooking _BookParamInwardMovementClone;
        public ACMethodBooking BookParamInwardMovementClone(ACComponent facilityManager, DatabaseApp dbApp)
        {
            if (_BookParamInwardMovementClone != null)
                return _BookParamInwardMovementClone;
            if (facilityManager == null)
                return null;
            _BookParamInwardMovementClone = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ProdOrderPosInward.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            return _BookParamInwardMovementClone;
        }

        ACMethodBooking _BookParamInCancelClone;
        public ACMethodBooking BookParamInCancelClone(ACComponent facilityManager, DatabaseApp dbApp)
        {
            if (_BookParamInCancelClone != null)
                return _BookParamInCancelClone;
            if (facilityManager == null)
                return null;
            _BookParamInCancelClone = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ProdOrderPosInwardCancel.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            return _BookParamInCancelClone;
        }

        ACMethodBooking _BookParamOutwardMovementClone;
        public ACMethodBooking BookParamOutwardMovementClone(ACComponent facilityManager, DatabaseApp dbApp)
        {
            if (_BookParamOutwardMovementClone != null)
                return _BookParamOutwardMovementClone;
            if (facilityManager == null)
                return null;
            _BookParamOutwardMovementClone = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ProdOrderPosOutward.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            return _BookParamOutwardMovementClone;
        }

        ACMethodBooking _BookParamOutCancelClone;
        public ACMethodBooking BookParamOutCancelClone(ACComponent facilityManager, DatabaseApp dbApp)
        {
            if (_BookParamOutCancelClone != null)
                return _BookParamOutCancelClone;
            if (facilityManager == null)
                return null;
            _BookParamOutCancelClone = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ProdOrderPosOutwardCancel.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            return _BookParamOutCancelClone;
        }

        ACMethodBooking _BookParamOutwardMovementOnEmptyingFacilityClone;
        public ACMethodBooking BookParamOutwardMovementOnEmptyingFacilityClone(ACComponent facilityManager, DatabaseApp dbApp)
        {
            if (_BookParamOutwardMovementOnEmptyingFacilityClone != null)
                return _BookParamOutwardMovementOnEmptyingFacilityClone;
            if (facilityManager == null)
                return null;
            _BookParamOutwardMovementOnEmptyingFacilityClone = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ProdOrderPosOutwardOnEmptyingFacility.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            return _BookParamOutwardMovementOnEmptyingFacilityClone;
        }

        private ACPropertyConfigValue<double> _TolRemainingCallQ;
        [ACPropertyConfig("en{'Tolerance in called up quantity %'}de{'Toleranz Abrufmenge %'}")]
        public double TolRemainingCallQ
        {
            get
            {
                return _TolRemainingCallQ.ValueT;
            }
            set
            {
                _TolRemainingCallQ.ValueT = value;
            }
        }

        private ACPropertyConfigValue<bool> _IsActiveMatReqCheck;
        [ACPropertyConfig("en{'Activate Material-Requirement-Check'}de{'Materialbedarfsprüfung aktivieren'}")]
        public bool IsActiveMatReqCheck
        {
            get
            {
                return _IsActiveMatReqCheck.ValueT;
            }
            set
            {
                _IsActiveMatReqCheck.ValueT = value;
            }
        }

        private ACPropertyConfigValue<bool> _CalculateOEEs;
        [ACPropertyConfig("en{'OEE-Calculation active'}de{'OEE-Berechnung aktiv'}")]
        public bool CalculateOEEs
        {
            get
            {
                return _CalculateOEEs.ValueT;
            }
            set
            {
                _CalculateOEEs.ValueT = value;
            }
        }

        public class BatchCreateEntry
        {
            public int BatchSeqNo { get; set; }
            public double Size { get; set; }
        }

        private ACPropertyConfigValue<bool> _IgnoreLineOrderInPlanZero;
        [ACPropertyConfig("en{'Ignore line where order of line is zero'}de{'Zeile ignorieren, bei der die Zeilenreihenfolge Null ist'}")]
        public bool IgnoreLineOrderInPlanZero
        {
            get
            {
                return _IgnoreLineOrderInPlanZero.ValueT;
            }
            set
            {
                _IgnoreLineOrderInPlanZero.ValueT = value;
            }
        }

        private ACPropertyConfigValue<bool> _ValidateBatchOverplan;
        [ACPropertyConfig("en{'Validate Overplanning'}de{'Prüfe Überplanung'}")]
        public bool ValidateBatchOverplan
        {
            get
            {
                return _ValidateBatchOverplan.ValueT;
            }
            set
            {
                _ValidateBatchOverplan.ValueT = value;
            }
        }


        [ACPropertyBindingSource(730, "Error", "en{'ProdOrderManager-Alarm'}de{'ProdOrderManager-Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> IsProdOrderManagerAlarm { get; set; }

        private ACPropertyConfigValue<ushort> _OneLotPerOrder;
        [ACPropertyConfig("en{'One Lot per Order (0=Off,1=Odernumber,2=Lotnumber)'}de{'Ein Los pro Auftrag (0=Aus,1=Auftragsnummer,2=Losnummer)'}", DefaultValue = false)]
        public ushort OneLotPerOrder
        {
            get { return _OneLotPerOrder.ValueT; }
            set { _OneLotPerOrder.ValueT = value; }
        }

        public LotCreationModeEnum LotCreationMode
        {
            get
            {
                if (OneLotPerOrder == (ushort)LotCreationModeEnum.EachBatchOneLot)
                    return LotCreationModeEnum.EachBatchOneLot;
                else if (OneLotPerOrder == (ushort)LotCreationModeEnum.OneOrderOneLotWithOrderNo)
                    return LotCreationModeEnum.OneOrderOneLotWithOrderNo;
                else if (OneLotPerOrder == (ushort)LotCreationModeEnum.OneOrderOneLotWithLotNo)
                    return LotCreationModeEnum.OneOrderOneLotWithLotNo;
                else
                    return LotCreationModeEnum.EachBatchOneLot;
            }
        }

        public enum LotCreationModeEnum
        {
            EachBatchOneLot = 0,
            OneOrderOneLotWithOrderNo = 1,
            OneOrderOneLotWithLotNo = 2
        }

        #endregion

        #region (ProdOrder)Partslist
        /// <summary>
        /// Add Partslist to production order
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="partslistID"></param>
        public Msg PartslistAdd(DatabaseApp dbApp, ProdOrder prodOrder, Partslist partsList, int sequence, double targetQuantityUOM, out ProdOrderPartslist prodOrderPartsList)
        {
            prodOrderPartsList = null;
            Msg msg = null;
            if (!PreExecute("PartslistAdd"))
            {
                //"Info: Adding parts list into production order is in progress!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "PartslistAdd(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50012")
                };
            }

            if (prodOrder == null)
            {
                //"Error: Poduction order is not defined!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "PartslistAdd(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50027")
                };
            }

            if (targetQuantityUOM <= 0)
            {
                //"Error: Target Quantity shuld be greather than zero!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "PartslistAdd(2)",
                    Message = Root.Environment.TranslateMessage(this, "Error50028")
                };
            }

            if (partsList == null)
            {
                //"Error: Parts list is not defined!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "PartslistAdd(3)",
                    Message = Root.Environment.TranslateMessage(this, "Error50029")
                };
            }


            if (partsList.TargetQuantityUOM <= Double.Epsilon)
            {
                //"Error: Partslist quantity shuld be greather than zero!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "PartslistAdd(4)",
                    Message = Root.Environment.TranslateMessage(this, "Error50030")
                };
            }

            double quantityFactor = targetQuantityUOM / partsList.TargetQuantityUOM;

            List<PartslistPos> partsListPosItems = dbApp.PartslistPos.Where(x => x.PartslistID == partsList.PartslistID).ToList();
            List<Guid> partsListPosItemIDs = partsListPosItems.Select(x => x.PartslistPosID).ToList();
            List<PartslistPosRelation> partsListPosRelationItems =
            dbApp.PartslistPosRelation.Where(x => partsListPosItemIDs.Contains(x.TargetPartslistPosID)).ToList();
            partsListPosRelationItems.AddRange(dbApp.PartslistPosRelation.Where(x => partsListPosItemIDs.Contains(x.SourcePartslistPosID)));

            partsListPosRelationItems = partsListPosRelationItems.Distinct().ToList();

            prodOrderPartsList = ProdOrderPartslist.NewACObject(dbApp, prodOrder);
            prodOrderPartsList.TargetQuantity = partsList.TargetQuantityUOM;
            prodOrderPartsList.IsEnabled = true;
            prodOrderPartsList.Partslist = partsList;
            prodOrderPartsList.Sequence = sequence;
            prodOrderPartsList.LastFormulaChange = partsList.LastFormulaChange;

            List<ProdOrderPartslistPos> prodOrderPartsListPosItems = new List<ProdOrderPartslistPos>();
            foreach (var pos in partsListPosItems)
            {
                ProdOrderPartslistPos prodPos = GetProdOrderPartslistPos(dbApp, prodOrderPartsList, pos);
                prodOrderPartsListPosItems.Add(prodPos);
                prodOrderPartsList.Context.Add(prodPos);
                prodOrderPartsList.ProdOrderPartslistPos_ProdOrderPartslist.Add(prodPos);
            }

            // update alternative relations
            prodOrderPartsListPosItems.ForEach(x =>
            {
                if (x.BasedOnPartslistPos.AlternativePartslistPosID != null)
                {
                    x.ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos = prodOrderPartsListPosItems.FirstOrDefault(c => c.BasedOnPartslistPos.PartslistPosID == x.BasedOnPartslistPos.AlternativePartslistPosID);
                }
            }
                );

            List<ProdOrderPartslistPosRelation> prodOrderPartsListPosRelationItems = new List<ProdOrderPartslistPosRelation>();
            foreach (PartslistPosRelation posRelation in partsListPosRelationItems)
            {
                ProdOrderPartslistPosRelation prodRelationItem = GetProdOrderPartslistPosRelation(dbApp, prodOrderPartsListPosItems, posRelation);
                if (prodRelationItem != null)
                {
                    prodRelationItem.TargetProdOrderPartslistPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Add(prodRelationItem);
                    prodOrderPartsListPosRelationItems.Add(prodRelationItem);
                }
            }

            // Think is not 
            //foreach (var item in prodOrderPartsListPosItems)
            //{
            //    List<ProdOrderPartslistPosRelation> targets = prodOrderPartsListPosRelationItems.Where(x => x.TargetProdOrderPartslistPosID == item.ProdOrderPartslistPosID).ToList();
            //    List<ProdOrderPartslistPosRelation> sources = prodOrderPartsListPosRelationItems.Where(x => x.SourceProdOrderPartslistPosID == item.ProdOrderPartslistPosID).ToList();
            //    targets.ForEach(x => item.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Add(x));
            //    sources.ForEach(x => item.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Add(x));
            //}

            // Resize quantity
            BatchLinearResize(prodOrderPartsList, quantityFactor);
            BatchLinearResize(prodOrderPartsListPosItems, quantityFactor);
            BatchLinearResize(prodOrderPartsListPosRelationItems, quantityFactor);

            dbApp.ProdOrderPartslist.Add(prodOrderPartsList);

            PostExecute("PartslistAdd");
            return msg;
        }

        public ProdOrderPartslistPos GetProdOrderPartslistPos(DatabaseApp dbApp, ProdOrderPartslist prodOrderPartsList, PartslistPos pos)
        {
            ProdOrderPartslistPos prodPos = ProdOrderPartslistPos.NewACObject(dbApp, null);
            prodPos.ProdOrderPartslistID = prodOrderPartsList.ProdOrderPartslistID;
            prodPos.ProdOrderPartslist = prodOrderPartsList;
            prodPos.Sequence = pos.Sequence;
            prodPos.SequenceProduction = pos.Sequence;
            prodPos.MaterialPosTypeIndex = pos.MaterialPosTypeIndex;
            prodPos.MaterialID = pos.MaterialID;
            prodPos.MDUnitID = pos.MDUnitID;
            prodPos.ActualQuantity = 0;
            prodPos.TargetQuantityUOM = pos.TargetQuantityUOM;
            //prodPos.TargetQuantity = pos.TargetQuantity;
            prodPos.ActualQuantityUOM = 0;
            prodPos.IsBaseQuantityExcluded = false;
            prodPos.ParentProdOrderPartslistPosID = null;
            prodPos.AlternativeProdOrderPartslistPosID = null;
            prodPos.LineNumber = pos.LineNumber;
            prodPos.BasedOnPartslistPos = pos;
            if (pos.TakeMatFromOtherOrder.HasValue)
                prodPos.TakeMatFromOtherOrder = pos.TakeMatFromOtherOrder.Value;

            return prodPos;
        }


        public ProdOrderPartslistPosRelation GetProdOrderPartslistPosRelation(DatabaseApp dbApp, List<ProdOrderPartslistPos> prodOrderPartsListPosItems, PartslistPosRelation posRelation)
        {
            ProdOrderPartslistPosRelation prodRelationItem = null;
            ProdOrderPartslistPos sourcePos = prodOrderPartsListPosItems.FirstOrDefault(c => c.BasedOnPartslistPos != null && c.BasedOnPartslistPos.PartslistPosID == posRelation.SourcePartslistPosID);
            ProdOrderPartslistPos targetPos = prodOrderPartsListPosItems.FirstOrDefault(c => c.BasedOnPartslistPos != null && c.BasedOnPartslistPos.PartslistPosID == posRelation.TargetPartslistPosID);// ToSetup

            if (sourcePos != null && targetPos != null)
            {
                prodRelationItem = GetProdOrderPartslistPosRelation(dbApp, prodOrderPartsListPosItems, posRelation, sourcePos, targetPos);
            }
            else
                Root.Messages.LogError(GetACUrl(), "", "Unable to build relation");
            return prodRelationItem;
        }

        public ProdOrderPartslistPosRelation GetProdOrderPartslistPosRelation(DatabaseApp dbApp, List<ProdOrderPartslistPos> prodOrderPartsListPosItems, PartslistPosRelation posRelation, ProdOrderPartslistPos sourcePos, ProdOrderPartslistPos targetPos)
        {
            ProdOrderPartslistPosRelation prodRelationItem = ProdOrderPartslistPosRelation.NewACObject(dbApp, null);
            prodRelationItem.Sequence = posRelation.Sequence;
            prodRelationItem.TargetQuantityUOM = posRelation.TargetQuantityUOM;
            prodRelationItem.RetrogradeFIFO = posRelation.RetrogradeFIFO;
            // build relation same to PartslistPosRelation
            prodRelationItem.SourceProdOrderPartslistPos = sourcePos;
            prodRelationItem.TargetProdOrderPartslistPos = targetPos;

            return prodRelationItem;
        }

        /// <summary>
        /// Removing partslist from prodorder
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="prodOrder"></param>
        /// <param name="prodOrderPartslist"></param>
        /// <returns></returns>
        public Msg PartslistRemove(DatabaseApp dbApp, ProdOrder prodOrder, ProdOrderPartslist prodOrderPartslist)
        {
            Msg msg = null;
            if (!PreExecute("PartslistRemove"))
            {
                //"Info: Adding parts list into production order is in progress!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "PartslistRemove(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50012")
                };
            }

            if (prodOrder == null)
            {
                //"Error: Poduction order is not defined!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "PartslistRemove(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50027")
                };
            }

            if (prodOrderPartslist == null)
            {
                //"Error: Production order parts list is not defined!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "PartslistRemove(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50031")
                };
            }

            // Delete reference to ProdorderPartslistPos witch this ProdOrderPartslist produced
            List<ProdOrderPartslistPos> prodOrderPosProducedWithThisProdPartList =
                dbApp
                .ProdOrderPartslistPos
                .Where(x => x.SourceProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID)
                .ToList();

            prodOrderPosProducedWithThisProdPartList.Where(c => c.EntityState != EntityState.Deleted).ToList().ForEach(x => x.SourceProdOrderPartslistID = null);

            List<ProdOrderPartslistPos> items =
                dbApp
                .ProdOrderPartslistPos
                .Where(x => x.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID)
                .ToList();

            List<FacilityReservation> reservations = items.SelectMany(c => c.FacilityReservation_ProdOrderPartslistPos).ToList();

            List<Guid> itemIDs = items.Select(x => x.ProdOrderPartslistPosID).ToList();

            List<ProdOrderPartslistPosRelation> relations =
                 dbApp
                .ProdOrderPartslistPosRelation
                .Where(x => itemIDs.Contains(x.TargetProdOrderPartslistPosID) || itemIDs.Contains(x.SourceProdOrderPartslistPosID))
                .ToList();


            List<ProdOrderBatch> batches = dbApp.ProdOrderBatch.Where(x => x.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID).ToList();

            List<FacilityPreBooking> preBookings = new List<FacilityPreBooking>();
            foreach (var item in items)
            {
                preBookings.AddRange(item.FacilityPreBooking_ProdOrderPartslistPos);
            }

            foreach (var bp in prodOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.ToArray())
            {
                foreach (var fr in bp.FacilityReservation_ProdOrderBatchPlan.ToArray())
                {
                    fr.DeleteACObject(dbApp, false);
                }
                bp.DeleteACObject(dbApp, false);
            }

            preBookings.ForEach(x => x.DeleteACObject(dbApp, false));

            relations.ForEach(x => x.DeleteACObject(dbApp, false));
            reservations.ForEach(x => x.DeleteACObject(dbApp, false));
            items.ForEach(x => x.DeleteACObject(dbApp, false));

            batches.ForEach(x => x.DeleteACObject(dbApp, false));

            foreach (PlanningMRProposal plan in prodOrderPartslist.PlanningMRProposal_ProdOrderPartslist.ToArray())
            {
                plan.DeleteACObject(dbApp, false);
            }

            prodOrderPartslist.DeleteACObject(dbApp, false);


            PostExecute("PartslistRemove");
            return msg;
        }

        public Msg RefreshScheduledTemplateOrders(DatabaseApp dbApp, Partslist partslist, ProdOrderPartslist prodOrderPartslist)
        {
            Msg msg = null;
            if (prodOrderPartslist.TargetQuantity <= 0)
            {
                //"Error: Target Quantity shuld be greather than zero!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "PartslistRebuild(2)",
                    Message = Root.Environment.TranslateMessage(this, "Error50028")
                };
            }

            ProdOrderPartslistPosRelation[] relationForDelete = prodOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(c => c.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos).ToArray();
            foreach (ProdOrderPartslistPosRelation relation in relationForDelete)
                relation.DeleteACObject(dbApp, false);

            // Delete Positions no final mixures
            ProdOrderPartslistPos[] posForDelete = prodOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist.AsEnumerable().Where(c => !c.IsFinalMixure).ToArray();
            foreach (ProdOrderPartslistPos pos in posForDelete)
                pos.DeleteACObject(dbApp, false);


            // Positions without relations will be not recreated
            PartslistPos[] plPos = partslist.PartslistPos_Partslist.Where(c => c.PartslistPosRelation_SourcePartslistPos.Any()).ToArray();
            PartslistPosRelation[] plRel = partslist.PartslistPos_Partslist.SelectMany(c => c.PartslistPosRelation_TargetPartslistPos).ToArray();

            double quantityFactor = prodOrderPartslist.TargetQuantity / partslist.TargetQuantityUOM;

            List<ProdOrderPartslistPos> prodOrderPartsListPosItems = new List<ProdOrderPartslistPos>();
            prodOrderPartsListPosItems.AddRange(prodOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist);
            foreach (ProdOrderPartslistPos prodPos in prodOrderPartsListPosItems)
            {
                PartslistPos pos = plPos.FirstOrDefault(c => c.MaterialID == prodPos.MaterialID);
                if (pos != null && prodPos.BasedOnPartslistPosID != pos.PartslistPosID)
                    prodPos.BasedOnPartslistPosID = pos.PartslistPosID;
            }

            foreach (var pos in plPos)
            {
                if (!prodOrderPartsListPosItems.Any(c => c.BasedOnPartslistPosID == pos.PartslistPosID))
                {
                    ProdOrderPartslistPos prodPos = GetProdOrderPartslistPos(dbApp, prodOrderPartslist, pos);
                    BatchLinearResize(prodPos, quantityFactor);
                    prodOrderPartsListPosItems.Add(prodPos);
                    prodOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist.Add(prodPos);
                }
            }

            // update alternative relations
            prodOrderPartsListPosItems.ForEach(x =>
            {
                if (x.BasedOnPartslistPos != null && x.BasedOnPartslistPos.AlternativePartslistPosID != null)
                {
                    x.ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos = prodOrderPartsListPosItems.FirstOrDefault(c => c.BasedOnPartslistPos.PartslistPosID == x.BasedOnPartslistPos.AlternativePartslistPosID);
                }
            });

            List<ProdOrderPartslistPosRelation> prodOrderPartsListPosRelationItems = new List<ProdOrderPartslistPosRelation>();
            foreach (PartslistPosRelation posRelation in plRel)
            {
                ProdOrderPartslistPosRelation prodRelationItem = GetProdOrderPartslistPosRelation(dbApp, prodOrderPartsListPosItems, posRelation);
                if (prodRelationItem != null)
                {
                    prodRelationItem.TargetProdOrderPartslistPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Add(prodRelationItem);
                    BatchLinearResize(prodRelationItem, quantityFactor);
                }
            }

            prodOrderPartslist.RecalcActualQuantity();
            prodOrderPartslist.LastFormulaChange = partslist.LastFormulaChange;

            return msg;
        }
        /// <summary>
        /// changing target quantity
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="prodOrderPartslist"></param>
        /// <param name="targetQuantityUOM"></param>
        public Msg ProdOrderPartslistChangeTargetQuantity(DatabaseApp dbApp, ProdOrderPartslist prodOrderPartslist, double targetQuantityUOM)
        {
            Msg msg = null;
            if (!PreExecute("ProdOrderPartslistChangeTargetQuantity"))
            {
                //"Info: Changing production order parts list target quantity in progress!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "ProdOrderPartslistChangeTargetQuantity(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50013")
                };
            }

            if (prodOrderPartslist == null)
            {
                //"Error: Production order parts list is not defined!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "ProdOrderPartslistChangeTargetQuantity(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50031")
                };
            }

            if (prodOrderPartslist.TargetQuantity <= 0)
            {
                //"Error: Production order parts list target quantity shuld be greather then zero!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "ProdOrderPartslistChangeTargetQuantity(2)",
                    Message = Root.Environment.TranslateMessage(this, "Error50032")
                };
            }

            double changeFactor = targetQuantityUOM / prodOrderPartslist.TargetQuantity;

            // calculate database items
            List<ProdOrderPartslistPos> positions = dbApp.ProdOrderPartslistPos.Where(x =>
                x.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID &&
                x.ParentProdOrderPartslistPosID == null).ToList(); // --> resize positions they are not batches
            List<Guid> positionIds = positions.Select(x => x.ProdOrderPartslistPosID).ToList();
            List<ProdOrderPartslistPosRelation> relations = dbApp.ProdOrderPartslistPosRelation.Where(x =>
            positionIds.Contains(x.TargetProdOrderPartslistPosID)).ToList();

            // calculate unsaved added items
            List<ProdOrderPartslistPos> positionsAdded = dbApp.GetAddedEntities<ProdOrderPartslistPos>()
                .Where(x => x.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID)
                .ToList();
            List<Guid> positionAddedIds = positionsAdded.Select(x => x.ProdOrderPartslistPosID).ToList();
            List<ProdOrderPartslistPosRelation> relationsAdded = dbApp.GetAddedEntities<ProdOrderPartslistPosRelation>()
                .Where(x => positionAddedIds.Contains(x.TargetProdOrderPartslistPosID))
                .ToList();


            BatchLinearResize(positions, changeFactor);
            BatchLinearResize(relations, changeFactor);

            BatchLinearResize(positionsAdded, changeFactor);
            BatchLinearResize(relationsAdded, changeFactor);

            prodOrderPartslist.TargetQuantity = targetQuantityUOM;

            PostExecute("ProdOrderPartslistChangeTargetQuantity");
            return msg;
        }


        public List<ProdOrderPartslist> GetNextStages(DatabaseApp dbApp, ProdOrderPartslist fromPOPartslist)
        {
            if (fromPOPartslist == null || dbApp == null)
                return new List<ProdOrderPartslist>();
            List<ProdOrderPartslist> nextStages = dbApp.ProdOrderPartslistPos.Where(c => c.ProdOrderPartslist.ProdOrderID == fromPOPartslist.ProdOrderID
                                                                                    && c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                                                                                    && c.MaterialID == fromPOPartslist.Partslist.MaterialID)
                                                    .Select(c => c.ProdOrderPartslist).ToList();
            return nextStages;
        }

        public List<ProdOrderBatchPlan> GetBatchplansOfNextStages(DatabaseApp dbApp, ProdOrderPartslist fromPOPartslist)
        {
            if (fromPOPartslist == null || dbApp == null)
                return new List<ProdOrderBatchPlan>();
            List<ProdOrderBatchPlan> nextStages = dbApp.ProdOrderPartslistPos.Where(c => c.ProdOrderPartslist.ProdOrderID == fromPOPartslist.ProdOrderID
                                                                                    && c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                                                                                    && c.MaterialID == fromPOPartslist.Partslist.MaterialID)
                                                    .SelectMany(c => c.ProdOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist).ToList();
            return nextStages;
        }
        #endregion

        #region Batch-Creation
        /// <summary>
        /// Create batch item
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="intermediateItem"></param>
        /// <param name="batches"></param>
        /// <returns></returns>
        public Msg BatchCreate(DatabaseApp dbApp, ProdOrderPartslistPos intermediateItem, List<BatchCreateEntry> batches, List<object> resultNewEntities, bool reduceWithLossFactor, double? toleranceRemainingQuantity = null)
        {
            Msg msg = null;
            if (!PreExecute("BatchCreate"))
            {
                //"Info: Batch creating in progress!"
                return new Msg(this, eMsgLevel.Info, "ACProdOrderManager", "BatchCreate", 1000, "Info50014");
            }

            if (intermediateItem == null)
            {
                //"Error50033: Intermediate product not defined!"
                return new Msg(this, eMsgLevel.Error, ClassName, "BatchCreate", 1010, "Error50033");
            }

            if (batches == null || !batches.Any())
            {
                //"Error50034: Batch size not defined!"
                return new Msg(this, eMsgLevel.Error, ClassName, "BatchCreate", 1020, "Error50034");
            }

            if (!toleranceRemainingQuantity.HasValue)
            {
                if (TolRemainingCallQ != 0 && intermediateItem.TargetQuantityUOM != 0)
                    toleranceRemainingQuantity = intermediateItem.TargetQuantityUOM * TolRemainingCallQ * 0.01;
                else
                    toleranceRemainingQuantity = 0;
            }

            intermediateItem.AutoRefresh(dbApp);

            if (reduceWithLossFactor)
            {
                double lossFactor = intermediateItem.ProdOrderPartslist.TargetQuantityLossFactor;
                if (lossFactor < 0.0000000001 || lossFactor > 2)
                    lossFactor = 1;
                batches.ForEach(c => c.Size = c.Size * lossFactor);
            }

            double quantityOverAllBatches = batches.Select(c => c.Size).Sum();

            if ((intermediateItem.RemainingCallQuantity + toleranceRemainingQuantity) < quantityOverAllBatches
                || intermediateItem.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                || intermediateItem.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Blocked)
            {
                //Error50052: Batch not startable. The remaining quantity is {1}. Planned are {0}. The maximum order quantity would be exceeeded.
                Msg rMsg = new Msg(this, eMsgLevel.Error, ClassName, "BatchCreate", 1030, "Error50052", quantityOverAllBatches, intermediateItem.RemainingCallQuantity);
                return rMsg;
            }

            BatchCreateModel model = null;
            if (intermediateItem.ParentProdOrderPartslistPosID == null)
            {
                model = new BatchCreateModel();
                List<ProdOrderPartslistPosRelation> relations = dbApp.ProdOrderPartslistPosRelation
                    .Where(x => x.TargetProdOrderPartslistPosID == intermediateItem.ProdOrderPartslistPosID).ToList();
                foreach (var item in batches)
                {
                    string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(ProdOrderBatch), ProdOrderBatch.NoColumnName, ProdOrderBatch.FormatNewNo, this);
                    ProdOrderBatch batch = ProdOrderBatch.NewACObject(dbApp, intermediateItem.ProdOrderPartslist, secondaryKey);
                    batch.BatchSeqNo = item.BatchSeqNo;
                    model.Batches.Add(batch);
                    resultNewEntities.Add(batch);

                    ProdOrderPartslistPos childPosition = ProdOrderPartslistPos.NewACObject(dbApp, intermediateItem);
                    childPosition.Sequence = item.BatchSeqNo;
                    childPosition.TargetQuantityUOM = item.Size;
                    intermediateItem.CalledUpQuantityUOM += childPosition.TargetQuantityUOM;
                    childPosition.ProdOrderBatch = batch;
                    model.BatchPositions.Add(childPosition);
                    resultNewEntities.Add(childPosition);
                    double quantityFactor = childPosition.TargetQuantityUOM / intermediateItem.TargetQuantityUOM;

                    foreach (var rel in relations)
                    {
                        ProdOrderPartslistPosRelation childRelation = ProdOrderPartslistPosRelation.NewACObject(dbApp, rel);
                        childRelation.Sequence = rel.Sequence;
                        childRelation.TargetProdOrderPartslistPos = childPosition;
                        childRelation.SourceProdOrderPartslistPos = rel.SourceProdOrderPartslistPos;
                        childRelation.TargetQuantityUOM = rel.TargetQuantityUOM * quantityFactor;
                        childRelation.RetrogradeFIFO = rel.RetrogradeFIFO;
                        childRelation.ProdOrderBatch = batch;
                        model.BatchRelations.Add(childRelation);
                        resultNewEntities.Add(childRelation);
                    }
                }

                model.Batches.ForEach(x => dbApp.ProdOrderBatch.Add(x));
                model.BatchPositions.ForEach(x => dbApp.ProdOrderPartslistPos.Add(x));
                model.BatchRelations.ForEach(x => dbApp.ProdOrderPartslistPosRelation.Add(x));
                //model.BatchDefinition = batches;
            }

            PostExecute("BatchCreate");
            return msg;
        }

        public Msg BatchCreate(DatabaseApp dbApp, ProdOrderPartslistPos intermediateItem, ProdOrderBatch batch, double batchFraction, int sequenceNo, List<object> resultNewEntities)
        {
            Msg msg = null;
            //if (!PreExecute("BatchCreate"))
            //{
            //    //"Info: Batch creating in progress!"
            //    return new Msg
            //    {
            //        Source = GetACUrl(),
            //        MessageLevel = eMsgLevel.Info,
            //        ACIdentifier = "BatchCreate(0)",
            //        Message = Root.Environment.TranslateMessage(this, "Info50014")
            //    };
            //}

            if (intermediateItem == null)
            {
                //"Error50033: Intermediate product not defined!"
                return new Msg(this, eMsgLevel.Error, ClassName, "BatchCreate", 1500, "Error50033");
            }

            if (batch == null)
            {
                //"Error50034: Batch size not defined!"
                return new Msg(this, eMsgLevel.Error, ClassName, "BatchCreate", 1510, "Error50034");
            }

            intermediateItem.AutoRefresh(dbApp);

            if (intermediateItem.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                || intermediateItem.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Blocked)
            {
                //Error50099: Orderline is already in state completed or blocked.
                return new Msg(this, eMsgLevel.Error, ClassName, "BatchCreate", 1520, "Error50099");
            }

            List<ProdOrderPartslistPosRelation> batchRelations = new List<ProdOrderPartslistPosRelation>();
            if (intermediateItem.ParentProdOrderPartslistPosID == null)
            {
                ProdOrderPartslistPos childPosition = ProdOrderPartslistPos.NewACObject(dbApp, intermediateItem);
                childPosition.Sequence = sequenceNo;
                childPosition.TargetQuantityUOM = intermediateItem.TargetQuantityUOM * batchFraction;
                intermediateItem.CalledUpQuantityUOM += childPosition.TargetQuantityUOM;
                childPosition.ProdOrderBatch = batch;
                resultNewEntities.Add(childPosition);

                List<ProdOrderPartslistPosRelation> relations = dbApp.ProdOrderPartslistPosRelation.Where(x => x.TargetProdOrderPartslistPosID == intermediateItem.ProdOrderPartslistPosID).ToList();
                foreach (var rel in relations)
                {
                    ProdOrderPartslistPosRelation childRelation = ProdOrderPartslistPosRelation.NewACObject(dbApp, rel);
                    childRelation.Sequence = rel.Sequence;
                    childRelation.TargetProdOrderPartslistPos = childPosition;
                    childRelation.SourceProdOrderPartslistPos = rel.SourceProdOrderPartslistPos;
                    // COMMENT @aagincic: This shuld be corect
                    // first we take proportion between quantity of batch intermediate component and intermediate product => childPosition.TargetQuantity / intermediateItem.TargetQuantity
                    // this shuld say percentage of batch component participation in final intermediate product
                    // second we multiple this factor with parent "complete" intermediate component to get batch component size
                    if (rel.TargetQuantityUOM > 0.00001)
                        childRelation.TargetQuantityUOM = rel.TargetQuantityUOM * batchFraction;
                    else
                        childRelation.TargetQuantityUOM = rel.SourceProdOrderPartslistPos.TargetQuantityUOM * batchFraction;
                    childRelation.RetrogradeFIFO = rel.RetrogradeFIFO;
                    childRelation.ProdOrderBatch = batch;
                    batchRelations.Add(childRelation);
                    resultNewEntities.Add(childRelation);
                }

                dbApp.ProdOrderPartslistPos.Add(childPosition);
                batchRelations.ForEach(x => dbApp.ProdOrderPartslistPosRelation.Add(x));
            }

            //PostExecute("BatchCreate");
            return msg;
        }

        public ProdOrderBatchPlan FactoryBatchPlan(
            DatabaseApp databaseApp,
            gip.mes.datamodel.ACClassWF vbACClassWF,
            Partslist partslist,
            ProdOrderPartslist prodOrderPartslist,
            GlobalApp.BatchPlanState startMode,
            int scheduledOrder,
            DateTime? scheduledEndDate,
            MDBatchPlanGroup batchPlanGroup,
            TimeSpan? offsetToEndTime)

        {
            ProdOrderBatchPlan prodOrderBatchPlan = ProdOrderBatchPlan.NewACObject(databaseApp, prodOrderPartslist);
            prodOrderBatchPlan.PlanState = startMode;
            prodOrderBatchPlan.ScheduledOrder = scheduledOrder;
            prodOrderBatchPlan.VBiACClassWF = vbACClassWF;


            var materialWFConnection = GetMaterialWFConnection(vbACClassWF, prodOrderPartslist.Partslist.MaterialWFID);
            var test = materialWFConnection.MaterialWFACClassMethod;
            prodOrderBatchPlan.MaterialWFACClassMethod = partslist.PartslistACClassMethod_Partslist
                                               .Where(c => c.MaterialWFACClassMethod.ACClassMethodID == vbACClassWF.ACClassMethodID)
                                               .Select(c => c.MaterialWFACClassMethod)
                                               .FirstOrDefault();
            prodOrderBatchPlan.ProdOrderPartslistPos = GetIntermediate(prodOrderPartslist, materialWFConnection);


            prodOrderBatchPlan.ScheduledEndDate = scheduledEndDate;
            if (offsetToEndTime != null)
                prodOrderBatchPlan.ScheduledStartDate = prodOrderBatchPlan.ScheduledEndDate - offsetToEndTime.Value;

            prodOrderBatchPlan.MDBatchPlanGroup = batchPlanGroup;
            return prodOrderBatchPlan;
        }

        public bool FactoryProdOrderPartslist(DatabaseApp databaseApp,
            PlanningMR filterPlanningMR,
            WizardSchedulerPartslist wizardSchedulerPartslist,
            ProdOrderPartslist defaultProdOrderPartslist,
            ref string programNo)
        {
            bool success = false;
            ProdOrder prodOrder = null;
            if (string.IsNullOrEmpty(programNo))
            {
                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(ProdOrder), ProdOrder.NoColumnName, ProdOrder.FormatNewNo, this);
                programNo = secondaryKey;
                prodOrder = ProdOrder.NewACObject(databaseApp, null, secondaryKey);
                ACSaveChanges();
            }
            else
            {
                string tmpProgramNo = programNo;
                prodOrder = databaseApp.ProdOrder.FirstOrDefault(c => c.ProgramNo == tmpProgramNo);
            }

            wizardSchedulerPartslist.ProgramNo = programNo;

            if (wizardSchedulerPartslist.ProdOrderPartslist != null)
            {
                success = true;
            }
            else
            {
                Msg msg = null;
                wizardSchedulerPartslist.ProdOrderPartslist = prodOrder.ProdOrderPartslist_ProdOrder.FirstOrDefault(c => c.PartslistID == wizardSchedulerPartslist.Partslist.PartslistID);
                if (wizardSchedulerPartslist.ProdOrderPartslist == null)
                {
                    ProdOrderPartslist tempPl = null;
                    msg = PartslistAdd(databaseApp, prodOrder, wizardSchedulerPartslist.Partslist, wizardSchedulerPartslist.Sn, wizardSchedulerPartslist.NewTargetQuantityUOM, out tempPl);
                    wizardSchedulerPartslist.ProdOrderPartslist = tempPl;
                }

                if (wizardSchedulerPartslist.ProdOrderPartslist != null && defaultProdOrderPartslist != null)
                {
                    CopyStartDepartmentFromMain(wizardSchedulerPartslist.ProdOrderPartslist, defaultProdOrderPartslist);
                }

                success = msg == null || msg.IsSucceded();


                if (filterPlanningMR != null && success)
                {
                    PlanningMR planningMR = databaseApp.PlanningMR.FirstOrDefault(c => c.PlanningMRID == filterPlanningMR.PlanningMRID);
                    PlanningMRProposal proposal = PlanningMRProposal.NewACObject(databaseApp, planningMR);
                    proposal.ProdOrder = prodOrder;
                    proposal.ProdOrderPartslist = wizardSchedulerPartslist.ProdOrderPartslist;
                    planningMR.PlanningMRProposal_PlanningMR.Add(proposal);
                }
            }

            return success;
        }

        public bool FactoryBatchPlans(DatabaseApp databaseApp, PlanningMR filterPlanningMR, GlobalApp.BatchPlanState createdBatchState,
            WizardSchedulerPartslist wizardSchedulerPartslist, out List<ProdOrderBatchPlan> generatedBatchPlans)
        {
            bool success = false;
            ProdOrderBatchPlan firstBatchPlan = null;

            MDProdOrderPartslistPosState mDProdOrderPartslistPosState = databaseApp.MDProdOrderPartslistPosState.FirstOrDefault(c => c.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created);

            generatedBatchPlans = new List<ProdOrderBatchPlan>();
            int nr = 0;
            gip.mes.datamodel.ACClassWF vbACClassWF = wizardSchedulerPartslist.WFNodeMES;
            List<SchedulingMaxBPOrder> maxSchedulerOrders = GetMaxScheduledOrder(databaseApp, filterPlanningMR?.PlanningMRNo);
            int scheduledOrder = 0;
            if (vbACClassWF != null)
                scheduledOrder =
                    maxSchedulerOrders
                    .Where(c => c.MDSchedulingGroup.MDSchedulingGroupID == wizardSchedulerPartslist.SelectedMDSchedulingGroup.MDSchedulingGroupID)
                    .SelectMany(c => c.WFs)
                    .Where(c => c.ACClassWF.ACClassWFID == vbACClassWF.ACClassWFID)
                    .Select(c => c.MaxScheduledOrder)
                    .DefaultIfEmpty()
                    .Max();

            foreach (var item in wizardSchedulerPartslist.BatchPlanSuggestion.ItemsList)
            {
                nr++;
                scheduledOrder++;
                ProdOrderBatchPlan batchPlan = FactoryBatchPlan(databaseApp,
                                                                vbACClassWF,
                                                                wizardSchedulerPartslist.Partslist,
                                                                wizardSchedulerPartslist.ProdOrderPartslist,
                                                                createdBatchState,
                                                                scheduledOrder,
                                                                item.ExpectedBatchEndTime,
                                                                wizardSchedulerPartslist.SelectedBatchPlanGroup,
                                                                wizardSchedulerPartslist.OffsetToEndTime);
                batchPlan.ProdOrderPartslistPos.MDProdOrderPartslistPosState = mDProdOrderPartslistPosState;
                wizardSchedulerPartslist.ProdOrderPartslistPos = batchPlan.ProdOrderPartslistPos;
                WriteBatchPlanQuantities(item, batchPlan);
                batchPlan.Sequence = nr;
                if (firstBatchPlan == null)
                    firstBatchPlan = batchPlan;
                generatedBatchPlans.Add(batchPlan);
            }
            wizardSchedulerPartslist.IsSolved = true;

            success = true;

            return success;
        }

        public (bool success, bool newBatchPlan) UpdateBatchPlans(DatabaseApp databaseApp, WizardSchedulerPartslist wizardSchedulerPartslist, List<ProdOrderBatchPlan> otherBatchPlans)
        {
            bool success = true;
            bool newBatchPlan = false;
            ProdOrderPartslist prodOrderPartslist = wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist;
            MDProdOrderPartslistPosState mDProdOrderPartslistPosState = databaseApp.MDProdOrderPartslistPosState.FirstOrDefault(c => c.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created);
            PartslistACClassMethod method = wizardSchedulerPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();

            List<ProdOrderBatchPlan> existingBatchPlans =
                wizardSchedulerPartslist
                .ProdOrderPartslistPos
                .ProdOrderPartslist
                .ProdOrderBatchPlan_ProdOrderPartslist
                .Where(c => c.VBiACClassWFID == wizardSchedulerPartslist.WFNodeMES.ACClassWFID)
                .ToList();

            int existingMinIndex = existingBatchPlans.Select(c => c.ScheduledOrder ?? 0).DefaultIfEmpty().Min();

            Guid[] wizardBatchPlanIDs = wizardSchedulerPartslist.BatchPlanSuggestion.ItemsList.Where(c => c.ProdOrderBatchPlan != null).Select(c => c.ProdOrderBatchPlan.ProdOrderBatchPlanID).ToArray();
            List<ProdOrderBatchPlan> missingBatchPlans = existingBatchPlans.Where(c => !wizardBatchPlanIDs.Contains(c.ProdOrderBatchPlanID)).ToList();

            int movingStep = wizardSchedulerPartslist.BatchPlanSuggestion.ItemsList.Count;
            otherBatchPlans =
                otherBatchPlans
                .Where(c => (
                                c.ScheduledOrder >= existingMinIndex)
                                && !missingBatchPlans.Select(x => x.ProdOrderBatchPlanID).Contains(c.ProdOrderBatchPlanID)
                                && !existingBatchPlans.Select(x => x.ProdOrderBatchPlanID).Contains(c.ProdOrderBatchPlanID)
                ).ToList();
            int schedulingOrder = 0;
            if (existingMinIndex > 0)
            {
                foreach (ProdOrderBatchPlan plan in otherBatchPlans)
                {
                    plan.ScheduledOrder = schedulingOrder + movingStep + existingMinIndex;
                    schedulingOrder++;
                }
                schedulingOrder = existingMinIndex;
            }
            else
                schedulingOrder = otherBatchPlans.Select(c => c.ScheduledOrder ?? 0).DefaultIfEmpty().Max() + 1;

            foreach (BatchPlanSuggestionItem suggestionItem in wizardSchedulerPartslist.BatchPlanSuggestion.ItemsList)
            {
                ProdOrderBatchPlan batchPlan = null;
                if (suggestionItem.ProdOrderBatchPlan != null)
                {
                    batchPlan = existingBatchPlans.FirstOrDefault(c => c.ProdOrderBatchPlanID == suggestionItem.ProdOrderBatchPlan.ProdOrderBatchPlanID);
                }
                else
                {
                    batchPlan = FactoryBatchPlan(databaseApp, wizardSchedulerPartslist.WFNodeMES, prodOrderPartslist.Partslist, prodOrderPartslist, GlobalApp.BatchPlanState.Created, 0, suggestionItem.ExpectedBatchEndTime, wizardSchedulerPartslist.SelectedBatchPlanGroup, wizardSchedulerPartslist.OffsetToEndTime);
                    prodOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.Add(batchPlan);
                    batchPlan.ProdOrderPartslistPos.MDProdOrderPartslistPosState = mDProdOrderPartslistPosState;
                    newBatchPlan = true;
                }
                WriteBatchPlanQuantities(suggestionItem, batchPlan);

                batchPlan.ScheduledEndDate = suggestionItem.ExpectedBatchEndTime;
                if (wizardSchedulerPartslist.OffsetToEndTime.HasValue)
                    batchPlan.ScheduledStartDate = batchPlan.ScheduledEndDate - wizardSchedulerPartslist.OffsetToEndTime.Value;


                batchPlan.ScheduledOrder = schedulingOrder;
                batchPlan.MDBatchPlanGroup = wizardSchedulerPartslist.SelectedBatchPlanGroup;
                schedulingOrder++;
            }
            foreach (ProdOrderBatchPlan missingBatchPlan in missingBatchPlans)
                missingBatchPlan.DeleteACObject(databaseApp, false);

            ProdOrderBatchPlan firstBatchPlan = prodOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.FirstOrDefault();

            return (success, newBatchPlan);
        }

        public void WriteBatchPlanQuantities(BatchPlanSuggestionItem suggestionItem, ProdOrderBatchPlan batchPlan)
        {
            batchPlan.TotalSize = suggestionItem.TotalBatchSizeUOM;
            batchPlan.BatchSize = suggestionItem.BatchSizeUOM;
            batchPlan.BatchTargetCount = suggestionItem.BatchTargetCount;
        }

        public void CopyStartDepartmentFromMain(ProdOrderPartslist prodOrderPartslist, ProdOrderPartslist mainProdOrderPartslist)
        {
            prodOrderPartslist.DepartmentUserName = mainProdOrderPartslist.DepartmentUserName;
            prodOrderPartslist.StartDate = mainProdOrderPartslist.StartDate;
            prodOrderPartslist.EndDate = mainProdOrderPartslist.EndDate;
        }


        public MsgWithDetails MergeOrders(DatabaseApp databaseApp, List<ProdOrderPartslist> plsForMerge)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();

            ProdOrder[] prodOrders = plsForMerge.Select(c => c.ProdOrder).ToArray();

            // 1.0 Define connections
            // Target PL | OutwardRoot Input Pos | Source PL
            List<Tuple<ProdOrderPartslist, ProdOrderPartslistPos, ProdOrderPartslist>> itemsForMerge = new List<Tuple<ProdOrderPartslist, ProdOrderPartslistPos, ProdOrderPartslist>>();
            foreach (ProdOrderPartslist pl in plsForMerge)
            {
                ProdOrderPartslistPos[] components = pl.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot).ToArray();
                foreach (ProdOrderPartslistPos component in components)
                {
                    ProdOrderPartslist sourcePL = plsForMerge.Where(c => c.ProdOrderPartslistID != pl.ProdOrderPartslistID && c.Partslist.MaterialID == component.MaterialID).FirstOrDefault();
                    if (sourcePL != null)
                    {
                        itemsForMerge.Add(new Tuple<ProdOrderPartslist, ProdOrderPartslistPos, ProdOrderPartslist>(pl, component, sourcePL));
                    }
                }
            }

            // 2.0 process connections: move partslist to order where beeing used>

            return msgWithDetails;
        }

        public MsgWithDetails GenerateBatchPlans(DatabaseApp databaseApp,
                                                 ConfigManagerIPlus configManagerIPlus,
                                                 double roundingQuantity,
                                                 ACComponent routingService,
                                                 string pwClassName,
                                                 List<ProdOrderPartslist> plsForBatchGenerate,
                                                 List<PartslistMDSchedulerGroupConnection> schedulerConnections)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            foreach (ProdOrderPartslist item in plsForBatchGenerate)
            {
                MsgWithDetails tmp = GenerateBatchPlan(databaseApp, configManagerIPlus, roundingQuantity, routingService, pwClassName, item, schedulerConnections);
                if (tmp != null && tmp.MsgDetails.Any())
                    foreach (Msg msg in tmp.MsgDetails)
                        msgWithDetails.AddDetailMessage(msg);
            }
            return msgWithDetails;
        }

        public MsgWithDetails GenerateBatchPlan(DatabaseApp databaseApp,
                                                ConfigManagerIPlus configManagerIPlus,
                                                double roundingQuantity,
                                                ACComponent routingService,
                                                string pwClassName,
                                                ProdOrderPartslist plForBatchGenerate,
                                                List<PartslistMDSchedulerGroupConnection> schedulerConnections)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();


            ProdOrder prodOrder = databaseApp.ProdOrder.Where(c => c.ProdOrderID == plForBatchGenerate.ProdOrderID).FirstOrDefault();

            prodOrder.AutoRefresh();
            prodOrder.ProdOrderPartslist_ProdOrder.AutoLoad(prodOrder.ProdOrderPartslist_ProdOrderReference, prodOrder);

            List<ProdOrderPartslist> prodOrderPartslists =
                databaseApp
                .ProdOrderPartslist
                .Where(c => c.ProdOrderID == prodOrder.ProdOrderID)
                .OrderByDescending(c => c.Sequence)
                .ToList();

            int countOfPl = prodOrderPartslists.Count();

            if (countOfPl == 1)
            {
                // 1.0 make BOM - create partslists
                double treeQuantityRatio = plForBatchGenerate.TargetQuantity / plForBatchGenerate.Partslist.TargetQuantityUOM;
                PartslistExpand rootPartslistExpand = new PartslistExpand(plForBatchGenerate.Partslist, 1, treeQuantityRatio);
                rootPartslistExpand.IsChecked = true;
                rootPartslistExpand.LoadTree();

                // 2.0 Extract suggestion
                List<ExpandResult> treeResult = rootPartslistExpand.BuildTreeList();
                treeResult =
                    treeResult
                    .Where(x =>
                        x.Item.IsChecked
                        && x.Item.IsEnabled)
                    .OrderByDescending(x => x.TreeVersion)
                    .ToList();

                int sn = 0;
                foreach (ExpandResult expand in treeResult)
                {
                    sn++;
                    PartslistExpand partslistExpand = expand.Item as PartslistExpand;
                    ProdOrderPartslist pl = prodOrder.ProdOrderPartslist_ProdOrder.FirstOrDefault(c => c.PartslistID == partslistExpand.Partslist.PartslistID);
                    if (pl == null)
                    {
                        PartslistAdd(databaseApp, prodOrder, partslistExpand.Partslist, sn, partslistExpand.TargetQuantityUOM, out pl);
                        if (pl != null)
                        {
                            pl.Sequence = sn;
                            prodOrderPartslists.Add(pl);
                        }
                    }
                }
                ConnectSourceProdOrderPartslist(prodOrder);
                CorrectSortOrder(prodOrder);
                ACSaveChanges();
                prodOrderPartslists = prodOrderPartslists.OrderByDescending(c => c.Sequence).ToList();
            }

            // 2.1 Fit quantities ProdOrderPartslist.TargetQuantity => FinalMix.TargetQuantity
            foreach (ProdOrderPartslist prodOrderPartslist in prodOrderPartslists)
            {
                ProdOrderPartslistPos finalMix = prodOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.IsFinalMixure).FirstOrDefault();
                if (finalMix != null)
                {
                    finalMix.TargetQuantityUOM = prodOrderPartslist.TargetQuantity;
                }
            }

            List<WizardSchedulerPartslist> wPls = new List<WizardSchedulerPartslist>();

            foreach (ProdOrderPartslist prodOrderPartslist in prodOrderPartslists)
            {
                bool plHaveBatchPlanOrBatch = prodOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.Any();
                double plannedBatchQuantity = prodOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.Select(c => c.TotalSize).DefaultIfEmpty().Sum();
                bool differentQuantity = plannedBatchQuantity > 0 && Math.Abs(prodOrderPartslist.TargetQuantity - plannedBatchQuantity) > double.Epsilon;

                if (!plHaveBatchPlanOrBatch || differentQuantity)
                {
                    List<MDSchedulingGroup> schedulingGroups = GetSchedulingGroups(databaseApp, pwClassName, prodOrderPartslist.Partslist, schedulerConnections);
                    WizardSchedulerPartslist item =
                        new WizardSchedulerPartslist(
                            databaseApp,
                            this,
                            configManagerIPlus,
                            roundingQuantity,
                            prodOrderPartslist.Partslist,
                            prodOrderPartslist.TargetQuantity,
                            prodOrderPartslist.Sequence,
                            schedulingGroups,
                            prodOrderPartslist);

                    item.LoadConfiguration();

                    if (!plHaveBatchPlanOrBatch)
                    {
                        item.LoadNewBatchSuggestion(wPls.ToArray());

                        // add message for not generated 
                        if (item.BatchPlanSuggestion.ItemsList == null || item.BatchPlanSuggestion.ItemsList.Count == 0)
                        {
                            // Warning50050
                            // For prodorder {0} recipe #{1} {2} {3} no batch plan generated!
                            Msg msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "GenerateBatchPlan()", 1137, "Warning50050",
                                prodOrderPartslist.ProdOrder.ProgramNo, prodOrderPartslist.Sequence, prodOrderPartslist.Partslist.Material.MaterialNo, prodOrderPartslist.Partslist.Material.MaterialName1);
                            msgWithDetails.AddDetailMessage(msg);
                        }
                    }
                    else
                    {
                        item.LoadExistingBatchSuggestion();
                        item.NewTargetQuantityUOM = prodOrderPartslist.TargetQuantity;
                    }
                    wPls.Add(item);
                }
            }

            // WizardSchedulerPartslist defaultWizardPl = wPls.Where(c => c.PartslistNo == plForBatchGenerate.Partslist.PartslistNo).FirstOrDefault();

            foreach (WizardSchedulerPartslist wPl in wPls)
            {
                if (wPl.ProdOrderPartslistPos != null && wPl.ProdOrderPartslistPos.ProdOrderPartslist != plForBatchGenerate && plForBatchGenerate != null)
                {
                    CopyStartDepartmentFromMain(wPl.ProdOrderPartslistPos.ProdOrderPartslist, plForBatchGenerate);
                }
            }

            ConnectSourceProdOrderPartslist(prodOrder);
            CorrectSortOrder(prodOrder);

            string programNo = prodOrder.ProgramNo;
            // 4.0 define targets
            List<ProdOrderBatchPlan> prodOrderBatchPlans = new List<ProdOrderBatchPlan>();
            foreach (WizardSchedulerPartslist wPl in wPls)
            {
                if (!wPl.ProdOrderPartslistPos.ProdOrderBatchPlan_ProdOrderPartslistPos.Any() && wPl.BatchPlanSuggestion.ItemsList.Any())
                {
                    FactoryProdOrderPartslist(databaseApp, null, wPl, plForBatchGenerate, ref programNo);
                    FactoryBatchPlans(databaseApp, null, GlobalApp.BatchPlanState.Created, wPl, out prodOrderBatchPlans);
                }
                else
                {
                    if (wPl.BatchPlanSuggestion.ItemsList.Any())
                    {
                        wPl.BatchPlanSuggestion.UpdateBatchPlan();
                        List<ProdOrderBatchPlan> otherBatchPlans = new List<ProdOrderBatchPlan>();
                        UpdateBatchPlans(databaseApp, wPl, otherBatchPlans);
                    }
                }
            }

            // 5.1 If many target - select first and remind
            foreach (WizardSchedulerPartslist wPl in wPls)
            {
                foreach (ProdOrderBatchPlan bp in wPl.ProdOrderPartslistPos.ProdOrderBatchPlan_ProdOrderPartslistPos)
                {
                    if (!bp.FacilityReservation_ProdOrderBatchPlan.Any())
                    {
                        Msg addTargetMsg = BatchPlanSelectTarget(databaseApp, configManagerIPlus, routingService, wPl.WFNodeMES, wPl.ProdOrderPartslistPos, bp);
                        if (addTargetMsg != null)
                            msgWithDetails.AddDetailMessage(addTargetMsg);
                    }
                }
            }

            MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
            if (saveMsg != null)
                foreach (Msg tmMsg in saveMsg.MsgDetails)
                    msgWithDetails.AddDetailMessage(tmMsg);

            return msgWithDetails;
        }

        public virtual Msg BatchPlanSelectTarget(DatabaseApp databaseApp,
                                         ConfigManagerIPlus configManagerIPlus,
                                         ACComponent routingService,
                                         datamodel.ACClassWF wfNodeMES,
                                         ProdOrderPartslistPos pos,
                                         ProdOrderBatchPlan bp)
        {
            Msg msg = null;
            string configUrl = bp.IplusVBiACClassWF.ConfigACUrl;
            BindingList<POPartslistPosReservation> targets = GetTargets(databaseApp, configManagerIPlus, routingService, wfNodeMES, pos.ProdOrderPartslist, pos, bp, configUrl, true, false, true, false, false);

            SetBatchPlanTargets(databaseApp, bp, targets);

            if (!targets.Any(c => c.IsChecked))
            {
                // Warning50051
                // For prodorder {0} recipe #{1} {2} {3} no target for batch plan!
                msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "GenerateBatchPlan()", 1137, "Warning50051",
                    bp.ProdOrderPartslist.ProdOrder.ProgramNo, bp.ProdOrderPartslist.Sequence, bp.ProdOrderPartslist.Partslist.Material.MaterialNo, bp.ProdOrderPartslist.Partslist.Material.MaterialName1);
            }
            return msg;
        }

        public virtual void SetBatchPlanTargets(DatabaseApp databaseApp, ProdOrderBatchPlan bp, BindingList<POPartslistPosReservation> targets)
        {
            if (!targets.Any(c => c.IsChecked))
            {
                POPartslistPosReservation selectedReservation = targets.FirstOrDefault();
                if (selectedReservation != null)
                    selectedReservation.IsChecked = true;
            }
        }

        public gip.core.datamodel.ACClassWF GetACClassWFDischarging(DatabaseApp databaseApp, ProdOrderPartslist prodOrderPartslist, gip.mes.datamodel.ACClassWF vbACClassWF, ProdOrderPartslistPos intermediatePos)
        {

            // TODO: Benutzerauswahl, mit welchem Steuerrezept gefahren werden soll (nicht .FirstOrDefault()):
            PartslistACClassMethod selectedProcessWF = null;
            if (vbACClassWF != null)
                selectedProcessWF = prodOrderPartslist.Partslist.PartslistACClassMethod_Partslist.Where(c => c.MaterialWFACClassMethod.ACClassMethodID == vbACClassWF.ACClassMethodID).FirstOrDefault();
            if (selectedProcessWF == null)
                prodOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
            if (selectedProcessWF == null)
                return null;

            MaterialWFACClassMethod materialWFACClassMethod = selectedProcessWF.MaterialWFACClassMethod;
            //MaterialWFACClassMethod materialWFACClassMethod = currentProdOrderPartslist.Partslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.FirstOrDefault();
            if (materialWFACClassMethod == null)
            {
                return null;
            }
            gip.core.datamodel.ACClass acClassOfPWDischarging = null;
            gip.core.datamodel.ACClassWF acClassWFDischarging = null;

            var matWFConnections = intermediatePos.Material.MaterialWFConnection_Material
                .Where(c => c.MaterialWFACClassMethodID == materialWFACClassMethod.MaterialWFACClassMethodID
                            && c.ACClassWFID != vbACClassWF.ACClassWFID);
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

        public virtual BindingList<POPartslistPosReservation> GetTargets(DatabaseApp databaseApp, ConfigManagerIPlus configManager, ACComponent routingService, gip.mes.datamodel.ACClassWF vbACClassWF,
            ProdOrderPartslist prodorderPartslist, ProdOrderPartslistPos intermediatePos, ProdOrderBatchPlan batchPlan, string configACUrl,
            bool showCellsInRoute, bool showSelectedCells, bool showEnabledCells, bool showSameMaterialCells, bool preselectFirstReservation)
        {
            BindingList<POPartslistPosReservation> reservationCollection = new BindingList<POPartslistPosReservation>();
            gip.core.datamodel.ACClassWF acClassWFDischarging = GetACClassWFDischarging(databaseApp, prodorderPartslist, vbACClassWF, intermediatePos);

            if (acClassWFDischarging != null && batchPlan != null)
            {
                List<Route> routes = new List<Route>();

                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    RoutingService = routingService,
                    Database = databaseApp.ContextIPlus,
                    AttachRouteItemsToContext = true,
                    SelectionRuleID = "Storage",
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

                short? destinationFilterClassCode = null;
                #region Filter routes if is selected  (ShowCellsInRoute)
                core.datamodel.ACClassWF aCClassWF = vbACClassWF.FromIPlusContext<gip.core.datamodel.ACClassWF>(databaseApp.ContextIPlus);
                bool checkShowCellsInRoute = showCellsInRoute && acClassWFDischarging != null && acClassWFDischarging.ACClassWF1_ParentACClassWF != null;
                if (checkShowCellsInRoute)
                {
                    List<IACConfigStore> mandatoryConfigStores =
                    GetCurrentConfigStores(
                        aCClassWF,
                        vbACClassWF,
                        prodorderPartslist.Partslist.MaterialWFID,
                        prodorderPartslist.Partslist,
                        prodorderPartslist
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

                    IACConfig configClassCode = configManager.GetConfiguration(
                            mandatoryConfigStores,
                            configACUrl + "\\",
                            acClassWFDischarging.ConfigACUrl + ACUrlHelper.Delimiter_DirSeperator + ACStateConst.SMStarting + ACUrlHelper.Delimiter_DirSeperator + nameof(Facility.ClassCode),
                            null,
                            out priorityLevel);
                    if (configClassCode != null)
                    {
                        destinationFilterClassCode = (short)configClassCode.Value;
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
                            batchPlan.EntityState != EntityState.Added && batchPlan.FacilityReservation_ProdOrderBatchPlan != null
                            && !batchPlan.FacilityReservation_ProdOrderBatchPlan.Any(c => c.EntityState != EntityState.Unchanged)
                        )
                    {
                        batchPlan.FacilityReservation_ProdOrderBatchPlan.AutoRefresh(batchPlan.FacilityReservation_ProdOrderBatchPlanReference, batchPlan);
                        selectedModules = batchPlan.Context.Entry(batchPlan)
                            .Collection(c => c.FacilityReservation_ProdOrderBatchPlan)
                            .Query()
                            .Include(c => c.Facility)
                            .Include(c => c.Facility.Material)
                            .AutoMergeOption()
                            .ToArray();
                    }
                    else
                    {
                        if (batchPlan.FacilityReservation_ProdOrderBatchPlan != null)
                        {
                            selectedModules = batchPlan.FacilityReservation_ProdOrderBatchPlan.ToArray();
                        }
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
                                        && Material.IsMaterialEqual(intermediatePos.BookingMaterial, unselFacility.Material);
                                //&& unselFacility.Material != null
                                //&& (
                                //       intermediatePos.BookingMaterial != null
                                //    && ((unselFacility.MaterialID == intermediatePos.BookingMaterial.MaterialID)
                                //        || (intermediatePos.BookingMaterial.ProductionMaterialID.HasValue && intermediatePos.BookingMaterial.ProductionMaterialID.Value == unselFacility.MaterialID))
                                // );
                                if (showSameMaterialCells && !ifMaterialMatch)
                                    continue;
                                if (unselFacility != null && destinationFilterClassCode.HasValue && unselFacility.ClassCode > 0)
                                {
                                    uint bitCmpResult = ((uint)unselFacility.ClassCode) & ((uint)destinationFilterClassCode.Value);
                                    if (bitCmpResult == 0)
                                        continue;
                                }
                            }
                            reservationCollection.Add(new POPartslistPosReservation(routeItem.Target, batchPlan, null, selectedReservationForModule, unselFacility, acClassWFDischarging, aCClassWF));
                        }
                    }

                    // select first if only one is present
                    if (preselectFirstReservation
                        && ((batchPlan.EntityState == EntityState.Added && reservationCollection.Count() == 1)
                            || ((batchPlan.EntityState == EntityState.Unchanged || batchPlan.EntityState == EntityState.Modified)
                                    && reservationCollection.Count() == 1
                                    && !reservationCollection.Any(c => c.IsChecked))))
                        reservationCollection[0].IsChecked = true;
                }
            }
            return reservationCollection;
        }

        public List<IACConfigStore> GetCurrentConfigStores(gip.core.datamodel.ACClassWF currentACClassWF, gip.mes.datamodel.ACClassWF vbCurrentACClassWF, Guid? materialWFID, Partslist partslist, ProdOrderPartslist prodOrderPartslist)
        {
            List<IACConfigStore> configStores = new List<IACConfigStore>();
            if (prodOrderPartslist != null)
            {
                configStores.Add(prodOrderPartslist);
            }

            if (partslist != null)
            {
                configStores.Add(partslist);
            }

            MaterialWFConnection matWFConnection = GetMaterialWFConnection(vbCurrentACClassWF, materialWFID);
            if (matWFConnection != null)
            {
                configStores.Add(matWFConnection.MaterialWFACClassMethod);
            }

            configStores.Add(currentACClassWF.ACClassMethod);

            if (currentACClassWF.RefPAACClassMethod != null)
            {
                configStores.Add(currentACClassWF.RefPAACClassMethod);
            }

            return configStores;
        }

        public void GetFacilityLotForPos(IACEntityObjectContext databaseForNumGen, DatabaseApp dbApp, ProdOrderPartslistPos pos, bool createNewLot, out FacilityLot facilityLot, out string lotNo, LotCreationModeEnum? lotCreationMode = null)
        {
            facilityLot = null;
            lotNo = null;
            if (pos == null)
                return;
            if (lotCreationMode == null)
                lotCreationMode = LotCreationMode;
            if (lotCreationMode.Value == LotCreationModeEnum.OneOrderOneLotWithOrderNo || lotCreationMode.Value == LotCreationModeEnum.OneOrderOneLotWithLotNo)
            {
                facilityLot = dbApp.ProdOrderPartslistPos.Where(c => c.ProdOrderPartslistID == pos.ProdOrderPartslistID && c.FacilityLotID.HasValue).Select(c => c.FacilityLot).FirstOrDefault();
                if (facilityLot != null)
                {
                    lotNo = facilityLot.LotNo;
                    if (createNewLot)
                        pos.FacilityLot = facilityLot;
                    return;
                }
            }
            string tempLotNo = null;
            if (lotCreationMode.Value == LotCreationModeEnum.OneOrderOneLotWithOrderNo)
            {
                tempLotNo = pos.ProdOrderPartslist.ProdOrder.ProgramNo + "-" + pos.ProdOrderPartslist.Sequence.ToString();
                facilityLot = dbApp.FacilityLot.Where(c => c.LotNo == tempLotNo).FirstOrDefault();
            }
            else
            {
                tempLotNo = Root.NoManager.GetNewNo(Database, typeof(FacilityLot), FacilityLot.NoColumnName, FacilityLot.FormatNewNo, this);
            }
            lotNo = tempLotNo;

            if (createNewLot)
            {
                if (facilityLot == null)
                {
                    facilityLot = FacilityLot.NewACObject(dbApp, null, tempLotNo);
                    facilityLot.UpdateExpirationInfo(pos.BookingMaterial);
                }
                pos.FacilityLot = facilityLot;
            }
        }

        public virtual void ValidateChangedPosReservation(VBBSOModulesSelector selector, DatabaseApp databaseApp, POPartslistPosReservation poReservation, object sender, PropertyChangedEventArgs e)
        {
        }
        #endregion

        #region ProdOrder -> Batch

        #region Batch cascade creation

        #region Batch cascade creation -> Public methods

        /// <summary>
        /// Generate list of connected intermediates 
        /// for selection in batch cascade creation process
        /// </summary>
        /// <param name="intermediate"></param>
        /// <returns></returns>
        public List<PosIntermediateDepthWrap> BatchCreateBuildIntermediateIncludedList(ProdOrderPartslistPos intermediate, MaterialWFACClassMethod materialWFACClassMethod)
        {
            List<PosIntermediateDepthWrap> list = new List<PosIntermediateDepthWrap>();
            BatchCreateBuildIntermediateIncludedList(intermediate, 1, list, intermediate.TargetQuantityUOM, materialWFACClassMethod);
            return list;
        }

        /// <summary>
        /// Create a cascade of batches
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="batchPercentageDefinition"></param>
        /// <param name="intermediateList"></param>
        /// <returns></returns>
        public Msg BatchCreateCascade(DatabaseApp dbApp, List<BatchPercentageModel> batchPercentageDefinition, List<PosIntermediateDepthWrap> intermediateList, RestHandleModeEnum calculationModel, List<object> createdEntities, int roundingDecimalPlaces = 2)
        {
            ProdOrderPartslist mandatoryPartslist = intermediateList.FirstOrDefault().Intermediate.ProdOrderPartslist;
            // Two list in game
            // 1. Batch list
            // 2. Intermediate List 

            // Creation definition
            List<IntermediateBatchQuantityConnection> defQuantityDistribution =
                intermediateList.Select(x => new IntermediateBatchQuantityConnection()
                {
                    IntermediateWarp = x,
                    BatchPercentageDefinition = batchPercentageDefinition,
                    BatchQuantityDefinition = BatchSizeCalculation.GetQuantityModel(x.TargetQuantityUOM, batchPercentageDefinition, calculationModel, roundingDecimalPlaces)
                }).ToList();

            foreach (BatchPercentageModel batchDef in batchPercentageDefinition)
            {
                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(ProdOrderBatch), ProdOrderBatch.NoColumnName, ProdOrderBatch.FormatNewNo, this);
                ProdOrderBatch batch = ProdOrderBatch.NewACObject(dbApp, mandatoryPartslist, secondaryKey);
                batch.BatchSeqNo = batchDef.Sequence;
                mandatoryPartslist.ProdOrderBatch_ProdOrderPartslist.Add(batch);
                createdEntities.Add(batch);
                foreach (IntermediateBatchQuantityConnection connItem in defQuantityDistribution)
                {
                    ProdOrderPartslistPos childPosition = BatchCreatePos(dbApp, connItem.IntermediateWarp.Intermediate, batch, connItem.BatchQuantityDefinition[batchDef.Sequence - 1]);
                    createdEntities.Add(childPosition);

                    List<ProdOrderPartslistPosRelation> relations = dbApp.ProdOrderPartslistPosRelation
                            .Where(x => x.TargetProdOrderPartslistPosID == connItem.IntermediateWarp.Intermediate.ProdOrderPartslistPosID).ToList();
                    if (relations != null && relations.Any())
                    {
                        foreach (ProdOrderPartslistPosRelation relation in relations)
                        {
                            double quantityFactor = connItem.IntermediateWarp.Intermediate.TargetQuantityUOM > 0 ?
                                childPosition.TargetQuantityUOM / connItem.IntermediateWarp.Intermediate.TargetQuantityUOM
                                : 0;
                            ProdOrderPartslistPosRelation batchRelation = BatchCreateRelation(dbApp, batch, childPosition, relation, quantityFactor);
                            createdEntities.Add(batchRelation);

                        }
                    }
                }
            }
            return null;
        }

        #endregion

        #region Batch cascade creation -> Helper methods

        /// <summary>
        ///  Created tree of included intermediates 
        ///  with applyed quantity in relation
        /// </summary>
        /// <param name="intermediate"></param>
        /// <param name="depth"></param>
        /// <param name="list"></param>
        /// <param name="targetQuantityUOM"></param>
        private void BatchCreateBuildIntermediateIncludedList(ProdOrderPartslistPos intermediate, int depth, List<PosIntermediateDepthWrap> list, double targetQuantityUOM, MaterialWFACClassMethod materialWFACClassMethod)
        {
            list.Add(new PosIntermediateDepthWrap() { Depth = depth, Selected = true, Intermediate = intermediate, TargetQuantityUOM = targetQuantityUOM });
            depth++;
            List<Guid> sharedGroup = intermediate.Material.MaterialWFConnection_Material.Select(x => x.ACClassWF).Select(x => x.RefPAACClassMethodID ?? Guid.Empty).ToList();
            var intermediateInList = intermediate
                .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                .Where(x =>
                        x.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.InwardIntern
                        && !x.SourceProdOrderPartslistPos.ParentProdOrderPartslistPosID.HasValue
                        && x.SourceProdOrderPartslistPos.Material.MaterialWFConnection_Material
                                            .Where(d =>
                                                    d.MaterialWFACClassMethodID == materialWFACClassMethod.MaterialWFACClassMethodID
                                            // && sharedGroup.Contains(d.ACClassWF.ACClassMethodID)
                                            ).Any()
                        )
                .Select(x => new { Pos = x.SourceProdOrderPartslistPos, TargetQuantityUOM = x.TargetQuantityUOM })
                .ToList();
            if (intermediateInList != null && intermediateInList.Any())
                foreach (var intermediateIn in intermediateInList)
                    BatchCreateBuildIntermediateIncludedList(intermediateIn.Pos, depth, list, intermediateIn.TargetQuantityUOM, materialWFACClassMethod);
        }

        private ProdOrderPartslistPos BatchCreatePos(DatabaseApp dbApp, ProdOrderPartslistPos intermediate, ProdOrderBatch batch, BatchQuantityModel batchQuantityDefinition)
        {
            ProdOrderPartslistPos childPosition = ProdOrderPartslistPos.NewACObject(dbApp, intermediate);
            childPosition.Sequence = batchQuantityDefinition.Sequence;
            childPosition.TargetQuantityUOM = batchQuantityDefinition.TargetQuantity;
            intermediate.CalledUpQuantityUOM += childPosition.TargetQuantityUOM;
            childPosition.ProdOrderBatch = batch;
            intermediate.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Add(childPosition);
            return childPosition;
        }

        private ProdOrderPartslistPosRelation BatchCreateRelation(DatabaseApp dbApp, ProdOrderBatch batch, ProdOrderPartslistPos childPosition, ProdOrderPartslistPosRelation parentRelation, double quantityFactor)
        {
            ProdOrderPartslistPosRelation childRelation = ProdOrderPartslistPosRelation.NewACObject(dbApp, parentRelation);
            childRelation.Sequence = parentRelation.Sequence;
            childRelation.TargetProdOrderPartslistPos = childPosition;
            childRelation.SourceProdOrderPartslistPos = parentRelation.SourceProdOrderPartslistPos;
            childRelation.TargetQuantityUOM = parentRelation.TargetQuantityUOM * quantityFactor;
            childRelation.RetrogradeFIFO = parentRelation.RetrogradeFIFO;
            childRelation.ProdOrderBatch = batch;
            return childRelation;
        }


        #endregion

        #endregion

        #region ProdOrder -> Batch -> Start


        public virtual void ActivateProdOrderStatesIntoProduction(DatabaseApp databaseApp, ProdOrderBatchPlan prodOrderBatchPlan, ProdOrderPartslist prodOrderPartslist, ProdOrderPartslistPos intermediate, bool setAutoStartOnBatchplan)
        {
            if ((prodOrderPartslist.ProdOrder.MDProdOrderState == null || prodOrderPartslist.ProdOrder.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.InProduction)
                || (prodOrderPartslist.MDProdOrderState == null || prodOrderPartslist.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.InProduction))
            {
                MDProdOrderState mDProdOrderStateInProduction = DatabaseApp.s_cQry_GetMDProdOrderState(databaseApp, MDProdOrderState.ProdOrderStates.InProduction).FirstOrDefault();
                if (prodOrderPartslist.ProdOrder.MDProdOrderState == null || prodOrderPartslist.ProdOrder.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.InProduction)
                    prodOrderPartslist.ProdOrder.MDProdOrderState = mDProdOrderStateInProduction;
                if (prodOrderPartslist.MDProdOrderState == null || prodOrderPartslist.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.InProduction)
                    prodOrderPartslist.MDProdOrderState = mDProdOrderStateInProduction;
            }
            if (prodOrderPartslist.StartDate == null)
                prodOrderPartslist.StartDate = DateTime.Now;
            if (setAutoStartOnBatchplan)
            {
                intermediate.MDProdOrderPartslistPosState = DatabaseApp.s_cQry_GetMDProdOrderPosState(databaseApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.AutoStart).FirstOrDefault();
                if (prodOrderBatchPlan.PlanState != GlobalApp.BatchPlanState.AutoStart)
                    prodOrderBatchPlan.PlanState = GlobalApp.BatchPlanState.AutoStart;
                prodOrderBatchPlan.PlannedStartDate = DateTimeUtils.NowDST;
            }
        }

        public ProdOrderPartslistPos GetIntermediate(ProdOrderPartslist prodOrderPartslist, MaterialWFConnection matWFConnection)
        {
            return
                prodOrderPartslist
                .ProdOrderPartslistPos_ProdOrderPartslist
                .Where(c =>
                    c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                    && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                    && !c.ParentProdOrderPartslistPosID.HasValue
                    )
                .FirstOrDefault();
        }

        public MaterialWFConnection GetMaterialWFConnection(gip.mes.datamodel.ACClassWF aCClassWF, Guid? materialWFID)
        {
            return
                aCClassWF
                .MaterialWFConnection_ACClassWF
                .Where(c => c.MaterialWFACClassMethod.MaterialWFID == materialWFID)
                .FirstOrDefault();
        }

        public MsgWithDetails StartBatchPlan(DatabaseApp databaseApp, gip.core.datamodel.ACClassMethod acClassMethod,
                                            gip.mes.datamodel.ACClassWF VbACClassWf, ProdOrderBatchPlan prodOrderBatchPlan,
                                            bool checkIfWorkflowIsRunning = true)
        {
            gip.core.datamodel.ACProject project = acClassMethod.ACClass.ACProject as gip.core.datamodel.ACProject;
            var managerList = this.Root.FindChildComponents(project.RootClass, 1);
            ACComponent appManager = managerList.FirstOrDefault() as ACComponent;

            return StartBatchPlan(appManager, databaseApp, acClassMethod, VbACClassWf, prodOrderBatchPlan, checkIfWorkflowIsRunning);
        }

        public MsgWithDetails StartBatchPlan(ACComponent appManager, DatabaseApp databaseApp,
                                            gip.core.datamodel.ACClassMethod acClassMethod, gip.mes.datamodel.ACClassWF VbACClassWf, ProdOrderBatchPlan prodOrderBatchPlan,
                                            bool checkIfWorkflowIsRunning = true)
        {
            ProdOrderPartslist prodOrderPartslist = prodOrderBatchPlan.ProdOrderPartslist;
            MaterialWFConnection matWFConnection = GetMaterialWFConnection(VbACClassWf, prodOrderPartslist.Partslist.MaterialWFID);
            ProdOrderPartslistPos intermediate = GetIntermediate(prodOrderPartslist, matWFConnection);

            return StartBatchPlan(databaseApp, appManager, prodOrderPartslist, intermediate, acClassMethod, checkIfWorkflowIsRunning);
        }

        public MsgWithDetails StartBatchPlan(DatabaseApp databaseApp, ACComponent appManager,
                                            ProdOrderPartslist prodOrderPartslist, ProdOrderPartslistPos intermediate, gip.core.datamodel.ACClassMethod acClassMethod,
                                            bool checkIfWorkflowIsRunning = true)
        {
            if (checkIfWorkflowIsRunning)
            {
                var acProgramIDs = databaseApp.OrderLog.Where(c => c.ProdOrderPartslistPosID.HasValue
                                                         && c.ProdOrderPartslistPos.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID)
                                              .Select(c => c.VBiACProgramLog.ACProgramID)
                                              .Distinct()
                                              .ToArray();

                if (acProgramIDs != null && acProgramIDs.Any())
                {
                    ChildInstanceInfoSearchParam searchParam = new ChildInstanceInfoSearchParam() { OnlyWorkflows = true, ACProgramIDs = acProgramIDs };
                    var childInstanceInfos = appManager.GetChildInstanceInfo(1, searchParam);
                    if (childInstanceInfos != null && childInstanceInfos.Any())
                    {
                        var childInstanceInfo = childInstanceInfos.FirstOrDefault();
                        string acUrlComand = String.Format("{0}\\{1}!{2}", childInstanceInfo.ACUrlParent, childInstanceInfo.ACIdentifier, ReloadBPAndResumeACIdentifier);
                        appManager.ACUrlCommand(acUrlComand);
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
            databaseApp.ContextIPlus.ACProgram.Add(program);
            //CurrentProdOrderPartslist.VBiACProgramID = program.ACProgramID;
            MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
            if (saveMsg == null)
            {
                ACValue paramProgram = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACProgram.ClassName);
                if (paramProgram == null)
                    acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACProgram.ClassName, typeof(Guid), program.ACProgramID));
                else
                    paramProgram.Value = program.ACProgramID;

                if (intermediate != null)
                {
                    ACValue acValuePPos = acMethod.ParameterValueList.GetACValue(ProdOrderPartslistPos.ClassName);
                    if (acValuePPos == null)
                        acMethod.ParameterValueList.Add(new ACValue(ProdOrderPartslistPos.ClassName, typeof(Guid), intermediate.ProdOrderPartslistPosID));
                    else
                        acValuePPos.Value = intermediate.ProdOrderPartslistPosID;
                }

                appManager.ExecuteMethod(acClassMethod.ACIdentifier, acMethod);
            }
            return saveMsg;
        }

        #endregion

        #region Public -> Batch -> Duration Calculation
        public TimeSpan? GetCalculatedBatchPlanDuration(DatabaseApp databaseApp, Guid materialWFACClassMethodID, Guid vBiACClassWFID)
        {
            TimeSpan? duration = null;
            ProdOrderBatchPlan prevousBatchPlan =
                databaseApp
                .ProdOrderBatchPlan
                .Where(c =>
                    c.MaterialWFACClassMethodID == materialWFACClassMethodID &&
                    c.VBiACClassWFID == vBiACClassWFID &&
                    c.ProdOrderPartslistPos.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed &&
                    c.ProdOrderPartslistPos.OrderLog_ProdOrderPartslistPos.Any()
                  )
                .OrderByDescending(c => c.InsertDate)
                .FirstOrDefault();
            if (prevousBatchPlan != null)
            {
                ProdOrderBatch firstBatch = prevousBatchPlan.ProdOrderBatch_ProdOrderBatchPlan.OrderBy(c => c.InsertDate).FirstOrDefault();
                ProdOrderBatch lastBatch = prevousBatchPlan.ProdOrderBatch_ProdOrderBatchPlan.OrderByDescending(c => c.InsertDate).FirstOrDefault();

                DateTime? startTime = firstBatch.ProdOrderPartslistPos_ProdOrderBatch.SelectMany(c => c.OrderLog_ProdOrderPartslistPos).Min(c => c.VBiACProgramLog.StartDate);
                DateTime? endTime = lastBatch.ProdOrderPartslistPos_ProdOrderBatch.SelectMany(c => c.OrderLog_ProdOrderPartslistPos).Max(c => c.VBiACProgramLog.StartDate);
                if (startTime != null && endTime != null)
                    duration = endTime - startTime;
            }
            return duration;
        }

        #endregion

        #region Batch -> Select batch
        protected static readonly Func<DatabaseApp, Guid?, short, short, DateTime?, DateTime?, short?, short?, Guid?, Guid?, string, string, IEnumerable<ProdOrderBatchPlan>> s_cQry_BatchPlansForPWNode =
        EF.CompileQuery<DatabaseApp, Guid?, short, short, DateTime?, DateTime?, short?, short?, Guid?, Guid?, string, string, IEnumerable<ProdOrderBatchPlan>>(
            (ctx, mdSchedulingGroupID, fromPlanState, toPlanState, filterStartTime, filterEndTime, minProdOrderState, maxProdOrderState, planningMRID, mdBatchPlanGroup, programNo, materialNo) =>
                                    ctx.ProdOrderBatchPlan
                                    .Include("ProdOrderPartslist")
                                    .Include("ProdOrderPartslist.MDProdOrderState")
                                    .Include("ProdOrderPartslist.ProdOrder")
                                    .Include("ProdOrderPartslist.ProdOrder.MDProdOrderState")
                                    .Include("ProdOrderPartslist.ProdOrder.ProdOrderPartslist_ProdOrder")
                                    .Include("ProdOrderPartslist.Partslist")
                                    .Include("ProdOrderPartslist.Partslist.Material")
                                    .Include("ProdOrderPartslist.Partslist.Material.BaseMDUnit")
                                    .Include("ProdOrderPartslist.Partslist.Material.MaterialUnit_Material")
                                    .Where(c =>
                                            (mdSchedulingGroupID == null || c.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Any(x => x.MDSchedulingGroupID == (mdSchedulingGroupID ?? Guid.Empty)))
                                            && c.PlanStateIndex >= fromPlanState
                                            && c.PlanStateIndex <= toPlanState
                                            && (string.IsNullOrEmpty(programNo) || c.ProdOrderPartslist.ProdOrder.ProgramNo.Contains(programNo))
                                            && (
                                                    string.IsNullOrEmpty(materialNo)
                                                    || (c.ProdOrderPartslist.Partslist.Material.MaterialNo.Contains(materialNo) || c.ProdOrderPartslist.Partslist.Material.MaterialName1.Contains(materialNo))
                                                )
                                            && (minProdOrderState == null || c.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex >= minProdOrderState)
                                            && (minProdOrderState == null || c.ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex >= minProdOrderState)
                                            && (maxProdOrderState == null || c.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex <= maxProdOrderState)
                                            && (maxProdOrderState == null || c.ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex <= maxProdOrderState)
                                            && (filterStartTime == null
                                                 || (c.ScheduledStartDate != null && c.ScheduledStartDate >= filterStartTime)
                                                 || (c.ScheduledStartDate == null && c.UpdateDate >= filterStartTime)
                                                )
                                            && (filterEndTime == null
                                                 || (c.ScheduledEndDate != null && c.ScheduledEndDate < filterEndTime)
                                                 || (c.ScheduledEndDate == null && c.ScheduledStartDate != null && c.ScheduledStartDate <= filterEndTime)
                                                 || (c.ScheduledEndDate == null && c.ScheduledStartDate == null && c.UpdateDate <= filterEndTime)
                                                )
                                             && (
                                                    (planningMRID == null && !c.ProdOrderPartslist.PlanningMRProposal_ProdOrderPartslist.Any())
                                                    || (planningMRID != null && c.ProdOrderPartslist.PlanningMRProposal_ProdOrderPartslist.Any(x => x.PlanningMRID == planningMRID))
                                                )
                                            && (
                                                   mdBatchPlanGroup == null
                                                   ||
                                                   c.MDBatchPlanGroupID == mdBatchPlanGroup
                                                )
                                          )
                                    .OrderBy(c => c.ScheduledOrder ?? 0)
                                    .ThenBy(c => c.InsertDate)
                                    .Take(500)
        );

        protected static readonly Func<DatabaseApp, short, short, DateTime?, DateTime?, short?, Guid?, Guid?, string, string, IEnumerable<ProdOrderBatchPlan>> s_cQry_BatchPlansWithPWNode =
        EF.CompileQuery<DatabaseApp, short, short, DateTime?, DateTime?, short?, Guid?, Guid?, string, string, IEnumerable<ProdOrderBatchPlan>>(
            (ctx, fromPlanState, toPlanState, filterStartTime, filterEndTime, minProdOrderState, planningMRID, mdBatchPlanGroup, programNo, materialNo) =>
                                    ctx.ProdOrderBatchPlan
                                    .Include("ProdOrderPartslist")
                                    .Include("ProdOrderPartslist.MDProdOrderState")
                                    .Include("ProdOrderPartslist.ProdOrder")
                                    .Include("ProdOrderPartslist.ProdOrder.MDProdOrderState")
                                    .Include("ProdOrderPartslist.ProdOrder.ProdOrderPartslist_ProdOrder")
                                    .Include("ProdOrderPartslist.Partslist")
                                    .Include("ProdOrderPartslist.Partslist.Material")
                                    .Include("ProdOrderPartslist.Partslist.Material.BaseMDUnit")
                                    .Include("ProdOrderPartslist.Partslist.Material.MaterialUnit_Material")
                                    .Where(c =>
                                            c.ProdOrderPartslist.Partslist.IsEnabled
                                            && c.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Any()
                                            && c.PlanStateIndex >= fromPlanState
                                            && c.PlanStateIndex <= toPlanState
                                            && (string.IsNullOrEmpty(programNo) || c.ProdOrderPartslist.ProdOrder.ProgramNo.Contains(programNo))
                                            && (
                                                    string.IsNullOrEmpty(materialNo)
                                                    || (c.ProdOrderPartslist.Partslist.Material.MaterialNo.Contains(materialNo) || c.ProdOrderPartslist.Partslist.Material.MaterialName1.Contains(materialNo))
                                                )
                                            && (minProdOrderState == null || c.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex >= minProdOrderState)
                                            && (minProdOrderState == null || c.ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex >= minProdOrderState)
                                            && (filterStartTime == null
                                                 || (c.ScheduledStartDate != null && c.ScheduledStartDate >= filterStartTime)
                                                 || (c.ScheduledStartDate == null && c.UpdateDate >= filterStartTime)
                                                )
                                            && (filterEndTime == null
                                                 || (c.ScheduledEndDate != null && c.ScheduledEndDate < filterEndTime)
                                                 || (c.ScheduledEndDate == null && c.ScheduledStartDate != null && c.ScheduledStartDate <= filterEndTime)
                                                 || (c.ScheduledEndDate == null && c.ScheduledStartDate == null && c.UpdateDate <= filterEndTime)
                                                )
                                             && (
                                                    (planningMRID == null && !c.ProdOrderPartslist.PlanningMRProposal_ProdOrderPartslist.Any())
                                                    || (planningMRID != null && c.ProdOrderPartslist.PlanningMRProposal_ProdOrderPartslist.Any(x => x.PlanningMRID == planningMRID))
                                                )
                                            && (
                                                   mdBatchPlanGroup == null
                                                   ||
                                                   c.MDBatchPlanGroupID == mdBatchPlanGroup
                                                )
                                          )
                                    .OrderBy(c => c.ScheduledOrder ?? 0)
                                    .ThenBy(c => c.InsertDate)
        );

        public ObservableCollection<ProdOrderBatchPlan> GetProductionLinieBatchPlans(
            DatabaseApp databaseApp,
            Guid? mdSchedulingGroupID,
            GlobalApp.BatchPlanState fromPlanState,
            GlobalApp.BatchPlanState toPlanState,
            DateTime? filterStartTime,
            DateTime? filterEndTime,
            MDProdOrderState.ProdOrderStates? minProdOrderState,
            MDProdOrderState.ProdOrderStates? maxProdOrderState,
            Guid? planningMRID,
            Guid? mdBatchPlanGroup,
            string programNo,
            string materialNo)
        {
            IEnumerable<ProdOrderBatchPlan> batchQuery = s_cQry_BatchPlansForPWNode(databaseApp,
                                                                                    mdSchedulingGroupID,
                                                                                    (short)fromPlanState,
                                                                                    (short)toPlanState,
                                                                                    filterStartTime,
                                                                                    filterEndTime,
                                                                                    minProdOrderState.HasValue ? (short?)minProdOrderState.Value : null,
                                                                                    maxProdOrderState.HasValue ? (short?)maxProdOrderState.Value : null,
                                                                                    planningMRID,
                                                                                    mdBatchPlanGroup,
                                                                                    programNo,
                                                                                    materialNo);
 			//batchQuery.MergeOption = MergeOption.OverwriteChanges;
            return new ObservableCollection<ProdOrderBatchPlan>(batchQuery);
        }

        public ObservableCollection<ProdOrderBatchPlan> GetProductionLinieBatchPlansWithPWNode(
            DatabaseApp databaseApp,
            GlobalApp.BatchPlanState fromPlanState,
            GlobalApp.BatchPlanState toPlanState,
            DateTime? filterStartTime,
            DateTime? filterEndTime,
            MDProdOrderState.ProdOrderStates? minProdOrderState,
            Guid? planningMRID,
            Guid? mdBatchPlanGroup,
            string programNo,
            string materialNo)
        {
            IQueryable<ProdOrderBatchPlan> batchQuery = s_cQry_BatchPlansWithPWNode(databaseApp, (short)fromPlanState,
                (short)toPlanState, filterStartTime, filterEndTime, minProdOrderState.HasValue ? (short?)minProdOrderState.Value : null,
                planningMRID, mdBatchPlanGroup, programNo, materialNo) as IQueryable<ProdOrderBatchPlan>;
            //batchQuery.MergeOption = MergeOption.OverwriteChanges;
            return new ObservableCollection<ProdOrderBatchPlan>(batchQuery);
        }

        #endregion

        #endregion

        #region ProdOrder -> Finished Batch

        public List<ProdOrderPartslistPos> GetFinishedBatch(
            DatabaseApp databaseApp,
            Guid? mdSchedulingGroupID,
            DateTime? filterStartTime,
            DateTime? filterEndTime,
            MDProdOrderState.ProdOrderStates? minProdOrderState,
            Guid? planningMRID,
            Guid? mdBatchPlanGroup,
            string programNo,
            string materialNo)
        {
            var batchQuery = s_cQry_FinishedBatch(databaseApp, mdSchedulingGroupID, filterStartTime, filterEndTime, minProdOrderState.HasValue ? (short?)minProdOrderState.Value : null,
                planningMRID, mdBatchPlanGroup, programNo, materialNo);
            //batchQuery.MergeOption = MergeOption.OverwriteChanges;
            return batchQuery.ToList();
        }

        protected static readonly Func<DatabaseApp, Guid?, DateTime?, DateTime?, short?, Guid?, Guid?, string, string, IEnumerable<ProdOrderPartslistPos>> s_cQry_FinishedBatch =
            EF.CompileQuery<DatabaseApp, Guid?, DateTime?, DateTime?, short?, Guid?, Guid?, string, string, IEnumerable<ProdOrderPartslistPos>>(
            (ctx, mdSchedulingGroupID, filterStartTime, filterEndTime, minProdOrderState, planningMRID, mdBatchPlanGroup, programNo, materialNo) =>
                    ctx
                    .ProdOrderBatchPlan
                    .Include("ProdOrderPartslist")
                    .Include("ProdOrderPartslist.MDProdOrderState")
                    .Include("ProdOrderPartslist.ProdOrder")
                    .Include("ProdOrderPartslist.ProdOrder.MDProdOrderState")
                    .Include("ProdOrderPartslist.ProdOrder.ProdOrderPartslist_ProdOrder")
                    .Include("ProdOrderPartslist.Partslist")
                    .Include("ProdOrderPartslist.Partslist.Material")
                    .Include("ProdOrderPartslist.Partslist.Material.BaseMDUnit")
                    .Include("ProdOrderPartslist.Partslist.Material.MaterialUnit_Material")

                    .Include("ProdOrderBatch_ProdOrderBatchPlan")
                    .Include("ProdOrderBatch_ProdOrderBatchPlan.ProdOrderPartslistPos_ProdOrderBatch")
                    .Where(c =>
                            c.ProdOrderPartslist.Partslist.IsEnabled
                            && (mdSchedulingGroupID == null || c.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Any(x => x.MDSchedulingGroupID == (mdSchedulingGroupID ?? Guid.Empty)))
                            && (string.IsNullOrEmpty(programNo) || c.ProdOrderPartslist.ProdOrder.ProgramNo.Contains(programNo))
                            && (
                                    string.IsNullOrEmpty(materialNo)
                                    || (
                                            c.ProdOrderPartslist.Partslist.Material.MaterialNo.Contains(materialNo)
                                            || c.ProdOrderPartslist.Partslist.Material.MaterialName1.Contains(materialNo)
                                        )
                                )
                            && (minProdOrderState == null || c.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex >= minProdOrderState)
                            && (minProdOrderState == null || c.ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex >= minProdOrderState)
                            && (
                                    (planningMRID == null && !c.ProdOrderPartslist.PlanningMRProposal_ProdOrderPartslist.Any())
                                    || (planningMRID != null && c.ProdOrderPartslist.PlanningMRProposal_ProdOrderPartslist.Any(x => x.PlanningMRID == planningMRID))
                                )
                            && (
                                   mdBatchPlanGroup == null
                                   ||
                                   c.MDBatchPlanGroupID == mdBatchPlanGroup
                                )
                          )
                    .SelectMany(c => c.ProdOrderBatch_ProdOrderBatchPlan)
                    .SelectMany(c => c.ProdOrderPartslistPos_ProdOrderBatch)
                    .Where(c =>
                            c.ParentProdOrderPartslistPosID != null
                            && !c.ProdOrderPartslistPos1_ParentProdOrderPartslistPos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any()
                            && (filterStartTime == null || c.InsertDate >= filterStartTime)
                            && (filterEndTime == null || c.InsertDate < filterEndTime)
                    )
                    .OrderBy(c => c.InsertDate)
                    .Take(500)
        );


        #endregion

        #region Public Methods

        #region ProdOrder
        public virtual void FinishOrder(DatabaseApp dbApp, ProdOrder currentProdOrder)
        {
            MDProdOrderState state = DatabaseApp.s_cQry_GetMDProdOrderState(dbApp, MDProdOrderState.ProdOrderStates.ProdFinished).FirstOrDefault();
            MDProdOrderPartslistPosState statePos = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
            if (state != null)
            {
                currentProdOrder.MDProdOrderState = state;
                foreach (ProdOrderPartslist pOPl in currentProdOrder.ProdOrderPartslist_ProdOrder)
                {
                    pOPl.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => c.PlanStateIndex <= (short)GlobalApp.BatchPlanState.Completed).ToList().ForEach(c => c.PlanState = GlobalApp.BatchPlanState.Completed);

                    if (statePos != null)
                    {
                        pOPl.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => (c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern || c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardRoot)
                                                                             && (c.MDProdOrderPartslistPosStateID.HasValue
                                                                                && c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex < (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                                                                && c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex > (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled))
                                                                 .ToList()
                                                                 .ForEach(c => c.MDProdOrderPartslistPosState = statePos);
                    }
                    if (pOPl.MDProdOrderState != state)
                        pOPl.MDProdOrderState = state;
                }
            }
        }

        public virtual void ConnectSourceProdOrderPartslist(ProdOrder prodOrder)
        {
            Guid[] producedMaterialIDs = prodOrder.ProdOrderPartslist_ProdOrder.Select(c => c.Partslist.MaterialID).Distinct().ToArray();
            ProdOrderPartslist[] partslists = prodOrder.ProdOrderPartslist_ProdOrder.ToArray();
            ProdOrderPartslistPos[] allComponents =
                prodOrder
                .ProdOrderPartslist_ProdOrder
                .SelectMany(c => c.ProdOrderPartslistPos_ProdOrderPartslist)
                .Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot && producedMaterialIDs.Contains(c.MaterialID ?? Guid.Empty))
                .ToArray();

            foreach (ProdOrderPartslistPos pos in allComponents)
            {
                ProdOrderPartslist pl = partslists.FirstOrDefault(c => c.Partslist != null && c.Partslist.MaterialID == pos.MaterialID);
                if (pl != null && pos.ProdOrderPartslistID != pl.ProdOrderPartslistID)
                    pos.SourceProdOrderPartslist = pl;
            }
        }

        public void CorrectSortOrder(ProdOrder prodOrder)
        {
            List<ProdOrderPartslist> items = prodOrder.ProdOrderPartslist_ProdOrder.ToList();
            Dictionary<Guid, ProdOrderPartslist> itemMap = items.ToDictionary(i => i.ProdOrderPartslistID);
            var depthMap = new Dictionary<Guid, int>();

            foreach (var item in items)
            {
                GetDepth(item, depthMap, itemMap);
            }

            int sequence = 1;
            foreach (var item in items.OrderByDescending(i => depthMap[i.ProdOrderPartslistID]))
            {
                item.Sequence = sequence++;
            }
        }

        int GetDepth(ProdOrderPartslist item, Dictionary<Guid, int> depthMap, Dictionary<Guid, ProdOrderPartslist> itemMap)
        {
            if (depthMap.ContainsKey(item.ProdOrderPartslistID))
                return depthMap[item.ProdOrderPartslistID];

            if (!item.ProdOrderPartslistPos_SourceProdOrderPartslist.Any())
                return depthMap[item.ProdOrderPartslistID] = 1;

            var depth = item.ProdOrderPartslistPos_SourceProdOrderPartslist
                .Select(c => c.ProdOrderPartslist)
                .AsEnumerable()
                .Distinct()
                .Select(sourcePl => itemMap.ContainsKey(sourcePl.ProdOrderPartslistID) ? GetDepth(sourcePl, depthMap, itemMap) : 0)
                .DefaultIfEmpty(0)
                .Max() + 1;

            return depthMap[item.ProdOrderPartslistID] = depth;
        }



        //private void CorrectSortOrder(int sequence, ProdOrder prodOrder, ProdOrderPartslist prodOrderPartslist)
        //{
        //    sequence++;
        //    ProdOrderPartslist[] partslists =
        //       prodOrder
        //       .ProdOrderPartslist_ProdOrder
        //       .Where(c => c.ProdOrderPartslistPos_ProdOrderPartslist.Any(x => x.SourceProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID)).ToArray();
        //    foreach (ProdOrderPartslist pl in partslists)
        //    {
        //        pl.Sequence = sequence;
        //        CorrectSortOrder(sequence, prodOrder, pl);
        //    }
        //}

        #endregion

        #region BookingOutward
        public FacilityPreBooking NewOutwardFacilityPreBooking(ACComponent facilityManager, DatabaseApp dbApp, ProdOrderPartslistPosRelation partsListPosRelation,
                                                               bool onEmptyingFacility = false)
        {
            ACMethodBooking acMethodClone = BookParamOutwardMovementClone(facilityManager, dbApp);
            if (onEmptyingFacility)
                acMethodClone = BookParamOutwardMovementOnEmptyingFacilityClone(facilityManager, dbApp);

            partsListPosRelation.AutoRefresh(dbApp);
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityPreBooking), FacilityPreBooking.NoColumnName, FacilityPreBooking.FormatNewNo, this);
            FacilityPreBooking facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, partsListPosRelation, secondaryKey); // TODO später: Child-Instanz erzeugen
            facilityPreBooking.ProdOrderPartslistPosRelation = partsListPosRelation;
            ACMethodBooking acMethodBooking = acMethodClone.Clone() as ACMethodBooking;
            acMethodBooking.OutwardMaterial = partsListPosRelation.SourceProdOrderPartslistPos.Material;
            acMethodBooking.PartslistPosRelation = partsListPosRelation;
            //acMethodBooking.InwardQuantity = deliveryNotePos.InOrderPos.TargetQuantityUOM;
            double quantityUOM = partsListPosRelation.TargetQuantityUOM - partsListPosRelation.PreBookingOutwardQuantityUOM() - partsListPosRelation.ActualQuantityUOM;
            if (partsListPosRelation.SourceProdOrderPartslistPos.MDUnit != null)
            {
                acMethodBooking.OutwardQuantity = partsListPosRelation.SourceProdOrderPartslistPos.Material.ConvertQuantity(quantityUOM, partsListPosRelation.SourceProdOrderPartslistPos.Material.BaseMDUnit, partsListPosRelation.SourceProdOrderPartslistPos.MDUnit);
                acMethodBooking.MDUnit = partsListPosRelation.SourceProdOrderPartslistPos.MDUnit;
            }
            else
            {
                acMethodBooking.OutwardQuantity = quantityUOM;
            }
            if (partsListPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.CPartnerCompany != null)
                acMethodBooking.CPartnerCompany = partsListPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.CPartnerCompany;
            facilityPreBooking.ACMethodBooking = acMethodBooking;
            return facilityPreBooking;
        }

        public List<FacilityPreBooking> CancelOutFacilityPreBooking(ACComponent facilityManager, DatabaseApp dbApp, ProdOrderPartslistPosRelation partsListPosRelation)
        {
            List<FacilityPreBooking> bookings = new List<FacilityPreBooking>();
            ACMethodBooking acMethodClone = null;
            FacilityPreBooking facilityPreBooking = null;
            if (partsListPosRelation == null || partsListPosRelation.MDProdOrderPartslistPosState == null || partsListPosRelation.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled)
                return null;
            if (partsListPosRelation.EntityState != EntityState.Added)
            {
                partsListPosRelation.FacilityBooking_ProdOrderPartslistPosRelation.AutoLoad(partsListPosRelation.FacilityBooking_ProdOrderPartslistPosRelationReference, partsListPosRelation);
                partsListPosRelation.FacilityPreBooking_ProdOrderPartslistPosRelation.AutoLoad(partsListPosRelation.FacilityPreBooking_ProdOrderPartslistPosRelationReference, partsListPosRelation);
            }
            else
                return null;
            if (partsListPosRelation.FacilityPreBooking_ProdOrderPartslistPosRelation.Any())
                return null;
            foreach (FacilityBooking previousBooking in partsListPosRelation.FacilityBooking_ProdOrderPartslistPosRelation)
            {
                if (previousBooking.FacilityBookingType != GlobalApp.FacilityBookingType.ProdOrderPosOutward
                    || previousBooking.FacilityBookingType != GlobalApp.FacilityBookingType.ProdOrderPosOutwardOnEmptyingFacility)
                    continue;
                // Wenn einmal Storniert, dann kann nicht mehr storniert werden. Der Fall dürfte normalerweise nicht auftreten, 
                // da der Positionsstatus auch MDOutOrderPosState.OutOrderPosStates.Cancelled sein müsste
                else if (previousBooking.FacilityBookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutwardCancel)
                    return null;
            }

            foreach (FacilityBooking previousBooking in partsListPosRelation.FacilityBooking_ProdOrderPartslistPosRelation)
            {
                if (previousBooking.FacilityBookingType != GlobalApp.FacilityBookingType.ProdOrderPosOutward
                    || previousBooking.FacilityBookingType != GlobalApp.FacilityBookingType.ProdOrderPosOutwardOnEmptyingFacility)
                    continue;
                acMethodClone = BookParamOutCancelClone(facilityManager, dbApp);
                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityPreBooking), FacilityPreBooking.NoColumnName, FacilityPreBooking.FormatNewNo, this);
                facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, partsListPosRelation, secondaryKey);
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
                acMethodBooking.PartslistPosRelation = partsListPosRelation;
                if (previousBooking.CPartnerCompany != null)
                    acMethodBooking.CPartnerCompany = previousBooking.CPartnerCompany;
                facilityPreBooking.ACMethodBooking = acMethodBooking;
                bookings.Add(facilityPreBooking);
            }
            return bookings;
        }

        public List<FacilityPreBooking> CancelOutFacilityPreBooking(ACComponent facilityManager, DatabaseApp dbApp, ProdOrderPartslist partsList)
        {
            if (partsList == null)
                return null;
            if (partsList.EntityState != EntityState.Added)
                partsList.ProdOrderPartslistPos_ProdOrderPartslist.AutoLoad(partsList.ProdOrderPartslistPos_ProdOrderPartslistReference, partsList);
            List<FacilityPreBooking> result = null;
            foreach (ProdOrderPartslistPos partsListPos in partsList.ProdOrderPartslistPos_ProdOrderPartslist)
            {
                if (!(partsListPos.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardRoot
                    || partsListPos.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardPart))
                    continue;
                foreach (ProdOrderPartslistPosRelation relation in partsListPos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                {
                    var subResult = CancelOutFacilityPreBooking(facilityManager, dbApp, relation);
                    if (subResult != null)
                    {
                        if (result == null)
                            result = subResult;
                        else
                            result.AddRange(subResult);
                    }
                }
            }
            return result;
        }

        #endregion

        #region BookingInward
        public FacilityPreBooking NewInwardFacilityPreBooking(ACComponent facilityManager, DatabaseApp dbApp, ProdOrderPartslistPos partsListPos)
        {
            ACMethodBooking acMethodClone = BookParamInwardMovementClone(facilityManager, dbApp);
            partsListPos.AutoRefresh(dbApp);
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityPreBooking), FacilityPreBooking.NoColumnName, FacilityPreBooking.FormatNewNo, this);
            FacilityPreBooking facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, partsListPos, secondaryKey); // TODO später: Child-Instanz erzeugen
            facilityPreBooking.ProdOrderPartslistPos = partsListPos;
            ACMethodBooking acMethodBooking = acMethodClone.Clone() as ACMethodBooking;
            acMethodBooking.InwardMaterial = partsListPos.BookingMaterial;
            acMethodBooking.PartslistPos = partsListPos;
            //acMethodBooking.InwardQuantity = deliveryNotePos.InOrderPos.TargetQuantityUOM;
            double quantityUOM = partsListPos.TargetQuantityUOM - partsListPos.PreBookingInwardQuantityUOM() - partsListPos.ActualQuantityUOM;
            if (partsListPos.MDUnit != null)
            {
                acMethodBooking.InwardQuantity = partsListPos.BookingMaterial.ConvertQuantity(quantityUOM, partsListPos.BookingMaterial.BaseMDUnit, partsListPos.MDUnit);
                acMethodBooking.MDUnit = partsListPos.MDUnit;
            }
            else
            {
                acMethodBooking.InwardQuantity = quantityUOM;
            }
            if (partsListPos.ProdOrderPartslist.ProdOrder.CPartnerCompany != null)
                acMethodBooking.CPartnerCompany = partsListPos.ProdOrderPartslist.ProdOrder.CPartnerCompany;
            acMethodClone.InwardSplitNo = null;
            facilityPreBooking.ACMethodBooking = acMethodBooking;
            return facilityPreBooking;
        }

        public List<FacilityPreBooking> CancelInFacilityPreBooking(ACComponent facilityManager, DatabaseApp dbApp, ProdOrderPartslistPos partsListPos)
        {
            List<FacilityPreBooking> bookings = new List<FacilityPreBooking>();
            ACMethodBooking acMethodClone = null;
            FacilityPreBooking facilityPreBooking = null;
            if (partsListPos == null || partsListPos.MDProdOrderPartslistPosState == null || partsListPos.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled)
                return null;
            if (partsListPos.EntityState != EntityState.Added)
            {
                partsListPos.FacilityBooking_ProdOrderPartslistPos.AutoLoad(partsListPos.FacilityBooking_ProdOrderPartslistPosReference, partsListPos);
                partsListPos.FacilityPreBooking_ProdOrderPartslistPos.AutoLoad(partsListPos.FacilityPreBooking_ProdOrderPartslistPosReference, partsListPos);
            }
            else
                return null;
            if (partsListPos.FacilityPreBooking_ProdOrderPartslistPos.Any())
                return null;
            foreach (FacilityBooking previousBooking in partsListPos.FacilityBooking_ProdOrderPartslistPos)
            {
                if (previousBooking.FacilityBookingType != GlobalApp.FacilityBookingType.ProdOrderPosInward)
                    continue;
                // Wenn einmal Storniert, dann kann nicht mehr storniert werden. Der Fall dürfte normalerweise nicht auftreten, 
                // da der Positionsstatus auch MDOutOrderPosState.OutOrderPosStates.Cancelled sein müsste
                else if (previousBooking.FacilityBookingType == GlobalApp.FacilityBookingType.ProdOrderPosInwardCancel)
                    return null;
            }

            foreach (FacilityBooking previousBooking in partsListPos.FacilityBooking_ProdOrderPartslistPos)
            {
                if (previousBooking.FacilityBookingType != GlobalApp.FacilityBookingType.ProdOrderPosInward)
                    continue;
                acMethodClone = BookParamInCancelClone(facilityManager, dbApp);
                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityPreBooking), FacilityPreBooking.NoColumnName, FacilityPreBooking.FormatNewNo, this);
                facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, partsListPos, secondaryKey);
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
                    acMethodBooking.InwardMaterial = partsListPos.BookingMaterial;
                acMethodBooking.PartslistPos = partsListPos;
                if (previousBooking.CPartnerCompany != null)
                    acMethodBooking.CPartnerCompany = previousBooking.CPartnerCompany;
                facilityPreBooking.ACMethodBooking = acMethodBooking;
                bookings.Add(facilityPreBooking);
            }
            return bookings;
        }

        public List<FacilityPreBooking> CancelInFacilityPreBooking(ACComponent facilityManager, DatabaseApp dbApp, ProdOrderPartslist partsList)
        {
            if (partsList == null)
                return null;
            if (partsList.EntityState != EntityState.Added)
                partsList.ProdOrderPartslistPos_ProdOrderPartslist.AutoLoad(partsList.ProdOrderPartslistPos_ProdOrderPartslistReference, partsList);
            List<FacilityPreBooking> result = null;
            foreach (ProdOrderPartslistPos partsListPos in partsList.ProdOrderPartslistPos_ProdOrderPartslist)
            {
                if (!(partsListPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardRoot
                    || partsListPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardPart
                    || partsListPos.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardInternInwardExtern))
                    continue;
                var subResult = CancelInFacilityPreBooking(facilityManager, dbApp, partsListPos);
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


        public MsgWithDetails BalanceBackAndForeflushedStocks(FacilityManager facilityManager, DatabaseApp dbApp, ProdOrderPartslist poPartsList, bool postWithRetry = false)
        {
            if (facilityManager == null || dbApp == null || poPartsList == null)
                return null;
            MsgWithDetails collectedMessages = new MsgWithDetails();
            IEnumerable<ProdOrderPartslistPos> backflushedPosFromPrevPartslist
                = poPartsList.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.SourceProdOrderPartslistID.HasValue
                                                                                    && c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot)
                                                                        .AsEnumerable()
                                                                        .Where(c => c.Backflushing);
            if (backflushedPosFromPrevPartslist.Any())
            {
                List<FacilityPreBooking> newPreBookings = new List<FacilityPreBooking>();
                List<ACMethodBooking> newZeroStockPostings = new List<ACMethodBooking>();
                foreach (ProdOrderPartslistPos backflushedPos in backflushedPosFromPrevPartslist)
                {
                    bool backflushedPosRecalced = false;
                    //if (Math.Abs(backflushedPos.ActualQuantityUOM) <= FacilityConst.C_ZeroCompare)
                    //    continue;
                    ProdOrderPartslistPos finalPosFromPrevPartslist =
                        backflushedPos.SourceProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                                 .Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern
                                    && !c.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
                                 .FirstOrDefault();
                    if (finalPosFromPrevPartslist != null && finalPosFromPrevPartslist.Foreflushing)
                    {
                        if (Math.Abs(finalPosFromPrevPartslist.ActualQuantityUOM) <= FacilityConst.C_ZeroCompare)
                            continue;
                        if (!backflushedPosRecalced)
                        {
                            backflushedPos.RecalcActualQuantity();
                            backflushedPosRecalced = true;
                        }
                        // If SuggestQuantQOnPosting not set, then increase produced quantity if more has been consumed through retrograde posting
                        if (!backflushedPos.SuggestQuantQOnPosting)
                        {
                            finalPosFromPrevPartslist.RecalcActualQuantity();
                            double diff = backflushedPos.ActualQuantityUOM - finalPosFromPrevPartslist.ActualQuantityUOM;
                            if (Math.Abs(diff) > 0.0000001)
                            {
                                FacilityBookingCharge lastInwardPosting =
                                dbApp.FacilityBookingCharge.Include(c => c.ProdOrderPartslistPos)
                                                            .Include(c => c.ProdOrderPartslistPos.Material)
                                                            .Include(c => c.InwardMaterial)
                                                            .Include(c => c.InwardFacility)
                                                            .Include(c => c.InwardFacilityCharge)
                                                            .Include(c => c.InwardFacilityLot)
                                                            .Where(c => c.ProdOrderPartslistPos != null
                                                                        && (c.ProdOrderPartslistPosID == finalPosFromPrevPartslist.ProdOrderPartslistPosID
                                                                            || (c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID.HasValue
                                                                                    && (c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID == finalPosFromPrevPartslist.ProdOrderPartslistPosID
                                                                                        || (c.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos.ParentProdOrderPartslistPosID.HasValue
                                                                                            && c.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos.ParentProdOrderPartslistPosID == finalPosFromPrevPartslist.ProdOrderPartslistPosID)))))
                                                            .OrderByDescending(c => c.FacilityBookingChargeNo)
                                                            .FirstOrDefault();
                                if (lastInwardPosting != null)
                                {
                                    FacilityPreBooking facilityPreBooking = NewInwardFacilityPreBooking(facilityManager, dbApp, lastInwardPosting.ProdOrderPartslistPos);
                                    if (facilityPreBooking != null)
                                    {
                                        ACMethodBooking acMethodBooking = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                                        if (acMethodBooking != null)
                                        {
                                            acMethodBooking.InwardFacility = lastInwardPosting.InwardFacility;
                                            acMethodBooking.InwardFacilityCharge = lastInwardPosting.InwardFacilityCharge;
                                            acMethodBooking.InwardFacilityLot = lastInwardPosting.InwardFacilityLot;
                                            acMethodBooking.InwardQuantity = diff;
                                            newPreBookings.Add(facilityPreBooking);
                                        }
                                    }
                                }
                            }
                        }
                        // else SuggestQuantQOnPosting is set, then set all remaining stocks to zero
                        else if (backflushedPos.SuggestQuantQOnPosting)
                        {
                            IEnumerable<FacilityCharge> remainingFCsWithStock =
                                                        dbApp.FacilityBookingCharge.Include(c => c.InwardFacilityCharge.Material)
                                                        .Include(c => c.InwardFacilityCharge.Facility)
                                                        .Include(c => c.InwardFacilityCharge.FacilityLot)
                                                        .Where(c => c.ProdOrderPartslistPos != null
                                                                    && (c.ProdOrderPartslistPosID == finalPosFromPrevPartslist.ProdOrderPartslistPosID
                                                                        || (c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID.HasValue
                                                                                && (c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID == finalPosFromPrevPartslist.ProdOrderPartslistPosID
                                                                                    || (c.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos.ParentProdOrderPartslistPosID.HasValue
                                                                                        && c.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos.ParentProdOrderPartslistPosID == finalPosFromPrevPartslist.ProdOrderPartslistPosID))))
                                                                    && !c.InwardFacilityCharge.NotAvailable
                                                              )
                                                        .Select(c => c.InwardFacilityCharge)
                                                        .AsEnumerable()
                                                        .Distinct();
                            if (remainingFCsWithStock != null && remainingFCsWithStock.Any())
                            {
                                ACMethodBooking bookingParamTemplate = facilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking;
                                bookingParamTemplate.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(dbApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);

                                foreach (FacilityCharge fc in remainingFCsWithStock)
                                {
                                    ACMethodBooking bookingParam = bookingParamTemplate.Clone() as ACMethodBooking;
                                    bookingParam.InwardFacilityCharge = fc;
                                    newZeroStockPostings.Add(bookingParam);
                                }
                            }
                        }
                    }
                }
                if (newPreBookings.Any() || newZeroStockPostings.Any())
                {
                    MsgWithDetails subMessage = postWithRetry ? dbApp.ACSaveChangesWithRetry() : dbApp.ACSaveChanges();
                    if (subMessage != null)
                        return subMessage;
                    if (newPreBookings.Any())
                    {
                        foreach (var facilityPreBooking in newPreBookings)
                        {
                            ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                            if (bookingParam != null)
                            {
                                ACMethodEventArgs resultBooking = postWithRetry ? facilityManager.BookFacilityWithRetry(ref bookingParam, dbApp) : facilityManager.BookFacility(bookingParam, dbApp);
                                if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                                    collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                else
                                {
                                    if (bookingParam.ValidMessage.IsSucceded())
                                    {
                                        facilityPreBooking.DeleteACObject(dbApp, true);
                                        if (bookingParam.PartslistPos != null)
                                        {
                                            bookingParam.PartslistPos.IncreaseActualQuantityUOM(bookingParam.InwardQuantity.Value);
                                            //bookingParam.PartslistPos.RecalcActualQuantity();
                                            //bookingParam.PartslistPos.TopParentPartslistPos.RecalcActualQuantity();
                                        }
                                        subMessage = postWithRetry ? dbApp.ACSaveChangesWithRetry() : dbApp.ACSaveChanges();
                                        if (subMessage != null)
                                            collectedMessages.AddDetailMessage(subMessage);
                                    }
                                    else
                                    {
                                        collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                    }
                                }
                            }
                        }
                    }
                    if (newZeroStockPostings.Any())
                    {
                        foreach (ACMethodBooking posting in newZeroStockPostings)
                        {
                            ACMethodBooking bookingParam = posting;
                            ACMethodEventArgs resultBooking = postWithRetry ? facilityManager.BookFacilityWithRetry(ref bookingParam, dbApp) : facilityManager.BookFacility(bookingParam, dbApp);
                            if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                                collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                            else
                            {
                                if (!bookingParam.ValidMessage.IsSucceded())
                                {
                                    collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                }
                            }
                        }
                    }
                }
            }
            return collectedMessages.MsgDetailsCount > 0 ? collectedMessages : null;
        }


        public MsgWithDetails CorrectLastInwardQuantAccordingOutwardPostings(FacilityManager facilityManager, DatabaseApp dbApp, ProdOrderBatch batch,
                                                                             ProdOrderPartslistPos endBatchPos, IEnumerable<int> outwardSequenceNo = null, bool postWithRetry = false)
        {
            MsgWithDetails collectedMessages = null;

            FacilityCharge lastQuant = endBatchPos.FacilityBookingCharge_ProdOrderPartslistPos.Select(c => c.InwardFacilityCharge)
                                                                                                     .Where(x => !x.NotAvailable)
                                                                                                     .OrderByDescending(x => x.InsertDate)
                                                                                                     .FirstOrDefault();

            if (lastQuant == null)
            {
                collectedMessages = new MsgWithDetails();
                //Error50569: The last quant is not available or not exists!
                collectedMessages.AddDetailMessage(new Msg(this, eMsgLevel.Error, nameof(ACProdOrderManager), nameof(CorrectLastInwardQuantAccordingOutwardPostings) + "(10)", 2197, "Error50569"));
                return collectedMessages;
            }

            double totalInwardQuantity = endBatchPos.ActualQuantityUOM;

            double totalOutwardQuantity = batch.ProdOrderPartslistPosRelation_ProdOrderBatch.Where(c => !c.SourceProdOrderPartslistPos.Material.IsIntermediate
                                                                                                      && (outwardSequenceNo == null || outwardSequenceNo.Contains(c.Sequence)))
                                                                                            .Sum(x => x.ActualQuantityUOM);

            if (totalOutwardQuantity != totalInwardQuantity)
            {
                double difference = totalOutwardQuantity - totalInwardQuantity;


                FacilityPreBooking facilityPreBooking = NewInwardFacilityPreBooking(facilityManager, dbApp, endBatchPos);
                if (facilityPreBooking != null)
                {
                    ACMethodBooking acMethodBooking = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                    if (acMethodBooking != null)
                    {
                        acMethodBooking.InwardFacility = lastQuant.Facility;
                        acMethodBooking.InwardFacilityCharge = lastQuant;
                        acMethodBooking.InwardFacilityLot = lastQuant.FacilityLot;
                        acMethodBooking.InwardQuantity = difference;
                    }
                }


                MsgWithDetails subMessage = postWithRetry ? dbApp.ACSaveChangesWithRetry() : dbApp.ACSaveChanges();
                if (subMessage != null)
                    return subMessage;

                ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                if (bookingParam != null)
                {
                    ACMethodEventArgs resultBooking = postWithRetry ? facilityManager.BookFacilityWithRetry(ref bookingParam, dbApp) : facilityManager.BookFacility(bookingParam, dbApp);
                    if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                    {
                        if (collectedMessages == null)
                            collectedMessages = new MsgWithDetails();

                        collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                    }
                    else
                    {
                        if (bookingParam.ValidMessage.IsSucceded())
                        {
                            facilityPreBooking.DeleteACObject(dbApp, true);
                            if (bookingParam.PartslistPos != null)
                            {
                                bookingParam.PartslistPos.IncreaseActualQuantityUOM(bookingParam.InwardQuantity.Value);
                                //bookingParam.PartslistPos.RecalcActualQuantity();
                                //bookingParam.PartslistPos.TopParentPartslistPos.RecalcActualQuantity();
                            }
                            subMessage = postWithRetry ? dbApp.ACSaveChangesWithRetry() : dbApp.ACSaveChanges();
                            if (subMessage != null)
                                collectedMessages.AddDetailMessage(subMessage);
                        }
                        else
                        {
                            collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                        }
                    }
                }
            }

            return collectedMessages;
        }

        #endregion

        #region RecalcTargetQuantity

        ///// <summary>
        ///// RecalcIntermediateItem
        ///// </summary>
        ///// <param name="inwardPos">mixure for recalculation</param>
        ///// <param name="updateMixureRelations">if true expected is for relations of mixure to sum all input mixure quantities</param>
        ///// <returns></returns>
        //public MsgWithDetails RecalcIntermediateItem(ProdOrderPartslistPos inwardPos, bool updateMixureRelations)
        //{
        //    MsgWithDetails msgWithDetails = null;
        //    try
        //    {
        //        List<ProdOrderPartslistPos> inputMixures =
        //           inwardPos
        //           .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
        //           .Where(c => c.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
        //           .Select(c => c.SourceProdOrderPartslistPos)
        //           .ToList();
        //        foreach (ProdOrderPartslistPos inputMixure in inputMixures)
        //        {
        //            RecalcIntermediateItem(inputMixure, updateMixureRelations);
        //        }

        //        if (inwardPos.Material.ExcludeFromSumCalc)
        //        {
        //            inwardPos.TargetQuantityUOM = 0;
        //        }
        //        else
        //        {
        //            // fix child relations
        //            double newTargetQuantityUOM = 0;
        //            if (inwardPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Any())
        //                newTargetQuantityUOM = inwardPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Sum(c => c.TargetQuantityUOM);
        //            else
        //                newTargetQuantityUOM = 0;

        //            bool noChange = newTargetQuantityUOM == inwardPos.TargetQuantityUOM;
        //            double diffQuantity = newTargetQuantityUOM - inwardPos.TargetQuantityUOM;
        //            inwardPos.TargetQuantityUOM = newTargetQuantityUOM;

        //            //mixure distrubutes it's quantity to target mixures
        //            if (inwardPos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
        //            {
        //                double ratioInwardPosQuantityGrowth = 0;
        //                if (inwardPos.TargetQuantityUOM > 0)
        //                    ratioInwardPosQuantityGrowth = diffQuantity / inwardPos.TargetQuantityUOM;
        //                if (ratioInwardPosQuantityGrowth == 0 && !noChange && inwardPos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
        //                    ratioInwardPosQuantityGrowth = 1 / inwardPos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Count();

        //                List<ProdOrderPartslistPosRelation> mixureDestinationRelations =
        //                    inwardPos
        //                    .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
        //                    .Where(c => c.TargetProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
        //                    .ToList();
        //                foreach (var rel in mixureDestinationRelations)
        //                {
        //                    if (updateMixureRelations /*&& rel.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern*/)
        //                    {
        //                        // Find pos relation
        //                        PartslistPosRelation plRel = null;
        //                        if (inwardPos.BasedOnPartslistPos != null)
        //                            plRel = inwardPos
        //                                               .BasedOnPartslistPos
        //                                               .PartslistPosRelation_SourcePartslistPos
        //                                               .Where(c =>
        //                                                           c.TargetPartslistPos.MaterialID == rel.TargetProdOrderPartslistPos.MaterialID
        //                                                     )
        //                                               .FirstOrDefault();

        //                        // calculate ratio of relation quantity in source quantity of partslist
        //                        double plRelQueryRatio = 0;
        //                        if (plRel != null && plRel.SourcePartslistPos.TargetQuantityUOM > 0)
        //                            plRelQueryRatio = plRel.TargetQuantityUOM / plRel.SourcePartslistPos.TargetQuantityUOM;
        //                        if (plRel == null || plRelQueryRatio == 0)
        //                        {
        //                            // if no pl connection or ratio -> quantity divide equally
        //                            int count = rel.SourceProdOrderPartslistPos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Count();
        //                            plRelQueryRatio = 1 / count;
        //                        }

        //                        // Distribute new SourceProdOrderPartslistPos.TargetQuantityUOM with ratio from partslist
        //                        if (plRelQueryRatio > 0)
        //                            rel.TargetQuantityUOM = rel.SourceProdOrderPartslistPos.TargetQuantityUOM * plRelQueryRatio;
        //                    }
        //                    else
        //                        rel.TargetQuantityUOM = rel.TargetQuantityUOM + ratioInwardPosQuantityGrowth * rel.TargetQuantityUOM;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ec)
        //    {
        //        msgWithDetails = new MsgWithDetails() { MessageLevel = eMsgLevel.Error, Message = ec.Message };
        //    }

        //    return msgWithDetails;
        //}

        public MsgWithDetails RecalcIntermediateItem(ProdOrderPartslistPos inwardPos, bool updateMixureRelations, MDUnit startMDUnit)
        {
            MsgWithDetails msgWithDetails = null;
            try
            {
                List<ProdOrderPartslistPos> inputMixures =
                   inwardPos
                   .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                   .Where(c => c.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                   .Select(c => c.SourceProdOrderPartslistPos)
                   .ToList();
                foreach (ProdOrderPartslistPos inputMixure in inputMixures)
                {
                    RecalcIntermediateItem(inputMixure, updateMixureRelations, startMDUnit);
                }

                MDUnit inwardPosMDUnit = inwardPos.MDUnit != null ? inwardPos.MDUnit : inwardPos.Material.BaseMDUnit;

                if (inwardPos.Material.ExcludeFromSumCalc)
                {
                    inwardPos.TargetQuantityUOM = 0;
                }
                else
                {
                    // fix child relations
                    double newTargetQuantityUOM = 0;
                    if (inwardPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Any())
                    {
                        ProdOrderPartslistPosRelation[] targetRelations = inwardPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray();
                        foreach (ProdOrderPartslistPosRelation targetRelation in targetRelations)
                        {
                            ProdOrderPartslistPos sourcePos = targetRelation.SourceProdOrderPartslistPos;
                            MDUnit sourcePosMDUnit = sourcePos.MDUnit != null ? sourcePos.MDUnit : sourcePos.Material.BaseMDUnit;
                            if (sourcePosMDUnit.MDUnitID == inwardPosMDUnit.MDUnitID)
                            {
                                newTargetQuantityUOM += targetRelation.TargetQuantityUOM;
                            }
                            else
                            {
                                if (sourcePos.Material.IsConvertableToUnit(sourcePosMDUnit, inwardPosMDUnit))
                                {
                                    newTargetQuantityUOM += sourcePos.Material.ConvertQuantity(targetRelation.TargetQuantityUOM, sourcePosMDUnit, inwardPosMDUnit);
                                }
                            }
                        }
                    }
                    else
                    {
                        newTargetQuantityUOM = 0;
                    }

                    var queryMixureConsumers =
                        inwardPos
                        .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                        .Where(c => c.TargetProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern);

                    double diffQuantity = newTargetQuantityUOM - inwardPos.TargetQuantityUOM;
                    inwardPos.TargetQuantityUOM = newTargetQuantityUOM;
                    int sourceRelationCount = queryMixureConsumers.Count();
                    double ratioInwardPosQuantityGrowth = 0;
                    if (!FacilityConst.IsDoubleZeroForPosting(inwardPos.TargetQuantityUOM))
                        ratioInwardPosQuantityGrowth = diffQuantity / inwardPos.TargetQuantityUOM;

                    //mixure distrubutes it's quantity to target mixures
                    if (sourceRelationCount > 0)
                    {
                        foreach (var rel in queryMixureConsumers)
                        {
                            // redistrubute complete quantity (updateMixureRelations or previous quanitity = 0
                            if (updateMixureRelations || ratioInwardPosQuantityGrowth == 0 /*&& rel.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern*/)
                            {
                                rel.TargetQuantityUOM = rel.SourceProdOrderPartslistPos.TargetQuantityUOM * (1 / sourceRelationCount);
                            }
                            else
                            {
                                rel.TargetQuantityUOM = rel.TargetQuantityUOM + ratioInwardPosQuantityGrowth * rel.TargetQuantityUOM;
                            }
                        }
                    }
                }
            }
            catch (Exception ec)
            {
                msgWithDetails = new MsgWithDetails() { MessageLevel = eMsgLevel.Error, Message = ec.Message };
            }

            return msgWithDetails;
        }


        public MsgWithDetails IsRecalcIntermediateSumPossible(ProdOrderPartslistPos inwardPos)
        {
            return IsRecalcIntermediateSumPossible(inwardPos, inwardPos.MDUnit);
        }

        public MsgWithDetails IsRecalcIntermediateSumPossible(ProdOrderPartslistPos inwardPos, MDUnit startMDUnit)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            Msg msgCheckComponents = IsInputComponentsCompatibile(inwardPos);
            Msg msgCheckMixures = IsInputMixuresCompatibile(inwardPos, startMDUnit);

            if (msgCheckComponents != null)
            {
                msgWithDetails.AddDetailMessage(msgCheckComponents);
            }

            if (msgCheckMixures != null)
            {
                msgWithDetails.AddDetailMessage(msgCheckMixures);
            }
            return msgWithDetails;
        }


        /// <summary>
        /// Pass component unit to own intermediate unit
        /// </summary>
        /// <param name="inwardPos"></param>
        /// <param name="startMDUnit"></param>
        /// <returns></returns>
        private Msg IsInputComponentsCompatibile(ProdOrderPartslistPos inwardPos)
        {
            Msg msg = null;
            ProdOrderPartslistPosRelation[] relations =
                inwardPos
                .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                .Where(c => c.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot)
                .ToArray();

            // OutwardRoot[Pos] MDUnit != InwardIntern[Pos] MDUnit
            //inwardPos.MDUnit.IsConvertableToUnit()
            MDUnit posUnit = inwardPos.MDUnit != null ? inwardPos.MDUnit : inwardPos.Material.BaseMDUnit;
            //relations.Any(c => c.SourceProdOrderPartslistPos.Material.IsConvertableToUnit(c.SourceProdOrderPartslistPos.MDUnit != null ? c.SourceProdOrderPartslistPos.MDUnit : c.SourceProdOrderPartslistPos.Material.BaseMDUnit, posUnit));
            //MDUnit[] inputUnits = relations.Select(c => c.SourceProdOrderPartslistPos.MDUnit != null ? c.SourceProdOrderPartslistPos.MDUnit : c.SourceProdOrderPartslistPos.Material.BaseMDUnit).ToArray();
            if (relations.Any(c => !c.SourceProdOrderPartslistPos.Material.IsConvertableToUnit(c.SourceProdOrderPartslistPos.MDUnit != null ? c.SourceProdOrderPartslistPos.MDUnit : c.SourceProdOrderPartslistPos.Material.BaseMDUnit, posUnit)))
            {
                msg = new Msg(this, eMsgLevel.Warning, nameof(ACProdOrderManager), nameof(IsInputComponentsCompatibile), 145, "Question50091");
            }

            // Recursive search to other input mixures
            if (msg == null)
            {
                ProdOrderPartslistPosRelation[] mixRelations =
                   inwardPos
                   .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                   .Where(c => c.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                   .ToArray();

                foreach (ProdOrderPartslistPosRelation inputMix in mixRelations)
                {
                    msg = IsInputComponentsCompatibile(inputMix.SourceProdOrderPartslistPos);
                    if (msg != null)
                        break;
                }
            }
            return msg;
        }

        private Msg IsInputMixuresCompatibile(ProdOrderPartslistPos inwardPos, MDUnit lastIntermediateUnit)
        {
            Msg msg = null;
            ProdOrderPartslistPosRelation[] mixRelations =
                    inwardPos
                    .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                    .Where(c => c.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                    .ToArray();

            foreach (ProdOrderPartslistPosRelation inputMix in mixRelations)
            {
                MDUnit intermediateUnit = inputMix.SourceProdOrderPartslistPos.MDUnit != null ? inputMix.SourceProdOrderPartslistPos.MDUnit : inputMix.SourceProdOrderPartslistPos.Material.BaseMDUnit;
                if (!inputMix.SourceProdOrderPartslistPos.Material.IsConvertableToUnit(intermediateUnit, lastIntermediateUnit))
                {
                    msg = new Msg(this, eMsgLevel.Warning, nameof(ACProdOrderManager), nameof(IsInputComponentsCompatibile), 145, "Question50092");
                }
                if (msg == null)
                {
                    msg = IsInputMixuresCompatibile(inputMix.SourceProdOrderPartslistPos, lastIntermediateUnit);
                }
            }
            return msg;
        }


        /// <summary>
        /// Recalculate rest quantity
        /// </summary>
        /// <param name="partslist"></param>
        /// <returns></returns>
        public MsgWithDetails RecalcRemainingQuantity(ProdOrderPartslistPos mixItem)
        {
            MsgWithDetails msgWithDetails = null;
            try
            {
                var inputMixures =
                    mixItem
                    .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                    .Where(c => c.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                    .Select(c => c.SourceProdOrderPartslistPos)
                    .ToList();
                foreach (ProdOrderPartslistPos inputMixure in inputMixures)
                    RecalcRemainingQuantity(inputMixure);

                var relations = mixItem.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos;
                mixItem.RestQuantityUOM = mixItem.TargetQuantityUOM;
                foreach (var rel in relations)
                {
                    mixItem.RestQuantity -= rel.TargetQuantity;
                    mixItem.RestQuantityUOM -= rel.TargetQuantityUOM;
                }
            }
            catch (Exception ec)
            {
                msgWithDetails = new MsgWithDetails() { MessageLevel = eMsgLevel.Error, Message = ec.Message };
            }
            return msgWithDetails;
        }

        /// <summary>
        /// Recalculate rest quantity
        /// </summary>
        /// <param name="partslist"></param>
        /// <returns></returns>
        public MsgWithDetails RecalcRemainingOutwardQuantity(ProdOrderPartslistPos outwardItem)
        {
            MsgWithDetails msgWithDetails = null;
            try
            {
                var relations = outwardItem.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos;
                outwardItem.PositionUsedCount = relations.Count();
                outwardItem.RestQuantity = outwardItem.TargetQuantity;
                outwardItem.RestQuantityUOM = outwardItem.TargetQuantityUOM;
                foreach (var rel in relations)
                {
                    outwardItem.RestQuantity -= rel.TargetQuantity;
                    outwardItem.RestQuantityUOM -= rel.TargetQuantityUOM;
                }
            }
            catch (Exception ec)
            {
                msgWithDetails = new MsgWithDetails() { MessageLevel = eMsgLevel.Error, Message = ec.Message };
            }
            return msgWithDetails;
        }

        #endregion

        #region CalcProducedBatchWeight
        public Msg CalcProducedBatchWeight(DatabaseApp dbApp, ProdOrderPartslistPos batchIntermediatePos, double? lossCorrectionFactor, out double sumWeight)
        {
            sumWeight = 0;
            if (batchIntermediatePos == null)
                return new Msg() { Message = "batchIntermediatePos is null" };
            if (batchIntermediatePos.ProdOrderBatch == null)
                return new Msg() { Message = "no Batch assigned to intermediate line" };

            MDUnit unitKG = MDUnit.GetSIUnit(dbApp, GlobalApp.SIDimensions.Mass);
            if (unitKG == null)
                return new Msg() { Message = "Mass-Unit KG not found" };

            CalcProducedBatchWeight(ref sumWeight, batchIntermediatePos, batchIntermediatePos.ProdOrderBatch, unitKG);

            if (sumWeight > 0.000001 && lossCorrectionFactor.HasValue)
            {
                if (lossCorrectionFactor > 0.000001)
                    sumWeight = sumWeight - (sumWeight * (lossCorrectionFactor.Value / 100));
                else
                {
                    Partslist pl = batchIntermediatePos.ProdOrderPartslist?.Partslist;
                    if (pl != null && pl.LossCorrectionFactor.HasValue)
                        sumWeight = sumWeight * pl.LossCorrectionFactor.Value;
                }
            }

            return null;
        }

        private void CalcProducedBatchWeight(ref double sumWeight, ProdOrderPartslistPos batchIntermediatePos, ProdOrderBatch forBatch, MDUnit unitKG)
        {
            foreach (var relation in batchIntermediatePos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
            {
                if (relation.ActualQuantityUOM > 0.000001)
                {
                    if (relation.SourceProdOrderPartslistPos.MDUnit != null)
                    {
                        if (!relation.SourceProdOrderPartslistPos.Material.IsConvertableToUnit(relation.SourceProdOrderPartslistPos.MDUnit, unitKG))
                            continue;
                        sumWeight += relation.SourceProdOrderPartslistPos.Material.ConvertQuantity(relation.ActualQuantity, relation.SourceProdOrderPartslistPos.MDUnit, unitKG);
                    }
                    else
                        sumWeight += relation.SourceProdOrderPartslistPos.Material.ConvertQuantity(relation.ActualQuantityUOM, relation.SourceProdOrderPartslistPos.Material.BaseMDUnit, unitKG);
                }
                else
                {
                    var prevBatchIntermediatePos = forBatch.ProdOrderPartslistPos_ProdOrderBatch.Where(c => c.ParentProdOrderPartslistPosID.HasValue && c.ParentProdOrderPartslistPosID.Value == relation.SourceProdOrderPartslistPosID).FirstOrDefault();
                    if (prevBatchIntermediatePos != null)
                    {
                        CalcProducedBatchWeight(ref sumWeight, prevBatchIntermediatePos, forBatch, unitKG);
                    }
                }
            }
        }

        #endregion

        #region Methods -> Connect 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseApp"></param>
        /// <param name="pwClassName">PWNodeProcessWorkflowVB.PWClassName</param>
        /// <returns></returns>
        public List<PartslistMDSchedulerGroupConnection> GetPartslistMDSchedulerGroupConnections(DatabaseApp databaseApp, string pwClassName, string partslistNoListComaSep = null)
        {
            List<PartslistMDSchedulerGroupConnection> connections =
            databaseApp
                   .Partslist
                   .Where(c => c.MaterialWFID != null
                            && !c.DeleteDate.HasValue
                            && (partslistNoListComaSep == null || partslistNoListComaSep.Contains(c.PartslistNo)))
                   .Select(c => new { c.PartslistID, pl = c })
                   .AsEnumerable()
                   .Select(c =>
                                new
                                {
                                    PartslistID = c.PartslistID,
                                    SchedulingGroups = GetPlartslistSchedulingGroups(pwClassName, c.pl)
                                }
                    )
                   .ToList()
                   .Select(c => new PartslistMDSchedulerGroupConnection()
                   {
                       PartslistID = c.PartslistID,
                       SchedulingGroups = c.SchedulingGroups
                   })
                   .ToList();

            return connections;
        }

        private List<MDSchedulingGroup> GetPlartslistSchedulingGroups(string pwClassName, Partslist partslist)
        {
            List<MDSchedulingGroup> schedulingGroups = new List<MDSchedulingGroup>();
            if (partslist.MaterialWF != null)
            {
                var query  = partslist
                            .MaterialWF
                            .MaterialWFACClassMethod_MaterialWF
                            .Select(x => x.ACClassMethod)
                            .SelectMany(x => x.ACClassWF_ACClassMethod)
                            .Where(x =>
                                        x.RefPAACClassMethodID.HasValue
                                        && x.RefPAACClassID.HasValue
                                        && x.RefPAACClassMethod.ACKindIndex == (short)Global.ACKinds.MSWorkflow
                                        && x.RefPAACClassMethod.PWACClass != null
                                        && (x.RefPAACClassMethod.PWACClass.ACIdentifier == pwClassName
                                            || x.RefPAACClassMethod.PWACClass.ACClass1_BasedOnACClass.ACIdentifier == pwClassName)
                                        && !string.IsNullOrEmpty(x.Comment))
                            .SelectMany(x => x.MDSchedulingGroupWF_VBiACClassWF)
                            .Select(x => x.MDSchedulingGroup)
                            .GroupBy(c => c.MDSchedulingGroupID);
                            
            
                schedulingGroups = 
                    query
                    .Select(c => c.FirstOrDefault())
                    .ToList();
            }

            return schedulingGroups;
        }


        /// <summary>
        /// Get MDSchedulingGroup list ordered by WF param LineOrderInPlan
        /// 
        /// </summary>
        /// <param name="databaseApp"></param>
        /// <param name="pwClassName"></param>
        /// <param name="partslist"></param>
        /// <param name="schedulingGroupConnection"></param>
        /// <returns></returns>
        public List<MDSchedulingGroup> GetSchedulingGroups(DatabaseApp databaseApp,
                                                           string pwClassName,
                                                           Partslist partslist,
                                                           List<PartslistMDSchedulerGroupConnection> schedulingGroupConnection)
        {
            if (!schedulingGroupConnection.Where(c => c.PartslistID == partslist.PartslistID).Any())
            {
                PartslistMDSchedulerGroupConnection connection = new PartslistMDSchedulerGroupConnection();
                connection.PartslistID = partslist.PartslistID;
                connection.SchedulingGroups = GetPlartslistSchedulingGroups(pwClassName, partslist);
                schedulingGroupConnection.Add(connection);
            }

            var assignedProcessWF = partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
            if (assignedProcessWF == null)
            {
                return new List<MDSchedulingGroup>();
            }
            Guid acClassMethodID = assignedProcessWF.MaterialWFACClassMethod.ACClassMethodID;

            List<MDSchedulingGroup> schedulingGroups =
                    schedulingGroupConnection
                    .Where(c => c.PartslistID == partslist.PartslistID)
                    .SelectMany(c => c.SchedulingGroups)
                    .Where(c => c.MDSchedulingGroupWF_MDSchedulingGroup.Any(d => d.ACClassWF.ACClassMethodID == acClassMethodID))
                    .OrderBy(c => c.SortIndex)
                    .ToList();

            IEnumerable<Tuple<int, Guid>> items =
                partslist
                .PartslistConfig_Partslist
                .Where(c => !string.IsNullOrEmpty(c.LocalConfigACUrl) && c.LocalConfigACUrl.Contains("LineOrderInPlan") && c.VBiACClassWFID != null && c.Value != null)
                .ToArray()
                .Select(c => new Tuple<int, Guid>((int)c.Value, c.VBiACClassWFID.Value))
                // if IgnoreLineOrderInPlanZero == false - usual scenario
                // if IgnoreLineOrderInPlanZero == true - remove values LineOrderInPlan == 0
                .Where(c => !IgnoreLineOrderInPlanZero || c.Item1 > 0)
                .OrderBy(c => c.Item1)
                .ToArray();

            if (items != null && items.Any())
            {
                List<MDSchedulingGroup> tmpSchedulingGroups = new List<MDSchedulingGroup>();
                if (schedulingGroups != null && schedulingGroups.Any())
                    foreach (Tuple<int, Guid> item in items)
                    {
                        MDSchedulingGroup mDSchedulingGroup = schedulingGroups.Where(c => c.MDSchedulingGroupWF_MDSchedulingGroup.Any(x => x.VBiACClassWFID == item.Item2)).FirstOrDefault();
                        if (mDSchedulingGroup != null)
                            tmpSchedulingGroups.Add(mDSchedulingGroup);
                    }

                // if IgnoreLineOrderInPlanZero == false => add lines with not configured LineOrderInPlan param
                // if if IgnoreLineOrderInPlanZero == true => add lines with not configured param LineOrderInPlan or lines with LineOrderInPlan == 0
                tmpSchedulingGroups.AddRange(
                    schedulingGroups
                    .Where(c => !tmpSchedulingGroups.Select(x => x.MDSchedulingGroupID).Contains(c.MDSchedulingGroupID))
                    .OrderBy(c => c.SortIndex)
                );
                schedulingGroups = tmpSchedulingGroups;
            }
            return schedulingGroups;
        }

        public List<SchedulingMaxBPOrder> GetMaxScheduledOrder(DatabaseApp databaseApp, string planningMRNo)
        {
            GlobalApp.BatchPlanState startState = GlobalApp.BatchPlanState.Created;
            GlobalApp.BatchPlanState endState = GlobalApp.BatchPlanState.Paused;
            return
            databaseApp
                   .MDSchedulingGroup
                   .Select(c => new
                   {
                       mdGroup = c,
                       wfs =
                             c.MDSchedulingGroupWF_MDSchedulingGroup
                             .Select(x =>
                            new
                            {
                                acClWf = x.VBiACClassWF,
                                maxOrder =
                                           x.VBiACClassWF
                                           .ProdOrderBatchPlan_VBiACClassWF
                                           .Where(a =>
                                                        (
                                                            (string.IsNullOrEmpty(planningMRNo) && !a.ProdOrderPartslist.PlanningMRProposal_ProdOrderPartslist.Any())
                                                            || (!string.IsNullOrEmpty(planningMRNo) && a.ProdOrderPartslist.PlanningMRProposal_ProdOrderPartslist.Where(y => y.PlanningMR.PlanningMRNo == planningMRNo).Any())
                                                        )
                                                        && a.PlanStateIndex >= (short)startState
                                                        && a.PlanStateIndex <= (short)endState
                                            )
                                           .Select(y => y.ScheduledOrder)
                                           .DefaultIfEmpty()
                                           .Max()
                            })

                   })
                   .ToList()
                   .Select(c => new SchedulingMaxBPOrder()
                   {
                       MDSchedulingGroup = c.mdGroup,
                       WFs = c.wfs.Select(x => new SchedulingMaxBPOrderWF()
                       {
                           ACClassWF = x.acClWf,
                           MaxScheduledOrder = x.maxOrder ?? 0
                       })
                       .ToList()
                   })
                   .ToList();
        }


        #endregion

        #region ProdOrder -> Clone ProdOrder

        public ProdOrder CloneProdOrder(DatabaseApp databaseApp, ProdOrder sourceProdOrder, string planningMRNo, DateTime? scheduledStartDate, Guid[] filterProdOrderBatchPlanIds, List<SchedulingMaxBPOrder> maxSchedulerOrders = null)
        {
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(ProdOrder), ProdOrder.NoColumnName, ProdOrder.FormatNewNo, this);
            ProdOrder targetProdOrder = ProdOrder.NewACObject(databaseApp, null, secondaryKey);

            targetProdOrder.MDProdOrderState = sourceProdOrder.MDProdOrderState;
            targetProdOrder.CPartnerCompany = sourceProdOrder.CPartnerCompany;

            if (maxSchedulerOrders == null)
                maxSchedulerOrders = GetMaxScheduledOrder(databaseApp, planningMRNo);

            Dictionary<Guid, Guid> connectionOldNewItems = new Dictionary<Guid, Guid>();

            List<ProdOrderPartslist> originalPartslists = sourceProdOrder.ProdOrderPartslist_ProdOrder.OrderBy(c => c.Sequence).ToList();
            foreach (ProdOrderPartslist originalPartslist in originalPartslists)
            {
                bool isGeneratePL = !filterProdOrderBatchPlanIds.Any();
                if (!isGeneratePL)
                {
                    isGeneratePL = originalPartslist.ProdOrderBatchPlan_ProdOrderPartslist.Any(c => filterProdOrderBatchPlanIds.Contains(c.ProdOrderBatchPlanID));
                }
                if (isGeneratePL)
                    ClonePartslist(databaseApp, originalPartslist, targetProdOrder, maxSchedulerOrders, connectionOldNewItems, scheduledStartDate, filterProdOrderBatchPlanIds);
            }

            return targetProdOrder;
        }

        public ProdOrderPartslist ClonePartslist(DatabaseApp databaseApp, ProdOrderPartslist sourcePartslist, ProdOrder targetProdOrder,
            List<SchedulingMaxBPOrder> maxSchedulerOrders,
            Dictionary<Guid, Guid> connectionOldNewItems, DateTime? scheduledStartDate, Guid[] filterProdOrderBatchPlanIds)
        {
            ProdOrderPartslist targetPartslist = ProdOrderPartslist.NewACObject(databaseApp, targetProdOrder);
            connectionOldNewItems.Add(sourcePartslist.ProdOrderPartslistID, targetPartslist.ProdOrderPartslistID);
            targetProdOrder.ProdOrderPartslist_ProdOrder.Add(targetPartslist);

            targetPartslist.TargetQuantity = sourcePartslist.TargetQuantity;
            targetPartslist.IsEnabled = sourcePartslist.IsEnabled;
            targetPartslist.LossComment = sourcePartslist.LossComment;
            targetPartslist.ProdUserEndDate = sourcePartslist.ProdUserEndDate;
            targetPartslist.ProdUserEndName = sourcePartslist.ProdUserEndName;
            targetPartslist.DepartmentUserDate = sourcePartslist.DepartmentUserDate;
            targetPartslist.DepartmentUserName = sourcePartslist.DepartmentUserName;
            targetPartslist.StartDate = sourcePartslist.StartDate;
            targetPartslist.EndDate = sourcePartslist.EndDate;
            targetPartslist.Partslist = sourcePartslist.Partslist;
            targetPartslist.Sequence = sourcePartslist.Sequence;
            targetPartslist.MDProdOrderState = sourcePartslist.MDProdOrderState;
            targetPartslist.VBiACProgram = sourcePartslist.VBiACProgram;
            targetPartslist.ExternProdOrderNo = sourcePartslist.ExternProdOrderNo;

            List<ProdOrderBatchPlan> sourcebatchPlans = sourcePartslist.ProdOrderBatchPlan_ProdOrderPartslist.ToList();
            foreach (ProdOrderBatchPlan sourceBatchPlan in sourcebatchPlans)
            {
                if (!filterProdOrderBatchPlanIds.Any() || filterProdOrderBatchPlanIds.Contains(sourceBatchPlan.ProdOrderBatchPlanID))
                    CloneBatchPlan(databaseApp, sourceBatchPlan, targetPartslist, maxSchedulerOrders, connectionOldNewItems, scheduledStartDate);
            }

            List<ProdOrderPartslistPos> components = sourcePartslist.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot).ToList();
            foreach (ProdOrderPartslistPos sourcePos in components)
                CloneProdPos(databaseApp, sourcePos, targetPartslist, false, connectionOldNewItems);

            List<ProdOrderPartslistPos> mixures = sourcePartslist.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern).ToList();
            foreach (ProdOrderPartslistPos sourcePos in mixures)
            {
                KeyValuePair<Guid, Guid> pair = connectionOldNewItems.FirstOrDefault(c => c.Key == sourcePos.ProdOrderPartslistPosID);
                if (pair.Key == Guid.Empty)
                    CloneProdPos(databaseApp, sourcePos, targetPartslist, true, connectionOldNewItems);
            }

            // fix batchplans 
            foreach (ProdOrderBatchPlan sourceBatchPlan in sourcebatchPlans)
            {
                KeyValuePair<Guid, Guid> pairBatch = connectionOldNewItems.FirstOrDefault(c => c.Key == sourceBatchPlan.ProdOrderBatchPlanID);
                ProdOrderBatchPlan targetBatchPlan = targetPartslist.ProdOrderBatchPlan_ProdOrderPartslist.FirstOrDefault(c => c.ProdOrderBatchPlanID == pairBatch.Value);

                KeyValuePair<Guid, Guid> pairPos = connectionOldNewItems.FirstOrDefault(c => c.Key == sourceBatchPlan.ProdOrderPartslistPosID);
                ProdOrderPartslistPos batchPos = targetPartslist.ProdOrderPartslistPos_ProdOrderPartslist.FirstOrDefault(c => c.ProdOrderPartslistPosID == pairPos.Value);
                targetBatchPlan.ProdOrderPartslistPos = batchPos;
            }

            // Batch should not be cloned?
            //List<ProdOrderBatch> sourceBatches = sourcePartslist.ProdOrderBatch_ProdOrderPartslist.ToList();
            //foreach (ProdOrderBatch sourceBatch in sourceBatches)
            //    CloneBatch(databaseApp, sourceBatch, targetPartslist, connectionOldNewItems);

            return targetPartslist;
        }

        public ProdOrderBatchPlan CloneBatchPlan(DatabaseApp databaseApp, ProdOrderBatchPlan sourceBatchPlan, ProdOrderPartslist targetPartslist,
            List<SchedulingMaxBPOrder> maxSchedulerOrders,
            Dictionary<Guid, Guid> connectionOldNewItems, DateTime? scheduledStartDate)
        {
            ProdOrderBatchPlan targetBatchPlan = ProdOrderBatchPlan.NewACObject(databaseApp, targetPartslist);
            targetPartslist.ProdOrderBatchPlan_ProdOrderPartslist.Add(targetBatchPlan);
            connectionOldNewItems.Add(sourceBatchPlan.ProdOrderBatchPlanID, targetBatchPlan.ProdOrderBatchPlanID);

            targetBatchPlan.VBiACClassWF = sourceBatchPlan.VBiACClassWF;
            targetBatchPlan.Sequence = sourceBatchPlan.Sequence;
            targetBatchPlan.BatchNoFrom = sourceBatchPlan.BatchNoFrom;
            targetBatchPlan.BatchNoTo = sourceBatchPlan.BatchNoTo;
            targetBatchPlan.BatchTargetCount = sourceBatchPlan.BatchTargetCount;
            targetBatchPlan.BatchActualCount = sourceBatchPlan.BatchActualCount;
            targetBatchPlan.BatchSize = sourceBatchPlan.BatchSize;
            targetBatchPlan.TotalSize = sourceBatchPlan.TotalSize;
            targetBatchPlan.PlanModeIndex = sourceBatchPlan.PlanModeIndex;
            targetBatchPlan.PlanStateIndex = sourceBatchPlan.PlanStateIndex;
            targetBatchPlan.MaterialWFACClassMethod = sourceBatchPlan.MaterialWFACClassMethod;
            targetBatchPlan.IsValidated = sourceBatchPlan.IsValidated;
            //targetBatchPlan.PlannedStartDate = sourceBatchPlan.PlannedStartDate;
            targetBatchPlan.MDBatchPlanGroup = sourceBatchPlan.MDBatchPlanGroup;

            // TODO: Recalc max ScheduledOrder
            SchedulingMaxBPOrderWF schedulingMaxOrder =
                maxSchedulerOrders
                .SelectMany(c => c.WFs)
                .Where(c => c.ACClassWF.ACClassWFID == targetBatchPlan.VBiACClassWF.ACClassWFID)
                .FirstOrDefault();
            //schedulingMaxOrder.MaxScheduledOrder++;
            targetBatchPlan.ScheduledOrder = schedulingMaxOrder.MaxScheduledOrder + sourceBatchPlan.ScheduledOrder;
            targetBatchPlan.ScheduledStartDate = scheduledStartDate;
            //targetBatchPlan.ScheduledEndDate = sourceBatchPlan.ScheduledEndDate;
            //targetBatchPlan.CalculatedStartDate = sourceBatchPlan.CalculatedStartDate;
            //targetBatchPlan.CalculatedEndDate = sourceBatchPlan.CalculatedEndDate;
            targetBatchPlan.PartialTargetCount = sourceBatchPlan.PartialTargetCount;
            targetBatchPlan.PartialActualCount = sourceBatchPlan.PartialActualCount;
            targetBatchPlan.StartOffsetSecAVG = sourceBatchPlan.StartOffsetSecAVG;
            targetBatchPlan.DurationSecAVG = sourceBatchPlan.DurationSecAVG;

            List<FacilityReservation> sourceFacilityReservations = sourceBatchPlan.FacilityReservation_ProdOrderBatchPlan.ToList();
            foreach (FacilityReservation sourceFacilityReservation in sourceFacilityReservations)
            {
                KeyValuePair<Guid, Guid> facilityReservationPair = connectionOldNewItems.FirstOrDefault(c => c.Key == sourceFacilityReservation.FacilityReservationID);
                if (facilityReservationPair.Key == Guid.Empty)
                    CloneFacilityReservation(databaseApp, sourceFacilityReservation, targetBatchPlan, connectionOldNewItems);

            }

            return targetBatchPlan;
        }

        public ProdOrderBatch CloneBatch(DatabaseApp databaseApp, ProdOrderBatch sourceBatch, ProdOrderPartslist targetPartslist, Dictionary<Guid, Guid> connectionOldNewItems)
        {
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(ProdOrderBatch), ProdOrderBatch.NoColumnName, ProdOrderBatch.FormatNewNo, this);
            ProdOrderBatch targetBatch = ProdOrderBatch.NewACObject(databaseApp, targetPartslist, secondaryKey);
            targetPartslist.ProdOrderBatch_ProdOrderPartslist.Add(targetBatch);
            connectionOldNewItems.Add(sourceBatch.ProdOrderBatchID, targetBatch.ProdOrderBatchID);

            targetBatch.BatchSeqNo = sourceBatch.BatchSeqNo;
            targetBatch.MDProdOrderState = sourceBatch.MDProdOrderState;

            // ProdOrderBatchPlanID
            if (sourceBatch.ProdOrderBatchPlanID != null)
            {
                KeyValuePair<Guid, Guid> pair = connectionOldNewItems.FirstOrDefault(c => c.Key == sourceBatch.ProdOrderBatchPlanID.Value);
                if (pair.Key == Guid.Empty)
                {
                    string exceptionMessage = GetCloneExceptionMessage(typeof(ProdOrderBatch));
                    throw new Exception(exceptionMessage);
                }
                targetBatch.ProdOrderBatchPlanID = targetPartslist.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => c.ProdOrderBatchPlanID == pair.Value).Select(C => C.ProdOrderBatchPlanID).FirstOrDefault();
            }

            return targetBatch;
        }

        public ProdOrderPartslistPos CloneProdPos(DatabaseApp databaseApp, ProdOrderPartslistPos sourcePos, ProdOrderPartslist targetPartslist, bool searchRelated, Dictionary<Guid, Guid> connectionOldNewItems)
        {
            ProdOrderPartslistPos targetPos = ProdOrderPartslistPos.NewACObject(databaseApp, targetPartslist);
            targetPartslist.ProdOrderPartslistPos_ProdOrderPartslist.Add(targetPos);
            connectionOldNewItems.Add(sourcePos.ProdOrderPartslistPosID, targetPos.ProdOrderPartslistPosID);

            targetPos.Sequence = sourcePos.Sequence;
            targetPos.SequenceProduction = sourcePos.SequenceProduction;
            targetPos.MaterialPosTypeIndex = sourcePos.MaterialPosTypeIndex;
            targetPos.Material = sourcePos.Material;
            targetPos.MDUnit = sourcePos.MDUnit;
            targetPos.TargetQuantity = sourcePos.TargetQuantity;
            targetPos.ActualQuantity = sourcePos.ActualQuantity;
            targetPos.TargetQuantityUOM = sourcePos.TargetQuantityUOM;
            targetPos.ActualQuantityUOM = sourcePos.ActualQuantityUOM;
            targetPos.MDToleranceStateID = sourcePos.MDToleranceStateID;
            targetPos.IsBaseQuantityExcluded = sourcePos.IsBaseQuantityExcluded;
            targetPos.BasedOnPartslistPos = sourcePos.BasedOnPartslistPos;

            // SourceProdOrderPartslist
            if (sourcePos.SourceProdOrderPartslist != null)
            {
                KeyValuePair<Guid, Guid> pair = connectionOldNewItems.FirstOrDefault(c => c.Key == sourcePos.SourceProdOrderPartslistID.Value);
                if (pair.Key == Guid.Empty)
                {
                    string exceptionMessage = GetCloneExceptionMessage(typeof(ProdOrderPartslistPos));
                    throw new Exception(exceptionMessage);
                }
                targetPos.SourceProdOrderPartslist = targetPartslist.ProdOrder.ProdOrderPartslist_ProdOrder.Where(c => c.ProdOrderPartslistID == pair.Value).FirstOrDefault();
            }

            // ParentProdOrderPartslistPosID
            if (sourcePos.ParentProdOrderPartslistPosID != null)
            {
                KeyValuePair<Guid, Guid> pair = connectionOldNewItems.FirstOrDefault(c => c.Key == sourcePos.ParentProdOrderPartslistPosID.Value);
                if (pair.Key == Guid.Empty)
                {
                    string exceptionMessage = GetCloneExceptionMessage(typeof(ProdOrderPartslistPos));
                    throw new Exception(exceptionMessage);
                }
                targetPos.ParentProdOrderPartslistPosID = targetPartslist.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.ProdOrderPartslistID == pair.Value).Select(C => C.ProdOrderPartslistPosID).FirstOrDefault();
            }

            // AlternativeProdOrderPartslistPosID
            if (sourcePos.AlternativeProdOrderPartslistPosID != null)
            {
                KeyValuePair<Guid, Guid> pair = connectionOldNewItems.FirstOrDefault(c => c.Key == sourcePos.AlternativeProdOrderPartslistPosID.Value);
                if (pair.Key == Guid.Empty)
                {
                    string exceptionMessage = GetCloneExceptionMessage(typeof(ProdOrderPartslistPos));
                    throw new Exception(exceptionMessage);
                }
                targetPos.AlternativeProdOrderPartslistPosID = targetPartslist.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.ProdOrderPartslistID == pair.Value).Select(C => C.ProdOrderPartslistPosID).FirstOrDefault();
            }

            targetPos.FacilityLot = sourcePos.FacilityLot;
            targetPos.LineNumber = sourcePos.LineNumber;
            targetPos.MDProdOrderPartslistPosState = sourcePos.MDProdOrderPartslistPosState;
            targetPos.CalledUpQuantityUOM = sourcePos.CalledUpQuantityUOM;
            targetPos.CalledUpQuantity = sourcePos.CalledUpQuantity;
            targetPos.TakeMatFromOtherOrder = sourcePos.TakeMatFromOtherOrder;
            targetPos.RetrogradeFIFO = sourcePos.RetrogradeFIFO;

            if (searchRelated)
            {
                if (sourcePos.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Any())
                {
                    List<ProdOrderPartslistPos> childPositions = sourcePos.ProdOrderPartslistPos_ParentProdOrderPartslistPos.OrderBy(c => c.Sequence).ToList();
                    foreach (ProdOrderPartslistPos childPos in childPositions)
                    {
                        KeyValuePair<Guid, Guid> pair = connectionOldNewItems.FirstOrDefault(c => c.Key == childPos.ProdOrderPartslistPosID);
                        if (pair.Key == Guid.Empty)
                            CloneProdPos(databaseApp, childPos, targetPartslist, true, connectionOldNewItems);
                    }
                }

                List<ProdOrderPartslistPosRelation> sourceRelations = sourcePos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.OrderBy(c => c.Sequence).ToList();
                foreach (ProdOrderPartslistPosRelation sourceRelation in sourceRelations)
                {
                    ProdOrderPartslistPos nextSourcePos = null;
                    KeyValuePair<Guid, Guid> pair = connectionOldNewItems.FirstOrDefault(c => c.Key == sourceRelation.SourceProdOrderPartslistPosID);
                    if (pair.Key != Guid.Empty)
                    {
                        nextSourcePos = targetPartslist.ProdOrderPartslistPos_ProdOrderPartslist.FirstOrDefault(c => c.ProdOrderPartslistPosID == pair.Value);
                        if (nextSourcePos == null)
                            throw new Exception("Problem finding existing pos!");
                    }
                    else
                    {
                        nextSourcePos = CloneProdPos(databaseApp, sourceRelation.SourceProdOrderPartslistPos, targetPartslist, true, connectionOldNewItems);
                    }
                    KeyValuePair<Guid, Guid> pairRelation = connectionOldNewItems.FirstOrDefault(c => c.Key == sourceRelation.ProdOrderPartslistPosRelationID);
                    if (pairRelation.Key == Guid.Empty)
                        CloneProdRelation(databaseApp, sourceRelation, targetPos, nextSourcePos, connectionOldNewItems);
                }
            }


            return targetPos;
        }

        private FacilityReservation CloneFacilityReservation(DatabaseApp databaseApp, FacilityReservation sourceReservation, ProdOrderBatchPlan targetBatchPlan, Dictionary<Guid, Guid> connectionOldNewItems)
        {
            string secondaryKey = ACRoot.SRoot.NoManager.GetNewNo(databaseApp, typeof(FacilityReservation), FacilityReservation.NoColumnName, FacilityReservation.FormatNewNo, null);
            FacilityReservation targetReservation = FacilityReservation.NewACObject(databaseApp, targetBatchPlan, secondaryKey);
            connectionOldNewItems.Add(sourceReservation.FacilityReservationID, targetReservation.FacilityReservationID);
            targetBatchPlan.FacilityReservation_ProdOrderBatchPlan.Add(targetReservation);

            targetReservation.Material = sourceReservation.Material;
            targetReservation.FacilityLot = sourceReservation.FacilityLot;
            targetReservation.FacilityCharge = sourceReservation.FacilityCharge;
            targetReservation.Facility = sourceReservation.Facility;

            targetReservation.ParentFacilityReservationID = sourceReservation.ParentFacilityReservationID;
            targetReservation.VBiACClass = sourceReservation.VBiACClass;
            targetReservation.Sequence = sourceReservation.Sequence;
            targetReservation.ReservationStateIndex = sourceReservation.ReservationStateIndex;

            return targetReservation;
        }

        public ProdOrderPartslistPosRelation CloneProdRelation(DatabaseApp databaseApp, ProdOrderPartslistPosRelation sourceRel, ProdOrderPartslistPos targetPos, ProdOrderPartslistPos nextSourcePos, Dictionary<Guid, Guid> connectionOldNewItems)
        {
            ProdOrderPartslistPosRelation targetRel = ProdOrderPartslistPosRelation.NewACObject(databaseApp, null);
            targetPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Add(targetRel);
            connectionOldNewItems.Add(sourceRel.ProdOrderPartslistPosRelationID, targetRel.ProdOrderPartslistPosRelationID);

            targetRel.Sequence = sourceRel.Sequence;

            targetRel.SourceProdOrderPartslistPos = nextSourcePos;

            targetRel.TargetQuantity = sourceRel.TargetQuantity;
            targetRel.ActualQuantity = sourceRel.ActualQuantity;
            targetRel.TargetQuantityUOM = sourceRel.TargetQuantityUOM;
            targetRel.ActualQuantityUOM = sourceRel.ActualQuantityUOM;

            // ParentProdOrderPartslistPosRelationID
            if (sourceRel.ParentProdOrderPartslistPosRelationID != null)
            {
                KeyValuePair<Guid, Guid> pair = connectionOldNewItems.FirstOrDefault(c => c.Key == sourceRel.ParentProdOrderPartslistPosRelationID.Value);
                if (pair.Key == Guid.Empty)
                {
                    string exceptionMessage = GetCloneExceptionMessage(typeof(ProdOrderPartslistPosRelation));
                    throw new Exception(exceptionMessage);
                }
                targetRel.ParentProdOrderPartslistPosRelationID = targetPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Where(c => c.ProdOrderPartslistPosRelationID == pair.Value).Select(C => C.ProdOrderPartslistPosRelationID).FirstOrDefault();
            }
            // ProdOrderBatchID
            if (sourceRel.ProdOrderBatchID != null)
            {
                KeyValuePair<Guid, Guid> pair = connectionOldNewItems.FirstOrDefault(c => c.Key == sourceRel.ProdOrderBatchID.Value);
                if (pair.Key == Guid.Empty)
                {
                    string exceptionMessage = GetCloneExceptionMessage(typeof(ProdOrderBatch));
                    throw new Exception(exceptionMessage);
                }
                targetRel.ProdOrderBatch = targetPos.ProdOrderPartslist.ProdOrderBatch_ProdOrderPartslist.Where(c => c.ProdOrderBatchID == pair.Value).FirstOrDefault();
            }

            targetRel.MDToleranceState = sourceRel.MDToleranceState;
            targetRel.MDProdOrderPartslistPosState = sourceRel.MDProdOrderPartslistPosState;

            return targetRel;
        }

        private string GetCloneExceptionMessage(Type type)
        {
            string memberName = "";
            ACClassInfo acClassInfo = (ACClassInfo)Attribute.GetCustomAttribute(type, typeof(ACClassInfo));
            if (acClassInfo != null)
            {
                memberName = Translator.GetTranslation(acClassInfo.ACCaptionTranslation);
            }
            string exceptionMessage = Root.Environment.TranslateMessage(this, "Exception50001", memberName);
            return exceptionMessage;
        }

        #endregion

        #endregion

        #region Common
        /// <summary>
        /// Batch linear resize by 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factor"></param>
        public static void BatchLinearResize(ITargetQuantity item, double factor)
        {
            ITargetQuantityUOM iUOM = item as ITargetQuantityUOM;
            if (iUOM != null)
                iUOM.TargetQuantityUOM = iUOM.TargetQuantityUOM * factor;
            else
                item.TargetQuantity = item.TargetQuantity * factor;
        }

        /// <summary>
        /// Batch linear resize
        /// </summary>
        /// <param name="items"></param>
        /// <param name="factor"></param>
        public static void BatchLinearResize(IEnumerable<ITargetQuantity> items, double factor)
        {
            foreach (var item in items)
            {
                BatchLinearResize(item, factor);
            }
        }
        #endregion

        #region Statistics


        public virtual MsgWithDetails RecalcAllQuantitesAndStatistics(DatabaseApp databaseApp, ProdOrder prodOrder, bool saveChanges, ACComponent caller)
        {
            MsgWithDetails msg = new MsgWithDetails();
            try
            {
                prodOrder.RecalcActualQuantitySP(databaseApp, false);
            }
            catch (Exception ec)
            {
                Msg udpErrMsg = new Msg(eMsgLevel.Exception, $"{prodOrder.ProgramNo} error running udpRecalcActualQuantity! Message: " + ec.Message);
                msg.AddDetailMessage(udpErrMsg);
            }
            List<object> itemsForPostProcessing = new List<object>();
            MsgWithDetails calcMsg = CalculateStatistics(databaseApp, prodOrder, itemsForPostProcessing);
            MsgWithDetails saveMsg = null;
            if (saveChanges && (calcMsg == null || calcMsg.IsSucceded()))
                saveMsg = databaseApp.ACSaveChanges();

            if (calcMsg != null && calcMsg.MsgDetailsCount > 0)
                msg.AddDetailMessage(calcMsg);

            if (saveMsg != null && saveMsg.MsgDetailsCount > 0)
                msg.AddDetailMessage(saveMsg);
            else
            {
                if (CalculateOEEs && FacilityOEEManager != null)
                    FacilityOEEManager.OnPostProcessingOEE(databaseApp, prodOrder, itemsForPostProcessing);
            }

            if (msg == null || msg.MsgDetailsCount <= 0)
                return null;
            return msg;
        }

        public MsgWithDetails CalculateStatistics(DatabaseApp databaseApp, ProdOrder prodOrder, List<object> itemsForPostProcessing)
        {
            MsgWithDetails msg = new MsgWithDetails();

            List<ProdOrderPartslist> pls = prodOrder.ProdOrderPartslist_ProdOrder.OrderByDescending(c => c.Sequence).ToList();

            // Final list as nobody source
            ProdOrderPartslist finalPartslist =
                prodOrder
                .ProdOrderPartslist_ProdOrder
                .Where(c => !c.ProdOrderPartslistPos_SourceProdOrderPartslist.Any())
                .FirstOrDefault();

            if (finalPartslist != null)
            {
                MsgWithDetails plMsg = CalculateStatistics(databaseApp, finalPartslist, finalPartslist, itemsForPostProcessing);
                if (plMsg != null && plMsg.MsgDetailsCount > 0)
                {
                    msg.AddDetailMessage(plMsg);
                }
            }

            foreach (ProdOrderPartslist pl in pls)
            {
                if (pl == finalPartslist)
                    continue;
                MsgWithDetails plMsg = CalculateStatistics(databaseApp, pl, finalPartslist, itemsForPostProcessing);
                if (plMsg != null && plMsg.MsgDetailsCount > 0)
                {
                    msg.AddDetailMessage(plMsg);
                }
            }
            if (msg == null)
                return null;
            if (msg.MsgDetailsCount <= 0)
                return null;
            return msg;
        }

        public MsgWithDetails CalculateStatistics(DatabaseApp databaseApp, ProdOrderPartslist prodOrderPartslist, ProdOrderPartslist finalPartslist, List<object> itemsForPostProcessing)
        {
            MsgWithDetails msg = new MsgWithDetails();

            IEnumerable<ProdOrderPartslistPos> components =
                                databaseApp.ProdOrderPartslistPos
                                            .Include(c => c.Material)
                                            .Where(c => c.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID && c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot)
                                            .ToArray();
            //List<ProdOrderPartslistPos> components =
            //   prodOrderPartslist
            //   .ProdOrderPartslistPos_ProdOrderPartslist
            //   .Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot)
            //   .ToList();

            ProdOrderPartslistPos[] mixures =
                prodOrderPartslist
                .ProdOrderPartslistPos_ProdOrderPartslist
                .Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                .ToArray();

            ProdOrderPartslistPos finalMixure = mixures.Where(c => c.IsFinalMixure).FirstOrDefault();

            if (finalMixure == null)
                return null;

            IEnumerable<ProdOrderPartslistPos> batches =
                prodOrderPartslist
                .ProdOrderPartslistPos_ProdOrderPartslist
                .Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern && c.ParentProdOrderPartslistPosID == finalMixure.ProdOrderPartslistPosID)
                .ToArray();

            IEnumerable<FacilityBooking> facilityBookings =
                batches
                .SelectMany(c => c.FacilityBooking_ProdOrderPartslistPos)
                .ToArray();

            prodOrderPartslist.ActualQuantity =
               facilityBookings
               .Select(c => c.InwardQuantity)
               .DefaultIfEmpty()
               .Sum();

            prodOrderPartslist.ActualQuantityScrapUOM =
                facilityBookings
                .Where(c => c.MDMovementReason != null && c.MDMovementReason.MDMovementReasonIndex == (short)MovementReasonsEnum.Reject)
                .Select(c => c.InwardQuantity)
                .DefaultIfEmpty()
                .Sum();

            ProdOrderPartslistPos lastIntermediatePos = prodOrderPartslist.FinalIntermediate;

            if (prodOrderPartslist.MDProdOrderState.ProdOrderState > MDProdOrderState.ProdOrderStates.InProduction
                && lastIntermediatePos != null)
            {
                bool recalcIntermediateTargetQ = false;
                foreach (ProdOrderPartslistPos component in components)
                {
                    if (component.IsBaseQuantityExcluded) //&& !component.BasedOnPartslistPosID.HasValue
                    {
                        if (Math.Abs(component.TargetQuantityUOM - component.ActualQuantityUOM) > FacilityConst.C_ZeroCompare)
                        {
                            component.TargetQuantityUOM = component.ActualQuantityUOM;
                            recalcIntermediateTargetQ = true;
                        }
                        foreach (var relation in component.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                                            .Where(c => c.TargetProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern))
                        {
                            if (Math.Abs(relation.TargetQuantityUOM - relation.ActualQuantityUOM) > FacilityConst.C_ZeroCompare)
                            {
                                relation.TargetQuantityUOM = relation.ActualQuantityUOM;
                                recalcIntermediateTargetQ = true;
                            }
                        }
                    }
                }

                if (recalcIntermediateTargetQ)
                {
                    MDUnit finalMDUnit = lastIntermediatePos.MDUnit != null ? lastIntermediatePos.MDUnit : lastIntermediatePos.Material.BaseMDUnit;
                    RecalcIntermediateItem(lastIntermediatePos, true, finalMDUnit);
                }
            }

            ProdOrderPartslistPos lastIntermediatePosOfFinalPL = finalPartslist.FinalIntermediate;

            foreach (ProdOrderPartslistPos component in components)
            {
                MsgWithDetails msgBatch = CalculateStatistics(component, lastIntermediatePos, finalPartslist, lastIntermediatePosOfFinalPL);
                if (msgBatch != null)
                {
                    msg.AddDetailMessage(msgBatch);
                }
            }

            CalculateStatisticProdPlPerAvg(prodOrderPartslist, components);

            if (CalculateOEEs && FacilityOEEManager != null)
                FacilityOEEManager.CalculateOEE(databaseApp, prodOrderPartslist, components, lastIntermediatePos, finalPartslist, lastIntermediatePosOfFinalPL, itemsForPostProcessing);

            if (msg == null)
                return null;
            if (msg.MsgDetailsCount <= 0)
                return null;
            return msg;
        }

        private static void CalculateStatisticProdPlPerAvg(ProdOrderPartslist prodOrderPartslist, IEnumerable<ProdOrderPartslistPos> batches)
        {
            var query = batches.Select(c => c.InputQForActualOutputPer);
            prodOrderPartslist.InputQForActualOutputPer = batches.Where(c => c.InputQForActualOutputPer.HasValue).Select(c => c.InputQForActualOutputPer).DefaultIfEmpty().Average();
            prodOrderPartslist.InputQForGoodActualOutputPer = batches.Where(c => c.InputQForGoodActualOutputPer.HasValue).Select(c => c.InputQForGoodActualOutputPer).Average();
            prodOrderPartslist.InputQForScrapActualOutputPer = batches.Where(c => c.InputQForScrapActualOutputPer.HasValue).Select(c => c.InputQForScrapActualOutputPer).Average();

            prodOrderPartslist.InputQForFinalActualOutputPer = batches.Where(c => c.InputQForFinalActualOutputPer.HasValue).Select(c => c.InputQForFinalActualOutputPer).Average();
            prodOrderPartslist.InputQForFinalGoodActualOutputPer = batches.Where(c => c.InputQForFinalGoodActualOutputPer.HasValue).Select(c => c.InputQForFinalGoodActualOutputPer).Average();
            prodOrderPartslist.InputQForFinalScrapActualOutputPer = batches.Where(c => c.InputQForFinalScrapActualOutputPer.HasValue).Select(c => c.InputQForFinalScrapActualOutputPer).Average();
        }

        private static void CalculateStatisticProdPlPerGemetricalAvg(ProdOrderPartslist prodOrderPartslist, IEnumerable<ProdOrderPartslistPos> components)
        {
            double sumComponentQuantities = components.Select(c => c.TargetQuantityUOM).Sum();
            if (sumComponentQuantities == 0)
                return;

            prodOrderPartslist.InputQForActualOutputPer = components.Where(c => c.InputQForActualOutputPer.HasValue).Select(c => c.InputQForActualOutputPer * c.TargetQuantityUOM).DefaultIfEmpty().Sum() / sumComponentQuantities;
            prodOrderPartslist.InputQForGoodActualOutputPer = components.Where(c => c.InputQForGoodActualOutputPer.HasValue).Select(c => c.InputQForGoodActualOutputPer * c.TargetQuantityUOM).DefaultIfEmpty().Sum() / sumComponentQuantities;
            prodOrderPartslist.InputQForScrapActualOutputPer = components.Where(c => c.InputQForScrapActualOutputPer.HasValue).Select(c => c.InputQForScrapActualOutputPer * c.TargetQuantityUOM).DefaultIfEmpty().Sum() / sumComponentQuantities;


            prodOrderPartslist.InputQForFinalActualOutputPer = components.Where(c => c.InputQForFinalActualOutputPer.HasValue).Select(c => c.InputQForFinalActualOutputPer * c.TargetQuantityUOM).DefaultIfEmpty().Sum() / sumComponentQuantities;
            prodOrderPartslist.InputQForFinalGoodActualOutputPer = components.Where(c => c.InputQForFinalGoodActualOutputPer.HasValue).Select(c => c.InputQForFinalGoodActualOutputPer * c.TargetQuantityUOM).DefaultIfEmpty().Sum() / sumComponentQuantities;
            prodOrderPartslist.InputQForFinalScrapActualOutputPer = components.Where(c => c.InputQForFinalScrapActualOutputPer.HasValue).Select(c => c.InputQForFinalScrapActualOutputPer * c.TargetQuantityUOM).DefaultIfEmpty().Sum() / sumComponentQuantities;
        }

        public MsgWithDetails CalculateStatistics(ProdOrderPartslistPos prodOrderPartslistPos, ProdOrderPartslistPos lastIntermediatePos, ProdOrderPartslist finalPartslist, ProdOrderPartslistPos lastIntermediatePosOfFinalPL)
        {
            MsgWithDetails msg = null;

            ProdOrderPartslist partslist = prodOrderPartslistPos.ProdOrderPartslist;
            double targetQForRatio = lastIntermediatePos != null ? lastIntermediatePos.TargetQuantityUOM : partslist.TargetQuantity;

            if (targetQForRatio > 0)
            {
                prodOrderPartslistPos.InputQForActualOutput =
                (partslist.ActualQuantity / targetQForRatio) * prodOrderPartslistPos.TargetQuantityUOM;

                prodOrderPartslistPos.InputQForScrapActualOutput =
                  (partslist.ActualQuantityScrapUOM / targetQForRatio) * prodOrderPartslistPos.TargetQuantity;

                prodOrderPartslistPos.InputQForGoodActualOutput = prodOrderPartslistPos.InputQForActualOutput - prodOrderPartslistPos.InputQForScrapActualOutput;
            }
            else
            {
                prodOrderPartslistPos.InputQForActualOutput = null;
                prodOrderPartslistPos.InputQForScrapActualOutput = null;
                prodOrderPartslistPos.InputQForGoodActualOutput = null;
            }

            targetQForRatio = lastIntermediatePosOfFinalPL != null ? lastIntermediatePosOfFinalPL.TargetQuantityUOM : finalPartslist.TargetQuantity;

            if (targetQForRatio > 0)
            {
                prodOrderPartslistPos.InputQForFinalActualOutput =
                 (finalPartslist.ActualQuantity / targetQForRatio) * prodOrderPartslistPos.TargetQuantityUOM;

                prodOrderPartslistPos.InputQForFinalScrapActualOutput =
                     (finalPartslist.ActualQuantityScrapUOM / targetQForRatio) * prodOrderPartslistPos.TargetQuantity;

                prodOrderPartslistPos.InputQForFinalGoodActualOutput = prodOrderPartslistPos.InputQForFinalActualOutput - prodOrderPartslistPos.InputQForFinalScrapActualOutput;
            }
            else
            {
                prodOrderPartslistPos.InputQForFinalActualOutput = null;
                prodOrderPartslistPos.InputQForFinalScrapActualOutput = null;
                prodOrderPartslistPos.InputQForFinalGoodActualOutput = null;
            }


            return msg;
        }

        #endregion

    }

}
