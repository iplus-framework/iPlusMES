using gip.core.datamodel;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    // MaterialUnit (Artikel)
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Material Unit'}de{'Materialeinheit'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "ToMDUnit", "en{'To Unit'}de{'Nach Einheit'}", Const.ContextDatabase + "\\" + MDUnit.ClassName, "", true)]
    [ACPropertyEntity(2, "Multiplier", "en{'Multiplier(From)'}de{'Multiplikator(Von)'}", "", "", true, DefaultValue = 1)]
    [ACPropertyEntity(3, "Divisor", "en{'Divisor(To)'}de{'Teiler(Nach)'}", "", "", true, DefaultValue = 1)]
    [ACPropertyEntity(4, "NetWeight", "en{'Net Weight'}de{'Nettogewicht'}", "", "", true)]
    [ACPropertyEntity(5, "GrossWeight", "en{'Gross Weight'}de{'Bruttogewicht'}", "", "", true)]
    [ACPropertyEntity(6, "ProductionWeight", "en{'Production Weight'}de{'Produktionsgewicht'}", "", "", true)]
    [ACPropertyEntity(7, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(9999, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MaterialUnit.ClassName, "en{'Material Unit'}de{'Materialeinheit'}", typeof(MaterialUnit), MaterialUnit.ClassName, "ToMDUnit\\ISOCode", "ToMDUnit\\SortIndex")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaterialUnit>) })]
    public partial class MaterialUnit //, IPackagingHierarchy
    {
        public const string ClassName = "MaterialUnit";

        #region New/Delete
        public static MaterialUnit NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MaterialUnit entity = new MaterialUnit();
            entity.MaterialUnitID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Material)
            {
                Material material = parentACObject as Material;
                entity.Material = material;
                material.MaterialUnit_Material.Add(entity);
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACObject Member

        public override string ToString()
        {
            return ACCaption;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                if (ToMDUnit == null)
                    return "";
                return ToMDUnit.MDUnitName;
            }
        }

        /// <summary>
        /// Returns Material
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Material</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return Material;
            }
        }

        /// <summary>
        /// Returns a related EF-Object which is in a Child-Relationship to this.
        /// </summary>
        /// <param name="className">Classname of the Table/EF-Object</param>
        /// <param name="filterValues">Search-Parameters</param>
        /// <returns>A Entity-Object as IACObject</returns>
        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            return null;
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "ToMDUnit\\ISOCode";
            }
        }
        #endregion

        #region IEntityProperty Members

        bool bRefreshConfig = false;
        protected override void OnPropertyChanging<T>(T newValue, string propertyName, bool afterChange)
        {
            if (propertyName == nameof(XMLConfig))
            {
                string xmlConfig = newValue as string;
                if (afterChange)
                {
                    if (bRefreshConfig)
                        ACProperties.Refresh();
                }
                else
                {
                    bRefreshConfig = false;
                    if (this.EntityState != EntityState.Detached && (!(String.IsNullOrEmpty(xmlConfig) && String.IsNullOrEmpty(XMLConfig)) && xmlConfig != XMLConfig))
                        bRefreshConfig = true;
                }
            }
            base.OnPropertyChanging(newValue, propertyName, afterChange);
        }

        #endregion

        #region Conversion Methods
        /// <summary>
        /// Converts the given quantity measured in this MaterialUnit 
        /// INTO the Base unit of measure (UOM) of this Material
        /// </summary>
        /// <param name="quantity">Quantity measured in this Material-Unit</param>
        /// <returns></returns>
        public Double FromUnitToBase(Double quantity)
        {
            if (Double.IsNaN(quantity) || Double.IsInfinity(quantity))
                return 0;
            if ((Math.Abs(quantity - 0) <= Double.Epsilon)
                || (Math.Abs(this.Multiplier - 0) <= Double.Epsilon)
                || (Math.Abs(this.Divisor - 0) <= Double.Epsilon))
                return 0;
            return quantity * this.Multiplier / this.Divisor;
        }

        /// <summary>
        /// Converts the given quantity measured in Base unit of measure (UOM) of this Material
        /// INTO this MaterialUnit 
        /// </summary>
        /// <param name="quantity">Quantity measured in Base unit of measure (UOM) of this Material</param>
        /// <returns></returns>
        public Double FromBaseToUnit(Double quantity)
        {
            if (Double.IsNaN(quantity) || Double.IsInfinity(quantity))
                return 0;
            if ((Math.Abs(quantity - 0) <= Double.Epsilon)
                || (Math.Abs(this.Multiplier - 0) <= Double.Epsilon)
                || (Math.Abs(this.Divisor - 0) <= Double.Epsilon))
                return 0;
            return quantity * this.Divisor / this.Multiplier;
        }
        #endregion

        #region additional properties

        [ACPropertyInfo(9999, "", "en{'BaseMDUnit'}de{'de-BaseMDUnit'}")]
        public MDUnit BaseMDUnit
        {
            get
            {
                if (Material == null)
                    return null;
                return Material.BaseMDUnit;
            }
        }

        private MaterialStock MaterialStock
        {
            get
            {
                if (this.Material == null)
                    return null;
                if (this.Material.MaterialStock_Material == null)
                    return null;
                if (this.Material.MaterialStock_Material.Count <= 0)
                    return null;
                return this.Material.MaterialStock_Material.First();
            }
        }

        /// <summary>
        /// There are no comments for Property DayInward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double DayInward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.DayInward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.DayInward = FromUnitToBase(value);
                this.OnPropertyChanged("DayInward");
            }
        }


        /// <summary>
        /// There are no comments for Property DayTargetInward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double DayTargetInward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.DayTargetInward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.DayTargetInward = FromUnitToBase(value);
                this.OnPropertyChanged("DayTargetInward");
            }
        }


        /// <summary>
        /// There are no comments for Property DayInwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double DayInwardDiff
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.DayInwardDiff);
            }
        }


        /// <summary>
        /// There are no comments for Property DayOutward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double DayOutward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.DayOutward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.DayOutward = FromUnitToBase(value);
                this.OnPropertyChanged("DayOutward");
                this.OnDayOutwardChanged();
            }
        }

        //private float _DayOutward;
        partial void OnDayOutwardChanging(float value);
        partial void OnDayOutwardChanged();


        /// <summary>
        /// There are no comments for Property DayTargetOutward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double DayTargetOutward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.DayTargetOutward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.DayTargetOutward = FromUnitToBase(value);
                this.OnPropertyChanged("DayTargetOutward");
            }
        }


        /// <summary>
        /// There are no comments for Property DayOutwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double DayOutwardDiff
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.DayOutwardDiff);
            }
        }


        /// <summary>
        /// There are no comments for Property WeekInward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double WeekInward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.WeekInward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.WeekInward = FromUnitToBase(value);
                this.OnPropertyChanged("WeekInward");
            }
        }


        /// <summary>
        /// There are no comments for Property WeekTargetInward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double WeekTargetInward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.WeekTargetInward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.WeekTargetInward = FromUnitToBase(value);
                this.OnPropertyChanged("WeekTargetInward");
            }
        }


        /// <summary>
        /// There are no comments for Property WeekInwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double WeekInwardDiff
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.WeekInwardDiff);
            }
        }


        /// <summary>
        /// There are no comments for Property WeekOutward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double WeekOutward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.WeekOutward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.WeekOutward = FromUnitToBase(value);
                this.OnPropertyChanged("WeekOutward");
            }
        }


        /// <summary>
        /// There are no comments for Property WeekTargetOutward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double WeekTargetOutward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.WeekTargetOutward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.WeekTargetOutward = FromUnitToBase(value);
                this.OnPropertyChanged("WeekTargetOutward");
            }
        }


        /// <summary>
        /// There are no comments for Property WeekOutwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double WeekOutwardDiff
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.WeekOutwardDiff);
            }
        }


        /// <summary>
        /// There are no comments for Property MonthInward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double MonthInward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.MonthInward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.MonthInward = FromUnitToBase(value);
                this.OnPropertyChanged("MonthInward");
            }
        }


        /// <summary>
        /// There are no comments for Property MonthTargetInward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double MonthTargetInward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.MonthTargetInward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.MonthTargetInward = FromUnitToBase(value);
                this.OnPropertyChanged("MonthTargetInward");
            }
        }


        /// <summary>
        /// There are no comments for Property MonthInwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double MonthInwardDiff
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.MonthInwardDiff);
            }
        }


        /// <summary>
        /// There are no comments for Property MonthOutward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double MonthOutward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.MonthOutward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.MonthOutward = FromUnitToBase(value);
                this.OnPropertyChanged("MonthOutward");
            }

        }

        /// <summary>
        /// There are no comments for Property MonthTargetOutward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double MonthTargetOutward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.MonthTargetOutward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.MonthTargetOutward = FromUnitToBase(value);
                this.OnPropertyChanged("MonthTargetOutward");
            }
        }


        /// <summary>
        /// There are no comments for Property MonthOutwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double MonthOutwardDiff
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.MonthOutwardDiff);
            }
        }


        /// <summary>
        /// There are no comments for Property YearInward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double YearInward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.YearInward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.YearInward = FromUnitToBase(value);
                this.OnPropertyChanged("YearInward");
            }
        }


        /// <summary>
        /// There are no comments for Property YearTargetInward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double YearTargetInward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.YearTargetInward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.YearTargetInward = FromUnitToBase(value);
                this.OnPropertyChanged("YearTargetInward");
            }
        }


        /// <summary>
        /// There are no comments for Property YearInwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double YearInwardDiff
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.YearInwardDiff);
            }
        }


        /// <summary>
        /// There are no comments for Property YearOutward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double YearOutward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.YearOutward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.YearOutward = FromUnitToBase(value);
                this.OnPropertyChanged("YearOutward");
            }
        }


        /// <summary>
        /// There are no comments for Property YearTargetOutward in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double YearTargetOutward
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.YearTargetOutward);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.YearTargetOutward = FromUnitToBase(value);
                this.OnPropertyChanged("YearTargetOutward");
            }
        }


        /// <summary>
        /// There are no comments for Property YearOutwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double YearOutwardDiff
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.YearOutwardDiff);
            }
        }


        /// <summary>
        /// There are no comments for Property ReservedInwardQuantity in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double ReservedInwardQuantity
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.ReservedInwardQuantity);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.ReservedInwardQuantity = FromUnitToBase(value);
                this.OnPropertyChanged("ReservedInwardQuantity");
            }
        }


        /// <summary>
        /// There are no comments for Property ReservedOutwardQuantity in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double ReservedOutwardQuantity
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.ReservedOutwardQuantity);
            }
            set
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return;
                materialStock.ReservedOutwardQuantity = FromUnitToBase(value);
                this.OnPropertyChanged("ReservedOutwardQuantity");
            }
        }


        /// <summary>
        /// There are no comments for Property ReservedQuantity in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double ReservedQuantity
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.ReservedQuantity);
            }
        }


        /// <summary>
        /// There are no comments for Property AvailableQuantity in the schema.
        /// </summary>
        [ACPropertyInfo(9999)]
        public Double AvailableQuantity
        {
            get
            {
                MaterialStock materialStock = MaterialStock;
                if (materialStock == null)
                    return 0;
                return FromBaseToUnit(materialStock.AvailableQuantity);
            }
        }

        #endregion

        #region IPackagingHierarchy Members
        public string PackagingHierarchyName
        {
            get
            {
                if (ToMDUnit == null)
                    return "";
                return ToMDUnit.MDUnitName;
            }
        }

        //public IEnumerable<IPackaging> PackagingList 
        //{
        //    get
        //    {
        //        return MaterialUnitPackaging_MaterialUnit.OrderBy(c => c.MaterialUnitPackagingLevel);
        //    }
        //}

        //public Single PackagingFactor
        //{
        //    get
        //    {
        //        Single factor = 0;
        //        foreach (IPackaging packaging in PackagingList)
        //        {
        //            if (factor < 0.00000001)
        //                factor = packaging.ProductQuantity;
        //            else if (packaging.ProductQuantity > 0)
        //                factor *= packaging.ProductQuantity;
        //        }
        //        if (factor < 0.00000001)
        //            factor = 1;
        //        return factor;
        //    }
        //}

        public Single TotalPackagingQuantity
        {
            get
            {
                //if (PackagingFactor > 0 && this.Quantity > 0)
                //    return this.Quantity * PackagingFactor;
                return 0;
            }
        }

        public Single TotalPackagingGrossWeight
        {
            get
            {
                //if (PackagingFactor > 0 && this.GrossWeight > 0)
                //    return this.GrossWeight * PackagingFactor;
                return 0;
            }
        }

        public Single TotalPackagingNetWeight
        {
            get
            {
                //if (PackagingFactor > 0 && this.NetWeight > 0)
                //    return this.NetWeight * PackagingFactor;
                return 0;
            }
        }

        #endregion
    }
}
