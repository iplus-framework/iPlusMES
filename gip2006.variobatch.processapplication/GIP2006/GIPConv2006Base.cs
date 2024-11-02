// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using System.Runtime.Serialization;
using gip.core.processapplication;
using gip.core.communication;

namespace gip2006.variobatch.processapplication
{
    public static class ConstGIP2006
    {
        public const string PackName_VarioGIP2006 = "gip.VarioGIP2006";
    }

    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006BaseMaskRes'}de{'Bitzugriff GIPConv2006BaseMaskRes'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(106, "Bit23", "en{'On site maintenance (Bit23/1.7)'}de{'Wartungsschalter (Bit23/1.7)'}")]
    [ACPropertyEntity(107, "Bit07", "en{'On site switch auto (Bit07/3.7)'}de{'Vorortschalter Auto (Bit07/3.7)'}")]
    [ACPropertyEntity(108, "Bit08", "en{'Allocated by way (Bit08/2.0)'}de{'Belegt durch Wegesteuerung (Bit08/2.0)'}")]
    [ACPropertyEntity(110, "Bit10", "en{'Turn on interlock (Bit10/2.2'}de{'Einschaltverriegelung (Bit10/2.2)'}")]
    [ACPropertyEntity(111, "Bit11", "en{'On site switch on (Bit11/2.3)'}de{'Vorortschalter (Bit11/2.3)'}")]
    [ACPropertyEntity(114, "Bit14", "en{'Manual mode (Bit14/2.6)'}de{'Handbetrieb (Bit14/2.6)'}")]
    [ACPropertyEntity(115, "Bit15", "en{'Common fault (Bit15/2.7)'}de{'Störung allgemein (Bit15/2.7)'}")]
    [ACPropertyEntity(120, "Bit20", "en{'Error extern (Bit20/1.4)'}de{'Störung Extern (Bit20/1.4)'}")]
    [ACPropertyEntity(130, "Bit30", "en{'Response error (Bit30/0.6)'}de{'Störung Rückmeldung (Bit30/0.6)'}")]
    public class GIPConv2006BaseMaskRes : BitAccessForInt32
    {
        #region c'tors
        public GIPConv2006BaseMaskRes()
        {
        }

        public GIPConv2006BaseMaskRes(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits
        public bool Bit23_OnSiteMaintenance
        {
            get { return Bit23; }
            set { Bit23 = value; }
        }

        public bool Bit07_OnSiteTurnedAuto
        {
            get { return Bit07; }
            set { Bit07 = value; }
        }

        public bool Bit08_AllocatedByWay
        {
            get { return Bit08; }
            set { Bit08 = value; }
        }

        public bool Bit10_TurnOnInterlock
        {
            get { return Bit10; }
            set { Bit10 = value; }
        }

        public bool Bit11_OnSiteTurnedOn
        {
            get { return Bit11; }
            set { Bit11 = value; }
        }

        public bool Bit14_Manual
        {
            get { return Bit14; }
            set { Bit14 = value; }
        }

        public bool Bit15_Fault
        {
            get { return Bit15; }
            set { Bit15 = value; }
        }

        public bool Bit20_FaultExtern
        {
            get { return Bit20; }
            set { Bit20 = value; }
        }

        public bool Bit30_FaultResponse
        {
            get { return Bit30; }
            set { Bit30 = value; }
        }

        #endregion

        #region Properties

        public virtual bool IsAnyBitSetted
        {
            get
            {
                return ValueT > 0;
            }
        }

        #endregion          
    }

    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006BaseMaskReq'}de{'Bitzugriff GIPConv2006BaseMaskReq'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(106, "Bit06", "en{'Manual mode (Bit06)'}de{'Handbetrieb (Bit06)'}")]
    [ACPropertyEntity(115, "Bit15", "en{'Fault acknowledge (Bit15)'}de{'Störungsquittung (Bit15)'}")]
    public class GIPConv2006BaseMaskReq : BitAccessForInt32
    {
        #region c'tors
        public GIPConv2006BaseMaskReq()
        {
        }

        public GIPConv2006BaseMaskReq(IACType acValueType)
            : base(acValueType)
        {
        }

