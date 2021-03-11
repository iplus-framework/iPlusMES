using gip.core.datamodel;
using System;

namespace gip.mes.datamodel
{

    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESTax, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOTax")]
    [ACPropertyEntity(1, "TaxNo", "en{'TaxNo'}de{'MwStNr'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, "DateFrom", "en{'Start time'}de{'Startzeit'}", "", "", true)]
    [ACPropertyEntity(3, "DateTo", "en{'End Time'}de{'Endzeit'}", "", "", true)]
    [ACPropertyEntity(4, "DefaultTaxValue", "en{'Tax'}de{'MwStr'}", "", "", true)]

    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]

    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + Tax.ClassName, ConstApp.ESTax, typeof(Tax), Tax.ClassName, "TaxNo", "TaxNo")]

    [ACSerializeableInfo(new Type[] { typeof(ACRef<Tax>) })]
    public partial class Tax
    {
        public const string ClassName = "Tax";
        public const string NoColumnName = "TaxNo";
        public const string FormatNewNo = "TAX{0}";

        #region New/Delete

        public static Tax NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            Tax entity = new Tax();
            entity.TaxID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.TaxNo = secondaryKey;
            entity.DateFrom = DateTime.Now;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }

        #endregion
    }
}
