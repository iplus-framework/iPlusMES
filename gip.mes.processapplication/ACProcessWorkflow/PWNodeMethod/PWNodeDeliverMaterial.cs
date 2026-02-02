using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using System.Xml;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;

namespace gip.mes.processapplication
{

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Virtual material transport'}de{'Virtueller Materialabtransport'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWNodeDeliverMaterial : PWBaseNodeProcess, IPWNodeDeliverMaterial
    {
        public const string PWClassName = "PWNodeDeliverMaterial";

        #region Constructors

        static PWNodeDeliverMaterial()
        {
            ACMethod TMP;
            TMP = new ACMethod(ACStateConst.SMStarting);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            TMP.ParameterValueList.Add(new ACValue("CorrectLastInwardQuantOnEnd", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("CorrectLastInwardQuantOnEnd", "en{'Correct last inward quant on end according inward posting'}de{'Letztes Einwärtsquant am Ende entsprechend der Einbuchung korrigieren'}");

            TMP.ParameterValueList.Add(new ACValue("OutwardSeqNo", typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add("OutwardSeqNo", "en{'Outward sequence numbers for inward quant correction'}de{'Sequenznummern im Ausgang für die Korrektur der Eingangsquanten'}");

            var wrapper = new ACMethodWrapper(TMP, "en{'SVirtual material transport'}de{'Virtueller Materialabtransport'}", typeof(PWNodeDeliverMaterial), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWNodeDeliverMaterial), ACStateConst.SMStarting, wrapper);

            RegisterExecuteHandler(typeof(PWNodeDeliverMaterial), HandleExecuteACMethod_PWNodeDeliverMaterial);
        }

        public PWNodeDeliverMaterial(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            return await base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties
        public Route CurrentDischargingRoute { get { return null; } set { } }

        public override bool MustBeInsidePWGroup
        {
            get
            {
                return false;
            }
        }

        public bool CorrectLastQuantOnEnd
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("CorrectLastInwardQuantOnEnd");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        public string OutwardSeqNo
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("OutwardSeqNo");
                    if (acValue != null)
                    {
                        return acValue.ParamAsString;
                    }
                }
                return null;
            }
        }

        #region Properties => Managers

        public ACProdOrderManager ProdOrderManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.ProdOrderManager : null;
            }
        }

        protected FacilityManager ACFacilityManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.ACFacilityManager as FacilityManager : null;
            }
        }


        #endregion

        #endregion

        #region Public

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWNodeDeliverMaterial(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWBaseNodeProcess(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        public T ParentPWMethod<T>() where T : PWMethodVBBase
        {
            if (ParentRootWFNode == null)
                return null;
            return ParentRootWFNode as T;
        }

        [ACMethodInfo("","",9999)]
        public void CorrectInwardQuantsAccordingOutwardPostings()
        {
            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            // If dosing is not for production, then do nothing
            if (pwMethodProduction == null)
                return;

            Msg msg;

            if (ACFacilityManager == null || ProdOrderManager == null)
            {
                //Error50571: The facility manager and/or prod order manager is not available.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, nameof(CorrectInwardQuantsAccordingOutwardPostings) + "(10)", 150, "Error50571");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, false);

                return;
            }

            using (var dbIPlus = new Database())
            {
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    ProdOrderPartslistPos intermediateChildPos;
                    ProdOrderPartslistPos intermediatePosition;
                    MaterialWFConnection matWFConnection;
                    ProdOrderBatch batch;
                    ProdOrderBatchPlan batchPlan;
                    ProdOrderPartslistPos endBatchPos;
                    MaterialWFConnection[] matWFConnections;
                    bool posFound = PWDosing.GetRelatedProdOrderPosForWFNode(this, dbIPlus, dbApp, pwMethodProduction,
                        out intermediateChildPos, out intermediatePosition, out endBatchPos, out matWFConnection, out batch, out batchPlan, out matWFConnections);
                    if (batch == null)
                    {
                        // Error50570: No batch assigned to last intermediate material of this workflow.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, nameof(CorrectInwardQuantsAccordingOutwardPostings) + "(20)", 168, "Error50570");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return;
                    }
                    else if (endBatchPos == null)
                    {
                        // Error50572: The last intermediate material not exist!
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, nameof(CorrectInwardQuantsAccordingOutwardPostings) + "(20)", 168, "Error50572");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return;
                    }

                    List<int> validSequenceNumbers = null;

                    if (OutwardSeqNo != null)
                    {
                        var parts = OutwardSeqNo.Split(',');

                        validSequenceNumbers = new List<int>();

                        foreach (var part in parts)
                        {
                            if (part.Contains("-"))
                            {
                                var fromTo = part.Split('-');
                                string from = fromTo.FirstOrDefault();
                                string to = fromTo.LastOrDefault();

                                int fromSeq = 0;
                                if (!string.IsNullOrEmpty(from) && int.TryParse(from, out fromSeq))
                                {
                                    int toSeq = 0;
                                    if (!string.IsNullOrEmpty(to) && int.TryParse(to, out toSeq))
                                    {
                                        for (int i = fromSeq; i <= toSeq; i++)
                                        {
                                            validSequenceNumbers.Add(i);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                int seq = 0;
                                if (!string.IsNullOrEmpty(part) && int.TryParse(part, out seq))
                                {
                                    validSequenceNumbers.Add(seq);
                                }
                            }
                        }

                        if (!validSequenceNumbers.Any())
                            validSequenceNumbers = null;
                    }


                    MsgWithDetails error = ProdOrderManager.CorrectLastInwardQuantAccordingOutwardPostings(ACFacilityManager, dbApp, batch, endBatchPos, validSequenceNumbers); 
                    if (error != null)
                    {
                        if (IsAlarmActive(ProcessAlarm, error.DetailsAsText) == null)
                            Messages.LogError(this.GetACUrl(), error.ACIdentifier, error.DetailsAsText);
                        OnNewAlarmOccurred(ProcessAlarm, error, false);
                        return;
                    }
                }
            }
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            RecalcTimeInfo();
            if (CreateNewProgramLog(NewACMethodWithConfiguration()) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                return;

            if (CorrectLastQuantOnEnd)
            {
                CorrectInwardQuantsAccordingOutwardPostings();
            }

            // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
            if (CurrentACState == ACStateEnum.SMStarting)
                CurrentACState = ACStateEnum.SMCompleted;
        }

        public override void SMIdle()
        {
            ClearMyConfiguration();
            base.SMIdle();
        }

        public override void Reset()
        {
            ClearMyConfiguration();
            base.Reset();
        }

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList, ref DumpStats dumpStats)
        {
            base.DumpPropertyList(doc, xmlACPropertyList, ref dumpStats);

            XmlElement xmlChild = xmlACPropertyList[nameof(CorrectLastQuantOnEnd)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(CorrectLastQuantOnEnd));
                if (xmlChild != null)
                    xmlChild.InnerText = CorrectLastQuantOnEnd.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList[nameof(OutwardSeqNo)];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement(nameof(OutwardSeqNo));
                if (xmlChild != null)
                    xmlChild.InnerText = OutwardSeqNo;
                xmlACPropertyList.AppendChild(xmlChild);
            }
        }

            #endregion

        }
}
