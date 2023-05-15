using gip.core.datamodel;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioPurchase, ConstApp.ESInOrderState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOInOrderState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDInOrderStateIndex", ConstApp.ESInOrderState, typeof(MDInOrderState.InOrderStates), Const.ContextDatabase + "\\InOrderStatesList", "", true, MinValue = (short)InOrderStates.Created)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioPurchase, Const.QueryPrefix + MDInOrderState.ClassName, ConstApp.ESInOrderState, typeof(MDInOrderState), MDInOrderState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDInOrderState>) })]
    [NotMapped]
    public partial class MDInOrderState
    {
        [NotMapped]
        public const string ClassName = "MDInOrderState";

        #region New/Delete
        public static MDInOrderState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDInOrderState entity = new MDInOrderState();
            entity.MDInOrderStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.InOrderState = InOrderStates.Created;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDInOrderState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDInOrderState>>(
            (database) => from c in database.MDInOrderState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDInOrderState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDInOrderState>>(
            (database, index) => from c in database.MDInOrderState where c.MDInOrderStateIndex == index select c
        );

        public static MDInOrderState DefaultMDInOrderState(DatabaseApp dbApp)
        {
            try
            {
                MDInOrderState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)InOrderStates.Created).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDInOrderState", "DefaultMDInOrderState", msg);
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
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return MDInOrderStateName;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
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
        [NotMapped]
        public String MDInOrderStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDInOrderStateName");
            }
        }

        #endregion

        #region IEntityProperty Members

        [NotMapped]
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
        [NotMapped]
        public InOrderStates InOrderState
        {
            get
            {
                return (InOrderStates)MDInOrderStateIndex;
            }
            set
            {
                MDInOrderStateIndex = (short)value;
                OnPropertyChanged("InOrderState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDInOrderStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'InOrderStates'}de{'InOrderStates'}", Global.ACKinds.TACEnum)]
        public enum InOrderStates : short
        {
            Created = 1, //Created
            InProcess = 2, //In Bearbeitung
            Completed = 3, //Abgeschlossen
            Blocked = 4, //Gesperrt
            Cancelled = 5, //Storniert
        }

        [NotMapped]
        static ACValueItemList _InOrderStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück.
        /// </summary>
        [NotMapped]
        public static ACValueItemList InOrderStatesList
        {
            get
            {
                if (_InOrderStatesList == null)
                {
                    _InOrderStatesList = new ACValueItemList("InOrderStates");
                    _InOrderStatesList.AddEntry((short)InOrderStates.Created, "en{'Created'}de{'Erstellt'}");
                    _InOrderStatesList.AddEntry((short)InOrderStates.InProcess, "en{'In Process'}de{'In Bearbeitung'}");
                    _InOrderStatesList.AddEntry((short)InOrderStates.Completed, "en{'Completed'}de{'Abgeschlossen'}");
                    _InOrderStatesList.AddEntry((short)InOrderStates.Blocked, "en{'Blocked'}de{'Gesperrt'}");
                    _InOrderStatesList.AddEntry((short)InOrderStates.Cancelled, "en{'Cancelled'}de{'Storniert'}");
                }
                return _InOrderStatesList;
            }
        }
        #endregion

    }
}




