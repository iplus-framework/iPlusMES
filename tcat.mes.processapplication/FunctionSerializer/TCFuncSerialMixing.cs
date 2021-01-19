using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.communication;
using gip.core.datamodel;
using gip.core.tcClient;
using gip.core.tcShared;
using System.Threading;
using gip.core.autocomponent;

namespace tcat.mes.processapplication
{
    [ACClassInfo(Const.PackName_TwinCAT, "en{'Serializer for Mixing'}de{'Serialisierer für Mischen'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.Required, false, false)]
    public class TCFuncSerialMixing : ACSessionObjSerializer
    {
        public TCFuncSerialMixing(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        protected readonly ACMonitorObject _40102_LockWaitHandles = new ACMonitorObject(40102);
        List<TCSerializerWaitHandle> _waitHandles = new List<TCSerializerWaitHandle>();
        private int _RequestCounter = 0;

        internal void OnMethodExecuted(int MethodInvokeRequestID, object MethodResult)
        {

            using (ACMonitor.Lock(_40102_LockWaitHandles))
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
                        gip.core.datamodel.Database.Root.Messages.LogException("TCFuncSerialMixing", "OnMethodExecuted", msg);
                }
            }
        }

        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            return MethodNameEquals(typeOrACMethodName, "Mixing")
                || MethodNameEquals(typeOrACMethodName, "MixingTime")
                || MethodNameEquals(typeOrACMethodName, "MixingTemperature");
        }

        public override bool SendObject(object complexObj, int dbNo, int offset, object miscParams)
        {
            TCSession session = ParentACComponent as TCSession;
            if (session == null && complexObj == null)
                return false;

            ACMethod request = complexObj as ACMethod;
            ACChildInstanceInfo instanceInfo = miscParams as ACChildInstanceInfo;

            if (request == null && instanceInfo == null)
                return false;

            int length = 0;

            length += TCDataTypeLength.UDInt; //InstanceID
            length += TCDataTypeLength.Bool; // SwitchOff
            length += TCDataTypeLength.Int; // Speed
            length += TCDataTypeLength.LReal; // MinWeight
            length += TCDataTypeLength.Time; // Duration
            length += TCDataTypeLength.LReal; //Temperature

            int iOffset = 0;
            byte[] paramPackage = new byte[length];

            string instanceACUrl = instanceInfo.ACUrlParent + "._" + instanceInfo.ACIdentifier;
            instanceACUrl = "_VB" + instanceACUrl.Replace("\\", "._");
            int instanceIndex = session.Metadata.IndexWhere(c => c._ACUrl == instanceACUrl);

            if (instanceIndex == -1)
                return false;

            Array.Copy(BitConverter.GetBytes(instanceIndex + 1), 0, paramPackage, iOffset, TCDataTypeLength.UDInt);
            iOffset += TCDataTypeLength.UDInt;

            if (MethodNameEquals(request.ACIdentifier, "Mixing"))
            {
                Array.Copy(BitConverter.GetBytes(0), 0, paramPackage, iOffset, TCDataTypeLength.Bool);
                iOffset += TCDataTypeLength.Bool;

                Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("Speed")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
                iOffset += TCDataTypeLength.Int;

                Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("MinWeight")), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
                iOffset += TCDataTypeLength.LReal;
            }
            else if (MethodNameEquals(request.ACIdentifier, "MixingTime"))
            {
                iOffset += TCDataTypeLength.Bool;

                Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("Speed")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
                iOffset += TCDataTypeLength.Int;

                Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("MinWeight")), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
                iOffset += TCDataTypeLength.LReal;

                double totalMiliSec = request.ParameterValueList.GetTimeSpan("Duration").TotalMilliseconds;
                if (totalMiliSec > Int32.MaxValue)
                    totalMiliSec = Int32.MaxValue;
                Array.Copy(BitConverter.GetBytes((int)totalMiliSec), 0, paramPackage, iOffset, TCDataTypeLength.Time);
                iOffset += TCDataTypeLength.Time;

            }
            else if (MethodNameEquals(request.ACIdentifier, "MixingTemperature"))
            {
                iOffset += TCDataTypeLength.Bool;

                Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("Speed")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
                iOffset += TCDataTypeLength.Int;

                Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("MinWeight")), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
                iOffset += TCDataTypeLength.LReal;

