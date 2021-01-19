using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;

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
        public class QrySilosResult
        {
            public QrySilosResult(IList<Facility> foundSilos, IPartslistPos doseAlternativeMat = null)
            {
                _FoundSilos = foundSilos;
                _DoseAlternativeMat = doseAlternativeMat;
            }

            private IList<Facility> _FoundSilos;
            public IList<Facility> FoundSilos
            {
                get
                {
                    return _FoundSilos;
                }
            }

            public IPartslistPos _DoseAlternativeMat;
            public IPartslistPos DoseAlternativeMat
            {
                get
                {
                    return _DoseAlternativeMat;
                }
            }
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
        protected static readonly Func<DatabaseApp, PartslistPos, DateTime, bool, IQueryable<Facility>> s_cQry_PlSilosWithMaterialTime =
        CompiledQuery.Compile<DatabaseApp, PartslistPos, DateTime, bool, IQueryable<Facility>>(
            (ctx, pos, filterTimeOlderThan, checkOutwardEnabled) => ctx.FacilityCharge
                                                                    .Include("Facility.FacilityStock_Facility")
                                                                    .Where(c => c.NotAvailable == false
                                                                           && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer
                                                                           && c.Facility.MaterialID.HasValue
                                                                           && ((pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.Material.ProductionMaterialID)
                                                                               || (!pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.MaterialID))
                                                                           && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                                              || !checkOutwardEnabled)
                                                                           && c.FillingDate.HasValue && c.FillingDate <= filterTimeOlderThan)
                                                                    .OrderBy(c => c.FillingDate)
                                                                    .Select(c => c.Facility)
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
                                                            ACValueList projSpecificParams = null)
        {
            try
            {
                PartslistPos pos = relation.SourcePartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PlSilosWithMaterialTime(ctx, pos, filterTimeOlderThan, checkOutwardEnabled).ToArray().Distinct().ToList());
                    // Additional filter, which checks if Silo contains quants above which is oldern than filterDate
                    //foreach (Facility facility in facilityList.ToArray())
                    //{
                    //    FacilityCharge fc = CoffeeSilo.s_cQry_PlQuants(ctx, facility).FirstOrDefault();
                    //    if (fc == null || fc.FillingDate > filterTimeOlderThan)
                    //        facilityList.Remove(facility);
                    //}
                }
                else if (pos.PartslistPos_AlternativePartslistPos.Any())
                {
                    foreach (PartslistPos altPos in pos.PartslistPos_AlternativePartslistPos)
                    {
                        var result = s_cQry_PlSilosWithMaterialTime(ctx, altPos, filterTimeOlderThan, checkOutwardEnabled).ToArray().Distinct().ToList();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult(new List<Facility>());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithMaterialTime", msg);

                return new QrySilosResult(new List<Facility>());
            }
        }

        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, PartslistPos, DateTime, bool, IQueryable<Facility>> s_cQry_PlSilosWithIntermediateMaterialTime =
        CompiledQuery.Compile<DatabaseApp, PartslistPos, DateTime, bool, IQueryable<Facility>>(
            (ctx, pos, filterTimeOlderThan, checkOutwardEnabled) => ctx.FacilityCharge
                                                                    .Include("Facility.FacilityStock_Facility")
                                                                    .Where(c => c.NotAvailable == false
                                                                           && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer
                                                                           && c.Facility.MaterialID.HasValue
                                                                           && ((pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.Material.ProductionMaterialID)
                                                                               || (!pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.MaterialID))
                                                                           && (c.Facility.PartslistID.HasValue
                                                                              && c.Facility.PartslistID == pos.PartslistID)
                                                                           && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                                              || !checkOutwardEnabled)
                                                                           && c.FillingDate.HasValue && c.FillingDate <= filterTimeOlderThan)
                                                                    .OrderBy(c => c.FillingDate)
                                                                    .Select(c => c.Facility)
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
                                                                       ACValueList projSpecificParams = null)
        {
            try
            {
                PartslistPos pos = relation.SourcePartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PlSilosWithIntermediateMaterialTime(ctx, pos, filterTimeOlderThan, checkOutwardEnabled).ToArray().Distinct().ToList());
                    // Additional filter, which checks if Silo contains quants above which is oldern than filterDate
                    //foreach (Facility facility in facilityList.ToArray())
                    //{
                    //    FacilityCharge fc = CoffeeSilo.s_cQry_PlQuants(ctx, facility).FirstOrDefault();
                    //    if (fc == null || fc.FillingDate > degassingTime)
                    //        facilityList.Remove(facility);
                    //}
                }
                else if (pos.PartslistPos_AlternativePartslistPos.Any())
                {
                    foreach (PartslistPos altPos in pos.PartslistPos_AlternativePartslistPos)
                    {
                        var result = s_cQry_PlSilosWithIntermediateMaterialTime(ctx, altPos, filterTimeOlderThan, checkOutwardEnabled).ToArray().Distinct().ToList();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult(new List<Facility>());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "StartWithIntermediateMaterialTime", msg);

                return new QrySilosResult(new List<Facility>());
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
        protected static readonly Func<DatabaseApp, PartslistPos, bool, IQueryable<Facility>> s_cQry_PlSilosWithMaterial =
        CompiledQuery.Compile<DatabaseApp, PartslistPos, bool, IQueryable<Facility>>(
            (ctx, pos, checkOutwardEnabled) => ctx.FacilityCharge
                                                .Include("Facility.FacilityStock_Facility")
                                                .Where(c => c.NotAvailable == false
                                                      && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer
                                                      && c.Facility.MaterialID.HasValue
                                                      && ((pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.Material.ProductionMaterialID)
                                                          || (!pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.MaterialID))
                                                      && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                          || !checkOutwardEnabled)
                                                      && c.FillingDate.HasValue)
                                               .OrderBy(c => c.FillingDate)
                                               .Select(c => c.Facility)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// </summary>
        virtual public QrySilosResult SilosWithMaterial(DatabaseApp ctx,
                                                        PartslistPosRelation relation,
                                                        bool checkOutwardEnabled = true,
                                                        bool searchForAlternativeMaterials = false,
                                                        ACValueList projSpecificParams = null)
        {
            try
            {
                PartslistPos pos = relation.SourcePartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PlSilosWithMaterial(ctx, pos, checkOutwardEnabled).ToArray().Distinct().ToList());
                }
                else if (pos.PartslistPos_AlternativePartslistPos.Any())
                {
                    foreach (PartslistPos altPos in pos.PartslistPos_AlternativePartslistPos)
                    {
                        var result = s_cQry_PlSilosWithMaterial(ctx, altPos, checkOutwardEnabled).ToArray().Distinct().ToList();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult(new List<Facility>());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithMaterial", msg);

                return new QrySilosResult(new List<Facility>());
            }
        }


        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, PartslistPos, bool, IQueryable<Facility>> s_cQry_PlSilosWithIntermediateMaterial =
        CompiledQuery.Compile<DatabaseApp, PartslistPos, bool, IQueryable<Facility>>(
            (ctx, pos, checkOutwardEnabled) => ctx.FacilityCharge
                                                .Include("Facility.FacilityStock_Facility")
                                                .Where(c => c.NotAvailable == false
                                                      && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer
                                                      && c.Facility.MaterialID.HasValue
                                                      && ((pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.Material.ProductionMaterialID)
                                                          || (!pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.MaterialID))
                                                      && (c.Facility.PartslistID.HasValue
                                                         && c.Facility.PartslistID == pos.PartslistID)
                                                     && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                         || !checkOutwardEnabled)
                                                      && c.FillingDate.HasValue)
                                               .OrderBy(c => c.FillingDate)
                                               .Select(c => c.Facility)
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
            ACValueList projSpecificParams = null)
        {
            try
            {
                PartslistPos pos = relation.SourcePartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PlSilosWithIntermediateMaterial(ctx, pos, checkOutwardEnabled).ToArray().Distinct().ToList());
                }
                else if (pos.PartslistPos_AlternativePartslistPos.Any())
                {
                    foreach (PartslistPos altPos in pos.PartslistPos_AlternativePartslistPos)
                    {
                        var result = s_cQry_PlSilosWithIntermediateMaterial(ctx, altPos, checkOutwardEnabled).ToArray().Distinct().ToList();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult(new List<Facility>());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithIntermediateMaterial", msg);

                return new QrySilosResult(new List<Facility>());
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
        protected static readonly Func<DatabaseApp, ProdOrderPartslistPos, DateTime, bool, IQueryable<Facility>> s_cQry_PoSilosWithMaterialTime =
        CompiledQuery.Compile<DatabaseApp, ProdOrderPartslistPos, DateTime, bool, IQueryable<Facility>>(
            (ctx, pos, filterTimeOlderThan, checkOutwardEnabled) => ctx.FacilityCharge
                                                                            .Include("Facility.FacilityStock_Facility")
                                                                            .Where(c => c.NotAvailable == false
                                                                                       && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer
                                                                                       && c.Facility.MaterialID.HasValue
                                                                                       && ((pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.Material.ProductionMaterialID)
                                                                                           || (!pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.MaterialID))
                                                                                       && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                                                          || !checkOutwardEnabled)
                                                                                       && c.FillingDate.HasValue && c.FillingDate <= filterTimeOlderThan)
                                                                           .OrderBy(c => c.FillingDate)
                                                                           .Select(c => c.Facility)
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
                                                            ACValueList projSpecificParams = null)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosWithMaterialTime(ctx, pos, filterTimeOlderThan, checkOutwardEnabled).ToArray().Distinct().ToList());
                    // Additional filter, which checks if Silo contains quants above which is oldern than filterDate
                    //foreach (Facility facility in facilityList.ToArray())
                    //{
                    //    FacilityCharge fc = CoffeeSilo.s_cQry_PoQuants(ctx, facility).FirstOrDefault();
                    //    if (fc == null || fc.FillingDate > filterTimeOlderThan)
                    //        facilityList.Remove(facility);
                    //}
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosWithMaterialTime(ctx, altPos, filterTimeOlderThan, checkOutwardEnabled).ToArray().Distinct().ToList();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult(new List<Facility>());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithMaterialTime", msg);

                return new QrySilosResult(new List<Facility>());
            }
        }

        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, ProdOrderPartslistPos, DateTime, bool, IQueryable<Facility>> s_cQry_PoSilosWithIntermediateMaterialTime =
        CompiledQuery.Compile<DatabaseApp, ProdOrderPartslistPos, DateTime, bool, IQueryable<Facility>>(
            (ctx, pos, filterTimeOlderThan, checkOutwardEnabled) => ctx.FacilityCharge
                                                                    .Include("Facility.FacilityStock_Facility")
                                                                    .Where(c => c.NotAvailable == false
                                                                           && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer
                                                                           && c.Facility.MaterialID.HasValue
                                                                           && ((pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.Material.ProductionMaterialID)
                                                                               || (!pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.MaterialID))
                                                                           && (c.Facility.PartslistID.HasValue
                                                                              && pos.ProdOrderPartslist.PartslistID.HasValue
                                                                              && c.Facility.PartslistID == pos.ProdOrderPartslist.PartslistID)
                                                                           && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                                              || !checkOutwardEnabled)
                                                                           && c.FillingDate.HasValue && c.FillingDate <= filterTimeOlderThan)
                                                                    .OrderBy(c => c.FillingDate)
                                                                    .Select(c => c.Facility)
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
                                                                       ACValueList projSpecificParams = null)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosWithIntermediateMaterialTime(ctx, pos, filterTimeOlderThan, checkOutwardEnabled).ToArray().Distinct().ToList());
                    // Additional filter, which checks if Silo contains quants above which is oldern than filterDate
                    //foreach (Facility facility in facilityList.ToArray())
                    //{
                    //    FacilityCharge fc = CoffeeSilo.s_cQry_PoQuants(ctx, facility).FirstOrDefault();
                    //    if (fc == null || fc.FillingDate > degassingTime)
                    //        facilityList.Remove(facility);
                    //}
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosWithIntermediateMaterialTime(ctx, altPos, filterTimeOlderThan, checkOutwardEnabled).ToArray().Distinct().ToList();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult(new List<Facility>());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithIntermediateMaterialTime", msg);

                return new QrySilosResult(new List<Facility>());
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
        protected static readonly Func<DatabaseApp, ProdOrderPartslistPos, DateTime, bool, IQueryable<Facility>> s_cQry_PoSilosFromPrevStageTime =
        CompiledQuery.Compile<DatabaseApp, ProdOrderPartslistPos, DateTime, bool, IQueryable<Facility>>(
            (ctx, pos, filterTimeOlderThan, checkOutwardEnabled) => ctx.FacilityBookingCharge
                                                                    .Include("InwardFacility.FacilityStock_Facility")
                                                                    .Where(c => c.ProdOrderPartslistPosID.HasValue
                                                                            && pos.SourceProdOrderPartslistID.HasValue
                                                                            && pos.MaterialID.HasValue
                                                                            && c.ProdOrderPartslistPos.ProdOrderPartslistID == pos.SourceProdOrderPartslistID
                                                                            && c.InwardFacilityID.HasValue && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer
                                                                            && c.InwardFacilityChargeID.HasValue && c.InwardFacilityCharge.NotAvailable == false
                                                                            && ((pos.Material.ProductionMaterialID.HasValue && c.InwardFacilityCharge.MaterialID == pos.Material.ProductionMaterialID)
                                                                               || (!pos.Material.ProductionMaterialID.HasValue && c.InwardFacilityCharge.MaterialID == pos.MaterialID))
                                                                            && ((pos.Material.ProductionMaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID == pos.Material.ProductionMaterialID)
                                                                                || (!pos.Material.ProductionMaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID == pos.MaterialID))
                                                                            && ((checkOutwardEnabled && c.InwardFacility.OutwardEnabled)
                                                                                || !checkOutwardEnabled)
                                                                            && c.InwardFacilityCharge.FillingDate.HasValue && c.InwardFacilityCharge.FillingDate <= filterTimeOlderThan)
                                                                    .OrderBy(c => c.InwardFacilityCharge.FillingDate)
                                                                    .Select(c => c.InwardFacility)
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
                                                            ACValueList projSpecificParams = null)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosFromPrevStageTime(ctx, pos, filterTimeOlderThan, checkOutwardEnabled).ToArray().Distinct().ToList());
                    // Additional filter, which checks if Silo contains quants above which is oldern than filterDate
                    //foreach (Facility facility in facilityList.ToArray())
                    //{
                    //    FacilityCharge fc = CoffeeSilo.s_cQry_PoQuants(ctx, facility).FirstOrDefault();
                    //    if (fc == null || fc.FillingDate > degassingTime)
                    //        facilityList.Remove(facility);
                    //}
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosFromPrevStageTime(ctx, altPos, filterTimeOlderThan, checkOutwardEnabled).ToArray().Distinct().ToList();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult(new List<Facility>());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosFromPrevStageTime", msg);

                return new QrySilosResult(new List<Facility>());
            }
        }


        /// <summary>
        /// Queries Silos which 
        /// contains this intermediate Material
        /// AND must pe produced form the same order in a previous stage
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, ProdOrderPartslistPos, DateTime, bool, IQueryable<Facility>> s_cQry_PoSilosFromPrevIntermediateTime =
        CompiledQuery.Compile<DatabaseApp, ProdOrderPartslistPos, DateTime, bool, IQueryable<Facility>>(
            (ctx, pos, filterTimeOlderThan, checkOutwardEnabled) => ctx.FacilityBookingCharge
                                                                    .Include("InwardFacility.FacilityStock_Facility")
                                                                    .Where(c => c.ProdOrderPartslistPosID.HasValue && c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID.HasValue && c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID == pos.ProdOrderPartslistPosID
                                                                          && c.InwardFacilityID.HasValue && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer
                                                                          && c.InwardFacilityChargeID.HasValue && c.InwardFacilityCharge.NotAvailable == false
                                                                          && ((pos.Material.ProductionMaterialID.HasValue && c.InwardFacilityCharge.MaterialID == pos.Material.ProductionMaterialID)
                                                                               || (!pos.Material.ProductionMaterialID.HasValue && c.InwardFacilityCharge.MaterialID == pos.MaterialID))
                                                                           && ((checkOutwardEnabled && c.InwardFacility.OutwardEnabled)
                                                                              || !checkOutwardEnabled)
                                                                          && c.InwardFacilityCharge.FillingDate.HasValue && c.InwardFacilityCharge.FillingDate <= filterTimeOlderThan)
                                                                    .OrderBy(c => c.InwardFacilityCharge.FillingDate)
                                                                    .Select(c => c.InwardFacility)
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
                                                                ACValueList projSpecificParams = null)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosFromPrevIntermediateTime(ctx, pos, filterTimeOlderThan, checkOutwardEnabled).ToArray().Distinct().ToList());
                    // Additional filter, which checks if Silo contains quants above which is oldern than filterDate
                    //foreach (Facility facility in facilityList.ToArray())
                    //{
                    //    FacilityCharge fc = CoffeeSilo.s_cQry_PoQuants(ctx, facility).FirstOrDefault();
                    //    if (fc == null || fc.FillingDate > degassingTime)
                    //        facilityList.Remove(facility);
                    //}
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosFromPrevIntermediateTime(ctx, altPos, filterTimeOlderThan, checkOutwardEnabled).ToArray().Distinct().ToList();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult(new List<Facility>());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosFromPrevIntermediateTime", msg);

                return new QrySilosResult(new List<Facility>());
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
        protected static readonly Func<DatabaseApp, ProdOrderPartslistPos, bool, IQueryable<Facility>> s_cQry_PoSilosWithMaterial =
        CompiledQuery.Compile<DatabaseApp, ProdOrderPartslistPos, bool, IQueryable<Facility>>(
            (ctx, pos, checkOutwardEnabled) => ctx.FacilityCharge
                                                .Include("Facility.FacilityStock_Facility")
                                                .Where(c => c.NotAvailable == false
                                                      && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer
                                                      && c.Facility.MaterialID.HasValue
                                                      && ((pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.Material.ProductionMaterialID)
                                                          || (!pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.MaterialID))
                                                      && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                          || !checkOutwardEnabled)
                                                      && c.FillingDate.HasValue)
                                               .OrderBy(c => c.FillingDate)
                                               .Select(c => c.Facility)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// </summary>
        virtual public QrySilosResult SilosWithMaterial(DatabaseApp ctx,
                                                        ProdOrderPartslistPosRelation relation,
                                                        bool checkOutwardEnabled = true,
                                                        bool searchForAlternativeMaterials = false,
                                                        ACValueList projSpecificParams = null)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosWithMaterial(ctx, pos, checkOutwardEnabled).ToArray().Distinct().ToList());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosWithMaterial(ctx, altPos, checkOutwardEnabled).ToArray().Distinct().ToList();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult(new List<Facility>());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithMaterial(10)", msg);

                return new QrySilosResult(new List<Facility>());
            }
        }


        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// AND is older than the filter-Date
        /// </summary>
        protected static readonly Func<DatabaseApp, ProdOrderPartslistPos, bool, IQueryable<Facility>> s_cQry_PoSilosWithIntermediateMaterial =
        CompiledQuery.Compile<DatabaseApp, ProdOrderPartslistPos, bool, IQueryable<Facility>>(
            (ctx, pos, checkOutwardEnabled) => ctx.FacilityCharge
                                                .Include("Facility.FacilityStock_Facility")
                                                .Where(c => c.NotAvailable == false
                                                      && c.Facility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer
                                                      && c.Facility.MaterialID.HasValue
                                                      && ((pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.Material.ProductionMaterialID)
                                                          || (!pos.Material.ProductionMaterialID.HasValue && c.Facility.MaterialID == pos.MaterialID))
                                                      && (c.Facility.PartslistID.HasValue
                                                         && pos.ProdOrderPartslist.PartslistID.HasValue
                                                         && c.Facility.PartslistID == pos.ProdOrderPartslist.PartslistID)
                                                     && ((checkOutwardEnabled && c.Facility.OutwardEnabled)
                                                         || !checkOutwardEnabled)
                                                      && c.FillingDate.HasValue)
                                               .OrderBy(c => c.FillingDate)
                                               .Select(c => c.Facility)
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
            ACValueList projSpecificParams = null)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosWithIntermediateMaterial(ctx, pos, checkOutwardEnabled).ToArray().Distinct().ToList());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosWithIntermediateMaterial(ctx, altPos, checkOutwardEnabled).ToArray().Distinct().ToList();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult(new List<Facility>());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosWithIntermediateMaterial(10)", msg);

                return new QrySilosResult(new List<Facility>());
            }
        }

        #endregion

        #region Search includes previous production stage
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// AND which also could be produced from another order 
        /// </summary>
        protected static readonly Func<DatabaseApp, ProdOrderPartslistPos, bool, IQueryable<Facility>> s_cQry_PoSilosFromPrevStage =
        CompiledQuery.Compile<DatabaseApp, ProdOrderPartslistPos, bool, IQueryable<Facility>>(
            (ctx, pos, checkOutwardEnabled) => ctx.FacilityBookingCharge
                                                .Include("InwardFacility.FacilityStock_Facility")
                                                .Where(c => c.ProdOrderPartslistPosID.HasValue && c.ProdOrderPartslistPos.ProdOrderPartslistID == pos.SourceProdOrderPartslistID
                                                     && c.InwardFacilityID.HasValue && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer
                                                     && c.InwardFacilityChargeID.HasValue && c.InwardFacilityCharge.NotAvailable == false
                                                     && ((pos.Material.ProductionMaterialID.HasValue && c.InwardFacilityCharge.MaterialID == pos.Material.ProductionMaterialID)
                                                          || (!pos.Material.ProductionMaterialID.HasValue && c.InwardFacilityCharge.MaterialID == pos.MaterialID))
                                                     && ((pos.Material.ProductionMaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID == pos.Material.ProductionMaterialID)
                                                          || (!pos.Material.ProductionMaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID.HasValue && c.InwardFacilityCharge.Facility.MaterialID == pos.MaterialID))
                                                     && c.InwardFacilityCharge.FillingDate.HasValue
                                                     && ((checkOutwardEnabled && c.InwardFacility.OutwardEnabled)
                                                          || !checkOutwardEnabled))
                                               .OrderBy(c => c.InwardFacilityCharge.FillingDate)
                                               .Select(c => c.InwardFacility)
        );
        /// <summary>
        /// Queries Silos 
        /// which contains this material 
        /// AND which also could be produced from another order 
        /// </summary>
        virtual public QrySilosResult SilosFromPrevStage(DatabaseApp ctx, ProdOrderPartslistPosRelation relation, bool checkOutwardEnabled = true, bool searchForAlternativeMaterials = false, ACValueList projSpecificParams = null)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosFromPrevStage(ctx, pos, checkOutwardEnabled).ToArray().Distinct().ToList());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosFromPrevStage(ctx, altPos, checkOutwardEnabled).ToArray().Distinct().ToList();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult(new List<Facility>());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosFromPrevStage", msg);

                return new QrySilosResult(new List<Facility>());
            }
        }


        /// <summary>
        /// Queries Silos 
        /// which contains this intermediate product 
        /// AND which also could be produced from another order 
        /// </summary>
        protected static readonly Func<DatabaseApp, ProdOrderPartslistPos, bool, IQueryable<Facility>> s_cQry_PoSilosFromPrevIntermediate =
        CompiledQuery.Compile<DatabaseApp, ProdOrderPartslistPos, bool, IQueryable<Facility>>(
            (ctx, pos, checkOutwardEnabled) => ctx.FacilityBookingCharge
                                                .Include("InwardFacility.FacilityStock_Facility")
                                                .Where(c => c.ProdOrderPartslistPosID.HasValue && c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID.HasValue && c.ProdOrderPartslistPos.ParentProdOrderPartslistPosID == pos.ProdOrderPartslistPosID
                                                     && c.InwardFacilityID.HasValue && c.InwardFacility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer
                                                     && c.InwardFacilityChargeID.HasValue && c.InwardFacilityCharge.NotAvailable == false
                                                     && ((pos.Material.ProductionMaterialID.HasValue && c.InwardFacilityCharge.MaterialID == pos.Material.ProductionMaterialID)
                                                          || (!pos.Material.ProductionMaterialID.HasValue && c.InwardFacilityCharge.MaterialID == pos.MaterialID))
                                                     && c.InwardFacilityCharge.FillingDate.HasValue
                                                     && ((checkOutwardEnabled && c.InwardFacility.OutwardEnabled)
                                                          || !checkOutwardEnabled))
                                               .OrderBy(c => c.InwardFacilityCharge.FillingDate)
                                               .Select(c => c.InwardFacility)
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
                                                                ACValueList projSpecificParams = null)
        {
            try
            {
                ProdOrderPartslistPos pos = relation.SourceProdOrderPartslistPos;
                if (!searchForAlternativeMaterials)
                {
                    return new QrySilosResult(s_cQry_PoSilosFromPrevIntermediate(ctx, pos, checkOutwardEnabled).ToArray().Distinct().ToList());
                }
                else if (pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos.Any())
                {
                    foreach (ProdOrderPartslistPos altPos in pos.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                    {
                        var result = s_cQry_PoSilosFromPrevIntermediate(ctx, altPos, checkOutwardEnabled).ToArray().Distinct().ToList();
                        if (result.Any())
                        {
                            return new QrySilosResult(result, altPos);
                        }
                    }
                }
                return new QrySilosResult(new List<Facility>());
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("ACPartslistManager", "SilosFromPrevIntermediate", msg);

                return new QrySilosResult(new List<Facility>());
            }
        }
        #endregion

        #endregion
        
        #endregion

        #endregion

        #region Methods
        public static void RemoveFacility(Guid? ignoreFacilityID, IEnumerable<gip.core.datamodel.ACClass> exclusionList, IList<Facility> possibleSilos)
        {
            if (possibleSilos == null || !possibleSilos.Any())
                return;
            if (ignoreFacilityID.HasValue)
            {
                Facility facilityToRemove = possibleSilos.Where(c => c.FacilityID == ignoreFacilityID.Value).FirstOrDefault();
                if (facilityToRemove != null)
                {
                    possibleSilos.Remove(facilityToRemove);
                }
            }
            if (exclusionList != null)
            {
                foreach (gip.core.datamodel.ACClass silo2Exlude in exclusionList)
                {
                    Facility facilityToRemove = possibleSilos.Where(c => c.VBiFacilityACClassID.HasValue  && c.VBiFacilityACClassID == silo2Exlude.ACClassID).FirstOrDefault();
                    if (facilityToRemove != null)
                    {
                        possibleSilos.Remove(facilityToRemove);
                    }
                }
            }
        }

        public virtual IList<Facility> FindSilos(IPartslistPosRelation relation,
                                                DatabaseApp dbApp, Database dbIPlus,
                                                SearchMode searchMode,
                                                DateTime? filterTimeOlderThan,
                                                Guid? ignoreFacilityID,
                                                IEnumerable<gip.core.datamodel.ACClass> exclusionList = null,
                                                ACValueList projSpecificParams = null)
        {
            IList<Facility> possibleSilos = null;
            PartslistPosRelation plRelation = null;
            ProdOrderPartslistPosRelation poRelation = relation as ProdOrderPartslistPosRelation;
            if (poRelation == null)
                plRelation = relation as PartslistPosRelation;

            // 1. Suche freie Silos, mit dem zu dosierenden Material + die Freigegeben sind + die keine gesperrte Chargen haben
            // soriert nach der ältesten eingelagerten Charge
            QrySilosResult facilityQuery = null;
            if (poRelation != null)
            {
                if (filterTimeOlderThan.HasValue)
                {
                    if (poRelation.SourceProdOrderPartslistPos.SourceProdOrderPartslistID.HasValue)
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosFromPrevIntermediateTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                        }
                        else
                        {
                            facilityQuery = SilosFromPrevStageTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                        }
                    }
                    else
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterialTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterialTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                        }
                    }
                }
                else
                {
                    if (poRelation.SourceProdOrderPartslistPos.SourceProdOrderPartslistID.HasValue)
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosFromPrevIntermediate(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                        }
                        else
                        {
                            facilityQuery = SilosFromPrevStage(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                        }
                    }
                    else
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterial(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterial(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                        }
                    }
                }
            }
            else
            {
                if (filterTimeOlderThan.HasValue)
                {
                    if (plRelation.SourcePartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                    {
                        facilityQuery = SilosWithIntermediateMaterialTime(dbApp, plRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                    }
                    else
                    {
                        facilityQuery = SilosWithMaterialTime(dbApp, plRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                    }
                }
                else
                {
                    if (plRelation.SourcePartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                    {
                        facilityQuery = SilosWithIntermediateMaterial(dbApp, plRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                    }
                    else
                    {
                        facilityQuery = SilosWithMaterial(dbApp, plRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                    }
                }
            }

            possibleSilos = facilityQuery.FoundSilos
                            .ToList()
                            .Distinct()
                            .Where(c => !c.QryHasBlockedQuants.Any())
                            .ToList();
            RemoveFacility(ignoreFacilityID, exclusionList, possibleSilos);
            if (possibleSilos.Any())
                return possibleSilos;

            // 2. Suche nach Material das von einem anderen Auftrag produziert worden ist, wenn es keine Silos gibt
            if (poRelation != null)
            {
                // Prüfe ob Entnahme von anderem Auftrag erlaubt
                if (poRelation.SourceProdOrderPartslistPos.TakeMatFromOtherOrder
                    && poRelation.SourceProdOrderPartslistPos.SourceProdOrderPartslistID.HasValue)
                {
                    if (filterTimeOlderThan.HasValue)
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterialTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterialTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                        }
                    }
                    else
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterial(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterial(dbApp, poRelation, searchMode != SearchMode.AllSilos, false, projSpecificParams);
                        }
                    }
                    possibleSilos = facilityQuery.FoundSilos
                                    .ToList()
                                    .Distinct()
                                    .Where(c => !c.QryHasBlockedQuants.Any())
                                    .ToList();
                    RemoveFacility(ignoreFacilityID, exclusionList, possibleSilos);
                    if (possibleSilos.Any())
                        return possibleSilos;
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
                            facilityQuery = SilosWithIntermediateMaterialTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, true, projSpecificParams);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterialTime(dbApp, poRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, true, projSpecificParams);
                        }
                    }
                    else
                    {
                        if (poRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterial(dbApp, poRelation, searchMode != SearchMode.AllSilos, true, projSpecificParams);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterial(dbApp, poRelation, searchMode != SearchMode.AllSilos, true, projSpecificParams);
                        }
                    }
                    possibleSilos = facilityQuery.FoundSilos
                                    .ToList()
                                    .Distinct()
                                    .Where(c => !c.QryHasBlockedQuants.Any())
                                    .ToList();
                    RemoveFacility(ignoreFacilityID, exclusionList, possibleSilos);
                    return possibleSilos;
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
                            facilityQuery = SilosWithIntermediateMaterialTime(dbApp, plRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, true, projSpecificParams);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterialTime(dbApp, plRelation, filterTimeOlderThan.Value, searchMode != SearchMode.AllSilos, true, projSpecificParams);
                        }
                    }
                    else
                    {
                        if (plRelation.SourcePartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern)
                        {
                            facilityQuery = SilosWithIntermediateMaterial(dbApp, plRelation, searchMode != SearchMode.AllSilos, true, projSpecificParams);
                        }
                        else
                        {
                            facilityQuery = SilosWithMaterial(dbApp, plRelation, searchMode != SearchMode.AllSilos, true, projSpecificParams);
                        }
                    }
                    possibleSilos = facilityQuery.FoundSilos
                                    .ToList()
                                    .Distinct()
                                    .Where(c => !c.QryHasBlockedQuants.Any())
                                    .ToList();
                    RemoveFacility(ignoreFacilityID, exclusionList, possibleSilos);
                    return possibleSilos;
                }
            }

            return possibleSilos;
        }

        public virtual IEnumerable<Route> GetRoutes(IPartslistPosRelation relation,
                                                DatabaseApp dbApp, Database dbIPlus,
                                                gip.core.datamodel.ACClass scaleACClass,
                                                SearchMode searchMode,
                                                DateTime? filterTimeOlderThan,
                                                out IList<Facility> possibleSilos,
                                                Guid? ignoreFacilityID,
                                                IEnumerable<gip.core.datamodel.ACClass> exclusionList = null,
                                                ACValueList projSpecificParams = null)
        {
            if (scaleACClass == null)
            {
                throw new NullReferenceException("AccessedProcessModule is null");
            }

            possibleSilos = FindSilos(relation, dbApp, dbIPlus, searchMode, filterTimeOlderThan, ignoreFacilityID, exclusionList, projSpecificParams);
            if (possibleSilos == null || !possibleSilos.Any())
                return null;

            RoutingResult result = null;
            if (searchMode == SearchMode.OnlyEnabledOldestSilo)
            {
                Facility oldestSilo = possibleSilos.FirstOrDefault();
                if (oldestSilo == null)
                    return null;
                if (!oldestSilo.VBiFacilityACClassID.HasValue)
                    return null;
                var oldestSiloClass = oldestSilo.GetFacilityACClass(dbIPlus);

                result = ACRoutingService.SelectRoutes(RoutingService, dbIPlus, true,
                                        scaleACClass, oldestSiloClass, RouteDirections.Backwards, "PAMSilo.Deselector", new object[] { },
                                        (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && oldestSilo.VBiFacilityACClassID == c.ACClassID,
                                        (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != scaleACClass.ACClassID, // Breche Suche ab sobald man bei einem Vorgänger der ein Silo oder Waage angelangt ist
                                        0,true, true, false, false, 10);
                if (result.Routes == null || !result.Routes.Any())
                {
                    // TODO: Fehler
                    return null;
                }
            }
            else
            {
                // 2. Suche Routen zu dieser Waage die von den vorgeschlagenen Silos aus führen
                var acClassIDsOfPossibleSilos = possibleSilos.Where(c => c.VBiFacilityACClassID.HasValue).Select(c => c.VBiFacilityACClassID.Value);
                IEnumerable<string> possibleSilosACUrl = possibleSilos.Where(c => c.FacilityACClass != null).Select(x => x.FacilityACClass.GetACUrlComponent());

                result = ACRoutingService.SelectRoutes(RoutingService, dbIPlus, true,
                                        scaleACClass, possibleSilosACUrl, RouteDirections.Backwards, "PAMSilo.Deselector", new object[] { },
                                        (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && acClassIDsOfPossibleSilos.Contains(c.ACClassID),
                                        (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != scaleACClass.ACClassID, // Breche Suche ab sobald man bei einem Vorgänger der ein Silo oder Waage angelangt ist
                                        0, true, true, false, false, 10);

                if (result.Routes == null || !result.Routes.Any())
                {
                    // TODO: Fehler
                    return null;
                }
            }
            return result.Routes;
        }
        #endregion
    }
}
