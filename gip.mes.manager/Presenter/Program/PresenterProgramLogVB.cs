using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.manager;
using gip.core.autocomponent;
using gip.mes.datamodel;

namespace gip.mes.manager
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'PresenterProgramLogVB'}de{'PresenterProgramLogVB'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + gip.core.datamodel.ACProgram.ClassName)]
    public class PresenterProgramLogVB : PresenterProgramLog
    {
        public PresenterProgramLogVB(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="") 
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        protected override void CreateProgramLogWrapper(IEnumerable<core.datamodel.ACProgramLog> items)
        {
            foreach (core.datamodel.ACProgramLog acprogramlog in items.OrderBy(c => c.StartDate))
            {
                ProgramLogWrapperVB tempWrapper = new ProgramLogWrapperVB() { ACProgramLog = acprogramlog, DisplayOrder = _DisplayOrder++ };
                wrapperList.Add(tempWrapper);
                if (!_IsFromVBBSOControlPA)
                {
                    CreateProgramLogWrapper(acprogramlog.ACProgramLog_ParentACProgramLog);
                    if (acprogramlog.ParentACProgramLogID != null)
                    {
                        var tempLog = wrapperList.FirstOrDefault(c => c.ACProgramLog.ACProgramLogID == acprogramlog.ParentACProgramLogID);
                        tempLog.Items.Add(tempWrapper);
                        if (tempWrapper.Status.Contains(Global.TimelineItemStatus.Alarm) || tempWrapper.Status.Contains(Global.TimelineItemStatus.ChildAlarm))
                            tempLog.ChildAlarm = true;
                    }
                }
            }
        }

        protected override void CreateProgramLogWrapperSearch(IEnumerable<core.datamodel.ACProgramLog> items)
        {
            if (!_IsFromVBBSOControlPA)
            {
                foreach (core.datamodel.ACProgramLog acprogramlog in items.OrderBy(c => c.StartDate))
                {
                    if (wrapperList.Any(c => c.ACProgramLog == acprogramlog))
                    {
                        ProgramLogWrapperVB tempWrapper = new ProgramLogWrapperVB() { ACProgramLog = acprogramlog, DisplayOrder = _DisplayOrder++ };
                        wrapperListSearch.Add(tempWrapper);
                        CreateProgramLogWrapperSearch(acprogramlog.ACProgramLog_ParentACProgramLog);
                        if (acprogramlog.ParentACProgramLogID != null)
                        {
                            var tempLog = wrapperListSearch.FirstOrDefault(c => c.ACProgramLog.ACProgramLogID == acprogramlog.ParentACProgramLogID);
                            tempLog.Items.Add(tempWrapper);
                            if (tempWrapper.Status.Contains(Global.TimelineItemStatus.Alarm) || tempWrapper.Status.Contains(Global.TimelineItemStatus.ChildAlarm))
                                tempLog.ChildAlarm = true;
                        }
                    }
                }
            }
            else
            {
                foreach (ProgramLogWrapper wrapper in wrapperList)
                {
                    wrapperListSearch.Add(new ProgramLogWrapperVB() { ACProgramLog = wrapper.ACProgramLog, DisplayOrder = _DisplayOrder++ });
                }
            }
        }

        private DatabaseApp _DatabaseApp = null;
        /// <summary>Returns the shared Database-Context for BSO's by calling GetAppContextForBSO()</summary>
        /// <value>Returns the shared Database-Context.</value>
        public virtual DatabaseApp DatabaseApp
        {
            get
            {
                if (_DatabaseApp == null && this.InitState != ACInitState.Destructed && this.InitState != ACInitState.Destructing && this.InitState != ACInitState.DisposedToPool && this.InitState != ACInitState.DisposingToPool)
                    _DatabaseApp = ACObjectContextManager.GetOrCreateContext<DatabaseApp>(this.GetACUrl(), "", new core.datamodel.Database());
                return _DatabaseApp as DatabaseApp;
            }
        }

        public override IACEntityObjectContext Database
        {
            get
            {
                return DatabaseApp;
            }
        }

        [ACMethodInteraction("", "en{'Show Order'}de{'Show Order'}", 901, true, "CurrentProgramLogWrapper")]
        public void ShowOrder()
        {
            //             IEnumerable<ACPropertyLogSumOfProgram> sumOfProgram = gip.core.datamodel.ACPropertyLog.GetSummarizedDurationsOfProgram(database, currentACProgram.ACProgramID, new string[] { GlobalProcApp.AvailabilityStatePropName });

            if (CurrentProgramLogWrapper != null)
            {
                OrderLog currentOrderLog = DatabaseApp.OrderLog.FirstOrDefault(c => c.VBiACProgramLogID == CurrentProgramLogWrapper.ACProgramLog.ACProgramLogID);
                if (currentOrderLog == null)
                {
                    var programLog = CurrentProgramLogWrapper.ACProgramLog.FromAppContext<gip.mes.datamodel.ACProgramLog>(DatabaseApp);
                    while (currentOrderLog == null && programLog.ACProgramLog1_ParentACProgramLog != null)
                    {
                        currentOrderLog = programLog.OrderLog_VBiACProgramLog;
                        programLog = programLog.ACProgramLog1_ParentACProgramLog;
                    }

                    //while (currentOrderLog == null && programLog != null)
                    //{
                    //    if (!programLog.ParentACProgramLogID.HasValue)
                    //        break;
                    //    currentOrderLog = DatabaseApp.OrderLog.FirstOrDefault(c => c.VBiACProgramLogID == programLog.ParentACProgramLogID);
                    //    programLog = Database.ContextIPlus.ACProgramLog.FirstOrDefault(c => c.ACProgramLogID == programLog.ParentACProgramLogID);
                    //}
                }
                if (    currentOrderLog == null
                    || (currentOrderLog.ProdOrderPartslistPos == null && currentOrderLog.ProdOrderPartslistPosRelation == null))
                    return;


                PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
                if (service != null)
                {
                    PAOrderInfo info = new PAOrderInfo();
                    info.Entities.Add(new PAOrderInfoEntry(OrderLog.ClassName, currentOrderLog.VBiACProgramLogID));
                    service.ShowDialogOrder(this, info);
                }
            }
        }

        public bool IsEnabledShowOrder()
        {
            if (_IsFromVBBSOControlPA)
                return true;
            return false;
        }


        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"ShowOrder":
                    ShowOrder();
                    return true;
                case"IsEnabledShowOrder":
                    result = IsEnabledShowOrder();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
