using gip.core.datamodel;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Production Order Pos. Status'}de{'Produktionsauftrag Pos.-Status'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Sequenz'}", "", "", true)]
    [ACPropertyEntity(2, "TargetQuantityUOM", "en{'Target Quantity (UOM)'}de{'Sollmenge (BME)'}", "", "", true)]
    [ACPropertyEntity(3, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(4, "ActualQuantityUOM", "en{'Actual Quantity (UOM)'}de{'Istmenge (BME)'}", "", "", true)]
    [ACPropertyEntity(5, "ActualQuantity", "en{'Actual Quantity'}de{'Istmenge'}", "", "", true)]
    [ACPropertyEntity(6, MDToleranceState.ClassName, "en{'Tolerance Status'}de{'Toleranzstatus'}", Const.ContextDatabase + "\\" + MDToleranceState.ClassName, "", true)]
    [ACPropertyEntity(7, MDProdOrderPartslistPosState.ClassName, "en{'Status'}de{'Status'}", Const.ContextDatabase + "\\" + MDProdOrderPartslistPosState.ClassName, "", true)]
    [ACPropertyEntity(8, ProdOrderBatch.ClassName, "en{'Batch'}de{'Charge'}", Const.ContextDatabase + "\\" + ProdOrderBatch.ClassName, "", true)]
    [ACPropertyEntity(9, "SourceProdOrderPartslistPos", "en{'Subitem from'}de{'Unterposition von'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName, "", true)]
    [ACPropertyEntity(10, "TargetProdOrderPartslistPos", "en{'Intermediate Product'}de{'Zwischenprodukt'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName, "", true)]
    [ACPropertyEntity(11, "ParentProdOrderPartslistPosRelation", "en{'Partial Quantity from'}de{'Teilmenge von'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPosRelation.ClassName, "", true)]
    [ACPropertyEntity(13, "RetrogradeFIFO", "en{'Backflushing'}de{'Retrograde Entnahme'}", "", "", true)]
    [ACPropertyEntity(14, "Anterograde", "en{'Anterograde inward posting'}de{'Anterograde Zugangsbuchung'}", "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + ProdOrderPartslistPosRelation.ClassName, "en{'Component Part'}de{'Teilmenge'}", typeof(ProdOrderPartslistPosRelation), ProdOrderPartslistPosRelation.ClassName, "", "ProdOrderPartslistPosRelationID")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<ProdOrderPartslistPosRelation>) })]
    public partial class ProdOrderPartslistPosRelation : IACObjectEntity, IACObject, IPartslistPosRelation
    {
        public const string ClassName = "ProdOrderPartslistPosRelation";

        #region New/Delete
        public static ProdOrderPartslistPosRelation NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            ProdOrderPartslistPosRelation entity = new ProdOrderPartslistPosRelation();
            entity.ProdOrderPartslistPosRelationID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDToleranceState = MDToleranceState.DefaultMDToleranceState(dbApp);
            entity.MDProdOrderPartslistPosState = MDProdOrderPartslistPosState.DefaultMDProdOrderPartslistPosState(dbApp);
            if (parentACObject != null)
            {
                if (parentACObject is ProdOrderPartslistPosRelation)
                {
                    entity.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation = parentACObject as ProdOrderPartslistPosRelation;
                    entity.CopyFromParent(parentACObject as ProdOrderPartslistPosRelation);
                }
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
            database.Remove(this);
            return null;
        }

        public void CopyFromParent(ProdOrderPartslistPosRelation parentPos)
        {
            this.MDToleranceState = parentPos.MDToleranceState;
            this.MDProdOrderPartslistPosState = parentPos.MDProdOrderPartslistPosState;
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "Sequence,TargetProdOrderPartslistPos\\Sequence,TargetProdOrderPartslistPos\\Material\\MaterialNo,TargetProdOrderPartslistPos\\MaterialPosTypeIndex";
            }
        }

        /// <summary>
        /// Returns SourceProdOrderPartslistPos
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to SourceProdOrderPartslistPos</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return SourceProdOrderPartslistPos;
            }
        }

        #endregion

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(TargetQuantity):
                    OnTargetQuantityChanged();
                    break;
                case nameof(TargetQuantityUOM):
                    OnTargetQuantityUOMChanged();
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

        #region Partial methods

        [NotMapped]
        public ProdOrderPartslistPosRelation TopParentPartslistPosRelation
        {
            get
            {
                if (this.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation != null)
                    return this.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.TopParentPartslistPosRelation;
                return this;
            }
        }

        [NotMapped]
        bool _OnTargetQuantityChanging = false;
        protected void OnTargetQuantityChanged()
        {
            if (!_OnTargetQuantityUOMChanging && SourceProdOrderPartslistPos != null && EntityState != EntityState.Detached && SourceProdOrderPartslistPos.Material != null)
            {
                _OnTargetQuantityChanging = true;
                try
                {
                    if (SourceProdOrderPartslistPos.MDUnit != null && SourceProdOrderPartslistPos.MDUnit != SourceProdOrderPartslistPos.Material.BaseMDUnit)
                    {
                        TargetQuantityUOM = SourceProdOrderPartslistPos.Material.ConvertToBaseQuantity(TargetQuantity, SourceProdOrderPartslistPos.MDUnit);
                    }
                    else if (TargetQuantityUOM != TargetQuantity)
                    {
                        TargetQuantityUOM = TargetQuantity;
                    }
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnTargetQuantityChanged", msg);
                }
                finally
                {
                    _OnTargetQuantityChanging = false;
                }
            }
            base.OnPropertyChanged("DifferenceQuantity");
        }

        [NotMapped]
        bool _OnTargetQuantityUOMChanging = false;
        protected void OnTargetQuantityUOMChanged()
        {
            if (!_OnTargetQuantityChanging && EntityState != EntityState.Detached && SourceProdOrderPartslistPos != null && SourceProdOrderPartslistPos.Material != null)
            {
                _OnTargetQuantityUOMChanging = true;
                try
                {
                    if (SourceProdOrderPartslistPos.MDUnit != null && SourceProdOrderPartslistPos.MDUnit != SourceProdOrderPartslistPos.Material.BaseMDUnit)
                    {
                        TargetQuantity = SourceProdOrderPartslistPos.Material.ConvertQuantity(TargetQuantityUOM, SourceProdOrderPartslistPos.Material.BaseMDUnit, SourceProdOrderPartslistPos.MDUnit);
                    }
                    else if (TargetQuantity != TargetQuantityUOM)
                    {
                        TargetQuantity = TargetQuantityUOM;
                    }
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnTargetQuantityUOMChanged", msg);
                }
                finally
                {
                    _OnTargetQuantityUOMChanging = false;
                }
            }
            base.OnPropertyChanged("DifferenceQuantityUOM");
        }

        [NotMapped]
        bool _OnActualQuantityChanging = false;
        protected void OnActualQuantityChanged()
        {
            if (!_OnActualQuantityUOMChanging && this.SourceProdOrderPartslistPos != null && EntityState != EntityState.Detached
                && this.SourceProdOrderPartslistPos.Material != null && this.SourceProdOrderPartslistPos.MDUnit != null)
            {
                _OnActualQuantityChanging = true;
                try
                {
                    ActualQuantityUOM = this.SourceProdOrderPartslistPos.Material.ConvertToBaseQuantity(ActualQuantity, this.SourceProdOrderPartslistPos.MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnActualQuantityChanged", msg);
                }
                finally
                {
                    _OnActualQuantityChanging = false;
                }
            }
            base.OnPropertyChanged("DifferenceQuantity");
        }

        [NotMapped]
        bool _OnActualQuantityUOMChanging = false;
        protected void OnActualQuantityUOMChanged()
        {
            if (!_OnActualQuantityChanging && this.SourceProdOrderPartslistPos != null && EntityState != EntityState.Detached
                && this.SourceProdOrderPartslistPos.Material != null && this.SourceProdOrderPartslistPos.MDUnit != null)
            {
                _OnActualQuantityUOMChanging = true;
                try
                {
                    ActualQuantity = this.SourceProdOrderPartslistPos.Material.ConvertQuantity(ActualQuantityUOM, this.SourceProdOrderPartslistPos.Material.BaseMDUnit, this.SourceProdOrderPartslistPos.MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnActualQuantityUOMChanged", msg);
                }
                finally
                {
                    _OnActualQuantityUOMChanging = false;
                }
            }
            base.OnPropertyChanged("DifferenceQuantityUOM");
        }

        [ACPropertyInfo(24, "", "en{'Difference Quantity'}de{'Differenzmenge'}")]
        [NotMapped]
        public double DifferenceQuantity
        {
            get
            {
                return ActualQuantity - TargetQuantity;
            }
        }

        [ACPropertyInfo(25, "", "en{'Difference Quantity (UOM)'}de{'Differenzmenge (BME)'}")]
        [NotMapped]
        public double DifferenceQuantityUOM
        {
            get
            {
                return ActualQuantityUOM - TargetQuantityUOM;
            }
        }

        /// <summary>
        /// The value is negative if the actual quantity is lower than the target quantity
        /// </summary>
        [ACPropertyInfo(26, "", "en{'Remaining Dosing Quantity (UOM)'}de{'Dosierrestmenge (BME)'}")]
        [NotMapped]
        public double RemainingDosingQuantityUOM
        {
            get
            {
                double wasteQ = this.FacilityBooking_ProdOrderPartslistPosRelation.Where(c => c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.Discarded).Sum(c => c.OutwardQuantity);
                //double newDosedQ = this.FacilityBooking_ProdOrderPartslistPosRelation.Where(c => c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New).Sum(c => c.OutwardQuantity);
                return ActualQuantityUOM - (TargetQuantityUOM + wasteQ);
            }
        }

        [NotMapped]
        public double RemainingDosingWeight
        {
            get
            {
                try
                {
                    return SourceProdOrderPartslistPos.Material.ConvertToBaseWeight(RemainingDosingQuantityUOM);
                }
                catch (Exception e)
                {
                    Database.Root.Messages.LogException("ProdOrderPartslistPosRelation", "RemainingDosingWeight", e);
                    return double.NaN;
                }
            }
        }

        [NotMapped]
        public double TargetWeight
        {
            get
            {
                try
                {
                    return SourceProdOrderPartslistPos.Material.ConvertToBaseWeight(TargetQuantityUOM);
                }
                catch (Exception e)
                {
                    Database.Root.Messages.LogException("ProdOrderPartslistPosRelation", "TargetWeight", e);
                    return 0;
                }
            }
        }

        [NotMapped]
        public double ActualWeight
        {
            get
            {
                try
                {
                    return SourceProdOrderPartslistPos.Material.ConvertToBaseWeight(ActualQuantityUOM);
                }
                catch (Exception e)
                {
                    Database.Root.Messages.LogException("ProdOrderPartslistPosRelation", "ActualWeight", e);
                    return 0;
                }
            }
        }

        public void IncreaseActualQuantityUOM(double quantityUOM, bool autoRefresh = false)
        {
            IncreaseActualQuantityUOM2(quantityUOM, autoRefresh, 0);
        }

        private void IncreaseActualQuantityUOM2(double quantityUOM, bool autoRefresh, short recCounter)
        {
            this.ActualQuantityUOM += quantityUOM;
            if (recCounter == 0)
                this.SourceProdOrderPartslistPos.IncreaseActualQuantityUOM(quantityUOM, autoRefresh);
            recCounter++;
            if (this.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation != null)
            {
                if (autoRefresh)
                    this.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.AutoRefresh();
                this.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.IncreaseActualQuantityUOM2(quantityUOM, autoRefresh, recCounter);
            }
        }

        public void IncreaseActualQuantity(double quantity, MDUnit mdUnit, bool autoRefresh = false)
        {
            IncreaseActualQuantity2(quantity, mdUnit, autoRefresh, 0);
        }

        private void IncreaseActualQuantity2(double quantity, MDUnit mdUnit, bool autoRefresh, short recCounter)
        {
            this.ActualQuantity += this.SourceProdOrderPartslistPos.Material.ConvertQuantity(quantity, mdUnit, this.SourceProdOrderPartslistPos.MDUnit);
            if (recCounter == 0)
                this.SourceProdOrderPartslistPos.IncreaseActualQuantity(quantity, mdUnit, autoRefresh);
            recCounter++;
            if (this.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation != null)
            {
                if (autoRefresh)
                    this.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.AutoRefresh();
                this.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.IncreaseActualQuantity2(quantity, mdUnit, autoRefresh, recCounter);
            }
        }


        public void RecalcActualQuantityFast()
        {
            ProdOrderPartslistPosRelation root = this;
            if (this.ParentProdOrderPartslistPosRelationID.HasValue)
                root = this.TopParentPartslistPosRelation;
            root.RecalcActualQuantityFast2();
            double sum = 0;
            foreach (var relation in root.SourceProdOrderPartslistPos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Where(c => !c.ParentProdOrderPartslistPosRelationID.HasValue))
            {
                sum += relation.ActualQuantityUOM;
            }
            if (Math.Abs(root.SourceProdOrderPartslistPos.ActualQuantityUOM - sum) > Double.Epsilon)
                root.SourceProdOrderPartslistPos.ActualQuantityUOM = sum;
        }

        private void RecalcActualQuantityFast2()
        {
            double sum = 0;
            if (!this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation.Any())
                return;
            if (this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation.Where(c => c.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation.Any()).Any())
            {
                foreach (ProdOrderPartslistPosRelation childPos in this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation)
                {
                    childPos.RecalcActualQuantityFast2();
                    sum += childPos.ActualQuantityUOM;
                }
            }
            else
            {
                sum = this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation.Sum(c => c.ActualQuantityUOM);
            }
            if (Math.Abs(this.ActualQuantityUOM - sum) > Double.Epsilon)
                this.ActualQuantityUOM = sum;
        }


        public void RecalcActualQuantity()
        {
            this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation.AutoLoad(this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelationReference, this);

            double sumActualQuantity = 0;
            double sumActualQuantityUOM = 0;
            foreach (ProdOrderPartslistPosRelation childPos in this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation)
            {
                childPos.RecalcActualQuantity();
                sumActualQuantity += childPos.ActualQuantity;
                sumActualQuantityUOM += childPos.ActualQuantityUOM;
            }

            DatabaseApp dbApp = null;
            var sumsPerUnitID = Context.Entry(this).Collection(c => c.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                                            .Query()
                                            .GroupBy(c => c.MDUnitID)
                                            .Select(t => new { MDUnitID = t.Key, quantity = t.Sum(u => u.OutwardQuantity), quantityUOM = t.Sum(u => u.OutwardQuantityUOM) })
                                            .ToArray();
            MDUnit thisMDUnit = this.SourceProdOrderPartslistPos.MDUnit;
            foreach (var sumPerUnit in sumsPerUnitID)
            {
                double quantity = sumPerUnit.quantityUOM;
                sumActualQuantityUOM += quantity;
                quantity = sumPerUnit.quantity;
                if (thisMDUnit != null && sumPerUnit.MDUnitID != thisMDUnit.MDUnitID)
                {
                    if (dbApp == null)
                        dbApp = this.GetObjectContext() as DatabaseApp;
                    MDUnit fromMDUnit = dbApp.MDUnit.Where(c => c.MDUnitID == sumPerUnit.MDUnitID).FirstOrDefault();
                    quantity = this.SourceProdOrderPartslistPos.Material.ConvertQuantity(quantity, fromMDUnit, thisMDUnit);
                }
                sumActualQuantity += quantity;
            }
            this.ActualQuantity = sumActualQuantity;
            this.ActualQuantityUOM = sumActualQuantityUOM;

            //if (sums != null)
            //{
            //    this.ActualQuantity = sums.quantity;
            //    this.ActualQuantityUOM = sums.quantityUOM;
            //}

            //if (mergeOption.HasValue)
            //    this.FacilityBooking_ProdOrderPartslistPosRelation.Load(mergeOption.Value);
            //else
            //    this.FacilityBooking_ProdOrderPartslistPosRelation.AutoLoad();
            //foreach (FacilityBooking fb in FacilityBooking_ProdOrderPartslistPosRelation)
            //{
            //    foreach (FacilityBookingCharge fbc in fb.FacilityBookingCharge_FacilityBooking)
            //    {
            //        sumActualQuantity += fbc.OutwardQuantity;
            //        sumActualQuantityUOM += fbc.OutwardQuantityUOM;
            //    }
            //}
            //this.ActualQuantity = sumActualQuantity;
            //this.ActualQuantityUOM = sumActualQuantityUOM;
        }

        [NotMapped]
        public string RemainingDosingWeightError
        {
            get
            {
                try
                {
                    SourceProdOrderPartslistPos.Material.ConvertToBaseWeight(RemainingDosingQuantityUOM);
                    return null;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }

        #endregion

        #region IACUrl member

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                if (SourceProdOrderPartslistPos == null || SourceProdOrderPartslistPos.Material == null)
                    return Sequence.ToString();
                return Sequence.ToString() + " " + SourceProdOrderPartslistPos.Material.ACCaption + " -rel";
            }
        }
        #endregion

        #region Additional Properties
        /// <summary>
        /// Property that evaluates the override of the RetrogradeFIFO-Fields in Tables PartslistPos->Material
        /// </summary>
        [NotMapped]
        public bool Backflushing
        {
            get
            {
                if (this.RetrogradeFIFO.HasValue)
                    return this.RetrogradeFIFO.Value;
                if (ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation != null)
                    return ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.Backflushing;
                if (SourceProdOrderPartslistPos != null)
                    return SourceProdOrderPartslistPos.Backflushing;
                return false;
            }
        }

        /// <summary>
        /// Property that evaluates the override of the Anterograde-Fields in Tables PartslistPos->Material
        /// </summary>
        [NotMapped]
        public bool Foreflushing
        {
            get
            {
                if (this.Anterograde.HasValue)
                    return this.Anterograde.Value;
                if (ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation != null)
                    return ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.Foreflushing;
                if (SourceProdOrderPartslistPos != null)
                    return SourceProdOrderPartslistPos.Foreflushing;
                return false;
            }
        }

        public double PreBookingOutwardQuantityUOM()
        {
            this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation.AutoLoad(this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelationReference, this);
            double sumUOM = 0;
            foreach (ProdOrderPartslistPosRelation childPos in this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation)
            {
                sumUOM += childPos.PreBookingOutwardQuantityUOM();
            }
            this.FacilityPreBooking_ProdOrderPartslistPosRelation.AutoLoad(this.FacilityPreBooking_ProdOrderPartslistPosRelationReference, this);
            foreach (FacilityPreBooking fb in FacilityPreBooking_ProdOrderPartslistPosRelation)
            {
                if (fb.OutwardQuantity.HasValue)
                {
                    try
                    {
                        sumUOM += SourceProdOrderPartslistPos.Material.ConvertToBaseQuantity(fb.OutwardQuantity.Value, SourceProdOrderPartslistPos.MDUnit);
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        this.Root().Messages.LogException(ClassName, "PreBookingOutwardQuantityUOM", msg);
                    }
                }
            }
            return sumUOM;
        }
        #endregion

        #region IPartslistPosRelation implementation
        [NotMapped]
        public IPartslistPos I_SourcePartslistPos
        {
            get { return this.SourceProdOrderPartslistPos; }
        }

        [NotMapped]
        public IPartslistPos I_TargetPartslistPos
        {
            get { return this.TargetProdOrderPartslistPos; }
        }

        #endregion

        #region Overrides
        public override string ToString()
        {
            string toStr = "";
            if(SourceProdOrderPartslistPos != null)
                toStr = SourceProdOrderPartslistPos.ToString();

            toStr += Environment.NewLine +
                " => " + "#" + Sequence.ToString() + " => ";

            if(TargetProdOrderPartslistPos != null)
                toStr += TargetProdOrderPartslistPos.ToString();

            toStr += Environment.NewLine +
                "|" + ProdOrderPartslistPosRelationID.ToString();

            return toStr;
        }
        #endregion
    }
}
