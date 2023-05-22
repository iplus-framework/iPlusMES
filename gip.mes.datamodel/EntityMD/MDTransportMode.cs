using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESTransportMode, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOTransportMode")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDTransportModeIndex", ConstApp.ESTransportMode, typeof(MDTransportMode.TransportModes), Const.ContextDatabase + "\\TransportModesList", "", true, MinValue = (short)MDTransportMode.TransportModes.Truck)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDTransportMode.ClassName, ConstApp.ESTransportMode, typeof(MDTransportMode), MDTransportMode.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDTransportMode>) })]
    [NotMapped]
    public partial class MDTransportMode
    {
        [NotMapped]
        public const string ClassName = "MDTransportMode";

        #region New/Delete
        public static MDTransportMode NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDTransportMode entity = new MDTransportMode();
            entity.MDTransportModeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.TransportMode = TransportModes.Truck;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IEnumerable<MDTransportMode>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IEnumerable<MDTransportMode>>(
            (database) => from c in database.MDTransportMode where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IEnumerable<MDTransportMode>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IEnumerable<MDTransportMode>>(
            (database, index) => from c in database.MDTransportMode where c.MDTransportModeIndex == index select c
        );

        public static MDTransportMode DefaultMDTransportMode(DatabaseApp dbApp)
        {
            try
            {
                MDTransportMode defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)TransportModes.Truck).FirstOrDefault();
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
                return MDTransportModeName;
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
        public String MDTransportModeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDTransportModeName");
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
        /// <summary>
        /// Enum für das Feld VBUnitTypeIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'TransportModes'}de{'TransportModes'}", Global.ACKinds.TACEnum)]
        public enum TransportModes : short
        {
            Truck = 0,

            /// <remarks/>
            Rail = 1,

            /// <remarks/>
            Barge = 2,

            /// <remarks/>
            Pipeline = 3,

            /// <remarks/>
            StorageTank = 4,

            /// <remarks/>
            Vessel = 5,

            /// <remarks/>
            BookTransfer = 6,

            /// <remarks/>
            Container = 7,
        }

        [NotMapped]
        static ACValueItemList _TransportModesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück.
        /// </summary>
        [NotMapped]
        public static ACValueItemList TransportModesList
        {
            get
            {
                if (_TransportModesList == null)
                {
                    _TransportModesList = new ACValueItemList("TransportModes");
                    _TransportModesList.AddEntry((short)TransportModes.Truck, "en{'Truck'}de{'LKW'}");
                    _TransportModesList.AddEntry((short)TransportModes.Rail, "en{'Rail'}de{'Bahn'}");
                    _TransportModesList.AddEntry((short)TransportModes.Barge, "en{'Barge'}de{'Lastkahn'}");
                    _TransportModesList.AddEntry((short)TransportModes.Pipeline, "en{'Pipeline'}de{'Pipeline'}");
                    _TransportModesList.AddEntry((short)TransportModes.StorageTank, "en{'Storage Tank'}de{'Lager-Tank'}");
                    _TransportModesList.AddEntry((short)TransportModes.Vessel, "en{'Vessel'}de{'Kesselwagen'}");
                    _TransportModesList.AddEntry((short)TransportModes.BookTransfer, "en{'Book Transfer'}de{'Umbuchung'}");
                    _TransportModesList.AddEntry((short)TransportModes.Container, "en{'Container'}de{'Container'}");
                }
                return _TransportModesList;
            }
        }

        [NotMapped]
        public TransportModes TransportMode
        {
            get
            {
                return (TransportModes)MDTransportModeIndex;
            }
            set
            {
                MDTransportModeIndex = (Int16)value;
                OnPropertyChanged("TransportMode");
            }
        }
#endregion

    }
}




