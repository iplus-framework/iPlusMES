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
    internal partial class TandTv3MixPointOutOrderPosEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.TandTv3MixPointOutOrderPos",
                typeof(TandTv3MixPointOutOrderPos),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(TandTv3MixPointOutOrderPos)));

            var tandTv3MixPointOutOrderPosID = runtimeEntityType.AddProperty(
                "TandTv3MixPointOutOrderPosID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3MixPointOutOrderPos).GetProperty("TandTv3MixPointOutOrderPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointOutOrderPos).GetField("_TandTv3MixPointOutOrderPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            tandTv3MixPointOutOrderPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var outOrderPosID = runtimeEntityType.AddProperty(
                "OutOrderPosID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3MixPointOutOrderPos).GetProperty("OutOrderPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointOutOrderPos).GetField("_OutOrderPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            outOrderPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var tandTv3MixPointID = runtimeEntityType.AddProperty(
                "TandTv3MixPointID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3MixPointOutOrderPos).GetProperty("TandTv3MixPointID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointOutOrderPos).GetField("_TandTv3MixPointID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            tandTv3MixPointID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(TandTv3MixPointOutOrderPos).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { tandTv3MixPointOutOrderPosID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { outOrderPosID });

            var uIXTandTv3MixPointOutOrderPos = runtimeEntityType.AddIndex(
                new[] { tandTv3MixPointID, outOrderPosID },
                name: "UIX_TandTv3MixPointOutOrderPos",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("OutOrderPosID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("OutOrderPosID") }),
                principalEntityType,
                required: true);

            var outOrderPos = declaringEntityType.AddNavigation("OutOrderPos",
                runtimeForeignKey,
                onDependent: true,
                typeof(OutOrderPos),
                propertyInfo: typeof(TandTv3MixPointOutOrderPos).GetProperty("OutOrderPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointOutOrderPos).GetField("_OutOrderPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tandTv3MixPointOutOrderPosOutOrderPos = principalEntityType.AddNavigation("TandTv3MixPointOutOrderPos_OutOrderPos",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<TandTv3MixPointOutOrderPos>),
                propertyInfo: typeof(OutOrderPos).GetProperty("TandTv3MixPointOutOrderPos_OutOrderPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOrderPos).GetField("_TandTv3MixPointOutOrderPos_OutOrderPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_TandTv3MixPointOutOrderPos_OutOrderPosID");
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
                propertyInfo: typeof(TandTv3MixPointOutOrderPos).GetProperty("TandTv3MixPoint", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointOutOrderPos).GetField("_TandTv3MixPoint", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tandTv3MixPointOutOrderPosTandTv3MixPoint = principalEntityType.AddNavigation("TandTv3MixPointOutOrderPos_TandTv3MixPoint",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<TandTv3MixPointOutOrderPos>),
                propertyInfo: typeof(TandTv3MixPoint).GetProperty("TandTv3MixPointOutOrderPos_TandTv3MixPoint", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPoint).GetField("_TandTv3MixPointOutOrderPos_TandTv3MixPoint", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_TandTv3MixPointOutOrderPos_TandTv3TandTv3MixPointID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "TandTv3MixPointOutOrderPos");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}