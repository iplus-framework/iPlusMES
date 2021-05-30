using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.processapplication;
using gip.mes.datamodel;

namespace gip.mes.webservices
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scan decoder WS'}de{'Scan decoder WS'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAEScannerDecoderWS : PAEScannerDecoderVB
    {
        #region c'tors
        static PAEScannerDecoderWS()
        {
            RegisterExecuteHandler(typeof(PAEScannerDecoderWS), HandleExecuteACMethod_PAEScannerDecoderWS);
        }

        public PAEScannerDecoderWS(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public const string ClassName = "PAEScannerDecoderWS";

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion


        #region Properties
        #endregion


        #region Methods

        public virtual WSResponse<BarcodeSequence> OnHandleNextBarcodeSequence(BarcodeSequence sequence)
        {
            if (sequence.Sequence.Count >= 3 && sequence.State != BarcodeSequence.ActionState.Question)
            {
                // Error50355: Unsupported command sequence!  (Nicht unterstützte Befehlsfolge!)
                sequence.Message = new Msg(this, eMsgLevel.Error, ClassName, "OnHandleNextBarcodeSequence", 10, "Error50355");
                sequence.State = BarcodeSequence.ActionState.Cancelled;
                return new WSResponse<BarcodeSequence>(sequence);
            }
            BarcodeEntity entityClass = null;
            if (sequence.Sequence.Count == 1)
            {
                sequence.QuestionSequence = 0;

                BarcodeEntity entity = sequence.Sequence.FirstOrDefault();
                if (entity.ValidEntity.GetType() == typeof(FacilityCharge)
                    || entity.ValidEntity.GetType() == typeof(FacilityLot)
                    || entity.ValidEntity.GetType() == typeof(Facility))
                {
                    sequence.Message = new Msg("OK: Bitte Maschine oder Waage scannen!", this, eMsgLevel.Info, ClassName, "OnHandleNextBarcodeSequence", 20);
                    sequence.State = BarcodeSequence.ActionState.ScanAgain;
                    return new WSResponse<BarcodeSequence>(sequence);
                }
                else if (entity.ValidEntity.GetType() == typeof(core.webservices.ACClass))
                    entityClass = entity;
            }
            else
                entityClass = sequence.Sequence.Where(c => c.ValidEntity.GetType() == typeof(core.webservices.ACClass)).FirstOrDefault();

            if (entityClass == null)
            {
                // Error50355: Unsupported command sequence!  (Nicht unterstützte Befehlsfolge!)
                sequence.Message = new Msg(this, eMsgLevel.Error, ClassName, "OnHandleNextBarcodeSequence", 30, "Error50355");
                sequence.State = BarcodeSequence.ActionState.Cancelled;
                return new WSResponse<BarcodeSequence>(sequence);
            }

            ACComponent resolvedComponent;
            PAScannedCompContrBase controller;
            if (!ResolveACComponentAndValidateState(sequence, entityClass, out resolvedComponent, out controller))
                return new WSResponse<BarcodeSequence>(sequence);

            OnHandleResolvedComponent(resolvedComponent, controller, sequence);

            return new WSResponse<BarcodeSequence>(sequence);
        }

        protected virtual bool ResolveACComponentAndValidateState(BarcodeSequence sequence, BarcodeEntity entityClass, out ACComponent resolvedComponent, out PAScannedCompContrBase controller)
        {
            controller = null;
            ACComponent acComponent = ResolveACComponent(sequence, entityClass);
            resolvedComponent = acComponent;
            if (resolvedComponent == null)
                return false;

            controller = FindChildComponents<PAScannedCompContrBase>
                                                (c => c is PAScannedCompContrBase
                                                  && (c as PAScannedCompContrBase).IsControllerFor(acComponent))
                                                .FirstOrDefault();
            if (controller == null)
            {
                sequence.Message = new Msg("Keine Scanunterstützung für Maschine oder Funktion", this, eMsgLevel.Error, ClassName, "ResolveACComponentAndValidateState", 10);
                sequence.State = BarcodeSequence.ActionState.Cancelled;
                return false;
            }

            return controller.CanHandleBarcodeSequence(acComponent, sequence);
        }

        protected ACComponent ResolveACComponent(BarcodeSequence sequence, BarcodeEntity entity)
        {
            if (entity == null)
            {
                sequence.Message = new Msg("Unbekannte Maschine oder Funktion", this, eMsgLevel.Error, ClassName, "ResolveACComponent", 10);
                sequence.State = BarcodeSequence.ActionState.Cancelled;
                return null;
            }

            gip.core.datamodel.ACClass classOfMachine = (Root.Database as Database).GetACType(entity.ACClass.ACClassID);
            if (classOfMachine == null || String.IsNullOrEmpty(classOfMachine.ACUrlComponent))
            {
                sequence.Message = new Msg("Unbekannte Maschine oder Funktion", this, eMsgLevel.Error, ClassName, "ResolveACComponent", 20);
                sequence.State = BarcodeSequence.ActionState.Cancelled;
            }

            ACComponent scannedComponent = ACUrlCommand(classOfMachine.ACUrlComponent) as ACComponent;
            if (scannedComponent == null)
            {
                sequence.Message = new Msg("Unbekannte Maschine oder Funktion", this, eMsgLevel.Error, ClassName, "ResolveACComponent", 30);
                sequence.State = BarcodeSequence.ActionState.Cancelled;
                return null;
            }
            return scannedComponent;
        }

        protected virtual void OnHandleResolvedComponent(ACComponent resolvedComponent, PAScannedCompContrBase controller, BarcodeSequence sequence)
        {
            controller.HandleBarcodeSequence(resolvedComponent, sequence);
        }

        public virtual void GetFacilityEntitiesFromSequence(BarcodeSequence sequence, out BarcodeEntity entityFacility, out BarcodeEntity entityCharge)
        {
            entityCharge = sequence.Sequence.Where(c => c.ValidEntity.GetType() == typeof(FacilityCharge)).FirstOrDefault();
            entityFacility = sequence.Sequence.Where(c => c.ValidEntity.GetType() == typeof(Facility)).FirstOrDefault();
            BarcodeEntity entityLot = sequence.Sequence.Where(c => c.ValidEntity.GetType() == typeof(FacilityLot)).FirstOrDefault();
            // More Quants with same Lot
            if (entityLot != null && entityCharge == null)
                entityCharge = OnResolveFacilityChargeFromLot(sequence, entityLot);
        }

        public void GetGuidFromFacilityEntities(BarcodeEntity entityFacility, BarcodeEntity entityCharge, out Guid facilityID, out Guid facilityChargeID)
        {
            facilityID = Guid.Empty;
            if (entityFacility != null && entityFacility.Facility != null)
                facilityID = entityFacility.Facility.FacilityID;
            facilityChargeID = Guid.Empty;
            if (entityCharge != null && entityCharge.FacilityCharge != null)
                facilityChargeID = entityCharge.FacilityCharge.FacilityChargeID;
        }

        public virtual BarcodeEntity OnResolveFacilityChargeFromLot(BarcodeSequence sequence, BarcodeEntity entityLot)
        {
            BarcodeEntity entityCharge = null;
            using (var dbApp = new gip.mes.datamodel.DatabaseApp())
            {
                var fcInLPWE = dbApp.FacilityCharge.Where(c => !c.NotAvailable
                                                && c.FacilityLotID.HasValue
                                                && c.FacilityLotID == entityLot.FacilityLot.FacilityLotID)
                                                .FirstOrDefault();
                if (fcInLPWE != null)
                {
                    var fcWS = VBWebService.s_cQry_GetFacilityCharge(dbApp, fcInLPWE.FacilityChargeID).FirstOrDefault();
                    if (fcWS != null)
                        entityCharge = new BarcodeEntity() { FacilityCharge = fcWS };
                }
            }
            return entityCharge;
        }

        #endregion


        #region Handle execute helpers
        public static bool HandleExecuteACMethod_PAEScannerDecoderWS(out object result, IACComponent acComponent, string acMethodName, core.datamodel.ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAEScannerDecoderVB(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion
    }

}
