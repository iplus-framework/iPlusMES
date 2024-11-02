// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;
using gip.mes.datamodel;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Beltscale'}de{'Bandwaage'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMBeltscale : PAProcessModuleVB
    {
        static PAMBeltscale()
        {
            RegisterExecuteHandler(typeof(PAMBeltscale), HandleExecuteACMethod_PAMBeltscale);
        }

        public PAMBeltscale(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

            _PAPointMatIn1 = new PAPoint(this, nameof(PAPointMatIn1));
            _PAPointMatOut1 = new PAPoint(this, nameof(PAPointMatOut1));

            #region Damir Test
            /*
            _MappingServicePoint = new ACPointServiceACObject(this, "MappingServicePoint", 1);
            _MappingServicePoint.SetMethod = OnSetMappingServicePoint;
            _MaxWeightExEvent = new ACPointEvent(this, "MaxWeightExEvent", 0);
            _MaxTempExEvent = new ACPointEvent(this, "MaxTempExEvent", 0);
             */
            #endregion
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #region Points
        PAPoint _PAPointMatIn1;
        [ACPropertyConnectionPoint(9999, "PointMaterial")]
        public PAPoint PAPointMatIn1
        {
            get
            {
                return _PAPointMatIn1;
            }
        }

        PAPoint _PAPointMatOut1;
        [ACPropertyConnectionPoint(9999, "PointMaterial")]
        [ACPointStateInfo(GlobalProcApp.AvailabilityStatePropName, core.processapplication.AvailabilityState.Idle, GlobalProcApp.AvailabilityStateGroupName, "", Global.Operators.none)]
        public PAPoint PAPointMatOut1
        {
            get
            {
                return _PAPointMatOut1;
            }
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMBeltscale(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessModuleVB(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region Damir Test
        /*
        ACPointServiceACObject _MappingServicePoint;
        [ACPropertyEventPoint(1, true)]
        public ACPointServiceACObject MappingServicePoint
        {
            get
            {
                return _MappingServicePoint;
            }
        }

        public void OnSetMappingServicePoint(IACPointNetBase point)
        {
            if (point != null)
            {
                if (point is IACPointNetService<IACObject, ACPointNetWrapObject<IACObject>>)
                {
                    IACPointNetService<IACObject, ACPointNetWrapObject<IACObject>> mappingPoint = point as IACPointNetService<IACObject, ACPointNetWrapObject<IACObject>>;
                    if (mappingPoint.ConnectionListLocal != null)
                    {
                        foreach (ACPointNetWrapObject<IACObject> item in mappingPoint.ConnectionListLocal)
                        {
                            item.ACState = Global.ACStates.Planned;
                        }
                    }
                }
            }
        }

        [ACPropertyBindingSource]
        public IACProperty<double> GrossWeight { get; set; }

        ACPointEvent _MaxWeightExEvent;
        [ACPropertyEventPoint(0,false)]
        public ACPointEvent MaxWeightExEvent
        {
            get
            {
                return _MaxWeightExEvent;
            }
            set
            {
                _MaxWeightExEvent = value;
            }
        }

        [ACPropertyBindingSource]
        public IACProperty<double> Temperature { get; set; }

        ACPointEvent _MaxTempExEvent;
        [ACPropertyEventPoint(0, false)]
        public ACPointEvent MaxTempExEvent
        {
            get
            {
                return _MaxTempExEvent;
            }
            set
            {
                _MaxTempExEvent = value;
            }
        }

        private void OnTimer()
        {
            while (true)
            {
                Thread.Sleep(5000);
                GrossWeight.Value += 1.0;
                MaxWeightExEvent.Raise(new ACValueList(new object[] { GrossWeight.Value }));
                Temperature.Value += 1.0;
                MaxTempExEvent.Raise(new ACValueList(new object[] { Temperature.Value }));
            }
        }
        */
        #endregion
    }


    #region Example

    //[ACClassInfo(Const.PackName_VarioAutomation, "en{'ExampleUseACRef'}de{'ExampleUseACRef'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class ExampleUseACRef : PAModule
    {
        public ExampleUseACRef(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        private ACRef<PAJobScheduler> _RefToCompA = null;

        // Not persistable Network property 
        [ACPropertyBindingSource(401, "Configuration", "en{'Material'}de{'Material'}", "", true, false)]
        public IACContainerTNet<ACRef<Material>> RefToMaterial { get; set; }

        // Persistable network property. 
        // Use only ACComponent as generic Type because this reference is a proxy-component on client-side.
        [ACPropertyBindingSource(402, "Configuration", "en{'RefToCompB'}de{'RefToCompB'}", "", true, true)]
        public IACContainerTNet<ACRef<ACComponent>> RefToCompB { get; set; }


        public override bool ACPostInit()
        {
            // Variant A: Pass a ACUrl for the component which you want to reference 
            _RefToCompA = new ACRef<PAJobScheduler>("\\ExampleProject\\ExampleServiceClass1", this);

            // RefToCompB is a persistable property. 
            // After restarting the iPlus-Service this ACRef ist restored automatically from the database.
            // If ValueT is null nothing was restored from the database
            if (RefToCompB.ValueT == null)
            {
                ACComponent compB = ACUrlCommand("\\ExampleProject\\ExampleServiceClass2") as ACComponent;
                // Variant B: If you already have a pointer to the component you can use this constructor. 
                RefToCompB.ValueT = new ACRef<ACComponent>(compB, this);

                // For Entity-Objects pass the Database-Context as the Parent-Object instead
                using (var dbApp = new DatabaseApp())
                {
                    Material mat = dbApp.Material.Where(c => c.MaterialNo == "ExampleNo4711").FirstOrDefault();
                    if (mat != null)
                        RefToMaterial.ValueT = new ACRef<Material>(mat, dbApp);
                }
            }

            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            Detach(false);
            return base.ACDeInit(deleteACClassTask);
        }

        public void Detach(bool removeRef)
        {
            _RefToCompA.Detach();
            RefToCompB.ValueT.Detach();
            RefToMaterial.ValueT.Detach(true);
            if (removeRef)
            {
                _RefToCompA = null;
                RefToCompB.ValueT = null;
                RefToMaterial.ValueT = null;
            }
        }

        public void Attach(bool withCheck)
        {
            if (withCheck)
            {
                if (!_RefToCompA.IsAttached)
                    _RefToCompA.Attach();
                if (!RefToCompB.ValueT.IsAttached)
                    RefToCompB.ValueT.Attach();
                if (!RefToMaterial.ValueT.IsAttached)
                {
                    // Pass a Database-Context to the attach-method
                    using (var dbApp = new DatabaseApp())
                    {
                        RefToMaterial.ValueT.AttachTo(dbApp);
                    }
                }
            }
            else
            {
                // Accessing the ValueT-Property automatically attaches the component
                var refToCompA = _RefToCompA.ValueT;
                var refToCompB = RefToCompB.ValueT.ValueT;
                // Not recommended: The Entity-Object is retreived from the first matching Database-Context in ACObjectContextManager.Contexts
                // To have control about it, use the AttachTo()-Method as you can see it in the example above
                Material material = RefToMaterial.ValueT.ValueT;
            }
        }

        public void ReplaceACRef()
        {
            // 1. Old ACRef must be detached first
            if (RefToCompB.ValueT != null)
                RefToCompB.ValueT.Detach();
            // 2. A assignment to a "Network capable" property results in a automatic broadcast.
            // All subscribed proxy-components are informed and their ACRef is replaced as well.
            RefToCompB.ValueT = new ACRef<ACComponent>("\\ExampleProject\\AnotherServiceClass1", this);

            // 1. Old ACRef must be detached first
            if (RefToMaterial.ValueT != null)
                RefToMaterial.ValueT.Detach(true);
            using (var dbApp = new DatabaseApp())
            {
                Material mat = dbApp.Material.Where(c => c.MaterialNo == "AnotherNo4711").FirstOrDefault();
                // 2. A assignment to a "Network capable" property results in a automatic broadcast.
                // All subscribed proxy-components are informed and their ACRef is replaced as well.
                if (mat != null)
                    RefToMaterial.ValueT = new ACRef<Material>(mat, dbApp);
            }
        }

        public void DoSomethingWithIt()
        {
            if (_RefToCompA != null && _RefToCompA.ValueT != null)
            {
                // ValueT is a PAJobScheduler
                _RefToCompA.ValueT.StartScheduling();
            }
            if (RefToCompB.ValueT != null && RefToCompB.ValueT.ValueT != null)
            {
                // For Proxy-Objects use ACUrlCommand
                RefToCompB.ValueT.ACUrlCommand("!StopScheduling");
                // On Serverside for real components you can cast to the concrete type and invoke the method directly
                (RefToCompB.ValueT.ValueT as PAJobScheduler).StopScheduling();
            }
        }
    }

    /// <summary>
    /// SERVER-Example
    /// </summary>
    //[ACClassInfo(Const.PackName_VarioAutomation, "en{'ExampleServiceClass'}de{'ExampleServiceClass'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class ExampleServiceClass : PAModule
    {
        #region c'tors
        static ExampleServiceClass()
        {
            // 1. Define a Dictionary for storing metadescriptions for Events
            // This class is a derivation of PAClassAlarmingBase that already have a dictionary for Events.
            // Therefore PABase.SVirtualEventArgs must be passed.
            _SVirtualEventArgs = new Dictionary<string, ACEventArgs>(PABase.SVirtualEventArgs, StringComparer.OrdinalIgnoreCase);

            // 2. Create a new instance of ACEventArgs which serves as a template when events are raised.
            ACEventArgs eventArgsMaxTemp = new ACEventArgs();
            eventArgsMaxTemp.Add(new ACValue("AlarmDate", typeof(DateTime), DateTime.MinValue, Global.ParamOption.Required));
            eventArgsMaxTemp.Add(new ACValue("Temperature", typeof(double), 0.0, Global.ParamOption.Required));

            // 3. Register this event by adding it to the Dictionary
            _SVirtualEventArgs.Add("MaxTempExEvent", eventArgsMaxTemp);


            // 1. Register a virtual method
            ACMethod.RegisterVirtualMethod(typeof(ExampleServiceClass), "ExampleMethodAsync", CreateVirtualTemperatureMethod("MixingTemperature", "en{'Mixing Temp.'}de{'Mischen Temp.'}", null));
        }

        public ExampleServiceClass(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _MaterialClientPoint = new ACPointClientObject<Material>(this, "MaterialClientPoint", 0);

            _MappingServicePoint = new ACPointServiceACObject(this, "MappingServicePoint", 1);
            _MappingServicePoint.SetMethod = OnSetMappingServicePoint;

            // 1. Create a new event-point to be able to raise network-capable events
            _MaxTempExEvent = new ACPointEvent(this, "MaxTempExEvent", 0);

            // 1. Create a new asynchronous RMI-Point to be able to handle asynchronous invocations
            _RMIPoint = new ACPointAsyncRMI(this, "RMIPoint", 1);

            // 2. Pass a delegate which is called when a new entry was added in the RMIPoint (new method-invocation for processing asynchronously)
            _RMIPoint.SetMethod = OnSetInvocationPoint;
        }
        #endregion

        #region Init
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            if (this.ApplicationManager != null)
                this.ApplicationManager.ProjectWorkCycleR10sec += CyclicCallback;
            return true;
        }

        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (this.ApplicationManager != null)
                this.ApplicationManager.ProjectWorkCycleR10sec -= CyclicCallback;
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Example From/To
        ACPointClientObject<Material> _MaterialClientPoint;
        [ACPropertyEventPoint(0, false)]
        ACPointClientObject<Material> MaterialClientPoint
        {
            get
            {
                return _MaterialClientPoint;
            }
        }

        public void FillOrderList()
        {
            using (var db = new DatabaseApp())
            {
                // Einzeln:
                foreach (Material material in db.Material)
                {
                    MaterialClientPoint.Add(material);
                }

                // Komlett:
                MaterialClientPoint.Add(db.Material);
            }
        }
        #endregion

        #region Example Mapping
        ACPointServiceACObject _MappingServicePoint;
        // Service-Point with a maximum capacity of 1
        [ACPropertyPoint(true, 1)]
        public ACPointServiceACObject MappingServicePoint
        {
            get
            {
                return _MappingServicePoint;
            }
        }

        public bool OnSetMappingServicePoint(IACPointNetBase point)
        {
            if (point != null)
            {
                if (point is IACPointNetService<ACComponent, ACPointNetWrapObject<ACComponent>>)
                {
                    IACPointNetService<ACComponent, ACPointNetWrapObject<ACComponent>> mappingPoint = point as IACPointNetService<ACComponent, ACPointNetWrapObject<ACComponent>>;
                    if (mappingPoint.ConnectionListLocal != null)
                    {
                        foreach (ACPointNetWrapObject<ACComponent> item in mappingPoint.ConnectionListLocal)
                        {
                            item.State = PointProcessingState.Accepted;
                        }
                    }
                }
            }
            return true;
        }
        #endregion

        #region Example Event
        private static Dictionary<string, ACEventArgs> _SVirtualEventArgs;
        public static new Dictionary<string, ACEventArgs> SVirtualEventArgs
        {
            get { return _SVirtualEventArgs; }
        }

        public override Dictionary<string, ACEventArgs> VirtualEventArgs
        {
            get { return SVirtualEventArgs; }
        }

        ACPointEvent _MaxTempExEvent;
        [ACPropertyEventPoint(0, false)]
        public ACPointEvent MaxTempExEvent
        {
            get
            {
                return _MaxTempExEvent;
            }
        }

        private void CyclicCallback(object sender, EventArgs e)
        {
            // 1. Get a new ACEventArgs-instance for the event "MaxTempExEvent"
            ACEventArgs eventArgs = ACEventArgs.GetVirtualEventArgs("MaxTempExEvent", SVirtualEventArgs);

            // 2. Fill out parameters
            eventArgs.GetACValue("AlarmDate").Value = DateTime.Now;
            eventArgs.GetACValue("Temperature").Value = (double)25.0;

            // 3. Raise the event an inform all subscribers (also over network)
            MaxTempExEvent.Raise(eventArgs);
        }
        #endregion

        #region Example Async-Method-Call
        ACPointAsyncRMI _RMIPoint;
        [ACPropertyAsyncMethodPoint(9999)]
        public ACPointAsyncRMI RMIPoint
        {
            get
            {
                return _RMIPoint;
            }
        }

        public bool OnSetInvocationPoint(IACPointNetBase point)
        {
            var query = ReferencePoint.ConnectionList.Where(c => c is IACContainerRef);
            // This delegate in invioked when the RMI-Point has got a new entry

            // VARIANT A:
            // DeQueueInvocationList() handles all new entries by calling ExampleMethodAsync() for each new entry
            RMIPoint.DeQueueInvocationList();

            // VARIANT B:
            // If you want to handle it on another way, then implement you own logic. 
            foreach (var newEntry in RMIPoint.ConnectionList.Where(c => c.State == PointProcessingState.NewEntry))
            {
                // Call ActivateAsyncRMI if you want to handle this entry. (ActivateAsyncRMI knows that your ExampleMethodAsync has to be invoked)
                RMIPoint.ActivateAsyncRMI(newEntry, true);
                // Attention: If you don't invoke all new entries, than you have to handle the other remaining entries in another cyclic thread
                // otherwise the requester will never get back a result!
            }

            return true;
        }

        [ACMethodAsync("Send", "en{'Example asynchronous method'}de{'Beispiel asynchrone Methode'}", 201, false)]
        public ACMethodEventArgs ExampleMethodAsync(ACMethod acMethod)
        {
            // 1. Optional validation. If parameters are not valid, then return with Global.ACMethodResultState.Failed
            // The invoker will get a callback and will be informed, that this asynchronous invocation is not possible.
            if (!IsEnabledExampleMethodAsync(acMethod))
                return new ACMethodEventArgs(acMethod, Global.ACMethodResultState.Failed);

            ACPointAsyncRMIWrap<ACComponent> currentAsyncRMI = RMIPoint.CurrentAsyncRMI;

            // 2.a) Since .net framework 4.5 you can use System.Threading.Tasks
            Task.Run(() => { AsyncWork(currentAsyncRMI, acMethod); });

            // 2. b) Otherwise use the classic ThreadPool instead:
            //ThreadPool.QueueUserWorkItem((object state) => { AsyncWork(currentAsyncRMI); } );

            // 3. Inform the invoker that the asynchronous invocation was accepted an is in processing
            return new ACMethodEventArgs(acMethod, Global.ACMethodResultState.InProcess);
        }

        public bool IsEnabledExampleMethodAsync(ACMethod acMethod)
        {
            ACValue param = acMethod.ParameterValueList.GetACValue("Temperature");
            if (param == null || param.ParamAsDouble <= 0.0)
                return false;
            return true;
        }

        private void AsyncWork(ACPointAsyncRMIWrap<ACComponent> currentAsyncRMI, ACMethod acMethod)
        {
            // 1. Run logic          

            if (currentAsyncRMI != null && !currentAsyncRMI.CallbackIsPending)
            {
                // 2. Fill out the result parameters
                ACMethodEventArgs result = new ACMethodEventArgs(acMethod, Global.ACMethodResultState.Succeeded);
                result.GetACValue("Temperature").Value = (double)25.0;

                // 3. Invoke callback method of the invoker. 
                // If client has requested the asynchronous invocation was via network the callback will be done on the remote side at the client
                RMIPoint.InvokeCallbackDelegate(result);
            }
        }

        protected static ACMethodWrapper CreateVirtualTemperatureMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("Temperature", typeof(Double), 0.0, Global.ParamOption.Required));
            paramTranslation.Add("Temperature", "en{'Temperature'}de{'Temperatur'}");
            method.ParameterValueList.Add(new ACValue("Speed", typeof(Int16), 0, Global.ParamOption.Optional));
            paramTranslation.Add("Speed", "en{'Speed'}de{'Geschwindigkeit'}");
            method.ParameterValueList.Add(new ACValue("MinWeight", typeof(Double), 0, Global.ParamOption.Optional));
            paramTranslation.Add("MinWeight", "en{'Minimum weight'}de{'Mindestgewicht'}");

            // 1. Add a Translation-Table for the result-parameters:
            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();

            // 2. Declare a new Result-Parameter "Temperature"
            method.ResultValueList.Add(new ACValue("ActTemperature", typeof(Double), 0.0, Global.ParamOption.Required));

            // 3. Add a description for the Result-Parameter
            resultTranslation.Add("ActTemperature", "en{'Temperature'}de{'Temperatur'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }
        #endregion
    }


    /// <summary>
    /// CONSUMER-Example
    /// </summary>
    //[ACClassInfo(Const.PackName_VarioAutomation, "en{'ExampleConsumerClass'}de{'ExampleConsumerClass'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class ExampleConsumerClass : PAModule
    {
        #region c'tors
        public ExampleConsumerClass(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            // 1. Create a Client-Point
            // If SynchronousMode is set to true, new Entries at the Service-Point are done synchronous
            // This means, that the calling thread is blocked till the response of the server has taken place
            _MappingClientPoint = new ACPointClientACObject(this, "MappingClientPoint", 1) { SynchronousMode = true };

            // 1. Create a Subscription point to be able to subscribe events at server-components
            _EventSubscr = new ACPointEventSubscr(this, "EventSubscr", 0);

            // 1. Create a Subscription point to be able to invoke remote asynchronous methods
            _RMISubscr = new ACPointAsyncRMISubscr(this, "RMISubscr", 1);
        }
        #endregion

        #region Init
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACPostInit()
        {
            // 1. Create a Reference to a ACComponent that provides service-points
            _ACServiceObject = new ACRef<ACComponent>("\\ExampleProject\\ExampleServiceClass1", this);

            // Abonniere alle Points
            // Methode sollte nur dann aufgerufen werden, wenn wirklich alle Points auch benötigt werden
            if (_ACServiceObject.IsObjLoaded)
                (_ACServiceObject.ValueT as ACComponent).SubscribeAllNetPoints();

            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_ACServiceObject.IsObjLoaded)
            {
                // Objekt freigeben:
                _ACServiceObject.Detach();
            }
            return base.ACDeInit(deleteACClassTask);
        }

        // Reference to a ACComponent that provides service-points 
        private ACRef<ACComponent> _ACServiceObject;

        #endregion

        #region Execute-Helper
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "EventCallback":
                    EventCallback(acParameter[0] as IACPointNetBase, acParameter[1] as ACEventArgs, acParameter[2] as IACObject);
                    return true;
                case "RMICallback":
                    RMICallback(acParameter[0] as IACPointNetBase, acParameter[1] as ACEventArgs, acParameter[2] as IACObject);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region Example From/To
        public void InitQueryOrder()
        {
            if (_ACServiceObject.IsObjLoaded)
            {
                IACPointNetObject<Material> iPoint = _ACServiceObject.ValueT.GetPointNet("MaterialClientPoint") as IACPointNetObject<Material>;
                if (iPoint != null)
                    iPoint.Subscribe();
            }
        }

        public void QueryOrder()
        {
            if (_ACServiceObject.IsObjLoaded)
            {
                IACPointNetObject<Material> iPoint = _ACServiceObject.ValueT.GetPointNet("MaterialClientPoint") as IACPointNetObject<Material>;
                if (iPoint != null)
                {
                    foreach (Material order in iPoint.RefObjectList)
                    {
                    }
                }
            }
        }
        #endregion

        #region Example Mapping
        ACPointClientACObject _MappingClientPoint;
        // Client-Point with a maximum capacity of 1
        [ACPropertyPoint(true, 1)]
        public ACPointClientACObject MappingClientPoint
        {
            get
            {
                return _MappingClientPoint;
            }
        }

        public void MappingExample(bool map)
        {
            // Checks if Service-Component is available
            if (!_ACServiceObject.IsObjLoaded)
                return;

            if (map)
            {
                // 1. Optional: Check Reference already added to service-point
                if (MappingClientPoint.ConnectionList.Where(c => c.ValueT == _ACServiceObject.ValueT).Any())
                    return;

                // 2. Mapping / Registering: Adds a reference to the remote service-point 
                ACPointNetWrapObject<ACComponent> referenceEntry = MappingClientPoint.AddToServicePoint(_ACServiceObject.ValueT, "MappingServicePoint");

                // 3. Optional: Validation if reference was added on server side.
                if (referenceEntry == null
                    || referenceEntry.State == PointProcessingState.Rejected)
                {
                    // Error
                }
            }
            else
            {
                /// Unmapping / Unregistering: Removes a reference to the remote service-point 
                MappingClientPoint.RemoveFromServicePoint(_ACServiceObject.ValueT, "MappingServicePoint");
            }
        }
        #endregion

        #region Example Event
        ACPointEventSubscr _EventSubscr;
        [ACPropertyEventPointSubscr(9999, false)]
        public ACPointEventSubscr EventSubscr
        {
            get
            {
                return _EventSubscr;
            }
        }

        [ACMethodInfo("Function", "en{'EventCallback'}de{'EventCallback'}", 9999)]
        public void EventCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            if (e != null)
            {
                // Read event data
                if (sender.ACIdentifier == "MaxTempExEvent")
                {
                    DateTime stamp = e.GetACValue("AlarmDate").ParamAsDateTime;
                    double temperature = (double)e["Temperature"];
                }
                ACPointEventWrap<ACComponent> eventEntry = wrapObject as ACPointEventWrap<ACComponent>;
                System.Diagnostics.Trace.WriteLine(eventEntry.State.ToString());
            }
        }

        private bool SubscribeEventsExample(bool subscribe, bool allEvents, bool withSubscriptionPoint)
        {
            if (!_ACServiceObject.IsObjLoaded)
                return false;

            // 1. With local subscription point for receiving events
            if (withSubscriptionPoint)
            {
                if (allEvents)
                {
                    // 1.a. Subscribe to all events that the service-component provides
                    if (subscribe)
                        return EventSubscr.SubscribeAllEvents(_ACServiceObject.ValueT, EventCallback);
                    // 1.b. Unsubscribe all events
                    else
                        return EventSubscr.UnSubscribeAllEvents(_ACServiceObject.ValueT);
                }
                else
                {
                    // 1.b. Subscribe to "MaxTempExEvent" that the service-component provides
                    if (subscribe)
                    {
                        ACPointEventSubscrWrap<ACComponent> subscriptionEntry = EventSubscr.SubscribeEvent(_ACServiceObject.ValueT, "MaxTempExEvent", EventCallback);
                        return subscriptionEntry != null;
                    }
                    // 1.c. Unsubscribe "MaxTempExEvent"
                    else
                        return EventSubscr.UnSubscribeEvent(_ACServiceObject.ValueT, "MaxTempExEvent");
                }
            }
            // 2. Without local subscription point
            else
            {
                if (allEvents)
                {
                    // Query service-component for all available events
                    IEnumerable<IACPointEvent> EventList = _ACServiceObject.ValueT.Events;
                    if (EventList != null)
                    {
                        foreach (IACPointEvent Event in EventList)
                        {
                            // 2.a. Subscribe
                            if (subscribe)
                            {
                                ACPointEventWrap<ACComponent> subscriptionEntry = Event.SubscribeEvent(EventCallback);
                                if (subscriptionEntry == null)
                                    return false;
                            }
                            // 2.b. Unsubscribe
                            else
                            {
                                if (!Event.UnSubscribeEvent(EventCallback))
                                    return false;
                            }
                        }
                    }
                }
                else
                {
                    // Query service-component for a special event
                    IACPointEvent iEvent = _ACServiceObject.ValueT.GetPointNet("MaxTempExEvent") as IACPointEvent;
                    if (iEvent != null)
                    {
                        // 2.c. Subscribe
                        if (subscribe)
                        {
                            ACPointEventWrap<ACComponent> subscriptionEntry = iEvent.SubscribeEvent(EventCallback);
                            return subscriptionEntry != null;
                        }
                        // 2.d. Unsubscribe
                        else
                        {
                            return iEvent.UnSubscribeEvent(EventCallback);
                        }
                    }
                }

            }
            return true;
        }
        #endregion

        #region Example Async-Method-Call
        ACPointAsyncRMISubscr _RMISubscr;
        [ACPropertyAsyncMethodPointSubscr(9999, false, 0, "RMICallback")]
        public ACPointAsyncRMISubscr RMISubscr
        {
            get
            {
                return _RMISubscr;
            }
        }

        [ACMethodInfo("Function", "en{'RMICallback'}de{'RMICallback'}", 9999)]
        public void RMICallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            // The callback-method can be called
            if (e != null)
            {
                IACTask taskEntry = wrapObject as IACTask;
                ACPointAsyncRMIWrap<ACComponent> taskEntryMoreConcrete = wrapObject as ACPointAsyncRMIWrap<ACComponent>;
                ACMethodEventArgs eM = e as ACMethodEventArgs;
                if (taskEntry.State == PointProcessingState.Deleted)
                {
                    // Compare RequestID to identify your asynchronus invocation
                    if (taskEntry.RequestID == myTestRequestID)
                    {
                        // Completed
                    }
                }
                if (taskEntryMoreConcrete.Result.ResultState == Global.ACMethodResultState.Succeeded)
                {
                    bool wasMyAsynchronousRequest = false;
                    // Don't compare references, because ACPointAsyncRMIWrap are entries which are serialized and deserialized through network.
                    // Use CompareTo-Method instead!
                    if (myRequestEntryB != null && myRequestEntryB.CompareTo(taskEntryMoreConcrete) == 0)
                        wasMyAsynchronousRequest = true;
                    // CompareTo also works with ACPointAsyncRMISubscrWrap and ACPointAsyncRMIWrap
                    // ACPointAsyncRMISubscrWrap-Entries are created ift invocation is done with a subscription point 
                    if (myRequestEntryA != null && myRequestEntryA.CompareTo(taskEntryMoreConcrete) == 0)
                        wasMyAsynchronousRequest = true;
                    System.Diagnostics.Trace.WriteLine(wasMyAsynchronousRequest.ToString());
                }
            }
        }

        Guid myTestRequestID;
        ACPointAsyncRMISubscrWrap<ACComponent> myRequestEntryA;
        ACPointAsyncRMIWrap<ACComponent> myRequestEntryB;
        public bool InvokeVirtualMethodAsync(bool withSubscriptionPoint)
        {
            // 1. Invoke ACUrlACTypeSignature for getting a default-ACMethod-Instance
            ACMethod acMethod = _ACServiceObject.ValueT.ACUrlACTypeSignature("!MixingTemperature", gip.core.datamodel.Database.GlobalDatabase);

            // 2. Fill out all important parameters
            acMethod.ParameterValueList.GetACValue("Temperature").Value = (double)25.0;

            // Starte Methode Asynchron und rufe "RMICallback"-Methode zurück wenn fertig
            // ENTWEDER: Mit lokalem Client-Point:

            // VARIANT A)
            // Invoke the method asynchronous by calling the InvokeAsyncMethod on the subscription-point passing the RMICallback-delegate.
            // If _ACServiceObject.ValueT is a proxy, then this invocation is sended over network.
            // When the execution was completed, the RMICallback-delegate from another thread.
            if (withSubscriptionPoint)
            {
                myRequestEntryA = RMISubscr.InvokeAsyncMethod(_ACServiceObject.ValueT, "RMIPoint", acMethod, RMICallback);
                if (myRequestEntryA != null)
                    myTestRequestID = myRequestEntryA.RequestID;
                return myRequestEntryA != null;
            }
            // ALTERNATIVE VARIANT B)
            // Invocation wothout a subscription-point.
            // If _ACServiceObject.ValueT is a proxy, then this invocation is sended over network.
            // When the execution was completed, the RMICallback-delegate from another thread.
            else
            {
                IACPointAsyncRMI iAsync = _ACServiceObject.ValueT.GetPointNet("RMIPoint") as IACPointAsyncRMI;
                if (iAsync != null)
                {
                    myRequestEntryB = iAsync.InvokeAsyncMethod(RMICallback, acMethod);
                    if (myRequestEntryB != null)
                        myTestRequestID = myRequestEntryB.RequestID;
                    return myRequestEntryB != null;
                }
            }

            // We recommend to use VARIANT A). The Advantage of using a RMISubscr is, that you can see in the Connection-List of the subscription-point 
            //  1. How many invocations are pending
            //  2. The state of the invocations
            return false;
        }

        public void InvokeVirtualMethodSync()
        {
            // 1. Get a reference to the instance
            ACComponent serviceComp = ACUrlCommand("\\ExampleProject\\ExampleServiceClass1") as ACComponent;

            // 2. Optional: If instance lives on server-side, then check the Connection-State for e.g. informing the user that the service is currently not available
            if (serviceComp == null || serviceComp.ConnectionState != ACObjectConnectionState.Connected)
                return;

            // 3. Invoke ACUrlACTypeSignature for getting a default-ACMethod-Instance
            ACMethod acMethod = serviceComp.ACUrlACTypeSignature("!MixingTemperature", gip.core.datamodel.Database.GlobalDatabase);

            // 4. Fill out all important parameters
            acMethod.ParameterValueList.GetACValue("Temperature").Value = (double)25.0;

            // 5. Invoke the method synchronous. 
            // If serviceComp is a proxy, then this invocation is sended over network. 
            // The invoking thread is blocked till the asnychronus exceution has completed. 
            // If serviceComp is a proxy, afterwards the result is send back over the network.
            ACMethodEventArgs result = serviceComp.ACUrlCommand("!" + acMethod.ACIdentifier, acMethod) as ACMethodEventArgs;

            // 6. Result can be null if serviceComp is a proxy an a network error has occured.
            if (result != null)
                System.Diagnostics.Trace.WriteLine(result.GetACValue("Temperature").ParamAsDouble.ToString());
        }
        #endregion
    }

    #endregion
}
