using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.ESGMPMaterialGroupPos, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Sequenz'}", "", "", true)]
    [ACPropertyEntity(2, "MDGMPAdditive", ConstApp.ESGMPAdditives, Const.ContextDatabase + "\\MDGMPAdditive", "", true)]
    [ACPropertyEntity(3, "MaxConcentration", "en{'Max Concentration'}de{'Max. Konzentration'}", "", "", true)]
    [ACPropertyEntity(9999, MDGMPMaterialGroup.ClassName, ConstApp.ESGMPMaterialGroup, Const.ContextDatabase + "\\" + MDGMPMaterialGroup.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MDGMPMaterialGroupPos.ClassName, ConstApp.ESGMPMaterialGroupPos, typeof(MDGMPMaterialGroupPos), MDGMPMaterialGroupPos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDGMPMaterialGroupPos>) })]
    public partial class MDGMPMaterialGroupPos
    {
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
                    if (!mdGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup.IsLoaded)
                        mdGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup.Load();
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

            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
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
            database.DeleteObject(this);
            MDGMPMaterialGroupPos.RenumberSequence(mdGMPMaterialGroup, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(MDGMPMaterialGroup MDGMPMaterialGroup, int sequence)
        {
            var elements = from c in MDGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup where c.Sequence > sequence orderby c.Sequence select c;
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

    }
}
