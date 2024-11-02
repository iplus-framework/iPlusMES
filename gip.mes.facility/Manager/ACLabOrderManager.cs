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

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'LabOrderManager'}de{'LabOrderManager'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class ACLabOrderManager : PARole
    {
        #region c'tors
        public ACLabOrderManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
           : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        public const string C_DefaultServiceACIdentifier = "LabOrderManager";
        #endregion

        #region Attach / Detach
        public static ACLabOrderManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACLabOrderManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ACLabOrderManager> ACRefToServiceInstance(ACComponent requester)
        {
            ACLabOrderManager serviceInstance = GetServiceInstance(requester);
            if (serviceInstance != null)
                return new ACRef<ACLabOrderManager>(serviceInstance, requester);
            return null;
        }
        #endregion


        public Msg CreateNewLabOrder(DatabaseApp dbApp, string labOrderName, InOrderPos inOrderPos, OutOrderPos outOrderPos, ProdOrderPartslistPos prodOrderPartslistPos, FacilityLot facilityLot, PickingPos pickingPos, out LabOrder labOrder)
        {
            List<LabOrder> labOrderTemplates = null;
            labOrder = null;
            Msg msg = GetOrCreateDefaultLabTemplate(dbApp, inOrderPos, outOrderPos, prodOrderPartslistPos, facilityLot, pickingPos, out labOrderTemplates);
            if (msg != null)
                return msg;

            if (labOrderTemplates == null || !labOrderTemplates.Any())
                return null;

            return CreateNewLabOrder(dbApp, labOrderTemplates.FirstOrDefault(), labOrderName, inOrderPos, outOrderPos, prodOrderPartslistPos, facilityLot, pickingPos, out labOrder);
        }

        /// <summary>
        /// Create new lab order based on template.
        /// </summary>
        public Msg CreateNewLabOrder(DatabaseApp dbApp, LabOrder template, string labOrderName, InOrderPos inOrderPos, OutOrderPos outOrderPos, ProdOrderPartslistPos prodOrderPartslistPos, FacilityLot facilityLot, PickingPos pickingPos, out LabOrder labOrder)
        {
            Msg msg = null;
            labOrder = null;

            if (template == null)
            {
                //"Error:Lab order template missing!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CreateNewLabOrder(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50050")
                };
            }

            if (inOrderPos == null && outOrderPos == null && prodOrderPartslistPos == null && facilityLot == null && pickingPos == null)
            {
                //"Error:Material state is not defined!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CreateNewLabOrder(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50051")
                };
            }

            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(LabOrder), LabOrder.NoColumnName, LabOrder.FormatNewNo, this);
            labOrder = LabOrder.NewACObject(dbApp, template, secondaryKey);
            labOrder.InOrderPos = inOrderPos;
            labOrder.OutOrderPos = outOrderPos;
            labOrder.ProdOrderPartslistPos = prodOrderPartslistPos;
            labOrder.FacilityLot = facilityLot;
            labOrder.PickingPos = pickingPos;
            if (!String.IsNullOrEmpty(labOrderName))
                labOrder.TemplateName = labOrderName;

            msg = CopyLabOrderTemplatePos(dbApp, labOrder, template);

            return msg;
        }

        public Msg CopyLabOrderTemplatePos(DatabaseApp dbApp, LabOrder current, LabOrder template)
        {
            Msg msg = null;
            if (template == null)
            {
                //"Error:Lab order template missing!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CreateNewLabOrder(0)",
                    Message = Root.Environment.TranslateMessage(this, "Error50050")
                };
            }


            foreach (var item in template.LabOrderPos_LabOrder)
            {
                current.LabOrderPos_LabOrder.Add(LabOrderPos.NewACObject(dbApp, current, item));
            }

            current.BasedOnTemplateID = template.LabOrderID;
            OnLabOrderTemplateCopied(dbApp, current, template);

            return msg;
        }

        public IQueryable<LabOrder> ReturnLabOrderTemplateList(DatabaseApp dbApp)
        {
            return dbApp.LabOrder.Where(c => c.LabOrderTypeIndex == (short)GlobalApp.LabOrderType.Template);
        }

        protected virtual void OnLabOrderTemplateCopied(DatabaseApp dbApp, LabOrder current, LabOrder template)
        {
        }

        public virtual MsgWithDetails GetMaterialReleaseState(LabOrder labOrder, Material material, ref MDReleaseState.ReleaseStates releaseStateSilo)
        {
            if (labOrder == null)
                return null;

            return new MsgWithDetails();
        }

        public virtual void OnEmptySampleMagazine()
        {

        }


        public Msg GetOrCreateLabTemplateForPWNode(DatabaseApp dbApp, string templateName, Material material, ACMethod pwNodeACMethod, IEnumerable<string> acIdentifiersOfExcludedResult, out LabOrder template)
        {
            template = null;
            if (material == null || pwNodeACMethod == null || dbApp == null)
                return null;
            return GetOrCreateLabTemplateForValueList(dbApp, templateName, material, pwNodeACMethod.ResultValueList, acIdentifiersOfExcludedResult, out template);
        }

        public Msg GetOrCreateLabTemplateForValueList(DatabaseApp dbApp, string templateName, Material material, ACValueList resultList, IEnumerable<string> acIdentifiersOfExcludedResult, out LabOrder template)
        {
            template = null;

            if (resultList == null || !resultList.Any())
                return null;

            template = dbApp.LabOrder.FirstOrDefault(c => c.LabOrderTypeIndex == (short)GlobalApp.LabOrderType.Template
                                                       && c.TemplateName == templateName
                                                       && c.MaterialID == material.MaterialID);
            if (template == null)
            {
                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(LabOrder), LabOrder.NoColumnName, LabOrder.FormatNewNo, this);
                template = LabOrder.NewACObject(dbApp, null, secondaryKey);
                template.LabOrderTypeIndex = (short)GlobalApp.LabOrderType.Template;
                template.MDLabOrderState = dbApp.MDLabOrderState.FirstOrDefault(c => c.IsDefault);
                template.TemplateName = templateName;
                template.Material = material;
                dbApp.LabOrder.Add(template);

            }

            foreach (ACValue resultItem in resultList)
            {
                if (acIdentifiersOfExcludedResult != null && acIdentifiersOfExcludedResult.Contains(resultItem.ACIdentifier))
                    continue;

                MDLabTag resultItemTag = dbApp.MDLabTag.FirstOrDefault(c => c.MDKey == resultItem.ACIdentifier);
                if (resultItemTag == null)
                {
                    resultItemTag = MDLabTag.NewACObject(dbApp, null);
                    resultItemTag.MDKey = resultItem.ACIdentifier;
                    resultItemTag.MDLabTagIndex = (short)MDLabTag.LabTags.Maesure;
                    resultItemTag.MDNameTrans = resultItem.ACCaptionTrans;
                    dbApp.MDLabTag.Add(resultItemTag);
                }

                LabOrderPos pos = template.LabOrderPos_LabOrder.FirstOrDefault(c => c.MDLabTagID == resultItemTag.MDLabTagID);
                if (pos == null)
                {
                    pos = LabOrderPos.NewACObject(dbApp, template);
                    pos.MDLabTag = resultItemTag;
                    dbApp.LabOrderPos.Add(pos);
                }
            }

            Msg msg = dbApp.ACSaveChanges();
            if (msg != null)
                return msg;

            return null;
        }


        public Msg CreateOrUpdateLabOrderForPWNode(DatabaseApp dbApp, string templateName, ACMethod pwNodeACMethod, ProdOrderPartslistPos intermediateChildPos,
                                                   ProdOrderPartslistPos intermediatePosition, ProdOrderPartslistPos endBatchPos,
                                                   IEnumerable<string> acIdentifersOfRepeatedResult, IEnumerable<string> acIdentifiersOfExcludedResult)
        {
            ACValueList resultList = pwNodeACMethod.ResultValueList;
            if (resultList == null || !resultList.Any())
                return null;


            Material material = null;
            if (endBatchPos != null)
                material = endBatchPos.BookingMaterial;
            if (material == null)
                material = intermediatePosition.BookingMaterial;
            if (material == null)
            {
                return new Msg(eMsgLevel.Error, "The material for laboratory order is not available!");
            }

            LabOrder template;

            Msg msg = GetOrCreateLabTemplateForPWNode(dbApp, templateName, material, pwNodeACMethod, acIdentifiersOfExcludedResult, out template);
            if (template == null && msg != null)
            {
                //TODO: details
                return new Msg(eMsgLevel.Error, "The laboratory order template is not available!");
            }

            LabOrder currentLabOrder = dbApp.LabOrder.FirstOrDefault(c => c.ProdOrderPartslistPosID == intermediateChildPos.ProdOrderPartslistPosID);

            if (currentLabOrder == null)
            {
                msg = CreateNewLabOrder(dbApp, template, "", null, null, intermediateChildPos, null, null, out currentLabOrder);
                if (msg != null)
                    return msg;
            }

            foreach (ACValue resultItem in resultList)
            {
                if (acIdentifiersOfExcludedResult != null && acIdentifiersOfExcludedResult.Contains(resultItem.ACIdentifier))
                    continue;

                double? result = null;

                try
                {
                    result = resultItem.ParamAsDouble;
                }
                catch (Exception e)
                {
                    Messages.LogException(this.GetACUrl(), nameof(CreateOrUpdateLabOrderForPWNode), e);
                }

                if (!result.HasValue)
                    continue;

                LabOrderPos loPos = currentLabOrder.LabOrderPos_LabOrder.OrderByDescending(c => c.Sequence).FirstOrDefault(c => c.MDLabTag.MDKey == resultItem.ACIdentifier);
                if (loPos != null && !(acIdentifersOfRepeatedResult != null && acIdentifersOfRepeatedResult.Contains(resultItem.ACIdentifier)))
                {
                    loPos.ActualValue = result;
                }
                else
                {
                    LabOrderPos templatePos = template.LabOrderPos_LabOrder.FirstOrDefault(c => c.MDLabTag.MDKey == resultItem.ACIdentifier);
                    if (templatePos != null)
                        loPos = LabOrderPos.NewACObject(dbApp, currentLabOrder, templatePos);

                    int nextSequence = currentLabOrder.LabOrderPos_LabOrder.Max(c => c.Sequence) + 1;

                    if (loPos != null)
                    {
                        loPos.Sequence = nextSequence;
                        loPos.ActualValue = result;
                    }
                }
            }

            return null;
        }

        public Msg CompleteLabOrderForPWNode(DatabaseApp dbApp, ProdOrderPartslistPos intermediateChildPos, ProdOrderPartslistPos intermediatePosition, ProdOrderPartslistPos endBatchPos)
        {
            Material material = null;
            if (endBatchPos != null)
                material = endBatchPos.BookingMaterial;
            if (material == null)
                material = intermediatePosition.BookingMaterial;
            if (material == null)
                return new Msg(eMsgLevel.Error, "The material is not assigned!");

            MDLabOrderState mdLabOrderState = dbApp.MDLabOrderState.FirstOrDefault(c => c.MDLabOrderStateIndex == (short)MDLabOrderState.LabOrderStates.Finished);
            if (mdLabOrderState != null)
            {
                LabOrder currentLabOrder = dbApp.LabOrder.FirstOrDefault(c => c.ProdOrderPartslistPosID == intermediateChildPos.ProdOrderPartslistPosID);
                if (currentLabOrder != null)
                    currentLabOrder.MDLabOrderState = mdLabOrderState;

                return dbApp.ACSaveChanges();
            }
            return null;
        }


        public Msg GetOrCreateDefaultLabTemplate(DatabaseApp dbApp, DeliveryNotePos inOrderPos, DeliveryNotePos outOrderPos, ProdOrderPartslistPos prodOrderPartslistPos, FacilityLot facilityLot, PickingPos pickingPos, out List<LabOrder> labOrderTemplates)
        {
            return GetOrCreateDefaultLabTemplate(dbApp, inOrderPos?.InOrderPos, outOrderPos?.OutOrderPos, prodOrderPartslistPos, facilityLot, pickingPos, out labOrderTemplates);
        }

        public Msg GetOrCreateDefaultLabTemplate(DatabaseApp dbApp, InOrderPos inOrderPos, OutOrderPos outOrderPos, ProdOrderPartslistPos prodOrderPartslistPos, FacilityLot facilityLot, PickingPos pickingPos, out List<LabOrder> labOrderTemplates)
        {
            labOrderTemplates = null;
            if (inOrderPos == null && outOrderPos == null && prodOrderPartslistPos == null && facilityLot == null && pickingPos == null)
            {
                //"Error:Material state is not defined!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CreateNewLabOrder(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50051")
                };
            }

            Material material = inOrderPos?.Material;
            if (material == null)
                material = outOrderPos?.Material;
            if (material == null)
                material = prodOrderPartslistPos?.BookingMaterial;
            if (material == null)
                material = facilityLot?.Material;
            if (material == null)
                material = pickingPos?.Material;
            return GetOrCreateDefaultLabTemplate(dbApp, material, out labOrderTemplates);
        }

        public Msg GetOrCreateDefaultLabTemplate(DatabaseApp dbApp, Material material, out List<LabOrder> labOrderTemplates)
        {
            labOrderTemplates = null;
            if (material == null)
            {
                //"Error:Material state is not defined!"
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "CreateNewLabOrder(1)",
                    Message = Root.Environment.TranslateMessage(this, "Error50051")
                };
            }
            labOrderTemplates = ReturnLabOrderTemplateList(dbApp).Where(c => c.MaterialID == material.MaterialID || (material.ProductionMaterialID.HasValue && c.MaterialID == material.ProductionMaterialID.Value)).ToList();
            if (labOrderTemplates != null && labOrderTemplates.Any())
                return null;

            LabOrder newTemplate = OnCreateNewTemplate(dbApp, material);
            if (newTemplate != null)
            {
                if (newTemplate.EntityState == EntityState.Added)
                {
                    Msg msg = dbApp.ACSaveChanges();
                    if (msg != null)
                        return msg;
                }
                labOrderTemplates.Add(newTemplate);
            }
            return null;
        }

        protected virtual LabOrder OnCreateNewTemplate(DatabaseApp dbApp, Material material)
        {
            return null;
        }

        public bool ResolveEntities(DatabaseApp dbApp, PAOrderInfo orderInfo, out ProdOrderPartslistPos batchPos, out DeliveryNotePos dnPos, out PickingPos pickingPos, out FacilityBooking fBooking, out InOrderPos inOrderPos, out OutOrderPos outOrderPos, out LabOrder labOrder)
        {
            batchPos = null;
            dnPos = null;
            pickingPos = null;
            fBooking = null;
            inOrderPos = null;
            outOrderPos = null;
            labOrder = null;
            if (orderInfo == null)
                return false;
            PAOrderInfoEntry orderInfoEntry = orderInfo.Entities.Where(c => c.EntityName == ProdOrderBatch.ClassName).FirstOrDefault();
            if (orderInfoEntry != null)
            {
                ProdOrderBatch batch = dbApp.ProdOrderBatch
                    .Include(c => c.ProdOrderPartslistPos_ProdOrderBatch)
                    .Include(c => c.ProdOrderPartslist)
                    .Include(c => c.ProdOrderPartslist.ProdOrder)
                    .Where(c => c.ProdOrderBatchID == orderInfoEntry.EntityID).FirstOrDefault();
                if (batch == null)
                    return false;
                batchPos = batch.ProdOrderPartslistPos_ProdOrderBatch.FirstOrDefault();
                if (batchPos == null)
                    return false;
                labOrder = batchPos.LabOrder_ProdOrderPartslistPos.FirstOrDefault();
            }
            if (orderInfoEntry == null)
            {
                orderInfoEntry = orderInfo.Entities.Where(c => c.EntityName == PickingPos.ClassName).FirstOrDefault();
                if (orderInfoEntry != null)
                {
                    pickingPos = dbApp.PickingPos
                        .Include(c => c.PickingMaterial)
                        .Where(c => c.PickingPosID == orderInfoEntry.EntityID).FirstOrDefault();
                    if (pickingPos == null)
                        return false;
                    labOrder = pickingPos.LabOrder_PickingPos.FirstOrDefault();
                }
            }
            if (orderInfoEntry == null)
            {
                orderInfoEntry = orderInfo.Entities.Where(c => c.EntityName == Picking.ClassName).FirstOrDefault();
                if (orderInfoEntry != null)
                {
                    Picking picking = dbApp.Picking
                        .Include("PickingPos_Picking.PickingMaterial")
                        .Where(c => c.PickingID == orderInfoEntry.EntityID).FirstOrDefault();
                    if (picking == null)
                        return false;
                    pickingPos = picking.PickingPos_Picking.Where(c => c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive).FirstOrDefault();
                    if (pickingPos == null)
                        pickingPos = picking.PickingPos_Picking.FirstOrDefault();
                    if (pickingPos == null)
                        return false;
                    labOrder = pickingPos.LabOrder_PickingPos.FirstOrDefault();
                }
            }
            if (orderInfoEntry == null)
            {
                orderInfoEntry = orderInfo.Entities.Where(c => c.EntityName == DeliveryNotePos.ClassName).FirstOrDefault();
                if (orderInfoEntry != null)
                {
                    dnPos = dbApp.DeliveryNotePos
                        .Include(c => c.DeliveryNote)
                        .Include(c => c.InOrderPos.Material)
                        .Include(c => c.OutOrderPos.Material)
                        .Where(c => c.DeliveryNotePosID == orderInfoEntry.EntityID).FirstOrDefault();
                    if (dnPos == null)
                        return false;
                    inOrderPos = dnPos.InOrderPos;
                    if (inOrderPos != null)
                        labOrder = inOrderPos.LabOrder_InOrderPos.FirstOrDefault();
                    outOrderPos = dnPos.OutOrderPos;
                    if (outOrderPos != null)
                        labOrder = outOrderPos.LabOrder_OutOrderPos.FirstOrDefault();
                }
            }
            if (orderInfoEntry == null)
            {
                orderInfoEntry = orderInfo.Entities.Where(c => c.EntityName == FacilityBooking.ClassName).FirstOrDefault();
                if (orderInfoEntry != null)
                {
                    fBooking = dbApp.FacilityBooking.Where(c => c.FacilityBookingID == orderInfoEntry.EntityID).FirstOrDefault();
                    if (fBooking == null)
                        return false;
                }
            }
            if (orderInfoEntry == null)
            {
                orderInfoEntry = orderInfo.Entities.Where(c => c.EntityName == ProdOrderPartslistPosRelation.ClassName).FirstOrDefault();
                if (orderInfoEntry != null)
                {
                    ProdOrderPartslistPosRelation relation = dbApp.ProdOrderPartslistPosRelation
                        .Include(c => c.TargetProdOrderPartslistPos)
                        .Include(c => c.TargetProdOrderPartslistPos.ProdOrderPartslist)
                        .Include(c => c.TargetProdOrderPartslistPos.ProdOrderPartslist.ProdOrder)
                        .Where(c => c.ProdOrderPartslistPosRelationID == orderInfoEntry.EntityID)
                        .FirstOrDefault();
                    if (relation != null)
                        batchPos = relation.TargetProdOrderPartslistPos;
                    else
                        return false;
                }
            }
            return batchPos != null || pickingPos != null || fBooking != null || inOrderPos != null || outOrderPos != null || labOrder != null;
        }

    }
}
