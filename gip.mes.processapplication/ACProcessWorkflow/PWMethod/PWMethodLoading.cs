using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Threading;

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

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {

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
        private gip.mes.datamodel.DeliveryNotePos _CurrentDeliveryNotePos = null;
        public gip.mes.datamodel.DeliveryNotePos CurrentDeliveryNotePos
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_CurrentDeliveryNotePos != null)
                        return _CurrentDeliveryNotePos;
                }
                LoadVBEntities();

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentDeliveryNotePos;
                }
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
                if (_NewAddedProgramLog != null)
                {
                    this.ApplicationManager.ApplicationQueue.Add(() =>
                    //ThreadPool.QueueUserWorkItem((object state) =>
                    {
                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            OrderLog orderLog = OrderLog.NewACObject(dbApp, _NewAddedProgramLog);
                            orderLog.DeliveryNotePosID = CurrentDeliveryNotePos.DeliveryNotePosID;
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
