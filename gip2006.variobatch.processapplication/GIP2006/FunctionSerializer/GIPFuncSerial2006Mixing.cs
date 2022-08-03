using gip.core.communication;
using gip.core.communication.ISOonTCP;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip2006.variobatch.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Serializer for Mixing'}de{'Serialisierer für Mischen'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPFuncSerial2006Mixing : ACSessionObjSerializer
    {
        public GIPFuncSerial2006Mixing(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            return MethodNameEquals(typeOrACMethodName, "Mixing")
                || MethodNameEquals(typeOrACMethodName, "MixingTime")
                || MethodNameEquals(typeOrACMethodName, "MixingTemperature");
        }

        public override bool SendObject(object complexObj, int dbNo, int offset, object miscParams)
        {
            S7TCPSession s7Session = ParentACComponent as S7TCPSession;
            if (s7Session == null || complexObj == null)
                return false;
            if (!s7Session.PLCConn.IsConnected)
                return false;
            ACMethod request = complexObj as ACMethod;
            if (request == null)
                return false;

            int iOffset = 0;
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Duration
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Speed
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Temperature
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // MinWeight

            OnSendObjectGetLength(request, dbNo, offset, miscParams, ref iOffset);
            if (s7Session.HashCodeValidation != HashCodeValidationEnum.Off)
                iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;

            byte[] sendPackage1 = new byte[iOffset];
            iOffset = 0;
            if (s7Session.HashCodeValidation == HashCodeValidationEnum.Head || s7Session.HashCodeValidation == HashCodeValidationEnum.Head_WithRead)
                iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;

            double totalSec = 0;

            if (MethodNameEquals(request.ACIdentifier, "Mixing"))
            {
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("Speed")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(0.0),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("MinWeight")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;
            }
            else if (MethodNameEquals(request.ACIdentifier, "MixingTime"))
            {
                totalSec = request.ParameterValueList.GetTimeSpan("Duration").TotalSeconds;
                if (totalSec > Int16.MaxValue)
                    totalSec = Int16.MaxValue;
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(Convert.ToInt16(totalSec)),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("Speed")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(0.0),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("MinWeight")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;
            }
            else if (MethodNameEquals(request.ACIdentifier, "MixingTemperature"))
            {
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("Speed")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("Temperature")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("MinWeight")),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;
            }

            OnSendObjectAppend(request, dbNo, offset, miscParams, ref sendPackage1, ref iOffset);

            return this.SendObjectToPLC(s7Session, request, sendPackage1, dbNo, offset, iOffset);
        }

        public override object ReadObject(object complexObj, int dbNo, int offset, object miscParams)
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
                int iOffset = 0;
                if (s7Session.HashCodeValidation == HashCodeValidationEnum.Head || s7Session.HashCodeValidation == HashCodeValidationEnum.Head_WithRead)
                    iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;

                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Duration
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Speed
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Temperature
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // MinWeight

                OnReadObjectGetLength(response, dbNo, offset, miscParams, readParameter, ref iOffset);

                byte[] readPackage1 = new byte[iOffset];

                PLC.Result errCode = s7Session.PLCConn.ReadBytes(DataTypeEnum.DataBlock, dbNo, offset, iOffset, out readPackage1);
                if (errCode != null && !errCode.IsSucceeded)
                    return null;

                iOffset = 0;
                if (s7Session.HashCodeValidation == HashCodeValidationEnum.Head || s7Session.HashCodeValidation == HashCodeValidationEnum.Head_WithRead)
                    iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;

                if (MethodNameEquals(response.ACIdentifier, "Mixing"))
                {
                    //response.ParameterValueList.GetACValue("Duration").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                    iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                    response.ParameterValueList.GetACValue("Speed").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                    iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                    //response.ParameterValueList.GetACValue("Temperature").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                    iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                    response.ParameterValueList.GetACValue("MinWeight").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                    iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;
                }
                else if (MethodNameEquals(response.ACIdentifier, "MixingTime"))
                {
                    response.ParameterValueList.GetACValue("Duration").Value = TimeSpan.FromSeconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                    iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                    response.ParameterValueList.GetACValue("Speed").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                    iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                    //response.ParameterValueList.GetACValue("Temperature").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                    iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                    response.ParameterValueList.GetACValue("MinWeight").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                    iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;
                }
                else if (MethodNameEquals(response.ACIdentifier, "MixingTemperature"))
                {
                    //response.ParameterValueList.GetACValue("Duration").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                    iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                    response.ParameterValueList.GetACValue("Speed").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                    iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                    response.ParameterValueList.GetACValue("Temperature").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                    iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                    response.ParameterValueList.GetACValue("MinWeight").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                    iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;
                }

                OnReadObjectAppend(response, dbNo, iOffset, miscParams, readPackage1, readParameter, ref iOffset);

            }
            else
            {
                int iOffset = 0;
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // ActDuration
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ActTemperature

                OnReadObjectGetLength(response, dbNo, offset, miscParams, readParameter, ref iOffset);
                
                byte[] readPackage1 = new byte[iOffset];

                PLC.Result errCode = s7Session.PLCConn.ReadBytes(DataTypeEnum.DataBlock, dbNo, offset, iOffset, out readPackage1);
                if (errCode != null && !errCode.IsSucceeded)
                    return null;

                iOffset = 0;

                var acValue = response.ResultValueList.GetACValue("ActDuration");
                if (acValue != null)
                    acValue.Value = TimeSpan.FromSeconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                acValue = response.ResultValueList.GetACValue("ActTemperature");
                if (acValue != null)
                    response.ResultValueList.GetACValue("ActTemperature").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

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
