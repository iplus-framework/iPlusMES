using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.processapplication;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            return result;
        }

        #endregion


        #region Properties

        private Type _PAFPickingByMaterialType = typeof(PAFPickingByMaterial);

        ACRef<IACComponent>[] _ProcessModuleScales;

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
                OnPropertyChanged();
            }
        }


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
                OnPropertyChanged();
            }
        }

        public override FacilityChargeItem SelectedFacilityCharge 
        { 
            get => base.SelectedFacilityCharge; 
            set => base.SelectedFacilityCharge = value; 
        }

        public override WeighingMaterial SelectedWeighingMaterial 
        { 
            get => base.SelectedWeighingMaterial; 
            set => base.SelectedWeighingMaterial = value; 
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
            }

            var pafACState = pafPickingByMaterial.GetPropertyNet(nameof(ACState));
            if (pafACState == null)
            {
                //todo: error
                return;
            }

            _PAFACStateProp = pafACState as IACContainerTNet<ACStateEnum>;

            _PAFACStateProp.PropertyChanged += _PAFACStateProp_PropertyChanged;
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


                    }
                }
            }
            else if (acState == ACStateEnum.SMCompleted)
            {

            }
        }

        private void GenerateWeighingModel(string pickingType, string sourceFacilityNo)
        {



        }

        public override void DeActivate()
        {
            base.DeActivate();
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

        private void ActivateScale(IACComponent scale)
        {
            if (scale == null)
                return;

            var actWeightProp = scale.GetPropertyNet(nameof(PAEScaleBase.ActualWeight));
            if (actWeightProp == null)
            {
                //Error50292: Initialization error: The scale component doesn't have the property {0}.
                // Initialisierungsfehler: Die Waagen-Komponente besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50292", false, "ActualWeight");
                return;
            }

            MaxScaleWeight = null;
            var actValProp = scale.GetPropertyNet(nameof(PAEScaleBase.ActualValue)) as IACContainerTNet<double>;
            if (actValProp == null)
            {
                //Error50292: Initialization error: The scale component doesn't have the property {0}.
                // Initialisierungsfehler: Die Waagen-Komponente besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50292", false, "ActualValue");
                return;
            }

            double digitWeight = 1.0;
            var digitWeightProp = scale.GetPropertyNet(nameof(PAEScaleBase.DigitWeight));
            if (digitWeightProp != null)
            {
                digitWeight = (digitWeightProp as IACContainerTNet<double>).ValueT;
                if (digitWeight <= double.Epsilon)
                    digitWeight = 1.0;
            }
            if (digitWeight >= 999.99999)
                ScalePrecisionFormat = "F0";
            else if (digitWeight >= 99.99999)
                ScalePrecisionFormat = "F1";
            else if (digitWeight >= 9.99999)
                ScalePrecisionFormat = "F2";
            else if (digitWeight >= 0.99999)
                ScalePrecisionFormat = "F3";
            else if (digitWeight >= 0.09999)
                ScalePrecisionFormat = "F4";
            else if (digitWeight >= 0.00999)
                ScalePrecisionFormat = "F5";
            else if (digitWeight >= 0.00099)
                ScalePrecisionFormat = "F6";

            _ScaleActualValue = actValProp;

            _ScaleActualWeight = actWeightProp as IACContainerTNet<double>;
            (_ScaleActualWeight as IACPropertyNetBase).PropertyChanged += ActWeightProp_PropertyChanged;
            (_ScaleActualValue as IACPropertyNetBase).PropertyChanged += ScaleActualValue_PropertyChanged;
            ScaleRealWeight = _ScaleActualWeight.ValueT;
            ScaleGrossWeight = _ScaleActualValue.ValueT;
            OnPropertyChanged("TargetWeight");
            OnPropertyChanged("ScaleDifferenceWeight");

           OnPropertyChanged(nameof(CurrentScaleObject));
        }

        #endregion


    }
}
