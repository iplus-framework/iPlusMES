using gip.core.datamodel;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.PickingType, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOPickingType")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(3, "MDPickingTypeIndex", "en{'Picking type'}de{'Kommissionierung Typ'}", typeof(GlobalApp.PickingType), "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + MDPickingType.ClassName, "en{'Picking type'}de{'Kommissionierung Typ'}", typeof(MDPickingType), MDPickingType.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDPickingType>) })]
    public partial class MDPickingType
    {
        #region c'tors

        public const string ClassName = "MDPickingType";

        public static MDPickingType NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDPickingType entity = new MDPickingType();
            entity.MDPickingTypeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDPickingTypeIndex = (short)GlobalApp.PickingType.AutomaticRelocation;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        public static readonly Func<DatabaseApp, IQueryable<MDPickingType>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDPickingType>>(
            (database) => from c in database.MDPickingType where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDPickingType>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDPickingType>>(
            (database, index) => from c in database.MDPickingType where c.MDPickingTypeIndex == index select c
        );

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
                return MDPickingTypeName;
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

        #region Additional

        [NotMapped]
        public GlobalApp.PickingType PickingType
        {
            get
            {
                return (GlobalApp.PickingType) MDPickingTypeIndex;
            }
            set
            {
                MDPickingTypeIndex = (short)value;
                OnPropertyChanged("PickingType");
            }
        }

        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDPickingTypeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDPickingTypeName");
            }
        }

        #endregion
    }
}
