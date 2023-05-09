using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESFacilityType, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOFacilityType")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(3, "MDFacilityTypeIndex", ConstApp.ESFacilityType, typeof(FacilityTypesEnum), Const.ContextDatabase + "\\FacilityTypesList", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDFacilityType.ClassName, ConstApp.ESFacilityType, typeof(MDFacilityType), MDFacilityType.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDFacilityType>) })]
    public partial class MDFacilityType
    {
        public const string ClassName = "MDFacilityType";

        #region New/Delete
        public static MDFacilityType NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDFacilityType entity = new MDFacilityType();
            entity.MDFacilityTypeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.FacilityType = FacilityTypesEnum.StorageBinContainer;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDFacilityType>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDFacilityType>>(
            (database) => from c in database.MDFacilityType where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDFacilityType>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDFacilityType>>(
            (database, index) => from c in database.MDFacilityType where c.MDFacilityTypeIndex == index select c
        );

        public static MDFacilityType DefaultMDFacilityType(DatabaseApp dbApp)
        {
            try
            {
                MDFacilityType defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)FacilityTypesEnum.StorageLocation).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDFacilityType", "DefaultMDFacilityType", msg);
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
                return MDFacilityTypeName;
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
        public String MDFacilityTypeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDFacilityTypeName");
            }
        }

        [ACPropertyInfo(9999, "", "en{'Automatic charge control'}de{'Automatische Ladekontrolle'}")]
        [NotMapped]
        public bool AutomaticControlFacilityCharge
        {
            get
            {
                return MDFacilityTypeIndex != (short)FacilityTypesEnum.StorageBin;
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
        //        /// <summary>
        //        /// Enum für das Feld VBUnitTypeIndex
        //        /// </summary>
        //        [ACClassInfo(Const.PackName_VarioSystem, "en{'FacilityTypes'}de{'FacilityTypes'}", Global.ACKinds.TACEnum)]
        //        public enum FacilityTypes : short
        //        {
        //            /// <summary>
        //            /// Location, store, hall
        //            /// </summary>
        //            StorageLocation = 1000,

        //            /// <summary>
        //            /// Storage place, bin (Lagerplatz)
        //            /// </summary>
        //            StorageBin = 1100,

        //            /// <summary>
        //            /// Silo or Tank
        //            /// </summary>
        //            StorageBinContainer = 1110, 

        //            /// <summary>
        //            /// Bin for prepration of imermediate material
        //            /// </summary>
        //            PreparationBin = 1120,

        //            /// <summary>
        //            /// Vehilce
        //            /// </summary>
        //            Vehicle = 2000,

        //            /// <summary>
        //            /// Silo, Chamber of a Vehicle
        //            /// </summary>
        //            VehicleContainer = 2100,
        //        }

        [NotMapped]
        public FacilityTypesEnum FacilityType
        {
            get
            {
                return (FacilityTypesEnum)MDFacilityTypeIndex;
            }
            set
            {
                MDFacilityTypeIndex = (Int16)value;
                OnPropertyChanged("MDFacilityType");
            }
        }

        //        static ACValueItemList _FacilityTypesList = null;

        //        public static ACValueItemList FacilityTypesList
        //        {
        //            get
        //            {
        //                if (_FacilityTypesList == null)
        //                {
        //                    _FacilityTypesList = new ACValueItemList("FacilityTypes");
        //                    _FacilityTypesList.AddEntry(FacilityTypes.StorageLocation, "en{'Storage Location'}de{'Lagerort'}");
        //                    _FacilityTypesList.AddEntry(FacilityTypes.StorageBin, "en{'Storage Bin'}de{'Lagerplatz'}");
        //                    _FacilityTypesList.AddEntry(FacilityTypes.StorageBinContainer, "en{'Tank/Silo/Cell/Scale Container'}de{'Tank/Silo/Zelle/Waage-Behältnis'}");
        //                    _FacilityTypesList.AddEntry(FacilityTypes.Vehicle, "en{'Vehicle'}de{'Fahrzeug'}");
        //                    _FacilityTypesList.AddEntry(FacilityTypes.VehicleContainer, "en{'Vehicle Container'}de{'Fahrzeugbehältnis'}");
        //                }
        //                return _FacilityTypesList;
        //            }
        //        }
        #endregion

    }
}
