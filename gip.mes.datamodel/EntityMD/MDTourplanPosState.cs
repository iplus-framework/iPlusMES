using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.ESTourplanPosState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOTourplanPosState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDTourplanPosStateIndex", "en{'Tourplan Pos. Status'}de{'Tourplanpos.-Status'}", typeof(MDTourplanPosState.TourplanPosStates), Const.ContextDatabase + "\\TourplanPosStatesList", "", true, MinValue = (short)MDTourplanPosState.TourplanPosStates.NotPlanned)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + MDTourplanPosState.ClassName, ConstApp.ESTourplanPosState, typeof(MDTourplanPosState), MDTourplanPosState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDTourplanPosState>) })]
    [NotMapped]
    public partial class MDTourplanPosState
    {
        [NotMapped]
        public const string ClassName = "MDTourplanPosState";

        #region New/Delete
        public static MDTourplanPosState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDTourplanPosState entity = new MDTourplanPosState();
            entity.MDTourplanPosStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.TourplanPosState = TourplanPosStates.NotPlanned;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IEnumerable<MDTourplanPosState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IEnumerable<MDTourplanPosState>>(
            (database) => from c in database.MDTourplanPosState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IEnumerable<MDTourplanPosState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IEnumerable<MDTourplanPosState>>(
            (database, index) => from c in database.MDTourplanPosState where c.MDTourplanPosStateIndex == index select c
        );

        public static MDTourplanPosState DefaultMDTourplanPosState(DatabaseApp dbApp)
        {
            try
            {
                MDTourplanPosState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)TourplanPosStates.NotPlanned).FirstOrDefault();
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
                return MDTourplanPosStateName;
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
        public String MDTourplanPosStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDTourplanPosStateName");
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
        public TourplanPosStates TourplanPosState
        {
            get
            {
                return (TourplanPosStates)MDTourplanPosStateIndex;
            }
            set
            {
                MDTourplanPosStateIndex = (short)value;
                OnPropertyChanged("TourplanPosState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDTourplanPosStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'TourplanPosStates'}de{'TourplanPosStates'}", Global.ACKinds.TACEnum)]
        public enum TourplanPosStates : short
        {
            NotPlanned = 1, //nicht geplant
            CompletelyAssigned = 2, //Vollständig zugeordnet
            SubsetAssigned = 3, //Teilmenge zugeordnet
            Delivered = 4, //Geliefert
        }

        [NotMapped]
        static ACValueItemList _TourplanPosStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück.
        /// </summary>
        [NotMapped]
        public static ACValueItemList TourplanPosStatesList
        {
            get
            {
                if (_TourplanPosStatesList == null)
                {
                    _TourplanPosStatesList = new ACValueItemList("TourplanPosStates");
                    _TourplanPosStatesList.AddEntry((short)TourplanPosStates.NotPlanned, "en{'Not Planned'}de{'Nicht geplant'}");
                    _TourplanPosStatesList.AddEntry((short)TourplanPosStates.CompletelyAssigned, "en{'Completely Assigned'}de{'Vollständig zugeordnet'}");
                    _TourplanPosStatesList.AddEntry((short)TourplanPosStates.SubsetAssigned, "en{'Subset Assigned'}de{'Teilmenge zugeordnet'}");
                    _TourplanPosStatesList.AddEntry((short)TourplanPosStates.Delivered, "en{'Delivered'}de{'Geliefert'}");
                }
                return _TourplanPosStatesList;
            }
        }
#endregion

    }
}




