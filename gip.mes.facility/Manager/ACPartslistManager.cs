// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.datamodel.EntityMaterial;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Partslistmanager'}de{'Stücklistenverwaltung'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public partial class ACPartslistManager : PARole
    {
        #region c´tors
        public ACPartslistManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _FindSiloModes = new ACPropertyConfigValue<short>(this, "FindSiloModes", 0);
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACRoutingService.DetachACRefFromServiceInstance(this, _RoutingService);
            _RoutingService = null;

            bool result = base.ACDeInit(deleteACClassTask);
            return result;
        }

        public override bool ACPostInit()
        {
            bool init = base.ACPostInit();
            _RoutingService = ACRoutingService.ACRefToServiceInstance(this);
            return init;
        }

        public const string C_DefaultServiceACIdentifier = "PartslistManager";
        #endregion

        #region const
        public static string[] S_Partslist_Change_Fields = new string[] { nameof(Partslist.IsEnabled), nameof(Partslist.MaterialID), nameof(Partslist.MaterialWFID), nameof(Partslist.MDUnitID), nameof(Partslist.TargetQuantityUOM) };
        public static string[] S_PartslistPosRelation_Change_Fields = new string[] { nameof(PartslistPos.MaterialPosTypeIndex), nameof(PartslistPos.MaterialID), nameof(PartslistPos.TargetQuantityUOM), nameof(PartslistPos.MDUnitID), nameof(PartslistPosRelation.SourcePartslistPosID) };
        #endregion

        #region Attach / Deattach
        public static ACPartslistManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACPartslistManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ACPartslistManager> ACRefToServiceInstance(ACComponent requester)
        {
            ACPartslistManager serviceInstance = GetServiceInstance(requester) as ACPartslistManager;
            if (serviceInstance != null)
                return new ACRef<ACPartslistManager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region Managers

        #endregion

        #region PrecompiledQueries


        static readonly Func<DatabaseApp, Guid, IEnumerable<PartslistPos>> s_cQry_AllPositions =
            EF.CompileQuery<DatabaseApp, Guid, IEnumerable<PartslistPos>>(
                (db, partslistID) =>
                    db.PartslistPos.Where(x => x.PartslistID == partslistID).OrderBy(x => x.Sequence)

            );

        /// <summary>
        /// Outward root position - Components
        /// </summary>
        static readonly Func<DatabaseApp, Guid, IEnumerable<PartslistPos>> s_cQry_PosComponents =
            EF.CompileQuery<DatabaseApp, Guid, IEnumerable<PartslistPos>>(
                (db, partslistID) => db.PartslistPos.Where(x =>
                            x.PartslistID == partslistID &&
                            x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.OutwardRoot &&
                            x.AlternativePartslistPosID == null)
                    .OrderBy(x => x.Sequence)

            );

        /// <summary>
        /// Outward root positions - Alternative for Component positions
        /// </summary>
        static readonly Func<DatabaseApp, Guid, IEnumerable<PartslistPos>> s_cQry_PosAlternative =
            EF.CompileQuery<DatabaseApp, Guid, IEnumerable<PartslistPos>>(
                (db, partslistPosID) => db.PartslistPos.Where(x => x.AlternativePartslistPosID == partslistPosID)
                    .OrderBy(x => x.Sequence)

            );


        /// <summary>
        /// Inward elements - Intermediate components
        /// </summary>
        public static readonly Func<DatabaseApp, Guid, IEnumerable<PartslistPos>> s_cQry_PosIntermediate =
            EF.CompileQuery<DatabaseApp, Guid, IEnumerable<PartslistPos>>(
                (db, partslistID) => db.PartslistPos.Where(x =>
                            x.PartslistID == partslistID &&
                            x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.InwardIntern)
                            .OrderBy(x => x.Sequence)
                );

        /// <summary>
        /// Outward root position - Components
        /// </summary>
        static readonly Func<DatabaseApp, Guid, IEnumerable<PartslistPosRelation>> s_cQry_PosIntermediateComponents =
            EF.CompileQuery<DatabaseApp, Guid, IEnumerable<PartslistPosRelation>>(
                (db, partslistPosID) => db.PartslistPosRelation.Where(x => x.TargetPartslistPosID == partslistPosID).OrderBy(x => x.Sequence)
            );

        #endregion

        #region Queries (union with added elements)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="partslistID"></param>
        /// <returns></returns>
        public static IEnumerable<PartslistPos> QueryAllPositions(DatabaseApp dbApp, Guid partslistID)
        {
            return
            s_cQry_AllPositions(dbApp, partslistID)
            .Union(dbApp.GetAddedEntities<PartslistPos>(x => x.PartslistID == partslistID).OrderBy(x => x.Sequence));
        }

        public static IEnumerable<PartslistPos> QueryPosComponents(DatabaseApp dbApp, Guid partslistID)
        {
            return s_cQry_PosComponents(dbApp, partslistID)
                .Union(dbApp.GetAddedEntities<PartslistPos>(x => x.PartslistID == partslistID
                                                            && x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.OutwardRoot
                                                            && x.AlternativePartslistPosID == null)
                            .OrderBy(x => x.Sequence));
        }


        public static IEnumerable<PartslistPos> QueryPosAlternative(DatabaseApp dbApp, Guid partslistPosID)
        {
            return s_cQry_PosAlternative(dbApp, partslistPosID)
                .Union(dbApp.GetAddedEntities<PartslistPos>(x => x.AlternativePartslistPosID == partslistPosID).OrderBy(x => x.Sequence));
        }


        public static IEnumerable<PartslistPosRelation> QueryPosIntermediateComponents(DatabaseApp dbApp, Guid partslistPosID)
        {
            return s_cQry_PosIntermediateComponents(dbApp, partslistPosID)
                .Union(dbApp.GetAddedEntities<PartslistPosRelation>(x => x.TargetPartslistPosID == partslistPosID).OrderBy(x => x.Sequence));
        }

        #endregion

        #region Assing WF
        /// <summary>
        /// Clone structure from MaterialWF to PartslistPos
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="materialWFID"></param>
        /// <param name="partslistID"></param>
        /// <returns></returns>
        public Msg AssignMaterialWF(DatabaseApp dbApp, MaterialWF materialWF, Partslist partsList)
        {
            Msg msg = null;
            if (!PreExecute("AssignMaterialWF"))
            {
                //"Error: Method is in progress!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "AssignMaterialWF(0)",
                    Message = Root.Environment.TranslateMessage(this, "Info50011")
                };
            }

            if (materialWF == null)
            {
                // Error: Material WF is not defined!
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "AssignMaterialWF(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50024")
                };
            }

            if (partsList == null)
            {
                // Error: Partslist is not defined
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "AssignMaterialWF(2)",
                    Message = Root.Environment.TranslateMessage(this, "Error50025")
                };
            }

            partsList.MaterialWF = materialWF;

            List<PartslistPos> positions = new List<PartslistPos>();
            List<PartslistPosRelation> posRelations = new List<PartslistPosRelation>();

            List<Material> materials = materialWF.GetMaterials().ToList();
            List<MaterialWFRelation> relations = materialWF.MaterialWFRelation_MaterialWF.ToList();

            if (relations.Any(x => x.TargetMaterialID == x.SourceMaterialID && materialWF.MaterialWFRelation_MaterialWF.Count > 1))
            {
                // Error: Self referencing material position relation exist!
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "AssignMaterialWF(3)",
                    Message = Root.Environment.TranslateMessage(this, "Error50026")
                };
            }

            foreach (var item in materials)
            {
                PartslistPos posItem = PartslistPos.NewACObject(dbApp, null);
                posItem.Partslist = partsList;
                //posItem.Sequence = MaterialWF.CalculateMixingLevel(item, materialWF.GetMaterials(), materialWF.MaterialWFRelation_MaterialWF);
                posItem.Sequence = 0;
                posItem.SequenceProduction = 0;
                posItem.MaterialPosTypeIndex = (short)GlobalApp.MaterialPosTypes.InwardIntern;
                posItem.Material = item;
                posItem.MDUnit = item.BaseMDUnit;
                posItem.LineNumber = "";
                positions.Add(posItem);
                partsList.PartslistPos_Partslist.Add(posItem);
            }

            CalculateMixingLevels(relations, positions);

            foreach (var item in relations)
            {
                if (item.SourceMaterialID == item.TargetMaterialID)
                    continue;

                PartslistPosRelation posRelation = PartslistPosRelation.NewACObject(dbApp, null);
                posRelation.TargetPartslistPos = positions.FirstOrDefault(x => x.MaterialID == item.TargetMaterialID);
                posRelation.MaterialWFRelationID = item.MaterialWFRelationID;
                posRelation.SourcePartslistPos = positions.FirstOrDefault(x => x.MaterialID == item.SourceMaterialID);
                posRelations.Add(posRelation);
            }

            foreach (var item in positions)
            {
                List<PartslistPosRelation> targets = posRelations.Where(x => x.TargetPartslistPosID == item.PartslistPosID).ToList();
                List<PartslistPosRelation> source = posRelations.Where(x => x.SourcePartslistPosID == item.PartslistPosID).ToList();
                targets.ForEach(x => item.PartslistPosRelation_TargetPartslistPos.Add(x));
                source.ForEach(x => item.PartslistPosRelation_SourcePartslistPos.Add(x));
            }

            foreach (var pos in positions)
            {
                int i = 0;
                if (pos.PartslistPosRelation_TargetPartslistPos != null)
                {
                    foreach (var sourceItem in pos.PartslistPosRelation_TargetPartslistPos)
                    {
                        sourceItem.Sequence = ++i;
                    }
                }
            }

            PartslistPos lastIntermediate = partsList.PartslistPos_Partslist.OrderByDescending(x => x.Sequence).FirstOrDefault();
            if (lastIntermediate.Material.BaseMDUnit == partsList.Material.BaseMDUnit)
                lastIntermediate.TargetQuantityUOM = partsList.TargetQuantityUOM;
            else
            {
                try
                {
                    lastIntermediate.TargetQuantityUOM = lastIntermediate.Material.ConvertToBaseQuantity(partsList.TargetQuantityUOM, partsList.Material.BaseMDUnit);
                }
                catch (Exception e)
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "AssignMaterialWF(4)",
                        Message = e.Message
                        //Message = Root.Environment.TranslateMessage(this, "Error50024")
                    };
                }
            }
            PostExecute("AssignMaterialWF");
            return msg;
        }

        public Msg UnAssignMaterialWF(DatabaseApp db, Partslist partsList)
        {
            if (partsList == null || db == null)
            {
                // Error: Partslist is not defined
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "UnAssignMaterialWF(2)",
                    Message = Root.Environment.TranslateMessage(this, "Error50025")
                };
            }

            partsList.MaterialWF = null;
            List<PartslistPos> intermediateItems = db.PartslistPos.Where(x =>
                               x.PartslistID == partsList.PartslistID
                            && x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern).ToList();
            List<Guid> Ids = intermediateItems.Select(x => x.PartslistPosID).ToList();
            List<PartslistPosRelation> relations = db.PartslistPosRelation.Where(x => Ids.Contains(x.TargetPartslistPosID) || Ids.Contains(x.SourcePartslistPosID)).ToList();
            relations.ForEach(x => x.DeleteACObject(db, false));
            intermediateItems.ForEach(x => x.DeleteACObject(db, false));

            return null;
        }

        public Msg UpdatePartslistFromMaterialWF(DatabaseApp dbApp, Partslist partsList)
        {
            if (partsList == null)
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "AssignMaterialWF(2)",
                    Message = Root.Environment.TranslateMessage(this, "Error50025")
                };
            }

            if (partsList.MaterialWF == null)
            {
                // Error: Material WF is not defined!
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "AssignMaterialWF(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50024")
                };
            }

            List<PartslistPos> positions = partsList.PartslistPos_Partslist.Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern).ToList();
            List<PartslistPosRelation> posRelations = new List<PartslistPosRelation>();

            List<Material> materials = partsList.MaterialWF.GetMaterials().ToList();
            List<MaterialWFRelation> relations = partsList.MaterialWF.MaterialWFRelation_MaterialWF.ToList();

            IEnumerable<Material> newMaterials = materials.Except(partsList.PartslistPos_Partslist.Select(x => x.Material));

            foreach (Material newMaterial in newMaterials)
            {
                PartslistPos posItem = PartslistPos.NewACObject(dbApp, null);
                posItem.Partslist = partsList;
                //posItem.Sequence = MaterialWF.CalculateMixingLevel(newMaterial, materials, partsList.MaterialWF.MaterialWFRelation_MaterialWF);
                posItem.Sequence = 0;
                posItem.SequenceProduction = 0;
                posItem.MaterialPosTypeIndex = (short)GlobalApp.MaterialPosTypes.InwardIntern;
                posItem.Material = newMaterial;
                posItem.MDUnit = newMaterial.BaseMDUnit;
                posItem.LineNumber = "";
                positions.Add(posItem);
                partsList.PartslistPos_Partslist.Add(posItem);
            }

            CalculateMixingLevels(relations, positions);

            List<MaterialWFRelation> existingRelations = dbApp.PartslistPosRelation.Where(c => (c.SourcePartslistPos != null && c.SourcePartslistPos.PartslistID == partsList.PartslistID) &&
                                                                                                 (c.TargetPartslistPos != null && c.TargetPartslistPos.PartslistID == partsList.PartslistID) &&
                                                                                                 c.MaterialWFRelationID.HasValue).Select(r => r.MaterialWFRelation)
                                                                                   .ToList();

            var newRelations = relations.Except(existingRelations);

            foreach (MaterialWFRelation newRelation in newRelations)
            {
                if (newRelation.SourceMaterialID == newRelation.TargetMaterialID)
                    continue;

                PartslistPosRelation posRelation = PartslistPosRelation.NewACObject(dbApp, null);
                posRelation.TargetPartslistPos = positions.FirstOrDefault(x => x.MaterialID == newRelation.TargetMaterialID);
                posRelation.MaterialWFRelationID = newRelation.MaterialWFRelationID;
                posRelation.SourcePartslistPos = positions.FirstOrDefault(x => x.MaterialID == newRelation.SourceMaterialID);
                posRelations.Add(posRelation);
            }

            if (!newRelations.Any())
                return null;

            foreach (var item in positions)
            {
                List<PartslistPosRelation> targets = posRelations.Where(x => x.TargetPartslistPosID == item.PartslistPosID).ToList();
                List<PartslistPosRelation> source = posRelations.Where(x => x.SourcePartslistPosID == item.PartslistPosID).ToList();
                targets.ForEach(x => item.PartslistPosRelation_TargetPartslistPos.Add(x));
                source.ForEach(x => item.PartslistPosRelation_SourcePartslistPos.Add(x));
            }

            PartslistPos lastIntermediate = partsList.PartslistPos_Partslist.OrderByDescending(x => x.Sequence).FirstOrDefault();
            if (lastIntermediate.Material.BaseMDUnit == partsList.Material.BaseMDUnit)
                lastIntermediate.TargetQuantityUOM = partsList.TargetQuantityUOM;
            else
            {
                try
                {
                    lastIntermediate.TargetQuantityUOM = lastIntermediate.Material.ConvertToBaseQuantity(partsList.TargetQuantityUOM, partsList.Material.BaseMDUnit);
                }
                catch (Exception e)
                {
                    return new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "AssignMaterialWF(4)",
                        Message = e.Message
                        //Message = Root.Environment.TranslateMessage(this, "Error50024")
                    };
                }
            }

            return null;
        }

        #endregion

        #region Calculation
        /// <summary>
        /// Recalculate UOM Quantity in Partslist tree
        /// </summary>
        /// <param name="db"></param>
        /// <param name="partslist"></param>
        /// <returns></returns>
        public MsgWithDetails CalculateUOMAndWeight(Partslist partslist)
        {
            MsgWithDetails msg = null;
            Material currentMaterial = null;
            try
            {
                if (partslist.Material != null)
                {
                    partslist.TargetQuantityUOM = MaterialHelper.ConvertToBaseQuantity(partslist.Material, partslist.TargetQuantity, partslist.MDUnit);
                }
                foreach (var postion in partslist.PartslistPos_Partslist)
                {
                    currentMaterial = postion.Material;
                    if (postion.Material != null)
                    {
                        postion.TargetQuantityUOM = MaterialHelper.ConvertToBaseQuantity(postion.Material, postion.TargetQuantity, postion.MDUnit);
                    }

                    foreach (var relation in postion.PartslistPosRelation_TargetPartslistPos)
                    {
                        if (relation.SourcePartslistPos.Material != null)
                        {
                            relation.TargetQuantityUOM = MaterialHelper.ConvertToBaseQuantity(relation.SourcePartslistPos.Material, relation.TargetQuantity, relation.SourcePartslistPos.MDUnit);
                        }
                    }
                }
            }
            catch (Exception ec)
            {
                msg = new MsgWithDetails();
                msg.MessageLevel = eMsgLevel.Error;
                Msg message = new Msg();
                message.MessageLevel = eMsgLevel.Error;
                string currentMaterialName = "";
                if (currentMaterial != null)
                    currentMaterialName = string.Format("{0} - {1}", currentMaterial.MaterialNo, currentMaterial.MaterialName1);
                message.Message = Root.Environment.TranslateMessage(this, "Error50043", currentMaterialName, ec.Message);
                message.ACIdentifier = this.ACIdentifier;
                msg.AddDetailMessage(message);
            }
            return msg;
        }

        /// <summary>
        /// Recalculate rest quantity
        /// </summary>
        /// <param name="partslist"></param>
        /// <returns></returns>
        public MsgWithDetails RecalcRemainingQuantity(Partslist partslist)
        {
            MsgWithDetails msg = null;
            var positions = partslist.PartslistPos_Partslist.Where(x => x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.OutwardRoot && x.AlternativePartslistPosID == null);
            foreach (var pos in positions)
            {
                pos.RestQuantity = pos.TargetQuantity;
                pos.RestQuantityUOM = pos.TargetQuantityUOM;
                foreach (var rel in pos.PartslistPosRelation_SourcePartslistPos)
                {
                    pos.RestQuantity -= rel.TargetQuantity;
                    pos.RestQuantityUOM -= rel.TargetQuantityUOM;
                }
            }
            return msg;
        }


        public MsgWithDetails RecalcIntermediateSum(Partslist partslist, bool setDefaultValueAtIncompatibleUnits = false)
        {
            MsgWithDetails msg = null;
            var lastIntermediate = partslist
                    .PartslistPos_Partslist
                    .Where(x => x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.InwardIntern
                            && !x.PartslistPosRelation_SourcePartslistPos.Any())
                    .FirstOrDefault();
            if (lastIntermediate != null)
                RecalcIntermediateItem(lastIntermediate, setDefaultValueAtIncompatibleUnits);
            return msg;
        }

        private void RecalcIntermediateItem(PartslistPos mixItem, bool setDefaultValueAtIncompatibleUnits)
        {
            var mixItemSources = mixItem.PartslistPosRelation_TargetPartslistPos.Where(x => x.SourcePartslistPos.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.InwardIntern).Select(x => x.SourcePartslistPos);
            foreach (PartslistPos childMixItem in mixItemSources)
            {
                RecalcIntermediateItem(childMixItem, setDefaultValueAtIncompatibleUnits);
            }

            if (mixItem.Material.ExcludeFromSumCalc)
            {
                mixItem.TargetQuantityUOM = 0;
                foreach (var rel in mixItem.PartslistPosRelation_SourcePartslistPos)
                {
                    rel.TargetQuantityUOM = 0;
                }
                return;
            }

            int convertableUnits = mixItem.PartslistPosRelation_TargetPartslistPos.Where(c => c.SourcePartslistPos.Material.IsBaseUnitConvertableToUnit(mixItem.Material.BaseMDUnit)).Count();
            if (convertableUnits > 0)
            {
                double mixSumUOM = 0;
                foreach (var relation in mixItem.PartslistPosRelation_TargetPartslistPos)
                {
                    if (relation.SourcePartslistPos.Material.IsBaseUnitConvertableToUnit(mixItem.Material.BaseMDUnit))
                    {
                        mixSumUOM += relation.SourcePartslistPos.Material.ConvertFromBaseQuantity(relation.TargetQuantityUOM, mixItem.Material.BaseMDUnit);
                    }
                }
                mixItem.TargetQuantityUOM = mixSumUOM;
                //mixItem.TargetQuantityUOM = mixItem.PartslistPosRelation_TargetPartslistPos.Sum(x => x.TargetQuantityUOM);

                // mixure distrubutes it's quantity to target mixures
                int countTargetRelations = mixItem.PartslistPosRelation_SourcePartslistPos.Count();
                if (countTargetRelations > 0)
                {
                    double quantityToSet = mixItem.TargetQuantityUOM / countTargetRelations;
                    foreach (var rel in mixItem.PartslistPosRelation_SourcePartslistPos)
                    {
                        // Set only if there ist only one relation or quantity was not overridden by the user
                        if (countTargetRelations == 1 || rel.TargetQuantityUOM <= 0.000001 || quantityToSet <= 0.000001)
                            rel.TargetQuantityUOM = quantityToSet;
                        //rel.TargetQuantity = rel.SourcePartslistPos.Material.ConvertQuantity(rel.TargetQuantityUOM,
                        //rel.SourcePartslistPos.Material.BaseMDUnit, rel.SourcePartslistPos.MDUnitID == null ? rel.SourcePartslistPos.Material.BaseMDUnit : mixItem.MDUnit);
                    }
                }
                //#if DEBUG
                //// @aagincic: code to checkin result
                //string debugForwardTo = string.Join(",", mixItem.PartslistPosRelation_SourcePartslistPos.Select(x => x.TargetPartslistPos.Material.MaterialNo));
                //System.Diagnostics.Debug.WriteLine(string.Format("{0} \t{1} (Q:{2}) D:{3}=>({4})",
                //    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), mixItem.Material.MaterialNo, mixItem.TargetQuantityUOM, quantityToSet, debugForwardTo));
                //#endif
            }
            else if (!mixItem.PartslistPosRelation_TargetPartslistPos.Any())
            {
                mixItem.TargetQuantityUOM = 0;
                foreach (var rel in mixItem.PartslistPosRelation_SourcePartslistPos)
                {
                    rel.TargetQuantityUOM = 0;
                }

            }
            else if (Math.Abs(mixItem.TargetQuantityUOM - 0) <= Double.Epsilon && setDefaultValueAtIncompatibleUnits)
            {
                mixItem.TargetQuantityUOM = 1;
            }
        }

        private void CalculateMixingLevels(List<MaterialWFRelation> relations, List<PartslistPos> positions)
        {
            Material rootMaterial = relations.FirstOrDefault(c => !relations.Any(x => x.SourceMaterialID == c.TargetMaterialID))?.TargetMaterial;
            if (rootMaterial == null && relations.Count == 1)
                rootMaterial = relations.FirstOrDefault()?.TargetMaterial;

            if (rootMaterial == null)
                return;

            int rootSeqNo = FindRootSeqNo(rootMaterial, relations) * 100;
            SetDepthRecursive(rootMaterial, positions, relations, rootSeqNo);
            SetLevels(positions, relations);
        }

        private int FindRootSeqNo(Material rootMaterial, List<MaterialWFRelation> relations)
        {
            int currentLevel = 0, resultLevel = 0;

            if (relations.Where(c => c.SourceMaterialID == c.TargetMaterialID).Count() == 1)
                resultLevel = 1;
            else
                FindRootSeqNoRecursive(rootMaterial, relations, currentLevel, ref resultLevel);

            return resultLevel;
        }

        private void FindRootSeqNoRecursive(Material material, List<MaterialWFRelation> relations, int currentLevel, ref int resultLevel)
        {
            int myLevel = currentLevel + 1;
            if (myLevel > resultLevel)
                resultLevel = myLevel;

            var sources = relations.Where(c => c.TargetMaterialID == material.MaterialID);

            foreach (var source in sources)
            {
                FindRootSeqNoRecursive(source.SourceMaterial, relations, myLevel, ref resultLevel);
            }
        }

        private void SetDepthRecursive(Material material, List<PartslistPos> positions, List<MaterialWFRelation> relations, int currentLevel)
        {
            PartslistPos pos = positions.FirstOrDefault(c => c.MaterialID == material.MaterialID);
            if (pos == null)
                return;

            pos.Sequence = currentLevel;
            var sources = relations.Where(c => c.TargetMaterialID != c.SourceMaterialID && c.TargetMaterialID == material.MaterialID);

            foreach (var source in sources)
            {
                currentLevel = pos.Sequence - 100;
                SetDepthRecursive(source.SourceMaterial, positions, relations, currentLevel);
            }
        }

        private void SetLevels(List<PartslistPos> positions, List<MaterialWFRelation> relations)
        {
            var depths = positions.Select(c => c.Sequence).Distinct().OrderByDescending(x => x);

            foreach (var depth in depths)
            {
                int withSources = 50, withoutSources = 0;
                var targetPositons = positions.Where(c => c.Sequence == depth).OrderBy(c => c.Material.MaterialName1);

                foreach (var targetPos in targetPositons)
                {
                    var sources = relations.Where(c => c.SourceMaterialID != c.TargetMaterialID && c.TargetMaterialID == targetPos.MaterialID);
                    if (sources != null && sources.Any())
                    {
                        targetPos.Sequence = depth + withSources;
                        if (withSources < 99)
                            withSources++;
                    }
                    else
                    {
                        targetPos.Sequence = depth + withoutSources;
                        if (withoutSources < 49)
                            withoutSources++;
                    }
                }
            }
        }

        #endregion

        #region Delete

        public static MsgWithDetails PartslistDelete(DatabaseApp dbApp, Partslist partslist)
        {
            MsgWithDetails msgWd = new MsgWithDetails();
            List<PartslistPos> positions = partslist.PartslistPos_Partslist.ToList();
            List<PartslistPosRelation> relations = new List<PartslistPosRelation>();
            List<PartslistACClassMethod> methods = partslist.PartslistACClassMethod_Partslist.ToList();

            foreach (PartslistACClassMethod metod in methods)
                metod.DeleteACObject(dbApp, false);

            foreach (var item in positions)
                relations.AddRange(item.PartslistPosRelation_TargetPartslistPos);

            foreach (var item in relations)
            {
                Msg relMsg = item.DeleteACObject(dbApp, false);
                if (relMsg != null)
                {
                    msgWd.AddDetailMessage(relMsg);
                }
            }

            foreach (var item in positions)
            {
                Msg posMsg = item.DeleteACObject(dbApp, false);
                if (posMsg != null)
                {
                    msgWd.AddDetailMessage(posMsg);
                }
            }

            Msg pMsg = partslist.DeleteACObject(dbApp, false);
            if (pMsg != null)
                msgWd.AddDetailMessage(pMsg);
            if (!msgWd.MsgDetails.Any())
                return null;
            return msgWd;
        }
        #endregion

        #region Change detection

        /// <summary>
        /// Check is Partslist formula changed
        /// </summary>
        /// <param name="databaseApp"></param>
        /// <param name="partslist"></param>
        /// <returns></returns>
        public bool IsFormulaChanged(DatabaseApp databaseApp, Partslist partslist)
        {
            bool isChangedPartslist = partslist.EntityState == EntityState.Modified;
            if (isChangedPartslist)
                isChangedPartslist = AreEntityPropertiesChanged(databaseApp, new List<VBEntityObject>() { partslist }, S_Partslist_Change_Fields);
            bool isChangedQuantities = false;
            bool isAddedElements = false;
            bool isDeletedElements = false;

            if (!isChangedPartslist && partslist.EntityState != EntityState.Detached && partslist.EntityState != EntityState.Deleted)
            {
                isAddedElements =
                       partslist.PartslistPos_Partslist.Any(c => c.EntityState == EntityState.Added);
                    //|| partslist.PartslistPos_Partslist.SelectMany(c => c?.PartslistPosRelation_TargetPartslistPos).Any(c => c != null && c.EntityState == EntityState.Added);

                if (!isAddedElements)
                {
                    List<VBEntityObject> changedPositions =
                         partslist
                         .PartslistPos_Partslist
                         .Where(c => c.EntityState == EntityState.Modified)
                         .Select(c => c as VBEntityObject)
                         .ToList();

                    List<VBEntityObject> changedRelations =
                        partslist
                        .PartslistPos_Partslist
                        .SelectMany(c => c.PartslistPosRelation_TargetPartslistPos)
                        .Where(c => c.EntityState == EntityState.Modified)
                        .Select(c => c as VBEntityObject)
                        .ToList();

                    List<VBEntityObject> changedObjects = changedPositions.Union(changedRelations).ToList();

                    isChangedQuantities = AreEntityPropertiesChanged(databaseApp, changedObjects, S_PartslistPosRelation_Change_Fields);
                }
            }

            if (!isChangedPartslist)
            {
                IEnumerable<EntityEntry> deletedItems = partslist.GetObjectContext().ChangeTracker.Entries().Where(c => c.State == EntityState.Deleted);
                if (deletedItems.Any())
                {
                    IEnumerable<EntityEntry> deletedPositions = deletedItems.Where(c => c.Metadata.Name == nameof(PartslistPos));
                    foreach (EntityEntry deletedPos in deletedPositions)
                    {
                        if (deletedPos.Entity is PartslistPos)
                        {
                            isChangedPartslist = (deletedPos.Entity as PartslistPos).PartslistID == partslist.PartslistID;
                            if (isChangedPartslist)
                                break;
                        }
                    }

                    if (!isChangedPartslist)
                    {
                        IEnumerable<EntityEntry> deletedRelations = deletedItems.Where(c => c.Metadata.Name == nameof(PartslistPosRelation));
                        foreach (EntityEntry deletedRel in deletedRelations)
                        {
                            if (deletedRel.Entity is PartslistPosRelation)
                            {
                                PartslistPosRelation deletedRelation = deletedRel.Entity as PartslistPosRelation;
                                isChangedPartslist =
                                    partslist.PartslistPos_Partslist.Any(c => c.PartslistPosID == deletedRelation.SourcePartslistPosID)
                                    || partslist.PartslistPos_Partslist.Any(c => c.PartslistPosID == deletedRelation.TargetPartslistPosID);
                                if (isChangedPartslist)
                                    break;
                            }
                        }

                    }
                }
            }

            return isChangedPartslist || isAddedElements || isChangedQuantities || isDeletedElements;
        }

        private bool AreEntityPropertiesChanged(DatabaseApp databaseApp, List<VBEntityObject> changedObjects, string[] fieldsForValidation)
        {

            bool isChangedProperty = false;
            foreach (VBEntityObject changedObject in changedObjects)
            {
                EntityEntry myObjectState = databaseApp.Entry(changedObject);
                IEnumerable<string> modifiedProperties = myObjectState.Properties.Where(c => c.IsModified).Select(c => c.Metadata.Name);
                foreach (string modifiedProperty in modifiedProperties)
                {
                    if (fieldsForValidation.Contains(modifiedProperty))
                    {
                        object oldValue = myObjectState.OriginalValues[modifiedProperty];
                        object newValue = myObjectState.CurrentValues[modifiedProperty];
                        bool isValuesEqual =
                            (oldValue == null && newValue == null) ||
                            (oldValue != null && newValue != null && IsValuesEquals(oldValue, newValue));
                        isChangedProperty = !isValuesEqual;
                        if (isChangedProperty)
                            return isChangedProperty;
                    }
                }
            }
            return isChangedProperty;
        }


        private bool IsValuesEquals(object oldValue, object newValue)
        {
            if (oldValue is double)
            {
                double oldValueD = Math.Round((double)oldValue, 6);
                double newValueD = Math.Round((double)newValue, 6);
                return Math.Abs(oldValueD - newValueD) < Double.Epsilon;
            }
            return oldValue.Equals(newValue);
        }
        #endregion

        #region Configuration

        public virtual void InitStandardPartslistConfigParams(Partslist pl, bool forNewPartslistOnly = false)
        {
            // Doing nothing
        }


        public virtual Msg CopyConfigParams(DatabaseApp databaseApp, Partslist oldPartslist, Partslist newPartslist)
        {
            Msg msg = null;
            if (oldPartslist.MaterialWFID == null || newPartslist.MaterialWFID == null || oldPartslist.MaterialWFID != newPartslist.MaterialWFID)
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CopyConfigParams(5)",
                    Message = Root.Environment.TranslateMessage(this, "lblCpCnfPrmWfsNotEq")
                };
            }
            List<OldNewPartslistConfigIDs> oldNewIDs = new List<OldNewPartslistConfigIDs>();
            if (newPartslist.PartslistConfig_Partslist.Any())
            {
                var importedConfigsToDelete = newPartslist.PartslistConfig_Partslist.ToList();
                foreach (var newConfig in importedConfigsToDelete)
                    newConfig.DeleteACObject(databaseApp, false);
            }
            foreach (var oldConfig in oldPartslist.PartslistConfig_Partslist)
            {
                PartslistConfig newConfig = new PartslistConfig();
                newConfig.PartslistConfigID = Guid.NewGuid();
                oldNewIDs.Add(new OldNewPartslistConfigIDs(oldConfig.PartslistConfigID, newConfig.PartslistConfigID));

                newConfig.VBiACClass = oldConfig.VBiACClass;
                newConfig.ACClassPropertyRelation = oldConfig.ACClassPropertyRelation;
                newConfig.Material = oldConfig.Material;
                newConfig.VBiACClass = oldConfig.VBiACClass;
                newConfig.VBiValueTypeACClass = oldConfig.VBiValueTypeACClass;
                newConfig.KeyACUrl = oldConfig.KeyACUrl;
                newConfig.PreConfigACUrl = oldConfig.PreConfigACUrl;
                newConfig.LocalConfigACUrl = oldConfig.LocalConfigACUrl;
                newConfig.Expression = oldConfig.Expression;
                newConfig.Comment = oldConfig.Comment;
                newConfig.XMLConfig = oldConfig.XMLConfig;


                newPartslist.PartslistConfig_Partslist.Add(newConfig);
            }
            // update ParentPartslistConfigID
            Dictionary<Guid, Guid> oldIDandParentID =
                oldPartslist
                .PartslistConfig_Partslist
                .Where(c => c.ParentPartslistConfigID != null)
                .ToDictionary(key => key.PartslistConfigID, val => val.ParentPartslistConfigID ?? Guid.Empty);

            if (oldIDandParentID != null && oldIDandParentID.Any())
            {
                foreach (var item in oldIDandParentID)
                {
                    Guid newId = oldNewIDs.FirstOrDefault(c => c.OldPartslistConfigID == item.Key).NewPartslistConfigID;
                    Guid newParentID = oldNewIDs.FirstOrDefault(c => c.OldPartslistConfigID == item.Value).NewPartslistConfigID;
                    PartslistConfig newConfig = newPartslist.PartslistConfig_Partslist.FirstOrDefault(c => c.PartslistConfigID == newId);
                    newConfig.ParentPartslistConfigID = newParentID;
                }
            }

            return msg;
        }
        #endregion

        #region Clone WF Relations

        public void GetMaterialWFConnections(List<ApplyMatConnectionToOtherWF> connections, gip.mes.datamodel.MaterialWFACClassMethod mwfMethod, gip.mes.datamodel.ACClassMethod method, string preACUrl = "")
        {
            foreach (datamodel.ACClassWF wf in method.ACClassWF_ACClassMethod.Where(c=>c.RefPAACClassID != null))
            {
                GetMaterialWFConnections(connections, mwfMethod, wf, preACUrl);
            }
        }

        public void GetMaterialWFConnections(List<ApplyMatConnectionToOtherWF> connections, gip.mes.datamodel.MaterialWFACClassMethod mwfMethod, gip.mes.datamodel.ACClassWF wf, string preACUrl = "")
        {
            ApplyMatConnectionToOtherWF item = new ApplyMatConnectionToOtherWF();
            item.WF = wf;
            gip.core.datamodel.ACClassWF iWf = wf.FromIPlusContext<gip.core.datamodel.ACClassWF>();
            item.ACUrl = preACUrl + "\\" + iWf.ConfigACUrl;
            item.WFConnection = mwfMethod.MaterialWFConnection_MaterialWFACClassMethod.Where(c => c.ACClassWFID == wf.ACClassWFID).ToList();
            item.MaterialWFACClassMethod = mwfMethod;
            if (wf.RefPAACClassMethod != null)
            {
                GetMaterialWFConnections(connections, mwfMethod, wf.RefPAACClassMethod, preACUrl + "\\" + iWf.ConfigACUrl);
            }

            connections.Add(item);
            foreach (datamodel.ACClassWF childWf in wf.ACClassWF_ParentACClassWF.Where(c => c.RefPAACClassID != null))
            {
                GetMaterialWFConnections(connections, mwfMethod, childWf, preACUrl);
            }
        }

        public List<ApplyMatConnectionToOtherWF> GetMaterialWFConnections(MaterialWF materialWF, Guid aCClassMethodID, Guid? targetACClassMethodID)
        {
            List<ApplyMatConnectionToOtherWF> connections = new List<ApplyMatConnectionToOtherWF>();
            MaterialWFACClassMethod wfMth = materialWF.MaterialWFACClassMethod_MaterialWF.FirstOrDefault(c => c.ACClassMethodID == aCClassMethodID);
            GetMaterialWFConnections(connections, wfMth, wfMth.ACClassMethod, "");
            connections = connections.Where(c => c.WFConnection.Any()).ToList();
            return connections;
        }


        public void ApplyMaterialWFConnections(DatabaseApp databaseApp, MaterialWF materialWF, List<ApplyMatConnectionToOtherWF> connections, Guid targetACClassMethodID)
        {
            List<ApplyMatConnectionToOtherWF> targetConnections = new List<ApplyMatConnectionToOtherWF>();
            MaterialWFACClassMethod targetMethod = materialWF.MaterialWFACClassMethod_MaterialWF.FirstOrDefault(c => c.ACClassMethodID == targetACClassMethodID);
            GetMaterialWFConnections(targetConnections, targetMethod, targetMethod.ACClassMethod, "");

            foreach (ApplyMatConnectionToOtherWF sourceConnection in connections)
            {
                ApplyMatConnectionToOtherWF targetConnection = targetConnections.Where(c => c.ACUrl == sourceConnection.ACUrl && c.WF.RefPAACClassID == sourceConnection.WF.RefPAACClassID).FirstOrDefault();
                if (targetConnection != null)
                {
                    foreach (MaterialWFConnection mwf in sourceConnection.WFConnection)
                    {
                        MaterialWFConnection targetMwf = targetConnection.WFConnection.Where(c => c.MaterialID == mwf.MaterialID).FirstOrDefault();
                        if (targetMwf == null)
                        {
                            targetMwf = MaterialWFConnection.NewACObject(databaseApp, targetConnection.MaterialWFACClassMethod);
                            targetMwf.ACClassWF = targetConnection.WF;
                            targetMwf.Material = mwf.Material;
                            databaseApp.MaterialWFConnection.Add(targetMwf);
                            targetConnection.MaterialWFACClassMethod.MaterialWFConnection_MaterialWFACClassMethod.Add(targetMwf);
                            targetConnection.WFConnection.Add(targetMwf);
                        }
                    }
                }
            }

        }
        #endregion
    }

    public class OldNewPartslistConfigIDs
    {

        public OldNewPartslistConfigIDs(Guid oldPartslistConfigID, Guid newPartslistConfigID)
        {
            OldPartslistConfigID = oldPartslistConfigID;
            NewPartslistConfigID = newPartslistConfigID;
        }
        public Guid OldPartslistConfigID { get; set; }
        public Guid NewPartslistConfigID { get; set; }
    }
}
