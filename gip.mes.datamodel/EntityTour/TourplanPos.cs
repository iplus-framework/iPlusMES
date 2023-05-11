using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Transactions; 
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'TourplanPos'}de{'TourplanPos'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "")]
    [ACPropertyEntity(1, Const.SortIndex, "en{'Sequence'}de{'Folge'}","", "", true)]
    [ACPropertyEntity(2, MDTourplanPosState.ClassName, "en{'State'}de{'Status'}", Const.ContextDatabase + "\\" + MDTourplanPosState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, Company.ClassName, "en{'Company'}de{'Firma'}", Const.ContextDatabase + "\\" + Company.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, CompanyAddress.ClassName, "en{'Address'}de{'Adresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, CompanyAddressUnloadingpoint.ClassName, "en{'Unloadingpoint'}de{'Abladestelle'}", Const.ContextDatabase + "\\" + CompanyAddressUnloadingpoint.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, MDTimeRange.ClassName, "en{'Timerange'}de{'Zeitraum'}", Const.ContextDatabase + "\\" + MDTimeRange.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(7, "Comment", "en{'Comment'}de{'Bemerkung'}","", "", true)]
    [ACPropertyEntity(8, Const.EntityInsertName, "en{'InsertName'}de{'InsertName'}")]
    [ACPropertyEntity(9, Tourplan.ClassName, "en{'Tourplan'}de{'Tourplan'}", Const.ContextDatabase + "\\" + Tourplan.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + TourplanPos.ClassName, "en{'TourplanPos'}de{'TourplanPos'}", typeof(TourplanPos), TourplanPos.ClassName, Const.SortIndex, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<TourplanPos>) })]
    public partial class TourplanPos
    {
        public const string ClassName = "TourplanPos";

        #region New/Delete
        public static TourplanPos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            TourplanPos entity = new TourplanPos();
            entity.TourplanPosID = Guid.NewGuid();
            if (parentACObject != null && parentACObject is Tourplan)
                entity.Tourplan = parentACObject as Tourplan;
            entity.MDTourplanPosState = MDTourplanPosState.DefaultMDTourplanPosState(dbApp);
            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
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
                return InsertName;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return Const.EntityInsertName;
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
