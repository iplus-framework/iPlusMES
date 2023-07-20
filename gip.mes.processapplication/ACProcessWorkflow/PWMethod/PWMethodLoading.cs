using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Threading;
using gip.mes.facility;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Workflow-Root für Warenausgang
    /// </summary>
    [ACClassConstructorInfo(
    new object[] 
        { 
            new object[] {gip.core.datamodel.ACProgram.ClassName, Global.ParamOption.Required, typeof(Guid)},
            new object[] {gip.core.datamodel.ACProgramLog.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {PWProcessFunction.C_InvocationCount, Global.ParamOption.Optional, typeof(int)},
            new object[] {DeliveryNotePos.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {Picking.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {PickingPos.ClassName, Global.ParamOption.Optional, typeof(Guid)}
        }
    )]

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Loading'}de{'Verladung'}", Global.ACKinds.TPWMethod, Global.ACStorableTypes.Optional, true, true, "", "ACProgram", 30)]
    public class PWMethodLoading : PWMethodTransportBase
    {
        new public const string PWClassName = "PWMethodLoading";

        #region c´tors
        static PWMethodLoading()
        {
            RegisterExecuteHandler(typeof(PWMethodLoading), HandleExecuteACMethod_PWMethodLoading);
        }

        public PWMethodLoading(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _LabOrderManager = ACLabOrderManager.ACRefToServiceInstance(this);
            if (_LabOrderManager == null)
                throw new Exception("LabOrderManager not configured");
            _InDeliveryNoteManager = ACInDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_InDeliveryNoteManager == null)
                throw new Exception("ACInDeliveryNoteManager not configured");

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACLabOrderManager.DetachACRefFromServiceInstance(this, _LabOrderManager);
            _LabOrderManager = null;
            ACInDeliveryNoteManager.DetachACRefFromServiceInstance(this, _InDeliveryNoteManager);
            _InDeliveryNoteManager = null;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _CurrentDeliveryNotePos = null;
            }

            if (!base.ACDeInit(deleteACClassTask))
                return false;

            return true;
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _CurrentDeliveryNotePos = null;
            }

            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }
        #endregion

        #region Properties
        protected ACRef<ACLabOrderManager> _LabOrderManager = null;
        public ACLabOrderManager LabOrderManager
        {
            get
            {
                if (_LabOrderManager == null)
                    return null;
                return _LabOrderManager.ValueT;
            }
        }

        protected ACRef<ACInDeliveryNoteManager> _InDeliveryNoteManager = null;
        public ACInDeliveryNoteManager InDeliveryNoteManager
        {
            get
            {
                if (_InDeliveryNoteManager == null)
                    return null;
                return _InDeliveryNoteManager.ValueT as ACInDeliveryNoteManager;
            }
        }
        #endregion

        #region Methods

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWMethodLoading(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWMethodTransportBase(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region Planning and Testing
        protected override TimeSpan GetPlannedDuration()
        {
            return base.GetPlannedDuration();
        }

        protected override DateTime GetPlannedStartTime()
        {
            return base.GetPlannedStartTime();
        }
        #endregion

        #region Order

        protected override void LoadVBEntities()
        {
            var rootPW = RootPW;
            if (rootPW == null)
                return;

            if (this.ContentTask.EntityState == System.Data.EntityState.Added)
            {
                Messages.LogError(this.GetACUrl(), "LoadVBEntities(10)", "EntityState of ContentTask is Added and not saved to the database. The call of LoadVBEntities is too early!");
                Messages.LogError(this.GetACUrl(), "LoadVBEntities(11)", System.Environment.StackTrace);
                return;
            }
            if (IsStartingProcessFunction)
            {
                Messages.LogError(this.GetACUrl(), "LoadVBEntities(20)", "IsStartingProcessFunction is true. The call of LoadVBEntities is too early!");
                Messages.LogError(this.GetACUrl(), "LoadVBEntities(21)", System.Environment.StackTrace);
                return;
            }

            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (_CurrentPicking != null || _CurrentDeliveryNotePos != null)
                    return;
            }
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                ACMethod acMethod = CurrentACMethod.ValueT;
                if (acMethod != null)
                {
                    var entity = PWDischarging.GetTransportEntityFromACMethod(dbApp, acMethod);
                    if (entity != null)
                    {
                        Picking currentPicking = entity as Picking;
                        PickingPos currentPickingPos = null;
                        if (currentPicking != null)
                        {
                            Guid pickingPosID = PWDischarging.GetPickingPosIDFromACMethod(acMethod);
                            if (pickingPosID != Guid.Empty)
                                currentPickingPos = currentPicking.PickingPos_Picking.Where(c => c.PickingPosID == pickingPosID).FirstOrDefault();
                            if (currentPickingPos == null)
                                currentPickingPos = currentPicking.PickingPos_Picking.Where(c => c.MDDelivPosLoadStateID.HasValue
                                                                && (c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive
                                                                    || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)).FirstOrDefault();
                            if (currentPickingPos == null)
                                currentPickingPos = currentPicking.PickingPos_Picking.FirstOrDefault();
                            dbApp.Detach(currentPicking);
                            if (currentPickingPos != null)
                                dbApp.Detach(currentPickingPos);
                        }

                        DeliveryNotePos currentDeliveryNotePos = entity as DeliveryNotePos;
                        if (currentDeliveryNotePos != null)
                            dbApp.Detach(currentDeliveryNotePos);


                        using (ACMonitor.Lock(_20015_LockValue))
                        {
                            _CurrentPicking = currentPicking;
                            _CurrentPickingPos = currentPickingPos;
                            _CurrentDeliveryNotePos = currentDeliveryNotePos;
                        }
                    }
                }
            }
        }

        protected override void ACClassTaskQueue_ChangesSaved(object sender, ACChangesEventArgs e)
        {
            if (CurrentDeliveryNotePos != null)
            {
                ACClassTaskQueue.TaskQueue.ObjectContext.ACChangesExecuted -= ACClassTaskQueue_ChangesSaved;
                gip.core.datamodel.ACProgramLog newAddedProgramLog = _NewAddedProgramLog;
                Guid deliveryNotePosID = CurrentDeliveryNotePos.DeliveryNotePosID;
                if (newAddedProgramLog != null)
                {
                    this.ApplicationManager.ApplicationQueue.Add(() =>
                    //ThreadPool.QueueUserWorkItem((object state) =>
                    {
                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            OrderLog orderLog = OrderLog.NewACObject(dbApp, newAddedProgramLog);
                            orderLog.DeliveryNotePosID = deliveryNotePosID;
                            dbApp.OrderLog.AddObject(orderLog);
                            dbApp.ACSaveChanges();
                        }
                        _NewAddedProgramLog = null;
                    });
                }
                else
                    _NewAddedProgramLog = null;
            }
            else
            {
                base.ACClassTaskQueue_ChangesSaved(sender, e);
            }
        }
        #endregion

        #endregion

    }
}
