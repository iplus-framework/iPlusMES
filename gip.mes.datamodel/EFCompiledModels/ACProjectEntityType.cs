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
    internal partial class ACProjectEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.ACProject",
                typeof(ACProject),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(ACProject)));

            var aCProjectID = runtimeEntityType.AddProperty(
                "ACProjectID",
                typeof(Guid),
                propertyInfo: typeof(ACProject).GetProperty("ACProjectID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_ACProjectID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            aCProjectID.TypeMapping = GuidTypeMapping.Default.Clone(
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
            aCProjectID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var aCProjectName = runtimeEntityType.AddProperty(
                "ACProjectName",
                typeof(string),
                propertyInfo: typeof(ACProject).GetProperty("ACProjectName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_ACProjectName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 50,
                unicode: false);
            aCProjectName.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
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
                    storeTypeName: "varchar(50)",
                    size: 50));
            aCProjectName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var aCProjectNo = runtimeEntityType.AddProperty(
                "ACProjectNo",
                typeof(string),
                propertyInfo: typeof(ACProject).GetProperty("ACProjectNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_ACProjectNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            aCProjectNo.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
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
            aCProjectNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var aCProjectTypeIndex = runtimeEntityType.AddProperty(
                "ACProjectTypeIndex",
                typeof(short),
                propertyInfo: typeof(ACProject).GetProperty("ACProjectTypeIndex", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_ACProjectTypeIndex", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: (short)0);
            aCProjectTypeIndex.TypeMapping = SqlServerShortTypeMapping.Default.Clone(
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
            aCProjectTypeIndex.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var basedOnACProjectID = runtimeEntityType.AddProperty(
                "BasedOnACProjectID",
                typeof(Guid?),
                propertyInfo: typeof(ACProject).GetProperty("BasedOnACProjectID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_BasedOnACProjectID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            basedOnACProjectID.TypeMapping = GuidTypeMapping.Default.Clone(
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
            basedOnACProjectID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment = runtimeEntityType.AddProperty(
                "Comment",
                typeof(string),
                propertyInfo: typeof(ACProject).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_Comment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
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
            comment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(ACProject).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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
                propertyInfo: typeof(ACProject).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var isControlCenterEnabled = runtimeEntityType.AddProperty(
                "IsControlCenterEnabled",
                typeof(bool),
                propertyInfo: typeof(ACProject).GetProperty("IsControlCenterEnabled", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_IsControlCenterEnabled", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isControlCenterEnabled.TypeMapping = SqlServerBoolTypeMapping.Default.Clone(
                comparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                keyComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                providerValueComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v));
            isControlCenterEnabled.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isDataAccess = runtimeEntityType.AddProperty(
                "IsDataAccess",
                typeof(bool),
                propertyInfo: typeof(ACProject).GetProperty("IsDataAccess", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_IsDataAccess", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isDataAccess.TypeMapping = SqlServerBoolTypeMapping.Default.Clone(
                comparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                keyComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                providerValueComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v));
            isDataAccess.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isEnabled = runtimeEntityType.AddProperty(
                "IsEnabled",
                typeof(bool),
                propertyInfo: typeof(ACProject).GetProperty("IsEnabled", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_IsEnabled", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isEnabled.TypeMapping = SqlServerBoolTypeMapping.Default.Clone(
                comparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                keyComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                providerValueComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v));
            isEnabled.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isGlobal = runtimeEntityType.AddProperty(
                "IsGlobal",
                typeof(bool),
                propertyInfo: typeof(ACProject).GetProperty("IsGlobal", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_IsGlobal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isGlobal.TypeMapping = SqlServerBoolTypeMapping.Default.Clone(
                comparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                keyComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                providerValueComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v));
            isGlobal.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isProduction = runtimeEntityType.AddProperty(
                "IsProduction",
                typeof(bool),
                propertyInfo: typeof(ACProject).GetProperty("IsProduction", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_IsProduction", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isProduction.TypeMapping = SqlServerBoolTypeMapping.Default.Clone(
                comparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                keyComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                providerValueComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v));
            isProduction.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isVisualisationEnabled = runtimeEntityType.AddProperty(
                "IsVisualisationEnabled",
                typeof(bool),
                propertyInfo: typeof(ACProject).GetProperty("IsVisualisationEnabled", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_IsVisualisationEnabled", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isVisualisationEnabled.TypeMapping = SqlServerBoolTypeMapping.Default.Clone(
                comparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                keyComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                providerValueComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v));
            isVisualisationEnabled.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isWorkflowEnabled = runtimeEntityType.AddProperty(
                "IsWorkflowEnabled",
                typeof(bool),
                propertyInfo: typeof(ACProject).GetProperty("IsWorkflowEnabled", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_IsWorkflowEnabled", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isWorkflowEnabled.TypeMapping = SqlServerBoolTypeMapping.Default.Clone(
                comparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                keyComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                providerValueComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v));
            isWorkflowEnabled.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var pAAppClassAssignmentACClassID = runtimeEntityType.AddProperty(
                "PAAppClassAssignmentACClassID",
                typeof(Guid?),
                propertyInfo: typeof(ACProject).GetProperty("PAAppClassAssignmentACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_PAAppClassAssignmentACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            pAAppClassAssignmentACClassID.TypeMapping = GuidTypeMapping.Default.Clone(
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
            pAAppClassAssignmentACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(ACProject).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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
                propertyInfo: typeof(ACProject).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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
                fieldInfo: typeof(ACProject).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
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
                propertyInfo: typeof(ACProject).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { aCProjectID });
            runtimeEntityType.SetPrimaryKey(key);

            var nCI_FK_ACProject_BasedOnACProjectID = runtimeEntityType.AddIndex(
                new[] { basedOnACProjectID },
                name: "NCI_FK_ACProject_BasedOnACProjectID");

            var nCI_FK_ACProject_PAAppClassAssignmentACClassID = runtimeEntityType.AddIndex(
                new[] { pAAppClassAssignmentACClassID },
                name: "NCI_FK_ACProject_PAAppClassAssignmentACClassID");

            var uIX_ACProject_ACProjectNo = runtimeEntityType.AddIndex(
                new[] { aCProjectNo },
                name: "UIX_ACProject_ACProjectNo",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("BasedOnACProjectID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACProjectID") }),
                principalEntityType);

            var aCProject1_BasedOnACProject = declaringEntityType.AddNavigation("ACProject1_BasedOnACProject",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACProject),
                propertyInfo: typeof(ACProject).GetProperty("ACProject1_BasedOnACProject", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_ACProject1_BasedOnACProject", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var aCProject_BasedOnACProject = principalEntityType.AddNavigation("ACProject_BasedOnACProject",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ACProject>),
                propertyInfo: typeof(ACProject).GetProperty("ACProject_BasedOnACProject", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_ACProject_BasedOnACProject", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ACProject_BasedOnACProjectID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("PAAppClassAssignmentACClassID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACClassID") }),
                principalEntityType);

            var pAAppClassAssignmentACClass = declaringEntityType.AddNavigation("PAAppClassAssignmentACClass",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACClass),
                propertyInfo: typeof(ACProject).GetProperty("PAAppClassAssignmentACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProject).GetField("_PAAppClassAssignmentACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var aCProject_PAAppClassAssignmentACClass = principalEntityType.AddNavigation("ACProject_PAAppClassAssignmentACClass",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ACProject>),
                propertyInfo: typeof(ACClass).GetProperty("ACProject_PAAppClassAssignmentACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClass).GetField("_ACProject_PAAppClassAssignmentACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ACProject_PAAppClassAssignmentACClassID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "ACProject");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}