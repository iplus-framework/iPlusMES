// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.datamodel;
using System.Threading;
using System.Collections.Generic;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Workflow-Root für Umlagerung
    /// </summary>
    [ACClassConstructorInfo(
        new object[] 
        { 
            new object[] {gip.core.datamodel.ACProgram.ClassName, Global.ParamOption.Required, typeof(Guid)},
            new object[] {gip.core.datamodel.ACProgramLog.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {PWProcessFunction.C_InvocationCount, Global.ParamOption.Optional, typeof(int)},
            new object[] {FacilityBooking.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {Picking.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {PickingPos.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {core.datamodel.ACClassWF.ClassName, Global.ParamOption.Optional, typeof(Guid)}
        }
    )]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Relocation'}de{'Umlagerung'}", Global.ACKinds.TPWMethod, Global.ACStorableTypes.Optional, true, true, "", "ACProgram", 40)]
    public class PWMethodRelocation : PWMethodTransportBase
    {
        new public const string PWClassName = "PWMethodRelocation";

        #region c´tors
        static PWMethodRelocation()
        {
            RegisterExecuteHandler(typeof(PWMethodRelocation), HandleExecuteACMethod_PWMethodRelocation);
        }

        public PWMethodRelocation(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            if (!await base.ACDeInit(deleteACClassTask))
                return false;
            return true;
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }
        #endregion

        #region Properties

        #endregion

        #region Methods

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWMethodRelocation(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
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

            if (this.ContentTask != null && this.ContentTask.EntityState == Microsoft.EntityFrameworkCore.EntityState.Added)
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
                if (_CurrentPicking != null || _CurrentFacilityBooking != null)
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
                        if (currentPicking != null)
                        {
                            PickingPos currentPickingPos = null;
                            Guid pickingPosID = PWDischarging.GetPickingPosIDFromACMethod(acMethod);
                            if (pickingPosID != Guid.Empty)
                                currentPickingPos = currentPicking.PickingPos_Picking.Where(c => c.PickingPosID == pickingPosID).FirstOrDefault();
                            if (currentPickingPos == null)
                                currentPickingPos = currentPicking.PickingPos_Picking.Where(c => c.MDDelivPosLoadStateID.HasValue 
                                                                && (   c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive
                                                                    || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)).FirstOrDefault();
                            if (currentPickingPos == null)
                                currentPickingPos = currentPicking.PickingPos_Picking.FirstOrDefault();
                            dbApp.Detach(currentPicking);
                            if (currentPickingPos != null)
                                dbApp.Detach(currentPickingPos);

                            using (ACMonitor.Lock(_20015_LockValue))
                            {
                                _CurrentPickingPos = currentPickingPos;
                            }
                        }

                        FacilityBooking currentFacilityBooking = entity as FacilityBooking;
                        if (currentFacilityBooking != null)
                            dbApp.Detach(currentFacilityBooking);

                        using (ACMonitor.Lock(_20015_LockValue))
                        {
                            _CurrentPicking = currentPicking;
                            _CurrentFacilityBooking = currentFacilityBooking;
                        }
                    }
                }
            }
        }

        protected override void ACClassTaskQueue_ChangesSaved(object sender, ACChangesEventArgs e)
        {
            if (CurrentFacilityBooking != null)
            {
                ACClassTaskQueue.TaskQueue.ObjectContext.ACChangesExecuted -= ACClassTaskQueue_ChangesSaved;
                if (_NewAddedProgramLog != null)
                {
                    gip.core.datamodel.ACProgramLog newAddedProgramLog = _NewAddedProgramLog;
                    Guid facilityBookingID = CurrentFacilityBooking.FacilityBookingID;

                    this.ApplicationManager.ApplicationQueue.Add(() =>
                    //ThreadPool.QueueUserWorkItem((object state) =>
                    {
                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            OrderLog orderLog = OrderLog.NewACObject(dbApp, newAddedProgramLog);
                            orderLog.FacilityBookingID = facilityBookingID;
                            dbApp.OrderLog.Add(orderLog);
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

        #region Overrides

        protected override void OnRebuildMandatoryConfigStoresCache(IACComponentPWNode invoker, List<IACConfigStore> mandatoryConfigStores, bool recalcExpectedConfigStoresCount)
        {
            base.OnRebuildMandatoryConfigStoresCache(invoker, mandatoryConfigStores, recalcExpectedConfigStoresCount);

            if (CurrentPicking == null && CurrentFacilityBooking == null && recalcExpectedConfigStoresCount)
            {
                Messages.LogError(this.GetACUrl(), "OnRebuildMandatoryConfigStoresCache(20)", "CurrentPicking is null => ConfigStore-Validation will fail!");
                using (ACMonitor.Lock(_20015_LockStoreList))
                {
                    // Minimum is PickingConfig:
                    _ExpectedConfigStoresCount += 1;
                }
            }
        }

        #endregion

        #endregion
    }
}
