using gip.core.datamodel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Demandorderposistion'}de{'Anforderungsauftragsposition'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Sequenz'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(4, "TargetQuantityUOM", "en{'Target Qty. (UOM)'}de{'Sollmenge (BME)'}", "", "", true)]
    [ACPropertyEntity(5, MDUnit.ClassName, "en{'Unit of Measurement'}de{'Maßeinheit'}", Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(6, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(7, "TargetWeight", "en{'Target Weight'}de{'Zielgewicht'}", "", "", true)]
    [ACPropertyEntity(8, Partslist.ClassName, "en{'Bill of Materials'}de{'Stückliste'}", Const.ContextDatabase + "\\" + Partslist.ClassName, "", true)]
    [ACPropertyEntity(10, "TargetDeliveryDate", "en{'Target Delivery Date'}de{'Gepl. Lieferdatum'}", "", "", true)]
    [ACPropertyEntity(11, "TargetDeliveryMaxDate", "en{'Max Target Deliverydate'}de{'max.Gepl.Lieferdatum'}", "", "", true)]
    [ACPropertyEntity(12, "TargetDeliveryPriority", "en{'Delivery Priority'}de{'Lieferpriorität'}", "", "", true)]
    [ACPropertyEntity(13, "Comment", "en{'Comment'}de{'Bemerkung'}", "", "", true)]
    [ACPropertyEntity(14, "LineNumber", "en{'Item Number'}de{'Positionsnummer'}", "", "", true)]
    [ACPropertyEntity(9999, "DemandOrder", "en{'Demandorder'}de{'Anforderungsauftrag'}", Const.ContextDatabase + "\\DemandOrderList", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + DemandOrderPos.ClassName, "en{'Demandorderposistion'}de{'Anforderungsauftragsposition'}", typeof(DemandOrderPos), DemandOrderPos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<DemandOrderPos>) })]
    public partial class DemandOrderPos
    {
        public const string ClassName = "DemandOrderPos";

        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static DemandOrderPos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            DemandOrderPos entity = new DemandOrderPos();
            entity.DemandOrderPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is DemandOrder)
            {
                DemandOrder demandOrder = parentACObject as DemandOrder;
                entity.TargetDeliveryDate = DateTime.Now;
                entity.TargetDeliveryMaxDate = DateTime.Now;
                if (demandOrder != null)
                {
                    try
                    {
                        if (!demandOrder.DemandOrderPos_DemandOrder.IsLoaded)
                            demandOrder.DemandOrderPos_DemandOrder.Load();
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        if (Database.Root != null && Database.Root.Messages != null)
                            Database.Root.Messages.LogException("DemandOrderPos", Const.MN_NewACObject, msg);
                    }

                    if (demandOrder.DemandOrderPos_DemandOrder != null && demandOrder.DemandOrderPos_DemandOrder.Select(c => c.Sequence).Any())
                        entity.Sequence = demandOrder.DemandOrderPos_DemandOrder.Select(c => c.Sequence).Max() + 1;
                    else
                        entity.Sequence = 1;
                }
                entity.DemandOrder = demandOrder;
            }
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
            DemandOrder demandOrder = DemandOrder;
            database.DeleteObject(this);
            DemandOrderPos.RenumberSequence(demandOrder, sequence);
            return null;
        }


        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(DemandOrder demandOrder, int sequence)
        {
            if (demandOrder == null
                || !demandOrder.DemandOrderPos_DemandOrder.Any())
                return;

            var elements = from c in demandOrder.DemandOrderPos_DemandOrder where c.Sequence > sequence orderby c.Sequence select c;
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
                if (Material == null)
                    return Sequence.ToString();
                return Sequence.ToString() + " " + Material.ACCaption;
            }
        }

        /// <summary>
        /// Returns DemandOrder
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to DemandOrder</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return DemandOrder;
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
                    ACIdentifier = Material.ClassName,
                    Message = "Material is null",
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

        #region VBIplus-Context
        private gip.core.datamodel.ACClassMethod _ProgramACClassMethod;
        [ACPropertyInfo(9999, "", "en{'Program Method'}de{'Programmmethode'}", Const.ContextDatabaseIPlus + "\\ProgramACClassMethodList")]
        public gip.core.datamodel.ACClassMethod ProgramACClassMethod
        {
            get
            {
                if (this.VBiProgramACClassMethodID == null || this.VBiProgramACClassMethodID == Guid.Empty)
                    return null;
                if (_ProgramACClassMethod != null)
                    return _ProgramACClassMethod;
                if (this.VBiProgramACClassMethod == null)
                {
                    DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                    _ProgramACClassMethod = dbApp.ContextIPlus.ACClassMethod.Where(c => c.ACClassMethodID == this.VBiProgramACClassMethodID).FirstOrDefault();
                    return _ProgramACClassMethod;
                }
                else
                {
                    _ProgramACClassMethod = this.VBiProgramACClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>();
                    return _ProgramACClassMethod;
                }
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiProgramACClassMethod == null)
                        return;
                    _ProgramACClassMethod = null;
                    this.VBiProgramACClassMethod = null;
                }
                else
                {
                    if (_ProgramACClassMethod != null && value == _ProgramACClassMethod)
                        return;
                    gip.mes.datamodel.ACClassMethod value2 = value.FromAppContext<gip.mes.datamodel.ACClassMethod>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiProgramACClassMethodID = value.ACClassMethodID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _ProgramACClassMethod = value;
                    if (value2 == this.VBiProgramACClassMethod)
                        return;
                    this.VBiProgramACClassMethod = value2;
                }
            }
        }

        partial void OnVBiProgramACClassMethodIDChanged()
        {
            OnPropertyChanged("ProgramACClassMethod");
        }
        #endregion

    }
}
