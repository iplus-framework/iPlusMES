using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioPurchase, ConstApp.ESInOrderPosState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOInOrderPosState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDInOrderPosStateIndex", ConstApp.ESInOrderPosState, typeof(MDInOrderPosState.InOrderPosStates), Const.ContextDatabase + "\\InOrderPosStatesList", "", true, MinValue = (short)InOrderPosStates.Created)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioPurchase, Const.QueryPrefix + MDInOrderPosState.ClassName, ConstApp.ESInOrderPosState, typeof(MDInOrderPosState), MDInOrderPosState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDInOrderPosState>) })]
    public partial class MDInOrderPosState
    {
        public const string ClassName = "MDInOrderPosState";

        #region New/Delete
        public static MDInOrderPosState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDInOrderPosState entity = new MDInOrderPosState();
            entity.MDInOrderPosStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.InOrderPosState = InOrderPosStates.Created;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDInOrderPosState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDInOrderPosState>>(
            (database) => from c in database.MDInOrderPosState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDInOrderPosState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDInOrderPosState>>(
            (database, index) => from c in database.MDInOrderPosState where c.MDInOrderPosStateIndex == index select c
        );

        public static MDInOrderPosState DefaultMDInOrderPosState(DatabaseApp dbApp)
        {
            try
            {
                MDInOrderPosState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)InOrderPosStates.Created).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDInOrderPosState", "DefaultMDInOrderPosState", msg);
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
                return MDInOrderPosStateName;
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
        public String MDInOrderPosStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDInOrderPosStateName");
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
        public InOrderPosStates InOrderPosState
        {
            get
            {
                return (InOrderPosStates)MDInOrderPosStateIndex;
            }
            set
            {
                MDInOrderPosStateIndex = (short)value;
                OnPropertyChanged("InOrderPosState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDInOrderStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'InOrderPosStates'}de{'InOrderPosStates'}", Global.ACKinds.TACEnum)]
        public enum InOrderPosStates : short
        {
            Created = 1, //Created
            InProcess = 2, //In Bearbeitung
            Completed = 3, //Abgeschlossen
            Blocked = 4, //Gesperrt
            Cancelled = 5, //Storniert
        }

        static ACValueItemList _InOrderPosStatesList = null;

        public static ACValueItemList InOrderPosStatesList
        {
            get
            {
                if (_InOrderPosStatesList == null)
                {
                    _InOrderPosStatesList = new ACValueItemList("InOrderPosStates");
                    _InOrderPosStatesList.AddEntry((short)InOrderPosStates.Created, "en{'Created'}de{'Erstellt'}");
                    _InOrderPosStatesList.AddEntry((short)InOrderPosStates.InProcess, "en{'In Process'}de{'In Bearbeitung'}");
                    _InOrderPosStatesList.AddEntry((short)InOrderPosStates.Completed, "en{'Completed'}de{'Abgeschlossen'}");
                    _InOrderPosStatesList.AddEntry((short)InOrderPosStates.Blocked, "en{'Blocked'}de{'Gesperrt'}");
                    _InOrderPosStatesList.AddEntry((short)InOrderPosStates.Cancelled, "en{'Cancelled'}de{'Storniert'}");
                }
                return _InOrderPosStatesList;
            }
        }
#endregion

    }
}
