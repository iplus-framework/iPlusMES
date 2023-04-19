using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESBookingNotAvailableMode, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOBookingNotAvailableMode")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDBookingNotAvailableModeIndex", "en{'Posting n.a.'}de{'Modus Buchung n. verf.'}", typeof(MDBookingNotAvailableMode.BookingNotAvailableModes), Const.ContextDatabase + "\\BookingNotAvailableModesList", "", true, MinValue = (short)BookingNotAvailableModes.AutoSet)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDBookingNotAvailableMode.ClassName, ConstApp.ESBookingNotAvailableMode, typeof(MDBookingNotAvailableMode), MDBookingNotAvailableMode.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDBookingNotAvailableMode>) })]
    public partial class MDBookingNotAvailableMode
    {
        public const string ClassName = "MDBookingNotAvailableMode";

        #region New/Delete
        public static MDBookingNotAvailableMode NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDBookingNotAvailableMode entity = new MDBookingNotAvailableMode();
            entity.MDBookingNotAvailableModeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.BookingNotAvailableMode = BookingNotAvailableModes.AutoSet;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDBookingNotAvailableMode>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDBookingNotAvailableMode>>(
            (database) => from c in database.MDBookingNotAvailableMode where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDBookingNotAvailableMode>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDBookingNotAvailableMode>>(
            (database, index) => from c in database.MDBookingNotAvailableMode where c.MDBookingNotAvailableModeIndex == index select c
        );

        public static MDBookingNotAvailableMode DefaultMDBookingNotAvailableMode(DatabaseApp dbApp)
        {
            try
            {
                MDBookingNotAvailableMode defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)BookingNotAvailableModes.Off).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDBookingNotAvailableMode", "DefaultMDBookingNotAvailableMode", msg);
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
                return MDBookingNotAvailableModeName;
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
        public BookingNotAvailableModes BookingNotAvailableMode
        {
            get
            {
                return (BookingNotAvailableModes)MDBookingNotAvailableModeIndex;
            }
            set
            {
                MDBookingNotAvailableModeIndex = (short)value;
                OnPropertyChanged("BookingNotAvailableMode");
            }
        }

        /// <summary>
        /// Enum für das Feld MDBookingNotAvailableModeIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'BookingNotAvailableModes'}de{'BookingNotAvailableModes'}", Global.ACKinds.TACEnum)]
        public enum BookingNotAvailableModes : short
        {
            // Default-Einstellung
            // Falls durch Buchungsvorgänge, der Bestand einer FacilityCharge Null unterschreitet, 
            // dann wird Nullbestandskennzeichen automatisch gesetzt
            AutoSet = 0,

            // Falls sich eine FacilityCharge im Nullbestand befindet, aber eine Mengenbuchung darauf erfolgen soll, dann automatisches Rücksetzen
            // Ansonsten wird eine neue FacilityCharge angelegt
            AutoReset = 1,

            // Kombination
            AutoSetAndReset = 2,

            // Chargen werden niemals automatisch auf Nullbestand gesetzt oder Rückgesetzt
            // -> Anwender muss Chargen manuell buchen
            Off = 3,

            // Wenn nichts an anderer Stelle Konfiguriert ist, wird AutoSet wird als Default angenommen
            _null = 4,
        }

        static ACValueItemList _BookingNotAvailableModesList = null;

        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück.
        /// </summary>
        public static ACValueItemList BookingNotAvailableModesList
        {
            get
            {
                if (_BookingNotAvailableModesList == null)
                {
                    _BookingNotAvailableModesList = new ACValueItemList("BookingNotAvailableModes");
                    _BookingNotAvailableModesList.AddEntry((short)BookingNotAvailableModes.AutoSet, "en{'Auto Set'}de{'Automatisch setzen'}");
                    _BookingNotAvailableModesList.AddEntry((short)BookingNotAvailableModes.AutoReset, "en{'Auto Reset'}de{'Automatisch zurücksetzen'}");
                    _BookingNotAvailableModesList.AddEntry((short)BookingNotAvailableModes.AutoSetAndReset, "en{'Auto Set and Reset'}de{'Automatisch setzen und zurücksetzen'}");
                    _BookingNotAvailableModesList.AddEntry((short)BookingNotAvailableModes.Off, "en{'Off'}de{'Aus'}");
                }
                return _BookingNotAvailableModesList;
            }
        }
        #endregion

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        public String MDBookingNotAvailableModeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDBookingNotAvailableModeName");
            }
        }

#endregion
    }
}
