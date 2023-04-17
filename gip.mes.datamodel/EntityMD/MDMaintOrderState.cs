using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, ConstApp.ESMaintOrderState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOMaintOrderState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDMaintOrderStateIndex", ConstApp.ESMaintOrderState, typeof(MDMaintOrderState.MaintOrderStates), Const.ContextDatabase + "\\MaintOrderStateList", "", true, MinValue = (short)MaintOrderStates.MaintenanceNeeded)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MDMaintOrderState.ClassName, ConstApp.ESMaintOrderState, typeof(MDMaintOrderState), MDMaintOrderState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDMaintOrderState>) })]
    public partial class MDMaintOrderState
    {
        public const string ClassName = "MDMaintOrderState";

        #region New/Delete
        public static MDMaintOrderState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDMaintOrderState entity = new MDMaintOrderState();
            entity.MDMaintOrderStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MaintOrderState = MaintOrderStates.MaintenanceNeeded;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDMaintOrderState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDMaintOrderState>>(
            (database) => from c in database.MDMaintOrderState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDMaintOrderState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDMaintOrderState>>(
            (database, index) => from c in database.MDMaintOrderState where c.MDMaintOrderStateIndex == index select c
        );

        public static MDMaintOrderState DefaultMDMaintOrderState(DatabaseApp dbApp)
        {
            try
            {
                MDMaintOrderState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)MaintOrderStates.MaintenanceNeeded).FirstOrDefault();
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
                return MDMaintOrderStateName;
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
        public String MDMaintOrderStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDMaintOrderStateName");
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
        public MaintOrderStates MaintOrderState
        {
            get
            {
                return (MaintOrderStates)MDMaintOrderStateIndex;
            }
            set
            {
                MDMaintOrderStateIndex = (short)value;
                OnPropertyChanged("MaintOrderState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDMaintOrderStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'MaintOrderStates'}de{'MaintOrderStates'}", Global.ACKinds.TACEnum)]
        public enum MaintOrderStates : short
        {
            MaintenanceNeeded = 1, //Neu angelegt
            MaintenanceInProcess = 2, //Wartung fällig/aktiv
            MaintenanceCompleted = 3 //Wartung durchgeführt
        }

        static ACValueItemList _MaintOrderStateList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die Gui zurück
        /// </summary>
        public static ACValueItemList MaintOrderStateList
        {
            get
            {
                if (_MaintOrderStateList == null)
                {
                    _MaintOrderStateList = new ACValueItemList("MaintOrderStates");
                    _MaintOrderStateList.AddEntry((short)MaintOrderStates.MaintenanceNeeded, "en{'Maintenance Needed'}de{'Wartung erforderlich'}");
                    _MaintOrderStateList.AddEntry((short)MaintOrderStates.MaintenanceInProcess, "en{'Maintenance in Process'}de{'Wartung aktiv'}");
                    _MaintOrderStateList.AddEntry((short)MaintOrderStates.MaintenanceCompleted, "en{'Maintenance Completed'}de{'Wartung durchgeführt'}");
                }
                return _MaintOrderStateList;
            }
        }
#endregion

    }
}
