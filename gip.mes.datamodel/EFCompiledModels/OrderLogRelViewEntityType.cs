﻿// <auto-generated />
using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;

#pragma warning disable 219, 612, 618
#nullable disable

namespace gip.mes.datamodel
{
    internal partial class OrderLogRelViewEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.OrderLogRelView",
                typeof(OrderLogRelView),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(OrderLogRelView)));

            var aCClassID = runtimeEntityType.AddProperty(
                "ACClassID",
                typeof(Guid),
                propertyInfo: typeof(OrderLogRelView).GetProperty("ACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_ACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            aCClassID.TypeMapping = GuidTypeMapping.Default.Clone(
                comparer: new ValueComparer<Guid>(
                    (Guid v1, Guid v2) => v1 == v2,
                    (Guid v) => v.GetHashCode(),
                    (Guid v) => v),
                keyComparer: new ValueComparer<Guid>(
                    (Guid v1, Guid v2) => v1 == v2,
                    (Guid v) => v.GetHashCode(),
                    (Guid v) => v),
                providerValueComparer: new ValueComparer<Guid>(
                    (Guid v1, Guid v2) => v1 == v2,
                    (Guid v) => v.GetHashCode(),
                    (Guid v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "uniqueidentifier"));
            aCClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var aCProgramLogID = runtimeEntityType.AddProperty(
                "ACProgramLogID",
                typeof(Guid),
                propertyInfo: typeof(OrderLogRelView).GetProperty("ACProgramLogID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_ACProgramLogID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            aCProgramLogID.TypeMapping = GuidTypeMapping.Default.Clone(
                comparer: new ValueComparer<Guid>(
                    (Guid v1, Guid v2) => v1 == v2,
                    (Guid v) => v.GetHashCode(),
                    (Guid v) => v),
                keyComparer: new ValueComparer<Guid>(
                    (Guid v1, Guid v2) => v1 == v2,
                    (Guid v) => v.GetHashCode(),
                    (Guid v) => v),
                providerValueComparer: new ValueComparer<Guid>(
                    (Guid v1, Guid v2) => v1 == v2,
                    (Guid v) => v.GetHashCode(),
                    (Guid v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "uniqueidentifier"));
            aCProgramLogID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var aCUrl = runtimeEntityType.AddProperty(
                "ACUrl",
                typeof(string),
                propertyInfo: typeof(OrderLogRelView).GetProperty("ACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_ACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 250,
                unicode: false);
            aCUrl.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(250)",
                    size: 250));
            aCUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var actualQuantityUOM = runtimeEntityType.AddProperty(
                "ActualQuantityUOM",
                typeof(double),
                propertyInfo: typeof(OrderLogRelView).GetProperty("ActualQuantityUOM", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_ActualQuantityUOM", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            actualQuantityUOM.TypeMapping = SqlServerDoubleTypeMapping.Default.Clone(
                comparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v),
                keyComparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v),
                providerValueComparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v));
            actualQuantityUOM.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var basedOnACClassID = runtimeEntityType.AddProperty(
                "BasedOnACClassID",
                typeof(Guid?),
                propertyInfo: typeof(OrderLogRelView).GetProperty("BasedOnACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_BasedOnACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            basedOnACClassID.TypeMapping = GuidTypeMapping.Default.Clone(
                comparer: new ValueComparer<Guid?>(
                    (Nullable<Guid> v1, Nullable<Guid> v2) => v1.HasValue && v2.HasValue && (Guid)v1 == (Guid)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<Guid> v) => v.HasValue ? ((Guid)v).GetHashCode() : 0,
                    (Nullable<Guid> v) => v.HasValue ? (Nullable<Guid>)(Guid)v : default(Nullable<Guid>)),
                keyComparer: new ValueComparer<Guid?>(
                    (Nullable<Guid> v1, Nullable<Guid> v2) => v1.HasValue && v2.HasValue && (Guid)v1 == (Guid)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<Guid> v) => v.HasValue ? ((Guid)v).GetHashCode() : 0,
                    (Nullable<Guid> v) => v.HasValue ? (Nullable<Guid>)(Guid)v : default(Nullable<Guid>)),
                providerValueComparer: new ValueComparer<Guid?>(
                    (Nullable<Guid> v1, Nullable<Guid> v2) => v1.HasValue && v2.HasValue && (Guid)v1 == (Guid)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<Guid> v) => v.HasValue ? ((Guid)v).GetHashCode() : 0,
                    (Nullable<Guid> v) => v.HasValue ? (Nullable<Guid>)(Guid)v : default(Nullable<Guid>)),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "uniqueidentifier"));
            basedOnACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var basedOnMachine = runtimeEntityType.AddProperty(
                "BasedOnMachine",
                typeof(string),
                propertyInfo: typeof(OrderLogRelView).GetProperty("BasedOnMachine", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_BasedOnMachine", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 100,
                unicode: false);
            basedOnMachine.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(100)",
                    size: 100));
            basedOnMachine.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(OrderLogRelView).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            insertDate.TypeMapping = SqlServerDateTimeTypeMapping.Default.Clone(
                comparer: new ValueComparer<DateTime>(
                    (DateTime v1, DateTime v2) => v1.Equals(v2),
                    (DateTime v) => v.GetHashCode(),
                    (DateTime v) => v),
                keyComparer: new ValueComparer<DateTime>(
                    (DateTime v1, DateTime v2) => v1.Equals(v2),
                    (DateTime v) => v.GetHashCode(),
                    (DateTime v) => v),
                providerValueComparer: new ValueComparer<DateTime>(
                    (DateTime v1, DateTime v2) => v1.Equals(v2),
                    (DateTime v) => v.GetHashCode(),
                    (DateTime v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "datetime",
                    dbType: System.Data.DbType.DateTime));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var machineName = runtimeEntityType.AddProperty(
                "MachineName",
                typeof(string),
                propertyInfo: typeof(OrderLogRelView).GetProperty("MachineName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_MachineName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 100,
                unicode: false);
            machineName.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(100)",
                    size: 100));
            machineName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var materialName1 = runtimeEntityType.AddProperty(
                "MaterialName1",
                typeof(string),
                propertyInfo: typeof(OrderLogRelView).GetProperty("MaterialName1", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_MaterialName1", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 350,
                unicode: false);
            materialName1.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(350)",
                    size: 350));
            materialName1.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var materialNo = runtimeEntityType.AddProperty(
                "MaterialNo",
                typeof(string),
                propertyInfo: typeof(OrderLogRelView).GetProperty("MaterialNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_MaterialNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 30,
                unicode: false);
            materialNo.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(30)",
                    size: 30));
            materialNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var materialPosTypeIndex = runtimeEntityType.AddProperty(
                "MaterialPosTypeIndex",
                typeof(short),
                propertyInfo: typeof(OrderLogRelView).GetProperty("MaterialPosTypeIndex", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_MaterialPosTypeIndex", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: (short)0);
            materialPosTypeIndex.TypeMapping = SqlServerShortTypeMapping.Default.Clone(
                comparer: new ValueComparer<short>(
                    (short v1, short v2) => v1 == v2,
                    (short v) => (int)v,
                    (short v) => v),
                keyComparer: new ValueComparer<short>(
                    (short v1, short v2) => v1 == v2,
                    (short v) => (int)v,
                    (short v) => v),
                providerValueComparer: new ValueComparer<short>(
                    (short v1, short v2) => v1 == v2,
                    (short v) => (int)v,
                    (short v) => v));
            materialPosTypeIndex.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var partslistSequence = runtimeEntityType.AddProperty(
                "PartslistSequence",
                typeof(int),
                propertyInfo: typeof(OrderLogRelView).GetProperty("PartslistSequence", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_PartslistSequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            partslistSequence.TypeMapping = IntTypeMapping.Default.Clone(
                comparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                keyComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                providerValueComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v));
            partslistSequence.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var posActualQuantityUOM = runtimeEntityType.AddProperty(
                "PosActualQuantityUOM",
                typeof(double),
                propertyInfo: typeof(OrderLogRelView).GetProperty("PosActualQuantityUOM", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_PosActualQuantityUOM", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            posActualQuantityUOM.TypeMapping = SqlServerDoubleTypeMapping.Default.Clone(
                comparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v),
                keyComparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v),
                providerValueComparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v));
            posActualQuantityUOM.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var posBatchNo = runtimeEntityType.AddProperty(
                "PosBatchNo",
                typeof(string),
                propertyInfo: typeof(OrderLogRelView).GetProperty("PosBatchNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_PosBatchNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 20,
                unicode: false);
            posBatchNo.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(20)",
                    size: 20));
            posBatchNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var posMaterialName = runtimeEntityType.AddProperty(
                "PosMaterialName",
                typeof(string),
                propertyInfo: typeof(OrderLogRelView).GetProperty("PosMaterialName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_PosMaterialName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 350,
                unicode: false);
            posMaterialName.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(350)",
                    size: 350));
            posMaterialName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var posMaterialNo = runtimeEntityType.AddProperty(
                "PosMaterialNo",
                typeof(string),
                propertyInfo: typeof(OrderLogRelView).GetProperty("PosMaterialNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_PosMaterialNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 30,
                unicode: false);
            posMaterialNo.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(30)",
                    size: 30));
            posMaterialNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var posSequence = runtimeEntityType.AddProperty(
                "PosSequence",
                typeof(int),
                propertyInfo: typeof(OrderLogRelView).GetProperty("PosSequence", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_PosSequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            posSequence.TypeMapping = IntTypeMapping.Default.Clone(
                comparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                keyComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                providerValueComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v));
            posSequence.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var posTargetQuantityUOM = runtimeEntityType.AddProperty(
                "PosTargetQuantityUOM",
                typeof(double),
                propertyInfo: typeof(OrderLogRelView).GetProperty("PosTargetQuantityUOM", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_PosTargetQuantityUOM", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            posTargetQuantityUOM.TypeMapping = SqlServerDoubleTypeMapping.Default.Clone(
                comparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v),
                keyComparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v),
                providerValueComparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v));
            posTargetQuantityUOM.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var prodOrderPartslistPosRelationID = runtimeEntityType.AddProperty(
                "ProdOrderPartslistPosRelationID",
                typeof(Guid?),
                propertyInfo: typeof(OrderLogRelView).GetProperty("ProdOrderPartslistPosRelationID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_ProdOrderPartslistPosRelationID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            prodOrderPartslistPosRelationID.TypeMapping = GuidTypeMapping.Default.Clone(
                comparer: new ValueComparer<Guid?>(
                    (Nullable<Guid> v1, Nullable<Guid> v2) => v1.HasValue && v2.HasValue && (Guid)v1 == (Guid)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<Guid> v) => v.HasValue ? ((Guid)v).GetHashCode() : 0,
                    (Nullable<Guid> v) => v.HasValue ? (Nullable<Guid>)(Guid)v : default(Nullable<Guid>)),
                keyComparer: new ValueComparer<Guid?>(
                    (Nullable<Guid> v1, Nullable<Guid> v2) => v1.HasValue && v2.HasValue && (Guid)v1 == (Guid)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<Guid> v) => v.HasValue ? ((Guid)v).GetHashCode() : 0,
                    (Nullable<Guid> v) => v.HasValue ? (Nullable<Guid>)(Guid)v : default(Nullable<Guid>)),
                providerValueComparer: new ValueComparer<Guid?>(
                    (Nullable<Guid> v1, Nullable<Guid> v2) => v1.HasValue && v2.HasValue && (Guid)v1 == (Guid)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<Guid> v) => v.HasValue ? ((Guid)v).GetHashCode() : 0,
                    (Nullable<Guid> v) => v.HasValue ? (Nullable<Guid>)(Guid)v : default(Nullable<Guid>)),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "uniqueidentifier"));
            prodOrderPartslistPosRelationID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var prodOrderProgramNo = runtimeEntityType.AddProperty(
                "ProdOrderProgramNo",
                typeof(string),
                propertyInfo: typeof(OrderLogRelView).GetProperty("ProdOrderProgramNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_ProdOrderProgramNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            prodOrderProgramNo.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(20)",
                    size: 20));
            prodOrderProgramNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var programNo = runtimeEntityType.AddProperty(
                "ProgramNo",
                typeof(string),
                propertyInfo: typeof(OrderLogRelView).GetProperty("ProgramNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_ProgramNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            programNo.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(20)",
                    size: 20));
            programNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var refACClassID = runtimeEntityType.AddProperty(
                "RefACClassID",
                typeof(Guid?),
                propertyInfo: typeof(OrderLogRelView).GetProperty("RefACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_RefACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            refACClassID.TypeMapping = GuidTypeMapping.Default.Clone(
                comparer: new ValueComparer<Guid?>(
                    (Nullable<Guid> v1, Nullable<Guid> v2) => v1.HasValue && v2.HasValue && (Guid)v1 == (Guid)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<Guid> v) => v.HasValue ? ((Guid)v).GetHashCode() : 0,
                    (Nullable<Guid> v) => v.HasValue ? (Nullable<Guid>)(Guid)v : default(Nullable<Guid>)),
                keyComparer: new ValueComparer<Guid?>(
                    (Nullable<Guid> v1, Nullable<Guid> v2) => v1.HasValue && v2.HasValue && (Guid)v1 == (Guid)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<Guid> v) => v.HasValue ? ((Guid)v).GetHashCode() : 0,
                    (Nullable<Guid> v) => v.HasValue ? (Nullable<Guid>)(Guid)v : default(Nullable<Guid>)),
                providerValueComparer: new ValueComparer<Guid?>(
                    (Nullable<Guid> v1, Nullable<Guid> v2) => v1.HasValue && v2.HasValue && (Guid)v1 == (Guid)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<Guid> v) => v.HasValue ? ((Guid)v).GetHashCode() : 0,
                    (Nullable<Guid> v) => v.HasValue ? (Nullable<Guid>)(Guid)v : default(Nullable<Guid>)),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "uniqueidentifier"));
            refACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var relSequence = runtimeEntityType.AddProperty(
                "RelSequence",
                typeof(int),
                propertyInfo: typeof(OrderLogRelView).GetProperty("RelSequence", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_RelSequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            relSequence.TypeMapping = IntTypeMapping.Default.Clone(
                comparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                keyComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                providerValueComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v));
            relSequence.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetQuantityUOM = runtimeEntityType.AddProperty(
                "TargetQuantityUOM",
                typeof(double),
                propertyInfo: typeof(OrderLogRelView).GetProperty("TargetQuantityUOM", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OrderLogRelView).GetField("_TargetQuantityUOM", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            targetQuantityUOM.TypeMapping = SqlServerDoubleTypeMapping.Default.Clone(
                comparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v),
                keyComparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v),
                providerValueComparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v));
            targetQuantityUOM.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(OrderLogRelView).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewDefinitionSql", null);
            runtimeEntityType.AddAnnotation("Relational:ViewName", "OrderLogRelView");
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}