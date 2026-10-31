using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Business object or App for calculating and checking the material requirements of production batch plans.
    /// This is a calculation app without a Current entity: the ProdOrderBatchPlanList property contains all batch
    /// plans in the states Created, AutoStart or InProcess and the selected plan is in the SelectedProdOrderBatchPlan
    /// property.
    /// CalculateMaterialsRequirement() recalculates the material requirement over all active batch plans via the
    /// ACMatReqManager service and copies the result into the MatReqResultList property; the selected result row is
    /// in the SelectedMatReqResult property. Each result contains the material, the required, available, actual,
    /// remaining and difference quantities.
    /// CheckMaterialRequirement() checks the material requirement of the selected batch plan only and shows the
    /// result as a message (or "OK" if no shortage was found); it is only enabled when a batch plan is selected.
    /// This BSO is read-only regarding the database; there are no New/Save/Delete entity workflows.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOProdMatRequirement&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Production materials requirement'}de{'Produktionsmaterialbedarf'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true,
        Description = @"Business object or App for calculating and checking the material requirements of production batch plans.
        This is a calculation app without a Current entity: the ProdOrderBatchPlanList property contains all batch plans in the states Created, AutoStart or InProcess and the selected plan is in the SelectedProdOrderBatchPlan property.
        CalculateMaterialsRequirement() recalculates the material requirement over all active batch plans via the ACMatReqManager service and copies the result into the MatReqResultList property; the selected result row is in the SelectedMatReqResult property. Each result contains the material, the required, available, actual, remaining and difference quantities.
        CheckMaterialRequirement() checks the material requirement of the selected batch plan only and shows the result as a message (or ""OK"" if no shortage was found); it is only enabled when a batch plan is selected.
        This BSO is read-only regarding the database; there are no New/Save/Delete entity workflows.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOProdMatRequirement&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOProdMatRequirement : ACBSOvb
    {
        #region c'tors

        public BSOProdMatRequirement(gip.core.datamodel.ACClass typeACClass, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(typeACClass, content, parentACObject, parameter)
        {
        }

        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _MatRequirementManager = ACMatReqManager.ACRefToServiceInstance(this);
            if (_MatRequirementManager == null)
                throw new Exception("MatRequirementManager not configured");

            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            ACMatReqManager.DetachACRefFromServiceInstance(this, _MatRequirementManager);
            _MatRequirementManager = null;
            _SelectedMatReqResult = null;
            _SelectedProdOrderBatchPlan = null;
            _MatReqResultList = null;
            var b = await base.ACDeInit(deleteACClassTask);
            return b;
        }

        #endregion

        #region Managers

        protected ACRef<ACMatReqManager> _MatRequirementManager = null;
        public ACMatReqManager MatRequirementManager
        {
            get
            {
                if (_MatRequirementManager == null)
                    return null;
                return _MatRequirementManager.ValueT;
            }
        }

        #endregion

        #region BSO -> ACProperty

        private ProdOrderBatchPlan _SelectedProdOrderBatchPlan;
        [ACPropertySelected(500,"BatchPlan", Description = @"Currently selected batch plan; target of CheckMaterialRequirement().")]
        public ProdOrderBatchPlan SelectedProdOrderBatchPlan
        {
            get
            {
                return _SelectedProdOrderBatchPlan;
            }
            set
            {
                _SelectedProdOrderBatchPlan = value;
            }
        }

        [ACPropertyList(501, "BatchPlan", Description = @"All batch plans in the states Created, AutoStart or InProcess.")]
        public IEnumerable<ProdOrderBatchPlan> ProdOrderBatchPlanList
        {
            get
            {
                return DatabaseApp.ProdOrderBatchPlan.Where(c => c.PlanStateIndex == (short)GlobalApp.BatchPlanState.AutoStart || 
                                                                 c.PlanStateIndex == (short)GlobalApp.BatchPlanState.InProcess ||
                                                                 c.PlanStateIndex == (short)GlobalApp.BatchPlanState.Created).ToList();
            }
        }

        private MatReqResult _SelectedMatReqResult;
        [ACPropertySelected(502,"MatReqResult", Description = @"Currently selected material requirement result row in the result list.")]
        public MatReqResult SelectedMatReqResult
        {
            get
            {
                return _SelectedMatReqResult;
            }
            set
            {
                _SelectedMatReqResult = value;
                OnPropertyChanged("SelectedMatReqResult");
            }
        }

        private IEnumerable<MatReqResult> _MatReqResultList;
        [ACPropertyList(503, "MatReqResult", Description = @"Material requirement results of the last calculation (CalculateMaterialsRequirement): material, required, available, actual, remaining and difference quantities.")]
        public IEnumerable<MatReqResult> MatReqResultList
        {
            get
            {
                return _MatReqResultList;
            }
            set
            {
                _MatReqResultList = value;
                OnPropertyChanged("MatReqResultList");
            }
        }

        #endregion

        #region BSO -> ACMethod

        [ACMethodInfo("", "en{'Calculate materials requirement'}de{'Materialbedarf berechnen'}", 501, Description = @"Recalculates the material requirement over all active batch plans via the ACMatReqManager service and copies the result into the MatReqResultList property.")]
        public void CalculateMaterialsRequirement()
        {
            OnPropertyChanged("ProdOrderBatchPlanList");
            MatReqResultList = MatRequirementManager.CheckMaterialsRequirementOverAllOrders(DatabaseApp, ProdOrderBatchPlanList);
            //MatReqResultList = MatRequirementManager.CalculateMaterialsRequirement(ProdOrderBatchPlanList);
        }

        public bool IsEnabledCalculateMaterialsRequirement()
        {
            return true;
        }

        [ACMethodInfo("", "en{'Check materials requirement'}de{'Materialbedarf prüfen'}", 501, Description = @"Checks the material requirement of the selected batch plan only and shows the result as a message (or ""OK"" if no shortage was found). Only enabled when a batch plan is selected.")]
        public void CheckMaterialRequirement()
        {
            MsgWithDetails resultMsg = null;

            var result = MatRequirementManager.CheckMaterialsRequirement(this.DatabaseApp, SelectedProdOrderBatchPlan.ProdOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist);
            if(result != null && result.Any())
                resultMsg = new MsgWithDetails(result.ToArray());

            if (resultMsg != null)
                Messages.MsgAsync(resultMsg);
            else
                Messages.MsgAsync(new Msg(eMsgLevel.Info, "OK"));
        }

        public bool IsEnabledCheckMaterialRequirement()
        {
            return SelectedProdOrderBatchPlan != null;
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "CalculateMaterialsRequirement":
                    CalculateMaterialsRequirement();
                    return true;
                case "IsEnabledCalculateMaterialsRequirement":
                    result = IsEnabledCalculateMaterialsRequirement();
                    return true;
                case "CheckMaterialRequirement":
                    CheckMaterialRequirement();
                    return true;
                case "IsEnabledCheckMaterialRequirement":
                    result = IsEnabledCheckMaterialRequirement();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                #region Always Enabled
                case nameof(CalculateMaterialsRequirement):
                case nameof(IsEnabledCalculateMaterialsRequirement):
                    return new string[] { nameof(InitState) };
                #endregion

                #region Check Material Requirement
                case nameof(CheckMaterialRequirement):
                case nameof(IsEnabledCheckMaterialRequirement):
                    return new string[] { nameof(SelectedProdOrderBatchPlan) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion
    }
}
