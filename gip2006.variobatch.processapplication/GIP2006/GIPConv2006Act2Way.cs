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
    /// Converter-Class for PAEActuator2way
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006Act2Way'}de{'GIPConv2006Act2Way'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPConv2006Act2Way : GIPConv2006Actuator
    {
        #region c'tors
        public GIPConv2006Act2Way(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion

        #region overridden methods
        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if (sender == ReqPos2)
            {
                if (!_LockResend_ReqRunState && (ReqPos1 != null))
                {
                    if (!ReqPos1.ValueT && ReqPos2.ValueT) // Position 2, Rechts
                    {
                        Request.ValueT.Bit02_Pos2Open = true;
                        Request.ValueT.Bit00_Pos1Close = false;
                    }
                    else // Position 1, Links
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
                // A.) Links:                  1, 0, 0  | 1, 0
                // B.) Ansteuerung Rechts      1, 0, 0  | 0, 1
                // C.) Geht nach R:            1, 0, 1  | 0, 1
                // D.) Verlässt R-Stellung:    0, 0, 1  | 0, 1
                // E.) Rechts:                 0, 1, 0  | 0, 1
                // F.) Ansteuerung Links:      0, 1, 0  | 1, 0
                // G.) Geht nach L:            0, 1, 1  | 1, 0
                // E.) Verlässt L-Stellung:    0, 0, 1  | 0, 1
                // wieder A.)
                _LockResend_ReqRunState = true;
                if ((Pos1 != null) && (Pos2 != null))
                {
                    if (!Response.ValueT.Bit00_Pos1Closed && Response.ValueT.Bit02_Pos2Open)
                    {
                        Pos2.ValueT = true;
                        Pos1.ValueT = false;
                    }
                    else
                    {
                        Pos2.ValueT = false;
                        Pos1.ValueT = true;
                    }
                    SynchronizeRequest();
                }
                _LockResend_ReqRunState = false; 
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

        private void SynchronizeRequest(bool reqChanged = false)
        {
            if (ReqPos1 == null || ReqPos2 == null)
                return;
            //if (!Response.ValueT.Bit01_Triggered && !reqChanged)
            //    return;
            if (!reqChanged)
                return;
            // Restore Req at startup:
            if (Request.ValueT.Bit02_Pos2Open
                && !Request.ValueT.Bit00_Pos1Close
                && (ReqPos1.ValueT || !ReqPos2.ValueT))
            {
                ReqPos1.ValueT = false;
                ReqPos2.ValueT = true;
            }
            else if (!Request.ValueT.Bit02_Pos2Open
                && Request.ValueT.Bit00_Pos1Close
                && (!ReqPos1.ValueT || ReqPos2.ValueT))
            {
                ReqPos1.ValueT = true;
                ReqPos2.ValueT = false;
            }
        }

        #endregion
    }
}
