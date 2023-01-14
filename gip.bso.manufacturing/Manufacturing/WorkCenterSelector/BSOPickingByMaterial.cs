using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.processapplication;
using gip.core.reporthandler;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using static gip.core.datamodel.Global;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Picking by material'}de{'Kommissionierung nach Material'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, SortIndex = 600)]
    public class BSOPickingByMaterial : BSOManualWeighing
    {
        #region c'tors

        public BSOPickingByMaterial(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool result = base.ACInit(startChildMode);

            if (result)
            {
                PickingsFrom = DateTime.Now.Date;
                PickingsTo = DateTime.Now.AddDays(1).Date;
            }

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        private Type _PAFPickingByMaterialType = typeof(PAFPickingByMaterial);

        private ACRef<IACComponent> _PAFPickingByMaterial;

        private IACContainerTNet<ACStateEnum> _PAFACStateProp;

        public string PickingType
        {
            get;
            set;
        }

        public string SourceFacilityNo
        {
            get;
            set;
        }

        public DateTime PWPickingsFrom
        {
            get;
            set;
        }

        public DateTime PWPickingsTo
        {
            get;
            set;
        }


        [ACPropertyInfo(9999, "", "en{'From'}de{'From'}")]
        public DateTime PickingsFrom
        {
            get;
            set;
        }

        [ACPropertyInfo(9999, "", "en{'To'}de{'To'}")]
        public DateTime PickingsTo
        {
            get;
            set;
        }

        private Picking _CurrentPicking;
        [ACPropertySelected(9999, "Picking")]
        public Picking CurrentPicking
        {
            get => _CurrentPicking;
            set
            {
                _CurrentPicking = value;

                if (_CurrentPicking != null && SelectedWeighingMaterial != null)
                {
                    CurrentPickingPos = _CurrentPicking.PickingPos_Picking.FirstOrDefault(c => c.Material.MaterialID == SelectedWeighingMaterial.PickingPosition.Material.MaterialID);
                }

                ActivateWeighing();
                OnPropertyChanged();
            }
        }

        private List<Picking> _PickingItems;

        private List<Picking> _PickingsList;
        [ACPropertyList(9999, "Picking")]
        public List<Picking> PickingsList
        {
            get => _PickingsList;
            set
            {
                _PickingsList = value;
                OnPropertyChanged();
            }
        }

        private PickingPos _CurrentPickingPos;
        public PickingPos CurrentPickingPos
        {
            get => _CurrentPickingPos;
            set
            {
                _CurrentPickingPos = value;

                if (_CurrentPickingPos != null)
                {
                    _SelectedWeighingMaterial.TargetQuantity = _CurrentPickingPos.TargetQuantityUOM;
                    _SelectedWeighingMaterial.ActualQuantity = _CurrentPickingPos.ActualQuantityUOM;
                }
                OnPropertyChanged();
            }
        }

        public override FacilityChargeItem SelectedFacilityCharge 
        { 
            get => base.SelectedFacilityCharge; 
            set
            {
                _SelectedFacilityCharge = value;
                if (value != null)
                    ShowSelectFacilityLotInfo = false;

                OnPropertyChanged("SelectedFacilityCharge");

                if (value != null && InformUserWithMsgNegQuantStock
                        && (_SelFacilityCharge == null
                            || _SelFacilityCharge.FacilityChargeID != value.FacilityChargeID))
                {
                    CheckIsQuantStockNegAndInformUser(value);
                    _SelFacilityCharge = value;
                }

                ActivateWeighing();
            }
        }

        public override IEnumerable<FacilityChargeItem> FacilityChargeList
        {
            get
            {
                try
                {
                    IACComponentPWNode componentPWNode = ComponentPWNodeLocked;

                    if (_FacilityChargeList == null && SelectedWeighingMaterial != null)
                    {
                        ACValueList facilities = componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.GetRoutableFacilities),
                                                                                SelectedWeighingMaterial.PickingPosition.PickingPosID) as ACValueList;

                        var facilityIDs = facilities.Select(c => c.ParamAsGuid).ToArray();
                        if (facilityIDs == null)
                            return null;

                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            //var facilitesDB = dbApp.Facility.Include(i => i.FacilityCharge_Facility).Where(c => facilityIDs.Contains(c.FacilityID));

                            if (_ACFacilityManager == null)
                            {
                                _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
                                if (_ACFacilityManager == null)
                                {
                                    //Error50432: The facility manager is null.
                                    Messages.Error(this, "Error50432");
                                }
                            }

                            if (SelectedWeighingMaterial.PickingPosition != null)
                                _FacilityChargeList = ACFacilityManager?.ManualWeighingFacilityChargeListQuery(dbApp, facilityIDs, SelectedWeighingMaterial.PickingPosition.Material.MaterialID).Select(s => new FacilityChargeItem(s, TargetWeight)).ToArray();



                            //ACValueList facilityCharges = componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.GetAvailableFacilityCharges), 
                            //                                                             SelectedWeighingMaterial.PosRelation.ProdOrderPartslistPosRelationID) as ACValueList;
                            //if (facilityCharges == null)
                            //    return null;

                            //    _FacilityChargeList = facilityCharges.Select(c => new FacilityChargeItem(dbApp.FacilityCharge
                            //                                                                                    .Include(d => d.FacilityLot).Include(d => d.MDUnit).Include(d => d.Material).Include(d => d.Facility)
                            //                                                                                     .FirstOrDefault(x => x.FacilityChargeID == c.ParamAsGuid), TargetWeight))
                            //                                         .ToArray();

                            FacilityChargeListCount = _FacilityChargeList.Count();
                        }
                    }
                    return _FacilityChargeList;
                }
                catch (Exception e)
                {
                    string message = null;
                    if (e.InnerException != null)
                        message = string.Format("ManualWeighingModel(FacilityChargeList): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                    else
                        message = string.Format("ManualWeighingModel(FacilityChargeList): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);

                    Messages.Error(this, message, true);
                }
                return null;
            }
        }

        public override WeighingMaterial SelectedWeighingMaterial 
        { 
            get => base.SelectedWeighingMaterial; 
            set
            {
                if (_SelectedWeighingMaterial != value)
                {

                    if (WeighingMaterialsFSM && _SelectedWeighingMaterial != null && _SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.Selected)
                    {
                        _SelectedWeighingMaterial.ChangeComponentState(WeighingComponentState.ReadyToWeighing, DatabaseApp);
                    }

                    _SelectedWeighingMaterial = value;
                    _FacilityChargeList = null;
                    FacilityChargeListCount = 0;
                    CurrentPicking = null;
                    CurrentPickingPos = null;
                    OnPropertyChanged(nameof(SelectedWeighingMaterial));
                    OnPropertyChanged(nameof(FacilityChargeList));
                    FacilityChargeNo = null;
                    ScaleAddAcutalWeight = _PAFManuallyAddedQuantity != null ? _PAFManuallyAddedQuantity.ValueT : 0;
                    SelectedFacilityCharge = null;
                    if (_SelectedWeighingMaterial != null)
                        ShowSelectFacilityLotInfo = true;

                    if (WeighingMaterialsFSM && _SelectedWeighingMaterial != null && _SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.ReadyToWeighing)
                    {
                        //_StartWeighingFromF_FC = true;
                        _SelectedWeighingMaterial.ChangeComponentState(WeighingComponentState.Selected, DatabaseApp);

                        PickingsList = _PickingItems.Where(c => c.PickingPos_Picking.Any(x => x.Material.MaterialID == _SelectedWeighingMaterial.PickingPosition.Material.MaterialID)).ToList();
                        var temp = PickingsList.SelectMany(c => c.PickingPos_Picking)
                                                     .Where(c => c.Material.MaterialID == _SelectedWeighingMaterial.PickingPosition.Material.MaterialID)
                                                     .FirstOrDefault(x => x.MDDelivPosLoadState.MDDelivPosLoadStateIndex < (short)MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck);

                        CurrentPicking = temp?.Picking;
                    }
                }
            }
        }

        public override ACComponent CurrentProcessModule 
        {
            get;
            protected set;
        }

        #endregion

        #region Methods

        public override void Activate(ACComponent selectedProcessModule)
        {
            base.Activate(selectedProcessModule);

            ACComponent currentProcessModule = CurrentProcessModule;
            if (currentProcessModule == null)
            {
                //Error50283: The manual weighing module can not be initialized. The property CurrentProcessModule is null.
                // Die Handverwiegungsstation konnte nicht initialisiert werden. Die Eigenschaft CurrentProcessModule ist null.
                Messages.Error(this, "Error50283");
                return;
            }

            //PAProcessModuleACUrl = currentProcessModule.ACUrl;
            //PAProcessModuleACCaption = currentProcessModule.ACCaption;

            if (currentProcessModule.ConnectionState == ACObjectConnectionState.DisConnected)
            {
                //Info50040: The server is unreachable. Reopen the program once the connection to the server has been established.
                // Der Server ist nicht erreichbar. Öffnen Sie das Programm erneut sobal die Verbindung zum Server wiederhergestellt wurde.
                //Messages.Info(this, "Info50040");
                return;
            }

            var processModuleChildComps = currentProcessModule.ACComponentChildsOnServer;
            IACComponent pafPickingByMaterial = processModuleChildComps.FirstOrDefault(c => _PAFPickingByMaterialType.IsAssignableFrom(c.ComponentClass.ObjectType));

            if (pafPickingByMaterial == null)
            {
                //Error50286: The manual weighing component can not be initialized. The process module {0} has not a child component of type PAFManualWeighing.
                // Die Verwiegekomponente konnte nicht initialisiert werden. Das Prozessmodul {0} hat keine Kindkomponente vom Typ PAFManualWeighing.
                //Messages.Info(this, "Error50286", false, PAProcessModuleACUrl); TODO
                return;
            }

            _PAFPickingByMaterial = new ACRef<IACComponent>(pafPickingByMaterial, this);

            IEnumerable<IACComponent> scaleObjects = processModuleChildComps.Where(c => typeof(PAEScaleBase).IsAssignableFrom(c.ComponentClass.ObjectType)).ToArray();
            if (scaleObjects != null && scaleObjects.Any())
            {
                _ProcessModuleScales = scaleObjects.Select(c => new ACRef<IACComponent>(c, this)).ToArray();
                ActivateScale(scaleObjects.FirstOrDefault());

                var scaleObjectInfoList = new List<ACValueItem>(_ProcessModuleScales.Select(c => new ACValueItem(c.ValueT.ACCaption, c.ACUrl, null)));
                ScaleObjectsList = scaleObjectInfoList;
            }

            var pafACState = pafPickingByMaterial.GetPropertyNet(nameof(ACState));
            if (pafACState == null)
            {
                //todo: error
                return;
            }

            _PAFACStateProp = pafACState as IACContainerTNet<ACStateEnum>;

            HandlePAFACState(_PAFACStateProp.ValueT);

            _PAFACStateProp.PropertyChanged += _PAFACStateProp_PropertyChanged;

            WeighingMaterialsFSM = true;
        }

        public override void DeActivate()
        {
            CurrentProcessModule = null;
            CurrentPicking = null;
            CurrentPickingPos = null;
            PickingsList = null;
            SelectedWeighingMaterial = null;
            WeighingMaterialList = null;
            _FacilityChargeList = null;

            if (ComponentPWNode != null)
            {
                ComponentPWNode.Detach();
                ComponentPWNode = null;
            }

            if (_PAFACStateProp != null)
            {
                _PAFACStateProp.PropertyChanged -= _PAFACStateProp_PropertyChanged;
                _PAFACStateProp = null;
            }

            if (_PAFPickingByMaterial != null)
            {
                _PAFPickingByMaterial.Detach();
                _PAFPickingByMaterial = null;
            }

            if (_ProcessModuleScales != null && _ProcessModuleScales.Any())
            {
                foreach (var scale in _ProcessModuleScales)
                {
                    scale.Detach();
                }
                _ProcessModuleScales = null;
            }

            base.DeActivate();
        }

        private void _PAFACStateProp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                var tempSender = sender as IACContainerTNet<ACStateEnum>;
                if (tempSender != null)
                {
                    ACStateEnum tempState = tempSender.ValueT;
                    ParentBSOWCS?.ApplicationQueue.Add(() => HandlePAFACState(tempState));
                }
            }
        }

        private void HandlePAFACState(ACStateEnum acState)
        {
            if (acState == ACStateEnum.SMRunning)
            {
                string wfNodeACUrl = _PAFPickingByMaterial.ValueT.ExecuteMethod(nameof(PAFPickingByMaterial.GetCurrentTaskACUrl)) as string;
                if (!string.IsNullOrEmpty(wfNodeACUrl))
                {
                    var pwNode = Root.ACUrlCommand(wfNodeACUrl) as IACComponentPWNode;
                    if (pwNode == null)
                    {
                        pwNode = Root.ACUrlCommand(wfNodeACUrl) as IACComponentPWNode;
                    }

                    if (pwNode != null)
                    {
                        ComponentPWNode = new ACRef<IACComponentPWNode>(pwNode, this);

                        ACMethod acMethod = pwNode?.ACUrlCommand(nameof(PWPickingByMaterial.MyConfiguration)) as ACMethod;
                        if (acMethod == null)
                        {
                            //Error50288: The configuration(ACMethod) for the workflow node cannot be found!
                            // Die Konfiguration (ACMethod) für den Workflow-Knoten kann nicht gefunden werden!
                            Messages.Error(this, "Error50288");
                            return;
                        }

                        ACValue pickingTypeACValue = acMethod.ParameterValueList.GetACValue("PickingType");
                        if (pickingTypeACValue != null)
                            PickingType = pickingTypeACValue.ParamAsString;

                        ACValue sourceFacilityNoACValue = acMethod.ParameterValueList.GetACValue("SourceFacilityNo");
                        if (sourceFacilityNoACValue != null)
                            SourceFacilityNo = sourceFacilityNoACValue.ParamAsString;

                        ACValue fromDTACValue = acMethod.ParameterValueList.GetACValue("FromDT");
                        if (fromDTACValue != null)
                            PWPickingsFrom = fromDTACValue.ParamAsDateTime;

                        ACValue toDTACValue = acMethod.ParameterValueList.GetACValue("ToDT");
                        if (toDTACValue != null)
                            PWPickingsTo = toDTACValue.ParamAsDateTime;

                        GenerateWeighingModel(PickingType, SourceFacilityNo, PWPickingsFrom, PWPickingsTo);

                    }
                }
            }
            else if (acState == ACStateEnum.SMResetting)
            {
                PickingType = null;
                SourceFacilityNo = null;
                PWPickingsFrom = DateTime.MinValue;
                PWPickingsTo = DateTime.MinValue;

                WeighingMaterialList = null;
                PickingsList = null;
                _PickingItems = null;
                CurrentPickingPos = null;
                _FacilityChargeList = null;
                SelectedWeighingMaterial = null;

                if (ComponentPWNode != null)
                {
                    ComponentPWNode.Detach();
                    ComponentPWNode = null;
                }
            }
        }

        private void GenerateWeighingModel(string pickingType, string sourceFacilityNo, DateTime from, DateTime to)
        {
            Guid? pickingTypeID = DatabaseApp.MDPickingType.FirstOrDefault(c => c.MDKey == pickingType)?.MDPickingTypeID;
            if (!pickingTypeID.HasValue)
            {
                //todo: error
                return;
            }

            Guid? sourceFacilityID = DatabaseApp.Facility.FirstOrDefault(c => c.FacilityNo == sourceFacilityNo)?.FacilityID;
            if (!sourceFacilityID.HasValue)
            {
                //todo error
                return;
            }


            _PickingItems = DatabaseApp.Picking.Where(c => c.DeliveryDateFrom >= from 
                                                       && c.DeliveryDateTo <= to
                                                       && c.MDPickingTypeID == pickingTypeID.Value
                                                       && (c.PickingStateIndex == (short)PickingStateEnum.New || c.PickingStateIndex == (short)PickingStateEnum.InProcess)
                                                       && (c.PickingPos_Picking.Any(x => x.FromFacilityID == sourceFacilityID || (x.FromFacility.ParentFacilityID.HasValue && x.FromFacility.ParentFacilityID == sourceFacilityID))))
                                              .ToList();

            WeighingMaterialList = _PickingItems.SelectMany(c => c.PickingPos_Picking)
                                           .GroupBy(c => c.Material.MaterialID)
                                           .Select(p => new WeighingMaterial(p.FirstOrDefault(), WeighingComponentState.ReadyToWeighing, DefaultMaterialIcon)).ToList();

        }

        private void ActivateWeighing()
        {
            if (SelectedWeighingMaterial != null && SelectedFacilityCharge != null && CurrentPicking != null && CurrentPickingPos != null)
            {
                PAFCurrentMaterial = SelectedWeighingMaterial.MaterialName;
                TargetWeight = SelectedWeighingMaterial.TargetQuantity;
            }
            else
            {
                PAFCurrentMaterial = null;
                TargetWeight = 0;
            }
        }

        private void BookPickingPosition()
        {
            FacilityCharge fc = DatabaseApp.FacilityCharge.Include("Facility").FirstOrDefault(c => c.FacilityChargeID == SelectedFacilityCharge.FacilityChargeID);
            if (fc == null)
            {
                //todo:error
                return;
            }

            FacilityPreBooking preBooking = ACFacilityManager.NewFacilityPreBooking(DatabaseApp, CurrentPickingPos, ScaleActualWeight);

            Msg msg = DatabaseApp.ACSaveChangesWithRetry();

            if (msg != null)
            {
                Messages.Msg(msg);
            }
            else if (preBooking != null)
            {
                ACMethodBooking bookingMethod = preBooking.ACMethodBooking as ACMethodBooking;
                bookingMethod.OutwardFacility = fc.Facility;
                bookingMethod.OutwardFacilityCharge = fc;
                ACMethodEventArgs result = ACFacilityManager.BookFacility(bookingMethod, DatabaseApp);


                if (!bookingMethod.ValidMessage.IsSucceded() || bookingMethod.ValidMessage.HasWarnings())
                    Messages.Msg(bookingMethod.ValidMessage);
                else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
                {
                    if (String.IsNullOrEmpty(result.ValidMessage.Message))
                        result.ValidMessage.Message = result.ResultState.ToString();
                    Messages.Msg(result.ValidMessage);
                }
                else
                {
                    double changedQuantity = 0;
                    FacilityCharge outwardFC = null;

                    if (bookingMethod != null)
                    {
                        if (bookingMethod.OutwardQuantity.HasValue)
                            changedQuantity = bookingMethod.OutwardQuantity.Value;
                        else if (bookingMethod.InwardQuantity.HasValue)
                            changedQuantity = bookingMethod.InwardQuantity.Value;

                        outwardFC = bookingMethod.OutwardFacilityCharge;
                    }

                    msg = preBooking.DeleteACObject(DatabaseApp, true);
                    if (msg != null)
                    {
                        Messages.Msg(msg);
                        return;
                    }

                    ACFacilityManager.RecalcAfterPosting(DatabaseApp, CurrentPickingPos, changedQuantity, false, true);
                    DatabaseApp.ACSaveChanges();

                    SelectedWeighingMaterial.RefreshFromPickingPos(CurrentPickingPos);

                    msg = ACFacilityManager.IsQuantStockConsumed(outwardFC, DatabaseApp);
                    if (msg != null)
                    {
                        if (Messages.Question(this, msg.Message, MsgResult.No, true) == MsgResult.Yes)
                        {
                            if (ACFacilityManager == null)
                                return;

                            ACMethodBooking fbtZeroBooking = ACPickingManager.BookParamZeroStockFacilityChargeClone(ACFacilityManager, DatabaseApp);
                            ACMethodBooking fbtZeroBookingClone = fbtZeroBooking.Clone() as ACMethodBooking;

                            fbtZeroBookingClone.InwardFacilityCharge = outwardFC;
                            fbtZeroBookingClone.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);

                            fbtZeroBookingClone.AutoRefresh = true;
                            ACMethodEventArgs resultZeroBook = ACFacilityManager.BookFacility(fbtZeroBookingClone, this.DatabaseApp);
                            if (!fbtZeroBookingClone.ValidMessage.IsSucceded() || fbtZeroBookingClone.ValidMessage.HasWarnings())
                            {
                                //return fbtZeroBooking.ValidMessage;
                            }
                            else if (resultZeroBook.ResultState == Global.ACMethodResultState.Failed || resultZeroBook.ResultState == Global.ACMethodResultState.Notpossible)
                            {
                                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                                    result.ValidMessage.Message = result.ResultState.ToString();

                                //return result.ValidMessage;
                            }
                        }
                    }
                }

            }
        }


        [ACMethodInfo("", "en{'Run pickings by material'}de{'Run pickings by material'}", 100, true)]
        public void RunPickingByMaterial()
        {
            if (PickingsFrom > DateTime.MinValue && PickingsTo > DateTime.MinValue && PickingsFrom < PickingsTo)
            {
                ACComponent currentProcessModule = CurrentProcessModule;
                if (currentProcessModule == null)
                {
                    //Error50283: The manual weighing module can not be initialized. The property CurrentProcessModule is null.
                    // Die Handverwiegungsstation konnte nicht initialisiert werden. Die Eigenschaft CurrentProcessModule ist null.
                    Messages.Error(this, "Error50283");
                    return;
                }


                if (ACFacilityManager == null)
                {
                    _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
                }

                _ACPickingManager = ACRefToPickingManager();

                ClearBookingData();

                using (Database db = new core.datamodel.Database())
                using(DatabaseApp dbApp = new DatabaseApp(db))
                {
                    if (_RoutingService == null)
                    {
                        _RoutingService = ACRoutingService.ACRefToServiceInstance(this);
                        if (_RoutingService == null)
                        {
                            //Error50430: The routing service is unavailable.
                            Messages.Error(this, "Error50430");
                            return;
                        }
                    }

                    if (!IsRoutingServiceAvailable)
                    {
                        //Error50430: The routing service is unavailable.
                        Messages.Error(this, "Error50430");
                        return;
                    }

                    RoutingResult rResult = ACRoutingService.FindSuccessors(RoutingService, db, true, currentProcessModule.ComponentClass, PAMParkingspace.SelRuleID_ParkingSpace,
                                                                        RouteDirections.Forwards, null, null, null, 0, true, true);

                    if (rResult == null || rResult.Routes == null)
                    {
                        //Error50431: Can not find any target storage for this station.
                        Messages.Error(this, "Error50431");
                        return;
                    }

                    var storageList = rResult.Routes.SelectMany(c => c.GetRouteTargets()).Select(x => x.Target);

                    if (storageList == null)
                    {
                        return;
                    }

                    var inwardFacilityACClass = storageList.FirstOrDefault(); 

                    Facility inwardFacility = dbApp.Facility.FirstOrDefault(c => c.VBiFacilityACClassID == inwardFacilityACClass.ACClassID);

                    if (inwardFacility == null)
                    {
                        //Error50434: Can not find any facility according target storage ID: {0}
                        Messages.Error(this, "Error50434", false, inwardFacilityACClass.ACClassID);
                        return;
                    }

                    Material material = dbApp.Material.FirstOrDefault(c => c.MDMaterialGroup.MDMaterialGroupIndex == (short)MDMaterialGroup.MaterialGroupTypes.Picking);
                    if (material == null)
                    {
                        //Error50436: The material with MaterialNo: {0} can not be found in database.
                        Messages.Error(this, "Error50436", false, "todo");
                        return;
                    }

                    var wfConfigs = material.MaterialConfig_Material.Where(c => c.KeyACUrl == MaterialConfig.PWMethodNodeConfigKeyACUrl);

                    if (wfConfigs == null || !wfConfigs.Any())
                    {
                        //Error50437: The single dosing workflow is not assigned to the material. Please assign single dosing workflow for this material in bussiness module Material. 
                        Messages.Error(this, "Error50437");
                        return;
                    }

                    var wfConfig = wfConfigs.FirstOrDefault(c => c.VBiACClassID == currentProcessModule.ComponentClass.ACClassID);
                    if (wfConfig == null)
                    {
                        wfConfig = wfConfigs.FirstOrDefault(c => !c.VBiACClassID.HasValue);
                    }

                    if (wfConfig == null)
                    {
                        //Error50438: The single dosing workflow is not assigned for this station. Please assign single dosing workflow for this station. 
                        Messages.Error(this, "Error50438");
                        return;
                    }

                    var workflow = wfConfig.VBiACClassWF.FromIPlusContext<core.datamodel.ACClassWF>(db);
                    var acClassMethod = workflow.ACClassMethod;

                    CurrentBookParamRelocation.InwardFacility = inwardFacility;
                    CurrentBookParamRelocation.OutwardFacility = inwardFacility;
                    CurrentBookParamRelocation.OutwardMaterial = material;
                    CurrentBookParamRelocation.InwardQuantity = 0.1;
                    CurrentBookParamRelocation.OutwardQuantity = 0.1;

                    RunWorkflow(dbApp, workflow, acClassMethod, currentProcessModule, false, true, PARole.ValidationBehaviour.Laxly);
                }
            }
        }

        public bool IsEnabledRunPickingByMaterial()
        {
            return CurrentProcessModule != null;
        }

        public override bool OnPreStartWorkflow(DatabaseApp dbApp, Picking picking, List<SingleDosingConfigItem> configItems, Route validRoute, core.datamodel.ACClassWF rootWF)
        {
            picking.DeliveryDateFrom = PickingsFrom;
            picking.DeliveryDateTo = PickingsTo;

            dbApp.ACSaveChanges();

            return base.OnPreStartWorkflow(dbApp, picking, configItems, validRoute, rootWF);
        }

        //private void ActivateScale(IACComponent scale)
        //{
        //    if (scale == null)
        //        return;

        //    var actWeightProp = scale.GetPropertyNet(nameof(PAEScaleBase.ActualWeight));
        //    if (actWeightProp == null)
        //    {
        //        //Error50292: Initialization error: The scale component doesn't have the property {0}.
        //        // Initialisierungsfehler: Die Waagen-Komponente besitzt nicht die Eigenschaft {0}.
        //        Messages.Error(this, "Error50292", false, "ActualWeight");
        //        return;
        //    }

        //    MaxScaleWeight = null;
        //    var actValProp = scale.GetPropertyNet(nameof(PAEScaleBase.ActualValue)) as IACContainerTNet<double>;
        //    if (actValProp == null)
        //    {
        //        //Error50292: Initialization error: The scale component doesn't have the property {0}.
        //        // Initialisierungsfehler: Die Waagen-Komponente besitzt nicht die Eigenschaft {0}.
        //        Messages.Error(this, "Error50292", false, "ActualValue");
        //        return;
        //    }

        //    double digitWeight = 1.0;
        //    var digitWeightProp = scale.GetPropertyNet(nameof(PAEScaleBase.DigitWeight));
        //    if (digitWeightProp != null)
        //    {
        //        digitWeight = (digitWeightProp as IACContainerTNet<double>).ValueT;
        //        if (digitWeight <= double.Epsilon)
        //            digitWeight = 1.0;
        //    }
        //    if (digitWeight >= 999.99999)
        //        ScalePrecisionFormat = "F0";
        //    else if (digitWeight >= 99.99999)
        //        ScalePrecisionFormat = "F1";
        //    else if (digitWeight >= 9.99999)
        //        ScalePrecisionFormat = "F2";
        //    else if (digitWeight >= 0.99999)
        //        ScalePrecisionFormat = "F3";
        //    else if (digitWeight >= 0.09999)
        //        ScalePrecisionFormat = "F4";
        //    else if (digitWeight >= 0.00999)
        //        ScalePrecisionFormat = "F5";
        //    else if (digitWeight >= 0.00099)
        //        ScalePrecisionFormat = "F6";

        //    _ScaleActualValue = actValProp;

        //    _ScaleActualWeight = actWeightProp as IACContainerTNet<double>;
        //    (_ScaleActualWeight as IACPropertyNetBase).PropertyChanged += ActWeightProp_PropertyChanged;
        //    (_ScaleActualValue as IACPropertyNetBase).PropertyChanged += ScaleActualValue_PropertyChanged;
        //    ScaleRealWeight = _ScaleActualWeight.ValueT;
        //    ScaleGrossWeight = _ScaleActualValue.ValueT;
        //    OnPropertyChanged("TargetWeight");
        //    OnPropertyChanged("ScaleDifferenceWeight");

        //   OnPropertyChanged(nameof(CurrentScaleObject));
        //}

        public override void Acknowledge()
        {
            BookPickingPosition();
        }

        public override bool IsEnabledAcknowledge()
        {
            return SelectedWeighingMaterial != null && SelectedFacilityCharge != null && CurrentPickingPos != null && CurrentPicking != null && ScaleActualWeight > 0.000001;
        }

        public override void AddKg()
        {
            ScaleAddAcutalWeight = SelectedWeighingMaterial.AddKg(ScaleAddAcutalWeight);
        }

        public override void RemoveKg()
        {
            ScaleAddAcutalWeight = SelectedWeighingMaterial.RemoveKg(ScaleAddAcutalWeight);
        }

        public override void Tare()
        {
            ACValueItem currentScaleObject = CurrentScaleObject;
            if (currentScaleObject != null)
            {
                ACRef<IACComponent> scaleRef = _ProcessModuleScales?.FirstOrDefault(c => c.ACUrl == _CurrentScaleObject.Value.ToString());
                if (scaleRef != null)
                {
                    scaleRef.ValueT.ExecuteMethod(nameof(PAEScaleGravimetric.Tare));
                }
            }
        }

        public override void Abort()
        {
            if (_PAFPickingByMaterial != null)
            {
                _PAFPickingByMaterial.ValueT.ExecuteMethod(nameof(PAFPickingByMaterial.Abort));
            }
        }

        public override void PrintLastQuant()
        {
            var currentProcessModule = CurrentProcessModule;

            if (CurrentPicking != null && CurrentPickingPos != null && currentProcessModule != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Add(nameof(Picking), CurrentPicking.PickingID);
                info.Add(nameof(PickingPos), CurrentPickingPos.PickingPosID);
                info.Add(nameof(core.datamodel.ACClass), currentProcessModule.ComponentClass.ACClassID);

                ACPrintManager printManger = ACPrintManager.GetServiceInstance(this);
                if (printManger != null)
                {
                    Msg msg = printManger.Print(info, 1);
                }
            }
        }

        #endregion
    }
}
