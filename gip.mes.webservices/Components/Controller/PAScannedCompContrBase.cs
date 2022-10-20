using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.processapplication;
using gip.mes.datamodel;

namespace gip.mes.webservices
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Controller for scanned component'}de{'Steuerklasse für gescannte Komponente'}", Global.ACKinds.TACAbstractClass, Global.ACStorableTypes.Required, false, false)]
    public abstract class PAScannedCompContrBase : PAClassAlarmingBase
    {
        #region c'tors
        public const string ClassName = "PAScannedCompContrBase";

        public PAScannedCompContrBase(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion


        #region Properties
        Type _ControlledType;
        protected Type ControlledType
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_ControlledType == null)
                        _ControlledType = OnGetControlledType();
                }
                return _ControlledType;
            }
        }

        protected abstract Type OnGetControlledType();

        public PAEScannerDecoderWS ParentDecoder
        {
            get
            {
                return ParentACComponent as PAEScannerDecoderWS;
            }
        }

        Type _TypeOfPAProcessFunction;
        protected Type TypeOfPAProcessFunction
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_TypeOfPAProcessFunction == null)
                        _TypeOfPAProcessFunction = typeof(PAProcessFunction);
                }
                return _TypeOfPAProcessFunction;
            }
        }
        #endregion 


        #region Methods
        public virtual bool IsControllerFor(ACComponent component)
        {
            return component.ComponentClass.IsDerivedOrEqual(ControlledType);
        }

        public virtual bool CanHandleBarcodeSequence(ACComponent component, BarcodeSequence sequence)
        {
            bool isPAF = component.ComponentClass.IsDerivedOrEqual(TypeOfPAProcessFunction);
            if (!isPAF)
                return true;
            ACStateEnum? acState = GetFunctionState(component, sequence);
            if (!acState.HasValue)
                return false;
            if (acState < ACStateEnum.SMStarting || acState >= ACStateEnum.SMCompleted)
            {
                // Info50042: Function is not active.
                sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "CanHandleBarcodeSequence", 10, "Info50042");
                sequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                return false;
            }
            return true;
        }
        public abstract void HandleBarcodeSequence(ACComponent component, BarcodeSequence sequence);

        protected ACStateEnum? GetFunctionState(ACComponent processFunction, BarcodeSequence sequence)
        {
            IACContainerTNet<ACStateEnum> acState = processFunction.GetPropertyNet(Const.ACState) as IACContainerTNet<ACStateEnum>;
            if (acState == null)
            {
                // Error50353: ACState-Property doesn't exist.
                sequence.Message = new Msg(this, eMsgLevel.Error, ClassName, "GetFunctionState", 10, "Error50353");
                sequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                return null;
            }
            return acState.ValueT;
        }
        #endregion
    }

}
