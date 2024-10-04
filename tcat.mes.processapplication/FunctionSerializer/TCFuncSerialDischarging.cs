using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.communication;
using gip.core.datamodel;
using gip.core.tcClient;
using System.Threading;
using gip.core.autocomponent;
using gip.core.tcShared;

namespace tcat.mes.processapplication
{
    [ACClassInfo(Const.PackName_TwinCAT, "en{'TwinCAT Ser. Discharging'}de{'TwinCAT Ser. Entleeren'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class TCFuncSerialDischarging : ACSessionObjSerializer
    {
        public TCFuncSerialDischarging(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        protected readonly ACMonitorObject _40100_LockWaitHandles = new ACMonitorObject(40100);
        List<TCSerializerWaitHandle> _waitHandles = new List<TCSerializerWaitHandle>();
        private int _RequestCounter = 0;

        internal void OnMethodExecuted(int MethodInvokeRequestID, object MethodResult)
        {

            using (ACMonitor.Lock(_40100_LockWaitHandles))
            {
                try
                {
                    TCSerializerWaitHandle waitHandle = _waitHandles.FirstOrDefault(c => c.RequestID == MethodInvokeRequestID);
                    if (waitHandle != null)
                    {
                        waitHandle.RemoteMethodInvocationResult = MethodResult;
                        waitHandle.Set();
                    }
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (gip.core.datamodel.Database.Root != null && gip.core.datamodel.Database.Root.Messages != null
                                                                  && gip.core.datamodel.Database.Root.InitState == ACInitState.Initialized)
                        gip.core.datamodel.Database.Root.Messages.LogException("TCFuncSerialDischarging", "OnMethodExecuted", msg);
                }
            }
        }

        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            return MethodNameEquals(typeOrACMethodName, "Discharging")
                || MethodNameEquals(typeOrACMethodName, "DischargingIntake");
        }

        public override bool SendObject(object complexObj, object prevComplexObj, int dbNo, int offset, int? routeOffset, object miscParams)
        {
            TCSession session = ParentACComponent as TCSession;
            if (session == null && complexObj == null)
                return false;

            ACMethod request = complexObj as ACMethod;
            (ACChildInstanceInfo instanceInfo, IACComponent invokerModule) = GetSendParameters(miscParams, false);

            if (request == null && instanceInfo == null)
                return false;

            int length = 0;

            length += TCDataTypeLength.UDInt; //InstanceID
            length += TCDataTypeLength.Int; // Source
            length += TCDataTypeLength.Array(TCDataTypeLength.UDInt, 20); //Route 
            length += TCDataTypeLength.Int; //Destination
            length += TCDataTypeLength.LReal; //EmptyWeight
            length += TCDataTypeLength.Time; //DischargingTime
            length += TCDataTypeLength.Time; //PulsationTime
            length += TCDataTypeLength.Time; //PulsationPauseTime
            length += TCDataTypeLength.LReal; //IdleCurrent
            length += TCDataTypeLength.Int; //InterDischarging
            length += TCDataTypeLength.Int; //Vibrator
            length += TCDataTypeLength.LReal; //Power
            length += TCDataTypeLength.LReal; //Tolerance
            length += TCDataTypeLength.LReal; //Tolerance-
            length += TCDataTypeLength.Int; //LastBatch
            length += TCDataTypeLength.Int; //Sieve
            length += TCDataTypeLength.LReal; //TargetQuantity
            if (MethodNameEquals(request.ACIdentifier, "DischargingIntake"))
            {
                length += TCDataTypeLength.LReal; //ScaleBatchWeight
                length += TCDataTypeLength.Int; //Scale
            }

            int iOffset = 0;
            byte[] paramPackage = new byte[length];

            string instanceACUrl = TCSession.ResolveACUrlToTwinCATUrl(instanceInfo.ACUrlParent + GCL.Delimiter_DirSeperator + instanceInfo.ACIdentifier);
            int instanceIndex = session.Metadata.IndexWhere(c => c._ACUrl == instanceACUrl);
            if (instanceIndex == -1)
                return false;

            Array.Copy(BitConverter.GetBytes(instanceIndex + 1), 0, paramPackage, iOffset, TCDataTypeLength.UDInt);
            iOffset += TCDataTypeLength.UDInt;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("Source")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
            iOffset += TCDataTypeLength.Int;

            Route route = request.ParameterValueList.FirstOrDefault(c => c.ACIdentifier == nameof(Route)).Value as Route;
            if (route != null && route.Any())
            {
                int iOffsetRoute = iOffset;
                foreach (RouteItem item in route)
                {
                    string tcEdgeKey = item.SourceACComponent.ACUrl + "\\" + item.SourceACPoint.ACIdentifier + "|" + item.TargetACComponent.ACUrl + "\\" + item.TargetACPoint.ACIdentifier;
                    uint instanceID;
                    if (session.TCEdgesDict.TryGetValue(tcEdgeKey, out instanceID))
                    {
                        byte[] tempArray = BitConverter.GetBytes(instanceID);
                        Array.Copy(tempArray, 0, paramPackage, iOffsetRoute, TCDataTypeLength.UDInt);
                        iOffsetRoute += TCDataTypeLength.UDInt;
                    }
                }
            }
            iOffset += TCDataTypeLength.Array(TCDataTypeLength.UDInt, 20);

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("Destination")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
            iOffset += TCDataTypeLength.Int;

            double emptyWeight = 0;
            var acValue = request.ParameterValueList.GetACValue("EmptyWeight");
            if (acValue.ValueT<Double?>().HasValue)
                emptyWeight = acValue.ValueT<Double?>().Value;
            Array.Copy(BitConverter.GetBytes(emptyWeight), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
            iOffset += TCDataTypeLength.LReal;

            Array.Copy(BitConverter.GetBytes((int)request.ParameterValueList.GetTimeSpan("DischargingTime").TotalMilliseconds), 0, paramPackage, iOffset, TCDataTypeLength.Time);
            iOffset += TCDataTypeLength.Time;

            Array.Copy(BitConverter.GetBytes((int)request.ParameterValueList.GetTimeSpan("PulsationTime").TotalMilliseconds), 0, paramPackage, iOffset, TCDataTypeLength.Time);
            iOffset += TCDataTypeLength.Time;

            Array.Copy(BitConverter.GetBytes((int)request.ParameterValueList.GetTimeSpan("PulsationPauseTime").TotalMilliseconds), 0, paramPackage, iOffset, TCDataTypeLength.Time);
            iOffset += TCDataTypeLength.Time;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("IdleCurrent")), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
            iOffset += TCDataTypeLength.LReal;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("InterDischarging")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
            iOffset += TCDataTypeLength.Int;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("Vibrator")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
            iOffset += TCDataTypeLength.Int;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("Power")), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
            iOffset += TCDataTypeLength.LReal;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("Tolerance")), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
            iOffset += TCDataTypeLength.LReal;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("ToleranceMin")), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
            iOffset += TCDataTypeLength.LReal;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("LastBatch")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
            iOffset += TCDataTypeLength.Int;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("Sieve")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
            iOffset += TCDataTypeLength.Int;

            double targetQuantity = 0.0;
            acValue = request.ParameterValueList.GetACValue("TargetQuantity");
            if (acValue != null)
                targetQuantity = acValue.ParamAsDouble;
            Array.Copy(BitConverter.GetBytes(targetQuantity), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
            iOffset += TCDataTypeLength.LReal;

            if (MethodNameEquals(request.ACIdentifier, "DischargingIntake"))
            {
                Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("ScaleBatchWeight")), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
                iOffset += TCDataTypeLength.LReal;

                Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("Scale")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
                iOffset += TCDataTypeLength.Int;
            }

            session.SendParameters(paramPackage);

            return true;
        }

