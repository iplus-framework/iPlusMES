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
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Production materials requirement'}de{'Produktionsmaterialbedarf'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true)]
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACMatReqManager.DetachACRefFromServiceInstance(this, _MatRequirementManager);
            _MatRequirementManager = null;
            _SelectedMatReqResult = null;
            _SelectedProdOrderBatchPlan = null;
            _MatReqResultList = null;
            var b = base.ACDeInit(deleteACClassTask);
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
        [ACPropertySelected(500,"BatchPlan")]
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

        [ACPropertyList(501, "BatchPlan")]
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
        [ACPropertySelected(502,"MatReqResult")]
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
        [ACPropertyList(503, "MatReqResult")]
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

        [ACMethodInfo("", "en{'Calculate materials requirement'}de{'Materialbedarf berechnen'}", 501)]
        public void CalculateMaterialsRequirement()
        {
            OnPropertyChanged("ProdOrderBatchPlanList");
            MatReqResultList = MatRequirementManager.CalculateMaterialsRequirement(ProdOrderBatchPlanList);
        }

        public bool IsEnabledCalculateMaterialsRequirement()
        {
            return true;
        }

        [ACMethodInfo("", "en{'Check materials requirement'}de{'Materialbedarf prüfen'}", 501)]
        public void CheckMaterialRequirement()
        {
            MsgWithDetails resultMsg = null;

            var result = MatRequirementManager.CheckMaterialsRequirement(this.DatabaseApp, SelectedProdOrderBatchPlan.ProdOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist);
            if(result != null && result.Any())
                resultMsg = new MsgWithDetails(result.ToArray());

            if (resultMsg != null)
                Messages.Msg(resultMsg);
            else
                Messages.Msg(new Msg(eMsgLevel.Info, "OK"));
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

        #endregion
    }
}
