using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.ESVisitorCardState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOVisitorCardState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDVisitorCardStateIndex", ConstApp.ESVisitorCardState, typeof(MDVisitorCardState.VisitorCardStates), Const.ContextDatabase + "\\VisitorCardStatesList", "", true, MinValue = 0)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + MDVisitorCardState.ClassName, ConstApp.ESVisitorCardState, typeof(MDVisitorCardState), MDVisitorCardState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDVisitorCardState>) })]
    public partial class MDVisitorCardState
    {
        public const string ClassName = "MDVisitorCardState";

        #region New/Delete
        public static MDVisitorCardState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDVisitorCardState entity = new MDVisitorCardState();
            entity.MDVisitorCardStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.VisitorCardState = VisitorCardStates.NotUsed;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }



        static readonly Func<DatabaseApp, IQueryable<MDVisitorCardState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDVisitorCardState>>(
            (database) => from c in database.MDVisitorCardState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDVisitorCardState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDVisitorCardState>>(
            (database, index) => from c in database.MDVisitorCardState where c.MDVisitorCardStateIndex == index select c
        );

        public static MDVisitorCardState DefaultMDVisitorCardState(DatabaseApp dbApp)
        {
            try
            {
                MDVisitorCardState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)VisitorCardStates.NotUsed).FirstOrDefault();
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
                return MDVisitorCardStateName;
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
        public String MDVisitorCardStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDVisitorCardStateName");
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
        public VisitorCardStates VisitorCardState
        {
            get
            {
                return (VisitorCardStates)MDVisitorCardStateIndex;
            }
            set
            {
                MDVisitorCardStateIndex = (short)value;
                OnPropertyChanged("VisitorCardState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDVisitorCardStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'VisitorCardStates'}de{'VisitorCardStates'}", Global.ACKinds.TACEnum)]
        public enum VisitorCardStates : short
        {
            NotUsed = 0,
            Active = 1,
            Blocked = 2
        }

        static ACValueItemList _VisitorCardStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList VisitorCardStatesList
        {
            get
            {
                if (_VisitorCardStatesList == null)
                {
                    _VisitorCardStatesList = new ACValueItemList("VisitorCardStates");
                    _VisitorCardStatesList.AddEntry((short)VisitorCardStates.NotUsed, "en{'Not Used'}de{'Nicht verwendet'}");
                    _VisitorCardStatesList.AddEntry((short)VisitorCardStates.Active, "en{'Active'}de{'Aktiv'}");
                    _VisitorCardStatesList.AddEntry((short)VisitorCardStates.Blocked, "en{'Blocked'}de{'Gesperrt'}");
                }
                return _VisitorCardStatesList;
            }
        }

#endregion


    }
}




