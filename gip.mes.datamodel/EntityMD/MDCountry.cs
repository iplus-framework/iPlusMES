using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.ESCountry, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOCountry")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, "CountryCode", "en{'Country-Code'}de{'Ländercode'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(3, "IsEUMember", "en{'EU-Member'}de{'EU-Mitglied'}", "", "", true)]
    [ACPropertyEntity(4, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(5, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(6, MDCurrency.ClassName, "en{'Currency'}de{'Währung'}", Const.ContextDatabase + "\\" + MDCurrency.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + MDCountry.ClassName, ConstApp.ESCountry, typeof(MDCountry), MDCountry.ClassName, "CountryCode", Const.SortIndex, new object[]
        {
            new object[] {Const.QueryPrefix + MDCountryLand.ClassName, ConstApp.ESCountryLand, typeof(MDCountryLand), MDCountryLand.ClassName + "_" + MDCountry.ClassName, Const.MDKey, Const.MDKey}
        }

    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDCountry>) })]
    [NotMapped]
    public partial class MDCountry
    {
        [NotMapped]
        public const string ClassName = "MDCountry";

        #region New/Delete
        public static MDCountry NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDCountry entity = new MDCountry();
            entity.MDCountryID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IEnumerable<MDCountry>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IEnumerable<MDCountry>>(
            (database) => from c in database.MDCountry where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, string, IEnumerable<MDCountry>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, string, IEnumerable<MDCountry>>(
            (database, index) => from c in database.MDCountry where c.CountryCode == index select c
        );

        public static MDCountry DefaultMDCountry(DatabaseApp dbApp)
        {
            try
            {
                MDCountry defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, "en").FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDCountry", "DefaultMDCountry", msg);
                return null;
            }
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
                return MDCountryName;
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
            if (filterValues.Any() && className == MDCountry.ClassName)
                return this.MDCountryLand_MDCountry.Where(c => c.MDKey == filterValues[0]).FirstOrDefault();
            return null;
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return Const.MDKey;
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

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDCountryName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDCountryName");
            }
        }

#endregion
    }
}
