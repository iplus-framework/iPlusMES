using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using System.Xml;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Sample light box RaspPi node'}de{'Stichproben Ampelbox Knoten'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWSamplePiLightBox : PWBaseNodeProcess
    {
        public const string PWClassName = "PWSamplePiLightBox";

        #region Properties
        #endregion

        #region Constructors

        static PWSamplePiLightBox()
        {
            ACMethod TMP;
            TMP = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> translation = new Dictionary<string, string>();
            
            TMP.ParameterValueList.Add(new ACValue("PiCommand", typeof(SamplePiCommand), (short)SamplePiCommand.SendOrder, Global.ParamOption.Required));
            translation.Add("PiCommand", "en{'Sample command'}de{'Stichproben Kommando'}");

            var wrapper = new ACMethodWrapper(TMP, "en{'Sample command'}de{'Stichproben Kommando'}", typeof(PWSamplePiLightBox), translation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWSamplePiLightBox), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWSamplePiLightBox), HandleExecuteACMethod_PWSamplePiLightBox);
        }

        public PWSamplePiLightBox(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            ClearMyConfiguration();
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }

        #endregion

        #region Public

        #region Properties
        public SamplePiCommand PiCommand
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("PiCommand");
                    if (acValue != null)
                        return (SamplePiCommand) acValue.ParamAsInt16;
                }
                return SamplePiCommand.SendOrder;
            }
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWSamplePiLightBox(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWBaseNodeProcess(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            //if (!PreExecute(PABaseState.SMStarting))
            //  return;
            RecalcTimeInfo();
            CreateNewProgramLog(NewACMethodWithConfiguration());

            var rootPW = RootPW;

            if (PiCommand == SamplePiCommand.SendOrder)
            {
            }
            else
            {
            }

            if (CurrentACState == ACStateEnum.SMStarting)
                CurrentACState = ACStateEnum.SMCompleted;
        }

        [ACMethodState("en{'Running'}de{'Läuft'}", 30, true)]
        public override void SMRunning()
        {
        }

        [ACMethodState("en{'Completed'}de{'Beendet'}", 40, true)]
        public override void SMCompleted()
        {
            base.SMCompleted();
        }

        #endregion

        #region Protected

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);
        }

        #endregion
    }
}
