using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESReservationMode, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOReservationMode")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDReservationModeIndex", ConstApp.ESReservationMode, typeof(MDReservationMode.ReservationModes), "", "", true, MinValue = (short)MDReservationMode.ReservationModes.Off)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDReservationMode.ClassName, ConstApp.ESReservationMode, typeof(MDReservationMode), MDReservationMode.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDReservationMode>) })]
    public partial class MDReservationMode
    {
        public const string ClassName = "MDReservationMode";

        #region New/Delete
        public static MDReservationMode NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDReservationMode entity = new MDReservationMode();
            entity.MDReservationModeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.ReservationMode = ReservationModes.Off;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDReservationMode>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDReservationMode>>(
            (database) => from c in database.MDReservationMode where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDReservationMode>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDReservationMode>>(
            (database, index) => from c in database.MDReservationMode where c.MDReservationModeIndex == index select c
        );

        public static MDReservationMode DefaultMDReservationMode(DatabaseApp dbApp)
        {
            try
            {
                MDReservationMode defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)ReservationModes.Off).FirstOrDefault();
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
                return MDReservationModeName;
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
        public String MDReservationModeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDReservationModeName");
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
        public ReservationModes ReservationMode
        {
            get
            {
                return (ReservationModes)MDReservationModeIndex;
            }
            set
            {
                MDReservationModeIndex = (short)value;
                OnPropertyChanged("ReservationMode");
            }
        }

        /// <summary>
        /// Enum für das Feld MDReservationModeIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'ReservationModes'}de{'ReservationModes'}", Global.ACKinds.TACEnum)]
        public enum ReservationModes : short
        {
            Off = 0, // Keine Reservierung
            Reserve = 1, // Reserviere für die entsprechend Übergebene Entität aus WE,WA,Produktion
            CancelSubset = 2, // Löse Reservierung auf für eine Teilmenge
            CancelComplete = 3, // Löse Reservierung vollständig auf
        }

        static ACValueItemList _ReservationModesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList ReservationModesList
        {
            get
            {
                _ReservationModesList = new ACValueItemList("ReservationModes");
                _ReservationModesList.AddEntry((short)ReservationModes.Off, "en{'No Reservation'}de{'Keine Reservierung'}");
                _ReservationModesList.AddEntry((short)ReservationModes.Reserve, "en{'Reservation'}de{'Reservierung'}");
                _ReservationModesList.AddEntry((short)ReservationModes.CancelSubset, "en{'Resolve Reservation for Partial Quantity'}de{'Reservierung für Teilmenge auflösen'}");
                _ReservationModesList.AddEntry((short)ReservationModes.CancelComplete, "en{'Completely Resolve Reservation'}de{'Reservierung vollständig auflösen'}");
                return _ReservationModesList;
            }
        }
#endregion

        
    }
}




