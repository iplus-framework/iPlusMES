using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{
#if NETFRAMEWORK
    //[ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESFacilityType, Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListFacilityTypesEnum")]
#else
        [DataContract]
#endif
    public enum FacilityTypesEnum : short
    {
        /// <summary>
        /// Location, store, hall
        /// </summary>
        StorageLocation = 1000,

        /// <summary>
        /// Storage place, bin (Lagerplatz)
        /// </summary>
        StorageBin = 1100,

        /// <summary>
        /// Silo or Tank
        /// </summary>
        StorageBinContainer = 1110,

        /// <summary>
        /// Bin for prepration of intermediate material
        /// </summary>
        PreparationBin = 1120,


        /// <summary>
        /// Virtual Inventory place where Inventory is stored as FacilityCharge
        /// Or Machines that are related to ACClass for OEE-Calculation
        /// </summary>
        MachineOrInventory = 1200,

        /// <summary>
        /// Vehilce
        /// </summary>
        Vehicle = 2000,

        /// <summary>
        /// Silo, Chamber of a Vehicle
        /// </summary>
        VehicleContainer = 2100,
    }

#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESFacilityType, Global.ACKinds.TACEnumACValueList)]
    public class ACValueListFacilityTypesEnum : ACValueItemList
    {
        public ACValueListFacilityTypesEnum() : base("FacilityTypesEnum")
        {
            AddEntry(FacilityTypesEnum.StorageLocation, "en{'Storage Location'}de{'Lagerort'}");
            AddEntry(FacilityTypesEnum.StorageBin, "en{'Storage Bin'}de{'Lagerplatz'}");
            AddEntry(FacilityTypesEnum.StorageBinContainer, "en{'Tank/Silo/Cell/Scale Container'}de{'Tank/Silo/Zelle/Waage-Behältnis'}");
            AddEntry(FacilityTypesEnum.Vehicle, "en{'Vehicle'}de{'Fahrzeug'}");
            AddEntry(FacilityTypesEnum.VehicleContainer, "en{'Vehicle Container'}de{'Fahrzeugbehältnis'}");
        }
    }
#endif
}
