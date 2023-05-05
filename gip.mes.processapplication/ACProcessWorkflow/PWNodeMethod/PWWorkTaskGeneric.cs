using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Xml;
using gip.mes.facility;
using gip.mes.datamodel;
using Microsoft.EntityFrameworkCore;

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

        [ACPropertyBindingSource(600, "Error", "en{'Function error'}de{'Funktionsfehler'}", "", false, false)]
        public IACContainerTNet<PANotifyState> OrderPostingAlarm
        {
            get;
            set;
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

        public override void Reset()
        {
            base.Reset();
            ClearMyConfiguration();
        }

        public override void SMIdle()
        {
            base.SMIdle();
            ClearMyConfiguration();
        }

        public override Msg OnGetMessageOnReleasingProcessModule(PAFWorkTaskScanBase invoker, bool pause)
        {
            if (!pause)
                VerifyOrderPostingsOnRelease(true);

            return base.OnGetMessageOnReleasingProcessModule(invoker, pause);
        }

        public virtual Msg VerifyOrderPostingsOnRelease(bool throwAlarm = false)
        {
            using (Database db = new core.datamodel.Database())
            using(DatabaseApp dbApp = new DatabaseApp(db))
            {
                ProdOrderPartslistPos intermediatePos, intermediateChildPos;

                GetAssignedIntermediate(dbApp, out intermediatePos, out intermediateChildPos);

                if (intermediateChildPos == null)
                    return null;


                bool anyWithoutPosting =  dbApp.ProdOrderPartslistPosRelation.Include(c => c.SourceProdOrderPartslistPos)
                                                                            .Include(c => c.SourceProdOrderPartslistPos.Material)
                                                                            .Include(c => c.FacilityBooking_ProdOrderPartslistPosRelation)
                                                                            .Include(c => c.MDProdOrderPartslistPosState)
                                                                            .Where(c => c.TargetProdOrderPartslistPosID == intermediateChildPos.ProdOrderPartslistPosID
                                                                                        && c.TargetQuantityUOM > 0.00001)
                                                                            .ToArray()
                                                                            .Where(c => c.MDProdOrderPartslistPosState != null
                                                                                    && c.MDProdOrderPartslistPosState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                                                                    && c.MDProdOrderPartslistPosState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled
                                                                                    && (c.SourceProdOrderPartslistPos != null && c.SourceProdOrderPartslistPos.Material != null
                                                                                     && c.SourceProdOrderPartslistPos.Material.UsageACProgram))
                                                                            .Where(c => !(c.FacilityBookingCharge_ProdOrderPartslistPosRelation.Any())).Any();
                                           
                if (anyWithoutPosting)
                {
                    if (throwAlarm)
                    {
                        ProdOrderPartslist poPL = intermediateChildPos.ProdOrderPartslist;
                        //Warning50058: On order {0} {1} are not posted all positions!
                        Msg msg = new Msg(this, eMsgLevel.Warning, nameof(PWWorkTaskGeneric), nameof(VerifyOrderPostingsOnRelease), 313, "Warning50058", poPL.ProdOrder.ProgramNo, poPL.Partslist.PartslistName);
                        OnNewAlarmOccurred(OrderPostingAlarm, msg);
                    }

                    return new Msg()
                    {
                        //Question50094: Please check if you perform all outward postings. Do you want continue with a release process?
                        Message = Root.Environment.TranslateMessage(this, "Question50094"),
                        MessageLevel = eMsgLevel.Question,
                        MessageButton = eMsgButton.YesNo
                    };
                }

                if (intermediateChildPos.IsFinalMixureBatch && !(intermediateChildPos.FacilityBookingCharge_ProdOrderPartslistPos.Any()))
                {
                    if (throwAlarm)
                    {
                        ProdOrderPartslist poPL = intermediateChildPos.ProdOrderPartslist;
                        //Warning50059: On order {0} {1} is not posted inward quantity!
                        Msg msg = new Msg(this, eMsgLevel.Warning, nameof(PWWorkTaskGeneric), nameof(VerifyOrderPostingsOnRelease), 313, "Warning50059", poPL.ProdOrder.ProgramNo, poPL.Partslist.PartslistName);
                        OnNewAlarmOccurred(OrderPostingAlarm, msg);
                    }

                    return new Msg()
                    {
                        //Question50095: Please check if you perform a inward posting. Do you want continue with a release process?
                        Message = Root.Environment.TranslateMessage(this, "Question50095"),
                        MessageLevel = eMsgLevel.Question,
                        MessageButton = eMsgButton.YesNo
                    };
                }
            }


            return null;
        }

        #endregion

    }
}
