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
    internal partial class CompanyMaterialHistoryEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.CompanyMaterialHistory",
                typeof(CompanyMaterialHistory),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(CompanyMaterialHistory)));

            var companyMaterialHistoryID = runtimeEntityType.AddProperty(
                "CompanyMaterialHistoryID",
                typeof(Guid),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("CompanyMaterialHistoryID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_CompanyMaterialHistoryID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            companyMaterialHistoryID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var adjustment = runtimeEntityType.AddProperty(
                "Adjustment",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("Adjustment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_Adjustment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            adjustment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var adjustmentAmb = runtimeEntityType.AddProperty(
                "AdjustmentAmb",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("AdjustmentAmb", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_AdjustmentAmb", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            adjustmentAmb.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var companyMaterialID = runtimeEntityType.AddProperty(
                "CompanyMaterialID",
                typeof(Guid),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("CompanyMaterialID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_CompanyMaterialID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            companyMaterialID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var historyID = runtimeEntityType.AddProperty(
                "HistoryID",
                typeof(Guid),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("HistoryID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_HistoryID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            historyID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inward = runtimeEntityType.AddProperty(
                "Inward",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("Inward", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_Inward", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            inward.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inwardAmb = runtimeEntityType.AddProperty(
                "InwardAmb",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("InwardAmb", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_InwardAmb", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            inwardAmb.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lastStockQuantity = runtimeEntityType.AddProperty(
                "LastStockQuantity",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("LastStockQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_LastStockQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            lastStockQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lastStockQuantityAmb = runtimeEntityType.AddProperty(
                "LastStockQuantityAmb",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("LastStockQuantityAmb", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_LastStockQuantityAmb", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            lastStockQuantityAmb.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var minStockQuantity = runtimeEntityType.AddProperty(
                "MinStockQuantity",
                typeof(double?),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("MinStockQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_MinStockQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            minStockQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var optStockQuantity = runtimeEntityType.AddProperty(
                "OptStockQuantity",
                typeof(double?),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("OptStockQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_OptStockQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            optStockQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var outward = runtimeEntityType.AddProperty(
                "Outward",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("Outward", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_Outward", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            outward.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var outwardAmb = runtimeEntityType.AddProperty(
                "OutwardAmb",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("OutwardAmb", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_OutwardAmb", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            outwardAmb.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var reservedInwardQuantity = runtimeEntityType.AddProperty(
                "ReservedInwardQuantity",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("ReservedInwardQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_ReservedInwardQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            reservedInwardQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var reservedOutwardQuantity = runtimeEntityType.AddProperty(
                "ReservedOutwardQuantity",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("ReservedOutwardQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_ReservedOutwardQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            reservedOutwardQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var stockQuantity = runtimeEntityType.AddProperty(
                "StockQuantity",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("StockQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_StockQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            stockQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var stockQuantityAmb = runtimeEntityType.AddProperty(
                "StockQuantityAmb",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("StockQuantityAmb", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_StockQuantityAmb", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            stockQuantityAmb.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetInward = runtimeEntityType.AddProperty(
                "TargetInward",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("TargetInward", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_TargetInward", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            targetInward.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetInwardAmb = runtimeEntityType.AddProperty(
                "TargetInwardAmb",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("TargetInwardAmb", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_TargetInwardAmb", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            targetInwardAmb.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetOutward = runtimeEntityType.AddProperty(
                "TargetOutward",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("TargetOutward", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_TargetOutward", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            targetOutward.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetOutwardAmb = runtimeEntityType.AddProperty(
                "TargetOutwardAmb",
                typeof(double),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("TargetOutwardAmb", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_TargetOutwardAmb", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            targetOutwardAmb.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { companyMaterialHistoryID });
            runtimeEntityType.SetPrimaryKey(key);

            var nCIFKCompanyMaterialHistoryCompanyMaterialID = runtimeEntityType.AddIndex(
                new[] { companyMaterialID },
                name: "NCI_FK_CompanyMaterialHistory_CompanyMaterialID");

            var nCIFKCompanyMaterialHistoryHistoryID = runtimeEntityType.AddIndex(
                new[] { historyID },
                name: "NCI_FK_CompanyMaterialHistory_HistoryID");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("CompanyMaterialID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("CompanyMaterialID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var companyMaterial = declaringEntityType.AddNavigation("CompanyMaterial",
                runtimeForeignKey,
                onDependent: true,
                typeof(CompanyMaterial),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("CompanyMaterial", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_CompanyMaterial", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var companyMaterialHistoryCompanyMaterial = principalEntityType.AddNavigation("CompanyMaterialHistory_CompanyMaterial",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<CompanyMaterialHistory>),
                propertyInfo: typeof(CompanyMaterial).GetProperty("CompanyMaterialHistory_CompanyMaterial", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterial).GetField("_CompanyMaterialHistory_CompanyMaterial", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_CompanyMaterialHistory_CompanyMaterialID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("HistoryID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("HistoryID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var history = declaringEntityType.AddNavigation("History",
                runtimeForeignKey,
                onDependent: true,
                typeof(History),
                propertyInfo: typeof(CompanyMaterialHistory).GetProperty("History", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyMaterialHistory).GetField("_History", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var companyMaterialHistoryHistory = principalEntityType.AddNavigation("CompanyMaterialHistory_History",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<CompanyMaterialHistory>),
                propertyInfo: typeof(History).GetProperty("CompanyMaterialHistory_History", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(History).GetField("_CompanyMaterialHistory_History", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_CompanyMaterialHistory_HistoryID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "CompanyMaterialHistory");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}