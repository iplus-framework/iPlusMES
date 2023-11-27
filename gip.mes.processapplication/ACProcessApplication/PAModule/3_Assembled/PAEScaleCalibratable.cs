using gip.core.datamodel;
using gip.core.processapplication;
using vd = gip.mes.datamodel;
using System;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scale calibratable'}de{'Waage alibi)'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAEScaleCalibratable : PAEScaleGravimetric
    {
        static PAEScaleCalibratable()
        {
            RegisterExecuteHandler(typeof(PAEScaleCalibratable), HandleExecuteACMethod_PAEScaleCalibratable);
        }

        public PAEScaleCalibratable(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public const string ClassName = "PAEScaleCalibratable";

        #region Handle execute helpers
        public static bool HandleExecuteACMethod_PAEScaleCalibratable(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAEScaleGravimetric(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "RegisterAlibiWeight":
                    result = RegisterAlibiWeight();
                    return true;
                case Const.IsEnabledPrefix + "RegisterAlibiWeight":
                    result = IsEnabledRegisterAlibiWeight();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        [ACPropertyBindingTarget(810, "Read from PLC", "en{'Alibi weight [kg]'}de{'Alibigewicht [kg]'}", "", false, false, RemotePropID = 86)]
        public IACContainerTNet<double> AlibiWeight
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(820, "Read from PLC", "en{'Alibi No'}de{'Alibi Nr'}", "", false, false, RemotePropID = 87)]
        public IACContainerTNet<string> AlibiNo
        {
            get;
            set;
        }


        [ACMethodInteraction("", "en{'Register alibi weight'}de{'Registriere Gewicht'}", 450, true)]
        public Msg RegisterAlibiWeight()
        {
            Msg msg = OnRegisterAlibiWeight(null);
            if (msg != null)
                return msg;
            return SaveAlibiWeighing(null);
        }

        public bool IsEnabledRegisterAlibiWeight()
        {
            return true;
        }

        public virtual Msg OnRegisterAlibiWeight(IACObject parentPos)
        {
            if (!IsEnabledOnRegisterAlibiWeight())
                return null;

            return null;
        }

        public virtual bool IsEnabledOnRegisterAlibiWeight()
        {
            return true;
        }

        public virtual Msg SaveAlibiWeighing(IACObject parentPos)
        {
            Msg msg = null;

            if (string.IsNullOrEmpty(AlibiNo.ValueT))
            {
                msg = new Msg(this, eMsgLevel.Error, ClassName, "SaveAlibiWeighing", 74, "Error50305");
                OnNewAlarmOccurred(StateScale, msg);
                StateScale.ValueT = core.autocomponent.PANotifyState.AlarmOrFault;
                if (IsAlarmActive(StateScale, msg.Message) == null)
                    Messages.LogMessageMsg(msg);
                return msg;
            }

            using (Database db = new Database())
            using (vd.DatabaseApp dbApp = new vd.DatabaseApp())
            {
                string secondaryKey = Root.NoManager.GetNewNo(db, typeof(vd.Weighing), vd.Weighing.NoColumnName, vd.Weighing.FormatNewNo, this);
                vd.Weighing weighing = vd.Weighing.NewACObject(dbApp, parentPos, secondaryKey);
                weighing.Weight = AlibiWeight.ValueT;
                weighing.IdentNr = AlibiNo.ValueT;
                dbApp.Weighing.AddObject(weighing);

                msg = dbApp.ACSaveChanges();
                if (msg != null)
                {
                    dbApp.ACUndoChanges();
                    OnNewAlarmOccurred(StateScale, msg);
                    StateScale.ValueT = core.autocomponent.PANotifyState.AlarmOrFault;
                    if (IsAlarmActive(StateScale, msg.Message) == null)
                        Messages.LogMessageMsg(msg);
                    return msg;
                }
                msg = new Msg(eMsgLevel.Default, "");
                msg.MsgId = weighing.WeighingID;
            }

            return msg;
        }
    }
}