        #endregion

        #region Customized Bits
        public bool Bit06_Manual
        {
            get { return Bit06; }
            set { Bit06 = value; }
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
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006Base'}de{'GIPConv2006Base'}", Global.ACKinds.TACAbstractClass, Global.ACStorableTypes.NotStorable, false, true)]
    public abstract class GIPConv2006Base : PAStateConverterBase, IGIPConvComp4MUX, IRouteItemIDProvider
    {
        #region c'tors
        public GIPConv2006Base(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _Response.ValueUpdatedOnReceival += Response_PropertyChanged;
            _Request.ValueUpdatedOnReceival += Request_PropertyChanged;

            //IACComponent TMP = (IACComponent)ACUrlCommand("\\DataAccess\\S7Service\\PLC1\\AGG_SOLL");

            //_Request.Source = (IACPropertyNetSource)TMP.GetMember("A" + this.AggrNo.ToString()); // ACUrlCommand("\\DataAccess\\S7Service\\PLC1\\AGG_SOLL\\A" + this.AggrNo.ToString());

            return true;
        }

        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();

            BindMyProperties();

            return result;
        }

        public override void RestoreTargetProp(bool onlyUnbound = true)
        {
            base.RestoreTargetProp(onlyUnbound);
            //BindMyProperties();
        }

        protected bool _PropertiesBound = false;
        protected virtual void BindMyProperties()
        {
            if (_PropertiesBound)
                return;
            if (_Response.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
            {
                IACPropertyNetTarget newTarget = null;
                IACPropertyNetTarget oldTarget = _Response;
                if (BindProperty("AGG_STAT", _Response, String.Format("A{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                {
                    OnReponsePropertyReplaced(oldTarget, newTarget);
                }
            }
            if (_Request.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
            {
                IACPropertyNetTarget newTarget = null;
                IACPropertyNetTarget oldTarget = _Request;
                if (BindProperty("AGG_KOMM", _Request, String.Format("A{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                {
                    OnRequestPropertyReplaced(oldTarget, newTarget);
                }
            }
            if (ParentACComponent != null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
            {
                PAEControlModuleBase controlModule = FindParentComponent<PAEControlModuleBase>(c => c is PAEControlModuleBase);
                if (controlModule != null && controlModule.RunningTime != null && (controlModule.RunningTime as IACPropertyNetTarget).Source == null)
                {
                    IACPropertyNetTarget newTarget = null;
                    if (BindProperty("AGG_ZEIT", controlModule.RunningTime as IACPropertyNetTarget, String.Format("A{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                    {
                    }
                }
            }
            _PropertiesBound = true;
        }


        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _Response.ValueUpdatedOnReceival -= Response_PropertyChanged;
            _Request.ValueUpdatedOnReceival -= Request_PropertyChanged;

            if (FaultState != null)
                (FaultState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            FaultState = null;

            if (AllocatedByWay != null)
                (AllocatedByWay as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            AllocatedByWay = null;

            if (OnSiteTurnedOn != null)
                (OnSiteTurnedOn as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            OnSiteTurnedOn = null;

            if (IsTriggered != null)
                (IsTriggered as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            IsTriggered = null;

            if (TurnOnInterlock != null)
                (TurnOnInterlock as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            TurnOnInterlock = null;

            if (OperatingMode != null)
                (OperatingMode as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            OperatingMode = null;

            if (FaultStateACK != null)
                (FaultStateACK as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            FaultStateACK = null;

            if (ReqOperatingMode != null)
                (ReqOperatingMode as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ReqOperatingMode = null;

            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Properties

        #region Alarms
        [ACPropertyBindingSource(301, "Read from PLC", "en{'Unbound alarm'}de{'Ungebundener Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> UnboundAlarm { get; set; }
        #endregion

        #region Configuration
        //[ACPropertyInfo(9999, DefaultValue = 0, ForceBroadcast = true)]
        //public Int16 AggrNo
        //{
        //    get;
        //    set;
        //}

        [ACPropertyBindingSource(9999, "", "en{'Aggregate number'}de{'Aggregatnummer'}", "", false, false)]
        public IACContainerTNet<Int16> AggrNo { get; set; }

        public string RouteItemID 
        {
            get
            {
                return RouteItemIDAsNum != 0 ? RouteItemIDAsNum.ToString() : null;
            }
        }

        public int RouteItemIDAsNum 
        {
            get
            {
                return AggrNo.ValueT;
            }
        }

        #endregion

        #region Read-Values from PLC
        public IACContainerTNet<PANotifyState> FaultState { get; set; }
        public IACContainerTNet<BitAccessForAllocatedByWay> AllocatedByWay { get; set; }
        public IACContainerTNet<ushort> OnSiteTurnedOn { get; set; }
        public IACContainerTNet<Boolean> TurnOnInterlock { get; set; }
        public IACContainerTNet<Global.OperatingMode> OperatingMode { get; set; }
        public IACContainerTNet<Boolean> IsTriggered { get; set; }
        #endregion

        #region Write-Value to PLC
        public IACContainerTNet<bool> FaultStateACK { get; set; }
        public IACContainerTNet<Global.OperatingMode> ReqOperatingMode { get; set; }
        #endregion

        #region Read-Values from PLC
        public bool HasConnectedSession
        {
            get
            {
                if (this.Session != null)
                {
                    ACSession acSession = this.Session as ACSession;
                    if (acSession != null)
                        return acSession.IsConnected.ValueT;
                    else
                        return (bool)this.Session.ACUrlCommand("IsConnected");
                }
                return false;
            }
        }

        protected abstract IACPropertyNetTarget _Response { get; }
        protected virtual void OnReponsePropertyReplaced(IACPropertyNetTarget oldTarget, IACPropertyNetTarget newTarget)
        {
            if (oldTarget != null)
                oldTarget.ValueUpdatedOnReceival -= Response_PropertyChanged;
            if (newTarget != null)
                newTarget.ValueUpdatedOnReceival += Response_PropertyChanged;
        }

        protected abstract GIPConv2006BaseMaskRes _ResponseValue { get; }

        protected virtual void Response_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                if (AllocatedByWay != null) AllocatedByWay.ValueT.Bit01_Allocated = _ResponseValue.Bit08_AllocatedByWay;
                if (TurnOnInterlock != null) TurnOnInterlock.ValueT = _ResponseValue.Bit10_TurnOnInterlock;
                if (OnSiteTurnedOn != null)
                {
                    if (!_ResponseValue.Bit07_OnSiteTurnedAuto && !_ResponseValue.Bit11_OnSiteTurnedOn)
                        OnSiteTurnedOn.ValueT = 0;
                    else if (_ResponseValue.Bit07_OnSiteTurnedAuto && !_ResponseValue.Bit11_OnSiteTurnedOn)
                        OnSiteTurnedOn.ValueT = 1;
                    else if (!_ResponseValue.Bit07_OnSiteTurnedAuto && _ResponseValue.Bit11_OnSiteTurnedOn)
                        OnSiteTurnedOn.ValueT = 2;
                    else //if (_ResponseValue.Bit07_OnSiteTurnedAuto && _ResponseValue.Bit11_OnSiteTurnedOn)
                        OnSiteTurnedOn.ValueT = 3;
                }
                if (OperatingMode != null)
                {
                    if (!_ResponseValue.IsAnyBitSetted && !IsSimulationOn && HasConnectedSession)
                        OperatingMode.ValueT = Global.OperatingMode.Inactive;
                    else if (_ResponseValue.Bit14_Manual && !_ResponseValue.Bit23_OnSiteMaintenance)
                        OperatingMode.ValueT = Global.OperatingMode.Manual;
                    else if (_ResponseValue.Bit23_OnSiteMaintenance)
                        OperatingMode.ValueT = Global.OperatingMode.Maintenance;
                    else
                        OperatingMode.ValueT = Global.OperatingMode.Automatic;
                }
                if (FaultState != null)
                {
                    if (_ResponseValue.Bit15_Fault)
                        FaultState.ValueT = PANotifyState.AlarmOrFault;
                    else
                        FaultState.ValueT = PANotifyState.Off;
                }
            }
        }
        #endregion
        
        #region Write-Values from PLC
        protected abstract IACPropertyNetTarget _Request { get; }
        protected virtual void OnRequestPropertyReplaced(IACPropertyNetTarget oldTarget, IACPropertyNetTarget newTarget)
        {
            if (oldTarget != null)
                oldTarget.ValueUpdatedOnReceival -= Request_PropertyChanged;
            if (newTarget != null)
                newTarget.ValueUpdatedOnReceival += Request_PropertyChanged;
        }

        protected abstract GIPConv2006BaseMaskReq _RequestValue { get; }

        public virtual void OnSetRequest(IACPropertyNetValueEvent valueEvent)
        {
        }

        protected virtual void Request_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
        }
        #endregion
        #endregion

        #region overridden methods
        protected override bool OnParentServerPropertyFound(IACPropertyNetServer parentProperty)
        {
            switch (parentProperty.ACIdentifier)
            {
                case "FaultState":
                    FaultState = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case nameof(IRoutableModule.AllocatedByWay):
                    AllocatedByWay = parentProperty as IACContainerTNet<BitAccessForAllocatedByWay>;
                    return true;
                case "OnSiteTurnedOn":
                    OnSiteTurnedOn = parentProperty as IACContainerTNet<ushort>;
                    return true;
                case "TurnOnInterlock":
                    TurnOnInterlock = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsTriggered":
                    IsTriggered = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "OperatingMode":
                    OperatingMode = parentProperty as IACContainerTNet<Global.OperatingMode>;
                    return true;
                case "ReqOperatingMode":
                    ReqOperatingMode = parentProperty as IACContainerTNet<Global.OperatingMode>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultStateACK":
                    FaultStateACK = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
            }
            return false;
        }

        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if (sender == FaultStateACK)
            {
                if (FaultStateACK.ValueT == true)
                    _RequestValue.Bit15_FaultAck = FaultStateACK.ValueT;
                return;
            }
            else if (sender == ReqOperatingMode)
            {
                if ((ReqOperatingMode.ValueT == Global.OperatingMode.Manual) || (ReqOperatingMode.ValueT == Global.OperatingMode.Maintenance))
                {
                    _RequestValue.Bit06_Manual = true;
                }
                else
                {
                    _RequestValue.Bit06_Manual = false;
                }
                return;
            }
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "SelectControlModule":
                    SelectControlModule();
                    return true;
                case "ResetStatistic":
                    ResetStatistic();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region pulic Methods
        [ACMethodInfo("", "en{'Select control module to multiplex'}de{'Auswahl des zu multiplexenden Control-Modul'}", 301, false, Global.ACKinds.MSMethodPrePost)]
        public void SelectControlModule()
        {
            if (!PreExecute("SelectControlModule"))
                return;
            IACPropertyNetSource sourcePropDA = (_Response as IACPropertyNetTarget).Source;
            if (sourcePropDA != null)
            {
                if (   (sourcePropDA.ParentACComponent != null) // Sollte ACSubscription sein
                    && (sourcePropDA.ParentACComponent.ParentACComponent != null)) // Sollte ACSession sein
                {
                    sourcePropDA.ParentACComponent.ParentACComponent.ACUrlCommand("GIPConfigMUX!SelectControlModule", ParentACComponent.GetACUrl());
                }
            }
            PostExecute("SelectControlModule");
        }

        [ACMethodInteraction("", "en{'Reset statistic data'}de{'Reset statistic data'}", 302, true)]
        public void ResetStatistic()
        {
            IACPropertyNetSource sourcePropDA = (_Response as IACPropertyNetTarget).Source;
            if (sourcePropDA != null)
            {
                if ((sourcePropDA.ParentACComponent != null) // Sollte ACSubscription sein
                    && (sourcePropDA.ParentACComponent.ParentACComponent != null)) // Sollte ACSession sein
                {
                    SelectControlModule();
                    sourcePropDA.ParentACComponent.ParentACComponent.ACUrlCommand("GIPConfigMUX!Reset", ParentACComponent.GetACUrl());
                }
            }
        }

        #endregion
    }
}
