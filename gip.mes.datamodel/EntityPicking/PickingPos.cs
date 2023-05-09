using gip.core.datamodel;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Picking Line'}de{'Kommissionierposition'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Sequenz'}", "", "", true)]
    [ACPropertyEntity(2, "LineNumber", "en{'Item Number'}de{'Positionsnummer'}", "", "", true)]
    [ACPropertyEntity(9, "FromFacility", "en{'From'}de{'Von'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(10, "ToFacility", "en{'To'}de{'Nach'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(11, "Comment", "en{'Comment'}de{'Bemerkung'}", "", "", true)]
    [ACPropertyEntity(12, "PickingMaterial", "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(13, "PickingQuantityUOM", "en{'Target Quantity (UOM)'}de{'Sollmenge (BME)'}", "", "", true)]
    [ACPropertyEntity(14, "PickingActualUOM", "en{'Actual Quantity (UOM)'}de{'Istmenge (BME)'}", "", "", true)]
    [ACPropertyEntity(15, OutOrderPos.ClassName, "en{'Sales Order Pos.'}de{'Verkaufsauftragsposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName, "", true)]
    [ACPropertyEntity(16, InOrderPos.ClassName, "en{'Purchase Order Pos.'}de{'Bestellposition'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName, "", true)]
    [ACPropertyEntity(17, MDDelivPosLoadState.ClassName, "en{'Loading State'}de{'Beladungszustand'}", Const.ContextDatabase + "\\" + MDDelivPosLoadState.ClassName, "", true)]
    [ACPropertyEntity(18, Picking.ClassName, ConstApp.PickingNo, Const.ContextDatabase + "\\" + Picking.ClassName, "", true)]
    [ACPropertyEntity(19, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + PickingPos.ClassName, "en{'Picking Line'}de{'Kommissionierposition'}", typeof(PickingPos), PickingPos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<PickingPos>) })]
    public partial class PickingPos
    {
        public const string ClassName = "PickingPos";

        #region New/Delete
        public static PickingPos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            PickingPos entity = new PickingPos();
            entity.PickingPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Picking)
            {
                Picking picking = parentACObject as Picking;
                if (picking.EntityState != EntityState.Added)
                {
                    try
                    {
                        if (!picking.PickingPos_Picking_IsLoaded)
                            picking.PickingPos_Picking.AutoLoad(picking.PickingPos_PickingReference, picking);
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        if (Database.Root != null && Database.Root.Messages != null)
                            Database.Root.Messages.LogException(ClassName, Const.MN_NewACObject, msg);
                    }
                }

                if (picking.PickingPos_Picking != null && picking.PickingPos_Picking.Select(c => c.Sequence).Any())
                    entity.Sequence = picking.PickingPos_Picking.Select(c => c.Sequence).Max() + 1;
                else
                    entity.Sequence = 1;
                entity.Picking = picking;
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
            int sequence = Sequence;
            Picking picking = Picking;
            if (picking != null && picking.PickingPos_Picking_IsLoaded)
                picking.PickingPos_Picking.Remove(this);
            database.Remove(this);
            if (picking != null)
            {
                PickingPos.RenumberSequence(picking, sequence);
                picking.UpdateDate = DateTime.Now;
            }
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(Picking inPicking, int sequence)
        {
            if (inPicking == null
                || !inPicking.PickingPos_Picking.Any())
                return;

            var elements = inPicking.PickingPos_Picking.Where(c => c.Sequence > sequence).OrderBy(c => c.Sequence);
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
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return Sequence.ToString();
            }
        }

        /// <summary>
        /// Returns Picking
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Picking</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return Picking;
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

        #endregion

        #region Partial Properties
        [ACPropertyInfo(3, "", "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName)]
        [NotMapped]
        public Material Material
        {
            get
            {
                if (this.InOrderPos != null)
                    return InOrderPos.Material;
                else if (this.OutOrderPos != null)
                    return OutOrderPos.Material;
                else if (this.PickingPosProdOrderPartslistPos_PickingPos != null && this.PickingPosProdOrderPartslistPos_PickingPos.Any())
                    return this.PickingPosProdOrderPartslistPos_PickingPos.Select(c => c.ProdorderPartslistPos.Material).FirstOrDefault();
                else if (this.PickingMaterial != null)
                    return this.PickingMaterial;
                return null;
            }
        }


        [NotMapped]
        private MDUnit _MDUnit;
        [ACPropertyInfo(4, "", "en{'Unit of Measurement'}de{'Maßeinheit'}", Const.ContextDatabase + "\\MDUnitList")]
        [NotMapped]
        public MDUnit MDUnit
        {
            get
            {
                if (this.InOrderPos != null)
                {
                    if (InOrderPos.MDUnit != null)
                        return InOrderPos.MDUnit;
                    if (InOrderPos.Material != null)
                        return InOrderPos.Material.BaseMDUnit;
                }
                else if (this.OutOrderPos != null)
                {
                    if (OutOrderPos.MDUnit != null)
                        return OutOrderPos.MDUnit;
                    if (OutOrderPos.Material != null)
                        return OutOrderPos.Material.BaseMDUnit;
                }
                else if (this.PickingPosProdOrderPartslistPos_PickingPos != null && this.PickingPosProdOrderPartslistPos_PickingPos.Any())
                {
                    var mdUnit = this.PickingPosProdOrderPartslistPos_PickingPos.Select(c => c.ProdorderPartslistPos.MDUnit).FirstOrDefault();
                    if (mdUnit == null)
                        mdUnit = this.PickingPosProdOrderPartslistPos_PickingPos.Select(c => c.ProdorderPartslistPos.Material.BaseMDUnit).FirstOrDefault();
                    return mdUnit;
                }
                else if (this.PickingMaterial != null)
                {
                    if(_MDUnit == null)
                        _MDUnit = PickingMaterial.BaseMDUnit;
                    return _MDUnit;
                }
                return null;
            }
            set
            {
                if (this.InOrderPos != null)
                {
                    InOrderPos.MDUnit = value;
                    if (InOrderPos.MDUnit != null)
                    {
                        InOrderPos.TargetQuantity = InOrderPos.Material.ConvertQuantity(InOrderPos.TargetQuantityUOM, InOrderPos.Material.BaseMDUnit, InOrderPos.MDUnit);
                        InOrderPos.ActualQuantity = InOrderPos.Material.ConvertQuantity(InOrderPos.ActualQuantityUOM, InOrderPos.Material.BaseMDUnit, InOrderPos.MDUnit);
                        InOrderPos.CalledUpQuantity = InOrderPos.Material.ConvertQuantity(InOrderPos.CalledUpQuantityUOM, InOrderPos.Material.BaseMDUnit, InOrderPos.MDUnit);
                    }
                }
                else if (this.OutOrderPos != null)
                {
                    OutOrderPos.MDUnit = value;
                    if (OutOrderPos.MDUnit != null)
                    {
                        OutOrderPos.TargetQuantity = OutOrderPos.Material.ConvertQuantity(OutOrderPos.TargetQuantityUOM, OutOrderPos.Material.BaseMDUnit, OutOrderPos.MDUnit);
                        OutOrderPos.ActualQuantity = OutOrderPos.Material.ConvertQuantity(OutOrderPos.ActualQuantityUOM, OutOrderPos.Material.BaseMDUnit, OutOrderPos.MDUnit);
                        OutOrderPos.CalledUpQuantity = OutOrderPos.Material.ConvertQuantity(OutOrderPos.CalledUpQuantityUOM, OutOrderPos.Material.BaseMDUnit, OutOrderPos.MDUnit);
                    }
                }
                else if (this.PickingPosProdOrderPartslistPos_PickingPos != null && this.PickingPosProdOrderPartslistPos_PickingPos.Any())
                {
                    var pickingPos = this.PickingPosProdOrderPartslistPos_PickingPos.FirstOrDefault();
                    if (pickingPos == null)
                    {
                        ProdOrderPartslistPos pos = pickingPos.ProdorderPartslistPos;
                        if (pos != null)
                        {
                            pos.MDUnit = value;
                            if (pos.MDUnit != null)
                            {
                                pos.TargetQuantity = pos.Material.ConvertQuantity(pos.TargetQuantityUOM, pos.Material.BaseMDUnit, pos.MDUnit);
                                pos.ActualQuantity = pos.Material.ConvertQuantity(pos.ActualQuantityUOM, pos.Material.BaseMDUnit, pos.MDUnit);
                                pos.CalledUpQuantity = pos.Material.ConvertQuantity(pos.CalledUpQuantityUOM, pos.Material.BaseMDUnit, pos.MDUnit);
                            }
                        }
                    }
                }
                else if (this.PickingMaterial != null)
                {
                    PickingQuantityUOM = PickingMaterial.ConvertQuantity(TargetQuantity, _MDUnit, value);
                    _MDUnit = value;
                    OnPropertyChanged(nameof(TargetQuantity));
                    OnPropertyChanged(nameof(TargetQuantityUOM));
                    OnPropertyChanged(nameof(MDUnit));
                }
            }
        }

        [ACPropertyInfo(5, "", ConstApp.TargetQuantity)]
        [NotMapped]
        public double TargetQuantity
        {
            get
            {
                if (this.InOrderPos != null)
                    return InOrderPos.TargetQuantity;
                else if (this.OutOrderPos != null)
                    return OutOrderPos.TargetQuantity;
                else if (this.PickingMaterial != null && this.PickingQuantityUOM.HasValue)
                    return this.PickingQuantityUOM.Value;
                    //return PickingMaterial.ConvertQuantity(PickingQuantityUOM.Value, Material.BaseMDUnit, MDUnit);
                else if (this.PickingPosProdOrderPartslistPos_PickingPos != null && this.PickingPosProdOrderPartslistPos_PickingPos.Any())
                    return this.PickingPosProdOrderPartslistPos_PickingPos.Select(c => c.ProdorderPartslistPos.TargetQuantity).Sum();
                return 0;
            }
            set
            {
                if (this.InOrderPos != null)
                {
                    //InOrderPos.TargetQuantity = value;
                }
                else if (this.OutOrderPos != null)
                {
                    // OutOrderPos.TargetQuantity = value;
                }
                else if (this.PickingMaterial != null)
                {
                    PickingQuantityUOM = value;
                    //PickingQuantityUOM = PickingMaterial.ConvertToBaseQuantity(value, MDUnit);
                }
                //else if (this.PickingPosProdOrderPartslistPos_PickingPos != null && this.PickingPosProdOrderPartslistPos_PickingPos.Any())
                //    return this.PickingPosProdOrderPartslistPos_PickingPos.Select(c => c.ProdorderPartslistPos.TargetQuantity).Sum();

                OnPropertyChanged(nameof(TargetQuantity));
                OnPropertyChanged(nameof(TargetQuantityUOM));
            }
        }

        [ACPropertyInfo(6, "", "en{'Target Quantity (UOM)'}de{'Sollmenge (BME)'}")]
        [NotMapped]
        public double TargetQuantityUOM
        {
            get
            {
                if (this.InOrderPos != null)
                    return InOrderPos.TargetQuantityUOM;
                else if (this.OutOrderPos != null)
                    return OutOrderPos.TargetQuantityUOM;
                else if (this.PickingMaterial != null && this.PickingQuantityUOM.HasValue)
                    return this.PickingQuantityUOM.Value;
                else if (this.PickingPosProdOrderPartslistPos_PickingPos != null && this.PickingPosProdOrderPartslistPos_PickingPos.Any())
                    return this.PickingPosProdOrderPartslistPos_PickingPos.Select(c => c.ProdorderPartslistPos.TargetQuantityUOM).Sum();
                return 0;
            }
            set
            {
                if (this.InOrderPos != null)
                {
                    //InOrderPos.TargetQuantityUOM = value;
                }
                else if (this.OutOrderPos != null)
                {
                    //OutOrderPos.TargetQuantityUOM = value;
                }
                else if (this.PickingMaterial != null)
                {
                    PickingQuantityUOM = value;
                }

                OnPropertyChanged(nameof(TargetQuantity));
                OnPropertyChanged(nameof(TargetQuantityUOM));
            }
        }

        [ACPropertyInfo(7, "", "en{'Actual Quantity'}de{'Istmenge'}")]
        [NotMapped]
        public double ActualQuantity
        {
            get
            {
                if (this.InOrderPos != null)
                    return InOrderPos.ActualQuantity;
                else if (this.OutOrderPos != null)
                    return OutOrderPos.ActualQuantity;
                else if (this.PickingPosProdOrderPartslistPos_PickingPos.Any())
                    return this.PickingPosProdOrderPartslistPos_PickingPos.Sum(c => c.ProdorderPartslistPos.ActualQuantity);
                else if (PickingActualUOM.HasValue)
                    return PickingActualUOM.Value;
                return 0;
            }
        }

        [ACPropertyInfo(8, "", "en{'Actual Quantity (UOM)'}de{'Istmenge (BME)'}")]
        [NotMapped]
        public double ActualQuantityUOM
        {
            get
            {
                if (this.InOrderPos != null)
                    return InOrderPos.ActualQuantityUOM;
                else if (this.OutOrderPos != null)
                    return OutOrderPos.ActualQuantityUOM;
                else if (this.PickingPosProdOrderPartslistPos_PickingPos.Any())
                    return this.PickingPosProdOrderPartslistPos_PickingPos.Sum(c => c.ProdorderPartslistPos.ActualQuantityUOM);
                else if (PickingActualUOM.HasValue)
                    return PickingActualUOM.Value;
                return 0;
            }
        }

        [NotMapped]
        public double DiffQuantityUOM
        {
            get
            {
                return ActualQuantityUOM - TargetQuantityUOM;
            }
        }

        [NotMapped]
        public double PickingDiffQuantityUOM
        {
            get
            {
                if (this.PickingQuantityUOM.HasValue)
                    return PickingActualUOM.HasValue ? PickingActualUOM.Value - this.PickingQuantityUOM.Value : 0 - this.PickingQuantityUOM.Value;
                return 0;
            }
        }

        /// <summary>
        /// Returns ActualQuantity - TargetQuantity (A negative value)
        /// </summary>
        [NotMapped]
        public double RemainingDosingQuantityUOM
        {
            get
            {
                if (this.PickingQuantityUOM.HasValue)
                    return PickingDiffQuantityUOM;
                else
                    return DiffQuantityUOM;
            }
        }

        [NotMapped]
        public double RemainingDosingWeight
        {
            get
            {
                try
                {
                    return Material.ConvertToBaseWeight(RemainingDosingQuantityUOM);
                }
                catch (Exception e)
                {
                    Database.Root.Messages.LogException(nameof(PickingPos), nameof(RemainingDosingWeight), e);
                    return double.NaN;
                }
            }
        }

        [NotMapped]
        public string RemainingDosingWeightError
        {
            get
            {
                try
                {
                    Material.ConvertToBaseWeight(RemainingDosingQuantityUOM);
                    return null;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
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

        [ACPropertyInfo(9999)]
        [NotMapped]
        public double CompleteFactor
        {
            get => (ActualQuantity / TargetQuantity) * 100;
        }

        public void RecalcAfterPosting(DatabaseApp dbApp, double postedQuantityUOM, bool isCancellation, bool autoSetState = false)
        {
            if (InOrderPos != null)
            {
                InOrderPos.TopParentInOrderPos.RecalcActualQuantity();
                if (isCancellation)
                {
                    MDInOrderPosState state = dbApp.MDInOrderPosState.Where(c => c.MDInOrderPosStateIndex == (short)MDInOrderPosState.InOrderPosStates.Cancelled).FirstOrDefault();
                    if (state != null)
                        InOrderPos.MDInOrderPosState = state;
                    InOrderPos.TopParentInOrderPos.CalledUpQuantity -= InOrderPos.TargetQuantity;
                    InOrderPos.TargetQuantity = 0;
                    InOrderPos.TargetQuantityUOM = 0;
                }
                else
                {
                    if (autoSetState && DiffQuantityUOM >= 0)
                    {
                        MDInOrderPosState state = dbApp.MDInOrderPosState.Where(c => c.MDInOrderPosStateIndex == (short)MDInOrderPosState.InOrderPosStates.Completed).FirstOrDefault();
                        if (state != null)
                            InOrderPos.MDInOrderPosState = state;
                    }
                }
            }
            else if (OutOrderPos != null)
            {
                OutOrderPos.TopParentOutOrderPos.RecalcActualQuantity();
                if (isCancellation)
                {
                    MDOutOrderPosState state = dbApp.MDOutOrderPosState.Where(c => c.MDOutOrderPosStateIndex == (short)MDOutOrderPosState.OutOrderPosStates.Cancelled).FirstOrDefault();
                    if (state != null)
                        OutOrderPos.MDOutOrderPosState = state;
                    OutOrderPos.TopParentOutOrderPos.CalledUpQuantity -= OutOrderPos.TargetQuantity;
                    OutOrderPos.TargetQuantity = 0;
                    OutOrderPos.TargetQuantityUOM = 0;
                }
                else
                {
                    if (autoSetState && DiffQuantityUOM >= 0)
                    {
                        MDOutOrderPosState state = dbApp.MDOutOrderPosState.Where(c => c.MDOutOrderPosStateIndex == (short)MDOutOrderPosState.OutOrderPosStates.Completed).FirstOrDefault();
                        if (state != null)
                            OutOrderPos.MDOutOrderPosState = state;
                    }
                }
            }
            else
            {
                IncreasePickingActualUOM(postedQuantityUOM);
                RecalcActualQuantity();
            }
            OnPropertyChanged("ActualQuantity");
            OnPropertyChanged("ActualQuantityUOM");
            OnPropertyChanged("DiffQuantityUOM");
            OnPropertyChanged("PickingDiffQuantityUOM");
        }

        public void IncreasePickingActualUOM(double quantityUOM, bool autoRefresh = false)
        {
            PickingActualUOM2(quantityUOM, autoRefresh, 0);
        }

        private void PickingActualUOM2(double quantityUOM, bool autoRefresh, short recCounter)
        {
            if (this.PickingQuantityUOM.HasValue)
            {
                if (!this.PickingActualUOM.HasValue)
                    this.PickingActualUOM = 0;
                this.PickingActualUOM += quantityUOM;
            }
        }

        public void RecalcActualQuantity()
        {
            if (this.InOrderPos != null)
            {
                this.InOrderPos.RecalcActualQuantity();
                return;
            }
            else if (this.OutOrderPos != null)
            {
                this.OutOrderPos.RecalcActualQuantity();
                return;
            }

            DatabaseApp dbApp = null;
            double sumActualQuantityUOM = 0;
            var sumsPerUnitID = Context.Entry(this).Collection(c => c.FacilityBookingCharge_PickingPos)
                                                   .Query()
                                                   .GroupBy(c => c.MDUnitID)
                                                   .Select(t => new { MDUnitID = t.Key, outwardQUOM = t.Sum(u => u.OutwardQuantityUOM), inwardQUOM = t.Sum(u => u.InwardQuantityUOM) })
                                                   .ToArray();
            MDUnit thisMDUnit = this.MDUnit;
            foreach (var sumPerUnit in sumsPerUnitID)
            {
                double quantity = Math.Abs(sumPerUnit.outwardQUOM) > Double.Epsilon ? sumPerUnit.outwardQUOM : sumPerUnit.inwardQUOM;
                if (thisMDUnit != null && sumPerUnit.MDUnitID != thisMDUnit.MDUnitID)
                {
                    if (dbApp == null)
                        dbApp = this.GetObjectContext() as DatabaseApp;
                    MDUnit fromMDUnit = dbApp.MDUnit.Where(c => c.MDUnitID == sumPerUnit.MDUnitID).FirstOrDefault();
                    quantity = this.Material.ConvertQuantity(quantity, fromMDUnit, thisMDUnit);
                }
                sumActualQuantityUOM += quantity;
            }

            //double sumActualQuantityUOM = this.FacilityBooking_PickingPos.SelectMany(c => c.FacilityBookingCharge_FacilityBooking).Sum(c => c.OutwardQuantityUOM);
            //if (Math.Abs(sumActualQuantityUOM) <= Double.Epsilon)
            //    sumActualQuantityUOM = this.FacilityBooking_PickingPos.SelectMany(c => c.FacilityBookingCharge_FacilityBooking).Sum(c => c.InwardQuantityUOM);

            //if (this.EntityState != System.Data.EntityState.Added)
            //{
            //    if (mergeOption.HasValue)
            //        this.FacilityBooking_PickingPos.Load(mergeOption.Value);
            //    else
            //        this.FacilityBooking_PickingPos.AutoLoad();
            //}

            //short postingType = 0;
            //foreach (FacilityBooking fb in FacilityBooking_PickingPos)
            //{
            //    foreach (FacilityBookingCharge fbc in fb.FacilityBookingCharge_FacilityBooking)
            //    {
            //        if ((postingType == 0 || postingType == 1)
            //            && Math.Abs(fbc.OutwardQuantityUOM - 0) > Double.Epsilon)
            //        {
            //            sumActualQuantityUOM += fbc.OutwardQuantityUOM;
            //            postingType = 1;
            //        }
            //        else if (postingType == 0 || postingType == 2)
            //        {
            //            sumActualQuantityUOM += fbc.InwardQuantityUOM;
            //            postingType = 2;
            //        }
            //    }
            //}
            if (this.PickingActualUOM != sumActualQuantityUOM)
                this.PickingActualUOM = sumActualQuantityUOM;
        }

        public void OnLocalPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        public void OnRefreshCompleteFactor()
        {
            OnPropertyChanged(nameof(CompleteFactor));
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(PickingMaterialID))
            {
                base.OnPropertyChanged(nameof(Material));
                base.OnPropertyChanged(nameof(MDUnit));
            }
            base.OnPropertyChanged(propertyName);
        }

        #endregion

        #region Cloning

        public object Clone(bool withReferences)
        {
            PickingPos clonedObject = new PickingPos();
            clonedObject.PickingPosID = this.PickingPosID;
            clonedObject.CopyFrom(this, withReferences, true);
            return clonedObject;
        }

        public void CopyFrom(PickingPos from, bool withReferences, bool copyActualQuantity)
        {
            if (withReferences)
            {
                OutOrderPosID = from.OutOrderPosID;
                InOrderPosID = from.InOrderPosID;
                FromFacilityID = from.FromFacilityID;
                ToFacilityID = from.ToFacilityID;
                PickingMaterialID = from.PickingMaterialID;
                MDDelivPosLoadStateID = from.MDDelivPosLoadStateID;
            }

            Sequence = from.Sequence;
            Comment = from.Comment;
            XMLConfig = from.XMLConfig;
            InsertName = from.InsertName;
            InsertDate = from.InsertDate;
            UpdateName = from.UpdateName;
            UpdateDate = from.UpdateDate;
            LineNumber = from.LineNumber;
            PickingQuantityUOM = from.PickingQuantityUOM;
            if (copyActualQuantity)
                PickingActualUOM = from.PickingActualUOM;
            KeyOfExtSys = from.KeyOfExtSys;
        }
        #endregion

    }
}
