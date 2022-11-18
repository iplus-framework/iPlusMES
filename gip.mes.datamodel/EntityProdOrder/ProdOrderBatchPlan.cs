using System;
using System.Linq;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Batch plan'}de{'Batchplan'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Reihenfolge'}", "", "", true)]
    [ACPropertyEntity(2, ProdOrderPartslist.ClassName, "en{'From BOM'}de{'Von Stückliste'}", Const.ContextDatabase + "\\" + ProdOrderPartslist.ClassName, "", true)]
    [ACPropertyEntity(3, "BatchNoFrom", "en{'From Batch-No.'}de{'Von Batch-Nr.'}", "", "", true)]
    [ACPropertyEntity(4, "BatchNoTo", "en{'To Batch-No.'}de{'Bis Batch-Nr.'}", "", "", true)]
    [ACPropertyEntity(5, "BatchTargetCount", "en{'Target Batch Count'}de{'Soll Batchanzahl'}", "", "", true)]
    [ACPropertyEntity(6, "BatchActualCount", "en{'Actual Batch Count'}de{'Ist Batchanzahl'}", "", "", true)]
    [ACPropertyEntity(7, "BatchSize", "en{'Batch Size'}de{'Batchgröße'}", "", "", true)]
    [ACPropertyEntity(8, "TotalSize", "en{'Total Size'}de{'Gesamtgröße'}", "", "", true)]
    [ACPropertyEntity(10, "PlanModeIndex", "en{'Mode'}de{'Modus'}", typeof(BatchPlanMode), Const.ContextDatabase + "\\BatchPlanModeList", "", true)]
    [ACPropertyEntity(11, "PlanStateIndex", "en{'Status'}de{'Status'}", typeof(GlobalApp.BatchPlanState), Const.ContextDatabase + "\\BatchPlanStateList", "", true)]
    [ACPropertyEntity(12, ProdOrderPartslistPos.ClassName, "en{'From Intermediate'}de{'Von Zwischenprodukt'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName, "", true)]
    [ACPropertyEntity(13, MaterialWFACClassMethod.ClassName, "en{'MaterialWFACClassMethod'}de{'MaterialWFACClassMethod'}", Const.ContextDatabase + "\\" + MaterialWFACClassMethod.ClassName, "", true)]
    [ACPropertyEntity(14, "IsValidated", "en{'Is Validated'}de{'Wurde überprüft'}", "", "", true)]
    [ACPropertyEntity(15, "PlannedStartDate", "en{'Started from UI'}de{'Gestartet von UI'}", "", "", true)]
    [ACPropertyEntity(16, "ScheduledOrder", "en{'Scheduled Order'}de{'Reihenfolge Plan'}", "", "", true)]
    [ACPropertyEntity(17, "ScheduledStartDate", "en{'Planned Start Date'}de{'Geplante Startzeit'}", "", "", true)]
    [ACPropertyEntity(18, "ScheduledEndDate", "en{'Planned End Date'}de{'Geplante Endezeit'}", "", "", true)]
    [ACPropertyEntity(19, "CalculatedStartDate", "en{'Calculated Start Date'}de{'Berechnete Startzeit'}", "", "", true)]
    [ACPropertyEntity(20, "CalculatedEndDate", "en{'Calculated End Date'}de{'Berechnete Endezeit'}", "", "", true)]
    [ACPropertyEntity(21, "PartialTargetCount", "en{'Partial Target Count'}de{'Teil B.-Anzahl'}", "", "", true)]
    [ACPropertyEntity(22, "PartialActualCount", "en{'Partial Actual Count'}de{'Teil Ist B.-Anzahl'}", "", "", true)]
    [ACPropertyEntity(23, "StartOffsetSecAVG", "en{'Starting Batch Offset AVG (s)'}de{'Starting Batch Offset AVG (s)'}", "", "", true)]
    [ACPropertyEntity(24, "DurationSecAVG", "en{'Batch Duration AVG (s)'}de{'Batch Duration AVG (s)'}", "", "", true)]
    [ACPropertyEntity(25, nameof(MDBatchPlanGroup), "en{'Batchplan group'}de{'Batchplan Gruppe'}", Const.ContextDatabase + "\\" + nameof(MDBatchPlanGroup), "", true)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + ProdOrderBatchPlan.ClassName, "en{'ProdOrderBatchPlan'}de{'ProdOrderBatchPlan'}", typeof(ProdOrderBatchPlan), ProdOrderBatchPlan.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<ProdOrderBatchPlan>) })]
    public partial class ProdOrderBatchPlan
    {
        public const string ClassName = "ProdOrderBatchPlan";

        #region New/Delete
        public static ProdOrderBatchPlan NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            ProdOrderBatchPlan entity = new ProdOrderBatchPlan();
            entity.ProdOrderBatchPlanID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.PlanMode = BatchPlanMode.UseBatchCount;
            entity.PlanState = GlobalApp.BatchPlanState.Created;
            entity.IsValidated = false;
            entity.PlannedStartDate = DateTimeUtils.NowDST;
            // TODO setup batchMode
            if (parentACObject is ProdOrderPartslist)
            {
                ProdOrderPartslist prodOrderPartslist = parentACObject as ProdOrderPartslist;
                prodOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.Add(entity);
                entity.ProdOrderPartslist = prodOrderPartslist;
            }
            else if (parentACObject is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos parentProdOrderPos = parentACObject as ProdOrderPartslistPos;
                entity.ProdOrderPartslistPos = parentProdOrderPos;
                entity.ProdOrderPartslist = parentProdOrderPos.ProdOrderPartslist;
                parentProdOrderPos.ProdOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.Add(entity);
                parentProdOrderPos.ProdOrderBatchPlan_ProdOrderPartslistPos.Add(entity);
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
            var reservations = FacilityReservation_ProdOrderBatchPlan.ToList();
            foreach (var reservation in reservations)
            {
                FacilityReservation_ProdOrderBatchPlan.Remove(reservation);
            }
            int sequence = Sequence;
            database.DeleteObject(this);
            return null;
        }

        #endregion

        #region IACUrl Member

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
        /// Returns ProdOrderPartslist
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to ProdOrderPartslist</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return ProdOrderPartslist;
            }
        }

        #endregion

        #region IACObjectEntity Members
        //public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        //{
        //    if (Material == null)
        //    {
        //        List<Msg> messages = new List<Msg>();
        //        messages.Add(new Msg
        //        {
        //            Source = GetACUrl(),
        //            ACIdentifier = "Key",
        //            Message = "Key",
        //            //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "Key"), 
        //            MessageLevel = eMsgLevel.Error
        //        });
        //        return messages;
        //    }
        //    base.EntityCheckAdded(user, context);
        //    return null;
        //}

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

        #region Properties

        [ACPropertyInfo(999, "", "en{'Mode'}de{'Modus'}", Const.ContextDatabase + "\\BatchPlanModeList")]
        public BatchPlanMode PlanMode
        {
            get
            {
                return (BatchPlanMode)PlanModeIndex;
            }
            set
            {
                PlanModeIndex = (Int16)value;
                OnPropertyChanged("PlanMode");
            }
        }

        public GlobalApp.BatchPlanState PlanState
        {
            get
            {
                return (GlobalApp.BatchPlanState)PlanStateIndex;
            }
            set
            {
                PlanStateIndex = (Int16)value;
                OnPropertyChanged("PlanState");
            }
        }

        [ACPropertyInfo(999, "", "en{'State'}de{'Status'}")]
        public string PlanStateIndexName
        {
            get
            {
                ACValueItem acValueItem = GlobalApp.BatchPlanStateList.FirstOrDefault(c => ((short)c.Value) == PlanStateIndex);
                return acValueItem.ACCaption;
            }
        }



        [ACPropertyInfo(999, "", "en{'Yield [%]'}de{'Ausbeute [%]'}")]
        public double YieldPerc
        {
            get
            {
                return Yield * 100;
            }
        }

        [ACPropertyInfo(999, "", "en{'Yield'}de{'Ausbeute'}")]
        public double Yield
        {
            get
            {
                ProdOrderPartslist partsList = null;
                if (ProdOrderPartslistPos != null)
                    partsList = ProdOrderPartslistPos.ProdOrderPartslist;
                if (partsList != null && this.ProdOrderPartslist != null)
                    partsList = this.ProdOrderPartslist;
                if (partsList != null)
                {
                    double yield = partsList.TargetQuantityLossFactor;
                    if (yield < 0.0000001 || yield > 100)
                        yield = 1;
                    return yield;
                }
                return 1;
            }
        }

        [ACPropertyInfo(999, "", "en{'Effective batch size'}de{'Effektive Batchgröße'}")]
        public double YieldBatchSize
        {
            get
            {
                if (this.BatchSize == 0)
                    return 0;
                return this.BatchSize * Yield;
            }
        }

        [ACPropertyInfo(999, "", "en{'Effective total size'}de{'Effektive Gesamtgröße'}")]
        public double YieldTotalSize
        {
            get
            {
                if (this.TotalSize == 0)
                    return 0;
                return this.TotalSize * Yield;
            }
        }

        private double _YieldTotalSizeExpected;
        [ACPropertyInfo(999, "", "en{'Expected Eff. total size'}de{'Erwartete eff. Gesamtgröße'}")]
        public double YieldTotalSizeExpected
        {
            get
            {
                return _YieldTotalSizeExpected;
            }
            set
            {
                if (_YieldTotalSizeExpected != value)
                {
                    _YieldTotalSizeExpected = value;
                    if (BatchSize >= 0.000001 && Yield >= 0.000001 && PlanMode == BatchPlanMode.UseBatchCount)
                    {
                        BatchTargetCount = (int)(_YieldTotalSizeExpected / (Yield * BatchSize));
                        if (Math.Abs(_YieldTotalSizeExpected - YieldTotalSize) > Double.Epsilon && _YieldTotalSizeExpected > YieldTotalSize)
                            BatchTargetCount++;
                    }
                    OnPropertyChanged("YieldTotalSizeExpected");
                }
            }
        }

        private gip.core.datamodel.ACClassWF _IplusVBiACClassWF;
        [ACPropertyInfo(9999)]
        public gip.core.datamodel.ACClassWF IplusVBiACClassWF
        {
            get
            {
                if (VBiACClassWFID == null || VBiACClassWFID == Guid.Empty)
                    return null;
                if (this._IplusVBiACClassWF == null)
                {
                    DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                    using (ACMonitor.Lock(dbApp.ContextIPlus.QueryLock_1X000))
                    {
                        _IplusVBiACClassWF = dbApp.ContextIPlus.ACClassWF.Where(c => c.ACClassWFID == VBiACClassWFID).FirstOrDefault();
                    }
                }
                return _IplusVBiACClassWF;
            }
        }

        private bool _IsSelected;
        [ACPropertyInfo(999, nameof(IsSelected), ConstApp.Select)]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        /// <summary>
        /// Returns PartialTargetCount - PartialActualCount if set.
        /// Otherwise null.
        /// If Value is lower or eaqual 0 than PartialActualCount has reached the PartialTargetCount
        /// </summary>
        /// <value>
        /// The difference partial count.
        /// </value>
        public int? DiffPartialCount
        {
            get
            {
                if (!PartialTargetCount.HasValue)
                    return null;
                return PartialActualCount.HasValue ? PartialTargetCount - PartialActualCount : PartialTargetCount;
            }
        }


        private string _MatNameWithFinalProductName;
        [ACPropertyInfo(999, "MatNameWithFinalProductName", ConstApp.MaterialName1)]
        public string MatNameWithFinalProductName
        {
            get
            {
                if (string.IsNullOrEmpty(_MatNameWithFinalProductName))
                    _MatNameWithFinalProductName = GetMatNameWithFinalProductName();
                return _MatNameWithFinalProductName;
            }
            set
            {
                if (_MatNameWithFinalProductName != value)
                {
                    _MatNameWithFinalProductName = value;
                    OnPropertyChanged("MatNameWithFinalProductName");
                }
            }
        }
        #endregion

        #region Partial methods

        bool _OnBatchBatchNoFromChanging = false;
        partial void OnBatchNoFromChanged()
        {
            if (EntityState == System.Data.EntityState.Detached)
                return;
            if (this.PlanMode != BatchPlanMode.UseFromTo)
                return;
            if (_OnTotalSizeChanging || _OnBatchTargetCountChanging || _OnPlanModeIndexChanging)
                return;
            _OnBatchBatchNoFromChanging = true;
            try
            {
                if (this.BatchNoFrom.HasValue && this.BatchNoTo.HasValue && this.BatchNoFrom.Value < this.BatchNoTo.Value)
                {
                    this.TotalSize = (this.BatchNoTo.Value - this.BatchNoFrom.Value) * this.BatchSize;
                }
                else
                {
                    this.TotalSize = 0;
                }
                OnPropertyChanged("TotalSizeAltUOM");
                OnPropertyChanged("BatchSizeAltUOM");
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                this.Root().Messages.LogException(ClassName, "OnBatchNoChanged", msg);
            }
            finally
            {
                _OnBatchBatchNoFromChanging = false;
            }
        }

        bool _OnBatchBatchNoToChanging = false;
        partial void OnBatchNoToChanged()
        {
            if (EntityState == System.Data.EntityState.Detached)
                return;
            if (this.PlanMode != BatchPlanMode.UseFromTo)
                return;
            if (_OnTotalSizeChanging || _OnBatchTargetCountChanging || _OnPlanModeIndexChanging)
                return;
            _OnBatchBatchNoToChanging = true;
            try
            {
                if (this.BatchNoFrom.HasValue && this.BatchNoTo.HasValue && this.BatchNoFrom.Value < this.BatchNoTo.Value)
                {
                    this.TotalSize = (this.BatchNoTo.Value - this.BatchNoFrom.Value) * this.BatchSize;
                }
                else
                {
                    this.TotalSize = 0;
                }
                OnPropertyChanged("TotalSizeAltUOM");
                OnPropertyChanged("BatchSizeAltUOM");
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                this.Root().Messages.LogException(ClassName, "OnBatchNoToChanged", msg);
            }
            finally
            {
                _OnBatchBatchNoToChanging = false;
            }
        }

        bool _OnBatchTargetCountChanging = false;
        partial void OnBatchTargetCountChanged()
        {
            if (EntityState == System.Data.EntityState.Detached)
                return;
            if (this.PlanMode != BatchPlanMode.UseBatchCount)
                return;
            if (_OnTotalSizeChanging || _OnTotalSizeChanging || _OnBatchSizeChanging || _OnPlanModeIndexChanging)
                return;
            _OnBatchTargetCountChanging = true;
            try
            {
                this.TotalSize = this.BatchTargetCount * this.BatchSize;
                OnPropertyChanged("TotalSizeAltUOM");
                OnPropertyChanged("BatchSizeAltUOM");
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                this.Root().Messages.LogException(ClassName, "OnBatchTargetCountChanged", msg);
            }
            finally
            {
                _OnBatchTargetCountChanging = false;
            }
        }

        bool _OnBatchSizeChanging = false;
        partial void OnBatchSizeChanged()
        {
            if (EntityState == System.Data.EntityState.Detached)
                return;
            if (this.PlanMode == BatchPlanMode.UseTotalSize)
                return;
            if (_OnTotalSizeChanging || _OnPlanModeIndexChanging)
                return;
            _OnBatchSizeChanging = true;
            try
            {
                this.IsValidated = false;
                if (this.PlanMode == BatchPlanMode.UseBatchCount)
                {
                    this.TotalSize = this.BatchTargetCount * this.BatchSize;
                }
                else if (this.PlanMode == BatchPlanMode.UseFromTo)
                {
                    if (this.BatchNoFrom.HasValue && this.BatchNoTo.HasValue && this.BatchNoFrom.Value < this.BatchNoTo.Value)
                    {
                        this.TotalSize = (this.BatchNoTo.Value - this.BatchNoFrom.Value) * this.BatchSize;
                    }
                    else
                    {
                        this.TotalSize = 0;
                    }
                }
                OnPropertyChanged("TotalSizeAltUOM");
                OnPropertyChanged("BatchSizeAltUOM");
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                this.Root().Messages.LogException(ClassName, "OnBatchSizeChanged", msg);
            }
            finally
            {
                _OnBatchSizeChanging = false;
            }
        }

        bool _OnTotalSizeChanging = false;
        partial void OnTotalSizeChanged()
        {
            if (EntityState == System.Data.EntityState.Detached)
                return;
            OnPropertyChanged("RemainingQuantity");
            OnPropertyChanged("DifferenceQuantity");
            OnPropertyChanged("YieldBatchSize");
            OnPropertyChanged("YieldTotalSize");
            if (this.PlanMode != BatchPlanMode.UseTotalSize)
                return;
            if (_OnBatchSizeChanging || _OnBatchTargetCountChanging || _OnBatchBatchNoToChanging || _OnBatchBatchNoFromChanging || _OnPlanModeIndexChanging)
                return;
            _OnTotalSizeChanging = true;
            this.IsValidated = false;
            this.BatchNoFrom = null;
            this.BatchNoTo = null;
            this.BatchSize = 0;
            this.BatchTargetCount = 0;
            _OnTotalSizeChanging = false;
        }


        bool _OnPlanModeIndexChanging = false;
        BatchPlanMode _PreviousMode = BatchPlanMode.UseBatchCount;
        partial void OnPlanModeIndexChanging(global::System.Int16 value)
        {
            if (EntityState == System.Data.EntityState.Detached)
                return;
            _PreviousMode = this.PlanMode;
        }

        partial void OnPlanModeIndexChanged()
        {
            if (EntityState == System.Data.EntityState.Detached)
                return;
            _OnPlanModeIndexChanging = true;
            try
            {
                if (this.PlanMode == BatchPlanMode.UseTotalSize)
                {
                    this.BatchNoFrom = null;
                    this.BatchNoTo = null;
                    this.BatchSize = 0;
                    this.BatchTargetCount = 0;
                }
                else if (this.PlanMode == BatchPlanMode.UseBatchCount)
                {
                    int nBatchCount = this.BatchTargetCount;
                    if (this.BatchNoFrom.HasValue && this.BatchNoTo.HasValue && this.BatchNoFrom.Value < this.BatchNoTo.Value)
                        nBatchCount = this.BatchNoTo.Value - this.BatchNoFrom.Value;
                    this.BatchTargetCount = nBatchCount;
                    this.TotalSize = this.BatchTargetCount * this.BatchSize;
                    this.BatchNoFrom = null;
                    this.BatchNoTo = null;
                }
                else //if (this.PlanMode == BatchPlanMode.UseFromTo)
                {
                    if (this.BatchTargetCount > 0)
                    {
                        this.BatchNoFrom = 1;
                        this.BatchNoTo = BatchTargetCount + 1;
                        this.TotalSize = this.BatchTargetCount * this.BatchSize;
                        this.BatchTargetCount = 0;
                    }
                    else
                    {
                        this.BatchNoFrom = null;
                        this.BatchNoTo = null;
                        this.TotalSize = 0;
                        this.BatchTargetCount = 0;
                    }
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                this.Root().Messages.LogException(ClassName, "OnPlanModeIndexChanged", msg);
            }
            finally
            {
                _OnPlanModeIndexChanging = false;
            }
        }

        partial void OnPlanStateIndexChanged()
        {
            OnPropertyChanged("PlanState");
            OnPropertyChanged("PlanStateIndexName");
        }

        [ACPropertyInfo(15, "", "en{'Called-up Quantity'}de{'Abgerufene Menge'}")]
        public double CalledUpQuantity
        {
            get
            {
                if (this.ProdOrderPartslistPos != null)
                {
                    return this.ProdOrderPartslistPos.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                        .Where(c => c.ProdOrderBatch != null
                            && c.ProdOrderBatch.ProdOrderBatchPlanID.HasValue
                            && c.ProdOrderBatch.ProdOrderBatchPlanID == this.ProdOrderBatchPlanID)
                        .Select(c => c.TargetQuantity)
                        .Sum();
                }
                return 0;
            }
        }

        //public double CalledUpQuantityUOM
        //{
        //    get
        //    {
        //        if (this.ProdOrderPartslistPos != null)
        //        {
        //            return this.ProdOrderPartslistPos.ProdOrderPartslistPos_ParentProdOrderPartslistPos
        //                .Where(c => c.ProdOrderBatch != null
        //                    && c.ProdOrderBatch.ProdOrderBatchPlanID.HasValue
        //                    && c.ProdOrderBatch.ProdOrderBatchPlanID == this.ProdOrderBatchPlanID)
        //                .Select(c => c.TargetQuantityUOM)
        //                .Sum();
        //        }
        //        return 0;
        //    }
        //}

        [ACPropertyInfo(16, "", "en{'Actual Quantity'}de{'Istmenge'}")]
        public double ActualQuantity
        {
            get
            {
                if (this.ProdOrderPartslistPos != null)
                {
                    return this.ProdOrderPartslistPos.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                        .Where(c => c.ProdOrderBatch != null
                            && c.ProdOrderBatch.ProdOrderBatchPlanID.HasValue
                            && c.ProdOrderBatch.ProdOrderBatchPlanID == this.ProdOrderBatchPlanID)
                        .Sum(c => c.ActualQuantity);
                }
                return 0;
            }
        }


        [ACPropertyInfo(17, "", "en{'Remaining Quantity'}de{'Restmenge'}")]
        public double RemainingQuantity
        {
            get
            {
                return TotalSize - CalledUpQuantity;
            }
        }

        [ACPropertyInfo(18, "", "en{'Difference quantity'}de{'Differenzmenge'}")]
        public double DifferenceQuantity
        {
            get
            {
                return ActualQuantity - TotalSize;
            }
        }

        public bool IsValid
        {
            get
            {
                return !((PlanMode == BatchPlanMode.UseBatchCount && (BatchTargetCount <= 0 || BatchSize <= 0.001))
                        || (PlanMode == BatchPlanMode.UseFromTo && (BatchNoFrom <= 0 || BatchNoTo <= 0 || BatchSize <= 0.001))
                        || (PlanMode == BatchPlanMode.UseTotalSize && (TotalSize <= 0.001)));
            }
        }

        [ACPropertyInfo(17, "", "en{'Batch-S. UOM alt.'}de{'Batchgr. ME alt.'}")]
        public double? BatchSizeAltUOM
        {
            get
            {
                return ConvertFromBaseUnit(BatchSize);
            }
        }

        [ACPropertyInfo(18, "", "en{'Total-S. UOM alt.'}de{'Gesamtgr. ME alt.'}")]
        public double? TotalSizeAltUOM
        {
            get
            {
                return ConvertFromBaseUnit(TotalSize);
            }
        }

        public MaterialUnit FirstAltMatUnit
        {
            get
            {
                if (ProdOrderPartslist == null
                    || ProdOrderPartslist.Partslist == null
                    || ProdOrderPartslist.Partslist.Material == null)
                    return null;
                return ProdOrderPartslist.Partslist.Material.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).FirstOrDefault();
            }
        }

        [ACPropertyInfo(19, "", "en{'UOM alt.'}de{'ME Alt.'}")]
        public MDUnit FirstAltUOM
        {
            get
            {
                var altMatUnit = FirstAltMatUnit;
                if (altMatUnit != null)
                    return altMatUnit.ToMDUnit;
                return null;
            }
        }

        public double? ConvertFromBaseUnit(double value)
        {
            var altMatUnit = FirstAltMatUnit;
            if (altMatUnit == null)
                return null;
            return altMatUnit.FromBaseToUnit(value);
        }

        public string GetMatNameWithFinalProductName()
        {
            string name = "";
            name = ProdOrderPartslist.Partslist.Material.MaterialName1.Trim();
            ProdOrderPartslist pl = ProdOrderPartslist.ProdOrder.ProdOrderPartslist_ProdOrder.OrderByDescending(c => c.Sequence).FirstOrDefault();
            if (pl.ProdOrderPartslistID != ProdOrderPartslist.ProdOrderPartslistID)
            {
                name += Environment.NewLine
                    + "("
                    + pl.Partslist.Material.MaterialName1.Trim()
                    + ")";
            }
            return name;
        }

        #endregion

    }
}
