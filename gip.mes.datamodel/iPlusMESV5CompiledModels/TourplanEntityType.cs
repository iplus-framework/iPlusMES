﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using gip.core.datamodel;

#pragma warning disable 219, 612, 618
#nullable disable

namespace gip.mes.datamodel
{
    internal partial class TourplanEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.Tourplan",
                typeof(Tourplan),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(Tourplan)));

            var tourplanID = runtimeEntityType.AddProperty(
                "TourplanID",
                typeof(Guid),
                propertyInfo: typeof(Tourplan).GetProperty("TourplanID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_TourplanID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            tourplanID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var actDeliveryDate = runtimeEntityType.AddProperty(
                "ActDeliveryDate",
                typeof(DateTime?),
                propertyInfo: typeof(Tourplan).GetProperty("ActDeliveryDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_ActDeliveryDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            actDeliveryDate.AddAnnotation("Relational:ColumnType", "datetime");
            actDeliveryDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var companyID = runtimeEntityType.AddProperty(
                "CompanyID",
                typeof(Guid),
                propertyInfo: typeof(Tourplan).GetProperty("CompanyID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_CompanyID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            companyID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deliveryDate = runtimeEntityType.AddProperty(
                "DeliveryDate",
                typeof(DateTime?),
                propertyInfo: typeof(Tourplan).GetProperty("DeliveryDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_DeliveryDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            deliveryDate.AddAnnotation("Relational:ColumnType", "datetime");
            deliveryDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var firstWeighing = runtimeEntityType.AddProperty(
                "FirstWeighing",
                typeof(double),
                propertyInfo: typeof(Tourplan).GetProperty("FirstWeighing", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_FirstWeighing", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            firstWeighing.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var firstWeighingIdentityNo = runtimeEntityType.AddProperty(
                "FirstWeighingIdentityNo",
                typeof(string),
                propertyInfo: typeof(Tourplan).GetProperty("FirstWeighingIdentityNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_FirstWeighingIdentityNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 20,
                unicode: false);
            firstWeighingIdentityNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(Tourplan).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(Tourplan).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lastWeighing = runtimeEntityType.AddProperty(
                "LastWeighing",
                typeof(double),
                propertyInfo: typeof(Tourplan).GetProperty("LastWeighing", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_LastWeighing", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            lastWeighing.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lastWeighingIdentityNo = runtimeEntityType.AddProperty(
                "LastWeighingIdentityNo",
                typeof(string),
                propertyInfo: typeof(Tourplan).GetProperty("LastWeighingIdentityNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_LastWeighingIdentityNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 20,
                unicode: false);
            lastWeighingIdentityNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var loadingStation = runtimeEntityType.AddProperty(
                "LoadingStation",
                typeof(string),
                propertyInfo: typeof(Tourplan).GetProperty("LoadingStation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_LoadingStation", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 20,
                unicode: false);
            loadingStation.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDTourID = runtimeEntityType.AddProperty(
                "MDTourID",
                typeof(Guid?),
                propertyInfo: typeof(Tourplan).GetProperty("MDTourID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_MDTourID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            mDTourID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDTourplanStateID = runtimeEntityType.AddProperty(
                "MDTourplanStateID",
                typeof(Guid),
                propertyInfo: typeof(Tourplan).GetProperty("MDTourplanStateID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_MDTourplanStateID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            mDTourplanStateID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maxCapacityDiff = runtimeEntityType.AddProperty(
                "MaxCapacityDiff",
                typeof(double?),
                propertyInfo: typeof(Tourplan).GetProperty("MaxCapacityDiff", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_MaxCapacityDiff", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            maxCapacityDiff.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maxCapacitySum = runtimeEntityType.AddProperty(
                "MaxCapacitySum",
                typeof(double),
                propertyInfo: typeof(Tourplan).GetProperty("MaxCapacitySum", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_MaxCapacitySum", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            maxCapacitySum.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maxWeightDiff = runtimeEntityType.AddProperty(
                "MaxWeightDiff",
                typeof(double),
                propertyInfo: typeof(Tourplan).GetProperty("MaxWeightDiff", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_MaxWeightDiff", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            maxWeightDiff.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maxWeightSum = runtimeEntityType.AddProperty(
                "MaxWeightSum",
                typeof(double),
                propertyInfo: typeof(Tourplan).GetProperty("MaxWeightSum", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_MaxWeightSum", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            maxWeightSum.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var netWeight = runtimeEntityType.AddProperty(
                "NetWeight",
                typeof(double),
                propertyInfo: typeof(Tourplan).GetProperty("NetWeight", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_NetWeight", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            netWeight.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var nightLoading = runtimeEntityType.AddProperty(
                "NightLoading",
                typeof(int),
                propertyInfo: typeof(Tourplan).GetProperty("NightLoading", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_NightLoading", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            nightLoading.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var periodInt = runtimeEntityType.AddProperty(
                "PeriodInt",
                typeof(int),
                propertyInfo: typeof(Tourplan).GetProperty("PeriodInt", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_PeriodInt", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            periodInt.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var secondWeighing = runtimeEntityType.AddProperty(
                "SecondWeighing",
                typeof(double),
                propertyInfo: typeof(Tourplan).GetProperty("SecondWeighing", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_SecondWeighing", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            secondWeighing.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var secondWeighingIdentityNo = runtimeEntityType.AddProperty(
                "SecondWeighingIdentityNo",
                typeof(string),
                propertyInfo: typeof(Tourplan).GetProperty("SecondWeighingIdentityNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_SecondWeighingIdentityNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 20,
                unicode: false);
            secondWeighingIdentityNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var tourplanName = runtimeEntityType.AddProperty(
                "TourplanName",
                typeof(string),
                propertyInfo: typeof(Tourplan).GetProperty("TourplanName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_TourplanName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 30,
                unicode: false);
            tourplanName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var tourplanNo = runtimeEntityType.AddProperty(
                "TourplanNo",
                typeof(string),
                propertyInfo: typeof(Tourplan).GetProperty("TourplanNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_TourplanNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            tourplanNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var trailerFacilityID = runtimeEntityType.AddProperty(
                "TrailerFacilityID",
                typeof(Guid?),
                propertyInfo: typeof(Tourplan).GetProperty("TrailerFacilityID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_TrailerFacilityID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            trailerFacilityID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(Tourplan).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(Tourplan).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vehicleFacilityID = runtimeEntityType.AddProperty(
                "VehicleFacilityID",
                typeof(Guid?),
                propertyInfo: typeof(Tourplan).GetProperty("VehicleFacilityID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_VehicleFacilityID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            vehicleFacilityID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var visitorVoucherID = runtimeEntityType.AddProperty(
                "VisitorVoucherID",
                typeof(Guid?),
                propertyInfo: typeof(Tourplan).GetProperty("VisitorVoucherID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_VisitorVoucherID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            visitorVoucherID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(Tourplan).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { tourplanID });
            runtimeEntityType.SetPrimaryKey(key);

            var nCIFKTourplanCompanyID = runtimeEntityType.AddIndex(
                new[] { companyID },
                name: "NCI_FK_Tourplan_CompanyID");

            var nCIFKTourplanMDTourID = runtimeEntityType.AddIndex(
                new[] { mDTourID },
                name: "NCI_FK_Tourplan_MDTourID");

            var nCIFKTourplanMDTourplanStateID = runtimeEntityType.AddIndex(
                new[] { mDTourplanStateID },
                name: "NCI_FK_Tourplan_MDTourplanStateID");

            var nCIFKTourplanTrailerFacilityID = runtimeEntityType.AddIndex(
                new[] { trailerFacilityID },
                name: "NCI_FK_Tourplan_TrailerFacilityID");

            var nCIFKTourplanVehicleFacilityID = runtimeEntityType.AddIndex(
                new[] { vehicleFacilityID },
                name: "NCI_FK_Tourplan_VehicleFacilityID");

            var nCIFKTourplanVisitorVoucherID = runtimeEntityType.AddIndex(
                new[] { visitorVoucherID },
                name: "NCI_FK_Tourplan_VisitorVoucherID");

            var uIXTourplan = runtimeEntityType.AddIndex(
                new[] { tourplanNo },
                name: "UIX_Tourplan",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("CompanyID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("CompanyID") }),
                principalEntityType,
                required: true);

            var company = declaringEntityType.AddNavigation("Company",
                runtimeForeignKey,
                onDependent: true,
                typeof(Company),
                propertyInfo: typeof(Tourplan).GetProperty("Company", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_Company", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tourplanCompany = principalEntityType.AddNavigation("Tourplan_Company",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Tourplan>),
                propertyInfo: typeof(Company).GetProperty("Tourplan_Company", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Company).GetField("_Tourplan_Company", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_Tourplan_CompanyID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDTourID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDTourID") }),
                principalEntityType);

            var mDTour = declaringEntityType.AddNavigation("MDTour",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDTour),
                propertyInfo: typeof(Tourplan).GetProperty("MDTour", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_MDTour", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tourplanMDTour = principalEntityType.AddNavigation("Tourplan_MDTour",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Tourplan>),
                propertyInfo: typeof(MDTour).GetProperty("Tourplan_MDTour", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDTour).GetField("_Tourplan_MDTour", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_Tourplan_MDTourID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDTourplanStateID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDTourplanStateID") }),
                principalEntityType,
                required: true);

            var mDTourplanState = declaringEntityType.AddNavigation("MDTourplanState",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDTourplanState),
                propertyInfo: typeof(Tourplan).GetProperty("MDTourplanState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_MDTourplanState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tourplanMDTourplanState = principalEntityType.AddNavigation("Tourplan_MDTourplanState",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Tourplan>),
                propertyInfo: typeof(MDTourplanState).GetProperty("Tourplan_MDTourplanState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDTourplanState).GetField("_Tourplan_MDTourplanState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_Tourplan_MDCommissionPlanStateID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey4(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("TrailerFacilityID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("FacilityID") }),
                principalEntityType);

            var trailerFacility = declaringEntityType.AddNavigation("TrailerFacility",
                runtimeForeignKey,
                onDependent: true,
                typeof(Facility),
                propertyInfo: typeof(Tourplan).GetProperty("TrailerFacility", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_TrailerFacility", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tourplanTrailerFacility = principalEntityType.AddNavigation("Tourplan_TrailerFacility",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Tourplan>),
                propertyInfo: typeof(Facility).GetProperty("Tourplan_TrailerFacility", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Facility).GetField("_Tourplan_TrailerFacility", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_Tourplan_TrailerFacilityID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey5(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VehicleFacilityID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("FacilityID") }),
                principalEntityType);

            var vehicleFacility = declaringEntityType.AddNavigation("VehicleFacility",
                runtimeForeignKey,
                onDependent: true,
                typeof(Facility),
                propertyInfo: typeof(Tourplan).GetProperty("VehicleFacility", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_VehicleFacility", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tourplanVehicleFacility = principalEntityType.AddNavigation("Tourplan_VehicleFacility",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Tourplan>),
                propertyInfo: typeof(Facility).GetProperty("Tourplan_VehicleFacility", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Facility).GetField("_Tourplan_VehicleFacility", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_Tourplan_VehicleFacilityID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey6(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VisitorVoucherID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("VisitorVoucherID") }),
                principalEntityType);

            var visitorVoucher = declaringEntityType.AddNavigation("VisitorVoucher",
                runtimeForeignKey,
                onDependent: true,
                typeof(VisitorVoucher),
                propertyInfo: typeof(Tourplan).GetProperty("VisitorVoucher", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tourplan).GetField("_VisitorVoucher", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tourplanVisitorVoucher = principalEntityType.AddNavigation("Tourplan_VisitorVoucher",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Tourplan>),
                propertyInfo: typeof(VisitorVoucher).GetProperty("Tourplan_VisitorVoucher", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VisitorVoucher).GetField("_Tourplan_VisitorVoucher", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_Tourplan_VisitorVoucherID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Tourplan");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}