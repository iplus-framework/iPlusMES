// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using static gip.mes.datamodel.MDReservationMode;

namespace gip.mes.facility
{
    public partial class ACPartslistManager
    {
        #region enums
        public enum SearchMode
        {
            /// <summary>
            /// Priorisierte Liste mit Silos, unabhängig davon ob Sie eine Entleerfreigabe haben oder nicht
            /// </summary>
            AllSilos,
            /// <summary>
            /// Priorisierte Liste mit Silos die eine Entleerfreigabe haben
            /// </summary>
            SilosWithOutwardEnabled,
            /// <summary>
            /// Immer nur ältestes Silo zurückgeben
            /// </summary>
            OnlyEnabledOldestSilo
        }
        #endregion

        #region Result
        public class QrySilosResult : ICloneable
        {
            public QrySilosResult()
            {
            }

            public QrySilosResult(IEnumerable<FacilityCharge> foundQuants, IPartslistPos doseAlternativeMat = null)
            {
                _GroupedStoresWithQuants = foundQuants != null && foundQuants.Any() ? foundQuants.Distinct().GroupBy(c => c.Facility) : null;
                if (_GroupedStoresWithQuants != null)
                    _FilteredResult = _GroupedStoresWithQuants.Select(c => new FacilitySumByLots() { StorageBin = c.Key, FacilityCharges = c.Select(d => new FacilityChargeInfo() { Quant = d }).ToList() }).ToList();
                _DoseAlternativeMat = doseAlternativeMat;
            }

            #region Internal Classes
            public class ReservationInfo
            {
                public Guid FacilityLotID { get; set; }
                public double? Quantity { get; set; }
                public short ReservationStateIndex { get; set; }
                public GlobalApp.ReservationState State { get { return (GlobalApp.ReservationState)ReservationStateIndex; } }
                public bool IsQuantityObservable { get { return State != GlobalApp.ReservationState.New; } }
                public double? ActualQuantity { get; set; }
                public double? RestQuantity
                {
                    get
                    {
                        if (Quantity.HasValue && ActualQuantity.HasValue)
                            return ActualQuantity - Quantity;
                        else if (Quantity.HasValue)
                            return 0.0 - Quantity.Value;
                        return null;
                    }
                }
                public static void UpdateActualQFromResCollection(IEnumerable<ReservationInfo> reservationsToUpdate, IEnumerable<ReservationInfo> fromCollection)
                {
                    if (reservationsToUpdate == null || fromCollection == null)
                        return;
                    foreach (ReservationInfo from in fromCollection)
                    {
                        ReservationInfo to = reservationsToUpdate.FirstOrDefault(c => c.FacilityLotID == from.FacilityLotID);
                        if (to != null)
                            to.ActualQuantity = from.ActualQuantity;
                    }
                }
            }

            public class FacilityChargeInfo
            {
                public FacilityCharge Quant { get; set; }
                public bool IsReservedLot { get; set; }
            }

            public class FacilitySumByLots
            {
                public Facility StorageBin { get; set; }

                private double? _StockOfReservations = null;
                public double? StockOfReservations
                {
                    get
                    {
                        if (_StockOfReservations == null && _StockFree == null)
                            Sum();
                        return _StockOfReservations;
                    }
                }

                private double? _StockFree = null;
                public double? StockFree
                {
                    get
                    {
                        if (_StockOfReservations == null && _StockFree == null)
                            Sum();
                        return _StockFree;
                    }
                }

                public double Stock
                {
                    get
                    {
                        return (StockOfReservations.HasValue ? StockOfReservations.Value : 0.0) + (StockFree.HasValue ? StockFree.Value : 0.0);
                    }
                }

                public List<FacilityChargeInfo> FacilityCharges { get; set; }

                private DateTime? _OldestQuantDate;
                public DateTime OldestQuantDate
                {
                    get
                    {
                        if (_OldestQuantDate == null)
                            Sum();
                        return _OldestQuantDate.Value;
                    }
                }

                private void Sum()
                {
                    _OldestQuantDate = DateTime.MaxValue;
                    if (FacilityCharges == null)
                        return;
                    _StockOfReservations = null;
                    _StockFree = null;
                    foreach (FacilityChargeInfo fcInfo in FacilityCharges)
                    {
                        if (fcInfo.Quant.FillingDate.HasValue)
                        {
                            if (fcInfo.Quant.FillingDate.Value < _OldestQuantDate)
                                _OldestQuantDate = fcInfo.Quant.FillingDate.Value;
                        }
                        else
                        {
                            if (fcInfo.Quant.InsertDate < _OldestQuantDate)
                                _OldestQuantDate = fcInfo.Quant.InsertDate;
                        }

                        if (fcInfo.IsReservedLot)
                        {
                            if (!_StockOfReservations.HasValue)
                                _StockOfReservations = 0.0;
                            _StockOfReservations += fcInfo.Quant.StockQuantityUOM;
                        }
                        else
                        {
                            if (!_StockFree.HasValue)
                                _StockFree = 0.0;
                            _StockFree += fcInfo.Quant.StockQuantityUOM;
                        }
                    }
                }

                private List<Tuple<Guid, double>> _StockPerLot;
                public IEnumerable<Tuple<Guid, double>> StockPerLot
                {
                    get
                    {
                        if (_StockPerLot != null)
                            return _StockPerLot;
                        _StockPerLot = FacilityCharges
                            .Where(c => c.IsReservedLot)
                            .Select(c => new { LotID = c.Quant.FacilityLotID, Q = c.Quant.StockQuantityUOM })
                            .GroupBy(c => c.LotID.Value)
                            .Select(g => new Tuple<Guid, double>(g.Key, g.Sum(e => e.Q)))
                            .ToList();
                        return _StockPerLot;
                    }
                }
            }
            #endregion

            #region Properties
            public IEnumerable<Facility> FoundSilos
            {
                get
                {
                    return SortedFilteredResult.Select(c => c.StorageBin);
                }
            }

            /// <summary>
            /// Result sorted by Oldest Quant Date
            /// </summary>
            public IOrderedEnumerable<FacilitySumByLots> SortedFilteredResult
            {
                get
                {
                    return FilteredResult.OrderBy(c => c.OldestQuantDate);
                }
            }

            private List<FacilitySumByLots> _FilteredResult;
            public List<FacilitySumByLots> FilteredResult
            {
                get
                {
                    if (_FilteredResult == null)
                        _FilteredResult = new List<FacilitySumByLots>();
                    return _FilteredResult;
                }
                set
                {
                    _FilteredResult = value;
                }
            }

            private IEnumerable<IGrouping<Facility, FacilityCharge>> _GroupedStoresWithQuants;
            public IEnumerable<IGrouping<Facility, FacilityCharge>> GroupedStoresWithQuants
            {
                get
                {
                    return _GroupedStoresWithQuants;
                }
            }

            private ReservationInfo[] _ReservationInfos;
            public IEnumerable<ReservationInfo> ReservationInfos
            {
                get
                {
                    return _ReservationInfos;
                }
            }

            private IPartslistPos _DoseAlternativeMat;
            public IPartslistPos DoseAlternativeMat
            {
                get
                {
                    return _DoseAlternativeMat;
                }
            }

            private bool _HasLotReservations;
            public bool HasLotReservations
            {
                get
                {
                    return _HasLotReservations;
                }
            }
            #endregion

            #region Methods
            public void ApplyBlockedQuantsFilter()
            {
                if (_FilteredResult == null || !_FilteredResult.Any())
                    return;
                _FilteredResult = _FilteredResult.Where(f => !f.FacilityCharges.Where(fc => Facility.FuncHasBlockedQuants(fc.Quant)).Any())
                                                    .ToList();
            }

            public void ApplyFreeQuantsInBinFilter()
            {
                if (_FilteredResult == null || !_FilteredResult.Any())
                    return;
                _FilteredResult = _FilteredResult.Where(f => (f.StorageBin.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBin && f.FacilityCharges.Where(fc => Facility.FuncHasFreeQuants(fc.Quant)).Any())
                                                            || !f.FacilityCharges.Where(fc => Facility.FuncHasBlockedQuants(fc.Quant)).Any())
                                                    .ToList();
            }

