using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

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
    [ACPropertyEntity(16, MDDelivPosLoadState.ClassName, "en{'Loading State'}de{'Beladungszustand'}", Const.ContextDatabase + "\\" + MDDelivPosLoadState.ClassName, "", true)]
    [ACPropertyEntity(9999, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
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
                if (picking.EntityState != System.Data.EntityState.Added)
                {
                    try
                    {
                        if (!picking.PickingPos_Picking.IsLoaded)
                            picking.PickingPos_Picking.Load();
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
            if (picking != null && picking.PickingPos_Picking.IsLoaded)
                picking.PickingPos_Picking.Remove(this);
            database.DeleteObject(this);
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
        public override IACObject ParentACObject
        {
            get
            {
                return Picking;
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

        #region Partial Properties
        [ACPropertyInfo(3, "", "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName)]
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

        [ACPropertyInfo(4, "", "en{'Unit of Measurement'}de{'Maßeinheit'}", Const.ContextDatabase + "\\MDUnitList")]
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
                    return this.PickingMaterial.BaseMDUnit;
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
            }
        }

        [ACPropertyInfo(5, "", ConstApp.TargetQuantity)]
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
                else if (this.PickingPosProdOrderPartslistPos_PickingPos != null && this.PickingPosProdOrderPartslistPos_PickingPos.Any())
                    return this.PickingPosProdOrderPartslistPos_PickingPos.Select(c => c.ProdorderPartslistPos.TargetQuantity).Sum();
                return 0;
            }
        }

        [ACPropertyInfo(6, "", "en{'Target Quantity (UOM)'}de{'Sollmenge (BME)'}")]
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
        }

        [ACPropertyInfo(7, "", "en{'Actual Quantity'}de{'Istmenge'}")]
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

        public double DiffQuantityUOM
        {
            get
            {
                return ActualQuantityUOM - TargetQuantityUOM;
            }
        }

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

        public double RemainingDosingWeight
        {
            get
            {
                return Material.ConvertToBaseWeight(RemainingDosingQuantityUOM);
            }
        }

        public double TargetWeight
        {
            get
            {
                return Material.ConvertToBaseWeight(TargetQuantityUOM);
            }
        }

        public double ActualWeight
        {
            get
            {
                return Material.ConvertToBaseWeight(ActualQuantityUOM);
            }
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

        public void RecalcActualQuantity(Nullable<MergeOption> mergeOption = null)
        {
            if (this.InOrderPos != null)
            {
                this.InOrderPos.RecalcActualQuantity(mergeOption);
                return;
            }
            else if (this.OutOrderPos != null)
            {
                this.OutOrderPos.RecalcActualQuantity(mergeOption);
                return;
            }

            if (this.EntityState != System.Data.EntityState.Added)
            {
                if (mergeOption.HasValue)
                    this.FacilityBooking_PickingPos.Load(mergeOption.Value);
                else
                    this.FacilityBooking_PickingPos.AutoLoad();
            }

            double sumActualQuantityUOM = 0;
            short postingType = 0;
            foreach (FacilityBooking fb in FacilityBooking_PickingPos)
            {
                foreach (FacilityBookingCharge fbc in fb.FacilityBookingCharge_FacilityBooking)
                {
                    if ((postingType == 0 || postingType == 1)
                        && Math.Abs(fbc.OutwardQuantityUOM - 0) > Double.Epsilon)
                    {
                        sumActualQuantityUOM += fbc.OutwardQuantityUOM;
                        postingType = 1;
                    }
                    else if (postingType == 0 || postingType == 2)
                    {
                        sumActualQuantityUOM += fbc.InwardQuantityUOM;
                        postingType = 2;
                    }
                    //try
                    //{
                    //    sumActualQuantity += fbc.OutwardMaterial.ConvertQuantity(fbc.OutwardQuantity, fbc.MDUnit, this.MDUnit);
                    //}
                    //catch (Exception ec)
                    //{
                    //    string msg = ec.Message;
                    //    if (ec.InnerException != null && ec.InnerException.Message != null)
                    //        msg += " Inner:" + ec.InnerException.Message;

                    //    this.Root().Messages.LogException(ClassName, "RecalcActualQuantity(10)", msg);
                    //}
                }
            }
            if (this.PickingActualUOM != sumActualQuantityUOM)
                this.PickingActualUOM = sumActualQuantityUOM;
        }

        partial void OnPickingMaterialIDChanged()
        {
            OnPropertyChanged(nameof(Material));
            OnPropertyChanged(nameof(MDUnit));
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
