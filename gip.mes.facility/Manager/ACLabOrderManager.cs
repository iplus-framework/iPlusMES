using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

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

        /// <summary>
        /// Create new lab order based on template.
        /// </summary>
        public Msg CreateNewLabOrder(DatabaseApp dbApp, LabOrder template, string labOrderName, InOrderPos inOrderPos, OutOrderPos outOrderPos, ProdOrderPartslistPos prodOrderPartslistPos, FacilityLot facilityLot, out LabOrder labOrder)
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

            if (inOrderPos == null && outOrderPos == null && prodOrderPartslistPos == null && facilityLot == null)
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

            ACValueList resultList = pwNodeACMethod.ResultValueList;
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
                dbApp.LabOrder.AddObject(template);

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
                    dbApp.MDLabTag.AddObject(resultItemTag);
                }

                LabOrderPos pos = template.LabOrderPos_LabOrder.FirstOrDefault(c => c.MDLabTagID == resultItemTag.MDLabTagID);
                if (pos == null)
                {
                    pos = LabOrderPos.NewACObject(dbApp, template);
                    pos.MDLabTag = resultItemTag;
                    dbApp.LabOrderPos.AddObject(pos);
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
                msg = CreateNewLabOrder(dbApp, template, "", null, null, intermediateChildPos, null, out currentLabOrder);
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
    }
}
