using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, ConstApp.ESMaintMode, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOMaintMode")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDMaintModeIndex", ConstApp.ESMaintMode, typeof(MDMaintMode.MaintModes), Const.ContextDatabase + "\\MaintModesList", "", true, MinValue = (short)MaintModes.TimeOnly)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MDMaintMode.ClassName, ConstApp.ESMaintMode, typeof(MDMaintMode), MDMaintMode.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDMaintMode>) })]
    public partial class MDMaintMode
    {
        public const string ClassName = "MDMaintMode";

        #region New/Delete
        public static MDMaintMode NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDMaintMode entity = new MDMaintMode();
            entity.MDMaintModeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MaintMode = MaintModes.TimeOnly;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDMaintMode>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDMaintMode>>(
            (database) => from c in database.MDMaintMode where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDMaintMode>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDMaintMode>>(
            (database, index) => from c in database.MDMaintMode where c.MDMaintModeIndex == index select c
        );

        public static MDMaintMode DefaultMDMaintMode(DatabaseApp dbApp)
        {
            try
            {
                MDMaintMode defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)MaintModes.TimeAndEvent).FirstOrDefault();
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
        public override string ACCaption
        {
            get
            {
                return MDMaintModeName;
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
        #endregion

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        public String MDMaintModeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDMaintModeName");
            }
        }

#endregion

#region IEntityProperty Members

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
        public MaintModes MaintMode
        {
            get
            {
                return (MaintModes)MDMaintModeIndex;
            }
            set
            {
                MDMaintModeIndex = (short)value;
                OnPropertyChanged("MaintMode");
            }
        }

        /// <summary>
        /// Enum für das Feld MDMaintModeIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'MaintModes'}de{'MaintModes'}", Global.ACKinds.TACEnum)]
        public enum MaintModes : short
        {
            TimeOnly = 1, //Nur Zeit
            EventOnly = 2, //Nur Ereignis
            TimeAndEvent = 3, //Nach Zeit und Ereignis
        }

        static ACValueItemList _MaintModesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück.
        /// </summary>
        public static ACValueItemList MaintModesList
        {
            get
            {
                if (_MaintModesList == null)
                {
                    _MaintModesList = new ACValueItemList("MaintModes");
                    _MaintModesList.AddEntry((short)MaintModes.TimeOnly, "en{'Time only'}de{'Nur Zeit'}");
                    _MaintModesList.AddEntry((short)MaintModes.EventOnly, "en{'Event only'}de{'Nur Ereignis'}");
                    _MaintModesList.AddEntry((short)MaintModes.TimeAndEvent, "en{'Time and Event'}de{'Zeit und Ereignis'}");
                }
                return _MaintModesList;
            }
        }
#endregion

    }
}
