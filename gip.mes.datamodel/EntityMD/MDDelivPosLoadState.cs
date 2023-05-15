using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.ESDelivPosLoadState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSODelivPosLoadState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDDelivPosLoadStateIndex", ConstApp.ESDelivPosLoadState, typeof(MDDelivPosLoadState.DelivPosLoadStates), Const.ContextDatabase + "\\DelivPosLoadStateList", "", true, MinValue = (short)DelivPosLoadStates.NewCreated)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + MDDelivPosLoadState.ClassName, ConstApp.ESDelivPosLoadState, typeof(MDDelivPosLoadState), MDDelivPosLoadState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDDelivPosLoadState>) })]
    [NotMapped]
    public partial class MDDelivPosLoadState
    {
        [NotMapped]
        public const string ClassName = "MDDelivPosLoadState";

        #region New/Delete
        public static MDDelivPosLoadState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDDelivPosLoadState entity = new MDDelivPosLoadState();
            entity.MDDelivPosLoadStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.DelivPosLoadState = DelivPosLoadStates.NewCreated;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDDelivPosLoadState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDDelivPosLoadState>>(
            (database) => from c in database.MDDelivPosLoadState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDDelivPosLoadState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDDelivPosLoadState>>(
            (database, index) => from c in database.MDDelivPosLoadState where c.MDDelivPosLoadStateIndex == index select c
        );

        public static MDDelivPosLoadState DefaultMDDelivPosLoadState(DatabaseApp dbApp)
        {
            try
            {
                MDDelivPosLoadState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)DelivPosLoadStates.NewCreated).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDDelivPosLoadState", "DefaultMDDelivPosLoadState", msg);
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
                return MDDelivPosLoadStateName;
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

        #region enums
        [NotMapped]
        public DelivPosLoadStates DelivPosLoadState
        {
            get
            {
                return (DelivPosLoadStates)MDDelivPosLoadStateIndex;
            }
            set
            {
                MDDelivPosLoadStateIndex = (short)value;
                OnPropertyChanged("DelivPosLoadState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDDelivPosLoadStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'DelivPosLoadState'}de{'DelivPosLoadState'}", Global.ACKinds.TACEnum)]
        public enum DelivPosLoadStates : short
        {
            NewCreated = 1, //Neu Angelegt
            ReadyToLoad = 2, //Verladebereit
            BatchActive = 3, //Beschickung aktiv
            LoadingToForeCell = 4, //Verladen in Vorzelle
            LoadingActive = 5, //Verladung aktiv
            LoadToTruck = 6 //Verladen in LKW
        }

        [NotMapped]
        static ACValueItemList _DelivPosLoadStateList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        [NotMapped]
        public static ACValueItemList DelivPosLoadStateList
        {
            get
            {
                if (_DelivPosLoadStateList == null)
                {
                    _DelivPosLoadStateList = new ACValueItemList("DelivPosLoadState");
                    _DelivPosLoadStateList.AddEntry((short)DelivPosLoadStates.NewCreated, "en{'New created'}de{'Neu erstellt'}");
                    _DelivPosLoadStateList.AddEntry((short)DelivPosLoadStates.ReadyToLoad, "en{'Ready to load'}de{'Verladebereit'}");
                    _DelivPosLoadStateList.AddEntry((short)DelivPosLoadStates.BatchActive, "en{'Batch active'}de{'Beschickung aktiv'}");
                    _DelivPosLoadStateList.AddEntry((short)DelivPosLoadStates.LoadingToForeCell, "en{'Loading to forecell'}de{'Verladen in Vorzelle'}");
                    _DelivPosLoadStateList.AddEntry((short)DelivPosLoadStates.LoadingActive, "en{'Loading active'}de{'Verladung aktiv'}");
                    _DelivPosLoadStateList.AddEntry((short)DelivPosLoadStates.LoadToTruck, "en{'Loading to Truck'}de{'Verladen in LKW'}");
                }
                return _DelivPosLoadStateList;
            }
        }
        #endregion

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDDelivPosLoadStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDDelivPosLoadStateName");
            }
        }
#endregion

    }
}
