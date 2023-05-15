using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESOutOrderType, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOOutOrderType")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "OrderTypeIndex", ConstApp.ESOutOrderType, typeof(GlobalApp.OrderTypes), Const.ContextDatabase + "\\OrderTypesList", "", true, MinValue = (short)GlobalApp.OrderTypes.Order)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + MDOutOrderType.ClassName, ConstApp.ESOutOrderType, typeof(MDOutOrderType), MDOutOrderType.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDOutOrderType>) })]
    [NotMapped]
    public partial class MDOutOrderType
    {
        [NotMapped]
        public const string ClassName = "MDOutOrderType";

        #region New/Delete
        public static MDOutOrderType NewMDOutOrderType(DatabaseApp dbApp)
        {
            MDOutOrderType entity = new MDOutOrderType();
            entity.MDOutOrderTypeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.OrderType = GlobalApp.OrderTypes.Order;

            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDOutOrderType>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDOutOrderType>>(
            (database) => from c in database.MDOutOrderType where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDOutOrderType>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDOutOrderType>>(
            (database, index) => from c in database.MDOutOrderType where c.OrderTypeIndex == index select c
        );

        public static MDOutOrderType DefaultMDOutOrderType(DatabaseApp dbApp, GlobalApp.OrderTypes orderType)
        {
            try
            {
                MDOutOrderType defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)orderType).FirstOrDefault();
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
                return MDOutOrderTypeName;
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
        public String MDOutOrderTypeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDOutOrderTypeName");
            }
        }

        [NotMapped]
        public GlobalApp.OrderTypes OrderType
        {
            get
            {
                return (GlobalApp.OrderTypes)OrderTypeIndex;
            }
            set
            {
                OrderTypeIndex = (short)value;
                OnPropertyChanged("OrderType");
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
