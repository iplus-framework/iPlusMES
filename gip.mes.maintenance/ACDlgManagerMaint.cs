// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.maintenance
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'DlgManagerMaint'}de{'Wartungs-Dialogmanager'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class ACDlgManagerMaint : ACComponent
    {
        static ACDlgManagerMaint()
        {
            RegisterExecuteHandler(typeof(ACDlgManagerMaint), HandleExecuteACMethod_ACDlgManagerMaint);
            RegisterExecuteHandlerAsync(typeof(ACDlgManagerMaint), HandleExecuteACMethodAsync_ACDlgManagerMaint);
        }

        public ACDlgManagerMaint(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            return await base.ACDeInit(deleteACClassTask);
        }

        #region Properties

        #endregion

        #region Methods
        #region Execute-Helper-Handlers
        //protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        //{
        //    result = null;
        //    switch (acMethodName)
        //    {
        //    }
        //    return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        //}

        public static bool HandleExecuteACMethod_ACDlgManagerMaint(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(ShowMaintenanceOrder):
                    result = ShowMaintenanceOrder(acComponent);
                    return true;
                case nameof(ShowMaintenanceOrderHistory):
                    result = ShowMaintenanceOrderHistory(acComponent);
                    return true;
                case nameof(GenerateNewMaintenanceOrder):
                    result = GenerateNewMaintenanceOrder(acComponent);
                    return true;
                case nameof(IsEnabledShowMaintenanceOrder):
                    result = IsEnabledShowMaintenanceOrder(acComponent);
                    return true;
                case nameof(IsEnabledShowMaintenanceOrderHistory):
                    result = IsEnabledShowMaintenanceOrderHistory(acComponent);
                    return true;
                case nameof(IsEnabledGenerateNewMaintenanceOrder):
                    result = IsEnabledGenerateNewMaintenanceOrder(acComponent);
                    return true;
            }
            return false;
        }

        public static async Task<object> HandleExecuteACMethodAsync_ACDlgManagerMaint(IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            switch (acMethodName)
            {
                case nameof(ShowMaintenanceOrder):
                    await ShowMaintenanceOrder(acComponent);
                    return true;
                case nameof(ShowMaintenanceOrderHistory):
                    await ShowMaintenanceOrderHistory(acComponent);
                    return true;
                case nameof(GenerateNewMaintenanceOrder):
                    await GenerateNewMaintenanceOrder(acComponent);
                    return true;
            }
            return null;
        }

        private static async Task AwaitIfTask(object result)
        {
            if (result is Task task)
                await task;
        }
        #endregion


        [ACMethodInfo("","",999)]
        public bool ShowMaintenaceDialog(IACComponent caller,  List<ACMaintWarning> warningComponentsList)
        {
            bool returnValue = true;
            if (caller == null)
                return returnValue;
            string bsoName = BSOMaintOrder.ClassName;
            ACComponent childBSO = caller.Root.Businessobjects.ACUrlCommand("?" + bsoName) as ACComponent;
            if (childBSO == null)
                childBSO = caller.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return returnValue;
            if (!(bool)childBSO.ExecuteMethod(nameof(BSOMaintOrder.ShowMaintenanceWarning), warningComponentsList))
                return returnValue = false;
            childBSO.Stop();
            return returnValue;
        }

        [ACMethodAttached("", "en{'View maintenance order'}de{'Wartungsauftrag anschauen'}", 450, typeof(PAClassAlarmingBase), true, "", false, Global.ContextMenuCategory.ProdPlanLog)]
        public static async Task ShowMaintenanceOrder(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            if (!IsEnabledShowMaintenanceOrder(_this))
                return;
            string bsoName = BSOMaintOrder.ClassName;
            ACComponent childBSO = _this.Root.Businessobjects.ACUrlCommand("?" + bsoName, null, new object[] { }) as ACComponent;
            if (childBSO == null)
                childBSO = _this.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            await AwaitIfTask(childBSO.ACUrlCommand(ACUrlHelper.CallAsync + nameof(BSOMaintOrder.ShowMaintenance), _this));
            await childBSO.Stop();
            return;
        }

        public static bool IsEnabledShowMaintenanceOrder(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            if (_this == null)
                return false;

            ACComponent appManager = _this.FindParentComponent<ACComponent>(c => c is ApplicationManager || c is ApplicationManagerProxy);
            if (appManager == null)
                return false;

            var maintService = appManager.ACUrlCommand("ACMaintService") as ACComponent;
            if (maintService == null)
                return false;

            if (_this.Database is mes.datamodel.DatabaseApp)
            {
                Guid componentClassID = _this.ComponentClass.ACClassID;
                DatabaseApp dbApp = _this.Database as DatabaseApp;

                using (ACMonitor.Lock(dbApp.QueryLock_1X000))
                {
                    if (dbApp.MaintOrder.Any(c => c.VBiPAACClassID == componentClassID
                       && c.MDMaintOrderState.MDMaintOrderStateIndex != (short)mes.datamodel.MDMaintOrderState.MaintOrderStates.MaintenanceCompleted))
                        return true;
                }
            }
            return false;
        }

        [ACMethodAttached("", "en{'View maintenance history'}de{'Wartungshistorie anschauen'}", 451, typeof(PAClassAlarmingBase), true, "", false, Global.ContextMenuCategory.ProdPlanLog)]
        public static async Task ShowMaintenanceOrderHistory(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            if (!IsEnabledShowMaintenanceOrderHistory(_this))
                return;
            string bsoName = BSOMaintOrder.ClassName;
            ACComponent childBSO = _this.Root.Businessobjects.ACUrlCommand("?" + bsoName, null, new object[] { }) as ACComponent;
            if (childBSO == null)
                childBSO = _this.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            await AwaitIfTask(childBSO.ACUrlCommand(ACUrlHelper.CallAsync + nameof(BSOMaintOrder.ShowMaintenanceHistory), _this));
            await childBSO.Stop();
            return;
        }

        public static bool IsEnabledShowMaintenanceOrderHistory(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            if (_this == null)
                return false;

            ACComponent appManager = _this.FindParentComponent<ACComponent>(c => c is ApplicationManager || c is ApplicationManagerProxy);
            if (appManager == null)
                return false;

            var maintService = appManager.ACUrlCommand(nameof(ACMaintService)) as ACComponent;
            if (maintService == null)
                return false;

            if (_this.Database is DatabaseApp)
            {
                Guid componentClassID = _this.ComponentClass.ACClassID;
                DatabaseApp dbApp = _this.Database as DatabaseApp;

                using (ACMonitor.Lock(dbApp.QueryLock_1X000))
                {
                    if (dbApp.MaintOrder.Any(c => c.VBiPAACClassID == componentClassID
                   && c.MDMaintOrderState.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceCompleted))
                        return true;
                }
            }
            return false;
        }

        [ACMethodAttached("", "en{'Generate new maintenance order'}de{'Neuen Wartungsauftrag anlegen'}", 550, typeof(PAClassAlarmingBase), true, "", false, Global.ContextMenuCategory.ProdPlanLog)]
        public static async Task GenerateNewMaintenanceOrder(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            if (_this == null)
                return;
            //if (_this.Database is DatabaseApp)
            //{
                ACComponent appManager = _this.FindParentComponent<ACComponent>(c => c is ApplicationManager || c is ApplicationManagerProxy);
                if (appManager == null)
                    return;

                var maintService = appManager.ACUrlCommand("ACMaintService") as ACComponent;
                if (maintService != null)
                    await AwaitIfTask(maintService.ACUrlCommand(ACUrlHelper.CallAsync + nameof(ACMaintService.SetNewMaintOrderManual), _this.GetACUrl()));
        }

        public static bool IsEnabledGenerateNewMaintenanceOrder(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            if (_this == null)
                return false;

            ACComponent appManager = _this.FindParentComponent<ACComponent>(c => c is ApplicationManager || c is ApplicationManagerProxy);
            if (appManager == null)
                return false;

            var maintService = appManager.ACUrlCommand("ACMaintService") as ACComponent;
            if (maintService == null)
                return false;

            if (IsEnabledShowMaintenanceOrder(acComponent))
                return false;
            return true;
        }

#endregion

    }
}

