using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, ConstApp.ESMaintOrderPropertyState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOMaintOrderPropertyState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDMaintOrderPropertyStateIndex", ConstApp.ESMaintOrderPropertyState, typeof(MDMaintOrderPropertyState.MaintOrderPropertyStates), "", "", true, MinValue = (short)MaintOrderPropertyStates.NewCreated)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MDMaintOrderPropertyState.ClassName, ConstApp.ESMaintOrderPropertyState, typeof(MDMaintOrderPropertyState), MDMaintOrderPropertyState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDMaintOrderPropertyState>) })]
    public partial class MDMaintOrderPropertyState
    {
        public const string ClassName = "MDMaintOrderPropertyState";

        #region New/Delete
        public static MDMaintOrderPropertyState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDMaintOrderPropertyState entity = new MDMaintOrderPropertyState();
            entity.MDMaintOrderPropertyStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MaintOrderPropertyState = MaintOrderPropertyStates.NewCreated;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDMaintOrderPropertyState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDMaintOrderPropertyState>>(
            (database) => from c in database.MDMaintOrderPropertyState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDMaintOrderPropertyState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDMaintOrderPropertyState>>(
            (database, index) => from c in database.MDMaintOrderPropertyState where c.MDMaintOrderPropertyStateIndex == index select c
        );

        public static MDMaintOrderPropertyState DefaultMDMaintOrderPropertyState(DatabaseApp dbApp)
        {
            try
            {
                MDMaintOrderPropertyState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)MaintOrderPropertyStates.NewCreated).FirstOrDefault();
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
                return MDMaintOrderPropertyStateName;
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
        public String MDMaintOrderPropertyStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDMaintOrderPropertyStateName");
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
        public MaintOrderPropertyStates MaintOrderPropertyState
        {
            get
            {
                return (MaintOrderPropertyStates)MDMaintOrderPropertyStateIndex;
            }
            set
            {
                MDMaintOrderPropertyStateIndex = (short)value;
                OnPropertyChanged("MaintOrderPropertyState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDMaintOrderPropertyStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'MaintOrderPropertyStates'}de{'MaintOrderPropertyStates'}", Global.ACKinds.TACEnum)]
        public enum MaintOrderPropertyStates : short
        {
            NewCreated = 1, //Neu angelegt
            MaintenanceNeededOrActive = 2, //Wartung fällig/aktiv
            MaintenanceInProcess = 3 //Wartung durchgeführt
        }

        static ACValueItemList _MaintOrderPropertyStatesList = null;

        public static ACValueItemList MaintOrderPropertyStatesList
        {
            get
            {
                if (_MaintOrderPropertyStatesList == null)
                {
                    _MaintOrderPropertyStatesList = new ACValueItemList("MaintOrderPropertyStates");
                    _MaintOrderPropertyStatesList.AddEntry((short)MaintOrderPropertyStates.NewCreated, "en{'New Created'}de{'New Angelegt'}");
                    _MaintOrderPropertyStatesList.AddEntry((short)MaintOrderPropertyStates.MaintenanceNeededOrActive, "en{'Maintenance needed or active'}de{'Wartung fällig o. aktiv'}");
                    _MaintOrderPropertyStatesList.AddEntry((short)MaintOrderPropertyStates.MaintenanceInProcess, "en{'Maintenance in Process'}de{'Wartung durchgeführt'}");
                }
                return _MaintOrderPropertyStatesList;
            }
        }

#endregion

    }
}
