using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.core.autocomponent;
using System.Data;
using System.Xml;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Process-Knoten zur implementierung eines untergeordneten (asynchronen) ACClassMethod-Aufruf auf die Model-Welt
    /// 
    /// Methoden zur Steuerung von außen: 
    /// -Start()    Starten des Processes
    ///
    /// Mögliche ACState:
    /// SMIdle      (Definiert in ACComponent)
    /// SMStarting (Definiert in PWNode)
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PWManualAddition'}de{'PWManualAddition'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWManualAddition : PWManualWeighing
    {
        public new const string PWClassName = nameof(PWManualAddition);

        static PWManualAddition()
        {
            List<ACMethodWrapper> wrappers = ACMethod.OverrideFromBase(typeof(PWManualAddition), ACStateConst.SMStarting);
            if (wrappers != null)
            {
                foreach (ACMethodWrapper wrapper in wrappers)
                {
                    wrapper.Method.ParameterValueList.Add(new ACValue("OnlyAcknowledge", typeof(bool), false, Global.ParamOption.Optional)); // Acknowledge without entering added quantity
                    wrapper.ParameterTranslation.Add("OnlyAcknowledge", "en{'Only acknowledge'}de{'Nur quittieren'}");
                }
            }
            RegisterExecuteHandler(typeof(PWManualAddition), HandleExecuteACMethod_PWManualAddition);
        }

        public PWManualAddition(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            return await base.ACDeInit(deleteACClassTask);
        }

        public bool OnlyAcknowledge
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("OnlyAcknowledge");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            base.SMStarting();
        }

        public override void Start()
        {
            base.Start();
        }

        public override bool IsManualWeighing => false;

        public static bool HandleExecuteACMethod_PWManualAddition(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWManualWeighing(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList, ref DumpStats dumpStats)
        {
            base.DumpPropertyList(doc, xmlACPropertyList, ref dumpStats);

            XmlElement xmlChild = xmlACPropertyList[nameof(OnlyAcknowledge)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(OnlyAcknowledge));
                if (xmlChild != null)
                    xmlChild.InnerText = OnlyAcknowledge.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }
        }
    }
}
