using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.webservices
{
    public partial class VBWebService
    {
        #region ProdOrderPartslist

        public static readonly Func<DatabaseApp, Guid?, IQueryable<gip.mes.datamodel.ProdOrderPartslist>> s_cQry_GetProdOrderPartslist =
            CompiledQuery.Compile<DatabaseApp, Guid?, IQueryable<gip.mes.datamodel.ProdOrderPartslist>>(
                (dbApp, prodOrderPartslistID) =>
                    dbApp.ProdOrderPartslist
                                   .Include("ProdOrder")
                                   .Include("Partslist")
                                   .Include("Partslist.Material")
                                   .Include("Partslist.Material.BaseMDUnit")
                                   .Where(c => (prodOrderPartslistID != null && c.ProdOrderPartslistID == prodOrderPartslistID)
                                               || (prodOrderPartslistID == null && c.ProdOrder.MDProdOrderState.MDProdOrderStateIndex == (short)MDProdOrderState.ProdOrderStates.InProduction)
                                   )
                                   .OrderByDescending(x => x.UpdateDate)
            );

        public static readonly Func<DatabaseApp, Guid[], IQueryable<gip.mes.datamodel.ProdOrderPartslist>> s_cQry_GetProdOrderPartslists =
            CompiledQuery.Compile<DatabaseApp, Guid[], IQueryable<gip.mes.datamodel.ProdOrderPartslist>>(
                (dbApp, prodOrderPartslistID) =>
                    dbApp.ProdOrderPartslist
                                   .Include("ProdOrder")
                                   .Include("Partslist")
                                   .Include("Partslist.Material")
                                   .Include("Partslist.Material.BaseMDUnit")
                                   .Where(c => (prodOrderPartslistID != null && prodOrderPartslistID.Contains(c.ProdOrderPartslistID))
                                               || (prodOrderPartslistID == null && c.ProdOrder.MDProdOrderState.MDProdOrderStateIndex == (short)MDProdOrderState.ProdOrderStates.InProduction)
                                   )
                                   .OrderByDescending(x => x.UpdateDate)
            );

        public virtual IEnumerable<ProdOrderPartslist> ConvertToWSProdOrderPartslists(IQueryable<gip.mes.datamodel.ProdOrderPartslist> query)
        {
            return query.AsEnumerable().Select(c => new ProdOrderPartslist()
            {
                ProdOrderPartslistID = c.ProdOrderPartslistID,
                ActualQuantity = c.ActualQuantity,
                TargetQuantity = c.TargetQuantity,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Comment = c.DepartmentUserName,
                ProdOrder = new ProdOrder()
                {
                    ProdOrderID = c.ProdOrder.ProdOrderID,
                    ProgramNo = c.ProdOrder.ProgramNo
                },
                Partslist = new Partslist()
                {
                    PartlistID = c.Partslist.PartslistID,
                    PartslistNo = c.Partslist.PartslistNo,
                    PartslistName = c.Partslist.PartslistName,
                    PartslistVersion = c.Partslist.PartslistVersion,
                    Material = new Material()
                    {
                        MaterialID = c.Partslist.MaterialID,
                        MaterialNo = c.Partslist.Material.MaterialNo,
                        MaterialName1 = c.Partslist.Material.MaterialName1,
                        BaseMDUnit = new MDUnit()
                        {
                            MDUnitID = c.Partslist.Material.BaseMDUnitID,
                            MDUnitNameTrans = c.Partslist.Material.BaseMDUnit.MDUnitNameTrans,
                            SymbolTrans = c.Partslist.Material.BaseMDUnit.SymbolTrans
                        }
                    }
                }
            });
        }

        public WSResponse<List<ProdOrderPartslist>> GetProdOrderPartslists()
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<ProdOrderPartslist>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderPartslists));
            List<ProdOrderPartslist> result = null;
            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    result = ConvertToWSProdOrderPartslists(s_cQry_GetProdOrderPartslist(dbApp, null).Take(myServiceHost.Root.Environment.AccessDefaultTakeCount)).ToList();
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderPartslists) + "(10)", e);
                return new WSResponse<List<ProdOrderPartslist>>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderPartslists));
            }
            return new WSResponse<List<ProdOrderPartslist>>(result);
        }

        public WSResponse<ProdOrderPartslist> GetProdOrderPartslist(string prodOrderPartslistID)
        {
            if (string.IsNullOrEmpty(prodOrderPartslistID))
                return new WSResponse<ProdOrderPartslist>(null, new Msg(eMsgLevel.Error, "prodOrderPartslistID is empty"));

            Guid guid;
            if (!Guid.TryParse(prodOrderPartslistID, out guid))
                return new WSResponse<ProdOrderPartslist>(null, new Msg(eMsgLevel.Error, "prodOrderPartslistID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<ProdOrderPartslist>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderPartslist));
            ProdOrderPartslist result = null;
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    result = ConvertToWSProdOrderPartslists(s_cQry_GetProdOrderPartslist(dbApp, guid)).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderPartslist) + "(10)", e);
                return new WSResponse<ProdOrderPartslist>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderPartslist));
            }
            return new WSResponse<ProdOrderPartslist>(result);
        }

        #endregion

        #region Intermediates

        public static readonly Func<DatabaseApp, Guid?, Guid?, IQueryable<gip.mes.datamodel.ProdOrderPartslistPos>> s_cQry_GetProdOrderPLIntermediates =
                        CompiledQuery.Compile<DatabaseApp, Guid?, Guid?, IQueryable<gip.mes.datamodel.ProdOrderPartslistPos>>(
                            (dbApp, prodOrderPartslistID, prodOrderPartslistPosID) =>
                                dbApp.ProdOrderPartslistPos.Include("Material")
                                                           .Include("Material.BaseMDUnit")
                                                           .Include("ProdOrderPartslist")
                                                           .Include("ProdOrderPartslist.ProdOrder")
                                                           .Include("ProdOrderPartslist.Partslist")
                                                           .Include("ProdOrderPartslist.Partslist.Material")
                                                           .Include("ProdOrderPartslist.Partslist.Material.BaseMDUnit")
                                                           .Include("ProdOrderPartslistPos1_ParentProdOrderPartslistPos")
                                                           .Include("Material.MaterialWFRelation_SourceMaterial")
                                                           .Where(c => (prodOrderPartslistID.HasValue
                                                                          && c.ProdOrderPartslistID == prodOrderPartslistID
                                                                          && c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                                                                      || (prodOrderPartslistPosID.HasValue
                                                                          && c.ProdOrderPartslistPosID == prodOrderPartslistPosID)
                                                                 )
                                                           .OrderBy(x => x.Sequence)
                                                           .ThenBy(c => c.Material != null ? c.Material.MaterialNo : "")
                        );

        public static readonly Func<DatabaseApp, Guid[], IQueryable<gip.mes.datamodel.ProdOrderPartslistPos>> s_cQry_GetProdOrderPLIntermediatesFromArray =
                        CompiledQuery.Compile<DatabaseApp, Guid[], IQueryable<gip.mes.datamodel.ProdOrderPartslistPos>>(
                            (dbApp, prodOrderPartslistPosID) =>
                                    dbApp.ProdOrderPartslistPos.Include("Material")
                                                               .Include("Material.BaseMDUnit")
                                                               .Include("ProdOrderPartslist")
                                                               .Include("ProdOrderPartslist.ProdOrder")
                                                               .Include("ProdOrderPartslist.Partslist")
                                                               .Include("ProdOrderPartslist.Partslist.Material")
                                                               .Include("ProdOrderPartslist.Partslist.Material.BaseMDUnit")
                                                               .Include("ProdOrderPartslistPos1_ParentProdOrderPartslistPos")
                                                               .Include("Material.MaterialWFRelation_SourceMaterial")
                                                               .Where(c => (prodOrderPartslistPosID != null
                                                                              && prodOrderPartslistPosID.Contains(c.ProdOrderPartslistPosID))
                                                                     )
                                                               .OrderBy(x => x.Sequence)
                                                               .ThenBy(c => c.Material != null ? c.Material.MaterialNo : "")
                        );

        public IEnumerable<ProdOrderPartslistPos> ConvertToWSProdOrderPLIntermediates(IQueryable<gip.mes.datamodel.ProdOrderPartslistPos> query)
        {
            return query.AsEnumerable().Select(c => new ProdOrderPartslistPos()
            {
                ProdOrderPartslistPosID = c.ProdOrderPartslistPosID,
                Sequence = c.Sequence,
                ActualQuantity = c.ActualQuantity,
                ActualQuantityUOM = c.ActualQuantityUOM,
                TargetQuantity = c.TargetQuantity,
                TargetQuantityUOM = c.TargetQuantityUOM,
                BookingMaterialID = c.BookingMaterial?.MaterialID,
                FacilityLotID = c.FacilityLotID,
                IsFinalMixure = c.IsFinalMixure || c.IsFinalMixureBatch,
                Material = new Material()
                {
                    MaterialID = c.Material.MaterialID,
                    MaterialNo = c.Material.MaterialNo,
                    MaterialName1 = c.Material.MaterialName1,
                    BaseMDUnit = new MDUnit()
                    {
                        MDUnitID = c.Material.BaseMDUnit.MDUnitID,
                        MDUnitNameTrans = c.Material.BaseMDUnit.MDUnitNameTrans,
                        SymbolTrans = c.Material.BaseMDUnit.SymbolTrans
                    }
                },
                ProdOrderPartslist = new ProdOrderPartslist()
                {
                    ProdOrderPartslistID = c.ProdOrderPartslistID,
                    ActualQuantity = c.ProdOrderPartslist.ActualQuantity,
                    TargetQuantity = c.ProdOrderPartslist.TargetQuantity,
                    Partslist = new Partslist()
                    {
                        PartlistID = c.ProdOrderPartslist.PartslistID.Value,
                        PartslistNo = c.ProdOrderPartslist.Partslist?.PartslistNo,
                        PartslistName = c.ProdOrderPartslist.Partslist?.PartslistName,
                        PartslistVersion = c.ProdOrderPartslist.Partslist?.PartslistVersion,
                        Material = new Material()
                        {
                            MaterialID = c.ProdOrderPartslist.Partslist.MaterialID,
                            MaterialNo = c.ProdOrderPartslist.Partslist.Material.MaterialNo,
                            MaterialName1 = c.ProdOrderPartslist.Partslist.Material.MaterialName1,
                            BaseMDUnit = new MDUnit()
                            {
                                MDUnitID = c.ProdOrderPartslist.Partslist.Material.BaseMDUnit.MDUnitID,
                                MDUnitNameTrans = c.ProdOrderPartslist.Partslist.Material.BaseMDUnit.MDUnitNameTrans,
                                SymbolTrans = c.ProdOrderPartslist.Partslist.Material.BaseMDUnit.SymbolTrans
                            }
                        }
                    },
                    ProdOrder = new ProdOrder()
                    {
                        ProdOrderID = c.ProdOrderPartslist.ProdOrderID,
                        ProgramNo = c.ProdOrderPartslist.ProdOrder.ProgramNo
                    }
                },
                MDUnit = c.MDUnit != null ? new MDUnit()
                {
                    MDUnitID = c.MDUnit.MDUnitID,
                    MDUnitNameTrans = c.MDUnit.MDUnitNameTrans
                } : null
            });
        }

        public WSResponse<List<ProdOrderPartslistPos>> GetProdOrderPLIntermediates(string prodOrderPartslistID)
        {
            if (string.IsNullOrEmpty(prodOrderPartslistID))
                return new WSResponse<List<ProdOrderPartslistPos>>(null, new Msg(eMsgLevel.Error, "prodOrderPartslistID is empty"));

            Guid guid;
            if (!Guid.TryParse(prodOrderPartslistID, out guid))
                return new WSResponse<List<ProdOrderPartslistPos>>(null, new Msg(eMsgLevel.Error, "prodOrderPartslistID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<ProdOrderPartslistPos>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderPLIntermediates));
            List<ProdOrderPartslistPos> result = null;
            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    result = ConvertToWSProdOrderPLIntermediates(s_cQry_GetProdOrderPLIntermediates(dbApp, guid, null)).ToList();
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderPLIntermediates) + "(10)", e);
                return new WSResponse<List<ProdOrderPartslistPos>>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderPLIntermediates));
            }
            return new WSResponse<List<ProdOrderPartslistPos>>(result);
        }

        public WSResponse<ProdOrderPartslistPos> GetProdOrderPLIntermediate(string intermediateID)
        {
            if (string.IsNullOrEmpty(intermediateID))
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "intermediateID is empty"));

            Guid guid;
            if (!Guid.TryParse(intermediateID, out guid))
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "intermediateID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderPLIntermediate));
            ProdOrderPartslistPos result = null;
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    result = ConvertToWSProdOrderPLIntermediates(s_cQry_GetProdOrderPLIntermediates(dbApp, null, guid)).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderPLIntermediate) + "(10)", e);
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderPLIntermediate));
            }
            return new WSResponse<ProdOrderPartslistPos>(result);
        }

        #endregion

        #region InputMaterials

        public static readonly Func<DatabaseApp, Guid?, IQueryable<gip.mes.datamodel.ProdOrderPartslistPosRelation>> s_cQry_GetProdOrderInputMaterials =
                CompiledQuery.Compile<DatabaseApp, Guid?, IQueryable<gip.mes.datamodel.ProdOrderPartslistPosRelation>>(
                    (dbApp, targetProdOrderPLPosID) =>
                        dbApp.ProdOrderPartslistPosRelation.Include("SourceProdOrderPartslistPos")
                                                           .Include("SourceProdOrderPartslistPos.Material")
                                                           .Include("SourceProdOrderPartslistPos.Material.BaseMDUnit")
                                                           .Include("SourceProdOrderPartslistPos.ProdOrderPartslist")
                                                           .Include("SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist")
                                                           .Include("SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material")
                                                           .Include("SourceProdOrderPartslistPos.MDUnit")
                                                           .Where(c => targetProdOrderPLPosID.HasValue && c.TargetProdOrderPartslistPosID == targetProdOrderPLPosID
                                                                                                       && c.SourceProdOrderPartslistPos != null
                                                                                                       && c.SourceProdOrderPartslistPos.Material != null
                                                                                                       && !c.SourceProdOrderPartslistPos.Material.IsIntermediate)
                                                           .OrderBy(x => x.Sequence)
                );


        public IEnumerable<ProdOrderPartslistPosRelation> ConvertToWSProdOrderPLPosInputMaterials(IQueryable<gip.mes.datamodel.ProdOrderPartslistPosRelation> query)
        {
            return query.AsEnumerable().Select(c => new ProdOrderPartslistPosRelation()
            {
                ProdOrderPartslistPosRelationID = c.ProdOrderPartslistPosRelationID,
                Sequence = c.Sequence,
                ActualQuantity = c.ActualQuantity,
                ActualQuantityUOM = c.ActualQuantityUOM,
                TargetQuantity = c.TargetQuantity,
                TargetQuantityUOM = c.TargetQuantityUOM,
                RetrogradeFIFO = c.RetrogradeFIFO,
                SourcePos = new ProdOrderPartslistPos()
                {
                    ProdOrderPartslistPosID = c.SourceProdOrderPartslistPosID,
                    RetrogradeFIFO = c.RetrogradeFIFO,
                    Material = new Material()
                    {
                        MaterialID = c.SourceProdOrderPartslistPos.Material.MaterialID,
                        MaterialNo = c.SourceProdOrderPartslistPos.Material.MaterialNo,
                        MaterialName1 = c.SourceProdOrderPartslistPos.Material.MaterialName1,
                        RetrogradeFIFO = c.SourceProdOrderPartslistPos.Material.RetrogradeFIFO,
                        BaseMDUnit = new MDUnit()
                        {
                            MDUnitID = c.SourceProdOrderPartslistPos.Material.BaseMDUnit.MDUnitID,
                            MDUnitNameTrans = c.SourceProdOrderPartslistPos.Material.BaseMDUnit.MDUnitNameTrans,
                            SymbolTrans = c.SourceProdOrderPartslistPos.Material.BaseMDUnit.SymbolTrans
                        }
                    },
                    ProdOrderPartslist = new ProdOrderPartslist()
                    {
                        ProdOrderPartslistID = c.SourceProdOrderPartslistPos.ProdOrderPartslistID,
                        ActualQuantity = c.SourceProdOrderPartslistPos.ProdOrderPartslist.ActualQuantity,
                        TargetQuantity = c.SourceProdOrderPartslistPos.ProdOrderPartslist.TargetQuantity,
                        Partslist = new Partslist()
                        {
                            PartlistID = c.SourceProdOrderPartslistPos.ProdOrderPartslist.PartslistID.Value,
                            PartslistNo = c.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist?.PartslistNo,
                            PartslistName = c.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist?.PartslistName,
                            PartslistVersion = c.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist?.PartslistVersion,
                            Material = new Material()
                            {
                                MaterialID = c.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.MaterialID,
                                MaterialNo = c.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialNo,
                                MaterialName1 = c.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialName1,
                                BaseMDUnit = new MDUnit()
                                {
                                    MDUnitID = c.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.BaseMDUnit.MDUnitID,
                                    MDUnitNameTrans = c.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.BaseMDUnit.MDUnitNameTrans,
                                    SymbolTrans = c.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.BaseMDUnit.SymbolTrans
                                }
                            }
                        }
                    },
                    MDUnit = c.SourceProdOrderPartslistPos.MDUnit != null ? new MDUnit()
                    {
                        MDUnitID = c.SourceProdOrderPartslistPos.MDUnit.MDUnitID,
                        MDUnitNameTrans = c.SourceProdOrderPartslistPos.MDUnit.MDUnitNameTrans,
                        SymbolTrans = c.SourceProdOrderPartslistPos.MDUnit.SymbolTrans
                    } : null
                },
                TargetPos = new ProdOrderPartslistPos()
                {
                    ProdOrderPartslistPosID = c.TargetProdOrderPartslistPosID
                }
            });
        }

        public WSResponse<List<ProdOrderPartslistPosRelation>> GetProdOrderInputMaterials(string targetPOPLPosID)
        {
            if (string.IsNullOrEmpty(targetPOPLPosID))
                return new WSResponse<List<ProdOrderPartslistPosRelation>>(null, new Msg(eMsgLevel.Error, "targetPOPLPosID is empty"));

            Guid guid;
            if (!Guid.TryParse(targetPOPLPosID, out guid))
                return new WSResponse<List<ProdOrderPartslistPosRelation>>(null, new Msg(eMsgLevel.Error, "targetPOPLPosID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<ProdOrderPartslistPosRelation>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderInputMaterials));
            List<ProdOrderPartslistPosRelation> result = null;
            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    result = ConvertToWSProdOrderPLPosInputMaterials(s_cQry_GetProdOrderInputMaterials(dbApp, guid)).ToList();
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderInputMaterials) + "(10)", e);
                return new WSResponse<List<ProdOrderPartslistPosRelation>>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderInputMaterials));
            }
            return new WSResponse<List<ProdOrderPartslistPosRelation>>(result);
        }

        #endregion

        #region Batches

        public static readonly Func<DatabaseApp, Guid?, Guid?, IQueryable<gip.mes.datamodel.ProdOrderPartslistPos>> s_cQry_GetProdOrderIntermBatches =
                CompiledQuery.Compile<DatabaseApp, Guid?, Guid?, IQueryable<gip.mes.datamodel.ProdOrderPartslistPos>>(
                    (dbApp, parentIntermediateID, batchIntermediateID) =>
                        dbApp.ProdOrderPartslistPos.Include("Material")
                                                   .Include("ProdOrderBatch")
                                                   .Include("ProdOrderPartslist")
                                                   .Include("ProdOrderPartslist.ProdOrder")
                                                   .Include("MDUnit")
                                                   .Include("ProdOrderPartslistPos1_ParentProdOrderPartslistPos")
                                                   .Include("ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos")
                                                   .Include("ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.SourceProdOrderPartslistPos")
                                                   .Include("ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.SourceProdOrderPartslistPos.Material")
                                                   .Include("Material.MaterialWFRelation_SourceMaterial")
                                                   .Where(c => (parentIntermediateID.HasValue
                                                                    && c.ParentProdOrderPartslistPosID == parentIntermediateID
                                                                    && c.ProdOrderBatch != null
                                                                    && (c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex < (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                                                        || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex > (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled))
                                                            || (batchIntermediateID.HasValue
                                                                    && c.ProdOrderPartslistPosID == batchIntermediateID)
                                                   )
                                                   .OrderBy(x => x.Sequence)
                                                   .ThenBy(c => c.Material != null ? c.Material.MaterialNo : "")
                );

        public static readonly Func<DatabaseApp, Guid[], IQueryable<gip.mes.datamodel.ProdOrderPartslistPos>> s_cQry_GetProdOrderIntermBatchesFromArray =
                CompiledQuery.Compile<DatabaseApp, Guid[], IQueryable<gip.mes.datamodel.ProdOrderPartslistPos>>(
                    (dbApp, batchIntermediateID) =>
                        dbApp.ProdOrderPartslistPos.Include("Material")
                                                   .Include("ProdOrderBatch")
                                                   .Include("ProdOrderPartslist")
                                                   .Include("ProdOrderPartslist.ProdOrder")
                                                   .Include("MDUnit")
                                                   .Include("ProdOrderPartslistPos1_ParentProdOrderPartslistPos")
                                                   .Include("ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos")
                                                   .Include("ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.SourceProdOrderPartslistPos")
                                                   .Include("ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.SourceProdOrderPartslistPos.Material")
                                                   .Include("Material.MaterialWFRelation_SourceMaterial")
                                                   .Where(c => (c.ProdOrderBatch != null
                                                                    && (c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex < (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                                                        || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex > (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled))
                                                            || (batchIntermediateID != null
                                                                    && batchIntermediateID.Contains(c.ProdOrderPartslistPosID))
                                                   )
                                                   .OrderBy(x => x.Sequence)
                                                   .ThenBy(c => c.Material != null ? c.Material.MaterialNo : "")
                );

        public IEnumerable<ProdOrderPartslistPos> ConvertToWSProdOrderIntermBatches(IQueryable<gip.mes.datamodel.ProdOrderPartslistPos> query)
        {
            return query.AsEnumerable().Select(c => new ProdOrderPartslistPos()
            {
                ProdOrderPartslistPosID = c.ProdOrderPartslistPosID,
                Sequence = c.Sequence,
                ActualQuantity = c.ActualQuantity,
                ActualQuantityUOM = c.ActualQuantityUOM,
                TargetQuantity = c.TargetQuantity,
                TargetQuantityUOM = c.TargetQuantityUOM,
                BookingMaterialID = c.BookingMaterial?.MaterialID,
                BookingMaterialInfo = c.BookingMaterial?.ACCaption,
                FacilityLotID = c.FacilityLotID,
                IsFinalMixure = c.IsFinalMixure || c.IsFinalMixureBatch,
                HasInputMaterials = c.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Where(x => !x.SourceProdOrderPartslistPos.Material.IsIntermediate).Any(),
                ProdOrderPartslist = new ProdOrderPartslist()
                {
                    ProdOrderPartslistID = c.ProdOrderPartslistID,
                    ActualQuantity = c.ProdOrderPartslist.ActualQuantity,
                    TargetQuantity = c.ProdOrderPartslist.TargetQuantity,
                    ProdOrder = new ProdOrder()
                    {
                        ProdOrderID = c.ProdOrderPartslist.ProdOrderID,
                        ProgramNo = c.ProdOrderPartslist.ProdOrder.ProgramNo
                    },
                    Partslist = new Partslist()
                    {
                        PartlistID = c.ProdOrderPartslist.PartslistID ?? Guid.Empty
                    }
                },
                Material = new Material()
                {
                    MaterialID = c.Material.MaterialID,
                    MaterialNo = c.Material.MaterialNo,
                    MaterialName1 = c.Material.MaterialName1,
                    BaseMDUnit = new MDUnit()
                    {
                        MDUnitID = c.Material.BaseMDUnit.MDUnitID,
                        MDUnitNameTrans = c.Material.BaseMDUnit.MDUnitNameTrans
                    }
                },
                ProdOrderBatch = new ProdOrderBatch()
                {
                    ProdOrderBatchID = c.ProdOrderBatchID.Value,
                    ProdOrderBatchNo = c.ProdOrderBatch.ProdOrderBatchNo,
                    BatchSeqNo = c.ProdOrderBatch.BatchSeqNo
                },
                MDUnit = c.MDUnit != null ? new MDUnit()
                {
                    MDUnitID = c.MDUnit.MDUnitID,
                    MDUnitNameTrans = c.MDUnit.MDUnitNameTrans
                } : null
            });
        }

        public WSResponse<List<ProdOrderPartslistPos>> GetProdOrderIntermBatches(string parentIntermediateID)
        {
            if (string.IsNullOrEmpty(parentIntermediateID))
                return new WSResponse<List<ProdOrderPartslistPos>>(null, new Msg(eMsgLevel.Error, "parentIntermediateID is empty"));

            Guid guid;
            if (!Guid.TryParse(parentIntermediateID, out guid))
                return new WSResponse<List<ProdOrderPartslistPos>>(null, new Msg(eMsgLevel.Error, "parentIntermediateID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<ProdOrderPartslistPos>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderIntermBatches));
            List<ProdOrderPartslistPos> result = null;
            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    result = ConvertToWSProdOrderIntermBatches(s_cQry_GetProdOrderIntermBatches(dbApp, guid, null)).ToList();
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderIntermBatches) + "(10)", e);
                return new WSResponse<List<ProdOrderPartslistPos>>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderIntermBatches));
            }
            return new WSResponse<List<ProdOrderPartslistPos>>(result);
        }

        public WSResponse<ProdOrderPartslistPos> GetProdOrderIntermBatch(string intermediateID)
        {
            if (string.IsNullOrEmpty(intermediateID))
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "intermediateID is empty"));

            Guid guid;
            if (!Guid.TryParse(intermediateID, out guid))
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "intermediateID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderIntermBatch));
            ProdOrderPartslistPos result = null;
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    result = ConvertToWSProdOrderIntermBatches(s_cQry_GetProdOrderIntermBatches(dbApp, null, guid)).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderIntermBatch) + "(10)", e);
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderIntermBatch));
            }
            return new WSResponse<ProdOrderPartslistPos>(result);
        }

        public WSResponse<ProdOrderBatch> GetProdOrderBatch(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return new WSResponse<ProdOrderBatch>(null, new Msg(eMsgLevel.Error, "barcodeID is empty"));
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<ProdOrderBatch>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderBatch));
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    Guid guid = Guid.Empty;

                    if (Guid.TryParse(barcodeID, out guid))
                    {
                        ProdOrderBatch poBatch = s_cQry_GetProdOrderBatch(dbApp, guid);
                        return new WSResponse<ProdOrderBatch>(poBatch);
                    }

                    return new WSResponse<ProdOrderBatch>(null, new Msg(eMsgLevel.Error, "Coudn't resolve barcodeID"));
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderBatch) + "(10)", e);
                    return new WSResponse<ProdOrderBatch>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderBatch));
                }
            }
        }

        public static readonly Func<DatabaseApp, Guid, ProdOrderBatch> s_cQry_GetProdOrderBatch =
        CompiledQuery.Compile<DatabaseApp, Guid, ProdOrderBatch>(
            (dbApp, batchID) =>
                dbApp.ProdOrderBatch
                .Where(c => c.ProdOrderBatchID == batchID)
                .Select(c => new gip.mes.webservices.ProdOrderBatch()
                {
                    ProdOrderBatchID = c.ProdOrderBatchID,
                    ProdOrderBatchNo = c.ProdOrderBatchNo,
                    BatchSeqNo = c.BatchSeqNo
                }
                
            ).FirstOrDefault()
        );

        #endregion

        #region Booking

        public static readonly Func<DatabaseApp, Guid?, IQueryable<gip.mes.datamodel.FacilityBookingCharge>> s_cQry_GetProdOrderFacilityBookings =
        CompiledQuery.Compile<DatabaseApp, Guid?, IQueryable<gip.mes.datamodel.FacilityBookingCharge>>(
            (dbApp, posRelationID) =>
                    dbApp.FacilityBookingCharge.Include("OutwardMaterial")
                                               .Include("InwardMaterial")
                                               .Include("OutwardFacility")
                                               .Include("InwardFacility")
                                               .Include("OutwardFacilityCharge")
                                               .Include("InwardFacilityCharge")
                                               .Include("FacilityBooking")
                                               .Include("FacilityBooking.ProdOrderPartslistPosRelation")
                                               .Include("FacilityBooking.InwardMaterial")
                                               .Include("FacilityBooking.OutwardMaterial")
                                               .Include("FacilityBooking.MDMovementReason")
                                               .Include("FacilityBooking.InwardFacilityCharge")
                                               .Include("FacilityBooking.OutwardFacilityCharge")
                                               .Include("ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ProdOrderPartslist.ProdOrder")
                                               .Where(c => c.FacilityBooking.ProdOrderPartslistPosRelationID == posRelationID)
        );


        public WSResponse<PostingOverview> GetProdOrderPosRelFacilityBooking(string POPLPosRelID)
        {
            Guid poPLPosRelID;

            if (!Guid.TryParse(POPLPosRelID, out poPLPosRelID))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "The given parameter POPLPosRelID is incorrect!"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderPosRelFacilityBooking));
            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var overview = facManager.ConvertGroupedBookingsToOverviewDictionary(s_cQry_GetProdOrderFacilityBookings(dbApp, poPLPosRelID)
                                             .GroupBy(x => x.FacilityBooking.FacilityBookingNo));
                    if (overview != null)
                        return new WSResponse<PostingOverview>(new PostingOverview() { Postings = overview.Keys.ToList(), PostingsFBC = overview.SelectMany(c => c.Value).ToList() });
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderPosRelFacilityBooking) + "(10)", e);
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderPosRelFacilityBooking));
            }

            return new WSResponse<PostingOverview>();
        }

        public static readonly Func<DatabaseApp, Guid?, IQueryable<gip.mes.datamodel.FacilityBookingCharge>> s_cQry_GetProdOrderPosFacilityBookings =
        CompiledQuery.Compile<DatabaseApp, Guid?, IQueryable<gip.mes.datamodel.FacilityBookingCharge>>(
            (dbApp, posID) =>
                    dbApp.FacilityBookingCharge.Include("OutwardMaterial")
                                               .Include("InwardMaterial")
                                               .Include("OutwardFacility")
                                               .Include("InwardFacility")
                                               .Include("OutwardFacilityCharge")
                                               .Include("InwardFacilityCharge")
                                               .Include("FacilityBooking")
                                               .Include("FacilityBooking.ProdOrderPartslistPosRelation")
                                               .Include("FacilityBooking.InwardMaterial")
                                               .Include("FacilityBooking.OutwardMaterial")
                                               .Include("FacilityBooking.MDMovementReason")
                                               .Include("FacilityBooking.InwardFacilityCharge")
                                               .Include("FacilityBooking.OutwardFacilityCharge")
                                               .Include("ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder")
                                               .Where(c => c.FacilityBooking.ProdOrderPartslistPosID == posID)
        );


        public WSResponse<PostingOverview> GetProdOrderPosFacilityBooking(string POPLPosID)
        {
            Guid poPLPosID;

            if (!Guid.TryParse(POPLPosID, out poPLPosID))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "The given parameter POPLPosRelID is incorrect!"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderPosFacilityBooking));
            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var overview = facManager.ConvertGroupedBookingsToOverviewDictionary(s_cQry_GetProdOrderPosFacilityBookings(dbApp, poPLPosID)
                                             .GroupBy(x => x.FacilityBooking.FacilityBookingNo));
                    if (overview != null)
                        return new WSResponse<PostingOverview>(new PostingOverview() { Postings = overview.Keys.ToList(), PostingsFBC = overview.SelectMany(c => c.Value).ToList() });
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderPosFacilityBooking) + "(10)", e);
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderPosFacilityBooking));
            }

            return new WSResponse<PostingOverview>();
        }

        #endregion

        #region Machine/Function

        public static readonly Func<DatabaseApp, Guid, Guid, datamodel.ProdOrderPartslistPos> s_cQry_GetProdOrderIntermOrIntermBatchByMachine =
            CompiledQuery.Compile<DatabaseApp, Guid, Guid, datamodel.ProdOrderPartslistPos>(
                (dbApp, posID, batchID) =>
                    dbApp.ProdOrderPartslistPos.Include("Material")
                                               .Include("Material.BaseMDUnit")
                                               .Include("ProdOrderBatch")
                                               .Include("MDUnit")
                                               .Include("ProdOrderPartslist")
                                               .Include("ProdOrderPartslist.Partslist")
                                               .Include("ProdOrderPartslist.Partslist.Material")
                                               .Include("ProdOrderPartslist.Partslist.Material.BaseMDUnit")
                                               .Include("ProdOrderPartslist.ProdOrder")
                                               .FirstOrDefault(c => c.ProdOrderPartslistPosID == posID
                                                                 && c.ProdOrderBatchID.HasValue
                                                                 && c.ProdOrderBatchID == batchID)
            );

        public WSResponse<ProdOrderPartslistPos> GetProdOrderIntermOrIntermBatchByMachine(string machineID)
        {
            Guid machineIdent;
            if (!Guid.TryParse(machineID, out machineIdent))
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "The given parameter machineID is incorrect!"));

            ProdOrderPartslistPos result = null;
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderIntermOrIntermBatchByMachine));

            try
            {
                using (Database db = new Database())
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    core.datamodel.ACClass machineClass = db.ACClass.FirstOrDefault(c => c.ACClassID == machineIdent);
                    if (machineClass == null)
                        return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "The machine of function is not exist with ID: " + machineID));

                    PAProcessFunction processFunction = db.Root()?.ACUrlCommand(machineClass.ACUrlComponent) as PAProcessFunction;
                    if (processFunction == null)
                        return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, string.Format("The function with ACUrl {0} is not exist!", machineClass.ACUrlComponent)));

                    if (processFunction.CurrentACState < ACStateEnum.SMStarting || processFunction.CurrentACState > ACStateEnum.SMRunning)
                        return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "The function is not in starting or running state!"));

                    PAProcessModuleVB processModule = processFunction.FindParentComponent<PAProcessModuleVB>(c => c is PAProcessModuleVB);
                    if (processModule == null)
                        return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "Can not find the process module of scanned function!"));

                    PAOrderInfo orderInfo = PAShowDlgManagerVBBase.QueryOrderInfo(processModule);
                    if (orderInfo == null)
                        return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "Order info on process module is null!"));

                    PAOrderInfoEntry entry = orderInfo.Entities.FirstOrDefault(c => c.EntityName == datamodel.ProdOrderBatch.ClassName);
                    if (entry == null)
                        return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "In the order info missing ProdOrderBatch entry!")); //TODO: if batch not exist

                    Guid batchID = entry.EntityID;


                    entry = orderInfo.Entities.FirstOrDefault(c => c.EntityName == datamodel.ProdOrderPartslistPos.ClassName);
                    if (entry == null)
                        return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "In the order info missing ProdOrderPartslistPos entry!"));

                    //datamodel.ProdOrderPartslistPos intermediate = dbApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistID == entry.EntityID);

                    datamodel.ProdOrderPartslistPos intermBatch = s_cQry_GetProdOrderIntermOrIntermBatchByMachine(dbApp, entry.EntityID, batchID);

                    if (intermBatch == null)
                        return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "Can not navigate to target intermediate or batch!"));

                    result = ConvertToWSProdOrderPartslistPos(intermBatch);
                }

            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderIntermOrIntermBatchByMachine) + "(10)", e);
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderIntermOrIntermBatchByMachine));
            }

            return new WSResponse<ProdOrderPartslistPos>(result);
        }

        #endregion

        #region TargetFacilities

        //static readonly Func<DatabaseApp, Guid?, IQueryable<datamodel.Facility>> s_cQry_GetBatchTargetFacilities =
        //                CompiledQuery.Compile<DatabaseApp, Guid?, IQueryable<datamodel.Facility>>(
        //                    (dbApp, intermBatchID) =>
        //                        dbApp.Facility
        //                );

        public WSResponse<List<Facility>> GetPOBatchTargetFacilities(string intermBatchID)
        {
            Guid batchID;
            if (!Guid.TryParse(intermBatchID, out batchID))
                return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "The given parameter intermBatchID is incorrect!"));

            List<Facility> result = new List<Facility>();
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetPOBatchTargetFacilities));

            try
            {
                using (Database db = new Database())
                using (DatabaseApp dbApp = new DatabaseApp(db))
                {
                    var batchPos = dbApp.ProdOrderPartslistPos.Include("ProdOrderBatch")
                                                              .Include("ProdOrderBatch.ProdOrderBatchPlan")
                                                              .Include("ProdOrderBatch.ProdOrderBatchPlan")
                                                              .Include("ProdOrderBatch.ProdOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan")
                                                              .FirstOrDefault(c => c.ProdOrderPartslistPosID == batchID);

                    result = batchPos?.ProdOrderBatch?.ProdOrderBatchPlan?
                                                      .FacilityReservation_ProdOrderBatchPlan
                                                      .Where(x => x.FacilityID.HasValue)
                                                      .Select(c => new Facility()
                                                      {
                                                          FacilityID = c.Facility.FacilityID,
                                                          FacilityNo = c.Facility.FacilityNo,
                                                          FacilityName = c.Facility.FacilityName,
                                                          ParentFacilityID = c.Facility.ParentFacilityID,
                                                          SkipPrintQuestion = c.Facility.SkipPrintQuestion
                                                      })
                                                      .ToList();
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetPOBatchTargetFacilities) + "(10)", e);
                return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetPOBatchTargetFacilities));
            }
            return new WSResponse<List<Facility>>(result);
        }

        public WSResponse<Facility> GetNFBatchTargetFacility(string machineFunctionID)
        {
            WSResponse<Facility> result = new WSResponse<Facility>();

            Guid idMachineFunction;
            if (!Guid.TryParse(machineFunctionID, out idMachineFunction))
            {
                result.Message = new Msg(eMsgLevel.Error, "The given parameter machineID is incorrect!");
            }
            else
            {
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
                if (myServiceHost == null)
                    return new WSResponse<Facility>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetNFBatchTargetFacility));
                try
                {
                    using (Database db = new Database())
                    using (DatabaseApp dbApp = new DatabaseApp(db))
                    {
                        ACRoutingService routingService = ACRoutingService.GetServiceInstance(myServiceHost);

                        core.datamodel.ACClass machineFunction = db.ACClass.Where(c => c.ACClassID == idMachineFunction).FirstOrDefault();
                        if (machineFunction == null)
                        {
                            result.Message = new Msg(eMsgLevel.Error, $"Not finded machine with machineID={machineFunction}!");
                        }
                        else
                        {
                            Facility facility = null;

                            ACRoutingParameters routingParameters = new ACRoutingParameters()
                            {
                                RoutingService = routingService,
                                Database = db,
                                AttachRouteItemsToContext = false,
                                SelectionRuleID = PAMParkingspace.SelRuleID_ParkingSpace,
                                Direction = RouteDirections.Forwards,
                                MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                                IncludeReserved = true,
                                IncludeAllocated = true
                            };

                            RoutingResult rResult = ACRoutingService.FindSuccessors(machineFunction.ACClass1_ParentACClass, routingParameters);
                            if (rResult != null && rResult.Routes != null && rResult.Routes.Any())
                            {
                                Route route = rResult.Routes.FirstOrDefault();
                                RouteItem routeItem = route.GetRouteTarget();
                                core.datamodel.ACClass facilityClass = routeItem.Target;
                                mes.datamodel.Facility dbFacility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == facilityClass.ACClassID).FirstOrDefault();
                                facility = new Facility()
                                {
                                    FacilityID = dbFacility.FacilityID,
                                    FacilityName = dbFacility.FacilityName,
                                    FacilityNo = dbFacility.FacilityNo,
                                    ParentFacilityID = dbFacility.ParentFacilityID
                                };
                            }

                            if (facility == null)
                            {
                                result.Message = new Msg(eMsgLevel.Error, "Unable to find facility connected with machine!");
                            }
                            else
                            {
                                result.Data = facility;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetNFBatchTargetFacility) + "(10)", e);
                    result.Message = new Msg(eMsgLevel.Exception, e.Message);
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetNFBatchTargetFacility));
                }
            }

            return result;
        }

        public WSResponse<List<FacilityCharge>> GetPOAvailableFC(string machineFunctionID, string POPLPosRelID)
        {
            WSResponse<List<FacilityCharge>> result = new WSResponse<List<FacilityCharge>>();

            Guid workFunctionID, relationID;
            if (!Guid.TryParse(machineFunctionID, out workFunctionID))
            {
                result.Message = new Msg(eMsgLevel.Error, "The given parameter machineID is incorrect!");
                return result;
            }

            if (!Guid.TryParse(POPLPosRelID, out relationID))
            {
                result.Message = new Msg(eMsgLevel.Error, "The given parameter POPLPosRelID is incorrect!");
                return result;
            }

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetPOAvailableFC));
            try
            {
                using (Database db = new Database())
                using (DatabaseApp dbApp = new DatabaseApp(db))
                {
                    core.datamodel.ACClass machineFunction = db.ACClass.Where(c => c.ACClassID == workFunctionID).FirstOrDefault();
                    if (machineFunction == null)
                    {
                        result.Message = new Msg(eMsgLevel.Error, "The ACClass for machineFunctionID is not available!");
                        return result;
                    }

                    core.datamodel.ACClass processModule = machineFunction.ACClass1_ParentACClass;
                    if (processModule == null || processModule.ACKindIndex != (short)Global.ACKinds.TPAProcessModule)
                    {
                        result.Message = new Msg(eMsgLevel.Error, "The process module of scanned function is not available!");
                        return result;
                    }

                    datamodel.ProdOrderPartslistPosRelation relation = dbApp.ProdOrderPartslistPosRelation.Where(c => c.ProdOrderPartslistPosRelationID == relationID).FirstOrDefault();
                    if (relation == null)
                    {
                        result.Message = new Msg(eMsgLevel.Error, "The relation for POPLPosRelID is not available!");
                        return result;
                    }

                    ACPartslistManager partslistManager = ACPartslistManager.GetServiceInstance(myServiceHost);

                    facility.ACPartslistManager.QrySilosResult possibleSilos;
                    facility.ACPartslistManager.QrySilosResult allSilos;

                    IEnumerable<Route> routes = partslistManager.GetRoutes(relation, dbApp, db, processModule, ACPartslistManager.SearchMode.AllSilos, null, out possibleSilos, out allSilos, null, null, null, false);
                    if (routes != null && routes.Any())
                    {
                        datamodel.FacilityCharge[] resultList = possibleSilos.SortedFilteredResult.SelectMany(c => c.FacilityCharges.Select(x => x.Quant)).ToArray();
                        result.Data = ConvertFacilityChargeList(resultList).ToList();
                    }
                    else
                    {
                        //Error50701: Quants for this position are not available!
                        result.Message = new Msg(partslistManager, eMsgLevel.Error, nameof(VBWebService), nameof(GetPOAvailableFC), 1035, "Error50701");
                    }
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetPOAvailableFC) + "(10)", e);
                result.Message = new Msg(eMsgLevel.Exception, e.Message);
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetPOAvailableFC));
            }
            

            return result;
        }


        #endregion

        #region Misc.

        public WSResponse<ProdOrderPartslistPos> GetProdOrderPartslistPos(string POPLPosID)
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderPartslistPos));
            ProdOrderPartslistPos pos = null;
            try
            {
                Guid posID;
                if (!Guid.TryParse(POPLPosID, out posID))
                    return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Error, "The given parameter is incorrect!"));

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var vbPos = dbApp.ProdOrderPartslistPos.Include("Material")
                                                           .Include("Material.BaseMDUnit")
                                                           .Include("ProdOrderBatch")
                                                           .Include("MDUnit")
                                                           .Include("ProdOrderPartslist")
                                                           .Include("ProdOrderPartslist.Partslist")
                                                           .Include("ProdOrderPartslist.Partslist.Material")
                                                           .Include("ProdOrderPartslist.Partslist.Material.BaseMDUnit")
                                                           .Include("ProdOrderPartslist.ProdOrder")
                                                           .FirstOrDefault(c => c.ProdOrderPartslistPosID == posID);
                    pos = ConvertToWSProdOrderPartslistPos(vbPos);
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderPartslistPos) + "(10)", e);
                return new WSResponse<ProdOrderPartslistPos>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderPartslistPos));
            }
            return new WSResponse<ProdOrderPartslistPos>(pos);
        }

        internal ProdOrderPartslistPos ConvertToWSProdOrderPartslistPos(datamodel.ProdOrderPartslistPos plPos)
        {
            return new ProdOrderPartslistPos()
            {
                ProdOrderPartslistPosID = plPos.ProdOrderPartslistPosID,
                Sequence = plPos.Sequence,
                ActualQuantity = plPos.ActualQuantity,
                ActualQuantityUOM = plPos.ActualQuantityUOM,
                TargetQuantity = plPos.TargetQuantity,
                TargetQuantityUOM = plPos.TargetQuantityUOM,
                BookingMaterialID = plPos.BookingMaterial?.MaterialID,
                BookingMaterial = plPos.BookingMaterial != null ? new Material()
                {
                    MaterialID = plPos.BookingMaterial.MaterialID,
                    MaterialNo = plPos.BookingMaterial.MaterialNo,
                    MaterialName1 = plPos.BookingMaterial.MaterialName1,
                    BaseMDUnit = new MDUnit()
                    {
                        MDUnitID = plPos.BookingMaterial.BaseMDUnit.MDUnitID,
                        MDUnitNameTrans = plPos.BookingMaterial.BaseMDUnit.MDUnitNameTrans
                    }
                } : null,
                FacilityLotID = plPos.FacilityLotID,
                Material = new Material()
                {
                    MaterialID = plPos.Material.MaterialID,
                    MaterialNo = plPos.Material.MaterialNo,
                    MaterialName1 = plPos.Material.MaterialName1,
                    BaseMDUnit = new MDUnit()
                    {
                        MDUnitID = plPos.Material.BaseMDUnit.MDUnitID,
                        MDUnitNameTrans = plPos.Material.BaseMDUnit.MDUnitNameTrans
                    }
                },
                ProdOrderBatch = new ProdOrderBatch()
                {
                    ProdOrderBatchID = plPos.ProdOrderBatchID.Value,
                    ProdOrderBatchNo = plPos.ProdOrderBatch.ProdOrderBatchNo,
                    BatchSeqNo = plPos.ProdOrderBatch.BatchSeqNo
                },
                MDUnit = plPos.MDUnit != null ? new MDUnit()
                {
                    MDUnitID = plPos.MDUnit.MDUnitID,
                    MDUnitNameTrans = plPos.MDUnit.MDUnitNameTrans
                } : null,
                ProdOrderPartslist = new ProdOrderPartslist()
                {
                    ProdOrderPartslistID = plPos.ProdOrderPartslistID,
                    ActualQuantity = plPos.ProdOrderPartslist.ActualQuantity,
                    TargetQuantity = plPos.ProdOrderPartslist.TargetQuantity,
                    Partslist = new Partslist()
                    {
                        PartlistID = plPos.ProdOrderPartslist.PartslistID.Value,
                        PartslistNo = plPos.ProdOrderPartslist.Partslist?.PartslistNo,
                        PartslistName = plPos.ProdOrderPartslist.Partslist?.PartslistName,
                        PartslistVersion = plPos.ProdOrderPartslist.Partslist?.PartslistVersion,
                        Material = new Material()
                        {
                            MaterialID = plPos.ProdOrderPartslist.Partslist.MaterialID,
                            MaterialNo = plPos.ProdOrderPartslist.Partslist.Material.MaterialNo,
                            MaterialName1 = plPos.ProdOrderPartslist.Partslist.Material.MaterialName1,
                            BaseMDUnit = new MDUnit()
                            {
                                MDUnitID = plPos.ProdOrderPartslist.Partslist.Material.BaseMDUnit.MDUnitID,
                                MDUnitNameTrans = plPos.ProdOrderPartslist.Partslist.Material.BaseMDUnit.MDUnitNameTrans,
                                SymbolTrans = plPos.ProdOrderPartslist.Partslist.Material.BaseMDUnit.SymbolTrans
                            }
                        }
                    },
                    ProdOrder = new ProdOrder()
                    {
                        ProdOrderID = plPos.ProdOrderPartslist.ProdOrderID,
                        ProgramNo = plPos.ProdOrderPartslist.ProdOrder.ProgramNo
                    }
                }
            };
        }

        public WSResponse<ProdOrderPartslistPosRelation> GetProdOrderPartslistPosRel(string POPLPosRelID)
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<ProdOrderPartslistPosRelation>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetProdOrderPartslistPosRel));
            ProdOrderPartslistPosRelation posRel = null;
            try
            {
                Guid posRelID;
                if (!Guid.TryParse(POPLPosRelID, out posRelID))
                    return new WSResponse<ProdOrderPartslistPosRelation>(null, new Msg(eMsgLevel.Error, "The given parameter is incorrect!"));

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var vbPos = dbApp.ProdOrderPartslistPosRelation.Include("SourceProdOrderPartslistPos")
                                                                   .Include("SourceProdOrderPartslistPos.Material")
                                                                   .Include("SourceProdOrderPartslistPos.Material.BaseMDUnit")
                                                                   .Include("SourceProdOrderPartslistPos.ProdOrderPartslist")
                                                                   .Include("SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist")
                                                                   .Include("SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material")
                                                                   .Include("SourceProdOrderPartslistPos.MDUnit")
                                                                   .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == posRelID);

                    posRel = ConvertToWSProdOrderPartslistPosRel(vbPos);
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetProdOrderPartslistPosRel) + "(10)", e);
                return new WSResponse<ProdOrderPartslistPosRelation>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetProdOrderPartslistPosRel));
            }
            return new WSResponse<ProdOrderPartslistPosRelation>(posRel);
        }

        internal ProdOrderPartslistPosRelation ConvertToWSProdOrderPartslistPosRel(datamodel.ProdOrderPartslistPosRelation plPosRel)
        {
            return new ProdOrderPartslistPosRelation()
            {
                ProdOrderPartslistPosRelationID = plPosRel.ProdOrderPartslistPosRelationID,
                Sequence = plPosRel.Sequence,
                ActualQuantity = plPosRel.ActualQuantity,
                ActualQuantityUOM = plPosRel.ActualQuantityUOM,
                TargetQuantity = plPosRel.TargetQuantity,
                TargetQuantityUOM = plPosRel.TargetQuantityUOM,
                SourcePos = new ProdOrderPartslistPos()
                {
                    ProdOrderPartslistPosID = plPosRel.SourceProdOrderPartslistPosID,
                    Material = new Material()
                    {
                        MaterialID = plPosRel.SourceProdOrderPartslistPos.Material.MaterialID,
                        MaterialNo = plPosRel.SourceProdOrderPartslistPos.Material.MaterialNo,
                        MaterialName1 = plPosRel.SourceProdOrderPartslistPos.Material.MaterialName1,
                        BaseMDUnit = new MDUnit()
                        {
                            MDUnitID = plPosRel.SourceProdOrderPartslistPos.Material.BaseMDUnit.MDUnitID,
                            MDUnitNameTrans = plPosRel.SourceProdOrderPartslistPos.Material.BaseMDUnit.MDUnitNameTrans,
                            SymbolTrans = plPosRel.SourceProdOrderPartslistPos.Material.BaseMDUnit.SymbolTrans
                        }
                    },
                    ProdOrderPartslist = new ProdOrderPartslist()
                    {
                        ProdOrderPartslistID = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslistID,
                        ActualQuantity = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslist.ActualQuantity,
                        TargetQuantity = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslist.TargetQuantity,
                        Partslist = new Partslist()
                        {
                            PartlistID = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslist.PartslistID.Value,
                            PartslistNo = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist?.PartslistNo,
                            PartslistName = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist?.PartslistName,
                            PartslistVersion = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist?.PartslistVersion,
                            Material = new Material()
                            {
                                MaterialID = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.MaterialID,
                                MaterialNo = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialNo,
                                MaterialName1 = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialName1,
                                BaseMDUnit = new MDUnit()
                                {
                                    MDUnitID = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.BaseMDUnit.MDUnitID,
                                    MDUnitNameTrans = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.BaseMDUnit.MDUnitNameTrans,
                                    SymbolTrans = plPosRel.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.BaseMDUnit.SymbolTrans
                                }
                            }
                        }
                    },
                    MDUnit = plPosRel.SourceProdOrderPartslistPos.MDUnit != null ? new MDUnit()
                    {
                        MDUnitID = plPosRel.SourceProdOrderPartslistPos.MDUnit.MDUnitID,
                        MDUnitNameTrans = plPosRel.SourceProdOrderPartslistPos.MDUnit.MDUnitNameTrans,
                        SymbolTrans = plPosRel.SourceProdOrderPartslistPos.MDUnit.SymbolTrans
                    } : null
                },
                TargetPos = new ProdOrderPartslistPos()
                {
                    ProdOrderPartslistPosID = plPosRel.TargetProdOrderPartslistPosID
                }
            };
        }

        public WSResponse<Msg> VerifyOrderPostingsOnRelease(BarcodeEntity entity)
        {
            if (entity == null)
                return new WSResponse<Msg>(null, new Msg(eMsgLevel.Error, "entity is null."));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<Msg>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PWWorkTaskGeneric pwWorkTaskGeneric = myServiceHost.ACUrlCommand(entity.SelectedOrderWF.ACUrlWF) as PWWorkTaskGeneric;
            if (pwWorkTaskGeneric != null)
            {
                return new WSResponse<Msg>(pwWorkTaskGeneric.VerifyOrderPostingsOnRelease(), null);
            }

            return null;
        }

        #endregion
    }
}
