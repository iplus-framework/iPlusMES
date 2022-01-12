using gip.core.datamodel;
using System;
using System.Data.Objects;
using System.Linq;

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
            database.DeleteObject(this);
            return null;
        }

        public void CopyFromParent(ProdOrderPartslistPosRelation parentPos)
        {
            this.MDToleranceState = parentPos.MDToleranceState;
            this.MDProdOrderPartslistPosState = parentPos.MDProdOrderPartslistPosState;
        }

        #endregion

        #region IACObjectEntity Members

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
        public override IACObject ParentACObject
        {
            get
            {
                return SourceProdOrderPartslistPos;
            }
        }

        #endregion

        #region Partial methods

        public ProdOrderPartslistPosRelation TopParentPartslistPosRelation
        {
            get
            {
                if (this.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation != null)
                    return this.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.TopParentPartslistPosRelation;
                return this;
            }
        }

        bool _OnTargetQuantityChanging = false;
        partial void OnTargetQuantityChanged()
        {
            if (!_OnTargetQuantityUOMChanging && SourceProdOrderPartslistPos != null && EntityState != System.Data.EntityState.Detached && SourceProdOrderPartslistPos.Material != null)
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
            OnPropertyChanged("DifferenceQuantity");
        }

        bool _OnTargetQuantityUOMChanging = false;
        partial void OnTargetQuantityUOMChanged()
        {
            if (!_OnTargetQuantityChanging && EntityState != System.Data.EntityState.Detached && SourceProdOrderPartslistPos != null && SourceProdOrderPartslistPos.Material != null)
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
            OnPropertyChanged("DifferenceQuantityUOM");
        }

        bool _OnActualQuantityChanging = false;
        partial void OnActualQuantityChanged()
        {
            if (!_OnActualQuantityUOMChanging && this.SourceProdOrderPartslistPos != null && EntityState != System.Data.EntityState.Detached
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
            OnPropertyChanged("DifferenceQuantity");
        }

        bool _OnActualQuantityUOMChanging = false;
        partial void OnActualQuantityUOMChanged()
        {
            if (!_OnActualQuantityChanging && this.SourceProdOrderPartslistPos != null && EntityState != System.Data.EntityState.Detached
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
            OnPropertyChanged("DifferenceQuantityUOM");
        }

        [ACPropertyInfo(24, "", "en{'Difference Quantity'}de{'Differenzmenge'}")]
        public double DifferenceQuantity
        {
            get
            {
                return ActualQuantity - TargetQuantity;
            }
        }

        [ACPropertyInfo(25, "", "en{'Difference Quantity (UOM)'}de{'Differenzmenge (BME)'}")]
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
        public double RemainingDosingQuantityUOM
        {
            get
            {
                double wasteQ = this.FacilityBooking_ProdOrderPartslistPosRelation.Where(c => c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.Discarded).Sum(c => c.OutwardQuantity);
                //double newDosedQ = this.FacilityBooking_ProdOrderPartslistPosRelation.Where(c => c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New).Sum(c => c.OutwardQuantity);
                return ActualQuantityUOM - (TargetQuantityUOM + wasteQ);
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


        public void RecalcActualQuantity(Nullable<MergeOption> mergeOption = null)
        {
            if (mergeOption.HasValue)
                this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation.Load(mergeOption.Value);
            else
                this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation.AutoLoad();

            double sumActualQuantity = 0;
            double sumActualQuantityUOM = 0;
            foreach (ProdOrderPartslistPosRelation childPos in this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation)
            {
                childPos.RecalcActualQuantity(mergeOption);
                sumActualQuantity += childPos.ActualQuantity;
                sumActualQuantityUOM += childPos.ActualQuantityUOM;
            }

            if (mergeOption.HasValue)
                this.FacilityBooking_ProdOrderPartslistPosRelation.Load(mergeOption.Value);
            else
                this.FacilityBooking_ProdOrderPartslistPosRelation.AutoLoad();
            foreach (FacilityBooking fb in FacilityBooking_ProdOrderPartslistPosRelation)
            {
                foreach (FacilityBookingCharge fbc in fb.FacilityBookingCharge_FacilityBooking)
                {
                    sumActualQuantity += fbc.OutwardQuantity;
                    sumActualQuantityUOM += fbc.OutwardQuantityUOM;
                }
            }
            this.ActualQuantity = sumActualQuantity;
            this.ActualQuantityUOM = sumActualQuantityUOM;
        }

        #endregion

        #region IACUrl member

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
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

        public double PreBookingOutwardQuantityUOM(Nullable<MergeOption> mergeOption = null)
        {
            if (mergeOption.HasValue)
                this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation.Load(mergeOption.Value);
            else
                this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation.AutoLoad();
            double sumUOM = 0;
            foreach (ProdOrderPartslistPosRelation childPos in this.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation)
            {
                sumUOM += childPos.PreBookingOutwardQuantityUOM(mergeOption);
            }

            if (mergeOption.HasValue)
                this.FacilityPreBooking_ProdOrderPartslistPosRelation.Load(mergeOption.Value);
            else
                this.FacilityPreBooking_ProdOrderPartslistPosRelation.AutoLoad();
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
        public IPartslistPos I_SourcePartslistPos
        {
            get { return this.SourceProdOrderPartslistPos; }
        }

        public IPartslistPos I_TargetPartslistPos
        {
            get { return this.TargetProdOrderPartslistPos; }
        }

        #endregion

        #region Overrides
        public override string ToString()
        {
            return
                SourceProdOrderPartslistPos.ToString() +
                Environment.NewLine +
                " => " + "#" + Sequence.ToString() + " => " +
                Environment.NewLine +
                TargetProdOrderPartslistPos.ToString() +
                Environment.NewLine + 
                "|" + ProdOrderPartslistPosRelationID.ToString(); ;

        }
        #endregion
    }
}
