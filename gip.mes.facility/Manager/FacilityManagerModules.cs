using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System.Data.Objects;

namespace gip.mes.facility
{
    public partial class FacilityManager
    {
        #region Module Constants

        private static ACPropertyConfigValue<string> _IntakeClass;
        public static string IntakeClass { get { return _IntakeClass == null ? "" : _IntakeClass.ValueT; } }
        [ACPropertyConfig("en{'Classname for intake'}de{'Klassenname für Annahme'}")]
        public string C_IntakeClass
        {
            get { return _IntakeClass.ValueT; }
            set { _IntakeClass.ValueT = value; }
        }


        private static ACPropertyConfigValue<string> _SiloClass;
        public static string SiloClass { get { return _SiloClass == null ? "" : _SiloClass.ValueT; } }
        [ACPropertyConfig("en{'Classname for Silo'}de{'Klassenname für Silos'}")]
        public string C_SiloClass
        {
            get { return _SiloClass.ValueT; }
            set { _SiloClass.ValueT = value; }
        }

        private static ACPropertyConfigValue<string> _PWMethodRelocClass;
        public static string PWMethodRelocClass { get { return _PWMethodRelocClass == null ? "" : _PWMethodRelocClass.ValueT; } }
        [ACPropertyConfig("en{'PWMEthod-Classname for relocation'}de{'PWMEthod-Klassenname für Umlagerungen'}")]
        public string C_PWMethodRelocClass
        {
            get { return _PWMethodRelocClass.ValueT; }
            set { _PWMethodRelocClass.ValueT = value; }
        }

        protected virtual void CreateModuleConstants()
        {
            if (_IntakeClass == null)
            {
                _IntakeClass = new ACPropertyConfigValue<string>(this, "C_IntakeClass", "PAMIntake");
                _SiloClass = new ACPropertyConfigValue<string>(this, "C_SiloClass", "PAMSilo");
                _PWMethodRelocClass = new ACPropertyConfigValue<string>(this, "C_PWMethodRelocClass", "PWMethodRelocation");
            }
        }

        protected virtual void InitModuleConstants()
        {
            _ = C_IntakeClass;
            _ = C_SiloClass;
            _ = C_PWMethodRelocClass;
        }


        public gip.core.datamodel.ACClass GetACClassForIdentifier(string acIdentifier, Database db)
        {
            return s_cQry_ACClassIdentifier(db, acIdentifier);
        }

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
        public static readonly Func<Database, string, gip.core.datamodel.ACClass> s_cQry_ACClassIdentifier =
        CompiledQuery.Compile<Database, string, gip.core.datamodel.ACClass>(
            (ctx, acIdentifier) => ctx.ACClass.Where(c => c.ACIdentifier == acIdentifier).FirstOrDefault()
        );

