using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.ESTourplanState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOTourplanState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDTourplanStateIndex", ConstApp.ESTourplanState, typeof(MDTourplanState.TourplanStates), Const.ContextDatabase + "\\TourplanStatesList", "", true, MinValue = (short)TourplanStates.NewCreated)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + MDTourplanState.ClassName, ConstApp.ESTourplanState, typeof(MDTourplanState), MDTourplanState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDTourplanState>) })]
    [NotMapped]
    public partial class MDTourplanState
    {
        [NotMapped]
        public const string ClassName = "MDTourplanState";

        #region New/Delete
        public static MDTourplanState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDTourplanState entity = new MDTourplanState();
            entity.MDTourplanStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.TourplanState = TourplanStates.NewCreated;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDTourplanState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDTourplanState>>(
            (database) => from c in database.MDTourplanState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDTourplanState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDTourplanState>>(
            (database, index) => from c in database.MDTourplanState where c.MDTourplanStateIndex == index select c
        );

        public static MDTourplanState DefaultMDTourplanState(DatabaseApp dbApp)
        {
            try
            {
                MDTourplanState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)TourplanStates.NewCreated).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException(ClassName, "Default" + ClassName, msg);
                return null;
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
                return MDTourplanStateName;
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
        public String MDTourplanStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDTourplanStateName");
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

        #region enums
        [NotMapped]
        public TourplanStates TourplanState
        {
            get
            {
                return (TourplanStates)MDTourplanStateIndex;
            }
            set
            {
                MDTourplanStateIndex = (short)value;
                OnPropertyChanged("TourplanState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDTourplanStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'TourplanState'}de{'TourplanState'}", Global.ACKinds.TACEnum)]
        public enum TourplanStates : short
        {
            NewCreated = 1,
            LoadingPlanGenerated = 2,
            LoadingPlanActive = 3,
            Loaded = 4,
            Delivered = 5,
            Blocked = 6, //Gesperrt
            Cancelled = 7, //Storniert
        }

        [NotMapped]
        static ACValueItemList _TourplanStatesList = null;
        [NotMapped]
        public static ACValueItemList TourplanStatesList
        {
            get
            {
                if (_TourplanStatesList == null)
                {
                    _TourplanStatesList = new ACValueItemList("TourplanState");
                    _TourplanStatesList.AddEntry((short)TourplanStates.NewCreated, "en{'New Created'}de{'Neu Angelegt'}");
                    _TourplanStatesList.AddEntry((short)TourplanStates.LoadingPlanGenerated, "en{'Loading Plan Generated'}de{'Verladeplan generiert'}");
                    _TourplanStatesList.AddEntry((short)TourplanStates.LoadingPlanActive, "en{'Loading Plan Active'}de{'Verladeplan aktiv'}");
                    _TourplanStatesList.AddEntry((short)TourplanStates.Loaded, "en{'Loaded'}de{'Verladen'}");
                    _TourplanStatesList.AddEntry((short)TourplanStates.Delivered, "en{'Delivered'}de{'Zugestellt'}");
                    _TourplanStatesList.AddEntry((short)TourplanStates.Blocked, "en{'Blocked'}de{'Gesperrt'}");
                    _TourplanStatesList.AddEntry((short)TourplanStates.Cancelled, "en{'Cancelled'}de{'Storniert'}");
                }
                return _TourplanStatesList;
            }
        }


#endregion

    }
}




