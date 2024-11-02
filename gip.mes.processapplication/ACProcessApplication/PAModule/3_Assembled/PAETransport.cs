// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using gip.core.processapplication;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Base Class for 
    /// Basisklasse für steuerbare Bauteile/Elemente
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Baseclass Assembled Equipment'}de{'Basisklasse zusammenbau'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAETransport : PAModule, IRoutableModule
    {
        #region c'tors

        static PAETransport()
        {
            RegisterExecuteHandler(typeof(PAETransport), HandleExecuteACMethod_PAETransport);
        }

        public PAETransport(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _PAPointMatIn1 = new PAPoint(this, nameof(PAPointMatIn1));
            _PAPointMatOut1 = new PAPoint(this, nameof(PAPointMatOut1));
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            PAEEMotorBase motor = Motor;
            if (motor != null && motor.AllocatedByWay != null)
            {
                IACPropertyNetServer serverProp = motor.AllocatedByWay as IACPropertyNetServer;
                if (serverProp != null)
                    serverProp.ValueUpdatedOnReceival += MotorProp_ValueUpdatedOnReceival;
            }

            if (this.AllocatedByWay != null)
            {
                IACPropertyNetServer serverProp = this.AllocatedByWay as IACPropertyNetServer;
                if (serverProp != null)
                    serverProp.ValueUpdatedOnReceival += SelfProp_ValueUpdatedOnReceival;
            }

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (this.AllocatedByWay != null)
            {
                IACPropertyNetServer serverProp = this.AllocatedByWay as IACPropertyNetServer;
                if (serverProp != null)
                    serverProp.ValueUpdatedOnReceival -= SelfProp_ValueUpdatedOnReceival;
            }
            if (_RefMotor != null && _RefMotor.IsAttached)
            {
                PAEEMotorBase motor = Motor;
                if (motor != null)
                {
                    IACPropertyNetServer serverProp = motor.AllocatedByWay as IACPropertyNetServer;
                    if (serverProp != null)
                        serverProp.ValueUpdatedOnReceival -= MotorProp_ValueUpdatedOnReceival;
                }

                _RefMotor.Detach();
                _RefMotor = null;
            }
            if (_RefRotationControl != null)
            {
                _RefRotationControl.Detach();
                _RefRotationControl = null;
            }
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

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
        public PAPoint PAPointMatOut1
        {
            get
            {
                return _PAPointMatOut1;
            }
        }
        #endregion

        #region Optional Members
        private ACRef<PAEEMotorBase> _RefMotor = null;
        public PAEEMotorBase Motor
        {
            get
            {
                if (_RefMotor == null)
                {
                    PAEEMotorBase result = FindChildComponents<PAEEMotorBase>(c => c is PAEEMotorBase).FirstOrDefault();
                    if (result != null)
                        _RefMotor = new ACRef<PAEEMotorBase>(result, this);
                }
                if (_RefMotor != null)
                    return _RefMotor.ValueT;
                return null;
            }
        }

        private ACRef<PAERotationControl> _RefRotationControl = null;
        public PAERotationControl RotationControl
        {
            get
            {
                if (_RefRotationControl == null)
                {
                    PAERotationControl result = FindChildComponents<PAERotationControl>(c => c is PAERotationControl).FirstOrDefault();
                    if (result != null)
                        _RefRotationControl = new ACRef<PAERotationControl>(result, this);
                }
                if (_RefRotationControl != null)
                    return _RefRotationControl.ValueT;
                return null;
            }
        }

        public IEnumerable<PAEJamSensor> JamSensors
        {
            get
            {
                return FindChildComponents<PAEJamSensor>(c => c is PAEJamSensor);
            }
        }

        public IEnumerable<PAELimitSwitch> LimitSwitches
        {
            get
            {
                return FindChildComponents<PAELimitSwitch>(c => c is PAELimitSwitch);
            }
        }
        #endregion



        #region Properties, Range: 400
        #region Read-Values from PLC
        [ACPropertyBindingTarget(441, "Read from PLC", "en{'Allocated by Way'}de{'Belegt von Wegesteuerung'}", "", false, false)]
        public IACContainerTNet<BitAccessForAllocatedByWay> AllocatedByWay { get; set; }

        [ACPropertyBindingTarget(442, "Configuration", "en{'Depleting time'}de{'Leerfahrzeit'}", "", true, true)]
        public IACContainerTNet<TimeSpan> DepletingTime { get; set; }

        [ACPropertyBindingTarget(443, "Configuration", "en{'Fee performance [kg/min]'}de{'Förderleistung [kg/min]'}", "", true, true)]
        public IACContainerTNet<double> FeedPerf { get; set; }
        #endregion

        public string RouteItemID
        {
            get
            {
                PAEEMotorBase motor = Motor;
                if (motor != null)
                    return motor.RouteItemID;
                return null;
            }
        }

        public int RouteItemIDAsNum
        {
            get
            {
                PAEEMotorBase motor = Motor;
                if (motor != null)
                    return motor.RouteItemIDAsNum;
                return 0;
            }
        }
        #endregion

        #region Methods, Range: 400
        public override ACMenuItemList GetMenu(string vbContent, string vbControl)
        {
            ACMenuItemList acMenuItemList = base.GetMenu(vbContent, vbControl);
            if (Motor != null)
            {
                ACMenuItemList acMenuItemList2 = Motor.GetMenu(vbContent, vbControl);
                foreach (ACMenuItem item in acMenuItemList2)
                {
                    item.ACUrl = Motor.ACIdentifier + item.ACUrl;
                    acMenuItemList.Add(item);
                }
            }
            return acMenuItemList;
        }

        public virtual void SimulateAllocationState(RouteItem item, bool switchOff)
        {
            PAEEMotorBase motor = Motor;
            if (motor != null)
            {
                motor.SimulateAllocationState(item, switchOff);
            }
            else if (AllocatedByWay != null && AllocatedByWay.ValueT != null)
            {
                AllocatedByWay.ValueT.Bit00_Reserved = false;
                AllocatedByWay.ValueT.Bit01_Allocated = !switchOff;
            }
        }

        public void ActivateRouteItemOnSimulation(RouteItem item, bool switchOff)
        {
            PAEEMotorBase motor = Motor;
            if (motor != null)
                motor.ActivateRouteItemOnSimulation(item, switchOff);
        }

        private static object _ChangingAllocationLock = new object();
        private ushort _ChangingAllocationProp = 0;
        private void MotorProp_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.BeforeBroadcast || e == null)
                return;
            if (e.PropertyName == nameof(AllocatedByWay))
            {
                IACContainerTNet<BitAccessForAllocatedByWay> allocatedByWay = sender as IACContainerTNet<BitAccessForAllocatedByWay>;
                if (allocatedByWay != null 
                    && allocatedByWay.ValueT != null
                    && this.AllocatedByWay.ValueT != null) 
                {
                    lock (_ChangingAllocationLock)
                    {
                        if (_ChangingAllocationProp > 0)
                            return;
                        _ChangingAllocationProp = 1;
                    }
                    try
                    {
                        // Copy allocation state from the motor to this transport object
                        this.AllocatedByWay.ValueT.ValueT = allocatedByWay.ValueT.ValueT;
                    }
                    finally
                    {
                        lock (_ChangingAllocationLock)
                        {
                            _ChangingAllocationProp = 0;
                        }
                    }
                    //BitAccessForAllocatedByWay clonedValue = allocatedByWay.ValueT.Clone() as BitAccessForAllocatedByWay;
                    //if (clonedValue != null)
                }
            }
        }

        private void SelfProp_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.BeforeBroadcast || e == null)
                return;
            if (e.PropertyName == nameof(AllocatedByWay))
            {
                PAEEMotorBase motor = Motor;
                if (motor == null || motor.AllocatedByWay == null) 
                    return;
                lock (_ChangingAllocationLock)
                {
                    if (_ChangingAllocationProp > 0)
                        return;
                    _ChangingAllocationProp = 2;
                }
                try
                {
                    // Copy allocation state from this transport object to the motor
                    motor.AllocatedByWay.ValueT.ValueT = this.AllocatedByWay.ValueT.ValueT;
                }
                finally
                {
                    lock (_ChangingAllocationLock)
                    {
                        _ChangingAllocationProp = 0;
                    }
                }
            }
        }

        #endregion

        #region Handle execute helpers

        public static bool HandleExecuteACMethod_PAETransport(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAModule(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
