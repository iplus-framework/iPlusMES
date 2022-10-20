using gip.core.datamodel;
using System;
using System.Collections.Generic;

namespace gip.mes.datamodel
{
    // MaterialUnit (Artikel)
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Material Calculation'}de{'Materialkalkulation'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "MaterialCalculationNo", "en{'Calculation No.'}de{'Kalkulationsnr.'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(3, "ProductionQuantity", "en{'Production Quantity'}de{'Produktionsmenge'}", "", "", true)]
    [ACPropertyEntity(4, "ValidFromDate", ConstApp.ValidFromDate, "", "", true)]
    [ACPropertyEntity(5, "ValidToDate", ConstApp.ValidToDate, "", "", true)]
    [ACPropertyEntity(6, "CostReQuantity", "en{'CostReQuantity'}de{'de-CostReQuantity'}", "", "", true)]
    [ACPropertyEntity(7, "CostMat", ConstApp.CostMat, "", "", true)]
    [ACPropertyEntity(8, "CostVar", ConstApp.CostVar, "", "", true)]
    [ACPropertyEntity(9, "CostFix", ConstApp.CostFix, "", "", true)]
    [ACPropertyEntity(10, "CostPack", ConstApp.CostPack, "", "", true)]
    [ACPropertyEntity(11, "CostGeneral", ConstApp.CostGeneral, "", "", true)]
    [ACPropertyEntity(12, "CostLoss", ConstApp.CostLoss, "", "", true)]
    [ACPropertyEntity(13, "CostHandlingVar", "en{'CostHandlingVar'}de{'Bearbeitungskosten variabel'}", "", "", true)]
    [ACPropertyEntity(14, "CostHandlingFix", "en{'CostHandlingFix'}de{'Bearbeitungskosten fix'}", "", "", true)]
    [ACPropertyEntity(15, "CalculationDate", "en{'Calculation Date'}de{'Kalkulation vom'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MaterialCalculation.ClassName, "en{'MaterialCalculation'}de{'Materialkalkulation'}", typeof(MaterialCalculation), MaterialCalculation.ClassName, "MaterialCalculationNo", "MaterialCalculationNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaterialUnit>) })]
    public partial class MaterialCalculation
    {
        public const string ClassName = "MaterialCalculation";
        public const string NoColumnName = "MaterialCalculationNo";
        public const string FormatNewNo = "MC{0}";

        #region New/Delete
        public static MaterialCalculation NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            MaterialCalculation entity = new MaterialCalculation();
            entity.MaterialCalculationID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Material)
            {
                Material material = parentACObject as Material;
                entity.Material = material;
                material.MaterialCalculation_Material.Add(entity);
                entity.MaterialCalculationNo = secondaryKey;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            if (Material == null)
                return ACCaption;
            return Material.MaterialName1;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                return MaterialCalculationNo;
            }
        }

        /// <summary>
        /// Returns Material
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Material</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return Material;
            }
        }

        #endregion

        #region IACObjectEntity Members
        /// <summary>
        /// Method for validating values and references in this EF-Object.
        /// Is called from Change-Tracking before changes will be saved for new unsaved entity-objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">Entity-Framework databasecontext</param>
        /// <returns>NULL if sucessful otherwise a Message-List</returns>
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            if (Material == null)
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = Material.ClassName,
                    Message = Material.ClassName,
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", Material.ClassName), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

        static public string KeyACIdentifier
        {
            get
            {
                return "MaterialCalculationNo";
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
    }
}
