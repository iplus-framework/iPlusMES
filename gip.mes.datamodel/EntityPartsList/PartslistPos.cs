using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    /// <summary>
    /// PartslistPos (RezeptKomp)
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Bill of Materials Position'}de{'Stücklistenposition'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]

    [ACPropertyEntity(1, nameof(Sequence), "en{'Sequence'}de{'Reihenfolge'}", "", "", true)]
    [ACPropertyEntity(2, nameof(SequenceProduction), "en{'Production Sequence'}de{'Produktionsreihenfolge'}", "", "", true)]
    [ACPropertyEntity(3, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(3, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(13, "BaseMDUnit", "en{'Base Unit of Measure UOM'}de{'Basismengeneinheit'}", Const.ContextDatabase + "\\Material\\MDUnitList", "", true)]
    [ACPropertyEntity(5, "TargetQuantityUOM", "en{'Target Quantity (UOM)'}de{'Sollmenge (BME)'}", "", "", true)]
    [ACPropertyEntity(6, MDUnit.ClassName, "en{'Unit of Measurement'}de{'Maßeinheit'}", Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(7, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(9, "IsBaseQuantityExcluded", "en{'IsBaseQuantityExcluded'}de{'IsBaseQuantityExcluded'}", "", "", true)]
    [ACPropertyEntity(11, "LineNumber", "en{'Item Number'}de{'Positionsnummer'}", "", "", true)]
    [ACPropertyEntity(12, "RetrogradeFIFO", "en{'Backflushing'}de{'Retrograde Entnahme'}", "", "", true)]
    [ACPropertyEntity(13, "ExplosionOff", "en{'Explosion Off'}de{'Stoprückauflösung'}", "", "", true)]
    [ACPropertyEntity(14, nameof(Anterograde), "en{'Anterograde inward posting'}de{'Anterograde Zugangsbuchung'}", "", "", true)]
    [ACPropertyEntity(15, nameof(PostingQuantitySuggestion), "en{'Suggest quant quantity on posting'}de{'Vorschlagsmenge bei der Buchung'}", "", "", true)]
    [ACPropertyEntity(16, nameof(KeepBatchCount), "en{'Keep batch count'}de{'Batchanzahl beibehalten'}", "", "", true)]
    [ACPropertyEntity(17, nameof(TakeMatFromOtherOrder), "en{'Take material from other order'}de{'Entnahme von anderem Auftrag erlaubt'}", "", "", true)]
    [ACPropertyEntity(9999, Partslist.ClassName, "en{'Bill of Materials'}de{'Stückliste'}", Const.ContextDatabase + "\\" + Partslist.ClassName, "", true)]
    [ACPropertyEntity(9999, "ParentPartslistPos", "en{'Parent line'}de{'Elternposition'}", Const.ContextDatabase + "\\" + PartslistPos.ClassName, "", true)]
    [ACPropertyEntity(9999, "AlternativePartslistPos", "en{'Alternative Item'}de{'Alternativposition'}", Const.ContextDatabase + "\\" + PartslistPos.ClassName, "", true)]
    [ACPropertyEntity(9999, "ParentPartslist", "en{'Parent Bill of Materials'}de{'Elternstückliste'}", Const.ContextDatabase + "\\" + Partslist.ClassName, "", true)]
    [ACPropertyEntity(9999, Partslist.ClassName, "en{'Bill of Materials'}de{'Stückliste'}", Const.ContextDatabase + "\\" + Partslist.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9999, "ParentPartslistPos", "en{'Parent line'}de{'Elternposition'}", Const.ContextDatabase + "\\" + PartslistPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9999, "AlternativePartslistPos", "en{'Alternative Item'}de{'Alternativposition'}", Const.ContextDatabase + "\\" + PartslistPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9999, "ParentPartslist", "en{'Parent Bill of Materials'}de{'Elternstückliste'}", Const.ContextDatabase + "\\" + Partslist.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]

    [ACPropertyEntity(9999, "MaterialPosTypeIndex", "en{'Positiontype'}de{'Posistionstyp'}", typeof(GlobalApp.MaterialPosTypes), "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]    
	[ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + PartslistPos.ClassName, "en{'Bill of Materials Pos.'}de{'Stücklistenposition'}", typeof(PartslistPos), PartslistPos.ClassName, "Sequence", "Sequence", new object[]
        {
            new object[] {Const.QueryPrefix + PartslistPosRelation.ClassName, "en{'PartslistPosRelation'}de{'PartslistPosRelation'}", typeof(PartslistPosRelation), PartslistPosRelation.ClassName + "_TargetPartslistPos", "Sequence", "Sequence"}
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<PartslistPos>) })]
    public partial class PartslistPos : IPartslistPos
    {
        public const string ClassName = "PartslistPos";

        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static PartslistPos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            PartslistPos entity = new PartslistPos();
            entity.PartslistPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();

            Partslist partslist = parentACObject as Partslist;
            entity.Partslist = partslist;

            entity.SequenceProduction = entity.Sequence;
            entity.MaterialPosType = GlobalApp.MaterialPosTypes.OutwardRoot;

            if (parentACObject != null && parentACObject.GetType() == typeof(Partslist))
            {
                entity.Partslist = parentACObject as Partslist;
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
            foreach (var basedPos in this.ProdOrderPartslistPos_BasedOnPartslistPos.ToArray())
            {
                basedPos.BasedOnPartslistPosID = null;
            }
            base.DeleteACObject(database, withCheck, softDelete);
            return null;
        }

        public static PartslistPos NewAlternativePartslistPos(DatabaseApp dbApp, IACObject parentACObject, PartslistPos partslistPos)
        {
            PartslistPos entity = PartslistPos.NewACObject(dbApp, parentACObject);
            entity.PartslistPos1_AlternativePartslistPos = partslistPos;
            return entity;
        }

        #endregion

        #region IACUrl Member
        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return Partslist.PartslistNo + " " + Sequence + " | " + Material.MaterialNo;
            }
        }

        /// <summary>
        /// Returns Partslist
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Partslist</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return Partslist;
            }
        }

        /// <summary>
        /// Returns a related EF-Object which is in a Child-Relationship to this.
        /// </summary>
        /// <param name="className">Classname of the Table/EF-Object</param>
        /// <param name="filterValues">Search-Parameters</param>
        /// <returns>A Entity-Object as IACObject</returns>
        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any())
            {
                switch (className)
                {
                    // PartslistPosRelation(KeyACIdentifier) "TargetPartslistPos\\Material\\MaterialNo,SourcePartslistPos\\Material\\MaterialNo,Sequence";
                    case PartslistPosRelation.ClassName:
                        string[] filterItems = filterValues[0].Split(',');
                        string targetMaterialNo = filterItems[0];
                        string soruceMaterialNo = filterItems[1];
                        int sequence = int.Parse(filterItems[2]);
                        return PartslistPosRelation_TargetPartslistPos
                            .Where(x =>
                                //x.TargetPartslistPos.Material.MaterialNo == targetMaterialNo &&
                                (x.SourcePartslistPos != null && x.SourcePartslistPos.Material != null && x.SourcePartslistPos.Material.MaterialNo == soruceMaterialNo) &&
                                x.Sequence == sequence).FirstOrDefault();

                }
            }

            return null;
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
            if (Material == null && MaterialPosType == GlobalApp.MaterialPosTypes.OutwardRoot)
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "Key",
                    Message = "Key",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "Key"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "Sequence,Material\\MaterialNo,MaterialPosTypeIndex";
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

        #region Properties
        [ACPropertyInfo(9999, "", "en{'Materialnummer'}de{'Materialnummer'}")]
        [NotMapped]
        public string MaterialNo
        {
            get
            {
                string no = "";
                if (Material != null)
                    no = Material.MaterialNo;

                return no;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Materialname'}de{'Materialname'}")]
        [NotMapped]
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

        /// <summary>
        /// Boolean value for editable content in partslist
        /// Only input positions shuld be editable, mixures not in part of material selection
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'IsOutwardRoot'}de{'IsOutwardRoot'}")]
        [NotMapped]
        public bool IsOutwardRoot
        {
            get
            {
                return this.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot;
            }
        }

        [ACPropertyInfo(9999)]
        public bool IsFinalMixure
        {
            get
            {
                return this.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                    && !PartslistPosRelation_SourcePartslistPos.Any();
                //&& Material != null
                //&& !this.Material.MaterialWFRelation_SourceMaterial.Where(c => c.SourceMaterialID != c.TargetMaterialID).Any();
            }
        }

        /// <summary>
        /// This rest quantity is calculated quantity - difference between input and used 
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'RestQuantity'}de{'RestQuantity'}")]
        [NotMapped]
        public double RestQuantity { get; set; }

        /// <summary>
        /// This rest quantity is calculated quantity - difference between input and used 
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'RestQuantityUOM'}de{'RestQuantityUOM'}")]
        [NotMapped]
        public double RestQuantityUOM { get; set; }

        /// <summary>
        /// This is used now only for handling value in new partslist version process
        /// not reference value in database
        /// </summary>
        [NotMapped]
        public PartslistPos NewVersion
        {
            get;
            set;
        }

        private int _PositionUsedCount;
        [ACPropertyInfo(9999)]
        [NotMapped]
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
        /// Property that evaluates the override of the RetrogradeFIFO-Fields in Tables PartslistPos->Material
        /// </summary>
        [NotMapped]
        public bool Backflushing
        {
            get
            {
                if (this.RetrogradeFIFO.HasValue)
                    return this.RetrogradeFIFO.Value;
                else if (Material != null && Material.RetrogradeFIFO.HasValue)
                    return Material.RetrogradeFIFO.Value;
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
                else if (Material != null && Material.Anterograde.HasValue)
                    return Material.Anterograde.Value;
                return false;
            }
        }

        [NotMapped]
        private bool _IsChecked;
        [ACPropertyInfo(9999, "", "en{'Selected'}de{'Ausgewählt'}")]
        [NotMapped]
        public bool IsChecked
        {
            get => _IsChecked;
            set
            {
                _IsChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        /// <summary>
        /// If this property is set, then the entire quantity of the selected quant will be suggested when the user has to enter the quantity in the class PWManualAddtition.
        /// In this case the quant will be set automatically to "NotAvailable" after posting because the stock was exactly zero.
        /// If this component is used with PWManualWeighing, then the Material.ZeroBookingTolerance is used to compare if the complete quant was used up. 
        /// If its in tolerance then the quant will be posted to zero stock.
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Consume entire quant quantity'}de{'Vollständige Quantmenge verbrauchen'}", "", true)]
        [NotMapped]
        public bool SuggestQuantQOnPosting
        {
            get
            {
                return PostingQuantitySuggestion.HasValue && PostingQuantitySuggestion.Value > 0;
            }
            set
            {
                if (value)
                    PostingQuantitySuggestion = 1;
                else
                    PostingQuantitySuggestion = 0;
            }
        }

        [NotMapped]
        public bool IsIntermediateForRecalculate { get; set; }

        #endregion

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(TargetQuantity))
            {
                OnTargetQuantityChanged();
            }
            else if (propertyName == nameof(TargetQuantityUOM))
            {
                OnTargetQuantityUOMChanged();
            }
            base.OnPropertyChanged(propertyName);
        }

        #region Partial Methods

        [NotMapped]
        bool _OnTargetQuantityChanging = false;
        protected void OnTargetQuantityChanged()
        {
            if (!_OnTargetQuantityUOMChanging && EntityState != EntityState.Detached && Material != null && MDUnit != null)
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

                    this.Root().Messages.LogException(ClassName, "OnTargetQuantityChanged", msg);
                }
                finally
                {
                    _OnTargetQuantityChanging = false;
                }
            }
        }

        [NotMapped]
        bool _OnTargetQuantityUOMChanging = false;
        protected void OnTargetQuantityUOMChanged()
        {
            if (!_OnTargetQuantityChanging && EntityState != EntityState.Detached && Material != null && MDUnit != null)
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

                    this.Root().Messages.LogException(ClassName, "OnTargetQuantityUOMChanged", msg);
                }
                finally
                {
                    _OnTargetQuantityUOMChanging = false;
                }
            }
        }

        #endregion

        #region Partslistpos selection

        public static List<PartslistPos> OutwardRootPositions(DatabaseApp dbApp, Guid partslistID)
        {
            return dbApp
                    .PartslistPos
                    .Include(c => c.Material)
                    .Include(c => c.Material.BaseMDUnit)
                    .Include(c => c.MDUnit)
                    .Where(c =>
                        c.PartslistID == partslistID
                        &&
                        c.AlternativePartslistPosID == null
                        &&
                        c.MaterialPosTypeIndex == (short)(gip.mes.datamodel.GlobalApp.MaterialPosTypes.OutwardRoot)
                        &&
                        c.ParentPartslistPosID == null)
                    .OrderBy(c => c.Sequence)
                    .ToList();
        }

        public static List<PartslistPos> Alternatives(DatabaseApp dbApp, Guid partslistID, Guid partslistPosID)
        {
            return dbApp.PartslistPos
                            .Include(c => c.Material)
                            .Include(c => c.Material.BaseMDUnit)
                            .Include(c => c.MDUnit)
                            .Where(c => c.PartslistID == partslistID &&
                                c.AlternativePartslistPosID == partslistPosID
                                && c.MaterialPosTypeIndex == (int)(gip.mes.datamodel.GlobalApp.MaterialPosTypes.OutwardRoot) && c.ParentPartslistPosID == null)
                            .OrderBy(c => c.Sequence)
                            .ToList();
        }

        public static List<PartslistPos> InwardInternPositions(DatabaseApp dbApp, Guid partslistID)
        {
            return dbApp
                   .PartslistPos
                   .Include(c => c.Material)
                   .Include(c => c.MDUnit)
                   .Where(c =>
                       c.PartslistID == partslistID
                       &&
                       c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                   .OrderBy(c => c.Sequence)
                   .ToList();
        }


        #endregion

        #region convention implementations

        public override string ToString()
        {
            string material = "-";
            if (Material != null)
                material = Material.ToString();
            return "PartslistPos #" + Sequence.ToString() + (Partslist != null ? " {" + Partslist.PartslistNo + "} " : "") + material + " (" + MaterialPosType.ToString() + ")";
        }
        #endregion

        #region IPartslistPos implementation
        [NotMapped]
        public IEnumerable<IPartslistPosRelation> I_PartslistPosRelation_TargetPartslistPos
        {
            get { return this.PartslistPosRelation_TargetPartslistPos; }
        }

        [NotMapped]
        public IEnumerable<IPartslistPosRelation> I_PartslistPosRelation_SourcePartslistPos
        {
            get { return this.PartslistPosRelation_SourcePartslistPos; }
        }
        #endregion

        #region Cloning

        public object Clone(bool withReferences)
        {
            PartslistPos clonedObject = new PartslistPos();
            clonedObject.PartslistPosID = this.PartslistPosID;
            clonedObject.CopyFrom(this, withReferences, true);
            return clonedObject;
        }

        public void CopyFrom(PartslistPos from, bool withReferences, bool copyQuantity)
        {
            if (withReferences)
            {
                PartslistID = from.PartslistID;
                Material = from.Material;
                MDUnit = from.MDUnit;
                ParentPartslistPosID = from.ParentPartslistPosID;
                AlternativePartslistPosID = from.AlternativePartslistPosID;
                ParentPartslistID = from.ParentPartslistID;
            }

            if (copyQuantity)
            {
                TargetQuantityUOM = from.TargetQuantityUOM;
                TargetQuantity = from.TargetQuantity;
            }

            Sequence = from.Sequence;
            SequenceProduction = from.SequenceProduction;
            MaterialPosTypeIndex = from.MaterialPosTypeIndex;
            IsBaseQuantityExcluded = from.IsBaseQuantityExcluded;
            XMLConfig = from.XMLConfig;
            LineNumber = from.LineNumber;
            RetrogradeFIFO = from.RetrogradeFIFO;
            Anterograde = from.Anterograde;
            ExplosionOff = from.ExplosionOff;
            KeyOfExtSys = from.KeyOfExtSys;
            PostingQuantitySuggestion = from.PostingQuantitySuggestion;
            KeepBatchCount = from.KeepBatchCount;
        }
        #endregion

        #region other methods
        public void CalcPositionUsedCount()
        {
            if (PartslistPosRelation_SourcePartslistPos != null)
                PositionUsedCount = PartslistPosRelation_SourcePartslistPos.Count();
        }
        #endregion

    }
}
