using gip.core.datamodel;
using System;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Bill of Materials Stock'}de{'Stücklistenbestand'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Partslist", ConstApp.BOM, Const.ContextDatabase + "\\" + Partslist.ClassName, "", true)]
    [ACPropertyEntity(2, MDReleaseState.ClassName, ConstApp.ESReleaseState, Const.ContextDatabase + "\\" + MDReleaseState.ClassName, "", true)]
    [ACPropertyEntity(3, "StockQuantity", ConstApp.StockQuantity, "", "", true)]
    [ACPropertyEntity(4, "StockWeight", ConstApp.StockWeight, "", "", true)]

    [ACPropertyEntity(5, "DayInward", ConstApp.DayInward, "", "", true)]
    [ACPropertyEntity(6, "DayTargetInward", ConstApp.DayTargetInward, "", "", true)]
    // 7 DayInwardDiff
    // 8 DayInwardDiffPercent
    [ACPropertyEntity(9, "DayOutward", ConstApp.DayOutward, "", "", true)]
    [ACPropertyEntity(10, "DayTargetOutward", ConstApp.DayTargetOutward, "", "", true)]
    // 11 DayOutwardDiff
    // 12 DayOutwardDiffPercent
    [ACPropertyEntity(13, "WeekInward", ConstApp.WeekInward, "", "", true)]
    [ACPropertyEntity(14, "WeekTargetInward", ConstApp.WeekTargetInward, "", "", true)]
    // 15 WeekInwardDiff
    // 16 WeekInwardDiffPercent
    [ACPropertyEntity(17, "WeekOutward", ConstApp.WeekOutward, "", "", true)]
    [ACPropertyEntity(18, "WeekTargetOutward", ConstApp.WeekTargetOutward, "", "", true)]
    // 19 WeekOutwardDiff
    // 20 WeekOutwardDiffPercent
    [ACPropertyEntity(21, "MonthInward", ConstApp.MonthInward, "", "", true)]
    [ACPropertyEntity(22, "MonthTargetInward", ConstApp.MonthTargetInward, "", "", true)]
    // 23 MonthInwardDiff
    // 24 MonthInwardDiffPercent
    [ACPropertyEntity(25, "MonthOutward", ConstApp.MonthOutward, "", "", true)]
    [ACPropertyEntity(26, "MonthTargetOutward", ConstApp.MonthTargetOutward, "", "", true)]
    // 27 MonthOutwardDiff
    // 28 MonthOutwardDiffPercent
    [ACPropertyEntity(29, "YearInward", ConstApp.YearInward, "", "", true)]
    [ACPropertyEntity(30, "YearTargetInward", ConstApp.YearTargetInward, "", "", true)]
    // 31 YearInwardDiff
    // 32 YearInwardDiffPercent
    [ACPropertyEntity(33, "YearOutward", ConstApp.YearOutward, "", "", true)]
    [ACPropertyEntity(34, "YearTargetOutward", ConstApp.YearTargetOutward, "", "", true)]
    // 35 YearOutwardDiff
    // 36 YearOutwardDiffPercent
    [ACPropertyEntity(37, "DayAdjustment", ConstApp.DayAdjustment, "", "", true)]
    [ACPropertyEntity(38, "DayBalanceDate", ConstApp.DayBalanceDate, "", "", true)]
    [ACPropertyEntity(40, "DayLastOutward", ConstApp.DayLastOutward, "", "", true)]
    [ACPropertyEntity(41, "DayLastStock", ConstApp.DayLastStock, "", "", true)]
    [ACPropertyEntity(42, "WeekAdjustment", ConstApp.WeekAdjustment, "", "", true)]
    [ACPropertyEntity(43, "WeekBalanceDate", ConstApp.WeekBalanceDate, "", "", true)]
    [ACPropertyEntity(44, "MonthActStock", ConstApp.MonthActStock, "", "", true)]
    [ACPropertyEntity(45, "MonthAdjustment", ConstApp.MonthAdjustment, "", "", true)]
    [ACPropertyEntity(46, "MonthAverageStock", ConstApp.MonthAverageStock, "", "", true)]
    [ACPropertyEntity(47, "MonthBalanceDate", ConstApp.MonthBalanceDate, "", "", true)]
    [ACPropertyEntity(49, "MonthLastOutward", ConstApp.MonthLastOutward, "", "", true)]
    [ACPropertyEntity(50, "MonthLastStock", ConstApp.MonthLastStock, "", "", true)]
    [ACPropertyEntity(51, "YearAdjustment", ConstApp.YearAdjustment, "", "", true)]
    [ACPropertyEntity(52, "YearBalanceDate", ConstApp.YearBalanceDate, "", "", true)]
    [ACPropertyEntity(53, "ReservedInwardQuantity", ConstApp.ReservedInwardQuantity, "", "", true)]
    [ACPropertyEntity(54, "ReservedOutwardQuantity", ConstApp.ReservedOutwardQuantity, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]

    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + PartslistStock.ClassName, "en{'Bill of Materials Stock'}de{'Stücklistenbestand'}", typeof(PartslistStock), PartslistStock.ClassName, Partslist.ClassName + "\\PartslistNo", Partslist.ClassName + "\\PartslistNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<PartslistStock>) })]
    public partial class PartslistStock
    {
        public const string ClassName = "PartslistStock";

        #region New/Delete
        public static PartslistStock NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            PartslistStock entity = new PartslistStock();
            entity.PartslistStockID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Partslist)
            {
                entity.Partslist = parentACObject as Partslist;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACObject Member


        /// <summary>
        /// Returns Partslist
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Partslist</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return Partslist;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "Partslist\\PartslistNo";
            }
        }
        #endregion

        #region IEntityProperty Members

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
