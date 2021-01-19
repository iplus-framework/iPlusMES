using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions; using gip.core.datamodel;

namespace gip.mes.datamodel
{
    // CompanyMaterialPickup (LagerHistorie)
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Companymaterial pickup'}de{'Unternehmensmaterial Abholung'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, CompanyMaterial.ClassName, "en{'Company material'}de{'Unternehmensmaterial'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName, "", true)]
    [ACPropertyEntity(9999, OutOrderPos.ClassName, "en{'Outorderpos'}de{'Auftragsposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName, "", true)]
    [ACPropertyEntity(9999, InOrderPos.ClassName, "en{'Inorderpos'}de{'Bestellposition'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName, "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + CompanyMaterialPickup.ClassName, "en{'Companymaterial pickup'}de{'Unternehmensmaterial Abholung'}", typeof(CompanyMaterialPickup), CompanyMaterialPickup.ClassName, "", "CompanyMaterialPickupID")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<CompanyMaterialPickup>) })]
    public partial class CompanyMaterialPickup : IACObjectEntity
    {
        public const string ClassName = "CompanyMaterialPickup";

        #region New/Delete
        public static CompanyMaterialPickup NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            CompanyMaterialPickup entity = new CompanyMaterialPickup();
            entity.CompanyMaterialPickupID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is CompanyMaterial)
            {
                CompanyMaterial companyMaterial = parentACObject as CompanyMaterial;
                entity.CompanyMaterial = companyMaterial;
                companyMaterial.CompanyMaterialPickup_CompanyMaterial.Add(entity);
            }
            else if (parentACObject is OutOrderPos)
            {
                OutOrderPos outOrderPos = parentACObject as OutOrderPos;
                entity.OutOrderPos = outOrderPos;
                outOrderPos.CompanyMaterialPickup_OutOrderPos.Add(entity);
            }
            else if (parentACObject is InOrderPos)
            {
                InOrderPos InOrderPos = parentACObject as InOrderPos;
                entity.InOrderPos = InOrderPos;
                InOrderPos.CompanyMaterialPickup_InOrderPos.Add(entity);
            }
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
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
                return CompanyMaterial.ACCaption;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "CompanyMaterial\\CompanyMaterialNo";
            }
        }

        #endregion

        //public object this[string property]
        //{
        //    get
        //    {
        //        return ACProperties[property];
        //    }
        //    set
        //    {
        //        ACProperties[property] = value;
        //    }
        //}

        //bool bRefreshConfig = false;
        //partial void OnXMLConfigChanging(global::System.String value)
        //{
        //    bRefreshConfig = false;
        //    if (this.EntityState != System.Data.EntityState.Detached && (!(String.IsNullOrEmpty(value) && String.IsNullOrEmpty(XMLConfig)) && value != XMLConfig))
        //        bRefreshConfig = true;
        //}

        //partial void OnXMLConfigChanged()
        //{
        //    if (bRefreshConfig)
        //        ACProperties.Refresh();
        //}


        #region Partial Properties
        #endregion

    }
}



