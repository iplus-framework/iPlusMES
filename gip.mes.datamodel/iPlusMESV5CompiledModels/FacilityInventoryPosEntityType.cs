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
    internal partial class FacilityInventoryPosEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.FacilityInventoryPos",
                typeof(FacilityInventoryPos),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(FacilityInventoryPos)));

            var facilityInventoryPosID = runtimeEntityType.AddProperty(
                "FacilityInventoryPosID",
                typeof(Guid),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("FacilityInventoryPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_FacilityInventoryPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            facilityInventoryPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment = runtimeEntityType.AddProperty(
                "Comment",
                typeof(string),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_Comment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var facilityChargeID = runtimeEntityType.AddProperty(
                "FacilityChargeID",
                typeof(Guid),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("FacilityChargeID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_FacilityChargeID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            facilityChargeID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var facilityInventoryID = runtimeEntityType.AddProperty(
                "FacilityInventoryID",
                typeof(Guid),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("FacilityInventoryID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_FacilityInventoryID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            facilityInventoryID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDFacilityInventoryPosStateID = runtimeEntityType.AddProperty(
                "MDFacilityInventoryPosStateID",
                typeof(Guid),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("MDFacilityInventoryPosStateID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_MDFacilityInventoryPosStateID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            mDFacilityInventoryPosStateID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var newStockQuantity = runtimeEntityType.AddProperty(
                "NewStockQuantity",
                typeof(double?),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("NewStockQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_NewStockQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            newStockQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var notAvailable = runtimeEntityType.AddProperty(
                "NotAvailable",
                typeof(bool),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("NotAvailable", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_NotAvailable", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            notAvailable.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var sequence = runtimeEntityType.AddProperty(
                "Sequence",
                typeof(int),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("Sequence", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_Sequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            sequence.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var stockQuantity = runtimeEntityType.AddProperty(
                "StockQuantity",
                typeof(double),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("StockQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_StockQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            stockQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { facilityInventoryPosID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { facilityChargeID });

            var index0 = runtimeEntityType.AddIndex(
                new[] { facilityInventoryID });

            var index1 = runtimeEntityType.AddIndex(
                new[] { mDFacilityInventoryPosStateID });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("FacilityChargeID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("FacilityChargeID") }),
                principalEntityType,
                required: true);

            var facilityCharge = declaringEntityType.AddNavigation("FacilityCharge",
                runtimeForeignKey,
                onDependent: true,
                typeof(FacilityCharge),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("FacilityCharge", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_FacilityCharge", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var facilityInventoryPosFacilityCharge = principalEntityType.AddNavigation("FacilityInventoryPos_FacilityCharge",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<FacilityInventoryPos>),
                propertyInfo: typeof(FacilityCharge).GetProperty("FacilityInventoryPos_FacilityCharge", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityCharge).GetField("_FacilityInventoryPos_FacilityCharge", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_FacilityInventoryPos_FacilityChargeID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("FacilityInventoryID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("FacilityInventoryID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var facilityInventory = declaringEntityType.AddNavigation("FacilityInventory",
                runtimeForeignKey,
                onDependent: true,
                typeof(FacilityInventory),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("FacilityInventory", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_FacilityInventory", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var facilityInventoryPosFacilityInventory = principalEntityType.AddNavigation("FacilityInventoryPos_FacilityInventory",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<FacilityInventoryPos>),
                propertyInfo: typeof(FacilityInventory).GetProperty("FacilityInventoryPos_FacilityInventory", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventory).GetField("_FacilityInventoryPos_FacilityInventory", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_FacilityInventoryPos_FacilityInventoryID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDFacilityInventoryPosStateID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDFacilityInventoryPosStateID") }),
                principalEntityType,
                required: true);

            var mDFacilityInventoryPosState = declaringEntityType.AddNavigation("MDFacilityInventoryPosState",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDFacilityInventoryPosState),
                propertyInfo: typeof(FacilityInventoryPos).GetProperty("MDFacilityInventoryPosState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityInventoryPos).GetField("_MDFacilityInventoryPosState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var facilityInventoryPosMDFacilityInventoryPosState = principalEntityType.AddNavigation("FacilityInventoryPos_MDFacilityInventoryPosState",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<FacilityInventoryPos>),
                propertyInfo: typeof(MDFacilityInventoryPosState).GetProperty("FacilityInventoryPos_MDFacilityInventoryPosState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDFacilityInventoryPosState).GetField("_FacilityInventoryPos_MDFacilityInventoryPosState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_FacilityInventoryPos_MDFacilityInventoryPosStateID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "FacilityInventoryPos");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}