using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Visitor Voucher'}de{'Besucher Beleg'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "VisitorVoucherNo", "en{'Visitor Voucher No.'}de{'Besucherbeleg-Nr.'}", MinValue = 1, MaxValue = Int32.MaxValue)]
    [ACPropertyEntity(2, "CheckInDate", "en{'Check-in date'}de{'Anmeldedatum'}", "", "", true)]
    [ACPropertyEntity(3, "CheckOutDate", "en{'Check-out date'}de{'Abmeldedatum'}", "", "", true)]
    [ACPropertyEntity(4, MDVisitorVoucherState.ClassName, "en{'Status'}de{'Status'}", Const.ContextDatabase + "\\" + MDVisitorVoucherState.ClassName, "", true)]
    [ACPropertyEntity(5, Visitor.ClassName, "en{'Visitor'}de{'Besucher'}", Const.ContextDatabase + "\\" + Visitor.ClassName, "", true)]
    [ACPropertyEntity(6, "TotalWeight", "en{'Total Weight'}de{'Gewicht beladen'}", "", "", true)]
    [ACPropertyEntity(7, "EmptyWeight", "en{'Empty Weight'}de{'Gewicht leer'}", "", "", true)]
    [ACPropertyEntity(8, "NetWeight", "en{'Net Weight'}de{'Nettogewicht'}", "", "", true)]
    [ACPropertyEntity(9, "LossWeight", "en{'Loss Weight'}de{'Schwund'}", "", "", true)]
    [ACPropertyEntity(10, "VehicleFacility", "en{'Vehicle'}de{'Fahrzeug'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(11, "TrailerFacility", "en{'Trailer'}de{'Anhänger'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(12, "VisitorCompany", "en{'Visitor Company'}de{'Besucher Unternehmen'}", Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(13, "VisitorCompanyPerson", "en{'Visitor Company Person'}de{'Besucher'}", Const.ContextDatabase + "\\" + CompanyPerson.ClassName, "", true)]
    [ACPropertyEntity(14, MDVisitorCard.ClassName, "en{'Chipcard'}de{'Chipkarte'}", Const.ContextDatabase + "\\" + MDVisitorCard.ClassName, "", true)]
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
                    if (companyVisitor.EntityState != System.Data.EntityState.Added)
                    {
                        if (!companyVisitor.VisitorVoucher_Visitor.IsLoaded)
                            companyVisitor.VisitorVoucher_Visitor.Load();
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
            database.DeleteObject(this);
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
        public override IACObject ParentACObject
        {
            get
            {
                return Visitor;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "VisitorVoucherNo";
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

        #region Additional Members

        /// <summary>
        /// Picking status info
        /// </summary>
        private string _PickingStatusInfo;
        [ACPropertyInfo(999, nameof(PickingStatusInfo), "en{'Picking status'}de{'Verladestatus'}")]
        public string PickingStatusInfo
        {
            get
            {
                if (_PickingStatusInfo == null)
                {
                    _PickingStatusInfo = LoadPickingStatusInfo();
                }
                return _PickingStatusInfo;
            }
            set
            {
                if (_PickingStatusInfo != value)
                {
                    _PickingStatusInfo = value;
                    OnPropertyChanged("PickingStatusInfo");
                }
            }
        }

        #endregion

        #region Helper mehtods

        private string LoadPickingStatusInfo()
        {
            StringBuilder sb = new StringBuilder();

            if (Picking_VisitorVoucher != null && Picking_VisitorVoucher.Any())
            {
                List<Picking> pickings = Picking_VisitorVoucher.OrderBy(c => c.PickingNo).ToList();
                foreach (Picking picking in pickings)
                {
                    string pickingState = "";
                    ACValueItem acvalueItem = (this.GetObjectContext() as DatabaseApp).PickingStateList[picking.PickingStateIndex];
                    if(acvalueItem != null)
                    {
                        pickingState = acvalueItem.ACCaption;
                    }
                    sb.Append($"{picking.PickingNo} - {pickingState}");
                    if(pickings.IndexOf(picking) < (pickings.Count - 1))
                    {
                        sb.Append(Environment.NewLine);
                    }
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}


