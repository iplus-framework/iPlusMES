using gip.core.datamodel;
using gip.core.processapplication;
using vd = gip.mes.datamodel;
using System;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scale calibratable'}de{'Waage alibi)'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAEScaleCalibratable : PAEScaleGravimetric
    {
        public PAEScaleCalibratable(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public const string ClassName = "PAEScaleCalibratable";

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


        [ACMethodInteractionClient("", "en{'Register alibi weight'}de{'Register alibi weight'}", 450, false, "", false)]
        public static void RegisterAlibiWeight(IACComponent acComponent)
        {
            if (acComponent == null || !IsEnabledRegisterAlibiWeight(acComponent))
                return;

            Msg msg = acComponent.ACUrlCommand("!OnRegisterAlibiWeight", null) as Msg;
            if(msg != null)
            {
                //TODO: alarm
                return;
            }
            acComponent.ExecuteMethod("SaveAlibiWeighing", null);
        }

        public static bool IsEnabledRegisterAlibiWeight(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;

            return true;
        }

        [ACMethodInfo("", "", 999)]
        public virtual Msg OnRegisterAlibiWeight(IACObject parentPos)
        {
            if (!IsEnabledOnRegisterAlibiWeight())
                return null;

            return null;
        }

        [ACMethodInfo("", "", 999)]
        public virtual bool IsEnabledOnRegisterAlibiWeight()
        {
            return true;
        }

        [ACMethodInfo("", "", 999)]
        public virtual Guid? SaveAlibiWeighing(IACObject parentPos)
        {
            Msg msg = null;

            if (string.IsNullOrEmpty(AlibiNo.ValueT))
            {
                msg = new Msg(this, eMsgLevel.Error, ClassName, "SaveAlibiWeighing", 74, "Error50305");
                OnNewAlarmOccurred(StateScale, msg);
                StateScale.ValueT = core.autocomponent.PANotifyState.AlarmOrFault;
                if (IsAlarmActive(StateScale, msg.Message) == null)
                    Messages.LogMessageMsg(msg);

                return null;
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
                if(msg != null)
                {
                    dbApp.ACUndoChanges();
                    OnNewAlarmOccurred(StateScale, msg);
                    StateScale.ValueT = core.autocomponent.PANotifyState.AlarmOrFault;
                    if (IsAlarmActive(StateScale, msg.Message) == null)
                        Messages.LogMessageMsg(msg);
                    return null;
                }
                return weighing.WeighingID;
            }
        }
    }
}
