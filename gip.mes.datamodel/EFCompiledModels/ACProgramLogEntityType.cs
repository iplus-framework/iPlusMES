﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using gip.core.datamodel;

#pragma warning disable 219, 612, 618
#nullable disable

namespace gip.mes.datamodel
{
    internal partial class ACProgramLogEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.ACProgramLog",
                typeof(ACProgramLog),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(ACProgramLog)));

            var aCProgramLogID = runtimeEntityType.AddProperty(
                "ACProgramLogID",
                typeof(Guid),
                propertyInfo: typeof(ACProgramLog).GetProperty("ACProgramLogID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_ACProgramLogID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
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

            var aCClassID = runtimeEntityType.AddProperty(
                "ACClassID",
                typeof(Guid?),
                propertyInfo: typeof(ACProgramLog).GetProperty("ACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_ACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            aCClassID.TypeMapping = GuidTypeMapping.Default.Clone(
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
            aCClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var aCProgramID = runtimeEntityType.AddProperty(
                "ACProgramID",
                typeof(Guid),
                propertyInfo: typeof(ACProgramLog).GetProperty("ACProgramID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_ACProgramID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            aCProgramID.TypeMapping = GuidTypeMapping.Default.Clone(
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
            aCProgramID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var aCUrl = runtimeEntityType.AddProperty(
                "ACUrl",
                typeof(string),
                propertyInfo: typeof(ACProgramLog).GetProperty("ACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_ACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var durationSec = runtimeEntityType.AddProperty(
                "DurationSec",
                typeof(double),
                propertyInfo: typeof(ACProgramLog).GetProperty("DurationSec", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_DurationSec", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            durationSec.TypeMapping = SqlServerDoubleTypeMapping.Default.Clone(
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
            durationSec.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var durationSecPlan = runtimeEntityType.AddProperty(
                "DurationSecPlan",
                typeof(double),
                propertyInfo: typeof(ACProgramLog).GetProperty("DurationSecPlan", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_DurationSecPlan", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            durationSecPlan.TypeMapping = SqlServerDoubleTypeMapping.Default.Clone(
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
            durationSecPlan.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var endDate = runtimeEntityType.AddProperty(
                "EndDate",
                typeof(DateTime?),
                propertyInfo: typeof(ACProgramLog).GetProperty("EndDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_EndDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            endDate.TypeMapping = SqlServerDateTimeTypeMapping.Default.Clone(
                comparer: new ValueComparer<DateTime?>(
                    (Nullable<DateTime> v1, Nullable<DateTime> v2) => v1.HasValue && v2.HasValue && (DateTime)v1 == (DateTime)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<DateTime> v) => v.HasValue ? ((DateTime)v).GetHashCode() : 0,
                    (Nullable<DateTime> v) => v.HasValue ? (Nullable<DateTime>)(DateTime)v : default(Nullable<DateTime>)),
                keyComparer: new ValueComparer<DateTime?>(
                    (Nullable<DateTime> v1, Nullable<DateTime> v2) => v1.HasValue && v2.HasValue && (DateTime)v1 == (DateTime)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<DateTime> v) => v.HasValue ? ((DateTime)v).GetHashCode() : 0,
                    (Nullable<DateTime> v) => v.HasValue ? (Nullable<DateTime>)(DateTime)v : default(Nullable<DateTime>)),
                providerValueComparer: new ValueComparer<DateTime?>(
                    (Nullable<DateTime> v1, Nullable<DateTime> v2) => v1.HasValue && v2.HasValue && (DateTime)v1 == (DateTime)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<DateTime> v) => v.HasValue ? ((DateTime)v).GetHashCode() : 0,
                    (Nullable<DateTime> v) => v.HasValue ? (Nullable<DateTime>)(DateTime)v : default(Nullable<DateTime>)),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "datetime",
                    dbType: System.Data.DbType.DateTime));
            endDate.AddAnnotation("Relational:ColumnType", "datetime");
            endDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var endDatePlan = runtimeEntityType.AddProperty(
                "EndDatePlan",
                typeof(DateTime),
                propertyInfo: typeof(ACProgramLog).GetProperty("EndDatePlan", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_EndDatePlan", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            endDatePlan.TypeMapping = SqlServerDateTimeTypeMapping.Default.Clone(
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
            endDatePlan.AddAnnotation("Relational:ColumnType", "datetime");
            endDatePlan.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(ACProgramLog).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(ACProgramLog).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
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
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var message = runtimeEntityType.AddProperty(
                "Message",
                typeof(string),
                propertyInfo: typeof(ACProgramLog).GetProperty("Message", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_Message", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            message.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
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
                    storeTypeName: "varchar(max)"),
                storeTypePostfix: StoreTypePostfix.None);
            message.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var parentACProgramLogID = runtimeEntityType.AddProperty(
                "ParentACProgramLogID",
                typeof(Guid?),
                propertyInfo: typeof(ACProgramLog).GetProperty("ParentACProgramLogID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_ParentACProgramLogID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            parentACProgramLogID.TypeMapping = GuidTypeMapping.Default.Clone(
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
            parentACProgramLogID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var refACClassID = runtimeEntityType.AddProperty(
                "RefACClassID",
                typeof(Guid?),
                propertyInfo: typeof(ACProgramLog).GetProperty("RefACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_RefACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var startDate = runtimeEntityType.AddProperty(
                "StartDate",
                typeof(DateTime?),
                propertyInfo: typeof(ACProgramLog).GetProperty("StartDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_StartDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            startDate.TypeMapping = SqlServerDateTimeTypeMapping.Default.Clone(
                comparer: new ValueComparer<DateTime?>(
                    (Nullable<DateTime> v1, Nullable<DateTime> v2) => v1.HasValue && v2.HasValue && (DateTime)v1 == (DateTime)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<DateTime> v) => v.HasValue ? ((DateTime)v).GetHashCode() : 0,
                    (Nullable<DateTime> v) => v.HasValue ? (Nullable<DateTime>)(DateTime)v : default(Nullable<DateTime>)),
                keyComparer: new ValueComparer<DateTime?>(
                    (Nullable<DateTime> v1, Nullable<DateTime> v2) => v1.HasValue && v2.HasValue && (DateTime)v1 == (DateTime)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<DateTime> v) => v.HasValue ? ((DateTime)v).GetHashCode() : 0,
                    (Nullable<DateTime> v) => v.HasValue ? (Nullable<DateTime>)(DateTime)v : default(Nullable<DateTime>)),
                providerValueComparer: new ValueComparer<DateTime?>(
                    (Nullable<DateTime> v1, Nullable<DateTime> v2) => v1.HasValue && v2.HasValue && (DateTime)v1 == (DateTime)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<DateTime> v) => v.HasValue ? ((DateTime)v).GetHashCode() : 0,
                    (Nullable<DateTime> v) => v.HasValue ? (Nullable<DateTime>)(DateTime)v : default(Nullable<DateTime>)),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "datetime",
                    dbType: System.Data.DbType.DateTime));
            startDate.AddAnnotation("Relational:ColumnType", "datetime");
            startDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var startDatePlan = runtimeEntityType.AddProperty(
                "StartDatePlan",
                typeof(DateTime),
                propertyInfo: typeof(ACProgramLog).GetProperty("StartDatePlan", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_StartDatePlan", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            startDatePlan.TypeMapping = SqlServerDateTimeTypeMapping.Default.Clone(
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
            startDatePlan.AddAnnotation("Relational:ColumnType", "datetime");
            startDatePlan.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(ACProgramLog).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            updateDate.TypeMapping = SqlServerDateTimeTypeMapping.Default.Clone(
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
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(ACProgramLog).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
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
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            xMLConfig.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
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
                    storeTypeName: "text"),
                storeTypePostfix: StoreTypePostfix.None);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(ACProgramLog).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { aCProgramLogID });
            runtimeEntityType.SetPrimaryKey(key);

            var nCI_FK_ACProgramLog_ACProgramID = runtimeEntityType.AddIndex(
                new[] { aCProgramID },
                name: "NCI_FK_ACProgramLog_ACProgramID");

            var nCI_FK_ACProgramLog_ParentACProgramLogID = runtimeEntityType.AddIndex(
                new[] { parentACProgramLogID },
                name: "NCI_FK_ACProgramLog_ParentACProgramLogID");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ACProgramID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACProgramID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var aCProgram = declaringEntityType.AddNavigation("ACProgram",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACProgram),
                propertyInfo: typeof(ACProgramLog).GetProperty("ACProgram", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_ACProgram", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var aCProgramLog_ACProgram = principalEntityType.AddNavigation("ACProgramLog_ACProgram",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ACProgramLog>),
                propertyInfo: typeof(ACProgram).GetProperty("ACProgramLog_ACProgram", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgram).GetField("_ACProgramLog_ACProgram", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ACProgramLog_ACProgramID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ParentACProgramLogID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACProgramLogID") }),
                principalEntityType);

            var aCProgramLog1_ParentACProgramLog = declaringEntityType.AddNavigation("ACProgramLog1_ParentACProgramLog",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACProgramLog),
                propertyInfo: typeof(ACProgramLog).GetProperty("ACProgramLog1_ParentACProgramLog", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_ACProgramLog1_ParentACProgramLog", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var aCProgramLog_ParentACProgramLog = principalEntityType.AddNavigation("ACProgramLog_ParentACProgramLog",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ACProgramLog>),
                propertyInfo: typeof(ACProgramLog).GetProperty("ACProgramLog_ParentACProgramLog", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgramLog).GetField("_ACProgramLog_ParentACProgramLog", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ACProgramLog_ParentACProgramLogID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "ACProgramLog");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}