﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace gip.mes.datamodel
{
    internal partial class TandTv3MixPointPickingPosEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.TandTv3MixPointPickingPos",
                typeof(TandTv3MixPointPickingPos),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(TandTv3MixPointPickingPos)));

            var tandTv3MixPointPickingPosID = runtimeEntityType.AddProperty(
                "TandTv3MixPointPickingPosID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3MixPointPickingPos).GetProperty("TandTv3MixPointPickingPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointPickingPos).GetField("_TandTv3MixPointPickingPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            tandTv3MixPointPickingPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var pickingPosID = runtimeEntityType.AddProperty(
                "PickingPosID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3MixPointPickingPos).GetProperty("PickingPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointPickingPos).GetField("_PickingPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            pickingPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var tandTv3MixPointID = runtimeEntityType.AddProperty(
                "TandTv3MixPointID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3MixPointPickingPos).GetProperty("TandTv3MixPointID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointPickingPos).GetField("_TandTv3MixPointID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            tandTv3MixPointID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(TandTv3MixPointPickingPos).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { tandTv3MixPointPickingPosID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { pickingPosID });

            var index0 = runtimeEntityType.AddIndex(
                new[] { tandTv3MixPointID });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("PickingPosID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("PickingPosID") }),
                principalEntityType,
                required: true);

            var pickingPos = declaringEntityType.AddNavigation("PickingPos",
                runtimeForeignKey,
                onDependent: true,
                typeof(PickingPos),
                propertyInfo: typeof(TandTv3MixPointPickingPos).GetProperty("PickingPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointPickingPos).GetField("_PickingPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tandTv3MixPointPickingPosPickingPos = principalEntityType.AddNavigation("TandTv3MixPointPickingPos_PickingPos",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<TandTv3MixPointPickingPos>),
                propertyInfo: typeof(PickingPos).GetProperty("TandTv3MixPointPickingPos_PickingPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingPos).GetField("_TandTv3MixPointPickingPos_PickingPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_TandTv3MixPointPickingPos_PickingPosID");
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
                propertyInfo: typeof(TandTv3MixPointPickingPos).GetProperty("TandTv3MixPoint", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointPickingPos).GetField("_TandTv3MixPoint", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tandTv3MixPointPickingPosTandTv3MixPoint = principalEntityType.AddNavigation("TandTv3MixPointPickingPos_TandTv3MixPoint",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<TandTv3MixPointPickingPos>),
                propertyInfo: typeof(TandTv3MixPoint).GetProperty("TandTv3MixPointPickingPos_TandTv3MixPoint", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPoint).GetField("_TandTv3MixPointPickingPos_TandTv3MixPoint", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_TandTv3MixPointPickingPos_TandTv3TandTv3MixPointID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "TandTv3MixPointPickingPos");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}