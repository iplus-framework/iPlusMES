using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.processapplication;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.webservices
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scan-Controller for PAESamplePiLightBoxSC'}de{'Scan-Controller für PAESamplePiLightBoxSC'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PAESamplePiLightBoxSC : PAScannedCompContrBase
    {
        public PAESamplePiLightBoxSC(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        private string _WeighingSequenceString;
        protected string WeighingSequenceString
        {
            get
            {
                if (_WeighingSequenceString == null)
                    _WeighingSequenceString = "weighing sequence";

                return _WeighingSequenceString;
            }
        }


        public override void HandleBarcodeSequence(ACComponent component, BarcodeSequence sequence)
        {
            if (sequence == null || component == null)
                return;

            PAESamplePiLightBox lightBox = component as PAESamplePiLightBox;
            if (lightBox == null)
                sequence.Message = new Msg(eMsgLevel.Error, "The scanned code is not from Sample Pi Light box!");

            if (lightBox.IsRecording.ValueT > 0)
            {
                BarcodeEntity commandEntity = sequence.Sequence.FirstOrDefault(c => c.Command != null && c.Command.ACMethodInvoked);
                if (commandEntity != null)
                {
                    if (commandEntity.Command.ACMethodName == nameof(ResetWeighingCycle))
                    {
                        ResetWeighingCycle(component);
                        sequence.Sequence.Remove(commandEntity);
                        sequence.Message = new Msg(eMsgLevel.Info, "The weighing sequence restart is invoked!");
                    }
                }

                BarcodeEntity barcodeEntity;

                ACMethod wfMethod;
                double setPoint = lightBox.SetPoint.ValueT;
                if (setPoint > double.Epsilon)
                {
                    wfMethod = new ACMethod("LightBoxParam");
                    wfMethod.ParameterValueList.Add(new ACValue(string.Format("1. {0}", WeighingSequenceString), string.Format("T: {0} kg  Tol +: {1} kg  Tol -: {2} kg", setPoint, lightBox.TolPlus.ValueT, lightBox.TolMinus.ValueT)));
                    barcodeEntity = new BarcodeEntity();
                    barcodeEntity.WFMethod = wfMethod;
                    sequence.AddSequence(barcodeEntity);
                }

                double setPoint2 = lightBox.SetPoint2.ValueT;
                if (setPoint2 > double.Epsilon)
                {
                    wfMethod = new ACMethod("LightBoxParam");
                    wfMethod.ParameterValueList.Add(new ACValue(string.Format("2. {0}", WeighingSequenceString), string.Format("T: {0} kg  Tol +: {1} kg  Tol -: {2} kg", setPoint2, lightBox.TolPlus2.ValueT, lightBox.TolMinus2.ValueT)));
                    barcodeEntity = new BarcodeEntity();
                    barcodeEntity.WFMethod = wfMethod;
                    sequence.AddSequence(barcodeEntity);
                }

                double setPoint3 = lightBox.SetPoint3.ValueT;
                if (setPoint3 > double.Epsilon)
                {
                    wfMethod = new ACMethod("LightBoxParam");
                    wfMethod.ParameterValueList.Add(new ACValue(string.Format("3. {0}", WeighingSequenceString), string.Format("T: {0} kg  Tol +: {1} kg  Tol -: {2} kg", setPoint3, lightBox.TolPlus3.ValueT, lightBox.TolMinus3.ValueT)));
                    barcodeEntity = new BarcodeEntity();
                    barcodeEntity.WFMethod = wfMethod;
                    sequence.AddSequence(barcodeEntity);
                }

                double setPoint4 = lightBox.SetPoint4.ValueT;
                if (setPoint4 > double.Epsilon)
                {
                    wfMethod = new ACMethod("LightBoxParam");
                    wfMethod.ParameterValueList.Add(new ACValue(string.Format("4. {0}", WeighingSequenceString), string.Format("T: {0} kg  Tol +: {1} kg  Tol -: {2} kg", setPoint4, lightBox.TolPlus4.ValueT, lightBox.TolMinus4.ValueT)));
                    barcodeEntity = new BarcodeEntity();
                    barcodeEntity.WFMethod = wfMethod;
                    sequence.AddSequence(barcodeEntity);
                }

                barcodeEntity = new BarcodeEntity();
                barcodeEntity.Command = new BarcodeEntityCommand() { ACMethodName = nameof(ResetWeighingCycle), ACCaption = "Reset weighing cycle", ACMethodInvoked = false };
                sequence.AddSequence(barcodeEntity);

                sequence.State = datamodel.BarcodeSequenceBase.ActionState.Completed;
            }
            else
            {
                sequence.Message = new Msg(eMsgLevel.Info, "The light box has not any active order!");
                sequence.State = datamodel.BarcodeSequenceBase.ActionState.Completed;
            }
        }

        public void ResetWeighingCycle(ACComponent component)
        {
            PAESamplePiLightBox lightBox = component as PAESamplePiLightBox;
            if (lightBox != null)
                lightBox.ResetWeighingCycle();
        }

        protected override Type OnGetControlledType()
        {
            return typeof(PAESamplePiLightBox);
        }
    }
}
