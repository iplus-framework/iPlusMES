using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Transactions; 
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Inventory'}de{'Inventur'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "FacilityInventoryNo", "en{'Inventory Nr.'}de{'Inventurno.'}","", "", true)]
    [ACPropertyEntity(2, "FacilityInventoryName", "en{'Inventoryname'}de{'Inventurname'}","", "", true)]
    [ACPropertyEntity(3, "MDFacilityInventoryState", "en{'Inventorystate'}de{'Inventurstatus'}", Const.ContextDatabase + "\\" + MDFacilityInventoryState.ClassName, "", true)]
    [ACPropertyEntity(4, Facility.ClassName, ConstApp.Facility, Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityInventory.ClassName, "en{'Inventory'}de{'Inventur'}", typeof(FacilityInventory), FacilityInventory.ClassName, "FacilityInventoryNo", "FacilityInventoryNo")] // TODO: Defin child entities to FacilityInventoryPos.ClassName
    [ACSerializeableInfo(new Type[] { typeof(ACRef<FacilityInventory>) })]
    public partial class FacilityInventory
    {

        #region constants

        public const string ClassName = "FacilityInventory";
        public const string NoColumnName = "FacilityInventoryNo";
        public const string FormatNewNo = "";
        #endregion

        #region New/Delete
        public static FacilityInventory NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            FacilityInventory entity = new FacilityInventory();
            entity.FacilityInventoryID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.FacilityInventoryNo = secondaryKey;
            entity.MDFacilityInventoryState = MDFacilityInventoryState.DefaultMDFacilityInventoryState(dbApp);
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
                return FacilityInventoryName;
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
            if (filterValues.Any() && className == FacilityInventoryPos.ClassName)
            {
                Int32 sequence = 0;
                if (Int32.TryParse(filterValues[0], out sequence))
                    return this.FacilityInventoryPos_FacilityInventory.Where(c => c.Sequence == sequence).FirstOrDefault();
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
            if (string.IsNullOrEmpty(FacilityInventoryName))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "FacilityInventoryName",
                    Message = "FacilityInventoryName is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "FacilityBookingChargeNo"), 
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
                return "FacilityInventoryName";
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

        #region Other methods

        public bool IsFaciltiyMatch(Facility facility)
        {
            return
                FacilityID == null
                || facility == null
                || FacilityID == facility.FacilityID
                || facility.IsLocatedIn(FacilityID ?? Guid.Empty);
        }

        #endregion

    }
}
