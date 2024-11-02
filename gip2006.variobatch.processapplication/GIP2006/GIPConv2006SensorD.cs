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

namespace gip2006.variobatch.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006SensorDMaskRes'}de{'Bitzugriff GIPConv2006SensorDMaskRes'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit08", "en{'Input direct (Bit00)'}de{'Eingang direkt (Bit00)'}")]
    [ACPropertyEntity(101, "Bit09", "en{'Sensor signal (Bit01)'}de{'Sensorsignal (Bit01)'}")]
    [ACPropertyEntity(102, "Bit10", "en{'Internal Signal in PLC (Bit02)'}de{'Internal Signal in SPS (Bit02)'}")]
    public class GIPConv2006SensorDMaskRes : BitAccessForInt16
    {
        #region c'tors
        public GIPConv2006SensorDMaskRes()
        {
        }

        public GIPConv2006SensorDMaskRes(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits
        public bool Bit00_IOSignal
        {
            get { return Bit08; }
            set { Bit08 = value; }
        }

        public bool Bit01_SignalDisplay
        {
            get { return Bit09; }
            set { Bit09 = value; }
        }

        public bool Bit02_SignalInPLC
        {
            get { return Bit10; }
            set { Bit10 = value; }
        }
        #endregion
    }

    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006SensorDMaskReq'}de{'Bitzugriff GIPConv2006SensorDMaskReq'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit08", "en{'Negate Input (Bit00)'}de{'Negiere Eingang (Bit00)'}")]
    [ACPropertyEntity(101, "Bit09", "en{'Collective Acknowledgement (Bit01)'}de{'Über Sammelquittung quittierbar (Bit01)'}")]
    [ACPropertyEntity(102, "Bit10", "en{'Acknowledgment obligatory (Bit02)'}de{'Quittierpflichtig (Bit02)'}")]
    [ACPropertyEntity(103, "Bit11", "en{'Acknowledge (Bit03)'}de{'Quittung (Bit03)'}")]
    [ACPropertyEntity(104, "Bit12", "en{'Ignore delay times (Bit04)'}de{'Verzögerungszeiten ignorieren (Bit04)'}")]
    [ACPropertyEntity(105, "Bit13", "en{'Force PLC-Signal FALSE (Bit05)'}de{'Erzwinge SPS-Signal FALSCH  (Bit05)'}")]
    [ACPropertyEntity(106, "Bit14", "en{'Force PLC-Signal TRUE (Bit06)'}de{'Erzwinge SPS-Signal WAHR (Bit06)'}")]
    public class GIPConv2006SensorDMaskReq : BitAccessForInt16
    {
        #region c'tors
        public GIPConv2006SensorDMaskReq()
        {
        }

        public GIPConv2006SensorDMaskReq(IACType acValueType)
            : base(acValueType)
        {
        }

        #endregion

        #region Customized Bits
        public bool Bit00_NegateIO
        {
            get { return Bit08; }
            set { Bit08 = value; }
        }

        public bool Bit01_CollectiveAck
        {
            get { return Bit09; }
            set { Bit09 = value; }
        }

        public bool Bit02_AckObligatory
        {
            get { return Bit10; }
            set { Bit10 = value; }
        }

        public bool Bit03_Acknowledge
        {
            get { return Bit11; }
            set { Bit11 = value; }
        }

        public bool Bit04_IgnoreDelayTimes
        {
            get { return Bit12; }
            set { Bit12 = value; }
        }

        public bool Bit05_ForcePLC_Low
        {
            get { return Bit13; }
            set { Bit13 = value; }
        }

        public bool Bit06_ForcePLC_High
        {
            get { return Bit14; }
            set { Bit14 = value; }
        }

        #endregion
    }


    /// <summary>
    /// Baseclass for converting State and Types between Standard-Model-Components and DataAccess-/Vendor Model
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006SensorD'}de{'GIPConv2006SensorD'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, true)]
    public class GIPConv2006SensorD : PAStateConverterBase, IGIPConvComp4MUX
    {
        #region c'tors
        public GIPConv2006SensorD(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            (Response as IACPropertyNetTarget).ValueUpdatedOnReceival += Response_PropertyChanged;
            (Request as IACPropertyNetTarget).ValueUpdatedOnReceival += Request_PropertyChanged;

            //(DelayMSecSignalOn as IACPropertyNetTarget).ValueUpdatedOnReceival += ConverterProperty_ValueUpdatedOnReceival;
            //(DelayMSecSignalOff as IACPropertyNetTarget).ValueUpdatedOnReceival += ConverterProperty_ValueUpdatedOnReceival;
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
            if ((Response as IACPropertyNetTarget).Source == null && AggrNo != null && AggrNo.ValueT > 1000 && Session != null)
            {
                IACPropertyNetTarget newTarget = null;
                IACPropertyNetTarget oldTarget = Response as IACPropertyNetTarget;
                if (BindProperty("SENS_STAT", oldTarget, String.Format("M{0:000}", AggrNo.ValueT - 1000), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                {
                    OnReponsePropertyReplaced(oldTarget, newTarget);
                }
            }
            if ((Request as IACPropertyNetTarget).Source == null && AggrNo != null && AggrNo.ValueT > 1000 && Session != null)
            {
                IACPropertyNetTarget newTarget = null;
                IACPropertyNetTarget oldTarget = Request as IACPropertyNetTarget;
                if (BindProperty("SENS_KOMM", oldTarget, String.Format("M{0:000}", AggrNo.ValueT - 1000), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                {
                    OnRequestPropertyReplaced(oldTarget, newTarget);
                }
            }
            //if ((this.DelayMSecSignalOn as IACPropertyNetTarget).Source == null && AggrNo > 0 && Session != null)
            //    BindProperty("SENS_KOMM", DelayMSecSignalOn as IACPropertyNetTarget, String.Format("M{0:000}DelayOn", AggrNo));
            //if ((this.DelayMSecSignalOff as IACPropertyNetTarget).Source == null && AggrNo > 0 && Session != null)
            //    BindProperty("SENS_KOMM", DelayMSecSignalOff as IACPropertyNetTarget, String.Format("M{0:000}DelayOff", AggrNo));
            _PropertiesBound = true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            (Response as IACPropertyNetTarget).ValueUpdatedOnReceival -= Response_PropertyChanged;
            (Request as IACPropertyNetTarget).ValueUpdatedOnReceival -= Request_PropertyChanged;
            //(DelayMSecSignalOn as IACPropertyNetTarget).ValueUpdatedOnReceival -= ConverterProperty_ValueUpdatedOnReceival;
            //(DelayMSecSignalOff as IACPropertyNetTarget).ValueUpdatedOnReceival -= ConverterProperty_ValueUpdatedOnReceival;

            if (SensorState != null)
                (SensorState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            SensorState = null;

            if (SensorRole != null)
                (SensorRole as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            SensorRole = null;

            if (IgnoreDelayTimes != null)
                (IgnoreDelayTimes as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            IgnoreDelayTimes = null;

            if (CollectiveAck != null)
                (CollectiveAck as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            CollectiveAck = null;

            if (ForceSensorSignal != null)
                (ForceSensorSignal as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ForceSensorSignal = null;

            if (ActiveLow != null)
                (ActiveLow as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ActiveLow = null;

            if (DelayTimeSignalOff != null)
                (DelayTimeSignalOff as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            DelayTimeSignalOff = null;

            if (DelayTimeSignalOn != null)
                (DelayTimeSignalOn as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            DelayTimeSignalOn = null;

            if (OperatingMode != null)
                (OperatingMode as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            OperatingMode = null;

            if (FaultACK != null)
                (FaultACK as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            FaultACK = null;

            if (ReqOperatingMode != null)
                (ReqOperatingMode as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ReqOperatingMode = null;

            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Properties
        private bool _LockResend = false;
        private bool _LockReread = false;

        #region Configuration
        //[ACPropertyInfo(9999, DefaultValue = 0, ForceBroadcast = true)]
        //public Int16 AggrNo
        //{
        //    get;
        //    set;
        //}

        [ACPropertyBindingSource(9999, "", "en{'Aggregate number'}de{'Aggregatnummer'}", "", false, false)]
        public IACContainerTNet<Int16> AggrNo { get; set; }

        #endregion

        #region Read-Values from PLC
        public IACContainerTNet<TimeSpan> DelayTimeSignalOn { get; set; }
        public IACContainerTNet<TimeSpan> DelayTimeSignalOff { get; set; }
        public IACContainerTNet<bool> ActiveLow { get; set; }
        //public IACContainerTNet<PAESensorRole> SensorRole { get; set; }
        public IACContainerTNet<bool> ForceSensorSignal { get; set; }
        public IACContainerTNet<bool> CollectiveAck { get; set; }
        public IACContainerTNet<bool> IgnoreDelayTimes { get; set; }
        public IACContainerTNet<Global.OperatingMode> OperatingMode { get; set; }
        public IACContainerTNet<PANotifyState> SensorState { get; set; }
        public IACContainerTNet<PAESensorRole> SensorRole { get; set; }
        #endregion

        #region Write-Value to PLC
        public IACContainerTNet<bool> FaultACK { get; set; }
        public IACContainerTNet<Global.OperatingMode> ReqOperatingMode { get; set; }
        #endregion

        #region Read-Values from PLC
        [ACPropertyBindingTarget(240, "Read from PLC", "en{'Response'}de{'Rückmeldung'}", "", false, false)]
        public IACContainerTNet<GIPConv2006SensorDMaskRes> Response { get; set; }
        protected virtual void OnReponsePropertyReplaced(IACPropertyNetTarget oldTarget, IACPropertyNetTarget newTarget)
        {
            if (oldTarget != null)
                (oldTarget as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            if (newTarget != null)
                (newTarget as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
        }
        protected GIPConv2006SensorDMaskRes ResponseValue
        {
            get { return Response.ValueT; }
        }


        protected virtual void Response_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                _LockResend = true;
                try
                {
                    if (SensorState != null)
                    {
                        var newState = ResponseValue.Bit01_SignalDisplay ? PANotifyState.InfoOrActive : PANotifyState.Off;
                        bool changed = SensorState.ValueT != newState;
                        SensorState.ValueT = newState;
                    }
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    Messages.LogException("GIPConv2006SensorD", "Response_PropertyChanged", msg);
                }
                finally
                {
                    _LockResend = false;
                }
            }
        }
        #endregion
        
        #region Write-Values to PLC
        [ACPropertyBindingTarget(250, "Read from PLC", "en{'Request'}de{'Kommando'}", "", false, false)]
        public IACContainerTNet<GIPConv2006SensorDMaskReq> Request { get; set; }
        protected GIPConv2006SensorDMaskReq RequestValue
        {
            get { return Request.ValueT; }
        }
        protected virtual void OnRequestPropertyReplaced(IACPropertyNetTarget oldTarget, IACPropertyNetTarget newTarget)
        {
            if (oldTarget != null)
                (oldTarget as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            if (newTarget != null)
                (newTarget as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
        }

        public virtual void OnSetRequest(IACPropertyNetValueEvent valueEvent)
        {
        }

        protected virtual void Request_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                if (_LockReread)
                    return;
                _LockResend = true;
                try
                {
#if DEBUG
                    if (AggrNo != null && AggrNo.ValueT > 0)
                    {
                        int debug = 0;
                        debug++;
                    }
#endif

                    if (SensorRole != null 
                        && SensorRole.ACType is ACClassProperty 
                        && String.IsNullOrEmpty((SensorRole.ACType as ACClassProperty).XMLValue))
                        SensorRole.ValueT = RequestValue.Bit02_AckObligatory ? PAESensorRole.FaultSensor : PAESensorRole.Indicator;
                    if (OperatingMode != null)
                    {
                        if (RequestValue.Bit05_ForcePLC_Low || RequestValue.Bit06_ForcePLC_High)
                            OperatingMode.ValueT = Global.OperatingMode.Manual;
                        else
                            OperatingMode.ValueT = Global.OperatingMode.Automatic;
                    }
                    if (ActiveLow != null)
                        ActiveLow.ValueT = RequestValue.Bit00_NegateIO;
                    if (CollectiveAck != null)
                        CollectiveAck.ValueT = RequestValue.Bit01_CollectiveAck;
                    if (FaultACK != null)
                        FaultACK.ValueT = RequestValue.Bit03_Acknowledge;
                    if (IgnoreDelayTimes != null)
                        IgnoreDelayTimes.ValueT = RequestValue.Bit04_IgnoreDelayTimes;
                    if (ForceSensorSignal != null)
                    {
                        if (RequestValue.Bit05_ForcePLC_Low || RequestValue.Bit06_ForcePLC_High)
                            ForceSensorSignal.ValueT = true;
                        else
                            ForceSensorSignal.ValueT = false;
                    }
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    Messages.LogException("GIPConv2006SensorD", "Request_PropertyChanged", msg);
                }
                finally
                {
                    _LockResend = false;
                }
            }
        }

        //[ACPropertyBindingTarget(251, "Read from PLC", "en{'Delaytime signal comes [ms]'}de{'Verzögerungszeit Signal kommt [ms]'}", "", false, false)]
        //public IACContainerTNet<double> DelayMSecSignalOn { get; set; }

        //[ACPropertyBindingTarget(252, "Read from PLC", "en{'Delaytime signal goes [ms]'}de{'Verzögerungszeit Signal geht [ms]'}", "", false, false)]
        //public IACContainerTNet<double> DelayMSecSignalOff { get; set; }

        //protected virtual void ConverterProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        //{
        //    if (phase == ACPropertyChangedPhase.AfterBroadcast)
        //        return;
        //    if (_LockReRead)
        //        return;
        //    if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
        //    {
        //        _LockResend = true;
        //        try
        //        {
        //            if (sender == DelayMSecSignalOn && DelayTimeSignalOn != null)
        //            {
        //                DelayTimeSignalOn.ValueT = TimeSpan.FromMilliseconds(DelayMSecSignalOn.ValueT);
        //            }
        //            else if (sender == DelayMSecSignalOff && DelayTimeSignalOff != null)
        //            {
        //                DelayTimeSignalOff.ValueT = TimeSpan.FromMilliseconds(DelayMSecSignalOff.ValueT);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //        }
        //        finally
        //        {
        //            _LockResend = false;
        //        }
        //    }
        //}


        #endregion
        #endregion

        #region overridden methods
        protected override bool OnParentServerPropertyFound(IACPropertyNetServer parentProperty)
        {
            switch (parentProperty.ACIdentifier)
            {
                case "SensorState":
                    SensorState = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "SensorRole":
                    SensorRole = parentProperty as IACContainerTNet<PAESensorRole>;
                    return true;
                case "OperatingMode":
                    OperatingMode = parentProperty as IACContainerTNet<Global.OperatingMode>;
                    return true;
                case "IgnoreDelayTimes":
                    IgnoreDelayTimes = parentProperty as IACContainerTNet<bool>;
                    return true;
                case "CollectiveAck":
                    CollectiveAck = parentProperty as IACContainerTNet<bool>;
                    return true;
                case "ForceSensorSignal":
                    ForceSensorSignal = parentProperty as IACContainerTNet<bool>;
                    return true;
                case "ActiveLow":
                    ActiveLow = parentProperty as IACContainerTNet<bool>;
                    return true;
                case "DelayTimeSignalOff":
                    DelayTimeSignalOff = parentProperty as IACContainerTNet<TimeSpan>;
                    return true;
                case "DelayTimeSignalOn":
                    DelayTimeSignalOn = parentProperty as IACContainerTNet<TimeSpan>;
                    return true;
                case "ReqOperatingMode":
                    ReqOperatingMode = parentProperty as IACContainerTNet<Global.OperatingMode>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultACK":
                    FaultACK = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
            }
            return false;
        }

        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if (sender == FaultACK)
            {
                _LockReread = true;
                RequestValue.Bit03_Acknowledge = FaultACK.ValueT;
                _LockReread = false;
                return;
            }
            if (_LockResend)
                return;
            _LockReread = true;
            try
            {
                #if DEBUG
                if (AggrNo != null && AggrNo.ValueT > 0)
                {
                    int debug = 0;
                    debug++;
                }
                #endif

                if (   (ForceSensorSignal != null && ReqOperatingMode != null) 
                         && (sender == ReqOperatingMode || sender == ForceSensorSignal))
                {
                    if ((ReqOperatingMode.ValueT == Global.OperatingMode.Manual) || (ReqOperatingMode.ValueT == Global.OperatingMode.Maintenance))
                    {
                        if (ForceSensorSignal.ValueT)
                        {
                            RequestValue.Bit05_ForcePLC_Low = false;
                            RequestValue.Bit06_ForcePLC_High = true;
                        }
                        else
                        {
                            RequestValue.Bit05_ForcePLC_Low = true;
                            RequestValue.Bit06_ForcePLC_High = false;
                        }
                    }
                    else
                    {
                        RequestValue.Bit05_ForcePLC_Low = false;
                        RequestValue.Bit06_ForcePLC_High = false;
                    }
                    OperatingMode.ValueT = ReqOperatingMode.ValueT;
                    return;
                }
                else if (sender == ActiveLow)
                {
                    RequestValue.Bit00_NegateIO = ActiveLow.ValueT;
                }
                else if (sender == CollectiveAck)
                {
                    RequestValue.Bit01_CollectiveAck = CollectiveAck.ValueT;
                }
                else if (sender == IgnoreDelayTimes)
                {
                    RequestValue.Bit04_IgnoreDelayTimes = IgnoreDelayTimes.ValueT;
                }
                else if (sender == SensorRole)
                {
                    if (SensorRole.ValueT == PAESensorRole.FaultSensor)
                        RequestValue.Bit02_AckObligatory = true;
                    else
                        RequestValue.Bit02_AckObligatory = false;
                }
                //else if (sender == DelayMSecSignalOn)
                //{
                //    _LockReRead = true;
                //    DelayMSecSignalOn.ValueT = DelayTimeSignalOn.ValueT.TotalMilliseconds;
                //    _LockReRead = false;
                //}
                //else if (sender == DelayTimeSignalOff)
                //{
                //    _LockReRead = true;
                //    DelayMSecSignalOff.ValueT = DelayTimeSignalOff.ValueT.TotalMilliseconds;
                //    _LockReRead = false;
                //}
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("GIPConv2006SensorD", "ModelProperty_ValueUpdatedOnReceival", msg);
            }
            finally
            {
                _LockReread = false;
            }


        }

        #endregion

        #region pulic Methods
        [ACMethodInfo("", "en{'Select control module to multiplex'}de{'Auswahl des zu multiplexenden Control-Modul'}", 301, false, Global.ACKinds.MSMethodPrePost)]
        public void SelectControlModule()
        {
            if (!PreExecute("SelectControlModule"))
                return;
            IACPropertyNetSource sourcePropDA = (Response as IACPropertyNetTarget).Source;
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
            IACPropertyNetSource sourcePropDA = (Response as IACPropertyNetTarget).Source;
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
