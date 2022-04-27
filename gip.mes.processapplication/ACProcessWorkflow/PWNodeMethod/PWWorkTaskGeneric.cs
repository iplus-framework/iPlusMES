using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Xml;
using gip.mes.facility;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Generic work task'}de{'Allgemeine Arbeitsaufgabe'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWWorkTaskGeneric : PWWorkTaskScanBase
    {
        new public const string PWClassName = nameof(PWWorkTaskGeneric);

        #region Constructors

        static PWWorkTaskGeneric()
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            //method.ParameterValueList.Add(new ACValue("PiecesPerRack", typeof(int), 0, Global.ParamOption.Required));
            //paramTranslation.Add("PiecesPerRack", "en{'Capacity: Pieces per oven rack'}de{'Kapazität: Stücke pro Stikkenwagen'}");
            method.ParameterValueList.Add(new ACValue("AutoComplete", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("AutoComplete", "en{'Auto complete'}de{'Autovervollständigung'}");

            method.ParameterValueList.Add(new ACValue("PostingQuantitySuggestionMode", typeof(PostingQuantitySuggestionMode), facility.PostingQuantitySuggestionMode.OrderQuantity, Global.ParamOption.Optional));
            paramTranslation.Add("PostingQuantitySuggestionMode", "en{'Posting quantity suggestion mode'}de{'Buchungsmengen-Vorschlagsmodus'}");

            //example: 1,2,3 or 1-3,4,5,
            method.ParameterValueList.Add(new ACValue("ValidSeqNoPostingQSMode", typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add("ValidSeqNoPostingQSMode", "en{'Valid sequence no. on posting quantity suggestion'}de{'Gültige laufende Nummer auf Buchungsmengenvorschlag'}");

            method.ParameterValueList.Add(new ACValue("OrderQuantityOnInwardPosting", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("OrderQuantityOnInwardPosting", "en{'Order quantity on inward posting'}de{'Order quantity on inward posting'}");

            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWWorkTaskGeneric), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWWorkTaskGeneric), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWWorkTaskGeneric), HandleExecuteACMethod_PWWorkTaskGeneric);
        }

        public PWWorkTaskGeneric(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        public bool AutoComplete
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("AutoComplete");
                    if (acValue != null)
                        return acValue.ParamAsBoolean;
                }
                return false;
            }
        }

        #endregion

        #region Methods

        public override void SMRunning()
        {
            base.SMRunning();
            if (AutoComplete)
            {
                ResetAndComplete();
            }    
        }

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            //result = null;
            //switch (acMethodName)
            //{
            //}
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PWWorkTaskGeneric(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWWorkTaskScanBase(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            base.SMStarting();
        }

        #endregion

    }
}
