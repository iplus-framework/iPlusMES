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

        public PAEScannerDecoderWS(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            WSResponse<BarcodeSequence> response = null;
            switch (sequence.BarcodeIssuer)
            {
                case BarcodeSequenceBase.BarcodeIssuerEnum.General:
                    response = OnHandleNextBarcodeSequenceGeneral(sequence);
                    break;
                case BarcodeSequenceBase.BarcodeIssuerEnum.Picking:
                    response = OnHandleNextBarcodeSequencePicking(sequence);
                    break;
                case BarcodeSequenceBase.BarcodeIssuerEnum.Production:
                    response = OnHandleNextBarcodeSequenceProduction(sequence);
                    break;
                case BarcodeSequenceBase.BarcodeIssuerEnum.Inventory:
                    response = OnHandleNextBarcodeSequenceInventory(sequence);
                    break;
                default:
                    response = OnHandleNextBarcodeSequenceProduction(sequence);
                    break;
            }
            return response;
        }

        public virtual WSResponse<BarcodeSequence> OnHandleNextBarcodeSequenceGeneral(BarcodeSequence sequence)
        {
            return new WSResponse<BarcodeSequence>(sequence);
        }

        public virtual WSResponse<BarcodeSequence> OnHandleNextBarcodeSequencePicking(BarcodeSequence sequence)
        {
            throw new NotImplementedException();
        }

        public virtual WSResponse<BarcodeSequence> OnHandleNextBarcodeSequenceProduction(BarcodeSequence sequence)
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

        /// <summary>
        /// Barcode sequence is complete when valid FacilityCharge is obtained
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public virtual WSResponse<BarcodeSequence> OnHandleNextBarcodeSequenceInventory(BarcodeSequence sequence)
        {
            // Can be scanned
            // Material
            // Facility Lot
            // Facility

            // Result: FacilityCharge (unique)
            // Is enough when unique charge is obtained

            if (sequence.Sequence.Any(c => c.FacilityCharge != null))
            {
                sequence.State = BarcodeSequenceBase.ActionState.Completed;
            }
            else if (sequence.Sequence == null || !sequence.Sequence.Any())
            {
                sequence.State = BarcodeSequenceBase.ActionState.ScanAgain;
            }
            else
            {

                // Remove old version item with new one
                List<BarcodeEntity> sameOlderItems =
                    sequence
                    .Sequence
                    .Where(c =>
                                sequence.LastAddedSequence != null
                                && c.ValidEntity.GetType() == sequence.LastAddedSequence.ValidEntity.GetType()
                                && c.Barcode != sequence.LastAddedSequence.Barcode
                            )
                    .ToList();

                foreach (BarcodeEntity sameOlderItem in sameOlderItems)
                    sequence.Sequence.Remove(sameOlderItem);

                sequence.State = BarcodeSequenceBase.ActionState.ScanAgain;
                BarcodeEntity material = sequence.Sequence.Where(c => c.Material != null).FirstOrDefault();
                BarcodeEntity facility = sequence.Sequence.Where(c => c.Facility != null).FirstOrDefault();
                BarcodeEntity facilityLot = sequence.Sequence.Where(c => c.FacilityLot != null).FirstOrDefault();

                List<gip.mes.datamodel.FacilityCharge> foundedFacilityCharges = new List<gip.mes.datamodel.FacilityCharge>();
                List<gip.mes.datamodel.FacilityCharge> tmpChargeList = new List<gip.mes.datamodel.FacilityCharge>();
                using (var dbApp = new gip.mes.datamodel.DatabaseApp())
                {
                    if (material != null && facility == null && facilityLot == null)
                    {
                        // Info50067
                        // Material scanned, please scan facility or lot
                        sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnHandleNextBarcodeSequenceInventory", 161, "Info50067");
                    }
                    else if (material == null && facility != null && facilityLot == null)
                    {
                        // Info50068
                        // Facility scanned, please scan material or facilityLot
                        sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnHandleNextBarcodeSequenceInventory", 165, "Info50068");
                    }
                    else if (material == null && facility == null && facilityLot != null)
                    {
                        // Info50069
                        // Lot scanned, please scan material or facilty
                        sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnHandleNextBarcodeSequenceInventory", 171, "Info50069");
                    }
                    // Search lot
                    else if (material != null && facility != null && facilityLot == null)
                    {
                        tmpChargeList =
                            dbApp
                            .FacilityCharge
                            .Where(c =>
                                        !c.NotAvailable
                                        && c.Material.MaterialNo == material.Material.MaterialNo
                                        && c.Facility.FacilityNo == facility.Facility.FacilityNo
                                    )
                           .ToList();

                        foundedFacilityCharges.AddRange(tmpChargeList);
                        if (tmpChargeList.Count > 1)
                        {
                            // Info50070
                            // For selected material and facility founded one or more quants, please scan lot to
                            sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnHandleNextBarcodeSequenceInventory", 192, "Info50070");
                        }
                    }
                    else if (material != null && facility == null && facilityLot != null)
                    {
                        tmpChargeList =
                            dbApp
                            .FacilityCharge
                            .Where(c =>
                                        !c.NotAvailable
                                        && c.Material.MaterialNo == material.Material.MaterialNo
                                        && c.FacilityLot.LotNo == facilityLot.FacilityLot.LotNo
                                    )
                             .ToList();
                        foundedFacilityCharges.AddRange(tmpChargeList);
                        if (tmpChargeList.Count > 1)
                        {
                            // Info50071
                            // For selected material and lot founded one or more quants, please scan faciltiy to!
                            sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnHandleNextBarcodeSequenceInventory", 213, "Info50071");
                        }
                    }
                    else if (material == null && facility != null && facilityLot != null)
                    {
                        tmpChargeList =
                            dbApp
                            .FacilityCharge
                            .Where(c =>
                                        !c.NotAvailable
                                        && c.Facility.FacilityNo == facility.Facility.FacilityNo
                                        && c.FacilityLot.LotNo == facilityLot.FacilityLot.LotNo
                                    )
                             .ToList();
                        foundedFacilityCharges.AddRange(tmpChargeList);
                        if (tmpChargeList.Count > 1)
                        {
                            // Info50072
                            // For selected facility and lot founded one or more charges, please scan material to
                            sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnHandleNextBarcodeSequenceInventory", 213, "Info50072");
                        }
                    }
                    else
                    {
                        tmpChargeList =
                            dbApp
                            .FacilityCharge
                            .Where(c =>
                                        !c.NotAvailable
                                        && c.Material.MaterialNo == material.Material.MaterialNo
                                        && c.Facility.FacilityNo == facility.Facility.FacilityNo
                                        && c.FacilityLot.LotNo == facilityLot.FacilityLot.LotNo
                                    )
                             .ToList();
                        foundedFacilityCharges.AddRange(tmpChargeList);
                        if (tmpChargeList.Count > 1)
                        {
                            // Info50073
                            // For selected facility, lot and material founded one or more charges, please scan exat faciltiy charge
                            sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnHandleNextBarcodeSequenceInventory", 256, "Info50073");
                        }
                    }

                    if (foundedFacilityCharges.Any())
                    {
                        sequence.Sequence.AddRange(
                            foundedFacilityCharges
                            .Select(c=> new BarcodeEntity()
                                {
                                    FacilityCharge = VBWebService.s_cQry_GetFacilityCharge(dbApp, c.FacilityChargeID).FirstOrDefault()
                                }
                            ));
                        sequence.LastAddedSequence = sequence.Sequence.LastOrDefault();
                        sequence.State = BarcodeSequenceBase.ActionState.Completed;
                    }
                }
            }
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
