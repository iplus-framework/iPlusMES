using gip.core.datamodel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESPriceList, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOPriceList")]
    [ACPropertyEntity(1, "PriceListNo", "en{'Price list No'}de{'Preislistenummer'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(9999, "PriceListNameTrans", Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(9999, "MDCurrency", "en{'Currency'}de{'Währung'}", Const.ContextDatabase + "\\" + MDCurrency.ClassName, "", true)]
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
        [NotMapped]
        public const string ClassName = "PriceList";
        [NotMapped]
        public const string NoColumnName = "PriceListNo";
        [NotMapped]
        public const string FormatNewNo = "PriceList{0}";

        #region New/Delete

        public static PriceList NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            PriceList entity = new PriceList();
            entity.PriceListID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.PriceListNo = secondaryKey;
            entity.DateFrom = DateTime.Now;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region Properties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String PriceListName
        {
            get
            {
                return Translator.GetTranslation(PriceListNameTrans);
            }
            set
            {
                PriceListNameTrans = Translator.SetTranslation(PriceListNameTrans, value);
                OnPropertyChanged("PriceListName");
            }
        }
        #endregion
    }
}
