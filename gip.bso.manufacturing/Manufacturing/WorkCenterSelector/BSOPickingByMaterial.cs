using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.processapplication;
using gip.core.reporthandler;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
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
                PickingsFrom = DateTime.Now.AddDays(1).Date;
                PickingsTo = PickingsFrom;
            }

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _ACPickingManager = ACRefToPickingManager();

            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_ACPickingManager != null)
            {
                _ACPickingManager.Detach();
                _ACPickingManager = null;
            }

            if (_ACFacilityManager != null)
            {
                _ACFacilityManager.Detach();
                _ACFacilityManager = null;
            }

            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        private Type _PAFPickingByMaterialType = typeof(PAFPickingByMaterial);

        private ACRef<IACComponent> _PAFPickingByMaterial;

        private IACContainerTNet<ACStateEnum> _PAFACStateProp;

        private IACContainerTNet<Guid> _ScannedFCProp;

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

        private PickingPos _CurrentPicking;
        [ACPropertySelected(9999, "Picking")]
        public PickingPos CurrentPicking
        {
            get => _CurrentPicking;
            set
            {
                _CurrentPicking = value;

                if (_CurrentPicking != null && SelectedWeighingMaterial != null)
                {
                    _SelectedWeighingMaterial.TargetQuantity = _CurrentPicking.TargetQuantityUOM;
                    _SelectedWeighingMaterial.ActualQuantity = _CurrentPicking.ActualQuantityUOM;
                }

                ActivateWeighing();
                OnPropertyChanged();
            }
        }

        private List<Picking> _PickingItems;

        private List<PickingPos> _PickingsList;
        [ACPropertyList(9999, "Picking")]
        public List<PickingPos> PickingsList
        {
            get => _PickingsList;
            set
            {
                _PickingsList = value;
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

                OnPropertyChanged();

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

        protected override ObservableCollection<FacilityChargeItem> FillFacilityChargeList()
        {
            try
            {
                IACComponentPWNode componentPWNode = ComponentPWNodeLocked;

                if (_FacilityChargeList == null && SelectedWeighingMaterial != null)
                {
                    ACValueList facilities = componentPWNode?.ExecuteMethod(nameof(PWPickingByMaterial.GetRoutableFacilities),
                                                                            SelectedWeighingMaterial.PickingPosition.PickingPosID) as ACValueList;

                    var facilityIDs = facilities.Select(c => c.ParamAsGuid).ToArray();
                    if (facilityIDs == null)
                        return null;

                    using (DatabaseApp dbApp = new DatabaseApp())
                    {
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
                            _FacilityChargeList = new ObservableCollection<FacilityChargeItem>(ACFacilityManager?.ManualWeighingFacilityChargeListQuery(dbApp, facilityIDs, SelectedWeighingMaterial.PickingPosition.Material.MaterialID)
                                                                                                                 .Select(s => new FacilityChargeItem(s, TargetWeight)));

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
                    FacilityChargeList = null;
                    FacilityChargeListCount = 0;
                    CurrentPicking = null;
                    OnPropertyChanged(nameof(SelectedWeighingMaterial));
                    FacilityChargeList = FillFacilityChargeList();
                    FacilityChargeNo = null;
                    ScaleAddActualWeight = _PAFManuallyAddedQuantity != null ? _PAFManuallyAddedQuantity.ValueT : 0;
                    SelectedFacilityCharge = null;
                    if (_SelectedWeighingMaterial != null)
                        ShowSelectFacilityLotInfo = true;

                    if (WeighingMaterialsFSM && _SelectedWeighingMaterial != null && _SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.ReadyToWeighing)
                    {
                        //_StartWeighingFromF_FC = true;
                        _SelectedWeighingMaterial.ChangeComponentState(WeighingComponentState.Selected, DatabaseApp);

                        var tempItems = _PickingItems.SelectMany(c => c.PickingPos_Picking).Where(c => c.Material.MaterialID == _SelectedWeighingMaterial.PickingPosition.Material.MaterialID)
                                                    .OrderBy(c => c.Picking.PickingNo)
                                                    .ToList();

                        if (tempItems != null)
                        {
                            foreach (var tempItem in tempItems)
                            {
                                try
                                {
                                    tempItem.AutoRefresh();
                                }
                                catch
                                {
                                }
                            }

                        }

                        PickingsList = tempItems;
                    }
                }
            }
        }

        public override ACComponent CurrentProcessModule 
        {
            get;
            protected set;
        }

        public bool AutoPrintOnPosting
        {
            get;
            set;
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
                //Info50040: The server is unreachable.Reopen the program once the connection to the server has been established.
                //     Der Server ist nicht erreichbar.Öffnen Sie das Programm erneut sobal die Verbindung zum Server wiederhergestellt wurde.
                Messages.Info(this, "Info50040");
                return;
            }

            var processModuleChildComps = currentProcessModule.ACComponentChildsOnServer;
            IACComponent pafPickingByMaterial = processModuleChildComps.FirstOrDefault(c => _PAFPickingByMaterialType.IsAssignableFrom(c.ComponentClass.ObjectType));

            if (pafPickingByMaterial == null)
            {
                //Error50286: The manual weighing component can not be initialized. The process module {0} has not a child component of type PAFManualWeighing.
                // Die Verwiegekomponente konnte nicht initialisiert werden. Das Prozessmodul {0} hat keine Kindkomponente vom Typ PAFManualWeighing.
                Messages.Info(this, "Error50286", false, PAProcessModuleACUrl);
                return;
            }

            _PAFPickingByMaterial = new ACRef<IACComponent>(pafPickingByMaterial, this);

            IEnumerable<IACComponent> scaleObjects = processModuleChildComps.Where(c => typeof(PAEScaleBase).IsAssignableFrom(c.ComponentClass.ObjectType)).ToArray();
            if (scaleObjects != null && scaleObjects.Any())
            {
                _ProcessModuleScales = scaleObjects.Select(c => new ACRef<IACComponent>(c, this)).ToArray();
                ActivateScale(scaleObjects.FirstOrDefault());

                var scaleObjectInfoList = new ObservableCollection<ACValueItem>(_ProcessModuleScales.Select(c => new ACValueItem(c.ValueT.ACCaption, c.ACUrl, null)));
                ScaleObjectsList = scaleObjectInfoList;
                CurrentScaleObject = ScaleObjectsList.FirstOrDefault();
            }

            var pafACState = pafPickingByMaterial.GetPropertyNet(nameof(ACState));
            if (pafACState == null)
            {
                Messages.Error(this, "50285", false, nameof(ACState));
                return;
            }

            _PAFACStateProp = pafACState as IACContainerTNet<ACStateEnum>;
            HandlePAFACState(_PAFACStateProp.ValueT);
            _PAFACStateProp.PropertyChanged += _PAFACStateProp_PropertyChanged;


            var scannedFC = pafPickingByMaterial.GetPropertyNet(nameof(PAFPickingByMaterial.ScannedFacilityCharge));
            if (scannedFC != null)
            {
                _ScannedFCProp = scannedFC as IACContainerTNet<Guid>;
                _ScannedFCProp.PropertyChanged += _ScannedFCProp_PropertyChanged;
            }

            WeighingMaterialsFSM = true;
        }

        public override void DeActivate()
        {
            CurrentProcessModule = null;
            CurrentPicking = null;
            PickingsList = null;
            SelectedWeighingMaterial = null;
            WeighingMaterialList = null;
            FacilityChargeList = null;

            if (ComponentPWNode != null)
            {
                ComponentPWNode.Detach();
                ComponentPWNode = null;
            }

            if (_ScannedFCProp != null)
            {
                _ScannedFCProp.PropertyChanged -= _ScannedFCProp_PropertyChanged;
                _ScannedFCProp = null;
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

        private void _ScannedFCProp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                var tempSender = sender as IACContainerTNet<Guid>;
                if (tempSender != null)
                {
                    Guid tempGuid = tempSender.ValueT;
                    ParentBSOWCS?.ApplicationQueue.Add(() => HandleScannedFC(tempGuid));
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

                        string pickingType = null, sourceFacilityNo = null, sourceFacilityNo2 = null;

                        ACValue pickingTypeACValue = acMethod.ParameterValueList.GetACValue("PickingType");
                        if (pickingTypeACValue != null)
                            pickingType = pickingTypeACValue.ParamAsString;

                        ACValue sourceFacilityNoACValue = acMethod.ParameterValueList.GetACValue("SourceFacilityNo");
                        if (sourceFacilityNoACValue != null)
                            sourceFacilityNo = sourceFacilityNoACValue.ParamAsString;

                        ACValue sourceFacilityNo2ACValue = acMethod.ParameterValueList.GetACValue("SourceFacilityNo2");
                        if (sourceFacilityNo2ACValue != null)
                            sourceFacilityNo2 = sourceFacilityNo2ACValue.ParamAsString;

                        ACValue autoPrintACValue = acMethod.ParameterValueList.GetACValue("AutoPrintOnPosting");
                        if (autoPrintACValue != null)
                            AutoPrintOnPosting = autoPrintACValue.ParamAsBoolean;
                        else
                            AutoPrintOnPosting = false;

                        ACValue fromDTACValue = acMethod.ParameterValueList.GetACValue("FromDT");
                        if (fromDTACValue != null)
                            PWPickingsFrom = fromDTACValue.ParamAsDateTime;

                        ACValue toDTACValue = acMethod.ParameterValueList.GetACValue("ToDT");
                        if (toDTACValue != null)
                            PWPickingsTo = toDTACValue.ParamAsDateTime;

                        GenerateWeighingModel(pickingType, sourceFacilityNo, sourceFacilityNo2, PWPickingsFrom, PWPickingsTo);
                    }
                }
            }
            else if (acState == ACStateEnum.SMResetting)
            {
                PWPickingsFrom = DateTime.MinValue;
                PWPickingsTo = DateTime.MinValue;
                AutoPrintOnPosting = false;

                WeighingMaterialList = null;
                PickingsList = null;
                _PickingItems = null;
                CurrentPicking = null;
                FacilityChargeList = null;
                SelectedWeighingMaterial = null;

                if (ComponentPWNode != null)
                {
                    ComponentPWNode.Detach();
                    ComponentPWNode = null;
                }
            }
        }

        private void HandleScannedFC(Guid facilityChargeID)
        {
            if (facilityChargeID == Guid.Empty || (SelectedFacilityCharge != null && SelectedFacilityCharge.FacilityChargeID == facilityChargeID))
            {
                return;
            }

            var tempFCList = FacilityChargeList;
            if (tempFCList != null && tempFCList.Any())
            {
                var tempFC = tempFCList.FirstOrDefault(c => c.FacilityChargeID == facilityChargeID);
                if (tempFC != null)
                {
                    SelectedFacilityCharge = tempFC;
                }
            }

            var tempMaterialsList = WeighingMaterialList;
            if (tempMaterialsList != null && tempMaterialsList.Any())
            {
                FacilityCharge fcDB = DatabaseApp.FacilityCharge.Include(nameof(Material)).FirstOrDefault(c => c.FacilityChargeID == facilityChargeID);
                if (fcDB != null)
                {
                    var tempMaterial = tempMaterialsList.FirstOrDefault(c => c.MaterialNo == fcDB.Material.MaterialNo);
                    if (tempMaterial != null)
                    {
                        SelectedWeighingMaterial = tempMaterial;

                        tempFCList = FacilityChargeList;
                        if (tempFCList != null && tempFCList.Any())
                        {
                            var tempFC = tempFCList.FirstOrDefault(c => c.FacilityChargeID == facilityChargeID);
                            if (tempFC != null)
                            {
                               SelectedFacilityCharge = tempFC;
                            }
                        }
                    }
                }
            }
        }

        private void GenerateWeighingModel(string pickingType, string sourceFacilityNo, string sourceFacilityNo2, DateTime from, DateTime to)
        {
            Guid? pickingTypeID = DatabaseApp.MDPickingType.FirstOrDefault(c => c.MDKey == pickingType)?.MDPickingTypeID;
            if (!pickingTypeID.HasValue)
            {
                Messages.Error(this, "Can not find a MDPickingType with MDkey: " + pickingType);
                return;
            }

            Guid? sourceFacilityID = DatabaseApp.Facility.FirstOrDefault(c => c.FacilityNo == sourceFacilityNo)?.FacilityID;
            if (!sourceFacilityID.HasValue)
            {
                Messages.Error(this, "Can not find a Facility with FacilityNo: " + sourceFacilityNo);
                return;
            }

            Guid? sourceFacilityID2 = null;
            if (sourceFacilityNo2 != null)
            {
                sourceFacilityID2 = DatabaseApp.Facility.FirstOrDefault(c => c.FacilityNo == sourceFacilityNo2)?.FacilityID;
            }

            _PickingItems = DatabaseApp.Picking.Where(c => c.DeliveryDateFrom >= from 
                                                       && c.DeliveryDateTo <= to
                                                       && c.MDPickingTypeID == pickingTypeID.Value
                                                       && (c.PickingStateIndex == (short)PickingStateEnum.New || c.PickingStateIndex == (short)PickingStateEnum.InProcess)
                                                       && (c.PickingPos_Picking.Any(x => (x.FromFacilityID == sourceFacilityID || (x.FromFacility.ParentFacilityID.HasValue && x.FromFacility.ParentFacilityID == sourceFacilityID))
                                                                                      || (x.FromFacilityID == sourceFacilityID2 || (x.FromFacility.ParentFacilityID.HasValue && x.FromFacility.ParentFacilityID == sourceFacilityID2)))))
                                              .ToList();

            WeighingMaterialList = _PickingItems.SelectMany(c => c.PickingPos_Picking)
                                           .GroupBy(c => c.Material.MaterialID)
                                           .Select(p => new WeighingMaterial(p.FirstOrDefault(), WeighingComponentState.ReadyToWeighing, DefaultMaterialIcon))
                                           .OrderBy(x => x.MaterialName)
                                           .ToList();

        }

        private void ActivateWeighing()
        {
            if (SelectedWeighingMaterial != null && SelectedFacilityCharge != null && CurrentPicking != null)
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
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                FacilityCharge fc = dbApp.FacilityCharge.Include(nameof(Facility)).FirstOrDefault(c => c.FacilityChargeID == SelectedFacilityCharge.FacilityChargeID);
                if (fc == null)
                {
                    Messages.Error(this, "Can not find in the database a FacilityCharge with ID: " + SelectedFacilityCharge.FacilityChargeID);
                    return;
                }

                var currentPickingPos = CurrentPicking.FromAppContext<PickingPos>(dbApp);

                FacilityPreBooking preBooking = ACFacilityManager.NewFacilityPreBooking(dbApp, currentPickingPos, ScaleActualWeight);

                Msg msg = dbApp.ACSaveChanges();

                if (msg != null)
                {
                    //DatabaseApp.ACUndoChanges();
                    Messages.Msg(msg);
                }
                else if (preBooking != null)
                {
                    ACMethodBooking bookingMethod = preBooking.ACMethodBooking as ACMethodBooking;
                    bookingMethod.OutwardFacility = fc.Facility;
                    bookingMethod.OutwardFacilityCharge = fc;
                    ACMethodEventArgs result = ACFacilityManager.BookFacility(bookingMethod, dbApp);


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

                        msg = preBooking.DeleteACObject(dbApp, true);
                        if (msg != null)
                        {
                            Messages.Msg(msg);
                            return;
                        }

                        ACFacilityManager.RecalcAfterPosting(dbApp, currentPickingPos, changedQuantity, false, true);
                        msg = dbApp.ACSaveChanges();
                        if (msg != null)
                        {
                            //DatabaseApp.ACUndoChanges();
                            Messages.Msg(msg);
                        }

                        SelectedWeighingMaterial.RefreshFromPickingPos(currentPickingPos);

                        msg = ACFacilityManager.IsQuantStockConsumed(outwardFC, dbApp);
                        if (msg != null)
                        {
                            if (Messages.Question(this, msg.Message, MsgResult.No, true) == MsgResult.Yes)
                            {
                                if (ACFacilityManager != null && ACPickingManager != null)
                                {
                                    ACMethodBooking fbtZeroBooking = ACPickingManager.BookParamZeroStockFacilityChargeClone(ACFacilityManager, dbApp);
                                    ACMethodBooking fbtZeroBookingClone = fbtZeroBooking.Clone() as ACMethodBooking;

                                    fbtZeroBookingClone.InwardFacilityCharge = outwardFC;
                                    fbtZeroBookingClone.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(dbApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);

                                    fbtZeroBookingClone.AutoRefresh = true;
                                    ACMethodEventArgs resultZeroBook = ACFacilityManager.BookFacility(fbtZeroBookingClone, dbApp);
                                    if (!fbtZeroBookingClone.ValidMessage.IsSucceded() || fbtZeroBookingClone.ValidMessage.HasWarnings())
                                    {
                                        Messages.Msg(fbtZeroBooking.ValidMessage);
                                    }
                                    else if (resultZeroBook.ResultState == Global.ACMethodResultState.Failed || resultZeroBook.ResultState == Global.ACMethodResultState.Notpossible)
                                    {
                                        if (String.IsNullOrEmpty(result.ValidMessage.Message))
                                            result.ValidMessage.Message = result.ResultState.ToString();

                                        Messages.Msg(result.ValidMessage);
                                    }

                                    RefreshMaterialOrFC_F();
                                    ShowSelectFacilityLotInfo = true;
                                }
                            }
                        }

                        if (AutoPrintOnPosting)
                        {
                            PrintLastQuant();
                        }

                        try
                        {
                            CurrentPicking.AutoRefresh();
                            CurrentPicking.OnRefreshCompleteFactor();
                            CurrentPicking.OnEntityPropertyChanged(nameof(CurrentPicking.ActualQuantity));
                            if (SelectedFacilityCharge != null)
                                SelectedFacilityCharge.StockQuantityUOM = fc.StockQuantityUOM;
                        }
                        catch
                        {

                        }

                        CurrentPicking = null;
                    }
                }
            }
        }

        [ACMethodInfo("", "en{'Run pickings by material'}de{'Run pickings by material'}", 100, true)]
        public void RunPickingByMaterial()
        {
            if (_PAFACStateProp != null && _PAFACStateProp.ValueT != ACStateEnum.SMIdle)
            {
                //Error50595 :The function PickingByMaterial is currently active. Please perform abort on the function then try start again.
                Messages.Error(this, "Error50595");
                return;
            }

            if (PickingsFrom > DateTime.MinValue && PickingsTo > DateTime.MinValue)
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

        public override void Acknowledge()
        {
            BookPickingPosition();
        }

        public override bool IsEnabledAcknowledge()
        {
            return SelectedWeighingMaterial != null && SelectedFacilityCharge != null && CurrentPicking != null && Math.Abs(ScaleActualWeight) > 0.000001;
        }

        public override void AddKg()
        {
            ScaleAddActualWeight = SelectedWeighingMaterial.AddKg(ScaleAddActualWeight);
        }

        public override void RemoveKg()
        {
            ScaleAddActualWeight = SelectedWeighingMaterial.RemoveKg(ScaleAddActualWeight);
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
            base.Abort();
        }

        [ACMethodInfo("", "en{'Finish order'}de{'Finish order'}", 9999)]
        public void FinishPickingOrder()
        {
            // Question50096: Are you sure that you want complete weighing of all components?
            if (Messages.Question(this, "Question50096") != Global.MsgResult.Yes)
            {
                return;
            }

            BackgroundWorker.WorkerReportsProgress = true;
            BackgroundWorker.WorkerSupportsCancellation = false;
            BackgroundWorker.RunWorkerAsync(nameof(FinishPickingOrder));
            ShowDialog(this,DesignNameProgressBar);
            
            if (_PAFPickingByMaterial != null)
            {
                _PAFPickingByMaterial.ValueT.ExecuteMethod(nameof(PAFPickingByMaterial.Abort));
            }

            CloseTopDialog();
        }

        [ACMethodInfo("", "en{'Cancel component'}de{'Komponente abbrechen'}", 9999)]
        public void CancelCurrentComponent()
        {
            ShowSelectFacilityLotInfo = true;
            SelectedFacilityCharge = null;
            CloseTopDialog();
        }

        [ACMethodInfo("", "en{'Abort picking by material'}de{'Kommissionierung nach Material abbrechen'}", 9999)]
        public void AbortPickingByMaterial()
        {
            if (_PAFPickingByMaterial != null)
            {
                _PAFPickingByMaterial.ValueT.ExecuteMethod(nameof(PAFPickingByMaterial.Abort));
            }
            CloseTopDialog();
        }

        public override void PrintLastQuant()
        {
            var pickingByMat = _PAFPickingByMaterial?.ValueT;

            if (CurrentPicking != null && pickingByMat != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Add(nameof(Picking), CurrentPicking.Picking.PickingID);
                info.Add(nameof(PickingPos), CurrentPicking.PickingPosID);
                info.Add(nameof(core.datamodel.ACClass), pickingByMat.ComponentClass.ACClassID);

                Msg msg = pickingByMat.ExecuteMethod(nameof(PAFPickingByMaterial.PrintOverPAOrderInfo), info) as Msg;
                if (msg != null && msg.MessageLevel > eMsgLevel.Info)
                {
                    Messages.Msg(msg);
                }
            }
        }

        public override void RefreshMaterialOrFC_F()
        {
            base.RefreshMaterialOrFC_F();

            if (PickingsList == null)
                return;

            try
            {
                foreach (PickingPos tempItem in PickingsList)
                {
                    tempItem.AutoRefresh();
                }

                var tempList = PickingsList.ToList();
                PickingsList = tempList;
            }
            catch
            {
            }
        }

        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            if (e.Argument.ToString() == nameof(FinishPickingOrder))
            {
                ACRef<ACInDeliveryNoteManager> inManagerRef = ACInDeliveryNoteManager.ACRefToServiceInstance(this);
                ACRef<ACOutDeliveryNoteManager> outManagerRef = ACOutDeliveryNoteManager.ACRefToServiceInstance(this);

                DeliveryNote delNote = null;
                InOrder inOrder = null;
                OutOrder outOrder = null;

                if (ACPickingManager == null)
                {
                    _ACPickingManager = ACRefToPickingManager();
                }

                if (ACPickingManager == null)
                {
                    Messages.Error(this, "ACPickingManager is null!");
                    return;
                }

                CurrentProgressInfo.TotalProgress.ProgressRangeFrom = 0;
                CurrentProgressInfo.TotalProgress.ProgressRangeTo = _PickingItems.Count;

                foreach (var picking in _PickingItems)
                {
                    CurrentProgressInfo.TotalProgress.ProgressText = picking.PickingNo;
                    ACPickingManager.FinishOrder(DatabaseApp, picking, inManagerRef.ValueT, outManagerRef.ValueT, ACFacilityManager, out delNote, out inOrder, out outOrder, true);
                    
                    CurrentProgressInfo.TotalProgress.ProgressCurrent = CurrentProgressInfo.TotalProgress.ProgressCurrent + 1;
                }
            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseTopDialog();
        }

        #endregion
    }
}
