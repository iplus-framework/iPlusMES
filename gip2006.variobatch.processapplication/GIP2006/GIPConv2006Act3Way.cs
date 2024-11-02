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

namespace gip2006.variobatch.processapplication
{
    /// <summary>
    /// Converter-Class for PAEActuator3way
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006Act3Way'}de{'GIPConv2006Act3Way'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPConv2006Act3Way : GIPConv2006Actuator
    {
        #region c'tors
        public GIPConv2006Act3Way(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion

        #region overridden methods
        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if (sender == ReqPos3)
            {
                if (!_LockResend_ReqRunState && (ReqPos1 != null) && (ReqPos2 != null))
                {
                    if (!ReqPos1.ValueT && !ReqPos2.ValueT && ReqPos3.ValueT) // Richtung3 / unten
                    {
                        Request.ValueT.Bit03_Pos3Mid = true;
                        Request.ValueT.Bit02_Pos2Open = false;
                        Request.ValueT.Bit00_Pos1Close = false;
                    }
                    else if (!ReqPos1.ValueT && ReqPos2.ValueT && !ReqPos3.ValueT) // Richtung2 / Rechts
                    {
                        Request.ValueT.Bit03_Pos3Mid = false;
                        Request.ValueT.Bit02_Pos2Open = true;
                        Request.ValueT.Bit00_Pos1Close = false;
                    }
                    else // Richtung 1 / Links
                    {
                        Request.ValueT.Bit03_Pos3Mid = false;
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
                //                         Bit00_Pos1Closed, Bit02_Pos2Open, Bit03_Mid, Bit01_Triggered | Bit00_Pos1Close, Bit02_Pos2Open, Bit03_Pos3Mid
                // A.) R1/Links:               1, 0, 0, 0  | 1, 0, 0
                // B.) Ansteuerung R3/Unten:   1, 0, 0, 0  | 0, 0, 1
                // C.) Geht richtung R3/Unten: 1, 0, 0, 1  | 0, 0, 1
                // D.) Verlässt R1-Stellung:   0, 0, 0, 1  | 0, 0, 1
                // E.) R3/Unten:               0, 0, 1, 0  | 0, 0, 1
                // F.) Ansteuerung R2/Rechts:  0, 0, 1, 0  | 0, 1, 0
                // G.) Geht richtung R2:       0, 0, 1, 1  | 0, 1, 0
                // D.) Verlässt R3-Stellung    0, 0, 0, 1  | 0, 1, 0
                // F.) R2/Rechts:              0, 1, 0, 0  | 0, 1, 0
                // G.) Ansteuerung R3/Unten:   0, 1, 0, 0  | 0, 0, 1
                // H.) Geht richtung R3/Unten: 0, 1, 0, 1  | 0, 0, 1
                // I.) Verlässt R2-Stellung    0, 0, 0, 1  | 0, 0, 1
                // E.) R3/Unten:
                // J.) Ansteuerung R1/Links:   0, 0, 1, 0  | 1, 0, 0
                // K.) Geht richtung R1/Links: 0, 0, 1, 1  | 1, 0, 0
                // L.) Verlässt R3-Stellung:   0, 0, 0, 1  | 1, 0, 0
                // wieder A.)

                _LockResend_ReqRunState = true;
                if ((Pos1 != null) && (Pos2 != null) && (Pos3 != null))
                {
                    // Richtung3 / Unten
                    if (!Response.ValueT.Bit00_Pos1Closed && !Response.ValueT.Bit02_Pos2Open && Response.ValueT.Bit03_Mid)
                    {
                        Pos3.ValueT = true;
                        Pos2.ValueT = false;
                        Pos1.ValueT = false;
                    }
                    // Richtung2 / Rechts
                    else if (!Response.ValueT.Bit00_Pos1Closed && Response.ValueT.Bit02_Pos2Open && !Response.ValueT.Bit03_Mid)
                    {
                        Pos3.ValueT = false;
                        Pos2.ValueT = true;
                        Pos1.ValueT = false;
                    }
                    // Richtung1 / Links
                    else
                    {
                        Pos3.ValueT = false;
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
            if (ReqPos1 == null || ReqPos2 == null || ReqPos3 == null)
                return;
            //if (!Response.ValueT.Bit01_Triggered && !reqChanged)
            //    return;
            if (!reqChanged)
                return;
            // Restore Req at startup:
            if (Request.ValueT.Bit03_Pos3Mid
                && !Request.ValueT.Bit02_Pos2Open
                && !Request.ValueT.Bit00_Pos1Close
                && (ReqPos1.ValueT || ReqPos2.ValueT || !ReqPos3.ValueT))
            {
                ReqPos1.ValueT = false;
                ReqPos2.ValueT = false;
                ReqPos3.ValueT = true;
            }
            else if (!Request.ValueT.Bit03_Pos3Mid
                && Request.ValueT.Bit02_Pos2Open
                && !Request.ValueT.Bit00_Pos1Close
                && (ReqPos1.ValueT || !ReqPos2.ValueT || ReqPos3.ValueT))
            {
                ReqPos1.ValueT = false;
                ReqPos2.ValueT = true;
                ReqPos3.ValueT = false;
            }
            else if (!Request.ValueT.Bit03_Pos3Mid
                && !Request.ValueT.Bit02_Pos2Open
                && Request.ValueT.Bit00_Pos1Close
                && (!ReqPos1.ValueT || ReqPos2.ValueT || ReqPos3.ValueT))
            {
                ReqPos1.ValueT = true;
                ReqPos2.ValueT = false;
                ReqPos3.ValueT = false;
            }
        }

        #endregion
    }
}
