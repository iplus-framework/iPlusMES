﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Batchplan group'}de{'Batchplan Gruppe'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOMDBatchPlanGroup")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(3, nameof(MDBatchPlanGroupIndex), "en{'Batchplan group'}de{'Batchplan Gruppe'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + nameof(MDBatchPlanGroup), "en{'Batchplan group'}de{'Batchplan Gruppe'}", typeof(MDBatchPlanGroup), nameof(MDBatchPlanGroup), Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDBatchPlanGroup>) })]
    public partial class MDBatchPlanGroup
    {
        #region c'tors


        public static MDBatchPlanGroup NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDBatchPlanGroup entity = new MDBatchPlanGroup();
            entity.MDBatchPlanGroupID = Guid.NewGuid();
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
        public override string ACCaption
        {
            get
            {
                return MDBatchPlanGroupName;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return Const.MDKey;
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

        #region Additional

       
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        public String MDBatchPlanGroupName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDBatchPlanGroupName");
            }
        }

        #endregion
    }
}