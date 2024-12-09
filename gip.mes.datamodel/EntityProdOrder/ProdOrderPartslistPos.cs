using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Production Component'}de{'Produktionskomponente'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Reihenfolge'}", "", "", true)]
    [ACPropertyEntity(2, "SequenceProduction", "en{'Production Sequence'}de{'Produktionsreihenfolge'}", "", "", true)]
    [ACPropertyEntity(3, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(4, MDUnit.ClassName, "en{'Unit of Measurement'}de{'Maßeinheit'}", Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(5, "TargetQuantityUOM", "en{'Target Quantity (UOM)'}de{'Sollmenge (BME)'}", "", "", true)]
    [ACPropertyEntity(6, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(7, "ActualQuantityUOM", "en{'Actual Quantity (UOM)'}de{'Istmenge (BME)'}", "", "", true)]
    [ACPropertyEntity(8, "ActualQuantity", "en{'Actual Quantity'}de{'Istmenge'}", "", "", true)]
    [ACPropertyEntity(9, "CalledUpQuantityUOM", "en{'Called up Quantity (UOM)'}de{'Abgerufene Menge (BME)'}", "", "", true)]
    [ACPropertyEntity(10, "CalledUpQuantity", "en{'Called up Quantity'}de{'Abgerufene Menge'}", "", "", true)]
    [ACPropertyEntity(12, MDToleranceState.ClassName, "en{'Tolernace state'}de{'Toleranzstatus'}", Const.ContextDatabase + "\\" + MDToleranceState.ClassName, "", true)]
    [ACPropertyEntity(13, "BasedOnPartslistPos", "en{'From BOM line'}de{'Von Stücklistenposition'}", Const.ContextDatabase + "\\" + PartslistPos.ClassName, "", true)]
    [ACPropertyEntity(13, "IsBaseQuantityExcluded", "en{'Excluded from total Quantity (Rework)'}de{'Nicht in Gesamtmenge enthalten (Rework)'}", "", "", true)]
    [ACPropertyEntity(15, "ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos", "en{'Alternative Line from'}de{'Alternativposition von'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName, "", true)]
    [ACPropertyEntity(16, "ProdOrderPartslistPos1_ParentProdOrderPartslistPos", "en{'Subline from'}de{'Unterposition von'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName, "", true)]
    [ACPropertyEntity(18, "LineNumber", "en{'Line Number'}de{'Positionsnummer'}", "", "", true)]
    [ACPropertyEntity(19, MDProdOrderPartslistPosState.ClassName, "en{'Status'}de{'Status'}", Const.ContextDatabase + "\\" + MDProdOrderPartslistPosState.ClassName, "", true)]
    [ACPropertyEntity(20, ProdOrderPartslist.ClassName, "en{'Order BOM'}de{'Auftragsbezogene Stückliste'}", Const.ContextDatabase + "\\" + ProdOrderPartslist.ClassName, "", true)]
    [ACPropertyEntity(20, "Source" + ProdOrderPartslist.ClassName, "en{'Produced from BOM'}de{'Hergestellt aus Stückliste'}", Const.ContextDatabase + "\\" + ProdOrderPartslist.ClassName, "", true)]
    [ACPropertyEntity(21, ProdOrderBatch.ClassName, "en{'Batch'}de{'Batch'}", Const.ContextDatabase + "\\" + ProdOrderBatch.ClassName, "", true)]
    [ACPropertyEntity(22, FacilityLot.ClassName, "en{'Lot/Charge'}de{'Los/Charge'}", Const.ContextDatabase + "\\" + FacilityLot.ClassName, "", true)]
    [ACPropertyEntity(23, "TakeMatFromOtherOrder", "en{'Take material from other order'}de{'Entnahme von anderem Auftrag erlaubt'}", "", "", true)]
    [ACPropertyEntity(24, "RetrogradeFIFO", "en{'Backflushing'}de{'Retrograde Entnahme'}", "", "", true)]
    [ACPropertyEntity(25, "Anterograde", "en{'Anterograde inward posting'}de{'Anterograde Zugangsbuchung'}", "", "", true)]
    [ACPropertyEntity(26, nameof(InputQForActualOutput), ConstIInputQForActual.InputQForActualOutput, "", "", true)]
    [ACPropertyEntity(27, nameof(InputQForGoodActualOutput), ConstIInputQForActual.InputQForGoodActualOutput, "", "", true)]
    [ACPropertyEntity(28, nameof(InputQForScrapActualOutput), ConstIInputQForActual.InputQForScrapActualOutput, "", "", true)]
    [ACPropertyEntity(29, nameof(InputQForFinalActualOutput), ConstIInputQForActual.InputQForFinalActualOutput, "", "", true)]
    [ACPropertyEntity(30, nameof(InputQForFinalGoodActualOutput), ConstIInputQForActual.InputQForFinalGoodActualOutput, "", "", true)]
    [ACPropertyEntity(31, nameof(InputQForFinalScrapActualOutput), ConstIInputQForActual.InputQForFinalScrapActualOutput, "", "", true)]

    [ACPropertyEntity(9999, "MaterialPosTypeIndex", "en{'Position Type'}de{'Positionstyp'}", typeof(GlobalApp.MaterialPosTypes), "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + ProdOrderPartslistPos.ClassName, "en{'Production Component'}de{'Produktionskomponente'}", typeof(ProdOrderPartslistPos), ProdOrderPartslistPos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<ProdOrderPartslistPos>) })]
    public partial class ProdOrderPartslistPos : IPartslistPos
    {
        public const string ClassName = "ProdOrderPartslistPos";

        #region New/Delete
        public static ProdOrderPartslistPos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            ProdOrderPartslistPos entity = new ProdOrderPartslistPos();
            entity.ProdOrderPartslistPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDProdOrderPartslistPosState = MDProdOrderPartslistPosState.DefaultMDProdOrderPartslistPosState(dbApp);
            entity.MDToleranceState = MDToleranceState.DefaultMDToleranceState(dbApp);
            if (parentACObject is ProdOrderPartslist)
            {
                ProdOrderPartslist prodOrderPartslist = parentACObject as ProdOrderPartslist;
                prodOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist.Add(entity);
                entity.MaterialPosTypeIndex = (Int16)GlobalApp.MaterialPosTypes.OutwardRoot;
            }
            else if (parentACObject is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos parentPos = parentACObject as ProdOrderPartslistPos;
                entity.ProdOrderPartslistPos1_ParentProdOrderPartslistPos = parentPos;
                entity.CopyFromParent(parentPos);
                entity.ProdOrderPartslist = parentPos.ProdOrderPartslist;
                entity.MaterialPosType = GlobalApp.MaterialPosTypes.InwardPartIntern;
                parentPos.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Add(entity);
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

        public void CopyFromParent(ProdOrderPartslistPos parentPos)
        {
            this.MDUnit = parentPos.MDUnit;
            this.Material = parentPos.Material;
        }

        public static ProdOrderPartslistPos NewAlternativeProdOrderPartslistPos(DatabaseApp DatabaseApp, IACObject parentACObject, ProdOrderPartslistPos CurrentProdOrderPartslistPos)
        {
            ProdOrderPartslistPos entity = ProdOrderPartslistPos.NewACObject(DatabaseApp, parentACObject);
            entity.ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos = CurrentProdOrderPartslistPos;
            return entity;
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
                if (Material == null)
                    return Sequence.ToString();
                return Sequence.ToString() + " " + Material.ACCaption;
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


        static public string KeyACIdentifier
        {
            get
            {
                return "Sequence,Material\\MaterialNo,MaterialPosTypeIndex";
            }
        }


        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && filterValues[0] != null)
            {
                string[] filterParams = filterValues[0].Split(',');
                switch (className)
                {
                    case ProdOrderPartslistPosRelation.ClassName:
                        //Sequence,TargetProdOrderPartslistPos\\Sequence,TargetProdOrderPartslistPos\\Material\\MaterialNo,TargetProdOrderPartslistPos\\MaterialPosTypeIndex
                        int sequence = int.Parse(filterParams[0]);
                        int targetPosSequence = int.Parse(filterParams[1]);
                        short materialPosTypeIndex = short.Parse(filterParams[3]);
                        return
                            ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                            .Where(c =>
                               c.Sequence == sequence
                               && c.TargetProdOrderPartslistPos.Sequence == targetPosSequence
                               && c.TargetProdOrderPartslistPos.Material.MaterialNo == filterParams[2]
                               && c.TargetProdOrderPartslistPos.MaterialPosTypeIndex == materialPosTypeIndex

                               )
                             .FirstOrDefault();
                }
            }

            return null;
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

        #region Alternative Positionen
        [ACPropertyInfo(9999, "", "en{'Alternative'}de{'Alternative'}")]
        public bool IsAlternative
        {
            get
            {
                return ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos != null;
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Define is material finall product (final mixure)
        /// </summary>
        [ACPropertyInfo(9999)]
        public bool IsFinalMixure
        {
            get
            {
                return this.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                    && !ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any();
                //&& Material != null
                //&& !this.Material.MaterialWFRelation_SourceMaterial.Where(c => c.SourceMaterialID != c.TargetMaterialID).Any();
            }
        }

        /// <summary>
        /// Check is this batch for final product
        /// </summary>
        [ACPropertyInfo(9999)]
        public bool IsFinalMixureBatch
        {
            get
            {
                return
                    this.MaterialPosType == GlobalApp.MaterialPosTypes.InwardPartIntern
                    && ParentProdOrderPartslistPosID != null
                    && ProdOrderPartslistPos1_ParentProdOrderPartslistPos.IsFinalMixure;
            }
        }

        [ACPropertyInfo(9999)]
        public Material BookingMaterial
        {
            get
            {
                if (IsFinalMixure || IsFinalMixureBatch)
                    return this.ProdOrderPartslist.Partslist.Material;
                else
                    return this.Material;
            }
        }

        public string MaterialNo
        {
            get
            {
                string caption = "";
                if (Material != null)
                    caption = Material.MaterialNo;
                return caption;
            }
        }

        [ACPropertyInfo(9999)]
        public string MaterialName
        {
            get
            {
                string caption = "";
                if (Material != null)
                    caption = Material.MaterialName1;
                return caption;
            }
        }

        public string PosCaption
        {
            get
            {
                return "";
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

        /// <summary>
        /// Used for display rest of quantity in production order editor
        /// </summary>
        [ACPropertyInfo(9999)]
        public double RestQuantity { get; set; }

        [ACPropertyInfo(9999)]
        public double RestQuantityUOM { get; set; }

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


        /// <summary>
        /// Test is current pos OutwardRoot type - for enabling in Prod Order editor
        /// </summary>
        [ACPropertyInfo(9999)]
        public bool IsOutwardRoot
        {
            get
            {
                return MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot;
            }
        }

        public double BatchFraction
        {
            get
            {
                if (!ParentProdOrderPartslistPosID.HasValue)
                    return 0;
                return this.TargetQuantityUOM / ProdOrderPartslistPos1_ParentProdOrderPartslistPos.TargetQuantityUOM;
            }
        }


        private int _PositionUsedCount;
        [ACPropertyInfo(9999)]
        public int PositionUsedCount
        {
            get
            {
                return _PositionUsedCount;
            }
            set
            {
                if (_PositionUsedCount != value)
                {
                    _PositionUsedCount = value;
                    OnPropertyChanged("PositionUsedCount");
                }
            }
        }

        /// <summary>
        /// Property that evaluates the override of the RetrogradeFIFO-Fields in Tables ProdOrderPartslistPos->BasedOnPartslistPos->Material
        /// </summary>
        public bool Backflushing
        {
            get
            {
                if (this.RetrogradeFIFO.HasValue)
                    return this.RetrogradeFIFO.Value;
                else if (ProdOrderPartslistPos1_ParentProdOrderPartslistPos != null)
                    return ProdOrderPartslistPos1_ParentProdOrderPartslistPos.Backflushing;
                else if (BasedOnPartslistPosID.HasValue && BasedOnPartslistPos != null)
                    return BasedOnPartslistPos.Backflushing;
                else if (MaterialID.HasValue && Material != null && Material.RetrogradeFIFO.HasValue)
                    return Material.RetrogradeFIFO.Value;
                return false;
            }
        }


        /// <summary>
        /// Property that evaluates the override of the Anterograde-Fields in Tables ProdOrderPartslistPos->BasedOnPartslistPos->Material
        /// </summary>
        public bool Foreflushing
        {
            get
            {
                if (this.Anterograde.HasValue)
                    return this.Anterograde.Value;
                else if (ProdOrderPartslistPos1_ParentProdOrderPartslistPos != null)
                    return ProdOrderPartslistPos1_ParentProdOrderPartslistPos.Foreflushing;
                else if (BasedOnPartslistPosID.HasValue && BasedOnPartslistPos != null)
                    return BasedOnPartslistPos.Foreflushing;
                else if (MaterialID.HasValue && Material != null && Material.Anterograde.HasValue)
                    return Material.Anterograde.Value;
                return false;
            }
        }

        //[ACPropertyEntity(25, "Anterograde", "en{'Anterograde inward posting'}de{'Anterograde Zugangsbuchung'}", "", "", true)]

        public ProdOrderPartslist _FinalProdOrderPartslist;
        public ProdOrderPartslist FinalProdOrderPartslist
        {
            get
            {
                if (_FinalProdOrderPartslist == null)
                {
                    if (ProdOrderPartslist.ProdOrderPartslistPos_SourceProdOrderPartslist.Any())
                    {
                        _FinalProdOrderPartslist = ProdOrderPartslist.ProdOrderPartslistPos_SourceProdOrderPartslist.FirstOrDefault().FinalProdOrderPartslist;
                    }
                    else
                    {
                        _FinalProdOrderPartslist = ProdOrderPartslist;
                    }
                }
                return _FinalProdOrderPartslist;
            }
        }

        #endregion

        #region Partial methods
        public ProdOrderPartslistPos TopParentPartslistPos
        {
            get
            {
                if (this.ProdOrderPartslistPos1_ParentProdOrderPartslistPos != null)
                    return this.ProdOrderPartslistPos1_ParentProdOrderPartslistPos.TopParentPartslistPos;
                return this;
            }
        }

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

                    this.Root().Messages.LogException(ClassName, "OnCalledUpQuantityChanged", msg);
                }
                finally
                {
                    _OnCalledUpQuantityChanging = false;
                }
            }
            OnPropertyChanged("RemainingCallQuantity");
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

                    this.Root().Messages.LogException(ClassName, "OnCalledUpQuantityUOMChanged", msg);
                }
                finally
                {
                    _OnCalledUpQuantityUOMChanging = false;
                }
            }
            OnPropertyChanged("RemainingCallQuantityUOM");
        }

        bool _OnTargetQuantityChanging = false;
        partial void OnTargetQuantityChanged()
        {
            if (!_OnTargetQuantityUOMChanging && EntityState != System.Data.EntityState.Detached && Material != null)
            {
                _OnTargetQuantityChanging = true;
                try
                {
                    if (MDUnit != null && Material.BaseMDUnit != MDUnit)
                    {
                        TargetQuantityUOM = Material.ConvertToBaseQuantity(TargetQuantity, MDUnit);
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
            if (this.ProdOrderPartslist != null)
                this.ProdOrderPartslist.OnPostionTargetQuantityChanged();
            OnPropertyChanged("RemainingCallQuantity");
        }

        bool _OnTargetQuantityUOMChanging = false;
        partial void OnTargetQuantityUOMChanged()
        {
            if (!_OnTargetQuantityChanging && EntityState != System.Data.EntityState.Detached && Material != null)
            {
                _OnTargetQuantityUOMChanging = true;
                try
                {
                    if (MDUnit != null && MDUnit != Material.BaseMDUnit)
                    {
                        TargetQuantity = Material.ConvertQuantity(TargetQuantityUOM, Material.BaseMDUnit, MDUnit);
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
            OnPropertyChanged(nameof(DifferenceQuantityUOM));
            OnPropertyChanged(nameof(RemainingCallQuantityUOM));
            OnPropertyChanged(nameof(DifferenceQuantityPer));
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

                    this.Root().Messages.LogException(ClassName, "OnBatchTargetCountChanged", msg);
                }
                finally
                {
                    _OnActualQuantityChanging = false;
                }
            }
            OnPropertyChanged(nameof(DifferenceQuantity));
            OnPropertyChanged(nameof(DifferenceQuantityPer));
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

                    this.Root().Messages.LogException(ClassName, "OnActualQuantityUOMChanged", msg);
                }
                finally
                {
                    _OnActualQuantityUOMChanging = false;
                }
            }
            OnPropertyChanged(nameof(DifferenceQuantityUOM));
            OnPropertyChanged(nameof(DifferenceQuantityPer));
        }

        private bool _OnMDUnitChanging = false;
        partial void OnMDUnitIDChanging(Nullable<global::System.Guid> value)
        {
            if (_MDUnitID != value && EntityState != System.Data.EntityState.Detached)
                _OnMDUnitChanging = true;
        }

        partial void OnMDUnitIDChanged()
        {
            _OnTargetQuantityUOMChanging = true;
            _OnActualQuantityUOMChanging = true;
            try
            {
                if (_OnMDUnitChanging && MDUnit != null && Material != null)
                {
                    TargetQuantity = Material.ConvertQuantity(TargetQuantityUOM, Material.BaseMDUnit, MDUnit);
                    ActualQuantity = Material.ConvertQuantity(ActualQuantityUOM, Material.BaseMDUnit, MDUnit);
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                this.Root().Messages.LogException(ClassName, "OnMDUnitIDChanged", msg);
            }
            finally
            {
                _OnMDUnitChanging = false;
                _OnTargetQuantityUOMChanging = false;
                _OnActualQuantityUOMChanging = false;
            }
        }

        partial void OnTargetQuantityUOMChanging(double value)
        {
            if (InappropriateComponentQuantityOccurrence.IsForAnalyse(TargetQuantityUOM, value))
            {
                bool isInappropriate = InappropriateComponentQuantityOccurrence.IsInappropriate(this);
                if (isInappropriate)
                {
                    InappropriateComponentQuantityOccurrence.WriteStackTrace(this);
                }
            }
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

        public void IncreaseActualQuantityUOM(double quantityUOM, bool autoRefresh = false)
        {
            IncreaseActualQuantityUOM2(quantityUOM, autoRefresh, 0);
        }

        private void IncreaseActualQuantityUOM2(double quantityUOM, bool autoRefresh, short recCounter)
        {
            this.ActualQuantityUOM += quantityUOM;
            if (recCounter == 0 && (this.IsFinalMixure || this.IsFinalMixureBatch))
                this.ProdOrderPartslist.ActualQuantity += quantityUOM;
            recCounter++;
            if (this.ProdOrderPartslistPos1_ParentProdOrderPartslistPos != null)
            {
                if (autoRefresh)
                    this.ProdOrderPartslistPos1_ParentProdOrderPartslistPos.AutoRefresh();
                this.ProdOrderPartslistPos1_ParentProdOrderPartslistPos.IncreaseActualQuantityUOM2(quantityUOM, autoRefresh, recCounter);
            }
        }

        public void IncreaseActualQuantity(double quantity, MDUnit mdUnit, bool autoRefresh = false)
        {
            IncreaseActualQuantity2(quantity, mdUnit, autoRefresh, 0);
        }

        private void IncreaseActualQuantity2(double quantity, MDUnit mdUnit, bool autoRefresh, short recCounter)
        {
            double quantityUOM = Material.ConvertQuantity(quantity, mdUnit, this.MDUnit);
            this.ActualQuantity += quantityUOM;
            if (recCounter == 0 && (this.IsFinalMixure || this.IsFinalMixureBatch))
                this.ProdOrderPartslist.ActualQuantity += quantityUOM;
            recCounter++;
            if (this.ProdOrderPartslistPos1_ParentProdOrderPartslistPos != null)
            {
                if (autoRefresh)
                    this.ProdOrderPartslistPos1_ParentProdOrderPartslistPos.AutoRefresh();
                this.ProdOrderPartslistPos1_ParentProdOrderPartslistPos.IncreaseActualQuantity2(quantity, mdUnit, autoRefresh, recCounter);
            }
        }

        //private class QuickActualQuantitySum
        //{
        //    public Guid? Key { get; set; }
        //    public double SumUOM { get; set; }
        //    public bool Done { get; set; }
        //}

        public void RecalcActualQuantityFast()
        {
            ProdOrderPartslistPos root = this;
            if (this.ParentProdOrderPartslistPosID.HasValue)
                root = this.TopParentPartslistPos;
            root.RecalcActualQuantityFast2();
            //root.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Where(c => c.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Any()).Any();

            //DatabaseApp context = this.GetObjectContext<DatabaseApp>();
            //QuickActualQuantitySum[] quickSumList = context.ProdOrderPartslistPos
            //            .Where(c => c.ParentProdOrderPartslistPosID.HasValue && c.ProdOrderPartslistID == root.ProdOrderPartslistID && c.MaterialID == root.MaterialID)
            //            .GroupBy(c => c.ParentProdOrderPartslistPosID,
            //            (k, c) => new QuickActualQuantitySum() { Key = k, SumUOM = c.Sum(s => s.ActualQuantityUOM) }).ToArray();
        }

        private void RecalcActualQuantityFast2()
        {
            double sumActualQuantityUOM = 0;
            double sumCalledUpQUOM = 0;
            if (!this.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Any())
                return;
            if (this.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Where(c => c.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Any()).Any())
            {
                foreach (ProdOrderPartslistPos childPos in this.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
                {
                    childPos.RecalcActualQuantityFast2();
                    if (childPos.ActualQuantityUOM > 0.000001
                        || (childPos.MDProdOrderPartslistPosStateID.HasValue
                            && childPos.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex >= (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                            && childPos.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex <= (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled))
                        sumCalledUpQUOM += childPos.ActualQuantityUOM;
                    else
                        sumCalledUpQUOM += childPos.TargetQuantityUOM;
                    sumActualQuantityUOM += childPos.ActualQuantityUOM;
                }
            }
            else
            {
                sumActualQuantityUOM = this.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Sum(c => c.ActualQuantityUOM);
                sumCalledUpQUOM = this.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Sum(c => (c.ActualQuantityUOM > 0.000001 || (c.MDProdOrderPartslistPosStateID.HasValue
                            && c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex >= (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                            && c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex <= (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled))
                            ? c.ActualQuantityUOM : c.TargetQuantityUOM);
            }
            if (Math.Abs(this.ActualQuantityUOM - sumActualQuantityUOM) > Double.Epsilon)
                this.ActualQuantityUOM = sumActualQuantityUOM;
            if (Math.Abs(this.CalledUpQuantityUOM - sumCalledUpQUOM) > Double.Epsilon)
                this.CalledUpQuantityUOM = sumCalledUpQUOM;
        }

        public void RecalcActualQuantity(Nullable<MergeOption> mergeOption = null)
        {
            if (mergeOption.HasValue)
                this.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Load(mergeOption.Value);
            else
                this.ProdOrderPartslistPos_ParentProdOrderPartslistPos.AutoLoad();

            double sumActualQuantity = 0;
            double sumActualQuantityUOM = 0;
            double sumCalledUpQUOM = 0;
            foreach (ProdOrderPartslistPos childPos in this.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
            {
                childPos.RecalcActualQuantity(mergeOption);
                if (childPos.ActualQuantityUOM > 0.000001)
                    sumCalledUpQUOM += childPos.ActualQuantityUOM;
                else
                    sumCalledUpQUOM += childPos.TargetQuantityUOM;
                sumActualQuantityUOM += childPos.ActualQuantityUOM;
                if (this.MDUnit == null && childPos.MDUnit != null)
                    this.MDUnit = childPos.MDUnit;
                else if (this.MDUnit != null && childPos.MDUnit == null)
                    this.MDUnit = this.MDUnit;
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

                        this.Root().Messages.LogException(ClassName, "RecalcActualQuantity", msg);
                    }
                }
            }

            DatabaseApp dbApp = null;
            var sumsPerUnitID = this.FacilityBookingCharge_ProdOrderPartslistPos
                                        .CreateSourceQuery()
                                        .Where(c => c.FacilityBookingTypeIndex != (short)GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge)
                                        .GroupBy(c => c.MDUnitID)
                                        .Select(t => new { MDUnitID = t.Key, outwardQUOM = t.Sum(u => u.OutwardQuantityUOM), inwardQUOM = t.Sum(u => u.InwardQuantityUOM) })
                                        .ToArray();
            MDUnit thisMDUnit = this.MDUnit;
            foreach (var sumPerUnit in sumsPerUnitID)
            {
                double quantity = Math.Abs(sumPerUnit.outwardQUOM) > Double.Epsilon ? sumPerUnit.outwardQUOM : sumPerUnit.inwardQUOM;
                sumActualQuantityUOM += quantity;
                if (thisMDUnit != null && sumPerUnit.MDUnitID != thisMDUnit.MDUnitID)
                {
                    if (dbApp == null)
                        dbApp = this.GetObjectContext() as DatabaseApp;
                    MDUnit fromMDUnit = dbApp.MDUnit.Where(c => c.MDUnitID == sumPerUnit.MDUnitID).FirstOrDefault();
                    quantity = this.Material.ConvertQuantity(quantity, fromMDUnit, thisMDUnit);
                }
                sumActualQuantity += quantity;
            }

            //if (mergeOption.HasValue)
            //    this.FacilityBooking_ProdOrderPartslistPos.Load(mergeOption.Value);
            //else
            //    this.FacilityBooking_ProdOrderPartslistPos.AutoLoad();
            //foreach (FacilityBooking fb in FacilityBooking_ProdOrderPartslistPos)
            //{
            //    foreach (FacilityBookingCharge fbc in fb.FacilityBookingCharge_FacilityBooking)
            //    {
            //        try
            //        {
            //            if (this.MaterialPosType == GlobalApp.MaterialPosTypes.InwardRoot
            //                || this.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
            //                || this.MaterialPosType == GlobalApp.MaterialPosTypes.InwardPart
            //                || this.MaterialPosType == GlobalApp.MaterialPosTypes.InwardPartIntern
            //                || this.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardInternInwardExtern)
            //            {
            //                if (this.MDUnit == null)
            //                    sumActualQuantity += fbc.InwardQuantityUOM;
            //                else
            //                    sumActualQuantity += fbc.InwardMaterial.ConvertQuantity(fbc.InwardQuantity, fbc.MDUnit, this.MDUnit);
            //                sumActualQuantityUOM += fbc.InwardQuantityUOM;
            //            }
            //            else if (this.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardRoot
            //                || this.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardPart)
            //            {
            //                if (this.MDUnit == null)
            //                {
            //                    sumActualQuantity += fbc.OutwardQuantityUOM;
            //                }
            //                else
            //                    sumActualQuantity += fbc.OutwardMaterial.ConvertQuantity(fbc.OutwardQuantity, fbc.MDUnit, this.MDUnit);
            //                sumActualQuantityUOM += fbc.OutwardQuantityUOM;

            //            }
            //        }
            //        catch (Exception ec)
            //        {
            //            string msg = ec.Message;
            //            if (ec.InnerException != null && ec.InnerException.Message != null)
            //                msg += " Inner:" + ec.InnerException.Message;

            //            this.Root().Messages.LogException(ClassName, "RecalcActualQuantity(10)", msg);
            //        }
            //    }
            //}

            if (this.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardRoot || this.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardPart)
            {
                if (mergeOption.HasValue)
                    this.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Load(mergeOption.Value);
                else
                    this.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.AutoLoad();
                if (this.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
                {
                    foreach (var relationItem in this.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                    {
                        relationItem.RecalcActualQuantity();
                        if (relationItem.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation != null)
                            continue;
                        if (this.MDUnit == null)
                        {
                            sumActualQuantity += relationItem.ActualQuantity;
                        }
                        else
                        {
                            sumActualQuantity += this.Material.ConvertQuantity(relationItem.ActualQuantity, this.Material.BaseMDUnit, this.MDUnit);
                        }
                        sumActualQuantityUOM += relationItem.ActualQuantityUOM;
                    }
                }
            }

            if (Math.Abs(this.ActualQuantityUOM - sumActualQuantityUOM) > Double.Epsilon)
                this.ActualQuantityUOM = sumActualQuantityUOM;
            if (Math.Abs(this.ActualQuantity - sumActualQuantity) > Double.Epsilon)
                this.ActualQuantity = sumActualQuantity;
            if (Math.Abs(this.CalledUpQuantityUOM - sumCalledUpQUOM) > Double.Epsilon)
                this.CalledUpQuantityUOM = sumCalledUpQUOM;
        }

        public double PreBookingInwardQuantityUOM(Nullable<MergeOption> mergeOption = null)
        {
            if (mergeOption.HasValue)
                this.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Load(mergeOption.Value);
            else
                this.ProdOrderPartslistPos_ParentProdOrderPartslistPos.AutoLoad();

            double sumUOM = 0;
            foreach (ProdOrderPartslistPos childPos in this.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
            {
                sumUOM += childPos.PreBookingInwardQuantityUOM(mergeOption);
            }

            if (mergeOption.HasValue)
                this.FacilityPreBooking_ProdOrderPartslistPos.Load(mergeOption.Value);
            else
                this.FacilityPreBooking_ProdOrderPartslistPos.AutoLoad();
            foreach (FacilityPreBooking fb in FacilityPreBooking_ProdOrderPartslistPos)
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

                        this.Root().Messages.LogException(ClassName, "PreBookingInwardQuantityUOM", msg);
                    }
                }
            }
            return sumUOM;
        }

        public double PreBookingOutwardQuantityUOM(Nullable<MergeOption> mergeOption = null)
        {
            if (mergeOption.HasValue)
                this.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Load(mergeOption.Value);
            else
                this.ProdOrderPartslistPos_ParentProdOrderPartslistPos.AutoLoad();
            double sumUOM = 0;
            foreach (ProdOrderPartslistPos childPos in this.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
            {
                sumUOM += childPos.PreBookingOutwardQuantityUOM(mergeOption);
            }

            if (mergeOption.HasValue)
                this.FacilityPreBooking_ProdOrderPartslistPos.Load(mergeOption.Value);
            else
                this.FacilityPreBooking_ProdOrderPartslistPos.AutoLoad();
            foreach (FacilityPreBooking fb in FacilityPreBooking_ProdOrderPartslistPos)
            {
                if (fb.OutwardQuantity.HasValue)
                {
                    try
                    {
                        sumUOM += Material.ConvertToBaseQuantity(fb.OutwardQuantity.Value, this.MDUnit);
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

        #region IPartslistPos implementation
        public IEnumerable<IPartslistPosRelation> I_PartslistPosRelation_TargetPartslistPos
        {
            get { return this.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos; }
        }

        public IEnumerable<IPartslistPosRelation> I_PartslistPosRelation_SourcePartslistPos
        {
            get { return this.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos; }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return
                (ProdOrderPartslist != null && ProdOrderPartslist.ProdOrder != null ?
                    (
                        ProdOrderPartslist.ProdOrder.ToString() + "/#" +
                        ProdOrderPartslist.Sequence.ToString() + "/"
                     ) : ""
                ) +
                    MaterialPosType + "/#" +
                    Sequence + "/" +
                    (Material != null ? Material.ToString() : "-");
        }

        #endregion
    }
}
