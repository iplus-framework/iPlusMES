using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Components list'}de{'Komponentenliste'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + ProdOrderPartslistPosRelation.ClassName)]
    public class BSOProdOrderBatchComponents : ACBSOvb
    {

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOProdOrder"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOProdOrderBatchComponents(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
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


            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            bool baseResult = base.ACDeInit(deleteACClassTask);
            return baseResult;
        }

        #endregion


        #region Component

        #region Component
        private ProdOrderPartslistPos _SelectedComponent;
        /// <summary>
        /// Selected property for ProdOrderPartslistPosRelation
        /// </summary>
        /// <value>The selected Component</value>
        [ACPropertySelected(9999, "Component", "en{'TODO: Component'}de{'TODO: Component'}")]
        public ProdOrderPartslistPos SelectedComponent
        {
            get
            {
                return _SelectedComponent;
            }
            set
            {
                if (_SelectedComponent != value)
                {
                    _SelectedComponent = value;
                    OnPropertyChanged("SelectedComponent");
                }
            }
        }


        private List<ProdOrderPartslistPos> _ComponentList;
        /// <summary>
        /// List property for ProdOrderPartslistPosRelation
        /// </summary>
        /// <value>The Component list</value>
        [ACPropertyList(9999, "Component")]
        public List<ProdOrderPartslistPos> ComponentList
        {
            get
            {
                if (_ComponentList == null)
                    _ComponentList = new List<ProdOrderPartslistPos>();
                return _ComponentList;
            }
        }

        private List<ProdOrderPartslistPos> LoadComponentList(Guid prodOrderBatchPlanID)
        {
            List<ProdOrderPartslistPos> components = new List<ProdOrderPartslistPos>();
            // detect batch
            ProdOrderBatchPlan batchPlan = DatabaseApp.ProdOrderBatchPlan.FirstOrDefault(c => c.ProdOrderBatchPlanID == prodOrderBatchPlanID);

            // get list
            components =
                DatabaseApp 
                .ProdOrderPartslistPos
               .Include(x => x.Material)
               .Include(x => x.Material.BaseMDUnit)
               .Include(x => x.MDUnit)
               .Where(x =>
                    x.ProdOrderPartslistID == batchPlan.ProdOrderPartslistID &&
                    x.AlternativeProdOrderPartslistPosID == null &&
                    x.MaterialPosTypeIndex == (short)(GlobalApp.MaterialPosTypes.OutwardRoot) &&
                    x.ParentProdOrderPartslistPosID == null &&
                    x.AlternativeProdOrderPartslistPosID == null)
                .OrderBy(x => x.Sequence)
                .ToList();

            double quantityIndex = (batchPlan.BatchSize * batchPlan.BatchTargetCount) / batchPlan.ProdOrderPartslistPos.TargetQuantityUOM;
            foreach(ProdOrderPartslistPos pos in components)
            {
                pos.TargetQuantityUOM *= quantityIndex;
                DatabaseApp.ObjectStateManager.ChangeObjectState(pos, System.Data.EntityState.Unchanged);
            }      

            return components;
        }
        #endregion

        [ACMethodInfo("Dialog", "en{'Dialog Lot'}de{'Dialog Los'}", (short)MISort.QueryPrintDlg)]
        public void ShowDialogComponent(PAOrderInfo paOrderInfo)
        {
            PAOrderInfoEntry entry = paOrderInfo.Entities.FirstOrDefault(c => c.EntityName == nameof(ProdOrderBatchPlan));
            if (entry != null)
            {
                _ComponentList = LoadComponentList(entry.EntityID);
                OnPropertyChanged(nameof(ComponentList));
                ShowDialog(this, "DialogComponent");
                this.ParentACComponent.StopComponent(this);
            }
        }


        #endregion
    }
}
