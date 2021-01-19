using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESFacilityInventoryPosState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOFacilityInventoryPosState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDFacilityInventoryPosStateIndex", ConstApp.ESFacilityInventoryPosState, typeof(MDFacilityInventoryPosState.FacilityInventoryPosStates), "", "", true, MinValue = (short)FacilityInventoryPosStates.New)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDFacilityInventoryPosState.ClassName, ConstApp.ESFacilityInventoryPosState, typeof(MDFacilityInventoryPosState), MDFacilityInventoryPosState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDFacilityInventoryPosState>) })]
    public partial class MDFacilityInventoryPosState
    {
        public const string ClassName = "MDFacilityInventoryPosState";

        #region New/Delete
        public static MDFacilityInventoryPosState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDFacilityInventoryPosState entity = new MDFacilityInventoryPosState();
            entity.MDFacilityInventoryPosStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.FacilityInventoryPosState = FacilityInventoryPosStates.New;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDFacilityInventoryPosState>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDFacilityInventoryPosState>>(
            (database) => from c in database.MDFacilityInventoryPosState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDFacilityInventoryPosState>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDFacilityInventoryPosState>>(
            (database, index) => from c in database.MDFacilityInventoryPosState where c.MDFacilityInventoryPosStateIndex == index select c
        );

        public static MDFacilityInventoryPosState DefaultMDFacilityInventoryPosState(DatabaseApp dbApp)
        {
            try
            {
                MDFacilityInventoryPosState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)FacilityInventoryPosStates.New).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDFacilityInventoryPosState", "DefaultMDFacilityInvetoryPosState", msg);
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
                return MDFacilityInventoryPosStateName;
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
        public String MDFacilityInventoryPosStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDFacilityInventoryPosStateName");
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
        public FacilityInventoryPosStates FacilityInventoryPosState
        {
            get
            {
                return (FacilityInventoryPosStates)MDFacilityInventoryPosStateIndex;
            }
            set
            {
                MDFacilityInventoryPosStateIndex = (short)value;
                OnPropertyChanged("FacilityInventoryPosState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDFacilityInventoryPosStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'FacilityInventoryPosStates'}de{'FacilityInventoryPosStates'}", Global.ACKinds.TACEnum)]
        public enum FacilityInventoryPosStates : short
        {
            New = 1,
            InProgress = 2,
            Paused = 3,
            Finished = 4,
        }

        static ACValueItemList _FacilityInventoryPosStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück.
        /// </summary>
        public static ACValueItemList FacilityInventoryPosStatesList
        {
            get
            {
                if (_FacilityInventoryPosStatesList == null)
                {
                    _FacilityInventoryPosStatesList = new ACValueItemList("FacilityInventoryPosStates");
                    _FacilityInventoryPosStatesList.AddEntry((short)FacilityInventoryPosStates.New, "en{'New'}de{'Neu'}");
                    _FacilityInventoryPosStatesList.AddEntry((short)FacilityInventoryPosStates.InProgress, "en{'In Process'}de{'In Bearbeitung'}");
                    _FacilityInventoryPosStatesList.AddEntry((short)FacilityInventoryPosStates.Paused, "en{'Paused'}de{'Pause'}");
                    _FacilityInventoryPosStatesList.AddEntry((short)FacilityInventoryPosStates.Finished, "en{'Finished'}de{'Beendet'}");
                }
                return _FacilityInventoryPosStatesList;
            }
        }
#endregion

    }
}
