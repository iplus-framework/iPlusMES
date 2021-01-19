using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.ESVisitorVoucherState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOTourplanPosState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDVisitorVoucherStateIndex", ConstApp.ESVisitorVoucherState, typeof(MDVisitorVoucherState.VisitorVoucherStates), Const.ContextDatabase + "\\VisitorVoucherStatesList", "", true, MinValue = (short)VisitorVoucherStates.New)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + MDVisitorVoucherState.ClassName, ConstApp.ESVisitorVoucherState, typeof(MDVisitorVoucherState), MDVisitorVoucherState.ClassName, "MDTransName", Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDVisitorVoucherState>) })]
    public partial class MDVisitorVoucherState
    {
        public const string ClassName = "MDVisitorVoucherState";

        #region New/Delete
        public static MDVisitorVoucherState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDVisitorVoucherState entity = new MDVisitorVoucherState();
            entity.MDVisitorVoucherStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.VisitorVoucherState = VisitorVoucherStates.New;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDVisitorVoucherState>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDVisitorVoucherState>>(
            (database) => from c in database.MDVisitorVoucherState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDVisitorVoucherState>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDVisitorVoucherState>>(
            (database, index) => from c in database.MDVisitorVoucherState where c.MDVisitorVoucherStateIndex == index select c
        );

        public static MDVisitorVoucherState DefaultMDVisitorVoucherState(DatabaseApp dbApp)
        {
            try
            {
                MDVisitorVoucherState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)VisitorVoucherStates.New).FirstOrDefault();
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

        public static MDVisitorVoucherState FromEnum(DatabaseApp dbApp, VisitorVoucherStates state)
        {
            try
            {
                MDVisitorVoucherState mdState = s_cQry_Index(dbApp, (short)state).FirstOrDefault();
                return mdState;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException(ClassName, "FromEnum", msg);
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
                return MDVisitorVoucherStateName;
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
        public String MDVisitorVoucherStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDVisitorVoucherStateName");
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

        public VisitorVoucherStates VisitorVoucherState
        {
            get
            {
                return (VisitorVoucherStates)MDVisitorVoucherStateIndex;
            }
            set
            {
                MDVisitorVoucherStateIndex = (Int16)value;
                OnPropertyChanged("VisitorVoucherState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDVisitorVoucherStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'TourplanPosStates'}de{'TourplanPosStates'}", Global.ACKinds.TACEnum)]
        public enum VisitorVoucherStates : short
        {
            New = 1,
            CheckedIn = 2,
            CheckedOut = 3
        }

        static ACValueItemList _VisitorVoucherStatesList = null;

        public static ACValueItemList VisitorVoucherStatesList
        {
            get
            {
                if (_VisitorVoucherStatesList == null)
                {
                    _VisitorVoucherStatesList = new ACValueItemList("VisitorVoucherStates");
                    _VisitorVoucherStatesList.AddEntry((short)VisitorVoucherStates.New, "en{'New'}de{'Neu'}");
                    _VisitorVoucherStatesList.AddEntry((short)VisitorVoucherStates.CheckedIn, "en{'Checked In'}de{'Eingecheckt'}");
                    _VisitorVoucherStatesList.AddEntry((short)VisitorVoucherStates.CheckedOut, "en{'Checked Out'}de{'Ausgecheckt'}");
                }
                return _VisitorVoucherStatesList;
            }
        }

#endregion


    }
}




