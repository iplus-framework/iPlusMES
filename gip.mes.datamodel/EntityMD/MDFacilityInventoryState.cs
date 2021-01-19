using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESFacilityInventoryState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOFacilityInventoryState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDFacilityInventoryStateIndex", ConstApp.ESFacilityInventoryState, typeof(MDFacilityInventoryState.FacilityInventoryStates), "", "", true, MinValue = (short)FacilityInventoryStates.New)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDFacilityInventoryState.ClassName, ConstApp.ESFacilityInventoryState, typeof(MDFacilityInventoryState), MDFacilityInventoryState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDFacilityInventoryState>) })]
    public partial class MDFacilityInventoryState
    {
        public const string ClassName = "MDFacilityInventoryState";

        #region New/Delete
        public static MDFacilityInventoryState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDFacilityInventoryState entity = new MDFacilityInventoryState();
            entity.MDFacilityInventoryStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.FacilityInventoryState = FacilityInventoryStates.New;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDFacilityInventoryState>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDFacilityInventoryState>>(
            (database) => from c in database.MDFacilityInventoryState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDFacilityInventoryState>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDFacilityInventoryState>>(
            (database, index) => from c in database.MDFacilityInventoryState where c.MDFacilityInventoryStateIndex == index select c
        );

        public static MDFacilityInventoryState DefaultMDFacilityInventoryState(DatabaseApp dbApp)
        {
            try
            {
                MDFacilityInventoryState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)FacilityInventoryStates.New).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDFacilityInventoryState", "DefaultMDFacilityInventoryState", msg);

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
                return MDFacilityInventoryStateName;
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

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        public String MDFacilityInventoryStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDFacilityInventoryStateName");
            }
        }
        #endregion

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
        public FacilityInventoryStates FacilityInventoryState
        {
            get
            {
                return (FacilityInventoryStates)MDFacilityInventoryStateIndex;
            }
            set
            {
                MDFacilityInventoryStateIndex = (short)value;
                OnPropertyChanged("FacilityInventoryState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDFacilityInventoryStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'FacilityInventoryStates'}de{'FacilityInventoryStates'}", Global.ACKinds.TACEnum)]
        public enum FacilityInventoryStates : short
        {
            New = 1,
            InProgress = 2,
            Finished = 3,
        }

        static ACValueItemList _FacilityInventoryStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück.
        /// </summary>
        public static ACValueItemList FacilityInventoryStatesList
        {
            get
            {
                if(_FacilityInventoryStatesList == null)
                {
                    _FacilityInventoryStatesList = new ACValueItemList("FacilityInventoryStates");
                    _FacilityInventoryStatesList.AddEntry((short)FacilityInventoryStates.New, "en{'New'}de{'Neu'}");
                    _FacilityInventoryStatesList.AddEntry((short)FacilityInventoryStates.InProgress, "en{'In Process'}de{'In Bearbeitung'}");
                    _FacilityInventoryStatesList.AddEntry((short)FacilityInventoryStates.Finished, "en{'Finished'}de{'Beendet'}");
                }

                return _FacilityInventoryStatesList;
            }
        }
        #endregion

    }
}