        public override object ReadObject(object complexObj, int dbNo, int offset, int? routeOffset, object miscParams)
        {
            TCSession session = ParentACComponent as TCSession;
            if (session == null && complexObj == null)
                return null;

            ACMethod request = complexObj as ACMethod;

            if (request == null)
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

            int length = 0;

            if (readParameter)
            {
                length += TCDataTypeLength.Int; // Source
                length += TCDataTypeLength.Int; // Destination
                length += TCDataTypeLength.LReal; // EmptyWeight
                length += TCDataTypeLength.Time; // DischargingTime
                length += TCDataTypeLength.Time; // PulsationTime
                length += TCDataTypeLength.Time; // PulsationPauseTime
                length += TCDataTypeLength.LReal; // IdleCurrent
                length += TCDataTypeLength.Int; // InterDischarging
                length += TCDataTypeLength.Int; // Vibrator
                length += TCDataTypeLength.LReal; // Power
                length += TCDataTypeLength.LReal; // Tolerance +
                length += TCDataTypeLength.LReal; // Tolerance -
                length += TCDataTypeLength.LReal; // TargetWeight -
                length += TCDataTypeLength.Int; // LastBatch
                length += TCDataTypeLength.Int; // Sieve
            }
            else
            {
                length += TCDataTypeLength.LReal; // ActualQuantity
                length += TCDataTypeLength.LReal; // ScaleTotalWeight
                length += TCDataTypeLength.Time; // DischargingTime
            }

            TCSerializerWaitHandle newWaitHandle = null;

            using (ACMonitor.Lock(_40100_LockWaitHandles))
            {
                _RequestCounter++;
                newWaitHandle = new TCSerializerWaitHandle(false, EventResetMode.AutoReset, _RequestCounter) { ACMethod = request };
                _waitHandles.Add(newWaitHandle);
            }

            session.ReadResult(childInfo.ACUrlParent + ACUrlHelper.Delimiter_DirSeperator + childInfo.ACIdentifier, length, readParameter, _RequestCounter, request.ACIdentifier);
            if (!newWaitHandle.WaitOne(5000))
                newWaitHandle.TimedOut = true;


            using (ACMonitor.Lock(_40100_LockWaitHandles))
            {
                newWaitHandle.ACMethod = null;
                newWaitHandle.Close();
                _waitHandles.Remove(newWaitHandle);
            }

            return newWaitHandle.RemoteMethodInvocationResult;
        }

