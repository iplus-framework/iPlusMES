using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Class that is responsible for processing input-materials that are associated with an intermediate product. 
    /// The intermediate prduct, in turn, is linked through the material workflow to one or more workflow nodes that are from this PWManualWeighing class. 
    /// PWManualWeighing is used to support manual production. 
    /// It calls the PAFManualWeighing process function asynchronously.
    /// The operator is guided by the business object BSOManualWeighing, which is a plugin for the business object BSOWorkCenter.
    /// Consumed quantities are posted by warehous management (ACFacilityManager).
    /// It can work with different data contexts (production and picking orders).
    /// </summary>
    /// <seealso cref="gip.core.autocomponent.PWNodeProcessMethod" />
    /// <seealso cref="gip.core.autocomponent.IPWNodeReceiveMaterial" />
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PWManualWeighing'}de{'PWManualWeighing'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWManualWeighing : PWNodeProcessMethod, IPWNodeReceiveMaterial, IACMyConfigCache
    {
        public const string PWClassName = "PWManualWeighing";

        #region c´tors

        static PWManualWeighing()
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("FreeSelectionMode", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("FreeSelectionMode", "en{'Material to be weighed can be freely selected'}de{'Zu verwiegendes Material kann frei ausgewählt werden'}");

            method.ParameterValueList.Add(new ACValue("AutoSelectLot", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("AutoSelectLot", "en{'Automatically select lot'}de{'Los automatisch auswählen'}");

            method.ParameterValueList.Add(new ACValue("AutoSelectLotPrio", typeof(LotUsageEnum), LotUsageEnum.ExpirationFirst, Global.ParamOption.Optional));
            paramTranslation.Add("AutoSelectLotPrio", "en{'Priority of auto lot selection'}de{'Priorität der automatischen Losauswahl'}");

            method.ParameterValueList.Add(new ACValue("AutoTare", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("AutoTare", "en{'Automatic tare'}de{'Automatische Tara'}");

            method.ParameterValueList.Add(new ACValue("AutoAcknowledge", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("AutoAcknowledge", "en{'Automatic acknowledge'}de{'Automatische Quittierung'}");

            method.ParameterValueList.Add(new ACValue("EnterLotManually", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("EnterLotManually", "en{'Enter lot manually'}de{'Los manuell eingeben'}");

            method.ParameterValueList.Add(new ACValue("LotValidation", typeof(LotUsageEnum?), null, Global.ParamOption.Optional));
            paramTranslation.Add("LotValidation", "en{'Lot validation'}de{'Chargenvalidierung'}");

            method.ParameterValueList.Add(new ACValue("ComponentsSeqFrom", typeof(Int32), 0, Global.ParamOption.Optional));
            paramTranslation.Add("ComponentsSeqFrom", "en{'Components from Seq.-No.'}de{'Komponenten VON Seq.-Nr.'}");

            method.ParameterValueList.Add(new ACValue("ComponentsSeqTo", typeof(Int32), 0, Global.ParamOption.Optional));
            paramTranslation.Add("ComponentsSeqTo", "en{'Components to Seq.-No.'}de{'Komponenten BIS Seq.-Nr.'}");

            method.ParameterValueList.Add(new ACValue("AutoInsertQuantToStore", typeof(string), "", Global.ParamOption.Optional));
            paramTranslation.Add("AutoInsertQuantToStore", "en{'Store for automatic quant creation'}de{'Lagerplatz für automatische Quantanlage'}");

            method.ParameterValueList.Add(new ACValue("IncludeContainerStores", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("IncludeContainerStores", "en{'IncludeContainerStores'}de{'IncludeContainerStores'}");

            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWManualWeighing), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWManualWeighing), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWManualWeighing), HandleExecuteACMethod_PWManualWeighing);
        }

        public PWManualWeighing(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _IsAborted = false;
            //_WeighingComponentsInfo = null;
            using (ACMonitor.Lock(_65050_WeighingCompLock))
            {
                WeighingComponents = null;
            }
            CurrentEndBatchPosKey = null;
            _LastOpenMaterial = null;

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            using (ACMonitor.Lock(_65050_WeighingCompLock))
            {
                WeighingComponents = null;
            }
            //_WeighingComponentsInfo = null;
            CurrentEndBatchPosKey = null;
            _LastOpenMaterial = null;
            ClearMyConfiguration();
            _ZeroBookingFacilityCharge = null;
            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            //_WeighingComponentsInfo = null;
            CurrentEndBatchPosKey = null;
            CurrentOpenMaterial = null;
            _LastOpenMaterial = null;
            ClearMyConfiguration();
            using (ACMonitor.Lock(_65050_WeighingCompLock))
            {
                WeighingComponents = null;
            }
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }

        #endregion

        #region Const

        public const string MNCompleteWeighing = "CompleteWeighing";
        public const string MaterialConfigLastUsedLotKeyACUrl = "ManWeighLastUsedLot";

        #endregion

        #region Properties

        #region Propeties => Private members

        private readonly ACMonitorObject _65001_CanStartFromBSOLock = new ACMonitorObject(65001);

        private readonly ACMonitorObject _65500_LotChangeLock = new ACMonitorObject(65500);

        protected readonly ACMonitorObject _65003_IsAbortedLock = new ACMonitorObject(65003);

        private readonly ACMonitorObject _65025_MemberCompLock = new ACMonitorObject(65025);
        private readonly ACMonitorObject _65050_WeighingCompLock = new ACMonitorObject(65050);

        protected bool _CanStartFromBSO = true, _IsAborted = false, _IsBinChangeActivated = false;

        private gip.core.datamodel.ACProgramLog _NewAddedProgramLog = null;

        #endregion

        public bool IsProduction
        {
            get
            {
                return ParentPWMethod<PWMethodProduction>() != null;
            }
        }

        public PWMethodVBBase ParentPWMethodVBBase
        {
            get
            {
                return ParentRootWFNode as PWMethodVBBase;
            }
        }

        public override ACMethod ExecutingACMethod
        {
            get
            {
                if (_ExecutingACMethod != null)
                    return _ExecutingACMethod;
                return base.ExecutingACMethod;
            }
        }

        public virtual bool IsManualWeighing => true;

        private ACMethodBooking _ZeroBookingFacilityCharge;
        public ACMethodBooking ZeroBookingFacilityCharge
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_ZeroBookingFacilityCharge != null)
                        return _ZeroBookingFacilityCharge.Clone() as ACMethodBooking;
                }

                if (ACFacilityManager == null)
                    return null;

                ACMethodBooking acBook = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_FacilityCharge, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _ZeroBookingFacilityCharge = acBook;
                    if (_ZeroBookingFacilityCharge != null)
                        return _ZeroBookingFacilityCharge.Clone() as ACMethodBooking;
                }
                return null;
            }
        }

        private ACMethodBooking _InventoryNewQuant;
        public ACMethodBooking InventoryNewQuant
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_InventoryNewQuant != null)
                        return _InventoryNewQuant.Clone() as ACMethodBooking;
                }

                if (ACFacilityManager == null)
                    return null;

                ACMethodBooking acBook = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_InventoryNewQuant, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _InventoryNewQuant = acBook;
                    if (_InventoryNewQuant != null)
                        return _InventoryNewQuant.Clone() as ACMethodBooking;
                }
                return null;
            }
        }

        #region Properties => Managers

        public ACProdOrderManager ProdOrderManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.ProdOrderManager : null;
            }
        }

        protected ACPartslistManager PartslistManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.PartslistManager : null;
            }
        }

        protected FacilityManager ACFacilityManager
        {
            get
            {
                if (ParentPWMethodVBBase == null)
                    return null;
                return ParentPWMethodVBBase.ACFacilityManager as FacilityManager;
            }
        }

        #endregion

        #region Properties => Configuration

        private ACMethod _MyConfiguration;
        [ACPropertyInfo(9999)]
        public ACMethod MyConfiguration
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_MyConfiguration != null)
                        return _MyConfiguration;
                }

                var myNewConfig = NewACMethodWithConfiguration();
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _MyConfiguration = myNewConfig;
                }
                return myNewConfig;
            }
        }

        //QUESTION:is this needed
        /// <summary>
        /// A endless dosing can be achieved by setting the MinDosQuantity to a large negative value.
        /// If the MinDosQuantity is positive and the remaining Quantity for dosing is smaller than this value, then the dosing will be skipped
        /// </summary>
        public double MinWeightQuantity
        {
            get
            {
                double? minDosQuantity = null;
                //var method = MyConfiguration;
                //if (method != null)
                //{
                //    var acValue = method.ParameterValueList.GetACValue("MinDosQuantity");
                //    if (acValue != null)
                //    {
                //        minDosQuantity = acValue.ParamAsDouble;
                //    }
                //}
                //if (!minDosQuantity.HasValue && this.ParentPWGroup != null)
                //{
                //    PAMHopperscale pamScale = this.ParentPWGroup.AccessedProcessModule as PAMHopperscale;
                //    if (pamScale != null)
                //    {
                //        if (pamScale.MinDosingWeight.HasValue)
                //            minDosQuantity = pamScale.MinDosingWeight.Value;
                //    }
                //}

                if (!minDosQuantity.HasValue)
                    minDosQuantity = 0.000001;
                return minDosQuantity.Value;
            }
        }

        public int ComponentsSeqFrom
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ComponentsSeqFrom");
                    if (acValue != null)
                    {
                        return acValue.ParamAsInt32;
                    }
                }
                return 0;
            }
        }

        public int ComponentsSeqTo
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ComponentsSeqTo");
                    if (acValue != null)
                    {
                        return acValue.ParamAsInt32;
                    }
                }
                return 0;
            }
        }

        public bool FreeSelectionMode
        { 
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("FreeSelectionMode");
                    if (acValue != null)
                        return acValue.ParamAsBoolean;
                }
                return false;
            }
        }

        public bool ScaleAutoTare
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("AutoTare");
                    if (acValue != null)
                        return acValue.ParamAsBoolean;
                }
                return false;
            }
        }

        public bool AutoSelectLot
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("AutoSelectLot");
                    if (acValue != null)
                        return acValue.ParamAsBoolean;
                }
                return false;
            }
        }

        public LotUsageEnum AutoSelectLotPriority
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("AutoSelectLotPrio");
                    if (acValue != null)
                        return (LotUsageEnum)acValue.Value;
                }
                return LotUsageEnum.ExpirationFirst;
            }
        }

        public bool AutoAcknowledge
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("AutoAcknowledge");
                    if (acValue != null)
                        return acValue.ParamAsBoolean;
                }
                return false;
            }
        }

        public LotUsageEnum? LotValidation
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("LotValidation");
                    if (acValue != null)
                        return acValue.Value as LotUsageEnum?;
                }
                return null;
            }
        }

        public string AutoInsertQuantToStore
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("AutoInsertQuantToStore");
                    if (acValue != null)
                        return acValue.ParamAsString;
                }
                return null;
            }
        }

        public bool IncludeContainerStores
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("IncludeContainerStores");
                    if (acValue != null)
                        return acValue.ParamAsBoolean;
                }
                return false;
            }
        }

        #endregion

        #region Properties => Materials, Relations and FacilityCharge

        [ACPropertyInfo(999)]
        public EntityKey CurrentEndBatchPosKey
        {
            get;
            set;
        }

        protected ProdOrderPartslistPos IntermediateChildPos
        {
            get;
            set;
        }

        protected Guid? _LastOpenMaterial;

        private Guid? _CurrentOpetMaterial;
        [ACPropertyInfo(999, IsPersistable = true)]
        public Guid? CurrentOpenMaterial
        {
            get
            {
                using (ACMonitor.Lock(_65025_MemberCompLock))
                {
                    return _CurrentOpetMaterial;
                }
            }
            set
            {
                using (ACMonitor.Lock(_65025_MemberCompLock))
                {
                    _CurrentOpetMaterial = value;
                }
                OnPropertyChanged("CurrentOpenMaterial");
            }
        }

        private Guid? _CurrentFacilityCharge;
        [ACPropertyInfo(999, IsPersistable = true)]
        public Guid? CurrentFacilityCharge
        {
            get
            {
                using (ACMonitor.Lock(_65025_MemberCompLock))
                {
                    return _CurrentFacilityCharge;
                }
            }
            set
            {
                using (ACMonitor.Lock(_65025_MemberCompLock))
                {
                    _CurrentFacilityCharge = value;
                }
                OnPropertyChanged("CurrentFacilityCharge");
            }
        }

        private Guid? _CurrentFacility;
        [ACPropertyInfo(999, IsPersistable = true)]
        public Guid? CurrentFacility
        {
            get => _CurrentFacility;
            set
            {
                _CurrentFacility = value;
                OnPropertyChanged("CurrentFacility");
            }
        }

        //Dictionary<string, string> _WeighingComponentsInfo;
        [ACPropertyInfo(999)]
        public ACValue WeighingComponentsInfo
        {
            get
            {
                Dictionary<string, string> weighingComponentsInfo = null;
                using (ACMonitor.Lock(_65050_WeighingCompLock))
                {
                    weighingComponentsInfo = WeighingComponents.ToDictionary(c => c.PLPosRelation.ToString(), c => c.WeighState.ToString());
                }

                return new ACValue("WM", weighingComponentsInfo);
            }
        }

        public List<WeighingComponent> WeighingComponents
        {
            get;
            set;
        }

        public virtual Func<IEnumerable<Facility>, Guid?, IEnumerable<FacilityCharge>> FacilityChargeListQuery
        {
            get
            {
                return (facility, matID) => facility.SelectMany(c => c.FacilityCharge_Facility.Where(x => !x.NotAvailable && (matID == null || x.MaterialID == matID)
                                                                                                && (x.MDReleaseState == null 
                                                                                                 || x.MDReleaseState.MDReleaseStateIndex <= (short)MDReleaseState.ReleaseStates.AbsFree)))
                                                                                              .ToArray().OrderBy(o => o.ExpirationDate);
            }
        }

        public virtual Func<IEnumerable<Facility>, Guid[], IEnumerable<Facility>> FacilityListQuery
        {
            get
            {
                return (fc, a) => fc.Where(c => !c.NotAvailable && a != null && c.VBiFacilityACClassID.HasValue &&
                                                 a.Any(x => x == c.VBiFacilityACClassID)).ToArray();
            }
        }

        #endregion

        #region Properties => WeighingInfo

        private ACMonitorObject _65000_CurrentWeighingComponentInfoLock = new ACMonitorObject(65000);

        [ACPropertyBindingSource]
        public IACContainerTNet<WeighingComponentInfo> CurrentWeighingComponentInfo
        {
            get;
            set;
        }

        #endregion

        [ACPropertyBindingSource]
        public IACContainerTNet<ManualWeighingTaskInfo> ManualWeihgingNextTask
        {
            get;
            set;
        }

        public bool HasAnyMaterialToProcess
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                // If dosing is not for production, then do nothing
                if (pwMethodProduction == null)
                    return true;

                using (var dbIPlus = new Database())
                {
                    using (var dbApp = new DatabaseApp(dbIPlus))
                    {
                        ProdOrderPartslistPos endBatchPos = pwMethodProduction.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                        if (pwMethodProduction.CurrentProdOrderBatch == null)
                            return true;

                        var contentACClassWFVB = ContentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(dbApp);
                        ProdOrderBatch batch = pwMethodProduction.CurrentProdOrderBatch.FromAppContext<ProdOrderBatch>(dbApp);
                        ProdOrderBatchPlan batchPlan = batch.ProdOrderBatchPlan;
                        MaterialWFConnection matWFConnection = null;
                        if (batchPlan != null && batchPlan.MaterialWFACClassMethodID.HasValue)
                        {
                            matWFConnection = dbApp.MaterialWFConnection
                                                    .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan.MaterialWFACClassMethodID.Value
                                                            && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                                    .FirstOrDefault();
                        }
                        else
                        {
                            PartslistACClassMethod plMethod = endBatchPos.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                            if (plMethod != null)
                            {
                                matWFConnection = dbApp.MaterialWFConnection
                                                        .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                                && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                                        .FirstOrDefault();
                            }
                            else
                            {
                                matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
                                    .Where(c => c.MaterialWFACClassMethod.MaterialWFID == endBatchPos.ProdOrderPartslist.Partslist.MaterialWFID
                                                && c.MaterialWFACClassMethod.PartslistACClassMethod_MaterialWFACClassMethod.Where(d => d.PartslistID == endBatchPos.ProdOrderPartslist.PartslistID).Any())
                                    .FirstOrDefault();
                            }
                        }

                        if (matWFConnection == null)
                            return true;

                        // Find intermediate position which is assigned to this Dosing-Workflownode
                        var currentProdOrderPartslist = endBatchPos.ProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
                        ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                            .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                                && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                                && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
                        if (intermediatePosition == null)
                            return true;

                        ProdOrderPartslistPos intermediateChildPos = null;
                        // Lock, if a parallel Dosing also creates a child Position for this intermediate Position

                        using (ACMonitor.Lock(pwMethodProduction._62000_PWGroupLockObj))
                        {
                            // Find intermediate child position, which is assigned to this Batch
                            intermediateChildPos = intermediatePosition.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                                .Where(c => c.ProdOrderBatchID.HasValue
                                            && c.ProdOrderBatchID.Value == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID)
                                .FirstOrDefault();
                        }

                        if (intermediateChildPos == null)
                            return true;

                        ProdOrderPartslistPosRelation[] queryOpenMaterials = OnGetAllMaterials(dbIPlus, dbApp, intermediateChildPos);
                        if ((ComponentsSeqFrom > 0 || ComponentsSeqTo > 0) && queryOpenMaterials != null && queryOpenMaterials.Any())
                            queryOpenMaterials = queryOpenMaterials.Where(c => c.Sequence >= ComponentsSeqFrom && c.Sequence <= ComponentsSeqTo)
                                                                .OrderBy(c => c.Sequence)
                                                                .ToArray();
                        if (queryOpenMaterials == null || !queryOpenMaterials.Any())
                            return false;

                        var materialsToProcess = queryOpenMaterials.Where(c => c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex < (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                                                                 || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex > (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled);

                        if (materialsToProcess == null || !materialsToProcess.Any())
                            return false;
                    }
                }

                return true;
            }
        }

        #endregion

        #region Methods

        #region Methods => ACState

        public override void SMIdle()
        {
            CurrentOpenMaterial = null;
            CurrentFacility = null;
            CurrentFacilityCharge = null;
            CurrentACMethod.ValueT = null;
            ClearMyConfiguration();

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _IsBinChangeActivated = false;
            }

            using (ACMonitor.Lock(_65003_IsAbortedLock))
            {
                _IsAborted = false;
            }
            base.SMIdle();
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            var pwGroup = ParentPWGroup;
            if (pwGroup == null) // Is null when Service-Application is shutting down
            {
                if (this.InitState == ACInitState.Initialized)
                    Messages.LogError(this.GetACUrl(), "SMStarting()", "ParentPWGroup is null");
                return;
            }

            if (((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
                || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp)
                || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging)
                || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)
                || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
                || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
            {
                UnSubscribeToProjectWorkCycle();
                // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                if (CurrentACState == ACStateEnum.SMStarting)
                    CurrentACState = ACStateEnum.SMCompleted;
                return;
            }

            if (!Root.Initialized)
            {
                SubscribeToProjectWorkCycle();
                return;
            }

            core.datamodel.ACClassMethod refPAACClassMethod = null;
            if (this.ContentACClassWF != null)
            {

                using (ACMonitor.Lock(this.ContextLockForACClassWF))
                {
                    refPAACClassMethod = this.ContentACClassWF.RefPAACClassMethod;
                }
            }

            if (pwGroup != null
                && this.ContentACClassWF != null
                && refPAACClassMethod != null)
            {
                PAProcessModule module = null;
                if (ParentPWGroup.NeedsAProcessModule && (ACOperationMode == ACOperationModes.Live || ACOperationMode == ACOperationModes.Simulation))
                    module = ParentPWGroup.AccessedProcessModule;
                // Testmode
                else
                    module = ParentPWGroup.ProcessModuleForTestmode;

                if (module == null)
                {
                    //Error50372: The workflow group has not occupied a process module.
                    // Die Workflowgruppe hat kein Prozessmodul belegt.
                    Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(10)", 1000, "Error50372");
                    ActivateProcessAlarmWithLog(msg, false);
                    SubscribeToProjectWorkCycle();
                    return;
                }

                PAFManualWeighing manualWeighing = CurrentExecutingFunction<PAFManualWeighing>();
                if(manualWeighing != null)
                {
                    if (CurrentACState == ACStateEnum.SMStarting)
                        CurrentACState = ACStateEnum.SMRunning;
                    return;
                }

                StartNextCompResult result = StartNextCompResult.Done;
                if (IsProduction)
                    result = StartManualWeighingProd(module);

                if (result == StartNextCompResult.CycleWait)
                {
                    SubscribeToProjectWorkCycle();
                    return;
                }
                else if (result == StartNextCompResult.NextCompStarted || result == StartNextCompResult.WaitForNextEvent)
                {
                    // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                    if (CurrentACState == ACStateEnum.SMStarting)
                    {
                        CurrentACState = ACStateEnum.SMRunning;
                        RaiseRunningEvent();
                    }
                    return;
                }
                else
                {
                    UnSubscribeToProjectWorkCycle();
                    // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                    if (CurrentACState == ACStateEnum.SMStarting)
                        CurrentACState = ACStateEnum.SMCompleted;
                    return;
                }
            }

            // Falls module.AddTask synchron ausgeführt wurde, dann ist der Status schon weiter
            if (CurrentACState == ACStateEnum.SMStarting)
            {
                CurrentACState = ACStateEnum.SMRunning;
                //PostExecute(PABaseState.SMStarting);
            }
        }

        public override void SMRunning()
        {
            if (!Root.Initialized)
            {
                SubscribeToProjectWorkCycle();
                return;
            }

            try
            {
                PAFManualWeighing manualWeighing = CurrentExecutingFunction<PAFManualWeighing>();

                bool isWeighComponentsNull;
                using (ACMonitor.Lock (_65050_WeighingCompLock))
                {
                    isWeighComponentsNull = WeighingComponents == null;
                }

                if (isWeighComponentsNull)
                {
                    if (ParentPWGroup.AccessedProcessModule == null)
                    {
                        SubscribeToProjectWorkCycle();
                        return;
                    }

                    StartManualWeighingProd(ParentPWGroup.AccessedProcessModule);

                    //Check if manual weighing currently active
                    if (manualWeighing != null)
                    {
                        Guid? currentOpenMat = manualWeighing.CurrentACMethod.ValueT.ParameterValueList.GetGuid("PLPosRelation");
                        //CurrentOpenMaterial = manualWeighing.CurrentACMethod.ValueT.ParameterValueList.GetGuid("PLPosRelation");
                        if (currentOpenMat == null)
                        {
                            SubscribeToProjectWorkCycle();
                            return;
                        }

                        CurrentOpenMaterial = currentOpenMat;

                        WeighingComponent comp = GetWeighingComponent(currentOpenMat);

                        if (comp == null)
                        {
                            SubscribeToProjectWorkCycle();
                            return;
                        }

                        ACValue facilityCharge = manualWeighing.CurrentACMethod.ValueT.ParameterValueList.GetACValue("FacilityCharge");
                        //ACValue facility = manualWeighing.CurrentACMethod.ValueT.ParameterValueList.GetACValue("Facility");
                        if (facilityCharge != null && facilityCharge.Value != null)
                        {
                            CurrentFacilityCharge = facilityCharge.ParamAsGuid;
                            comp.SwitchState(WeighingComponentState.InWeighing);
                            //comp.WeighState = (short)WeighingComponentState.InWeighing;
                            SetInfo(comp, WeighingComponentInfoType.StateSelectCompAndFC_F, facilityCharge.ParamAsGuid, null);
                        }
                        //else if (facility != null && facility.Value != null)
                        //{
                        //    CurrentFacility = facility.ParamAsGuid;
                        //    //comp.WeighState = (short)WeighingComponentState.InWeighing;
                        //    comp.SwitchState(WeighingComponentState.InWeighing);
                        //    SetInfo(comp, WeighingComponentInfoType.StateSelectCompAndFC_F, null, CurrentFacility);
                        //}
                        else
                        {
                            //_ExitFromWaitForFC = false;
                            //ThreadPool.QueueUserWorkItem((object state) => WaitForFacilityChargeOrFacility(CurrentOpenMaterial, WeighingComponentInfoType.State)); //Auto Comp && Man Lot
                            SetInfo(comp, WeighingComponentInfoType.SelectCompReturnFC_F, null, null);
                        }
                    }
                }

                if (manualWeighing != null)
                {
                    UnSubscribeToProjectWorkCycle();
                    return;
                }

                bool isAnyNeedToWeigh = false;
                using (ACMonitor.Lock(_65050_WeighingCompLock))
                {
                    isAnyNeedToWeigh = WeighingComponents.Any(c => c.WeighState < (short)WeighingComponentState.InWeighing);
                }

                Guid? currentOpenMaterial = CurrentOpenMaterial;

                if (!FreeSelectionMode)
                {
                    if (currentOpenMaterial == null)
                    {
                        WeighingComponent nextComp = null;
                        using (ACMonitor.Lock(_65050_WeighingCompLock))
                        {
                            nextComp = WeighingComponents.OrderBy(c => c.Sequence).FirstOrDefault(c => c.WeighState == (short)WeighingComponentState.ReadyToWeighing);
                        }
                        if (nextComp == null)
                        {
                            bool isAllCompleted = RefreshCompStateFromDBAndCheckIsAllCompleted();

                            if (isAllCompleted)
                            {
                                UnSubscribeToProjectWorkCycle();
                                CurrentACState = ACStateEnum.SMCompleted;
                                return;
                            }
                            else
                            {
                                SubscribeToProjectWorkCycle();
                                return;
                            }
                        }

                        CurrentOpenMaterial = nextComp.PLPosRelation;
                        bool hasQuants = TryAutoSelectFacilityCharge(nextComp.PLPosRelation);
                        StartManualWeighingNextComp(ParentPWGroup.AccessedProcessModule, nextComp, hasQuants); //Auto Comp && Auto Lot

                        Guid? currentFacilityCharge = CurrentFacilityCharge;

                        if (!currentFacilityCharge.HasValue && !CurrentFacility.HasValue)
                        {
                            //_ExitFromWaitForFC = false;
                            SetInfo(nextComp, WeighingComponentInfoType.SelectCompReturnFC_F, currentFacilityCharge, CurrentFacility);
                            //ThreadPool.QueueUserWorkItem((object state) => WaitForFacilityChargeOrFacility(CurrentOpenMaterial, WeighingComponentInfoType.State)); //Auto Comp && Man Lot
                        }
                    }
                    else
                        UnSubscribeToProjectWorkCycle();
                }
                else if ((currentOpenMaterial != _LastOpenMaterial || _LastOpenMaterial == null) && isAnyNeedToWeigh)
                {
                    WeighingComponent nextComp = GetWeighingComponent(currentOpenMaterial);// WeighingComponents.FirstOrDefault(c => c.PLPosRelation == CurrentOpenMaterial);
                    //if (nextComp == null)
                    //{
                    //    SubscribeToProjectWorkCycle();
                    //    return;
                    //}

                    Guid? currentFacilityCharge = CurrentFacilityCharge;

                    if (CurrentFacility != null || currentFacilityCharge != null)
                    {
                        StartManualWeighingNextComp(ParentPWGroup.AccessedProcessModule, nextComp, null); //Man Comp && Auto Lot || ManLot
                        UnSubscribeToProjectWorkCycle();
                    }
                    else if (AutoSelectLot)
                    {
                        bool hasQuants = TryAutoSelectFacilityCharge(currentOpenMaterial);
                        if (CurrentFacility != null || currentFacilityCharge != null)
                        {
                            StartManualWeighingNextComp(ParentPWGroup.AccessedProcessModule, nextComp, hasQuants); //Man Comp && Auto Lot || ManLot
                            UnSubscribeToProjectWorkCycle();
                        }
                    }
                    else
                    {
                        StartManualWeighingNextComp(ParentPWGroup.AccessedProcessModule, nextComp, null);
                        UnSubscribeToProjectWorkCycle();
                        SetCanStartFromBSO(true);
                    }
                }
                else
                {
                    bool isAnyReadyToWeigh = false;
                    using(ACMonitor.Lock(_65050_WeighingCompLock))
                    {
                        isAnyReadyToWeigh = WeighingComponents.Any(c => c.WeighState == (short)WeighingComponentState.ReadyToWeighing);
                    }

                    if (isAnyReadyToWeigh)
                        SubscribeToProjectWorkCycle();
                    else
                    {
                        bool isAllCompleted = RefreshCompStateFromDBAndCheckIsAllCompleted();

                        if (isAllCompleted)
                        {
                            UnSubscribeToProjectWorkCycle();
                            CurrentACState = ACStateEnum.SMCompleted;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string message = "";
                if (e.InnerException != null)
                    message = String.Format("{0}, {1}", e.Message, e.InnerException.Message);
                else
                    message = e.Message;

                Msg msg = new Msg(message, this, eMsgLevel.Exception, PWClassName, "SMRunning(10)", 772);
                Messages.LogMessageMsg(msg);
            }
        }

        public override void SMCompleted()
        {
            //_WeighingComponentsInfo = null;
            CurrentOpenMaterial = null;

            using (ACMonitor.Lock(_65050_WeighingCompLock))
            {
                WeighingComponents = null;
            }
            base.SMCompleted();
        }

        /// <summary>
        /// State: Resetting
        /// </summary>
        [ACMethodState("en{'Resetting'}de{'Zurücksetzen'}", 40, true)]
        public virtual void SMResetting()
        {
        }

        public override void Reset()
        {
            CurrentOpenMaterial = null;
            CurrentFacilityCharge = null;
            CurrentFacility = null;
            //_WeighingComponentsInfo = null;
            using (ACMonitor.Lock(_65050_WeighingCompLock))
            {
                WeighingComponents = null;
            }
            ClearMyConfiguration();
            SetCanStartFromBSO(true);

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _IsBinChangeActivated = false;
            }

            UnSubscribeToProjectWorkCycle();

            CurrentACState = ACStateEnum.SMResetting;

            base.Reset();
        }

        #endregion        

        #region Methods => Commands

        [ACMethodInfo("","",999)]
        public Msg StartWeighing(Guid? prodOrderPartslistPosRelation, Guid? facilityCharge, Guid? facility, bool forceSetFC_F)
        {
            using (ACMonitor.Lock(_65001_CanStartFromBSOLock))
            {
                if (!_CanStartFromBSO)
                    return null;
                _CanStartFromBSO = false;
            }

            if(prodOrderPartslistPosRelation == null)
                return null;

            WeighingComponent comp = GetWeighingComponent(prodOrderPartslistPosRelation);  // WeighingComponents?.FirstOrDefault(c => c.PLPosRelation == prodOrderPartslistPosRelation);
            if (comp == null || comp.WeighState == (short)WeighingComponentState.WeighingCompleted)
                return null;

            if (facilityCharge.HasValue)
            {
                Msg msg = SetFacilityCharge(facilityCharge, prodOrderPartslistPosRelation, forceSetFC_F);
                if (msg != null)
                {
                    SetCanStartFromBSO(true);
                    return msg;
                }

                if (!FreeSelectionMode)
                {
                    Guid? currentOpenMaterial = CurrentOpenMaterial;

                    if (ApplicationManager != null && ApplicationManager.ApplicationQueue != null && !ApplicationManager.ApplicationQueue.IsBusy)
                        ApplicationManager.ApplicationQueue.Add(() => SelectFacilityChargeOrFacility(currentOpenMaterial, WeighingComponentInfoType.State));
                    else
                        SelectFacilityChargeOrFacility(currentOpenMaterial, WeighingComponentInfoType.State);
                }
                else
                {
                    ACMethod acMethod = CurrentExecutingFunction<PAFManualWeighing>()?.CurrentACMethod.ValueT;
                    if (acMethod != null)
                    {
                        UpdatePAFACMethod(comp, acMethod);
                    }
                    else
                    {
                        //TODO: error
                    }
                }
            }
            else if (facility.HasValue)
            {
                Msg msg = SetFacility(facility, prodOrderPartslistPosRelation);
                if(msg != null)
                {
                    SetCanStartFromBSO(true);
                    return msg;
                }

                if (!FreeSelectionMode)
                {
                    if (ApplicationManager != null && ApplicationManager.ApplicationQueue != null && !ApplicationManager.ApplicationQueue.IsBusy)
                        ApplicationManager.ApplicationQueue.Add(() => SelectFacilityChargeOrFacility(CurrentOpenMaterial, WeighingComponentInfoType.State));
                    else
                        SelectFacilityChargeOrFacility(CurrentOpenMaterial, WeighingComponentInfoType.State);
                }
            }
            else
                SetCanStartFromBSO(true);

            CurrentOpenMaterial = prodOrderPartslistPosRelation;
            return null;
        }

        [ACMethodInfo("", "", 999)]
        public virtual void CompleteWeighing(double actualQuantity, bool bookAndWeighRest)
        {
            PAFManualWeighing manualWeighing = CurrentExecutingFunction<PAFManualWeighing>();

            if (bookAndWeighRest && manualWeighing != null)
            {
                var targetQ = CurrentACMethod.ValueT.ParameterValueList.GetACValue("TargetQuantity");
                double targetQuantity = 0;
                if (targetQ != null)
                    targetQuantity = targetQ.ParamAsDouble;

                if (actualQuantity > 0.000001 && actualQuantity < targetQuantity)
                {
                    Guid? currentFacilityCharge = CurrentFacilityCharge;
                    var msgBooking = DoManualWeighingBooking(actualQuantity, false, false, currentFacilityCharge);
                    if (msgBooking != null)
                    {
                        Messages.LogError(this.GetACUrl(), msgBooking.ACIdentifier, msgBooking.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msgBooking, false);
                        ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;

                        return;
                    }

                    double rest = targetQuantity - actualQuantity;
                    manualWeighing.CurrentACMethod.ValueT.ParameterValueList["TargetQuantity"] = rest;
                    manualWeighing.ReSendACMethod(manualWeighing.CurrentACMethod.ValueT);

                    Guid? currentOpenMaterial = CurrentOpenMaterial;

                    WeighingComponent comp = GetWeighingComponent(currentOpenMaterial); //WeighingComponents.FirstOrDefault(c => c.PLPosRelation == CurrentOpenMaterial); //TODO:lock currentOpenMaterial
                    if (comp == null)
                    {
                        return;
                    }

                    comp.SwitchState(WeighingComponentState.PartialCompleted);
                    SetInfo(comp, WeighingComponentInfoType.State, null, null);
                    comp.SwitchState(WeighingComponentState.InWeighing);
                    SetInfo(comp, WeighingComponentInfoType.StateSelectCompAndFC_F, currentFacilityCharge, null);
                }
            }
            else if (manualWeighing != null)
            {
                manualWeighing.CompleteWeighing(actualQuantity);
            }
        }

        [ACMethodInfo("", "", 999)]
        public void TareScale()
        {
            PAFManualWeighing manualWeighing = CurrentExecutingFunction<PAFManualWeighing>();
            if (manualWeighing != null)
                manualWeighing.ActiveScaleObject?.Tare();
        }

        protected internal Msg SelectFC_FFromPAF(Guid? newFacilityCharge, double actualQuantity, bool isConsumed, bool forceSetFC_F)
        {
            Guid? currentOpenMaterial = CurrentOpenMaterial;

            if (!currentOpenMaterial.HasValue)
            {
                // Error50373: Manual weighing error, the property {0} is null!
                // Fehler bei Handverwiegung, die Eigenschaft {0} ist null!
                return new Msg(this, eMsgLevel.Error, PWClassName, "SelectFC_FFromPAF(10)", 910, "Error50373", "CurrentOpenMaterial");
            }

            Guid? currentFacilityCharge = CurrentFacilityCharge;

            if (!currentFacilityCharge.HasValue && !CurrentFacility.HasValue)
            {
                using (Database db = new core.datamodel.Database())
                using (DatabaseApp dbApp = new DatabaseApp(db))
                {
                    ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == currentOpenMaterial.Value);
                    if (rel == null)
                    {
                        //Error50374: ProdOrderPartslistPosRelation {0} doesn't exist in the database.
                        return new Msg(this, eMsgLevel.Error, PWClassName, "SelectFC_FFromPAF(20)", 921, "Error50374", currentOpenMaterial.Value);
                    }

                    var fc = GetFacilityChargesForMaterial(dbApp, rel);
                    if (fc.Any(c => c.FacilityChargeID == newFacilityCharge.Value))
                    {
                        Msg msg = SetFacilityCharge(newFacilityCharge.Value, currentOpenMaterial, forceSetFC_F);
                        if (msg != null)
                            return msg;
                        SelectFacilityChargeOrFacility(currentOpenMaterial, WeighingComponentInfoType.State);
                    }
                    else
                    {
                        //Error50266: The weighing cannot be started because the selected quant cannot be used at this work center {0} {1}.
                        // Die Verwiegung kann nicht gestartet werden weil das ausgewählte Quant nicht an diesem Arbeitsplatz {0} {1} nicht verwendet werden kann.
                        return new Msg(this, eMsgLevel.Error, PWClassName, "SelectFC_FFromPAF", 935, "Error50266", rel.SourceProdOrderPartslistPos.Material.MaterialNo,
                                                                                                                          rel.SourceProdOrderPartslistPos.Material.MaterialName1);
                    }
                }
            }
            else if (currentFacilityCharge.HasValue)
            {
                return LotChange(newFacilityCharge, actualQuantity, isConsumed, forceSetFC_F);
            }
            return null;
        }

        [ACMethodInfo("", "", 999)]
        public virtual Msg LotChange(Guid? newFacilityCharge, double actualQuantity, bool isConsumed, bool forceSetFC_F)
        {
            if (newFacilityCharge == null)
            {
                // Error50273: The parameter newFacilityCharge ID is null.
                Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "LotChange(1)", 953, "Error50273");
                ActivateProcessAlarmWithLog(msg);
                return msg;
            }

            Msg msgSet = null;
            Guid? facilityCharge = null;
            Guid? currentOpenMaterial = CurrentOpenMaterial;
            Guid? currentFacilityCharge = CurrentFacilityCharge;

            using (ACMonitor.Lock(_65500_LotChangeLock))
            {
                if (currentFacilityCharge == newFacilityCharge)
                    return null;

                facilityCharge = currentFacilityCharge;

                msgSet = SetFacilityCharge(newFacilityCharge, currentOpenMaterial, forceSetFC_F);
            }

            if (actualQuantity > 0.000001)
            {
                var msgBooking = DoManualWeighingBooking(actualQuantity, false, isConsumed, facilityCharge);
                if (msgBooking != null)
                {
                    Messages.LogError(this.GetACUrl(), msgBooking.ACIdentifier, msgBooking.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msgBooking, false);
                    ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;
                    return msgBooking;
                }
            }

            if(facilityCharge.HasValue && isConsumed)
            {
                DoFacilityChargeZeroBooking(facilityCharge.Value);
            }

            if (msgSet == null)
            {
                SaveLastUsedLot(newFacilityCharge, currentOpenMaterial);
                WeighingComponent comp = GetWeighingComponent(currentOpenMaterial); //WeighingComponents.FirstOrDefault(c => c.PLPosRelation == CurrentOpenMaterial);
                SetInfo(comp, WeighingComponentInfoType.SelectFC_F, newFacilityCharge, null, true, true);
            }

            return msgSet;
        }

        [ACMethodInfo("", "", 999)]
        public virtual void BinChange()
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (_IsBinChangeActivated)
                    return;

                _IsBinChangeActivated = true;
            }

            PAFManualWeighing pafManualWeighing = CurrentExecutingFunction<PAFManualWeighing>();
            if (pafManualWeighing != null)
            {
                if (pafManualWeighing.CurrentACState == ACStateEnum.SMRunning)
                {
                    pafManualWeighing.CompleteWeighing(0, true);
                    return;
                }
                else
                    pafManualWeighing.Reset();
            }

            Reset();
            RaiseOutEvent();
        }

        [ACMethodInfo("", "", 999)]
        public virtual Msg OnApplyManuallyEnteredLot(string enteredLotNo, Guid plPosRelation)
        {
            Msg msg = null;
            try
            { 
                using (Database db = new core.datamodel.Database())
                using (DatabaseApp dbApp = new DatabaseApp(db))
                {
                    ProdOrderPartslistPosRelation posRel = dbApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == plPosRelation);
                    if (posRel == null)
                    {
                        //Error50374: ProdOrderPartslistPosRelation {0} doesn't exist in the database.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnApplyManuallyEnteredLot(10)", 1034, "Error50374", plPosRelation);
                        Messages.LogMessageMsg(msg);
                        return msg;
                    }

                    if (!posRel.SourceProdOrderPartslistPos.Material.IsLotManaged)
                    {
                        //Error50267: The material {0} is not lot managed!
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnApplyManuallyEnteredLot(20)", 1040, "Error50267", posRel.SourceProdOrderPartslistPos.Material.MaterialNo);
                        Messages.LogMessageMsg(msg);
                        return msg;
                    }

                    FacilityCharge fc = GetFacilityChargesForMaterial(dbApp, posRel).FirstOrDefault(c => c.FacilityLot != null && c.FacilityLot.LotNo == enteredLotNo);
                    if (fc == null)
                    {
                        //Error50268: An available quant with Lotnumber {0} doesn't exist in the warehouse!
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnApplyManuallyEnteredLot(30)", 1049, "Error50268", enteredLotNo);
                        Messages.LogMessageMsg(msg);
                        return msg;
                    }

                    SelectFC_FFromPAF(fc.FacilityChargeID, 0, false, false);

                    //WeighingComponent comp = GetWeighingComponent(plPosRelation); //WeighingComponents?.FirstOrDefault(c => c.PLPosRelation == plPosRelation);
                    //if (comp == null)
                    //{
                    //    //Error50270: The component {0} doesn't exist for weighing.
                    //    msg = new Msg(this, eMsgLevel.Exception, PWClassName, "OnApplyManuallyEnteredLot(35)", 1058, "Error50270", plPosRelation);
                    //    Messages.Msg(msg);
                    //    return msg;
                    //}

                    //SetInfo(comp, WeighingComponentInfoType.SelectFC_F, fc.FacilityChargeID, null, true);
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
                if (e.InnerException != null)
                    message += "  " + e.InnerException.Message;

                msg = new Msg(message, this, eMsgLevel.Error, PWClassName, "OnApplyManuallyEnteredLot(40)", 1072);
                Messages.LogMessageMsg(msg);
            }
            return msg;
        }

        [ACMethodInfo("", "", 999)]
        public virtual void Abort(bool isConsumed)
        {
            PAFManualWeighing paf = CurrentExecutingFunction<PAFManualWeighing>();
            using (ACMonitor.Lock(_65003_IsAbortedLock))
            {
                if (_IsAborted)
                    return;
                _IsAborted = true;
            }

            if (paf != null)
                paf.Abort(isConsumed);
        }

        #endregion

        #region Methods => StartPWNode

        public virtual StartNextCompResult StartManualWeighingProd(PAProcessModule module)
        {
            Msg msg = null;

            if (!Root.Initialized)
                return StartNextCompResult.CycleWait;

            if (module == null)
            {
                // Error50274: The PAProcessModule is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(10)", 956, "Error50274");
                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, false);
                return StartNextCompResult.CycleWait;
            }

            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            // If dosing is not for production, then do nothing
            if (pwMethodProduction == null)
                return StartNextCompResult.Done;
            
            if (ProdOrderManager == null)
            {
                // Error50275: ProdOrderManager is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(20)", 970, "Error50275");
                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, false);
                return StartNextCompResult.CycleWait;
            }

            using (var dbIPlus = new Database())
            {
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    ProdOrderPartslistPos endBatchPos = pwMethodProduction.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                    CurrentEndBatchPosKey = endBatchPos.EntityKey;
                    if (pwMethodProduction.CurrentProdOrderBatch == null)
                    {
                        // Error50276: No batch assigned to last intermediate material of this workflow
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(30)", 1010, "Error50276");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return StartNextCompResult.CycleWait;
                    }

                    var contentACClassWFVB = ContentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(dbApp);
                    ProdOrderBatch batch = pwMethodProduction.CurrentProdOrderBatch.FromAppContext<ProdOrderBatch>(dbApp);
                    ProdOrderBatchPlan batchPlan = batch.ProdOrderBatchPlan;
                    MaterialWFConnection matWFConnection = null;
                    if (batchPlan != null && batchPlan.MaterialWFACClassMethodID.HasValue)
                    {
                        matWFConnection = dbApp.MaterialWFConnection
                                                .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan.MaterialWFACClassMethodID.Value
                                                        && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                                .FirstOrDefault();
                    }
                    else
                    {
                        PartslistACClassMethod plMethod = endBatchPos.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                        if (plMethod != null)
                        {
                            matWFConnection = dbApp.MaterialWFConnection
                                                    .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                            && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                                    .FirstOrDefault();
                        }
                        else
                        {
                            matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
                                .Where(c => c.MaterialWFACClassMethod.MaterialWFID == endBatchPos.ProdOrderPartslist.Partslist.MaterialWFID
                                            && c.MaterialWFACClassMethod.PartslistACClassMethod_MaterialWFACClassMethod.Where(d => d.PartslistID == endBatchPos.ProdOrderPartslist.PartslistID).Any())
                                .FirstOrDefault();
                        }
                    }

                    if (matWFConnection == null)
                    {
                        // Error50277: No relation defined between Workflownode and intermediate material in Materialworkflow
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(40)", 761, "Error50277");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return StartNextCompResult.CycleWait;
                    }

                    // Find intermediate position which is assigned to this Dosing-Workflownode
                    var currentProdOrderPartslist = endBatchPos.ProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
                    ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                        .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                            && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                            && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
                    if (intermediatePosition == null)
                    {
                        // Error50278: Intermediate product line not found which is assigned to this workflownode.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(50)", 778, "Error50278");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return StartNextCompResult.CycleWait;
                    }

                    ProdOrderPartslistPos intermediateChildPos = null;
                    // Lock, if a parallel Dosing also creates a child Position for this intermediate Position

                    using (ACMonitor.Lock(pwMethodProduction._62000_PWGroupLockObj))
                    {
                        // Find intermediate child position, which is assigned to this Batch
                        intermediateChildPos = intermediatePosition.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                            .Where(c => c.ProdOrderBatchID.HasValue
                                        && c.ProdOrderBatchID.Value == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID)
                            .FirstOrDefault();

                        // If intermediate child position not found, generate childposition for this Batch/Intermediate
                        if (intermediateChildPos == null)
                        {
                            List<object> resultNewEntities = new List<object>();
                            msg = ProdOrderManager.BatchCreate(dbApp, intermediatePosition, batch, endBatchPos.BatchFraction, batch.BatchSeqNo, resultNewEntities); // Toleranz ist max. ein Batch mehr
                            if (msg != null)
                            {
                                Messages.LogException(this.GetACUrl(), "StartManualWeighingProd(60)", msg.InnerMessage);
                                dbApp.ACUndoChanges();
                                return StartNextCompResult.CycleWait;
                            }
                            else
                            {
                                dbApp.ACSaveChanges();
                            }
                            intermediateChildPos = resultNewEntities.Where(c => c is ProdOrderPartslistPos).FirstOrDefault() as ProdOrderPartslistPos;
                            if (intermediateChildPos != null && endBatchPos.FacilityLot != null)
                                endBatchPos.FacilityLot = endBatchPos.FacilityLot;
                        }
                    }
                    if (intermediateChildPos == null)
                    {
                        //Error50279:intermediateChildPos is null.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(70)", 1238, "Error50279");
                        ActivateProcessAlarmWithLog(msg, false);
                        return StartNextCompResult.CycleWait;
                    }

                    IntermediateChildPos = intermediateChildPos;

                    ProdOrderPartslistPosRelation[] queryOpenMaterials = OnGetAllMaterials(dbIPlus, dbApp, intermediateChildPos);
                    if ((ComponentsSeqFrom > 0 || ComponentsSeqTo > 0) && queryOpenMaterials != null && queryOpenMaterials.Any())
                        queryOpenMaterials = queryOpenMaterials.Where(c => c.Sequence >= ComponentsSeqFrom && c.Sequence <= ComponentsSeqTo)
                                                            .OrderBy(c => c.Sequence)
                                                            .ToArray();
                    if (queryOpenMaterials == null || !queryOpenMaterials.Any())
                    {
                        //Error50280: queryOpenMaterials is null or does not contain any material.
                        //msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(80)", 834, "Error50280");
                        //Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.Message);
                        //OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return StartNextCompResult.Done;
                    }

                    using (ACMonitor.Lock(_65050_WeighingCompLock))
                    {
                        WeighingComponents = queryOpenMaterials.Select(c => new WeighingComponent(c, DetermineWeighingComponentState(c.MDProdOrderPartslistPosState
                                                                                                                                      .MDProdOrderPartslistPosStateIndex))).ToList();

                        //_WeighingComponentsInfo = WeighingComponents.ToDictionary(c => c.PLPosRelation.ToString(), c => c.WeighState.ToString());
                    }

                    //Route[] routes;
                    AvailableRoutes = GetAvailableStorages(module);
                    //AvailableRoutes = routes;
                    //if (AvailableStorages == null || !AvailableStorages.Any())
                    //{
                    //    //Error50271: No source storage bins have been defined for this workcenter {0}!
                    //    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeihgingProd", 1268, "Error50271", module.GetACUrl());
                    //    ActivateProcessAlarmWithLog(msg);
                    //}
                }
            }

            return StartNextCompResult.NextCompStarted;
        }

        public static short DetermineWeighingComponentState(short posStateIndex)
        {
            if (posStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed)
                return (short)WeighingComponentState.WeighingCompleted;

            if(posStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled)
                return (short)WeighingComponentState.Aborted;

            return (short)WeighingComponentState.ReadyToWeighing;
        }

        public virtual void SetACMethodValues(ACMethod acMethod)
        {
            acMethod["TargetQuantity"] = 0.0;
            acMethod[Material.ClassName] = "";
        }

        //TODO:Get component only for pwnode, in bso get from database directy in one query
        protected virtual ProdOrderPartslistPosRelation[] OnGetAllMaterials(Database dbIPlus, DatabaseApp dbApp, ProdOrderPartslistPos intermediateChildPos)
        {
            ProdOrderPartslistPosRelation[] queryOpenDosings = dbApp.ProdOrderPartslistPosRelation.Include(c => c.SourceProdOrderPartslistPos)
                                                .Include(c => c.SourceProdOrderPartslistPos.Material)
                                                .Include(c => c.SourceProdOrderPartslistPos.Material.BaseMDUnit)
                                                .Where(c => c.TargetProdOrderPartslistPosID == intermediateChildPos.ProdOrderPartslistPosID)
                                                .ToArray()
            //ProdOrderPartslistPosRelation[] queryOpenDosings = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray()
                                .Where(c => c.RemainingDosingQuantityUOM < (MinWeightQuantity * -1) && c.MDProdOrderPartslistPosState != null
                                        && (c.SourceProdOrderPartslistPos != null && c.SourceProdOrderPartslistPos.Material != null
                                         && c.SourceProdOrderPartslistPos.Material.UsageACProgram))
                                .OrderBy(c => c.Sequence)
                                .ToArray();
            return queryOpenDosings;
        }

        private bool RefreshCompStateFromDBAndCheckIsAllCompleted()
        {
            bool result = true;

            using (ACMonitor.Lock(_65050_WeighingCompLock))
            {
                if (WeighingComponents == null)
                    return result;

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    foreach (WeighingComponent comp in WeighingComponents)
                    {
                        ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation.Include(s => s.MDProdOrderPartslistPosState)
                                                                                               .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == comp.PLPosRelation);
                        if (rel != null)
                        {
                            short newState = DetermineWeighingComponentState(rel.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex);
                            if (newState == (short)WeighingComponentState.ReadyToWeighing)
                            {
                                comp.SwitchState((WeighingComponentState)newState);

                                //comp.WeighState = newState;
                                comp.TargetQuantity = Math.Abs(rel.RemainingDosingQuantityUOM);
                                //if(_WeighingComponentsInfo != null && _WeighingComponentsInfo.ContainsKey(comp.PLPosRelation.ToString()))
                                //    _WeighingComponentsInfo[comp.PLPosRelation.ToString()] = newState.ToString();
                                SetInfo(comp, WeighingComponentInfoType.State, null, null);
                            }
                        }

                        if (comp.WeighState != (short)WeighingComponentState.WeighingCompleted && comp.WeighState != (short)WeighingComponentState.Aborted)
                        {
                            result = false;
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region Methods => FacilityCharge

        public Msg SetFacilityCharge(Guid? facilityChargeID, Guid? plPosRelationID, bool forceSet = false)
        {
            if (!facilityChargeID.HasValue)
            {
                CurrentFacilityCharge = facilityChargeID;
                return null;
            }

            Msg msg = null;

            if (!forceSet)
            {
                msg = OnFacilityChargeSet(facilityChargeID, plPosRelationID);
                if (msg != null)
                    return msg;

                msg = ValidateFacilityCharge(facilityChargeID, plPosRelationID);
                if (msg != null)
                    return msg;
            }

            CurrentFacilityCharge = facilityChargeID;
            return null;
        }

        public virtual Msg OnFacilityChargeSet(Guid? facilityChargeID, Guid? plPosRelationID)
        {
            return null;
        }

        //TODO Ivan: Last used and Expiration first
        public virtual Msg ValidateFacilityCharge(Guid? facilityChargeID, Guid? plPosRelationID)
        {
            if (LotValidation != null && LotValidation != LotUsageEnum.None)
            {
                if (LotValidation == LotUsageEnum.FIFO || LotValidation == LotUsageEnum.LIFO)
                {
                    return ValidateFacilityChargeFIFOorLIFO(facilityChargeID, plPosRelationID);
                }
                else if (LotValidation == LotUsageEnum.LastUsed)
                {

                }
                else if (LotValidation == LotUsageEnum.ExpirationFirst)
                {

                }
            }
            return null;
        }

        private Msg ValidateFacilityChargeFIFOorLIFO(Guid? facilityChargeID, Guid? plPosRelationID, bool isLIFO = false)
        {
            if (!facilityChargeID.HasValue)
                return new Msg(eMsgLevel.Error, "The parameter facilityChargeID is null!");

            if (!plPosRelationID.HasValue)
                return new Msg(eMsgLevel.Error, "The parameter plPosRelationID is null!");

            using (Database db = new core.datamodel.Database())
            using (DatabaseApp dbApp = new DatabaseApp(db))
            {
                FacilityCharge newFacilityCharge = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == facilityChargeID);
                if (newFacilityCharge == null)
                {
                    //Error50376: The quant {0} doesn't exist in the database!
                    return new Msg(this, eMsgLevel.Error, PWClassName, "ValidateFacilityChargeFIFOorLIFO(30)", 30, "Error50376", facilityChargeID);
                }

                ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == plPosRelationID);

                if (rel == null)
                {
                    //Error50374: ProdOrderPartslistPosRelation {0} doesn't exist in the database.
                    return new Msg(this, eMsgLevel.Error, PWClassName, "ValidateFacilityChargeFIFOorLIFO(40)", 40, "Error50374", plPosRelationID);
                }

                IEnumerable<FacilityCharge> relevantFacilityCharges = GetFacilityChargesForMaterial(dbApp, rel);

                if (relevantFacilityCharges != null && relevantFacilityCharges.Any())
                {
                    if (!isLIFO)
                    {
                        FacilityCharge oldestFacilityCharge = relevantFacilityCharges.OrderBy(c => c.FillingDate).FirstOrDefault();
                        if (   oldestFacilityCharge != null
                            && oldestFacilityCharge != newFacilityCharge)
                        {
                            //Question50051: The scanned or selected batch (Quant) is not the oldest in the warehouse. Do you still want to use this batch?
                            // Die gescannte bzw. ausgewählte Charge (Quant) ist nicht die älteste im Lager. Möchten Sie diese Charge dennoch verwenden?
                            return new Msg(this, eMsgLevel.Question, PWClassName, "ValidateFacilityChargeFIFOorLIFO(50)", 50, "Question50051")
                                            { MessageButton = eMsgButton.YesNo };
                        }
                    }
                    else
                    {
                        FacilityCharge youngestFacilityCharge = relevantFacilityCharges.OrderByDescending(c => c.FillingDate).FirstOrDefault();
                        if (   youngestFacilityCharge != null
                            && youngestFacilityCharge != newFacilityCharge)
                        {
                            //Question50052: The scanned or selected batch (Quant) is not the youngest in the warehouse. Do you still want to use this batch?
                            // Die gescannte bzw. ausgewählte Charge (Quant) ist nicht die jüngste im Lager. Möchten Sie diese Charge dennoch verwenden?
                            return new Msg(this, eMsgLevel.Question, PWClassName, "ValidateFacilityChargeFIFOorLIFO(60)", 60, "Question50052")
                                    { MessageButton = eMsgButton.YesNo };
                        }
                    }
                }
            }
            return null;
        }

        public Msg SetFacility(Guid? facilityID, Guid? plPosRelationID)
        {
            if(!facilityID.HasValue)
            {
                CurrentFacility = facilityID;
                return null;
            }

            Msg msg = OnSetFacility(facilityID, plPosRelationID);
            if (msg != null)
                return msg;

            CurrentFacility = facilityID;
            return null;
        }

        public virtual Msg OnSetFacility(Guid? facilityID, Guid? plPosRelationID)
        {
            return null;
        }

        [ACMethodInfo("", "", 999)]
        public ACValueList GetAvailableFacilityCharges(Guid PLPosRelation)
        {
            using (Database db = new Database())
            using (DatabaseApp dbApp = new DatabaseApp(db))
            {
                ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation
                                                    .Include(c => c.SourceProdOrderPartslistPos)
                                                    .Include(c => c.SourceProdOrderPartslistPos.Material)
                                                    .Include(c => c.SourceProdOrderPartslistPos.Material.BaseMDUnit)
                                                    .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == PLPosRelation);
                if (rel != null)
                {
                    var facilityChargeList = GetFacilityChargesForMaterial(dbApp, rel);
                    return new ACValueList(facilityChargeList.Select(c => new ACValue("ID", c.FacilityChargeID)).ToArray());
                }
                return null;
            }
        }

        private IEnumerable<FacilityCharge> GetFacilityChargesForMaterial(DatabaseApp dbApp, ProdOrderPartslistPosRelation posRel)
        {
            IEnumerable<Facility> facilities = GetAvailableFacilitiesForMaterial(dbApp, posRel);

            return FacilityChargeListQuery(facilities, posRel.SourceProdOrderPartslistPos.MaterialID);
        }

        public IEnumerable<Facility> GetAvailableFacilitiesForMaterial(DatabaseApp dbApp, ProdOrderPartslistPosRelation posRel)
        {
            if (ParentPWGroup == null || ParentPWGroup.AccessedProcessModule == null || PartslistManager == null)
            {
                throw new NullReferenceException("AccessedProcessModule is null");
            }

            IList<Facility> facilities;

            core.datamodel.ACClass accessAClass = ParentPWGroup.AccessedProcessModule.ComponentClass;
            IEnumerable<Route> routes = PartslistManager.GetRoutes(posRel, dbApp, dbApp.ContextIPlus,
                                                                    accessAClass,
                                                                    ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                                                    null,
                                                                    out facilities,
                                                                    null,
                                                                    null,
                                                                    null,
                                                                    false);

            if (routes == null || facilities == null)
                return new List<Facility>();

            var routeList = routes.ToList();

            List<Facility> routableFacilities = new List<Facility>();

            PAFManualWeighing manWeigh = CurrentExecutingFunction<PAFManualWeighing>();
            if (manWeigh == null)
            {
                gip.core.datamodel.ACClassMethod refPAACClassMethod = null;
                using (ACMonitor.Lock(this.ContextLockForACClassWF))
                {
                    refPAACClassMethod = this.ContentACClassWF.RefPAACClassMethod;
                }

                ACMethod acMethod = refPAACClassMethod.TypeACSignature();
                if (acMethod != null)
                {
                    PAProcessModule module = ParentPWGroup.AccessedProcessModule;
                    manWeigh = CanStartProcessFunc(module, acMethod) as PAFManualWeighing;
                }
            }

            foreach (Route currRoute in routes)
            {
                RouteItem lastRouteItem = currRoute.Items.LastOrDefault();
                if (lastRouteItem != null && lastRouteItem.TargetProperty != null)
                {
                    // Gehe zur nächsten Komponente, weil es mehrere Dosierfunktionen gibt und der Eingangspunkt des Prozessmoduls nicht mit dem Eingangspunkt dieser Funktion übereinstimmt.
                    // => eine andere Funktion ist dafür zuständig
                    if (manWeigh != null && !manWeigh.PAPointMatIn1.ConnectionList.Where(c => ((c as PAEdge).Source as PAPoint).ACIdentifier == lastRouteItem.TargetProperty.ACIdentifier).Any())
                    {
                        routeList.Remove(currRoute);
                    }
                    else
                    {
                        RouteItem source = currRoute.GetRouteSource();
                        if (source != null)
                        {
                            Facility facilityToAdd = facilities.FirstOrDefault(c => c.VBiFacilityACClassID.HasValue && c.VBiFacilityACClassID == source.SourceGuid);
                            if (facilityToAdd != null)
                                routableFacilities.Add(facilityToAdd);
                        }
                    }
                }
            }

            if (!IncludeContainerStores)
            {
                routableFacilities = routableFacilities.Where(c => c.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBin).ToList();
            }

            return routableFacilities;
        }

        /// <summary>
        /// Attention: Deatached from db context
        /// </summary>
        public Route[] AvailableRoutes
        {
            get;
            set;
        }

        ///// <summary>
        ///// Represents the storage module ACClassID
        ///// </summary>
        //public Guid[] AvailableStorages
        //{
        //    get;
        //    set;
        //}

        public virtual Route[] GetAvailableStorages(ACComponent toComponent)
        {
            if (!IsRoutingServiceAvailable)
            {
                //Error50375: The routing service is not available!
                ActivateProcessAlarmWithLog(new Msg(this, eMsgLevel.Error, PWClassName, "GetManualScaleStorages(10)", 1455, "Error50375"));
                return null;
            }

            using (Database db = new core.datamodel.Database())
            {
                RoutingResult routeResult = ACRoutingService.FindSuccessors(RoutingService, db, false, toComponent,
                                        PAMSilo.SelRuleID_Storage, RouteDirections.Backwards, new object[] { },
                                        (c, p, r) => c.ObjectFullType == typeof(PAMParkingspace),
                                        (c, p, r) => c.ObjectFullType == typeof(PAProcessModule), 
                                        0, true, true);

                if (routeResult != null)
                {
                    if (routeResult.Message != null && (routeResult.Routes == null || !routeResult.Routes.Any()))
                    {
                        //Error50271: No source storage bins have been defined for this workcenter {0}!
                        ActivateProcessAlarmWithLog(new Msg(this, eMsgLevel.Error, PWClassName, "", 1316, "Error50271", toComponent.GetACUrl()));
                        return null;
                    }

                    return routeResult.Routes.ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// return true if there are any quants in the store
        /// </summary>
        /// <param name="materialID"></param>
        /// <returns></returns>
        private bool TryAutoSelectFacilityCharge(Guid? materialID)
        {
            using (Database db = new core.datamodel.Database())
            using (DatabaseApp dbApp = new DatabaseApp(db))
            {
                ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation.Include(c => c.SourceProdOrderPartslistPos.Material)
                                                                                       .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == materialID);
                if (rel != null)
                {
                    Material mat = rel.SourceProdOrderPartslistPos.Material;
                    if (mat != null)
                    {
                        Msg msg = null;
                        var availableFC = GetFacilityChargesForMaterial(dbApp, rel);
                        if (!AutoSelectLot)
                            return availableFC.Any();

                        if (mat.IsLotManaged)
                        {
                            switch (AutoSelectLotPriority)
                            {
                                case LotUsageEnum.FIFO:
                                    availableFC = availableFC.OrderBy(c => c.FillingDate.HasValue).ThenBy(c => c.FillingDate).ToArray();
                                    break;
                                case LotUsageEnum.ExpirationFirst:
                                    availableFC = availableFC.OrderBy(c => c.ExpirationDate.HasValue).ThenBy(c => c.ExpirationDate).ToArray();
                                    break;
                                case LotUsageEnum.LastUsed:
                                    availableFC = TryGetLastUsedLot(availableFC, mat.MaterialID, dbApp);
                                    break;
                                case LotUsageEnum.LIFO:
                                    availableFC = availableFC.OrderByDescending(c => c.FillingDate.HasValue).ThenByDescending(c => c.FillingDate).ToArray();
                                    break;
                            }
                        }

                        foreach (var fc in availableFC)
                        {
                            msg = SetFacilityCharge(fc.FacilityChargeID, materialID);
                            if (msg == null)
                                return true;
                        }
                        //}
                        //else
                        //{
                        //    Msg msg = null;
                        //    var availableFacilities = GetFacilitiesForMaterial(rel);

                        //    foreach(var f in availableFacilities)
                        //    {
                        //        msg = SetFacility(f.FacilityID, materialID);
                        //        if (msg == null)
                        //            return;
                        //    }
                        //}
                    }
                }
            }
            return false;
        }

        private IEnumerable<FacilityCharge> TryGetLastUsedLot(IEnumerable<FacilityCharge> facilityCharges, Guid materialID, DatabaseApp dbApp)
        {
            Guid? moduleID = ParentPWGroup?.AccessedProcessModule?.ComponentClass?.ACClassID;

            if (!moduleID.HasValue)
                return new List<FacilityCharge>();

            MaterialConfig lastUsedFC = dbApp.MaterialConfig.FirstOrDefault(c => c.VBiACClassID == moduleID && c.MaterialID == materialID
                                                                                                            && c.KeyACUrl == MaterialConfigLastUsedLotKeyACUrl);

            if (lastUsedFC == null || lastUsedFC.Value == null)
                return new List<FacilityCharge>();

            Guid? fcID = lastUsedFC.Value as Guid?;
            if (fcID.HasValue)
            {
                FacilityCharge fc = facilityCharges.FirstOrDefault(c => c.FacilityChargeID == fcID);
                if (fc != null)
                    return new List<FacilityCharge>() { fc };
            }

            return new List<FacilityCharge>();
        }

        protected void SaveLastUsedLot(Guid? facilityCharge, Guid? materialID)
        {
            if (!facilityCharge.HasValue || !materialID.HasValue)
                return;

            if (AutoSelectLotPriority == LotUsageEnum.LastUsed || LotValidation == LotUsageEnum.LastUsed)
            {
                Guid? moduleID = ParentPWGroup?.AccessedProcessModule?.ComponentClass?.ACClassID;
                if (!moduleID.HasValue)
                    return;

                ApplicationManager?.ApplicationQueue.Add(() =>
                {
                    using (Database db = new core.datamodel.Database())
                   using (DatabaseApp dbApp = new DatabaseApp(db))
                   {
                       ProdOrderPartslistPosRelation posRel = dbApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == materialID);
                       if (posRel == null)
                           return;

                       MaterialConfig lastUsedFC = dbApp.MaterialConfig.FirstOrDefault(c => c.VBiACClassID == moduleID && c.MaterialID == posRel.SourceProdOrderPartslistPos.MaterialID
                                                                                                                       && c.KeyACUrl == MaterialConfigLastUsedLotKeyACUrl);

                       if (lastUsedFC == null)
                       {
                           lastUsedFC = MaterialConfig.NewACObject(dbApp, null);
                           lastUsedFC.MaterialID = posRel.SourceProdOrderPartslistPos.MaterialID;
                           lastUsedFC.VBiACClassID = moduleID;
                           lastUsedFC.KeyACUrl = MaterialConfigLastUsedLotKeyACUrl;
                            dbApp.MaterialConfig.AddObject(lastUsedFC);
                            lastUsedFC.SetValueTypeACClass(db.GetACType(typeof(Guid)));
                       }

                       Guid? lastFCID = lastUsedFC.Value as Guid?;
                       if (!lastFCID.HasValue || lastFCID.Value != facilityCharge.Value)
                       {
                           lastUsedFC.Value = facilityCharge;
                       }

                       Msg msg = dbApp.ACSaveChanges();
                   }
                });
            }    
        }

        private void SelectFacilityChargeOrFacility(Guid? componentID, WeighingComponentInfoType infoType)
        {
            try
            {
                Msg msg = null;
                PAFManualWeighing paf = CurrentExecutingFunction<PAFManualWeighing>();
                if (paf == null)
                {
                    //Error50272: CurrentExecutingFunction is null.
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "SelectFacilityChargeOrFacility(10)", 1390, "Error50272");
                }

                if (msg == null)
                {
                    Guid? currentOpenMaterial = CurrentOpenMaterial;

                    WeighingComponent comp = GetWeighingComponent(currentOpenMaterial); // WeighingComponents.FirstOrDefault(c => c.PLPosRelation == CurrentOpenMaterial.Value);
                    if (comp == null)
                    {
                        //Error50270: The component {0} doesn't exist for weighing.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "SelectFacilityChargeOrFacility(20)", 1563, "Error50270", currentOpenMaterial.Value);
                    }
                }

                if (msg == null)
                {
                    Guid? currentFacilityCharge = CurrentFacilityCharge;
                    msg = InitializePAFACMethod(paf.CurrentACMethod.ValueT, currentFacilityCharge);
                }

                if (msg != null)
                    ActivateProcessAlarmWithLog(msg);
            }
            catch (Exception e)
            {
                ActivateProcessAlarmWithLog(new Msg(e.Message, this, eMsgLevel.Exception, PWClassName, "SelectFacilityChargeOrFacility(30)", 1575));
            }
        }

        private MsgWithDetails DoAutoInsertQuantToStore(WeighingComponent weighingComponent)
        {
            MsgWithDetails msg = null;
            if (weighingComponent == null)
                return null;
            try
            {
                using (Database db = new Database())
                using (DatabaseApp dbApp = new DatabaseApp(db))
                {
                    Material material = dbApp.Material.Where(c => c.MaterialID == weighingComponent.Material).FirstOrDefault();
                    if (material == null)
                        return null;

                    ProdOrderPartslistPosRelation posRel = dbApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == weighingComponent.PLPosRelation);
                    if (posRel == null)
                        return null;

                    var facilityChargeList = GetFacilityChargesForMaterial(dbApp, posRel);
                    if (!facilityChargeList.Any() && !String.IsNullOrEmpty(AutoInsertQuantToStore))
                    {
                        ACComponent storeComponent = ACUrlCommand(AutoInsertQuantToStore) as ACComponent;
                        if (storeComponent != null)
                        {
                            Facility store = dbApp.Facility.Where(c => c.VBiFacilityACClassID.HasValue && c.VBiFacilityACClassID.Value == storeComponent.ComponentClass.ACClassID).FirstOrDefault();
                            if (store != null)
                            {
                                ACMethodBooking bookingParam = InventoryNewQuant;
                                if (bookingParam != null)
                                {
                                    bookingParam.InwardMaterial = material;
                                    bookingParam.InwardFacility = store;
                                    bookingParam.InwardQuantity = 1000000;
                                    if (material.IsLotManaged)
                                    {
                                        string secondaryKey = Root.NoManager.GetNewNo(db, typeof(FacilityLot), FacilityLot.NoColumnName, FacilityLot.FormatNewNo, this);
                                        FacilityLot fl = FacilityLot.NewACObject(dbApp, null, secondaryKey);
                                        fl.Material = material;
                                        bookingParam.InwardFacilityLot = fl;
                                    }

                                    msg = dbApp.ACSaveChangesWithRetry();
                                    // 2. Führe Buchung durch
                                    if (msg != null)
                                    {
                                        Messages.LogError(this.GetACUrl(), "DoAutoInsertQuantToStore(1100)", msg.InnerMessage);
                                        OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoAutoInsertQuantToStore", 1100), true);
                                    }
                                    else
                                    {
                                        ACMethodEventArgs resultBooking = ACFacilityManager.BookFacilityWithRetry(ref bookingParam, dbApp);
                                        if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                                        {
                                            Messages.LogError(this.GetACUrl(), "DoAutoInsertQuantToStore(1110)", bookingParam.ValidMessage.InnerMessage);
                                            OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoAutoInsertQuantToStore", 1110), true);
                                        }
                                        else
                                        {
                                            if (!bookingParam.ValidMessage.IsSucceded() || bookingParam.ValidMessage.HasWarnings())
                                            {
                                                Messages.LogError(this.GetACUrl(), "DoAutoInsertQuantToStore(1120)", bookingParam.ValidMessage.InnerMessage);
                                                OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoAutoInsertQuantToStore", 1120), true);
                                            }
                                            msg = dbApp.ACSaveChangesWithRetry();
                                            if (msg != null)
                                            {
                                                OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1130), true);
                                                Messages.LogError(this.GetACUrl(), "DoAutoInsertQuantToStore(1130)", msg.Message);
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.LogException(this.GetACUrl(), "GetAvailableFacilityCharges(0)", ex);
            }
            return msg;
        }

        private void UpdatePAFACMethod(WeighingComponent weighingComponent, ACMethod acMethod)
        {
            if (weighingComponent != null)
            {
                ManualWeihgingNextTask.ValueT = ManualWeighingTaskInfo.None;
                acMethod["TargetQuantity"] = weighingComponent.TargetQuantity;
                acMethod[Material.ClassName] = weighingComponent.MaterialName;
                acMethod["PLPosRelation"] = weighingComponent.PLPosRelation;

                Guid? currentFacilityCharge = CurrentFacilityCharge;

                if (currentFacilityCharge.HasValue || CurrentFacility.HasValue)
                    InitializePAFACMethod(acMethod, currentFacilityCharge);

                bool isLast;
                using(ACMonitor.Lock(_65050_WeighingCompLock))
                {
                    isLast = WeighingComponents.Count(c => c.WeighState == (short)WeighingComponentState.ReadyToWeighing) == 1;
                }

                acMethod["IsLastWeighingMaterial"] = isLast;
            }
        }

        private Msg InitializePAFACMethod(ACMethod acMethod, Guid? currentFacilityCharge)
        {
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                Facility facility = null;

                if (currentFacilityCharge.HasValue)
                {
                    FacilityCharge fc = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == currentFacilityCharge);
                    if (fc == null)
                    {
                        //Error50376: The quant {0} doesn't exist in the database!
                        return new Msg(this, eMsgLevel.Error, PWClassName,"InitializePAFACMethod(10)", 1607, "Error50376", currentFacilityCharge);
                    }

                    //if (fc.Facility == null)
                    //{
                    //    //Error50361: On the FacilityCharge {0}, Facility is null!
                    //    return new Msg(this, eMsgLevel.Error, PWClassName, "InitializePAFACMethod(20)", 1613, "Error50361", fc.ACCaption);
                    //}
                    facility = fc.Facility;
                }
                else if (CurrentFacility.HasValue)
                    facility = dbApp.Facility.FirstOrDefault(c => c.FacilityID == CurrentFacility);

                if (facility == null || facility.VBiFacilityACClass == null)
                {
                    //Error50377: The facility-variable is null or a module(VBiFacilityACClass) isn't assigned to the Facility!
                    return new Msg(this, eMsgLevel.Error, PWClassName, "InitializePAFACMethod(30)", 1623, "Error50377");
                }

                Route route = AvailableRoutes?.FirstOrDefault(c => c.GetRouteSource().SourceGuid == facility.VBiFacilityACClassID);
                if (route == null)
                {
                    //Error50363: Can't find a Route in the AvailableRoutes with RouteSourceID: {0}
                    return new Msg(this, eMsgLevel.Error, PWClassName, "InitializePAFACMethod(40)", 1360, "Error50363", facility.VBiFacilityACClassID);
                }
                acMethod.ParameterValueList["Route"] = route;
            }
            if (currentFacilityCharge.HasValue)
                acMethod.ParameterValueList["FacilityCharge"] = currentFacilityCharge.Value;
            else if (CurrentFacility.HasValue)
                acMethod.ParameterValueList["Facility"] = CurrentFacility.Value;


            return null;
        }

        #endregion

        #region Methods => StartWeighingComponent

        private StartNextCompResult StartManualWeighingNextComp(PAProcessModule module, WeighingComponent weighingComponent, bool? hasQuantsChecked)
        {
            Msg msg = null;

            if (module == null)
                return StartNextCompResult.CycleWait;

            if (   (!hasQuantsChecked.HasValue || !hasQuantsChecked.Value)
                && !String.IsNullOrEmpty(AutoInsertQuantToStore))
            {
                DoAutoInsertQuantToStore(weighingComponent);
            }

            gip.core.datamodel.ACClassMethod refPAACClassMethod = null;
            using (ACMonitor.Lock(this.ContextLockForACClassWF))
            {
                refPAACClassMethod = this.ContentACClassWF.RefPAACClassMethod;
            }

            ACMethod acMethod = refPAACClassMethod.TypeACSignature();

            if (acMethod == null)
            {
                //Error50281: acMethod is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingNextComp(10)", 1667, "Error50281");
                OnNewAlarmOccurred(ProcessAlarm, msg, false);
                return StartNextCompResult.CycleWait;
            }

            if (!OnStartManualWeighingNextComp(weighingComponent, acMethod))
                return StartNextCompResult.CycleWait;

            PAProcessFunction responsibleFunc = CanStartProcessFunc(module, acMethod);
            if (responsibleFunc == null)
                return StartNextCompResult.CycleWait;

            if (!(bool)ExecuteMethod("GetConfigForACMethod", acMethod, true))
                return StartNextCompResult.CycleWait;

            //TODO: get target scale from partslistconfig if is rule configured for this material

            if (weighingComponent != null)
            {
                acMethod["TargetQuantity"] = weighingComponent.TargetQuantity;
                acMethod[Material.ClassName] = weighingComponent.MaterialName;
                acMethod["PLPosRelation"] = weighingComponent.PLPosRelation;
                acMethod["Route"] = new Route();

                Guid? currentFacilityCharge = CurrentFacilityCharge;
                if (currentFacilityCharge.HasValue || CurrentFacility.HasValue)
                    InitializePAFACMethod(acMethod, currentFacilityCharge);

                bool isLast;
                using (ACMonitor.Lock(_65050_WeighingCompLock))
                {
                    isLast = WeighingComponents.Count(c => c.WeighState == (short)WeighingComponentState.ReadyToWeighing) == 1;
                }

                acMethod["IsLastWeighingMaterial"] = isLast;
            }
            else
            {
                acMethod["TargetQuantity"] = PWBinSelection.BinSelectionReservationQuantity;
                acMethod["Route"] = new Route();
            }

                if (!(bool)ExecuteMethod("AfterConfigForACMethodIsSet", acMethod, true))
                return StartNextCompResult.CycleWait;

            if (!acMethod.IsValid())
            {
                //Error50282: The function cannot be started due to incorrect parameters on order {0}, Bill of material {1}, line {2}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingNextComp(20)", 1703, "Error50282",
                                 weighingComponent.ErrorInfoProgramNo,
                                 weighingComponent.ErrorInfoPartslistNo,
                                 weighingComponent.ErrorInfoBookingMaterialName);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, false);
                return StartNextCompResult.CycleWait;
            }

            RecalcTimeInfo(true);
            if (CreateNewProgramLog(acMethod, _NewAddedProgramLog == null) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                return StartNextCompResult.CycleWait;
            _ExecutingACMethod = acMethod;

            module.TaskInvocationPoint.ClearMyInvocations(this);
            _CurrentMethodEventArgs = null;
            if (!IsTaskStarted(module.TaskInvocationPoint.AddTask(acMethod, this)))
            {
                ACMethodEventArgs eM = _CurrentMethodEventArgs;
                if (eM == null || eM.ResultState != Global.ACMethodResultState.FailedAndRepeat)
                {
                    //Error50282: The function cannot be started due to incorrect parameters on order {0}, Bill of material {1}, line {2}
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(110)", 1727, "Error50282",
                                weighingComponent.ErrorInfoProgramNo,
                                weighingComponent.ErrorInfoPartslistNo,
                                weighingComponent.ErrorInfoBookingMaterialName);

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, false);
                }
                return StartNextCompResult.CycleWait;
            }

            UpdateCurrentACMethod();

            _LastOpenMaterial = CurrentOpenMaterial;

            return StartNextCompResult.NextCompStarted;
        }

        public virtual bool OnStartManualWeighingNextComp(WeighingComponent component, ACMethod acMethod)
        {
            return true;
        }

        #endregion

        #region Methods => TaskCallback, Booking

        //TODO: When is PAFManualWeighing is reseted, skip booking and leave MDProdOrderPartslistPosRelationState untouched! (Test it)
        public override void TaskCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            _InCallback = true;
            if (e != null)
            {
                IACTask taskEntry = wrapObject as IACTask;
                ACMethodEventArgs eM = e as ACMethodEventArgs;
                _CurrentMethodEventArgs = eM;
                if (taskEntry.State == PointProcessingState.Deleted && CurrentACState != ACStateEnum.SMIdle)
                {
                    try
                    {
                        PAProcessModule module = sender.ParentACComponent as PAProcessModule;
                        if (module != null)
                        {
                            PAProcessFunction function = module.GetExecutingFunction<PAProcessFunction>(eM.ACRequestID);
                            if (function != null)
                            {
                                WeighingComponentState state = WeighingComponentState.WeighingCompleted;
                                Msg msg = null;
                                bool changeState = false;

                                Guid? currentOpenMaterial = CurrentOpenMaterial;
                                Guid? currentFacilityCharge = CurrentFacilityCharge;

                                if (function.CurrentACState == ACStateEnum.SMCompleted || function.CurrentACState == ACStateEnum.SMAborted || 
                                   (function.CurrentACState == ACStateEnum.SMIdle && function.LastACState == ACStateEnum.SMResetting))
                                {
                                    bool isComponentConsumed = false;
                                    ACValue isCC = e.GetACValue("IsComponentConsumed");
                                    if (isCC != null)
                                        isComponentConsumed = isCC.ParamAsBoolean;

                                    double? actQuantity = e.GetDouble("ActualQuantity");
                                    //double? tolerancePlus = (double)e.ParentACMethod["TolerancePlus"];
                                    double? toleranceMinus = (double)e.ParentACMethod["ToleranceMinus"];
                                    double? targetQuantity = (double)e.ParentACMethod["TargetQuantity"];

                                    bool isWeighingInTol = true;
                                    if (targetQuantity.HasValue && toleranceMinus.HasValue && actQuantity.HasValue && (actQuantity < (targetQuantity - toleranceMinus)))
                                        isWeighingInTol = false;

                                    if (actQuantity > 0.000001)
                                        msg = DoManualWeighingBooking(actQuantity, isWeighingInTol, false, currentFacilityCharge);

                                    if (isComponentConsumed)
                                    {
                                        Msg msgResult = SetRelationState(currentOpenMaterial, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed, true);
                                        if (msgResult != null)
                                            ActivateProcessAlarmWithLog(msgResult);
                                    }

                                    if (CurrentACMethod?.ValueT != null)
                                    {
                                        RecalcTimeInfo();
                                        FinishProgramLog(CurrentACMethod.ValueT);
                                    }

                                    if (_IsBinChangeActivated)
                                    {
                                        Reset();
                                        RaiseOutEvent();
                                    }

                                    changeState = true;
                                }

                                if (msg != null || function.CurrentACState == ACStateEnum.SMAborted || 
                                    (function.LastACState == ACStateEnum.SMResetting && function.CurrentACState == ACStateEnum.SMIdle && _IsAborted))
                                {
                                    using (ACMonitor.Lock(_65003_IsAbortedLock))
                                    {
                                        _IsAborted = false;
                                    }

                                    Msg msgResult = SetRelationState(currentOpenMaterial, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled);
                                    if (msgResult != null)
                                        ActivateProcessAlarmWithLog(msgResult);

                                    state = WeighingComponentState.Aborted;
                                    changeState = true;
                                }

                                if (currentOpenMaterial != null && changeState)
                                {
                                    WeighingComponent weighingComp = GetWeighingComponent(currentOpenMaterial); //WeighingComponents.FirstOrDefault(c => c.PLPosRelation == CurrentOpenMaterial);
                                    if (weighingComp != null)
                                    {
                                        weighingComp.SwitchState(state);
                                        SetInfo(weighingComp, WeighingComponentInfoType.State, currentFacilityCharge, CurrentFacility);
                                    }
                                }
                            }
                        }
                    }
                    catch(Exception exc)
                    {
                        Messages.LogException(this.GetACUrl(), "TaskCallback(10)", exc);
                    }
                    finally
                    {
                        SetCanStartFromBSO(true);
                        CurrentOpenMaterial = null;
                        CurrentFacility = null;
                        CurrentFacilityCharge = null;
                        SubscribeToProjectWorkCycle();
                    }
                }
                else if (taskEntry.State == PointProcessingState.Accepted && CurrentACState != ACStateEnum.SMIdle)
                {
                    PAProcessModule module = sender.ParentACComponent as PAProcessModule;
                    if (module != null)
                    {
                        PAProcessFunction function = module.GetExecutingFunction<PAProcessFunction>(eM.ACRequestID);
                        if (function != null)
                        {
                            if (function.CurrentACState == ACStateEnum.SMRunning)
                            {
                                Guid? currentOpenMaterial = CurrentOpenMaterial;

                                WeighingComponent comp = GetWeighingComponent(currentOpenMaterial); //WeighingComponents.FirstOrDefault(c => c.PLPosRelation == CurrentOpenMaterial.Value);
                                if (comp == null)
                                {
                                    //Error50270: The component {0} doesn't exist for weighing.
                                    ActivateProcessAlarmWithLog(new Msg(this, eMsgLevel.Error, PWClassName, "TaskCallback(20)", 1866, "Error50270", currentOpenMaterial));
                                    return;
                                }

                                if (comp.WeighState < (short)WeighingComponentState.InWeighing)
                                {
                                    comp.SwitchState(WeighingComponentState.InWeighing);

                                    WeighingComponentInfoType infoType = WeighingComponentInfoType.StateSelectCompAndFC_F;
                                    if (!FreeSelectionMode && !AutoSelectLot)
                                        infoType = WeighingComponentInfoType.StateSelectFC_F;

                                    Guid? currentFacilityCharge = CurrentFacilityCharge;
                                    SetInfo(comp, infoType, currentFacilityCharge.Value, CurrentFacility);
                                    SaveLastUsedLot(currentFacilityCharge, currentOpenMaterial);
                                }
                            }
                        }
                    }
                }
            }
            _InCallback = false;
        }

        protected Msg SetRelationState(Guid? plPosRelationID, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates targetState, bool setOnTopRelation = false)
        {
            Msg msg = null;
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                if (plPosRelationID != null)
                {
                    ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == plPosRelationID);
                    if (rel != null)
                    {
                        MDProdOrderPartslistPosState relState = dbApp.MDProdOrderPartslistPosState.FirstOrDefault(c => c.MDProdOrderPartslistPosStateIndex == (short)targetState);
                        if (relState != null)
                        {
                            if (setOnTopRelation)
                            {
                                if (rel.TopParentPartslistPosRelation != null)
                                    rel.TopParentPartslistPosRelation.MDProdOrderPartslistPosState = relState;
                            }
                            else
                                rel.MDProdOrderPartslistPosState = relState;

                            msg = dbApp.ACSaveChanges();
                        }
                    }
                    else
                    {
                        //Error50374: ProdOrderPartslistPosRelation {0} doesn't exist in the database.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "SetRelationState(10)", 1915, "Error50374", plPosRelationID);
                    }
                }
                else
                {
                    // Error50373: Manual weighing error, the property {0} is null!
                    // Fehler bei Handverwiegung, die Eigenschaft {0} ist null!
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "SetRelationState(20)", 1921, "Error50373", "CurrentOpenMaterial");
                }
            }
            return msg;
        }

        public Msg DoManualWeighingBooking(double? actualQuantity, bool thisWeighingIsInTol, bool isConsumedLot, Guid? currentFacilityCharge)
        {
            MsgWithDetails collectedMessages = new MsgWithDetails();
            Msg msg = null;

            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                try
                {
                    MDProdOrderPartslistPosState posState;
                    // Falls in Toleranz oder Dosierung abgebrochen ohne Grund, dann beende Position
                    if (thisWeighingIsInTol)
                        posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                    else
                        posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted).FirstOrDefault();

                    if (posState == null)
                    {
                        // Error50265: MDProdOrderPartslistPosState for Completed-State doesn't exist
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(1)", 1702, "Error50265");
                        ActivateProcessAlarmWithLog(msg, false);
                        return msg;
                    }

                    Guid? currentOpenMaterial = CurrentOpenMaterial;

                    if (currentOpenMaterial == null)
                    {
                        // Error50373: Manual weighing error, the property {0} is null!
                        // Fehler bei Handverwiegung, die Eigenschaft {0} ist null!
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(10)", 1953, "Error50373", "CurrentOpenMaterial");
                        ActivateProcessAlarmWithLog(msg, false);
                        return msg;
                    }

                    ProdOrderPartslistPosRelation weighingPosRelation = dbApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == currentOpenMaterial);
                    if (weighingPosRelation != null)
                    {
                        bool changePosState = false;

                        if (currentFacilityCharge == null && CurrentFacility == null)
                        {
                            // Error50373: Manual weighing error, the property {0} is null!
                            // Fehler bei Handverwiegung, die Eigenschaft {0} ist null!
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(20)", 1967, "Error50373", "currentFacilityCharge");
                            ActivateProcessAlarmWithLog(msg, false);
                            return msg;
                        }

                        FacilityCharge facilityCharge = null;
                        Facility facility = null;

                        if (currentFacilityCharge != null)
                        {
                            facilityCharge = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == currentFacilityCharge) as FacilityCharge;
                            if (facilityCharge == null)
                            {
                                // Error50367: The quant {0} doesn't exist in the database!
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(30)", 1981, "Error50367", currentFacilityCharge);
                                ActivateProcessAlarmWithLog(msg, false);
                                return msg;
                            }
                            facility = facilityCharge.Facility;
                        }
                        else
                        {
                            facility = dbApp.Facility.FirstOrDefault(c => c.FacilityID == CurrentFacility);
                            if (facility == null)
                            {
                                //Error50378: Can't get the Facility from database with FacilityID: {0}
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(31)", 1686, "Error50378", CurrentFacility);
                                ActivateProcessAlarmWithLog(msg, false);
                                return msg;
                            }
                        }

                        double targetQuantity = weighingPosRelation.TargetQuantityUOM;
                        WeighingComponent comp = GetWeighingComponent(weighingPosRelation.ParentProdOrderPartslistPosRelationID);  //WeighingComponents.FirstOrDefault(c => c.PLPosRelation == weighingPosRelation.ProdOrderPartslistPosRelationID);
                        if(comp != null)
                            targetQuantity = comp.TargetQuantity;

                        double calcActualQuantity = targetQuantity + weighingPosRelation.RemainingDosingQuantityUOM;
                        if (actualQuantity > calcActualQuantity)
                            actualQuantity = actualQuantity - calcActualQuantity;

                        if (actualQuantity > 0.000001)
                        {
                            FacilityPreBooking facilityPreBooking = ProdOrderManager.NewOutwardFacilityPreBooking(this.ACFacilityManager, dbApp, weighingPosRelation);
                            ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                            bookingParam.OutwardQuantity = (double)actualQuantity;
                            bookingParam.OutwardFacility = facility;
                            bookingParam.OutwardFacilityCharge = facilityCharge;
                            bookingParam.SetCompleted = isConsumedLot;
                            if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                                bookingParam.PropertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();
                            msg = dbApp.ACSaveChangesWithRetry();

                            if (msg != null)
                            {
                                collectedMessages.AddDetailMessage(msg);
                                ActivateProcessAlarmWithLog(new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(40)", 2020), false);
                                changePosState = false;
                            }
                            else if (facilityPreBooking != null)
                            {
                                bookingParam.IgnoreIsEnabled = true;
                                ACMethodEventArgs resultBooking = ACFacilityManager.BookFacilityWithRetry(ref bookingParam, dbApp);
                                if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                                {
                                    msg = new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(60)", 2045);
                                    ActivateProcessAlarm(msg, false);
                                    changePosState = false;
                                }
                                else
                                {
                                    if (!bookingParam.ValidMessage.IsSucceded() || bookingParam.ValidMessage.HasWarnings())
                                    {
                                        collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                        msg = new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(70)", 2053);
                                        ActivateProcessAlarmWithLog(msg, false);
                                        changePosState = false;
                                    }
                                    changePosState = true;
                                    if (bookingParam.ValidMessage.IsSucceded())
                                    {
                                        facilityPreBooking.DeleteACObject(dbApp, true);
                                        weighingPosRelation.IncreaseActualQuantityUOM(bookingParam.OutwardQuantity.Value);
                                        msg = dbApp.ACSaveChangesWithRetry();
                                        if (msg != null)
                                        {
                                            collectedMessages.AddDetailMessage(msg);
                                            msg = new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(80)", 2065);
                                            ActivateProcessAlarmWithLog(msg, false);
                                        }

                                        Messages.LogInfo(this.GetACUrl(), "", "ManualWeighingTrace - changePosState value: " + changePosState.ToString());

                                        if (changePosState)
                                        {
                                            Messages.LogInfo(this.GetACUrl(), "", "ManualWeighingTrace - posState value: " + posState.ToString());

                                            weighingPosRelation.MDProdOrderPartslistPosState = posState;
                                            if(posState != null && posState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed)
                                            {
                                                var unconfirmedBookings = weighingPosRelation.FacilityBooking_ProdOrderPartslistPosRelation
                                                                                             .Where(c => c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New);

                                                Messages.LogInfo(this.GetACUrl(), "", "ManualWeighingTrace - Bookings count with state new:" + unconfirmedBookings?.Count().ToString());

                                                foreach (var booking in unconfirmedBookings)
                                                {
                                                    booking.MaterialProcessState = GlobalApp.MaterialProcessState.Processed;
                                                    Messages.LogInfo(this.GetACUrl(), "", "ManualWeighingTrace - Booking change state");
                                                }
                                            }
                                        }

                                        msg = dbApp.ACSaveChangesWithRetry();
                                        if (msg != null)
                                        {
                                            collectedMessages.AddDetailMessage(msg);
                                            msg = new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(90)", 2094);
                                            ActivateProcessAlarmWithLog(msg, false);
                                        }
                                        else
                                        {
                                            weighingPosRelation.RecalcActualQuantityFast();
                                            if (dbApp.IsChanged)
                                                dbApp.ACSaveChanges();
                                        }
                                    }
                                    else
                                        collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                }
                            }
                        }
                        else
                        {
                            ProdOrderPartslistPos sourcePos = weighingPosRelation.SourceProdOrderPartslistPos;

                            // Error50269 The actual quantity for posting is too small. Order {0}, Bill of material {1}, line {2}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeihgingBooking(100)", 2112, "Error50269", sourcePos?.ProdOrderPartslist?.ProdOrder?.ProgramNo,
                                          sourcePos?.ProdOrderPartslist?.Partslist?.PartslistNo, sourcePos?.BookingMaterial?.MaterialNo);
                            ActivateProcessAlarmWithLog(msg, false);
                        }
                    }
                }
                catch (Exception e)
                {
                    msg = new Msg(e.Message, this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(120)", 2120);
                    ActivateProcessAlarmWithLog(msg, false);
                }
            }

            return collectedMessages.MsgDetailsCount > 0 ? collectedMessages : null;
        }

        public Msg DoFacilityChargeZeroBooking(Guid facilityCharge)
        {
            ACMethodBooking fbtZeroBooking = ZeroBookingFacilityCharge as ACMethodBooking;

            if (fbtZeroBooking == null)
            {
                //Error50364: Can not find the zero booking ACMethod!
                return new Msg(this, eMsgLevel.Error, PWClassName, "DoFacilityChargeZeroBooking(10)", 2133, "Error50364");
            }

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    FacilityCharge fc = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == facilityCharge);
                    if (fc == null)
                    {
                        //Error50376: The quant {0} doesn't exist in the database!
                        return new Msg(this, eMsgLevel.Error, PWClassName, "DoFacilityChargeZeroBooking(20)", 2146, "Error50376", facilityCharge);
                    }

                    fbtZeroBooking.InwardFacilityCharge = fc;
                    fbtZeroBooking.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(dbApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);

                    ACMethodEventArgs result = ACFacilityManager.BookFacility(fbtZeroBooking, dbApp);
                    if (!fbtZeroBooking.ValidMessage.IsSucceded() || fbtZeroBooking.ValidMessage.HasWarnings())
                    {
                        return fbtZeroBooking.ValidMessage;
                    }
                    else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
                    {
                        if (String.IsNullOrEmpty(result.ValidMessage.Message))
                            result.ValidMessage.Message = result.ResultState.ToString();

                        return result.ValidMessage;
                    }
                }
                catch (Exception e)
                {
                    string message = "";
                    if (e.InnerException != null)
                        message = String.Format("{0}, {1}", e.Message, e.InnerException.Message);
                    else
                        message = e.Message;
                    return new MsgWithDetails(message, this, eMsgLevel.Exception, PWClassName, "DoFacilityChargeZeroBooking", 2166);
                }
            }
            return null;
        }

        #endregion

        #region Methods => Misc.

        public T ParentPWMethod<T>() where T : PWMethodVBBase
        {
            if (ParentRootWFNode == null)
                return null;
            return ParentRootWFNode as T;
        }

        //public T CurrentExecutingFunction<T>() where T : PAProcessFunction
        //{
        //    IEnumerable<ACPointAsyncRMISubscrWrap<ACComponent>> taskEntries = null;

        //    using (ACMonitor.Lock(TaskSubscriptionPoint.LockLocalStorage_20033))
        //    {
        //        taskEntries = this.TaskSubscriptionPoint.ConnectionList.ToArray();
        //    }
        //    // Falls Dosierung zur Zeit aktiv ist, dann gibt es auch einen Eintrag in der TaskSubscriptionListe
        //    if (taskEntries != null && taskEntries.Any())
        //    {
        //        foreach (var entry in taskEntries)
        //        {
        //            T manualWeighing = ParentPWGroup.GetExecutingFunction<T>(entry.RequestID);
        //            if (manualWeighing != null)
        //                return manualWeighing;
        //        }
        //    }
        //    return null;
        //}

        public void SetInfo(WeighingComponent weighingComp, WeighingComponentInfoType infoType, Guid? facilityCharge, Guid? facility, bool dbAutoRefresh = false, 
                            bool lotChange = false)
        {
            using (ACMonitor.Lock(_65000_CurrentWeighingComponentInfoLock))
            {
                WeighingComponentInfo compInfo = new WeighingComponentInfo();
                compInfo.WeighingComponentInfoType = (short)infoType;
                compInfo.IsManualAddition = !IsManualWeighing;

                switch (infoType)
                {
                    case WeighingComponentInfoType.State:
                        {
                            compInfo.PLPosRelation = weighingComp.PLPosRelation;
                            compInfo.WeighingComponentState = weighingComp.WeighState;
                            break;
                        }
                    case WeighingComponentInfoType.StateSelectFC_F:
                    case WeighingComponentInfoType.StateSelectCompAndFC_F:
                        {
                            compInfo.PLPosRelation = weighingComp.PLPosRelation;
                            compInfo.WeighingComponentState = weighingComp.WeighState;
                            if (facilityCharge != null)
                                compInfo.FacilityCharge = facilityCharge;
                            else if (facility == null)
                                compInfo.Facility = facility;
                            break;
                        }
                    case WeighingComponentInfoType.SelectCompReturnFC_F:
                        {
                            compInfo.PLPosRelation = weighingComp.PLPosRelation;
                            break;
                        }
                    case WeighingComponentInfoType.SelectFC_F:
                        {
                            if (facilityCharge != null)
                                compInfo.FacilityCharge = facilityCharge;
                            else if (facility == null)
                                compInfo.Facility = facility;

                            compInfo.FC_FAutoRefresh = dbAutoRefresh;
                            compInfo.IsLotChange = lotChange;
                            compInfo.PLPosRelation = weighingComp.PLPosRelation;
                            compInfo.WeighingComponentState = weighingComp.WeighState;
                            break;
                        }
                }
                CurrentWeighingComponentInfo.ValueT = compInfo;
            }
        }

        public void SetCanStartFromBSO(bool canStart)
        {
            using (ACMonitor.Lock(_65001_CanStartFromBSOLock))
            {
                _CanStartFromBSO = canStart;
            }
        }

        public void ClearMyConfiguration()
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _MyConfiguration = null;
            }
            this.HasRules.ValueT = 0;
        }

        protected override void OnNewProgramLogAddedToQueue(ACMethod acMethod, gip.core.datamodel.ACProgramLog currentProgramLog)
        {
            //if (_NewAddedProgramLog == null)
            //{
            //    _NewAddedProgramLog = currentProgramLog;
            //    //ACClassTaskQueue.TaskQueue.ObjectContext.ACChangesExecuted += ACClassTaskQueue_ChangesSaved;
            //}
        }

        public WeighingComponent GetWeighingComponent(Guid? currentOpenMaterial)
        {
            using(ACMonitor.Lock(_65050_WeighingCompLock))
            {
                if (WeighingComponents == null)
                    return null;

                return WeighingComponents.FirstOrDefault(c => c.PLPosRelation == currentOpenMaterial);
            }
        }

        [ACMethodInfo("","",999)]
        public bool IsBinChangeLoopNodeAvailable()
        {
            if (PWPointOut == null || PWPointOut.ConnectionList == null || !PWPointOut.ConnectionList.Any())
                return false;

            return PWPointOut.ConnectionList.Any(c => c.ValueT is PWBinChangeLoop);
        }

        #endregion

        #endregion

        #region Enums

        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'Lot usage Enum'}de{'Lot usage Enum'}", Global.ACKinds.TACEnum, Global.ACStorableTypes.NotStorable, true, false)]
        [DataContract]
        public enum LotUsageEnum : short   
        {
            [EnumMember(Value = "LU0")]
            FIFO = 0,
            [EnumMember(Value = "LU10")]
            ExpirationFirst = 10,
            [EnumMember(Value = "LU20")]
            LastUsed = 20,
            [EnumMember(Value = "LU30")]
            LIFO = 30,
            [EnumMember(Value = "LU100")]
            None = 100,
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "StartWeighing":
                    result = StartWeighing(acParameter[0] as Guid?, acParameter[1] as Guid?, acParameter[2] as Guid?, (bool)acParameter[3]);
                    return true;
                case "CompleteWeighing":
                    CompleteWeighing((double)acParameter[0], (bool)acParameter[1]);
                    return true;
                case "TareScale":
                    TareScale();
                    return true;
                case "LotChange":
                    LotChange(acParameter[0] as Guid?, (double)acParameter[1], (bool)acParameter[2], (bool)acParameter[3]);
                    return true;
                case "BinChange":
                    BinChange();
                    return true;
                case "Abort":
                    Abort((bool)acParameter[0]);
                    return true;
                case "OnApplyManuallyEnteredLot":
                    result = OnApplyManuallyEnteredLot(acParameter[0] as string, (Guid)acParameter[1]);
                    return true;
                case "GetAvailableFacilityCharges":
                    result = GetAvailableFacilityCharges((Guid)acParameter[0]);
                    return true;
                //case "GetAvailableFacilities":
                //    result = GetAvailableFacilities((Guid)acParameter[0]);
                //    return true;
                case "SMResetting":
                    SMResetting();
                    return true;
                case "IsBinChangeLoopNodeAvailable":
                    result = IsBinChangeLoopNodeAvailable();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PWManualWeighing(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeProcessMethod(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        
        #endregion
    }

    #region Helper classes and enums

    public class WeighingComponent
    {
        public WeighingComponent(ProdOrderPartslistPosRelation posRelation, short weighState)
        {
            PLPosRelation = posRelation.ProdOrderPartslistPosRelationID;
            Material = posRelation.SourceProdOrderPartslistPos.MaterialID.Value;
            Sequence = posRelation.Sequence;
            WeighState = weighState;
            TargetQuantity = Math.Abs(posRelation.RemainingDosingQuantityUOM);
            MaterialName = posRelation.SourceProdOrderPartslistPos.Material?.MaterialName1;

            ErrorInfoPartslistNo = posRelation.TargetProdOrderPartslistPos?.ProdOrderPartslist?.Partslist?.PartslistNo;
            ErrorInfoProgramNo = posRelation.TargetProdOrderPartslistPos?.ProdOrderPartslist?.ProdOrder?.ProgramNo;
            ErrorInfoBookingMaterialName = posRelation.TargetProdOrderPartslistPos?.BookingMaterial?.MaterialName1;
        }

        public Guid PLPosRelation
        {
            get;
            set;
        }

        public Guid Material
        {
            get;
            set;
        }

        public double TargetQuantity
        {
            get;
            set;
        }

        public string MaterialName
        {
            get;
            set;
        }

        public int Sequence
        {
            get;
            set;
        }

        public short WeighState
        {
            get;
            private set;
        }

        public string ErrorInfoProgramNo
        {
            get;
            set;
        }

        public string ErrorInfoPartslistNo
        {
            get;
            set;
        }

        public string ErrorInfoBookingMaterialName
        {
            get;
            set;
        }

        public void SwitchState(WeighingComponentState state)
        {
            WeighState = (short)state;
            //if(info.ContainsKey(PLPosRelation.ToString()))
            //    info[PLPosRelation.ToString()] = WeighState.ToString();
        }
    }

    [ACSerializeableInfo]
    [DataContract]
    public class WeighingComponentInfo : ICloneable
    {
        [DataMember(Name = "A")]
        public short WeighingComponentInfoType
        {
            get;
            set;
        }

        [DataMember(Name = "B")]
        public Guid PLPosRelation
        {
            get;
            set;
        }

        [DataMember(Name = "C")]
        public short WeighingComponentState
        {
            get;
            set;
        }

        [DataMember(Name = "D")]
        public Guid? FacilityCharge
        {
            get;
            set;
        }

        [DataMember(Name = "E")]
        public Guid? Facility
        {
            get;
            set;
        }

        [DataMember(Name = "F")]
        public bool FC_FAutoRefresh
        {
            get;
            set;
        }

        [DataMember(Name = "G")]
        public bool IsManualAddition
        {
            get;
            set;
        }

        [DataMember(Name = "H")]
        public bool IsLotChange
        {
            get;
            set;
        }

        public object Clone()
        {
            WeighingComponentInfo compInfo = new WeighingComponentInfo();
            compInfo.WeighingComponentInfoType = this.WeighingComponentInfoType;
            compInfo.PLPosRelation = this.PLPosRelation;
            compInfo.WeighingComponentState = this.WeighingComponentState;
            compInfo.FacilityCharge = this.FacilityCharge;
            compInfo.Facility = this.Facility;
            compInfo.FC_FAutoRefresh = this.FC_FAutoRefresh;
            compInfo.IsManualAddition = this.IsManualAddition;
            compInfo.IsLotChange = this.IsLotChange;
            return compInfo;
        }

    }

    public enum WeighingComponentState : short
    {
        ReadyToWeighing = 0,
        Selected = 5,
        InWeighing = 10,
        WeighingCompleted = 20,
        PartialCompleted = 25,
        Aborted = 30
    }

    public enum WeighingComponentInfoType : short
    {
        State = 0,
        ReturnComp = 10, //Manual component select - automatic facility charge or facility select
        SelectFC_F = 15, 
        StateSelectFC_F = 20, //Manual component select - automatic facility charge or facility select
        StateSelectCompAndFC_F = 30, //Automatic component select - automatic facility charge or facility select
        SelectCompReturnFC_F = 40 //Automatic component select - manual facility charge or facility select
    }

    [ACSerializeableInfo]
    [DataContract(Name = "MWTaskInfo")]
    public enum ManualWeighingTaskInfo
    {
        [EnumMember(Value = "10")]
        None,
        [EnumMember(Value = "20")]
        WaitForStart,
        [EnumMember(Value = "30")]
        WaitForTare,
        [EnumMember(Value = "40")]
        WaitForAcknowledge
    }

    #endregion
}
