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
    internal partial class MachineMaterialRelViewEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.MachineMaterialRelView",
                typeof(MachineMaterialRelView),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(MachineMaterialRelView)));

            var aCClassID = runtimeEntityType.AddProperty(
                "ACClassID",
                typeof(Guid),
                propertyInfo: typeof(MachineMaterialRelView).GetProperty("ACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MachineMaterialRelView).GetField("_ACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var basedOnACClassID = runtimeEntityType.AddProperty(
                "BasedOnACClassID",
                typeof(Guid),
                propertyInfo: typeof(MachineMaterialRelView).GetProperty("BasedOnACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MachineMaterialRelView).GetField("_BasedOnACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            basedOnACClassID.TypeMapping = GuidTypeMapping.Default.Clone(
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
            basedOnACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var basedOnMachineName = runtimeEntityType.AddProperty(
                "BasedOnMachineName",
                typeof(string),
                propertyInfo: typeof(MachineMaterialRelView).GetProperty("BasedOnMachineName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MachineMaterialRelView).GetField("_BasedOnMachineName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 100,
                unicode: false);
            basedOnMachineName.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
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
            basedOnMachineName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var machineName = runtimeEntityType.AddProperty(
                "MachineName",
                typeof(string),
                propertyInfo: typeof(MachineMaterialRelView).GetProperty("MachineName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MachineMaterialRelView).GetField("_MachineName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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
                propertyInfo: typeof(MachineMaterialRelView).GetProperty("MaterialName1", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MachineMaterialRelView).GetField("_MaterialName1", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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
                propertyInfo: typeof(MachineMaterialRelView).GetProperty("MaterialNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MachineMaterialRelView).GetField("_MaterialNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var outwardActualQuantityUOM = runtimeEntityType.AddProperty(
                "OutwardActualQuantityUOM",
                typeof(double?),
                propertyInfo: typeof(MachineMaterialRelView).GetProperty("OutwardActualQuantityUOM", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MachineMaterialRelView).GetField("_OutwardActualQuantityUOM", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            outwardActualQuantityUOM.TypeMapping = SqlServerDoubleTypeMapping.Default.Clone(
                comparer: new ValueComparer<double?>(
                    (Nullable<double> v1, Nullable<double> v2) => v1.HasValue && v2.HasValue && ((double)v1).Equals((double)v2) || !v1.HasValue && !v2.HasValue,
                    (Nullable<double> v) => v.HasValue ? ((double)v).GetHashCode() : 0,
                    (Nullable<double> v) => v.HasValue ? (Nullable<double>)(double)v : default(Nullable<double>)),
                keyComparer: new ValueComparer<double?>(
                    (Nullable<double> v1, Nullable<double> v2) => v1.HasValue && v2.HasValue && ((double)v1).Equals((double)v2) || !v1.HasValue && !v2.HasValue,
                    (Nullable<double> v) => v.HasValue ? ((double)v).GetHashCode() : 0,
                    (Nullable<double> v) => v.HasValue ? (Nullable<double>)(double)v : default(Nullable<double>)),
                providerValueComparer: new ValueComparer<double?>(
                    (Nullable<double> v1, Nullable<double> v2) => v1.HasValue && v2.HasValue && ((double)v1).Equals((double)v2) || !v1.HasValue && !v2.HasValue,
                    (Nullable<double> v) => v.HasValue ? ((double)v).GetHashCode() : 0,
                    (Nullable<double> v) => v.HasValue ? (Nullable<double>)(double)v : default(Nullable<double>)));
            outwardActualQuantityUOM.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var outwardTargetQuantityUOM = runtimeEntityType.AddProperty(
                "OutwardTargetQuantityUOM",
                typeof(double?),
                propertyInfo: typeof(MachineMaterialRelView).GetProperty("OutwardTargetQuantityUOM", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MachineMaterialRelView).GetField("_OutwardTargetQuantityUOM", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            outwardTargetQuantityUOM.TypeMapping = SqlServerDoubleTypeMapping.Default.Clone(
                comparer: new ValueComparer<double?>(
                    (Nullable<double> v1, Nullable<double> v2) => v1.HasValue && v2.HasValue && ((double)v1).Equals((double)v2) || !v1.HasValue && !v2.HasValue,
                    (Nullable<double> v) => v.HasValue ? ((double)v).GetHashCode() : 0,
                    (Nullable<double> v) => v.HasValue ? (Nullable<double>)(double)v : default(Nullable<double>)),
                keyComparer: new ValueComparer<double?>(
                    (Nullable<double> v1, Nullable<double> v2) => v1.HasValue && v2.HasValue && ((double)v1).Equals((double)v2) || !v1.HasValue && !v2.HasValue,
                    (Nullable<double> v) => v.HasValue ? ((double)v).GetHashCode() : 0,
                    (Nullable<double> v) => v.HasValue ? (Nullable<double>)(double)v : default(Nullable<double>)),
                providerValueComparer: new ValueComparer<double?>(
                    (Nullable<double> v1, Nullable<double> v2) => v1.HasValue && v2.HasValue && ((double)v1).Equals((double)v2) || !v1.HasValue && !v2.HasValue,
                    (Nullable<double> v) => v.HasValue ? ((double)v).GetHashCode() : 0,
                    (Nullable<double> v) => v.HasValue ? (Nullable<double>)(double)v : default(Nullable<double>)));
            outwardTargetQuantityUOM.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var programNo = runtimeEntityType.AddProperty(
                "ProgramNo",
                typeof(string),
                propertyInfo: typeof(MachineMaterialRelView).GetProperty("ProgramNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MachineMaterialRelView).GetField("_ProgramNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var sequence = runtimeEntityType.AddProperty(
                "Sequence",
                typeof(int),
                propertyInfo: typeof(MachineMaterialRelView).GetProperty("Sequence", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MachineMaterialRelView).GetField("_Sequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            sequence.TypeMapping = IntTypeMapping.Default.Clone(
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
            sequence.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(MachineMaterialRelView).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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
            runtimeEntityType.AddAnnotation("Relational:ViewName", "MachineMaterialRelView");
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
