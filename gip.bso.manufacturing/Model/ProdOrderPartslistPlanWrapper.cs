// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
﻿using System.Runtime.CompilerServices;
using System.Text;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Order BOM Plan'}de{'Auftrag Stückliste Plan'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class ProdOrderPartslistPlanWrapper : INotifyPropertyChanged
    {
        #region event

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        [NotMapped]
        private bool _IsSelected;
        [ACPropertyInfo(999, "IsSelected", "en{'Select'}de{'Auswahl'}")]
        [NotMapped]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

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
                return Material != null ? Material.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).FirstOrDefault() : null;
            }
        }

        [ACPropertyInfo(3, "", "en{'Base UOM'}de{'Basiseinheit'}")]
        public MDUnit MDUnit
        {
            get
            {
                if (Material == null)
                    return null;
                return Material.BaseMDUnit;
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

        private double? _TargetQuantity;
        [ACPropertyInfo(9, "", "en{'Target Quantity)'}de{'Sollmenge'}")]
        public double TargetQuantity
        {
            get
            {
                if (_TargetQuantity == null)
                {
                    if (Material == null || AltMDUnit == Material.BaseMDUnit)
                        _TargetQuantity = TargetQuantityUOM;
                    else
                        _TargetQuantity = Material.ConvertQuantity(TargetQuantityUOM, Material.BaseMDUnit, AltMDUnit);
                }
                return _TargetQuantity ?? 0;
            }
        }

        private double? _ActualQuantity;
        [ACPropertyInfo(10, "", "en{'Actual Quantity'}de{'Ist-Menge'}")]
        public double ActualQuantity
        {
            get
            {
                if (_ActualQuantity == null)
                {
                    if (Material == null || AltMDUnit == Material.BaseMDUnit)
                        _ActualQuantity = ActualQuantityUOM;
                    else
                        _ActualQuantity = Material.ConvertQuantity(ActualQuantityUOM, Material.BaseMDUnit, AltMDUnit);
                }
                return _ActualQuantity ?? 0;
            }
        }

        private double? _PlannedQuantity;
        [ACPropertyInfo(11, "", "en{'Planned Quantity'}de{'Verplante Menge'}")]
        public double PlannedQuantity
        {
            get
            {
                if (_PlannedQuantity == null)
                {
                    if (Material == null || AltMDUnit == Material.BaseMDUnit)
                        _PlannedQuantity = PlannedQuantityUOM;
                    else
                        _PlannedQuantity = Material.ConvertQuantity(PlannedQuantityUOM, Material.BaseMDUnit, AltMDUnit);
                }
                return _PlannedQuantity ?? 0;
            }
        }

        private double? _UnPlannedQuantity;
        [ACPropertyInfo(12, "", "en{'Open Planning Quantity'}de{'Offene Planmenge'}")]
        public double UnPlannedQuantity
        {
            get
            {
                if (_UnPlannedQuantity == null)
                {
                    if (Material == null || AltMDUnit == Material.BaseMDUnit)
                        _UnPlannedQuantity = UnPlannedQuantityUOM;
                    else
                        return Material.ConvertQuantity(UnPlannedQuantityUOM, Material.BaseMDUnit, AltMDUnit);
                }
                return _UnPlannedQuantity ?? 0;
            }
        }

        private PlanningStateEnum? _PlanningState;
        public PlanningStateEnum PlanningState
        {
            get
            {
                if (_PlanningState == null)
                {
                    if ((ActualQuantityUOM + 1) >= TargetQuantityUOM && TargetQuantityUOM > 1)
                        _PlanningState = PlanningStateEnum.TargetQuantityReached;
                    else if (UnPlannedQuantityUOM <= 1)
                        _PlanningState = PlanningStateEnum.Planned;
                    else if (UnPlannedQuantityUOM > 1 && PlannedQuantityUOM > 1)
                        _PlanningState = PlanningStateEnum.PartiallyPlanned;
                    return PlanningStateEnum.UnPlanned;
                }
                return _PlanningState ?? PlanningStateEnum.UnPlanned;
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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


