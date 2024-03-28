using gip.core.datamodel;
using gip.core.processapplication;
using VD = gip.mes.datamodel;
using System;
using gip.core.autocomponent;
using gip.mes.facility;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scale calibratable'}de{'Waage alibi)'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAEScaleCalibratableMES : PAEScaleCalibratable
    {
        static PAEScaleCalibratableMES()
        {
            RegisterExecuteHandler(typeof(PAEScaleCalibratableMES), HandleExecuteACMethod_PAEScaleCalibratableMES);
        }

        public PAEScaleCalibratableMES(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #region Handle execute helpers
        public static bool HandleExecuteACMethod_PAEScaleCalibratableMES(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAEScaleCalibratable(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            //switch (acMethodName)
            //{
            //}
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        public override void SimulateAlibi()
        {
            SimulateAlibiStatic(this);
        }

        public static void SimulateAlibiStatic(PAEScaleCalibratable _this)
        {
            double maxWeight = _this.MaxScaleWeight.ValueT;
            if (FacilityConst.IsDoubleZeroForPosting(maxWeight))
            {
                IPAMCont container = _this.FindParentComponent<IPAMCont>(c => c is IPAMCont);
                if (container != null)
                    maxWeight = container.MaxVolumeCapacity.ValueT;
            }
            if (FacilityConst.IsDoubleZeroForPosting(maxWeight))
                maxWeight = 1000;
            Random random = new Random();
            double weight = random.NextDouble() * maxWeight;
            if (_this.ActualValue.ValueT >= 0.0001)
                weight = _this.ActualValue.ValueT;
            _this.AlibiWeight.ValueT = weight;
            _this.AlibiNo.ValueT = DateTime.Now.ToString();
        }


        public override Msg SaveAlibiWeighing(PAOrderInfoEntry entity = null)
        {
            return SaveAlibiWeighingStatic(this, entity);
        }

        public static Msg SaveAlibiWeighingStatic(PAEScaleCalibratable _this, PAOrderInfoEntry entity = null)
        {
            Msg msg = null;

            if (string.IsNullOrEmpty(_this.AlibiNo.ValueT))
            {
                msg = new Msg(_this, eMsgLevel.Error, nameof(PAEScaleCalibratableMES), "SaveAlibiWeighing", 74, "Error50305");
                _this.OnNewAlarmOccurred(_this.StateScale, msg);
                _this.StateScale.ValueT = core.autocomponent.PANotifyState.AlarmOrFault;
                if (_this.IsAlarmActive(_this.StateScale, msg.Message) == null)
                    _this.Messages.LogMessageMsg(msg);
                return msg;
            }


            using (Database db = new Database())
            using (VD.DatabaseApp dbApp = new VD.DatabaseApp())
            {
                string secondaryKey = _this.Root.NoManager.GetNewNo(db, typeof(VD.Weighing), VD.Weighing.NoColumnName, VD.Weighing.FormatNewNo, _this);
                VD.Weighing weighing = VD.Weighing.NewACObject(dbApp, null, secondaryKey);
                weighing.Weight = _this.AlibiWeight.ValueT;
                weighing.IdentNr = _this.AlibiNo.ValueT;

                if (entity != null)
                {
                    switch (entity.EntityName) 
                    {
                        case nameof(VD.VisitorVoucher):
                            weighing.VisitorVoucherID = entity.EntityID;
                            break;
                        case nameof(VD.PickingPos):
                            weighing.PickingPosID = entity.EntityID;
                            break;
                        case nameof(VD.LabOrderPos):
                            weighing.LabOrderPosID = entity.EntityID;
                            break;
                        case nameof(VD.OutOrderPos):
                            weighing.OutOrderPosID = entity.EntityID;
                            break;
                        case nameof(VD.InOrderPos):
                            weighing.InOrderPosID = entity.EntityID;
                            break;
                    }
                }

                dbApp.Weighing.Add(weighing);

                msg = dbApp.ACSaveChanges();
                if (msg != null)
                {
                    dbApp.ACUndoChanges();
                    _this.OnNewAlarmOccurred(_this.StateScale, msg);
                    _this.StateScale.ValueT = core.autocomponent.PANotifyState.AlarmOrFault;
                    if (_this.IsAlarmActive(_this.StateScale, msg.Message) == null)
                        _this.Messages.LogMessageMsg(msg);
                    return msg;
                }
                msg = new Msg(eMsgLevel.Default, "");
                msg.MsgId = weighing.WeighingID;
            }
            return msg;

        }
    }
}
