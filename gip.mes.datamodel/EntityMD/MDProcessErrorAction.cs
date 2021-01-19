using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, ConstApp.ESProcessErrorAction, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOProcessErrorAction")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDProcessErrorActionIndex", ConstApp.ESProcessErrorAction, typeof(MDProcessErrorAction.ProcessErrorActions), Const.ContextDatabase + "\\ProcessErrorActionsList", "", true, MinValue = (short)MDProcessErrorAction.ProcessErrorActions.WarningOnly)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MDProcessErrorAction.ClassName, ConstApp.ESProcessErrorAction, typeof(MDProcessErrorAction), MDProcessErrorAction.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDProcessErrorAction>) })]
    public partial class MDProcessErrorAction
    {
        public const string ClassName = "MDProcessErrorAction";

        #region New/Delete
        public static MDProcessErrorAction NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDProcessErrorAction entity = new MDProcessErrorAction();
            entity.MDProcessErrorActionID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.ProcessErrorAction = ProcessErrorActions.WarningOnly;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDProcessErrorAction>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDProcessErrorAction>>(
            (database) => from c in database.MDProcessErrorAction where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDProcessErrorAction>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDProcessErrorAction>>(
            (database, index) => from c in database.MDProcessErrorAction where c.MDProcessErrorActionIndex == index select c
        );

        public static MDProcessErrorAction DefaultMDProcessErrorAction(DatabaseApp dbApp)
        {
            try
            {
                MDProcessErrorAction defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)ProcessErrorActions.WarningOnly).FirstOrDefault();
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
                return MDProcessErrorActionName;
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
        public String MDProcessErrorActionName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDProcessErrorActionName");
            }
        }

#endregion

#region IEntityProperty Members

        bool bRefreshConfig = false;
        partial void OnXMLConfigChanging(global::System.String value)
        {
            bRefreshConfig = false;
            if (this.EntityState != System.Data.EntityState.Detached && (!(String.IsNullOrEmpty(value) && String.IsNullOrEmpty(XMLConfig)) && value != XMLConfig))
                bRefreshConfig = true;
        }

        partial void OnXMLConfigChanged()
        {
            if (bRefreshConfig)
                ACProperties.Refresh();
        }

#endregion

#region enums
        public ProcessErrorActions ProcessErrorAction
        {
            get
            {
                return (ProcessErrorActions)MDProcessErrorActionIndex;
            }
            set
            {
                MDProcessErrorActionIndex = (short)value;
                OnPropertyChanged("ProcessErrorAction");
            }
        }

        /// <summary>
        /// Enum für das Feld MDProcessErrorActionIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'ProcessErrorActions'}de{'ProcessErrorActions'}", Global.ACKinds.TACEnum)]
        public enum ProcessErrorActions : short
        {
            WarningOnly = 1,
            BlockProcess = 2
        }

        static ACValueItemList _ProcessErrorActionsList;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList ProcessErrorActionList
        {
            get
            {
                if (_ProcessErrorActionsList == null)
                {
                    _ProcessErrorActionsList = new ACValueItemList("ProcessErrorActions");
                    _ProcessErrorActionsList.AddEntry((short)ProcessErrorActions.WarningOnly, "en{'Warning Only'}de{'Nur Warnung'}");
                    _ProcessErrorActionsList.AddEntry((short)ProcessErrorActions.BlockProcess, "en{'Block Process'}de{'Prozess blockieren'}");
                }
                return _ProcessErrorActionsList;
            }
        }
#endregion

    }
}
