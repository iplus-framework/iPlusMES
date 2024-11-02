// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.communication;
using gip.core.communication.ISOonTCP;
using gip.core.datamodel;
using gip.mes.processapplication;
using System;
using System.Linq;

namespace gip2006.variobatch.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Serializer for Discharging'}de{'Serialisierer für Entleeren'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPFuncSerial2006Discharging : ACSessionObjSerializer
    {
        public GIPFuncSerial2006Discharging(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            return MethodNameEquals(typeOrACMethodName, "Discharging")
                || MethodNameEquals(typeOrACMethodName, "DischargingIntake");
        }

        public override bool SendObject(object complexObj, object prevComplexObj, int dbNo, int offset, int? routeOffset, object miscParams)
        {
            S7TCPSession s7Session = ParentACComponent as S7TCPSession;
            if (s7Session == null || complexObj == null || s7Session.PLCConn == null)
                return false;
            if (!s7Session.PLCConn.IsConnected)
                return false;
            ACMethod request = complexObj as ACMethod;
            if (request == null)
                return false;

            int iOffset = 0;
            if (routeOffset.HasValue)
            {
                GIPFuncSerial2006Way serializer = FindChildComponents<GIPFuncSerial2006Way>(c => c is GIPFuncSerial2006Way).FirstOrDefault();
                if (serializer == null)
                    serializer = ParentACComponent.FindChildComponents<GIPFuncSerial2006Way>(c => c is GIPFuncSerial2006Way, null, 1).FirstOrDefault();
                if (serializer != null)
                {
                    string errorMsg = null;
                    // If route data not sucessful transferred to plc, then wait
                    if (!serializer.SendRoute(this, complexObj, prevComplexObj, dbNo, routeOffset.Value, null, miscParams, out errorMsg))
                    {
                        WriteAlarm(miscParams, errorMsg);
                        return false;
                    }
                }
            }

            if (!routeOffset.HasValue || routeOffset.Value > offset)
            {
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Source
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Destination
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // EmptyWeight
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // DischargingTime
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // PulsationTime
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // PulsationPauseTime
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // IdleCurrent
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // InterDischarging
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Vibrator
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Power
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Tolerance
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Tolerance -
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // TargetWeight -
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // LastBatch
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Sieve
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ScaleBatchWeight (SWT)

                OnSendObjectGetLength(request, dbNo, offset, miscParams, ref iOffset);
                if (s7Session.HashCodeValidation != HashCodeValidationEnum.Off)
                    iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;

                byte[] sendPackage1 = new byte[iOffset];
                iOffset = 0;
                if (s7Session.HashCodeValidation == HashCodeValidationEnum.Head || s7Session.HashCodeValidation == HashCodeValidationEnum.Head_WithRead)
                    iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("Source")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("Destination")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                double emptyWeight = 0;
                var acValue = request.ParameterValueList.GetACValue("EmptyWeight");
                if (acValue.ValueT<Double?>().HasValue)
                    emptyWeight = acValue.ValueT<Double?>().Value;
                Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(emptyWeight),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(Convert.ToInt16(request.ParameterValueList.GetTimeSpan("DischargingTime").TotalSeconds)),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(Convert.ToInt16(request.ParameterValueList.GetTimeSpan("PulsationTime").TotalMilliseconds)),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(Convert.ToInt16(request.ParameterValueList.GetTimeSpan("PulsationPauseTime").TotalMilliseconds)),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("IdleCurrent")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("InterDischarging")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("Vibrator")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("Power")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("Tolerance")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("ToleranceMin")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                acValue = request.ParameterValueList.GetACValue("TargetQuantity");
                if (acValue != null)
                {
                    Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(acValue.ParamAsDouble),
                        0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                    iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;
                }
                else
                {
                    Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(0.0),
                        0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                    iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;
                }

                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16(PWMethodVBBase.IsLastBatchParamName)),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("Sieve")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                acValue = request.ParameterValueList.GetACValue("ScaleBatchWeight");
                if (acValue != null)
                {
                    Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(acValue.ParamAsDouble),
                        0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                }
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                //Array.Copy(gip.core.communication.ISOonTCP.Types.String.ToByteArray(request.ParameterValueList.GetString(""), 30, 32), 0, sendPackage1, 2, 32);
                //iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //byte[] sendPackage2 = new byte[iOffset];
                //Array.Copy(sendPackage1, sendPackage2, iOffset);

                OnSendObjectAppend(request, dbNo, offset, miscParams, ref sendPackage1, ref iOffset);

                bool sended = this.SendObjectToPLC(s7Session, request, sendPackage1, dbNo, offset, iOffset);
                if (!sended)
                    return false;
            }
            return true;
        }

        protected void WriteAlarm(object miscParams, string message)
        {
            bool written = false;

            (ACChildInstanceInfo childInfo, IACComponent invokerModule) = GetSendParameters(miscParams);

            if (invokerModule != null)
            {
                PAProcessModuleVB paModule = invokerModule as PAProcessModuleVB;
                if (paModule != null)
                {
                    PAFDischarging pafDis = paModule.FindChildComponents<PAFDischarging>(c => c is PAFDischarging).FirstOrDefault();
                    if (pafDis != null)
                    {
                        pafDis.FunctionError.ValueT = gip.core.autocomponent.PANotifyState.AlarmOrFault;
                        pafDis.OnNewAlarmOccurred(pafDis.FunctionError, new Msg(message, this, eMsgLevel.Exception, "PAProcessFunction", "ReSendACMethod", 9010), true);
                        written = true;
                    }
                }
            }
            if (!written)
            {
                Messages.LogError(this.GetACUrl(), "WriteAlarm", message);
            }
            return;
        }

        public override object ReadObject(object complexObj, int dbNo, int offset, int? routeOffset, object miscParams)
        {
            S7TCPSession s7Session = ParentACComponent as S7TCPSession;
            if (s7Session == null || complexObj == null)
                return null;
            if (!s7Session.PLCConn.IsConnected)
                return null;
            ACMethod response = complexObj as ACMethod;
            if (response == null)
                return null;

            ACChildInstanceInfo childInfo = null;
            IACComponent invokerModule = null;
            bool readParameter = false;
            if (miscParams != null && miscParams is bool)
                readParameter = (bool)miscParams;
            else if (miscParams != null && miscParams is object[])
            {
                object[] paramArr = miscParams as object[];
                childInfo = paramArr[0] as ACChildInstanceInfo;
                if (childInfo != null)
                {
                    invokerModule = ACUrlCommand(childInfo.ACUrlParent) as IACComponent;
                }
                readParameter = (bool)paramArr[1];
            }

            if (readParameter)
            {
                if (routeOffset.HasValue)
                {
                    GIPFuncSerial2006Way serializer = FindChildComponents<GIPFuncSerial2006Way>(c => c is GIPFuncSerial2006Way).FirstOrDefault();
                    if (serializer == null)
                        serializer = ParentACComponent.FindChildComponents<GIPFuncSerial2006Way>(c => c is GIPFuncSerial2006Way, null, 1).FirstOrDefault();
                    if (serializer != null)
                    {
                        serializer.ReadObject(complexObj, dbNo, offset, routeOffset, miscParams);
                    }
                }


                int iOffset = 0;
                if (s7Session.HashCodeValidation == HashCodeValidationEnum.Head || s7Session.HashCodeValidation == HashCodeValidationEnum.Head_WithRead)
                    iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;

                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Source
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Destination
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // EmptyWeight
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // DischargingTime
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // PulsationTime
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // PulsationPauseTime
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // IdleCurrent
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // InterDischarging
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Vibrator
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Power
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Tolerance +
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Tolerance -
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // TargetWeight -
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // LastBatch
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Sieve
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ScaleBatchWeight (SWT)

                OnReadObjectGetLength(response, dbNo, offset, miscParams, readParameter, ref iOffset);

                byte[] readPackage1 = new byte[iOffset];

                PLC.Result errCode = s7Session.PLCConn.ReadBytes(DataTypeEnum.DataBlock, dbNo, offset, iOffset, out readPackage1);
                if (errCode != null && !errCode.IsSucceeded)
                    return null;

                iOffset = 0;
                if (s7Session.HashCodeValidation == HashCodeValidationEnum.Head || s7Session.HashCodeValidation == HashCodeValidationEnum.Head_WithRead)
                    iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;

                response.ParameterValueList.GetACValue("Source").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("Destination").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("EmptyWeight").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("DischargingTime").Value = TimeSpan.FromSeconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("PulsationTime").Value = TimeSpan.FromMilliseconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("PulsationPauseTime").Value = TimeSpan.FromMilliseconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("IdleCurrent").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("InterDischarging").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("Vibrator").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("Power").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("Tolerance").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("ToleranceMin").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                var acValue = response.ParameterValueList.GetACValue("TargetQuantity");
                if (acValue != null)
                    acValue.Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName).Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("Sieve").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                acValue = response.ParameterValueList.GetACValue("ScaleBatchWeight");
                if (acValue != null)
                    acValue.Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                OnReadObjectAppend(response, dbNo, iOffset, miscParams, readPackage1, readParameter, ref iOffset);
            }
            else
            {
                int iOffset = 0;
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ActualQuantity
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ScaleTotalWeight
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // DischargingTime
                iOffset += 20; // GaugeCode/Alibi
                OnReadObjectGetLength(response, dbNo, offset, miscParams, readParameter, ref iOffset);

                byte[] readPackage1 = new byte[iOffset];

                PLC.Result errCode = s7Session.PLCConn.ReadBytes(DataTypeEnum.DataBlock, dbNo, offset, iOffset, out readPackage1);
                if (errCode != null && !errCode.IsSucceeded)
                    return null;

                iOffset = 0;
                response.ResultValueList.GetACValue("ActualQuantity").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ResultValueList.GetACValue("ScaleTotalWeight").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ResultValueList.GetACValue("DischargingTime").Value = TimeSpan.FromMilliseconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                ACValue acValue = response.ResultValueList.GetACValue("GaugeCode");
                if (acValue != null)
                    acValue.Value = gip.core.communication.ISOonTCP.Types.String.FromByteArray(readPackage1, iOffset, 20, true);
                iOffset += 20;

                OnReadObjectAppend(response, dbNo, iOffset, miscParams, readPackage1, readParameter, ref iOffset);
            }

            return response;
        }

        protected virtual void OnSendObjectGetLength(ACMethod acMethod, int dbNo, int offset, object miscParams, ref int iOffset, short telegramNo = 1) { }
        protected virtual void OnSendObjectAppend(ACMethod acMethod, int dbNo, int offset, object miscParams, ref byte[] sendPackage, ref int iOffset, short telegramNo = 1) { }

        protected virtual void OnReadObjectGetLength(ACMethod acMethod, int dbNo, int offset, object miscParams, bool readParameter, ref int iOffset, short telegramNo = 1) { }
        protected virtual void OnReadObjectAppend(ACMethod acMethod, int dbNo, int offset, object miscParams, byte[] readPackage, bool readParameter, ref int iOffset, short telegramNo = 1) { }

    }
}
