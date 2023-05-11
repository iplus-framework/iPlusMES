using System;
using System.ComponentModel.DataAnnotations.Schema;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Visitor Voucher'}de{'Besucher Beleg'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "VisitorVoucherNo", "en{'Visitor Voucher No.'}de{'Besucherbeleg-Nr.'}", MinValue = 1, MaxValue = Int32.MaxValue)]
    [ACPropertyEntity(2, "CheckInDate", "en{'Check-in date'}de{'Anmeldedatum'}", "", "", true)]
    [ACPropertyEntity(3, "CheckOutDate", "en{'Check-out date'}de{'Abmeldedatum'}", "", "", true)]
    [ACPropertyEntity(4, MDVisitorVoucherState.ClassName, "en{'Status'}de{'Status'}", Const.ContextDatabase + "\\" + MDVisitorVoucherState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, Visitor.ClassName, "en{'Visitor'}de{'Besucher'}", Const.ContextDatabase + "\\" + Visitor.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, "TotalWeight", "en{'Total Weight'}de{'Gewicht beladen'}", "", "", true)]
    [ACPropertyEntity(7, "EmptyWeight", "en{'Empty Weight'}de{'Gewicht leer'}", "", "", true)]
    [ACPropertyEntity(8, "NetWeight", "en{'Net Weight'}de{'Nettogewicht'}", "", "", true)]
    [ACPropertyEntity(9, "LossWeight", "en{'Loss Weight'}de{'Schwund'}", "", "", true)]
    [ACPropertyEntity(10, "VehicleFacility", "en{'Vehicle'}de{'Fahrzeug'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(11, "TrailerFacility", "en{'Trailer'}de{'Anhänger'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(12, "VisitorCompany", "en{'Visitor Company'}de{'Besucher Unternehmen'}", Const.ContextDatabase + "\\" + Company.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(13, "VisitorCompanyPerson", "en{'Visitor Company Person'}de{'Besucher'}", Const.ContextDatabase + "\\" + CompanyPerson.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(14, MDVisitorCard.ClassName, "en{'Chipcard'}de{'Chipkarte'}", Const.ContextDatabase + "\\" + MDVisitorCard.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioCompany, Const.QueryPrefix + VisitorVoucher.ClassName, "en{'Visitor Voucher'}de{'Besucherbeleg'}", typeof(VisitorVoucher), VisitorVoucher.ClassName, "VisitorVoucherNo", "VisitorVoucherNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<VisitorVoucher>) })]
    public partial class VisitorVoucher
    {
        public const string ClassName = "VisitorVoucher";
        public const string NoColumnName = "VisitorVoucherNo";
        public const string FormatNewNo = null;

        #region New/Delete
        public static VisitorVoucher NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey, bool autoGetNewNo = true)
        {
            VisitorVoucher entity = new VisitorVoucher();
            entity.DefaultValuesACObject();
            entity.VisitorVoucherID = Guid.NewGuid();
            if (autoGetNewNo)
                entity.VisitorVoucherNo = System.Convert.ToInt32(secondaryKey);
            if (parentACObject != null && parentACObject is Visitor)
            {
                Visitor companyVisitor = parentACObject as Visitor;

                try
                {
                    if (companyVisitor.EntityState != EntityState.Added)
                    {
                        if (!companyVisitor.VisitorVoucher_Visitor_IsLoaded)
                            companyVisitor.VisitorVoucher_Visitor.AutoLoad(companyVisitor.VisitorVoucher_VisitorReference, companyVisitor);
                    }
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (Database.Root != null && Database.Root.Messages != null)
                        Database.Root.Messages.LogException(ClassName, Const.MN_NewACObject, msg);
                }
                entity.Visitor = companyVisitor;
            }
            entity.MDVisitorVoucherState = MDVisitorVoucherState.DefaultMDVisitorVoucherState(dbApp);
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        /// <summary>
        /// Deletes this entity-object from the database
        /// </summary>
        /// <param name="database">Entity-Framework databasecontext</param>
        /// <param name="withCheck">If set to true, a validation happens before deleting this EF-object. If Validation fails message ís returned.</param>
        /// <param name="softDelete">If set to true a delete-Flag is set in the dabase-table instead of a physical deletion. If  a delete-Flag doesn't exit in the table the record will be deleted.</param>
        /// <returns>If a validation or deletion failed a message is returned. NULL if sucessful.</returns>
        public override MsgWithDetails DeleteACObject(IACEntityObjectContext database, bool withCheck, bool softDelete = false)
        {
            if (withCheck)
            {
                MsgWithDetails msg = IsEnabledDeleteACObject(database);
                if (msg != null)
                    return msg;
            }
            Visitor companyPerson = Visitor;
            database.Remove(this);
            return null;
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
                return VisitorVoucherNo.ToString();
            }
        }

        /// <summary>
        /// Returns Visitor
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Visitor</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return Visitor;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "VisitorVoucherNo";
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


