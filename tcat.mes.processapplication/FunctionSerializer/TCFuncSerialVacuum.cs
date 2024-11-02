// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
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
    [ACClassInfo(Const.PackName_TwinCAT, "en{'TwinCAT Ser. Vacuum'}de{'TwinCAT Ser. Vakuum'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class TCFuncSerialVacuum : ACSessionObjSerializer
    {
        public TCFuncSerialVacuum(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        protected readonly ACMonitorObject _40103_LockWaitHandles = new ACMonitorObject(40103);
        List<TCSerializerWaitHandle> _waitHandles = new List<TCSerializerWaitHandle>();
        private int _RequestCounter = 0;

        internal void OnMethodExecuted(int MethodInvokeRequestID, object MethodResult)
        {

            using (ACMonitor.Lock(_40103_LockWaitHandles))
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
                        gip.core.datamodel.Database.Root.Messages.LogException("TCFuncSerialVacuum", "OnMethodExecuted", msg);
                }
            }
        }

        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            return MethodNameEquals(typeOrACMethodName, "Vacuum")
                || MethodNameEquals(typeOrACMethodName, "VacuumTime");
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
            length += TCDataTypeLength.UInt; // Mode
            length += TCDataTypeLength.LReal; // Pressure
            length += TCDataTypeLength.Bool; // HoldPressure
            length += TCDataTypeLength.Time; //Duration

            int iOffset = 0;
            byte[] paramPackage = new byte[length];

            string instanceACUrl = TCSession.ResolveACUrlToTwinCATUrl(instanceInfo.ACUrlParent + GCL.Delimiter_DirSeperator + instanceInfo.ACIdentifier);
            int instanceIndex = session.Metadata.IndexWhere(c => c._ACUrl == instanceACUrl);
            if (instanceIndex == -1)
                return false;

            Array.Copy(BitConverter.GetBytes(instanceIndex + 1), 0, paramPackage, iOffset, TCDataTypeLength.UDInt);
            iOffset += TCDataTypeLength.UDInt;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetUInt16("Mode")), 0, paramPackage, iOffset, TCDataTypeLength.UInt);
            iOffset += TCDataTypeLength.UInt;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("Pressure")), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
            iOffset += TCDataTypeLength.LReal;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetBoolean("HoldPressure")), 0, paramPackage, iOffset, TCDataTypeLength.Bool);
            iOffset += TCDataTypeLength.Bool;

            if (MethodNameEquals(request.ACIdentifier, "VacuumTime"))
            {
                double totalMiliSec = request.ParameterValueList.GetTimeSpan("Duration").TotalMilliseconds;
                if(totalMiliSec > int.MaxValue)
                    totalMiliSec = int.MaxValue;
                Array.Copy(BitConverter.GetBytes((int)totalMiliSec), 0, paramPackage, iOffset, TCDataTypeLength.Time);
                iOffset += TCDataTypeLength.Time;
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

            if(readParameter)
            {
                length += TCDataTypeLength.String(81); // Mode
                length += TCDataTypeLength.LReal; // Pressure
                length += TCDataTypeLength.Bool; // HoldPressure
                length += TCDataTypeLength.Time; //Duration
            }
            else
            {
                length += TCDataTypeLength.LReal; // ActPressure
                length += TCDataTypeLength.Time; // ActDuration
            }

            TCSerializerWaitHandle newWaitHandle = null;

            using (ACMonitor.Lock(_40103_LockWaitHandles))
            {
                _RequestCounter++;
                newWaitHandle = new TCSerializerWaitHandle(false, EventResetMode.AutoReset, _RequestCounter) { ACMethod = request };
                _waitHandles.Add(newWaitHandle);
            }

            session.ReadResult(childInfo.ACUrlParent + ACUrlHelper.Delimiter_DirSeperator + childInfo.ACIdentifier, length, readParameter, _RequestCounter, request.ACIdentifier);
            if (!newWaitHandle.WaitOne(5000))
                newWaitHandle.TimedOut = true;


            using (ACMonitor.Lock(_40103_LockWaitHandles))
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


            using (ACMonitor.Lock(_40103_LockWaitHandles))
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
                acMethod.ParameterValueList.GetACValue("Mode").Value = Encoding.ASCII.GetString(result, iOffset, TCDataTypeLength.String(81)).Replace("\0", "");
                iOffset += TCDataTypeLength.String(81);

                acMethod.ParameterValueList.GetACValue("Pressure").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ParameterValueList.GetACValue("HoldPressure").Value = BitConverter.ToBoolean(result, iOffset);
                iOffset += TCDataTypeLength.Bool;

                if (MethodNameEquals(acMethod.ACIdentifier, "VacuumTime"))
                    acMethod.ParameterValueList.GetACValue("Duration").Value = TimeSpan.FromMilliseconds(BitConverter.ToDouble(result, iOffset));
            }
            else
            {
                acMethod.ResultValueList.GetACValue("ActPressure").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                if (MethodNameEquals(acMethod.ACIdentifier, "VacuumTime"))
                    acMethod.ResultValueList.GetACValue("ActDuration").Value = TimeSpan.FromMilliseconds(BitConverter.ToDouble(result, iOffset));
            }
            OnMethodExecuted(requestID, acMethod);
        }
    }
}
