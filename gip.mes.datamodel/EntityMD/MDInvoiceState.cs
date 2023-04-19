using System;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{

    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESInvoiceState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOInvoiceState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(1, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "InvoiceStateIndex", ConstApp.ESInvoiceState, typeof(MDInvoiceState.InvoiceStates), Const.ContextDatabase + "\\InvoiceStatesList", "", true, MinValue = (short)MDInvoiceState.InvoiceStates.Created)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + MDInvoiceState.ClassName, ConstApp.ESInvoiceState, typeof(MDInvoiceState), MDInvoiceState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDInvoiceState>) })]
    public partial class MDInvoiceState
    {
        public const string ClassName = "MDInvoiceState";

        #region New/Delete
        public static MDInvoiceState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDInvoiceState entity = new MDInvoiceState();
            entity.MDInvoiceStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.InvoiceState = InvoiceStates.Created;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDInvoiceState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDInvoiceState>>(
            (database) => from c in database.MDInvoiceState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDInvoiceState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDInvoiceState>>(
            (database, index) => from c in database.MDInvoiceState where c.InvoiceStateIndex == index select c
        );

       public static MDInvoiceState DefaultMDInvoiceState(DatabaseApp dbApp)
        {
            try
            {
                MDInvoiceState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)MDInvoiceState.InvoiceStates.Created).FirstOrDefault();
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
                return MDInvoiceStateName;
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
        public String MDInvoiceStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDInvoiceStateName");
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
        public InvoiceStates InvoiceState
        {
            get
            {
                return (InvoiceStates)InvoiceStateIndex;
            }
            set
            {
                InvoiceStateIndex = (short)value;
                OnPropertyChanged("InvoiceState");
            }
        }

        /// <summary>
        /// Enum für das Feld InvoiceStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, ConstApp.ESInvoiceState, Global.ACKinds.TACEnum)]
        public enum InvoiceStates : short
        {
            Created = 1, // Neu Angelegt
            InProcess = 2, //In Bearbeitung
            Completed = 3, //Fertiggestellt
            Blocked = 4, //Gesperrt
            Cancelled = 5, //Storniert
        }

        static ACValueItemList _InvoiceStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList InvoiceStatesList
        {
            get
            {
                if (_InvoiceStatesList == null)
                {
                    _InvoiceStatesList = new ACValueItemList("InvoiceStateIndex");
                    _InvoiceStatesList.AddEntry((short)InvoiceStates.Created, "en{'Created'}de{'Neu Angelegt'}");
                    _InvoiceStatesList.AddEntry((short)InvoiceStates.InProcess, "en{'In Process'}de{'In Bearbeitung'}");
                    _InvoiceStatesList.AddEntry((short)InvoiceStates.Completed, "en{'Completed'}de{'Fertiggestellt'}");
                    _InvoiceStatesList.AddEntry((short)InvoiceStates.Blocked, "en{'Blocked'}de{'Gesperrt'}");
                    _InvoiceStatesList.AddEntry((short)InvoiceStates.Cancelled, "en{'Cancelled'}de{'Storniert'}");
                }
                return _InvoiceStatesList;
            }
        }

#endregion

    }
}
