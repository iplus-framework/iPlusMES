using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioScheduling, ConstApp.ESTimeRange, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOTimeRange")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(6, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(1, "MDTimeRangeName", ConstApp.ESTimeRange, "", "", true)]
    [ACPropertyEntity(2, "TimeFrom", "en{'Begin of Shift'}de{'Schichtanfang'}", "", "", true)]
    [ACPropertyEntity(3, "TimeTo", "en{'End of Shift'}de{'Schichtende'}", "", "", true)]
    [ACPropertyEntity(4, "IsShiftModel", "en{'Shiftmodel'}de{'Schichtmodell'}", "", "", true)]
    [ACPropertyEntity(5, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(9999, "Comment", "en{'Comment'}de{'Bemerkung'}", "", "", true)]
    //[ACPropertyEntity(5, "MDInOrderStateIndex", "en{'MDInOrderStateIndex'}de{'de-MDInOrderStateIndex'}", "", false, false, false, "gip.core.datamodel.MDInOrderState+InOrderStates")]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioScheduling, Const.QueryPrefix + MDTimeRange.ClassName, ConstApp.ESTimeRange, typeof(MDTimeRange), MDTimeRange.ClassName, Const.MDNameTrans, Const.MDKey)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDInOrderState>) })]
    public partial class MDTimeRange
    {
        public const string ClassName = "MDTimeRange";

        #region New/Delete
        public static MDTimeRange NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDTimeRange entity = new MDTimeRange();
            entity.MDTimeRangeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is MDTimeRange)
            {
                entity.IsShiftModel = false;
                entity.MDTimeRange1_ParentMDTimeRange = parentACObject as MDTimeRange;
            }
            else
            {
                entity.IsShiftModel = true;
            }

            entity.TimeFrom = TimeSpan.FromHours(8);
            entity.TimeTo = TimeSpan.FromHours(16);
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDTimeRange>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDTimeRange>>(
            (database) => from c in database.MDTimeRange where c.IsDefault select c
        );

        public static MDTimeRange DefaultMDTimeRange(DatabaseApp dbApp)
        {
            try
            {
                MDTimeRange defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
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
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return MDTimeRangeName;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
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
        [NotMapped]
        public String MDTimeRangeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDTimeRangeName");
            }
        }

#endregion

#region IEntityProperty Members

        [NotMapped]
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


    }
}
