using gip.core.datamodel;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Purchase Order Pos.'}de{'Bestellposition'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Folge'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
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
    [ACPropertyEntity(15, MDTimeRange.ClassName, "en{'Shift'}de{'Schicht'}", Const.ContextDatabase + "\\" + MDTimeRange.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(16, MDDelivPosState.ClassName, ConstApp.ESDelivPosState, Const.ContextDatabase + "\\" + MDDelivPosState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(17, MDInOrderPosState.ClassName, ConstApp.ESInOrderPosState, Const.ContextDatabase + "\\" + MDInOrderPosState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(18, MDDelivPosLoadState.ClassName, ConstApp.ESDelivPosLoadState, Const.ContextDatabase + "\\" + MDDelivPosLoadState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(19, "PriceNet", ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(20, "PriceGross", ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(21, MDCountrySalesTax.ClassName, "en{'Sales Tax'}de{'Steuersatz'}", Const.ContextDatabase + "\\" + MDCountrySalesTax.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(22, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(23, "Comment2", "en{'Comment 2'}de{'Bemerkung 2'}", "", "", true)]
    [ACPropertyEntity(24, "LineNumber", "en{'Item Number'}de{'Positionsnummer'}", "", "", true)]
    [ACPropertyEntity(32, MDTransportMode.ClassName, ConstApp.ESTransportMode, Const.ContextDatabase + "\\" + MDTransportMode.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(33, "InOrderPos1_ParentInOrderPos", "en{'Parent'}de{'Parent'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9999, InOrder.ClassName, "en{'Purchase Order'}de{'Bestellung'}", Const.ContextDatabase + "\\" + InOrder.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9999, "MaterialPosTypeIndex", "en{'Position Type'}de{'Posistionstyp'}", typeof(GlobalApp.MaterialPosTypes), "", "", true)]
    [ACPropertyEntity(9999, "PickupCompanyMaterial", "en{'Material for Pick-Up'}de{'Material für Abholung'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(34, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioPurchase, Const.QueryPrefix + InOrderPos.ClassName, "en{'Purchase Order Pos.'}de{'Bestellposition'}", typeof(InOrderPos), InOrderPos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<InOrderPos>) })]
    [NotMapped]
    public partial class InOrderPos
    {
        [NotMapped]
        public const string ClassName = "InOrderPos";

        #region Private members
        [NotMapped]
        private string PropertyChangedName;
        #endregion

        #region New/Delete
        public static InOrderPos NewACObject(DatabaseApp dbApp, IACObject parentACObject, InOrder attachToOrder = null)
        {
            InOrderPos entity = new InOrderPos();
            entity.InOrderPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.TargetDeliveryDate = DateTime.Now;
            InOrder inOrder = null;
            InOrderPos parentInOrderPos = parentACObject as InOrderPos;
            if (parentInOrderPos != null)
            {
                entity.InOrderPos1_ParentInOrderPos = parentInOrderPos;
                entity.CopyFromParent(parentInOrderPos);
                inOrder = attachToOrder != null ? attachToOrder : parentInOrderPos.InOrder;
                entity.MaterialPosType = GlobalApp.MaterialPosTypes.InwardPart;
            }
            else
                inOrder = parentACObject as InOrder;

            if (inOrder != null)
            {
                entity.TargetDeliveryDate = inOrder.TargetDeliveryDate;
                entity.TargetDeliveryMaxDate = inOrder.TargetDeliveryMaxDate;

                if (!inOrder.InOrderPos_InOrder_IsLoaded
                    && (inOrder.EntityState == EntityState.Modified || inOrder.EntityState == EntityState.Unchanged))
                    entity.Sequence = inOrder.Context.Entry(inOrder).Collection(c => c.InOrderPos_InOrder).Query().Max(c => c.Sequence) + 1;
                else if (inOrder.InOrderPos_InOrder.Any())
                {
                    IEnumerable<int> querySequence = inOrder.InOrderPos_InOrder.Select(c => c.Sequence);
                    entity.Sequence = querySequence.Any() ? querySequence.Max() + 1 : 1;
                }
                else
                    entity.Sequence = 1;

                entity.InOrder = inOrder;
                entity.MaterialPosType = GlobalApp.MaterialPosTypes.InwardRoot;
                if (parentInOrderPos == null || inOrder.InOrderPos_InOrder_IsLoaded)
                    inOrder.InOrderPos_InOrder.Add(entity);
                else
                    dbApp.InOrderPos.Add(entity);
            }
            entity.TargetQuantityUOM = 1;
            entity.MDInOrderPosState = MDInOrderPosState.DefaultMDInOrderPosState(dbApp);

            entity.MDDelivPosState = MDDelivPosState.DefaultMDDelivPosState(dbApp);
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
            if (inOrder != null)
            {
                if (inOrder.InOrderPos_InOrder_IsLoaded)
                    inOrder.InOrderPos_InOrder.Remove(this);
            }
            base.DeleteACObject(database, withCheck, softDelete);
            if (inOrder != null)
                InOrderPos.RenumberSequence(inOrder, sequence);
            return null;
        }

        public override void RevertDeleteACObject(IACEntityObjectContext database)
        {
            base.RevertDeleteACObject(database);
            if (InOrder != null && InOrder.InOrderPos_InOrder_IsLoaded)
                InOrder.InOrderPos_InOrder.Add(this);
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(InOrder inOrder, int sequence)
        {
            if (inOrder == null
                || !inOrder.InOrderPos_InOrder.Any())
                return;

            var elements = inOrder.InOrderPos_InOrder.Where(c => c.Sequence > sequence && c.EntityState != EntityState.Deleted).OrderBy(c => c.Sequence);
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
        [NotMapped]
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
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return InOrder;
            }
        }

        #endregion

        #region IACObjectEntity Members
        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "Sequence";
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

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(CalledUpQuantity):
                    OnCalledUpQuantityChanged();
                    break;
                case nameof(CalledUpQuantityUOM):
                    OnCalledUpQuantityUOMChanged();
                    break;
                case nameof(TargetQuantity):
                    OnTargetQuantityChanged();
                    break;
                case nameof(TargetQuantityUOM):
                    OnTargetQuantityUOMChanged();
                    break;
                case nameof(MDUnitID):
                    OnMDUnitIDChanged();
                    break;
                case nameof(ActualQuantity):
                    OnActualQuantityChanged();
                    break;
                case nameof(ActualQuantityUOM):
                    OnActualQuantityUOMChanged();
                    break;
            }
            base.OnPropertyChanged(propertyName);
        }

        #endregion

        [ACPropertyInfo(9999)]
        [NotMapped]
        public string Position
        {
            get
            {
                return Sequence.ToString("00");
                //return "Position " + Sequence.ToString();
            }
        }

        [NotMapped]
        private string _CachedDeliveryNoteNo;
        [ACPropertyInfo(999, "LocalDeliveryNoteNo", "en{'Delivery Note No.'}de{'Lieferschein-Nr.'}")]
        [NotMapped]
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

        [NotMapped]
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
        [NotMapped]
        bool _OnCalledUpQuantityChanging = false;
        protected void OnCalledUpQuantityChanged()
        {
            if (!_OnCalledUpQuantityUOMChanging && EntityState != EntityState.Detached && Material != null && MDUnit != null)
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
            base.OnPropertyChanged("RemainingCallQuantity");
            base.OnPropertyChanged("DifferenceQuantity");
        }

        [NotMapped]
        bool _OnCalledUpQuantityUOMChanging = false;
        protected void OnCalledUpQuantityUOMChanged()
        {
            if (!_OnCalledUpQuantityChanging && EntityState != EntityState.Detached && Material != null && MDUnit != null)
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
            base.OnPropertyChanged("RemainingCallQuantityUOM");
            base.OnPropertyChanged("DifferenceQuantityUOM");
        }

        protected void OnTargetQuantityChanged()
        {
            if (EntityState != EntityState.Detached 
                && Material != null 
                && MDUnit != null
                && string.IsNullOrEmpty(PropertyChangedName))
            {
                PropertyChangedName = nameof(TargetQuantity);
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
                    PropertyChangedName = null;
                }
            }
            base.OnPropertyChanged("RemainingCallQuantity");
            base.OnPropertyChanged("DifferenceQuantity");
        }

        protected void OnTargetQuantityUOMChanged()
        {
            if (EntityState != EntityState.Detached 
                && Material != null 
                && MDUnit != null)
            {
                PropertyChangedName = nameof(TargetQuantity);
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
                    PropertyChangedName = null;
                }
            }
            base.OnPropertyChanged("RemainingCallQuantityUOM");
            base.OnPropertyChanged("DifferenceQuantityUOM");
        }

        protected void OnMDUnitIDChanged()
        {
            if (EntityState != EntityState.Detached
                && Material != null
                && MDUnit != null)
            {
                PropertyChangedName = nameof(MDUnit);
                _OnActualQuantityChanging = true;
                _OnCalledUpQuantityChanging = true;
                try
                {
                    TargetQuantity = Material.ConvertQuantity(TargetQuantityUOM, Material.BaseMDUnit, MDUnit);
                    ActualQuantity = Material.ConvertQuantity(ActualQuantityUOM, Material.BaseMDUnit, MDUnit);
                    CalledUpQuantity = Material.ConvertQuantity(CalledUpQuantityUOM, Material.BaseMDUnit, MDUnit);
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
                    PropertyChangedName = null;
                    _OnActualQuantityChanging = false;
                    _OnCalledUpQuantityChanging = false;
                }
            }
            base.OnPropertyChanged("RemainingCallQuantityUOM");
            base.OnPropertyChanged("DifferenceQuantityUOM");
        }

        [NotMapped]
        bool _OnActualQuantityChanging = false;
        protected void OnActualQuantityChanged()
        {
            if (!_OnActualQuantityUOMChanging && EntityState != EntityState.Detached && Material != null && MDUnit != null)
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

        [NotMapped]
        bool _OnActualQuantityUOMChanging = false;
        protected void OnActualQuantityUOMChanged()
        {
            if (!_OnActualQuantityChanging && EntityState != EntityState.Detached && Material != null && MDUnit != null)
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
        [NotMapped]
        public double RemainingCallQuantity
        {
            get
            {
                return TargetQuantity - CalledUpQuantity;
            }
        }

        [ACPropertyInfo(25, "", "en{'Remaining Quantity (UOM)'}de{'Restmenge (BME)'}")]
        [NotMapped]
        public double RemainingCallQuantityUOM
        {
            get
            {
                return TargetQuantityUOM - CalledUpQuantityUOM;
            }
        }

        [ACPropertyInfo(26, "", "en{'Difference Quantity'}de{'Differenzmenge'}")]
        [NotMapped]
        public double DifferenceQuantity
        {
            get
            {
                return ActualQuantity - TargetQuantity;
            }
        }

        [ACPropertyInfo(27, "", "en{'Difference Quantity (UOM)'}de{'Differenzmenge (BME)'}")]
        [NotMapped]
        public double DifferenceQuantityUOM
        {
            get
            {
                return ActualQuantityUOM - TargetQuantityUOM;
            }
        }


        [NotMapped]
        public double DifferenceWeight
        {
            get
            {
                return Material.ConvertToBaseWeight(DifferenceQuantityUOM);
            }
        }

        [NotMapped]
        public double TargetWeight
        {
            get
            {
                return Material.ConvertToBaseWeight(TargetQuantityUOM);
            }
        }

        [NotMapped]
        public double ActualWeight
        {
            get
            {
                return Material.ConvertToBaseWeight(ActualQuantityUOM);
            }
        }

        [NotMapped]
        public InOrderPos TopParentInOrderPos
        {
            get
            {
                if (this.InOrderPos1_ParentInOrderPos != null)
                    return this.InOrderPos1_ParentInOrderPos.TopParentInOrderPos;
                return this;
            }
        }


        public void RecalcActualQuantity()
        {
            if (this.EntityState != EntityState.Added)
            {
                this.InOrderPos_ParentInOrderPos.AutoLoad(this.InOrderPos_ParentInOrderPosReference, this);
            }

            double sumActualQuantity = 0;
            double sumActualQuantityUOM = 0;
            foreach (InOrderPos childPos in this.InOrderPos_ParentInOrderPos)
            {
                childPos.RecalcActualQuantity();
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

            DatabaseApp dbApp = null;
            var sumsPerUnitID = Context.Entry(this).Collection(c => c.FacilityBookingCharge_InOrderPos)
                                .Query()
                                .GroupBy(c => c.MDUnitID)
                                .Select(t => new { MDUnitID = t.Key, inwardQUOM = t.Sum(u => u.InwardQuantityUOM), inwardQ = t.Sum(u => u.InwardQuantity) })
                                .ToArray();
            MDUnit thisMDUnit = this.MDUnit;
            foreach (var sumPerUnit in sumsPerUnitID)
            {
                sumActualQuantityUOM += sumPerUnit.inwardQUOM;
                double quantity = sumPerUnit.inwardQ;
                if (thisMDUnit != null && sumPerUnit.MDUnitID != thisMDUnit.MDUnitID && this.Material != null)
                {
                    if (dbApp == null)
                        dbApp = this.GetObjectContext() as DatabaseApp;
                    MDUnit fromMDUnit = dbApp.MDUnit.Where(c => c.MDUnitID == sumPerUnit.MDUnitID).FirstOrDefault();
                    quantity = this.Material.ConvertQuantity(quantity, fromMDUnit, thisMDUnit);
                }
                sumActualQuantity += quantity;
            }

            //if (this.EntityState != System.Data.EntityState.Added)
            //{
            //    if (mergeOption.HasValue)
            //        this.FacilityBooking_InOrderPos.Load(mergeOption.Value);
            //    else
            //        this.FacilityBooking_InOrderPos.AutoLoad();
            //}
            //foreach (FacilityBooking fb in FacilityBooking_InOrderPos)
            //{
            //    foreach (FacilityBookingCharge fbc in fb.FacilityBookingCharge_FacilityBooking)
            //    {
            //        sumActualQuantityUOM += fbc.InwardQuantityUOM;
            //        try
            //        {
            //            sumActualQuantity += fbc.InwardMaterial.ConvertQuantity(fbc.InwardQuantity, fbc.MDUnit, this.MDUnit);
            //        }
            //        catch (Exception ec)
            //        {
            //            string msg = ec.Message;
            //            if (ec.InnerException != null && ec.InnerException.Message != null)
            //                msg += " Inner:" + ec.InnerException.Message;

            //            this.Root().Messages.LogException("InOrderPos", "RecalcActualQuantity(10)", msg);
            //        }
            //    }
            //}
            this.ActualQuantity = sumActualQuantity;
            this.ActualQuantityUOM = sumActualQuantityUOM;
        }

        public void RecalcDeliveryStates()
        {
            if (this.EntityState != EntityState.Added)
            {
                this.InOrderPos_ParentInOrderPos.AutoLoad(this.InOrderPos_ParentInOrderPosReference, this);
            }

            bool childsDelivererd = true;
            foreach (InOrderPos childPos in this.InOrderPos_ParentInOrderPos)
            {
                childPos.RecalcDeliveryStates();
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

        public double PreBookingInwardQuantityUOM()
        {
            if (this.EntityState != EntityState.Added)
            {
                this.InOrderPos_ParentInOrderPos.AutoLoad(this.InOrderPos_ParentInOrderPosReference, this);
            }

            double sumUOM = 0;
            foreach (InOrderPos childPos in this.InOrderPos_ParentInOrderPos)
            {
                sumUOM += childPos.PreBookingInwardQuantityUOM();
            }

            if (this.EntityState != EntityState.Added)
            {
                this.FacilityPreBooking_InOrderPos.AutoLoad(this.FacilityPreBooking_InOrderPosReference, this);
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