            public void ApplyLotReservationFilter(ProdOrderPartslistPosRelation poRelation, short reservationMode)
            {
                if (_FilteredResult == null || !_FilteredResult.Any())
                    return;
                if (poRelation.SourceProdOrderPartslistPos != null && poRelation.SourceProdOrderPartslistPos.FacilityReservation_ProdOrderPartslistPos.Any())
                {
                    _ReservationInfos = poRelation.SourceProdOrderPartslistPos.FacilityReservation_ProdOrderPartslistPos
                        .Where(c => c.FacilityLotID.HasValue)
                        .Select(c => new ReservationInfo() { FacilityLotID = c.FacilityLotID.Value, Quantity = c.ReservedQuantityUOM, ReservationStateIndex = c.ReservationStateIndex })
                        .ToArray();
                }
                if (_ReservationInfos != null && _ReservationInfos.Any())
                {
                    Guid[] reservedLots = _ReservationInfos.Select(c => c.FacilityLotID).ToArray();
                    _HasLotReservations = true;
                    if (_FilteredResult != null && _FilteredResult.Any())
                    {
                        _FilteredResult.ForEach(c => c.FacilityCharges.ForEach(d => d.IsReservedLot = d.Quant.FacilityLotID.HasValue && reservedLots.Contains(d.Quant.FacilityLotID.Value)));
                        // Sort List first by silos with reserved Lots then append the rest
                        if (reservationMode == 1)
                        {
                            List<FacilitySumByLots> binsWithReservedLots = _FilteredResult
                                .Where(c => c.StockOfReservations.HasValue)
                                .ToList();
                            foreach (var bin in _FilteredResult)
                            {
                                if (!bin.StockOfReservations.HasValue)
                                    binsWithReservedLots.Add(bin);
                            }
                            _FilteredResult = binsWithReservedLots;
                        }
                        // Select only that have reserved lots
                        else
                        {
                            _FilteredResult = _FilteredResult
                                .Where(c => c.FacilityCharges.Where(d => d.IsReservedLot).Any())
                                .ToList();
                        }
                    }
                }
            }

            public void RemoveFacility(Guid? ignoreFacilityID, IEnumerable<gip.core.datamodel.ACClass> exclusionList)
            {
                if (_FilteredResult == null || !_FilteredResult.Any())
                    return;
                if (ignoreFacilityID.HasValue)
                {
                    FacilitySumByLots facilityToRemove = _FilteredResult.Where(c => c.StorageBin.FacilityID == ignoreFacilityID.Value).FirstOrDefault();
                    if (facilityToRemove != null)
                    {
                        _FilteredResult.Remove(facilityToRemove);
                    }
                }
                if (exclusionList != null)
                {
                    foreach (gip.core.datamodel.ACClass silo2Exlude in exclusionList)
                    {
                        FacilitySumByLots facilityToRemove = _FilteredResult.Where(c => c.StorageBin.VBiFacilityACClassID.HasValue && c.StorageBin.VBiFacilityACClassID == silo2Exlude.ACClassID).FirstOrDefault();
                        if (facilityToRemove != null)
                        {
                            _FilteredResult.Remove(facilityToRemove);
                        }
                    }
                }
            }

            public void ApplyRoutableSilos(IEnumerable<gip.core.datamodel.ACClass> routableSilos)
            {
                if (_FilteredResult == null || !_FilteredResult.Any() || routableSilos == null)
                    return;

                List<FacilitySumByLots> tempList = new List<FacilitySumByLots>();

                List<Guid> routableSiloIDs = routableSilos.Select(c => c.ACClassID).ToList();

                foreach (FacilitySumByLots item in _FilteredResult)
                {
                    if (item.StorageBin.VBiFacilityACClassID.HasValue && routableSiloIDs.Contains(item.StorageBin.VBiFacilityACClassID.Value))
                        tempList.Add(item);
                }

                _FilteredResult = tempList;
            }

            public void ApplyLotReservationFilter(PickingPos pickingPos, short reservationMode, IList<Guid> allowedFacilities = null)
            {
                if (_FilteredResult == null || !_FilteredResult.Any())
                    return;
                if (pickingPos.FacilityReservation_PickingPos.Any())
                {
                    _ReservationInfos = pickingPos.FacilityReservation_PickingPos
                        .Where(c => c.FacilityLotID.HasValue)
                        .Select(c => new ReservationInfo() { FacilityLotID = c.FacilityLotID.Value, Quantity = c.ReservedQuantityUOM, ReservationStateIndex = c.ReservationStateIndex })
                        .ToArray();
                }
                BuildFilteredResultFromReservationInfo(reservationMode, allowedFacilities);
            }

            private void BuildFilteredResultFromReservationInfo(short reservationMode, IList<Guid> allowedFacilities = null)
            {
                if (_ReservationInfos == null || !_ReservationInfos.Any())
                    return;
                Guid[] reservedLots = _ReservationInfos.Select(c => c.FacilityLotID).ToArray();
                _HasLotReservations = true;
                if (_FilteredResult == null || !_FilteredResult.Any())
                    return;
                _FilteredResult.ForEach(c => c.FacilityCharges.ForEach(d => d.IsReservedLot = d.Quant.FacilityLotID.HasValue && reservedLots.Contains(d.Quant.FacilityLotID.Value)));
                // Sort List first by silos with reserved Lots then append the rest
                if (reservationMode == 1)
                {
                    List<FacilitySumByLots> binsWithReservedLots = _FilteredResult
                        .Where(c => c.StockOfReservations.HasValue)
                        .ToList();
                    foreach (var bin in _FilteredResult)
                    {
                        if (!bin.StockOfReservations.HasValue)
                            binsWithReservedLots.Add(bin);
                    }
                    _FilteredResult = binsWithReservedLots;
                }
                // Select only that have reserved lots
                else
                {
                    _FilteredResult = _FilteredResult
                        .Where(c => c.FacilityCharges.Where(d => d.IsReservedLot).Any())
                        .ToList();
                }
                if (allowedFacilities != null && allowedFacilities.Any())
                    _FilteredResult = _FilteredResult.Where(c => allowedFacilities.Contains(c.StorageBin.FacilityID)).ToList();
            }

            public object Clone()
            {
                QrySilosResult result = new QrySilosResult();
                if (this.GroupedStoresWithQuants != null)
                    result._GroupedStoresWithQuants = this.GroupedStoresWithQuants.ToList();
                if (this._FilteredResult != null)
                    result._FilteredResult = this._FilteredResult.ToList();
                return result;
            }

            #endregion
        }
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

        private ACPropertyConfigValue<short> _FindSiloModes;
        [ACPropertyConfig("en{'find Silo mode'}de{'Finde Silo Modus'}")]
        public short FindSiloModes
        {
            get
            {
                return _FindSiloModes.ValueT;
            }
            set
            {
                _FindSiloModes.ValueT = value;
            }
        }
        #endregion

        #region Precompiled Queries


        #region Partslist

        #region Queries with Time filter

