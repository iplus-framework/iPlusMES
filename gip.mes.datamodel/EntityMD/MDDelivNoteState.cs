using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.ESDelivNoteState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSODelivNoteState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDDelivNoteStateIndex", ConstApp.ESDelivNoteState, typeof(MDDelivNoteState.DelivNoteStates), Const.ContextDatabase + "\\DelivNoteStatesList", "", true, MinValue = (short)DelivNoteStates.Created)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + MDDelivNoteState.ClassName, ConstApp.ESDelivNoteState, typeof(MDDelivNoteState), MDDelivNoteState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDDelivNoteState>) })]
    public partial class MDDelivNoteState
    {
        public const string ClassName = "MDDelivNoteState";

        #region New/Delete
        public static MDDelivNoteState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDDelivNoteState entity = new MDDelivNoteState();
            entity.MDDelivNoteStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.DelivNoteState = DelivNoteStates.Created;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDDelivNoteState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDDelivNoteState>>(
            (database) => from c in database.MDDelivNoteState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDDelivNoteState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDDelivNoteState>>(
            (database, index) => from c in database.MDDelivNoteState where c.MDDelivNoteStateIndex == index select c
        );

        public static MDDelivNoteState DefaultMDDelivNoteState(DatabaseApp dbApp)
        {
            try
            {
                MDDelivNoteState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)DelivNoteStates.Created).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDDelivNoteState", "DefaultMDDelivNoteState", msg);
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
                return MDDelivNoteStateName;
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
        public DelivNoteStates DelivNoteState
        {
            get
            {
                return (DelivNoteStates)MDDelivNoteStateIndex;
            }
            set
            {
                MDDelivNoteStateIndex = (short)value;
                OnPropertyChanged("DelivNoteState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDDelivNoteStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'DelivNoteStates'}de{'DelivNoteStates'}", Global.ACKinds.TACEnum)]
        public enum DelivNoteStates : short
        {
            Created = 1, // Erstellt
            InProcess = 2, // In Bearbeitung
            Completed = 3, // Abgeschlossen
            Cancelled = 4 // Storniert
        }

        [NotMapped]
        static ACValueItemList _DelivNoteStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        [NotMapped]
        public static ACValueItemList DelivNoteStatesList
        {
            get
            {
                if (_DelivNoteStatesList == null)
                {
                    _DelivNoteStatesList = new ACValueItemList("DelivNoteStates");
                    _DelivNoteStatesList.AddEntry((short)DelivNoteStates.Created, "en{'Created'}de{'Erstellt'}");
                    _DelivNoteStatesList.AddEntry((short)DelivNoteStates.InProcess, "en{'In Process'}de{'In Bearbeitung'}");
                    _DelivNoteStatesList.AddEntry((short)DelivNoteStates.Completed, "en{'Completed'}de{'Abgeschlossen'}");
                    _DelivNoteStatesList.AddEntry((short)DelivNoteStates.Cancelled, "en{'Cancelled'}de{'Storniert'}");
                }
                return _DelivNoteStatesList;
            }
        }
        #endregion

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDDelivNoteStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDDelivNoteStateName");
            }
        }
       
#endregion

    }
}




