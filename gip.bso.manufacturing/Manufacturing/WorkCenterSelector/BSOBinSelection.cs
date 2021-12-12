using gip.core.autocomponent;
using gip.core.datamodel;
using System.Linq;
using vb = gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data;
using gip.mes.processapplication;
using System.ComponentModel;
using System.Threading.Tasks;
using static gip.core.datamodel.Global;
using gip.mes.facility;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Bin selection'}de{'Behältnis auswählen'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, SortIndex = 300)]
    public class BSOBinSelection : BSOWorkCenterChild
    {
        public static string ClassName = @"BSOBinSelection";
        #region c'tors

        public BSOBinSelection(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //CurrentMsg = new Msg() { MessageLevel = eMsgLevel.Info, ACIdentifier = "TestMsg", Message = "Test message!" };
        }

        public override bool ACInit(ACStartTypes startChildMode = ACStartTypes.Automatic)
        {
            bool init = base.ACInit(startChildMode);

            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            DischargingItemManager = new DischargingItemManager(Root, this, ClassName, ACFacilityManager, ProdOrderManager, null);
            return init;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (ItemFunction != null && ItemFunction.ACStateProperty != null)
                ItemFunction.ACStateProperty.PropertyChanged -= ACStateProperty_PropertyChanged;

            if (_PWBinSelectionACStateProp != null)
            {
                _PWBinSelectionACStateProp.PropertyChanged -= _PWBinSelectionACStateProp_PropertyChanged;
                _PWBinSelectionACStateProp = null;
            }

            if (_PWBinSelection != null)
            {
                _PWBinSelection.Detach();
                _PWBinSelection = null;
            }

            DischargingItemManager = null;

            if (_ProdOrderManager != null)
                ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;

            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Managers

        protected ACRef<ACProdOrderManager> _ProdOrderManager = null;
        public ACProdOrderManager ProdOrderManager
        {
            get
            {
                if (_ProdOrderManager == null)
                    return null;
                return _ProdOrderManager.ValueT;
            }
        }

        public DischargingItemManager DischargingItemManager { get; private set; }

        #endregion

        #region Properties

        private ManualPreparationSourceInfoTypeEnum _SourceType;
        public ManualPreparationSourceInfoTypeEnum SourceType
        {
            get
            {
                //ManualPreparationSourceInfoTypeEnum sourceType = ManualPreparationSourceInfoTypeEnum.FacilityID;
                //object[] selectedItems = ItemFunction?.ProcessFunction?.ACUrlCommand("!GetSelection") as object[];
                //if (selectedItems != null)
                //{
                //    sourceType = (ManualPreparationSourceInfoTypeEnum)selectedItems[1];
                //}
                return _SourceType;
            }
        }

        #region Properties -> Messages

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyInfo(610, "Message", "en{'Message'}de{'Meldung'}")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged("CurrentMsg");
            }
        }

        #endregion

        #region Properties -> Other

        public string _InputSourceCode;
        [ACPropertyInfo(611, "InputSourceCode", "en{'Code'}de{'Code'}")]
        public string InputSourceCode
        {
            get
            {
                return _InputSourceCode;
            }
            set
            {
                if (_InputSourceCode != value)
                {
                    _InputSourceCode = value;
                    OnPropertyChanged("InputSourceCode");
                }
            }
        }

        ACRef<IACComponentPWNode> _PWBinSelection;
        public IACComponentPWNode PWBinSelectionNode
        {
            get => _PWBinSelection?.ValueT;
        }

        private IACContainerTNet<ACStateEnum> _PWBinSelectionACStateProp;

        #endregion

        #region Properties -> BinFacility

        private BinSelectionItem _SelectedBinFacility;
        /// <summary>
        /// Selected property for Facility
        /// </summary>
        /// <value>The selected BinFacility</value>
        [ACPropertySelected(601, "BinFacility", "en{'TODO: BinFacility'}de{'TODO: BinFacility'}")]
        public BinSelectionItem SelectedBinFacility
        {
            get
            {
                return _SelectedBinFacility;
            }
            set
            {
                if (_SelectedBinFacility != value)
                {
                    _SelectedBinFacility = value;
                    OnPropertyChanged("SelectedBinFacility");
                }
            }
        }

        private List<BinSelectionItem> _BinFacilityList;
        /// <summary>
        /// List property for Facility
        /// </summary>
        /// <value>The BinFacility list</value>
        [ACPropertyList(602, "BinFacility")]
        public List<BinSelectionItem> BinFacilityList
        {
            get => _BinFacilityList;
            set
            {
                _BinFacilityList = value;
                OnPropertyChanged("BinFacilityList");
            }
        }

        private void LoadList()
        {
            switch (SourceType)
            {
                case ManualPreparationSourceInfoTypeEnum.FacilityID:
                    LoadBinFacilityList();
                    break;
                case ManualPreparationSourceInfoTypeEnum.FacilityChargeID:
                    LoadBinFacilityChargeList();
                    break;
                default:
                    break;
            }
        }

        private void LoadBinFacilityList()
        {
            using (vb.DatabaseApp dbApp = new vb.DatabaseApp())
            {
                List<BinSelectionItem> binSelectionList =
                    dbApp
                    .Facility
                    .Where(c => c.MDFacilityType.MDFacilityTypeIndex == (short)vb.FacilityTypesEnum.PreparationBin)
                   .OrderBy(c => c.FacilityNo)
                   .AsEnumerable()
                   .Select(c => new BinSelectionItem()
                   {
                       FacilityID = c.FacilityID,
                       FacilityNo = c.FacilityNo,
                       FacilityName = c.FacilityName,
                       LastInwardFBC = c.FacilityBooking_InwardFacility.OrderByDescending(d => d.InsertDate).FirstOrDefault(),
                       LastOutwardFBC = c.FacilityBooking_OutwardFacility.OrderByDescending(d => d.InsertDate).FirstOrDefault()
                   })
                   .ToList();

                BinFacilityList = binSelectionList;
            }
        }

        private void LoadBinFacilityChargeList()
        {
            using (vb.DatabaseApp dbApp = new vb.DatabaseApp())
            {
                // c.FacilityLot == null - Charge recive LotNo from production
                List<BinSelectionItem> binSelectionList =
                    dbApp
                        .FacilityCharge
                        .Where(c =>
                            //c.IsEnabled 
                            !c.NotAvailable
                            && c.MaterialID == EndBatchPos.MaterialID
                            && c.FacilityLot == null)
                        .AsEnumerable()
                        .Where(c => c.AvailableQuantity > 0.1)
                        .OrderBy(c => c.FacilityLot.LotNo)
                        .Select(c => new BinSelectionItem()
                        {
                            FacilityChargeID = c.FacilityChargeID,
                            FacilityNo = c.FacilityLot.LotNo,
                            FacilityName = c.FacilityLot.FacilityLotID.ToString(),
                            LastInwardFBC = c.FacilityBooking_InwardFacilityCharge.OrderByDescending(d => d.InsertDate).FirstOrDefault(),
                            LastOutwardFBC = c.FacilityBooking_OutwardFacilityCharge.OrderByDescending(d => d.InsertDate).FirstOrDefault()
                        })
                        .ToList();

                BinFacilityList = binSelectionList;
            }
        }

        #endregion

        #region Properties => OrderInfo

        private vb.ProdOrderPartslistPos _EndBatchPos;
        [ACPropertyInfo(604)]
        public vb.ProdOrderPartslistPos EndBatchPos
        {
            get => _EndBatchPos;
            set
            {
                _EndBatchPos = value;
                OnPropertyChanged("EndBatchPos");
            }
        }

        #endregion

        #endregion

        #region Methods

        public override void Activate(ACComponent selectedProcessModule)
        {
            CurrentMsg = null;
            LoadBinSelection();
            ItemFunction.ACStateProperty.PropertyChanged += ACStateProperty_PropertyChanged;
        }

        public override void DeActivate()
        {
            ItemFunction.ACStateProperty.PropertyChanged -= ACStateProperty_PropertyChanged;
            UnloadBinSelection();
        }


        public ACRef<IACComponentPWNode> GetPWComponent(string className)
        {
            ACRef<IACComponentPWNode> component = null;
            if (ItemFunction != null && ItemFunction.ProcessFunction != null && PWBinSelectionNode == null)
            {
                string[] accessArr = (string[])ItemFunction.ProcessFunction.ParentACComponent?.ACUrlCommand("!SemaphoreAccessedFrom");
                if (accessArr == null || !accessArr.Any())
                    return null;

                string pwGroupACUrl = accessArr[0];
                IACComponentPWNode pwGroup = null;

                pwGroup = Root.ACUrlCommand(pwGroupACUrl) as IACComponentPWNode;
                if (pwGroup == null)
                    return null;

                IEnumerable<ACChildInstanceInfo> pwNodes;

                using (Database db = new Database())
                {
                    var pwClass = db.ACClass.FirstOrDefault(c => c.ACProject.ACProjectTypeIndex == (short)Global.ACProjectTypes.ClassLibrary &&
                                                                                                    c.ACIdentifier == className);
                    ACRef<ACClass> refClass = new ACRef<ACClass>(pwClass, true);
                    pwNodes = pwGroup.GetChildInstanceInfo(1, new ChildInstanceInfoSearchParam() { OnlyWorkflows = true, TypeOfRoots = refClass });
                    refClass.Detach();
                }

                ACChildInstanceInfo pwNode = pwNodes?.FirstOrDefault();
                if (pwNode != null)
                {
                    var binNode = pwGroup.ACUrlCommand(pwNode.ACUrlParent + "\\" + pwNode.ACIdentifier) as IACComponentPWNode;
                    if (binNode != null)
                    {
                        component = new ACRef<IACComponentPWNode>(binNode, this);
                    }
                }
            }
            return component;
        }

        private void LoadBinSelection()
        {
            EntityKey entityKey = null;

            _PWBinSelection = GetPWComponent(PWBinSelection.PWClassName);
            if (_PWBinSelection != null)
            {
                entityKey = _PWBinSelection.ValueT.ACUrlCommand("!GetIntermediateChildPos") as EntityKey;

                var acState = _PWBinSelection.ValueT.GetPropertyNet(Const.ACState);
                _PWBinSelectionACStateProp = acState as IACContainerTNet<ACStateEnum>;
                if (_PWBinSelectionACStateProp != null)
                {
                    _PWBinSelectionACStateProp.PropertyChanged += _PWBinSelectionACStateProp_PropertyChanged;
                }

                ManualPreparationSourceInfoTypeEnum sourceType = ManualPreparationSourceInfoTypeEnum.FacilityID;
                object[] selectedItems = ItemFunction?.ProcessFunction?.ACUrlCommand("!GetSelection") as object[];
                if (selectedItems != null)
                    sourceType = (ManualPreparationSourceInfoTypeEnum)selectedItems[1];

                _SourceType = sourceType;
            }

            if (entityKey != null)
            {
                Guid intermediateChildPosID = (Guid)entityKey.EntityKeyValues[0].Value;
                GetEndBatchPos(DatabaseApp, intermediateChildPosID);
            }
            LoadList();
        }

        private void UnloadBinSelection()
        {
            if (_PWBinSelectionACStateProp != null)
            {
                _PWBinSelectionACStateProp.PropertyChanged -= _PWBinSelectionACStateProp_PropertyChanged;
                _PWBinSelectionACStateProp = null;
            }

            if (_PWBinSelection != null)
            {
                _PWBinSelection.Detach();
                _PWBinSelection = null;
            }

            EndBatchPos = null;
            CurrentMsg = null;
        }

        private void _PWBinSelectionACStateProp_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT && _PWBinSelectionACStateProp != null)
            {
                if (_PWBinSelectionACStateProp.ValueT == ACStateEnum.SMCompleted)
                {
                    Task.Run(() => LoadList());
                    //LoadList();
                }
            }
        }

        ACStateEnum tmpState = ACStateEnum.SMStarting;
        private void ACStateProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT && sender != null && sender is ACPropertyNetSource<ACStateEnum>)
            {
                ACPropertyNetSource<ACStateEnum> tmp =
                    sender as ACPropertyNetSource<ACStateEnum>;
                if (tmpState != tmp.ValueT)
                {
                    if (tmp.ValueT == ACStateEnum.SMStarting || tmp.ValueT == ACStateEnum.SMRunning)
                        LoadBinSelection();
                    else if (tmp.ValueT == ACStateEnum.SMCompleted)
                        LoadList();
                    else if (tmp.ValueT == ACStateEnum.SMResetting)
                    {
                        //_BinFacilityList = null;
                        //OnPropertyChanged("BinFacilityList");
                        InputSourceCode = "";
                        UnloadBinSelection();
                    }
                    tmpState = tmp.ValueT;
                }
            }
        }

        [ACMethodInfo("SendCode", "en{'Select'}de{'Auswählen'}", 601)]
        public void SendCode()
        {
            if (!IsEnabledSendCode()) return;
            CurrentMsg = null;
            Msg msg = ItemFunction.ProcessFunction.ACUrlCommand("!SetCodeNo", InputSourceCode) as Msg;
            if (msg.IsSucceded())
                InputSourceCode = "";
            CurrentMsg = msg;
        }

        public bool IsEnabledSendCode()
        {
            Guid testID = new Guid();
            bool isValid = (!string.IsNullOrEmpty(InputSourceCode) && ItemFunction != null)
                && Guid.TryParse(InputSourceCode, out testID);
            if (SourceType == ManualPreparationSourceInfoTypeEnum.FacilityID)
                isValid = isValid && BinFacilityList.Any(c =>
                    (
                        c.FacilityID.ToString() == InputSourceCode
                        || c.FacilityChargeID.ToString() == InputSourceCode
                    )
                    && !c.IsReserved);
            return isValid;
        }

        [ACMethodInfo("BreakBinSelection", "en{'Break'}de{'Abbrechen'}", 602)]
        public void BreakBinSelection()
        {
            if (!IsEnabledBreakBinSelection()) return;
            CurrentMsg = null;
            CurrentMsg = ItemFunction.ProcessFunction.ACUrlCommand("!BreakBinSelection") as Msg;
        }

        public bool IsEnabledBreakBinSelection()
        {
            return true;
        }


        [ACMethodInfo("BinSelect", "en{'BinSelect'}de{'BinSelect'}", 603, false, false, true)]
        public void DblClickBin()
        {
            if (!IsEnabledDblClickBin())
                return;
            if (SourceType == ManualPreparationSourceInfoTypeEnum.FacilityID)
                InputSourceCode = SelectedBinFacility.FacilityID.ToString();
            else if (SourceType == ManualPreparationSourceInfoTypeEnum.FacilityChargeID)
                InputSourceCode = SelectedBinFacility.FacilityChargeID.ToString();
            SendCode();
        }

        public bool IsEnabledDblClickBin()
        {
            return SelectedBinFacility != null && !SelectedBinFacility.IsReserved;
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;

            switch (acMethodName)
            {
                case "SendCode":
                    SendCode();
                    return true;
                case "IsEnabledSendCode":
                    result = IsEnabledSendCode();
                    return true;
                case "BreakBinSelection":
                    BreakBinSelection();
                    return true;
                case "IsEnabledBreakBinSelection":
                    result = IsEnabledBreakBinSelection();
                    return true;
                case "DblClickBin":
                    DblClickBin();
                    return true;
                case "BinFreeUp":
                    BinFreeUp();
                    return true;
                case "IsEnabledDblClickBin":
                    result = IsEnabledDblClickBin();
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        /// <summary>
        /// Free reserved container
        /// </summary>
        [ACMethodInfo("BinFreeUp", "en{'Bin discharging'}de{'Gebinde entleeren'}", 607, false, false, true)]
        public void BinFreeUp()
        {
            BinFreeUpActionEnum freeUpStatus = BinFreeUpActionEnum.UsedInProdOrder;
            MsgResult msgUsedInProduction = Messages.Question(this, "Question50056");
            if (msgUsedInProduction == MsgResult.No)
            {
                MsgResult msgIsAussuss = Messages.Question(this, "Question50057");
                if (msgIsAussuss == MsgResult.Yes)
                    freeUpStatus = BinFreeUpActionEnum.Ausschuss;
                else
                    freeUpStatus = BinFreeUpActionEnum.BackToStock;
            }
            
            List<DischargingItem> dischargingItems = DischargingItemManager.LoadDischargingItemList(SelectedBinFacility.LastInwardFBC.ProdOrderPartslistPosID ?? Guid.Empty, SourceType);
            DischargingItem dischargingItem = null;
            if (SourceType == ManualPreparationSourceInfoTypeEnum.FacilityID)
                dischargingItem = dischargingItems.FirstOrDefault(c => c.ItemID == SelectedBinFacility.FacilityID);
            else
                dischargingItem = dischargingItems.FirstOrDefault(c => c.ItemID == SelectedBinFacility.FacilityChargeID);
            if (dischargingItem != null)
                ProcessBinFreeUp(freeUpStatus, dischargingItem);
        }


        public bool IsEnabledBinFreeUp()
        {
            return
                SelectedBinFacility != null
                && SelectedBinFacility.IsReserved;
        }

        public void ProcessBinFreeUp(BinFreeUpActionEnum action, DischargingItem dischargingItem)
        {
            KeyValuePair<Msg, DischargingItem> dischOutwardRez = MakeOutwardBooking(dischargingItem);
            if (dischOutwardRez.Key == null || dischOutwardRez.Key.MessageLevel < eMsgLevel.Warning)
            {
                if (action == BinFreeUpActionEnum.Ausschuss)
                    MakeNegativeOutwardBookingOnAusschuss(dischOutwardRez.Value);
                else if (action == BinFreeUpActionEnum.BackToStock)
                    MakeNegativeOutwardBooking(dischOutwardRez.Value);

                LoadList();
            }
        }

        private KeyValuePair<Msg, DischargingItem> MakeOutwardBooking(DischargingItem dischargingItem)
        {
            return DischargingItemManager.ProceeedBooking(SourceType, dischargingItem.ItemID.ToString(), dischargingItem);
        }

        private KeyValuePair<Msg, DischargingItem> MakeNegativeOutwardBooking(DischargingItem dischargingItem)
        {
            BinSelectionModel binSelectionModel = new BinSelectionModel();
            if (SourceType == ManualPreparationSourceInfoTypeEnum.FacilityChargeID)
                binSelectionModel.FacilityChargeID = dischargingItem.InwardFacilityChargeID;
            else
                binSelectionModel.FacilityID = DatabaseApp.Facility.Where(c => c.FacilityNo == dischargingItem.InwardFacilityNo).Select(c => c.FacilityID).FirstOrDefault();
            binSelectionModel.ProdorderPartslistPosRelationID = dischargingItem.ProdorderPartslistPosRelationID;
            binSelectionModel.RestQuantity = -EndBatchPos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Where(c => c.ProdOrderPartslistPosRelationID == dischargingItem.ProdorderPartslistPosRelationID).Sum(c => c.TargetQuantityUOM);
            return DischargingItemManager.DoOutwardBooking(binSelectionModel);
        }

        private KeyValuePair<Msg, DischargingItem> MakeNegativeOutwardBookingOnAusschuss(DischargingItem dischargingItem)
        {
            BinSelectionModel binSelectionModel = new BinSelectionModel();
            // TODO: check Ausschuss Lagerplatz
            binSelectionModel.FacilityID = DatabaseApp.Facility.Where(c => c.FacilityNo == PWBinSelection.Config_LPFW).Select(c => c.FacilityID).FirstOrDefault();
            binSelectionModel.ProdorderPartslistPosRelationID = dischargingItem.ProdorderPartslistPosRelationID;
            binSelectionModel.RestQuantity = EndBatchPos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Where(c => c.ProdOrderPartslistPosRelationID == dischargingItem.ProdorderPartslistPosRelationID).Sum(c => c.TargetQuantityUOM);

            return DischargingItemManager.DoOutwardBooking(binSelectionModel);
        }

        #endregion

        #region private methods

        internal void GetEndBatchPos(vb.DatabaseApp db, Guid posID)
        {
            using (ACMonitor.Lock(db.QueryLock_1X000))
            {
                EndBatchPos = db.ProdOrderPartslistPos.Include(p => p.ProdOrderBatch).Include(pl => pl.ProdOrderPartslist.ProdOrder).Include(m => m.Material).FirstOrDefault(c => c.ProdOrderPartslistPosID == posID);
            }

        }

        #endregion
    }

    public enum BinFreeUpActionEnum
    {
        UsedInProdOrder,
        BackToStock,
        Ausschuss
    }
}