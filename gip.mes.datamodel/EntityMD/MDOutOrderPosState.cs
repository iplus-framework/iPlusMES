using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESOutOrderPosState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOOutOrderPosState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDOutOrderPosStateIndex", ConstApp.ESOutOrderPosState, typeof(MDOutOrderPosState.OutOrderPosStates), Const.ContextDatabase + "\\OutOrderPosStatesList", "", true, MinValue = (short)MDOutOrderPosState.OutOrderPosStates.Created)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + MDOutOrderPosState.ClassName, ConstApp.ESOutOrderPosState, typeof(MDOutOrderPosState), MDOutOrderPosState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDOutOrderPosState>) })]
    public partial class MDOutOrderPosState
    {
        public const string ClassName = "MDOutOrderPosState";

        #region New/Delete
        public static MDOutOrderPosState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDOutOrderPosState entity = new MDOutOrderPosState();
            entity.MDOutOrderPosStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.OutOrderPosState = OutOrderPosStates.Created;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDOutOrderPosState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDOutOrderPosState>>(
            (database) => from c in database.MDOutOrderPosState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDOutOrderPosState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDOutOrderPosState>>(
            (database, index) => from c in database.MDOutOrderPosState where c.MDOutOrderPosStateIndex == index select c
        );

        public static MDOutOrderPosState DefaultMDOutOrderPosState(DatabaseApp dbApp)
        {
            try
            {
                MDOutOrderPosState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)OutOrderPosStates.Created).FirstOrDefault();
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
                return MDOutOrderPosStateName;
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
        public String MDOutOrderPosStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDOutOrderPosStateName");
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
        public OutOrderPosStates OutOrderPosState
        {
            get
            {
                return (OutOrderPosStates)MDOutOrderPosStateIndex;
            }
            set
            {
                MDOutOrderPosStateIndex = (short)value;
                OnPropertyChanged("OutOrderPosState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDOutOrderPosStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'OutOrderPosStates'}de{'OutOrderPosStates'}", Global.ACKinds.TACEnum)]
        public enum OutOrderPosStates : short
        {
            Created = 1, //Neu Angelegt
            InProcess = 2, //In Bearbeitung
            Completed = 3, //Fertiggestellt
            Blocked = 4, //Gesperrt
            Cancelled = 5, //Storniert
        }

        static ACValueItemList _OutOrderPosStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList OutOrderPosStatesList
        {
            get
            {
                _OutOrderPosStatesList = new ACValueItemList("OutOrderPosStates");
                _OutOrderPosStatesList.AddEntry((short)OutOrderPosStates.Created, "en{'Created'}de{'Neu Angelegt'}");
                _OutOrderPosStatesList.AddEntry((short)OutOrderPosStates.InProcess, "en{'In Process'}de{'In Bearbeitung'}");
                _OutOrderPosStatesList.AddEntry((short)OutOrderPosStates.Completed, "en{'Completed'}de{'Fertiggestellt'}");
                _OutOrderPosStatesList.AddEntry((short)OutOrderPosStates.Blocked, "en{'Blocked'}de{'Gesperrt'}");
                _OutOrderPosStatesList.AddEntry((short)OutOrderPosStates.Cancelled, "en{'Cancelled'}de{'Storniert'}");
                return _OutOrderPosStatesList;
            }
        }
#endregion

    }
}
