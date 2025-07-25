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
    public partial class TandTv3MixPointFacilityPreBookingEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.TandTv3MixPointFacilityPreBooking",
                typeof(TandTv3MixPointFacilityPreBooking),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(TandTv3MixPointFacilityPreBooking)),
                propertyCount: 3,
                navigationCount: 2,
                servicePropertyCount: 1,
                foreignKeyCount: 2,
                unnamedIndexCount: 1,
                namedIndexCount: 1,
                keyCount: 1);

            var tandTv3MixPointFacilityPreBookingID = runtimeEntityType.AddProperty(
                "TandTv3MixPointFacilityPreBookingID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3MixPointFacilityPreBooking).GetProperty("TandTv3MixPointFacilityPreBookingID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointFacilityPreBooking).GetField("_TandTv3MixPointFacilityPreBookingID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            tandTv3MixPointFacilityPreBookingID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var facilityPreBookingID = runtimeEntityType.AddProperty(
                "FacilityPreBookingID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3MixPointFacilityPreBooking).GetProperty("FacilityPreBookingID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointFacilityPreBooking).GetField("_FacilityPreBookingID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            facilityPreBookingID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var tandTv3MixPointID = runtimeEntityType.AddProperty(
                "TandTv3MixPointID",
                typeof(Guid),
                propertyInfo: typeof(TandTv3MixPointFacilityPreBooking).GetProperty("TandTv3MixPointID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointFacilityPreBooking).GetField("_TandTv3MixPointID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            tandTv3MixPointID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(TandTv3MixPointFacilityPreBooking).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { tandTv3MixPointFacilityPreBookingID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { facilityPreBookingID });

            var uIX_TandTv3MixPointFacilityPreBooking = runtimeEntityType.AddIndex(
                new[] { tandTv3MixPointID, facilityPreBookingID },
                name: "UIX_TandTv3MixPointFacilityPreBooking",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("FacilityPreBookingID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("FacilityPreBookingID") }),
                principalEntityType,
                required: true);

            var facilityPreBooking = declaringEntityType.AddNavigation("FacilityPreBooking",
                runtimeForeignKey,
                onDependent: true,
                typeof(FacilityPreBooking),
                propertyInfo: typeof(TandTv3MixPointFacilityPreBooking).GetProperty("FacilityPreBooking", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointFacilityPreBooking).GetField("_FacilityPreBooking", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tandTv3MixPointFacilityPreBooking_FacilityPreBooking = principalEntityType.AddNavigation("TandTv3MixPointFacilityPreBooking_FacilityPreBooking",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<TandTv3MixPointFacilityPreBooking>),
                propertyInfo: typeof(FacilityPreBooking).GetProperty("TandTv3MixPointFacilityPreBooking_FacilityPreBooking", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityPreBooking).GetField("_TandTv3MixPointFacilityPreBooking_FacilityPreBooking", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_TandTv3MixPointFacilityPreBooking_FacilityPreBooking");
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
                propertyInfo: typeof(TandTv3MixPointFacilityPreBooking).GetProperty("TandTv3MixPoint", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPointFacilityPreBooking).GetField("_TandTv3MixPoint", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var tandTv3MixPointFacilityPreBooking_TandTv3MixPoint = principalEntityType.AddNavigation("TandTv3MixPointFacilityPreBooking_TandTv3MixPoint",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<TandTv3MixPointFacilityPreBooking>),
                propertyInfo: typeof(TandTv3MixPoint).GetProperty("TandTv3MixPointFacilityPreBooking_TandTv3MixPoint", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TandTv3MixPoint).GetField("_TandTv3MixPointFacilityPreBooking_TandTv3MixPoint", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_TandTv3MixPointFacilityPreBooking_TandTv3MixPoint");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "TandTv3MixPointFacilityPreBooking");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
