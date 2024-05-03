using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Vml.Office;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using static gip.mes.facility.ACPartslistManager;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Class that is responsible for processing input-materials that are associated with an intermediate product. 
    /// The intermediate product, in turn, is linked through the material workflow to one or more workflow nodes that are from this PWManualWeighing class. 
    /// PWManualWeighing is used to support manual production. 
    /// It calls the PAFManualWeighing process function asynchronously.
    /// The operator is guided by the business object BSOManualWeighing, which is a plugin for the business object BSOWorkCenter.
    /// Consumed quantities are posted by warehous management (ACFacilityManager).
    /// It can work with different data contexts (production and picking orders).
    /// </summary>
    /// <seealso cref="gip.core.autocomponent.PWNodeProcessMethod" />
    /// <seealso cref="gip.core.autocomponent.IPWNodeReceiveMaterial" />
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PWManualWeighing'}de{'PWManualWeighing'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public partial class PWManualWeighing : PWNodeProcessMethod, IPWNodeReceiveMaterial
    {
        public const string PWClassName = nameof(PWManualWeighing);

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

            method.ParameterValueList.Add(new ACValue("ScaleOtherComp", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("ScaleOtherComp", "en{'Scale other components after weighing'}de{'Restliche Komponenten anpassen'}");

            method.ParameterValueList.Add(new ACValue("ReservationMode", typeof(short), (short)0, Global.ParamOption.Optional));
            paramTranslation.Add("ReservationMode", "en{'Allow other lots if reservation'}de{'Erlaube andere Lose bei Reservierungen'}");

            method.ParameterValueList.Add(new ACValue("ReworkMaterialNo", typeof(string), "", Global.ParamOption.Optional));
            paramTranslation.Add("ReworkMaterialNo", "en{'Material number for rework'}de{'Materialnummer für Nacharbeit'}");

            method.ParameterValueList.Add(new ACValue("ReworkQuantity", typeof(int), 0, Global.ParamOption.Optional));
            paramTranslation.Add("ReworkQuantity", "en{'Rework quantity [%]'}de{'Nachbearbeitungsmenge [%]'}");

            method.ParameterValueList.Add(new ACValue("CompSequenceNo", typeof(int), (int)0, Global.ParamOption.Optional));
            paramTranslation.Add("CompSequenceNo", "en{'Sequence-No. for adding rework into BOM'}de{'Folgenummer beim Hinzufügen Rework in die Rezeptur'}");

            method.ParameterValueList.Add(new ACValue("RuleForSelMulLots", typeof(LotSelectionRuleEnum), LotSelectionRuleEnum.None, Global.ParamOption.Optional));
            paramTranslation.Add("RuleForSelMulLots", "en{'Rule for lot selection when multiple lots available'}de{'Rule for lot selection when multiple lots available'}");

            method.ParameterValueList.Add(new ACValue("AutoInterDis", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("AutoInterDis", "en{'Auto inter discharging'}de{'Automatische Zwischenentleerung'}");

            method.ParameterValueList.Add(new ACValue("DiffWeighing", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("DiffWeighing", "en{'Difference weighing'}de{'Differenzwägung'}");

            method.ParameterValueList.Add(new ACValue("EachPosSeparated", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("EachPosSeparated", "en{'Weigh each line separated in outer loop'}de{'Position einzeln in äußerer Schleife verwiegen'}");

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
            IntermediateChildPos = null;
            _ZeroBookingFacilityCharge = null;
            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            //_WeighingComponentsInfo = null;
            CurrentEndBatchPosKey = null;
            CurrentOpenMaterial = null;
            CurrentFacilityCharge = null;
            _LastOpenMaterial = null;
            IntermediateChildPos = null;
            using (ACMonitor.Lock(_65050_WeighingCompLock))
            {
                WeighingComponents = null;
            }
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }

        #endregion

        #region Const

        public const string MaterialConfigLastUsedLotKeyACUrl = "ManWeighLastUsedLot";

        #endregion

        #region Properties

        #region Propeties => Private members

        private readonly ACMonitorObject _65001_CanStartFromBSOLock = new ACMonitorObject(65001);

        private readonly ACMonitorObject _65500_LotChangeLock = new ACMonitorObject(65500);

        private readonly ACMonitorObject _65025_MemberCompLock = new ACMonitorObject(65025);
        private readonly ACMonitorObject _65050_WeighingCompLock = new ACMonitorObject(65050);

        //TODO: _IsAborted and _ScaleComp change with PAF paramter AbortType
        protected bool _CanStartFromBSO = true, _IsAborted = false, _IsBinChangeActivated = false, _IsLotChanged = false, _ScaleComp = false;

        private gip.core.datamodel.ACProgramLog _NewAddedProgramLog = null;

        #endregion

        public bool IsProduction
        {
            get
            {
                return ParentPWMethod<PWMethodProduction>() != null;
            }
        }

        public bool IsTransport
        {
            get
            {
                return ParentPWMethod<PWMethodTransportBase>() != null;
            }
        }

        public PWMethodVBBase ParentPWMethodVBBase
        {
            get
            {
                return ParentRootWFNode as PWMethodVBBase;
            }
        }

        //public override ACMethod ExecutingACMethod
        //{
        //    get
        //    {
        //        if (_ExecutingACMethod != null)
        //            return _ExecutingACMethod;
        //        return base.ExecutingACMethod;
        //    }
        //}

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

        [ACPropertyBindingSource(IsPersistable = true)]
        public IACContainerTNet<double> InterdischargingScaleActualWeight
        {
            get;
            set;
        }

        [ACPropertyBindingSource(IsPersistable = true)]
        public IACContainerTNet<string> InterdischargingScaleActualValue
        {
            get;
            set;
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

        public bool ScaleOtherComp
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ScaleOtherComp");
                    if (acValue != null)
                        return acValue.ParamAsBoolean;
                }
                return false;
            }
        }

        public short ReservationMode
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ReservationMode");
                    if (acValue != null)
                    {
                        return acValue.ParamAsInt16;
                    }
                }
                return 0;
            }
        }

        public bool AutoInterDis
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("AutoInterDis");
                    if (acValue != null)
                        return acValue.ParamAsBoolean;
                }
                return false;
            }
        }

        public string ReworkMaterialNo
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ReworkMaterialNo");
                    if (acValue != null)
                        return acValue.ParamAsString;
                }
                return null;
            }
        }

        public int ReworkQuantityPercentage
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ReworkQuantity");
                    if (acValue != null)
                        return acValue.ParamAsInt32;
                }
                return 0;
            }
        }

        internal int CompSequenceNo
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("CompSequenceNo");
                    if (acValue != null)
                    {
                        return acValue.ParamAsInt32;
                    }
                }
                return 0;
            }
        }

        public LotSelectionRuleEnum? MultipleLotsSelectionRule
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("RuleForSelMulLots");
                    if (acValue != null)
                        return acValue.Value as LotSelectionRuleEnum?;
                }
                return null;
            }

        }

        public bool DiffWeighing
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("DiffWeighing");
                    if (acValue != null)
                        return acValue.ParamAsBoolean;
                }
                return false;
            }
        }

        public bool EachPosSeparated
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("EachPosSeparated");
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

        protected ProdOrderPartslistPos IntermediateChildPos
        {
            get;
            set;
        }

        [ACPropertyInfo(999)]
        public ACValue WeighingComponentsInfo
        {
            get
            {
                Dictionary<string, string> weighingComponentsInfo = null;
                if (IsProduction)
                {
                    using (ACMonitor.Lock(_65050_WeighingCompLock))
                    {
                        if (WeighingComponents != null)
                            weighingComponentsInfo = WeighingComponents.ToDictionary(c => c.PLPosRelation.ProdOrderPartslistPosRelationID.ToString(), c => ((short)c.WeighState).ToString());
                    }

                    ProdOrderPartslistPos intermediateChildPos = null;
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        intermediateChildPos = IntermediateChildPos;
                    }

                    if (intermediateChildPos != null && weighingComponentsInfo != null)
                    {
                        weighingComponentsInfo.Add(intermediateChildPos.ProdOrderPartslistPosID.ToString(), null);
                    }
                }
                else if (IsTransport)
                {
                    using (ACMonitor.Lock(_65050_WeighingCompLock))
                    {
                        if (WeighingComponents != null)
                            weighingComponentsInfo = WeighingComponents.ToDictionary(c => c.PickingPosition.PickingPosID.ToString(), c => ((short)c.WeighState).ToString());
                    }
                }

                return new ACValue("WM", weighingComponentsInfo);
            }
        }

        public List<WeighingComponent> WeighingComponents
        {
            get;
            set;
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
        public IACContainerTNet<ManualWeighingTaskInfo> ManualWeighingNextTask
        {
            get;
            set;
        }

        [ACPropertyBindingSource]
        public IACContainerTNet<bool> IsBinChangeLoopNodeAvailable
        {
            get;
            set;
        }

        public bool HasAnyMaterialToProcessProd
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
                        ProdOrderPartslistPos intermediateChildPos;
                        MaterialWFConnection matWFConnection;
                        ProdOrderBatch batch;
                        ProdOrderBatchPlan batchPlan;
                        ProdOrderPartslistPos intermediatePos;
                        ProdOrderPartslistPos endBatchPos;
                        MaterialWFConnection[] matWFConnections;
                        bool posFound = PWDosing.GetRelatedProdOrderPosForWFNode(this, dbIPlus, dbApp, pwMethodProduction, out intermediateChildPos, out intermediatePos,
                            out endBatchPos, out matWFConnection, out batch, out batchPlan, out matWFConnections);
                        if (!posFound)
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

        public bool HasAnyMaterialToProcess
        {
            get
            {
                if (IsProduction)
                    return HasAnyMaterialToProcessProd;
                else if (IsTransport)
                {
                    PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
                    if (pwMethodTransport != null && pwMethodTransport.CurrentPicking != null)
                        return HasAnyMaterialToProcessPicking;
                }


                return true;
            }
        }

        public bool HasRunSomeDosings
        {
            get
            {
                var previousLogs = PreviousProgramLogs;
                return previousLogs != null && previousLogs.Any();
            }
        }

        public int CountRunDosings
        {
            get
            {
                var previousLogs = PreviousProgramLogs;
                return previousLogs != null ? previousLogs.Count() : 0;
            }
        }

        public virtual ScaleBoundaries OnGetScaleBoundariesForDosing(IPAMContScale scale, DatabaseApp dbApp, ProdOrderPartslistPosRelation[] queryOpenDosings)
        {
            return new ScaleBoundaries(scale);
        }

        public virtual bool HasDosedComponents(out double sumQuantity)
        {
            using (var dbApp = new DatabaseApp())
            {
                return ManageDosingState(PWDosing.ManageDosingStatesMode.QueryDosedComponents, dbApp, out sumQuantity);
            }
        }

        public virtual bool HasOpenDosings(out double sumQuantity)
        {
            using (var dbApp = new DatabaseApp())
            {
                return ManageDosingState(PWDosing.ManageDosingStatesMode.QueryOpenDosings, dbApp, out sumQuantity);
            }
        }

        public virtual bool HasAnyDosings(out double sumQuantity)
        {
            using (var dbApp = new DatabaseApp())
            {
                return ManageDosingState(PWDosing.ManageDosingStatesMode.QueryHasAnyDosings, dbApp, out sumQuantity);
            }
        }

        public virtual bool ResetDosingsAfterInterDischarging(IACEntityObjectContext dbApp)
        {
            double sumQuantity;
            return ManageDosingState(PWDosing.ManageDosingStatesMode.ResetDosings, dbApp as DatabaseApp, out sumQuantity);
        }

        public virtual bool SetDosingsCompletedAfterDischarging(IACEntityObjectContext dbApp)
        {
            double sumQuantity;
            return ManageDosingState(PWDosing.ManageDosingStatesMode.SetDosingsCompleted, dbApp as DatabaseApp, out sumQuantity);
        }

        public virtual void OnDosingLoopDecision(IACComponentPWNode dosingloop, bool willRepeatDosing)
        {
        }

        private bool ManageDosingState(PWDosing.ManageDosingStatesMode mode, DatabaseApp dbApp, out double sumQuantity)
        {
            sumQuantity = 0.0;
            if (IsProduction)
                return ManageDosingStateProd(mode, dbApp, out sumQuantity);
            //else if (IsTransport)
            //    return ManageDosingStatePicking(mode, dbApp, out sumQuantity);
            return false;
        }

        private bool ManageDosingStateProd(PWDosing.ManageDosingStatesMode mode, DatabaseApp dbApp, out double sumQuantity)
        {
            sumQuantity = 0.0;
            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            if (pwMethodProduction == null)
                return false;
            ProdOrderPartslistPos endBatchPos = pwMethodProduction.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
            if (pwMethodProduction.CurrentProdOrderBatch == null)
                return false;

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


            //MaterialWFConnection matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
            //    .Where(c => c.MaterialWFACClassMethod.MaterialWFID == endBatchPos.ProdOrderPartslist.Partslist.MaterialWFID)
            //    .FirstOrDefault();
            if (matWFConnection == null)
                return false;

            // Find intermediate position which is assigned to this Dosing-Workflownode
            var currentProdOrderPartslist = endBatchPos.ProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
            ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                    && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                    && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
            if (intermediatePosition == null)
                return false;

            ProdOrderPartslistPos intermediateChildPos = null;
            // Lock, if a parallel Dosing also creates a child Position for this intermediate Position

            using (ACMonitor.Lock(pwMethodProduction._62000_PWGroupLockObj))
            {
                // Find intermediate child position, which is assigned to this Batch
                intermediateChildPos = intermediatePosition.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                    .Where(c => c.ProdOrderBatchID.HasValue
                                && c.ProdOrderBatchID.Value == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID)
                    .FirstOrDefault();

                if (intermediateChildPos == null)
                    return false;
            }
            if (intermediateChildPos == null)
                return false;

            // Falls noch Dosierungen anstehen, dann dosiere nächste Komponente
            if (mode == PWDosing.ManageDosingStatesMode.ResetDosings || mode == PWDosing.ManageDosingStatesMode.SetDosingsCompleted)
            {
                if (ParentPWGroup == null || ParentPWGroup.AccessedProcessModule == null)
                    return false;
                string acUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();

                bool changed = false;
                var posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted).FirstOrDefault();
                if (posState != null)
                {
                    var queryDosings = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray();
                    foreach (var childPos in queryDosings)
                    {
                        // Suche alle Dosierungen die auf DIESER Waage stattgefunden haben
                        var unconfirmedBookings = childPos.FacilityBooking_ProdOrderPartslistPosRelation.Where(c => c.PropertyACUrl == acUrl && c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New);
                        if (unconfirmedBookings.Any())
                        {
                            changed = true;
                            // Falls alle Komponenten entleert, setze Status auf Succeeded
                            foreach (var booking in unconfirmedBookings)
                            {
                                if (mode == PWDosing.ManageDosingStatesMode.SetDosingsCompleted)
                                    booking.MaterialProcessState = GlobalApp.MaterialProcessState.Processed;
                                else // (mode == ManageDosingStatesMode.ResetDosings)
                                    booking.MaterialProcessState = GlobalApp.MaterialProcessState.Discarded;
                                sumQuantity += booking.OutwardQuantity;
                            }
                            // Sonderentleerung, setze Status auf Teilerledigt
                            if (mode == PWDosing.ManageDosingStatesMode.ResetDosings)
                            {
                                childPos.MDProdOrderPartslistPosState = posState;
                            }
                        }
                    }
                }
                return changed;
            }
            else
            {
                if (mode == PWDosing.ManageDosingStatesMode.QueryOpenDosings)
                {
                    var queryOpenDosings = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray()
                                        .Where(c => c.RemainingDosingWeight < -1.0 // TODO: Unterdosierung ist Min-Dosiermenge auf Waage
                                                    && c.MDProdOrderPartslistPosState != null
                                                    && (c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created
                                                        || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted))
                                        .OrderBy(c => c.Sequence);
                    bool any = queryOpenDosings.Any();
                    if (any)
                        sumQuantity = queryOpenDosings.Sum(c => c.RemainingDosingQuantityUOM);
                    return any;
                }
                else if (mode == PWDosing.ManageDosingStatesMode.QueryHasAnyDosings)
                {
                    bool any = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Any();
                    if (any)
                        sumQuantity = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Sum(c => c.TargetQuantityUOM);
                    return any;
                }
                else //if (mode == ManageDosingStatesMode.QueryDosedComponents)
                {
                    if (ParentPWGroup == null)
                        return false;

                    PAProcessModule apm = ParentPWGroup.AccessedProcessModule != null ? ParentPWGroup.AccessedProcessModule : ParentPWGroup.PreviousAccessedPM;
                    if (apm == null)
                    {
                        apm = HandleNotFoundPMOnManageDosingStateProd(mode, dbApp);
                        if (apm == null)
                            return false;
                    }
                    string acUrl = apm.GetACUrl();
                    var queryDosings = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray();
                    bool hasDosings = false;
                    foreach (var childPos in queryDosings)
                    {
                        var bookings = childPos.FacilityBooking_ProdOrderPartslistPosRelation.Where(c => c.PropertyACUrl == acUrl
                                                        && (c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New
                                                           || c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.Processed));
                        if (bookings.Any())
                        {
                            sumQuantity += bookings.Sum(c => c.OutwardQuantity);
                            hasDosings = true;
                        }
                    }
                    return hasDosings;
                }
            }
        }

        protected virtual PAProcessModule HandleNotFoundPMOnManageDosingStateProd(PWDosing.ManageDosingStatesMode mode, DatabaseApp dbApp)
        {
            return null;
        }

        #endregion

        #region Methods

        #region Methods => ACState

        public override void SMIdle()
        {
            CurrentOpenMaterial = null;
            IntermediateChildPos = null;
            CurrentFacilityCharge = null;
            CurrentACMethod.ValueT = null;
            ClearMyConfiguration();

            _IsBinChangeActivated = false;
            _IsAborted = false;

            _IsLotChanged = false;

            base.SMIdle();
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            var pwGroup = ParentPWGroup;
            if (!CheckParentGroupAndHandleSkipMode())
                return;

            if (!Root.Initialized)
            {
                SubscribeToProjectWorkCycle();
                return;
            }

            CheckIsBinChangeLoopNodeAvailable();

            core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
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
                if (manualWeighing != null)
                {
                    if (CurrentACState == ACStateEnum.SMStarting)
                        CurrentACState = ACStateEnum.SMRunning;
                    return;
                }

                StartNextCompResult result = StartNextCompResult.Done;
                if (IsProduction)
                {
                    result = StartManualWeighingProd(module);
                }
                else if (IsTransport)
                {
                    result = StartManualWeighingPicking(module);
                }

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

            if (IsProduction)
                SMRunning_Prod();

            else if (IsTransport)
                SMRunning_Picking();
        }

        private void SMRunning_Prod()
        {
            try
            {
                CheckIsBinChangeLoopNodeAvailable();

                PAFManualWeighing manualWeighing = CurrentExecutingFunction<PAFManualWeighing>();

                bool isWeighComponentsNull;
                using (ACMonitor.Lock(_65050_WeighingCompLock))
                {
                    isWeighComponentsNull = WeighingComponents == null;
                }

                if (isWeighComponentsNull
                    || (AutoInterDis && manualWeighing == null))
                {
                    if (ParentPWGroup.AccessedProcessModule == null)
                    {
                        SubscribeToProjectWorkCycle();
                        Messages.LogInfo(this.GetACUrl(), nameof(SMRunning_Prod), "AccessedProcessModule == null");
                        return;
                    }

                    StartNextCompResult result = StartNextCompResult.Done;
                    if (IsProduction)
                    {
                        if (!ParentPWGroup.IsInSkippingMode)
                            result = StartManualWeighingProd(ParentPWGroup.AccessedProcessModule);
                    }

                    if (result == StartNextCompResult.CycleWait)
                    {
                        SubscribeToProjectWorkCycle();
                        return;
                    }
                    else if (result == StartNextCompResult.Done)
                    {
                        UnSubscribeToProjectWorkCycle();
                        // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                        if (CurrentACState == ACStateEnum.SMRunning)
                            CurrentACState = ACStateEnum.SMCompleted;
                        return;
                    }

                    //Check if manual weighing currently active
                    if (manualWeighing != null)
                    {
                        ACValue plPosRelation = manualWeighing.CurrentACMethod.ValueT.ParameterValueList.GetACValue("PLPosRelation");
                        Guid? currentOpenMat = null;
                        if (plPosRelation != null && plPosRelation.Value != null)
                            currentOpenMat = plPosRelation.ParamAsGuid;

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
                        if (facilityCharge != null && facilityCharge.Value != null)
                        {
                            CurrentFacilityCharge = facilityCharge.ParamAsGuid;
                            comp.SwitchState(WeighingComponentState.InWeighing);
                            SetInfo(comp, WeighingComponentInfoType.StateSelectCompAndFC_F, facilityCharge.ParamAsGuid);
                        }
                        else
                        {
                            //_ExitFromWaitForFC = false;
                            SetInfo(comp, WeighingComponentInfoType.SelectCompReturnFC_F, null);
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
                    if (WeighingComponents != null)
                    {
                        isAnyNeedToWeigh = WeighingComponents.Any(c => c.WeighState < WeighingComponentState.InWeighing);
                    }
                    else
                    {
                        Messages.LogMessage(eMsgLevel.Info, this.GetACUrl(), nameof(SMRunning), "The property WeighingComponents is null(10)");
                    }
                }

                Guid? currentOpenMaterial = CurrentOpenMaterial;

                if (!FreeSelectionMode)
                {
                    if (currentOpenMaterial == null)
                    {
                        WeighingComponent nextComp = null;
                        using (ACMonitor.Lock(_65050_WeighingCompLock))
                        {
                            nextComp = WeighingComponents.OrderBy(c => c.Sequence).FirstOrDefault(c => c.WeighState == WeighingComponentState.ReadyToWeighing);
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

                        CurrentOpenMaterial = nextComp.PLPosRelation.ProdOrderPartslistPosRelationID;
                        bool hasQuants = TryAutoSelectFacilityCharge(nextComp.PLPosRelation.ProdOrderPartslistPosRelationID);

                        StartNextCompResult funcStartResult = StartManualWeighingNextComp(ParentPWGroup.AccessedProcessModule, nextComp, hasQuants); //Auto Comp && Auto Lot
                        if (funcStartResult == StartNextCompResult.CycleWait)
                        {
                            CurrentOpenMaterial = null;
                            SubscribeToProjectWorkCycle();
                            return;
                        }

                        Guid? currentFacilityCharge = CurrentFacilityCharge;

                        if (!currentFacilityCharge.HasValue)
                        {
                            //_ExitFromWaitForFC = false;
                            SetInfo(nextComp, WeighingComponentInfoType.SelectCompReturnFC_F, currentFacilityCharge);
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

                    if (currentFacilityCharge != null)
                    {
                        StartManualWeighingNextComp(ParentPWGroup.AccessedProcessModule, nextComp, null); //Man Comp && Auto Lot || ManLot
                        UnSubscribeToProjectWorkCycle();
                    }
                    else if (AutoSelectLot && currentOpenMaterial != null)
                    {
                        bool hasQuants = TryAutoSelectFacilityCharge(currentOpenMaterial);
                        if (currentFacilityCharge != null)
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
                    using (ACMonitor.Lock(_65050_WeighingCompLock))
                    {
                        isAnyReadyToWeigh = WeighingComponents.Any(c => c.WeighState == WeighingComponentState.ReadyToWeighing);
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

            using (ACMonitor.Lock(_65025_MemberCompLock))
            {
                InterdischargingScaleActualValue.ValueT = null;
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
            //_WeighingComponentsInfo = null;
            using (ACMonitor.Lock(_65050_WeighingCompLock))
            {
                WeighingComponents = null;
            }
            ClearMyConfiguration();
            SetCanStartFromBSO(true);

            _IsBinChangeActivated = false;
            IsBinChangeLoopNodeAvailable.ValueT = true;

            using (ACMonitor.Lock(_65025_MemberCompLock))
            {
                InterdischargingScaleActualValue.ValueT = null;
            }

            UnSubscribeToProjectWorkCycle();

            CurrentACState = ACStateEnum.SMResetting;
            CurrentWeighingComponentInfo.ValueT = null;

            base.Reset();
        }

        #endregion        

        #region Methods => Commands

        [ACMethodInfo("", "", 9999)]
        public Msg StartWeighing(Guid? prodOrderPartslistPosRelation, Guid? facilityCharge, Guid? facility, bool forceSetFC_F)
        {
            if (!DiffWeighing)
            {
                using (ACMonitor.Lock(_65001_CanStartFromBSOLock))
                {
                    if (!_CanStartFromBSO)
                        return null;
                    _CanStartFromBSO = false;
                }
            }

            if (prodOrderPartslistPosRelation == null)
                return null;

            bool isProd = IsProduction;

            if (DiffWeighing && isProd)
            {
                Guid? currentOpenMaterial = CurrentOpenMaterial;
                if (currentOpenMaterial.HasValue)
                {
                    WeighingComponent currentComp = GetWeighingComponent(currentOpenMaterial);
                    if (currentComp != null && currentComp.WeighState == WeighingComponentState.InWeighing)
                    {
                        CurrentFacilityCharge = null;
                        currentComp.SwitchState(WeighingComponentState.ReadyToWeighing);
                        SetInfo(currentComp, WeighingComponentInfoType.State, null);
                    }
                }
            }

            WeighingComponent comp = null;

            if (isProd)
                comp = GetWeighingComponent(prodOrderPartslistPosRelation);
            else
                comp = GetWeighingComponentPicking(prodOrderPartslistPosRelation);

            if (comp == null || comp.WeighState == WeighingComponentState.WeighingCompleted)
                return null;

            if (FreeSelectionMode && AutoSelectLot && !facilityCharge.HasValue)
            {
                if (isProd)
                    TryAutoSelectFacilityCharge(prodOrderPartslistPosRelation);
                else
                    TryAutoSelectFacilityChargePicking(prodOrderPartslistPosRelation);

                facilityCharge = CurrentFacilityCharge;
            }

            if (facilityCharge.HasValue)
            {
                if (FreeSelectionMode)
                {
                    CurrentOpenMaterial = prodOrderPartslistPosRelation;
                }

                Msg msg = SetFacilityCharge(facilityCharge, prodOrderPartslistPosRelation, forceSetFC_F);

                if (msg != null)
                {
                    SetCanStartFromBSO(true);
                    return msg;
                }

                if (!FreeSelectionMode)
                {
                    Guid? currentOpenMaterial = CurrentOpenMaterial;
                    Guid? currentFacilityCharge = CurrentFacilityCharge;

                    if (ApplicationManager != null && ApplicationManager.ApplicationQueue != null && !ApplicationManager.ApplicationQueue.IsBusy)
                        ApplicationManager.ApplicationQueue.Add(() => SelectFacilityChargeOrFacility(currentOpenMaterial, currentFacilityCharge, WeighingComponentInfoType.State));
                    else
                        SelectFacilityChargeOrFacility(currentOpenMaterial, currentFacilityCharge, WeighingComponentInfoType.State);
                }
                else
                {
                    PAFManualWeighing manWeighing = CurrentExecutingFunction<PAFManualWeighing>();

                    if (manWeighing != null)
                    {
                        ACMethod acMethod = manWeighing.CurrentACMethod.ValueT;
                        if (acMethod != null)
                        {
                            UpdatePAFACMethod(comp, acMethod);
                            if (manWeighing.CurrentACState == ACStateEnum.SMRunning)
                            {
                                if (comp.WeighState < WeighingComponentState.InWeighing)
                                {
                                    comp.SwitchState(WeighingComponentState.InWeighing);

                                    if (isProd)
                                        SetRelationState(comp.PLPosRelation.ProdOrderPartslistPosRelationID, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.InProcess);
                                    else
                                        SetRelationStatePicking(comp.PickingPosition.PickingPosID, MDDelivPosLoadState.DelivPosLoadStates.LoadingActive);

                                    WeighingComponentInfoType infoType = WeighingComponentInfoType.StateSelectCompAndFC_F;
                                    Guid? currentFacilityCharge = CurrentFacilityCharge;
                                    SetInfo(comp, infoType, currentFacilityCharge.Value);
                                }
                            }
                            manWeighing.SubscribeToProjectWorkCycle();
                        }
                        else
                        {
                            Messages.LogMessage(eMsgLevel.Error, this.GetACUrl(), nameof(StartWeighing), "The ACMethod from function is null!");
                        }
                    }
                    else
                    {
                        SubscribeToProjectWorkCycle();
                    }
                }
            }
            else
            {
                SetCanStartFromBSO(true);
            }

            CurrentOpenMaterial = prodOrderPartslistPosRelation;
            return null;
        }

        [ACMethodInfo("", "", 9999)]
        public virtual void CompleteWeighing(double actualWeight, bool bookAndWeighRest)
        {
            PAFManualWeighing manualWeighing = CurrentExecutingFunction<PAFManualWeighing>();

            if (manualWeighing != null)
            {
                if (!DiffWeighing)
                {
                    if (bookAndWeighRest)
                    {
                        ACMethod currentACMethod = CurrentACMethod.ValueT;
                        var targetQ = currentACMethod.ParameterValueList.GetACValue("TargetQuantity");
                        double targetWeight = 0;
                        if (targetQ != null)
                            targetWeight = targetQ.ParamAsDouble;

                        if (actualWeight > 0.000001 && actualWeight < targetWeight)
                        {
                            Guid? currentFacilityCharge = CurrentFacilityCharge;
                            bool isForInterdischarge = true;
                            if (_IsLotChanged)
                            {
                                isForInterdischarge = false;
                                _IsLotChanged = false;
                            }

                            Guid? currentOpenMaterial = CurrentOpenMaterial;

                            //Guid? correctedFc = IsCurrentFacilityChargeCorrect(currentFacilityCharge, currentOpenMaterial, currentACMethod);
                            //if (correctedFc.HasValue)
                            //    currentFacilityCharge = correctedFc;

                            var msgBooking = DoManualWeighingBooking(actualWeight, false, false, currentOpenMaterial, currentFacilityCharge, isForInterdischarge);
                            if (msgBooking != null)
                            {
                                Messages.LogError(this.GetACUrl(), msgBooking.ACIdentifier, msgBooking.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, msgBooking, false);
                                ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;
                                return;
                            }

                            double rest = targetWeight - actualWeight;
                            manualWeighing.CurrentACMethod.ValueT.ParameterValueList["TargetQuantity"] = rest;
                            manualWeighing.ReSendACMethod(manualWeighing.CurrentACMethod.ValueT);

                            WeighingComponent comp = GetWeighingComponent(currentOpenMaterial);
                            if (comp == null)
                            {
                                return;
                            }

                            comp.SwitchState(WeighingComponentState.PartialCompleted);
                            SetInfo(comp, WeighingComponentInfoType.State, null);
                            comp.SwitchState(WeighingComponentState.InWeighing);
                            SetInfo(comp, WeighingComponentInfoType.StateSelectCompAndFC_F, currentFacilityCharge);
                        }
                    }
                    else
                    {
                        manualWeighing.CompleteWeighing(actualWeight);
                    }
                }
                else
                {
                    manualWeighing.CompleteOnDifferenceWeighing(true);
                    CurrentACState = ACStateEnum.SMCompleted;
                }
            }
        }

        [ACMethodInfo("", "", 9999)]
        public void TareScale()
        {
            PAFManualWeighing manualWeighing = CurrentExecutingFunction<PAFManualWeighing>();
            if (manualWeighing != null)
                manualWeighing.ActiveScaleObject?.Tare();
        }

        protected internal Msg SelectFCFromPAF(Guid? newFacilityCharge, double actualQuantity, LotUsedUpEnum? isConsumed, bool forceSetFC_F)
        {
            if (DiffWeighing)
            {
                using (Database db = new core.datamodel.Database())
                using (DatabaseApp dbApp = new DatabaseApp(db))
                {
                    FacilityCharge newFC = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == newFacilityCharge.Value);

                    if (newFC != null)
                    {
                        Guid materialID = newFC.MaterialID;
                        WeighingComponent comp = GetWeighingComponentByMaterial(materialID);

                        if (comp != null)
                        {
                            ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation
                                                                     .Include(c => c.SourceProdOrderPartslistPos)
                                                                     .Include(c => c.FacilityPreBooking_ProdOrderPartslistPosRelation)
                                                                     .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == comp.PLPosRelation.ProdOrderPartslistPosRelationID);

                            var facilityCharges = GetFacilityChargesForMaterial(dbApp, rel);
                            var fc = facilityCharges?.FirstOrDefault(c => c.FacilityChargeID == newFacilityCharge.Value);
                            if (fc == null)
                            {
                                return new Msg(this, eMsgLevel.Error, PWClassName, nameof(SelectFCFromPAF), 1602, "Error50266", rel.SourceProdOrderPartslistPos.Material.MaterialNo,
                                                                                                                   rel.SourceProdOrderPartslistPos.Material.MaterialName1);
                            }


                            if (rel.ProdOrderPartslistPosRelationID != CurrentOpenMaterial)
                            {
                                Guid? currentOpenMaterial = CurrentOpenMaterial;
                                if (currentOpenMaterial.HasValue)
                                {
                                    WeighingComponent currentComp = GetWeighingComponent(currentOpenMaterial);
                                    if (currentComp != null && currentComp.WeighState == WeighingComponentState.InWeighing)
                                    {
                                        currentComp.SwitchState(WeighingComponentState.ReadyToWeighing);
                                        SetInfo(currentComp, WeighingComponentInfoType.State, null);
                                    }
                                }
                            }

                            if (rel.FacilityPreBooking_ProdOrderPartslistPosRelation.Any())
                            {
                                //Error : Missing the weighing at the end. Please perform a weigh at the end, then you can change lot!

                                return new Msg(eMsgLevel.Error, "Nedostaje vaganje na kraju!");
                            }

                            Msg msg = SetFacilityCharge(newFacilityCharge.Value, rel.ProdOrderPartslistPosRelationID, forceSetFC_F);
                            if (msg != null)
                                return msg;

                            var currentFacilityCharge = CurrentFacilityCharge;
                            SelectFacilityChargeOrFacility(rel.ProdOrderPartslistPosRelationID, currentFacilityCharge, WeighingComponentInfoType.State);

                            PAFManualWeighing manWeighing = CurrentExecutingFunction<PAFManualWeighing>();

                            if (manWeighing != null)
                            {
                                ACMethod acMethod = manWeighing.CurrentACMethod.ValueT;
                                if (acMethod != null)
                                {
                                    UpdatePAFACMethod(comp, acMethod);
                                    if (manWeighing.CurrentACState == ACStateEnum.SMRunning)
                                    {
                                        if (comp.WeighState < WeighingComponentState.InWeighing)
                                        {
                                            comp.SwitchState(WeighingComponentState.InWeighing);

                                            SetRelationState(comp.PLPosRelation.ProdOrderPartslistPosRelationID, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.InProcess);

                                            WeighingComponentInfoType infoType = WeighingComponentInfoType.StateSelectCompAndFC_F;
                                            //Guid? currentFacilityCharge = CurrentFacilityCharge;
                                            SetInfo(comp, infoType, currentFacilityCharge);
                                        }
                                    }
                                    manWeighing.SubscribeToProjectWorkCycle();
                                }
                                else
                                {
                                    Messages.LogMessage(eMsgLevel.Error, this.GetACUrl(), nameof(StartWeighing), "The ACMethod from function is null!");
                                }
                            }
                            else
                            {
                                SubscribeToProjectWorkCycle();
                            }

                            CurrentOpenMaterial = rel.ProdOrderPartslistPosRelationID;
                        }
                    }
                }
            }
            else
            {
                Guid? currentOpenMaterial = CurrentOpenMaterial;

                if (!currentOpenMaterial.HasValue)
                {
                    // Error50373: Manual weighing error, the property {0} is null!
                    // Fehler bei Handverwiegung, die Eigenschaft {0} ist null!
                    return new Msg(this, eMsgLevel.Error, PWClassName, "SelectFC_FFromPAF(10)", 910, "Error50373", "CurrentOpenMaterial");
                }

                Guid? currentFacilityCharge = CurrentFacilityCharge;

                using (Database db = new core.datamodel.Database())
                using (DatabaseApp dbApp = new DatabaseApp(db))
                {
                    ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation.Include(c => c.SourceProdOrderPartslistPos)
                                                                           .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == currentOpenMaterial.Value);
                    if (rel == null)
                    {
                        //Error50374: ProdOrderPartslistPosRelation {0} doesn't exist in the database.
                        return new Msg(this, eMsgLevel.Error, PWClassName, "SelectFC_FFromPAF(20)", 921, "Error50374", currentOpenMaterial.Value);
                    }

                    var facilityCharges = GetFacilityChargesForMaterial(dbApp, rel);
                    var fc = facilityCharges?.FirstOrDefault(c => c.FacilityChargeID == newFacilityCharge.Value);

                    if (fc == null)
                    {
                        return new Msg(this, eMsgLevel.Error, PWClassName, nameof(SelectFCFromPAF), 935, "Error50266", rel.SourceProdOrderPartslistPos.Material.MaterialNo,
                                                                                                                   rel.SourceProdOrderPartslistPos.Material.MaterialName1);
                    }

                    if (!currentFacilityCharge.HasValue)
                    {
                        if (fc.MaterialID != rel.SourceProdOrderPartslistPos?.MaterialID)
                        {
                            Messages.LogError(this.GetACUrl(), "Wrong quant(10)", "The quant ID: " + fc.FacilityChargeID + ", material ID: " +
                                                               rel.SourceProdOrderPartslistPos?.MaterialID);
                        }

                        Msg msg = SetFacilityCharge(newFacilityCharge.Value, currentOpenMaterial, forceSetFC_F);
                        if (msg != null)
                            return msg;

                        currentFacilityCharge = CurrentFacilityCharge;
                        SelectFacilityChargeOrFacility(currentOpenMaterial, currentFacilityCharge, WeighingComponentInfoType.State);
                    }
                    else if (currentFacilityCharge.HasValue)
                    {
                        bool isLotConsumed = false;

                        if (isConsumed == null)
                        {
                            FacilityCharge currentFC = dbApp.FacilityCharge.Include(c => c.FacilityLot).FirstOrDefault(c => c.FacilityChargeID == currentFacilityCharge.Value);
                            if (currentFC != null)
                            {
                                double? zeroBookTolerance = currentFC.Material?.ZeroBookingTolerance;
                                if (zeroBookTolerance.HasValue && Math.Abs(zeroBookTolerance.Value) > double.Epsilon)
                                {
                                    double stockAfterBooking = currentFC.StockQuantityUOM - actualQuantity;
                                    if (zeroBookTolerance > stockAfterBooking)
                                    {
                                        isLotConsumed = true;
                                    }
                                }

                                if (!isLotConsumed)
                                {
                                    //Question50089: Was the material with the lot number {0} used up?
                                    return new Msg(this, eMsgLevel.Question, nameof(PWManualWeighing), nameof(SelectFCFromPAF), 1617, "Question50089", eMsgButton.YesNo,
                                                   currentFC.FacilityLot.LotNo);
                                }
                            }
                        }
                        else if (isConsumed == LotUsedUpEnum.Yes)
                        {
                            //Question50090: Are you sure the batch is used up?
                            return new Msg(this, eMsgLevel.Question, nameof(PWManualWeighing), nameof(SelectFCFromPAF), 1617, "Question50090", eMsgButton.YesNo);
                        }
                        else
                        {
                            isLotConsumed = isConsumed == LotUsedUpEnum.Yes || isConsumed == LotUsedUpEnum.YesVerified ? true : false;
                        }

                        return LotChange(newFacilityCharge, actualQuantity, isLotConsumed, forceSetFC_F);
                    }
                }
            }
            return null;
        }

        [ACMethodInfo("", "", 9999)]
        public virtual Msg LotChange(Guid? newFacilityCharge, double actualWeight, bool isConsumed, bool forceSetFC_F)
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

            ACMethod currentACMethod = CurrentACMethod.ValueT.Clone() as ACMethod;

            using (ACMonitor.Lock(_65500_LotChangeLock))
            {
                if (currentFacilityCharge == newFacilityCharge)
                    return null;

                facilityCharge = currentFacilityCharge;

                msgSet = SetFacilityCharge(newFacilityCharge, currentOpenMaterial, forceSetFC_F, true);
            }

            //Guid? correctedFc = IsCurrentFacilityChargeCorrect(facilityCharge, currentOpenMaterial, currentACMethod);
            //if (correctedFc.HasValue)
            //    facilityCharge = correctedFc;

            if (actualWeight > 0.000001)
            {
                var msgBooking = DoManualWeighingBooking(actualWeight, false, isConsumed, currentOpenMaterial, facilityCharge);
                if (msgBooking != null)
                {
                    Messages.LogError(this.GetACUrl(), msgBooking.ACIdentifier, msgBooking.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msgBooking, false);
                    ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;
                    return msgBooking;
                }
            }

            if (facilityCharge.HasValue && isConsumed)
            {
                DoFacilityChargeZeroBooking(facilityCharge.Value);
            }

            if (msgSet == null)
            {
                SaveLastUsedLot(newFacilityCharge, currentOpenMaterial);
                WeighingComponent comp = GetWeighingComponent(currentOpenMaterial); //WeighingComponents.FirstOrDefault(c => c.PLPosRelation == CurrentOpenMaterial);
                SetInfo(comp, WeighingComponentInfoType.SelectFC_F, newFacilityCharge, true, true);
                _IsLotChanged = true;
            }

            return msgSet;
        }

        [ACMethodInfo("", "", 9999)]
        public virtual void BinChange()
        {

            if (_IsBinChangeActivated)
                return;

            _IsBinChangeActivated = true;

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

        [ACMethodInfo("", "", 9999)]
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

                    SelectFCFromPAF(fc.FacilityChargeID, 0, LotUsedUpEnum.None, false);

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

        [ACMethodInfo("", "", 9999)]
        public virtual void Abort(bool isConsumed, bool scaleComp = false)
        {
            _ScaleComp = scaleComp;
            PAFManualWeighing paf = CurrentExecutingFunction<PAFManualWeighing>();

            if (_IsAborted)
                return;
            _IsAborted = true;

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
                    ProdOrderPartslistPos intermediateChildPos;
                    ProdOrderPartslistPos intermediatePosition;
                    MaterialWFConnection matWFConnection;
                    ProdOrderBatch batch;
                    ProdOrderBatchPlan batchPlan;
                    ProdOrderPartslistPos endBatchPos;
                    MaterialWFConnection[] matWFConnections;
                    bool posFound = PWDosing.GetRelatedProdOrderPosForWFNode(this, dbIPlus, dbApp, pwMethodProduction,
                        out intermediateChildPos, out intermediatePosition, out endBatchPos, out matWFConnection, out batch, out batchPlan, out matWFConnections);
                    if (batch == null)
                    {
                        // Error50276: No batch assigned to last intermediate material of this workflow
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(30)", 1010, "Error50276");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return StartNextCompResult.CycleWait;
                    }
                    else if (matWFConnection == null)
                    {
                        // Error50277: No relation defined between Workflownode and intermediate material in Materialworkflow
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(40)", 761, "Error50277");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return StartNextCompResult.CycleWait;
                    }
                    else if (intermediatePosition == null)
                    {
                        // Error50278: Intermediate product line not found which is assigned to this workflownode.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(50)", 778, "Error50278");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return StartNextCompResult.CycleWait;
                    }

                    using (ACMonitor.Lock(pwMethodProduction._62000_PWGroupLockObj))
                    {
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

                    using (ACMonitor.Lock(_20015_LockValue))
                        IntermediateChildPos = intermediateChildPos;

                    if (intermediateChildPos == null)
                    {
                        //Error50279:intermediateChildPos is null.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(70)", 1238, "Error50279");
                        ActivateProcessAlarmWithLog(msg, false);
                        Messages.LogMessageMsg(msg);
                        return StartNextCompResult.CycleWait;
                    }

                    ProdOrderPartslistPosRelation[] queryOpenMaterials = null;

                    try
                    {
                        queryOpenMaterials = OnGetAllMaterials(dbIPlus, dbApp, intermediateChildPos);
                        if ((ComponentsSeqFrom > 0 || ComponentsSeqTo > 0) && queryOpenMaterials != null && queryOpenMaterials.Any())
                            queryOpenMaterials = queryOpenMaterials.Where(c => c.Sequence >= ComponentsSeqFrom && c.Sequence <= ComponentsSeqTo)
                                                                .OrderBy(c => c.Sequence)
                                                                .ToArray();
                    }
                    catch (Exception e)
                    {
                        Messages.LogException(this.GetACUrl(), nameof(StartManualWeighingProd)+"(80)", e);
                        return StartNextCompResult.CycleWait;
                    }

                    if (queryOpenMaterials == null || !queryOpenMaterials.Any())
                    {
                        return StartNextCompResult.Done;
                    }

                    using (ACMonitor.Lock(_65050_WeighingCompLock))
                    {
                        WeighingComponents = queryOpenMaterials.Select(c => new WeighingComponent(c, DetermineWeighingComponentState(c.MDProdOrderPartslistPosState
                                                                                                                                      .MDProdOrderPartslistPosStateIndex))).ToList();
                    }

                    AvailableRoutes = GetAvailableStorages(module);
                }
            }

            return StartNextCompResult.NextCompStarted;
        }

        public static WeighingComponentState DetermineWeighingComponentState(short posStateIndex, bool isForTransport = false)
        {
            if (!isForTransport)
            {
                if (posStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed)
                    return WeighingComponentState.WeighingCompleted;

                if (posStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled)
                    return WeighingComponentState.Aborted;
            }
            else
            {
                if (posStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck)
                    return WeighingComponentState.WeighingCompleted;
            }

            return WeighingComponentState.ReadyToWeighing;
        }

        public virtual void SetACMethodValues(ACMethod acMethod)
        {
            acMethod["TargetQuantity"] = 0.0;
            acMethod[Material.ClassName] = "";
        }

        protected virtual ProdOrderPartslistPosRelation[] OnGetAllMaterials(Database dbIPlus, DatabaseApp dbApp, ProdOrderPartslistPos intermediateChildPos)
        {
            ProdOrderPartslistPosRelation[] queryOpenDosings = s_cQry_WeighMaterials(dbApp, intermediateChildPos.ProdOrderPartslistPosID).ToArray(); //Qry_WeighMaterials(dbApp, intermediateChildPos.ProdOrderPartslistPosID);
            return queryOpenDosings;
        }

        public static ProdOrderPartslistPosRelation[] Qry_WeighMaterials(DatabaseApp dbApp, Guid intermediateChildPosPOPartslistPosID)
        {
            return dbApp.ProdOrderPartslistPosRelation.Include(c => c.SourceProdOrderPartslistPos)
                                                        .Include(c => c.SourceProdOrderPartslistPos.BasedOnPartslistPos)
                                                        .Include(c => c.SourceProdOrderPartslistPos.Material)
                                                        .Include(c => c.SourceProdOrderPartslistPos.Material.BaseMDUnit)
                                                        .Where(c => c.TargetProdOrderPartslistPosID == intermediateChildPosPOPartslistPosID
                                                                    && c.TargetQuantityUOM > 0.00001)
                                                        .ToArray()
                                                        .Where(c => c.MDProdOrderPartslistPosState != null
                                                                && c.TopParentPartslistPosRelation != null && c.TopParentPartslistPosRelation.MDProdOrderPartslistPosState != null
                                                                && c.TopParentPartslistPosRelation.MDProdOrderPartslistPosState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                                                && c.TopParentPartslistPosRelation.MDProdOrderPartslistPosState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled
                                                                && (c.SourceProdOrderPartslistPos != null && c.SourceProdOrderPartslistPos.Material != null
                                                                 && c.SourceProdOrderPartslistPos.Material.UsageACProgram))
                                                        .OrderBy(c => c.Sequence)
                                                        .ToArray();
        }

        public static readonly Func<DatabaseApp, Guid, IEnumerable<ProdOrderPartslistPosRelation>> s_cQry_WeighMaterials =
                EF.CompileQuery<DatabaseApp, Guid, IEnumerable<ProdOrderPartslistPosRelation>>(
                    (ctx, intermediateChildPOPLPosID) => ctx.ProdOrderPartslistPosRelation
                                                            .Include("SourceProdOrderPartslistPos")
                                                            .Include("SourceProdOrderPartslistPos.BasedOnPartslistPos")
                                                            .Include("SourceProdOrderPartslistPos.Material")
                                                            .Include("SourceProdOrderPartslistPos.Material.BaseMDUnit")
                                                            .Include("MDProdOrderPartslistPosState")
                                                            .Where(c => c.TargetProdOrderPartslistPosID == intermediateChildPOPLPosID
                                                                     && c.TargetQuantityUOM > 0.00001
                                                                     && c.SourceProdOrderPartslistPos != null
                                                                     && c.SourceProdOrderPartslistPos.Material != null
                                                                     && c.SourceProdOrderPartslistPos.Material.UsageACProgram

                                                                      && (c.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation == null
                                                                      || (c.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.MDProdOrderPartslistPosState != null
                                                                      && c.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex != (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                                                      && c.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex != (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled))

                                                                      && ((c.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation == null
                                                                      || c.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation == null)
                                                                      || (c.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.MDProdOrderPartslistPosState != null
                                                                      && c.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex != (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                                                      && c.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex != (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled))

                                                            ).OrderBy(c => c.Sequence)
                    );


        private bool RefreshCompStateFromDBAndCheckIsAllCompleted()
        {
            bool result = true;
            ProdOrderPartslistPos intermediateChildPos = null;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                intermediateChildPos = IntermediateChildPos;
            }

            if (intermediateChildPos == null)
                return true;

            using (Database db = new core.datamodel.Database())
            using (DatabaseApp dbApp = new DatabaseApp(db))
            {
                var allRelations = OnGetAllMaterials(db, dbApp, intermediateChildPos);
                using (ACMonitor.Lock(_65050_WeighingCompLock))
                {
                    foreach (ProdOrderPartslistPosRelation rel in allRelations)
                    {
                        WeighingComponent comp = WeighingComponents.FirstOrDefault(c => c.PLPosRelation.ProdOrderPartslistPosRelationID == rel.ProdOrderPartslistPosRelationID);
                        if (comp == null)
                            continue;

                        WeighingComponentState newState = DetermineWeighingComponentState(rel.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex);
                        if (newState == WeighingComponentState.ReadyToWeighing)
                        {
                            comp.SwitchState(newState);
                            comp.TargetWeight = Math.Abs(rel.RemainingDosingWeight);
                            SetInfo(comp, WeighingComponentInfoType.State, null);
                        }

                        if (comp.WeighState != WeighingComponentState.WeighingCompleted && comp.WeighState != WeighingComponentState.Aborted)
                        {
                            result = false;
                        }
                    }
                }
            }

            return result;
        }

        private void RefreshCompWeighingList()
        {
            ProdOrderPartslistPos intermediateChildPos = null;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                intermediateChildPos = IntermediateChildPos;
            }

            if (intermediateChildPos == null)
                return;

            using (Database db = new core.datamodel.Database())
            using (DatabaseApp dbApp = new DatabaseApp(db))
            {
                var result = OnGetAllMaterials(db, dbApp, intermediateChildPos);
                using (ACMonitor.Lock(_65050_WeighingCompLock))
                {
                    foreach (ProdOrderPartslistPosRelation rel in result)
                    {
                        WeighingComponent comp = WeighingComponents.FirstOrDefault(c => c.PLPosRelation.ProdOrderPartslistPosRelationID == rel.ProdOrderPartslistPosRelationID);
                        if (comp == null)
                        {
                            WeighingComponents.Add(new WeighingComponent(rel, DetermineWeighingComponentState(rel.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex)));
                        }
                        else if (comp != null)
                        {
                            comp.RefreshComponent(rel);
                        }
                    }
                }

            }
        }

        #endregion

        #region Methods => FacilityCharge

        public Msg SetFacilityCharge(Guid? facilityChargeID, Guid? plPosRelationID, bool forceSet = false, bool changeOnPAF = false)
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
            if (changeOnPAF)
            {
                PAFManualWeighing manualWeighing = CurrentExecutingFunction<PAFManualWeighing>();
                if (manualWeighing != null && manualWeighing.CurrentACMethod != null)
                {
                    ACMethod temp = manualWeighing.CurrentACMethod.ValueT;
                    if (temp != null)
                    {
                        temp.ParameterValueList[nameof(FacilityCharge)] = facilityChargeID;
                        manualWeighing.ReSendACMethod(temp);
                    }
                }
            }

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

                IEnumerable<FacilityCharge> relevantFacilityCharges = null;

                if (IsProduction)
                {
                    ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == plPosRelationID);

                    if (rel == null)
                    {
                        //Error50374: ProdOrderPartslistPosRelation {0} doesn't exist in the database.
                        return new Msg(this, eMsgLevel.Error, PWClassName, "ValidateFacilityChargeFIFOorLIFO(40)", 40, "Error50374", plPosRelationID);
                    }

                    relevantFacilityCharges = GetFacilityChargesForMaterial(dbApp, rel);
                }
                else
                {
                    PickingPos pickingPosition = dbApp.PickingPos.Include(c => c.PickingMaterial).FirstOrDefault(c => c.PickingPosID == plPosRelationID);
                    if (pickingPosition == null)
                        return null;

                    relevantFacilityCharges = GetFacilityChargesForMaterialPicking(dbApp, pickingPosition);
                }

                if (relevantFacilityCharges != null && relevantFacilityCharges.Any())
                {
                    if (!isLIFO)
                    {
                        FacilityCharge oldestFacilityCharge = relevantFacilityCharges.OrderBy(c => c.FillingDate).FirstOrDefault();
                        if (oldestFacilityCharge != null
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
                        if (youngestFacilityCharge != null
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

        public virtual Msg OnSetFacility(Guid? facilityID, Guid? plPosRelationID)
        {
            return null;
        }

        [ACMethodInfo("", "", 9999)]
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

        [ACMethodInfo("", "", 9999)]
        public ACValueList GetRoutableFacilities(Guid PLPosRelation)
        {
            using (Database db = new Database())
            using (DatabaseApp dbApp = new DatabaseApp(db))
            {
                if (IsProduction)
                {

                    ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation
                                                        .Include(c => c.SourceProdOrderPartslistPos)
                                                        .Include(c => c.SourceProdOrderPartslistPos.Material)
                                                        .Include(c => c.SourceProdOrderPartslistPos.Material.BaseMDUnit)
                                                        .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == PLPosRelation);
                    if (rel != null)
                    {
                        IEnumerable<Facility> facilities = GetAvailableFacilitiesForMaterial(dbApp, rel);
                        return new ACValueList(facilities.Select(c => new ACValue("ID", c.FacilityID)).ToArray());
                    }

                }
                else
                {
                    PickingPos pickingPos = dbApp.PickingPos.FirstOrDefault(c => c.PickingPosID == PLPosRelation);
                    if (pickingPos != null)
                    {
                        QrySilosResult qrySilosResult = null;
                        IEnumerable<facility.ACPartslistManager.QrySilosResult.FacilitySumByLots> result = GetAvailableFacilitiesForMaterialPicking(dbApp, pickingPos, out qrySilosResult);
                        if (result == null || !result.Any())
                            return new ACValueList();
                        return new ACValueList(result.Select(c => new ACValue("ID", c.StorageBin.FacilityID)).ToArray());
                    }
                }
            }
            return null;
        }

        private IEnumerable<FacilityCharge> GetFacilityChargesForMaterial(DatabaseApp dbApp, ProdOrderPartslistPosRelation posRel)
        {
            Guid[] facilities = GetAvailableFacilitiesForMaterial(dbApp, posRel).Select(c => c.FacilityID).ToArray();

            if (ACFacilityManager == null)
                return null;

            return ACFacilityManager.ManualWeighingFacilityChargeListQuery(dbApp, facilities, posRel.SourceProdOrderPartslistPos.MaterialID);
        }

        public IEnumerable<Facility> GetAvailableFacilitiesForMaterial(DatabaseApp dbApp, ProdOrderPartslistPosRelation posRel)
        {
            if (ParentPWGroup == null || ParentPWGroup.AccessedProcessModule == null || PartslistManager == null)
            {
                throw new NullReferenceException("AccessedProcessModule is null");
            }

            facility.ACPartslistManager.QrySilosResult facilities;

            core.datamodel.ACClass accessAClass = ParentPWGroup.AccessedProcessModule.ComponentClass;
            IEnumerable<Route> routes = PartslistManager.GetRoutes(posRel, dbApp, dbApp.ContextIPlus,
                                                                    accessAClass,
                                                                    ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                                                    null,
                                                                    out facilities,
                                                                    null,
                                                                    null,
                                                                    null,
                                                                    false,
                                                                    ReservationMode);

            // Temp fix - GetRoutes DBException
            if (routes == null || facilities == null || facilities.FilteredResult == null || !facilities.FilteredResult.Any())
            {
                routes = PartslistManager.GetRoutes(posRel, dbApp, dbApp.ContextIPlus,
                                                                    accessAClass,
                                                                    ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                                                    null,
                                                                    out facilities,
                                                                    null,
                                                                    null,
                                                                    null,
                                                                    false,
                                                                    ReservationMode);

                if (routes == null || facilities == null || facilities.FilteredResult == null || !facilities.FilteredResult.Any())
                {
                    routes = PartslistManager.GetRoutes(posRel, dbApp, dbApp.ContextIPlus,
                                                                        accessAClass,
                                                                        ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                                                        null,
                                                                        out facilities,
                                                                        null,
                                                                        null,
                                                                        null,
                                                                        false,
                                                                        ReservationMode);
                }
            }

            if (routes == null || facilities == null || facilities.FilteredResult == null || !facilities.FilteredResult.Any())
                return new List<Facility>();

            var routeList = routes.ToList();

            List<Facility> routableFacilities = new List<Facility>();

            PAFManualWeighing manWeigh = CurrentExecutingFunction<PAFManualWeighing>();
            if (manWeigh == null)
            {
                core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
                ACMethod acMethod = refPAACClassMethod?.TypeACSignature();
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
                            facility.ACPartslistManager.QrySilosResult.FacilitySumByLots facilityToAdd = facilities.FilteredResult.FirstOrDefault(c => c.StorageBin.VBiFacilityACClassID.HasValue && c.StorageBin.VBiFacilityACClassID == source.SourceGuid);
                            if (facilityToAdd != null)
                                routableFacilities.Add(facilityToAdd.StorageBin);
                        }
                    }
                }
            }

            if (!IncludeContainerStores)
            {
                routableFacilities = routableFacilities.Where(c => c.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBin).ToList();
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
                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    RoutingService = this.RoutingService,
                    Database = db,
                    AttachRouteItemsToContext = false,
                    SelectionRuleID = PAMSilo.SelRuleID_Storage,
                    Direction = RouteDirections.Backwards,
                    DBSelector = (c, p, r) => typeof(PAMParkingspace).IsAssignableFrom(c.ObjectFullType),
                    DBDeSelector = (c, p, r) => typeof(PAProcessModule).IsAssignableFrom(c.ObjectFullType),
                    MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                    IncludeReserved = true,
                    IncludeAllocated = true
                    };

                RoutingResult routeResult = ACRoutingService.FindSuccessors(toComponent.GetACUrl(), routingParameters);

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
                            if (availableFC != null && availableFC.Count() > 1 && MultipleLotsSelectionRule.HasValue)
                            {
                                if (MultipleLotsSelectionRule.Value != LotSelectionRuleEnum.None)
                                    return availableFC.Any();
                            }

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
                            if (mat != null && fc.MaterialID != mat.MaterialID)
                            {
                                Messages.LogError(this.GetACUrl(), "Wrong quant(20)", "The quant ID: " + fc.FacilityChargeID + ", material ID: " +
                                                  mat?.MaterialID);
                            }

                            msg = SetFacilityCharge(fc.FacilityChargeID, materialID);
                            if (msg == null)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        private IEnumerable<FacilityCharge> TryGetLastUsedLot(IEnumerable<FacilityCharge> facilityCharges, Guid materialID, DatabaseApp dbApp)
        {
            Guid? moduleID = ParentPWGroup?.AccessedProcessModule?.ComponentClass?.ACClassID;

            if (!moduleID.HasValue)
            {
                Messages.LogError(this.GetACUrl(), nameof(TryGetLastUsedLot)+"(10)", "The AccessedProcessModule component class is null!");
                return new List<FacilityCharge>();
            }

            MaterialConfig lastUsedFC = null;

            try
            {
                lastUsedFC = dbApp.MaterialConfig.FirstOrDefault(c => c.VBiACClassID == moduleID && c.MaterialID == materialID
                                                                                                 && c.KeyACUrl == MaterialConfigLastUsedLotKeyACUrl);
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), nameof(TryGetLastUsedLot) + "(20)", e);

                try
                {
                    lastUsedFC = dbApp.MaterialConfig.FirstOrDefault(c => c.VBiACClassID == moduleID && c.MaterialID == materialID
                                                                                                 && c.KeyACUrl == MaterialConfigLastUsedLotKeyACUrl);
                }
                catch (Exception exc)
                {
                    Messages.LogException(this.GetACUrl(), nameof(TryGetLastUsedLot) + "(25)", exc);
                }
            }

            if (lastUsedFC == null || lastUsedFC.Value == null)
            {
                if (facilityCharges != null && facilityCharges.Count() == 1)
                {
                    return facilityCharges;
                }

                return new List<FacilityCharge>();
            }

            Guid? fcID = lastUsedFC.Value as Guid?;
            if (fcID.HasValue)
            {
                FacilityCharge fc = facilityCharges.FirstOrDefault(c => c.FacilityChargeID == fcID);
                if (fc != null)
                    return new List<FacilityCharge>() { fc };
                else
                    Messages.LogInfo(this.GetACUrl(), nameof(TryGetLastUsedLot) + "30", "In the available facility charges missing last used facility charge.");
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
                        Guid? matID;

                        if (IsProduction)
                        {
                            ProdOrderPartslistPosRelation posRel = dbApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == materialID);
                            if (posRel == null)
                                return;

                            matID = posRel.SourceProdOrderPartslistPos.MaterialID;
                        }
                        else
                        {
                            PickingPos pickingPos = dbApp.PickingPos.FirstOrDefault(c => c.PickingPosID == materialID);
                            if (pickingPos == null)
                                return;

                            matID = pickingPos.Material.MaterialID;
                        }

                        MaterialConfig lastUsedFC = dbApp.MaterialConfig.FirstOrDefault(c => c.VBiACClassID == moduleID && c.MaterialID == matID
                                                                                                                        && c.KeyACUrl == MaterialConfigLastUsedLotKeyACUrl);

                        if (lastUsedFC == null)
                        {
                            lastUsedFC = MaterialConfig.NewACObject(dbApp, null);
                            lastUsedFC.MaterialID = matID;
                            lastUsedFC.VBiACClassID = moduleID;
                            lastUsedFC.KeyACUrl = MaterialConfigLastUsedLotKeyACUrl;
                            dbApp.MaterialConfig.Add(lastUsedFC);
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

        private void SelectFacilityChargeOrFacility(Guid? currentOpenMaterial, Guid? currentFacilityCharge, WeighingComponentInfoType infoType)
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
                    WeighingComponent comp = null;
                    if (IsProduction)
                        comp = GetWeighingComponent(currentOpenMaterial); // WeighingComponents.FirstOrDefault(c => c.PLPosRelation == CurrentOpenMaterial.Value);
                    else
                        comp = GetWeighingComponentPicking(currentOpenMaterial);                    
                    
                    if (comp == null)
                    {
                        //Error50270: The component {0} doesn't exist for weighing.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "SelectFacilityChargeOrFacility(20)", 1563, "Error50270", currentOpenMaterial.Value);
                    }
                }

                if (msg == null)
                {
                    //Guid? currentFacilityCharge = CurrentFacilityCharge;
                    msg = InitializePAFACMethod(paf.CurrentACMethod.ValueT, currentFacilityCharge);

                    if (msg == null)
                    {
                        paf.RunManualWeighing();
                    }
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

                    IEnumerable<FacilityCharge> facilityChargeList = null;

                    if (IsProduction)
                    {
                        ProdOrderPartslistPosRelation posRel = dbApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == weighingComponent.PLPosRelation.ProdOrderPartslistPosRelationID);
                        if (posRel == null)
                            return null;

                        facilityChargeList = GetFacilityChargesForMaterial(dbApp, posRel);
                    }
                    else
                    {
                        PickingPos pickingPos = dbApp.PickingPos.FirstOrDefault(c => c.PickingPosID == weighingComponent.PickingPosition.PickingPosID);
                        if (pickingPos == null)
                            return null;

                        facilityChargeList = GetFacilityChargesForMaterialPicking(dbApp, pickingPos);
                    }

                    if (facilityChargeList != null && !facilityChargeList.Any() && !String.IsNullOrEmpty(AutoInsertQuantToStore))
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
                                        fl.UpdateExpirationInfo(material);
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
                ManualWeighingNextTask.ValueT = ManualWeighingTaskInfo.None;
                acMethod["TargetQuantity"] = weighingComponent.TargetWeight;
                acMethod[Material.ClassName] = weighingComponent.MaterialName;
                acMethod["PLPosRelation"] = weighingComponent.PLPosRelation.ProdOrderPartslistPosRelationID;

                Guid? currentFacilityCharge = CurrentFacilityCharge;

                if (currentFacilityCharge.HasValue)
                    InitializePAFACMethod(acMethod, currentFacilityCharge);

                bool isLast;
                using (ACMonitor.Lock(_65050_WeighingCompLock))
                {
                    isLast = WeighingComponents.Count(c => c.WeighState == WeighingComponentState.ReadyToWeighing) == 1;
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
                        return new Msg(this, eMsgLevel.Error, PWClassName, "InitializePAFACMethod(10)", 1607, "Error50376", currentFacilityCharge);
                    }
                    facility = fc.Facility;
                }

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

            return null;
        }

        #endregion

        #region Methods => StartWeighingComponent

        private StartNextCompResult StartManualWeighingNextComp(PAProcessModule module, WeighingComponent weighingComponent, bool? hasQuantsChecked)
        {
            Msg msg = null;

            if (((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
                || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
            {
                UnSubscribeToProjectWorkCycle();
                // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                if (CurrentACState == ACStateEnum.SMRunning)
                    CurrentACState = ACStateEnum.SMCompleted;
                return StartNextCompResult.Done;
            }

            if (module == null)
                return StartNextCompResult.CycleWait;

            if ((!hasQuantsChecked.HasValue || !hasQuantsChecked.Value)
                && !String.IsNullOrEmpty(AutoInsertQuantToStore))
            {
                DoAutoInsertQuantToStore(weighingComponent);
            }

            core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
            ACMethod acMethod = refPAACClassMethod?.TypeACSignature();
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

            if (!(bool)ExecuteMethod(nameof(GetConfigForACMethod), acMethod, true))
                return StartNextCompResult.CycleWait;

            //TODO: get target scale from partslistconfig if is rule configured for this material

            double? dosingWeight = weighingComponent?.TargetWeight;

            bool interDischargingNeeded = false;
            if (AutoInterDis && weighingComponent != null && IsProduction)
            {
                ProdOrderPartslistPosRelation relation = weighingComponent.PLPosRelation;
                if (relation != null)
                {
                    using (Database dbIPlus = new core.datamodel.Database())
                    using (DatabaseApp dbApp = new DatabaseApp(dbIPlus))
                    {
                        relation = relation.FromAppContext<ProdOrderPartslistPosRelation>(dbApp);

                        dosingWeight = Math.Abs(relation.RemainingDosingWeight);
                        IPAMContScale scale = ParentPWGroup != null ? ParentPWGroup.AccessedProcessModule as IPAMContScale : null;
                        ScaleBoundaries scaleBoundaries = null;


                        if (scale != null)
                        {
                            ProdOrderPartslistPosRelation[] relations = null;
                            using (ACMonitor.Lock(_65050_WeighingCompLock))
                            {
                                relations = WeighingComponents.Select(c => c.PLPosRelation).ToArray();
                            }

                            scaleBoundaries = OnGetScaleBoundariesForDosing(scale, dbApp, relations);
                        }
                        if (scaleBoundaries != null && scaleBoundaries.MaxWeightCapacity > 0.00000001)
                        {
                            double? remainingWeight = null;
                            if (scaleBoundaries.RemainingWeightCapacity.HasValue)
                                remainingWeight = scaleBoundaries.RemainingWeightCapacity.Value;
                            else if (scaleBoundaries.MaxWeightCapacity > 0.00000001)
                                remainingWeight = scaleBoundaries.MaxWeightCapacity;
                            if (remainingWeight.HasValue)
                            {
                                // FALL A:
                                if (Math.Abs(relation.RemainingDosingWeight) > remainingWeight.Value)
                                {
                                    // Falls die Komponentensollmenge größer als die maximale Waagenkapazität ist, dann muss die Komponente gesplittet werden, 
                                    // ansonsten dosiere volle sollmenge nach der Zwischenentleerung
                                    if (scaleBoundaries.MaxWeightCapacity > 0.00000001 
                                        && dosingWeight > scaleBoundaries.MaxWeightCapacity 
                                        && (   !scaleBoundaries.MinDosingWeight.HasValue 
                                            || remainingWeight.Value > scaleBoundaries.MinDosingWeight.Value))
                                    {
                                        // Fall A.1:
                                        interDischargingNeeded = true;
                                        dosingWeight = remainingWeight.Value;
                                    }
                                    else
                                    {
                                        ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMInterDischarging;
                                        return StartNextCompResult.Done;
                                    }
                                }

                                if (scaleBoundaries.RemainingVolumeCapacity.HasValue
                                    && relation.SourceProdOrderPartslistPos.Material != null
                                    && relation.SourceProdOrderPartslistPos.Material.IsDensityValid)
                                {
                                    double remainingDosingVolume = relation.SourceProdOrderPartslistPos.Material.ConvertToVolume(Math.Abs(relation.RemainingDosingQuantityUOM));
                                    if (remainingDosingVolume > scaleBoundaries.RemainingVolumeCapacity.Value)
                                    {
                                        double targetVolume = relation.SourceProdOrderPartslistPos.Material.ConvertToVolume(relation.TargetQuantityUOM);
                                        // FALL B:
                                        // Falls die Komponentenvolumen größer als die maximale Volumenkapazität ist, dann muss die Komponente gesplittet werden, 
                                        // ansonsten dosiere volle sollmenge nach der Zwischenentleerung
                                        if (scaleBoundaries.MaxVolumeCapacity > 0.00000001 && targetVolume > scaleBoundaries.MaxVolumeCapacity)
                                        {
                                            double dosingWeightAccordingVolume = (scaleBoundaries.RemainingVolumeCapacity.Value * relation.SourceProdOrderPartslistPos.Material.Density) / 1000;
                                            // Falls Dichte > 1000 g/dm³, dann kann das errechnete zu dosierende Teilgewicht größer als das Restgewicht in der Waage sein,
                                            // dann muss das Restgewicht genommen werden (interDischargingNeeded ist true wenn weiter oben die Restgewichtermittlung durchgeführt wurde 
                                            // und die komponentenmenge gesplittet werden musste. SIEHE FALL A.1)
                                            if (!interDischargingNeeded || dosingWeightAccordingVolume < dosingWeight)
                                            {
                                                // FALL B.1:
                                                dosingWeight = dosingWeightAccordingVolume;
                                            }
                                            // Prüfe erneut ob Restgewicht der Waage überschritten wird, falls ja reduziere die Restmenge
                                            // Dieser Fall kommt dann vor, wenn die Dichte > 1000 g/dm³ ist, jedoch die zu dosierende Komponentenmenge kleiner war als das Restgewicht der Waage.
                                            // Dann wurde interDischargingNeeded nicht gesetzt (FALL A ist nicht eingetreten).
                                            if (!remainingWeight.HasValue && dosingWeight > remainingWeight.Value)
                                                dosingWeight = remainingWeight.Value;
                                            interDischargingNeeded = true;
                                        }
                                        else
                                        {
                                            ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMInterDischarging;
                                            return StartNextCompResult.Done;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (weighingComponent != null)
            {
                if (dosingWeight == null)
                    dosingWeight = weighingComponent.TargetWeight;

                if (dosingWeight.HasValue && dosingWeight.Value == double.NaN)
                {
                    Messages.LogDebug(this.GetACUrl(), "ManWeighing(double.Nan)", string.Format("Target weight:{0}", weighingComponent.TargetWeight));
                }

                acMethod["TargetQuantity"] = dosingWeight.Value;
                acMethod[Material.ClassName] = weighingComponent.MaterialName;
                if (IsProduction)
                    acMethod["PLPosRelation"] = weighingComponent.PLPosRelation.ProdOrderPartslistPosRelationID;
                else
                    acMethod["PLPosRelation"] = weighingComponent.PickingPosition.PickingPosID;
                acMethod["Route"] = new Route();

                if (!IsManualWeighing)
                {
                    try
                    {
                        bool? skipTol = weighingComponent.PLPosRelation?.SourceProdOrderPartslistPos?.BasedOnPartslistPos?.SuggestQuantQOnPosting;
                        acMethod["SkipToleranceCheck"] = skipTol != null ? skipTol : false;
                    }
                    catch (Exception e)
                    {
                        Messages.LogException(this.GetACUrl(), nameof(StartManualWeighingNextComp), e);
                    }
                }
                else
                {
                    acMethod["SkipToleranceCheck"] = false;
                }

                Guid? currentFacilityCharge = CurrentFacilityCharge;
                if (currentFacilityCharge.HasValue)
                {
                    InitializePAFACMethod(acMethod, currentFacilityCharge);
                }
                else
                {
                    Messages.LogError(this.GetACUrl(), nameof(StartManualWeighingNextComp) + "(20)", "Current facility charge is null on StartManualWeighingNextComp");
                }

                bool isLast;
                using (ACMonitor.Lock(_65050_WeighingCompLock))
                {
                    isLast = WeighingComponents.Count(c => c.WeighState == WeighingComponentState.ReadyToWeighing) == 1;
                }

                acMethod["IsLastWeighingMaterial"] = isLast;
            }
            else
            {
                acMethod["TargetQuantity"] = PWBinSelection.BinSelectionReservationQuantity;
                acMethod["Route"] = new Route();
            }

            if (!(bool)ExecuteMethod(nameof(AfterConfigForACMethodIsSet), acMethod, true))
            {
                Messages.LogInfo(this.GetACUrl(), nameof(StartManualWeighingNextComp), nameof(AfterConfigForACMethodIsSet) + " return false!");
                return StartNextCompResult.CycleWait;
            }

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
            {
                Messages.LogInfo(this.GetACUrl(), nameof(StartManualWeighingNextComp), nameof(CreateNewProgramLog) + " Cycle wait!");
                return StartNextCompResult.CycleWait;
            }
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

            if (interDischargingNeeded && AutoInterDis)
                ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMInterDischarging;

            _LastOpenMaterial = CurrentOpenMaterial;
            ExecuteMethod(nameof(OnACMethodSended), acMethod, true, responsibleFunc);
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
                    if (IsProduction)
                        OnDeletedTaskProd(sender, e, wrapObject, eM);
                    else if (IsTransport)
                        OnDeletedTaskPicking(sender, e, wrapObject, eM);

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

                                WeighingComponent comp = null;

                                if (IsProduction)
                                    comp = GetWeighingComponent(currentOpenMaterial);
                                else
                                    comp = GetWeighingComponentPicking(currentOpenMaterial);

                                if (comp == null)
                                {
                                    if (!FreeSelectionMode)
                                    {
                                        //Error50270: The component {0} doesn't exist for weighing.
                                        ActivateProcessAlarmWithLog(new Msg(this, eMsgLevel.Error, PWClassName, "TaskCallback(20)", 1866, "Error50270", currentOpenMaterial));
                                    }
                                    return;
                                }

                                if (comp.WeighState < WeighingComponentState.InWeighing)
                                {
                                    comp.SwitchState(WeighingComponentState.InWeighing);

                                    if (IsProduction)
                                        SetRelationState(comp.PLPosRelation.ProdOrderPartslistPosRelationID, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.InProcess);
                                    else
                                        SetRelationStatePicking(comp.PickingPosition.PickingPosID, MDDelivPosLoadState.DelivPosLoadStates.LoadingActive);

                                    WeighingComponentInfoType infoType = WeighingComponentInfoType.StateSelectCompAndFC_F;
                                    if (!FreeSelectionMode && !AutoSelectLot)
                                        infoType = WeighingComponentInfoType.StateSelectFC_F;

                                    Guid? currentFacilityCharge = CurrentFacilityCharge;
                                    SetInfo(comp, infoType, currentFacilityCharge.Value);
                                    SaveLastUsedLot(currentFacilityCharge, currentOpenMaterial);
                                }
                            }
                        }
                    }
                }
            }
            _InCallback = false;
        }

        private void OnDeletedTaskProd(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject, ACMethodEventArgs eM)
        {
            if (DiffWeighing)
            {
                CompleteOnDifferenceWeighing(sender, e, eM);
            }
            else
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

                                ACMethod parentACMethod = e.ParentACMethod;

                                double? actWeight = e.GetDouble("ActualQuantity");
                                //double? tolerancePlus = (double)e.ParentACMethod["TolerancePlus"];
                                double? toleranceMinus = (double)parentACMethod["ToleranceMinus"];
                                double? targetQuantity = (double)parentACMethod["TargetQuantity"];

                                bool isWeighingInTol = true;

                                if (targetQuantity.HasValue && actWeight.HasValue && toleranceMinus.HasValue)
                                {
                                    double actWeightRounded = Math.Round(actWeight.Value, 5);
                                    if (actWeightRounded < Math.Round(targetQuantity.Value - toleranceMinus.Value, 5))
                                    {
                                        isWeighingInTol = false;
                                    }

                                    if (!isWeighingInTol)
                                    {
                                        ACValue skipTolCheckValue = parentACMethod.ParameterValueList.GetACValue("SkipToleranceCheck");
                                        if (skipTolCheckValue != null)
                                        {
                                            bool skipToleranceCheck = skipTolCheckValue.ParamAsBoolean;
                                            if (skipToleranceCheck)
                                                isWeighingInTol = true;
                                        }
                                    }

                                    if (AutoInterDis && currentOpenMaterial != null)
                                    {
                                        using (DatabaseApp dbApp = new DatabaseApp())
                                        {
                                            var relation = dbApp.ProdOrderPartslistPosRelation.FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == currentOpenMaterial);
                                            if (relation != null)
                                            {
                                                var tempTargetQ = Math.Abs(relation.RemainingDosingQuantityUOM);
                                                if (actWeightRounded < Math.Round(tempTargetQ - toleranceMinus.Value, 5))
                                                {
                                                    isWeighingInTol = false;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (actWeight > 0.000001)
                                {
                                    bool scaleOtherComp = ScaleOtherComp && _ScaleComp && (_IsAborted || function.CurrentACState == ACStateEnum.SMAborted);
                                    msg = DoManualWeighingBooking(actWeight, isWeighingInTol, false, currentOpenMaterial, currentFacilityCharge, false,
                                                                  scaleOtherComp, targetQuantity);
                                    _ScaleComp = false;
                                }
                                else if (isWeighingInTol)
                                {
                                    SetRelationState(currentOpenMaterial, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled);
                                }

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
                                _IsAborted = false;

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
                                    SetInfo(weighingComp, WeighingComponentInfoType.State, currentFacilityCharge);
                                }
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Messages.LogException(this.GetACUrl(), "TaskCallback(10)", exc);
                }
                finally
                {
                    SetCanStartFromBSO(true);
                    CurrentOpenMaterial = null;
                    CurrentFacilityCharge = null;
                    SubscribeToProjectWorkCycle();
                }
            }
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

        public Msg DoManualWeighingBooking(double? actualWeight, bool thisWeighingIsInTol, bool isConsumedLot, Guid? currentOpenMaterial, Guid? currentFacilityCharge,
                                           bool isForInterdischarge = false, bool scaleOtherCompAfterAbort = false, double? tQuantityFromPAF = null)
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

                    if (currentOpenMaterial == null)
                    {
                        // Error50373: Manual weighing error, the property {0} is null!
                        // Fehler bei Handverwiegung, die Eigenschaft {0} ist null!
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(10)", 1953, "Error50373", "CurrentOpenMaterial");
                        ActivateProcessAlarmWithLog(msg, false);
                        return msg;
                    }

                    ProdOrderPartslistPosRelation weighingPosRelation = dbApp.ProdOrderPartslistPosRelation.Include(c => c.SourceProdOrderPartslistPos)
                                                                                                           .Include(c => c.SourceProdOrderPartslistPos.Material)
                                                                                                           .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == currentOpenMaterial);
                    if (weighingPosRelation != null)
                    {
                        bool changePosState = false;

                        if (currentFacilityCharge == null)
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
                        //else
                        //{
                        //    facility = dbApp.Facility.FirstOrDefault(c => c.FacilityID == CurrentFacility);
                        if (facility == null)
                        {
                            //Error50378: Can't get the Facility from database with FacilityID: {0}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(31)", 1686, "Error50378", "from quant!");
                            ActivateProcessAlarmWithLog(msg, false);
                            return msg;
                        }
                        //}

                        if (facilityCharge != null && facilityCharge.MaterialID != weighingPosRelation.SourceProdOrderPartslistPos.MaterialID
                                                   && facilityCharge.Material.ProductionMaterialID != weighingPosRelation.SourceProdOrderPartslistPos.MaterialID)
                        {
                            msg = new Msg(this, eMsgLevel.Error, nameof(PWManualWeighing), nameof(DoManualWeighingBooking) + "(32)", 2809,
                                           "The material of quant is different than weighing material. The quant ID: " + facilityCharge.FacilityChargeID + " The material ID: " +
                                           weighingPosRelation.SourceProdOrderPartslistPos.MaterialID);

                            ActivateProcessAlarmWithLog(msg, false);

                            return msg;
                        }

                        double actualQuantity = actualWeight.HasValue ? weighingPosRelation.SourceProdOrderPartslistPos.Material.ConvertBaseWeightToBaseUnit(actualWeight.Value) : 0;
                        double targetQuantity = weighingPosRelation.TargetQuantityUOM;
                        WeighingComponent comp = GetWeighingComponent(weighingPosRelation.ParentProdOrderPartslistPosRelationID);
                        if (comp != null)
                            targetQuantity = weighingPosRelation.SourceProdOrderPartslistPos.Material.ConvertBaseWeightToBaseUnit(comp.TargetWeight);

                        if (!isForInterdischarge && (!tQuantityFromPAF.HasValue || _IsLotChanged))
                        {
                            double calcActualQuantity = targetQuantity + weighingPosRelation.RemainingDosingQuantityUOM;
                            if (tQuantityFromPAF.HasValue && calcActualQuantity > 0.00001)
                            {
                                calcActualQuantity = tQuantityFromPAF.Value + weighingPosRelation.RemainingDosingQuantityUOM;
                            }
                            if (actualQuantity > calcActualQuantity)
                                actualQuantity = actualQuantity - calcActualQuantity;
                        }

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

                                        if (  !facilityCharge.NotAvailable 
                                            && Math.Abs(facilityCharge.Material.ZeroBookingTolerance) > double.Epsilon
                                            && facilityCharge.StockQuantityUOM < facilityCharge.Material.ZeroBookingTolerance)
                                        {
                                            bool? suggestQOnPosting = weighingPosRelation.SourceProdOrderPartslistPos?.BasedOnPartslistPos?.SuggestQuantQOnPosting;
                                            if (suggestQOnPosting.HasValue && suggestQOnPosting.Value)
                                            {
                                                DoFacilityChargeZeroBooking(facilityCharge, dbApp);
                                            }
                                        }

                                        //Messages.LogInfo(this.GetACUrl(), "", "ManualWeighingTrace - changePosState value: " + changePosState.ToString());

                                        if (changePosState)
                                        {
                                            //Messages.LogInfo(this.GetACUrl(), "", "ManualWeighingTrace - posState value: " + posState.ToString());

                                            weighingPosRelation.MDProdOrderPartslistPosState = posState;
                                            if (!AutoInterDis)
                                            {
                                                if (posState != null && posState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed)
                                                {
                                                    var unconfirmedBookings = weighingPosRelation.FacilityBooking_ProdOrderPartslistPosRelation
                                                                                                 .Where(c => c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New);

                                                    //Messages.LogInfo(this.GetACUrl(), "", "ManualWeighingTrace - Bookings count with state new:" + unconfirmedBookings?.Count().ToString());

                                                    foreach (var booking in unconfirmedBookings)
                                                    {
                                                        booking.MaterialProcessState = GlobalApp.MaterialProcessState.Processed;
                                                        //Messages.LogInfo(this.GetACUrl(), "", "ManualWeighingTrace - Booking change state");
                                                    }
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

                                            if (scaleOtherCompAfterAbort)
                                            {
                                                var queryOpenDosings = weighingPosRelation.ProdOrderBatch.ProdOrderPartslistPosRelation_ProdOrderBatch
                                                    .Where(c => c.RemainingDosingWeight < -0.00001 // TODO: Unterdosierung ist Min-Dosiermenge auf Waage
                                                    && c.MDProdOrderPartslistPosState != null
                                                    && (c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created
                                                        || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted));

                                                if (weighingPosRelation.ActualQuantityUOM > 0.1 && queryOpenDosings.Any())
                                                {
                                                    double scaleFactor = weighingPosRelation.ActualQuantityUOM / weighingPosRelation.TargetQuantityUOM;
                                                    foreach (var plPos in queryOpenDosings)
                                                    {
                                                        if (plPos != weighingPosRelation)
                                                        {
                                                            plPos.TargetQuantityUOM = plPos.TargetQuantityUOM * scaleFactor;

                                                            var wComp = GetWeighingComponent(plPos.ProdOrderPartslistPosRelationID);
                                                            if (wComp != null)
                                                                wComp.TargetWeight = Math.Abs(plPos.RemainingDosingWeight);

                                                        }
                                                    }
                                                    SetInfo(null, WeighingComponentInfoType.RefreshCompTargetQ, null);

                                                    msg = dbApp.ACSaveChanges();

                                                    RefreshActiveNodesOnScaleComponent();
                                                }
                                            }
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
                    if (fc.NotAvailable)
                        return null;

                    return DoFacilityChargeZeroBooking(fc, dbApp);
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
        }

        public Msg DoFacilityChargeZeroBooking(FacilityCharge fc, DatabaseApp dbApp)
        {
            try
            {
                ACMethodBooking fbtZeroBooking = ZeroBookingFacilityCharge as ACMethodBooking;
                if (fbtZeroBooking == null)
                {
                    //Error50364: Can not find the zero booking ACMethod!
                    return new Msg(this, eMsgLevel.Error, PWClassName, "DoFacilityChargeZeroBooking(10)", 2133, "Error50364");
                }
                fbtZeroBooking.InwardFacilityCharge = fc;
                fbtZeroBooking.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(dbApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);

                ACMethodEventArgs resultBooking = ACFacilityManager.BookFacilityWithRetry(ref fbtZeroBooking, dbApp);
                if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                {
                    if (String.IsNullOrEmpty(resultBooking.ValidMessage.Message))
                        resultBooking.ValidMessage.Message = resultBooking.ResultState.ToString();
                    return resultBooking.ValidMessage;
                }
                else
                {
                    if (!fbtZeroBooking.ValidMessage.IsSucceded() || fbtZeroBooking.ValidMessage.HasWarnings())
                        return fbtZeroBooking.ValidMessage;
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
            return null;
        }

        public virtual void CompleteOnDifferenceWeighing(IACPointNetBase sender, ACEventArgs e, ACMethodEventArgs eM)
        {
            try
            {
                PAProcessModule module = sender.ParentACComponent as PAProcessModule;
                if (module != null)
                {
                    PAProcessFunction function = module.GetExecutingFunction<PAProcessFunction>(eM.ACRequestID);
                    if (function != null)
                    {
                        WeighingComponentState state = WeighingComponentState.InWeighing;
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

                            using (DatabaseApp dbApp = new DatabaseApp())
                            {
                                var relation = dbApp.ProdOrderPartslistPosRelation.Include(c => c.FacilityPreBooking_ProdOrderPartslistPosRelation)
                                                                                  .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == currentOpenMaterial);
                                if (relation == null)
                                {
                                    //Error50592 : The ProdOrderPartslistPosRelation with ID: {0} doesn't exists in the database!
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, nameof(CompleteOnDifferenceWeighing) + "(10)", 3659, "Error50592", currentOpenMaterial);
                                    return;
                                }

                                bool onlyComplete = false;
                                ACValue onlyCompleteValue = e.GetACValue("CompleteOnDiffWeighing");
                                if (onlyCompleteValue != null)
                                {
                                    onlyComplete = onlyCompleteValue.ParamAsBoolean;
                                }

                                double? grossWeight = e.GetDouble("ActualQuantity");
                                if (grossWeight.HasValue && !onlyComplete)
                                {
                                    FacilityCharge facilityCharge = null;
                                    Facility facility = null;

                                    if (currentFacilityCharge != null)
                                    {
                                        facilityCharge = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == currentFacilityCharge) as FacilityCharge;
                                        if (facilityCharge == null)
                                        {
                                            // Error50367: The quant {0} doesn't exist in the database!
                                            msg = new Msg(this, eMsgLevel.Error, PWClassName, nameof(CompleteOnDifferenceWeighing) + "(20)", 3675, "Error50367", currentFacilityCharge);
                                            ActivateProcessAlarmWithLog(msg, false);
                                            return;
                                        }
                                        facility = facilityCharge.Facility;
                                    }

                                    var prebookings = relation.FacilityPreBooking_ProdOrderPartslistPosRelation.ToArray();
                                    var preBooking = prebookings.LastOrDefault();

                                    if (preBooking != null)
                                    {
                                        double? prevQuantity = preBooking.OutwardQuantity;
                                        if (!prevQuantity.HasValue)
                                        {
                                            //Error50593 : The quantity from the first weighing is not saved!
                                            msg = new Msg(this, eMsgLevel.Error, PWClassName, nameof(CompleteOnDifferenceWeighing) + "(30)", 3691, "Error50593");
                                            ActivateProcessAlarmWithLog(msg, false);
                                            return;
                                        }

                                        //if (preBooking.OutwardFacilityCharge.FacilityChargeID != facilityCharge.FacilityChargeID)
                                        //{
                                        //    //Error : The quant from the first weighing is not equal to the quant from the second weighing!
                                        //    msg = new Msg(this, eMsgLevel.Error, PWClassName, nameof(CompleteOnDifferenceWeighing) + "(40)", 3699, "Error50594");
                                        //    ActivateProcessAlarmWithLog(msg, false);
                                        //    return;
                                        //}

                                        double bookingQuantity = prevQuantity.Value - grossWeight.Value;
                                        if (bookingQuantity > 0.000001)
                                        {
                                            Msg msgBooking = DoManualWeighingBooking(bookingQuantity, false, isComponentConsumed, currentOpenMaterial, preBooking.OutwardFacilityCharge.FacilityChargeID);
                                            if (msgBooking != null)
                                            {
                                                ActivateProcessAlarmWithLog(msgBooking);
                                            }
                                            else
                                            {
                                                preBooking.DeleteACObject(dbApp, true);
                                                msg = dbApp.ACSaveChangesWithRetry();

                                                if (msg != null)
                                                {
                                                    ActivateProcessAlarmWithLog(msg);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Error : The weiging on end {0} {1} is greather than weigh on start {2} {3}!
                                            Msg msg1 = new Msg(this, eMsgLevel.Error, PWClassName, nameof(CompleteOnDifferenceWeighing) + "(50)", 3699, "Error50598",
                                                               grossWeight.Value, "kg", prevQuantity.Value, "kg");
                                            ActivateProcessAlarmWithLog(msg1, false);
                                        }
                                    }
                                    else if (grossWeight > 0.000001)
                                    {
                                        //create prebooking and remember gross quantity
                                        FacilityPreBooking facilityPreBooking = ProdOrderManager.NewOutwardFacilityPreBooking(this.ACFacilityManager, dbApp, relation);
                                        ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                                        bookingParam.OutwardQuantity = (double)grossWeight;
                                        bookingParam.OutwardFacility = facility;
                                        bookingParam.OutwardFacilityCharge = facilityCharge;
                                        bookingParam.SetCompleted = false;
                                        if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                                            bookingParam.PropertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();
                                        msg = dbApp.ACSaveChangesWithRetry();

                                        if (msg != null)
                                        {
                                            ActivateProcessAlarmWithLog(msg);
                                            return;
                                        }
                                    }
                                }

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

                                changeState = true;
                            }

                            if (msg != null || function.CurrentACState == ACStateEnum.SMAborted ||
                                (function.LastACState == ACStateEnum.SMResetting && function.CurrentACState == ACStateEnum.SMIdle && _IsAborted))
                            {
                                _IsAborted = false;

                                Msg msgResult = SetRelationState(currentOpenMaterial, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled);
                                if (msgResult != null)
                                    ActivateProcessAlarmWithLog(msgResult);

                                state = WeighingComponentState.Aborted;
                                changeState = true;
                            }

                            if (currentOpenMaterial != null && changeState)
                            {
                                WeighingComponent weighingComp = GetWeighingComponent(currentOpenMaterial);
                                if (weighingComp != null)
                                {
                                    weighingComp.SwitchState(state);
                                    SetInfo(weighingComp, WeighingComponentInfoType.StateSelectCompAndFC_F, currentFacilityCharge);
                                }

                                PAFManualWeighing currentFunction = CurrentExecutingFunction<PAFManualWeighing>();
                                if (currentFunction != null)
                                {
                                    ACMethod acMethod = currentFunction.CurrentACMethod.ValueT;
                                    if (acMethod != null)
                                    {
                                        UpdatePAFACMethod(weighingComp, acMethod);
                                        currentFunction.SubscribeToProjectWorkCycle();
                                    }
                                    else
                                    {
                                        Messages.LogMessage(eMsgLevel.Error, this.GetACUrl(), nameof(StartWeighing), "The ACMethod from function is null!");
                                    }
                                }
                                else
                                {
                                    SubscribeToProjectWorkCycle();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Messages.LogException(this.GetACUrl(), "TaskCallback(10)", exc);
            }
            finally
            {
                SetCanStartFromBSO(true);
                _LastOpenMaterial = null;
                //CurrentOpenMaterial = null;
                //CurrentFacilityCharge = null;
                SubscribeToProjectWorkCycle();
            }
        }

        [ACMethodInfo("", "en{'Weigh difference'}de{'Unterschiedliche Gewichte'}", 9999)]
        public virtual Msg WeighDifference()
        {
            Msg msg = null;

            PAFManualWeighing manWeighing = CurrentExecutingFunction<PAFManualWeighing>();

            if (manWeighing == null)
            {
                return new Msg(eMsgLevel.Error, "The manual weighing currently is not active.");
            }

            manWeighing.CompleteOnDifferenceWeighing();

            return msg;
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

        public void SetInfo(WeighingComponent weighingComp, WeighingComponentInfoType infoType, Guid? facilityCharge, bool dbAutoRefresh = false,
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
                            if (weighingComp.PLPosRelation != null)
                                compInfo.PLPosRelation = weighingComp.PLPosRelation.ProdOrderPartslistPosRelationID;

                            if (weighingComp.PickingPosition != null)
                                compInfo.PickingPos = weighingComp.PickingPosition.PickingPosID;

                            compInfo.WeighingComponentState = (short)weighingComp.WeighState;
                            break;
                        }
                    case WeighingComponentInfoType.StateSelectFC_F:
                    case WeighingComponentInfoType.StateSelectCompAndFC_F:
                        {
                            if (weighingComp.PLPosRelation != null)
                                compInfo.PLPosRelation = weighingComp.PLPosRelation.ProdOrderPartslistPosRelationID;

                            if (weighingComp.PickingPosition != null)
                                compInfo.PickingPos = weighingComp.PickingPosition.PickingPosID;

                            compInfo.WeighingComponentState = (short)weighingComp.WeighState;
                            if (facilityCharge != null)
                                compInfo.FacilityCharge = facilityCharge;
                            break;
                        }
                    case WeighingComponentInfoType.SelectCompReturnFC_F:
                        {
                            if (weighingComp.PLPosRelation != null)
                                compInfo.PLPosRelation = weighingComp.PLPosRelation.ProdOrderPartslistPosRelationID;

                            if (weighingComp.PickingPosition != null)
                                compInfo.PickingPos = weighingComp.PickingPosition.PickingPosID;

                            break;
                        }
                    case WeighingComponentInfoType.SelectFC_F:
                        {
                            if (facilityCharge != null)
                                compInfo.FacilityCharge = facilityCharge;

                            compInfo.FC_FAutoRefresh = dbAutoRefresh;
                            compInfo.IsLotChange = lotChange;
                            if (weighingComp.PLPosRelation != null)
                                compInfo.PLPosRelation = weighingComp.PLPosRelation.ProdOrderPartslistPosRelationID;

                            if (weighingComp.PickingPosition != null)
                                compInfo.PickingPos = weighingComp.PickingPosition.PickingPosID;

                            compInfo.WeighingComponentState = (short)weighingComp.WeighState;
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
            using (ACMonitor.Lock(_65050_WeighingCompLock))
            {
                if (WeighingComponents == null)
                    return null;

                return WeighingComponents.FirstOrDefault(c => c.PLPosRelation.ProdOrderPartslistPosRelationID == currentOpenMaterial);
            }
        }

        public WeighingComponent GetWeighingComponentPicking(Guid? currentOpenMaterial)
        {
            using (ACMonitor.Lock(_65050_WeighingCompLock))
            {
                if (WeighingComponents == null)
                    return null;

                return WeighingComponents.FirstOrDefault(c => c.PickingPosition.PickingPosID == currentOpenMaterial);
            }
        }

        public WeighingComponent GetWeighingComponentByMaterial(Guid? materialID)
        {
            using (ACMonitor.Lock(_65050_WeighingCompLock))
            {
                if (WeighingComponents == null)
                    return null;

                return WeighingComponents.FirstOrDefault(c => c.Material == materialID);
            }
        }

        [ACMethodInfo("", "", 9999)]
        public void CheckIsBinChangeLoopNodeAvailable()
        {
            if (PWPointOut == null || PWPointOut.ConnectionList == null || !PWPointOut.ConnectionList.Any())
            {
                IsBinChangeLoopNodeAvailable.ValueT = false;
                return;
            }

            IsBinChangeLoopNodeAvailable.ValueT = PWPointOut.ConnectionList.Any(c => c.ValueT is PWBinChangeLoop);
        }

        [ACMethodInfo("", "", 9999)]
        public double? InterdischargingStart()
        {
            string returnValue = null;

            using (ACMonitor.Lock(_65025_MemberCompLock))
            {
                returnValue = InterdischargingScaleActualValue.ValueT;
            }

            double parsedValue = 0;
            if (!string.IsNullOrEmpty(returnValue) && double.TryParse(returnValue, out parsedValue))
            {
                return parsedValue;
            }

            PAFManualWeighing manWeigh = CurrentExecutingFunction<PAFManualWeighing>();
            if (manWeigh != null)
            {
                var scale = manWeigh.CurrentScaleForWeighing;
                if (scale != null)
                {
                    returnValue = scale.ActualValue.ValueT.ToString();
                    double actualWeight = scale.ActualWeight.ValueT;
                    using (ACMonitor.Lock(_65025_MemberCompLock))
                    {
                        InterdischargingScaleActualValue.ValueT = returnValue;
                        InterdischargingScaleActualWeight.ValueT = actualWeight;
                    }
                }
            }

            if (!string.IsNullOrEmpty(returnValue) && double.TryParse(returnValue, out parsedValue))
            {
                return parsedValue;
            }

            return null;
        }

        [ACMethodInfo("", "", 9999)]
        public void CompleteInterdischarging()
        {
            double compQuantity = 0;
            using (ACMonitor.Lock(_65025_MemberCompLock))
            {
                compQuantity = InterdischargingScaleActualWeight.ValueT;
            }

            CompleteWeighing(compQuantity, true);

            using (ACMonitor.Lock(_65025_MemberCompLock))
            {
                InterdischargingScaleActualValue.ValueT = null;
            }

            TareScale();
        }

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);

            XmlElement xmlChild = xmlACPropertyList[nameof(InterdischargingScaleActualValue)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(InterdischargingScaleActualValue));
                if (xmlChild != null)
                    xmlChild.InnerText = InterdischargingScaleActualValue.ValueT;
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(InterdischargingScaleActualWeight)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(InterdischargingScaleActualWeight));
                if (xmlChild != null)
                    xmlChild.InnerText = InterdischargingScaleActualWeight.ValueT.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(MinWeightQuantity)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(MinWeightQuantity));
                if (xmlChild != null)
                    xmlChild.InnerText = MinWeightQuantity.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(ComponentsSeqFrom)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(ComponentsSeqFrom));
                if (xmlChild != null)
                    xmlChild.InnerText = ComponentsSeqFrom.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(ComponentsSeqTo)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(ComponentsSeqTo));
                if (xmlChild != null)
                    xmlChild.InnerText = ComponentsSeqTo.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(ScaleAutoTare)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(ScaleAutoTare));
                if (xmlChild != null)
                    xmlChild.InnerText = ScaleAutoTare.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(AutoSelectLot)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(AutoSelectLot));
                if (xmlChild != null)
                    xmlChild.InnerText = AutoSelectLot.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(AutoSelectLotPriority)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(AutoSelectLotPriority));
                if (xmlChild != null)
                    xmlChild.InnerText = AutoSelectLotPriority.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(LotValidation)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(LotValidation));
                if (xmlChild != null)
                    xmlChild.InnerText = LotValidation.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(AutoInsertQuantToStore)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(AutoInsertQuantToStore));
                if (xmlChild != null)
                    xmlChild.InnerText = AutoInsertQuantToStore.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(IncludeContainerStores)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(IncludeContainerStores));
                if (xmlChild != null)
                    xmlChild.InnerText = IncludeContainerStores.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(CurrentEndBatchPosKey)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(CurrentEndBatchPosKey));
                if (xmlChild != null)
                    xmlChild.InnerText = CurrentEndBatchPosKey?.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(_LastOpenMaterial)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(_LastOpenMaterial));
                if (xmlChild != null)
                    xmlChild.InnerText = _LastOpenMaterial?.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(CurrentOpenMaterial)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(CurrentOpenMaterial));
                if (xmlChild != null)
                    xmlChild.InnerText = CurrentOpenMaterial?.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(CurrentFacilityCharge)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(CurrentFacilityCharge));
                if (xmlChild != null)
                    xmlChild.InnerText = CurrentFacilityCharge?.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(HasAnyMaterialToProcess)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(HasAnyMaterialToProcess));
                if (xmlChild != null)
                    xmlChild.InnerText = HasAnyMaterialToProcess.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(ManualWeighingNextTask)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(ManualWeighingNextTask));
                if (xmlChild != null)
                    xmlChild.InnerText = ManualWeighingNextTask.ValueT.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }
        }

        [ACMethodInfo("", "", 9999)]
        public string GetPWParametersInfo()
        {
            var configStores = MandatoryConfigStores?.ToArray();
            if (configStores != null)
            {
                string preConfigACUrl = RootPW.PreValueACUrl;
                return ConfigManagerIPlus.GetParametersInfo(configStores, preConfigACUrl);
            }

            return null;
        }

        private void RefreshActiveNodesOnScaleComponent()
        {
            var root = ParentPWGroup?.RootPW;

            var activeManualWeighings = root.FindChildComponents<PWManualWeighing>(c => c is PWManualWeighing && c != this)
                                           .Where(x => x.CurrentACState == ACStateEnum.SMRunning || x.CurrentACState == ACStateEnum.SMPaused);

            if (activeManualWeighings != null && activeManualWeighings.Any())
            {
                foreach (PWManualWeighing manWeigh in activeManualWeighings)
                {
                    manWeigh.RefreshCompWeighingList();

                    PAFManualWeighing manualWeighing = manWeigh.CurrentExecutingFunction<PAFManualWeighing>();

                    if (manualWeighing != null)
                    {
                        WeighingComponent wcomp = manWeigh.GetWeighingComponent(manWeigh.CurrentOpenMaterial);
                        if (wcomp != null)
                        {
                            manualWeighing.CurrentACMethod.ValueT.ParameterValueList["TargetQuantity"] = wcomp.TargetWeight;
                            manualWeighing.ReSendACMethod(manualWeighing.CurrentACMethod.ValueT);
                        }
                    }
                }
            }
        }

        private Guid? IsCurrentFacilityChargeCorrect(Guid? currentFc, Guid? currentOpenMaterial, ACMethod pafACMethod)
        {
            if (currentFc == null || currentOpenMaterial == null || pafACMethod == null)
                return null;

            try
            {
                Guid? fcFromMethod = null;
                ACValue fcFromMethodVal = pafACMethod.ParameterValueList?.GetACValue(nameof(FacilityCharge));
                if (fcFromMethodVal != null && fcFromMethodVal.Value is Guid)
                    fcFromMethod = fcFromMethodVal.ParamAsGuid;

                if (fcFromMethod.HasValue && fcFromMethod.Value != currentFc)
                {
                    Messages.LogError(this.GetACUrl(), nameof(IsCurrentFacilityChargeCorrect) + "(10)", String.Format("CurrentFacilityCharge: {0} is different than from method: {1}", currentFc, fcFromMethod.Value));

                    using (DatabaseApp dbApp = new DatabaseApp())
                    {
                        var fcFromPAF = dbApp.FacilityCharge.Include(c => c.Material).FirstOrDefault(c => c.FacilityChargeID == fcFromMethod.Value);
                        var fcFromPW = dbApp.FacilityCharge.Include(c => c.Material).FirstOrDefault(c => c.FacilityChargeID == currentFc);

                        ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation.Include(c => c.SourceProdOrderPartslistPos).FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == currentOpenMaterial);
                        if (rel != null)
                        {
                            if (rel.SourceProdOrderPartslistPos.MaterialID == fcFromPAF.MaterialID && rel.SourceProdOrderPartslistPos.MaterialID != fcFromPW.MaterialID)
                            {
                                Messages.LogError(this.GetACUrl(), nameof(IsCurrentFacilityChargeCorrect) + "(20)", String.Format("FacilityCharge is sync from method: {0}", fcFromMethod.Value));
                                return fcFromMethod.Value;
                            }
                            else if (rel.SourceProdOrderPartslistPos.MaterialID == currentFc)
                            {
                                Messages.LogError(this.GetACUrl(), nameof(IsCurrentFacilityChargeCorrect) + "(30)", String.Format("FacilityCharge from method is wrong: {0}, CurrentFacilityCharge: {1}", fcFromMethod.Value, currentFc));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), nameof(IsCurrentFacilityChargeCorrect) + "(40)", e);
            }

            return null;
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
                case nameof(StartWeighing):
                    result = StartWeighing(acParameter[0] as Guid?, acParameter[1] as Guid?, acParameter[2] as Guid?, (bool)acParameter[3]);
                    return true;
                case nameof(CompleteWeighing):
                    CompleteWeighing((double)acParameter[0], (bool)acParameter[1]);
                    return true;
                case nameof(TareScale):
                    TareScale();
                    return true;
                case nameof(LotChange):
                    LotChange(acParameter[0] as Guid?, (double)acParameter[1], (bool)acParameter[2], (bool)acParameter[3]);
                    return true;
                case nameof(BinChange):
                    BinChange();
                    return true;
                case nameof(Abort):
                    if (acParameter.Count() > 1)
                        Abort((bool)acParameter[0], (bool)acParameter[1]);
                    else
                        Abort((bool)acParameter[0]);
                    return true;
                case nameof(OnApplyManuallyEnteredLot):
                    result = OnApplyManuallyEnteredLot(acParameter[0] as string, (Guid)acParameter[1]);
                    return true;
                case nameof(GetAvailableFacilityCharges):
                    result = GetAvailableFacilityCharges((Guid)acParameter[0]);
                    return true;
                case nameof(GetRoutableFacilities):
                    result = GetRoutableFacilities((Guid)acParameter[0]);
                    return true;
                //case "GetAvailableFacilities":
                //    result = GetAvailableFacilities((Guid)acParameter[0]);
                //    return true;
                case nameof(SMResetting):
                    SMResetting();
                    return true;
                //case nameof(IsBinChangeLoopNodeAvailable):
                //    result = IsBinChangeLoopNodeAvailable();
                //    return true;
                case nameof(GetPWParametersInfo):
                    result = GetPWParametersInfo();
                    return true;
                case nameof(ActivateRework):
                    result = ActivateRework();
                    return true;
                case nameof(GetReworkStatus):
                    result = GetReworkStatus();
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
        public WeighingComponent(ProdOrderPartslistPosRelation posRelation, WeighingComponentState weighState)
        {
            PLPosRelation = posRelation;
            Material = posRelation.SourceProdOrderPartslistPos.MaterialID.Value;
            Sequence = posRelation.Sequence;
            WeighState = weighState;
            TargetWeight = Math.Abs(posRelation.RemainingDosingWeight);
            MaterialName = posRelation.SourceProdOrderPartslistPos.Material?.MaterialName1;

            ErrorInfoPartslistNo = posRelation.TargetProdOrderPartslistPos?.ProdOrderPartslist?.Partslist?.PartslistNo;
            ErrorInfoProgramNo = posRelation.TargetProdOrderPartslistPos?.ProdOrderPartslist?.ProdOrder?.ProgramNo;
            ErrorInfoBookingMaterialName = posRelation.TargetProdOrderPartslistPos?.BookingMaterial?.MaterialName1;
        }

        public WeighingComponent(PickingPos pickingPos, WeighingComponentState weighState)
        {
            PickingPosition = pickingPos;
            Material = pickingPos.Material.MaterialID;
            Sequence = pickingPos.Sequence;
            WeighState = weighState;
            TargetWeight = Math.Abs(pickingPos.RemainingDosingWeight);
            MaterialName = pickingPos.Material.MaterialName1;
        }

        public ProdOrderPartslistPosRelation PLPosRelation
        {
            get;
            set;
        }

        public PickingPos PickingPosition
        {
            get;
            set;
        }

        public Guid Material
        {
            get;
            set;
        }

        public double TargetWeight
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

        public WeighingComponentState WeighState
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
            WeighState = state;
        }

        public void RefreshComponent(ProdOrderPartslistPosRelation posRelation)
        {
            Material = posRelation.SourceProdOrderPartslistPos.MaterialID.Value;
            Sequence = posRelation.Sequence;
            TargetWeight = Math.Abs(posRelation.RemainingDosingWeight);
            MaterialName = posRelation.SourceProdOrderPartslistPos.Material?.MaterialName1;
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

        [DataMember(Name = "I")]
        public Guid PickingPos
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
            compInfo.PickingPos = this.PickingPos;
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
        SelectCompReturnFC_F = 40, //Automatic component select - manual facility charge or facility select
        RefreshCompTargetQ = 50,
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
