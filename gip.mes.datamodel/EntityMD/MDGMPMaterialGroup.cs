using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.ESGMPMaterialGroup, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOGMPMaterialGroup")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, "MDGMPMaterialGroupNo", "en{'Material Group No.'}de{'Material Gruppen Nr.'}", "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "WearFacilityNo", "en{'Wear Part No.'}de{'Verschleißteil-Nr.'}", "", "", true)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MDGMPMaterialGroup.ClassName, ConstApp.ESGMPMaterialGroup, typeof(MDGMPMaterialGroup), MDGMPMaterialGroup.ClassName, Const.MDNameTrans, "MDGMPMaterialGroupNo", new object[]
        {
                new object[] {Const.QueryPrefix + MDGMPMaterialGroupPos.ClassName, ConstApp.ESGMPMaterialGroupPos, typeof(MDGMPMaterialGroupPos), MDGMPMaterialGroupPos.ClassName + "_" + MDGMPMaterialGroup.ClassName, "Sequence", "Sequence"}
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDGMPMaterialGroup>) })]
    [NotMapped]
    public partial class MDGMPMaterialGroup
    {
        [NotMapped]
        public const string ClassName = "MDGMPMaterialGroup";

        #region New/Delete
        public static MDGMPMaterialGroup NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDGMPMaterialGroup entity = new MDGMPMaterialGroup();
            entity.MDGMPMaterialGroupID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
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
                return MDGMPMaterialGroupName;
            }
        }

        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && className == MDGMPMaterialGroupPos.ClassName)
            {
                Int32 sequence = 0;
                if (Int32.TryParse(filterValues[0], out sequence))
                    return this.MDGMPMaterialGroupPos_MDGMPMaterialGroup.Where(c => c.Sequence == sequence).FirstOrDefault();
            }
            return null;
        }

        #endregion

        #region IACObjectEntity Members
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            if (string.IsNullOrEmpty(MDGMPMaterialGroupName))
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
                return "MDGMPMaterialGroupName";
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

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDGMPMaterialGroupName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDGMPMaterialGroupName");
            }
        }

#endregion

    }
}
