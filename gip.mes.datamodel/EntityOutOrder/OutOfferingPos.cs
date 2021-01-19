using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'Offering Pos'}de{'Angebotsbotsposition'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Folge'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(3, "TargetQuantityUOM", ConstApp.TargetQuantityUOM, "", "", true)]
    [ACPropertyEntity(4, MDUnit.ClassName, ConstApp.MDUnit, Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(6, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(7, "TargetWeight", "en{'Target Weight'}de{'Zielgewicht'}", "", "", true)]
    [ACPropertyEntity(8, "TargetDeliveryDate", "en{'Target Delivery Date'}de{'Gewünschter Liefertermin'}", "", "", true)]
    [ACPropertyEntity(9, "TargetDeliveryMaxDate", "en{'Latest Delivery Date'}de{'Spätester Liefertermin'}", "", "", true)]
    [ACPropertyEntity(10, "TargetDeliveryPriority", "en{'Delivery Priority'}de{'Lieferpriorität'}", "", "", true)]
    [ACPropertyEntity(11, MDTimeRange.ClassName, "en{'Time Range'}de{'Zeitraum'}", Const.ContextDatabase + "\\" + MDTimeRange.ClassName, "", true)]
    [ACPropertyEntity(12, "PriceNet", ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(13, "PriceGross", ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(14, MDCountrySalesTax.ClassName, "en{'Sales Tax'}de{'Umsatzsteuer'}", Const.ContextDatabase + "\\" + MDCountrySalesTax.ClassName, "", true)]
    [ACPropertyEntity(15, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(16, "Comment2", "en{'Comment 2'}de{'Bemerkung 2'}", "", "", true)]
    [ACPropertyEntity(9999, OutOffering.ClassName, "en{'Offer'}de{'Angebot'}", "", "", true)]
    [ACPropertyEntity(9999, "MaterialPosTypeIndex", "en{'Position Type'}de{'Posistionstyp'}", typeof(GlobalApp.MaterialPosTypes), "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + OutOfferingPos.ClassName, "en{'Offer Position'}de{'Angebotsposition'}", typeof(OutOfferingPos), OutOfferingPos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<OutOfferingPos>) })]
    public partial class OutOfferingPos
    {
        public const string ClassName = "OutOfferingPos";

        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static OutOfferingPos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            OutOfferingPos entity = new OutOfferingPos();
            entity.OutOfferingPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is OutOffering)
            {
                OutOffering outOffering = parentACObject as OutOffering;
                try
                {
                    if (outOffering.EntityState != System.Data.EntityState.Added)
                    {
                        if (!outOffering.OutOfferingPos_OutOffering.IsLoaded)
                            outOffering.OutOfferingPos_OutOffering.Load();
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

                if (outOffering.OutOfferingPos_OutOffering != null && outOffering.OutOfferingPos_OutOffering.Select(c => c.Sequence).Any())
                    entity.Sequence = outOffering.OutOfferingPos_OutOffering.Select(c => c.Sequence).Max() + 1;
                else
                    entity.Sequence = 1;
                entity.OutOffering = outOffering;
            }
            entity.MaterialPosTypeIndex = (Int16)GlobalApp.MaterialPosTypes.OutwardRoot;
            entity.TargetQuantityUOM = 1;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
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
            int sequence = Sequence;
            OutOffering outOffering = OutOffering;
            database.DeleteObject(this);
            OutOfferingPos.RenumberSequence(outOffering, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(OutOffering OutOffering, int sequence)
        {
            var elements = from c in OutOffering.OutOfferingPos_OutOffering where c.Sequence > sequence orderby c.Sequence select c;
            int sequenceCount = sequence;
            foreach (var element in elements)
            {
                element.Sequence = sequence;
                sequence++;
            }
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
                return Sequence.ToString() + " " + Material.ACCaption;
            }
        }

        /// <summary>
        /// Returns OutOffering
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to OutOffering</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return OutOffering;
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
            if (this.Material == null)
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

        static public string KeyACIdentifier
        {
            get
            {
                return "Sequence";
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

        [ACPropertyInfo(9999)]
        public string Position
        {
            get
            {
                return "Position " + Sequence.ToString();
            }
        }
    }
}
