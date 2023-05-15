using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.ESCostCenter, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOCostCenter")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, "MDCostCenterNo", ConstApp.ESCostCenter, "", "", true, MinLength = 1)]
    [ACPropertyEntity(3, Const.IsEnabled, Const.EntityIsEnabled, "", "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioSystem, Const.QueryPrefix + MDCostCenter.ClassName, ConstApp.ESCostCenter, typeof(MDCostCenter), MDCostCenter.ClassName, "MDCostCenterNo,MDNameTrans", "MDCostCenterNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDCostCenter>) })]
    [NotMapped]
    public partial class MDCostCenter : IACObjectEntity
    {
        [NotMapped]
        public const string ClassName = "MDCostCenter";

        #region New/Delete
        public static MDCostCenter NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDCostCenter entity = new MDCostCenter();
            entity.MDCostCenterID = Guid.NewGuid();
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
                return MDCostCenterName;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return Const.MDKey;
            }
        }

        #endregion

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDCostCenterName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDCostCenterName");
            }
        }

#endregion
    }
}
