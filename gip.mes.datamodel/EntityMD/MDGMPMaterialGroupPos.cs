using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.ESGMPMaterialGroupPos, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Sequenz'}", "", "", true)]
    [ACPropertyEntity(2, "MDGMPAdditive", ConstApp.ESGMPAdditives, Const.ContextDatabase + "\\MDGMPAdditive" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, "MaxConcentration", "en{'Max Concentration'}de{'Max. Konzentration'}", "", "", true)]
    [ACPropertyEntity(9999, MDGMPMaterialGroup.ClassName, ConstApp.ESGMPMaterialGroup, Const.ContextDatabase + "\\" + MDGMPMaterialGroup.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MDGMPMaterialGroupPos.ClassName, ConstApp.ESGMPMaterialGroupPos, typeof(MDGMPMaterialGroupPos), MDGMPMaterialGroupPos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDGMPMaterialGroupPos>) })]
    [NotMapped]
    public partial class MDGMPMaterialGroupPos
    {
        [NotMapped]
        public const string ClassName = "MDGMPMaterialGroupPos";

        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static MDGMPMaterialGroupPos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDGMPMaterialGroupPos entity = new MDGMPMaterialGroupPos();
            entity.MDGMPMaterialGroupPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is MDGMPMaterialGroup)
            {
                MDGMPMaterialGroup mdGMPMaterialGroup = parentACObject as MDGMPMaterialGroup;
                try
                {
                    if (!mdGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup_IsLoaded)
                        mdGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup.AutoLoad(mdGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroupReference, mdGMPMaterialGroup);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (Database.Root != null && Database.Root.Messages != null)
                        Database.Root.Messages.LogException("MDGMPMaterialGroupPos", Const.MN_NewACObject, msg);
                }

                if (mdGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup != null && mdGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup.Select(c => c.Sequence).Any())
                    entity.Sequence = mdGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup.Select(c => c.Sequence).Max() + 1;
                else
                    entity.Sequence = 1;

                entity.MDGMPMaterialGroup = mdGMPMaterialGroup;
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
            MDGMPMaterialGroup mdGMPMaterialGroup = MDGMPMaterialGroup;
            database.Remove(this);
            MDGMPMaterialGroupPos.RenumberSequence(mdGMPMaterialGroup, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(MDGMPMaterialGroup mdGMPMaterialGroup, int sequence)
        {
            if (mdGMPMaterialGroup == null
                || !mdGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup.Any())
                return;

            var elements = from c in mdGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup where c.Sequence > sequence orderby c.Sequence select c;
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
                return Sequence.ToString() + " " + MDGMPAdditive.ACCaption;
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

    }
}
