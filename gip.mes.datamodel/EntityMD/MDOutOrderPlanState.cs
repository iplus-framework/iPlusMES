using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESOutOrderPlanState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOOutOrderPlanState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDOutOrderPlanStateIndex", ConstApp.ESOutOrderPlanState, typeof(MDOutOrderPlanState.OutOrderPlanStates), Const.ContextDatabase + "\\OutOrderPlanStatesList", "", true, MinValue = (short)MDOutOrderPlanState.OutOrderPlanStates.NotPlanned)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + MDOutOrderPlanState.ClassName, ConstApp.ESOutOrderPlanState, typeof(MDOutOrderPlanState), MDOutOrderPlanState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDOutOrderPlanState>) })]
    public partial class MDOutOrderPlanState
    {
        public const string ClassName = "MDOutOrderPlanState";

        #region New/Delete
        public static MDOutOrderPlanState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDOutOrderPlanState entity = new MDOutOrderPlanState();
            entity.MDOutOrderPlanStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.OutOrderPlanState = OutOrderPlanStates.NotPlanned;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDOutOrderPlanState>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDOutOrderPlanState>>(
            (database) => from c in database.MDOutOrderPlanState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDOutOrderPlanState>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDOutOrderPlanState>>(
            (database, index) => from c in database.MDOutOrderPlanState where c.MDOutOrderPlanStateIndex == index select c
        );

        public static MDOutOrderPlanState DefaultMDOutOrderPlanState(DatabaseApp dbApp)
        {
            try
            {
                MDOutOrderPlanState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)OutOrderPlanStates.NotPlanned).FirstOrDefault();
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
                return MDOutOrderPlanStateName;
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

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        public String MDOutOrderPlanStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDOutOrderPlanStateName");
            }
        }
        #endregion
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
        public OutOrderPlanStates OutOrderPlanState
        {
            get
            {
                return (OutOrderPlanStates)MDOutOrderPlanStateIndex;
            }
            set
            {
                MDOutOrderPlanStateIndex = (short)value;
                OnPropertyChanged("OutOrderPlanState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDOutOrderPlanStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'OutOrderPlanStates'}de{'OutOrderPlanStates'}", Global.ACKinds.TACEnum)]
        public enum OutOrderPlanStates : short
        {
            NotPlanned = 1, //Nicht geplant
            SubsetPlanned = 2, //Teilmenge geplant
            CompletelyPlanned = 3, //Vollständig geplant
            Produced = 4, //Produziert      
        }

        static ACValueItemList _OutOrderPlanStatesList;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList OutOrderPlanStatesList
        {
            get
            {
                if (_OutOrderPlanStatesList == null)
                {
                    _OutOrderPlanStatesList = new ACValueItemList("OutOrderPlanStates");
                    _OutOrderPlanStatesList.AddEntry((short)OutOrderPlanStates.NotPlanned, "en{'Not Planned'}de{'Nicht geplant'}");
                    _OutOrderPlanStatesList.AddEntry((short)OutOrderPlanStates.SubsetPlanned, "en{'Subset Planned'}de{'Teilmenge geplant'}");
                    _OutOrderPlanStatesList.AddEntry((short)OutOrderPlanStates.CompletelyPlanned, "en{'Completely Planned'}de{'Vollständig geplant'}");
                    _OutOrderPlanStatesList.AddEntry((short)OutOrderPlanStates.Produced, "en{'Produced'}de{'Produziert'}");

                }
                return _OutOrderPlanStatesList;
            }
        }
        #endregion

    }
}