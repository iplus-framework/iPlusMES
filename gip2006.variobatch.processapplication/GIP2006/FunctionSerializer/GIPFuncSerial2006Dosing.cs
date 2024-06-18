using gip.core.autocomponent;
using gip.core.communication;
using gip.core.communication.ISOonTCP;
using gip.core.datamodel;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip2006.variobatch.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Serializer for Dosing'}de{'Serialisierer für Dosieren'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPFuncSerial2006Dosing : ACSessionObjSerializer
    {
        public GIPFuncSerial2006Dosing(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            return MethodNameEquals(typeOrACMethodName, "Dosing")
                || MethodNameEquals(typeOrACMethodName, "DosingProd")
                || MethodNameEquals(typeOrACMethodName, "DosingInco")
                || MethodNameEquals(typeOrACMethodName, "DosingOutg")
                || MethodNameEquals(typeOrACMethodName, "DosingRear");
        }

        public override bool SendObject(object complexObj, object prevComplexObj, int dbNo, int offset, int? routeOffset, object miscParams)
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
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Source
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Destination
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // TargetQuantity
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate1
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate2
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate3
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate4
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate5
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate6
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate7
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate8
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate9
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching1
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching2
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching3
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching4
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching5
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching6
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching7
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching8
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching9
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // RoughFineSwitching
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRateRough
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRateFine
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // TolerancePlus
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ToleranceMinus
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Afterflow
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // AfterflowMAX
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // LackOfMatCheckWeight
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // LackOfMatCheckTime
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // PulsationTime
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // PulsationPauseTime
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // AdjustmentTime
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // CalmingTime
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // StandstillTime
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // StandstillWeight
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Temperature
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // LastBatch
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // SprayNozzle
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ImpulsePerLiter
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Density
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // MaxDosingTime
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // AdjustmentFlowRate
            iOffset += gip.core.communication.ISOonTCP.Types.Byte.Length; // EndlessDosing
            iOffset += gip.core.communication.ISOonTCP.Types.Byte.Length; // Additional Byte because S7 only allows 2Byte-Blocks
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // SWTWeight

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

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("TargetQuantity")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("FlowRate1")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("FlowRate2")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("FlowRate3")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("FlowRate4")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("FlowRate5")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("FlowRate6")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("FlowRate7")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("FlowRate8")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("FlowRate9")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("FlowSwitching1")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("FlowSwitching2")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("FlowSwitching3")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("FlowSwitching4")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("FlowSwitching5")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("FlowSwitching6")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("FlowSwitching7")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("FlowSwitching8")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("FlowSwitching9")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("RoughFineSwitching")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("FlowRateRough")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("FlowRateFine")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("TolerancePlus")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("ToleranceMinus")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("Afterflow")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("AfterflowMAX")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("LackOfMatCheckWeight")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            double totalSec = request.ParameterValueList.GetTimeSpan("LackOfMatCheckTime").TotalSeconds;
            if (totalSec > Int16.MaxValue)
                totalSec = Int16.MaxValue;
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(Convert.ToInt16(totalSec)), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            totalSec = request.ParameterValueList.GetTimeSpan("PulsationTime").TotalMilliseconds;
            if (totalSec > Int16.MaxValue)
                totalSec = Int16.MaxValue;
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(Convert.ToInt16(totalSec)), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            totalSec = request.ParameterValueList.GetTimeSpan("PulsationPauseTime").TotalMilliseconds;
            if (totalSec > Int16.MaxValue)
                totalSec = Int16.MaxValue;
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(Convert.ToInt16(totalSec)), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            totalSec = request.ParameterValueList.GetTimeSpan("AdjustmentTime").TotalSeconds;
            if (totalSec > Int16.MaxValue)
                totalSec = Int16.MaxValue;
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(Convert.ToInt16(totalSec)), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            totalSec = request.ParameterValueList.GetTimeSpan("CalmingTime").TotalSeconds;
            if (totalSec > Int16.MaxValue)
                totalSec = Int16.MaxValue;
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(Convert.ToInt16(totalSec)), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            totalSec = request.ParameterValueList.GetTimeSpan("StandstillTime").TotalSeconds;
            if (totalSec > Int16.MaxValue)
                totalSec = Int16.MaxValue;
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(Convert.ToInt16(totalSec)), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("StandstillWeight")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("Temperature")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16(PWMethodVBBase.IsLastBatchParamName)), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("SprayNozzle")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("ImpulsePerLiter")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ParameterValueList.GetDouble("Density")), 
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Real.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

            totalSec = request.ParameterValueList.GetTimeSpan("MaxDosingTime").TotalSeconds;
            if (totalSec > Int16.MaxValue)
                totalSec = Int16.MaxValue;
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(Convert.ToInt16(totalSec)),
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(request.ParameterValueList.GetInt16("AdjustmentFlowRate")),
                0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            var acValue = request.ParameterValueList.GetACValue("EndlessDosing");
            if (acValue != null)
            {
                Array.Copy(gip.core.communication.ISOonTCP.Types.Byte.ToByteArray(Convert.ToByte(acValue.ParamAsBoolean)),
                    0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.Byte.Length);
            }
            iOffset += gip.core.communication.ISOonTCP.Types.Byte.Length;
            iOffset += gip.core.communication.ISOonTCP.Types.Byte.Length; // Additional Byte because S7 only allows 2Byte-Blocks

            acValue = request.ParameterValueList.GetACValue("SWTWeight");
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

            return this.SendObjectToPLC(s7Session, request, sendPackage1, dbNo, offset, iOffset);
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
                int iOffset = 0;
                if (s7Session.HashCodeValidation == HashCodeValidationEnum.Head || s7Session.HashCodeValidation == HashCodeValidationEnum.Head_WithRead)
                    iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;

                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Source
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // Destination
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // TargetQuantity
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate1
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate2
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate3
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate4
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate5
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate6
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate7
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate8
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRate9
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching1
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching2
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching3
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching4
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching5
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching6
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching7
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching8
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowSwitching9
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // RoughFineSwitching
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRateRough
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // FlowRateFine
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // TolerancePlus
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ToleranceMinus
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Afterflow
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // AfterflowMAX
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // LackOfMatCheckWeight
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // LackOfMatCheckTime
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // PulsationTime
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // PulsationPauseTime
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // AdjustmentTime
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // CalmingTime
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // StandstillTime
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // StandstillWeight
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Temperature
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // LastBatch
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // SprayNozzle
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ImpulsePerLiter
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Density
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // MaxDosingTime
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // AdjustmentFlowRate
                iOffset += gip.core.communication.ISOonTCP.Types.Byte.Length; // EndlessDosing
                iOffset += gip.core.communication.ISOonTCP.Types.Byte.Length; // Additional Byte because S7 only allows 2Byte-Blocks
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // SWTWeight

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

                response.ParameterValueList.GetACValue("TargetQuantity").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("FlowRate1").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("FlowRate2").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("FlowRate3").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("FlowRate4").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("FlowRate5").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("FlowRate6").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("FlowRate7").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("FlowRate8").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("FlowRate9").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("FlowSwitching1").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("FlowSwitching2").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("FlowSwitching3").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("FlowSwitching4").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("FlowSwitching5").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("FlowSwitching6").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("FlowSwitching7").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("FlowSwitching8").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("FlowSwitching9").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("RoughFineSwitching").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("FlowRateRough").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("FlowRateFine").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("TolerancePlus").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("ToleranceMinus").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("Afterflow").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("AfterflowMAX").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("LackOfMatCheckWeight").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("LackOfMatCheckTime").Value = TimeSpan.FromSeconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("PulsationTime").Value = TimeSpan.FromMilliseconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("PulsationPauseTime").Value = TimeSpan.FromMilliseconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("AdjustmentTime").Value = TimeSpan.FromSeconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("CalmingTime").Value = TimeSpan.FromSeconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("StandstillTime").Value = TimeSpan.FromSeconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("StandstillWeight").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("Temperature").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName).Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("SprayNozzle").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("ImpulsePerLiter").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("Density").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ParameterValueList.GetACValue("MaxDosingTime").Value = TimeSpan.FromSeconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ParameterValueList.GetACValue("AdjustmentFlowRate").Value = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                var acValue = response.ParameterValueList.GetACValue("EndlessDosing");
                if (acValue != null)
                {
                    try { acValue.Value = Convert.ToBoolean(readPackage1[iOffset]); }
                    catch (Exception e)
                    {
                        string msg = e.Message;
                        if (e.InnerException != null && e.InnerException.Message != null)
                            msg += " Inner:" + e.InnerException.Message;

                        Messages.LogException("GIPFuncSerial2006Dosing", "ReadObject", msg);
                    }
                }
                iOffset += gip.core.communication.ISOonTCP.Types.Byte.Length;
                iOffset += gip.core.communication.ISOonTCP.Types.Byte.Length; // Additional Byte because S7 only allows 2Byte-Blocks

                acValue = response.ParameterValueList.GetACValue("SWTWeight");
                if (acValue != null)
                    acValue.Value =  gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                OnReadObjectAppend(response, dbNo, iOffset, miscParams, readPackage1, readParameter, ref iOffset);
            }
            else
            {
                int iOffset = 0;
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ActualQuantity
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ScaleTotalWeight
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Afterflow
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length; // DosingTime
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // Temperature
                iOffset += 20; // GaugeCode/Alibi
                //iOffset += 23; // GaugeCodeStart
                //iOffset += 23; // GaugeCodeEnd
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // FlowRate
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length; // ImpulseCounter

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

                response.ResultValueList.GetACValue("Afterflow").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ResultValueList.GetACValue("DosingTime").Value = TimeSpan.FromMilliseconds(gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage1, iOffset));
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                response.ResultValueList.GetACValue("Temperature").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ResultValueList.GetACValue("GaugeCode").Value = gip.core.communication.ISOonTCP.Types.String.FromByteArray(readPackage1, iOffset, 23, true);
                iOffset += 20;

                //response.ResultValueList.GetACValue("GaugeCodeStart").Value = gip.core.communication.ISOonTCP.Types.String.FromByteArray(readPackage1, iOffset, 23, true);
                //iOffset += 23;

                //response.ResultValueList.GetACValue("GaugeCodeEnd").Value = gip.core.communication.ISOonTCP.Types.String.FromByteArray(readPackage1, iOffset, 23, true);
                //iOffset += 23;

                response.ResultValueList.GetACValue("FlowRate").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Real.Length;

                response.ResultValueList.GetACValue("ImpulseCounter").Value = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, iOffset);
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
