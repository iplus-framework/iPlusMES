using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Xml;
using gip.mes.facility;
using gip.mes.datamodel;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Generic work task'}de{'Allgemeine Arbeitsaufgabe'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWWorkTaskGeneric : PWWorkTaskScanBase
    {
        new public const string PWClassName = nameof(PWWorkTaskGeneric);

        #region Constructors

        static PWWorkTaskGeneric()
        {
            var wrapper = CreateACMethodWrapper(typeof(PWWorkTaskGeneric));
            ACMethod.RegisterVirtualMethod(typeof(PWWorkTaskGeneric), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWWorkTaskGeneric), HandleExecuteACMethod_PWWorkTaskGeneric);
        }

        protected static ACMethodWrapper CreateACMethodWrapper(Type thisType)
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue(nameof(PWWorkTaskGeneric.AutoComplete), typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(PWWorkTaskGeneric.AutoComplete), "en{'Auto complete on scan'}de{'Beende automatisch bei Scan'}");

            method.ParameterValueList.Add(new ACValue(nameof(PWWorkTaskGeneric.PostingQuantitySuggestionMode), typeof(PostingQuantitySuggestionMode), facility.PostingQuantitySuggestionMode.OrderQuantity, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(PWWorkTaskGeneric.PostingQuantitySuggestionMode), "en{'Posting quantity suggestion mode'}de{'Buchungsmengen-Vorschlagsmodus'}");

            //example: 1,2,3 or 1-3,4,5,
            method.ParameterValueList.Add(new ACValue(nameof(PWWorkTaskGeneric.ValidSeqNoPostingQSMode), typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(PWWorkTaskGeneric.ValidSeqNoPostingQSMode), "en{'Valid sequence no. on posting quantity suggestion'}de{'Gültige laufende Nummer auf Buchungsmengenvorschlag'}");

            method.ParameterValueList.Add(new ACValue(nameof(PWWorkTaskGeneric.PostingQuantitySuggestionMode2), typeof(PostingQuantitySuggestionMode), gip.mes.facility.PostingQuantitySuggestionMode.OrderQuantity, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(PWWorkTaskGeneric.PostingQuantitySuggestionMode2), "en{'Posting quantity suggestion mode 2'}de{'Buchungsmengen-Vorschlagsmodus 2'}");

            method.ParameterValueList.Add(new ACValue(nameof(PWWorkTaskGeneric.ValidSeqNoPostingQSMode2), typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(PWWorkTaskGeneric.ValidSeqNoPostingQSMode2), "en{'Valid sequence no. on posting quantity suggestion 2'}de{'Gültige laufende Nummer auf Buchungsmengenvorschlag 2'}");

            return new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", thisType, paramTranslation, null);
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

        public PostingQuantitySuggestionMode? PostingQuantitySuggestionMode
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue(nameof(PostingQuantitySuggestionMode));
                    if (acValue != null)
                        return acValue.Value as PostingQuantitySuggestionMode?;
                }
                return null;
            }
        }

        public string ValidSeqNoPostingQSMode
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue(nameof(ValidSeqNoPostingQSMode));
                    if (acValue != null)
                        return acValue.Value as string;
                }
                return null;
            }
        }

        public PostingQuantitySuggestionMode? PostingQuantitySuggestionMode2
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue(nameof(PostingQuantitySuggestionMode2));
                    if (acValue != null)
                        return acValue.Value as PostingQuantitySuggestionMode?;
                }
                return null;
            }
        }

        public string ValidSeqNoPostingQSMode2
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue(nameof(ValidSeqNoPostingQSMode2));
                    if (acValue != null)
                        return acValue.Value as string;
                }
                return null;
            }
        }

        public bool AutoComplete
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue(nameof(AutoComplete));
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

        public virtual Msg IsOrderCompletedOnRelease(PAFWorkTaskScanBase invoker)
        {
            Msg question = null;

            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            // If dosing is not for production, then do nothing
            if (pwMethodProduction == null)
                return null;

            using (var dbIPlus = new Database())
            {
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    ProdOrderBatch batch = dbApp.ProdOrderBatch.Include(c => c.ProdOrderPartslistPosRelation_ProdOrderBatch)
                                                               .FirstOrDefault(c => c.ProdOrderBatchID == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID);

                    ProdOrderPartslistPos endBatchPos = dbApp.ProdOrderPartslistPos
                                                             .Include(c => c.FacilityBookingCharge_ProdOrderPartslistPos)
                                                             .FirstOrDefault(c => c.ProdOrderPartslistID == pwMethodProduction.CurrentProdOrderPartslistPos.ProdOrderPartslistID);

                    if (batch == null)
                    {
                        //// Error50570: No batch assigned to last intermediate material of this workflow.
                        //msg = new Msg(this, eMsgLevel.Error, PWClassName, nameof(IsOrderCompletedOnRelease) + "(20)", 168, "Error50570");

                        //if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        //    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        //OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return null;
                    }
                    else if (endBatchPos == null)
                    {
                        //// Error50572: The last intermediate material not exist!
                        //msg = new Msg(this, eMsgLevel.Error, PWClassName, nameof(CorrectInwardQuantsAccordingOutwardPostings) + "(20)", 168, "Error50572");

                        //if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        //    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        //OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return null;
                    }

                    if (!endBatchPos.FacilityBookingCharge_ProdOrderPartslistPos.Any())
                    {
                        return new Msg()
                        {
                            Message = "Please check if you perform a inward posting. Do you want continue with release process?",
                            MessageLevel = eMsgLevel.Question,
                            MessageButton = eMsgButton.YesNo
                        };
                    }

                    var relations = batch.ProdOrderPartslistPosRelation_ProdOrderBatch.Where(c => !c.SourceProdOrderPartslistPos.Material.IsIntermediate);

                    foreach (var relation in relations)
                    {
                        if (relation.FacilityBookingCharge_ProdOrderPartslistPosRelation.Any()
                            || relation.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                            || relation.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled)
                        {
                            continue;
                        }

                        return new Msg()
                        {
                            Message = "Please check if you perform all outward postings. Do you want continue with release process?",
                            MessageLevel = eMsgLevel.Question,
                            MessageButton = eMsgButton.YesNo
                        };

                    }
                }
            }


            return question;
        }

        #endregion

    }
}
