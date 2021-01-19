using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESFacilityVehicleType, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOFacilityVehicleType")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDFacilityVehicleTypeIndex", ConstApp.ESFacilityVehicleType, typeof(MDFacilityVehicleType.FacilityVehicleTypes), Const.ContextDatabase + "\\FacilityVehicleTypesList", "", true, MinValue = (short)MDFacilityVehicleType.FacilityVehicleTypes.CalibrationVehicle)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDFacilityVehicleType.ClassName, ConstApp.ESFacilityVehicleType, typeof(MDFacilityVehicleType), MDFacilityVehicleType.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDFacilityVehicleType>) })]
    public partial class MDFacilityVehicleType
    {
        public const string ClassName = "MDFacilityVehicleType";

        #region New/Delete
        public static MDFacilityVehicleType NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDFacilityVehicleType entity = new MDFacilityVehicleType();
            entity.MDFacilityVehicleTypeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.FacilityVehicleType = FacilityVehicleTypes.CalibrationVehicle;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDFacilityVehicleType>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDFacilityVehicleType>>(
            (database) => from c in database.MDFacilityVehicleType where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDFacilityVehicleType>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDFacilityVehicleType>>(
            (database, index) => from c in database.MDFacilityVehicleType where c.MDFacilityVehicleTypeIndex == index select c
        );

        public static MDFacilityVehicleType DefaultMDFacilityVehicleType(DatabaseApp dbApp)
        {
            try
            {
                MDFacilityVehicleType defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)FacilityVehicleTypes.CalibrationVehicle).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDFacilityVehicleType", "DefaultMDFacilityVehicleType", msg);
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
                return MDFacilityVehicleTypeName;
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
        public String MDFacilityVehicleTypeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDFacilityVehicleTypeName");
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
        /// <summary>
        /// Enum für das Feld VBUnitTypeIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'FacilityVehicleTypes'}de{'FacilityVehicleTypes'}", Global.ACKinds.TACEnum)]
        public enum FacilityVehicleTypes : short
        {
            CalibrationVehicle = 1, // Eichfahrzeug

            Bicycles = 1000,

            Cars = 2000,

            Trucks = 3000,

            SmallTrucks = 3100,
            MiniTruck = 3101,

            LightTrucks = 3200,
            Minivan = 3201,
            SportUtilityVehicle = 3202,
            CanopyExpress = 3203,
            PickupTruck = 3204,
            PanelTruck = 3205,
            CabForward = 3206,
            TowTruck = 3207,
            PanelVan = 3208,
            SedanDelivery = 3209,

            MediumTrucks = 3300,
            BoxTruck = 3301,
            Van = 3302,
            CutawayVanChassis = 3303,
            MediumDutyTruck = 3304,
            MediumStandardTruck = 3305,
            PlatformTruck = 3306,
            FlatbedTruck = 3307,
            FireTruck = 3308,
            RecreationalVehicle = 3309,

            HeavyTrucks = 3400,
            BallastTractor = 3401, // Motorwagen
            TruckMixer = 3402, // Fahrmischer
            CraneTruck = 3403,
            DumpTruck = 3404,
            GarbageTruck = 3405,
            LogCarrier = 3406,
            RefrigeratorTruck = 3407,
            SemiTrailerTruck = 3408, // Sattelzug
            TankTruck = 3409, // Tankwagen

            VeryHeavyTrucks = 3500,
            HeavyBallastTractor = 3501,
            HeavyHauler = 3402,

            Buses = 4000,

            Motorcycles = 5000,

            Trains = 6000,

            Ships = 7100,
            Boats = 7200,

            Aircraft = 8000,

            Trailer = 9000,
            RigidDrawbarTrailers = 9001, // Starrdeichselanhänger (SDAH)
            SemiTrailer = 9002, // Auflieger, Sattelanhänger
            FullTrailer = 9003 // Mehrachsige Anhänger mit gelenkter Vorderachse
        }

        static ACValueItemList _FacilityVehicleTypesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList FacilityVehicleTypesList
        {
            get
            {
                if (_FacilityVehicleTypesList == null)
                {
                    _FacilityVehicleTypesList = new ACValueItemList("FacilityVehicleTypes");
                    _FacilityVehicleTypesList.AddEntry((short)FacilityVehicleTypes.CalibrationVehicle, "en{'Calibration Vehicle'}de{'Eichfahrzeug'}");
                    _FacilityVehicleTypesList.AddEntry((short)FacilityVehicleTypes.SmallTrucks, "en{'Small Truck'}de{'Kleinlaster'}");
                    _FacilityVehicleTypesList.AddEntry((short)FacilityVehicleTypes.BoxTruck, "en{'Box Truck'}de{'Kastenwagen'}");
                    _FacilityVehicleTypesList.AddEntry((short)FacilityVehicleTypes.BallastTractor, "en{'Ballast Tractor'}de{'Schwertransporter'}");
                    _FacilityVehicleTypesList.AddEntry((short)FacilityVehicleTypes.TruckMixer, "en{'Truck Mixer'}de{'Fahrmischer'}");
                    _FacilityVehicleTypesList.AddEntry((short)FacilityVehicleTypes.RefrigeratorTruck, "en{'Refrigerated Truck'}de{'Kühltransporter'}");
                    _FacilityVehicleTypesList.AddEntry((short)FacilityVehicleTypes.SemiTrailerTruck, "en{'Semi Trailer Truck'}de{'Sattelzugmaschine'}");
                    _FacilityVehicleTypesList.AddEntry((short)FacilityVehicleTypes.TankTruck, "en{'Tank Truck'}de{'Tankwagen'}");
                    _FacilityVehicleTypesList.AddEntry((short)FacilityVehicleTypes.Trailer, "en{'Trailer'}de{'Anhänger'}");
                    _FacilityVehicleTypesList.AddEntry((short)FacilityVehicleTypes.RigidDrawbarTrailers, "en{'Rigid Drawbar Trailers'}de{'Starrdeichselanhänger'}");
                    _FacilityVehicleTypesList.AddEntry((short)FacilityVehicleTypes.SemiTrailer, "en{'Semi Trailer'}de{'Sattelauflieger'}");
                    _FacilityVehicleTypesList.AddEntry((short)FacilityVehicleTypes.FullTrailer, "en{'Full Trailer'}de{'Mehrachsiger Anhänger mit gelenkter Vorderachse'}");
                }
                return _FacilityVehicleTypesList;
            }
        }

        public FacilityVehicleTypes FacilityVehicleType
        {
            get
            {
                return (FacilityVehicleTypes)MDFacilityVehicleTypeIndex;
            }
            set
            {
                MDFacilityVehicleTypeIndex = (Int16)value;
                OnPropertyChanged("MDFacilityVehicleType");
            }
        }
#endregion

    }
}




