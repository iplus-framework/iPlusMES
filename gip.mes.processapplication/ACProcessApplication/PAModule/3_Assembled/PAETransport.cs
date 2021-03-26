using System;
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
    public class PAETransport : PAModule
    {
        #region c'tors

        static PAETransport()
        {
            RegisterExecuteHandler(typeof(PAETransport), HandleExecuteACMethod_PAETransport);
        }

        public PAETransport(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _PAPointMatIn1 = new PAPoint(this, Const.PAPointMatIn1);
            _PAPointMatOut1 = new PAPoint(this, Const.PAPointMatOut1);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_RefMotor != null)
            {
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
                    PAEEMotorBase result = FindMemberACComponent(typeof(PAEEMotorBase)) as PAEEMotorBase;
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
                    PAERotationControl result = FindMemberACComponent(typeof(PAERotationControl)) as PAERotationControl;
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
                List<PAEJamSensor> listResult = new List<PAEJamSensor>();
                if (this.ACComponentChilds.Count() <= 0)
                    return listResult;
                var query = this.ACComponentChilds.Where(c => typeof(PAEJamSensor).IsAssignableFrom(c.GetType())).Select(c => c as PAEJamSensor);
                if (!query.Any())
                    return listResult;
                foreach (PAEJamSensor member in query)
                {
                    listResult.Add(member);
                }
                return listResult;
            }
        }
        #endregion



        #region Properties, Range: 400
        #region Read-Values from PLC
        [ACPropertyBindingTarget(441, "Read from PLC", "en{'Allocated by Way'}de{'Belegt von Wegesteuerung'}", "", false, false)]
        public IACContainerTNet<BitAccessForAllocatedByWay> AllocatedByWay { get; set; }

        [ACPropertyBindingTarget(442, "Configuration", "en{'Depleting time'}de{'Leerfahrzeit'}", "", false, false)]
        public IACContainerTNet<TimeSpan> DepletingTime { get; set; }
        #endregion

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
        #endregion

        #region Handle execute helpers

        public static bool HandleExecuteACMethod_PAETransport(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAModule(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
