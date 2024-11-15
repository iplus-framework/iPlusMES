using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.ESLabTag, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOLabTag")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDLabTagIndex", ConstApp.ESLabTag, typeof(MDLabTag.LabTags), Const.ContextDatabase + "\\LabTagsList", "", true, MinValue = (short)LabTags.Maesure)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MDLabTag.ClassName, ConstApp.ESLabTag, typeof(MDLabTag), MDLabTag.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDLabTag>) })]
    [NotMapped]
    public partial class MDLabTag
    {
        [NotMapped]
        public const string ClassName = "MDLabTag";

        #region New/Delete
        public static MDLabTag NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDLabTag entity = new MDLabTag();
            entity.MDLabTagID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.LabTag = LabTags.Maesure;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IEnumerable<MDLabTag>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IEnumerable<MDLabTag>>(
            (database) => from c in database.MDLabTag where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IEnumerable<MDLabTag>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IEnumerable<MDLabTag>>(
            (database, index) => from c in database.MDLabTag where c.MDLabTagIndex == index select c
        );

        public static MDLabTag DefaultMDLabTag(DatabaseApp dbApp)
        {
            try
            {
                MDLabTag defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)LabTags.Maesure).FirstOrDefault();
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
                return MDLabTagName;
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
        public String MDLabTagName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDLabTagName");
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

#region enums
        [NotMapped]
        public LabTags LabTag
        {
            get
            {
                return (LabTags)MDLabTagIndex;
            }
            set
            {
                MDLabTagIndex = (short)value;
                OnPropertyChanged("LabTag");
            }
        }

        /// <summary>
        /// Enum für das Feld MDLabTagIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'LabTags'}de{'LabTags'}", Global.ACKinds.TACEnum)]
        public enum LabTags : short
        {
            Maesure = 1,
        }

        [NotMapped]
        static ACValueItemList _LabTagsList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        [NotMapped]
        public static ACValueItemList LabTagsList
        {
            get
            {
                if (_LabTagsList == null)
                {
                    _LabTagsList = new ACValueItemList("LabTags");
                    _LabTagsList.AddEntry((short)LabTags.Maesure, "en{'Measure'}de{'Messen'}");
                }
                return _LabTagsList;
            }
        }

#endregion

    }
}




