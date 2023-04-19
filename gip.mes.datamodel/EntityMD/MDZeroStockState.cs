using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESZeroStockState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOZeroStockState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDZeroStockStateIndex", ConstApp.ESZeroStockState, typeof(MDZeroStockState.ZeroStockStates), Const.ContextDatabase + "\\ZeroStockStatesList", "", true, MinValue = 0)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDZeroStockState.ClassName, ConstApp.ESZeroStockState, typeof(MDZeroStockState), MDZeroStockState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDZeroStockState>) })]
    public partial class MDZeroStockState
    {
        public const string ClassName = "MDZeroStockState";

        #region New/Delete
        public static MDZeroStockState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDZeroStockState entity = new MDZeroStockState();
            entity.MDZeroStockStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.ZeroStockState = ZeroStockStates.Off;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDZeroStockState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDZeroStockState>>(
            (database) => from c in database.MDZeroStockState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDZeroStockState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDZeroStockState>>(
            (database, index) => from c in database.MDZeroStockState where c.MDZeroStockStateIndex == index select c
        );


        static readonly Func<DatabaseApp, short, IQueryable<MDZeroStockState>> s_cQry_IndexDefault =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDZeroStockState>>(
            (database, index) => from c in database.MDZeroStockState where c.IsDefault && c.MDZeroStockStateIndex == index select c
        );

        public static MDZeroStockState DefaultMDZeroStockState(DatabaseApp dbApp)
        {
            try
            {
                MDZeroStockState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)ZeroStockStates.Off).FirstOrDefault();
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

        public static MDZeroStockState DefaultMDZeroStockState(DatabaseApp dbApp, ZeroStockStates zeroStockState)
        {
            try
            {
                MDZeroStockState defaultObj = s_cQry_IndexDefault(dbApp, (short)zeroStockState).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)zeroStockState).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException(ClassName, "Default" + ClassName+"(10)", msg);
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
                return MDZeroStockStateName;
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
        public String MDZeroStockStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDZeroStockStateName");
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
        public ZeroStockStates ZeroStockState
        {
            get
            {
                return (ZeroStockStates)MDZeroStockStateIndex;
            }
            set
            {
                MDZeroStockStateIndex = (short)value;
                OnPropertyChanged("ZeroStockState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDZeroStockStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'ZeroStockStates'}de{'ZeroStockStates'}", Global.ACKinds.TACEnum)]
        public enum ZeroStockStates : short
        {
            // Default-Einstellung -> Keine Auswirkung
            Off = 0,

            // Fall dieser Parameter gesetzt wird, dann wird er Bestand auf 0 kg bzw. einheiten gebucht
            // Falls Konfigurationsparameter "NotAvailableMode" auf AutoSet oder AutoSetAndReset gesetzt ist, dann wird Nullbestandskennzeichen automatisch gesetzt
            // Ansonsten bleibt der Bestand auf 0 kg
            BookToZeroStock = 1,

            // Entspricht BookToZeroStock jedoch wird das Nullbestandskennzeichen auf jeden Fall gesetzt
            SetNotAvailable = 2,

            // Der Nullbestands-Zustand soll aufgehoben werden
            ResetIfNotAvailable = 3,
        }

        static ACValueItemList _ZeroStockStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList ZeroStockStatesList
        {
            get
            {
                if (_ZeroStockStatesList == null)
                {
                    _ZeroStockStatesList = new ACValueItemList("");
                    _ZeroStockStatesList.AddEntry((short)ZeroStockStates.Off, "en{'Off'}de{'Aus'}");
                    _ZeroStockStatesList.AddEntry((short)ZeroStockStates.BookToZeroStock, "en{'Post to Zero Stock'}de{'Buchen auf Nullbestand'}");
                    _ZeroStockStatesList.AddEntry((short)ZeroStockStates.SetNotAvailable, "en{'Set Not Available'}de{'Auf nicht verfügbar setzen'}");
                    _ZeroStockStatesList.AddEntry((short)ZeroStockStates.ResetIfNotAvailable, "en{'Reset If Not Available'}de{'Zurücksetzen, wenn nicht verfügbar'}");
                }
                return _ZeroStockStatesList;
            }
        }
#endregion


    }
}




