using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, ConstApp.ESToleranceState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOToleranceState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDToleranceStateIndex", ConstApp.ESToleranceState, typeof(MDToleranceState.ToleranceStates), Const.ContextDatabase + "\\ToleranceStateList", "", true, MinValue = (short)MDToleranceState.ToleranceStates.NoExceedanceOfTolerance)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + MDToleranceState.ClassName, ConstApp.ESToleranceState, typeof(MDToleranceState), MDToleranceState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDToleranceState>) })]
    public partial class MDToleranceState
    {
        public const string ClassName = "MDToleranceState";

        #region New/Delete
        public static MDToleranceState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDToleranceState entity = new MDToleranceState();
            entity.MDToleranceStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.ToleranceState = ToleranceStates.NoExceedanceOfTolerance;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDToleranceState>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDToleranceState>>(
            (database) => from c in database.MDToleranceState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDToleranceState>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDToleranceState>>(
            (database, index) => from c in database.MDToleranceState where c.MDToleranceStateIndex == index select c
        );

        public static MDToleranceState DefaultMDToleranceState(DatabaseApp dbApp)
        {
            try
            {
                MDToleranceState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)ToleranceStates.NoExceedanceOfTolerance).FirstOrDefault();
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
                return MDToleranceStateName;
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
        public String MDToleranceStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDToleranceStateName");
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
        public ToleranceStates ToleranceState
        {
            get
            {
                return (ToleranceStates)MDToleranceStateIndex;
            }
            set
            {
                MDToleranceStateIndex = (short)value;
                OnPropertyChanged("ToleranceState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDToleranceStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'ToleranceStates'}de{'ToleranceStates'}", Global.ACKinds.TACEnum)]
        public enum ToleranceStates : short
        {
            NoExceedanceOfTolerance = 1, //Keine Toleranzüberschreitungen
            ExceedanceOfTolerance = 2, //Toleranzüberschreitungen vorhanden
        }

        static ACValueItemList _ToleranceStates = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList ToleranceStatesList
        {
            get
            {
                if (_ToleranceStates == null)
                {
                    _ToleranceStates = new ACValueItemList("ToleranceState");
                    _ToleranceStates.AddEntry((short)ToleranceStates.NoExceedanceOfTolerance, "en{'No tolerance exceedence'}de{'Keine Toleranzüberschreitung'}");
                    _ToleranceStates.AddEntry((short)ToleranceStates.ExceedanceOfTolerance, "en{'Tolerance exceedances exist'}de{'Toleranzüberschreitung vorhanden'}");
                }
                return _ToleranceStates;
            }
        }
#endregion


    }
}




