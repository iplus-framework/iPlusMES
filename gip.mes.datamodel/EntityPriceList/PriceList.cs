using gip.core.datamodel;
using System;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESPriceList, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOPriceList")]
    [ACPropertyEntity(1, "PriceListNo", "en{'Price list No'}de{'Preislistenummer'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, "DateFrom", "en{'Start time'}de{'Startzeit'}", "", "", true)]
    [ACPropertyEntity(3, "DateTo", "en{'End Time'}de{'Endzeit'}", "", "", true)]
    [ACPropertyEntity(4, "Comment", "en{'Comment'}de{'Kommentar'}", "", "", true)]

    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]

    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + PriceList.ClassName, ConstApp.ESPriceList, typeof(PriceList), PriceList.ClassName, "PriceListNo", "PriceListNo")]

    [ACSerializeableInfo(new Type[] { typeof(ACRef<PriceList>) })]
    public partial class PriceList
    {
        public const string ClassName = "PriceList";
        public const string NoColumnName = "PriceListNo";
        public const string FormatNewNo = "PriceList{0}";

        #region New/Delete

        public static PriceList NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            PriceList entity = new PriceList();
            entity.PriceListID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.PriceListNo = secondaryKey;
            entity.DateFrom = DateTime.Now;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }

        #endregion
    }
}
