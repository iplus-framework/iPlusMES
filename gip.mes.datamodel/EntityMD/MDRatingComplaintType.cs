using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.ESRatingComplaintType, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSORatingComplaintType")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans,  "", "", true, MinLength = 1)]
    [ACPropertyEntity(1, Const.MDKey, Const.EntityKey,  "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence,"", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault,"", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioCompany, Const.QueryPrefix + MDRatingComplaintType.ClassName, ConstApp.ESRatingComplaintType, typeof(MDRatingComplaintType), MDRatingComplaintType.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDRatingComplaintType>) })]
    [NotMapped]
    public partial class MDRatingComplaintType
    {
        [NotMapped]
        public const string ClassName = "MDRatingComplaintType";

        #region New/Delete
        public static MDRatingComplaintType NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDRatingComplaintType entity = new MDRatingComplaintType();
            entity.MDRatingComplaintTypeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
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
                return MDRatingComplaintTypeName;
            }
        }

        #endregion

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDRatingComplaintTypeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDRatingComplaintTypeName");
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
