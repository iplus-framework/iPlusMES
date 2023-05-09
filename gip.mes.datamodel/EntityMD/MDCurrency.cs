using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.ESCurrency, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOCurrency")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(1, "MDCurrencyShortname", ConstApp.ESCurrency, "", "", true, MinLength = 1)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + MDCurrency.ClassName, ConstApp.ESCurrency, typeof(MDCurrency), MDCurrency.ClassName, Const.MDNameTrans, "MDCurrencyShortname", new object[]
        {
                new object[] {Const.QueryPrefix + MDCurrencyExchange.ClassName, ConstApp.ESCurrencyExchange, typeof(MDCurrencyExchange), MDCurrencyExchange.ClassName + "_" + MDCurrency.ClassName, "ToMDCurrency\\MDCurrencyShortname", "ToMDCurrency\\MDCurrencyShortname"}
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDCurrency>) })]
    public partial class MDCurrency : IACObjectEntity
    {
        public const string ClassName = "MDCurrency";

        #region New/Delete
        public static MDCurrency NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDCurrency entity = new MDCurrency();
            entity.MDCurrencyID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDCurrency>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDCurrency>>(
            (database) => from c in database.MDCurrency where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, string, IQueryable<MDCurrency>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, string, IQueryable<MDCurrency>>(
            (database, index) => from c in database.MDCurrency where c.MDCurrencyShortname == index select c
        );

        public static MDCurrency DefaultMDCurrency(DatabaseApp dbApp)
        {
            try
            {
                MDCurrency defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, "EUR").FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDCurrency", "DefaultMDCurrency", msg);
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
                return MDCurrencyName;
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
            if (filterValues.Any() && className == Const.MDKey)
                return this.MDCurrencyExchange_MDCurrency.Where(c => c.ToMDCurrency.MDKey == filterValues[0]).FirstOrDefault();
            return null;
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return Const.MDKey;
            }
        }

        #endregion

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDCurrencyName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDCurrencyName");
            }
        }

        public MDCurrencyExchange GetExchangeRate(MDCurrency toCurrency, DateTime invoiceDate)
        {
            if (toCurrency == null)
                return null;
            DateTime dateTimeFrom = new DateTime(invoiceDate.Year, invoiceDate.Month, invoiceDate.Day);
            DateTime dateTimeTo = dateTimeFrom.AddDays(1);
            return MDCurrencyExchange_MDCurrency.Where(c => c.ToMDCurrencyID == toCurrency.MDCurrencyID && c.InsertDate >= dateTimeFrom && c.InsertDate <= dateTimeTo).FirstOrDefault();
            //return null;
        }

        public IEnumerable<MDCurrencyExchange> GetAlternativeExchangeRates(DateTime invoiceDate)
        {
            DateTime dateTimeFrom = new DateTime(invoiceDate.Year, invoiceDate.Month, invoiceDate.Day);
            DateTime dateTimeTo = dateTimeFrom.AddDays(1);
            return MDCurrencyExchange_MDCurrency.Where(c => c.InsertDate >= dateTimeFrom && c.InsertDate <= dateTimeTo);
        }

        #endregion

    }
}
