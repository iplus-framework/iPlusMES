using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESOutOfferingState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOOutOfferingState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDOutOfferingStateIndex", ConstApp.ESOutOfferingState, typeof(MDOutOfferingState.OutOfferingStates), Const.ContextDatabase + "\\OutOfferingStatesList", "", true, MinValue = (short)MDOutOfferingState.OutOfferingStates.OfferingNew)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + MDOutOfferingState.ClassName, ConstApp.ESOutOfferingState, typeof(MDOutOfferingState), MDOutOfferingState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDOutOfferingState>) })]
    public partial class MDOutOfferingState
    {
        public const string ClassName = "MDOutOfferingState";

        #region New/Delete
        public static MDOutOfferingState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDOutOfferingState entity = new MDOutOfferingState();
            entity.MDOutOfferingStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.OutOfferingState = OutOfferingStates.OfferingNew;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDOutOfferingState>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDOutOfferingState>>(
            (database) => from c in database.MDOutOfferingState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDOutOfferingState>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDOutOfferingState>>(
            (database, index) => from c in database.MDOutOfferingState where c.MDOutOfferingStateIndex == index select c
        );

        public static MDOutOfferingState DefaultMDOutOfferingState(DatabaseApp dbApp)
        {
            try
            {
                MDOutOfferingState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)OutOfferingStates.OfferingNew).FirstOrDefault();
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
                return MDOutOfferingStateName;
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
        public String MDOutOfferingStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDOutOfferingStateName");
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
        public OutOfferingStates OutOfferingState
        {
            get
            {
                return (OutOfferingStates)MDOutOfferingStateIndex;
            }
            set
            {
                MDOutOfferingStateIndex = (short)value;
                OnPropertyChanged("OutOfferingState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDOutOfferingStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'OutOfferingStates'}de{'OutOfferingStates'}", Global.ACKinds.TACEnum)]
        public enum OutOfferingStates : short
        {
            OfferingNew = 1,
            OfferingSended = 2,
            OfferingCompleted = 3,
            OfferingCancelled = 4
        }

        static ACValueItemList _OutOfferingStatesList;

        public static ACValueItemList OutOfferingStatesList
        {
            get
            {
                if (_OutOfferingStatesList == null)
                {
                    _OutOfferingStatesList = new ACValueItemList("OutOfferingStates");
                    _OutOfferingStatesList.AddEntry((short)OutOfferingStates.OfferingNew, "en{'New'}de{'Neu'}");
                    _OutOfferingStatesList.AddEntry((short)OutOfferingStates.OfferingSended, "en{'Sended'}de{'Gesendet'}");
                    _OutOfferingStatesList.AddEntry((short)OutOfferingStates.OfferingCompleted, "en{'Completed'}de{'Fertiggestellt'}");
                    _OutOfferingStatesList.AddEntry((short)OutOfferingStates.OfferingCancelled, "en{'Cancelled'}de{'Storniert'}");
                }
                return _OutOfferingStatesList;
            }
        }
#endregion

    }
}