        public override void OnObjectRead(byte[] result)
        {
            bool readParam = BitConverter.ToBoolean(result, 4);
            int requestID = BitConverter.ToInt32(result, 0);
            ACMethod acMethod = null;


            using (ACMonitor.Lock(_40100_LockWaitHandles))
            {
                TCSerializerWaitHandle handle = _waitHandles.FirstOrDefault(c => c.RequestID == requestID);
                if (handle != null)
                    acMethod = handle.ACMethod as ACMethod;
            }

            if (acMethod == null)
            {
                OnMethodExecuted(requestID, null);
                return;
            }

            int iOffset = 5;

            if(readParam)
            {
                acMethod.ParameterValueList.GetACValue("Source").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;

                acMethod.ParameterValueList.GetACValue("Destination").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;

                acMethod.ParameterValueList.GetACValue("EmptyWeight").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ParameterValueList.GetACValue("DischargingTime").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Time;

                acMethod.ParameterValueList.GetACValue("PulsationTime").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Time;

                acMethod.ParameterValueList.GetACValue("PulsationPauseTime").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Time;

                acMethod.ParameterValueList.GetACValue("IdleCurrent").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ParameterValueList.GetACValue("InterDischarging").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;

                acMethod.ParameterValueList.GetACValue("Vibrator").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;

                acMethod.ParameterValueList.GetACValue("Power").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ParameterValueList.GetACValue("Tolerance").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ParameterValueList.GetACValue("ToleranceMin").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                var acValue = acMethod.ParameterValueList.GetACValue("TargetQuantity");
                if (acValue != null)
                    acValue.Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ParameterValueList.GetACValue("LastBatch").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;

                acMethod.ParameterValueList.GetACValue("Sieve").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;
            }
            else
            {
                acMethod.ResultValueList.GetACValue("ActualQuantity").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ResultValueList.GetACValue("ScaleTotalWeight").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ResultValueList.GetACValue("DischargingTime").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Int;
            }
            OnMethodExecuted(requestID, acMethod);
        }
    }
}
