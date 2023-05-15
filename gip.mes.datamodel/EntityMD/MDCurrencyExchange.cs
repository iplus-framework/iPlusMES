using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.ESCurrencyExchange, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "ToMDCurrency", "en{'To Currency'}de{'Nach Währung'}", Const.ContextDatabase + "\\" + MDCurrency.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, "ExchangeRate", ConstApp.ESCurrencyExchange, "", "", true)]
    [ACPropertyEntity(3, "ExchangeNo", "en{'Exchange number'}de{'Wechselkursnummer'}", "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + MDCurrencyExchange.ClassName, ConstApp.ESCurrencyExchange, typeof(MDCurrencyExchange), MDCurrencyExchange.ClassName, "ToMDCurrency\\MDCurrencyShortname", "ToMDCurrency\\MDCurrencyShortname")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDCurrencyExchange>) })]
    [NotMapped]
    public partial class MDCurrencyExchange : IACObjectEntity
    {
        [NotMapped]
        public const string ClassName = "MDCurrencyExchange";

        #region New/Delete
        public static MDCurrencyExchange NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            // Bei Systembelegung gibt es keine Vorbelegung, da hier kein Customizing erwünscht ist
            MDCurrencyExchange entity = new MDCurrencyExchange();
            entity.MDCurrencyExchangeID = Guid.NewGuid();
            if (parentACObject is MDCurrency)
            {
                entity.MDCurrency = parentACObject as MDCurrency;
            }
            entity.ExchangeRate = 1;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        /// <summary>
        /// Returns MDCurrency
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to MDCurrency</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return MDCurrency;
            }
        }

        #endregion

        #region IACObjectEntity Members
        /// <summary>
        /// Method for validating values and references in this EF-Object.
        /// Is called from Change-Tracking before changes will be saved for new unsaved entity-objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">Entity-Framework databasecontext</param>
        /// <returns>NULL if sucessful otherwise a Message-List</returns>
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            if (ToMDCurrency == null)
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "ToMDCurrency",
                    Message = "ToMDCurrency",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "ToMDCurrency"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "MDCurrency\\MDKey,ToMDCurrency\\MDKey";
            }
        }

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
                return ToMDCurrency?.MDCurrencyShortname;
            }
        }
        #endregion

        #region Methods
        public decimal ConvertToForeignCurrency(decimal localValue)
        {
            if (ExchangeRate <= 0.00000000001 || localValue == 0)
                return 0;
            return Convert.ToDecimal(Math.Round(Convert.ToDouble(localValue) / ExchangeRate, 2));
        }

        public decimal ConvertBackToLocalCurrency(decimal foreignValue)
        {
            if (ExchangeRate <= 0.00000000001 || foreignValue == 0)
                return 0;
            return Convert.ToDecimal(Math.Round(Convert.ToDouble(foreignValue) * ExchangeRate, 2));
        }
        #endregion

    }
}
