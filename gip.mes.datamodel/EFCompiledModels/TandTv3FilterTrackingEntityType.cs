﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace gip.mes.datamodel
{
    [EntityFrameworkInternal]
    public partial class TandTv3FilterTrackingEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.TandTv3FilterTracking",
                typeof(TandTv3FilterTracking),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(TandTv3FilterTracking)),
                propertyCount: 11,
                navigationCount: 4,
                servicePropertyCount: 1,
                foreignKeyCount: 2,
                unnamedIndexCount: 2,
                keyCount: 1);

            var tandTv3FilterTrackingID = runtimeEntityType.AddProperty(
                "TandTv3FilterTrackingID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("TandTv3FilterTrackingID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_TandTv3FilterTrackingID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            tandTv3FilterTrackingID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var endTime = runtimeEntityType.AddProperty(
                "EndTime",
                typeof(DateTime?),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("EndTime", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_EndTime", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            endTime.AddAnnotation("Relational:ColumnType", "datetime");
            endTime.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var filterDateFrom = runtimeEntityType.AddProperty(
                "FilterDateFrom",
                typeof(DateTime?),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("FilterDateFrom", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_FilterDateFrom", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            filterDateFrom.AddAnnotation("Relational:ColumnType", "datetime");
            filterDateFrom.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var filterDateTo = runtimeEntityType.AddProperty(
                "FilterDateTo",
                typeof(DateTime?),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("FilterDateTo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_FilterDateTo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            filterDateTo.AddAnnotation("Relational:ColumnType", "datetime");
            filterDateTo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var filterTrackingNo = runtimeEntityType.AddProperty(
                "FilterTrackingNo",
                typeof(string),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("FilterTrackingNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_FilterTrackingNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 50,
                unicode: false);
            filterTrackingNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var itemSystemNo = runtimeEntityType.AddProperty(
                "ItemSystemNo",
                typeof(string),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("ItemSystemNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_ItemSystemNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 50,
                unicode: false);
            itemSystemNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var primaryKeyID = runtimeEntityType.AddProperty(
                "PrimaryKeyID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("PrimaryKeyID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_PrimaryKeyID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            primaryKeyID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var startTime = runtimeEntityType.AddProperty(
                "StartTime",
                typeof(DateTime),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("StartTime", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_StartTime", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            startTime.AddAnnotation("Relational:ColumnType", "datetime");
            startTime.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var tandTv3MDTrackingDirectionID = runtimeEntityType.AddProperty(
                "TandTv3MDTrackingDirectionID",
                typeof(string),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("TandTv3MDTrackingDirectionID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_TandTv3MDTrackingDirectionID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20);
            tandTv3MDTrackingDirectionID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var tandTv3MDTrackingStartItemTypeID = runtimeEntityType.AddProperty(
                "TandTv3MDTrackingStartItemTypeID",
                typeof(string),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("TandTv3MDTrackingStartItemTypeID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_TandTv3MDTrackingStartItemTypeID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 150,
                unicode: false);
            tandTv3MDTrackingStartItemTypeID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { tandTv3FilterTrackingID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { tandTv3MDTrackingDirectionID });

            var index0 = runtimeEntityType.AddIndex(
                new[] { tandTv3MDTrackingStartItemTypeID });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("TandTv3MDTrackingDirectionID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("TandTv3MDTrackingDirectionID") }),
                principalEntityType,
                required: true);

            var tandTv3MDTrackingDirection = declaringEntityType.AddNavigation("TandTv3MDTrackingDirection",
                runtimeForeignKey,
                onDependent: true,
                typeof(TandTv3MDTrackingDirection),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("TandTv3MDTrackingDirection", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_TandTv3MDTrackingDirection", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tandTv3FilterTracking_TandTv3MDTrackingDirection = principalEntityType.AddNavigation("TandTv3FilterTracking_TandTv3MDTrackingDirection",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<TandTv3FilterTracking>),
                propertyInfo: typeof(TandTv3MDTrackingDirection).GetProperty("TandTv3FilterTracking_TandTv3MDTrackingDirection", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MDTrackingDirection).GetField("_TandTv3FilterTracking_TandTv3MDTrackingDirection", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_TandTv3FilterTracking_TandTv3MDTrackingDirectionID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("TandTv3MDTrackingStartItemTypeID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("TandTv3MDTrackingStartItemTypeID") }),
                principalEntityType,
                required: true);

            var tandTv3MDTrackingStartItemType = declaringEntityType.AddNavigation("TandTv3MDTrackingStartItemType",
                runtimeForeignKey,
                onDependent: true,
                typeof(TandTv3MDTrackingStartItemType),
                propertyInfo: typeof(TandTv3FilterTracking).GetProperty("TandTv3MDTrackingStartItemType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3FilterTracking).GetField("_TandTv3MDTrackingStartItemType", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tandTv3FilterTracking_TandTv3MDTrackingStartItemType = principalEntityType.AddNavigation("TandTv3FilterTracking_TandTv3MDTrackingStartItemType",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<TandTv3FilterTracking>),
                propertyInfo: typeof(TandTv3MDTrackingStartItemType).GetProperty("TandTv3FilterTracking_TandTv3MDTrackingStartItemType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MDTrackingStartItemType).GetField("_TandTv3FilterTracking_TandTv3MDTrackingStartItemType", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_TandTv3FilterTracking_TandTv3MDTrackingStartItemTypeID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "TandTv3FilterTracking");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
