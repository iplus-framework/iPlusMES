﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESInvoiceType, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOInvoiceType")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(1, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "InvoiceTypeIndex", ConstApp.ESInvoiceType, typeof(GlobalApp.InvoiceTypes), Const.ContextDatabase + "\\InvoiceTypesList", "", true, MinValue = (short)GlobalApp.InvoiceTypes.Invoice)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + MDInvoiceType.ClassName, ConstApp.ESInvoiceType, typeof(MDInvoiceType), MDInvoiceType.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDInvoiceType>) })]
    [NotMapped]
    public partial class MDInvoiceType
    {
        [NotMapped]
        public const string ClassName = "MDInvoiceType";

        #region New/Delete
        public static MDInvoiceType NewACObject(DatabaseApp dbApp)
        {
            MDInvoiceType entity = new MDInvoiceType();
            entity.MDInvoiceTypeID = Guid.NewGuid();
            entity.Context = dbApp;
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.InvoiceType = GlobalApp.InvoiceTypes.Invoice;

            return entity;
        }


        static readonly Func<DatabaseApp, IEnumerable<MDInvoiceType>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IEnumerable<MDInvoiceType>>(
            (database) => from c in database.MDInvoiceType where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IEnumerable<MDInvoiceType>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IEnumerable<MDInvoiceType>>(
            (database, index) => from c in database.MDInvoiceType where c.InvoiceTypeIndex == index select c
        );

        public static MDInvoiceType DefaultMDInvoiceType(DatabaseApp dbApp)
        {
            try
            {
                MDInvoiceType defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)GlobalApp.InvoiceTypes.Invoice).FirstOrDefault();
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
                return MDInvoiceTypeName;
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
        public String MDInvoiceTypeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDInvoiceTypeName");
            }
        }

        [NotMapped]
        public GlobalApp.InvoiceTypes InvoiceType
        {
            get
            {
                return (GlobalApp.InvoiceTypes)InvoiceTypeIndex;
            }
            set
            {
                InvoiceTypeIndex = (short)value;
                OnPropertyChanged("InvoiceType");
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
