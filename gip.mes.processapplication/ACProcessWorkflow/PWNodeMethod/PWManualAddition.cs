using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.core.autocomponent;
using System.Data;

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
        public new const string PWClassName = "PWManualAddition";

        static PWManualAddition()
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue("FreeSelectionMode", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("FreeSelectionMode", "en{'Material to be weighed can be freely selected'}de{'Zu verwiegendes Material kann frei ausgewählt werden'}");

            method.ParameterValueList.Add(new ACValue("AutoSelectLot", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("AutoSelectLot", "en{'Automatically select lot'}de{'Los automatisch auswählen'}");

            method.ParameterValueList.Add(new ACValue("AutoSelectLotPrio", typeof(LotUsageEnum), LotUsageEnum.ExpirationFirst, Global.ParamOption.Optional));
            paramTranslation.Add("AutoSelectLotPrio", "en{'Priority of auto lot selection'}de{'Priorität der automatischen Losauswahl'}");

            method.ParameterValueList.Add(new ACValue("EnterLotManually", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("EnterLotManually", "en{'Enter lot manually'}de{'Los manuell eingeben'}");

            method.ParameterValueList.Add(new ACValue("LotValidation", typeof(LotUsageEnum?), null, Global.ParamOption.Optional));
            paramTranslation.Add("LotValidation", "en{'Lot validation'}de{'Chargenvalidierung'}");

            method.ParameterValueList.Add(new ACValue("OnlyAcknowledge", typeof(bool), false, Global.ParamOption.Optional)); // Acknowledge without entering added quantity
            paramTranslation.Add("OnlyAcknowledge", "en{'Only acknowledge'}de{'Nur quittieren'}");

            method.ParameterValueList.Add(new ACValue("ComponentsSeqFrom", typeof(Int32), 0, Global.ParamOption.Optional));
            paramTranslation.Add("ComponentsSeqFrom", "en{'Components from Seq.-No.'}de{'Komponenten VON Seq.-Nr.'}");

            method.ParameterValueList.Add(new ACValue("ComponentsSeqTo", typeof(Int32), 0, Global.ParamOption.Optional));
            paramTranslation.Add("ComponentsSeqTo", "en{'Components to Seq.-No.'}de{'Komponenten BIS Seq.-Nr.'}");

            method.ParameterValueList.Add(new ACValue("AutoInsertQuantToStore", typeof(string), false, Global.ParamOption.Optional));
            paramTranslation.Add("AutoInsertQuantToStore", "en{'Store for automatic quant creation'}de{'Lagerplatz für automatische Quantanlage'}");

            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWManualAddition), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWManualAddition), ACStateConst.SMStarting, wrapper);
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
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
    }
}
