// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using System.Runtime.Serialization;
using gip.core.processapplication;
using System.Runtime.CompilerServices;
using gip.core.communication;
using gip.core.communication.ISOonTCP;

namespace gip2006.variobatch.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006ScaleMaskRes'}de{'Bitzugriff GIPConv2006ScaleMaskRes'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'Empty(Bit00)'}de{'Empty(Bit00)'}")]
    [ACPropertyEntity(101, "Bit01", "en{'Rough(Bit01)'}de{'Rough(Bit01)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Fine(Bit02)'}de{'Fine(Bit02)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Full(Bit03)'}de{'Full(Bit03)'}")]
    [ACPropertyEntity(104, "Bit04", "en{'Flow(Bit04)'}de{'Flow(Bit04)'}")]
    [ACPropertyEntity(105, "Bit05", "en{'Dosingtime(Bit06)'}de{'Dosingtime(Bit05)'}")]
    [ACPropertyEntity(106, "Bit06", "en{'Manual(Bit06)'}de{'Manual(Bit06)'}")]
    [ACPropertyEntity(107, "Bit07", "en{'Standstill(Bit07)'}de{'Standstill(Bit07)'}")]
    [ACPropertyEntity(108, "Bit08", "en{'LackOfMaterial(Bit08)'}de{'LackOfMaterial(Bit08)'}")]
    [ACPropertyEntity(109, "Bit09", "en{'Presignal(Bit09)'}de{'Presignal(Bit09)'}")]
    [ACPropertyEntity(110, "Bit10", "en{'Discharging(Bit10)'}de{'Discharging(Bit10)'}")]
    [ACPropertyEntity(111, "Bit11", "en{'Dosing(Bit11)'}de{'Dosing(Bit11)'}")]
    [ACPropertyEntity(112, "Bit12", "en{'Tared(Bit12)'}de{'Tared(Bit12)'}")]
    [ACPropertyEntity(113, "Bit13", "en{'Tolerance(Bit13)'}de{'Tolerance(Bit13)'}")]
    [ACPropertyEntity(114, "Bit14", "en{'AllocatedByWay(Bit14)'}de{'AllocatedByWay(Bit14)'}")]
    [ACPropertyEntity(115, "Bit15", "en{'Fault(Bit15)'}de{'Fault(Bit15)'}")]
    public class GIPConv2006ScaleMaskRes : BitAccessForInt16
    {
        #region c'tors
        public GIPConv2006ScaleMaskRes()
        {
        }

        public GIPConv2006ScaleMaskRes(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        public bool Bit00_Empty
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        public bool Bit01_Rough
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        public bool Bit02_Fine
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        public bool Bit03_Full
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        public bool Bit04_Flow
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }

        public bool Bit05_Dosingtime
        {
            get { return Bit05; }
            set { Bit05 = value; }
        }

        public bool Bit06_Manual
        {
            get { return Bit06; }
            set { Bit06 = value; }
        }

        public bool Bit07_Standstill
        {
            get { return Bit07; }
            set { Bit07 = value; }
        }

        public bool Bit08_LackOfMaterial
        {
            get { return Bit08; }
            set { Bit08 = value; }
        }

        public bool Bit09_Presignal
        {
            get { return Bit09; }
            set { Bit09 = value; }
        }

        public bool Bit10_Discharging
        {
            get { return Bit10; }
            set { Bit10 = value; }
        }

        public bool Bit11_Dosing
        {
            get { return Bit11; }
            set { Bit11 = value; }
        }

        public bool Bit12_Tared
        {
            get { return Bit12; }
            set { Bit12 = value; }
        }

        public bool Bit13_Tolerance
        {
            get { return Bit13; }
            set { Bit13 = value; }
        }

        public bool Bit14_AllocatedByWay
        {
            get { return Bit14; }
            set { Bit14 = value; }
        }

        public bool Bit15_Fault
        {
            get { return Bit15; }
            set { Bit15 = value; }
        }
        #endregion
    }

    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006ScaleMaskReq'}de{'Bitzugriff GIPConv2006ScaleMaskReq'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'AckFault(Bit00)'}de{'StörungQuitt(Bit00)'}")]
    [ACPropertyEntity(106, "Bit06", "en{'ResetZero(Bit06)'}de{'ResetZero(Bit06)'}")]
    [ACPropertyEntity(107, "Bit07", "en{'SetZero(Bit07)'}de{'SetZero(Bit07)'}")]
    [ACPropertyEntity(108, "Bit08", "en{'Service(Bit08)'}de{'ServiceBetrieb(Bit08)'}")]
    [ACPropertyEntity(109, "Bit09", "en{'OwnCommand(Bit09)'}de{'EigenerBefehl(Bit09)'}")]
    [ACPropertyEntity(110, "Bit10", "en{'AllDataReceived(Bit10)'}de{'AlleDatenEmpf(Bit10)'}")]
    [ACPropertyEntity(111, "Bit11", "en{'DataTransfer(Bit11)'}de{'DatenUebernehmen(Bit11)'}")]
    [ACPropertyEntity(112, "Bit12", "en{'ZeroAdjustment(Bit12)'}de{'NullAbgleich(Bit12)'}")]
    [ACPropertyEntity(113, "Bit13", "en{'MaxAdjustment(Bit13)'}de{'MaxAbgleich(Bit13)'}")]
    [ACPropertyEntity(114, "Bit14", "en{'Postpone(Bit14)'}de{'Verschieben(Bit14)'}")]
    [ACPropertyEntity(115, "Bit15", "en{'FaultAck(Bit15)'}de{'FaultAck(Bit15)'}")]
    public class GIPConv2006ScaleMaskReq : BitAccessForInt16
    {
        #region c'tors
        public GIPConv2006ScaleMaskReq()
        {
        }

        public GIPConv2006ScaleMaskReq(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        public bool Bit06_ResetZero
        {
            get { return Bit06; }
            set { Bit06 = value; }
        }

        public bool Bit07_SetZero
        {
            get { return Bit07; }
            set { Bit07 = value; }
        }

        public bool Bit08_Service
        {
            get { return Bit08; }
            set { Bit08 = value; }
        }

        public bool Bit09_OwnCommand
        {
            get { return Bit09; }
            set { Bit09 = value; }
        }

        public bool Bit10_AllDataReceived
        {
            get { return Bit10; }
            set { Bit10 = value; }
        }   

        public bool Bit11_DataTransfer
        {
            get { return Bit11; }
            set { Bit11 = value; }
        }

        public bool Bit12_ZeroAdjustment
        {
            get { return Bit12; }
            set { Bit12 = value; }
        }

        public bool Bit13_MaxAdjustment
        {
            get { return Bit13; }
            set { Bit13 = value; }
        }

        public bool Bit14_Postpone
        {
            get { return Bit14; }
            set { Bit14 = value; }
        }

        public bool Bit15_FaultAck
        {
            get { return Bit15; }
            set { Bit15 = value; }
        }

        #endregion
    }


    /// <summary>
    /// Baseclass for converting State and Types between Standard-Model-Components and DataAccess-/Vendor Model
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006Scale'}de{'GIPConv2006Scale'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPConv2006Scale : PAStateConverterBase
    {
        #region c'tors
        public GIPConv2006Scale(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _TareInternal = new ACPropertyConfigValue<bool>(this, "TareInternal", false);

            _DBNo = new ACPropertyConfigValue<UInt16>(this, nameof(DBNo), 0);
            _DBOffset = new ACPropertyConfigValue<UInt16?>(this, nameof(DBOffset), null);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _Response.ValueUpdatedOnReceival += Response_PropertyChanged;
            _Request.ValueUpdatedOnReceival += Request_PropertyChanged;
            bool tare = _TareInternal.ValueT;

            _ = DBNo;
            _ = DBOffset;

            return true;
        }

        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();

            CSData.ValueT = new GIPConvScaleData2006();

            BindMyProperties();

            return result;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            _Response.ValueUpdatedOnReceival -= Response_PropertyChanged;
            _Request.ValueUpdatedOnReceival -= Request_PropertyChanged;

            if (AllocatedByWay != null)
                (AllocatedByWay as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            AllocatedByWay = null;

            return await base.ACDeInit(deleteACClassTask);
        }

        protected bool _PropertiesBound = false;
        protected void BindMyProperties()
        {
            if (_PropertiesBound)
                return;
            _PropertiesBound = true;

            if (_Response.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
            {
                IACPropertyNetTarget newTarget = null;
                IACPropertyNetTarget oldTarget = _Response;
                if (BindProperty("Waagen_Stat", _Response, String.Format("W{0:000}_RM", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                {
                    OnReponsePropertyReplaced(oldTarget, newTarget);
                }
            }
            if (_Request.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
            {
                IACPropertyNetTarget newTarget = null;
                IACPropertyNetTarget oldTarget = _Request;
                if (BindProperty("Waagen_Komm", _Request, String.Format("W{0:000}_KM", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                {
                    OnRequestPropertyReplaced(oldTarget, newTarget);
                }
            }

            if (ParentACComponent != null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
            {
                PAEScaleBase controlModule = FindParentComponent<PAEScaleBase>(c => c is PAEScaleBase);
                if (controlModule != null)
                {
                    PAEScaleGravimetric scaleGravimetric = controlModule as PAEScaleGravimetric;
                    if (controlModule.DesiredWeight != null && (controlModule.DesiredWeight as IACPropertyNetTarget).Source == null)
                    {
                        IACPropertyNetTarget newTarget = null;
                        if (BindProperty("Waagen_Stat", controlModule.DesiredWeight as IACPropertyNetTarget, String.Format("W{0:000}_SOLL", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                        {
                        }
                    }
                    if (TareInternal)
                    {
                        if (scaleGravimetric != null && scaleGravimetric.ActualWeightExternal != null && (scaleGravimetric.ActualWeightExternal as IACPropertyNetTarget).Source == null)
                        {
                            IACPropertyNetTarget newTarget = null;
                            if (BindProperty("Waagen_Stat", scaleGravimetric.ActualWeightExternal as IACPropertyNetTarget, String.Format("W{0:000}_IST", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                            {
                            }
                            if ((scaleGravimetric.ActualWeightExternal as IACPropertyNetTarget).Source != null)
                                scaleGravimetric.IsVisibleExtActualWeight.ValueT = true;
                        }
                    }
                    else
                    {
                        if (controlModule.ActualWeight != null && (controlModule.ActualWeight as IACPropertyNetTarget).Source == null)
                        {
                            IACPropertyNetTarget newTarget = null;
                            if (BindProperty("Waagen_Stat", controlModule.ActualWeight as IACPropertyNetTarget, String.Format("W{0:000}_IST", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                            {
                            }
                        }

                        if (scaleGravimetric != null && scaleGravimetric.TareCounterReq != null && (scaleGravimetric.TareCounterReq as IACPropertyNetTarget).Source == null)
                        {
                            IACPropertyNetTarget newTarget = null;
                            if (BindProperty("Waagen_Stat", scaleGravimetric.TareCounterReq as IACPropertyNetTarget, String.Format("W{0:000}_TCOUNTSOLL", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                            {
                            }
                        }

                        if (scaleGravimetric != null && scaleGravimetric.TareCounterRes != null && (scaleGravimetric.TareCounterRes as IACPropertyNetTarget).Source == null)
                        {
                            IACPropertyNetTarget newTarget = null;
                            if (BindProperty("Waagen_Stat", scaleGravimetric.TareCounterRes as IACPropertyNetTarget, String.Format("W{0:000}_TCOUNTIST", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                            {
                            }
                        }
                    }
                    if (controlModule.ActualValue != null && (controlModule.ActualValue as IACPropertyNetTarget).Source == null)
                    {
                        IACPropertyNetTarget newTarget = null;
                        if (BindProperty("Waagen_Stat", controlModule.ActualValue as IACPropertyNetTarget, String.Format("W{0:000}_BRUTTO", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                        {
                        }
                    }

                    PAEScaleTotalizing totalizingScale = controlModule as PAEScaleTotalizing;
                    if (totalizingScale != null)
                    {
                        if (totalizingScale.TotalActualWeight != null && (totalizingScale.TotalActualWeight as IACPropertyNetTarget).Source == null)
                        {
                            IACPropertyNetTarget newTarget = null;
                            if (BindProperty("Waagen_Stat", totalizingScale.TotalActualWeight as IACPropertyNetTarget, String.Format("W{0:000}_GESIST", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                            {
                            }
                        }
                        if (totalizingScale.TotalDesiredWeight != null && (totalizingScale.TotalDesiredWeight as IACPropertyNetTarget).Source == null)
                        {
                            IACPropertyNetTarget newTarget = null;
                            if (BindProperty("Waagen_Stat", totalizingScale.TotalDesiredWeight as IACPropertyNetTarget, String.Format("W{0:000}_GESSOLL", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                            {
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Properties

        #region Configuration

        private ACPropertyConfigValue<bool> _TareInternal;
        [ACPropertyConfig("en{'Tare internal'}de{'Internes Tarieren'}")]
        public bool TareInternal
        {
            get
            {
                PAEScaleGravimetric gravimetricScale = ParentACComponent as PAEScaleGravimetric;
                if (gravimetricScale != null && gravimetricScale.TareInternal)
                    return gravimetricScale.TareInternal;
                return _TareInternal.ValueT;
            }
            set
            {
                _TareInternal.ValueT = value;
            }
        }

        [ACPropertyBindingSource(9999, "", "en{'Aggregate number'}de{'Aggregatnummer'}", "", false, false)]
        public IACContainerTNet<Int16> AggrNo { get; set; }

        private ACPropertyConfigValue<UInt16> _DBNo;
        [ACPropertyConfig("en{'Datablocknumber'}de{'DB-Nummer'}")]
        public UInt16 DBNo
        {
            get
            {

                UInt16 resultValue = _DBNo.ValueT;
                if (resultValue == 0 && AggrNo.ValueT > 0)
                    resultValue = 815;
                return resultValue;
            }
            set
            {
                _DBNo.ValueT = value;
            }
        }

        private ACPropertyConfigValue<UInt16?> _DBOffset;
        [ACPropertyConfig("en{'Startaddress in DB'}de{'Startadresse im DB'}")]
        public UInt16? DBOffset
        {
            get
            {
                if (!_DBOffset.ValueT.HasValue)
                {
                    if (AggrNo != null && AggrNo.ValueT > 0)
                    {
                        if (AggrNo.ValueT == 1)
                            return 0;
                        else
                            return Convert.ToUInt16((AggrNo.ValueT - 1) * (GIPSerialControllerPID2006.StructLen + GIPSerialControllerPID2006.DBReservLen));
                    }
                    return null;
                }
                else
                    return _DBOffset.ValueT;
            }
            set
            {
                _DBOffset.ValueT = value;
            }
        }

        #endregion

        #region Read-Values from PLC
        public IACContainerTNet<BitAccessForAllocatedByWay> AllocatedByWay { get; set; }

        public IACContainerTNet<PANotifyState> StateUL1 { get; set; }
        public IACContainerTNet<PANotifyState> StateUL2 { get; set; }
        public IACContainerTNet<PANotifyState> StateLL1 { get; set; }
        public IACContainerTNet<PANotifyState> StateLL2 { get; set; }

        public IACContainerTNet<PANotifyState> StateScale { get; set; }
        public IACContainerTNet<PANotifyState> StateTolerance { get; set; }
        public IACContainerTNet<PANotifyState> StateLackOfMaterial { get; set; }
        public IACContainerTNet<PANotifyState> StateDosingTime { get; set; }
        public IACContainerTNet<Boolean> IsDosing { get; set; }
        public IACContainerTNet<Boolean> IsRough { get; set; }
        public IACContainerTNet<Boolean> IsFine { get; set; }

        public IACContainerTNet<Boolean> IsDischarging { get; set; }
        public IACContainerTNet<Boolean> IsTared { get; set; }
        public IACContainerTNet<Boolean> ZeroSet { get; set; }
        public IACContainerTNet<Boolean> IsManualDosing { get; set; }
        public IACContainerTNet<Int32> TareCounterRes { get; set; }

        public IACContainerTNet<Boolean> IsPresignal { get; set; }
        public IACContainerTNet<Boolean> IsFlow { get; set; }


        [ACPropertyBindingTarget(240, "Read from PLC", "en{'Response'}de{'Rückmeldung'}", "", false, false)]
        public IACContainerTNet<GIPConv2006ScaleMaskRes> Response { get; set; }
        protected virtual IACPropertyNetTarget _Response
        {
            get
            {
                return (IACPropertyNetTarget)this.Response;
            }
        }
        protected virtual void OnReponsePropertyReplaced(IACPropertyNetTarget oldTarget, IACPropertyNetTarget newTarget)
        {
            if (oldTarget != null)
                oldTarget.ValueUpdatedOnReceival -= Response_PropertyChanged;
            if (newTarget != null)
                newTarget.ValueUpdatedOnReceival += Response_PropertyChanged;
        }
        protected virtual GIPConv2006ScaleMaskRes _ResponseValue
        {
            get { return Response.ValueT; }
        }

        [ACPropertyBindingTarget(241, "Read from PLC", "en{'Scale data'}de{'Scale Daten'}", "", false, true)]
        public IACContainerTNet<GIPConvScaleData2006> CSData { get; set; }

        #endregion

        #region Write-Values to PLC
        public IACContainerTNet<bool> FaultAckUL1 { get; set; }
        public IACContainerTNet<bool> FaultAckUL2 { get; set; }
        public IACContainerTNet<bool> FaultAckLL1 { get; set; }
        public IACContainerTNet<bool> FaultAckLL2 { get; set; }

        public IACContainerTNet<bool> FaultAckScale { get; set; }
        public IACContainerTNet<bool> FaultAckTolerance { get; set; }
        public IACContainerTNet<bool> FaultAckLackOfMaterial { get; set; }
        public IACContainerTNet<bool> FaultAckDosingTime { get; set; }

        public IACContainerTNet<Boolean> ReqZeroSet { get; set; }
        public IACContainerTNet<Boolean> ReqZeroReset { get; set; }
        public IACContainerTNet<Boolean> ReqIsTared { get; set; }
        public IACContainerTNet<Int32> TareCounterReq { get; set; }


        [ACPropertyBindingTarget(250, "Read from PLC", "en{'Request'}de{'Kommando'}", "", false, false)]
        public IACContainerTNet<GIPConv2006ScaleMaskReq> Request { get; set; }

        protected virtual IACPropertyNetTarget _Request
        {
            get
            {
                return (IACPropertyNetTarget)Request;
            }
        }
        protected virtual void OnRequestPropertyReplaced(IACPropertyNetTarget oldTarget, IACPropertyNetTarget newTarget)
        {
            if (oldTarget != null)
                oldTarget.ValueUpdatedOnReceival -= Request_PropertyChanged;
            if (newTarget != null)
                newTarget.ValueUpdatedOnReceival += Request_PropertyChanged;
        }

        protected virtual GIPConv2006ScaleMaskReq _RequestValue
        {
            get { return Request.ValueT; }
        }
        #endregion

        public bool IsReadyForSending
        {
            get
            {
                if (!this.Root.Initialized)
                    return false;
                if (IsSimulationOn)
                    return true;
                bool isReadyForSending = false;
                if (this.Session != null)
                {
                    isReadyForSending = true;
                    ACSession acSession = this.Session as ACSession;
                    if (acSession != null && !acSession.IsReadyForWriting)
                        isReadyForSending = false;
                    else if (acSession == null && !(bool)this.Session.ACUrlCommand(nameof(ACSession.IsReadyForWriting)))
                        isReadyForSending = false;
                }
                return isReadyForSending;
            }
        }

        //private bool _IsDataLoaded = false;

        #endregion

        #region Methods

        #region overridden methods
        protected override bool OnParentServerPropertyFound(IACPropertyNetServer parentProperty)
        {
            switch (parentProperty.ACIdentifier)
            {
                case nameof(IRoutableModule.AllocatedByWay):
                    AllocatedByWay = parentProperty as IACContainerTNet<BitAccessForAllocatedByWay>;
                    return true;
                case "StateUL1":
                    StateUL1 = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateUL2":
                    StateUL2 = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateLL1":
                    StateLL1 = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateLL2":
                    StateLL2 = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateScale":
                    StateScale = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateTolerance":
                    StateTolerance = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateLackOfMaterial":
                    StateLackOfMaterial = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateDosingTime":
                    StateDosingTime = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "IsDosing":
                    IsDosing = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsRough":
                    IsRough = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsFine":
                    IsFine = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsDischarging":
                    IsDischarging = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsTared":
                    IsTared = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsManualDosing":
                    IsManualDosing = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "ZeroSet":
                    ZeroSet = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsPresignal":
                    IsPresignal = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsFlow":
                    IsFlow = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "TareCounterRes":
                    TareCounterRes = parentProperty as IACContainerTNet<Int32>;
                    return true;
                case "FaultAckUL1":
                    FaultAckUL1 = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckUL2":
                    FaultAckUL2 = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckLL1":
                    FaultAckLL1 = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckLL2":
                    FaultAckLL2 = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckScale":
                    FaultAckScale = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckTolerance":
                    FaultAckTolerance = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckLackOfMaterial":
                    FaultAckLackOfMaterial = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckDosingTime":
                    FaultAckDosingTime = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqZeroSet":
                    ReqZeroSet = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqZeroReset":
                    ReqZeroReset = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqIsTared":
                    ReqIsTared = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "TareCounterReq":
                    TareCounterReq = parentProperty as IACContainerTNet<Int32>;
                    parentProperty.ForceBroadcast = true;
                    return true;
            }
            return false;
        }

        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if (!_LockResend_ReqRunState)
            {
                if (sender == FaultAckScale)
                {
                    _RequestValue.Bit15_FaultAck = FaultAckScale.ValueT;
                    return;
                }
                else if (sender == ReqZeroSet)
                {
                    _RequestValue.Bit07_SetZero = ReqZeroSet.ValueT;
                }
                else if (sender == ReqZeroReset)
                {
                    _RequestValue.Bit06_ResetZero = ReqZeroReset.ValueT;
                }
                else if (sender == ReqIsTared && !TareInternal)
                {
                    // This bit currently doesn't exist in GIP-PLC
                    //_RequestValue.Bit05_ReqIsTared = ReqIsTared.ValueT;
                }
            }
        }
        #endregion

        #region Methods => Calibration

        [ACMethodInfo("", "en{'1. Turn on service mode'}de{'1. Servicemodus aktivieren'}", 1000, true)]
        public void ServiceModeOn()
        {
            _RequestValue.Bit08_Service = true;
        }

        public bool IsServiceModeOn()
        {
            return !_RequestValue.Bit08_Service;
        }

        [ACMethodInfo("", "en{'Turn off service mode'}de{'Servicemodus deaktivieren'}", 1001, true)]
        public void ServiceModeOff()
        {
            _RequestValue.Bit08_Service = false;
        }

        public bool IsServiceModeOff()
        {
            return _RequestValue.Bit08_Service;
        }

        [ACMethodInfo("", "en{'2. Take data from scale module'}de{'2. Daten vom Wäge-Modul übernehmen'}", 1002, true)]
        public void TakeDataFromScaleModule()
        {
            _RequestValue.Bit10_AllDataReceived = true;
            LoadCSData();
        }

        public bool IsEnabledTakeDataFromScaleModule()
        {
            return _RequestValue.Bit08_Service;
        }

        [ACMethodInfo("", "en{'4. Accept entered data'}de{'4. Eingegebene Daten übernehmen'}", 1003, true)]
        public void AcceptEnteredData()
        {
            WriteCSData();
            _RequestValue.Bit11_DataTransfer = true;
        }

        public bool IsEnabledAcceptEnteredData()
        {
            return _RequestValue.Bit08_Service;
        }

        [ACMethodInfo("", "en{'5. Register empty scale'}de{'5. Leere Waage registrieren'}", 1004, true)]
        public void RegisterEmptyScale()
        {
            _RequestValue.Bit12_ZeroAdjustment = true;
        }

        public bool IsEnabledRegisterEmptyScale()
        {
            return _RequestValue.Bit08_Service;
        }

        [ACMethodInfo("", "en{'6. Register after filled scale'}de{'6. Nach Befüllen der Waage registrieren'}", 1005, true)]
        public void RegisterAfterFilledScale()
        {
            _RequestValue.Bit13_MaxAdjustment = true;
        }

        public bool IsEnabledRegisterAfterFilledScale()
        {
            return _RequestValue.Bit08_Service;
        }

        [ACMethodInfo("", "en{'Optional - Postpone process'}de{'Optional - verschieben'}", 1006, true)]
        public void PostponeProcess()
        {
            _RequestValue.Bit14_Postpone = true;
        }

        public bool IsEnabledPostponeProcess()
        {
            return _RequestValue.Bit08_Service;
        }

        #endregion

        #region Methods => ScaleData

        [ACMethodInteraction("", "en{'Load scale data'}de{'Waage Daten laden'}", 500, true, "", Global.ACKinds.MSMethod)]
        public virtual void LoadCSData()
        {
            if (!IsEnabledLoadCSData())
                return;

            GIPConvScaleData2006 cData = new GIPConvScaleData2006();
            cData = this.Session.ACUrlCommand("!ReadObject", cData, DBNo, DBOffset.Value, null) as GIPConvScaleData2006;
            if (cData != null)
            {
                CSData.ValueT = cData;
                //_IsDataLoaded = true;
            }
            else
            {
                Messages.LogError(this.GetACUrl(), "LoadCData()", "ReadObject failed. Could not read GIPConvScaleData2006");
            }

            if (CSData.ValueT == null)
            {
                // TODO: Alarm?
            }
        }

        public virtual bool IsEnabledLoadCSData()
        {
            return IsReadyForSending && DBNo > 0 && DBOffset.HasValue;
        }

        [ACMethodInteraction("", "en{'Write scale data'}de{'Waage Daten schreiben'}", 501, true, "", Global.ACKinds.MSMethod)]
        public virtual void WriteCSData()
        {
            if (!IsEnabledWriteCSData())
                return;

            // TODO: Validate Range of Values
            object sended = this.Session.ACUrlCommand("!SendObject", CSData.ValueT, null, DBNo, DBOffset.Value, null, null);
            if (sended == null || !((bool)sended))
            {
                // TODO: Alarm?
            }
        }

        public virtual bool IsEnabledWriteCSData()
        {
            return IsReadyForSending
                && DBNo > 0
                && DBOffset.HasValue
                && CSData.ValueT != null;
        }

        #endregion

        #endregion

        #region Converter-Logic
        protected virtual void Response_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                if (AllocatedByWay != null) AllocatedByWay.ValueT.Bit01_Allocated = _ResponseValue.Bit14_AllocatedByWay;
                if (StateUL2 != null)
                {
                    if (_ResponseValue.Bit03_Full)
                        StateUL2.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateUL2.ValueT = PANotifyState.Off;
                }
                if (StateLL2 != null)
                {
                    if (_ResponseValue.Bit00_Empty)
                        StateLL2.ValueT = PANotifyState.InfoOrActive;
                    else
                        StateLL2.ValueT = PANotifyState.Off;
                }
                if (StateScale != null)
                {
                    if (_ResponseValue.Bit15_Fault)
                        StateScale.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateScale.ValueT = PANotifyState.Off;
                }
                if (StateTolerance != null)
                {
                    if (_ResponseValue.Bit13_Tolerance)
                        StateTolerance.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateTolerance.ValueT = PANotifyState.Off;
                }
                if (StateLackOfMaterial != null)
                {
                    if (_ResponseValue.Bit08_LackOfMaterial)
                        StateLackOfMaterial.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateLackOfMaterial.ValueT = PANotifyState.Off;
                }
                if (StateDosingTime != null)
                {
                    if (_ResponseValue.Bit05_Dosingtime)
                        StateDosingTime.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateDosingTime.ValueT = PANotifyState.Off;
                }
                if (IsDosing != null) IsDosing.ValueT = _ResponseValue.Bit11_Dosing;
                if (IsRough != null) IsRough.ValueT = _ResponseValue.Bit01_Rough;
                if (IsFine != null) IsFine.ValueT = _ResponseValue.Bit02_Fine;

                if (IsDischarging != null) IsDischarging.ValueT = _ResponseValue.Bit10_Discharging;
                if (IsTared != null && !TareInternal) IsTared.ValueT = _ResponseValue.Bit12_Tared;
                if (IsManualDosing != null) IsManualDosing.ValueT = _ResponseValue.Bit06_Manual;
                if (IsPresignal != null) IsPresignal.ValueT = _ResponseValue.Bit09_Presignal;
                if (IsFlow != null) IsFlow.ValueT = _ResponseValue.Bit04_Flow;
            }
        }

        private bool _LockResend_ReqRunState = false;
        protected virtual void Request_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                _LockResend_ReqRunState = true;
                if (ReqZeroSet != null && ReqZeroReset != null && ZeroSet != null)
                { 
                    ReqZeroSet.ValueT = _RequestValue.Bit07_SetZero;
                    ReqZeroReset.ValueT = _RequestValue.Bit06_ResetZero;
                    if (ReqZeroSet.ValueT && !ReqZeroReset.ValueT)
                        ZeroSet.ValueT = true;
                    else
                        ZeroSet.ValueT = false;
                    // This bit currently doesn't exist in GIP-PLC
                    //ReqIsTared.ValueT = _RequestValue.Bit05_ReqIsTared;
                }
                _LockResend_ReqRunState = false;
            }
        }
        #endregion
    }

    #region ConvScaleData

    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'Scale converter data'}de{'Scale converter Daten'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class GIPConvScaleData2006 : INotifyPropertyChanged, IACObject
    {
        public GIPConvScaleData2006()
        {
        }

        #region ScaleProperties
        
        [IgnoreDataMember]
        private double _MSW;
        /// <summary>
        /// MaxScaleWeight
        /// </summary>
        [DataMember]
        [ACPropertyInfo(1, "", "en{'3. Max scale weight'}de{'3. MaxWaegebereich'}")]
        public double MSW
        {
            get
            {
                return _MSW;
            }
            set
            {
                _MSW = value;
                OnPropertyChanged("MSW");
            }
        }

        [IgnoreDataMember]
        private double _DW;
        /// <summary>
        /// DigitWeight
        /// </summary>
        [DataMember]
        [ACPropertyInfo(2, "", "en{'3. Digit/Precision [g]'}de{'3. Teilung/Ziffernschritt [g]'}")]
        public double DW
        {
            get
            {
                return _DW;
            }
            set
            {
                _DW = value;
                OnPropertyChanged("DW");
            }
        }

        [IgnoreDataMember]
        private double _CW;
        /// <summary>
        /// AdjustmentWeight
        /// </summary>
        [DataMember]
        [ACPropertyInfo(3, "", "en{'3. Calibrate weight'}de{'3. Justagegewicht'}")]
        public double CW
        {
            get
            {
                return _CW;
            }
            set
            {
                _CW = value;
                OnPropertyChanged();
            }
        }


        [IgnoreDataMember]
        private short _OC;
        /// <summary>
        /// OwnCommand
        /// </summary>
        [DataMember]
        [ACPropertyInfo(4, "", "en{'Own command'}de{'EigenerBefehl'}")]
        public short OC
        {
            get
            {
                return _OC;
            }
            set
            {
                _OC = value;
                OnPropertyChanged("OC");
            }
        }


        [IgnoreDataMember]
        private short _R1;
        /// <summary>
        /// Reserve1
        /// </summary>
        [DataMember]
        [ACPropertyInfo(5, "", "en{'Reserve 1'}de{'Reserve 1'}")]
        public short R1
        {
            get
            {
                return _R1;
            }
            set
            {
                _R1 = value;
                OnPropertyChanged("R1");
            }
        }

        [IgnoreDataMember]
        private double _R0;
        /// <summary>
        /// Reserve
        /// </summary>
        [DataMember]
        [ACPropertyInfo(6, "", "en{'Reserve'}de{'Reserve'}")]
        public double R0
        {
            get
            {
                return _R0;
            }
            set
            {
                _R0 = value;
                OnPropertyChanged("R0");
            }
        }

        public IACObject ParentACObject => null;

        public IACType ACType => this.ReflectACType();

        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        public string ACIdentifier => this.ReflectGetACIdentifier();

        public string ACCaption => this.ACIdentifier;

        #endregion

        #region Methods

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
        }

        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ReflectGetACUrl(rootACObject);
        }

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        public bool ACUrlTypeInfo(string acUrl, ref ACUrlTypeInfo acUrlTypeInfo)
        {
            return this.ReflectACUrlTypeInfo(acUrl, ref acUrlTypeInfo);
        }
        #endregion
    }

    #endregion

    #region Serializer
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'Serializer for Scale converter'}de{'Serialisierer für Scale converter'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPSerialConvScale2006 : ACSessionObjSerializer
    {
        #region c´tors
        public GIPSerialConvScale2006(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion

        public const int StructLen = 20;

        [ACPropertyInfo(true, 500)]
        public short SerializerBehaviour
        {
            get; set;
        }

        private readonly string _ConverterTypeName = typeof(GIPConvScaleData2006).FullName;
        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            return typeOrACMethodName == _ConverterTypeName;
        }

        public override bool SendObject(object complexObj, object prevComplexObj, int dbNo, int offset, int? routeOffset, object miscParams)
        {
            S7TCPSession s7Session = ParentACComponent as S7TCPSession;
            if (s7Session == null || complexObj == null)
                return false;
            if (!s7Session.PLCConn.IsConnected)
                return false;
            GIPConvScaleData2006 request = complexObj as GIPConvScaleData2006;
            if (request == null)
                return false;
            byte[] sendPackage1 = new byte[StructLen];
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.MSW), 0, sendPackage1, 0, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.DW), 0, sendPackage1, 4, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.CW), 0, sendPackage1, 8, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.OC), 0, sendPackage1, 12, 2);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.R1), 0, sendPackage1, 14, 2);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.R0), 0, sendPackage1, 16, 4);

            PLC.Result errCode = s7Session.PLCConn.WriteBytes(DataTypeEnum.DataBlock, dbNo, offset, ref sendPackage1);
            return errCode == null || errCode.IsSucceeded;
        }

        public override object ReadObject(object complexObj, int dbNo, int offset, int? routeOffset, object miscParams)
        {
            S7TCPSession s7Session = ParentACComponent as S7TCPSession;
            if (s7Session == null || complexObj == null)
                return null;
            if (!s7Session.PLCConn.IsConnected)
                return null;
            GIPConvScaleData2006 response = complexObj as GIPConvScaleData2006;
            if (response == null)
                return null;

            byte[] readPackage1 = new byte[StructLen];
            PLC.Result errCode = s7Session.PLCConn.ReadBytes(DataTypeEnum.DataBlock, dbNo, offset, StructLen, out readPackage1);
            if (errCode != null && !errCode.IsSucceeded)
                return null;

            response.MSW = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 0);
            response.DW = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 4);
            response.CW = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 8);
            response.OC = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, 12);
            response.R1 = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, 14);
            response.R0 = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 16);

            return response;
        }
    }
    #endregion

}
