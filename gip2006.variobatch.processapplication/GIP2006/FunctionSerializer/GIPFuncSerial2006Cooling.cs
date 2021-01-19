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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Serializer for Cooling'}de{'Serialisierer für Kühlen'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.Required, false, false)]
    public class GIPFuncSerial2006Cooling : ACSessionObjSerializer
    {
        public GIPFuncSerial2006Cooling(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            return MethodNameEquals(typeOrACMethodName, "Cooling");
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

            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Temperature
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowTemperature
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // HoldTemperature
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // HoldTime
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // MinWeightSwitchOn

            OnSendObjectGetLength(request, dbNo, offset, miscParams, ref iOffset);
            if (s7Session.HashCodeValidation != HashCodeValidationEnum.Off)
                iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;

            byte[] sendPackage1 = new byte[iOffset];
            iOffset = 0;
            if (s7Session.HashCodeValidation == HashCodeValidationEnum.Head || s7Session.HashCodeValidation == HashCodeValidationEnum.Head_WithRead)
                iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("Temperature")),
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("FlowTemperature")),
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("HoldTemperature")),
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            double totalSec = request.ParameterValueList.GetTimeSpan("HoldTime").TotalSeconds;
            if (totalSec > Int16.MaxValue)
                totalSec = Int16.MaxValue;
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(Convert.ToInt16(totalSec)),
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("MinWeightSwitchOn")),
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

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

                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Temperature
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowTemperature
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // HoldTemperature
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // HoldTime
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // MinWeightSwitchOn

                OnReadObjectGetLength(response, dbNo, offset, miscParams, readParameter, ref iOffset);

                byte[] readPackage1 = new byte[iOffset];

                ErrorCode errCode = s7Session.PLCConn.ReadBytes(DataType.DataBlock, dbNo, offset, iOffset, out readPackage1);
                if (errCode != ErrorCode.NoError)
                    return null;

                iOffset = 0;
                if (s7Session.HashCodeValidation == HashCodeValidationEnum.Head || s7Session.HashCodeValidation == HashCodeValidationEnum.Head_WithRead)
                    iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;

                response.ParameterValueList.GetACValue("Temperature").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("FlowTemperature").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("HoldTemperature").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;
                
                response.ParameterValueList.GetACValue("HoldTime").Value = TimeSpan.FromSeconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("MinWeightSwitchOn").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                OnReadObjectAppend(response, dbNo, iOffset, miscParams, readPackage1, readParameter, ref iOffset);

            }
            else
            {
                int iOffset = 0;
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ReachedTemperature

                OnReadObjectGetLength(response, dbNo, offset, miscParams, readParameter, ref iOffset);

                byte[] readPackage1 = new byte[iOffset];

                ErrorCode errCode = s7Session.PLCConn.ReadBytes(DataType.DataBlock, dbNo, offset, iOffset, out readPackage1);
                if (errCode != ErrorCode.NoError)
                    return null;

                iOffset = 0;

                ACValue acValue = response.ResultValueList.GetACValue("ReachedTemperature");
                if (acValue != null)
                    response.ResultValueList.GetACValue("ReachedTemperature").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
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
