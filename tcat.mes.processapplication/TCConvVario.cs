// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.communication;

namespace tcat.mes.processapplication
{
    [ACClassInfo(Const.PackName_TwinCAT, "en{'TwinCAT Converter'}de{'TwinCAT Converter'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class TCConvVario : PAFuncStateConvBase
    {
        public TCConvVario(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool IsReadyForSending
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
                    else if (acSession == null && !(bool)this.Session.ACUrlCommand("IsReadyForWriting"))
                        isReadyForSending = false;
                }
                //if (isReadyForSending && !IsACStateRestored)
                //    isReadyForSending = false;
                return isReadyForSending;
            }
        }

        public override bool IsReadyForReading
        {
            get
            {
                if (IsSimulationOn)
                    return true;
                bool isReadyForReading = false;
                if (this.Session != null)
                {
                    isReadyForReading = true;
                    ACSession acSession = this.Session as ACSession;
                    if (acSession != null && !acSession.IsReadyForWriting)
                        isReadyForReading = false;
                    else if (acSession == null && !(bool)this.Session.ACUrlCommand("IsReadyForWriting"))
                        isReadyForReading = false;
                }
                //if (isReadyForReading && !IsACStateRestored)
                //    isReadyForReading = false;
                return isReadyForReading;
            }
        }

        public override ACStateEnum GetNextACState(PAProcessFunction sender, string transitionMethod = "")
        {
            return PAFuncStateConvBase.GetDefaultNextACState(sender.CurrentACState, transitionMethod);
        }

        public override bool IsEnabledTransition(PAProcessFunction sender, string transitionMethod)
        {
            return PAFuncStateConvBase.IsEnabledTransitionDefault(ACState.ValueT, transitionMethod, sender);
        }

        public override MsgWithDetails SendACMethod(PAProcessFunction sender, ACMethod acMethod, ACMethod previousParams = null)
        {
            if (Session == null)
                return new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "SendACMethod()", Message = "Session is null" };

            if (!IsReadyForSending)
                return new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "SendACMethod()", Message = "Session is not ready for writing" };

            bool sended = false;
            ACChildInstanceInfo childInfo = new ACChildInstanceInfo(ParentACComponent);
            object result = this.Session.ACUrlCommand("!SendObject", acMethod, previousParams, 0, 0, 0, childInfo);
            if (result != null)
                sended = (bool)result;

            if (!sended)
            {
                return new MsgWithDetails()
                {
                    Source = this.GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "SendACMethod()",
                    Message = string.Format("ACMethod was not sended. Method name: {0}. Please check if connection is established.", acMethod == null ? "-" : acMethod.ACIdentifier)
                };
            }
            return null;
        }

        public override PAProcessFunction.CompleteResult ReceiveACMethodResult(PAProcessFunction sender, ACMethod acMethod, out MsgWithDetails msg)
        {
            if (Session == null)
            {
                msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "ReceiveACMethodResult()", Message = "Session is null" };
                return PAProcessFunction.CompleteResult.FailedAndWait;
            }

            if (!IsReadyForReading)
            {
                msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "ReceiveACMethodResult()", Message = "Session is not ready for writing" };
                return PAProcessFunction.CompleteResult.FailedAndWait;
            }

            if (acMethod == null)
            {
                msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "SendACMethod()", Message = "acMethod is null" };
                return PAProcessFunction.CompleteResult.FailedAndWait;
            }

            ACChildInstanceInfo childInfo = new ACChildInstanceInfo(ParentACComponent);
            object[] miscParams = new object[] { childInfo, false };

            ACMethod result = this.Session.ACUrlCommand("!ReadObject", acMethod, 0, 0, miscParams) as ACMethod;
            if (result == null)
            {
                msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "ReceiveACMethodResult()", Message = "ACMethod-Result was not received. Please check if connection is established." };
                return PAProcessFunction.CompleteResult.FailedAndWait;
            }

            // If acMethod was serilized over network, we don't receive the original object. Therefore the results mus be copied
            if (result != acMethod)
                acMethod.ResultValueList = result.ResultValueList;
            msg = null;
            return PAProcessFunction.CompleteResult.Succeeded;
        }

        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            
        }
    }
}
