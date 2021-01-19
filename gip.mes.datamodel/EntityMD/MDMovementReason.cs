using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESMovementReason, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOMovementReason")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDMovementReasonIndex", ConstApp.ESMovementReason, typeof(MDMovementReason.MovementReasons), Const.ContextDatabase + "\\MovementReasonsList", "", true, MinValue = (short)MovementReasons.Adjustment)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDMovementReason.ClassName, ConstApp.ESMovementReason, typeof(MDMovementReason), MDMovementReason.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDMovementReason>) })]
    public partial class MDMovementReason
    {
        public const string ClassName = "MDMovementReason";

        #region New/Delete
        public static MDMovementReason NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDMovementReason entity = new MDMovementReason();
            entity.MDMovementReasonID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.MovementReason = MovementReasons.Adjustment;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }

        static readonly Func<DatabaseApp, IQueryable<MDMovementReason>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDMovementReason>>(
            (database) => from c in database.MDMovementReason where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDMovementReason>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDMovementReason>>(
            (database, index) => from c in database.MDMovementReason where c.MDMovementReasonIndex == index select c
        );

        public static MDMovementReason DefaultMDMovementReason(DatabaseApp dbApp)
        {
            try
            {
                MDMovementReason defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)MovementReasons.Adjustment).FirstOrDefault();
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
        public override string ACCaption
        {
            get
            {
                return MDMovementReasonName;
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
        [ACPropertyInfo(1, "", "en{'Movement Reason'}de{'Bewegungsgrund'}", MinLength = 1)]
        public String MDMovementReasonName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDMovementReasonName");
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
        public MovementReasons MovementReason
        {
            get
            {
                return (MovementReasons)MDMovementReasonIndex;
            }
            set
            {
                MDMovementReasonIndex = (short)value;
                OnPropertyChanged("MovementReason");
            }
        }

        /// <summary>
        /// Enum für das Feld MDMovementReasonIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'MovementReasons'}de{'MovementReasons'}", Global.ACKinds.TACEnum)]
        public enum MovementReasons : short
        {
            Adjustment = 1, //Korrektur
            ZeroStock = 2, //Nullbestand
            Enabling = 3, //Freigabe
            Blocking = 4, //Sperrung            
            Consumption = 5, //Verbrauch
            Production = 6, //Herstellung
            GoodsReceipt = 7, //Wareneingang
            GoodsIssue = 8, //Warenausgang           
            Relocation = 9, //Warenumlagerung
            Inventory = 10, //Inventur
            ConsumptionWithoutBalance = 11, //Verbrauch ohne Bilanz
            ProductionWithoutBalance = 12, //Herstellung ohne Bilanz
            CorrectionFromERP = 90
        }

        static ACValueItemList _MovementReasonsList = null;

        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList MovementReasonsList
        {
            get
            {
                if (_MovementReasonsList == null)
                {
                    _MovementReasonsList = new ACValueItemList("MovementReasons");
                    _MovementReasonsList.AddEntry(MovementReasons.Adjustment, "en{'Adjustment'}de{'Korrektur'}");
                    _MovementReasonsList.AddEntry(MovementReasons.ZeroStock, "en{'Zero Stock'}de{'Nullbestand'}");
                    _MovementReasonsList.AddEntry(MovementReasons.Enabling, "en{'Enabling'}de{'Freigabe'}");
                    _MovementReasonsList.AddEntry(MovementReasons.Blocking, "en{'Blocking'}de{'Sperrung'}");
                    _MovementReasonsList.AddEntry(MovementReasons.Consumption, "en{'Consumption'}de{'Verbrauch'}");
                    _MovementReasonsList.AddEntry(MovementReasons.Production, "en{'Production'}de{'Herstellung'}");
                    _MovementReasonsList.AddEntry(MovementReasons.GoodsReceipt, "en{'Goods Receipt'}de{'Wareneingang'}");
                    _MovementReasonsList.AddEntry(MovementReasons.GoodsIssue, "en{'Goods Issue'}de{'Warenausgang'}");
                    _MovementReasonsList.AddEntry(MovementReasons.Relocation, "en{'Relocation'}de{'Umlagerung'}");
                    _MovementReasonsList.AddEntry(MovementReasons.Inventory, "en{'Inventory'}de{'Inventur'}");
                    _MovementReasonsList.AddEntry(MovementReasons.ConsumptionWithoutBalance, "en{'Consumption Without Balance'}de{'Verbrauch ohne Bilanz'}");
                    _MovementReasonsList.AddEntry(MovementReasons.ProductionWithoutBalance, "en{'Production Without Balance'}de{'Herstellung ohne Bilanz'}");
                }
                return _MovementReasonsList;
            }
        }
        #endregion

    }
}




