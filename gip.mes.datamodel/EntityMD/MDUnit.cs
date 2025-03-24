using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.ESUnit, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOUnit")]
    [ACPropertyEntity(3, "TechnicalSymbol", ConstApp.TechnicalSymbol, "", "", true)]
    [ACPropertyEntity(4, "SIDimensionIndex", "en{'Dimension'}de{'Maß'}", typeof(GlobalApp.SIDimensions), Const.ContextDatabase + "\\SIDimensionList", "", true)]
    [ACPropertyEntity(5, "IsSIUnit", "en{'SI-Unit'}de{'Ist SI-Einheit'}", "", "", true, DefaultValue = false)]
    [ACPropertyEntity(6, "Rounding", "en{'Rounding'}de{'Genauigkeit'}", "", "", true, DefaultValue = (int)-1)]
    [ACPropertyEntity(7, "IsQuantityUnit", "en{'Commercial Unit'}de{'Kommerz. Einheit'}", "", "", true)]
    [ACPropertyEntity(8, "ISOCode", "en{'ISO Code'}de{'ISO-Code'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(9, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(9999, "MDUnitNameTrans", "en{'Name'}de{'Bezeichnung'}", "", "", true)]
    [ACPropertyEntity(10, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MDUnit.ClassName, ConstApp.ESUnit, typeof(MDUnit), MDUnit.ClassName, "ISOCode", Const.SortIndex, new object[]
        {
                new object[] {Const.QueryPrefix + MDUnitConversion.ClassName, ConstApp.ESUnitConversion, typeof(MDUnitConversion), MDUnitConversion.ClassName + "_" + MDUnit.ClassName, "ToMDUnit\\MDUnitName", "ToMDUnit\\MDUnitName"}
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDUnit>) })]
    [NotMapped]
    public partial class MDUnit
    {
        [NotMapped]
        public const string ClassName = "MDUnit";

        #region New/Delete
        public static MDUnit NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDUnit entity = new MDUnit();
            entity.MDUnitID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.SIDimensionIndex = (short)GlobalApp.SIDimensions.None;
            //entity.IsSIUnit = false;
            //entity.Rounding = 0;
            //entity.IsQuantityUnit = true;
            entity.SortIndex = System.Convert.ToInt16(dbApp.MDUnit.Count() + 1);
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            return ACCaption;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return MDUnitName;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "ISOCode";
            }
        }
        #endregion

        #region IEntityProperty Members

        [NotMapped]
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

        #region Additional Properties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDUnitName
        {
            get
            {
                return Translator.GetTranslation(MDUnitNameTrans);
            }
            set
            {
                MDUnitNameTrans = Translator.SetTranslation(MDUnitNameTrans, value);
                OnPropertyChanged("MDUnitName");
            }
        }

        [ACPropertyInfo(2, "", "en{'Symbol comm.'}de{'Zeichen kommerz.'}", MinLength = 1)]
        [NotMapped]
        public String Symbol
        {
            get
            {
                return Translator.GetTranslation(SymbolTrans);
            }
            set
            {
                SymbolTrans = Translator.SetTranslation(SymbolTrans, value);
                OnPropertyChanged("Symbol");
            }
        }

        /// <summary>
        /// Gets or sets the Dimension
        /// </summary>
        /// <value>Dimension</value>
        [NotMapped]
        public GlobalApp.SIDimensions SIDimension
        {
            get
            {
                return (GlobalApp.SIDimensions)SIDimensionIndex;
            }
            set
            {
                SIDimensionIndex = (Int16)value;
            }
        }

        /// <summary>
        /// Gets the MDUnit which is a SIUnit if UOM has a Dimension
        /// e.g. this is gram and returns kilogram
        /// e.g. this is kilometre and returns metre
        /// </summary>
        [NotMapped]
        public MDUnit SIUnit
        {
            get
            {
                if (this.SIDimension == GlobalApp.SIDimensions.None)
                    return null;
                else if (this.IsSIUnit)
                    return this;
                else
                {
                    if (this.MDUnitConversion_ToMDUnit.Count > 0)
                        return this.MDUnitConversion_ToMDUnit.First().MDUnit;
                    var query = this.GetObjectContext<DatabaseApp>().MDUnit.Where(c => c.SIDimensionIndex == this.SIDimensionIndex && c.IsSIUnit == true);
                    if (query.Any())
                        return query.First();
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns a list in which MDUnits this Unit can be converted
        /// </summary>
        [NotMapped]
        public List<MDUnit> ConvertableUnits
        {
            get
            {
                MDUnit fromUnit = this;
                if (this.SIDimension != GlobalApp.SIDimensions.None)
                    fromUnit = SIUnit;
                if (fromUnit == null || fromUnit.MDUnitConversion_MDUnit == null)
                    return new List<MDUnit>();
                else
                {
                    if (fromUnit.IsSIUnit)
                    {
                        // this is not SI-Unit
                        if (this.MDUnitID != fromUnit.MDUnitID)
                        {
                            List<MDUnit> result = fromUnit.MDUnitConversion_MDUnit.Where(c => c.ToMDUnit.MDUnitID != this.MDUnitID).Select(c => c.ToMDUnit).ToList();
                            result.Add(fromUnit);
                            return result;
                        }
                        // this is SI-Unit
                        else
                        {
                            return fromUnit.MDUnitConversion_MDUnit.Select(c => c.ToMDUnit).ToList();
                        }
                    }
                    else
                    {
                        return fromUnit.MDUnitConversion_MDUnit.Select(c => c.ToMDUnit).ToList();
                    }
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Converts the passed quantity measured in this Unit
        /// INTO the passed unit
        /// </summary>
        /// <param name="quantityThis">Quantity measured in this unit</param>
        /// <param name="toMDUnit">Unit into the passed quantity shoulb be converted</param>
        /// <returns>quantity measured in toMDUnit</returns>
        /// <exception cref="ArgumentException">Thrown if conversion not possible because of incompatible units</exception>
        /// <exception cref="ArgumentNullException">Thrown if mdUnit is null</exception>
        public Double ConvertToUnit(Double quantityThis, MDUnit toMDUnit)
        {
            if (toMDUnit == null)
                throw new ArgumentNullException("toMDUnit");
            if (toMDUnit.MDUnitID == this.MDUnitID)
                return quantityThis;
            if (Double.IsNaN(quantityThis) || Double.IsInfinity(quantityThis))
                return 0;
            if (Math.Abs(quantityThis - 0) <= Double.Epsilon)
                return 0;
            List<MDUnit> convertableUnits = this.ConvertableUnits;
            if (!convertableUnits.Any())
            {
                throw new ArgumentException("Not convertable " + this.MDUnitName + "->" + toMDUnit.MDUnitName);
            }
            if (this.SIDimension != GlobalApp.SIDimensions.None)
            {
                Double quantityInSIUnit = quantityThis;
                MDUnit siUnit = SIUnit;
                if (!this.IsSIUnit || (siUnit.MDUnitID != this.MDUnitID))
                {
                    var query = siUnit.MDUnitConversion_MDUnit.Where(c => c.ToMDUnitID == this.MDUnitID);
                    if (!query.Any())
                    {
                        throw new ArgumentException("Not convertable " + this.MDUnitName + "->" + toMDUnit.MDUnitName);
                    }
                    MDUnitConversion conversionThis = query.First();
                    quantityInSIUnit = quantityThis * conversionThis.Multiplier / conversionThis.Divisor;
                }

                if (siUnit.MDUnitID == toMDUnit.MDUnitID)
                {
                    if (toMDUnit.Rounding >= 0)
                        quantityInSIUnit = toMDUnit.GetRoundedValue(quantityInSIUnit);
                    return quantityInSIUnit;
                }

                var query2 = siUnit.MDUnitConversion_MDUnit.Where(c => c.ToMDUnitID == toMDUnit.MDUnitID);
                if (!query2.Any())
                {
                    throw new ArgumentException("Not convertable " + this.MDUnitName + "->" + toMDUnit.MDUnitName);
                }
                MDUnitConversion conversionTo = query2.First();
                double result = quantityInSIUnit * conversionTo.Divisor / conversionTo.Multiplier;
                if (toMDUnit.Rounding >= 0)
                    result = toMDUnit.GetRoundedValue(result);
                return result;
            }
            else
            {
                var query = this.MDUnitConversion_MDUnit.Where(c => c.ToMDUnitID == toMDUnit.MDUnitID);
                MDUnitConversion conversionTo = null;
                MDUnit siUnit = null;
                if (!query.Any())
                {
                    if (!toMDUnit.IsSIUnit && toMDUnit.SIDimension != GlobalApp.SIDimensions.None && toMDUnit.SIUnit != null)
                    {
                        query = this.MDUnitConversion_MDUnit.Where(c => c.ToMDUnitID == toMDUnit.SIUnit.MDUnitID);
                        if (!query.Any())
                            throw new ArgumentException("Not convertable" + this.MDUnitName + "->" + toMDUnit.MDUnitName);
                        conversionTo = query.First();
                        siUnit = toMDUnit.SIUnit;
                    }
                    else
                        throw new ArgumentException("Not convertable" + this.MDUnitName + "->" + toMDUnit.MDUnitName);
                }
                else
                    conversionTo = query.First();
                double result = quantityThis * conversionTo.Divisor / conversionTo.Multiplier;
                if (siUnit != null)
                    result = siUnit.ConvertToUnit(result, toMDUnit);
                if (toMDUnit.Rounding >= 0)
                    result = toMDUnit.GetRoundedValue(result);
                return result;
            }
        }

        public bool IsConvertableToUnit(MDUnit toMDUnit)
        {
            if (toMDUnit == null)
                return false;
            if (toMDUnit.MDUnitID == this.MDUnitID)
                return true;
            List<MDUnit> convertableUnits = this.ConvertableUnits;
            if (convertableUnits.Count <= 0)
                return false;
            if (this.SIDimension != GlobalApp.SIDimensions.None)
            {
                MDUnit siUnit = SIUnit;
                if (!this.IsSIUnit || (siUnit.MDUnitID != this.MDUnitID))
                {
                    var query = siUnit.MDUnitConversion_MDUnit.Where(c => c.ToMDUnitID == this.MDUnitID);
                    if (!query.Any())
                        return false;
                }

                if (siUnit.MDUnitID == toMDUnit.MDUnitID)
                    return true;

                var query2 = siUnit.MDUnitConversion_MDUnit.Where(c => c.ToMDUnitID == toMDUnit.MDUnitID);
                if (!query2.Any())
                    return false;
                return true;
            }
            else
            {
                var query = this.MDUnitConversion_MDUnit.Where(c => c.ToMDUnitID == toMDUnit.MDUnitID);
                if (!query.Any())
                {
                    if (!toMDUnit.IsSIUnit && toMDUnit.SIDimension != GlobalApp.SIDimensions.None && toMDUnit.SIUnit != null)
                    {
                        query = this.MDUnitConversion_MDUnit.Where(c => c.ToMDUnitID == toMDUnit.SIUnit.MDUnitID);
                        if (!query.Any())
                            return false;
                    }
                    else
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Converts the passed quantity maesured in the passed unit
        /// INTO this unit
        /// </summary>
        /// <param name="quantityFrom"></param>
        /// <param name="fromMDUnit"></param>
        /// <returns></returns>
        public Double ConvertFromUnit(Double quantityFrom, MDUnit fromMDUnit)
        {
            if (fromMDUnit == null)
                throw new ArgumentNullException("fromMDUnit");
            if (fromMDUnit.MDUnitID == this.MDUnitID)
                return quantityFrom;
            if (Double.IsNaN(quantityFrom) || Double.IsInfinity(quantityFrom))
                return 0;
            if (Math.Abs(quantityFrom - 0) <= Double.Epsilon)
                return 0;
            List<MDUnit> convertableUnits = this.ConvertableUnits;
            if (!convertableUnits.Any())
            {
                throw new ArgumentException("Not convertable " + this.MDUnitName + "->" + fromMDUnit.MDUnitName);
            }
            if (this.SIDimension != GlobalApp.SIDimensions.None)
            {
                return fromMDUnit.ConvertToUnit(quantityFrom, this);
                //return result;
            }
            else
            {
                var query = this.MDUnitConversion_MDUnit.Where(c => c.ToMDUnitID == fromMDUnit.MDUnitID);
                MDUnitConversion conversionTo = null;
                if (!query.Any())
                {
                    if (!fromMDUnit.IsSIUnit && fromMDUnit.SIDimension != GlobalApp.SIDimensions.None && fromMDUnit.SIUnit != null)
                    {
                        query = this.MDUnitConversion_MDUnit.Where(c => c.ToMDUnitID == fromMDUnit.SIUnit.MDUnitID);
                        if (!query.Any())
                        {
                            throw new ArgumentException("Not convertable " + fromMDUnit.MDUnitName + "->" + this.MDUnitName);
                        }
                        quantityFrom = fromMDUnit.ConvertToUnit(quantityFrom, fromMDUnit.SIUnit);
                        fromMDUnit = fromMDUnit.SIUnit;
                        conversionTo = query.First();
                    }
                    else
                        throw new ArgumentException("Not convertable " + fromMDUnit.MDUnitName + "->" + this.MDUnitName);
                }
                else
                    conversionTo = query.First();
                double result = (quantityFrom / conversionTo.Divisor) * conversionTo.Multiplier;
                if (fromMDUnit.Rounding >= 0)
                    result = fromMDUnit.GetRoundedValue(result);
                return result;
            }
        }

        public bool IsConvertableFromUnit(MDUnit fromMDUnit)
        {
            if (fromMDUnit == null)
                return false;
            if (fromMDUnit.MDUnitID == this.MDUnitID)
                return true;
            List<MDUnit> convertableUnits = this.ConvertableUnits;
            if (convertableUnits.Count <= 0)
                return false;
            if (this.SIDimension != GlobalApp.SIDimensions.None)
            {
                return fromMDUnit.IsConvertableToUnit(this);
            }
            else
            {
                var query = this.MDUnitConversion_MDUnit.Where(c => c.ToMDUnitID == fromMDUnit.MDUnitID);
                if (!query.Any())
                {
                    if (!fromMDUnit.IsSIUnit && fromMDUnit.SIDimension != GlobalApp.SIDimensions.None && fromMDUnit.SIUnit != null)
                    {
                        query = this.MDUnitConversion_MDUnit.Where(c => c.ToMDUnitID == fromMDUnit.SIUnit.MDUnitID);
                        if (!query.Any())
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                return true;
            }
        }


        public static bool IsConvertableToUnit(MDUnit fromMDUnit, MDUnit toMDUnit)
        {
            if (fromMDUnit == null)
                return false;
            return fromMDUnit.IsConvertableToUnit(toMDUnit);
        }


        //static readonly Func<DatabaseApp, IEnumerable<MDUnit>> s_cQry_Default =
        //    EF.CompileQuery<DatabaseApp, IEnumerable<MDUnit>>(
        //    (database) => from c in database.MDUnit where c.IsDefault select c
        //);

        static readonly Func<DatabaseApp, short, IEnumerable<MDUnit>> s_cQry_SiUnit =
            EF.CompileQuery<DatabaseApp, short, IEnumerable<MDUnit>>(
            (database, index) => from c in database.MDUnit where c.SIDimensionIndex == index && c.IsSIUnit select c
        );

        public static MDUnit GetSIUnit(DatabaseApp dbApp, GlobalApp.SIDimensions forDimension)
        {
            if (forDimension == GlobalApp.SIDimensions.None)
                return null;
            if (dbApp == null)
                return null;
            return s_cQry_SiUnit(dbApp, (short)forDimension).FirstOrDefault();
        }

        public double GetRoundedValue(double value)
        {
            if (Rounding <= -1)
                return value;
            return Math.Round(value, Rounding);
        }

        #endregion

    }
}




