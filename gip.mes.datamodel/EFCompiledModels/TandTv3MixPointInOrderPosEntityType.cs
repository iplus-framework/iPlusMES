﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

#pragma warning disable 219, 612, 618
#nullable disable

namespace gip.mes.datamodel
{
    internal partial class TandTv3MixPointInOrderPosEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.TandTv3MixPointInOrderPos",
                typeof(TandTv3MixPointInOrderPos),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(TandTv3MixPointInOrderPos)));

            var tandTv3MixPointInOrderPosID = runtimeEntityType.AddProperty(
                "TandTv3MixPointInOrderPosID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3MixPointInOrderPos).GetProperty("TandTv3MixPointInOrderPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointInOrderPos).GetField("_TandTv3MixPointInOrderPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            tandTv3MixPointInOrderPosID.TypeMapping = GuidTypeMapping.Default.Clone(
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
            tandTv3MixPointInOrderPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inOrderPosID = runtimeEntityType.AddProperty(
                "InOrderPosID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3MixPointInOrderPos).GetProperty("InOrderPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointInOrderPos).GetField("_InOrderPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            inOrderPosID.TypeMapping = GuidTypeMapping.Default.Clone(
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
            inOrderPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var tandTv3MixPointID = runtimeEntityType.AddProperty(
                "TandTv3MixPointID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3MixPointInOrderPos).GetProperty("TandTv3MixPointID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointInOrderPos).GetField("_TandTv3MixPointID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            tandTv3MixPointID.TypeMapping = GuidTypeMapping.Default.Clone(
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
            tandTv3MixPointID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(TandTv3MixPointInOrderPos).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { tandTv3MixPointInOrderPosID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { inOrderPosID });

            var uIX_TandTv3MixPointInOrderPos = runtimeEntityType.AddIndex(
                new[] { tandTv3MixPointID, inOrderPosID },
                name: "UIX_TandTv3MixPointInOrderPos",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("InOrderPosID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("InOrderPosID") }),
                principalEntityType,
                required: true);

            var inOrderPos = declaringEntityType.AddNavigation("InOrderPos",
                runtimeForeignKey,
                onDependent: true,
                typeof(InOrderPos),
                propertyInfo: typeof(TandTv3MixPointInOrderPos).GetProperty("InOrderPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointInOrderPos).GetField("_InOrderPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tandTv3MixPointInOrderPos_InOrderPos = principalEntityType.AddNavigation("TandTv3MixPointInOrderPos_InOrderPos",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<TandTv3MixPointInOrderPos>),
                propertyInfo: typeof(InOrderPos).GetProperty("TandTv3MixPointInOrderPos_InOrderPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InOrderPos).GetField("_TandTv3MixPointInOrderPos_InOrderPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_TandTv3MixPointInOrderPos_InOrderPosID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("TandTv3MixPointID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("TandTv3MixPointID") }),
                principalEntityType,
                required: true);

            var tandTv3MixPoint = declaringEntityType.AddNavigation("TandTv3MixPoint",
                runtimeForeignKey,
                onDependent: true,
                typeof(TandTv3MixPoint),
                propertyInfo: typeof(TandTv3MixPointInOrderPos).GetProperty("TandTv3MixPoint", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointInOrderPos).GetField("_TandTv3MixPoint", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tandTv3MixPointInOrderPos_TandTv3MixPoint = principalEntityType.AddNavigation("TandTv3MixPointInOrderPos_TandTv3MixPoint",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<TandTv3MixPointInOrderPos>),
                propertyInfo: typeof(TandTv3MixPoint).GetProperty("TandTv3MixPointInOrderPos_TandTv3MixPoint", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPoint).GetField("_TandTv3MixPointInOrderPos_TandTv3MixPoint", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_TandTv3MixPointInOrderPos_TandTv3TandTv3MixPointID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "TandTv3MixPointInOrderPos");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}