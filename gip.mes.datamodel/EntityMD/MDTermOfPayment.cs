using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Transactions; using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.ESTermOfPayment, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOTermOfPayment")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans,  "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey,  "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.IsDefault, Const.EntityIsDefault,"", "", true)]
    [ACPropertyEntity(3, "TermOfPaymentDays", "en{'Payment Term (days)'}de{'Zahlungsziel (Tage)'}","", "", true)]
    [ACPropertyEntity(4, "Description", "en{'Description'}de{'Beschreibung'}","", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + MDTermOfPayment.ClassName, ConstApp.ESTermOfPayment, typeof(MDTermOfPayment), MDTermOfPayment.ClassName, Const.MDNameTrans, Const.MDKey)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDTermOfPayment>) })]
    public partial class MDTermOfPayment
    {
        public const string ClassName = "MDTermOfPayment";

        #region New/Delete
        public static MDTermOfPayment NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDTermOfPayment entity = new MDTermOfPayment();
            entity.MDTermOfPaymentID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDTermOfPayment>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDTermOfPayment>>(
            (database) => from c in database.MDTermOfPayment where c.IsDefault select c
        );

        public static MDTermOfPayment DefaultMDTermOfPayment(DatabaseApp dbApp)
        {
            try
            {
                MDTermOfPayment defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException(ClassName, "Default" + ClassName, msg);
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
                return MDTermOfPaymentName;
            }
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
        public String MDTermOfPaymentName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDTermOfPaymentName");
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

    }
}
