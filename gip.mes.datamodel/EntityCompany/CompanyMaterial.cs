using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioCompany, ConstApp.CompanyMaterial, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, nameof(CompanyMaterialNo), ConstApp.CompanyMaterialNo, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, nameof(CompanyMaterialName), ConstApp.CompanyMaterialName, "", "", true)]
    [ACPropertyEntity(3, nameof(Comment), ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(4, Company.ClassName, ConstApp.Company, Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(5, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(6, MDUnit.ClassName, ConstApp.MDUnit, Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(7, nameof(ValidFromDate), ConstApp.ValidFromDate, "", "", true)]
    [ACPropertyEntity(8, nameof(ValidToDate), ConstApp.ValidToDate, "", "", true)]
    [ACPropertyEntity(9, nameof(Blocked), "en{'Blocked'}de{'Gesperrt'}", "", "", true)]
    [ACPropertyEntity(10, nameof(CMTypeIndex), "en{'Type'}de{'Typ'}", typeof(GlobalApp.CompanyMaterialTypes), Const.ContextDatabase + "\\CompanyMaterialTypeList", "", true)]
    [ACPropertyEntity(11, nameof(MinStockQuantity), ConstApp.MinStockQuantity, "", "", true)]
    [ACPropertyEntity(12, nameof(OptStockQuantity), ConstApp.OptStockQuantity, "", "", true)]
    [ACPropertyEntity(13, "Tax", "en{'Taxed'}de{'Versteuert'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioCompany, Const.QueryPrefix + CompanyMaterial.ClassName, ConstApp.CompanyMaterial, typeof(CompanyMaterial), CompanyMaterial.ClassName, "CompanyMaterialNo", "CompanyMaterialNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<CompanyMaterial>) })]
    public partial class CompanyMaterial
    {
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
        public override IACObject ParentACObject
        {
            get
            {
                return Company;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "CompanyMaterialNo";
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
