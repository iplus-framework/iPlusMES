using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
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

    }
}
