using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Der FacilityBookingManager ist der zentrale Manager zur Durchführung aller physikalischen 
    /// und logischen Lagerbuchungen.
    /// 
    /// Die verschiedenen Buchungsmethoden bekommen, keine Einzelparameter, sondern immer eine 
    /// Parameterklasse "BookingParameter" übergeben, da die Anzahl der Parameter recht unübersichtlich würde. 
    /// Für kundenspezifische Erweiterungen gibt es eine XML-Config-Property, welche beliebig bestückt werden
    /// kann.
    /// 
    /// Aufteilung in partielle Klassen/Dateien:
    /// 1. FacilityBookingManager.cs            Basisfunktionalität
    /// 2. FacilityBookingManagerBooking.cs     Buchungsfunktionen, die im Tagesgeschäft verwendet werden
    /// 3. FacilityBookingManagerCorrection.cs  Korrekturbuchungen und Inventur
    /// 4. FacilityBookingManagerMaintenance.cs Wartungsfunktionen und Abschlüsse
    /// 
    /// 
    /// 
    /// Version 1
    /// TODO: Noch fertig implementieren
    ///  1 FacilityBooking (Lagerbewegung)
    ///  2. FacilityLotTrace (ChargenPlatz)
    ///  3. FacilityReservation (Reservierungen)
    ///  
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public partial class FacilityManager : PARole, IFacilityManager
    {

        #region Properties

        /// <summary>Returns the shared Database-Context for BSO's by calling GetAppContextForBSO()</summary>
        /// <value>Returns the shared Database-Context.</value>
        public override IACEntityObjectContext Database
        {
            get
            {
                return this.GetAppContextForBSO();
            }
        }

        public override bool IsPoolable
        {
            get
            {
                // TODO: Auf true setzen wenn alle Memeber sauber zurückgesetzt sind
                return true;
            }
        }

        /// <summary>
        /// Returns LocalServiceObjects-Instance if Facility-Manager is child of LocalServiceObjects
        /// </summary>
        public LocalServiceObjects LocalServiceObjects
        {
            get
            {
                return FindParentComponent<LocalServiceObjects>(c => c is LocalServiceObjects);
            }
        }

        public ACDelegateQueue ApplicationQueue
        {
            get
            {
                var lso = LocalServiceObjects;
                if (lso != null)
                    return LocalServiceObjects.ApplicationQueue;
                return null;
            }
        }

        private ACMonitorObject _40010_ValueLock = new ACMonitorObject(40010);
        protected gip.core.datamodel.ACClass _TypeOfACMethodBooking = null;
        public virtual core.datamodel.ACClass TypeOfACMethodBooking
        {
            get
            {
                using (ACMonitor.Lock(_40010_ValueLock))
                {
                    if (_TypeOfACMethodBooking != null)
                        return _TypeOfACMethodBooking;
                }

                gip.core.datamodel.ACClass typeOfBooking = null;
                string acMethodBooking_ClassName = typeof(ACMethodBooking).Name;
                using (ACMonitor.Lock(core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                {
                    typeOfBooking = core.datamodel.Database.GlobalDatabase.ACClass.Where(c => c.ACIdentifier == acMethodBooking_ClassName).FirstOrDefault();
                }

                using (ACMonitor.Lock(_40010_ValueLock))
                {
                    _TypeOfACMethodBooking = typeOfBooking;
                    return _TypeOfACMethodBooking;
                }
            }
        }

        #endregion

        #region Constructors

        static FacilityManager()
        {
            #region FacilityCharge

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementFacilityChargeMethod(GlobalApp.FBT_InwardMovement_FacilityCharge, GlobalApp.FacilityBookingType.InwardMovement_FacilityCharge));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementFacilityChargeMethod(GlobalApp.FBT_InwardMovement_FacilityCharge, GlobalApp.FacilityBookingType.InwardMovement_FacilityCharge));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementFacilityChargeMethod(GlobalApp.FBT_OutwardMovement_FacilityCharge, GlobalApp.FacilityBookingType.OutwardMovement_FacilityCharge));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualRelocationFacilityChargeMethod(GlobalApp.FBT_Relocation_FacilityCharge, GlobalApp.FacilityBookingType.Relocation_FacilityCharge));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualRelocationFacilityChargeMethod(GlobalApp.FBT_Relocation_FacilityCharge_Facility, GlobalApp.FacilityBookingType.Relocation_FacilityCharge_Facility));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualRelocationFacilityChargeMethod(GlobalApp.FBT_Relocation_FacilityCharge_FacilityLocation, GlobalApp.FacilityBookingType.Relocation_FacilityCharge_FacilityLocation));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualStateFacilityChargeMethod(GlobalApp.FBT_ZeroStock_FacilityCharge, GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualStateFacilityChargeMethod(GlobalApp.FBT_ReleaseState_FacilityCharge, GlobalApp.FacilityBookingType.ReleaseState_FacilityCharge));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualReassignFacilityChargeMethod(GlobalApp.FBT_Reassign_FacilityCharge, GlobalApp.FacilityBookingType.Reassign_FacilityCharge));

            #endregion

            #region BulkMaterial

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementBulkMaterialMethod(GlobalApp.FBT_InwardMovement_Facility_BulkMaterial, GlobalApp.FacilityBookingType.InwardMovement_Facility_BulkMaterial));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementBulkMaterialMethod(GlobalApp.FBT_OutwardMovement_Facility_BulkMaterial, GlobalApp.FacilityBookingType.OutwardMovement_Facility_BulkMaterial));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualRelocationBulkMaterialMethod(GlobalApp.FBT_Relocation_Facility_BulkMaterial, GlobalApp.FacilityBookingType.Relocation_Facility_BulkMaterial));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualStateBulkMaterialMethod(GlobalApp.FBT_ZeroStock_Facility_BulkMaterial, GlobalApp.FacilityBookingType.ZeroStock_Facility_BulkMaterial));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualStateBulkMaterialMethod(GlobalApp.FBT_ReleaseState_Facility_BulkMaterial, GlobalApp.FacilityBookingType.ReleaseState_Facility_BulkMaterial));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualReassignBulkMaterialMethod(GlobalApp.FBT_Reassign_Facility_BulkMaterial, GlobalApp.FacilityBookingType.Reassign_Facility_BulkMaterial));

            #endregion

            #region Simple

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingFacilityChargeQuantities, GlobalApp.FacilityBookingType.MatchingFacilityChargeQuantities));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingFacilityChargeQuantitiesAll, GlobalApp.FacilityBookingType.MatchingFacilityChargeQuantitiesAll));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingMaterialStock, GlobalApp.FacilityBookingType.MatchingMaterialStock));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingMaterialStockAll, GlobalApp.FacilityBookingType.MatchingMaterialStockAll));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingFacilityStock, GlobalApp.FacilityBookingType.MatchingFacilityStock));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingFacilityStockAll, GlobalApp.FacilityBookingType.MatchingFacilityStockAll));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingFacilityLotStock, GlobalApp.FacilityBookingType.MatchingFacilityLotStock));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingFacilityLotStockAll, GlobalApp.FacilityBookingType.MatchingFacilityLotStockAll));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingPartslistStock, GlobalApp.FacilityBookingType.MatchingPartslistStock));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingPartslistStockAll, GlobalApp.FacilityBookingType.MatchingPartslistStockAll));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingStockAll, GlobalApp.FacilityBookingType.MatchingStockAll));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_ClosingDay, GlobalApp.FacilityBookingType.ClosingDay));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_ClosingWeek, GlobalApp.FacilityBookingType.ClosingWeek));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_ClosingMonth, GlobalApp.FacilityBookingType.ClosingMonth));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_ClosingYear, GlobalApp.FacilityBookingType.ClosingYear));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_InventoryNew, GlobalApp.FacilityBookingType.InventoryNew));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_InventoryStockCorrection, GlobalApp.FacilityBookingType.InventoryStockCorrection));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_InventoryClose, GlobalApp.FacilityBookingType.InventoryClose));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualNewQuantMethod(GlobalApp.FBT_InventoryNewQuant, GlobalApp.FacilityBookingType.InventoryNewQuant));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_InOrderPosActivate, GlobalApp.FacilityBookingType.InOrderPosActivate));

            #endregion

            #region OrderPos

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_InOrderPosInwardMovement, GlobalApp.FacilityBookingType.InOrderPosInwardMovement));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_InOrderPosCancel, GlobalApp.FacilityBookingType.InOrderPosCancel));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_OutOrderPosOutwardMovement, GlobalApp.FacilityBookingType.OutOrderPosOutwardMovement));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_OutOrderPosCancel, GlobalApp.FacilityBookingType.OutOrderPosCancel));

            // Simple methods
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_OutOrderPosActivate, GlobalApp.FacilityBookingType.OutOrderPosActivate));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_ChangeStorageUnit, GlobalApp.FacilityBookingType.ChangeStorageUnit));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_StockCorrection, GlobalApp.FacilityBookingType.StockCorrection));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_CorrectionCostRateAll, GlobalApp.FacilityBookingType.CorrectionCostRateAll));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_CorrectionCostRateMaterial, GlobalApp.FacilityBookingType.CorrectionCostRateMaterial));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingCostRateMaterialInwardFacilityChargeAll, GlobalApp.FacilityBookingType.MatchingCostRateMaterialInwardFacilityChargeAll));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualSimpleMethod(GlobalApp.FBT_MatchingCostRateMaterialInwardFacilityCharge, GlobalApp.FacilityBookingType.MatchingCostRateMaterialInwardFacilityCharge));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_ProdOrderPosInward, GlobalApp.FacilityBookingType.ProdOrderPosInward));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_ProdOrderPosInwardCancel, GlobalApp.FacilityBookingType.ProdOrderPosInwardCancel));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_ProdOrderPosOutward, GlobalApp.FacilityBookingType.ProdOrderPosOutward));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_ProdOrderPosOutwardCancel, GlobalApp.FacilityBookingType.ProdOrderPosOutwardCancel));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_ProdOrderPosOutwardOnEmptyingFacility, GlobalApp.FacilityBookingType.ProdOrderPosOutwardOnEmptyingFacility));

            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_PickingInward, GlobalApp.FacilityBookingType.PickingInward));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_PickingInwardCancel, GlobalApp.FacilityBookingType.PickingInwardCancel));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_PickingOutward, GlobalApp.FacilityBookingType.PickingOutward));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualMovementOrderPosMethod(GlobalApp.FBT_PickingOutwardCancel, GlobalApp.FacilityBookingType.PickingOutwardCancel));
            ACMethod.RegisterVirtualMethod(typeof(FacilityManager), "BookFacility", CreateVirtualRelocationBulkMaterialMethod(GlobalApp.FBT_PickingRelocation, GlobalApp.FacilityBookingType.PickingRelocation));

            #endregion
        }

        public FacilityManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            CreateConfigParams();
            // Da FacilityManager-Instanz unter den Local-ServiceObjects instanziiert wird,
            // wird hier die Application-Datenbank  beim Startvorgangh einmal erzeugt und einmal gespeichert, 
            // damit später an der Obberfläche das erstmalige Öffnen eine BSO's beschleunigt wird.
            DatabaseApp.InitializeDBOnStartup();
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            InitConfigParams();
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACPartslistManager.DetachACRefFromServiceInstance<ACPartslistManager>(this, _PartslistManager);
            _PartslistManager = null;
            bool result = base.ACDeInit(deleteACClassTask);
            return result;
        }

        public override bool ACPostInit()
        {
            bool init = base.ACPostInit();
            _PartslistManager = ACPartslistManager.ACRefToServiceInstance(this);
            return init;
        }

        #endregion

        #region Public
        /// <summary>
        /// Call BookFacility method with more retries
        /// </summary>
        [ACMethodInfo("BookFacilityWithRetry", "en{'BookFacilityWithRetry'}de{'BookFacilityWithRetry'}", 9999)]
        public ACMethodEventArgs BookFacilityWithRetry(ref ACMethodBooking bookingParam, DatabaseApp dbApp, bool retryIfNotPossible = true)
        {
            ACMethodEventArgs bookingResult = null;
            ACMethodBooking bookingParamClone = bookingParam.Clone() as ACMethodBooking;
            for (int i = 0; i < ACObjectContextHelper.C_NumberOfRetriesOnTransError; i++)
            {
                bookingResult = BookFacility(bookingParam, dbApp);
                if (bookingResult.ResultState == Global.ACMethodResultState.Failed
                    || bookingResult.ResultState == Global.ACMethodResultState.Notpossible)
                {
                    if (dbApp != null)
                        dbApp.ACUndoChanges();
                    if (bookingResult.ResultState == Global.ACMethodResultState.Notpossible && !retryIfNotPossible)
                        return bookingResult;
                    bookingParam = bookingParamClone.Clone() as ACMethodBooking;
                    Thread.Sleep(ACObjectContextHelper.C_TimeoutForRetryAfterTransError);
                }
                else
                    break;
            }
            return bookingResult;
        }

        /// <summary>
        /// Zentrale Buchungsfunktion
        /// Vorgehensweise:
        /// 1. Alle Parameter werden über die Klasse BookingParameter übergeben
        /// 2. Als Rückgabe wird BookingResult zurückgegeben (Keine externen Exceptions)
        ///     -Notpossible,   // Falls falsche Parameteruebergabe
        ///     -Failed = 0,    // Buchungs fehlgeschlagen 
        ///     -Succeeded = 1, // Erfolgreich ausgeführt
        /// 3. Unabhängig vom Rückgabewert kann die "BookingMessage" abgefragt werden,
        ///    der Buchungsinformationen und/oder Fehlermeldungen enthält.
        ///    
        ///  Mittels der "BookingMessage" erfährt der Programmierer auch, welche Parameter er evtl. noch
        ///  versorgen muss.
        ///
        /// Beispielaufruf:
        /// Instanz der Buchungklasse erzeugen (evtl. eine Instanz für die gesamte Laufzeit des Kontextes)
        /// FacilityBookingManager _FacilityManager;
        /// _FacilityManager = new FacilityBookingManager(Database);
        /// </summary>
        [ACMethodInfo("", "", 9999)]
        public ACMethodEventArgs BookFacility(ACMethodBooking BP, DatabaseApp dbApp)
        {
            // TODO: Replace _ACMethodBooking and othe Memebers with a kind TransactionToken, so that BookFacility could be executed concurrently
            try
            {
                if (BP == null || dbApp == null)
                    return new ACMethodEventArgs(BP, Global.ACMethodResultState.Notpossible);
                BP.Database = dbApp;

                // Überprüfe Syntax der übergebenen Parameter
                if (!BP.IsValid())
                {
                    dbApp.ACUndoChanges();
                    return new ACMethodEventArgs(BP, Global.ACMethodResultState.Notpossible);
                }
                if (!BP.CheckAndAdjustPropertiesForBooking(dbApp))
                {
                    dbApp.ACUndoChanges();
                    return new ACMethodEventArgs(BP, Global.ACMethodResultState.Notpossible);
                }

                // Kundenspezifiche Funktion vor eigentlicher Buchung ausführen (Quellcode wird in der Datenbank hinterlegt)
                if (!PreFacilityBooking(BP))
                {
                    dbApp.ACUndoChanges();
                    return new ACMethodEventArgs(BP, Global.ACMethodResultState.Notpossible);
                }

                // Standard-Buchungsfunktion ausführen
                Global.ACMethodResultState result = DoFacilityBooking(BP);
                if (result == Global.ACMethodResultState.Notpossible || result == Global.ACMethodResultState.Failed)
                {
                    dbApp.ACUndoChanges();
                    return new ACMethodEventArgs(BP, result);
                }

                // Kundenspezifiche Funktion nach eigentlicher Buchung ausführen (Quellcode wird in der Datenbank hinterlegt)
                if (!PostFacilityBooking(BP))
                {
                    dbApp.ACUndoChanges();
                    return new ACMethodEventArgs(BP, Global.ACMethodResultState.Failed);
                }
                if (result != Global.ACMethodResultState.Succeeded)
                {
                    dbApp.ACUndoChanges();
                    return new ACMethodEventArgs(BP, result);
                }

                // Commit auf Datenbank
                MsgWithDetails saveMsg = dbApp.ACSaveChanges();
                if (saveMsg != null)
                {
                    BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, saveMsg.InnerMessage);
                    result = Global.ACMethodResultState.Failed;
                    return new ACMethodEventArgs(BP, result);
                }

                PostFacilityBookingSaved(BP);

                return new ACMethodEventArgs(BP, result);
            }
            catch (Exception e)
            {
                dbApp.ACUndoChanges();
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.Message);
                if (e.InnerException != null && !String.IsNullOrEmpty(e.InnerException.Message))
                    BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.InnerException.Message);
                return new ACMethodEventArgs(BP, Global.ACMethodResultState.Failed);
            }
        }


        /// <summary>
        /// Löscht eine Charge. Überprüft jedoch ob zuvor die Charge auf Nullbestand gebucht worden ist
        /// Ansonsten gibt es eine Fehlermeldung
        /// </summary>
        public Global.ACMethodResultState DeleteFacilityCharge(ACMethodBooking BP, FacilityCharge facilityCharge, bool setStockZeroAndNotAvailable, DatabaseApp dbApp)
        {
            BP.Database = dbApp;
            Global.ACMethodResultState bResult = Global.ACMethodResultState.Succeeded;
            if (facilityCharge == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.RequiredParamsNotSet, Root.Environment.TranslateMessage(this, "Error00058"));
                return Global.ACMethodResultState.Notpossible;
            }
           
            MsgWithDetails msgWithDetails = facilityCharge.DeleteACObject(dbApp, false);
            if (!msgWithDetails.IsSucceded())
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.RequiredParamsNotSet, msgWithDetails.InnerMessage);
            }
           
            return bResult;
        }

        #endregion

        #region Private
        /// <summary>
        /// TOOD: 
        /// </summary>
        protected virtual bool PreFacilityBooking(ACMethodBooking acMethodBooking)
        {
            return true;
        }

        /// <summary>
        /// TOOD: 
        /// </summary>
        protected virtual bool PostFacilityBooking(ACMethodBooking acMethodBooking)
        {
            return true;
        }

        protected virtual void PostFacilityBookingSaved(ACMethodBooking acMethodBooking)
        {
        }

        #region FacilityCharge

        private static ACMethodWrapper CreateVirtualMovementFacilityChargeMethod(string AcIdentifier, GlobalApp.FacilityBookingType BookingType)
        {
            string Prefix;
            ACMethodBooking TMP;

            if (AcIdentifier.StartsWith("In"))
                Prefix = "In";
            else if (AcIdentifier.StartsWith("Out"))
                Prefix = "Out";
            else
                throw new ArgumentException("Virtual movement identifier doesn't start with 'In' or 'Out'.", "AcIdentifier");

            TMP = new ACMethodBooking();
            TMP.ACIdentifier = AcIdentifier;

            TMP.ParameterValueList.Add(new ACValue("BookingType", typeof(GlobalApp.FacilityBookingType), BookingType, Global.ParamOption.Fix));
            TMP.ParameterValueList.Add(new ACValue(MDMovementReason.ClassName, typeof(MDMovementReason), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardMaterial", typeof(Material), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardFacilityCharge", typeof(FacilityCharge), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardPartslist", typeof(ProdOrderPartslist), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardCompanyMaterial", typeof(CompanyMaterial), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardHandlingUnit", typeof(HandlingUnit), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardStackBookingModel", typeof(core.datamodel.ACClass), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardQuantity", typeof(Nullable<double>), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardTargetQuantity", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardTargetQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue(MDUnit.ClassName, typeof(MDUnit), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBookingNotAvailableMode.ClassName, typeof(MDBookingNotAvailableMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("DontAllowNegativeStock", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("IgnoreManagement", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityIsAbsolute", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBalancingMode.ClassName, typeof(MDBalancingMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityParamsNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("InwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("StorageDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("ProductionDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("ExpirationDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("MinimumDurability", typeof(Nullable<Int32>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("RecipeOrFactoryInfo", typeof(string), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("PropertyACUrl", typeof(string), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("Comment", typeof(string), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("CPartnerCompany", typeof(Company), null, Global.ParamOption.Optional));

            TMP.ResultValueList.Add(new ACValue("BookingResult", typeof(ACMethodEventArgs), null, Global.ParamOption.Required));

            return new ACMethodWrapper(TMP, GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)BookingType).ACCaptionTranslation, null);
        }

        private static ACMethodWrapper CreateVirtualRelocationFacilityChargeMethod(string AcIdentitifer, GlobalApp.FacilityBookingType BookingType)
        {
            ACMethodBooking TMP = new ACMethodBooking();

            TMP.ACIdentifier = AcIdentitifer;

            TMP.ParameterValueList.Add(new ACValue("BookingType", typeof(GlobalApp.FacilityBookingType), BookingType, Global.ParamOption.Fix));
            TMP.ParameterValueList.Add(new ACValue(MDMovementReason.ClassName, typeof(MDMovementReason), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("InwardMaterial", typeof(Material), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardFacilityCharge", typeof(FacilityCharge), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardStackBookingModel", typeof(core.datamodel.ACClass), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("OutwardMaterial", typeof(Material), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("OutwardFacilityCharge", typeof(FacilityCharge), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("OutwardStackBookingModel", typeof(core.datamodel.ACClass), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("InwardQuantity", typeof(Nullable<double>), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("InwardTargetQuantity", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardTargetQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("OutwardQuantity", typeof(Nullable<double>), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("OutwardTargetQuantity", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("OutwardQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("OutwardTargetQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue(MDUnit.ClassName, typeof(MDUnit), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBookingNotAvailableMode.ClassName, typeof(MDBookingNotAvailableMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("DontAllowNegativeStock", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("IgnoreManagement", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityIsAbsolute", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBalancingMode.ClassName, typeof(MDBalancingMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityParamsNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("InwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));
            TMP.ParameterValueList.Add(new ACValue("OutwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("StorageDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("ProductionDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("ExpirationDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("MinimumDurability", typeof(Nullable<Int32>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("RecipeOrFactoryInfo", typeof(string), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("PropertyACUrl", typeof(string), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("Comment", typeof(string), null, Global.ParamOption.Optional));

            TMP.ResultValueList.Add(new ACValue("BookingResult", typeof(ACMethodEventArgs), null, Global.ParamOption.Required));

            return new ACMethodWrapper(TMP, GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)BookingType).ACCaptionTranslation, null);
        }

        private static ACMethodWrapper CreateVirtualStateFacilityChargeMethod(string AcIdentitifer, GlobalApp.FacilityBookingType BookingType)
        {
            ACMethodBooking TMP = new ACMethodBooking();

            TMP.ACIdentifier = AcIdentitifer;

            TMP.ParameterValueList.Add(new ACValue("BookingType", typeof(GlobalApp.FacilityBookingType), BookingType, Global.ParamOption.Fix));

            if (BookingType == GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge)
                TMP.ParameterValueList.Add(new ACValue(MDZeroStockState.ClassName, typeof(MDZeroStockState), null, Global.ParamOption.Required));
            else
                TMP.ParameterValueList.Add(new ACValue(MDReleaseState.ClassName, typeof(MDReleaseState), null, Global.ParamOption.Required));

            TMP.ParameterValueList.Add(new ACValue(MDMovementReason.ClassName, typeof(MDMovementReason), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("InwardMaterial", typeof(Material), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardFacilityCharge", typeof(FacilityCharge), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("InwardStackBookingModel", typeof(core.datamodel.ACClass), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue(MDBookingNotAvailableMode.ClassName, typeof(MDBookingNotAvailableMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("DontAllowNegativeStock", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("IgnoreManagement", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityIsAbsolute", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBalancingMode.ClassName, typeof(MDBalancingMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityParamsNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("InwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("Comment", typeof(string), null, Global.ParamOption.Optional));

            if (BookingType == GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge)
                TMP.ParameterValueList.Add(new ACValue("CPartnerCompany", typeof(Company), null, Global.ParamOption.Optional));


            TMP.ResultValueList.Add(new ACValue("BookingResult", typeof(ACMethodEventArgs), null, Global.ParamOption.Required));

            return new ACMethodWrapper(TMP, GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)BookingType).ACCaptionTranslation, null);
        }


        private static ACMethodWrapper CreateVirtualReassignFacilityChargeMethod(string AcIdentitifer, GlobalApp.FacilityBookingType BookingType)
        {
            ACMethodBooking TMP = new ACMethodBooking();

            TMP.ACIdentifier = AcIdentitifer;

            TMP.ParameterValueList.Add(new ACValue("BookingType", typeof(GlobalApp.FacilityBookingType), BookingType, Global.ParamOption.Fix));
            TMP.ParameterValueList.Add(new ACValue(MDMovementReason.ClassName, typeof(MDMovementReason), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("OutwardFacilityCharge", typeof(FacilityCharge), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("InwardMaterial", typeof(Material), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("InwardPartslist", typeof(Partslist), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityParamsNeeded", typeof(bool), false, Global.ParamOption.Fix));
            TMP.ParameterValueList.Add(new ACValue("Comment", typeof(string), null, Global.ParamOption.Optional));
            TMP.ResultValueList.Add(new ACValue("BookingResult", typeof(ACMethodEventArgs), null, Global.ParamOption.Required));

            return new ACMethodWrapper(TMP, GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)BookingType).ACCaptionTranslation, null);
        }

        #endregion

        #region BulkMaterial

        private static ACMethodWrapper CreateVirtualMovementBulkMaterialMethod(string AcIdentifier, GlobalApp.FacilityBookingType BookingType)
        {
            string Prefix;
            ACMethodBooking TMP;

            if (AcIdentifier.StartsWith("In"))
                Prefix = "In";
            else if (AcIdentifier.StartsWith("Out"))
                Prefix = "Out";
            else
                throw new ArgumentException("Virtual movement identifier doesn't start with 'In' or 'Out'.", "AcIdentifier");

            TMP = new ACMethodBooking();
            TMP.ACIdentifier = AcIdentifier;

            TMP.ParameterValueList.Add(new ACValue("BookingType", typeof(GlobalApp.FacilityBookingType), BookingType, Global.ParamOption.Fix));
            TMP.ParameterValueList.Add(new ACValue("ShiftBookingReverse", typeof(bool), false, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue(MDMovementReason.ClassName, typeof(MDMovementReason), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardMaterial", typeof(Material), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardFacility", typeof(Facility), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardFacilityLot", typeof(FacilityLot), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardStackBookingModel", typeof(core.datamodel.ACClass), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardQuantity", typeof(Nullable<double>), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardTargetQuantity", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardTargetQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue(MDUnit.ClassName, typeof(MDUnit), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBookingNotAvailableMode.ClassName, typeof(MDBookingNotAvailableMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("DontAllowNegativeStock", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("IgnoreManagement", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityIsAbsolute", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBalancingMode.ClassName, typeof(MDBalancingMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityParamsNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("InwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("StorageDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("ProductionDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("ExpirationDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("MinimumDurability", typeof(Nullable<Int32>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("RecipeOrFactoryInfo", typeof(string), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("PropertyACUrl", typeof(string), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("Comment", typeof(string), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("CPartnerCompany", typeof(Company), null, Global.ParamOption.Optional));

            TMP.ResultValueList.Add(new ACValue("BookingResult", typeof(ACMethodEventArgs), null, Global.ParamOption.Required));

            return new ACMethodWrapper(TMP, GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)BookingType).ACCaptionTranslation, null);
        }

        private static ACMethodWrapper CreateVirtualRelocationBulkMaterialMethod(string AcIdentitifer, GlobalApp.FacilityBookingType BookingType)
        {
            ACMethodBooking TMP = new ACMethodBooking();

            TMP.ACIdentifier = AcIdentitifer;

            TMP.ParameterValueList.Add(new ACValue("BookingType", typeof(GlobalApp.FacilityBookingType), BookingType, Global.ParamOption.Fix));
            TMP.ParameterValueList.Add(new ACValue("ShiftBookingReverse", typeof(bool), false, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDMovementReason.ClassName, typeof(MDMovementReason), null, Global.ParamOption.Optional));

            if (BookingType == GlobalApp.FacilityBookingType.PickingRelocation)
                TMP.ParameterValueList.Add(new ACValue(PickingPos.ClassName, typeof(PickingPos), null, Global.ParamOption.Required));

            TMP.ParameterValueList.Add(new ACValue("InwardMaterial", typeof(Material), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardFacility", typeof(Facility), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("InwardFacilityLot", typeof(FacilityLot), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardStackBookingModel", typeof(core.datamodel.ACClass), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardFacilityCharge", typeof(FacilityCharge), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("OutwardMaterial", typeof(Material), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("OutwardFacility", typeof(Facility), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("OutwardFacilityLot", typeof(FacilityLot), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("OutwardStackBookingModel", typeof(core.datamodel.ACClass), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("OutwardFacilityCharge", typeof(FacilityCharge), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("InwardQuantity", typeof(Nullable<double>), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("InwardTargetQuantity", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardTargetQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("OutwardQuantity", typeof(Nullable<double>), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("OutwardTargetQuantity", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("OutwardQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("OutwardTargetQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue(MDUnit.ClassName, typeof(MDUnit), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBookingNotAvailableMode.ClassName, typeof(MDBookingNotAvailableMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("DontAllowNegativeStock", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("IgnoreManagement", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityIsAbsolute", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBalancingMode.ClassName, typeof(MDBalancingMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityParamsNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("InwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));
            TMP.ParameterValueList.Add(new ACValue("OutwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("StorageDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("ProductionDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("ExpirationDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("MinimumDurability", typeof(Nullable<Int32>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("RecipeOrFactoryInfo", typeof(string), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("PropertyACUrl", typeof(string), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("Comment", typeof(string), null, Global.ParamOption.Optional));

            TMP.ResultValueList.Add(new ACValue("BookingResult", typeof(ACMethodEventArgs), null, Global.ParamOption.Required));

            return new ACMethodWrapper(TMP, GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)BookingType).ACCaptionTranslation, null);
        }

        private static ACMethodWrapper CreateVirtualStateBulkMaterialMethod(string AcIdentitifer, GlobalApp.FacilityBookingType BookingType)
        {
            ACMethodBooking TMP = new ACMethodBooking();

            TMP.ACIdentifier = AcIdentitifer;

            TMP.ParameterValueList.Add(new ACValue("BookingType", typeof(GlobalApp.FacilityBookingType), BookingType, Global.ParamOption.Fix));

            if (BookingType == GlobalApp.FacilityBookingType.ZeroStock_Facility_BulkMaterial)
            {
                TMP.ParameterValueList.Add(new ACValue(MDZeroStockState.ClassName, typeof(MDZeroStockState), null, Global.ParamOption.Required));
                TMP.ParameterValueList.Add(new ACValue(MDMovementReason.ClassName, typeof(MDMovementReason), null, Global.ParamOption.Optional));
            }
            else
                TMP.ParameterValueList.Add(new ACValue(MDReleaseState.ClassName, typeof(MDReleaseState), null, Global.ParamOption.Required));

            TMP.ParameterValueList.Add(new ACValue("InwardMaterial", typeof(Material), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardFacility", typeof(Facility), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("InwardStackBookingModel", typeof(core.datamodel.ACClass), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue(MDBookingNotAvailableMode.ClassName, typeof(MDBookingNotAvailableMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("DontAllowNegativeStock", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("IgnoreManagement", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityIsAbsolute", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBalancingMode.ClassName, typeof(MDBalancingMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityParamsNeeded", typeof(bool), false, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("InwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("Comment", typeof(string), null, Global.ParamOption.Optional));

            if (BookingType == GlobalApp.FacilityBookingType.ZeroStock_Facility_BulkMaterial)
                TMP.ParameterValueList.Add(new ACValue("CPartnerCompany", typeof(Company), null, Global.ParamOption.Optional));

            TMP.ResultValueList.Add(new ACValue("BookingResult", typeof(ACMethodEventArgs), null, Global.ParamOption.Required));

            return new ACMethodWrapper(TMP, GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)BookingType).ACCaptionTranslation, null);
        }

        private static ACMethodWrapper CreateVirtualReassignBulkMaterialMethod(string AcIdentitifer, GlobalApp.FacilityBookingType BookingType)
        {
            ACMethodBooking TMP = new ACMethodBooking();

            TMP.ACIdentifier = AcIdentitifer;

            TMP.ParameterValueList.Add(new ACValue("BookingType", typeof(GlobalApp.FacilityBookingType), BookingType, Global.ParamOption.Fix));
            TMP.ParameterValueList.Add(new ACValue(MDMovementReason.ClassName, typeof(MDMovementReason), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("OutwardFacility", typeof(Facility),  null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("InwardMaterial", typeof(Material), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("InwardPartslist", typeof(Partslist), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityParamsNeeded", typeof(bool), false, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("Comment", typeof(string), null, Global.ParamOption.Optional));
            TMP.ResultValueList.Add(new ACValue("BookingResult", typeof(ACMethodEventArgs), null, Global.ParamOption.Required));

            return new ACMethodWrapper(TMP, GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)BookingType).ACCaptionTranslation, null);
        }

        #endregion

        #region Simple

        private static ACMethodWrapper CreateVirtualSimpleMethod(string AcIdentitifer, GlobalApp.FacilityBookingType BookingType)
        {
            ACMethodBooking TMP = new ACMethodBooking();

            TMP.ACIdentifier = AcIdentitifer;

            TMP.ParameterValueList.Add(new ACValue("BookingType", typeof(GlobalApp.FacilityBookingType), BookingType, Global.ParamOption.Fix));

            switch (BookingType)
            {
                case GlobalApp.FacilityBookingType.MatchingFacilityChargeQuantitiesAll:
                case GlobalApp.FacilityBookingType.MatchingMaterialStockAll:
                case GlobalApp.FacilityBookingType.MatchingFacilityStockAll:
                case GlobalApp.FacilityBookingType.MatchingFacilityLotStockAll:
                case GlobalApp.FacilityBookingType.MatchingPartslistStockAll:
                case GlobalApp.FacilityBookingType.MatchingStockAll:
                case GlobalApp.FacilityBookingType.ClosingDay:
                case GlobalApp.FacilityBookingType.ClosingWeek:
                case GlobalApp.FacilityBookingType.ClosingMonth:
                case GlobalApp.FacilityBookingType.ClosingYear:
                case GlobalApp.FacilityBookingType.InventoryNew:
                case GlobalApp.FacilityBookingType.InventoryStockCorrection:
                case GlobalApp.FacilityBookingType.InventoryClose:
                case GlobalApp.FacilityBookingType.ChangeStorageUnit:
                case GlobalApp.FacilityBookingType.StockCorrection:
                case GlobalApp.FacilityBookingType.CorrectionCostRateAll:
                case GlobalApp.FacilityBookingType.CorrectionCostRateMaterial:
                case GlobalApp.FacilityBookingType.MatchingCostRateMaterialInwardFacilityChargeAll:
                case GlobalApp.FacilityBookingType.MatchingCostRateMaterialInwardFacilityCharge:
                case GlobalApp.FacilityBookingType.InOrderPosActivate:
                case GlobalApp.FacilityBookingType.OutOrderPosActivate:
                    break;

                case GlobalApp.FacilityBookingType.MatchingFacilityChargeQuantities:
                    TMP.ParameterValueList.Add(new ACValue("InwardFacilityCharge", typeof(FacilityCharge), null, Global.ParamOption.Required));
                    TMP.ParameterValueList.Add(new ACValue("InwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));
                    break;

                case GlobalApp.FacilityBookingType.MatchingMaterialStock:
                    TMP.ParameterValueList.Add(new ACValue("InwardMaterial", typeof(Material), null, Global.ParamOption.Required));
                    TMP.ParameterValueList.Add(new ACValue("InwardStackBookingModel", typeof(core.datamodel.ACClass), null, Global.ParamOption.Optional));
                    TMP.ParameterValueList.Add(new ACValue("InwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));
                    break;

                case GlobalApp.FacilityBookingType.MatchingFacilityStock:
                    TMP.ParameterValueList.Add(new ACValue("InwardFacility", typeof(Facility), null, Global.ParamOption.Required));
                    TMP.ParameterValueList.Add(new ACValue("InwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));
                    break;

                case GlobalApp.FacilityBookingType.MatchingFacilityLotStock:
                    TMP.ParameterValueList.Add(new ACValue("InwardFacilityLot", typeof(FacilityLot), null, Global.ParamOption.Required));
                    TMP.ParameterValueList.Add(new ACValue("InwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));
                    break;

                case GlobalApp.FacilityBookingType.MatchingPartslistStock:
                    TMP.ParameterValueList.Add(new ACValue("InwardPartslist", typeof(Partslist), null, Global.ParamOption.Required));
                    TMP.ParameterValueList.Add(new ACValue("InwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));
                    break;

                default:
                    throw new Exception("Unnown booking type '" + BookingType.ToString() + "'.");
            }

            TMP.ParameterValueList.Add(new ACValue("Comment", typeof(string), null, Global.ParamOption.Optional));


            TMP.ResultValueList.Add(new ACValue("BookingResult", typeof(ACMethodEventArgs), null, Global.ParamOption.Required));

            return new ACMethodWrapper(TMP, GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)BookingType).ACCaptionTranslation, null);
        }

        private static ACMethodWrapper CreateVirtualNewQuantMethod(string AcIdentifier, GlobalApp.FacilityBookingType BookingType)
        {
            ACMethodBooking TMP;

            TMP = new ACMethodBooking();
            TMP.ACIdentifier = AcIdentifier;

            TMP.ParameterValueList.Add(new ACValue("BookingType", typeof(GlobalApp.FacilityBookingType), BookingType, Global.ParamOption.Fix));
            TMP.ParameterValueList.Add(new ACValue("ShiftBookingReverse", typeof(bool), false, Global.ParamOption.NotRequired));
            TMP.ParameterValueList.Add(new ACValue(MDMovementReason.ClassName, typeof(MDMovementReason), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("InwardMaterial", typeof(Material), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("InwardFacility", typeof(Facility), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("InwardFacilityLot", typeof(FacilityLot), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardStackBookingModel", typeof(core.datamodel.ACClass), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardQuantity", typeof(Nullable<double>), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue("InwardTargetQuantity", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("InwardTargetQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue(MDUnit.ClassName, typeof(MDUnit), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBookingNotAvailableMode.ClassName, typeof(MDBookingNotAvailableMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("DontAllowNegativeStock", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("IgnoreManagement", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityIsAbsolute", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBalancingMode.ClassName, typeof(MDBalancingMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityParamsNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("InwardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("StorageDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("ProductionDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("ExpirationDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("MinimumDurability", typeof(Nullable<Int32>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("RecipeOrFactoryInfo", typeof(string), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("PropertyACUrl", typeof(string), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("Comment", typeof(string), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("CPartnerCompany", typeof(Company), null, Global.ParamOption.Optional));

            TMP.ResultValueList.Add(new ACValue("BookingResult", typeof(ACMethodEventArgs), null, Global.ParamOption.Required));

            return new ACMethodWrapper(TMP, GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)BookingType).ACCaptionTranslation, null);
        }

        #endregion

        #region OrderPos

        private static ACMethodWrapper CreateVirtualMovementOrderPosMethod(string AcIdentifier, GlobalApp.FacilityBookingType BookingType)
        {
            string Prefix;
            ACMethodBooking TMP;

            TMP = new ACMethodBooking();
            TMP.ACIdentifier = AcIdentifier;

            TMP.ParameterValueList.Add(new ACValue("BookingType", typeof(GlobalApp.FacilityBookingType), BookingType, Global.ParamOption.Fix));

            switch (BookingType)
            {
                case GlobalApp.FacilityBookingType.InOrderPosInwardMovement:
                case GlobalApp.FacilityBookingType.InOrderPosCancel:
                    TMP.ParameterValueList.Add(new ACValue(InOrderPos.ClassName, typeof(InOrderPos), null, Global.ParamOption.Required));
                    Prefix = "In";
                    break;

                case GlobalApp.FacilityBookingType.OutOrderPosOutwardMovement:
                case GlobalApp.FacilityBookingType.OutOrderPosCancel:
                    TMP.ParameterValueList.Add(new ACValue(OutOrderPos.ClassName, typeof(OutOrderPos), null, Global.ParamOption.Required));
                    Prefix = "Out";
                    break;

                case GlobalApp.FacilityBookingType.ProdOrderPosInward:
                case GlobalApp.FacilityBookingType.ProdOrderPosInwardCancel:
                    TMP.ParameterValueList.Add(new ACValue("PartslistPos", typeof(PartslistPos), null, Global.ParamOption.Required));
                    TMP.ParameterValueList.Add(new ACValue("ProdOrderPartslistPosFacilityLot", typeof(ProdOrderPartslistPosFacilityLot), null, Global.ParamOption.Optional));
                    Prefix = "In";
                    break;

                case GlobalApp.FacilityBookingType.ProdOrderPosOutward:
                case GlobalApp.FacilityBookingType.ProdOrderPosOutwardCancel:
                case GlobalApp.FacilityBookingType.ProdOrderPosOutwardOnEmptyingFacility:
                    Prefix = "Out";
                    TMP.ParameterValueList.Add(new ACValue("PartslistPosRelation", typeof(PartslistPosRelation), null, Global.ParamOption.Required));
                    TMP.ParameterValueList.Add(new ACValue("ProdOrderPartslistPosFacilityLot", typeof(ProdOrderPartslistPosFacilityLot), null, Global.ParamOption.Optional));
                    break;

                case GlobalApp.FacilityBookingType.PickingInward:
                case GlobalApp.FacilityBookingType.PickingInwardCancel:
                    TMP.ParameterValueList.Add(new ACValue(PickingPos.ClassName, typeof(PickingPos), null, Global.ParamOption.Required));
                    Prefix = "In";
                    break;

                case GlobalApp.FacilityBookingType.PickingOutward:
                case GlobalApp.FacilityBookingType.PickingOutwardCancel:
                    TMP.ParameterValueList.Add(new ACValue(PickingPos.ClassName, typeof(PickingPos), null, Global.ParamOption.Required));
                    Prefix = "Out";
                    break;


                default:
                    throw new ArgumentException("Virtual movement identifier doesn't start with 'In' or 'Out'.", "AcIdentifier");
            }

            TMP.ParameterValueList.Add(new ACValue(MDMovementReason.ClassName, typeof(MDMovementReason), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardMaterial", typeof(Material), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardFacility", typeof(Facility), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardFacilityLot", typeof(FacilityLot), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardSplitNo", typeof(Int32), 0, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardFacilityCharge", typeof(FacilityCharge), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardPartslist", typeof(ProdOrderPartslist), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardCompanyMaterial", typeof(CompanyMaterial), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardHandlingUnit", typeof(HandlingUnit), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardStackBookingModel", typeof(core.datamodel.ACClass), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardQuantity", typeof(Nullable<double>), null, Global.ParamOption.Required));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardTargetQuantity", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardTargetQuantityAmb", typeof(Nullable<double>), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue(MDUnit.ClassName, typeof(MDUnit), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBookingNotAvailableMode.ClassName, typeof(MDBookingNotAvailableMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("DontAllowNegativeStock", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("IgnoreManagement", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityIsAbsolute", typeof(Nullable<bool>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue(MDBalancingMode.ClassName, typeof(MDBalancingMode), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("QuantityParamsNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue(Prefix + "wardFacilityEntitiesNeeded", typeof(bool), true, Global.ParamOption.Fix));

            TMP.ParameterValueList.Add(new ACValue("StorageDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("ProductionDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("ExpirationDate", typeof(Nullable<DateTime>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("MinimumDurability", typeof(Nullable<Int32>), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("RecipeOrFactoryInfo", typeof(string), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("PropertyACUrl", typeof(string), null, Global.ParamOption.Optional));

            TMP.ParameterValueList.Add(new ACValue("Comment", typeof(string), null, Global.ParamOption.Optional));
            TMP.ParameterValueList.Add(new ACValue("CPartnerCompany", typeof(Company), null, Global.ParamOption.Optional));

            TMP.ResultValueList.Add(new ACValue("BookingResult", typeof(ACMethodEventArgs), null, Global.ParamOption.Required));

            return new ACMethodWrapper(TMP, GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)BookingType).ACCaptionTranslation, null);
        }

        #endregion

        #endregion

        #region Temp added - should be arranged into file on appropriate place

        public const string MN_ProdOrderPosInward = GlobalApp.FBT_ProdOrderPosInward;
        public const string MN_ProdOrderPosInwardCancel = GlobalApp.FBT_ProdOrderPosInwardCancel;
        public const string MN_ProdOrderPosOutward = GlobalApp.FBT_ProdOrderPosOutward;
        public const string MN_ProdOrderPosOutwardCancel = GlobalApp.FBT_ProdOrderPosOutwardCancel;
        public const string MN_ProdOrderPosOutwardOnEmptyingFacility = GlobalApp.FBT_ProdOrderPosOutwardOnEmptyingFacility;

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"BookFacility":
                    result = BookFacility((gip.mes.facility.ACMethodBooking)acParameter[0], acParameter[1] as DatabaseApp);
                    return true;
                case "BookFacilityWithRetry":
                    {
                        gip.mes.facility.ACMethodBooking bookingParam = (gip.mes.facility.ACMethodBooking)acParameter[0];
                        if (acParameter.Count() > 2)
                            result = BookFacilityWithRetry(ref bookingParam, acParameter[1] as DatabaseApp, (bool) acParameter[2]);
                        else
                            result = BookFacilityWithRetry(ref bookingParam, acParameter[1] as DatabaseApp);
                    }
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}


