using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions; using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Material GMP-Additive'}de{'Material GMP-Zusatzstoffe'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Reihenfolge'}","", "", true)]
    [ACPropertyEntity(2, "MDGMPAdditive", "en{'MDGMPAdditive'}de{'de-MDGMPAdditive'}", Const.ContextDatabase + "\\MDGMPAdditive", "", true)]
    [ACPropertyEntity(3, "Concentration", "en{'Concentration'}de{'de-Concentration'}","", "", true)]
    [ACPropertyEntity(9999, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MaterialGMPAdditive.ClassName, "en{'Material GMP-Additive'}de{'Material GMP-Zusatzstoffe'}", typeof(MaterialGMPAdditive), MaterialGMPAdditive.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaterialGMPAdditive>) })]
    public partial class MaterialGMPAdditive : IACObjectEntity
    {
        public const string ClassName = "MaterialGMPAdditive";

        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static MaterialGMPAdditive NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MaterialGMPAdditive entity = new MaterialGMPAdditive();
            entity.MaterialGMPAdditiveID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Material)
            {
                Material material = parentACObject as Material;
                try
                {
                    if (!material.MaterialGMPAdditive_Material_IsLoaded)
                        material.MaterialGMPAdditive_Material.AutoLoad(material.MaterialGMPAdditive_MaterialReference, material);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (Database.Root != null && Database.Root.Messages != null)
                        Database.Root.Messages.LogException("MaterialGMPAdditive", Const.MN_NewACObject, msg);
                }

                if (material.MaterialGMPAdditive_Material != null && material.MaterialGMPAdditive_Material.Select(c => c.Sequence).Any())
                    entity.Sequence = material.MaterialGMPAdditive_Material.Select(c => c.Sequence).Max() + 1;
                else
                    entity.Sequence = 1;

                entity.Material = material;
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
            Material material = Material;
            database.Remove(this);
            MaterialGMPAdditive.RenumberSequence(material, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(Material material, int sequence)
        {
            if (material == null
                || !material.MaterialGMPAdditive_Material.Any())
                return;

            var elements = material.MaterialGMPAdditive_Material.Where(c => c.Sequence > sequence).OrderBy(c => c.Sequence);
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
                return Sequence.ToString() + " " + MDGMPAdditive.ACCaption;
            }
        }

        /// <summary>
        /// Returns Material
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Material</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return Material;
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
    }
}