        #region Search ignoring previous production stage
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>> s_cQry_PlSilosWithMaterialTime =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, filterTimeOlderThan, checkOutwardEnabled, onlyContainer) => ctx.FacilityCharge
                                                                    .Include("Facility.FacilityStock_Facility")
                                                                    .Include("MDReleaseState")
                                                                    .Include("FacilityLot.MDReleaseState")
                                                                    .Include("Facility.MDFacilityType")
                                                                    .Where(c => c.NotAvailable == false
                                                                           && ( (onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                                              ||(!onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.Facility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                                           && (   (!onlyContainer 
                                                                                    && (   (posProdMaterialId.HasValue && (c.MaterialID == posProdMaterialId || (c.Material.ProductionMaterialID.HasValue && c.Material.ProductionMaterialID == posProdMaterialId)))
                                                                                        || (!posProdMaterialId.HasValue && c.MaterialID == posMaterialId)))
                                                                               || (onlyContainer && c.Facility.MaterialID.HasValue
                                                                                    && (   (posProdMaterialId.HasValue && c.Facility.MaterialID == posProdMaterialId)
                                                                                        || (!posProdMaterialId.HasValue && c.Facility.MaterialID == posMaterialId)))
                                                                              )
                                                                           && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                                              || !checkOutwardEnabled)
                                                                           && (    (c.Facility.MinStockQuantity.HasValue && c.Facility.MinStockQuantity.Value < -0.1)
                                                                                || (c.FillingDate.HasValue && c.FillingDate <= filterTimeOlderThan)))
                                                                    .OrderBy(c => c.FillingDate)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// AND is older than the filter-Date
        /// </summary>
        virtual public QrySilosResult SilosWithMaterialTime(DatabaseApp ctx,
                                                            PartslistPosRelation relation,
                                                            DateTime filterTimeOlderThan,
                                                            bool checkOutwardEnabled = true,
                                                            bool searchForAlternativeMaterials = false,
                                                            ACValueList projSpecificParams = null,
                                                            bool onlyContainer = true)
        {
            try
            {
                PartslistPos pos = relation.SourcePartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PlSilosWithMaterialTime(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.PartslistPos_AlternativePartslistPos.Any())
                {
                    foreach (PartslistPos altPos in pos.PartslistPos_AlternativePartslistPos)
                    {
                        var result = s_cQry_PlSilosWithMaterialTime(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithMaterialTime", msg);

                return new QrySilosResult();
            }
        }

        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>> s_cQry_PlSilosWithIntermediateMaterialTime =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, posPartslistId, filterTimeOlderThan, checkOutwardEnabled, onlyContainer) => ctx.FacilityCharge
                                                                    .Include("Facility.FacilityStock_Facility")
                                                                    .Include("MDReleaseState")
                                                                    .Include("FacilityLot.MDReleaseState")
                                                                    .Include("Facility.MDFacilityType")
                                                                    .Where(c => c.NotAvailable == false
                                                                           && (  (onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                                              || (!onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.Facility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                                           && (  (!onlyContainer
                                                                                    && ((posProdMaterialId.HasValue && (c.MaterialID == posProdMaterialId || (c.Material.ProductionMaterialID.HasValue && c.Material.ProductionMaterialID == posProdMaterialId)))
                                                                                        || (!posProdMaterialId.HasValue && c.MaterialID == posMaterialId)))
                                                                               || (onlyContainer && c.Facility.MaterialID.HasValue
                                                                                    && ((posProdMaterialId.HasValue && c.Facility.MaterialID == posProdMaterialId)
                                                                                        || (!posProdMaterialId.HasValue && c.Facility.MaterialID == posMaterialId)))
                                                                              )
                                                                           && (c.Facility.PartslistID.HasValue
                                                                              && c.Facility.PartslistID == posPartslistId)
                                                                           && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                                              || !checkOutwardEnabled)
                                                                           && ((c.Facility.MinStockQuantity.HasValue && c.Facility.MinStockQuantity.Value < -0.1)
                                                                                || (c.FillingDate.HasValue && c.FillingDate <= filterTimeOlderThan)))
                                                                    .OrderBy(c => c.FillingDate)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// AND is older than the filter-Date
        /// </summary>
        virtual public QrySilosResult SilosWithIntermediateMaterialTime(DatabaseApp ctx,
                                                                       PartslistPosRelation relation,
                                                                       DateTime filterTimeOlderThan,
                                                                       bool checkOutwardEnabled = true,
                                                                       bool searchForAlternativeMaterials = false,
                                                                       ACValueList projSpecificParams = null,
                                                                       bool onlyContainer = true)
        {
            try
            {
                PartslistPos pos = relation.SourcePartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PlSilosWithIntermediateMaterialTime(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, pos.PartslistID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.PartslistPos_AlternativePartslistPos.Any())
                {
                    foreach (PartslistPos altPos in pos.PartslistPos_AlternativePartslistPos)
                    {
                        var result = s_cQry_PlSilosWithIntermediateMaterialTime(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, altPos.PartslistID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "StartWithIntermediateMaterialTime", msg);

                return new QrySilosResult();
            }
        }

        #endregion

        #endregion


        #region Queries without time filter

        #region Search ignoring previous production stage

        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>> s_cQry_PlSilosWithMaterial =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, checkOutwardEnabled, onlyContainer) => ctx.FacilityCharge
                                                .Include("Facility.FacilityStock_Facility")
                                                .Include("MDReleaseState")
                                                .Include("FacilityLot.MDReleaseState")
                                                .Include("Facility.MDFacilityType")
                                                .Where(c => c.NotAvailable == false
                                                      && (   (onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                          || (!onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.Facility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                      && (   (!onlyContainer
                                                                && ((posProdMaterialId.HasValue && (c.MaterialID == posProdMaterialId || (c.Material.ProductionMaterialID.HasValue && c.Material.ProductionMaterialID == posProdMaterialId)))
                                                                    || (!posProdMaterialId.HasValue && c.MaterialID == posMaterialId)))
                                                          || (  onlyContainer &&  c.Facility.MaterialID.HasValue
                                                                && (   (posProdMaterialId.HasValue && c.Facility.MaterialID == posProdMaterialId)
                                                                    || (!posProdMaterialId.HasValue && c.Facility.MaterialID == posMaterialId)))
                                                            )
                                                      && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                          || !checkOutwardEnabled)
                                                      && c.FillingDate.HasValue)
                                               .OrderBy(c => c.FillingDate)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// </summary>
        virtual public QrySilosResult SilosWithMaterial(DatabaseApp ctx,
                                                        PartslistPosRelation relation,
                                                        bool checkOutwardEnabled = true,
                                                        bool searchForAlternativeMaterials = false,
                                                        ACValueList projSpecificParams = null,
                                                        bool onlyContainer = true)
        {
            try
            {
                PartslistPos pos = relation.SourcePartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PlSilosWithMaterial(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.PartslistPos_AlternativePartslistPos.Any())
                {
                    foreach (PartslistPos altPos in pos.PartslistPos_AlternativePartslistPos)
                    {
                        var result = s_cQry_PlSilosWithMaterial(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithMaterial", msg);

                return new QrySilosResult();
            }
        }


        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>> s_cQry_PlSilosWithIntermediateMaterial =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, posPartslistId, checkOutwardEnabled, onlyContainer) => ctx.FacilityCharge
                                                .Include("Facility.FacilityStock_Facility")
                                                .Include("MDReleaseState")
                                                .Include("FacilityLot.MDReleaseState")
                                                .Include("Facility.MDFacilityType")
                                                .Where(c => c.NotAvailable == false
                                                      && (   (onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                          || (!onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.Facility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                      && (( !onlyContainer
                                                            && ((posProdMaterialId.HasValue && (c.MaterialID == posProdMaterialId || (c.Material.ProductionMaterialID.HasValue && c.Material.ProductionMaterialID == posProdMaterialId)))
                                                                || (!posProdMaterialId.HasValue && c.MaterialID == posMaterialId)))
                                                         || (onlyContainer && c.Facility.MaterialID.HasValue
                                                             && (   (posProdMaterialId.HasValue && c.Facility.MaterialID == posProdMaterialId)
                                                                 || (!posProdMaterialId.HasValue && c.Facility.MaterialID == posMaterialId)))
                                                            )
                                                      && (  !onlyContainer
                                                         || (c.Facility.PartslistID.HasValue && c.Facility.PartslistID == posPartslistId))
                                                      && (  (checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                         || !checkOutwardEnabled)
                                                      && c.FillingDate.HasValue)
                                               .OrderBy(c => c.FillingDate)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// AND is older than the filter-Date
        /// </summary>
        virtual public QrySilosResult SilosWithIntermediateMaterial(DatabaseApp ctx,
            PartslistPosRelation relation,
            bool checkOutwardEnabled = true,
            bool searchForAlternativeMaterials = false,
            ACValueList projSpecificParams = null,
            bool onlyContainer = true)
        {
            try
            {
                PartslistPos pos = relation.SourcePartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PlSilosWithIntermediateMaterial(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, pos.PartslistID, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.PartslistPos_AlternativePartslistPos.Any())
                {
                    foreach (PartslistPos altPos in pos.PartslistPos_AlternativePartslistPos)
                    {
                        var result = s_cQry_PlSilosWithIntermediateMaterial(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, altPos.PartslistID, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithIntermediateMaterial", msg);

                return new QrySilosResult();
            }
        }

        #endregion

        #endregion

        #endregion


        #region ProdOrder

        #region Queries with Time filter

        #region Search ignoring previous production stage
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>> s_cQry_PoSilosWithMaterialTime =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, filterTimeOlderThan, checkOutwardEnabled, onlyContainer) => ctx.FacilityCharge
                                                                            .Include("Facility.FacilityStock_Facility")
                                                                            .Include("MDReleaseState")
                                                                            .Include("FacilityLot.MDReleaseState")
                                                                            .Include("Facility.MDFacilityType")
                                                                            .Where(c => c.NotAvailable == false
                                                                                       && (  (onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                                                          || (!onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.Facility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                                                       && (   (!onlyContainer
                                                                                                && ((posProdMaterialId.HasValue && (c.MaterialID == posProdMaterialId || (c.Material.ProductionMaterialID.HasValue && c.Material.ProductionMaterialID == posProdMaterialId)))
                                                                                                    || (!posProdMaterialId.HasValue && c.MaterialID == posMaterialId)))
                                                                                           || (onlyContainer && c.Facility.MaterialID.HasValue
                                                                                                && (   (posProdMaterialId.HasValue && c.Facility.MaterialID == posProdMaterialId)
                                                                                                    || (!posProdMaterialId.HasValue && c.Facility.MaterialID == posMaterialId)))
                                                                                            )
                                                                                       && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                                                          || !checkOutwardEnabled)
                                                                                        && (   (c.Facility.MinStockQuantity.HasValue && c.Facility.MinStockQuantity.Value < -0.1)
                                                                                            || (c.FillingDate.HasValue && c.FillingDate <= filterTimeOlderThan)))
                                                                           .OrderBy(c => c.FillingDate)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// AND is older than the filter-Date
        /// </summary>
        virtual public QrySilosResult SilosWithMaterialTime(DatabaseApp ctx,
                                                            ProdOrderPartslistPosRelation relation,
                                                            DateTime filterTimeOlderThan,
                                                            bool checkOutwardEnabled = true,
                                                            bool searchForAlternativeMaterials = false,
                                                            ACValueList projSpecificParams = null,
                                                            bool onlyContainer = true)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosWithMaterialTime(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosWithMaterialTime(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithMaterialTime", msg);

                return new QrySilosResult();
            }
        }

        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>> s_cQry_PoSilosWithIntermediateMaterialTime =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, posProdOrderPartsListID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer) => ctx.FacilityCharge
                                                                    .Include("Facility.FacilityStock_Facility")
                                                                    .Include("MDReleaseState")
                                                                    .Include("FacilityLot.MDReleaseState")
                                                                    .Include("Facility.MDFacilityType")
                                                                    .Where(c => c.NotAvailable == false
                                                                           && (  (onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                                              || (!onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.Facility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                                           && (   (!onlyContainer
                                                                                    && ((posProdMaterialId.HasValue && (c.MaterialID == posProdMaterialId || (c.Material.ProductionMaterialID.HasValue && c.Material.ProductionMaterialID == posProdMaterialId)))
                                                                                        || (!posProdMaterialId.HasValue && c.MaterialID == posMaterialId)))
                                                                                || (onlyContainer && c.Facility.MaterialID.HasValue
                                                                                    && (   (posProdMaterialId.HasValue && c.Facility.MaterialID == posProdMaterialId)
                                                                                        || (!posProdMaterialId.HasValue && c.Facility.MaterialID == posMaterialId)))
                                                                                )
                                                                           && (     !onlyContainer
                                                                                || (   c.Facility.PartslistID.HasValue
                                                                                    && posProdOrderPartsListID.HasValue
                                                                                    && c.Facility.PartslistID == posProdOrderPartsListID)
                                                                              )
                                                                           && (  (checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                                              || !checkOutwardEnabled)
                                                                           && (  (c.Facility.MinStockQuantity.HasValue && c.Facility.MinStockQuantity.Value < -0.1)
                                                                               || (c.FillingDate.HasValue && c.FillingDate <= filterTimeOlderThan)))
                                                                    .OrderBy(c => c.FillingDate)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// AND is older than the filter-Date
        /// </summary>
        virtual public QrySilosResult SilosWithIntermediateMaterialTime(DatabaseApp ctx,
                                                                       ProdOrderPartslistPosRelation relation,
                                                                       DateTime filterTimeOlderThan,
                                                                       bool checkOutwardEnabled = true,
                                                                       bool searchForAlternativeMaterials = false,
                                                                       ACValueList projSpecificParams = null,
                                                                       bool onlyContainer = true)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosWithIntermediateMaterialTime(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, pos.ProdOrderPartslist.PartslistID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosWithIntermediateMaterialTime(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, altPos.ProdOrderPartslist.PartslistID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithIntermediateMaterialTime", msg);

                return new QrySilosResult();
            }
        }

        #endregion

        #region Search includes previous production stage
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// AND must pe produced form the same order in a previous stage
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>> s_cQry_PoSilosFromPrevStageTime =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, posSourceProdOrderPartslistId, filterTimeOlderThan, checkOutwardEnabled, onlyContainer) => ctx.FacilityBookingCharge
                                                                    .Include("InwardFacility.FacilityStock_Facility")
                                                                    .Include("InwardFacilityCharge.MDReleaseState")
                                                                    .Include("InwardFacilityCharge.FacilityLot.MDReleaseState")
                                                                    .Include("InwardFacility.MDFacilityType")
                                                                    .Where(c => c.ProdOrderPartslistPosID.HasValue
                                                                            && posSourceProdOrderPartslistId.HasValue
                                                                            && posMaterialId.HasValue
                                                                            && c.ProdOrderPartslistPos.ProdOrderPartslistID == posSourceProdOrderPartslistId
                                                                            && c.InwardFacilityID.HasValue 
                                                                            && (   (onlyContainer && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                                                || (!onlyContainer && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                                            && c.InwardFacilityChargeID.HasValue && c.InwardFacilityCharge.NotAvailable == false
                                                                            && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posProdMaterialId)
                                                                               || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posMaterialId))
                                                                            && (   (!onlyContainer
                                                                                            && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posProdMaterialId)
                                                                                                || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posMaterialId)))
                                                                                || (onlyContainer 
                                                                                            && (   (posProdMaterialId.HasValue && c.InwardFacilityCharge.Facility.MaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID == posProdMaterialId)
                                                                                                || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.Facility.MaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID == posMaterialId)))
                                                                                )
                                                                            && ((checkOutwardEnabled && c.InwardFacility.OutwardEnabled)
                                                                                || !checkOutwardEnabled)
                                                                           && ((c.InwardFacilityCharge.Facility.MinStockQuantity.HasValue && c.InwardFacilityCharge.Facility.MinStockQuantity.Value < -0.1)
                                                                                || (c.InwardFacilityCharge.FillingDate.HasValue && c.InwardFacilityCharge.FillingDate <= filterTimeOlderThan)))
                                                                    .OrderBy(c => c.InwardFacilityCharge.FillingDate)
                                                                    .Select(c => c.InwardFacilityCharge)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// AND must pe produced form the same order in a previous stage
        /// AND is older than the filter-Date
        /// </summary>
        virtual public QrySilosResult SilosFromPrevStageTime(DatabaseApp ctx,
                                                            ProdOrderPartslistPosRelation relation,
                                                            DateTime filterTimeOlderThan,
                                                            bool checkOutwardEnabled = true,
                                                            bool searchForAlternativeMaterials = false,
                                                            ACValueList projSpecificParams = null,
                                                            bool onlyContainer = true)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosFromPrevStageTime(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, pos.SourceProdOrderPartslistID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosFromPrevStageTime(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, altPos.SourceProdOrderPartslistID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosFromPrevStageTime", msg);

                return new QrySilosResult();
            }
        }


        /// <summary>
        /// Queries Silos which 
        /// contains this intermediate Material
        /// AND must pe produced form the same order in a previous stage
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>> s_cQry_PoSilosFromPrevIntermediateTime =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, posProdOrderPartslistPosId, filterTimeOlderThan, checkOutwardEnabled, onlyContainer) => ctx.FacilityBookingCharge
                                                                    .Include("InwardFacility.FacilityStock_Facility")
                                                                    .Include("InwardFacilityCharge.MDReleaseState")
                                                                    .Include("InwardFacilityCharge.FacilityLot.MDReleaseState")
                                                                    .Include("InwardFacility.MDFacilityType")
                                                                    .Where(c => c.ProdOrderPartslistPosID.HasValue && c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID.HasValue && c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID == posProdOrderPartslistPosId
                                                                            && c.InwardFacilityID.HasValue
                                                                            && (   (onlyContainer && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                                                || (!onlyContainer && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                                          && c.InwardFacilityChargeID.HasValue && c.InwardFacilityCharge.NotAvailable == false
                                                                          && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posProdMaterialId)
                                                                               || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posMaterialId))
                                                                           && ((checkOutwardEnabled && c.InwardFacility.OutwardEnabled)
                                                                              || !checkOutwardEnabled)
                                                                           && ((c.InwardFacilityCharge.Facility.MinStockQuantity.HasValue && c.InwardFacilityCharge.Facility.MinStockQuantity.Value < -0.1)
                                                                                || (c.InwardFacilityCharge.FillingDate.HasValue && c.InwardFacilityCharge.FillingDate <= filterTimeOlderThan)))
                                                                    .OrderBy(c => c.InwardFacilityCharge.FillingDate)
                                                                    .Select(c => c.InwardFacilityCharge)
        );
        /// <summary>
        /// Queries Silos which 
        /// contains this intermediate Material
        /// AND must pe produced form the same order in a previous stage
        /// AND is older than the filter-Date
        /// </summary>
        virtual public QrySilosResult SilosFromPrevIntermediateTime(DatabaseApp ctx,
                                                                ProdOrderPartslistPosRelation relation,
                                                                DateTime filterTimeOlderThan,
                                                                bool checkOutwardEnabled = true,
                                                                bool searchForAlternativeMaterials = false,
                                                                ACValueList projSpecificParams = null,
                                                                bool onlyContainer = true)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosFromPrevIntermediateTime(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, pos.ProdOrderPartslistPosID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosFromPrevIntermediateTime(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, altPos.ProdOrderPartslistPosID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosFromPrevIntermediateTime", msg);

                return new QrySilosResult();
            }
        }
        #endregion

        #region Search includes all lots from previous stage
        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>> s_cQry_SilosFromLotsOfPrevStageTime =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, Guid?, DateTime, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, posSourceProdOrderPartslistID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer) =>
            ctx
            .FacilityBookingCharge
            .Include("InwardFacility.FacilityStock_Facility")
            .Include("InwardFacilityCharge.MDReleaseState")
            .Include("InwardFacilityCharge.FacilityLot.MDReleaseState")
            .Include("InwardFacility.MDFacilityType")
            .Where(c =>
                        c.ProdOrderPartslistPosID.HasValue
                        && c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID.HasValue
                        && c.ProdOrderPartslistPos.ProdOrderPartslistID == posSourceProdOrderPartslistID
                  )
            //.Select(c => c.InwardFacilityLot)
            //.SelectMany(c => c.FacilityBookingCharge_InwardFacilityLot)
            .Where(c =>
                        c.InwardFacilityID.HasValue
                        && (
                            (onlyContainer && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                            ||
                            (!onlyContainer && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin)
                            )
                        && c.InwardFacilityChargeID.HasValue && c.InwardFacilityCharge.NotAvailable == false

                        && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posProdMaterialId)
                            || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posMaterialId))
                        && ((!onlyContainer
                            && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posProdMaterialId)
                                || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posMaterialId)))
                        || (onlyContainer
                            && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.Facility.MaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID == posProdMaterialId)
                                || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.Facility.MaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID == posMaterialId))))

                        && c.InwardFacilityCharge.FillingDate.HasValue
                        && ((checkOutwardEnabled && c.InwardFacility.OutwardEnabled)
                            || !checkOutwardEnabled)
                        && ((c.InwardFacilityCharge.Facility.MinStockQuantity.HasValue && c.InwardFacilityCharge.Facility.MinStockQuantity.Value < -0.1)
                            || (c.InwardFacilityCharge.FillingDate.HasValue && c.InwardFacilityCharge.FillingDate <= filterTimeOlderThan))
                    )
            .OrderBy(c => c.InwardFacilityCharge.FillingDate)
            .Select(c => c.InwardFacilityCharge)
        );

        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// AND which also could be produced from another order 
        /// </summary>
        virtual public QrySilosResult SilosFromLotsOfPrevStageTime(DatabaseApp ctx,
                                                        ProdOrderPartslistPosRelation relation,
                                                        DateTime filterTimeOlderThan,
                                                        bool checkOutwardEnabled = true,
                                                        bool searchForAlternativeMaterials = false,
                                                        ACValueList projSpecificParams = null,
                                                        bool onlyContainer = true)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_SilosFromLotsOfPrevStageTime(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, pos.SourceProdOrderPartslistID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_SilosFromLotsOfPrevStageTime(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, altPos.SourceProdOrderPartslistID, filterTimeOlderThan, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException(nameof(ACPartslistManager), nameof(SilosFromLotsOfPrevStage), msg);

                return new QrySilosResult();
            }
        }

        #endregion

        #endregion


        #region Queries without time filter

        #region Search ignoring previous production stage

        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>> s_cQry_PoSilosWithMaterial =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, checkOutwardEnabled, onlyContainer) => ctx.FacilityCharge
                                                .Include("Facility.FacilityStock_Facility")
                                                .Include("MDReleaseState")
                                                .Include("FacilityLot.MDReleaseState")
                                                .Include("Facility.MDFacilityType")
                                                .Where(c => c.NotAvailable == false
                                                        && (   (onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                            || (!onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.Facility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                        && (    (!onlyContainer
                                                                && ((posProdMaterialId.HasValue && (c.MaterialID == posProdMaterialId || (c.Material.ProductionMaterialID.HasValue && c.Material.ProductionMaterialID == posProdMaterialId)))
                                                                    || (!posProdMaterialId.HasValue && c.MaterialID == posMaterialId)))
                                                            || (onlyContainer && c.Facility.MaterialID.HasValue
                                                                && (   (posProdMaterialId.HasValue && c.Facility.MaterialID == posProdMaterialId)
                                                                    || (!posProdMaterialId.HasValue && c.Facility.MaterialID == posMaterialId)))
                                                            )
                                                      && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                          || !checkOutwardEnabled)
                                                      && c.FillingDate.HasValue)
                                               .OrderBy(c => c.FillingDate)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// </summary>
        virtual public QrySilosResult SilosWithMaterial(DatabaseApp ctx,
                                                        ProdOrderPartslistPosRelation relation,
                                                        bool checkOutwardEnabled = true,
                                                        bool searchForAlternativeMaterials = false,
                                                        ACValueList projSpecificParams = null,
                                                        bool onlyContainer = true)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosWithMaterial(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosWithMaterial(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithMaterial(10)", msg);

                return new QrySilosResult();
            }
        }


        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>> s_cQry_PoSilosWithIntermediateMaterial =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, posProdOrderPartsListID, checkOutwardEnabled, onlyContainer) => ctx.FacilityCharge
                                                .Include("Facility.FacilityStock_Facility")
                                                .Include("MDReleaseState")
                                                .Include("FacilityLot.MDReleaseState")
                                                .Include("Facility.MDFacilityType")
                                                .Where(c => c.NotAvailable == false
                                                        && (   (onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                            || (!onlyContainer && c.Facility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.Facility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                        && (    (!onlyContainer
                                                                && ((posProdMaterialId.HasValue && (c.MaterialID == posProdMaterialId || (c.Material.ProductionMaterialID.HasValue && c.Material.ProductionMaterialID == posProdMaterialId)))
                                                                    || (!posProdMaterialId.HasValue && c.MaterialID == posMaterialId)))
                                                            || (onlyContainer && c.Facility.MaterialID.HasValue
                                                                && (   (posProdMaterialId.HasValue && c.Facility.MaterialID == posProdMaterialId)
                                                                    || (!posProdMaterialId.HasValue && c.Facility.MaterialID == posMaterialId)))
                                                            )
                                                      && (     !onlyContainer
                                                            || (    c.Facility.PartslistID.HasValue
                                                                 && posProdOrderPartsListID.HasValue
                                                                 && c.Facility.PartslistID == posProdOrderPartsListID))
                                                      && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                         || !checkOutwardEnabled)
                                                      && c.FillingDate.HasValue)
                                               .OrderBy(c => c.FillingDate)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// AND is older than the filter-Date
        /// </summary>
        virtual public QrySilosResult SilosWithIntermediateMaterial(DatabaseApp ctx,
            ProdOrderPartslistPosRelation relation,
            bool checkOutwardEnabled = true,
            bool searchForAlternativeMaterials = false,
            ACValueList projSpecificParams = null,
            bool onlyContainer = true)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    var result = s_cQry_PoSilosWithIntermediateMaterial(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, pos.ProdOrderPartslist.PartslistID, checkOutwardEnabled, onlyContainer).ToArray();
                    return new QrySilosResult(result);
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosWithIntermediateMaterial(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, altPos.ProdOrderPartslist.PartslistID, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithIntermediateMaterial(10)", msg);

                return new QrySilosResult();
            }
        }

        #endregion

        #region Search includes previous production stage
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// AND which also could be produced from another order 
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>> s_cQry_PoSilosFromPrevStage =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, posSourceProdOrderPartslistID, checkOutwardEnabled, onlyContainer) => ctx.FacilityBookingCharge
                                                .Include("InwardFacility.FacilityStock_Facility")
                                                .Include("InwardFacilityCharge.MDReleaseState")
                                                .Include("InwardFacilityCharge.FacilityLot.MDReleaseState")
                                                .Include("InwardFacility.MDFacilityType")
                                                .Where(c => c.ProdOrderPartslistPosID.HasValue && c.ProdOrderPartslistPos.ProdOrderPartslistID == posSourceProdOrderPartslistID
                                                    && c.InwardFacilityID.HasValue
                                                    && (   (onlyContainer && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                        || (!onlyContainer && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                     && c.InwardFacilityChargeID.HasValue && c.InwardFacilityCharge.NotAvailable == false
                                                     && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posProdMaterialId)
                                                          || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posMaterialId))
                                                     && ( (!onlyContainer
                                                            && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posProdMaterialId)
                                                                || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posMaterialId)))
                                                       || (onlyContainer
                                                            && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.Facility.MaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID == posProdMaterialId)
                                                                || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.Facility.MaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID == posMaterialId))))
                                                     && c.InwardFacilityCharge.FillingDate.HasValue
                                                     && ((checkOutwardEnabled && c.InwardFacility.OutwardEnabled)
                                                          || !checkOutwardEnabled))
                                               .OrderBy(c => c.InwardFacilityCharge.FillingDate)
                                               .Select(c => c.InwardFacilityCharge)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// AND which also could be produced from another order 
        /// </summary>
        virtual public QrySilosResult SilosFromPrevStage(DatabaseApp ctx, 
                                                        ProdOrderPartslistPosRelation relation, 
                                                        bool checkOutwardEnabled = true, 
                                                        bool searchForAlternativeMaterials = false, 
                                                        ACValueList projSpecificParams = null, 
                                                        bool onlyContainer = true)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosFromPrevStage(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, pos.SourceProdOrderPartslistID, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosFromPrevStage(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, altPos.SourceProdOrderPartslistID, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosFromPrevStage", msg);

                return new QrySilosResult();
            }
        }


        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>> s_cQry_PoSilosFromPrevIntermediate =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, posProdOrderPartslistPosID, checkOutwardEnabled, onlyContainer) => ctx.FacilityBookingCharge
                                                .Include("InwardFacility.FacilityStock_Facility")
                                                .Include("InwardFacilityCharge.MDReleaseState")
                                                .Include("InwardFacilityCharge.FacilityLot.MDReleaseState")
                                                .Include("InwardFacility.MDFacilityType")
                                                .Where(c => c.ProdOrderPartslistPosID.HasValue && c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID.HasValue && c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID == posProdOrderPartslistPosID
                                                    && c.InwardFacilityID.HasValue
                                                    && (   (onlyContainer && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                                                        || (!onlyContainer && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin))
                                                     && c.InwardFacilityChargeID.HasValue && c.InwardFacilityCharge.NotAvailable == false
                                                     && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posProdMaterialId)
                                                          || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posMaterialId))
                                                     && c.InwardFacilityCharge.FillingDate.HasValue
                                                     && ((checkOutwardEnabled && c.InwardFacility.OutwardEnabled)
                                                          || !checkOutwardEnabled))
                                               .OrderBy(c => c.InwardFacilityCharge.FillingDate)
                                               .Select(c => c.InwardFacilityCharge)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// </summary>
        virtual public QrySilosResult SilosFromPrevIntermediate(DatabaseApp ctx,
                                                                ProdOrderPartslistPosRelation relation,
                                                                bool checkOutwardEnabled = true,
                                                                bool searchForAlternativeMaterials = false,
                                                                ACValueList projSpecificParams = null,
                                                                bool onlyContainer = true)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosFromPrevIntermediate(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, pos.ProdOrderPartslistPosID, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosFromPrevIntermediate(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, altPos.ProdOrderPartslistPosID, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosFromPrevIntermediate", msg);

                return new QrySilosResult();
            }
        }
        #endregion

        #region Search includes all lots from previous stage
        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// </summary>
        protected static readonly Func<DatabaseApp, Guid?, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>> s_cQry_SilosFromLotsOfPrevStage =
        EF.CompileQuery<DatabaseApp, Guid?, Guid?, Guid?, bool, bool, IEnumerable<FacilityCharge>>(
            (ctx, posMaterialId, posProdMaterialId, posSourceProdOrderPartslistID, checkOutwardEnabled, onlyContainer) =>
            ctx
            .FacilityBookingCharge
            .Include("InwardFacility.FacilityStock_Facility")
            .Include("InwardFacilityCharge.MDReleaseState")
            .Include("InwardFacilityCharge.FacilityLot.MDReleaseState")
            .Include("InwardFacility.MDFacilityType")
            .Where(c =>
                        c.ProdOrderPartslistPosID.HasValue
                        && c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID.HasValue
                        && c.ProdOrderPartslistPos.ProdOrderPartslistID == posSourceProdOrderPartslistID
                  )
            //.Select(c => c.InwardFacilityLot)
            //.SelectMany(c => c.FacilityBookingCharge_InwardFacilityLot)
            .Where(c =>
                        c.InwardFacilityID.HasValue
                        && (
                            (onlyContainer && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
                            ||
                            (!onlyContainer && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex >= (short)FacilityTypesEnum.StorageBin && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex <= (short)FacilityTypesEnum.PreparationBin)
                            )
                        && c.InwardFacilityChargeID.HasValue && c.InwardFacilityCharge.NotAvailable == false

                        && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posProdMaterialId)
                            || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posMaterialId))
                        && ((!onlyContainer
                            && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posProdMaterialId)
                                || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.MaterialID == posMaterialId)))
                        || (onlyContainer
                            && ((posProdMaterialId.HasValue && c.InwardFacilityCharge.Facility.MaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID == posProdMaterialId)
                                || (!posProdMaterialId.HasValue && c.InwardFacilityCharge.Facility.MaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID == posMaterialId))))

                        && c.InwardFacilityCharge.FillingDate.HasValue
                        && (
                            (checkOutwardEnabled && c.InwardFacility.OutwardEnabled)
                            || !checkOutwardEnabled
                            )
                    )
            .OrderBy(c => c.InwardFacilityCharge.FillingDate)
            .Select(c => c.InwardFacilityCharge)
        );

        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// AND which also could be produced from another order 
        /// </summary>
        virtual public QrySilosResult SilosFromLotsOfPrevStage(DatabaseApp ctx,
                                                        ProdOrderPartslistPosRelation relation,
                                                        bool checkOutwardEnabled = true,
                                                        bool searchForAlternativeMaterials = false,
                                                        ACValueList projSpecificParams = null,
                                                        bool onlyContainer = true)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    //#if DEBUG
                    //                    if (System.Diagnostics.Debugger.IsAttached)
                    //                    {
                    //                        string strQuery = ((System.Data.Objects.ObjectQuery)s_cQry_SilosFromLotsOfPrevStage(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, pos.SourceProdOrderPartslistID, checkOutwardEnabled, onlyContainer)).ToTraceString();
                    //                        this.Messages.LogDebug(this.GetACUrl(), "Query", strQuery);
                    //                    }
                    //#endif
                    return new QrySilosResult(s_cQry_SilosFromLotsOfPrevStage(ctx, pos.MaterialID, pos.Material.ProductionMaterialID != null ? pos.Material.ProductionMaterialID : pos.Material.MaterialID, pos.SourceProdOrderPartslistID, checkOutwardEnabled, onlyContainer).ToArray());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_SilosFromLotsOfPrevStage(ctx, altPos.MaterialID, altPos.Material.ProductionMaterialID != null ? altPos.Material.ProductionMaterialID : altPos.Material.MaterialID, altPos.SourceProdOrderPartslistID, checkOutwardEnabled, onlyContainer).ToArray();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException(nameof(ACPartslistManager), nameof(SilosFromLotsOfPrevStage), msg);

                return new QrySilosResult();
            }
        }

        #endregion


        #endregion

        #endregion

        #endregion

        #region Methods
        public virtual QrySilosResult FindSilos(IPartslistPosRelation relation,
                                                DatabaseApp dbApp, Database dbIPlus,
                                                SearchMode searchMode,
                                                DateTime? filterTimeOlderThan,
                                                Guid? ignoreFacilityID,
                                                out QrySilosResult allSilos,
                                                IEnumerable<gip.core.datamodel.ACClass> exclusionList = null,
                                                ACValueList projSpecificParams = null,
                                                bool onlyContainer = true,
                                                short reservationMode = 0,
                                                IEnumerable<gip.core.datamodel.ACClass> routableSilos = null)
        {
            PartslistPosRelation plRelation = null;
            ProdOrderPartslistPosRelation poRelation = relation as ProdOrderPartslistPosRelation;
            if (poRelation == null)
                plRelation = relation as PartslistPosRelation;

            // 1. Suche freie Silos, mit dem zu dosierenden Material + die Freigegeben sind + die keine gesperrte Chargen haben
            // soriert nach der ältesten eingelagerten Charge
            QrySilosResult facilityQuery = null;
            allSilos = null;

            //using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted}))
            //{
            if (poRelation != null)
            {
                if (filterTimeOlderThan.HasValue)
                {
                    if (poRelation.SourceProdOrderPartslistPos.SourceProdOrderPartslistID.HasValue)
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosFromPrevIntermediateTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                        }
                        else
                        {
                            if (FindSiloModes <= 0)
                                facilityQuery = SilosFromLotsOfPrevStageTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                            else
                                facilityQuery = SilosFromPrevStageTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                        }
                    }
                    else
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterialTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterialTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                        }
                    }
                }
                else
                {
                    if (poRelation.SourceProdOrderPartslistPos.SourceProdOrderPartslistID.HasValue)
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosFromPrevIntermediate(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                        }
                        else
                        {
                            if (FindSiloModes <= 0)
                                facilityQuery = SilosFromLotsOfPrevStage(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                            else
                                facilityQuery = SilosFromPrevStage(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                        }
                    }
                    else
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterial(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterial(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                        }
                    }
                }
                ApplyLotReservationFilter(facilityQuery, poRelation, reservationMode);
            }
            else
            {
                if (filterTimeOlderThan.HasValue)
                {
                    if (plRelation.SourcePartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                    {
                        facilityQuery = SilosWithIntermediateMaterialTime(dbApp, plRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                    }
                    else
                    {
                        facilityQuery = SilosWithMaterialTime(dbApp, plRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                    }
                }
                else
                {
                    if (plRelation.SourcePartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                    {
                        facilityQuery = SilosWithIntermediateMaterial(dbApp, plRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                    }
                    else
                    {
                        facilityQuery = SilosWithMaterial(dbApp, plRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                    }
                }
            }

            if (onlyContainer)
                if (searchMode != SearchMode.AllSilos)
                    facilityQuery.ApplyBlockedQuantsFilter();
            else
                facilityQuery.ApplyFreeQuantsInBinFilter();

            facilityQuery.RemoveFacility(ignoreFacilityID, exclusionList);

            allSilos = facilityQuery.Clone() as QrySilosResult;

            if (routableSilos != null && routableSilos.Any())
                facilityQuery.ApplyRoutableSilos(routableSilos);

            if (facilityQuery.FilteredResult != null && facilityQuery.FilteredResult.Any())
                return facilityQuery;

            // 2. Suche nach Material das von einem anderen Auftrag produziert worden ist, wenn es keine Silos gibt
            if (poRelation != null)
            {
                // Prüfe ob Entnahme von anderem Auftrag erlaubt
                if (poRelation.SourceProdOrderPartslistPos.SourceProdOrderPartslistID.HasValue)
                {
                    if (filterTimeOlderThan.HasValue)
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterialTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterialTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                        }
                    }
                    else
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterial(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterial(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams, onlyContainer);
                        }
                    }
                    ApplyLotReservationFilter(facilityQuery, poRelation, reservationMode);

                    if (onlyContainer)
                        facilityQuery.ApplyBlockedQuantsFilter();
                    else
                        facilityQuery.ApplyFreeQuantsInBinFilter();

                    facilityQuery.RemoveFacility(ignoreFacilityID, exclusionList);

                    allSilos = facilityQuery.Clone() as QrySilosResult;

                    if (routableSilos != null && routableSilos.Any())
                        facilityQuery.ApplyRoutableSilos(routableSilos);

                    if (!poRelation.SourceProdOrderPartslistPos.TakeMatFromOtherOrder)
                    {
                        List<Guid> lots = poRelation.SourceProdOrderPartslistPos
                                                           .SourceProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                                                           .Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern && c.FacilityLotID.HasValue)
                                                           .Select(c => c.FacilityLotID.Value).Distinct().ToList();

                        facilityQuery.FilteredResult = facilityQuery.FilteredResult.Where(c => c.FacilityCharges.Any(x => x.Quant.FacilityLotID.HasValue && lots.Contains(x.Quant.FacilityLotID.Value))).ToList();
                    }

                    if ((facilityQuery.FilteredResult == null || !facilityQuery.FilteredResult.Any()) && !poRelation.SourceProdOrderPartslistPos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                        return facilityQuery;
                }
                // Falls kein Alternativmaterial gepflegt, gebe leeres resultat zurück
                else if (!poRelation.SourceProdOrderPartslistPos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                    return null;
            }
            // Falls kein Alternativmaterial gepflegt, gebe leeres resultat zurück
            else if (!plRelation.SourcePartslistPos.PartslistPos_AlternativePartslistPos.Any())
                return null;


            // 3. Suche nach alternativem Material
            if (poRelation != null)
            {
                if (poRelation.SourceProdOrderPartslistPos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    if (filterTimeOlderThan.HasValue)
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterialTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, true, projSpecificParams, onlyContainer);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterialTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, true, projSpecificParams, onlyContainer);
                        }
                    }
                    else
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterial(dbApp, poRelation, searchMode != SearchMode.AllSilos, true, projSpecificParams, onlyContainer);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterial(dbApp, poRelation, searchMode != SearchMode.AllSilos, true, projSpecificParams, onlyContainer);
                        }
                    }
                    ApplyLotReservationFilter(facilityQuery, poRelation, reservationMode);

                    if (onlyContainer)
                        facilityQuery.ApplyBlockedQuantsFilter();
                    else
                        facilityQuery.ApplyFreeQuantsInBinFilter();

                    facilityQuery.RemoveFacility(ignoreFacilityID, exclusionList);

                    allSilos = facilityQuery.Clone() as QrySilosResult;

                    return facilityQuery;
                }
            }
            else
            {
                if (plRelation.SourcePartslistPos.PartslistPos_AlternativePartslistPos.Any())
                {
                    if (filterTimeOlderThan.HasValue)
                    {
                        if (plRelation.SourcePartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterialTime(dbApp, plRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, true, projSpecificParams, onlyContainer);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterialTime(dbApp, plRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, true, projSpecificParams, onlyContainer);
                        }
                    }
                    else
                    {
                        if (plRelation.SourcePartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterial(dbApp, plRelation, searchMode != SearchMode.AllSilos, true, projSpecificParams, onlyContainer);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterial(dbApp, plRelation, searchMode != SearchMode.AllSilos, true, projSpecificParams, onlyContainer);
                        }
                    }
                    ApplyLotReservationFilter(facilityQuery, poRelation, reservationMode);

                    if (onlyContainer)
                        facilityQuery.ApplyBlockedQuantsFilter();
                    else
                        facilityQuery.ApplyFreeQuantsInBinFilter();

                    facilityQuery.RemoveFacility(ignoreFacilityID, exclusionList);

                    allSilos = facilityQuery.Clone() as QrySilosResult;

                    return facilityQuery;
                }
            }
            //}
            return facilityQuery == null ? new QrySilosResult() : facilityQuery;
        }


        protected virtual void ApplyLotReservationFilter(QrySilosResult qrySilosResult, ProdOrderPartslistPosRelation poRelation, short reservationMode)
        {
            if (qrySilosResult == null)
                return;
            qrySilosResult.ApplyLotReservationFilter(poRelation, reservationMode);
        }

        public virtual IEnumerable<Route> GetRoutes(IPartslistPosRelation relation,
                                                DatabaseApp dbApp, Database dbIPlus,
                                                gip.core.datamodel.ACClass scaleACClass,
                                                SearchMode searchMode,
                                                DateTime? filterTimeOlderThan,
                                                out QrySilosResult possibleSilos,
                                                out QrySilosResult allSilos,
                                                Guid? ignoreFacilityID,
                                                IEnumerable<gip.core.datamodel.ACClass> exclusionList = null,
                                                ACValueList projSpecificParams = null,
                                                bool onlyContainer = true,
                                                short reservationMode = 0,
                                                string selectionRuleID = "PAMSilo.Deselector",
                                                bool includeReserved = true, bool includeAllocated = true)
        {
            if (scaleACClass == null)
            {
                throw new NullReferenceException("AccessedProcessModule is null");
            }

            ACRoutingParameters routingParamRoutableSilos = new ACRoutingParameters()
            {
                RoutingService = this.RoutingService,
                Database = dbIPlus,
                Direction = RouteDirections.Backwards,
                SelectionRuleID = "Storage",
                DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != scaleACClass.ACClassID,
                MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                IncludeReserved = includeReserved,
                IncludeAllocated = includeAllocated,
                DBRecursionLimit = 10,
                ResultMode = RouteResultMode.ShortRoute
            };

            RoutingResult routableSilos = ACRoutingService.FindSuccessors(scaleACClass, routingParamRoutableSilos);
            IEnumerable<core.datamodel.ACClass> routableSilosACClass = null;
            if (routableSilos != null && routableSilos.Routes != null && routableSilos.Routes.Any())
            {
                var tempList = new List<core.datamodel.ACClass>();
                foreach(Route silosRoute in routableSilos.Routes)
                {
                    RouteItem sourceRouteItem = silosRoute.GetRouteSource();
                    if(sourceRouteItem != null && sourceRouteItem.Source != null)
                    {
                        tempList.Add(sourceRouteItem.Source);
                    }
                }
                routableSilosACClass = tempList;
            }

            possibleSilos = FindSilos(relation, dbApp, dbIPlus, searchMode, filterTimeOlderThan, ignoreFacilityID, out allSilos, exclusionList, projSpecificParams, onlyContainer, reservationMode, routableSilosACClass);
            if (possibleSilos == null || possibleSilos.FilteredResult == null || !possibleSilos.FilteredResult.Any())
            {
                possibleSilos = allSilos;
                return null;
            }

            ACRoutingParameters routingParameters = new ACRoutingParameters()
            {
                RoutingService = this.RoutingService,
                Database = dbIPlus,
                Direction = RouteDirections.Backwards,
                SelectionRuleID = selectionRuleID,
                DBDeSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != scaleACClass.ACClassID,
                MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                IncludeReserved = includeReserved,
                IncludeAllocated = includeAllocated,
                DBRecursionLimit = 10
            };

            RoutingResult result = null;
            if (searchMode == SearchMode.OnlyEnabledOldestSilo)
            {
                QrySilosResult.FacilitySumByLots oldestSilo = possibleSilos.FilteredResult.FirstOrDefault();
                if (oldestSilo == null)
                {
                    possibleSilos = allSilos;
                    return null;
                }
                if (!oldestSilo.StorageBin.VBiFacilityACClassID.HasValue)
                {
                    possibleSilos = allSilos;
                    return null;
                }
                var oldestSiloClass = oldestSilo.StorageBin.GetFacilityACClass(dbIPlus);
                routingParameters.DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && oldestSilo.StorageBin.VBiFacilityACClassID == c.ACClassID;

                result = ACRoutingService.SelectRoutes(scaleACClass, oldestSiloClass, routingParameters);
                if (result.Routes == null || !result.Routes.Any())
                {
                    possibleSilos = allSilos;
                    return null;
                }
            }
            else
            {
                // 2. Suche Routen zu dieser Waage die von den vorgeschlagenen Silos aus führen
                var acClassIDsOfPossibleSilos = possibleSilos.FilteredResult.Where(c => c.StorageBin.VBiFacilityACClassID.HasValue).Select(c => c.StorageBin.VBiFacilityACClassID.Value);
                IEnumerable<string> possibleSilosACUrl = possibleSilos.FilteredResult.Where(c => c.StorageBin.FacilityACClass != null).Select(x => x.StorageBin.FacilityACClass.GetACUrlComponent());

                routingParameters.DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && acClassIDsOfPossibleSilos.Contains(c.ACClassID);

                result = ACRoutingService.SelectRoutes(scaleACClass, possibleSilosACUrl, routingParameters);

                if (result.Routes == null || !result.Routes.Any())
                {
                    possibleSilos = allSilos;
                    return null;
                }
            }

            possibleSilos = allSilos;
            return result.Routes;
        }
        #endregion
    }
}