                iOffset += TCDataTypeLength.Time;

                Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("Temperature")), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
                iOffset += TCDataTypeLength.LReal;
            }

            session.SendParameters(paramPackage);

            return true;
        }

        public override object ReadObject(object complexObj, int dbNo, int offset, object miscParams)
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
            if(readParameter)
            {
                length += TCDataTypeLength.Int; // Speed
                length += TCDataTypeLength.LReal; // MinWeight
                length += TCDataTypeLength.Time; // Duration
                length += TCDataTypeLength.LReal; //Temperature
            }
            else
            {
                length += TCDataTypeLength.Time; // ActDuration
                length += TCDataTypeLength.LReal; // ActTemperature
            }

            TCSerializerWaitHandle newWaitHandle = null;

            using (ACMonitor.Lock(_40102_LockWaitHandles))
            {
                _RequestCounter++;
                newWaitHandle = new TCSerializerWaitHandle(false, EventResetMode.AutoReset, _RequestCounter) { ACMethod = request };
                _waitHandles.Add(newWaitHandle);
            }

            session.ReadResult(childInfo.ACUrlParent + "\\" + childInfo.ACIdentifier, length, readParameter, _RequestCounter, request.ACIdentifier);
            if (!newWaitHandle.WaitOne(5000))
                newWaitHandle.TimedOut = true;


            using (ACMonitor.Lock(_40102_LockWaitHandles))
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


            using (ACMonitor.Lock(_40102_LockWaitHandles))
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

            if (readParam)
            {
                if (MethodNameEquals(acMethod.ACIdentifier, "Mixing"))
                {
                    //acMethod.ParameterValueList.GetACValue("Duration").Value = BitConverter.ToInt16(result, iOffset);
                    iOffset += TCDataTypeLength.Int;

                    acMethod.ParameterValueList.GetACValue("Speed").Value = BitConverter.ToInt16(result, iOffset);
                    iOffset += TCDataTypeLength.Int;

                    //acMethod.ParameterValueList.GetACValue("Temperature").Value = BitConverter.ToDouble(result, iOffset);
                    iOffset += TCDataTypeLength.LReal;

                    acMethod.ParameterValueList.GetACValue("MinWeight").Value = BitConverter.ToDouble(result, iOffset);
                    iOffset += TCDataTypeLength.LReal;
                }
                else if (MethodNameEquals(acMethod.ACIdentifier, "MixingTime"))
                {
                    acMethod.ParameterValueList.GetACValue("Duration").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                    iOffset += TCDataTypeLength.Time;

                    acMethod.ParameterValueList.GetACValue("Speed").Value = BitConverter.ToInt16(result, iOffset);
                    iOffset += TCDataTypeLength.Int;

                    //acMethod.ParameterValueList.GetACValue("Temperature").Value = BitConverter.ToDouble(result, iOffset);
                    iOffset += TCDataTypeLength.LReal;

                    acMethod.ParameterValueList.GetACValue("MinWeight").Value = BitConverter.ToDouble(result, iOffset);
                    iOffset += TCDataTypeLength.LReal;
                }
                else if (MethodNameEquals(acMethod.ACIdentifier, "MixingTemperature"))
                {
                    //acMethod.ParameterValueList.GetACValue("Duration").Value = BitConverter.ToInt16(result, iOffset);
                    iOffset += TCDataTypeLength.Int;

                    acMethod.ParameterValueList.GetACValue("Speed").Value = BitConverter.ToInt16(result, iOffset);
                    iOffset += TCDataTypeLength.Int;

                    acMethod.ParameterValueList.GetACValue("Temperature").Value = BitConverter.ToDouble(result, iOffset);
                    iOffset += TCDataTypeLength.LReal;

                    acMethod.ParameterValueList.GetACValue("MinWeight").Value = BitConverter.ToDouble(result, iOffset);
                    iOffset += TCDataTypeLength.LReal;
                }
            }
            else
            {
                var acValue = acMethod.ResultValueList.GetACValue("ActDuration");
                if (acValue != null)
                    acValue.Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Time;

                acValue = acMethod.ResultValueList.GetACValue("ActTemperature");
                if (acValue != null)
                    acMethod.ResultValueList.GetACValue("ActTemperature").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            }
            OnMethodExecuted(requestID, acMethod);
        }
    }
}
