using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace gip2006.variobatch.processapplication
{
    /// <summary>
    /// Converter-Class for PAEActuator1way and  PAEActuator1way_Analog
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006Act1Way'}de{'GIPConv2006Act1Way'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPConv2006Act1Way : GIPConv2006Actuator
    {
        #region c'tors
        public GIPConv2006Act1Way(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion

        #region overridden methods
        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if (sender == FaultStateACK)
            {
                if (FaultStateACK.ValueT == true)
                {
                    _LockResend_ReqRunState = true;
                    if (ReqPos1.ValueT)
                        ReqPos1.ValueT = false;
                    _LockResend_ReqRunState = false;
                    _RequestValue.Bit15_FaultAck = FaultStateACK.ValueT;
                }
                return;
            }
            else if (sender == ReqPos1)
            {
                if (!_LockResend_ReqRunState)
                {
                    if (ReqPos1.ValueT)
                    {
                        Request.ValueT.Bit00_Pos1Close = false;
                        Request.ValueT.Bit02_Pos2Open = true;
                    }
                    else
                    {
                        Request.ValueT.Bit02_Pos2Open = false;
                        Request.ValueT.Bit00_Pos1Close = true;
                    }
                }
                return;
            }
            base.ModelProperty_ValueUpdatedOnReceival(sender, e, phase);
        }
        #endregion

        #region Converter-Logic
        private bool _LockResend_ReqRunState = false;
        protected override void Response_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            base.Response_PropertyChanged(sender, e, phase);
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                // Zustandsautomat:
                //                         Bit00_Pos1Closed, Bit02_Pos2Open, Bit01_Triggered | Bit00_Pos1Close, Bit02_Pos2Open
                // A.) Zu:                     1, 0, 0  | 1, 0
                // B.) Ansteuerung auf:        1, 0, 0  | 0, 1
                // C.) Geht auf:               1, 0, 1  | 0, 1
                // D.) Verlässt Zu-Stellung:   0, 0, 1  | 0, 1
                // E.) Offen:                  0, 1, 0  | 0, 1
                // F.) Ansteuerung Zu:         0, 1, 0  | 1, 0
                // G.) Geht zu:                0, 1, 1  | 1, 0
                // E.) Verlässt Auf-Stellung:  0, 0, 1  | 0, 1
                // wieder A.)
                _LockResend_ReqRunState = true;
                if (Pos1 != null && ReqPos1 != null)
                {
                    bool changed = false;
                    if (!Response.ValueT.Bit00_Pos1Closed && Response.ValueT.Bit02_Pos2Open)
                    {
                        changed = !Pos1.ValueT;
                        Pos1.ValueT = true;
                        // Restore Req at startup:
                        //if (!ReqPos1.ValueT && (OperatingMode != null) && (OperatingMode.ValueT == Global.OperatingMode.Manual))
                        //{
                        //    _LockResend_ReqRunState = true;
                        //    //ReqPos1.ValueT = true;
                        //    _LockResend_ReqRunState = false;
                        //}
                    }
                    else if (Response.ValueT.Bit00_Pos1Closed && !Response.ValueT.Bit02_Pos2Open)
                    {
                        changed = Pos1.ValueT;
                        Pos1.ValueT = false;
                        // Restore Req at startup:
                        //if (ReqPos1.ValueT && (OperatingMode != null) && (OperatingMode.ValueT == Global.OperatingMode.Manual))
                        //{
                        //    _LockResend_ReqRunState = true;
                        //    //ReqPos1.ValueT = false;
                        //    _LockResend_ReqRunState = false;
                        //}
                    }
                    else
                    {
                        changed = Pos1.ValueT;
                        Pos1.ValueT = false;
                    }

                    if (changed)
                    {
                        gip.core.processapplication.PAEControlModuleBase controlModule = ParentACComponent as gip.core.processapplication.PAEControlModuleBase;
                        if (controlModule != null)
                        {
                            if (Pos1.ValueT)
                                controlModule.TurnOnInstant.ValueT = DateTime.Now;
                            else
                                controlModule.TurnOffInstant.ValueT = DateTime.Now;
                        }
                    }
                    SynchronizeRequest();
                    _LockResend_ReqRunState = false;
                }
            }
        }

        protected override void Request_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            base.Request_PropertyChanged(sender, e, phase);
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                _LockResend_ReqRunState = true;
                SynchronizeRequest(true);
                _LockResend_ReqRunState = false;
            }
        }

        private void SynchronizeRequest(bool reqChanged=false)
        {
            if (ReqPos1 == null)
                return;
            //if (!Response.ValueT.Bit01_Triggered && !reqChanged)
                //return;
            //if (!reqChanged)
                //return;
            if (ReqPos1.ValueT && (Response.ValueT.Bit02_Pos2Open || Request.ValueT.Bit00_Pos1Close))
            {
                ReqPos1.ValueT = false;
            }
            // Restore Req at startup:
            /*if (!ReqPos1.ValueT && Request.ValueT.Bit02_Pos2Open)
            {
                ReqPos1.ValueT = true;
            }
            else if (ReqPos1.ValueT && (Request.ValueT.Bit00_Pos1Close || !Request.ValueT.Bit02_Pos2Open))
            {
                ReqPos1.ValueT = false;
            }*/
        }

        #endregion
    }
}
