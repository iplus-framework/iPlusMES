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
using gip.mes.processapplication;

namespace gip2006.variobatch.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006OnOffMaskRes'}de{'Bitzugriff GIPConv2006OnOffMaskRes'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'Switched off (Bit00/3.0)'}de{'Ausgeschaltet (Bit00/3.0)'}")]
    [ACPropertyEntity(101, "Bit01", "en{'Is triggered (Bit01/3.1)'}de{'Angesteuert (Bit01/3.1)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Switched on (Bit02/3.2)'}de{'Eingeschaltet (Bit02/3.2)'}")]
    public class GIPConv2006OnOffMaskRes : GIPConv2006BaseMaskRes
    {
        #region c'tors
        public GIPConv2006OnOffMaskRes()
        {
        }

        public GIPConv2006OnOffMaskRes(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        public bool Bit00_Off
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        public bool Bit01_Triggered
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        public bool Bit02_On
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        #endregion
    }

    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006OnOffMaskReq'}de{'Bitzugriff GIPConv2006OnOffMaskReq'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'Stop (Bit00)'}de{'Stopp (Bit00)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Start (Bit02)'}de{'Start (Bit02)'}")]
    public class GIPConv2006OnOffMaskReq : GIPConv2006BaseMaskReq
    {
        #region c'tors
        public GIPConv2006OnOffMaskReq()
        {
        }

        public GIPConv2006OnOffMaskReq(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        public bool Bit00_Off
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        public bool Bit02_On
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        #endregion
    }

    /// <summary>
    /// Baseclass for converting State and Types between Standard-Model-Components and DataAccess-/Vendor Model
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006OnOff'}de{'GIPConv2006OnOff'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPConv2006OnOff : GIPConv2006Base
    {
        #region c'tors
        public GIPConv2006OnOff(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        protected override void BindMyProperties()
        {
            if (_PropertiesBound)
                return;
            base.BindMyProperties();
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            if (IsSwitchedOn != null)
                (IsSwitchedOn as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            IsSwitchedOn = null;

            if (MaintenanceSwitch != null)
                (MaintenanceSwitch as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            MaintenanceSwitch = null;

            if (ReqIsSwitchedOn != null)
                (ReqIsSwitchedOn as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ReqIsSwitchedOn = null;

            return await base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Properties

        #region Read-Values from PLC
        [ACPropertyBindingTarget(240, "Read from PLC", "en{'Response'}de{'Rückmeldung'}", "", false, false)]
        public IACContainerTNet<GIPConv2006OnOffMaskRes> Response { get; set; }
        protected override IACPropertyNetTarget _Response 
        {
            get
            {
                 return (IACPropertyNetTarget) this.Response;
            }
        }

        protected override GIPConv2006BaseMaskRes _ResponseValue
        {
            get { return Response.ValueT; }
        }


        #region OnOffModule
        public IACContainerTNet<Boolean> IsSwitchedOn { get; set; }
        public IACContainerTNet<Boolean> MaintenanceSwitch { get; set; }
        #endregion

        #endregion

        #region Write-Value to PLC
        [ACPropertyBindingTarget(250, "Read from PLC", "en{'Request'}de{'Kommando'}", "", false, false)]
        public IACContainerTNet<GIPConv2006OnOffMaskReq> Request { get; set; }
        protected override IACPropertyNetTarget _Request
        {
            get
            {
                return (IACPropertyNetTarget)Request;
            }
        }

        protected override GIPConv2006BaseMaskReq _RequestValue 
        { 
            get { return Request.ValueT; } 
        }


        #region OnOffModule
        public IACContainerTNet<Boolean> ReqIsSwitchedOn { get; set; }
        #endregion

        #endregion

        #endregion

        #region overridden methods
        protected override bool OnParentServerPropertyFound(IACPropertyNetServer parentProperty)
        {
            switch (parentProperty.ACIdentifier)
            {
                case "IsSwitchedOn":
                    IsSwitchedOn = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "MaintenanceSwitch":
                    MaintenanceSwitch = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "ReqIsSwitchedOn":
                    ReqIsSwitchedOn = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                default:
                    break;
            }

            return base.OnParentServerPropertyFound(parentProperty);
        }

        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if (sender == FaultStateACK)
            {
                if (FaultStateACK.ValueT == true)
                {
                    _LockResend_ReqIsSwitchedOn = true;
                    if (ReqIsSwitchedOn.ValueT)
                        ReqIsSwitchedOn.ValueT = false;
                    _LockResend_ReqIsSwitchedOn = false;
                    _RequestValue.Bit15_FaultAck = FaultStateACK.ValueT;
                }
                return;
            }
            else if (sender == ReqIsSwitchedOn)
            {
                if (!_LockResend_ReqIsSwitchedOn)
                {
                    if (ReqIsSwitchedOn.ValueT)
                    {
                        Request.ValueT.Bit00_Off = false;
                        Request.ValueT.Bit02_On = true;
                    }
                    else
                    {
                        Request.ValueT.Bit00_Off = true;
                        Request.ValueT.Bit02_On = false;
                    }
                }
                return;
            }
            else if (sender == ReqOperatingMode)
            {
                if ((ReqOperatingMode.ValueT == Global.OperatingMode.Manual) || (ReqOperatingMode.ValueT == Global.OperatingMode.Maintenance))
                {
                    Request.ValueT.Bit00_Off = true;
                    Request.ValueT.Bit02_On = false;
                    Request.ValueT.Bit06_Manual = true;
                }
                else
                {
                    Request.ValueT.Bit06_Manual = false;
                }
                return;
            }
            base.ModelProperty_ValueUpdatedOnReceival(sender, e, phase);
        }
        #endregion

        #region Converter-Logic
        private bool _LockResend_ReqIsSwitchedOn = false;
        protected override void Response_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            base.Response_PropertyChanged(sender, e, phase);
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                _LockResend_ReqIsSwitchedOn = true;
                if (IsSwitchedOn != null)
                {
                    bool changed = IsSwitchedOn.ValueT != Response.ValueT.Bit02_On;
                    IsSwitchedOn.ValueT = Response.ValueT.Bit02_On;
                    if (changed)
                    {
                        PAEControlModuleBase controlModule = ParentACComponent as PAEControlModuleBase;
                        if (controlModule != null)
                        {
                            if (IsSwitchedOn.ValueT)
                                controlModule.TurnOnInstant.ValueT = DateTime.Now;
                            else
                                controlModule.TurnOffInstant.ValueT = DateTime.Now;
                        }
                    }
                }
                _LockResend_ReqIsSwitchedOn = false;
            }
        }

        public override void OnSetRequest(IACPropertyNetValueEvent valueEvent)
        {
            base.OnSetRequest(valueEvent);
            if (valueEvent.Sender == EventRaiser.Source)
            {
                // Nicht zurücklesen, da SPS die gesetzten Werte selbst zurücksetzt
                //if (ReqIsSwitchedOn != null) ReqIsSwitchedOn.ValueT = Request.ValueT.Bit02;
            }
        }

        protected override void Request_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            base.Request_PropertyChanged(sender, e, phase);
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                _LockResend_ReqIsSwitchedOn = true;
                SynchronizeRequest(true);
                _LockResend_ReqIsSwitchedOn = false;
            }
        }

        private void SynchronizeRequest(bool reqChanged = false)
        {
            //if (!Response.ValueT.Bit01_Triggered && !reqChanged)
            //return;
            if (!reqChanged)
                return;
            if (ReqIsSwitchedOn != null && ReqIsSwitchedOn.ValueT && (Response.ValueT.Bit02_On || Request.ValueT.Bit00_Off))
            {
                ReqIsSwitchedOn.ValueT = false;
            }
        }

        #endregion

    }
}
