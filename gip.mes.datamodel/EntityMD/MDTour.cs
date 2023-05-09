using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.ESTour, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOTour")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(11, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, "MDTourNo", "en{'Tour No.'}de{'Tour-Nr.'}", "", "", true)]
    [ACPropertyEntity(3, "Monday", "en{'Monday'}de{'Montag'}", "", "", true)]
    [ACPropertyEntity(4, "Tuesday", "en{'Tuesday'}de{'Dienstag'}", "", "", true)]
    [ACPropertyEntity(5, "Wednesday", "en{'Wednesday'}de{'Mittwoch'}", "", "", true)]
    [ACPropertyEntity(6, "Thursday", "en{'Thursday'}de{'Donnerstag'}", "", "", true)]
    [ACPropertyEntity(7, "Friday", "en{'Friday'}de{'Freitag'}", "", "", true)]
    [ACPropertyEntity(8, "Saturday", "en{'Saturday'}de{'Samstag'}", "", "", true)]
    [ACPropertyEntity(9, "Sunday", "en{'Sunday'}de{'Sonntag'}", "", "", true)]
    [ACPropertyEntity(10, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + MDTour.ClassName, ConstApp.ESTour, typeof(MDTour), MDTour.ClassName, "MDNameTrans,MDTourNo", "MDTourNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDTour>) })]
    public partial class MDTour
    {
        public const string ClassName = "MDTour";

        #region New/Delete
        public static MDTour NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDTour entity = new MDTour();
            entity.MDTourID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.Monday = false;
            entity.Tuesday = false;
            entity.Wednesday = false;
            entity.Thursday = false;
            entity.Friday = false;
            entity.Saturday = false;
            entity.Sunday = false;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDTour>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDTour>>(
            (database) => from c in database.MDTour where c.IsDefault select c
        );

        public static MDTour DefaultMDTour(DatabaseApp dbApp)
        {
            try
            {
                MDTour defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
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
                return MDTourName;
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
        public String MDTourName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDTourName");
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




