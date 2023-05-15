using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Company Material'}de{'Unternehmensmaterial'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "CompanyMaterialNo", "en{'External Material-No.'}de{'Externe Material-Nr.'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, "CompanyMaterialName", "en{'External Material Name'}de{'Externer Materialname'}", "", "", true)]
    [ACPropertyEntity(3, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(4, Company.ClassName, "en{'Company'}de{'Unternehmen'}", Const.ContextDatabase + "\\" + Company.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, MDUnit.ClassName, ConstApp.MDUnit, Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(7, "ValidFromDate", ConstApp.ValidFromDate, "", "", true)]
    [ACPropertyEntity(8, "ValidToDate", ConstApp.ValidToDate, "", "", true)]
    [ACPropertyEntity(9, "Blocked", "en{'Blocked'}de{'Gesperrt'}", "", "", true)]
    [ACPropertyEntity(10, "CMTypeIndex", "en{'Type'}de{'Typ'}", typeof(GlobalApp.CompanyMaterialTypes), Const.ContextDatabase + "\\CompanyMaterialTypeList", "", true)]
    [ACPropertyEntity(11, "MinStockQuantity", ConstApp.MinStockQuantity, "", "", true)]
    [ACPropertyEntity(12, "OptStockQuantity", ConstApp.OptStockQuantity, "", "", true)]
    [ACPropertyEntity(13, "Tax", "en{'Taxed'}de{'Versteuert'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioCompany, Const.QueryPrefix + CompanyMaterial.ClassName, "en{'Company Material'}de{'Unternehmensmaterial'}", typeof(CompanyMaterial), CompanyMaterial.ClassName, "CompanyMaterialNo", "CompanyMaterialNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<CompanyMaterial>) })]
    [NotMapped]
    public partial class CompanyMaterial
    {
        [NotMapped]
        public const string ClassName = "CompanyMaterial";

        #region New/Delete
        public static CompanyMaterial NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            CompanyMaterial entity = new CompanyMaterial();
            entity.CompanyMaterialID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Company)
            {
                entity.Company = parentACObject as Company;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region Hilfsfunktionen
        /// <summary>
        /// Gets the material stock_ insert if not exists.
        /// </summary>
        /// <param name="dbApp">The database.</param>
        /// <returns>MaterialStock.</returns>
        public CompanyMaterialStock GetCompanyMaterialStock_InsertIfNotExists(DatabaseApp dbApp)
        {
            CompanyMaterialStock materialStock = CompanyMaterialStock_CompanyMaterial.FirstOrDefault();
            if (materialStock != null)
                return materialStock;
            materialStock = CompanyMaterialStock.NewACObject(dbApp, this);
            return materialStock;
        }

        [NotMapped]
        public CompanyMaterialStock CurrentCompanyMaterialStock
        {
            get
            {
                return CompanyMaterialStock_CompanyMaterial.FirstOrDefault();
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
                return CompanyMaterialNo + " " + CompanyMaterialName;
            }
        }

        /// <summary>
        /// Returns Company
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Company</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return Company;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "CompanyMaterialNo";
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

        [NotMapped]
        public GlobalApp.CompanyMaterialTypes CMMype
        {
            get
            {
                return (GlobalApp.CompanyMaterialTypes)CMTypeIndex;
            }
            set
            {
                CMTypeIndex = (Int16)value;
            }
        }
    }
}
