// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using VD = gip.mes.datamodel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using gip.mes.facility;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Businessobject or App for emptying (discharging) preparation bins at a work center.
    /// It is a child BSO of the work center selector (BSOWorkCenterChild) and is activated with the
    /// current process function (Activate()). The BSO loads the containers that must be emptied for
    /// the current production step into DischargingItemList (SelectedDischargingItem holds the
    /// selected entry), each with lot, inward booking (filled), outward booking (discharged) and
    /// the discharged flag.
    /// The containers are emptied in the order shown: scan the container (its GUID is written to
    /// InputSourceCode) and call SendCode() (button "Select") which forwards the code to the
    /// process function ("!SetCodeNo"); or double-click a container in the list (SelectForDiscarge(),
    /// button "SelectForDiscarge") which fills InputSourceCode from the selected entry and calls
    /// SendCode(). When the process function waits for the discharge confirmation
    /// (WaitForConfirmDischarge), the matching container is preselected automatically.
    /// ConfirmDischarge() (button "Confirm discharge") confirms the discharge of the selected
    /// container. If the tolerance check weighing fails, the ToleranceDialog is shown with the
    /// ToleranceMsg: CheckWeightAgain() (button "Check weight again") re-checks the weight,
    /// AcceptToleranceError() (button "Accept tolerance error") accepts the tolerance error and
    /// continues, Cancel() closes the dialog.
    /// The list is reloaded automatically when the state of the process function changes; when the
    /// function returns to idle and no undischarged container is left, the list is cleared.
    /// This BSO works directly against the database and the process function; there is no separate
    /// Save()/Delete()/Load() workflow as in the master data BSOs.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOBinDischarging&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Bin discharging'}de{'Gebinde entleeren'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, SortIndex = 300,
        Description = @"Businessobject or App for emptying (discharging) preparation bins at a work center.
        It is a child BSO of the work center selector (BSOWorkCenterChild) and is activated with the current process function (Activate()). The BSO loads the containers that must be emptied for the current production step into DischargingItemList (SelectedDischargingItem holds the selected entry), each with lot, inward booking (filled), outward booking (discharged) and the discharged flag.
        The containers are emptied in the order shown: scan the container (its GUID is written to InputSourceCode) and call SendCode() (button ""Select"") which forwards the code to the process function (""!SetCodeNo""); or double-click a container in the list (SelectForDiscarge(), button ""SelectForDiscarge"") which fills InputSourceCode from the selected entry and calls SendCode(). When the process function waits for the discharge confirmation (WaitForConfirmDischarge), the matching container is preselected automatically.
        ConfirmDischarge() (button ""Confirm discharge"") confirms the discharge of the selected container. If the tolerance check weighing fails, the ToleranceDialog is shown with the ToleranceMsg: CheckWeightAgain() (button ""Check weight again"") re-checks the weight, AcceptToleranceError() (button ""Accept tolerance error"") accepts the tolerance error and continues, Cancel() closes the dialog.
        The list is reloaded automatically when the state of the process function changes; when the function returns to idle and no undischarged container is left, the list is cleared.
        This BSO works directly against the database and the process function; there is no separate Save()/Delete()/Load() workflow as in the master data BSOs.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOBinDischarging&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOBinDischarging : BSOWorkCenterChild
    {
        public static string ClassName = @"BSOBinDischarging";

        #region ctor's

        public BSOBinDischarging(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool init = base.ACInit(startChildMode);

            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _DischargingItemManager = DischargingItemManager.ACRefToServiceInstance(this);
            if (_DischargingItemManager == null)
                throw new Exception("DischargingItemManager not configured");

            return init;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            if (ItemFunction != null)
                ItemFunction.ACStateProperty.PropertyChanged -= ACStateProperty_PropertyChanged;

            if (_WaitForConfirm != null)
                _WaitForConfirm.PropertyChanged -= _WaitForConfirm_PropertyChanged;
            _WaitForConfirm = null;

            //if (_InToleranceError != null)
            //    _InToleranceError.PropertyChanged += _InToleranceError_PropertyChanged;
            //_InToleranceError = null;

            if (_ProdOrderManager != null)
                ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;

            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            if (_DischargingItemManager != null)
                DischargingItemManager.DetachACRefFromServiceInstance(this, _DischargingItemManager);
            _DischargingItemManager = null;


            return await base.ACDeInit(deleteACClassTask);
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

        protected ACRef<DischargingItemManager> _DischargingItemManager = null;
        public DischargingItemManager DischargingItemManager
        {
            get
            {
                if (_DischargingItemManager == null)
                    return null;
                return _DischargingItemManager.ValueT;
            }
        }

        #endregion


        #region Properties

        #region Properties -> Other


        public string _InputSourceCode;
        [ACPropertyInfo(609, "InputSourceCode", "en{'Code'}de{'Code'}", Description = "Gets or sets the code (GUID) of the container that is selected with SendCode(). Filled by SelectForDiscarge() from the selected list entry or by scanning.")]
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

        private IACContainerTNet<ACMethod> _CurrentPAFACMethod;

        private Guid? _IntermediateChildPosID, _SourceCodeItemID;

        public ManualPreparationSourceInfoTypeEnum _SourceInfoType;

        private IACContainerTNet<bool> _WaitForConfirm;

        private string _ToleranceMsg;
        [ACPropertyInfo(610, "ToleranceMsg", "en{'Tolerance message'}de{'Toleranzmeldung'}", Description = "Gets or sets the message of the tolerance check weighing that is shown in the ToleranceDialog.")]
        public string ToleranceMsg
        {
            get => _ToleranceMsg;
            set
            {
                _ToleranceMsg = value;
                OnPropertyChanged("ToleranceMsg");
            }
        }

        #endregion

        #region Properties -> Message

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyInfo(9999, "Message", "en{'Message'}de{'Meldung'}", Description = "Gets or sets the last message of the discharging (e.g. result of SendCode or ConfirmDischarge).")]
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

        #region DischargingItem
        private DischargingItem _SelectedDischargingItem;
        /// <summary>
        /// Selected property for DischargingItem
        /// </summary>
        /// <value>The selected DischargingItem</value>
        [ACPropertySelected(601, "DischargingItem", "en{'Discharging item'}de{'Entladeposition'}", Description = "Gets or sets the currently selected container (discharging item) of the list. Preselected automatically when the process function waits for the discharge confirmation.")]
        public DischargingItem SelectedDischargingItem
        {
            get
            {
                return _SelectedDischargingItem;
            }
            set
            {
                if (_SelectedDischargingItem != value)
                {
                    _SelectedDischargingItem = value;
                    OnPropertyChanged("SelectedDischargingItem");
                }
            }
        }

        private List<DischargingItem> _DischargingItemList;
        /// <summary>
        /// List property for DischargingItem
        /// </summary>
        /// <value>The DischargingItem list</value>
        [ACPropertyList(602, "DischargingItem", "en{'Discharging items'}de{'Entladepositionen'}", Description = "Gets the list of containers that must be emptied for the current production step, with lot, inward booking (filled), outward booking (discharged) and discharged flag.")]
        public List<DischargingItem> DischargingItemList
        {
            get
            {
                if (_DischargingItemList == null)
                    _DischargingItemList = new List<DischargingItem>();
                return _DischargingItemList;
            }
        }

        #endregion

        #endregion

        #region Methods

        public override void Activate(ACComponent selectedProcessModule)
        {
            if (ItemFunction == null || ItemFunction.ProcessFunction == null)
                return;

            object[] selectedItems = ItemFunction?.ProcessFunction?.ACUrlCommand("!GetSelection") as object[];
            if (selectedItems != null)
            {
                _IntermediateChildPosID = (Guid)selectedItems[0];
                _SourceInfoType = (ManualPreparationSourceInfoTypeEnum)selectedItems[1];
            }

            LoadDischargingItemList();

            ItemFunction.ACStateProperty.PropertyChanged += ACStateProperty_PropertyChanged;
            _CurrentPAFACMethod = ItemFunction?.ProcessFunction?.GetPropertyNet("CurrentACMethod") as IACContainerTNet<ACMethod>;

            _WaitForConfirm = ItemFunction?.ProcessFunction.GetPropertyNet("WaitForConfirmDischarge") as IACContainerTNet<bool>;
            if(_WaitForConfirm != null)
            {
                _WaitForConfirm_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(Const.ValueT));
                _WaitForConfirm.PropertyChanged += _WaitForConfirm_PropertyChanged;
            }

        }

        ACStateEnum tmpState = ACStateEnum.SMStarting;

        private void ACStateProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != Const.ValueT)
                return;

            IACContainerTNet<ACStateEnum> tmp = sender as IACContainerTNet<ACStateEnum>;

            if (tmp != null && tmpState != tmp.ValueT)
            {
                tmpState = tmp.ValueT;
                Task.Run(() =>
                {
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        if ((_DischargingItemList == null || !_DischargingItemList.Any()) && tmp.ValueT == ACStateEnum.SMRunning)
                        {
                            if (_IntermediateChildPosID == null)
                            {
                                object[] selectedItems = ItemFunction?.ProcessFunction?.ACUrlCommand("!GetSelection") as object[];
                                if (selectedItems != null)
                                {
                                    _IntermediateChildPosID = (Guid)selectedItems[0];
                                    _SourceInfoType = (ManualPreparationSourceInfoTypeEnum)selectedItems[1];
                                }
                            }

                            LoadDischargingItemList();
                        }
                        else if (tmp.ValueT == ACStateEnum.SMCompleted && _CurrentPAFACMethod != null)
                        {
                            string id = "";
                            Guid itemID = Guid.Empty;
                            if (_CurrentPAFACMethod.ValueT[PAFBinDischarging.Const_InputSourceCodes] != null)
                                id = _CurrentPAFACMethod.ValueT[PAFBinDischarging.Const_InputSourceCodes].ToString();

                            if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out itemID))
                                _SourceCodeItemID = itemID;
                        }

                        else if (tmp.ValueT == ACStateEnum.SMStarting && _SourceCodeItemID.HasValue)
                        {
                            CurrentMsg = null;
                            DischargingItem dischargingItem = DischargingItemList.FirstOrDefault(c => c.ItemID == _SourceCodeItemID);
                            if (dischargingItem != null)
                            {
                                using (VD.DatabaseApp dbApp = new VD.DatabaseApp())
                                {
                                    var outwardBooking = dbApp.FacilityBooking.Where(c => c.ProdOrderPartslistPosRelationID == dischargingItem.ProdorderPartslistPosRelationID
                                                                                       && ((c.OutwardFacilityID.HasValue && c.OutwardFacility.FacilityNo == dischargingItem.InwardFacilityNo)
                                                                                         || c.OutwardFacilityChargeID.HasValue)).FirstOrDefault();

                                    if (outwardBooking != null)
                                    {
                                        dischargingItem.OutwardBookingQuantityUOM = outwardBooking.OutwardQuantity;
                                    }
                                }
                            }
                        }

                        else if (tmp.ValueT == ACStateEnum.SMIdle)
                        {
                            if (DischargingItemList == null || DischargingItemList.Count(c => !c.IsDischarged) <= 1)
                            {
                                _DischargingItemList = null;
                                OnPropertyChanged("DischargingItemList");
                                InputSourceCode = "";
                                //EndBatchPos = null;
                                CurrentMsg = null;
                                _IntermediateChildPosID = null;
                                _SourceCodeItemID = null;
                            }
                        }
                    }
                });
            }
        }

        private void _WaitForConfirm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                if (_WaitForConfirm.ValueT)
                {
                    SelectedDischargingItem = null;

                    ACValue binIDVal = _CurrentPAFACMethod?.ValueT?.ParameterValueList.GetACValue(PAFBinDischarging.Const_InputSourceCodes);
                    if (binIDVal != null)
                    {
                        string id = binIDVal.ParamAsString;
                        Guid binID;
                        if (Guid.TryParse(id, out binID))
                        {
                            SelectedDischargingItem = DischargingItemList.FirstOrDefault(c => c.ItemID == binID);
                        }
                    }
                    
                    if (SelectedDischargingItem == null)
                    {
                        //alarm
                        return;
                    }

                    SelectedDischargingItem.IsDischarging = true;
                }
            }
        }

        public void LoadDischargingItemList()
        {
            if (_IntermediateChildPosID.HasValue && DischargingItemManager != null)
            {
                _DischargingItemList = DischargingItemManager.LoadDischargingItemList(_IntermediateChildPosID.Value, _SourceInfoType);
                OnPropertyChanged("DischargingItemList");
            }
        }

        public override void DeActivate()
        {
            ItemFunction.ACStateProperty.PropertyChanged -= ACStateProperty_PropertyChanged;

            if (_WaitForConfirm != null)
                _WaitForConfirm.PropertyChanged -= _WaitForConfirm_PropertyChanged;
            _WaitForConfirm = null;

            //if(_InToleranceError != null)
            //    _InToleranceError.PropertyChanged += _InToleranceError_PropertyChanged;
            //_InToleranceError = null;

        }

        [ACMethodInfo("SendCode", "en{'Select'}de{'Auswählen'}", 601, Description = "Forwards the code in InputSourceCode to the process function (\"!SetCodeNo\") which selects the container and starts its discharge. Enabled only if the code is a valid GUID.")]
        public void SendCode()
        {
            if (!IsEnabledSendCode())
                return;
            CurrentMsg = null;
            CurrentMsg = ItemFunction.ProcessFunction.ACUrlCommand("!SetCodeNo", InputSourceCode) as Msg;
        }

        public bool IsEnabledSendCode()
        {
            bool isValidInputSourceCode = false;
            if (!string.IsNullOrEmpty(InputSourceCode) && ItemFunction != null)
                isValidInputSourceCode = true;

            if (isValidInputSourceCode)
            {
                Guid testID = new Guid();
                isValidInputSourceCode = Guid.TryParse(InputSourceCode, out testID);
            }
            return isValidInputSourceCode;
        }

        [ACMethodInfo("SelectForDiscarge", "en{'SelectForDiscarge'}de{'SelectForDiscarge'}", 602, false, false, true, Description = "Double-click handler of the container list: fills InputSourceCode with the GUID of the selected container and calls SendCode(). Enabled only if a container is selected.")]
        public void SelectForDiscarge()
        {
            if (!IsEnabledSelectForDiscarge()) return;
            InputSourceCode = SelectedDischargingItem.ItemID.ToString();
            SendCode();
        }

        public bool IsEnabledSelectForDiscarge()
        {
            return SelectedDischargingItem != null;
        }

        [ACMethodInfo("", "en{'Confirm discharge'}de{'Entladung bestätigen'}", 603, Description = "Confirms the discharge of the selected container (\"!ConfirmDischarge\" of the process function). If a message is returned (e.g. tolerance check weighing failed), it is shown in the ToleranceDialog.")]
        public async Task ConfirmDischarge()
        {
            CurrentMsg = null;
            Msg msg = ItemFunction.ProcessFunction.ACUrlCommand("!ConfirmDischarge") as Msg;
            if (msg != null)
            {
                ToleranceMsg = msg.Message;
                await ShowDialogAsync(this, "ToleranceDialog");
            }
        }

        public bool IsEnabledConfirmDischarge()
        {
            return _WaitForConfirm != null && _WaitForConfirm.ValueT;
        }

        #region Methods => ToleranceResolve

        [ACMethodInfo("", "en{'Check weight again'}de{'Gewicht erneut prüfen'}", 610, Description = "Re-checks the weight (\"!ConfirmDischarge\" of the process function) and shows the result; if the check succeeds the ToleranceDialog is closed.")]
        public void CheckWeightAgain()
        {
            Msg msg = ItemFunction.ProcessFunction.ACUrlCommand("!ConfirmDischarge") as Msg;
            if (msg != null)
            {
                Messages.MsgAsync(msg);
            }
            else
                CloseTopDialog();
        }

        [ACMethodInfo("", "en{'Accept tolerance error'}de{'Toleranzfehler akzeptieren'}", 611, Description = "Accepts the tolerance error (\"!ConfirmDischarge\" with force flag of the process function) and closes the ToleranceDialog.")]
        public void AcceptToleranceError()
        {
            ItemFunction.ProcessFunction.ACUrlCommand("!ConfirmDischarge", true);
            CloseTopDialog();
        }

        [ACMethodInfo("", Const.Cancel, (short)MISort.Cancel, Description = "Closes the ToleranceDialog without accepting the tolerance error.")]
        public void Cancel()
        {
            CloseTopDialog();
        }

        #endregion

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;

            switch(acMethodName)
            {
                case nameof(SendCode):
                    SendCode();
                    return true;
                case nameof(IsEnabledSendCode):
                    result = IsEnabledSendCode();
                    return true;
                case nameof(SelectForDiscarge):
                    SelectForDiscarge();
                    return true;
                case nameof(IsEnabledSelectForDiscarge):
                    result = IsEnabledSelectForDiscarge();
                    return true;
                case nameof(ConfirmDischarge):
                    _ = ConfirmDischarge();
                    return true;
                case nameof(IsEnabledConfirmDischarge):
                    result = IsEnabledConfirmDischarge();
                    return true;
                case nameof(CheckWeightAgain):
                    CheckWeightAgain();
                    return true;
                case nameof(AcceptToleranceError):
                    AcceptToleranceError();
                    return true;
                case nameof(Cancel):
                    Cancel();
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        
        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(SendCode):
                    return new string[] { nameof(InputSourceCode) };
                case nameof(SelectForDiscarge):
                case nameof(ConfirmDischarge):
                    return new string[] { nameof(SelectedDischargingItem) };
                case nameof(CheckWeightAgain):
                case nameof(AcceptToleranceError):
                    return new string[] { nameof(ToleranceMsg) };
                case nameof(Cancel):
                    return new string[] { nameof(InitState) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

        #region Helper methods

        private List<DischargingItem> LoadTestDischargingItemList()
        {
            List<DischargingItem> testList = new List<DischargingItem>();
            testList.Add(
                new DischargingItem()
                {
                    ItemID = Guid.NewGuid(),
                    LotNo = "LOT100200",
                    InwardBookingNo = "FB0304040",
                    InwardBookingQuantityUOM = 10.3,
                    InwardFacilityNo = "S103",
                    InwardFacilityName = "Silo103",
                    InwardMaterialNo = "0001",
                    InwardMaterialName = "Ingerent 01",
                    InwardBookingTime = DateTime.Now,

                    OutwardBookingNo = "FB34000",
                    OutwardBookingQuantityUOM = 10.4,
                    OutwardBookingTime = DateTime.Now,

                    SourceInfoType = ManualPreparationSourceInfoTypeEnum.FacilityID

                });

            testList.Add(
                new DischargingItem()
                {
                    ItemID = Guid.NewGuid(),
                    LotNo = "LOT100200",
                    InwardBookingNo = "FB0304040",
                    InwardBookingQuantityUOM = 10.3,
                    InwardFacilityNo = "S103",
                    InwardFacilityName = "Silo103",
                    InwardMaterialNo = "0001",
                    InwardMaterialName = "Ingerent 01",
                    InwardBookingTime = DateTime.Now,

                    OutwardBookingNo = "FB34000",
                    OutwardBookingQuantityUOM = 10.4,
                    OutwardBookingTime = DateTime.Now,

                    SourceInfoType = ManualPreparationSourceInfoTypeEnum.FacilityID,

                });


            testList.Add(
                new DischargingItem()
                {
                    ItemID = Guid.NewGuid(),
                    LotNo = "LOT100200",
                    InwardBookingNo = "FB0304040",
                    InwardBookingQuantityUOM = 10.3,
                    InwardFacilityNo = "S103",
                    InwardFacilityName = "Silo103",
                    InwardMaterialNo = "0001",
                    InwardMaterialName = "Ingerent 01",

                    OutwardBookingNo = "",
                    OutwardBookingQuantityUOM = 0,
                    SourceInfoType = ManualPreparationSourceInfoTypeEnum.FacilityID
                });
            return testList;
        }

        #endregion
    }
}
