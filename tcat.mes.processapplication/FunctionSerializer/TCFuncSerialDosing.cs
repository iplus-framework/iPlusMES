using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.communication;
using gip.core.datamodel;
using gip.core.tcShared;
using gip.core.tcClient;
using gip.core.autocomponent;
using System.Threading;

namespace tcat.mes.processapplication
{
    [ACClassInfo(Const.PackName_TwinCAT, "en{'TwinCAT Ser. Dosing'}de{'TwinCAT Ser. Dosieren'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class TCFuncSerialDosing : ACSessionObjSerializer
    {
        public TCFuncSerialDosing(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        protected readonly ACMonitorObject _40101_LockWaitHandles = new ACMonitorObject(40101);
        List<TCSerializerWaitHandle> _waitHandles = new List<TCSerializerWaitHandle>();
        private int _RequestCounter = 0;

        internal void OnMethodExecuted(int MethodInvokeRequestID, object MethodResult)
        {

            using (ACMonitor.Lock(_40101_LockWaitHandles))
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
                        gip.core.datamodel.Database.Root.Messages.LogException("TCFuncSerialDosing", "OnMethodExecuted", msg);
                }
            }
        }

        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            return MethodNameEquals(typeOrACMethodName, "Dosing")
                || MethodNameEquals(typeOrACMethodName, "DosingProd")
                || MethodNameEquals(typeOrACMethodName, "DosingInco")
                || MethodNameEquals(typeOrACMethodName, "DosingOutg")
                || MethodNameEquals(typeOrACMethodName, "DosingRear");
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

            length += TCDataTypeLength.UDInt ; //InstanceID
            length += TCDataTypeLength.LReal; //TargetQuantity LReal
            length += TCDataTypeLength.Int;  //FlowRate1 Int
            length += TCDataTypeLength.String(81); //Material String
            length += TCDataTypeLength.String(81); //PLPosRelation String
            length += TCDataTypeLength.Array(TCDataTypeLength.UDInt, 20); //Route Int Array[20]
            length += TCDataTypeLength.Int;  //Source Int
            length += TCDataTypeLength.Int;  //Destination Int
            length += TCDataTypeLength.Int;  //FlowRate2 Int
            length += TCDataTypeLength.Int;  //FlowRate3 Int
            length += TCDataTypeLength.Int;  //FlowRate4 Int
            length += TCDataTypeLength.Int;  //FlowRate5 Int
            length += TCDataTypeLength.Int;  //FlowRate6 Int
            length += TCDataTypeLength.Int;  //FlowRate7 Int
            length += TCDataTypeLength.Int;  //FlowRate8 Int
            length += TCDataTypeLength.Int;  //FlowRate9 Int
            length += TCDataTypeLength.LReal;  //FlowSwitching1 LReal
            length += TCDataTypeLength.LReal;  //FlowSwitching2 LReal
            length += TCDataTypeLength.LReal;  //FlowSwitching3 LReal
            length += TCDataTypeLength.LReal;  //FlowSwitching4 LReal
            length += TCDataTypeLength.LReal;  //FlowSwitching5 LReal
            length += TCDataTypeLength.LReal;  //FlowSwitching6 LReal
            length += TCDataTypeLength.LReal;  //FlowSwitching7 LReal
            length += TCDataTypeLength.LReal;  //FlowSwitching8 LReal
            length += TCDataTypeLength.LReal;  //FlowSwitching9 LReal
            length += TCDataTypeLength.LReal;  //RoughFineSwitching LReal
            length += TCDataTypeLength.Int;  //FlowRateRought Int
            length += TCDataTypeLength.Int;  //FlowRateFine Int
            length += TCDataTypeLength.LReal;  //TolerancePlus LReal
            length += TCDataTypeLength.LReal;  //ToleranceMinus LReal
            length += TCDataTypeLength.LReal;  //Afterflow LReal
            length += TCDataTypeLength.LReal;  //AfterflowMAX LReal
            length += TCDataTypeLength.LReal;  //LackOfMatCheckWeight LReal
            length += TCDataTypeLength.Time;  //LackOfMatCheckTime Time
            length += TCDataTypeLength.Time;  //PulsationTime Time
            length += TCDataTypeLength.Time;  //PulsationPauseTime Time
            length += TCDataTypeLength.Time;  //AdjustmentTime Time
            length += TCDataTypeLength.Time;  //CalmingTime Time
            length += TCDataTypeLength.Time;  //StandstillTime Time
            length += TCDataTypeLength.LReal;  //StandstillWeight LReal
            length += TCDataTypeLength.LReal;  //Temperature LReal
            length += TCDataTypeLength.Int;  //LastBatch Int
            length += TCDataTypeLength.Int;  //SprayNozzle Int
            length += TCDataTypeLength.LReal;  //ImpulsePerLiter LReal
            length += TCDataTypeLength.LReal;  //Density LReal
            length += TCDataTypeLength.Time;  //MaxDosingTime Time
            length += TCDataTypeLength.Int;  //AdjustmentFlowRate Int
            length += TCDataTypeLength.Bool;  //EndlessDosing Bool

            int iOffset = 0;
            byte[] paramPackage = new byte[length];
            byte[] stringTemp;

            string instanceACUrl = TCSession.ResolveACUrlToTwinCATUrl(instanceInfo.ACUrlParent + GCL.Delimiter_DirSeperator + instanceInfo.ACIdentifier);
            int instanceIndex = session.Metadata.IndexWhere(c => c._ACUrl == instanceACUrl);

            if (instanceIndex == -1)
                return false;

            Array.Copy(BitConverter.GetBytes(instanceIndex + 1), 0, paramPackage, iOffset, TCDataTypeLength.UDInt);
            iOffset += TCDataTypeLength.UDInt;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("TargetQuantity")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("FlowRate1")), 0, paramPackage, iOffset, TCDataTypeLength.Int); 
            iOffset += TCDataTypeLength.Int;

