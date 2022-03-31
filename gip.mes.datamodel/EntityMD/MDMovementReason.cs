using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESMovementReason, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOMovementReason")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDMovementReasonIndex", ConstApp.ESMovementReason, typeof(MovementReasonsEnum), Const.ContextDatabase + "\\MovementReasonsList", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDMovementReason.ClassName, ConstApp.ESMovementReason, typeof(MDMovementReason), MDMovementReason.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDMovementReason>) })]
    public partial class MDMovementReason
    {
        public const string ClassName = "MDMovementReason";

        #region New/Delete
        public static MDMovementReason NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDMovementReason entity = new MDMovementReason();
            entity.MDMovementReasonID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.MovementReason = MovementReasonsEnum.Adjustment;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        static readonly Func<DatabaseApp, IQueryable<MDMovementReason>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDMovementReason>>(
            (database) => from c in database.MDMovementReason where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDMovementReason>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDMovementReason>>(
            (database, index) => from c in database.MDMovementReason where c.MDMovementReasonIndex == index select c
        );

        public static MDMovementReason DefaultMDMovementReason(DatabaseApp dbApp)
        {
            try
            {
                MDMovementReason defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)MovementReasonsEnum.Adjustment).FirstOrDefault();
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
                return MDMovementReasonName;
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
        [ACPropertyInfo(1, "", "en{'Movement Reason'}de{'Bewegungsgrund'}", MinLength = 1)]
        public String MDMovementReasonName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDMovementReasonName");
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
        [ACPropertyInfo(37, "", ConstApp.ESMovementReason, Const.ContextDatabase + "\\MovementReasonsList", true)]
        public MovementReasonsEnum MovementReason
        {
            get
            {
                return (MovementReasonsEnum)MDMovementReasonIndex;
            }
            set
            {
                MDMovementReasonIndex = (short)value;
                OnPropertyChanged("MovementReason");
            }
        }

        #endregion

    }
}




