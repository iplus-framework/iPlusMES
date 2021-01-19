using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions; 
using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Programlog variobatch'}de{'Programlog variobatch'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, false, "", "MDBSOBalancingMode")]
    [ACPropertyEntity(1, "VBiACProgramLog", "en{'Program log'}de{'Program log'}", Const.ContextDatabase + "\\ACProgramLog", "", true)]
    [ACPropertyEntity(2, ProdOrderPartslistPos.ClassName, "en{'ProdOrderPartslistPos'}de{'ProdOrderPartslistPos'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName, "", true)]
    [ACPropertyEntity(3, ProdOrderPartslistPosRelation.ClassName, "en{'ProdOrderPartslistPosRelation'}de{'ProdOrderPartslistPosRelation'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPosRelation.ClassName, "", true)]
    [ACPropertyEntity(3, "DeliveryNotePos", "en{'DeliveryNotePos'}de{'DeliveryNotePos'}", Const.ContextDatabase + "\\DeliveryNotePos", "", true)]
    [ACPropertyEntity(4, "PickingPos", "en{'PickingPos'}de{'PickingPos'}", Const.ContextDatabase + "\\PickingPos", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + OrderLog.ClassName, "en{'Programlog'}de{'Programlog'}", typeof(OrderLog), OrderLog.ClassName, "VBiACProgramLogID", "VBiACProgramLogID")]
    public partial class OrderLog : IACObjectEntity
    {
        public const string ClassName = "OrderLog";

        #region New/Delete
        public static OrderLog NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            if (parentACObject == null)
                return null;
            gip.core.datamodel.ACProgramLog acProgramLog = parentACObject as gip.core.datamodel.ACProgramLog;
            if (acProgramLog == null)
                return null;
            OrderLog entity = new OrderLog();
            entity.VBiACProgramLogID = acProgramLog.ACProgramLogID;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
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
        [IgnoreDataMember]
        public override string ACCaption
        {
            get
            {
                if (ACProgramLog == null)
                    return null;
                return ACProgramLog.ACUrl;
            }
        }

        #endregion

        #region IACObjectEntity Members
        [IgnoreDataMember]
        static public string KeyACIdentifier
        {
            get
            {
                return "ACProgramLog\\ACUrl,ACProgramLog\\ACProgram\\ProgramNo";
            }
        }
        #endregion

        #region VBIplus-Context
        [IgnoreDataMember]
        private gip.core.datamodel.ACProgramLog _ACProgramLog;
        [ACPropertyInfo(9999, "", "en{'Module'}de{'Modul'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClass.ClassName)]
        [IgnoreDataMember]
        public gip.core.datamodel.ACProgramLog ACProgramLog
        {
            get
            {
                if (this.VBiACProgramLogID == null || this.VBiACProgramLogID == Guid.Empty)
                    return null;
                if (_ACProgramLog != null)
                    return _ACProgramLog;
                if (this.VBiACProgramLog == null)
                {
                    DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                    if (dbApp == null)
                        return null;
                    _ACProgramLog = dbApp.ContextIPlus.ACProgramLog.Where(c => c.ACProgramLogID == this.VBiACProgramLogID).FirstOrDefault();
                    return _ACProgramLog;
                }
                else
                {
                    _ACProgramLog = this.VBiACProgramLog.FromIPlusContext<gip.core.datamodel.ACProgramLog>();
                    return _ACProgramLog;
                }
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiACProgramLog == null)
                        return;
                    _ACProgramLog = null;
                    this.VBiACProgramLog = null;
                }
                else
                {
                    if (_ACProgramLog != null && value == _ACProgramLog)
                        return;
                    gip.mes.datamodel.ACProgramLog value2 = value.FromAppContext<gip.mes.datamodel.ACProgramLog>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiACProgramLogID = value.ACProgramLogID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _ACProgramLog = value;
                    if (value2 == this.VBiACProgramLog)
                        return;
                    this.VBiACProgramLog = value2;
                }
            }
        }
        #endregion
    }
}




