using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Enter laborartory order'}de{'Laborauftrag eingeben'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWLabOrderAck : PWNodeUserAck
    {
        #region c'tors

        static PWLabOrderAck()
        {
            RegisterExecuteHandler(typeof(PWLabOrderAck), HandleExecuteACMethod_PWLabOrderAck);

            List<ACMethodWrapper> wrappers = ACMethod.OverrideFromBase(typeof(PWLabOrderAck), ACStateConst.SMStarting);
            if (wrappers != null)
            {
                foreach (ACMethodWrapper wrapper in wrappers)
                {
                    wrapper.Method.ResultValueList.Add(new ACValue("AlwaysCreateNew", typeof(bool), "", Global.ParamOption.Optional));
                    wrapper.ResultTranslation.Add("AlwaysCreateNew", "en{'Alway create a new Laboratory order'}de{'Immer neuen Laborauftrag anlegen'}");
                    //wrapper.CaptionTranslation = "en{'Enter laborartory order'}de{'Laborauftrag eingeben'}";
                }
            }
        }

        public PWLabOrderAck(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            return await base.ACDeInit(deleteACClassTask);
        }

        public new const string PWClassName = nameof(PWLabOrderAck);

        #endregion

        #region Properties


        protected bool AlwaysCreateNew
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("AlwaysCreateNew");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }


        #endregion

        #region Methods

        public override void Start()
        {
            base.Start();
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            base.SMStarting();
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }

        [ACMethodInfo("", "en{'Alway create a new Laboratory order'}de{'Immer neuen Laborauftrag anlegen'}", 400, false)]
        public bool GetAlwaysCreateNew()
        {
            return AlwaysCreateNew;
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            switch (acMethodName)
            {
                case nameof(GetAlwaysCreateNew):
                    result = GetAlwaysCreateNew();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PWLabOrderAck(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(AckStartClient):
                    AckStartClient(acComponent);
                    return true;
            }
            return HandleExecuteACMethod_PWNodeUserAck(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }


        public static async new void AckStartClient(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            if (!IsEnabledAckStartClient(acComponent))
                return;
            ACStateEnum acState = (ACStateEnum)_this.ACUrlCommand("ACState");

            // If needs Password
            if (acState == ACStateEnum.SMStarting)
            {
                string bsoName = "BSOChangeMyPW";
                ACBSO childBSO = acComponent.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACBSO;
                if (childBSO == null)
                    childBSO = acComponent.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACBSO;
                if (childBSO == null)
                    return;
                var dlgResultAsync = childBSO.ACUrlCommand("!ShowCheckUserDialog") as Task<VBDialogResult>;
                VBDialogResult dlgResult = await dlgResultAsync;
                await childBSO.Stop();
                if (dlgResult != null && dlgResult.SelectedCommand == eMsgButton.OK)
                {
                    PAShowDlgManagerVB serviceInstance = PAShowDlgManagerBase.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
                    if (serviceInstance != null)
                    {
                        bool createNew = (bool)acComponent.ExecuteMethod(nameof(GetAlwaysCreateNew));
                        serviceInstance.GenerateNewLabOrder(acComponent, createNew);
                    }

                    string userName = "";
                    if (dlgResult.ReturnValue != null)
                        userName = (dlgResult.ReturnValue as ACValueItem)?.Value?.ToString();

                    if (string.IsNullOrEmpty(userName))
                        acComponent.ACUrlCommand("!" + nameof(AckStart));
                    else
                        acComponent.ACUrlCommand("!" + nameof(AckStartUserName), userName);
                }
            }
            else
            {
                PAShowDlgManagerVB serviceInstance = PAShowDlgManagerBase.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
                if (serviceInstance != null)
                {
                    bool createNew = (bool)acComponent.ExecuteMethod(nameof(GetAlwaysCreateNew));
                    serviceInstance.GenerateNewLabOrder(acComponent, createNew);
                }
                acComponent.ACUrlCommand("!" + nameof(AckStart));
            }
        }

        #endregion
    }
}
