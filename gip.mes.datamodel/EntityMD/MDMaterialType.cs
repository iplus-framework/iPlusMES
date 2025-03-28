// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 10.01.2018
// ***********************************************************************
// <copyright file="ACFacility.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.ESMaterialType, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOMaterialType")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDMaterialTypeIndex", ConstApp.ESMaterialType, typeof(MDMaterialGroup.MaterialGroupTypes), "", "", true, MinValue = (short)MDMaterialGroup.MaterialGroupTypes.Undefined)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MDMaterialType.ClassName, ConstApp.ESMaterialType, typeof(MDMaterialType), MDMaterialType.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDMaterialType>) })]
    [NotMapped]
    public partial class MDMaterialType
    {
        [NotMapped]
        public const string ClassName = "MDMaterialType";

        #region New/Delete
        public static MDMaterialType NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDMaterialType entity = new MDMaterialType();
            entity.MDMaterialTypeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.MaterialType = MaterialTypes.Undefined;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IEnumerable<MDMaterialType>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IEnumerable<MDMaterialType>>(
            (database) => from c in database.MDMaterialType where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IEnumerable<MDMaterialType>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IEnumerable<MDMaterialType>>(
            (database, index) => from c in database.MDMaterialType where c.MDMaterialTypeIndex == index select c
        );

        public static MDMaterialType DefaultMDMaterialType(DatabaseApp dbApp)
        {
            try
            {
                MDMaterialType defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)MaterialTypes.Undefined).FirstOrDefault();
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
                return MDMaterialTypeName;
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
        public String MDMaterialTypeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDMaterialTypeName");
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

#region enums
        [NotMapped]
        public MaterialTypes MaterialType
        {
            get
            {
                return (MaterialTypes)MDMaterialTypeIndex;
            }
            set
            {
                MDMaterialTypeIndex = (short)value;
                OnPropertyChanged("MaterialType");
            }
        }

        /// <summary>
        /// Enum für das Feld MDMaterialGroupIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'MaterialTypes'}de{'Material-Typen'}", Global.ACKinds.TACEnum)]
        public enum MaterialTypes : short
        {
            Undefined = 0,
        }
#endregion

    }
}