        public static readonly Func<Database, string, IQueryable<gip.core.datamodel.ACClass>> s_cQry_GetAvailableModulesAsACClass =
        CompiledQuery.Compile<Database, string, IQueryable<gip.core.datamodel.ACClass>>(
            (ctx, acIdentifier) => ctx.ACClass.Where(c => (c.BasedOnACClassID.HasValue
                                                            && (c.ACClass1_BasedOnACClass.ACIdentifier == acIdentifier // 1. Ableitungsstufe
                                                                || (c.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                        && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == acIdentifier // 2. Ableitungsstufe
                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                        && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == acIdentifier // 3. Ableitungsstufe
                                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == acIdentifier) // 4. Ableitungsstufe
                                                                                            )
                                                                                )
                                                                            )
                                                                    )
                                                                )
                                                            )
                                                            && c.ACProject != null && c.ACProject.ACProjectTypeIndex == (short)Global.ACProjectTypes.Application)
                                                            .OrderBy(c => c.ACIdentifier)
        );


        #endregion

        #region DeliveryNotePos

        #region Available Modules

        public virtual IList<gip.core.datamodel.ACClass> GetAvailableIntakeModulesAsACClass(Database db)
        {
            return s_cQry_GetAvailableModulesAsACClass(db, C_IntakeClass).ToList();
        }


        #endregion


        #region Available Targets

        public virtual IList<gip.core.datamodel.ACClass> GetAvailableTargetsAsACClass(DeliveryNotePos deliveryNotePos, gip.core.datamodel.ACClass sourceModule, Database db)
        {
            if (sourceModule == null || deliveryNotePos == null)
                return null;

            RoutingResult result = ACRoutingService.FindSuccessors(RoutingService, db, true, 
                                    sourceModule, C_SiloClass, RouteDirections.Forwards, new object[] { },
                                    (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                                    null,
                                    0, true, true, false, false, 3);
            if (result.Routes == null || !result.Routes.Any())
                return null;
            if (!result.IsDbResult)
                return result.Routes.Select(c => c.Last().Target).OrderBy(c => c.ACIdentifier).ToList();
            else
            {
                gip.core.datamodel.ACClass acClassSilo = s_cQry_ACClassIdentifier(db, C_SiloClass);
                if (acClassSilo == null || acClassSilo.ObjectType == null)
                    return null;
                return result.Routes.Select(c => c.Last().Target)
                    .OrderBy(c => c.ACIdentifier)
                    .ToList()
                    .Where(c => acClassSilo.ObjectType.IsAssignableFrom(c.ObjectType))
                    .ToList();
            }
        }


        #endregion


        #region Selected Modules

        public virtual IList<gip.core.datamodel.ACClass> GetSelectedModulesAsACClass(DeliveryNotePos DeliveryNotePos, Database db)
        {
            if (DeliveryNotePos == null)
                return null;
            try
            {
                var query = GetSelectedModules(DeliveryNotePos);
                if (query == null)
                    return null;
                return query
                    .Select(c => c.GetFacilityACClass(db))
                    .ToArray();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("FacilityManager", "GetSelectedModulesAsACClass", msg);
            }
            return null;
        }

        public virtual IList<FacilityReservation> GetSelectedModules(DeliveryNotePos deliveryNotePos)
        {
            if (deliveryNotePos == null || deliveryNotePos.InOrderPos == null)
                return null;
            try
            {
                return deliveryNotePos.InOrderPos.FacilityReservation_InOrderPos
                    .Where(c => c.VBiACClassID.HasValue && !c.ParentFacilityReservationID.HasValue)
                    .OrderBy(c => c.Sequence)
                    .ToArray();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("FacilityManager", "GetSelectedModules", msg);
            }
            return null;
        }

        public virtual bool IsModuleSelected(DeliveryNotePos deliveryNotePos, Guid acClassID)
        {
            var selectedModules = GetSelectedModules(deliveryNotePos);
            if (selectedModules == null)
                return false;
            return selectedModules.Where(c => c.VBiACClassID.HasValue && c.VBiACClassID == acClassID).Any();
        }


        #endregion


        #region Selected Targets

        public virtual IList<gip.core.datamodel.ACClass> GetSelectedTargetsAsACClass(DeliveryNotePos deliveryNotePos, gip.core.datamodel.ACClass sourceMachine, Database db)
        {
            if (deliveryNotePos == null || sourceMachine == null)
                return null;
            try
            {
                var query = GetSelectedTargets(deliveryNotePos, sourceMachine);
                if (query == null)
                    return null;
                return query
                    .Select(c => c.GetFacilityACClass(db))
                    .ToArray();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("FacilityManager", "GetSelectedTargetsAsACClass", msg);
            }
            return null;
        }

        public virtual IList<FacilityReservation> GetSelectedTargets(DeliveryNotePos deliveryNotePos, gip.core.datamodel.ACClass sourceMachine)
        {
            if (deliveryNotePos == null || deliveryNotePos.InOrderPos == null || sourceMachine == null)
                return null;
            try
            {
                if (!deliveryNotePos.InOrderPos.FacilityReservation_InOrderPos.Any())
                    return null;
                FacilityReservation reservation = deliveryNotePos.InOrderPos.FacilityReservation_InOrderPos.Where(c => c.VBiACClassID.HasValue && c.VBiACClassID == sourceMachine.ACClassID).FirstOrDefault();
                if (reservation == null)
                    return null;
                if (!reservation.FacilityReservation_ParentFacilityReservation.Any())
                    return null;
                return reservation.FacilityReservation_ParentFacilityReservation
                    .Where(c => c.VBiACClassID.HasValue)
                    .OrderBy(c => c.Sequence)
                    .ToArray();
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("FacilityManager", "GetSelectedTargets", msg);
            }
            return null;
        }


        #endregion

        #endregion
    }
}
