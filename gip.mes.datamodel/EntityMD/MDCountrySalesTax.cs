using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.ESCountrySalesTax, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(9999, MDCountry.ClassName, ConstApp.ESCountry, Const.ContextDatabase + "\\" + MDCountry.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(1, "SalesTax", ConstApp.ESCountrySalesTax, "", "", true)]
    [ACPropertyEntity(2, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(3, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(4, "DateFrom", "en{'Start time'}de{'Startzeit'}", "", "", true)]
    [ACPropertyEntity(5, "DateTo", "en{'End Time'}de{'Endzeit'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + MDCountrySalesTax.ClassName, ConstApp.ESCountrySalesTax, typeof(MDCountrySalesTax), MDCountrySalesTax.ClassName, Const.MDNameTrans, Const.MDKey)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDCountrySalesTax>) })]
    [NotMapped]
    public partial class MDCountrySalesTax : IACObjectEntity
    {
        [NotMapped]
        public const string ClassName = "MDCountrySalesTax";

        #region New/Delete
        public static MDCountrySalesTax NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            // Bei Systembelegung gibt es keine Vorbelegung, da hier kein Customizing erwünscht ist
            MDCountrySalesTax entity = new MDCountrySalesTax();
            entity.MDCountrySalesTaxID = Guid.NewGuid();
            if (parentACObject is MDCountry)
            {
                entity.MDCountry = parentACObject as MDCountry;
            }
            entity.DateFrom = DateTime.Now;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        /// <summary>
        /// Returns MDCountry
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to MDCountry</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return MDCountry;
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
                return MDCountrySalesTaxName;
            }
        }


        static readonly Func<DatabaseApp, IQueryable<MDCountrySalesTax>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDCountrySalesTax>>(
            (database) => from c in database.MDCountrySalesTax where c.IsDefault select c
        );

        public static MDCountrySalesTax DefaultMDCountrySalesTax(DatabaseApp dbApp)
        {
            try
            {
                MDCountrySalesTax defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = dbApp.MDCountrySalesTax.FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDCountrySalesTax", "DefaultMDCountrySalesTax", msg);
                return null;
            }
        }

        #endregion

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDCountrySalesTaxName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDCountrySalesTaxName");
            }
        }

        #endregion

    }
}
