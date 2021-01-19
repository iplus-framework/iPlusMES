using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Order BOM Plan'}de{'Auftrag Stückliste Plan'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class ProdOrderPartslistPlanWrapper
    {
        [ACPropertyInfo(1, "", ConstApp.ProdOrderPartslist)]
        public ProdOrderPartslist ProdOrderPartslist
        {
            get;
            set;
        }

        [ACPropertyInfo(2, "", ConstApp.Material)]
        public Material Material
        {
            get
            {
                if (ProdOrderPartslist == null
                    || ProdOrderPartslist.Partslist == null
                    || ProdOrderPartslist.Partslist.Material == null)
                {
                    return null;
                }
                return ProdOrderPartslist.Partslist.Material;
            }
        }

        public MaterialUnit AltMaterialUnit
        {
            get
            {
                return Material != null ? Material.MaterialUnit_Material.FirstOrDefault() : null;
            }
        }

        [ACPropertyInfo(3, "", "en{'Base UOM'}de{'Basiseinheit'}")]
        public MDUnit MDUnit
        {
            get
            {
                if (Material == null)
                    return null;
                return AltMaterialUnit != null ? AltMaterialUnit.ToMDUnit : Material.BaseMDUnit;
            }
        }

        [ACPropertyInfo(4, "", "en{'Alt. Unit of Measure'}de{'Alt. Einheit'}")]
        public MDUnit AltMDUnit
        {
            get
            {
                if (Material == null)
                    return null;
                return AltMaterialUnit != null ? AltMaterialUnit.ToMDUnit : Material.BaseMDUnit;
            }
        }

        [ACPropertyInfo(5, "", "en{'Target Quantity (UOM)'}de{'Sollmenge (BME)'}")]
        public double TargetQuantityUOM
        {
            get
            {
                return ProdOrderPartslist.TargetQuantity;
            }
        }

        [ACPropertyInfo(6, "", "en{'Actual Quantity (UOM)'}de{'Ist-Menge (BME)'}")]
        public double ActualQuantityUOM
        {
            get
            {
                return ProdOrderPartslist.ActualQuantity;
            }
        }

        [ACPropertyInfo(7, "", "en{'Planned Quantity (UOM)'}de{'Verplante Menge (BME)'}")]
        public double PlannedQuantityUOM
        {
            get;
            set;
        }

        [ACPropertyInfo(8, "", "en{'Open Planning Quantity (UOM)'}de{'Offene Planmenge (BME)'}")]
        public double UnPlannedQuantityUOM
        {
            get
            {
                return ProdOrderPartslist.TargetQuantity - PlannedQuantityUOM;
            }
        }


        [ACPropertyInfo(9, "", "en{'Target Quantity)'}de{'Sollmenge'}")]
        public double TargetQuantity
        {
            get
            {
                if (Material == null || AltMDUnit == Material.BaseMDUnit)
                    return TargetQuantityUOM;
                return Material.ConvertQuantity(TargetQuantityUOM, Material.BaseMDUnit, AltMDUnit);
            }
        }

        [ACPropertyInfo(10, "", "en{'Actual Quantity'}de{'Ist-Menge'}")]
        public double ActualQuantity
        {
            get
            {
                if (Material == null || AltMDUnit == Material.BaseMDUnit)
                    return ActualQuantityUOM;
                return Material.ConvertQuantity(ActualQuantityUOM, Material.BaseMDUnit, AltMDUnit);
            }
        }

        [ACPropertyInfo(11, "", "en{'Planned Quantity'}de{'Verplante Menge'}")]
        public double PlannedQuantity
        {
            get
            {
                if (Material == null || AltMDUnit == Material.BaseMDUnit)
                    return PlannedQuantityUOM;
                return Material.ConvertQuantity(PlannedQuantityUOM, Material.BaseMDUnit, AltMDUnit);
            }
        }

        [ACPropertyInfo(12, "", "en{'Open Planning Quantity'}de{'Offene Planmenge'}")]
        public double UnPlannedQuantity
        {
            get
            {
                if (Material == null || AltMDUnit == Material.BaseMDUnit)
                    return UnPlannedQuantityUOM;
                return Material.ConvertQuantity(UnPlannedQuantityUOM, Material.BaseMDUnit, AltMDUnit);
            }
        }

        public PlanningStateEnum PlanningState
        {
            get
            {
                if ((ActualQuantityUOM + 0.001) >= TargetQuantityUOM && TargetQuantityUOM > 0.001)
                    return PlanningStateEnum.TargetQuantityReached;
                else if (UnPlannedQuantityUOM <= 0.001)
                    return PlanningStateEnum.Planned;
                else if (UnPlannedQuantityUOM > 0.001 && PlannedQuantityUOM > 0.001)
                    return PlanningStateEnum.PartiallyPlanned;
                return PlanningStateEnum.UnPlanned;
            }
        }

        public enum PlanningStateEnum
        {
            UnPlanned = 0,
            PartiallyPlanned = 1,
            Planned = 2,
            TargetQuantityReached = 3
        }
    }
}


