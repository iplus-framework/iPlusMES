using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue(nameof(PWWorkTaskGeneric.AutoComplete), typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(PWWorkTaskGeneric.AutoComplete), "en{'Auto complete on scan'}de{'Beende automatisch bei Scan'}");

            method.ParameterValueList.Add(new ACValue(nameof(AllowEditProgramLogTime), typeof(short), 0, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(AllowEditProgramLogTime), "en{'Allow edit program log time (0 - Not allowed, 1 - Allowed, 2 - Required)'}de{'Allow edit program log time (0 - Not allowed, 1 - Allowed, 2 - Required)'}");

            method.ParameterValueList.Add(new ACValue(nameof(PWWorkTaskGeneric.PostingQuantitySuggestionMode), typeof(PostingQuantitySuggestionMode), facility.PostingQuantitySuggestionMode.OrderQuantity, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(PWWorkTaskGeneric.PostingQuantitySuggestionMode), "en{'Posting quantity suggestion mode'}de{'Buchungsmengen-Vorschlagsmodus'}");

            //example: 1,2,3 or 1-3,4,5,
            method.ParameterValueList.Add(new ACValue(nameof(PWWorkTaskGeneric.ValidSeqNoPostingQSMode), typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(PWWorkTaskGeneric.ValidSeqNoPostingQSMode), "en{'Valid sequence no. on posting quantity suggestion'}de{'Gültige laufende Nummer auf Buchungsmengenvorschlag'}");

            method.ParameterValueList.Add(new ACValue(nameof(PWWorkTaskGeneric.PostingQuantitySuggestionMode2), typeof(PostingQuantitySuggestionMode), gip.mes.facility.PostingQuantitySuggestionMode.OrderQuantity, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(PWWorkTaskGeneric.PostingQuantitySuggestionMode2), "en{'Posting quantity suggestion mode 2'}de{'Buchungsmengen-Vorschlagsmodus 2'}");

            method.ParameterValueList.Add(new ACValue(nameof(PWWorkTaskGeneric.ValidSeqNoPostingQSMode2), typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(PWWorkTaskGeneric.ValidSeqNoPostingQSMode2), "en{'Valid sequence no. on posting quantity suggestion 2'}de{'Gültige laufende Nummer auf Buchungsmengenvorschlag 2'}");

            method.ParameterValueList.Add(new ACValue(nameof(PWWorkTaskGeneric.InwardPostingSuggestionQ), typeof(double), null, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(PWWorkTaskGeneric.InwardPostingSuggestionQ), "en{'Suggestion Quantity for Inward Posting (- = Zero Stock)'}de{'Vorschlagsmenge für Zugangsbuchung (- = Nullbestand)'}");

            method.ParameterValueList.Add(new ACValue(nameof(AllowEditProductionTime), typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add(nameof(AllowEditProductionTime), "en{'Manufacturing date changeable'}de{'Herstelldatum änderbar'}");

            method.ResultValueList.Add(new ACValue("WorkingHours", typeof(double), 0, Global.ParamOption.Optional));
            resultTranslation.Add("WorkingHours", "en{'Working hours (FTE)'}de{'Arbeitszeit (FTE)'}");

            return new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", thisType, paramTranslation, resultTranslation);
        }

        public PWWorkTaskGeneric(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            return await base.ACDeInit(deleteACClassTask);
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

        public double InwardPostingSuggestionQ
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue(nameof(InwardPostingSuggestionQ));
                    if (acValue != null)
                        return (double)acValue.Value;
                }
                return 0;
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

        public short AllowEditProgramLogTime
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue(nameof(AllowEditProgramLogTime));
                    if (acValue != null)
                        return acValue.ParamAsInt16;
                }
                return 0;
            }
        }

        public bool AllowEditProductionTime
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue(nameof(AllowEditProductionTime));
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

        public virtual Msg VerifyOrderPostingsOnRelease(bool throwAlarm = false, core.datamodel.VBUser vbUser = null)
        {
            using (Database db = new core.datamodel.Database())
            using(DatabaseApp dbApp = new DatabaseApp(db))
            {
                ProdOrderPartslistPos intermediatePos, intermediateChildPos, endBatchPos;
                MaterialWFConnection[] matWFConnections;

                GetAssignedIntermediate(dbApp, out intermediatePos, out intermediateChildPos, out endBatchPos, out matWFConnections);

                if (intermediateChildPos == null)
                    return null;

                List<ProdOrderPartslistPos> intermediateChildrenList;

                if (matWFConnections.Count() > 1)
                {
                    intermediateChildrenList = GetReleatedIntermediates(matWFConnections, endBatchPos, intermediateChildPos, intermediatePos);
                }
                else
                {
                    intermediateChildrenList = new List<ProdOrderPartslistPos>
                    {
                        intermediateChildPos
                    };
                }

                List<Guid> intermediateChildrenIdList = intermediateChildrenList.Select(c => c.ProdOrderPartslistPosID).ToList();

                bool anyWithoutPosting = dbApp.ProdOrderPartslistPosRelation.Include(c => c.SourceProdOrderPartslistPos)
                                                                                .Include(c => c.SourceProdOrderPartslistPos.Material)
                                                                                .Include(c => c.FacilityBooking_ProdOrderPartslistPosRelation)
                                                                                .Include(c => c.MDProdOrderPartslistPosState)
                                                                                .Where(c => intermediateChildrenIdList.Contains(c.TargetProdOrderPartslistPosID)
                                                                                            && c.TargetQuantityUOM > 0.00001)
                                                                                .ToArray()
                                                                                .Where(c => c.MDProdOrderPartslistPosState != null
                                                                                        && c.MDProdOrderPartslistPosState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                                                                        && c.MDProdOrderPartslistPosState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled
                                                                                        && !c.SourceProdOrderPartslistPos.Material.IsIntermediate
                                                                                        && (c.SourceProdOrderPartslistPos != null && c.SourceProdOrderPartslistPos.Material != null
                                                                                         && c.SourceProdOrderPartslistPos.Material.UsageACProgram))
                                                                                .Where(c => !(c.FacilityBookingCharge_ProdOrderPartslistPosRelation.Any())).Any();
                                           
                if (anyWithoutPosting)
                {
                    bool hasRight = true;
                    if (vbUser != null)
                    {
                        ACProdOrderManager prodOrderManager = ACProdOrderManager.GetServiceInstance(this);
                        string name;
                        var method = prodOrderManager?.GetACClassMethod(nameof(prodOrderManager.FinishOrderOutwardMissing), out name);
                        if (method != null)
                        {
                            var groups = method.VBGroupRight_ACClassMethod.Where(c => c.VBGroup.VBUserGroup_VBGroup.Any(x => x.VBUserID == vbUser.VBUserID)).ToArray();
                            if (groups.Any(c => c.ControlModeIndex == (short)Global.ControlModes.Disabled) && !groups.Any(c => c.ControlModeIndex != (short)Global.ControlModes.Disabled))
                                hasRight = false;
                        }
                    }

                    if (throwAlarm)
                    {
                        ProdOrderPartslist poPL = intermediateChildPos.ProdOrderPartslist;
                        //Warning50058: On order {0} {1} are not posted all positions!
                        Msg msg = new Msg(this, eMsgLevel.Warning, nameof(PWWorkTaskGeneric), nameof(VerifyOrderPostingsOnRelease), 313, "Warning50058", poPL.ProdOrder.ProgramNo, poPL.Partslist.PartslistName);
                        OnNewAlarmOccurred(OrderPostingAlarm, msg);
                    }

                    if (!hasRight)
                    {
                        return new Msg()
                        {
                            //Error50716: The consumption components are not posted! Please post the consumption components first, then you can release machine!
                            Message = Root.Environment.TranslateMessage(this, "Error50716"),
                            MessageLevel = eMsgLevel.Error,
                        };
                    }

                    return new Msg()
                    {
                        //Question50094: Please check if you perform all outward postings. Do you want continue with a release process?
                        Message = Root.Environment.TranslateMessage(this, "Question50094"),
                        MessageLevel = eMsgLevel.Question,
                        MessageButton = eMsgButton.YesNo
                    };
                }

                var tempPos = intermediateChildrenList.Where(c => c.IsFinalMixureBatch).FirstOrDefault();
                if (tempPos != null)
                    intermediateChildPos = tempPos;

                if (intermediateChildPos.IsFinalMixureBatch && !(intermediateChildPos.FacilityBookingCharge_ProdOrderPartslistPos.Any()))
                {
                    bool hasRight = true;
                    if (vbUser != null)
                    {
                        ACProdOrderManager prodOrderManager = ACProdOrderManager.GetServiceInstance(this);
                        string name;
                        var method = prodOrderManager?.GetACClassMethod(nameof(prodOrderManager.FinishOrderInwardMissing), out name);
                        if (method != null)
                        {
                            var groups = method.VBGroupRight_ACClassMethod.Where(c => c.VBGroup.VBUserGroup_VBGroup.Any(x => x.VBUserID == vbUser.VBUserID)).ToArray();
                            if (groups.Any(c => c.ControlModeIndex == (short)Global.ControlModes.Disabled) && !groups.Any(c => c.ControlModeIndex != (short)Global.ControlModes.Disabled))
                                hasRight = false;
                        }
                    }

                    if (throwAlarm)
                    {
                        ProdOrderPartslist poPL = intermediateChildPos.ProdOrderPartslist;
                        //Warning50059: On order {0} {1} is not posted inward quantity!
                        Msg msg = new Msg(this, eMsgLevel.Warning, nameof(PWWorkTaskGeneric), nameof(VerifyOrderPostingsOnRelease), 313, "Warning50059", poPL.ProdOrder.ProgramNo, poPL.Partslist.PartslistName);
                        OnNewAlarmOccurred(OrderPostingAlarm, msg);
                    }

                    if (!hasRight)
                    {
                        return new Msg()
                        {
                            //Error50717: The inward posting was not performed! Please perform the inward posting first, then you can release machine!
                            Message = Root.Environment.TranslateMessage(this, "Error50717"),
                            MessageLevel = eMsgLevel.Error
                        };
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


        internal static List<ProdOrderPartslistPos> GetReleatedIntermediates(MaterialWFConnection[] connectionList, ProdOrderPartslistPos endBatchPos, ProdOrderPartslistPos intermediateChildPos, ProdOrderPartslistPos intermediatePosition)
        {
            List<ProdOrderPartslistPos> resultList = new List<ProdOrderPartslistPos>();

            GetRelatedMatWFConn(connectionList, endBatchPos, resultList);

            return resultList;
        }

        private static void GetRelatedMatWFConn(MaterialWFConnection[] connectionList, ProdOrderPartslistPos currentPos, List<ProdOrderPartslistPos> resultList)
        {
            MaterialWFConnection matWFConnection = connectionList.FirstOrDefault(c => c.MaterialID == currentPos.MaterialID);

            if (matWFConnection != null)
            {
                if (currentPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                {
                    currentPos = currentPos.ProdOrderPartslistPos_ParentProdOrderPartslistPos.OrderByDescending(c => c.InsertDate).FirstOrDefault();
                }

                resultList.Add(currentPos);
            }

            foreach (ProdOrderPartslistPos sourcePos in currentPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Select(x => x.SourceProdOrderPartslistPos))
            {
                GetRelatedMatWFConn(connectionList, sourcePos, resultList);
            }
        }

        #endregion

    }
}
