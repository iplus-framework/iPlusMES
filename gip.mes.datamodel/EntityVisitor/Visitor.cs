using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Visitor'}de{'Besucher'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "VisitorNo", "en{'Visitor No.'}de{'Besucher Nr.'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(3, "VisitorCompany", "en{'Company'}de{'Unternehmen'}", Const.ContextDatabase + "\\" + Company.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, "VisitorCompanyPerson", "en{'Visitor'}de{'Besucher'}", Const.ContextDatabase + "\\" + CompanyPerson.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, MDVisitorCard.ClassName, "en{'Chipcard'}de{'Chipkarte'}", Const.ContextDatabase + "\\" + MDVisitorCard.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, "VehicleFacility", "en{'Vehicle'}de{'Fahrzeug'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(7, "TrailerFacility", "en{'Trailer'}de{'Anhänger'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(8, "ScheduledFromDate", "en{'Scheduled from'}de{'Zutrittsgenehmigung von'}", "", "", true)]
    [ACPropertyEntity(9, "ScheduledToDate", "en{'Scheduled to'}de{'Zutrittsgenehmigung bis'}", "", "", true)]
    [ACPropertyEntity(10, "IsFinished", "en{'Is blocked/finished'}de{'Zutritt gesperrt/erledigt'}", "", "", true)]
    [ACPropertyEntity(11, "Comment", "en{'Comment'}de{'Kommentar'}", "", "", true)]
    [ACPropertyEntity(12, "VisitedCompany", "en{'Visited Company'}de{'Besuchtes Unternehmen'}", Const.ContextDatabase + "\\" + Company.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(13, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioCompany, Const.QueryPrefix + Visitor.ClassName, "en{'Visitor'}de{'Besucher'}", typeof(Visitor), Visitor.ClassName, "VisitorNo", "VisitorNo", new object[]
        {
                new object[] {Const.QueryPrefix + VisitorVoucher.ClassName, "en{'Visitor Voucher'}de{'Besucherbeleg'}", typeof(VisitorVoucher), VisitorVoucher.ClassName + "_" + Visitor.ClassName, "VisitorVoucherNo", "VisitorVoucherNo"}
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<Visitor>) })]
    [NotMapped]
    public partial class Visitor
    {
        [NotMapped]
        public const string ClassName = "Visitor";
        [NotMapped]
        public const string NoColumnName = "VisitorNo";
        [NotMapped]
        public const string FormatNewNo = "V{0}";

        #region New/Delete
        public static Visitor NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            Visitor entity = new Visitor();
            entity.VisitorID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.VisitorNo = secondaryKey;
            if (parentACObject is Company)
            {
                entity.VisitedCompany = parentACObject as Company;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
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
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return VisitorNo;
            }
        }

        /// <summary>
        /// Returns a related EF-Object which is in a Child-Relationship to this.
        /// </summary>
        /// <param name="className">Classname of the Table/EF-Object</param>
        /// <param name="filterValues">Search-Parameters</param>
        /// <returns>A Entity-Object as IACObject</returns>
        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && className == VisitorVoucher.ClassName)
            {
                Int16 visitorVoucherNo = 0;
                if (Int16.TryParse(filterValues[0], out visitorVoucherNo))
                    return this.VisitorVoucher_Visitor.Where(c => c.VisitorVoucherNo == visitorVoucherNo).FirstOrDefault();
            }
            return null;
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
            if (string.IsNullOrEmpty(VisitorNo))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "Key",
                    Message = "Key",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "Key"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "VisitorNo";
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
