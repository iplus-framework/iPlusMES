using System;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESOutOrderState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOOutOrderState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDOutOrderStateIndex", ConstApp.ESOutOrderState, typeof(MDOutOrderState.OutOrderStates), Const.ContextDatabase + "\\OutOrderStatesList", "", true, MinValue = (short)MDOutOrderState.OutOrderStates.Created)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + MDOutOrderState.ClassName, ConstApp.ESOutOrderState, typeof(MDOutOrderState), MDOutOrderState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDOutOrderState>) })]
    public partial class MDOutOrderState
    {
        public const string ClassName = "MDOutOrderState";

        #region New/Delete
        public static MDOutOrderState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDOutOrderState entity = new MDOutOrderState();
            entity.MDOutOrderStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.OutOrderState = OutOrderStates.Created;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDOutOrderState>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDOutOrderState>>(
            (database) => from c in database.MDOutOrderState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDOutOrderState>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDOutOrderState>>(
            (database, index) => from c in database.MDOutOrderState where c.MDOutOrderStateIndex == index select c
        );

        public static MDOutOrderState DefaultMDOutOrderState(DatabaseApp dbApp)
        {
            try
            {
                MDOutOrderState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)OutOrderStates.Created).FirstOrDefault();
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
                return MDOutOrderStateName;
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
        public String MDOutOrderStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDOutOrderStateName");
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
        public OutOrderStates OutOrderState
        {
            get
            {
                return (OutOrderStates)MDOutOrderStateIndex;
            }
            set
            {
                MDOutOrderStateIndex = (short)value;
                OnPropertyChanged("OutOrderState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDOutOrderStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'Sales Order Status'}de{'Verkaufsauftrag-Status'}", Global.ACKinds.TACEnum)]
        public enum OutOrderStates : short
        {
            Created = 1, // Neu Angelegt
            InProcess = 2, //In Bearbeitung
            Completed = 3, //Fertiggestellt
            Blocked = 4, //Gesperrt
            Cancelled = 5, //Storniert
        }

        static ACValueItemList _OutOrderStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList OutOrderStatesList
        {
            get
            {
                if (_OutOrderStatesList == null)
                {
                    _OutOrderStatesList = new ACValueItemList("OutOrderStates");
                    _OutOrderStatesList.AddEntry((short)OutOrderStates.Created, "en{'Created'}de{'Neu Angelegt'}");
                    _OutOrderStatesList.AddEntry((short)OutOrderStates.InProcess, "en{'In Process'}de{'In Bearbeitung'}");
                    _OutOrderStatesList.AddEntry((short)OutOrderStates.Completed, "en{'Completed'}de{'Fertiggestellt'}");
                    _OutOrderStatesList.AddEntry((short)OutOrderStates.Blocked, "en{'Blocked'}de{'Gesperrt'}");
                    _OutOrderStatesList.AddEntry((short)OutOrderStates.Cancelled, "en{'Cancelled'}de{'Storniert'}");
                }
                return _OutOrderStatesList;
            }
        }

#endregion

    }
}