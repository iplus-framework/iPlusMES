using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.ESDelivPosState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOTourState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDDelivPosStateIndex", ConstApp.ESDelivPosState, typeof(MDDelivPosState.DelivPosStates), Const.ContextDatabase + "\\DelivPosStatesList", "", true, MinValue = (short)DelivPosStates.NotPlanned)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + MDDelivPosState.ClassName, ConstApp.ESDelivPosState, typeof(MDDelivPosState), MDDelivPosState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDDelivPosState>) })]
    public partial class MDDelivPosState
    {
        public const string ClassName = "MDDelivPosState";

        #region New/Delete
        public static MDDelivPosState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDDelivPosState entity = new MDDelivPosState();
            entity.MDDelivPosStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.DelivPosState = DelivPosStates.NotPlanned;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDDelivPosState>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDDelivPosState>>(
            (database) => from c in database.MDDelivPosState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDDelivPosState>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDDelivPosState>>(
            (database, index) => from c in database.MDDelivPosState where c.MDDelivPosStateIndex == index select c
        );

        public static MDDelivPosState DefaultMDDelivPosState(DatabaseApp dbApp)
        {
            try
            {
                MDDelivPosState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)DelivPosStates.NotPlanned).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDDelivPosState", "DefaultMDDelivPosState", msg);
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
        public override string ACCaption
        {
            get
            {
                return MDDelivPosStateName;
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

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        public String MDDelivPosStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDDelivPosStateName");
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

#region enums
        public DelivPosStates DelivPosState
        {
            get
            {
                return (DelivPosStates)MDDelivPosStateIndex;
            }
            set
            {
                MDDelivPosStateIndex = (short)value;
                OnPropertyChanged("DelivPosState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDDelivPosStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'Delivery Status'}de{'Lieferstatus'}", Global.ACKinds.TACEnum)]
        public enum DelivPosStates : short
        {
            NotPlanned = 1, //nicht geplant
            SubsetAssigned = 2, //Teilmenge zugeordnet
            CompletelyAssigned = 3, //Vollständig zugeordnet
            Delivered = 4, //Geliefert
        }

        static ACValueItemList _DelivPosStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList DelivPosStatesList
        {
            get
            {
                if (_DelivPosStatesList == null)
                {
                    _DelivPosStatesList = new ACValueItemList("DelivPosStates");
                    _DelivPosStatesList.AddEntry((short)DelivPosStates.NotPlanned, "en{'Not Planned'}de{'Nicht geplant'}");
                    _DelivPosStatesList.AddEntry((short)DelivPosStates.SubsetAssigned, "en{'Subset Assigned'}de{'Teilmenge zugeordnet'}");
                    _DelivPosStatesList.AddEntry((short)DelivPosStates.CompletelyAssigned, "en{'Completely Assigned'}de{'Vollständig zugeordnet'}");
                    _DelivPosStatesList.AddEntry((short)DelivPosStates.Delivered, "en{'Delivered'}de{'Geliefert'}");
                }
                return _DelivPosStatesList;
            }
        }
#endregion
    }
}



