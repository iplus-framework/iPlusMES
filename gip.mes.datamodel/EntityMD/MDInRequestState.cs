using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioPurchase, ConstApp.ESInRequestState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOInRequestState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDInRequestStateIndex", ConstApp.ESInRequestState, typeof(MDInRequestState.InRequestStates), Const.ContextDatabase + "\\InRequestStatesList", "", true, MinValue = (short)InRequestStates.InRequestNew)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioPurchase, Const.QueryPrefix + MDInRequestState.ClassName, ConstApp.ESInRequestState, typeof(MDInRequestState), MDInRequestState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDInRequestState>) })]
    public partial class MDInRequestState
    {
        public const string ClassName = "MDInRequestState";

        #region New/Delete
        public static MDInRequestState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDInRequestState entity = new MDInRequestState();
            entity.MDInRequestStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.InRequestState = InRequestStates.InRequestNew;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDInRequestState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDInRequestState>>(
            (database) => from c in database.MDInRequestState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDInRequestState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDInRequestState>>(
            (database, index) => from c in database.MDInRequestState where c.MDInRequestStateIndex == index select c
        );

        public static MDInRequestState DefaultMDInRequestState(DatabaseApp dbApp)
        {
            try
            {
                MDInRequestState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)InRequestStates.InRequestNew).FirstOrDefault();
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
                return MDInRequestStateName;
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
        public String MDInRequestStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDInRequestStateName");
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
        public InRequestStates InRequestState
        {
            get
            {
                return (InRequestStates)MDInRequestStateIndex;
            }
            set
            {
                MDInRequestStateIndex = (short)value;
                OnPropertyChanged("InRequestState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDInRequestStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'InRequestStates'}de{'InRequestStates'}", Global.ACKinds.TACEnum)]
        public enum InRequestStates : short
        {
            InRequestNew = 1,
            InRequestSended = 2,
            InRequestCompleted = 3,
            InRequestCancelled = 4
        }

        static ACValueItemList _InRequestStatesList = null;

        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück.
        /// </summary>
        public static ACValueItemList InRequestStatesList
        {
            get
            {
                if (_InRequestStatesList == null)
                {
                    _InRequestStatesList = new ACValueItemList("InRequestStates");
                    _InRequestStatesList.AddEntry((short)InRequestStates.InRequestNew, "en{'New'}de{'Neu'}");
                    _InRequestStatesList.AddEntry((short)InRequestStates.InRequestSended, "en{'Sended'}de{'Gesendet'}");
                    _InRequestStatesList.AddEntry((short)InRequestStates.InRequestCompleted, "en{'Completed'}de{'Fertiggestellt'}");
                    _InRequestStatesList.AddEntry((short)InRequestStates.InRequestCancelled, "en{'Cancelled'}de{'Storniert'}");
                }
                return _InRequestStatesList;
            }
        }

#endregion

    }
}
