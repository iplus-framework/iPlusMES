using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESReleaseState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOReleaseState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(1, "MDReleaseStateName", ConstApp.ESReleaseState, "", "", true)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDReleaseStateIndex", ConstApp.ESReleaseState, typeof(MDReleaseState.ReleaseStates), "", "", true, MinValue = (short)MDReleaseState.ReleaseStates.Free)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDReleaseState.ClassName, ConstApp.ESReleaseState, typeof(MDReleaseState), MDReleaseState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDReleaseState>) })]
    public partial class MDReleaseState
    {
        public const string ClassName = "MDReleaseState";

        #region New/Delete
        public static MDReleaseState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDReleaseState entity = new MDReleaseState();
            entity.MDReleaseStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.ReleaseState = ReleaseStates.Free;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDReleaseState>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDReleaseState>>(
            (database) => from c in database.MDReleaseState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDReleaseState>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDReleaseState>>(
            (database, index) => from c in database.MDReleaseState where c.MDReleaseStateIndex == index select c
        );

        public static MDReleaseState DefaultMDReleaseState(DatabaseApp dbApp)
        {
            try
            {
                MDReleaseState state = s_cQry_Default(dbApp).FirstOrDefault();
                if (state == null)
                    state = s_cQry_Index(dbApp, (short)ReleaseStates.Free).FirstOrDefault();
                return state;
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

        public static MDReleaseState DefaultMDReleaseState(DatabaseApp dbApp, MDReleaseState.ReleaseStates releaseState)
        {
            try
            {
                MDReleaseState defaultObj = s_cQry_Index(dbApp, (short)releaseState).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
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
                return MDReleaseStateName;
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
        public String MDReleaseStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDReleaseStateName");
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
        /// <summary>
        /// Enum für das Feld MDReleaseStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'ReleaseStates'}de{'ReleaseStates'}", Global.ACKinds.TACEnum)]
        public enum ReleaseStates : short
        {
            Free = 1, // freigegebener Bestand
            AbsFree = 2, // Absolut gesperrten Bestand freigeben, anschließender Status ist Free
            Locked = 3, // gesperrter Bestand
            AbsLocked = 4 // absolut gesperrter Bestand. Kann nur mit AbsFree zurückgesetzt werden
        }

        public ReleaseStates ReleaseState
        {
            get
            {
                return (ReleaseStates)MDReleaseStateIndex;
            }
            set
            {
                MDReleaseStateIndex = (Int16)value;
                OnPropertyChanged("ReleaseState");
            }
        }

        static ACValueItemList _ReleaseStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList ReleaseStatesList
        {
            get
            {
                if (_ReleaseStatesList == null)
                {
                    _ReleaseStatesList = new ACValueItemList("ReleaseStates");
                    _ReleaseStatesList.AddEntry((short)ReleaseStates.Free, "en{'Released Stock'}de{'Freigegebener Bestand'}");
                    _ReleaseStatesList.AddEntry((short)ReleaseStates.AbsFree, "en{'Releasing Absolute Blocked Stock'}de{'Absolut gesperrten Bestand freigeben'}");
                    _ReleaseStatesList.AddEntry((short)ReleaseStates.Locked, "en{'Blocked Stock'}de{'Gesperrter Bestand'}");
                    _ReleaseStatesList.AddEntry((short)ReleaseStates.AbsLocked, "en{'Absolute Blocked Stock'}de{'Absolut gesperrter Bestand'}");
                }
                return _ReleaseStatesList;
            }
        }

#endregion

    }
}




