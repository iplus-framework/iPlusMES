// ***********************************************************************
// Assembly         : gip.core.datamodel
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 12-14-2012
// ***********************************************************************
// <copyright file="Material.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{

    // Material (Artikel)
    /// <summary>
    /// Class Material
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.Material, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOMaterial")]
    [ACPropertyEntity(1, nameof(MaterialNo), ConstApp.MaterialNo, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, nameof(MaterialName1), ConstApp.MaterialName1, "", "", true)]
    [ACPropertyEntity(3, nameof(MaterialName2), ConstApp.MaterialName2, "", "", true)]
    [ACPropertyEntity(4, nameof(MaterialName3), ConstApp.MaterialName3, "", "", true)]
    [ACPropertyEntity(5, nameof(MDMaterialGroup), "en{'Material Group'}de{'Materialgruppe'}", Const.ContextDatabase + "\\" + MDMaterialGroup.ClassName, "", true)]
    [ACPropertyEntity(6, nameof(MDMaterialType), "en{'Material Type'}de{'Materialart'}", Const.ContextDatabase + "\\" + MDMaterialType.ClassName, "", true)]
    [ACPropertyEntity(7, nameof(BaseMDUnit), "en{'Base Unit of Measure (UOM)'}de{'Basismengeneinheit'}", Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(9, nameof(NetWeight), "en{'Net Weight'}de{'Nettogewicht'}", "", "", true)]
    [ACPropertyEntity(10, nameof(GrossWeight), "en{'Gross Weight'}de{'Bruttogewicht'}", "", "", true)]
    [ACPropertyEntity(11, nameof(ProductionWeight), "en{'Production Weight'}de{'Produktionsgewicht'}", "", "", true)]
    [ACPropertyEntity(12, nameof(Comment), ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(13, nameof(Density), "en{'Density [g/dm³]'}de{'Dichte [g/dm³]'}", "", "", true)]
    [ACPropertyEntity(14, nameof(IsActive), ConstApp.IsActive, "", "", true)]
    [ACPropertyEntity(16, nameof(DontAllowNegativeStock), "en{'No negative Stock'}de{'Kein negativer Bestand'}", "", "", true)]
    [ACPropertyEntity(17, nameof(InFacility), "en{'In Storage Location'}de{'Eingangslager'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(18, nameof(OutFacility), "en{'Out Storage Location'}de{'Ausgangslager'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(19, nameof(MDGMPMaterialGroup), "en{'GMP-Material Group'}de{'GMP-Materialgruppe'}", Const.ContextDatabase + "\\" + MDGMPMaterialGroup.ClassName, "", true)]
    [ACPropertyEntity(20, nameof(MDInventoryManagementType), "en{'Inventory Mgmt. Type'}de{'Inventurverwaltungsart'}", Const.ContextDatabase + "\\" + MDInventoryManagementType.ClassName, "", true)]
    [ACPropertyEntity(21, nameof(MDFacilityManagementType), "en{'Facility Mgmt. Type'}de{'Lagerführungsart'}", Const.ContextDatabase + "\\" + MDFacilityManagementType.ClassName, "", true)]
    [ACPropertyEntity(22, nameof(MinStockQuantity), ConstApp.MinStockQuantity, "", "", true)]
    [ACPropertyEntity(23, nameof(OptStockQuantity), ConstApp.OptStockQuantity, "", "", true)]
    [ACPropertyEntity(24, nameof(Material1_ProductionMaterial), "en{'Production Material'}de{'Produktionsmaterial'}", Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(26, nameof(StorageLife), ConstApp.StorageLife, "", "", true)]
    [ACPropertyEntity(27, nameof(UsageInOrder), "en{'Usage Purchase Order'}de{'Verwendung Bestellung'}", "", "", true)]
    [ACPropertyEntity(28, nameof(UsageOutOrder), "en{'Usage Sales Order'}de{'Verwendung Auftrag'}", "", "", true)]
    [ACPropertyEntity(29, nameof(UsageOwnProduct), "en{'Usage Own Product'}de{'Verwendung Intern'}", "", "", true)]
    [ACPropertyEntity(30, nameof(UsageACProgram), "en{'Usage in Workflow'}de{'Verwendung Workflow'}", "", "", true)]
    [ACPropertyEntity(32, nameof(ContractorStock), "en{'Manage Contractor Stock'}de{'Vertragspartnerbestand'}", "", "", true)]
    [ACPropertyEntity(34, nameof(PetroleumGroupIndex), "en{'Petroleum Group'}de{'Mineralölgruppe'}", typeof(GlobalApp.PetroleumGroups), Const.ContextDatabase + "\\PetroleumGroupList", "", true)]
    [ACPropertyEntity(35, nameof(DensityAmb), "en{'Ambient Density'}de{'Umgebungsdichte'}", "", "", true)]
    [ACPropertyEntity(45, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACPropertyEntity(36, nameof(Label), "en{'Label'}de{'Label'}", Const.ContextDatabase + "\\Label", "", true)]
    [ACPropertyEntity(37, nameof(MaterialWF), "en{'Material-WF'}de{'Material-WF'}", Const.ContextDatabase + "\\" + MaterialWF.ClassName, "", true)]
    [ACPropertyEntity(38, nameof(IsIntermediate), "en{'Is an Intermediate'}de{'Ist ein Zwischenprodukt'}", "", "", true)]
    [ACPropertyEntity(39, nameof(ZeroBookingTolerance), "en{'ZeroBooking Tolerance'}de{'Null-Buchungstoleranz'}", "", "", true)]
    [ACPropertyEntity(40, nameof(RetrogradeFIFO), "en{'Backflushing'}de{'Retrograde Entnahme'}", "", "", true)]
    [ACPropertyEntity(41, nameof(ExplosionOff), "en{'Explosion Off'}de{'Stoprückauflösung'}", "", "", true)]
    [ACPropertyEntity(42, nameof(SpecHeatCapacity), "en{'Specific heat capacity J/kgK'}de{'Spezifische Wärmekapazität J/kgK'}", "", "", true)]
    [ACPropertyEntity(43, nameof(Anterograde), "en{'Anterograde inward posting'}de{'Anterograde Zugangsbuchung'}", "", "", true)]
    [ACPropertyEntity(44, nameof(ExcludeFromSumCalc), "en{'Exclude from sum calculation'}de{'Aus Summenberechnung ausschließen'}", "", "", true)]
    [ACPropertyEntity(46, nameof(MRPProcedureIndex), ConstApp.MRPProcedure, typeof(MRPProcedure), Const.ContextDatabase + "\\" + nameof(DatabaseApp.MRPProcedureList), "", true)]
    //QRYMaterialCalculation", "en{'MaterialCalculation'}de{'Materialkalkulation'}", typeof(MaterialCalculation), "MaterialCalculation", "Material\\MaterialName1", "Material\\MaterialName1")]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + Material.ClassName, ConstApp.Material, typeof(Material), Material.ClassName, "MaterialNo,MaterialName1", "MaterialNo", new object[]
        {
                new object[] {Const.QueryPrefix + MaterialUnit.ClassName, "en{'Units'}de{'Einheiten'}", typeof(MaterialUnit), MaterialUnit.ClassName + "_" +  Material.ClassName, "ToMDUnit\\ISOCode", "ToMDUnit\\SortIndex"},
                new object[] {Const.QueryPrefix + MaterialCalculation.ClassName, "en{'Calulation'}de{'Kalkulation'}", typeof(MaterialCalculation), MaterialCalculation.ClassName + "_" + Material.ClassName, "MaterialCalculationNo", "MaterialCalculationNo"},

                new object[] {Const.QueryPrefix + MaterialStock.ClassName, "en{'Material Stock'}de{'Materialbestand'}", typeof(MaterialStock), MaterialStock.ClassName + "_" + Material.ClassName, Material.ClassName + "\\MaterialNo", Material.ClassName + "\\MaterialNo"},
                new object[] {Const.QueryPrefix + MaterialGMPAdditive.ClassName, "en{'GMP-Additive'}de{'GMP-Zusatzstoffe'}", typeof(MaterialGMPAdditive), MaterialGMPAdditive.ClassName + "_" + Material.ClassName, "Sequence", "Sequence"}
        })
    ]
    //[ACSerializeableInfo(new Type[] { typeof(ACRef<Material>), typeof(Material) })]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<Material>) })]
    public partial class Material : IACConfigStore, IACWorkflowNode, IACClassDesignProvider, IImageInfo, ICloneable
    {
        public const string ClassName = "Material";
        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(11020);

        #region New/Delete
        /// <summary>
        /// News the AC object.
        /// </summary>
        /// <param name="dbApp">The database.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <returns>Material.</returns>
        public static Material NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            Material entity = new Material();
            entity.MaterialID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDFacilityManagementType = MDFacilityManagementType.DefaultMDFacilityManagementType(dbApp);
            entity.MDInventoryManagementType = MDInventoryManagementType.DefaultMDInventoryManagementType(dbApp);

            entity.MDMaterialGroup = MDMaterialGroup.DefaultMDMaterialGroup(dbApp);
            entity.BaseMDUnit = MDUnit.GetSIUnit(dbApp, GlobalApp.SIDimensions.Mass);
            entity.UsageInOrder = true;
            entity.UsageOutOrder = true;
            entity.UsageOwnProduct = false;
            entity.UsageACProgram = true;
            entity.IsActive = true;
            entity.IsIntermediate = false;
            entity.ZeroBookingTolerance = 0;
            entity.SpecHeatCapacity = 0;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACObject Member

        public override string ToString()
        {
            return MaterialNo + "/" + MaterialName1;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                return MaterialNo + " " + MaterialName1;
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
            if (filterValues.Any())
            {
                switch (className)
                {
                    case MaterialGMPAdditive.ClassName:
                        Int32 sequence = 0;
                        if (Int32.TryParse(filterValues[0], out sequence))
                            return this.MaterialGMPAdditive_Material.Where(c => c.Sequence == sequence).FirstOrDefault();
                        break;
                    case MaterialStock.ClassName:
                        return this.MaterialStock_Material.Where(c => c.Material.MaterialNo == filterValues[0]).FirstOrDefault();
                    case MaterialUnit.ClassName:
                        return this.MaterialUnit_Material.Where(c => (c.ToMDUnit != null) && (c.ToMDUnit.ISOCode == filterValues[0])).FirstOrDefault();
                    case MaterialCalculation.ClassName:
                        return this.MaterialCalculation_Material.Where(c => c.MaterialCalculationNo == filterValues[0]).FirstOrDefault();
                }
            }
            return null;
        }

        public MRPProcedure MRPProcedure
        {
            get
            {
                return (MRPProcedure)this.MRPProcedureIndex;
            }
        }

        #endregion

        #region IACObjectEntity Members
        /// <summary>
        /// Gets the key AC identifier.
        /// </summary>
        /// <value>The key AC identifier.</value>
        static public string KeyACIdentifier
        {
            get
            {
                return "MaterialNo";
            }
        }
        #endregion

        #region
        /// <summary>
        /// Gets the material stock_ insert if not exists.
        /// </summary>
        /// <param name="dbApp">The database.</param>
        /// <returns>MaterialStock.</returns>
        public MaterialStock GetMaterialStock_InsertIfNotExists(DatabaseApp dbApp)
        {
            MaterialStock materialStock = MaterialStock_Material.FirstOrDefault();
            if (materialStock != null)
                return materialStock;
            materialStock = MaterialStock.NewACObject(dbApp, this);
            return materialStock;
        }

        /// <summary>
        /// Gets the default partslist.
        /// </summary>
        /// <returns>Partslist.</returns>
        public Partslist GetDefaultPartslist()
        {
            var query = Partslist_Material.Where(c => c.IsDefault);
            if (query.Any())
                return query.First();
            var query2 = Partslist_Material;
            if (query2.Any())
                return query2.First();
            return null;
        }
        #endregion

        #region IEntityProperty Members

        bool bRefreshConfig = false;
        partial void OnXMLConfigChanging(global::System.String value)
        {
            bRefreshConfig = false;
            if (this.EntityState != System.Data.EntityState.Detached && (!(String.IsNullOrEmpty(value) && String.IsNullOrEmpty(XMLConfig)) && value != XMLConfig))
                bRefreshConfig = true;
        }

        partial void OnXMLConfigChanged()
        {
            if (bRefreshConfig)
                ACProperties.Refresh();
        }

        /// <summary>
        /// There are no comments for Property CurrentMaterialStock in the schema.
        /// </summary>
        /// <value>The current material stock.</value>
        public MaterialStock CurrentMaterialStock
        {
            get
            {
                if (!this.MaterialStock_Material.Any())
                    return null;
                return this.MaterialStock_Material.First();
            }
        }

        #endregion

        #region UnitCalculation

        /// <summary>
        /// Gibt eine Liste von Mengeneneinheiten zurück in denen das Material im Lager verwaltet werden darf
        /// </summary>
        List<MDUnit> _MaterialUnitList = null;
        /// <summary>
        /// Gets the MU quantity unit list.
        /// </summary>
        /// <value>The MU quantity unit list.</value>
        [ACPropertyInfo(9999, "", "en{'Mat. Unit'}de{'Mat. Einheiten'}")]
        public List<MDUnit> MaterialUnitList
        {
            get
            {
                if (this.BaseMDUnit == null)
                    return null;
                if (_MaterialUnitList == null)
                {
                    _MaterialUnitList = new List<MDUnit>();
                    _MaterialUnitList.Insert(0, this.BaseMDUnit);

                    // Materialbezogene Einheiten
                    foreach (var materialUnit in this.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0))
                    {
                        if (!_MaterialUnitList.Contains(materialUnit.ToMDUnit))
                        {
                            _MaterialUnitList.Add(materialUnit.ToMDUnit);
                        }
                    }
                }
                return _MaterialUnitList;
            }
        }

        /// <summary>
        /// Gibt eine Liste von Mengeneneinheiten zurück in denen das Material im Lager verwaltet werden darf
        /// </summary>
        List<MDUnit> _MDUnitList = null;
        /// <summary>
        /// Gets the MU quantity unit list.
        /// </summary>
        /// <value>The MU quantity unit list.</value>
        [ACPropertyInfo(9999, "", "en{'Unit'}de{'Einheiten'}")]
        public List<MDUnit> MDUnitList
        {
            get
            {
                if (this.BaseMDUnit == null)
                    return null;
                if (_MDUnitList == null)
                {
                    _MDUnitList = new List<MDUnit>();
                    _MDUnitList.AddRange(MaterialUnitList);

                    // Umrechenbare Einheiten
                    foreach (var mdUnit in this.BaseMDUnit.ConvertableUnits)
                    {
                        if (!_MDUnitList.Contains(mdUnit))
                        {
                            _MDUnitList.Add(mdUnit);
                        }
                    }
                }
                return _MDUnitList;
            }
        }

        /// <summary>
        /// Converts the passed quantity measured in the passed mdUnit
        /// INTO the Base unit of measure (UOM) of this Material
        /// </summary>
        /// <param name="quantity">quantity measured in mdUnit</param>
        /// <param name="mdUnit">Unit pf the passed quantity</param>
        /// <returns>Quantity in Base unit of measure (UOM) of this Material</returns>
        /// <exception cref="ArgumentException">Thrown if conversion not possible because of incompatible units</exception>
        /// <exception cref="ArgumentNullException">Thrown if mdUnit is null</exception>
        public Double ConvertToBaseQuantity(Double quantity, MDUnit mdUnit)
        {
            if (mdUnit == null)
                throw new ArgumentNullException("mdUnit");

            // Einheiten identisch
            if (this.BaseMDUnit != null && mdUnit.MDUnitID == this.BaseMDUnit.MDUnitID)
                return quantity;

            if (((BaseMDUnit.SIDimension == GlobalApp.SIDimensions.Volume && mdUnit.SIDimension == GlobalApp.SIDimensions.Mass)
                    || (BaseMDUnit.SIDimension == GlobalApp.SIDimensions.Mass && mdUnit.SIDimension == GlobalApp.SIDimensions.Volume))
                && Density > 0.0001)// Density g / dm³
            {
                return ConvertQuantity(quantity, mdUnit, BaseMDUnit);
            }

            // Umrechnen über MaterialUnit
            var query = this.MaterialUnit_Material.Where(c => c.ToMDUnitID == mdUnit.MDUnitID);
            if (query.Any())
            {
                MaterialUnit materialUnit = query.First();
                double result = materialUnit.FromUnitToBase(quantity);
                if (BaseMDUnit.Rounding >= 0)
                    result = BaseMDUnit.GetRoundedValue(result);
                return result;
            }

            try
            {
                return this.BaseMDUnit.ConvertFromUnit(quantity, mdUnit);
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                this.Root().Messages.LogException(ClassName, "ConvertToBaseQuantity", msg);
            }
            return mdUnit.ConvertToUnit(quantity, this.BaseMDUnit);
        }

        public bool IsConvertableToBaseUnit(MDUnit mdUnit)
        {
            if (mdUnit == null)
                throw new ArgumentNullException("mdUnit");

            // Einheiten identisch
            if (mdUnit.MDUnitID == this.BaseMDUnit.MDUnitID)
                return true;

            if (((BaseMDUnit.SIDimension == GlobalApp.SIDimensions.Volume && mdUnit.SIDimension == GlobalApp.SIDimensions.Mass)
                    || (BaseMDUnit.SIDimension == GlobalApp.SIDimensions.Mass && mdUnit.SIDimension == GlobalApp.SIDimensions.Volume))
                && Density >= 0.0001)// Density g / dm³
            {
                return true;
            }

            // Umrechnen über MaterialUnit
            var query = this.MaterialUnit_Material.Where(c => c.ToMDUnitID == mdUnit.MDUnitID);
            if (query.Any())
                return true;

            if (BaseMDUnit.IsConvertableFromUnit(mdUnit))
                return true;

            return mdUnit.IsConvertableToUnit(this.BaseMDUnit);
        }

        /// <summary>
        /// Converts the passed quantioty measured in the Base unit of measure (UOM)
        /// INTO the target Unit
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="toMDUnit"></param>
        /// <returns></returns>
        public Double ConvertFromBaseQuantity(Double quantity, MDUnit toMDUnit)
        {
            if (toMDUnit == null)
                throw new ArgumentNullException("mdUnit");

            // Einheiten identisch
            if (toMDUnit.MDUnitID == this.BaseMDUnit.MDUnitID)
                return quantity;

            if (((BaseMDUnit.SIDimension == GlobalApp.SIDimensions.Volume && toMDUnit.SIDimension == GlobalApp.SIDimensions.Mass)
                    || (BaseMDUnit.SIDimension == GlobalApp.SIDimensions.Mass && toMDUnit.SIDimension == GlobalApp.SIDimensions.Volume))
                && Density >= 0.0001)// Density g / dm³
            {
                return ConvertQuantity(quantity, BaseMDUnit, toMDUnit);
            }

            // Umrechnen über MaterialUnit
            var query = this.MaterialUnit_Material.Where(c => c.ToMDUnitID == toMDUnit.MDUnitID);
            if (query.Any())
            {
                MaterialUnit materialUnit = query.First();
                double result = materialUnit.FromBaseToUnit(quantity);
                if (toMDUnit.Rounding >= 0)
                    result = toMDUnit.GetRoundedValue(result);
                return result;
            }

            try
            {
                return this.BaseMDUnit.ConvertToUnit(quantity, toMDUnit);
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                this.Root().Messages.LogException(ClassName, "ConvertFromBaseQuantity", msg);
            }
            return toMDUnit.ConvertFromUnit(quantity, this.BaseMDUnit);

        }


        public List<Tuple<MDUnit, double>> ConvertAllFromBaseQuantity(Double quantity)
        {
            List<Tuple<MDUnit, double>> convertedValues = new List<Tuple<MDUnit, double>>();
            foreach (MaterialUnit altUnit in MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0))
            {
                convertedValues.Add(new Tuple<MDUnit, double>(altUnit.ToMDUnit, ConvertFromBaseQuantity(quantity, altUnit.ToMDUnit)));
            }
            return convertedValues;
        }

        public Tuple<MDUnit, double> ConvertBaseQuantity(Double quantity, ushort indexInMaterialUnit)
        {
            List<Tuple<MDUnit, double>> convertedValues = ConvertAllFromBaseQuantity(quantity);
            if (indexInMaterialUnit >= convertedValues.Count)
                return null;
            return convertedValues[indexInMaterialUnit];
        }


        /// <summary>
        /// Converts the passed quantity measured in the passed fromMDUnit
        /// INTO a quantity measured in the passed toMDUnit
        /// First it tries to convert the Quantity over alkternative Unit of Measure (MaterialUnit)
        /// IF not possible then over the common conversion with MDUnit
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="fromMDUnit"></param>
        /// <param name="toMDUnit"></param>
        /// <returns>Quantity measured in toMDUnit</returns>
        /// <exception cref="ArgumentException">Thrown if conversion not possible because of incompatible units</exception>
        /// <exception cref="ArgumentNullException">Thrown if mdUnit is null</exception>
        public Double ConvertQuantity(Double quantity, MDUnit fromMDUnit, MDUnit toMDUnit)
        {
            if (fromMDUnit == null)
                throw new ArgumentNullException("fromMDUnit");
            if (toMDUnit == null)
                throw new ArgumentNullException("toMDUnit");
            if (fromMDUnit.MDUnitID == toMDUnit.MDUnitID)
                return quantity;
            if (Double.IsNaN(quantity) || Double.IsInfinity(quantity))
                return 0;
            if (Math.Abs(quantity - 0) <= Double.Epsilon)
                return 0;

            if (((fromMDUnit.SIDimension == GlobalApp.SIDimensions.Volume && toMDUnit.SIDimension == GlobalApp.SIDimensions.Mass)
                    || (fromMDUnit.SIDimension == GlobalApp.SIDimensions.Mass && toMDUnit.SIDimension == GlobalApp.SIDimensions.Volume))
                && Density >= 0.0001)// Density g / dm³
            {
                // Mass to Volume
                if (fromMDUnit.SIDimension == GlobalApp.SIDimensions.Mass)
                {
                    double weightInGram = quantity;
                    if (fromMDUnit.ISOCode != ConstApp.UOM_ISOCode_g)
                    {
                        if (fromMDUnit.ISOCode == ConstApp.UOM_ISOCode_kg)
                            weightInGram = quantity * 1000;
                        else // UOM_ISOCode_t
                            weightInGram = quantity * 1000 * 1000;
                    }
                    return weightInGram / Density;
                }
                // Volume to mass
                else
                {
                    double weightInGram = quantity * Density;
                    if (toMDUnit.ISOCode == ConstApp.UOM_ISOCode_g)
                        return weightInGram;
                    else if (toMDUnit.ISOCode == ConstApp.UOM_ISOCode_kg)
                        return weightInGram * 0.001;
                    else //if (toMDUnit.ISOCode == ConstApp.UOM_ISOCode_t)
                        return weightInGram * 0.000001;
                }
            }

            // Falls Umwandlung von Basiseinheit nach Materialeinheit
            if (this.BaseMDUnitID == fromMDUnit.MDUnitID)
            {
                var queryTo = this.MaterialUnit_Material.Where(c => c.ToMDUnitID == toMDUnit.MDUnitID);
                if (queryTo.Any())
                {
                    double result = queryTo.First().FromBaseToUnit(quantity);
                    if (toMDUnit.Rounding >= 0)
                        result = toMDUnit.GetRoundedValue(result);
                    return result;
                }
            }
            // Sonst Falls Umwandlung von Materialeinheit nach Basiseinheit
            else if (this.BaseMDUnitID == toMDUnit.MDUnitID)
            {
                var queryFrom = this.MaterialUnit_Material.Where(c => c.ToMDUnitID == fromMDUnit.MDUnitID);
                if (queryFrom.Any())
                {
                    double result = queryFrom.First().FromUnitToBase(quantity);
                    if (toMDUnit.Rounding >= 0)
                        result = toMDUnit.GetRoundedValue(result);
                    return result;
                }
            }
            // Sonst Falls Umwandlung innerhalb von Materialeinheiten
            else
            {
                var queryTo = this.MaterialUnit_Material.Where(c => c.ToMDUnitID == toMDUnit.MDUnitID);
                var queryFrom = this.MaterialUnit_Material.Where(c => c.ToMDUnitID == fromMDUnit.MDUnitID);
                if (queryTo.Any() && queryFrom.Any())
                {
                    // Wandle Menge in Basiseinheit um und dann von Basiseinheit in Ziel-Materialeinheit
                    double quantityBase = queryFrom.First().FromUnitToBase(quantity);
                    double result = queryTo.First().FromBaseToUnit(quantityBase);
                    if (toMDUnit.Rounding >= 0)
                        result = toMDUnit.GetRoundedValue(result);
                    return result;
                }
                if ((fromMDUnit.SIDimension != GlobalApp.SIDimensions.None) && (!fromMDUnit.IsSIUnit) && (fromMDUnit.SIUnit != null) && (fromMDUnit.SIUnit.MDUnitID == this.BaseMDUnitID))
                {
                    if (queryTo.Any())
                    {
                        double quantityBase = fromMDUnit.SIUnit.ConvertToUnit(quantity, fromMDUnit);
                        double result = queryTo.First().FromBaseToUnit(quantityBase);
                        if (toMDUnit.Rounding >= 0)
                            result = toMDUnit.GetRoundedValue(result);
                        return result;
                    }
                }
                if ((toMDUnit.SIDimension != GlobalApp.SIDimensions.None) && (!toMDUnit.IsSIUnit) && (toMDUnit.SIUnit != null) && (toMDUnit.SIUnit.MDUnitID == this.BaseMDUnitID))
                {
                    if (queryFrom.Any())
                    {
                        // Wandle Menge in Basiseinheit um und dann von Basiseinheit in Ziel-Materialeinheit
                        double quantityBase = queryFrom.First().FromUnitToBase(quantity);
                        double result = toMDUnit.SIUnit.ConvertToUnit(quantityBase, toMDUnit);
                        if (toMDUnit.Rounding >= 0)
                            result = toMDUnit.GetRoundedValue(result);
                        return result;
                    }
                }
            }

            // Sonst versuche Umwandung über Maßeinheiten
            if (fromMDUnit.IsConvertableToUnit(toMDUnit))
                return fromMDUnit.ConvertToUnit(quantity, toMDUnit);
            else
                return toMDUnit.ConvertFromUnit(quantity, fromMDUnit);
        }

        public bool IsConvertableToUnit(MDUnit fromMDUnit, MDUnit toMDUnit)
        {
            if (fromMDUnit == null)
                throw new ArgumentNullException("fromMDUnit");
            if (toMDUnit == null)
                throw new ArgumentNullException("toMDUnit");
            if (fromMDUnit.MDUnitID == toMDUnit.MDUnitID)
                return true;

            if (((fromMDUnit.SIDimension == GlobalApp.SIDimensions.Volume && toMDUnit.SIDimension == GlobalApp.SIDimensions.Mass)
                    || (fromMDUnit.SIDimension == GlobalApp.SIDimensions.Mass && toMDUnit.SIDimension == GlobalApp.SIDimensions.Volume))
                && Density >= 0.0001)
            {
                return true;
            }

            // Falls Umwandlung von Basiseinheit nach Materialeinheit
            if (this.BaseMDUnitID == fromMDUnit.MDUnitID)
            {
                var queryTo = this.MaterialUnit_Material.Where(c => c.ToMDUnitID == toMDUnit.MDUnitID);
                if (queryTo.Any())
                    return true;
            }
            // Sonst Falls Umwandlung von Materialeinheit nach Basiseinheit
            else if (this.BaseMDUnitID == toMDUnit.MDUnitID)
            {
                var queryFrom = this.MaterialUnit_Material.Where(c => c.ToMDUnitID == fromMDUnit.MDUnitID);
                if (queryFrom.Any())
                    return true;
            }
            // Sonst Falls Umwandlung innerhalb von Materialeinheiten
            else
            {
                var queryTo = this.MaterialUnit_Material.Where(c => c.ToMDUnitID == toMDUnit.MDUnitID);
                var queryFrom = this.MaterialUnit_Material.Where(c => c.ToMDUnitID == fromMDUnit.MDUnitID);
                if (queryTo.Any() && queryFrom.Any())
                    return true;
                if ((fromMDUnit.SIDimension != GlobalApp.SIDimensions.None) && (!fromMDUnit.IsSIUnit) && (fromMDUnit.SIUnit != null) && (fromMDUnit.SIUnit.MDUnitID == this.BaseMDUnitID))
                {
                    if (queryTo.Any())
                        return fromMDUnit.SIUnit.IsConvertableToUnit(fromMDUnit);
                }
                if ((toMDUnit.SIDimension != GlobalApp.SIDimensions.None) && (!toMDUnit.IsSIUnit) && (toMDUnit.SIUnit != null) && (toMDUnit.SIUnit.MDUnitID == this.BaseMDUnitID))
                {
                    if (queryFrom.Any())
                        return toMDUnit.SIUnit.IsConvertableToUnit(toMDUnit);
                }
            }

            // Sonst versuche Umwandung über Maßeinheiten
            return fromMDUnit.IsConvertableToUnit(toMDUnit);
        }


        public bool IsBaseUnitConvertableToUnit(MDUnit toMDUnit)
        {
            return IsConvertableToUnit(this.BaseMDUnit, toMDUnit);
        }

        /// <summary>
        /// Converts to base weight.
        /// </summary>
        /// <param name="fromQuantity">From quantity.</param>
        /// <param name="fromMDUnit">From MD unit.</param>
        /// <returns>System.Single.</returns>
        public Double ConvertToBaseWeight(Double fromQuantity, MDUnit fromMDUnit)
        {
            if (fromMDUnit.ISOCode == ConstApp.UOM_ISOCode_kg)
                return fromQuantity;
            if (fromMDUnit.SIDimension == GlobalApp.SIDimensions.Volume)
            {
                if (Density >= 0.0001)
                    return fromQuantity * Density * 0.001;
            }
            MDUnit weightUnit = MDUnitList.Where(c => c.ISOCode == ConstApp.UOM_ISOCode_kg).FirstOrDefault();
            if (weightUnit == null)
                throw new ArgumentException("Not convertable to KG!");
            return ConvertQuantity(fromQuantity, fromMDUnit, weightUnit);
        }


        /// <summary>
        /// Converts a weight measured in 
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="toMDUnit"></param>
        /// <returns></returns>
        public Double ConvertBaseWeightToUnit(Double weight, MDUnit toMDUnit)
        {
            if (toMDUnit.SIDimension == GlobalApp.SIDimensions.Volume)
            {
                if (Density >= 0.0001)
                    return (weight * 1000) / Density;
            }

            MDUnit weightUnit = MDUnitList.Where(c => c.ISOCode == ConstApp.UOM_ISOCode_kg).FirstOrDefault();
            if (weightUnit == null)
                throw new ArgumentException("Not convertable");
            return ConvertQuantity(weight, weightUnit, toMDUnit);
        }

        public double ConvertToBaseWeight(Double fromQuantityInUOM)
        {
            return ConvertToBaseWeight(fromQuantityInUOM, this.BaseMDUnit);
        }

        public double ConvertBaseWeightToBaseUnit(Double weight)
        {
            return ConvertBaseWeightToUnit(weight, this.BaseMDUnit);
        }

        public double ConvertToVolume(Double fromQuantityInUOM)
        {
            if (this.BaseMDUnit.SIDimension == GlobalApp.SIDimensions.Volume)
                return fromQuantityInUOM;
            double weight = ConvertToBaseWeight(fromQuantityInUOM);
            return ConvertWeightToVolume(weight);
        }

        public double ConvertWeightToVolume(Double weight)
        {
            if (this.Density >= 0.0001)
                return (weight * 1000) / Density;
            MDUnit volumeUnit = MDUnitList.Where(c => c.SIDimension == GlobalApp.SIDimensions.Volume).FirstOrDefault();
            if (volumeUnit == null)
                throw new ArgumentException("Not convertable");
            return ConvertBaseWeightToUnit(weight, volumeUnit);
        }

        /// <summary>
        /// Falls chemisch/physisch gleiche Materialen unter verschiedenen Materialnummern verwaltet werden,
        /// muss in den Plausibilitätsprüfungen oder Materialsuchalgorithmen herausgefunden werden, ob es sich chemisch um das gleiche Material handelt.
        /// Dafür hat jedes Material einen Verweis auf ein sogenanntes "AlternativMaterial" das als eine Art Materialgruppe dient
        /// Haben zwei Materialien denselben Verweis oder eines von beiden ist der Verweis, dann sind die Materialien indentisch.
        /// </summary>
        /// <param name="material1">The material1.</param>
        /// <param name="material2">The material2.</param>
        /// <returns><c>true</c> if [is material equal] [the specified material1]; otherwise, <c>false</c>.</returns>
        static public bool IsMaterialEqual(Material material1, Material material2)
        {
            if ((material1 == null) || (material2 == null))
                return false;
            if (material1 == material2)
                return true;
            if (material1.MaterialID == material2.MaterialID)
                return true;
            if (material1.ProductionMaterialID.HasValue && material1.ProductionMaterialID == material2.MaterialID)
                return true;
            if (material2.ProductionMaterialID.HasValue && material2.ProductionMaterialID == material1.MaterialID)
                return true;
            if (material1.ProductionMaterialID.HasValue && material2.ProductionMaterialID.HasValue && material1.ProductionMaterialID == material2.ProductionMaterialID)
                return true;
            return false;
        }

        /// <summary>
        /// Interner Aufruf der statischen Methode Material.IsMaterialEqual(Material material1, Material material2)
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns><c>true</c> if [is material equal] [the specified material]; otherwise, <c>false</c>.</returns>
        public bool IsMaterialEqual(Material material)
        {
            return IsMaterialEqual(this, material);
        }
        #endregion

        #region Properties

        [ACPropertyInfo(15, "", "en{'Lot Managed'}de{'Chargenführung'}")]
        public bool IsLotManaged
        {
            get
            {
                bool lotManaged = false;
                if (MDFacilityManagementType != null)
                {
                    if (MDFacilityManagementType.MDFacilityManagementTypeIndex == (int)MDFacilityManagementType.FacilityManagementTypes.FacilityCharge
                        || MDFacilityManagementType.MDFacilityManagementTypeIndex == (int)MDFacilityManagementType.FacilityManagementTypes.FacilityChargeReservation)
                        lotManaged = true;
                }
                return lotManaged;
            }
            set
            {
                if (MDFacilityManagementType != null)
                {
                    if (MDFacilityManagementType.MDFacilityManagementTypeIndex == (int)MDFacilityManagementType.FacilityManagementTypes.NoFacility)
                        return;
                }

                DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                if (dbApp != null)
                {
                    if (value == true)
                    {
                        var query = dbApp.MDFacilityManagementType.Where(c => c.MDFacilityManagementTypeIndex == (int)MDFacilityManagementType.FacilityManagementTypes.FacilityCharge);
                        if (query.Any())
                            MDFacilityManagementType = query.First();
                    }
                    else
                    {
                        var query = dbApp.MDFacilityManagementType.Where(c => c.MDFacilityManagementTypeIndex == (int)MDFacilityManagementType.FacilityManagementTypes.Facility);
                        if (query.Any())
                            MDFacilityManagementType = query.First();
                    }
                }
                OnPropertyChanged(nameof(IsLotReservationNeeded));
                OnPropertyChanged(nameof(IsLotManaged));
            }
        }

        [ACPropertyInfo(15, "", "en{'Is Lot reservation obligatory'}de{'Chargenreservierungspflichtig'}")]
        public bool IsLotReservationNeeded
        {
            get
            {
                bool lotManaged = false;
                if (MDFacilityManagementType != null)
                {
                    if (MDFacilityManagementType.MDFacilityManagementTypeIndex == (int)MDFacilityManagementType.FacilityManagementTypes.FacilityChargeReservation)
                        lotManaged = true;
                }
                return lotManaged;
            }
            set
            {
                if (MDFacilityManagementType != null)
                {
                    if (MDFacilityManagementType.MDFacilityManagementTypeIndex == (int)MDFacilityManagementType.FacilityManagementTypes.NoFacility)
                        return;
                }

                DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                if (dbApp != null)
                {
                    if (value == true)
                    {
                        var query = dbApp.MDFacilityManagementType.Where(c => c.MDFacilityManagementTypeIndex == (int)MDFacilityManagementType.FacilityManagementTypes.FacilityChargeReservation);
                        if (query.Any())
                            MDFacilityManagementType = query.First();
                    }
                    else
                    {
                        var query = dbApp.MDFacilityManagementType.Where(c => c.MDFacilityManagementTypeIndex == (int)MDFacilityManagementType.FacilityManagementTypes.FacilityCharge);
                        if (query.Any())
                            MDFacilityManagementType = query.First();
                    }
                }
                OnPropertyChanged(nameof(IsLotReservationNeeded));
                OnPropertyChanged(nameof(IsLotManaged));
            }
        }

        public bool IsDensityValid
        {
            get
            {
                return ValidateDensity(Density);
            }
        }

        public static bool ValidateDensity(double density)
        {
            return density > 50 && density < 5000;
        }

        private bool _IsSelected;
        [ACPropertyInfo(999, "IsSelected", "en{'Selected'}de{'Ausgewählt'}")]
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

        #endregion

        #region IACConfigStore

        private string configStoreName;
        public string ConfigStoreName
        {
            get
            {
                if (configStoreName == null)
                {
                    ACClassInfo acClassInfo = (ACClassInfo)GetType().GetCustomAttributes(typeof(ACClassInfo), false)[0];
                    configStoreName = Translator.GetTranslation(acClassInfo.ACCaptionTranslation);
                }
                return configStoreName;
            }
        }

        /// <summary>
        /// ACConfigKeyACUrl returns the relative Url to the "main table" in group a group of semantically related tables.
        /// This property is used when NewACConfig() is called. NewACConfig() creates a new IACConfig-Instance and set the IACConfig.KeyACUrl-Property with this ACConfigKeyACUrl.
        /// </summary>
        public string ACConfigKeyACUrl
        {
            get
            {
                return ".\\Material(" + this.ACIdentifier + ")";
            }
        }

        /// <summary>
        /// Creates and adds a new IACConfig-Entry to ConfigItemsSource.
        /// The implementing class creates a new entity object an add it to its "own Configuration-Table".
        /// It sets automatically the IACConfig.KeyACUrl-Property with this ACConfigKeyACUrl.
        /// </summary>
        /// <param name="acObject">Optional: Reference to another Entity-Object that should be related for this new configuration entry.</param>
        /// <param name="valueTypeACClass">The iPlus-Type of the "Value"-Property.</param>
        /// <returns>IACConfig as a new entry</returns>
        public IACConfig NewACConfig(IACObjectEntity acObject = null, gip.core.datamodel.ACClass valueTypeACClass = null, string localConfigACUrl = null)
        {
            MaterialConfig acConfig = MaterialConfig.NewACObject(this.GetObjectContext<DatabaseApp>(), this);
            acConfig.KeyACUrl = ACConfigKeyACUrl;
            acConfig.LocalConfigACUrl = localConfigACUrl;
            acConfig.ValueTypeACClass = valueTypeACClass;
            MaterialConfig_Material.Add(acConfig);
            ACConfigListCache.Add(acConfig);
            return acConfig;
        }

        /// <summary>Removes a configuration from ConfigurationEntries and the database-context.</summary>
        /// <param name="acObject">Entry as IACConfig</param>
        public void RemoveACConfig(IACConfig acObject)
        {
            MaterialConfig acConfig = acObject as MaterialConfig;
            if (acConfig == null)
                return;
            if (ACConfigListCache.Contains(acConfig))
                ACConfigListCache.Remove(acConfig);
            MaterialConfig_Material.Remove(acConfig);
            if (acConfig.EntityState != System.Data.EntityState.Detached)
                acConfig.DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
        }

        /// <summary>
        /// Deletes all IACConfig-Entries in the Database-Context as well as in ConfigurationEntries.
        /// </summary>
        public void DeleteAllConfig()
        {
            if (!ConfigurationEntries.Any())
                return;
            ClearCacheOfConfigurationEntries();
            IEnumerable<IACConfig> list = ConfigurationEntries.ToList();
            foreach (var acConfig in list)
            {
                (acConfig as MaterialConfig).DeleteACObject(this.GetObjectContext<DatabaseApp>(), false);
            }
            ClearCacheOfConfigurationEntries();
        }

        public decimal OverridingOrder { get; set; }

        /// <summary>
        /// A thread-safe and cached list of Configuration-Values of type IACConfig.
        /// </summary>
        public IEnumerable<IACConfig> ConfigurationEntries
        {
            get
            {
                return ACConfigListCache;
            }
        }

        private SafeList<IACConfig> _ACConfigListCache;
        private SafeList<IACConfig> ACConfigListCache
        {
            get
            {
                using (ACMonitor.Lock(_11020_LockValue))
                {
                    if (_ACConfigListCache != null)
                        return _ACConfigListCache;
                }
                SafeList<IACConfig> newSafeList = new SafeList<IACConfig>();
                if (MaterialConfig_Material.IsLoaded)
                {
                    MaterialConfig_Material.AutoRefresh();
                    MaterialConfig_Material.AutoLoad();
                }
                newSafeList = new SafeList<IACConfig>(MaterialConfig_Material.ToList().Select(x => (IACConfig)x));
                using (ACMonitor.Lock(_11020_LockValue))
                {
                    _ACConfigListCache = newSafeList;
                    return _ACConfigListCache;
                }
            }
        }

        /// <summary>Clears the cache of configuration entries. (ConfigurationEntries)
        /// Re-accessing the ConfigurationEntries property rereads all configuration entries from the database.</summary>
        public void ClearCacheOfConfigurationEntries()
        {
            using (ACMonitor.Lock(_11020_LockValue))
            {
                _ACConfigListCache = null;
            }
        }

        /// <summary>
        /// Checks if cached configuration entries are loaded from database successfully
        /// </summary>
        public bool ValidateConfigurationEntriesWithDB(ConfigEntriesValidationMode mode = ConfigEntriesValidationMode.AnyCheck)
        {
            if (mode == ConfigEntriesValidationMode.AnyCheck)
            {
                if (ConfigurationEntries.Any())
                    return true;
            }
            using (DatabaseApp database = new DatabaseApp())
            {
                var query = database.MaterialConfig.Where(c => c.MaterialID == this.MaterialID);
                if (mode == ConfigEntriesValidationMode.AnyCheck)
                {
                    if (query.Any())
                        return false;
                }
                else if (mode == ConfigEntriesValidationMode.CompareCount
                         || mode == ConfigEntriesValidationMode.CompareContent)
                    return query.Count() == ConfigurationEntries.Count();
            }
            return true;
        }

        #endregion

        #region VBIplus-Context
        private gip.core.datamodel.ACClassMethod _ProgramACClassMethod;
        [ACPropertyInfo(9999, "", "en{'Program Method'}de{'Programmmethode'}", Const.ContextDatabaseIPlus + "\\ACClassMethod")]
        public gip.core.datamodel.ACClassMethod ProgramACClassMethod
        {
            get
            {
                if (this.VBiProgramACClassMethodID == null || this.VBiProgramACClassMethodID == Guid.Empty)
                    return null;
                if (_ProgramACClassMethod != null)
                    return _ProgramACClassMethod;
                if (this.VBiProgramACClassMethod == null)
                {
                    DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                    _ProgramACClassMethod = dbApp.ContextIPlus.ACClassMethod.Where(c => c.ACClassMethodID == this.VBiProgramACClassMethodID).FirstOrDefault();
                    return _ProgramACClassMethod;
                }
                else
                {
                    _ProgramACClassMethod = this.VBiProgramACClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>();
                    return _ProgramACClassMethod;
                }
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiProgramACClassMethod == null)
                        return;
                    _ProgramACClassMethod = null;
                    this.VBiProgramACClassMethod = null;
                }
                else
                {
                    if (_ProgramACClassMethod != null && value == _ProgramACClassMethod)
                        return;
                    gip.mes.datamodel.ACClassMethod value2 = value.FromAppContext<gip.mes.datamodel.ACClassMethod>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiProgramACClassMethodID = value.ACClassMethodID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _ProgramACClassMethod = value;
                    if (value2 == this.VBiProgramACClassMethod)
                        return;
                    this.VBiProgramACClassMethod = value2;
                }
            }
        }

        partial void OnVBiProgramACClassMethodIDChanged()
        {
            OnPropertyChanged("ProgramACClassMethod");
        }

        private gip.core.datamodel.ACClass _StackCalculatorACClass;
        [ACPropertyInfo(9999, "", "en{'Stack Posting Type'}de{'Stapelbuchungsart'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClass.ClassName)]
        public gip.core.datamodel.ACClass StackCalculatorACClass
        {
            get
            {
                if (this.VBiStackCalculatorACClassID == null || this.VBiStackCalculatorACClassID == Guid.Empty)
                    return null;
                if (_StackCalculatorACClass != null)
                    return _StackCalculatorACClass;
                if (this.VBiStackCalculatorACClass == null)
                {
                    DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                    _StackCalculatorACClass = dbApp.ContextIPlus.ACClass.Where(c => c.ACClassID == this.VBiStackCalculatorACClassID).FirstOrDefault();
                    return _StackCalculatorACClass;
                }
                else
                {
                    _StackCalculatorACClass = this.VBiStackCalculatorACClass.FromIPlusContext<gip.core.datamodel.ACClass>();
                    return _StackCalculatorACClass;
                }
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiStackCalculatorACClass == null)
                        return;
                    _StackCalculatorACClass = null;
                    this.VBiStackCalculatorACClass = null;
                }
                else
                {
                    if (_StackCalculatorACClass != null && value == _StackCalculatorACClass)
                        return;
                    gip.mes.datamodel.ACClass value2 = value.FromAppContext<gip.mes.datamodel.ACClass>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiStackCalculatorACClassID = value.ACClassID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _StackCalculatorACClass = value;
                    if (value2 == this.VBiStackCalculatorACClass)
                        return;
                    this.VBiStackCalculatorACClass = value2;
                }
            }
        }

        partial void OnVBiStackCalculatorACClassIDChanged()
        {
            OnPropertyChanged("StackCalculatorACClass");
        }

        #endregion

        #region Others

        public int MixingLevel { get; set; }

        #endregion

        #region IACWorkflowNode Members

        /// <summary>
        /// All edges that starts from this node
        /// </summary>
        /// <param name="context">Workflowcontext. (Main Entity Class that saves the Workflow and the Nodes and Edges are related to)</param>
        /// <value>List of edges</value>
        public IEnumerable<IACWorkflowEdge> GetOutgoingWFEdges(IACWorkflowContext context)
        {
            MaterialWF materialWF = context as MaterialWF;
            if (materialWF == null)
                return new IACWorkflowEdge[] { };
            return this.MaterialWFRelation_SourceMaterial.Where(c => c.MaterialWFID == materialWF.MaterialWFID);
        }

        /// <summary>
        /// If this Node is a Workflow-Group, this property returns all outgoing-edges belonging to this node.
        /// </summary>
        /// <param name="context">Workflowcontext. (Main Entity Class that saves the Workflow and the Nodes and Edges are related to)</param>
        /// <value>List of edges</value>
        public IEnumerable<IACWorkflowEdge> GetOutgoingWFEdgesInGroup(IACWorkflowContext context)
        {
            return new IACWorkflowEdge[] { };
        }

        /// <summary>
        /// All edges that ends in this node
        /// </summary>
        /// <param name="context">Workflowcontext. (Main Entity Class that saves the Workflow and the Nodes and Edges are related to)</param>
        /// <value>List of edges</value>
        public IEnumerable<IACWorkflowEdge> GetIncomingWFEdges(IACWorkflowContext context)
        {
            MaterialWF materialWF = context as MaterialWF;
            if (materialWF == null)
                return new IACWorkflowEdge[] { };
            return this.MaterialWFRelation_TargetMaterial.Where(c => c.MaterialWFID == materialWF.MaterialWFID);
        }

        /// <summary>
        /// If this Node is a Workflow-Group, this property returns all incoming-edges belonging to this node.
        /// </summary>
        /// <param name="context">Workflowcontext. (Main Entity Class that saves the Workflow and the Nodes and Edges are related to)</param>
        /// <value>List of edges</value>
        public IEnumerable<IACWorkflowEdge> GetIncomingWFEdgesInGroup(IACWorkflowContext context)
        {
            return new IACWorkflowEdge[] { };
        }

        /// <summary>
        /// Returns true if this Node is a Workflow-Group and is the most outer node.
        /// </summary>
        /// <param name="context">Workflowcontext. (Main Entity Class that saves the Workflow and the Nodes and Edges are related to)</param>
        /// <value><c>true</c> if this Node is a Workflow-Group and is the most outer node; otherwise, <c>false</c>.</value>
        public bool IsRootWFNode(IACWorkflowContext context)
        {
            return false;
        }


        /// <summary>
        /// If this Node is a Workflow-Group, this method returns all nnodes that are inside of this group.
        /// </summary>
        /// <param name="context">Workflowcontext. (Main Entity Class that saves the Workflow and the Nodes and Edges are related to)</param>
        /// <value>List of nodes</value>
        public IEnumerable<IACWorkflowNode> GetChildWFNodes(IACWorkflowContext context)
        {
            MaterialWF materialWF = context as MaterialWF;
            if (materialWF == null)
                return new IACWorkflowNode[] { };
            return MaterialWFRelation_TargetMaterial.Where(c => c.SourceMaterial == this && c.MaterialWFID == materialWF.MaterialWFID).Select(c => c.SourceMaterial);
        }

        /// <summary>
        /// Returns the ACClassProperty that reprensents a Connection-Point where Edges can be connected to.
        /// </summary>
        /// <param name="acPropertyName">Name of the property.</param>
        /// <returns>ACClassProperty.</returns>
        public core.datamodel.ACClassProperty GetConnector(string acPropertyName)
        {
            return PWACClass.GetPoint(acPropertyName);
        }

        /// <summary>
        /// Returns null
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>null</value>
        public override IACObject ParentACObject
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a ACUrl, to be able to find this instance in the WPF-Logical-Tree.
        /// </summary>
        /// <value>ACUrl as string</value>
        public string VisualACUrl
        {
            get
            {
                return this.ACIdentifier;
            }
        }


        /// <summary>
        /// The Runtime-type of the Workflow-Class that will be instantiated when the Workflow is loaded.
        /// </summary>
        /// <value>Reference to a ACClass</value>
        public core.datamodel.ACClass PWACClass
        {
            get
            {
                return this.GetObjectContext().ContextIPlus.GetACType(typeof(PWMaterial));
            }
        }

        /// <summary>
        /// Unique ID of the Workflow Node
        /// </summary>
        /// <value>Returns MaterialID</value>
        public Guid WFObjectID
        {
            get { return MaterialID; }
        }

        /// <summary>
        /// Reference to the parent Workflow-Node that groups more child-nodes together
        /// </summary>
        /// <value>Parent Workflow-Node (Group)</value>
        public IACWorkflowNode WFGroup
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// WPF's x:Name to indentify this instance in the Logical-Tree
        /// </summary>
        /// <value>x:Name (WPF)</value>
        public string XName
        {
            get
            {
                string xName = this.ACIdentifier.Replace("(", "");
                xName = xName.Replace(")", "");
                xName = xName.Replace('-', '_');
                xName = xName.Replace('+', '_');
                return xName;
            }
        }
        #endregion

        #region Design
        /// <summary>Returns a ACClassDesign for presenting itself on the gui</summary>
        /// <param name="acUsage">Filter for selecting designs that belongs to this ACUsages-Group</param>
        /// <param name="acKind">Filter for selecting designs that belongs to this ACKinds-Group</param>
        /// <param name="vbDesignName">Optional: The concrete acIdentifier of the design</param>
        /// <returns>ACClassDesign</returns>
        public core.datamodel.ACClassDesign GetDesign(Global.ACUsages acUsage, Global.ACKinds acKind, string vbDesignName = "")
        {
            return PWACClass.ACType.GetDesign(PWACClass, acUsage, acKind, vbDesignName);
        }

        public IACObject GetParentACObject(IACObject context)
        {
            if (context is MaterialWF)
                return context as MaterialWF;
            else
                return null;
        }
        #endregion

        #region Image

        private string _DefaultImage;
        /// <summary>
        /// Doc  DefaultImage
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "DefaultImage", "en{'Image'}de{'Bild'}")]
        public string DefaultImage
        {
            get
            {
                return _DefaultImage;
            }
            set
            {
                if (_DefaultImage != value)
                {
                    _DefaultImage = value;
                    OnPropertyChanged("DefaultImage");
                }
            }
        }


        private string _DefaultThumbImage;
        /// <summary>
        /// Doc  DefaultThumbImage
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "DefaultThumbImage", "en{'Image'}de{'Bild'}")]
        public string DefaultThumbImage
        {
            get
            {
                return _DefaultThumbImage;
            }
            set
            {
                if (_DefaultThumbImage != value)
                {
                    _DefaultThumbImage = value;
                    OnPropertyChanged("DefaultThumbImage");
                }
            }
        }

        #endregion

        #region Cloning
        public object Clone()
        {
            Material clonedObject = new Material();
            clonedObject.MaterialID = this.MaterialID;
            clonedObject.CopyFrom(this, true);
            return clonedObject;
        }

        public void CopyFrom(Material from, bool withReferences)
        {
            if (withReferences)
            {
                MDMaterialGroupID = from.MDMaterialGroupID;
                MDMaterialTypeID = from.MDMaterialTypeID;
                MDFacilityManagementTypeID = from.MDFacilityManagementTypeID;
                InFacilityID = from.InFacilityID;
                OutFacilityID = from.OutFacilityID;
                VBiStackCalculatorACClassID = from.VBiStackCalculatorACClassID;
                MDInventoryManagementTypeID = from.MDInventoryManagementTypeID;
                MDGMPMaterialGroupID = from.MDGMPMaterialGroupID;
                ProductionMaterialID = from.ProductionMaterialID;
                VBiProgramACClassMethodID = from.VBiProgramACClassMethodID;

                // LabelID = from.LabelID;
                BaseMDUnitID = from.BaseMDUnitID;
                VBiProgramACClassMethodID = from.VBiProgramACClassMethodID;
            }

            MaterialNo = from.MaterialNo;
            MaterialName1 = from.MaterialName1;
            MaterialName2 = from.MaterialName2;
            MaterialName3 = from.MaterialName3;

            MinStockQuantity = from.MinStockQuantity;
            OptStockQuantity = from.OptStockQuantity;
            DontAllowNegativeStock = from.DontAllowNegativeStock;

            StorageLife = from.StorageLife;
            UsageInOrder = from.UsageInOrder;
            UsageOutOrder = from.UsageOutOrder;
            UsageACProgram = from.UsageACProgram;
            UsageOwnProduct = from.UsageOwnProduct;
            IsActive = from.IsActive;
            ContractorStock = from.ContractorStock;

            NetWeight = from.NetWeight;
            GrossWeight = from.GrossWeight;
            ProductionWeight = from.ProductionWeight;
            Density = from.Density;
            Comment = from.Comment;
            XMLConfig = from.XMLConfig;
            PetroleumGroupIndex = from.PetroleumGroupIndex;
            DensityAmb = from.DensityAmb;
            IsIntermediate = from.IsIntermediate;
            ZeroBookingTolerance = from.ZeroBookingTolerance;
            RetrogradeFIFO = from.RetrogradeFIFO;
            ExplosionOff = from.ExplosionOff;
            SpecHeatCapacity = from.SpecHeatCapacity;
            KeyOfExtSys = from.KeyOfExtSys;
            Anterograde = from.Anterograde;
        }
        #endregion
    }
}
