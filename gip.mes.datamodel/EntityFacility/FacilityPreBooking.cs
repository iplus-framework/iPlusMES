using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using gip.core.datamodel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    // FacilityPreBooking (Lagerbewegung)
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Planned posting'}de{'Geplante Buchung'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "FacilityPreBookingNo", "en{'Posting-No.'}de{'Buchungsnr.'}", "", "", true)]
    [ACPropertyEntity(2, InOrderPos.ClassName, "en{'Order line receival'}de{'Bestellposition Eingang'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName, "", true)]
    [ACPropertyEntity(3, OutOrderPos.ClassName, "en{'Order line issue'}de{'Bestellposition Ausgang'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName, "", true)]
    [ACPropertyEntity(4, ProdOrderPartslistPos.ClassName, "en{'Production line'}de{'Auftragsposition'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName, "", true)]
    [ACPropertyEntity(5, ProdOrderPartslistPosRelation.ClassName, "en{'Bill of Materials relation'}de{'Entnahmeposition'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPosRelation.ClassName, "", true)]
    [ACPropertyEntity(6, PickingPos.ClassName, "en{'Picking line'}de{'Kommissionierposition'}", Const.ContextDatabase + "\\" + PickingPos.ClassName, "", true)]
    [ACPropertyEntity(9999, "ACMethodBookingXML", "en{'ACMethodBooking'}de{'ACMethodBooking'}")]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityPreBooking.ClassName, "en{'Planned posting'}de{'Geplante Buchung'}", typeof(FacilityPreBooking), FacilityPreBooking.ClassName, "FacilityPreBookingNo", "FacilityPreBookingNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<FacilityPreBooking>) })]
    public partial class FacilityPreBooking : IACObjectEntity
    {
        [NotMapped]
        public const string ClassName = "FacilityPreBooking";
        [NotMapped]
        public const string NoColumnName = "FacilityPreBookingNo";
        [NotMapped]
        public const string FormatNewNo = "FPB{0}";

        #region New/Delete
        public static FacilityPreBooking NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {

            FacilityPreBooking entity = new FacilityPreBooking();
            entity.FacilityPreBookingID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.FacilityPreBookingNo = secondaryKey;
            if (parentACObject is InOrderPos)
            {
                entity.InOrderPos = parentACObject as InOrderPos;
                entity.InOrderPos.FacilityPreBooking_InOrderPos.Add(entity);
            }
            else if (parentACObject is OutOrderPos)
            {
                entity.OutOrderPos = parentACObject as OutOrderPos;
                entity.OutOrderPos.FacilityPreBooking_OutOrderPos.Add(entity);
            }
            else if (parentACObject is ProdOrderPartslistPos)
            {
                entity.ProdOrderPartslistPos = parentACObject as ProdOrderPartslistPos;
                entity.ProdOrderPartslistPos.FacilityPreBooking_ProdOrderPartslistPos.Add(entity);
            }
            else if (parentACObject is ProdOrderPartslistPosRelation)
            {
                entity.ProdOrderPartslistPosRelation = parentACObject as ProdOrderPartslistPosRelation;
                entity.ProdOrderPartslistPosRelation.FacilityPreBooking_ProdOrderPartslistPosRelation.Add(entity);
            }
            else if (parentACObject is PickingPos)
            {
                entity.PickingPos = parentACObject as PickingPos;
                entity.PickingPos.FacilityPreBooking_PickingPos.Add(entity);
            }
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
            if (InOrderPos != null)
            {
                InOrderPos.FacilityPreBooking_InOrderPos.Remove(this);
            }
            else if (OutOrderPos != null)
            {
                OutOrderPos.FacilityPreBooking_OutOrderPos.Remove(this);
            }
            if (_ACMethodBooking != null)
                _ACMethodBooking.PropertyChanged -= _ACMethodBooking_PropertyChanged;
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
                return FacilityPreBookingNo;
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
            if (string.IsNullOrEmpty(FacilityPreBookingNo))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "FacilityPreBookingNo",
                    Message = "FacilityPreBookingNo is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "FacilityPreBookingNo"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

        // Buchungssätze werden nur einmalig eingefügt und danach nie
        // mehr geändert
        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "FacilityPreBookingNo";
            }
        }
        #endregion
        #region IEntityProperty Members

        //bool bRefreshConfig = false;
        //partial void OnXMLConfigChanging(global::System.String value)
        //{
        //    bRefreshConfig = false;
        //    if (this.EntityState != System.Data.EntityState.Detached && (!(String.IsNullOrEmpty(value) && String.IsNullOrEmpty(XMLConfig)) && value != XMLConfig))
        //        bRefreshConfig = true;
        //}

        //partial void OnXMLConfigChanged()
        //{
        //    if (bRefreshConfig)
        //        ACProperties.Refresh();
        //}

        [NotMapped]
        private gip.core.datamodel.ACClass _TypeOfACMethodBooking = null;
        [NotMapped]
        private gip.core.datamodel.ACClass TypeOfACMethodBooking
        {
            get
            {
                IFacilityManager manager = HelperIFacilityManager.GetServiceInstance();
                if (manager != null)
                    return manager.TypeOfACMethodBooking;
                if (_TypeOfACMethodBooking != null)
                    return _TypeOfACMethodBooking;
                using (ACMonitor.Lock(Database.GlobalDatabase.QueryLock_1X000))
                {
                    _TypeOfACMethodBooking = Database.GlobalDatabase.ACClass.Where(c => c.ACIdentifier == ConstApp.ACMethodBooking_ClassName).FirstOrDefault();
                }
                return _TypeOfACMethodBooking;
            }
        }

        [NotMapped]
        private ACMethod _ACMethodBooking = null;
        [ACPropertyInfo(9999)]
        [NotMapped]
        public ACMethod ACMethodBooking
        {
            get
            {
                if (_ACMethodBooking != null)
                    return _ACMethodBooking;
                if (String.IsNullOrEmpty(this.ACMethodBookingXML))
                    return null;
                if (TypeOfACMethodBooking == null)
                    return null;
                IACEntityObjectContext acEntityObjectContext = this.GetObjectContext();
                _ACMethodBooking = ACConvert.XMLToObject(TypeOfACMethodBooking.ObjectFullType, this.ACMethodBookingXML, true, acEntityObjectContext) as ACMethod;
                if (_ACMethodBooking != null)
                {
                    _ACMethodBooking.AttachTo(acEntityObjectContext);
                    _ACMethodBooking.PropertyChanged += new PropertyChangedEventHandler(_ACMethodBooking_PropertyChanged);
                }
                return _ACMethodBooking;
            }
            set
            {
                if (_ACMethodBooking != null)
                    _ACMethodBooking.PropertyChanged -= _ACMethodBooking_PropertyChanged;
                _ACMethodBooking = value;
                _ACMethodBooking.PropertyChanged += new PropertyChangedEventHandler(_ACMethodBooking_PropertyChanged);
                SerializeACMethodBooking();
                OnPropertyChanged("ACMethodBooking");
            }
        }

        void _ACMethodBooking_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SerializeACMethodBooking();
        }

        private void SerializeACMethodBooking()
        {
            try
            {
                string valueXML = ACConvert.ObjectToXML(_ACMethodBooking, true);
                if (ACMethodBookingXML != valueXML)
                    ACMethodBookingXML = valueXML;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                this.Root().Messages.LogException("FacilityPreBooking", "SerializeACMethodBooking", msg);
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public Nullable<Double> InwardQuantity
        {
            get
            {
                if (ACMethodBooking == null)
                    return null;
                ACValue acValue = ACMethodBooking.ParameterValueList.GetACValue("InwardQuantity");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Double>;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public Nullable<Double> OutwardQuantity
        {
            get
            {
                if (ACMethodBooking == null)
                    return null;
                ACValue acValue = ACMethodBooking.ParameterValueList.GetACValue("OutwardQuantity");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Double>;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public MDUnit MDUnit
        {
            get
            {
                if (ACMethodBooking == null)
                    return null;
                ACValue acValue = ACMethodBooking.ParameterValueList.GetACValue(MDUnit.ClassName);
                if (acValue == null)
                    return null;
                return acValue.Value as MDUnit;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public Facility InwardFacility
        {
            get
            {
                if (ACMethodBooking == null)
                    return null;
                ACValue acValue = ACMethodBooking.ParameterValueList.GetACValue("InwardFacility");
                if (acValue == null)
                    return null;
                return acValue.Value as Facility;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public Facility OutwardFacility
        {
            get
            {
                if (ACMethodBooking == null)
                    return null;
                ACValue acValue = ACMethodBooking.ParameterValueList.GetACValue("OutwardFacility");
                if (acValue == null)
                    return null;
                return acValue.Value as Facility;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public FacilityLot InwardFacilityLot
        {
            get
            {
                if (ACMethodBooking == null)
                    return null;
                ACValue acValue = ACMethodBooking.ParameterValueList.GetACValue("InwardFacilityLot");
                if (acValue == null)
                    return null;
                return acValue.Value as FacilityLot;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public FacilityLot OutwardFacilityLot
        {
            get
            {
                if (ACMethodBooking == null)
                    return null;
                ACValue acValue = ACMethodBooking.ParameterValueList.GetACValue("OutwardFacilityLot");
                if (acValue == null)
                    return null;
                return acValue.Value as FacilityLot;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public FacilityCharge InwardFacilityCharge
        {
            get
            {
                if (ACMethodBooking == null)
                    return null;
                ACValue acValue = ACMethodBooking.ParameterValueList.GetACValue("InwardFacilityCharge");
                if (acValue == null)
                    return null;
                return acValue.Value as FacilityCharge;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public FacilityCharge OutwardFacilityCharge
        {
            get
            {
                if (ACMethodBooking == null)
                    return null;
                ACValue acValue = ACMethodBooking.ParameterValueList.GetACValue("OutwardFacilityCharge");
                if (acValue == null)
                    return null;
                return acValue.Value as FacilityCharge;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public Material InwardMaterial
        {
            get
            {
                if (ACMethodBooking == null)
                    return null;
                ACValue acValue = ACMethodBooking.ParameterValueList.GetACValue("InwardMaterial");
                if (acValue == null)
                    return null;
                return acValue.Value as Material;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public Material OutwardMaterial
        {
            get
            {
                if (ACMethodBooking == null)
                    return null;
                ACValue acValue = ACMethodBooking.ParameterValueList.GetACValue("OutwardMaterial");
                if (acValue == null)
                    return null;
                return acValue.Value as Material;
            }
        }

        #endregion
    }
}
