using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.ESCountryLand, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(1, "MDCountryLandCode", "en{'Abbr. Fed. State'}de{'Abk. Bundesland'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + MDCountryLand.ClassName, ConstApp.ESCountryLand, typeof(MDCountryLand), MDCountryLand.ClassName, "MDCountryLandCode,MDNameTrans", "MDCountryLandCode")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDCountryLand>) })]
    public partial class MDCountryLand : IACObjectEntity
    {
        public const string ClassName = "MDCountryLand";

        #region New/Delete
        public static MDCountryLand NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            // Bei Systembelegung gibt es keine Vorbelegung, da hier kein Customizing erwünscht ist
            MDCountryLand entity = new MDCountryLand();
            entity.MDCountryLandID = Guid.NewGuid();
            if (parentACObject is MDCountry)
            {
                entity.MDCountry = parentACObject as MDCountry;
            }
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
        public override IACObject ParentACObject
        {
            get
            {
                return MDCountry;
            }
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
                return MDCountryLandName;
            }
        }


        static readonly Func<DatabaseApp, IQueryable<MDCountryLand>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDCountryLand>>(
            (database) => from c in database.MDCountryLand where c.IsDefault select c
        );

        public static MDCountryLand DefaultMDCountryLand(DatabaseApp dbApp)
        {
            try
            {
                MDCountryLand defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = dbApp.MDCountryLand.FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDCountryLand", "DefaultMDCountryLand", msg);

                return null;
            }
        }

        #endregion

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        public String MDCountryLandName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDCountryLandName");
            }
        }
      
#endregion

    }
}