            string materialParam = "";
            if (request.ParameterValueList.FirstOrDefault(c => c.ACIdentifier == "Material").Value != null)
                materialParam = request.ParameterValueList.GetString("Material");
            stringTemp = Encoding.UTF8.GetBytes(materialParam);
            Array.Copy(stringTemp, 0, paramPackage, iOffset, stringTemp.Length);
            iOffset += TCDataTypeLength.String(81);

            string plPosRelationParam = "";
            //if (request.ParameterValueList.FirstOrDefault(c => c.ACIdentifier == "PLPosRelation").Value != null)
                //plPosRelationParam = request.ParameterValueList.GetString("PLPosRelation");
            stringTemp = Encoding.UTF8.GetBytes(plPosRelationParam);
            Array.Copy(stringTemp, 0, paramPackage, iOffset, stringTemp.Length);
            iOffset += TCDataTypeLength.String(81);

            Route route = request.ParameterValueList.FirstOrDefault(c => c.ACIdentifier == "Route").Value as Route;
            if(route != null && route.Any())
            {
                int iOffsetRoute = iOffset;
                foreach(RouteItem item in route)
                {
                    string tcEdgeKey = item.SourceACComponent.ACUrl + "\\" + item.SourceACPoint.ACIdentifier + "|" + item.TargetACComponent.ACUrl +"\\"+ item.TargetACPoint.ACIdentifier;
                    uint instanceID;
                    if(session.TCEdgesDict.TryGetValue(tcEdgeKey, out instanceID))
                    {
                        byte[] tempArray = BitConverter.GetBytes(instanceID);
                        Array.Copy(tempArray, 0, paramPackage, iOffsetRoute, TCDataTypeLength.UDInt);
                        iOffsetRoute += TCDataTypeLength.UDInt;
                    }
                }
            }
            iOffset += TCDataTypeLength.Array(TCDataTypeLength.UDInt, 20);

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("Source")), 0, paramPackage, iOffset, TCDataTypeLength.Int); 
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("Destination")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("FlowRate2")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("FlowRate3")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("FlowRate4")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("FlowRate5")), 0, paramPackage, iOffset, TCDataTypeLength.Int); 
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("FlowRate6")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("FlowRate7")), 0, paramPackage, iOffset, TCDataTypeLength.Int);
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("FlowRate8")), 0, paramPackage, iOffset, TCDataTypeLength.Int); 
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("FlowRate9")), 0, paramPackage, iOffset, TCDataTypeLength.Int); 
            iOffset += TCDataTypeLength.Int;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("FlowSwitching1")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("FlowSwitching2")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
           
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("FlowSwitching3")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("FlowSwitching4")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("FlowSwitching5")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
           
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("FlowSwitching6")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("FlowSwitching7")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("FlowSwitching8")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("FlowSwitching9")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("RoughFineSwitching")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("FlowRateRough")), 0, paramPackage, iOffset, TCDataTypeLength.Int); 
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("FlowRateFine")), 0, paramPackage, iOffset, TCDataTypeLength.Int); 
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("TolerancePlus")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("ToleranceMinus")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("Afterflow")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
           
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("AfterflowMAX")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;

            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("LackOfMatCheckWeight")), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
            iOffset += TCDataTypeLength.LReal;

            Array.Copy(BitConverter.GetBytes((int)request.ParameterValueList.GetTimeSpan("LackOfMatCheckTime").TotalMilliseconds), 0, paramPackage, iOffset, TCDataTypeLength.Time); 
            iOffset += TCDataTypeLength.Time;
            
            Array.Copy(BitConverter.GetBytes((int)request.ParameterValueList.GetTimeSpan("PulsationTime").TotalMilliseconds), 0, paramPackage, iOffset, TCDataTypeLength.Time); 
            iOffset += TCDataTypeLength.Time;
            
            Array.Copy(BitConverter.GetBytes((int)request.ParameterValueList.GetTimeSpan("PulsationPauseTime").TotalMilliseconds), 0, paramPackage, iOffset, TCDataTypeLength.Time); 
            iOffset += TCDataTypeLength.Time;
            
            Array.Copy(BitConverter.GetBytes((int)request.ParameterValueList.GetTimeSpan("AdjustmentTime").TotalMilliseconds), 0, paramPackage, iOffset, TCDataTypeLength.Time); 
            iOffset += TCDataTypeLength.Time;
            
            Array.Copy(BitConverter.GetBytes((int)request.ParameterValueList.GetTimeSpan("CalmingTime").TotalMilliseconds), 0, paramPackage, iOffset, TCDataTypeLength.Time); 
            iOffset += TCDataTypeLength.Time;
            
            Array.Copy(BitConverter.GetBytes((int)request.ParameterValueList.GetTimeSpan("StandstillTime").TotalMilliseconds), 0, paramPackage, iOffset, TCDataTypeLength.Time); 
            iOffset += TCDataTypeLength.Time;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("StandstillWeight")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("Temperature")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("LastBatch")), 0, paramPackage, iOffset, TCDataTypeLength.Int); 
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("SprayNozzle")), 0, paramPackage, iOffset, TCDataTypeLength.Int); 
            iOffset += TCDataTypeLength.Int;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("ImpulsePerLiter")), 0, paramPackage, iOffset, TCDataTypeLength.LReal);
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetDouble("Density")), 0, paramPackage, iOffset, TCDataTypeLength.LReal); 
            iOffset += TCDataTypeLength.LReal;
            
            Array.Copy(BitConverter.GetBytes((int)request.ParameterValueList.GetTimeSpan("MaxDosingTime").TotalMilliseconds), 0, paramPackage, iOffset, TCDataTypeLength.Time); 
            iOffset += TCDataTypeLength.Time;
            
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetInt16("AdjustmentFlowRate")), 0, paramPackage, iOffset, TCDataTypeLength.Int); 
            iOffset += TCDataTypeLength.Int;
           
            Array.Copy(BitConverter.GetBytes(request.ParameterValueList.GetBoolean("EndlessDosing")), 0, paramPackage, iOffset, TCDataTypeLength.Bool); 
            iOffset += TCDataTypeLength.Bool;

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

            if (readParameter)
            {
                length += TCDataTypeLength.UDInt; //InstanceID
                length += TCDataTypeLength.LReal; //TargetQuantity LReal
                length += TCDataTypeLength.Int;  //FlowRate1 Int
                length += TCDataTypeLength.String(81); //Material String
                length += TCDataTypeLength.String(81); //PLPosRelation String
                length += TCDataTypeLength.Array(TCDataTypeLength.UDInt, 20); //Route Int Array[20]
                length += TCDataTypeLength.Int;  //Source Int
                length += TCDataTypeLength.Int;  //Destination Int
                length += TCDataTypeLength.Int;  //FlowRate2 Int
                length += TCDataTypeLength.Int;  //FlowRate3 Int
                length += TCDataTypeLength.Int;  //FlowRate4 Int
                length += TCDataTypeLength.Int;  //FlowRate5 Int
                length += TCDataTypeLength.Int;  //FlowRate6 Int
                length += TCDataTypeLength.Int;  //FlowRate7 Int
                length += TCDataTypeLength.Int;  //FlowRate8 Int
                length += TCDataTypeLength.Int;  //FlowRate9 Int
                length += TCDataTypeLength.LReal;  //FlowSwitching1 LReal
                length += TCDataTypeLength.LReal;  //FlowSwitching2 LReal
                length += TCDataTypeLength.LReal;  //FlowSwitching3 LReal
                length += TCDataTypeLength.LReal;  //FlowSwitching4 LReal
                length += TCDataTypeLength.LReal;  //FlowSwitching5 LReal
                length += TCDataTypeLength.LReal;  //FlowSwitching6 LReal
                length += TCDataTypeLength.LReal;  //FlowSwitching7 LReal
                length += TCDataTypeLength.LReal;  //FlowSwitching8 LReal
                length += TCDataTypeLength.LReal;  //FlowSwitching9 LReal
                length += TCDataTypeLength.LReal;  //RoughFineSwitching LReal
                length += TCDataTypeLength.Int;  //FlowRateRought Int
                length += TCDataTypeLength.Int;  //FlowRateFine Int
                length += TCDataTypeLength.LReal;  //TolerancePlus LReal
                length += TCDataTypeLength.LReal;  //ToleranceMinus LReal
                length += TCDataTypeLength.LReal;  //Afterflow LReal
                length += TCDataTypeLength.LReal;  //AfterflowMAX LReal
                length += TCDataTypeLength.LReal;  //LackOfMatCheckWeight LReal
                length += TCDataTypeLength.Time;  //LackOfMatCheckTime Time
                length += TCDataTypeLength.Time;  //PulsationTime Time
                length += TCDataTypeLength.Time;  //PulsationPauseTime Time
                length += TCDataTypeLength.Time;  //AdjustmentTime Time
                length += TCDataTypeLength.Time;  //CalmingTime Time
                length += TCDataTypeLength.Time;  //StandstillTime Time
                length += TCDataTypeLength.LReal;  //StandstillWeight LReal
                length += TCDataTypeLength.LReal;  //Temperature LReal
                length += TCDataTypeLength.Int;  //LastBatch Int
                length += TCDataTypeLength.Int;  //SprayNozzle Int
                length += TCDataTypeLength.LReal;  //ImpulsePerLiter LReal
                length += TCDataTypeLength.LReal;  //Density LReal
                length += TCDataTypeLength.Time;  //MaxDosingTime Time
                length += TCDataTypeLength.Int;  //AdjustmentFlowRate Int
                length += TCDataTypeLength.Bool;  //EndlessDosing Bool
            }
            else
            {
                length += TCDataTypeLength.LReal; // ActualQuantity
                length += TCDataTypeLength.LReal; // ScaleTotalWeight
                length += TCDataTypeLength.LReal; // Afterflow
                length += TCDataTypeLength.Time; // DosingTime
                length += TCDataTypeLength.LReal; // Temperature
                length += TCDataTypeLength.String(81); // GaugeCode
                length += TCDataTypeLength.String(81); // GaugeCodeStart
                length += TCDataTypeLength.String(81); // GaugeCodeEnd
                length += TCDataTypeLength.LReal; // FlowRate
                length += TCDataTypeLength.LReal; // ImpulseCounter
            }

            TCSerializerWaitHandle newWaitHandle = null;

            using (ACMonitor.Lock(_40101_LockWaitHandles))
            {
                _RequestCounter++;
                newWaitHandle = new TCSerializerWaitHandle(false, EventResetMode.AutoReset, _RequestCounter) { ACMethod = request };
                _waitHandles.Add(newWaitHandle);
            }

            session.ReadResult(childInfo.ACUrlParent + ACUrlHelper.Delimiter_DirSeperator + childInfo.ACIdentifier, length, readParameter, _RequestCounter, request.ACIdentifier);
            if (!newWaitHandle.WaitOne(5000))
                newWaitHandle.TimedOut = true;


            using (ACMonitor.Lock(_40101_LockWaitHandles))
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

            using(ACMonitor.Lock(_40101_LockWaitHandles))
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
                acMethod.ParameterValueList.GetACValue("TargetQuantity").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("FlowRate1").Value = BitConverter.ToInt16(result, iOffset) ;
                iOffset += TCDataTypeLength.Int;

                var acValueMaterial = acMethod.ParameterValueList.GetACValue("MaterialName");
                if (acValueMaterial != null)
                    acValueMaterial.Value = Encoding.ASCII.GetString(result, iOffset, TCDataTypeLength.String(81)).Replace("\0", "");
                iOffset += TCDataTypeLength.String(81);

                var acValueRelation = acMethod.ParameterValueList.GetACValue("PLPosRelation");
                if (acValueRelation != null)
                    acValueRelation.Value = Encoding.ASCII.GetString(result, iOffset, TCDataTypeLength.String(81)).Replace("\0","");
                iOffset += TCDataTypeLength.String(81);

                //route:todo
                iOffset += TCDataTypeLength.Array(TCDataTypeLength.UDInt, 20);

                acMethod.ParameterValueList.GetACValue("Source").Value = BitConverter.ToInt16(result, iOffset); 
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("Destination").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("FlowRate2").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("FlowRate3").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("FlowRate4").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("FlowRate5").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("FlowRate6").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("FlowRate7").Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("FlowRate8").Value = BitConverter.ToInt16(result, iOffset); 
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("FlowRate9").Value = BitConverter.ToInt16(result, iOffset); 
                iOffset += TCDataTypeLength.Int;

                acMethod.ParameterValueList.GetACValue("FlowSwitching1").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("FlowSwitching2").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
           
                acMethod.ParameterValueList.GetACValue("FlowSwitching3").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("FlowSwitching4").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("FlowSwitching5").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
           
                acMethod.ParameterValueList.GetACValue("FlowSwitching6").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("FlowSwitching7").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("FlowSwitching8").Value = BitConverter.ToDouble(result, iOffset); 
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("FlowSwitching9").Value = BitConverter.ToDouble(result, iOffset); 
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("RoughFineSwitching").Value = BitConverter.ToDouble(result, iOffset); 
                iOffset += TCDataTypeLength.LReal;

                acMethod.ParameterValueList.GetACValue("FlowRateRough") .Value = BitConverter.ToInt16(result, iOffset);
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("FlowRateFine").Value = BitConverter.ToInt16(result, iOffset); 
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("TolerancePlus").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("ToleranceMinus").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("Afterflow").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
           
                acMethod.ParameterValueList.GetACValue("AfterflowMAX").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ParameterValueList.GetACValue("LackOfMatCheckWeight").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ParameterValueList.GetACValue("LackOfMatCheckTime").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Time;
            
                acMethod.ParameterValueList.GetACValue("PulsationTime").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Time;
            
                acMethod.ParameterValueList.GetACValue("PulsationPauseTime").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Time;
            
                acMethod.ParameterValueList.GetACValue("AdjustmentTime").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Time;
            
                acMethod.ParameterValueList.GetACValue("CalmingTime").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Time;
            
                acMethod.ParameterValueList.GetACValue("StandstillTime").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Time;
            
                acMethod.ParameterValueList.GetACValue("StandstillWeight").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("Temperature").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("LastBatch").Value = BitConverter.ToInt16(result, iOffset); 
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("SprayNozzle").Value = BitConverter.ToInt16(result, iOffset); 
                iOffset += TCDataTypeLength.Int;
            
                acMethod.ParameterValueList.GetACValue("ImpulsePerLiter").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("Density").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            
                acMethod.ParameterValueList.GetACValue("MaxDosingTime").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Time;
            
                acMethod.ParameterValueList.GetACValue("AdjustmentFlowRate").Value = BitConverter.ToInt16(result, iOffset); 
                iOffset += TCDataTypeLength.Int;
           
                var acValue = acMethod.ParameterValueList.GetACValue("EndlessDosing");
                if (acValue != null)
                {
                    try { acValue.Value = BitConverter.ToBoolean(result, iOffset); }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        if (gip.core.datamodel.Database.Root != null && gip.core.datamodel.Database.Root.Messages != null
                                                                       && gip.core.datamodel.Database.Root.InitState == ACInitState.Initialized)
                            gip.core.datamodel.Database.Root.Messages.LogException("WcfAdsClientChannel", "DeinitWcfAdsClientChannel", msg);
                    }
                } 
                iOffset += TCDataTypeLength.Bool;
            }
            else
            {
                acMethod.ResultValueList.GetACValue("ActualQuantity").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ResultValueList.GetACValue("ScaleTotalWeight").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ResultValueList.GetACValue("Afterflow").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ResultValueList.GetACValue("DosingTime").Value = TimeSpan.FromMilliseconds(BitConverter.ToInt32(result, iOffset));
                iOffset += TCDataTypeLength.Time;

                acMethod.ResultValueList.GetACValue("Temperature").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ResultValueList.GetACValue("GaugeCode").Value = Encoding.ASCII.GetString(result, iOffset, TCDataTypeLength.String(81)).Replace("\0", "");
                iOffset += TCDataTypeLength.String(81);

                acMethod.ResultValueList.GetACValue("GaugeCodeStart").Value = Encoding.ASCII.GetString(result, iOffset, TCDataTypeLength.String(81)).Replace("\0", "");
                iOffset += TCDataTypeLength.String(81);

                acMethod.ResultValueList.GetACValue("GaugeCodeEnd").Value = Encoding.ASCII.GetString(result, iOffset, TCDataTypeLength.String(81)).Replace("\0", "");
                iOffset += TCDataTypeLength.String(81);

                acMethod.ResultValueList.GetACValue("FlowRate").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;

                acMethod.ResultValueList.GetACValue("ImpulseCounter").Value = BitConverter.ToDouble(result, iOffset);
                iOffset += TCDataTypeLength.LReal;
            }
            OnMethodExecuted(requestID, acMethod);
        }
    }
}
