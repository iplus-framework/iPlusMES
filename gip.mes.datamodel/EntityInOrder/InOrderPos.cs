using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Purchase Order Pos.'}de{'Bestellposition'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Folge'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(4, MDUnit.ClassName, ConstApp.MDUnit, Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(5, "TargetQuantityUOM", ConstApp.TargetQuantityUOM, "", "", true)]
    [ACPropertyEntity(6, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(7, "ActualQuantityUOM", ConstApp.ActualQuantityUOM, "", "", true)]
    [ACPropertyEntity(8, "ActualQuantity", ConstApp.ActualQuantity, "", "", true)]
    [ACPropertyEntity(9, "CalledUpQuantityUOM", "en{'Called up Quantity (UOM)'}de{'Abgerufene Menge (BME)'}", "", "", true)]
    [ACPropertyEntity(10, "CalledUpQuantity", "en{'Called up Quantity'}de{'Abgerufene Menge'}", "", "", true)]
    [ACPropertyEntity(11, "TargetDeliveryDate", ConstApp.TargetDeliveryDate, "", "", true)]
    [ACPropertyEntity(12, "TargetDeliveryMaxDate", ConstApp.TargetDeliveryMaxDate, "", "", true)]
    [ACPropertyEntity(13, "TargetDeliveryPriority", "en{'Delivery Priority'}de{'Lieferpriorität'}", "", "", true)]
    [ACPropertyEntity(14, "TargetDeliveryDateConfirmed", "en{'Confirmed Delivery Date'}de{'Bestätigtes Lieferdatum'}", "", "", true)]
    [ACPropertyEntity(15, MDTimeRange.ClassName, "en{'Shift'}de{'Schicht'}", Const.ContextDatabase + "\\" + MDTimeRange.ClassName, "", true)]
    [ACPropertyEntity(16, MDDelivPosState.ClassName, ConstApp.ESDelivPosState, Const.ContextDatabase + "\\" + MDDelivPosState.ClassName, "", true)]
    [ACPropertyEntity(17, MDInOrderPosState.ClassName, ConstApp.ESInOrderPosState, Const.ContextDatabase + "\\" + MDInOrderPosState.ClassName, "", true)]
    [ACPropertyEntity(18, MDDelivPosLoadState.ClassName, ConstApp.ESDelivPosLoadState, Const.ContextDatabase + "\\" + MDDelivPosLoadState.ClassName, "", true)]
    [ACPropertyEntity(19, "PriceNet", ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(20, "PriceGross", ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(21, MDCountrySalesTax.ClassName, "en{'Sales Tax'}de{'Steuersatz'}", Const.ContextDatabase + "\\" + MDCountrySalesTax.ClassName, "", true)]
    [ACPropertyEntity(22, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(23, "Comment2", "en{'Comment 2'}de{'Bemerkung 2'}", "", "", true)]
    [ACPropertyEntity(24, "LineNumber", "en{'Item Number'}de{'Positionsnummer'}", "", "", true)]
    [ACPropertyEntity(32, MDTransportMode.ClassName, ConstApp.ESTransportMode, Const.ContextDatabase + "\\" + MDTransportMode.ClassName, "", true)]
    [ACPropertyEntity(33, "InOrderPos1_ParentInOrderPos", "en{'Parent'}de{'Parent'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName, "", true)]
    [ACPropertyEntity(9999, InOrder.ClassName, "en{'Purchase Order'}de{'Bestellung'}", Const.ContextDatabase + "\\" + InOrder.ClassName, "", true)]
    [ACPropertyEntity(9999, "MaterialPosTypeIndex", "en{'Position Type'}de{'Posistionstyp'}", typeof(GlobalApp.MaterialPosTypes), "", "", true)]
    [ACPropertyEntity(9999, "PickupCompanyMaterial", "en{'Material for Pick-Up'}de{'Material für Abholung'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName, "", true)]
    [ACPropertyEntity(9999, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioPurchase, Const.QueryPrefix + InOrderPos.ClassName, "en{'Purchase Order Pos.'}de{'Bestellposition'}", typeof(InOrderPos), InOrderPos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<InOrderPos>) })]
    public partial class InOrderPos
    {
        public const string ClassName = "InOrderPos";

        #region New/Delete
        public static InOrderPos NewACObject(DatabaseApp dbApp, IACObject parentACObject, InOrder attachToOrder = null)
        {
            InOrderPos entity = new InOrderPos();
            entity.InOrderPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.TargetDeliveryDate = DateTime.Now;
            InOrder inOrder = null;
            if (parentACObject is InOrderPos)
            {
                InOrderPos parentInOrderPos = parentACObject as InOrderPos;
                entity.InOrderPos1_ParentInOrderPos = parentInOrderPos;
                entity.CopyFromParent(parentInOrderPos);
                inOrder = attachToOrder != null ? attachToOrder : parentInOrderPos.InOrder;
                entity.MaterialPosType = GlobalApp.MaterialPosTypes.InwardPart;
            }
            if (parentACObject is InOrder)
            {
                inOrder = parentACObject as InOrder;
                entity.TargetDeliveryDate = inOrder.TargetDeliveryDate;
                entity.TargetDeliveryMaxDate = inOrder.TargetDeliveryMaxDate;
            }
            if (inOrder != null)
            {
                if (inOrder.EntityState != System.Data.EntityState.Added
                    && inOrder.InOrderPos_InOrder != null
                    && inOrder.InOrderPos_InOrder.Any())
                    entity.Sequence = inOrder.InOrderPos_InOrder.Select(c => c.Sequence).Max() + 1;
                else
                    entity.Sequence = 1;
                entity.InOrder = inOrder;
                entity.MaterialPosType = GlobalApp.MaterialPosTypes.InwardRoot;
                inOrder.InOrderPos_InOrder.Add(entity);
            }
            entity.TargetQuantityUOM = 1;
            entity.MDInOrderPosState = MDInOrderPosState.DefaultMDInOrderPosState(dbApp);

            entity.MDDelivPosState = MDDelivPosState.DefaultMDDelivPosState(dbApp);
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
            foreach (CompanyMaterialPickup pickup in CompanyMaterialPickup_InOrderPos.ToList())
            {
                MsgWithDetails msg = pickup.DeleteACObject(database, withCheck);
                if (msg != null)
                    return msg;
            }
            if (withCheck)
            {
                MsgWithDetails msg = IsEnabledDeleteACObject(database);
                if (msg != null)
                    return msg;
            }
            int sequence = Sequence;

            foreach (FacilityPreBooking facilityPreBooking in FacilityPreBooking_InOrderPos.ToList())
            {
                facilityPreBooking.DeleteACObject(database, false);
            }

            InOrder inOrder = InOrder;
            if (inOrder.InOrderPos_InOrder.IsLoaded)
                inOrder.InOrderPos_InOrder.Remove(this);
            database.DeleteObject(this);
            InOrderPos.RenumberSequence(inOrder, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(InOrder inOrder, int sequence)
        {
            var elements = from c in inOrder.InOrderPos_InOrder where c.Sequence > sequence && c.EntityState != System.Data.EntityState.Deleted orderby c.Sequence select c;
            int sequenceCount = sequence;
            foreach (var element in elements)
            {
                element.Sequence = sequence;
                sequence++;
            }
        }

        public void CopyFromParent(InOrderPos parentInOrderPos)
        {
            this.MDUnit = parentInOrderPos.MDUnit;
            this.Material = parentInOrderPos.Material;
            this.TargetDeliveryDate = parentInOrderPos.TargetDeliveryDate;
            this.TargetDeliveryMaxDate = parentInOrderPos.TargetDeliveryMaxDate;
            this.TargetDeliveryPriority = parentInOrderPos.TargetDeliveryPriority;
            this.TargetDeliveryDateConfirmed = parentInOrderPos.TargetDeliveryDateConfirmed;
            this.MDTimeRange = parentInOrderPos.MDTimeRange;
            this.PriceNet = parentInOrderPos.PriceNet;
            this.PriceGross = parentInOrderPos.PriceGross;
            this.MDCountrySalesTax = parentInOrderPos.MDCountrySalesTax;
            this.Comment = parentInOrderPos.Comment;
            this.Comment2 = parentInOrderPos.Comment2;
            this.MDTransportMode = parentInOrderPos.MDTransportMode;
        }
        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            return InOrder?.InOrderNo + "/#" + Sequence.ToString() + "/" + Material?.ToString();
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
        /// Returns InOrder
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to InOrder</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return InOrder;
            }
        }

        #endregion

        #region IACObjectEntity Members
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
                return Sequence.ToString("00");
                //return "Position " + Sequence.ToString();
            }
        }

        private string _CachedDeliveryNoteNo;
        [ACPropertyInfo(999, "LocalDeliveryNoteNo", "en{'Delivery Note No.'}de{'Lieferschein-Nr.'}")]
        public string CachedDeliveryNoteNo
        {
            get
            {
                if (_CachedDeliveryNoteNo == null)
                {
                    _CachedDeliveryNoteNo = DeliveryNotePos_InOrderPos.Select(c => c.DeliveryNote.DeliveryNoteNo).FirstOrDefault();
                    if (string.IsNullOrEmpty(_CachedDeliveryNoteNo))
                        _CachedDeliveryNoteNo = "-";
                }
                return _CachedDeliveryNoteNo;
            }
        }

        public GlobalApp.MaterialPosTypes MaterialPosType
        {
            get
            {
                return (GlobalApp.MaterialPosTypes)MaterialPosTypeIndex;
            }
            set
            {
                MaterialPosTypeIndex = (Int16)value;
            }
        }


        #region Partial Methods
        bool _OnCalledUpQuantityChanging = false;
        partial void OnCalledUpQuantityChanged()
        {
            if (!_OnCalledUpQuantityUOMChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnCalledUpQuantityChanging = true;
                try
                {
                    CalledUpQuantityUOM = Material.ConvertToBaseQuantity(CalledUpQuantity, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException("InOrderPos", "OnCalledUpQuantityChanged", msg);
                }
                finally
                {
                    _OnCalledUpQuantityChanging = false;
                }
            }
            OnPropertyChanged("RemainingCallQuantity");
            OnPropertyChanged("DifferenceQuantity");
        }

        bool _OnCalledUpQuantityUOMChanging = false;
        partial void OnCalledUpQuantityUOMChanged()
        {
            if (!_OnCalledUpQuantityChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnCalledUpQuantityUOMChanging = true;
                try
                {
                    CalledUpQuantity = Material.ConvertQuantity(CalledUpQuantityUOM, Material.BaseMDUnit, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException("InOrderPos", "OnCalledUpQuantityUOMChanged", msg);
                }
                finally
                {
                    _OnCalledUpQuantityUOMChanging = false;
                }
            }
            OnPropertyChanged("RemainingCallQuantityUOM");
            OnPropertyChanged("DifferenceQuantityUOM");
        }

        bool _OnTargetQuantityChanging = false;
        partial void OnTargetQuantityChanged()
        {
            if (!_OnTargetQuantityUOMChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnTargetQuantityChanging = true;
                try
                {
                    TargetQuantityUOM = Material.ConvertToBaseQuantity(TargetQuantity, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException("InOrderPos", "OnTargetQuantityChanged", msg);
                }
                finally
                {
                    _OnTargetQuantityChanging = false;
                }
            }
            OnPropertyChanged("RemainingCallQuantity");
            OnPropertyChanged("DifferenceQuantity");
        }

        bool _OnTargetQuantityUOMChanging = false;
        partial void OnTargetQuantityUOMChanged()
        {
            if (!_OnTargetQuantityChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnTargetQuantityUOMChanging = true;
                try
                {
                    TargetQuantity = Material.ConvertQuantity(TargetQuantityUOM, Material.BaseMDUnit, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException("InOrderPos", "OnTargetQuantityUOMChanged", msg);
                }
                finally
                {
                    _OnTargetQuantityUOMChanging = false;
                }
            }
            OnPropertyChanged("RemainingCallQuantityUOM");
            OnPropertyChanged("DifferenceQuantityUOM");
        }

        bool _OnActualQuantityChanging = false;
        partial void OnActualQuantityChanged()
        {
            if (!_OnActualQuantityUOMChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnActualQuantityChanging = true;
                try
                {
                    ActualQuantityUOM = Material.ConvertToBaseQuantity(ActualQuantity, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException("InOrderPos", "OnActualQuantityChanged", msg);
                }
                finally
                {
                    _OnActualQuantityChanging = false;
                }
            }
        }

        bool _OnActualQuantityUOMChanging = false;
        partial void OnActualQuantityUOMChanged()
        {
            if (!_OnActualQuantityChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnActualQuantityUOMChanging = true;
                try
                {
                    ActualQuantity = Material.ConvertQuantity(ActualQuantityUOM, Material.BaseMDUnit, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException("InOrderPos", "OnActualQuantityUOMChanged", msg);
                }
                finally
                {
                    _OnActualQuantityUOMChanging = false;
                }
            }
        }

        [ACPropertyInfo(24, "", "en{'Remaining Quantity'}de{'Restmenge'}")]
        public double RemainingCallQuantity
        {
            get
            {
                return TargetQuantity - CalledUpQuantity;
            }
        }

        [ACPropertyInfo(25, "", "en{'Remaining Quantity (UOM)'}de{'Restmenge (BME)'}")]
        public double RemainingCallQuantityUOM
        {
            get
            {
                return TargetQuantityUOM - CalledUpQuantityUOM;
            }
        }

        [ACPropertyInfo(26, "", "en{'Difference Quantity'}de{'Differenzmenge'}")]
        public double DifferenceQuantity
        {
            get
            {
                return ActualQuantity - TargetQuantity;
            }
        }

        [ACPropertyInfo(27, "", "en{'Difference Quantity (UOM)'}de{'Differenzmenge (BME)'}")]
        public double DifferenceQuantityUOM
        {
            get
            {
                return ActualQuantityUOM - TargetQuantityUOM;
            }
        }

        public InOrderPos TopParentInOrderPos
        {
            get
            {
                if (this.InOrderPos1_ParentInOrderPos != null)
                    return this.InOrderPos1_ParentInOrderPos.TopParentInOrderPos;
                return this;
            }
        }


        public void RecalcActualQuantity(Nullable<MergeOption> mergeOption = null)
        {
            if (this.EntityState != System.Data.EntityState.Added)
            {
                if (mergeOption.HasValue)
                    this.InOrderPos_ParentInOrderPos.Load(mergeOption.Value);
                else
                    this.InOrderPos_ParentInOrderPos.AutoLoad();
            }

            double sumActualQuantity = 0;
            double sumActualQuantityUOM = 0;
            foreach (InOrderPos childPos in this.InOrderPos_ParentInOrderPos)
            {
                childPos.RecalcActualQuantity(mergeOption);
                sumActualQuantityUOM += childPos.ActualQuantityUOM;
                if (childPos.MDUnit == this.MDUnit)
                    sumActualQuantity += childPos.ActualQuantity;
                else if (this.Material != null)
                {
                    try
                    {
                        sumActualQuantity += Material.ConvertQuantity(childPos.ActualQuantity, childPos.MDUnit, this.MDUnit);
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        this.Root().Messages.LogException("InOrderPos", "RecalcActualQuantity", msg);
                    }
                }
            }

            if (this.EntityState != System.Data.EntityState.Added)
            {
                if (mergeOption.HasValue)
                    this.FacilityBooking_InOrderPos.Load(mergeOption.Value);
                else
                    this.FacilityBooking_InOrderPos.AutoLoad();
            }
            foreach (FacilityBooking fb in FacilityBooking_InOrderPos)
            {
                foreach (FacilityBookingCharge fbc in fb.FacilityBookingCharge_FacilityBooking)
                {
                    sumActualQuantityUOM += fbc.InwardQuantityUOM;
                    try
                    {
                        sumActualQuantity += fbc.InwardMaterial.ConvertQuantity(fbc.InwardQuantity, fbc.MDUnit, this.MDUnit);
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        this.Root().Messages.LogException("InOrderPos", "RecalcActualQuantity(10)", msg);
                    }
                }
            }
            this.ActualQuantity = sumActualQuantity;
            this.ActualQuantityUOM = sumActualQuantityUOM;
        }

        public void RecalcDeliveryStates(Nullable<MergeOption> mergeOption = null)
        {
            if (this.EntityState != System.Data.EntityState.Added)
            {
                if (mergeOption.HasValue)
                    this.InOrderPos_ParentInOrderPos.Load(mergeOption.Value);
                else
                    this.InOrderPos_ParentInOrderPos.AutoLoad();
            }

            bool childsDelivererd = true;
            foreach (InOrderPos childPos in this.InOrderPos_ParentInOrderPos)
            {
                childPos.RecalcDeliveryStates(mergeOption);
                if (childPos.MDDelivPosState.DelivPosState != datamodel.MDDelivPosState.DelivPosStates.Delivered)
                {
                    childsDelivererd = false;
                    break;
                }
            }
            if (this.MDDelivPosState.DelivPosState == datamodel.MDDelivPosState.DelivPosStates.CompletelyAssigned
                && this.ActualQuantityUOM > 0.00001
                && childsDelivererd)
            {
                MDDelivPosState deliveredState = DatabaseApp.s_cQry_GetMDDelivPosState(this.GetObjectContext<DatabaseApp>(), datamodel.MDDelivPosState.DelivPosStates.Delivered).FirstOrDefault();
                if (deliveredState != null)
                    this.MDDelivPosState = deliveredState;
            }
        }


        //public double PreBookingQuantity
        //{
        //    get;
        //}

        //public double PreBookingQuantityUOM
        //{
        //    get;
        //}

        public double PreBookingInwardQuantityUOM(Nullable<MergeOption> mergeOption = null)
        {
            if (this.EntityState != System.Data.EntityState.Added)
            {
                if (mergeOption.HasValue)
                    this.InOrderPos_ParentInOrderPos.Load(mergeOption.Value);
                else
                    this.InOrderPos_ParentInOrderPos.AutoLoad();
            }

            double sumUOM = 0;
            foreach (InOrderPos childPos in this.InOrderPos_ParentInOrderPos)
            {
                sumUOM += childPos.PreBookingInwardQuantityUOM(mergeOption);
            }

            if (this.EntityState != System.Data.EntityState.Added)
            {
                if (mergeOption.HasValue)
                    this.FacilityPreBooking_InOrderPos.Load(mergeOption.Value);
                else
                    this.FacilityPreBooking_InOrderPos.AutoLoad();
            }
            foreach (FacilityPreBooking fb in FacilityPreBooking_InOrderPos)
            {
                if (fb.InwardQuantity.HasValue)
                {
                    try
                    {
                        sumUOM += Material.ConvertToBaseQuantity(fb.InwardQuantity.Value, this.MDUnit);
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        this.Root().Messages.LogException("InOrderPos", "PreBookingInwardQuantityUOM", msg);
                    }
                }
            }
            return sumUOM;
        }

        #endregion
    }
}
