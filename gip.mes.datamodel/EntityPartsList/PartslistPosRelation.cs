using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'BOM Line Relation'}de{'Stücklistenpositionsbeziehung'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Reihenfolge'}", "", "", true)]
    [ACPropertyEntity(2, "TargetQuantityUOM", "en{'Target Quantity (UOM)'}de{'Sollmenge (BME)'}", "", "", true)]
    [ACPropertyEntity(3, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(4, "SourcePartslistPos", "en{'Sub-Item from'}de{'Unterposition von'}", Const.ContextDatabase + "\\PartslistPos", "", true)]
    [ACPropertyEntity(5, "TargetPartslistPos", "en{'Intermediate Product'}de{'Zwischenprodukt'}", Const.ContextDatabase + "\\PartslistPos", "", true)]
    [ACPropertyEntity(6, "MaterialWFRelation", "en{'MaterialWFRelation'}de{'MaterialWFRelation'}", Const.ContextDatabase + "\\MaterialWFRelation", "", true)]
    [ACPropertyEntity(7, "BaseMDUnit", "en{'Base Unit of Measure UOM'}de{'Basismengeneinheit'}", Const.ContextDatabase + "\\SourcePartslistPos\\Material\\MDUnitList", "", true)]
    [ACPropertyEntity(8, "RetrogradeFIFO", "en{'Backflushing'}de{'Retrograde Entnahme'}", "", "", true)]
    [ACPropertyEntity(9, "Anterograde", "en{'Anterograde inward posting'}de{'Anterograde Zugangsbuchung'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + PartslistPosRelation.ClassName, "en{'PartslistPosRelation'}de{'PartslistPosRelation'}", typeof(PartslistPosRelation), PartslistPosRelation.ClassName, "", PartslistPosRelation.ClassName + "ID")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<PartslistPosRelation>) })]
    public partial class PartslistPosRelation : IPartslistPosRelation
    {
        public const string ClassName = "PartslistPosRelation";

        #region static mehtods
        public static PartslistPosRelation NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            PartslistPosRelation entity = new PartslistPosRelation();
            entity.PartslistPosRelationID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject != null)
                entity.TargetPartslistPos = parentACObject as PartslistPos;

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
        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "TargetPartslistPos\\Material\\MaterialNo,SourcePartslistPos\\Material\\MaterialNo,Sequence";
            }
        }

        /// <summary>
        /// Returns Partslist
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Partslist</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return TargetPartslistPos;
            }
        }
        #endregion

        #region Partial Methods

        bool _OnTargetQuantityChanging = false;
        partial void OnTargetQuantityChanged()
        {
            if (!_OnTargetQuantityUOMChanging && EntityState != System.Data.EntityState.Detached && SourcePartslistPos != null && SourcePartslistPos.Material != null && SourcePartslistPos.MDUnit != null)
            {
                _OnTargetQuantityChanging = true;
                try
                {
                    TargetQuantityUOM = SourcePartslistPos.Material.ConvertToBaseQuantity(TargetQuantity, SourcePartslistPos.MDUnit);
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

        bool _OnTargetQuantityUOMChanging = false;
        partial void OnTargetQuantityUOMChanged()
        {
            if (!_OnTargetQuantityChanging && EntityState != System.Data.EntityState.Detached && SourcePartslistPos != null && SourcePartslistPos.Material != null && SourcePartslistPos.MDUnit != null)
            {
                _OnTargetQuantityUOMChanging = true;
                try
                {
                    TargetQuantity = SourcePartslistPos.Material.ConvertQuantity(TargetQuantityUOM, SourcePartslistPos.Material.BaseMDUnit, SourcePartslistPos.MDUnit);
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
                if (SourcePartslistPos != null)
                    return SourcePartslistPos.Backflushing;
                return false;
            }
        }

        /// <summary>
        /// Property that evaluates the override of the Anterograde-Fields in Tables PartslistPos->Material
        /// </summary>
        public bool Foreflushing
        {
            get
            {
                if (this.Anterograde.HasValue)
                    return this.Anterograde.Value;
                if (SourcePartslistPos != null)
                    return SourcePartslistPos.Foreflushing;
                return false;
            }
        }

        #endregion

        #region convention implementation

        public override string ToString()
        {
            string plNo = "";
            string source = "";
            string target = "";
            if (SourcePartslistPosID != Guid.Empty && SourcePartslistPos != null && SourcePartslistPos.MaterialID != Guid.Empty)
            {
                plNo = SourcePartslistPos.Partslist.PartslistNo;
                source = SourcePartslistPos.Material.ToString();
            }
            if (TargetPartslistPosID != Guid.Empty && TargetPartslistPos != null && TargetPartslistPos.MaterialID != Guid.Empty)
            {
                target = TargetPartslistPos.Material.ToString();
            }
            return string.Format(@"PartslistPosRelation #{0} PL: {1} {2} => {3}", Sequence, plNo, source, target);
        }

        #endregion

        #region IPartslistPosRelation implementation
        public IPartslistPos I_SourcePartslistPos
        {
            get { return this.SourcePartslistPos; }
        }

        public IPartslistPos I_TargetPartslistPos
        {
            get { return this.TargetPartslistPos; }
        }
        #endregion

        #region Cloning

        public object Clone(bool withReferences)
        {
            PartslistPosRelation clonedObject = new PartslistPosRelation();
            clonedObject.PartslistPosRelationID = this.PartslistPosRelationID;
            clonedObject.CopyFrom(this, withReferences, true);
            return clonedObject;
        }

        public void CopyFrom(PartslistPosRelation from, bool withReferences, bool copyQuantity)
        {
            if (withReferences)
            {
                TargetPartslistPosID = from.TargetPartslistPosID;
                SourcePartslistPosID = from.SourcePartslistPosID;
                MaterialWFRelationID = from.MaterialWFRelationID;
            }

            if(copyQuantity)
            {
                TargetQuantity = from.TargetQuantity;
                TargetQuantityUOM = from.TargetQuantityUOM;
            }

            Sequence = from.Sequence;
            RetrogradeFIFO = from.RetrogradeFIFO;
            Anterograde = from.Anterograde;
            XMLConfig = from.XMLConfig;
        }
        #endregion
    }
}