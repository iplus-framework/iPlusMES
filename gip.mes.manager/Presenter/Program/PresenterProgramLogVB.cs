using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.manager;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Net;

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

        [ACMethodInteraction("", "en{'Show Order'}de{'Show Order'}", 781, true, "CurrentProgramLogWrapper")]
        public void ShowOrder()
        {
            if (CurrentProgramLogWrapper != null)
            {
                OrderLog orderLog = DatabaseApp.OrderLog.FirstOrDefault(c => c.VBiACProgramLogID == CurrentProgramLogWrapper.ACProgramLog.ACProgramLogID);
                if (orderLog == null)
                {
                    var programLog = CurrentProgramLogWrapper.ACProgramLog.FromAppContext<gip.mes.datamodel.ACProgramLog>(DatabaseApp);
                    while (orderLog == null && programLog.ACProgramLog1_ParentACProgramLog != null)
                    {
                        orderLog = programLog.OrderLog;
                        programLog = programLog.ACProgramLog1_ParentACProgramLog;
                    }
                }

                if (orderLog == null)
                    return;

                PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
                if (service != null)
                {
                    PAOrderInfo info = new PAOrderInfo();
                    if (orderLog.ProdOrderPartslistPosID.HasValue)
                    {
                        info.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = orderLog.ProdOrderPartslistPosID.Value,
                            EntityName = ProdOrderPartslistPos.ClassName
                        });
                    }
                    if (orderLog.ProdOrderPartslistPosRelationID.HasValue)
                    {
                        info.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = orderLog.ProdOrderPartslistPosRelationID.Value,
                            EntityName = ProdOrderPartslistPosRelation.ClassName
                        });
                    }
                    if (orderLog.PickingPosID.HasValue)
                    {
                        info.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = orderLog.PickingPosID.Value,
                            EntityName = PickingPos.ClassName
                        });
                    }
                    if (orderLog.FacilityBookingID.HasValue)
                    {
                        info.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = orderLog.FacilityBookingID.Value,
                            EntityName = FacilityBooking.ClassName
                        });
                    }
                    if (orderLog.DeliveryNotePosID.HasValue)
                    {
                        info.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = orderLog.DeliveryNotePosID.Value,
                            EntityName = DeliveryNotePos.ClassName
                        });
                    }
                    if (!info.Entities.Any())
                        info.Entities.Add(new PAOrderInfoEntry(OrderLog.ClassName, orderLog.VBiACProgramLogID));
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
                case nameof(ShowOrder):
                    ShowOrder();
                    return true;
                case nameof(IsEnabledShowOrder):
                    result = IsEnabledShowOrder();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
